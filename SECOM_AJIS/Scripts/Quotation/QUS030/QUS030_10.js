/* --- Initial Variable -------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
var doNewFacility = null;
var gridFacility = null;
var facility_quantity = "FacQuantity";
var btn_remove_f = "btnRemoveFacility";
/* ----------------------------------------------------------------------------------- */

function QUS030_10_InitialGrid() {
    /* --- Bind events --- */
    /* ------------------- */
    $("#FacilityCode").blur(GetFacilityInfoData);
    $("#FacilityQty").BindNumericBox(5, 0, 0, 99999, true);
    /* ------------------- */

    /* --- Initial Grid --- */
    /* -------------------- */
    gridFacility = $("#gridFacilityDetail").LoadDataToGridWithInitial(0, false, false,
                                "/Quotation/QUS030_GetFacilityDetailData",
                                "",
                                "doInstrumentDetail", false);
    /* -------------------- */

    SpecialGridControl(gridFacility, ["InstrumentQty", "Remove"]);

    BindOnLoadedEvent(gridFacility, function (gen_ctrl) {
        for (var i = 0; i < gridFacility.getRowsNum(); i++) {
            var row_id = gridFacility.getRowId(i);
            var qtyCol = gridFacility.getColIndexById("InstrumentQty");
            var defCol = gridFacility.getColIndexById("IsDefault");

            if (gen_ctrl == true) {
                /* --- Enable/Diable Button --- */
                /* ---------------------------- */
                var enable_txt = true;
                var enable = true;
                if (GetDisableFlag("FacilityQuantityTextBox") == true) {
                    enable_txt = false;
                }

                if (GetDisableFlag("RemoveFacilityButton") == true) {
                    enable = false;
                }
                else if (gridFacility.cells2(i, defCol).getValue() == 1) {
                    enable = false;
                }
                /* ---------------------------- */

                var val = GetValueFromLinkType(gridFacility, i, qtyCol);
            
                GenerateNumericBox2(gridFacility, facility_quantity, row_id, "InstrumentQty", val, 5, 0, 0, 99999, true, enable_txt);
                GenerateRemoveButton(gridFacility, btn_remove_f, row_id, "Remove", enable);
            }

            /* --- Set Event --- */
            /* ----------------- */
            BindGridButtonClickEvent(btn_remove_f, row_id, function (rid) {
                DeleteRow(gridFacility, rid);
            });
            /* ----------------- */
        }
        gridFacility.setSizes();
    });

    /* --- Add Events --- */
    $("#btnAddFacility").click(function () {
        /* --- Set Parameter --- */
        /* --------------------- */
        var vv = new Array();
        if (CheckFirstRowIsEmpty(gridFacility) == false) {
            var codeCol = gridFacility.getColIndexById("InstrumentCode");
            var defCol = gridFacility.getColIndexById("IsDefault");

            for (var i = 0; i < gridFacility.getRowsNum(); i++) {
                var iobj = {
                    InstrumentCode: gridFacility.cells2(i, codeCol).getValue(),
                    IsDefaultFlag: (gridFacility.cells2(i, defCol).getValue() == 1)
                };
                vv.push(iobj);
            }
        }
        var obj = {
            FacilityCode: $("#FacilityCode").val(),
            FacilityQty: $("#FacilityQty").NumericValue(),
            FacilityList: vv,
            doFacilityDetail: doNewFacility
        };
        /* --------------------- */

        call_ajax_method_json("/Quotation/QUS030_CheckBeforeAddFacility", obj, function (result, controls) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["FacilityCode", "FacilityQty"], controls);
            /* --------------------- */

            if (controls != undefined) {
                return;
            }
            else if (result == true) {
                /* --- Check Empty Row --- */
                /* ----------------------- */
                CheckFirstRowIsEmpty(gridFacility, true);
                /* ----------------------- */

                /* --- Add Row --- */
                /* --------------- */
                AddNewRow(gridFacility,
                        [ConvertBlockHtml(doNewFacility.InstrumentCode), //doNewFacility.InstrumentCode, //Modify by Jutarat A. on 28112013
                        ConvertBlockHtml(doNewFacility.InstrumentName),
                        doNewFacility.LineUpTypeCodeName,
                        "",
                        "",
                        ""]);

                var row_idx = gridFacility.getRowsNum() - 1;
                var row_id = gridFacility.getRowId(row_idx);
                
                GenerateNumericBox2(gridFacility, facility_quantity, row_id, "InstrumentQty", $("#FacilityQty").val(), 5, 0, 0, 99999, true, true);
                GenerateRemoveButton(gridFacility, btn_remove_f, row_id, "Remove", true);
                gridFacility.setSizes();
                /* --------------- */

                /* --- Set Event --- */
                /* ----------------- */
                BindGridButtonClickEvent(btn_remove_f, row_id, function (rid) {
                    DeleteRow(gridFacility, rid);
                });
                /* ----------------- */

                ClearFacilityData();
            }
        });
    });
    /* --- Cancel Events --- */
    $("#btnCancelFacility").click(function () {
        ClearFacilityData();
    });
}

