
/* --- BLS031 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. BLS031Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. BLS031Response(result) in Caller Screen ----------------- */
/* ----------- 3. BLS031Object() in Caller Screen ------------------------- */
/* ----------- 4. BLS031ClearPopupSession() in Popup Screen --------------- */
/* ------------------------------------------------------------------------ */
/* ------------------------------------------------------------------------ */
$.fn.OpenBLS031Dialog = function (caller_id) {
    /// <summary>Create BLS031 Dialog</summary>

    var objParam = "";
    if (typeof (BLS031Object) == "function")
        objParam = BLS031Object();

    /* --- Get Data from Server --- */
    /* ---------------------------- */
    var ctrl = $(this);
    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Billing/BLS031", objParam, function (result) {
        if (result != undefined) {
            var event = {
                OK: function () {
                    $(this).CloseDialog();
                },
                Cancel: function () {
                    $(this).CloseDialog();
                }
            };

            var open_event = function (event, ui) {
                if (typeof (BLS031Initial) == "function")
                    BLS031Initial();
            };

            ctrl.OpenPopupDialog(result, "", 980, 350, event, null, open_event);
        }
    });
    /* ---------------------------- */
}
/* ------------------------------------------------------------------------ */



/* --- BLS032 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. BLS032Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. BLS032Response(result) in Caller Screen ----------------- */
/* ----------- 3. BLS032Object() in Caller Screen ------------------------- */
/* ----------- 4. BLS032ClearPopupSession() in Popup Screen --------------- */
/* ------------------------------------------------------------------------ */
/* ------------------------------------------------------------------------ */
$.fn.OpenBLS032Dialog = function (caller_id) {
    /// <summary>Create BLS032 Dialog</summary>

    var objParam = "";
    if (typeof (BLS032Object) == "function")
        objParam = BLS032Object();

    /* --- Get Data from Server --- */
    /* ---------------------------- */
    var ctrl = $(this);
    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Billing/BLS032", objParam, function (result) {
        if (result != undefined) {
            var event = {
                OK: function () {
                    $(this).CloseDialog();
                },
                Cancel: function () {
                    $(this).CloseDialog();
                }
            };

            var open_event = function (event, ui) {
                if (typeof (BLS032Initial) == "function")
                    BLS032Initial();
            };

            ctrl.OpenPopupDialog(result, "", 700, 370, event, null, open_event);
        }
    });
    /* ---------------------------- */
}
/* ------------------------------------------------------------------------ */



/* --- BLS071 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. BLS071Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. BLS071Response(result) in Caller Screen ----------------- */
/* ----------- 3. BLS071Object() in Caller Screen ------------------------- */
/* ----------- 4. BLS071ClearPopupSession() in Popup Screen --------------- */
/* ------------------------------------------------------------------------ */
/* ------------------------------------------------------------------------ */
$.fn.OpenBLS071Dialog = function (caller_id) {
    /// <summary>Create BLS071 Dialog</summary>

    var objParam = "";
    if (typeof (BLS071Object) == "function")
        objParam = BLS071Object();

    /* --- Get Data from Server --- */
    /* ---------------------------- */
    var ctrl = $(this);
    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Billing/BLS071", objParam, function (result) {
        if (result != undefined) {
            var event = {
                OK: function () {
                    $(this).CloseDialog();
                },
                Cancel: function () {
                    $(this).CloseDialog();
                }
            };

            var open_event = function (event, ui) {
                if (typeof (BLS071Initial) == "function")
                    BLS071Initial(objParam);
            };

            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);
        }
    });
    /* ---------------------------- */
}
/* ------------------------------------------------------------------------ */