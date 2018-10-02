/// <reference path="../../Scripts  /jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>

/* --- Infomation Dialog ------------------------------- */
/* ----------------------------------------------------- */
function InitialMessageDialog(type, code, msg, event) {
    /// <summary>Message to initial message dialog</summary>
    /// <param name="type" type="int">Message Type >> 0: Error, 1: Information, 2: Question, 3: Warning</param>
    /// <param name="code" type="string">Message code</param>
    /// <param name="msg" type="string">Message</param>
    /// <param name="event" type="object">Button event</param>

    $("#msg_info_dialog").dialog({
        modal: true,
        resizable: false,
        title: code,
        width: 400,
        height: 220,
        buttons: event,
        closeOnEscape: false,
    });

    var img = "<img src=\"../../Content/images/dialog/error.png\" />";
    if (type == 1)
        img = "<img src=\"../../Content/images/dialog/information.png\" />";
    else if (type == 2)
        img = "<img src=\"../../Content/images/dialog/question.png\" />";
    else if (type == 3)
        img = "<img src=\"../../Content/images/dialog/warning.png\" />";

    $("#msg_info_dialog").children("div[class=msg-info-image]:first").html(img);

    /* --- Merge --- */
    /* $("#msg_info_dialog").children("div[class=msg-popup-dialog]:first").html(msg); */
    $("#msg_info_dialog").children("div[class=msg-popup-dialog]:first").html(ConvertSSH(msg));
    /* ------------- */
    
    
    $("#msg_info_dialog").parent().children(":nth-child(1)").children("a").css("display", "none");
    $("#msg_info_dialog").SetDialogStyle();

    var ctrl_name = ["OK", "Cancel", "Yes", "No"];
    var new_ctrl_name = [
        $("#DialogBtnOK").val(),
        $("#DialogBtnCancel").val(),
        $("#DialogBtnYes").val(),
        $("#DialogBtnNo").val() ];
    if (ctrl_name != null && new_ctrl_name != null) {
        if (ctrl_name.length == new_ctrl_name.length) {
            var ctrl = $("#msg_info_dialog").parent();
            ctrl.children(":nth-child(3)").children().children("button").each(function () {
                for (var idx = 0; idx < ctrl_name.length; idx++) {
                    if ($(this).text() == ctrl_name[idx]) {
                        $(this).text(new_ctrl_name[idx])
                    }
                }
            });
        }
    }
}
$.fn.CloseMessageDialog = function () {
    $(this).dialog("close");
    $(this).dialog("destroy");
}
/* ----------------------------------------------------- */

/* --- Error Message Dialog ---------------------------- */
/* ----------------------------------------------------- */
function OpenErrorMessageDialog(code, msg, func) {
    /// <summary>Create Error Message Dialog</summary>
    /// <param name="code" type="string">Message Code</param>
    /// <param name="msg" type="string">Message</param>
    /// <param name="func" type="string">Method when click "Yes"</param>

    var event = {
        OK: function () {
            $(this).CloseMessageDialog();
            if (func != null)
                func();
        }
    };

    InitialMessageDialog(0, code, msg, event);
}
/* ----------------------------------------------------- */

/* --- Information Message Dialog ---------------------- */
/* ----------------------------------------------------- */
function OpenInformationMessageDialog(code, msg, func) {
    /// <summary>Create Information Message Dialog</summary>
    /// <param name="code" type="string">Message Code</param>
    /// <param name="msg" type="string">Message</param>
    /// <param name="func" type="string">Method when click "Yes"</param>

    var event = {
        OK: function () {
            $(this).CloseMessageDialog();
            if (func != null)
                func();
        }
    };

    InitialMessageDialog(1, code, msg, event);
}
/* ----------------------------------------------------- */

/*--- Confirm Dialog [Yes/No] -------------------------- */
/* ----------------------------------------------------- */
function OpenYesNoMessageDialog(code, msg, yes_func, no_func) {
    /// <summary>Create Confirm Message [Yes/No] Dialog</summary>
    /// <param name="code" type="string">Message Code</param>
    /// <param name="msg" type="string">Message</param>
    /// <param name="yes_func" type="string">Method when click "Yes"</param>
    /// <param name="no_func" type="string">Method when click "No"</param>

    var event = {
        Yes: function () {
            $(this).CloseMessageDialog();

            if (yes_func != null)
                yes_func();
        },
        No: function () {
            $(this).CloseMessageDialog();

            if (no_func != null)
                no_func();
        }
    };

    InitialMessageDialog(2, code, msg, event);
}
/* ----------------------------------------------------- */

