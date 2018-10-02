
$(document).ready(function () {
});

function InitModeSection(steps) {
    $("#divSpecifyProcessTypeSection").SetEnableView(false);
//    $("#divSpecifyProcessTypeSection").attr('disabled', true);
//    $("#divSpecifyProcessTypeSection").attr('readonly', true);
//    $('#divSpecifyProcessTypeSection :input').attr('disabled', true);
//    $('#divSpecifyProcessTypeSection :input').attr('readonly', true);

    CallMultiLoadScreen("Contract", steps, MaintainScreenItem);
}

function MaintainScreenItem() {
    if (processType == "Delete")
        MaintainScreenItemDeleteMode();
    else
        MaintainScreenItemInsertMode();
}

function MaintainScreenItemInsertMode() {
//    $("#divSpecifyProcessTypeSection").attr('disabled', true);
//    $("#divSpecifyProcessTypeSection").attr('readonly', true);
//    $('#divSpecifyProcessTypeSection :input').attr('disabled', true);
//    $('#divSpecifyProcessTypeSection :input').attr('readonly', true);

//    $("#divContractBasicSection").show();
    $("#divProductSection").show();
//    $("#divChangeContractSection").show();
//    $("#divInsuranceSection").show();
//    $("#divContractDocumentSection").show();
//    $("#divFutureDateSection").show();
//    $("#divQuotationSection").show();
//    $("#divInstallationSection").show();

    $("#ChangeImplementDate").InitialDate();
    $("#ExpectedResumeDate").InitialDate();
    $("#ReturnToOriginalFeeDate").InitialDate();
    $("#InstallationCompleteDate").InitialDate();

    SetRegisterCommand(true, BtnRegisterClick);
    SetResetCommand(true, BtnResetClick);
}

function MaintainScreenItemDeleteMode() {
//    $("#divSpecifyProcessTypeSection").attr('disabled', true);
//    $("#divSpecifyProcessTypeSection").attr('readonly', true);
//    $('#divSpecifyProcessTypeSection :input').attr('disabled', true);
//    $('#divSpecifyProcessTypeSection :input').attr('readonly', true);

//    $("#divContractBasicSection").show();
//    $("#divContractBasicSection").attr('disabled', true);
//    $("#divContractBasicSection").attr('readonly', true);
//    $('#divContractBasicSection :input').attr('disabled', true);
//    $('#divContractBasicSection :input').attr('readonly', true);
    $('#divContractBasicSection').SetDisabled(true);

//    $("#divProductSection").show();
//    $("#divProductSection").attr('disabled', true);
//    $("#divProductSection").attr('readonly', true);
//    $('#divProductSection :input').attr('disabled', true);
//    $('#divProductSection :input').attr('readonly', true);
    $('#divProductSection').SetDisabled(true);

//    $("#divChangeContractSection").show();
//    $("#divChangeContractSection").attr('disabled', true);
//    $("#divChangeContractSection").attr('readonly', true);
//    $('#divChangeContractSection :input').attr('disabled', true);
//    $('#divChangeContractSection :input').attr('readonly', true);
    $('#divChangeContractSection').SetDisabled(true);

//    $("#divInsuranceSection").show();
//    $("#divInsuranceSection").attr('disabled', true);
//    $("#divInsuranceSection").attr('readonly', true);
//    $('#divInsuranceSection :input').attr('disabled', true);
//    $('#divInsuranceSection :input').attr('readonly', true);
    $('#divInsuranceSection').SetDisabled(true);

//    $("#divContractDocumentSection").show();
//    $("#divContractDocumentSection").attr('disabled', true);
//    $("#divContractDocumentSection").attr('readonly', true);
//    $('#divContractDocumentSection :input').attr('disabled', true);
//    $('#divContractDocumentSection :input').attr('readonly', true);
    $('#divContractDocumentSection').SetDisabled(true);

//    $("#divFutureDateSection").show();
//    $("#divFutureDateSection").attr('disabled', true);
//    $("#divFutureDateSection").attr('readonly', true);
//    $('#divFutureDateSection :input').attr('disabled', true);
//    $('#divFutureDateSection :input').attr('readonly', true);
    $('#divFutureDateSection').SetDisabled(true);

//   $("#divQuotationSection").show();
//    $("#divQuotationSection").attr('disabled', true);
//    $("#divQuotationSection").attr('readonly', true);
//    $('#divQuotationSection :input').attr('disabled', true);
//    $('#divQuotationSection :input').attr('readonly', true);
    $('#divQuotationSection').SetDisabled(true);

//    $("#divInstallationSection").show();
//    $("#divInstallationSection").attr('disabled', true);
//    $("#divInstallationSection").attr('readonly', true);
//    $('#divInstallationSection :input').attr('disabled', true);
//    $('#divInstallationSection :input').attr('readonly', true);
    $('#divInstallationSection').SetDisabled(true);

    //Disable mygridCTS220_09
    if (CheckFirstRowIsEmpty(mygridCTS220_09, false) == false) {
        BindInstallationGrid(false);
    }

    SetOKCommand(true, BtnOKClick);
    SetCancelCommand(true, BtnCancelClick);

    SetRegisterCommand(false, null);
    SetResetCommand(false, null);
}

