/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */

$(document).ready(function () {
    SetInitialStateCTS130_08(true);
    InitialEventCTS130_08();
});

function SetInitialStateCTS130_08(isInitGrid) {
    ClearDataCTS130_8(true);
    SetReadonlySpecifyCodeCTS130_08(true);
    SetReadonlyCopyNameCTS130_08(true);

    if (isInitGrid) {
        InitialBillingTargetGridData();
    }

    $("#divBillingTargetDetailSection").hide();
}

function SetReadonlySpecifyCodeCTS130_08(isReadonly) {
    $("#BT_BillingTargetCode").attr("readonly", isReadonly);
    $("#BT_BillingClientCode").attr("readonly", isReadonly);
    $("#BT_rdoTargetCode").attr("disabled", isReadonly);
    $("#BT_rdoClientCode").attr("disabled", isReadonly);


    if (isReadonly == false) {
        $("#BT_btnRetrieveBillingTarget").attr("disabled", false);
        $("#BT_btnRetrieveBillingClient").attr("disabled", true);
    }
    else {
        $("#BT_btnRetrieveBillingTarget").attr("disabled", isReadonly);
        $("#BT_btnRetrieveBillingClient").attr("disabled", isReadonly);
    }
    

    $("#BT_btnSearchBillingClient").attr("disabled", isReadonly);
}

function SetReadonlyCopyNameCTS130_08(isReadonly) {
    $("#BT_rdoContractTarget").attr("disabled", isReadonly);
    $("#BT_rdoBranchOfContractTarget").attr("disabled", isReadonly);
    $("#BT_rdoRealCustomer").attr("disabled", isReadonly);
    $("#BT_rdoSite").attr("disabled", isReadonly);
    $("#BT_btnCopy").attr("disabled", isReadonly);
}

function InitialEventCTS130_08() {
    $("#BT_btnRetrieveBillingTarget").click(retrieve_billing_target_button_click_CTS130_08);
    $("#BT_btnRetrieveBillingClient").click(retrieve_billing_client_button_click_CTS130_08);

    $("#BT_btnSearchBillingClient").click(search_button_click_CTS130_08);
    $("#BT_btnCopy").click(copy_button_click_CTS130_08);
    $("#BT_btnNewEdit").click(newedit_button_click_CTS130_08);
    $("#BT_btnClearBillingTarget").click(clear_button_click_CTS130_08);

    $("#BT_rdoTargetCode").click(target_code_click_CTS130_08);
    $("#BT_rdoClientCode").click(client_code_click_CTS130_08);
    $("#BT_btnUpdate").click(update_button_click_CTS130_08);
    $("#BT_btnCancelBilling").click(cancel_billing_button_click_CTS130_08);
}

function InitialBillingTargetGridData() {
    gridBillingTargetCTS130 = $("#gridBillingTarget").LoadDataToGridWithInitial(0, false, false, "/Contract/CTS130_GetBillingTargetListData",
    "", "CTS110_RemovalInstallationFeeGridData", false);

    SpecialGridControl(gridBillingTargetCTS130, ["Detail"]);

    BindOnLoadedEvent(gridBillingTargetCTS130,
        function (gen_ctrl) {
            gridBillingTargetCTS130.setColumnHidden(gridBillingTargetCTS130.getColIndexById('Sequence'), true);

            BindGridRemovalFeeBilling(true, gen_ctrl);
        }
    );
}

function BindGridRemovalFeeBilling(isEnabled, gen_ctrl) {
    var row_id;
    var billingOCC = "";
    for (var i = 0; i < gridBillingTargetCTS130.getRowsNum(); i++) {
        row_id = gridBillingTargetCTS130.getRowId(i);

        var detailColinx = gridBillingTargetCTS130.getColIndexById('Detail');

        if (gen_ctrl == true) {
            GenerateEditButton(gridBillingTargetCTS130, "btnDetail", row_id, "Detail", isEnabled);
        }

        BindGridButtonClickEvent("btnDetail", row_id,
            function (row_id) {
                GetBillingTargetDetail(row_id);
            }
        );
    }

    gridBillingTargetCTS130.setSizes();
}