/*--- Confirm Dialog [OK/Cancel] ----------------------- */
/* ----------------------------------------------------- */
function OpenOkCancelDialog(code, msg, ok_func, cancel_func) {
    /// <summary>Create Confirm Message [OK/Cancel] Dialog</summary>
    /// <param name="code" type="string">Message Code</param>
    /// <param name="msg" type="string">Message</param>
    /// <param name="ok_func" type="function">Method when click "Yes"</param>
    /// <param name="cancel_func" type="function">Method when click "No"</param>

    var event = {
        OK: function () {
            $(this).CloseMessageDialog();

            if (ok_func != null)
                ok_func();
        },
        Cancel: function () {
            $(this).CloseMessageDialog();

            if (cancel_func != null)
                cancel_func();
        }
    };

    InitialMessageDialog(2, code, msg, event);
}
/* ----------------------------------------------------- */

/* --- Warning Dialog ---------------------------------- */
/* ----------------------------------------------------- */
function OpenYesNoWarningMessageDialog(code, msg, func, nofunc) {
    /// <summary>Create Information Message Dialog</summary>
    /// <param name="code" type="string">Message Code</param>
    /// <param name="msg" type="string">Message</param>
    /// <param name="func" type="string">Method when click "Yes"</param>

    var event = {
        Yes: function () {
            $(this).CloseMessageDialog();
            if (func != null)
                func();
        },
        No: function () {
            $(this).CloseMessageDialog();
            if (nofunc != null)
                nofunc();
        }
    };

    InitialMessageDialog(3, code, msg, event);
}
function OpenWarningMessageDialog(code, msg, func) {
    /// <summary>Create Information Message Dialog</summary>
    /// <param name="code" type="string">Message Code</param>
    /// <param name="msg" type="string">Message</param>
    /// <param name="func" type="string">Method when click "Yes"</param>

    var event = {
        OK: function () {
            $(this).CloseMessageDialog();
            if (func != null)
                func();
        }
    };

    InitialMessageDialog(3, code, msg, event);
}

$(document).ready(function () {
    $("#msg_warning_dialog").ajaxStart(function () {
        CloseWarningDialog();
    });
});
function OpenWarningDialog(msg) {

    /* --- Merge --- */
    /* $("#msg_warning_dialog div[class=warning-popup-dialog]").html(msg); */
    $("#msg_warning_dialog div[class=warning-popup-dialog]").html(ConvertSSH(msg));
    /* ------------- */
    

    var width = parseInt($(document).width()) - 330;
    var height = parseInt($(document).height()) - 320;
    
    $("#msg_warning_dialog").dialog({
        modal: false,
        resizable: false,
        title: $("#DialogTitleError").val(),
        position: [width, height],
        width: 300,
        height: 300
    });

    var ctrl = $("#msg_warning_dialog");
    var ctrlP1 = ctrl.parent();

    ctrlP1.css({
        "background-color": "#FF9999",
        "border-color": "#FF0000"
    });
    ctrlP1.children().each(function (index) {
        if (index == 0) {
            $(this).css({
                "background": "none",
                "background-color": "#FF6565"
            });
        }
        else {
            $(this).css({
                "background-color": "#FFE2E2"
            });
        }
    });

    ctrl.SetDialogStyle();
}
function CloseWarningDialog() {
    $("#msg_warning_dialog").CloseMessageDialog();
}
/* ----------------------------------------------------- */


