/// <reference path="../Base/Master.js" />

/* --- Inital Variables -------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
var ProductTypeMode;
var isApproveMode = false;
var isEditMode = false;
/* ----------------------------------------------------------------------------------- */

/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    InitialCommandButton(0);
});
function InitialCTS010Control() {
    /// <summary>Method to initial control</summary>
    if (typeof (InitQuotationTargetInfo) == "function")
        InitQuotationTargetInfo();

    InitialCommandButton(0);
}
/* ----------------------------------------------------------------------------------- */

/* --- Command Methods --------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function InitialCommandButton(step) {
    if (step == 0) {
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        approve_command.SetCommand(null);
        reject_command.SetCommand(null);
        return_command.SetCommand(null);
        close_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }
    else if (step == 1) {
        register_command.SetCommand(command_register_click);
        reset_command.SetCommand(command_reset_click);
        approve_command.SetCommand(null);
        reject_command.SetCommand(null);
        return_command.SetCommand(null);
        close_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }
    else if (step == 2) {
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        approve_command.SetCommand(null);
        reject_command.SetCommand(null);
        return_command.SetCommand(null);
        close_command.SetCommand(null);
        confirm_command.SetCommand(command_confirm_click);
        back_command.SetCommand(command_back_click);
    }
    else if (step == 3) {
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        approve_command.SetCommand(command_approve_click);
        reject_command.SetCommand(command_reject_click);
        return_command.SetCommand(command_return_click);
        close_command.SetCommand(command_close_click);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }
}

function command_register_click() {
    RegisterRentalData()
}
function command_reset_click() {
    InitialCTS010Control();
    ResetSection();
}
function command_confirm_click() {
    /// <summary>Event when click Confirm command</summary>

    /* --- Call Confirm method --- */
    /* --------------------------- */
    var cmd_method = function () {
        ajax_method.CallScreenController("/Contract/CTS010_ConfirmRentalContractData_P1", "", function (result) {
            var confirm_function = function (res) {
                if (typeof (res) == "string") {
                    window.location = ajax_method.GenerateURL(res);
                }
                else {
                    OpenInformationMessageDialog(res[0].Code, res[0].Message, function () {
                        if (typeof (res[1]) == "string") {
                            window.location = ajax_method.GenerateURL(res[1]);
                        }
                        else {
                            $("#divCPCustCodeShort").html(res[1].ContractTargetCustCode);
                            $("#divCPCustStatusCodeName").html(res[1].ContractTargetStatusCodeName);
                            $("#divCPIDNo").html(res[1].ContractTargetIDNo);
                            $("#divRCCustCodeShort").html(res[1].RealCustomerCustCode);
                            $("#divRCCustStatusCodeName").html(res[1].RealCustomerStatusCodeName);
                            $("#divRCIDNo").html(res[1].RealCustomerIDNo);
                            $("#divSiteCodeShort").html(res[1].SiteCode);

                            $("#gridBillingTarget").LoadDataToGrid(gridBillingTarget, 0, false,
                                "/Contract/CTS010_GetBillingTargetList",
                                "",
                                "CTS010_BillingTargetData", false,
                                function () {
                                    $("#gridBillingTarget").find("img").each(function () {
                                        if (this.id != "" && this.id != undefined) {
                                            if (this.id.indexOf(btnBillingTargetDetail) == 0) {
                                                var row_id = GetGridRowIDFromControl(this);
                                                EnableGridButton(gridBillingTarget, btnBillingTargetDetail, row_id, "Detail", false);
                                            }
                                            else if (this.id.indexOf(btnRemoveBillingTargetDetail) == 0) {
                                                var row_id = GetGridRowIDFromControl(this);
                                                EnableGridButton(gridBillingTarget, btnRemoveBillingTargetDetail, row_id, "Remove", false);
                                            }
                                        }
                                    });
                                });


                            CallMultiLoadScreen("Contract", [["CTS010_18"]], function () {
                                InitialCommandButton(0);

                                if ($("#btnRegisterNextNewRentalContract").length > 0)
                                    $("#btnRegisterNextNewRentalContract").focus();
                                else if ($("#btnEditNextNewrentalContract").length > 0)
                                    $("#btnEditNextNewrentalContract").focus();
                            });
                        }
                    });
                }
            };

            if (result == 1) {
                ajax_method.CallScreenController("/Contract/CTS010_ConfirmRentalContractData_P2", "", function (result2, controls) {
                    if (result2 == 2) {
                        ajax_method.CallScreenController("/Contract/CTS010_ConfirmRentalContractData_P3", "", function (result3, controls) {
                            if (result3 != undefined) {
                                confirm_function(result3);
                            }
                        });
                    }
                    else if (result2 != undefined) {
                        ChangeSectionMode(true);
                        confirm_function(result2);
                    }
                });
            }
            else if (result == 2) {
                ajax_method.CallScreenController("/Contract/CTS010_ConfirmRentalContractData_P3", "", function (result3, controls) {
                    if (result3 != undefined) {
                        confirm_function(result3);
                    }
                });
            }
            else if (result != undefined) {
                confirm_function(result);
            }
        });
    }

    if (isApproveMode == true) {
        var obj = {
            module: "Contract",
            code: "MSG3250"
        };
        ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
            command_control.CommandControlMode(false);
            OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    cmd_method();
                },
                function () {
                    command_control.CommandControlMode(true);
                });
        });
    }
    else {
        cmd_method();
    }


    /* --------------------------- */
}
function command_back_click() {
    /// <summary>Event when click Back command</summary>

    ChangeSectionMode(false);

    if (isApproveMode)
        InitialCommandButton(3);
    else
        InitialCommandButton(1);
}
function command_approve_click() {
    RegisterRentalData();
}
function command_reject_click() {

    var reject_function = function (res) {
        if (typeof (res) == "string") {
            window.location = ajax_method.GenerateURL(res);
        }
        else if (res != undefined) {
            OpenInformationMessageDialog(res[0].Code, res[0].Message, function () {
                if (typeof (res[1]) == "string") {
                    window.location = ajax_method.GenerateURL(res[1]);
                }
            });
        }
    };

    ajax_method.CallScreenController("/Contract/CTS010_RejectRentalContractData_P1", "", function (result) {
        if (result != undefined) {
            command_control.CommandControlMode(false);
            OpenYesNoMessageDialog(result.Code, result.Message,
            function () {
                ajax_method.CallScreenController("/Contract/CTS010_RejectRentalContractData_P2", "", function (result) {
                    reject_function(result);
                });
            },
                function () {
                    command_control.CommandControlMode(true);
                });
        }
        else {
            reject_function(result);
        }
    });
}
function command_return_click() {
    ajax_method.CallScreenController("/Contract/CTS010_ReturnRentalContractData_P1", "", function (result) {
        if (result != undefined) {
            command_control.CommandControlMode(false);
            OpenYesNoMessageDialog(result.Code, result.Message,
            function () {
                ajax_method.CallScreenController("/Contract/CTS010_ReturnRentalContractData_P2", "", function (result) {
                    if (result != undefined) {
                        window.location = ajax_method.GenerateURL(result);
                    }
                });
            },
                function () {
                    command_control.CommandControlMode(true);
                });
        }
    });
}
function command_close_click() {
    ajax_method.CallScreenController("/Contract/CTS010_CloseRentalContract", "", function (result) {
        if (result != undefined) {
            window.location = ajax_method.GenerateURL(result);
        }
    });
}
/* ----------------------------------------------------------------------------------- */


