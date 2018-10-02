/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

/*--- Main ---*/
var pageRow = ROWS_PER_PAGE_FOR_SEARCHPAGE;
var gridDocumentListCTS180;
var gridContractReportListCTS180;
var gridCancelContractCTS180;
var gridQuotationCancelContractCTS180;
var documentCode;
var isViewMode;
var currRowId = null; //Add by Jutarat A. on 09082012

$(document).ready(function () {
    InitialEvent();
    SetInitialState();
});

function RegisterCommandControl() {
    SetRegisterCommand(true, command_register_click);
    SetResetCommand(true, command_reset_click);
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
}

function ConfirmCommandControl() {
    SetRegisterCommand(false, null);
    SetResetCommand(false, null);
    SetConfirmCommand(true, command_confirm_click);
    SetBackCommand(true, command_back_click);
}

function HideCommandControl() {
    SetRegisterCommand(false, null);
    SetResetCommand(false, null);
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
}

function InitialEvent() {
    $("#btnSearch").click(search_button_click);
    $("#btnClear").click(clear_button_click);

    $("#txtNegotiationStaff1Name").InitialAutoComplete("/Master/MAS070_GetEmployeeName");
}

function SetInitialState() {
    isViewMode = false;

    //$("#divDocumentList").hide();
    //HideDocumentSection();
    InitialInputData();
    SetDocumentSectionMode(false);

    HideCommandControl();
}

function SetResetState() {
    SetInitialState();

    $("#divDocumentList").hide();
    HideDocumentSection();
}

function SetViewState() {
    isViewMode = false;

    SetDocumentSectionMode(false);
    HideDocumentSection();
    DiableSpecifySearchCondition(false);

    HideCommandControl();
}

function HideDocumentSection() {
    VisibleDocumentSectionCTS180_02(false);
    VisibleDocumentSectionCTS180_03(false);
    VisibleDocumentSectionCTS180_04(false);
    VisibleDocumentSectionCTS180_05(false);
    VisibleDocumentSectionCTS180_06(false);
    VisibleDocumentSectionCTS180_07(false);
}

function InitialInputData() {
    InitialInputDataCTS180_01();
    InitialInputDataCTS180_02();
    InitialInputDataCTS180_03();
    InitialInputDataCTS180_04();
    InitialInputDataCTS180_05();
    InitialInputDataCTS180_06();
    InitialInputDataCTS180_07();
}

function SetRegisterState(docCode) {
    isViewMode = true;

    DiableSpecifySearchCondition(true);

    //GenerateSelectButtonDocumentList(false);
    GridControl.LockGrid(gridDocumentListCTS180);
    GridControl.SetDisabledButtonOnGrid(gridDocumentListCTS180, "btnSelect", "Select", true);

    if (docCode == $("#C_DOCUMENT_CODE_CONTRACT_EN").val()
        || docCode == $("#C_DOCUMENT_CODE_CONTRACT_TH").val()) {

        VisibleDocumentSectionCTS180_02(true);
        SetRegisterStateCTS180_02();
        master_event.ScrollWindow("#divContractReport", false);
    }
    else if (docCode == $("#C_DOCUMENT_CODE_CHANGE_MEMO").val()
        || docCode == $("#C_DOCUMENT_CODE_CHANGE_NOTICE").val()) {

        VisibleDocumentSectionCTS180_03(true);
        SetRegisterStateCTS180_03(docCode);
        master_event.ScrollWindow("#divChangeMemo", false);
    }
    else if (docCode == $("#C_DOCUMENT_CODE_CONFIRM_CURRENT_INSTRUMENT_MEMO").val()) {
        VisibleDocumentSectionCTS180_04(true);
        SetRegisterStateCTS180_04();
        master_event.ScrollWindow("#divConfirmCurrent", false);
    }
    else if (docCode == $("#C_DOCUMENT_CODE_CONCEL_CONTRACT_MEMO").val()) {
        VisibleDocumentSectionCTS180_05(true);
        SetRegisterStateCTS180_05();
        master_event.ScrollWindow("#divCancelContractMemo", false);
    }
    else if (docCode == $("#C_DOCUMENT_CODE_CHANGE_FEE_MEMO").val()) {
        VisibleDocumentSectionCTS180_06(true);
        SetRegisterStateCTS180_06();
        master_event.ScrollWindow("#divChangeFeeMemo", false);
    }
    else if (docCode == $("#C_DOCUMENT_CODE_CANCEL_CONTRACT_QUOTATION").val()) {
        VisibleDocumentSectionCTS180_07(true);
        SetRegisterStateCTS180_07();
        master_event.ScrollWindow("#divQuotationCancelContract", false);
    }
    else {
        HideDocumentSection();
    }

    RegisterCommandControl();
}

