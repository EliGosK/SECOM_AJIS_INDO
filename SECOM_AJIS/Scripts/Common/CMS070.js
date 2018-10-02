/// <reference path = "../../Scripts/jquery-1.5.1-vsdoc.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxcommon.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgrid.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgridcell.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js" />
/// <reference path = "../../Scripts/Base/Master.js" />

// Modify by Siripoj S. 24-04-2012

var btn_view_customer = "btnViewCustomer";
var btn_contract_digest = "btnContractDigest";
var btn_site_info = "btnSiteInfo";

var clearContract = false;
var clearCustomer = false;

var cbo_width = "184px";

var mygrid;
var mygrid2


$(document).ready(function () {
    $("#CustomerStatusControl").hide();
    // In section "Contract info (English/Local)"  *** (note: section still hide???)
    InitialDateFromToControl("#OperationDateFrom", "#OperationDateTo");
    InitialDateFromToControl("#CustAcceptDateFrom", "#CustAcceptDateTo");
    InitialDateFromToControl("#CompleteDateFrom", "#CompleteDateTo");

    InitialDateFromToControl("#StopDateFrom", "#StopDateTo");
    InitialDateFromToControl("#CancelDateFrom", "#CancelDateTo");

    // Cbo
    $("#CustomerJangwat").RelateControlEvent(custJangwat_change);
    $("#SiteJangwat").RelateControlEvent(siteJangwat_change);

    // Radio button (group 1)
    $("#SpecifiedSearchCustomer").change(configSearchForCustomer);
    $("#SpecifiedSearchContract").change(configSearchForContract);

    // --- disable for phase 1 , so phase 2 is enable ---
    //$("#SpecifiedSearchBilling").attr("disabled", true);
    // for phase 2
    $("#SpecifiedSearchBilling").change(radSpecifiedSearchBilling_change);



    // Radio button (group 2)
    $("#SearchTypeCode").change(rdo_SearchTypeCode_change);
    $("#SearchTypeCondition").change(rdo_SearchTypeCondition_change);


    // Radio button (group 3)
    $("#SearchContractAll").change(allContractChecked);
    $("#SearchContractRental").change(rentalContractChecked);
    $("#SearchContractSale").change(saleContractChecked);

    // Check box
    $("#CustTypeJuristic").change(function () {
        if ($("#CustTypeJuristic").prop("checked")) {
            $("#CompanyType").attr("disabled", false);
        } else {
            $("#CompanyType").attr("disabled", true);
        }
    });


    // Button
    $("#btnSearchCode").click(function () {

        // disable button btnSearchCode
        $("#btnSearchCode").attr("disabled", true);
        $("#btnSearchCondition").attr("disabled", true);
        master_event.LockWindow(true);

        searchByCode();
    });
    $("#btnClearCode").click(function () {
        $("#CMS070_SearchCode").clearForm();

        $("#Customer_List").hide();
        $("#Contract_List").hide();
    });
    $("#btnSearchCondition").click(function () {

        // disable button btnSearchCode
        $("#btnSearchCode").attr("disabled", true);
        $("#btnSearchCondition").attr("disabled", true);
        master_event.LockWindow(true);

        searchByCondition();
        //alert('scroll');

    });

    $("#btnClearCondition").click(function () {
        $("#CMS070_SearchCondition_Real").clearForm();
        $("#CMS070_SearchCondition_Site").clearForm();
        $("#CMS070_SearchCondition_Contact").clearForm();
        $("#CustomerRole").val(c_realcust);
        $("#CustomerStatusNew").attr("checked", true);
        $("#CustomerStatusExist").attr("checked", true);
        $("#CustTypeJuristic").attr("checked", true);
        $("#CompanyType").attr("disabled", false);
        $("#CustTypeIndividual").attr("checked", true);
        $("#CustTypeAssociation").attr("checked", true);
        $("#CustTypePublicOffice").attr("checked", true);
        $("#CustTypeOther").attr("checked", true);
        $("#Customer_List").hide();
        $("#Contract_List").hide();


        ClearDateFromToControl("#OperationDateFrom", "#OperationDateTo");
        ClearDateFromToControl("#CustAcceptDateFrom", "#CustAcceptDateTo");
        ClearDateFromToControl("#CompleteDateFrom", "#CompleteDateTo");

        ClearDateFromToControl("#StopDateFrom", "#StopDateTo");
        ClearDateFromToControl("#CancelDateFrom", "#CancelDateTo");
    });


    initial_autocomplete();

    innitialAllGrid();

    initialPage();

    CheckRadioDefault();
});

// tt
function initialPage() {

    $("#Search_Contract").hide();
    $("#Search_by_Condition").hide();


    // group of "Search result"
    $("#Customer_List").hide();
    $("#Contract_List").hide();

    configSearchForCustomer();

    $("#CustomerRole").val(c_realcust);
    $("#CustomerStatusNew").attr("checked", true);
    $("#CustomerStatusExist").attr("checked", true);
    $("#CustTypeJuristic").attr("checked", true);
    $("#CompanyType").attr("disabled", false);
    $("#CustTypeIndividual").attr("checked", true);
    $("#CustTypeAssociation").attr("checked", true);
    $("#CustTypePublicOffice").attr("checked", true);
    $("#CustTypeOther").attr("checked", true);
}

