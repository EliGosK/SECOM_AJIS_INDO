var CTS140_03 = {
    InitialControl: function () {
        CTS140_03.AcquisitionTypeChange();

        $('#AcquisitionTypeCode').change(function () {
            $("#IntroducerCode").ResetToNormalControl();
            CTS140_03.AcquisitionTypeChange();
        });

        GetEmployeeNameData("#SalesmanEmpNo1", "#SalesmanEmpName1");
        GetEmployeeNameData("#SalesmanEmpNo2", "#SalesmanEmpName2");
        GetEmployeeNameData("#SalesSupporterEmpNo", "#SalesSupporterEmpName");

        $('#PaymentDateIncentive').InitialDate();
    },
    SetSectionMode: function (isView) {
        $("#divContractAgreement").SetViewMode(isView);
        if (isView) {
            if ($("#SalesmanEmpName1").val() == "") {
                $("#divSalesmanEmpName1").html("");
            }
            if ($("#SalesmanEmpName2").val() == "") {
                $("#divSalesmanEmpName2").html("");
            }
            if ($("#SalesSupporterEmpName").val() == "") {
                $("#divSalesSupporterEmpName").html("");
            }
        }
    },
    DisabledSection: function (isDisabled) {
        $("#divContractAgreement").SetEnableView(!isDisabled);
    },

    AcquisitionTypeChange: function () {
        if (objCTS140.C_ACQUISITION_TYPE_CUST == $("#AcquisitionTypeCode option:selected").val() ||
            objCTS140.C_ACQUISITION_TYPE_INSIDE_COMPANY == $("#AcquisitionTypeCode option:selected").val() ||
            objCTS140.C_ACQUISITION_TYPE_SUBCONTRACTOR == $("#AcquisitionTypeCode option:selected").val()) {
            $("#IntroducerCode").attr("readonly", false);
        }
        else {
            $("#IntroducerCode").attr("readonly", true);
            $("#IntroducerCode").val("");
        }
    }
}


//function BindDOContractAgreementInformationDateField() {
//    call_ajax_method_json('/Contract/BindDOContractAgreementInformation_CTS140', "",
//            function (result, controls) {
//                if (result != undefined) {
//                    result.ApproveContractDate = ConvertDateObject(result.ApproveContractDate);
//                    $("#ApproveContractDate").val(ConvertDateToTextFormat(result.ApproveContractDate));
//                    $("#ContractOfficeCode").val(result.ContractOfficeCode);
//                    $("#AcquisitionTypeCode").val(result.AcquisitionTypeCode);
//                    $("#MotivationTypeCode").val(result.MotivationTypeCode);

//                    //$("#QuotationTargetCode").val(result.QuotationTargetCode);
//                    $("#QuotationTargetCode").val(result.QuotationTargetCodeShort);

//                    $("#QuotationAlphabet").val(result.QuotationAlphabet);
//                    $("#PlanCode").val(result.PlanCode);
//                    $("#SalesmanEmpNo1").val(result.SalesmanEmpNo1);
//                    $("#SalesmanEmpName1").val(result.SalesmanEmpName1);
//                    $("#SalesmanEmpNo2").val(result.SalesmanEmpNo2);
//                    $("#SalesmanEmpName2").val(result.SalesmanEmpName2);
//                    $("#RelatedContractCode").val(result.RelatedContractCodeShort);

//                    //$("#OldContractCode").val(result.OldContractCode);
//                    $("#OldContractCode").val(result.OldContractCodeShort);

//                    $("#IntroducerCode").val(result.IntroducerCode);
//                    $("#SalesSupporterEmpNo").val(result.SalesSupporterEmpNo);
//                    $("#SalesSupporterEmpName").val(result.SalesSupporterEmpName);

//                    AcquisitionTypeChange();
//                }
//            });
//}