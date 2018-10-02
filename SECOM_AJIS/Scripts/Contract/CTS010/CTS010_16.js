/// <reference path="../../Base/GridControl.js" />

/* --- Variable ------------------------------------------------------- */
/* -------------------------------------------------------------------- */
var gridBillingTarget = null;
var btnBillingTargetDetail = "btnBillingTargetDetail";
var btnRemoveBillingTargetDetail = "btnRemoveBillingTargetDetail";
var billing_target_mode = 0; //0 = Add, 1 = Edit
/* -------------------------------------------------------------------- */




var isBillingSelected = false;

function CTS010_16_InitialGrid() {
    /* --- Initial Grid --- */
    /* -------------------- */
    gridBillingTarget = $("#gridBillingTarget").LoadDataToGridWithInitial(0, false, false,
                                "/Contract/CTS010_GetBillingTargetList",
                                "",
                                "CTS010_BillingTargetData", false);
    /* -------------------- */

    SpecialGridControl(gridBillingTarget, ["Edit", "Remove"]);

    gridBillingTarget.attachEvent("onBeforeSelect", function (new_row, old_row) {
        if (isBillingSelected == true)
            return false;
        else
            return true;
    });

    var EditBillingTargetDetailEvent = function (row_id) {
        /* --- Disable All Button --- */
        /* -------------------------- */
        $("#gridBillingTarget").find("img").each(function () {
            if (this.id != "" && this.id != undefined) {
                if (this.id.indexOf(btnBillingTargetDetail) == 0) {
                    var rid = GetGridRowIDFromControl(this);
                    EnableGridButton(gridBillingTarget, btnBillingTargetDetail, rid, "Edit", false);
                }
                else if (this.id.indexOf(btnRemoveBillingTargetDetail) == 0) {
                    var rid = GetGridRowIDFromControl(this);
                    EnableGridButton(gridBillingTarget, btnRemoveBillingTargetDetail, rid, "Remove", false);
                }
            }
        });
        $("#btnNewBillingTargetDetail").attr("disabled", true);
        /* -------------------------- */

        if (row_id == -1) {
            $("#divBillingTargetDetail").clearForm();
            $("#divSpecifyBillingCode").SetEnableView(true);
            $("#divCopyBillingNameAddress").SetEnableView(true);
            SpecifyBillingTargetChanged();

            $("#rdoBillingClientCode").attr("checked", true);
            $("#rdoBillingContractTarget").attr("checked", true);
            $("#BillingOfficeCode").removeAttr("disabled");
            $("#divBillingTargetDetail").show();

            $("#ContractFee").val("");
            $("#ContractFee").SetNumericCurrency($("#NormalContractFee").NumericCurrencyValue());

            $("#InstallFee_Approval").val("");
            $("#InstallFee_Approval").SetNumericCurrency($("#NormalInstallFee").NumericCurrencyValue());

            $("#PaymentMethod_Approval").val("0");

            $("#InstallFee_CompleteInstallation").val("");
            $("#InstallFee_CompleteInstallation").SetNumericCurrency($("#NormalInstallFee").NumericCurrencyValue());

            $("#PaymentMethod_CompleteInstallation").val("0");

            $("#InstallFee_StartService").val("");
            $("#InstallFee_StartService").SetNumericCurrency($("#NormalInstallFee").NumericCurrencyValue());

            $("#PaymentMethod_StartService").val("0");

            $("#TotalFee").val("0.00");
            $("#TotalFeeUsd").val("0.00");
            
            $("#DepositFee").val("");
            $("#DepositFee").SetNumericCurrency($("#NormalDepositFee").NumericCurrencyValue());

            $("#PaymentMethod_Deposit").val("0");
            
            $("#rdoBillingClientCode").focus();
            billing_target_mode = 0;
            isBillingSelected = true;

            $("#DocLanguage").val($("#hidDocLanguageTH").val()); //Add by Jutarat A. on 18122013
        }
        else {
            gridBillingTarget.selectRow(gridBillingTarget.getRowIndex(row_id));

            var keyCol = gridBillingTarget.getColIndexById("BillingTargetKeyIndex");
            var obj = {
                //key: gridBillingTarget.cells(rid, keyCol).getValue()
                rowIndex: gridBillingTarget.getRowIndex(row_id)
            };

            call_ajax_method_json("/Contract/CTS010_GetSelectedBillingTargetDetailData", obj, function (result, controls) {
                if (result != undefined) {
                    $("#divBillingTargetDetail").clearForm();
                    $("#divSpecifyBillingCode").SetEnableView(false);
                    $("#divCopyBillingNameAddress").SetEnableView(false);

                    $("#rdoBillingClientCode").attr("checked", true);
                    $("#rdoBillingContractTarget").attr("checked", true);
                    $("#BillingOfficeCode").removeAttr("disabled");
                    
                    $("#divBillingClientInformation").bindJSON_Prefix("Billing", result.BillingClient);
                    $("#SelectedBillingTargetCode").val(result.BillingTargetCode);
                    $("#SelectedBillingClientCode").val(result.BillingClientCode);
                    $("#BillingTargetCodeShort").val(result.BillingTargetCodeShort);
                    $("#BillingClientCodeShort").val(result.BillingClientCodeShort);

                    if ($("#BillingTargetCodeShort").val() != "") {
                        $("#BillingOfficeCode").attr("disabled", true);
                    }
                    DisplayDocLanguage(); //Add by Jutarat A. on 18122013

                    $("#BillingOfficeCode").val(result.BillingOfficeCode);
                    $("#DocLanguage").val(result.DocLanguage); //Add by Jutarat A. on 18122013

                    $("#formBillingTargetDetail").bindJSON(result);

                    if (result.ContractFeeCurrencyType == C_CURRENCY_US) {
                        $("#ContractFee").val(SetNumericValue(result.ContractFeeUsd, 2));
                    }
                    else {
                        $("#ContractFee").val(SetNumericValue(result.ContractFee, 2));
                    }

                    if (result.InstallFee_ApprovalCurrencyType == C_CURRENCY_US) {
                        $("#InstallFee_Approval").val(SetNumericValue(result.InstallFee_ApprovalUsd, 2));
                    }
                    else {
                        $("#InstallFee_Approval").val(SetNumericValue(result.InstallFee_Approval, 2));
                    }

                    if (result.InstallFee_CompleteInstallationCurrencyType == C_CURRENCY_US) {
                        $("#InstallFee_CompleteInstallation").val(SetNumericValue(result.InstallFee_CompleteInstallationUsd, 2));
                    }
                    else {
                        $("#InstallFee_CompleteInstallation").val(SetNumericValue(result.InstallFee_CompleteInstallation, 2));
                    }

                    if (result.InstallFee_StartServiceCurrencyType == C_CURRENCY_US) {
                        $("#InstallFee_StartService").val(SetNumericValue(result.InstallFee_StartServiceUsd, 2));
                    }
                    else {
                        $("#InstallFee_StartService").val(SetNumericValue(result.InstallFee_StartService, 2));
                    }

                    if (result.DepositFeeCurrencyType == C_CURRENCY_US) {
                        $("#DepositFee").val(SetNumericValue(result.DepositFeeUsd, 2));
                    }
                    else {
                        $("#DepositFee").val(SetNumericValue(result.DepositFee, 2));
                    }

                    $("#ContractFee").setComma();
                    $("#InstallFee_Approval").setComma();
                    $("#InstallFee_CompleteInstallation").setComma();
                    $("#InstallFee_StartService").setComma();
                    $("#DepositFee").setComma();

                    SummaryBillingFee();

                    $("#divBillingTargetDetail").show();

                    if ($("#BillingOfficeCode").prop("disabled") == false) //Add by Jutarat A. on 20122013
                        $("#BillingOfficeCode").focus();
                }
            });

            billing_target_mode = 1;
        }

        var lst = ["OrderInstallFee_ApproveContract",
                    "OrderInstallFee_CompleteInstall",
                    "OrderInstallFee_StartService"];
        for (var idx = 0; idx < lst.length; idx++) {
            var disabled = GetDisableFlag(lst[idx]);
            if (disabled == true) {
                if (idx == 0) {
                    $("#InstallFee_Approval").attr("readonly", true);
                    $("#InstallFee_Approval").NumericCurrency().attr("disabled", true);
                    $("#PaymentMethod_Approval").attr("disabled", true);
                }
                else if (idx == 1) {
                    $("#InstallFee_CompleteInstallation").attr("readonly", true);
                    $("#InstallFee_CompleteInstallation").NumericCurrency().attr("disabled", true);
                    $("#PaymentMethod_CompleteInstallation").attr("disabled", true);
                }
                else if (idx == 2) {
                    $("#InstallFee_StartService").attr("readonly", true);
                    $("#InstallFee_StartService").NumericCurrency().attr("disabled", true);
                    $("#PaymentMethod_StartService").attr("disabled", true);
                }
            }
        }

        isBillingSelected = true;
    }

    BindOnLoadedEvent(gridBillingTarget, function (gen_ctrl) {
        for (var i = 0; i < gridBillingTarget.getRowsNum(); i++) {
            var row_id = gridBillingTarget.getRowId(i);

            var textInstallFeeColIdx = gridBillingTarget.getColIndexById("TextInstallationFee");
            var textInstallFee = gridBillingTarget.cells(row_id, textInstallFeeColIdx).getValue();
            textInstallFee = textInstallFee.replace(/&lt;/g, "<").replace(/&gt;/g, ">");
            gridBillingTarget.cells(row_id, textInstallFeeColIdx).setValue(textInstallFee);

            if (gen_ctrl == true) {
                GenerateEditButton(gridBillingTarget, btnBillingTargetDetail, row_id, "Edit", true);
                GenerateRemoveButton(gridBillingTarget, btnRemoveBillingTargetDetail, row_id, "Remove", true);
            }

            /* --- Set Event --- */
            /* ----------------- */
            BindGridButtonClickEvent(btnBillingTargetDetail, row_id, function (rid) {
                EditBillingTargetDetailEvent(rid);
            });
            BindGridButtonClickEvent(btnRemoveBillingTargetDetail, row_id, function (rid) {
                var keyCol = gridBillingTarget.getColIndexById("BillingTargetKeyIndex");
                var obj = {
                    //key: gridBillingTarget.cells(rid, keyCol).getValue()
                    rowIndex: gridBillingTarget.getRowIndex(rid)
                };
                call_ajax_method_json("/Contract/CTS010_RemoveSelectedBillingTargetDetailData", obj, function (result, controls) {
                    DeleteRow(gridBillingTarget, rid);

                    if (result != undefined) {
                        $("#TotalContractFee").val(result.TotalContractFee);
                        $("#TotalInstallationFee").val(result.TotalInstallationFee);
                        $("#TotalDepositFee").val(result.TotalDepositFee);
                    }
                });
            });
            /* ----------------- */
        }
        gridBillingTarget.setSizes();
    });
    /* ------------------------ */

    $("#btnNewBillingTargetDetail").click(function () {
        EditBillingTargetDetailEvent(-1);
    });
}
function GetCTS010_16_SectionData() {
    var flag = false;
    if ($("#DevideContractFeeBillingFlag").prop("checked") == true)
        flag = true;

    return {
        DivideContractFeeBillingFlag: flag,
        IsBillingEditMode: isBillingSelected
    };
}
function SetCTS010_16_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#divBillingTargetInfo").SetViewMode(true);

        if (gridBillingTarget != undefined) {
            var detailCol = gridBillingTarget.getColIndexById("Edit");
            var removeCol = gridBillingTarget.getColIndexById("Remove");

            gridBillingTarget.setColumnHidden(detailCol, true);
            gridBillingTarget.setColumnHidden(removeCol, true);
            gridBillingTarget.setSizes();
        }
    }
    else {
        $("#divBillingTargetInfo").SetViewMode(false);

        if (gridBillingTarget != undefined) {
            var detailCol = gridBillingTarget.getColIndexById("Edit");
            var removeCol = gridBillingTarget.getColIndexById("Remove");

            gridBillingTarget.setColumnHidden(detailCol, false);
            gridBillingTarget.setColumnHidden(removeCol, false);

            for (var i = 0; i < gridBillingTarget.getRowsNum(); i++) {
                gridBillingTarget.setColspan(gridBillingTarget.getRowId(i), removeCol, 2);
            }
        }
    }
}
function SetCTS010_16_EnableSection(enable) {
    $("#divBillingTargetDetail").hide();
    billing_target_mode = 0;

    /* --- Enable Button --- */
    /* --------------------- */
    $("#gridBillingTarget").find("img").each(function () {
        if (this.id != "" && this.id != undefined) {
            if (this.id.indexOf(btnBillingTargetDetail) == 0) {
                var row_id = GetGridRowIDFromControl(this);
                EnableGridButton(gridBillingTarget, btnBillingTargetDetail, row_id, "Edit", enable);
            }
            else if (this.id.indexOf(btnRemoveBillingTargetDetail) == 0) {
                var row_id = GetGridRowIDFromControl(this);
                EnableGridButton(gridBillingTarget, btnRemoveBillingTargetDetail, row_id, "Remove", enable);
            }
        }
    });
    /* --------------------- */

    if (enable) {
        $("#DevideContractFeeBillingFlag").removeAttr("disabled");
        $("#btnNewBillingTargetDetail").removeAttr("disabled");
    }
    else {
        $("#DevideContractFeeBillingFlag").attr("disabled", true);
        $("#btnNewBillingTargetDetail").attr("disabled", true);
    }

}