/* --- Popup Dialog ------------------------------------ */
/* ----------------------------------------------------- */
$.fn.OpenPopupDialog = function (url, obj, width, height, event, hide_ctrl, open_event, close_event, disable_ctrl) {
    /// <summary>Create Popup Dialog</summary>
    /// <param name="url" type="string">Controller Path</param>
    /// <param name="obj" type="string">Input Parameters</param>
    /// <param name="width" type="string">Popup width</param>
    /// <param name="height" type="string">Popup height</param>
    /// <param name="event" type="function">Button Event</param>
    /// <param name="hide_ctrl" type="array">Button name that you want to hide</param>
    /// <param name="open_event" type="function">Event when Open dialog</param>
    /// <param name="close_event" type="function">Event when Close dialog</param>

    if (typeof (event) == "undefined") {
        event = {
            Close: function () {
                $(this).CloseDialog();
            }
        };
    }

    var info = ajax_method.GetScreenInformation(url);
    var ctrl = $(this);

    master_event.LockWindow(true);

    ajax_method.CallScreenController(url, obj, function (result) {
        if (result == undefined)
            return;

        // --- Load Control.
        ctrl.children("div[class=popup-dialog]").html(result);
        ctrl.children("div[class=popup-dialog]").css({
            "width": width - 24 + "px",
            "height": height - 96 + "px"
        });

        // --- Show Dialog
        ctrl.dialog({
            modal: true,
            resizable: false,
            width: width,
            height: height,
            //position: ["center","top"],
            buttons: event,
            open: open_event,
            close: function (event, ui) {
                if (ajax_method.PopupKeys != undefined) {
                    var module = info[0];
                    var key = ajax_method.GetKeyURL(url);
                    $.ajax({
                        type: "POST",
                        url: ajax_method.GenerateURL("/" + module + "/ClearCurrentScreenSession?k=" + key),
                        async: false,
                        success: function (result) {
                            //alert("Success Clear session, remain = " + result);
                        }
                    });
                    //alert("Close popup screen");

                    if (ajax_method.PopupKeys.length > 0) {
                        for (var i = ajax_method.PopupKeys.length - 1; i >= 0; i--) {
                            var s = ajax_method.PopupKeys[i].split(";");
                            if (s[0].toLowerCase() == info[0].toLowerCase()
                                && s[1].toLowerCase() == info[1].toLowerCase()) {
                                ajax_method.PopupKeys.pop();
                            }
                            else {
                                break;
                            }
                        }
                    }
                    if (ajax_method.PopupKeys.length == 0)
                        ajax_method.PopupKeys = null;
                }
                if (typeof (close_event) == "function")
                    close_event();
                ctrl.children("div[class=popup-dialog]").html("");
            }
        });

        // --- Update Style
        ctrl.parent().children(":nth-child(1)").css({
            "display": "none"
        });
        ctrl.parent().children(":nth-child(1)").children("a").css("display", "none");
        ctrl.SetDialogStyle();

        if (hide_ctrl != null) {
            for (var idx = 0; idx < hide_ctrl.length; idx++) {
                ctrl.parent().children(":nth-child(3)").children().children("button").each(function () {
                    if ($(this).text() == hide_ctrl[idx]) {
                        $(this).hide();
                    }
                });
            }
        }

        if (disable_ctrl == true) {
            ctrl.parent().children(":nth-child(3)").children().children("button").each(function () {
                $(this).attr("disabled", true);
            });
        }

        /* --- Merge --- */
        var div = "<div id='divloadingDlg' style='display:none;float:left;margin:3px 0 0 10px;'><div id='divLoadingDefault'>Loading&nbsp;<img src='../../Content/images/loading.gif' /></div></div>";

        if (actionTotal > 0) {
            div = div + "<div id='divPopuploading" + info[1] + "' style='float:left;margin:3px 0 0 10px;'><div>Initialize popup&nbsp;<span id='spanPopupPersent" + info[1] + "'>[0%]</span>&nbsp;<img src='../../Content/images/loading.gif' /></div></div>";


        }
        ctrl.parent().children(":nth-child(3)").append(div);

        $("#divLoadingDefault").show();
        $("#divLoadStatus").hide();
        $("#divloading").hide();

        $(document).find("div[id^=divPopuploading], div[id^=divloadingDlg]").each(function () {
            var id = $(this).attr("id").replace("divPopuploading", "").replace("divloadingDlg", "");
            if (id != info[1]) {
                $(this).hide();
            }

        });
        /* ------------- */

        master_event.LockWindow(false);
    });
}
function EnableControlDialog() {
    $("div[class=ui-dialog-buttonset]").find("button").each(function () {
        $(this).removeAttr("disabled");
    });
}
$.fn.CloseDialog = function () {
    $(this).dialog("close");
    $(this).dialog("destroy");
}
function DialogButtonClick(ctrl_name)
{
    var ctrl = $("div[class=popup-dialog]").parent();
    ctrl.parent().children(":nth-child(3)").children().children("button").each(function () {
        if ($(this).text() == ctrl_name) {
            $(this).click();
        }
    });
}
function BindDialogButtonClick(ctrl_name, func) {
    var ctrl = $("div[class=popup-dialog]").parent();
    ctrl.parent().children(":nth-child(3)").children().children("button").each(function () {
        if ($(this).text() == ctrl_name) {
            $(this).unbind("click");
            $(this).click(func);
        }
    });
}
function ChangeDialogButtonText(ctrl_name, new_ctrl_name) {
    if (ctrl_name != null && new_ctrl_name != null) {
        if (ctrl_name.length == new_ctrl_name.length) {
            var ctrl = $("div[class=popup-dialog]").parent();
            ctrl.parent().children(":nth-child(3)").children().children("button").each(function () {
                for (var idx = 0; idx < ctrl_name.length; idx++) {
                    if ($(this).text() == ctrl_name[idx]) {
                        $(this).text(new_ctrl_name[idx])
                    }
                }
            });
        }
    }
}
function CallBackCallerFunction(name, result) {
    var callerScreenID = "";
    var info = ajax_method.GetScreenInformation(null);
    if (info.length == 2) {
        callerScreenID = info[1];
    }

    var screenID = "";
    if (ajax_method.PopupKeys != undefined) {
        if (ajax_method.PopupKeys.length > 0) {
            var screenID = "";
            var key = "";
            if (ajax_method.PopupKeys.length > 0) {
                screenID = ajax_method.PopupKeys[ajax_method.PopupKeys.length - 1];
                var spp = screenID.split(";");
                if (spp.length >= 3) {
                    screenID = spp[1];
                    key = spp[2];
                }
            }
        }
    }

    if (callerScreenID == "" && screenID == "") {
        screenID = ajax_method.GetScreenInformation()[1];
    }
    if (screenID != "") {
        var func_name = screenID + "_" + name;
        if (typeof (window[func_name]) == "function") {
            return window[func_name](result);
        }
    }
    if (callerScreenID != "") {
        var func_name = callerScreenID + "_" + name;
        if (typeof (window[func_name]) == "function") {
            return window[func_name](result);
        }
    }

    if (typeof (window[name]) == "function") {
        return window[name](result);
    }
    
    return null;
}

