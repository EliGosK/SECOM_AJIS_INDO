/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {

});

function SetSectionModeCTS130_02(isView) {
    $("#divSaleContractBasicSection").SetViewMode(isView);

    if (isView == false) {
        $("#SL_PurchaserImportantFlag").attr("disabled", true);
    }
}