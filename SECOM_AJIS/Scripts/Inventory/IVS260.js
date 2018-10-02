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
/// <reference path="../Base/object/command_event.js" />


var IVS260_DownloadSlipInfo = null;
var PorderGrid = null;
var InstGrid = null;
var dtNewInstrument = null;

var calRowId = 0;
var strPurchaseOrderNo = null;
var po_RowID = 0;
var LastPorderRowId = 0;
var btnCancel = "BtnCancel";
var ModifyBox = "ModifyBox";
var btnSelect = "btnSelect";
var UpdateDate = null;
var selected_porder = false;

//Add by Jutarat A. on 29102013
var isTotalRow = false;
var _TTLRowId = null;
var _TTLBeforeVatRowId = null;
var _VatRowId = null;
var lvlVatTotal = "vatTotal";
var OriginalUnitPrice = "OriginalUnitPriceBox";
var UnitPrice = "UnitPriceBox";
var OrderQty = "OrderQtyBox";
var cboUnit = "cboUnit";
var _isAddedInstrument = false;
var _VatTotalText = "";
var PurchaseOrderStatus = "";
//End Add

var _DiscountRowId = null;
var lblDiscount = "discount";

var _WHTRowId = null;
var lblWHTTotal = "wht";
var _WHTTotalText = null;

var _TTLFinalRowId = null;
var _btnCancelRowId = null;

$(document).ready(function () {

    InitialDateFromToControl("#POIssueDateFrom", "#POIssueDateTo");
    InitialDateFromToControl("#SearchExpectedDeliveryDateFrom", "#SearchExpectedDeliveryDateTo");

    initScreen();
    initGrid();
    initButton();
    initEvent();
});
function initEvent() {
    $("#InstrumentCode").InitialAutoComplete("/Master/GetInstrumentCodeAll", null, true, "150px");

    $("#InstrumentCode").on("autocompleteclose", function (event, ui) {
        if (!$(this).is(":focus")) {
            GetInstrumentData($("#InstrumentCode"));
        }
    });

    $("#InstrumentCode").on("autocompleteselect", function (event, ui) {
        $("#InstrumentCode").val(ui.item.value);
    });

    $("#InstrumentCode").keyup(function (event) {
        if (event.keyCode == 13) {
            GetInstrumentData($("#InstrumentCode"));
        }
    });

    $("#InstrumentCode").blur(function () {
        if (!$(this).autocomplete('widget').is(':visible')) {
            GetInstrumentData($(this));
        }
    });

    function GetInstrumentData(ctrl) {
        var InstCode = $.trim($(ctrl).val());

        if (InstCode != '') {
            ajax_method.CallScreenController('/inventory/IVS250_getInstrumentName', { 'InstrumentCode': InstCode }, function (data) {
                if (data != undefined && $.trim(data.InstrumentName) != '') {
                    dtNewInstrument = data;
                    $("#InstrumentCode").val(dtNewInstrument.InstrumentCode);
                    $("#InstrumentName").val(dtNewInstrument.InstrumentName);
                } else {
                    dtNewInstrument = null;
                    $("#InstrumentCode").val('');
                    $("#InstrumentName").val('');
                }
            });
            $(ctrl).val(InstCode);
        } else {
            $("#InstrumentName").val('');
            $(ctrl).val(InstCode);
            dtNewInstrument = null;
        }
    }

    $("#DTPurchaseOrderType").change(function () {
        CloseWarningDialog();
        VaridateCtrl(["DTPurchaseOrderType", "DTTransportType", "Currency"], null);

        if ($("#DTPurchaseOrderType").val() == C_PURCHASE_ORDER_TYPE_DOMESTIC) {
            $("#DTTransportType").prop("selectedIndex", 0)
            $("#Currency").val('1');
        } else {
            $("#Currency").prop("selectedIndex", 0)
        }

        CheckDisabledPurchaseOrderTypeChange();
    });

    $("#Currency").change(function () {
        if (InstGrid != null && _VatRowId != null) {
            if ($("#Currency").val() == c_currency_THB) {
                CalculateVatAmount(null, false, true);
                InstGrid.setRowHidden(_TTLBeforeVatRowId, true);
                InstGrid.setRowHidden(_VatRowId, true);
            }
            else {
                $("#" + GenerateGridControlID(lvlVatTotal, _VatRowId)).val("0.00");
                InstGrid.cells(_VatRowId, InstGrid.getColIndexById("DetailAmount")).setAttribute("title", "0.00");
                CalculateVatAmount(null, false, false);
                InstGrid.setRowHidden(_TTLBeforeVatRowId, true);
                InstGrid.setRowHidden(_VatRowId, true);
            }
        }
    });
    //End Add
}

//Add by Jutarat A. on 04112013
function CheckDisabledPurchaseOrderTypeChange() {
    if ($("#DTPurchaseOrderType").val() == C_PURCHASE_ORDER_TYPE_DOMESTIC) {
        $("#DTTransportType").SetDisabled(true);
        $("#Currency").SetDisabled(true);

        if (InstGrid != null && _VatRowId != 0) {
            //InstGrid.setRowHidden(_TTLBeforeVatRowId, $("#Currency").val() != c_currency_THB);
            //InstGrid.setRowHidden(_VatRowId, $("#Currency").val() != c_currency_THB);
            InstGrid.setRowHidden(_TTLBeforeVatRowId, true);
            InstGrid.setRowHidden(_VatRowId, true);
            CalculateVatAmount(null, false, false);
        }
    } else {
        $("#DTTransportType").SetDisabled(false);
        $("#Currency").SetDisabled(false);

        if (InstGrid != null && _VatRowId != 0) {
            //InstGrid.setRowHidden(_TTLBeforeVatRowId, $("#Currency").val() != c_currency_THB);
            //InstGrid.setRowHidden(_VatRowId, $("#Currency").val() != c_currency_THB);
            InstGrid.setRowHidden(_TTLBeforeVatRowId, true);
            InstGrid.setRowHidden(_VatRowId, true);
            CalculateVatAmount(null, false, false);
        }
    }
}
//End Add

