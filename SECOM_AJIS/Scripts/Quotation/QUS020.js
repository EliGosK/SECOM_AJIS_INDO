/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../Common/Dialog.js" />
/// <reference path="../Master/Dialog.js" />
/// <reference path="Dialog.js" />

/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function InitialCommandButton(mode) {
    if (mode == 0) {
        register_command.SetCommand(command_register_click);
        reset_command.SetCommand(command_reset_click);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }
    else if (mode == 1) {
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(command_confirm_click);
        back_command.SetCommand(command_back_click);
    }
    else if (mode == 2) {
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }
}


$(document).ready(function () {
    InitialTrimTextEvent([
        "CPSearchCustCode",
        "BranchNameEN",
        "BranchAddressEN",
        "BranchNameLC",
        "BranchAddressLC",
        "RCSearchCustCode",
        "SiteCustCodeNo",
        "IntroducerCode",
        "OldContractCode",
        "QuotationStaffEmpNo"
    ]);


    $("#CurrentDialogKey").val(ajax_method.GetKeyURL(null));
    InitialCommandButton(0);
    InitialSection();
    InitialEvents();


    $("#ContractTargetMemo").SetMaxLengthTextArea(300);
    $("#RealCustomerMemo").SetMaxLengthTextArea(300);

    /* --- Merge --- */
    $("#CPSearchCustCode").focus();
    /* ------------- */
});


function InitialSection() {
    /* --- Hide Section --- */
    $("#divResultRegister").hide();
    $("#btnRegisterQuotationDetail").attr("disabled", true);
    $("#btnRegisterNextQuotationTarget").attr("disabled", true);

    InitialContractCustomerSection(true);
    InitialRealCustomerSection(true);
    InitialSiteCustomerSection();
    InitialMoreInformationSection();
}
function InitialEvents() {
    ImportSectionEvents();
    ContractCustomerSection();
    RealCustomerSection();
    SiteCustomerSection();
    MoreInfomationSection();

    $("#btnRegisterQuotationDetail").click(register_quotation_detail_click);
    $("#btnRegisterNextQuotationTarget").click(register_next_quotation_target_click);
}
/* ----------------------------------------------------------------------------------- */

/* --- Events ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function register_quotation_detail_click() {
    ajax_method.CallScreenController("/Quotation/QUS020_RegisterQuotationDetail", "", function (result) {
        var obj = {
            QuotationKey: {
                QuotationTargetCode: $("#QuotationTargetCode").val()
            },
            ImportKey: result
        };
        ajax_method.CallScreenControllerWithAuthority("/Quotation/QUS030", obj);
    });
}
function register_next_quotation_target_click() {
    ajax_method.CallScreenController("/Quotation/ResetSessionData", "", function (result, controls) {
        SetContractCustomerSectionMode(false);
        SetRealCustomerSectionMode(false);
        SetSiteCustomerSectionMode(false);
        SetMoreInformationSectionMode(false);

        InitialCommandButton(0);
        InitialSection();

        $("#CPSearchCustCode").focus();
        master_event.ScrollToTopWindow();
    });
}
/* ----------------------------------------------------------------------------------- */

/* --- Dialog Methods (CMS250) ------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
var searchFunction = null;
function SearchCustomerData(func) {
    searchFunction = func;
    $("#dlgBox").OpenCMS250Dialog();
}
function CMS250Object() {
    return { "bExistCustOnlyFlag": false };
}
function CMS250Response(result) {
    $("#dlgBox").CloseDialog();

    if (typeof (searchFunction) == "function") {
        searchFunction(result.CustomerData);
    }
}
/* ----------------------------------------------------------------------------------- */

/* --- Dialog Methods (CMS260) ------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
var searchSiteFunction = null;
function SearchSiteData(func) {
    searchSiteFunction = func;
    $("#dlgBox").OpenCMS260Dialog();
}
function CMS260Object() {
    return {
        strRealCustomerCode: $("#RCSearchCustCode").val()
    };
}
function CMS260Response(result) {
    $("#dlgBox").CloseDialog();

    if (typeof (searchSiteFunction) == "function") {
        searchSiteFunction(result);
    }
}
/* ----------------------------------------------------------------------------------- */

