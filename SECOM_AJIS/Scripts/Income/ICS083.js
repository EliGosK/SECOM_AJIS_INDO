var ics083DetailGrid;

$(document).ready(function () {
    //Call Only one
    ICS083_InitialGrid();
});

//Grid
function ICS083_InitialGrid() {
    if ($.find("#ics083BillingDetailGrid").length > 0) {
        ics083DetailGrid = $("#ics083BillingDetailGrid").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, false, false, "/Income/ICS083_GetDetailGrid", "", "doUnpaidBillingDetail", false);
        ics083BindingGridEvent();
    }
}

function ICS083Initial() {
    ChangeDialogButtonText(["Close"], [$('#ics083btnClose').val()]);
}

function ics083BindingGridEvent() {
    BindOnLoadedEvent(ics083DetailGrid, function () {
        var billingTargetCode = $("#BillingTargetCode").val();

        var colFirstFeeFlag = ics083DetailGrid.getColIndexById('FirstFeeFlag');
        var colCreditNoteIssuedFlag = ics083DetailGrid.getColIndexById('CreditNoteIssuedFlag');

        for (var i = 0; i < ics083DetailGrid.getRowsNum(); i++) {
            var rowId = ics083DetailGrid.getRowId(i);

            var firstFeeFlag = ics083DetailGrid.cells2(i, colFirstFeeFlag).getValue();
            if (firstFeeFlag == true) {
                //Firstfee Image Button
                GenerateFirstFeeButton(ics083DetailGrid, "btnFirstFee", rowId, "FirstFee", true);
            }

            var creditNoteIssue = ics083DetailGrid.cells2(i, colCreditNoteIssuedFlag).getValue();
            if (creditNoteIssue == true) {
                //CreditNote Image Button
                GenerateCreditNoteButton(ics083DetailGrid, "btnCreditNote", rowId, "CreditNote", true);
            }
        }
        ics083DetailGrid.setSizes();
    });
}