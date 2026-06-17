(() => {
    interface AdminPropertyGeocodeResult {
        succeeded: boolean;
        latitude?: number | null;
        longitude?: number | null;
        message?: string;
    }

    const gridSelector = "[data-property-media-grid]";
    const itemSelector = "[data-property-media-item]";
    const sortOrderSelector = "[data-property-media-sort-order]";
    const removeButtonSelector = "[data-property-media-remove]";
    const removeInputSelector = "[data-property-media-remove-input]";
    const positionSelector = "[data-property-media-position]";
    const coverSelector = "[data-property-media-cover]";
    const locationRootSelector = "[data-property-location]";
    const locationDisplaySelector = "[data-property-location-display]";
    const locationAddressSelector = "[data-property-location-address]";
    const locationLatitudeSelector = "[data-property-location-latitude]";
    const locationLongitudeSelector = "[data-property-location-longitude]";
    const locationStatusSelector = "[data-property-location-status]";
    const propertyFormSelector = "#propertyDetailForm";
    const uploadMediaAction = "UploadMedia";
    let activeSubmitter: HTMLElement | null = null;

    function closest<T extends Element>(element: EventTarget | null, selector: string): T | null {
        return element instanceof Element ? element.closest(selector) as T | null : null;
    }

    function visibleItems(grid: HTMLElement | null): HTMLElement[] {
        if (!grid) {
            return [];
        }

        return Array.from(grid.querySelectorAll<HTMLElement>(itemSelector))
            .filter((item) => !item.hidden);
    }

    function updateMediaOrder(grid: HTMLElement | null) {
        visibleItems(grid).forEach((item, index) => {
            const order = index + 1;
            const sortOrderInput = item.querySelector<HTMLInputElement>(sortOrderSelector);
            const position = item.querySelector<HTMLElement>(positionSelector);
            const cover = item.querySelector<HTMLElement>(coverSelector);

            if (sortOrderInput) {
                sortOrderInput.value = order.toString();
            }

            if (position) {
                position.textContent = order.toString();
            }

            if (cover) {
                cover.hidden = index !== 0;
            }
        });
    }

    function locationFieldValue(root: HTMLElement, selector: string): string {
        return root.querySelector<HTMLInputElement>(selector)?.value.trim() || "";
    }

    function normalizeLocationPart(value: string): string {
        return value
            .replace(/\s+/g, " ")
            .trim();
    }

    function locationQuery(root: HTMLElement): string {
        const address = normalizeLocationPart(locationFieldValue(root, locationAddressSelector));
        const displayLocation = normalizeLocationPart(locationFieldValue(root, locationDisplaySelector));
        return [ address, displayLocation ]
            .filter((value) => !!value)
            .join(" | ");
    }

    function antiForgeryToken(root: HTMLElement): string {
        const form = closest<HTMLFormElement>(root, propertyFormSelector);
        return form?.querySelector<HTMLInputElement>("input[name='__RequestVerificationToken']")?.value || "";
    }

    function isUploadMediaSubmitter(submitter: HTMLElement | null): boolean {
        return submitter instanceof HTMLButtonElement &&
            submitter.name === "SubmitAction" &&
            submitter.value === uploadMediaAction;
    }

    function setLocationStatus(root: HTMLElement, message: string): void {
        const status = root.querySelector<HTMLElement>(locationStatusSelector);
        if (status) {
            status.textContent = message;
        }
    }

    function coordinateValue(root: HTMLElement, selector: string): number | null {
        const value = locationFieldValue(root, selector).replace(",", ".");
        const parsed = Number.parseFloat(value);
        return Number.isFinite(parsed) ? parsed : null;
    }

    function hasUsableCoordinates(root: HTMLElement): boolean {
        const latitude = coordinateValue(root, locationLatitudeSelector);
        const longitude = coordinateValue(root, locationLongitudeSelector);

        return latitude !== null &&
            longitude !== null &&
            latitude >= -90 &&
            latitude <= 90 &&
            longitude >= -180 &&
            longitude <= 180 &&
            (latitude !== 0 || longitude !== 0);
    }

    function needsGeocoding(root: HTMLElement): boolean {
        const query = locationQuery(root);
        return !!query &&
            (!hasUsableCoordinates(root) || root.dataset.geocodedQuery !== query);
    }

    function setCoordinates(root: HTMLElement, latitude: number, longitude: number): void {
        const latitudeInput = root.querySelector<HTMLInputElement>(locationLatitudeSelector);
        const longitudeInput = root.querySelector<HTMLInputElement>(locationLongitudeSelector);

        if (latitudeInput) {
            latitudeInput.value = latitude.toFixed(6);
        }

        if (longitudeInput) {
            longitudeInput.value = longitude.toFixed(6);
        }

        root.dataset.geocodedQuery = locationQuery(root);
    }

    function clearCoordinates(root: HTMLElement): void {
        const latitudeInput = root.querySelector<HTMLInputElement>(locationLatitudeSelector);
        const longitudeInput = root.querySelector<HTMLInputElement>(locationLongitudeSelector);

        if (latitudeInput) {
            latitudeInput.value = "";
        }

        if (longitudeInput) {
            longitudeInput.value = "";
        }

        delete root.dataset.geocodedQuery;
    }

    async function geocodeLocation(root: HTMLElement): Promise<boolean> {
        const address = normalizeLocationPart(locationFieldValue(root, locationAddressSelector));
        const displayLocation = normalizeLocationPart(locationFieldValue(root, locationDisplaySelector));
        const endpoint = root.dataset.propertyLocationGeocodeUrl || "";

        if (!address) {
            setLocationStatus(root, "Inserisci un indirizzo preciso.");
            return false;
        }

        if (!endpoint) {
            clearCoordinates(root);
            setLocationStatus(root, "Mappa non disponibile.");
            return false;
        }

        setLocationStatus(root, "Ricerca posizione...");

        try {
            const body = new URLSearchParams();
            body.set("Address", address);
            body.set("DisplayLocation", displayLocation);

            const token = antiForgeryToken(root);
            if (token) {
                body.set("__RequestVerificationToken", token);
            }

            const response = await fetch(endpoint, {
                method: "POST",
                body,
                credentials: "same-origin",
                headers: {
                    "X-Requested-With": "XMLHttpRequest",
                    "Accept": "application/json",
                    "Content-Type": "application/x-www-form-urlencoded;charset=UTF-8"
                }
            });

            if (!response.ok) {
                clearCoordinates(root);
                setLocationStatus(root, "Mappa non disponibile.");
                return false;
            }

            const result = await response.json() as AdminPropertyGeocodeResult;
            if (!result.succeeded || typeof result.latitude !== "number" || typeof result.longitude !== "number") {
                clearCoordinates(root);
                setLocationStatus(root, result.message || "Indirizzo non trovato. Inserisci via, numero civico, comune e provincia.");
                return false;
            }

            setCoordinates(root, result.latitude, result.longitude);
            setLocationStatus(root, "Posizione mappa aggiornata.");
            return true;
        } catch {
            clearCoordinates(root);
            setLocationStatus(root, "Mappa non disponibile.");
            return false;
        }
    }

    function resubmitPropertyForm(form: HTMLFormElement, submitter: HTMLElement | null): void {
        form.dataset.propertyLocationSubmitting = "true";
        const requestSubmit = (form as any).requestSubmit as ((submitter?: HTMLElement) => void) | undefined;

        if (requestSubmit) {
            requestSubmit.call(form, submitter || undefined);
            return;
        }

        form.submit();
    }

    function bindLocationGeocoder(root: HTMLElement): void {
        const displayInput = root.querySelector<HTMLInputElement>(locationDisplaySelector);
        const addressInput = root.querySelector<HTMLInputElement>(locationAddressSelector);

        if (hasUsableCoordinates(root)) {
            root.dataset.geocodedQuery = locationQuery(root);
        }

        [ displayInput, addressInput ].forEach((input) => {
            if (!input) {
                return;
            }

            input.addEventListener("input", () => {
                clearCoordinates(root);
                setLocationStatus(root, "");
            });

            input.addEventListener("change", () => {
                if (needsGeocoding(root)) {
                    void geocodeLocation(root);
                }
            });
        });
    }

    function insertBeforeTarget(
        grid: HTMLElement,
        draggedItem: HTMLElement,
        targetItem: HTMLElement | null,
        pointerY: number) {
        if (!targetItem || targetItem === draggedItem) {
            return;
        }

        const targetBounds = targetItem.getBoundingClientRect();
        const insertAfter = pointerY > targetBounds.top + targetBounds.height / 2;
        grid.insertBefore(draggedItem, insertAfter ? targetItem.nextSibling : targetItem);
    }

    document.addEventListener("dragstart", (event: DragEvent) => {
        const item = closest<HTMLElement>(event.target, itemSelector);
        if (!item || item.hidden || !event.dataTransfer) {
            return;
        }

        item.classList.add("is-dragging");
        event.dataTransfer.effectAllowed = "move";
        event.dataTransfer.setData("text/plain", "");
    });

    document.addEventListener("dragend", (event: DragEvent) => {
        const item = closest<HTMLElement>(event.target, itemSelector);
        if (!item) {
            return;
        }

        item.classList.remove("is-dragging");
        updateMediaOrder(closest<HTMLElement>(item, gridSelector));
    });

    document.addEventListener("dragover", (event: DragEvent) => {
        const grid = closest<HTMLElement>(event.target, gridSelector);
        const draggedItem = grid?.querySelector<HTMLElement>(".is-dragging") || null;
        const targetItem = closest<HTMLElement>(event.target, itemSelector);
        if (!grid || !draggedItem || !targetItem) {
            return;
        }

        event.preventDefault();
        insertBeforeTarget(grid, draggedItem, targetItem, event.clientY);
    });

    document.addEventListener("drop", (event: DragEvent) => {
        const grid = closest<HTMLElement>(event.target, gridSelector);
        if (!grid) {
            return;
        }

        event.preventDefault();
        updateMediaOrder(grid);
    });

    document.addEventListener("click", (event) => {
        const button = closest<HTMLButtonElement>(event.target, removeButtonSelector);
        if (!button) {
            return;
        }

        const item = closest<HTMLElement>(button, itemSelector);
        const grid = closest<HTMLElement>(item, gridSelector);
        const removeInput = item?.querySelector<HTMLInputElement>(removeInputSelector) || null;

        if (removeInput) {
            removeInput.value = "true";
        }

        if (item) {
            item.hidden = true;
            item.draggable = false;
        }

        updateMediaOrder(grid);
    });

    document.addEventListener("click", (event) => {
        const submitter = closest<HTMLElement>(event.target, "button[type='submit'], input[type='submit']");
        if (submitter) {
            activeSubmitter = submitter;
        }
    });

    document.addEventListener("submit", (event) => {
        const form = event.target instanceof HTMLFormElement ? event.target : null;
        if (!form || !form.matches(propertyFormSelector)) {
            return;
        }

        if (form.dataset.propertyLocationSubmitting === "true") {
            delete form.dataset.propertyLocationSubmitting;
            return;
        }

        if (isUploadMediaSubmitter(activeSubmitter)) {
            return;
        }

        const root = form.querySelector<HTMLElement>(locationRootSelector);
        if (!root || !needsGeocoding(root)) {
            return;
        }

        event.preventDefault();
        // Non-blocking: try to resolve coordinates, but always submit afterwards — never block
        // the save because geocoding is unavailable or the address was not found.
        void geocodeLocation(root).finally(() => resubmitPropertyForm(form, activeSubmitter));
    });

    document.querySelectorAll<HTMLElement>(gridSelector).forEach(updateMediaOrder);
    document.querySelectorAll<HTMLElement>(locationRootSelector).forEach(bindLocationGeocoder);
})();
