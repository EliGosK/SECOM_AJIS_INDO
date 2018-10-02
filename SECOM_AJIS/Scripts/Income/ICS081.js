var ics081resultGrid;

var ICS081 = {
    ScreenMode: {
        Initial: "InitialMode",
        SearchBillingByCode: "SearchBillingByCodeMode",
        SearchBillingByCriteria: "SearchBillingByCriteriaMode"
    }
};

$(document).ready(function () {
    //Call Only one
    ics081InitialPage();
    ics081InitialGrid();
    ics081InitialBindingEvent();

    //Start Screen Mode
    ics081SetScreenMode(ICS081.ScreenMode.Initial);
}); 

//UI Common
function ics081InitialPage() {
    InitialDateFromToControl("#IssueInvoiceDateFrom", "#IssueInvoiceDateTo");
    InitialDateFromToControl("#ExpectedPaymentDateFrom", "#ExpectedPaymentDateTo");

    $("#InvoiceAmountFrom").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#InvoiceAmountTo").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#BillingDetailAmountFrom").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#BillingDetailAmountTo").BindNumericBox(12, 2, 0, 999999999999.99);

}

//Grid
function ics081InitialGrid() {
    if ($.find("#ResultGrid").length > 0) {
        ics081resultGrid = $("#ResultGrid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Income/ICS081_InitialResultGrid");
        ics081BindingGridEvent();
    }
}

//Binding Event
function ics081InitialBindingEvent() {
    //Button
    $("#btnSelectProcess").click(SelectSearchProcess);
    $("#btnSearchByCode").click(SerachBillingByCode);
    $("#btnClearByCode").click(ClearFormSerachBillingByCode);
    $("#btnSearchByCriteria").click(SerachBillingByCriteria);
    $("#btnClearByCriteria").click(ClearFormSerachBillingByCriteria);

    //Radio
    $("#rdoBillingTargetCode").change(SearchBillingByCodeChange);
    $("#rdoCutomerCode").change(SearchBillingByCodeChange); //Add by Jutarat A. on 09042013
    $("#rdoInvoiceNo").change(SearchBillingByCodeChange);
    $("#rdoBillingCode").change(SearchBillingByCodeChange);
    $("#rdoReceiptNo").change(SearchBillingByCodeChange);
}

function SearchBillingByCodeChange() {
    if ($("#rdoBillingTargetCode").prop("checked") == true) {
        $("#BillingTargetCode1").removeAttr("readonly");
        $("#BillingTargetCode2").removeAttr("readonly");
        $("#ics081CutomerCode").attr("readonly", true); //Add by Jutarat A. on 09042013
        $("#ics081InvoiceNo").attr("readonly", true);
        $("#BillingCode1").attr("readonly", true);
        $("#BillingCode2").attr("readonly", true);
        $("#ics081ReceiptNo").attr("readonly", true);
    }
    //Add by Jutarat A. on 09042013
    else if ($("#rdoCutomerCode").prop("checked") == true) {
        $("#BillingTargetCode1").attr("readonly", true);
        $("#BillingTargetCode2").attr("readonly", true);
        $("#ics081CutomerCode").removeAttr("readonly");
        $("#ics081InvoiceNo").attr("readonly", true);
        $("#BillingCode1").attr("readonly", true);
        $("#BillingCode2").attr("readonly", true);
        $("#ics081ReceiptNo").attr("readonly", true);
    }
    //End Add
    else if ($("#rdoInvoiceNo").prop("checked") == true) {
        $("#BillingTargetCode1").attr("readonly", true);
        $("#BillingTargetCode2").attr("readonly", true);
        $("#ics081CutomerCode").attr("readonly", true); //Add by Jutarat A. on 09042013
        $("#ics081InvoiceNo").removeAttr("readonly");
        $("#BillingCode1").attr("readonly", true);
        $("#BillingCode2").attr("readonly", true);
        $("#ics081ReceiptNo").attr("readonly", true);
    }
    else if ($("#rdoBillingCode").prop("checked") == true) {
        $("#BillingTargetCode1").attr("readonly", true);
        $("#BillingTargetCode2").attr("readonly", true);
        $("#ics081CutomerCode").attr("readonly", true); //Add by Jutarat A. on 09042013
        $("#ics081InvoiceNo").attr("readonly", true);
        $("#BillingCode1").removeAttr("readonly");
        $("#BillingCode2").removeAttr("readonly");
        $("#ics081ReceiptNo").attr("readonly", true);
    }
    else if ($("#rdoReceiptNo").prop("checked") == true) {
        $("#BillingTargetCode1").attr("readonly", true);
        $("#BillingTargetCode2").attr("readonly", true);
        $("#ics081CutomerCode").attr("readonly", true); //Add by Jutarat A. on 09042013
        $("#ics081InvoiceNo").attr("readonly", true);
        $("#BillingCode1").attr("readonly", true);
        $("#BillingCode2").attr("readonly", true);
        $("#ics081ReceiptNo").removeAttr("readonly");
    }
}

