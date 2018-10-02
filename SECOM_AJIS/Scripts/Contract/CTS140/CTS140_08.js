var CTS140_08 = {
    InitialControl: function () {
        $("#SiteBulidingArea").BindNumericBox(5, 2, 0, 99999.99);
        $("#NumberOfBuilding").BindNumericBox(2, 0, 0, 99);
        $("#SecurityAreaFrom").BindNumericBox(5, 2, 0, 99999.99);
        $("#SecurityAreaTo").BindNumericBox(5, 2, 0, 99999.99);
        $("#NumOfFloor").BindNumericBox(3, 0, 0, 999);
    },
    SetSectionMode: function (isView) {
        $("#divSiteInformation").SetViewMode(isView);
    },
    DisabledSection: function (isDisabled) {
        $("#divSiteInformation").SetEnableView(!isDisabled);
    }
}


//function BindDOSiteInformation() {
//    call_ajax_method_json('/Contract/BindDOSiteInformation_CTS140', "",
//    function (result, controls) {
//        if (result != undefined) {
//            $("#BuildingTypeCode").val(result.BuildingTypeCode);
//            $("#SiteBulidingArea").val(result.SiteBuildingArea);
//            $("#SiteBulidingArea").setComma();
//            $("#NumberOfBuilding").val(result.NumOfBuilding);
//            $("#NumberOfBuilding").setComma();
//            $("#SecurityAreaFrom").val(result.SecurityAreaFrom);
//            $("#SecurityAreaFrom").setComma();
//            $("#SecurityAreaTo").val(result.SecurityAreaTo);
//            $("#SecurityAreaTo").setComma();
//            $("#NumOfFloor").val(result.NumOfFloor);
//            $("#NumOfFloor").setComma();
//            $("#MainStructureTypeCode").val(result.MainStructureTypeCode);
//        }
//    });
//}