/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "FacilityCode",
        "FacilityMemo"
    ]);

    /* --- Bind events --- */
    /* ------------------- */
    $("#btnShowDefaultFacility").click(show_default_facility_click);
    $("#btnSearchFacility").click(search_facility_click);
    $("#FacilityMemo").SetMaxLengthTextArea(500);
    /* ------------------- */

    ClearFacilityData();

    if (GetDisableFlag("AllFacilityControls") == true) {
        $("#btnShowDefaultFacility").attr("disabled", true);
        $("#FacilityCode").attr("readonly", true);
        $("#btnSearchFacility").attr("disabled", true);
        $("#FacilityQty").attr("readonly", true);
        $("#btnAddFacility").attr("disabled", true);
        $("#btnCancelFacility").attr("disabled", true);
        $("#FacilityMemo").attr("readonly", true);
    }
});

/* ----------------------------------------------------------------------------------- */

/* --- Events ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function show_default_facility_click() {
    if (gridFacility == undefined)
        return null;

    var isAsk = (CheckFirstRowIsEmpty(gridFacility) == false);
    var obj = {
        ProductCode: $("#ProductCode").val(),
        IsAskQuestion: isAsk
    };
    if (isAsk) {
        call_ajax_method_json("/Quotation/QUS030_GetDefaultFacilityDetailData_P1", obj, function (result, controls) {
            if (controls != undefined) {
                /* --- Higlight Text --- */
                /* --------------------- */
                VaridateCtrl(["ProductCode"], controls);
                /* --------------------- */

                return;
            }
            else if (result != undefined) {
                OpenYesNoMessageDialog(result.Code, result.Message, function () {
                    $("#gridDefaultFacility").LoadDataToGrid(gridFacility, 0, false,
                                    "/Quotation/QUS030_GetDefaultFacilityDetailData_P2", "", "doDefaultFacility", false);
                }, null);
            }
        });
    }
    else {
        $("#gridFacilityDetail").LoadDataToGrid(gridFacility, 0, false,
                                    "/Quotation/QUS030_GetDefaultFacilityDetailData_P1", obj, "doDefaultFacility", false, 
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


function search_facility_click() {
    var obj = {
            bExpTypeHas: false,
            bExpTypeNo: false,
            bProdTypeSale: false,
            bProdTypeAlarm: false,
            bInstTypeGen: false,
            bInstTypeMonitoring: true,
            bInstTypeMat: false
        };
    var func = function(result)
    {
        $("#FacilityCode").ResetToNormalControl();
        $("#FacilityQty").ResetToNormalControl();

        doNewFacility = result;
        $("#FacilityCode").val(result.InstrumentCode);
        $("#FacilityName").val(result.InstrumentName);
    };

    SearchInstrument(obj, func);
}
/* ----------------------------------------------------------------------------------- */

/* --- Methods ----------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function ClearFacilityData() {
    $("#FacilityCode").val("");
    $("#FacilityName").val("");
    $("#FacilityQty").val("0");

    $("#FacilityCode").ResetToNormalControl();
    $("#FacilityQty").ResetToNormalControl();
    doNewFacility = null;
}
function GetFacilityInfoData() {
    $("#FacilityName").val("");
    if ($.trim($(this).val()) != "") {
        /* --- Set Parameter --- */
        var obj = {
            InstrumentCode: $(this).val(),
        };

        call_ajax_method_json("/Quotation/QUS030_GetFacilityDetailInfo", obj, function (result) {
            if (result != undefined) {
                doNewFacility = result;
                $("#FacilityName").val(doNewFacility.InstrumentName);
            }
            else {
                doNewFacility = null;
            }
        });
    }
}
/* ----------------------------------------------------------------------------------- */

