/// <reference path="../../Scripts/Base/object/master_event.js"/>

function VaridateCtrl(ctrl_lst, null_ctrl) {
    if (ctrl_lst != null) {
        for (var idx = 0; idx < ctrl_lst.length; idx++) {
            var ctrl = $("#" + ctrl_lst[idx]);
            if (ctrl.length > 0) {
                ctrl.removeClass("highlight");


                if (ctrl[0].tagName.toLowerCase() == "select") {
                    var unb = function () {
                        $(this).removeClass("highlight");
                        $(this).unbind("change", unb);
                    };
                    ctrl.change(unb);
                }
                else {
                    var unb = function (event) {
                        if (event.keyCode != 9) {
                            $(this).removeClass("highlight");
                            $(this).unbind("keyup", unb);
                        }
                    };
                    ctrl.keyup(unb);
                }
            }
        }
    }
    if (null_ctrl != null) {
        for (var idx = 0; idx < null_ctrl.length; idx++) {
            if (null_ctrl[idx] != "") {
                var ctrl = $("#" + null_ctrl[idx]);
                if (ctrl.length > 0) {
                    ctrl.addClass("highlight");
                }
            }
        }
    }

}
$.fn.moveCursorToEnd = function () {
    $(this).each(function (i) {
        if ($(this).is(':input') && $(this).attr('id')) {
            var inputDOM = document.getElementById($(this).attr('id'));
            var pos = $(this).val().length;

            if (inputDOM.setSelectionRange) {
                //For FF,Chrome, IE9
                inputDOM.focus();
                inputDOM.setSelectionRange(pos, pos);
            } else if (inputDOM.createTextRange) {
                //For IE8
                var range = inputDOM.createTextRange();
                range.collapse(true);
                range.moveEnd('character', pos);
                range.moveStart('character', pos);
                range.select();
            }
        }
    });
}
$.fn.ResetToNormalControl = function () {
    var tag = this[0].tagName.toLowerCase();
    if (tag == "div" || tag == "form") {
        $(this).find("input,select").each(function () {
            $(this).removeClass("highlight");
        });
    }
    else {
        $(this).removeClass("highlight");
    }
}
$.fn.SetNumericControlViewInGrid = function (ctrls) {
    for (var idx = 0; idx < ctrls.length; idx++) {
        $(this).find("div[id^=div" + ctrls[idx] + "]").each(function () {
            $(this).attr("class", $(this).attr("class") + " grid-view-object-unit");
        });
    }
}


