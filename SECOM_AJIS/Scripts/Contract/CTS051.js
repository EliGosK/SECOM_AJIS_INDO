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
var numBox_Max = 999999999999.9999;
var numBox_DefaultMin = false;

var contractDat = null;
var quotationDat = null;
var billingTempDat = null;
var isForceEnableAll = false;
var blurPress = false;

var billingTempObj = new Array();
//var deleteItemList = new Array();
//var newItemList = new Array();
//var updateItemList = new Array();

var disabledElement_ContractBasicInfo = new Array();
var disabledElement_SpecifyQuotation = new Array();
var disabledElement_ChangePlan = new Array();
var disabledElement_BillingTarget = new Array();

var validateQuotationRetrieve = ['QuotationCode', 'Alphabet'];
var validateRetrieveBillingTempDetail = ['BillingTargetCodeSearch', 'BillingClientCodeSearch'];
var validateChangePlanItem = ['ExpectedOperationDate', 'ChangePlanNormalContractFee', 'ChangePlanOrderContractFee', 'ChangePlanNormalInstallationFee', 'ChangePlanOrderInstallationFee', 'ChangePlanApproveInstallationFee', 'ChangePlanCompleteInstallationFee', 'ChangePlanStartInstallationFee', 'NegotiationStaffEmpNo1', 'NegotiationStaffEmpNo2', 'ApproveNo1', 'ContractDurationFlag', 'ApproveNo2', 'ContractDurationMonth', 'ApproveNo3', 'AutoRenewMonth', 'ApproveNo4', 'EndContractDate', 'ApproveNo5', 'ChangePlanCompleteInstallationFeeCurrencyType', 'ChangePlanStartInstallationFeeCurrencyType'];
var validateChangePlanDetail = ['BillingOffice', 'BillingContractFee', 'BillingInstallationFee', 'PaymentInstallationFee', 'BillingApproveInstallationFee', 'BillingCompleteInstallationFee', 'PaymentCompleteInstallationFee', 'BillingStartInstallationFee', 'PaymentStartInstallationFee'];
var dateTimePickerControl = new Array();
var disableFeeGrid = [  'ChangePlanNormalContractFee', 'ChangePlanNormalContractFeeCurrencyType',
                        'ChangePlanOrderContractFee', 'ChangePlanOrderContractFeeCurrencyType',
                        'ChangePlanNormalInstallationFee', 'ChangePlanNormalInstallationFeeCurrencyType',
                        'ChangePlanOrderInstallationFee', 'ChangePlanOrderInstallationFeeCurrencyType',
                        'ChangePlanApproveInstallationFee', 'ChangePlanApproveInstallationFeeCurrencyType',
                        'ChangePlanCompleteInstallationFee', 'ChangePlanCompleteInstallationFeeCurrencyType',
                        'ChangePlanStartInstallationFee', 'ChangePlanStartInstallationFeeCurrencyType'];
var disableFeeBillingDetail = [ 'BillingContractFee', 'BillingContractFeeCurrencyType',
                                'BillingInstallationFee', 'BillingInstallationFeeCurrencyType',
                                'InstallationFee', 'InstallationFeeCurrencyType',
                                'BillingApproveInstallationFee', 'BillingApproveInstallationFeeCurrencyType',
                                'BillingCompleteInstallationFee', 'BillingCompleteInstallationFeeCurrencyType',
                                'CompleteInstallationFee', 'CompleteInstallationFeeCurrencyType',
                                'BillingStartInstallationFee', 'BillingStartInstallationFeeCurrencyType',
                                'StartServiceInstallationFee', 'StartServiceInstallationFeeCurrencyType',
                                'DepositInstallationFee', 'DepositInstallationFeeCurrencyType'];
var hideFeeBillingDetail = ['trContractFee', 'trInstallFee', 'trApproval', 'trCompleteInstall', 'trStartService', 'trTotal', 'trDepositFee'];

var gridBillingTemp = null;
var btnDetailID = "btnBillingTempDetail";
var btnRemoveID = "btnBillingTempRemove";

// ---------------------------------------------------------------------------------
// Initial
// ---------------------------------------------------------------------------------
$(document).ready(function () {
    _isp1 = (_isp1.toUpperCase() == "TRUE") ? true : false;
    InitialPage();

    
    //    InitialGrid();
});

