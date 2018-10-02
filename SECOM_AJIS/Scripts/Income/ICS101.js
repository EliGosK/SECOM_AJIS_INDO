/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/control_events.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>

/// <reference path="../Base/object/ajax_method.js" />
/// <reference path="../Base/object/command_event.js" />
/// <reference path="../Base/object/master_event.js" />

var ICS101 = {
    pageRow: ROWS_PER_PAGE_FOR_SEARCHPAGE,
    grdMoneyCollectionManagementInformationGrid: null,
    bolViewMode: false,
    CommandMode: {
        Init: 0,
        View: 1,
        Edit: 2,
        Confirm: 9
    },
    TableMode: {
        No: 0,
        Yes: 1
    },

    InitialControl: function () {
        InitialDateFromToControl("#dtpExpectedCollectDateFrom", "#dtpExpectedCollectDateTo");
        ICS101.SetFormMode(ICS101.CommandMode.Init);
        ICS101.SetVisableTable(ICS101.TableMode.No);

        $("#btnSearch").click(ICS101.btn_Search_click);
        $("#btnClear").click(ICS101.btn_Clear_click);
        $("#btnDownloadFile").click(ICS101.btn_Download_File_click);
        $("#divResaultList").hide();

        /* --- Grid --------------------------- */
        ICS101.grdMoneyCollectionManagementInformationGrid =
            $("#ICS101_MoneyCollectionManagementInformationGrid").InitialGrid(ICS101.pageRow, true, "/Income/ICS101_InitialMoneyCollectionManagementInformationGrid");

        SpecialGridControl(ICS101.grdMoneyCollectionManagementInformationGrid, ["ICS101_MoneyCollectionManagementInformationGrid", "Del"]);

        var funcDrawControl = function (rid) {
            GenerateRemoveButton(ICS101.grdMoneyCollectionManagementInformationGrid, "btnRemove", rid, "Del", true);
            BindGridButtonClickEvent("btnRemove", rid, ICS101.btnRemoveByMoneyCollectionManagementInformationGrid);
        }
        BindOnLoadedEventV2(ICS101.grdMoneyCollectionManagementInformationGrid, ICS101.pageRow, true, true, funcDrawControl);
        /* ------------------------------------ */
    },

    /* --- Command mode ------------------------------- */
    SetFormMode: function (intMode) {
        if (intMode == ICS101.CommandMode.Init) {
            // ModeInit
            register_command.SetCommand(null);
            reset_command.SetCommand(null);
            approve_command.SetCommand(null);
            reject_command.SetCommand(null);
            return_command.SetCommand(null);
            close_command.SetCommand(null);
            confirm_command.SetCommand(null);
            back_command.SetCommand(null);
        }
//        else if (intMode == ICS101.CommandMode.View) {
//            // ModeView = 1;
//            register_command.SetCommand(ICS101.btn_Register_click);
//            reset_command.SetCommand(ICS101.btn_Reset_click);
//            approve_command.SetCommand(null);
//            reject_command.SetCommand(null);
//            return_command.SetCommand(null);
//            close_command.SetCommand(null);
//            confirm_command.SetCommand(null);
//            back_command.SetCommand(null);
//        }
//        else if (intMode == ICS101.CommandMode.Edit) {
//            // ModeEdit = 2;
//            register_command.SetCommand(null);
//            reset_command.SetCommand(null);
//            approve_command.SetCommand(null);
//            reject_command.SetCommand(null);
//            return_command.SetCommand(null);
//            close_command.SetCommand(null);
//            confirm_command.SetCommand(ICS101.btn_Confirm_click);
//            back_command.SetCommand(ICS101.btn_Back_click);
//        }
//        else if (intMode == ICS101.CommandMode.Confirm) {
//            // ModeConfirm = 9;
//            register_command.SetCommand(null);
//            reset_command.SetCommand(null);
//            approve_command.SetCommand(null);
//            reject_command.SetCommand(null);
//            return_command.SetCommand(null);
//            close_command.SetCommand(null);
//            confirm_command.SetCommand(ICS101.btn_Confirm_click);
//            back_command.SetCommand(ICS101.btn_Back_click);
//        }
    },
    /* ------------------------------------------------ */

    GetUserAdjustData: function () {
        var arr1 = new Array();
        var arr2 = new Array();

        var CollectionArea = new Array();
        $("#chklCollectionArea").find("input:checkbox").each(function () {
            if ($(this).prop("checked") == true) {
                CollectionArea.push($(this).val());
            }
        });

        var header = {
            dtpExpectedCollectDateFrom: $('#dtpExpectedCollectDateFrom').val(),
            dtpExpectedCollectDateTo: $('#dtpExpectedCollectDateTo').val(),
            chklCollectionArea: CollectionArea
        };

        if (CheckFirstRowIsEmpty(ICS101.grdMoneyCollectionManagementInformationGrid) == false) {

            for (var i = 0; i < ICS101.grdMoneyCollectionManagementInformationGrid.getRowsNum(); i++) {

                var row_id = ICS101.grdMoneyCollectionManagementInformationGrid.getRowId(i);

                // non custom object in grid use object name
                // use column name in XML not declare ass txt, dtp ect
                var obj1 = {
                    txtReceiptNo: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("ReceiptNo")).getValue(),
                    dtpReceiptDate: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("ReceiptDateText")).getValue(),
                    txtBillingTargetCode: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("BillingTargetCodeShort")).getValue(),
                    txtBillingClientName: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("BillingClientName")).getValue(),
                    txtBillingClientAddress: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("BillingClientAddress")).getValue(),
                    txtReceiptAmount: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("ReceiptAmountShow")).getValue(),
                    txtCollectionArea: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("CollectionAreaCodeName")).getValue(),
                    dtpExpectedCollectDate: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("ExpectedCollectDateText")).getValue(),
                    txtMemo: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("Memo")).getValue(),
                    rowid: i

                    // example non clustom grid getvalue
                    //strBillingamount: grdMoneyCollectionManagementInformationGrid.cells
                    //(row_id, grdMoneyCollectionManagementInformationGrid.getColIndexById("Billingamount")).getValue(),
                };
                arr1.push(obj1);

                var obj2 = {

                    ReceiptNo: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("ReceiptNo")).getValue(),
                    ReceiptDate: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("ReceiptDateText")).getValue(),
                    BillingTargetCode: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("BillingTargetCodeShort")).getValue(),
                    BillingClientNameEN: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("BillingClientNameEN")).getValue(),
                    BillingClientNameLC: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("BillingClientNameLC")).getValue(),
                    BillingClientAddressEN: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("BillingClientAddressEN")).getValue(),
                    BillingClientAddressLC: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("BillingClientAddressLC")).getValue(),
                    ReceiptAmount: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("ReceiptAmountShow")).getValue(),
                    CollectionAreaName: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("CollectionAreaCodeName")).getValue(),
                    ExpectedCollectDate: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("ExpectedCollectDateText")).getValue(),
                    Memo: ICS101.grdMoneyCollectionManagementInformationGrid.cells
                                        (row_id, ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("Memo")).getValue()

                    // example non clustom grid getvalue
                    //strBillingamount: grdMoneyCollectionManagementInformationGrid.cells
                    //(row_id, grdMoneyCollectionManagementInformationGrid.getColIndexById("Billingamount")).getValue(),
                };
                arr2.push(obj2);
            }
        }
        var detail1 = arr1;
        var doICS101_CSVGridData = arr2;

        var returnObj = {
            Header: header,
            Detail1: detail1,
            doICS101_CSVGridData: doICS101_CSVGridData
        };

        return returnObj;
    },

    btn_Search_click: function () {
        $("#btnSearch").attr("disabled", true);
        master_event.LockWindow(true);

        var obj = ICS101.GetUserAdjustData();
        $("#ICS101_MoneyCollectionManagementInformationGrid").LoadDataToGrid(
            ICS101.grdMoneyCollectionManagementInformationGrid,
            ICS101.pageRow, false, "/Income/ICS101_SearchData", obj, "ICS101_doGetMoneyCollectionManagementInfo", false,
            function (result, controls, isWarning) { //post-load
                master_event.LockWindow(false);

                if (controls != undefined) {
                    VaridateCtrl(controls, controls);
                }
                if (result != undefined) {
                    master_event.ScrollWindow("#divResaultList");

                    if (CheckFirstRowIsEmpty(ICS101.grdMoneyCollectionManagementInformationGrid) == false) {
                        ICS101.SetVisableTable(ICS101.TableMode.Yes);
                        ICS101.SetFormMode(ICS101.CommandMode.Init);
                        bolViewMode = true;

                        $("#btnDownloadFile").attr("disabled", false);
                        $("#dtpExpectedCollectDateFrom").EnableDatePicker(false);
                        $("#dtpExpectedCollectDateTo").EnableDatePicker(false);
                        $("#chklCollectionArea").attr("disabled", true);
                        $("#btnSearch").attr("disabled", true);
                        $("#btnClear").attr("disabled", false);
                    }
                    else {
                        ICS101.SetVisableTable(ICS101.TableMode.Yes);
                        ICS101.SetFormMode(ICS101.CommandMode.Init);
                        bolViewMode = true;

                        $("#btnDownloadFile").attr("disabled", true);
                        $("#dtpExpectedCollectDateFrom").EnableDatePicker(true);
                        $("#dtpExpectedCollectDateTo").EnableDatePicker(true);
                        $("#chklCollectionArea").attr("disabled", false);

                        $("#btnSearch").attr("disabled", false);
                        $("#btnClear").attr("disabled", false);
                    }
                }
                else {
                    $("#btnSearch").removeAttr("disabled");
                }
            },
            function (result, controls, isWarning) { //pre-load
                if (isWarning == undefined) {
                    $("#divResaultList").show();
                }
            });
    },
    btn_Clear_click: function () {
        /* --- Get Message --- */
        //        var obj = {
        //            module: "Common",
        //            code: "MSG0038"
        //        };
        //        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        //            OpenOkCancelDialog(result.Code, result.Message,
        //            function () {
        $("#dtpExpectedCollectDateFrom").EnableDatePicker(true);
        $("#dtpExpectedCollectDateTo").EnableDatePicker(true);
        $("#chklCollectionArea").attr("disabled", false);

        $("#dtpExpectedCollectDateFrom").val("");
        $("#dtpExpectedCollectDateTo").val("");
        ClearDateFromToControl("#dtpExpectedCollectDateFrom", "#dtpExpectedCollectDateTo")

        $("#chklCollectionArea").find("input:checkbox").each(function () {
            if ($(this).prop("checked") == true) {
                $(this).attr("checked", false);
            }
        });

        ICS101.SetVisableTable(ICS101.TableMode.No);

        $("#btnSearch").attr("disabled", false);
        $("#btnClear").attr("disabled", false);
        $("#btnDownloadFile").attr("disabled", false);

        CloseWarningDialog();
        //            },
        //            null);
        //        });
    },
    btn_Download_File_click: function () {
        var obj = ICS101.GetUserAdjustData();
        ajax_method.CallScreenController("/Income/ICS101_SendGRIDData", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                var url = "/Income/ICS101_ExportCSV";
                download_method.CallDownloadController("ifDownload", url, null);
            }
        }, false);
    },

    btnRemoveByMoneyCollectionManagementInformationGrid: function (rowId) {
        /* --- Get Message --- */
        var obj = {
            module: "Common",
            code: "MSG0142"
        };
        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
            OpenOkCancelDialog(result.Code, result.Message,
            function () {

                var ReceiptNo = ICS101.grdMoneyCollectionManagementInformationGrid.cells(rowId,
                                        ICS101.grdMoneyCollectionManagementInformationGrid.getColIndexById("ReceiptNo")).getValue();
                var objDelete = { DeleteReceiptNo: ReceiptNo };
                ajax_method.CallScreenController("/Income/ICS101_DeleteData", objDelete, function (result, controls, isWarning) {
                    if (result != undefined) {

                        DeleteRow(ICS101.grdMoneyCollectionManagementInformationGrid, rowId);

                        if (CheckFirstRowIsEmpty(ICS101.grdMoneyCollectionManagementInformationGrid) == true) {
                            ICS101.SetFormMode(ICS101.CommandMode.Init);
                            bolViewMode = false;
                        }
                    }
                    else if (controls != undefined) {
                        VaridateCtrl(controls, controls);
                    }
                });
            },
            null);
        });
    },

    btn_Reset_click: function () {
        ICS101.SetVisableTable(ICS101.TableMode.No);
        ICS101.SetFormMode(ICS101.CommandMode.Init);
        bolViewMode = false;
        $("#divResaultList").SetViewMode(false);
        $("#divResaultList").ResetToNormalControl(true);

        ClearScreenToInitStage();
    },
