/* --- Initial ----------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    /* --- Bind events --- */
    /* ------------------- */
    $("#ContractFee").BindNumericBox(12, 2, 1, 999999999999.99);
    $("#DepositFee").BindNumericBox(12, 2, 0, 999999999999.99);
    /* ------------------- */

    $("#ContractFee").DisableControl();
    $("#DepositFee").DisableControl();
});
/* ----------------------------------------------------------------------------------- */

/* --- Output Methods ---------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function GetQUS030_15_SectionData() {
    /// <summary>Method return object data for ONLINE, BE, SG, MA section</summary>
    return CreateObjectData($("#formFeeInfo_OTHER").serialize());
}
function SetQUS030_15_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode for ONLINE, BE, SG, MA section</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#formFeeInfo_OTHER").SetViewMode(true);
    }
    else {
        $("#formFeeInfo_OTHER").SetViewMode(false);
    }
}
/* ----------------------------------------------------------------------------------- */

function QUS030_15_ResetToNormalControl() {
    $("#formFeeInfo_OTHER").ResetToNormalControl();
}