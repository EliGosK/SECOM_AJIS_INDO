/* --- Initial ----------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "SecurityTypeCode",
        "SaleOnlineContractCode",
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
    $("#NumOfBuilding").BindNumericBox(2, 0, 0, 99);
    $("#NumOfFloor").BindNumericBox(3, 0, 0, 999);
    $("#InsuranceCoverageAmount").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#MonthlyInsuranceFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#MaintenanceFee1").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#AdditionalFee1").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#AdditionalFee2").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#AdditionalFee3").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#AdditionalFee3").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#NewBldMgmtCost").BindNumericBox(12, 2, 0, 999999999999.99);
    GetEmployeeNameData("#SalesmanEmpNo1", "#Saleman1Name");
    GetEmployeeNameData("#SalesmanEmpNo2", "#Saleman2Name");
    GetEmployeeNameData("#SalesSupporterEmpNo", "#SalesSupporterName");
    /* ------------------------ */

    /* --- Initial Data --- */
    /* -------------------- */
    InitialSaleOnlineInfomation();
    /* -------------------- */

    /* --- Initial Events --- */
    /* ---------------------- */
    $("#btnRetrieveContract").click(retrieve_linkage_sale_contract_click);
    $("#btnResetLinkageContract").click(reset_linkage_sale_contract_click);
    $("#chkMaintenanceCycle").change(maintenance_cycle_change);
    /* ---------------------- */
});
function InitialSaleOnlineInfomation() {
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

    if (GetDisableFlag("SaleOnlineContractCode") == true) {
        $("#SaleOnlineContractCode").DisableControl();
        $("#btnRetrieveContract").attr("disabled", true);
        $("#btnResetLinkageContract").attr("disabled", true);
    }

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

    if ($("#MaintenanceCycle").val() == $("#DefaultMaintenanceCycle").val()) {
        /* --- *** --- */
        /* EnableMaintenanceCycle(false); */

        EnableMaintenanceCycle($("#chkMaintenanceCycle").prop("checked"));
    }
}
/* ----------------------------------------------------------------------------------- */

/* --- Events ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function retrieve_linkage_sale_contract_click() {
    var objParam = {
        SaleOnlineContractCode: $("#SaleOnlineContractCode").val(),
        QuotationTargetCode: $("#QuotationTargetCodeShort").val(),
    };
    $("#btnRetrieveContract").attr("disabled", true);
    $("#btnResetLinkageContract").attr("disabled", true);

    call_ajax_method_json("/Quotation/QUS030_RetrieveLinkageSaleContractData", objParam, function (result, controls) {
        $("#btnRetrieveContract").removeAttr("disabled");
        $("#btnResetLinkageContract").removeAttr("disabled");
        
        if (controls != undefined) {
            /* --- Higlight Text --- */
            VaridateCtrl(["SaleOnlineContractCode"], controls);
            return;
        }

        var bSaleCode = $("#SaleOnlineContractCode").val();

        $("#divLinkageSaleContractInformation").clearForm();
        if (result != undefined) {
            $("#btnRetrieveContract").attr("disabled", true);
            $("#SaleOnlineContractCode").attr("readonly", true);
            $("#SaleOnlineContractCode").val(result.ContractCodeShort);

            $("#divLinkageSaleContractInformation").bindJSON(result);

            result.PlanCheckDate = ConvertDateObject(result.PlanCheckDate);
            result.PlanApproveDate = ConvertDateObject(result.PlanApproveDate);

            $("#PlanCheckDate").val(ConvertDateToTextFormat(result.PlanCheckDate));
            $("#PlanApproveDate").val(ConvertDateToTextFormat(result.PlanApproveDate));
            
            $("#SecurityAreaFrom").val(SetNumericValue(result.SecurityAreaFrom,2));
            $("#SecurityAreaFrom").setComma();
            $("#SecurityAreaTo").val(SetNumericValue(result.SecurityAreaTo,2));
            $("#SecurityAreaTo").setComma();
            $("#NewBldMgmtCost").val(SetNumericValue(result.NewBldMgmtCost,2));
            $("#NewBldMgmtCost").setComma();
            $("#SiteBuildingArea").val(SetNumericValue(result.SiteBuildingArea,2));
            $("#SiteBuildingArea").setComma();
            
            if (typeof (QUS030_09_InitialGrid) == "function") {
                if (result.ContractCodeShort != undefined) {
                    $("#InstSaleOnlineContractCode").val(result.ContractCodeShort);
                }
                else {
                    $("#InstSaleOnlineContractCode").val(objParam.SaleOnlineContractCode);
                }

                QUS030_09_InitialGrid(true);
            }
        }
        else {
            $("#SaleOnlineContractCode").val(bSaleCode);
        }
    });
    return false;
}
function reset_linkage_sale_contract_click() {
    $("#divLinkageSaleContractInformation").clearForm();
    $("#btnRetrieveContract").removeAttr("disabled");
    $("#SaleOnlineContractCode").removeAttr("readonly");
    $("#SaleOnlineContractCode").val("");
    $("#InstSaleOnlineContractCode").val("");

    if (typeof (ResetGridInstrumentDetail_ONLINE) == "function") {
        ResetGridInstrumentDetail_ONLINE();
    }
    return false;
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
function GetQUS030_04_SectionData() {
    var obj = CreateObjectData($("#formQuotationDetailInfo_ONLINE").serialize());

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

    return obj;
}
function SetQUS030_04_SectionMode(isview) {
    if (isview) {
        $("#formQuotationDetailInfo_ONLINE").SetViewMode(true);

        /* --- Special --- */
        $("span[class='format-date']").hide();
        $("#rdoSpecialInstallationFlag").SetRadioGroupViewMode(true);
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

        if ($("#PlannerName").val() == "") {
            $("#divPlannerName").html("");
        }
        if ($("#PlanCheckerName").val() == "") {
            $("#divPlanCheckerName").html("");
        }
        if ($("#PlanApproverName").val() == "") {
            $("#divPlanApproverName").html("");
        }
    }
    else {
        $("#formQuotationDetailInfo_ONLINE").SetViewMode(false);

        /* --- Special --- */
        $("span[class='format-date']").show();
        $("#rdoSpecialInstallationFlag").SetRadioGroupViewMode(false);
        $("#chkMaintenanceCycle").show();
        $("#divSecurityAreaFromTo").show();

        $("#OperationType").DisableCheckListControl();
        $("#ServiceType").DisableCheckListControl();
        if (GetDisableFlag("MaintenanceCycle") == true) {
                $("#chkMaintenanceCycle").attr("disabled", true);
        }
    }
}
/* ----------------------------------------------------------------------------------- */

function QUS030_04_ResetToNormalControl() {
    $("#formQuotationDetailInfo_ONLINE").ResetToNormalControl();
}