/* --- Initial -------------------------------------------------------- */
/* -------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "BillingClientCode"
    ]);

    $("#TotalContractFee").BindNumericBox(13, 2, 0, 9999999999999.99);
    $("#TotalInstallationFee").BindNumericBox(13, 2, 0, 9999999999999.99);
    $("#TotalDepositFee").BindNumericBox(13, 2, 0, 9999999999999.99);

    InitialBillingTargetEvents();
    InitialBillingTargetDetail();
    InitialBillingTargetDetailEvents();
});


function InitialBillingTargetEvents() {
    $("#rdoBillingTargetCode").change(SpecifyBillingTargetChanged);
    $("#rdoBillingClientCode").change(SpecifyBillingTargetChanged);
    SpecifyBillingTargetChanged();
}

function InitialBillingTargetDetail() {
    $("#ContractFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#InstallFee_Approval").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#InstallFee_CompleteInstallation").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#InstallFee_StartService").BindNumericBox(12, 2, 0, 999999999999.99);
    //$("#TotalFee").BindNumericBox(10, 2, 0, 9999999999.99);
    $("#DepositFee").BindNumericBox(12, 2, 0, 999999999999.99);
}
function InitialBillingTargetDetailEvents() {
    $("#btnRetrieveBillingTargetCode").click(retrieve_billing_target_code_click);
    $("#btnRetrieveBillingClientCode").click(retrieve_billing_client_code_click);

    $("#btnSearchBillingClient").click(search_billing_client_click);
    $("#btnCopyBilling").click(copy_billing_click);
    $("#btnNewEditBillingTarget").click(NewEdit_BillingCustomerData);
    $("#btnAddUpdateBillingTargetdetail").click(add_update_billing_target_detail_click);
    $("#btnCancelBillingTargetDetail").click(cancel_billing_target_detail_click);

    $("#InstallFee_Approval").blur(SummaryBillingFee);
    $("#InstallFee_Approval").NumericCurrency().change(SummaryBillingFee);
    $("#InstallFee_CompleteInstallation").blur(SummaryBillingFee);
    $("#InstallFee_CompleteInstallation").NumericCurrency().change(SummaryBillingFee);
    $("#InstallFee_StartService").blur(SummaryBillingFee);
    $("#InstallFee_StartService").NumericCurrency().change(SummaryBillingFee);

    $("#btnClearBillingTarget").click(clear_billing_target_click);
}

/* -------------------------------------------------------------------- */

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

    call_ajax_method_json("/Contract/CTS010_RetrieveBillingTargetDetailData", obj, function (result, controls) {
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
            DisplayDocLanguage(); //Add by Jutarat A. on 18122013

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

    call_ajax_method_json("/Contract/CTS010_RetrieveBillingClientDetailData", obj, function (result, controls) {
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
            DisplayDocLanguage(); //Add by Jutarat A. on 18122013

            $("#divSpecifyBillingCode").SetEnableView(false);
            $("#divCopyBillingNameAddress").SetEnableView(false);
        }
    });
}

