/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "QuotationTargetCode",
        "Alphabet"
    ]);

    InitSpecifyQuotation();
    InitSpecifyQuotationEvents();

    //Approve case
    if (isApproveMode) {

        //Add by Jutarat A. on 16102012
        if ($("#ProcessType").val() != null || $("#ProcessType").val() != "") {
            processType = $("#ProcessType").val();
        }
        //End Add

        RetrieveQuotationTargetData($("#ApproveQuotationTargetCode").val(), "");
    }
});

function InitSpecifyQuotation() {
    var show = (isApproveMode || isEditMode);
    ShowSpecifyQuotationSection(show);

    if (show) {
        $("#QuotationTargetCode").focus();
    }
}
function InitSpecifyQuotationEvents() {
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
    var code = $("#QuotationTargetCode").val();
    var alphabet = $("#Alphabet").val();

    $("#QuotationTargetCode").val(code);
    $("#Alphabet").val(alphabet);
    RetrieveQuotationTargetData(code, alphabet);
}
function search_quotation_target_click() {
    $("#dlgBox").OpenQUS010Dialog("CTS020");
}
function change_quotation_target_click() {
    EnableSection(false);
    $("#Alphabet").removeAttr("readonly");
    $("#btnRetrieveQuotationTarget").removeAttr("disabled");
    $("#btnSearchQuotationTarget").removeAttr("disabled");

    isChangeAlphabetMode = true;
}
function view_quotation_detail_click() {
    $("#dlgBox").OpenQUS011Dialog("CTS020");
}
/* ----------------------------------------------------------------------------------- */


function ShowSpecifyQuotationSection(show) {
    if (show) {                         //Edit,Approve
        $("#divSpecifyQuotation").show();

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

        SetCTS020_02_EnableSection(true);

        if ($("#ScreenMode").val() == 0 || isEditMode) {
            $("#btnViewQuotationDetail").attr("disabled", true);

            if (isEditMode) {
                $("#Alphabet").attr("readonly", true);
                $("#btnSearchQuotationTarget").attr("disabled", true);
            }
        }
        else
            $("#btnViewQuotationDetail").removeAttr("disabled");
    }
    else {                              //New
        $("#divSpecifyQuotation").hide();
    }
}
function RetrieveQuotationTargetData(QuotationTargetCode, Alphabet) {
    var IsChangeQuotationSite = false;
    if ($("#QuotationTargetCode").prop("readonly") == true)
        IsChangeQuotationSite = true;

    var objParam = {
        QuotationTargetCode: QuotationTargetCode,
        Alphabet: Alphabet,
        IsChangeQuotationSite: IsChangeQuotationSite,
        IsAddSale: $("#rdoProcessTypeAdd").prop("checked")
    };

    /* --- Call Retrieve quotation target method --- */
    /* --------------------------------------------- */
    call_ajax_method_json("/Contract/CTS020_RetrieveQuotationData", objParam, function (result, controls) {
        if (controls != undefined) {
            VaridateCtrl(["QuotationTargetCode", "Alphabet"], controls);

            if (result != undefined) {
                $("#QuotationTargetCode").val(result);
                $("#Alphabet").val("");
            }
        }

        if (result != undefined) {
            processType = result.ProcessType; //Add by Jutarat A. on 16102012

            FillQuotationTargetData(result);

            /* --- Init each section --- */
            /* ------------------------- */
            var funcSuccess = function () {
                /* --- Change Command --- */
                /* ---------------------- */
                if (isApproveMode)
                    InitialCommandButton(3);
                else
                    InitialCommandButton(1);
                /* ---------------------- */

                $("#btnViewQuotationDetail").removeAttr("disabled");

                if (isEditMode) {
                    $("#btnChangeQuotationTarget").removeAttr("disabled");
                }
            };

            InitModeSection(funcSuccess);
            /* ------------------------- */
        }
        else {
            if (isApproveMode != true) {
                if (processType == 1 && isChangeAlphabetMode == false) {
                    $("#QuotationTargetCode").removeAttr("readonly");
                }
                if (isEditMode != true) {
                    $("#Alphabet").removeAttr("readonly");
                }
            }

            if (isChangeAlphabetMode) {
                InitialCommandButton(1);
                DisableRegisterCommand(true);
            }
            else {
                InitialCommandButton(4);
            }
        }

        isChangeAlphabetMode = false;
    });
    /* --------------------------------------------- */

    InitialCommandButton(0);
}
function FillQuotationTargetData(specifyData) {
    SetCTS020_02_EnableSection(false);

    var QuotationTargetCode = $("#QuotationTargetCode").val().toUpperCase();
    var Alphabet = $("#Alphabet").val().toUpperCase();

    $("#divSpecifyQuotation").clearForm();
    if (isApproveMode) {
        QuotationTargetCode = specifyData.doTbt_DraftSaleContract.QuotationTargetCodeShort;
        Alphabet = specifyData.doTbt_DraftSaleContract.Alphabet;
    }
    else if (isEditMode) {
        Alphabet = specifyData.doTbt_DraftSaleContract.Alphabet;
    }

    $("#QuotationTargetCode").val(QuotationTargetCode);
    $("#Alphabet").val(Alphabet);
    $("#ProductName").val(specifyData.doTbt_DraftSaleContract.ProductName);
}



