$(window).bind("beforeunload", master_event.UnloadScreen);
$(window).bind("resize", master_event.ResizeWindows);

$(function () {
    //======Temporary code for freeze menu and bottom bar =======//
    $("div[class=content]").css("overflow-y", "scroll");
    $(window).resize();
    //=========================//
});
jQuery(function () {
    jQuery('ul.sf-menu').superfish();
});


$(document).ready(function () {
    /* --- Merge --- */
    //Check session is expired?
    var info = ajax_method.GetScreenInformation();
    if (info.length >= 2) {
        if (info[1] != "CMS010"
            && info[1] != "CMS020") {
            ajax_method.CallScreenController("/Common/CheckSession", "", function (result) {
                if (result == true) {
                    master_event.LockWindow(false);
                }
                else {
                    window.location.href = master_event.GenerateURL("/common/CMS020");
                }
            });
        }
        else {
            master_event.LockWindow(false);
        }
    }
    else {
        master_event.LockWindow(false);
    }
    /* ------------- */

    master_event.InitialTabEvent();
    master_event.LoadingEffect();
    master_event.InitialScrollTop();
    master_event.InitialMenu();

    $("#LC_ChangeLang").click(function () {
        master_event.ChangeLanguage($(this).attr("id"), C_LOCAL_LANGUAGE, $("#DEF_LANG_LC").val());
        return false;
    });
    $("#EN_ChangeLang").click(function () {
        master_event.ChangeLanguage($(this).attr("id"), C_ENGLISH_LANGUAGE, $("#DEF_LANG_EN").val());
        return false;
    });
    $("#JP_ChangeLang").click(function () {
        master_event.ChangeLanguage($(this).attr("id"), C_JAPANESE_LANGUAGE, $("#DEF_LANG_JP").val());
        return false;
    });
    $("#lnkLogOut").click(function () {
        call_ajax_method_json("/common/Logout", "", function () {
            window.location.href = master_event.GenerateURL("/common/CMS010");
        });

        return false;
    });

    //Add by Jutarat A. on 16072012
    //For fix bug when press Esc button
    $("form").each(function (f) {
        this.onreset = function () { return false; }
    });
    //End Add
});



/* ---------------------------------------------------------------------------------------- */

/* --- AJAX Send Methods ------------------------------------------------------------------ */
/* ---------------------------------------------------------------------------------------- */
function call_ajax_method(url, obj, func) {
    /// <summary>Method to call back to server by ajax</summary>
    /// <param name="url" type="string">Controller Path</param>
    /// <param name="obj" type="string">Input Parameters</param>
    /// <param name="func" type="function">Function when can get result from server | function(result) or function(result, controls)</param>
    if (url.indexOf("k=") < 0) {
        var key = ajax_method.GetKeyURL();
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

    $.ajax({
        type: "POST",
        url: ajax_method.GenerateURL(url),
        data: obj,
        success: function (result) {
            ajax_method.OnAjaxSendSuccess(result, func);
        },
        error: ajax_method.OnAjaxSendError
    });
}
function call_ajax_method_json(url, obj, func) {
    if (url.indexOf("k=") < 0) {
        var key = ajax_method.GetKeyURL();
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
            ajax_method.OnAjaxSendSuccess(result, func);
        },
        error: function (result) {
            if (result.status == 200)
                func(result.responseText);
            else if (result.status == 0) {
            }
            else
                ajax_method.OnAjaxSendError(result);
        }
    });
}

/* ---------------------------------------------------------------------------------------- */


function generate_url(path) {
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
    if (url == "")
        url = "/us";

    return url + path;
}


function search_array_index(arrOfOject, key, searchValue) {
    /// <summary>Method to searh index in arry of object ; Return int index ; Remark -1 : search not found , -2 : invalid key column </summary>
    /// <param name="arrOfOject" type="array">Array of object</param>
    /// <param name="key" type="string">Key column in object</param>
    /// <param name="searchValue" type="string">Search value</param>


    for (var i = 0; i < arrOfOject.length; i++) {
        if (arrOfOject[i][key] == null) {
            return -2;
        }
        if ($.trim(arrOfOject[i][key].toString()) == $.trim(searchValue.toString())) {
            return i;
        }
    }

    return -1;

}

function htmlDecode(text) {
    /// <summary>Method to convert special charecter to normal character ex. &amp; --> "&"</summary>
    /// <param name="text" type="string">input text </param>

    var newTxt = text.replace(new RegExp(" ", "gi"), "&nbsp;");

    var div = $('<div></div>').hide().text(newTxt);
    var html = div.html(div.text());

    var decode_text = html.text();

    return decode_text;
}

