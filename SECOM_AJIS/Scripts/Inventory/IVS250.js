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
var InstGrid = null;
var dtNewInstrument = null;
var isTotalRow = false;
var _TTLRowId = null;
var _TTLBeforeVatRowId = null;
var _VatRowId = null;
var lvlVatTotal = "vatTotal";
var OriginalUnitPrice = "OriginalUnitPrice";
var UnitPrice = "UnitPriceBox";
var OrderQty = "OrderQtyBox";
var cboUnit = "cboUnit";
var _isSelectedSupplier = false;
var _isAddedInstrument = false;
var _VatTotalText = ""; //Add by Jutarat A. on 01082013

var _DiscountRowId = null;
var lblDiscount = "discount";

var _WHTRowId = null;
var lblWHTTotal = "wht";
var _WHTTotalText = null;

var _TTLFinalRowId = null;

$(document).ready(function () {
    initScreen();
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

    $("#PorderType").change(function () {
        CloseWarningDialog();
        VaridateCtrl(["PorderType", "TransportType", "Currency"], null);
        if ($("#PorderType").val() == C_PURCHASE_ORDER_TYPE_DOMESTIC) {

            $("#TransportType").prop("selectedIndex", 0)

            $("#TransportType").SetDisabled(true);
            //$("#Currency").prop("selectedIndex", 0)
            $("#Currency").val('1');
            $("#Currency").SetDisabled(false);

            if (InstGrid != null && _VatRowId != 0) {
                InstGrid.setRowHidden(_TTLBeforeVatRowId, true);
                InstGrid.setRowHidden(_VatRowId, true);
                //InstGrid.setRowHidden(_TTLBeforeVatRowId, $("#Currency").val() != c_currency_THB);
                //InstGrid.setRowHidden(_VatRowId, $("#Currency").val() != c_currency_THB);
                //CalculateVatAmount(null, false, false);
            }
        } else {
            $("#Currency").prop("selectedIndex", 0)
            $("#TransportType").SetDisabled(false);
            $("#Currency").SetDisabled(false);

            if (InstGrid != null && _VatRowId != 0) {
                InstGrid.setRowHidden(_TTLBeforeVatRowId, true);
                InstGrid.setRowHidden(_VatRowId, true);
                //InstGrid.setRowHidden(_TTLBeforeVatRowId, $("#Currency").val() != c_currency_THB);
                //InstGrid.setRowHidden(_VatRowId, $("#Currency").val() != c_currency_THB);
                //CalculateVatAmount(null, false, false);
            }
        }
    });

    $("#Currency").change(function () {
        if (InstGrid != null && _VatRowId != null) {
            if ($("#Currency").val() == c_currency_THB) {
                //CalculateVatAmount(null, false, true);
                InstGrid.setRowHidden(_TTLBeforeVatRowId, true);
                InstGrid.setRowHidden(_VatRowId, true);
            }
            else {
                $("#" + GenerateGridControlID(lvlVatTotal, _VatRowId)).val("0.00");
                InstGrid.cells(_VatRowId, InstGrid.getColIndexById("Amount")).setAttribute("title", "0.00");
                CalculateVatAmount(null, false, false);
                InstGrid.setRowHidden(_TTLBeforeVatRowId, true);
                InstGrid.setRowHidden(_VatRowId, true);
            }
        }
    });

}
function initScreen() {
    SetRegisterCommand(false, null);
    SetResetCommand(true, cmdReset);
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);

    _isSelectedSupplier = false;
    _isAddedInstrument = false;
    _TTLRowId = null;
    _DiscountRowId = null;
    _TTLBeforeVatRowId = null;
    _WHTRowId = null;
    _VatRowId = null;
    _TTLFinalRowId = null;

    $("#memo").SetMaxLengthTextArea(1000);

    $("#SupInfomation").show();
    $("#SpecifyPurchaseOrder").show();
    $("#SpecifyPurchaseOrderInstrument").show();
    $("#resOfRegSection").hide();
    $("#InstInput").show();

    $("#SpecifyPurchaseOrderInstrument").clearForm();

    $("#SpecifyPurchaseOrderInstrument").SetViewMode(false);
    $("#SpecifyPurchaseOrder").SetViewMode(false);
    $("#SupInfomation").SetViewMode(false);

    $("#TransportType").prop("selectedIndex", 0)
    $("#TransportType").SetDisabled(false);
    $("#Currency").prop("selectedIndex", 0)
    $("#Currency").SetDisabled(false);
    $("#PorderType").prop("selectedIndex", 1)
    $("#PorderType").SetDisabled(false);
    $("#AdjustDueDate").val("");
    $("#memo").val("");

    $("#SupName").InitialAutoComplete("/Master/GetSupplierName");
    $("#AdjustDueDate").InitialDate();
    InstGrid = $("#InstrumentGridPlain").InitialGrid(0, false, "/inventory/IVS250InstrumentGrid");
    $("#UnitPrice").BindNumericBox(10, 3, 0, 9999999999.999);
    $("#OrderQty").BindNumericBox(5, 0, 0, 99999);

    $("#Unit").val("PCS");

    BindOnLoadedEvent(InstGrid, function () {
        for (var i = 0; i < InstGrid.getRowsNum() ; i++) {
            var row_id = InstGrid.getRowId(i);

            GenerateTextBox(InstGrid, "InstrumentName", row_id, "InstrumentName", "", true, 100); //Add by Jutarat A. on 28102013
            //GenerateTextBox(InstGrid, "Memo", row_id, "Memo", "", true, 1000); //Add by Jutarat A. on 28102013

            GenerateGridCombobox(InstGrid, rowId, cboUnit, "Unit", "/Inventory/IVS250_GetUnit", null, true);

            GenerateNumericBox2(InstGrid, OriginalUnitPrice, row_id, "OriginalUnitPrice", 0, 10, 3, 0, 9999999999.999, true, true);
            GenerateNumericBox2(InstGrid, UnitPrice, row_id, "UnitPrice", 0, 10, 3, 0, 9999999999.999, true, true);
            GenerateNumericBox2(InstGrid, OrderQty, row_id, "OrderQty", 0, 5, 0, 0, 99999, true, true);
            BindGridButtonClickEvent("btnRemove", row_id, function (rid) {
                DeleteRow(InstGrid, rid);
            });
        }
        InstGrid.setSizes();
    });
    //SpecialGridControl(InstGrid, ["UnitPrice", "OrderQty", "Remove", "Amount"]);
    //SpecialGridControl(InstGrid, ["UnitPrice", "OrderQty", "Remove"]);
    //SpecialGridControl(InstGrid, ["InstrumentName","Memo","UnitPrice", "OrderQty", "Unit", "Remove"]); //Modify by Jutarat A. on 28102013
    SpecialGridControl(InstGrid, ["InstrumentName", "OriginalUnitPrice", "UnitPrice", "OrderQty", "Unit", "Remove"]);
}
function initButton() {
    $("#btnNewInstruemnt").click(function () {
        ajax_method.CallScreenControllerWithAuthority("/master/MAS090", "", true);

    });
    $('#btnSearchInstrument').click(function () {
        $('#dlgBox').OpenCMS170Dialog("IVS250");
    });

    $("#btnSearch").click(function () {
        master_event.LockWindow(true);
        var obj = {
            SupplierCode: $("#SupCode").val()
          , SupplierName: $("#SupName").val()
        };

        call_ajax_method_json("/inventory/SearchSupplier", obj, function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl_AtLeast(["SupCode", "SupName"], controls);
            }
            if (result != null) {
                $("#BankName").val(result.BankName);
                $("#AccNo").val(result.AccountNo);
                $("#AccName").val(result.AccountName);

                if ($.trim($("#SupCode").val()) == "")
                    $("#SupCode").val(result.SupplierCode);
                if ($.trim($("#SupName").val()) == "")
                    $("#SupName").val(result.SupplierName);

                $("#SupCode").SetDisabled(true);
                $("#SupName").SetDisabled(true);
                $("#btnSearch").SetDisabled(true);
                master_event.LockWindow(false);

                _isSelectedSupplier = true;

                if (_isSelectedSupplier && _isAddedInstrument) {
                    SetRegisterCommand(true, cmdRegister);
                }
                else {
                    SetRegisterCommand(false, null);
                }
            }
        });
    });

    $("#btnRegNew").click(function () {
        call_ajax_method_json("/inventory/IVS250_resetParam", "", function (result, controls) {
            DeleteAllRow(InstGrid);
            initScreen();
            btnClearClick();
            btnCancelClick();
        });
    });

    $("#btnClear").click(btnClearClick);
    $("#btnCancel").click(btnCancelClick);

    $("#btnAdd").click(function () {
        master_event.LockWindow(true);
        var obj = {
            InstrumentCode: $("#InstrumentCode").val(),
            InstrumentName: $("#InstrumentName").val()
                , UnitPrice: $("#UnitPrice").NumericValue()
                , OriginalUnitPrice: $("#UnitPrice").NumericValue()
                , OrderQty: $("#OrderQty").NumericValue()
                , dtNewInstrument: dtNewInstrument
                , Unit: $("#Unit").val()
        };

        call_ajax_method_json("/inventory/IVS250_ValidateAddInst", obj, function (result, controls) {
            if (controls != undefined) VaridateCtrl(["InstrumentCode", "UnitPrice", "OrderQty", "Unit"], controls);
            if (result != null) {
                var bIsEmptyGrid = CheckFirstRowIsEmpty(InstGrid, true);

                //#########################################################################################
                // Add new detail row.
                //#########################################################################################
                //var row_idx = AddNewDetailRow(InstGrid, [result.InstrumentCode, result.InstrumentName, "", "", result.Amount_view, ""]);
                var row_idx = AddNewDetailRow(InstGrid, [ConvertBlockHtml(result.InstrumentCode), ConvertBlockHtml(result.InstrumentName), "", "", "", ConvertBlockHtml(result.Unit), result.Amount_view, ""]); //Modify by Jutarat A. on 28112013

                var row_id = InstGrid.getRowId(row_idx);

                if (result.UnitPrice != null && ((result.UnitPrice).toString().indexOf('.', 0) == -1)) {
                    result.UnitPrice = result.UnitPrice.toString() + ".000";
                }

                InstGrid.cells(row_id, InstGrid.getColIndexById("Amount")).setAttribute("title", result.Amount_view);

                GenerateTextBox(InstGrid, "InstrumentName", row_id, "InstrumentName", result.InstrumentName, true, 100); //Add by Jutarat A. on 28102013
                //GenerateTextBox(InstGrid, "Memo", row_id, "Memo", "", true, 1000); //Add by Jutarat A. on 28102013

                GenerateGridCombobox(InstGrid, row_id, cboUnit, "Unit", "/Inventory/IVS250_GetUnit", result.Unit, true);
                InstGrid.cells(row_id, InstGrid.getColIndexById("Unit")).setAttribute("title", result.Unit);

                GenerateNumericBox2(InstGrid, OriginalUnitPrice, row_id, "OriginalUnitPrice", result.UnitPrice, 10, 3, 0, 9999999999.999, true, true);
                GenerateNumericBox2(InstGrid, UnitPrice, row_id, "UnitPrice", result.UnitPrice, 10, 3, 0, 9999999999.999, true, true);
                GenerateNumericBox2(InstGrid, OrderQty, row_id, "OrderQty", result.OrderQty, 5, 0, 0, 99999, true, true);
                GenerateRemoveButton(InstGrid, "btnRemove", row_id, "Remove", true);

                BindGridButtonClickEvent("btnRemove", row_id, function (rid) {
                    call_ajax_method_json("/inventory/IVS250_RemoveInst"
                        , { InstCode: InstGrid.cells(rid, InstGrid.getColIndexById("InstrumentCode")).getValue() }
                        , function (result, controls) {
                            if (result) {
                                DeleteRow(InstGrid, rid);

                                if (getDetailRowsNum() <= 0) {
                                    DeleteRow(InstGrid, _TTLRowId);
                                    DeleteRow(InstGrid, _DiscountRowId);
                                    DeleteRow(InstGrid, _TTLBeforeVatRowId);
                                    DeleteRow(InstGrid, _VatRowId);
                                    DeleteRow(InstGrid, _WHTRowId);
                                    DeleteRow(InstGrid, _TTLFinalRowId);

                                    _TTLRowId = null;
                                    _DiscountRowId = null;
                                    _TTLBeforeVatRowId = null;
                                    _WHTRowId = null;
                                    _VatRowId = null;
                                    _TTLFinalRowId = null;

                                    _isAddedInstrument = false;
                                    SetRegisterCommand(false, null);

                                } else {
                                    CalculateDiscountAmount();
                                }
                            }
                        }
                    ); // End IVS250_RemoveInst
                }); // End btnRemove Click  

                var orderID = "#" + GenerateGridControlID(OrderQty, row_id);
                $(orderID).change(function () {
                    if (+$(orderID).NumericValue() <= 0) {
                        master_event.LockWindow(true);
                        var obj = {
                            module: "inventory",
                            code: "MSG4020"
                        };
                        call_ajax_method("/Shared/GetMessage", obj, function (result) {
                            master_event.LockWindow(false);
                            OpenErrorMessageDialog(result.Code, result.Message, function () { $(orderID).focus(); }, null);
                        });
                    }

                    CalculateDiscountAmount();
                });

                var OriginalUnitPriceID = "#" + GenerateGridControlID(OriginalUnitPrice, row_id);
                $(OriginalUnitPriceID).change(function () {
                    CalculateDiscountAmount();
                });

                var UnitPriceID = "#" + GenerateGridControlID(UnitPrice, row_id);
                $(UnitPriceID).change(function () {
                    CalculateAmount(row_id, InstGrid.cells(row_id, InstGrid.getColIndexById("InstrumentCode")).getValue(), $(UnitPriceID).NumericValue(), null, null);
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

                //#########################################################################################
                // Add/Update data for Total row.
                //#########################################################################################
                if (bIsEmptyGrid) {
                    var TTLRowNum = 0;
                    var TTLRowId = 0;
                    AddNewRow(InstGrid, ["Before Discount", "", "", "", "", "", "", ""]);
                    TTLRowNum = InstGrid.getRowsNum() - 1;
                    TTLRowId = InstGrid.getRowId(TTLRowNum);
                    InstGrid.setColspan(TTLRowId, 0, 6);
                    InstGrid.cells(TTLRowId, InstGrid.getColIndexById("InstrumentCode")).cell.style.textAlign = 'right';
                    _TTLRowId = TTLRowId;

                    var DiscountRowNum = 0;
                    var DiscountRowId = 0;
                    AddNewRow(InstGrid, ["Discount", "", "", "", "", "", "", ""]);
                    DiscountRowNum = InstGrid.getRowsNum() - 1;
                    DiscountRowId = InstGrid.getRowId(DiscountRowNum);
                    GenerateNumericBox2(InstGrid, lblDiscount, DiscountRowId, "Amount", "", 14, 2, 0, 99999999999999.99, true, true);
                    InstGrid.setColspan(DiscountRowId, 0, 6);
                    InstGrid.cells(DiscountRowId, InstGrid.getColIndexById("InstrumentCode")).cell.style.textAlign = 'right';
                    _DiscountRowId = DiscountRowId;
                    $("#" + GenerateGridControlID(lblDiscount, _DiscountRowId))
                        .val("0.00")
                        .change(function () {
                            $(this).removeClass("highlight");
                            var value = +$(this).NumericValue();
                            var valueText = SetNumericText(value, 2);
                            InstGrid.cells(_DiscountRowId, InstGrid.getColIndexById("Amount")).setAttribute("title", valueText);
                            CalculateDiscountAmount();
                        });
                    InstGrid.cells(_DiscountRowId, InstGrid.getColIndexById("Amount")).setAttribute("title", "0.00");

                    GenerateCalculateButton(InstGrid, "btnCalculateDiscount", DiscountRowId, "Remove", true);

                    BindGridButtonClickEvent("btnCalculateDiscount", DiscountRowId, function (rid) {
                        CalculateDiscountAmount();
                    });

                    var TTLBeforeVatRowNum = 0;
                    var TTLBeforeVatRowId = 0;
                    AddNewRow(InstGrid, ["Before Vat", "", "", "", "", "", "", ""]);
                    TTLBeforeVatRowNum = InstGrid.getRowsNum() - 1;
                    TTLBeforeVatRowId = InstGrid.getRowId(TTLBeforeVatRowNum);
                    InstGrid.setColspan(TTLBeforeVatRowId, 0, 6);
                    InstGrid.cells(TTLBeforeVatRowId, InstGrid.getColIndexById("InstrumentCode")).cell.style.textAlign = 'right';
                    _TTLBeforeVatRowId = TTLBeforeVatRowId;

                    var WHTRowNum = 0;
                    var WHTRowId = 0;
                    AddNewRow(InstGrid, ["Withholding Tax", "", "", "", "", "", "", ""]);
                    WHTRowNum = InstGrid.getRowsNum() - 1;
                    WHTRowId = InstGrid.getRowId(WHTRowNum);
                    GenerateNumericBox2(InstGrid, lblWHTTotal, WHTRowId, "Amount", "", 14, 2, 0, 99999999999999.99, true, true);
                    InstGrid.setColspan(WHTRowId, 0, 6);
                    InstGrid.cells(WHTRowId, InstGrid.getColIndexById("InstrumentCode")).cell.style.textAlign = 'right';
                    _WHTRowId = WHTRowId;
                    $("#" + GenerateGridControlID(lblWHTTotal, WHTRowId))
                        .val("0.00")
                        .blur(function () {
                            $(this).removeClass("highlight");
                            var value = +$(this).NumericValue();
                            var valueText = SetNumericText(value, 2);
                            InstGrid.cells(_WHTRowId, InstGrid.getColIndexById("Amount")).setAttribute("title", valueText);
                            CalculateVatAmount(null, false, false);
                        });
                    InstGrid.cells(_WHTRowId, InstGrid.getColIndexById("Amount")).setAttribute("title", "0.00");

                    GenerateCalculateButton(InstGrid, "btnCalculate", WHTRowId, "Remove", true);

                    BindGridButtonClickEvent("btnCalculate", WHTRowId, function (rid) {
                        CalculateVatAmount(null, true, false);
                    });

                    var VatRowNum = 0;
                    var VatRowId = 0;
                    AddNewRow(InstGrid, ["Vat", "", "", "", "", "", "", ""]);
                    VatRowNum = InstGrid.getRowsNum() - 1;
                    VatRowId = InstGrid.getRowId(VatRowNum);
                    GenerateNumericBox2(InstGrid, lvlVatTotal, VatRowId, "Amount", "", 14, 2, 0, 99999999999999.99, true, true);
                    InstGrid.setColspan(VatRowId, 0, 6);
                    InstGrid.cells(VatRowId, InstGrid.getColIndexById("InstrumentCode")).cell.style.textAlign = 'right';
                    _VatRowId = VatRowId;
                    $("#" + GenerateGridControlID(lvlVatTotal, _VatRowId))
                        .val("0.00")
                        .blur(function () {
                            $(this).removeClass("highlight");
                            var value = +$(this).NumericValue();
                            var valueText = SetNumericText(value, 2);
                            InstGrid.cells(_VatRowId, InstGrid.getColIndexById("Amount")).setAttribute("title", valueText);
                            CalculateVatAmount(null, false, false);
                        });
                    InstGrid.cells(_VatRowId, InstGrid.getColIndexById("Amount")).setAttribute("title", "0.00");

                    var TTLFinalRowNum = 0;
                    var TTLFinalRowId = 0;
                    AddNewRow(InstGrid, ["Total Amount", "", "", "", "", "", "", ""]);
                    TTLFinalRowNum = InstGrid.getRowsNum() - 1;
                    TTLFinalRowId = InstGrid.getRowId(TTLFinalRowNum);
                    InstGrid.setColspan(TTLFinalRowId, 0, 6);
                    InstGrid.cells(TTLFinalRowId, InstGrid.getColIndexById("InstrumentCode")).cell.style.textAlign = 'right';
                    _TTLFinalRowId = TTLFinalRowId;

                    InstGrid.setRowHidden(_WHTRowId, true);
                    InstGrid.setRowHidden(_TTLBeforeVatRowId, true);
                    InstGrid.setRowHidden(_VatRowId, true);
                    //InstGrid.setRowHidden(_TTLBeforeVatRowId, $("#Currency").val() != c_currency_THB);
                    //InstGrid.setRowHidden(_VatRowId, $("#Currency").val() != c_currency_THB);
                }

                CalculateTotalAmount();

                InstGrid.setSizes();
                ClearInstrumentSection();

                $("#Unit").val("PCS");

                $("#InstrumentGridPlain").focus();

                _isAddedInstrument = true;

                if (_isSelectedSupplier && _isAddedInstrument) {
                    SetRegisterCommand(true, cmdRegister);
                }
                else {
                    SetRegisterCommand(false, null);
                }

            } // End if (result != null)
            master_event.LockWindow(false);
        }); // IVS250_ValidateAddInst        
    });                      // End btnAdd Click

    $("#btnDownload").click(function () {
        ajax_method.CallScreenController("/Inventory/IVS250_CheckExistFile", "", function (result) {
            if (result == 1) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Inventory/IVS250_DownloadPdfAndWriteLog?k=" + key);
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
    });

}

function btnClearClick() {
    $("#SupInfomation").clearForm();
    $("#SupCode").SetDisabled(false);
    $("#SupName").SetDisabled(false);
    $("#btnSearch").SetDisabled(false);

    ClearDateFromToControl("#POIssueDateFrom", "#POIssueDateTo");
    ClearDateFromToControl("#SearchExpectedDeliveryDateFrom", "#SearchExpectedDeliveryDateTo");

    _isSelectedSupplier = false;
    SetRegisterCommand(false, null);
}

function btnCancelClick() {
    $("#InstrumentCode").val("");
    $("#InstrumentName").val("");
    $("#UnitPrice").val("");
    $("#OrderQty").val("");
    $("#Unit").val("PCS");
}

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

function ClearInstrumentSection() {
    $("#InstInput").clearForm();
}

function getOriginalTotalAmount() {
    var totalAmount = 0;
    var decUnitPrice = 0;
    var decOrderQty = 0;

    for (var i = 0; i < getDetailRowsNum() ; i++) {
        var row_id = InstGrid.getRowId(i);

        decUnitPrice = +$("#" + GenerateGridControlID(OriginalUnitPrice, row_id)).NumericValue();
        decOrderQty = +$("#" + GenerateGridControlID(OrderQty, row_id)).NumericValue();

        totalAmount += Math.round10((decUnitPrice * decOrderQty), -2);
    }

    return Math.round10(totalAmount, -2);
}

function getTotalAmount() {
    var totalAmount = 0;
    var decUnitPrice = 0;
    var decOrderQty = 0;

    for (var i = 0; i < getDetailRowsNum() ; i++) {
        var row_id = InstGrid.getRowId(i);
        totalAmount += +(GetValueFromLinkType(InstGrid, InstGrid.getRowIndex(row_id), InstGrid.getColIndexById("Amount")).replace(/ /g, "").replace(/,/g, ""));
    }

    return Math.round10(totalAmount, -2);
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
    //    CalculateAmount(row_id, InstGrid.cells(row_id, InstGrid.getColIndexById("InstrumentCode")).getValue(), $(UnitPriceID).NumericValue(), null);
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
    for (var i = 0; i < getDetailRowsNum() ; i++) {
        var row_id = InstGrid.getRowId(i);

        var decUnitPrice = +$("#" + GenerateGridControlID(OriginalUnitPrice, row_id)).NumericValue();
        var decOrderQty = +$("#" + GenerateGridControlID(OrderQty, row_id)).NumericValue();
        var decAmount = Math.round10((decOrderQty * decUnitPrice), -2);

        var objAmount = {
            RowId: row_id,
            UnitPrice: decUnitPrice,
            Qty: decOrderQty,
            Amount: decAmount,
            DiscountedUnitPrice: Math.round10(Math.round10(decAmount * decAmountPercentage, -2) / decOrderQty, -3)
        };

        objAmount.DiscountedAmount = Math.round10(objAmount.DiscountedUnitPrice * decOrderQty, -2)

        decTotalAmountTemp = Math.round10(decTotalAmountTemp + objAmount.DiscountedAmount, -2);
        arrAmount.push(objAmount);
    }

    decTotalAmountDelta = Math.round10(decTotalAmount - decTotalAmountTemp, -2);

    if (arrAmount.length > 0) {
        if (decTotalAmountDelta != 0) {
            arrAmount[arrAmount.length - 1].DiscountedAmount += decTotalAmountDelta;
        }
    }

    for (var i = 0; i < arrAmount.length; i++) {
        var obj = arrAmount[i];
        var decUnitPriceText = SetNumericText(obj.DiscountedUnitPrice, 3);
        var UnitPriceID = "#" + GenerateGridControlID(UnitPrice, obj.RowId);
        $(UnitPriceID).val(decUnitPriceText);
        InstGrid.cells(obj.RowId, InstGrid.getColIndexById("UnitPrice")).setAttribute("title", decUnitPriceText);
        CalculateAmount(
            obj.RowId
            , InstGrid.cells(obj.RowId, InstGrid.getColIndexById("InstrumentCode")).getValue()
            , obj.DiscountedUnitPrice + ""
            , obj.Qty + ""
            , obj.DiscountedAmount + ""
        );
    }
    InstGrid.setSizes();

    CalculateTotalAmount();
}

function CalculateTotalAmount() {
    var decUnitPrice = 0;
    var decOrderQty = 0;
    var decTotalAmount = getOriginalTotalAmount();
    var totalAmountText = SetNumericText(decTotalAmount, 2);

    if (_TTLRowId) {
        InstGrid.cells(_TTLRowId, InstGrid.getColIndexById("Amount")).setValue(totalAmountText);
        InstGrid.cells(_TTLRowId, InstGrid.getColIndexById("Amount")).setAttribute("title", totalAmountText);
    }

    ////var decDiscountAmount = +$("#" + GenerateGridControlID(lblDiscount, _DiscountRowId)).NumericValue();

    ////var totalAmountBeforeVat = decTotalAmount - decDiscountAmount;
    ////var totalAmountBeforeVatText = SetNumericText(totalAmountBeforeVat, 2);
    var totalAmountBeforeVat = getTotalAmount();
    var totalAmountBeforeVatText = SetNumericText(totalAmountBeforeVat, 2);

    //SetValue grid
    if (_TTLBeforeVatRowId) {
        InstGrid.cells(_TTLBeforeVatRowId, InstGrid.getColIndexById("Amount")).setValue(totalAmountBeforeVatText);
        InstGrid.cells(_TTLBeforeVatRowId, InstGrid.getColIndexById("Amount")).setAttribute("title", totalAmountBeforeVatText);

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

    CalculateVatAmount(totalAmountBeforeVat, false, true);
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
    //        InstGrid.cells(_WHTRowId, InstGrid.getColIndexById("Amount")).setAttribute("title", whtAmountText);
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
        InstGrid.cells(_WHTRowId, InstGrid.getColIndexById("Amount")).setAttribute("title", whtAmountText);
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
            InstGrid.cells(_VatRowId, InstGrid.getColIndexById("Amount")).setAttribute("title", vatAmountText);
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
        InstGrid.cells(_TTLFinalRowId, InstGrid.getColIndexById("Amount")).setValue(totalAmountFinalText);
        InstGrid.cells(_TTLFinalRowId, InstGrid.getColIndexById("Amount")).setAttribute("title", totalAmountFinalText);

        var remark = InstGrid.cells(_TTLBeforeVatRowId, InstGrid.getColIndexById("Remove")).getValue();
        InstGrid.cells(_TTLFinalRowId, InstGrid.getColIndexById("Remove")).setValue(remark);
    }
    InstGrid.setSizes();
}

function CalculateAmount(row_id, InstCode, UnitPrice, OrderQty, Amount) {
    var obj = {
        InstCode: InstCode,
        UnitPrice: UnitPrice,
        OrderQty: OrderQty,
        Amount: Amount
    };
    call_ajax_method_json("/inventory/IVS250_CalculateAmount", obj, function (result, controls) {
        if (result != null) {
            var idxColAmount = InstGrid.getColIndexById("Amount");

            InstGrid.cells(row_id, idxColAmount).setValue(result.Amount);
            InstGrid.cells(row_id, idxColAmount).setAttribute("title", result.Amount);

            CalculateTotalAmount();
        }
    });


}

//Add by Jutarat A. on 28102013
function getGridData() {
    var InstList = null;

    if (CheckFirstRowIsEmpty(InstGrid, false) == false) {
        InstList = new Array();

        for (var i = 0; i < getDetailRowsNum() ; i++) {
            var row_id = InstGrid.getRowId(i);
            var Inst = {
                InstrumentCode: InstGrid.cells(row_id, InstGrid.getColIndexById("InstrumentCode")).getValue(),
                InstrumentName: $("#" + GenerateGridControlID("InstrumentName", row_id)).val(),
                //Memo: $("#" + GenerateGridControlID("Memo", row_id)).val(),
                OriginalUnitPrice: $("#" + GenerateGridControlID(OriginalUnitPrice, row_id)).NumericValue(),
                UnitPrice: $("#" + GenerateGridControlID(UnitPrice, row_id)).NumericValue(),
                OrderQty: $("#" + GenerateGridControlID(OrderQty, row_id)).NumericValue(),
                Unit: $("#" + GenerateGridControlID(cboUnit, row_id)).val(),
                UnitCtrlID: GenerateGridControlID(cboUnit, row_id),
                Amount: GetValueFromLinkType(InstGrid, InstGrid.getRowIndex(row_id), InstGrid.getColIndexById("Amount")).replace(/ /g, "").replace(/,/g, "")
            };
            InstList.push(Inst);
        }
    }

    return InstList;
}
//End Add

function cmdRegister() {
    command_control.CommandControlMode(false);

    var obj = {
        SupplierCode: $("#SupCode").val(),
        SupplierName: $("#SupName").val(),

        BankName: $("#BankName").val(),
        AccountNo: $("#AccNo").val(),
        AccountName: $("#AccName").val(),

        PurchaseOrderType: $("#PorderType").val(),
        TransportType: $("#TransportType").val(),
        AdjustDueDate: $("#AdjustDueDate").val(),
        Currency: $("#Currency").val(),
        Memo: $("#memo").val(),
        TotalAmount: 0,
        Vat: 0,
        InstrumentData: getGridData(), //Add by Jutarat A. on 28102013
        Discount: 0,
        WHT: 0
    };

    if (InstGrid.getRowsNum() > 0 && CheckFirstRowIsEmpty(InstGrid) == false) {
        obj.TotalAmount = GetValueFromLinkType(InstGrid, InstGrid.getRowIndex(_TTLBeforeVatRowId), InstGrid.getColIndexById("Amount")).replace(/ /g, "").replace(/,/g, "");
        obj.Vat = $("#" + GenerateGridControlID(lvlVatTotal, _VatRowId)).NumericValue();
        _VatTotalText = $("#" + GenerateGridControlID(lvlVatTotal, _VatRowId)).val(); //Add by Jutarat A. on 01082013
        obj.Discount = $("#" + GenerateGridControlID(lblDiscount, _DiscountRowId)).NumericValue();
        obj.WHT = $("#" + GenerateGridControlID(lblWHTTotal, _WHTRowId)).NumericValue();
        _WHTTotalText = $("#" + GenerateGridControlID(lblWHTTotal, _WHTRowId)).val();
    }

    call_ajax_method_json("/inventory/IVS250_ValidateRegis", obj, function (result, controls) {

        if (controls != undefined) {
            VaridateCtrl(["SupCode", "SupName", "PorderType", "TransportType", "AdjustDueDate", "Currency"], controls);
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
        } else {
            if (result != null) {
                if (result == "4022") {
                    //$("#memo").addClass("highlight");
                    //clearHightLight("#memo");
                    VaridateCtrl(["memo"], ["memo"]);
                }
                else {

                    call_ajax_method_json("/inventory/IVS250_ValidateRegis_2", "", function (result, controls) {
                        if (result) {
                            InstGrid.setColumnHidden(InstGrid.getColIndexById("Remove"), true);
                            SetFitColumnForBackAction(InstGrid, "TempColumn");
                            $("#SpecifyPurchaseOrderInstrument").SetViewMode(true);
                            $("#SpecifyPurchaseOrder").SetViewMode(true);
                            $("#SupInfomation").SetViewMode(true);
                            $("#InstInput").hide();

                            SetRegisterCommand(false, null);
                            SetResetCommand(false, null);
                            SetConfirmCommand(true, cmdConfirm);
                            SetBackCommand(true, cmdBack);
                            //  reset_command.SetCommand(null);
                        } else {
                            var obj = {
                                module: "Inventory",
                                code: "MSG4024"
                            };
                            call_ajax_method("/Shared/GetMessage", obj, function (result) {
                                OpenYesNoMessageDialog(result.Code, result.Message,
                                function () {
                                    InstGrid.setColumnHidden(InstGrid.getColIndexById("Remove"), true);
                                    SetFitColumnForBackAction(InstGrid, "TempColumn");
                                    $("#SpecifyPurchaseOrderInstrument").SetViewMode(true);
                                    $("#SpecifyPurchaseOrder").SetViewMode(true);
                                    $("#SupInfomation").SetViewMode(true);
                                    $("#InstInput").hide();
                                    SetRegisterCommand(false, null);
                                    SetResetCommand(false, null);
                                    SetConfirmCommand(true, cmdConfirm);
                                    SetBackCommand(true, cmdBack);
                                },
                                null);
                            });
                        }
                    });
                }
            }
        }

        command_control.CommandControlMode(true);
    });
}
function cmdConfirm() {
    command_control.CommandControlMode(false);
    ajax_method.CallScreenController("/inventory/IVS250_cmdConfirm", "", function (result, controls) {
        if (result != null && result != "") {
            var param = { "module": "Common", "code": "MSG0046" };
            call_ajax_method("/Shared/GetMessage", param, function (data) {
                /* ====== Open info dialog =====*/
                OpenInformationMessageDialog(param.code, data.Message, function () {
                    $("#resOfRegSection").show();
                    $("#PorderNo").val(result);
                    $("#PorderNo").focus();
                    SetRegisterCommand(false, null);
                    SetResetCommand(false, null);
                    SetConfirmCommand(false, null);
                    SetBackCommand(false, null);
                });
            });
        }

        command_control.CommandControlMode(true);
    });
}
function cmdBack() {
    InstGrid.setColumnHidden(InstGrid.getColIndexById("Remove"), false);
    SetFitColumnForBackAction(InstGrid, "TempColumn");
    $("#SpecifyPurchaseOrderInstrument").SetViewMode(false);
    $("#SpecifyPurchaseOrder").SetViewMode(false);
    $("#SupInfomation").SetViewMode(false);
    $("#InstInput").show();

    SetRegisterCommand(true, cmdRegister);
    SetResetCommand(true, cmdReset);
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);

    GenerateNumericBox2(InstGrid, lvlVatTotal, _VatRowId, "Amount", _VatTotalText, 16, 2, 0, 99999999999999.99, true, true); //Add by Jutarat A. on 01082013
    InstGrid.setSizes();
}
function cmdReset() {
    var obj = {
        module: "Common",
        code: "MSG0038"
    };
    call_ajax_method("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
        function () {
            call_ajax_method_json("/inventory/IVS250_resetParam", "", function (result, controls) {
                DeleteAllRow(InstGrid);
                initScreen();
                btnClearClick();
                btnCancelClick();
            });

        },
        null);
    });


}
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
    return {
        bExpTypeHas: true,
        bExpTypeNo: true,
        bProdTypeSale: false,
        bProdTypeAlarm: false,
        bInstTypeGen: true,
        bInstTypeMonitoring: false,
        bInstTypeMat: true
    };
}
function addCommas(nStr) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}
function chkNum(ele) {
    var num = parseFloat(ele);
    //  addCommas(num.toFixed(2));
    return addCommas(num.toFixed(2));
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

function clearHightLight(ctrl) {
    $(ctrl).keyup(function () {
        $(ctrl).removeClass("highlight");
    });
}

function getDetailRowsNum() {
    return InstGrid.getRowsNum()
        - (_TTLRowId ? 1 : 0)
        - (_DiscountRowId ? 1 : 0)
        - (_TTLBeforeVatRowId ? 1 : 0)
        - (_WHTRowId ? 1 : 0)
        - (_VatRowId ? 1 : 0)
        - (_TTLFinalRowId ? 1 : 0);
}