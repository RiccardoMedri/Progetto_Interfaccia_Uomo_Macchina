"use strict";
(function () {
    var gridSelector = "[data-property-media-grid]";
    var itemSelector = "[data-property-media-item]";
    var sortOrderSelector = "[data-property-media-sort-order]";
    var removeButtonSelector = "[data-property-media-remove]";
    var removeInputSelector = "[data-property-media-remove-input]";
    var positionSelector = "[data-property-media-position]";
    var coverSelector = "[data-property-media-cover]";
    function closest(element, selector) {
        return element instanceof Element ? element.closest(selector) : null;
    }
    function visibleItems(grid) {
        if (!grid) {
            return [];
        }
        return Array.from(grid.querySelectorAll(itemSelector))
            .filter(function (item) { return !item.hidden; });
    }
    function updateMediaOrder(grid) {
        visibleItems(grid).forEach(function (item, index) {
            var order = index + 1;
            var sortOrderInput = item.querySelector(sortOrderSelector);
            var position = item.querySelector(positionSelector);
            var cover = item.querySelector(coverSelector);
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
    function insertBeforeTarget(grid, draggedItem, targetItem, pointerY) {
        if (!targetItem || targetItem === draggedItem) {
            return;
        }
        var targetBounds = targetItem.getBoundingClientRect();
        var insertAfter = pointerY > targetBounds.top + targetBounds.height / 2;
        grid.insertBefore(draggedItem, insertAfter ? targetItem.nextSibling : targetItem);
    }
    document.addEventListener("dragstart", function (event) {
        var item = closest(event.target, itemSelector);
        if (!item || item.hidden || !event.dataTransfer) {
            return;
        }
        item.classList.add("is-dragging");
        event.dataTransfer.effectAllowed = "move";
        event.dataTransfer.setData("text/plain", "");
    });
    document.addEventListener("dragend", function (event) {
        var item = closest(event.target, itemSelector);
        if (!item) {
            return;
        }
        item.classList.remove("is-dragging");
        updateMediaOrder(closest(item, gridSelector));
    });
    document.addEventListener("dragover", function (event) {
        var grid = closest(event.target, gridSelector);
        var draggedItem = (grid === null || grid === void 0 ? void 0 : grid.querySelector(".is-dragging")) || null;
        var targetItem = closest(event.target, itemSelector);
        if (!grid || !draggedItem || !targetItem) {
            return;
        }
        event.preventDefault();
        insertBeforeTarget(grid, draggedItem, targetItem, event.clientY);
    });
    document.addEventListener("drop", function (event) {
        var grid = closest(event.target, gridSelector);
        if (!grid) {
            return;
        }
        event.preventDefault();
        updateMediaOrder(grid);
    });
    document.addEventListener("click", function (event) {
        var button = closest(event.target, removeButtonSelector);
        if (!button) {
            return;
        }
        var item = closest(button, itemSelector);
        var grid = closest(item, gridSelector);
        var removeInput = (item === null || item === void 0 ? void 0 : item.querySelector(removeInputSelector)) || null;
        if (removeInput) {
            removeInput.value = "true";
        }
        if (item) {
            item.hidden = true;
            item.draggable = false;
        }
        updateMediaOrder(grid);
    });
    document.querySelectorAll(gridSelector).forEach(updateMediaOrder);
})();