function InitialPage() {
    $('#ExpectedOperationDate').InitialDate();
    $('#EndContractDate').InitialDate();

    dateTimePickerControl[dateTimePickerControl.length] = "ExpectedOperationDate";
    dateTimePickerControl[dateTimePickerControl.length] = "EndContractDate";

    $('#ChangePlanNormalContractFee').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#ChangePlanNormalContractFee').setComma();
    $('#ChangePlanOrderContractFee').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, 16666666666.00, numBox_DefaultMin);
    $('#ChangePlanOrderContractFee').setComma();
    $('#ChangePlanNormalInstallationFee').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#ChangePlanNormalInstallationFee').setComma();
    $('#ChangePlanOrderInstallationFee').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#ChangePlanOrderInstallationFee').setComma();
    $('#ChangePlanApproveInstallationFee').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#ChangePlanApproveInstallationFee').setComma();
    $('#ChangePlanCompleteInstallationFee').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#ChangePlanCompleteInstallationFee').setComma();
    $('#ChangePlanStartInstallationFee').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#ChangePlanStartInstallationFee').setComma();
    
    $('#ContractDurationMonth').BindNumericBox(3, 0, 0, 999, 0);
    $('#ContractDurationMonth').setComma();
    $('#AutoRenewMonth').BindNumericBox(3, 0, 0, 999, 0);
    $('#AutoRenewMonth').setComma();

    $('#BillingContractFee').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, 16666666666.00, numBox_DefaultMin);
    $('#BillingContractFee').setComma();
    $('#BillingInstallationFee').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#BillingInstallationFee').setComma();
    $('#BillingApproveInstallationFee').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#BillingApproveInstallationFee').setComma();
    $('#BillingCompleteInstallationFee').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#BillingCompleteInstallationFee').setComma();
    $('#BillingStartInstallationFee').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#BillingStartInstallationFee').setComma();
    $('#AmountTotal').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#AmountTotal').setComma();
    $('#AmountTotalUsd').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#AmountTotalUsd').setComma();

    GetEmployeeNameData("#NegotiationStaffEmpNo1", "#NegotiationStaffEmpName1");
    GetEmployeeNameData("#NegotiationStaffEmpNo2", "#NegotiationStaffEmpName2");

    InitialTrimTextEvent(["ApproveNo1", "ApproveNo2", "ApproveNo3", "ApproveNo4", "ApproveNo5"]);

//    $('#Alphabet').blur(function () {
//        $('#Alphabet').val($('#Alphabet').val().toUpperCase());
//    });

    $('#btnViewInstalltionDetail').click(btnViewInstalltionDetail_Click);

    $('#btnRetrieve').click(btnRetrieve_Click);
    $('#btnSearchQuotation').click(btnSearchQuotation_Click);
    $('#btnViewQuotationDetail').click(btnViewQuotationDetail_Click);
    $('#btnSpecifyClear').click(btnSpecifyClear_Click);

    $('#ContractDurationFlag').change(ContractDurationFlag_Change);

    $('#DisplayAll').change(DisplayAll_Change);
    $('#btnNewBillingTarget').click(btnNewBillingTarget_Click);

    $('#rdoTargetCode').change(specifyCodeBillingTarget_Change);
    $('#rdoClientCode').change(specifyCodeBillingTarget_Change);

    $('#btnRetrieveBillingTarget').click(btnRetrieveBillingTarget_Click);
    $('#btnRetrieveBillingClient').click(btnRetrieveBillingClient_Click);

    $('#btnSearchBillingClient').click(btnSearchBillingClient_Click);

//    $('#rdoContractTarget').change(copyAddressInfo_Change);
//    $('#rdoBranchOfContractTarget').change(copyAddressInfo_Change);
//    $('#rdoRealCustomer').change(copyAddressInfo_Change);
//    $('#rdoSite').change(copyAddressInfo_Change);
    $('#btnCopyAddressInfo').click(btnCopyAddressInfo_Click);

    $('#btnNewEdit').click(btnNewEdit_Click);
    $('#btnClearBillingTarget').click(btnClearBillingTarget_Click);

    $('#BillingContractFee').blur(CalculateTotalFee);
    $('#BillingCompleteInstallationFee').blur(CalculateTotalFee);
    $('#BillingCompleteInstallationFee').NumericCurrency().change(CalculateTotalFee);
    $('#BillingStartInstallationFee').blur(CalculateTotalFee);
    $('#BillingStartInstallationFee').NumericCurrency().change(CalculateTotalFee);
    $('#BillingApproveInstallationFee').blur(CalculateTotalFee);
    $('#BillingApproveInstallationFee').NumericCurrency().change(CalculateTotalFee);

    $('#btnAddUpdate').click(btnAddUpdate_Click);
    $('#btnCancelBillingTargetDetail').click(btnCancelBillingTargetDetail_Click);

    SetScreenToDefault();
    LoadContractData();
}

//function InitialGrid() {
//    gridBillingTemp = $('#gridChangePlanBillingTarget').InitialGrid(0, false, "/Contract/CTS051_InitialGridChangePlanBillingTarget", function () {
//        
//    });
//}

// ---------------------------------------------------------------------------------
// Event
// ---------------------------------------------------------------------------------



function btnViewInstalltionDetail_Click() {
    var obj = {
        ContractCode: $('#ContractCode').val()
    };
    ajax_method.CallScreenControllerWithAuthority("/Common/CMS180", obj, true);
//    $("#dlgCTS051").OpenCMS180Dialog('CTS051');
}

function btnRetrieve_Click() {
    LockButton('btnRetrieve', true);
    LockButton('btnSearchQuotation', true);
    VaridateCtrl(validateQuotationRetrieve, null);

    var obj = { QuotationCode: $('#QuotationCode').val(),
        Alphabet: $('#Alphabet').val()
    };

    call_ajax_method_json("/Contract/CTS051_LoadQuotationData", obj, function (result, controls) {
        LockButton('btnRetrieve', false);
        LockButton('btnSearchQuotation', false);
        VaridateCtrl(validateQuotationRetrieve, controls);
        if (result != null) {
            $('#DivideContractFeeBillingFlag').removeAttr('checked', 'checked');
            $('#DisplayAll').removeAttr('checked', 'checked');

            quotationDat = result;
            BindingQuotationData();
            ShowChangePlan();
            ShowRegisterPane();

            ReLoadingBillingTempGrid();
        }
    });
}


function btnSearchQuotation_Click() {
    $("#dlgCTS051").OpenQUS010Dialog('CTS051');
}

