/* --- Initial ------------------------------------------------------ */
/* ------------------------------------------------------------------ */
function QUS012_11_InitialGrid() {
    var gridSentryGuard = $("#gridSentryGuardDetail").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, true, true,
                                "/Quotation/QUS012_GetSentryGuardDetailData", "", "doSentryGuardDetail", false);
}
/* ------------------------------------------------------------------ */