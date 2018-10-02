/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>
/// <reference path="../../Scripts/Base/control_events.js"/>

/* --- Variables --- */
var mygrid;
var gridCustoemrGroup;
var pageRow; // = 5;
var btnDetailId = "MAS010DetailBtn";
var btnRemoveId = "MAS010RemoveBtn";
var detailRid;
var removeRid;
var custObj;
var resultListMode = true;
var isSelected = false;

var temp_IDNo = "";
var true_IDNo = "";
var disable_dummyFlag = false;
var cbo_width = "214px";

var currentCompany;

$(document).ready(function () {
    $("#ChkAttachImportanceFlag").attr("disabled", true);
    pageRow = MAS010Data.PageRow;

    InitialGrid();
    InitialInputEvent();
    InitialButtonEvent();

    initial();

    $("#site_BuildingUsageCode").change(getAttachImportanceFlag);
});

/* ----------------------------------------------------------------------------------- */
/* --- Initial ----------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function InitialGrid() {
    /* --- Initial grid --- */
    if ($("#grid_result").length > 0) {
        mygrid = $("#grid_result").InitialGrid(pageRow, false, "/Master/InitialGrid_MAS010");
    }

    /* ===== binding event when finish load data ===== */
    SpecialGridControl(mygrid, ["Edit", "Remove"]);

    mygrid.attachEvent("onBeforeSelect", function (new_row, old_row) {
        if (isSelected == true)
            return false;
        else
            return true;
    });
    mygrid.attachEvent("onBeforeSorting", function (ind, type, direction) {
        if (isSelected == true)
            return false;
        else
            return true;
    });
    mygrid.attachEvent("onBeforePageChanged", function (ind, count) {
        if (isSelected == true)
            return false;
        else
            return true;
    });

    BindOnLoadedEvent(mygrid, function (gen_ctrl) {
        for (var i = 0; i < mygrid.getRowsNum(); i++) {

            var row_id = mygrid.getRowId(i);

            if (gen_ctrl == true) {
                //GenerateDetailButton(mygrid, btnDetailId, row_id, "Detail", true);
                GenerateEditButton(mygrid, btnDetailId, row_id, "Edit", true);
                if (MAS010Data.HasEditPermission == "True") {
                    GenerateRemoveButton(mygrid, btnRemoveId, row_id, "Remove", true);
                } else {
                    GenerateRemoveButton(mygrid, btnRemoveId, row_id, "Remove", false);
                }
            }

            /* ===== Bind event onClick to button ===== */
            BindGridButtonClickEvent(btnDetailId, row_id, function (rid) {
                doDetailAction(rid);
            });
            BindGridButtonClickEvent(btnRemoveId, row_id, function (rid) {
                doRemoveAction(rid);
            });
        }

        setModeResultListSection(resultListMode);
    });


    // Customer group
    // gridCustoemrGroup



    if ($("#MAS010_CustomerGroupGrid").length > 0) {
        gridCustoemrGroup = $("#MAS010_CustomerGroupGrid").InitialGrid(0, false, "/Master/MAS010_InitialCustomerGroupGrid");

        gridCustoemrGroup.enableAutoHeight(true, 84);
        SpecialGridControl(gridCustoemrGroup, ["BtnRemove"]);
        BindOnLoadedEvent(gridCustoemrGroup, function (gen_ctrl) {
            var colInx = gridCustoemrGroup.getColIndexById('BtnRemove');
            for (var i = 0; i < gridCustoemrGroup.getRowsNum(); i++) {
                var rowId = gridCustoemrGroup.getRowId(i);

                if (gen_ctrl == true) {
                    GenerateRemoveButton(gridCustoemrGroup, "btnRemove", rowId, "BtnRemove", true);
                }

                // binding grid button event 
                BindGridButtonClickEvent("btnRemove", rowId, RemoveCustomerGroupCode);
            }

            gridCustoemrGroup.setSizes();

        });


    }

}

function getAttachImportanceFlag() {
    var obj = { UsageCode: $("#site_BuildingUsageCode").val() };
    call_ajax_method_json("/Master/MAS010_GetAttachImportanceFlag", obj,
    function (result, controls) {
        if (result == true) {
            $("#ChkAttachImportanceFlag").attr("checked", true);
        }
        else {
            $("#ChkAttachImportanceFlag").attr("checked", false);
        }
    });
}

