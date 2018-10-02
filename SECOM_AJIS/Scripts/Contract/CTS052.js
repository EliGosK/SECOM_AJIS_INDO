/*--- Main ---*/

var mygridBillingDetailCTS052;
var pageRow = 20;
var mode;
var eventCopyNameComeFrom;
var validateRegisForm = ["Alphabet"
, "ChangeImplementDate"
, "NegotiationStaffEmpNo1"
, "ApproveNo1"];

$(document).ready(function () {
    ObjCTS052.Mode = 'Search';
    InitialControlProperty();
    MaintainScreenItemOnInit();
    ISHideModifyInstrumentSection(true);
//    $('#Alphabet').blur(function () {
//        $('#Alphabet').val($('#Alphabet').val().toUpperCase());
//    });
});

function InitialControlProperty() {
    $("#ChangeImplementDate").InitialDate();

    $("#ContractFee").attr("readonly", true);
    $("#ContractFee").NumericCurrency().attr("disabled", true);
    $("#CurrentContractFee").attr("readonly", true);
    $("#CurrentContractFee").NumericCurrency().attr("disabled", true);

    $("#QuotationTargetCode").attr("readonly", true);
    $("#NegotiationStaffName1").attr("readonly", true);

    $("#ContractFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#CurrentContractFee").BindNumericBox(12, 2, 0, 999999999999.99);

    $("#btnRetrieve").click(function () {
        BtnRetrieveClick();
    });

    $("#btnSearchQuotation").click(function () {
        $("#dlgCTS052").OpenQUS010Dialog('CTS052');
    });

    $("#NegotiationStaffEmpName1").attr("readonly", true);
    $('#NegotiationStaffEmpNo1').blur(
    function () {
        ObjCTS052.EmpNo = $("#NegotiationStaffEmpNo1").val();
        call_ajax_method_json('/Contract/GetActiveEmployee_CTS052', ObjCTS052,
        function (result, controls) {
            if (result != undefined) {
                if (result.Message != undefined && $("#NegotiationStaffEmpNo1").val() != "") {
                    OpenErrorMessageDialog(result.Code, result.Message, function () { }, null);
                    $("#NegotiationStaffEmpName1").val("");
                }
                else {
                    $("#NegotiationStaffEmpName1").val(result.EmpName);
                }
            }
        });
    });

    $("#NegotiationStaffEmpName2").attr("readonly", true);
    $('#NegotiationStaffEmpNo2').blur(
    function () {
        ObjCTS052.EmpNo = $("#NegotiationStaffEmpNo2").val();
        call_ajax_method_json('/Contract/GetActiveEmployee_CTS052', ObjCTS052,
        function (result, controls) {
            if (result != undefined) {
                if (result.Message != undefined && $("#NegotiationStaffEmpNo2").val() != "") {
                    OpenErrorMessageDialog(result.Code, result.Message, function () { }, null);
                    $("#NegotiationStaffEmpName2").val("");
                }
                else {
                    $("#NegotiationStaffEmpName2").val(result.EmpName);
                }
            }
        });
    });

    $("#btnSpecifyClear").click(function () {
        BtnClearClick();
    });

    
    if (ObjCTS052.InstallationStatusCode == "99") {
        $("#btnViewInstalltionDetail").attr("disabled", true);
    }

    InitialTrimTextEvent(["ApproveNo1", "ApproveNo2", "ApproveNo3", "ApproveNo4", "ApproveNo5"]);
}

//Event------------------------------------------------

