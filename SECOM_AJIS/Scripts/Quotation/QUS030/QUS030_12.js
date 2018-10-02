/// <reference path="../../Base/Master.js" />

/* --- Variable ---------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
var gridSentryGuard = null;
var btn_edit_sentry_guard = "btnEditSentryGuard";
var btn_remove_sentry_guard = "btnRemoveSentryGuard";
var sentry_guard_mode = 0; //0 = Add, 1 = Edit
var isSentryGuardSelected = false;
/* ----------------------------------------------------------------------------------- */

var EditSentryGuardEvent = function (row_id) {
    gridSentryGuard.selectRow(gridSentryGuard.getRowIndex(row_id));
    $("#formRegisterSentryGuardInformation").ResetToNormalControl();

    /* --- Disable All Button --- */
    /* -------------------------- */
    $("#gridSentryGuardDetail").find("img").each(function () {
        if (this.id != "" && this.id != undefined) {
            if (this.id.indexOf(btn_edit_sentry_guard) == 0) {
                var rid = GetGridRowIDFromControl(this);
                EnableGridButton(gridSentryGuard, btn_edit_sentry_guard, rid, "Edit", false);
            }
            else if (this.id.indexOf(btn_remove_sentry_guard) == 0) {
                var rid = GetGridRowIDFromControl(this);
                EnableGridButton(gridSentryGuard, btn_remove_sentry_guard, rid, "Remove", false);
            }
        }
    });
    /* -------------------------- */

    var noCol = gridSentryGuard.getColIndexById("No");
    var obj = {
        RunningNo: gridSentryGuard.cells(row_id, noCol).getValue()
    };
    call_ajax_method_json("/Quotation/QUS030_GetSentryGuard", obj, function (result, controls) {
        $("#LineNo").val(obj.RunningNo);
        $("#SentryGuardTypeCode").val(result.SentryGuardTypeCode);
        $("#NumOfDate").val(SetNumericValue(result.NumOfDate, 1));
        $("#SecurityStartTime").val(ConvetTimeSpan(result.SecurityStartTime));
        $("#SecurityFinishTime").val(ConvetTimeSpan(result.SecurityFinishTime));
        $("#WorkHourPerMonth").val(SetNumericValue(result.WorkHourPerMonth, 1));
        $("#CostPerHour").val(SetNumericValue(result.CostPerHour, 2));
        $("#CostPerHourCurrencyType").val(result.CostPerHourCurrencyType);
        $("#NumOfSentryGuard").val(result.NumOfSentryGuard);

        $("#LineNo").setComma();
        $("#WorkHourPerMonth").setComma();
        $("#CostPerHour").setComma();
        $("#NumOfSentryGuard").setComma();

        sentry_guard_mode = 1;
    });

    isSentryGuardSelected = true;
}
var DeleteSentryGuardEvent = function (row_id) {
    var noCol = gridSentryGuard.getColIndexById("No");

    var obj = {
        RunningNo: gridSentryGuard.cells(row_id, noCol).getValue()
    };
    call_ajax_method_json("/Quotation/QUS030_RemoveSentryGuard", obj, function (result, controls) {
        DeleteRow(gridSentryGuard, row_id);

        var noCol = gridSentryGuard.getColIndexById("No");
        if (CheckFirstRowIsEmpty(gridSentryGuard) == false) {
            for (var i = 0; i < gridSentryGuard.getRowsNum(); i++) {
                gridSentryGuard.cells2(i, noCol).setValue(i + 1);
            }
        }
    });
}

