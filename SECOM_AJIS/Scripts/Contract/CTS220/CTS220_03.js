$(document).ready(function () {
    InitialControlPropertyCTS220_03();
    BindDOProductInformation();
    //MaintainScreenItem();
});

function InitialControlPropertyCTS220_03() {
//    $("#ChangeImplementDate").InitialDate();

//    $('#SaleManEmpNo1').blur(
//    function () {
//        var obj = { EmpNo: $("#SaleManEmpNo1").val() }
//        call_ajax_method('/Contract/GetActiveEmployee_CTS220', obj,
//        function (result, controls) {
//            if (result == undefined) {
//                $("#SalesmanEmpNo1").val("");
//                $("#SaleManEmpName1").val("");
//            }
//            else {
//                if (result.Message != undefined && $("#SaleManEmpNo1").val() != "") {
//                    $("#SaleManEmpNo1").val("");
//                    $("#SaleManEmpName1").val("");
//                }
//                else {
//                    $("#SaleManEmpName1").val(result.EmpName);
//                }
//            }
//        });
//    });
    GetEmployeeNameData("#SaleManEmpNo1", "#SaleManEmpName1");
    InitialTrimTextEvent(["ApproveNo1", "ApproveNo2"]);

//    $('#SaleManEmpNo2').blur(
//    function () {
//        var obj = { EmpNo: $("#SaleManEmpNo2").val() }
//        call_ajax_method('/Contract/GetActiveEmployee_CTS220', obj,
//        function (result, controls) {
//            if (result == undefined) {
//                $("#SalesmanEmpNo2").val("");
//                $("#SaleManEmpName2").val("");
//            }
//            else {
//                if (result.Message != undefined && $("#SaleManEmpNo2").val() != "") {
//                    $("#SaleManEmpNo2").val("");
//                    $("#SaleManEmpName2").val("");
//                }
//                else {
//                    $("#SaleManEmpName2").val(result.EmpName);
//                }
//            }
//        });
//    });
    GetEmployeeNameData("#SaleManEmpNo2", "#SaleManEmpName2");

    $('#ChangeType').change(
    function () {
        ChangeTypeChange();
    });

    $("#OrderContractFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#StopFee").BindNumericBox(12, 2, 0, 999999999999.99); //Add by Jutarat A. on 14082012
    $("#SecurityMemo").SetMaxLengthTextArea(4000);
}

function BindDOProductInformation() {
    call_ajax_method_json('/Contract/BindDOProductInformation_CTS220', "", function (result, controls) {
        if (result != undefined)
        {
            result.ChangeImplementDate = ConvertDateObject(result.ChangeImplementDate);
            $("#ChangeImplementDate").val(ConvertDateToTextFormat(result.ChangeImplementDate));

            result.CreateDate = ConvertDateObject(result.CreateDate);
            $("#CreateDate").val(ConvertDateToTextFormat(result.CreateDate));

            $("#ChangeType").val(result.ChangeType);
            ChangeTypeChange(result.ChangeReasonType);

            $("#OrderContractFee").SetNumericCurrency(result.OrderContractFeeCurrencyType);
            $("#OrderContractFee").val(result.OrderContractFee);

            $("#StopFee").SetNumericCurrency(result.ContractFeeOnStopCurrencyType);
            $("#StopFee").val(result.ContractFeeOnStop); //Add by Jutarat A. on 14082012

            $("#SecurityTypeCode").val(result.SecurityTypeCode);
            $("#ProductCode").val(result.ProductCode);
            $("#SaleManEmpNo1").val(result.SaleManEmpNo1);
            $("#SaleManEmpName1").val(result.SaleManEmpName1);

            $("#SaleManEmpNo2").val(result.SaleManEmpNo2);
            $("#SaleManEmpName2").val(result.SaleManEmpName2);

            $("#ApproveNo1").val(result.ApproveNo1);
            $("#ApproveNo2").val(result.ApproveNo2);
            $("#CreateBy").val(result.CreateBy);
            $("#CreateByName").val(result.CreateByName);
            $("#SecurityMemo").val(result.SecurityMemo);

            //        if (result.OperationTypeCode != "" && result.OperationTypeCode != undefined) {
            //            $("#HaveKey").attr("checked", true);
            //        }
            //        else {
            //            $("#HaveNoKey").attr("checked", true);
            //        }
        }

    });

}

function ChangeTypeChange(reasonValue) {
    var obj = { ChangeType: $("#ChangeType").val() };

    if (obj.ChangeType == objCTS220_03.C_RENTAL_CHANGE_TYPE_PLAN_CHANGE
        || obj.ChangeType == objCTS220_03.C_RENTAL_CHANGE_TYPE_CHANGE_NAME
        || obj.ChangeType == objCTS220_03.C_RENTAL_CHANGE_TYPE_END_CONTRACT
        || obj.ChangeType == objCTS220_03.C_RENTAL_CHANGE_TYPE_CANCEL
        || obj.ChangeType == objCTS220_03.C_RENTAL_CHANGE_TYPE_STOP
        || obj.ChangeType == objCTS220_03.C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP
        || obj.ChangeType == objCTS220_03.C_RENTAL_CHANGE_TYPE_CHANGE_NAME_DURING_STOP
        || obj.ChangeType == objCTS220_03.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START
        || obj.ChangeType == objCTS220_03.C_RENTAL_CHANGE_TYPE_REMOVE_INSTRU_DURING_STOP
        || obj.ChangeType == objCTS220_03.C_RENTAL_CHANGE_TYPE_REMOVE_ALL) {

        call_ajax_method('/Contract/GetComboBoxReasonMethod_CTS220', obj, function (result, controls) {
            regenerate_combo("#ChangeReasonType", result);

            if (reasonValue != undefined) {
                $("#ChangeReasonType").val(reasonValue);
            }
        });  
        
        $("#ChangeReasonType").attr("disabled", false);                           
    }
    else {
        $("#ChangeReasonType").val("");
        $("#ChangeReasonType").attr("disabled", true); 
    }
}

function SetSectionModeCTS220_03(isView) {
    $("#divProductSection").SetViewMode(isView);

    if (isView) {
        if ($("#SaleManEmpName1").val() == "") {
            $("#divSaleManEmpName1").html("");
        }

        if ($("#SaleManEmpName2").val() == "") {
            $("#divSaleManEmpName2").html("");
        }
    }
}