
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

var pageRow = ROWS_PER_PAGE_FOR_VIEWPAGE;
var grdCancelCreditNote;

var grdViewUnpaidBillingDetailListGrid;
var _doGetMoneyCollectionManagementInfoList;

$.fn.BindingReadOnly = function (t) {
    $(this).bind("propertychange", function () {
        if (event.propertyName == "readOnly") {
            if ($(this).prop("readOnly") != undefined) {
                if ($(this).prop("readonly"))
                    $(t).hide();
                else
                    $(t).show();
            }
        }
    });
}



$(document).ready(function () {
    if ($.find("#ICS070_CancelCreditNoteGrid").length > 0) {

        grdCancelCreditNote = $("#ICS070_CancelCreditNoteGrid").InitialGrid(pageRow, false, "/Income/ICS070_InitialCancelCreditNoteGrid", function () {

            SpecialGridControl(grdCancelCreditNote, ["ICS070_CancelCreditNoteGrid", "Del"]);
            BindOnLoadedEvent(grdCancelCreditNote, function () {

                var colInx = grdCancelCreditNote.getColIndexById('Button');

                for (var i = 0; i < grdCancelCreditNote.getRowsNum(); i++) {

                    var rowId = grdCancelCreditNote.getRowId(i);
                    //-----------------------------------------

                    // Col 6
                    GenerateRemoveButton(grdCancelCreditNote, "btnRemove", rowId, "Del", true);
                    BindGridButtonClickEvent("btnRemove", rowId, btnRemoveCancelCreditNoteGrid);
                }

                grdCancelCreditNote.setSizes();
            });
        });

    }

    //Init Object Event
    // 1 Div Panel Body

    $("#optRegisterRefundCreditNoteExceptDepositFee").change(optRegisterRefundCreditNoteExceptDepositFee_Select);
    $("#optRegisterDecreasedCreditNote").change(optRegisterDecreasedCreditNote_Select);
    $("#optRegisterRefundCreditNoteDepositFee").change(optRegisterRefundCreditNoteDepositFee_Select);
    $("#optRevenueDepositFee").change(optRevenueDepositFee_Select);
    $("#optCancelCreditNote").change(optCancelCreditNote_Select);

    $("#btnSelectProcess").click(btn_SelectProcess_click);

    $("#btnExceptDepositFeeRetrieveBillingInformation").click(btn_ExceptDepositFeeRetrieveBillingInformation_click);
    $("#btnExceptDepositFeeRetrieve").click(btn_ExceptDepositFeeRetrieve_click);
    $("#btnExceptDepositFeeAdd").click(btn_ExceptDepositFeeAdd_click);
    $("#btnExceptDepositFeeCancel").click(btn_ExceptDepositFeeCancel_click);

    $("#btnDepositFeeRetrieveBillingInformation").click(btn_DepositFeeRetrieveBillingInformation_click);
    $("#btnDepositFeeRetrieve").click(btn_DepositFeeRetrieve_click);
    $("#btnDepositFeeAdd").click(btn_DepositFeeAdd_click);
    $("#btnDepositFeeCancel").click(btn_DepositFeeCancel_click);

    $("#btnRevenueRetrieve").click(btn_RevenueRetrieve_click);
    $("#btnRevenueAdd").click(btn_RevenueAdd_click);
    $("#btnRevenueCancel").click(btn_RevenueCancel_click);

    $("#btnNextToRegister").click(btn_NextToRegister_click);

    $("#chkExceptDepositFeeNotRetrieve").change(chk_ExceptDepositFeeNotRetrieve_change);
    $("#chkDepositFeeNotRetrieve").change(chk_DepositFeeNotRetrieve_change);

    ajax_method.CallScreenController("/Income/ICS070_CheckOperate", "",
        function (result, controls, isWarning) {
            if (result != undefined) {
                if (result == '1') {

                    $("#optRegisterRefundCreditNoteExceptDepositFee").attr("disabled", false);
                    $("#optRegisterDecreasedCreditNote").attr("disabled", false);
                    $("#optRegisterRefundCreditNoteDepositFee").attr("disabled", false);
                    $("#optRevenueDepositFee").attr("disabled", false);

                } else {

                    $("#optRegisterRefundCreditNoteExceptDepositFee").attr("disabled", true);
                    $("#optRegisterDecreasedCreditNote").attr("disabled", true);
                    $("#optRegisterRefundCreditNoteDepositFee").attr("disabled", true);
                    $("#optRevenueDepositFee").attr("disabled", true);
                    $("#optCancelCreditNote").attr("checked", true);
                    verModeRadio1 = conModeRadio1optCancelCreditNote;
                    $("#txtCancelCreditNoteNo").attr("readonly", false);
                }
            }
            else {

                $("#optRegisterRefundCreditNoteExceptDepositFee").attr("disabled", true);
                $("#optRegisterDecreasedCreditNote").attr("disabled", true);
                $("#optRegisterRefundCreditNoteDepositFee").attr("disabled", true);
                $("#optRevenueDepositFee").attr("disabled", true);
                $("#optCancelCreditNote").attr("checked", true);
                verModeRadio1 = conModeRadio1optCancelCreditNote;
                $("#txtCancelCreditNoteNo").attr("readonly", false);
            }



            ajax_method.CallScreenController("/Income/ICS070_CheckCancelCreditNote", "",
            function (result, controls, isWarning) {
                if (result != undefined) {
                    if (result == '1') {
                        //$("#optCancelCreditNote").attr("disabled", false);
                        //$("#txtCancelCreditNoteNo").attr("readonly", false);
                    } else {
                        $("#optCancelCreditNote").attr("disabled", true);
                        $("#txtCancelCreditNoteNo").attr("readonly", true);
                    }
                }
                else {
                    $("#optCancelCreditNote").attr("disabled", true);
                    $("#txtCancelCreditNoteNo").attr("readonly", true);
                    //VaridateCtrl(controls, controls);
                }
            });

        });

    if (verModeRadio1 != conModeRadio1optCancelCreditNote) {
        $("#txtCancelCreditNoteNo").attr("readonly", true);
    }

    //Initial Page
    InitialPage();
});


// Grid Function
function btnRemoveCancelCreditNoteGrid(rowId) {

    if (!(bolViewMode)) {
        //if (grdCancelCreditNote.getRowsNum() > 1) {
        DeleteRow(grdCancelCreditNote, rowId);
        //}
    }
}

function Add_CancelCreditNoteBlankLine(intRow, strRegistrationContents, intAmountIncludingVatShow, intVatAmountShow, dtpCreditNoteDate,
strBillingCode, strCreditNoteNo, strBillingClientName,
strCreditNoteType, strTaxInvoiceNo, strBillingTargetCode, intCreditAmountIncVAT, intCreditVATAmount,
strBillingTypeCode, strApproveNo, strIssueReason, intTaxInvoiceAmount, intTaxInvoiceVATAmount,
dtpTaxInvoiceDate, strPaymentTransNo, strCancelFlag, strRevenueNo, dtpIssueDate,
intRevenueAmountIncVAT, intRevenueVATAmount, strInvoiceNo, strInvoiceOCC, boNotRetrieveFlag,
strBillingClientNameEN, intAmountIncludingVat, intVatAmount, intAmountIncludingVatCurrencyType, intVatAmountCurrencyType
, strRevenueAmountIncVATCurrencyType, strRevenueVATAmountCurrencyType) {

    //Add by Budd, Convert bool to string
    var strNotRetrieveFlag = "0";
    if (boNotRetrieveFlag == true) strNotRetrieveFlag = "1";

    CheckFirstRowIsEmpty(grdCancelCreditNote, true);

    AddNewRow(grdCancelCreditNote, [intRow,
                                strRegistrationContents,
                                intAmountIncludingVatShow,
                                intVatAmountShow,
                                dtpCreditNoteDate,
                                strBillingCode,
                                strCreditNoteNo,
                                strBillingClientName,
                                "",
                                "",
                                strCreditNoteType,
                                strTaxInvoiceNo,
                                strBillingTargetCode,
                                intCreditAmountIncVAT,
                                intCreditVATAmount,
                                strBillingTypeCode,
                                strApproveNo,
                                strIssueReason,
                                intTaxInvoiceAmount,
                                intTaxInvoiceVATAmount,
                                dtpTaxInvoiceDate,
                                strPaymentTransNo,
                                strCancelFlag,
                                strRevenueNo,
                                dtpIssueDate,
                                intRevenueAmountIncVAT,
                                intRevenueVATAmount,
                                strInvoiceNo,
                                strInvoiceOCC,
                                strNotRetrieveFlag,
                                strBillingClientNameEN,
                                intAmountIncludingVat,
                                intVatAmount,
                                intAmountIncludingVatCurrencyType,
                                intVatAmountCurrencyType,
                                strRevenueAmountIncVATCurrencyType, // add by jirawat jannet on 2016-11-01
                                strRevenueVATAmountCurrencyType]); // add by jirawat jannet on 2016-11-01

    grdCancelCreditNote.setSizes();
}


function InitialPage() {

    $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtExceptDepositFeeTaxInvoiceAmount").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtExceptDepositFeeAccumulatedPaymentAmount").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtExceptDepositFeeCreditNoteAmount").BindNumericBox(12, 2, 0, 999999999999.99);

    $("#txtDepositFeeBalanceOfDepositFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtDepositFeeTaxInvoiceAmountIncludeVat").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtDepositFeeTaxInvoiceAmount").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtDepositFeeAccumulatedPaymentAmount").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtDepositFeeCreditNoteAmountIncludeVat").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtDepositFeeCreditNoteAmount").BindNumericBox(12, 2, 0, 999999999999.99);

    $("#txtRevenueBalanceOfDepositFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtRevenueRevenueDepositFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtRevenueRevenueVatAmount").BindNumericBox(12, 2, 0, 999999999999.99);

    $("#txtRegisterCancelTaxInvoiceAmountIncludeVat").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtRegisterCancelTaxInvoiceAmount").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtRegisterCancelAccumulatedPaymentAmount").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtRegisterCancelCreditNoteAmountIncludeVat").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtRegisterCancelCreditNoteAmount").BindNumericBox(12, 2, 0, 999999999999.99);

    $("#dtpExceptDepositFeeTaxInvoiceDate").InitialDate();
    $("#dtpExceptDepositFeeCreditNoteDate").InitialDate();
    $("#dtpDepositFeeTaxInvoiceDate").InitialDate();
    $("#dtpDepositFeeCreditNoteDate").InitialDate();

    $("#dtpRegisterCancelTaxInvoiceDate").InitialDate();
    $("#dtpRegisterCancelCreditNoteDate").InitialDate();

    $("#txtCancelCreditNoteNo").attr("maxlength", 12);

    $("#txtExceptDepositFeeTaxInvoiceNoForCreditNote").attr("maxlength", 12);
    $("#txtDepositFeeTaxInvoiceNoForCreditNote").attr("maxlength", 12);
    $("#txtExceptDepositFeeBillingCode1").attr("maxlength", 9);
    $("#txtExceptDepositFeeBillingCode2").attr("maxlength", 2);
    $("#txtDepositFeeBillingCode1").attr("maxlength", 9);
    $("#txtDepositFeeBillingCode2").attr("maxlength", 2);
    $("#txtRevenueBillingCode1").attr("maxlength", 9);
    $("#txtRevenueBillingCode2").attr("maxlength", 2);

    $("#txtaExceptDepositFeeIssueReason").SetMaxLengthTextArea(200);
    $("#txtaRegisterCancelIssueReason").SetMaxLengthTextArea(200);
    $("#txtaDepositFeeIssueReason").SetMaxLengthTextArea(200);

    //Binding disabled control and requir field
    $("#txtDepositFeeTaxInvoiceAmountIncludeVat").BindingReadOnly("#divDepositFeeTaxInvoiceAmountIncludeVatRequired");
    $("#txtDepositFeeTaxInvoiceAmount").BindingReadOnly("#divDepositFeeTaxInvoiceAmountRequired");
    $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").BindingReadOnly("#divExceptDepositFeeTaxInvoiceAmountIncludeVatRequired");
    $("#txtExceptDepositFeeTaxInvoiceAmount").BindingReadOnly("#divExceptDepositFeeTaxInvoiceAmountRequired");

    setVisableTable(0);
    setFormMode(conModeInit);

    setResetEnabledObject();
    setEnabledObject(verModeRadio1);
}
// Form Mode Section

// Form Mode Section

var conModeInit = 0;
var conModeView = 1;
var conModeEdit = 2;
var conModeConfirm = 9;

var conModeRadio1optRegisterRefundCreditNoteExceptDepositFee = 1;
var conModeRadio1optRegisterDecreasedCreditNote = 2;
var conModeRadio1optRegisterRefundCreditNoteDepositFee = 3;
var conModeRadio1optRevenueDepositFee = 4;
var conModeRadio1optCancelCreditNote = 5;

var conNo = 0;
var conYes = 1;

var verModeRadio1 = 3;

var bolViewMode = false;

function setFormMode(intMode) {
    // ModeInit
    if (intMode == conModeInit) {
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        approve_command.SetCommand(null);
        reject_command.SetCommand(null);
        return_command.SetCommand(null);
        close_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
        confirmCancel_command.SetCommand(null);
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
        confirmCancel_command.SetCommand(null);
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
        confirmCancel_command.SetCommand(null);
    }
    else if (intMode == conModeConfirm) {
        // ModeConfirm = 9;
        register_command.SetCommand(null);
        reset_command.SetCommand(btn_Back_click);       //Custom
        approve_command.SetCommand(null);
        reject_command.SetCommand(null);
        return_command.SetCommand(null);
        close_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
        confirmCancel_command.SetCommand(btn_Confirm_click);
    }
}

