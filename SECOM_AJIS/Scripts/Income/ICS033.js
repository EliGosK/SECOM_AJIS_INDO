/// <reference path="../number-functions.js" />

var grdViewUnpaidBillingDetailListGrid;

$(document).ready(function () {
    $('#divInputData').SetViewMode(true);
    ICS033_InitialGrid();
});

function ICS033_InitialGrid() {
    if ($.find("#ICS033_ViewUnpaidBillingDetailListGrid").length > 0) {
        grdViewUnpaidBillingDetailListGrid = $("#ICS033_ViewUnpaidBillingDetailListGrid").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, false, false, "/Income/ICS033_GetViewUnpaidBillingDetailListGrid","","doGetUnpaidDetailDebtSummary", false);
        SpecialGridControl(grdViewUnpaidBillingDetailListGrid, ["ICS033_ViewUnpaidBillingDetailListGrid", "FirstFeeDetail", "CreditNoteIssueDetail", "TracingResaultRegistered"]);
        BindOnLoadedEvent(grdViewUnpaidBillingDetailListGrid, function () {
            var firstFeeFlagColInx = grdViewUnpaidBillingDetailListGrid.getColIndexById("FirstFeeFlag");
            var creditNoteIssuedFlagColInx = grdViewUnpaidBillingDetailListGrid.getColIndexById("CreditNoteIssuedFlag");
            var billingTypeCodeColInx = grdViewUnpaidBillingDetailListGrid.getColIndexById("BillingTypeCode");
            var billingTypeDescColInx = grdViewUnpaidBillingDetailListGrid.getColIndexById("BillingType");
            var paymentStatusCodeColInx = grdViewUnpaidBillingDetailListGrid.getColIndexById("PaymentStatus");
            var paymentStatusDescColInx = grdViewUnpaidBillingDetailListGrid.getColIndexById("PaymentStatusDesc");
            

            for (var i = 0; i < grdViewUnpaidBillingDetailListGrid.getRowsNum(); i++) {
                var rowId = grdViewUnpaidBillingDetailListGrid.getRowId(i);

                //Generate first fee icon
                var dummyFirstFee = grdViewUnpaidBillingDetailListGrid.cells(rowId, firstFeeFlagColInx).getValue();
                if (dummyFirstFee == '1') {
                    GenerateFirstFeeButton(grdViewUnpaidBillingDetailListGrid, "btnFirstFee", rowId, "FirstFeeDetail", true);
                }

                //Generate credit note issued icon
                var dummyResaultRegister = grdViewUnpaidBillingDetailListGrid.cells(rowId, creditNoteIssuedFlagColInx).getValue();
                if (dummyResaultRegister == '1') {
                    GenerateCreditNoteButton(grdViewUnpaidBillingDetailListGrid, "btnCreditNote", rowId, "CreditNoteIssueDetail", true);
                }

                // tool tip
                //--------------------------------------------------------------------
                var colToolTip1 = grdViewUnpaidBillingDetailListGrid.cells(rowId, billingTypeDescColInx).getValue()
                grdViewUnpaidBillingDetailListGrid.cellById(rowId, billingTypeCodeColInx).setAttribute("title", colToolTip1);

                var colToolTip2 = grdViewUnpaidBillingDetailListGrid.cells(rowId, paymentStatusDescColInx).getValue();
                grdViewUnpaidBillingDetailListGrid.cellById(rowId, paymentStatusCodeColInx).setAttribute("title", colToolTip2);
            }
            grdViewUnpaidBillingDetailListGrid.setSizes();
        });
    }
}

// Init Popup Screen Call
function ICS033Initial(objSendToICS033) {
    ChangeDialogButtonText(["Close"], [$('#ics033btnClose').val()]);
    BindDialogButtonClick($("#btnOK").val(), function () {
        ConfirmData();
    });

    if (objICS033 != undefined) {
        $("#txtBillingTragetCode").text(objICS033.BillingTargetCode);
        $("#txtBillingOfficeName").text(objICS033.BillingOfficeName);
        $("#txtBillingOfficeCode").text(objICS033.BillingOfficeCode);
        $("#txtBillingClientNameEN").text(objICS033.BillingClientNameEN);
        $("#txtBillingClientNameLC").text(objICS033.BillingClientNameLC);
        $("#txtBillingClientAddressEN").text(objICS033.BillingClientAddressEN);
        $("#txtBillingClientAddressLC").text(objICS033.BillingClientAddressLC);
        $("#txtBillingClientTelNo").text(objICS033.BillingClientTelNo);
        $("#txtBillingClientContactPerson").text(objICS033.ContactPersonName); 
        $("#txtTotalUnpaidAmount").text(objICS033.UnpaidAmountString);
        $("#txtTotalUnpaidAmountUs").text(objICS033.unpaidAmountUsString);
    }
}

function ConfirmData() {
    ICS033Response();
}