function InitModeSection(func) {
    /// <summary>Method to Show AL section</summary>

    var step1 = ["CTS010_07",
                 "CTS010_16"];

    var step2 = ["CTS010_02"];
    if (ProductTypeMode.IsProductTypeAL
        || ProductTypeMode.IsProductTypeRentalSale) {
        step2.push("CTS010_03");
        step2.push("CTS010_03_F");
        step2.push("CTS010_04");
    }
    else if (ProductTypeMode.IsProductSaleOnline) {
        step2.push("CTS010_03_F");
        step2.push("CTS010_04");
    }
    else if (ProductTypeMode.IsProductMaintenance) {
        step2.push("CTS010_05");
    }

    var step3 = ["CTS010_06",
                 "CTS010_08",
                 "CTS010_10"];
    if (ProductTypeMode.IsProductTypeAL == true
        || ProductTypeMode.IsProductTypeRentalSale == true
        || ProductTypeMode.IsProductSaleOnline == true) {
        step3.push("CTS010_09");
    }

    var step4 = ["CTS010_11",
                 "CTS010_12"];
    var step5 = ["CTS010_13",
                 "CTS010_14",
                 "CTS010_15"];
    CallMultiLoadScreen("Contract", [step5, step4, step3, step2, step1], func);
}
function ResetSection() {
    var screen = ["CTS010_02",
                  "CTS010_03",
                  "CTS010_03_F",
                  "CTS010_04",
                  "CTS010_05",
                  "CTS010_06",
                  "CTS010_07",
                  "CTS010_08",
                  "CTS010_09",
                  "CTS010_10",
                  "CTS010_11",
                  "CTS010_12",
                  "CTS010_13",
                  "CTS010_14",
                  "CTS010_15",
                  "CTS010_16",
                  "CTS010_18"];

    ResetAllScreen(screen);
}