$.fn.bindJSON = function (json) {

    // iterate each matching form
    return this.each(function () {
        // iterate the elements within the form
        if (json != null) {
            $(':input', this).each(function () {
                var type = this.type, tag = this.tagName.toLowerCase();
                if (type == 'text' || type == 'password' || tag == 'textarea' || tag == 'select')
                    if (json[this.name] != undefined && json[this.name] != null) {
                        this.value = json[this.name];
                        if (this.className == "numeric-box")
                            $(this).setComma();
                    }
            });
        }
    });
};
$.fn.bindJSON_Prefix = function (prefix, json) {
    // iterate each matching form
    return this.each(function () {
        if (json != null) {
            // iterate the elements within the form
            $(':input', this).each(function () {
                var type = this.type, tag = this.tagName.toLowerCase();
                if (type == 'text' || type == 'password' || tag == 'textarea' || tag == 'select') {
                    var name = this.name;
                    var start = this.name.indexOf(prefix, 0);
                    if (start == 0) {
                        name = name.substring(prefix.length);
                    //}

                        if (json[name] != undefined)
                            this.value = json[name];
                        else if (json[this.name] != undefined)
                            this.value = json[this.name];
                        else
                            this.value = "";

                    } //Move by Jutarat A. on 18122013
                }
            });
        }
    });
};

$.fn.bindJSON_ViewMode = function (json, prefix) {
    return this.each(function () {
        // iterate the elements within the form
        if (json != null) {
            $('div', this).each(function () {
                var name = this.id;
                if (prefix != undefined) {
                    var start = this.id.indexOf(prefix, 0);
                    if (start == 0) {
                        name = name.substring(prefix.length);
                    }
                }
                if (this.id != undefined && this.id != null && json[name] != undefined && json[name] != null) {
                    $("#" + this.id).html(json[name]);
                }
            });
        }
    });
};

$.fn.clearForm = function () {
    // iterate each matching form
    return this.each(function () {
        // iterate the elements within the form
        $(':input', this).each(function () {
            var type = this.type, tag = this.tagName.toLowerCase();
            if (type == 'text' || type == 'password' || tag == 'textarea')
                this.value = '';
            else if (type == 'checkbox' || type == 'radio')
                this.checked = false;
            else if (tag == 'select')
                this.selectedIndex = 1; //for default Dropdown to ---select----

            $(this).removeClass("highlight");
        });
    });
};
$.fn.clearForm = function () {
    return this.each(function () {
        var type = this.type, tag = this.tagName.toLowerCase();
        if (tag == 'form' || tag == 'div')
            return $(':input', this).clearForm();
        if (type == 'text' || type == 'password' || tag == 'textarea')
            this.value = '';
        else if (type == 'checkbox' || type == 'radio')
            this.checked = false;
        else if (tag == 'select') {
            // Akat K. 2011-07-19 : first option is index 0
            this.selectedIndex = 0;
            //this.selectedIndex = -1;
        }

        $(this).removeClass("highlight");
    });
};




$.fn.initial_link = function (func) {
    $(this).click(function () {
        if (func != null)
            func($(this).html());
        return false;
    });
}

/* --- Command Button ------------------------------------------------------- */
/* -------------------------------------------------------------------------- */
$.fn.SetCommand = function (show, func) {
    $(this).unbind("click");
    if (show) {
        $(this).show();
        $(this).removeAttr("disabled");

        if (func != null) {
//            $(this).click(function () {
//                func();
//            });

            var ctrl = $(this);
            var fnOnClick = function () {
                setTimeout(function () {
                    func();
                    ctrl.one("click", fnOnClick);
                }, 1);
            };

            ctrl.one("click", fnOnClick);
        }
    }
    else {
        $(this).hide();
    }
}

/* --- Command Button [Register] -------------------------------------------- */
/* -------------------------------------------------------------------------- */
function SetRegisterCommand(show, func) {
    $("#btnCommandRegister").SetCommand(show, func);
}
function DisableRegisterCommand(disabled) {
    $("#btnCommandRegister").attr("disabled", disabled);
}
/* -------------------------------------------------------------------------- */

/* --- Command Button [RequestApprove] -------------------------------------------- */
/* -------------------------------------------------------------------------- */
function SetRequestApproveCommand(show, func) {
    $("#btnCommandRequestApprove").SetCommand(show, func);
}
function DisableRequestApproveCommand(disabled) {
    $("#btnCommandRequestApprove").attr("disabled", disabled);
}
/* -------------------------------------------------------------------------- */