function GenerateHtmlButton2(id, row_id, value, widthPX, endable) {
    /// <summary>Method to create Html Button</summary>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="value" type="string">value</param>
    /// <param name="enable" type="bool">enable flag</param>
    var disabled_txt = "";
    if (endable == false)
        disabled_txt = "disabled='disabled'";

    var fid = GenerateGridControlID(id, row_id);
    return "<button id='" + fid + "' name='" + fid + "' style='width:" + widthPX + "px;margin-left:-2px;'" + disabled_txt + " >" + value + "</button>";
}
function initButton() {
    $("#btnSearch").click(IVS260_Search);

    $("#btnClear").click(function () {
        $("#SearchPurchaseOrder").clearForm();
        $("#SearchPurchaseOrder input").SetDisabled(false);
        $("#SearchPurchaseOrder select").SetDisabled(false);
        $("#btnSearch").SetDisabled(false);
        //PorderGrid = null;
        //initScreen();

        $("#POIssueDateFrom").EnableDatePicker(true);
        $("#POIssueDateTo").EnableDatePicker(true);
        $("#SearchExpectedDeliveryDateFrom").EnableDatePicker(true);
        $("#SearchExpectedDeliveryDateTo").EnableDatePicker(true);
        
        ClearDateFromToControl("#POIssueDateFrom", "#POIssueDateTo");
        ClearDateFromToControl("#SearchExpectedDeliveryDateFrom", "#SearchExpectedDeliveryDateTo");

        // R2
        if (PorderGrid != null) {
            selected_porder = false;
            DeleteAllRow(PorderGrid);
            //PorderGrid.sortRows(PorderGrid.getColIndexById("PurchaseOrderNo"), "str", "asc");
        }
        //End R2

        //Add by Jutarat A. on 04112013
        $("#MaintainPurchaseOrder").clearForm(); 
        $("#MaintainPurchaseOrder").hide();

        if (InstGrid != null)
            DeleteAllRow(InstGrid);
        //End Add

        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    });

    $("#btnDownload").click(function () {
        var param = {
            strPurchaseOrderNo: IVS260_DownloadSlipInfo.PurchaseOrderNo,
            strReportID: IVS260_DownloadSlipInfo.ReportID
        };

        ajax_method.CallScreenController("/inventory/IVS260_DownloadDocument", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/inventory/IVS260_DownloadPdfAndWriteLog?k=" + key + "&strDocumentNo=" + result.DocumentNo + "&documentOCC=" + result.DocumentOCC + "&strDocumentCode=" + result.DocumentCode + "&fileName=" + result.FilePath)
                window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnNewRegister").click(function () {
        $("#IVS260PAGE").SetViewMode(false);
        initScreen();
    });

    //Add by Jutarat A. on 30102013
    $('#btnSearchInstrument').click(function () {
        $('#dlgBox').OpenCMS170Dialog("IVS260");
    });

    $("#btnCancel").click(btnCancelClick);
    //End Add

    //Add by Jutarat A. on 01112013
    $("#btnAdd").click(function () {
        master_event.LockWindow(true);
        var obj = { InstrumentCode: $("#InstrumentCode").val()
                , InstrumentName: $("#InstrumentName").val()
                , OriginalUnitPrice: $("#UnitPrice").NumericValue()
                , UnitPrice: $("#UnitPrice").NumericValue()
                , OrderQty: $("#OrderQty").NumericValue()
                , Unit: $("#Unit").val()
                , dtNewInstrument: dtNewInstrument
                , InstrumentData: getGridData()
        };

        ajax_method.CallScreenController("/inventory/IVS260_ValidateAddInst", obj, function (result, controls) {
            if (controls != undefined) VaridateCtrl(["InstrumentCode", "UnitPrice", "OrderQty", "Unit"], controls);
            if (result != null) {
                var bIsEmptyGrid = CheckFirstRowIsEmpty(InstGrid, true);

                //#########################################################################################
                // Add new detail row.
                //#########################################################################################
                //var row_idx = AddNewDetailRow(InstGrid, [result.InstrumentCode, result.InstrumentName, "", "", "0", "", "0", result.OrderQty, result.Amount_view, "", ""]);
                var row_idx = AddNewDetailRow(InstGrid, [ConvertBlockHtml(result.InstrumentCode), ConvertBlockHtml(result.InstrumentName), "", "", "0", "", "0", result.OrderQty, result.Unit, result.Amount_view, "", ""]); //Modify by Jutarat A. on 29112013

                var row_id = InstGrid.getRowId(row_idx);

                if (result.UnitPrice != null && ((result.UnitPrice).toString().indexOf('.', 0) == -1)) {
                    result.UnitPrice = result.UnitPrice.toString() + ".000";
                }

                InstGrid.cells(row_id, InstGrid.getColIndexById("DetailAmount")).setAttribute("title", result.Amount_view);

                GenerateTextBox(InstGrid, "InstrumentName", row_id, "InstrumentName", result.InstrumentName, true, 100);
                //GenerateTextBox(InstGrid, "Memo", row_id, "Memo", "", true, 1000);

                GenerateGridCombobox(InstGrid, row_id, cboUnit, "Unit", "/Inventory/IVS260_GetUnit", result.Unit, true);

                GenerateNumericBox2(InstGrid, OriginalUnitPrice, row_id, "OriginalUnitPrice", result.UnitPrice, 10, 3, 0, 9999999999.999, true, true);
                GenerateNumericBox2(InstGrid, UnitPrice, row_id, "UnitPrice", result.UnitPrice, 10, 3, 0, 9999999999.999, true, true);
                GenerateNumericBox2(InstGrid, ModifyBox, row_id, "ModifyOrderQty", result.OrderQty, 5, 0, "", 99999, true, true);
                GenerateRemoveButton(InstGrid, "btnRemove", row_id, "Remove", true);

                InstGrid.cells(row_id, InstGrid.getColIndexById("IsShowRemove")).setValue(true);

                BindGridButtonClickEvent("btnRemove", row_id, function (rid) {

                    DeleteRow(InstGrid, rid);

                    if (getDetailRowsNum() <= 0) {
                        DeleteRow(InstGrid, _TTLRowId);
                        DeleteRow(InstGrid, _DiscountRowId);
                        DeleteRow(InstGrid, _TTLBeforeVatRowId);
                        DeleteRow(InstGrid, _VatRowId);
                        DeleteRow(InstGrid, _WHTRowId);
                        DeleteRow(InstGrid, _TTLFinalRowId);
                        DeleteRow(InstGrid, _btnCancelRowId);

                        _TTLRowId = null;
                        _DiscountRowId = null;
                        _TTLBeforeVatRowId = null;
                        _WHTRowId = null;
                        _VatRowId = null;
                        _TTLFinalRowId = null;
                        _btnCancelRowId = null;

                        _isAddedInstrument = false;
                        SetRegisterCommand(false, null);

                    } else {
                        CalculateDiscountAmount();
                    }
                }); // End btnRemove Click  

                var orderID = "#" + GenerateGridControlID(ModifyBox, row_id);
                $(orderID).attr("name", row_id);
                $(orderID).change(function () {
                    var rowOrder_id = $(this).attr("name");

                    if (+$(this).NumericValue() <= 0) {
                        master_event.LockWindow(true);
                        var obj = {
                            module: "inventory",
                            code: "MSG4020"
                        };
                        call_ajax_method("/Shared/GetMessage", obj, function (result) {
                            master_event.LockWindow(false);
                            OpenErrorMessageDialog(result.Code, result.Message, function () { $(this).focus(); }, null);
                        });
                    }

                    CalculateDiscountAmount();

                    if (PurchaseOrderStatus == WaitToReceive) {
                        var decOrderQty = $("#" + GenerateGridControlID(ModifyBox, rowOrder_id)).NumericValue();
                        if (decOrderQty != null && decOrderQty != "")
                            InstGrid.cells(rowOrder_id, InstGrid.getColIndexById("RemainQty")).setValue(decOrderQty);
                    }
                });

                var OriginalUnitPriceID = "#" + GenerateGridControlID(OriginalUnitPrice, row_id);
                $(OriginalUnitPriceID).attr("name", row_id);
                $(OriginalUnitPriceID).change(function () {
                    CalculateDiscountAmount();
                });

                var UnitPriceID = "#" + GenerateGridControlID(UnitPrice, row_id);
                $(UnitPriceID).attr("name", row_id);
                $(UnitPriceID).change(function () {
                    CalculateDiscountAmount();
                });

                var UnitID = "#" + GenerateGridControlID(cboUnit, row_id);
                $(UnitID).attr("name", row_id);
                $(UnitID).blur(function () {
                    if (!$(UnitID).val()) {
                        master_event.LockWindow(true);
                        var obj = {
                            module: "inventory",
                            code: "MSG4145"
                        };
                        call_ajax_method("/Shared/GetMessage", obj, function (result) {
                            master_event.LockWindow(false);
                            OpenErrorMessageDialog(result.Code, result.Message, function () { $(UnitID).focus(); }, null);
                        });
                    }
                })
                .change(function () {
                    var row_id = $(this).attr("name");
                    var value = $(this).val();
                    InstGrid.cells(row_id, InstGrid.getColIndexById("Unit")).setAttribute("title", value);
                });

                CalculateTotalAmount(false, true);

                InstGrid.setSizes();
                ClearInstrumentSection();

                _isAddedInstrument = true;

            } // End if (result != null)
            master_event.LockWindow(false);
        }); // IVS250_ValidateAddInst        
    });                     
    // End Add
}

//Add by Jutarat A. on 01112013
function AddNewDetailRow(grid, data) {
    /// <summary>Method to add new row to grid</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="data" type="Array">Data in each column</param>
    /// <return>RowId of new row.</return>
    var NewRowNo = getDetailRowsNum();
    if (NewRowNo < 0) {
        NewRowNo = 0;
    }
    grid.addRow(grid.uid(), data, NewRowNo);
    grid.setSizes();

    return NewRowNo;
}
//End Add

//Add by Jutarat A. on 30102013
function CMS170Response(dtNewInst) {

    $("#dlgBox").CloseDialog();
    $("#InstrumentCode").val(dtNewInst.InstrumentCode);

    dtNewInstrument = dtNewInst;

    if (dtNewInst.InstrumentCode != '') {
        ajax_method.CallScreenController('/inventory/IVS250_getInstrumentName', { 'InstrumentCode': dtNewInst.InstrumentCode }, function (data) {
            if (data != undefined && $.trim(data.InstrumentName) != '') {
                $("#InstrumentCode").val(data.InstrumentCode);
                $("#InstrumentName").val(data.InstrumentName);
                $("#UnitPrice").focus();
            } else {
                $("#InstrumentCode").val('');
                $("#InstrumentName").val('');
            }
        });
    }

}

function CMS170Object() {
    return { bExpTypeHas: true,
        bExpTypeNo: true,
        bProdTypeSale: false,
        bProdTypeAlarm: false,
        bInstTypeGen: true,
        bInstTypeMonitoring: false,
        bInstTypeMat: true
    };
}

function btnCancelClick() {
    $("#InstrumentCode").val("");
    $("#InstrumentName").val("");
    $("#UnitPrice").val("");
    $("#OrderQty").val("");
    $("#Unit").val("PCS");
}
//End Add

function IVS260_Search() {
    master_event.LockWindow(true);
    $("#btnSearch").SetDisabled(true);
    var obj = { PurchaseOrderNo: $("#PurchaseOrderNo").val(),
        PurchaseOrderStatus: $("#PurchaseOrderStatus").val(),
        SupplierCode: $("#SupplierCode").val(),
        SupplierName: $("#SupplierName").val(),
        TransportType: $("#TransportType").val(),
        POIssueDateFrom: $("#POIssueDateFrom").val(),
        POIssueDateTo: $("#POIssueDateTo").val(),
        SearchInstrumentCode: $("#SearchInstrumentCode").val(),
        SearchExpectedDeliveryDateFrom: $("#SearchExpectedDeliveryDateFrom").val(),
        SearchExpectedDeliveryDateTo: $("#SearchExpectedDeliveryDateTo").val()
    };

    $("#POrderGrid").LoadDataToGrid(PorderGrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS260_SearchPurcahseOrder", { Cond: obj }, "doPurchaseOrder", false,
        function (result, controls) {
            if (controls != undefined || result == null) {
                VaridateCtrl_AtLeast(["PurchaseOrderNo", "PurchaseOrderStatus", "SupplierCode", "SupplierName", "TransportType", "POIssueDateFrom", "POIssueDateTo", "SearchInstrumentCode", "SearchExpectedDeliveryDateFrom", "SearchExpectedDeliveryDateTo"], controls);
                $("#btnSearch").SetDisabled(false);
                master_event.LockWindow(false);
            }
        },

         function (result, controls, isWarning) {
             if (isWarning == undefined) {
                 master_event.ScrollWindow("#ResultSection");
                 $("#SearchPurchaseOrder input").SetDisabled(true);
                 $("#SearchPurchaseOrder select").SetDisabled(true);
                 $("#btnSearch").SetDisabled(true);

                 $("#POIssueDateFrom").EnableDatePicker(false);
                 $("#POIssueDateTo").EnableDatePicker(false);
                 $("#SearchExpectedDeliveryDateFrom").EnableDatePicker(false);
                 $("#SearchExpectedDeliveryDateTo").EnableDatePicker(false);

                 master_event.LockWindow(false);
             }
         });
}


function btnCancel_OnClick() {
    // JS Call Message

    var obj = {
        module: "Inventory",
        code: "MSG4027"
    };
    call_ajax_method("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
        function () {

            call_ajax_method_json("/inventory/IVS260_CancelPurchaseOrder", { PurchaseOrderNo: $("#DTPurchaseOrderNo").val() }, function (result, controls) {
                if (result) {
                    var obj = {
                        module: "Common",
                        code: "MSG0108"
                    };
                    call_ajax_method("/Shared/GetMessage", obj, function (result) {
                        OpenInformationMessageDialog(result.Code, result.Message, function () {

                            $("#MaintainPurchaseOrder").clearForm();
                            $("#MaintainPurchaseOrder").hide();

                            DeleteAllRow(InstGrid);
                            DeleteRow(PorderGrid, po_RowID);
                            po_RowID = 0;
                        }, null);
                    });
                }
            });
        },
        null);
    });
}
function initScreen() {

    //Add by Jutarat A. on 30102013
    _isAddedInstrument = false;

    _TTLRowId = null;
    _DiscountRowId = null;
    _TTLBeforeVatRowId = null;
    _WHTRowId = null;
    _VatRowId = null;
    _TTLFinalRowId = null;
    _btnCancelRowId = null;

    $("#AdjustDueDate").InitialDate(); 
    $("#Memo").SetMaxLengthTextArea(1000);

    $("#UnitPrice").BindNumericBox(10, 3, 0, 9999999999.999);
    $("#OrderQty").BindNumericBox(5, 0, 0, 99999);
    //End Add

    $("#Unit").val("PCS");

    ClearDateFromToControl("#POIssueDateFrom", "#POIssueDateTo");
    ClearDateFromToControl("#SearchExpectedDeliveryDateFrom", "#SearchExpectedDeliveryDateTo");

    if (PorderGrid != null) {
        DeleteAllRow(PorderGrid);
        PorderGrid.sortRows(PorderGrid.getColIndexById("PurchaseOrderNo"), "str", "asc");
    }

    IVS260_DownloadSlipInfo = null;

    call_ajax_method_json("/inventory/IVS260_InitParam", "", function (result, controls) {
        UpdateDate = null;
        selected_porder = false;
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);

        $("#SearchPurchaseOrder").show();

        strPurchaseOrderNo = null;
        $("#SearchPurchaseOrder").clearForm();
        $("#SearchPurchaseOrder input").SetDisabled(false);
        $("#SearchPurchaseOrder select").SetDisabled(false);
        $("#btnSearch").SetDisabled(false);

        $("#POIssueDateFrom").EnableDatePicker(true);
        $("#POIssueDateTo").EnableDatePicker(true);
        $("#SearchExpectedDeliveryDateFrom").EnableDatePicker(true);
        $("#SearchExpectedDeliveryDateTo").EnableDatePicker(true);

        $("#MaintainPurchaseOrder").hide();
        $("#divShowPurchaseOrder").hide();
        $("#SupplierName").InitialAutoComplete("/Master/GetSupplierName");
        //    $("#AdjustDueDate").InitialDate();

        $("#txtShowPurchaseOrderNo").SetDisabled(true);
    });
}