function InitialInputEvent() {
    /*==== event keypress  ====*/
    //Customer Info
    //$("#cust_CustNameEN").InitialAutoComplete("/Master/GetCustomerNameEN"); //$("#cust_CustNameEN").keypress(cust_CustNameEN_keypress);
    //$("#cust_CustNameLC").InitialAutoComplete("/Master/GetCustomerNameLC"); // $("#cust_CustNameLC").keypress(cust_CustNameLC_keypress);
    $("#cust_AddressEN").InitialAutoComplete("/Master/GetCustomerAddressEN"); // $("#cust_AddressEN").keypress(cust_AddressEN_keypress);
    $("#cust_AddressLC").InitialAutoComplete("/Master/GetCustomerAddressLC"); // $("#cust_AddressLC").keypress(cust_AddressLC_keypress);
    $("#cust_AlleyEN").InitialAutoComplete("/Master/GetCustomerAlleyEN"); // $("#cust_AlleyEN").keypress(cust_AlleyEN_keypress);
    $("#cust_AlleyLC").InitialAutoComplete("/Master/GetCustomerAlleyLC"); // $("#cust_AlleyLC").keypress(cust_AlleyLC_keypress);
    $("#cust_RoadEN").InitialAutoComplete("/Master/GetCustomerRoadEN"); // $("#cust_RoadEN").keypress(cust_RoadEN_keypress);
    $("#cust_RoadLC").InitialAutoComplete("/Master/GetCustomerRoadLC"); //  $("#cust_RoadLC").keypress(cust_RoadLC_keypress);
    $("#cust_SubDistrictEN").InitialAutoComplete("/Master/GetCustomerSubDistrictEN"); // $("#cust_SubDistrictEN").keypress(cust_SubDistrictEN_keypress);
    $("#cust_SubDistrictLC").InitialAutoComplete("/Master/GetCustomerSubDistrictLC"); //  $("#cust_SubDistrictLC").keypress(cust_SubDistrictLC_keypress);
    $("#cust_ProvinceCodeEN").RelateControlEvent(cust_ProvinceCodeEN_change);
    $("#cust_ProvinceCodeLC").RelateControlEvent(cust_ProvinceCodeLC_change);
    $("#cust_DistrictCodeEN").RelateControlEvent(cust_DistrictCodeEN_change);
    $("#cust_DistrictCodeLC").RelateControlEvent(cust_DistrictCodeLC_change);

    //Site Info
    $("#site_SiteNameEN").InitialAutoComplete("/Master/GetSiteNameEN"); //$("#site_SiteNameEN").keypress(site_SiteNameEN_keypress);
    $("#site_SiteNameLC").InitialAutoComplete("/Master/GetSiteNameLC"); //$("#site_SiteNameLC").keypress(site_SiteNameLC_keypress);
    $("#site_AddressEN").InitialAutoComplete("/Master/GetSiteAddressEN"); //$("#site_AddressEN").keypress(site_AddressEN_keypress);
    $("#site_AddressLC").InitialAutoComplete("/Master/GetSiteAddressLC"); //$("#site_AddressLC").keypress(site_AddressLC_keypress);
    $("#site_AlleyEN").InitialAutoComplete("/Master/GetSiteAlleyEN"); //$("#site_AlleyEN").keypress(site_AlleyEN_keypress);
    $("#site_AlleyLC").InitialAutoComplete("/Master/GetSiteAlleyLC"); //$("#site_AlleyLC").keypress(site_AlleyLC_keypress);
    $("#site_RoadEN").InitialAutoComplete("/Master/GetSiteRoadEN"); //$("#site_RoadEN").keypress(site_RoadEN_keypress);
    $("#site_RoadLC").InitialAutoComplete("/Master/GetSiteRoadLC"); //$("#site_RoadLC").keypress(site_RoadLC_keypress);
    $("#site_SubDistrictEN").InitialAutoComplete("/Master/GetSiteSubDistrictEN"); //$("#site_SubDistrictEN").keypress(site_SubDistrictEN_keypress);
    $("#site_SubDistrictLC").InitialAutoComplete("/Master/GetSiteSubDistrictLC"); //$("#site_SubDistrictLC").keypress(site_SubDistrictLC_keypress);
    $("#site_ProvinceCodeEN").RelateControlEvent(site_ProvinceCodeEN_change);
    $("#site_ProvinceCodeLC").RelateControlEvent(site_ProvinceCodeLC_change);
    $("#site_DistrictCodeEN").RelateControlEvent(site_DistrictCodeEN_change);
    $("#site_DistrictCodeLC").RelateControlEvent(site_DistrictCodeLC_change);

    /*==== event on change customerType combo --> enable/disable companyType combo ====*/
    //$("#cust_CustTypeCode").RelateControlEvent(doCustomerChange_related);

    /*==== event on change customerType combo --> enable/disable companyType combo ====*/
    //$("#cust_CompanyTypeCode").RelateControlEvent(doCompanyChange_related);

    /*==== event on lost focus at name --> fill fullname ====*/
    //    $("#cust_CustNameEN").blur(function () {
    //        doCustNameENChange();
    //    });
    //    $("#cust_CustNameLC").blur(function () {
    //        doCustNameLCChange();
    //    });

    $("#cust_CustTypeCode").RelateControlEvent(CustomerTypeChange);
    $("#cust_CompanyTypeCode").RelateControlEvent(CompanyTypeChange);
    $("#cust_CustNameEN").InitialAutoComplete("/Master/GetCustomerNameEN");
    $("#cust_CustNameEN").keyup(CustomerNameEN_change);
    $("#cust_CustNameEN").blur(CustomerNameEN_change);
    $("#cust_CustNameLC").InitialAutoComplete("/Master/GetCustomerNameLC");
    $("#cust_CustNameLC").keyup(CustomerNameLC_change);
    $("#cust_CustNameLC").blur(CustomerNameLC_change);

    /*==== allow only numeric, backspace, del without format ====*/
    //    $("#cust_PhoneNo").keypress(function (e) {
    //        if (e.which == 0 || e.which == 8 || e.which == 45 || (e.which >= 48 && e.which <= 57)) {
    //            return true;
    //        } else {
    //            return false;
    //        }
    //    });
    //    $("#cust_FaxNo").keypress(function (e) {
    //        if (e.which == 0 || e.which == 8 || e.which == 45 || (e.which >= 48 && e.which <= 57)) {
    //            return true;
    //        } else {
    //            return false;
    //        }
    //    });
    //    $("#cust_ZipCode").keypress(function (e) {
    //        if (e.which == 0 || e.which == 8 || (e.which >= 48 && e.which <= 57)) {
    //            return true;
    //        } else {
    //            return false;
    //        }
    //    });
    //    $("#site_PhoneNo").keypress(function (e) {
    //        if (e.which == 0 || e.which == 8 || e.which == 45 || (e.which >= 48 && e.which <= 57)) {
    //            return true;
    //        } else {
    //            return false;
    //        }
    //    });
    //    $("#site_ZipCode").keypress(function (e) {
    //        if (e.which == 0 || e.which == 8 || (e.which >= 48 && e.which <= 57)) {
    //            return true;
    //        } else {
    //            return false;
    //        }
    //    });


    InitialNumericInputTextBox(["cust_IDNo", "cust_ZipCode", "site_ZipCode"]);

    $('#cust_IDNo').blur(IDNoChange); //Add by Jutarat A. on 23122013

    /*==== event on click at radio Dummy ID No./Tax ID No. ====*/
    $("#cust_DummyIDFlag").click(function () {
        doDummyIDFlagChange();
    });

    /*==== event on click at radio Dummy ID No./Tax ID No. ====*/
    $("#cust_DeleteFlag").click(function () {
        var checked = $("#cust_DeleteFlag").prop("checked");
        if (checked) {
            setDisableCustomerInfoSection();
            $("#cust_DeleteFlag").removeAttr("disabled"); //Set disable customer section except deleteFlag itself
            setDisableResultListSection(); //$("#ResultList_Section").attr("disabled", true);
            setDisableSiteInfoSection();
        } else {
            setEnableCustomerInfoSection();
            setEnableResultListSection(); //$("#ResultList_Section").removeAttr("disabled");
            setEnableSiteInfoSection();
        }
    });
}

function InitialButtonEvent() {
    /*==== event button click ====*/
    $("#btnRetrieve").click(function () {
        doRetrieveAction();
    });
    $("#btnSearchCustomer").click(function () {
        doSearchCustomer();
    });
    $("#btnUpdate").click(function () {
        isSelected = false;
        doUpdateAction();
    });
    $("#btnCancel").click(function () {
        isSelected = false;
        doCancelAction();
    });

    $("#btnAddCustomerGroupCode").click(AddCustomerGroupCode);
}

function initial() {
    CustomerTypeChange(false);

    //$("#Search_CustCode").removeClass("highlight");
    $("#MaintainCustCode_Section").ResetToNormalControl();
    $("#CustomerInfo_Section").ResetToNormalControl();
    $("#SiteInfo_Section").ResetToNormalControl();

    $("#Search_CustCode").val("");
    setEnableMaintainCustCodeSection();

    $("#cust_DeleteFlag").attr("checked", false);

    $("#MaintainCustCode_Section").show();
    $("#CustomerInfo_Section").hide();
    $("#SiteInfo_Section").hide();

    //SetConfirmCommand(false, null);
    //SetClearCommand(false, null);
    confirm_command.SetCommand(null);
    clear_command.SetCommand(null);


    $("#CustomerGroupCode").val("");

    $("#MAS010_CustomerGroupGrid").hide();

}

function AddCustomerGroupCode() {
    if ($("#CustomerGroupCode").val() == "") {
        return false;
    }
    var groupCode = $("#CustomerGroupCode").val();
    var obj = { type: "add", groupCode: groupCode };

    ajax_method.CallScreenController("/Master/MAS010_AddRemoveCustomerGroup", obj, function (result) {
        if (result != undefined) {
            if (result.length == 2) {
                CheckFirstRowIsEmpty(gridCustoemrGroup, true);
                AddNewRow(gridCustoemrGroup, [result[1].GroupCode, result[1].GroupNameEN, result[1].GroupNameLC, "", ""]);

                var row_idx = gridCustoemrGroup.getRowsNum() - 1;
                var row_id = gridCustoemrGroup.getRowId(row_idx);

                GenerateRemoveButton(gridCustoemrGroup, "btnRemove", row_id, "BtnRemove", true);
                BindGridButtonClickEvent("btnRemove", row_id, RemoveCustomerGroupCode);

                gridCustoemrGroup.setSizes();
            }
        }
    });
}

function RemoveCustomerGroupCode(row_id) {
    var groupCode = gridCustoemrGroup.cells2(gridCustoemrGroup.getRowIndex(row_id), gridCustoemrGroup.getColIndexById('GroupCode')).getValue();
    var obj = { type: "delete", groupCode: groupCode };

    ajax_method.CallScreenController("/Master/MAS010_AddRemoveCustomerGroup", obj, function (result) {
        if (result != undefined) {
            if (result[0] == "1") {
                DeleteRow(gridCustoemrGroup, row_id);
            }
        }
    });
}

