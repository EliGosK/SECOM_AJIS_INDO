/// <reference path="../../Base/GridControl.js" />

/* --- Variable ------------------------------------------------------- */
/* -------------------------------------------------------------------- */
var gridFeeInformation = null;
/* -------------------------------------------------------------------- */

/* --- Initial -------------------------------------------------------- */
/* -------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "ProjectCode",
        "ConnectTargetCode",
        "DistributedOriginCode",
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
        "ApproveNo5",
        "BICContractCode"
    ]);

    $("#ExpectedInstallCompleteDate").InitialDate();
    $("#ExpectedAcceptanceAgreeDate").InitialDate();
    $("#BidGuaranteeAmount1").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#BidGuaranteeAmount2").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#SalesOfficeCode").attr("readonly", true);

    //InitialFeeInformationGrid();





    $("#OrderProductPrice").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#BillingAmt_ApproveContract").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#BillingAmt_PartialFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#BillingAmt_Acceptance").BindNumericBox(12, 2, 0, 999999999999.99);

    $("#OrderInstallFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#BillingAmtInstallation_ApproveContract").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#BillingAmtInstallation_PartialFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#BillingAmtInstallation_Acceptance").BindNumericBox(12, 2, 0, 999999999999.99);

    //$("#OrderSalePrice").BindNumericBox(12, 2, 0, 999999999999.99);
    
    var bfunc = function () {
        var id = $(this).attr("id");
        var val = $(this).val();
        var cval = $(this).NumericCurrencyValue();

        setTimeout(function () {
            if (id == "BillingAmt_ApproveContract") {
                $("#SalePrice_Approval").val(val);
                $("#SalePrice_Approval").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmt_PartialFee") {
                $("#SalePrice_Partial").val(val);
                $("#SalePrice_Partial").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmt_Acceptance") {
                $("#SalePrice_Acceptance").val(val);
                $("#SalePrice_Acceptance").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmtInstallation_ApproveContract") {
                $("#InstallationFee_Approval").val(val);
                $("#InstallationFee_Approval").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmtInstallation_PartialFee") {
                $("#InstallationFee_Partial").val(val);
                $("#InstallationFee_Partial").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmtInstallation_Acceptance") {
                $("#InstallationFee_Acceptance").val(val);
                $("#InstallationFee_Acceptance").SetNumericCurrency(cval);
            }

            if (typeof (SummaryBillingFee) == "function") {
                SummaryBillingFee();
            }
        }, 0);
        
    };
    
    $("#BillingAmt_ApproveContract").blur(bfunc);
    $("#BillingAmt_PartialFee").blur(bfunc);
    $("#BillingAmt_Acceptance").blur(bfunc);
    $("#BillingAmtInstallation_ApproveContract").blur(bfunc);
    $("#BillingAmtInstallation_PartialFee").blur(bfunc);
    $("#BillingAmtInstallation_Acceptance").blur(bfunc);

    var bcfunc = function () {
        var id = $(this).attr("id");
        var val = $("#" + id.replace("CurrencyType", "")).val();
        var cval = $(this).val();        
        
        setTimeout(function () {
            if (id == "BillingAmt_ApproveContractCurrencyType") {
                $("#SalePrice_Approval").val(val);
                $("#SalePrice_Approval").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmt_PartialFeeCurrencyType") {
                $("#SalePrice_Partial").val(val);
                $("#SalePrice_Partial").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmt_AcceptanceCurrencyType") {
                $("#SalePrice_Acceptance").val(val);
                $("#SalePrice_Acceptance").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmtInstallation_ApproveContractCurrencyType") {
                $("#InstallationFee_Approval").val(val);
                $("#InstallationFee_Approval").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmtInstallation_PartialFeeCurrencyType") {
                $("#InstallationFee_Partial").val(val);
                $("#InstallationFee_Partial").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmtInstallation_AcceptanceCurrencyType") {
                $("#InstallationFee_Acceptance").val(val);
                $("#InstallationFee_Acceptance").SetNumericCurrency(cval);
            }

            if (typeof (SummaryBillingFee) == "function") {
                SummaryBillingFee();
            }
        }, 0);
    };

    $("#BillingAmt_ApproveContract").NumericCurrency().blur(bcfunc);
    $("#BillingAmt_PartialFee").NumericCurrency().blur(bcfunc);
    $("#BillingAmt_Acceptance").NumericCurrency().blur(bcfunc);
    $("#BillingAmtInstallation_ApproveContract").NumericCurrency().blur(bcfunc);
    $("#BillingAmtInstallation_PartialFee").NumericCurrency().blur(bcfunc);
    $("#BillingAmtInstallation_Acceptance").NumericCurrency().blur(bcfunc);

    $("#chkConnectTargetCode").change(function () {
        connect_online_change($("#chkConnectTargetCode").prop("checked"));
    });
    connect_online_change($("#ConnectTargetCode").val() != "");

    var type = "";
    if ($("#rdoDistributedTarget").val() == $("#DistributedInstallTypeCode").val()) {
        $("#rdoDistributedTarget").attr("checked", true);
        type = $("#rdoDistributedTarget").val();
    }
    else if ($("#rdoDistributedOrigin").val() == $("#DistributedInstallTypeCode").val()) {
        $("#rdoDistributedOrigin").attr("checked", true);
        type = $("#rdoDistributedOrigin").val();
    }
    else if ($("#rdoNotDistribute").val() == $("#DistributedInstallTypeCode").val()) {
        $("#rdoNotDistribute").attr("checked", true);
        type = $("#rdoNotDistribute").val();
    }
    distributed_type_change(type);

    $("#rdoDistributedTarget").change(function () {
        distributed_type_change($(this).val());
    });
    $("#rdoDistributedOrigin").change(function () {
        distributed_type_change($(this).val());
    });
    $("#rdoNotDistribute").change(function () {
        distributed_type_change($(this).val());
    });

    GetEmployeeNameData("#SalesmanEmpNo1", "#SalesmanEmpNameNo1");
    GetEmployeeNameData("#SalesmanEmpNo2", "#SalesmanEmpNameNo2");
    GetEmployeeNameData("#SalesmanEmpNo3", "#SalesmanEmpNameNo3");
    GetEmployeeNameData("#SalesmanEmpNo4", "#SalesmanEmpNameNo4");
    GetEmployeeNameData("#SalesmanEmpNo5", "#SalesmanEmpNameNo5");
    GetEmployeeNameData("#SalesmanEmpNo6", "#SalesmanEmpNameNo6");
    GetEmployeeNameData("#SalesmanEmpNo7", "#SalesmanEmpNameNo7");
    GetEmployeeNameData("#SalesmanEmpNo8", "#SalesmanEmpNameNo8");
    GetEmployeeNameData("#SalesmanEmpNo9", "#SalesmanEmpNameNo9");
    GetEmployeeNameData("#SalesmanEmpNo10", "#SalesmanEmpNameNo10");

    $("span[id=aLess]").click(aLess_click);
    $("span[id=aMore]").click(aMore_click);
    ShowSaleInformation(false);

    $("#OperationOfficeCode").change(function () {
        if ($("#OperationOfficeCode").val() != "") {
            $("#SalesOfficeCode").val($("#OperationOfficeCode option:selected").text());
        }
        else {
            $("#SalesOfficeCode").val("");
        }
    });
    if ($("#OperationOfficeCode").val() != "") {
        $("#SalesOfficeCode").val($("#OperationOfficeCode option:selected").text());
    }
    else {
        $("#SalesOfficeCode").val("");
    }

    $("#OrderProductPrice").NumericCurrency().attr("disabled", true);
    $("#OrderInstallFee").NumericCurrency().attr("disabled", true);

    var useinstallfee = $("#formFQ99Information").data("useinstallfee");
    if (useinstallfee == "0") {

        $("#OrderInstallFee").attr("readonly", true);
        $("#OrderInstallFee").NumericCurrency().attr("disabled", true);
        $("#OrderInstallFee").val("0.00");

        $("#BillingAmtInstallation_ApproveContract").attr("readonly", true);
        $("#BillingAmtInstallation_ApproveContract").NumericCurrency().attr("disabled", true);
        $("#BillingAmtInstallation_ApproveContract").val("0.00");

        $("#BillingAmtInstallation_PartialFee").attr("readonly", true);
        $("#BillingAmtInstallation_PartialFee").NumericCurrency().attr("disabled", true);
        $("#BillingAmtInstallation_PartialFee").val("0.00");

        $("#BillingAmtInstallation_Acceptance").attr("readonly", true);
        $("#BillingAmtInstallation_Acceptance").NumericCurrency().attr("disabled", true);
        $("#BillingAmtInstallation_Acceptance").val("0.00");
    }
});
//function InitialFeeInformationGrid() {
//    /* --- Initial Grid --- */
//    /* -------------------- */
//    gridFeeInformation = $("#gridFeeInformation").LoadDataToGridWithInitial(0, false, false,
//                                "/Contract/CTS020_GetFeeInformation",
//                                "",
//                                "CTS020_FeeInformation", false);
//    /* -------------------- */

