/* --- Global Variable --- */
var objQUS010;
var objQUS040;

/* --- Methods --- */
$.fn.OpenAfterContractMenu = function () {
    var event = {
        Close: function () {
            $(this).CloseDialog();
        }
    }
    $(this).OpenPopupDialog("/Contract/Contract_After", "", 900, 600, event, ["Select"]);
}

$.fn.OpenBeforeContractMenu = function () {
    var event = {
        Close: function () {
            $(this).CloseDialog();
        }
    }
    $(this).OpenPopupDialog("/Contract/Contract_Before", "", 900, 600, event, ["Select"]);
}

$.fn.OpenIncidentARContractMenu = function () {
    var event = {
        Close: function () {
            $(this).CloseDialog();
        }
    }
    $(this).OpenPopupDialog("/Contract/Contract_IncidentAR", "", 900, 600, event, ["Select"]);
}

$.fn.OpenARContractMenu = function () {
    var event = {
        Close: function () {
            $(this).CloseDialog();
        }
    }
    $(this).OpenPopupDialog("/Contract/Contract_AR", "", 900, 600, event, ["Select"]);
}

$.fn.OpenProjectContractMenu = function () {
    var event = {
        Close: function () {
            $(this).CloseDialog();
        }
    }
    $(this).OpenPopupDialog("/Contract/Contract_Project", "", 900, 600, event, ["Select"]);
}

$.fn.OpenCTS261Dialog = function (caller_id) {

    var ctrl = $(this);

    var ObjCTS261 = {};
    if (typeof (CTS261Object) == 'function') {
        ObjCTS261 = CTS261Object();
    }

    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Contract/CTS261", ObjCTS261, function (result) {
        if (result != undefined) {
            var event = {
                Cancel: function () {
                    ctrl.CloseDialog();
                }
            };

            var open_event = function (event, ui) {
                if (typeof (CTS261Initial) == "function")
                    CTS261Initial();
            };


            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);


        }

    });
}