function GetBillingTargetDetail(row_id) {
    gridBillingTargetCTS130.selectRow(gridBillingTargetCTS130.getRowIndex(row_id));

    var seqCol = gridBillingTargetCTS130.getColIndexById("Sequence");
    var seq = gridBillingTargetCTS130.cells(row_id, seqCol).getValue();

    var obj = { strSequence: seq };
    call_ajax_method_json("/Contract/CTS130_GetBillingTargetDetail", obj,
        function (result) {
            if (result != undefined) {
                BindGridRemovalFeeBilling(false, true);

                BindDataCTS130_08(result);
                SetBillingTargetDetailMode();
            }
        }
    );
}

function SetBillingTargetDetailMode() {
    $("#divBillingTargetDetailSection").show();

//    SetReadonlySpecifyCodeCTS130_08(true);
//    SetReadonlyCopyNameCTS130_08(true);
    DisableSectionAfterBindBillingDetail();
}

function retrieve_billing_target_button_click_CTS130_08() {
    var obj = {
        strBillingTargetCode: $("#BT_BillingTargetCode").val()
    };

    call_ajax_method_json("/Contract/CTS130_RetrieveBillingTargetData", obj,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["BT_BillingTargetCode"], controls);
            }
            if (result != undefined) {
                BindDataCTS130_08(result);
                DisableSectionAfterBindBillingDetail();
            }
        }
    );
}
function retrieve_billing_client_button_click_CTS130_08() {
    var obj = {
        strBillingClientCode: $("#BT_BillingClientCode").val()
    };

    call_ajax_method_json("/Contract/CTS130_RetrieveBillingClientData", obj,
    function (result, controls) {
        if (controls != undefined) {
            VaridateCtrl(["BT_BillingClientCode"], controls);
        }
        if (result != undefined) {
            BindDataCTS130_08(result);
            DisableSectionAfterBindBillingDetail();
        }
    }
    );
}

function BindDataCTS130_08(result) {
    var billingClient;
    var billingTarget;

    if (result != undefined) {
        if (result.length == 2) {
            billingClient = result[0];
            billingTarget = result[1];
        }
        else {
            billingClient = result;
        }
    }

    if (billingClient != undefined) {
        $("#BT_BillingClientCodeDetail").val(billingClient.BillingClientCodeShort);
        $("#BT_FullNameEN").val(billingClient.FullNameEN);
        $("#BT_BranchNameEN").val(billingClient.BranchNameEN);
        $("#BT_AddressEN").val(billingClient.AddressEN);
        $("#BT_FullNameLC").val(billingClient.FullNameLC);
        $("#BT_BranchNameLC").val(billingClient.BranchNameLC);
        $("#BT_AddressLC").val(billingClient.AddressLC);
        $("#BT_Nationality").val(billingClient.Nationality);
        $("#BT_PhoneNo").val(billingClient.PhoneNo);
        $("#BT_BusinessType").val(billingClient.BusinessTypeName);
        $("#BT_IDNo").val(billingClient.IDNo);
    }

    if (billingTarget != undefined) {
        $("#BT_BillingTargetCodeDetail").val(billingTarget.BillingTargetCodeShort);
        $("#BT_BillingOffice").val(billingTarget.BillingOfficeCode);
    }

}

function DisableSectionAfterBindBillingDetail() {
    SetReadonlySpecifyCodeCTS130_08(true);
    SetReadonlyCopyNameCTS130_08(true);
    //$("#BT_BillingOffice").attr("disabled", $("#BT_BillingTargetCode").val() != "");
    $("#BT_BillingOffice").attr("disabled", $("#BT_BillingTargetCodeDetail").val() != "");
}