/* --- Set View Mode --- */
$.fn.SetViewMode = function (isview, editonly) {
    // Modified by Natthavat S. (02/11/2011)
    // Remark: Modified to support radio button
    $(this).find("input[type=text],input[type=checkbox],input[type=radio],textarea,button,select,span[class=label-remark],span[class*=label-currency],div[class=label-remark],a,div[class=grid-datepicker-fromto]").each(function () {

        if ($(this).hasClass("grid-datepicker-from") || $(this).hasClass("grid-datepicker-from")) {
            return;
        }

        if ($(this).attr("id") == undefined) {
            var isctrl = true;
            var tag = this.tagName.toLowerCase();
            if ((tag == "input") || tag == "a") {
                isctrl = false;
            }

            if (isctrl) {
                if (isview) {
                    $(this).hide();
                }
                else {
                    $(this).show();
                }
            }
        }
        else {

            var div = "div" + $(this).attr("id");
            var tag = this.tagName.toLowerCase();
            var readonly = this.readOnly;

            /* --- Merge --- */
            var disabled = this.disabled;
            /* ------------- */

            if (tag == "span" && $(this).hasClass("label-currency")) {
                $(this).removeClass("label-edit-view");
                $(this).removeClass("label-readonly-view");

                if (isview) {
                    readonly = ($(this).data("readonly") == 1);
                    var css = "label-readonly-view";
                    if (readonly == false || readonly == undefined || editonly == true)
                        css = "label-edit-view";
                  
                    $(this).addClass(css);
                }
            }
            else {
                if (isview) {
                    if (tag == "input" && $(this).attr("type") == "checkbox") {
                        $(this).attr("disabled", true);
                    } else if (tag == "input" && $(this).attr("type") == "radio") {
                        $(this).attr("disabled", true);
                    }
                    else {
                        $(this).hide();
                        var unitDate = $("#unitDate" + $(this).attr("id"));
                        if (unitDate.length > 0) {
                            unitDate.hide();
                        }

                        if (tag == "input" || tag == "select" || tag == "textarea" || tag == "a") {
                            /* --- For Datetime Picker --- */
                            $(this).parent().find("img").hide();

                            var css = "label-readonly-view";
                            if (readonly == false || readonly == undefined || editonly == true)
                                css = "label-edit-view";

                            /* --- Merge --- */
                            if (disabled == true)
                                css = "label-readonly-view";
                            /* ------------- */

                            var val = $(this).val();
                            if (tag == "select") {
                                if (val != "") {
                                    val = $(this).find(":selected").text();
                                }
                            }

                            //Add by Jutarat A. (15/02/2012) 
                            //For support link
                            if (tag == "a") {
                                css = "label-readonly-view";
                                val = $(this).text();
                            }
                            //End Add

                            if ($.trim(val) == "") {
                                val = "-";
                                //                        var unit = $("#unit" + $(this).attr("id"));
                                //                        if (unit.length > 0) {
                                //                            unit.hide();
                                //                        }
                            }

                            if (tag == "textarea") {
                                val = val.replace(/\n/g, "<br/>");
                            }

                            /* --- Merge --- */
                            if (typeof (val) == "string" && val != undefined) {
                                val = ConvertSSH(val);
                            }
                            /* ------------- */

                            $(this).parent().find("#" + div).remove();
                            $(this).parent().append("<div id='" + div + "' class='" + css + "'>" + val + "</div>");
                        }
                        else if ($(this).hasClass("grid-datepicker-fromto")) {
                            var dtpFrom = $(this).find(".grid-datepicker-from");
                            var dtpTo = $(this).find(".grid-datepicker-to");

                            var cssFrom = "label-readonly-view";
                            if (!(dtpFrom[0].readOnly) || editonly == true)
                                cssFrom = "label-edit-view";
                            if (dtpFrom[0].disabled == true)
                                cssFrom = "label-readonly-view";
                            var valFrom = dtpFrom.val();

                            var cssTo = "label-readonly-view";
                            if (!(dtpTo[0].readOnly) || editonly == true)
                                cssTo = "label-edit-view";
                            if (dtpTo[0].disabled == true)
                                cssTo = "label-readonly-view";
                            var valTo = dtpTo.val();

                            $(this).hide();
                            $(this).parent().find("#" + div).remove();
                            var divView = $("<div></div>").attr("id", div)
                                .append($("<div></div>").addClass(cssFrom).text((valFrom ? valFrom : "-")))
                                //.append($("<div class='label-readonly-view'> - </div>"))
                                .append($("<div></div>").addClass(cssTo).text((valTo ? valTo : "-")));
                            $(this).parent().append(divView);
                        }
                    }
                }
                else {
                    if (tag == "input" && $(this).attr("type") == "checkbox") {
                        $(this).removeAttr("disabled");
                    } else if (tag == "input" && $(this).attr("type") == "radio") {
                        $(this).removeAttr("disabled");
                    }
                    else if ($(this).hasClass("grid-datepicker-fromto")) {
                        $(this).show();
                        $(this).parent().find("#" + div).remove();
                    }
                    else {
                        $(this).show();
                        var unitlst = ["unitDate", "unit"];
                        for (var ui = 0; ui < unitlst.length; ui++) {
                            var unit = $("#" + unitlst[ui] + $(this).attr("id"));
                            if (unit.length > 0) {
                                unit.show();
                            }
                        }

                        if (tag == "input" || tag == "select" || tag == "textarea" || tag == "a") {
                            /* --- For Datetime Picker --- */
                            $(this).parent().find("img").show();
                            $(this).parent().find("#" + div).remove();
                        }
                    }
                }
            }
        }
    });
}
$.fn.SetRadioGroupViewMode = function (isview) {
    var parent = $(this);
    parent.find("input[type=radio]").each(function () {
        var div = "div" + $(this).attr("id");

        if (isview) {
            $(this).hide();
            $("#span" + $(this).attr("id")).hide();

            var val = "";
            if ($(this).prop("checked") == true)
                val = $("#span" + $(this).attr("id")).text();
            if (val != "") {
                var css = "label-edit-view";
                parent.append("<div id='" + div + "' class='" + css + "'>" + val + "</div>");
            }
        }
        else {
            $(this).show();
            $("#span" + $(this).attr("id")).show();
            parent.find("#" + div).remove();
        }
    });
}

