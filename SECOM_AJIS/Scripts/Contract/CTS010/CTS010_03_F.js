/// <reference path="../../Base/GridControl.js" />
var gridInst = null;

function CTS010_03_F_InitialGrid() {
    gridInst = $("#gridFacilityDetailC").LoadDataToGridWithInitial(0, false, false,
                                "/Contract/CTS010_GetFacilityDetailData",
                                "",
                                "doInstrumentDetail", false);
}