/* --- Initial ------------------------------------------------------ */
/* ------------------------------------------------------------------ */
function QUS012_08_InitialGrid() {
    var gridInstOnline = $("#gridInstrumentDetailOnline").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, true, true,
                                "/Quotation/QUS012_GetOnlineInstrumentDetailData", "", "doInstrumentDetail", false);

    BindOnLoadedEvent(gridInstOnline, function (gen_ctrl) {
        for (var i = 0; i < gridInstOnline.getRowsNum(); i++) {
            if (gen_ctrl == true) {
                if (gridInstOnline.cells2(i, 1).getValue() == 1)
                    gridInstOnline.cells2(i, 1).setValue(GenerateStarImage());
                else
                    gridInstOnline.cells2(i, 1).setValue("");
            }
        }
        gridInstOnline.setSizes();
    });
}
/* ------------------------------------------------------------------ */