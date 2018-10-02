/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>

$.fn.InitialDate = function () {
    $(this).datepicker({
        showOn: 'button',
        //showButtonPanel: true,
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: '',
        buttonImage: '/Content/images/calendar.png',
        dateFormat: 'dd-M-yy',
        beforeShow: function (dateText, inst) {
            $(this).unbind("focus");
            $(this).unbind("blur");
            $(this).unbind("keypress");
            $(this).unbind("keydown");
            $(this).unbind("paste");
            $(this).unbind("drop");
        },
        onClose: function (dateText, inst) {
            $(this).ResetToNormalControl();
            $(this).blur(SetDateWhenBlur);
            $(this).focus(SetDateFocus);
            $(this).keypress(DateKeyPress);
            $(this).keydown(DateKeyDown);
            $(this).bind("paste", DatePaste);
            $(this).bind("drop", DatePaste);

            $(this).focus();
            $(this).select();
        }
    });

    $(this).blur(SetDateWhenBlur);
    $(this).focus(SetDateFocus);
    $(this).keypress(DateKeyPress);
    $(this).keydown(DateKeyDown);
    $(this).bind("paste", DatePaste);
    $(this).bind("drop", DatePaste);

    $(this).parent().find("img[class='ui-datepicker-trigger']").css({
        "vertical-align": "bottom",
        "padding-left": "1px"
    });
}

function InitialDateFromToControl(from_ctrl, to_ctrl) {
    var ctrl = from_ctrl + ", " + to_ctrl;
    var dates = null;

    var from_to_blur_function = function () {
        var txt = $(this).val();
        if (txt.length == 8 && txt.match(/^[0-9]+$/)) { //Modified by Non A. 26/Mar/2012 : Fix bug when input 8 character's which not date format can caused error
            var dd = txt.substring(0, 2);
            if (dd.charAt(0) == "0")
                dd = dd.substring(1, 2);
            dd = parseInt(dd);

            var mm = txt.substring(2, 4);
            if (mm.charAt(0) == "0")
                mm = mm.substring(1, 2);
            mm = parseInt(mm);

            var yy = parseInt(txt.substring(4, 8), 10);

            if (isNaN(dd) == false
                && isNaN(mm) == false
                && isNaN(yy) == false) {
                mm = mm - 1;
                var d = new Date(yy, mm, dd, 0, 0, 0);
                if (d != NaN && d != null) {
                    if (dd != d.getDate()
                        || mm != d.getMonth()
                        || yy != d.getFullYear()
                        || d.getFullYear() > 9999
                        || d.getFullYear() < 1962) {

                        $(this).val("");

                        var inst2 = dates.not(this).data("datepicker");
                        $(this).datepicker("setDate", null);

                        var option = this.id == $(from_ctrl).attr("id") ? "minDate" : "maxDate";
                        var nd = new Date(inst2.selectedYear, inst2.selectedMonth, inst2.selectedDay, 0, 0, 0);

                        var ctrl2 = dates.not(this);
                        if (ctrl2.val() != "") {
                            ctrl2.datepicker("setDate", nd);
                        }
                        ctrl2.datepicker("option", option, null);

                        dates.parent().find("img[class='ui-datepicker-trigger']").css({
                            "vertical-align": "bottom",
                            "padding-left": "1px"
                        });

                        $(this).trigger('BlurAfterSetDate');
                        return;
                    }
                }

                var inst1 = $(this).data("datepicker");
                var inst2 = dates.not(this).data("datepicker");

                $(this).datepicker("setDate", d);
                var option = this.id == $(from_ctrl).attr("id") ? "minDate" : "maxDate";
                var date = $.datepicker.parseDate(
						                inst1.settings.dateFormat ||
						                $.datepicker._defaults.dateFormat,
						                $(this).val(), inst1.settings);

                var nd = new Date(inst2.selectedYear, inst2.selectedMonth, inst2.selectedDay, 0, 0, 0);

                var ctrl2 = dates.not(this);
                if (ctrl2.val() != "") {
                    ctrl2.datepicker("setDate", nd);
                }
                ctrl2.datepicker("option", option, date);
                // Added by Non A. 26/Jul/2012 : Fix bug when leave focus on disabled datepicker, trigger icon will return to normal state (opacity removed).
                ctrl2.EnableDatePicker(!(ctrl2.datepicker("isDisabled") || false)); 

                dates.parent().find("img[class='ui-datepicker-trigger']").css({
                    "vertical-align": "bottom",
                    "padding-left": "1px"
                });
            }
            else {
                $(this).focus();
                $(this).select();
            }
        }
        else {
            var inst2 = dates.not(this).data("datepicker");

            $(this).datepicker("setDate", null);

            var option = this.id == $(from_ctrl).attr("id") ? "minDate" : "maxDate";
            var nd = new Date(inst2.selectedYear, inst2.selectedMonth, inst2.selectedDay, 0, 0, 0);

            var ctrl2 = dates.not(this);
            if (ctrl2.val() != "") {
                ctrl2.datepicker("setDate", nd);
            }
            ctrl2.datepicker("option", option, null);
            // Added by Non A. 26/Jul/2012 : Fix bug when leave focus on disabled datepicker, trigger icon will return to normal state (opacity removed).
            ctrl2.EnableDatePicker(!(ctrl2.datepicker("isDisabled") || false));

            dates.parent().find("img[class='ui-datepicker-trigger']").css({
                "vertical-align": "bottom",
                "padding-left": "1px"
            });
        }

        $(this).trigger('BlurAfterSetDate');
    };

    dates = $(ctrl).datepicker({
        //defaultDate: "+1w",
        changeMonth: true,
        changeYear: true,
        showOn: 'button',
        buttonImageOnly: true,
        //showButtonPanel: true,
        buttonText: '',
        buttonImage: '/Content/images/calendar.png',
        dateFormat: 'dd-M-yy',
        onSelect: function (selectedDate) {
            var option = this.id == $(from_ctrl).attr("id") ? "minDate" : "maxDate";
            var instance = $(this).data("datepicker");
            var date = $.datepicker.parseDate(
						instance.settings.dateFormat ||
						$.datepicker._defaults.dateFormat,
						selectedDate, instance.settings);

            dates.not(this).datepicker("option", option, date);
            dates.not(this).parent().find("img[class='ui-datepicker-trigger']").css({
                "vertical-align": "bottom",
                "padding-left": "1px"
            });
        },
        beforeShow: function (dateText, inst) {
            $(this).unbind("focus");
            $(this).unbind("blur", from_to_blur_function);
            $(this).unbind("keypress");
            $(this).unbind("keydown");
            $(this).unbind("paste");
            $(this).unbind("drop");
        },
        onClose: function (dateText, inst) {
            $(this).ResetToNormalControl();
            $(this).blur(from_to_blur_function);
            $(this).focus(SetDateFocus);
            $(this).keypress(DateKeyPress);
            $(this).keydown(DateKeyDown);
            $(this).bind("paste", DatePaste);
            $(this).bind("drop", DatePaste);

            $(this).focus();
            $(this).select();
        }
    });

    $(ctrl).blur(from_to_blur_function);
    $(ctrl).focus(SetDateFocus);
    $(ctrl).keypress(DateKeyPress);
    $(ctrl).keydown(DateKeyDown);
    $(ctrl).bind("paste", DatePaste);
    $(ctrl).bind("drop", DatePaste);

    $(ctrl).parent().find("img[class='ui-datepicker-trigger']").css({
        "vertical-align": "bottom",
        "padding-left": "1px"
    });
}