function BtnRetrieveClick() {
    VaridateCtrl(["Alphabet"], null);
    call_ajax_method_json('/Contract/ValidateRetrieve_CTS052', GetValidateObject(),
    function (result, controls) {
        VaridateCtrl(["Alphabet"], controls);
        if (result == undefined) {
            call_ajax_method_json('/Contract/RetrieveClick_CTS052', GetValidateObject(),
            function (result, controls) {
                VaridateCtrl(["Alphabet"], controls);
                if ((result != null) && (result.Code == null)) {
                    //                    $("#ChangeImplementDate").datepicker("getDate");
                    //                    result.ChangeImplementDate = ConvertDateObject(result.ChangeImplementDate);
                    //                    $("#ChangeImplementDate").val(ConvertDateToTextFormat(result.ChangeImplementDate));
                    $('#Alphabet').val(result.Alphabet);

                    $("#ContractFee").SetNumericCurrency(result.ContractFeeCurrencyType);
                    $("#ContractFee").val(result.ContractFee);

                    $("#CurrentContractFee").SetNumericCurrency(result.OrderContractFeeCurrencyType);
                    $("#CurrentContractFee").val(result.OrderContractFee);

                    //                    $("#NegotiationStaffEmpNo1").val(result.NegotiationStaffEmpNo1);
                    //                    $("#NegotiationStaffEmpName1").val(result.NegotiationStaffEmpName1);
                    //                    $("#NegotiationStaffEmpNo2").val(result.NegotiationStaffEmpNo2);
                    //                    $("#NegotiationStaffEmpName2").val(result.NegotiationStaffEmpName2);

                    $("#ApproveNo1").val(result.ApproveNo1);
                    $("#ApproveNo2").val(result.ApproveNo2);

                    $("#btnViewQuotationDetail").attr("disabled", false);
                    //                    $("#btnViewQuotationDetail").initial_link(function (val) {
                    //                        var parameter = {
                    //                            QuotationTargetCode: $("#QuotationTargetCode").val(),
                    //                            Alphabet: $("#Alphabet").val(),
                    //                            HideQuotationTarget: true
                    //                        };

                    //                        $("#dlgCTS052").OpenQUS012Dialog();
                    //                    });

                    $("#btnViewQuotationDetail").unbind('click');
                    $("#btnViewQuotationDetail").click(function (val) {
                        var parameter = {
                            QuotationTargetCode: $("#QuotationTargetCode").val(),
                            Alphabet: $("#Alphabet").val(),
                            HideQuotationTarget: true
                        };

                        $("#dlgCTS052").OpenQUS012Dialog();
                    });

                    ISDisableSpecifyQuotation(true);
                    ISHideModifyInstrumentSection(false);
                    $('#btnSpecifyClear').attr('disabled', false);
                    EnableRegisterCommand();
                }
                else if (result != undefined) {
                    if (result.Code == "MSG3038") {
                        call_ajax_method_json('/Contract/GetQuotationInformationData_CTS052', "",
                        function (result, controls) {
                            $('#Alphabet').val(result.Alphabet);

                            //$("#ContractFee").val(result.ContractFee);
                            //$("#CurrentContractFee").val(result.OrderContractFee);
                            $("#ContractFee").SetNumericCurrency(result.ContractFeeCurrencyType);
                            $("#ContractFee").val(result.ContractFee);

                            $("#CurrentContractFee").SetNumericCurrency(result.OrderContractFeeCurrencyType);
                            $("#CurrentContractFee").val(result.OrderContractFee);

                            $("#ApproveNo1").val(result.ApproveNo1);
                            $("#ApproveNo2").val(result.ApproveNo2);

                            $("#btnViewQuotationDetail").attr("disabled", false);
                            //                            $("#btnViewQuotationDetail").initial_link(function (val) {
                            //                                var parameter = {
                            //                                    QuotationTargetCode: $("#QuotationTargetCode").val(),
                            //                                    Alphabet: $("#Alphabet").val(),
                            //                                    HideQuotationTarget: true
                            //                                };

                            //                                $("#dlgCTS052").OpenQUS012Dialog();
                            //                            });

                            $("#btnViewQuotationDetail").unbind('click');
                            $("#btnViewQuotationDetail").click(function (val) {
                                var parameter = {
                                    QuotationTargetCode: $("#QuotationTargetCode").val(),
                                    Alphabet: $("#Alphabet").val(),
                                    HideQuotationTarget: true
                                };

                                $("#dlgCTS052").OpenQUS012Dialog();
                            });

                            EnableRegisterCommand();
                            ISDisableSpecifyQuotation(true);
                            ISHideModifyInstrumentSection(false);
                            $('#btnSpecifyClear').attr('disabled', false);
                        });
                    }
                }
            });
        }
    });    
}

function BtnCancelClick() {
    ClearBillingDetail();
    ClearBillingDetailGrid();    
    ISDisableBillingTargetDetailSection(true);
    ISDisableBillingTargetSection(false);
    ISDisableRegister(false);
    GetBillingTargetInformation();
}

function BtnClearClick() {
    $("#ModifyInstrument :input").val("");
    $("#Alphabet").val("");
    ISHideModifyInstrumentSection(true);
    ISDisableSpecifyQuotation(false);
    DisableAllCommand();
    $("#btnViewQuotationDetail").attr("disabled",true);
    $('#btnSpecifyClear').attr('disabled', true);
}

function NewClick() {
    mode = "New";
    MaintainScreenItemOnInit();
    call_ajax_method_json('/Contract/NewClick_CTS053', "", function (result, controls) { }, null);
    ClearBillingDetail();
    ClearBillingDetailGrid();
    ISDisableBillingTargetDetailSection(false);
    ISDisableNewRecordSection(false);
}

