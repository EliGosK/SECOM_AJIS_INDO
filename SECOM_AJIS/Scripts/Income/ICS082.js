var ics082InvoiceGrid;

$(document).ready(function () {
    //Call Only one
    ICS082_InitialGrid();
}); 

//Grid
function ICS082_InitialGrid() {
    if ($.find("#ics082InvoiceGrid").length > 0) {
        ics082InvoiceGrid = $("#ics082InvoiceGrid").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, false, false, "/Income/ICS082_GetInvoiceGrid", "", "doUnpaidInvoice", false);
        ics082BindingGridEvent();
    }
}

function ICS082Initial() {
    ChangeDialogButtonText(["Close"], [$('#ics082btnClose').val()]);
}

function ics082BindingGridEvent() {
    BindOnLoadedEvent(ics082InvoiceGrid, function () {
        //Use long format
        var billingTargetCode = $("#ics082BillingTargetCodeLongFormat").val();

        var colInvoiceNo = ics082InvoiceGrid.getColIndexById('InvoiceNo');
        var colInvoiceOCC = ics082InvoiceGrid.getColIndexById('InvoiceOCC');
        var colNoOfBillingDetail = ics082InvoiceGrid.getColIndexById('NoOfBillingDetail');

        for (var i = 0; i < ics082InvoiceGrid.getRowsNum(); i++) {
            var rowId = ics082InvoiceGrid.getRowId(i);

            //Unpaid detail Dialog
            var unpaidDetailValue = ics082InvoiceGrid.cells2(i, colNoOfBillingDetail).getValue();

            if (unpaidDetailValue != "" && unpaidDetailValue != "0") {
                var invoiceNo = ics082InvoiceGrid.cells2(i, colInvoiceNo).getValue();
                var invoiceOCC = ics082InvoiceGrid.cells2(i, colInvoiceOCC).getValue();
                var tagAics083 = "<a href='#'>" + unpaidDetailValue + "<input type='hidden' name='callScreenID' value='ICS083'/><input type='hidden' name='billingTargetCode' value='" + billingTargetCode + "'/><input type='hidden' name='InvoiceNo' value='" + invoiceNo + "'/><input type='hidden' name='InvoiceOCC' value='" + invoiceOCC + "'/></a>";
                ics082InvoiceGrid.cells2(i, colNoOfBillingDetail).setValue(tagAics083);
            }
        }

        //Binding Event for TAG A
        $("#ics082InvoiceGrid a").each(function () {
            $(this).click(function () {
                var callScreenID = $(this).children("input:hidden[name=callScreenID]").val();

                if (callScreenID == "ICS083") {
                    var obj = {
                        BillingTargetCode: $(this).children("input:hidden[name=billingTargetCode]").val(),
                        InvoiceNo: $(this).children("input:hidden[name=InvoiceNo]").val(),
                        InvoiceOCC: $(this).children("input:hidden[name=InvoiceOCC]").val()
                    };
                    $("#ics082dlgBox").OpenICS083Dialog("ICS082", obj);
                }
            });
        });
        ics082InvoiceGrid.setSizes();
    });
}