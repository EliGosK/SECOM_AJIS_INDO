/* --- Variable --- */
var dtNewInstrument = null;
var gridInst02 = null;
var installed_before_change = "InstalledBeforeChange";
var change_quantity_additional = "ChangeQuantityAdditional";
var change_quantity_removal = "ChangeQuantityRemoval";
var installed_quantity = "InstalledQuantity";
var btn_remove = "btnRemove";




function QUS030_08_InitialGrid() {
    $("#InstrumentCode").blur(GetInstrument02InfoData);
    $("#InstrumentQty").BindNumericBox(5, 0, 0, 99999, true);

    /* --- Initial Grid --- */
    /* -------------------- */
    gridInst02 = $("#gridInstrumentDetail02").LoadDataToGridWithInitial(0, false, false,
                                "/Quotation/QUS030_GetInstrumentDetail02Data",
                                "",
                                "doInstrumentDetail", false);
    /* -------------------- */

    SpecialGridControl(gridInst02, ["InstrumentQty", "AddQty", "RemoveQty", "InstalledQty", "Remove"]);

    BindOnLoadedEvent(gridInst02, function (gen_ctrl) {
        for (var i = 0; i < gridInst02.getRowsNum(); i++) {
            var row_id = gridInst02.getRowId(i);

            var flagCol = gridInst02.getColIndexById("MaintenanceFlag");
            var qtyCol = gridInst02.getColIndexById("InstrumentQty");
            var addQtyCol = gridInst02.getColIndexById("AddQty");
            var removeQtyCol = gridInst02.getColIndexById("RemoveQty");
            var installedQtyCol = gridInst02.getColIndexById("InstalledQty");
            var defCol = gridInst02.getColIndexById("IsDefault");

            if (gen_ctrl == true) {
                /* --- Set Start --- */
                /* ----------------- */
                var flag = gridInst02.cells2(i, flagCol).getValue();
                if (flag == 1 || flag == true)
                    flag = GenerateStarImage();
                else
                    flag = "";
                gridInst02.cells2(i, flagCol).setValue(flag);
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
                else if (gridInst02.cells2(i, defCol).getValue() == 1) {
                    enable = false;
                }
                /* ---------------------------- */

                var val1 = GetValueFromLinkType(gridInst02, i, qtyCol);
                var val2 = GetValueFromLinkType(gridInst02, i, addQtyCol);
                var val3 = GetValueFromLinkType(gridInst02, i, removeQtyCol);
                var val4 = GetValueFromLinkType(gridInst02, i, installedQtyCol);

                GenerateNumericBox2(gridInst02, installed_before_change, row_id, "InstrumentQty", val1, 5, 0, 0, 99999, true, false);
                GenerateNumericBox2(gridInst02, change_quantity_additional, row_id, "AddQty", val2, 5, 0, 0, 99999, true, enable_txt);
                GenerateNumericBox2(gridInst02, change_quantity_removal, row_id, "RemoveQty", val3, 5, 0, 0, 99999, true, enable_txt);
                GenerateNumericBox2(gridInst02, installed_quantity, row_id, "InstalledQty", val4, 5, 0, 0, 99999, true, false);
                GenerateRemoveButton(gridInst02, btn_remove, row_id, "Remove", enable);
            }

            /* --- Set Event --- */
            /* ----------------- */
            if (enable_txt) {
                var txt2 = "#" + GenerateGridControlID(change_quantity_additional, row_id);
                $(txt2).CalInstalledQty();

                var txt3 = "#" + GenerateGridControlID(change_quantity_removal, row_id);
                $(txt3).CalInstalledQty();
            }

            BindGridButtonClickEvent(btn_remove, row_id, function (rid) {
                DeleteRow(gridInst02, rid);
            });
            /* ----------------- */
        }
        gridInst02.setSizes();
    });

    /* --- Add Events --- */
    $("#btnAdd").click(function () {
        /* --- Set Parameter --- */
        /* --------------------- */
        var vv = new Array();
        if (CheckFirstRowIsEmpty(gridInst02) == false) {
            var codeCol = gridInst02.getColIndexById("InstrumentCode");
            var defCol = gridInst02.getColIndexById("IsDefault");

            for (var i = 0; i < gridInst02.getRowsNum(); i++) {
                var iobj = {
                    InstrumentCode: gridInst02.cells2(i, codeCol).getValue(),
                    IsDefaultFlag: (gridInst02.cells2(i, defCol).getValue() == 1)
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
                CheckFirstRowIsEmpty(gridInst02, true);
                /* ----------------------- */

                /* --- Set Flag --- */
                /* ---------------- */
                var flag = "";
                if (dtNewInstrument.MaintenanceFlag == true)
                    flag = GenerateStarImage();
                /* ---------------- */

                /* --- Add Row --- */
                /* --------------- */
                AddNewRow(gridInst02,
                        [flag,
                        ConvertBlockHtml(dtNewInstrument.InstrumentCode),
                        ConvertBlockHtml(dtNewInstrument.InstrumentName),
                        dtNewInstrument.LineUpTypeCodeName,
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        dtNewInstrument.SaleFlag,
                        dtNewInstrument.RentalFlag,
                        dtNewInstrument.LineUpTypeCode]);

                var row_idx = gridInst02.getRowsNum() - 1;
                var row_id = gridInst02.getRowId(row_idx);

                GenerateNumericBox2(gridInst02, installed_before_change, row_id, "InstrumentQty", 0, 5, 0, 0, 99999, true, false);
                GenerateNumericBox2(gridInst02, change_quantity_additional, row_id, "AddQty", $("#InstrumentQty").val(), 5, 0, 0, 99999, true, true);
                GenerateNumericBox2(gridInst02, change_quantity_removal, row_id, "RemoveQty", 0, 5, 0, 0, 99999, true, false);
                GenerateNumericBox2(gridInst02, installed_quantity, row_id, "InstalledQty", $("#InstrumentQty").val(), 5, 0, 0, 99999, true, false);
                GenerateRemoveButton(gridInst02, btn_remove, row_id, "Remove", true);
                gridInst02.setSizes();
                /* --------------- */

                /* --- Set Event --- */
                /* ----------------- */
                var txt2 = "#" + GenerateGridControlID(change_quantity_additional, row_id);
                $(txt2).CalInstalledQty();

                BindGridButtonClickEvent(btn_remove, row_id, function (rid) {
                    DeleteRow(gridInst02, rid);
                });
                /* ----------------- */

                ClearInstrument02Data();
            }
        });
    });
    /* --- Cancel Events --- */
    $("#btnCancel").click(function () {
        ClearInstrument02Data();
    });


    $("#btnSearchInstrument").click(search_instrument_click);
}



