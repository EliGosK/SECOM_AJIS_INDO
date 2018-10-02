//Add by Jutarat A. on 09012013
var SEARCH_CONDITION = {
    ContractCode: "",
    ProjectCode: ""
};
//End Add

var ajax_method = $.extend({
    PopupKeys: null,

    GetScreenInformation: function (furl) {
        var info = new Array();
        var count = 2;
        var url = furl;
        if (url == undefined) {
            count = 5;
            url = window.location.href;
        }

        var arr = url.split("/");
        if (arr.length >= count) {
            var controlIdx = 2;
            var screenIdx = 1;

            var type = parseInt(arr[arr.length - 1]);
            if (isNaN(type) == false) {
                controlIdx = 3;
                screenIdx = 2;
            }

            info.push(arr[arr.length - controlIdx]);

            var screen = arr[arr.length - screenIdx];
            var pidx = screen.indexOf("?");
            if (pidx > 0)
                screen = screen.substring(0, pidx);
            info.push(screen);
        }

        return info;
    },
    GetKeyURL: function (furl) {
        var key = "";
        var url = furl;
        if (url == undefined) {
            if (ajax_method.PopupKeys != undefined) {
                if (ajax_method.PopupKeys.length > 0)
                    key = ajax_method.PopupKeys[ajax_method.PopupKeys.length - 1].split(";")[2];
            }
            else
                url = window.location.href;
        }
        if (key == "" && url != undefined) {
            var idx = url.indexOf("k=");
            if (idx > 0) {
                key = url.substring(idx + 2, url.length);

                idx = key.indexOf("&");
                if (idx > 0) {
                    key = key.substring(0, idx);
                }
            }
        }

        if (key == "") {
            var url = window.location.href;
            if (furl != undefined)
                url = furl;
        }

        return key;
    },
    GenerateURL: function (path) {
        var url = window.location.href;
        var arr = url.split("/");
        if (arr.length > 0) {
            var count = 0;
            for (var idx = 1; idx <= arr.length; idx++) {
                url = url.substring(0, url.length - arr[arr.length - idx].length - 1);
                if (arr[arr.length - idx] != "") {
                    if (1 == idx) {
                        var type = parseInt(arr[arr.length - idx]);
                        if (isNaN(type) == true)
                            count += 1;
                    }
                    else {
                        count += 1;
                    }

                    if (count == 2)
                        break;
                }
            }
        }
        if (url == "" || url == "http:/")
            url = "/us";

        return url + path;
    },

    ConvertIncorrectText: function (txt) {
        var ntxt = txt;
        if (ntxt != undefined) {
            //Convert &amp; to &
            ntxt = ntxt.replace(/&amp;/g, "&");

            /* --- Merge --- */
            ntxt = ntxt.replace(/&lt;/g, "<");
            ntxt = ntxt.replace(/&gt;/g, ">");
            /* ------------- */
        }

        return ntxt;
    },

    OnAjaxSendSuccess: function (result, func) {
        if (result != undefined) {
            if (result.IsError == true) {
                if (result.Message.MessageType == 0) {
                    // Warning
                    OpenWarningDialog(result.Message.Message);
                    if (typeof (func) == "function")
                        func(result.ResultData, result.Message.Controls, true);
                }
                else if (result.Message.MessageType == 1) {
                    // Message Dialog [Close]
                    OpenErrorMessageDialog(result.Message.Code,
                                            result.Message.Message,
                                            function () {
                                                if (typeof (func) == "function")
                                                    func(result.ResultData, result.Message.Controls);
                                            });
                }
                else if (result.Message.MessageType == 2) {
                    // Message Dialog [OK]
                    OpenInformationMessageDialog(result.Message.Code,
                                                    result.Message.Message,
                                                    function () {
                                                        if (typeof (func) == "function")
                                                            func(result.ResultData, result.Message.Controls);
                                                    });
                }
                else if (result.Message.MessageType == 3) {
                    OpenWarningMessageDialog(result.Message.Code,
                                                result.Message.Message,
                                                function () {
                                                    if (typeof (func) == "function")
                                                        func(result.ResultData, result.Message.Controls);
                                                });
                }
                else if (result.Message.MessageType == 4) {
                    if (result.MessageList != undefined) {
                        var loopWarningFunc = function (idx) {
                            if (idx < result.MessageList.length) {
                                var msg = result.MessageList[idx];
                                OpenYesNoWarningMessageDialog(msg.Code, msg.Message, function () {
                                    loopWarningFunc(idx + 1);
                                    if (result.MessageList.length - 1 == idx && typeof (func) == "function") {
                                        /* --- Merge --- */
                                        command_control.CommandControlMode(true);
                                        /* ------------- */

                                        var midx = idx - 1;
                                        if (midx < 0)
                                            midx = 0;
                                        func(result.ResultData, result.MessageList[midx].Controls);
                                    }
                                },
                                /* --- Merge --- */
                                /* null); */
                                function () {
                                    command_control.CommandControlMode(true);
                                });
                                /* ------------- */
                            }
                        };
                        loopWarningFunc(0);
                    }
                }
                else {
                    // Message Type not found  
                    OpenErrorMessageDialog("MSG0000", "Message type not found.", null);
                }

                /* --- Merge --- */
                if (result.Message.MessageType != 4)
                    command_control.CommandControlMode(true);
                /* ------------- */
                master_event.LockWindow(false);
            }
            else {
                if (result != undefined) {
                    if (result.IsRedirectToLogin == true) {
                        window.location = result.URL;
                        return;
                    }
                }

                command_control.CommandControlMode(true);
                if (typeof (func) == "function") {
                    var objRes = null;
                    if (result.HasResultData == undefined)
                        objRes = result;
                    if (result.HasResultData == true)
                        objRes = result.ResultData;
                    func(objRes);
                }
            }
        }
    },
    OnAjaxSendError: function (result) {
        if (result != undefined) {
            if (result.IsRedirectToLogin == true) {
                window.location = result.URL;
                return;
            }
        }

        //Modify by Jutarat A. on 01032013
        //OpenErrorMessageDialog("MSG0111", "Internal error (" + result.status + "), Please contact system administrator.", null);
        if (result.status >= 12000 && result.status <= 12099) {
            OpenErrorMessageDialog("MSG0111", "Your internet connection has problems.", null);
        }
        else {
            OpenErrorMessageDialog("MSG0111", "Internal error (" + result.status + "), Please contact system administrator.", null);
        }
        //End Modify

        master_event.LockWindow(false);
    },

    CallScreenController: function (url, obj, func, isAsync) {
        if (url.indexOf("k=") < 0) {
            var key = ajax_method.GetKeyURL(null);
            if (key != "") {
                if (url.indexOf("?") > 0) {
                    url = url + "&k=" + key;
                }
                else {
                    url = url + "?k=" + key;
                }
            }
        }
        if (url[url.length - 1] == "#") {
            url = url.substring(0, url.length - 1);
        }
        if (url.indexOf("?") > 0) {
            url = url + "&ajax=1";
        }
        else {
            url = url + "?ajax=1";
        }

        var objJson = $.toJSON(obj);
        objJson = ajax_method.ConvertIncorrectText(objJson);

        $.ajax({
            type: "POST",
            url: ajax_method.GenerateURL(url),
            data: objJson,
            dataType: 'json',
            contentType: 'application/json; charset=UTF-8',
            success: function (result) {
                /* --- Merge --- */
                /* command_control.CommandControlMode(true); */
                /* ------------- */
                ajax_method.OnAjaxSendSuccess(result, func);
            },
            error: function (result) {
                /* --- Merge --- */
                /* command_control.CommandControlMode(true);
                if (result.status == 200)
                func(result.responseText);
                else if (result.status == 0) {
                }
                else
                ajax_method.OnAjaxSendError(result); */

                if (result.status == 200) {
                    command_control.CommandControlMode(true);
                    func(result.responseText);
                }
                else if (result.status == 0) {
                    command_control.CommandControlMode(true);
                }
                else
                    ajax_method.OnAjaxSendError(result);
                /* ------------- */
            },
            async: (isAsync == undefined ? true : isAsync)
        });
    },

    CallScreenControllerWithAuthority: function (url, obj, new_window, subobject) {
        var key = ajax_method.GetKeyURL(null);
        var module = "";
        var screen = "";
        var info = ajax_method.GetScreenInformation(null);
        if (info.length == 2) {
            module = info[0];
            screen = info[1];
        }

        if (typeof (obj) == "object") {
            if (obj == null)
                obj = new Object();

            obj.CallerKey = key;
            obj.CallerModule = module;
            obj.CallerScreenID = screen;
            obj.SubObjectID = subobject;
        }
        else {
            obj = {
                CallerKey: key,
                CallerModule: module,
                CallerScreenID: screen,
                SubObjectID: subobject
            };

            
        }

        if (SEARCH_CONDITION != undefined) {
            obj.CommonSearch = {
                ContractCode: SEARCH_CONDITION.ContractCode,
                ProjectCode: SEARCH_CONDITION.ProjectCode
            };
        }

        var link = url + "_Authority";
        ajax_method.CallScreenController(link, obj, function (result) {
            if (result != undefined) {
                /* --- Merge --- */
                /* master_event.LoadScreen(result, false, new_window); */
                master_event.LoadScreen(result, null, new_window);
                /* ------------- */
            }
        });
    },
    CallPopupScreenControllerWithAuthority: function (caller_id, url, obj, func) {
        var key = ajax_method.GetKeyURL(null);
        var module = "";
        var screen = "";
        var info = ajax_method.GetScreenInformation(null);
        if (info.length == 2) {
            module = info[0];
            screen = info[1];
        }

        if (typeof (obj) == "object") {
            if (obj == null)
                obj = new Object();

            obj.CallerKey = key;
            obj.CallerModule = module;
            obj.CallerScreenID = screen;
            obj.IsPopup = true;
        }
        else {
            obj = {
                CallerKey: key,
                CallerModule: module,
                CallerScreenID: screen,
                IsPopup: true
            };
        }

        var link = url + "_Authority";
        ajax_method.CallScreenController(link, obj, function (result) {
            if (result != undefined) {
                var nkey = ajax_method.GetKeyURL(result);
                var nmodule = "";
                var nscreen = "";
                var ninfo = ajax_method.GetScreenInformation(url);
                if (ninfo.length == 2) {
                    nmodule = ninfo[0];
                    nscreen = ninfo[1];
                }

                if (ajax_method.PopupKeys == undefined) {
                    ajax_method.PopupKeys = new Array();
                }
                ajax_method.PopupKeys.push(nmodule + ";" + nscreen + ";" + nkey);

                if (typeof (func) == "function")
                    func(result);
            }
        });
    }
});