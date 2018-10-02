var screenGroup;
var screenList;
var allScreenList;
var actionTotal;
var actionCount;

function CallMultiLoadScreen(controller, screens, finish_func) {
    allScreenList = new Array();
    screenGroup = screens;

    actionCount = 0;
    actionTotal = 0;
    for (var i = 0; i < screenGroup.length; i++) {
        actionTotal += screenGroup[i].length;
    }

    var callerScreenID = "";
    var screenID = "";
    var info = ajax_method.GetScreenInformation();
    if (info.length == 2) {
        callerScreenID = info[1];
    }
    if (ajax_method.PopupKeys != undefined) {
        if (ajax_method.PopupKeys.length > 0) {
            screenID = ajax_method.PopupKeys[ajax_method.PopupKeys.length - 1].split(";")[1];
        }
    }

    if ($("#divPopuploading" + screenID).length > 0) {
        $("#divPopuploading" + screenID).show();
        $("#spanPopupPersent" + screenID).html("[0%]");
    }
    else {
        $("#divLoadingDefault").hide();
        $("#divLoadStatus").show();
        $("#divloading").show();
        $("#spanScreenPersent").html("[0%]");
    }

    var func = function (action) {
        if (screenList.length > 0) {
            var idx = -1;
            for (var i = 0; i < screenList.length; i++) {
                if (screenList[i] == action) {
                    idx = i;
                    break;
                }
            }

            if (idx >= 0) {
                allScreenList.push(action);
                screenList.splice(idx, 1);
            }
        }
        if (screenList.length == 0) {
            if (screenGroup.length > 0) {
                screenList = screenGroup.pop();
                for (var i = 0; i < screenList.length; i++) {
                    CallScreenController(controller, screenList[i], "", func);
                }
            }
            else {
                for (var i = 0; i < allScreenList.length; i++) {
                    var div = $("#div" + allScreenList[i]);
                    div.show();

                    var cust_funciton = allScreenList[i] + "_InitialGrid";
                    if (typeof (window[cust_funciton]) == "function") {
                        window[cust_funciton]();
                    }
                }

                var callerScreenID = "";
                var screenID = "";
                var info = ajax_method.GetScreenInformation();
                if (info.length == 2) {
                    callerScreenID = info[1];
                }
                if (ajax_method.PopupKeys != undefined) {
                    if (ajax_method.PopupKeys.length > 0) {
                        screenID = ajax_method.PopupKeys[ajax_method.PopupKeys.length - 1].split(";")[1];
                    }
                }

                if ($("#divPopuploading" + screenID).length > 0) {
                    $("#divPopuploading" + screenID).hide();
                }
                else {
                    $("#divLoadingDefault").show();
                    $("#divLoadStatus").hide();
                }

                if (typeof (finish_func) == "function")
                    finish_func();

                actionCount = 0;
                actionTotal = 0;
            }
        }
    };

    screenList = screenGroup.pop();
    for (var i = 0; i < screenList.length; i++) {
        CallScreenController(controller, screenList[i], "", func);
    }
}

function CallScreenController(controller, action, obj, func) {
    var div = $("#div" + action);
    call_ajax_method_json("/" + controller + "/" + action, obj,
        function (result) {
            actionCount += 1;
            var persent = Math.floor((actionCount * 100) / actionTotal);

            var callerScreenID = "";
            var screenID = "";
            var info = ajax_method.GetScreenInformation();
            if (info.length == 2) {
                callerScreenID = info[1];
            }
            if (ajax_method.PopupKeys != undefined) {
                if (ajax_method.PopupKeys.length > 0) {
                    screenID = ajax_method.PopupKeys[ajax_method.PopupKeys.length - 1].split(";")[1];
                }
            }

            if ($("#divPopuploading" + screenID).length > 0) {
                $("#divPopuploading" + screenID).show();
                $("#spanPopupPersent" + screenID).html("[" + persent + "%]");
            }
            else {
                $("#spanScreenPersent").html("[" + persent + "%]");
            }

            div.html(result);
            func(action);
        }
    );
}

function ResetAllScreen(screens) {
    if (screens != undefined) {
        for (var i = 0; i < screens.length; i++) {
            var div = $("#div" + screens[i]);
            div.html("");
            div.hide();
        }
    }
}


function ChangeAllSectionMode(screens, mode) {
    if (screens != undefined) {
        for (var i = 0; i < screens.length; i++) {
            var cust_function = "Set" + screens[i] + "_SectionMode";
            if (typeof (window[cust_function]) == "function") {
                window[cust_function](mode);
            }
        }
    }
}
function EnableAllScreenSection(screens, enable) {
    if (screens != undefined) {
        for (var i = 0; i < screens.length; i++) {
            var cust_function = "Set" + screens[i] + "_EnableSection";
            if (typeof (window[cust_function]) == "function") {
                window[cust_function](enable);
            }
        }
    }
}