var isEmpty = false;

function initGrid() {
    PorderGrid = $("#POrderGrid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS260_intiPorderGrid");
    SpecialGridControl(PorderGrid, ["Select"]);

    // Akat K. : Not lock grid after selected
//    PorderGrid.attachEvent("onBeforeSelect", function (new_row, old_row) {
//        return !(selected_porder);
//    });
//    PorderGrid.attachEvent("onBeforeSorting", function (ind, type, direction) {
//        return !(selected_porder);
//    });
//    PorderGrid.attachEvent("onBeforePageChanged", function (ind, count) {
//        return !(selected_porder);
//    });

    BindOnLoadedEvent(PorderGrid, function (gen_ctrl) {
        for (var i = 0; i < PorderGrid.getRowsNum(); i++) {
            var row_id = PorderGrid.getRowId(i);
            LastPorderRowId = row_id;
            if (gen_ctrl == true) {
                GenerateSelectButton(PorderGrid, btnSelect, row_id, "Select", true);

                var pono = PorderGrid.cells(row_id, PorderGrid.getColIndexById("PurchaseOrderNo")).getValue();
                var link = $("<a></a>");
                link.attr("name", "PurchaseOrderNoLink");
                link.attr("href", "#");
                link.text(pono);
                var linkHtml = $("<p></p>").append(link).html();
                PorderGrid.cells(row_id, PorderGrid.getColIndexById("PurchaseOrderNoLink")).setValue(linkHtml);
            }

            BindGridButtonClickEvent(btnSelect, row_id, function (rid) {
                PorderGrid.selectRow(PorderGrid.getRowIndex(rid));
                po_RowID = rid;
                UpdateDate = PorderGrid.cells(rid, PorderGrid.getColIndexById("UpdateDate")).getValue();
                var POrderNo = PorderGrid.cells(rid, PorderGrid.getColIndexById("PurchaseOrderNo")).getValue();
                strPurchaseOrderNo = POrderNo;
                $("#MaintainPurchaseOrder").show();
                $("#InstInput").show(); //Add by Jutarat A. on 19022014
                $("#InstGrid").LoadDataToGrid(InstGrid, 0, false, "/inventory/IVS260_GetPurchaseOrderDetail", { PurchaseOrderNo: POrderNo }, "doPurchaseOrderDetail", false, function () {

                    //-------------------------------------------- ----------------------------------------------------

                    //Add by Jutarat A. on 01112013
                    if (_isAddedInstrument == false) {
                        var TTLRowNum = 0;
                        var TTLRowId = 0;
                        AddNewRow(InstGrid, ["Before Discount", "", "", "", "", "", "", "", "", "", ""]);
                        TTLRowNum = InstGrid.getRowsNum() - 1;
                        TTLRowId = InstGrid.getRowId(TTLRowNum);
                        InstGrid.setColspan(TTLRowId, 0, 8);
                        InstGrid.cells(TTLRowId, InstGrid.getColIndexById("InstrumentCode")).cell.style.textAlign = 'right';
                        _TTLRowId = TTLRowId;

                        var DiscountRowNum = 0;
                        var DiscountRowId = 0;
                        AddNewRow(InstGrid, ["Discount", "", "", "", "", "", "", "", "", "", ""]);
                        DiscountRowNum = InstGrid.getRowsNum() - 1;
                        DiscountRowId = InstGrid.getRowId(DiscountRowNum);
                        GenerateNumericBox2(InstGrid, lblDiscount, DiscountRowId, "DetailAmount", "", 14, 2, 0, 99999999999999.99, true, true);
                        InstGrid.setColspan(DiscountRowId, 0, 8);
                        InstGrid.cells(DiscountRowId, InstGrid.getColIndexById("InstrumentCode")).cell.style.textAlign = 'right';
                        _DiscountRowId = DiscountRowId;
                        var discountAmount = +(PorderGrid.cells(po_RowID, PorderGrid.getColIndexById("Discount")).getValue());
                        $("#" + GenerateGridControlID(lblDiscount, _DiscountRowId))
                            .val(SetNumericText(discountAmount, 2))
                            .change(function () {
                                $(this).removeClass("highlight");
                                var value = +$(this).NumericValue();
                                var valueText = SetNumericText(value, 2);
                                InstGrid.cells(_DiscountRowId, InstGrid.getColIndexById("DetailAmount")).setAttribute("title", valueText);
                                CalculateDiscountAmount();
                            });
                        InstGrid.cells(_DiscountRowId, InstGrid.getColIndexById("DetailAmount")).setAttribute("title", SetNumericText(discountAmount, 2));

                        GenerateCalculateButton(InstGrid, "btnCalculateDiscount", _DiscountRowId, "Remove", true);

                        BindGridButtonClickEvent("btnCalculateDiscount", _DiscountRowId, function (rid) {
                            CalculateDiscountAmount();
                            CalculateVatAmount(null, true, false);
                        });

                        var TTLBeforeVatRowNum = 0;
                        var TTLBeforeVatRowId = 0;
                        AddNewRow(InstGrid, ["Before VAT", "", "", "", "", "", "", "", "", "", ""]);
                        TTLBeforeVatRowNum = InstGrid.getRowsNum() - 1;
                        TTLBeforeVatRowId = InstGrid.getRowId(TTLBeforeVatRowNum);
                        InstGrid.setColspan(TTLBeforeVatRowId, 0, 8);
                        InstGrid.cells(TTLBeforeVatRowId, InstGrid.getColIndexById("InstrumentCode")).cell.style.textAlign = 'right';
                        _TTLBeforeVatRowId = TTLBeforeVatRowId;

                        var WHTRowNum = 0;
                        var WHTRowId = 0;
                        AddNewRow(InstGrid, ["Withholding Tax", "", "", "", "", "", "", "", "", "", ""]);
                        WHTRowNum = InstGrid.getRowsNum() - 1;
                        WHTRowId = InstGrid.getRowId(WHTRowNum);
                        GenerateNumericBox2(InstGrid, lblWHTTotal, WHTRowId, "DetailAmount", "", 14, 2, 0, 99999999999999.99, true, true);
                        InstGrid.setColspan(WHTRowId, 0, 8);
                        InstGrid.cells(WHTRowId, InstGrid.getColIndexById("InstrumentCode")).cell.style.textAlign = 'right';
                        _WHTRowId = WHTRowId;
                        var whtAmount = +(PorderGrid.cells(po_RowID, PorderGrid.getColIndexById("WHT")).getValue());
                        $("#" + GenerateGridControlID(lblWHTTotal, WHTRowId))
                            .val(SetNumericText(whtAmount, 2))
                            .change(function () {
                                $(this).removeClass("highlight");
                                var value = +$(this).NumericValue();
                                var valueText = SetNumericText(value, 2);
                                InstGrid.cells(_WHTRowId, InstGrid.getColIndexById("DetailAmount")).setAttribute("title", valueText);
                                CalculateDiscountAmount();
                            });
                        InstGrid.cells(_WHTRowId, InstGrid.getColIndexById("DetailAmount")).setAttribute("title", SetNumericText(whtAmount, 2));

                        GenerateCalculateButton(InstGrid, "btnCalculate", WHTRowId, "Remove", true);

                        BindGridButtonClickEvent("btnCalculate", WHTRowId, function (rid) {
                            CalculateVatAmount(null, true, false);
                        });

                        var VatRowNum = 0;
                        var VatRowId = 0;
                        AddNewRow(InstGrid, ["Vat", "", "", "", "", "", "", "", "", "", ""]);
                        VatRowNum = InstGrid.getRowsNum() - 1;
                        VatRowId = InstGrid.getRowId(VatRowNum);
                        GenerateNumericBox2(InstGrid, lvlVatTotal, VatRowId, "DetailAmount", "", 14, 2, 0, 99999999999999.99, true, true);
                        InstGrid.setColspan(VatRowId, 0, 8);
                        InstGrid.cells(VatRowId, InstGrid.getColIndexById("InstrumentCode")).cell.style.textAlign = 'right';
                        _VatRowId = VatRowId;
                        //var vatAmount = +(PorderGrid.cells(po_RowID, PorderGrid.getColIndexById("Vat")).getValue());
                        var vatAmount = 0;
                        $("#" + GenerateGridControlID(lvlVatTotal, _VatRowId))
                            .val(SetNumericText(vatAmount, 2))
                            .change(function () {
                                $(this).removeClass("highlight");
                                var value = +$(this).NumericValue();
                                var valueText = SetNumericText(value, 2);
                                InstGrid.cells(_VatRowId, InstGrid.getColIndexById("DetailAmount")).setAttribute("title", valueText);
                                CalculateVatAmount(null, false, false);
                            });
                        InstGrid.cells(_VatRowId, InstGrid.getColIndexById("DetailAmount")).setAttribute("title", SetNumericText(vatAmount, 2));

                        var TTLFinalRowNum = 0;
                        var TTLFinalRowId = 0;
                        AddNewRow(InstGrid, ["Total Amount", "", "", "", "", "", "", "", "", "", ""]);
                        TTLFinalRowNum = InstGrid.getRowsNum() - 1;
                        TTLFinalRowId = InstGrid.getRowId(TTLFinalRowNum);
                        InstGrid.setColspan(TTLFinalRowId, 0, 8);
                        InstGrid.cells(TTLFinalRowId, InstGrid.getColIndexById("InstrumentCode")).cell.style.textAlign = 'right';
                        _TTLFinalRowId = TTLFinalRowId;

                        InstGrid.setRowHidden(_WHTRowId, true);
                        //InstGrid.setRowHidden(_TTLBeforeVatRowId, $("#Currency").val() != c_currency_THB);
                        //InstGrid.setRowHidden(_VatRowId, $("#Currency").val() != c_currency_THB);
                        InstGrid.setRowHidden(_TTLBeforeVatRowId, true);
                        InstGrid.setRowHidden(_VatRowId, true);
                    }
                    //End Add

                    //isEmpty = CheckFirstRowIsEmpty(InstGrid);
                    AddNewRow(InstGrid, ["", "", "", "", "", "", "", "", "", "", ""]);
                    var row_id = InstGrid.getRowId(InstGrid.getRowsNum() - 1);

                    InstGrid.cells(row_id, InstGrid.getColIndexById("InstrumentCode")).setValue('<div style="float:right;">' + c_cancelPurchaseOrder + " " + GenerateHtmlButton2(btnCancel, row_id, CancelPurchaseOrder, 160, true) + '</div>');

                    InstGrid.cells(row_id, InstGrid.getColIndexById("InstrumentCode")).setBgColor("#ffffff");

                    var btnCancelID = "#" + GenerateGridControlID(btnCancel, row_id);
                    $(btnCancelID).click(btnCancel_OnClick);
                    InstGrid.setColspan(row_id, 0, 10); //Modify (Change 7 -> 10) by Jutarat A. on 30102013
                    _btnCancelRowId = row_id;

                    CalculateTotalAmount(false, false); //Add by Jutarat A. on 01112013
                    InstGrid.setSizes();

                    call_ajax_method_json("/inventory/IVS260_GetPurchaseOrderStatus", "", function (result, controls) {
                        if (result != null) {
                            PurchaseOrderStatus = result.doPOrderDetailData.PurchaseOrderStatus; //Add by Jutarat A. on 31102013

                            if (PurchaseOrderStatus == WaitToReceive) //if (result.PurchaseOrderStatus == WaitToReceive) //Modify by Jutarat A. on 31102013
                                $("#" + GenerateGridControlID(btnCancel, _btnCancelRowId)).SetDisabled(false);
                            else
                                $("#" + GenerateGridControlID(btnCancel, _btnCancelRowId)).SetDisabled(true);

                            //Modify by Jutarat A. on 31102013
                            //$("#PurchaseData").bindJSON_Prefix("DT", result);
                            $("#PurchaseData").bindJSON_Prefix("DT", result.doPOrderDetailData);

                            $("#DTPurchaseOrderType").val(result.doPurchaseOrderData.PurchaseOrderType);
                            $("#DTTransportType").val(result.doPurchaseOrderData.TransportType);
                            $("#AdjustDueDate").val(ConvertDateToTextFormat(ConvertDateObject(result.doPurchaseOrderData.ShippingDate)));
                            $("#Currency").val(result.doPurchaseOrderData.Currency);
                            $("#Memo").val(result.doPurchaseOrderData.Memo);
                            //End Modify

                            //Add by Jutarat A. on 04112013
                            var isDisableCtrl = PurchaseOrderStatus != WaitToReceive;

                            $("#DTPurchaseOrderType").SetDisabled(isDisableCtrl);
                            $("#AdjustDueDate").EnableDatePicker(!isDisableCtrl); //$("#AdjustDueDate").SetDisabled(isDisableCtrl); //Modify by Jutarat A. on 19112013
                            $("#Memo").SetDisabled(isDisableCtrl);

                            if (isDisableCtrl) {
                                $("#DTTransportType").SetDisabled(true);
                                $("#Currency").SetDisabled(true);
                            }
                            else {
                                CheckDisabledPurchaseOrderTypeChange();
                            }

                            $("#InstrumentCode").SetDisabled(isDisableCtrl);
                            $("#btnSearchInstrument").SetDisabled(isDisableCtrl);
                            $("#UnitPrice").SetDisabled(isDisableCtrl);
                            $("#OrderQty").SetDisabled(isDisableCtrl);
                            $("#btnAdd").SetDisabled(isDisableCtrl);
                            $("#btnCancel").SetDisabled(isDisableCtrl);

                            $("#" + GenerateGridControlID(lblDiscount, _DiscountRowId)).SetDisabled(isDisableCtrl);
                            $("#" + GenerateGridControlID(lblWHTTotal, WHTRowId)).SetDisabled(isDisableCtrl);
                            $("#" + GenerateGridControlID(lvlVatTotal, _VatRowId)).SetDisabled(isDisableCtrl);

                            for (var i = 0; i < getDetailRowsNum(); i++) {
                                var row_id = InstGrid.getRowId(i);

                                $("#" + GenerateGridControlID("InstrumentName", row_id)).SetDisabled(isDisableCtrl);
                                //$("#" + GenerateGridControlID("Memo", row_id)).SetDisabled(isDisableCtrl);
                                $("#" + GenerateGridControlID(UnitPrice, row_id)).SetDisabled(isDisableCtrl);
                            }
                            //End Add

                            ////Add by Jutarat A. on 01112013
                            //if (InstGrid != null && _VatRowId != null) {
                            //    if ($("#Currency").val() == c_currency_THB) {
                            //        CalculateVatAmount(null, false, true);
                            //        InstGrid.setRowHidden(_VatRowId, false);
                            //    }
                            //    else {
                            //        $("#" + GenerateGridControlID(lvlVatTotal, _VatRowId)).val("0.00");
                            //        InstGrid.cells(_VatRowId, InstGrid.getColIndexById("DetailAmount")).setAttribute("title", "0.00");
                            //        InstGrid.setRowHidden(_VatRowId, true);
                            //    }
                            //}
                            ////End Add

                            //R2
                            if (PurchaseOrderStatus != CompleteReceive && !ViewOnlyMode) //if (result.PurchaseOrderStatus != CompleteReceive) //Modify by Jutarat A. on 31102013
                                register_command.SetCommand(cmdRegister);
                            else {
                                register_command.SetCommand(null);

                                for (var i = 0; i < getDetailRowsNum(); i++) {
                                    var rid = InstGrid.getRowId(i);
                                    var mbox_id = "#" + GenerateGridControlID(ModifyBox, rid);
                                    $(mbox_id).attr("readonly", "readonly");
                                }
                            }
                            //End R2

                            //Comment by Jutarat A. on 30102013
                            /*$("#DTAmount").val(SetNumericValue($("#DTAmount").val(), 2));
                            $("#DTAmount").setComma();*/
                            //End Comment

                            if (ViewOnlyMode) {
                                $("#MaintainPurchaseOrder").find("button,select").prop("disabled", true);
                                $("#MaintainPurchaseOrder").find("input[type=text],textarea").prop("readonly", true);
                                $("#AdjustDueDate").EnableDatePicker(false);
                                $("#MaintainPurchaseOrder").find("img[id^=btnCalculateDiscount]").hide();
                                register_command.SetCommand(null);
                                reset_command.SetCommand(null);
                                confirm_command.SetCommand(null);
                                back_command.SetCommand(null);
                            }
                            else {
                                reset_command.SetCommand(cmdReset);
                            }
                        }
                    });
                }
                , function () {

                });

                selected_porder = true;
            });
        }

        $("#POrderGrid a[name=PurchaseOrderNoLink]").click(function () {
            var param = {
                strPurchaseOrderNo: $(this).text(),
                strReportID: null
            };

            ajax_method.CallScreenController("/inventory/IVS260_PrepareDownloadDocument", param, function (result, controls) {
                if (result == true) {
                    var key = ajax_method.GetKeyURL(null);
                    var url = ajax_method.GenerateURL("/inventory/IVS260_DownloadPreparedDocument?k=" + key)
                    window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
                }
            });
        });
    });

    InstGrid = $("#InstGrid").InitialGrid(0, false, "/inventory/IVS260_intiInstGrid");
    BindOnLoadedEvent(InstGrid, function (gen_ctrl) {
        isEmpty = CheckFirstRowIsEmpty(InstGrid);
        if (isEmpty == true) {
            isEmpty = false;
            return;
        }

        //SpecialGridControl(InstGrid, ["ModifyOrderQty"]);
        //SpecialGridControl(InstGrid, ["InstrumentName", "Memo", "UnitPrice", "ModifyOrderQty", "Unit", "Remove"]); //Modify by Jutarat A. on 29102013
        SpecialGridControl(InstGrid, ["InstrumentName", "OriginalUnitPrice", "UnitPrice", "ModifyOrderQty", "Unit", "Remove"]); 

        var isRemoveColinx = InstGrid.getColIndexById("IsShowRemove"); //Add by Jutarat A. on 29102013

        for (var i = 0; i < InstGrid.getRowsNum(); i++) {

            var row_id = InstGrid.getRowId(i);

            //Add by Jutarat A. on 29102013
            if (gen_ctrl == true) {
                var InstrumentNameVal = GetValueFromLinkType(InstGrid, i, InstGrid.getColIndexById("InstrumentName"));
                //var memoVal = GetValueFromLinkType(InstGrid, i, InstGrid.getColIndexById("Memo"));
                var originalUnitPriceVal = GetValueFromLinkType(InstGrid, i, InstGrid.getColIndexById("OriginalUnitPrice"));
                var unitPriceVal = GetValueFromLinkType(InstGrid, i, InstGrid.getColIndexById("UnitPrice"));
                var unitVal = GetValueFromLinkType(InstGrid, i, InstGrid.getColIndexById("Unit"));

                GenerateTextBox(InstGrid, "InstrumentName", row_id, "InstrumentName", InstrumentNameVal, true, 100);
                //GenerateTextBox(InstGrid, "Memo", row_id, "Memo", memoVal, true, 1000);

                GenerateGridCombobox(InstGrid, row_id, cboUnit, "Unit", "/Inventory/IVS260_GetUnit", unitVal, true);

                GenerateNumericBox2(InstGrid, OriginalUnitPrice, row_id, "OriginalUnitPrice", SetNumericText(originalUnitPriceVal, 3), 10, 3, 0, 9999999999.999, true, true);
                GenerateNumericBox2(InstGrid, UnitPrice, row_id, "UnitPrice", SetNumericText(unitPriceVal, 3), 10, 3, 0, 9999999999.999, true, true);

                var isRemove = InstGrid.cells(row_id, isRemoveColinx).getValue();
                if (isRemove == true) {
                    GenerateRemoveButton(InstGrid, "btnRemove", row_id, "Remove", true);
                }
                //End Add

                if (InstGrid.cells(row_id, InstGrid.getColIndexById("RemainQty")).getValue() == 0) {
                    var receiveQty = InstGrid.cells(row_id, InstGrid.getColIndexById("ReceiveQty")).getValue();
                    GenerateNumericBox2(InstGrid, ModifyBox, row_id, "ModifyOrderQty", receiveQty, 5, 0, "", 99999, true, false);
                } else {
                    //Modify by Jutarat A. on 19112013
                    //GenerateNumericBox2(InstGrid, ModifyBox, row_id, "ModifyOrderQty", "", 4, 0, "", 9999, true, true);
                    var modifyQtyVal = GetValueFromLinkType(InstGrid, i, InstGrid.getColIndexById("ModifyOrderQty"));
                    GenerateNumericBox2(InstGrid, ModifyBox, row_id, "ModifyOrderQty", modifyQtyVal, 5, 0, "", 99999, true, true);
                    //End Modify
                }

                CalculateAmount(row_id, false); //Add by Jutarat A. on 29102013


                /*var ModBox = "#" + GenerateGridControlID(ModifyBox, row_id);

                $(ModBox).attr("name", row_id);

                $(ModBox).blur(function () {
                if ($.trim($(this).val()) == "") {
                $(this).val("");
                }
                //              else {
                //                    if (!$(this).prop("readonly")) {
                //                        row_id = $(this).attr("name");
                //                        if (parseInt($(this).NumericValue()) < InstGrid.cells(row_id, InstGrid.getColIndexById("ReceiveQty")).getValue()) {
                //                            //alert($(this).NumericValue());
                //                            var txt = $(this);
                //                            var obj = { module: "Inventory",
                //                                code: "MSG4025"
                //                            };
                //                            call_ajax_method("/Shared/GetMessage", obj, function (result) {
                //                                OpenWarningDialog(result.Message);
                //                                txt.addClass("highlight");
                //                                txt.keyup(function () {
                //                                    txt.removeClass("highlight");
                //                                });
                //                            });
                //                            return false;
                //                        }
                //                        if (parseInt($(this).NumericValue()) > InstGrid.cells(row_id, InstGrid.getColIndexById("FirstOrderQty")).getValue()) {
                //                            //alert($(this).NumericValue());
                //                            var txt = $(this);
                //                            var obj = {
                //                                module: "Inventory",
                //                                code: "MSG4026"
                //                            };
                //                            call_ajax_method("/Shared/GetMessage", obj, function (result) {
                //                                OpenWarningDialog(result.Message);
                //                                txt.addClass("highlight");
                //                                txt.keyup(function () {
                //                                    txt.removeClass("highlight");
                //                                });
                //                            });
                //                            return false;
                //                        }
                //                    }
                //                }
                }); // End blur*/

                //Add by Jutarat A. on 01112013
                var orderID = "#" + GenerateGridControlID(ModifyBox, row_id);
                $(orderID).attr("name", row_id);
                $(orderID).change(function () {
                    CalculateDiscountAmount();
                });

                var OriginalUnitPriceID = "#" + GenerateGridControlID(OriginalUnitPrice, row_id);
                $(OriginalUnitPriceID).attr("name", row_id);
                $(OriginalUnitPriceID).change(function () {
                    CalculateDiscountAmount();
                });

                var UnitPriceID = "#" + GenerateGridControlID(UnitPrice, row_id);
                $(UnitPriceID).attr("name", row_id);
                $(UnitPriceID).change(function () {
                    CalculateAmount($(this).attr("name"));
                    CalculateTotalAmount(false, true);
                });

                var UnitID = "#" + GenerateGridControlID(cboUnit, row_id);
                $(UnitID).attr("name", row_id);
                $(UnitID).blur(function () {
                    if (!$(UnitID).val()) {
                        master_event.LockWindow(true);
                        var obj = {
                            module: "inventory",
                            code: "MSG4145"
                        };
                        call_ajax_method("/Shared/GetMessage", obj, function (result) {
                            master_event.LockWindow(false);
                            OpenErrorMessageDialog(result.Code, result.Message, function () { $(UnitID).focus(); }, null);
                        });
                    }
                })
                .change(function () {
                    var row_id = $(this).attr("name");
                    var value = $(this).val();
                    InstGrid.cells(row_id, InstGrid.getColIndexById("Unit")).setAttribute("title", value);
                });


                //End Add
            }

            //Add by Jutarat A. on 29102013
            BindGridButtonClickEvent("btnRemove", row_id, function (rid) {

                DeleteRow(InstGrid, rid);

                if (getDetailRowsNum() <= 0) {
                    DeleteRow(InstGrid, _TTLRowId);
                    DeleteRow(InstGrid, _DiscountRowId);
                    DeleteRow(InstGrid, _TTLBeforeVatRowId);
                    DeleteRow(InstGrid, _VatRowId);
                    DeleteRow(InstGrid, _WHTRowId);
                    DeleteRow(InstGrid, _TTLFinalRowId);
                    DeleteRow(InstGrid, _btnCancelRowId);

                    _TTLRowId = null;
                    _DiscountRowId = null;
                    _TTLBeforeVatRowId = null;
                    _WHTRowId = null;
                    _VatRowId = null;
                    _TTLFinalRowId = null;
                    _btnCancelRowId = null;

                    _isAddedInstrument = false;
                    //SetRegisterCommand(false, null);

                } else {
                    CalculateTotalAmount(false, true);
                }
            }); // End btnRemove Click  
            //End Add
        }
    });
}