function btnViewQuotationDetail_Click() {
    $("#dlgCTS051").OpenQUS012Dialog('CTS051');
}

function btnSpecifyClear_Click() {
    SetScreenToDefault();
}

function ContractDurationFlag_Change() {
    var chkStatus = $('#ContractDurationFlag').prop('checked');

    if (chkStatus) {
        $('#ContractDurationMonth').removeAttr('readonly');
        $('#AutoRenewMonth').removeAttr('readonly');
        $('#EndContractDate').EnableDatePicker(true);

        $('#ContractDurationMonth').val('');
        $('#AutoRenewMonth').val('');
        $('#EndContractDate').val('');
    } else {
        $('#ContractDurationMonth').attr('readonly', 'readonly');
        $('#AutoRenewMonth').attr('readonly', 'readonly');
        $('#EndContractDate').EnableDatePicker(false);

        // Re-Binding Data
        $('#ContractDurationMonth').val(quotationDat.ContractDurationMonth);
        $('#AutoRenewMonth').val(quotationDat.AutoRenewMonth);
        $('#EndContractDate').val(quotationDat.EndContractDate);
    }
}

function DisplayAll_Change() {
    ReLoadingBillingTempGrid();
}

function btnNewBillingTarget_Click() {
    call_ajax_method_json("/Contract/CTS051_NewBillingTempData", "", function (result, controls) {
        if (result != null) {
            billingTempDat = result;
            billingTempDat.IsNewItem = true;            
            BindingBillingTempData();
            HideRegisterPane();
            ShowBillingTargetDetail();
        }
    });
}

