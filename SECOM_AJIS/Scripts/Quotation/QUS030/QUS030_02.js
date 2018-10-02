/// <reference path="../../Base/Master.js" />

/* --- Initial ----------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "PlanCode",
        "PlannerEmpNo",
        "PlanCheckerEmpNo",
        "PlanApproverEmpNo",
        "SalesmanEmpNo1",
        "SalesmanEmpNo2",
        "SalesmanEmpNo3",
        "SalesmanEmpNo4",
        "SalesmanEmpNo5",
        "SalesmanEmpNo6",
        "SalesmanEmpNo7",
        "SalesmanEmpNo8",
        "SalesmanEmpNo9",
        "SalesmanEmpNo10",
        "ApproveNo1",
        "ApproveNo2",
        "ApproveNo3",
        "ApproveNo4",
        "ApproveNo5"
    ]);

    /* --- Initial Controls --- */
    /* ------------------------ */
    ShowSaleInformation(false);

    $("#PlanCheckDate").InitialDate();
    $("#PlanApproveDate").InitialDate();

    $("#SiteBuildingArea").BindNumericBox(5, 2, 0, 99999.99);
    $("#SecurityAreaFrom").BindNumericBox(5, 2, 0, 99999.99);
    $("#SecurityAreaTo").BindNumericBox(5, 2, 0, 99999.99);
    $("#BidGuaranteeAmount1").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#BidGuaranteeAmount2").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#NewBldMgmtCost").BindNumericBox(12, 2, 0, 999999999999.99);

    GetEmployeeNameData("#PlannerEmpNo", "#PlannerName");
    GetEmployeeNameData("#PlanCheckerEmpNo", "#PlanCheckerName");
    GetEmployeeNameData("#PlanApproverEmpNo", "#PlanApproverName");
    GetEmployeeNameData("#SalesmanEmpNo1", "#Saleman1Name");
    GetEmployeeNameData("#SalesmanEmpNo2", "#Saleman2Name");
    GetEmployeeNameData("#SalesmanEmpNo3", "#Saleman3Name");
    GetEmployeeNameData("#SalesmanEmpNo4", "#Saleman4Name");
    GetEmployeeNameData("#SalesmanEmpNo5", "#Saleman5Name");
    GetEmployeeNameData("#SalesmanEmpNo6", "#Saleman6Name");
    GetEmployeeNameData("#SalesmanEmpNo7", "#Saleman7Name");
    GetEmployeeNameData("#SalesmanEmpNo8", "#Saleman8Name");
    GetEmployeeNameData("#SalesmanEmpNo9", "#Saleman9Name");
    GetEmployeeNameData("#SalesmanEmpNo10", "#Saleman10Name");
    /* ------------------------ */

    /* --- Initial Data --- */
    /* -------------------- */
    InitialSaleInfomation();
    /* -------------------- */

    /* --- Initial Events --- */
    /* ---------------------- */
    $("#BuildingTypeCode").RelateControlEvent(building_type_code_change);


    $("#NewBldMgmtFlagNeed").change(new_bld_mgmt_flag_change);
    $("#NewBldMgmtFlagNoNeed").change(new_bld_mgmt_flag_change);
    $("span[id=aLess]").click(aLess_click);
    $("span[id=aMore]").click(aMore_click);
    /* ---------------------- */

    /* Begin:: 2015/11/11 Added: tbt_QuotationInstallationDetail */
    $("#txtCeilingHeight").BindNumericBox(5, 2, 0, 99999.99);
    $("#chkSpecialInsOther").change(function () {
        if ($(this).is(":checked")) {
            $("#txtSpecialInsOther").SetDisabled(false);
        }
        else {
            $("#txtSpecialInsOther").SetDisabled(true);
            $("#txtSpecialInsOther").val("");
        }
    });
    $("#chkCeilingTypeTBar, #chkCeilingTypeSlabConcrete, #chkCeilingTypeMBar, #chkCeilingTypeSteel").change(function () {
        if ($("#chkCeilingTypeTBar, #chkCeilingTypeSlabConcrete, #chkCeilingTypeMBar, #chkCeilingTypeSteel").is(":checked")) {
            $("#chkCeilingTypeNone").prop("checked", false);
        }
    });
    $("#chkCeilingTypeNone").change(function () {
        if ($(this).is(":checked")) {
            $("#chkCeilingTypeTBar, #chkCeilingTypeSlabConcrete, #chkCeilingTypeMBar, #chkCeilingTypeSteel").prop("checked", false);
        }
    });
    /* End:: 2015/11/11 Added: tbt_QuotationInstallationDetail */

});
function InitialSaleInfomation() {
    var newBldMgmtFlag = $("#NewBldMgmtFlagNeed").prop("checked");
    var newBldMgmtCost = $("#NewBldMgmtCost").val();

    if ($("#BuildingTypeCode").val() == "1") {
        EnableNewBldMgmtFlag(true);
        if ($("#NewBldMgmtFlagNeed").prop("checked") == true)
            EnableNewBldMgmtCost(true);
        else
            EnableNewBldMgmtCost(false);
    }
    else
        EnableNewBldMgmtFlag(false);

    $("#ProductCode").DisableControl();
    $("#SiteBuildingArea").DisableControl();
    $("#SecurityAreaFrom").DisableControl();
    $("#SecurityAreaTo").DisableControl();
    $("#MainStructureTypeCode").DisableControl();
    $("#BuildingTypeCode").DisableControl();
    $("#NewBldMgmtCost").DisableControl();
    $("#BidGuaranteeAmount1").DisableControl();
    $("#BidGuaranteeAmount2").DisableControl();
    $("#ApproveNo1").DisableControl();
    $("#ApproveNo2").DisableControl();
    $("#ApproveNo3").DisableControl();
    $("#ApproveNo4").DisableControl();
    $("#ApproveNo5").DisableControl();

    DisableRadioControl("SpecialInstallationFlag", ["Yes", "No"]);
    DisableRadioControl("NewBldMgmtFlag", ["Need", "NoNeed"]);

    if ($("#NewBldMgmtFlagNeed").prop("disabled") == true) {
        if (newBldMgmtFlag == true) {
            $("#NewBldMgmtFlagNeed").attr("checked", true);
        } else {
            $("#NewBldMgmtFlagNoNeed").attr("checked", true);
        }
    }
    if ($("#NewBldMgmtCost").prop("readonly") == true)
        $("#NewBldMgmtCost").val(newBldMgmtCost);
}
/* ----------------------------------------------------------------------------------- */