function QUS030_12_InitialGrid() {
    /* --- Initial Grid --- */
    /* -------------------- */
    gridSentryGuard = $("#gridSentryGuardDetail").LoadDataToGridWithInitial(0, false, false,
                                "/Quotation/QUS030_GetSentryGuardDetailData",
                                "",
                                "doSentryGuardDetail", false);
    /* -------------------- */

    SpecialGridControl(gridSentryGuard, ["Edit", "Remove"]);

    gridSentryGuard.attachEvent("onBeforeSelect", function (new_row, old_row) {
        if (isSentryGuardSelected == true)
            return false;
        else
            return true;
    });



    var ChangeToViewMode = function () {
        $("#formRegisterSentryGuardInformation").clearForm();
        $("#formRegisterSentryGuardInformation").ResetToNormalControl();

        /* --- Enable Button --- */
        /* --------------------- */
        $("#gridSentryGuardDetail").find("img").each(function () {
            if (this.id != "" && this.id != undefined) {
                if (this.id.indexOf(btn_edit_sentry_guard) == 0) {
                    var row_id = GetGridRowIDFromControl(this);
                    EnableGridButton(gridSentryGuard, btn_edit_sentry_guard, row_id, "Edit", true);
                }
                else if (this.id.indexOf(btn_remove_sentry_guard) == 0) {
                    var row_id = GetGridRowIDFromControl(this);
                    EnableGridButton(gridSentryGuard, btn_remove_sentry_guard, row_id, "Remove", true);
                }
            }
        });
        /* --------------------- */

        sentry_guard_mode = 0;
    }

    BindOnLoadedEvent(gridSentryGuard, function (gen_ctrl) {
        for (var i = 0; i < gridSentryGuard.getRowsNum(); i++) {
            var row_id = gridSentryGuard.getRowId(i);
            var noCol = gridSentryGuard.getColIndexById("No");

            if (gen_ctrl == true) {
                /* --- Enable/Diable Button --- */
                /* ---------------------------- */
                var enable_edit = true;
                var enable_remove = true;
                if (GetDisableFlag("EditSentryGuardButton") == true) {
                    enable_edit = false;
                }
                if (GetDisableFlag("RemoveSentryButton") == true) {
                    enable_remove = false;
                }
                /* ---------------------------- */

                gridSentryGuard.cells2(i, noCol).setValue(i + 1);
                GenerateEditButton(gridSentryGuard, btn_edit_sentry_guard, row_id, "Edit", enable_edit);
                GenerateRemoveButton(gridSentryGuard, btn_remove_sentry_guard, row_id, "Remove", enable_remove);

            }

            /* --- Set Event --- */
            /* ----------------- */
            BindGridButtonClickEvent(btn_edit_sentry_guard, row_id, EditSentryGuardEvent);
            BindGridButtonClickEvent(btn_remove_sentry_guard, row_id, DeleteSentryGuardEvent);
            /* ----------------- */
        }
        gridSentryGuard.setSizes();
    });

    /* --- Add Events --- */
    $("#btnAddUpdateSentryGuard").click(function () {
        /* --- Create Object Parameter --- */
        /* ------------------------------- */
        var obj = CreateObjectData($("#formRegisterSentryGuardInformation").serialize());
        if (obj != undefined) {
            var name = $("#SentryGuardTypeCode").find("option:selected").text();
            var idx = name.indexOf(":");

            obj.RunningNo = obj.LineNo;
            obj.SentryGuardTypeName = $.trim(name.substring(idx + 1, name.length));
            obj.UpdateMode = sentry_guard_mode;
            obj.CostPerHourCurrencyType = $("#CostPerHour").NumericCurrencyValue();
        }
        /* ------------------------------- */

        /* --- Call method --- */
        /* ------------------- */
        call_ajax_method_json("/Quotation/QUS030_CheckBeforeAddSentryGuard", obj, function (result, controls) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl([
                "SentryGuardTypeCode",
                "NumOfDate",
                "SecurityStartTime",
                "SecurityFinishTime",
                "CostPerHour",
                "NumOfSentryGuard"], controls);
            /* --------------------- */

            if (controls != undefined) {
                return;
            }
            else if (result != null) {
                if (sentry_guard_mode == 0) {
                    /* --- Check Empty Row --- */
                    /* ----------------------- */
                    CheckFirstRowIsEmpty(gridSentryGuard, true);
                    /* ----------------------- */
                    var row_num = gridSentryGuard.getRowsNum();

                    /* --- Add Row --- */
                    /* --------------- */
                    AddNewRow(gridSentryGuard,
                    [row_num + 1,
                        obj.SentryGuardTypeName,
                        obj.NumOfDate,
                        obj.SecurityStartTime,
                        obj.SecurityFinishTime,
                        obj.WorkHourPerMonth,
                        result.TextTransferCostPerHour,
                        obj.NumOfSentryGuard,
                        "",
                        "",
                        "",
                        ""]);
                    /* --------------- */

                    var row_idx = gridSentryGuard.getRowsNum() - 1;
                    var row_id = gridSentryGuard.getRowId(row_idx);

                    GenerateEditButton(gridSentryGuard, btn_edit_sentry_guard, row_id, "Edit", true);
                    GenerateRemoveButton(gridSentryGuard, btn_remove_sentry_guard, row_id, "Remove", true);
                    gridSentryGuard.setSizes();

                    /* --- Set Event --- */
                    /* ----------------- */
                    BindGridButtonClickEvent(btn_edit_sentry_guard, row_id, EditSentryGuardEvent);
                    BindGridButtonClickEvent(btn_remove_sentry_guard, row_id, DeleteSentryGuardEvent);
                    /* ----------------- */
                }
                else {
                    var row_num = $("#LineNo").val() - 1;

                    var sNameCol = gridSentryGuard.getColIndexById("SentryGuardTypeName");
                    var numDateCol = gridSentryGuard.getColIndexById("NumOfDate");
                    var sTimeCol = gridSentryGuard.getColIndexById("SecurityStartTime");
                    var fTimeCol = gridSentryGuard.getColIndexById("SecurityFinishTime");
                    var wHMCol = gridSentryGuard.getColIndexById("WorkHourPerMonth");
                    var cHCol = gridSentryGuard.getColIndexById("CostPerHour");
                    var numSentryCol = gridSentryGuard.getColIndexById("NumOfSentryGuard");
                    var jCol = gridSentryGuard.getColIndexById("ToJson");

                    gridSentryGuard.cells2(row_num, sNameCol).setValue(obj.SentryGuardTypeName);
                    gridSentryGuard.cells2(row_num, numDateCol).setValue(obj.NumOfDate);
                    gridSentryGuard.cells2(row_num, sTimeCol).setValue(obj.SecurityStartTime);
                    gridSentryGuard.cells2(row_num, fTimeCol).setValue(obj.SecurityFinishTime);
                    gridSentryGuard.cells2(row_num, wHMCol).setValue(obj.WorkHourPerMonth);
                    gridSentryGuard.cells2(row_num, cHCol).setValue(result.TextTransferCostPerHour);
                    gridSentryGuard.cells2(row_num, numSentryCol).setValue(obj.NumOfSentryGuard);
                }

                ChangeToViewMode();

                /* --- Enable Button --- */
                /* --------------------- */
                $("#gridSentryGuardDetail").find("img").each(function () {
                    if (this.id != "" && this.id != undefined) {
                        if (this.id.indexOf(btn_edit_sentry_guard) == 0) {
                            var row_id = GetGridRowIDFromControl(this);
                            EnableGridButton(gridSentryGuard, btn_edit_sentry_guard, row_id, "Edit", true);
                        }
                        else if (this.id.indexOf(btn_remove_sentry_guard) == 0) {
                            var row_id = GetGridRowIDFromControl(this);
                            EnableGridButton(gridSentryGuard, btn_remove_sentry_guard, row_id, "Remove", true);
                        }
                    }
                });
                QUS030_12_DefaultData();
                isSentryGuardSelected = false;
            }
        });
        /* ------------------- */

        return false;
    });
    /* --- Cancel Events --- */
    $("#btnCancelSentryGuard").click(function () {
        ChangeToViewMode();

        /* --- Enable Button --- */
        /* --------------------- */
        $("#gridSentryGuardDetail").find("img").each(function () {
            if (this.id != "" && this.id != undefined) {
                if (this.id.indexOf(btn_edit_sentry_guard) == 0) {
                    var row_id = GetGridRowIDFromControl(this);
                    EnableGridButton(gridSentryGuard, btn_edit_sentry_guard, row_id, "Edit", true);
                }
                else if (this.id.indexOf(btn_remove_sentry_guard) == 0) {
                    var row_id = GetGridRowIDFromControl(this);
                    EnableGridButton(gridSentryGuard, btn_remove_sentry_guard, row_id, "Remove", true);
                }
            }
        });
        QUS030_12_DefaultData();
        isSentryGuardSelected = false;
        return false;
    });
}


