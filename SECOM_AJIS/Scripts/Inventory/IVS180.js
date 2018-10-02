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



var gridSearchResult;
var gridCheckingDetail;
var Hilightlist = [];
var currencyTotal = 0;

var IVS180 = {
    SelectedInstrumentList: [],
    ScreenMode: {
        Register: "RegisterMode",
        Confirm: "ConfirmMode",
        Finish: "FinishMode",
        NewRegister: "NewRegisterMode",
        Reset: "Reset"
    }
};

$(document).ready(function () {

    // Initial grid 1
    if ($.find("#IVS180_SearchResultGrid").length > 0) {
        gridSearchResult = $("#IVS180_SearchResultGrid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Inventory/IVS180_InitialSearchResultGrid");
    }

    // Initial grid 2
    if ($.find("#IVS180_RetrundInstrumentCheckingDetailGrid").length > 0) {
        gridCheckingDetail = $("#IVS180_RetrundInstrumentCheckingDetailGrid").InitialGrid(0, false, "/Inventory/IVS180_InitialAdjustmentDetailGrid");
    }

    $("#InstrumentCode").InitialAutoComplete("/Master/GetInstrumentCodeAll", null, true);

    //Binding 1. (search result)
    SpecialGridControl(gridSearchResult, ["BtnAdd"]);
    BindOnLoadedEvent(gridSearchResult, function () {
        var colInx = gridSearchResult.getColIndexById('BtnAdd');
        for (var i = 0; i < gridSearchResult.getRowsNum(); i++) {
            var rowId = gridSearchResult.getRowId(i);
            GenerateAddButton(gridSearchResult, "btnAdd", rowId, "BtnAdd", true);

            // binding grid button event 
            BindGridButtonClickEvent("btnAdd", rowId, AddInstrument);
        }

        gridSearchResult.setSizes();
    });

    SpecialGridControl(gridCheckingDetail, ["StockAdjQty", "BtnRemove"]);

    //Binding 2. (adjustment detail)    
    BindOnLoadedEvent(gridCheckingDetail, function () {
        var colInx = gridCheckingDetail.getColIndexById('BtnRemove');
        for (var i = 0; i < gridCheckingDetail.getRowsNum(); i++) {
            var rowId = gridCheckingDetail.getRowId(i);
            GenerateAddButton(gridCheckingDetail, "btnRemove", rowId, "BtnRemove", true);

            // binding grid button event 
            BindGridButtonClickEvent("btnRemove", rowId, RemoveInstrument);

            // generate numeric text box in grid
            var qtyColIndex = gridCheckingDetail.getColIndexById("StockAdjQty");
            var val = GetValueFromLinkType(gridCheckingDetail, i, qtyColIndex);
            GenerateNumericBox2(gridCheckingDetail, "txtStockAdjQty", rowId, "StockAdjQty", val, 5, 0, 0, 99999, true, true);
        }



        gridCheckingDetail.setSizes();
    });

    // Event binding
    $("#btnSearch").click(IVS180_Search);
    $("#btnClear").click(Clear);
    $("#btnDownloadSlip").click(DownloadSlip);
    $("#btnNewRegister").click(NewRegister);

    // Initial related combobox
    $("#SourceLocationCode").change(cbo_sourceLocationCode_change);

    //Initial Page
    InitialPage();
});

function InitialPage() {
    $("#TransferDate").InitialDate();
    $("#Memo").SetMaxLengthTextArea(1000);

    cbo_sourceLocationCode_change();

    if (IVS180_ViewBag_Flag.EnableChecking == 1)
        $("#divTransferInstrumentDetail #TransferDate").EnableDatePicker(false);
    else
        $("#divTransferInstrumentDetail #TransferDate").EnableDatePicker(true);

    //var now = new Date();
    //$("#TransferDate").val(ConvertDateToTextFormat(ConvertDateObject(now)));

    $("#IVS180_SearchResultGrid").show();

    // Hide adjust detail and download
    $("#divTransferInstrumentDetail").hide();
    $("#divShowSlipNo").hide();
}

