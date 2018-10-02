/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {

});

function SetSectionModeCTS130_01(isView) {
    $("#divRentalContractBasicSection").SetViewMode(isView);

    if (isView == false) {
        $("#RT_ImportantFlag").attr("disabled", true);
    }
}
