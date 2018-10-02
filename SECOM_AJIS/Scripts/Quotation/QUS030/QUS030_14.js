/* --- Initial ----------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    /* --- Bind events --- */
    /* ------------------- */
    $("#ContractFee").BindNumericBox(12, 2, 1, 999999999999.99);
    $("#InstallationFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#DepositFee").BindNumericBox(12, 2, 0, 999999999999.99);
    /* ------------------- */

    $("#ContractFee").DisableControl();
    $("#InstallationFee").DisableControl();
    $("#DepositFee").DisableControl();
});
/* ----------------------------------------------------------------------------------- */

/* --- Output Methods ---------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function GetQUS030_14_SectionData() {
    /// <summary>Method return object data for AL section</summary>
    return CreateObjectData($("#formFeeInfo_AL").serialize());
}
function SetQUS030_14_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode for AL section</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#formFeeInfo_AL").SetViewMode(true);
    }
    else {
        $("#formFeeInfo_AL").SetViewMode(false);
    }
}
/* ----------------------------------------------------------------------------------- */

function QUS030_14_ResetToNormalControl() {
    $("#formFeeInfo_AL").ResetToNormalControl();
}