/* --- Numeric Box -------------------------------------------------- */
/* ------------------------------------------------------------------ */
$.fn.BindNumericBox = function (integer, decimal, minimum, maximum, default_minimum) {
    $(this).keypress(function (e) {
        var txt = String.fromCharCode(e.which);
        if (txt != "." && txt != "-" && isFinite(txt) == false) {
            return false;
        }

        var pidx = 0;
        var sidx = 0;
        var sel = master_event.GetInputTextSelection(e.target);
        if (sel != undefined) {
            pidx = sel.start;
            sidx = sel.end;
        }
        else {
            if (document.selection.type != "None") {
                var stxt = document.selection.createRange().text;
                pidx = $(this).val().indexOf(stxt);
                sidx = pidx + stxt.length;
            }
            else {
                pidx = $(this).val().length;
                sidx = pidx;
            }
        }

        var prefix = $(this).val().substring(0, pidx);
        var suffix = $(this).val().substring(sidx);

        //Check minus.
        var minus = 0;
        if (txt == "-") {
            if (prefix.indexOf("-") >= 0
					|| prefix.length > 0
					|| minimum >= 0)
                return false;
            minus = 1;
        }

        var num = prefix + txt + suffix;

        //Check dott
        if (num.indexOf("..") > 0)
            return false;

        //Check is Maximum length
        var idx = num.indexOf(".");
        if (idx >= 0) {
            var dNum = num.substring(idx + 1);
            if (dNum.length > decimal + minus)
                return false;
        }
        else {
            if (num.length > integer + minus) {
                return false;
            }
        }

        //Check Minimum/Maximum value
        var f = parseFloat(num);
        if (minimum != undefined) {
            if (f < minimum)
                return false;
        }
        if (maximum != undefined) {
            if (f > maximum)
                return false;
        }
    });
    $(this).keydown(function (e) {
        if (e.which == 229) {
            return false;
        }
    });
    $(this).blur(function () {
        var txt = $(this).val();
        /*txt = funcSpecial(txt);*/

        if ((txt == "" || txt == ".") && default_minimum == true) {
            $(this).val(minimum);
            txt = minimum.toString();
        }
        else {
            $(this).each(function () {
                $(this).val($(this).val().replace(/^(-?)0+([0-9])/, "$1$2"));
            });

            var num = $(this).val().replace(/ /g, "").replace(/,/g, "");
            var f = parseFloat(num);
            if (isNaN(f)) {
                txt = "";
            }
            else if (this.readOnly == false) {
                if (f < minimum || f > maximum) {
                    txt = "";
                }
            }
        }

        if (txt != "" && txt != undefined) {
            if (decimal > 0) {
                if (txt != "") {
                    var spt = txt.split(".");

                    if ($(this).NumericCurrencyValue() == C_CURRENCY_LOCAL)
                    {
                        var dt = spt[0] + ".";
                        var start = 0;
                        for (var idx = start; idx < decimal; idx++) {
                            dt += "0";
                        }
                        txt = dt;
                    }
                    else
                    {
                        var dt = "";
                        var start = 0;
                        if (spt.length > 1) {
                            start = spt[1].length;
                        }
                        else {
                            dt = ".";
                        }
                        for (var idx = start; idx < decimal; idx++) {
                            dt += "0";
                        }
                        txt += dt;
                    }
                }
            }
        }

        $(this).val(txt);
        $(this).setComma();
    });
    $(this).focus(function () {
        $(this).val($(this).val().replace(/ /g, "").replace(/,/g, ""));
        $(this).select();
    });

    $(this).bind("paste", function () {
        var ctrl = $(this);
        setTimeout(function () {
            var isError = false;
            var txt = ctrl.val().replace(/ /g, "").replace(/,/g, "");
            if (isFinite(txt) == false) {
                isError = true;
            }
            else if (txt < minimum || txt > maximum) {
                if (default_minimum == true) {
                    txt = new String(minimum);
                }
                else {
                    isError = true;
                }
            }
            if (isError == true) {
                ctrl.val("");
            }
            else {
                if (decimal > 0) {
                    var spt = txt.split(".");

                    if (ctrl.NumericCurrencyValue() == C_CURRENCY_LOCAL) {
                        var dt = spt[0] + ".";
                        var start = 0;
                        for (var idx = start; idx < decimal; idx++) {
                            dt += "0";
                        }
                        txt = dt;
                    }
                    else {
                        var dt = "";
                        var start = 0;
                        if (spt.length > 1) {
                            start = spt[1].length;
                        }
                        else {
                            dt = ".";
                        }
                        for (var idx = start; idx < decimal; idx++) {
                            dt += "0";
                        }
                        txt += dt;
                    }
                }

                ctrl.val(txt);
            }

            ctrl.select();
            ctrl.focus();
        });
    });
    var tmpIDNumeric = '#' + $(this).attr('id');
    $("#" + $(this).attr('id') + 'CurrencyType').change(function () {
        if ($(this).val() == C_CURRENCY_LOCAL)
        {
            var txt = $(tmpIDNumeric).val();
            if (txt != "" && txt != undefined) {
                if (decimal > 0) {
                    var spt = txt.split(".");
                    var dt = spt[0] + ".";
                    var start = 0;
                    for (var idx = start; idx < decimal; idx++) {
                        dt += "0";
                    }
                    txt = dt;
                }
            }
            $(tmpIDNumeric).val(txt);
        }
    });
}
$.fn.setComma = function () {
    if (($(this) != undefined) && ($(this).val() != undefined)) {
        var sout = "";
        // Added by Non A. 31/Jan/2012 : Left-Trim zero leading before passing to formatting event (.setComma()).
        $(this).each(function () {
            $(this).val($(this).val().replace(/^(-?)0+([0-9])/, "$1$2"));
        });
        var result = $(this).val().replace(/ /g, "").replace(/,/g, "");
        var spt = result.split(".");
        if (spt.length > 0) {
            var pos = 1;
            var num = 0;
            if (spt[0] != "")
                num = parseInt(spt[0]).toString();

            for (idx = num.length - 1; idx >= 0; idx--) {
                if (num.charAt(idx) != "-") {
                    if (pos % 4 == 0) {
                        sout = "," + sout;
                        pos = 1;
                    }
                }

                sout = num.charAt(idx) + sout;
                pos++;
            }

            if (spt.length > 1) {
                if (spt[1] != "") {
                    if (sout == "")
                        sout = "0";
                    else if (sout == "-")
                        sout = "-0";
                    sout = sout + "." + spt[1];
                }
            }
        }

        $(this).val(sout);
    }
}
$.fn.NumericValue = function () {
    if ($(this).val() != undefined)
        return $(this).val().replace(/ /g, "").replace(/,/g, "");
    else
        return null;
}

