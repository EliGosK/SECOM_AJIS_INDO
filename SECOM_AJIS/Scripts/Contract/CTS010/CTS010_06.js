/// <reference path="../../Base/GridControl.js" />



function GetCTS010_06_SectionData() {
    var flag = "0";
    if ($("#IrregulationContractDurationFlag").prop("checked") == true)
        flag = "1";

    return {
        IrregulationContractDurationFlag: flag,
        ContractDurationMonth: $("#ContractDurationMonth").val(),
        AutoRenewMonth: $("#AutoRenewMonth").val(),
        ContractEndDate: $("#ContractEndDate").val()
    };
}
function SetCTS010_06_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#divContractDurationInfo").SetViewMode(true);
    }
    else {
        $("#divContractDurationInfo").SetViewMode(false);
    }
}
function SetCTS010_06_EnableSection(enable) {
    $("#divContractDurationInfo").SetEnableView(enable);

    if (enable) {
        SetEnableContractDuration($("#IrregulationContractDurationFlag").prop("checked"));
    }
}



/* --- Initial -------------------------------------------------------- */
/* -------------------------------------------------------------------- */
$(document).ready(function () {
    $("#ContractDurationMonth").BindNumericBox(3, 0, 0, 999);
    $("#AutoRenewMonth").BindNumericBox(3, 0, 0, 999);
    $("#ContractEndDate").InitialDate();

    $("#IrregulationContractDurationFlag").change(irregulation_contract_duration_change);
    SetEnableContractDuration($("#IrregulationContractDurationFlag").prop("checked"));
});
/* -------------------------------------------------------------------- */

/* --- Initial -------------------------------------------------------- */
/* -------------------------------------------------------------------- */
function irregulation_contract_duration_change() {
    SetEnableContractDuration($(this).prop("checked"));
}
/* -------------------------------------------------------------------- */


/* --- Methods -------------------------------------------------------- */
/* -------------------------------------------------------------------- */
function SetEnableContractDuration(enable) {
    if (enable) {
        $("#ContractDurationMonth").removeAttr("readonly");
        $("#AutoRenewMonth").removeAttr("readonly");
        $("#ContractEndDate").EnableDatePicker(true);
    }
    else {
        $("#ContractDurationMonth").val($("#defContractDurationMonth").val());
        $("#AutoRenewMonth").val($("#defAutoRenewMonth").val());
        $("#ContractEndDate").val($("#defContractEndDate").val());

        $("#ContractDurationMonth").attr("readonly", true);
        $("#AutoRenewMonth").attr("readonly", true);
        $("#ContractEndDate").EnableDatePicker(false);
    }
}
/* -------------------------------------------------------------------- */