function getOriginalTotalAmount() {
    var firstOrderQty = 0;
    var decUnitPrice = 0;
    var decOrderQty = 0;
    var decTotalAmount = 0;

    for (var i = 0; i < getDetailRowsNum(); i++) {
        var row_id = InstGrid.getRowId(i);

        firstOrderQty = InstGrid.cells(row_id, InstGrid.getColIndexById("FirstOrderQty")).getValue();
        decUnitPrice = $("#" + GenerateGridControlID(OriginalUnitPrice, row_id)).NumericValue();
        decOrderQty = $("#" + GenerateGridControlID(ModifyBox, row_id)).NumericValue();

        if (decOrderQty != null && decOrderQty != "") {
            decTotalAmount += Math.round10((+decOrderQty * +decUnitPrice), -2);
        }
        else {
            decTotalAmount += Math.round10((firstOrderQty * +decUnitPrice), -2);
        }
    }

    return Math.round10(decTotalAmount, -2);
}

function getTotalAmount() {
    var firstOrderQty = 0;
    var decUnitPrice = 0;
    var decOrderQty = 0;
    var decTotalAmount = 0;

    for (var i = 0; i < getDetailRowsNum(); i++) {
        var row_id = InstGrid.getRowId(i);

        //firstOrderQty = InstGrid.cells(row_id, InstGrid.getColIndexById("FirstOrderQty")).getValue();
        //decUnitPrice = +$("#" + GenerateGridControlID(UnitPrice, row_id)).NumericValue();
        //decOrderQty = $("#" + GenerateGridControlID(ModifyBox, row_id)).NumericValue();

        //if (decOrderQty != null && decOrderQty != "") {
        //    decTotalAmount += Math.round10((decOrderQty * decUnitPrice), -2);
        //}
        //else {
        //    decTotalAmount += Math.round10((firstOrderQty * decUnitPrice), -2);
        //}

        decTotalAmount += +(GetValueFromLinkType(InstGrid, InstGrid.getRowIndex(row_id), InstGrid.getColIndexById("DetailAmount")).replace(/ /g, "").replace(/,/g, ""));
    }

    return Math.round10(decTotalAmount, -2);
}