$.fn.SetNumericCurrency = function (currency) {
    var currCtrl = $(this).NumericCurrency();
    if (currCtrl.length > 0) {
        if (currCtrl[0].tagName.toUpperCase() == "SPAN") {
            currCtrl.find("span").hide();
            if (currency == undefined) {
                $(currCtrl.find("span")[0]).show();
            }
            else {
                currCtrl.find("span[data-type=" + currency + "]").show();
            }
        }
        else {
            currCtrl.val(currency);
        }
    }
}
$.fn.NumericCurrencyValue = function () {
    var currCtrl = $(this).NumericCurrency();
    if (currCtrl.length > 0) {
        if (currCtrl[0].tagName.toUpperCase() == "SPAN") {
            return currCtrl.find("span:visible").data("type");
        }

        return currCtrl.val();
    }

    return null;
}
// add by Jirawat Jannet @ 2016-10-20
// get currency combobox selected text
$.fn.NumericCurrencyText = function () {
    var currCtrl = $(this).NumericCurrency();
    var id = currCtrl.attr('id');
    return $('#' + id + ' option:selected').text();
}
$.fn.NumericCurrency = function () {
    var id = $(this).attr("id");
    return $("#" + id + "CurrencyType");
}


function SetNumericValue(num, dec) {
    if (num == undefined)
        return "";
    if (dec > 0) {
        if (num != "" || num == 0) {
            var spt = num.toString().split(".");

            var dt = "";
            var start = 0;
            if (spt.length > 1) {
                start = spt[1].length;

                //Add by Jutarat A. on 17072012
                if (start > dec) {
                    var strNum = num.toString();
                    strNum = strNum.substring(0, strNum.length - (start - dec));
                    return strNum;
                }
                //End Add
            }
            else {
                dt = ".";
            }

            for (var idx = start; idx < dec; idx++) {
                dt += "0";
            }
            num += dt;
        }
    }
    return num;
}

