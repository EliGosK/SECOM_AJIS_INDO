var CTS140_10 = {
    InitialControl: function () {
        $("#Memo").SetMaxLengthTextArea(4000);
    },
    SetSectionMode: function (isView) {
        $("#divOtherInformation").SetViewMode(isView);
    },
    DisabledSection: function (isDisabled) {
        $("#divOtherInformation").SetEnableView(!isDisabled);
    }
}

//function BindDOOtherInformation() {
//    call_ajax_method_json('/Contract/BindOtherInformation_CTS140', "",
//        function (result, controls) {
//            if (result != undefined) {
//                $("#Memo").val(result.Memo);
//            }
//        }
//    );
//}