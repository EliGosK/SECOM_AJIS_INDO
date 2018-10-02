$(document).ready(function () {
    //MaintainScreenItem();
    InitialControlPropertyCTS220_08();
    BindDOQuotaion();
});

function BindDOQuotaion() {
    call_ajax_method_json('/Contract/BindDOQuotaion_CTS220', "",
        function (result, controls) {

            $("#QuotationTargetCode").text(result.QuotationTargetCode);
            $("#QuotationAlphabet").val(result.QuotationAlphabet);
            $("#PlanCode").val(result.PlanCode);

            if (result.QuotationTargetCode == "-" 
                || result.QuotationTargetCode == "") {
                $("#divQuotationTargetCode").SetViewMode(true);
            }
            else {
                $("#divQuotationTargetCode").SetViewMode(false);
            }


            result.PlanApproveDate = ConvertDateObject(result.PlanApproveDate);
            $("#PlanApproveDate").val(ConvertDateToTextFormat(result.PlanApproveDate));

            $("#PlanApproverEmpNo").val(result.PlanApproverEmpNo);
            $("#PlanApproverEmpName").val(result.PlanApproverEmpName);

            $("#NormalContractFee").SetNumericCurrency(result.NormalContractFeeCurrencyType);
            $("#NormalContractFee").val(result.NormalContractFee);
            $("#MaintenanceFee1").SetNumericCurrency(result.MaintenanceFee1CurrencyType);
            $("#MaintenanceFee1").val(result.MaintenanceFee1);
            $("#AdditionalFee1").SetNumericCurrency(result.AdditionalFee1CurrencyType);
            $("#AdditionalFee1").val(result.AdditionalFee1);
            $("#AdditionalFee2").SetNumericCurrency(result.AdditionalFee2CurrencyType);
            $("#AdditionalFee2").val(result.AdditionalFee2);
            $("#AdditionalFee3").SetNumericCurrency(result.AdditionalFee3CurrencyType);
            $("#AdditionalFee3").val(result.AdditionalFee3);

        });
}

function InitialControlPropertyCTS220_08() {
    $("#QuotationTargetCode").click(function () { OpenQUS012(); }); 

    $("#NormalContractFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#MaintenanceFee1").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#AdditionalFee1").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#AdditionalFee2").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#AdditionalFee3").BindNumericBox(12, 2, 0, 999999999999.99);
}

/*----- QUS012 Dialog -----*/
function OpenQUS012() {
    $("#dlgCTS220").OpenQUS012Dialog("CTS220");
}

function QUS012Object() {
    return {
        QuotationTargetCode: objCTS220_08.QuotationTargetCode,
        Alphabet: objCTS220_08.QuotationAlphabet,
        HideQuotationTarget: true
    };
}

function QUS012Response(result) {
    $("#dlgCTS220").CloseDialog();
}
/*------------------------*/