/* ----------------------------------------------------------------------------------- */
/* --- Event Customer Code Section Event --------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function CustomerTypeChange(istab) {
    if ($("#cust_CustTypeCode").val() == "0") {
        $("#cust_CompanyTypeCode").removeAttr("disabled");

        if (istab) {
            $("#cust_CompanyTypeCode").focus();
        }
    }
    else {
        $("#cust_CompanyTypeCode").val("");
        $("#cust_CompanyTypeCode").attr("disabled", true);

        if (istab)
            $("#cust_IDNo").focus();
    }
    CompanyTypeChange(false);
}
function CompanyTypeChange(istab) {
    if ($("#cust_CompanyTypeCode").val() == "04") {
        $("#cust_FinancialMarketTypeCode").removeAttr("disabled");

        if (istab)
            $("#cust_FinancialMarketTypeCode").focus();
    }
    else {
        $("#cust_FinancialMarketTypeCode").val("");
        $("#cust_FinancialMarketTypeCode").attr("disabled", true);

        if (istab)
            $("#cust_IDNo").focus();
    }

    var obj = {
        CompanyTypeCode: $("#cust_CompanyTypeCode").val()
    };
    call_ajax_method_json("/Master/MAS050_GetCompanyData", obj, function (result) {
        currentCompany = null;
        if (result != undefined) {
            currentCompany = result;
        }

        DisplayFullNameEN();
        DisplayFullNameLC();
    });
}
function CustomerNameEN_change() {
    DisplayFullNameEN();
}
function CustomerNameLC_change() {
    DisplayFullNameLC();
}



function DisplayFullNameEN() {
    var name = $("#cust_CustNameEN").val();
    if (name != "") {
        if (currentCompany != undefined) {
            if (currentCompany.CustNamePrefixEN != undefined) {
                name = currentCompany.CustNamePrefixEN + " " + name;
            }
            if (currentCompany.CustNameSuffixEN != undefined) {
                name = name + " " + currentCompany.CustNameSuffixEN;
            }
        }
    }
    $("#cust_CustFullNameEN").val(name);
}
function DisplayFullNameLC() {
    var name = $("#cust_CustNameLC").val();
    if (name != "") {
        if (currentCompany != undefined) {
            if (currentCompany.CustNamePrefixLC != undefined) {
                name = currentCompany.CustNamePrefixLC + " " + name;
            }
            if (currentCompany.CustNameSuffixLC != undefined) {
                name = name + " " + currentCompany.CustNameSuffixLC;
            }
        }
    }
    $("#cust_CustFullNameLC").val(name);
}








//function doCustomerChange() {

//    var customerTypeCode = $("#cust_CustTypeCode").val();
//    var custTypeJuristic = MAS010Data.CustTypeJuristic;

//    if (customerTypeCode == custTypeJuristic) {
//        $("#cust_CompanyTypeCode").removeAttr("disabled");
//    } else {
//        $("#cust_CompanyTypeCode").attr("disabled", true);
//        $("#cust_CompanyTypeCode").val(""); //Reset CompanyType
//        doCompanyChange(); //Generate new full name
//        $("#cust_FinancialMarketTypeCode").val(""); //Reset FinancialMarketTypeCode
//    }
//}

//function doCustomerChange_related(istab, isblur) {

//    doCustomerChange();

//    if (istab && isblur) {

//        var customerTypeCode = $("#cust_CustTypeCode").val();
//        var custTypeJuristic = MAS010Data.CustTypeJuristic;
//        var checked = $("#cust_DummyIDFlag").prop("checked");

//        if (customerTypeCode == custTypeJuristic) {
//            $("#cust_CompanyTypeCode").focus();
//        } else {
//            if (checked) {
//                $("#cust_DummyIDFlag").focus();
//            } else {
//                $("#cust_IDNo").focus();
//            }

//        }
//    }

//}



//function doCompanyChange() {

//    doCompanyRelateChange(); //set enable/disable to financial combo

//    var param = {
//        "NameEN": $("#cust_CustNameEN").val(),
//        "NameLC": $("#cust_CustNameLC").val(),
//        "CompanyTypeCode": $("#cust_CompanyTypeCode").val()
//    };

//    call_ajax_method(
//        '/Master/MAS010_CompanyChange/',
//        param,
//        function (result, controls) {
//            if (result != undefined) {
//                $("#cust_CustFullNameEN").val(result.FullNameEN);
//                $("#cust_CustFullNameLC").val(result.FullNameLC);
//            }
//        }
//    );
//}

//function doCompanyChange_related(istab, isblur) {

//    doCompanyChange();

//}

//function doCompanyRelateChange() {
//    var companyTypeCode = $("#cust_CompanyTypeCode").val();
//    var compTypePublicCoLtd = MAS010Data.CompTypePublicCoLtd;

//    if (companyTypeCode == compTypePublicCoLtd) {
//        $("#cust_FinancialMarketTypeCode").removeAttr("disabled");
//    } else {
//        $("#cust_FinancialMarketTypeCode").val(""); //Reset FinancialMarketTypeCode
//        $("#cust_FinancialMarketTypeCode").attr("disabled", true);
//    }
//}

//function doCustNameENChange() {
//    var param = {
//        "NameEN": $("#cust_CustNameEN").val(),
//        "CompanyTypeCode": $("#cust_CompanyTypeCode").val()
//    };
//    call_ajax_method(
//        '/Master/MAS010_CustNameENChange/',
//        param,
//        function (result, controls) {
//            if (result != undefined) {
//                $("#cust_CustFullNameEN").val(result.FullNameEN);
//            }
//        }
//    );
//}

//function doCustNameLCChange() {
//    var param = {
//        "NameLC": $("#cust_CustNameLC").val(),
//        "CompanyTypeCode": $("#cust_CompanyTypeCode").val()
//    };
//    call_ajax_method(
//        '/Master/MAS010_CustNameLCChange/',
//        param,
//        function (result, controls) {
//            if (result != undefined) {
//                $("#cust_CustFullNameLC").val(result.FullNameLC);
//            }
//        }
//    );
//}

//Add by Jutarat A. on 23122013
function IDNoChange() {
    true_IDNo = $("#cust_IDNo").val();
}
//End Add

function doDummyIDFlagChange() {
    var checked = $("#cust_DummyIDFlag").prop("checked");
    if (checked) {
        $("#cust_IDNo").attr("readonly", true);
        $("#cust_IDNo").val(temp_IDNo);
        $("#cust_IDStar").html("&nbsp;&nbsp;");
    } else {
        $("#cust_IDNo").attr("readonly", false);
        $("#cust_IDNo").val(true_IDNo);
        $("#cust_IDStar").html("<span class=\"label-remark\">*</span>");
    }
}

/* ----------------------------------------------------------------------------------- */
/* --- Maintain Customer Code Section Action------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function doRetrieveAction() {

    // disable button [Retrieve] , [Search customer]
    $("#btnRetrieve").attr("disabled", true);
    $("#btnSearchCustomer").attr("disabled", true);

    var parameter = { "custCode": $("#Search_CustCode").val() };

    call_ajax_method(
        '/Master/MAS010_RetrieveCustInfo',
        parameter,
        function (result, controls) {

            if (controls != undefined) {

                // enable button [Retrieve] , [Search customer]
                $("#btnRetrieve").attr("disabled", false);
                $("#btnSearchCustomer").attr("disabled", false);

                VaridateCtrl(["Search_CustCode"], controls);
                $("#Search_CustCode").focus();
                return;
            } else if (result != undefined) {
                if (result[0] != undefined) {
                    doAfterLoadCustInfo(result[0]);
                }
                retrieveSiteInfo();

                if (result[1] != undefined) {

                    // enable serch customer button (set delay 2 sec.)
                    setTimeout(
                    function () {
                        LoadCustomerGroup(result[1]);
                    }, 2000);

                }

            }
            else {
                // enable button [Retrieve] , [Search customer]
                $("#btnRetrieve").attr("disabled", false);
                $("#btnSearchCustomer").attr("disabled", false);
            }
        }
    );
}

function LoadCustomerGroup(resultList) {

    $("#MAS010_CustomerGroupGrid").show();

    if (resultList != undefined) {
        if (resultList.length > 0) {
            DeleteAllRow(gridCustoemrGroup);
            CheckFirstRowIsEmpty(gridCustoemrGroup, true);
            for (var i = 0; i < resultList.length; i++) {
                CheckFirstRowIsEmpty(gridCustoemrGroup, true);
                AddNewRow(gridCustoemrGroup, [resultList[i].GroupCode, resultList[i].GroupNameEN, resultList[i].GroupNameLC, "", ""]);

                var row_idx = gridCustoemrGroup.getRowsNum() - 1;
                var row_id = gridCustoemrGroup.getRowId(row_idx);

                GenerateRemoveButton(gridCustoemrGroup, "btnRemove", row_id, "BtnRemove", true);
                BindGridButtonClickEvent("btnRemove", row_id, RemoveCustomerGroupCode);

                gridCustoemrGroup.setSizes();
            }
        }
        else {
            DeleteAllRow(gridCustoemrGroup);
        }
    }
    else {
        DeleteAllRow(gridCustoemrGroup);
    }


}

function doAfterLoadCustInfo(custObj) {
    $("#CustomerInfo_Section").clearForm();

    $("#cust_CustCodeShort").val(custObj.CustCodeShort);
    $("#cust_CustStatusCodeName").val(custObj.CustStatusCodeName);
    //$("#cust_ImportantFlag").val(custObj.ImportantFlag);
    if (custObj.ImportantFlag == true)
        $("#cust_ImportantFlag").attr("checked", true)
    $("#cust_CustTypeCode").val(custObj.CustTypeCode);
    $("#cust_CompanyTypeCode").val(custObj.CompanyTypeCode);
    $("#cust_FinancialMarketTypeCode").val(custObj.FinancialMarketTypeCode);
    //$("#cust_DummyIDFlag").val(custObj.DummyIDFlag);
    if (custObj.DummyIDFlag == true) {
        $("#cust_DummyIDFlag").attr("checked", true);
        temp_IDNo = custObj.IDNo;
        disable_dummyFlag = false;
    } else {
        $("#cust_DummyIDFlag").attr("checked", false);
        true_IDNo = custObj.IDNo;
        disable_dummyFlag = true;
    }

    $("#cust_DummyIDFlag").attr("disabled", disable_dummyFlag);
    $("#cust_IDNo").val(custObj.IDNo);
    $("#cust_CustNameEN").val(custObj.CustNameEN);
    $("#cust_CustFullNameEN").val(custObj.CustFullNameEN);
    $("#cust_CustNameLC").val(custObj.CustNameLC)
    $("#cust_CustFullNameLC").val(custObj.CustFullNameLC);
    $("#cust_RepPersonName").val(custObj.RepPersonName);
    $("#cust_ContactPersonName").val(custObj.ContactPersonName);
    $("#cust_SECOMContactPerson").val(custObj.SECOMContactPerson);
    $("#cust_RegionCode").val(custObj.RegionCode);
    $("#cust_BusinessTypeCode").val(custObj.BusinessTypeCode);
    $("#cust_PhoneNo").val(custObj.PhoneNo);
    $("#cust_FaxNo").val(custObj.FaxNo);
    $("#cust_AddressEN").val(custObj.AddressEN);
    $("#cust_AddressLC").val(custObj.AddressLC);
    $("#cust_AlleyEN").val(custObj.AlleyEN);
    $("#cust_AlleyLC").val(custObj.AlleyLC);
    $("#cust_RoadEN").val(custObj.RoadEN);
    $("#cust_RoadLC").val(custObj.RoadLC);
    $("#cust_SubDistrictEN").val(custObj.SubDistrictEN);
    $("#cust_SubDistrictLC").val(custObj.SubDistrictLC);
    $("#cust_ProvinceCodeEN").val(custObj.ProvinceCode);
    $("#cust_ProvinceCodeLC").val(custObj.ProvinceCode);
    //$("#cust_DistrictCodeEN").val(custObj.DistrictCode);
    //$("#cust_DistrictCodeLC").val(custObj.DistrictCode);
    setCustDistrictControl(custObj);
    $("#cust_ZipCode").val(custObj.ZipCode);
    $("#cust_URL").val(custObj.URL);
    $("#cust_Memo").val(custObj.Memo);

    //Hidden data Long Cust Code
    $("#cust_CustCode").val(custObj.CustCode);
}

function setCustDistrictControl(custObj) {
    var Prov = {
        provinceCode: custObj.ProvinceCode
    };

    call_ajax_method_json("/Master/GetAmphorEN", Prov, function (result) {
        regenerate_combo("#cust_DistrictCodeEN", result);
        $("#cust_DistrictCodeEN").val(custObj.DistrictCode);
        $('#cust_DistrictCodeEN').attr('style', 'width: ' + cbo_width);
    });
    call_ajax_method_json("/Master/GetAmphorLC", Prov, function (result) {
        regenerate_combo("#cust_DistrictCodeLC", result);
        $("#cust_DistrictCodeLC").val(custObj.DistrictCode);
        $('#cust_DistrictCodeLC').attr('style', 'width: ' + cbo_width);
    });
}

function retrieveSiteInfo() {

    var parameter = { "custCode": $("#Search_CustCode").val() };

    $("#grid_result").LoadDataToGrid(mygrid, pageRow, false, "/Master/MAS010_RetrieveSiteInfo", parameter, "doSite", false,
        doAfterLoadSiteInfo, //post-load
        function (result, controls, isWarning) { //pre-load
            if (isWarning == undefined) {
                $("#CustomerInfo_Section").show();
            }
        });
}

function doAfterLoadSiteInfo() {
    $("#cust_DeleteFlag").attr("disabled", true);
    setDisableCustomerInfoSection();

    //SetConfirmCommand(false, null);
    //SetClearCommand(true, doClearAction);
    confirm_command.SetCommand(null);
    clear_command.SetCommand(doClearAction);

    if (MAS010Data.HasEditPermission == "True") {
        //SetConfirmCommand(true, doConfirmAction);
        confirm_command.SetCommand(doConfirmAction);
        setEnableCustomerInfoSection();
    }

    if (MAS010Data.HasDeletePermission == "True") {
        //SetConfirmCommand(true, doConfirmAction);
        confirm_command.SetCommand(doConfirmAction);
        $("#cust_DeleteFlag").removeAttr("disabled");
    }

    $("#CustomerInfo_Section").show();
    setDisableMaintainCustCodeSection();
}

/* ----------------------------------------------------------------------------------- */
/* --- Process Action----------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function doConfirmAction(rid) {

    //DisableConfirmCommand(true);

    var checked = $("#cust_DeleteFlag").prop("checked");
    if (checked) {
        doConfirmDelete();
    } else {
        doConfirmUpdate();
    }
}

function doConfirmUpdate() {

    custObj = {
        ImportantFlag: $("#cust_ImportantFlag").prop("checked"),
        CustTypeCode: $.trim($("#cust_CustTypeCode").val()),
        CompanyTypeCode: $.trim($("#cust_CompanyTypeCode").val()),
        FinancialMarketTypeCode: $.trim($("#cust_FinancialMarketTypeCode").val()),
        IDNo: $.trim($("#cust_IDNo").val()),
        DummyIDFlag: $("#cust_DummyIDFlag").prop("checked"),
        CustNameEN: $.trim($("#cust_CustNameEN").val()),
        CustFullNameEN: $.trim($("#cust_CustFullNameEN").val()),
        CustNameLC: $.trim($("#cust_CustNameLC").val()),
        CustFullNameLC: $.trim($("#cust_CustFullNameLC").val()),
        RepPersonName: $.trim($("#cust_RepPersonName").val()),
        ContactPersonName: $.trim($("#cust_ContactPersonName").val()),
        SECOMContactPerson: $.trim($("#cust_SECOMContactPerson").val()),
        RegionCode: $.trim($("#cust_RegionCode").val()),
        BusinessTypeCode: $.trim($("#cust_BusinessTypeCode").val()),
        PhoneNo: $.trim($("#cust_PhoneNo").val()),
        FaxNo: $.trim($("#cust_FaxNo").val()),
        AddressEN: $.trim($("#cust_AddressEN").val()),
        AddressLC: $.trim($("#cust_AddressLC").val()),
        AlleyEN: $.trim($("#cust_AlleyEN").val()),
        AlleyLC: $.trim($("#cust_AlleyLC").val()),
        RoadEN: $.trim($("#cust_RoadEN").val()),
        RoadLC: $.trim($("#cust_RoadLC").val()),
        SubDistrictEN: $.trim($("#cust_SubDistrictEN").val()),
        SubDistrictLC: $.trim($("#cust_SubDistrictLC").val()),
        ProvinceCodeEN: $.trim($("#cust_ProvinceCodeEN").val()),
        ProvinceCodeLC: $.trim($("#cust_ProvinceCodeLC").val()),
        ProvinceNameEN: $.trim($("#cust_ProvinceCodeEN option:selected").text()),
        ProvinceNameLC: $.trim($("#cust_ProvinceCodeLC option:selected").text()),
        DistrictCodeEN: $.trim($("#cust_DistrictCodeEN").val()),
        DistrictCodeLC: $.trim($("#cust_DistrictCodeLC").val()),
        DistrictNameEN: $.trim($("#cust_DistrictCodeEN option:selected").text()),
        DistrictNameLC: $.trim($("#cust_DistrictCodeLC option:selected").text()),
        ZipCode: $.trim($("#cust_ZipCode").val()),
        URL: $.trim($("#cust_URL").val()),
        Memo: $.trim($("#cust_Memo").val())
    };

    ajax_method.CallScreenController(
        '/Master/MAS010_ConfirmUpdate',
        custObj,
        function (result, controls) {

            // enable confirm button
            //DisableConfirmCommand(false);

            if (controls != undefined) {
                VaridateCtrl(["cust_IDNo"
                            , "cust_CustTypeCode"
                            , "cust_CustNameEN"
                            //, "cust_CustNameLC"
                            , "cust_RegionCode"
                            , "cust_BusinessTypeCode"
                            , "cust_AddressEN"
                            //, "cust_AddressLC"
                            , "cust_RoadEN"
                            //, "cust_RoadLC"
                            , "cust_SubDistrictEN"
                            //, "cust_SubDistrictLC"
                            , "cust_ProvinceCodeEN"
                            //, "cust_ProvinceCodeLC"
                            , "cust_DistrictCodeEN"
                            //, "cust_DistrictCodeLC"
                            ]
                            , controls);
                return;
            } else if (result == true) {
                confirm_command.SetCommand(null);
                clear_command.SetCommand(null);

                ajax_method.CallScreenController(
                    '/Master/MAS010_ConfirmUpdate_Cont',
                    custObj,
                    function (result, controls) {
                        if (result != undefined) {
                            OpenInformationMessageDialog(result.Code, result.Message, function () {
                                temp_IDNo = "";
                                true_IDNo = "";
                                setEnableCustomerInfoSection();
                                initial();
                            });
                        }
                    }
                );
            }
        }
    );
}

function doConfirmDelete() {

    //    var custCode = $("#cust_CustCodeShort").val();
    //    var param = { "module": "Master", "code": "MSG1005", "param": custCode };
    //    ajax_method.CallScreenController("/Shared/GetMessage", param, function (data) {
    //        command_control.CommandControlMode(false);

    //        /* ====== Open confirm dialog =====*/
    //        OpenYesNoMessageDialog(data.Code, data.Message, function () {
    //            call_ajax_method(
    //                '/Master/MAS010_ComfirmDelete',
    //                "",
    //                function (result, controls) {
    //                    if (result != undefined) {
    //                        //Show delete complete dialogue
    //                        OpenInformationMessageDialog(result.Code, result.Message, function () {
    //                            temp_IDNo = "";
    //                            true_IDNo = "";
    //                            setEnableCustomerInfoSection();
    //                            initial();
    //                        });
    //                    }
    //                    else {
    //                        //DisableConfirmCommand(false);
    //                        command_control.CommandControlMode(true);
    //                    }
    //                }

    //            );
    //        }, function () {
    //            //DisableConfirmCommand(false);
    //            command_control.CommandControlMode(true);
    //        });

    //        return false;
    //    });

    ajax_method.CallScreenController("/Master/MAS010_ConfirmDelete_P1", "", function (result) {
        if (result != undefined) {
            command_control.CommandControlMode(false);

            OpenYesNoMessageDialog(result.Code, result.Message, function () {
                ajax_method.CallScreenController("/Master/MAS010_ConfirmDelete_P2", "", function (result2, control2) {
                    if (result2 != undefined) {
                        //Show delete complete dialogue
                        OpenInformationMessageDialog(result2.Code, result2.Message, function () {
                            temp_IDNo = "";
                            true_IDNo = "";
                            setEnableCustomerInfoSection();
                            initial();
                        });
                    }
                    else {
                        command_control.CommandControlMode(true);
                    }
                });
            }, function () {
                command_control.CommandControlMode(true);
            });
        }
    });
}

