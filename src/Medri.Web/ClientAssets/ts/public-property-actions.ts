(function () {
    const compareKey = "medri.comparePropertyIds";
    const maxCompareItems = 4;

    interface FavoriteCommandResult {
        propertyId: string;
        isSaved: boolean;
        succeeded: boolean;
    }

    function bindFavoriteForms(): void {
        document.querySelectorAll<HTMLFormElement>("[data-favorite-form]").forEach((form) => {
            form.addEventListener("submit", async (event) => {
                if (document.body.dataset.authenticated !== "true") {
                    return;
                }

                event.preventDefault();
                event.stopPropagation();

                const propertyId = form.dataset.propertyId;
                if (!propertyId) {
                    return;
                }

                setFavoriteControlsBusy(propertyId, true);
                try {
                    const response = await fetch(form.action, {
                        method: "POST",
                        body: new FormData(form),
                        credentials: "same-origin",
                        headers: {
                            "X-Requested-With": "XMLHttpRequest",
                            "Accept": "application/json"
                        }
                    });

                    if (response.redirected && response.url.includes("/login")) {
                        window.location.href = response.url;
                        return;
                    }

                    if (!response.ok) {
                        throw new Error(`Richiesta preferiti non riuscita con stato ${response.status}.`);
                    }

                    const result = await response.json() as FavoriteCommandResult;
                    if (!result.succeeded) {
                        throw new Error("Richiesta preferiti non completata.");
                    }

                    syncFavoriteControls(result.propertyId, result.isSaved);
                } catch {
                    
                } finally {
                    setFavoriteControlsBusy(propertyId, false);
                }
            });
        });
    }

    function syncFavoriteControls(propertyId: string, isSaved: boolean): void {
        document.querySelectorAll<HTMLFormElement>(`[data-favorite-form][data-property-id="${CSS.escape(propertyId)}"]`).forEach((form) => {
            form.action = isSaved ? "/preferiti/rimuovi" : "/preferiti/aggiungi";
        });

        document.querySelectorAll<HTMLButtonElement>(`[data-favorite-button][data-property-id="${CSS.escape(propertyId)}"]`).forEach((button) => {
            const title = button.dataset.propertyTitle || "annuncio";
            button.classList.toggle("is-selected", isSaved);
            button.setAttribute("aria-pressed", isSaved ? "true" : "false");
            button.setAttribute("aria-label", isSaved ? `Rimuovi ${title} dai preferiti` : `Salva ${title} nei preferiti`);
        });
    }

    function setFavoriteControlsBusy(propertyId: string, isBusy: boolean): void {
        document.querySelectorAll<HTMLButtonElement>(`[data-favorite-button][data-property-id="${CSS.escape(propertyId)}"]`).forEach((button) => {
            button.disabled = isBusy;
            button.setAttribute("aria-busy", isBusy ? "true" : "false");
        });
    }

    function getStorage(): Storage | null {
        try {
            return window.sessionStorage;
        } catch {
            return null;
        }
    }

    function readIds(key: string): string[] {
        try {
            const storage = getStorage();
            const value = storage?.getItem(key);
            if (!value) {
                return [];
            }

            const parsed = JSON.parse(value);
            if (!Array.isArray(parsed)) {
                return [];
            }

            return parsed
                .filter((id) => typeof id === "string")
                .filter((id, index, ids) => ids.indexOf(id) === index)
                .slice(0, maxCompareItems);
        } catch {
            return [];
        }
    }

    function writeIds(key: string, ids: string[]): void {
        const storage = getStorage();
        if (!storage) {
            return;
        }

        const normalizedIds = ids
            .filter(Boolean)
            .filter((id, index, values) => values.indexOf(id) === index)
            .slice(0, maxCompareItems);
        storage.setItem(key, JSON.stringify(normalizedIds));
    }

    function toggleId(key: string, id: string, maxItems?: number): void {
        const ids = readIds(key);
        const existingIndex = ids.indexOf(id);
        if (existingIndex >= 0) {
            ids.splice(existingIndex, 1);
        } else if (!maxItems || ids.length < maxItems) {
            ids.push(id);
        }

        writeIds(key, ids);
    }

    function syncComparisonPageWithQuery(): void {
        if (window.location.pathname !== "/confronto") {
            return;
        }

        const ids = new URLSearchParams(window.location.search)
            .get("ids")
            ?.split(",")
            .map((id) => id.trim())
            .filter(Boolean) || [];
        writeIds(compareKey, ids);
    }

    function syncButtons(selector: string, key: string): void {
        const ids = readIds(key);
        document.querySelectorAll<HTMLElement>(selector).forEach((button) => {
            const id = button.dataset.propertyId || "";
            const isSelected = ids.indexOf(id) >= 0;
            button.classList.toggle("is-selected", isSelected);
            button.setAttribute("aria-pressed", isSelected ? "true" : "false");
        });
    }

    function updateCompareLinks(): void {
        const ids = readIds(compareKey);
        const canCompare = ids.length >= 2;
        const href = canCompare ? `/confronto?ids=${encodeURIComponent(ids.join(","))}` : "";
        document.querySelectorAll<HTMLAnchorElement>("[data-compare-link]").forEach((link) => {
            if (canCompare) {
                link.href = href;
                link.removeAttribute("aria-disabled");
                link.removeAttribute("tabindex");
            } else {
                link.removeAttribute("href");
                link.setAttribute("aria-disabled", "true");
                link.setAttribute("tabindex", "-1");
            }
            link.classList.toggle("is-disabled", !canCompare);
        });

        document.querySelectorAll<HTMLElement>("[data-compare-count]").forEach((element) => {
            element.textContent = ids.length === 1 ? "1 immobile selezionato" : `${ids.length} immobili selezionati`;
        });
    }

    function bindCompareButtons(): void {
        document.querySelectorAll<HTMLElement>("[data-compare-button]").forEach((button) => {
            button.addEventListener("click", (event) => {
                event.preventDefault();
                event.stopPropagation();
                const id = button.dataset.propertyId;
                if (!id) {
                    return;
                }

                toggleId(compareKey, id, maxCompareItems);
                syncButtons("[data-compare-button]", compareKey);
                updateCompareLinks();
            });
        });
    }

    function bindCompareButtonEventShield(): void {
        document.querySelectorAll<HTMLElement>("[data-compare-button]").forEach((button) => {
            ["pointerdown", "mousedown", "mouseup"].forEach((eventName) => {
                button.addEventListener(eventName, (event) => {
                    event.stopPropagation();
                }, true);
            });
        });
    }

    function bindPrintButtons(): void {
        document.querySelectorAll<HTMLElement>("[data-print-button]").forEach((button) => {
            button.addEventListener("click", () => window.print());
        });
    }

    function bindShareButtons(): void {
        document.querySelectorAll<HTMLElement>("[data-share-button]").forEach((button) => {
            const originalLabel = button.textContent ?? "";
            const originalAria = button.getAttribute("aria-label") ?? "";
            let resetHandle = 0;

            button.addEventListener("click", async () => {
                const shareData = {
                    title: document.title,
                    url: window.location.href
                };

                try {
                    if (navigator.share) {
                        await navigator.share(shareData);
                    } else if (navigator.clipboard) {
                        await navigator.clipboard.writeText(shareData.url);
                        button.textContent = "Link copiato";
                        button.classList.add("is-copied");
                        button.setAttribute("aria-label", "Link confronto copiato");
                        if (resetHandle) {
                            window.clearTimeout(resetHandle);
                        }
                        resetHandle = window.setTimeout(() => {
                            button.textContent = originalLabel;
                            button.classList.remove("is-copied");
                            button.setAttribute("aria-label", originalAria);
                            resetHandle = 0;
                        }, 2200);
                    }
                } catch {

                }
            });
        });
    }

    function bindSavedSearchForms(): void {
        document.querySelectorAll<HTMLFormElement>("#saveSearchDesktop").forEach((form) => {
            form.addEventListener("submit", async (event) => {
                if (document.body.dataset.authenticated !== "true") {
                    return;
                }

                event.preventDefault();
                try {
                    await fetch(form.action, {
                        method: "POST",
                        body: new FormData(form),
                        credentials: "same-origin",
                        headers: {
                            "X-Requested-With": "XMLHttpRequest"
                        }
                    });
                    document.querySelectorAll<HTMLElement>("[data-save-search-button]").forEach((button) => {
                        button.classList.add("is-selected");
                        button.setAttribute("aria-pressed", "true");
                        button.textContent = "Ricerca salvata";
                    });
                } catch {
                    
                }
            });
        });
    }

    function bindPriceRangeOutputs(): void {
        document.querySelectorAll<HTMLElement>("[data-advanced-filters-panel]").forEach((panel) => {
            panel.querySelectorAll<HTMLInputElement>("[data-price-range-input]").forEach((input) => {
                const output = panel.querySelector<HTMLOutputElement>(`[data-price-range-output="${input.dataset.priceRangeRole}"]`);
                const update = () => {
                    if (output) {
                        output.textContent = input.value;
                    }
                };
                input.addEventListener("input", update);
                update();
            });
        });
    }

    function bindSearchCardNavigation(): void {
        document.querySelectorAll<HTMLElement>("[data-property-detail-url]").forEach((card) => {
            card.addEventListener("click", (event) => {
                const target = event.target as HTMLElement | null;
                if (target?.closest("a, button, input, label, select, textarea, form")) {
                    return;
                }

                const url = card.dataset.propertyDetailUrl;
                if (url) {
                    window.location.href = url;
                }
            });
        });
    }

    function bindSearchFiltersDialog(): void {
        const dialog = document.querySelector<HTMLElement>("[data-search-filters-dialog]");
        const openButton = document.querySelector<HTMLElement>("[data-search-filters-open]");
        if (!dialog || !openButton) {
            return;
        }

        
        dialog.querySelectorAll<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>("input, select, textarea").forEach((control) => {
            if (!control.hasAttribute("form")) {
                control.setAttribute("form", "searchFiltersDesktop");
            }
        });

        
        
        
        const basicGroup = dialog.querySelector<HTMLElement>("[data-search-drawer-basic].d-md-none");
        if (basicGroup) {
            const desktopQuery = window.matchMedia("(min-width: 768px)");
            const syncBasicGroup = () => {
                basicGroup.querySelectorAll<HTMLInputElement>("input").forEach((input) => {
                    input.disabled = desktopQuery.matches;
                });
            };
            syncBasicGroup();
            desktopQuery.addEventListener("change", syncBasicGroup);
        }

        const open = () => {
            dialog.hidden = false;
            window.requestAnimationFrame(() => dialog.classList.add("is-open"));
            openButton.setAttribute("aria-expanded", "true");
        };

        const close = () => {
            dialog.classList.remove("is-open");
            openButton.setAttribute("aria-expanded", "false");
            window.setTimeout(() => {
                if (!dialog.classList.contains("is-open")) {
                    dialog.hidden = true;
                }
            }, 320);
        };

        openButton.addEventListener("click", (event) => {
            event.preventDefault();
            open();
        });

        dialog.querySelectorAll<HTMLElement>("[data-search-filters-close]").forEach((control) => {
            control.addEventListener("click", (event) => {
                event.preventDefault();
                close();
            });
        });

        document.addEventListener("keydown", (event) => {
            if (event.key === "Escape" && dialog.classList.contains("is-open")) {
                close();
                openButton.focus();
            }
        });
    }

    function bindNotificationPreferenceForms(): void {
        document.querySelectorAll<HTMLFormElement>("[data-notification-preference-form]").forEach((form) => {
            const stateToggle = form.querySelector<HTMLInputElement>("[data-notification-state-toggle]");
            const stateLabel = form.querySelector<HTMLElement>("[data-notification-state-label]");
            const frequencyGroup = form.querySelector<HTMLFieldSetElement>("[data-notification-frequency-group]");
            const syncState = (): void => {
                const isActive = stateToggle?.checked === true;
                if (stateLabel) {
                    stateLabel.textContent = isActive ? "Attivo" : "Non attivo";
                }
                if (frequencyGroup) {
                    frequencyGroup.disabled = !isActive;
                    frequencyGroup.classList.toggle("is-disabled", !isActive);
                }
            };

            syncState();
            form.querySelectorAll<HTMLInputElement>('input[type="checkbox"]').forEach((input) => {
                input.addEventListener("change", () => {
                    syncState();
                    form.requestSubmit();
                });
            });
        });
    }

    document.addEventListener("DOMContentLoaded", () => {
        bindFavoriteForms();
        syncComparisonPageWithQuery();
        syncButtons("[data-compare-button]", compareKey);
        updateCompareLinks();
        bindCompareButtonEventShield();
        bindCompareButtons();
        bindPrintButtons();
        bindShareButtons();
        bindSavedSearchForms();
        bindPriceRangeOutputs();
        bindSearchCardNavigation();
        bindSearchFiltersDialog();
        bindNotificationPreferenceForms();
    });
})();
