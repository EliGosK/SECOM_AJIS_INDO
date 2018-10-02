/// <reference path="../../Scripts/Base/Master.js"/>

/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "SiteCustCodeNo"
    ]);
    
    InitialSiteSection();
    InitialSiteEvents();
});

function InitialSiteSection() {
    if ($("#HasSiteData").val() == "True") {
        $("#SiteCustCodeNo").attr("readonly", true);
        $("#btnRetrieveSiteData").attr("disabled", true);
        $("#btnSearchSite").attr("disabled", true);
        $("#btnCopy").attr("disabled", true);
        $("#rdoContractTarget").attr("disabled", true);
        $("#rdoRealCustomer").attr("disabled", true);

        $("#HasSiteData").val("");
    }
    else {
        $("#SiteCustCodeNo").removeAttr("readonly");
        $("#btnRetrieveSiteData").removeAttr("disabled");
        $("#btnSearchSite").removeAttr("disabled");
        $("#btnCopy").removeAttr("disabled");
        $("#rdoContractTarget").removeAttr("disabled");
        $("#rdoRealCustomer").removeAttr("disabled");

        var siteCustCode = $("#SiteCustCode").val();
        $("#divSiteCustomerInfo").clearForm();
        $("#SiteCustCode").val(siteCustCode);
        $("#rdoRealCustomer").attr("checked", true);
    }
}
function InitialSiteEvents() {
    $("#btnRetrieveSiteData").click(retrieve_site_customer_click);
    $("#btnSearchSite").click(search_site_customer_click);
    $("#btnNewEditSite").click(new_edit_site_click);
    $("#btnClearSite").click(clear_site_click);
    $("#btnCopy").click(copy_site_information_click);
}
/* ----------------------------------------------------------------------------------- */

/* --- Events ------------------------------------------------------------------------- */
/* ------------------------------------------------------------------------------------ */
function retrieve_site_customer_click() {
    RetrieveSiteInformationData();
}
function search_site_customer_click() {
    /* --- Set Parameter --- */
    var obj = {
        CustCode: $("#RCSearchCustCode").val(),
        CustType: 2
    };

    call_ajax_method_json("/Contract/CTS010_CheckRealCustomer", obj,
        function (result, controls) {
            if (controls != undefined) {
                /* --- Higlight Text --- */
                /* --------------------- */
                VaridateCtrl(["RCSearchCustCode"], ["RCSearchCustCode"]);
                /* --------------------- */

                return;
            }

            /* --- Open Screen --- */
            /* ------------------- */
            SearchSiteData(function (result) {
                $("#SiteCustCodeNo").val(result.SiteNo);
                RetrieveSiteInformationData();
            });
            /* ------------------- */
        });
}
function new_edit_site_click() {
    NewEdit_SiteData()
}
function clear_site_click() {
    InitialSiteSection();
    ClearInitialData(3);
}
function copy_site_information_click() {
    /* --- Set Parameter --- */
    /* --------------------- */
    var custType = 1;
    var custCode = $("#CPCustCodeShort").val();
    if ($("#rdoRealCustomer").prop("checked") == true) {
        custType = 2;
        custCode = $("#RCCustCodeShort").val();
    }

    var obj = {
        CustCode: custCode,
        CustType: custType,
        BranchNameEN: $("#BranchNameEN").val(),
        BranchNameLC: $("#BranchNameLC").val()
    };
    /* --------------------- */

    call_ajax_method_json("/Contract/CTS010_CopySiteInfomation", obj, function (result) {
        if (result != undefined) {
            /* --- Set Data --- */
            /* ---------------- */
            ViewSiteCustomerData(result, true);
            /* ---------------- */
        }
    });
}
/* ------------------------------------------------------------------------------------ */

/* --- Methods ------------------------------------------------------------------------ */
/* ------------------------------------------------------------------------------------ */
function RetrieveSiteInformationData() {
    /* --- Set Parameter --- */
    var obj = {
        CustCode: $("#RCCustCodeShort").val(),
        SiteCustCode: $("#SiteCustCode").val(),
        SiteNo: $("#SiteCustCodeNo").val()
    };

    call_ajax_method_json("/Contract/CTS010_RetrieveSiteData", obj, function (result, controls) {
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["RCSearchCustCode", "SiteCustCodeNo"], controls);
            /* --------------------- */

            return;
        }

        /* --- Set Data --- */
        /* ---------------- */
        ViewSiteCustomerData(result, false);
        /* ---------------- */
    });
}
function ViewSiteCustomerData(doSite, iscopy) {
    /* --- Disable Control --- */
    /* ----------------------- */
    $("#SiteCustCodeNo").attr("readonly", true);
    $("#btnRetrieveSiteData").attr("disabled", true);
    $("#btnSearchSite").attr("disabled", true);

    if (iscopy == false) {
        $("#btnCopy").attr("disabled", true);
        $("#rdoContractTarget").attr("disabled", true);
        $("#rdoRealCustomer").attr("disabled", true);
    }
    /* ----------------------- */

    var SiteCustCode = $("#SiteCustCode").val();
    var SiteNo = $("#SiteCustCodeNo").val();

    var rdoCustomer = $("#rdoRealCustomer").prop("checked");

    $("#divSiteCustomerInfo").clearForm();
    /* --- Fill Data to Control --- */
    /* ---------------------------- */
    $("#divSiteCustomerInfo").bindJSON(doSite);
    
    if (iscopy == false) {
        $("#SiteCustCode").val(SiteCustCode);
        $("#SiteCustCodeNo").val(SiteNo);
    }
    else {
        $("#SiteCustCode").val(SiteCustCode);
        $("#SiteCustCodeNo").val("");
    }

    if (rdoCustomer == true)
        $("#rdoRealCustomer").attr("checked", true);
    else
        $("#rdoContractTarget").attr("checked", true);
    /* ---------------------------- */
}
/* ------------------------------------------------------------------------------------ */


function SetCTS010_14_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#divSiteCustomerInfo").SetViewMode(true);
        $("#divSpecifySite_CopyNameAddress").hide();
    }
    else {
        $("#divSiteCustomerInfo").SetViewMode(false);
        $("#divSpecifySite_CopyNameAddress").show();
    }
}
function SetCTS010_14_EnableSection(enable) {
    $("#divSpecifySite_CopyNameAddress").SetEnableView(enable);

    if (enable) {
        $("#SiteCustCode").attr("readonly", true);
        $("#btnNewEditSite").removeAttr("disabled");
        $("#btnClearSite").removeAttr("disabled");
    }
    else {
        $("#btnNewEditSite").attr("disabled", true);
        $("#btnClearSite").attr("disabled", true);
    }
}