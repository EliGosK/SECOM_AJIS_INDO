/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */

$(document).ready(function () {
    SetInitialStateCTS130_07();
});

function SetInitialStateCTS130_07() {
    $("#ContactPointDetail").SetMaxLengthTextArea(500);
    $("#ContactPointDetail").val($("#ContactPointDetailValue").val());
}

function SetSectionModeCTS130_07(isView) {
    $("#divContactPointSection").SetViewMode(isView);
}