function RegisterRentalData() {
//    if (typeof (cancel_billing_target_detail_click) == "function")
//        cancel_billing_target_detail_click();

    /// <summary>Method to Register AL section</summary>
    if (typeof (GetCTS010_01_SectionData) == "function"
        && typeof (GetCTS010_02_SectionData) == "function"
        && typeof (GetCTS010_06_SectionData) == "function"
        && typeof (GetCTS010_07_SectionData) == "function"
        && typeof (GetCTS010_08_SectionData) == "function"
        && typeof (GetCTS010_12_SectionData) == "function"
        && typeof (GetCTS010_15_SectionData) == "function"
        && typeof (GetCTS010_16_SectionData) == "function") {

        /* --- Generate Object --- */
        /* ----------------------- */
        var contractData = GetCTS010_02_SectionData();

        var spdata = GetCTS010_01_SectionData();
        contractData.QuotationTargetCode = spdata.QuotationTargetCode;
        contractData.Alphabet = spdata.Alphabet;

        if (typeof (GetCTS010_04_SectionData) == "function") {
            var mi = GetCTS010_04_SectionData();
            contractData.MaintenanceCycle = mi.MaintenanceCycle;
        }

        var maintenance;
        if (typeof (GetCTS010_05_SectionData) == "function") {
            maintenance = GetCTS010_05_SectionData();
        }

        var cdi = GetCTS010_06_SectionData();
        contractData.IrregulationContractDurationFlag = cdi.IrregulationContractDurationFlag;
        contractData.ContractDurationMonth = cdi.ContractDurationMonth;
        contractData.AutoRenewMonth = cdi.AutoRenewMonth;
        contractData.ContractEndDate = cdi.ContractEndDate;

        var pt = GetCTS010_07_SectionData();
        contractData.BillingCycle = pt.BillingCycle;
        contractData.PayMethod = pt.PayMethod;
        contractData.CreditTerm = pt.CreditTerm;
        contractData.CalDailyFeeStatus = pt.CalDailyFeeStatus;

        var ic = GetCTS010_08_SectionData();
        contractData.ContractOfficeCode = ic.ContractOfficeCode;
        contractData.OperationOfficeCode = ic.OperationOfficeCode;
        contractData.SalesmanEmpNo1 = ic.SalesmanEmpNo1;
        contractData.SalesmanEmpNo2 = ic.SalesmanEmpNo2;
        contractData.SalesSupporterEmpNo = ic.SalesSupporterEmpNo;

        var ct = GetCTS010_12_SectionData();
        contractData.ContractTargetSignerTypeCode = ct.ContractTargetSignerTypeCode;
        contractData.IsBranchChecked = ct.IsBranchChecked;
        contractData.BranchNameEN = ct.BranchNameEN;
        contractData.BranchNameLC = ct.BranchNameLC;
        contractData.BranchAddressEN = ct.BranchAddressEN;
        contractData.BranchAddressLC = ct.BranchAddressLC;

        var cp = GetCTS010_15_SectionData();
        contractData.ContactPoint = cp.ContactPoint;

        var b = GetCTS010_16_SectionData();
        contractData.DivideContractFeeBillingFlag = b.DivideContractFeeBillingFlag;
        contractData.IsBillingEditMode = b.IsBillingEditMode;

        var obj = {
            contract: contractData,
            maintenance: maintenance
        };
        /* ----------------------- */

        /* --- Call register methods --- */
        /* ----------------------------- */
        ajax_method.CallScreenController("/Contract/CTS010_RegisterRentalContractData", obj, function (result, controls) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl([
                "ExpectedInstallCompleteDate",
                "ExpectedStartServiceDate",
                "CounterBalanceOriginContractCode",
                "BillingTimingDepositFee",
                "ApproveNo1",
                "ApproveNo2",
                "ApproveNo3",
                "ApproveNo4",
                "OrderContractFee",
                "OrderInstallFee",
                "OrderInstallFee_ApproveContract",
                "OrderInstallFee_CompleteInstall",
                "OrderInstallFee_StartService",
                "OrderDepositFee",
                "ContractOfficeCode",
                "SalesmanEmpNo1",
                "SalesmanEmpNo2",
                "SalesSupporterEmpNo",
                "BillingCycle",
                "PayMethod",
                "ContractDurationMonth",
                "AutoRenewMonth",
                "ContractEndDate",
                "CreditTerm",
                "CalDailyFeeStatus",
                //"BICContractCode", //Comment by Jutarat A. on 12102012
                "CPContractTargetSignerTypeCode",
                "BranchNameEN",
                "BranchAddressEN",
                "BranchNameLC",
                "BranchAddressLC"], controls);
            /* --------------------- */
            if (controls != undefined) {
                for (var i = 0; i < controls.length; i++) {
                    if (controls[i] == "Email" && CheckFirstRowIsEmpty(gridEmailList) == true) {
                        var row_id = gridEmailList.getRowId(0);
                        gridEmailList.setRowTextStyle(row_id, "background-color:#FFE2E2;");
                    }
                }
                return;
            }
            else if (result == true) {
                ChangeSectionMode(true);
                InitialCommandButton(2);
            }
        });
        /* ----------------------------- */
    }
}
function ChangeSectionMode(view_mode) {
    /// <summary>Method to change to View/Edit mode for AL section</summary>
    /// <param name="view_mode" type="bool">Flag to set View mode</param>

    var screens = ["CTS010_01",
                    "CTS010_02",
                    "CTS010_04",
                    "CTS010_05",
                    "CTS010_06",
                    "CTS010_07",
                    "CTS010_08",
                    "CTS010_09",
                    "CTS010_10",
                    "CTS010_11",
                    "CTS010_12",
                    "CTS010_13",
                    "CTS010_14",
                    "CTS010_15",
                    "CTS010_16"];

    ChangeAllSectionMode(screens, view_mode);
}
function EnableSection(enable) {
    var screens = ["CTS010_01",
                    "CTS010_02",
                    "CTS010_04",
                    "CTS010_05",
                    "CTS010_06",
                    "CTS010_07",
                    "CTS010_08",
                    "CTS010_09",
                    "CTS010_10",
                    "CTS010_11",
                    "CTS010_12",
                    "CTS010_13",
                    "CTS010_14",
                    "CTS010_15",
                    "CTS010_16"];

    EnableAllScreenSection(screens, enable);

    if (enable)
        DisableRegisterCommand(false);
    else
        DisableRegisterCommand(true);
}



