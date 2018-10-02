/// <reference path="../../Base/GridControl.js" />


function GetCTS010_08_SectionData() {
    return CreateObjectData($("#formInChargeInformation").serialize());
}
function SetCTS010_08_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#divInChargeInformation").SetViewMode(true);

        for (var idx = 1; idx <= 2; idx++) {
            if ($("#SalesmanEmpNameNo" + idx).val() == "") {
                $("#divSalesmanEmpNameNo" + idx).html("");
            }
        }
        if ($("#SalesSupporterEmpName").val() == "") {
            $("#divSalesSupporterEmpName").html("");
        }
    }
    else {
        $("#divInChargeInformation").SetViewMode(false);
    }
}
function SetCTS010_08_EnableSection(enable) {
    $("#divInChargeInformation").SetEnableView(enable);

    if (enable) {
        $("#SalesmanEmpNameNo1").attr("readonly", true);
        $("#SalesmanEmpNameNo2").attr("readonly", true);
        $("#SalesSupporterEmpName").attr("readonly", true);
    }
}



/* --- Initial -------------------------------------------------------- */
/* -------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "SalesmanEmpNo1",
        "SalesmanEmpNo2",
        "SalesSupporterEmpNo"
    ]);

    GetEmployeeNameData("#SalesmanEmpNo1", "#SalesmanEmpNameNo1");
    GetEmployeeNameData("#SalesmanEmpNo2", "#SalesmanEmpNameNo2");
    GetEmployeeNameData("#SalesSupporterEmpNo", "#SalesSupporterEmpName");
});
/* -------------------------------------------------------------------- */


