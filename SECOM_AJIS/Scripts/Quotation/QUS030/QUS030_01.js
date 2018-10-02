/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "QuotationTargetCodeShort"
    ]);

    InitQuotationTargetInfo();
    InitQuotationTargetEvents();

    if ($("#IsImportFromQUS020").val() != "")
        QUS050Response(true, $("#IsImportFromQUS020").val());
    else if ($("#defQuotationTargetCode").val() != "") {
        $("#QuotationTargetCodeShort").val($("#defQuotationTargetCode").val());
        retrieve_quotation_target_click();
    }
});
/* ----------------------------------------------------------------------------------- */

/* --- Quotation Target Information -------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function InitQuotationTargetInfo() {
    /// <summary>Method to initial Quotation target section</summary>

    /* --- Visible Controls --- */
    /* ------------------------ */
    $("#btnImportQuotationInfo").show();
    $("#btnRetrieveQuotationTarget").show();
    $("#btnSearchQuotationTarget").show();
    ContractBranchInfoVisible(false);
    SetQUS030_01_SectionMode(false);
    /* ------------------------ */

    /* --- Enable Controls --- */
    /* ----------------------- */
    $("#QuotationTargetCodeShort").removeAttr("readonly");
    $("#btnRetrieveQuotationTarget").removeAttr("disabled");
    $("#btnSearchQuotationTarget").removeAttr("disabled");
    $("#btnImportQuotationInfo").removeAttr("disabled");
    /* ----------------------- */

    /* --- Reset Data --- */
    /* ------------------ */
    $("#divQuotationTargetInfo").clearForm();
    /* ------------------ */

    /* --- Focus control --- */
    /* --------------------- */
    $("#QuotationTargetCodeShort").focus();
    /* --------------------- */
}
function InitQuotationTargetEvents() {
    /// <summary>Method to init event in Quotation target section</summary>
    $("#btnRetrieveQuotationTarget").click(retrieve_quotation_target_click);
    $("#btnSearchQuotationTarget").click(search_quotation_target_click);
    ImportSectionEvents();
}
function FillQuotationTargetData(doInitQuotationData) {
    /// <summary>Method to fill data to Quotation target section</summary>

    /* --- Visible Controls --- */
    /* ------------------------ */
    ContractBranchInfoVisible(false);
    /* ------------------------ */

    /* --- Enable Controls --- */
    /* ----------------------- */
    $("#QuotationTargetCodeShort").attr("readonly", true);
    $("#btnRetrieveQuotationTarget").attr("disabled", true);
    $("#btnSearchQuotationTarget").attr("disabled", true);
    $("#btnImportQuotationInfo").attr("disabled", true);
    /* ----------------------- */

    /* --- Fill Data to Control --- */
    /* ---------------------------- */
    $("#divQuotationTargetInfo").clearForm();
    if (doInitQuotationData != null) {
        if (doInitQuotationData.doQuotationHeaderData != null) {
            $("#divQuotationTargetInfo").bindJSON(doInitQuotationData.doQuotationHeaderData.doQuotationTarget);
            $("#divPurchaserInformation").bindJSON_Prefix("CP", doInitQuotationData.doQuotationHeaderData.doContractTarget);
            $("#divRealCustomerInformation").bindJSON_Prefix("CR", doInitQuotationData.doQuotationHeaderData.doRealCustomer);
            $("#divSiteInformation").bindJSON(doInitQuotationData.doQuotationHeaderData.doQuotationSite);

            if (doInitQuotationData.doQuotationHeaderData.doQuotationTarget.BranchNameEN != null
                || doInitQuotationData.doQuotationHeaderData.doQuotationTarget.BranchNameLC != null
                || doInitQuotationData.doQuotationHeaderData.doQuotationTarget.BranchAddressEN != null
                || doInitQuotationData.doQuotationHeaderData.doQuotationTarget.BranchAddressLC != null) {

                ContractBranchInfoVisible(true);
            }
        }
    }
    /* ---------------------------- */
}

/* -------- Events ------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function retrieve_quotation_target_click() {
    /// <summary>Event when click Retrieve quotation target</summary>
    var objParam = {
        QuotationTargetCode: $("#QuotationTargetCodeShort").val()
    };

    /* --- Call Retrieve quotation target method --- */
    /* --------------------------------------------- */
    $("#btnRetrieveQuotationTarget").attr("disabled", true);
    $("#btnSearchQuotationTarget").attr("disabled", true);

    call_ajax_method_json("/Quotation/QUS030_RetrieveQuotationTargetData", objParam, function (result, controls) {
        $("#btnRetrieveQuotationTarget").removeAttr("disabled");
        $("#btnSearchQuotationTarget").removeAttr("disabled");

        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["QuotationTargetCodeShort"], controls);
            /* --------------------- */

            return;
        }
        else if (result != undefined) {
            FillQuotationTargetData(result);

            /* --- Init each section --- */
            /* ------------------------- */
            ProductTypeMode = {
                IsProductTypeSale: result.IsProductTypeSale,
                IsProductTypeAL: result.IsProductTypeAL,
                IsProductTypeRentalSale: result.IsProductTypeRentalSale,
                IsProductSaleOnline: result.IsProductSaleOnline,
                IsProductBeatGuard: result.IsProductBeatGuard,
                IsProductSentryGuard: result.IsProductSentryGuard,
                IsProductMaintenance: result.IsProductMaintenance,
                IsShowInstrument01: result.IsShowInstrument01
            };

            var funcSuccess = function () {
                /* --- Change Command --- */
                /* ---------------------- */
                InitialCommandButton(1);
                /* ---------------------- */
            };

            if (result.IsProductTypeSale)
                InitSaleSection(funcSuccess);
            else if (result.IsProductTypeAL || result.IsProductTypeRentalSale)
                InitALSection(funcSuccess);
            else if (result.IsProductSaleOnline)
                InitSaleOnlineSection(funcSuccess);
            else if (result.IsProductBeatGuard)
                InitBeatGuardSection(funcSuccess);
            else if (result.IsProductSentryGuard)
                InitSentryGuardSection(funcSuccess);
            else if (result.IsProductMaintenance)
                InitMaintenanceDetailSection(funcSuccess);
            /* ------------------------- */
        }

        //$('#SecurityAreaFrom').val('0.00');
        //$('#SecurityAreaTo').val('0.00');
        //$('#SiteBuildingArea').val('0.00');

        //$('#PhoneLineTypeCode1').val('9');
        //$('#PhoneLineTypeCode1').val('2');
    });
    /* --------------------------------------------- */
}
function search_quotation_target_click() {
    /// <summary>Event when click Search quotation target</summary>
    SearchQuotationTargetData();
}
/* ----------------------------------------------------------------------------------- */

