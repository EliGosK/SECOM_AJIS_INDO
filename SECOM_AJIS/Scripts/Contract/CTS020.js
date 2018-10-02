/* --- Inital Variables -------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
var isApproveMode = false;
var isEditMode = false;
var isChangeAlphabetMode = false;
var processType = 1;
/* ----------------------------------------------------------------------------------- */

/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    if ($("#ApproveQuotationTargetCode").val() != "") {
        isApproveMode = true;
    }
    else if ($("#ScreenMode").val() == 1)
        isEditMode = true;

    InitialCommandButton(0);
});
function InitialCTS020Control() {
    /// <summary>Method to initial control</summary>
    if (typeof (InitSpecifyProcessType) == "function")
        InitSpecifyProcessType();
    if (typeof (ShowSpecifyQuotationSection) == "function") {
        ShowSpecifyQuotationSection(false);

        if (typeof (SetCTS020_01_EnableSection) == "function") {
            SetCTS020_01_EnableSection(true);

            if (isApproveMode == true) {
                $("#QuotationTargetCode").attr("readonly", true);
            }
            else {
                $("#QuotationTargetCode").val("");
                $("#ProductName").val("");
            }

            $("#rdoProcessTypeNew").attr("checked", true);
            $("#ContractCode").val("");
            $("#Alphabet").val("");
            $("#Alphabet").removeAttr("readonly");

            $("#QuotationTargetCode").ResetToNormalControl();
            $("#Alphabet").ResetToNormalControl();
        }
    }

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
    else if (step == 4) {
        if (isEditMode == false) {
            register_command.SetCommand(null);
            reset_command.SetCommand(command_reset_click);
            approve_command.SetCommand(null);
            reject_command.SetCommand(null);
            return_command.SetCommand(null);
            close_command.SetCommand(null);
            confirm_command.SetCommand(null);
            back_command.SetCommand(null);
        }
    }
}

