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
    var gridSelector = "[data-property-media-grid]";
    var itemSelector = "[data-property-media-item]";
    var sortOrderSelector = "[data-property-media-sort-order]";
    var removeButtonSelector = "[data-property-media-remove]";
    var removeInputSelector = "[data-property-media-remove-input]";
    var positionSelector = "[data-property-media-position]";
    var coverSelector = "[data-property-media-cover]";
    var locationRootSelector = "[data-property-location]";
    var locationDisplaySelector = "[data-property-location-display]";
    var locationAddressSelector = "[data-property-location-address]";
    var locationLatitudeSelector = "[data-property-location-latitude]";
    var locationLongitudeSelector = "[data-property-location-longitude]";
    var locationStatusSelector = "[data-property-location-status]";
    var propertyFormSelector = "#propertyDetailForm";
    var uploadMediaAction = "UploadMedia";
    var activeSubmitter = null;
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
    function locationFieldValue(root, selector) {
        var _a;
        return ((_a = root.querySelector(selector)) === null || _a === void 0 ? void 0 : _a.value.trim()) || "";
    }
    function normalizeLocationPart(value) {
        return value
            .replace(/\s+/g, " ")
            .trim();
    }
    function locationQuery(root) {
        var address = normalizeLocationPart(locationFieldValue(root, locationAddressSelector));
        var displayLocation = normalizeLocationPart(locationFieldValue(root, locationDisplaySelector));
        return [address, displayLocation]
            .filter(function (value) { return !!value; })
            .join(" | ");
    }
    function antiForgeryToken(root) {
        var _a;
        var form = closest(root, propertyFormSelector);
        return ((_a = form === null || form === void 0 ? void 0 : form.querySelector("input[name='__RequestVerificationToken']")) === null || _a === void 0 ? void 0 : _a.value) || "";
    }
    function isUploadMediaSubmitter(submitter) {
        return submitter instanceof HTMLButtonElement &&
            submitter.name === "SubmitAction" &&
            submitter.value === uploadMediaAction;
    }
    function setLocationStatus(root, message) {
        var status = root.querySelector(locationStatusSelector);
        if (status) {
            status.textContent = message;
        }
    }
    function coordinateValue(root, selector) {
        var value = locationFieldValue(root, selector).replace(",", ".");
        var parsed = Number.parseFloat(value);
        return Number.isFinite(parsed) ? parsed : null;
    }
    function hasUsableCoordinates(root) {
        var latitude = coordinateValue(root, locationLatitudeSelector);
        var longitude = coordinateValue(root, locationLongitudeSelector);
        return latitude !== null &&
            longitude !== null &&
            latitude >= -90 &&
            latitude <= 90 &&
            longitude >= -180 &&
            longitude <= 180 &&
            (latitude !== 0 || longitude !== 0);
    }
    function needsGeocoding(root) {
        var query = locationQuery(root);
        return !!query &&
            (!hasUsableCoordinates(root) || root.dataset.geocodedQuery !== query);
    }
    function setCoordinates(root, latitude, longitude) {
        var latitudeInput = root.querySelector(locationLatitudeSelector);
        var longitudeInput = root.querySelector(locationLongitudeSelector);
        if (latitudeInput) {
            latitudeInput.value = latitude.toFixed(6);
        }
        if (longitudeInput) {
            longitudeInput.value = longitude.toFixed(6);
        }
        root.dataset.geocodedQuery = locationQuery(root);
    }
    function clearCoordinates(root) {
        var latitudeInput = root.querySelector(locationLatitudeSelector);
        var longitudeInput = root.querySelector(locationLongitudeSelector);
        if (latitudeInput) {
            latitudeInput.value = "";
        }
        if (longitudeInput) {
            longitudeInput.value = "";
        }
        delete root.dataset.geocodedQuery;
    }
    function geocodeLocation(root) {
        return __awaiter(this, void 0, void 0, function () {
            var address, displayLocation, endpoint, body, token, response, result, _a;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        address = normalizeLocationPart(locationFieldValue(root, locationAddressSelector));
                        displayLocation = normalizeLocationPart(locationFieldValue(root, locationDisplaySelector));
                        endpoint = root.dataset.propertyLocationGeocodeUrl || "";
                        if (!address) {
                            setLocationStatus(root, "Inserisci un indirizzo preciso.");
                            return [2, false];
                        }
                        if (!endpoint) {
                            clearCoordinates(root);
                            setLocationStatus(root, "Mappa non disponibile.");
                            return [2, false];
                        }
                        setLocationStatus(root, "Ricerca posizione...");
                        _b.label = 1;
                    case 1:
                        _b.trys.push([1, 4, , 5]);
                        body = new URLSearchParams();
                        body.set("Address", address);
                        body.set("DisplayLocation", displayLocation);
                        token = antiForgeryToken(root);
                        if (token) {
                            body.set("__RequestVerificationToken", token);
                        }
                        return [4, fetch(endpoint, {
                                method: "POST",
                                body: body,
                                credentials: "same-origin",
                                headers: {
                                    "X-Requested-With": "XMLHttpRequest",
                                    "Accept": "application/json",
                                    "Content-Type": "application/x-www-form-urlencoded;charset=UTF-8"
                                }
                            })];
                    case 2:
                        response = _b.sent();
                        if (!response.ok) {
                            clearCoordinates(root);
                            setLocationStatus(root, "Mappa non disponibile.");
                            return [2, false];
                        }
                        return [4, response.json()];
                    case 3:
                        result = _b.sent();
                        if (!result.succeeded || typeof result.latitude !== "number" || typeof result.longitude !== "number") {
                            clearCoordinates(root);
                            setLocationStatus(root, result.message || "Indirizzo non trovato. Inserisci via, numero civico, comune e provincia.");
                            return [2, false];
                        }
                        setCoordinates(root, result.latitude, result.longitude);
                        setLocationStatus(root, "Posizione mappa aggiornata.");
                        return [2, true];
                    case 4:
                        _a = _b.sent();
                        clearCoordinates(root);
                        setLocationStatus(root, "Mappa non disponibile.");
                        return [2, false];
                    case 5: return [2];
                }
            });
        });
    }
    function resubmitPropertyForm(form, submitter) {
        form.dataset.propertyLocationSubmitting = "true";
        var requestSubmit = form.requestSubmit;
        if (requestSubmit) {
            requestSubmit.call(form, submitter || undefined);
            return;
        }
        form.submit();
    }
    function bindLocationGeocoder(root) {
        var displayInput = root.querySelector(locationDisplaySelector);
        var addressInput = root.querySelector(locationAddressSelector);
        if (hasUsableCoordinates(root)) {
            root.dataset.geocodedQuery = locationQuery(root);
        }
        [displayInput, addressInput].forEach(function (input) {
            if (!input) {
                return;
            }
            input.addEventListener("input", function () {
                clearCoordinates(root);
                setLocationStatus(root, "");
            });
            input.addEventListener("change", function () {
                if (needsGeocoding(root)) {
                    void geocodeLocation(root);
                }
            });
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
    document.addEventListener("click", function (event) {
        var submitter = closest(event.target, "button[type='submit'], input[type='submit']");
        if (submitter) {
            activeSubmitter = submitter;
        }
    });
    document.addEventListener("submit", function (event) {
        var form = event.target instanceof HTMLFormElement ? event.target : null;
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
        var root = form.querySelector(locationRootSelector);
        if (!root || !needsGeocoding(root)) {
            return;
        }
        event.preventDefault();
        void geocodeLocation(root).finally(function () { return resubmitPropertyForm(form, activeSubmitter); });
    });
    document.querySelectorAll(gridSelector).forEach(updateMediaOrder);
    document.querySelectorAll(locationRootSelector).forEach(bindLocationGeocoder);
})();
