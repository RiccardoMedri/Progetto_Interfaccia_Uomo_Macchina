(() => {
    const gridSelector = "[data-property-media-grid]";
    const itemSelector = "[data-property-media-item]";
    const sortOrderSelector = "[data-property-media-sort-order]";
    const removeButtonSelector = "[data-property-media-remove]";
    const removeInputSelector = "[data-property-media-remove-input]";
    const positionSelector = "[data-property-media-position]";
    const coverSelector = "[data-property-media-cover]";

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

    document.querySelectorAll<HTMLElement>(gridSelector).forEach(updateMediaOrder);
})();