/* --- Dialog Methods (MAS050) ------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
var callCustomerFlag = -1;
var doCustomer = null;
function NewEdit_CustomerData(flag) {
    /* --- Set Flag --- */
    /* ---------------- */
    callCustomerFlag = flag;
    /* ---------------- */

    GetRegisterData(flag, function (result) {
        doCustomer = result;
        $("#dlgBox").OpenMAS050Dialog("QUS020");
    });
}
function MAS050Object() {
    return {
        doCustomer: doCustomer
    };
}
function MAS050Response(doCustomer) {
    $("#dlgBox").CloseDialog();

    if (callCustomerFlag == 1) {
        /* --- Reset Control --- */
        /* --------------------- */
        $("#CPSearchCustCode").val("");
        /* --------------------- */

        /* --- Set Data --- */
        /* ---------------- */
        var obj = {
            doContractTargetData: doCustomer,
            ObjectTypeID: 1
        };
        SetRegisterData(obj, function () {
            ViewContractCustomerData(doCustomer);
        });
        /* ---------------- */
    }
    else if (callCustomerFlag == 2) {
        /* --- Reset Control --- */
        $("#RCSearchCustCode").val("");
        /* --------------------- */

        /* --- Set Data --- */
        /* ---------------- */
        var obj = {
            doRealCustomerData: doCustomer,
            ObjectTypeID: 2
        };
        SetRegisterData(obj, function () {
            ViewRealCustomerData(doCustomer);
        });
        /* ---------------- */
    }
}
/* ----------------------------------------------------------------------------------- */

/* --- Dialog Methods (MAS040) ------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
var doSite = null;
function NewEdit_SiteData() {
    GetRegisterData(3, function (result) {
        doSite = result;
        $("#dlgBox").OpenMAS040Dialog("QUS020");
    });
}
function MAS040Object() {
    return doSite;
}
function MAS040Response(doSite) {
    $("#dlgBox").CloseDialog();

    /* ---------------- */
    var obj = {
        doQuotationSiteData: doSite,
        ObjectTypeID: 3
    };
    SetRegisterData(obj, function () {
        ViewSiteCustomerData(doSite, true);
    });
    /* ---------------- */
}
/* ----------------------------------------------------------------------------------- */

/* --- Methods ----------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function SetRegisterData(object, func) {
    ajax_method.CallScreenController("/Quotation/QUS020_SetInitialQuotationData", object, function (result) {
        if (result == true) {
            if (typeof (func) == "function")
                func();
        }
    });
}
function GetRegisterData(objectTypeID, func) {
    var obj = {
        ObjectTypeID: objectTypeID
    };
    ajax_method.CallScreenController("/Quotation/QUS020_GetInitialQuotationData", obj, function (result) {
        if (typeof (func) == "function")
            func(result);
    });
}
function ClearRegisterData(objectTypeID) {
    var object = {
        ObjectTypeID: objectTypeID
    };
    SetRegisterData(object);
}
function RetrieveCustomerData(custCode, custType, func) {
    /* --- Set Parameter --- */
    /* --------------------- */
    var obj = {
        CustCode: custCode,
        CustType: custType
    };
    /* --------------------- */

    /* --- Call Event --- */
    /* ------------------ */
    master_event.LockWindow(true);
    ajax_method.CallScreenController("/Quotation/QUS020_RetrieveCustomer", obj, function (result, controls) {
        master_event.LockWindow(false);
        if (func != null) {
            func(result, controls);
        }
    });
    /* ------------------ */
}
/* ----------------------------------------------------------------------------------- */





/* ----------------------------------------------------------------------------------- */
/* --- Import Section ---------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */

/* --- Import Section  > Initial ----------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function ImportSectionEvents() {
    $("#btnImportQuotationInfo").click(importQuotationInfo_click);
}
/* ----------------------------------------------------------------------------------- */

/* --- Import Section  > Events ------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function importQuotationInfo_click() {
    $("#dlgBox").OpenQUS050Dialog("QUS020");
}
/* ----------------------------------------------------------------------------------- */

/* --- Import Section  > Dialog Methods ---------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function QUS050Response(dsImportData, key) {
    $("#dlgBox").CloseDialog();

    if (dsImportData != undefined) {
        InitialSection();

        FillAllImportData(dsImportData, key);
    }
}
function QUS050Object() {
    return {
        ScreenID: "QUS020"
    };
}
/* ----------------------------------------------------------------------------------- */