// AddInstrument
function AddInstrument(rowId) {

    // Set selected row 
    gridSearchResult.selectRow(gridSearchResult.getRowIndex(rowId));

    // Create JSON object from string JSON
    var strJson = gridSearchResult.cells2(gridSearchResult.getRowIndex(rowId), gridSearchResult.getColIndexById('ToJson')).getValue();
    strJson = htmlDecode(strJson);
    var data = JSON.parse(strJson);

    if (data != undefined) {

        var key = data.Instrumentcode + "-" + data.AreaCode + "-" + data.ShelfNo;

        // Check exist
        var checkInx = search_array_index(IVS180.SelectedInstrumentList, "key", key);
        if (checkInx >= 0) {
            var param = { "module": "Inventory", "code": "MSG4005", "param": "" };
            ajax_method.CallScreenController("/Shared/GetMessage", param, function (data) {
                OpenWarningDialog(data.Message);
            });
            return;
        }

        data["key"] = key;
        IVS180.SelectedInstrumentList.push(data);

        // Show section adjustment
        $("#divTransferInstrumentDetail").show();

        //R2
        $("#IVS180_RetrundInstrumentCheckingDetailGrid").focus();
        //End R2

        // Show command [Register] [Reset]
        register_command.SetCommand(command_register_click);
        reset_command.SetCommand(command_reset_click);


        //================ Add New row to grid ================
        var blankData = [data.Instrumentcode,
                            data.InstrumentName,
                            data.AreaCodeName,
                            data.InstrumentQty,
                            "", // StockAdjQty
                            "0.00", // StockAdjAmount
                            "", // BtnRemove
                            "", // TempSpan
                            strJson];

        if (CheckFirstRowIsEmpty(gridCheckingDetail, false)) {

            // Add new row with Row# Total 

            CheckFirstRowIsEmpty(gridCheckingDetail, true);
            AddNewRow(gridCheckingDetail, blankData); // ToJson

            var row_idx = gridCheckingDetail.getRowsNum() - 1;
            var row_id = gridCheckingDetail.getRowId(row_idx);

            var defaultVal = 0;
            GenerateNumericBox2(gridCheckingDetail, "txtStockAdjQty", row_id, "StockAdjQty", "", 5, 0, 0, 99999, true, true);
            GenerateRemoveButton(gridCheckingDetail, "btnRemove", row_id, "BtnRemove", true);
            gridCheckingDetail.setSizes();

            var StockAdj = "#" + GenerateGridControlID("txtStockAdjQty", row_id);
            var row_i = row_id;
            var row_ix = row_idx;

            $(StockAdj).blur(function () {
                CheckStockAdjust($(this), row_i, row_ix);
            });

            BindGridButtonClickEvent("btnRemove", row_id, function (rid) {

                if (gridCheckingDetail.getRowsNum() <= 2) {
                    DeleteAllRow(gridCheckingDetail);
                }
                else {
                    DeleteRow(gridCheckingDetail, rid);
                }


                // delete from array
                var removedIdx = search_array_index(IVS180.SelectedInstrumentList, "key", key);
                IVS180.SelectedInstrumentList.splice(removedIdx, 1);

                if (CheckFirstRowIsEmpty(gridCheckingDetail, false) == true) {
                    //register_command.EnableCommand(false);
                }

            });


            // Row# Total

            var btnRemoveInx = gridCheckingDetail.getColIndexById("BtnRemove");
            var lblCalulate = $("#btnCalculate").val();
            var lblTotalAmountOfTransfer = "<div style='text-align: right;'>" + $("#lblTotalAmountOfTransfer").val() + " </div>";
            AddNewRow(gridCheckingDetail, [lblTotalAmountOfTransfer,
                                       "",
                                       "",
                                       "",
                                       "", // StockAdjQty
                                       "", // StockAdjAmount
                                       "", // BtnRemove
                                       "", // TempSpan
                                       ""]); // ToJson

            var row_idx = gridCheckingDetail.getRowsNum() - 1;
            var row_id = gridCheckingDetail.getRowId(row_idx);

            //gridCheckingDetail.cells(row_id, btnRemoveInx).setValue(GenerateHtmlButton("btnCalculate", row_id, lblCalulate, true));
            GenerateCalculateButton(gridCheckingDetail, "btnCalculate", row_id, "BtnRemove", true);

            //BindGridHtmlButtonClickEvent("btnCalculate", row_id, CalculateAmount);
            BindGridButtonClickEvent("btnCalculate", row_id, function (rid) {
                CalculateAmount();
            });

            gridCheckingDetail.setColspan(row_id, 0, 4);
            gridCheckingDetail.setColspan(row_id, 4, 2);

            gridCheckingDetail.setSizes();
        }
        else {

            // Add new row (Normal)

            gridCheckingDetail.addRow(gridCheckingDetail.uid(),
                                        blankData,
                                        gridCheckingDetail.getRowsNum() - 1);

            var row_idx = gridCheckingDetail.getRowsNum() - 2;
            var row_id = gridCheckingDetail.getRowId(row_idx);

            var defaultVal = 0;
            GenerateNumericBox2(gridCheckingDetail, "txtStockAdjQty", row_id, "StockAdjQty", "", 5, 0, 0, 99999, true, true);
            GenerateRemoveButton(gridCheckingDetail, "btnRemove", row_id, "BtnRemove", true);
            gridCheckingDetail.setSizes();

            var StockAdj = "#" + GenerateGridControlID("txtStockAdjQty", row_id);
            var row_i = row_id;
            var row_ix = row_idx;

            $(StockAdj).blur(function () {
                CheckStockAdjust($(this), row_i, row_ix);
            });

            BindGridButtonClickEvent("btnRemove", row_id, function (rid) {
                if (gridCheckingDetail.getRowsNum() <= 2) {
                    DeleteAllRow(gridCheckingDetail);
                }
                else {
                    DeleteRow(gridCheckingDetail, rid);
                }

                // delete from array
                var removedIdx = search_array_index(IVS180.SelectedInstrumentList, "key", key);
                IVS180.SelectedInstrumentList.splice(removedIdx, 1);

                if (CheckFirstRowIsEmpty(gridCheckingDetail, false) == true) {
                    register_command.EnableCommand(false);
                }

            });

            gridCheckingDetail.setSizes();

        }


    }
}