function doClearAction(rid) {
    var param = { "module": "Common", "code": "MSG0044" }; // , "param": "clear" };
    call_ajax_method("/Shared/GetMessage", param, function (data) {

        /* ====== Open confirm dialog =====*/
        OpenYesNoMessageDialog(data.Code, data.Message, function () {
            temp_IDNo = "";
            true_IDNo = "";
            initial();
        }, null);

        return false;
    });
}

/* ----------------------------------------------------------------------------------- */
/* --- Grid Section Action------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function doRemoveAction(rid) {

    //hilight row
    mygrid.selectRow(mygrid.getRowIndex(rid));

    removeRid = rid;

    var code = mygrid.cells2(mygrid.getRowIndex(removeRid), mygrid.getColIndexById('SiteCodeShort')).getValue();
    var siteCodeShort = { "siteCodeShort": code };

    call_ajax_method(
        '/Master/MAS010_Remove/',
        siteCodeShort,
        function (result, controls) {
            if (result != undefined) {
                DeleteRow(mygrid, removeRid);
            }
        }
    );
}

function doDetailAction(rid) {
    //hilight row
    mygrid.selectRow(mygrid.getRowIndex(rid));
    detailRid = rid;
    isSelected = true;

    clearSiteInfoSection();

    //Set Value from Do to Grid
    var col = mygrid.getColIndexById('ToJson');
    var strJson = mygrid.cells(rid, col).getValue().toString();
    strJson = htmlDecode(strJson);
    var rowObj = JSON.parse(strJson);

    $("#site_SiteCodeShort").val(rowObj.SiteCodeShort);
    $("#site_SiteNameEN").val(rowObj.SiteNameEN);
    $("#site_SiteNameLC").val(rowObj.SiteNameLC);
    $("#site_SECOMContactPerson").val(rowObj.SECOMContactPerson);
    $("#site_PersonInCharge").val(rowObj.PersonInCharge);
    $("#site_BuildingUsageCode").val(rowObj.BuildingUsageCode);
    $("#site_PhoneNo").val(rowObj.PhoneNo);
    $("#site_AddressEN").val(rowObj.AddressEN);
    $("#site_AddressLC").val(rowObj.AddressLC);
    $("#site_AlleyEN").val(rowObj.AlleyEN);
    $("#site_AlleyLC").val(rowObj.AlleyLC);
    $("#site_RoadEN").val(rowObj.RoadEN);
    $("#site_RoadLC").val(rowObj.RoadLC);
    $("#site_SubDistrictEN").val(rowObj.SubDistrictEN);
    $("#site_SubDistrictLC").val(rowObj.SubDistrictLC);
    $("#site_ProvinceCodeEN").val(rowObj.ProvinceCode);
    $("#site_ProvinceCodeLC").val(rowObj.ProvinceCode);
    setSiteDistrictControl(rowObj);
    $("#site_ZipCode").val(rowObj.ZipCode);

    //--Render Enable/Disable ----------------------------------------
    var checked = $("#cust_DeleteFlag").prop("checked");
    if (checked || MAS010Data.HasEditPermission == "False") {
        setDisableSiteInfoSection();
    } else {
        setEnableSiteInfoSection();
    }

    setDisableCustomerInfoSection();
    setDisableResultListSection(); //$("#ResultList_Section").attr("disabled", true);

    //Hide Confirm&Cancel Section while working on site detail section
    //SetConfirmCommand(false, null);
    //SetClearCommand(false, null);
    confirm_command.SetCommand(null);
    clear_command.SetCommand(null);

    $("#SiteInfo_Section").show();
    getAttachImportanceFlag();
    $("#site_SiteNameEN").focus();
}

function setSiteDistrictControl(rowObj) {
    var Prov = {
        provinceCode: rowObj.ProvinceCode
    };

    call_ajax_method_json("/Master/GetAmphorEN", Prov, function (result) {
        regenerate_combo("#site_DistrictCodeEN", result);
        $("#site_DistrictCodeEN").val(rowObj.DistrictCode);
        $('#site_DistrictCodeEN').attr('style', 'width: ' + cbo_width);
    });
    call_ajax_method_json("/Master/GetAmphorLC", Prov, function (result) {
        regenerate_combo("#site_DistrictCodeLC", result);
        $("#site_DistrictCodeLC").val(rowObj.DistrictCode);
        $('#site_DistrictCodeLC').attr('style', 'width: ' + cbo_width);
    });
}

function clearSiteInfoSection() {
    $("#SiteInfo_Section").clearForm();
}

/* ----------------------------------------------------------------------------------- */
/* --- Site Info Section Action------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function doUpdateAction() {
    var detailObj = {
        SiteCode: $("#site_SiteCodeShort").val(),
        SiteNameEN: $("#site_SiteNameEN").val(),
        SiteNameLC: $("#site_SiteNameLC").val(),
        SECOMContactPerson: $("#site_SECOMContactPerson").val(),
        PersonInCharge: $("#site_PersonInCharge").val(),
        BuildingUsageCode: $("#site_BuildingUsageCode").val(),
        PhoneNo: $("#site_PhoneNo").val(),
        AddressEN: $("#site_AddressEN").val(),
        AddressLC: $("#site_AddressLC").val(),
        AlleyEN: $("#site_AlleyEN").val(),
        AlleyLC: $("#site_AlleyLC").val(),
        RoadEN: $("#site_RoadEN").val(),
        RoadLC: $("#site_RoadLC").val(),
        SubDistrictEN: $("#site_SubDistrictEN").val(),
        SubDistrictLC: $("#site_SubDistrictLC").val(),
        ProvinceCodeEN: $("#site_ProvinceCodeEN").val(),
        ProvinceCodeLC: $("#site_ProvinceCodeLC").val(),
        ProvinceNameEN: $("#site_ProvinceCodeEN option:selected").text(),
        ProvinceNameLC: $("#site_ProvinceCodeLC option:selected").text(),
        DistrictCodeEN: $("#site_DistrictCodeEN").val(),
        DistrictCodeLC: $("#site_DistrictCodeLC").val(),
        DistrictNameEN: $("#site_DistrictCodeEN option:selected").text(),
        DistrictNameLC: $("#site_DistrictCodeLC option:selected").text(),
        ZipCode: $("#site_ZipCode").val()
    };

    ajax_method.CallScreenController(
        "/Master/MAS010_GetSiteFullAddress",
        detailObj,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["site_SiteNameEN"
                            //, "site_SiteNameLC"
                            , "site_BuildingUsageCode"
                            , "site_AddressEN"
                            //, "site_AddressLC"
                            , "site_RoadEN"
                            //, "site_RoadLC"
                            , "site_SubDistrictEN"
                            //, "site_SubDistrictLC"
                            , "site_ProvinceCodeEN"
                            //, "site_ProvinceCodeLC"
                            , "site_DistrictCodeEN"
                            //, "site_DistrictCodeLC"
                ]
                            , controls);
                return;
            } else if (result != undefined) {
                // Akat K. Check Duplicate Site within GRID #######################################
                //                var match = false;
                //                for (var i = 0; i < mygrid.getRowsNum(); i++) {
                //                    if (i == mygrid.getRowIndex(detailRid)) {
                //                        continue;
                //                    } //end if

                //                    var gridSiteNameLC = mygrid.cells2(i, mygrid.getColIndexById('SiteNameLC')).getValue();
                //                    var gridAddressLC = mygrid.cells2(i, mygrid.getColIndexById('AddressFullLC')).getValue();

                //                    var clearGridSiteNameLC = gridSiteNameLC.replace(/[ !"#$%&'()*+,-.:;<=>?@^_`{|}\\\[\]]/gi, "");
                //                    var clearGridAddressLC = gridAddressLC.replace(/[ !"#$%&'()*+,-.:;<=>?@^_`{|}\\\[\]]/gi, "");

                //                    var clearSiteLC = result.SiteNameLC.replace(/[ !"#$%&'()*+,-.:;<=>?@^_`{|}\\\[\]]/gi, "");
                //                    var clearAddressFullLC = result.AddressFullLC.replace(/[ !"#$%&'()*+,-.:;<=>?@^_`{|}\\\[\]]/gi, "");

                //                    if (clearGridSiteNameLC == clearSiteLC && clearGridAddressLC == clearAddressFullLC) {
                //                        match = true;
                //                        break;
                //                    } //end if
                //                } //end for

                //                if (match) {
                //                    var param = { "module": "Master", "code": "MSG1002" };
                //                    call_ajax_method("/Shared/GetMessage", param, function (data, controls) {
                //                        OpenWarningDialog(data.Message);
                //                    });
                //                    return;
                //                } //end if

                keepSiteDataInSession();
            }
        }
    );
}

function keepSiteDataInSession() {
    var detailObj = {
        SiteCode: $.trim($("#site_SiteCodeShort").val()),
        SiteNameEN: $.trim($("#site_SiteNameEN").val()),
        SiteNameLC: $.trim($("#site_SiteNameLC").val()),
        SECOMContactPerson: $.trim($("#site_SECOMContactPerson").val()),
        PersonInCharge: $.trim($("#site_PersonInCharge").val()),
        BuildingUsageCode: $.trim($("#site_BuildingUsageCode").val()),
        PhoneNo: $.trim($("#site_PhoneNo").val()),
        AddressEN: $.trim($("#site_AddressEN").val()),
        AddressLC: $.trim($("#site_AddressLC").val()),
        AlleyEN: $.trim($("#site_AlleyEN").val()),
        AlleyLC: $.trim($("#site_AlleyLC").val()),
        RoadEN: $.trim($("#site_RoadEN").val()),
        RoadLC: $.trim($("#site_RoadLC").val()),
        SubDistrictEN: $.trim($("#site_SubDistrictEN").val()),
        SubDistrictLC: $.trim($("#site_SubDistrictLC").val()),
        ProvinceCodeEN: $.trim($("#site_ProvinceCodeEN").val()),
        ProvinceCodeLC: $.trim($("#site_ProvinceCodeLC").val()),
        ProvinceNameEN: $.trim($("#site_ProvinceCodeEN option:selected").text()),
        ProvinceNameLC: $.trim($("#site_ProvinceCodeLC option:selected").text()),
        DistrictCodeEN: $.trim($("#site_DistrictCodeEN").val()),
        DistrictCodeLC: $.trim($("#site_DistrictCodeLC").val()),
        DistrictNameEN: $.trim($("#site_DistrictCodeEN option:selected").text()),
        DistrictNameLC: $.trim($("#site_DistrictCodeLC option:selected").text()),
        ZipCode: $.trim($("#site_ZipCode").val())
    };

    call_ajax_method(
        "/Master/MAS010_Update",
        detailObj,
        function (result, controls) {
            if (result != undefined) {
                //set return group code 
                var AddressFullForShow = result.AddressFullENAndLC;
                var SiteNameForShow = result.SiteNameENAndLC;
                var JsonObj = result.ToJson;

                //update site information to grid
                mygrid.cells2(mygrid.getRowIndex(detailRid), mygrid.getColIndexById('SiteNameENAndLC')).setValue(SiteNameForShow);
                mygrid.cells2(mygrid.getRowIndex(detailRid), mygrid.getColIndexById('AddressFullENAndLC')).setValue(AddressFullForShow);

                //update new obj data to grid
                mygrid.cells2(mygrid.getRowIndex(detailRid), mygrid.getColIndexById('ToJson')).setValue(JsonObj);
                mygrid.cells2(mygrid.getRowIndex(detailRid), mygrid.getColIndexById('SiteNameLC')).setValue(result.SiteNameLC);
                mygrid.cells2(mygrid.getRowIndex(detailRid), mygrid.getColIndexById('AddressFullLC')).setValue(result.AddressFullLC);

                setEnableCustomerInfoSection();
                setEnableResultListSection();
                setEnableSiteInfoSection();

                //Show Confirm&Cancel Section
                //SetConfirmCommand(false, null);
                //SetClearCommand(true, doClearAction);
                confirm_command.SetCommand(null);
                clear_command.SetCommand(doClearAction);

                if (MAS010Data.HasEditPermission == "True" || MAS010Data.HasDeletePermission == "True") {
                    //SetConfirmCommand(true, doConfirmAction);
                    confirm_command.SetCommand(doConfirmAction);
                }

                $("#SiteInfo_Section").hide();
            }
        }
    );
}

function doCancelAction() {
    var param = { "module": "Common", "code": "MSG0140" }; // "param": "cancel" };
    ajax_method.CallScreenController("/Shared/GetMessage", param, function (data) {

        /* ====== Open confirm dialog =====*/
        OpenYesNoMessageDialog(data.Code, data.Message, function () {
            setEnableCustomerInfoSection();
            setEnableResultListSection();
            setEnableSiteInfoSection();

            //Show Confirm&Cancel Section
            //SetConfirmCommand(false, null);
            //SetClearCommand(true, doClearAction);
            confirm_command.SetCommand(null);
            clear_command.SetCommand(doClearAction);

            if (MAS010Data.HasEditPermission == "True" || MAS010Data.HasDeletePermission == "True") {
                //SetConfirmCommand(true, doConfirmAction);
                confirm_command.SetCommand(doConfirmAction);
            }

            $("#SiteInfo_Section").hide();
        }, null);

        return false;
    });
}