/* --- Initial ----------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    /* --- Initial Controls --- */
    /* ------------------------ */
    $("#SecurityItemFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#OtherItemFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#CostPerHour").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#NumOfSentryGuard").BindNumericBox(2, 0, 1, 99);

    $("#SecurityStartTime").BindTimeBox();
    $("#SecurityFinishTime").BindTimeBox();
    /* ------------------------ */

    /* --- Initial Events --- */
    /* ---------------------- */
    $("#NumOfDate").RelateControlEvent(number_of_date_change);
    $("#SecurityStartTime").blur(security_start_time_blur);
    $("#SecurityFinishTime").blur(security_finish_time_blur);
    /* ---------------------- */

    $("#SecurityItemFee").DisableControl();
    $("#OtherItemFee").DisableControl();
    $("#SentryGuardAreaTypeCode").DisableControl();

    if (GetDisableFlag("SentryGuardDetail") == true) {
        $("#SentryGuardTypeCode").attr("disabled", true);
        $("#NumOfDate").attr("disabled", true);
        $("#SecurityStartTime").attr("readonly", true);
        $("#SecurityFinishTime").attr("readonly", true);
        $("#CostPerHour").attr("readonly", true);
        $("#NumOfSentryGuard").attr("readonly", true);
        $("#btnAddUpdateSentryGuard").attr("disabled", true);
        $("#btnCancelSentryGuard").attr("disabled", true);
    }
});
/* ----------------------------------------------------------------------------------- */