function BtnRegisterClick() {
    command_control.CommandControlMode(false);

    obj = GetValidateObject();
    call_ajax_method_json('/Contract/RegisterClick_CTS220', obj,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["OrderContractFee",
                            "ChangeType",
                            "ChangeImplementDate",
                            "SaleManEmpNo1",
                            "SaleManEmpNo2",
                            "NegotiationStaffEmpNo1",
                            "NegotiationStaffEmpNo2",
                            "ChangeReasonType",
                            "ReturnToOriginalFeeDate",
                            "ExpectedResumeDate"], controls);
                return;
            }
            else if (result != undefined) {
                $("#divContractBasicSection").SetViewMode(true);

                //$("#divProductSection").SetViewMode(true);
                SetSectionModeCTS220_03(true);

                //$("#divChangeContractSection").SetViewMode(true);
                SetSectionModeCTS220_04(true);

                $("#divInsuranceSection").SetViewMode(true);
                $("#divContractDocumentSection").SetViewMode(true);
                $("#divFutureDateSection").SetViewMode(true);
                $("#divQuotationSection").SetViewMode(true);

                //$("#divInstallationSection").SetViewMode(true);
                SetSectionModeCTS220_09(true);

                SetConfirmCommand(true, BtnConfirmClick);
                SetBackCommand(true, BtnBackClick);
                SetRegisterCommand(false, null);
                SetResetCommand(false, null);
            }
        });

    //command_control.CommandControlMode(true);
}

function BtnResetClick() {
    DisableResetCommand(true);

    /* --- Get Message --- */
    var obj = {
        module: "Common",
        code: "MSG0038"
    };
    call_ajax_method_json("/Shared/GetMessage", obj, 
        function (result) {
            OpenOkCancelDialog(result.Code, result.Message,
            function () {
    //            InitialSelectProcessMode();
    //            $("#rdoCorrectSecurityData").attr("checked", true);
    //            $("#Occurence").val("");

    //            SetRegisterCommand(false, null);
    //            SetResetCommand(false, null);

                //SetResetState();
                var obj = { ContractCode: $("#ContractCode").val() };
                ajax_method.CallScreenControllerWithAuthority("/Contract/CTS220", obj, false, null);
            },
            null);
        });

    DisableResetCommand(false);
}

function SetResetState() {
    InitialSelectProcessMode();
    $("#rdoCorrectSecurityData").attr("checked", true);
    $("#Occurence").val("");

    SetRegisterCommand(false, null);
    SetResetCommand(false, null);
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
    SetOKCommand(false, null);
    SetCancelCommand(false, null);
}

