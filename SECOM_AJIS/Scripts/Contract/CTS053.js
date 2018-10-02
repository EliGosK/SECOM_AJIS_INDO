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

var numBox_Length = 12;
var numBox_Decimal = 2;
var numBox_Min = 0;
var numBox_Max = 999999999999.99;
var numBox_DefaultMin = false;

var contractDat = null;
var changeFeeDat = null;
var billingTempDat = null;
var billingTempObj = null;
var currentMailList = null;

var validateEmail = ["EmailAddress"];
var validateChangePlanForm = ["ChangeDateContractFee", "ChangeContractFee", "ReturnToOriginalFeeDate", "NegotiationStaffEmpNo1", "NegotiationStaffEmpNo2", "ApproveNo1"];
var validateChangePlanDetail = ['BillingOffice', 'BillingContractFee'];
var validateRetrieveBillingTempDetail = ['BillingTargetCodeSearch', 'BillingClientCodeSearch'];

var gridNotify = null;
var gridBillingTemp = null;
var btnEmailRemoveID = "btnRemoveEmail";
var btnBillingTempRemoveID = "btnRemoveBillingTemp";
var btnBillingTempDetailID = "btnDetailBillingTemp";

// ---------------------------------------------------------------------------------
// Initial
// ---------------------------------------------------------------------------------
$(document).ready(function () {
    _isp1 = (_isp1.toUpperCase() == "TRUE") ? true : false;
    InitialPage();
});

function InitialPage() {
    $('#ChangeDateContractFee').InitialDate();
    $('#ReturnToOriginalFeeDate').InitialDate();

    $('#CurrentContractFee').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#CurrentContractFee').setComma();
    $('#ChangeContractFee').BindNumericBox(11, numBox_Decimal, numBox_Min, 16666666666.00, numBox_DefaultMin);
    $('#ChangeContractFee').setComma();
    $('#BillingContractFee').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#BillingContractFee').setComma();

    GetEmployeeNameData("#NegotiationStaffEmpNo1", "#NegotiationStaffEmpName1");
    GetEmployeeNameData("#NegotiationStaffEmpNo2", "#NegotiationStaffEmpName2");

    $('#ChangeFeeFlag').change(ChangeFeeFlag_change);
    $('#btnAddEmail').click(btnAddEmail_click);
    $('#btnClearEmail').click(btnClearEmail_click);
    $('#btnSearchEmail').click(btnSearchEmail_click);
    $('#btnNewBillingTarget').click(btnNewBillingTarget_click);
    $('#rdoTargetCode').change(specifyCode_Change);
    $('#rdoClientCode').change(specifyCode_Change);
    
    $('#btnRetrieveBillingTarget').click(btnRetrieveBillingTarget_click);
    $('#btnRetrieveBillingClient').click(btnRetrieveBillingClient_click);

    $('#btnSearchBillingClient').click(btnSearchBillingClient_click);
    $('#btnCopyAddressInfo').click(btnCopyAddressInfo_click);
    $('#btnNewEdit').click(btnNewEdit_click);
    $('#btnClearBillingTarget').click(btnClearBillingTarget_click);
    $('#btnAddUpdate').click(btnAddUpdate_click);
    $('#btnCancelCTS053').click(btnCancelCTS053_click);

    InitialTrimTextEvent(["ApproveNo1", "ApproveNo2", "ApproveNo3", "ApproveNo4", "ApproveNo5"]);

    SetScreenToDefault();
    LoadContractData();
    ShowRegisterPane();
}

// ---------------------------------------------------------------------------------
// Event
// ---------------------------------------------------------------------------------
function ChangeFeeFlag_change() {
    if ($('#ChangeFeeFlag').is(':checked')) {
        $('#ReturnToOriginalFeeDate').EnableDatePicker(false);
        $('#ReturnToOriginalFeeDate').val('');
        $('#NotifyTarget').hide();
    } else {
        $('#ReturnToOriginalFeeDate').EnableDatePicker(true);
        $('#ReturnToOriginalFeeDate').val('');
        $('#NotifyTarget').show();
    }
}

function btnAddEmail_click() {
    VaridateCtrl(validateEmail, null);
    var obj = {
        "emailAddress": new Array(),
        "fromSearch": false
    };

    obj.emailAddress[obj.emailAddress.length] = $('#EmailAddress').val();

    call_ajax_method_json("/Contract/CTS053_AddEmail", obj, function (result, control) {
        VaridateCtrl(VaridateCtrl, control);
        if ((result != null) && result) {
            ReloadEmailGrid();
            $('#EmailAddress').val('');
        }
    });
}