/* -------- Methods ------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function ContractBranchInfoVisible(visible) {
    /// <summary>Method to set Visible/Disable Branch Information</summary>
    /// <param name="visible" type="bool">Flag to visible</param>
    if (visible) {
        $("#divBranchContractSection").show();
    }
    else {
        $("#BranchNameEN").val("");
        $("#BranchAddressEN").val("");
        $("#BranchNameLC").val("");
        $("#BranchAddressLC").val("");
        $("#divBranchContractSection").hide();
    }
}
function SetQUS030_01_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#divQuotationTargetInfo").SetViewMode(true);
        $("#btnImportQuotationInfo").hide();

        if ($("#QuotationStaffName").val() == "") {
            $("#divQuotationStaffName").html("");
        }
    }
    else {
        $("#divQuotationTargetInfo").SetViewMode(false);
        $("#btnImportQuotationInfo").show();
    }
}
/* ----------------------------------------------------------------------------------- */

/* ----------------------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */





/* ----------------------------------------------------------------------------------- */
/* --- Import Section ---------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */

/* --- Import Section  > Initial ----------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function ImportSectionEvents() {
    $("#btnImportQuotationInfo").click(import_quotation_info_click);
}
/* ----------------------------------------------------------------------------------- */

/* --- Import Section  > Events ------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function import_quotation_info_click() {
    $("#dlgBox").OpenQUS050Dialog("QUS030");
}
/* ----------------------------------------------------------------------------------- */

/* --- Import Section  > Dialog Methods ---------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function QUS050Response(dsImportData, key) {
    $("#dlgBox").CloseDialog();

    if (dsImportData != undefined) {
        /* --- Call Retrieve quotation target method --- */
        /* --------------------------------------------- */
        call_ajax_method_json("/Quotation/QUS030_ImportQuotationData", { ImportKey: key }, function (result, controls) {
            if (result != undefined) {
                FillQuotationTargetData(result);

                /* --- Init each section --- */
                /* ------------------------- */
                ProductTypeMode = {
                    IsProductTypeSale: result.IsProductTypeSale,
                    IsProductTypeAL: result.IsProductTypeAL,
                    IsProductTypeRentalSale: result.IsProductTypeRentalSale,
                    IsProductSaleOnline: result.IsProductSaleOnline,
                    IsProductBeatGuard: result.IsProductBeatGuard,
                    IsProductSentryGuard: result.IsProductSentryGuard,
                    IsProductMaintenance: result.IsProductMaintenance,
                    IsShowInstrument01: result.IsShowInstrument01
                };

                var funcSuccess = function () {
                    /* --- Set Command Button --- */
                    /* -------------------------- */
                    InitialCommandButton(1);
                    /* -------------------------- */

                    if (controls != undefined) {
                        /* --- Higlight Text --- */
                        /* --------------------- */
                        VaridateCtrl(["ProductCode",
                                        "PhoneLineTypeCode1",
                                        "PhoneLineTypeCode2",
                                        "PhoneLineTypeCode3",
                                        "PhoneLineOwnerTypeCode1",
                                        "PhoneLineOwnerTypeCode2",
                                        "PhoneLineOwnerTypeCode3",
                                        "MainStructureTypeCode",
                                        "BuildingTypeCode",
                                        "MaintenanceCycle",
                                        "InsuranceTypeCode"], controls);
                        /* --------------------- */
                    }

                    DisableRegisterCommand(result.IsEnableRegister);
                };

                if (result.IsProductTypeSale)
                    InitSaleSection(funcSuccess);
                else if (result.IsProductTypeAL || result.IsProductTypeRentalSale)
                    InitALSection(funcSuccess);
                else if (result.IsProductSaleOnline)
                    InitSaleOnlineSection(funcSuccess);
                else if (result.IsProductBeatGuard)
                    InitBeatGuardSection(funcSuccess);
                else if (result.IsProductSentryGuard)
                    InitSentryGuardSection(funcSuccess);
                else if (result.IsProductMaintenance)
                    InitMaintenanceDetailSection(funcSuccess);
                /* ------------------------- */
            }
        });
        /* --------------------------------------------- */
    }
}
function QUS050Object() {
    return {
        ScreenID: "QUS030"
    };
}
/* ----------------------------------------------------------------------------------- */

/* --- Dialog Methods (QUS040) ------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function SearchQuotationTargetData() {
    $("#dlgBox").OpenQUS040Dialog();
}
function QUS040Response(result) {
    $("#dlgBox").CloseDialog();

    $("#QuotationTargetCodeShort").val(result);
    retrieve_quotation_target_click();
}
/* ----------------------------------------------------------------------------------- */