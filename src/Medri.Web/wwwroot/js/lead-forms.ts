(() => {
    const forms = document.querySelectorAll<HTMLElement>("[data-lead-form]");

    forms.forEach(form => {
        const customPanels = form.querySelectorAll<HTMLElement>("[data-custom-time]");
        const timeInputs = form.querySelectorAll<HTMLInputElement>("input[name='PreferredTimeSlot']");

        const updateCustomTime = (value: string) => {
            customPanels.forEach(panel => panel.classList.toggle("is-collapsed", value !== "Altro orario"));
        };

        form.querySelectorAll<HTMLElement>("[data-contact-choice]").forEach(card => {
            card.addEventListener("click", () => {
                const input = card.querySelector<HTMLInputElement>("input[type='radio']");
                if (!input) {
                    return;
                }

                input.checked = true;
                form.querySelectorAll<HTMLElement>("[data-contact-choice]")
                    .forEach(item => item.classList.toggle("is-selected", item === card));
            });
        });

        form.querySelectorAll<HTMLButtonElement>("[data-time-choice]").forEach(button => {
            button.addEventListener("click", () => {
                const value = button.dataset.timeChoice || "";
                timeInputs.forEach(input => input.value = value);
                form.querySelectorAll<HTMLElement>("[data-time-choice]")
                    .forEach(item => item.classList.toggle("is-selected", item === button));
                updateCustomTime(value);
            });
        });

        updateCustomTime(timeInputs.item(0)?.value || "");
    });
})();
