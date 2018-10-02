
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

/// <reference path="../Base/object/ajax_method.js" />
/// <reference path="../Base/object/command_event.js" />
/// <reference path="../Base/object/master_event.js" />

var grdByBillingDetailGrid;
var grdByInvoiceDetailGrid;
var isEditMode = true;

$(document).ready(function () {

    // tt
    GridControl.DisableSelectionHighlight();

    // Initial grid 1
    if ($.find("#BLS060_ByBillingDetailGrid").length > 0) {

        grdByBillingDetailGrid = $("#BLS060_ByBillingDetailGrid").InitialGrid(999, false, "/Billing/BLS060_InitialByBillingDetailGrid", function () {

            SpecialGridControl(grdByBillingDetailGrid, ["ContractCode", "BillingOCC", "RunningNo", "IssuedateAutoTransferDate", "RealTimeIssue", "Del"]);

            BindOnLoadedEvent(grdByBillingDetailGrid, function () {
                for (var i = 0; i < grdByBillingDetailGrid.getRowsNum(); i++) {
                    // --
                    var rowId = grdByBillingDetailGrid.getRowId(i);

                    var clt_ContractCode = "#" + GenerateGridControlID("txtContractCode", rowId);
                    $(clt_ContractCode).unbind("focus");

                    var clt_BillingOCC = "#" + GenerateGridControlID("txtBillingOCC", rowId);
                    $(clt_BillingOCC).unbind("focus");

                    var clt_RunningNo = "#" + GenerateGridControlID("txtRunningNo", rowId);
                    $(clt_RunningNo).unbind("focus");
                }

                // --

                if (isEditMode == true) {
                    var row_idx = grdByBillingDetailGrid.getRowsNum() - 1;
                    var rowId = grdByBillingDetailGrid.getRowId(row_idx);


                    var defStringVal = "";
                    var defNumVal = 0;
                    var defDateVal = "";

                    // ContractCode
                    GenerateTextBox(grdByBillingDetailGrid, "txtContractCode", rowId, "ContractCode", defStringVal, true);
                    var clt_ContractCode = "#" + GenerateGridControlID("txtContractCode", rowId);
                    $(clt_ContractCode).attr("maxlength", 9);
                    BindFocusEvent(clt_ContractCode, Add_ByBillingBlankLine);


                    // BillingOCC
                    GenerateTextBox(grdByBillingDetailGrid, "txtBillingOCC", rowId, "BillingOCC", defStringVal, true);
                    var clt_BillingOCC = "#" + GenerateGridControlID("txtBillingOCC", rowId);
                    $(clt_BillingOCC).attr("maxlength", 2);
                    //BindFocusEvent(clt_BillingOCC, Add_ByBillingBlankLine);


                    // RunningNo
                    GenerateTextBox(grdByBillingDetailGrid, "txtRunningNo", rowId, "RunningNo", "", true);
                    var clt_RunningNo = "#" + GenerateGridControlID("txtRunningNo", rowId);
                    //BindFocusEvent(clt_RunningNo, Add_ByBillingBlankLine);
                    InitialNumericInputTextBox([GenerateGridControlID("txtRunningNo", rowId)], false);
                    $(clt_RunningNo).attr("maxlength", "4");
                    $(clt_RunningNo).css("text-align", "right");

                    // IssueDate
                    if (verModeRadio1 == conModeRadio1rdoStopAutoTransferCreditCard) {
                        GenerateGridDateTimePicker(grdByBillingDetailGrid, rowId, "dtpIssueDate", "IssuedateAutoTransferDate", defDateVal, false);
                    } else {
                        GenerateGridDateTimePicker(grdByBillingDetailGrid, rowId, "dtpIssueDate", "IssuedateAutoTransferDate", defDateVal, true);
                    }

                    // RealTimeIssue
                    var checkboxColInx = grdByBillingDetailGrid.getColIndexById("RealTimeIssue");
                    grdByBillingDetailGrid.cells2(row_idx, checkboxColInx).setValue(GenerateCheckBox("chkRealTimeIssue", rowId, "", true));

                    // Remove
                    GenerateRemoveButton(grdByBillingDetailGrid, "btnRemove", rowId, "Del", true);
                    BindGridButtonClickEvent("btnRemove", rowId, btnRemoveByBillingDetailGrid);
                }

                grdByBillingDetailGrid.setSizes();

            });

        });

    }

    // Initial grid 2
    if ($.find("#BLS060_ByInvoiceGrid").length > 0) {
        grdByInvoiceDetailGrid = $("#BLS060_ByInvoiceGrid").InitialGrid(999, false, "/Billing/BLS060_InitialByInvoiceGrid", function () {

            SpecialGridControl(grdByInvoiceDetailGrid, ["InvoiceNo", "AutoTransferDate", "RealTimeIssue", "Del"]);

            BindOnLoadedEvent(grdByInvoiceDetailGrid, function () {
                for (var i = 0; i < grdByInvoiceDetailGrid.getRowsNum(); i++) {

                    // --
                    var rowId = grdByInvoiceDetailGrid.getRowId(i);

                    var clt_InvoiceNo = "#" + GenerateGridControlID("txtInvoiceNo", rowId);
                    $(clt_InvoiceNo).unbind("focus");

                }

                // --
                if (isEditMode == true) {
                    var row_idx = grdByInvoiceDetailGrid.getRowsNum() - 1;
                    var rowId = grdByInvoiceDetailGrid.getRowId(row_idx);

                    var defStringVal = "";
                    var defNumVal = 0;
                    var defDateVal = "";

                    // InvoiceNo
                    GenerateTextBox(grdByInvoiceDetailGrid, "txtInvoiceNo", rowId, "InvoiceNo", defStringVal, true);
                    var clt_InvoiceNo = "#" + GenerateGridControlID("txtInvoiceNo", rowId);
                    $(clt_InvoiceNo).attr("maxlength", 12);
                    BindFocusEvent(clt_InvoiceNo, Add_ByInvoiceBlankLine);

                    // AutoTransferDate
                    if (verModeRadio1 == conModeRadio1rdoStopAutoTransferCreditCard) {
                        GenerateGridDateTimePicker(grdByInvoiceDetailGrid, rowId, "dtpAutoTransferDate", "AutoTransferDate", defDateVal, false);
                    } else {
                        GenerateGridDateTimePicker(grdByInvoiceDetailGrid, rowId, "dtpAutoTransferDate", "AutoTransferDate", defDateVal, true);
                    }

                    // RealTimeIssue
                    var checkboxColInx = grdByInvoiceDetailGrid.getColIndexById("RealTimeIssue");
                    grdByInvoiceDetailGrid.cells2(row_idx, checkboxColInx).setValue(GenerateCheckBox("chkRealTimeIssue", rowId, "", true));


                    // Remove
                    GenerateRemoveButton(grdByInvoiceDetailGrid, "btnRemove", rowId, "Del", true);
                    BindGridButtonClickEvent("btnRemove", rowId, btnRemoveByInvoiceDetailGrid);
                }

                grdByInvoiceDetailGrid.setSizes();


            });

        });
    }

    //Init Object Event
    $("#btnSelectProcess").click(btn_Select_Process_click);

    $("#rdoSwitchToAutoTransferCreditCard").change(rdoSwitchToAutoTransferCreditCard_Select);
    $("#rdoStopAutoTransferCreditCard").change(rdoStopAutoTransferCreditCard_Select);
    $("#rdoChangeExpectedIssueDateAutoTransferDate").change(rdoChangeExpectedIssueDateAutoTransferDate_Select);

    $("#rdoByBillingDetails").change(rdoByBillingDetails_Select);
    $("#rdoByInvoice").change(rdoByInvoice_Select);

    //Initial Page
    InitialPage();


});