/* --- Events ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function number_of_date_change() {
    CalSentryGuardWorkingHour();
}
function security_start_time_blur() {
    CalSentryGuardWorkingHour();
}
function security_finish_time_blur() {
    CalSentryGuardWorkingHour();
}
/* ----------------------------------------------------------------------------------- */

/* --- Methods ----------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function CalSentryGuardWorkingHour() {
    if ($("#SecurityStartTime").val() == ""
        || $("#SecurityFinishTime").val() == ""
        || $("#NumOfDate").val() == "") {
        $("#WorkHourPerMonth").val("");
        return;
    }

    var dt1_hr = 0;
    var dr1_min = 0;
    var dt1 = new Date();
    var dt2_hr = 0;
    var dr2_min = 0;
    var dt2 = new Date();

    var spStart = $("#SecurityStartTime").val().split(":");
    if (spStart.length == 1) {
        var txt = $("#SecurityStartTime").val();
        spStart = [txt.substring(0, 2), txt.substring(2, 4)];
    }
    if (spStart.length == 2) {
        dt1_hr = parseInt(spStart[0], 10);
        dr1_min = parseInt(spStart[1], 10);

        var spFinish = $("#SecurityFinishTime").val().split(":");
        if (spFinish.length == 2) {
            dt2_hr = parseInt(spFinish[0], 10);
            dr2_min = parseInt(spFinish[1], 10);
        }
        else if (spFinish.length == 1) {
            var v = $("#SecurityFinishTime").val();
            dt2_hr = parseInt(v.substring(0, 2), 10);
            dr2_min = parseInt(v.substring(2, 4), 10);
        }

        if (dt1_hr > dt2_hr) {
            dt2.setDate(dt2.getDate() + 1);
        }
        else if (dt1_hr == dt2_hr && dr1_min > dr2_min) {
            dt2.setDate(dt2.getDate() + 1);
        }
    }

    dt1.setHours(dt1_hr, dr1_min, 0, 0);
    dt2.setHours(dt2_hr, dr2_min, 0, 0);

    /* --- Calculate Diff --- */
    /* ---------------------- */
    var diff = 0;
    if (dt1 <= dt2) {
        var ddiff = (((dt2 - dt1) / 1000) / 60) / 60;
        diff = ddiff;
    }
    /* ---------------------- */

    var nDate = $("#NumOfDate").val();
    if (nDate != "") {
        var f = parseFloat(nDate);
        diff = diff * f;
        diff = Math.round(diff * 100) / 100;
    }
    else
        diff = 0;

    $("#WorkHourPerMonth").val(diff.toFixed(1));
    $("#WorkHourPerMonth").setComma();
}
/* ----------------------------------------------------------------------------------- */