//    SpecialGridControl(gridFeeInformation, ["Normal", "Order", "Approve", "Partial", "Acceptance"]);
//    gridFeeInformation.attachEvent("onBeforeSelect", function (id, state) {
//        return false;
//    });

//    /* --- Bind Grid events --- */
//    /* ------------------------ */
//    BindOnLoadedEvent(gridFeeInformation, function () {
//        var GenNumericFunction = function (grid, row, col_name, name, require, enable) {
//            var row_id = grid.getRowId(row);
//            /* --- Normal --- */
//            var nCol = grid.getColIndexById(col_name);
//            var nVal = GetValueFromLinkType(grid, row, nCol);
//            grid.cells2(row, nCol).setValue(GenerateNumericBoxWithUnit(name, "", nVal, "113px", $("#CurrencyUnit").val(), require, enable));
//            $("#" + name).BindNumericBox(10, 2, 0, 9999999999.99);

//            if (col_name == "Approve"
//                || col_name == "Partial"
//                || col_name == "Acceptance") {

//                $("#" + name).blur(function () {
//                    if (col_name == "Approve")
//                        $("#SalePrice_Approval").val($(this).val());
//                    else if (col_name == "Partial")
//                        $("#SalePrice_Partial").val($(this).val());
//                    else if (col_name == "Acceptance")
//                        $("#SalePrice_Acceptance").val($(this).val());