function btnRemoveEmail_click(row_id) {
    var _colEmail = gridNotify.getColIndexById("EmailAddress");
    var currentEmail = gridNotify.cells(row_id, _colEmail).getValue();

    var obj = {
        "emailAddress": currentEmail
    };

    call_ajax_method_json("/Contract/CTS053_RemoveEmail", obj, function (result, control) {
        if ((result != null) && result) {
            ReloadEmailGrid();
        }
    });
}

function btnClearEmail_click() {
//    call_ajax_method_json("/Contract/CTS053_ClearEmail", "", function (result) {
//        if ((result != null) && result) {
//            ReloadEmailGrid();
//        }
//    });

    if ((result != null) && result) {
        $('#EmailAddress').val('');
        ReloadEmailGrid();
    }
}

function btnSearchEmail_click() {
    call_ajax_method_json("/Contract/CTS053_RetrieveEmailList", "", function (result, controls) {
        if (result != null) {
            currentMailList = result;
            $("#dlgCTS053").OpenCMS060Dialog("CTS053");
        }
    });
}

function btnDetail_Click(rowid) {
    var currentBillingTemp = GetTempBillingObj(rowid);
    call_ajax_method_json("/Contract/CTS053_LoadBillingTempData", currentBillingTemp, function (result, controls) {
        if (result != null) {
            billingTempDat = result;
            billingTempDat.IsNewItem = false;
            BindingBillingTempData();
            ShowBillingTargetDetail();
        }
    });
}

function btnRemove_Click(rowid) {
    var tmpBillingObjItem = GetTempBillingObj(rowid);
    call_ajax_method_json("/Contract/CTS053_DeleteBillingItem", tmpBillingObjItem, function (result, controls) {
        if ((result != null) && result) {
            ReloadBillingTempGrid();
        }
    });
}

function btnNewBillingTarget_click() {
    call_ajax_method_json("/Contract/CTS053_NewBillingTempData", "", function (result, controls) {
        if (result != null) {
            billingTempDat = result;
            billingTempDat.IsNewItem = true;
            BindingBillingTempData();
            ShowBillingTargetDetail();
        }
    });
}

function specifyCode_Change() {
    var currentCode = GetSpecifyCodeBillingTarget();

    if (currentCode == '0') {
        $('#BillingTargetCodeSearch').val('');
        $('#BillingTargetCodeSearch').removeAttr('readonly');
        $("#BillingTargetCodeSearch").ResetToNormalControl();

        $('#BillingClientCodeSearch').val('');
        $('#BillingClientCodeSearch').attr('readonly', 'readonly');

        $("#btnRetrieveBillingTarget").removeAttr("disabled");
        $("#btnRetrieveBillingClient").attr("disabled", "disabled");
    } else if (currentCode == '1') {
        $('#BillingTargetCodeSearch').val('');
        $('#BillingTargetCodeSearch').attr('readonly', 'readonly');
        $('#BillingClientCodeSearch').val('');
        $('#BillingClientCodeSearch').removeAttr('readonly');
        $("#BillingClientCodeSearch").ResetToNormalControl();

        $("#btnRetrieveBillingTarget").attr("disabled", "disabled");
        $("#btnRetrieveBillingClient").removeAttr("disabled");
    }
}

function btnRetrieveBillingTarget_click() {
    LockButton('btnRetrieveBillingTarget', true);
    LockButton('btnSearchBillingClient', true);
    VaridateCtrl(validateRetrieveBillingTempDetail, null);
    var obj = {
        BillingTargetCode: $('#BillingTargetCodeSearch').val()
    }

    call_ajax_method_json("/Contract/CTS053_RetrieveBillingTargetDetailFromCode", obj, function (result, controls) {
        LockButton('btnRetrieveBillingTarget', false);
        LockButton('btnSearchBillingClient', false);
        VaridateCtrl(validateRetrieveBillingTempDetail, controls);
        if (result != null) {
            billingTempDat.CustomerType = result.CustomerType;
            billingTempDat.BillingOCC = result.BillingOCC;
            billingTempDat.BillingTargetCode = result.BillingTargetCode;
            billingTempDat.BillingClientCode = result.BillingClientCode;
            $('#BillingTargetDetail').bindJSON(result);
            SetEnableDisableItemOnBillingTargetDetail();
        }
    });
}
function btnRetrieveBillingClient_click() {
    LockButton('btnRetrieveBillingClient', true);
    LockButton('btnSearchBillingClient', true);
    VaridateCtrl(validateRetrieveBillingTempDetail, null);

    var obj = {
        BillingClientCode: $('#BillingClientCodeSearch').val()
    }

    call_ajax_method_json("/Contract/CTS053_RetrieveBillingClientDetailFromCode", obj, function (result, controls) {
        LockButton('btnRetrieveBillingClient', false);
        LockButton('btnSearchBillingClient', false);
        VaridateCtrl(validateRetrieveBillingTempDetail, controls);
        if (result != null) {
            billingTempDat.CustomerType = result.CustomerType;
            billingTempDat.BillingOCC = result.BillingOCC;
            billingTempDat.BillingTargetCode = result.BillingTargetCode;
            billingTempDat.BillingClientCode = result.BillingClientCode;
            $('#BillingTargetDetail').bindJSON(result);
            SetEnableDisableItemOnBillingTargetDetail();
        }
    });
}

