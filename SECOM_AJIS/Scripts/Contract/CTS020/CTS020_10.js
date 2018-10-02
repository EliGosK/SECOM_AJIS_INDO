/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "BillingTargetCode",
        "BillingClientCode"
    ]);

    InitialBillingSection();
    InitialBillingEvents();
    
    var useinstallfee = $("#formBillingTargetDetail").data("useinstallfee");
    if (processType == 1) {
        if (useinstallfee == "0") {
            $("#InstallationFee_Approval").attr("readonly", true);
            $("#InstallationFee_Approval").NumericCurrency().attr("disabled", true);
            $("#InstallationFee_Approval").val("0.00");
            $("#InstallationFee_PaymentMethod_Approval").attr("disabled", true);

            $("#InstallationFee_Partial").attr("readonly", true);
            $("#InstallationFee_Partial").NumericCurrency().attr("disabled", true);
            $("#InstallationFee_Partial").val("0.00");
            $("#InstallationFee_PaymentMethod_Partial").attr("disabled", true);

            $("#InstallationFee_Acceptance").attr("readonly", true);
            $("#InstallationFee_Acceptance").NumericCurrency().attr("disabled", true);
            $("#InstallationFee_Acceptance").val("0.00");
            $("#InstallationFee_PaymentMethod_Acceptance").attr("disabled", true);
        }
    }
    else if (processType == 2) {
        SetCTS020_10_EnableSection(false);

        $("#SalePrice_PaymentMethod_Approval").removeAttr("disabled");
        $("#SalePrice_PaymentMethod_Partial").removeAttr("disabled");
        $("#SalePrice_PaymentMethod_Acceptance").removeAttr("disabled");

        if (useinstallfee != "0") {
            $("#InstallationFee_PaymentMethod_Approval").removeAttr("disabled");
            $("#InstallationFee_PaymentMethod_Partial").removeAttr("disabled");
            $("#InstallationFee_PaymentMethod_Acceptance").removeAttr("disabled");
        }
    }
});


function InitialBillingSection() {
    $("#SalePrice_Approval").BindNumericBox(12, 2, 0, 999999999999.99, true);
    $("#SalePrice_Partial").BindNumericBox(12, 2, 0, 999999999999.99, true);
    $("#SalePrice_Acceptance").BindNumericBox(12, 2, 0, 999999999999.99, true);
    $("#InstallationFee_Approval").BindNumericBox(12, 2, 0, 999999999999.99, true);
    $("#InstallationFee_Partial").BindNumericBox(12, 2, 0, 999999999999.99, true);
    $("#InstallationFee_Acceptance").BindNumericBox(12, 2, 0, 999999999999.99, true);

    //$("#TotalPrice").BindNumericBox(12, 2, 0, 999999999999.99, true);

    SpecifyBillingTargetChanged();

    if ($("#HasBillingData").val() == "True") {
        $("#divSpecifyBillingTargetDetail").SetEnableView(false);
        $("#HasSiteData").val("");
    }

    $("#DocLanguage").val($("#hidDocLanguageTH").val()); //Add by Jutarat A. on 20122013
}
function InitialBillingEvents() {
    $("#btnRetrieveBillingTargetCode").click(retrieve_billing_target_code_click);
    $("#btnRetrieveBillingClientCode").click(retrieve_billing_client_code_click);

    $("#btnSearchBillingClient").click(search_billing_client_click);
    $("#btnCopyBilling").click(copy_billing_click);
    $("#btnNewEditBillingTarget").click(NewEdit_BillingCustomerData);
    $("#btnClearBillingTarget").click(clear_billing_target_click);

    $("#SalePrice_Approval").blur(SummaryBillingFee);
    $("#SalePrice_Partial").blur(SummaryBillingFee);
    $("#SalePrice_Acceptance").blur(SummaryBillingFee);

    $("#rdoBillingTargetCode").change(SpecifyBillingTargetChanged);
    $("#rdoBillingClientCode").change(SpecifyBillingTargetChanged);
}
/* ----------------------------------------------------------------------------------- */

