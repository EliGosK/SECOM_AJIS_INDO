/* --- Initial ------------------------------------------------------ */
/* ------------------------------------------------------------------ */
$(document).ready(function () {
    ShowSaleInformation(false);

    $("span[id=aLess]").click(aLess_click);
    $("span[id=aMore]").click(aMore_click);
});
/* ------------------------------------------------------------------ */

/* --- Events ------------------------------------------------------- */
/* ------------------------------------------------------------------ */
function aLess_click() {
    ShowSaleInformation(false);
    return false;
}
function aMore_click() {
    ShowSaleInformation(true);
    return false;
}
/* ------------------------------------------------------------------ */

/* --- Methods ------------------------------------------------------ */
/* ------------------------------------------------------------------ */
function ShowSaleInformation(show) {
    if (show) {
        $("span[id=aLess]").show();
        $("span[id=aMore]").hide();
        $("div[id=divSaleMore]").show();
    }
    else {
        $("span[id=aLess]").hide();
        $("span[id=aMore]").show();
        $("div[id=divSaleMore]").hide();
    }
}
/* ------------------------------------------------------------------ */