/* --- Methods ----------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function SetInitialData(object, objectTypeID, func) {
    var obj = {
        initData: object,
        ObjectTypeID: objectTypeID
    };
    ajax_method.CallScreenController("/Contract/CTS010_SetInitialData", obj, function (result) {
        if (result[0] == true) {
            if (typeof (func) == "function")
                func(result[1]);
        }
    });
}
function GetInitialData(objectTypeID, func) {
    var obj = {
        ObjectTypeID: objectTypeID
    };
    ajax_method.CallScreenController("/Contract/CTS010_GetInitialData", obj, function (result) {
        if (typeof (func) == "function")
            func(result);
    });
}
function ClearInitialData(objectTypeID) {
    SetInitialData(null, objectTypeID);
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
    ajax_method.CallScreenController("/Contract/CTS010_RetrieveCustomer", obj, function (result, controls) {
        if (func != null) {
            func(result, controls);
        }
    });
    /* ------------------ */
}
/* ----------------------------------------------------------------------------------- */

/* --- Dialog Methods (CMS250) ------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
var searchFunction = null;
function SearchCustomerData(func) {
    searchFunction = func;
    $("#dlgBox").OpenCMS250Dialog("CTS010");
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
    $("#dlgBox").OpenCMS260Dialog("CTS010");
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

    GetInitialData(flag, function (result) {
        doCustomer = result;
        $("#dlgBox").OpenMAS050Dialog("CTS010");
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
            doContractCustomer: doCustomer
        };
        SetInitialData(obj, 1, function (result) {
            if (result == true) {
                InitialRealCustomerSection();
                DeleteAllRow(gridRealCustomerGroup);

                $("#SameCustomer").val("True");
                $("#btnRCNewEditCustomer").attr("disabled", true);
            }

            ViewContractCustomerData(doCustomer);
            if ($("#SameCustomer").val() == "True") {
                var obj2 = {
                    doRealCustomer: doCustomer
                };
                SetInitialData(obj2, 2, function () {
                    ViewRealCustomerData(doCustomer);

                    var isChanged = true;
                    if (doCustomer != undefined) {
                        if ($("#SiteCustCode").val() == doCustomer.SiteCustCodeShort) {
                            isChanged = false;
                        }
                        else {
                            $("#SiteCustCode").val(doCustomer.SiteCustCodeShort);
                        }
                    }
                    if (isChanged && $("#SiteCodeShort").val() != "") {
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
                    }
                });
            }
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
            doRealCustomer: doCustomer
        };
        SetInitialData(obj, 2, function (result) {
            if (result == true) {
                $("#SameCustomer").val("True");
                $("#btnRCNewEditCustomer").attr("disabled", true);
            }

            ViewRealCustomerData(doCustomer);

            var isChanged = true;
            if (doCustomer != undefined) {
                if ($("#SiteCustCode").val() == doCustomer.SiteCustCodeShort) {
                    isChanged = false;
                }
                else {
                    $("#SiteCustCode").val(doCustomer.SiteCustCodeShort);
                }
            }
            if (isChanged && $("#SiteCodeShort").val() != "") {
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
            }
        });
        /* ---------------- */
    }
}
/* ----------------------------------------------------------------------------------- */

