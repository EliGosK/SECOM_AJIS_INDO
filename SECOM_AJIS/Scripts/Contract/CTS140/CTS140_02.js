var CTS140_02 = {
    InitialControl: function () {
        $("#StartDealDate").InitialDate();
        InitialDateFromToControl("#ContractStartDate", "#ContractEndDate");

        $("#LastOrderContractFee").BindNumericBox(10, 2, 0, 9999999999.99);
        $("#ContractDurationMonth").BindNumericBox(3, 0, 0, 999);
        $("#AutoRenewMonth").BindNumericBox(3, 0, 0, 999);

        //SetDateFromToData("#ContractStartDate", "#ContractEndDate", $("#ContractEndDate").val(), $("#ContractStartDate").val());
        SetDateFromToData("#ContractStartDate", "#ContractEndDate", $("#ContractStartDate").val(), $("#ContractEndDate").val()); //Modify by Jutarat A. on 22032013

        $("#FirstSecurityStartDate").InitialDate(); //Add by Jutarat A. on 18102013
    },
    SetSectionMode: function (isView) {
        $("#divContractRelation").SetViewMode(isView);
    },
    DisabledSection: function (isDisabled) {
        $("#divContractRelation").SetEnableView(!isDisabled);
    }
}





//function BindDOContractRelateInformationDateField() {
//    call_ajax_method_json('/Contract/BindDOContractRelateInformation_CTS140', "",
//    function (result, controls) {
//        if (result != undefined) {
//            $("#LastOrderContractFee").val(SetNumericValue(result.LastOrderContractFee, 2));
//            $("#LastOrderContractFee").setComma();

//            $("#LastChangeType").val(result.LastChangeTypeCodeName);
//            $("#ContractDurationMonth").val(result.ContractDurationMonth);
//            $("#AutoRenewMonth").val(result.AutoRenewMonth);
//            $("#LastOCC").val(result.LastOCC);
//            $("#ProjectCode").val(result.ProjectCode);

//            result.StartDealDate = ConvertDateObject(result.StartDealDate);
//            $("#StartDealDate").val(ConvertDateToTextFormat(result.StartDealDate));

//            result.FirstSecurityStartDate = ConvertDateObject(result.FirstSecurityStartDate);
//            $("#FirstSecurityStartDate").val(ConvertDateToTextFormat(result.FirstSecurityStartDate));

//            if (result.ContractStartDate != undefined) {
//                result.ContractStartDate = new Date(ConvertDateObject(result.ContractStartDate));
//                //$("#ContractStartDate").val(ConvertDateToTextFormat(result.ContractStartDate));
//            }
//            if (result.ContractEndDate != undefined) {
//                result.ContractEndDate = new Date(ConvertDateObject(result.ContractEndDate));
//                //$("#ContractEndDate").val(ConvertDateToTextFormat(result.ContractEndDate));
//            }

//            SetDateFromToData("#ContractStartDate", "#ContractEndDate", result.ContractStartDate, result.ContractEndDate);

//            result.LastChangeImplementDate = ConvertDateObject(result.LastChangeImplementDate);
//            $("#LastChangeImplementDate").val(ConvertDateToTextFormat(result.LastChangeImplementDate));
//        }
//    });
//}