/* --- Command Button [Reset] ----------------------------------------------- */
/* -------------------------------------------------------------------------- */
function SetResetCommand(show, func) {
    $("#btnCommandReset").SetCommand(show, func);
}
function DisableResetCommand(disabled) {
    $("#btnCommandReset").attr("disabled", disabled);
}
/* -------------------------------------------------------------------------- */
function SetLastCompleteCommand(show, func) {
    $("#btnCommandLastComplete").SetCommand(show, func);
}
//function SetCancelPcodeCommand(disabled) {
//    $("#btnCommandCancelPcode").attr("disabled", disabled)
//}
//======= Teerapong 20/07/2012 =====
function SetCancelPcodeCommand(show, func) {
    $("#btnCommandCancelPcode").SetCommand(show, func);
}
function DisableCancelPcodeCommand(disabled) {
    $("#btnCommandCancelPcode").attr("disabled", disabled);
}

/* --- Command Button [Approve] --------------------------------------------- */
/* -------------------------------------------------------------------------- */
function SetApproveCommand(show, func) {
    $("#btnCommandApprove").SetCommand(show, func);
}
/* -------------------------------------------------------------------------- */

/* --- Command Button [Reject] --------------------------------------------- */
/* -------------------------------------------------------------------------- */
function SetRejectCommand(show, func) {
    $("#btnCommandReject").SetCommand(show, func);
}
/* -------------------------------------------------------------------------- */

/* --- Command Button [Confirm] --------------------------------------------- */
/* -------------------------------------------------------------------------- */
function SetConfirmCommand(show, func) {
    $("#btnCommandConfirm").SetCommand(show, func);
}
function DisableConfirmCommand(disabled) {
    $("#btnCommandConfirm").attr("disabled", disabled);
}
/* -------------------------------------------------------------------------- */

/* --- Command Button [Back] ------------------------------------------------ */
/* -------------------------------------------------------------------------- */
function SetBackCommand(show, func) {
    $("#btnCommandBack").SetCommand(show, func);
}
function DisableBackCommand(disabled) {
    $("#btnCommandBack").attr("disabled", disabled);
}
/* -------------------------------------------------------------------------- */

/* --- Command Button [Clear] ----------------------------------------------- */
/* -------------------------------------------------------------------------- */
function SetClearCommand(show, func) {
    $("#btnCommandClear").SetCommand(show, func);
}
function DisableClearCommand(disabled) {
    $("#btnCommandClear").attr("disabled", disabled)
}
/* -------------------------------------------------------------------------- */

/* --- Command Button [OK] -------------------------------------------------- */
/* -------------------------------------------------------------------------- */
function SetOKCommand(show, func) {
    $("#btnCommandOK").SetCommand(show, func);
}
function DisableOKCommand(disabled) {
    $("#btnCommandOK").attr("disabled", disabled)
}
/* -------------------------------------------------------------------------- */

/* --- Command Button [Cancel] ---------------------------------------------- */
/* -------------------------------------------------------------------------- */
function SetCancelCommand(show, func) {
    $("#btnCommandCancel").SetCommand(show, func);
}
function DisableCancelCommand(disabled) {
    $("#btnCommandCancel").attr("disabled", disabled)
}
/* -------------------------------------------------------------------------- */

/* --- Command Button [Purge] ----------------------------------------------- */
/* -------------------------------------------------------------------------- */
function SetPurgeCommand(show, func) {
    $("#btnCommandPurge").SetCommand(show, func);
}
function DisablePurgeCommand(disabled) {
    $("#btnCommandPurge").attr("disabled", disabled);
}
/* -------------------------------------------------------------------------- */

/* --- Command Button [Edit] ------------------------------------------------ */
/* -------------------------------------------------------------------------- */
function SetEditCommand(show, func) {
    $("#btnCommandEdit").SetCommand(show, func);
}
/* -------------------------------------------------------------------------- */

/* --- Command Button [Return] ---------------------------------------------- */
/* -------------------------------------------------------------------------- */
function SetReturnCommand(show, func) {
    $("#btnCommandReturn").SetCommand(show, func);
}
/* -------------------------------------------------------------------------- */

/* --- Command Button [Close] ----------------------------------------------- */
/* -------------------------------------------------------------------------- */
function SetCloseCommand(show, func) {
    $("#btnCommandClose").SetCommand(show, func);
}
/* -------------------------------------------------------------------------- */
/* --- Command Button [Close] ----------------------------------------------- */
/* -------------------------------------------------------------------------- */
function SetConfirmCancelCommand(show, func) {
    $("#btnCommandConfirmCancel").SetCommand(show, func);
}
/* -------------------------------------------------------------------------- */

/* -------------------------------------------------------------------------- */
/* -------------------------------------------------------------------------- */