/* --- Dialog Methods (MAS040) ------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
var doSite = null;
function NewEdit_SiteData() {
    GetInitialData(3, function (result) {
        doSite = result;
        $("#dlgBox").OpenMAS040Dialog("CTS010");
    });
}
function MAS040Object() {
    return doSite;
}
function MAS040Response(doSite) {
    $("#dlgBox").CloseDialog();

    /* --- Reset Control --- */
    /* --------------------- */
    $("#SiteCustCodeNo").val("");
    /* --------------------- */

    /* ---------------- */
    var obj = {
        dosite: doSite
    };
    SetInitialData(obj, 3, function () {
        doSite.SiteCodeShort = doSite.SiteCode;
        ViewSiteCustomerData(doSite, false);
    });
    /* ---------------- */
}
/* ----------------------------------------------------------------------------------- */

//function GetEmployeeNameData(src, dest) {
//    if (src != undefined && dest != undefined) {
//        $(src).blur(function () {
//            if (dest != undefined) {
//                $(dest).val("");
//                if ($.trim($(src).val()) != "") {
//                    /* --- Set Parameter --- */
//                    var obj = {
//                        empNo: $(src).val()
//                    };

//                    ajax_method.CallScreenController("/Master/GetActiveEmployeeName", obj, function (result) {
//                        if (result != undefined) {
//                            $(dest).val(result);
//                        }
//                    });
//                }
//            }
//        });
//    }
//}