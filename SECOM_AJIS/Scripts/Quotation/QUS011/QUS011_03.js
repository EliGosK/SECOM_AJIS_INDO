/* --- Initial ------------------------------------------------------ */
/* ------------------------------------------------------------------ */
function QUS011_03_InitialGrid() {
    var mygrid = $("#gridQUS011InstrumentDetail").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, true, true, "/Quotation/QUS011_GetInstrumentData", "", "doInstrumentDetail", false);

    SpecialGridControl(mygrid, ["MaintenanceFlag"]);

    BindOnLoadedEvent(mygrid, function (gen_ctrl) {
        var mCol = mygrid.getColIndexById("MaintenanceFlag");

        for (var i = 0; i < mygrid.getRowsNum(); i++) {
            if (gen_ctrl == true) {
                var flag = GetValueFromLinkType(mygrid, i, mCol);
                if (flag == 1)
                    mygrid.cells2(i, 1).setValue(GenerateStarImage());
                else if (flag == 0)
                    mygrid.cells2(i, 1).setValue("");
            }
        }
    });
}
/* ------------------------------------------------------------------ */