function CreateObjectData(txt, isTrim) {

    if (isTrim == undefined) {
        isTrim = false; // Optional for trim text
    }

    var obj = new Object();
    var xs = txt.split("&");
    for (var i = 0; i < xs.length; i++) {
        var vx = xs[i].split("=");

        if (vx.length >= 2) {
            var val = "";
            if ($("#" + vx[0]).length > 0) {
                var isNumeric = false;
                var cClass = $("#" + vx[0]).attr("class");
                if (cClass != undefined)
                    isNumeric = (cClass.indexOf("numeric-box") >= 0);
                if (isNumeric == true)
                    val = $("#" + vx[0]).NumericValue();
                else {
                    var txtVal = vx[1];
                    var newTxt = txtVal.replace(new RegExp("\\+", "gi"), " ");     // Form serialize the space (" ") charecter is replaced by "+"
                   
                    if (isTrim) // Optional for trim text
                        newTxt = $.trim(newTxt);

                    val = decodeURIComponent(newTxt);  //old. $("#" + vx[0]).val();   // ex. decodeURIComponent("%E0%B8%9F%E0%B8%AB%E0%B8%81%E0%B8%94")

                    // Comment because jQuery 1.7 is fixed this case alrady !!!
                    //if (val.indexOf("??") != -1)
                    //    val = val.replace(new RegExp("\\?", "gi"), "\\?"); // For avoid "??" jQuery error. ( replace "??" -> "\\?\\?" )
                }


            }
            else
                val = vx[1];

            obj[vx[0]] = val;


        }
    }
    return obj;
}
///
//function ChangeLanguage(elementID, txtLang) {
//    //    alert(element);
//    var obj = {
//        module: "Common",
//        code: "MSG0103",
//        param: txtLang //Language name to change
//    };
//    call_ajax_method("/Shared/GetMessage", obj, function (res) {
//        OpenOkCancelDialog(res.Code, res.Message, function () {
//            call_ajax_method("/common/changeLanguageDsTransData", "lang=" + txtLang, function () {
//                isChangeLanguageEvent = true;
//                window.location.href = $("#" + elementID).parent().attr("href");
//            });
//        }, null);
//    });


//}

/* --- Method to Convert Date format that return from JSON (format /Date(xxxxx)/) --- */
function ConvertDateObject(obj) {
    if (obj === null) return null; // add by jirawat jannet on 2016-11-24
    try {
        var vDate = obj;
        vDate = parseInt(vDate.replace(/\/+Date\(([-]*[\d+]+)\)\/+/, '$1'));
        if (isNaN(vDate) == false) {
            var date = new Date(vDate);
            return date.toDateString();
        }
        return null;
    }
    catch (e) {
    }

    return obj;
}


//function InitialTabEvent() {
//    $(document).keydown(function (e) {
//        if (e.which == 9) {
//            var ctrl = $(document).find(":focus");
//            var fields = $(document).find("input, button, select, textarea");
//            var idx = fields.index(ctrl);

//            var cidx = idx;
//            if (e.originalEvent.shiftKey)
//                cidx = cidx - 1;
//            else
//                cidx = cidx + 1;

//            while (true) {
//                if (cidx < 0)
//                    break;
//                else if (cidx >= fields.length) {
//                    if (idx < 0)
//                        break;
//                    cidx = 0;
//                }

//                var f = fields.eq(cidx);
//                if (f == undefined) {
//                    if (e.originalEvent.shiftKey)
//                        cidx = cidx - 1;
//                    else
//                        cidx = cidx + 1;
//                    continue;
//                }

//                var isvisible = true;
//                if (f[0].offsetLeft != undefined
//                    && f[0].offsetTop != undefined) {
//                    if (f[0].offsetLeft == 0
//                        && f[0].offsetTop == 0) {
//                        isvisible = false;
//                    }
//                }

//                if (($(f).attr("readonly") == false
//                      || $(f).attr("readonly") == undefined)
//                    && $(f).css("display") != "none"
//                    && ($(f).attr("disabled") == false
//                        || $(f).attr("disabled") == undefined)
//                    && f[0].type != "hidden"
//                    && isvisible == true) {
//                    f.focus();
//                    break;
//                }
//                else if (e.originalEvent.shiftKey)
//                    cidx = cidx - 1;
//                else
//                    cidx = cidx + 1;
//            }

//            tabFocusCtrl = null;
//            return false;
//        }
//    });
//}

