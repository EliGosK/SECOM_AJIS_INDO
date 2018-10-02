var gridInstOnline = null;

function QUS030_09_InitialGrid(fc) {
    /* --- Initial Grid --- */
    /* -------------------- */
    var f = false;
    if (fc != undefined)
        f = fc;

    var obj = {
        fromContract: f
    };

    gridInstOnline = $("#gridInstrumentDetailONLINE").LoadDataToGridWithInitial(0, false, false,
                                "/Quotation/QUS030_GetInstrumentDetailData_ONLINE",
                                obj,
                                "doInstrumentDetail", false);
    /* -------------------- */

    BindOnLoadedEvent(gridInstOnline, function (gen_ctrl) {
        for (var i = 0; i < gridInstOnline.getRowsNum(); i++) {
            var row_id = gridInstOnline.getRowId(i);
            var flagCol = gridInstOnline.getColIndexById("MaintenanceFlag");

            if (gen_ctrl == true) {
                /* --- Set Start --- */
                /* ----------------- */
                var flag = gridInstOnline.cells2(i, flagCol).getValue();
                if (flag == 1 || flag == true)
                    flag = GenerateStarImage();
                else
                    flag = "";
                gridInstOnline.cells2(i, flagCol).setValue(flag);
                /* ----------------- */
            }
        }
        gridInstOnline.setSizes();
    });
}
function SetQUS030_09_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode for Instrument detail section</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    $("#divInstSaleOnlineContractCode").SetViewMode(isview);
}


/* --- Initial -------------------------------------------------------- */
/* -------------------------------------------------------------------- */
function ResetGridInstrumentDetail_ONLINE() {
    if (gridInstOnline != null) {
        DeleteAllRow(gridInstOnline);
    }
}
/* -------------------------------------------------------------------- */