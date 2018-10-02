/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/AutoComplete.js"/>

/*--- Initialize ---*/
$(document).ready(function () {
    //$("#divSearchAddrCtrl input[id=Address]").keyup(txtAddress_keyup);
    $("#divSearchAddrCtrl input[id=Address]").InitialAutoComplete("/Master/GetSiteAddress");  // Note: in Master/.../SiteData.cs/GetSiteAddress()


    //$("#divSearchAddrCtrl input[id=Alley]").keyup(txtSoi_keyup);
    $("#divSearchAddrCtrl input[id=Alley]").InitialAutoComplete("/Master/GetSiteAlley");  // Note: in Master/.../SiteData.cs/GetSiteAlley()


    //$("#divSearchAddrCtrl input[id=Road]").keyup(txtRoad_keyup);
    $("#divSearchAddrCtrl input[id=Road]").InitialAutoComplete("/Master/GetSiteRoad");  // Note: in Master/.../SiteData.cs/GetSiteRoad()


    //$("#divSearchAddrCtrl input[id=SubDistrict]").keyup(txtTambolKwaeng_keyup);
    $("#divSearchAddrCtrl input[id=SubDistrict]").InitialAutoComplete("/Master/GetSiteSubDistrict"); // Note: in Master/.../SiteData.cs/GetSiteSubDistrict()


    $("#divSearchAddrCtrl select[id=ProvinceCode]").RelateControlEvent(cboJangwat_change);

    var provinceCode = { provinceCode: $("#divSearchAddrCtrl select[id=ProvinceCode]").val() };
    call_ajax_method("/Master/GetAmphorCurrentLanguageFirstElementAll", provinceCode, update_amperked_combo);
});

/*--- Events ---*/
//function txtAddress_keyup(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val(); // + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#divSearchAddrCtrl input[id=Address]",
//                                cond,
//                                "/Master/GetSiteAddress", // Note: in Master/.../SiteData.cs/GetSiteAddress()
//                                { "cond": cond },
//                                "dtSiteAddress",
//                                "Address",
//                                "Address");
//    }
//}


//function txtSoi_keyup(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val(); // + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#divSearchAddrCtrl input[id=Alley]",
//                                cond,
//                                "/Master/GetSiteAlley", // Note: in Master/.../SiteData.cs/GetSiteAlley()
//                                { "cond": cond },
//                                "dtSiteAlley",
//                                "Alley",
//                                "Alley");
//    }
//}


//function txtRoad_keyup(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val(); // + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#divSearchAddrCtrl input[id=Road]",
//                                cond,
//                                "/Master/GetSiteRoad", // Note: in Master/.../SiteData.cs/GetSiteRoad()
//                                { "cond": cond },
//                                "dtSiteRoad",
//                                "Road",
//                                "Road");
//    }
//}


//function txtTambolKwaeng_keyup(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val(); //  + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#divSearchAddrCtrl input[id=SubDistrict]",
//                                cond,
//                                "/Master/GetSiteSubDistrict", // Note: in Master/.../SiteData.cs/GetSiteSubDistrict()
//                                { "cond": cond },
//                                "dtSiteSubDistrict",
//                                "SubDistrict",
//                                "SubDistrict");
//    }
//}


function cboJangwat_change(istab, isblur) {

    var provinceCode = { provinceCode: $("#divSearchAddrCtrl select[id=ProvinceCode]").val() };
    call_ajax_method("/Master/GetAmphorCurrentLanguageFirstElementAll", provinceCode, update_amperked_combo);

}

/*--- Methods ---*/
function update_amperked_combo(data) {
    regenerate_combo("#divSearchAddrCtrl select[id=DistrictCode]", data);
}