/* ----------------------------------------------------------------------------------- */
/* --- Dialog Methods (CMS250) ------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function doSearchCustomer() {
    // disable search customer button
    //$("#btnSearchCustomer").attr("disabled", true);

    $("#dlgMAS010").OpenCMS250Dialog("MAS010");

    // enable serch customer button (set delay 3 sec.)
    //setTimeout(
    //function () {
    //    $("#btnSearchCustomer").attr("disabled", false);
    //}, 3000);
}

function CMS250Response(result) {
    $("#dlgMAS010").CloseDialog();

    $("#Search_CustCode").val(result.CustomerData.CustCode);
    var param = { "CustCode": result.CustomerData.CustCode };
    doRetrieveAction(param);
}

/* ----------------------------------------------------------------------------------- */
/* --- Enable/Disable Secion---------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
$.fn.enableForm = function () {
    return this.each(function () {
        var type = this.type, tag = this.tagName.toLowerCase();
        if (tag == 'form' || tag == 'div')
            return $(':input', this).enableForm();
        if (type == 'text' || type == 'password' || tag == 'textarea')
            $(this).attr("readonly", false);
        else if (type == 'button' || type == 'checkbox' || type == 'radio' || tag == 'select')
            $(this).removeAttr("disabled");
    });
};

$.fn.disableForm = function () {
    return this.each(function () {
        var type = this.type, tag = this.tagName.toLowerCase();
        if (tag == 'form' || tag == 'div')
            return $(':input', this).disableForm();
        if (type == 'text' || type == 'password' || tag == 'textarea')
            $(this).attr("readonly", true);
        else if (type == 'button' || type == 'checkbox' || type == 'radio' || tag == 'select')
            $(this).attr("disabled", true);
    });
};

function setEnableMaintainCustCodeSection() {
    $("#MaintainCustCode_Section").enableForm();
}

function setDisableMaintainCustCodeSection() {
    $("#MaintainCustCode_Section").disableForm();
}

function setEnableCustomerInfoSection() {
    if (MAS010Data.HasEditPermission == "True") {

        $("#CustomerInfo_Section").enableForm();

        $("#cust_DummyIDFlag").attr("disabled", disable_dummyFlag);

        $("#cust_CustCodeShort").attr("readonly", true);
        $("#cust_CustStatusCodeName").attr("readonly", true);
        $("#cust_CustFullNameEN").attr("readonly", true);
        $("#cust_CustFullNameLC").attr("readonly", true);

        setRenderCustomerInfoSection();
    }

    if (MAS010Data.HasDeletePermission == "True") {
        $("#cust_DeleteFlag").removeAttr("disabled");
    } else {
        $("#cust_DeleteFlag").attr("disabled", true);
    }
}

function setRenderCustomerInfoSection() {
    //doCustomerChange(); //set disable/enaable to company combo
    //doCompanyRelateChange(); //set disable/enaable to financial combo

    CustomerTypeChange(false);
    CompanyTypeChange(false);

    doDummyIDFlagChange(); //set disable/enaable to id input
}

function setDisableCustomerInfoSection() {
    $("#CustomerInfo_Section").disableForm();
}

function setEnableResultListSection() {
    setModeResultListSection(true);
    resultListMode = true;
}

function setDisableResultListSection() {
    setModeResultListSection(false);
    resultListMode = false;
}

function setModeResultListSection(enable) {
    $("#grid_result").find("img").each(function () {
        if (this.id != "" && this.id != undefined) {
            if (this.id.indexOf(btnRemoveId) == 0) {
                var row_id = GetGridRowIDFromControl(this);
                EnableGridButton(mygrid, btnRemoveId, row_id, "Remove", enable);
            }
            if (this.id.indexOf(btnDetailId) == 0) {
                var row_id = GetGridRowIDFromControl(this);
                EnableGridButton(mygrid, btnDetailId, row_id, "Edit", enable);
            }
        }
    });
}

function setEnableSiteInfoSection() {
    $("#SiteInfo_Section").enableForm();
    $("#ChkAttachImportanceFlag").attr("disabled", true);
    $("#site_SiteCodeShort").attr("readonly", true);
    if (MAS010Data.HasEditPermission == "False") {
        $("#btnUpdate").attr("disabled", true);
    }
}

function setDisableSiteInfoSection() {
    $("#SiteInfo_Section").disableForm();
    $("#btnCancel").removeAttr("disabled");  //user may click cancel
}

/* ----------------------------------------------------------------------------------- */
/* --- Auto Complete ----------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
//Customer----------------------------------------------------
//function cust_CustNameEN_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#cust_CustNameEN",
//                                cond,
//                                "/Master/GetCustomerNameEN",
//                                { "cond": cond },
//                                "dtCustNameEN",
//                                "CustNameEN",
//                                "CustNameEN");
//    }
//}

//function cust_CustNameLC_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#cust_CustNameLC",
//                                cond,
//                                "/Master/GetCustomerNameLC",
//                                { "cond": cond },
//                                "dtCustNameLC",
//                                "CustNameLC",
//                                "CustNameLC");
//    }
//}

//function cust_AddressEN_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#cust_AddressEN",
//                                cond,
//                                "/Master/GetCustomerAddressEN",
//                                { "cond": cond },
//                                "doCustAddressEN",
//                                "AddressEN",
//                                "AddressEN");
//    }
//}

//function cust_AddressLC_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#cust_AddressLC",
//                                cond,
//                                "/Master/GetCustomerAddressLC",
//                                { "cond": cond },
//                                "doCustAddressLC",
//                                "AddressLC",
//                                "AddressLC");
//    }
//}

//function cust_AlleyEN_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#cust_AlleyEN",
//                                cond,
//                                "/Master/GetCustomerAlleyEN",
//                                { "cond": cond },
//                                "doCustAlleyEN",
//                                "AlleyEN",
//                                "AlleyEN");
//    }
//}

//function cust_AlleyLC_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#cust_AlleyLC",
//                                cond,
//                                "/Master/GetCustomerAlleyLC",
//                                { "cond": cond },
//                                "doCustAlleyLC",
//                                "AlleyLC",
//                                "AlleyLC");
//    }
//}

//function cust_RoadEN_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);

//        InitialAutoCompleteControl("#cust_RoadEN",
//                                cond,
//                                "/Master/GetCustomerRoadEN",
//                                { "cond": cond },
//                                "doCustRoadEN",
//                                "RoadEN",
//                                "RoadEN");
//    }

//}

//function cust_RoadLC_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#cust_RoadLC",
//                                cond,
//                                "/Master/GetCustomerRoadLC",
//                                { "cond": cond },
//                                "doCustRoadLC",
//                                "RoadLC",
//                                "RoadLC");
//    }
//}

//function cust_SubDistrictEN_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#cust_SubDistrictEN",
//                                cond,
//                                "/Master/GetCustomerSubDistrictEN",
//                                { "cond": cond },
//                                "dtSiteSubDistrictEN",
//                                "SubDistrictEN",
//                                "SubDistrictEN");
//    }
//}

//function cust_SubDistrictLC_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#cust_SubDistrictLC",
//                                cond,
//                                "/Master/GetCustomerSubDistrictLC",
//                                { "cond": cond },
//                                "dtSiteSubDistrictLC",
//                                "SubDistrictLC",
//                                "SubDistrictLC");
//    }

//}

function cust_ProvinceCodeEN_change(istab, isblur) {
    $("#cust_ProvinceCodeLC").val($("#cust_ProvinceCodeEN").val());
    $("#cust_ProvinceCodeLC").ResetToNormalControl();


    var Prov = { "provinceCode": $("#cust_ProvinceCodeEN").val() };
    call_ajax_method_json("/Master/GetAmphorEN", Prov, update_cust_DistrictCodeEN_combo);
    call_ajax_method_json("/Master/GetAmphorLC", Prov, update_cust_DistrictCodeLC_combo);

}

function cust_ProvinceCodeLC_change(istab, isblur) {
    $("#cust_ProvinceCodeEN").val($("#cust_ProvinceCodeLC").val());
    $("#cust_ProvinceCodeEN").ResetToNormalControl();

    var Prov = { "provinceCode": $("#cust_ProvinceCodeLC").val() };
    call_ajax_method_json("/Master/GetAmphorEN", Prov, update_cust_DistrictCodeEN_combo);
    call_ajax_method_json("/Master/GetAmphorLC", Prov, update_cust_DistrictCodeLC_combo);

}

function update_cust_DistrictCodeEN_combo(data) {
    regenerate_combo("#cust_DistrictCodeEN", data);
    $('#cust_DistrictCodeEN').attr('style', 'width: ' + cbo_width);
}

function update_cust_DistrictCodeLC_combo(data) {
    regenerate_combo("#cust_DistrictCodeLC", data);
    $('#cust_DistrictCodeLC').attr('style', 'width: ' + cbo_width);
}

function cust_DistrictCodeEN_change(istab, isblur) {

    $("#cust_DistrictCodeLC").val($("#cust_DistrictCodeEN").val());


}

function cust_DistrictCodeLC_change(istab, isblur) {
    $("#cust_DistrictCodeEN").val($("#cust_DistrictCodeLC").val());
}

//Site----------------------------------------------------
//function site_SiteNameEN_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#site_SiteNameEN",
//                                cond,
//                                "/Master/GetSiteNameEN",
//                                { "cond": cond },
//                                "dtSiteNameEN",
//                                "SiteNameEN",
//                                "SiteNameEN");
//    }
//}

//function site_SiteNameLC_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#site_SiteNameLC",
//                                cond,
//                                "/Master/GetSiteNameLC",
//                                { "cond": cond },
//                                "dtSiteNameLC",
//                                "SiteNameLC",
//                                "SiteNameLC");
//    }
//}

//function site_AddressEN_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#site_AddressEN",
//                                cond,
//                                "/Master/GetSiteAddressEN",
//                                { "cond": cond },
//                                "dtSiteAddressEN",
//                                "AddressEN",
//                                "AddressEN");
//    }
//}

//function site_AddressLC_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#site_AddressLC",
//                                cond,
//                                "/Master/GetSiteAddressLC",
//                                { "cond": cond },
//                                "dtSiteAddressLC",
//                                "AddressLC",
//                                "AddressLC");
//    }
//}

//function site_AlleyEN_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#site_AlleyEN",
//                                cond,
//                                "/Master/GetSiteAlleyEN",
//                                { "cond": cond },
//                                "dtSiteAlleyEN",
//                                "AlleyEN",
//                                "AlleyEN");
//    }
//}

//function site_AlleyLC_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#site_AlleyLC",
//                                cond,
//                                "/Master/GetSiteAlleyLC",
//                                { "cond": cond },
//                                "dtSiteAlleyLC",
//                                "AlleyLC",
//                                "AlleyLC");
//    }
//}

//function site_RoadEN_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#site_RoadEN",
//                                cond,
//                                "/Master/GetSiteRoadEN",
//                                { "cond": cond },
//                                "dtSiteRoadEN",
//                                "RoadEN",
//                                "RoadEN");
//    }
//}

//function site_RoadLC_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#site_RoadLC",
//                                cond,
//                                "/Master/GetSiteRoadLC",
//                                { "cond": cond },
//                                "dtSiteRoadLC",
//                                "RoadLC",
//                                "RoadLC");
//    }
//}

//function site_SubDistrictEN_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#site_SubDistrictEN",
//                                cond,
//                                "/Master/GetSiteSubDistrictEN",
//                                { "cond": cond },
//                                "dtSiteSubDistrictEN",
//                                "SubDistrictEN",
//                                "SubDistrictEN");
//    }
//}

//function site_SubDistrictLC_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#site_SubDistrictLC",
//                                cond,
//                                "/Master/GetSiteSubDistrictLC",
//                                { "cond": cond },
//                                "dtSiteSubDistrictLC",
//                                "SubDistrictLC",
//                                "SubDistrictLC");
//    }
//}

function site_ProvinceCodeEN_change(istab, isblur) {
    $("#site_ProvinceCodeLC").val($("#site_ProvinceCodeEN").val());
    $("#site_ProvinceCodeLC").ResetToNormalControl();

    var Prov = { "provinceCode": $("#site_ProvinceCodeEN").val() };
    call_ajax_method("/Master/GetAmphorEN", Prov, update_site_DistrictCodeEN_combo);
    call_ajax_method("/Master/GetAmphorLC", Prov, update_site_DistrictCodeLC_combo);

}

function site_ProvinceCodeLC_change(istab, isblur) {

    $("#site_ProvinceCodeEN").val($("#site_ProvinceCodeLC").val());
    $("#site_ProvinceCodeEN").ResetToNormalControl();


    var Prov = { "provinceCode": $("#site_ProvinceCodeLC").val() };
    call_ajax_method("/Master/GetAmphorEN", Prov, update_site_DistrictCodeEN_combo);
    call_ajax_method("/Master/GetAmphorLC", Prov, update_site_DistrictCodeLC_combo);

}

function update_site_DistrictCodeEN_combo(data) {
    regenerate_combo("#site_DistrictCodeEN", data);
    $('#site_DistrictCodeEN').attr('style', 'width: ' + cbo_width);
}

function update_site_DistrictCodeLC_combo(data) {
    regenerate_combo("#site_DistrictCodeLC", data);
    $('#site_DistrictCodeLC').attr('style', 'width: ' + cbo_width);
}

function site_DistrictCodeEN_change(istab, isblur) {

    $("#site_DistrictCodeLC").val($("#site_DistrictCodeEN").val());


}

function site_DistrictCodeLC_change(istab, isblur) {

    $("#site_DistrictCodeEN").val($("#site_DistrictCodeLC").val());


}

