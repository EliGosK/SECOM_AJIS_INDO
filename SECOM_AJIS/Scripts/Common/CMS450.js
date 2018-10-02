/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

/// <reference path="../Base/object/ajax_method.js" />


var gridBillingDetailCMS450;
$(document).ready(function () {

    InitialBillingDetailGridData();
    IntialPage();

});


function IntialPage() {

    // Set null value to "-"  ***
    $("#divBillingTargetInformation").SetEmptyViewData();
}




function CMS450Initial() {

}

function InitialBillingDetailGridData() {

    gridBillingDetailCMS450 = $("#gridBillingDetail").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, true, true, "/Common/CMS450_ViewBillingDetailList",
    "", "dtViewBillingDetailList", false);

    gridBillingDetailCMS450.enableResizing("false");

    //-------- Generate Column image

    SpecialGridControl(gridBillingDetailCMS450, ["CreditNoteIssueDetail", "FirstFeeDetail", "TaxInvoiceIssuedDetail", "ReceiptIssuedDetail"]);
    BindOnLoadedEvent(gridBillingDetailCMS450,
        function (gen_ctrl) {
            if (CheckFirstRowIsEmpty(gridBillingDetailCMS450, false) == false) {
                var colCreditNoteFlag = gridBillingDetailCMS450.getColIndexById("CreditNoteFlag");
                var colFirstFeeFlag = gridBillingDetailCMS450.getColIndexById("FirstFeeFlag");
                var colCreditNoteInvoiceForTooltip = gridBillingDetailCMS450.getColIndexById("CreditNoteInvoiceForTooltip");

                // Add By Sommai P. Nov 5, 2013
                var colTaxInvoiceFlag = gridBillingDetailCMS450.getColIndexById("TaxInvoiceFlag");
                var colReceiptFlag = gridBillingDetailCMS450.getColIndexById("ReceiptFlag");
                var colTaxInvoiceForTooltip = gridBillingDetailCMS450.getColIndexById("TaxInvoiceForTooltip");
                var colReceiptForTooltip = gridBillingDetailCMS450.getColIndexById("ReceiptForTooltip");
                var colPaymentDateForTooltip = gridBillingDetailCMS450.getColIndexById("PostdateChequeForTooltip");
                var colPaymentDate = gridBillingDetailCMS450.getColIndexById("PaymentDate_Text");
                // End Add

                for (var i = 0; i < gridBillingDetailCMS450.getRowsNum(); i++) {
                    var rid = gridBillingDetailCMS450.getRowId(i);
                    //var imageColInx = gridBillingDetailCMS450.getColIndexById('CreditNoteIssueDetail');

                    //if (gen_ctrl == true) {
                    //----------- Generate image         
                    var creditFlag = gridBillingDetailCMS450.cells2(i, colCreditNoteFlag).getValue();
                    var firstFeeFlag = gridBillingDetailCMS450.cells2(i, colFirstFeeFlag).getValue();

                    // Add By Sommai P. Nov 5, 2013
                    var taxinvoiceFlag = gridBillingDetailCMS450.cells2(i, colTaxInvoiceFlag).getValue();
                    var receiptFlag = gridBillingDetailCMS450.cells2(i, colReceiptFlag).getValue();
                    var paymentDateForTooltip = gridBillingDetailCMS450.cells2(i, colPaymentDateForTooltip).getValue();
                    // End Add


                    if (firstFeeFlag == 1) {
                        // GenerateRemoveButton(gridBillingDetailCMS450, "imgFirstFee", rid, "FirstFeeDetail", true);
                        GenerateImageButtonToGrid(gridBillingDetailCMS450, "imgFirstFee", rid, "FirstFeeDetail", true, "F.png", "FirstFeeDetail")
                        var imgFirstFeeControl = GenerateGridControlID("imgFirstFee", rid);
                        // var firstFeeTooltip = gridBillingDetailCMS450.cells2(i, gridBillingDetailCMS450.getColIndexById("BillingCode_Short")).getValue();
                        $("#" + imgFirstFeeControl).attr("title", $("#lblFirstFeeDetail").val()).css("cursor", "default").css("width", "16px").css("height", "16px");
                    }
                    if (creditFlag == 1) {
                        //GenerateEditButton(gridBillingDetailCMS450, "imgCredit", rid, "CreditNoteIssueDetail", true);
                        GenerateImageButtonToGrid(gridBillingDetailCMS450, "imgCredit", rid, "CreditNoteIssueDetail", true, "C.png", "CreditNote")
                        var imgCreditControl = GenerateGridControlID("imgCredit", rid);
                        var creditTooltip = gridBillingDetailCMS450.cells2(i, colCreditNoteInvoiceForTooltip).getValue();
                        $("#" + imgCreditControl).attr("title", creditTooltip.replace(new RegExp("_", "gi"), "\r\n")).css("cursor", "default").css("width", "16px").css("height", "16px");
                        //creditTooltip.replace("\r\n", "\r\n")
                    }
                    //}

                    // Add By Sommai P. Nov 5, 2013
                    if (taxinvoiceFlag == 1) {
                        GenerateImageButtonToGrid(gridBillingDetailCMS450, "imgTaxinvoice", rid, "TaxInvoiceIssuedDetail", true, "T.png", "TaxInvoiceIssuedDetail")
                        var imgTaxinvoiceControl = GenerateGridControlID("imgTaxinvoice", rid);
                        var taxInvoiceTooltip = gridBillingDetailCMS450.cells2(i, colTaxInvoiceForTooltip).getValue();
                        $("#" + imgTaxinvoiceControl).attr("title", taxInvoiceTooltip.replace(new RegExp("_", "gi"), "\r\n")).css("cursor", "default").css("width", "16px").css("height", "16px");

                        BindGridButtonClickEvent("imgTaxinvoice", rid, function (rid) {
                            gridBillingDetailCMS450.selectRowById(rid);

                            var colTaxInvoiceForTooltip = gridBillingDetailCMS450.getColIndexById("TaxInvoiceForTooltip");
                            var taxInvoiceTooltip = gridBillingDetailCMS450.cells(rid, colTaxInvoiceForTooltip).getValue();
                            var title = taxInvoiceTooltip.replace(new RegExp("_", "gi"), "\r\n");
                            var taxinvno = (title && title.indexOf(", ") >= -1 ? title.substring(0, title.indexOf(", ")) : null);

                            var objParam = {
                                reportId: CMS450_ViewBag.C_REPORT_ID_TAX_INVOICE,
                                docNo: taxinvno
                            };
                            ajax_method.CallScreenController("/Common/CMS450_CheckExistFile", objParam, function (data) {

                                if (data != undefined) {
                                    if (data == "1") {
                                        var key = ajax_method.GetKeyURL(null);
                                        var link = ajax_method.GenerateURL("/Common/CMS450_DownloadDocument?reportId=" + CMS450_ViewBag.C_REPORT_ID_TAX_INVOICE + "&docNo=" + taxinvno + "&k=" + key);

                                        window.open(link, "download");
                                    }
                                    else {

                                        var param = { "module": "Common", "code": "MSG0112" };
                                        call_ajax_method_json("/Shared/GetMessage", param, function (data) {

                                            /* ====== Open info dialog =====*/
                                            OpenInformationMessageDialog(param.code, data.Message);
                                        });

                                    }
                                }
                            });

                        });
                    }

                    if (receiptFlag == 1) {
                        GenerateImageButtonToGrid(gridBillingDetailCMS450, "imgRecipt", rid, "ReceiptIssuedDetail", true, "R.png", "ReceiptIssuedDetail")
                        var imgReceiptControl = GenerateGridControlID("imgRecipt", rid);
                        var taxReceiptTooltip = gridBillingDetailCMS450.cells2(i, colReceiptForTooltip).getValue();
                        $("#" + imgReceiptControl).attr("title", taxReceiptTooltip.replace(new RegExp("_", "gi"), "\r\n")).css("cursor", "default").css("width", "16px").css("height", "16px");

                        BindGridButtonClickEvent("imgRecipt", rid, function (rid) {
                            gridBillingDetailCMS450.selectRowById(rid);

                            var colReceiptForTooltip = gridBillingDetailCMS450.getColIndexById("ReceiptForTooltip");
                            var receiptTooltip = gridBillingDetailCMS450.cells(rid, colReceiptForTooltip).getValue();
                            var title = receiptTooltip.replace(new RegExp("_", "gi"), "\r\n");
                            var receiptNo = (title && title.indexOf(", ") >= -1 ? title.substring(0, title.indexOf(", ")) : null);

                            var objParam = {
                                reportId: CMS450_ViewBag.C_REPORT_ID_RECEIPT,
                                docNo: receiptNo
                            };
                            ajax_method.CallScreenController("/Common/CMS450_CheckExistFile", objParam, function (data) {

                                if (data != undefined) {
                                    if (data == "1") {
                                        var key = ajax_method.GetKeyURL(null);
                                        var link = ajax_method.GenerateURL("/Common/CMS450_DownloadDocument?reportId=" + CMS450_ViewBag.C_REPORT_ID_RECEIPT + "&docNo=" + receiptNo + "&k=" + key);

                                        window.open(link, "download");
                                    }
                                    else {

                                        var param = { "module": "Common", "code": "MSG0112" };
                                        call_ajax_method_json("/Shared/GetMessage", param, function (data) {

                                            /* ====== Open info dialog =====*/
                                            OpenInformationMessageDialog(param.code, data.Message);
                                        });

                                    }
                                }
                            });

                        });
                    }

                    var colInvoiceNo = gridBillingDetailCMS450.getColIndexById("InvoiceNo_Text");
                    var invoiceNo = gridBillingDetailCMS450.cells(rid, colInvoiceNo).getValue();
                    var tagInvoiceNo = "<a id='" + GenerateGridControlID("InvoiceNo_Text", rid) + "' href='#'>" + invoiceNo + "<input type='hidden' name='InvoiceNo' value='" + invoiceNo + "'/></a>";
                    gridBillingDetailCMS450.cells(rid, colInvoiceNo).setValue(tagInvoiceNo);


                    if (paymentDateForTooltip != null && paymentDateForTooltip != '') {
                        //gridBillingDetailCMS450.cellById(rid, colPaymentDate).setAttribute("title", paymentDateForTooltip);
                        gridBillingDetailCMS450.cellById(rid, colPaymentDate).setAttribute("title", paymentDateForTooltip.replace(new RegExp("_", "gi"), "\r\n")); //Modify by Jutarat A. on 18122013
                    }
                    // End Add
                }

                $("a[id^=InvoiceNo_Text]")
                .unbind("click")
                .click(function () {
                    var invoiceNo = $(this).children("input:hidden[name=InvoiceNo]").val()

                    var objParam = {
                        reportId: CMS450_ViewBag.C_REPORT_ID_INVOICE,
                        docNo: invoiceNo
                    };
                    ajax_method.CallScreenController("/Common/CMS450_CheckExistFile", objParam, function (data) {

                        if (data != undefined) {
                            if (data == "1") {
                                var key = ajax_method.GetKeyURL(null);
                                var link = ajax_method.GenerateURL("/Common/CMS450_DownloadDocument?reportId=" + CMS450_ViewBag.C_REPORT_ID_INVOICE + "&docNo=" + invoiceNo + "&k=" + key);

                                window.open(link, "download");
                            }
                            else {

                                var param = { "module": "Common", "code": "MSG0112" };
                                call_ajax_method_json("/Shared/GetMessage", param, function (data) {

                                    /* ====== Open info dialog =====*/
                                    OpenInformationMessageDialog(param.code, data.Message);
                                });

                            }
                        }
                    });
                });


                //$("#chkHeader").unbind("click");
                //$("#chkHeader").click(selectAllCheckboxControl);
                gridBillingDetailCMS450.setSizes();
            }



            $('#gridBillingDetail_grid').css('width', '1930px');
            $('#gridBillingDetail_grid').resize(function () {
                $(this).css('width', '1930px');
            });

           
        });
}