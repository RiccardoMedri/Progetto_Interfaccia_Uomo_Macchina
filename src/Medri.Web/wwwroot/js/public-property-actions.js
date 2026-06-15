"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g = Object.create((typeof Iterator === "function" ? Iterator : Object).prototype);
    return g.next = verb(0), g["throw"] = verb(1), g["return"] = verb(2), typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (g && (g = 0, op[0] && (_ = 0)), _) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
(function () {
    var compareKey = "medri.comparePropertyIds";
    var maxCompareItems = 4;
    function bindFavoriteForms() {
        var _this = this;
        document.querySelectorAll("[data-favorite-form]").forEach(function (form) {
            form.addEventListener("submit", function (event) { return __awaiter(_this, void 0, void 0, function () {
                var propertyId, response, result, _a;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
                            if (document.body.dataset.authenticated !== "true") {
                                return [2];
                            }
                            event.preventDefault();
                            event.stopPropagation();
                            propertyId = form.dataset.propertyId;
                            if (!propertyId) {
                                return [2];
                            }
                            setFavoriteControlsBusy(propertyId, true);
                            _b.label = 1;
                        case 1:
                            _b.trys.push([1, 4, 5, 6]);
                            return [4, fetch(form.action, {
                                    method: "POST",
                                    body: new FormData(form),
                                    credentials: "same-origin",
                                    headers: {
                                        "X-Requested-With": "XMLHttpRequest",
                                        "Accept": "application/json"
                                    }
                                })];
                        case 2:
                            response = _b.sent();
                            if (response.redirected && response.url.includes("/login")) {
                                window.location.href = response.url;
                                return [2];
                            }
                            if (!response.ok) {
                                throw new Error("Favorite request failed with status ".concat(response.status, "."));
                            }
                            return [4, response.json()];
                        case 3:
                            result = _b.sent();
                            if (!result.succeeded) {
                                throw new Error("Favorite request was not completed.");
                            }
                            syncFavoriteControls(result.propertyId, result.isSaved);
                            return [3, 6];
                        case 4:
                            _a = _b.sent();
                            return [3, 6];
                        case 5:
                            setFavoriteControlsBusy(propertyId, false);
                            return [7];
                        case 6: return [2];
                    }
                });
            }); });
        });
    }
    function syncFavoriteControls(propertyId, isSaved) {
        document.querySelectorAll("[data-favorite-form][data-property-id=\"".concat(CSS.escape(propertyId), "\"]")).forEach(function (form) {
            form.action = isSaved ? "/preferiti/rimuovi" : "/preferiti/aggiungi";
        });
        document.querySelectorAll("[data-favorite-button][data-property-id=\"".concat(CSS.escape(propertyId), "\"]")).forEach(function (button) {
            var title = button.dataset.propertyTitle || "annuncio";
            button.classList.toggle("is-selected", isSaved);
            button.setAttribute("aria-pressed", isSaved ? "true" : "false");
            button.setAttribute("aria-label", isSaved ? "Rimuovi ".concat(title, " dai preferiti") : "Salva ".concat(title, " nei preferiti"));
        });
    }
    function setFavoriteControlsBusy(propertyId, isBusy) {
        document.querySelectorAll("[data-favorite-button][data-property-id=\"".concat(CSS.escape(propertyId), "\"]")).forEach(function (button) {
            button.disabled = isBusy;
            button.setAttribute("aria-busy", isBusy ? "true" : "false");
        });
    }
    function getStorage() {
        try {
            return window.sessionStorage;
        }
        catch (_a) {
            return null;
        }
    }
    function readIds(key) {
        try {
            var storage = getStorage();
            var value = storage === null || storage === void 0 ? void 0 : storage.getItem(key);
            if (!value) {
                return [];
            }
            var parsed = JSON.parse(value);
            if (!Array.isArray(parsed)) {
                return [];
            }
            return parsed
                .filter(function (id) { return typeof id === "string"; })
                .filter(function (id, index, ids) { return ids.indexOf(id) === index; })
                .slice(0, maxCompareItems);
        }
        catch (_a) {
            return [];
        }
    }
    function writeIds(key, ids) {
        var storage = getStorage();
        if (!storage) {
            return;
        }
        var normalizedIds = ids
            .filter(Boolean)
            .filter(function (id, index, values) { return values.indexOf(id) === index; })
            .slice(0, maxCompareItems);
        storage.setItem(key, JSON.stringify(normalizedIds));
    }
    function toggleId(key, id, maxItems) {
        var ids = readIds(key);
        var existingIndex = ids.indexOf(id);
        if (existingIndex >= 0) {
            ids.splice(existingIndex, 1);
        }
        else if (!maxItems || ids.length < maxItems) {
            ids.push(id);
        }
        writeIds(key, ids);
    }
    function syncComparisonPageWithQuery() {
        var _a;
        if (window.location.pathname !== "/confronto") {
            return;
        }
        var ids = ((_a = new URLSearchParams(window.location.search)
            .get("ids")) === null || _a === void 0 ? void 0 : _a.split(",").map(function (id) { return id.trim(); }).filter(Boolean)) || [];
        writeIds(compareKey, ids);
    }
    function syncButtons(selector, key) {
        var ids = readIds(key);
        document.querySelectorAll(selector).forEach(function (button) {
            var id = button.dataset.propertyId || "";
            var isSelected = ids.indexOf(id) >= 0;
            button.classList.toggle("is-selected", isSelected);
            button.setAttribute("aria-pressed", isSelected ? "true" : "false");
        });
    }
    function updateCompareLinks() {
        var ids = readIds(compareKey);
        var canCompare = ids.length >= 2;
        var href = canCompare ? "/confronto?ids=".concat(encodeURIComponent(ids.join(","))) : "";
        document.querySelectorAll("[data-compare-link]").forEach(function (link) {
            if (canCompare) {
                link.href = href;
                link.removeAttribute("aria-disabled");
                link.removeAttribute("tabindex");
            }
            else {
                link.removeAttribute("href");
                link.setAttribute("aria-disabled", "true");
                link.setAttribute("tabindex", "-1");
            }
            link.classList.toggle("is-disabled", !canCompare);
        });
        document.querySelectorAll("[data-compare-count]").forEach(function (element) {
            element.textContent = ids.length === 1 ? "1 immobile selezionato" : "".concat(ids.length, " immobili selezionati");
        });
    }
    function bindCompareButtons() {
        document.querySelectorAll("[data-compare-button]").forEach(function (button) {
            button.addEventListener("click", function (event) {
                event.preventDefault();
                event.stopPropagation();
                var id = button.dataset.propertyId;
                if (!id) {
                    return;
                }
                toggleId(compareKey, id, maxCompareItems);
                syncButtons("[data-compare-button]", compareKey);
                updateCompareLinks();
            });
        });
    }
    function bindCompareButtonEventShield() {
        document.querySelectorAll("[data-compare-button]").forEach(function (button) {
            ["pointerdown", "mousedown", "mouseup"].forEach(function (eventName) {
                button.addEventListener(eventName, function (event) {
                    event.stopPropagation();
                }, true);
            });
        });
    }
    function bindPrintButtons() {
        document.querySelectorAll("[data-print-button]").forEach(function (button) {
            button.addEventListener("click", function () { return window.print(); });
        });
    }
    function bindShareButtons() {
        var _this = this;
        document.querySelectorAll("[data-share-button]").forEach(function (button) {
            button.addEventListener("click", function () { return __awaiter(_this, void 0, void 0, function () {
                var shareData, _a;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
                            shareData = {
                                title: document.title,
                                url: window.location.href
                            };
                            _b.label = 1;
                        case 1:
                            _b.trys.push([1, 6, , 7]);
                            if (!navigator.share) return [3, 3];
                            return [4, navigator.share(shareData)];
                        case 2:
                            _b.sent();
                            return [3, 5];
                        case 3:
                            if (!navigator.clipboard) return [3, 5];
                            return [4, navigator.clipboard.writeText(shareData.url)];
                        case 4:
                            _b.sent();
                            button.setAttribute("aria-label", "Link confronto copiato");
                            _b.label = 5;
                        case 5: return [3, 7];
                        case 6:
                            _a = _b.sent();
                            return [3, 7];
                        case 7: return [2];
                    }
                });
            }); });
        });
    }
    function bindSavedSearchForms() {
        var _this = this;
        document.querySelectorAll("#saveSearchDesktop").forEach(function (form) {
            form.addEventListener("submit", function (event) { return __awaiter(_this, void 0, void 0, function () {
                var _a;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
                            if (document.body.dataset.authenticated !== "true") {
                                return [2];
                            }
                            event.preventDefault();
                            _b.label = 1;
                        case 1:
                            _b.trys.push([1, 3, , 4]);
                            return [4, fetch(form.action, {
                                    method: "POST",
                                    body: new FormData(form),
                                    credentials: "same-origin",
                                    headers: {
                                        "X-Requested-With": "XMLHttpRequest"
                                    }
                                })];
                        case 2:
                            _b.sent();
                            document.querySelectorAll("[data-save-search-button]").forEach(function (button) {
                                button.classList.add("is-selected");
                                button.setAttribute("aria-pressed", "true");
                                button.textContent = "Ricerca salvata";
                            });
                            return [3, 4];
                        case 3:
                            _a = _b.sent();
                            return [3, 4];
                        case 4: return [2];
                    }
                });
            }); });
        });
    }
    function bindPriceRangeOutputs() {
        document.querySelectorAll("[data-advanced-filters-panel]").forEach(function (panel) {
            panel.querySelectorAll("[data-price-range-input]").forEach(function (input) {
                var output = panel.querySelector("[data-price-range-output=\"".concat(input.dataset.priceRangeRole, "\"]"));
                var update = function () {
                    if (output) {
                        output.textContent = input.value;
                    }
                };
                input.addEventListener("input", update);
                update();
            });
        });
    }
    function bindSearchCardNavigation() {
        document.querySelectorAll("[data-property-detail-url]").forEach(function (card) {
            card.addEventListener("click", function (event) {
                var target = event.target;
                if (target === null || target === void 0 ? void 0 : target.closest("a, button, input, label, select, textarea, form")) {
                    return;
                }
                var url = card.dataset.propertyDetailUrl;
                if (url) {
                    window.location.href = url;
                }
            });
        });
    }
    function bindSearchFiltersDialog() {
        var dialog = document.querySelector("[data-search-filters-dialog]");
        var openButton = document.querySelector("[data-search-filters-open]");
        if (!dialog || !openButton) {
            return;
        }
        dialog.querySelectorAll("input, select, textarea").forEach(function (control) {
            if (!control.hasAttribute("form")) {
                control.setAttribute("form", "searchFiltersDesktop");
            }
        });
        var basicGroup = dialog.querySelector("[data-search-drawer-basic].d-md-none");
        if (basicGroup) {
            var desktopQuery_1 = window.matchMedia("(min-width: 768px)");
            var syncBasicGroup = function () {
                basicGroup.querySelectorAll("input").forEach(function (input) {
                    input.disabled = desktopQuery_1.matches;
                });
            };
            syncBasicGroup();
            desktopQuery_1.addEventListener("change", syncBasicGroup);
        }
        var open = function () {
            dialog.hidden = false;
            window.requestAnimationFrame(function () { return dialog.classList.add("is-open"); });
            openButton.setAttribute("aria-expanded", "true");
        };
        var close = function () {
            dialog.classList.remove("is-open");
            openButton.setAttribute("aria-expanded", "false");
            window.setTimeout(function () {
                if (!dialog.classList.contains("is-open")) {
                    dialog.hidden = true;
                }
            }, 320);
        };
        openButton.addEventListener("click", function (event) {
            event.preventDefault();
            open();
        });
        dialog.querySelectorAll("[data-search-filters-close]").forEach(function (control) {
            control.addEventListener("click", function (event) {
                event.preventDefault();
                close();
            });
        });
        document.addEventListener("keydown", function (event) {
            if (event.key === "Escape" && dialog.classList.contains("is-open")) {
                close();
                openButton.focus();
            }
        });
    }
    function bindNotificationPreferenceForms() {
        document.querySelectorAll("[data-notification-preference-form]").forEach(function (form) {
            var stateToggle = form.querySelector("[data-notification-state-toggle]");
            var stateLabel = form.querySelector("[data-notification-state-label]");
            var frequencyGroup = form.querySelector("[data-notification-frequency-group]");
            var syncState = function () {
                var isActive = (stateToggle === null || stateToggle === void 0 ? void 0 : stateToggle.checked) === true;
                if (stateLabel) {
                    stateLabel.textContent = isActive ? "Attivo" : "Non attivo";
                }
                if (frequencyGroup) {
                    frequencyGroup.disabled = !isActive;
                    frequencyGroup.classList.toggle("is-disabled", !isActive);
                }
            };
            syncState();
            form.querySelectorAll('input[type="checkbox"]').forEach(function (input) {
                input.addEventListener("change", function () {
                    syncState();
                    form.requestSubmit();
                });
            });
        });
    }
    document.addEventListener("DOMContentLoaded", function () {
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
