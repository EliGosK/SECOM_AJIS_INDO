
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

var grdManageMoneyCollectionGrid;
var _doReceipt;

// temp wait del
var grdByBillingDetailGrid;
var grdByInvoiceDetailGrid;

$(document).ready(function () {
    // ..

    // Initial grid 1

    if ($.find("#ICS100_ManageMoneyCollection").length > 0) {

        grdManageMoneyCollectionGrid = $("#ICS100_ManageMoneyCollection").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Income/ICS100_InitialManageMoneyCollectionGrid", function () {

            SpecialGridControl(grdManageMoneyCollectionGrid, ["ICS100_ByBillingDetailGrid", "Del"]);
            BindOnLoadedEvent(grdManageMoneyCollectionGrid, function () {

                var colInx = grdManageMoneyCollectionGrid.getColIndexById('Button');

                for (var i = 0; i < grdManageMoneyCollectionGrid.getRowsNum(); i++) {

                    var rowId = grdManageMoneyCollectionGrid.getRowId(i);
                    //-----------------------------------------
                    // Col 8
                    GenerateRemoveButton(grdManageMoneyCollectionGrid, "btnRemove", rowId, "Del", true);
                    BindGridButtonClickEvent("btnRemove", rowId, btnRemoveByManageMoneyCollectionGrid);
                }

                grdManageMoneyCollectionGrid.setSizes();
            });
        });

    }

    //Init Object Event
    // 1 Div Panel Body

    $("#btnRetrieve").click(btn_Retrieve_click);
    $("#btnAdd").click(btn_Add_click);
    $("#btnCancel").click(btn_Cancel_click);

    // example
    // $("#rdoDelete").change(rdoDelete_Select);
    // $("#rdoDelete").attr("disabled", true);

    //Initial Page
    InitialPage();
});

function BindGridObjectClickEvent(id, row_id, row_idx, func) {

    var ctrl = "#" + GenerateGridControlID(id, row_id);

    $(ctrl).unbind("focus");
    $(ctrl).focus(function () {
        if (this.className.indexOf("row-image-disabled") < 0) {
            if (typeof (func) == "function") {
                func(row_idx);
            }
        }
        return false;
    });
}


// Grid Function

function Add_ManageMoneyCollectionBlankLine(doReceipt, cboCollectionAreaCode, cboCollectionAreaSelectText, dtpReceiptDate, txtMemo) {

    CheckFirstRowIsEmpty(grdManageMoneyCollectionGrid, true);

    AddNewRow(grdManageMoneyCollectionGrid, [ doReceipt.ReceiptNo,
                                            ConvertDateToTextFormat(ConvertDateObject(doReceipt.ReceiptDate)),
                                            doReceipt.BillingTargetCodeShort,
                                            doReceipt.BillingTargetCode,
                                            '(1) ' + doReceipt.BillingClientNameEN  + '<BR>(2) ' + doReceipt.BillingClientNameLC,
                                            '(1) ' + doReceipt.BillingClientAddressEN + '<BR>(2) ' + doReceipt.BillingClientAddressLC,
                                            doReceipt.ReceiptAmountCurrencyTypeName + ' ' + doReceipt.ReceiptAmountString,
                                            cboCollectionAreaSelectText,
                                            cboCollectionAreaCode,
                                            dtpReceiptDate,
                                            txtMemo,
                                            "",
                                            "",
                                            doReceipt.ReceiptAmount,
                                            doReceipt.ReceiptAmountCurrencyType
                                             ]);
    grdManageMoneyCollectionGrid.setSizes();

}

function btnRemoveByManageMoneyCollectionGrid(rowId) {
    DeleteRow(grdManageMoneyCollectionGrid, rowId);

    if (CheckFirstRowIsEmpty(grdManageMoneyCollectionGrid) == true) {
        setFormMode(conModeInit);
        bolViewMode = false;
    }

}

