function GetCTS020_09_SectionData() {
    return {
        ContactPoint: $("#ContactPoint").val()
    };
}
function SetCTS020_09_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#divContactPoint").SetViewMode(true);
    }
    else {
        $("#divContactPoint").SetViewMode(false);
    }
}
function SetCTS020_09_EnableSection(enable) {
    $("#divContactPoint").SetEnableView(enable);
}

$(document).ready(function () {
    InitialTrimTextEvent([
        "ContactPoint"
    ]);

    $("#ContactPoint").SetMaxLengthTextArea(500);

    if (processType == 2)
        SetCTS020_09_EnableSection(false);
});