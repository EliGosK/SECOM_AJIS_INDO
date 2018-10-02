/* --- Initial -------------------------------------------------------- */
/* -------------------------------------------------------------------- */
$(document).ready(function () {
    $("#btnRegisterNextQuotationDetail").click(register_next_quotation_detail_click);
    $("#btnRegisterNextQuotationDetail").focus();
});
/* -------------------------------------------------------------------- */

/* --- Event ---------------------------------------------------------- */
/* -------------------------------------------------------------------- */
function register_next_quotation_detail_click() {
    //    ajax_method.CallScreenController("/Quotation/ResetSessionData", "", function () {
    //        if (typeof (InitialControl) == "function") {
    //            InitialControl();
    //        }
    //        if (typeof (ResetSection) == "function") {
    //            ResetSection();
    //        }
    //    });
    if (typeof (command_reset_click) == "function") {
        command_reset_click();
        master_event.ScrollToTopWindow();
    }
}
/* -------------------------------------------------------------------- */