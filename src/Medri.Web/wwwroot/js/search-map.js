"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g = Object.create((typeof Iterator === "function" ? Iterator : Object).prototype);
    return g.next = verb(0), g["throw"] = verb(1), g["return"] = verb(2), typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (g && (g = 0, op[0] && (_ = 0)), _) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
(function () {
    var configNode = document.getElementById("search-map-config");
    var root = document.querySelector("[data-search-map-root]");
    var mountNode = document.querySelector("[data-search-map-vue-controller]");
    if (!configNode || !root || !mountNode || typeof Vue === "undefined") {
        return;
    }
    var config = JSON.parse(configNode.textContent || "{}");
    config.markers = Array.isArray(config.markers) ? config.markers : [];
    var googleMapsScriptId = "google-maps-javascript-api";
    function loadGoogleMaps(apiKey) {
        var _a, _b;
        var browserWindow = window;
        if ((_b = (_a = browserWindow.google) === null || _a === void 0 ? void 0 : _a.maps) === null || _b === void 0 ? void 0 : _b.importLibrary) {
            return Promise.resolve();
        }
        return new Promise(function (resolve, reject) {
            var existingScript = document.getElementById(googleMapsScriptId);
            var callbackName = "__medriSearchMapReady";
            browserWindow[callbackName] = function () { return resolve(); };
            if (existingScript) {
                existingScript.addEventListener("error", function () { return reject(new Error("caricamento-mappa-non-riuscito")); }, { once: true });
                return;
            }
            var script = document.createElement("script");
            script.id = googleMapsScriptId;
            script.async = true;
            script.defer = true;
            script.src =
                "https://maps.googleapis.com/maps/api/js?key=" +
                    encodeURIComponent(apiKey) +
                    "&v=weekly&libraries=marker&callback=" +
                    encodeURIComponent(callbackName);
            script.onerror = function () { return reject(new Error("caricamento-mappa-non-riuscito")); };
            document.head.appendChild(script);
        });
    }
    Vue.createApp({
        data: function () {
            return {
                config: config,
                maps: [],
                selectedId: config.selectedId || "",
                previewPositionFrame: 0
            };
        },
        mounted: function () {
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
            initializeMaps: function () {
                return __awaiter(this, void 0, void 0, function () {
                    var mapsLibrary, markerLibrary, Map_1, AdvancedMarkerElement_1, _a;
                    var _this = this;
                    return __generator(this, function (_b) {
                        switch (_b.label) {
                            case 0:
                                _b.trys.push([0, 4, , 5]);
                                return [4, loadGoogleMaps(this.config.apiKey)];
                            case 1:
                                _b.sent();
                                return [4, google.maps.importLibrary("maps")];
                            case 2:
                                mapsLibrary = _b.sent();
                                return [4, google.maps.importLibrary("marker")];
                            case 3:
                                markerLibrary = _b.sent();
                                Map_1 = mapsLibrary.Map;
                                AdvancedMarkerElement_1 = markerLibrary.AdvancedMarkerElement;
                                document.querySelectorAll("[data-google-map]").forEach(function (element) {
                                    var map = new Map_1(element, {
                                        center: _this.initialCenter(),
                                        zoom: 13,
                                        mapId: _this.config.mapId,
                                        disableDefaultUI: true,
                                        clickableIcons: false
                                    });
                                    var markerEntries = _this.config.markers.map(function (marker) {
                                        var content = _this.createMarkerContent(marker);
                                        var advancedMarker = new AdvancedMarkerElement_1({
                                            map: map,
                                            position: { lat: marker.latitude, lng: marker.longitude },
                                            title: marker.title,
                                            content: content
                                        });
                                        content.addEventListener("click", function () { return _this.selectMarker(marker.id, map); });
                                        return { marker: marker, advancedMarker: advancedMarker, content: content };
                                    });
                                    var selectedMarker = _this.selectedMarker();
                                    if (!selectedMarker && _this.config.markers.length > 1) {
                                        var bounds_1 = new google.maps.LatLngBounds();
                                        _this.config.markers.forEach(function (marker) {
                                            bounds_1.extend({ lat: marker.latitude, lng: marker.longitude });
                                        });
                                        map.fitBounds(bounds_1, 56);
                                    }
                                    _this.maps.push({ map: map, markerEntries: markerEntries, mapElement: element });
                                    ["bounds_changed", "drag", "dragend", "zoom_changed", "idle"].forEach(function (eventName) {
                                        map.addListener(eventName, function () { return _this.schedulePreviewPosition(); });
                                    });
                                    if (selectedMarker) {
                                        _this.selectMarker(selectedMarker.id, map);
                                    }
                                    else {
                                        _this.syncSelectedMarker();
                                    }
                                });
                                return [3, 5];
                            case 4:
                                _a = _b.sent();
                                this.showFallback();
                                return [3, 5];
                            case 5: return [2];
                        }
                    });
                });
            },
            initialCenter: function () {
                var selected = this.selectedMarker() || this.config.markers[0];
                return selected
                    ? { lat: selected.latitude, lng: selected.longitude }
                    : { lat: 44.1396, lng: 12.2431 };
            },
            createMarkerContent: function (marker) {
                var content = document.createElement("button");
                content.type = "button";
                content.className = "map-marker-pin";
                content.textContent = marker.label;
                content.setAttribute("aria-label", marker.title);
                content.dataset.markerId = marker.id;
                return content;
            },
            bindCardSelection: function () {
                var _this = this;
                document.querySelectorAll("[data-map-card-id]").forEach(function (card) {
                    card.addEventListener("click", function (event) {
                        if (event.target.closest("button, a")) {
                            return;
                        }
                        _this.selectMarker(card.dataset.mapCardId || "");
                    });
                });
            },
            bindZoomControls: function () {
                var _this = this;
                document.querySelectorAll("[data-map-zoom-in]").forEach(function (button) {
                    button.addEventListener("click", function () { return _this.zoomBy(1); });
                });
                document.querySelectorAll("[data-map-zoom-out]").forEach(function (button) {
                    button.addEventListener("click", function () { return _this.zoomBy(-1); });
                });
            },
            bindPreviewCloseButtons: function () {
                var _this = this;
                document.querySelectorAll("[data-map-preview-close]").forEach(function (button) {
                    button.addEventListener("click", function (event) {
                        event.preventDefault();
                        event.stopPropagation();
                        _this.selectedId = "";
                        _this.hidePreview();
                        _this.syncSelectedMarker();
                        _this.syncSelectedCard();
                    });
                });
            },
            zoomBy: function (delta) {
                this.maps.forEach(function (entry) {
                    var zoom = entry.map.getZoom();
                    if (typeof zoom === "number") {
                        entry.map.setZoom(zoom + delta);
                    }
                });
            },
            selectMarker: function (id, sourceMap) {
                var _this = this;
                if (!id) {
                    return;
                }
                this.selectedId = id;
                var selected = this.selectedMarker();
                this.updatePreview(selected);
                this.syncSelectedMarker();
                this.syncSelectedCard();
                if (!selected) {
                    return;
                }
                var target = { lat: selected.latitude, lng: selected.longitude };
                this.maps.forEach(function (entry) {
                    entry.map.panTo(target);
                });
                if (sourceMap) {
                    sourceMap.panTo(target);
                }
                this.positionPreview(selected.id);
                this.schedulePreviewPosition();
                window.setTimeout(function () { return _this.positionPreview(selected.id); }, 180);
            },
            selectedMarker: function () {
                var _this = this;
                return this.selectedId
                    ? this.config.markers.find(function (marker) { return marker.id === _this.selectedId; }) || null
                    : null;
            },
            updatePreview: function (marker) {
                if (!marker) {
                    return;
                }
                document.querySelectorAll("[data-map-preview-title]").forEach(function (node) {
                    node.textContent = marker.title;
                });
                document.querySelectorAll("[data-map-preview-location]").forEach(function (node) {
                    node.textContent = marker.displayLocation;
                });
                document.querySelectorAll("[data-map-preview-facts]").forEach(function (node) {
                    node.textContent = marker.factsLabel;
                });
                document.querySelectorAll("[data-map-preview-price]").forEach(function (node) {
                    node.textContent = marker.priceLabel;
                });
                document.querySelectorAll("[data-map-preview-tag]").forEach(function (node) {
                    node.textContent = marker.tag;
                });
                document.querySelectorAll("[data-map-preview-link]").forEach(function (link) {
                    link.href = marker.detailUrl;
                    link.setAttribute("aria-label", "Apri " + marker.title);
                });
                document.querySelectorAll("[data-map-preview-image]").forEach(function (node) {
                    node.style.backgroundImage = marker.imageUrl ? "url(\"".concat(marker.imageUrl, "\")") : "";
                });
                document.querySelectorAll(".map-card[data-map-preview]").forEach(function (node) {
                    node.hidden = false;
                    node.removeAttribute("hidden");
                    node.classList.add("is-visible");
                    node.style.display = "block";
                    node.style.opacity = "1";
                    node.style.visibility = "visible";
                });
            },
            hidePreview: function () {
                document.querySelectorAll(".map-card[data-map-preview]").forEach(function (node) {
                    node.hidden = true;
                    node.setAttribute("hidden", "hidden");
                    node.classList.remove("is-visible");
                    node.removeAttribute("style");
                });
            },
            schedulePreviewPosition: function () {
                var _this = this;
                if (!this.selectedId) {
                    return;
                }
                if (this.previewPositionFrame) {
                    window.cancelAnimationFrame(this.previewPositionFrame);
                }
                this.previewPositionFrame = window.requestAnimationFrame(function () {
                    _this.previewPositionFrame = 0;
                    _this.positionPreview(_this.selectedId);
                });
            },
            positionPreview: function (markerId) {
                var preview = document.querySelector(".map-card[data-map-preview]");
                if (!preview || preview.hidden) {
                    return;
                }
                var markerEntry = this.findMarkerEntry(markerId);
                if (!markerEntry) {
                    return;
                }
                var mapCanvas = markerEntry.mapElement.closest("[data-map-canvas]");
                if (!mapCanvas) {
                    return;
                }
                var markerRect = markerEntry.markerItem.content.getBoundingClientRect();
                var canvasRect = mapCanvas.getBoundingClientRect();
                var previewRect = preview.getBoundingClientRect();
                var rootFontSize = Number.parseFloat(window.getComputedStyle(document.documentElement).fontSize) || 16;
                var gap = rootFontSize * 0.75;
                var markerCenter = markerRect.left + markerRect.width / 2 - canvasRect.left;
                var markerTop = markerRect.top - canvasRect.top;
                preview.style.left = "".concat(markerCenter, "px");
                preview.style.top = "".concat(markerTop - gap - previewRect.height, "px");
                preview.style.right = "auto";
                preview.style.bottom = "auto";
                preview.style.transform = "translateX(-50%)";
            },
            findMarkerEntry: function (markerId) {
                for (var _i = 0, _a = this.maps; _i < _a.length; _i++) {
                    var entry = _a[_i];
                    var markerItem = entry.markerEntries.find(function (item) { return item.marker.id === markerId; });
                    if (markerItem) {
                        return { mapElement: entry.mapElement, markerItem: markerItem };
                    }
                }
                return null;
            },
            syncSelectedMarker: function () {
                var _this = this;
                this.maps.forEach(function (entry) {
                    entry.markerEntries.forEach(function (item) {
                        item.content.classList.toggle("is-selected", item.marker.id === _this.selectedId);
                    });
                });
            },
            syncSelectedCard: function () {
                var _this = this;
                document.querySelectorAll("[data-map-card-id]").forEach(function (card) {
                    card.classList.toggle("is-map-selected", card.dataset.mapCardId === _this.selectedId);
                });
            },
            showFallback: function () {
                document.querySelectorAll("[data-map-fallback]").forEach(function (fallback) {
                    fallback.hidden = false;
                });
            }
        }
    }).mount(mountNode);
})();