function btnSearchBillingClient_click() {
    $("#dlgCTS053").OpenCMS270Dialog('CTS053');
}

function btnCopyAddressInfo_click() {
    var obj = {
        AddressCopyInfo: GetAddressInformation()
    }

    call_ajax_method_json("/Contract/CTS053_RetrieveBillingDetailFromCopy", obj, function (result, controls) {
        if (result != null) {
            $('#BillingTargetDetail').bindJSON(result);
            billingTempDat.CustomerType = result.CustomerType;
            billingTempDat.CompanyTypeCode = result.CompanyTypeCode;
            SetEnableDisableItemOnBillingTargetDetail();

            SetEnableCopyNameAddressInfo(false);
            SetEnableSpecifyCode(false);
        }
    });
}

function btnNewEdit_click() {
    $("#dlgCTS053").OpenMAS030Dialog('CTS053');
}

function btnClearBillingTarget_click() {
    $('#divViewBillingClientDetail').clearForm();
    billingTempDat.BillingTargetCode = null;
    billingTempDat.BillingClientCode = null;
    billingTempDat.CustomerType = null;
    billingTempDat.CompanyTypeCode = null;

    $('#BillingTargetCode').val('');
    $('#BillingClientCode').val('');

    SetEnableDisableItemOnBillingTargetDetail();
}

function btnAddUpdate_click() {
    LockButton('btnAddUpdate', true);
    VaridateCtrl(validateChangePlanDetail, null);

    var formObj = CreateObjectData($('#frmChangePlanDetail').serialize());
    formObj = ReCorrectingObjectValue_ChangePlanDetail(formObj);
    formObj.ObjectType = billingTempDat.ObjectType;
    formObj.BillingOCC = billingTempDat.BillingOCC;
    formObj.CustomerType = billingTempDat.CustomerType;
    formObj.CompanyTypeCode = billingTempDat.CompanyTypeCode;
    formObj.OldOfficeCode = billingTempDat.OldOfficeCode;
    formObj.UID = billingTempDat.UID;
    formObj.RegionCode = billingTempDat.RegionCode;
    formObj.BillingClientCode = billingTempDat.BillingClientCode;
    formObj.BillingTargetCode = billingTempDat.BillingTargetCode;
    formObj.BillingOffice = $('#BillingOffice').val();

    formObj.OriginalBillingOCC = billingTempDat.OriginalBillingOCC;
    formObj.OriginalBillingClientCode = billingTempDat.OriginalBillingClientCode;
    formObj.OriginalBillingOfficeCode = billingTempDat.OriginalBillingOfficeCode;
    //formObj.SequenceNo = billingTempDat.SequenceNo;

    var obj = {
        targObj: formObj
    }

    call_ajax_method_json("/Contract/CTS053_ValidateBusiness_ChangePlanDetail", obj, function (result, controls) {
        LockButton('btnAddUpdate', false);
        VaridateCtrl(validateChangePlanDetail, controls);
        if ((result != null) && result) {
            btnCancelCTS053_click();
            ReloadBillingTempGrid();
        }
    });
}

function btnCancelCTS053_click() {
    billingTempDat = null;
    HideBillingTargetDetail();
    FreezeBillingGrid(false);
}

function Register_Click() {
    VaridateCtrl(validateChangePlanForm, null);
    DisableRegisterCommand(true);
    DisableResetCommand(true);

    var obj = CreateObjectData($('#frmChangePlan').serialize());
    obj = ReCorrectingObjectValue(obj);
    obj.DivideBillingContractFee = $('#DivideBillingContractFee').is(':checked');
    obj.HasChangePlanDetailTask = (billingTempDat != null);

    call_ajax_method_json("/Contract/CTS053_ValidateBusiness_All", obj, function (result, controls) {
        VaridateCtrl(validateChangePlanForm, controls);
        if ((result != null) && result) {
            SetScreenToViewMode(true);
            HideRegisterPane();
            ShowConfirmPane();
        }

        DisableRegisterCommand(false);
        DisableResetCommand(false);
    });
}

