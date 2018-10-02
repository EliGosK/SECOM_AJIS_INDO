/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

$(document).ready(function () {
    InitialControlCTS180_02();
    InitialEventCTS180_02();
    VisibleDocumentSectionCTS180_02(false);
});

function InitialEventCTS180_02() {
    $("#CT_UseSignaturePicture").click(function () {
        if ($(this).prop("checked") == true) {
            $("#CT_EmployeeName").val("");
            $("#CT_EmployeePosition").val("");

            $("#CT_EmployeeName").attr("disabled", true);
            $("#CT_EmployeePosition").attr("disabled", true);
        }
        else {
            $("#CT_EmployeeName").attr("disabled", false);
            $("#CT_EmployeePosition").attr("disabled", false);
        }
    });
}

function InitialControlCTS180_02() {
    $("#CT_BillingCycle").BindNumericBox(3, 0, 0, 999);
    $("#CT_CreditTerm").BindNumericBox(3, 0, 0, 999);
    $("#CT_ContractDuration").BindNumericBox(3, 0, 0, 999);
    $("#CT_AutoRenew").BindNumericBox(3, 0, 0, 999);

    $("#OrderContractFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#OrderInstallFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#InstallFee_ApproveContract").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#InstallFee_CompleteInstall").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#InstallFee_StartService").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#OrderDepositFee").BindNumericBox(12, 2, 0, 999999999999.99);
}

function InitialInputDataCTS180_02() {
    $("#divContractReport").clearForm();
}