function BtnConfirmClick() {
    command_control.CommandControlMode(false);

    obj = GetValidateObject();
    call_ajax_method_json('/Contract/ConfirmClick_CTS220', obj,
        function (result, controls) {
            //if (result.Code == "MSG0046") {
            if (result != undefined) {
                OpenInformationMessageDialog(result.Code, result.Message, function () {
                    //SetConfirmCommand(false, null);
                    //SetBackCommand(false, null);
                    //BtnResetClick();
                    SetResetState();
                }, null);
            }
        });

    //command_control.CommandControlMode(true);
}

function BtnBackClick() {
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);

    SetRegisterCommand(true, BtnRegisterClick);
    SetResetCommand(true, BtnResetClick);

    $("#divContractBasicSection").SetViewMode(false);
    $("#ImportantFlag").attr("disabled", true)

    //$("#divProductSection").SetViewMode(false);
    SetSectionModeCTS220_03(false);

    //$("#divChangeContractSection").SetViewMode(false);
    SetSectionModeCTS220_04(false);

    $("#divInsuranceSection").SetViewMode(false);
    $("#divContractDocumentSection").SetViewMode(false);
    $("#divFutureDateSection").SetViewMode(false);
    $("#divQuotationSection").SetViewMode(false);

    //$("#divInstallationSection").SetViewMode(false);
    SetSectionModeCTS220_09(false);
}

function BtnOKClick() {
    obj = GetValidateObject();
    call_ajax_method('/Contract/OKClick_CTS220', obj,
    function (result, controls) {
        if (result == undefined) {

            var obj = {
                module: "Contract",
                code: "MSG3137"
            };

            call_ajax_method_json("/Shared/GetMessage", obj, 
                function (result) {
                    OpenYesNoWarningMessageDialog(result.Code, result.Message,
                        function () {
                            obj = GetValidateObject();
                            call_ajax_method('/Contract/ConfirmClick_CTS220', obj,
                            function (result, controls) {
//                                if (result == true) {
//                                    OpenErrorMessageDialog(result.Code, result.Message, function () { }, null);
//                                }

//                                if (result != true && result != false) {
                                if (result != undefined) {
                                    OpenInformationMessageDialog(result.Code, result.Message, function () {
                                        //SetOKCommand(false, null);
                                        //SetCancelCommand(false, null);
                                        //BtnResetClick();
                                        SetResetState();
                                    }, null);
                                }
                            });
                        });
                });
        }
    });
}

function BtnCancelClick() {
//    InitialSelectProcessMode();
//    $("#rdoCorrectSecurityData").attr("checked", true);
//    $("#Occurence").val("");

//    SetOKCommand(false, null);
//    SetCancelCommand(false, null);
    SetResetState();
}