// tt
function initial_autocomplete() {
    $("#CustomerName").InitialAutoComplete("/Master/GetCustName");
    $("#Branchename").InitialAutoComplete("/Contract/GetContractBranchName");
    $("#GroupName").InitialAutoComplete("/Master/GetGroupName");
    $("#CustomerAddress").InitialAutoComplete("/Master/GetCustAddress");
    $("#CustomerSoi").InitialAutoComplete("/Master/GetCustAlley");
    $("#CustomerRoad").InitialAutoComplete("/Master/GetCustRoad");
    $("#CustomerTumbol").InitialAutoComplete("/Master/GetCustSubDistrict");

    $("#SiteName").InitialAutoComplete("/Master/GetSiteName");
    $("#SiteAddress").InitialAutoComplete("/Master/GetSiteAddress");
    $("#SiteSoi").InitialAutoComplete("/Master/GetSiteAlley");
    $("#SiteRoad").InitialAutoComplete("/Master/GetSiteRoad");
    $("#SiteTambol").InitialAutoComplete("/Master/GetSiteSubDistrict");
}

// tt
function rdo_SearchTypeCode_change() {

    if ($("#SearchTypeCode").prop("checked")) {
        $("#Search_By_Code").show(10);
        $("#Search_by_Condition").hide();
    }

}

// tt
function rdo_SearchTypeCondition_change() {

    if ($("#SearchTypeCondition").prop("checked")) {
        $("#Search_By_Code").hide();
        $("#Search_by_Condition").show(10);
    }



}


function configSearchForCustomer() {
    if ($("#SpecifiedSearchCustomer").prop("checked")) {
        $("#Search_Contract").hide();

        $("#CMS070_SearchCode #SiteCode").attr("readonly", "readonly");
        $("#CMS070_SearchCode #ContractCode").attr("readonly", "readonly");
        $("#CMS070_SearchCode #UserCode").attr("readonly", "readonly");
        $("#CMS070_SearchCode #PlanCode").attr("readonly", "readonly");
        $("#CMS070_SearchCode #ProjectCode").attr("readonly", "readonly");

        $("#CMS070_SearchCondition_Real #Branchename").attr("readonly", "readonly");
        $("#CMS070_SearchCondition_Site input").attr("readonly", "readonly");
        $("#CMS070_SearchCondition_Site select").attr("disabled", true);
        $("#CMS070_SearchCondition_Contact input").attr("readonly", "readonly");
        $("#CMS070_SearchCondition_Contact select").attr("disabled", true);

        $("#CMS070_SearchCode #SiteCode").val("");
        $("#CMS070_SearchCode #ContractCode").val("");
        $("#CMS070_SearchCode #UserCode").val("");
        $("#CMS070_SearchCode #PlanCode").val("");
        $("#CMS070_SearchCode #ProjectCode").val("");
        $("#CMS070_SearchCondition_Real #Branchename").val("");
        $("#CMS070_SearchCondition_Site input").val("");
        $("#CMS070_SearchCondition_Contact input").val("");

        $("#CMS070_SearchCondition_Site select").val("");
        $("#CMS070_SearchCondition_Contact select").val("");

        $("#OperationDateFrom").EnableDatePicker(false);
        $("#OperationDateTo").EnableDatePicker(false);
        $("#CustAcceptDateFrom").EnableDatePicker(false);
        $("#CustAcceptDateTo").EnableDatePicker(false);
        $("#CompleteDateFrom").EnableDatePicker(false);
        $("#CompleteDateTo").EnableDatePicker(false);

        $("#StopDateFrom").EnableDatePicker(false);
        $("#StopDateTo").EnableDatePicker(false);
        $("#CancelDateFrom").EnableDatePicker(false);
        $("#CancelDateTo").EnableDatePicker(false);

    }

    if ($("#contract_list_grid").length > 0) {
        if (mygrid2.getRowsNum() != 0) {
            innitialAllGrid();
        }
    }
    $("#Contract_List").hide();

}

