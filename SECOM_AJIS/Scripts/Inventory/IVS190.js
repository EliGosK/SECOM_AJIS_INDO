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
var currencyTotal = 0;

var IVS190 = {
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
    if ($.find("#IVS190_SearchResultGrid").length > 0) {
        gridSearchResult = $("#IVS190_SearchResultGrid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Inventory/IVS190_InitialSearchResultGrid");
    }

    // Initial grid 2
    if ($.find("#IVS190_RetrundInstrumentCheckingDetailGrid").length > 0) {
        gridCheckingDetail = $("#IVS190_RetrundInstrumentCheckingDetailGrid").InitialGrid(0, false, "/Inventory/IVS190_InitialAdjustmentDetailGrid");
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

    //Binding 2. (adjustment detail)
    SpecialGridControl(gridCheckingDetail, ["StockAdjQty", "BtnRemove"]);
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
    $("#btnSearch").click(IVS190_Search);
    $("#btnClear").click(Clear);
    $("#btnDownloadSlip").click(DownloadSlip);
    $("#btnNewRegister").click(NewRegister);

    // Initial page
    InitialPage();
});


// InitialPage
function InitialPage() {
    $("#Memo").SetMaxLengthTextArea(1000);

    $("#radPlus").attr("disabled", false);
    if (IVS190_ViewBag.DisableMinus == "True") {
        $("#radMinus").attr("disabled", true);
    }
    else {
        $("#radMinus").attr("disabled", false);
    }

    $("#IVS190_SearchResultGrid").show();

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
        var checkInx = search_array_index(IVS190.SelectedInstrumentList, "key", key);
        if (checkInx >= 0) {
            var param = { "module": "Inventory", "code": "MSG4005", "param": "" };
            ajax_method.CallScreenController("/Shared/GetMessage", param, function (data) {
                OpenWarningDialog(data.Message);
            });
            return;
        }

        data["key"] = key;
        IVS190.SelectedInstrumentList.push(data);

        // Show section adjustment
        $("#divTransferInstrumentDetail").show();

        //R2
        $("#IVS190_RetrundInstrumentCheckingDetailGrid").focus();
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

            //R2
            SetColorOnAdjustment(row_idx);
            //End R2

            BindGridButtonClickEvent("btnRemove", row_id, function (rid) {

                if (gridCheckingDetail.getRowsNum() <= 2) {
                    DeleteAllRow(gridCheckingDetail);
                }
                else {
                    DeleteRow(gridCheckingDetail, rid);
                }


                // delete from array
                var removedIdx = search_array_index(IVS190.SelectedInstrumentList, "key", key);
                IVS190.SelectedInstrumentList.splice(removedIdx, 1);

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

            //R2
            SetColorOnAdjustment(row_idx);
            //End R2

            BindGridButtonClickEvent("btnRemove", row_id, function (rid) {
                if (gridCheckingDetail.getRowsNum() <= 2) {
                    DeleteAllRow(gridCheckingDetail);
                }
                else {
                    DeleteRow(gridCheckingDetail, rid);
                }

                // delete from array
                var removedIdx = search_array_index(IVS190.SelectedInstrumentList, "key", key);
                IVS190.SelectedInstrumentList.splice(removedIdx, 1);

                if (CheckFirstRowIsEmpty(gridCheckingDetail, false) == true) {
                    //register_command.EnableCommand(false);
                }

            });

            gridCheckingDetail.setSizes();
        }


    }
}