/* --- Import Section  > Methods ----------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function FillAllImportData(dsImportData, key) {
    ajax_method.CallScreenController("/Quotation/QUS020_InitImportData", { ImportKey: key }, function (result, controls) {
        var hasContractTarget = true;
        var hasReal = true;
        var hasSite = true;

        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["CPSearchCustCode",
                            "RCSearchCustCode",
                            "SiteCustCodeNo",
                            "ProductTypeCode",
                            "QuotationOfficeCode",
                            "OperationOfficeCode",
                            "AcquisitionTypeCode",
                            "MotivationTypeCode"], controls);
            /* --------------------- */

            for (var idx = 0; idx < controls.length; idx++) {
                if (controls[idx] == "CPSearchCustCode")
                    hasContractTarget = false;
                if (controls[idx] == "RCSearchCustCode")
                    hasReal = false;
                if (controls[idx] == "SiteCustCodeNo")
                    hasSite = false;
            }
        }

        if (result != undefined) {
            if (result.doContractTargetData != undefined) {
                if (hasContractTarget) {
                    $("#CPSearchCustCode").val(result.doContractTargetData.CustCodeShort);
                    ViewContractCustomerData(result.doContractTargetData);
                }
                else {
                    $("#CPSearchCustCode").val(result.doContractTargetData.CustCodeShort);
                }
            }
            if (result.doRealCustomerData != undefined) {
                if (hasReal) {
                    $("#RCSearchCustCode").val(result.doRealCustomerData.CustCodeShort);
                    ViewRealCustomerData(result.doRealCustomerData);
                }
                else {
                    $("#RCSearchCustCode").val(result.doRealCustomerData.CustCodeShort);
                }
            }
            if (result.doQuotationSiteData != undefined) {
                $("#SiteCustCodeNo").val(result.doQuotationSiteData.SiteNo);

                if (hasReal && hasSite) {
                    $("#SiteCustCode").val(result.doRealCustomerData.SiteCustCodeShort);

                    var iscopy = true;
                    if (result.doRealCustomerData.SiteCustCodeShort != "" && result.doRealCustomerData.SiteCustCodeShort != undefined)
                        iscopy = false;

                    ViewSiteCustomerData(result.doQuotationSiteData, iscopy);
                }
                else if (hasReal == true) {
                    $("#SiteCustCode").val(result.doRealCustomerData.SiteCustCodeShort);
                }
            }
            if (result.doQuotationTargetData != null) {
                $("#divMoreInformation").bindJSON(result.doQuotationTargetData);
                $("#ContractTargetMemo").val(result.doQuotationTargetData.ContractTargetMemo);
                $("#RealCustomerMemo").val(result.doQuotationTargetData.RealCustomerMemo);

                GetQuotationStaffName();

                if (result.doQuotationTargetData.BranchNameEN != undefined
                    || result.doQuotationTargetData.BranchAddressEN != undefined
                    || result.doQuotationTargetData.BranchNameLC != undefined
                    || result.doQuotationTargetData.BranchAddressLC != undefined) {
                    ContractBranchInfoVisible(true);
                    $("#BranchContract").attr("checked", true);
                    $("#BranchNameEN").val(result.doQuotationTargetData.BranchNameEN);
                    $("#BranchAddressEN").val(result.doQuotationTargetData.BranchAddressEN);
                    $("#BranchNameLC").val(result.doQuotationTargetData.BranchNameLC);
                    $("#BranchAddressLC").val(result.doQuotationTargetData.BranchAddressLC);
                }

                //Acquistion type
                var va = $("#AcquisitionTypeCode").val();
                if (va == $("#C_ACQUISITION_TYPE_CUST").val()
                    || va == $("#C_ACQUISITION_TYPE_INSIDE_COMPANY").val()

                /* --- Merge --- */
                /* || va == $("#C_ACQUISITION_TYPE_SUBCONTRACTOR").val() */
                /* ------------- */

                    ) {
                    $("#IntroducerCode").removeAttr("readonly");
                }
                else {
                    $("#IntroducerCode").attr("readonly", true);
                }

                //Product type
                var vp = $("#ProductTypeCode").val();
                if (
                /* --- Merge --- */
                /* vp == "" */
                    vp == undefined
                    || vp == ""
                /* ------------- */

                    || vp == $("#C_PROD_TYPE_SALE").val()) {
                    $("#OldContractCode").attr("readonly", true);
                }
                else {
                    $("#OldContractCode").removeAttr("readonly");
                }
            }
        }
    });
}
/* ----------------------------------------------------------------------------------- */