// For intial numberic texbox for Zipcode , Taxno ,Id no ,...
function InitialNumericInputTextBox(controls, isIncludeHyphen) {
    /// <summary>Method for intial numberic texbox ex. Zipcode , Taxno ,Idno ,...</summary>
    /// <param name="controls" type="array"> array of controls name </param>
    /// <param name="isIncludeHyphen" type="bool"> include hyphen (default = true) </param>

    if (isIncludeHyphen == undefined) {
        isIncludeHyphen = true;
    }

    for (var idx = 0; idx < controls.length; idx++) {
        if ($("#" + controls[idx]).length > 0) {
            $("#" + controls[idx]).bind('paste', function (e) {
                var el = $(this);
                setTimeout(function () {
                    var text = $(el).val();

                    var pattern;
                    if (isIncludeHyphen) {
                        pattern = /[^0-9\-]/g;
                    }
                    else {
                        pattern = /[^0-9]/g;
                    }


                    var match = text.match(pattern);

                    if (match != undefined) {
                        el.val("");
                    }
                   
                }, 0);
            });

            //$("#" + controls[idx]).unbind("keypress");
            $("#" + controls[idx]).keypress(function (e) {
                var checking;
                if (isIncludeHyphen) {
                    checking = (e.which == 0 || e.which == 8 || e.which == 45 || (e.which >= 48 && e.which <= 57));
                }
                else {
                    checking = (e.which == 0 || e.which == 8 || (e.which >= 48 && e.which <= 57));
                }

                if (checking) {
                    return true;
                } else {
                    return false;
                }
            });
        }
    }
}
/* ------------------------------------------------------------------ */

/* --- Time Box ----------------------------------------------------- */
/* ------------------------------------------------------------------ */
$.fn.BindTimeBox = function () {
    $(this).keypress(function (e) {
        var txt = String.fromCharCode(e.which);
        if (isFinite(txt) == false) {
            return false;
        }

        txt = $(this).val() + txt;
        if (txt.length > 4) { //time format (9999)
            return false;
        }
        else if (txt.length == 1) {	//first digit of hour must less than or equal 2 (time format 24 hr)
            if (txt[0] > 2) {
                return false;
            }
        }
        else if (txt.length == 2) { //second digit of hour must less than 4 in case first digit is 2 (time format 24 hr)
            if (txt[0] == 2
					&& txt[1] > 3) {
                return false;
            }
        }
        else if (txt.length == 3) { //first digit of minute must less than 6 (59 min is maximum)
            if (txt[2] > 5) {
                return false;
            }
        }
    });
    $(this).keydown(function (e) {
        if (e.which == 229) {
            return false;
        }
    });
    $(this).blur(function () {
        var isError = false;
        var txt = $(this).val().replace(/ /g, "").replace(/:/g, "");
        if (isFinite(txt) == false) {
            isError = true;
        }
        else if (txt.length > 4) { //time format (9999)
            isError = true;
        }
        else if (txt.length == 1) {	//first digit of hour must less than or equal 2 (time format 24 hr)
            if (txt[0] > 2) {
                isError = true;
            }
        }
        else if (txt.length == 2) { //second digit of hour must less than 4 in case first digit is 2 (time format 24 hr)
            if (txt[0] == 2
						&& txt[1] > 3) {
                isError = true;
            }
        }
        else if (txt.length == 3) { //first digit of minute must less than 6 (59 min is maximum)
            if (txt[2] > 5) {
                isError = true;
            }
        }

        if (isError == true) {
            $(this).val("");
        }
        else {
            //Convert number to format time (99:99)
            if (txt.length == 1)
                txt = "0" + txt + ":00";
            else if (txt.length == 2)
                txt = txt + ":00";
            else if (txt.length == 3)
                txt = txt.substring(0, 2) + ":0" + txt.substring(2, 3);
            else if (txt.length == 4)
                txt = txt.substring(0, 2) + ":" + txt.substring(2, 4);


            var obj = ToTimeSpan(txt);
            if (obj == null)
                txt = "";
            $(this).val(txt);
        }
    });
    $(this).focus(function () {
        $(this).val($(this).val().replace(/ /g, "").replace(/:/g, ""));
        $(this).select();
    });

    $(this).bind("paste", function () {
        var ctrl = $(this);
        setTimeout(function () {
            var isError = false;
            var txt = ctrl.val().replace(/ /g, "").replace(/:/g, "");
            if (isFinite(txt) == false) {
                isError = true;
            }
            else if (txt.length > 4) { //time format (9999)
                isError = true;
            }
            else if (txt.length == 1) {	//first digit of hour must less than or equal 2 (time format 24 hr)
                if (txt[0] > 2) {
                    isError = true;
                }
            }
            else if (txt.length == 2) { //second digit of hour must less than 4 in case first digit is 2 (time format 24 hr)
                if (txt[0] == 2
						&& txt[1] > 3) {
                    isError = true;
                }
            }
            else if (txt.length == 3) { //first digit of minute must less than 6 (59 min is maximum)
                if (txt[2] > 5) {
                    isError = true;
                }
            }

            if (isError == true) {
                ctrl.val("");
            }
            else {
                ctrl.val(txt);
            }

            ctrl.select();
            ctrl.focus();
        });
    });
}
function ConvetTimeSpan(time) {
    var txt = "";
    if (time != undefined) {
        txt = time.Hours;
        if (time.Hours < 10)
            txt = "0" + time.Hours;
        if (time.Minutes < 10)
            txt = txt + ":0" + time.Minutes;
        else
            txt = txt + ":" + time.Minutes;
    }

    return txt;
}
function ToTimeSpan(time) {
    if (time == undefined)
        return null;
    if (time.indexOf(":") < 0)
        return null;

    var sp = time.split(":");
    var obj = new Object();
    obj["Hours"] = parseInt(sp[0]);
    obj["Minutes"] = parseInt(sp[1]);

    if (obj.Hours < 0 || obj.Hours >= 24)
        obj = null;
    else if (obj.Minutes < 0 || obj.Minutes > 59)
        obj = null;

    return obj;
}
/* ------------------------------------------------------------------ */


