/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Base/GridControl.js" />

/* --- Variable ------------------------------------------------------- */
/* -------------------------------------------------------------------- */
var gridRealCustomerGroup = null;
/* -------------------------------------------------------------------- */

function CTS020_07_InitialGrid() {
    gridRealCustomerGroup = $("#gridRealCustomerGroup").LoadDataToGridWithInitial(0, false, false,
                                "/Contract/CTS020_GetRealCustomerGroupData",
                                "",
                                "dtCustomeGroupData", false);
}
function SetCTS020_07_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#divRealCustomerInfo").SetViewMode(true);
        $("#divSpecifyRealCustomer").hide();
        $("#divCopyPurchaser").hide();
    }
    else {
        $("#divRealCustomerInfo").SetViewMode(false);
        $("#divSpecifyRealCustomer").show();
        $("#divCopyPurchaser").show();
    }
}
function SetCTS020_07_EnableSection(enable) {
    $("#divSpecifyRealCustomer").SetEnableView(enable);
    $("#divCopyPurchaser").SetEnableView(enable);

    if (enable) {
        $("#btnRCNewEditCustomer").removeAttr("disabled");
        $("#btnRCClearCustomer").removeAttr("disabled");
    }
    else {
        $("#btnRCNewEditCustomer").attr("disabled", true);
        $("#btnRCClearCustomer").attr("disabled", true);
    }
}

/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "RCSearchCustCode"
    ]);

    InitialRealCustomerSection();
    InitialRealCustomerEvents();

    if (processType == 2)
        SetCTS020_07_EnableSection(false);
});

function InitialRealCustomerSection() {
    /* --- Enable Control --- */
    if ($("#HasRealCustomerData").val() == "True") {
        $("#RCSearchCustCode").attr("readonly", true);
        $("#btnRCRetrieve").attr("disabled", true);
        $("#btnRCSearchCustomer").attr("disabled", true);
        $("#HasRealCustomerData").val("");

        if ($("#SameCustomer").val() == "True"
            || $("#RCSearchCustCode").val() != "") {
            $("#btnRCNewEditCustomer").attr("disabled", true);
        }
        else {
            $("#btnRCNewEditCustomer").removeAttr("disabled");
        }
    }
    else {
        $("#RCSearchCustCode").removeAttr("readonly");
        $("#btnRCRetrieve").removeAttr("disabled");
        $("#btnRCSearchCustomer").removeAttr("disabled");
        $("#btnRCNewEditCustomer").removeAttr("disabled");

        var rmemo = $("#RealCustomerMemo").val();
        $("#divRealCustomerInfo").clearForm();
        
        $("#RealCustomerMemo").val(rmemo);
    }
}
function InitialRealCustomerEvents() {
    $("#btnRCRetrieve").click(retrieve_real_customer_click);
    $("#btnRCSearchCustomer").click(search_real_customer_click);
    $("#btnRCNewEditCustomer").click(new_edit_real_customer_click);
    $("#btnRCClearCustomer").click(clear_real_customer_click);
    $("#btnRCCopyPurchaser").click(copy_purchaser_click);
}
/* ----------------------------------------------------------------------------------- */

/* --- Events ------------------------------------------------------------------------- */
/* ------------------------------------------------------------------------------------ */
function retrieve_real_customer_click() {
    RetrieveRealCustomerData();
}
function search_real_customer_click() {
    SearchCustomerData(function (data) {
        var code = data.CustCode;
        if (code == undefined)
            code = "";

        $("#RCSearchCustCode").val(code);
        RetrieveRealCustomerData();
    });
}
function new_edit_real_customer_click() {
    NewEdit_CustomerData(2);
}
function clear_real_customer_click() {
    InitialRealCustomerSection();
    DeleteAllRow(gridRealCustomerGroup);
    ClearInitialData(2);

    $("#SameCustomer").val("");

    $("#SiteCustCode").val("");
    if ($("#SiteCodeShort").val() != "") {
        $("#SiteCustCodeNo").val("");
        $("#divSiteCustomerInfo").clearForm();

        $("#SiteCustCodeNo").removeAttr("readonly");
        $("#btnRetrieveSiteData").removeAttr("disabled");
        $("#btnSearchSite").removeAttr("disabled");
        $("#btnCopy").removeAttr("disabled");
        $("#rdoContractTarget").removeAttr("disabled");
        $("#rdoRealCustomer").removeAttr("disabled");
        $("#btnNewEditSite").removeAttr("disabled");
        $("#btnClearSite").removeAttr("disabled");
        $("#rdoRealCustomer").attr("checked", true);
        ClearInitialData(3);
    }
}
function copy_purchaser_click() {
    /* --- Call Event --- */
    /* ------------------ */
    call_ajax_method_json("/Contract/CTS020_CopyCustomer", "", function (result) {
        if (result == undefined)
            return;
        if (result.length < 2)
            return;
        ViewRealCustomerData(result[0]);

        if (result[1] == false) {
            if (typeof (InitialSiteSection) == "function")
                InitialSiteSection();
        }

        $("#RCSearchCustCode").val(result[0].CustCodeShort);
        if (result[0].CustCode != undefined) {
            $("#SiteCustCode").val(result[0].SiteCustCodeShort);
        }

        $("#btnRCNewEditCustomer").attr("disabled", true);
        $("#SameCustomer").val("True");
    });
    /* ------------------ */
}
/* ------------------------------------------------------------------------------------ */

/* --- Methods ------------------------------------------------------------------------ */
/* ------------------------------------------------------------------------------------ */
function RetrieveRealCustomerData() {
    RetrieveCustomerData($("#RCSearchCustCode").val(), 2, function (result, controls) {
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["RCSearchCustCode"], ["RCSearchCustCode"]);
            /* --------------------- */

            return;
        }

        /* --- Set Data --- */
        /* ---------------- */
        ViewRealCustomerData(result[0]);
        /* ---------------- */

        if (result[0].CustCode != undefined) {
            $("#SiteCustCode").val(result[0].SiteCustCodeShort);
        }
        if (result[1] == true) {
            $("#btnRCNewEditCustomer").attr("disabled", true);
            $("#SameCustomer").val("True");
        }
        if (result[0].CustCode != undefined) {
            $("#btnRCNewEditCustomer").attr("disabled", true);
        }
    });
}
function ViewRealCustomerData(contractData) {
    var rmemo = $("#RealCustomerMemo").val();
    $("#divRealCustomerInfo").clearForm();

    /* --- Fill Data to Control --- */
    /* ---------------------------- */
    $("#divRealCustomerInfo").bindJSON_Prefix("RC", contractData);
    $("#RCSearchCustCode").val(contractData.CustCodeShort);
    $("#RealCustomerMemo").val(rmemo);
    /* ---------------------------- */

    /* --- Disable Control --- */
    /* ----------------------- */
    $("#RCSearchCustCode").attr("readonly", true);
    $("#btnRCRetrieve").attr("disabled", true);
    $("#btnRCSearchCustomer").attr("disabled", true);
    /* ----------------------- */

    $("#gridRealCustomerGroup").LoadDataToGrid(gridRealCustomerGroup, 0, false,
                                            "/Contract/CTS020_GetRealCustomerGroupData",
                                            "",
                                            "dtCustomeGroupData", false);
}
/* ----------------------------------------------------------------------------------- */


