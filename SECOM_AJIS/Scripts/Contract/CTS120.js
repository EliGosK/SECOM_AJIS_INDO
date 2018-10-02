// ---------------------------------------------------------------------------------
// Initial
// ---------------------------------------------------------------------------------
var isFromParameter = false;

$(document).ready(function () {
    // Set to initial
    InitialScreen();
    InitialEvent();

    // Check passing parameter
    CheckingParameter();
});

function InitialScreen() {
    SetScreenToDefault();
}

function InitialEvent() {
    $("#btnRetrieve").click(retrieve_Click);
    $("#btnClear").click(function () {
        call_ajax_method_json("/Contract/CTS120_ClearContract", "", function (result, controls) {
            SetScreenToDefault();

            SEARCH_CONDITION = null;
        });
    });
}
// ---------------------------------------------------------------------------------
// Event
// ---------------------------------------------------------------------------------
function command_ok_click() {

    DisableOKCommand(true);
    DisableResetCommand(true);

    var obj = {
        strContractCode: $('#EntryContractCode').val()
    };

    call_ajax_method_json("/Contract/CTS120_CompleteCacelContract", obj, function (result, controls) {
        if (controls != undefined) {
            // Hilight Text
            //VaridateCtrl(["EntryContractCode"], ["EntryContractCode"]);
        }

        if (result) {
            var obj = {
                module: "Common",
                code: "MSG0046"
            };

            call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                OpenInformationMessageDialog(result.Code, result.Message, complete_ok_click);
            });
        } else {
            DisableOKCommand(false);
            DisableResetCommand(false);
        }
    });
}

function command_reset_click() {
    DisableOKCommand(true);
    DisableResetCommand(true);

    // Get Message
    var obj = {
        module: "Common",
        code: "MSG0038"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message, confirm_yes_click, function () {
            DisableOKCommand(false);
            DisableResetCommand(false);
        });

    });
}

function confirm_yes_click() {
    SetScreenToDefault();
    CheckingParameter();
}

function complete_ok_click() {
    if (isFromParameter) {
        window.location.href = generate_url("/Common/CMS020");
    } else {
        SetScreenToDefault();
    }
}

function retrieve_Click() {
    $('#EntryContractCode').removeClass("highlight");

    var obj = {
        strContractCode: $('#EntryContractCode').val()
    };

    call_ajax_method_json("/Contract/CTS120_RetrieveRentalContractData", obj, function (result, controls) {
        if (controls != undefined) {
            // Hilight Text
            //VaridateCtrl(["EntryContractCode"], ["EntryContractCode"]);
            VaridateCtrl(["EntryContractCode"], controls);
            return;
        }

        if (result != null) {
            SetScreenToView(result);

            /* --- Set condition --- */
            SEARCH_CONDITION = {
                ContractCode: obj.strContractCode
            };
            /* --------------------- */
        }
    });
}
// ---------------------------------------------------------------------------------
// Function
// ---------------------------------------------------------------------------------
function SetScreenToDefault() {
    // Set View
    EnableSpecifyContractCodeSection();
    HideDetailSection();
    HideRemarkSection();

    // Clear Contract Code Textbox
    $('#EntryContractCode').removeClass("highlight");
    $('#EntryContractCode').val('');

    // Set Footer Button
    DisableFooterButton();
}

function SetScreenToView(contractData) {
    // Set View
    $("#divContractBasicInformation").clearForm();
    $("#divContractBasicInformation").bindJSON(contractData);
    ShowDetailSection();

    // Set Footer Button
    if (contractData.CanContinue) {
        EnableFooterButton();
        DisableRegisterCommand(false);
        DisableSpecifyContractCodeSection();
        ShowRemarkSection();
    } else {
        DisableFooterButton();
        DisableRegisterCommand(true);
        EnableSpecifyContractCodeSection();
        HideRemarkSection();
    }
}

function ShowRemarkSection() {
    $('#divCompleteCancelContract').show();
}

function ShowDetailSection() {
    $('#divContractBasicInformation').show();
}

function HideRemarkSection() {
    $('#divCompleteCancelContract').hide();
}

function HideDetailSection() {
    $('#divContractBasicInformation').hide();
}

function EnableSpecifyContractCodeSection() {
    $('#EntryContractCode').removeAttr("readonly");
    $('#btnRetrieve').removeAttr("disabled");
    //$('#btnClear').removeAttr("disabled");
}

function DisableSpecifyContractCodeSection() {
    $("#EntryContractCode").attr("readonly", true);
    $("#btnRetrieve").attr("disabled", true);
    //$("#btnClear").attr("disabled", true);
}

function EnableFooterButton() {
    SetOKCommand(true, command_ok_click);
    SetResetCommand(true, command_reset_click);
}

function DisableFooterButton() {
    SetOKCommand(false, null);
    SetResetCommand(false, null);
}

function CheckingParameter() {
    call_ajax_method_json("/Contract/CTS120_CheckInitialParameter", "", function (result, controls) {
        // Do something
        if (result != null) {
            $('#EntryContractCode').val(result);
            retrieve_Click();
            isFromParameter = true;
        }
    });
}
// ---------------------------------------------------------------------------------