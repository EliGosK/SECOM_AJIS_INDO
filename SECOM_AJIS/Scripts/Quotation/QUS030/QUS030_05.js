/* --- Initial ----------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "SalesmanEmpNo1",
        "SalesmanEmpNo2",
        "SalesSupporterEmpNo",
        "AdditionalApproveNo1",
        "AdditionalApproveNo2",
        "AdditionalApproveNo3",
        "ApproveNo1",
        "ApproveNo2",
        "ApproveNo3",
        "ApproveNo4",
        "ApproveNo5"
    ]);

    /* --- Initial Controls --- */
    /* ------------------------ */
    $("#MaintenanceFee1").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#AdditionalFee1").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#AdditionalFee2").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#AdditionalFee3").BindNumericBox(12, 2, 0, 999999999999.99);

    GetEmployeeNameData("#SalesmanEmpNo1", "#Saleman1Name");
    GetEmployeeNameData("#SalesmanEmpNo2", "#Saleman2Name");
    GetEmployeeNameData("#SalesSupporterEmpNo", "#SalesSupporterName");
    /* ------------------------ */

    /* --- Initial Data --- */
    /* -------------------- */
    InitialBeatGuardInfomation();
    /* -------------------- */

});
function InitialBeatGuardInfomation() {
    $("#ProductCode").DisableControl();
    $("#MaintenanceFee1").DisableControl();
    $("#AdditionalFee1").DisableControl();
    $("#AdditionalFee2").DisableControl();
    $("#AdditionalFee3").DisableControl();
    $("#AdditionalApproveNo1").DisableControl();
    $("#AdditionalApproveNo2").DisableControl();
    $("#AdditionalApproveNo3").DisableControl();
    $("#ApproveNo1").DisableControl();
    $("#ApproveNo2").DisableControl();
    $("#ApproveNo3").DisableControl();
    $("#ApproveNo4").DisableControl();
    $("#ApproveNo5").DisableControl();
}
/* ----------------------------------------------------------------------------------- */

/* --- Output Methods ---------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function GetQUS030_05_SectionData() {
    /// <summary>Method return object data for ONLINE, BE, SG, MA section</summary>
    var obj = CreateObjectData($("#formQuotationDetailInfo_OTHER").serialize());
    if (obj != undefined) {
        obj["ProductCode"] = $("#ProductCode").val();
    }

    return obj;
}
function SetQUS030_05_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode for ONLINE, BE, SG, MA section</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#formQuotationDetailInfo_OTHER").SetViewMode(true);

        for (var idx = 1; idx <= 2; idx++) {
            if ($("#Saleman" + idx + "Name").val() == "") {
                $("#divSaleman" + idx + "Name").html("");
            }
        }
        if ($("#SalesSupporterName").val() == "") {
            $("#divSalesSupporterName").html("");
        }
    }
    else {
        $("#formQuotationDetailInfo_OTHER").SetViewMode(false);
    }
}
/* ----------------------------------------------------------------------------------- */

function QUS030_05_ResetToNormalControl() {
    $("#formQuotationDetailInfo_OTHER").ResetToNormalControl();
}