function configSearchForContract() {
    if ($("#SpecifiedSearchContract").prop("checked")) {

        $("#Search_Contract").show();

        $("#CMS070_SearchCode input").attr("readonly", false);

        $("#CMS070_SearchCondition_Real #Branchename").attr("readonly", false);
        $("#CMS070_SearchCondition_Site input").attr("readonly", false);
        $("#CMS070_SearchCondition_Site select").attr("disabled", false);
        $("#CMS070_SearchCondition_Contact input").attr("readonly", false);
        $("#CMS070_SearchCondition_Contact select").attr("disabled", false);

        $("#OperationDateFrom").EnableDatePicker(true);
        $("#OperationDateTo").EnableDatePicker(true);
        $("#CustAcceptDateFrom").EnableDatePicker(true);
        $("#CustAcceptDateTo").EnableDatePicker(true);
        $("#CompleteDateFrom").EnableDatePicker(true);
        $("#CompleteDateTo").EnableDatePicker(true);

        $("#StopDateFrom").EnableDatePicker(true);
        $("#StopDateTo").EnableDatePicker(true);
        $("#CancelDateFrom").EnableDatePicker(true);
        $("#CancelDateTo").EnableDatePicker(true);

        if ($("#SearchContractAll").prop("checked")) {
            allContractChecked();
        } else if ($("#SearchContractRental").prop("checked")) {
            rentalContractChecked();
        } else if ($("#SearchContractSale").prop("checked")) {
            saleContractChecked();
        }

    }

    if ($("#customer_list_grid").length > 0) {
        if (mygrid.getRowsNum() != 0) {
            innitialAllGrid();
        }
    }
    $("#Customer_List").hide();

}

//// REFRESH COMBOBOX SECTION #########################################################################################################################
function custJangwat_change(istab, isblur) {

    $('#CustomerAmper >option').remove();

    var parameter = { "provinceCode": $("#CustomerJangwat").val() }
    call_ajax_method("/Master/GetAmphorCurrentLanguageFirstElementAll", parameter, function (data, ctrl) {
        regenerate_combo("#CustomerAmper", data);
        $('#CustomerAmper').attr('style', 'width: ' + cbo_width);
    });

}

function siteJangwat_change(istab, isblur) {

    $('#SiteAmper >option').remove();

    var parameter = { "provinceCode": $("#SiteJangwat").val() }
    call_ajax_method("/Master/GetAmphorCurrentLanguageFirstElementAll", parameter, function (data, ctrl) {
        regenerate_combo("#SiteAmper", data);
        $('#SiteAmper').attr('style', 'width: ' + cbo_width);
    });

}

//------------ TRS -----------------------
function filterRoleType() {
    if ($("#SearchContractRental").prop("checked")) {
        $("#CustomerRole").find("option").each(function () {
            if ($(this).val() != CMS070_Constant.C_CUST_ROLE_TYPE_CONTRACT_TARGET
                && $(this).val() != CMS070_Constant.C_CUST_ROLE_TYPE_REAL_CUST
                && $(this).val() != "") {
                $(this).attr("disabled", true);
                //$("#CustomerRole option[value = " + $(this).val() + "]").remove();
            }
            else {
                $(this).attr("disabled", false);
            }
        });
    }
    else if ($("#SearchContractSale").prop("checked")) {
        $("#CustomerRole").find("option").each(function () {
            if ($(this).val() != CMS070_Constant.C_CUST_ROLE_TYPE_PURCHASER
                && $(this).val() != CMS070_Constant.C_CUST_ROLE_TYPE_REAL_CUST
                && $(this).val() != "") {
                $(this).attr("disabled", true);
                //$("#CustomerRole option[value = " + $(this).val() + "]").remove();
            }
            else {
                $(this).attr("disabled", false);
            }
        });
    }
    else {
        $("#CustomerRole").find("option").each(function () {
            $(this).attr("disabled", false);
        });
    }

    $("#CustomerRole").val(CMS070_Constant.C_CUST_ROLE_TYPE_REAL_CUST);
}
//----------------------------------------

function allContractChecked() {

    if ($("#SearchContractAll").prop("checked")) {
        $("#CustAcceptDateFrom").attr("readonly", false);
        $("#CustAcceptDateTo").attr("readonly", false);
        $("#CustAcceptDateFrom").EnableDatePicker(true);
        $("#CustAcceptDateTo").EnableDatePicker(true);
        $("#ProcessStatus").attr("disabled", false);

        $("#OperationDateFrom").attr("readonly", false);
        $("#OperationDateTo").attr("readonly", false);
        $("#OperationDateFrom").EnableDatePicker(true);
        $("#OperationDateTo").EnableDatePicker(true);

        $("#StopDateFrom").attr("readonly", false);
        $("#StopDateFrom").EnableDatePicker(true);
        $("#StopDateTo").attr("readonly", false);
        $("#StopDateTo").EnableDatePicker(true);

        $("#CancelDateFrom").attr("readonly", false);
        $("#CancelDateFrom").EnableDatePicker(true);
        $("#CancelDateTo").attr("readonly", false);
        $("#CancelDateTo").EnableDatePicker(true);
        
        $("#StartType").attr("disabled", false);

        $("#UserCode").attr("readonly", false);

        $('#ChangeType >option').remove();

        var parameter = {}
        call_ajax_method("/Common/GetAllChangeTypeFirstElementAllCombo", parameter, function (data, ctrl) {
            regenerate_combo("#ChangeType", data);
            $('#ChangeType').attr('style', 'width: ' + cbo_width);
        });
    }

    filterRoleType();
    var parameter = { ServiceTypeCode: "" }
    call_ajax_method("/Common/CMS070_GetProductTypeByProductTypeCode", parameter, function (data, ctrl) {
        regenerate_combo("#ProductName", data);
    });
}