// Mode Event
function btn_Register_click() {
    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Income/ICS070_Register", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            if (result == "1") {

                setAllObjectToConfirmMode(conYes);
                setFormMode(conModeEdit);
                // Hide delete button in grid
                var colBtnRemoveIdx1 = grdCancelCreditNote.getColIndexById("Del");
                grdCancelCreditNote.setColumnHidden(colBtnRemoveIdx1, true);
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
        rdoProcessType: verModeRadio1,
        txtCancelCreditNoteNo: $('#txtCancelCreditNoteNo').val().toUpperCase(),

        txtExceptDepositFeeTaxInvoiceNoForCreditNote: $('#txtExceptDepositFeeTaxInvoiceNoForCreditNote').val().toUpperCase(),
        txtDepositFeeTaxInvoiceNoForCreditNote: $('#txtDepositFeeTaxInvoiceNoForCreditNote').val().toUpperCase(),

        txtDepositFeeBillingCode: $('#txtDepositFeeBillingCode1').val().toUpperCase() + '-' + $('#txtDepositFeeBillingCode2').val().toUpperCase(),
        txtExceptDepositFeeBillingCode: $('#txtExceptDepositFeeBillingCode1').val().toUpperCase() + '-' + $('#txtExceptDepositFeeBillingCode2').val().toUpperCase(),
        txtRevenueBillingCode: $('#txtRevenueBillingCode1').val().toUpperCase() + '-' + $('#txtRevenueBillingCode2').val().toUpperCase()
    };

    var Input1 = {
        chkExceptDepositFeeNotRetrieve: $('#chkExceptDepositFeeNotRetrieve').prop('checked'),

        dtpExceptDepositFeeTaxInvoiceDate: $('#dtpExceptDepositFeeTaxInvoiceDate').val(),
        dtpExceptDepositFeeCreditNoteDate: $('#dtpExceptDepositFeeCreditNoteDate').val(),

        strExceptDepositFeeTaxInvoiceBillingType: $('#cboExceptDepositFeeTaxInvoiceBillingType').val(),

        strExceptDepositFeeTaxInvoiceNoForCreditNote: $('#txtExceptDepositFeeTaxInvoiceNoForCreditNote').val().toUpperCase(),
        strExceptDepositFeeBillingCode: $('#txtExceptDepositFeeBillingCode1').val().toUpperCase() + '-' + $('#txtExceptDepositFeeBillingCode2').val().toUpperCase(),

        strExceptDepositFeeTaxInvoiceAmountIncludeVat: $('#txtExceptDepositFeeTaxInvoiceAmountIncludeVat').val(),
        strExceptDepositFeeTaxInvoiceAmountIncludeVatCurrencyType: $('#txtExceptDepositFeeTaxInvoiceAmountIncludeVat').NumericCurrencyValue(),

        strExceptDepositFeeTaxInvoiceAmount: $('#txtExceptDepositFeeTaxInvoiceAmount').val(),
        strExceptDepositFeeTaxInvoiceAmountCurrencyType: $('#txtExceptDepositFeeTaxInvoiceAmount').NumericCurrencyValue(),

        strExceptDepositFeeAccumulatedPaymentAmount: $('#txtExceptDepositFeeAccumulatedPaymentAmount').val(),
        strExceptDepositFeeAccumulatedPaymentAmountCurrencyType: $('#txtExceptDepositFeeAccumulatedPaymentAmount').NumericCurrencyValue(),

        strExceptDepositFeeCreditNoteAmountIncludeVat: $('#txtExceptDepositFeeCreditNoteAmountIncludeVat').val(),
        strExceptDepositFeeCreditNoteAmountIncludeVatCurrencyType: $('#txtExceptDepositFeeCreditNoteAmountIncludeVat').NumericCurrencyValue(),

        strExceptDepositFeeCreditNoteAmount: $('#txtExceptDepositFeeCreditNoteAmount').val(),
        strExceptDepositFeeCreditNoteAmountCurrencyType: $('#txtExceptDepositFeeCreditNoteAmount').NumericCurrencyValue(),

        strExceptDepositFeeApproveNo: $('#txtExceptDepositFeeApproveNo').val(),
        straExceptDepositFeeIssueReason: $('#txtaExceptDepositFeeIssueReason').val()
    };

    var Input2 = {
        chkDepositFeeNotRetrieve: $('#chkDepositFeeNotRetrieve').prop('checked'),

        dtpDepositFeeTaxInvoiceDate: $('#dtpDepositFeeTaxInvoiceDate').val(),
        dtpDepositFeeCreditNoteDate: $('#dtpDepositFeeCreditNoteDate').val(),

        strDepositFeeTaxInvoiceBillingType: $('#cboDepositFeeTaxInvoiceBillingType').val(),

        strDepositFeeTaxInvoiceNoForCreditNote: $('#txtDepositFeeTaxInvoiceNoForCreditNote').val().toUpperCase(),
        strDepositFeeBillingCode: $('#txtDepositFeeBillingCode1').val().toUpperCase() + '-' + $('#txtDepositFeeBillingCode2').val().toUpperCase(),
        strDepositFeeBalanceOfDepositFee: $('#txtDepositFeeBalanceOfDepositFee').val(),
        strDepositFeeBalanceOfDepositFeeCurrencyType: $('#txtDepositFeeBalanceOfDepositFee').NumericCurrencyValue(),

        strDepositFeeTaxInvoiceAmountIncludeVat: $('#txtDepositFeeTaxInvoiceAmountIncludeVat').val(),
        strDepositFeeTaxInvoiceAmountIncludeVatCurrencyType: $('#txtDepositFeeTaxInvoiceAmountIncludeVat').NumericCurrencyValue(),

        strDepositFeeTaxInvoiceAmount: $('#txtDepositFeeTaxInvoiceAmount').val(),
        strDepositFeeTaxInvoiceAmountCurrencyType: $('#txtDepositFeeTaxInvoiceAmount').NumericCurrencyValue(),

        strDepositFeeAccumulatedPaymentAmount: $('#txtDepositFeeAccumulatedPaymentAmount').val(),
        strDepositFeeAccumulatedPaymentAmountCurrencyType: $('#txtDepositFeeAccumulatedPaymentAmount').NumericCurrencyValue(),

        strDepositFeeCreditNoteAmountIncludeVat: $('#txtDepositFeeCreditNoteAmountIncludeVat').val(),
        strDepositFeeCreditNoteAmountIncludeVatCurrencyType: $('#txtDepositFeeCreditNoteAmountIncludeVat').NumericCurrencyValue(),

        strDepositFeeCreditNoteAmount: $('#txtDepositFeeCreditNoteAmount').val(),
        strDepositFeeCreditNoteAmountCurrencyType: $('#txtDepositFeeCreditNoteAmount').NumericCurrencyValue(),

        strDepositFeeApproveNo: $('#txtDepositFeeApproveNo').val(),
        straDepositFeeIssueReason: $('#txtaDepositFeeIssueReason').val()
    };

    var Input3 = {
        strRevenueBillingCode: $('#txtRevenueBillingCode1').val().toUpperCase() + '-' + $('#txtRevenueBillingCode2').val().toUpperCase(),
        strRevenueBalanceOfDepositFee: $('#txtRevenueBalanceOfDepositFee').val(),
        strRevenueBalanceOfDepositFeeCurrencyType: $('#txtRevenueBalanceOfDepositFee').NumericCurrencyValue(),

        strRevenueRevenueDepositFee: $('#txtRevenueRevenueDepositFee').val(),
        strRevenueRevenueDepositFeeCurrencyType: $('#txtRevenueRevenueDepositFee').NumericCurrencyValue(),

        strRevenueRevenueVatAmount: $('#txtRevenueRevenueVatAmount').val(),
        strRevenueRevenueVatAmountCurrencyType: $('#txtRevenueRevenueVatAmount').NumericCurrencyValue()
    };

    if (CheckFirstRowIsEmpty(grdCancelCreditNote) == false) {

        for (var i = 0; i < grdCancelCreditNote.getRowsNum(); i++) {

            var row_id = grdCancelCreditNote.getRowId(i);

            // non custom object in grid use object name
            // use column name in XML not declare ass txt, dtp ect

            var obj1 = {

                strRegistrationContents: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("RegistrationContents")).getValue(),
                strAmountIncludingVat: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("AmountIncludingVat")).getValue(),
                strAmountIncludingVatCurrencyType: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("AmountIncludingVatCurrencyType")).getValue(),
                strVatAmount: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("VatAmount")).getValue(),
                strVatAmountCurrencyType: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("VatAmountCurrencyType")).getValue(),
                dtpCreditNoteDate: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("CreditNoteDate")).getValue(),
                strBillingCode: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("BillingCode")).getValue(),
                strCreditNoteNo: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("CreditNoteNo")).getValue(),
                strBillingClientName: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("BillingClientName")).getValue(),
                //------------------------------------------------
                strCreditNoteType: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("CreditNoteType")).getValue(),
                strTaxInvoiceNo: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("TaxInvoiceNo")).getValue(),
                strBillingTargetCode: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("BillingTargetCode")).getValue(),
                strCreditAmountIncVAT: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("CreditAmountIncVAT")).getValue(),
                strCreditVATAmount: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("CreditVATAmount")).getValue(),
                strBillingTypeCode: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("BillingTypeCode")).getValue(),
                strApproveNo: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("ApproveNo")).getValue(),
                strIssueReason: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("IssueReason")).getValue(),
                strTaxInvoiceAmount: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("TaxInvoiceAmount")).getValue(),
                strTaxInvoiceVATAmount: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("TaxInvoiceVATAmount")).getValue(),
                dtpTaxInvoiceDate: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("TaxInvoiceDate")).getValue(),
                strPaymentTransNo: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("PaymentTransNo")).getValue(),
                strCancelFlag: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("CancelFlag")).getValue(),
                strRevenueNo: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("RevenueNo")).getValue(),
                dtpIssueDate: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("IssueDate")).getValue(),
                strRevenueAmountIncVAT: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("RevenueAmountIncVAT")).getValue(),
                strRevenueAmountIncVATCurrencyType: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("RevenueAmountIncVATCurrencyType")).getValue(),
                strRevenueVATAmount: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("RevenueVATAmount")).getValue(),
                strRevenueVATAmountCurrencyType: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("RevenueVATAmountCurrencyType")).getValue(),
                strInvoiceNo: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("InvoiceNo")).getValue(),
                strInvoiceOCC: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("InvoiceOCC")).getValue(),
                strNotRetrieveFlag: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("NotRetrieveFlag")).getValue(),
                strBillingClientNameEN: grdCancelCreditNote.cells
                                    (row_id, grdCancelCreditNote.getColIndexById("BillingClientNameEN")).getValue(),

                rowid: i

                // example non clustom grid getvalue
                //strBillingamount: grdMoneyCollectionManagementInformationGrid.cells
                //(row_id, grdMoneyCollectionManagementInformationGrid.getColIndexById("Billingamount")).getValue(),
            };
            arr1.push(obj1);

        }
    };


    var detail1 = arr1;

    var returnObj = {
        Header: header,
        Input1: Input1,
        Input2: Input2,
        Input3: Input3,
        Detail1: detail1
    };

    return returnObj;

}


function btn_Reset_click() {
    ajax_method.CallScreenController("/Income/ICS070_Reset", "", function (result, controls, isWarning) {
        setAllObjectToConfirmMode(conNo);
        setVisableTable(0);
        setFormMode(conModeInit);
        bolViewMode = false;

        ClearScreenToInitStage();
    });
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

    // Confirm Delete ?
    if (verModeRadio1 == conModeRadio1optCancelCreditNote) {

        /* --- Get Message --- */
        var obj = {
            module: "Income",
            code: "MSG7061"
        };
        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
            OpenOkCancelDialog(result.Code, result.Message,
        function () {

            // save data
            ajax_method.CallScreenController("/Income/ICS070_ConfirmDelete", "",
            function (result, controls, isWarning) {
                if (result != undefined) {
                    if (result == "1") {
                        // goto confirm state

                        setAllObjectToConfirmMode(conNo);
                        setVisableTable(0);
                        setFormMode(conModeInit);
                        bolViewMode = false;

                        ClearScreenToInitStage();

                        // Show delete button in grid
                        var colBtnRemoveIdx1 = grdCancelCreditNote.getColIndexById("Del");
                        grdCancelCreditNote.setColumnHidden(colBtnRemoveIdx1, false);
                        // Concept by P'Leing
                        SetFitColumnForBackAction(grdCancelCreditNote, "TempSpan");

                        /* --- Get Message --- */
                        var obj = {
                            module: "Income",
                            code: "MSG7062"
                        };
                        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                            OpenInformationMessageDialog(result.Code, result.Message,
                        function () {
                            // no funcition
                        },
                        null);
                        });
                    }


                }
                else {
                    VaridateCtrl(controls, controls);
                }
            });

        },
        null);
        });
    } else {



        // save data
        ajax_method.CallScreenController("/Income/ICS070_Confirm", "",
        function (result, controls, isWarning) {
            if (result != undefined) {

                // goto confirm state
                // this section show new credit no than system gen to client
                // btn Next will Reset screen

                DeleteAllRow(grdCancelCreditNote);

                //Modified by Budd,  i+1 -> grdCancelCreditNote.getRowsNum()

                for (var i = 0; i < result.Detail1.length; i++) {

                    if (!(verModeRadio1 == conModeRadio1optRevenueDepositFee)) {

                        Add_CancelCreditNoteBlankLine(
                        grdCancelCreditNote.getRowsNum(),
                        result.Detail1[i].strRegistrationContents,
                        result.Detail1[i].strAmountIncludingVatCurrencyTypeName + ' ' + number_format(parseFloat(result.Detail1[i].strAmountIncludingVat), 2, '.', ','),
                        result.Detail1[i].strVatAmountCurrencyTypeName + ' ' + number_format(parseFloat(result.Detail1[i].strVatAmount), 2, '.', ','),
                        ConvertDateToTextFormat(ConvertDateObject(result.Detail1[i].dtpCreditNoteDate)),

                        result.Detail1[i].strBillingCode,
                        result.Detail1[i].strCreditNoteNo,
                        result.Detail1[i].strBillingClientName,

                        '',
                        '',

                        '',
                        '',
                        '',
                        '',
                        '',

                        '',
                        '',
                        '',
                        '',
                        '',

                        '',
                        '',
                        '', // RevenueAmountIncVAT
                        '', // RevenueVATAmount
                        '',

                        '',
                        '',
                        result.Detail1[i].strBillingClientNameEN,
                        number_format(parseFloat(result.Detail1[i].strAmountIncludingVat), 2, '.', ','),
                        number_format(parseFloat(result.Detail1[i].strVatAmount), 2, '.', ','),
                        result.Detail1[i].strAmountIncludingVatCurrencyType,
                        result.Detail1[i].strVatAmountCurrencyType,
                        '',
                        '');

                        var key = ajax_method.GetKeyURL(null);
                        //var url = ajax_method.GenerateURL("/Income/ICR020_CreditNote?k=" + key + "&creditNoteNo=" + result.Detail1[i].strCreditNoteNo);
                        //var url = ajax_method.GenerateURL("/Income/ICS070_CreditNoteReport?k=" + key + "&strFilePath=" + result.Detail1[i].FilePath); //Modify by Jutarat A. on 02102013
                        //window.open(url, "", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");

                    } else {

                        Add_CancelCreditNoteBlankLine(
                        grdCancelCreditNote.getRowsNum(),
                        result.Detail1[i].strRegistrationContents,
                        result.Detail1[i].strAmountIncludingVatCurrencyTypeName + ' ' + number_format(parseFloat(result.Detail1[i].strAmountIncludingVat), 2, '.', ','),
                        result.Detail1[i].strVatAmountCurrencyTypeName + ' ' + number_format(parseFloat(result.Detail1[i].strVatAmount), 2, '.', ','),
                        ConvertDateToTextFormat(ConvertDateObject(result.Detail1[i].dtpCreditNoteDate)),

                        result.Detail1[i].strBillingCode,
                        result.Detail1[i].strRevenueNo,
                        result.Detail1[i].strBillingClientName,

                        '',
                        '',

                        '',
                        '',
                        '',
                        '',
                        '',

                        '',
                        '',
                        '',
                        '',
                        '',

                        '',
                        '',
                        '',
                        '',
                        '',

                        '',
                        '',
                        result.Detail1[i].strBillingClientNameEN,
                        number_format(parseFloat(result.Detail1[i].strAmountIncludingVat), 2, '.', ','),
                        number_format(parseFloat(result.Detail1[i].strVatAmount), 2, '.', ','),
                        result.Detail1[i].strAmountIncludingVatCurrencyType,
                        result.Detail1[i].strVatAmountCurrencyType,
                        '',
                        '');
                    }




                }

                setFormMode(conModeInit);
                $("#btnNextToRegister").attr("disabled", false);
                $("#btnNextToRegister").show();
            }
            else {
                VaridateCtrl(controls, controls);
            }
        });
    }
}
function btn_Back_click() {


    if (verModeRadio1 == conModeRadio1optCancelCreditNote) {

        var TemptxtCancelCreditNoteNo;

        TemptxtCancelCreditNoteNo = $("#txtCancelCreditNoteNo").val();
        // reset all screen
        setVisableTable(0);
        setFormMode(conModeInit);
        bolViewMode = false;

        $("#divSelectProcess").SetViewMode(false);
        $("#divSelectProcess").ResetToNormalControl(true);

        $("#divRegisterRefundCreditNoteExceptDepositFee").SetViewMode(false);
        $("#divRegisterRefundCreditNoteExceptDepositFee").ResetToNormalControl(true);

        $("#divRegisterRefundCreditNoteDepositFee").SetViewMode(false);
        $("#divRegisterRefundCreditNoteDepositFee").ResetToNormalControl(true);

        $("#divRevenueDepositFee").SetViewMode(false);
        $("#divRevenueDepositFee").ResetToNormalControl(true);

        $("#divCancelCreditNote").SetViewMode(false);
        $("#divCancelCreditNote").ResetToNormalControl(true);

        $("#divRegisterCancelCreditNoteInformation").SetViewMode(false);
        $("#divRegisterCancelCreditNoteInformation").ResetToNormalControl(true);

        ClearScreenToInitStage();
        // reture user input and stage
        //$("#txtCancelCreditNoteNo").val(TemptxtCancelCreditNoteNo);
        $("#txtCancelCreditNoteNo").attr("readonly", false);
        verModeRadio1 = conModeRadio1optCancelCreditNote;
        $("#optCancelCreditNote").attr("checked", true);
        $("#btnNextToRegister").attr("disabled", true);
        $("#btnNextToRegister").hide();

    }
    else {
        setAllObjectToConfirmMode(conNo);
        setFormMode(conModeView);
        setVisableTable(verModeRadio1);

        $("#optRegisterRefundCreditNoteExceptDepositFee").attr("disabled", true);
        $("#optRegisterDecreasedCreditNote").attr("disabled", true);
        $("#optRegisterRefundCreditNoteDepositFee").attr("disabled", true);
        $("#optRevenueDepositFee").attr("disabled", true);
        $("#optCancelCreditNote").attr("disabled", true);
        $("#btnNextToRegister").attr("disabled", true);
        $("#btnNextToRegister").hide();
    }


    //    setFormMode(conModeView);
    //    bolViewMode = false;
    //    $("#divResaultList").SetViewMode(false);

    //    $("#divResaultList").ResetToNormalControl(true);


    // Show delete button in grid
    var colBtnRemoveIdx1 = grdCancelCreditNote.getColIndexById("Del");
    grdCancelCreditNote.setColumnHidden(colBtnRemoveIdx1, false);
    // Concept by P'Leing
    SetFitColumnForBackAction(grdCancelCreditNote, "TempSpan");


}