function BindDocumentDataCTS180_02(result) {
    if (result != undefined && result.length == 2) {
        var contDoc = result[0];
        var docContRpt = result[1];

        $("#CT_ContractCodeShort").val(contDoc.ContractCodeShort);
        
        //$("#CT_DocumentLanguage").val(docContRpt.DocumentLanguageCodeName);
        $("#CT_DocumentLanguage").val(docContRpt.DocumentLanguageName);

        if (docContRpt.DocumentLanguage == $("#C_DOC_LANGUAGE_TH").val()) {
            $("#CT_ContractTargetName").val(contDoc.ContractTargetNameLC);
            $("#CT_RealCustomerName").val(contDoc.RealCustomerNameLC);
            $("#CT_SiteAddress").val(contDoc.SiteAddressLC);
        }
        else {
            $("#CT_ContractTargetName").val(contDoc.ContractTargetNameEN);
            $("#CT_RealCustomerName").val(contDoc.RealCustomerNameEN);
            $("#CT_SiteAddress").val(contDoc.SiteAddressEN);
        }

        $("#CT_SiteNameEN").val(contDoc.SiteNameEN);
        $("#CT_SiteNameLC").val(contDoc.SiteNameLC);
        $("#CT_PlanCode").val(docContRpt.PlanCode);
        $("#CT_SystemProduct").val(contDoc.ProductCode);
        $("#CT_LineType").val(contDoc.PhoneLineTypeCode);
        $("#CT_TelephoneOwner").val(contDoc.PhoneLineOwnerTypeCode);

        $("#chkFireMonitoring").attr("checked", contDoc.FireSecurityFlag);
        $("#chkCrimePrevention").attr("checked", contDoc.CrimePreventFlag);
        $("#chkEmergencyReport").attr("checked", contDoc.EmergencyReportFlag);

        $("#CT_BillingTimingType").val(docContRpt.DepositFeePhase);
        $("#CT_BillingCycle").val(contDoc.PaymentCycle);
        $("#CT_PaymentMethodType").val(contDoc.ContractFeePayMethod);
        $("#CT_CreditTerm").val(contDoc.CreditTerm);
        $("#CT_ContractDuration").val(contDoc.ContractDurationMonth);
        $("#CT_AutoRenew").val(docContRpt.AutoRenewMonth);
        $("#CT_CustomerSignatureName").val(docContRpt.CustomerSignatureName);

        //$("#CT_UseSignaturePicture").attr("checked", contDoc.SECOMSignatureFlag);
        //$("#CT_EmployeeName").val(contDoc.EmpName);
        //$("#CT_EmployeePosition").val(contDoc.EmpPosition);
        $("#CT_UseSignaturePicture").attr("checked", contDoc.SECOMSignatureFlag);
        if (contDoc.SECOMSignatureFlag == true) {
            $("#CT_EmployeeName").attr("disabled", true);
            $("#CT_EmployeePosition").attr("disabled", true);
        }
        else {
            $("#CT_EmployeeName").val(contDoc.EmpName);
            $("#CT_EmployeePosition").val(contDoc.EmpPosition);
        }

        $("#OrderContractFee").SetNumericCurrency(contDoc.ContractFeeCurrencyType);
        $("#OrderContractFee").val(SetNumericValue(contDoc.ContractFeeCurrencyType == C_CURRENCY_US ? 
            contDoc.ContractFeeUsd : contDoc.ContractFee, 2));
        $("#OrderContractFee").setComma();
        
        $("#OrderInstallFee").SetNumericCurrency(docContRpt.NegotiationTotalInstallFeeCurrencyType);
        $("#OrderInstallFee").val(SetNumericValue(docContRpt.NegotiationTotalInstallFeeCurrencyType == C_CURRENCY_US ? 
            docContRpt.NegotiationTotalInstallFeeUsd : docContRpt.NegotiationTotalInstallFee, 2));
        $("#OrderInstallFee").setComma();

        $("#InstallFee_ApproveContract").SetNumericCurrency(docContRpt.InstallFee_ApproveContractCurrencyType);
        $("#InstallFee_ApproveContract").val(SetNumericValue(docContRpt.InstallFee_ApproveContractCurrencyType == C_CURRENCY_US ? 
            docContRpt.InstallFee_ApproveContractUsd : docContRpt.InstallFee_ApproveContract, 2));
        $("#InstallFee_ApproveContract").setComma();

        $("#InstallFee_CompleteInstall").SetNumericCurrency(docContRpt.InstallFee_CompleteInstallCurrencyType);
        $("#InstallFee_CompleteInstall").val(SetNumericValue(docContRpt.InstallFee_CompleteInstallCurrencyType == C_CURRENCY_US ? 
            docContRpt.InstallFee_CompleteInstallUsd : docContRpt.InstallFee_CompleteInstall, 2));
        $("#InstallFee_CompleteInstall").setComma();

        $("#InstallFee_StartService").SetNumericCurrency(docContRpt.InstallFee_StartServiceCurrencyType);
        $("#InstallFee_StartService").val(SetNumericValue(docContRpt.InstallFee_StartServiceCurrencyType == C_CURRENCY_US ?
            docContRpt.InstallFee_StartServiceUsd : docContRpt.InstallFee_StartService, 2));
        $("#InstallFee_StartService").setComma();

        $("#OrderDepositFee").SetNumericCurrency(contDoc.DepositFeeCurrencyType);
        $("#OrderDepositFee").val(SetNumericValue(contDoc.DepositFeeCurrencyType == C_CURRENCY_US ? 
            contDoc.DepositFeeUsd : contDoc.DepositFee, 2));
        $("#OrderDepositFee").setComma();

        //InitialContractReportGridData();
    }
}

//function InitialContractReportGridData() {

//    gridContractReportListCTS180 = $("#gridContractReportList").LoadDataToGridWithInitial(0, false, false, "/Contract/CTS180_GetContractReportList",
//    "", "CTS180_ContractReportGridData", false);

//    SpecialGridControl(gridContractReportListCTS180, ["Order", "ApproveContract", "CompleteInstallation", "StartService"]);

//    BindOnLoadedEvent(gridContractReportListCTS180,
//        function (gen_ctrl) {

