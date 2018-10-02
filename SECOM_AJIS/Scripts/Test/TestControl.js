/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>
/// <reference path="../../Scripts/Base/AutoComplete.js"/>
/// <reference path="../json.js" />

$(document).ready(function () {
    $("#btnDialogE").click(function () {
        var obj = {
            module: "Common",
            code: "MSG0001"
        };
        call_ajax_method("/Shared/GetMessage", obj, function (data) {
            OpenErrorDialog(data.Code, data.Message);
        });

//        $.ajax({
//            type: "POST",
//            url: "/us/Shared/GetMessage",
//            data: obj,
//            success: function (data) {
//                OpenErrorDialog(data.Code, data.Message);
//            },
//            error: function (data, stext) {
//                OpenErrorDialog("MSG0000", "Cannot get load data.");
//            }
//        });
    });
    $("#btnDialog1").click(function () {
        OpenErrorMessageDialog("MSG0001", "Test Default Dialog");
    });
    $("#btnDialog2").click(function () {
        OpenYesNoMessageDialog("MSG0001", "Test Yes/No Dialog", function () {
            alert("Yes Event");
        }, function () {
            alert("No Event");
        });
    });
    $("#btnDialogW").click(function () {
        OpenWarningDialog("<ul><li>Error 1</li><li>Error 2</li><li>Error 3</li></ul>");
    });

    // --- Initialize List ---
    $("#iList").keypress(txt_keypress);

    // --- Date Picker ---
    $("#datepicker").InitialDate();
    //InitialDateControl("#datepicker");

    InitialDateFromToControl("#from", "#to");

    $("#datepicker2").InitialDate();
    $("#datepicker2").EnableDatePicker(false);
});

function txt_keypress(e) {
    if ($(this).val().length + 1 >= 3) {
        var cond = $(this).val() + String.fromCharCode(e.which);
        InitialAutoCompleteControl("#iList",
                                cond,
                                "/Master/GetCustAddress",
                                { "cond": cond },
                                "doCustAddress",
                                "Address",
                                "Address");
    }
}

