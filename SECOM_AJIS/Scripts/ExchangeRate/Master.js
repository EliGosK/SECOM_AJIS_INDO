jQuery(function () {
    var loadCurrentExchangeRate = function () {
        call_ajax_method("/ExchangeRate/GetCurrentExchangeRate", "", function (data) {
            if (data.is_today) {
                $('#btnCurrentRateEdit').show();
            } else {
                $('#btnCurrentRateEdit').hide(); 
            }
            $('#EffectiveDate').text(data.target_date.replace("-", "/").replace("-", "/"));
            $('#currentBankRate').text(data.bank_rate);
            $('#currentTaxRate').text(data.tax_rate);
            $('#currentBankRate').setComma();
            $('#currentTaxRate').setComma();
        });
    };
    $(document).ready(loadCurrentExchangeRate);
    
    $('#btnCurrentRateEdit').click(function () {
        $('#dlgBox').OpenInputRateDialog(new Date);
    });
});
