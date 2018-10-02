$(document).ready(function () {
    InitialControlPropertyCTS220_07();
    BindDoFutureDate();
    //MaintainScreenItem();
});

function InitialControlPropertyCTS220_07() {
//    $("#ExpectedResumeDate").InitialDate();
//    $("#ReturnToOriginalFeeDate").InitialDate();
}

function BindDoFutureDate() {
    call_ajax_method_json('/Contract/BindDoFutureDate_CTS220', "", function (result, controls) {

        result.ExpectedResumeDate = ConvertDateObject(result.ExpectedResumeDate);
        $("#ExpectedResumeDate").val(ConvertDateToTextFormat(result.ExpectedResumeDate));

        result.ReturnToOriginalFeeDate = ConvertDateObject(result.ReturnToOriginalFeeDate);
        $("#ReturnToOriginalFeeDate").val(ConvertDateToTextFormat(result.ReturnToOriginalFeeDate));

    });
}