$.fn.EnableDatePicker = function (enable) {
    if (enable) {
        $(this).datepicker("enable");
        $(this).removeAttr("readonly");
    }
    else {
        $(this).datepicker("disable");
        $(this).removeAttr("disabled");
        $(this).attr("readonly", "readonly");
    }
}
$.fn.SetDate = function (date) {
    $(this).datepicker("setDate", date);
    $(this).val(ConvertDateToTextFormat(date));
}

function DateKeyPress(e) {
    var con = false;
    if (e.srcElement.selectionStart != undefined
            || document.selection.type != "None"
            || $(this).val().length + 1 <= 8) {
        con = true;
    }

    if (con == true) {
        var key = parseInt(String.fromCharCode(e.which));
        if (isNaN(key)) {
            return false;
        }
    }
    else {
        return false;
    }
}
function DateKeyDown(e) {
    if (e.which == 229) {
        return false;
    }
}
function DatePaste () {
    var ctrl = $(this);
    setTimeout(function () {
        var next_step = true;
        var num = parseInt(ctrl.val().replace(/ /g, ""));
        if (isNaN(num)) {
            ctrl.val("");
            next_step = false;
        }

        if (next_step) {
            var txt = ctrl.val();
            if (txt.length == 8 && txt.match(/^[0-9]+$/)) { //Modified by Non A. 26/Mar/2012 : Fix bug when input 8 character's which not date format can caused error
                var dd = txt.substring(0, 2);
                if (dd.charAt(0) == "0")
                    dd = dd.substring(1, 2);
                dd = parseInt(dd);

                var mm = txt.substring(2, 4);
                if (mm.charAt(0) == "0")
                    mm = mm.substring(1, 2);
                mm = parseInt(mm);

                var yy = parseInt(txt.substring(4, 8), 10);

                var isChange = true;
                if (isNaN(dd) == false
                    && isNaN(mm) == false
                    && isNaN(yy) == false) {
                    mm = mm - 1;

                    var d = new Date(yy, mm, dd, 0, 0, 0);
                    if (d != NaN && d != null) {
                        if (dd != d.getDate()
                            || mm != d.getMonth()
                            || yy != d.getFullYear()
                            || d.getFullYear() > 9999
                            || d.getFullYear() < 1962) {

                            ctrl.val("");
                            ctrl.datepicker("setDate", null);
                            return;
                        }

                        var cd = ctrl.datepicker("getDate");
                        if (cd - d == 0) {
                            isChange = false;
                        }
                        ctrl.datepicker("setDate", d);
                    }
                }

                if (isChange == true) {
                    ctrl.removeClass("highlight");
                }
            }
            else {
                ctrl.val("");
                ctrl.datepicker("setDate", null);
            }

            ctrl.select();
            ctrl.focus();
        }

    }, 0);
}

