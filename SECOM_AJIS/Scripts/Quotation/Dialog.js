/// <reference path="../Base/Master.js" />

/* --- Global Variable --- */



/* --- Methods --- */
$.fn.OpenQUS010Dialog = function (caller_id) {
    var ctrl = $(this);
    var ObjToQus010;
    if (typeof (QUS010Object) == 'function') {
        ObjToQus010 = QUS010Object();
    } else {
        ObjToQus010 = {};
    }
    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, '/quotation/QUS010', ObjToQus010, function (result) {
        if (result != undefined) {
            var event = {
                Close: function (data) {
                    ctrl.CloseDialog();
                }
            };

            var open_event = function (event, ui) {
                if (typeof (QUS010Initial) == 'function') {
                    QUS010Initial();
                }
            };
            
            ctrl.OpenPopupDialog(result, "", 1000, 600, event, null, open_event);
        }
    });
}



/* --- QUS011 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. QUS011Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. QUS011Response(result) in Caller Screen ----------------- */
/* ----------- 3. QUS011Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenQUS011Dialog = function (caller_id) {
    /// <summary>Create QUS011 Dialog</summary>
    /// <param name="caller_id" type="string">Caller Screen ID</param>

    var ctrl = $(this);

    /* --- Set Parameters --- */
    /* ---------------------- */
    var objParam = "";
    var obj = CallBackCallerFunction("QUS011Object");
    if (obj != undefined) {
        objParam = {
            Condition: {
                QuotationTargetCode: obj.QuotationTargetCode,
                Alphabet: obj.Alphabet
            },
            HideQuotationTarget: obj.HideQuotationTarget
        }
    }
    /* ---------------------- */

    /* --- Get Data from Server --- */
    /* ---------------------------- */
    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Quotation/QUS011", objParam, function (result) {
        if (result != undefined) {
            /* --- Button Events --- */
            /* --------------------- */
            var event = {
                Close: function (data) {
                    ctrl.CloseDialog();
                }
            };
            /* --------------------- */

            /* --- Event when open Dialog --- */
            /* ------------------------------ */
            var open_event = function (event, ui) {
                if (typeof (QUS011Initial) == "function")
                    QUS011Initial();
            };
            /* ------------------------------ */

            ctrl.OpenPopupDialog(result, "", 900, 600, event, false, open_event, null, true);
        }
    }, true);
    /* ---------------------------- */
}
/* ------------------------------------------------------------------------ */

/* --- QUS012 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. QUS012Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. QUS012Response(result) in Caller Screen ----------------- */
/* ----------- 3. QUS012Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenQUS012Dialog = function (caller_id) {
    /// <summary>Create QUS012 Dialog</summary>
    /// <param name="caller_id" type="string">Caller Screen ID</param>

    var ctrl = $(this);

   
    /* --- Set Parameters --- */
    /* ---------------------- */
    var objParam = "";
    var obj = CallBackCallerFunction("QUS012Object");

    //alert(obj.QuotationTargetCode);
    if (obj != undefined) {
        objParam = {
            Condition: {
                QuotationTargetCode: obj.QuotationTargetCode,
                Alphabet: obj.Alphabet
            },
            HideQuotationTarget: obj.HideQuotationTarget
        }
    }
    /* ---------------------- */

    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Quotation/QUS012", objParam, function (result) {
        if (result != undefined) {
            /* --- Button Events --- */
            /* --------------------- */
            var event = {
                Close: function (data) {
                    ctrl.CloseDialog();
                }
            };
            /* --------------------- */

            /* --- Event when open Dialog --- */
            /* ------------------------------ */
            var open_event = function (event, ui) {
                if (typeof (QUS012Initial) == "function")
                    QUS012Initial();
            };
            /* ------------------------------ */

            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event, null, true);
        }
    }, true);
}
/* ------------------------------------------------------------------------ */

$.fn.OpenQUS040Dialog = function (caller_id) {
    /// <summary>Create QUS040 Dialog</summary>
    /// <param name="caller_id" type="string">Caller Screen ID</param>
    var ctrl = $(this);

    /* --- Set Parameters --- */
    /* ---------------------- */

    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Quotation/QUS040", "", function (result) {
        if (result != undefined) {
            /* --- Button Events --- */
            /* --------------------- */
            var event = {
                Close: function (data) {
                    ctrl.CloseDialog();
                }
            };
            /* --------------------- */

            /* --- Event when open Dialog --- */
            /* ------------------------------ */

            var open_event = function (event, ui) {
                if (typeof (QUS040Initial) == 'function')
                    QUS040Initial();
            };
            /* ------------------------------ */

            ctrl.OpenPopupDialog(result, "", 1000, 600, event, null, open_event);

        }
    });

}

/* --- QUS050 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. QUS050Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. QUS050Response(result) in Caller Screen ----------------- */
/* ----------- 3. QUS050Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenQUS050Dialog = function (caller_id) {
    /// <summary>Create QUS050 Dialog</summary>
    /// <param name="caller_id" type="string">Caller Screen ID</param>

    var ctrl = $(this);
    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Quotation/QUS050", "", function (result) {
        /* --- Event when open Dialog --- */
        var event = {
            Import: function () {
                /* --- Get Message --- */
                var obj = {
                    module: "Quotation",
                    code: "MSG2018"
                };

                /* --- Merge --- */
                /* call_ajax_method("/Shared/GetMessage", obj, function (result) {
                    OpenInformationMessageDialog(result.Code, result.Message);
                }); */
                call_ajax_method("/Shared/GetMessage", obj, function (result) {
                    OpenErrorMessageDialog(result.Code, result.Message);
                });
                /* ------------- */
                
            },
            Cancel: function () {
                CloseWarningDialog();
                ctrl.CloseDialog();
            }
        };

        /* --- Event when open Dialog --- */
        var open_event = function (event, ui) {
            if (typeof (QUS050Initial) == "function")
                QUS050Initial();
        };

        ctrl.OpenPopupDialog(result, "", 600, 250, event, null, open_event);
    });

}
/* ------------------------------------------------------------------------ */