function InitialPage() {
    $("#txtReceiptNo").attr("maxlength", 12);

    $("#dtpExpectedCollectDate").InitialDate();
    $("#txtReceiptAmount").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtaMemo").SetMaxLengthTextArea(100);

    $("#btnRetrieve").attr("disabled",false);

    $("#btnAdd").attr("disabled", true);
    $("#btnCancel").attr("disabled", true);
// example
    // Date
    //$("#dtpCustomerAcceptanceDate").InitialDate();
    //InitialDateFromToControl("#dptAdjustBillingPeriodDateFrom", "#dptAdjustBillingPeriodDateTo");
    //Text Input
    //$("#txtSelSeparateFromInvoiceNo").attr("maxlength", 12);
    // Number
    //$("#txtBillingAmount").BindNumericBox(10, 2, 0, 9999999999.99);

    //setVisableTable(0);
    setFormMode(conModeInit);

}
// Form Mode Section

var conModeInit = 0;
var conModeView = 1;
var conModeEdit = 2;
var conModeConfirm = 9;

var conNo = 0;
var conYes = 1;

var bolViewMode = false;

var conModeRadio1rdoSwitchToAutoTransferCreditCard = 1;
var conModeRadio1rdoStopAutoTransferCreditCard = 2;
var conModeRadio1rdoChangeExpectedIssueDateAutoTransferDate = 3;
var conModeRadio2rdoByBillingDetails = 1;
var conModeRadio2rdoByInvoice = 2;
var verModeRadio1 = 1;
var verModeRadio2 = 1;

function setFormMode(intMode) {
    if (intMode == conModeInit) {
        // ModeInit
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        approve_command.SetCommand(null);
        reject_command.SetCommand(null);
        return_command.SetCommand(null);
        close_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }
    else if (intMode == conModeView) {
        // ModeView = 1;
        register_command.SetCommand(btn_Register_click);
        reset_command.SetCommand(btn_Reset_click);
        approve_command.SetCommand(null);
        reject_command.SetCommand(null);
        return_command.SetCommand(null);
        close_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }
    else if (intMode == conModeEdit) {
        // ModeEdit = 2;
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        approve_command.SetCommand(null);
        reject_command.SetCommand(null);
        return_command.SetCommand(null);
        close_command.SetCommand(null);
        confirm_command.SetCommand(btn_Confirm_click);
        back_command.SetCommand(btn_Back_click);
    }
    else if (intMode == conModeConfirm) {
        // ModeConfirm = 9;
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        approve_command.SetCommand(null);
        reject_command.SetCommand(null);
        return_command.SetCommand(null);
        close_command.SetCommand(null);
        confirm_command.SetCommand(btn_Confirm_click);
        back_command.SetCommand(btn_Back_click);
    }
}