function rentalContractChecked() {

    if ($("#SearchContractRental").prop("checked")) {
        $("#CustAcceptDateFrom").attr("readonly", "readonly");
        $("#CustAcceptDateTo").attr("readonly", "readonly");
        $("#CustAcceptDateFrom").EnableDatePicker(false);
        $("#CustAcceptDateTo").EnableDatePicker(false);
        $("#ProcessStatus").attr("disabled", true);

        $("#OperationDateFrom").attr("readonly", false);
        $("#OperationDateTo").attr("readonly", false);
        $("#OperationDateFrom").EnableDatePicker(true);
        $("#OperationDateTo").EnableDatePicker(true);

        $("#StopDateFrom").attr("readonly", false);
        $("#StopDateFrom").EnableDatePicker(true);
        $("#StopDateTo").attr("readonly", false);
        $("#StopDateTo").EnableDatePicker(true);

        $("#CancelDateFrom").attr("readonly", false);
        $("#CancelDateFrom").EnableDatePicker(true);
        $("#CancelDateTo").attr("readonly", false);
        $("#CancelDateTo").EnableDatePicker(true);

        $("#StartType").attr("disabled", false);

        $("#UserCode").attr("readonly", false);

        $("#CustAcceptDateFrom").val("");
        $("#CustAcceptDateTo").val("");
        $("#ProcessStatus").val("");

        $('#ChangeType >option').remove();

        var parameter = {}
        call_ajax_method("/Common/GetRentalChangeTypeFirstElementAllCombo", parameter, function (data, ctrl) {
            regenerate_combo("#ChangeType", data);
            $('#ChangeType').attr('style', 'width: ' + cbo_width);
        });
    }


    filterRoleType();
    var parameter = { ServiceTypeCode: CMS070_Constant.C_SERVICE_TYPE_RENTAL }
    call_ajax_method("/Common/CMS070_GetProductTypeByProductTypeCode", parameter, function (data, ctrl) {
        regenerate_combo("#ProductName", data);
    });
}

function saleContractChecked() {

    if ($("#SearchContractSale").prop("checked")) {

        $("#CustAcceptDateFrom").attr("readonly", false);
        $("#CustAcceptDateTo").attr("readonly", false);
        $("#CustAcceptDateFrom").EnableDatePicker(true);
        $("#CustAcceptDateTo").EnableDatePicker(true);
        $("#ProcessStatus").attr("disabled", false);

        $("#OperationDateFrom").attr("readonly", "readonly");
        $("#OperationDateTo").attr("readonly", "readonly");
        $("#OperationDateFrom").EnableDatePicker(false);
        $("#OperationDateTo").EnableDatePicker(false);

        $("#StopDateFrom").attr("readonly", "readonly");
        $("#StopDateFrom").EnableDatePicker(false);
        $("#StopDateTo").attr("readonly", "readonly");
        $("#StopDateTo").EnableDatePicker(false);

        $("#CancelDateFrom").attr("readonly", "readonly");
        $("#CancelDateFrom").EnableDatePicker(false);
        $("#CancelDateTo").attr("readonly", "readonly");
        $("#CancelDateTo").EnableDatePicker(false);

        $("#StartType").attr("disabled", true);

        $("#UserCode").attr("readonly", "readonly");

        $("#OperationDateFrom").val("");
        $("#OperationDateTo").val("");
        $("#StartType").val("");
        $("#UserCode").val("");

        $("#StopDateFrom").val("");
        $("#StopDateTo").val("");
        $("#CancelDateFrom").val("");
        $("#CancelDateTo").val("");

        $('#ChangeType >option').remove();

        var parameter = {}
        call_ajax_method("/Common/GetSaleChangeTypeFirstElementAllCombo", parameter, function (data, ctrl) {
            regenerate_combo("#ChangeType", data);
            $('#ChangeType').attr('style', 'width: ' + cbo_width);
        });

    }
    filterRoleType();
    var parameter = { ServiceTypeCode: CMS070_Constant.C_SERVICE_TYPE_SALE }
    call_ajax_method("/Common/CMS070_GetProductTypeByProductTypeCode", parameter, function (data, ctrl) {
        regenerate_combo("#ProductName", data);        
    });

}

//// SEARCH COMMAND SECTION ###########################################################################################################################
function collectInformationCustomerByCode() {
    var searchInfoCondition = {
        //CMS070_SearchCode
        GroupCode: $.trim($('#GroupCode').val()),
        CustomerCode: $.trim($('#CustomerCode').val())
    }
    return searchInfoCondition; //return JSON.stringify(searchInfoCondition);
}

function collectInformationContractByCode() {
    var searchInfoCondition = {
        SearchContractRental: $('#SearchContractRental').prop("checked"),
        SearchContractSale: $('#SearchContractSale').prop("checked"),
        //CMS070_SearchCode
        GroupCode: $.trim($('#GroupCode').val()),
        CustomerCode: $.trim($('#CustomerCode').val()),
        SiteCode: $.trim($('#SiteCode').val()),
        ContractCode: $.trim($('#ContractCode').val()),
        UserCode: $.trim($('#UserCode').val()),
        PlanCode: $.trim($('#PlanCode').val()),
        ProjectCode: $.trim($('#ProjectCode').val())
    }
    return searchInfoCondition; // return JSON.stringify(searchInfoCondition);
}

