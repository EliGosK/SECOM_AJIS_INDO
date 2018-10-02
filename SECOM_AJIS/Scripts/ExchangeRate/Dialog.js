var monthEnglishNameHash = {
    '1': 'Jan',
    '2': 'Feb',
    '3': 'Mar',
    '4': 'Apr',
    '5': 'May',
    '6': 'Jun',
    '7': 'Jul',
    '8': 'Aug',
    '9': 'Sep',
    '10': 'Oct',
    '11': 'Nov',
    '12': 'Dec'
};
$.fn.OpenInputRateDialog = function (targetDate) {
    var ctrl = $(this);
    var isPast = false;
    var currentDate = new Date;
    if (currentDate < targetDate) {
        return;
    }
    //if (targetDate < new Date(currentDate.getYear() + 1900, currentDate.getMonth(), currentDate.getDate(), 0, 0, 0, 0)) {
    //    isPast = true;
    //}
    var targetDateString = targetDate.getDate() + '/' + monthEnglishNameHash[targetDate.getMonth() + 1] + '/' + (targetDate.getYear() + 1900);
    var params = { targetDate: targetDateString };

    ajax_method.CallPopupScreenControllerWithAuthority("", "/ExchangeRate/Detail", params, function (result) {
        if (result != undefined) {
            var event = {
                Cancel: function () {
                    ctrl.CloseDialog();
                }
            };

            var open_event = function (event, ui) {
                // Dialog init event
                $('#BankRateToUSD').val('');
                $('#BankRateToIRP').val('');
                $('#TaxRateToUSD').val('');
                $('#TaxRateToIRP').val('');

                $("#BankRateToIRP").BindNumericBox(9, 3, 0, 99999.999);
                $("#TaxRateToIRP").BindNumericBox(9, 3, 0, 99999.999);
                $("#BankRateToUSD").BindNumericBox(11, 10, 0, 9.9999999999);
                $("#TaxRateToUSD").BindNumericBox(11, 10, 0, 9.9999999999);

                $('#MaintainTargetEffectiveDate').text(params.targetDate);
                call_ajax_method("/ExchangeRate/GetExchangeRateByTargetDate", params, function (data) {
                    $('#BankRateToIRP').val(data.BankRateRupiahPerDollar);
                    $('#TaxRateToIRP').val(data.TaxRateRupiahPerDollar);
                    $('#BankRateToIRP').setComma()
                    $('#TaxRateToIRP').setComma()
                    calculateRate($('#BankRateFromUSD').val(), $('#BankRateToIRP').val(), $('#BankRateToUSD'));
                    calculateRate($('#TaxRateFromUSD').val(), $('#TaxRateToIRP').val(), $('#TaxRateToUSD'));
                });

                if (isPast) {
                    $("#BankRateToIRP").attr('disabled', 'disabled');
                    $("#TaxRateToIRP").attr('disabled', 'disabled');
                    return;
                }

                // Setting UI
                // Register button setting
                $('.ui-dialog-buttonset').prepend(
                    "<button type='button' id='register_rate' class='ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' role='button'>" +
                    "<span class='ui-button-text' style='color: rgb(0, 0, 0); padding: 0px;'>Register</span>" +
                    "</button>");

                $('#register_rate').mouseover(function () {
                    $(this).addClass("ui-state-hover");
                });
                $('#register_rate').mouseout(function () {
                    $(this).removeClass("ui-state-hover ui-state-active");
                });

                $('#register_rate').mousedown(function () {
                    $(this).addClass("ui-state-active");
                });
                $('#register_rate').mouseup(function () {
                    $(this).removeClass("ui-state-active");
                });
                
                // Setting event
                $('#BankRateToIRP').change(function () {
                    calculateRate($('#BankRateFromUSD').val(), $('#BankRateToIRP').val(), $('#BankRateToUSD'));
                });

                $('#TaxRateToIRP').change(function () {
                    calculateRate($('#TaxRateFromUSD').val(), $('#TaxRateToIRP').val(), $('#TaxRateToUSD'));
                });
                                
                $('#register_rate').click(function () {
                    var params = {
                        targetDate: $.trim($('#MaintainTargetEffectiveDate').text()),
                        bankRateRupiah: $('#BankRateToIRP').val(),
                        taxRateRupiah: $('#TaxRateToIRP').val(),
                        bankRateDollar: $('#BankRateToUSD').val(),
                        taxRateDollar: $('#TaxRateToUSD').val()
                    }
                    
                    call_ajax_method("/ExchangeRate/RegisterRate", params, function () {
                        ctrl.CloseDialog();
                        OpenInformationMessageDialog("SaveRate", "Save completely");
                        $("#btnCalendarReload").click();
                        loadCurrentExchangeRate();
                    });
                });
            };

            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);
        }

    });
    
    calculateRate = function (currencyFrom, currencyTo, displayArea) {
        if (currencyTo.length > 0) {
            var convertedValue = 1 / currencyTo.replace(",", "");
            //displayArea.val(Math.round(convertedValue * 1000000000) / 1000000000);
            displayArea.val(convertedValue.toFixed(9));
        } else {
            displayArea.val('');
        }
    };
    
    loadCurrentExchangeRate = function () {
        call_ajax_method("/ExchangeRate/GetCurrentExchangeRate", "", function (data) {
            if (data.is_today) {
                $('#btnCurrentRateEdit').show();
            } else {
                $('#btnCurrentRateEdit').hide();
            }
            $('#EffectiveDate').text(data.target_date.replace("-", "/").replace("-", "/"));
            $('#currentBankRate').text(data.bank_rate);
            $('#currentTaxRate').text(data.tax_rate);
        });
    };
}