function search_button_click_CTS130_08() {
    $("#dlgCTS130").OpenCMS270Dialog();
}

function CMS270Response(result) {
    var obj = { billingClientData: result };
    $("#dlgCTS130").CloseDialog();

    call_ajax_method_json("/Contract/CTS130_SearchBillingClient", obj,
        function (result, controls) {
            if (result != undefined) {
                BindDataCTS130_08(result);
                DisableSectionAfterBindBillingDetail();
            }
        }
    );

}

function copy_button_click_CTS130_08() {
    var copyType = "0";
    if ($("#BT_rdoContractTarget").prop("checked")) {
        copyType = $("#BT_rdoContractTarget").val();
    }
    else if ($("#BT_rdoBranchOfContractTarget").prop("checked")) {
        copyType = $("#BT_rdoBranchOfContractTarget").val();
    }
    else if ($("#BT_rdoRealCustomer").prop("checked")) {
        copyType = $("#BT_rdoRealCustomer").val();
    }
    else if ($("#BT_rdoSite").prop("checked")) {
        copyType = $("#BT_rdoSite").val();
    }

    var obj = {
        CopyType: copyType,
        PurchaserCustCode: $("#PC_CustomerCode").val(),
        RealCustCode: $("#RC_CustomerCode").val(),
        SiteCode: $("#ST_SiteCode").val(),
        BranchNameEN: $("#PC_BranchNameEnglish").val(),
        BranchNameLC: $("#PC_BranchNameLocal").val(),
        BranchAddressEN: $("#PC_BranchAddressEnglish").val(),
        BranchAddressLC: $("#PC_BranchAddressLocal").val()
    };

    call_ajax_method_json("/Contract/CTS130_CopyBillingTarget", obj,
        function (result, controls) {
            if (result != undefined) {
                BindDataCTS130_08(result);
                DisableSectionAfterBindBillingDetail();
            }
        }
    );
}

/*------ MAS030 Dialog ------*/
/*---------------------------*/
var doBillingCustomerObject = null;
function newedit_button_click_CTS130_08() {
    call_ajax_method_json("/Contract/CTS130_GetTempBillingClientData", "",
        function (result) {
            //if (result != undefined) {
                doBillingCustomerObject = result;
            //}
            $("#dlgCTS130").OpenMAS030Dialog("CTS130");
        });
}
function MAS030Object() {
    return doBillingCustomerObject;
}
function MAS030Response(result) {
    var obj = { billingClientData: result };
    $("#dlgCTS130").CloseDialog();

    call_ajax_method_json("/Contract/CTS130_SearchBillingClient", obj,
        function (result, controls) {
            if (result != undefined) {
                BindDataCTS130_08(result);

                SetReadonlySpecifyCodeCTS130_08(false);
                SetReadonlyCopyNameCTS130_08(false);
                $("#BT_BillingClientCode").attr("readonly", true); //default

                $("#BT_BillingOffice").attr("disabled", false);
                $("#BT_btnUpdate").attr("disabled", false);
            }
        }
    );
}
/*--------------------------*/

function clear_button_click_CTS130_08() {
    call_ajax_method_json("/Contract/CTS130_ClearBillingTarget", "",
        function (result, controls) {
            if (result != undefined) {
                ClearDataCTS130_8(false);

                SetReadonlySpecifyCodeCTS130_08(false);
                SetReadonlyCopyNameCTS130_08(false);

                $("#BT_BillingClientCode").attr("readonly", true); //default
                $("#BT_rdoContractTarget").attr("readonly", true); //default
            }
        }
    );
}

function ClearDataCTS130_8(isClearBillingOffice) {
    //$("#divBillingTargetDetailSection").clearForm();
    $("#divSpecifyCodeAndCopyName").clearForm();
    $("#divBillingTargetDetail").clearForm();

    if ($("#BT_BillingOffice").prop("disabled") == false && isClearBillingOffice) {
        $("#BT_BillingOffice").val("");
    }

    $("#BT_rdoTargetCode").attr("checked", true)
    $("#BT_rdoContractTarget").attr("checked", true)
}

