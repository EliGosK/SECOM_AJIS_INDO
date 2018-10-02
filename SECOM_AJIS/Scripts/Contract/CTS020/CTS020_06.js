/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Base/GridControl.js" />

/* --- Variable ------------------------------------------------------- */
/* -------------------------------------------------------------------- */
var gridPurchaserTargetGroup = null;
/* -------------------------------------------------------------------- */

function CTS020_06_InitialGrid() {
    gridPurchaserTargetGroup = $("#gridPurchaserTargetGroup").LoadDataToGridWithInitial(0, false, false,
                                "/Contract/CTS020_GetPurchaserCustomerGroupData",
                                "",
                                "dtCustomeGroupData", false);
}
function GetCTS020_06_SectionData() {
    var ischecked = $("#BranchContract").prop("checked");
    return {
        PurchaserSignerTypeCode: $("#CPPurchaserSignerTypeCode").val(),
        IsBranchChecked: ischecked,
        BranchNameEN: $("#BranchNameEN").val(),
        BranchNameLC: $("#BranchNameLC").val(),
        BranchAddressEN: $("#BranchAddressEN").val(),
        BranchAddressLC: $("#BranchAddressLC").val()
    };
}
function SetCTS020_06_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#divPurchaserInfo").SetViewMode(true);
        $("#divSpecifyPurchaser").hide();
        $("#divBranchContract").hide();
    }
    else {
        $("#divPurchaserInfo").SetViewMode(false);
        $("#divSpecifyPurchaser").show();
        $("#divBranchContract").show();
    }
}
function SetCTS020_06_EnableSection(enable) {
    $("#divSpecifyPurchaser").SetEnableView(enable);

    if (enable) {
        $("#BranchContract").removeAttr("disabled");
        $("#BranchNameEN").removeAttr("readonly");
        $("#BranchNameLC").removeAttr("readonly");
        $("#BranchAddressEN").removeAttr("readonly");
        $("#BranchAddressLC").removeAttr("readonly");

        $("#btnCPNewEditCustomer").removeAttr("disabled");
        $("#btnCPClearCustomer").removeAttr("disabled");
        $("#CPPurchaserSignerTypeCode").removeAttr("disabled");
    }
    else {
        $("#BranchContract").attr("disabled", true);
        $("#BranchNameEN").attr("readonly", true);
        $("#BranchNameLC").attr("readonly", true);
        $("#BranchAddressEN").attr("readonly", true);
        $("#BranchAddressLC").attr("readonly", true);

        $("#btnCPNewEditCustomer").attr("disabled", true);
        $("#btnCPClearCustomer").attr("disabled", true);
        $("#CPPurchaserSignerTypeCode").attr("disabled", true);
    }
}


/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "CPSearchCustCode",
        "BranchNameEN",
        "BranchAddressEN",
        "BranchNameLC",
        "BranchAddressLC"
    ]);

    InitialPurchaserSection();
    InitialPurchaserEvents();

    if (processType == 2)
        SetCTS020_06_EnableSection(false);
});

function InitialPurchaserSection(clear_data) {
    var branchChecked = false;

    /* --- Enable Control --- */
    if ($("#HasCustomerData").val() == "True") {
        $("#CPSearchCustCode").attr("readonly", true);
        $("#btnCPRetrieve").attr("disabled", true);
        $("#btnCPSearchCustomer").attr("disabled", true);
        $("#HasCustomerData").val("");

        if ($("#BranchContract").prop("checked") == true)
            branchChecked = true;

        if ($("#CPSearchCustCode").val() != "") {
            $("#btnCPNewEditCustomer").attr("disabled", true);
        }
        else {
            $("#btnCPNewEditCustomer").removeAttr("disabled");
        }
    }
    else {
        $("#CPSearchCustCode").removeAttr("readonly");
        $("#btnCPRetrieve").removeAttr("disabled");
        $("#btnCPSearchCustomer").removeAttr("disabled");
        $("#btnCPNewEditCustomer").removeAttr("disabled");
        $("#divPurchaserInfoData").clearForm();
    }

    /* --- Clear Data from Control --- */
    if (clear_data == undefined || clear_data == true) {
        ContractBranchInfoVisible(branchChecked);
    }
}
function InitialPurchaserEvents() {
    $("#btnCPRetrieve").click(retrieve_contract_customer_click);
    $("#btnCPSearchCustomer").click(search_contract_customer_click);
    $("#btnCPNewEditCustomer").click(new_edit_contract_customer_click);
    $("#btnCPClearCustomer").click(clear_contract_customer_click);
    $("#BranchContract").change(branch_contract_change);
}
/* ----------------------------------------------------------------------------------- */

