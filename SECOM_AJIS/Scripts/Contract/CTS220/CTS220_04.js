$(document).ready(function () {
    InitialControlPropertyCTS220_04();
    //MaintainScreenItem();
});

function BindDoChangeContractCondition() {
    call_ajax_method_json('/Contract/BindDoChangeContractCondition_CTS220', "", null);
}

function InitialControlPropertyCTS220_04() {
//    $('#NegotiationStaffEmpNo1').blur(
//    function () {
//        var obj = { EmpNo: $("#NegotiationStaffEmpNo1").val() }
//        call_ajax_method('/Contract/GetActiveEmployee_CTS220', obj,
//        function (result, controls) {
//            if (result == undefined) {
//                $("#NegotiationStaffEmpNo1").val("");
//                $("#NegotiationStaffEmpName1").val("");
//            }
//            else {
//                if (result.Message != undefined && $("#NegotiationStaffEmpNo1").val() != "") {
//                    $("#NegotiationStaffEmpNo1").val("");
//                    $("#NegotiationStaffEmpName1").val("");
//                }
//                else {
//                    $("#NegotiationStaffEmpName1").val(result.EmpName);
//                }
//            }
//        });
//    });
    GetEmployeeNameData("#NegotiationStaffEmpNo1", "#NegotiationStaffEmpName1");

//    $('#NegotiationStaffEmpNo2').blur(
//    function () {
//        var obj = { EmpNo: $("#NegotiationStaffEmpNo2").val() }
//        call_ajax_method('/Contract/GetActiveEmployee_CTS220', obj,
//        function (result, controls) {
//            if (result == undefined) {
//                $("#NegotiationStaffEmpNo2").val("");
//                $("#NegotiationStaffEmpName2").val("");
//            }
//            else {
//                if (result.Message != undefined && $("#NegotiationStaffEmpNo2").val() != "") {
//                    $("#NegotiationStaffEmpNo2").val("");
//                    $("#NegotiationStaffEmpName2").val("");
//                }
//                else {
//                    $("#NegotiationStaffEmpName2").val(result.EmpName);
//                }
//            }
//        });
//    });
    GetEmployeeNameData("#NegotiationStaffEmpNo2", "#NegotiationStaffEmpName2");
}

function SetSectionModeCTS220_04(isView) {
    $("#divChangeContractSection").SetViewMode(isView);

    if (isView) {
        if ($("#NegotiationStaffEmpName1").val() == "") {
            $("#divNegotiationStaffEmpName1").html("");
        }

        if ($("#NegotiationStaffEmpName2").val() == "") {
            $("#divNegotiationStaffEmpName2").html("");
        }
    }
}