/* ----------------------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */





/* ----------------------------------------------------------------------------------- */
/* --- Contract Customer Section ----------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */

/* --- Contract Customer Section  > Initial ------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function InitialContractCustomerSection(clear_all) {
    $("#divContractTargetPurchaserInfo").ResetToNormalControl();

    /* --- Enable Control --- */
    $("#CPSearchCustCode").removeAttr("readonly");
    $("#btnCPRetrieve").removeAttr("disabled");
    $("#btnCPSearchCustomer").removeAttr("disabled");
    $("#btnCPNewEditCustomer").removeAttr("disabled");

    /* --- Clear Data from Control --- */
    $("#CPSearchCustCode").val("");
    
    $("#divContractTargetPurchaserInfoData").clearForm();

    if (clear_all == true) {
        $("#BranchContract").removeAttr("checked");
        ContractBranchInfoVisible(false);
        $("#ContractTargetMemo").val("");
    }
}
function SetContractCustomerSectionMode(isview) {
    if (isview) {
        $("#divContractTargetPurchaserInfo").SetViewMode(true, true);
        $("#divSpecifyContractTarget").hide();
        $("#divBranchContract").hide();
        $("#btnImportQuotationInfo").hide();
    }
    else {
        $("#divContractTargetPurchaserInfo").SetViewMode(false);
        $("#divSpecifyContractTarget").show();
        $("#divBranchContract").show();
        $("#btnImportQuotationInfo").show();
    }
}
function ContractCustomerSection() {
    $("#btnCPRetrieve").click(retrieve_contract_customer_click);
    $("#btnCPSearchCustomer").click(search_contract_customer_click);
    $("#btnCPNewEditCustomer").click(new_edit_contract_customer_click);
    $("#btnCPClearCustomer").click(clear_contract_customer_click);
    $("#BranchContract").change(branch_contract_change);
}
/* ----------------------------------------------------------------------------------- */

/* --- Contract Customer Section  > Events ------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function retrieve_contract_customer_click() {
    RetrieveContractCustomerData();
}
function search_contract_customer_click() {
    SearchCustomerData(function (data) {
        var code = data.CustCode;
        if (code == undefined)
            code = "";

        $("#CPSearchCustCode").val(code);
        RetrieveContractCustomerData();
    });
}
function new_edit_contract_customer_click() {
    NewEdit_CustomerData(1);
}
function clear_contract_customer_click() {
    InitialContractCustomerSection(false);
    ClearRegisterData(1);
}
function branch_contract_change() {
    ContractBranchInfoVisible($(this).prop("checked"));
}
/* ----------------------------------------------------------------------------------- */