// Grid Function
function Add_ByBillingBlankLine(clt) {

    CheckFirstRowIsEmpty(grdByBillingDetailGrid, true);
    var row = ["", "", "", "", "", "", "", ""];
    AddNewRow(grdByBillingDetailGrid, row);

    if (grdByBillingDetailGrid.getRowsNum() > 1) {
        if (clt == undefined) {
            var rowId = grdByBillingDetailGrid.getRowId(0);
            var clt_ContractCode = "#" + GenerateGridControlID("txtContractCode", rowId);
            $(clt_ContractCode).focus();
        }
        else {

            $(clt).focus();
        }
    }

}


function Add_ByInvoiceBlankLine(clt) {

    CheckFirstRowIsEmpty(grdByInvoiceDetailGrid, true);
    var row = ["", "", "", "", "", ""];
    AddNewRow(grdByInvoiceDetailGrid, row);

    if (grdByInvoiceDetailGrid.getRowsNum() > 1) {
        if (clt == undefined) {
            var rowId = grdByInvoiceDetailGrid.getRowId(0);
            var clt_InvoiceNo = "#" + GenerateGridControlID("txtInvoiceNo", rowId);
            $(clt_InvoiceNo).focus();
        }
        else {
            $(clt).focus();
        }
    }

}

function BindFocusEvent(clt, func) {
    $(clt).focus(function () {
        if (typeof (func) == "function") {
            func(clt);
        }
    });
}