function target_code_click_CTS130_08() {
    $("#BT_BillingTargetCode").attr("readonly", false);
    $("#BT_BillingTargetCode").ResetToNormalControl();

    $("#BT_BillingClientCode").attr("readonly", true);
    $("#BT_BillingClientCode").val("");

    $("#BT_btnRetrieveBillingTarget").removeAttr("disabled");
    $("#BT_btnRetrieveBillingClient").attr("disabled", "disabled");
}

function client_code_click_CTS130_08() {
    $("#BT_BillingClientCode").attr("readonly", false);
    $("#BT_BillingClientCode").ResetToNormalControl();

    $("#BT_BillingTargetCode").attr("readonly", true);
    $("#BT_BillingTargetCode").val("");

    $("#BT_btnRetrieveBillingTarget").attr("disabled", "disabled");
    $("#BT_btnRetrieveBillingClient").removeAttr("disabled");
}

function update_button_click_CTS130_08() {
    var obj = { strBillingOffice: $("#BT_BillingOffice").val() };

    call_ajax_method_json("/Contract/CTS130_UpdateBillingTargetDetail", obj,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["BT_BillingOffice"], controls);
            }
            else if (result != undefined) {
                //                gridBillingTargetCTS130 = $("#gridBillingTarget").LoadDataToGridWithInitial(0, false, false, "/Contract/CTS130_RefreshBillingTargetDetail",
                //                                                "", "CTS110_RemovalInstallationFeeGridData", false);

                //                SpecialGridControl(gridBillingTargetCTS130, ["Detail"]);
                //                BindGridRemovalFeeBilling();

                $("#gridBillingTarget").LoadDataToGrid(gridBillingTargetCTS130, 0, false, "/Contract/CTS130_RefreshBillingTargetDetail",
                                                "", "CTS110_RemovalInstallationFeeGridData", false, null, null);

                BindGridRemovalFeeBilling(true, true);

                ClearDataCTS130_8(true);
                $("#divBillingTargetDetailSection").hide();
            }
        }
    );
}

function cancel_billing_button_click_CTS130_08() {
    ClearDataCTS130_8(true);

    $("#divBillingTargetDetailSection").hide();

    BindGridRemovalFeeBilling(true, true);
}

function SetSectionModeCTS130_08(isView) {
    $("#divBillingTargetSection").SetViewMode(isView);

    if (isView) {
        if (gridBillingTargetCTS130 != undefined) {
            var detailCol = gridBillingTargetCTS130.getColIndexById("Detail");
            gridBillingTargetCTS130.setColumnHidden(detailCol, true);
            gridBillingTargetCTS130.setSizes();

            $("#gridBillingTarget").hide();
            $("#gridBillingTarget").show();

            //$("#divBillingTargetDetailSection").hide();
        }
    }
    else {
        if (gridBillingTargetCTS130 != undefined) {
            var detailCol = gridBillingTargetCTS130.getColIndexById("Detail");
            gridBillingTargetCTS130.setColumnHidden(detailCol, false);
            gridBillingTargetCTS130.setSizes();

            $("#gridBillingTarget").hide();
            $("#gridBillingTarget").LoadDataToGrid(gridBillingTargetCTS130, 0, false, "/Contract/CTS130_RefreshBillingTargetDetail",
                                                "", "CTS110_RemovalInstallationFeeGridData", false, null, null);
            $("#gridBillingTarget").show();

            if ($("#BT_btnSearchBillingClient").prop("disabled")) {
                SetReadonlySpecifyCodeCTS130_08(true);
            }

            if ($("#BT_btnCopy").prop("disabled")) {
                SetReadonlyCopyNameCTS130_08(true);
            }
        }
    }
}
