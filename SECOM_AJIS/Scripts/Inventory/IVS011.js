/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../Base/GridControl.js" />

/// <reference path="../Base/DateTimePicker.js" />
/// <reference path="../Base/control_events.js" />
var slipListGrid = null, SlipDetailGrid = null, StockinTypeHidden = null;

$(document).ready(function () {

    InitialDateFromToControl("#from", "#to");
    initScreen();
    initButton();
    initEvent();
});

function initScreen() {
    StockinTypeHidden = null;
    //  $("#StockInList").();
    $("#SlipDetailSection").hide();

    $("#memo").SetMaxLengthTextArea(1000);
    $("#Detmemo").SetMaxLengthTextArea(1000);
    $("#Detmemo").SetReadOnly(true);
    $("#SlipDetailSection input[type='text']").SetReadOnly(true);

    slipListGrid = $("#SlipGrid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS011_SlipList");
    SlipDetailGrid = $("#DetailGrid").InitialGrid(0, false, "/inventory/IVS011_SlipDetailList", function () { });
    BindOnLoadedEvent(slipListGrid, function (gen_ctrl) {
        for (var i = 0; i < slipListGrid.getRowsNum(); i++) {
            var row_id = slipListGrid.getRowId(i);

            if (gen_ctrl) {
                //var registerflag = slipListGrid.cells(row_id, slipListGrid.getColIndexById("RegisterAssetFlag")).getValue();
                //if (registerflag == AssetUnregistered) {
                var lockflag = slipListGrid.cells(row_id, slipListGrid.getColIndexById("LockFlag")).getValue();
                if (lockflag != true) {
                    var status = slipListGrid.cells(row_id, slipListGrid.getColIndexById("RegisterAssetName")).getValue();
                    var slipno = slipListGrid.cells(row_id, slipListGrid.getColIndexById("SlipNo")).getValue();
                    var tagRegister = "<a id='" + GenerateGridControlID("lnkRegisterAsset", row_id) + "' href='#'>" + status + "<input type='hidden' name='slipno' value='" + slipno + "'/></a>";
                    slipListGrid.cells(row_id, slipListGrid.getColIndexById("RegisterAssetName")).setValue(tagRegister);
                }
            }

            GenerateSelectButton(slipListGrid, "btnSelect", row_id, "Select", true);
            BindGridButtonClickEvent("btnSelect", row_id, function (rid) {
                var SlipNo = slipListGrid.cells(rid, slipListGrid.getColIndexById("SlipNo")).getValue();

                StockinTypeHidden = slipListGrid.cells(rid, slipListGrid.getColIndexById("StockInFlag")).getValue();

                call_ajax_method_json("/inventory/GetHeadSlipDetail", { SlipNo: SlipNo }, function (res) {
                    if (res != undefined) {
                        $("#SlipDetailSection").clearForm();
                        $("#SlipDetailSection").show();
                        if (SlipDetailGrid != null)
                            DeleteAllRow(SlipDetailGrid);

                        if (res.LockFlag == true)
                            $("#btnCancelSlip").SetDisabled(true);
                        else
                            $("#btnCancelSlip").SetDisabled(false);


                        $("#DetSlipNo").val(res.SlipNo);
                        $("#DetPorderNo").val(res.PurchaseOrderNo);
                        $("#DetstockType").val(res.StockInTypeCodeName);

                        $("#DetSupOrderNo").val(res.DeliveryOrderNo);
                        $("#DetStockDate").val(ConvertDateToTextFormat(ConvertDateObject(res.StockInDate)));
                        $("#DetRegAsset").val(res.RegisterAssetName);
                        $("#Detmemo").val(res.Memo);

                        $("#DetailGrid").LoadDataToGrid(SlipDetailGrid, 0, false, "/inventory/GetSlipDetail", { SlipNo: SlipNo }, "doInventorySlipDetailList", false, null, function () {
                            $("#DetailGrid").show();
                            $("#DetailGrid").focus();
                        });

                    }
                    else {
                        $("#SlipDetailSection").hide();
                    }
                });

            });
        }

        $("a[id^=lnkRegisterAsset]")
        .css("color", "blue")
        .unbind("click")
        .click(function () {
            var obj = {
                PreloadSlipNo: $(this).children("input:hidden[name=slipno]").val()
            };
            ajax_method.CallScreenControllerWithAuthority("/Inventory/IVS012", obj, true);
        });


        slipListGrid.setSizes();
    });
    SpecialGridControl(slipListGrid, ["Select"]);
}
function cancelSlip() {
    var InstList = new Array();
    for (var i = 0; i < SlipDetailGrid.getRowsNum(); i++) {
        var row_id = SlipDetailGrid.getRowId(i);
        var Inst = {
            InstrumentCode: SlipDetailGrid.cells(row_id, SlipDetailGrid.getColIndexById("InstrumentCode")).getValue(),
            InstrumentName: SlipDetailGrid.cells(row_id, SlipDetailGrid.getColIndexById("InstrumentName")).getValue(),
            TransferQty: SlipDetailGrid.cells(row_id, SlipDetailGrid.getColIndexById("TransferQty")).getValue(),
            SourceAreaCode: SlipDetailGrid.cells(row_id, SlipDetailGrid.getColIndexById("SourceAreaCode")).getValue(),
            AreaCodeName: SlipDetailGrid.cells(row_id, SlipDetailGrid.getColIndexById("AreaCodeName")).getValue(),
            row_id: row_id
        };
        InstList.push(Inst);
    }
    var obj = {
        SlipNo: $.trim($("#DetSlipNo").val()),
        strStockInType: StockinTypeHidden,
        pOrderNo: $.trim($("#DetPorderNo").val()),
        stockInDate: $.trim($("#DetStockDate").val()),
        InstrumentList: InstList
    }

    call_ajax_method_json("/inventory/CancelSlip", obj, function (res, controls) {
        if (res) {
            var obj = {
                module: "common",
                code: "MSG0108"
            };
            call_ajax_method("/Shared/GetMessage", obj, function (result) {
                OpenInformationMessageDialog(result.Code, result.Message, function () {
                    $("#SlipDetailSection").hide();
                    DeleteAllRow(slipListGrid);
                    $("#Criteria").clearForm();
                    $("#DetailGrid").hide();
                    $("#SlipDetailSection").hide();
                    $("#headSlipDetail").clearForm();
                    $("#btnSearch").SetDisabled(false);
                    StockinTypeHidden = null;
                });
            });
        }

        if (controls != undefined) {
            for(var i = 0; i < controls.length; i ++)
            {
                SlipDetailGrid.selectRowById(controls[i]);
            }
        }

    });


}
function initButton() {


    $("#btnCancelSlip").click(function () {
        var obj = {
            module: "inventory",
            code: "MSG4043"
        };
        call_ajax_method("/Shared/GetMessage", obj, function (result) {
            OpenYesNoMessageDialog(result.Code, result.Message, cancelSlip, null);
        });
    });
    $("#btnSearch").click(function () {
        master_event.LockWindow(true);
        $("#btnSearch").SetDisabled(true);
        var obj = {
            SlipNo: $.trim($("#SlipNo").val()),
            PurchaseOrderNo: $.trim($("#PorderNo").val()),
            StockInFlag: $.trim($("#StockType").val()),
            DeliveryOrderNo: $.trim($("#SupOrderNo").val()),
            StockInDateFrom: $.trim($("#from").val()),
            StockInDateTo: $.trim($("#to").val()),
            RegisterAssetFlag: $.trim($("#RegAsset").val()),
            Memo: $.trim($("#memo").val())
        };

        $("#SlipGrid").LoadDataToGrid(slipListGrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/SearchStockSlip", obj, "doInventorySlipList", false, null, function (res) {
            master_event.ScrollWindow("#ResultSection");
        });

        $("#btnSearch").SetDisabled(false);
        master_event.LockWindow(false);

        $("#StockInList").show();






    });

    $("#btnClear").click(function () {

        $("#Criteria").clearForm();
        DeleteAllRow(slipListGrid);
        $("#DetailGrid").hide();
        //$("#StockInList").hide();
        $("#SlipDetailSection").hide();
        $("#headSlipDetail").clearForm();
        $("#btnSearch").SetDisabled(false);
        StockinTypeHidden = null;
        ClearDateFromToControl("#from", "#to");
    });

}


function initEvent() {
}