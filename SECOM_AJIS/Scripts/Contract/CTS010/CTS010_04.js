/// <reference path="../../Base/GridControl.js" />




function GetCTS010_04_SectionData() {
    var MaintenanceCycle = $("#DefaultMaintenanceCycle").val();
    if ($("#chkMaintenanceCycle").prop("checked") == true)
        MaintenanceCycle = $("#MaintenanceCycle").val();
    return {
        MaintenanceCycle: MaintenanceCycle
    };
}
function SetCTS010_04_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#divMaintenanceInfo_AL_ONLINE").SetViewMode(true);
        $("#chkMaintenanceCycle").hide();
    }
    else {
        $("#divMaintenanceInfo_AL_ONLINE").SetViewMode(false);
        $("#chkMaintenanceCycle").show();
    }
}
function SetCTS010_04_EnableSection(enable) {
    $("#divMaintenanceInfo_AL_ONLINE").SetEnableView(enable);

    if (enable) {
        EnableMaintenanceCycle($("#chkMaintenanceCycle").prop("checked"));
    }
}




/* --- Initial -------------------------------------------------------- */
/* -------------------------------------------------------------------- */
$(document).ready(function () {
    $("#chkMaintenanceCycle").change(function () {
        var enable = false;
        if ($(this).prop("checked") == true) {
            enable = true;
        }
        EnableMaintenanceCycle(enable);
    });

    var enable = false;
    if ($("#MaintenanceCycle").val() != "") {
        if ($("#MaintenanceCycle").val() != $("#DefaultMaintenanceCycle").val()) {
            enable = true;
        }
    }
    EnableMaintenanceCycle(enable);
});
/* -------------------------------------------------------------------- */

/* --- Methods -------------------------------------------------------- */
/* -------------------------------------------------------------------- */
function EnableMaintenanceCycle(enable) {
    if (enable) {
        $("#chkMaintenanceCycle").attr("checked", true);
        $("#MaintenanceCycle").removeAttr("disabled");
    }
    else {
        $("#chkMaintenanceCycle").removeAttr("checked");
        $("#MaintenanceCycle").attr("disabled", true);
        $("#MaintenanceCycle").find("option").each(function () {
            if ($(this).val() == $("#DefaultMaintenanceCycle").val())
                $(this).attr("selected", true);
        });
    }
}
/* -------------------------------------------------------------------- */