/* --- Events ------------------------------------------------------------------------- */
/* ------------------------------------------------------------------------------------ */
function retrieve_contract_customer_click() {
    RetrievePurchaserCustomerData();
}
function search_contract_customer_click() {
    SearchCustomerData(function (data) {
        var code = data.CustCode;
        if (code == undefined)
            code = "";

        $("#CPSearchCustCode").val(code);
        RetrievePurchaserCustomerData();
    });
}
function new_edit_contract_customer_click() {
    NewEdit_CustomerData(1);
}
function clear_contract_customer_click() {
    if ($("#SameCustomer").val() == "True") {
        //clear_real_customer_click();
        $("#SameCustomer").val("False");
        if ($("#RCSearchCustCode").val() == "") {
            $("#btnRCNewEditCustomer").removeAttr("disabled");
        }
    }

    $("#CPSearchCustCode").val("");
    InitialPurchaserSection(false);
    DeleteAllRow(gridPurchaserTargetGroup);
    ClearInitialData(1);
}
function branch_contract_change() {
    ContractBranchInfoVisible($(this).prop("checked"));
}
/* ------------------------------------------------------------------------------------ */

/* --- Methods ------------------------------------------------------------------------ */
/* ------------------------------------------------------------------------------------ */
function RetrievePurchaserCustomerData() {
    RetrieveCustomerData($("#CPSearchCustCode").val(), 1, function (result, controls) {
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["CPSearchCustCode"], ["CPSearchCustCode"]);
            /* --------------------- */

            return;
        }

        /* --- Set Data --- */
        /* ---------------- */
        ViewPurchaserCustomerData(result[0]);
        /* ---------------- */

        if (result[1] == true) {
            if (result[0].CustCode != undefined) {
                $("#btnRCNewEditCustomer").attr("disabled", true);
            }
            else {
                $("#btnRCNewEditCustomer").removeAttr("disabled");
            }

            $("#SameCustomer").val("True");
        }

        if (result[0].CustCode != undefined) {
            $("#btnCPNewEditCustomer").attr("disabled", true);
        }
    });
}
function ViewPurchaserCustomerData(contractData) {
    var signerType = $("#CPPurchaserSignerTypeCode").val();
    $("#divPurchaserInfoData").clearForm();

    /* --- Fill Data to Control --- */
    /* ---------------------------- */
    $("#divPurchaserInfoData").bindJSON_Prefix("CP", contractData);
    $("#CPSearchCustCode").val(contractData.CustCodeShort);
    $("#CPPurchaserSignerTypeCode").val(signerType);
    /* ---------------------------- */

    /* --- Disable Control --- */
    /* ----------------------- */
    $("#CPSearchCustCode").attr("readonly", true);
    $("#btnCPRetrieve").attr("disabled", true);
    $("#btnCPSearchCustomer").attr("disabled", true);
    /* ----------------------- */

    $("#gridPurchaserTargetGroup").LoadDataToGrid(gridPurchaserTargetGroup, 0, false,
                                            "/Contract/CTS020_GetPurchaserCustomerGroupData",
                                            "",
                                            "dtCustomeGroupData", false);
}
function ContractBranchInfoVisible(visible) {
    if (visible) {
        $("#BranchContractSection").show();
    }
    else {
        $("#BranchNameEN").val("");
        $("#BranchAddressEN").val("");
        $("#BranchNameLC").val("");
        $("#BranchAddressLC").val("");
        $("#BranchContractSection").hide();
    }
}
/* ----------------------------------------------------------------------------------- */