function Reset_Click() {
    VaridateCtrl(validateChangePlanForm, null);
    DisableRegisterCommand(true);
    DisableResetCommand(true);
    doAskYesNo("Common", "MSG0038", null, function () {
        call_ajax_method_json("/Contract/CTS053_ResetData", "", function (result, controls) {
            SetScreenToDefault();
            LoadContractData();
            DisableRegisterCommand(false);
            DisableResetCommand(false);
        });
    },
    function () {
        DisableRegisterCommand(false);
        DisableResetCommand(false);
    });
}

function Confirm_Click() {
    DisableConfirmCommand(true);

    var obj = CreateObjectData($('#frmChangePlan').serialize());
    obj = ReCorrectingObjectValue(obj);
    obj.DivideBillingContractFee = $('#DivideBillingContractFee').is(':checked');
    obj.HasChangePlanDetailTask = (billingTempDat != null);

    call_ajax_method_json("/Contract/CTS053_ConfirmRegister", obj, function (result, controls) {
        if ((result != null) && result) {
            OpenInformationMessageDialog(result.Code, result.Message, function () {
                window.location.href = generate_url("/Common/CMS020");
            });
        }

        DisableConfirmCommand(false);
    });
}

function Back_Click() {
    DisableConfirmCommand(true);
    SetScreenToViewMode(false);
    HideConfirmPane();
    ShowRegisterPane();
    DisableConfirmCommand(false);
}

function gridNotify_Binding(gen_ctrl) {
    var _colRemoveBtn = gridNotify.getColIndexById("Remove");

    for (var i = 0; i < gridNotify.getRowsNum(); i++) {
        var row_id = gridNotify.getRowId(i);

        if (gen_ctrl) {
            GenerateRemoveButton(gridNotify, btnEmailRemoveID, row_id, "Remove", true);
        }

        BindGridButtonClickEvent(btnEmailRemoveID, row_id, btnRemoveEmail_click);
    }
}

function gridBillingTemp_Binding(gen_ctrl) {
    var _colDetailBtn = gridBillingTemp.getColIndexById("Detail");
    var _colRemoveBtn = gridBillingTemp.getColIndexById("Remove");
    var _colCanDelete = gridBillingTemp.getColIndexById("CanDelete");
    var _colIsNew = gridBillingTemp.getColIndexById("IsNew");
    var _colHasUpdate = gridBillingTemp.getColIndexById("HasUpdate");
    var _colUID = gridBillingTemp.getColIndexById("UID");

    var _colBillingOCC = gridBillingTemp.getColIndexById("BillingOCC");
    var _colBillingClientCode = gridBillingTemp.getColIndexById("BillingClientCode");
    var _colBillingTargetCode = gridBillingTemp.getColIndexById("BillingTargetCode");
    var _colBillingOfficeCdoe = gridBillingTemp.getColIndexById("BillingOfficeCode");

    var _colOriginalBillingOCC = gridBillingTemp.getColIndexById("OriginalBillingOCC");
    var _colOriginalBillingClientCode = gridBillingTemp.getColIndexById("OriginalBillingClientCode");
    var _colOriginalBillingOfficeCdoe = gridBillingTemp.getColIndexById("OriginalBillingOfficeCode");

    for (var i = 0; i < gridBillingTemp.getRowsNum(); i++) {
        var row_id = gridBillingTemp.getRowId(i);

        var billingTempIDObj = {
            GridRowID: row_id,
            BillingOCC: gridBillingTemp.cells(row_id, _colBillingOCC).getValue(),
            BillingClientCode: gridBillingTemp.cells(row_id, _colBillingClientCode).getValue(),
            BillingTargetCode: gridBillingTemp.cells(row_id, _colBillingTargetCode).getValue(),
            BillingOfficeCode: gridBillingTemp.cells(row_id, _colBillingOfficeCdoe).getValue(),
            CanDelete: (gridBillingTemp.cells(row_id, _colCanDelete).getValue() == "1"),
            IsNew: (gridBillingTemp.cells(row_id, _colIsNew).getValue() == "1"),
            HasUpdate: (gridBillingTemp.cells(row_id, _colHasUpdate).getValue() == "1"),
            UID: gridBillingTemp.cells(row_id, _colUID).getValue(),
            OriginalBillingOCC: gridBillingTemp.cells(row_id, _colOriginalBillingOCC).getValue(),
            OriginalBillingClientCode: gridBillingTemp.cells(row_id, _colOriginalBillingClientCode).getValue(),
            OriginalBillingOfficeCode: gridBillingTemp.cells(row_id, _colOriginalBillingOfficeCdoe).getValue()
        }

        AddTempBillingObj(billingTempIDObj);

        if (gen_ctrl) {
            GenerateDetailButton(gridBillingTemp, btnBillingTempDetailID, row_id, "Detail", true);
            GenerateRemoveButton(gridBillingTemp, btnBillingTempRemoveID, row_id, "Remove", billingTempIDObj.CanDelete);
        }

        BindGridButtonClickEvent(btnBillingTempDetailID, row_id, btnDetail_Click);
        BindGridButtonClickEvent(btnBillingTempRemoveID, row_id, btnRemove_Click);
    }
}

