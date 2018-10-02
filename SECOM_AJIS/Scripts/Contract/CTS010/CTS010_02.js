/// <reference path="../../Base/GridControl.js" />

/* --- Variable ------------------------------------------------------- */
/* -------------------------------------------------------------------- */
//var gridFeeInformation = null;
/* -------------------------------------------------------------------- */

//function CTS010_02_InitialGrid() {
//    /* --- Initial Grid --- */
//    /* -------------------- */
//    gridFeeInformation = $("#gridFeeInformation").LoadDataToGridWithInitial(0, false, false,
//                                "/Contract/CTS010_GetFeeInformation",
//                                "",
//                                "CTS010_FeeInformation", false);
//    /* -------------------- */

//    SpecialGridControl(gridFeeInformation, ["Normal", "Order", "Approve", "Complete", "Start"]);

//    /* --- Bind Grid events --- */
//    /* ------------------------ */
//    BindOnLoadedEvent(gridFeeInformation, function () {
//        var GenNumericFunction = function (grid, row, col_name, name, require, enable) {
//            var row_id = grid.getRowId(row);
//            var require_space = false;
//            var disabled = GetDisableFlag(name);
//            if (disabled == true) {
//                require = false;
//                enable = false;
//                require_space = true;
//            }

//            var required_flag = "#Required" + name;
//            if ($(required_flag).val() != undefined) {
//                if ($(required_flag).val().toLowerCase() == "false") {
//                    require_space = true;
//                    require = false;
//                }
//            }

//            /* --- Normal --- */
//            var nCol = grid.getColIndexById(col_name);
//            var nVal = GetValueFromLinkType(grid, row, nCol);
//            grid.cells2(row, nCol).setValue(GenerateNumericBoxWithUnit(name, "", nVal, "113px", $("#CurrencyUnit").val(), require, enable, require_space));
//            $("#" + name).BindNumericBox(10, 2, 0, 9999999999.99);
//        }


//        /* --- Contract fee --- */
//        gridFeeInformation.cells2(0, gridFeeInformation.getColIndexById("Name")).setValue($("#lblContractFee").val());
//        GenNumericFunction(gridFeeInformation, 0, "Normal", "NormalContractFee", false, false);
//        GenNumericFunction(gridFeeInformation, 0, "Order", "OrderContractFee", true, true);

//        /* --- Installation fee --- */
//        gridFeeInformation.cells2(1, gridFeeInformation.getColIndexById("Name")).setValue($("#lblInstallationFee").val());
//        GenNumericFunction(gridFeeInformation, 1, "Normal", "NormalInstallFee", false, false);
//        GenNumericFunction(gridFeeInformation, 1, "Order", "OrderInstallFee", true, true);
//        GenNumericFunction(gridFeeInformation, 1, "Approve", "OrderInstallFee_ApproveContract", true, true);
//        GenNumericFunction(gridFeeInformation, 1, "Complete", "OrderInstallFee_CompleteInstall", true, true);
//        GenNumericFunction(gridFeeInformation, 1, "Start", "OrderInstallFee_StartService", true, true);

//        /* --- Contract fee --- */
//        gridFeeInformation.cells2(2, gridFeeInformation.getColIndexById("Name")).setValue($("#lblDepositFee").val());
//        GenNumericFunction(gridFeeInformation, 2, "Normal", "NormalDepositFee", false, false);
//        GenNumericFunction(gridFeeInformation, 2, "Order", "OrderDepositFee", true, true);


//        /* --- Set Events --- */
//        /* ------------------ */
//        $("#OrderContractFee").blur(function () {
//            OrderContractfeeChange(true);
//        });
//        $("#OrderDepositFee").blur(function () {
//            OrderDepositfeeChange();
//        });
//        OrderContractfeeChange(false);
//        OrderDepositfeeChange();
//        /* ------------------ */

//        gridFeeInformation.setSizes();
//    });
//    /* ------------------------ */
//}


