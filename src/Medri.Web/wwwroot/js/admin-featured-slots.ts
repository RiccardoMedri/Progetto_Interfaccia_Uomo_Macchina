(() => {
    const panelSelector = "[data-featured-panel]";
    const formSelector = "[data-featured-form]";
    const rowButtonSelector = "[data-featured-row-button]";
    let lastSubmitter: HTMLButtonElement | null = null;

    function closest<T extends Element>(element: EventTarget | null, selector: string): T | null {
        return element instanceof Element ? element.closest(selector) as T | null : null;
    }

    function setBusy(form: HTMLFormElement, submitter: HTMLButtonElement | null, isBusy: boolean) {
        const panel = closest<HTMLElement>(form, panelSelector) || document.querySelector<HTMLElement>(panelSelector);
        if (panel) {
            panel.setAttribute("aria-busy", isBusy ? "true" : "false");
        }

        if (submitter) {
            submitter.disabled = isBusy;
        }
    }

    function submitAction(form: HTMLFormElement, submitter: HTMLButtonElement | null) {
        return submitter?.getAttribute("formaction") || form.getAttribute("action");
    }

    function syncRowButtons(nextDocument: Document) {
        const nextButtons: Record<string, HTMLButtonElement> = {};
        nextDocument.querySelectorAll<HTMLButtonElement>(rowButtonSelector).forEach((button) => {
            const reference = button.getAttribute("data-feature-reference");
            if (reference) {
                nextButtons[reference] = button;
            }
        });

        document.querySelectorAll<HTMLButtonElement>(rowButtonSelector).forEach((button) => {
            const reference = button.getAttribute("data-feature-reference");
            const nextButton = reference ? nextButtons[reference] : null;
            if (nextButton) {
                button.replaceWith(nextButton.cloneNode(true));
            }
        });
    }

    function syncFeaturedPanel(html: string, responseUrl: string) {
        const nextDocument = new DOMParser().parseFromString(html, "text/html");
        const currentPanel = document.querySelector<HTMLElement>(panelSelector);
        const nextPanel = nextDocument.querySelector<HTMLElement>(panelSelector);

        if (!currentPanel || !nextPanel) {
            window.location.href = responseUrl || window.location.href;
            return;
        }

        currentPanel.replaceWith(nextPanel);
        syncRowButtons(nextDocument);
    }

    function sendFeaturedForm(form: HTMLFormElement, submitter: HTMLButtonElement | null) {
        const action = submitAction(form, submitter);
        if (!action || submitter?.disabled) {
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
            .then((response) => {
                if (!response.ok) {
                    throw new Error("Richiesta homepage non riuscita");
                }

                return response.text().then((html) => {
                    syncFeaturedPanel(html, response.url);
                });
            })
            .catch(() => {
                form.setAttribute("action", action);
                form.submit();
            })
            .finally(() => {
                setBusy(form, submitter, false);
                lastSubmitter = null;
            });
    }

    document.addEventListener("click", (event) => {
        const button = closest<HTMLButtonElement>(event.target, "button[type='submit']");
        if (button) {
            lastSubmitter = button;
        }
    }, true);

    document.addEventListener("submit", (event) => {
        const form = event.target instanceof HTMLFormElement ? event.target : null;
        if (!form) {
            return;
        }

        const submitEvent = event as SubmitEvent;
        const submitter = submitEvent.submitter instanceof HTMLButtonElement
            ? submitEvent.submitter
            : lastSubmitter;
        const isFeaturedPanelForm = form.matches(formSelector);
        const isFeaturedRowAction = submitter?.matches(rowButtonSelector) === true;

        if (!isFeaturedPanelForm && !isFeaturedRowAction) {
            return;
        }

        event.preventDefault();
        sendFeaturedForm(form, submitter);
    });
})();