function SelectSearchProcess() {
    //Get search method, Set screen mode
    if ($("#rdoSearchByBillingCode").prop("checked") == true) {
        ics081SetScreenMode(ICS081.ScreenMode.SearchBillingByCode);
    }
    else if ($("#rdoSearchByBillingCriteria").prop("checked") == true) {
        ics081SetScreenMode(ICS081.ScreenMode.SearchBillingByCriteria);
    }
    else {
        var objMsg = {
            module: "Income",
            code: "MSG7067"
        };
        call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
            OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () { });
        });
    }
}

function SerachBillingByCode() {
    $("#btnSearchByCode").attr("disabled", true);
    DeleteAllRow(ics081resultGrid);
    CloseWarningDialog();
    $("#divSearchBillingByCodeDetail").ResetToNormalControl();

    var obj = {};
    var action = "";

    if ($("#rdoBillingTargetCode").prop("checked") == true) {
        obj = { billingTargetCode: $.trim($("#BillingTargetCode1").val()) + '-' + $.trim($("#BillingTargetCode2").val()) }
        action = "/Income/ICS081_SearchBillingbyBillingTargetCode";
    }
    //Add by Jutarat A. on 09042013
    else if ($("#rdoCutomerCode").prop("checked") == true) {
        obj = { customerCode: $.trim($("#ics081CutomerCode").val()) }
        action = "/Income/ICS081_SearchBillingbyCustomerCode";
    }
    //End Add
    else if ($("#rdoInvoiceNo").prop("checked") == true) {
        obj = { invoiceNo: $.trim($("#ics081InvoiceNo").val()) }
        action = "/Income/ICS081_SearchBillingbyInvoiceNo";
    }
    else if ($("#rdoBillingCode").prop("checked") == true) {
        obj = { billingCode: $.trim($("#BillingCode1").val()) + '-' + $.trim($("#BillingCode2").val()) }
        action = "/Income/ICS081_SearchBillingbyBillingCode";
    }
    else if ($("#rdoReceiptNo").prop("checked") == true) {
        obj = { receiptNo: $.trim($("#ics081ReceiptNo").val()) }
        action = "/Income/ICS081_SearchBillingbyReceiptNo";
    }

    if (action != "") {
        //Process
        $("#ResultGrid").LoadDataToGrid(ics081resultGrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, action, obj, "doGetUnpaidBillingTargetByCodeWithExchange", false,
                    function (result, controls, isWarning) { // post-load
                        $("#btnSearchByCode").removeAttr("disabled");

                        if (controls != undefined) {
                            VaridateCtrl(controls, controls);
                        }
                        else {
                            document.getElementById('ResultGrid').scrollIntoView();
                        }
                    },
                    function (result, controls, isWarning) {
                        if (isWarning == undefined) {
                            $("#divResultList").show();
                        }
                    });
    }
    else {
        var objMsg = {
            module: "Income",
            code: "MSG7068"
        };
        call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
            OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {});
        });
    }
}

function SerachBillingByCriteria() {
    $("#btnSearchByCriteria").attr("disabled", true);
    DeleteAllRow(ics081resultGrid);
    CloseWarningDialog();
    $("#divSearchBillingByCriteriaDetail").ResetToNormalControl();


    var obj = CreateObjectData($("#formSearchBillingByCriteria").serialize());
    $("#ResultGrid").LoadDataToGrid(ics081resultGrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Income/ICS081_SearchBillingByCriteria", obj, "doGetUnpaidBillingTargetByCodeWithExchange", false,
                    function (result, controls, isWarning) { // post-load
                        $("#btnSearchByCriteria").removeAttr("disabled");

                        if (controls != undefined) {
                            VaridateCtrl(controls, controls);
                        }
                        else {
                            document.getElementById('ResultGrid').scrollIntoView();
                        }
                    },
                    function (result, controls, isWarning) {
                        if (isWarning == undefined) {
                            $("#divResultList").show();
                        }
                    });
}

function ClearFormSerachBillingByCode() {
    $("#divSearchBillingByCode").clearForm();
    CloseWarningDialog();
    $("#divResultList").hide();
    DeleteAllRow(ics081resultGrid);

    //set default
    $("#rdoBillingTargetCode").prop("checked", true);
    SearchBillingByCodeChange();
}

function ClearFormSerachBillingByCriteria() {
    $("#divSearchBillingByCriteria").clearForm();
    ClearDateFromToControl("#IssueInvoiceDateFrom", "#IssueInvoiceDateTo");
    ClearDateFromToControl("#ExpectedPaymentDateFrom", "#ExpectedPaymentDateTo");
    CloseWarningDialog();
    $("#divResultList").hide();
    DeleteAllRow(ics081resultGrid);
}