function BtnRegisterClick() {
    DisableRegisterCommand(true);
    DisableResetCommand(true);
    VaridateCtrl(validateRegisForm, null);
    call_ajax_method_json('/Contract/RegisterClick_CTS052', GetValidateObject(),
        function (result, controls) {
            VaridateCtrl(validateRegisForm, controls);
            if (result == undefined) {
                EnableConfirmCommand();

                $("#ContractChange").SetViewMode(true);
                $("#ModifyInstrument").SetViewMode(true);
                $('#SpecifyQuotation').SetViewMode(true);

                $('#divQuotationTargetCode').html($('#QuotationTargetCode').val() + '-' + $('#Alphabet').val());
                $('#divAlphabet').hide();

                if ($('#NegotiationStaffEmpNo1').val().length == 0) {
                    $('#divNegotiationStaffEmpName1').hide();
                }

                if ($('#NegotiationStaffEmpNo2').val().length == 0) {
                    $('#divNegotiationStaffEmpName2').hide();
                }
            }

            DisableRegisterCommand(false);
            DisableResetCommand(false);
        });
}

function BtnConfirmClick() {
    DisableConfirmCommand(true);
    call_ajax_method_json('/Contract/ConfirmClick_CTS052', GetValidateObject(), function (result, controls) {
        if (result != undefined) { //Add by Jutarat A. on 28112013
            if (result.Code == 'MSG0046') {
                OpenInformationMessageDialog(result.Code, result.Message, function () {
                    window.location.href = generate_url("/common/CMS020");
                }, null);
            }
        }

        DisableConfirmCommand(false);
    });       
}

function BtnBackClick() {
    /* --- Set Command Button --- */
    EnableRegisterCommand();
    DisableConfirmCommand(true);

    $("#ContractChange").SetViewMode(false);
    $("#ModifyInstrument").SetViewMode(false);
    $('#SpecifyQuotation').SetViewMode(false);

    DisableConfirmCommand(false);
}

function ResetClick() {
    DisableRegisterCommand(true);
    DisableResetCommand(true);
    var obj = {
        module: "Common",
        code: "MSG0038"
    };
    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenOkCancelDialog(result.Code, result.Message,
        function () {
            call_ajax_method_json('/Contract/ResetData_CTS052', "", function (result, controls) {
                if ((result != null)) {
                    $("#ModifyInstrument :input").val("");
                    $("#Alphabet").val("");
                    ISHideModifyInstrumentSection(true);
                    ISDisableSpecifyQuotation(false);

                    $("#btnViewQuotationDetail").attr("disabled", true);
                    $('#btnSpecifyClear').attr('disabled', true);

                    DisableAllCommand();
                    ReBindingData(result);

                    DisableRegisterCommand(false);
                    DisableResetCommand(false);
                }
            });
        },
        null);
    });
}

function ReBindingData(dataObj) {
    ObjCTS052 = { ContractCodeShort: dataObj.ContractCodeShort
        , ContractCode: dataObj.ContractCode
        , OCC: dataObj.OCC
        , ContractStatus: dataObj.ContractStatus
        , QuotationTargetCode: dataObj.QuotationTargetCode
        , Alphabet: ""
        , EmpNo: ""
        , EmpName: ""
        , DisplayAll: ""
        , BillingClientCode: ""
        , BillingOffice: ""
        , PaymentMethod: ""
        , Sequence: ""
        , ServiceTypeCode: dataObj.ServiceTypeCode
        , TargetCodeType: dataObj.TargetCodeType
        , EndContractDate: dataObj.EndContractDate
        , InstallationStatusCode: dataObj.InstallationStatusCode
    }

    $('#txtContractCode').val(dataObj.ContractCodeShort);
    $('#txtUserCode').val(dataObj.UserCode);
    $('#txtCustomerCode').val(dataObj.CustomerCode);
    $('#txtCustomerCodeReal').val(dataObj.RealCustomerCode);
    $('#txtSiteCode').val(dataObj.SiteCode);
    if (dataObj.ImportantFlag) {
        $('#chkIsimportantCustomer').attr('checked', 'checked');
    } else {
        $('#chkIsimportantCustomer').removeAttr('checked');
    }
    $('#txtContractTargetName').val(dataObj.CustFullNameEN);
    $('#txtContractTargetAddress').val(dataObj.AddressFullEN);
    $('#txtSiteName').val(dataObj.SiteName);
    $('#txtSiteAddress').val(dataObj.SiteAddress);
    $('#txtCustFullNameLC').val(dataObj.CustFullNameLC);
    $('#txtContractTargetAddressLocal').val(dataObj.AddressFullLC);
    $('#txtSiteNameLocal').val(dataObj.SiteNameLC);
    $('#txtSiteAddressLocal').val(dataObj.SiteAddressLC);
    $('#txtInstallationStatus').val(dataObj.InstallationStatus);
    $('#txtOperationOffice').val(dataObj.OfficeName);
    $('#QuotationTargetCode').val(dataObj.QuotationTargetCode);
}