/* --- Output Methods ---------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function GetQUS030_10_SectionData() {
    /// <summary>Method return object data for Facility detail section</summary>
    if (gridFacility == undefined)
        return null;

    var vv = new Array();
    if (CheckFirstRowIsEmpty(gridFacility) == false) {
        var codeCol = gridFacility.getColIndexById("InstrumentCode");
        var defCol = gridFacility.getColIndexById("IsDefault");

        for (var i = 0; i < gridFacility.getRowsNum(); i++) {
            var row_id = gridFacility.getRowId(i);
            var txt = "#" + GenerateGridControlID(facility_quantity, row_id);

            var obj = {
                InstrumentCode: gridFacility.cells2(i, codeCol).getValue(),
                InstrumentQty: $(txt).NumericValue(),
                IsDefaultFlag: (gridFacility.cells2(i, defCol).getValue() == 1 || gridFacility.cells2(i, defCol).getValue() == "true")
            };
            vv.push(obj);
        }
    }

    if (vv.length <= 0)
        vv = null;
    
    return {
        FacilityLst : vv,
        FacilityMemo : $("#FacilityMemo").val()
        };
}
function SetQUS030_10_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode for Facility detail section</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>

    if (isview) {
        $("#divFacilityDetail").SetViewMode(true);
        $("#divFacilityAdd").hide();

        if (gridFacility != undefined) {
            $("#divFacilityDetail").SetNumericControlViewInGrid([facility_quantity]);

            var removeCol = gridFacility.getColIndexById("Remove");

            gridFacility.setColumnHidden(removeCol, true);
            gridFacility.setSizes();
        }
    }
    else {
        $("#divFacilityDetail").SetViewMode(false);
        $("#divFacilityAdd").show();

        if (gridFacility != undefined) {
            var removeCol = gridFacility.getColIndexById("Remove");
            gridFacility.setColumnHidden(removeCol, false);

            if (CheckFirstRowIsEmpty(gridFacility) == false) {
                for (var i = 0; i < gridFacility.getRowsNum(); i++) {
                    gridFacility.setColspan(gridFacility.getRowId(i), removeCol, 2);
                }
            }
        }
    }
}
function GetQUS030_10_SectionValidation(controls) {
    /// <summary>Method to validate data for Facility detail section</summary>
    /// <param name="controls" type="array">List of controls that error</param>

    if (controls != undefined) {
        for (var idx = 0; idx < controls.length; idx++) {
            if (controls[idx] == null || controls[idx] == undefined)
                continue;

            if (controls[idx].indexOf("FacilityDetail;") >= 0) {
                var errCols = controls[idx].replace("FacilityDetail;", "").split(";");

                var colQtyIdx;
                for (var i = 0; i < errCols.length; i++) {
                    if (errCols[i] == facility_quantity
                        && (i + 1) < errCols.length) {
                        i += 1;
                        colQtyIdx = errCols[i];
                    }
                }

                var ctrlLst = new Array();
                var errLst = new Array();
                for (var i = 0; i < gridFacility.getRowsNum(); i++) {
                    var row_id = gridFacility.getRowId(i);
                    var txt = GenerateGridControlID(facility_quantity, row_id);

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

function QUS030_10_ResetToNormalControl() {
    $("#divFacilityDetail").ResetToNormalControl();
}