/* ----------------------------------------------------- */

/* --- Set Button Style -------------------------------- */
/* ----------------------------------------------------- */
$.fn.SetDialogStyle = function () {
    $(this).parent().css({
        "padding": "0"
    });
    $(this).parent().children(":nth-child(1)").css({
        "border": "none",
        "-moz-border-radius": "0",
        "-webkit-border-radius": "0",
        "border-radius": "0"
    });
    $(this).children("p").css({
        "margin-top":"3px"
    });
    $(this).parent().children(":nth-child(3)").children().children("button").children("span").css({
        "color": "#000000",
        "padding": "0"
    });
}
/* ----------------------------------------------------- */

/* --- Contract Menu Dialog ---------------------------- */
/* ----------------------------------------------------- */
function OpenContractMenuDialog(url, type) {
    /// <summary>Method to open Contract sub menu</summary>
    /// <param name="url" type="string">Controller Path</param>
    var ctrl = $("#subContractMenu");

    call_ajax_method_json(url, { type: type }, function (result) {
        if (result == undefined)
            return;
        ctrl.html(result);

        ctrl.dialog({
            modal: true,
            resizable: false,
            draggable: false,
            position: [0, 75],
            width: "100%"
        });

        var ctrlP1 = ctrl.parent();
        var ctrlP2 = ctrlP1.parent();

        ctrlP2.find("div[class=ui-widget-overlay]:first").attr("class", "contract-menu-overlay");

        ctrlP1.css({
            "background-color": "#98DCD5"
        });
        ctrlP1.children().each(function (index) {
            if (index == 0) {
                $(this).css({
                    "display": "none"
                });
            }
            else {
                $(this).css({
                    "background-color": "#98DCD5"
                });
            }

        });

        var height = parseInt($("div[class=content]").height());
        ctrl.dialog("option", "height", height + 35);

        if ($("#imgClose").length > 0) {
            $("#imgClose").click(function () {
                ctrl.CloseDialog();
            });
        }
        ctrl.find("a").each(function () {
            $(this).click(function () {
                var url = $(this).attr("href");
                if (url != undefined) {
                    var link = "/" + url.substring(4);
                    var sp = link.split("/");
                    var subobject = sp[sp.length - 1];

                    link = link.substring(0, link.length - 2);
                    ajax_method.CallScreenControllerWithAuthority(link, "", false, subobject);
                }

                return false;
            });
        });
    });
}
/* ----------------------------------------------------- */