$.fn.DisableControl = function () {
    var name = $(this).attr("id");
    if (GetDisableFlag(name) == true) {
        var tag = $(this)[0].tagName.toLowerCase();
        if ((tag == "input" && $(this).attr("type") == "text")
            || tag == "textarea") {
            $(this).attr("readonly", true);
        }
        else {
            $(this).attr("disabled", true);
        }
    }
}
$.fn.DisableCheckListControl = function () {
    var name = $(this).attr("id");
    if (GetDisableFlag(name) == true) {
        $(this).find("input[type=checkbox]").each(function () {
            $(this).attr("disabled", true);
        });
    }
}
function DisableRadioControl(name, suffix) {
    if (GetDisableFlag(name) == true) {
        for (var idx = 0; idx < suffix.length; idx++) {
            $("#" + name + suffix[idx]).attr("disabled", true);
        }
    }
}
$.fn.DisableDatePickerControl = function () {
    var name = $(this).attr("id");
    if (GetDisableFlag(name) == true) {
        var tag = $(this)[0].tagName.toLowerCase();
        if (tag == "input" && $(this).attr("type") == "text") {
            $(this).EnableDatePicker(false);
        }
    }
}
function GetDisableFlag(name) {
    var disable_flag = "#Disabled" + name;
    if ($(disable_flag).val() != undefined) {
        if ($(disable_flag).val().toLowerCase() == "true") {
            return true;
        }
    }
    return false;
}






$.fn.SetEnableView = function (enable) {
    $(this).find("input[type=text],input[type=checkbox],textarea,button,select,input[type=radio]").each(function () {
        var tag = this.tagName.toLowerCase();
        if (enable == false) {
            if ((tag == "input" && $(this).attr("type") == "text")
                || tag == "textarea") {
                $(this).attr("readonly", true);
            }
            else {
                $(this).attr("disabled", true);
            }
        }
        else {
            if ((tag == "input" && $(this).attr("type") == "text")
                || tag == "textarea") {
                $(this).removeAttr("readonly");
            }
            else {
                $(this).removeAttr("disabled");
            }
        }
    });
}

//-- Nattapong N. 29 Sep 2011------------------------