/* --- Method to create html by call view in controller ------------------------ */
/* ----------------------------------------------------------------------------- */
var callerQueiue = null;
function SetCallerQueiue(list) {
    callerQueiue = list;
}
function RemoveCallerQueiue(ctrl) {
    if (callerQueiue != undefined) {
        var sidx = -1;
        for (var idx = 0; idx < callerQueiue.length; idx++) {
            if (callerQueiue[idx] == ctrl) {
                sidx = idx;
                break;
            }
        }

        if (sidx >= 0)
            callerQueiue.splice(sidx, 1);

        return callerQueiue.length;
    }

    return -1;
}
function CallHtmlSection(ctrl, controller, action, obj, clear, func) {
    /// <summary>Method to create html by call view in controller</summary>
    /// <param name="ctrl" type="string">Control ID</param>
    /// <param name="controller" type="string">Controller Name</param>
    /// <param name="action" type="string">Action Method</param>
    /// <param name="obj" type="object">Object to sent to action</param>
    /// <param name="clear" type="bool">Flag to clear section</param>
    if (clear) {
        //$(ctrl).html("");
        $(ctrl).children("div").each(function (idx) {
            if (idx == 0) {
                $(this).hide();
            }
            else {
                $(this).html("");
            }
        });
    }
    else {
        var ctrl0 = null;
        $(ctrl).children("div").each(function (idx) {
            if (idx == 0) {
                ctrl0 = $(this);
                ctrl0.show();
            }
            else {
                var ictrl = $(this);
                call_ajax_method_json("/" + controller + "/" + action, obj,
                    function (result) {
                        ctrl0.hide();
                        ictrl.html(result);

                        if (RemoveCallerQueiue($(ctrl).attr("id")) == 0) {
                            if (typeof (func) == "function") {
                                func();
                            }
                        }
                    });
            }
        });
    }
}
/* ----------------------------------------------------------------------------- */


function GetEmployeeNameData(src, dest) {
    if (src != undefined && dest != undefined) {
        $(src).blur(function () {
            if (dest != undefined) {
                $(dest).val("");
                if ($.trim($(src).val()) != "") {
                    /* --- Set Parameter --- */
                    var obj = {
                        empNo: $(src).val()
                    };

                    call_ajax_method_json("/Master/GetActiveEmployeeName", obj, function (result) {
                        if (result != undefined) {
                            $(dest).val(result);
                        }
                    });
                }
            }
        });
    }
}


(function ($) {

    $.fn.serializeArray2 = function (isTrim) {
        var rselectTextarea = /^(?:select|textarea)/i;
        var rinput = /^(?:color|date|datetime|datetime-local|email|hidden|month|number|password|range|search|tel|text|time|url|week)$/i;
        var rCRLF = /\r?\n/g;

        return this.map(function () {
            return this.elements ? jQuery.makeArray(this.elements) : this;
        })
		    .filter(function () {
		        return this.name && !this.disabled &&
				    (this.checked || rselectTextarea.test(this.nodeName) ||
					    rinput.test(this.type));
		    })
		    .map(function (i, elem) {
		        var val = jQuery(this).val();
                if(isTrim) {
                    val = $.trim(val);
                }

		        /// For custom value
		        if (jQuery(this).hasClass("numeric-box")) {
		            val = jQuery(this).NumericValue();
		        }

		        return val == null ?
				    null :
				    jQuery.isArray(val) ?
					    jQuery.map(val, function (val, i) {
					        return { name: elem.name, value: val.replace(rCRLF, "\r\n") };
					    }) :
					    { name: elem.name, value: val.replace(rCRLF, "\r\n") };
		    }).get();
    };

    $.fn.serializeObject2 = function (isTrim) {
        var arrayData, objectData;
        arrayData = this.serializeArray2(isTrim);
        objectData = {};

        $.each(arrayData, function () {
            var value;

            if (this.value != null) {
                value = this.value;
            } else {
                value = '';
            }

            if (objectData[this.name] != null) {
                if (!objectData[this.name].push) {
                    objectData[this.name] = [objectData[this.name]];
                }

                objectData[this.name].push(value);
            } else {
                objectData[this.name] = value;
            }
        });

        return objectData;
    };

})(jQuery);


(function(){

    /**
     * Decimal adjustment of a number.
     *
     * @param   {String}    type    The type of adjustment.
     * @param   {Number}    value   The number.
     * @param   {Integer}   exp     The exponent (the 10 logarithm of the adjustment base).
     * @returns {Number}            The adjusted value.
     */
    function decimalAdjust(type, value, exp) {
        // If the exp is undefined or zero...
        if (typeof exp === 'undefined' || +exp === 0) {
            return Math[type](value);
        }
        value = +value;
        exp = +exp;
        // If the value is not a number or the exp is not an integer...
        if (isNaN(value) || !(typeof exp === 'number' && exp % 1 === 0)) {
            return NaN;
        }
        // Shift
        value = value.toString().split('e');
        value = Math[type](+(value[0] + 'e' + (value[1] ? (+value[1] - exp) : -exp)));
        // Shift back
        value = value.toString().split('e');
        return +(value[0] + 'e' + (value[1] ? (+value[1] + exp) : exp));
    }

    // Decimal round
    if (!Math.round10) {
        Math.round10 = function(value, exp) {
            return decimalAdjust('round', value, exp);
        };
    }
    // Decimal floor
    if (!Math.floor10) {
        Math.floor10 = function(value, exp) {
            return decimalAdjust('floor', value, exp);
        };
    }
    // Decimal ceil
    if (!Math.ceil10) {
        Math.ceil10 = function(value, exp) {
            return decimalAdjust('ceil', value, exp);
        };
    }

})();