function CheckStockAdjust(stockAdj, row_i, row_ix) {
    var instrumentQty = gridCheckingDetail.cells(row_i, gridCheckingDetail.getColIndexById("InstrumentQty")).getValue();
    var stockAdjQty = $(stockAdj).NumericValue();

    //R2
    if (stockAdjQty > instrumentQty) {
        gridCheckingDetail.setCellTextStyle(row_i, gridCheckingDetail.getColIndexById("InstrumentCode"), "color:red;");
        gridCheckingDetail.setCellTextStyle(row_i, gridCheckingDetail.getColIndexById("InstrumentName"), "color:red;");
        gridCheckingDetail.setCellTextStyle(row_i, gridCheckingDetail.getColIndexById("AreaCodeName"), "color:red;");
        gridCheckingDetail.setCellTextStyle(row_i, gridCheckingDetail.getColIndexById("InstrumentQty"), "color:red;");
        gridCheckingDetail.setCellTextStyle(row_i, gridCheckingDetail.getColIndexById("StockAdjAmount"), "color:red;");
    }
    else {
        gridCheckingDetail.setCellTextStyle(row_i, gridCheckingDetail.getColIndexById("InstrumentCode"), "color:black;");
        gridCheckingDetail.setCellTextStyle(row_i, gridCheckingDetail.getColIndexById("InstrumentName"), "color:black;");
        gridCheckingDetail.setCellTextStyle(row_i, gridCheckingDetail.getColIndexById("AreaCodeName"), "color:black;");
        gridCheckingDetail.setCellTextStyle(row_i, gridCheckingDetail.getColIndexById("InstrumentQty"), "color:black;");
        gridCheckingDetail.setCellTextStyle(row_i, gridCheckingDetail.getColIndexById("StockAdjAmount"), "color:black;");
    }
    //End R2
}