function GetCTS010_02_SectionData() {
    var obj = CreateObjectData($("#formFN99Information").serialize());

    var operationType = new Array();
    $("#OperationType").find("input:checkbox").each(function () {
        if ($(this).prop("checked") == true) {
            operationType.push($(this).val());
        }
    });
    obj.OperationType = operationType;

    if ($("#chkCounterBalanceOriginContractCode").prop("checked") == true) {
        obj.IsContractCodeForDepositFeeSlide = true;
        obj.CounterBalanceOriginContractCode = $("#CounterBalanceOriginContractCode").val();
    }

    obj.OrderContractFeeCurrencyType = $("#OrderContractFee").NumericCurrencyValue();
    obj.OrderInstallFeeCurrencyType = $("#OrderInstallFee").NumericCurrencyValue();
    obj.OrderInstallFee_ApproveContractCurrencyType = $("#OrderInstallFee_ApproveContract").NumericCurrencyValue();
    obj.OrderInstallFee_CompleteInstallCurrencyType = $("#OrderInstallFee_CompleteInstall").NumericCurrencyValue();
    obj.OrderInstallFee_StartServiceCurrencyType = $("#OrderInstallFee_StartService").NumericCurrencyValue();
    obj.OrderDepositFeeCurrencyType = $("#OrderDepositFee").NumericCurrencyValue();

    if (obj.OrderContractFeeCurrencyType == C_CURRENCY_US) {
        obj.OrderContractFeeUsd = obj.OrderContractFee;
        obj.OrderContractFee = null;
    }
    if (obj.OrderInstallFeeCurrencyType == C_CURRENCY_US) {
        obj.OrderInstallFeeUsd = obj.OrderInstallFee;
        obj.OrderInstallFee = null;
    }
    if (obj.OrderInstallFee_ApproveContractCurrencyType == C_CURRENCY_US) {
        obj.OrderInstallFee_ApproveContractUsd = obj.OrderInstallFee_ApproveContract;
        obj.OrderInstallFee_ApproveContract = null;
    }
    if (obj.OrderInstallFee_CompleteInstallCurrencyType == C_CURRENCY_US) {
        obj.OrderInstallFee_CompleteInstallUsd = obj.OrderInstallFee_CompleteInstall;
        obj.OrderInstallFee_CompleteInstall = null;
    }
    if (obj.OrderInstallFee_StartServiceCurrencyType == C_CURRENCY_US) {
        obj.OrderInstallFee_StartServiceUsd = obj.OrderInstallFee_StartService;
        obj.OrderInstallFee_StartService = null;
    }
    if (obj.OrderDepositFeeCurrencyType == C_CURRENCY_US) {
        obj.OrderDepositFeeUsd = obj.OrderDepositFee;
        obj.OrderDepositFee = null;
    }

    return obj;
}
function SetCTS010_02_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#formFN99Information").SetViewMode(true);
        $("#chkCounterBalanceOriginContractCode").hide();

        $("#divOrderInstallFeeLine").css({ "margin-right": "-9px" });
        $("#divOrderDepositLine").css({ "margin-right": "-9px" });
    }
    else {
        $("#formFN99Information").SetViewMode(false);
        $("#chkCounterBalanceOriginContractCode").show();

        $("#divOrderInstallFeeLine").css({ "margin-right": "0px" });
        $("#divOrderDepositLine").css({ "margin-right": "0px" });
    }
}
function SetCTS010_02_EnableSection(enable) {
    $("#formFN99Information").SetEnableView(enable);
    $("#ExpectedInstallCompleteDate").EnableDatePicker(enable);
    $("#ExpectedStartServiceDate").EnableDatePicker(enable);

    if (enable) {
        $("#OperationType").DisableCheckListControl();
        $("#ExpectedInstallCompleteDate").DisableDatePickerControl();

        $("#NormalContractFee").attr("readonly", true);
        $("#NormalInstallFee").attr("readonly", true);
        $("#NormalDepositFee").attr("readonly", true);
        $("#LinkageSaleContractCode").attr("readonly", true);
        CounterBalanceOriginContractCodeChange($("#chkCounterBalanceOriginContractCode").prop("checked"));
    }
}




