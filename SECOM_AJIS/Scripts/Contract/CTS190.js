
/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>

/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

var CTS190_RecieveContacrDocList;
var CTS190_RowPerPage = 20;
var CTS190_Arr = [];


$(document).ready(function () {


    initialPage();

    // Grid
    if ($.find("#CTS190_RecieveContacrDocList").length) {
        CTS190_RecieveContacrDocList = $("#CTS190_RecieveContacrDocList").InitialGrid(CTS190_RowPerPage, false, "/Contract/CTS190_IntialGridRecieveContractDoc");
    }

    SpecialGridControl(CTS190_RecieveContacrDocList, ["Button"]);

    // remove_row

    /* ===== binding event when finish load data ===== */
    BindOnLoadedEvent(CTS190_RecieveContacrDocList, function () {
        var colInx = CTS190_RecieveContacrDocList.getColIndexById('Button');
        for (var i = 0; i < CTS190_RecieveContacrDocList.getRowsNum(); i++) {
            var rowId = CTS190_RecieveContacrDocList.getRowId(i);
            GenerateRemoveButton(CTS190_RecieveContacrDocList, "btnRemove", rowId, "Button", true);

            // binding grid button event 
            BindGridButtonClickEvent("btnRemove", rowId, remove_row);

        }
        CTS190_RecieveContacrDocList.setSizes();
    });



    // Button
    $("#btnCTS190Add").click(function () {
        // ADD

        // Disable Add button
        $("#btnCTS190Add").attr("disabled", true);
        master_event.LockWindow(true);

        var keyValue = $.trim($("#ContractCode_QuotationTargetCode").val()) + "-"
                                     + $.trim($("#OCC_Alphabet").val()) + "-"
                                      + $.trim($("#ContractDocOCC").val());

        keyValue = keyValue.toUpperCase();

        var param = CreateObjectData($("#CTS190_RecieveForm").serialize());

        call_ajax_method_json("/Contract/CTS190_AddReceivedContractDoc", param, function (result, controls) {

            // Enable Add button
            $("#btnCTS190Add").attr("disabled", false);
            master_event.LockWindow(false);

            if (controls != undefined) {
                VaridateCtrl(["ContractCode_QuotationTargetCode", "OCC_Alphabet", "DocAuditResult", "ContractDocOCC"], controls);
            }
            else if (result != undefined) {

                // Check exist

                var foundIndex = -1;

                if (CTS190_Arr.length > 0) {
                    if (result.IsContractFlag == true) {
                        foundIndex = search_array_index(CTS190_Arr, "my_checked_id_by_ct", result.my_checked_id_by_ct);
                    }
                    else {
                        foundIndex = search_array_index(CTS190_Arr, "my_checked_id_by_quo", result.my_checked_id_by_quo);
                    }
                }


                if (foundIndex >= 0) { // found !!! -> dupplicate
                    // Data is exist in grid already. so not insert into grid



                    var param = { "module": "Contract", "code": "MSG3217" };
                    call_ajax_method("/Shared/GetMessage", param, function (data) {

                        /* ====== Open warning dialog =====*/
                        //OpenWarningMessageDialog(data.Code, data.Message);
                        OpenErrorMessageDialog(data.Code, data.Message);

                        //CTS190_RecieveContacrDocList.selectRow(foundIndex);
                    });
                }
                else {

                    // insert into grid

                    CheckFirstRowIsEmpty(CTS190_RecieveContacrDocList, true);

                    inserted_arr = [
                        result.ContractCode_QuotationTargetCode,
                        result.OCC_Alphabet,
                        result.ContractDocOCC,
                        result.DocAuditResultCodeName,
                        result.DocumentName,
                        result.my_checked_id_by_ct,
                        result.my_checked_id_by_quo,
                        result.my_checked_id,
                        result.IsContractFlag,
                        "" // button
                    ];

                    AddNewRow(CTS190_RecieveContacrDocList, inserted_arr);
                    CTS190_RecieveContacrDocList.setSizes();

                    CTS190_Arr.push(result);

                    // Disable / Enable Register button
                    var bShow = (CTS190_Arr.length > 0);

                    SetResetCommand(bShow, command_reset_click);
                    SetRegisterCommand(bShow, command_register_click);

                    $("#CTS190_RecieveForm").clearForm();

                    $("#ContractCode_QuotationTargetCode").focus();

                }


            }
        });



    });

    $("#btnCTS190Clear").click(function () {
        // Clear

        $("#CTS190_RecieveForm").clearForm();
        CloseWarningDialog();

        $("#ContractCode_QuotationTargetCode").focus();

    });


    InitialTrimTextEvent([
        "ContractCode_QuotationTargetCode",
        "OCC_Alphabet",
        "ContractDocOCC"
    ]);



});

function initialPage() {
    SetResetCommand(false, command_reset_click);
    SetRegisterCommand(false, command_register_click);

    $("#ContractCode_QuotationTargetCode").focus();

}

function remove_row(row_id) {
    if (CTS190_Arr.length > 0) {

        var colIndex = CTS190_RecieveContacrDocList.getColIndexById("my_checked_id");
        var my_checked_id = CTS190_RecieveContacrDocList.cells(row_id, colIndex).getValue();

        if (CTS190_Arr[0].my_checked_id != undefined) {

            // Remove from array
            var removedIdx = search_array_index(CTS190_Arr, "my_checked_id", my_checked_id);
            if (removedIdx >= 0) {
                CTS190_Arr.splice(removedIdx, 1);
            }

            // Remove from grid
            DeleteRow(CTS190_RecieveContacrDocList, row_id);
            CTS190_RecieveContacrDocList.setSizes();

            // Disable / Enable Register button
            var bShow = (CTS190_Arr.length > 0);

            SetResetCommand(bShow, command_reset_click);
            SetRegisterCommand(bShow, command_register_click);

            $("#ContractCode_QuotationTargetCode").focus();
        }
    }

}

function command_reset_click() {
    var param = { "module": "Common", "code": "MSG0038" };
    call_ajax_method_json("/Shared/GetMessage", param, function (data) {

        /* ====== Open confirm dialog =====*/
        OpenYesNoMessageDialog(data.Code, data.Message, function () {

            resest();

        });
    });
}

function resest() {
    $("#CTS190_RecieveForm").clearForm();
    DeleteAllRow(CTS190_RecieveContacrDocList);

    // Clear array
    CTS190_Arr = [];
    // Disable Register , Reset button
    SetResetCommand(false, command_reset_click);
    SetRegisterCommand(false, command_register_click);

    $("#ContractCode_QuotationTargetCode").focus();
}



function command_register_click() {

    // Disable Register button
    DisableRegisterCommand(true);

    // CTS190_Arr
    call_ajax_method_json("/Contract/CTS190_SaveContractDocumentRecieving", CTS190_Arr, register);
}

function register(result) {

    if (result != undefined) {

        if (result == 2) {

            // Show message "Save completely"
            var param = { "module": "Common", "code": "MSG0046" };
            call_ajax_method_json("/Shared/GetMessage", param, function (data) {
                OpenInformationMessageDialog(data.Code, data.Message, function () {

                    resest();

                });
            });

        } else if (result == 1) {
            resest();
        }

    }
    else { // error
        DisableRegisterCommand(false);
    }


}