function CalculateAmount() {
    var all = GetUserAdjustData();
    var obj = { detail: all.Detail };
    ajax_method.CallScreenController("/Inventory/IVS180_CalculateAmount", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            if (result.length >= 2) {

                // ----- Result -----
                // [0] = detail list
                // [1] = total amount
                // [2] = HiglightRows

                // Update Amount
                var colInx = gridCheckingDetail.getColIndexById('StockAdjAmount');
                for (var i = 0; i < result[0].length; i++) {
                    //gridCheckingDetail.cells(result[0][i].row_id, colInx).setValue(result[0][i].AdjustAmount.toString());
                    gridCheckingDetail.cells(result[0][i].row_id, colInx).setValue(result[0][i].TextTransferAmount);
                    currencyTotal = (result[0][i].TextTransferAmount).split(" ");
                }

                // Update total amount
                if (result[1] != undefined) {
                    var lastRowInx = gridCheckingDetail.getRowsNum() - 1;
                    gridCheckingDetail.cells2(lastRowInx, colInx).setValue(currencyTotal[0] + ' ' + result[1]); // result[1] => totalAmount
                }

                // Hilight row (if has)
                for (var i = 0; i < Hilightlist.length; i++) {

                    gridCheckingDetail.setRowTextStyle(Hilightlist[i], "color:black;");

                    //gridCheckingDetail.setCellTextStyle(Hilightlist[i], gridCheckingDetail.getColIndexById("InstrumentCode"), "color:black;");
                    //gridCheckingDetail.setCellTextStyle(Hilightlist[i], gridCheckingDetail.getColIndexById("InstrumentName"), "color:black;");
                    //gridCheckingDetail.setCellTextStyle(Hilightlist[i], gridCheckingDetail.getColIndexById("AreaCodeName"), "color:black;");
                    //gridCheckingDetail.setCellTextStyle(Hilightlist[i], gridCheckingDetail.getColIndexById("InstrumentQty"), "color:black;");
                    //gridCheckingDetail.setCellTextStyle(Hilightlist[i], gridCheckingDetail.getColIndexById("StockAdjAmount"), "color:black;");

                    gridCheckingDetail.setCellTextStyle(Hilightlist[i], gridCheckingDetail.getColIndexById("StockAdjQty"), "color:transparent;");
                }
                if (result[2].length > 0) {
                    for (var i = 0; i < result[2].length; i++) {

                        gridCheckingDetail.setRowTextStyle(result[2][i], "color:red;"); // result[2][i] => row_id

                        // InstrumentCode
                        // InstrumentName
                        // AreaCodeName
                        // InstrumentQty
                        // StockAdjAmount

                        //gridCheckingDetail.setCellTextStyle(result[2][i], gridCheckingDetail.getColIndexById("InstrumentCode"), "color:red;");
                        //gridCheckingDetail.setCellTextStyle(result[2][i], gridCheckingDetail.getColIndexById("InstrumentName"), "color:red;");
                        //gridCheckingDetail.setCellTextStyle(result[2][i], gridCheckingDetail.getColIndexById("AreaCodeName"), "color:red;");
                        //gridCheckingDetail.setCellTextStyle(result[2][i], gridCheckingDetail.getColIndexById("InstrumentQty"), "color:red;");
                        //gridCheckingDetail.setCellTextStyle(result[2][i], gridCheckingDetail.getColIndexById("StockAdjAmount"), "color:red;");

                        gridCheckingDetail.setCellTextStyle(result[2][i], gridCheckingDetail.getColIndexById("StockAdjQty"), "color:transparent;");
                    }

                    Hilightlist = result[2];

                }

            }
        }
        else if (controls != undefined) {
            // Validate again -> but not pass then set screen to input mode and hilight controls
            VaridateCtrl(controls, controls);
        }
    });
}

// RemoveInstrument
function RemoveInstrument(rowId) {
    // ..
}


