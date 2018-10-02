/// <reference path="../../Base/GridControl.js" />

/* --- Variable --- */
var dtNewInstrument = null;
var gridInst01 = null;
var instrument_quantity = "InstQuantity";
var btn_remove = "btnRemoveInstrument";


function QUS030_07_InitialGrid() {
    $("#InstrumentCode").blur(GetInstrument01InfoData);
    $("#InstrumentQty").BindNumericBox(5, 0, 0, 99999, true);

    /* --- Initial Grid --- */
    /* -------------------- */
    gridInst01 = $("#gridInstrumentDetail01").LoadDataToGridWithInitial(0, false, false,
                                "/Quotation/QUS030_GetInstrumentDetail01Data",
                                "",
                                "doInstrumentDetail", false);
    /* -------------------- */

    SpecialGridControl(gridInst01, ["InstrumentQty", "Remove"]);

    BindOnLoadedEvent(gridInst01, function (gen_ctrl) {
        for (var i = 0; i < gridInst01.getRowsNum(); i++) {
            var row_id = gridInst01.getRowId(i);
            var flagCol = gridInst01.getColIndexById("MaintenanceFlag");
            var qtyCol = gridInst01.getColIndexById("InstrumentQty");
            var defCol = gridInst01.getColIndexById("IsDefault");

            if (gen_ctrl == true) {
                /* --- Set Start --- */
                /* ----------------- */
                var flag = gridInst01.cells2(i, flagCol).getValue();
                if (flag == 1 || flag == true)
                    flag = GenerateStarImage();
                else
                    flag = "";
                gridInst01.cells2(i, flagCol).setValue(flag);
                /* ----------------- */

                /* --- Enable/Diable Button --- */
                /* ---------------------------- */
                var enable_txt = true;
                var enable = true;
                if (GetDisableFlag("QuantityTextBox") == true) {
                    enable_txt = false;
                }
                if (GetDisableFlag("RemoveButton") == true) {
                    enable = false;
                }
                else if (gridInst01.cells2(i, defCol).getValue() == 1) {
                    enable = false;
                }
                /* ---------------------------- */

                var val = GetValueFromLinkType(gridInst01, i, qtyCol);
                GenerateNumericBox2(gridInst01, instrument_quantity, row_id, "InstrumentQty", val, 5, 0, 0, 99999, true, enable_txt);
                GenerateRemoveButton(gridInst01, btn_remove, row_id, "Remove", enable);
            }

            BindGridButtonClickEvent(btn_remove, row_id, function (rid) {
                DeleteRow(gridInst01, rid);
            });
            /* ----------------- */
        }
        gridInst01.setSizes();
    });

    /* --- Add Events --- */
    $("#btnAddInstrument").click(function () {
        /* --- Set Parameter --- */
        /* --------------------- */
        var vv = new Array();
        if (CheckFirstRowIsEmpty(gridInst01) == false) {
            var codeCol = gridInst01.getColIndexById("InstrumentCode");
            var defCol = gridInst01.getColIndexById("IsDefault");

            for (var i = 0; i < gridInst01.getRowsNum(); i++) {
                var iobj = {
                    InstrumentCode: gridInst01.cells2(i, codeCol).getValue(),
                    IsDefaultFlag: (gridInst01.cells2(i, defCol).getValue() == 1)
                };
                vv.push(iobj);
            }
        }
        var obj = {
            InstrumentCode: $("#InstrumentCode").val(),
            InstrumentQty: $("#InstrumentQty").NumericValue(),
            InstrumentList: vv,
            doInstrumentData: dtNewInstrument
        };
        /* --------------------- */

        call_ajax_method_json("/Quotation/QUS030_CheckBeforeAddInstrument", obj, function (result, controls) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["InstrumentCode", "InstrumentQty"], controls);
            /* --------------------- */

            if (controls != undefined) {
                return;
            }
            else if (result == true) {
                /* --- Check Empty Row --- */
                /* ----------------------- */
                CheckFirstRowIsEmpty(gridInst01, true);
                /* ----------------------- */

                /* --- Set Flag --- */
                /* ---------------- */
                var flag = "";
                if (dtNewInstrument.MaintenanceFlag == true)
                    flag = GenerateStarImage();
                /* ---------------- */

                /* --- Add Row --- */
                /* --------------- */
                AddNewRow(gridInst01,
                        [flag,
                        ConvertBlockHtml(dtNewInstrument.InstrumentCode),
                        ConvertBlockHtml(dtNewInstrument.InstrumentName),
                        dtNewInstrument.LineUpTypeCodeName,
                        "",
                        "",
                        "",
                        "",
                        dtNewInstrument.SaleFlag,
                        dtNewInstrument.RentalFlag,
                        dtNewInstrument.LineUpTypeCode]);

                var row_idx = gridInst01.getRowsNum() - 1;
                var row_id = gridInst01.getRowId(row_idx);

                GenerateNumericBox2(gridInst01, instrument_quantity, row_id, "InstrumentQty", $("#InstrumentQty").val(), 5, 0, 0, 99999, true, true);
                GenerateRemoveButton(gridInst01, btn_remove, row_id, "Remove", true);
                gridInst01.setSizes();
                /* --------------- */
    
                BindGridButtonClickEvent(btn_remove, row_id, function (rid) {
                    DeleteRow(gridInst01, rid);
                });
                /* ----------------- */

                ClearInstrument01Data();
            }
        });
    });
    /* --- Cancel Events --- */
    $("#btnCancelInstrument").click(function () {
        ClearInstrument01Data();
    });

    $("#btnSearchInstrument").click(search_instrument_click);
}