function CalculateDiscountAmount() {
    //var decTotalAmount = getOriginalTotalAmount();
    //var decDiscountAmount = +$("#" + GenerateGridControlID(lblDiscount, _DiscountRowId)).NumericValue();
    //var decAmountPercentage = 1;
    //if (decTotalAmount > 0) {
    //    decAmountPercentage = 1 - (decDiscountAmount / decTotalAmount);
    //}

    //for (var i = 0; i < getDetailRowsNum(); i++) {
    //    var row_id = InstGrid.getRowId(i);

    //    var decOriginalUnitPrice = +$("#" + GenerateGridControlID(OriginalUnitPrice, row_id)).NumericValue();
    //    var decUnitPrice = decOriginalUnitPrice * decAmountPercentage;
    //    var decUnitPriceText = SetNumericText(decUnitPrice, 3);

    //    var UnitPriceID = "#" + GenerateGridControlID(UnitPrice, row_id);
    //    $(UnitPriceID).val(decUnitPriceText);
    //    InstGrid.cells(row_id, InstGrid.getColIndexById("UnitPrice")).setAttribute("title", decUnitPriceText);
    //    CalculateAmount(row_id);
    //        
    //    InstGrid.setSizes();
    //}

    var decOriginalTotalAmount = getOriginalTotalAmount();
    var decDiscountAmount = +$("#" + GenerateGridControlID(lblDiscount, _DiscountRowId)).NumericValue();
    var decTotalAmount = decOriginalTotalAmount - decDiscountAmount;

    var decAmountPercentage = 1;
    if (decOriginalTotalAmount > 0) {
        decAmountPercentage = decTotalAmount / decOriginalTotalAmount;
    }

    var arrAmount = [], decTotalAmountTemp = 0, decTotalAmountDelta = 0;
    for (var i = 0; i < getDetailRowsNum(); i++) {
        var row_id = InstGrid.getRowId(i);

        var firstOrderQty = InstGrid.cells(row_id, InstGrid.getColIndexById("FirstOrderQty")).getValue();
        var decUnitPrice = +$("#" + GenerateGridControlID(OriginalUnitPrice, row_id)).NumericValue();
        var decOrderQty = $("#" + GenerateGridControlID(ModifyBox, row_id)).NumericValue();
        var decAmount = 0; qty = 0;

        if (decOrderQty != null && decOrderQty != "") {
            qty = +decOrderQty;
            decAmount = Math.round10((+decOrderQty * +decUnitPrice), -2);
        }
        else {
            qty = +firstOrderQty;
            decAmount = Math.round10((firstOrderQty * +decUnitPrice), -2);
        }

        var objAmount = {
            RowId: row_id,
            UnitPrice: decUnitPrice,
            Qty: qty,
            Amount: decAmount,
            DiscountedUnitPrice: (qty == 0 ? 0 : Math.round10(Math.round10(decAmount * decAmountPercentage, -2) / qty, -3))
        };

        objAmount.DiscountedAmount = Math.round10(objAmount.DiscountedUnitPrice * qty, -2)

        decTotalAmountTemp = Math.round10(decTotalAmountTemp + objAmount.DiscountedAmount, -2);
        arrAmount.push(objAmount);
    }

    decTotalAmountDelta = Math.round10(decTotalAmount - decTotalAmountTemp, -2);

    if (arrAmount.length > 0) {
        if (decTotalAmountDelta != 0) {
            arrAmount[arrAmount.length - 1].DiscountedAmount += decTotalAmountDelta;
        }
    }

    for (var i=0; i<arrAmount.length; i++) {
        var obj = arrAmount[i];
        var decUnitPriceText = SetNumericText(obj.DiscountedUnitPrice, 3);
        var UnitPriceID = "#" + GenerateGridControlID(UnitPrice, obj.RowId);
        $(UnitPriceID).val(decUnitPriceText);
        InstGrid.cells(obj.RowId, InstGrid.getColIndexById("UnitPrice")).setAttribute("title", decUnitPriceText);
        CalculateAmount(obj.RowId, arrAmount[i].DiscountedAmount);
    }
    InstGrid.setSizes();

    CalculateTotalAmount(false, true);
}