// Clear Screen
function ClearScreenToInitStage() {


    $("#optRegisterRefundCreditNoteExceptDepositFee").attr("disabled", false);
    $("#optRegisterDecreasedCreditNote").attr("disabled", false);
    $("#optRegisterRefundCreditNoteDepositFee").attr("disabled", false);
    $("#optRevenueDepositFee").attr("disabled", false);
    $("#optCancelCreditNote").attr("disabled", false);

    //$("#optRegisterRefundCreditNoteExceptDepositFee").attr("checked", true);
    //    $("#optRegisterDecreasedCreditNote").attr("checked", true);
    $("#optRegisterRefundCreditNoteDepositFee").attr("checked", true);
    //    $("#optRevenueDepositFee").attr("disabled").attr("checked", true);
    //    $("#optCancelCreditNote").attr("disabled").attr("checked", true);

    $("#btnSelectProcess").attr("disabled", false);

    $("#btnExceptDepositFeeRetrieveBillingInformation").attr("disabled", false);
    $("#btnExceptDepositFeeRetrieve").attr("disabled", false);
    $("#btnExceptDepositFeeAdd").attr("disabled", false);
    $("#btnExceptDepositFeeCancel").attr("disabled", false);
    $("#btnDepositFeeRetrieveBillingInformation").attr("disabled", false);
    $("#btnDepositFeeRetrieve").attr("disabled", false);
    $("#btnDepositFeeAdd").attr("disabled", false);
    $("#btnDepositFeeCancel").attr("disabled", false);
    $("#btnRevenueRetrieve").attr("disabled", false);
    $("#btnRevenueAdd").attr("disabled", false);
    $("#btnRevenueCancel").attr("disabled", false);
    $("#btnNextToRegister").attr("disabled", false);
    $("#btnNextToRegister").hide();

    $("#chkExceptDepositFeeNotRetrieve").attr("checked", false);
    $("#chkDepositFeeNotRetrieve").attr("checked", false);

    $("#dtpExceptDepositFeeTaxInvoiceDate").val("");
    $("#dtpExceptDepositFeeCreditNoteDate").val("");
    $("#dtpDepositFeeTaxInvoiceDate").val("");
    $("#dtpDepositFeeCreditNoteDate").val("");

    $("#dtpExceptDepositFeeTaxInvoiceDate").val("");        //Modified by budd 
    $("#dtpExceptDepositFeeCreditNoteDate").val("");
    $("#dtpDepositFeeTaxInvoiceDate").val("");
    $("#dtpDepositFeeCreditNoteDate").val("");

    $("#cboExceptDepositFeeTaxInvoiceBillingType").val("");
    $("#cboDepositFeeTaxInvoiceBillingType").val("");

    $("#txtCancelCreditNoteNo").val("");

    $("#txtExceptDepositFeeTaxInvoiceNoForCreditNote").val("");
    $("#txtExceptDepositFeeBillingCode1").val("");
    $("#txtExceptDepositFeeBillingCode2").val("");
    $("#txtExceptDepositFeeBillingClientNameEN").val("");
    $("#txtExceptDepositFeeBillingClientNameLC").val("");
    $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").val("");
    $("#txtExceptDepositFeeTaxInvoiceAmount").val("");
    $("#txtExceptDepositFeeAccumulatedPaymentAmount").val("");
    $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").val("");
    $("#txtExceptDepositFeeCreditNoteAmount").val("");
    $("#txtExceptDepositFeeApproveNo").val("");
    $("#txtaExceptDepositFeeIssueReason").val("");

    $("#txtDepositFeeTaxInvoiceNoForCreditNote").val("");
    $("#txtDepositFeeBillingCode1").val("");
    $("#txtDepositFeeBillingCode2").val("");
    $("#txtDepositFeeBalanceOfDepositFee").val("");
    $("#txtDepositFeeBillingClientNameEN").val("");
    $("#txtDepositFeeBillingClientNameLC").val("");
    $("#txtDepositFeeTaxInvoiceAmountIncludeVat").val("");
    $("#txtDepositFeeTaxInvoiceAmount").val("");
    $("#txtDepositFeeAccumulatedPaymentAmount").val("");
    $("#txtDepositFeeCreditNoteAmountIncludeVat").val("");
    $("#txtDepositFeeCreditNoteAmount").val("");
    $("#txtDepositFeeApproveNo").val("");
    $("#txtaDepositFeeIssueReason").val("");

    $("#txtRevenueBillingCode1").val("");
    $("#txtRevenueBillingCode2").val("");
    $("#txtRevenueBalanceOfDepositFee").val("");
    $("#txtRevenueBillingClientNameEN").val("");
    $("#txtRevenueBillingClientNameLC").val("");
    $("#txtRevenueRevenueDepositFee").val("");
    $("#txtRevenueRevenueVatAmount").val("");

    $("#txtRegisterCancelCreditNote").val("");
    $("#txtRegisterCancelBillingCode").val("");
    $("#txtRegisterCancelBillingClientNameEN").val("");
    $("#txtRegisterCancelBillingClientNameLC").val("");
    $("#txtRegisterCancelTaxInvoiceAmountIncludeVat").val("");
    $("#txtRegisterCancelTaxInvoiceAmount").val("");
    $("#txtRegisterCancelAccumulatedPaymentAmount").val("");

    $("#dtpRegisterCancelTaxInvoiceDate").val("");
    $("#dtpRegisterCancelTaxInvoiceDate").val("");
    $("#txtRegisterCancelCreditNoteAmountIncludeVat").val("");
    $("#txtRegisterCancelCreditNoteAmount").val("");
    $("#dtpRegisterCancelCreditNoteDate").val("");
    $("#dtpRegisterCancelCreditNoteDate").val("");
    $("cboRegisterCancelTaxInvoiceBillingType").val("");
    $("txtRegisterCancelApproveNo").val("");
    $("txtaRegisterCancelIssueReason").val("");

    $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
    $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true);

    $("#txtExceptDepositFeeTaxInvoiceAmount").attr("readonly", true);
    $("#txtExceptDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true);

    $("#txtDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
    $("#txtDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true);

    $("#txtDepositFeeTaxInvoiceAmount").attr("readonly", true);
    $("#txtDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true);

    // clear currency combobox selected value
    // add by jirawat jannet @ 2016-10-20
    $('#txtExceptDepositFeeTaxInvoiceAmountIncludeVat').SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
    $('#txtExceptDepositFeeTaxInvoiceAmount').SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
    $('#txtExceptDepositFeeAccumulatedPaymentAmount').SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
    $('#txtExceptDepositFeeCreditNoteAmountIncludeVat').SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
    $('#txtExceptDepositFeeCreditNoteAmount').SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
    $('#txtRegisterCancelTaxInvoiceAmountIncludeVat').SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
    $('#txtRegisterCancelTaxInvoiceAmount').SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
    $('#txtRegisterCancelAccumulatedPaymentAmount').SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
    $('#txtRegisterCancelCreditNoteAmountIncludeVat').SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
    $('#txtRegisterCancelCreditNoteAmount').SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
    $('#txtDepositFeeBalanceOfDepositFee').SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
    $('#txtDepositFeeTaxInvoiceAmountIncludeVat').SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
    $('#txtDepositFeeTaxInvoiceAmount').SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
    $('#txtDepositFeeAccumulatedPaymentAmount').SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
    $('#txtDepositFeeCreditNoteAmountIncludeVat').SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
    $('#txtDepositFeeCreditNoteAmount').SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
    $('#txtRevenueBalanceOfDepositFee').SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
    $('#txtRevenueRevenueDepositFee').SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
    $('#txtRevenueRevenueVatAmount').SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);

    DeleteAllRow(grdCancelCreditNote);

    verModeRadio1 = conModeRadio1optRegisterRefundCreditNoteDepositFee;


    ajax_method.CallScreenController("/Income/ICS070_CheckOperate", "",
                function (result, controls, isWarning) {
                    if (result != undefined) {
                        if (result == '1') {

                            $("#optRegisterRefundCreditNoteExceptDepositFee").attr("disabled", false);
                            $("#optRegisterDecreasedCreditNote").attr("disabled", false);
                            $("#optRegisterRefundCreditNoteDepositFee").attr("disabled", false);
                            $("#optRevenueDepositFee").attr("disabled", false);

                        } else {

                            $("#optRegisterRefundCreditNoteExceptDepositFee").attr("disabled", true);
                            $("#optRegisterDecreasedCreditNote").attr("disabled", true);
                            $("#optRegisterRefundCreditNoteDepositFee").attr("disabled", true);
                            $("#optRevenueDepositFee").attr("disabled", true);
                            $("#optCancelCreditNote").attr("checked", true);
                            verModeRadio1 = conModeRadio1optCancelCreditNote;
                            $("#txtCancelCreditNoteNo").attr("readonly", false);
                        }
                    }
                    else {

                        $("#optRegisterRefundCreditNoteExceptDepositFee").attr("disabled", true);
                        $("#optRegisterDecreasedCreditNote").attr("disabled", true);
                        $("#optRegisterRefundCreditNoteDepositFee").attr("disabled", true);
                        $("#optRevenueDepositFee").attr("disabled", true);
                        $("#optCancelCreditNote").attr("checked", true);
                        verModeRadio1 = conModeRadio1optCancelCreditNote;
                        $("#txtCancelCreditNoteNo").attr("readonly", false);
                    }



                    ajax_method.CallScreenController("/Income/ICS070_CheckCancelCreditNote", "",
                    function (result, controls, isWarning) {
                        if (result != undefined) {
                            if (result == '1') {
                                //$("#optCancelCreditNote").attr("disabled", false);
                                //$("#txtCancelCreditNoteNo").attr("readonly", false);
                            } else {
                                $("#optCancelCreditNote").attr("disabled", true);
                                $("#txtCancelCreditNoteNo").attr("readonly", true);
                            }
                        }
                        else {
                            $("#optCancelCreditNote").attr("disabled", true);
                            $("#txtCancelCreditNoteNo").attr("readonly", true);
                            //VaridateCtrl(controls, controls);
                        }
                    });

                });

}


function ClearScreenByMode(intMode) {

    if (intMode == conModeRadio1optRegisterRefundCreditNoteExceptDepositFee) {

        $("#chkExceptDepositFeeNotRetrieve").attr("checked", false); //

        $("#dtpExceptDepositFeeTaxInvoiceDate").val("");
        $("#dtpExceptDepositFeeCreditNoteDate").val("");

        $("#cboExceptDepositFeeTaxInvoiceBillingType").val("");

        $("#txtExceptDepositFeeTaxInvoiceNoForCreditNote").val("");
        $("#txtExceptDepositFeeBillingCode1").val("");
        $("#txtExceptDepositFeeBillingCode2").val("");
        $("#txtExceptDepositFeeBillingClientNameEN").val("");
        $("#txtExceptDepositFeeBillingClientNameLC").val("");
        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").val("");
        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
        $("#txtExceptDepositFeeTaxInvoiceAmount").val("");
        $("#txtExceptDepositFeeTaxInvoiceAmount").SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
        $("#txtExceptDepositFeeAccumulatedPaymentAmount").val("");
        $("#txtExceptDepositFeeAccumulatedPaymentAmount").SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
        $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").val("");
        $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
        $("#txtExceptDepositFeeCreditNoteAmount").val("");
        $("#txtExceptDepositFeeCreditNoteAmount").SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
        $("#txtExceptDepositFeeApproveNo").val("");
        $("#txtaExceptDepositFeeIssueReason").val("");
    }
    else if (intMode == conModeRadio1optRegisterDecreasedCreditNote) {

        $("#chkExceptDepositFeeNotRetrieve").attr("checked", false); //

        $("#dtpExceptDepositFeeTaxInvoiceDate").val("");
        $("#dtpExceptDepositFeeCreditNoteDate").val("");

        $("#cboExceptDepositFeeTaxInvoiceBillingType").val("");

        $("#txtExceptDepositFeeTaxInvoiceNoForCreditNote").val("");
        $("#txtExceptDepositFeeBillingCode1").val("");
        $("#txtExceptDepositFeeBillingCode2").val("");
        $("#txtExceptDepositFeeBillingClientNameEN").val("");
        $("#txtExceptDepositFeeBillingClientNameLC").val("");
        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").val("");
        $("#txtExceptDepositFeeTaxInvoiceAmount").val("");
        $("#txtExceptDepositFeeAccumulatedPaymentAmount").val("");
        $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").val("");
        $("#txtExceptDepositFeeCreditNoteAmount").val("");
        $("#txtExceptDepositFeeApproveNo").val("");
        $("#txtaExceptDepositFeeIssueReason").val("");

    }
    else if (intMode == conModeRadio1optRegisterRefundCreditNoteDepositFee) {

        $("#chkDepositFeeNotRetrieve").attr("checked", false); //

        $("#dtpDepositFeeTaxInvoiceDate").val("");
        $("#dtpDepositFeeCreditNoteDate").val("");

        $("#cboDepositFeeTaxInvoiceBillingType").val("");

        $("#txtDepositFeeTaxInvoiceNoForCreditNote").val("");
        $("#txtDepositFeeBillingCode1").val("");
        $("#txtDepositFeeBillingCode2").val("");
        $("#txtDepositFeeBalanceOfDepositFee").val("");
        $("#txtDepositFeeBalanceOfDepositFee").SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
        $("#txtDepositFeeBillingClientNameEN").val(""); 
        $("#txtDepositFeeBillingClientNameLC").val("");
        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").val("");
        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
        $("#txtDepositFeeTaxInvoiceAmount").val("");
        $("#txtDepositFeeTaxInvoiceAmount").SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
        $("#txtDepositFeeAccumulatedPaymentAmount").val("");
        $("#txtDepositFeeAccumulatedPaymentAmount").SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
        $("#txtDepositFeeCreditNoteAmountIncludeVat").val("");
        $("#txtDepositFeeCreditNoteAmountIncludeVat").SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
        $("#txtDepositFeeCreditNoteAmount").val("");
        $("#txtDepositFeeCreditNoteAmount").SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
        $("#txtDepositFeeApproveNo").val("");
        $("#txtaDepositFeeIssueReason").val("");

    }
    else if (intMode == conModeRadio1optRevenueDepositFee) {

        $("#txtRevenueBillingCode1").val("");
        $("#txtRevenueBillingCode2").val("");
        $("#txtRevenueBalanceOfDepositFee").val("");
        $("#txtRevenueBalanceOfDepositFee").SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
        $("#txtRevenueBillingClientNameEN").val("");
        $("#txtRevenueBillingClientNameLC").val("");
        $("#txtRevenueRevenueDepositFee").val("");
        $("#txtRevenueRevenueDepositFee").SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
        $("#txtRevenueRevenueVatAmount").val("");
        $("#txtRevenueRevenueVatAmount").SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);

    }
    else if (intMode == conModeRadio1optCancelCreditNote) {

        $("#txtRegisterCancelCreditNote").val("");
        $("#txtRegisterCancelBillingCode").val("");
        $("#txtRegisterCancelBillingClientNameEN").val("");
        $("#txtRegisterCancelBillingClientNameLC").val("");
        $("#txtRegisterCancelTaxInvoiceAmountIncludeVat").val("");
        $("#txtRegisterCancelTaxInvoiceAmountIncludeVat").SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
        $("#txtRegisterCancelTaxInvoiceAmount").val("");
        $("#txtRegisterCancelTaxInvoiceAmount").SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
        $("#txtRegisterCancelAccumulatedPaymentAmount").val("");
        $("#txtRegisterCancelAccumulatedPaymentAmount").SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);

        $("#dtpRegisterCancelTaxInvoiceDate").val("");
        $("#dtpRegisterCancelTaxInvoiceDate").val("");
        $("#txtRegisterCancelCreditNoteAmountIncludeVat").val("");
        $("#txtRegisterCancelCreditNoteAmountIncludeVat").SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
        $("#txtRegisterCancelCreditNoteAmount").val("");
        $("#txtRegisterCancelCreditNoteAmount").SetNumericCurrency(ICS070_ViewBag.C_CURRENCY_LOCAL);
        $("#dtpRegisterCancelCreditNoteDate").val("");
        $("#dtpRegisterCancelCreditNoteDate").val("");
        $("cboRegisterCancelTaxInvoiceBillingType").val("");
        $("txtRegisterCancelApproveNo").val("");
        $("txtaRegisterCancelIssueReason").val("");
    }

}


function btn_SelectProcess_click() {

    if (verModeRadio1 == conModeRadio1optCancelCreditNote) {

        var obj = GetUserAdjustData();
        ajax_method.CallScreenController("/Income/ICS070_CancelCreditNoteRetrieveData", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                $("#txtCancelCreditNoteNo").val(result.doGetCreditNote.CreditNoteNo);
                $("#txtRegisterCancelCreditNote").val(result.doGetCreditNote.TaxInvoiceNo);
                $("#txtRegisterCancelBillingCode").val(result.doGetCreditNote.BillingCodeShort);
                $("#txtRegisterCancelBillingClientNameEN").val(result.doGetCreditNote.BillingClientNameEN);
                $("#txtRegisterCancelBillingClientNameLC").val(result.doGetCreditNote.BillingClientNameLC);

                // Add by Jirawat Jannet @ 2016-10-26

                var TaxInvoiceAmount = 0;
                var TaxInvoiceVATAmount = 0;
                var CreditAmountIncVAT = 0;
                var CreditVATAmount = 0;
                
                //TaxInvoiceAmount
                if (result.doGetCreditNote.TaxInvoiceAmountCurrencyType == ICS070_ViewBag.C_CURRENCY_LOCAL) {
                    TaxInvoiceAmount = result.doGetCreditNote.TaxInvoiceAmount;
                } else if (result.doGetCreditNote.TaxInvoiceAmountCurrencyType == ICS070_ViewBag.C_CURRENCY_US) {
                    TaxInvoiceAmount = result.doGetCreditNote.TaxInvoiceAmountUsd;
                }
                if (TaxInvoiceAmount == null) TaxInvoiceAmount = 0;

                //TaxInvoiceVATAmount
                if (result.doGetCreditNote.TaxInvoiceVATAmountCurrencyType == ICS070_ViewBag.C_CURRENCY_LOCAL) {
                    TaxInvoiceVATAmount = result.doGetCreditNote.TaxInvoiceVATAmount;
                } else if (result.doGetCreditNote.TaxInvoiceVATAmountCurrencyType == ICS070_ViewBag.C_CURRENCY_US) {
                    TaxInvoiceVATAmount = result.doGetCreditNote.TaxInvoiceVATAmountUsd;
                }
                if (TaxInvoiceVATAmount == null) TaxInvoiceVATAmount = 0;

                //CreditAmountIncVAT
                if (result.doGetCreditNote.CreditAmountIncVATCurrencyType == ICS070_ViewBag.C_CURRENCY_LOCAL) {
                    CreditAmountIncVAT = result.doGetCreditNote.CreditAmountIncVAT;
                } else if (result.doGetCreditNote.CreditAmountIncVATCurrencyType == ICS070_ViewBag.C_CURRENCY_US) {
                    CreditAmountIncVAT = result.doGetCreditNote.CreditAmountIncVATUsd;
                }
                if (CreditAmountIncVAT == null) CreditAmountIncVAT = 0;

                //CreditVATAmount
                if (result.doGetCreditNote.CreditVATAmountCurrencyType == ICS070_ViewBag.C_CURRENCY_LOCAL) {
                    CreditVATAmount = result.doGetCreditNote.CreditVATAmount;
                } else if (result.doGetCreditNote.CreditVATAmountCurrencyType == ICS070_ViewBag.C_CURRENCY_US) {
                    CreditVATAmount = result.doGetCreditNote.CreditVATAmountUsd;
                }
                if (CreditVATAmount == null) CreditVATAmount = 0;

                // End add

                $("#txtRegisterCancelTaxInvoiceAmountIncludeVat").val(number_format(parseFloat(TaxInvoiceAmount), 2, '.', ','));
                $("#txtRegisterCancelTaxInvoiceAmountIncludeVat").NumericCurrency().val(result.doGetCreditNote.TaxInvoiceAmountCurrencyType);

                $("#txtRegisterCancelTaxInvoiceAmount").val(number_format(parseFloat(TaxInvoiceVATAmount), 2, '.', ','));
                $("#txtRegisterCancelTaxInvoiceAmount").NumericCurrency().val(result.doGetCreditNote.TaxInvoiceVATAmountCurrencyType);

                //$("#txtExceptDepositFeeAccumulatedPaymentAmount
                $("#dtpRegisterCancelTaxInvoiceDate").val(ConvertDateToTextFormat(ConvertDateObject(result.doGetCreditNote.TaxInvoiceDate)));

                $("#txtRegisterCancelCreditNoteAmountIncludeVat").val(number_format(parseFloat(CreditAmountIncVAT), 2, '.', ','));
                $("#txtRegisterCancelCreditNoteAmountIncludeVat").NumericCurrency().val(result.doGetCreditNote.CreditAmountIncVATCurrencyType);

                $("#txtRegisterCancelCreditNoteAmount").val(number_format(parseFloat(CreditVATAmount), 2, '.', ','));
                $("#txtRegisterCancelCreditNoteAmount").NumericCurrency().val(result.doGetCreditNote.CreditVATAmountCurrencyType);

                $("#dtpRegisterCancelCreditNoteDate").val(ConvertDateToTextFormat(ConvertDateObject(result.doGetCreditNote.CreditNoteDate)));
                $("#cboRegisterCancelTaxInvoiceBillingType").val(result.doGetCreditNote.BillingTypeCode);
                //$("#txtRegisterCancelFeeApproveNo").val(result.doGetCreditNote.ApproveNo);        //Comment by budd
                $("#txtRegisterCancelApproveNo").val(result.doGetCreditNote.ApproveNo);
                $("#txtaRegisterCancelIssueReason").val(result.doGetCreditNote.IssueReason);

                setFormMode(conModeConfirm);
                //setVisableTable(conModeRadio1optRegisterRefundCreditNoteExceptDepositFee);
                setVisableTable(verModeRadio1);
                setEnabledObject(conModeRadio1optRegisterRefundCreditNoteExceptDepositFee);
                // fix to this mode only
                $("#divCancelCreditNote").hide();

                $("#divRegisterCancelCreditNoteInformation").SetViewMode(true);
                $("#divRegisterCancelCreditNoteInformation").ResetToNormalControl(false);

                $("#optRegisterRefundCreditNoteExceptDepositFee").attr("disabled", true);
                $("#optRegisterDecreasedCreditNote").attr("disabled", true);
                $("#optRegisterRefundCreditNoteDepositFee").attr("disabled", true);
                $("#optRevenueDepositFee").attr("disabled", true);
                $("#optCancelCreditNote").attr("disabled", true);

                $("#btnSelectProcess").attr("disabled", true);
            }
            else {
                // after show Error MSG Layer on Right on Screen then go to Init Mode
                VaridateCtrl(controls, controls);
            }
        });

    } else {

        setFormMode(conModeView);
        setVisableTable(verModeRadio1);
        setEnabledObject(verModeRadio1);

        $("#optRegisterRefundCreditNoteExceptDepositFee").attr("disabled", true);
        $("#optRegisterDecreasedCreditNote").attr("disabled", true);
        $("#optRegisterRefundCreditNoteDepositFee").attr("disabled", true);
        $("#optRevenueDepositFee").attr("disabled", true);
        $("#optCancelCreditNote").attr("disabled", true);

        $("#btnSelectProcess").attr("disabled", true);
    }
}


