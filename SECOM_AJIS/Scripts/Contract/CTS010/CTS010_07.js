/// <reference path="../../Base/GridControl.js" />

function GetCTS010_07_SectionData() {
    return CreateObjectData($("#formContractPaymentTerm").serialize());
}
function SetCTS010_07_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#divContractPaymentTerm").SetViewMode(true);
    }
    else {
        $("#divContractPaymentTerm").SetViewMode(false);
    }
}
function SetCTS010_07_EnableSection(enable) {
    $("#divContractPaymentTerm").SetEnableView(enable);
}


/* --- Initial -------------------------------------------------------- */
/* -------------------------------------------------------------------- */
$(document).ready(function () {
    $("#CreditTerm").BindNumericBox(2, 0, 0, 99);
});
/* -------------------------------------------------------------------- */

function InitialContractPaymentTerm() {
    $("#BillingCycle").val($("#defBillingCycle").val());
    $("#PayMethod").val($("#defPayMethod").val());
    $("#CreditTerm").val($("#defCreditTerm").val());
    $("#CalDailyFeeStatus").val($("#defCalDailyFeeStatus").val());
}