function command_register_click() {
    RegisterSaleData();
}
function command_reset_click() {
    InitialCTS020Control();
    ResetSection();
}
function command_confirm_click() {
    /// <summary>Event when click Confirm command</summary>

    /* --- Call Confirm method --- */
    /* --------------------------- */
    var cmd_method = function () {
        ajax_method.CallScreenController("/Contract/CTS020_ConfirmSaleContractData_P1", "", function (result) {
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
                            $("#divCPCustCodeShort").html(res[1].PurchaserCustCode);
                            $("#divCPCustStatusCodeName").html(res[1].PurchaserStatusCodeName);
                            $("#divCPIDNo").html(res[1].PurchaserIDNo);
                            $("#divRCCustCodeShort").html(res[1].RealCustomerCustCode);
                            $("#divRCCustStatusCodeName").html(res[1].RealCustomerStatusCodeName);
                            $("#divRCIDNo").html(res[1].RealCustomerIDNo);
                            $("#divSiteCodeShort").html(res[1].SiteCode);

                            if (res[1].BillingTargetCode != null && res[1].BillingTargetCode != "")
                                $("#divBillingTargetCodeShort").html(res[1].BillingTargetCode);
                            if (res[1].BillingClientCode != null && res[1].BillingClientCode != "")
                                $("#divBillingClientCodeShort").html(res[1].BillingClientCode);

                            CallMultiLoadScreen("Contract", [["CTS020_11"]], function () {
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
                ajax_method.CallScreenController("/Contract/CTS020_ConfirmSaleContractData_P2", "", function (result2, controls) {
                    if (result2 == 2) {
                        ajax_method.CallScreenController("/Contract/CTS020_ConfirmSaleContractData_P3", "", function (result3, controls) {
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
                ajax_method.CallScreenController("/Contract/CTS020_ConfirmSaleContractData_P3", "", function (result3, controls) {
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
    RegisterSaleData();
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

    ajax_method.CallScreenController("/Contract/CTS020_RejectSaleContractData_P1", "", function (result) {
        if (result != undefined) {
            command_control.CommandControlMode(false);
            OpenYesNoMessageDialog(result.Code, result.Message,
            function () {
                ajax_method.CallScreenController("/Contract/CTS020_RejectSaleContractData_P2", "", function (result) {
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
    ajax_method.CallScreenController("/Contract/CTS020_ReturnSaleContractData_P1", "", function (result) {
        if (result != undefined) {
            command_control.CommandControlMode(false);
            OpenYesNoMessageDialog(result.Code, result.Message,
            function () {
                ajax_method.CallScreenController("/Contract/CTS020_ReturnSaleContractData_P2", "", function (result) {
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
    ajax_method.CallScreenController("/Contract/CTS020_CloseSaleContract", "", function (result) {
        if (result != undefined) {
            window.location = ajax_method.GenerateURL(result);
        }
    });
}


function RegisterSaleData() {
    /// <summary>Method to Register AL section</summary>
    if (typeof (GetCTS020_02_SectionData) == "function"
        && typeof (GetCTS020_03_SectionData) == "function"
        && typeof (GetCTS020_06_SectionData) == "function"
        && typeof (GetCTS020_09_SectionData) == "function"
        && typeof (GetCTS020_10_SectionData) == "function") {

        /* --- Generate Object --- */
        /* ----------------------- */
        var contractData = GetCTS020_03_SectionData();

        var spdata = GetCTS020_02_SectionData();
        contractData.QuotationTargetCode = spdata.QuotationTargetCode;
        contractData.Alphabet = spdata.Alphabet;

        var ct = GetCTS020_06_SectionData();
        contractData.PurchaserSignerTypeCode = ct.PurchaserSignerTypeCode;
        contractData.IsBranchChecked = ct.IsBranchChecked;
        contractData.BranchNameEN = ct.BranchNameEN;
        contractData.BranchNameLC = ct.BranchNameLC;
        contractData.BranchAddressEN = ct.BranchAddressEN;
        contractData.BranchAddressLC = ct.BranchAddressLC;

        var cp = GetCTS020_09_SectionData();
        contractData.ContactPoint = cp.ContactPoint;

        var billing = GetCTS020_10_SectionData();

        var obj = {
            contract: contractData,
            billingData: billing
        };
        /* ----------------------- */

        /* --- Call register methods --- */
        /* ----------------------------- */
        ajax_method.CallScreenController("/Contract/CTS020_RegisterSaleContractData", obj, function (result, controls) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl([
                "ExpectedInstallCompleteDate",
                "ExpectedAcceptanceAgreeDate",
                "SaleType",
                "OrderProductPrice", 
                "OrderInstallFee", 
                "OrderSalePrice",
                "ProjectCode",
                "ConnectTargetCode",
                "DistributedOriginCode",
                "ContractOfficeCode",
                "OperationOfficeCode",
                "SalesmanEmpNo1",
                "SalesmanEmpNo2",
                "SalesmanEmpNo3",
                "SalesmanEmpNo4",
                "SalesmanEmpNo5",
                "SalesmanEmpNo6",
                "SalesmanEmpNo7",
                "SalesmanEmpNo8",
                "SalesmanEmpNo9",
                "SalesmanEmpNo10",
                "ApproveNo1",
                "ApproveNo2",
                "ApproveNo3",
                "ApproveNo4",
                "BillingOfficeCode",
                //"BICContractCode", //Comment by Jutarat A. on 16102012
                "BranchNameEN",
                "BranchAddressEN",
                "BranchNameLC",
                "BranchAddressLC",
                "CPPurchaserSignerTypeCode",
                "PaymentMethod_Approval",
                "PaymentMethod_Partial",
                "PaymentMethod_Acceptance",
                "SalePrice_PaymentMethod_Approval",
                "SalePrice_PaymentMethod_Partial",
                "SalePrice_PaymentMethod_Acceptance",
                "InstallationFee_PaymentMethod_Approval",
                "InstallationFee_PaymentMethod_Partial",
                "InstallationFee_PaymentMethod_Acceptance"], controls);
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

    var screens = ["CTS020_01",
                    "CTS020_02",
                    "CTS020_03",
                    "CTS020_05",
                    "CTS020_06",
                    "CTS020_07",
                    "CTS020_08",
                    "CTS020_09",
                    "CTS020_10"];

    ChangeAllSectionMode(screens, view_mode);
}
function EnableSection(enable) {
    var screens = ["CTS020_02",
                    "CTS020_03",
                    "CTS020_05",
                    "CTS020_06",
                    "CTS020_07",
                    "CTS020_08",
                    "CTS020_09",
                    "CTS020_10"];

    EnableAllScreenSection(screens, enable);

    if (enable)
        DisableRegisterCommand(false);
    else
        DisableRegisterCommand(true);
}
/* ----------------------------------------------------------------------------------- */

function InitModeSection(func) {
    var step1 = ["CTS020_03",
                 "CTS020_04",
                 "CTS020_05"];
    var step2 = ["CTS020_06",
                 "CTS020_07"];
    var step3 = ["CTS020_08",
                 "CTS020_09",
                 "CTS020_10"];
    CallMultiLoadScreen("Contract", [step3, step2, step1], func);
}

function ResetSection() {
    var screen = ["CTS020_03",
                  "CTS020_04",
                  "CTS020_05",
                  "CTS020_06",
                  "CTS020_07",
                  "CTS020_08",
                  "CTS020_09",
                  "CTS020_10",
                  "CTS020_11"];

    ResetAllScreen(screen);

    InitSpecifyProcessType();
    InitSpecifyQuotation();
}





function SetInitialData(object, objectTypeID, func) {
    var obj = {
        initData: object,
        ObjectTypeID: objectTypeID
    };
    ajax_method.CallScreenController("/Contract/CTS020_SetInitialData", obj, function (result) {
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
    ajax_method.CallScreenController("/Contract/CTS020_GetInitialData", obj, function (result) {
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
    ajax_method.CallScreenController("/Contract/CTS020_RetrieveCustomer", obj, function (result, controls) {
        if (func != null) {
            func(result, controls);
        }
    });
    /* ------------------ */
}

/* --- Dialog Methods (CMS250) ------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
var searchFunction = null;
function SearchCustomerData(func) {
    searchFunction = func;
    $("#dlgBox").OpenCMS250Dialog("CTS020");
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
    $("#dlgBox").OpenCMS260Dialog("CTS020");
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
        $("#dlgBox").OpenMAS050Dialog("CTS020");
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
            doPurchaserCustomer: doCustomer
        };
        SetInitialData(obj, 1, function (result) {
            if (result == true) {
                InitialRealCustomerSection();
                DeleteAllRow(gridRealCustomerGroup);

                $("#SameCustomer").val("True");
                $("#btnRCNewEditCustomer").attr("disabled", true);
            }

            ViewPurchaserCustomerData(doCustomer);
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
        $("#dlgBox").OpenMAS040Dialog("CTS020");
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