$.fn.SetEnableViewIfReadonly = function (enable) {
    $(this).find("input[type=text],input[type=checkbox],textarea,button,select,input[type=radio]").each(function () {
        var tag = this.tagName.toLowerCase();
        if (enable == false) {
            if ((tag == "input" && $(this).attr("type") == "text") || tag == "textarea") {
                if ($(this).prop("readonly")) {
                    $(this).attr("class", "ReadOnlyCtrl");
                }
                $(this).attr("readonly", true);
            }
            else {
                if ($(this).prop("disabled"))
                    $(this).attr("class", "ReadOnlyCtrl");
                else
                    $(this).attr("disabled", true);
            }
        }
        else {
            if ((tag == "input" && $(this).attr("type") == "text") || tag == "textarea") {
                if ($(this).attr("class") == "ReadOnlyCtrl") {
                    $(this).removeAttr("class");
                } else {
                    $(this).removeAttr("readonly");
                }

            }
            else {
                if ($(this).attr("class") == "ReadOnlyCtrl")
                    $(this).removeAttr("class");
                else
                    $(this).removeAttr("disabled");
            }
        }
    });
}
$.fn.SetReadOnly = function (IsReadOnly) {
    if (IsReadOnly) {
        $(this).attr("readonly", true);
    } else if (IsReadOnly == false) {
        $(this).removeAttr("readonly");
    }
}
$.fn.SetDisabled = function (IsDisabled) {
    var name = $(this).attr("id");
    var tag = $(this).prop("tagName").toLowerCase();
    if (tag == 'text'
    || tag == 'button'
    || tag == 'select'
    || tag == 'radio'
    || tag == 'checkbox'
    || tag == 'input'
    || tag == 'textarea'
    ) {
        if (IsDisabled) {
            if (($(this).attr("type") == "text") || tag == 'textarea')
                $(this).SetReadOnly(IsDisabled);
            else
                $(this).attr("disabled", true);
        } else {
            if (($(this).attr("type") == "text") || tag == 'textarea')
                $(this).SetReadOnly(IsDisabled);
            else
                $(this).removeAttr("disabled");
        }
        return false;
    }
    //$(this).find("input[type='text'],input[type='radio'],input[type='button'],button,input[type='select']").each(function () {
    $(this).find("input[type=text],input[type=checkbox],input[type=radio],textarea,button,select,a").each(function () {

        if (IsDisabled) {
            if ($(this).attr("type") == "text" || $(this).prop("tagName").toLowerCase() == "textarea")
                $(this).SetReadOnly(IsDisabled);
            else
                $(this).attr("disabled", true);
        } else {
            if ($(this).attr("type") == "text" || $(this).prop("tagName").toLowerCase() == "textarea")
                $(this).SetReadOnly(IsDisabled);
            else
                $(this).removeAttr("disabled");
        }
    });
}
$.fn.SetNumericValue_Prefix = function (prefix, obj, dec) {
    var strProp;
    this.each(function () {
        strProp = $(this).attr("id");
        strProp = strProp.substring(prefix.length);
        $(this).val(SetNumericValue(obj[strProp], dec));
        $(this).setComma();
    });

}
//---------------------------------------------------

$.fn.SetEmptyViewData = function () {
    $(this).find("div[class=label-view]").each(function () {
        if ($(this).html() == "" || $(this).html() == "&nbsp;")
            $(this).html("-");
    });
}

/* --- Text Area -------------------------------------------------- */
/* ------------------------------------------------------------------ */
$.fn.SetMaxLengthTextArea = function (maxLength, maxRow) {
    //    $(this).keypress(function (e) {
    //        if ($(this).val().length + 1 > maxLength) {
    //            return false;
    //        }
    //    });

    //    $(this).keyup(function (e) {
    //        if (this.value.length > maxLength) {
    //            while (this.value.length > maxLength) {
    //                this.value = this.value.replace(/.$/, '');
    //            }
    //        }
    //    });

    //    $(this).blur(function (e) {
    //        if (this.value.length > maxLength) {
    //            while (this.value.length > maxLength) {
    //                this.value = this.value.replace(/.$/, '');
    //            }
    //        }
    //    });



    $(this).keypress(function (e) {

        var numRow = this.value.split("\n").length;

        //Get the event object (for IE)
        var ob = e || event;
        //Get the code of key pressed
        var keyCode = ob.keyCode;
        //Check if it has a selected text
        var hasSelection = document.selection ? document.selection.createRange().text.length > 0 : this.selectionStart != this.selectionEnd;


        if (maxRow != undefined && maxRow != 0) {
            if (numRow > maxRow) {
                return false;
            }
        }

        //return false if can't write more
        return !(this.value.length >= maxLength && (keyCode > 50 || keyCode == 32 || keyCode == 0 || keyCode == 13) && !ob.ctrlKey && !ob.altKey && !hasSelection);

        

    });
    //Add the key up event
    $(this).keyup(function () {

        var numRow = this.value.split("\n").length;

        if (maxRow != undefined && maxRow != 0) {
            if (numRow > maxRow) {
                var textList = this.value.replace(/(\r\n|\n|\r)/gm,"\n").split("\n");
                textList.splice(maxRow, numRow - maxRow);
                this.value = textList.join("\n");

                if (this.value.length > maxLength) {
                    this.value = this.value.substring(0, maxLength);
                }
            }
        }


        //If the keypress fail and allow write more text that required, this event will remove it
        if (this.value.length > maxLength) {
            this.value = this.value.substring(0, maxLength);
        }

        

    });
}