//Mode Event Click
function btn_Retrieve_click() {
    // check all input on Server
    var obj = GetUserAdjustData();
    _doReceipt = null;
    _dotbt_MoneyCollectionInfo = null;
    ajax_method.CallScreenController("/Income/ICS100_RetrieveData", obj, function (result, controls, isWarning) {
        if (result != undefined) {

            // goto View state
            // Wired design
            // move mode to add button
            //setFormMode(conModeView);
            //bolViewMode = true;
            //$("#divSpecifyProcessType").SetViewMode(true);

            $("#txtReceiptNo").attr("disabled", true);

            $("#txtBillingTargetCode").val(result.doReceipt.BillingTargetCodeShort);
            $("#txtBillingClientNameEN").val(result.doReceipt.BillingClientNameEN);
            $("#txtBillingClientNameLC").val(result.doReceipt.BillingClientNameLC);
            $("#txtBillingClientAddressEN").val(result.doReceipt.BillingClientAddressEN);
            $("#txtBillingClientAddressLC").val(result.doReceipt.BillingClientAddressLC); 
            $("#txtReceiptAmount").val(result.doReceipt.ReceiptAmountString);
            $("#txtReceiptAmount").NumericCurrency().val(result.doReceipt.ReceiptAmountCurrencyType);
            _doReceipt = result.doReceipt;

            if (result._dotbt_MoneyCollectionInfo != undefined) {
                $("#cboCollectionArea").val(result._dotbt_MoneyCollectionInfo.CollectionArea);
                $("#dtpExpectedCollectDate").val(ConvertDateToTextFormat(ConvertDateObject(result._dotbt_MoneyCollectionInfo.ExpectedCollectDate)));
                $("#txtaMemo").val(result._dotbt_MoneyCollectionInfo.Memo);
            } else {
                $("#cboCollectionArea").val("");
                $("#dtpExpectedCollectDate").val();
                $("#txtaMemo").val();
            }


            // Hide delete button in grid
            //var colBtnRemoveIdx1 = grdManageMoneyCollectionGrid.getColIndexById("Del");
            //grdManageMoneyCollectionGrid.setColumnHidden(colBtnRemoveIdx1, true);

            $("#btnRetrieve").attr("disabled", true);

            $("#btnAdd").attr("disabled", false);
            $("#btnCancel").attr("disabled", false);

        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });

}

function btn_Add_click() {


    var bolCheckDuplicate = false;

    if (CheckFirstRowIsEmpty(grdManageMoneyCollectionGrid) == false) {

        for (var i = 0; i < grdManageMoneyCollectionGrid.getRowsNum(); i++) {
            
            var row_id = grdManageMoneyCollectionGrid.getRowId(i);

            if ($("#txtReceiptNo").val() == grdManageMoneyCollectionGrid.cells
                (row_id, grdManageMoneyCollectionGrid.getColIndexById("ReceiptNo")).getValue())
            {
                bolCheckDuplicate = true;
            }

        }
    }

    var obj = {
        txtCollectionArea : $("#cboCollectionArea").val(),
        dtpExpectedCollectDate : $("#dtpExpectedCollectDate").val(),
        bolCheckDuplicate : bolCheckDuplicate,
        txtReceiptNo: $("#txtReceiptNo").val()
    };



    // check all input on Server
    ajax_method.CallScreenController("/Income/ICS100_CheckAddDataToGrid", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            if (result == "1") {
                // goto confirm state
                setFormMode(conModeView);
                bolViewMode = true;

                Add_ManageMoneyCollectionBlankLine(
                                                    _doReceipt
                                                    , $("#cboCollectionArea").val()
                                                    , $("#cboCollectionArea option:selected").text()
                                                    , $("#dtpExpectedCollectDate").val()
                                                    , $("#txtaMemo").text());

                // after add to grid clear data
                ClearInputData();
                $("#txtReceiptNo").attr("disabled", false);
                $("#btnRetrieve").attr("disabled", false);

                $("#btnAdd").attr("disabled", true);
                $("#btnCancel").attr("disabled", true);

            }
            else {
                VaridateCtrl(controls, controls);
            }
        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });


//    // use server style
//    //[Collection area] text box, [Expected collected date] date time picker
//    if ($("#cboCollectionArea").val() == '') {

//        /* --- Get Message --- */
//        var obj = {
//            module: "Common",
//            code: "MSG0007"
//        };
//        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
//            OpenErrorMessageDialog(result.Code, result.Message,
//            function () {
//                VaridateCtrl( 'cboCollectionArea' ,  'cboCollectionArea' );
//            });
//        });
//    }
//    else if ($("#dtpExpectedCollectDate").val() == '') {

//        /* --- Get Message --- */
//        var obj = {
//            module: "Common",
//            code: "MSG0007"
//        };
//        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
//        OpenErrorMessageDialog(result.Code, result.Message,
//            function () {
//                VaridateCtrl( 'dtpExpectedCollectDate' ,  'dtpExpectedCollectDate' );
//            });
//        });
//    }
//    else if (bolCheckDuplicate) {

//        /* --- Get Message --- */
//        var obj = {
//            module: "Income",
//            code: "MSG7002"
//        };
//        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
//        OpenErrorMessageDialog(result.Code, result.Message,
//            function () {

//            });
//        });
//    }
//    else{

//        setFormMode(conModeView);
//        bolViewMode = true;

//        Add_ManageMoneyCollectionBlankLine(
//            _doReceipt
//            , $("#cboCollectionArea").val()
//            , $("#cboCollectionArea option:selected").text()
//            , $("#dtpExpectedCollectDate").val()
//            , $("#txtaMemo").text());

//        // after add to grid clear data
//        ClearInputData();
//        $("#txtReceiptNo").attr("disabled", false);
//        $("#btnRetrieve").attr("disabled", false);

//        $("#btnAdd").attr("disabled", true);
//        $("#btnCancel").attr("disabled", true);
//    }
}