// ---------------------------------------------------------------------------------
// Method
// ---------------------------------------------------------------------------------
function SetScreenToViewMode(IsViewMode) {
    EnableBillingGrid(!IsViewMode);
    EnableEmailGrid(!IsViewMode);

    $('#ContractBasicInfo').SetViewMode(IsViewMode);
    $('#ChangeContractFeeAfter').SetViewMode(IsViewMode);
    $('#NotifyTarget').SetViewMode(IsViewMode);
    $('#BillingTarget').SetViewMode(IsViewMode);
    $('#BillingTargetDetail').SetViewMode(IsViewMode);

    if (!IsViewMode) {
        if (changeFeeDat.IsDisableDivideFlag) {
            $('#DivideBillingContractFee').attr('disabled', 'disabled');
        } else {
            $('#DivideBillingContractFee').removeAttr('disabled');
        }

        $('#divSelectEmail').show();
    } else
    {
        $('#divSelectEmail').hide();
    }
}

function EnableEmailGrid(isEnable) {
    if (gridNotify != null) {
        var _colRemove = gridNotify.getColIndexById("Remove");

        gridNotify.setColumnHidden(_colRemove, !isEnable);
        gridNotify.setSizes();

        if (isEnable) {
            SetFitColumnForBackAction(gridNotify, "TmpColumn");
        }
    }
}

function EnableBillingGrid(isEnable) {
    if (isEnable)
    {
        ReloadBillingTempGrid();
    } else
    {
        if (gridBillingTemp != null) {
            var _colRemove = gridBillingTemp.getColIndexById("Remove");
            var _colDetail = gridBillingTemp.getColIndexById("Detail");

            gridBillingTemp.setColumnHidden(_colRemove, !isEnable);
            gridBillingTemp.setColumnHidden(_colDetail, !isEnable);
            gridBillingTemp.setSizes();

            if (!isEnable) {
                SetFitColumnForBackAction(gridBillingTemp, "TmpColumn");
            }
        }
    }
}

function FreezeBillingGrid(IsFreezing) {
    if (IsFreezing) {
        $('#btnNewBillingTarget').attr('disabled', 'disabled');
    } else {
        $('#btnNewBillingTarget').removeAttr('disabled');
    }

    var _colCanDelete = gridBillingTemp.getColIndexById("CanDelete");

    for (var i = 0; i < gridBillingTemp.getRowsNum(); i++) {
        var row_id = gridBillingTemp.getRowId(i);
        var canDeleteThisRow = (gridBillingTemp.cells(row_id, _colCanDelete).getValue() == "1");

        EnableGridButton(gridBillingTemp, btnBillingTempDetailID, row_id, "Detail", !IsFreezing);
        EnableGridButton(gridBillingTemp, btnBillingTempRemoveID, row_id, "Remove", (!IsFreezing && canDeleteThisRow));
    }
}

function BindingBillingTempData() {
    $('#BillingTargetDetail').clearForm();
    $('#BillingTargetDetail').bindJSON(billingTempDat);
    if ((billingTempDat.HasFee == null) || (!billingTempDat.HasFee)) {
        //        if (billingTempDat.IsBefore) {
        //            $('#BillingContractFee').val($('#ChangePlanOrderContractFee').val());
        //            $('#BillingApproveInstallationFee').val($('#ChangePlanApproveInstallationFee').val());
        //            $('#BillingCompleteInstallationFee').val($('#ChangePlanCompleteInstallationFee').val());
        //            $('#BillingStartInstallationFee').val($('#ChangePlanStartInstallationFee').val());
        //            $('#BillingDepositFee').val($('#ChangePlanOrderDepositFee').val());
        //        } else {

        //            $('#BillingContractFee').val($('#ChangePlanOrderContractFee').val());
        //            $('#BillingInstallationFee').val($('#ChangePlanOrderInstallationFee').val());
        //            $('#BillingDepositFee').val($('#ChangePlanOrderDepositFee').val());
        //        }
    } else {
        $('#BillingContractFee').val(parseFloat($('#BillingContractFee').val().replaceAll(',','')).toFixed(2));
        $('#BillingContractFee').setComma();
    }

    //DisableBillingTempGrid();
    FreezeBillingGrid(true);
}

// Add by Jirawat Jannet on 2016-11-14
String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

function ReCorrectingObjectValue_ChangePlanDetail(obj) {
    obj.BillingContractFee = obj.BillingContractFee.replace(/ /g, "").replace(/,/g, "");

    return obj;
}

