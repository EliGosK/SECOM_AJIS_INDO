/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */

$(document).ready(function () {
    SetInitialStateCTS130_04();
    InitialEventCTS130_04();
});

function SetInitialStateCTS130_04() {
    $("#PC_CustomerCodeSpecify").val("");
    SetReadonlySpecifyCodeCTS130_04(true);

    if ($("#PC_BranchNameEnglish").val() != "" || $("#PC_BranchAddressEnglish").val() != ""
        || $("#PC_BranchNameLocal").val() != "" || $("#PC_BranchAddressLocal").val() != "") {
        SetShowBranchSectionCTS130_04(true);
    }
    else {
        SetShowBranchSectionCTS130_04(false);
    }

    $("#PC_ContractSignerType").val($("#PC_ContractSignerTypeVal").val());

    gridContractTargetPurchaserCTS130 = $("#gvContractTargetPurchaser").LoadDataToGridWithInitial(0, false, false, "/Contract/CTS130_GetContractTargetPurchaserListData", "", "dtCustomeGroupData", false);

    $("#PC_btnNewEditCustomer").attr("disabled", $("#PC_CustomerCode").val() != "");
}

function SetReadonlySpecifyCodeCTS130_04(isReadonly) {
    $("#PC_CustomerCodeSpecify").attr("readonly", isReadonly);
    $("#PC_btnRetrieve").attr("disabled", isReadonly);
    $("#PC_btnSearchCustomer").attr("disabled", isReadonly);
}

function SetShowBranchSectionCTS130_04(isShow) {
    if (isShow) {
        $("#divBranchSection").show();
    }
    else {
        $("#divBranchSection").hide();
    }

    $("#PC_BranchContract").attr("checked", isShow);
}

function InitialEventCTS130_04() {
    $("#PC_btnRetrieve").click(retrieve_button_click_CTS130_04);
    $("#PC_btnSearchCustomer").click(search_button_click_CTS130_04);
    $("#PC_btnNewEditCustomer").click(newedit_button_click_CTS130_04);
    $("#PC_btnClearCustomer").click(clear_button_click_CTS130_04);
    $("#PC_btnViewCustomerHistory").click(view_button_click_CTS130_04);
    $("#PC_BranchContract").click(branch_contract_click_CTS130_04);
}

function retrieve_button_click_CTS130_04() {
    var obj = {
        strCustomerCode: $("#PC_CustomerCodeSpecify").val()
    };

    call_ajax_method_json("/Contract/CTS130_RetrieveContractTargetPurchaserData", obj,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["PC_CustomerCodeSpecify"], controls);

            } else if (result != undefined) {
                BindDataCTS130_04(result);
                SetReadonlySpecifyCodeCTS130_04(true);

                $("#PC_btnNewEditCustomer").attr("disabled", $("#PC_CustomerCode").val() != "");
            }
        }
    );
}

function BindDataCTS130_04(result) {
    $("#PC_CustomerCode").val(result.CustCodeShort);
    $("#PC_CustomerStatus").val(result.CustStatusCodeName);
    $("#PC_CustomerType").val(result.CustTypeCodeName);
    $("#PC_NameEnglish").val(result.CustFullNameEN);
    $("#PC_AddressEnglish").val(result.AddressFullEN);
    $("#PC_NameLocal").val(result.CustFullNameLC);
    $("#PC_AddressLocal").val(result.AddressFullLC);
    $("#PC_Nationality").val(result.Nationality);
    $("#PC_TelephoneNo").val(result.PhoneNo);
    $("#PC_BusinessType").val(result.BusinessTypeName);
    $("#PC_IDTaxID").val(result.IDNo);
    $("#PC_URL").val(result.URL);

    $("#gvContractTargetPurchaser").LoadDataToGrid(gridContractTargetPurchaserCTS130, 0, false, "/Contract/CTS130_GetContractTargetPurchaserListData",
                                                "", "dtCustomeGroupData", false, null, null);
}

/*------ CMS250 Dialog ------*/
/*---------------------------*/
function search_button_click_CTS130_04() {
    customerSearchType = "PC";
    $("#dlgCTS130").OpenCMS250Dialog("CTS130");
}
function CMS250Object() {
    return {
        bExistCustOnlyFlag: false
    };
}
function CMS250Response(result) {
    var custCode = "";
    if (result != undefined && result.CustomerData != undefined) {
        custCode = result.CustomerData.CustCode;
    }
    $("#dlgCTS130").CloseDialog();

    if (customerSearchType == "PC") {
        $("#PC_CustomerCodeSpecify").val(custCode);
        retrieve_button_click_CTS130_04();
    }
    else if (customerSearchType == "RC") {
        $("#RC_CustomerCodeSpecify").val(custCode);
        retrieve_button_click_CTS130_05();
    }
}
/*--------------------------*/

/*------ MAS050 Dialog ------*/
/*---------------------------*/
var doCustomerObject = null;
function newedit_button_click_CTS130_04() {
    customerSearchType = "PC";

    call_ajax_method_json("/Contract/CTS130_GetContractTargetPurchaserData", "",
        function (result) {
            newedit_button_click_MAS050(result);
        });

}
/*--------------------------*/

function clear_button_click_CTS130_04() {
    var obj = { customerData: null };
    call_ajax_method_json("/Contract/CTS130_UpdateContractTargetPurchaserData", obj,
        function (result, controls) {
            if (result != undefined) {
                ClearDataCTS130_4();
                SetReadonlySpecifyCodeCTS130_04(false);

                $("#PC_btnNewEditCustomer").attr("disabled", false);
            }
        }
    );
}

function ClearDataCTS130_4() {
    $("#PC_CustomerCodeSpecify").val("");
    $("#divPurchaserDetail").clearForm();

    $("#gvContractTargetPurchaser").LoadDataToGrid(gridContractTargetPurchaserCTS130, 0, false, "/Contract/CTS130_GetContractTargetPurchaserListData",
                                                "", "dtCustomeGroupData", false, null, null);
}

/*------ CMS300 Dialog ------*/
/*---------------------------*/
function view_button_click_CTS130_04() {
    $("#dlgCTS130").OpenCMS300Dialog("CTS130");
}
function CMS300Object() {
    return {
        ContractCode: contractCode,
        OCC: "",
        CSCustCode: "",
        RCCustCode: "",
        SiteCode: "",
        ServiceTypeCode: $("#ServiceTypeCode").val()
    };
}
/*--------------------------*/

function branch_contract_click_CTS130_04() {
    if ($("#PC_BranchContract").prop("checked")) {
        SetShowBranchSectionCTS130_04(true);

        $("#PC_BranchNameEnglish").val("");
        $("#PC_BranchAddressEnglish").val("");
        $("#PC_BranchNameLocal").val("");
        $("#PC_BranchAddressLocal").val("");
    }
    else {
        SetShowBranchSectionCTS130_04(false);
    }
}

function SetSectionModeCTS130_04(isView) {
    $("#divContractTargetPurchaserSection").SetViewMode(isView);

    if (isView) {
        $("#divSpecifyCode_PC").hide();
    }
    else {
        $("#divSpecifyCode_PC").show();
    }
}