function btnRemoveByBillingDetailGrid(rowId) {

    if (!(bolViewMode)) {
        if (grdByBillingDetailGrid.getRowsNum() > 1) {
            DeleteRow(grdByBillingDetailGrid, rowId);
        }
    }
}

function btnRemoveByInvoiceDetailGrid(rowId) {
    if (!(bolViewMode)) {
        if (grdByInvoiceDetailGrid.getRowsNum() > 1) {
            DeleteRow(grdByInvoiceDetailGrid, rowId);
        }
    }
}



function InitialPage() {

    $("#dtpIssueDate").InitialDate();
    $("#dtpAutoTransferDate").InitialDate();

    setVisableTable(0);
    setFormMode(conModeInit);

}
// Form Mode Section

var conModeInit = 0;
var conModeView = 1;
var conModeEdit = 2;
var conModeConfirm = 9;

var conModeRadio1rdoSwitchToAutoTransferCreditCard = 1;
var conModeRadio1rdoStopAutoTransferCreditCard = 2;
var conModeRadio1rdoChangeExpectedIssueDateAutoTransferDate = 3;

var conModeRadio2rdoByBillingDetails = 1;
var conModeRadio2rdoByInvoice = 2;

var conNo = 0;
var conYes = 1;

var verModeRadio1 = 1;
var verModeRadio2 = 1;

var bolViewMode = false;

function setFormMode(intMode) {
    // ModeInit
    if (intMode == conModeInit) {
        register_command.SetCommand(null);
        reset_command.SetCommand(null);

        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }

    // ModeView = 1;

    if (intMode == conModeView) {
        register_command.SetCommand(btn_Register_click);
        reset_command.SetCommand(btn_Reset_click);

        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }

    // ModeEdit = 2;

    if (intMode == conModeEdit) {
        register_command.SetCommand(null);
        reset_command.SetCommand(null);

        confirm_command.SetCommand(btn_Confirm_click);
        back_command.SetCommand(btn_Back_click);
    }

    // ModeConfirm = 9;

    if (intMode == conModeConfirm) {
        register_command.SetCommand(null);
        reset_command.SetCommand(null);

        confirm_command.SetCommand(btn_Confirm_click);
        back_command.SetCommand(btn_Back_click);
    }
}