function SetScreenMode(mode) {
    if (mode == IVS180.ScreenMode.Register) {

        $("#divSearchInstrument").show();
        $("#divTransferInstrumentDetail").show();
        $("#divShowSlipNo").hide();


        register_command.SetCommand(command_register_click);
        reset_command.SetCommand(command_reset_click);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);

        $("#divTransferInstrumentDetail").SetViewMode(false);


        // Show delete button in grid
        var colBtnRemoveIdx = gridCheckingDetail.getColIndexById("BtnRemove");
        gridCheckingDetail.setColumnHidden(colBtnRemoveIdx, false);
        // Concept by P'Leing
        SetFitColumnForBackAction(gridCheckingDetail, "TempSpan");

        // Set Hilight row back to normal (this is for fix temprorary) 
        for (var i = 0; i < Hilightlist.length; i++) {
            //gridCheckingDetail.setRowTextStyle(Hilightlist[i], "color:black;");

            gridCheckingDetail.setCellTextStyle(Hilightlist[i], gridCheckingDetail.getColIndexById("StockAdjQty"), "color:transparent;");
        }
    }
    else if (mode == IVS180.ScreenMode.Confirm) {

        $("#divSearchInstrument").hide();
        $("#divTransferInstrumentDetail").show();
        $("#divShowSlipNo").hide();

        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(command_confirm_click);
        back_command.SetCommand(command_back_click);

        $("#divTransferInstrumentDetail").SetViewMode(true);

        // Hide delete button in grid
        var colBtnRemoveIdx = gridCheckingDetail.getColIndexById("BtnRemove");
        gridCheckingDetail.setColumnHidden(colBtnRemoveIdx, true);
    }
    else if (mode == IVS180.ScreenMode.Finish) {

        $("#divSearchInstrument").hide();
        $("#divTransferInstrumentDetail").show();
        $("#divShowSlipNo").show();

        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);

        IVS180.SelectedInstrumentList = [];
    }
    else if (mode == IVS180.ScreenMode.NewRegister || mode == IVS180.ScreenMode.Reset) {
        DeleteAllRow(gridSearchResult);
        $("#divSearchInstrument").show();
        $("#divTransferInstrumentDetail").hide();
        $("#divShowSlipNo").hide();

        $("#SourceLocationCode").attr("disabled", false);


        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);

        $("#divTransferInstrumentDetail").SetViewMode(false);

        // Show delete button in grid
        var colBtnRemoveIdx = gridCheckingDetail.getColIndexById("BtnRemove");
        gridCheckingDetail.setColumnHidden(colBtnRemoveIdx, false);
        // Concept by P'Leing
        SetFitColumnForBackAction(gridCheckingDetail, "TempSpan");

        $("#divSpecifyLocation").clearForm();
        $("#divTransferInstrumentDetail").clearForm();
        IVS180.SelectedInstrumentList = [];

        var param = { "LocationCode": $("#SourceLocationCode").val() };
        ajax_method.CallScreenController("/Inventory/IVS180_GetLocation", param, update_location_combo);

        var now = new Date();
        $("#TransferDate").val(ConvertDateToTextFormat(ConvertDateObject(now)));


        // Clear row
        //DeleteAllRow(gridSearchResult);
        DeleteAllRow(gridCheckingDetail);

        $("#IVS180_SearchResultGrid").show();

        // Clear list
        Hilightlist = [];
    }
}

//Search
function IVS180_Search() {
    // Disable specify location
    $("#SourceLocationCode").attr("disabled", true);
    reset_command.SetCommand(command_reset_click);

    // For prevent click this button more than one time
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    var obj = {
        cond: CreateObjectData($("#formSearchCriteria").serialize()),
        sourceLocationCode: $("#SourceLocationCode").val()
    };

    //R2
    if (obj.cond.AreaCode == "") {
        obj.cond.AreaCodeList = [];
        $("#AreaCode > option").each(function (index) {
            if (index > 0) {
                obj.cond.AreaCodeList.push($(this).val());
            }
        });
    }
    //End R2

    $("#IVS180_SearchResultGrid").LoadDataToGrid(gridSearchResult, ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Inventory/IVS180_SearchResponse", obj, "dtSearchInstrumentListResult", false,
                    function (result, controls, isWarning) { // post-load
                        // For prevent click this button more than one time
                        if (isWarning == true) {
                            VaridateCtrl_AtLeast(["InstrumentCode", "InstrumentName", "AreaCode"], ["InstrumentCode", "InstrumentName", "AreaCode"]);
                        }
                        master_event.LockWindow(false);
                        master_event.ScrollWindow("#ResultSection");
                        $("#btnSearch").attr("disabled", false);
                    },
                    function (result, controls, isWarning) { // pre-load
                        if (isWarning == undefined) {
                            $("#IVS180_SearchResultGrid").show();
                        }
                    });
}