/* --- Contract Customer Section  > Methods ------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function RetrieveContractCustomerData() {
    RetrieveCustomerData($("#CPSearchCustCode").val(), 1, function (result, controls) {
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["CPSearchCustCode"], ["CPSearchCustCode"]);
            /* --------------------- */

            return;
        }
        if (result != undefined) {
            /* --- Set Data --- */
            /* ---------------- */
            ViewContractCustomerData(result);
            /* ---------------- */
        }
    });
}
function ViewContractCustomerData(contractData) {
    $("#divContractTargetPurchaserInfoData").clearForm();

    /* --- Fill Data to Control --- */
    /* ---------------------------- */
    $("#divContractTargetPurchaserInfoData").bindJSON_Prefix("CP", contractData);
    $("#CPSearchCustCode").val(contractData.CustCodeShort);
    /* ---------------------------- */

    /* --- Disable Control --- */
    /* ----------------------- */
    $("#CPSearchCustCode").attr("readonly", true);
    $("#btnCPRetrieve").attr("disabled", true);
    $("#btnCPSearchCustomer").attr("disabled", true);

    if ($("#CPCustCodeShort").val() != "")
        $("#btnCPNewEditCustomer").attr("disabled", true);
    /* ----------------------- */
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

/* ----------------------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */





/* ----------------------------------------------------------------------------------- */
/* --- Real Customer Section --------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */

/* --- Real Customer Section  > Initial ---------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function InitialRealCustomerSection(clear_all) {
    $("#divRealCustomerInfo").ResetToNormalControl();

    /* --- Enable Control --- */
    /* ---------------------- */
    $("#RCSearchCustCode").removeAttr("readonly");
    $("#RCSearchCustCode").ResetToNormalControl();

    $("#btnRCRetrieve").removeAttr("disabled");
    $("#btnRCSearchCustomer").removeAttr("disabled");
    $("#btnRCNewEditCustomer").removeAttr("disabled");
    /* ---------------------- */

    /* --- Clear Data from Control --- */
    /* ------------------------------- */
    $("#divRealCustomerInfo").clearForm();
    /* ------------------------------- */

    if (clear_all == true) {
        $("#RealCustomerMemo").val("");
    }
}
function SetRealCustomerSectionMode(isview) {
    if (isview) {
        $("#divRealCustomerInfo").SetViewMode(true, true);
        $("#divRealCustomerMemo").SetViewMode(true, true);
        $("#divCopyContractInfo").hide();
        $("#divSpecifyRealCustomer").hide();
    }
    else {
        $("#divRealCustomerInfo").SetViewMode(false);
        $("#divRealCustomerMemo").SetViewMode(false);
        $("#divCopyContractInfo").show();
        $("#divSpecifyRealCustomer").show();
    }
}
function RealCustomerSection() {
    $("#btnCopyContractInfo").click(copy_contract_customer_click);
    $("#btnRCRetrieve").click(retrieve_real_customer_click);
    $("#btnRCSearchCustomer").click(search_real_customer_click);
    $("#btnRCNewEditCustomer").click(new_edit_real_customer_click);
    $("#btnRCClearCustomer").click(clear_real_customer_click);
}
/* ----------------------------------------------------------------------------------- */

/* --- Real Customer Section  > Events ----------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function copy_contract_customer_click() {
    /* --- Call Event --- */
    /* ------------------ */
    ajax_method.CallScreenController("/Quotation/QUS020_CopyCustomer", "", function (result) {
        if (result == undefined)
            return;
        if (result.length < 2)
            return;
        ViewRealCustomerData(result[0]);

        if (result[1] == false)
            InitialSiteCustomerSection();

        $("#RCSearchCustCode").val(result[0].CustCodeShort);
        if (result[0].SiteCustCodeShort != undefined) {
            $("#SiteCustCode").val(result[0].SiteCustCodeShort);
        }

        if ($("#RCSearchCustCode").val() == "") {
            $("#btnRCNewEditCustomer").removeAttr("disabled");
        }
    });
    /* ------------------ */
}
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
    InitialRealCustomerSection(false);
    ClearRegisterData(2);
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
        ClearRegisterData(3);
    }
}
/* ----------------------------------------------------------------------------------- */

/* --- Real Customer Section  > Methods ---------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function RetrieveRealCustomerData() {
    RetrieveCustomerData($("#RCSearchCustCode").val(), 2, function (result, controls) {
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["RCSearchCustCode"], ["RCSearchCustCode"]);
            /* --------------------- */

            return;
        }
        if (result != undefined) {
            /* --- Set Data --- */
            /* ---------------- */
            ViewRealCustomerData(result);
            /* ---------------- */
        }
    });
}
function ViewRealCustomerData(doRealCustomer) {
    /* --- Disable Control --- */
    $("#RCSearchCustCode").attr("readonly", true);
    $("#btnRCRetrieve").attr("disabled", true);
    $("#btnRCSearchCustomer").attr("disabled", true);

    $("#divRealCustomerInfo").clearForm();

    /* --- Fill Data to Control --- */
    $("#divRealCustomerInfo").bindJSON_Prefix("RC", doRealCustomer);
    $("#RCSearchCustCode").val(doRealCustomer.CustCodeShort);
    $("#SiteCustCode").val(doRealCustomer.SiteCustCodeShort);

    if ($("#RCCustCodeShort").val() != "")
        $("#btnRCNewEditCustomer").attr("disabled", true);
}
/* ----------------------------------------------------------------------------------- */

/* ----------------------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */




/* ----------------------------------------------------------------------------------- */
/* --- Site Information Section ------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */

/* --- Site Information Section > Initial -------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function InitialSiteCustomerSection() {
    $("#divSiteCustomerInfo").ResetToNormalControl();

    /* --- Enable Control --- */
    $("#SiteCustCodeNo").removeAttr("readonly");
    $("#btnRetrieveSiteData").removeAttr("disabled");
    $("#btnSearchSite").removeAttr("disabled");
    $("#btnCopy").removeAttr("disabled");
    $("#rdoContractTarget").removeAttr("disabled");
    $("#rdoRealCustomer").removeAttr("disabled");
    $("#btnNewEditSite").removeAttr("disabled");
    $("#btnClearSite").removeAttr("disabled");

    /* --- Clear Data from Control --- */
    $("#divSiteCustomerInfo").clearForm();

    /* --- Init Control --- */
    $("#rdoRealCustomer").attr("checked", true);
}
function SetSiteCustomerSectionMode(isview) {
    if (isview) {
        $("#divSiteCustomerInfo").SetViewMode(true, true);
        $("#divSpecifySite_CopyNameAddress").hide();
    }
    else {
        $("#divSiteCustomerInfo").SetViewMode(false);
        $("#divSpecifySite_CopyNameAddress").show();

        if ($("#btnCopy").prop("disabled") == "disabled"
            || $("#btnCopy").prop("disabled") == true) {
            $("#rdoContractTarget").attr("disabled", true);
            $("#rdoRealCustomer").attr("disabled", true);
        }
    }
}
function SiteCustomerSection() {
    $("#btnRetrieveSiteData").click(retrieve_site_customer_click);
    $("#btnSearchSite").click(search_site_customer_click);
    $("#btnNewEditSite").click(new_edit_site_click);
    $("#btnClearSite").click(clear_site_click);
    $("#btnCopy").click(copy_site_information_click);
}
/* ----------------------------------------------------------------------------------- */