/* --- Output Methods ---------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function GetQUS030_12_SectionData() {
    var qb = CreateObjectData($("#formSentryGuardInformation").serialize());
    if (qb != undefined) {
        qb.SentryGuardAreaTypeCode = $("#SentryGuardAreaTypeCode").val();
    }

    var vv = null;

    if (gridSentryGuard != undefined) {
        var vv = new Array();
        //        if (CheckFirstRowIsEmpty(gridSentryGuard) == false) {
        //            var codeCol = gridSentryGuard.getColIndexById("ToJson");

        //            for (var i = 0; i < gridSentryGuard.getRowsNum(); i++) {
        //                var txt = gridSentryGuard.cells2(i, codeCol).getValue();
        //                var obj = $.parseJSON(txt);
        //                if (obj != undefined) {
        //                    obj.SecurityStartTime = ConvetTimeSpan(obj.SecurityStartTime);
        //                    obj.SecurityFinishTime = ConvetTimeSpan(obj.SecurityFinishTime);

        //                    vv.push(obj);
        //                }
        //            }
        //        }

        //        if (vv.length == 0)
        //            vv = null;

    }

    return {
        QuotationBasic: qb,
        SentryGuardList: vv,
        IsEditMode: isSentryGuardSelected
    };
}
function SetQUS030_12_SectionMode(isview) {
    if (isview) {
        $("#divSentryGuardDetail").SetViewMode(true);
        $("#divRegisterSentryGuardInformation").hide();

        if (gridSentryGuard != undefined) {
            var editCol = gridSentryGuard.getColIndexById("Edit");
            var removeCol = gridSentryGuard.getColIndexById("Remove");
            var fCol = gridSentryGuard.getColIndexById("FixScroll1");

            gridSentryGuard.setColumnHidden(editCol, true);
            gridSentryGuard.setColumnHidden(fCol, true);
            gridSentryGuard.setColumnHidden(removeCol, true);
        }
    }
    else {
        $("#divSentryGuardDetail").SetViewMode(false);
        $("#divRegisterSentryGuardInformation").show();

        if (gridSentryGuard != undefined) {
            var editCol = gridSentryGuard.getColIndexById("Edit");
            var removeCol = gridSentryGuard.getColIndexById("Remove");
            var fCol = gridSentryGuard.getColIndexById("FixScroll1");

            gridSentryGuard.setColumnHidden(editCol, false);
            gridSentryGuard.setColumnHidden(fCol, false);
            gridSentryGuard.setColumnHidden(removeCol, false);

            for (var i = 0; i < gridSentryGuard.getRowsNum(); i++) {
                gridSentryGuard.setColspan(gridSentryGuard.getRowId(i), removeCol, 2);
                gridSentryGuard.setColspan(gridSentryGuard.getRowId(i), editCol, 2);
            }
        }
    }
}
/* ----------------------------------------------------------------------------------- */

function QUS030_12_ResetToNormalControl() {
    $("#divSentryGuardDetail").ResetToNormalControl();
}

function QUS030_12_DefaultData() {
    $("#SentryGuardTypeCode").val("1");
    $("#NumOfDate").val("30.4");
    $("#SecurityStartTime").val("00:00");
    $("#SecurityFinishTime").val("00:00");
    $("#WorkHourPerMonth").val("0.0");
}