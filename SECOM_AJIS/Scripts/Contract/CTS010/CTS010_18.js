$(document).ready(function () {
    $("#btnRegisterNextNewRentalContract").click(register_next_rental_contract);
    $("#btnEditNextNewrentalContract").click(register_next_rental_contract);
});

function register_next_rental_contract() {
    ajax_method.CallScreenController("/Contract/ResetSessionData", "", function () {
        if (typeof (InitialCTS010Control) == "function") {
            InitialCTS010Control();
        }
        if (typeof (ResetSection) == "function") {
            ResetSection();
        }
    });
}