//            for (var i = 0; i < gridContractReportListCTS180.getRowsNum(); i++) {
//                var orderColinx = gridContractReportListCTS180.getColIndexById("Order");
//                var aprvContColinx = gridContractReportListCTS180.getColIndexById("ApproveContract");
//                var compInstColinx = gridContractReportListCTS180.getColIndexById("CompleteInstallation");
//                var startServColinx = gridContractReportListCTS180.getColIndexById("StartService");

//                var row_id = gridContractReportListCTS180.getRowId(i);

//                if (gen_ctrl == true) {
//                    var nValOrder = GetValueFromLinkType(gridContractReportListCTS180, i, orderColinx);
//                    GenerateNumericBoxWithUnit2(gridContractReportListCTS180, "OrderFee", row_id, "Order", nValOrder, 10, 2, 0, 9999999999.99, false, "120px", C_CURRENCY_UNIT, false, $("#CanEditFee").val() == "True");

//                    if (i == 1) {
//                        var nValAprvCont = GetValueFromLinkType(gridContractReportListCTS180, i, aprvContColinx);
//                        GenerateNumericBoxWithUnit2(gridContractReportListCTS180, "AprvCont", row_id, "ApproveContract", nValAprvCont, 10, 2, 0, 9999999999.99, false, "120px", C_CURRENCY_UNIT, false, $("#CanEditFee").val() == "True");

//                        var nValCompInst = GetValueFromLinkType(gridContractReportListCTS180, i, compInstColinx);
//                        GenerateNumericBoxWithUnit2(gridContractReportListCTS180, "CompInst", row_id, "CompleteInstallation", nValCompInst, 10, 2, 0, 9999999999.99, false, "120px", C_CURRENCY_UNIT, false, $("#CanEditFee").val() == "True");

//                        var nValStartServ = GetValueFromLinkType(gridContractReportListCTS180, i, startServColinx);
//                        GenerateNumericBoxWithUnit2(gridContractReportListCTS180, "StartServ", row_id, "StartService", nValStartServ, 10, 2, 0, 9999999999.99, false, "120px", C_CURRENCY_UNIT, false, $("#CanEditFee").val() == "True");
//                    }
//                }

//            }

//            gridContractReportListCTS180.setSizes();
//        }
//    );

//}

function SetDocumentSectionModeCTS180_02(isView) {
    $("#divContractReport").SetViewMode(isView);
}

function VisibleDocumentSectionCTS180_02(isVisible) {
    if (isVisible) {
        $("#divContractReport").show();
    }
    else {
        $("#divContractReport").hide();
    }
}

function SetRegisterStateCTS180_02() {
    $("#CT_ContractCodeShort").attr("readonly", true);
    $("#CT_DocumentLanguage").attr("readonly", true);
    $("#CT_ContractTargetName").attr("readonly", true);

    if ($("#CanEditFee").val() == "False")
        $("#CT_BillingTimingType").attr("disabled", true);
}

