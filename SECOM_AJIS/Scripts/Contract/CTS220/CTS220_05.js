$(document).ready(function () {
    InitialControlPropertyCTS220_05();
    BindDoInsurance();
    //MaintainScreenItem();
});

function InitialControlPropertyCTS220_05() {
    $("#InsuranceCoverageFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#MonthlyInsuranceFee").BindNumericBox(12, 2, 0, 999999999999.99);
}

function BindDoInsurance() {
    call_ajax_method_json('/Contract/BindDoInsurance_CTS220', "", function (result, controls) {    
        $("#InsuranceType").val(result.InsuranceTypeCode);

        $("#InsuranceCoverageFee").SetNumericCurrency(result.InsuranceCoverageAmountCurrencyType);
        $("#InsuranceCoverageFee").val(result.InsuranceCoverageAmount);
        $("#MonthlyInsuranceFee").SetNumericCurrency(result.MonthlyInsuranceFeeCurrencyType);
        $("#MonthlyInsuranceFee").val(result.MonthlyInsuranceFee);
    });                            
}