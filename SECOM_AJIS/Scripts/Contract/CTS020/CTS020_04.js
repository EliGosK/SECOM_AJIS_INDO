/// <reference path="../../Base/GridControl.js" />

/* --- Variable ------------------------------------------------------- */
/* -------------------------------------------------------------------- */
var gridInst = null;
/* -------------------------------------------------------------------- */

/* --- Initial -------------------------------------------------------- */
/* -------------------------------------------------------------------- */
$(document).ready(function () {
    InitialInstrumentDetail();
});
function InitialInstrumentDetail() {
    /* --- Initial Grid --- */
    /* -------------------- */
    gridInst = $("#gridInstrumentDetail").LoadDataToGridWithInitial(0, false, false,
                                "/Contract/CTS020_GetInstrumentDetailData",
                                "",
                                "doInstrumentDetail", false);
    /* -------------------- */
}
/* -------------------------------------------------------------------- */