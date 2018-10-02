/* --- Initial ------------------------------------------------------ */
/* ------------------------------------------------------------------ */
function QUS012_07_InitialGrid() {
    var gridInstAfter = $("#gridInstrumentDetailAfter").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, true, true,
                                "/Quotation/QUS012_GetAfterInstrumentDetailData", "", "doInstrumentDetail", false);

    BindOnLoadedEvent(gridInstAfter, function (gen_ctrl) {
        for (var i = 0; i < gridInstAfter.getRowsNum(); i++) {
            if (gen_ctrl == true) {
                if (gridInstAfter.cells2(i, 1).getValue() == 1)
                    gridInstAfter.cells2(i, 1).setValue(GenerateStarImage());
                else
                    gridInstAfter.cells2(i, 1).setValue("");
            }
        }
        gridInstAfter.setSizes();
    });
}
/* ------------------------------------------------------------------ */