function GetDocumentDataCTS180_02() {
    //var row_id;
    //var orderColinx = gridContractReportListCTS180.getColIndexById("Order");
    //var aprvContColinx = gridContractReportListCTS180.getColIndexById("ApproveContract");
    //var compInstColinx = gridContractReportListCTS180.getColIndexById("CompleteInstallation");
    //var startServColinx = gridContractReportListCTS180.getColIndexById("StartService");

    //row_id = gridContractReportListCTS180.getRowId(0);
    //var contOrderCtrlName = GenerateGridControlID("OrderFee", row_id);
    //var nValContOrder = ($("#" + contOrderCtrlName)).NumericValue();

    //row_id = gridContractReportListCTS180.getRowId(1);
    //var nValInstOrder = ($("#" + GenerateGridControlID("OrderFee", row_id))).NumericValue();
    //var nValAprvCont = ($("#" + GenerateGridControlID("AprvCont", row_id))).NumericValue();
    //var nValCompInst = ($("#" + GenerateGridControlID("CompInst", row_id))).NumericValue();
    //var nValStartServ = ($("#" + GenerateGridControlID("StartServ", row_id))).NumericValue();

    //row_id = gridContractReportListCTS180.getRowId(2);
    //var nValDepOrder = ($("#" + GenerateGridControlID("OrderFee", row_id))).NumericValue();

    var objCTS180_02 = {
        ContractCodeShort: $("#CT_ContractCodeShort").val(),
        ContractTargetNameLC: $("#CT_ContractTargetName").val(),
        RealCustomerNameLC: $("#CT_RealCustomerName").val(),
        SiteAddressLC: $("#CT_SiteAddress").val(),
        ContractTargetNameEN: $("#CT_ContractTargetName").val(),
        RealCustomerNameEN: $("#CT_RealCustomerName").val(),
        SiteAddressEN: $("#CT_SiteAddress").val(),
        SiteNameEN: $("#CT_SiteNameEN").val(),
        SiteNameLC: $("#CT_SiteNameLC").val(),
        PlanCode: $("#CT_PlanCode").val(),
        ProductCode: $("#CT_SystemProduct").val(),
        PhoneLineTypeCode: $("#CT_LineType").val(),
        PhoneLineOwnerTypeCode: $("#CT_TelephoneOwner").val(),
        FireSecurityFlag: $("#chkFireMonitoring").prop("checked"),
        CrimePreventFlag: $("#chkCrimePrevention").prop("checked"),
        EmergencyReportFlag: $("#chkEmergencyReport").prop("checked"),

        //ContractFee: nValContOrder,
        //ContractFeeControlName: contOrderCtrlName,
        //NegotiationTotalInstallFee: nValInstOrder,
        //InstallFee_ApproveContract: nValAprvCont,
        //InstallFee_CompleteInstall: nValCompInst,
        //InstallFee_StartService: nValStartServ,
        //DepositFee: nValDepOrder,

        ContractFeeCurrencyType: $("#OrderContractFee").NumericCurrencyValue(),
        ContractFee: $("#OrderContractFee").NumericValue(),
        ContractFeeControlName: "OrderContractFee",

        NegotiationTotalInstallFeeCurrencyType: $("#OrderInstallFee").NumericCurrencyValue(),
        NegotiationTotalInstallFee: $("#OrderInstallFee").NumericValue(),

        InstallFee_ApproveContractCurrencyType: $("#InstallFee_ApproveContract").NumericCurrencyValue(),
        InstallFee_ApproveContract: $("#InstallFee_ApproveContract").NumericValue(),

        InstallFee_CompleteInstallCurrencyType: $("#InstallFee_CompleteInstall").NumericCurrencyValue(),
        InstallFee_CompleteInstall: $("#InstallFee_CompleteInstall").NumericValue(),

        InstallFee_StartServiceCurrencyType: $("#InstallFee_StartService").NumericCurrencyValue(),
        InstallFee_StartService: $("#InstallFee_StartService").NumericValue(),

        DepositFeeCurrencyType: $("#OrderDepositFee").NumericCurrencyValue(),
        DepositFee: $("#OrderDepositFee").NumericValue(),

        DepositFeePhase: $("#CT_BillingTimingType").val(),
        PaymentCycle: $("#CT_BillingCycle").NumericValue(),
        ContractFeePayMethod: $("#CT_PaymentMethodType").val(),
        CreditTerm: $("#CT_CreditTerm").NumericValue(),
        ContractDurationMonth: $("#CT_ContractDuration").NumericValue(),
        AutoRenewMonth: $("#CT_AutoRenew").NumericValue(),
        CustomerSignatureName: $("#CT_CustomerSignatureName").val(),
        SECOMSignatureFlag: $("#CT_UseSignaturePicture").prop("checked"),
        EmpName: $("#CT_EmployeeName").val(),
        EmpPosition: $("#CT_EmployeePosition").val()
    };

    return objCTS180_02;
}