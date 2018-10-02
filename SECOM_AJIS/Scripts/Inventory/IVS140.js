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
var IVS140 = {
    SelectedInstrumentList: [],
    ScreenMode: {
        Register: "RegisterMode",
        Confirm: "ConfirmMode",
        Finish: "FinishMode",
        NewRegister: "NewRegisterMode"
    }
};



$(document).ready(function () {

    // Initial grid 1
    if ($.find("#IVS140_SearchResultGrid").length > 0) {
        gridSearchResult = $("#IVS140_SearchResultGrid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Inventory/IVS140_InitialSearchResultGrid");

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
    }

    // Initial grid 2
    if ($.find("#IVS140_RetrundInstrumentCheckingDetailGrid").length > 0) {
        gridCheckingDetail = $("#IVS140_RetrundInstrumentCheckingDetailGrid").InitialGrid(0, false, "/Inventory/IVS140_InitialAdjustmentDetailGrid");

        //Binding 2. (adjustment detail)
        SpecialGridControl(gridCheckingDetail, ["FixedReturnQty", "BtnRemove"]);
        BindOnLoadedEvent(gridCheckingDetail, function () {
            var colInx = gridCheckingDetail.getColIndexById('BtnRemove');
            for (var i = 0; i < gridCheckingDetail.getRowsNum(); i++) {

                var rowId = gridCheckingDetail.getRowId(i);
                GenerateRemoveButton(gridCheckingDetail, "btnRemove", rowId, "BtnRemove", true);

                // binding grid button event 
                BindGridButtonClickEvent("btnRemove", rowId, RemoveInstrument);

                // generate numeric text box in grid
                var qtyColIndex = gridCheckingDetail.getColIndexById("FixedReturnQty");
                var val = GetValueFromLinkType(gridCheckingDetail, i, qtyColIndex);
                GenerateNumericBox2(gridCheckingDetail, "txtFixedReturnQty", rowId, "FixedReturnQty", val, 5, 0, 0, 99999, true, true);
            }

            gridCheckingDetail.setSizes();
        });

    }


    // Event binding
    $("#btnSearch").click(IVS140_Search);
    $("#btnClear").click(Clear);
    $("#btnDownloadSlip").click(DownloadSlip);
    $("#btnNewRegister").click(NewRegister);

    $("#InstrumentCode").InitialAutoComplete("/Master/GetInstrumentCodeAll", null, true);

    InitialPage();

});


//InitialPage
function InitialPage() {
    $("#Memo").SetMaxLengthTextArea(1000);

    // Hide adjust detail and download
    $("#divCheckingReturndDetail").hide();
    $("#divShowSlipNo").hide();
    $("#formSearchInstrument").clearForm();
    $("#formCheckingReturnd").clearForm();
    IVS140.SelectedInstrumentList = [];

    $("#IVS140_SearchResultGrid").show();
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
        var checkInx = search_array_index(IVS140.SelectedInstrumentList, "key", key);
        if (checkInx >= 0) {
            var param = { "module": "Inventory", "code": "MSG4005", "param": "" };
            ajax_method.CallScreenController("/Shared/GetMessage", param, function (data) {
                OpenWarningDialog(data.Message);
            });
            return;
        }

        data["key"] = key;
        IVS140.SelectedInstrumentList.push(data);

        // Show section Checking return instrument
        $("#divCheckingReturndDetail").show();

        // Show command [Register] [Reset]
        register_command.SetCommand(command_register_click);
        reset_command.SetCommand(command_reset_click);


        CheckFirstRowIsEmpty(gridCheckingDetail, true);
        AddNewRow(gridCheckingDetail, [ConvertBlockHtml(data.Instrumentcode), //data.Instrumentcode, //Modify by Jutarat A. on 28112013
                                       ConvertBlockHtml(data.InstrumentName), //data.InstrumentName, //Modify by Jutarat A. on 28112013
                                       data.AreaCodeName,
                                       data.InstrumentQty,
                                       "", // FixedReturnQty
                                       "", // TempSpan
                                       "", // BtnRemove
                                       strJson]); // ToJson

        var row_idx = gridCheckingDetail.getRowsNum() - 1;
        var row_id = gridCheckingDetail.getRowId(row_idx);

        var defaultVal = "";
        GenerateNumericBox2(gridCheckingDetail, "txtFixedReturnQty", row_id, "FixedReturnQty", defaultVal, 5, 0, 0, 99999, true, true);
        GenerateRemoveButton(gridCheckingDetail, "btnRemove", row_id, "BtnRemove", true);
        gridCheckingDetail.setSizes();

        BindGridButtonClickEvent("btnRemove", row_id, function (rid) {
            DeleteRow(gridCheckingDetail, rid);

            // delete from array
            var removedIdx = search_array_index(IVS140.SelectedInstrumentList, "key", key);
            IVS140.SelectedInstrumentList.splice(removedIdx, 1);

            //if (CheckFirstRowIsEmpty(gridCheckingDetail, false) == true) {
            //    register_command.EnableCommand(false);
            //}

        });



    }
}