function CalculateTotalAmount(calWHT, calVat) {
    var decUnitPrice = 0;
    var decOrderQty = 0;
    var decTotalAmount = getOriginalTotalAmount();
    var totalAmountText = SetNumericText(decTotalAmount, 2);

    if (_TTLRowId) {
        InstGrid.cells(_TTLRowId, InstGrid.getColIndexById("DetailAmount")).setValue(totalAmountText);
        InstGrid.cells(_TTLRowId, InstGrid.getColIndexById("DetailAmount")).setAttribute("title", totalAmountText);
    }
    //var decDiscountAmount = +$("#" + GenerateGridControlID(lblDiscount, _DiscountRowId)).NumericValue();

    //var totalAmountBeforeVat = decTotalAmount - decDiscountAmount;
    //var totalAmountBeforeVatText = SetNumericText(totalAmountBeforeVat, 2);
    var totalAmountBeforeVat = getTotalAmount();
    var totalAmountBeforeVatText = SetNumericText(totalAmountBeforeVat, 2);

    //SetValue grid
    if (_TTLBeforeVatRowId) {
        InstGrid.cells(_TTLBeforeVatRowId, InstGrid.getColIndexById("DetailAmount")).setValue(totalAmountBeforeVatText);
        InstGrid.cells(_TTLBeforeVatRowId, InstGrid.getColIndexById("DetailAmount")).setAttribute("title", totalAmountBeforeVatText);

        var decDiscountAmount = +$("#" + GenerateGridControlID(lblDiscount, _DiscountRowId)).NumericValue();
        var decTotalAmountDiscounted = Math.round10(decTotalAmount - decDiscountAmount, -2);
        if (decTotalAmountDiscounted != totalAmountBeforeVat) {
            InstGrid.cells(_TTLBeforeVatRowId, InstGrid.getColIndexById("Remove")).setValue("<div style='color:red; font-weight:bold'>X</div>");
        }
        else {
            InstGrid.cells(_TTLBeforeVatRowId, InstGrid.getColIndexById("Remove")).setValue("");
        }
    }
    InstGrid.setSizes();

    CalculateVatAmount(totalAmountBeforeVat, calWHT, calVat);
}

