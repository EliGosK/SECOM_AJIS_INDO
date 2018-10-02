/// <reference path="../../Base/GridControl.js" />
var gridInst = null;

function CTS010_03_InitialGrid() {
    gridInst = $("#gridInstrumentDetail").LoadDataToGridWithInitial(0, false, false,
                                "/Contract/CTS010_GetInstrumentDetailData",
                                "",
                                "doInstrumentDetail", false);
}