function ChangeFeeNoExpirationCheck() {
    $("#ReturnToOriginalFeeDate").attr("readonly", !$("#ReturnToOriginalFeeDate").prop("readonly"));
    if ($("#ReturnToOriginalFeeDate").prop("readonly") == true) {
        $("#ReturnToOriginalFeeDate").val("");
    }

    if ($("#ChangeFee").prop("checked")) {
        IsDisableNotifyTargetSection(true);
    }
    else {
        IsDisableNotifyTargetSection(false);
    }
}

//-----------------------------------------------------

function ISDisableRegister(status) {
    $('#btnCommandRegister').attr('disabled', status);
    if (status == false) {
        SetRegisterCommand(true, BtnRegisterClick);
    }
}

//--------------------------------------------------------------------------------

function MaintainScreenItemOnInit() {    
    
    DisableAllCommand();

    $('#btnSpecifyClear').attr('disabled', true);
    $('#btnCommandRegister').attr('disabled', true);

    if (mode != "New") {
        $('#btnViewQuotationDetail').attr('disabled', true);
    }
}

function GetValidateObject() {

    var ChangeContractFee = $("#ChangeContractFee").val().toString();
    var registerValidationObject = {
        QuotationTargetCode: $("#QuotationTargetCode").val(),
        Alphabet: $("#Alphabet").val(),
        ChangeImplementDate: $("#ChangeImplementDate").val(),
        NegotiationStaffEmpNo1: $("#NegotiationStaffEmpNo1").val(),
        NegotiationStaffEmpNo2: $("#NegotiationStaffEmpNo2").val(),
        ApproveNo1: $("#ApproveNo1").val(),
        ApproveNo2: $("#ApproveNo2").val(),
        FullNameEN: $("#FullNameEN").val(),
        FullNameLC: $("#FullNameLC").val(),
        AddressEN: $("#AddressEN").val(),
        AddressLC: $("#AddressLC").val()
    }

    return registerValidationObject;
}

function IsDisableNotifyTargetSection(status) {
    $('#NotifyTargetSection').attr('disabled', status);
    $('#NotifyTargetSection :input').attr('disabled', status);
    $('#NotifyTargetSection :input').attr('readonly', status);
}

function ISDisableSpecifyQuotation(status) {
    $('#QuotationTargetCode').attr('readonly', true);
    $('#Alphabet').attr('readonly', status);
    $('#btnRetrieve').attr('disabled', status);
    $('#btnSearchQuotation').attr('disabled', status);
}

function ISHideModifyInstrumentSection(status) {
    if (status == true) {
        $('#ModifyInstrument').hide();
    }
    else {
        $('#ModifyInstrument').show();
    }
}

function GetValidateObject() {
    var registerValidationObject = {
        QuotationTargetCode: $("#QuotationTargetCode").val(),
        Alphabet: $("#Alphabet").val(),
        ChangeImplementDate: $("#ChangeImplementDate").val(),
        NegotiationStaffEmpNo1: $("#NegotiationStaffEmpNo1").val(),
        NegotiationStaffEmpNo2: $("#NegotiationStaffEmpNo2").val(),
        ApproveNo1: $("#ApproveNo1").val(),
        ApproveNo2: $("#ApproveNo2").val()


    }
    return registerValidationObject;
}

function QUS010Object() {
    return {
        strCallerScreenID: "CTS052"
        , ViewMode: '2'
        , strServiceTypeCode: ObjCTS052.ServiceTypeCode
        , strTargetCodeTypeCode: ObjCTS052.TargetCodeType
        , strQuotationTargetCode: $("#QuotationTargetCode").val()
    };
}

function QUS010Response(result) {  
    if (result != undefined) {
        $("#Alphabet").val(result.Alphabet);
        $("#dlgCTS052").CloseDialog();
        BtnRetrieveClick();
    }
}

function QUS012Object() {
    return {
        "QuotationTargetCode": $("#QuotationTargetCode").val(),
        "Alphabet": $("#Alphabet").val(),
        "HideQuotationTarget": true
    };
}

function QUS012Response(result) {
    $("#Alphabet").val(result.Alphabet);
}

function EnableRegisterCommand() {
    DisableAllCommand();
    SetRegisterCommand(true, BtnRegisterClick);
    SetResetCommand(true, ResetClick);
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