function CalculateVatAmount(totalAmountBeforeVat, calWHT, calVat) {
    if (!totalAmountBeforeVat) {
        //var totalAmount = getTotalAmount();
        //var decDiscountAmount = +$("#" + GenerateGridControlID(lblDiscount, _DiscountRowId)).NumericValue();
        //totalAmountBeforeVat = totalAmount - decDiscountAmount;
        totalAmountBeforeVat = getTotalAmount();
    }

    // WHT disabled, always fixed to 0
    var whtAmount = 0;
    //if (calWHT) {
    //    whtAmount = Math.round10(((totalAmountBeforeVat * WHT) / 100), -2);
    //    if (whtAmount < 0) {
    //        whtAmount = 0
    //    }
    //    var whtAmountText = SetNumericText(whtAmount, 2);
    //
    //    $("#" + GenerateGridControlID(lblWHTTotal, _WHTRowId)).val(whtAmountText);
    //    $("#" + GenerateGridControlID(lblWHTTotal, _WHTRowId)).removeClass("highlight");
    //
    //    if (_WHTRowId) {
    //        InstGrid.cells(_WHTRowId, InstGrid.getColIndexById("DetailAmount")).setAttribute("title", whtAmountText);
    //    }
    //    _WHTTotalText = whtAmountText;
    //}
    //else {
    //    whtAmount = +$("#" + GenerateGridControlID(lblWHTTotal, _WHTRowId)).NumericValue();
    //}
    var whtAmountText = SetNumericText(whtAmount, 2);
    $("#" + GenerateGridControlID(lblWHTTotal, _WHTRowId)).val(whtAmountText);
    $("#" + GenerateGridControlID(lblWHTTotal, _WHTRowId)).removeClass("highlight");
    if (_WHTRowId) {
        InstGrid.cells(_WHTRowId, InstGrid.getColIndexById("DetailAmount")).setAttribute("title", whtAmountText);
    }
    _WHTTotalText = whtAmountText;

    var vatAmount = 0;
    if (calVat) {
        if ($("#Currency").val() == c_currency_THB) {
            //vatAmount = Math.round10(((totalAmountBeforeVat * VatTHB) / 100), -2);
        }
        else {
            vatAmount = 0;
        }
        if (vatAmount < 0) {
            vatAmount = 0
        }
        var vatAmountText = SetNumericText(vatAmount, 2);

        $("#" + GenerateGridControlID(lvlVatTotal, _VatRowId)).val(vatAmountText);
        $("#" + GenerateGridControlID(lvlVatTotal, _VatRowId)).removeClass("highlight");
        if (_VatRowId) {
            InstGrid.cells(_VatRowId, InstGrid.getColIndexById("DetailAmount")).setAttribute("title", vatAmountText);
        }
        _VatTotalText = vatAmountText; //Add by Jutarat A. on 01082013
    }
    else {
        vatAmount = +$("#" + GenerateGridControlID(lvlVatTotal, _VatRowId)).NumericValue();
    }

    var ctrl_focused = $(":focus");
    if (GenerateGridControlID(lvlVatTotal, _VatRowId) == ctrl_focused.attr("id")
        || GenerateGridControlID(lblWHTTotal, _WHTRowId) == ctrl_focused.attr("id")) {
        ctrl_focused.select();
    }

    var totalAmountFinal = totalAmountBeforeVat - whtAmount + vatAmount;
    CalculateTotalFinalAmount(totalAmountFinal);
}

function CalculateTotalFinalAmount(totalAmountFinal) {
    if (!totalAmountFinal) {
        var totalAmount = getTotalAmount();
        var decDiscountAmount = +$("#" + GenerateGridControlID(lblDiscount, _DiscountRowId)).NumericValue();
        var whtAmount = +$("#" + GenerateGridControlID(lblWHTTotal, _WHTRowId)).NumericValue();
        var vatAmount = +$("#" + GenerateGridControlID(lvlVatTotal, _VatRowId)).NumericValue();
        totalAmountFinal = totalAmount - decDiscountAmount - whtAmount + vatAmount;
    }

    var totalAmountFinalText = SetNumericText(totalAmountFinal, 2);
    if (_TTLFinalRowId) {
        InstGrid.cells(_TTLFinalRowId, InstGrid.getColIndexById("DetailAmount")).setValue(totalAmountFinalText);
        InstGrid.cells(_TTLFinalRowId, InstGrid.getColIndexById("DetailAmount")).setAttribute("title", totalAmountFinalText);

        var remark = InstGrid.cells(_TTLBeforeVatRowId, InstGrid.getColIndexById("Remove")).getValue();
        InstGrid.cells(_TTLFinalRowId, InstGrid.getColIndexById("Remove")).setValue(remark);
    }
    InstGrid.setSizes();
}

//Add by Jutarat A. on 01112013
function CalculateAmount(row_id, fixed_amount) {
    var idxColFirstOrderQty = InstGrid.getColIndexById("FirstOrderQty");
    var idxColAmount = InstGrid.getColIndexById("DetailAmount");

    if (fixed_amount === false) {
        if (row_id) {
            var decAmount = InstGrid.cells(row_id, idxColAmount).getValue();
            var decAmountText = SetNumericText(decAmount, 2);
            InstGrid.cells(row_id, idxColAmount).setValue(decAmountText);
            InstGrid.cells(row_id, idxColAmount).setAttribute("title", decAmountText);
        }
    }
    else {
        var firstOrderQty = InstGrid.cells(row_id, idxColFirstOrderQty).getValue();
        var decUnitPrice = $("#" + GenerateGridControlID(UnitPrice, row_id)).NumericValue();
        var decOrderQty = $("#" + GenerateGridControlID(ModifyBox, row_id)).NumericValue();

        var decAmount = 0;
        if (fixed_amount) {
            decAmount = fixed_amount;
        } else if (decOrderQty != null && decOrderQty != "") {
            decAmount = Math.round10((+decOrderQty * +decUnitPrice), -2);
        }
        else {
            decAmount = Math.round10((firstOrderQty * +decUnitPrice), -2);
        }

        var decAmountText = SetNumericText(decAmount, 2);

        if (row_id) {
            InstGrid.cells(row_id, idxColAmount).setValue(decAmountText);
            InstGrid.cells(row_id, idxColAmount).setAttribute("title", decAmountText);
        }
    }
}
//End Add

//Add by Jutarat A. on 30102013
function ClearInstrumentSection() {
    $("#InstInput").clearForm();
}
//End Add

