$(document).ready(function () {
    //MaintainScreenItem();
});

function BindDOContractBasicInformation() {
    call_ajax_method_json('/Contract/BindDOContractBasicInformation_CTS220', "", null);
}