function searchByCode() {
    var param = "";
    if ($("#SpecifiedSearchCustomer").prop("checked")) {
        param = collectInformationCustomerByCode();
        validateCustomerSearchByCode(param);
    } else {
        param = collectInformationContractByCode();
        validateContractSearchByCode(param);
    }
}

function validateCustomerSearchByCode(param) {
    call_ajax_method_json(
        '/Common/CMS070_ValidateCustomerSearchByCode',
        param,
        function (result, controls) {
            if (result != undefined) {
                $("#Header_Customer_List").html(c_sectionCustomer + "(" + c_role + ": " + c_roleAll + ")");
                loadCustomerGridData(param);
            }
            else {
                // enable button btnSearchCode
                $("#btnSearchCode").attr("disabled", false);
                $("#btnSearchCondition").attr("disabled", false);
                master_event.LockWindow(false);
            }
        }
    );
}

function drawCustomerListGrid(row_id) {
    if (mygrid.getRowsNum() != 0 && !clearCustomer) {
        if (permission.CMS080 == "True") {
            GenerateDetailButton(mygrid, btn_view_customer, row_id, "Detail", true);
        } else {
            GenerateDetailButton(mygrid, btn_view_customer, row_id, "Detail", false);
        }

        if (permission.CMS080 == "True") {
            BindGridButtonClickEvent(btn_view_customer, row_id, function (rid) {
                var cust_code = mygrid.cells(rid, mygrid.getColIndexById('CustCode')).getValue();

                var custRole;
                if ($("#SearchTypeCode").prop("checked")) {
                    custRole = null;
                } else {
                    custRole = $.trim($('#CustomerRole').val());
                }

                var parameter = {
                    strCustomerCode: cust_code,
                    strCustomerRole: custRole
                };
                ajax_method.CallScreenControllerWithAuthority('/Common/CMS080', parameter, true);
                mygrid.selectRow(mygrid.getRowIndex(rid));
            });
        }
    }
}

function drawContractListGrid(row_id) {
    if (mygrid2.getRowsNum() != 0 && !clearContract) {
        if (permission.CMS190 == "True") {
            GenerateDetailButton(mygrid2, btn_contract_digest, row_id, "ContactDetail", true);
        } else {
            GenerateDetailButton(mygrid2, btn_contract_digest, row_id, "ContactDetail", false);
        }

        if (permission.CMS280 == "True") {
            GenerateDetailButton(mygrid2, btn_site_info, row_id, "SiteDetail", true);
        } else {
            GenerateDetailButton(mygrid2, btn_site_info, row_id, "SiteDetail", false);
        }

        if (permission.CMS190 == "True") {
            BindGridButtonClickEvent(btn_contract_digest, row_id, function (rid) {
                var contract_code = mygrid2.cells(rid, mygrid2.getColIndexById('ContractCode')).getValue();
                var service_type_code = mygrid2.cells(rid, mygrid2.getColIndexById('ServiceTypeCode')).getValue();
                var parameter = {
                    strContractCode: contract_code,
                    strServiceTypeCode: service_type_code
                };
                ajax_method.CallScreenControllerWithAuthority('/Common/CMS190', parameter, true);
                mygrid2.selectRow(mygrid2.getRowIndex(rid));
            });
        }
        if (permission.CMS280 == "True") {
            BindGridButtonClickEvent(btn_site_info, row_id, function (rid) {
                var site_code = mygrid2.cells(rid, mygrid2.getColIndexById('SiteCode')).getValue();
                var parameter = {
                    strSiteCode: site_code
                };
                ajax_method.CallScreenControllerWithAuthority('/Common/CMS280', parameter, true);
                mygrid2.selectRow(mygrid2.getRowIndex(rid));
            });
        }
    }
}

function innitialAllGrid() {
    mygrid = $("#customer_list_grid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS070_InitCustomerGrid");
    SpecialGridControl(mygrid, ["Detail"]);
    BindOnLoadedEventV2(mygrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, true, drawCustomerListGrid);

    mygrid2 = $("#contract_list_grid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS070_InitContractGrid");
    SpecialGridControl(mygrid2, ["ContactDetail", "SiteDetail"]);
    BindOnLoadedEventV2(mygrid2, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, true, drawContractListGrid);
}

function eXcell_price(cell) {
    this.base = eXcell_edn;
    this.base(cell);
    this.setValue = function (val) {
        if (!val || val.toString()._dhx_trim() == "") {
            val = "0";
        }
        this.cell.innerHTML = this.grid._aplNF(val, this.cell._cellIndex);
    }
}

eXcell_price.prototype = new eXcell_edn;