function SetDateFromToData(from_ctrl, to_ctrl, from_date, to_date) {
    $(from_ctrl).datepicker("setDate", from_date);
    $(to_ctrl).datepicker("setDate", to_date);

    $(from_ctrl).datepicker("option", "maxDate", to_date);
    $(to_ctrl).datepicker("option", "minDate", from_date);

    var ctrl = from_ctrl + ", " + to_ctrl;
    $(ctrl).parent().find("img[class='ui-datepicker-trigger']").css({
        "vertical-align": "bottom",
        "padding-left": "1px"
    });
}
function ClearDateFromToControl(from_ctrl, to_ctrl) {
    var ctrl = from_ctrl + ", " + to_ctrl;

    $(ctrl).val("");
    $(ctrl).datepicker("option", { minDate: null, maxDate: null });
    $(ctrl).parent().find("img[class='ui-datepicker-trigger']").css({
        "vertical-align": "bottom",
        "padding-left": "1px"
    });
}

function SetDateWhenBlur(event) {
    /// <summary>Event when blur from text</summary>
    var txt = $(this).val();

    if (txt.length == 8 && txt.match(/^[0-9]+$/)) { //Modified by Non A. 26/Mar/2012 : Fix bug when input 8 character's which not date format can caused error
        var dd = txt.substring(0, 2);
        if (dd.charAt(0) == "0")
            dd = dd.substring(1, 2);
        dd = parseInt(dd);

        var mm = txt.substring(2, 4);
        if (mm.charAt(0) == "0")
            mm = mm.substring(1, 2);
        mm = parseInt(mm);

        var yy = parseInt(txt.substring(4, 8), 10);

        var isChange = true;
        if (isNaN(dd) == false
            && isNaN(mm) == false
            && isNaN(yy) == false) {
            mm = mm - 1;

            var d = new Date(yy, mm, dd, 0, 0, 0);
            if (d != NaN && d != null) {
                if (dd != d.getDate()
                    || mm != d.getMonth()
                    || yy != d.getFullYear()
                    || d.getFullYear() > 9999
                    || d.getFullYear() < 1962) {
                    
                    $(this).val("");
                    $(this).datepicker("setDate", null);
                    $(this).trigger('BlurAfterSetDate');
                    return;
                }

                var cd = $(this).datepicker("getDate");
                if (cd - d == 0) {
                    isChange = false;
                }
                $(this).datepicker("setDate", d);
                isCorrect = true;
            }
        }
        if (isCorrect == false) {
            $(this).focus();
            $(this).select();
        }
        else if (isChange == true) {
            $(this).removeClass("highlight");
        }
    }
    else {
        $(this).datepicker("setDate", null);
    }

    $(this).trigger('BlurAfterSetDate');
}
function SetDateFocus() {
    /// <summary>Event when focuse to text</summary>
    var ctxt = $(this).val();
    if (ctxt != "") {
        var instance = $(this).data("datepicker");
        if (instance.selectedDay == 0
            && instance.selectedMonth == 0
            && instance.selectedYear == 0) {
            $(this).datepicker("setDate", ctxt);
        }

        var ddate = instance.selectedDay;
        if (ddate < 10)
            ddate = "0" + ddate;
        var dmonth = instance.selectedMonth + 1;
        if (dmonth < 10)
            dmonth = "0" + dmonth;
        var dyear = instance.selectedYear;

        var txt = "" + ddate + dmonth + dyear;
        $(this).val(txt);
        $(this).select();
    }
}
function ConvertDateToTextFormat(date) {
    if (date != undefined) {
        var d = new Date(date);
        var day = d.getDate();
        if (day < 10)
            day = "0" + day;

        var month = "";
        var year = d.getFullYear();

        var spTxt = d.toString().split(" ");
        if (spTxt.length > 1) {
            month = spTxt[1];
        }

        return day + "-" + month + "-" + year;
    }

    return "";
}

$.fn.SetMaxDate = function (maxDate) {
    $(this).datepicker("option", "maxDate", maxDate)
        .parent().find("img[class='ui-datepicker-trigger']").css({
            "vertical-align": "bottom",
            "padding-left": "1px"
        });
}

$.fn.SetMinDate = function (minDate) {
    $(this).datepicker("option", "minDate", minDate)
    .parent().find("img[class='ui-datepicker-trigger']").css({
        "vertical-align": "bottom",
        "padding-left": "1px"
    });
}