// Cbo_SourceLocationCode_Change
function cbo_sourceLocationCode_change() {
    if ($("#SourceLocationCode").val() != "") {
        var param = { "LocationCode": $("#SourceLocationCode").val() };
        ajax_method.CallScreenController("/Inventory/IVS180_GetLocation", param, update_location_combo);
    }
}

function update_location_combo(data) {
    if (data != undefined) {
        if (data.length == 2) {
            regenerate_combo("#LocationCode", data[0]);

            // show destination name
            $("#DestinationLocation").val(data[1]);
        }
    }

}

// Clear
function Clear() {
    $("#formSearchCriteria").clearForm();
    //$("#IVS180_SearchResultGrid").hide();
    DeleteAllRow(gridSearchResult);
    CloseWarningDialog();
}

// DownloadSlip
function DownloadSlip() {
    ajax_method.CallScreenController("/Inventory/IVS180_CheckExistFile", "", function (result) {
        if (result == 1) {
            var key = ajax_method.GetKeyURL(null);
            var url = ajax_method.GenerateURL("/Inventory/IVS180_DownloadPdfAndWriteLog?k=" + key);
            window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
        }
        else {
            var param = { "module": "Common", "code": "MSG0112" };
            call_ajax_method("/Shared/GetMessage", param, function (data) {
                /* ====== Open info dialog =====*/
                OpenInformationMessageDialog(param.code, data.Message);
            });
        }

    });
}


// NewRegister
function NewRegister() {
    SetScreenMode(IVS180.ScreenMode.NewRegister);
}

function command_register_click() {
    command_control.CommandControlMode(false);
    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Inventory/IVS180_Register", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            if (result.length >= 2) {

                //--------- Result -----------
                // resul[0] = is success
                // resul[1] = HilightRows list
                // resul[2] = detail list
                // resul[3] = total amount

                if (result[0] == "1") {
                    // goto confirm state
                    SetScreenMode(IVS180.ScreenMode.Confirm);
                }


                // HilightRows
                for (var i = 0; i < Hilightlist.length; i++) {
                    gridCheckingDetail.setRowTextStyle(Hilightlist[i], "color:black;");
                }
                if (result[1].length > 0) {
                    // HilightRows  
                    for (var i = 0; i < result[1].length; i++) {
                        gridCheckingDetail.setRowTextStyle(result[1][i], "color:red;"); // result[1][i] => list of row_id
                    }

                    Hilightlist = result[1];
                }

                // Update Amount
                var colInx = gridCheckingDetail.getColIndexById('StockAdjAmount');
                for (var i = 0; i < result[2].length; i++) {
                    //gridCheckingDetail.cells(result[2][i].row_id, colInx).setValue(result[2][i].AdjustAmount.toString());
                    gridCheckingDetail.cells(result[2][i].row_id, colInx).setValue(result[2][i].TextTransferAmount);
                    gridCheckingDetail.cells(result[2][i].row_id, gridCheckingDetail.getColIndexById('InstrumentQty')).setValue(result[2][i].InstrumentQty.toString());
                    currencyTotal = (result[2][i].TextTransferAmount).split(" ");
                }

                // Update total amount
                if (result[3] != undefined) {
                    var lastRowInx = gridCheckingDetail.getRowsNum() - 1;
                    gridCheckingDetail.cells2(lastRowInx, colInx).setValue(currencyTotal[0] + ' ' + result[3]); // result[3] => totalAmount
                }
            }
            else {
                var colInx = gridCheckingDetail.getColIndexById('InstrumentQty');
                for (var i = 0; i < result[0].length; i++) {
                    gridCheckingDetail.cells(result[0][i].row_id, colInx).setValue(result[0][i].InstrumentQty.toString());
                }

                VaridateCtrl(controls, controls);
            }
        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });

    command_control.CommandControlMode(true);
}

