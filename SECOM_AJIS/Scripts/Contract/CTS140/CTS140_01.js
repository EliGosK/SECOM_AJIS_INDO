var CTS140_01 = {
    InitialControl: function () {
        $("#chkIsimportantCustomer").attr("readonly", true);
        $("#chkIsimportantCustomer").attr("disabled", true);

        $("#OperationOffice").val($("#OperationOfficeValue").val()); //Add by Jutarat A. on 15082012
    },
    SetSectionMode: function (isView) {
        $("#divContractBasicSection").SetViewMode(isView);
        if (isView == false) {
            $("#ImportantFlag").attr("disabled", true);
        }
    },
    DisabledSection: function (isDisabled) {
        $("#divContractBasicSection").SetEnableView(!isDisabled);

        if (isDisabled == true) {
            $("#OperationOffice").val($("#OperationOfficeValue").val());
        }
    }
}
