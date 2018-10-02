function GetCTS010_15_SectionData() {
    return {
        ContactPoint: $("#ContactPoint").val()
    };
}
function SetCTS010_15_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#divContactPoint").SetViewMode(true);
    }
    else {
        $("#divContactPoint").SetViewMode(false);
    }
}
function SetCTS010_15_EnableSection(enable) {
    $("#divContactPoint").SetEnableView(enable);
}

$(document).ready(function () {
    InitialTrimTextEvent([
        "ContactPoint"
    ]);

    $("#ContactPoint").SetMaxLengthTextArea(500);
});