/* -------- QUS010 ------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function QUS010Object() {
    var targetCodeTypeCode = $("#C_TARGET_CODE_TYPE_QTN_CODE").val();
    if ($("#rdoProcessTypeAdd").prop("checked") == true)
        targetCodeTypeCode = $("#C_TARGET_CODE_TYPE_CONTRACT_CODE").val();

    var obj = {
        strServiceTypeCode: $("#C_SERVICE_TYPE_SALE").val(),
        strTargetCodeTypeCode: targetCodeTypeCode
    };

    if ($("#QuotationTargetCode").prop("readonly") == true) {
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
        $("#QuotationTargetCode").attr("readonly", true);
        $("#Alphabet").attr("readonly", true);

        $("#QuotationTargetCode").val(result.QuotationTargetCode);
        $("#Alphabet").val(result.Alphabet);

        RetrieveQuotationTargetData(result.QuotationTargetCode, result.Alphabet);
    }
}
/* ----------------------------------------------------------------------------------- */




/* -------- QUS011 ------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function CTS020_QUS011Object() {
    return {
        QuotationTargetCode: $("#QuotationTargetCode").val(),
        Alphabet: $("#Alphabet").val(),
        HideQuotationTarget: true
    };
}
/* ----------------------------------------------------------------------------------- */






function GetCTS020_02_SectionData() {
    return {
        QuotationTargetCode: $("#QuotationTargetCode").val(),
        Alphabet: $("#Alphabet").val()
    };
}
function SetCTS020_02_EnableSection(enable) {
    $("#divSpecifyQuotation").SetEnableView(enable);

    if (enable) {
        $("#ProductName").attr("readonly", true);

        if ($("#btnChangeQuotationTarget").length > 0) {
            $("#btnChangeQuotationTarget").show();
            $("#btnChangeQuotationTarget").attr("disabled", true);
        }

        $("#btnViewQuotationDetail").attr("disabled", true);
    }
    else {
        $("#QuotationTargetCode").attr("readonly", true);
        $("#Alphabet").attr("readonly", true);
    }
}
function SetCTS020_02_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#divSpecifyQuotation").SetViewMode(true);
        $("#divQuotationTargetCode").html($("#QuotationTargetCode").val() + "-" + $("#Alphabet").val());
        $("#divAlphabet").html("");
    }
    else {
        $("#divSpecifyQuotation").SetViewMode(false);
        if (isApproveMode) {
            $("#btnRetrieveQuotationTarget").hide();
            $("#btnSearchQuotationTarget").hide();
        }
    }
}