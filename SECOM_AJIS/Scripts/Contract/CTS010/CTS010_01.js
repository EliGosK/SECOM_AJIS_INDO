function GetCTS010_01_SectionData() {
    return {
        QuotationTargetCode: $("#QuotationTargetCode").val(),
        Alphabet: $("#Alphabet").val()
    };
}
function SetCTS010_01_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#divQuotationTargetInfo").SetViewMode(true);
        $("#divQuotationTargetCode").html($("#QuotationTargetCode").val() + "-" + $("#Alphabet").val());
        $("#divAlphabet").html("");

        if ($("#SecurityAreaTo").val() == "") {
            $("#divSecurityAreaFromTo").hide();
            $("#divSecurityAreaTo").html("");
        }
    }
    else {
        $("#divQuotationTargetInfo").SetViewMode(false);
        $("#ServiceType").find("input").each(function () {
            $(this).attr("disabled", true);
        });
        if (isApproveMode) {
            $("#btnRetrieveQuotationTarget").hide();
            $("#btnSearchQuotationTarget").hide();
        }

        $("#divSecurityAreaFromTo").show();
    }
}
function SetCTS010_01_EnableSection(enable) {
    if (enable) {
        $("#QuotationTargetCode").attr("readonly", true);
        $("#Alphabet").removeAttr("readonly");
        $("#btnRetrieveQuotationTarget").removeAttr("disabled");
        $("#btnSearchQuotationTarget").removeAttr("disabled");
        $("#btnChangeQuotationTarget").removeAttr("disabled");
        $("#btnViewQuotationDetail").removeAttr("disabled");
    }
    else {
        $("#QuotationTargetCode").attr("readonly", true);
        $("#Alphabet").removeAttr("readonly");
        $("#btnRetrieveQuotationTarget").removeAttr("disabled");
        $("#btnSearchQuotationTarget").removeAttr("disabled");
        $("#btnChangeQuotationTarget").attr("disabled", true);
        $("#btnViewQuotationDetail").attr("disabled", true);
    }

}





/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "QuotationTargetCode",
        "Alphabet"
    ]);

    //Approve case
    if ($("#ApproveQuotationTargetCode").val() != "") {
        isApproveMode = true;
    }
    else if ($("#ScreenMode").val() == 1)
        isEditMode = true;

    InitQuotationTargetInfo();
    InitQuotationTargetEvents();

    //Approve case
    if (isApproveMode) {
        RetrieveQuotationTargetData($("#ApproveQuotationTargetCode").val(), "");
    }
});

/* ----------------------------------------------------------------------------------- */

/* --- Quotation Target Information -------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */

function InitQuotationTargetInfo() {
    /// <summary>Method to initial Quotation target section</summary>

    /* --- Visible Controls --- */
    /* ------------------------ */
    if (isApproveMode) {
        $("#btnRetrieveQuotationTarget").hide();
        $("#btnSearchQuotationTarget").hide();
    }
    else {
        $("#btnRetrieveQuotationTarget").show();
        $("#btnSearchQuotationTarget").show();
    }

    if ($("#btnChangeQuotationTarget").length > 0) {
        $("#btnChangeQuotationTarget").show();
        $("#btnChangeQuotationTarget").attr("disabled", true);
    }

    $("#btnViewQuotationDetail").show();
    SetCTS010_01_SectionMode(false);
    /* ------------------------ */

    /* --- Enable Controls --- */
    /* ----------------------- */
    $("#QuotationTargetCode").removeAttr("readonly");
    $("#Alphabet").removeAttr("readonly");
    $("#btnRetrieveQuotationTarget").removeAttr("disabled");
    $("#btnSearchQuotationTarget").removeAttr("disabled");

    if ($("#ScreenMode").val() == 0 || isEditMode) {
        $("#btnViewQuotationDetail").attr("disabled", true);

        if (isEditMode) {
            $("#Alphabet").attr("readonly", true);
            $("#btnSearchQuotationTarget").attr("disabled", true);
        }
    }
    else
        $("#btnViewQuotationDetail").removeAttr("disabled");
    /* ----------------------- */

    /* --- Reset Data --- */
    /* ------------------ */
    $("#divQuotationTargetInfo").clearForm();
    /* ------------------ */

    /* --- Focus control --- */
    /* --------------------- */
    $("#QuotationTargetCode").focus();
    /* --------------------- */
}
function InitQuotationTargetEvents() {
    /// <summary>Method to init event in Quotation target section</summary>
    $("#btnRetrieveQuotationTarget").click(retrieve_quotation_target_click);
    $("#btnSearchQuotationTarget").click(search_quotation_target_click);

    if ($("#btnChangeQuotationTarget").length > 0)
        $("#btnChangeQuotationTarget").click(change_quotation_target_click);

    $("#btnViewQuotationDetail").click(view_quotation_detail_click);
}