// RemoveInstrument
function RemoveInstrument(rowId) {
    // ..

}

//Search
function IVS140_Search() {
    // formSearchInstrument

    // For prevent click this button more than one time
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    var obj = CreateObjectData($("#formSearchInstrument").serialize());
    $("#IVS140_SearchResultGrid").LoadDataToGrid(gridSearchResult, ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Inventory/IVS140_SearchResponse", obj, "dtSearchInstrumentListResult", false,
                    function () { // post-load
                        // For prevent click this button more than one time
                        $("#btnSearch").attr("disabled", false);

                        if (gridSearchResult.getRowsNum() > 0 && !CheckFirstRowIsEmpty(gridSearchResult, false)) {
                            reset_command.SetCommand(command_reset_click);
                        }

                        master_event.LockWindow(false);
                        master_event.ScrollWindow("#IVS140_SearchResultGrid");

                    },
                    function (result, controls, isWarning) { // pre-load
                        if (isWarning == undefined) {
                            $("#IVS140_SearchResultGrid").show();
                        }
                    });
}


// Clear
function Clear() {
    $("#formSearchInstrument").clearForm();
    $("#IVS140_SearchResultGrid").show();

    DeleteAllRow(gridSearchResult);

    CloseWarningDialog();
}

// DownloadSlip
function DownloadSlip() {

    ajax_method.CallScreenController("/Inventory/IVS140_CheckExistFile", "", function (result) {
        if (result == 1) {
            var key = ajax_method.GetKeyURL(null);
            var url = ajax_method.GenerateURL("/Inventory/IVS140_DownloadPdfAndWriteLog?k=" + key);
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
    SetScreenMode(IVS140.ScreenMode.NewRegister);
}

function ResetScreen() {
    register_command.SetCommand(null);  // ---- unset
    reset_command.SetCommand(null);     // ---- unset
    confirm_command.SetCommand(null);   // ---- unset
    back_command.SetCommand(null);      // ---- unset

    InitialPage();

    // Clear row
    DeleteAllRow(gridSearchResult);
    DeleteAllRow(gridCheckingDetail);

    $("#IVS140_SearchResultGrid").show();
}

function GetUserAdjustData() {

    var arr = new Array();
    var grid = gridCheckingDetail;

    if (CheckFirstRowIsEmpty(grid) == false) {
        for (var i = 0; i < grid.getRowsNum(); i++) {

            var row_id = grid.getRowId(i);
            var txtFixedReturnQty_id = GenerateGridControlID("txtFixedReturnQty", row_id);

            var strJson = grid.cells2(i, grid.getColIndexById("ToJson")).getValue();
            var data = JSON.parse(htmlDecode(strJson));

            var obj = {
                txtFixedReturnQtyID: txtFixedReturnQty_id,
                Instrumentcode: data.Instrumentcode,
                AreaCode: data.AreaCode,
                ShelfNo: data.ShelfNo,
                FixedReturnQty: $("#" + txtFixedReturnQty_id).NumericValue(),
                RowNo: i
            };

            arr.push(obj);
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

function SetScreenMode(mode) {
    if (mode == IVS140.ScreenMode.Register) {

        $("#divSearchInstrument").show();
        $("#divSearchInstrumentInner").show();
        $("#divCheckingReturndDetail").show();
        $("#divShowSlipNo").hide();

        register_command.SetCommand(command_register_click);
        reset_command.SetCommand(command_reset_click);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);

        $("#divCheckingReturndDetail").SetViewMode(false);
        $("#divSearchInstrument").SetViewMode(false);


        // Show delete button in grid
        var colBtnRemoveIdx = gridCheckingDetail.getColIndexById("BtnRemove");
        gridCheckingDetail.setColumnHidden(colBtnRemoveIdx, false);
        // Concept by P'Leing
        SetFitColumnForBackAction(gridCheckingDetail, "TempSpan");
    }
    else if (mode == IVS140.ScreenMode.Confirm) {

        $("#divSearchInstrument").show();
        $("#divSearchInstrumentInner").hide();
        $("#divCheckingReturndDetail").show();
        $("#divShowSlipNo").hide();

        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(command_confirm_click);
        back_command.SetCommand(command_back_click);

        $("#divCheckingReturndDetail").SetViewMode(true);
        $("#divSearchInstrument").SetViewMode(true);

        // Hide delete button in grid
        var colBtnRemoveIdx = gridCheckingDetail.getColIndexById("BtnRemove");
        gridCheckingDetail.setColumnHidden(colBtnRemoveIdx, true);
    }
    else if (mode == IVS140.ScreenMode.Finish) {

        $("#divSearchInstrument").show();
        $("#divSearchInstrumentInner").hide();
        $("#divCheckingReturndDetail").show();
        $("#divShowSlipNo").show();

        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);

        IVS140.SelectedInstrumentList = [];
    }
    else if (mode == IVS140.ScreenMode.NewRegister) {
        $("#divSearchInstrument").show();
        $("#divSearchInstrumentInner").show();
        $("#divCheckingReturndDetail").hide();
        $("#divShowSlipNo").hide();

        $("#IVS140_SearchResultGrid").show();

        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);

        $("#divCheckingReturndDetail").SetViewMode(false);
        $("#divSearchInstrument").SetViewMode(false);

        // Show delete button in grid
        var colBtnRemoveIdx = gridCheckingDetail.getColIndexById("BtnRemove");
        gridCheckingDetail.setColumnHidden(colBtnRemoveIdx, false);
        // Concept by P'Leing
        SetFitColumnForBackAction(gridCheckingDetail, "TempSpan");

        $("#formSearchInstrument").clearForm();
        $("#formCheckingReturnd").clearForm();
        IVS140.SelectedInstrumentList = [];

        // Clear row
        DeleteAllRow(gridSearchResult);
        DeleteAllRow(gridCheckingDetail);
    }
}

function IVS140_RefreshDetailData(data) {
    var grid = gridCheckingDetail;

    for (var i = 0; i < data.Detail.length; i++) {
        grid.cells2(data.Detail[i].RowNo, grid.getColIndexById("InstrumentQty")).setValue(data.Detail[i].InstrumentQty);
    }
}

function command_register_click() {
    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Inventory/IVS140_Register", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            IVS140_RefreshDetailData(result.Data);
            if (result.IsSuccess) {
                // goto confirm state
                SetScreenMode(IVS140.ScreenMode.Confirm);
            }
        }
        if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });
}

function command_confirm_click() {
    // Call ajax to save
    ajax_method.CallScreenController("/Inventory/IVS140_Confirm", "",
        function (result, controls, isWarning) {
            if (result != undefined) {
                IVS140_RefreshDetailData(result.Data);
                if (result.IsSuccess) {
                    // Set slip no to textbox
                    $("#SlipNo").val(result.SlipNo);
                    SetScreenMode(IVS140.ScreenMode.Finish);
                }
                else {
                    SetScreenMode(IVS140.ScreenMode.Register);
                }
            }
            if (controls != undefined) {
                // Validate again -> but not pass then set screen to input mode and hilight controls
                if (result == undefined) {
                    SetScreenMode(IVS140.ScreenMode.Register);
                }
                VaridateCtrl(controls, controls);
            }
        });


}

function command_reset_click() {
    ResetScreen();
}

function command_back_click() {
    SetScreenMode(IVS140.ScreenMode.Register);
}