function search_billing_client_click() {
    $("#dlgBox").OpenCMS270Dialog("CTS010");
}
function CMS270Response(object) {
    $("#dlgBox").CloseDialog();

    if (object != undefined) {
        var obj = {
            BillingClientCode: object.BillingClientCode
        };

        $("#divBillingClientInformation").clearForm();
        $("#BillingOfficeCode").removeAttr("disabled");

        call_ajax_method_json("/Contract/CTS010_RetrieveBillingClientDetailData", obj, function (result, controls) {
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

                $("#BillingTargetCode").val("");
                $("#BillingClientCode").val("");

                if ($("#BillingTargetCodeShort").val() != "") {
                    $("#BillingOfficeCode").attr("disabled", true);
                    $("#BillingOfficeCode").val(result.BillingOfficeCode);
                }
                DisplayDocLanguage(); //Add by Jutarat A. on 18122013

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

    call_ajax_method_json("/Contract/CTS010_CopyBillingNameAddressData", obj, function (result, controls) {
        if (result != undefined) {
            $("#divBillingClientInformation").bindJSON_Prefix("Billing", result.BillingClient);
            $("#BillingTargetCodeShort").val(result.BillingTargetCodeShort);
            $("#BillingClientCodeShort").val(result.BillingClientCodeShort);
            
            if ($("#BillingTargetCodeShort").val() != "") {
                $("#BillingOfficeCode").attr("disabled", true);
            }
            DisplayDocLanguage(); //Add by Jutarat A. on 18122013

            $("#divSpecifyBillingCode").SetEnableView(false);
            $("#divCopyBillingNameAddress").SetEnableView(false);
        }
    });
}
function clear_billing_target_click() {
    call_ajax_method_json("/Contract/CTS010_ClearTempBillingTargetDetailData", "", function () {
        $("#divBillingClientInformation").clearForm();
        $("#BillingOfficeCode").removeAttr("disabled");
        $("#divSpecifyBillingCode").SetEnableView(true);
        $("#divCopyBillingNameAddress").SetEnableView(true);
        SpecifyBillingTargetChanged();

        DisplayDocLanguage(); //Add by Jutarat A. on 18122013
    });
}
function cancel_billing_target_detail_click() {
    call_ajax_method_json("/Contract/CTS010_CancelBillingTargetDetail", null, function (result, controls) {
        $("#divBillingTargetDetail").hide();

        /* --- Enable Button --- */
        /* --------------------- */
        $("#gridBillingTarget").find("img").each(function () {
            if (this.id != "" && this.id != undefined) {
                if (this.id.indexOf(btnBillingTargetDetail) == 0) {
                    var row_id = GetGridRowIDFromControl(this);
                    EnableGridButton(gridBillingTarget, btnBillingTargetDetail, row_id, "Edit", true);
                }
                else if (this.id.indexOf(btnRemoveBillingTargetDetail) == 0) {
                    var row_id = GetGridRowIDFromControl(this);
                    EnableGridButton(gridBillingTarget, btnRemoveBillingTargetDetail, row_id, "Remove", true);
                }
            }
        });
        $("#btnNewBillingTargetDetail").removeAttr("disabled");
        /* --------------------- */

        billing_target_mode = 0;
        isBillingSelected = false;
    });
}
function add_update_billing_target_detail_click() {
    var obj = CreateObjectData($("#formBillingTargetDetail").serialize());
    if (obj != undefined) {
        obj.BillingTargetCode = $("#SelectedBillingTargetCode").val();
        obj.BillingClientCode = $("#SelectedBillingClientCode").val();
        obj.BillingOfficeCode = $("#BillingOfficeCode").val();
        obj.DocLanguage = $("#DocLanguage").val(); //Add by Jutarat A. on 18122013
        obj.UpdateModeID = billing_target_mode;
        obj.RowIndex = gridBillingTarget.getRowIndex(gridBillingTarget.getSelectedId());

        obj.ContractFeeCurrencyType = $("#ContractFee").NumericCurrencyValue();
        obj.InstallFee_ApprovalCurrencyType = $("#InstallFee_Approval").NumericCurrencyValue();
        obj.InstallFee_CompleteInstallationCurrencyType = $("#InstallFee_CompleteInstallation").NumericCurrencyValue();
        obj.InstallFee_StartServiceCurrencyType = $("#InstallFee_StartService").NumericCurrencyValue();
        obj.DepositFeeCurrencyType = $("#DepositFee").NumericCurrencyValue();
        
        if (obj.ContractFeeCurrencyType == C_CURRENCY_US) {
            obj.ContractFeeUsd = obj.ContractFee;
            obj.ContractFee = null;
        }
        if (obj.InstallFee_ApprovalCurrencyType == C_CURRENCY_US) {
            obj.InstallFee_ApprovalUsd = obj.InstallFee_Approval;
            obj.InstallFee_Approval = null;
        }
        if (obj.InstallFee_CompleteInstallationCurrencyType == C_CURRENCY_US) {
            obj.InstallFee_CompleteInstallationUsd = obj.InstallFee_CompleteInstallation;
            obj.InstallFee_CompleteInstallation = null;
        }
        if (obj.InstallFee_StartServiceCurrencyType == C_CURRENCY_US) {
            obj.InstallFee_StartServiceUsd = obj.InstallFee_StartService;
            obj.InstallFee_StartService = null;
        }
        if (obj.DepositFeeCurrencyType == C_CURRENCY_US) {
            obj.DepositFeeUsd = obj.DepositFee;
            obj.DepositFee = null;
        }
    }
    
    call_ajax_method_json("/Contract/CTS010_UpdateBillingTargetDetail", obj, function (result, controls) {
        if (controls != undefined) {
            for (var idx = 0; idx < controls.length; idx++) {
                if (controls[idx] == "ALLFee") {
                    controls.push("ContractFee");
                    controls.push("InstallFee_Approval");
                    controls.push("InstallFee_CompleteInstallation");
                    controls.push("InstallFee_StartService");
                    controls.push("DepositFee");
                    break;
                }
            }

            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["BillingOfficeCode",
                            "ContractFee",
                            "InstallFee_Approval",
                            "PaymentMethod_Approval",
                            "InstallFee_CompleteInstallation",
                            "PaymentMethod_CompleteInstallation",
                            "InstallFee_StartService",
                            "PaymentMethod_StartService",
                            "DepositFee",
                            "PaymentMethod_Deposit"], controls);
            /* --------------------- */

            return;
        }
        if (result != undefined) {
            $("#divBillingTargetDetail").hide();
            billing_target_mode = 0;
            isBillingSelected = false;

            $("#gridBillingTarget").LoadDataToGrid(gridBillingTarget, 0, false,
                                "/Contract/CTS010_GetBillingTargetList",
                                "",
                                "CTS010_BillingTargetData", false,
                                function () {
                                    /* --- Enable Button --- */
                                    /* --------------------- */
                                    $("#btnNewBillingTargetDetail").removeAttr("disabled");
                                    /* --------------------- */

                                    $("#TotalContractFee").val(result.TotalContractFee);
                                    $("#TotalContractFeeUsd").val(result.TotalContractFeeUsd);
                                    $("#TotalInstallationFee").val(result.TotalInstallationFee);
                                    $("#TotalInstallationFeeUsd").val(result.TotalInstallationFeeUsd);
                                    $("#TotalDepositFee").val(result.TotalDepositFee);
                                    $("#TotalDepositFeeUsd").val(result.TotalDepositFeeUsd);
                                });
        }
    });
}

/* -------------------------------------------------------------------- */

function SummaryBillingFee() {
    var total = 0;
    var totalUS = 0;
    var allNull = true;

    var installFee_Approval = parseFloat($("#InstallFee_Approval").NumericValue());
    if (isNaN(installFee_Approval) == false) {
        if ($("#InstallFee_Approval").NumericCurrencyValue() == C_CURRENCY_LOCAL) {
            total += installFee_Approval;
        }
        else {
            totalUS += installFee_Approval;
        }

        allNull = false;
    }

    var installFee_CompleteInstallation = parseFloat($("#InstallFee_CompleteInstallation").NumericValue());
    if (isNaN(installFee_CompleteInstallation) == false) {
        if ($("#InstallFee_CompleteInstallation").NumericCurrencyValue() == C_CURRENCY_LOCAL) {
            total += installFee_CompleteInstallation;
        }
        else {
            totalUS += installFee_CompleteInstallation;
        }

        allNull = false;
    }

    var installFee_StartService = parseFloat($("#InstallFee_StartService").NumericValue());
    if (isNaN(installFee_StartService) == false) {
        if ($("#InstallFee_StartService").NumericCurrencyValue() == C_CURRENCY_LOCAL) {
            total += installFee_StartService;
        }
        else {
            totalUS += installFee_StartService;
        }

        allNull = false;
    }

    if (allNull) {
        $("#TotalFee").val("");
        $("#TotalFeeUsd").val("");
    }
    else {
        $("#TotalFee").val(SetNumericValue(total, 2));
        $("#TotalFee").setComma();

        $("#TotalFeeUsd").val(SetNumericValue(totalUS, 2));
        $("#TotalFeeUsd").setComma();
    }
}


/* --- Dialog Methods (MAS030) ------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
var doBillingCustomer = null;
function NewEdit_BillingCustomerData() {
    doBillingCustomer = null;
    call_ajax_method_json("/Contract/CTS010_GetTempBillingClientData", "", function (result) {
        if (result != undefined)
            doBillingCustomer = result.BillingClient;
        $("#dlgBox").OpenMAS030Dialog("CTS010");
    });
}
function MAS030Object() {
    return doBillingCustomer;
}
function MAS030Response(res) {
    $("#dlgBox").CloseDialog();

    call_ajax_method_json("/Contract/CTS010_UpdateTempBillingClientData", res, function (result) {
        $("#divBillingClientInformation").bindJSON_Prefix("Billing", result.BillingClient);
        $("#BillingTargetCodeShort").val(result.BillingTargetCodeShort);
        $("#BillingClientCodeShort").val(result.BillingClientCodeShort);

        if ($("#BillingTargetCodeShort").val() != "") {
            $("#BillingOfficeCode").attr("disabled", true);

            $("#lblInvoiceLanguage").hide(); //Add by Jutarat A. on 18122013
            $("#DocLanguage").hide(); //Add by Jutarat A. on 18122013
        }
        //Add by Jutarat A. on 18122013
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

//Add by Jutarat A. on 18122013
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