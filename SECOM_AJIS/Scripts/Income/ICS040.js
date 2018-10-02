/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/control_events.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>

/// <reference path="../../Scripts/Base/ComboBox.js"/>
/// <reference path="../Base/object/ajax_method.js" />
/// <reference path="../Base/object/command_event.js" />
/// <reference path="../Base/object/master_event.js" />
/// <reference path="Dialog.js" />

var ics040_var = {
    objInvoiceGrid: null,
    objInvoiceConfirmGrid: null
};
ics040_var.screenMode = {
    registerMode: "RegisterMode",
    confirmMode: "ConfirmMode"
};
ics040_var.controls = {
    invoiceNo: "#InvoiceNo",
    billingClientNameENG: "#BillingClientNameENG",
    billingClientNameLOCAL: "#BillingClientNameLOCAL",
    issueInvoiceDate: "#IssueInvoiceDate",
    billingAmount: "#BillingAmount",
    invoiceDetailQTY: "#InvoiceDetailQTY",
    approveNo: "#ApproveNo",
    
    btnRetrieve: "#BtnRetrieve",
    btnAdd: "#BtnAdd",
    btnCancel: "#BtnCancel",

    invoiceGrid: "#InvoiceGrid",
    invoiceConfirmGrid: "#InvoiceConfirmGrid",

    DivInputInvoice: "#DivInputInvoice",
    DivInvoiceDetail: "#DivInvoiceDetail",
    DivInvoiceGrid: "#DivInvoiceGrid",
    DivInvoiceConfirmGrid: "#DivInvoiceConfirmGrid"
};