/* --- Events --------------------------------------------------------- */
/* -------------------------------------------------------------------- */
function SpecifyBillingTargetChanged() {
    if ($("#rdoBillingTargetCode").prop("checked") == true) {
        $("#BillingTargetCode").removeAttr("readonly");
        $("#BillingTargetCode").ResetToNormalControl();

        $("#BillingClientCode").val("");
        $("#BillingClientCode").attr("readonly", true);

        $("#btnRetrieveBillingTargetCode").removeAttr("disabled");
        $("#btnRetrieveBillingClientCode").attr("disabled", "disabled");
    }
    else {
        $("#BillingTargetCode").val("");
        $("#BillingTargetCode").attr("readonly", true);

        $("#BillingClientCode").ResetToNormalControl();
        $("#BillingClientCode").removeAttr("readonly");

        $("#btnRetrieveBillingTargetCode").attr("disabled", "disabled");
        $("#btnRetrieveBillingClientCode").removeAttr("disabled");
    }

    //$("#rdoBillingTargetCode").attr("disabled", true);
}
function retrieve_billing_target_code_click() {
    var obj = {
        BillingTargetCode: $("#BillingTargetCode").val()
    };

    $("#divBillingClientInformation").clearForm();
    $("#BillingOfficeCode").removeAttr("disabled");

    call_ajax_method_json("/Contract/CTS020_RetrieveBillingTargetDetailData", obj, function (result, controls) {
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["BillingTargetCode"], controls);
            /* --------------------- */

            return;
        }
        if (result != undefined) {
            $("#divBillingClientInformation").bindJSON_Prefix("Billing", result.BillingClient);
            $("#BillingTargetCodeShort").val(result.BillingTargetCodeShort);
            $("#BillingClientCodeShort").val(result.BillingClientCodeShort);

            if ($("#BillingTargetCodeShort").val() != "") {
                $("#BillingOfficeCode").attr("disabled", true);
                $("#BillingOfficeCode").val(result.BillingOfficeCode);
            }
            DisplayDocLanguage(); //Add by Jutarat A. on 20122013

            $("#divSpecifyBillingCode").SetEnableView(false);
            $("#divCopyBillingNameAddress").SetEnableView(false);
        }
    });
}
function retrieve_billing_client_code_click() {
    var obj = {
        BillingClientCode: $("#BillingClientCode").val()
    };

    $("#divBillingClientInformation").clearForm();
    $("#BillingOfficeCode").removeAttr("disabled");

    call_ajax_method_json("/Contract/CTS020_RetrieveBillingClientDetailData", obj, function (result, controls) {
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["BillingClientCode"], controls);
            /* --------------------- */

            return;
        }
        if (result != undefined) {
            $("#divBillingClientInformation").bindJSON_Prefix("Billing", result.BillingClient);
            $("#BillingTargetCodeShort").val(result.BillingTargetCodeShort);
            $("#BillingClientCodeShort").val(result.BillingClientCodeShort);

            if ($("#BillingTargetCodeShort").val() != "") {
                $("#BillingOfficeCode").attr("disabled", true);
                $("#BillingOfficeCode").val(result.BillingOfficeCode);
            }
            DisplayDocLanguage(); //Add by Jutarat A. on 20122013

            $("#divSpecifyBillingCode").SetEnableView(false);
            $("#divCopyBillingNameAddress").SetEnableView(false);
        }
    });
}


function search_billing_client_click() {
    $("#dlgBox").OpenCMS270Dialog("CTS020");
}
function CMS270Response(object) {
    $("#dlgBox").CloseDialog();

    if (object != undefined) {
        var obj = {
            BillingClientCode: object.BillingClientCode
        };

        $("#divBillingClientInformation").clearForm();
        $("#BillingOfficeCode").removeAttr("disabled");

        call_ajax_method_json("/Contract/CTS020_RetrieveBillingClientDetailData", obj, function (result, controls) {
            if (controls != undefined) {
                /* --- Higlight Text --- */
                /* --------------------- */
                VaridateCtrl(["BillingClientCode"], controls);
                /* --------------------- */

                return;
            }
            if (result != undefined) {
                $("#divBillingClientInformation").bindJSON_Prefix("Billing", result.BillingClient);
                $("#BillingTargetCodeShort").val(result.BillingTargetCodeShort);
                $("#BillingClientCodeShort").val(result.BillingClientCodeShort);
                
                if ($("#BillingTargetCodeShort").val() != "") {
                    $("#BillingOfficeCode").attr("disabled", true);
                    $("#BillingOfficeCode").val(result.BillingOfficeCode);
                }
                DisplayDocLanguage(); //Add by Jutarat A. on 20122013

                $("#divSpecifyBillingCode").SetEnableView(false);
                $("#divCopyBillingNameAddress").SetEnableView(false);
            }
        });
    }
}