function specifyCodeBillingTarget_Change() {
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

function btnRetrieveBillingTarget_Click() {
    LockButton('btnRetrieveBillingTarget', true);
    LockButton('btnSearchBillingClient', true);
    VaridateCtrl(validateRetrieveBillingTempDetail, null);

    var obj = {
        BillingTargetCode: $('#BillingTargetCodeSearch').val()
    }

    call_ajax_method_json("/Contract/CTS051_RetrieveBillingTargetDetailFromCode", obj, function (result, controls) {
        LockButton('btnRetrieveBillingTarget', false);
        LockButton('btnSearchBillingClient', false);
        VaridateCtrl(validateRetrieveBillingTempDetail, controls);
        if (result != null) {
            billingTempDat.BillingTargetCode = result.BillingTargetCode;
            billingTempDat.BillingCLientCode = result.BillingClientCode;
            billingTempDat.CustomerType = result.CustomerType;
            $('#BillingTargetDetail').bindJSON(result);
            SetEnableDisableItemOnBillingTargetDetail();
        }
    });
}
function btnRetrieveBillingClient_Click() {
    LockButton('btnRetrieveBillingClient', true);
    LockButton('btnSearchBillingClient', true);
    VaridateCtrl(validateRetrieveBillingTempDetail, null);

    var obj = {
        BillingClientCode: $('#BillingClientCodeSearch').val()
    }

    call_ajax_method_json("/Contract/CTS051_RetrieveBillingClientDetailFromCode", obj, function (result, controls) {
        LockButton('btnRetrieveBillingClient', false);
        LockButton('btnSearchBillingClient', false);
        VaridateCtrl(validateRetrieveBillingTempDetail, controls);
        if (result != null) {
            billingTempDat.BillingTargetCode = result.BillingTargetCode;
            billingTempDat.BillingCLientCode = result.BillingClientCode;
            billingTempDat.CustomerType = result.CustomerType;
            $('#BillingTargetDetail').bindJSON(result);
            SetEnableDisableItemOnBillingTargetDetail();
        }
    });
}

function btnSearchBillingClient_Click() {
    $("#dlgCTS051").OpenCMS270Dialog('CTS051');
}

function btnCopyAddressInfo_Click() {
    var obj = {
        AddressCopyInfo: GetAddressInformation()
    }

    call_ajax_method_json("/Contract/CTS051_RetrieveBillingDetailFromCopy", obj, function (result, controls) {
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

function btnNewEdit_Click() {
    $("#dlgCTS051").OpenMAS030Dialog('CTS051');
//    mas030Object = null;
//    call_ajax_method_json('/Contract/GetMAS030Object_CTS051', "",
//        function (result, controls) {
//            mas030Object = result;
//            $("#dlgCTS051").OpenMAS030Dialog("CTS051");
//        }, null
//    );
}

function btnClearBillingTarget_Click() {
    $('#divViewBillingClientDetail').clearForm();
    billingTempDat.BillingTargetCode = null;
    billingTempDat.BillingClientCode = null;
    billingTempDat.CustomerType = null;
    billingTempDat.CompanyTypeCode = null;

    SetEnableDisableItemOnBillingTargetDetail();
}

function btnAddUpdate_Click() {
    VaridateCtrl(validateChangePlanDetail, null);

    var formObj = CreateObjectData($('#frmChangePlanDetail').serialize());
    formObj = ReCorrectingObjectValue_ChangePlanDetail(formObj);
    formObj.BillingOffice = $('#BillingOffice').val();
    formObj.ObjectType = billingTempDat.ObjectType;
    formObj.BillingOCC = billingTempDat.BillingOCC;
    formObj.CustomerType = billingTempDat.CustomerType;
    formObj.OldOfficeCode = billingTempDat.OldOfficeCode;
    formObj.UID = billingTempDat.UID;
    formObj.RegionCode = billingTempDat.RegionCode;
    formObj.CompanyTypeCode = billingTempDat.CompanyTypeCode;

    // Prevent lost value from disable control
    formObj.PaymentDepositFee = $('#PaymentDepositFee').val();

    formObj.OriginalBillingOCC = billingTempDat.OriginalBillingOCC;
    formObj.OriginalBillingClientCode = billingTempDat.OriginalBillingClientCode;
    formObj.OriginalBillingOfficeCode = billingTempDat.OriginalBillingOfficeCode;
    //formObj.SequenceNo = billingTempDat.SequenceNo;

    var obj = {
        targObj: formObj
    }

    call_ajax_method_json("/Contract/CTS051_ValidateBusiness_ChangePlanDetail", obj, function (result, controls) {
        VaridateCtrl(validateChangePlanDetail, controls);
        if ((result != null) && result) {
            btnCancelBillingTargetDetail_Click();
            ReLoadingBillingTempGrid();
        }
    });
}

function btnCancelBillingTargetDetail_Click() {
    billingTempDat = null;
    MaskViewToEditable();
    HideBillingTargetDetail();
    ShowRegisterPane();
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
            GenerateDetailButton(gridBillingTemp, btnDetailID, row_id, "Detail", true);
            GenerateRemoveButton(gridBillingTemp, btnRemoveID, row_id, "Remove", billingTempIDObj.CanDelete);
        }

        BindGridButtonClickEvent(btnDetailID, row_id, btnDetail_Click);
        BindGridButtonClickEvent(btnRemoveID, row_id, btnRemove_Click);
    }

    gridBillingTemp.setSizes();
}

function btnDetail_Click(rowid) {
    var currentBillingTemp = GetTempBillingObj(rowid);
    call_ajax_method_json("/Contract/CTS051_LoadBillingTempData", currentBillingTemp, function (result, controls) {
        if (result != null) {
            billingTempDat = result;
            billingTempDat.IsNewItem = false;
            BindingBillingTempData();
            HideRegisterPane();
            ShowBillingTargetDetail();
        }
    });
}

function btnRemove_Click(rowid) {
    var tmpBillingObjItem = GetTempBillingObj(rowid);
    call_ajax_method_json("/Contract/CTS051_DeleteBillingItem", tmpBillingObjItem, function (result, controls) {
        if ((result != null) && result) {
            ReLoadingBillingTempGrid();
        }
    });
}

function CalculateTotalFee() {
    if (($('#BillingApproveInstallationFee').val().length == 0) && ($('#BillingCompleteInstallationFee').val().length == 0) && ($('#BillingStartInstallationFee').val().length == 0)) {
        $('#AmountTotal').val('');
        $('#AmountTotalUsd').val('');
    } else {
        //var billingContractFee = parseFloat($('#BillingContractFee').val().replace(/ /g, "").replace(/,/g, ""));
        //var billingApproveFee = parseFloat($('#BillingApproveInstallationFee').val().replace(/ /g, "").replace(/,/g, ""));

        var rawBillingApproveInstallationFee = $('#BillingApproveInstallationFee').val().replace(/ /g, "").replace(/,/g, "");
        var rawBillingCompleteInstallFee = $('#BillingCompleteInstallationFee').val().replace(/ /g, "").replace(/,/g, "");
        var rawBillingStartServiceFee = $('#BillingStartInstallationFee').val().replace(/ /g, "").replace(/,/g, "");

        var billingCompleteInstallFee = 0;
        var billingCompleteInstallFeeUS = 0;
        var billingStartServiceFee = 0;
        var billingStartServiceFeeUS = 0;
        var billingApproveInstallationFee = 0
        var billingApproveInstallationFeeUS = 0

        if (rawBillingApproveInstallationFee.length != 0) {
            if ($('#BillingApproveInstallationFee').NumericCurrencyValue() == C_CURRENCY_LOCAL)
                billingApproveInstallationFee = parseFloat(rawBillingApproveInstallationFee);
            else
                billingApproveInstallationFeeUS = parseFloat(rawBillingApproveInstallationFee);
        }

        if (rawBillingCompleteInstallFee.length != 0) {
            if ($('#BillingCompleteInstallationFee').NumericCurrencyValue() == C_CURRENCY_LOCAL)
                billingCompleteInstallFee = parseFloat(rawBillingCompleteInstallFee);
            else
                billingCompleteInstallFeeUS = parseFloat(rawBillingCompleteInstallFee);
        }

        if (rawBillingStartServiceFee.length != 0) {
            if ($('#BillingStartInstallationFee').NumericCurrencyValue() == C_CURRENCY_LOCAL)
                billingStartServiceFee = parseFloat(rawBillingStartServiceFee);
            else
                billingStartServiceFeeUS = parseFloat(rawBillingStartServiceFee);
        }

//        var billingCompleteInstallFee = parseFloat($('#BillingCompleteInstallationFee').val().replace(/ /g, "").replace(/,/g, ""));
//        var billingStartServiceFee = parseFloat($('#BillingStartInstallationFee').val().replace(/ /g, "").replace(/,/g, ""));

        var res = 0, resUS = 0;
        
        //res = billingContractFee + billingApproveFee + billingCompleteInstallFee + billingStartServiceFee;
        res = billingCompleteInstallFee + billingStartServiceFee + billingApproveInstallationFee;
        res = res.toFixed(2);
        $('#AmountTotal').val(res);
        $('#AmountTotal').setComma();

        resUS = billingCompleteInstallFeeUS + billingStartServiceFeeUS + billingApproveInstallationFeeUS;
        resUS = resUS.toFixed(2);
        $('#AmountTotalUsd').val(resUS);
        $('#AmountTotalUsd').setComma();
    }
}

function Register_Click() {
    VaridateCtrl(validateChangePlanItem, null);
    DisableRegisterCommand(true);
    DisableResetCommand(true);

    var obj = CreateObjectData($('#frmChangePlan').serialize());
    obj = ReCorrectingObjectValue(obj);
    obj.QuotationCode = quotationDat.QuotationCode;
    obj.Alphabet = quotationDat.Alphabet;
    obj.DivideContractFeeBillingFlag = $('#DivideContractFeeBillingFlag').is(':checked');
    obj.HasChangePlanDetailTask = (billingTempDat != null);
    call_ajax_method_json("/Contract/CTS051_ValidateBusiness_All", obj, function (result, controls) {
        VaridateCtrl(validateChangePlanItem, controls);
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
    VaridateCtrl(validateChangePlanItem, null);
    DisableRegisterCommand(true);
    DisableResetCommand(true);
    doAskYesNo("Common", "MSG0038", null, function ()
    {
        SetScreenToDefault();
        LoadContractData();
        DisableRegisterCommand(false);
        DisableResetCommand(false);
    },
    function ()
    {
        DisableRegisterCommand(false);
        DisableResetCommand(false);
    });
}

function Confirm_Click() {
    command_control.CommandControlMode(false);

    var obj = CreateObjectData($('#frmChangePlan').serialize());
    obj = ReCorrectingObjectValue(obj);
    obj.QuotationCode = quotationDat.QuotationCode;
    obj.Alphabet = quotationDat.Alphabet;
    obj.DivideContractFeeBillingFlag = $('#DivideContractFeeBillingFlag').is(':checked');
    obj.HasChangePlanDetailTask = (billingTempDat != null);

    call_ajax_method_json("/Contract/CTS051_ConfirmRegister", obj, function (result, controls) {
        if ((result != null) && result) {
            OpenInformationMessageDialog(result.Code, result.Message, function () {
                window.location.href = generate_url("/Common/CMS020");
            });
        }
    });
}

function Back_Click() {
    DisableConfirmCommand(true);
    SetScreenToViewMode(false);
    HideConfirmPane();
    ShowRegisterPane();
    DisableConfirmCommand(false);
}

// ---------------------------------------------------------------------------------
// Method
// ---------------------------------------------------------------------------------
function ReCorrectingObjectValue_ChangePlanDetail(obj) {
    obj.BillingContractFee = obj.BillingContractFee.replace(/ /g, "").replace(/,/g, "");
    obj.BillingContractFeeCurrencyType = $('#BillingContractFee').NumericCurrencyValue();
    obj.BillingInstallationFee = obj.BillingInstallationFee.replace(/ /g, "").replace(/,/g, "");
    obj.BillingInstallationFeeCurrencyType = $('#BillingInstallationFee').NumericCurrencyValue();
    obj.BillingApproveInstallationFee = obj.BillingApproveInstallationFee.replace(/ /g, "").replace(/,/g, "");
    obj.BillingApproveInstallationFeeCurrencyType = $('#BillingApproveInstallationFee').NumericCurrencyValue();
    obj.BillingCompleteInstallationFee = obj.BillingCompleteInstallationFee.replace(/ /g, "").replace(/,/g, "");
    obj.BillingCompleteInstallationFeeCurrencyType = $('#BillingCompleteInstallationFee').NumericCurrencyValue();
    obj.BillingStartInstallationFee = obj.BillingStartInstallationFee.replace(/ /g, "").replace(/,/g, "");
    obj.BillingStartInstallationFeeCurrencyType = $('#BillingStartInstallationFee').NumericCurrencyValue();
    obj.AmountTotal = obj.AmountTotal.replace(/ /g, "").replace(/,/g, "");
    obj.AmountTotalUsd = obj.AmountTotalUsd.replace(/ /g, "").replace(/,/g, "");

    return obj;
}

function ReCorrectingObjectValue(obj) {
    
    obj.ChangePlanNormalContractFee = obj.ChangePlanNormalContractFee.replace(/ /g, "").replace(/,/g, "");
    obj.ChangePlanNormalContractFeeCurrencyType = $('#ChangePlanNormalContractFee').NumericCurrencyValue();
    obj.ChangePlanOrderContractFee = obj.ChangePlanOrderContractFee.replace(/ /g, "").replace(/,/g, "");
    obj.ChangePlanOrderContractFeeCurrencyType = $('#ChangePlanOrderContractFee').NumericCurrencyValue();
    obj.ChangePlanNormalInstallationFee = obj.ChangePlanNormalInstallationFee.replace(/ /g, "").replace(/,/g, "");
    obj.ChangePlanNormalInstallationFeeCurrencyType = $('#ChangePlanNormalInstallationFee').NumericCurrencyValue();
    obj.ChangePlanOrderInstallationFee = obj.ChangePlanOrderInstallationFee.replace(/ /g, "").replace(/,/g, "");
    obj.ChangePlanOrderInstallationFeeFeeCurrencyType = $('#ChangePlanOrderInstallationFee').NumericCurrencyValue();
    obj.ChangePlanApproveInstallationFee = obj.ChangePlanApproveInstallationFee.replace(/ /g, "").replace(/,/g, "");
    obj.ChangePlanApproveInstallationFeeCurrencyType = $('#ChangePlanApproveInstallationFee').NumericCurrencyValue();
    obj.ChangePlanCompleteInstallationFee = obj.ChangePlanCompleteInstallationFee.replace(/ /g, "").replace(/,/g, "");
    obj.ChangePlanCompleteInstallationFeeCurrencyType = $('#ChangePlanCompleteInstallationFee').NumericCurrencyValue();
    obj.ChangePlanStartInstallationFee = obj.ChangePlanStartInstallationFee.replace(/ /g, "").replace(/,/g, "");
    obj.ChangePlanStartInstallationFeeCurrencyType = $('#ChangePlanStartInstallationFee').NumericCurrencyValue();

    if (obj.ContractDurationFlag == 'on')
    {
        obj.ContractDurationFlag = true;
    } else
    {
        obj.ContractDurationFlag = false;
    }

    return obj;
}

function SetEnableDisableItemOnBillingTargetDetail() {
    // Have BillingOCC

    if (((billingTempDat.BillingOCC == null) || (billingTempDat.BillingOCC == ""))
    && ((billingTempDat.BillingTargetCode == null) || (billingTempDat.BillingTargetCode == ""))
    && ((billingTempDat.BillingClientCode == null) || (billingTempDat.BillingClientCode == ""))
    && ((billingTempDat.CustomerType == null) || (billingTempDat.CustomerType == ""))
    && billingTempDat.IsNewItem)
    {
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

function SetScreenToViewMode(IsViewMode) {
    EnableBillingGrid(!IsViewMode);
    $('#ContractBasicInfo').SetViewMode(IsViewMode);
    $('#SpecifyQuotaion').SetViewMode(IsViewMode);
    $('#ChangePlan').SetViewMode(IsViewMode);
    $('#BillingTarget').SetViewMode(IsViewMode);
    $('#BillingTargetDetail').SetViewMode(IsViewMode);

    if (IsViewMode) {
        $('#divQuotationCode').html($('#QuotationCode').val() + '-' + $('#Alphabet').val());
        $('#divAlphabet').hide();

        if ($('#NegotiationStaffEmpNo1').val().length == 0) {
            $('#divNegotiationStaffEmpName1').hide();
        }

        if ($('#NegotiationStaffEmpNo2').val().length == 0) {
            $('#divNegotiationStaffEmpName2').hide();
        }
    }

    if (isGridFinInit) {
        gridBillingTemp.setSizes();
    }
}

function ReLoadingBillingTempGrid() {
    var obj = {
        IsDisplayAll: $('#DisplayAll').prop('checked')
        //deleteList: deleteItemList
    }

    isGridFinInit = false;

    gridBillingTemp = $('#gridChangePlanBillingTarget').InitialGrid(0, false, "/Contract/CTS051_InitGridChangePlanBillingTarget", function () {
        SpecialGridControl(gridBillingTemp, ["Detail", "Remove"]);
        BindOnLoadedEvent(gridBillingTemp, gridBillingTemp_Binding);
        isGridFinInit = true;
        $('#gridChangePlanBillingTarget').LoadDataToGrid(gridBillingTemp, 0, false, "/Contract/CTS051_LoadGridChangePlanBillingTarget", obj, "CTS051_ChangePlanGrid", false, null, null);
    });
//    gridBillingTemp = $('#gridChangePlanBillingTarget').LoadDataToGridWithInitial(0, false, false, "/Contract/CTS051LoadGridChangePlanBillingTarget", obj, "CTS051_ChangePlanGrid", false, false, function (result) {
//        BindingContractData();
//    })
}

function LoadContractData() {
    call_ajax_method_json("/Contract/CTS051_LoadContractData", "", function (result, controls) {
        if (result != null) {
            contractDat = result;
            BindingContractData();
        }
    });
}

function BindingContractData() {
    $('#ContractBasicInfo').bindJSON(contractDat);
    if (contractDat.IsImportantCustomer) {
        $('#IsImportantCustomer').attr('checked', 'checked');
    } else {
        $('#IsImportantCustomer').removeAttr('checked');
    }

    if (!contractDat.CanViewInstallStatus) {
        $('#btnViewInstalltionDetail').attr('disabled', 'disabled');
    } else {
        $('#btnViewInstalltionDetail').removeAttr('disabled');
    }

    $('#QuotationCode').val(contractDat.QuotationCode);
}

function BindingQuotationData() {
    $('#Alphabet').val(quotationDat.Alphabet);
    $('#ChangePlan').bindJSON(quotationDat);

    for (var i = 0; i < disableFeeGrid.length; i++) {
        if ($('#' + disableFeeGrid[i]).attr('type') == 'text') {
            $('#' + disableFeeGrid[i]).removeAttr('readonly');
        } else {
            $('#' + disableFeeGrid[i]).removeAttr('disabled');
        }
    }

    if (quotationDat.DisableList != null) {
        for (var i = 0; i < quotationDat.DisableList.length; i++) {
            if ($('#' + quotationDat.DisableList[i]).attr('type') == 'text') {
                $('#' + quotationDat.DisableList[i]).attr('readonly', 'readonly');
            } else {
                $('#' + quotationDat.DisableList[i]).attr('disabled', 'disabled');
            }
        }
    }

    $('#ContractDurationFlag').removeAttr('checked');
    ContractDurationFlag_Change();

    
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

        $('#BillingContractFee').val((billingTempDat.BillingContractFee == null) ? "" : parseFloat($('#BillingContractFee').val().replace(/ /g, "").replace(/,/g, "")).toFixed(2));
        $('#BillingApproveInstallationFee').val((billingTempDat.BillingApproveInstallationFee == null) ? "" : parseFloat($('#BillingApproveInstallationFee').val().replace(/ /g, "").replace(/,/g, "")).toFixed(2));
        $('#BillingCompleteInstallationFee').val((billingTempDat.BillingCompleteInstallationFee == null) ? "" : parseFloat($('#BillingCompleteInstallationFee').val().replace(/ /g, "").replace(/,/g, "")).toFixed(2));
        $('#BillingStartInstallationFee').val((billingTempDat.BillingStartInstallationFee == null) ? "" : parseFloat($('#BillingStartInstallationFee').val().replace(/ /g, "").replace(/,/g, "")).toFixed(2));
        $('#BillingInstallationFee').val((billingTempDat.BillingInstallationFee == null) ? "" : parseFloat($('#BillingInstallationFee').val().replace(/ /g, "").replace(/,/g, "")).toFixed(2));
        
        $('#BillingContractFee').setComma();
        $('#BillingApproveInstallationFee').setComma();
        $('#BillingCompleteInstallationFee').setComma();
        $('#BillingStartInstallationFee').setComma();
        $('#BillingInstallationFee').setComma();
    }

    // Loop for set to default
    for (var i = 0; i < hideFeeBillingDetail.length; i++) {
        $('#' + hideFeeBillingDetail[i]).show();
    }

    // Loop for hide
    for (var i = 0; i < billingTempDat.HideList.length; i++) {
        $('#' + billingTempDat.HideList[i]).hide();
    }

    // Loop for set to default
    for (var i = 0; i < disableFeeBillingDetail.length; i++) {
        if ($('#' + disableFeeBillingDetail[i]).attr('type') == 'text') {
            $('#' + disableFeeBillingDetail[i]).removeAttr('readonly');
        } else {
            $('#' + disableFeeBillingDetail[i]).removeAttr('disabled');
        }
    }

    // Loop for disable
    for (var i = 0; i < billingTempDat.DisableList.length; i++) {
        if ($('#' + billingTempDat.DisableList[i]).attr('type') == 'text') {
            $('#' + billingTempDat.DisableList[i]).attr('readonly', 'readonly');
        } else {
            $('#' + billingTempDat.DisableList[i]).attr('disabled', 'disabled');
        }
    }

    CalculateTotalFee();
    MaskViewToReadonly();
}

function MaskViewToReadonly() {
//    disabledElement_ContractBasicInfo = SetDisableView($('#ContractBasicInfo'), false, disabledElement_ContractBasicInfo);
//    disabledElement_SpecifyQuotation = SetDisableView($('#SpecifyQuotaion'), false, disabledElement_SpecifyQuotation);
//    disabledElement_ChangePlan = SetDisableView($('#ChangePlan'), false, disabledElement_ChangePlan);
//    disabledElement_BillingTarget = SetDisableView($('#BillingTarget'), false, disabledElement_BillingTarget);
    //    EnableBillingGrid(false);

    FreezeBillingGrid(true);
}

function MaskViewToEditable() {
//    disabledElement_ContractBasicInfo = SetDisableView($('#ContractBasicInfo'), true, disabledElement_ContractBasicInfo);
//    disabledElement_SpecifyQuotation = SetDisableView($('#SpecifyQuotaion'), true, disabledElement_SpecifyQuotation);
//    disabledElement_ChangePlan = SetDisableView($('#ChangePlan'), true, disabledElement_ChangePlan);
//    disabledElement_BillingTarget = SetDisableView($('#BillingTarget'), true, disabledElement_BillingTarget);
    //    EnableBillingGrid(true);

    FreezeBillingGrid(false);
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

        EnableGridButton(gridBillingTemp, btnDetailID, row_id, "Detail", !IsFreezing);
        EnableGridButton(gridBillingTemp, btnRemoveID, row_id, "Remove", (!IsFreezing && canDeleteThisRow));
    }
}

function EnableBillingGrid(isEnable) {
    if ((gridBillingTemp != null) && isGridFinInit) {
        if (isEnable) {
            ReLoadingBillingTempGrid();
        } else {
            var _colRemove = gridBillingTemp.getColIndexById("Remove");
            var _colDetail = gridBillingTemp.getColIndexById("Detail");

            gridBillingTemp.setColumnHidden(_colRemove, !isEnable);
            gridBillingTemp.setColumnHidden(_colDetail, !isEnable);

            SetFitColumnForBackAction(gridBillingTemp, "TmpColumn");

            gridBillingTemp.setSizes();
        }
    }
}

function SetScreenToDefault() {
    $('#Alphabet').val('');
    contractDat = null;
    quotationDat = null;
    billingTempDat = null;

    billingTempObj = new Array();

    HideChangePlan();
    HideBillingTargetDetail();
    HideRegisterPane();
    HideConfirmPane();
}

function ShowChangePlan() {
    $('#ChangePlan').show();
    $('#BillingTarget').show();

    $('#Alphabet').attr('readonly', 'readonly');
    $('#btnViewQuotationDetail').removeAttr('disabled');
    $('#btnSpecifyClear').removeAttr('disabled');

    $('#btnRetrieve').attr('disabled', 'disabled');
    $('#btnSearchQuotation').attr('disabled', 'disabled');
}

function HideChangePlan() {
    $('#ChangePlan').hide();
    $('#BillingTarget').hide();

    $('#Alphabet').removeAttr('readonly');
    $('#btnViewQuotationDetail').attr('disabled', 'disabled');
    $('#btnSpecifyClear').attr('disabled', 'disabled');

    $('#btnRetrieve').removeAttr('disabled');
    $('#btnSearchQuotation').removeAttr('disabled');
}

function ShowBillingTargetDetail() {
    

    $('#BillingTargetDetail').show();
    SetAddressInformation("0");
    SetSpecifyCodeBillingTarget("0");

    specifyCodeBillingTarget_Change();

    SetEnableDisableItemOnBillingTargetDetail();
}

function HideBillingTargetDetail() {
    $('#BillingTargetDetail').hide();
}

function SetEnableSpecifyCode(IsEnable)
{
    if (IsEnable) {
        $('#rdoTargetCode').removeAttr('disabled');
        $('#rdoClientCode').removeAttr('disabled');
        $('#btnSearchBillingClient').removeAttr('disabled');
        $('#BillingTargetCodeSearch').removeAttr('readonly');
        $('#BillingClientCodeSearch').removeAttr('readonly');
        specifyCodeBillingTarget_Change();
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
            specifyCodeBillingTarget_Change();
        }
    }
}

function SetEnableCopyNameAddressInfo(IsEnable)
{
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

function SetEnableBillingOffice(IsEnable)
{
    if (IsEnable)
    {
        $('#BillingOffice').removeAttr('disabled');
    } else
    {
        $('#BillingOffice').attr('disabled', 'disabled');
    }
}

function SetEnableNewEditAndClear(IsEnable)
{
    if (IsEnable)
    {
        $('#btnNewEdit').removeAttr('disabled');
        $('#btnClearBillingTarget').removeAttr('disabled');
    } else
    {
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

function QUS010Object() {
    return {
        strCallerScreenID: "CTS051"
        , ViewMode: '2'
        , strServiceTypeCode: _rentalServiceType
        , strTargetCodeTypeCode: _targetCodeType
        , strQuotationTargetCode: $("#QuotationCode").val()
    };
}

function QUS012Object() {
    return {
        "QuotationTargetCode": $("#QuotationCode").val(),
        "Alphabet": $("#Alphabet").val(),
        "HideQuotationTarget": true
    };
}

//function CMS180Object() {
//    return {
//        "ContractCode": $("#ContractCode").val()
//    };
//}

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

function QUS010Response(result) {
    $("#Alphabet").val(result.Alphabet);
    $("#dlgCTS051").CloseDialog();

    btnRetrieve_Click();
}

function CMS270Response(result) {
    $("#dlgCTS051").CloseDialog();
    var clientObj = {
        BillingClientCode: result.BillingClientCode,
        CanCauseError: false
    }

    call_ajax_method_json("/Contract/CTS051_RetrieveBillingClientDetailFromCode", clientObj, function (result, controls) {
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

function MAS030Response(result) {
    $("#dlgCTS051").CloseDialog();

    $('#BillingTargetDetail').bindJSON(result);
    $('#BusinessType').val(result.BusinessTypeName);

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

function SetDisableView(elementObject, isEnable, disableArrayList) {
    if (!isEnable) {
        disableArrayList = new Array();
        elementObject.find("input[type=text],input[type=checkbox],textarea,button,select,input[type=radio]").each(function () {
            var tag = this.tagName.toLowerCase();
            var isEditable = false;

            if ((tag == "input" && $(this).attr("type") == "text")
                || tag == "textarea") {
                isEditable = !$(this).prop('readonly');
            } else {
                isEditable = !$(this).prop('disabled');
            }

            if (isEditable) {
                if (jQuery.inArray($(this).attr('id'), dateTimePickerControl) >= 0) {
                    $(this).EnableDatePicker(false);
                } else {
                    if ((tag == "input" && $(this).attr("type") == "text")
                || tag == "textarea") {
                        $(this).attr('readonly', 'readonly');
                    } else {
                        $(this).attr('disabled', 'disabled');
                    }
                }

                disableArrayList[disableArrayList.length] = $(this).attr('id');
            }
        });
    } else {
        for (var i = 0; i < disableArrayList.length; i++) {
            if (jQuery.inArray(disableArrayList[i], dateTimePickerControl) >= 0) {
                $('#' + disableArrayList[i]).EnableDatePicker(true);
            } else {
                if (($('#' + disableArrayList[i]).attr('type') == 'text') || ($('#' + disableArrayList[i]).get(0).tagName.toLowerCase() == 'textarea')) {
                    $('#' + disableArrayList[i]).removeAttr('readonly');
                } else {
                    $('#' + disableArrayList[i]).removeAttr('disabled');
                }
            }
        }

        disableArrayList = new Array();
    }

    return disableArrayList;
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

function LockButton(elementID, isLock) {
    if (isLock) {
        $('#' + elementID).attr('disabled', 'disabled');
    } else {
        $('#' + elementID).removeAttr('disabled');
    }
}