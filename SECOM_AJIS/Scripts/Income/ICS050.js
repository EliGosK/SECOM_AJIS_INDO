/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/control_events.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>
/// <reference path="../Base/object/ajax_method.js" />
/// <reference path="../Base/object/command_event.js" />
/// <reference path="../Base/object/master_event.js" />

var ics050_var = {
    isRetrieveInvoice: false
};

ics050_var.screenMode = {
    registerMode: "RegisterMode",
    confirmMode: "ConfirmMode"
};
ics050_var.controls = {
    invoiceNo: "#InvoiceNo",
    billingTargetcode: "#BillingTargetCode",
    billingClientNameENG: "#BillingClientNameENG",
    billingClientNameLOCAL: "#BillingClientNameLOCAL",
    issueInvoiceDate: "#IssueInvoiceDate",
    billingAmount: "#BillingAmount",
   
    btnRetrieve: "#BtnRetrieve",
    invoiceDetail: "#DivInvoiceDetail",
    inputInvoice: "#DivInputInvoice"
};



var ics050_process = {
    InitialScreen: function () {
        $(ics050_var.controls.issueInvoiceDate).InitialDate();

        ics050_process.BidingEvent();
        ics050_process.SetScreenMode(ics050_var.screenMode.registerMode);
    },

    BidingEvent: function () {
        $(ics050_var.controls.btnRetrieve).click(ics050_process.CmdRetrieve_OnClick);
    },

    SetScreenMode: function (mode) {
        if (mode == ics050_var.screenMode.registerMode) {
            $(ics050_var.controls.inputInvoice).SetViewMode(false);
            if (ics050_var.isRetrieveInvoice == true) {
                $(ics050_var.controls.invoiceNo).SetDisabled(true);
                $(ics050_var.controls.btnRetrieve).SetDisabled(true);
            }
            else {
                $(ics050_var.controls.invoiceNo).SetDisabled(false);
                $(ics050_var.controls.btnRetrieve).SetDisabled(false);
            }
            $(ics050_var.controls.issueInvoiceDate).SetDisabled(false);

            register_command.SetCommand(ics050_process.CmdRegister_OnClick);
            reset_command.SetCommand(ics050_process.CmdReset_OnClick);
            confirm_command.SetCommand(null);
            back_command.SetCommand(null);
        }
        else if (mode == ics050_var.screenMode.confirmMode) {
            $(ics050_var.controls.inputInvoice).SetViewMode(true);

            register_command.SetCommand(null);
            reset_command.SetCommand(null);
            confirm_command.SetCommand(ics050_process.CmdConfirm_OnClick);
            back_command.SetCommand(ics050_process.CmdBack_OnClick);
        }
    },


    CmdRetrieve_OnClick: function () {
        //Retrive invoice information and display on screen
        var obj = {
            InvoiceNo: $(ics050_var.controls.invoiceNo).val()
        };
        ics050_var.isRetrieveInvoice = false;
        ajax_method.CallScreenController("/Income/ICS050_GetInvoice", obj, function (result, controls) {
            if (result != undefined) {
                //Success
                $(ics050_var.controls.billingTargetcode).val(result.BillingTargetCodeShortFormat);
                $(ics050_var.controls.billingClientNameENG).val(result.BillingClientNameEN);
                $(ics050_var.controls.billingClientNameLOCAL).val(result.BillingClientNameLC);
                $(ics050_var.controls.billingAmount).val((new Number(result.BillingAmountIncVATGridDisplay)).numberFormat("#,##0.00"));
                $(ics050_var.controls.billingAmount).NumericCurrency().val(result.InvoiceAmountCurrencyType);
                $(ics050_var.controls.issueInvoiceDate).val("");

                $(ics050_var.controls.btnRetrieve).SetDisabled(true);
                $(ics050_var.controls.invoiceNo).SetDisabled(true);
                ics050_var.isRetrieveInvoice = true;
            }
            else if (controls != undefined) {
                VaridateCtrl(controls, controls);
                $(ics050_var.controls.invoiceDetail).clearForm();
            }
        });
    },

    CmdRegister_OnClick: function () {
        var invoiceNo = "";

        //Assign when already retrieved invoice information
        if (ics050_var.isRetrieveInvoice == true)
            invoiceNo = $(ics050_var.controls.invoiceNo).val();

        var obj = {
            InvoiceNo: invoiceNo,
            IssueInvoiceDate: $(ics050_var.controls.issueInvoiceDate).val()
        };

        ajax_method.CallScreenController("/Income/ICS050_cmdRegister", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                //Goto confirm state
                ics050_process.SetScreenMode(ics050_var.screenMode.confirmMode);
            }
            else if (controls != undefined) {
                VaridateCtrl(controls, controls);
            }
        });
    },

    CmdConfirm_OnClick: function () {
        var obj = {
            InvoiceNo: $(ics050_var.controls.invoiceNo).val(),
            IssueInvoiceDate: $(ics050_var.controls.issueInvoiceDate).val()
        };

        ajax_method.CallScreenController("/Income/ICS050_cmdConfirm", obj,
        function (result, controls, isWarning) {
            if (result != undefined) {
                //Success
                var objMsg = { module: "Income", code: "MSG7008" };
                call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                    OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {
                        //Show pdf file on new window
                        // Comment by Jirawat Jannet @ 2016-10-17
                        var key = ajax_method.GetKeyURL(null);
                        var url = ajax_method.GenerateURL("/Income/ICS050_DisplayReport?k=" + key);
                        window.open(url, "", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes"); //Merge at 14032017 By Pachara S.

                        //Same as reset screen
                        ics050_process.CmdReset_OnClick();
                    });
                });
            }
            else if (controls != undefined) {
                ics050_process.SetScreenMode(ics050_var.screenMode.registerMode);
                VaridateCtrl(controls, controls);
            }
        });
    },

    CmdBack_OnClick: function () {
        ics050_process.SetScreenMode(ics050_var.screenMode.registerMode);
    },

    CmdReset_OnClick: function () {
        //Reset all and get back as initial screen state
        ics050_var.isRetrieveInvoice = false;
        $(ics050_var.controls.inputInvoice).clearForm();
        ics050_process.SetScreenMode(ics050_var.screenMode.registerMode);
    }
};




$(document).ready(function () {
    ics050_process.InitialScreen();
});