function ConvertBlockHtml(txt) {
    var result = txt;
    if (result != undefined) {
        result = result.replace(/[&]/g, '&amp;');
        result = result.replace(/[<]/g, '&lt;');
        result = result.replace(/[>]/g, '&gt;');
    }

    return result;
}


function InitialTrimTextEvent(controls) {
    if (controls == undefined)
        return;

    for (var idx = 0; idx < controls.length; idx++) {
        if ($("#" + controls[idx]).length > 0) {
            $("#" + controls[idx]).blur(function () {
                if ($(this).prop("readonly") != true) {
                    $(this).val($.trim($(this).val()));
                }
            });
        }
    }
}


var isTabKey = false;
var ctrlCurrentVal = null;
$.fn.RelateControlEvent = function (func) {
    $(this).change(function () {
        func(false);
    });
    $(this).keydown(function (e) {
        if (e.which == 9 && e.shiftKey == false) {
            isTabKey = true;
        }
    });

    $(this).focus(function () {
        ctrlCurrentVal = $(this).val();
    });
    $(this).blur(function () {
        if (ctrlCurrentVal != $(this).val()) {
            func(isTabKey, true);
        }

        isTabKey = false;
    });
}


$.fn.SetCheckBoxListViewMode = function () {
    $(this).find("span").each(function () {
        if ($(this).parent().attr("class") != "fieldset-header") {
            $(this).attr("class", "label-checkbox-view");
        }
    });
}

function SetControlFromToView(from, to, fromto) {
    var vfrom = $("#div" + from).html();
    if (vfrom == "" || vfrom == undefined)
        vfrom = $("#" + from).html();

    var vto = $("#div" + to).html();
    if (vto == "" || vto == undefined)
        vto = $("#" + to).html();

    if (vfrom == "-" && vto == "-") {
        if ($("#div" + to).length > 0)
            $("#div" + to).html("&nbsp;");
        else
            $("#" + to).html("&nbsp;");

        if ($("#div" + fromto).length > 0)
            $("#div" + fromto).hide();
    }
}


/* --- Merge --- */
function ConvertSSH(txt) {
    var x = txt;

    var y = txt.toUpperCase();
    var bidx = y.indexOf("<SCRIPT");
    var eidx = y.indexOf("</SCRIPT");
    if (bidx >= 0) {
        var bx = txt.substring(0, bidx);
        if (eidx >= 0) {
            var nx = txt.substring(bidx, eidx);
            var enx = txt.substring(eidx);

            nx = nx + enx.substring(0, enx.indexOf(">") + 1);
            nx = nx.replace(/</g, "&lt;").replace(/>/g, "&gt;");

            var ex = enx.substring(enx.indexOf(">") + 1);
            x = nx + ConvertSSH(ex);
        }
        else {
            var nx = txt.substring(bidx);

            var enx = nx.substring(0, nx.indexOf(">") + 1);
            enx = enx.replace(/</g, "&lt;").replace(/>/g, "&gt;");

            var ex = nx.substring(nx.indexOf(">") + 1);
            x = enx + ConvertSSH(ex);
        }

        x = bx + x;
    }

    return x;
}
/* ------------- */

//Add by Jutarat A. on 31072013
function addCommas(nStr) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

function SetNumericText(num, dec) {
    if (num == undefined)
        return "";

    var obj = parseFloat(num);
    num = addCommas(obj.toFixed(dec));
    return num;
}
//End Add

function DecodeHtml(encoded) {
    var elem = document.createElement('textarea');
    elem.innerHTML = encoded;
    return elem.value;
}

