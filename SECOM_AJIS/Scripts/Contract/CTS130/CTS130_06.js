/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */

$(document).ready(function () {
    SetInitialStateCTS130_06();
    InitialEventCTS130_06();
});

function SetInitialStateCTS130_06() {
    $("#ST_SpecifySiteNo").val("");
    SetReadonlySpecifyCodeCTS130_06(true);
    SetReadonlyCopyNameCTS130_06(true);

    $("#ST_btnNewEditSiteInfo").attr("disabled", $("#ST_SiteCode").val() != "");
}

function SetReadonlySpecifyCodeCTS130_06(isReadonly) {
    $("#ST_SpecifySiteNo").attr("readonly", isReadonly);
    $("#ST_btnRetrieve").attr("disabled", isReadonly);
    $("#ST_btnSearchSite").attr("disabled", isReadonly);
}

function SetReadonlyCopyNameCTS130_06(isReadonly) {
    $("#ST_rdoContractTargetPurchaser").attr("disabled", isReadonly);
    $("#ST_rdoRealCustomerEndUser").attr("disabled", isReadonly);
    $("#ST_btnCopy").attr("disabled", isReadonly);
}

function InitialEventCTS130_06() {
    $("#ST_btnRetrieve").click(retrieve_button_click_CTS130_06);
    $("#ST_btnSearchSite").click(search_button_click_CTS130_06);
    $("#ST_btnCopy").click(copy_button_click_CTS130_06);
    $("#ST_btnNewEditSiteInfo").click(newedit_button_click_CTS130_06);
    $("#ST_btnClearSite").click(clear_button_click_CTS130_06);
}

function retrieve_button_click_CTS130_06() {
    var obj = {
        RealCustomerCode: $("#RC_CustomerCode").val(),
        SiteCustCode: $("#ST_SpecifySiteCustCode").val(),
        SiteNo: $("#ST_SpecifySiteNo").val()
    };

    call_ajax_method_json("/Contract/CTS130_RetrieveSiteData", obj,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["RC_CustomerCodeSpecify",
                              "ST_SpecifySiteNo"], controls);

            } else if (result != undefined) {
                BindDataCTS130_06(result);
                SetReadonlySpecifyCodeCTS130_06(true);
                SetReadonlyCopyNameCTS130_06(true);

                $("#ST_btnNewEditSiteInfo").attr("disabled", $("#ST_SiteCode").val() != "");
            }
        }
    );
}

function BindDataCTS130_06(result) {
    $("#ST_SiteCode").val(result.SiteCodeShort);
    $("#ST_NameEnglish").val(result.SiteNameEN);
    $("#ST_AddressEnglish").val(result.AddressFullEN);
    $("#ST_NameLocal").val(result.SiteNameLC);
    $("#ST_AddressLocal").val(result.AddressFullLC);
    $("#ST_TelephoneNo").val(result.PhoneNo);
    $("#ST_Usage").val(result.BuildingUsageName);
}

function search_button_click_CTS130_06() {
    var obj = {
        RealCustomerCode: $("#RC_CustomerCode").val(),
        SiteCustCode: "",
        SiteNo: ""
    };

    call_ajax_method_json("/Contract/CTS130_SearchSiteData", obj,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["RC_CustomerCodeSpecify",
                              "ST_SpecifySiteNo"], controls);

            } else if (result != undefined) {
                $("#dlgCTS130").OpenCMS260Dialog("CTS130");
            }
        }
    );
}

function CMS260Object() {
    return {
        strRealCustomerCode: $("#RC_CustomerCode").val()
    };
}
function CMS260Response(result) {
    var siteNo = "";
    if (result != undefined) {
        siteNo = result.SiteNo;
    }
    $("#dlgCTS130").CloseDialog();

    $("#ST_SpecifySiteNo").val(siteNo);
    retrieve_button_click_CTS130_06();
}

function copy_button_click_CTS130_06() {
    var copyType = "0";
    if ($("#ST_rdoContractTargetPurchaser").prop("checked")) {
        copyType = $("#ST_rdoContractTargetPurchaser").val();
    }
    else if ($("#ST_rdoRealCustomerEndUser").prop("checked")) {
        copyType = $("#ST_rdoRealCustomerEndUser").val();
    }

    var obj = {
        CopyType: copyType,
        BranchContractFlag: $("#PC_BranchContract").prop("checked"),
        BranchNameEN: $("#PC_BranchNameEnglish").val(),
        BranchNameLC: $("#PC_BranchNameLocal").val()
    };

    call_ajax_method_json("/Contract/CTS130_CopySiteName", obj,
        function (result, controls) {
            if (result != undefined) {
                $("#ST_SpecifySiteNo").val("");

                BindDataCTS130_06(result);

                SetReadonlySpecifyCodeCTS130_06(true);
                SetReadonlyCopyNameCTS130_06(true);
            }
        }
    );
}

/*------ MAS040 Dialog ------*/
/*---------------------------*/
var doSiteObject = null;
function newedit_button_click_CTS130_06() {
    call_ajax_method_json("/Contract/CTS130_GetSiteData", "",
        function (result) {
            //if (result != undefined) {
                doSiteObject = result;
            //}
            $("#dlgCTS130").OpenMAS040Dialog("CTS130");
        });
}
function MAS040Object() {
    return doSiteObject;
}
function MAS040Response(result) {
    var obj = { siteData: result };
    $("#dlgCTS130").CloseDialog();

    call_ajax_method_json("/Contract/CTS130_UpdateSiteData", obj,
    function (result, controls) {
        if (result != undefined) {
            BindDataCTS130_06(obj.siteData);
            SetReadonlySpecifyCodeCTS130_06(true);
            SetReadonlyCopyNameCTS130_06(true);
        }
    });
}
/*--------------------------*/

function clear_button_click_CTS130_06() {
    var obj = { siteData: null };
    call_ajax_method_json("/Contract/CTS130_UpdateSiteData", obj,
        function (result, controls) {
            if (result != undefined) {
                ClearDataCTS130_6();
                SetReadonlySpecifyCodeCTS130_06(false);
                SetReadonlyCopyNameCTS130_06(false);

                $("#ST_rdoContractTargetPurchaser").attr("checked", true); //default
                $("#ST_btnNewEditSiteInfo").attr("disabled", false);
            }
        }
    );
}

function ClearDataCTS130_6() {
    $("#ST_SpecifySiteNo").val("");
    $("#divSiteDetail").clearForm();

    if ($("#RC_CustomerCode").val() == "") {
        $("#ST_SpecifySiteCustCode").val("");
    }
}

function SetSectionModeCTS130_06(isView) {
    $("#divSiteSection").SetViewMode(isView);

    if (isView) {
        $("#divSpecifyCode_ST").hide();
    }
    else {
        $("#divSpecifyCode_ST").show();

        if ($("#ST_btnCopy").prop("disabled")) {
            SetReadonlyCopyNameCTS130_06(true);
        }
    }
}