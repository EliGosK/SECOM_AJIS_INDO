var CTS140_04 = {
    InitialControl: function () {
        $("#NormalDepositFee").BindNumericBox(12, 2, 0, 999999999999.99);
        $("#OrderDepositFee").BindNumericBox(12, 2, 0, 999999999999.99);
        $("#ExemptedDepositFee").BindNumericBox(12, 2, 0, 999999999999.99);

        $("#NormalDepositFee").blur(CTS140_04.CalculateExemptDepositFee);
        $("#OrderDepositFee").blur(CTS140_04.CalculateExemptDepositFee);

        $("#NormalDepositFeeCurrencyType").change(CTS140_04.SetExemptedDepositFeeCurrencyType);

        $("#NormalDepositFeeCurrencyType").change(CTS140_04.CheckCurrencyType);
        $("#OrderDepositFeeCurrencyType").change(CTS140_04.CheckCurrencyType);

    },
    SetSectionMode: function (isView) {
        $("#divDepositInformation").SetViewMode(isView);
    },
    DisabledSection: function (isDisabled) {
        $("#divDepositInformation").SetEnableView(!isDisabled);
    },

    CalculateExemptDepositFee: function () {
        var normal = $("#NormalDepositFee").NumericValue();
        var order = $("#OrderDepositFee").NumericValue();
        var exempt = normal - order;
        $("#ExemptedDepositFee").val(SetNumericValue(exempt, 2));
        $("#ExemptedDepositFee").setComma();
    },
    SetExemptedDepositFeeCurrencyType: function () {
        $("#ExemptedDepositFee").SetNumericCurrency($("#NormalDepositFee").NumericCurrencyValue());
    },
    CheckCurrencyType: function () {
        if ($("#NormalDepositFee").NumericCurrencyValue() != $("#OrderDepositFee").NumericCurrencyValue()) {
            $("#ExemptedDepositFee").val("-");
        }
        else
        {
            CTS140_04.CalculateExemptDepositFee();
        }
    }
}


//function BindDODepositInformation() {
//    call_ajax_method_json('/Contract/BindDODepositInformation_CTS140', "",
//            function (result, controls) {
//                if (result != undefined) {
//                    $("#NormalDepositFee").val(result.NormalDepositFee);
//                    $("#OrderDepositFee").val(result.OrderDepositFee);
//                    $("#ExemptedDepositFee").val(result.ExemptedDepositFee);
//                    //$("#CounterBalanceOriginContractCode").val(result.CounterBalanceOriginContractCode);
//                    $("#CounterBalanceOriginContractCode").val(result.CounterBalanceOriginContractCodeShort);
//                }
//            });
//}