function ReCorrectingObjectValue(obj) {
    obj.CurrentContractFee = obj.CurrentContractFee.replace(/ /g, "").replace(/,/g, "");
    obj.CurrentContractFeeCurrencyType = $('#CurrentContractFee').NumericCurrencyValue();;
    obj.ChangeContractFee = obj.ChangeContractFee.replace(/ /g, "").replace(/,/g, "");
    obj.ChangeFeeFlag = $('#ChangeFeeFlag').is(':checked');

    return obj;
}

function SetEnableDisableItemOnBillingTargetDetail() {
    // Have BillingOCC

    if (((billingTempDat.BillingOCC == null) || (billingTempDat.BillingOCC == ""))
    && ((billingTempDat.BillingTargetCode == null) || (billingTempDat.BillingTargetCode == ""))
    && ((billingTempDat.BillingClientCode == null) || (billingTempDat.BillingClientCode == ""))
    && ((billingTempDat.CustomerType == null) || (billingTempDat.CustomerType == ""))
    && billingTempDat.IsNewItem) {
        SetEnableSpecifyCode(true);
        SetEnableCopyNameAddressInfo(true);

        SetEnableNewEditAndClear(true);
        SetEnableBillingOffice(true);
    } else if ((billingTempDat.BillingOCC != null) && (billingTempDat.BillingOCC != "")) {
        SetEnableSpecifyCode(false);
        SetEnableCopyNameAddressInfo(false);

        SetEnableNewEditAndClear(false);
        SetEnableBillingOffice(false);
    } else if (((billingTempDat.BillingTargetCode != null) && (billingTempDat.BillingTargetCode != ""))) {
        SetEnableSpecifyCode(false);
        SetEnableCopyNameAddressInfo(false);

        SetEnableNewEditAndClear(true);
        SetEnableBillingOffice(false);
    } else if (((billingTempDat.BillingClientCode != null) && (billingTempDat.BillingClientCode != ""))) {
        SetEnableSpecifyCode(false);
        SetEnableCopyNameAddressInfo(false);

        SetEnableNewEditAndClear(true);
        SetEnableBillingOffice(true);
    } else if (((billingTempDat.CustomerType != null) && (billingTempDat.CustomerType != ""))) {
        SetEnableSpecifyCode(false);
        SetEnableCopyNameAddressInfo(false);

        SetEnableNewEditAndClear(true);
        SetEnableBillingOffice(true);

    } else {
        SetEnableSpecifyCode(true);
        SetEnableCopyNameAddressInfo(true);

        SetEnableNewEditAndClear(true);
        SetEnableBillingOffice(true);
    } 
}

function ReloadEmailGrid() {
    $('#gridNotify').LoadDataToGrid(gridNotify, 0, false, "/Contract/CTS053_RetrieveEmailGrid", "", "dtEmailAddress", false, null, null);
}

function ReloadBillingTempGrid() {
    $('#gridBilling').LoadDataToGrid(gridBillingTemp, 0, false, "/Contract/CTS053_RetrieveBillingTempGrid", "", "CTS053_ChangePlanGrid", false, function () {
        gridBillingTemp.setSizes();
    }, null);
}

function LoadContractData() {
    call_ajax_method_json("/Contract/CTS053_RetrieveContractData", "", function (result, controls) {
        if (result != null) {
            contractDat = result.ContractData;
            changeFeeDat = result.ChangeFeeData;

            BindingContractData();
            BindingChangeFeeData();

            gridNotify = $('#gridNotify').InitialGrid(0, false, "/Contract/CTS053_InitialNotifyEmailGrid", function () {
                SpecialGridControl(gridNotify, ["Remove"]);
                BindOnLoadedEvent(gridNotify, gridNotify_Binding);
                ReloadEmailGrid();
            });

            gridBillingTemp = $('#gridBilling').InitialGrid(0, false, "/Contract/CTS053_InitialBillingTempGrid", function () {
                isBillingTempGridFreezing = false;
                SpecialGridControl(gridBillingTemp, ["Detail", "Remove"]);
                BindOnLoadedEvent(gridBillingTemp, gridBillingTemp_Binding);
                ReloadBillingTempGrid();
            });
        }
    });
}

function BindingContractData() {
    $('#ContractBasicInfo').clearForm();
    $('#ContractBasicInfo').bindJSON(contractDat);

    if (contractDat.IsImportantCustomer) {
        $('#IsImportantCustomer').attr('checked', 'checked');
    } else {
        $('#IsImportantCustomer').removeAttr('checked');
    }
}

function BindingChangeFeeData() {
    $('#ChangeContractFeeAfter').clearForm();
    $('#ChangeContractFeeAfter').bindJSON(changeFeeDat);

    //$('#ChangeFeeFlag').removeAttr('checked');
    $('#ChangeFeeFlag').prop('checked', true);
    ChangeFeeFlag_change();

    if (changeFeeDat.IsDisableDivideFlag) {
        $('#DivideBillingContractFee').attr('disabled', 'disabled');
    } else {
        $('#DivideBillingContractFee').removeAttr('disabled');
    }
}