function CalculateAmount() {
    var all = GetUserAdjustData();
    var obj = { detail: all.Detail };
    ajax_method.CallScreenController("/Inventory/IVS190_CalculateAmount", obj, function (result, controls, isWarning) {
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
                    gridCheckingDetail.cells2(lastRowInx, colInx).setValue(currencyTotal[0] + ' '+ result[1]); // result[1] => totalAmount
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

}


function SetScreenMode(mode) {
    if (mode == IVS190.ScreenMode.Register) {

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
    }
    else if (mode == IVS190.ScreenMode.Confirm) {

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
    else if (mode == IVS190.ScreenMode.Finish) {

        $("#divSearchInstrument").hide();
        $("#divTransferInstrumentDetail").show();
        $("#divShowSlipNo").show();

        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);

        IVS190.SelectedInstrumentList = [];
    }
    else if (mode == IVS190.ScreenMode.NewRegister || mode == IVS190.ScreenMode.Reset) {
        DeleteAllRow(gridSearchResult);
        $("#divSearchInstrument").show();
        $("#divTransferInstrumentDetail").hide();
        $("#divShowSlipNo").hide();

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

        $("#formSearchCriteria").clearForm();
        $("#divTransferInstrumentDetail").clearForm();
        IVS190.SelectedInstrumentList = [];

        $("#radPlus").attr("disabled", false);
        if (IVS190_ViewBag.DisableMinus == "True") {
            $("#radMinus").attr("disabled", true);
        }
        else {
            $("#radMinus").attr("disabled", false);
        }

        $("#radPlus").attr("checked", true);

        // Clear row
        //DeleteAllRow(gridSearchResult);
        DeleteAllRow(gridCheckingDetail);

        $("#IVS190_SearchResultGrid").show();
    }
}

//Search
function IVS190_Search() {
    // For prevent click this button more than one time
    master_event.LockWindow(true);
    $("#btnSearch").attr("disabled", true);

    var plus_minus_type;
    if ($("#radPlus").prop("checked")) {
        plus_minus_type = $("#radPlus").val();
    } else if ($("#radMinus").prop("checked")) {
        plus_minus_type = $("#radMinus").val();
    }

    var obj = {
        cond: CreateObjectData($("#formSearchCriteria").serialize()),
        type: plus_minus_type
    };
    $("#IVS190_SearchResultGrid").LoadDataToGrid(gridSearchResult, ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Inventory/IVS190_SearchResponse", obj, "dtSearchInstrumentListResult", false,
                    function () { // post-load
                        // For prevent click this button more than one time
                        $("#btnSearch").attr("disabled", false);
                        $("#radPlus").attr("disabled", true);
                        $("#radMinus").attr("disabled", true);

                        reset_command.SetCommand(command_reset_click);

                        master_event.LockWindow(false);
                        master_event.ScrollWindow("#ResultSection");
                        //R2
                        SetColorOnStockAmount();
                        //End R2
                    },
                    function (result, controls, isWarning) { // pre-load
                        if (isWarning == undefined) {
                            $("#IVS190_SearchResultGrid").show();
                        }
                    });
}


// Clear
function Clear() {
    $("#formSearchCriteria").clearForm();
    //$("#IVS190_SearchResultGrid").hide();
    DeleteAllRow(gridSearchResult);
    CloseWarningDialog();
}

// DownloadSlip
function DownloadSlip() {
    ajax_method.CallScreenController("/Inventory/IVS190_CheckExistFile", "", function (result) {
        if (result == 1) {
            var key = ajax_method.GetKeyURL(null);
            var url = ajax_method.GenerateURL("/Inventory/IVS190_DownloadPdfAndWriteLog?k=" + key);
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
    SetScreenMode(IVS190.ScreenMode.NewRegister);
}


function command_register_click() {
    var obj = {
        module: "Inventory",
        code: "MSG4100"
    };
    call_ajax_method("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
        function () {
            command_control.CommandControlMode(false);
            RegisData();
        },
        null);
    });

    command_control.CommandControlMode(true);
}

function RegisData() {
    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Inventory/IVS190_Register", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            if (result.length >= 2) {

                //--------- Result -----------
                // resul[0] = is success
                // resul[1] = HilightRows list
                // resul[2] = detail list
                // resul[3] = total amount


                if (result[0] == "1") {
                    // goto confirm state
                    SetScreenMode(IVS190.ScreenMode.Confirm);
                }

                if (result[1].length > 0) {
                    // HiglightRows  // result[1][i] => list of row_id
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
                if (controls != undefined) {
                    VaridateCtrl(controls, controls);
                }

                var colInx = gridCheckingDetail.getColIndexById('InstrumentQty');
                for (var i = 0; i < result[0].length; i++) {
                    gridCheckingDetail.cells(result[0][i].row_id, colInx).setValue(result[0][i].InstrumentQty.toString());
                }
            }
        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });
}

function command_confirm_click() {
    // Call ajax to save
    command_control.CommandControlMode(false);
    var plus_minus_type;
    if ($("#radPlus").prop("checked")) {
        plus_minus_type = $("#radPlus").val();
    } else if ($("#radMinus").prop("checked")) {
        plus_minus_type = $("#radMinus").val();
    }

    var obj = {
        cond: CreateObjectData($("#formSearchCriteria").serialize()),
        type: plus_minus_type
    };

    ajax_method.CallScreenController("/Inventory/IVS190_Confirm", obj,
        function (result, controls, isWarning) {
            if (controls != undefined) {
                // Validate again -> but not pass then set screen to input mode and hilight controls
                SetScreenMode(IVS190.ScreenMode.Register);
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
                    SetScreenMode(IVS190.ScreenMode.Finish);
                }
            }
        });

    command_control.CommandControlMode(true);
}

function command_reset_click() {
    SetScreenMode(IVS190.ScreenMode.Reset);
}

function command_back_click() {
    SetScreenMode(IVS190.ScreenMode.Register);
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
        Memo: $("#Memo").val()
    };

    var detail = arr;

    var returnObj = {
        Header: header,
        Detail: detail
    };

    return returnObj;

}

//R2
function SetColorOnStockAmount() {
    if ($("#radMinus").prop("checked")) {
        for (var i = 0; i < gridSearchResult.getRowsNum(); i++) {
            if (CheckFirstRowIsEmpty(gridCheckingDetail, false)) {
                gridSearchResult.setCellTextStyle(gridSearchResult.getRowId(i), gridSearchResult.getColIndexById("InstrumentQty"), "color:red;");
            }
        }
    }
}

function SetColorOnAdjustment(row_id) {
    if ($("#radMinus").prop("checked")) {
        gridCheckingDetail.setCellTextStyle(gridCheckingDetail.getRowId(row_id), gridCheckingDetail.getColIndexById("InstrumentQty"), "color:red;");
        gridCheckingDetail.setCellTextStyle(gridCheckingDetail.getRowId(row_id), gridCheckingDetail.getColIndexById("StockAdjAmount"), "color:red;");
    }
}
//End R2