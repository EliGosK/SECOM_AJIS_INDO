$(document).ready(function () {
    $("#btnRegisterNextNewRentalContract").click(register_next_sale_contract);
    $("#btnEditNextNewrentalContract").click(register_next_sale_contract);
});

function register_next_sale_contract() {
    ajax_method.CallScreenController("/Contract/ResetSessionData", "", function () {
        if (typeof (SetCTS020_01_SectionMode) == "function") {
            SetCTS020_01_SectionMode(false);
        }
        if (typeof (SetCTS020_02_SectionMode) == "function") {
            SetCTS020_02_SectionMode(false);
        }
        if (typeof (InitialCTS020Control) == "function") {
            InitialCTS020Control(false);
        }
        if (typeof (ResetSection) == "function") {
            ResetSection();
        }
    });
}