function copy_billing_click() {
    var mode = 1;
    if ($("#rdoBillingBranchOfContractTarget").prop("checked") == true)
        mode = 2;
    if ($("#rdoBillingRealCustomer").prop("checked") == true)
        mode = 3;
    if ($("#rdoBillingSite").prop("checked") == true)
        mode = 4;

    var obj = {
        CopyModeID: mode,
        BranchNameEN: $("#BranchNameEN").val(),
        BranchAddressEN: $("#BranchAddressEN").val(),
        BranchNameLC: $("#BranchNameLC").val(),
        BranchAddressLC: $("#BranchAddressLC").val()
    };

    $("#divBillingClientInformation").clearForm();
    $("#BillingOfficeCode").removeAttr("disabled");

    call_ajax_method_json("/Contract/CTS020_CopyBillingNameAddressData", obj, function (result, controls) {
        if (result != undefined) {
            $("#divBillingClientInformation").bindJSON_Prefix("Billing", result.BillingClient);
            $("#BillingTargetCodeShort").val(result.BillingTargetCodeShort);
            $("#BillingClientCodeShort").val(result.BillingClientCodeShort);

            if ($("#BillingTargetCodeShort").val() != "") {
                $("#BillingOfficeCode").attr("disabled", true);
            }
            DisplayDocLanguage(); //Add by Jutarat A. on 20122013

            $("#divSpecifyBillingCode").SetEnableView(false);
            $("#divCopyBillingNameAddress").SetEnableView(false);
        }
    });
}
function clear_billing_target_click() {
    call_ajax_method_json("/Contract/CTS020_ClearBillingTargetDetailData", "", function () {
        $("#divBillingClientInformation").clearForm();
        $("#BillingOfficeCode").removeAttr("disabled");
        $("#divSpecifyBillingCode").SetEnableView(true);
        $("#divCopyBillingNameAddress").SetEnableView(true);
        SpecifyBillingTargetChanged();

        DisplayDocLanguage(); //Add by Jutarat A. on 20122013
    });
}

/* -------------------------------------------------------------------- */

function SummaryBillingFee() {
    //var total = 0;

    //var SalePrice_Approval = parseFloat($("#SalePrice_Approval").NumericValue());
    //if (isNaN(SalePrice_Approval) == false)
    //    total += SalePrice_Approval;

    //var SalePrice_Partial = parseFloat($("#SalePrice_Partial").NumericValue());
    //if (isNaN(SalePrice_Partial) == false)
    //    total += SalePrice_Partial;

    //var SalePrice_Acceptance = parseFloat($("#SalePrice_Acceptance").NumericValue());
    //if (isNaN(SalePrice_Acceptance) == false)
    //    total += SalePrice_Acceptance;

    //$("#TotalPrice").val(SetNumericValue(total, 2));
    //$("#TotalPrice").setComma();
}


/* --- Dialog Methods (MAS030) ------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
var doBillingCustomer = null;
function NewEdit_BillingCustomerData() {
    doBillingCustomer = null;
    call_ajax_method_json("/Contract/CTS020_GetBillingClientData", "", function (result) {
        if (result != undefined)
            doBillingCustomer = result.BillingClient;
        doBillingCustomer.BillingClientCode = $("#BillingClientCodeShort").val();
        doBillingCustomer.BillingClientCodeShort = $("#BillingClientCodeShort").val();
        $("#dlgBox").OpenMAS030Dialog("CTS020");
    });
}
function MAS030Object() {
    return doBillingCustomer;
}
function MAS030Response(res) {
    $("#dlgBox").CloseDialog();

    call_ajax_method_json("/Contract/CTS020_UpdateBillingClientData", res, function (result) {
        $("#divBillingClientInformation").bindJSON_Prefix("Billing", result.BillingClient);
        $("#BillingTargetCodeShort").val(result.BillingTargetCodeShort);
        $("#BillingClientCodeShort").val(result.BillingClientCodeShort);

        if ($("#BillingTargetCodeShort").val() != "") {
            $("#BillingOfficeCode").attr("disabled", true);

        //Add by Jutarat A. on 20122013
            $("#lblInvoiceLanguage").hide();
            $("#DocLanguage").hide();
        }
        else {
            $("#lblInvoiceLanguage").show();
            $("#DocLanguage").show();
        }
        //End Add

        $("#divSpecifyBillingCode").SetEnableView(false);
        $("#divCopyBillingNameAddress").SetEnableView(false);
    });
}
/* ----------------------------------------------------------------------------------- */