function loadCustomerGridData(parameter) {
    //3.2	Search Employee Data
    if ($("#customer_list_grid").length > 0) {
        //mygrid.startFastOperations();
        //mygrid.enableDistributedParsing(true, 1000);
        $("#customer_list_grid").LoadDataToGrid(mygrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS070_CustomerSearch", parameter, "CMS070_CustomerView", false,
            function () {

                // enable  search button
                $("#btnSearchCode").attr("disabled", false);
                $("#btnSearchCondition").attr("disabled", false);
                master_event.LockWindow(false);

                master_event.ScrollWindow("#Customer_List");
                //mygrid.stopFastOperations();
            },
            function (result, controls, isWarnig) {
                $("#Customer_List").show();
            });
    }
}

function validateContractSearchByCode(param) {
    call_ajax_method_json(
        '/Common/CMS070_ValidateContractSearchByCode',
        param,
        function (result, controls) {
            if (result != undefined) {
                loadContractGridData(param);
            }
            else {
                // enable button btnSearchCode
                $("#btnSearchCode").attr("disabled", false);
                $("#btnSearchCondition").attr("disabled", false);
                master_event.LockWindow(false);
            }
        }
    );
}

function loadContractGridData(parameter) {
    //3.2	Search Employee Data

    if ($("#contract_list_grid").length > 0) {

        mygrid.startFastOperations();
        mygrid.enableDistributedParsing(true, 1000);


        $("#contract_list_grid").LoadDataToGrid(mygrid2, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS070_ContractSearch", parameter, "CMS070_ContractView", false,
            function () {

                // enable  search button
                $("#btnSearchCode").attr("disabled", false);
                $("#btnSearchCondition").attr("disabled", false);
                master_event.LockWindow(false);

                //document.getElementById('Contract_List').scrollIntoView();
                master_event.ScrollWindow("#Contract_List");
                mygrid.stopFastOperations();
            },
            function (result, controls, isWarnig) {
                $("#Contract_List").show();
            });
    }
}

function collectInformationCustomerByCondition() {
    var searchInfoCondition = {
        //CMS070_SearchCondition_Real
        CustomerName: $.trim($('#CustomerName').val()),
        Branchename: $.trim($('#Branchename').val()),
        GroupName: $.trim($('#GroupName').val()),
        CustomerRole: $.trim($('#CustomerRole').val()),
        CustomerStatusNew: $('#CustomerStatusNew').prop("checked") ? true : null,
        CustomerStatusExist: $('#CustomerStatusExist').prop("checked") ? true : null,
        CustTypeJuristic: $('#CustTypeJuristic').prop("checked") ? true : null,
        CompanyType: $.trim($('#CompanyType').val()),
        CustTypeIndividual: $('#CustTypeIndividual').prop("checked") ? true : null,
        CustTypeAssociation: $('#CustTypeAssociation').prop("checked") ? true : null,
        CustTypePublicOffice: $('#CustTypePublicOffice').prop("checked") ? true : null,
        CustTypeOther: $('#CustTypeOther').prop("checked") ? true : null,
        CustomerIDNo: $.trim($('#CustomerIDNo').val()),
        CustomerNatioality: $.trim($('#CustomerNatioality').val()),
        CustomerBusinessType: $.trim($('#CustomerBusinessType').val()),
        CustomerAddress: $.trim($('#CustomerAddress').val()),
        CustomerSoi: $.trim($('#CustomerSoi').val()),
        CustomerZipCode: $.trim($('#CustomerZipCode').val()),
        CustomerRoad: $.trim($('#CustomerRoad').val()),
        CustomerTelephone: $.trim($('#CustomerTelephone').val()),
        CustomerTumbol: $.trim($('#CustomerTumbol').val()),
        CustomerFax: $.trim($('#CustomerFax').val()),
        CustomerJangwat: $.trim($('#CustomerJangwat').val()),
        CustomerAmper: $.trim($('#CustomerAmper').val())
    }
    return searchInfoCondition; // return JSON.stringify(searchInfoCondition);
}