function DiableSpecifySearchCondition(isDisable) {
    $("#chkNotIssued").attr("disabled", isDisable);
    $("#chkIssuedButNoRegist").attr("disabled", isDisable);
    $("#chkCollectionRegist").attr("disabled", isDisable);
    $("#chkIssuedButNotUsed").attr("disabled", isDisable);

    $("#txtContQuotProjCode").attr("readonly", isDisable);
    $("#txtOCCAlphabet").attr("readonly", isDisable);
    $("#cboContractOffice").attr("disabled", isDisable);
    $("#cboOperationOffice").attr("disabled", isDisable);
    $("#txtNegotiationStaff1Code").attr("readonly", isDisable);
    $("#txtNegotiationStaff1Name").attr("readonly", isDisable);
    $("#cboDocumentCode").attr("disabled", isDisable);
    $("#btnSearch").attr("disabled", isDisable);
}

function search_button_click() {
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    var strDocStatus = "";
    if ($("#chkNotIssued").prop("checked")) {
        strDocStatus = strDocStatus + $("#chkNotIssued").val() + ",";
    }

    if ($("#chkIssuedButNoRegist").prop("checked")) {
        strDocStatus = strDocStatus + $("#chkIssuedButNoRegist").val() + ",";
    }

    if ($("#chkCollectionRegist").prop("checked")) {
        strDocStatus = strDocStatus + $("#chkCollectionRegist").val() + ",";
    }

    if ($("#chkIssuedButNotUsed").prop("checked")) {
        strDocStatus = strDocStatus + $("#chkIssuedButNotUsed").val();
    }

    var objCond = {
        DocStatus: strDocStatus,
        ContractCode: $("#txtContQuotProjCode").val(),
        QuotationTargetCode: $("#txtContQuotProjCode").val(),
        ProjectCode: $("#txtContQuotProjCode").val(),
        OCC: $("#txtOCCAlphabet").val(),
        Alphabet: $("#txtOCCAlphabet").val(),
        ContractOfficeCode: $("#cboContractOffice").val(),
        OperationOfficeCode: $("#cboOperationOffice").val(),
        NegotiationStaffEmpNo: $("#txtNegotiationStaff1Code").val(),
        NegotiationStaffEmpName: $("#txtNegotiationStaff1Name").val(),
        DocumentCode: $("#cboDocumentCode").val()
    }

    $("#gridDocumentList").LoadDataToGrid(gridDocumentListCTS180, pageRow, true, "/Contract/CTS180_GetDocumentListData", objCond, "CTS180_DocumentListGridData", false,
        function (result, controls, isWarning) {
            if (isWarning == undefined) {
                GridControl.UnlockGrid(gridDocumentListCTS180);
                $("#divDocumentList").show();

                //document.getElementById('divDocumentList').scrollIntoView();
                master_event.ScrollWindow("#divDocumentList", false);

                //DiableSpecifySearchCondition(true);
            }

            master_event.LockWindow(false);
            $("#btnSearch").attr("disabled", false);
        }

        , function () {
            $("#divDocumentList").show();
        });

//Move to set when InitialGrid()
//    SpecialGridControl(gridDocumentListCTS180, ["Select"]);

//    BindOnLoadedEvent(gridDocumentListCTS180,
//        function () {
//            gridDocumentListCTS180.setColumnHidden(gridDocumentListCTS180.getColIndexById("IsEnableSelect"), true);
//            gridDocumentListCTS180.setColumnHidden(gridDocumentListCTS180.getColIndexById("DocID"), true);
//            gridDocumentListCTS180.setColumnHidden(gridDocumentListCTS180.getColIndexById("DocumentCode"), true);

//            GenerateSelectButtonDocumentList(isViewMode == false);
//        }
//    );

    gridDocumentListCTS180.setSizes();
}

