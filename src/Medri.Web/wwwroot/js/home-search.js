"use strict";
(function () {
    document.querySelectorAll("[data-search-multiselect]").forEach(function (multiselect) {
        var label = multiselect.querySelector("[data-search-multiselect-label]");
        var inputs = multiselect.querySelectorAll("[data-search-multiselect-input]");
        var placeholder = multiselect.dataset.searchMultiselectPlaceholder || "";
        var updateLabel = function () {
            var selected = Array.from(inputs).filter(function (input) { return input.checked; });
            if (!label) {
                return;
            }
            label.textContent = selected.length === 0
                ? placeholder
                : selected.length === 1
                    ? "1 selezione"
                    : "".concat(selected.length, " selezioni");
        };
        inputs.forEach(function (input) { return input.addEventListener("change", updateLabel); });
        updateLabel();
    });
})();