function command_confirm_click() {
    // Call ajax to save
    command_control.CommandControlMode(false);
    ajax_method.CallScreenController("/Inventory/IVS180_Confirm", "",
        function (result, controls, isWarning) {
            if (controls != undefined) {
                // Validate again -> but not pass then set screen to input mode and hilight controls
                SetScreenMode(IVS180.ScreenMode.Register);
                VaridateCtrl(controls, controls);

                if (result != undefined) {
                    var colInx = gridCheckingDetail.getColIndexById('InstrumentQty');
                    for (var i = 0; i < result[0].length; i++) {
                        gridCheckingDetail.cells(result[0][i].row_id, colInx).setValue(result[0][i].InstrumentQty.toString());
                    }
                }
            }
            else if (result != undefined) {
                if (result != "") {
                    // Set slip no to textbox
                    $("#SlipNo").val(result);
                    SetScreenMode(IVS180.ScreenMode.Finish);
                }
            }
        });

    command_control.CommandControlMode(true);
}

function command_reset_click() {
    SetScreenMode(IVS180.ScreenMode.Reset);
}

function command_back_click() {
    SetScreenMode(IVS180.ScreenMode.Register);
}


function GetUserAdjustData() {

    var arr = new Array();
    var grid = gridCheckingDetail;

    if (CheckFirstRowIsEmpty(grid) == false) {
        for (var i = 0; i < grid.getRowsNum(); i++) {
            if (i < grid.getRowsNum() - 1) { // Not include Row# Total
                var row_id = grid.getRowId(i);
                var txtStockAdjQty_id = GenerateGridControlID("txtStockAdjQty", row_id);

                var strJson = grid.cells2(i, grid.getColIndexById("ToJson")).getValue();
                var data = JSON.parse(htmlDecode(strJson));

                var obj = {
                    txtStockAdjQtyID: txtStockAdjQty_id,
                    Instrumentcode: data.Instrumentcode,
                    AreaCode: data.AreaCode,
                    ShelfNo: data.ShelfNo,
                    FixedStockQty: $("#" + txtStockAdjQty_id).NumericValue(),
                    row_id: row_id,
                    LocationCode: data.LocationCode
                };

                arr.push(obj);
            }
        }
    }


    var header = {
        ApproveNo: $("#ApproveNo").val(),
        Memo: $("#Memo").val(),
        TransferDate: $("#TransferDate").val()
    };

    var detail = arr;

    var returnObj = {
        Header: header,
        Detail: detail
    };

    return returnObj;

}

function VaridateCtrl_AtLeast(ctrl_lst, null_ctrl) {
    if (ctrl_lst != null) {
        for (var idx = 0; idx < ctrl_lst.length; idx++) {
            var ctrl = $("#" + ctrl_lst[idx]);
            if (ctrl.length > 0) {
                ctrl.removeClass("highlight");


                if (ctrl[0].tagName.toLowerCase() == "select") {
                    var unb = function () {
                        for (var i = 0; i < ctrl_lst.length; i++) {
                            $("#" + ctrl_lst[i]).removeClass("highlight");
                            $("#" + ctrl_lst[i]).unbind("change", unb);
                        }
                    };
                    ctrl.change(unb);
                }
                else {
                    var unb = function () {
                        for (var i = 0; i < ctrl_lst.length; i++) {
                            $("#" + ctrl_lst[i]).removeClass("highlight");
                            $("#" + ctrl_lst[i]).unbind("keyup", unb);
                        }
                        // $(this).removeClass("highlight");
                        //  $(this).unbind("keyup", unb);
                    };
                    ctrl.keyup(unb);


                }
            }
        }
    }
    if (null_ctrl != null) {
        for (var idx = 0; idx < null_ctrl.length; idx++) {
            if (null_ctrl[idx] != "") {
                var ctrl = $("#" + null_ctrl[idx]);
                if (ctrl.length > 0) {
                    ctrl.addClass("highlight");
                }
            }
        }
    }

}