function GetCTS020_10_SectionData() {
    var obj = CreateObjectData($("#formBillingTargetDetail").serialize());
    if (obj != undefined) {
        obj.BillingTargetCode = $("#SelectedBillingTargetCode").val();
        obj.BillingClientCode = $("#SelectedBillingClientCode").val();
        obj.BillingOfficeCode = $("#BillingOfficeCode").val();
        obj.DocLanguage = $("#DocLanguage").val(); //Add by Jutarat A. on 20122013

        obj.SalePrice_ApprovalCurrencyType = $("#SalePrice_Approval").NumericCurrencyValue();
        obj.SalePrice_PartialCurrencyType = $("#SalePrice_Partial").NumericCurrencyValue();
        obj.SalePrice_AcceptanceCurrencyType = $("#SalePrice_Acceptance").NumericCurrencyValue();
        obj.InstallationFee_ApprovalCurrencyType = $("#InstallationFee_Approval").NumericCurrencyValue();
        obj.InstallationFee_PartialCurrencyType = $("#InstallationFee_Partial").NumericCurrencyValue();
        obj.InstallationFee_AcceptanceCurrencyType = $("#InstallationFee_Acceptance").NumericCurrencyValue();

        if (obj.SalePrice_ApprovalCurrencyType == C_CURRENCY_US) {
            obj.SalePrice_ApprovalUsd = obj.SalePrice_Approval;
            obj.SalePrice_Approval = null;
        }
        if (obj.SalePrice_PartialCurrencyType == C_CURRENCY_US) {
            obj.SalePrice_PartialUsd = obj.SalePrice_Partial;
            obj.SalePrice_Partial = null;
        }
        if (obj.SalePrice_AcceptanceCurrencyType == C_CURRENCY_US) {
            obj.SalePrice_AcceptanceUsd = obj.SalePrice_Acceptance;
            obj.SalePrice_Acceptance = null;
        }

        if (obj.InstallationFee_ApprovalCurrencyType == C_CURRENCY_US) {
            obj.InstallationFee_ApprovalUsd = obj.InstallationFee_Approval;
            obj.InstallationFee_Approval = null;
        }
        if (obj.InstallationFee_PartialCurrencyType == C_CURRENCY_US) {
            obj.InstallationFee_PartialUsd = obj.InstallationFee_Partial;
            obj.InstallationFee_Partial = null;
        }
        if (obj.InstallationFee_AcceptanceCurrencyType == C_CURRENCY_US) {
            obj.InstallationFee_AcceptanceUsd = obj.InstallationFee_Acceptance;
            obj.InstallationFee_Acceptance = null;
        }
    }

    return obj;
}
function SetCTS020_10_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    $("#divBillingTargetDetail").SetViewMode(isview);
    if (isview) {
        $("#divSpecifyBillingTargetDetail").hide();
    }
    else {
        $("#divSpecifyBillingTargetDetail").show();
    }
}
function SetCTS020_10_EnableSection(enable) {
    $("#divBillingTargetDetail").SetEnableView(enable);
}

//Add by Jutarat A. on 20122013
function DisplayDocLanguage() {
    if ($("#BillingTargetCodeShort").val() != "") {
        $("#lblInvoiceLanguage").hide();
        $("#DocLanguage").hide();
    }
    else {
        $("#lblInvoiceLanguage").show();
        $("#DocLanguage").show();

        $("#DocLanguage").val($("#hidDocLanguageTH").val());
    }
}
//End Add