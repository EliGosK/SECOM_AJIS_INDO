/* --- Initial ------------------------------------------------------ */
/* ------------------------------------------------------------------ */
function QUS012_09_InitialGrid() {
    var gridFacility = $("#gridFacilityDetail").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, true, true,
                                "/Quotation/QUS012_GetFacilityDetailData", "", "doFacilityDetail", false);
}
/* ------------------------------------------------------------------ */