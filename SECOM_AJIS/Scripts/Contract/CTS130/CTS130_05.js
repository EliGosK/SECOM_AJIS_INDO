/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */

$(document).ready(function () {
    SetInitialStateCTS130_05();
    InitialEventCTS130_05();
});

function SetInitialStateCTS130_05() {
    $("#RC_CustomerCodeSpecify").val("");
    SetReadonlySpecifyCodeCTS130_05(true);

    gridRealCustomerCTS130 = $("#gvRealCustomer").LoadDataToGridWithInitial(0, false, false, "/Contract/CTS130_GetRealCustomerListData", "", "dtCustomeGroupData", false);

    $("#RC_btnNewEditCustomer").attr("disabled", $("#RC_CustomerCode").val() != "");
}

function SetReadonlySpecifyCodeCTS130_05(isReadonly) {
    $("#RC_CustomerCodeSpecify").attr("readonly", isReadonly);
    $("#RC_btnRetrieve").attr("disabled", isReadonly);
    $("#RC_btnSearchCustomer").attr("disabled", isReadonly);
    $("#RC_btnSameAsContractTartget").attr("disabled", isReadonly);
}

function InitialEventCTS130_05() {
    $("#RC_btnRetrieve").click(retrieve_button_click_CTS130_05);
    $("#RC_btnSearchCustomer").click(search_button_click_CTS130_05);
    $("#RC_btnNewEditCustomer").click(newedit_button_click_CTS130_05);
    $("#RC_btnClearCustomer").click(clear_button_click_CTS130_05);
    $("#RC_btnSameAsContractTartget").click(same_purchaser_button_click_CTS130_05);
}

function retrieve_button_click_CTS130_05() {
    var obj = {
        strCustomerCode: $("#RC_CustomerCodeSpecify").val()
    };

    call_ajax_method_json("/Contract/CTS130_RetrieveRealCustomerData", obj,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["RC_CustomerCodeSpecify"], controls);

            } else if (result != undefined) {
                BindDataCTS130_05(result);
                SetReadonlySpecifyCodeCTS130_05(true);

                $("#RC_btnNewEditCustomer").attr("disabled", $("#RC_CustomerCode").val() != "");
            }
        }
    );
}

function BindDataCTS130_05(result) {
    $("#RC_CustomerCode").val(result.CustCodeShort);
    $("#RC_CustomerStatus").val(result.CustStatusCodeName);
    $("#RC_CustomerType").val(result.CustTypeCodeName);
    $("#RC_NameEnglish").val(result.CustFullNameEN);
    $("#RC_AddressEnglish").val(result.AddressFullEN);
    $("#RC_NameLocal").val(result.CustFullNameLC);
    $("#RC_AddressLocal").val(result.AddressFullLC);
    $("#RC_Nationality").val(result.Nationality);
    $("#RC_TelephoneNo").val(result.PhoneNo);
    $("#RC_BusinessType").val(result.BusinessTypeName);
    $("#RC_IDTaxID").val(result.IDNo);
    $("#RC_URL").val(result.URL);

    $("#ST_SpecifySiteCustCode").val(result.SiteCustCodeShort);

    $("#gvRealCustomer").LoadDataToGrid(gridRealCustomerCTS130, 0, false, "/Contract/CTS130_GetRealCustomerListData",
                                                "", "dtCustomeGroupData", false, null, null);
}

/*------ CMS250 Dialog ------*/
/*---------------------------*/
function search_button_click_CTS130_05() {
    customerSearchType = "RC";
    $("#dlgCTS130").OpenCMS250Dialog("CTS130");
}
/*--------------------------*/

/*------ MAS050 Dialog ------*/
/*---------------------------*/
function newedit_button_click_CTS130_05() {
    customerSearchType = "RC";

    call_ajax_method_json("/Contract/CTS130_GetRealCustomerData", "",
        function (result) {
            newedit_button_click_MAS050(result);
        });
}
/*---------------------------*/

function clear_button_click_CTS130_05() {
    var obj = { customerData: null };
    call_ajax_method_json("/Contract/CTS130_UpdateRealCustomerData", obj,
        function (result, controls) {
            if (result != undefined) {
                ClearDataCTS130_5();
                SetReadonlySpecifyCodeCTS130_05(false);

                $("#RC_btnNewEditCustomer").attr("disabled", false);

                clear_button_click_CTS130_06();
            }
        }
    );
}

function ClearDataCTS130_5() {
    $("#RC_CustomerCodeSpecify").val("");
    $("#divRealCustomerDetail").clearForm();

    $("#gvRealCustomer").LoadDataToGrid(gridRealCustomerCTS130, 0, false, "/Contract/CTS130_GetRealCustomerListData",
                                                "", "dtCustomeGroupData", false, null, null);
}

function same_purchaser_button_click_CTS130_05() {
    call_ajax_method_json("/Contract/CTS130_SetSameContractTargetPurchaserData", "",
        function (result, controls) {
            if (result != undefined) {
                BindDataCTS130_05(result[0]);
                SetReadonlySpecifyCodeCTS130_05(true);

                $("#RC_btnNewEditCustomer").attr("disabled", $("#RC_CustomerCode").val() != "");

                if (result[1] == undefined) {
//                    ClearDataCTS130_6();
//                    SetInitialStateCTS130_06();
                    ClearDataCTS130_6();
                    SetReadonlySpecifyCodeCTS130_06(false);
                    SetReadonlyCopyNameCTS130_06(false);

                    $("#ST_rdoContractTargetPurchaser").attr("checked", true); //default
                    $("#ST_btnNewEditSiteInfo").attr("disabled", false);
                }
            }
        }
    );
}

function SetSectionModeCTS130_05(isView) {
    $("#divRealCustomerSection").SetViewMode(isView);

    if (isView) {
        $("#divSpecifyCode_RC").hide();
        $("#divSameAsContractTartget").hide();
    }
    else {
        $("#divSpecifyCode_RC").show();
        $("#divSameAsContractTartget").show();
    }
}