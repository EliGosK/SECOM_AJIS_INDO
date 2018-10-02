var CTS140_05 = {
    InitialControl: function () {
    },
    SetSectionMode: function (isView) {
        $("#divContractDocumentInformation").SetViewMode(isView);
    },
    DisabledSection: function (isDisabled) {
        $("#divContractDocumentInformation").SetEnableView(!isDisabled);
    }
}

//function BindDOContractDocumentInformation() {
//    call_ajax_method_json('/Contract/BindDOContractDocumentInformation_CTS140', "",
//            function (result, controls) {
//                if (result != undefined) {
//                    $("#IrregurationDocUsageFlag").attr('checked', result.IrregurationDocUsageFlag);

//                    //                    $("#PODocAuditResult").val(result.PODocAuditResult);
//                    //                    $("#ContractDocAuditResult").val(result.ContractDocAuditResult);
//                    //                    $("#StartMemoAuditResult").val(result.StartMemoAuditResult);
//                    $("#PODocAuditResult").val(result.PODocAuditResultCodeName);
//                    $("#ContractDocAuditResult").val(result.ContractDocAuditResultCodeName);
//                    $("#StartMemoAuditResult").val(result.StartMemoAuditResultCodeName);

//                    result.PODocReceiveDate = ConvertDateObject(result.PODocReceiveDate);
//                    $("#PODocReceiveDate").val(ConvertDateToTextFormat(result.PODocReceiveDate));

//                    result.ContractDocReceiveDate = ConvertDateObject(result.ContractDocReceiveDate);
//                    $("#ContractDocReceiveDate").val(ConvertDateToTextFormat(result.ContractDocReceiveDate));

//                    result.StartMemoReceiveDate = ConvertDateObject(result.StartMemoReceiveDate);
//                    $("#StartMemoReceiveDate").val(ConvertDateToTextFormat(result.StartMemoReceiveDate));
//                }
//            });
//}