// Mode Event
function btn_Register_click() {
    command_control.CommandControlMode(false); //Add by Jutarat A. on 25122013

    // check all input on Server
    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Billing/BLS060_Register", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            if (result != "0") {

                // tt
                isEditMode = false;

                // goto confirm state
                if (result.Detail1 != undefined) {
                    // don't swap sequence
                    for (var i = 0; i < grdByBillingDetailGrid.getRowsNum(); i++) {
                        if (result.Detail1[i].dtpIssueDate != undefined) {

                            var row_id = grdByBillingDetailGrid.getRowId(i);
                            var clt1 = "#" + GenerateGridControlID("dtpIssueDate", row_id);

                            $(clt1).val(ConvertDateToTextFormat(ConvertDateObject(result.Detail1[i].dtpIssueDate)));
                        }
                    }
                }

                if (result.Detail2 != undefined) {
                    for (var i = 0; i < grdByInvoiceDetailGrid.getRowsNum(); i++) {
                        if (result.Detail2[i].dtpAutoTransferDate != undefined) {

                            var row_id = grdByInvoiceDetailGrid.getRowId(i);
                            var clt1 = "#" + GenerateGridControlID("dtpAutoTransferDate", row_id);

                            $(clt1).val(ConvertDateToTextFormat(ConvertDateObject(result.Detail2[i].dtpAutoTransferDate)));
                        }
                    }
                }
                setFormMode(conModeEdit);

                bolViewMode = true;
                $("#divSpecifyProcessType").SetViewMode(true);
                $("#divByBillingDetail").SetViewMode(true);
                $("#divByInvoice").SetViewMode(true);

                // Hide delete button in grid
                var colBtnRemoveIdx1 = grdByBillingDetailGrid.getColIndexById("Del");
                grdByBillingDetailGrid.setColumnHidden(colBtnRemoveIdx1, true);

                var colBtnRemoveIdx2 = grdByInvoiceDetailGrid.getColIndexById("Del");
                grdByInvoiceDetailGrid.setColumnHidden(colBtnRemoveIdx2, true);

                // delete blank row
                if (CheckFirstRowIsEmpty(grdByBillingDetailGrid) == false) {
                    for (var i = 0; i < grdByBillingDetailGrid.getRowsNum(); i++) {
                        var row_id = grdByBillingDetailGrid.getRowId(i);
                        var clt1 = "#" + GenerateGridControlID("txtContractCode", row_id);

                        if ($(clt1).val() == '') {
                            DeleteRow(grdByBillingDetailGrid, row_id);
                            i = i - 1;
                        }
                    }
                }

                if (CheckFirstRowIsEmpty(grdByInvoiceDetailGrid) == false) {
                    for (var i = 0; i < grdByInvoiceDetailGrid.getRowsNum(); i++) {
                        var row_id = grdByInvoiceDetailGrid.getRowId(i);
                        var clt1 = "#" + GenerateGridControlID("txtInvoiceNo", row_id);

                        if ($(clt1).val() == '') {
                            DeleteRow(grdByInvoiceDetailGrid, row_id);
                            i = i - 1;
                        }

                    }
                }


            }
            else {
                VaridateCtrl(controls, controls);
            }
        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }

        command_control.CommandControlMode(true); //Add by Jutarat A. on 25122013
    });
}
// create all send to server data for check mendatory and save (in case all input data is ok)
function GetUserAdjustData() {

    var arr1 = new Array();
    var arr2 = new Array();

    var header = {
        rdoProcessType: verModeRadio1,
        rdoProcessUnit: verModeRadio2
    };

    if (CheckFirstRowIsEmpty(grdByBillingDetailGrid) == false) {

        for (var i = 0; i < grdByBillingDetailGrid.getRowsNum(); i++) {

            var row_id = grdByBillingDetailGrid.getRowId(i);
            // custom object in grid use object name
            var clt1 = "#" + GenerateGridControlID("txtContractCode", row_id);
            var clt2 = "#" + GenerateGridControlID("txtBillingOCC", row_id);
            var clt3 = "#" + GenerateGridControlID("txtRunningNo", row_id);
            var clt4 = "#" + GenerateGridControlID("dtpIssueDate", row_id);
            var clt5 = "#" + GenerateGridControlID("chkRealTimeIssue", row_id);
            // non custom object in grid use object name
            // use column name in XML not declare ass txt, dtp ect

            var obj1 = {

                txtContractCode: $(clt1).val(),
                txtBillingOCC: $(clt2).val(),
                txtRunningNo: $(clt3).val(),
                dtpIssueDate: $(clt4).val(),
                chkRealTimeIssue: $(clt5).prop('checked'),

                txtContractCodeID: GenerateGridControlID("txtContractCode", row_id),
                txtBillingOCCID: GenerateGridControlID("txtBillingOCC", row_id),
                txtRunningNoID: GenerateGridControlID("txtRunningNo", row_id),
                dtpIssueDateID: GenerateGridControlID("dtpIssueDate", row_id)

                // example non clustom grid getvalue
                //strBillingamount: grdByBillingDetailGrid.cells
                //(row_id, grdCancelBillingDetailGrid.getColIndexById("Billingamount")).getValue(),
            };
            arr1.push(obj1);
        }
    }


    var detail1 = arr1;

    if (CheckFirstRowIsEmpty(grdByInvoiceDetailGrid) == false) {
        for (var i = 0; i < grdByInvoiceDetailGrid.getRowsNum(); i++) {

            var row_id = grdByInvoiceDetailGrid.getRowId(i);
            var clt1 = "#" + GenerateGridControlID("txtInvoiceNo", row_id);
            var clt2 = "#" + GenerateGridControlID("dtpAutoTransferDate", row_id);
            var clt3 = "#" + GenerateGridControlID("chkRealTimeIssue", row_id);

            var obj2 = {
                txtInvoiceNo: $(clt1).val(),
                dtpAutoTransferDate: $(clt2).val(),
                chkRealTimeIssue: $(clt3).prop('checked'),

                txtInvoiceNoID: GenerateGridControlID("txtInvoiceNo", row_id),
                dtpAutoTransferDateID: GenerateGridControlID("dtpAutoTransferDate", row_id)

            };

            arr2.push(obj2);
        }
    }

    var detail2 = arr2;

    var returnObj = {
        Header: header,
        Detail1: detail1,
        Detail2: detail2
    };

    return returnObj;

}