function GetValidateObject() {
    var processType;
    if ($("#rdoCorrectSecurityData").prop("checked") == true)
        processType = "Correct";

    if ($("#rdoInsertSecurityOccurence").prop("checked") == true)
        processType = "Insert";

    if ($("#rdoDeleteOperatedOccurence").prop("checked") == true)
        processType = "Delete";

    var operationType = new Array();
    $("#OperationType").find("input:checkbox").each(function () {
        if ($(this).prop("checked") == true) {
            operationType.push($(this).val());
        }
    });

    var obj = {
        ContractCode: $("#ContractCode").val(),
        OCC: $("#Occurence").val(),
        ProcessType: processType,

        ContractFeeCurrencyType: $("#OrderContractFee").NumericCurrencyValue(),
        ContractFee: $("#OrderContractFee").NumericValue(),
        ContractFeeOnStopCurrencyType: $("#StopFee").NumericCurrencyValue(),
        ContractFeeOnStop: $("#StopFee").NumericValue(), //Add by Jutarat A. on 14082012

        ChangeImplementDate: $("#ChangeImplementDate").val(),
        ChangeType: $("#ChangeType").val(),
        ChangeReasonType: $("#ChangeReasonType").val(),
        DocumentCode: $("#DocumentName").val(),
        SecurityMemo: $("#SecurityMemo").val(),
        //OperationTypeCode: $("#OperationTypeCode").val(),
        InsuranceTypeCode: $("#InsuranceType").val(),

        InsuranceCoverageAmountCurrencyType: $("#InsuranceCoverageFee").NumericCurrencyValue(),
        InsuranceCoverageAmount: $("#InsuranceCoverageFee").NumericValue(),
        MonthlyInsuranceFeeCurrencyType: $("#MonthlyInsuranceFee").NumericCurrencyValue(),
        MonthlyInsuranceFee: $("#MonthlyInsuranceFee").NumericValue(),

        PlanCode: $("#PlanCode").val(),

        MaintenanceFee1CurrencyType: $("#MaintenanceFee1").NumericCurrencyValue(),
        MaintenanceFee1: $("#MaintenanceFee1").NumericValue(),
        AdditionalFee1CurrencyType: $("#AdditionalFee1").NumericCurrencyValue(),
        AdditionalFee1: $("#AdditionalFee1").NumericValue(),
        AdditionalFee2CurrencyType: $("#AdditionalFee2").NumericCurrencyValue(),
        AdditionalFee2: $("#AdditionalFee2").NumericValue(),
        AdditionalFee3CurrencyType: $("#AdditionalFee3").NumericCurrencyValue(),
        AdditionalFee3: $("#AdditionalFee3").NumericValue(),

        InstallationTypeCode: $("#InstallationTypeCode").val(),
        InstallationCompleteDate: $("#InstallationCompleteDate").val(),

        NormalInstallFeeCurrencyType: $("#NormalInstallFee").NumericCurrencyValue(),
        NormalInstallFee: $("#NormalInstallFee").NumericValue(),
        OrderInstallFeeCurrencyType: $("#OrderInstallFee").NumericCurrencyValue(),
        OrderInstallFee: $("#OrderInstallFee").NumericValue(),
        OrderInstallFee_ApproveContractCurrencyType: $("#OrderInstallFee_ApproveContract").NumericCurrencyValue(),
        OrderInstallFee_ApproveContract: $("#OrderInstallFee_ApproveContract").NumericValue(),
        OrderInstallFee_CompleteInstallCurrencyType: $("#OrderInstallFee_CompleteInstall").NumericCurrencyValue(),
        OrderInstallFee_CompleteInstall: $("#OrderInstallFee_CompleteInstall").NumericValue(),
        OrderInstallFee_StartServiceCurrencyType: $("#OrderInstallFee_StartService").NumericCurrencyValue(),
        OrderInstallFee_StartService: $("#OrderInstallFee_StartService").NumericValue(),

        ReturnToOriginalFeeDate: $("#ReturnToOriginalFeeDate").val(),
        ExpectedResumeDate: $("#ExpectedResumeDate").val(),
        SalesmanEmpNo1: $("#SaleManEmpNo1").val(),
        SalesmanEmpNo2: $("#SaleManEmpNo2").val(),
        NegotiationStaffEmpNo1: $("#NegotiationStaffEmpNo1").val(),
        NegotiationStaffEmpNo2: $("#NegotiationStaffEmpNo2").val(),
        ApproveNo1: $("#ApproveNo1").val(),
        ApproveNo2: $("#ApproveNo2").val(),
        IsEnableReasonType: ($("#ChangeReasonType").prop("disabled") == false),
        OperationType: operationType,

        NormalContractFeeCurrencyType: $("#NormalContractFee").NumericCurrencyValue(),
        NormalContractFee: $("#NormalContractFee").NumericValue(), //Add by Jutarat A. on 06022014

        InstallFeePaidBySECOMCurrencyType: $("#InstallFeePaidBySECOM").NumericCurrencyValue(),
        InstallFeePaidBySECOM: $("#InstallFeePaidBySECOM").NumericValue(),
        InstallFeeRevenueBySECOMCurrencyType: $("#InstallFeeRevenueBySECOM").NumericCurrencyValue(),
        InstallFeeRevenueBySECOM: $("#InstallFeeRevenueBySECOM").NumericValue()
    }

    return obj;
}
