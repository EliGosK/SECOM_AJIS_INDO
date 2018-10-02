
/* --- ICS082 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. ICS082Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. ICS082Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenICS082Dialog = function (caller_id, objParam) {
    var ctrl = $(this);

    var param = {};
    if (objParam != undefined) {
        param = objParam;
    }
    else if (typeof (ICS082Object) == "function") {
        param = ICS082Object();
    }

    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Income/ICS082", param, function (result) {
        if (result != undefined) {
            var event = {
                Close: function (data) {
                    ctrl.CloseDialog();
                }
            };

            var open_event = function (event, ui) {
                if (typeof (ICS082Initial) == "function") {
                    ICS082Initial();
                }
            };
            ctrl.OpenPopupDialog(result, "", 1000, 600, event, null, open_event);
        }
    });
}

//$.fn.OpenICS082DialogWithParam = function (caller_id, objParam) {
//    var ctrl = $(this);

//    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Income/ICS082", objParam, function (result) {
//        if (result != undefined) {
//            var event = {
//                Close: function (data) {
//                    ctrl.CloseDialog();
//                }
//            };

//            var open_event = function (event, ui) {
//                if (typeof (ICS082Initial) == "function") {
//                    ICS082Initial();
//                }
//            };
//            ctrl.OpenPopupDialog(result, "", 1000, 600, event, null, open_event);
//        }
//    });
//}

/* --- ICS083 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. ICS083Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. ICS083Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenICS083Dialog = function (caller_id, objParam) {
    var ctrl = $(this);
    var param = {};

    if (objParam != undefined) {
        param = objParam;
    }
    else if (typeof (ICS083Object) == "function") {
        param = ICS083Object();
    }

    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Income/ICS083", param, function (result) {
        if (result != undefined) {
            var event = {
                Close: function (data) {
                    ctrl.CloseDialog();
                }
            };

            var open_event = function (event, ui) {
                if (typeof (ICS083Initial) == "function") {
                    ICS083Initial();
                }
            };

            ctrl.OpenPopupDialog(result, "", 1000, 600, event, null, open_event);
        }
    });
}


/* --- ICS033 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. ICS033Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. ICS033Response(result) in Caller Screen ----------------- */
/* ----------- 3. ICS033Object() in Caller Screen ------------------------- */
/* ----------- 4. ICS033ClearPopupSession() in Popup Screen --------------- */
/* ------------------------------------------------------------------------ */
/* ------------------------------------------------------------------------ */
$.fn.OpenICS033Dialog = function (caller_id, objParam) {
    /// <summary>Create ICS033 Dialog</summary>

    var param = {};
    if (objParam != undefined) {
        param = objParam;
    }
    else if (typeof (ICS033Object) == "function") {
        objParam = ICS033Object();
    }

    /* --- Get Data from Server --- */
    /* ---------------------------- */
    var ctrl = $(this);
    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Income/ICS033", objParam, function (result) {
        if (result != undefined) {
            var event = {
                Close: function (data) {
                    $(this).CloseDialog();
                }
            };

            var open_event = function (event, ui) {
                if (typeof (ICS033Initial) == "function") {
                    ICS033Initial(objParam);
                }
            };

            ctrl.OpenPopupDialog(result, "", 1050, 500, event, null, open_event);
        }
    });
    /* ---------------------------- */
}
/* ------------------------------------------------------------------------ */