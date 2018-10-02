/* --- Initial ------------------------------------------------------ */
/* ------------------------------------------------------------------ */
function QUS012_06_InitialGrid() {
    var gridInstBefore = $("#gridInstrumentDetailBefore").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, true, true,
                                "/Quotation/QUS012_GetBeforeInstrumentDetailData", "", "doInstrumentDetail", false);

    BindOnLoadedEvent(gridInstBefore, function (gen_ctrl) {
        for (var i = 0; i < gridInstBefore.getRowsNum(); i++) {
            if (gen_ctrl == true) {
                if (gridInstBefore.cells2(i, 1).getValue() == 1)
                    gridInstBefore.cells2(i, 1).setValue(GenerateStarImage());
                else
                    gridInstBefore.cells2(i, 1).setValue("");
            }
        }
        gridInstBefore.setSizes();
    });
}
/* ------------------------------------------------------------------ */