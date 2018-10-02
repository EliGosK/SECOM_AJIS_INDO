/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>
/// <reference path="../../Scripts/Base/AutoComplete.js"/>
/// <reference path="../json.js" />

var gridTest;
var bls060_grid;
$(document).ready(function () {

    $("#ivr140_batchdatetime").InitialDate();
    $("#IVP140_batchdatetime").InitialDate();

    // Akat K. Test Download Document ############################################
    $("#btnTestDownload").click(function () {
        var url = "/Inventory/IVR_TestDownload";
        if (url.indexOf("k=") < 0) {
            var key = ajax_method.GetKeyURL(null);
            if (key != "") {
                if (url.indexOf("?") > 0) {
                    url = url + "&k=" + key;
                } else {
                    url = url + "?k=" + key;
                }
                $("#strSessionKey").val(key);
            }
        }
        url = generate_url(url);
        $('#IVR_TestDownload').attr('action', url);
        $("#IVR_TestDownload").submit();
    });
    // Akat K. Test Download Document ############################################

    $("#btnClearReportCache").click(function () {
        ajax_method.CallScreenController("/Common/CMS999_ClearReportCache", null, function (result, controls) {
            if (result) {
                OpenInformationMessageDialog("------", "OK");
            }
            else {
                OpenInformationMessageDialog("------", "NO..");
            }
        });
    });

    $("#btnGetIVR100").click(function () {
        var param = {
            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR100", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadPdfAndWriteLog?k=" + key + "&strDocumentNo=" + result.DocumentNo + "&documentOCC=" + result.DocumentOCC + "&strDocumentCode=" + result.DocumentCode + "&fileName=" + result.FilePath)
                window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnGetIVR110").click(function () {
        var param = {
            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR110", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadPdfAndWriteLog?k=" + key + "&strDocumentNo=" + result.DocumentNo + "&documentOCC=" + result.DocumentOCC + "&strDocumentCode=" + result.DocumentCode + "&fileName=" + result.FilePath)
                window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnDownload").click(function () {

        var url = "/Common/CMS999_TestDownload";
        if (url.indexOf("k=") < 0) {

            var key = ajax_method.GetKeyURL(null);
            if (key != "") {
                if (url.indexOf("?") > 0) {
                    url = url + "&k=" + key;
                }
                else {
                    url = url + "?k=" + key;
                }
            }
        }
        url = generate_url(url) + "&strFileName=" + $("#txtFileName").val();
        $("#ifDownload").get(0).src = url;
    });


    /*-----------------------------------------*/
    /*--------- Test Contract Report ----------*/
    /*-----------------------------------------*/
    $("#btnCTR010").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/Contract/CTR010_ContractEnglish?k=" + key + "&iDocID=" + $("#txtDocId").val());
        window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnCTR011").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/Contract/CTR011_ContractThai?k=" + key + "&iDocID=" + $("#txtDocId").val());
        window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnCTR020").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/Contract/CTR020_ChangeNotice?k=" + key + "&iDocID=" + $("#txtDocId").val());
        window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnCTR030").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/Contract/CTR030_ChangeMemorandum?k=" + key + "&iDocID=" + $("#txtDocId").val());
        window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnCTR040").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/Contract/CTR040_StartResumeMemorandum?k=" + key + "&iDocID=" + $("#txtDocId").val());
        window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnCTR050").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/Contract/CTR050_ConfirmCurrentInstrumentMemorandum?k=" + key + "&iDocID=" + $("#txtDocId").val());
        window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnCTR060").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/Contract/CTR060_CancelContractMemorandum?k=" + key + "&iDocID=" + $("#txtDocId").val());
        window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnCTR070").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/Contract/CTR070_ChangeFeeMemorandum?k=" + key + "&iDocID=" + $("#txtDocId").val());
        window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnCTR080").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/Contract/CTR080_QuotationForCancelContractMemorandum?k=" + key + "&iDocID=" + $("#txtDocId").val());
        window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnCTR090").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/Contract/CTR090_CoverLetter?k=" + key + "&iDocID=" + $("#txtDocId").val());
        window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnGenContractRpt").click(function () {
        var obj = { iDocID: $("#txtDocId2").val(),
            strDocNo: $("#txtDocNo").val(),
            strDocumentCode: $("#txtDocCode").val()
        };

        call_ajax_method_json("/Contract/CTR_GenerateReport", obj, null);
    });
    /*-----------------------------------------*/


    $("#btnGetIVR010").click(function () {
        var param = {
            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR010", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR010?k=" + key);
                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnGetIVR020").click(function () {
        var param = {
            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR020", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR020?k=" + key);
                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnGetIVR030").click(function () {
        var param = {
            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR030", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR030?k=" + key);
                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnGetIVR040").click(function () {
        var param = {
            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR040", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR040?k=" + key);
                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnGetIVR050").click(function () {
        var param = {
            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR050", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR050?k=" + key);
                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnGetIVR060").click(function () {
        var param = {
            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR060", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR060?k=" + key);
                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnIVP070").click(function () {
        ajax_method.CallScreenController("/Common/CMS999_TestProcessIVR070", null, function (result, controls) {
            if (result != null) {

            }
        });
    });

    $("#btnIVP080").click(function () {
        ajax_method.CallScreenController("/Common/CMS999_TestProcessIVR080", null, function (result, controls) {
            if (result != null) {

            }
        });
    });

    $("#btnIVP090").click(function () {
        ajax_method.CallScreenController("/Common/CMS999_TestProcessIVR090", null, function (result, controls) {
            if (result != null) {
                alert(result);
            }
        });
    });

    $("#btnIVP110").click(function () {
        ajax_method.CallScreenController("/Common/CMS999_TestProcessIVP110", null, function (result, controls) {
            if (result != null) {

            }
        });
    });

    $("#btnIVP120").click(function () {
        ajax_method.CallScreenController("/Common/CMS999_TestProcessIVP120", null, function (result, controls) {
            if (result != null) {
                alert(result);
            }
        });
    });

    $("#btnIVP040").click(function () {
        ajax_method.CallScreenController("/Common/CMS999_TestProcessIVP040", null, function (result, controls) {
            if (result != null) {
                alert(result);
            }
        });
    });

    $("#btnIVP060").click(function () {
        ajax_method.CallScreenController("/Common/CMS999_TestProcessIVP060", null, function (result, controls) {
            if (result != null) {
                alert(result);
            }
        });
    });

    $("#btnCompleteInstallation").click(function () {
        ajax_method.CallScreenController("/Common/CMS999_TestUpdateCompleteInstallation", null, function (result, controls) {
            if (result != null) {
                alert(result);
            }
        });
    });

    $("#btnContractStartService").click(function () {
        ajax_method.CallScreenController("/Common/CMS999_TestUpdateContractStartService", null, function (result, controls) {
            if (result != null) {
                alert(result);
            }
        });
    });

    $("#btnCompleteProject").click(function () {
        ajax_method.CallScreenController("/Common/CMS999_TestUpdateCompleteProject", null, function (result, controls) {
            if (result != null) {
                alert(result);
            }
        });
    });

    $("#btnCancelInstallation").click(function () {
        ajax_method.CallScreenController("/Common/CMS999_TestUpdateCancelInstallation", null, function (result, controls) {
            if (result != null) {
                alert(result);
            }
        });
    });

    $("#btnCustomerAcceptance").click(function () {
        ajax_method.CallScreenController("/Common/CMS999_TestUpdateCustomerAcceptance", null, function (result, controls) {
            if (result != null) {
                alert(result);
            }
        });
    });

    $("#btnRealInvestigation").click(function () {
        ajax_method.CallScreenController("/Common/CMS999_TestUpdateRealInvestigation", null, function (result, controls) {
            if (result != null) {
                alert(result);
            }
        });
    });

    $("#btnGetIVR150").click(function () {
        var param = {
            batchdate: $("#ivr140_batchdatetime").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR150", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR150?k=" + key);
                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnGetIVR140").click(function () {
        var param = {
            batchdate: $("#ivr140_batchdatetime").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR140", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR140?k=" + key);
                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    // TON Report ----------------------------------------------------------------------------------- //

    $("#btnGetIVR070").click(function () {
        var param = {
            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR070", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR070?k=" + key);
                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnGetIVR080").click(function () {
        var param = {
            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR080", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR080?k=" + key);
                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnGetIVR090").click(function () {
        var param = {
            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR090", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR090?k=" + key);
                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnGetIVR120").click(function () {
        var param = {
            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR120", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR120?k=" + key);
                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnGetIVR130").click(function () {
        var param = {
            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR130", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR130?k=" + key);
                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnGetIVR170").click(function () {
        var param = {
            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR170", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR170?k=" + key);
                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnGetIVR180").click(function () {
        var param = {
            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR180", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR180?k=" + key);
                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnGetIVR190").click(function () {
        var param = {
            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR190", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR190?k=" + key);
                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnGetIVR191").click(function () {
        var param = {
            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR191", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR191?k=" + key);
                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnGetIVR192").click(function () {
        var param = {
            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR192", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR192?k=" + key);
                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnGetIVR210").click(function () {
        var param = {
            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetIVR210", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR210?k=" + key);
                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    // TON Report ----------------------------------------------------------------------------------- //

    //---------------- Common ---------------//
    $("#btnCMR010").click(function () {

        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/Common/CMR010_IssueList?k=" + key);
        window.open(url, "CommonDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnBLR010").click(function () {

        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/Billing/BLR010_Invoice?k=" + key + "&invoiceNo=" + $("#Invoice_Taxinvice").val());  // in TestBillingController.cs
        window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");

        //        var obj = { invoiceNo: $("#Invoice_Taxinvice").val() };
        //        download_method.CallDownloadController("iframeDownload", "/Billing/BLR010_Invoice", obj);

        //        //Test by Jutarat A.
        //        var obj = {
        //            invoiceNo: $("#Invoice_Taxinvice").val()
        //        };

        //        call_ajax_method_json("/Billing/BLR010_Invoice", obj, null);
        //        //End Test

    });

    $("#btnBLR020").click(function () {

        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/Billing/BLR020_Invoice?k=" + key + "&invoiceNo=" + $("#Invoice_Taxinvice").val());  // in TestBillingController.cs
        window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");

    });

    $("#btnBLR030").click(function () {

        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/Billing/BLR030_Invoice?k=" + key + "&invoiceNo=" + $("#Invoice_Taxinvice").val());  // in TestBillingController.cs
        window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");

        //        var obj = { invoiceNo: $("#Invoice_Taxinvice").val() };
        //        download_method.CallDownloadController("iframeDownload", "/Billing/BLR030_PaymentForm", obj);
    });

    $("#btnBLR040").click(function () {

        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/Billing/BLR040_Invoice?k=" + key + "&invoiceNo=" + $("#Invoice_Taxinvice").val());  // in TestBillingController.cs
        window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");

        //        var obj = { invoiceNo: $("#Invoice_Taxinvice").val() };
        //        download_method.CallDownloadController("iframeDownload", "/Billing/BLR030_PaymentForm", obj);
    });

    $("#btnBLR050").click(function () {

        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/Billing/BLR050_DocReceive?k=" + key + "&invoiceNo=" + $("#Invoice_Taxinvice").val());  // in TestBillingController.cs
        window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });


    $("#btnFoxitPrint").click(function () {
        //        var param = {
        //            strInventorySlipNo: $("#txtInvCheckingSlipNo").val()
        //        };

        //        ajax_method.CallScreenController("/Common/CMS999_GetIVR191", param, function (result, controls) {
        //            if (result != null) {
        //                var key = ajax_method.GetKeyURL(null);
        //                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadIVR191?k=" + key);
        //                window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
        //            }
        //        });

        ajax_method.CallScreenController("/Billing/CMS999_TestPrintFoxitByCommand", "", function (result) {
            if (result != undefined) {
                alert("printed");
            }
        });
    });

    $("#btnGenBLR010").click(function () {
        var obj = { strInvoiceNo: $("#Invoice_Taxinvice").val() };
        call_ajax_method_json("/Billing/BLP010_GenerateReport", obj, null);
    });

    $("#btnGenBLR020").click(function () {
        var obj = { strInvoiceNo: $("#Invoice_Taxinvice").val() };
        call_ajax_method_json("/Billing/BLP020_GenerateReport", obj, null);
    });

    $("#btnGenBLR010FromTable").click(function () {
        ajax_method.CallScreenController("/Billing/BLP010_GenerateReportFromTable", null, function (result, controls) {
            if (result) {
                OpenInformationMessageDialog("------", "Done");
            }
        });
    });

    /*-----------------------------------------*/
    /*--------- Test Contract Process ----------*/
    /*-----------------------------------------*/
    $("#btnCTP020").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/SwtCTP020/index?k=" + key);
        window.open(url, "ContractProcess", "scrollbars=no,menubar=yes,height=450,width=450,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnCTP021").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/SwtCTP021/index?k=" + key);
        window.open(url, "ContractProcess", "scrollbars=no,menubar=yes,height=450,width=450,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnCTP022").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/SwtCTP022/index?k=" + key);
        window.open(url, "ContractProcess", "scrollbars=no,menubar=yes,height=450,width=450,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnCTP090").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/SwtCTP090/index?k=" + key);
        window.open(url, "ContractProcess", "scrollbars=no,menubar=yes,height=450,width=450,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnCTP100").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/SwtCTP100/index?k=" + key);
        window.open(url, "ContractProcess", "scrollbars=no,menubar=yes,height=450,width=450,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnCTP120").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/SwtCTP120/index?k=" + key);
        window.open(url, "ContractProcess", "scrollbars=no,menubar=yes,height=450,width=450,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnCTP030").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/SwtCTP030/index?k=" + key);
        window.open(url, "ContractProcess", "scrollbars=no,menubar=yes,height=450,width=450,resizable=yes,toolbar=no,location=no,status=yes");
    });
    /*-----------------------------------------*/

    $("#btnCTP060").click(function () {
        ajax_method.CallScreenController("/Common/CMS999_CTP060_TestSendNotifyEmail", "", function (result, controls) {
            if (result != undefined) {
                alert("Finish");
            }
        });
    });
    $("#btnCTP070").click(function () {
        var obj = {
            mode: 1
        };

        if ($("#rdoCase2").attr("checked") == "checked")
            obj.mode = 2;
        else if ($("#rdoCase3").attr("checked") == "checked")
            obj.mode = 3;
        else if ($("#rdoCase4").attr("checked") == "checked")
            obj.mode = 4;
        else if ($("#rdoCase5").attr("checked") == "checked")
            obj.mode = 5;

        ajax_method.CallScreenController("/Common/CMS999_CTP070_TestUpdateCustomerAcceptance", obj, function (result, controls) {
            if (result != undefined) {
                alert("Finish");
            }
        });
    });
    $("#btnCTP080").click(function () {
        ajax_method.CallScreenController("/Common/CMS999_CTP080_TestGenerateProjecCode", "", function (result, controls) {
            if (result != undefined) {
                alert("Project code 1 = " + result[0] + ", Project code 2 = " + result[1]);
            }
        });
    });
    $("#btnCTP110").click(function () {
        var obj = {
            mode: "1"
        };

        if ($("#rdoCTP110Case2").attr("checked") == "checked")
            obj.mode = "2";
        else if ($("#rdoCTP110Case31").attr("checked") == "checked")
            obj.mode = "3.1";
        else if ($("#rdoCTP110Case32").attr("checked") == "checked")
            obj.mode = "3.2";
        else if ($("#rdoCTP110Case33").attr("checked") == "checked")
            obj.mode = "3.3";
        else if ($("#rdoCTP110Case4").attr("checked") == "checked")
            obj.mode = "4";
        else if ($("#rdoCTP110Case5").attr("checked") == "checked")
            obj.mode = "5";
        else if ($("#rdoCTP110Case6").attr("checked") == "checked")
            obj.mode = "6";
        else if ($("#rdoCTP110Case7").attr("checked") == "checked")
            obj.mode = "7";

        ajax_method.CallScreenController("/Common/CMS999_CTP110_TestGenerateMaintenanceCheckupSchedule", obj, function (result, controls) {
            if (result != undefined) {
                alert("Finish");
            }
        });
    });

    //----------- Software Test : Billing ----------//
    $("#btnBLP010").click(function () {
        var param = { case_on: $("#BillingCase").val() };
        $("#txtResultBilling").val("Executing... \n" + "BLP010");

        ajax_method.CallScreenController("/Common/CMS999_BLP010", param, function (result, controls) {
            if (result != undefined) {

                var strTotalResult = $("#txtResultBilling").val() + '\n';


                for (var i = 0; i < result.length; i++) {
                    var str1 = JSON.stringify(result[i]);
                    str1 = str1.replace("{", "");
                    str1 = str1.replace("}", "");

                    var arr = str1.split(",");

                    strTotalResult += ('------- Row ' + (i + 1).toString() + ' -------\n');

                    for (var j = 0; j < arr.length; j++) {
                        strTotalResult += (arr[j] + '\n');
                    }

                    strTotalResult += '\n\n';
                }
                $("#txtResultBilling").val(strTotalResult + 'Finished');
            }
            else {
                $("#txtResultBilling").val($("#txtResultBilling").val() + '\n\nFinished');
            }



        });
    });

    //
    $("#btnBLP011").click(function () {
        var param = { case_on: $("#BillingCase").val() };
        $("#txtResultBilling").val("Executing... \n" + "BLP011");

        ajax_method.CallScreenController("/Common/CMS999_BLP011", param, function (result, controls) {
            if (result != undefined) {

                var strTotalResult = $("#txtResultBilling").val() + '\n';


                for (var i = 0; i < result.length; i++) {
                    var str1 = JSON.stringify(result[i]);
                    str1 = str1.replace("{", "");
                    str1 = str1.replace("}", "");

                    var arr = str1.split(",");

                    strTotalResult += ('------- Row ' + (i + 1).toString() + ' -------\n');

                    for (var j = 0; j < arr.length; j++) {
                        strTotalResult += (arr[j] + '\n');
                    }

                    strTotalResult += '\n\n';
                }
                $("#txtResultBilling").val(strTotalResult + 'Finished');
            }
            else {
                $("#txtResultBilling").val($("#txtResultBilling").val() + '\nFinished...');
            }

        });
    });

    //

    $("#btnBLP012").click(function () {
        var param = { case_on: $("#BillingCase").val() };
        $("#txtResultBilling").val("Executing... \n" + "BLP012");

        ajax_method.CallScreenController("/Common/CMS999_BLP012", param, function (result, controls) {
            if (result != undefined) {

                var strTotalResult = $("#txtResultBilling").val() + '\n';


                for (var i = 0; i < result.length; i++) {
                    var str1 = JSON.stringify(result[i]);
                    str1 = str1.replace("{", "");
                    str1 = str1.replace("}", "");

                    var arr = str1.split(",");

                    strTotalResult += ('------- Row ' + (i + 1).toString() + ' -------\n');

                    for (var j = 0; j < arr.length; j++) {
                        strTotalResult += (arr[j] + '\n');
                    }

                    strTotalResult += '\n\n';
                }
                $("#txtResultBilling").val(strTotalResult + 'Finished');
            }
            else {
                $("#txtResultBilling").val($("#txtResultBilling").val() + '\n\nFinished');
            }

        });
    });

    //

    $("#btnBLP013").click(function () {
        var param = { case_on: $("#BillingCase").val() };
        $("#txtResultBilling").val("Executing... \n" + "BLP013");

        ajax_method.CallScreenController("/Common/CMS999_BLP013", param, function (result, controls) {
            if (result != undefined) {

                var strTotalResult = $("#txtResultBilling").val() + '\n';


                for (var i = 0; i < result.length; i++) {
                    var str1 = JSON.stringify(result[i]);
                    str1 = str1.replace("{", "");
                    str1 = str1.replace("}", "");

                    var arr = str1.split(",");

                    strTotalResult += ('------- Row ' + (i + 1).toString() + ' -------\n');

                    for (var j = 0; j < arr.length; j++) {
                        strTotalResult += (arr[j] + '\n');
                    }

                    strTotalResult += '\n\n';
                }
                $("#txtResultBilling").val(strTotalResult + 'Finished');
            }
            else {
                $("#txtResultBilling").val($("#txtResultBilling").val() + '\n\nFinished');
            }

        });
    });

    //

    $("#btnBLP015").click(function () {
        var param = { case_on: $("#BillingCase").val() };
        $("#txtResultBilling").val("Executing... \n" + "BLP015");

        ajax_method.CallScreenController("/Common/CMS999_BLP015", param, function (result, controls) {
            if (result != undefined) {

                var strTotalResult = $("#txtResultBilling").val() + '\n';


                for (var i = 0; i < result.length; i++) {
                    var str1 = JSON.stringify(result[i]);
                    str1 = str1.replace("{", "");
                    str1 = str1.replace("}", "");

                    var arr = str1.split(",");

                    strTotalResult += ('------- Row ' + (i + 1).toString() + ' -------\n');

                    for (var j = 0; j < arr.length; j++) {
                        strTotalResult += (arr[j] + '\n');
                    }

                    strTotalResult += '\n\n';
                }
                $("#txtResultBilling").val(strTotalResult + 'Finished');
            }
            else {
                $("#txtResultBilling").val($("#txtResultBilling").val() + '\n\nFinished');
            }

        });
    });

    //

    $("#btnBLP016").click(function () {
        var param = { case_on: $("#BillingCase").val() };
        $("#txtResultBilling").val("Executing... \n" + "BLP016");

        ajax_method.CallScreenController("/Common/CMS999_BLP016", param, function (result, controls) {
            if (result != undefined) {

                var strTotalResult = $("#txtResultBilling").val() + '\n';


                for (var i = 0; i < result.length; i++) {
                    var str1 = JSON.stringify(result[i]);
                    str1 = str1.replace("{", "");
                    str1 = str1.replace("}", "");

                    var arr = str1.split(",");

                    strTotalResult += ('------- Row ' + (i + 1).toString() + ' -------\n');

                    for (var j = 0; j < arr.length; j++) {
                        strTotalResult += (arr[j] + '\n');
                    }

                    strTotalResult += '\n\n';
                }
                $("#txtResultBilling").val(strTotalResult + 'Finished');
            }
            else {
                $("#txtResultBilling").val($("#txtResultBilling").val() + '\n\nFinished');
            }

        });
    });

    //

    $("#btnBLP017").click(function () {
        var param = { case_on: $("#BillingCase").val() };
        $("#txtResultBilling").val("Executing... \n" + "BLP017");

        ajax_method.CallScreenController("/Common/CMS999_BLP017", param, function (result, controls) {
            if (result != undefined) {

                var strTotalResult = "";


                for (var i = 0; i < result.length; i++) {
                    var str1 = JSON.stringify(result[i]);
                    str1 = str1.replace("{", "");
                    str1 = str1.replace("}", "");

                    var arr = str1.split(",");

                    strTotalResult += ('------- Row ' + (i + 1).toString() + ' -------\n');

                    for (var j = 0; j < arr.length; j++) {
                        strTotalResult += (arr[j] + '\n');
                    }

                    strTotalResult += '\n\n';
                }
                $("#txtResultBilling").val(strTotalResult + 'Finished');
            }
            else {
                $("#txtResultBilling").val($("#txtResultBilling").val() + '\n\nFinished');
            }

        });
    });

    //

    $("#btnBLP070").click(function () {
        var param = { case_on: $("#BillingCase").val() };
        $("#txtResultBilling").val("Executing... \n" + "BLP070");

        ajax_method.CallScreenController("/Common/CMS999_BLP070", param, function (result, controls) {
            if (result != undefined) {

                var strTotalResult = $("#txtResultBilling").val() + '\n';


                for (var i = 0; i < result.length; i++) {
                    var str1 = JSON.stringify(result[i]);
                    str1 = str1.replace("{", "");
                    str1 = str1.replace("}", "");

                    var arr = str1.split(",");

                    strTotalResult += ('------- Row ' + (i + 1).toString() + ' -------\n');

                    for (var j = 0; j < arr.length; j++) {
                        strTotalResult += (arr[j] + '\n');
                    }

                    strTotalResult += '\n\n';
                }
                $("#txtResultBilling").val(strTotalResult + 'Finished');
            }
            else {
                $("#txtResultBilling").val($("#txtResultBilling").val() + '\n\nFinished');
            }

        });
    });

    //

    $("#btnBLP080").click(function () {
        var param = { case_on: $("#BillingCase").val() };
        $("#txtResultBilling").val("Executing... \n" + "BLP080");

        ajax_method.CallScreenController("/Common/CMS999_BLP080", param, function (result, controls) {
            if (result != undefined) {

                var strTotalResult = $("#txtResultBilling").val() + '\n';


                for (var i = 0; i < result.length; i++) {
                    var str1 = JSON.stringify(result[i]);
                    str1 = str1.replace("{", "");
                    str1 = str1.replace("}", "");

                    var arr = str1.split(",");

                    strTotalResult += ('------- Row ' + (i + 1).toString() + ' -------\n');

                    for (var j = 0; j < arr.length; j++) {
                        strTotalResult += (arr[j] + '\n');
                    }

                    strTotalResult += '\n\n';
                }
                $("#txtResultBilling").val(strTotalResult + 'Finished');
            }
            else {
                $("#txtResultBilling").val($("#txtResultBilling").val() + '\n\nFinished');
            }

        });
    });

    // 

    $("#btnBLP014").click(function () {
        var param = { case_on: $("#BillingCase").val() };
        $("#txtResultBilling").val("Executing... \n" + "BLP014");

        ajax_method.CallScreenController("/Common/CMS999_BLP014", param, function (result, controls) {
            if (result != undefined) {

                var strTotalResult = $("#txtResultBilling").val() + '\n';


                for (var i = 0; i < result.length; i++) {
                    var str1 = JSON.stringify(result[i]);
                    str1 = str1.replace("{", "");
                    str1 = str1.replace("}", "");

                    var arr = str1.split(",");

                    strTotalResult += ('------- Row ' + (i + 1).toString() + ' -------\n');

                    for (var j = 0; j < arr.length; j++) {
                        strTotalResult += (arr[j] + '\n');
                    }

                    strTotalResult += '\n\n';
                }
                $("#txtResultBilling").val(strTotalResult + 'Finished');
            }
            else {
                $("#txtResultBilling").val($("#txtResultBilling").val() + '\n\nFinished');
            }

        });
    });


    $("#btnBLP031").click(function () {
        var param = { case_on: $("#BillingCase").val() };
        $("#txtResultBilling").val("Executing... \n" + "BLP031");

        ajax_method.CallScreenController("/Common/CMS999_BLP031", param, function (result, controls) {
            if (result != undefined) {

                var strTotalResult = $("#txtResultBilling").val() + '\n';


                for (var i = 0; i < result.length; i++) {
                    var str1 = JSON.stringify(result[i]);
                    str1 = str1.replace("{", "");
                    str1 = str1.replace("}", "");

                    var arr = str1.split(",");

                    strTotalResult += ('------- Row ' + (i + 1).toString() + ' -------\n');

                    for (var j = 0; j < arr.length; j++) {
                        strTotalResult += (arr[j] + '\n');
                    }

                    strTotalResult += '\n\n';
                }
                $("#txtResultBilling").val(strTotalResult + 'Finished');
            }
            else {
                $("#txtResultBilling").val($("#txtResultBilling").val() + '\n\nFinished');
            }

        });
    });

    $("#btnBLP030Temp").click(function () {
        call_ajax_method_json("/Billing/BLP030_ManageInvoiceProcessTemp", null,
            function (result) {
                if (result != undefined) {
                    $("#txtResultBilling").val(result);
                }
            }
        );

    });


    /*-----------------------------------------*/
    /*--------- Test Income Report ----------*/
    /*-----------------------------------------*/
    $("#btnICR010").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/Income/ICR010_Receipt?k=" + key + "&receiptNo=" + $("#incomeDocId").val());
        window.open(url, "IncomeDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });
    $("#btnICR020").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/Income/ICR020_CreditNote?k=" + key + "&creditNoteNo=" + $("#incomeDocId").val());
        window.open(url, "IncomeDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnICR030").click(function () {
        var param = {
            strDocumentNo: $("#incomeDocId").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetICR030", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadICR030?k=" + key);
                window.open(url, "IncomeDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnICR040").click(function () {
        var param = {
            strDocumentNo: $("#incomeDocId").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_GetICR040", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Common/CMS999_DownloadICR040?k=" + key);
                window.open(url, "IncomeDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnGenICR010").click(function () {
        var obj = { receiptNo: $("#incomeDocId").val() };
        call_ajax_method_json("/Income/ICR010_GenerateReport", obj, null);
    });


    /*-----------------------------------------*/
    /*--------- Test Installation Report ----------*/
    /*-----------------------------------------*/
    $("#btnISR010").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var link = ajax_method.GenerateURL("/Common/CMS999_DownloadISR010" + "?strSlipNo=" + $("#txtInstallNo").val() + "&k=" + key);
        window.open(link, "InstallationDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });
    $("#btnISR020").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var link = ajax_method.GenerateURL("/Common/CMS999_DownloadISR020" + "?strSlipNo=" + $("#txtInstallNo").val() + "&k=" + key);
        window.open(link, "InstallationDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });
    $("#btnISR030").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var link = ajax_method.GenerateURL("/Common/CMS999_DownloadISR030" + "?strSlipNo=" + $("#txtInstallNo").val() + "&k=" + key);
        window.open(link, "InstallationDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });
    $("#btnISR040").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var link = ajax_method.GenerateURL("/Common/CMS999_DownloadISR040" + "?strSlipNo=" + $("#txtInstallNo").val() + "&k=" + key);
        window.open(link, "InstallationDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });
    $("#btnISRReprint").click(function () {
        ajax_method.CallScreenController("/Common/CMS999_ISRReprint", null, function (result, controls) {
            if (result) {
                OpenInformationMessageDialog("------", "Done");
            }
        });
    });
    $("#btnISR050").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var link = ajax_method.GenerateURL("/Common/CMS999_DownloadISR050" + "?strMaintenanceNo=" + $("#txtInstallNo").val() + "&strSubcontractorCode=" + $("#txtSubNo").val() + "&k=" + key);
        window.open(link, "InstallationDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });
    $("#btnISR060").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var link = ajax_method.GenerateURL("/Common/CMS999_DownloadISR060" + "?strMaintenanceNo=" + $("#txtInstallNo").val() + "&k=" + key);
        window.open(link, "InstallationDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });
    $("#btnISR070").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var link = ajax_method.GenerateURL("/Common/CMS999_DownloadISR070" + "?strMaintenanceNo=" + $("#txtInstallNo").val() + "&strSubcontractorCode=" + $("#txtSubNo").val() + "&k=" + key);
        window.open(link, "InstallationDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });
    $("#btnISR080").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var link = ajax_method.GenerateURL("/Common/CMS999_DownloadISR080" + "?strMaintenanceNo=" + $("#txtInstallNo").val() + "&strSubcontractorCode=" + $("#txtSubNo").val() + "&k=" + key);
        window.open(link, "InstallationDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });
    $("#btnISR090").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var link = ajax_method.GenerateURL("/Common/CMS999_DownloadISR090" + "?strSlipNo=" + $("#txtInstallNo").val() + "&k=" + key);
        window.open(link, "InstallationDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });
    $("#btnISR100").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var link = ajax_method.GenerateURL("/Common/CMS999_DownloadISR100" + "?strMaintenanceNo=" + $("#txtInstallNo").val() + "&strSubcontractorCode=" + $("#txtSubNo").val() + "&k=" + key);
        window.open(link, "InstallationDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });
    $("#btnISR110").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var link = ajax_method.GenerateURL("/Common/CMS999_DownloadISR110" + "?strSlipNo=" + $("#txtInstallNo").val() + "&k=" + key);
        window.open(link, "InstallationDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });
    $("#btnISR111").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var link = ajax_method.GenerateURL("/Common/CMS999_DownloadISR111" + "?strSlipNo=" + $("#txtInstallNo").val() + "&k=" + key);
        window.open(link, "InstallationDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnInstallSlip").click(function () {
        call_ajax_method_json("/Installation/GenerateInstallationSlipDocBySlipNo", null, null);
    });

    /*--------------------------------------------------------*/
    /*--------- End script test Installation Report ----------*/
    /*--------------------------------------------------------*/


    // ---------------------------------------------------------------------------------
    // Test IVP 
    // ---------------------------------------------------------------------------------

    $("#btnTestIVP010").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/SwtIVP010/index?k=" + key);
        window.open(url, "InventoryProcess_010", "scrollbars=no,menubar=yes,height=450,width=450,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnTestIVP100").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/SwtIVP100/index?k=" + key);
        window.open(url, "InventoryProcess_100", "scrollbars=no,menubar=yes,height=450,width=450,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnTestIVP020").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/SwtIVP020/index?k=" + key);
        window.open(url, "InventoryProcess_020", "scrollbars=no,menubar=yes,height=450,width=450,resizable=yes,toolbar=no,location=no,status=yes");
    });

    $("#btnTestIVP130").click(function () {
        var key = ajax_method.GetKeyURL(null);
        var url = ajax_method.GenerateURL("/SwtIVP130/index?k=" + key);
        window.open(url, "InventoryProcess_130", "scrollbars=no,menubar=yes,height=450,width=450,resizable=yes,toolbar=no,location=no,status=yes");
    });

    // ---------------------------------------------------------------------------------
    // End Test IVP 
    // ---------------------------------------------------------------------------------



    if ($.find("#TT060_ResultListGrid").length > 0) {
        bls060_grid = $("#TT060_ResultListGrid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Billing/BLS060_InitialByBillingDetailGrid", function () {

            SpecialGridControl(bls060_grid, ["ContractCode", "BillingOCC", "RunningNo", "Del"]);
            BindOnLoadedEvent(bls060_grid, function () {
                //var colInx = gridCheckingDetail.getColIndexById('BtnRemove');

                var defStringVal = "";

                for (var i = 0; i < bls060_grid.getRowsNum(); i++) {
                    var rowId = bls060_grid.getRowId(i);

                    var clt_ContractCode = "#" + GenerateGridControlID("txtContractCode", rowId);
                    $(clt_ContractCode).unbind("focus");


                    //GenerateTextBox(bls060_grid, "txtContractCode", rowId, "ContractCode", defStringVal, true);
                    //GenerateTextBox(bls060_grid, "txtBillingOCC", rowId, "BillingOCC", defStringVal, true);
                    //GenerateNumericBox2(bls060_grid, "txtRunningNo", rowId, "RunningNo", "", 4, 0, 1, 9999, 0, true);

                    //GenerateRemoveButton(bls060_grid, "btnRemove", rowId, "Del", true);
                    ////BindGridButtonClickEvent("btnDownload", rowId, Download_click);

                }

                var rowId = bls060_grid.getRowId(bls060_grid.getRowsNum() - 1);
                GenerateTextBox(bls060_grid, "txtContractCode", rowId, "ContractCode", defStringVal, true);
                GenerateTextBox(bls060_grid, "txtBillingOCC", rowId, "BillingOCC", defStringVal, true);
                GenerateNumericBox2(bls060_grid, "txtRunningNo", rowId, "RunningNo", "", 4, 0, 1, 9999, 0, true);

                GenerateRemoveButton(bls060_grid, "btnRemove", rowId, "Del", true);
                BindGridButtonClickEvent("btnRemove", rowId, BLS030DeleteRow);

                var clt_ContractCode = "#" + GenerateGridControlID("txtContractCode", rowId);
                $(clt_ContractCode).focus(AddRow);

                bls060_grid.setSizes();
            });
        });
    }

    $("#tbn060_add").click(function () {
        var row = ["", "", "", "", "", "", "", ""];
        CheckFirstRowIsEmpty(bls060_grid, true);
        AddNewRow(bls060_grid, row);


    });


    $("#tbn060_clear").click(function () {
        DeleteAllRow(bls060_grid);

    });


    $("#btnTestIVP").click(function () {
        var param = $("#frmTestIVP").serializeObject2();
        $("#txtResultIVP").text("Executing... \n" + param.procname);

        ajax_method.CallScreenController("/Common/CMS999_TestIVP", param, function (result, controls) {
            $("#txtResultIVP").text(JSON.stringify(result));
        });
    });

    $("#btnGenTempFile").click(function () {
        var obj = {
            numOfFiles: parseInt($("#txtTempfileNo").val())
        };

        ajax_method.CallScreenController("/Common/CMS999_GenerateTempFile", obj, function (result, controls) {
            if (result != undefined) {
                alert("Finish");
            }
        });
    });

    $("#btnIVP140").click(function () {
        var param = {
            batchdate: $("#IVP140_batchdatetime").val()
        };

        ajax_method.CallScreenController("/Common/CMS999_RunIVP140", param, function (result, controls) {
            if (result != undefined) {
                alert("Finish");
            }
        });
    });

});

function AddRow() {
    var row = ["", "", "", "", "", "", "", ""];
    CheckFirstRowIsEmpty(bls060_grid, true);
    AddNewRow(bls060_grid, row);

    if (bls060_grid.getRowsNum() > 1) {
        var rowId = bls060_grid.getRowId(bls060_grid.getRowsNum() - 2);
        var clt_ContractCode = "#" + GenerateGridControlID("txtContractCode", rowId);
        $(clt_ContractCode).focus();

    }
}

function BLS030DeleteRow(rowId) {
    DeleteRow(bls060_grid, rowId);
}

