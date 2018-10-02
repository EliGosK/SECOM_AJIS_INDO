/// <reference path="../../jquery-1.5.1-vsdoc.js" />

var master_event = $.extend({
    CMS010_SCREEN: "CMS010",
    CMS020_SCREEN: "CMS020",

    IsChangeLanguageEvent: false,
    IsShowSearchBar: false,

    ResizeWindows: function () {
        //======For freeze menu and bottom bar =======//
        var browserDOM = $(window);
        var browserH = browserDOM.height();

        var headerDOM = $("div[class=pageheader]");
        var headerH = headerDOM.height();

        var actionbuttonsDOM = $("div[class=pagefooter]");
        var actionbuttonsH = actionbuttonsDOM.height();

        var chight = browserH - headerH - actionbuttonsH - 30;
        if (chight < 100) {
            chight = 100;
        }

        $("div[class=content]").height(chight);

        if ($("#subMenu").length > 0)
            $("#subMenu").dialog("option", "height", chight + 35);
    },

    /* --- Merge --- */
    /* LoadScreen: function (url, use_key, new_window) {
    var nurl = ajax_method.GenerateURL(url);
    if (use_key == true)
    nurl = nurl + "?k=" + use_key;

    if (new_window == true) {
    window.open(nurl);
    }
    else {
    window.location = nurl;
    }
    },*/
    LoadScreen: function (url, key, new_window) {
        var nurl = ajax_method.GenerateURL(url);
        if (key != undefined)
            nurl = nurl + "?k=" + key;

        if (new_window == true) {
            window.open(nurl);
        }
        else {
            window.location = nurl;
        }
    },
    /* ------------- */


    UnloadScreen: function () {
        master_event.LockWindow(true);

        if (master_event.IsChangeLanguageEvent)
            return;

        var info = ajax_method.GetScreenInformation(null);
        if (info.length == 2) {
            if (info[1] != master_event.CMS010_SCREEN && info[1] != master_event.CMS020_SCREEN) {
                var clear_session_func = function (module, key) {
                    $.ajax({
                        type: "POST",
                        url: ajax_method.GenerateURL("/" + module + "/ClearCurrentScreenSession?k=" + key),
                        async: false,
                        success: function (result) {
                            //alert("Success Clear session, remain = " + result);
                        }
                    });
                    //alert("Close screen");
                };

                if (ajax_method.PopupKeys != undefined) {
                    while (ajax_method.PopupKeys.length > 0) {
                        var module = "";
                        var key = "";
                        if (ajax_method.PopupKeys.length > 0) {
                            module = ajax_method.PopupKeys[ajax_method.PopupKeys.length - 1];
                            var spp = module.split(";");
                            if (spp.length >= 3) {
                                module = spp[0];
                                key = spp[2];
                            }
                        }

                        if (module != "" && key != "") {
                            clear_session_func(module, key);
                        }
                        ajax_method.PopupKeys.pop();
                    }
                }

                clear_session_func(info[0], ajax_method.GetKeyURL(null));
            }
        }
    },

    InitialTabEvent: function () {
        $(document).keydown(function (e) {
            /* --- Merge --- */
            /* if (e.which == 9) { */
            if (e.which == 8
                || (e.originalEvent.altKey == true && e.which == 37)) {
                var tag = e.target.tagName.toLowerCase();
                var type = e.target.type;
                if (tag != "input" && tag != "textarea") {
                    return false;
                }
                else if (tag == "input" && type != "text" && type != "password") {
                    return false;
                }
                else if (e.target.readOnly == true) {
                    return false;
                }
            }
            else if (e.which == 9) {
                /* ------------- */

                var ctrl = $(document).find(":focus");
                var fields = $(document).find("input, button, select, textarea");
                var idx = fields.index(ctrl);

                /* --- Merge --- */
                var ispopup_ctrl = false;
                var parents = ctrl.parents();
                for (var pi = 0; pi < parents.length; pi++) {
                    if (parents[pi].className == "popup-dialog"
                        || parents[pi].className == "ui-dialog-buttonset") {
                        ispopup_ctrl = true;
                        break;
                    }
                }
                /* ------------- */

                var cidx = idx;
                if (e.originalEvent.shiftKey)
                    cidx = cidx - 1;
                else
                    cidx = cidx + 1;

                while (true) {
                    if (cidx < 0)
                        break;
                    else if (cidx >= fields.length) {
                        if (idx < 0)
                            break;
                        cidx = 0;
                    }

                    var f = fields.eq(cidx);
                    if (f == undefined) {
                        if (e.originalEvent.shiftKey)
                            cidx = cidx - 1;
                        else
                            cidx = cidx + 1;
                        continue;
                    }

                    var isvisible = true;
                    if (f[0].offsetLeft != undefined
                        && f[0].offsetTop != undefined) {
                        if (f[0].offsetLeft == 0
                            && f[0].offsetTop == 0) {
                            isvisible = false;
                        }
                    }

                    if (($(f).prop("readonly") == false
                          || $(f).prop("readonly") == undefined)
                        && $(f).css("display") != "none"
                        && ($(f).prop("disabled") == false
                            || $(f).prop("disabled") == undefined)
                        && f[0].type != "hidden"
                        && isvisible == true) {

                        /* --- Merge --- */
                        /* f.focus();
                        break; */
                        if (ispopup_ctrl == true) {
                            var issamgroup = false;
                            parents = f.parents();
                            for (pi = 0; pi < parents.length; pi++) {
                                if (parents[pi].className == "popup-dialog"
                                    || parents[pi].className == "ui-dialog-buttonset") {
                                    issamgroup = true;
                                    break;
                                }
                            }
                            if (issamgroup) {
                                f.focus();
                                break;
                            }
                            else if (e.originalEvent.shiftKey)
                                cidx = cidx - 1;
                            else
                                cidx = cidx + 1;
                        }
                        else {
                            f.focus();
                            break;
                        }
                        /* ------------- */
                    }
                    else if (e.originalEvent.shiftKey)
                        cidx = cidx - 1;
                    else
                        cidx = cidx + 1;
                }

                tabFocusCtrl = null;
                return false;
            }
        });
    },

    LoadingEffect: function () {
        /* --- Merge --- */
        /* $("#divloading").ajaxStart(function () {
        $(this).show();
        });
        $("#divloading").ajaxStop(function () {
        $(this).hide();
        }); */
        $("#divloading").ajaxStart(function () {
            if ($("#divloadingDlg").length > 0) {
                $("#divloadingDlg").show();
            }
            else {
                $(this).show();
            }
        });
        $("#divloading").ajaxStop(function () {
            $(this).hide();

            if ($("#divloadingDlg").length > 0) {
                $("#divloadingDlg").hide();
            }
        });
        /* ------------- */
    },

    /* --- Merge --- */
    /* LoadingEffectDlg: function () {

    $("#divloadingDlg").ajaxStart(function () {
    $(this).show();
    });
    $("#divloadingDlg").ajaxStop(function () {
    $(this).hide();
    });
    }, */
    /* ------------- */

    ChangeLanguage: function (elementID, txtLang, lang) {
        //        var obj = {
        //            module: "Common",
        //            code: "MSG0103",
        //            param: txtLang //Language name to change
        //        };
        //        call_ajax_method("/Shared/GetMessage", obj, function (res) {
        //            OpenOkCancelDialog(res.Code, res.Message, function () {
        //                call_ajax_method("/common/changeLanguageDsTransData", { lang: lang }, function () {
        //                    master_event.IsChangeLanguageEvent = true;
        //                    window.location = $("#" + elementID).parent().attr("href");
        //                });
        //            }, null);
        //        });


        var obj = {
            lang: lang
        };
        call_ajax_method("/Shared/GetLanguageMessage", obj, function (res) {
            var tmpOK = $("#DialogBtnOK").val();
            var tmpCancel = $("#DialogBtnCancel").val();

            $("#DialogBtnOK").val(res[1][0]);
            $("#DialogBtnCancel").val(res[1][1]);

            OpenOkCancelDialog(res[0].Code, res[0].Message, function () {
                call_ajax_method("/common/changeLanguageDsTransData", { lang: lang }, function () {
                    master_event.IsChangeLanguageEvent = true;
                    window.location = $("#" + elementID).parent().attr("href");
                });

                $("#DialogBtnOK").val(tmpOK);
                $("#DialogBtnCancel").val(tmpCancel);

            }, function () {
                $("#DialogBtnOK").val(tmpOK);
                $("#DialogBtnCancel").val(tmpCancel);
            });
        });
    },

    InitialScrollTop: function () {
        $("#scrollTop").click(function () {
            master_event.ScrollToTopWindow();
        });
    },

    InitialMenu: function () {
        $("div[class=section_menu]:first").find("a").each(function () {
            if ($(this).attr("id") != "btnSearchBar") {
                $(this).click(function () {
                    var url = $(this).attr("href");
                    var target = $(this).attr("target");
                    if (url != undefined) {
                        if (/^https?:\/\//.test(url) || /^file:\/\/\//.test(url)) {
                            window.open(url, target);
                            return false;
                        }

                        var callScreen = true;

                        var sp = url.split("/");
                        var type = parseInt(sp[sp.length - 1]);
                        if (isNaN(type) == false) {
                            if (type > 90) {
                                callScreen = false;

                                type = type - 91;
                                if (type >= 0)
                                    OpenContractMenuDialog("/contract/ContractMenu", type);
                            }
                        }
                        if (callScreen == true) {
                            var link = "/" + url.substring(4);
                            if (isNaN(type) == false) {
                                var len = url.length - (sp[sp.length - 1].length + 1);
                                link = "/" + url.substring(4, len);
                            }


                            if (link.indexOf(master_event.CMS020_SCREEN) >= 0) {
                                return true;
                            }
                            else {
                                ajax_method.CallScreenControllerWithAuthority(link, "", false, type);
                            }
                        }
                    }
                    return false;
                });
            }
        });

        $("#btnSearchBar").click(function () {
            if (master_event.IsShowSearchBar == false) {
                OpenSearchBarDialog();
                master_event.IsShowSearchBar = true;
            }
            else {
                $("#divSearchBar").CloseDialog();
                master_event.IsShowSearchBar = false;
            }

            return false;
        });
    },

    // credit: http://stackoverflow.com/questions/235411/is-there-an-internet-explorer-approved-substitute-for-selectionstart-and-selecti
    GetInputTextSelection: function (el) {
        /// <summary>Get selection start/end of the focusing textarea/input.</summary>
        /// <param name="el" type="object">DOM Element of focusing textarea or input (text).</param>

        var start = 0, end = 0, normalizedValue, range,
            textInputRange, len, endRange;

        if (typeof el.selectionStart == "number" && typeof el.selectionEnd == "number") {
            start = el.selectionStart;
            end = el.selectionEnd;
        } else {
            range = document.selection.createRange();

            if (range && range.parentElement() == el) {
                len = el.value.length;
                normalizedValue = el.value.replace(/\r\n/g, "\n");

                // Create a working TextRange that lives only in the input
                textInputRange = el.createTextRange();
                textInputRange.moveToBookmark(range.getBookmark());

                // Check if the start and end of the selection are at the very end
                // of the input, since moveStart/moveEnd doesn't return what we want
                // in those cases
                endRange = el.createTextRange();
                endRange.collapse(false);

                if (textInputRange.compareEndPoints("StartToEnd", endRange) > -1) {
                    start = end = len;
                } else {
                    start = -textInputRange.moveStart("character", -len);
                    start += normalizedValue.slice(0, start).split("\n").length - 1;

                    if (textInputRange.compareEndPoints("EndToEnd", endRange) > -1) {
                        end = len;
                    } else {
                        end = -textInputRange.moveEnd("character", -len);
                        end += normalizedValue.slice(0, end).split("\n").length - 1;
                    }
                }
            }
        }

        return {
            start: start,
            end: end
        };
    },

    ScrollWindow: function (div, isPopup, notDivCtrl) {
        if (div != undefined) {
            if ($(div).length > 0) {
                var offset = 0;
                if (notDivCtrl == true) {
                    offset = 250;
                }
                else {
                    if (isPopup == true) {
                        offset = 20;
                    }
                    else {
                        offset = -80;
                    }
                }

                if (isPopup == true) {
                    $("div[class=popup-dialog]").animate({ scrollTop: $(div)[0].offsetTop + offset }, "slow");
                }
                else {
                    $("div[class=content]").animate({ scrollTop: $(div)[0].offsetTop + offset }, "slow");
                }
            }
        }
    },
    ScrollToTopWindow: function () {
        master_event.ScrollWindow("#divMainTitleScreen");
    },

    LockWindow: function (lock) {
        if (lock == true) {
            $("#bgNull").show();
        }
        else {
            $("#bgNull").delay(100).fadeOut(500);
        }
    }
});