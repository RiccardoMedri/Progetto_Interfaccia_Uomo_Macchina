"use strict";
(function () {
    var panelSelector = "[data-featured-panel]";
    var formSelector = "[data-featured-form]";
    var rowButtonSelector = "[data-featured-row-button]";
    var lastSubmitter = null;
    function closest(element, selector) {
        return element instanceof Element ? element.closest(selector) : null;
    }
    function setBusy(form, submitter, isBusy) {
        var panel = closest(form, panelSelector) || document.querySelector(panelSelector);
        if (panel) {
            panel.setAttribute("aria-busy", isBusy ? "true" : "false");
        }
        if (submitter) {
            submitter.disabled = isBusy;
        }
    }
    function submitAction(form, submitter) {
        return (submitter === null || submitter === void 0 ? void 0 : submitter.getAttribute("formaction")) || form.getAttribute("action");
    }
    function syncRowButtons(nextDocument) {
        var nextButtons = {};
        nextDocument.querySelectorAll(rowButtonSelector).forEach(function (button) {
            var reference = button.getAttribute("data-feature-reference");
            if (reference) {
                nextButtons[reference] = button;
            }
        });
        document.querySelectorAll(rowButtonSelector).forEach(function (button) {
            var reference = button.getAttribute("data-feature-reference");
            var nextButton = reference ? nextButtons[reference] : null;
            if (nextButton) {
                button.replaceWith(nextButton.cloneNode(true));
            }
        });
    }
    function syncFeaturedPanel(html, responseUrl) {
        var nextDocument = new DOMParser().parseFromString(html, "text/html");
        var currentPanel = document.querySelector(panelSelector);
        var nextPanel = nextDocument.querySelector(panelSelector);
        if (!currentPanel || !nextPanel) {
            window.location.href = responseUrl || window.location.href;
            return;
        }
        currentPanel.replaceWith(nextPanel);
        syncRowButtons(nextDocument);
    }
    function sendFeaturedForm(form, submitter) {
        var action = submitAction(form, submitter);
        if (!action || (submitter === null || submitter === void 0 ? void 0 : submitter.disabled)) {
            return;
        }
        setBusy(form, submitter, true);
        fetch(action, {
            method: (form.getAttribute("method") || "post").toUpperCase(),
            body: new FormData(form),
            credentials: "same-origin",
            headers: {
                "X-Requested-With": "XMLHttpRequest"
            }
        })
            .then(function (response) {
            if (!response.ok) {
                throw new Error("Featured request failed");
            }
            return response.text().then(function (html) {
                syncFeaturedPanel(html, response.url);
            });
        })
            .catch(function () {
            form.setAttribute("action", action);
            form.submit();
        })
            .finally(function () {
            setBusy(form, submitter, false);
            lastSubmitter = null;
        });
    }
    document.addEventListener("click", function (event) {
        var button = closest(event.target, "button[type='submit']");
        if (button) {
            lastSubmitter = button;
        }
    }, true);
    document.addEventListener("submit", function (event) {
        var form = event.target instanceof HTMLFormElement ? event.target : null;
        if (!form) {
            return;
        }
        var submitEvent = event;
        var submitter = submitEvent.submitter instanceof HTMLButtonElement
            ? submitEvent.submitter
            : lastSubmitter;
        var isFeaturedPanelForm = form.matches(formSelector);
        var isFeaturedRowAction = (submitter === null || submitter === void 0 ? void 0 : submitter.matches(rowButtonSelector)) === true;
        if (!isFeaturedPanelForm && !isFeaturedRowAction) {
            return;
        }
        event.preventDefault();
        sendFeaturedForm(form, submitter);
    });
})();