/* --- Initial -------------------------------------------------------- */
/* -------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "InstrumentCode"
    ]);

    ClearInstrument02Data();

    if (GetDisableFlag("AllControls") == true) {
        $("#InstrumentCode").attr("readonly", true);
        $("#btnSearchInstrument").attr("disabled", true);
        $("#InstrumentQty").attr("readonly", true);
        $("#btnAdd").attr("disabled", true);
        $("#btnCancel").attr("disabled", true);
    }
});
/* -------------------------------------------------------------------- */



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


/* --- Methods -------------------------------------------------------- */
/* -------------------------------------------------------------------- */
function ClearInstrument02Data() {
    $("#InstrumentCode").val("");
    $("#InstrumentName").val("");
    $("#InstrumentQty").val("0");

    $("#InstrumentCode").ResetToNormalControl();
    $("#InstrumentQty").ResetToNormalControl();
    dtNewInstrument = null;
}
function GetInstrument02InfoData() {
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
$.fn.CalInstalledQty = function () {
    $(this).blur(function () {
        var row_id = GetGridRowIDFromControl(this);
        var txt1 = "#" + GenerateGridControlID(installed_before_change, row_id);
        var txt2 = "#" + GenerateGridControlID(change_quantity_additional, row_id);
        var txt3 = "#" + GenerateGridControlID(change_quantity_removal, row_id);
        var txt4 = "#" + GenerateGridControlID(installed_quantity, row_id);

        var ibc = 0;
        if ($(txt1).NumericValue() != "")
            ibc = parseInt($(txt1).NumericValue());

        var caq = 0;
        if ($(txt2).NumericValue() != "")
            caq = parseInt($(txt2).NumericValue());

        var crq = 0;
        if ($(txt3).NumericValue() != "")
            crq = parseInt($(txt3).NumericValue());

        if (crq > ibc) {
            crq = ibc;
            $(txt3).val(ibc).setComma();
        }
        var iq = ibc + caq - crq;
        if (iq < 0) {
            crq = ibc + caq;
            iq = 0;
            $(txt3).val(crq).setComma();
        }
        $(txt4).val(iq).setComma();
    });
}
/* -------------------------------------------------------------------- */

/* --- Output Methods ---------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function GetQUS030_08_SectionData() {
    if (gridInst02 == undefined)
        return null;

    var vv = new Array();
    if (CheckFirstRowIsEmpty(gridInst02) == false) {
        var codeCol = gridInst02.getColIndexById("InstrumentCode");
        var defCol = gridInst02.getColIndexById("IsDefault");
        var sfCol = gridInst02.getColIndexById("SaleFlag");
        var rfCol = gridInst02.getColIndexById("RentalFlag");
        var ltCol = gridInst02.getColIndexById("LineUpTypeCode");

        for (var i = 0; i < gridInst02.getRowsNum(); i++) {
            var row_id = gridInst02.getRowId(i);
            var txt1 = "#" + GenerateGridControlID(installed_before_change, row_id);
            var txt2 = "#" + GenerateGridControlID(change_quantity_additional, row_id);
            var txt3 = "#" + GenerateGridControlID(change_quantity_removal, row_id);
            
            var obj = {
                InstrumentCode: gridInst02.cells2(i, codeCol).getValue(),
                InstrumentQty: $(txt1).NumericValue(),
                AddQty: $(txt2).NumericValue(),
                RemoveQty: $(txt3).NumericValue(),
                IsDefaultFlag: (gridInst02.cells2(i, defCol).getValue() == 1 || gridInst02.cells2(i, defCol).getValue() == "true"),
                SaleFlag: (gridInst02.cells2(i, sfCol).getValue() == 1 || gridInst02.cells2(i, sfCol).getValue() == "true"),
                RentalFlag: (gridInst02.cells2(i, rfCol).getValue() == 1 || gridInst02.cells2(i, rfCol).getValue() == "true"),
                LineUpTypeCode: gridInst02.cells2(i, ltCol).getValue()
            };
            vv.push(obj);
        }
    }

    if (vv.length > 0)
        return vv;
    else
        return null;
}
function SetQUS030_08_SectionMode(isview) {
    if (isview) {
        $("#gridInstrumentDetail02").SetViewMode(true);
        $("#divInstrumentAdd").hide();

        if (gridInst02 != undefined) {
            $("#gridInstrumentDetail02").SetNumericControlViewInGrid([installed_before_change,
                                                                        change_quantity_additional,
                                                                        change_quantity_removal,
                                                                        installed_quantity]);

            var removeCol = gridInst02.getColIndexById("Remove");

            gridInst02.setColumnHidden(removeCol, true);
            gridInst02.setSizes();
        }
    }
    else {
        $("#gridInstrumentDetail02").SetViewMode(false);
        $("#divInstrumentAdd").show();

        if (gridInst02 != undefined) {
            var removeCol = gridInst02.getColIndexById("Remove");
            gridInst02.setColumnHidden(removeCol, false);
            for (var i = 0; i < gridInst02.getRowsNum(); i++) {
                gridInst02.setColspan(gridInst02.getRowId(i), removeCol, 2);
            }
        }
    }
}
function GetQUS030_08_SectionValidation(controls) {
    if (controls != undefined) {
        for (var idx = 0; idx < controls.length; idx++) {
            if (controls[idx] == null || controls[idx] == undefined)
                continue;

            if (controls[idx].indexOf("InstrumentDetail;") >= 0) {
                var errCols = controls[idx].replace("InstrumentDetail;", "").split(";");

                var colQtyIdx;
                for (var i = 0; i < errCols.length; i++) {
                    if (errCols[i] == change_quantity_additional
                        && (i + 1) < errCols.length) {
                        i += 1;
                        colQtyIdx = errCols[i];
                    }
                }
                
                var ctrlLst = new Array();
                var errLst = new Array();
                for (var i = 0; i < gridInst02.getRowsNum(); i++) {
                    var row_id = gridInst02.getRowId(i);
                    var txt = GenerateGridControlID(change_quantity_additional, row_id);

                    ctrlLst.push(txt);
                    if (colQtyIdx == "ALL" || colQtyIdx == i) {
                        errLst.push(txt);
                    }
                }
                VaridateCtrl(ctrlLst, errLst);
                break;
            }
        }
    }
}
/* ----------------------------------------------------------------------------------- */

