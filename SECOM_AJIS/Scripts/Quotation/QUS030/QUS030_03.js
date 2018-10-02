/* --- Initial ----------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "SecurityTypeCode",
        "PlanCode",
        "PlannerEmpNo",
        "PlanCheckerEmpNo",
        "PlanApproverEmpNo",
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
    $("#PlanCheckDate").InitialDate();
    $("#PlanApproveDate").InitialDate();

    $("#SiteBuildingArea").BindNumericBox(5, 2, 0, 99999.99);
    $("#SecurityAreaFrom").BindNumericBox(5, 2, 0, 99999.99);
    $("#SecurityAreaTo").BindNumericBox(5, 2, 0, 99999.99);
    $("#NewBldMgmtCost").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#InsuranceCoverageAmount").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#MonthlyInsuranceFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#MaintenanceFee1").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#AdditionalFee1").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#AdditionalFee2").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#AdditionalFee3").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#NumOfBuilding").BindNumericBox(2, 0, 0, 99);
    $("#NumOfFloor").BindNumericBox(3, 0, 0, 999);

    GetEmployeeNameData("#PlannerEmpNo", "#PlannerName");
    GetEmployeeNameData("#PlanCheckerEmpNo", "#PlanCheckerName");
    GetEmployeeNameData("#PlanApproverEmpNo", "#PlanApproverName");
    GetEmployeeNameData("#SalesmanEmpNo1", "#Saleman1Name");
    GetEmployeeNameData("#SalesmanEmpNo2", "#Saleman2Name");
    GetEmployeeNameData("#SalesSupporterEmpNo", "#SalesSupporterName");
    /* ------------------------ */

    /* --- Initial Data --- */
    /* -------------------- */
    InitialALInfomation();
    /* -------------------- */

    /* --- Initial Events --- */
    /* ---------------------- */
    $("#BuildingTypeCode").RelateControlEvent(building_type_code_change);
    $("#NewBldMgmtFlagNeed").change(new_bld_mgmt_flag_change);
    $("#NewBldMgmtFlagNoNeed").change(new_bld_mgmt_flag_change);
    $("#chkMaintenanceCycle").change(maintenance_cycle_change);
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
function InitialALInfomation() {
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
    $("#SecurityTypeCode").DisableControl();
    $("#DispatchTypeCode").DisableControl();
    $("#OperationType").DisableCheckListControl();
    $("#PhoneLineTypeCode1").DisableControl();
    $("#PhoneLineTypeCode2").DisableControl();
    $("#PhoneLineTypeCode3").DisableControl();
    $("#PhoneLineOwnerTypeCode1").DisableControl();
    $("#PhoneLineOwnerTypeCode2").DisableControl();
    $("#PhoneLineOwnerTypeCode3").DisableControl();
    $("#ServiceType").DisableCheckListControl();
    $("#SiteBuildingArea").DisableControl();
    $("#SecurityAreaFrom").DisableControl();
    $("#SecurityAreaTo").DisableControl();
    $("#MainStructureTypeCode").DisableControl();
    $("#BuildingTypeCode").DisableControl();
    $("#NewBldMgmtCost").DisableControl();
    $("#NumOfBuilding").DisableControl();
    $("#NumOfFloor").DisableControl();

    $("#MaintenanceCycle").DisableControl();
    if (GetDisableFlag("MaintenanceCycle") == true) {
        $("#chkMaintenanceCycle").attr("disabled", true);
    }

    $("#InsuranceTypeCode").DisableControl();
    $("#InsuranceCoverageAmount").DisableControl();
    $("#MonthlyInsuranceFee").DisableControl();
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

    if ($("#MaintenanceCycle").val() == $("#DefaultMaintenanceCycle").val()) {
        /* --- *** --- */
        /* EnableMaintenanceCycle(false); */

        EnableMaintenanceCycle($("#chkMaintenanceCycle").prop("checked"));
    }
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
                $("#NumOfBuilding").focus();
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
function maintenance_cycle_change() {
    var enable = false;
    if ($("#chkMaintenanceCycle").prop("checked") == true) {
        enable = true;
    }
    EnableMaintenanceCycle(enable);
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
        $("#NewBldMgmtCost").NumericCurrency().prop("disabled", false);
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
function EnableMaintenanceCycle(enable) {
    if (enable) {
        $("#MaintenanceCycle").removeAttr("disabled");
    }
    else {
        $("#MaintenanceCycle").attr("disabled", true);
        $("#MaintenanceCycle").find("option").each(function () {
            if ($(this).val() == $("#DefaultMaintenanceCycle").val())
                $(this).attr("selected", true);
        });
    }
}
/* ----------------------------------------------------------------------------------- */

/* --- Output Methods ---------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function GetQUS030_03_SectionData() {
    //var obj = CreateObjectData($("#formQuotationDetailInfo_AL").serialize());
    var obj = $("#formQuotationDetailInfo_AL").serializeObject2();

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

    obj["DispatchTypeCode"] = $("#DispatchTypeCode").val();
    obj["PhoneLineTypeCode1"] = $("#PhoneLineTypeCode1").val();
    obj["PhoneLineTypeCode2"] = $("#PhoneLineTypeCode2").val();
    obj["PhoneLineTypeCode3"] = $("#PhoneLineTypeCode3").val();
    obj["PhoneLineOwnerTypeCode1"] = $("#PhoneLineOwnerTypeCode1").val();
    obj["PhoneLineOwnerTypeCode2"] = $("#PhoneLineOwnerTypeCode2").val();
    obj["PhoneLineOwnerTypeCode3"] = $("#PhoneLineOwnerTypeCode3").val();

    obj["MainStructureTypeCode"] = $("#MainStructureTypeCode").val();
    obj["BuildingTypeCode"] = $("#BuildingTypeCode").val();
    obj["MaintenanceCycle"] = $("#MaintenanceCycle").val();
    obj["InsuranceTypeCode"] = $("#InsuranceTypeCode").val();

    var operationType = new Array();
    $("#OperationType").find("input:checkbox").each(function () {
        if ($(this).prop("checked") == true) {
            operationType.push($(this).val());
        }
    });
    obj["OperationType"] = operationType;

    var serviceType = new Array();
    $("#ServiceType").find("input:checkbox").each(function () {
        if ($(this).prop("checked") == true) {
            serviceType.push($(this).val());
        }
    });
    obj["ServiceType"] = serviceType;

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
function SetQUS030_03_SectionMode(isview) {
    if (isview) {
        $("#formQuotationDetailInfo_AL").SetViewMode(true);

        /* --- Special --- */
        $("span[class='format-date']").hide();
        $("#rdoSpecialInstallationFlag").SetRadioGroupViewMode(true);
        $("#rdoNewBldMgmtFlag").SetRadioGroupViewMode(true);
        $("#chkMaintenanceCycle").hide();

        SetControlFromToView("SecurityAreaFrom", "SecurityAreaTo", "SecurityAreaFromTo");

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
        $("#formQuotationDetailInfo_AL").SetViewMode(false);

        /* --- Special --- */
        $("span[class='format-date']").show();
        $("#rdoSpecialInstallationFlag").SetRadioGroupViewMode(false);
        $("#rdoNewBldMgmtFlag").SetRadioGroupViewMode(false);
        $("#chkMaintenanceCycle").show();
        $("#divSecurityAreaFromTo").show();

        $("#OperationType").DisableCheckListControl();
        $("#ServiceType").DisableCheckListControl();

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

        if (GetDisableFlag("MaintenanceCycle") == true) {
            $("#chkMaintenanceCycle").attr("disabled", true);
        }
    }
}
/* ----------------------------------------------------------------------------------- */

function QUS030_03_ResetToNormalControl() {
    $("#formQuotationDetailInfo_AL").ResetToNormalControl();
}