var ics040_process = {
    InitialScreen: function () {
        ics040_process.InitialGrid();
        ics040_process.BidingEvent();
        ics040_process.SetScreenMode(ics040_var.screenMode.registerMode);
    },

    InitialGrid: function () {
        if ($.find(ics040_var.controls.invoiceGrid).length > 0) {
            ics040_var.objInvoiceGrid = $(ics040_var.controls.invoiceGrid).InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Income/ICS040_InitialInvoiceGrid");
        }
        if ($.find(ics040_var.controls.invoiceConfirmGrid).length > 0) {
            ics040_var.objInvoiceConfirmGrid = $(ics040_var.controls.invoiceConfirmGrid).InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Income/ICS040_InitialInvoiceConfirmGrid");
        }
    },

    BidingEvent: function () {
        $(ics040_var.controls.btnRetrieve).click(ics040_process.CmdRetrieve_OnClick);
        $(ics040_var.controls.btnAdd).click(ics040_process.CmdAdd_OnClick);
        $(ics040_var.controls.btnCancel).click(ics040_process.CmdCancel_OnClick);


        //Grid event
        SpecialGridControl(ics040_var.objInvoiceGrid, ["ViewInvoiceDetail", "Remove"]);
        BindOnLoadedEvent(ics040_var.objInvoiceGrid, function () {
            var colInx = ics040_var.objInvoiceGrid.getColIndexById('Remove');

            for (var i = 0; i < ics040_var.objInvoiceGrid.getRowsNum(); i++) {
                var rowId = ics040_var.objInvoiceGrid.getRowId(i);
                GenerateDetailButton(ics040_var.objInvoiceGrid, "btnViewInvoiceDetail", rowId, "ViewInvoiceDetail", true);
                GenerateRemoveButton(ics040_var.objInvoiceGrid, "btnRemove", rowId, "Remove", true);

                BindGridButtonClickEvent("btnViewInvoiceDetail", rowId, ics040_process.ViewInvoiceDetail);
                BindGridButtonClickEvent("btnRemove", rowId, ics040_process.RemoveInvoice);
            }
            ics040_var.objInvoiceGrid.setSizes();
        });

        SpecialGridControl(ics040_var.objInvoiceConfirmGrid, ["ViewInvoiceDetail"]);
        BindOnLoadedEvent(ics040_var.objInvoiceConfirmGrid, function () {
            
            for (var i = 0; i < ics040_var.objInvoiceConfirmGrid.getRowsNum(); i++) {
                var rowId = ics040_var.objInvoiceConfirmGrid.getRowId(i);
                GenerateDetailButton(ics040_var.objInvoiceConfirmGrid, "btnViewInvoiceDetail", rowId, "ViewInvoiceDetail", true);

                BindGridButtonClickEvent("btnViewInvoiceDetail", rowId, ics040_process.ViewInvoiceDetailConfirm);
            }
            ics040_var.objInvoiceConfirmGrid.setSizes();
        });
    },

    ViewInvoiceDetail: function (rowId) {
        var obj083 = {
            BillingTargetCode: ics040_var.objInvoiceGrid.cells(rowId, ics040_var.objInvoiceGrid.getColIndexById("BillingTargetCode")).getValue(),
            InvoiceNo: ics040_var.objInvoiceGrid.cells(rowId, ics040_var.objInvoiceGrid.getColIndexById("InvoiceNo")).getValue(),
            InvoiceOCC: ics040_var.objInvoiceGrid.cells(rowId, ics040_var.objInvoiceGrid.getColIndexById("InvoiceOCC")).getValue()
        };
        $("#ics040dlgBox").OpenICS083Dialog("ICS040", obj083);
    },

    ViewInvoiceDetailConfirm: function (rowId) {
        var obj083 = {
            BillingTargetCode: ics040_var.objInvoiceConfirmGrid.cells(rowId, ics040_var.objInvoiceConfirmGrid.getColIndexById("BillingTargetCode")).getValue(),
            InvoiceNo: ics040_var.objInvoiceConfirmGrid.cells(rowId, ics040_var.objInvoiceConfirmGrid.getColIndexById("InvoiceNo")).getValue(),
            InvoiceOCC: ics040_var.objInvoiceConfirmGrid.cells(rowId, ics040_var.objInvoiceConfirmGrid.getColIndexById("InvoiceOCC")).getValue()
        };
        $("#ics040dlgBox").OpenICS083Dialog("ICS040", obj083);
    },

    RemoveInvoice: function (rowId) {
        obj = { rowIndex: ics040_var.objInvoiceGrid.getRowIndex(rowId) };

        $(ics040_var.controls.invoiceGrid).LoadDataToGrid(ics040_var.objInvoiceGrid, 0, false, "/Income/ICS040_RemoveInvoice", obj, "doInvoice", false
        , function (result, controls, isWarning) {
            if (controls != undefined) {
                VaridateCtrl(controls, controls);
            }
        }
        , function () {});
    },

    SetScreenMode: function (mode) {
        if (mode == ics040_var.screenMode.registerMode) {
            $(ics040_var.controls.DivInputInvoice).show();
            $(ics040_var.controls.DivInvoiceGrid).show();
            $(ics040_var.controls.DivInvoiceConfirmGrid).hide();

            $(ics040_var.controls.invoiceNo).SetDisabled(false);
            $(ics040_var.controls.btnRetrieve).SetDisabled(false);
            $(ics040_var.controls.btnAdd).SetDisabled(true);
            $(ics040_var.controls.btnCancel).SetDisabled(true);

            register_command.SetCommand(ics040_process.CmdRegister_OnClick);
            reset_command.SetCommand(ics040_process.CmdReset_OnClick);
            confirm_command.SetCommand(null);
            back_command.SetCommand(null);
        }
        else if (mode == ics040_var.screenMode.confirmMode) {
            $(ics040_var.controls.DivInputInvoice).hide();
            $(ics040_var.controls.DivInvoiceGrid).hide();
            $(ics040_var.controls.DivInvoiceConfirmGrid).show();
            
            register_command.SetCommand(null);
            reset_command.SetCommand(null);
            confirm_command.SetCommand(ics040_process.CmdConfirm_OnClick);
            back_command.SetCommand(ics040_process.CmdBack_OnClick);
        }
    },


    CmdRetrieve_OnClick: function () {
        //Retrive invoice information and display on screen
        var obj = {
            InvoiceNo: $(ics040_var.controls.invoiceNo).val()
        };

        ajax_method.CallScreenController("/Income/ICS040_GetInvoice", obj, function (result, controls) {
            if (result != undefined) {
                //Success
                $(ics040_var.controls.billingClientNameENG).val(result.BillingClientNameEN);
                $(ics040_var.controls.billingClientNameLOCAL).val(result.BillingClientNameLC);
                $(ics040_var.controls.issueInvoiceDate).val(result.IssueInvDateDisplay);
                $(ics040_var.controls.billingAmount).val((new Number(result.BillingAmountIncVATGridDisplay)).numberFormat("#,##0.00"));
                $(ics040_var.controls.billingAmount).NumericCurrency().val(result.InvoiceAmountCurrencyType);
                $(ics040_var.controls.invoiceDetailQTY).val((new Number(result.InvoiceDetailQtyGridDisplay)).numberFormat("#,##0"));
                $(ics040_var.controls.approveNo).val("");

                $(ics040_var.controls.invoiceNo).SetDisabled(true);
                $(ics040_var.controls.btnRetrieve).SetDisabled(true);
                $(ics040_var.controls.btnAdd).SetDisabled(false);
                $(ics040_var.controls.btnCancel).SetDisabled(false);
            }
            else if (controls != undefined) {
                VaridateCtrl(controls, controls);
                $(ics040_var.controls.DivInvoiceDetail).clearForm();
            }
        });
    },

    CmdAdd_OnClick: function () {
        //Add invoice into invoice list and display 
        var obj = {
            InvoiceNo: $(ics040_var.controls.invoiceNo).val(),
            ApproveNo: $(ics040_var.controls.approveNo).val()
        };

        $(ics040_var.controls.invoiceGrid).LoadDataToGrid(ics040_var.objInvoiceGrid, 0, false, "/Income/ICS040_AddInvoice", obj, "doInvoice", false
            , function (result, controls, isWarnig) {
                if (result != undefined) {
                    $(ics040_var.controls.DivInputInvoice).clearForm();
                    $(ics040_var.controls.invoiceNo).SetDisabled(false);

                    $(ics040_var.controls.btnRetrieve).SetDisabled(false);
                    $(ics040_var.controls.btnAdd).SetDisabled(true);
                    $(ics040_var.controls.btnCancel).SetDisabled(true);
                }
                else if (controls != undefined) {
                    VaridateCtrl(controls, controls);
                }
            }
            , function () {
            });
    },

    CmdCancel_OnClick: function () {
        //Clear inputed invoice 
        $(ics040_var.controls.DivInputInvoice).clearForm();
        $(ics040_var.controls.invoiceNo).SetDisabled(false);
        $(ics040_var.controls.btnRetrieve).SetDisabled(false);
        $(ics040_var.controls.btnAdd).SetDisabled(true);
        $(ics040_var.controls.btnCancel).SetDisabled(true);
    },

    CmdRegister_OnClick: function () {
        ajax_method.CallScreenController("/Income/ICS040_cmdRegister", null, function (result, controls, isWarning) {
            if (result != undefined) {
                $(ics040_var.controls.invoiceConfirmGrid).LoadDataToGrid(ics040_var.objInvoiceConfirmGrid, 0, false, "/Income/ICS040_GetInvoiceForConfirm", "", "doInvoice", false
                    , function () {
                        //Goto confirm state
                        ics040_process.SetScreenMode(ics040_var.screenMode.confirmMode);
                    }
                    , function (result, controls, isWarning) {
                    });
            }
            else if (controls != undefined) {
                VaridateCtrl(controls, controls);
            }
        });
    },

    CmdConfirm_OnClick: function () {
        ajax_method.CallScreenController("/Income/ICS040_cmdConfirm", null,
        function (result, controls, isWarning) {
            if (result != undefined) {
                // Success
                var objMsg = { module: "Income", code: "MSG7008"};
                call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                    OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {
                        $(ics040_var.controls.DivInputInvoice).clearForm();
                        DeleteAllRow(ics040_var.objInvoiceGrid);
                        ics040_process.SetScreenMode(ics040_var.screenMode.registerMode);
                    });
                });
            }
            else if (controls != undefined) {
                //ics040_process.SetScreenMode(ics040_var.screenMode.registerMode);
                VaridateCtrl(controls, controls);
            }
        });
    },

    CmdBack_OnClick: function () {
        ics040_process.SetScreenMode(ics040_var.screenMode.registerMode);
    },

    CmdReset_OnClick: function () {
        //Reset all and get back as initial screen state
        $(ics040_var.controls.invoiceGrid).LoadDataToGrid(ics040_var.objInvoiceGrid, 0, false, "/Income/ICS040_cmdReset", "", "doInvoice", false
            , function () {
                $(ics040_var.controls.DivInputInvoice).clearForm();
                ics040_process.SetScreenMode(ics040_var.screenMode.registerMode);
            }
            , null);
    }
};




$(document).ready(function () {
    ics040_process.InitialScreen();
});