function collectInformationContractByCondition() {
    var searchInfoCondition = {
        SearchContractRental: $('#SearchContractRental').prop("checked"),
        SearchContractSale: $('#SearchContractSale').prop("checked"),
        //CMS070_SearchCondition_Real
        CustomerName: $.trim($('#CustomerName').val()),
        Branchename: $.trim($('#Branchename').val()),
        GroupName: $.trim($('#GroupName').val()),
        CustomerRole: $.trim($('#CustomerRole').val()),
        CustomerStatusNew: $('#CustomerStatusNew').prop("checked") ? true : null,
        CustomerStatusExist: $('#CustomerStatusExist').prop("checked") ? true : null,
        CustTypeJuristic: $('#CustTypeJuristic').prop("checked") ? true : null,
        CompanyType: $.trim($('#CompanyType').val()),
        CustTypeIndividual: $('#CustTypeIndividual').prop("checked") ? true : null,
        CustTypeAssociation: $('#CustTypeAssociation').prop("checked") ? true : null,
        CustTypePublicOffice: $('#CustTypePublicOffice').prop("checked") ? true : null,
        CustTypeOther: $('#CustTypeOther').prop("checked") ? true : null,
        CustomerIDNo: $.trim($('#CustomerIDNo').val()),
        CustomerNatioality: $.trim($('#CustomerNatioality').val()),
        CustomerBusinessType: $.trim($('#CustomerBusinessType').val()),
        CustomerAddress: $.trim($('#CustomerAddress').val()),
        CustomerSoi: $.trim($('#CustomerSoi').val()),
        CustomerZipCode: $.trim($('#CustomerZipCode').val()),
        CustomerRoad: $.trim($('#CustomerRoad').val()),
        CustomerTelephone: $.trim($('#CustomerTelephone').val()),
        CustomerTumbol: $.trim($('#CustomerTumbol').val()),
        CustomerFax: $.trim($('#CustomerFax').val()),
        CustomerJangwat: $.trim($('#CustomerJangwat').val()),
        CustomerAmper: $.trim($('#CustomerAmper').val()),

        //CMS070_SearchCondition_Site
        SiteName: $.trim($('#SiteName').val()),
        SiteAddress: $.trim($('#SiteAddress').val()),
        SiteSoi: $.trim($('#SiteSoi').val()),
        SiteZipCode: $.trim($('#SiteZipCode').val()),
        SiteRoad: $.trim($('#SiteRoad').val()),
        SiteTelephone: $.trim($('#SiteTelephone').val()),
        SiteTambol: $.trim($('#SiteTambol').val()),
        SiteJangwat: $.trim($('#SiteJangwat').val()),
        SiteAmper: $.trim($('#SiteAmper').val()),

        //CMS070_SearchCondition_Contact
        OperationDateFrom: $.trim($('#OperationDateFrom').val()),
        OperationDateTo: $.trim($('#OperationDateTo').val()),
        StopDateFrom: $.trim($('#StopDateFrom').val()),
        StopDateTo: $.trim($('#StopDateTo').val()),
        CancelDateFrom: $.trim($('#CancelDateFrom').val()),
        CancelDateTo: $.trim($('#CancelDateTo').val()),

        CustAcceptDateFrom: $.trim($('#CustAcceptDateFrom').val()),
        CustAcceptDateTo: $.trim($('#CustAcceptDateTo').val()),
        CompleteDateFrom: $.trim($('#CompleteDateFrom').val()),
        CompleteDateTo: $.trim($('#CompleteDateTo').val()),
        ContractOffice: $.trim($('#ContractOffice').val()),
        OperationOffice: $.trim($('#OperationOffice').val()),
        SaleEmpNo: $.trim($('#SaleEmpNo').val()),
        SaleName: $.trim($('#SaleName').val()),
        ProductName: $.trim($('#ProductName').val()),
        ChangeType: $.trim($('#ChangeType').val()),
        ProcessStatus: $.trim($('#ProcessStatus').val()),
        StartType: $.trim($('#StartType').val())
    }
    return searchInfoCondition; // return JSON.stringify(searchInfoCondition);
}

function searchByCondition() {
    var param = "";
    if ($("#SpecifiedSearchCustomer").prop("checked")) {
        param = collectInformationCustomerByCondition();
        validateCustomerSearchByCondition(param);
    } else {
        param = collectInformationContractByCondition();
        validateContractSearchByCondition(param);
    }
}

function validateContractSearchByCondition(param) {
    call_ajax_method_json(
        '/Common/CMS070_ValidateSearchByCondition',
        param,
        function (result, controls) {
            if (result != undefined) {
                loadContractGridData(param);
            }
            else {
                // enable button btnSearchCode
                $("#btnSearchCode").attr("disabled", false);
                $("#btnSearchCondition").attr("disabled", false);
                master_event.LockWindow(false);

                if (controls != undefined) {
                    VaridateCtrl(["OperationDateFrom",
                                "OperationDateTo",
                                "StopDateFrom",
                                "StopDateTo",
                                "CancelDateFrom",
                                "CancelDateTo",
                                "CustAcceptDateFrom",
                                "CustAcceptDateTo",
                                "CompleteDateFrom",
                                "CompleteDateTo"], controls);
                }
            }
        }
    );
}

function validateCustomerSearchByCondition(param) {
    call_ajax_method_json(
        '/Common/CMS070_ValidateSearchByCondition',
        param,
        function (result, controls) {
            if (result != undefined) {

                if ($("#CustomerRole").val() == "") {
                    $("#Header_Customer_List").html(c_sectionCustomer + "(" + c_role + ": " + c_roleAll + ")");
                }
                else if ($("#CustomerRole").val() == "1") {
                    
                    $("#Header_Customer_List").html(c_sectionCustomer + "(" + c_role + ": Contract)");
                }
                else {
                    var role_name = $("#CustomerRole option[selected='selected']").text();
                    role_name = role_name.substr(3);
                     $("#Header_Customer_List").html(c_sectionCustomer + "(" + c_role + ": " + role_name + ")");
                }



                loadCustomerGridData(param);
            }
            else {

                // enable button btnSearchCode
                $("#btnSearchCode").attr("disabled", false);
                $("#btnSearchCondition").attr("disabled", false);
                master_event.LockWindow(false);

                if (controls != undefined) {
                    VaridateCtrl(["OperationDateFrom",
                                "OperationDateTo",
                                "StopDateFrom",
                                "StopDateTo",
                                "CancelDateFrom",
                                "CancelDateTo",
                                "CustAcceptDateFrom",
                                "CustAcceptDateTo",
                                "CompleteDateFrom",
                                "CompleteDateTo"], controls);
                }
            }

        }
    );
}

