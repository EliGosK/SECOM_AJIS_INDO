/* --- Initial ------------------------------------------------------ */
/* ------------------------------------------------------------------ */
function QUS011Initial() {
    ChangeDialogButtonText(
            ["Close"],
            [$("#btnClose").val()]);
}

$(document).ready(function () {
    var step = new Array();
    if ($("#divQUS011_01").length > 0) {
        step.push("QUS011_01");
    }
    step.push("QUS011_02");
    step.push("QUS011_03");

    CallMultiLoadScreen("Quotation", [step], function () {
        $("#divQuotationTargetInformation").SetEmptyViewData();
        $("#divQuotationDetailInformation").SetEmptyViewData();

        SetControlFromToView("SecurityAreaSizeFrom", "SecurityAreaSizeTo", "QUS011SecurityAreaFromTo");
        EnableControlDialog();
    });
});
/* ------------------------------------------------------------------ */