// Add by Jirawat Jannet
/*!
 * accounting.js v0.4.2
 * Copyright 2014 Open Exchange Rates
 *
 * Freely distributable under the MIT license.
 * Portions of accounting.js are inspired or borrowed from underscore.js
 *
 * Full details and documentation:
 * http://openexchangerates.github.io/accounting.js/
 */

(function (root, undefined) {

    /* --- Setup --- */

    // Create the local library object, to be exported or referenced globally later
    var lib = {};

    // Current version
    lib.version = '0.4.1';


    /* --- Exposed settings --- */

    // The library's settings configuration object. Contains default parameters for
    // currency and number formatting
    lib.settings = {
        currency: {
            symbol: "$",		// default currency symbol is '$'
            format: "%s%v",	// controls output: %s = symbol, %v = value (can be object, see docs)
            decimal: ".",		// decimal point separator
            thousand: ",",		// thousands separator
            precision: 2,		// decimal places
            grouping: 3		// digit grouping (not implemented yet)
        },
        number: {
            precision: 0,		// default precision on numbers is 0
            grouping: 3,		// digit grouping (not implemented yet)
            thousand: ",",
            decimal: "."
        }
    };


    /* --- Internal Helper Methods --- */

    // Store reference to possibly-available ECMAScript 5 methods for later
    var nativeMap = Array.prototype.map,
		nativeIsArray = Array.isArray,
		toString = Object.prototype.toString;

    /**
	 * Tests whether supplied parameter is a string
	 * from underscore.js
	 */
    function isString(obj) {
        return !!(obj === '' || (obj && obj.charCodeAt && obj.substr));
    }

    /**
	 * Tests whether supplied parameter is a string
	 * from underscore.js, delegates to ECMA5's native Array.isArray
	 */
    function isArray(obj) {
        return nativeIsArray ? nativeIsArray(obj) : toString.call(obj) === '[object Array]';
    }

    /**
	 * Tests whether supplied parameter is a true object
	 */
    function isObject(obj) {
        return obj && toString.call(obj) === '[object Object]';
    }

    /**
	 * Extends an object with a defaults object, similar to underscore's _.defaults
	 *
	 * Used for abstracting parameter handling from API methods
	 */
    function defaults(object, defs) {
        var key;
        object = object || {};
        defs = defs || {};
        // Iterate over object non-prototype properties:
        for (key in defs) {
            if (defs.hasOwnProperty(key)) {
                // Replace values with defaults only if undefined (allow empty/zero values):
                if (object[key] == null) object[key] = defs[key];
            }
        }
        return object;
    }

    /**
	 * Implementation of `Array.map()` for iteration loops
	 *
	 * Returns a new Array as a result of calling `iterator` on each array value.
	 * Defers to native Array.map if available
	 */
    function map(obj, iterator, context) {
        var results = [], i, j;

        if (!obj) return results;

        // Use native .map method if it exists:
        if (nativeMap && obj.map === nativeMap) return obj.map(iterator, context);

        // Fallback for native .map:
        for (i = 0, j = obj.length; i < j; i++) {
            results[i] = iterator.call(context, obj[i], i, obj);
        }
        return results;
    }

    /**
	 * Check and normalise the value of precision (must be positive integer)
	 */
    function checkPrecision(val, base) {
        val = Math.round(Math.abs(val));
        return isNaN(val) ? base : val;
    }


    /**
	 * Parses a format string or object and returns format obj for use in rendering
	 *
	 * `format` is either a string with the default (positive) format, or object
	 * containing `pos` (required), `neg` and `zero` values (or a function returning
	 * either a string or object)
	 *
	 * Either string or format.pos must contain "%v" (value) to be valid
	 */
    function checkCurrencyFormat(format) {
        var defaults = lib.settings.currency.format;

        // Allow function as format parameter (should return string or object):
        if (typeof format === "function") format = format();

        // Format can be a string, in which case `value` ("%v") must be present:
        if (isString(format) && format.match("%v")) {

            // Create and return positive, negative and zero formats:
            return {
                pos: format,
                neg: format.replace("-", "").replace("%v", "-%v"),
                zero: format
            };

            // If no format, or object is missing valid positive value, use defaults:
        } else if (!format || !format.pos || !format.pos.match("%v")) {

            // If defaults is a string, casts it to an object for faster checking next time:
            return (!isString(defaults)) ? defaults : lib.settings.currency.format = {
                pos: defaults,
                neg: defaults.replace("%v", "-%v"),
                zero: defaults
            };

        }
        // Otherwise, assume format was fine:
        return format;
    }


    /* --- API Methods --- */

    /**
	 * Takes a string/array of strings, removes all formatting/cruft and returns the raw float value
	 * Alias: `accounting.parse(string)`
	 *
	 * Decimal must be included in the regular expression to match floats (defaults to
	 * accounting.settings.number.decimal), so if the number uses a non-standard decimal 
	 * separator, provide it as the second argument.
	 *
	 * Also matches bracketed negatives (eg. "$ (1.99)" => -1.99)
	 *
	 * Doesn't throw any errors (`NaN`s become 0) but this may change in future
	 */
    var unformat = lib.unformat = lib.parse = function (value, decimal) {
        // Recursively unformat arrays:
        if (isArray(value)) {
            return map(value, function (val) {
                return unformat(val, decimal);
            });
        }

        // Fails silently (need decent errors):
        value = value || 0;

        // Return the value as-is if it's already a number:
        if (typeof value === "number") return value;

        // Default decimal point comes from settings, but could be set to eg. "," in opts:
        decimal = decimal || lib.settings.number.decimal;

        // Build regex to strip out everything except digits, decimal point and minus sign:
        var regex = new RegExp("[^0-9-" + decimal + "]", ["g"]),
			unformatted = parseFloat(
				("" + value)
				.replace(/\((.*)\)/, "-$1") // replace bracketed values with negatives
				.replace(regex, '')         // strip out any cruft
				.replace(decimal, '.')      // make sure decimal point is standard
			);

        // This will fail silently which may cause trouble, let's wait and see:
        return !isNaN(unformatted) ? unformatted : 0;
    };


    /**
	 * Implementation of toFixed() that treats floats more like decimals
	 *
	 * Fixes binary rounding issues (eg. (0.615).toFixed(2) === "0.61") that present
	 * problems for accounting- and finance-related software.
	 */
    var toFixed = lib.toFixed = function (value, precision) {
        precision = checkPrecision(precision, lib.settings.number.precision);
        var power = Math.pow(10, precision);

        // Multiply up by precision, round accurately, then divide and use native toFixed():
        return (Math.round(lib.unformat(value) * power) / power).toFixed(precision);
    };


    /**
	 * Format a number, with comma-separated thousands and custom precision/decimal places
	 * Alias: `accounting.format()`
	 *
	 * Localise by overriding the precision and thousand / decimal separators
	 * 2nd parameter `precision` can be an object matching `settings.number`
	 */
    var formatNumber = lib.formatNumber = lib.format = function (number, precision, thousand, decimal) {
        // Resursively format arrays:
        if (isArray(number)) {
            return map(number, function (val) {
                return formatNumber(val, precision, thousand, decimal);
            });
        }

        // Clean up number:
        number = unformat(number);

        // Build options object from second param (if object) or all params, extending defaults:
        var opts = defaults(
				(isObject(precision) ? precision : {
				    precision: precision,
				    thousand: thousand,
				    decimal: decimal
				}),
				lib.settings.number
			),

			// Clean up precision
			usePrecision = checkPrecision(opts.precision),

			// Do some calc:
			negative = number < 0 ? "-" : "",
			base = parseInt(toFixed(Math.abs(number || 0), usePrecision), 10) + "",
			mod = base.length > 3 ? base.length % 3 : 0;

        // Format the number:
        return negative + (mod ? base.substr(0, mod) + opts.thousand : "") + base.substr(mod).replace(/(\d{3})(?=\d)/g, "$1" + opts.thousand) + (usePrecision ? opts.decimal + toFixed(Math.abs(number), usePrecision).split('.')[1] : "");
    };


    /**
	 * Format a number into currency
	 *
	 * Usage: accounting.formatMoney(number, symbol, precision, thousandsSep, decimalSep, format)
	 * defaults: (0, "$", 2, ",", ".", "%s%v")
	 *
	 * Localise by overriding the symbol, precision, thousand / decimal separators and format
	 * Second param can be an object matching `settings.currency` which is the easiest way.
	 *
	 * To do: tidy up the parameters
	 */
    var formatMoney = lib.formatMoney = function (number, symbol, precision, thousand, decimal, format) {
        // Resursively format arrays:
        if (isArray(number)) {
            return map(number, function (val) {
                return formatMoney(val, symbol, precision, thousand, decimal, format);
            });
        }

        // Clean up number:
        number = unformat(number);

        // Build options object from second param (if object) or all params, extending defaults:
        var opts = defaults(
				(isObject(symbol) ? symbol : {
				    symbol: symbol,
				    precision: precision,
				    thousand: thousand,
				    decimal: decimal,
				    format: format
				}),
				lib.settings.currency
			),

			// Check format (returns object with pos, neg and zero):
			formats = checkCurrencyFormat(opts.format),

			// Choose which format to use for this value:
			useFormat = number > 0 ? formats.pos : number < 0 ? formats.neg : formats.zero;

        // Return with currency symbol added:
        return useFormat.replace('%s', opts.symbol).replace('%v', formatNumber(Math.abs(number), checkPrecision(opts.precision), opts.thousand, opts.decimal));
    };


    /**
	 * Format a list of numbers into an accounting column, padding with whitespace
	 * to line up currency symbols, thousand separators and decimals places
	 *
	 * List should be an array of numbers
	 * Second parameter can be an object containing keys that match the params
	 *
	 * Returns array of accouting-formatted number strings of same length
	 *
	 * NB: `white-space:pre` CSS rule is required on the list container to prevent
	 * browsers from collapsing the whitespace in the output strings.
	 */
    lib.formatColumn = function (list, symbol, precision, thousand, decimal, format) {
        if (!list) return [];

        // Build options object from second param (if object) or all params, extending defaults:
        var opts = defaults(
				(isObject(symbol) ? symbol : {
				    symbol: symbol,
				    precision: precision,
				    thousand: thousand,
				    decimal: decimal,
				    format: format
				}),
				lib.settings.currency
			),

			// Check format (returns object with pos, neg and zero), only need pos for now:
			formats = checkCurrencyFormat(opts.format),

			// Whether to pad at start of string or after currency symbol:
			padAfterSymbol = formats.pos.indexOf("%s") < formats.pos.indexOf("%v") ? true : false,

			// Store value for the length of the longest string in the column:
			maxLength = 0,

			// Format the list according to options, store the length of the longest string:
			formatted = map(list, function (val, i) {
			    if (isArray(val)) {
			        // Recursively format columns if list is a multi-dimensional array:
			        return lib.formatColumn(val, opts);
			    } else {
			        // Clean up the value
			        val = unformat(val);

			        // Choose which format to use for this value (pos, neg or zero):
			        var useFormat = val > 0 ? formats.pos : val < 0 ? formats.neg : formats.zero,

						// Format this value, push into formatted list and save the length:
						fVal = useFormat.replace('%s', opts.symbol).replace('%v', formatNumber(Math.abs(val), checkPrecision(opts.precision), opts.thousand, opts.decimal));

			        if (fVal.length > maxLength) maxLength = fVal.length;
			        return fVal;
			    }
			});

        // Pad each number in the list and send back the column of numbers:
        return map(formatted, function (val, i) {
            // Only if this is a string (not a nested array, which would have already been padded):
            if (isString(val) && val.length < maxLength) {
                // Depending on symbol position, pad after symbol or at index 0:
                return padAfterSymbol ? val.replace(opts.symbol, opts.symbol + (new Array(maxLength - val.length + 1).join(" "))) : (new Array(maxLength - val.length + 1).join(" ")) + val;
            }
            return val;
        });
    };


    /* --- Module Definition --- */

    // Export accounting for CommonJS. If being loaded as an AMD module, define it as such.
    // Otherwise, just add `accounting` to the global object
    if (typeof exports !== 'undefined') {
        if (typeof module !== 'undefined' && module.exports) {
            exports = module.exports = lib;
        }
        exports.accounting = lib;
    } else if (typeof define === 'function' && define.amd) {
        // Return the library as an AMD module:
        define([], function () {
            return lib;
        });
    } else {
        // Use accounting.noConflict to restore `accounting` back to its original value.
        // Returns a reference to the library's `accounting` object;
        // e.g. `var numbers = accounting.noConflict();`
        lib.noConflict = (function (oldAccounting) {
            return function () {
                // Reset the value of the root's `accounting` variable:
                root.accounting = oldAccounting;
                // Delete the noConflict method:
                lib.noConflict = undefined;
                // Return reference to the library to re-assign it:
                return lib;
            };
        })(root.accounting);

        // Declare `fx` on the root (global/window) object:
        root['accounting'] = lib;
    }

    // Root will be `window` in browser or `global` on the server:
}(this));