// Goto CMS400
function radSpecifiedSearchBilling_change() {
    if ($("#SpecifiedSearchBilling").prop("checked") == true) {
        var obj = "";
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS400", obj, false);
    }
}

function CheckRadioDefault() {
    if (c_radioDefault == "Contract") {
        $("#SpecifiedSearchContract").attr("checked", true);
        configSearchForContract();
    }
    else if (c_radioDefault == "Customer") {
        $("#SpecifiedSearchCustomer").attr("checked", true);
        configSearchForCustomer();
    }
}

// Akat K. : Innitial All Grid Old version
//function innitialAllGrid() {
//    mygrid = $("#customer_list_grid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS070_InitCustomerGrid");

//    SpecialGridControl(mygrid, ["Detail"]);
//    BindOnLoadedEvent(mygrid, function (gen_ctrl) {
//        if (mygrid.getRowsNum() != 0 && !clearCustomer) {
//            for (var i = 0; i < mygrid.getRowsNum(); i++) {
//                var row_id = mygrid.getRowId(i);

//                if (gen_ctrl == true) {
//                    if (permission.CMS080 == "True") {
//                        GenerateDetailButton(mygrid, btn_view_customer, row_id, "Detail", true);
//                    } else {
//                        GenerateDetailButton(mygrid, btn_view_customer, row_id, "Detail", false);
//                    }
//                }

//                if (permission.CMS080 == "True") {
//                    BindGridButtonClickEvent(btn_view_customer, row_id, function (rid) {
//                        var cust_code = mygrid.cells(rid, mygrid.getColIndexById('CustCode')).getValue();

//                        var custRole;
//                        if ($("#SearchTypeCode").prop("checked")) {
//                            custRole = null;
//                        } else {
//                            custRole = $.trim($('#CustomerRole').val());
//                        }

//                        var parameter = {
//                            strCustomerCode: cust_code,
//                            strCustomerRole: custRole
//                        };
//                        ajax_method.CallScreenControllerWithAuthority('/Common/CMS080', parameter, true);
//                        mygrid.selectRow(mygrid.getRowIndex(rid));
//                    });
//                }
//            }
//        }
//    });

//    mygrid2 = $("#contract_list_grid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS070_InitContractGrid");

//    SpecialGridControl(mygrid2, ["ContactDetail", "SiteDetail"]);
//    BindOnLoadedEvent(mygrid2, function (gen_ctrl) {
//        if (mygrid2.getRowsNum() != 0 && !clearContract) {
//            for (var i = 0; i < mygrid2.getRowsNum(); i++) {
//                var row_id = mygrid2.getRowId(i);

//                if (gen_ctrl == true) {
//                    if (permission.CMS190 == "True") {
//                        GenerateDetailButton(mygrid2, btn_contract_digest, row_id, "ContactDetail", true);
//                    } else {
//                        GenerateDetailButton(mygrid2, btn_contract_digest, row_id, "ContactDetail", false);
//                    }

//                    if (permission.CMS280 == "True") {
//                        GenerateDetailButton(mygrid2, btn_site_info, row_id, "SiteDetail", true);
//                    } else {
//                        GenerateDetailButton(mygrid2, btn_site_info, row_id, "SiteDetail", false);
//                    }
//                }

//                if (permission.CMS190 == "True") {
//                    BindGridButtonClickEvent(btn_contract_digest, row_id, function (rid) {
//                        var contract_code = mygrid2.cells(rid, mygrid2.getColIndexById('ContractCode')).getValue();
//                        var service_type_code = mygrid2.cells(rid, mygrid2.getColIndexById('ServiceTypeCode')).getValue();
//                        var parameter = {
//                            strContractCode: contract_code,
//                            strServiceTypeCode: service_type_code
//                        };
//                        ajax_method.CallScreenControllerWithAuthority('/Common/CMS190', parameter, true);
//                        mygrid2.selectRow(mygrid2.getRowIndex(rid));
//                    });
//                }
//                if (permission.CMS280 == "True") {
//                    BindGridButtonClickEvent(btn_site_info, row_id, function (rid) {
//                        var site_code = mygrid2.cells(rid, mygrid2.getColIndexById('SiteCode')).getValue();
//                        var parameter = {
//                            strSiteCode: site_code
//                        };
//                        ajax_method.CallScreenControllerWithAuthority('/Common/CMS280', parameter, true);
//                        mygrid2.selectRow(mygrid2.getRowIndex(rid));
//                    });
//                }
//            }
//        }
//    });
//}