function btn_Reset_click() {



    //    /* --- Get Message --- */
    //    var obj = {
    //        module: "Common",
    //        code: "MSG0038"
    //    };
    //    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
    //        OpenOkCancelDialog(result.Code, result.Message,
    //        function () {

    //            setVisableTable(0);
    //            setFormMode(conModeInit);
    //            bolViewMode = false;
    //            $("#divSpecifyProcessType").SetViewMode(false);
    //            $("#divByBillingDetail").SetViewMode(false);
    //            $("#divByInvoice").SetViewMode(false);

    //            $("#divSpecifyProcessType").ResetToNormalControl(true);
    //            $("#divByBillingDetail").ResetToNormalControl(true);
    //            $("#divByInvoice").ResetToNormalControl(true);

    //            ClearScreenToInitStage();
    //        },
    //        null);
    //    });


    setVisableTable(0);
    setFormMode(conModeInit);
    bolViewMode = false;
    $("#divSpecifyProcessType").SetViewMode(false);
    $("#divByBillingDetail").SetViewMode(false);
    $("#divByInvoice").SetViewMode(false);

    $("#divSpecifyProcessType").ResetToNormalControl(true);
    $("#divByBillingDetail").ResetToNormalControl(true);
    $("#divByInvoice").ResetToNormalControl(true);

    ClearScreenToInitStage();


}
function btn_Approve_click() {
}
function btn_Reject_click() {
}
function btn_Return_click() {
}
function btn_Close_click() {
}
function btn_Confirm_click() {
    command_control.CommandControlMode(false); //Add by Jutarat A. on 25122013

    // save data
    ajax_method.CallScreenController("/Billing/BLS060_Confirm", "",
        function (result, controls, isWarning) {
            if (result != undefined) {
                if (result != "0") {



                    // goto confirm state
                    setVisableTable(0);
                    setFormMode(conModeInit);
                    bolViewMode = false;
                    $("#divSpecifyProcessType").SetViewMode(false);
                    $("#divByBillingDetail").SetViewMode(false);
                    $("#divByInvoice").SetViewMode(false);

                    $("#divSpecifyProcessType").ResetToNormalControl(true);
                    $("#divByBillingDetail").ResetToNormalControl(true);
                    $("#divByInvoice").ResetToNormalControl(true);

                    // Show delete button in grid
                    var colBtnRemoveIdx1 = grdByBillingDetailGrid.getColIndexById("Del");
                    grdByBillingDetailGrid.setColumnHidden(colBtnRemoveIdx1, false);
                    // Concept by P'Leing
                    SetFitColumnForBackAction(grdByBillingDetailGrid, "TempSpan");

                    // Show delete button in grid
                    var colBtnRemoveIdx2 = grdByInvoiceDetailGrid.getColIndexById("Del");
                    grdByInvoiceDetailGrid.setColumnHidden(colBtnRemoveIdx2, false);
                    // Concept by P'Leing
                    SetFitColumnForBackAction(grdByInvoiceDetailGrid, "TempSpan");

                    ClearScreenToInitStage();



                    // Success
                    var objMsg = {
                        module: "Common",
                        code: "MSG0046"
                    };
                    call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                        OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {

                            if (result.strFilePath != '') {
                                if (result.strFilePath != undefined) {
                                    var key = ajax_method.GetKeyURL(null);
                                    var url = ajax_method.GenerateURL("/Billing/BLS060_GetInvoiceReport?k=" + key + "&fileName=" + result.strFilePath);
                                    window.open(url, "", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
                                }
                            }

                        });
                    });

                }
            }
            else {
                VaridateCtrl(controls, controls);
            }

            command_control.CommandControlMode(true); //Add by Jutarat A. on 25122013
        });

}