//                    if (typeof (SummaryBillingFee) == "function") {
//                        SummaryBillingFee();
//                    }
//                });
//            }
//        }


//        /* --- Product price --- */
//        gridFeeInformation.cells2(0, gridFeeInformation.getColIndexById("Name")).setValue($("#lblProductPrice").val());
//        GenNumericFunction(gridFeeInformation, 0, "Normal", "NormalProductPrice", false, false);
//        GenNumericFunction(gridFeeInformation, 0, "Order", "OrderProductPrice", true, true);

//        /* --- Installation fee --- */
//        gridFeeInformation.cells2(1, gridFeeInformation.getColIndexById("Name")).setValue($("#lblInstallationFee").val());
//        GenNumericFunction(gridFeeInformation, 1, "Normal", "NormalInstallFee", false, false);
//        GenNumericFunction(gridFeeInformation, 1, "Order", "OrderInstallFee", true, true);

//        /* --- Sale price --- */
//        gridFeeInformation.cells2(2, gridFeeInformation.getColIndexById("Name")).setValue($("#lblSalePrice").val());
//        GenNumericFunction(gridFeeInformation, 2, "Normal", "NormalSalePrice", false, false);
//        GenNumericFunction(gridFeeInformation, 2, "Order", "OrderSalePrice", true, true);
//        GenNumericFunction(gridFeeInformation, 2, "Approve", "BillingAmt_ApproveContract", false, true);
//        GenNumericFunction(gridFeeInformation, 2, "Partial", "BillingAmt_PartialFee", false, true);
//        GenNumericFunction(gridFeeInformation, 2, "Acceptance", "BillingAmt_Acceptance", false, true);

//        gridFeeInformation.setSizes();
//    });
//    /* ------------------------ */
//}
/* -------------------------------------------------------------------- */

