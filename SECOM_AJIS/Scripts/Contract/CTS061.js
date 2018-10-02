/*--- Main ---*/

var mygridBillingDetailCTS061;
var pageRow = 20;
var mode;
var eventCopyNameComeFrom;

$(document).ready(function () {
    if (ObjCTS061.CanOperate.toUpperCase() != "FALSE") {
        InitialControlProperty();
        EnableRegisterCommand();
        $("#SaleContractChangeSection").SetViewMode(false);
        $('#ChangeExpectedSection').SetViewMode(false);
        $("#ImportantFlag").attr("disabled", true);
    }
});

function InitialControlProperty() {
    
    $("#ExpectedInstallCompleteDate").InitialDate();
    if (ObjCTS061.InstallationStatusCode == "99") {
        $("#btnViewInstalltionDetail").attr("disabled", true);
    }

    $('#btnViewInstalltionDetail').click(function () {
        var obj = {
            ContractCode: $('#ContractCode').val()
        };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS180", obj, true);
    });
}

//Event------------------------------------------------

function BtnRegisterClick() {
    VaridateCtrl(['ExpectedInstallCompleteDate'], null);
    DisableRegisterCommand(true);
    DisableResetCommand(true);
    var obj = { ExpectedInstallCompleteDate: $("#ExpectedInstallCompleteDate").val() };
    call_ajax_method_json('/Contract/RegisterClick_CTS061', obj,
            function (result, controls) {
            VaridateCtrl(['ExpectedInstallCompleteDate'], controls);
                if (result == undefined) {
                    EnableConfirmCommand();
                    $("#SaleContractChangeSection").SetViewMode(true);
                    $('#ChangeExpectedSection').SetViewMode(true);
                }

                DisableRegisterCommand(false);
                DisableResetCommand(false);
            });
}

function BtnConfirmClick() {
    DisableConfirmCommand(true);
    var obj = { ExpectedInstallCompleteDate: $("#ExpectedInstallCompleteDate").val() };
    call_ajax_method_json('/Contract/ConfirmClick_CTS061', obj, function (result, controls) {
        if (result.Code == 'MSG0046') {
            OpenInformationMessageDialog(result.Code, result.Message, function () {
                window.location.href = generate_url("/common/CMS020");
            }, null);
        }
    });       
}

function BtnBackClick() {
    DisableConfirmCommand(true);
    EnableRegisterCommand();
    $("#SaleContractChangeSection").SetViewMode(false);
    $('#ChangeExpectedSection').SetViewMode(false);
    DisableConfirmCommand(false);
}

function BtnResetClick() {
    DisableRegisterCommand(true);
    DisableResetCommand(true);
    var obj = {
        module: "Common",
        code: "MSG0038"
    };
    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenOkCancelDialog(result.Code, result.Message,
        function () {
            call_ajax_method_json('/Contract/ResetClick_CTS061', ObjCTS061.LastOCC, function (result, controls) {
                if (result != null) {
                    ReBindingData(result);
                    EnableRegisterCommand();
                }
            });
        },
        null);
        DisableRegisterCommand(false);
        DisableResetCommand(false);
    });
}

function ReBindingData(dataObj) {
    ObjCTS061 = { LastOCC: dataObj.LastOCC,
        ExpectedInstallCompleteDate: dataObj.ExpectedInstallCompleteDate,
        InstallationStatusCode: dataObj.InstallationStatusCode,
        CanOperate: dataObj.CanOperate
    }

    if (dataObj.ImportantFlag) {
        $('#ImportantFlag').attr('checked', 'checked');
    } else {
        $('#ImportantFlag').removeAttr('checked');
    }
    $('#ContractCode').val(dataObj.ContractCodeShort);
    $('#PurchaserCustCode').val(dataObj.PurchaserCustCode);
    $('#RealCustomerCustCode').val(dataObj.RealCustomerCustCode);
    $('#SiteCode').val(dataObj.SiteCode);
    $('#PurchaserNameEN').val(dataObj.PurchaserNameEN);
    $('#PurchaserAddressEnglish').val(dataObj.PurchaserAddressEN);
    $('#SiteNameEN').val(dataObj.SiteNameEN);
    $('#SiteAddressEN').val(dataObj.SiteAddressEN);
    $('#CustFullNameLC').val(dataObj.PurchaserNameLC);
    $('#PurchaserAddressLocal').val(dataObj.PurchaserAddressLC);
    $('#SiteNameLC').val(dataObj.SiteNameLC);
    $('#SiteAddressLC').val(dataObj.SiteAddressLC);
    $('#InstallationStatusName').val(dataObj.InstallationStatusCodeName);
    $('#OperationOffice').val(dataObj.OperationOfficeName);
    $('#ExpectedInstallCompleteDate').val(dataObj.ExpectedInstallCompleteDate);
}

function MaintainScreenItemOnInit() {        
//    SetRegisterCommand(true, BtnRegisterClick);
//    SetResetCommand(true, BtnResetClick);
//    SetConfirmCommand(false, BtnConfirmClick);
//    SetBackCommand(false, BtnConfirmClick);
}

function EnableRegisterCommand() {
    DisableAllCommand();
    SetRegisterCommand(true, BtnRegisterClick);
    SetResetCommand(true, BtnResetClick);
}

function EnableConfirmCommand() {
    DisableAllCommand();
    SetConfirmCommand(true, BtnConfirmClick);
    SetBackCommand(true, BtnBackClick);
}

function DisableAllCommand() {
    SetRegisterCommand(false, null);
    SetResetCommand(false, null);
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
}