function SetScreenToDefault() {
    contractDat = null;
    changeFeeDat = null;

    //SECOM-SIMS.INDO-BGL.Thanawit.xls #5 : Checked as default.
    $('#ChangeFeeFlag').prop('checked', true);
    
    HideBillingTargetDetail();
}

function SetEnableSpecifyCode(IsEnable) {
    if (IsEnable) {
        $('#rdoTargetCode').removeAttr('disabled');
        $('#rdoClientCode').removeAttr('disabled');
        $('#btnSearchBillingClient').removeAttr('disabled');
        $('#BillingTargetCodeSearch').removeAttr('readonly');
        $('#BillingClientCodeSearch').removeAttr('readonly');
        specifyCode_Change();
    } else {
        $('#rdoTargetCode').attr('disabled', 'disabled');
        $('#rdoClientCode').attr('disabled', 'disabled');
        $('#btnRetrieveBillingTarget').attr('disabled', 'disabled');
        $('#btnRetrieveBillingClient').attr('disabled', 'disabled');
        $('#btnSearchBillingClient').attr('disabled', 'disabled');
        $('#BillingTargetCodeSearch').attr('readonly', 'readonly');
        $('#BillingClientCodeSearch').attr('readonly', 'readonly');
    }

    if (_isp1) {
        $('#rdoTargetCode').attr('disabled', 'disabled');
        $('#rdoClientCode').attr('disabled', 'disabled');
        $('#BillingTargetCodeSearch').attr('readonly', 'readonly');

        if (IsEnable) {
            SetSpecifyCodeBillingTarget(1);
            specifyCode_Change();
        }
    }
}

function SetEnableCopyNameAddressInfo(IsEnable) {
    if (IsEnable) {
        $('#rdoContractTarget').removeAttr('disabled');
        $('#rdoBranchOfContractTarget').removeAttr('disabled');
        $('#rdoRealCustomer').removeAttr('disabled');
        $('#rdoSite').removeAttr('disabled');
        $('#btnCopyAddressInfo').removeAttr('disabled');
    } else {
        $('#rdoContractTarget').attr('disabled', 'disabled');
        $('#rdoBranchOfContractTarget').attr('disabled', 'disabled');
        $('#rdoRealCustomer').attr('disabled', 'disabled');
        $('#rdoSite').attr('disabled', 'disabled');
        $('#btnCopyAddressInfo').attr('disabled', 'disabled');
    }
}

function SetEnableBillingOffice(IsEnable) {
    if (IsEnable) {
        $('#BillingOffice').removeAttr('disabled');
    } else {
        $('#BillingOffice').attr('disabled', 'disabled');
    }
}

function SetEnableNewEditAndClear(IsEnable) {
    if (IsEnable) {
        $('#btnNewEdit').removeAttr('disabled');
        $('#btnClearBillingTarget').removeAttr('disabled');
    } else {
        $('#btnNewEdit').attr('disabled', 'disabled');
        $('#btnClearBillingTarget').attr('disabled', 'disabled');
    }
}

function SetSpecifyCodeBillingTarget(val) {
    var obj = $('input:radio[name=SpecifyCode]');
    obj.filter('[value=' + val + ']').attr('checked', true);
}

function GetSpecifyCodeBillingTarget() {
    return $('input[name="SpecifyCode"]:checked').val();
}

function SetAddressInformation(val) {
    var obj = $('input:radio[name=AddressInformation]');
    obj.filter('[value=' + val + ']').attr('checked', true);
}

function GetAddressInformation() {
    return $('input[name="AddressInformation"]:checked').val();
}

function ShowRegisterPane() {
    SetRegisterCommand(true, Register_Click);
    SetResetCommand(true, Reset_Click);
}

function HideRegisterPane() {
    SetRegisterCommand(false, null);
    SetResetCommand(false, null);
}

function ShowConfirmPane() {
    SetConfirmCommand(true, Confirm_Click);
    SetBackCommand(true, Back_Click);
}

function HideConfirmPane() {
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
}

function doAlert(moduleCode, msgCode, paramObj) {
    var obj = {
        module: moduleCode,
        code: msgCode,
        param: paramObj
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenWarningDialog(result.Message, result.Message, null);
    });
}

function doAlertWithMessage(msgCode, msgText) {
    hasAlert = true;
    alertMsg = msgText;
}

