"use strict";
(function () {
    var forms = document.querySelectorAll("[data-lead-form]");
    forms.forEach(function (form) {
        var _a;
        var customPanels = form.querySelectorAll("[data-custom-time]");
        var timeInputs = form.querySelectorAll("input[name='PreferredTimeSlot']");
        var updateCustomTime = function (value) {
            customPanels.forEach(function (panel) { return panel.classList.toggle("is-collapsed", value !== "Altro orario"); });
        };
        form.querySelectorAll("[data-contact-choice]").forEach(function (card) {
            card.addEventListener("click", function () {
                var input = card.querySelector("input[type='radio']");
                if (!input) {
                    return;
                }
                input.checked = true;
                form.querySelectorAll("[data-contact-choice]")
                    .forEach(function (item) { return item.classList.toggle("is-selected", item === card); });
            });
        });
        form.querySelectorAll("[data-time-choice]").forEach(function (button) {
            button.addEventListener("click", function () {
                var value = button.dataset.timeChoice || "";
                timeInputs.forEach(function (input) { return input.value = value; });
                form.querySelectorAll("[data-time-choice]")
                    .forEach(function (item) { return item.classList.toggle("is-selected", item === button); });
                updateCustomTime(value);
            });
        });
        updateCustomTime(((_a = timeInputs.item(0)) === null || _a === void 0 ? void 0 : _a.value) || "");
    });
})();
