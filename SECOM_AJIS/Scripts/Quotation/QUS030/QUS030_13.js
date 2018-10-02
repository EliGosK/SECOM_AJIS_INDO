/* --- Initial ----------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    /* --- Bind events --- */
    /* ------------------- */
    $("#ProductPrice").BindNumericBox(12, 2, 1, 999999999999.99);
    $("#InstallationFee").BindNumericBox(12, 2, 0, 999999999999.99);
    /* ------------------- */

    $("#ProductPrice").DisableControl();
    $("#InstallationFee").DisableControl();
});
/* ----------------------------------------------------------------------------------- */

/* --- Output Methods ---------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function GetQUS030_13_SectionData() {
    /// <summary>Method return object data for SALE section</summary>
    return CreateObjectData($("#formFeeInfo_Sale").serialize());
}
function SetQUS030_13_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode for SALE section</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#formFeeInfo_Sale").SetViewMode(true);
    }
    else {
        $("#formFeeInfo_Sale").SetViewMode(false);
    }
}
/* ----------------------------------------------------------------------------------- */

function QUS030_13_ResetToNormalControl() {
    $("#formFeeInfo_Sale").ResetToNormalControl();
}