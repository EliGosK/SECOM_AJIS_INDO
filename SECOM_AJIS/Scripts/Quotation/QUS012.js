/* --- Initial ------------------------------------------------------ */
/* ------------------------------------------------------------------ */
function QUS012Initial() {
    ChangeDialogButtonText(
            ["Close"],
            [$("#btnClose").val()]);
}

$(document).ready(function () {
    var step = new Array();
    for (var idx = 1; idx <= 11; idx++) {
        var tidx = idx;
        if (tidx < 10)
            tidx = "0" + tidx;

        if ($("#divQUS012_" + tidx).length > 0) {
            step.push("QUS012_" + tidx);
        }
    }

    CallMultiLoadScreen("Quotation", [step], function () {
        $("#divQuotationTargetInformation").SetEmptyViewData();
        $("#divQuotationDetailInformtionAL").SetEmptyViewData();
        $("#divQuotationDetailInformationONLINE").SetEmptyViewData();
        $("#divQuotationDetailInfomationOTHER").SetEmptyViewData();
        $("#divMaintenanceDetail").SetEmptyViewData();
        $("#divInstrumentDetailONLINE").SetEmptyViewData();
        $("#divFacilityDetail").SetEmptyViewData();
        $("#divBeatGuardDetail").SetEmptyViewData();
        $("#divSentryGuardDetail").SetEmptyViewData();

        $("#chkOperationType").SetCheckBoxListViewMode();
        $("#chklstServiceType").SetCheckBoxListViewMode();

        SetControlFromToView("SecurityAreaSizeFrom", "SecurityAreaSizeTo", "QUS012SecurityAreaFromTo");
        EnableControlDialog();
    });
});
/* ------------------------------------------------------------------ */