//Add by Jutarat A. on 01112013
function getGridData() {
    var InstList = null;

    if (CheckFirstRowIsEmpty(InstGrid, false) == false) {
        InstList = new Array();

        for (var i = 0; i < getDetailRowsNum(); i++) {
            var row_id = InstGrid.getRowId(i);
            var Inst = {
                InstrumentCode: InstGrid.cells(row_id, InstGrid.getColIndexById("InstrumentCode")).getValue(),
                InstrumentName: $("#" + GenerateGridControlID("InstrumentName", row_id)).val(),
                //Memo: $("#" + GenerateGridControlID("Memo", row_id)).val(),
                OriginalUnitPrice: $("#" + GenerateGridControlID(OriginalUnitPrice, row_id)).NumericValue(),
                UnitPrice: $("#" + GenerateGridControlID(UnitPrice, row_id)).NumericValue(),
                FirstOrderQty: InstGrid.cells(row_id, InstGrid.getColIndexById("FirstOrderQty")).getValue(),
                ModifyOrderQty: $("#" + GenerateGridControlID(ModifyBox, row_id)).NumericValue(),
                ReceiveQty: InstGrid.cells(row_id, InstGrid.getColIndexById("ReceiveQty")).getValue(),
                RemainQty: InstGrid.cells(row_id, InstGrid.getColIndexById("RemainQty")).getValue(),
                //Amount: InstGrid.cells(row_id, InstGrid.getColIndexById("DetailAmount")).getValue(),
                Amount: GetValueFromLinkType(InstGrid, InstGrid.getRowIndex(row_id), InstGrid.getColIndexById("DetailAmount")).replace(/ /g, "").replace(/,/g, ""), //Modify by Jutarat A. on 19112013
                IsShowRemove: InstGrid.cells(row_id, InstGrid.getColIndexById("IsShowRemove")).getValue(),
                row_id: row_id,
                ModifyOrderQtyID: GenerateGridControlID(ModifyBox, row_id),
                Unit: $("#" + GenerateGridControlID(cboUnit, row_id)).val(),
                DetailAmount: GetValueFromLinkType(InstGrid, InstGrid.getRowIndex(row_id), InstGrid.getColIndexById("DetailAmount")).replace(/ /g, "").replace(/,/g, "")

            };
            InstList.push(Inst);
        }
    }

    return InstList;
}
//End Add

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
function cmdRegister() {
    command_control.CommandControlMode(false);
    var lstInst = new Array();
    var lstModifyOrderQtyID = new Array();

    //Modify by Jutarat A. on 06112013
    /*for (var i = 0; i < InstGrid.getRowsNum() - 1; i++) {
        var rid = InstGrid.getRowId(i);

        var mbox_id = "#" + GenerateGridControlID(ModifyBox, rid);
        //        if ($(mbox_id).prop("readonly")) {
        //            continue;
        //        }

        //Add by Jutarat A. on 21082013
        var objUnitPrice = "0";
        var tmpUnitPrice = InstGrid.cells(rid, InstGrid.getColIndexById("UnitPrice")).getValue();
        if (tmpUnitPrice != null)
            objUnitPrice = tmpUnitPrice.toString();
        //End Add

        var obj = {
            InstrumentCode: InstGrid.cells(rid, InstGrid.getColIndexById("InstrumentCode")).getValue(),
            FirstOrderQty: InstGrid.cells(rid, InstGrid.getColIndexById("FirstOrderQty")).getValue(),
            ModifyOrderQty: $(mbox_id).NumericValue(),
            ReceiveQty: InstGrid.cells(rid, InstGrid.getColIndexById("ReceiveQty")).getValue(),
            //UnitPrice: InstGrid.cells(rid, InstGrid.getColIndexById("UnitPrice")).getValue() + ".00",
            UnitPrice: objUnitPrice, //Modify by Jutarat A. on 21082013
            RemainQty: InstGrid.cells(rid, InstGrid.getColIndexById("RemainQty")).getValue(),
            row_id: rid,
            ModifyOrderQtyID: GenerateGridControlID(ModifyBox, rid)
        };

        lstInst.push(obj);
        lstModifyOrderQtyID.push(GenerateGridControlID(ModifyBox, rid));
    }*/
    for (var i = 0; i < getDetailRowsNum(); i++) {
        var rid = InstGrid.getRowId(i);

        var obj = {
            InstrumentCode: InstGrid.cells(rid, InstGrid.getColIndexById("InstrumentCode")).getValue(),
            InstrumentName: $("#" + GenerateGridControlID("InstrumentName", rid)).val(),
            //Memo: $("#" + GenerateGridControlID("Memo", rid)).val(),
            OriginalUnitPrice: $("#" + GenerateGridControlID(OriginalUnitPrice, rid)).NumericValue(),
            UnitPrice: $("#" + GenerateGridControlID(UnitPrice, rid)).NumericValue(),
            FirstOrderQty: InstGrid.cells(rid, InstGrid.getColIndexById("FirstOrderQty")).getValue(),
            ModifyOrderQty: $("#" + GenerateGridControlID(ModifyBox, rid)).NumericValue(),
            ReceiveQty: InstGrid.cells(rid, InstGrid.getColIndexById("ReceiveQty")).getValue(),
            RemainQty: InstGrid.cells(rid, InstGrid.getColIndexById("RemainQty")).getValue(),
            //Amount: InstGrid.cells(rid, InstGrid.getColIndexById("DetailAmount")).getValue(),
            Amount: GetValueFromLinkType(InstGrid, InstGrid.getRowIndex(rid), InstGrid.getColIndexById("DetailAmount")).replace(/ /g, "").replace(/,/g, ""), //Modify by Jutarat A. on 19112013
            IsShowRemove: InstGrid.cells(rid, InstGrid.getColIndexById("IsShowRemove")).getValue(),
            row_id: rid,
            ModifyOrderQtyID: GenerateGridControlID(ModifyBox, rid),
            Unit: $("#" + GenerateGridControlID(cboUnit, rid)).val(),
            UnitCtrlID: GenerateGridControlID(cboUnit, rid),
            DetailAmount: GetValueFromLinkType(InstGrid, InstGrid.getRowIndex(rid), InstGrid.getColIndexById("DetailAmount")).replace(/ /g, "").replace(/,/g, "")
        };

        lstInst.push(obj);
        lstModifyOrderQtyID.push(GenerateGridControlID(ModifyBox, rid));
    }
    //End Modify

    //Add by Jutarat A. on 06112013
    var param = {
        PurchaseOrderType: $("#DTPurchaseOrderType").val(),
        TransportType: $("#DTTransportType").val(),
        AdjustDueDate: $("#AdjustDueDate").val(),
        Currency: $("#Currency").val(),
        Memo: $("#Memo").val(),
        //TotalAmount: InstGrid.cells(_TTLBeforeVatRowId, InstGrid.getColIndexById("DetailAmount")).getValue().replace(/ /g, "").replace(/,/g, ""),
        TotalAmount: GetValueFromLinkType(InstGrid, InstGrid.getRowIndex(_TTLBeforeVatRowId), InstGrid.getColIndexById("DetailAmount")).replace(/ /g, "").replace(/,/g, ""), //Modify by Jutarat A. on 19112013
        Vat: $("#" + GenerateGridControlID(lvlVatTotal, _VatRowId)).NumericValue(),
        InstrumentData: lstInst,
        Discount: $("#" + GenerateGridControlID(lblDiscount, _DiscountRowId)).NumericValue(),
        WHT: $("#" + GenerateGridControlID(lblWHTTotal, _WHTRowId)).NumericValue()
    };
    //End Add

    //call_ajax_method_json("/inventory/IVS260_cmdRegister", { Cond: lstInst }, function (result, controls) {
    call_ajax_method_json("/inventory/IVS260_cmdRegister", { Cond: param }, function (result, controls) { //Modify by Jutarat A. on 06112013
        if (controls != undefined) {
            VaridateCtrl(lstModifyOrderQtyID, controls);

            $.each(controls, function (index, value) {
                if (value == "Vat" && _VatRowId != null) {
                    var vatCtrlId = GenerateGridControlID(lvlVatTotal, _VatRowId);
                    VaridateCtrl([vatCtrlId], [vatCtrlId]);
                }
                else if (value == "Discount" && _DiscountRowId != null) {
                    var discountCtrlId = GenerateGridControlID(lblDiscount, _DiscountRowId);
                    VaridateCtrl([discountCtrlId], [discountCtrlId]);
                }
                else if (value == "WHT" && _WHTRowId != null) {
                    var whtCtrlId = GenerateGridControlID(lblWHTTotal, _WHTRowId);
                    VaridateCtrl([whtCtrlId], [whtCtrlId]);
                }
                else {
                    VaridateCtrl([value], [value]);
                }
            });
        }

        //Modify by Jutarat A. on 06112013
        /*if (result != null && result) {
        PorderGrid.setColumnHidden(PorderGrid.getColIndexById("Select"), true);
        $("#IVS260PAGE").SetViewMode(true);
        $("#SearchPurchaseOrder").hide();
        selected_porder = false;
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(cmdConfirm);
        back_command.SetCommand(cmdBack);
        var rid = InstGrid.getRowId(InstGrid.getRowsNum() - 1);
        DeleteRow(InstGrid, rid);
        }*/
        if (result != null) {
            if (result == "4022") {
                VaridateCtrl(["Memo"], ["Memo"]);
            }
            else {
                call_ajax_method_json("/inventory/IVS260_ValidateRegis", "", function (result, controls) {
                    if (result) {
                        SetConfirmState();
                    } else {
                        var obj = {
                            module: "Inventory",
                            code: "MSG4024"
                        };
                        call_ajax_method("/Shared/GetMessage", obj, function (result) {
                            OpenYesNoMessageDialog(result.Code, result.Message,
                                function () {
                                    SetConfirmState();
                                },
                                null);
                        });
                    }
                });

            }
        }
        //End Modify

    });
    command_control.CommandControlMode(true);
}

//Add by Jutarat A. on 06112013
function SetConfirmState() {
    PorderGrid.setColumnHidden(PorderGrid.getColIndexById("Select"), true);
    $("#IVS260PAGE").SetViewMode(true);
    $("#SearchPurchaseOrder").hide();
    selected_porder = false;

    InstGrid.setRowHidden(_btnCancelRowId, true);

    InstGrid.setColumnHidden(InstGrid.getColIndexById("Remove"), true);
    SetFitColumnForBackAction(InstGrid, "TempColumn");
    $("#InstInput").hide();

    register_command.SetCommand(null);
    reset_command.SetCommand(null);
    confirm_command.SetCommand(cmdConfirm);
    back_command.SetCommand(cmdBack);
}
//End Add

function cmdConfirm() {
    command_control.CommandControlMode(false);
    call_ajax_method_json("/inventory/IVS260_cmdConfirm", { PurchaseOrderNo: strPurchaseOrderNo, UpdateDate: UpdateDate }, function (result, controls) {
        if (result != null) {
            IVS260_DownloadSlipInfo = result;
            // JS Call Message

            var obj = {
                module: "common",
                code: "MSG0046"
            };
            call_ajax_method("/Shared/GetMessage", obj, function (result) {
                OpenInformationMessageDialog(result.Code, result.Message, function () {
                    confirm_command.SetCommand(null);
                    back_command.SetCommand(null);
                    $("#divShowPurchaseOrder").show();
                    $("#txtShowPurchaseOrderNo").val(IVS260_DownloadSlipInfo.PurchaseOrderNo);
                }, null);
            });


        }
    });

    command_control.CommandControlMode(true);
}

function cmdBack() {
    $("#IVS260PAGE").SetViewMode(false);

    //Add by Jutarat A. on 06112013
    InstGrid.setColumnHidden(InstGrid.getColIndexById("Remove"), false);
    SetFitColumnForBackAction(InstGrid, "TempColumn");
    $("#InstInput").show();
    //End Add

    InstGrid.setRowHidden(_btnCancelRowId, false);

    $("#SearchPurchaseOrder").show();

    PorderGrid.setColumnHidden(PorderGrid.getColIndexById("Select"), false);
    for (var i = 0; i < PorderGrid.getRowsNum(); i++) {
        PorderGrid.setColspan(PorderGrid.getRowId(i), 6, 2);
    }

    selected_porder = true;
    register_command.SetCommand(cmdRegister);
    reset_command.SetCommand(cmdReset);
    confirm_command.SetCommand(null);
    back_command.SetCommand(null);
    //reset_command.SetCommand(cmdReset);    
}

function cmdReset() {
    DeleteAllRow(InstGrid);
    initScreen();
    initGrid();
}

function getDetailRowsNum() {
    return InstGrid.getRowsNum()
        - (_TTLRowId ? 1 : 0)
        - (_DiscountRowId ? 1 : 0)
        - (_TTLBeforeVatRowId ? 1 : 0)
        - (_WHTRowId ? 1 : 0)
        - (_VatRowId ? 1 : 0)
        - (_TTLFinalRowId ? 1 : 0)
        - (_btnCancelRowId ? 1 : 0);
}