/* --- Events ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function building_type_code_change(istab) {
    var enable = false;
    if (GetDisableFlag("NewBldMgmtFlag") == false &&
        $("#BuildingTypeCode").find("option:selected").val() == "1") {
        enable = true;
    }

    if (GetDisableFlag("NewBldMgmtFlag") == false) {
        EnableNewBldMgmtFlag(enable);

        if (istab == true) {
            if (enable)
                $("#NewBldMgmtFlagNeed").focus();
            else
                $("#SalesmanEmpNo1").focus();
        }
    }
    else if (GetDisableFlag("NewBldMgmtCost") == false) {
        enable = ($("#BuildingTypeCode").find("option:selected").val() == "1");
        EnableNewBldMgmtCost(enable);
    }
}
function new_bld_mgmt_flag_change() {
    var enable = false;
    if (GetDisableFlag("NewBldMgmtCost") == false &&
        $("#NewBldMgmtFlagNeed").prop("checked") == true) {
        enable = true;
    }

    if (GetDisableFlag("NewBldMgmtCost") == false) {
        EnableNewBldMgmtCost(enable);
    }
}
function aLess_click() {
    ShowSaleInformation(false);
    return false;
}
function aMore_click() {
    ShowSaleInformation(true);
    return false;
}
/* ----------------------------------------------------------------------------------- */

/* --- Methods ----------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function EnableNewBldMgmtFlag(enable) {
    if (enable) {
        $("#NewBldMgmtFlagNeed").removeAttr("disabled");
        $("#NewBldMgmtFlagNoNeed").removeAttr("disabled");

        if ($("#NewBldMgmtFlagNeed").prop("checked") != true) {
            $("#NewBldMgmtFlagNoNeed").attr("checked", true);
        }
    }
    else {
        $("#NewBldMgmtFlagNeed").attr("disabled", true);
        $("#NewBldMgmtFlagNoNeed").attr("disabled", true);

        $("#NewBldMgmtFlagNoNeed").attr("checked", true);
        EnableNewBldMgmtCost(false);
    }
}
function EnableNewBldMgmtCost(enable) {
    if (enable) {
        $("#NewBldMgmtCost").ResetToNormalControl();
        $("#NewBldMgmtCost").removeAttr("readonly");
        $("#NewBldMgmtCost").NumericCurrency().ResetToNormalControl();
        $("#NewBldMgmtCost").NumericCurrency().prop("disabled",false);
    }
    else {
        $("#NewBldMgmtCost").attr("readonly", true);
        $("#NewBldMgmtCost").NumericCurrency().prop("disabled", true);
        if (GetDisableFlag("NewBldMgmtCost") == false) {
            $("#NewBldMgmtCost").val(""); 
            $("#NewBldMgmtCost").NumericCurrencyValue(C_CURRENCY_LOCAL);
        }
    }
}
function ShowSaleInformation(show) {
    if (show) {
        $("span[id=aLess]").show();
        $("span[id=aMore]").hide();
        $("div[id=divSaleMore]").show();
    }
    else {
        $("span[id=aLess]").hide();
        $("span[id=aMore]").show();
        $("div[id=divSaleMore]").hide();
    }
}
/* ----------------------------------------------------------------------------------- */