function btn_Cancel_click() {

    ClearInputData();
    $("#txtReceiptNo").attr("disabled", false);
    $("#btnRetrieve").attr("disabled", false);

    $("#btnAdd").attr("disabled", true);
    $("#btnCancel").attr("disabled", true);
}

function ClearInputData() {

    $("#txtReceiptNo").val("");

    $("#txtBillingTargetCode").val("");
    $("#txtBillingClientNameEN").val("");
    $("#txtBillingClientNameLC").val("");
    $("#txtBillingClientAddressEN").val("");
    $("#txtBillingClientAddressLC").val("");
    $("#txtReceiptAmount").val("");

    $("#cboCollectionArea").val("");
    $("#dtpExpectedCollectDate").val("");
    $("#txtaMemo").val("");
    _doReceipt = null;
    _dotbt_MoneyCollectionInfo = null;
    $("#divInputData").clearForm();
    CloseWarningDialog();
 
}

function btn_Register_click() {
    // check all input on Server
    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Income/ICS100_Register", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            if (result == "1") {
                // goto confirm state
                setFormMode(conModeEdit);
                
                $("#divInputData").hide();
                $("#divResaultList100").SetViewMode(true);
                $("#divResaultList100").ResetToNormalControl(false);

                bolViewMode = true;
                // Hide delete button in grid
                var colBtnRemoveIdx1 = grdManageMoneyCollectionGrid.getColIndexById("Del");
                grdManageMoneyCollectionGrid.setColumnHidden(colBtnRemoveIdx1, true);

            }
            else {
                VaridateCtrl(controls, controls);
            }
        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });
}
// create all send to server data for check mendatory and save (in case all input data is ok)
function GetUserAdjustData() {

    var arr1 = new Array();

    var header = {
        txtReceiptNo: $("#txtReceiptNo").val()
    };
    
    if (CheckFirstRowIsEmpty(grdManageMoneyCollectionGrid) == false) {

        for (var i = 0; i < grdManageMoneyCollectionGrid.getRowsNum(); i++) {

            var row_id = grdManageMoneyCollectionGrid.getRowId(i);

            var obj1 = {

                        txtReceiptNo: grdManageMoneyCollectionGrid.cells(row_id,
                                    grdManageMoneyCollectionGrid.getColIndexById("ReceiptNo")).getValue(),
                        dtpReceiptDate: grdManageMoneyCollectionGrid.cells(row_id,
                                    grdManageMoneyCollectionGrid.getColIndexById("ReceiptDate")).getValue(),
                        txtBillingTargetCode: grdManageMoneyCollectionGrid.cells(row_id,
                                    grdManageMoneyCollectionGrid.getColIndexById("BillingTargetCodeLong")).getValue(),
                        txtBillingClientName: grdManageMoneyCollectionGrid.cells(row_id,
                                    grdManageMoneyCollectionGrid.getColIndexById("BillingClientName")).getValue(),
                        txtBillingClientAddress: grdManageMoneyCollectionGrid.cells(row_id,
                                    grdManageMoneyCollectionGrid.getColIndexById("BillingClientAddress")).getValue(),
                        txtReceiptAmount: grdManageMoneyCollectionGrid.cells(row_id,
                                    grdManageMoneyCollectionGrid.getColIndexById("ReceiptAmount")).getValue(),
                        txtReceiptAmountCurrencyType: grdManageMoneyCollectionGrid.cells(row_id,
                                            grdManageMoneyCollectionGrid.getColIndexById("ReceiptAmountCurrencyType")).getValue(),
                        txtCollectionArea: grdManageMoneyCollectionGrid.cells(row_id,
                                    grdManageMoneyCollectionGrid.getColIndexById("CollectionAreaCode")).getValue(),
                        dtpExpectedCollectDate: grdManageMoneyCollectionGrid.cells(row_id,
                                    grdManageMoneyCollectionGrid.getColIndexById("ExpectedCollectDate")).getValue(),
                        txtMemo: grdManageMoneyCollectionGrid.cells(row_id,
                                    grdManageMoneyCollectionGrid.getColIndexById("Memo")).getValue(),
                        rowid: i

                // example non clustom grid getvalue
                //strBillingamount: grdByBillingDetailGrid.cells
                //(row_id, grdCancelBillingDetailGrid.getColIndexById("Billingamount")).getValue(),
            };
            arr1.push(obj1);
        }
    }

    var detail1 = arr1;

    var returnObj = {
        Header: header,
        Detail1: detail1
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

            setFormMode(conModeInit);
            bolViewMode = false;
            
            ClearScreenToInitStage();
//        },
//        null);
//    });
}

function btn_Confirm_click() {

    // save data
    ajax_method.CallScreenController("/Income/ICS100_Confirm", "",
        function (result, controls, isWarning) {
            if (result != undefined) {
                if (result == "1") {

                    // Success
                    var objMsg = {
                        module: "Income",
                        code: "MSG7008"
                    };
                    call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                        OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {
                            // goto confirm state

                            setFormMode(conModeInit);
                            bolViewMode = false;

                            $("#divInputData").show();
                            $("#divResaultList100").SetViewMode(false);
                            $("#divResaultList100").ResetToNormalControl(true);

                            // Show delete button in grid
                            var colBtnRemoveIdx1 = grdManageMoneyCollectionGrid.getColIndexById("Del");
                            grdManageMoneyCollectionGrid.setColumnHidden(colBtnRemoveIdx1, false);
                            // Concept by P'Leing
                            SetFitColumnForBackAction(grdManageMoneyCollectionGrid, "TempSpan");

                            ClearScreenToInitStage();
                        });
                    });
                }
            }
            else {
                VaridateCtrl(controls, controls);
            }
        });
}
function btn_Back_click() {
    setFormMode(conModeView);
    bolViewMode = true;

    $("#divInputData").show();
    $("#divResaultList100").SetViewMode(false);
    $("#divResaultList100").ResetToNormalControl(true);

    // Show delete button in grid
    var colBtnRemoveIdx1 = grdManageMoneyCollectionGrid.getColIndexById("Del");
    grdManageMoneyCollectionGrid.setColumnHidden(colBtnRemoveIdx1, false);
    // Concept by P'Leing
    SetFitColumnForBackAction(grdManageMoneyCollectionGrid, "TempSpan");
}

// Clear Screen
function ClearScreenToInitStage() {

    $("#divInputData").show();
    $("#divResaultList100").SetViewMode(false);
    $("#divResaultList100").ResetToNormalControl(true);

    $("#txtReceiptNo").attr("disabled", false);
    $("#btnRetrieve").attr("disabled", false);

    $("#btnAdd").attr("disabled", true);
    $("#btnCancel").attr("disabled", true);

    ClearInputData();

    DeleteAllRow(grdManageMoneyCollectionGrid);

    _doReceipt = null;
    _dotbt_MoneyCollectionInfo = null;
}
