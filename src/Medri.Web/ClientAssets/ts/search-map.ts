interface SearchMapMarker {
    id: string;
    label: string;
    title: string;
    tag: string;
    priceLabel: string;
    displayLocation: string;
    factsLabel: string;
    imageUrl: string;
    latitude: number;
    longitude: number;
    detailUrl: string;
}

interface SearchMapConfig {
    apiKey: string;
    mapId: string;
    isConfigured: boolean;
    selectedId?: string;
    markers: SearchMapMarker[];
}

interface SearchMapMarkerEntry {
    marker: SearchMapMarker;
    advancedMarker: unknown;
    content: HTMLElement;
}

interface SearchMapInstance {
    map: any;
    markerEntries: SearchMapMarkerEntry[];
    mapElement: HTMLElement;
}

interface MedriWindow extends Window {
    google?: any;
    __medriSearchMapReady?: () => void;
}

interface VueGlobal {
    createApp(options: any): {
        mount(selectorOrElement: string | Element): unknown;
    };
}

declare const Vue: VueGlobal | undefined;
declare const google: any;

(() => {
    const configNode = document.getElementById("search-map-config");
    const root = document.querySelector("[data-search-map-root]");
    const mountNode = document.querySelector("[data-search-map-vue-controller]");

    if (!configNode || !root || !mountNode || typeof Vue === "undefined") {
        return;
    }

    const config = JSON.parse(configNode.textContent || "{}") as SearchMapConfig;
    config.markers = Array.isArray(config.markers) ? config.markers : [];
    const googleMapsScriptId = "google-maps-javascript-api";

    function loadGoogleMaps(apiKey: string): Promise<void> {
        const browserWindow = window as MedriWindow;
        if (browserWindow.google?.maps?.importLibrary) {
            return Promise.resolve();
        }

        return new Promise((resolve, reject) => {
            const existingScript = document.getElementById(googleMapsScriptId) as HTMLScriptElement | null;
            const callbackName = "__medriSearchMapReady";

            browserWindow[callbackName] = () => resolve();

            if (existingScript) {
                existingScript.addEventListener("error", () => reject(new Error("caricamento-mappa-non-riuscito")), { once: true });
                return;
            }

            const script = document.createElement("script");
            script.id = googleMapsScriptId;
            script.async = true;
            script.defer = true;
            script.src =
                "https://maps.googleapis.com/maps/api/js?key=" +
                encodeURIComponent(apiKey) +
                "&v=weekly&libraries=marker&callback=" +
                encodeURIComponent(callbackName);
            script.onerror = () => reject(new Error("caricamento-mappa-non-riuscito"));
            document.head.appendChild(script);
        });
    }

    Vue.createApp({
        data() {
            return {
                config,
                maps: [] as SearchMapInstance[],
                selectedId: config.selectedId || "",
                previewPositionFrame: 0
            };
        },
        mounted(this: any) {
            this.bindCardSelection();
            this.bindZoomControls();
            this.bindPreviewCloseButtons();

            if (!this.config.isConfigured) {
                this.showFallback();
                return;
            }

            this.initializeMaps();
        },
        methods: {
            async initializeMaps(this: any): Promise<void> {
                try {
                    await loadGoogleMaps(this.config.apiKey);
                    const mapsLibrary = await google.maps.importLibrary("maps");
                    const markerLibrary = await google.maps.importLibrary("marker");
                    const Map = mapsLibrary.Map;
                    const AdvancedMarkerElement = markerLibrary.AdvancedMarkerElement;

                    document.querySelectorAll<HTMLElement>("[data-google-map]").forEach((element) => {
                        const map = new Map(element, {
                            center: this.initialCenter(),
                            zoom: 13,
                            mapId: this.config.mapId,
                            disableDefaultUI: true,
                            clickableIcons: false
                        });

                        const markerEntries = this.config.markers.map((marker: SearchMapMarker) => {
                            const content = this.createMarkerContent(marker);
                            const advancedMarker = new AdvancedMarkerElement({
                                map,
                                position: { lat: marker.latitude, lng: marker.longitude },
                                title: marker.title,
                                content
                            });

                            content.addEventListener("click", () => this.selectMarker(marker.id, map));
                            return { marker, advancedMarker, content };
                        });

                        const selectedMarker = this.selectedMarker();
                        if (!selectedMarker && this.config.markers.length > 1) {
                            const bounds = new google.maps.LatLngBounds();
                            this.config.markers.forEach((marker: SearchMapMarker) => {
                                bounds.extend({ lat: marker.latitude, lng: marker.longitude });
                            });
                            map.fitBounds(bounds, 56);
                        }

                        this.maps.push({ map, markerEntries, mapElement: element });
                        ["bounds_changed", "drag", "dragend", "zoom_changed", "idle"].forEach((eventName) => {
                            map.addListener(eventName, () => this.schedulePreviewPosition());
                        });

                        if (selectedMarker) {
                            this.selectMarker(selectedMarker.id, map);
                        } else {
                            this.syncSelectedMarker();
                        }
                    });
                } catch {
                    this.showFallback();
                }
            },
            initialCenter(this: any): { lat: number; lng: number } {
                const selected = this.selectedMarker() || this.config.markers[0];
                return selected
                    ? { lat: selected.latitude, lng: selected.longitude }
                    : { lat: 44.1396, lng: 12.2431 };
            },
            createMarkerContent(marker: SearchMapMarker): HTMLElement {
                const content = document.createElement("button");
                content.type = "button";
                content.className = "map-marker-pin";
                content.textContent = marker.label;
                content.setAttribute("aria-label", marker.title);
                content.dataset.markerId = marker.id;
                return content;
            },
            bindCardSelection(this: any): void {
                document.querySelectorAll<HTMLElement>("[data-map-card-id]").forEach((card) => {
                    card.addEventListener("click", (event) => {
                        if ((event.target as HTMLElement).closest("button, a")) {
                            return;
                        }

                        this.selectMarker(card.dataset.mapCardId || "");
                    });
                });
            },
            bindZoomControls(this: any): void {
                document.querySelectorAll("[data-map-zoom-in]").forEach((button) => {
                    button.addEventListener("click", () => this.zoomBy(1));
                });
                document.querySelectorAll("[data-map-zoom-out]").forEach((button) => {
                    button.addEventListener("click", () => this.zoomBy(-1));
                });
            },
            bindPreviewCloseButtons(this: any): void {
                document.querySelectorAll("[data-map-preview-close]").forEach((button) => {
                    button.addEventListener("click", (event) => {
                        event.preventDefault();
                        event.stopPropagation();
                        this.selectedId = "";
                        this.hidePreview();
                        this.syncSelectedMarker();
                        this.syncSelectedCard();
                    });
                });
            },
            zoomBy(this: any, delta: number): void {
                this.maps.forEach((entry: SearchMapInstance) => {
                    const zoom = entry.map.getZoom();
                    if (typeof zoom === "number") {
                        entry.map.setZoom(zoom + delta);
                    }
                });
            },
            selectMarker(this: any, id: string, sourceMap?: any): void {
                if (!id) {
                    return;
                }

                this.selectedId = id;
                const selected = this.selectedMarker();
                this.updatePreview(selected);
                this.syncSelectedMarker();
                this.syncSelectedCard();

                if (!selected) {
                    return;
                }

                const target = { lat: selected.latitude, lng: selected.longitude };
                this.maps.forEach((entry: SearchMapInstance) => {
                    entry.map.panTo(target);
                });

                if (sourceMap) {
                    sourceMap.panTo(target);
                }

                this.positionPreview(selected.id);
                this.schedulePreviewPosition();
                window.setTimeout(() => this.positionPreview(selected.id), 180);
            },
            selectedMarker(this: any): SearchMapMarker | null {
                return this.selectedId
                    ? this.config.markers.find((marker: SearchMapMarker) => marker.id === this.selectedId) || null
                    : null;
            },
            updatePreview(marker: SearchMapMarker | null): void {
                if (!marker) {
                    return;
                }

                document.querySelectorAll("[data-map-preview-title]").forEach((node) => {
                    node.textContent = marker.title;
                });
                document.querySelectorAll("[data-map-preview-location]").forEach((node) => {
                    node.textContent = marker.displayLocation;
                });
                document.querySelectorAll("[data-map-preview-facts]").forEach((node) => {
                    node.textContent = marker.factsLabel;
                });
                document.querySelectorAll("[data-map-preview-price]").forEach((node) => {
                    node.textContent = marker.priceLabel;
                });
                document.querySelectorAll("[data-map-preview-tag]").forEach((node) => {
                    node.textContent = marker.tag;
                });
                document.querySelectorAll<HTMLAnchorElement>("[data-map-preview-link]").forEach((link) => {
                    link.href = marker.detailUrl;
                    link.setAttribute("aria-label", "Apri " + marker.title);
                });
                document.querySelectorAll<HTMLElement>("[data-map-preview-image]").forEach((node) => {
                    node.style.backgroundImage = marker.imageUrl ? `url("${marker.imageUrl}")` : "";
                });
                document.querySelectorAll<HTMLElement>(".map-card[data-map-preview]").forEach((node) => {
                    node.hidden = false;
                    node.removeAttribute("hidden");
                    node.classList.add("is-visible");
                    node.style.display = "block";
                    node.style.opacity = "1";
                    node.style.visibility = "visible";
                });
            },
            hidePreview(): void {
                document.querySelectorAll<HTMLElement>(".map-card[data-map-preview]").forEach((node) => {
                    node.hidden = true;
                    node.setAttribute("hidden", "hidden");
                    node.classList.remove("is-visible");
                    node.removeAttribute("style");
                });
            },
            schedulePreviewPosition(this: any): void {
                if (!this.selectedId) {
                    return;
                }

                if (this.previewPositionFrame) {
                    window.cancelAnimationFrame(this.previewPositionFrame);
                }

                this.previewPositionFrame = window.requestAnimationFrame(() => {
                    this.previewPositionFrame = 0;
                    this.positionPreview(this.selectedId);
                });
            },
            positionPreview(this: any, markerId: string): void {
                const preview = document.querySelector<HTMLElement>(".map-card[data-map-preview]");
                if (!preview || preview.hidden) {
                    return;
                }

                const markerEntry = this.findMarkerEntry(markerId);
                if (!markerEntry) {
                    return;
                }

                const mapCanvas = markerEntry.mapElement.closest("[data-map-canvas]") as HTMLElement | null;
                if (!mapCanvas) {
                    return;
                }

                const markerRect = markerEntry.markerItem.content.getBoundingClientRect();
                const canvasRect = mapCanvas.getBoundingClientRect();
                const previewRect = preview.getBoundingClientRect();
                const rootFontSize = Number.parseFloat(window.getComputedStyle(document.documentElement).fontSize) || 16;
                const gap = rootFontSize * 0.75;
                const markerCenter = markerRect.left + markerRect.width / 2 - canvasRect.left;
                const markerTop = markerRect.top - canvasRect.top;

                preview.style.left = `${markerCenter}px`;
                preview.style.top = `${markerTop - gap - previewRect.height}px`;
                preview.style.right = "auto";
                preview.style.bottom = "auto";
                preview.style.transform = "translateX(-50%)";
            },
            findMarkerEntry(this: any, markerId: string): { mapElement: HTMLElement; markerItem: SearchMapMarkerEntry } | null {
                for (const entry of this.maps as SearchMapInstance[]) {
                    const markerItem = entry.markerEntries.find((item) => item.marker.id === markerId);
                    if (markerItem) {
                        return { mapElement: entry.mapElement, markerItem };
                    }
                }

                return null;
            },
            syncSelectedMarker(this: any): void {
                this.maps.forEach((entry: SearchMapInstance) => {
                    entry.markerEntries.forEach((item) => {
                        item.content.classList.toggle("is-selected", item.marker.id === this.selectedId);
                    });
                });
            },
            syncSelectedCard(this: any): void {
                document.querySelectorAll<HTMLElement>("[data-map-card-id]").forEach((card) => {
                    card.classList.toggle("is-map-selected", card.dataset.mapCardId === this.selectedId);
                });
            },
            showFallback(): void {
                document.querySelectorAll<HTMLElement>("[data-map-fallback]").forEach((fallback) => {
                    fallback.hidden = false;
                });
            }
        }
    }).mount(mountNode);
})();