function btn_Back_click() {

    // tt
    isEditMode = true;

    setFormMode(conModeView);
    bolViewMode = false;
    $("#divSpecifyProcessType").SetViewMode(false);
    $("#divByBillingDetail").SetViewMode(false);
    $("#divByInvoice").SetViewMode(false);

    $("#divSpecifyProcessType").ResetToNormalControl(true);
    $("#divByBillingDetail").ResetToNormalControl(true);
    $("#divByInvoice").ResetToNormalControl(true);

    $("#rdoSwitchToAutoTransferCreditCard").attr("disabled", true);
    $("#rdoStopAutoTransferCreditCard").attr("disabled", true);
    $("#rdoChangeExpectedIssueDateAutoTransferDate").attr("disabled", true);

    $("#rdoByBillingDetails").attr("disabled", true);
    $("#rdoByInvoice").attr("disabled", true);

    // Show delete button in grid
    var colBtnRemoveIdx1 = grdByBillingDetailGrid.getColIndexById("Del");
    grdByBillingDetailGrid.setColumnHidden(colBtnRemoveIdx1, false);
    // Concept by P'Leing
    SetFitColumnForBackAction(grdByBillingDetailGrid, "TempSpan");

    // Show delete button in grid
    var colBtnRemoveIdx2 = grdByInvoiceDetailGrid.getColIndexById("Del");
    grdByInvoiceDetailGrid.setColumnHidden(colBtnRemoveIdx2, false);
    // Concept by P'Leing
    SetFitColumnForBackAction(grdByInvoiceDetailGrid, "TempSpan");

    // restore blank row incase input < 3 row
    if (CheckFirstRowIsEmpty(grdByBillingDetailGrid) == false) {
        if (grdByBillingDetailGrid.getRowsNum() < 3) {
            var tempgrdByBillingDetailGridgetRowsNum = grdByBillingDetailGrid.getRowsNum()
            for (var i = 0; i < 3 - tempgrdByBillingDetailGridgetRowsNum; i++) {
                Add_ByBillingBlankLine();
            }
        }
    }

    if (CheckFirstRowIsEmpty(grdByInvoiceDetailGrid) == false) {
        if (grdByInvoiceDetailGrid.getRowsNum() < 3) {
            var tempgrdByInvoiceDetailGridgetRowsNum = grdByInvoiceDetailGrid.getRowsNum()
            for (var i = 0; i < 3 - tempgrdByInvoiceDetailGridgetRowsNum; i++) {
                Add_ByInvoiceBlankLine();
            }
        }
    }

}

// Clear Screen
function ClearScreenToInitStage() {

    // tt
    isEditMode = true;

    $("#rdoSwitchToAutoTransferCreditCard").attr("disabled", false);
    $("#rdoStopAutoTransferCreditCard").attr("disabled", false);
    $("#rdoChangeExpectedIssueDateAutoTransferDate").attr("disabled", false);

    $("#rdoByBillingDetails").attr("disabled", false);
    $("#rdoByInvoice").attr("disabled", false);

    //$("#rdoSwitchToAutoTransferCreditCard").attr("checked", true);
    //$("#rdoStopAutoTransferCreditCard").attr("checked", true);
    $("#rdoChangeExpectedIssueDateAutoTransferDate").attr("checked", true);
    1
    $("#rdoByBillingDetails").attr("checked", true);
    //$("#rdoByInvoice").attr("checked", true);

    $("#btnSelectProcess").attr("disabled", false);

    DeleteAllRow(grdByBillingDetailGrid);
    DeleteAllRow(grdByInvoiceDetailGrid);

    verModeRadio1 = conModeRadio1rdoSwitchToAutoTransferCreditCard;
    verModeRadio2 = conModeRadio2rdoByBillingDetails;

    bolAlreadyAddNewRowToGRID1 = false;
    bolAlreadyAddNewRowToGRID2 = false;
}
// Enable Obj On Screen

// Visable Obj On Screen
function setVisableTable(intMode) {

    if (intMode == conModeRadio2rdoByBillingDetails) {
        $("#divSpecifyProcessType").show();
        $("#divByBillingDetail").show();
        $("#divByInvoice").hide();
    }
    else if (intMode == conModeRadio2rdoByInvoice) {
        $("#divSpecifyProcessType").show();
        $("#divByBillingDetail").hide();
        $("#divByInvoice").show();
    }
    else {
        $("#divSpecifyProcessType").show();
        $("#divByBillingDetail").hide();
        $("#divByInvoice").hide();
    };


}

