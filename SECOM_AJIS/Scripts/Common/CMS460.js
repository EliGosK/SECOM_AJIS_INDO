

$(document).ready(function () {

    $("#IssueDate").InitialDate();

    var now = new Date();
    $("#IssueDate").val(ConvertDateToTextFormat(ConvertDateObject(now)));

    InitialNumericInputTextBox(["ManagementNoFrom", "ManagementNoTo"], false);

    $("#btnReprint").click(RePrint_click);
});

function FirstLoadScreen() {

}

function RePrint_click() {
    $("#btnReprint").attr("disabled",true);
    var obj = { IssueDate: $("#IssueDate").val(),
        ManagementNoFrom: $("#ManagementNoFrom").val(),
        ManagementNoTo: $("#ManagementNoTo").val()
    };

    ajax_method.CallScreenController("/Common/CMS460_ReprintData", obj,
        function (result, controls) {
            //$("#btnReprint").attr("disabled", "false");
            if (controls != undefined || result == false) {
                VaridateCtrl(["IssueDate", "ManagementNoFrom", "ManagementNoTo"], controls);
                
                //return;
            }
            else {
                //SetScreenControl(result);
                //InitialComboValue(1);
                //InitialCommandButton(0);

                var objMsg = { module: "Common", code: "MSG0153" };
                call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                    OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {});
                });
            }
            $("#btnReprint").attr("disabled", false);
        });
}