/* --- Output Methods ---------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function GetQUS030_02_SectionData() {
    //var obj = CreateObjectData($("#formQuotationDetailInfo_Sale").serialize());
    var obj = $("#formQuotationDetailInfo_Sale").serializeObject2();

    if ($("#SpecialInstallationFlagYes").prop("checked") == true)
        obj["SpecialInstallationFlag"] = $("#SpecialInstallationFlagYes").val();
    else
        obj["SpecialInstallationFlag"] = $("#SpecialInstallationFlagNo").val();

    obj["ProductCode"] = $("#ProductCode").val();

    var name = $("#ProductCode :selected").text();
    if (name.indexOf(":") >= 0) {
        name = $.trim(name.substring(name.indexOf(":") + 1));
    }
    obj["ProductName"] = name;

    obj["MainStructureTypeCode"] = $("#MainStructureTypeCode").val();
    obj["BuildingTypeCode"] = $("#BuildingTypeCode").val();

    if ($("#NewBldMgmtFlagNeed").prop("checked") == true)
        obj["NewBldMgmtFlag"] = $("#NewBldMgmtFlagNeed").val();
    else
        obj["NewBldMgmtFlag"] = $("#NewBldMgmtFlagNoNeed").val();

    obj.CeilingTypeTBar = (obj.chkCeilingTypeTBar == "on");
    obj.CeilingTypeSlabConcrete = (obj.chkCeilingTypeSlabConcrete == "on");
    obj.CeilingTypeMBar = (obj.chkCeilingTypeMBar == "on");
    obj.CeilingTypeSteel = (obj.chkCeilingTypeSteel == "on");
    obj.CeilingTypeNone = (obj.chkCeilingTypeNone == "on");
    obj.SpecialInsPVC = (obj.chkSpecialInsPVC == "on");
    obj.SpecialInsSLN = (obj.chkSpecialInsSLN == "on");
    obj.SpecialInsProtector = (obj.chkSpecialInsProtector == "on");
    obj.SpecialInsEMT = (obj.chkSpecialInsEMT == "on");
    obj.SpecialInsPE = (obj.chkSpecialInsPE == "on");
    obj.SpecialInsOther = (obj.chkSpecialInsOther == "on");
    obj.SpecialInsOtherText = (obj.chkSpecialInsOther == "on" ? obj.SpecialInsOtherText : null);

    return obj;
}
function SetQUS030_02_SectionMode(isview) {
    if (isview) {
        $("#divQuotationDetailInfo_Sale").SetViewMode(true);

        /* --- Special --- */
        $("span[class='format-date']").hide();
        $("#rdoSpecialInstallationFlag").SetRadioGroupViewMode(true);
        $("#rdoNewBldMgmtFlag").SetRadioGroupViewMode(true);

        SetControlFromToView("SecurityAreaFrom", "SecurityAreaTo", "SecurityAreaFromTo");

        for (var idx = 1; idx <= 10; idx++) {
            if ($("#Saleman" + idx + "Name").val() == "") {
                $("#divSaleman" + idx + "Name").html("");
            }
        }
    }
    else {
        $("#divQuotationDetailInfo_Sale").SetViewMode(false);

        /* --- Special --- */
        $("span[class='format-date']").show();
        $("#divSecurityAreaFromTo").show();
        $("#rdoSpecialInstallationFlag").SetRadioGroupViewMode(false);
        $("#rdoNewBldMgmtFlag").SetRadioGroupViewMode(false);

        if ($("#BuildingTypeCode").val() == "1") {
            EnableNewBldMgmtFlag(true);
            if (GetDisableFlag("NewBldMgmtCost") == false && $("#NewBldMgmtFlagNeed").prop("checked") == true)
                EnableNewBldMgmtCost(true);
            else
                EnableNewBldMgmtCost(false);
        }
        else
            EnableNewBldMgmtFlag(false);

        DisableRadioControl("SpecialInstallationFlag", ["Yes", "No"]);
        DisableRadioControl("NewBldMgmtFlag", ["Need", "NoNeed"]);
    }
}
/* ----------------------------------------------------------------------------------- */

function QUS030_02_ResetToNormalControl() {
    $("#divQuotationDetailInfo_Sale").ResetToNormalControl();
    $("#SiteBuildingArea").val("0.00");
}