/* --- Initial -------------------------------------------------------- */
/* -------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "InstrumentCode"
    ]);

    if ($("#IsDefaultInstrument").val() == "True") {
        $("#btnShowDefaultInstrument").attr("disabled", true);
    }
    $("#btnShowDefaultInstrument").click(show_default_instrument_click);

    ClearInstrument01Data();

    if (GetDisableFlag("AllControls") == true) {
        $("#btnShowDefaultInstrument").attr("disabled", true);
        $("#InstrumentCode").attr("readonly", true);
        $("#btnSearchInstrument").attr("disabled", true);
        $("#InstrumentQty").attr("readonly", true);
        $("#btnAddInstrument").attr("disabled", true);
        $("#btnCancelInstrument").attr("disabled", true);
    }
});
/* -------------------------------------------------------------------- */

/* --- Events --------------------------------------------------------- */
/* -------------------------------------------------------------------- */
function show_default_instrument_click() {
    if (gridInst01 == undefined)
        return null;

    var isAsk = (CheckFirstRowIsEmpty(gridInst01) == false);
    var obj = {
        ProductCode: $("#ProductCode").val(),
        IsAskQuestion: isAsk
    };
    if (isAsk) {
        call_ajax_method_json("/Quotation/QUS030_GetDefaultInstrumentDetail01Data_P1", obj, function (result, controls) {
            if (controls != undefined) {
                /* --- Higlight Text --- */
                /* --------------------- */
                VaridateCtrl(["ProductCode"], controls);
                /* --------------------- */

                return;
            }
            else if (result != undefined) {
                OpenYesNoMessageDialog(result.Code, result.Message, function () {
                    $("#gridInstrumentDetail01").LoadDataToGrid(gridInst01, 0, false,
                                    "/Quotation/QUS030_GetDefaultInstrumentDetail01Data_P2", "", "doDefaultInstrument", false);
                }, null);
            }
        });
    }
    else {
        $("#gridInstrumentDetail01").LoadDataToGrid(gridInst01, 0, false,
                                    "/Quotation/QUS030_GetDefaultInstrumentDetail01Data_P1", obj, "doDefaultInstrument", false,
            function (result, controls) {
                if (controls != undefined) {
                    /* --- Higlight Text --- */
                    /* --------------------- */
                    VaridateCtrl(["ProductCode"], controls);
                    /* --------------------- */

                    return;
                }
            });
    }

    }

    function search_instrument_click() {
        var obj = {
            bExpTypeHas: true,
            bExpTypeNo: false,
            bProdTypeSale: ProductTypeMode.IsProductTypeSale,
            bProdTypeAlarm: ProductTypeMode.IsProductTypeAL || ProductTypeMode.IsProductTypeRentalSale,
            bInstTypeGen: true,
            bInstTypeMonitoring: false,
            bInstTypeMat: false
        };
        var func = function (result) {
            $("#InstrumentCode").ResetToNormalControl();
            $("#InstrumentQty").ResetToNormalControl();

            dtNewInstrument = result;
            $("#InstrumentCode").val(result.InstrumentCode);
            $("#InstrumentName").val(result.InstrumentName);
        };

        SearchInstrument(obj, func);
    }
/* -------------------------------------------------------------------- */

