(() => {
    document.querySelectorAll<HTMLElement>("[data-search-multiselect]").forEach((multiselect) => {
        const label = multiselect.querySelector<HTMLElement>("[data-search-multiselect-label]");
        const inputs = multiselect.querySelectorAll<HTMLInputElement>("[data-search-multiselect-input]");
        const placeholder = multiselect.dataset.searchMultiselectPlaceholder || "";

        const updateLabel = () => {
            const selected = Array.from(inputs).filter((input) => input.checked);
            if (!label) {
                return;
            }

            label.textContent = selected.length === 0
                ? placeholder
                : selected.length === 1
                    ? "1 selezione"
                    : `${selected.length} selezioni`;
        };

        inputs.forEach((input) => input.addEventListener("change", updateLabel));
        updateLabel();
    });
})();