function GenerateSelectButtonDocumentList(isEnable, gen_ctrl) {
    for (var i = 0; i < gridDocumentListCTS180.getRowsNum(); i++) {
        var row_id = gridDocumentListCTS180.getRowId(i);

        if (isEnable) {
            var isEnableColinx = gridDocumentListCTS180.getColIndexById("IsEnableSelect");
            isEnable = gridDocumentListCTS180.cells(row_id, isEnableColinx).getValue();
        }

        if (gen_ctrl == true) {
            GenerateSelectButton(gridDocumentListCTS180, "btnSelect", row_id, "Select", isEnable);
        }

        /* --- Set Event --- */
        /* ----------------- */
        BindGridButtonClickEvent("btnSelect", row_id,
                    function (row_id) {
                        SelectDocumentReport(row_id);
                    }
                );
        /* ----------------- */
    }

    gridDocumentListCTS180.setSizes();
}

function SelectDocumentReport(row_id) {
    currRowId = row_id; //Add by Jutarat A. on 09082012

    gridDocumentListCTS180.selectRow(gridDocumentListCTS180.getRowIndex(row_id));

    var docIDCol = gridDocumentListCTS180.getColIndexById("DocID");
    var docID = gridDocumentListCTS180.cells(row_id, docIDCol).getValue();

    var docCodeCol = gridDocumentListCTS180.getColIndexById("DocumentCode");
    documentCode = gridDocumentListCTS180.cells(row_id, docCodeCol).getValue();

    var obj = { iDocID: docID };
    call_ajax_method_json("/Contract/CTS180_SelectDocumentReport", obj,
        function (result) {
            if (result != undefined && result.length == 2) {
                HideDocumentSection();

                if (documentCode == $("#C_DOCUMENT_CODE_CONTRACT_EN").val()
                    || documentCode == $("#C_DOCUMENT_CODE_CONTRACT_TH").val()) {

                    BindDocumentDataCTS180_02(result);
                }
                else if (documentCode == $("#C_DOCUMENT_CODE_CHANGE_MEMO").val()
                    || documentCode == $("#C_DOCUMENT_CODE_CHANGE_NOTICE").val()) {

                    BindDocumentDataCTS180_03(result);
                }
                else if (documentCode == $("#C_DOCUMENT_CODE_CONFIRM_CURRENT_INSTRUMENT_MEMO").val()) {
                    BindDocumentDataCTS180_04(result);
                }
                else if (documentCode == $("#C_DOCUMENT_CODE_CONCEL_CONTRACT_MEMO").val()) {
                    BindDocumentDataCTS180_05(result);
                }
                else if (documentCode == $("#C_DOCUMENT_CODE_CHANGE_FEE_MEMO").val()) {
                    BindDocumentDataCTS180_06(result);
                }
                else if (documentCode == $("#C_DOCUMENT_CODE_CANCEL_CONTRACT_QUOTATION").val()) {
                    BindDocumentDataCTS180_07(result);
                }

                SetRegisterState(documentCode);
            }
        }
    );

}

function clear_button_click() {
    CloseWarningDialog();
    //SetInitialState();
    SetResetState();
}