function connect_online_change(enable) {
    if (enable) {
        $("#chkConnectTargetCode").attr("checked", true);
        $("#ConnectTargetCode").removeAttr("readonly");
        $("#ConnectTargetCode").ResetToNormalControl();
    }
    else {
        $("#chkConnectTargetCode").removeAttr("checked");
        $("#ConnectTargetCode").val("");
        $("#ConnectTargetCode").attr("readonly", true);
    }
}
function distributed_type_change(type) {
    if (type == $("#C_DISTRIBUTE_TYPE_TARGET").val()) {
        $("#DistributedOriginCode").removeAttr("readonly");
    }
    else {
        $("#DistributedOriginCode").val("");
        $("#DistributedOriginCode").attr("readonly", true);
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






function GetCTS020_03_SectionData() {
    var obj = CreateObjectData($("#formFQ99Information").serialize());

    // Begin add: by Jirawat Jannet on 2016 -12-02

    obj.NormalProductPriceCurrencyType = $('#NormalProductPrice').NumericCurrencyValue();
    obj.NormalInstallFeeCurrencyType = $('#NormalInstallFee').NumericCurrencyValue();
    obj.OrderProductPriceCurrencyType = $('#OrderProductPrice').NumericCurrencyValue();
    obj.OrderInstallFeeCurrencyType = $('#OrderInstallFee').NumericCurrencyValue();


    // End add

    if (obj.NormalProductPriceCurrencyType == C_CURRENCY_US) {
        obj.NormalProductPriceUsd = obj.NormalProductPrice;
        obj.NormalProductPrice = null;
    }
    else
    {
        obj.NormalProductPriceUsd = null;
        obj.NormalProductPrice = obj.NormalProductPrice;
    }

    if (obj.OrderProductPriceCurrencyType == C_CURRENCY_US) {
        obj.OrderProductPriceUsd = obj.OrderProductPrice;
        obj.OrderProductPrice = null;
    }
    else
    {
        obj.OrderProductPriceUsd = null;
        obj.OrderProductPrice = obj.OrderProductPrice;
    }

    if (obj.BillingAmt_ApproveContractCurrencyType == C_CURRENCY_US) {
        obj.BillingAmt_ApproveContractUsd = obj.BillingAmt_ApproveContract;
        obj.BillingAmt_ApproveContract = null;
    }
    else
    {
        obj.BillingAmt_ApproveContractUsd = null;
        obj.BillingAmt_ApproveContract = obj.BillingAmt_ApproveContract;
    }

    if (obj.BillingAmt_PartialFeeCurrencyType == C_CURRENCY_US) {
        obj.BillingAmt_PartialFeeUsd = obj.BillingAmt_PartialFee;
        obj.BillingAmt_PartialFee = null;
    }
    else
    {
        obj.BillingAmt_PartialFeeUsd = null;
        obj.BillingAmt_PartialFee = obj.BillingAmt_PartialFee;
    }

    if (obj.BillingAmt_AcceptanceCurrencyType == C_CURRENCY_US) {
        obj.BillingAmt_AcceptanceUsd = obj.BillingAmt_Acceptance;
        obj.BillingAmt_Acceptance = null;
    }
    else
    {
        obj.BillingAmt_AcceptanceUsd = null;
        obj.BillingAmt_Acceptance = obj.BillingAmt_Acceptance;
    }

    if (obj.NormalInstallFeeCurrencyType == C_CURRENCY_US) {
        obj.NormalInstallFeeUsd = obj.NormalInstallFee;
        obj.NormalInstallFee = null;
    }
    else
    {
        obj.NormalInstallFeeUsd = null;
        obj.NormalInstallFee = obj.NormalInstallFee;
    }

    if (obj.OrderInstallFeeCurrencyType == C_CURRENCY_US) {
        obj.OrderInstallFeeUsd = obj.OrderInstallFee;
        obj.OrderInstallFee = null;
    }
    else
    {
        obj.OrderInstallFeeUsd = null;
        obj.OrderInstallFee = obj.OrderInstallFee;
    }

    if (obj.BillingAmtInstallation_ApproveContractCurrencyType == C_CURRENCY_US) {
        obj.BillingAmtInstallation_ApproveContractUsd = obj.BillingAmtInstallation_ApproveContract;
        obj.BillingAmtInstallation_ApproveContract = null;
    }
    else
    {
        obj.BillingAmtInstallation_ApproveContractUsd = null;
        obj.BillingAmtInstallation_ApproveContract = obj.BillingAmtInstallation_ApproveContract;
    }

    if (obj.BillingAmtInstallation_PartialFeeCurrencyType == C_CURRENCY_US) {
        obj.BillingAmtInstallation_PartialFeeUsd = obj.BillingAmtInstallation_PartialFee;
        obj.BillingAmtInstallation_PartialFee = null;
    }
    else
    {
        obj.BillingAmtInstallation_PartialFeeUsd = null;
        obj.BillingAmtInstallation_PartialFee = obj.BillingAmtInstallation_PartialFee;
    }

    if (obj.BillingAmtInstallation_AcceptanceCurrencyType == C_CURRENCY_US) {
        obj.BillingAmtInstallation_AcceptanceUsd = obj.BillingAmtInstallation_Acceptance;
        obj.BillingAmtInstallation_Acceptance = null;
    }
    else
    {
        obj.BillingAmtInstallation_AcceptanceUsd = null;
        obj.BillingAmtInstallation_Acceptance = obj.BillingAmtInstallation_Acceptance;
    }


    if ($("#chkConnectTargetCode").prop("checked") == true) {
        obj.IsConnectTargetCode = true;
        obj.ConnectTargetCode = $("#ConnectTargetCode").val();
    }

    var type = "";
    if ($("#rdoDistributedTarget").prop("checked") == true) {
        type = $("#rdoDistributedTarget").val();
    }
    else if ($("#rdoDistributedOrigin").prop("checked") == true) {
        type = $("#rdoDistributedOrigin").val();
    }
    else if ($("#rdoNotDistribute").prop("checked") == true) {
        type = $("#rdoNotDistribute").val();
    }
    obj.DistributedInstallTypeCode = type;

    if (obj.BidGuaranteeAmount1CurrencyType == C_CURRENCY_US) {
        obj.BidGuaranteeAmount1Usd = obj.BidGuaranteeAmount1;
        obj.BidGuaranteeAmount1 = null;
    }
    if (obj.BidGuaranteeAmount2CurrencyType == C_CURRENCY_US) {
        obj.BidGuaranteeAmount2Usd = obj.BidGuaranteeAmount2;
        obj.BidGuaranteeAmount2 = null;
    }

    return obj;
}
function SetCTS020_03_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#formFQ99Information").SetViewMode(true);
        $("#chkConnectTargetCode").hide();
        $("#divDistributedGroup").SetRadioGroupViewMode(true);

        for (var idx = 1; idx <= 10; idx++) {
            if ($("#SalesmanEmpNameNo" + idx).val() == "") {
                $("#divSalesmanEmpNameNo" + idx).html("");
            }
        }
        if ($("#SalesSupporterEmpName").val() == "") {
            $("#divSalesSupporterEmpName").html("");
        }
    }
    else {
        $("#formFQ99Information").SetViewMode(false);
        $("#chkConnectTargetCode").show();
        $("#divDistributedGroup").SetRadioGroupViewMode(false);
    }
}
function SetCTS020_03_EnableSection(enable) {
    $("#formFQ99Information").SetEnableView(enable);
    $("#ExpectedInstallCompleteDate").EnableDatePicker(enable);
    $("#ExpectedAcceptanceAgreeDate").EnableDatePicker(enable);

    if (enable) {
        $("#NormalProductPrice").attr("readonly", true);
        $("#NormalInstallFee").attr("readonly", true);
        //$("#NormalSalePrice").attr("readonly", true);

        connect_online_change($("#chkConnectTargetCode").prop("checked"));

        var type = "";
        if ($("#rdoDistributedTarget").prop("checked") == true) {
            type = $("#rdoDistributedTarget").val();
        }
        else if ($("#rdoDistributedOrigin").prop("checked") == true) {
            type = $("#rdoDistributedOrigin").val();
        }
        else if ($("#rdoNotDistribute").prop("checked") == true) {
            type = $("#rdoNotDistribute").val();
        }
        distributed_type_change(type);


        $("#LinkageSaleContractCode").attr("readonly", true);
        CounterBalanceOriginContractCodeChange($("#chkCounterBalanceOriginContractCode").prop("checked"));

        $("#SalesmanEmpNameNo1").attr("readonly", true);
        $("#SalesmanEmpNameNo2").attr("readonly", true);
        $("#SalesmanEmpNameNo3").attr("readonly", true);
        $("#SalesmanEmpNameNo4").attr("readonly", true);
        $("#SalesmanEmpNameNo5").attr("readonly", true);
        $("#SalesmanEmpNameNo6").attr("readonly", true);
        $("#SalesmanEmpNameNo7").attr("readonly", true);
        $("#SalesmanEmpNameNo8").attr("readonly", true);
        $("#SalesmanEmpNameNo9").attr("readonly", true);
        $("#SalesmanEmpNameNo10").attr("readonly", true);
    }
}