function setVisableforChangeExpected(intMode) {

    if (intMode == conYes) {
        $("#rdoByBillingDetails").attr("disabled", false);
        $("#rdoByInvoice").attr("disabled", true);
    }
    else {
        $("#rdoByBillingDetails").attr("disabled", false);
        $("#rdoByInvoice").attr("disabled", false);
    }
}

function setVisableforStopAuto(intMode) {

    if (intMode == conYes) {

        colInx = grdSearchResult.getColIndexById("IssuedateAutoTransferDate");
        grdSearchResult.setColumnHidden(colInx, true);
        colInx = grdCheckingDetail.getColIndexById("AutoTransferDate")
        grdCheckingDetail.setColumnHidden(colInx, true);
    }
    else {
        colInx = grdSearchResult.getColIndexById("IssuedateAutoTransferDate");
        grdSearchResult.setColumnHidden(colInx, false);
        colInx = grdCheckingDetail.getColIndexById("AutoTransferDate")
        grdCheckingDetail.setColumnHidden(colInx, false);
    }
}

function setVisableforByInvoice(intMode) {

    if (intMode == conYes) {
        $("#rdoSwitchToAutoTransferCreditCard").attr("disabled", false);
        $("#rdoStopAutoTransferCreditCard").attr("disabled", false);
        $("#rdoChangeExpectedIssueDateAutoTransferDate").attr("disabled", true);
    }
    else {
        $("#rdoSwitchToAutoTransferCreditCard").attr("disabled", false);
        $("#rdoStopAutoTransferCreditCard").attr("disabled", false);
        $("#rdoChangeExpectedIssueDateAutoTransferDate").attr("disabled", false);
    }
}

var bolAlreadyAddNewRowToGRID1 = false;
var bolAlreadyAddNewRowToGRID2 = false;

function btn_Select_Process_click() {

    setFormMode(conModeView);
    setVisableTable(verModeRadio2);

    if (verModeRadio2 == conModeRadio2rdoByBillingDetails) {

        //Set column lable for auto transfer date
        if (verModeRadio1 == conModeRadio1rdoSwitchToAutoTransferCreditCard) {
            //Auto transfer date
            grdByBillingDetailGrid.setColumnLabel(4, htmlDecode(BLS060_ViewBag.lblHeader2AutoTransferDate));
        }
        else {
            //Issue date/auto transfer date
            grdByBillingDetailGrid.setColumnLabel(4, htmlDecode(BLS060_ViewBag.lblHeader2IssuedateAutoTransferDate));
        }


        if (!(bolAlreadyAddNewRowToGRID1)) {

            Add_ByBillingBlankLine();
            Add_ByBillingBlankLine();
            Add_ByBillingBlankLine();
            bolAlreadyAddNewRowToGRID1 = true;
        }

    } else {
        if (!(bolAlreadyAddNewRowToGRID2)) {

            Add_ByInvoiceBlankLine();
            Add_ByInvoiceBlankLine();
            Add_ByInvoiceBlankLine();
            bolAlreadyAddNewRowToGRID2 = true;
        }

    }

    $("#rdoSwitchToAutoTransferCreditCard").attr("disabled", true);
    $("#rdoStopAutoTransferCreditCard").attr("disabled", true);
    $("#rdoChangeExpectedIssueDateAutoTransferDate").attr("disabled", true);

    $("#rdoByBillingDetails").attr("disabled", true);
    $("#rdoByInvoice").attr("disabled", true);

    $("#btnSelectProcess").attr("disabled", true);
}

// Radio Select
function rdoSwitchToAutoTransferCreditCard_Select() {
    verModeRadio1 = conModeRadio1rdoSwitchToAutoTransferCreditCard;
    setVisableforChangeExpected(conNo);
}

function rdoStopAutoTransferCreditCard_Select() {
    verModeRadio1 = conModeRadio1rdoStopAutoTransferCreditCard;
    setVisableforChangeExpected(conNo);
}

function rdoChangeExpectedIssueDateAutoTransferDate_Select() {
    verModeRadio1 = conModeRadio1rdoChangeExpectedIssueDateAutoTransferDate;
    setVisableforChangeExpected(conYes);
}

function rdoByBillingDetails_Select() {
    verModeRadio2 = conModeRadio2rdoByBillingDetails;
    setVisableforByInvoice(conNo);
}

function rdoByInvoice_Select() {
    verModeRadio2 = conModeRadio2rdoByInvoice;
    setVisableforByInvoice(conYes);
}