function doAskYesNo(moduleCode, msgCode, paramObj, yesFunc, noFunc) {
    var obj = {
        module: moduleCode,
        code: msgCode,
        param: paramObj
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        if ((yesFunc != null) && (typeof (yesFunc) != "function")) {
            yesFunc = null;
        }

        if ((noFunc != null) && (typeof (noFunc) != "function")) {
            noFunc = null;
        }

        OpenYesNoMessageDialog(result.Code, result.Message, yesFunc, noFunc);
    });
}

function CMS060Object() {
    return {
        "EmailList": currentMailList,
    };
}

function CMS060Response(result) {
    $("#dlgCTS053").CloseDialog();
    currentMailList = null;
    var mailLst = new Array();

    for (var i = 0; i < result.length; i++) {
        mailLst[mailLst.length] = result[i].EmailAddress;
    }

    var obj = {
        "emailAddress": mailLst,
        "fromSearch": true
    };

    call_ajax_method_json("/Contract/CTS053_AddEmail", obj, function (result) {
        if ((result != null) && result) {
            ReloadEmailGrid();
        }
    });
}

function CMS270Response(result) {
    $("#dlgCTS053").CloseDialog();
    var clientObj = {
        BillingClientCode: result.BillingClientCode,
        BillingTargetCode: "",
        BillingCodeSelected: "1",
        CanCauseError: false
    }

    call_ajax_method_json("/Contract/CTS053_RetrieveBillingClientDetailFromCode", clientObj, function (result, controls) {
        if (result != null) {
            billingTempDat.CustomerType = result.CustomerType;
            billingTempDat.BillingTargetCode = result.BillingTargetCode;
            billingTempDat.BillingClientCode = result.BillingClientCode;
            billingTempDat.BusinessTypeCode = result.BusinessTypeCode;
            billingTempDat.CompanyTypeCode = result.CompanyTypeCode;

            $('#BillingTargetDetail').bindJSON(result);
            SetEnableDisableItemOnBillingTargetDetail();
        }
    });
}

function MAS030Object() {
    return {
        "AddressEN": $('#AddressEN').val(),
        "AddressLC": $('#AddressLC').val(),
        "BranchNameEN": $('#BranchNameEN').val(),
        "BranchNameLC": $('#BranchNameLC').val(),
        "BillingClientCode": $('#BillingClientCode').val(),
        "BusinessTypeCode": billingTempDat.BusinessTypeCode,
        "CompanyTypeCode": billingTempDat.CompanyTypeCode,
        "CustTypeCode": billingTempDat.CustomerType,
        "NameEN": $('#FullNameEN').val(),
        "NameLC": $('#FullNameLC').val(),
        "PhoneNo": $('#PhoneNo').val(),
        "RegionCode": billingTempDat.RegionCode
    };
}

function MAS030Response(result) {
    $("#dlgCTS053").CloseDialog();

    $('#BillingTargetDetail').bindJSON(result);
    $('#BusinessType').val(result.BusinessTypeName);
    $('#BillingTargetCode').val('');

    billingTempDat.BillingTargetCode = null;
    billingTempDat.BillingClientCode = result.BillingClientCode;
    billingTempDat.BusinessTypeCode = result.BusinessTypeCode;
    billingTempDat.CustomerType = result.CustTypeCode;
    billingTempDat.CompanyTypeCode = result.CompanyTypeCode;
    billingTempDat.RegionCode = result.RegionCode;

    $('#BillingTargetCode').val();
    SetEnableDisableItemOnBillingTargetDetail();
}

function GetTempBillingObj(rowID) {
    if ((billingTempObj != null) && (billingTempObj != undefined)) {
        for (var i = 0; i < billingTempObj.length; i++) {
            if (billingTempObj[i].GridRowID == rowID) {
                return billingTempObj[i];
            }
        }
    }

    return null;
}

function AddTempBillingObj(obj) {
    if ((billingTempObj == null) || (billingTempObj == undefined)) {
        billingTempObj = new Array();
    }

    billingTempObj[billingTempObj.length] = obj;
}

function RemoveTempBillingObj(rowID) {
    for (var i = 0; i < billingTempObj.length; i++) {
        if (billingTempObj[i].GridRowID == rowID) {
            billingTempObj.splice(i, 1);
        }
    }
}

function ShowBillingTargetDetail() {
    //$('#BillingTargetDetail').clearForm();
    $('#BillingTargetDetail').show();
    SetAddressInformation("0");
    SetSpecifyCodeBillingTarget("0");

    specifyCode_Change();

    SetEnableDisableItemOnBillingTargetDetail();
}

function HideBillingTargetDetail() {
    $('#BillingTargetDetail').hide();
}

function LockButton (elementID, isLock) {
    if (isLock)
    {
        $('#' + elementID).attr('disabled', 'disabled');
    } else
    {
        $('#' + elementID).removeAttr('disabled');
    }
}