function ics081BindingGridEvent() {
    SpecialGridControl(ics081resultGrid, ["Select"]);

    BindOnLoadedEvent(ics081resultGrid, function () {
        var colInx = ics081resultGrid.getColIndexById('Select');
        var colBillingTargetCode = ics081resultGrid.getColIndexById('BillingTargetCode');
        var colUnpaidInvoice = ics081resultGrid.getColIndexById('UnpaidInvoice');
        var colUnpaidDetail = ics081resultGrid.getColIndexById('UnpaidDetail');

        for (var i = 0; i < ics081resultGrid.getRowsNum(); i++) {
            var rowId = ics081resultGrid.getRowId(i);

            //Select Image Button
            GenerateSelectButton(ics081resultGrid, "btnSelect", rowId, "Select", true);
            BindGridButtonClickEvent("btnSelect", rowId, ProcessBilling);

            var billingTargetCode = ics081resultGrid.cells2(i, colBillingTargetCode).getValue();

            //Unpaid invoice Dialog
            var unpaidInvoiceValue = ics081resultGrid.cells2(i, colUnpaidInvoice).getValue();
            if (unpaidInvoiceValue != "" && unpaidInvoiceValue != "0") {
                var tagAics082 = "<a href='#'>" + unpaidInvoiceValue + "<input type='hidden' name='callScreenID' value='ICS082'/><input type='hidden' name='billingTargetCode' value='" + billingTargetCode + "'/></a>";
                ics081resultGrid.cells2(i, colUnpaidInvoice).setValue(tagAics082);
            }
            //Unpaid detail Dialog
            var unpaidDetailValue = ics081resultGrid.cells2(i, colUnpaidDetail).getValue();
            if (unpaidDetailValue != "" && unpaidDetailValue != "0") {
                var tagAics083 = "<a href='#'>" + unpaidDetailValue + "<input type='hidden' name='callScreenID' value='ICS083'/><input type='hidden' name='billingTargetCode' value='" + billingTargetCode + "'/></a>";
                ics081resultGrid.cells2(i, colUnpaidDetail).setValue(tagAics083);
            }
        }

        //Binding Event for TAG A
        $("#ResultGrid a").each(function () {
            $(this).click(function () {
                var callScreenID = $(this).children("input:hidden[name=callScreenID]").val();

                if (callScreenID == "ICS082") {
                    var obj = {
                        BillingTargetCode: $(this).children("input:hidden[name=billingTargetCode]").val()
                    };
                    $("#ics081dlgBox").OpenICS082Dialog("ICS081", obj);
                }
                else if (callScreenID == "ICS083") {
                    var obj = {
                        BillingTargetCode: $(this).children("input:hidden[name=billingTargetCode]").val()
                    };
                    $("#ics081dlgBox").OpenICS083Dialog("ICS081", obj);
                }

            });
        });
        ics081resultGrid.setSizes();
    });
}


function ProcessBilling(rowId) {
    var rownum = ics081resultGrid.getRowIndex(rowId);
    ics081resultGrid.selectRow(rownum);
    var billingTargetCode = ics081resultGrid.cells2(rownum, ics081resultGrid.getColIndexById('BillingTargetCode')).getValue();
    var paymentTransNo = $("#ics081PaymentTransNo").val();
    var obj = {
        ScreenCaller: "ICS081",
        PaymentTransNo: paymentTransNo,
        BillingTargetCode: billingTargetCode
    };

    ajax_method.CallScreenController("/Income/ICS081_MatchPaymentNextStep", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            if (result == "ICS084") {

                //Check permission at this point to support multiload (page), to prevent blank screen.
                ajax_method.CallScreenController("/Income/ICS084_Authority", obj, function (result, controls, isWarning) {
                    if (result != undefined) {
                        ajax_method.CallScreenController("/Income/ICS084_SetScreenData", obj, function (result, controls, isWarning) {
                            SetScreenPageMode(ICS080_Constant.ScreenPageMode.ICS084);
                        });
                    }
                });
            }
        }
        if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });
}



function ics081SetScreenMode(mode) {
    if (mode == ICS081.ScreenMode.Initial) {
        $("#divSearchBillingByCode").hide();
        $("#divSearchBillingByCriteria").hide();
        $("#divResultList").hide();

        //Call ics080.js
        if (typeof (ics080ShowBackButton) == "function") {
            ics080ShowBackButton();
        }
    }
    else if (mode == ICS081.ScreenMode.SearchBillingByCode) {
        $("#divSearchBillingByCode").show();
        $("#divSearchBillingByCriteria").hide();
        $("#divResultList").hide();
    }
    else if (mode == ICS081.ScreenMode.SearchBillingByCriteria) {
        $("#divSearchBillingByCode").hide();
        $("#divSearchBillingByCriteria").show();
        $("#divResultList").hide();
    }
}