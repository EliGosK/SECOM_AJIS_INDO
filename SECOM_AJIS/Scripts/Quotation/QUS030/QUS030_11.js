/* --- Initial ----------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    /* --- Initial Controls --- */
    /* ------------------------ */
    $("#NumOfDayTimeWd").BindNumericBox(2, 0, 0, 99);
    $("#NumOfNightTimeWd").BindNumericBox(2, 0, 0, 99);
    $("#NumOfDayTimeSat").BindNumericBox(2, 0, 0, 99);
    $("#NumOfNightTimeSat").BindNumericBox(2, 0, 0, 99);
    $("#NumOfDayTimeSun").BindNumericBox(2, 0, 0, 99);
    $("#NumOfNightTimeSun").BindNumericBox(2, 0, 0, 99);
    $("#NumOfBeatStep").BindNumericBox(6, 0, 0, 999999);
    $("#FreqOfGateUsage").BindNumericBox(3, 0, 0, 999);
    $("#NumOfClockKey").BindNumericBox(3, 0, 0, 999);

    $("#NotifyTime").BindTimeBox();
    /* ------------------------ */

    $("#NumOfDayTimeWd").DisableControl();
    $("#NumOfNightTimeWd").DisableControl();
    $("#NumOfDayTimeSat").DisableControl();
    $("#NumOfNightTimeSat").DisableControl();
    $("#NumOfDayTimeSun").DisableControl();
    $("#NumOfNightTimeSun").DisableControl();
    $("#NumOfBeatStep").DisableControl();
    $("#FreqOfGateUsage").DisableControl();
    $("#NumOfClockKey").DisableControl();
    $("#NumOfDate").DisableControl();
    $("#NotifyTime").DisableControl();
});
/* ----------------------------------------------------------------------------------- */

/* --- Output Methods ---------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function GetQUS030_11_SectionData() {
    /// <summary>Method return object data for ONLINE, BE, SG, MA section</summary>
    var obj = CreateObjectData($("#formBeatGuardDetail").serialize());
    obj["NumOfDate"] = $("#NumOfDate").val();

    return obj;
}
function SetQUS030_11_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode for ONLINE, BE, SG, MA section</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#formBeatGuardDetail").SetViewMode(true);
    }
    else {
        $("#formBeatGuardDetail").SetViewMode(false);
    }
}
/* ----------------------------------------------------------------------------------- */

function QUS030_11_ResetToNormalControl() {
    $("#formBeatGuardDetail").ResetToNormalControl();
}