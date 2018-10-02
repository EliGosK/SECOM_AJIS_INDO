var CTS140_06 = {
    InitialControl: function () {
    },
    SetSectionMode: function (isView) {
        $("#divProviderServiceInformation").SetViewMode(isView);
    },
    DisabledSection: function (isDisabled) {
        $("#divProviderServiceInformation").SetEnableView(!isDisabled);
    }
}

//function BindDOProvideServiceInformation() {
//    if (objCTS140.ProductTypeCode == objCTS140.C_PROD_TYPE_ONLINE) {
//        $("#divAlarmProvidedServiceType").hide();
//    }

//    call_ajax_method_json('/Contract/BindDOProvideServiceInformation_CTS140', "",
//            function (result, controls) {
//                if (result != undefined) {
//                    $("#FireMonitoringFlag").attr('checked', result.FireMonitorFlag);
//                    $("#CrimePreventFlag").attr('checked', result.CrimePreventFlag);
//                    $("#EmergencyReportFlag").attr('checked', result.EmergencyReportFlag);
//                    $("#FacilityMonitorFlag").attr('checked', result.FacilityMonitorFlag);

//                    if (result.PhoneLineTypeCode1 == null) {
//                        $("#TelephoneLineType1").val("");
//                    }
//                    else {
//                        $("#TelephoneLineType1").val(result.PhoneLineTypeCode1);
//                    }

//                    if (result.PhoneLineOwnerCode1 == null) {
//                        $("#TelephoneLineOwner1").val("");
//                    }
//                    else {
//                        $("#TelephoneLineOwner1").val(result.PhoneLineOwnerCode1);
//                    }

//                    if (result.PhoneNo1 == null) {
//                        $("#TelephoneNo1").val("");
//                    }
//                    else {
//                        $("#TelephoneNo1").val(result.PhoneNo1);
//                    }

//                    if (result.PhoneLineTypeCode2 == null) {
//                        $("#TelephoneLineType2").val("");
//                    }
//                    else {
//                        $("#TelephoneLineType2").val(result.PhoneLineTypeCode2);
//                    }

//                    if (result.PhoneLineOwnerCode2 == null) {
//                        $("#TelephoneLineOwner2").val("");
//                    }
//                    else {
//                        $("#TelephoneLineOwner2").val(result.PhoneLineOwnerCode2);
//                    }

//                    if (result.PhoneNo2 == null) {
//                        $("#TelephoneNo2").val("");
//                    }
//                    else {
//                        $("#TelephoneNo2").val(result.PhoneNo2);
//                    }

//                    if (result.PhoneLineTypeCode3 == null) {
//                        $("#TelephoneLineType3").val("");
//                    }
//                    else {
//                        $("#TelephoneLineType3").val(result.PhoneLineTypeCode3);
//                    }

//                    if (result.PhoneLineOwnerCode3 == null) {
//                        $("#TelephoneLineOwner3").val("");
//                    }
//                    else {
//                        $("#TelephoneLineOwner3").val(result.PhoneLineOwnerCode3);
//                    }

//                    if (result.PhoneNo3 == null) {
//                        $("#TelephoneNo3").val("");
//                    }
//                    else {
//                        $("#TelephoneNo3").val(result.PhoneNo3);
//                    }
//                }
//            });
//}