/* --- Initial -------------------------------------------------------- */
/* -------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "CounterBalanceOriginContractCode",
        "ProjectCode",
        "ApproveNo1",
        "ApproveNo2",
        "ApproveNo3",
        "ApproveNo4",
        "ApproveNo5",
        "BICContractCode"
    ]);

    $("#ExpectedInstallCompleteDate").InitialDate();
    $("#ExpectedStartServiceDate").InitialDate();

    $("#chkCounterBalanceOriginContractCode").change(function () {
        CounterBalanceOriginContractCodeChange($("#chkCounterBalanceOriginContractCode").prop("checked"));
    });
    CounterBalanceOriginContractCodeChange($("#CounterBalanceOriginContractCode").val() != "");

    $("#OperationType").DisableCheckListControl();
    $("#ExpectedInstallCompleteDate").DisableDatePickerControl();


    $("#OrderContractFee").BindNumericBox(12, 2, 0, 999999999999.00);
    $("#OrderInstallFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#OrderInstallFee_ApproveContract").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#OrderInstallFee_CompleteInstall").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#OrderInstallFee_StartService").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#OrderDepositFee").BindNumericBox(12, 2, 0, 999999999999.99);

    $("#OrderContractFee").blur(function () {
        OrderContractfeeChange(true);
    });
    $("#OrderDepositFee").RelateControlEvent(OrderDepositfeeChange);
    OrderContractfeeChange(false);
    OrderDepositfeeChange();

    $("#OrderContractFee").NumericCurrency().attr("disabled", true);
    $("#OrderInstallFee").NumericCurrency().attr("disabled", true);
    $("#OrderDepositFee").NumericCurrency().attr("disabled", true);

    if (GetDisableFlag("OrderInstallFee")) {
        $("#OrderInstallFee").attr("readonly", true);
        $("#OrderInstallFee").NumericCurrency().attr("disabled", true);
    }
    if (GetDisableFlag("OrderInstallFee_ApproveContract")) {
        $("#OrderInstallFee_ApproveContract").attr("readonly", true);
        $("#OrderInstallFee_ApproveContract").NumericCurrency().attr("disabled", true);
    }
    if (GetDisableFlag("OrderInstallFee_CompleteInstall")) {
        $("#OrderInstallFee_CompleteInstall").attr("readonly", true);
        $("#OrderInstallFee_CompleteInstall").NumericCurrency().attr("disabled", true);
    }
    if (GetDisableFlag("OrderInstallFee_StartService")) {
        $("#OrderInstallFee_StartService").attr("readonly", true);
        $("#OrderInstallFee_StartService").NumericCurrency().attr("disabled", true);
    }
    if (GetDisableFlag("OrderDepositFee")) {
        $("#OrderDepositFee").attr("readonly", true);
        $("#OrderDepositFee").NumericCurrency().attr("disabled", true);
    }
});
/* -------------------------------------------------------------------- */

function CounterBalanceOriginContractCodeChange(enable) {
    if (enable) {
        $("#chkCounterBalanceOriginContractCode").attr("checked", true);
        $("#CounterBalanceOriginContractCode").removeAttr("readonly");
    }
    else {
        $("#chkCounterBalanceOriginContractCode").removeAttr("checked");
        $("#CounterBalanceOriginContractCode").val("");
        $("#CounterBalanceOriginContractCode").attr("readonly", true);
    }
}
function OrderContractfeeChange() {
    var val = $("#OrderContractFee").NumericValue();
    if (val == 0) {
        $("#divContractPaymentTerm").hide();
        InitialContractPaymentTerm();
        $("#DivideBillingContractFee").removeAttr("checked")
        $("#DivideBillingContractFee").attr("disabled", true);
    }
    else {
        $("#divContractPaymentTerm").show();
        $("#DivideBillingContractFee").removeAttr("disabled");
    }
}
function OrderDepositfeeChange(isFocus) {
    var val = $("#OrderDepositFee").NumericValue();
    if (val == 0) {
        $("#BillingTimingDepositFee").val("");
        $("#BillingTimingDepositFee").attr("disabled", true);

        if (isFocus == true)
            $("#chkCounterBalanceOriginContractCode").focus();
    }
    else {
        if ($("#BillingTimingDepositFee").prop("disabled") == true) {
            $("#BillingTimingDepositFee").removeAttr("disabled");
        }
        if (isFocus == true)
            $("#BillingTimingDepositFee").focus();
    }
}