function SetDocumentSectionMode(isView) {
    SetDocumentSectionModeCTS180_01(isView);
    SetDocumentSectionModeCTS180_02(isView);
    SetDocumentSectionModeCTS180_03(isView);
    SetDocumentSectionModeCTS180_04(isView);
    SetDocumentSectionModeCTS180_05(isView);
    SetDocumentSectionModeCTS180_06(isView);
    SetDocumentSectionModeCTS180_07(isView);
}

function GetDocumentData() {
    var objData = null;
    if (documentCode == $("#C_DOCUMENT_CODE_CONTRACT_EN").val()
        || documentCode == $("#C_DOCUMENT_CODE_CONTRACT_TH").val()) {

        objData = GetDocumentDataCTS180_02();
    }
    else if (documentCode == $("#C_DOCUMENT_CODE_CHANGE_MEMO").val()
        || documentCode == $("#C_DOCUMENT_CODE_CHANGE_NOTICE").val()) {

        objData = GetDocumentDataCTS180_03();
    }
    else if (documentCode == $("#C_DOCUMENT_CODE_CONFIRM_CURRENT_INSTRUMENT_MEMO").val()) {
        objData = GetDocumentDataCTS180_04();
    }
    else if (documentCode == $("#C_DOCUMENT_CODE_CONCEL_CONTRACT_MEMO").val()) {
        objData = GetDocumentDataCTS180_05();
    }
    else if (documentCode == $("#C_DOCUMENT_CODE_CHANGE_FEE_MEMO").val()) {
        objData = GetDocumentDataCTS180_06();
    }
    else if (documentCode == $("#C_DOCUMENT_CODE_CANCEL_CONTRACT_QUOTATION").val()) {
        objData = GetDocumentDataCTS180_07();
    }

    return objData;
}

function command_register_click() {
    command_control.CommandControlMode(false);

    var objData = GetDocumentData();
    call_ajax_method_json("/Contract/CTS180_RegisterContractDocument", objData, doAfterRegister);

    //command_control.CommandControlMode(true);
}

function doAfterRegister(result, controls) {
    if (controls != undefined) {
        /* --- Higlight Text --- */
        /* --------------------- */

        var contOrderCtrlName;
        if (gridContractReportListCTS180 != undefined) {
            var row_id = gridContractReportListCTS180.getRowId(0);
            contOrderCtrlName = GenerateGridControlID("OrderFee", row_id);
        }

        VaridateCtrl([//"OrderFee0",
                        contOrderCtrlName,
                        "CM_ChangeDate",
                        "CC_CancelDate",
                        "CC_AutoTransferBillingAmount",
                        "CC_BankTransferBillingAmount",
                        "CC_BankTransferBillingAmountUsd",
                        "QC_CancelDate",
                        "CHF_ChangeContractFeeDate",
                        "CHF_NewContractFee"], controls);
        return;
    }
    else if (result != undefined) {
        /* --- Set View Mode --- */
        /* --------------------- */
        SetDocumentSectionMode(true);

        ConfirmCommandControl();
    }
}

function command_reset_click() {
    /* --- Get Message --- */
    /* ------------------- */
    var obj = {
        module: "Common",
        code: "MSG0038"
    };
    call_ajax_method_json("/Shared/GetMessage", obj,
        function (result) {
            OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    //SetInitialState();

                    //Modify by Jutarat A. on 09082012
                    //SetResetState();
                    if (currRowId != null) {
                        SelectDocumentReport(currRowId);
                    }
                    //End Modify
                },
                null);
        }
    );
    /* ------------------- */
}

function command_back_click() {
    SetDocumentSectionMode(false);

    RegisterCommandControl();
}

function command_confirm_click() {
    command_control.CommandControlMode(false);

    var objData = GetDocumentData();
    call_ajax_method_json("/Contract/CTS180_ConfirmContractDocument", objData,
        function (result) {
            if (result != undefined) {
                OpenInformationMessageDialog(result.Code, result.Message,
                    function () {
                        //SetDocumentSectionMode(false);
                        //SetResetState();

                        search_button_click();
                        SetViewState();
                    }
                );
            }
        }
    );

    //command_control.CommandControlMode(true);
}