/* --- Site Information Section > Events --------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function retrieve_site_customer_click() {
    RetrieveSiteInformationData();
}
function search_site_customer_click() {
    /* --- Set Parameter --- */


    /* --- Merge --- */
    /* var obj = {
    CustCode: $("#RCSearchCustCode").val(),
    CustType: 2
    }; */
    var obj = {
        CustCode: $("#SiteCustCode").val(),
        CustType: 3
    };
    /* ------------- */

    ajax_method.CallScreenController("/Quotation/QUS020_CheckRealCustomer", obj,
        function (result, controls) {
            if (controls != undefined) {

                /* --- Merge --- */
                /* VaridateCtrl(["RCSearchCustCode"], ["RCSearchCustCode"]); */
                /* ------------- */

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
    var siteCustCode = $("#SiteCustCode").val();

    InitialSiteCustomerSection();
    ClearRegisterData(3);

    $("#SiteCustCode").val(siteCustCode);
}
function copy_site_information_click() {
    /* --- Set Parameter --- */
    /* --------------------- */
    var custType = 1;
    if ($("#rdoRealCustomer").prop("checked") == true) {
        custType = 2;
    }

    var obj = {
        CustType: custType,
        BranchNameEN: $("#BranchNameEN").val(),
        BranchNameLC: $("#BranchNameLC").val()
    };
    /* --------------------- */

    ajax_method.CallScreenController("/Quotation/QUS020_CopySiteInfomation", obj, function (result) {
        if (result != undefined) {
            /* --- Set Data --- */
            /* ---------------- */
            ViewSiteCustomerData(result, true);
            /* ---------------- */
        }
    });
}
/* ----------------------------------------------------------------------------------- */

/* --- Site Information Section > Events --------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function RetrieveSiteInformationData() {
    /* --- Set Parameter --- */
    var obj = {
        CustCode: $("#RCCustCodeShort").val(),
        SiteCustCode: $("#SiteCustCode").val(),
        SiteNo: $("#SiteCustCodeNo").val()
    };

    master_event.LockWindow(true);
    ajax_method.CallScreenController("/Quotation/QUS020_RetrieveSiteData", obj, function (result, controls) {
        master_event.LockWindow(false);

        if (controls != undefined) {
            
            /* --- Merge --- */
            /* VaridateCtrl(["RCSearchCustCode", "SiteCustCodeNo"], controls); */
            VaridateCtrl(["SiteCustCodeNo"], controls);
            /* ------------- */

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
        $("#btnNewEditSite").attr("disabled", true);
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
/* ----------------------------------------------------------------------------------- */

/* ----------------------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */





/* --- More Information Section ------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */

/* --- More Information Section > Initial -------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function InitialMoreInformationSection() {
    $("#divMoreInformation").ResetToNormalControl();

    /* --- Disable Control --- */
    $("#OldContractCode").attr("readonly", true);
    $("#IntroducerCode").attr("readonly", true);

    /* --- Clear Data from Control --- */
    $("#divMoreInformation").clearForm();
    $("#ProductTypeCode option:first").attr("selected", true);
    $("#QuotationOfficeCode option:first").attr("selected", true);
    $("#OperationOfficeCode option:first").attr("selected", true);
    $("#AcquisitionTypeCode option:first").attr("selected", true);
    $("#MotivationTypeCode option:first").attr("selected", true);
}
function SetMoreInformationSectionMode(isview) {
    if (isview) {
        $("#divMoreInformation").SetViewMode(true, true);
        if ($("#QuotationStaffEmpNo").val() == "")
            $("#divQuotationStaffName").html("");
    }
    else {
        $("#divMoreInformation").SetViewMode(false);
    }
}
function MoreInfomationSection() {
    $("#ProductTypeCode").RelateControlEvent(product_type_code_change);
    $("#AcquisitionTypeCode").RelateControlEvent(acquisition_type_code_change);
    $("#QuotationStaffEmpNo").blur(quotation_staff_blur);
}
/* ----------------------------------------------------------------------------------- */

/* --- More Information Section > Events --------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function product_type_code_change() {
    var v = $("#ProductTypeCode").val();

    /* --- Merge --- */
    /* if (v == ""
    || v == $("#C_PROD_TYPE_SALE").val()) {
    $("#OldContractCode").attr("readonly", true);
    } */
    if (v == undefined
        || v == ""
        || v == $("#C_PROD_TYPE_SALE").val()) {
    /* ------------- */

        $("#OldContractCode").attr("readonly", true);
    }
    else {
        $("#OldContractCode").removeAttr("readonly");
    }

    $("#OldContractCode").val("");
    $("#OldContractCode").ResetToNormalControl();
}
function acquisition_type_code_change(istab) {
    var v = $("#AcquisitionTypeCode").val();
    if (v == $("#C_ACQUISITION_TYPE_CUST").val()
        || v == $("#C_ACQUISITION_TYPE_INSIDE_COMPANY").val()

        /* --- Merge --- */
        /* || v == $("#C_ACQUISITION_TYPE_SUBCONTRACTOR").val() */
        /* ------------- */

        ) {
        $("#IntroducerCode").removeAttr("readonly");

        if (istab)
            $("#IntroducerCode").focus();
    }
    else {
        $("#IntroducerCode").attr("readonly", true);

        if (istab)
            $("#MotivationTypeCode").focus();
    }

    $("#IntroducerCode").val("");
    $("#IntroducerCode").ResetToNormalControl();
}
function quotation_staff_blur() {
    GetQuotationStaffName();
}
/* ----------------------------------------------------------------------------------- */

/* --- More Information Section > Methods -------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function GetQuotationStaffName() {
    $("#QuotationStaffName").val("");
    if ($.trim($("#QuotationStaffEmpNo").val()) != "") {
        /* --- Set Parameter --- */
        var obj = {
            empNo: $("#QuotationStaffEmpNo").val()
        };

        ajax_method.CallScreenController("/Master/GetActiveEmployeeName", obj, function (data) {
            if (data != undefined) {
                $("#QuotationStaffName").val(data);
                $("#btnCommandRegister").focus();
            }
        });
    }
}
/* ----------------------------------------------------------------------------------- */

/* ----------------------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */





/* --- Command Button Events --------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function command_register_click() {
    /* --- Set Parameter --- */
    var obj = {
        ProductTypeCode: $("#ProductTypeCode").val(),
        QuotationOfficeCode: $("#QuotationOfficeCode").val(),
        OperationOfficeCode: $("#OperationOfficeCode").val(),
        AcquisitionTypeCode: $("#AcquisitionTypeCode").val(),
        IntroducerCode: $("#IntroducerCode").val(),
        MotivationTypeCode: $("#MotivationTypeCode").val(),
        OldContractCode: $("#OldContractCode").val(),
        QuotationStaffEmpNo: $("#QuotationStaffEmpNo").val(),
        isBranchChecked: $("#BranchContract").prop("checked"),
        BranchNameEN: $("#BranchNameEN").val(),
        BranchAddressEN: $("#BranchAddressEN").val(),
        BranchNameLC: $("#BranchNameLC").val(),
        BranchAddressLC: $("#BranchAddressLC").val(),
        ContractTargetMemo: $("#ContractTargetMemo").val(),
        RealCustomerMemo: $("#RealCustomerMemo").val()
    };

    ajax_method.CallScreenController("/Quotation/QUS020_RegisterQuotationData", obj, function (result, controls) {
        /* --- Higlight Text --- */
        /* --------------------- */
        VaridateCtrl(["SiteCustCodeNo",
                            "OldContractCode",
                            "CPSearchCustCode",
                            "RCSearchCustCode",
                            "QuotationStaffEmpNo",
                            "BranchNameEN",
                            "BranchAddressEN",
                            "BranchNameLC",
                            "BranchAddressLC",
                            "ProductTypeCode",
                            "QuotationOfficeCode",
                            "OperationOfficeCode",
                            "IntroducerCode",
                            "ContractTargetMemo",
                            "RealCustomerMemo",
                            "AcquisitionTypeCode",
                            "MotivationTypeCode"], controls);
        /* --------------------- */

        if (controls != undefined) {
            return;
        }
        if (result == true) {
            /* --- Set View Mode --- */
            /* --------------------- */
            SetContractCustomerSectionMode(true);
            SetRealCustomerSectionMode(true);
            SetSiteCustomerSectionMode(true);
            SetMoreInformationSectionMode(true);
            /* --------------------- */

            /* --- Set Command Button --- */
            /* -------------------------- */
            InitialCommandButton(1);
            /* -------------------------- */
        }
    });
}
function command_reset_click() {
    InitialSection();

    /* --- Merge --- */
    $("#CPSearchCustCode").focus();
    /* ------------- */
}
function command_confirm_click() {
    /* --- Set Command Button --- */
    ajax_method.CallScreenController("/Quotation/QUS020_ConfirmData", "", function (result, controls) {
        /* --- Higlight Text --- */
        VaridateCtrl(["SiteCustCodeNo", "OldContractCode", "CPSearchCustCode", "RCSearchCustCode", "QuotationStaffEmpNo"], controls);

        if (controls != undefined) {
            command_back_click();
            return;
        }
        if (result != null && result != "") {
            /* --- Get Message --- */
            /* ------------------- */
            var obj = {
                module: "Common",
                code: "MSG0046"
            };
            call_ajax_method_json("/Shared/GetMessage", obj, function (res) {
                OpenInformationMessageDialog(res.Code, res.Message, function () {
                    InitialCommandButton(2);

                    $("#divResultRegister").show();
                    $("#btnRegisterQuotationDetail").removeAttr("disabled");
                    $("#btnRegisterNextQuotationTarget").removeAttr("disabled");
                    $("#QuotationTargetCode").val(result);

                    $("#btnRegisterQuotationDetail").focus();
                    master_event.ScrollWindow("#divResultRegister");
                });
            });
            /* ------------------- */
        }
    });
}
function command_back_click() {
    /* --- Set View Mode --- */
    /* --------------------- */
    SetContractCustomerSectionMode(false);
    SetRealCustomerSectionMode(false);
    SetSiteCustomerSectionMode(false);
    SetMoreInformationSectionMode(false);
    /* --------------------- */

    /* --- Set Command Button --- */
    /* -------------------------- */
    InitialCommandButton(0);
    /* -------------------------- */
}
/* ----------------------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */



reset_command.SetCommand = function (func) {
    var ctrlC = $("#" + this.Control);

    ctrlC.unbind("click");
    if (func != undefined && typeof (func) == "function") {
        ctrlC.show();
        ctrlC.removeAttr("disabled");
        ctrlC.click(function () {
            command_control.CommandControlMode(false);

            var obj = {
                module: "Common",
                code: "MSG0038"
            };
            ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
                OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    ajax_method.CallScreenController("/Quotation/QUS020_ResetSessionData", "", function () {
                        func();
                    });
                },
                function () {
                    command_control.CommandControlMode(true);
                });
            });
        });
    }
    else {
        ctrlC.hide();
    }
}