/* ----------------------------------------------------------------------------------- */





/* -------- Events ------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */

function retrieve_quotation_target_click() {
    RetrieveQuotationTargetData($("#QuotationTargetCode").val(), $("#Alphabet").val());
}
function search_quotation_target_click() {
    $("#divQuotationTargetInfo").ResetToNormalControl();
    $("#dlgBox").OpenQUS010Dialog("CTS010");
}
function change_quotation_target_click() {
    EnableSection(false);
    $("#Alphabet").removeAttr("readonly");
    $("#btnRetrieveQuotationTarget").removeAttr("disabled");
}
function view_quotation_detail_click() {
    $("#dlgBox").OpenQUS012Dialog("CTS010");
}
/* ----------------------------------------------------------------------------------- */





/* -------- Methods ------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */

function RetrieveQuotationTargetData(QuotationTargetCode, Alphabet) {
    var IsChangeQuotationSite = false;
    if ($("#QuotationTargetCode").prop("readonly") == true) {
        IsChangeQuotationSite = true;
        ResetSection();
    }

    var objParam = {
        QuotationTargetCode: QuotationTargetCode,
        Alphabet: Alphabet,
        IsChangeQuotationSite: IsChangeQuotationSite
    };

    /* --- Call Retrieve quotation target method --- */
    /* --------------------------------------------- */
    call_ajax_method_json("/Contract/CTS010_RetrieveQuotationData", objParam, function (result, controls) {
        if (controls != undefined) {
            VaridateCtrl(["QuotationTargetCode", "Alphabet"], controls);

            if (result != undefined) {
                $("#QuotationTargetCode").val(result);
                $("#Alphabet").val("");
            }
        }
        else if (result != undefined) {
            FillQuotationTargetData(result);

            /* --- Init each section --- */
            /* ------------------------- */
            ProductTypeMode = {
                IsProductTypeAL: result.IsProductTypeAL,
                IsProductTypeRentalSale: result.IsProductTypeRentalSale,
                IsProductSaleOnline: result.IsProductSaleOnline,
                IsProductBeatGuard: result.IsProductBeatGuard,
                IsProductSentryGuard: result.IsProductSentryGuard,
                IsProductMaintenance: result.IsProductMaintenance
            };

            var funcSuccess = function () {
                /* --- Change Command --- */
                /* ---------------------- */
                if (isApproveMode)
                    InitialCommandButton(3);
                else
                    InitialCommandButton(1);
                /* ---------------------- */

                $("#btnViewQuotationDetail").removeAttr("disabled");
                $("#btnViewQuotationDetail").focus();

                if (isEditMode) {
                    $("#btnChangeQuotationTarget").removeAttr("disabled");
                }
            };

            InitModeSection(funcSuccess);
            /* ------------------------- */
        }
    });
    /* --------------------------------------------- */
}
function FillQuotationTargetData(specifyData) {
    /* --- Disabled Controls --- */
    /* ------------------------- */
    $("#QuotationTargetCode").attr("readonly", true);
    $("#Alphabet").attr("readonly", true);
    $("#btnRetrieveQuotationTarget").attr("disabled", true);
    $("#btnSearchQuotationTarget").attr("disabled", true);
    $("#btnViewQuotationDetail").attr("disabled", true);
    /* ------------------------- */

    /* --- Fill Data to Control --- */
    /* ---------------------------- */
    var QuotationTargetCode = $("#QuotationTargetCode").val().toUpperCase();
    var Alphabet = $("#Alphabet").val().toUpperCase();

    $("#divQuotationTargetInfo").clearForm();
    if (specifyData != null) {
        if (isApproveMode) {
            QuotationTargetCode = specifyData.doTbt_DraftRentalContrat.QuotationTargetCodeShort;
            Alphabet = specifyData.doTbt_DraftRentalContrat.Alphabet;
        }
        else if (isEditMode) {
            Alphabet = specifyData.doTbt_DraftRentalContrat.Alphabet;
        }


        $("#CustFullNameEN").val(specifyData.doContractCustomer.CustFullNameEN);
        $("#SiteNameEN").val(specifyData.doSite.SiteNameEN);
        $("#AddressFullEN").val(specifyData.doSite.AddressFullEN);
        $("#CustFullNameLC").val(specifyData.doContractCustomer.CustFullNameLC);
        $("#SiteNameLC").val(specifyData.doSite.SiteNameLC);
        $("#AddressFullLC").val(specifyData.doSite.AddressFullLC);
        $("#divQuotationTarget_ContractInfo").bindJSON(specifyData.doTbt_DraftRentalContrat);

        if (specifyData.doTbt_DraftRentalContrat != undefined) {
            $("#SecurityAreaFrom").val(SetNumericValue(specifyData.doTbt_DraftRentalContrat.SecurityAreaFrom, 2));
            $("#SecurityAreaFrom").setComma();
            $("#SecurityAreaTo").val(SetNumericValue(specifyData.doTbt_DraftRentalContrat.SecurityAreaTo, 2));
            $("#SecurityAreaTo").setComma();
        }

        if (specifyData.doTbt_DraftRentalContrat != null) {
            $("#ServiceType input[type=checkbox]").each(function (idx) {
                if (idx == 0) {
                    if (specifyData.doTbt_DraftRentalContrat.FireMonitorFlag == true)
                        $(this).attr("checked", true);
                }
                else if (idx == 1) {
                    if (specifyData.doTbt_DraftRentalContrat.CrimePreventFlag == true)
                        $(this).attr("checked", true);
                }
                else if (idx == 2) {
                    if (specifyData.doTbt_DraftRentalContrat.EmergencyReportFlag == true)
                        $(this).attr("checked", true);
                }
                else if (idx == 3) {
                    if (specifyData.doTbt_DraftRentalContrat.FacilityMonitorFlag == true)
                        $(this).attr("checked", true);
                }
            });
        }
    }

    $("#QuotationTargetCode").val(QuotationTargetCode);
    $("#Alphabet").val(Alphabet);
    /* ---------------------------- */
}

/* ----------------------------------------------------------------------------------- */





/* -------- QUS010 ------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function QUS010Object() {
    var obj = {
        strServiceTypeCode: $("#C_SERVICE_TYPE_RENTAL").val(),
        strTargetCodeTypeCode: $("#C_TARGET_CODE_TYPE_QTN_CODE").val()
    };

    if (isEditMode) {
        obj.strQuotationTargetCode = $("#QuotationTargetCode").val();
    }
    else {
        obj.strContractTransferStatus = $("#C_CONTRACT_TRANS_STATUS_QTN_REG").val();
    }

    return obj;
}
function QUS010Response(result) {
    $("#dlgBox").CloseDialog();
    if (result != undefined) {
        $("#QuotationTargetCode").val(result.QuotationTargetCode);
        $("#Alphabet").val(result.Alphabet);

        RetrieveQuotationTargetData(result.QuotationTargetCode, result.Alphabet);
    }
}
/* ----------------------------------------------------------------------------------- */




/* -------- QUS012 ------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function CTS010_QUS012Object() {
    return {
        QuotationTargetCode: $("#QuotationTargetCode").val(),
        Alphabet: $("#Alphabet").val(),
        HideQuotationTarget: true
    };
}
/* ----------------------------------------------------------------------------------- */