// Enable Obj On Screen

function setResetEnabledObject() {


    $("#btnExceptDepositFeeRetrieveBillingInformation").attr("disabled", true);
    $("#btnExceptDepositFeeRetrieve").attr("disabled", true);
    $("#btnExceptDepositFeeAdd").attr("disabled", true);
    $("#btnExceptDepositFeeCancel").attr("disabled", true);
    $("#btnDepositFeeRetrieveBillingInformation").attr("disabled", true);
    $("#btnDepositFeeRetrieve").attr("disabled", true);
    $("#btnDepositFeeAdd").attr("disabled", true);
    $("#btnDepositFeeCancel").attr("disabled", true);
    $("#btnRevenueRetrieve").attr("disabled", true);
    $("#btnRevenueAdd").attr("disabled", true);
    $("#btnRevenueCancel").attr("disabled", true);
    $("#btnNextToRegister").attr("disabled", true);
    $("#btnNextToRegister").hide();

    $("#chkExceptDepositFeeNotRetrieve").attr("disabled", true);
    $("#chkDepositFeeNotRetrieve").attr("disabled", true);

    $("#dtpExceptDepositFeeTaxInvoiceDate").attr("disabled", true);
    $("#dtpExceptDepositFeeCreditNoteDate").attr("disabled", true);
    $("#dtpDepositFeeTaxInvoiceDate").attr("disabled", true);
    $("#dtpDepositFeeCreditNoteDate").attr("disabled", true);

    $("#cboExceptDepositFeeTaxInvoiceBillingType").attr("disabled", true);
    $("#divExceptDepositFeeTaxInvoiceBillingTypeRequired").hide();
    $("#cboDepositFeeTaxInvoiceBillingType").attr("disabled", true);

    $("#txtCancelCreditNoteNo").attr("readonly", true);

    $("#txtExceptDepositFeeTaxInvoiceNoForCreditNote").attr("readonly", true);
    $("#txtExceptDepositFeeBillingCode1").attr("readonly", true);
    $("#txtExceptDepositFeeBillingCode2").attr("readonly", true);
    $("#txtExceptDepositFeeBillingClientNameEN").attr("readonly", true);
    $("#txtExceptDepositFeeBillingClientNameLC").attr("readonly", true);

    $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
    $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true);

    $("#txtExceptDepositFeeTaxInvoiceAmount").attr("readonly", true);
    $("#txtExceptDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true);

    $("#txtExceptDepositFeeAccumulatedPaymentAmount").attr("readonly", true);
    $("#txtExceptDepositFeeAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true);

    $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").attr("readonly", true);
    $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", true);

    $("#txtExceptDepositFeeCreditNoteAmount").attr("readonly", true);
    $("#txtExceptDepositFeeCreditNoteAmount").NumericCurrency().attr("disabled", true);

    $("#txtExceptDepositFeeApproveNo").attr("readonly", true);
    $("#txtaExceptDepositFeeIssueReason").attr("readonly", true);

    $("#txtDepositFeeTaxInvoiceNoForCreditNote").attr("readonly", true);
    $("#txtDepositFeeBillingCode1").attr("readonly", true);
    $("#txtDepositFeeBillingCode2").attr("readonly", true);

    $("#txtDepositFeeBalanceOfDepositFee").attr("readonly", true);
    $("#txtDepositFeeBalanceOfDepositFee").NumericCurrency().attr("disabled", true);

    $("#txtDepositFeeBillingClientNameEN").attr("readonly", true);
    $("#txtDepositFeeBillingClientNameLC").attr("readonly", true);

    $("#txtDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
    $("#txtDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true);

    $("#txtDepositFeeTaxInvoiceAmount").attr("readonly", true);
    $("#txtDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true);

    $("#txtDepositFeeAccumulatedPaymentAmount").attr("readonly", true);
    $("#txtDepositFeeAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true);

    $("#txtDepositFeeCreditNoteAmountIncludeVat").attr("readonly", true);
    $("#txtDepositFeeCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", true);

    $("#txtDepositFeeCreditNoteAmount").attr("readonly", true);
    $("#txtDepositFeeCreditNoteAmount").NumericCurrency().attr("disabled", true);

    $("#txtDepositFeeApproveNo").attr("readonly", true);
    $("#txtaDepositFeeIssueReason").attr("readonly", true);

    $("#txtRevenueBillingCode1").attr("readonly", true);
    $("#txtRevenueBillingCode2").attr("readonly", true);

    $("#txtRevenueBalanceOfDepositFee").attr("readonly", true);
    $("#txtRevenueBalanceOfDepositFee").NumericCurrency().attr("disabled", true);

    $("#txtRevenueBillingClientNameEN").attr("readonly", true);
    $("#txtRevenueBillingClientNameLC").attr("readonly", true);

    $("#txtRevenueRevenueDepositFee").attr("readonly", true);
    $("#txtRevenueRevenueDepositFee").NumericCurrency().attr("disabled", true);

    $("#txtRevenueRevenueVatAmount").attr("readonly", true);
    $("#txtRevenueRevenueVatAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

    $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
    $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true);

    $("#txtExceptDepositFeeTaxInvoiceAmount").attr("readonly", true);
    $("#txtExceptDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true);

    $("#txtDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
    $("#txtDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-1-18

    $("#txtDepositFeeTaxInvoiceAmount").attr("readonly", true);
    $("#txtDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-1-18

    $("#txtRegisterCancelCreditNote").attr("readonly", true);
    $("#txtRegisterCancelBillingCode").attr("readonly", true);
    $("#txtRegisterCancelBillingClientNameEN").attr("readonly", true);
    $("#txtRegisterCancelBillingClientNameLC").attr("readonly", true);

    $("#txtRegisterCancelTaxInvoiceAmountIncludeVat").attr("readonly", true);
    $("#txtRegisterCancelTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

    $("#txtRegisterCancelTaxInvoiceAmount").attr("readonly", true);
    $("#txtRegisterCancelTaxInvoiceAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

    $("#txtRegisterCancelAccumulatedPaymentAmount").attr("readonly", true);
    $("#txtRegisterCancelAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

    $("#dtpRegisterCancelTaxInvoiceDate").attr("readonly", true);

    $("#txtRegisterCancelCreditNoteAmountIncludeVat").attr("readonly", true);
    $("#txtRegisterCancelCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

    $("#txtRegisterCancelCreditNoteAmount").attr("readonly", true);
    $("#txtRegisterCancelCreditNoteAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

    $("#dtpRegisterCancelCreditNoteDate").attr("readonly", true);
    $("#cboRegisterCancelTaxInvoiceBillingType").attr("readonly", true);

    $("#txtRegisterCancelApproveNo").attr("readonly", true);
    $("#txtaRegisterCancelIssueReason").attr("readonly", true);

}

function setEnabledObject(intMode) {

    if (intMode == conModeRadio1optRegisterRefundCreditNoteExceptDepositFee) {

        $("#btnExceptDepositFeeRetrieveBillingInformation").attr("disabled", false); //
        $("#btnExceptDepositFeeRetrieve").attr("disabled", true); //
        $("#btnExceptDepositFeeAdd").attr("disabled", true);
        $("#btnExceptDepositFeeCancel").attr("disabled", true);
        $("#btnDepositFeeRetrieveBillingInformation").attr("disabled", true);
        $("#btnDepositFeeRetrieve").attr("disabled", true);
        $("#btnDepositFeeAdd").attr("disabled", true);
        $("#btnDepositFeeCancel").attr("disabled", true);
        $("#btnRevenueRetrieve").attr("disabled", true);
        $("#btnRevenueAdd").attr("disabled", true);
        $("#btnRevenueCancel").attr("disabled", true);
        $("#btnNextToRegister").attr("disabled", true);
        $("#btnNextToRegister").hide();

        $("#chkExceptDepositFeeNotRetrieve").attr("disabled", false); //
        $("#chkDepositFeeNotRetrieve").attr("disabled", true);

        $("#dtpExceptDepositFeeTaxInvoiceDate").attr("disabled", false); //
        $("#dtpExceptDepositFeeCreditNoteDate").attr("disabled", false); //
        $("#dtpDepositFeeTaxInvoiceDate").attr("disabled", true);
        $("#dtpDepositFeeCreditNoteDate").attr("disabled", true);

        $("#cboExceptDepositFeeTaxInvoiceBillingType").attr("disabled", true);
        $("#divExceptDepositFeeTaxInvoiceBillingTypeRequired").hide();
        $("#cboDepositFeeTaxInvoiceBillingType").attr("disabled", true);

        //Add by Budd
        $("#cboRegisterCancelTaxInvoiceBillingType").attr("disabled", true);
        //End add

        $("#txtCancelCreditNoteNo").attr("readonly", true);

        $("#txtExceptDepositFeeTaxInvoiceNoForCreditNote").attr("readonly", false); //
        $("#txtExceptDepositFeeBillingCode1").attr("readonly", true);
        $("#txtExceptDepositFeeBillingCode2").attr("readonly", true);
        $("#txtExceptDepositFeeBillingClientNameEN").attr("readonly", true);
        $("#txtExceptDepositFeeBillingClientNameLC").attr("readonly", true);

        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", false); //;
        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", false);

        $("#txtExceptDepositFeeTaxInvoiceAmount").attr("readonly", false); //
        $("#txtExceptDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", false);

        $("#txtExceptDepositFeeAccumulatedPaymentAmount").attr("readonly", true);
        $("#txtExceptDepositFeeAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").attr("readonly", false);
        $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", false);

        $("#txtExceptDepositFeeCreditNoteAmount").attr("readonly", false);
        $("#txtExceptDepositFeeCreditNoteAmount").NumericCurrency().attr("disabled", false);

        $("#txtExceptDepositFeeApproveNo").attr("readonly", false); //
        $("#txtaExceptDepositFeeIssueReason").attr("readonly", false); //

        $("#txtDepositFeeTaxInvoiceNoForCreditNote").attr("readonly", true);
        $("#txtDepositFeeBillingCode1").attr("readonly", true);
        $("#txtDepositFeeBillingCode2").attr("readonly", true);

        $("#txtDepositFeeBalanceOfDepositFee").attr("readonly", true);
        $("#txtDepositFeeBalanceOfDepositFee").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeBillingClientNameEN").attr("readonly", true);
        $("#txtDepositFeeBillingClientNameLC").attr("readonly", true);

        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-1-18

        $("#txtDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-1-18

        $("#txtDepositFeeAccumulatedPaymentAmount").attr("readonly", true);
        $("#txtDepositFeeAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeCreditNoteAmountIncludeVat").attr("readonly", false);
        $("#txtDepositFeeCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", false);

        $("#txtDepositFeeCreditNoteAmount").attr("readonly", false);
        $("#txtDepositFeeCreditNoteAmount").NumericCurrency().attr("disabled", false);

        $("#txtDepositFeeApproveNo").attr("readonly", true);
        $("#txtaDepositFeeIssueReason").attr("readonly", true);

        $("#txtRevenueBillingCode1").attr("readonly", true);
        $("#txtRevenueBillingCode2").attr("readonly", true);

        $("#txtRevenueBalanceOfDepositFee").attr("readonly", true);
        $("#txtRevenueBalanceOfDepositFee").NumericCurrency().attr("disabled", true);

        $("#txtRevenueBillingClientNameEN").attr("readonly", true);
        $("#txtRevenueBillingClientNameLC").attr("readonly", true);

        $("#txtRevenueRevenueDepositFee").attr("readonly", true);
        $("#txtRevenueRevenueDepositFee").NumericCurrency().attr("disabled", true);

        $("#txtRevenueRevenueVatAmount").attr("readonly", true);
        $("#txtRevenueRevenueVatAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-1-18

        $("#txtDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-1-18

        $("#txtRegisterCancelCreditNote").attr("readonly", true);
        $("#txtRegisterCancelBillingCode").attr("readonly", true);
        $("#txtRegisterCancelBillingClientNameEN").attr("readonly", true);
        $("#txtRegisterCancelBillingClientNameLC").attr("readonly", true);

        $("#txtRegisterCancelTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtRegisterCancelTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtRegisterCancelTaxInvoiceAmount").attr("readonly", true);
        $("#txtRegisterCancelTaxInvoiceAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtRegisterCancelAccumulatedPaymentAmount").attr("readonly", true);
        $("#txtRegisterCancelAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#dtpRegisterCancelTaxInvoiceDate").attr("readonly", true);

        $("#txtRegisterCancelCreditNoteAmountIncludeVat").attr("readonly", true);
        $("#txtRegisterCancelCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtRegisterCancelCreditNoteAmount").attr("readonly", true);
        $("#txtRegisterCancelCreditNoteAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#dtpRegisterCancelCreditNoteDate").attr("readonly", true);
        $("#cboRegisterCancelTaxInvoiceBillingType").attr("readonly", true);

        $("#txtRegisterCancelApproveNo").attr("readonly", true);
        $("#txtaRegisterCancelIssueReason").attr("readonly", true);


    }
    else if (intMode == conModeRadio1optRegisterDecreasedCreditNote) {

        $("#btnExceptDepositFeeRetrieveBillingInformation").attr("disabled", false); //
        $("#btnExceptDepositFeeRetrieve").attr("disabled", true); //
        $("#btnExceptDepositFeeAdd").attr("disabled", true);
        $("#btnExceptDepositFeeCancel").attr("disabled", true);
        $("#btnDepositFeeRetrieveBillingInformation").attr("disabled", true);
        $("#btnDepositFeeRetrieve").attr("disabled", true);
        $("#btnDepositFeeAdd").attr("disabled", true);
        $("#btnDepositFeeCancel").attr("disabled", true);
        $("#btnRevenueRetrieve").attr("disabled", true);
        $("#btnRevenueAdd").attr("disabled", true);
        $("#btnRevenueCancel").attr("disabled", true);
        $("#btnNextToRegister").attr("disabled", true);
        $("#btnNextToRegister").hide();

        $("#chkExceptDepositFeeNotRetrieve").attr("disabled", false); //
        $("#chkDepositFeeNotRetrieve").attr("disabled", true);

        $("#dtpExceptDepositFeeTaxInvoiceDate").attr("disabled", false); //
        $("#dtpExceptDepositFeeCreditNoteDate").attr("disabled", false); //
        $("#dtpDepositFeeTaxInvoiceDate").attr("disabled", true);
        $("#dtpDepositFeeCreditNoteDate").attr("disabled", true);

        $("#cboExceptDepositFeeTaxInvoiceBillingType").attr("disabled", true);
        $("#divExceptDepositFeeTaxInvoiceBillingTypeRequired").hide();
        $("#cboDepositFeeTaxInvoiceBillingType").attr("disabled", true);

        $("#txtCancelCreditNoteNo").attr("readonly", true);

        $("#txtExceptDepositFeeTaxInvoiceNoForCreditNote").attr("readonly", false); //
        $("#txtExceptDepositFeeBillingCode1").attr("readonly", true);
        $("#txtExceptDepositFeeBillingCode2").attr("readonly", true);
        $("#txtExceptDepositFeeBillingClientNameEN").attr("readonly", true);
        $("#txtExceptDepositFeeBillingClientNameLC").attr("readonly", true);

        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeAccumulatedPaymentAmount").attr("readonly", true);
        $("#txtExceptDepositFeeAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").attr("readonly", false); //
        $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", false);

        $("#txtExceptDepositFeeCreditNoteAmount").attr("readonly", false); //
        $("#txtExceptDepositFeeCreditNoteAmount").NumericCurrency().attr("disabled", false);

        $("#txtExceptDepositFeeApproveNo").attr("readonly", false); //
        $("#txtaExceptDepositFeeIssueReason").attr("readonly", false); //

        $("#txtDepositFeeTaxInvoiceNoForCreditNote").attr("readonly", true);
        $("#txtDepositFeeBillingCode1").attr("readonly", true);
        $("#txtDepositFeeBillingCode2").attr("readonly", true);

        $("#txtDepositFeeBalanceOfDepositFee").attr("readonly", true);
        $("#txtDepositFeeBalanceOfDepositFee").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeBillingClientNameEN").attr("readonly", true);
        $("#txtDepositFeeBillingClientNameLC").attr("readonly", true);

        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-1-18

        $("#txtDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-1-18

        $("#txtDepositFeeAccumulatedPaymentAmount").attr("readonly", true);
        $("#txtDepositFeeAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeCreditNoteAmountIncludeVat").attr("readonly", false);
        $("#txtDepositFeeCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", false);

        $("#txtDepositFeeCreditNoteAmount").attr("readonly", false);
        $("#txtDepositFeeCreditNoteAmount").NumericCurrency().attr("disabled", false);

        $("#txtDepositFeeApproveNo").attr("readonly", true);
        $("#txtaDepositFeeIssueReason").attr("readonly", true);

        $("#txtRevenueBillingCode1").attr("readonly", true);
        $("#txtRevenueBillingCode2").attr("readonly", true);

        $("#txtRevenueBalanceOfDepositFee").attr("readonly", true);
        $("#txtRevenueBalanceOfDepositFee").NumericCurrency().attr("disabled", true);

        $("#txtRevenueBillingClientNameEN").attr("readonly", true);
        $("#txtRevenueBillingClientNameLC").attr("readonly", true);

        $("#txtRevenueRevenueDepositFee").attr("readonly", true);
        $("#txtRevenueRevenueDepositFee").NumericCurrency().attr("disabled", true);

        $("#txtRevenueRevenueVatAmount").attr("readonly", true);
        $("#txtRevenueRevenueVatAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-1-18

        $("#txtDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-1-18

        $("#txtRegisterCancelCreditNote").attr("readonly", true);
        $("#txtRegisterCancelBillingCode").attr("readonly", true);
        $("#txtRegisterCancelBillingClientNameEN").attr("readonly", true);
        $("#txtRegisterCancelBillingClientNameLC").attr("readonly", true);

        $("#txtRegisterCancelTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtRegisterCancelTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtRegisterCancelTaxInvoiceAmount").attr("readonly", true);
        $("#txtRegisterCancelTaxInvoiceAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtRegisterCancelAccumulatedPaymentAmount").attr("readonly", true);
        $("#txtRegisterCancelAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#dtpRegisterCancelTaxInvoiceDate").attr("readonly", true);

        $("#txtRegisterCancelCreditNoteAmountIncludeVat").attr("readonly", true);
        $("#txtRegisterCancelCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtRegisterCancelCreditNoteAmount").attr("readonly", true);
        $("#txtRegisterCancelCreditNoteAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#dtpRegisterCancelCreditNoteDate").attr("readonly", true);
        $("#cboRegisterCancelTaxInvoiceBillingType").attr("readonly", true);

        $("#txtRegisterCancelApproveNo").attr("readonly", true);
        $("#txtaRegisterCancelIssueReason").attr("readonly", true);

    }
    else if (intMode == conModeRadio1optRegisterRefundCreditNoteDepositFee) {

        $("#btnExceptDepositFeeRetrieveBillingInformation").attr("disabled", true);
        $("#btnExceptDepositFeeRetrieve").attr("disabled", true);
        $("#btnExceptDepositFeeAdd").attr("disabled", true);
        $("#btnExceptDepositFeeCancel").attr("disabled", true);
        $("#btnDepositFeeRetrieveBillingInformation").attr("disabled", false); //
        $("#btnDepositFeeRetrieve").attr("disabled", true); //
        $("#btnDepositFeeAdd").attr("disabled", true);
        $("#btnDepositFeeCancel").attr("disabled", true);
        $("#btnRevenueRetrieve").attr("disabled", true);
        $("#btnRevenueAdd").attr("disabled", true);
        $("#btnRevenueCancel").attr("disabled", true);
        $("#btnNextToRegister").attr("disabled", true);
        $("#btnNextToRegister").hide();

        $("#chkExceptDepositFeeNotRetrieve").attr("disabled", true);
        $("#chkDepositFeeNotRetrieve").attr("disabled", false); //

        $("#dtpExceptDepositFeeTaxInvoiceDate").attr("disabled", true);
        $("#dtpExceptDepositFeeCreditNoteDate").attr("disabled", true);
        $("#dtpDepositFeeTaxInvoiceDate").attr("disabled", false); //
        $("#dtpDepositFeeCreditNoteDate").attr("disabled", false); //

        $("#cboExceptDepositFeeTaxInvoiceBillingType").attr("disabled", true);
        $("#divExceptDepositFeeTaxInvoiceBillingTypeRequired").hide();
        $("#cboDepositFeeTaxInvoiceBillingType").attr("disabled", true);

        $("#txtCancelCreditNoteNo").attr("readonly", true);

        $("#txtExceptDepositFeeTaxInvoiceNoForCreditNote").attr("readonly", true);
        $("#txtExceptDepositFeeBillingCode1").attr("readonly", true);
        $("#txtExceptDepositFeeBillingCode2").attr("readonly", true);
        $("#txtExceptDepositFeeBillingClientNameEN").attr("readonly", true);
        $("#txtExceptDepositFeeBillingClientNameLC").attr("readonly", true);

        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeAccumulatedPaymentAmount").attr("readonly", true);
        $("#txtExceptDepositFeeAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").attr("readonly", false);
        $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", false);

        $("#txtExceptDepositFeeCreditNoteAmount").attr("readonly", false);
        $("#txtExceptDepositFeeCreditNoteAmount").NumericCurrency().attr("disabled", false);

        $("#txtExceptDepositFeeApproveNo").attr("readonly", true);
        $("#txtaExceptDepositFeeIssueReason").attr("readonly", true);

        $("#txtDepositFeeTaxInvoiceNoForCreditNote").attr("readonly", false); //
        $("#txtDepositFeeBillingCode1").attr("readonly", true);
        $("#txtDepositFeeBillingCode2").attr("readonly", true);

        $("#txtDepositFeeBalanceOfDepositFee").attr("readonly", true);
        $("#txtDepositFeeBalanceOfDepositFee").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeBillingClientNameEN").attr("readonly", true);
        $("#txtDepositFeeBillingClientNameLC").attr("readonly", true);

        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", false); //
        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", false); // add by Jirawat Jannet @ 2016-1-18

        $("#txtDepositFeeTaxInvoiceAmount").attr("readonly", false); //
        $("#txtDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", false); // add by Jirawat Jannet @ 2016-1-18

        $("#txtDepositFeeAccumulatedPaymentAmount").attr("readonly", true);        //Modified by budd   false --> true
        $("#txtDepositFeeAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeCreditNoteAmountIncludeVat").attr("readonly", false);
        $("#txtDepositFeeCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", false);

        $("#txtDepositFeeCreditNoteAmount").attr("readonly", false);
        $("#txtDepositFeeCreditNoteAmount").NumericCurrency().attr("disabled", false);

        $("#txtDepositFeeApproveNo").attr("readonly", false); //
        $("#txtaDepositFeeIssueReason").attr("readonly", false); //

        $("#txtRevenueBillingCode1").attr("readonly", true);
        $("#txtRevenueBillingCode2").attr("readonly", true);

        $("#txtRevenueBalanceOfDepositFee").attr("readonly", true);
        $("#txtRevenueBalanceOfDepositFee").NumericCurrency().attr("disabled", true);

        $("#txtRevenueBillingClientNameEN").attr("readonly", true);
        $("#txtRevenueBillingClientNameLC").attr("readonly", true);

        $("#txtRevenueRevenueDepositFee").attr("readonly", true);
        $("#txtRevenueRevenueDepositFee").NumericCurrency().attr("disabled", true);

        $("#txtRevenueRevenueVatAmount").attr("readonly", true);
        $("#txtRevenueRevenueVatAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-1-18

        $("#txtDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-1-18

        $("#txtRegisterCancelCreditNote").attr("readonly", true);
        $("#txtRegisterCancelBillingCode").attr("readonly", true);
        $("#txtRegisterCancelBillingClientNameEN").attr("readonly", true);
        $("#txtRegisterCancelBillingClientNameLC").attr("readonly", true);

        $("#txtRegisterCancelTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtRegisterCancelTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtRegisterCancelTaxInvoiceAmount").attr("readonly", true);
        $("#txtRegisterCancelTaxInvoiceAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtRegisterCancelAccumulatedPaymentAmount").attr("readonly", true);
        $("#txtRegisterCancelAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#dtpRegisterCancelTaxInvoiceDate").attr("readonly", true);

        $("#txtRegisterCancelCreditNoteAmountIncludeVat").attr("readonly", true);
        $("#txtRegisterCancelCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtRegisterCancelCreditNoteAmount").attr("readonly", true);
        $("#txtRegisterCancelCreditNoteAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#dtpRegisterCancelCreditNoteDate").attr("readonly", true);
        $("#cboRegisterCancelTaxInvoiceBillingType").attr("readonly", true);

        $("#txtRegisterCancelApproveNo").attr("readonly", true);
        $("#txtaRegisterCancelIssueReason").attr("readonly", true);

    }
    else if (intMode == conModeRadio1optRevenueDepositFee) {

        $("#btnExceptDepositFeeRetrieveBillingInformation").attr("disabled", true);
        $("#btnExceptDepositFeeRetrieve").attr("disabled", true);
        $("#btnExceptDepositFeeAdd").attr("disabled", true);
        $("#btnExceptDepositFeeCancel").attr("disabled", true);
        $("#btnDepositFeeRetrieveBillingInformation").attr("disabled", true);
        $("#btnDepositFeeRetrieve").attr("disabled", true);
        $("#btnDepositFeeAdd").attr("disabled", true);
        $("#btnDepositFeeCancel").attr("disabled", true);
        $("#btnRevenueRetrieve").attr("disabled", false); //
        $("#btnRevenueAdd").attr("disabled", true);
        $("#btnRevenueCancel").attr("disabled", true);
        $("#btnNextToRegister").attr("disabled", true);
        $("#btnNextToRegister").hide();

        $("#chkExceptDepositFeeNotRetrieve").attr("disabled", true);
        $("#chkDepositFeeNotRetrieve").attr("disabled", true);

        $("#dtpExceptDepositFeeTaxInvoiceDate").attr("disabled", true);
        $("#dtpExceptDepositFeeCreditNoteDate").attr("disabled", true);
        $("#dtpDepositFeeTaxInvoiceDate").attr("disabled", true);
        $("#dtpDepositFeeCreditNoteDate").attr("disabled", true);

        $("#cboExceptDepositFeeTaxInvoiceBillingType").attr("disabled", true);
        $("#divExceptDepositFeeTaxInvoiceBillingTypeRequired").hide();
        $("#cboDepositFeeTaxInvoiceBillingType").attr("disabled", true);

        $("#txtCancelCreditNoteNo").attr("readonly", true);

        $("#txtExceptDepositFeeTaxInvoiceNoForCreditNote").attr("readonly", true);
        $("#txtExceptDepositFeeBillingCode1").attr("readonly", true);
        $("#txtExceptDepositFeeBillingCode2").attr("readonly", true);
        $("#txtExceptDepositFeeBillingClientNameEN").attr("readonly", true);
        $("#txtExceptDepositFeeBillingClientNameLC").attr("readonly", true);

        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeAccumulatedPaymentAmount").attr("readonly", true);
        $("#txtExceptDepositFeeAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").attr("readonly", true);
        $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeCreditNoteAmount").attr("readonly", true);
        $("#txtExceptDepositFeeCreditNoteAmount").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeApproveNo").attr("readonly", true);
        $("#txtaExceptDepositFeeIssueReason").attr("readonly", true);

        $("#txtDepositFeeTaxInvoiceNoForCreditNote").attr("readonly", true);
        $("#txtDepositFeeBillingCode1").attr("readonly", true);
        $("#txtDepositFeeBillingCode2").attr("readonly", true);

        $("#txtDepositFeeBalanceOfDepositFee").attr("readonly", true);
        $("#txtDepositFeeBalanceOfDepositFee").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeBillingClientNameEN").attr("readonly", true);
        $("#txtDepositFeeBillingClientNameLC").attr("readonly", true);

        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by jirawat jannet @ 2016-10-17

        $("#txtDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true); // add by jirawat jannet @ 2016-10-17

        $("#txtDepositFeeAccumulatedPaymentAmount").attr("readonly", true);
        $("#txtDepositFeeAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeCreditNoteAmountIncludeVat").attr("readonly", true);
        $("#txtDepositFeeCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeCreditNoteAmount").attr("readonly", true);
        $("#txtDepositFeeCreditNoteAmount").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeApproveNo").attr("readonly", true);
        $("#txtaDepositFeeIssueReason").attr("readonly", true);

        $("#txtRevenueBillingCode1").attr("readonly", false);
        $("#txtRevenueBillingCode2").attr("readonly", false);

        $("#txtRevenueBalanceOfDepositFee").attr("readonly", true);
        $("#txtRevenueBalanceOfDepositFee").NumericCurrency().attr("disabled", true);

        $("#txtRevenueBillingClientNameEN").attr("readonly", true);
        $("#txtRevenueBillingClientNameLC").attr("readonly", true);

        $("#txtRevenueRevenueDepositFee").attr("readonly", false); //
        $("#txtRevenueRevenueDepositFee").NumericCurrency().attr("disabled", true); // edit by jirawat on 2016-11-14

        $("#txtRevenueRevenueVatAmount").attr("readonly", false); //
        $("#txtRevenueRevenueVatAmount").NumericCurrency().attr("disabled", true); // edit by jirawat on 2016-11-14 // add by Jirawat Jannet @ 2016-10-19

        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true);// add by Jirawat Jannet @ 2016-1-18

        $("#txtDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true);// add by Jirawat Jannet @ 2016-1-18


        $("#txtRegisterCancelCreditNote").attr("readonly", true);
        $("#txtRegisterCancelBillingCode").attr("readonly", true);
        $("#txtRegisterCancelBillingClientNameEN").attr("readonly", true);
        $("#txtRegisterCancelBillingClientNameLC").attr("readonly", true);

        $("#txtRegisterCancelTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtRegisterCancelTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtRegisterCancelTaxInvoiceAmount").attr("readonly", true);
        $("#txtRegisterCancelTaxInvoiceAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtRegisterCancelAccumulatedPaymentAmount").attr("readonly", true);
        $("#txtRegisterCancelAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#dtpRegisterCancelTaxInvoiceDate").attr("readonly", true);

        $("#txtRegisterCancelCreditNoteAmountIncludeVat").attr("readonly", true);
        $("#txtRegisterCancelCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtRegisterCancelCreditNoteAmount").attr("readonly", true);
        $("#txtRegisterCancelCreditNoteAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#dtpRegisterCancelCreditNoteDate").attr("readonly", true);
        $("#cboRegisterCancelTaxInvoiceBillingType").attr("readonly", true);

        $("#txtRegisterCancelApproveNo").attr("readonly", true);
        $("#txtaRegisterCancelIssueReason").attr("readonly", true);

    }
    else if (intMode == conModeRadio1optCancelCreditNote) {

        $("#btnExceptDepositFeeRetrieveBillingInformation").attr("disabled", true);
        $("#btnExceptDepositFeeRetrieve").attr("disabled", true);
        $("#btnExceptDepositFeeAdd").attr("disabled", true);
        $("#btnExceptDepositFeeCancel").attr("disabled", true);
        $("#btnDepositFeeRetrieveBillingInformation").attr("disabled", true);
        $("#btnDepositFeeRetrieve").attr("disabled", true);
        $("#btnDepositFeeAdd").attr("disabled", true);
        $("#btnDepositFeeCancel").attr("disabled", true);
        $("#btnRevenueRetrieve").attr("disabled", true);
        $("#btnRevenueAdd").attr("disabled", true);
        $("#btnRevenueCancel").attr("disabled", true);
        $("#btnNextToRegister").attr("disabled", true);
        $("#btnNextToRegister").hide();

        $("#chkExceptDepositFeeNotRetrieve").attr("disabled", true);
        $("#chkDepositFeeNotRetrieve").attr("disabled", true);

        $("#dtpExceptDepositFeeTaxInvoiceDate").attr("disabled", true);
        $("#dtpExceptDepositFeeCreditNoteDate").attr("disabled", true);
        $("#dtpDepositFeeTaxInvoiceDate").attr("disabled", true);
        $("#dtpDepositFeeCreditNoteDate").attr("disabled", true);

        $("#cboExceptDepositFeeTaxInvoiceBillingType").attr("disabled", true);
        $("#divExceptDepositFeeTaxInvoiceBillingTypeRequired").hide();
        $("#cboDepositFeeTaxInvoiceBillingType").attr("disabled", true);

        $("#txtCancelCreditNoteNo").attr("readonly", true);

        $("#txtExceptDepositFeeTaxInvoiceNoForCreditNote").attr("readonly", true);
        $("#txtExceptDepositFeeBillingCode1").attr("readonly", true);
        $("#txtExceptDepositFeeBillingCode2").attr("readonly", true);
        $("#txtExceptDepositFeeBillingClientNameEN").attr("readonly", true);
        $("#txtExceptDepositFeeBillingClientNameLC").attr("readonly", true);

        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeAccumulatedPaymentAmount").attr("readonly", true);
        $("#txtExceptDepositFeeAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").attr("readonly", true);
        $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeCreditNoteAmount").attr("readonly", true);
        $("#txtExceptDepositFeeCreditNoteAmount").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeApproveNo").attr("readonly", true);
        $("#txtaExceptDepositFeeIssueReason").attr("readonly", true);

        $("#txtDepositFeeTaxInvoiceNoForCreditNote").attr("readonly", true);
        $("#txtDepositFeeBillingCode1").attr("readonly", true);
        $("#txtDepositFeeBillingCode2").attr("readonly", true);

        $("#txtDepositFeeBalanceOfDepositFee").attr("readonly", true);
        $("#txtDepositFeeBalanceOfDepositFee").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeBillingClientNameEN").attr("readonly", true);
        $("#txtDepositFeeBillingClientNameLC").attr("readonly", true);

        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-1-18

        $("#txtDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-1-18

        $("#txtDepositFeeAccumulatedPaymentAmount").attr("readonly", true);
        $("#txtDepositFeeAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeCreditNoteAmountIncludeVat").attr("readonly", true);
        $("#txtDepositFeeCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeCreditNoteAmount").attr("readonly", true);
        $("#txtDepositFeeCreditNoteAmount").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeApproveNo").attr("readonly", true);
        $("#txtaDepositFeeIssueReason").attr("readonly", true);

        $("#txtRevenueBillingCode1").attr("readonly", true);
        $("#txtRevenueBillingCode2").attr("readonly", true);

        $("#txtRevenueBalanceOfDepositFee").attr("readonly", true);
        $("#txtRevenueBalanceOfDepositFee").NumericCurrency().attr("disabled", true);

        $("#txtRevenueBillingClientNameEN").attr("readonly", true);
        $("#txtRevenueBillingClientNameLC").attr("readonly", true);

        $("#txtRevenueRevenueDepositFee").attr("readonly", true);
        $("#txtRevenueRevenueDepositFee").NumericCurrency().attr("disabled", true);

        $("#txtRevenueRevenueVatAmount").attr("readonly", true);
        $("#txtRevenueRevenueVatAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-1-18

        $("#txtDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-1-18

        $("#txtRegisterCancelCreditNote").attr("readonly", true);
        $("#txtRegisterCancelBillingCode").attr("readonly", true);
        $("#txtRegisterCancelBillingClientNameEN").attr("readonly", true);
        $("#txtRegisterCancelBillingClientNameLC").attr("readonly", true);

        $("#txtRegisterCancelTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtRegisterCancelTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtRegisterCancelTaxInvoiceAmount").attr("readonly", true);
        $("#txtRegisterCancelTaxInvoiceAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtRegisterCancelAccumulatedPaymentAmount").attr("readonly", true);
        $("#txtRegisterCancelAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#dtpRegisterCancelTaxInvoiceDate").attr("readonly", true);

        $("#txtRegisterCancelCreditNoteAmountIncludeVat").attr("readonly", true);
        $("#txtRegisterCancelCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtRegisterCancelCreditNoteAmount").attr("readonly", true);
        $("#txtRegisterCancelCreditNoteAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#dtpRegisterCancelCreditNoteDate").attr("readonly", true);
        $("#cboRegisterCancelTaxInvoiceBillingType").attr("readonly", true);

        $("#txtRegisterCancelApproveNo").attr("readonly", true);
        $("#txtaRegisterCancelIssueReason").attr("readonly", true);

    }
    else {

        $("#btnExceptDepositFeeRetrieveBillingInformation").attr("disabled", true);
        $("#btnExceptDepositFeeRetrieve").attr("disabled", true);
        $("#btnExceptDepositFeeAdd").attr("disabled", true);
        $("#btnExceptDepositFeeCancel").attr("disabled", true);
        $("#btnDepositFeeRetrieveBillingInformation").attr("disabled", true);
        $("#btnDepositFeeRetrieve").attr("disabled", true);
        $("#btnDepositFeeAdd").attr("disabled", true);
        $("#btnDepositFeeCancel").attr("disabled", true);
        $("#btnRevenueRetrieve").attr("disabled", true);
        $("#btnRevenueAdd").attr("disabled", true);
        $("#btnRevenueCancel").attr("disabled", true);
        $("#btnNextToRegister").attr("disabled", true);
        $("#btnNextToRegister").hide();

        $("#chkExceptDepositFeeNotRetrieve").attr("disabled", true);
        $("#chkDepositFeeNotRetrieve").attr("disabled", true);

        $("#dtpExceptDepositFeeTaxInvoiceDate").attr("disabled", true);
        $("#dtpExceptDepositFeeCreditNoteDate").attr("disabled", true);
        $("#dtpDepositFeeTaxInvoiceDate").attr("disabled", true);
        $("#dtpDepositFeeCreditNoteDate").attr("disabled", true);

        $("#cboExceptDepositFeeTaxInvoiceBillingType").attr("disabled", true);
        $("#divExceptDepositFeeTaxInvoiceBillingTypeRequired").hide();
        $("#cboDepositFeeTaxInvoiceBillingType").attr("disabled", true);

        $("#txtCancelCreditNoteNo").attr("readonly", true);

        $("#txtExceptDepositFeeTaxInvoiceNoForCreditNote").attr("readonly", true);
        $("#txtExceptDepositFeeBillingCode1").attr("readonly", true);
        $("#txtExceptDepositFeeBillingCode2").attr("readonly", true);
        $("#txtExceptDepositFeeBillingClientNameEN").attr("readonly", true);
        $("#txtExceptDepositFeeBillingClientNameLC").attr("readonly", true);

        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeAccumulatedPaymentAmount").attr("readonly", true);
        $("#txtExceptDepositFeeAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").attr("readonly", true);
        $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeCreditNoteAmount").attr("readonly", true);
        $("#txtExceptDepositFeeCreditNoteAmount").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeApproveNo").attr("readonly", true);
        $("#txtaExceptDepositFeeIssueReason").attr("readonly", true);

        $("#txtDepositFeeTaxInvoiceNoForCreditNote").attr("readonly", true);
        $("#txtDepositFeeBillingCode1").attr("readonly", true);
        $("#txtDepositFeeBillingCode2").attr("readonly", true);

        $("#txtDepositFeeBalanceOfDepositFee").attr("readonly", true);
        $("#txtDepositFeeBalanceOfDepositFee").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeBillingClientNameEN").attr("readonly", true);
        $("#txtDepositFeeBillingClientNameLC").attr("readonly", true);

        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-18

        $("#txtDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-18

        $("#txtDepositFeeAccumulatedPaymentAmount").attr("readonly", true);
        $("#txtDepositFeeAccumulatedPaymentAmount").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeCreditNoteAmountIncludeVat").attr("readonly", true);
        $("#txtDepositFeeCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeCreditNoteAmount").attr("readonly", true);
        $("#txtDepositFeeCreditNoteAmount").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeApproveNo").attr("readonly", true);
        $("#txtaDepositFeeIssueReason").attr("readonly", true);

        $("#txtRevenueBillingCode1").attr("readonly", true);
        $("#txtRevenueBillingCode2").attr("readonly", true);

        $("#txtRevenueBalanceOfDepositFee").attr("readonly", true);
        $("#txtRevenueBalanceOfDepositFee").NumericCurrency().attr("disabled", true);

        $("#txtRevenueBillingClientNameEN").attr("readonly", true);
        $("#txtRevenueBillingClientNameLC").attr("readonly", true);

        $("#txtRevenueRevenueDepositFee").attr("readonly", true);
        $("#txtRevenueRevenueDepositFee").NumericCurrency().attr("disabled", true);

        $("#txtRevenueRevenueVatAmount").attr("readonly", true);
        $("#txtRevenueRevenueVatAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-19

        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-18

        $("#txtDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true); // add by Jirawat Jannet @ 2016-10-18

    };

}

// Visable Obj On Screen
function setVisableTable(intMode) {

    if (intMode == conModeRadio1optRegisterRefundCreditNoteExceptDepositFee) {
        $("#divSelectProcess").show();
        $("#divRegisterRefundCreditNoteExceptDepositFee").show();
        // swap header 
        $("#divRegisterRefundCreditNoteExceptDepositFeeHeader1").show();
        $("#divRegisterRefundCreditNoteExceptDepositFeeHeader2").hide();
        $("#divRegisterRefundCreditNoteExceptDepositFeeHeader3").hide();
        $("#divRegisterCancelCreditNoteInformation").hide();
        $("#divRegisterRefundCreditNoteDepositFee").hide();
        $("#divRevenueDepositFee").hide();
        $("#divCancelCreditNote").show();
    }
    else if (intMode == conModeRadio1optRegisterDecreasedCreditNote) {
        $("#divSelectProcess").show();
        $("#divRegisterRefundCreditNoteExceptDepositFee").show();
        // swap header 
        $("#divRegisterRefundCreditNoteExceptDepositFeeHeader1").hide();
        $("#divRegisterRefundCreditNoteExceptDepositFeeHeader2").show();
        $("#divRegisterRefundCreditNoteExceptDepositFeeHeader3").hide();
        $("#divRegisterCancelCreditNoteInformation").hide();
        $("#divRegisterRefundCreditNoteDepositFee").hide();
        $("#divRevenueDepositFee").hide();
        $("#divCancelCreditNote").show();
    }
    else if (intMode == conModeRadio1optRegisterRefundCreditNoteDepositFee) {
        $("#divSelectProcess").show();
        $("#divRegisterRefundCreditNoteExceptDepositFee").hide();
        $("#divRegisterRefundCreditNoteDepositFee").show();
        $("#divRegisterCancelCreditNoteInformation").hide();
        $("#divRevenueDepositFee").hide();
        $("#divCancelCreditNote").show();
    }
    else if (intMode == conModeRadio1optRevenueDepositFee) {
        $("#divSelectProcess").show();
        $("#divRegisterRefundCreditNoteExceptDepositFee").hide();
        $("#divRegisterRefundCreditNoteDepositFee").hide();
        $("#divRegisterCancelCreditNoteInformation").hide();
        $("#divRevenueDepositFee").show();
        $("#divCancelCreditNote").show();
    }
    else if (intMode == conModeRadio1optCancelCreditNote) {
        $("#divSelectProcess").show();
        $("#divRegisterRefundCreditNoteExceptDepositFee").hide();
        // swap header 
        $("#divRegisterRefundCreditNoteExceptDepositFeeHeader1").hide();
        $("#divRegisterRefundCreditNoteExceptDepositFeeHeader2").hide();
        $("#divRegisterRefundCreditNoteExceptDepositFeeHeader3").hide();
        $("#divRegisterCancelCreditNoteInformation").show();
        $("#divRegisterRefundCreditNoteDepositFee").hide();
        $("#divRevenueDepositFee").hide();
        $("#divCancelCreditNote").hide();
    }
    else {
        $("#divSelectProcess").show();
        $("#divRegisterRefundCreditNoteExceptDepositFee").hide();
        // swap header 
        $("#divRegisterRefundCreditNoteExceptDepositFeeHeader1").hide();
        $("#divRegisterRefundCreditNoteExceptDepositFeeHeader2").hide();
        $("#divRegisterRefundCreditNoteExceptDepositFeeHeader3").hide();
        $("#divRegisterCancelCreditNoteInformation").hide();
        $("#divRegisterRefundCreditNoteDepositFee").hide();
        $("#divRevenueDepositFee").hide();
        $("#divCancelCreditNote").hide();
    };

}
// Clear Obj On Screen

function setAllObjectToConfirmMode(intMode) {
    if (intMode == conYes) {

        $("#divSelectProcess").SetViewMode(true);
        $("#divRegisterRefundCreditNoteExceptDepositFee").SetViewMode(true);
        $("#divRegisterRefundCreditNoteDepositFee").SetViewMode(true);
        $("#divRevenueDepositFee").SetViewMode(true);
        //$("#divCancelCreditNote").SetViewMode(true);

        $("#divSelectProcess").ResetToNormalControl(false);
        $("#divRegisterRefundCreditNoteExceptDepositFee").ResetToNormalControl(false);
        $("#divRegisterRefundCreditNoteDepositFee").ResetToNormalControl(false);
        $("#divRevenueDepositFee").ResetToNormalControl(false);
        $("#divCancelCreditNote").ResetToNormalControl(false);
        // hide input section when cancel
        $("#divSelectProcess").hide();
        $("#divRegisterRefundCreditNoteExceptDepositFee").hide();
        $("#divRegisterRefundCreditNoteDepositFee").hide();
        $("#divRevenueDepositFee").hide();

        $("#divRegisterCancelCreditNoteInformation").SetViewMode(true);
        $("#divRegisterCancelCreditNoteInformation").ResetToNormalControl(false);

    } else {

        $("#divSelectProcess").SetViewMode(false);
        $("#divRegisterRefundCreditNoteExceptDepositFee").SetViewMode(false);
        $("#divRegisterRefundCreditNoteDepositFee").SetViewMode(false);
        $("#divRevenueDepositFee").SetViewMode(false);
        $("#divCancelCreditNote").SetViewMode(false);

        $("#divSelectProcess").ResetToNormalControl(true);
        $("#divRegisterRefundCreditNoteExceptDepositFee").ResetToNormalControl(true);
        $("#divRegisterRefundCreditNoteDepositFee").ResetToNormalControl(true);
        $("#divRevenueDepositFee").ResetToNormalControl(true);
        $("#divCancelCreditNote").ResetToNormalControl(true);
        // hide input section when cancel
        $("#divSelectProcess").show();
        $("#divRegisterRefundCreditNoteExceptDepositFee").show();
        $("#divRegisterRefundCreditNoteDepositFee").show();
        $("#divRevenueDepositFee").show();

        $("#divRegisterCancelCreditNoteInformation").SetViewMode(false);
        $("#divRegisterCancelCreditNoteInformation").ResetToNormalControl(true);
    }
}

// Radio Select

function optRegisterRefundCreditNoteExceptDepositFee_Select() {
    verModeRadio1 = conModeRadio1optRegisterRefundCreditNoteExceptDepositFee;
    $("#txtCancelCreditNoteNo").attr("readonly", true);
}

function optRegisterDecreasedCreditNote_Select() {
    verModeRadio1 = conModeRadio1optRegisterDecreasedCreditNote;
    $("#txtCancelCreditNoteNo").attr("readonly", true);
}

function optRegisterRefundCreditNoteDepositFee_Select() {
    verModeRadio1 = conModeRadio1optRegisterRefundCreditNoteDepositFee;
    $("#txtCancelCreditNoteNo").attr("readonly", true);
}

function optRevenueDepositFee_Select() {
    verModeRadio1 = conModeRadio1optRevenueDepositFee;
    $("#txtCancelCreditNoteNo").attr("readonly", true);
}

function optCancelCreditNote_Select() {
    verModeRadio1 = conModeRadio1optCancelCreditNote;
    $("#txtCancelCreditNoteNo").ResetToNormalControl();
    $("#txtCancelCreditNoteNo").attr("readonly", false);
}

function chk_ExceptDepositFeeNotRetrieve_change() {

    var chkExceptDepositFeeNotRetrieve;

    chkExceptDepositFeeNotRetrieve = $("#chkExceptDepositFeeNotRetrieve").prop('checked');
    $("#divRegisterRefundCreditNoteExceptDepositFeeBody").ResetToNormalControl();

    if (chkExceptDepositFeeNotRetrieve) {

        $("#btnExceptDepositFeeRetrieveBillingInformation").attr("disabled", true);
        $("#btnExceptDepositFeeRetrieve").attr("disabled", false);
        $("#btnExceptDepositFeeAdd").attr("disabled", true);
        $("#btnExceptDepositFeeCancel").attr("disabled", true);

        $("#dtpExceptDepositFeeTaxInvoiceDate").val("");
        //$("#dtpExceptDepositFeeCreditNoteDate").attr("disabled", true);

        $("#cboExceptDepositFeeTaxInvoiceBillingType").val("");

        //$("#txtExceptDepositFeeTaxInvoiceNoForCreditNote").attr("disabled", true);
        $("#txtExceptDepositFeeTaxInvoiceNoForCreditNote").attr("readonly", false);
        $("#txtExceptDepositFeeBillingCode1").attr("readonly", false);
        $("#txtExceptDepositFeeBillingCode2").attr("readonly", false);
        $("#txtExceptDepositFeeBillingCode1").val("");
        $("#txtExceptDepositFeeBillingCode2").val("");
        $("#txtExceptDepositFeeBillingClientNameEN").val("");
        $("#txtExceptDepositFeeBillingClientNameLC").val("");

        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", false);
        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", false);
        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").val("");

        $("#txtExceptDepositFeeTaxInvoiceAmount").attr("readonly", false);
        $("#txtExceptDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", false);
        $("#txtExceptDepositFeeTaxInvoiceAmount").val("");

        $("#txtExceptDepositFeeAccumulatedPaymentAmount").val("");
        $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").val("");
        $("#txtExceptDepositFeeCreditNoteAmount").val("");

        $("#dtpExceptDepositFeeTaxInvoiceDate").val("");
        $("#dtpExceptDepositFeeTaxInvoiceDate").val("");
        //$("#txtExceptDepositFeeApproveNo").attr("disabled", true);
        //$("#txtaExceptDepositFeeIssueReason").attr("disabled", true);

    }
    else {
        $("#btnExceptDepositFeeRetrieveBillingInformation").attr("disabled", false);
        $("#btnExceptDepositFeeRetrieve").attr("disabled", true);
        $("#txtExceptDepositFeeBillingCode1").attr("readonly", true);
        $("#txtExceptDepositFeeBillingCode2").attr("readonly", true);
        $("#txtExceptDepositFeeBillingCode1").val("");
        $("#txtExceptDepositFeeBillingCode2").val("");

        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true);

        $("#txtExceptDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtExceptDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true);
    }

}
function chk_DepositFeeNotRetrieve_change() {

    var chkDepositFeeNotRetrieve;

    chkDepositFeeNotRetrieve = $("#chkDepositFeeNotRetrieve").prop('checked');

    $("#divRegisterRefundCreditNoteDepositFeeBody").ResetToNormalControl();

    if (chkDepositFeeNotRetrieve) {
        $("#btnDepositFeeRetrieveBillingInformation").attr("disabled", true);
        $("#btnDepositFeeRetrieve").attr("disabled", false);
        $("#btnDepositFeeAdd").attr("disabled", true);
        $("#btnDepositFeeCancel").attr("disabled", true);

        $("#dtpDepositFeeTaxInvoiceDate").val("");
        //$("#dtpDepositFeeCreditNoteDate").attr("disabled", true);

        $("#txtDepositFeeTaxInvoiceNoForCreditNote").attr("readonly", false);
        $("#txtDepositFeeBillingCode1").attr("readonly", false);
        $("#txtDepositFeeBillingCode2").attr("readonly", false);
        $("#txtDepositFeeBillingCode1").val("");
        $("#txtDepositFeeBillingCode2").val("");
        $("#txtDepositFeeBalanceOfDepositFee").attr("readonly", true);
        $("#txtDepositFeeBalanceOfDepositFee").NumericCurrency().attr("disabled", true);
        $("#txtDepositFeeBalanceOfDepositFee").val("");
        $("#txtDepositFeeBillingClientNameEN").val("");
        $("#txtDepositFeeBillingClientNameLC").val("");

        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", false);
        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", false); // add by Jirawat Jannet @ 2016-10-18
        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").val("");

        $("#txtDepositFeeTaxInvoiceAmount").attr("readonly", false);
        $("#txtDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", false); // add by Jirawat Jannet @ 2016-10-18
        $("#txtDepositFeeTaxInvoiceAmount").val("");

        $("#txtDepositFeeAccumulatedPaymentAmount").val("");
        $("#txtDepositFeeCreditNoteAmountIncludeVat").val("");
        $("#txtDepositFeeCreditNoteAmount").val("");
        //$("#txtDepositFeeApproveNo").attr("disabled", true);
        //$("#txtaDepositFeeIssueReason").attr("disabled", true);
        $("#dtpDepositFeeTaxInvoiceDate").val("");
        $("#dtpDepositFeeTaxInvoiceDate").val("");
    }
    else {
        $("#btnDepositFeeRetrieveBillingInformation").attr("disabled", false);
        $("#btnDepositFeeRetrieve").attr("disabled", true);
        $("#txtDepositFeeBillingCode1").attr("readonly", true);
        $("#txtDepositFeeBillingCode2").attr("readonly", true);
        $("#txtDepositFeeBillingCode1").val("");
        $("#txtDepositFeeBillingCode2").val("");

        $("#txtDepositFeeBalanceOfDepositFee").attr("readonly", true);
        $("#txtDepositFeeBalanceOfDepositFee").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().attr("disabled", true);

        $("#txtDepositFeeTaxInvoiceAmount").attr("readonly", true);
        $("#txtDepositFeeTaxInvoiceAmount").NumericCurrency().attr("disabled", true);

    }

}

function btn_ExceptDepositFeeCancel_click() {
    ajax_method.CallScreenController("/Income/ICS070_ExceptDepositFeeCancel", "", function (result, controls, isWarning) {
        setEnabledObject(conModeRadio1optRegisterRefundCreditNoteExceptDepositFee);
        ClearScreenByMode(conModeRadio1optRegisterRefundCreditNoteExceptDepositFee);
        CloseWarningDialog();

        $("#divSelectProcess").ResetToNormalControl(true);
        $("#divRegisterRefundCreditNoteExceptDepositFee").ResetToNormalControl(true);
        $("#divRegisterRefundCreditNoteDepositFee").ResetToNormalControl(true);
        $("#divRegisterCancelCreditNoteInformation").ResetToNormalControl(true);
        $("#divRevenueDepositFee").ResetToNormalControl(true);
        $("#divCancelCreditNote").ResetToNormalControl(true);
    });
}
function btn_DepositFeeCancel_click() {
    ajax_method.CallScreenController("/Income/ICS070_DepositFeeCancel", "", function (result, controls, isWarning) {
        setEnabledObject(conModeRadio1optRegisterRefundCreditNoteDepositFee);
        ClearScreenByMode(conModeRadio1optRegisterRefundCreditNoteDepositFee);
        CloseWarningDialog();

        $("#divSelectProcess").ResetToNormalControl(true);
        $("#divRegisterRefundCreditNoteExceptDepositFee").ResetToNormalControl(true);
        $("#divRegisterRefundCreditNoteDepositFee").ResetToNormalControl(true);
        $("#divRegisterCancelCreditNoteInformation").ResetToNormalControl(true);
        $("#divRevenueDepositFee").ResetToNormalControl(true);
        $("#divCancelCreditNote").ResetToNormalControl(true);
    });
}
function btn_RevenueCancel_click() {
    ajax_method.CallScreenController("/Income/ICS070_RevenueCancel", "", function (result, controls, isWarning) {
        setEnabledObject(conModeRadio1optRevenueDepositFee);
        ClearScreenByMode(conModeRadio1optRevenueDepositFee);
        CloseWarningDialog();

        $("#divSelectProcess").ResetToNormalControl(true);
        $("#divRegisterRefundCreditNoteExceptDepositFee").ResetToNormalControl(true);
        $("#divRegisterRefundCreditNoteDepositFee").ResetToNormalControl(true);
        $("#divRegisterCancelCreditNoteInformation").ResetToNormalControl(true);
        $("#divRevenueDepositFee").ResetToNormalControl(true);
        $("#divCancelCreditNote").ResetToNormalControl(true);
    });
}


function btn_ExceptDepositFeeRetrieveBillingInformation_click() {
    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Income/ICS070_ExceptDepositFeeRetrieveBillingInformation", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            $("#txtExceptDepositFeeTaxInvoiceNoForCreditNote").attr("readonly", true);
            $("#btnExceptDepositFeeRetrieveBillingInformation").attr("disabled", true);

            // Add by Jirawat Jannet 2016-10-26

            var VatAmount = 0;
            var InvoiceAmount = 0;
            var PaidAmountIncVat = 0;
            var RegisteredWHTAmount = 0;

            //VatAmount
            if (result.doGetTaxInvoiceForIC.VatAmountCurrencyType == ICS070_ViewBag.C_CURRENCY_LOCAL) {
                VatAmount = result.doGetTaxInvoiceForIC.VatAmount;
            } else if (result.doGetTaxInvoiceForIC.VatAmountCurrencyType == ICS070_ViewBag.C_CURRENCY_US) {
                VatAmount = result.doGetTaxInvoiceForIC.VatAmountUsd;
            }
            if (VatAmount == null) VatAmount = 0;

            //InvoiceAmount
            if (result.doGetTaxInvoiceForIC.InvoiceAmountCurrencyType == ICS070_ViewBag.C_CURRENCY_LOCAL) {
                InvoiceAmount = result.doGetTaxInvoiceForIC.InvoiceAmount;
            } else if (result.doGetTaxInvoiceForIC.InvoiceAmountCurrencyType == ICS070_ViewBag.C_CURRENCY_US) {
                InvoiceAmount = result.doGetTaxInvoiceForIC.InvoiceAmountUsd;
            }
            if (InvoiceAmount == null) InvoiceAmount = 0;

            //PaidAmountIncVat
            if (result.doGetTaxInvoiceForIC.PaidAmountIncVatCurrencyType == ICS070_ViewBag.C_CURRENCY_LOCAL) {
                PaidAmountIncVat = result.doGetTaxInvoiceForIC.PaidAmountIncVat;
            } else if (result.doGetTaxInvoiceForIC.PaidAmountIncVatCurrencyType == ICS070_ViewBag.C_CURRENCY_US) {
                PaidAmountIncVat = result.doGetTaxInvoiceForIC.PaidAmountIncVatUsd;
            }
            if (PaidAmountIncVat == null) PaidAmountIncVat = 0;

            //RegisteredWHTAmount
            if (result.doGetTaxInvoiceForIC.RegisteredWHTAmountCurrencyType == ICS070_ViewBag.C_CURRENCY_LOCAL) {
                RegisteredWHTAmount = result.doGetTaxInvoiceForIC.RegisteredWHTAmount;
            } else if (result.doGetTaxInvoiceForIC.RegisteredWHTAmountCurrencyType == ICS070_ViewBag.C_CURRENCY_US) {
                RegisteredWHTAmount = result.doGetTaxInvoiceForIC.RegisteredWHTAmountUsd;
            }
            if (RegisteredWHTAmount == null) RegisteredWHTAmount = 0;

            // End add


            $("#btnExceptDepositFeeAdd").attr("disabled", false);
            $("#btnExceptDepositFeeCancel").attr("disabled", false);


            $("#txtExceptDepositFeeTaxInvoiceNoForCreditNote").val(result.doGetTaxInvoiceForIC.TaxInvoiceNo);
            if (result.strBillingCode != undefined) {
                $("#txtExceptDepositFeeBillingCode1").val(result.strBillingCode.substring(0, result.strBillingCode.indexOf('-')));
                $("#txtExceptDepositFeeBillingCode2").val(result.strBillingCode.substring(result.strBillingCode.indexOf('-') + 1));
            }
            $("#txtExceptDepositFeeBillingClientNameEN").val(result.doGetTaxInvoiceForIC.BillingClientNameEN);
            $("#txtExceptDepositFeeBillingClientNameLC").val(result.doGetTaxInvoiceForIC.BillingClientNameLC);

            $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").val(number_format(parseFloat(parseFloat(InvoiceAmount) + parseFloat(VatAmount)), 2, '.', ','));
            $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().val(result.doGetTaxInvoiceForIC.InvoiceAmountCurrencyType);

            $("#txtExceptDepositFeeTaxInvoiceAmount").val(number_format(parseFloat(VatAmount), 2, '.', ','));
            $("#txtExceptDepositFeeTaxInvoiceAmount").NumericCurrency().val(result.doGetTaxInvoiceForIC.VatAmountCurrencyType); // Add by Jirawat Jannet @ 2016-10-26

            var depFeeAccmPayAmt = parseFloat(PaidAmountIncVat) + parseFloat(RegisteredWHTAmount);

            $("#txtExceptDepositFeeAccumulatedPaymentAmount").val(number_format(depFeeAccmPayAmt, 2, '.', ','));
            $("#txtExceptDepositFeeAccumulatedPaymentAmount").NumericCurrency().val(result.doGetTaxInvoiceForIC.PaidAmountIncVatCurrencyType);

            $("#dtpExceptDepositFeeTaxInvoiceDate").val(ConvertDateToTextFormat(ConvertDateObject(result.doGetTaxInvoiceForIC.TaxInvoiceDate)));
            $("#cboExceptDepositFeeTaxInvoiceBillingType").val(result.doGetTaxInvoiceForIC.BillingTypeCode);

            if (depFeeAccmPayAmt == 0 && verModeRadio1 == conModeRadio1optRegisterRefundCreditNoteExceptDepositFee) {
                var obj = {
                    module: "Income",
                    code: "MSG7125"
                };
                call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                    OpenInformationMessageDialog(result.Code, result.Message,
                        function () {
                            btn_Reset_click();
                        },
                        null);
                });
            }

        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });

}

function btn_DepositFeeRetrieveBillingInformation_click() {

    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Income/ICS070_DepositFeeRetrieveBillingInformation", obj, function (result, controls, isWarning) {
        if (result != undefined) {

            $("#btnDepositFeeRetrieveBillingInformation").attr("disabled", true);

            $("#txtDepositFeeTaxInvoiceNoForCreditNote").attr("readonly", true);

            $("#txtDepositFeeCreditNoteAmountIncludeVat").attr("readonly", false);
            $("#txtDepositFeeCreditNoteAmountIncludeVat").NumericCurrency().attr("disabled", false);

            $("#btnDepositFeeAdd").attr("disabled", false);
            $("#btnDepositFeeCancel").attr("disabled", false);


            $("#txtDepositFeeTaxInvoiceNoForCreditNote").val(result.doGetTaxInvoiceForIC.TaxInvoiceNo);
            if (result.decBalanceDeposit == null) {
                result.decBalanceDeposit = 0;
            }
            $("#txtDepositFeeBalanceOfDepositFee").val(number_format(parseFloat(result.decBalanceDeposit), 2, '.', ','));
            $("#txtDepositFeeBalanceOfDepositFee").NumericCurrency().val(result.decBalanceDepositCurrencyType);
            if (result.strBillingCode != undefined) {
                $("#txtDepositFeeBillingCode1").val(result.strBillingCode.substring(0, result.strBillingCode.indexOf('-')));
                $("#txtDepositFeeBillingCode2").val(result.strBillingCode.substring(result.strBillingCode.indexOf('-') + 1));
            }
            $("#txtDepositFeeBillingClientNameEN").val(result.doGetTaxInvoiceForIC.BillingClientNameEN);
            $("#txtDepositFeeBillingClientNameLC").val(result.doGetTaxInvoiceForIC.BillingClientNameLC);

            // Add by Jirawat Jannet @ 2016-10-26
            var InvoiceAmount = 0;
            var VatAmount = 0;
            if (result.doGetTaxInvoiceForIC.InvoiceAmountCurrencyType == ICS070_ViewBag.C_CURRENCY_LOCAL) {
                InvoiceAmount = result.doGetTaxInvoiceForIC.InvoiceAmount;
            }
            else if (result.doGetTaxInvoiceForIC.InvoiceAmountCurrencyType == ICS070_ViewBag.C_CURRENCY_US) {
                InvoiceAmount = result.doGetTaxInvoiceForIC.InvoiceAmountUsd;
            }
            if (result.doGetTaxInvoiceForIC.VatAmountCurrencyType == ICS070_ViewBag.C_CURRENCY_LOCAL) {
                VatAmount = result.doGetTaxInvoiceForIC.VatAmount;
            }
            else if (result.doGetTaxInvoiceForIC.VatAmountCurrencyType == ICS070_ViewBag.C_CURRENCY_US) {
                VatAmount = result.doGetTaxInvoiceForIC.VatAmountUsd;
            }
            if (InvoiceAmount == null) PaidAmountIncVat = 0;
            if (VatAmount == null) PaidAmountIncVat = 0;
            // End add 


            $("#txtDepositFeeTaxInvoiceAmountIncludeVat").val(number_format(parseFloat(parseFloat(InvoiceAmount) + parseFloat(VatAmount)), 2, '.', ','));
            $("#txtDepositFeeTaxInvoiceAmountIncludeVat").NumericCurrency().val(result.doGetTaxInvoiceForIC.InvoiceAmountCurrencyType); // add by Jirawat Jannet @ 2016-10-19

            $("#txtDepositFeeTaxInvoiceAmount").val(number_format(parseFloat(VatAmount), 2, '.', ','));
            $("#txtDepositFeeTaxInvoiceAmount").NumericCurrency().val(result.doGetTaxInvoiceForIC.VatAmountCurrencyType);

            // Add by Jirawat Jannet @ 2016-10-26
            var RegisteredWHTAmount = 0;
            var PaidAmountIncVat = 0;
            if (result.doGetTaxInvoiceForIC.PaidAmountIncVatCurrencyType == ICS070_ViewBag.C_CURRENCY_LOCAL) {
                RegisteredWHTAmount = result.doGetTaxInvoiceForIC.RegisteredWHTAmount;
                PaidAmountIncVat = result.doGetTaxInvoiceForIC.PaidAmountIncVat;
            }
            else if (result.doGetTaxInvoiceForIC.PaidAmountIncVatCurrencyType == ICS070_ViewBag.C_CURRENCY_US) {
                RegisteredWHTAmount = result.doGetTaxInvoiceForIC.RegisteredWHTAmountUsd;
                PaidAmountIncVat = result.doGetTaxInvoiceForIC.PaidAmountIncVatUsd;
            }
            if (RegisteredWHTAmount == null) RegisteredWHTAmount = 0;
            if (PaidAmountIncVat == null) PaidAmountIncVat = 0;
            // End add 


            $("#txtDepositFeeAccumulatedPaymentAmount").val(number_format(parseFloat(PaidAmountIncVat) + parseFloat(RegisteredWHTAmount), 2, '.', ','));
            $("#txtDepositFeeAccumulatedPaymentAmount").NumericCurrency().val(result.doGetTaxInvoiceForIC.PaidAmountIncVatCurrencyType);

            $("#dtpDepositFeeTaxInvoiceDate").val(ConvertDateToTextFormat(ConvertDateObject(result.doGetTaxInvoiceForIC.TaxInvoiceDate)));

        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });

}

function btn_ExceptDepositFeeRetrieve_click() {
    $("#divRegisterRefundCreditNoteExceptDepositFeeBody").ResetToNormalControl();

    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Income/ICS070_ExceptDepositFeeRetrieve", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            $("#chkExceptDepositFeeNotRetrieve").attr("disabled", true);
            $("#txtExceptDepositFeeBillingCode1").attr("readonly", true);
            $("#txtExceptDepositFeeBillingCode2").attr("readonly", true);
            $("#btnExceptDepositFeeRetrieve").attr("disabled", true);
            $("#cboExceptDepositFeeTaxInvoiceBillingType").attr("disabled", false);
            $("#divExceptDepositFeeTaxInvoiceBillingTypeRequired").show();

            $("#btnExceptDepositFeeAdd").attr("disabled", false);
            $("#btnExceptDepositFeeCancel").attr("disabled", false);

            //Display information
            $("#txtExceptDepositFeeBillingCode1").val(result.doGetBillingCodeInfo.ContractCodeShortFormat);
            $("#txtExceptDepositFeeBillingCode2").val(result.doGetBillingCodeInfo.BillingOCC);
            $("#txtExceptDepositFeeBillingClientNameEN").val(result.doGetBillingCodeInfo.BillingClientNameEN);
            $("#txtExceptDepositFeeBillingClientNameLC").val(result.doGetBillingCodeInfo.BillingClientNameLC);
        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });
}

function btn_DepositFeeRetrieve_click() {
    $("#divRegisterRefundCreditNoteDepositFeeBody").ResetToNormalControl();

    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Income/ICS070_DepositFeeRetrieve", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            $("#chkDepositFeeNotRetrieve").attr("disabled", true);
            $("#txtDepositFeeBillingCode1").attr("readonly", true);
            $("#txtDepositFeeBillingCode2").attr("readonly", true);
            $("#btnDepositFeeRetrieve").attr("disabled", true);

            $("#btnDepositFeeAdd").attr("disabled", false);
            $("#btnDepositFeeCancel").attr("disabled", false);

            //Display value
            $("#txtDepositFeeBillingCode1").val(result.doGetBillingCodeInfo.ContractCodeShortFormat);
            $("#txtDepositFeeBillingCode2").val(result.doGetBillingCodeInfo.BillingOCC);
            if (result.doGetBillingCodeInfo.BalanceDeposit == null) {
                result.doGetBillingCodeInfo.BalanceDeposit = 0;
            }
            $("#txtDepositFeeBalanceOfDepositFee").val(number_format(parseFloat(result.doGetBillingCodeInfo.BalanceDeposit), 2, '.', ','));
            $("#txtDepositFeeBillingClientNameEN").val(result.doGetBillingCodeInfo.BillingClientNameEN);
            $("#txtDepositFeeBillingClientNameLC").val(result.doGetBillingCodeInfo.BillingClientNameLC);

        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });
}


function btn_RevenueRetrieve_click() {
    $("#divRevenueDepositFeeBody").ResetToNormalControl();

    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Income/ICS070_RevenueRetrieve", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            $("#chkRevenueNotRetrieve").attr("disabled", true);
            $("#txtRevenueBillingCode1").attr("readonly", true);
            $("#txtRevenueBillingCode2").attr("readonly", true);
            $("#btnRevenueRetrieve").attr("disabled", true);

            $("#btnRevenueAdd").attr("disabled", false);
            $("#btnRevenueCancel").attr("disabled", false);

            // add by Jirawat Jannet @ 2016-10-26

            var BalanceDeposit = 0;

            if (result.doGetBillingCodeInfo.BalanceDepositCurrencyType == ICS070_ViewBag.C_CURRENCY_LOCAL) {
                BalanceDeposit = result.doGetBillingCodeInfo.BalanceDeposit;
            } else if (result.doGetBillingCodeInfo.BalanceDepositCurrencyType == ICS070_ViewBag.C_CURRENCY_US) {
                BalanceDeposit = result.doGetBillingCodeInfo.BalanceDepositUsd;
            }

            // End add

            //Display information
            $("#txtRevenueBillingCode1").val(result.doGetBillingCodeInfo.ContractCodeShortFormat);
            $("#txtRevenueBillingCode2").val(result.doGetBillingCodeInfo.BillingOCC);
            $("#txtRevenueBalanceOfDepositFee").val(number_format(parseFloat(BalanceDeposit), 2, '.', ',')); // edit by Jirawat Jannet @ 2016-10-26
            $("#txtRevenueBalanceOfDepositFee").NumericCurrency().val(result.doGetBillingCodeInfo.BalanceDepositCurrencyType); // add by Jirawat Jannet @ 2016-10-26

            $('#txtRevenueRevenueDepositFee').NumericCurrency().val(result.doGetBillingCodeInfo.BalanceDepositCurrencyType);
            $('#txtRevenueRevenueVatAmount').NumericCurrency().val(result.doGetBillingCodeInfo.BalanceDepositCurrencyType);

            $("#txtRevenueBillingClientNameEN").val(result.doGetBillingCodeInfo.BillingClientNameEN);
            $("#txtRevenueBillingClientNameLC").val(result.doGetBillingCodeInfo.BillingClientNameLC);
        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });
}

function btn_ExceptDepositFeeAdd_click() {

    // check add duplicate
    var strTaxInvoiceNo = $("#txtExceptDepositFeeTaxInvoiceNoForCreditNote").val();
    var bolDubTransaction = false;

    for (var i = 0; i < grdCancelCreditNote.getRowsNum(); i++) {

        var row_id = grdCancelCreditNote.getRowId(i);

        if (strTaxInvoiceNo.toUpperCase() == grdCancelCreditNote.cells(row_id, grdCancelCreditNote.getColIndexById("TaxInvoiceNo")).getValue().toUpperCase()) {
            bolDubTransaction = true;
            break;
        }
        //else {
        //    strTaxInvoiceNo = grdCancelCreditNote.cells(row_id, grdCancelCreditNote.getColIndexById("TaxInvoiceNo")).getValue();
        //}
    }

    if (bolDubTransaction) {
        /* --- Get Message --- */
        var obj = {
            module: "Income",
            code: "MSG7103",
            param: [strTaxInvoiceNo]
        };
        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
            OpenInformationMessageDialog(result.Code, result.Message,
        function () {
            // no funcition
        },
        null);
        });
    } else {
        var obj = GetUserAdjustData();
        ajax_method.CallScreenController("/Income/ICS070_ExceptDepositFeeAdd", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                //Confirm warning message before add to grid
                ExceptDepositFeeAddToGrid(result);
            }
            else if (controls != undefined) {
                VaridateCtrl(controls, controls);
            }
        });
    }
}

function ExceptDepositFeeAddToGrid(result) {
    if (result.WarningMessage == undefined || result.WarningMessage.length == 0) {
        // add grd

        bolViewMode = false;

        Add_CancelCreditNoteBlankLine(
                            grdCancelCreditNote.getRowsNum(),
                            result.strRegContent,
                            $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").NumericCurrencyText() + ' ' + $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").val(),
                            $("#txtExceptDepositFeeCreditNoteAmount").NumericCurrencyText() + ' ' + $("#txtExceptDepositFeeCreditNoteAmount").val(),
                            $("#dtpExceptDepositFeeCreditNoteDate").val(),
                            $("#txtExceptDepositFeeBillingCode1").val() + '-' + $("#txtExceptDepositFeeBillingCode2").val(),
                            '',
                            $("#txtExceptDepositFeeBillingClientNameEN").val(), //Merge at 14032017 By Pachara S.

                            result.strCreditNoteType,
                            $("#txtExceptDepositFeeTaxInvoiceNoForCreditNote").val().toUpperCase(),
                            result.strBillingTargetCode,

                            $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").val(),
                            $("#txtExceptDepositFeeCreditNoteAmount").val(),
                            $("#cboExceptDepositFeeTaxInvoiceBillingType").val(),
                            $("#txtExceptDepositFeeApproveNo").val(),
                            $("#txtaExceptDepositFeeIssueReason").val(),
                            $("#txtExceptDepositFeeTaxInvoiceAmountIncludeVat").val(),
                            $("#txtExceptDepositFeeTaxInvoiceAmount").val(),
                            $("#dtpExceptDepositFeeTaxInvoiceDate").val(),
                            '',
                            '',
                            '',
                            '',
                            '',
                            '',
                            result.strInvoiceNo,
                            result.strInvoiceOCC,
                            result.bolNotRetrieveFlag,
                            $("#txtExceptDepositFeeBillingClientNameEN").val(),
                            $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").val(),
                            $("#txtExceptDepositFeeCreditNoteAmount").val(),
                            $("#txtExceptDepositFeeCreditNoteAmountIncludeVat").NumericCurrencyValue(),
                            $("#txtExceptDepositFeeCreditNoteAmount").NumericCurrencyValue(),
                            '',
                            '');

        setEnabledObject(conModeRadio1optRegisterRefundCreditNoteExceptDepositFee);
        ClearScreenByMode(conModeRadio1optRegisterRefundCreditNoteExceptDepositFee);
    }
    else {
        var msgID = result.WarningMessage.shift();
        var param = { "module": "Income", "code": msgID };
        call_ajax_method("/Shared/GetMessage", param, function (data) {
            OpenOkCancelDialog(data.Code, data.Message, function () {
                ExceptDepositFeeAddToGrid(result);
            }, null);
        });
    }
}

function btn_DepositFeeAdd_click() {
    // check add duplicate
    var strTaxInvoiceNo = $("#txtDepositFeeTaxInvoiceNoForCreditNote").val();
    var bolDubTransaction = false;

    for (var i = 0; i < grdCancelCreditNote.getRowsNum(); i++) {

        var row_id = grdCancelCreditNote.getRowId(i);

        if (strTaxInvoiceNo.toUpperCase() == grdCancelCreditNote.cells(row_id, grdCancelCreditNote.getColIndexById("TaxInvoiceNo")).getValue().toUpperCase()) {
            bolDubTransaction = true;
            break;
        }
    }

    if (bolDubTransaction) {
        /* --- Get Message --- */
        var obj = {
            module: "Income",
            code: "MSG7103",
            param: [strTaxInvoiceNo]
        };
        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
            OpenInformationMessageDialog(result.Code, result.Message,
        function () {
            // no funcition
        },
        null);
        });
    } else {

        var obj = GetUserAdjustData();
        ajax_method.CallScreenController("/Income/ICS070_DepositFeeAdd", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                //Confirm warning message before add to grid
                DepositFeeAddToGrid(result);
            }
            else if (controls != undefined) {
                VaridateCtrl(controls, controls);
            }
        });
    }
}

function DepositFeeAddToGrid(result) {
    if (result.WarningMessage == undefined || result.WarningMessage.length == 0) {
        // add grd

        bolViewMode = false;


        Add_CancelCreditNoteBlankLine(
                            grdCancelCreditNote.getRowsNum(),
                            result.strRegContent,
                            $("#txtDepositFeeCreditNoteAmountIncludeVat").NumericCurrencyText() + ' ' + $("#txtDepositFeeCreditNoteAmountIncludeVat").val(),
                            $("#txtDepositFeeCreditNoteAmount").NumericCurrencyText() + ' ' + $("#txtDepositFeeCreditNoteAmount").val(),
                            $("#dtpDepositFeeCreditNoteDate").val(),
                            $("#txtDepositFeeBillingCode1").val() + '-' + $("#txtDepositFeeBillingCode2").val(),
                            '',
                             $("#txtDepositFeeBillingClientNameLC").val(),

                            ICS070_ViewBag.C_CN_TYPE_REFUND_DEPOSIT,
                            $("#txtDepositFeeTaxInvoiceNoForCreditNote").val().toUpperCase(),
                            result.strBillingTargetCode,

                            $("#txtDepositFeeCreditNoteAmountIncludeVat").val(),
                            $("#txtDepositFeeCreditNoteAmount").val(),
                            ICS070_ViewBag.C_BILLING_TYPE_DEPOSIT,
                            $("#txtDepositFeeApproveNo").val(),
                            $("#txtaDepositFeeIssueReason").val(),
                            $("#txtDepositFeeTaxInvoiceAmountIncludeVat").val(),
                            $("#txtDepositFeeTaxInvoiceAmount").val(),
                            $("#dtpDepositFeeTaxInvoiceDate").val(),
                            '',
                            '',
                            '',
                            '',
                            '',
                            '',
                            result.strInvoiceNo,
                            result.strInvoiceOCC,
                            result.bolNotRetrieveFlag,
                            $("#txtDepositFeeBillingClientNameEN").val(),
                            $("#txtDepositFeeCreditNoteAmountIncludeVat").val(),
                            $("#txtDepositFeeCreditNoteAmount").val(),
                            $("#txtDepositFeeCreditNoteAmountIncludeVat").NumericCurrencyValue(),
                            $("#txtDepositFeeCreditNoteAmount").NumericCurrencyValue(),
                            '',
                            '');

        setEnabledObject(conModeRadio1optRegisterRefundCreditNoteDepositFee);
        ClearScreenByMode(conModeRadio1optRegisterRefundCreditNoteDepositFee);
    }
    else {
        var msgID = result.WarningMessage.shift();
        var param = { "module": "Income", "code": msgID };
        call_ajax_method("/Shared/GetMessage", param, function (data) {
            OpenOkCancelDialog(data.Code, data.Message, function () {
                DepositFeeAddToGrid(result);
            }, null);
        });
    }
}

function btn_RevenueAdd_click() {
    // check add duplicate
    var strBillingCode = $("#txtRevenueBillingCode1").val() + '-' + $("#txtRevenueBillingCode2").val();
    var bolDubTransaction = false;

    for (var i = 0; i < grdCancelCreditNote.getRowsNum(); i++) {
        var row_id = grdCancelCreditNote.getRowId(i);
        if (strBillingCode.toUpperCase() == grdCancelCreditNote.cells(row_id, grdCancelCreditNote.getColIndexById("BillingCode")).getValue().toUpperCase()) {
            bolDubTransaction = true;
            break;
        }
    }


    if (bolDubTransaction) {
        /* --- Get Message --- */
        var obj = {
            module: "Income",
            code: "MSG7107",
            param: [strBillingCode]
        };
        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
            OpenInformationMessageDialog(result.Code, result.Message,
        function () {
            // no funcition
        },
        null);
        });
    } else {
        var obj = GetUserAdjustData();
        ajax_method.CallScreenController("/Income/ICS070_RevenueAdd", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                //Confirm warning message before add to grid
                RevenueAddToGrid(result);
            }
            else if (controls != undefined) {
                VaridateCtrl(controls, controls);
            }
        });
    }
}

function RevenueAddToGrid(result) {
    if (result.WarningMessage == undefined || result.WarningMessage.length == 0) {
        CloseWarningDialog();

        // add grd

        bolViewMode = false;

        Add_CancelCreditNoteBlankLine(
                            grdCancelCreditNote.getRowsNum(),
                            result.strRegContent,
                            $("#txtRevenueRevenueDepositFee").NumericCurrencyText() + ' ' + $("#txtRevenueRevenueDepositFee").val(),
                            $("#txtRevenueRevenueVatAmount").NumericCurrencyText() + ' ' + $("#txtRevenueRevenueVatAmount").val(),
                            '-',
                            $("#txtRevenueBillingCode1").val().toUpperCase() + '-' + $("#txtRevenueBillingCode2").val().toUpperCase(),
                            '',
                             $("#txtRevenueBillingClientNameLC").val(),

                            result.strCreditNoteType,
                            '',
                            result.strBillingTargetCode,

                            '',
                            '',
                            '',
                            '',
                            '',
                            '',
                            '',
                            '',
                            '',
                            '',
                            '',
                            '', // today
                            $("#txtRevenueRevenueDepositFee").val(),
                            $("#txtRevenueRevenueVatAmount").val(),
                            result.strInvoiceNo,
                            result.strInvoiceOCC,
                            result.bolNotRetrieveFlag,
                            $("#txtRevenueBillingClientNameEN").val(),
                            $("#txtRevenueRevenueDepositFee").val(),
                            $("#txtRevenueRevenueVatAmount").val(),
                            $("#txtRevenueRevenueDepositFee").NumericCurrencyValue(),
                            $("#txtRevenueRevenueVatAmount").NumericCurrencyValue(),
                            $("#txtRevenueRevenueDepositFee").NumericCurrencyValue(),
                            $("#txtRevenueRevenueVatAmount").NumericCurrencyValue());

        setEnabledObject(conModeRadio1optRevenueDepositFee);
        ClearScreenByMode(conModeRadio1optRevenueDepositFee);
    }
    else {
        var msgID = result.WarningMessage.shift();
        var param = { "module": "Income", "code": msgID };
        call_ajax_method("/Shared/GetMessage", param, function (data) {
            OpenOkCancelDialog(data.Code, data.Message, function () {
                RevenueAddToGrid(result);
            }, null);
        });
    }
}

function btn_NextToRegister_click() {


    setAllObjectToConfirmMode(conNo);
    setVisableTable(0);
    setFormMode(conModeInit);
    bolViewMode = false;

    ClearScreenToInitStage();

    // Show delete button in grid
    var colBtnRemoveIdx1 = grdCancelCreditNote.getColIndexById("Del");
    grdCancelCreditNote.setColumnHidden(colBtnRemoveIdx1, false);
    // Concept by P'Leing
    SetFitColumnForBackAction(grdCancelCreditNote, "TempSpan");


}
//number_format(parseFloat( , 1, '.', ',')
function number_format(number, decimals, dec_point, thousands_sep) {

    if (number == '') {
        number = '0'
    }

    if (number == undefined) {
        number = '0'
    }
    number = (number + '').replace(/[^0-9+\-Ee.]/g, '');
    var n = !isFinite(+number) ? 0 : +number,
        prec = !isFinite(+decimals) ? 0 : Math.abs(decimals), sep = (typeof thousands_sep === 'undefined') ? ',' : thousands_sep,
        dec = (typeof dec_point === 'undefined') ? '.' : dec_point,
        s = '',
        toFixedFix = function (n, prec) {
            var k = Math.pow(10, prec); return '' + Math.round(n * k) / k;
        };
    // Fix for IE parseFloat(0.55).toFixed(0) = 0;
    s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.');
    if (s[0].length > 3) {
        s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
    }
    if ((s[1] || '').length < prec) {
        s[1] = s[1] || '';
        s[1] += new Array(prec - s[1].length + 1).join('0');
    }

    return s.join(dec);
}