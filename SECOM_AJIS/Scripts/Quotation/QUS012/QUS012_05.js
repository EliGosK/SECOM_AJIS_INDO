/* --- Initial ------------------------------------------------------ */
/* ------------------------------------------------------------------ */
function QUS012_05_InitialGrid() {
    if ($("#gridMaintenanceDetail").length > 0) {
        var pageRow = 0;
        var gridMaintenance = $("#gridMaintenanceDetail").LoadDataToGridWithInitial(pageRow, false, false,
                                "/Quotation/QUS012_GetMaintenanceDetailData", "", "View_doContractHeader", false);

        SpecialGridControl(gridMaintenance, ["Detail"]);

        BindOnLoadedEvent(gridMaintenance, function (gen_ctrl) {
            for (var i = 0; i < gridMaintenance.getRowsNum(); i++) {
                var row_id = gridMaintenance.getRowId(i);
                var sCol = gridMaintenance.getColIndexById("ShowDetail");

                if (gen_ctrl == true) {
                    var enable = false;
                    if (gridMaintenance.cells(row_id, sCol).getValue() == 1)
                        enable = true;
                    GenerateDetailButton(gridMaintenance, "btnDetail", row_id, "Detail", enable);
                }

                BindGridButtonClickEvent("btnDetail", row_id, function (rid) {
                    var codeCol = gridMaintenance.getColIndexById("ContractCodeShort");
                    var ptCol = gridMaintenance.getColIndexById("ProductTypeCode");

                    var obj = {
                        strContractCode: gridMaintenance.cells(rid, codeCol).getValue()
                    };
                    if (gridMaintenance.cells(rid, ptCol).getValue() == $("#C_PROD_TYPE_SALE").val()) {
                        ajax_method.CallScreenControllerWithAuthority("/Common/CMS160", obj, true);
                    }
                    else {
                        ajax_method.CallScreenControllerWithAuthority("/Common/CMS140", obj, true);
                    }
                });
            }
            gridMaintenance.setSizes();
        });
    }
}
/* ------------------------------------------------------------------ */