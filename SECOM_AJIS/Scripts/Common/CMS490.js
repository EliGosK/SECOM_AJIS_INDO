
var param;

$(document).ready(function () {
    $("#btnReIssue").click(function () {
        var DocType = $("#txtDocType").val();
        var DocNo = $("#txtDocNo").val();
        var key = ajax_method.GetKeyURL(null);
        var strmsgbox = $("#msgbox").val();

        if (DocType == "") {
            param = {
                DocNo: $.trim($("#txtDocNo").val())
            };
            ajax_method.CallScreenController("/Common/CMS490_blank", param,
            function (result, controls) {
                if (result == true) {
                }
                else if (controls != undefined) {
                    VaridateCtrl(controls, controls);
                }
            });
        }
        else if (DocType == "BLR010") {
            param = {
                DocNo: $.trim($("#txtDocNo").val())
            };
            ajax_method.CallScreenController("/Common/CMS490_UpdateInvoice", param,
            function (result, controls) {
                if (result == true) {
                    var obj = {
                        module: "Common",
                        code: "MSG0028",
                        param: strmsgbox + " " + DocNo
                    };
                    ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
            OpenYesNoMessageDialog(result.Code, result.Message,
                    function () {
                    var objMsg = { module: "Common", code: "MSG0153" };
                    call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                        OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {
                            var url = ajax_method.GenerateURL("/Billing/BLR010_Invoice?k=" + key + "&invoiceNo=" + DocNo);  // Invoice
                            window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
                            ClearScreen();
                        });
                    });
                }, null);
            });
                }
                else if (controls != undefined) {
                    VaridateCtrl(controls, controls);
                }
            });
        }
        else if (DocType == "BLR020") {
            param = {
                DocNo: $.trim($("#txtDocNo").val())
            };
            ajax_method.CallScreenController("/Common/CMS490_UpdateTaxInvoice", param,
            function (result) {
                if (result == true) {
                    var obj = {
                        module: "Common",
                        code: "MSG0028",
                        param: strmsgbox + " " + DocNo
                    };
                    ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
            OpenYesNoMessageDialog(result.Code, result.Message,
                    function () {
                    var objMsg = { module: "Common", code: "MSG0153" };
                    call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                        OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {
                            var url = ajax_method.GenerateURL("/Billing/BLR020_TaxInvoice?k=" + key + "&taxInvoice=" + DocNo); // Tax Invoice
                            window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
                            ClearScreen();
                        });
                    });
                }, null);
            });
                }
            });
        }
        else if (DocType == "ICR010") {
            param = {
                DocNo: $.trim($("#txtDocNo").val())
            };
            ajax_method.CallScreenController("/Common/CMS490_UpdateReceipt", param,
            function (result) {
                if (result == true) {
                    var obj = {
                        module: "Common",
                        code: "MSG0028",
                        param: strmsgbox + " " + DocNo
                    };
                    ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
            OpenYesNoMessageDialog(result.Code, result.Message,
                    function () {
                    var objMsg = { module: "Common", code: "MSG0153" };
                    call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                        OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {
                            var url = ajax_method.GenerateURL("/Income/ICR010_Receipt?k=" + key + "&receiptNo=" + DocNo);     //Receipt
                            window.open(url, "IncomeDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
                            ClearScreen();
                        });
                    });
                }, null);
            });
                }
            });
        }
        else if (DocType == "ICR020") {
            param = {
                DocNo: $.trim($("#txtDocNo").val())
            };
            ajax_method.CallScreenController("/Common/CMS490_UpdateCreditNote", param,
            function (result) {
                if (result == true) {
                    var obj = {
                        module: "Common",
                        code: "MSG0028",
                        param: strmsgbox + " " + DocNo
                    };
                    ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
                        OpenYesNoMessageDialog(result.Code, result.Message,
                    function () {
                    var objMsg = { module: "Common", code: "MSG0153" };
                    call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                        OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {
                            var url = ajax_method.GenerateURL("/Income/ICR020_CreditNote?k=" + key + "&creditNoteNo=" + DocNo);     //Credit Note
                            window.open(url, "IncomeDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
                            ClearScreen();
                        });
                    });
                    }, null);
            });
                }
            });
        }
    });
    $("#btnClear").click(function () {
        ClearScreen();
    });
    function ClearScreen() {
        $("#txtDocNo").val("");
        $("#txtDocType").val("");
    }
});