//    btn_Confirm_click: function () {
//        // save data
//        ajax_method.CallScreenController("/Income/ICS101_Confirm", "",
//            function (result, controls, isWarning) {
//                if (result != undefined) {
//                    if (result == "1") {
//                        // goto confirm state
//                        ICS101.SetVisableTable(ICS101.TableMode.No);
//                        ICS101.SetFormMode(ICS101.CommandMode.Init);
//                        bolViewMode = false;
//                        $("#divResaultList").SetViewMode(false);
//                        $("#divResaultList").ResetToNormalControl(true);

//                        // Show delete button in grid
//                        var colBtnRemoveIdx1 = grdByBillingDetailGrid.getColIndexById("Del");
//                        grdByBillingDetailGrid.setColumnHidden(colBtnRemoveIdx1, false);
//                        // Concept by P'Leing
//                        SetFitColumnForBackAction(grdByBillingDetailGrid, "TempSpan");

//                        ClearScreenToInitStage();
//                    }
//                }
//                else {
//                    VaridateCtrl(controls, controls);
//                }
//            });
//    },
//    btn_Back_click: function () {
//        ICS101.SetFormMode(ICS101.CommandMode.View);
//        bolViewMode = false;
//        $("#divResaultList").SetViewMode(false);

//        $("#divResaultList").ResetToNormalControl(true);

//        // Show delete button in grid
//        var colBtnRemoveIdx1 = grdByBillingDetailGrid.getColIndexById("Del");
//        grdByBillingDetailGrid.setColumnHidden(colBtnRemoveIdx1, false);
//        // Concept by P'Leing
//        SetFitColumnForBackAction(grdByBillingDetailGrid, "TempSpan");
//    },

    // Clear Screen
    ClearScreenToInitStage: function () {
    },
    // Enable Obj On Screen
    // Visable Obj On Screen
    SetVisableTable: function (intMode) {
        if (intMode == ICS101.TableMode.Yes) {
            $("#divResaultList").show();
        }
        else if (intMode == ICS101.TableMode.No) {
            $("#divResaultList").hide();
        }
        else {
            $("#divResaultList").hide();
        };
    }
}

$(document).ready(function () {
    ICS101.InitialControl();
});