function OpenSearchBarDialog() {
    $("#divSearchBar").dialog({
        modal: false,
        resizable: false,
        draggable: false,
        position: [0, 75],
        width: "185px"
    });

    var ctrlP1 = $("#divSearchBar").parent();
    var ctrlP2 = ctrlP1.parent();

    ctrlP1.css({
        "background-color": "#F3EAF3",
        "border-top": "none",
        "border-left": "none",
        "border-bottom": "none"
    });
    ctrlP1.children().each(function (index) {
        if (index == 0) {
            $(this).css({
                "display": "none"
            });
        }
        else {
            $(this).css({
                "background-color": "#F3EAF3"
            });
        }

    });

    var height = parseInt($("div[class=content]").height());
    $("#divSearchBar").dialog("option", "height", height - 6);
}

$.fn.OpenPopupDialog2 = function (width, height, event, hide_ctrl, open_event, close_event, disable_ctrl) {
    /// <summary>Open on-page DIV as pop-up dialog</summary>
    /// <param name="width" type="int">Width of dialog</param>
    /// <param name="height" type="int">Height of dialog</param>
    /// <param name="event" type="object">Dialog's command button and event. eg: OK, Close</param>
    /// <param name="hide_ctrl" type="array">Array of controls used to hide</param>
    /// <param name="open_event" type="function">On dialog opening event</param>
    /// <param name="close_event" type="function">On dialog closed event</param>
    /// <param name="disable_ctrl" type="array">Array of controls used to disabled</param>
    
    if (!event) {
        event = {};
    }

    if (!event.Close) {
        event.Close = function () {
            $(this).CloseDialog();
        };
    }

    var ctrl = $(this);

    master_event.LockWindow(true);

    ctrl.children("div[class=popup-dialog]").css({
        "width": width - 24 + "px",
        "height": height - 96 + "px"
    });

    // --- Show Dialog
    ctrl.dialog({
        modal: true,
        resizable: false,
        width: width,
        height: height,
        //position: ["center","top"],
        buttons: event,
        open: function(event, ui) {
            if (open_event && jQuery.isFunction(open_event)) {
                open_event(event, ui, function () {
                    master_event.LockWindow(false);
                });
            }
            else {
                master_event.LockWindow(false);
            }
        },
        close: function (event, ui) {
            if (typeof (close_event) == "function")
                close_event();
        }
    });

    // --- Update Style
    ctrl.parent().children(":nth-child(1)").css({
        "display": "none"
    });
    ctrl.parent().children(":nth-child(1)").children("a").css("display", "none");
    ctrl.SetDialogStyle();

    if (hide_ctrl != null) {
        for (var idx = 0; idx < hide_ctrl.length; idx++) {
            ctrl.parent().children(":nth-child(3)").children().children("button").each(function () {
                if ($(this).text() == hide_ctrl[idx]) {
                    $(this).hide();
                }
            });
        }
    }

    if (disable_ctrl == true) {
        ctrl.parent().children(":nth-child(3)").children().children("button").each(function () {
            $(this).attr("disabled", true);
        });
    }

    /* --- Merge --- */
    var div = "<div id='divloadingDlg' style='display:none;float:left;margin:3px 0 0 10px;'><div id='divLoadingDefault'>Loading&nbsp;<img src='../../Content/images/loading.gif' /></div></div>";

    if (actionTotal > 0) {
        div = div + "<div id='divPopuploading" + info[1] + "' style='float:left;margin:3px 0 0 10px;'><div>Initialize popup&nbsp;<span id='spanPopupPersent" + info[1] + "'>[0%]</span>&nbsp;<img src='../../Content/images/loading.gif' /></div></div>";
    }
    ctrl.parent().children(":nth-child(3)").append(div);

    $("#divLoadingDefault").show();
    $("#divLoadStatus").hide();
    $("#divloading").hide();

    $(document).find("div[id^=divPopuploading], div[id^=divloadingDlg]").each(function () {
        var id = $(this).attr("id").replace("divPopuploading", "").replace("divloadingDlg", "");
    });
    /* ------------- */
}