/* --- Methods -------------------------------------------------------- */
/* -------------------------------------------------------------------- */
function ClearInstrument01Data() {
    $("#InstrumentCode").val("");
    $("#InstrumentName").val("");
    $("#InstrumentQty").val("0");

    $("#InstrumentCode").ResetToNormalControl();
    $("#InstrumentQty").ResetToNormalControl();
    dtNewInstrument = null;
}
function GetInstrument01InfoData() {
    $("#InstrumentName").val("");
    if ($.trim($(this).val()) != "") {
        /* --- Set Parameter --- */
        var obj = {
            InstrumentCode: $(this).val()
        };

        call_ajax_method_json("/Quotation/QUS030_GetInstrumentDetailInfo", obj, function (result) {
            if (result != undefined) {
                dtNewInstrument = result;
                $("#InstrumentName").val(dtNewInstrument.InstrumentName);
            }
            else {
                dtNewInstrument = null;
            }
        });
    }
}
/* -------------------------------------------------------------------- */

/* --- Output Methods ---------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function GetQUS030_07_SectionData() {
    if (gridInst01 == undefined)
        return null;

    var vv = new Array();
    if (CheckFirstRowIsEmpty(gridInst01) == false) {
        var codeCol = gridInst01.getColIndexById("InstrumentCode");
        var defCol = gridInst01.getColIndexById("IsDefault");
        var sfCol = gridInst01.getColIndexById("SaleFlag");
        var rfCol = gridInst01.getColIndexById("RentalFlag");
        var ltCol = gridInst01.getColIndexById("LineUpTypeCode");

        for (var i = 0; i < gridInst01.getRowsNum(); i++) {
            var row_id = gridInst01.getRowId(i);
            var txt = "#" + GenerateGridControlID(instrument_quantity, row_id);

            var obj = {
                InstrumentCode: gridInst01.cells2(i, codeCol).getValue(),
                InstrumentQty: $(txt).NumericValue(),
                IsDefaultFlag: (gridInst01.cells2(i, defCol).getValue() == 1 || gridInst01.cells2(i, defCol).getValue() == "true"),
                SaleFlag: (gridInst01.cells2(i, sfCol).getValue() == 1 || gridInst01.cells2(i, sfCol).getValue() == "true"),
                RentalFlag: (gridInst01.cells2(i, rfCol).getValue() == 1 || gridInst01.cells2(i, rfCol).getValue() == "true"),
                LineUpTypeCode: gridInst01.cells2(i, ltCol).getValue()
            };
            vv.push(obj);
        }
    }

    if (vv.length > 0)
        return vv;
    else
        return null;
}
function SetQUS030_07_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode for Instrument detail section</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>

    if (isview) {
        $("#divInstrumentDetail_01").SetViewMode(true);
        $("#divInstrumentAdd").hide();

        if (gridInst01 != undefined) {
            $("#divInstrumentDetail_01").SetNumericControlViewInGrid([instrument_quantity]);

            var removeCol = gridInst01.getColIndexById("Remove");
            gridInst01.setColumnHidden(removeCol, true);
            gridInst01.setSizes();
        }
    }
    else {
        $("#divInstrumentDetail_01").SetViewMode(false);
        $("#divInstrumentAdd").show();

        if (gridInst01 != undefined) {
            var removeCol = gridInst01.getColIndexById("Remove");
            gridInst01.setColumnHidden(removeCol, false);
            for (var i = 0; i < gridInst01.getRowsNum(); i++) {
                gridInst01.setColspan(gridInst01.getRowId(i), removeCol, 2);
            }
        }
    }
}
function GetQUS030_07_SectionValidation(controls) {
    /// <summary>Method to validate data for Instrument detail section</summary>
    /// <param name="controls" type="array">List of controls that error</param>

    var colQtyIdx;
    if (controls != undefined) {
        for (var idx = 0; idx < controls.length; idx++) {
            if (controls[idx] == null || controls[idx] == undefined)
                continue;

            if (controls[idx].indexOf("InstrumentDetail;") >= 0) {
                var errCols = controls[idx].replace("InstrumentDetail;", "").split(";");
                for (var i = 0; i < errCols.length; i++) {
                    if (errCols[i] == instrument_quantity
                        && (i + 1) < errCols.length) {
                        i += 1;
                        colQtyIdx = errCols[i];
                    }
                }

                break;
            }
        }
    }

    var ctrlLst = new Array();
    var errLst = new Array();
    for (var i = 0; i < gridInst01.getRowsNum(); i++) {
        var row_id = gridInst01.getRowId(i);
        var txt = GenerateGridControlID(instrument_quantity, row_id);

        ctrlLst.push(txt);
        if (colQtyIdx != undefined) {
            if (colQtyIdx == "ALL" || colQtyIdx == i) {
                errLst.push(txt);
            }
        }
    }
    VaridateCtrl(ctrlLst, errLst);
}
/* ----------------------------------------------------------------------------------- */

function QUS030_07_ResetToNormalControl() {
    $("#divInstrumentDetail_01").ResetToNormalControl();
}