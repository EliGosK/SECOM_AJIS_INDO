/// <reference path="../../Base/Master.js" />
/// <reference path="../../Base/GridControl.js" />

/* --- Variable --- */
var gridMATarget = null;
var btn_detail_ma_target = "btnDetailMATarget";
var btn_remove_ma_target = "btnRemoveMATarget";

function QUS030_06_InitialGrid() {
    /* --- Initial Grid --- */
    /* -------------------- */
    gridMATarget = $("#gridMaintenanceTargetContract").LoadDataToGridWithInitial(0, false, false,
                                "/Quotation/QUS030_GetMaintenanceDetailData",
                                "",
                                "doContractHeader", false);
    /* -------------------- */

    SpecialGridControl(gridMATarget, ["Detail", "Remove"]);

    var DetailMAEvent = function (row_id) {
        var codeCol = gridMATarget.getColIndexById("ContractCodeShort");
        var ptCol = gridMATarget.getColIndexById("ProductTypeCode");

        var obj = {
            strContractCode: gridMATarget.cells(row_id, codeCol).getValue()
        };
        if (gridMATarget.cells(row_id, ptCol).getValue() == $("#C_PROD_TYPE_SALE").val()) {
            ajax_method.CallScreenControllerWithAuthority("/Common/CMS160", obj, true);
        }
        else {
            ajax_method.CallScreenControllerWithAuthority("/Common/CMS140", obj, true);
        }
    }

    /* --- Bind Grid events --- */
    /* ------------------------ */
    BindOnLoadedEvent(gridMATarget, function (gen_ctrl) {
        for (var i = 0; i < gridMATarget.getRowsNum(); i++) {
            var row_id = gridMATarget.getRowId(i);

            if (gen_ctrl == true) {
                var enabledD = true;
                var enabledR = true;
                if (GetDisableFlag("MaintenanceTargetContractDetail") == true) {
                    enabledD = false;
                }
                else {
                    var scCol = gridMATarget.getColIndexById("SiteCode");
                    var scValue = gridMATarget.cells2(i, scCol).getValue();

                    enabledD = false;
                    if (scValue != undefined && scValue != "")
                        enabledD = true;
                }

                if (GetDisableFlag("MaintenanceTargetContractRemove") == true) {
                    enabledR = false;
                }

                GenerateDetailButton(gridMATarget, btn_detail_ma_target, row_id, "Detail", enabledD);
                GenerateRemoveButton(gridMATarget, btn_remove_ma_target, row_id, "Remove", enabledR);
            }

            /* --- Set Event --- */
            /* ----------------- */
            BindGridButtonClickEvent(btn_detail_ma_target, row_id, DetailMAEvent);
            BindGridButtonClickEvent(btn_remove_ma_target, row_id, function (rid) {
                DeleteRow(gridMATarget, rid);
            });
            /* ----------------- */
        }
        gridMATarget.setSizes();
    });
    /* ------------------------ */

    /* --- Add Events --- */
    /* ------------------ */
    $("#btnAddMaintenanceTarget").click(function () {
        /* --- Set Parameter --- */
        /* --------------------- */
        var vv = new Array();
        if (CheckFirstRowIsEmpty(gridMATarget) == false) {
            for (var i = 0; i < gridMATarget.getRowsNum(); i++) {
                var codeCol = gridMATarget.getColIndexById("ContractCodeShort");
                var ptCol = gridMATarget.getColIndexById("ProductTypeCode");

                var iobj = {
                    ContractCode: gridMATarget.cells2(i, codeCol).getValue(),
                    ProductTypeCode: gridMATarget.cells2(i, ptCol).getValue()
                };

                vv.push(iobj);
            }
        }

        var obj = {
            MaintenanceTargetContractCode: $("#MaintenanceTargetContractCode").val(),
            MaintenanceList: vv
        };
        /* --------------------- */

        /* --- Check and Add event --- */
        /* --------------------------- */
        call_ajax_method_json("/Quotation/QUS030_CheckBeforeAddMaintenanceDetail", obj, function (result, controls) {
            if (controls != undefined) {
                /* --- Higlight Text --- */
                /* --------------------- */
                VaridateCtrl(["MaintenanceTargetContractCode"], controls);
                /* --------------------- */

                return;
            }
            else if (result != undefined) {
                /* --- Check Empty Row --- */
                /* ----------------------- */
                CheckFirstRowIsEmpty(gridMATarget, true);
                /* ----------------------- */

                /* --- Add new row --- */
                /* ------------------- */
                AddNewRow(gridMATarget,
                    [result.ContractCodeShort,
                        result.ProductName,
                        "",
                        "",
                        "",
                        result.ProductTypeCode,
                        result.SiteCode]);
                /* ------------------- */

                var row_idx = gridMATarget.getRowsNum() - 1;
                var row_id = gridMATarget.getRowId(row_idx);

                var enableDetail = false;
                if (result.SiteCode != undefined && result.SiteCode != "")
                    enableDetail = true;

                GenerateDetailButton(gridMATarget, btn_detail_ma_target, row_id, "Detail", enableDetail);
                GenerateRemoveButton(gridMATarget, btn_remove_ma_target, row_id, "Remove", true);
                gridMATarget.setSizes();

                /* --- Set Event --- */
                /* ----------------- */
                BindGridButtonClickEvent(btn_detail_ma_target, row_id, DetailMAEvent);
                BindGridButtonClickEvent(btn_remove_ma_target, row_id, function (rid) {
                    DeleteRow(gridMATarget, rid);
                });
                /* ----------------- */

                /* --- Clear data --- */
                /* ------------------ */
                $("#MaintenanceTargetContractCode").val("");
                $("#MaintenanceTargetContractCode").ResetToNormalControl();
                /* ------------------ */
            }
        });
        /* --------------------------- */

        return false;
    });
    /* ------------------ */

    /* --- Clear Events --- */
    /* -------------------- */
    $("#btnClearMaintenaceTarget").click(function () {
        $("#MaintenanceTargetContractCode").val("");
        $("#MaintenanceTargetContractCode").ResetToNormalControl();

        return false;
    });
    /* -------------------- */
}






/* --- Initial -------------------------------------------------------- */
/* -------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "MaintenanceTargetContractCode",
        "MaintenanceMemo"
    ]);

    /* --- Initial --- */
    /* --------------- */
    InitialMaintenanceEvents();
    /* --------------- */

    $("#MaintenanceTargetProductTypeCode").DisableControl();
    $("#MaintenanceTypeCode").DisableControl();
    $("#MaintenanceCycle").DisableControl();
    $("#MaintenanceMemo").DisableControl();

    if (GetDisableFlag("MaintenanceTargetContract") == true) {
        $("#MaintenanceTargetContractCode").attr("readonly", true);
        $("#btnAddMaintenanceTarget").attr("disabled", true);
        $("#btnClearMaintenaceTarget").attr("disabled", true);
    }

    $("#MaintenanceMemo").SetMaxLengthTextArea(500);
});
function InitialMaintenanceEvents() {
    $("#MaintenanceTargetProductTypeCode").RelateControlEvent(maintenance_target_product_type_code_change);
}
/* -------------------------------------------------------------------- */

/* --- Events --------------------------------------------------------- */
/* -------------------------------------------------------------------- */
function maintenance_target_product_type_code_change() {
    if ($("#MaintenanceTargetProductTypeCode").val() == $("#C_MA_TARGET_PROD_TYPE_SECOM").val()) {
        $("#MaintenanceTargetContractCode").removeAttr("readonly");
        $("#btnAddMaintenanceTarget").removeAttr("disabled");
        $("#btnClearMaintenaceTarget").removeAttr("disabled");
    }
    else {
        $("#MaintenanceTargetContractCode").val("");
        DeleteAllRow(gridMATarget);

        $("#MaintenanceTargetContractCode").attr("readonly", true);
        $("#btnAddMaintenanceTarget").attr("disabled", true);
        $("#btnClearMaintenaceTarget").attr("disabled", true);
    }
}
/* -------------------------------------------------------------------- */

/* --- Output Methods ---------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function GetQUS030_06_SectionData() {
    var qb = CreateObjectData($("#formMaintenanceDetail").serialize());
    if (qb != null) {
        qb.MaintenanceTargetProductTypeCode = $("#MaintenanceTargetProductTypeCode").val();
        qb.MaintenanceTypeCode = $("#MaintenanceTypeCode").val();
        qb.MaintenanceCycle = $("#MaintenanceCycle").val();
    }

    var vv = new Array();
    if (CheckFirstRowIsEmpty(gridMATarget) == false) {
        for (var i = 0; i < gridMATarget.getRowsNum(); i++) {
            var codeCol = gridMATarget.getColIndexById("ContractCodeShort");

            var code = gridMATarget.cells2(i, codeCol).getValue();
            var iobj = {
                ContractCode: code
            };

            vv.push(iobj);
        }
    }
    if (vv.length == 0)
        vv = null;

    return {
        QuotationBasic: qb,
        MaintenanceList: vv
    };
}
function SetQUS030_06_SectionMode(isview) {
    if (isview) {
        $("#divMaintenanceDetail").SetViewMode(true);
        $("#divMaintenanceTargetContractCode").html("");

        if (gridMATarget != undefined) {
            var detailCol = gridMATarget.getColIndexById("Detail");
            var removeCol = gridMATarget.getColIndexById("Remove");
            
            gridMATarget.setColumnHidden(detailCol, true);
            gridMATarget.setColumnHidden(removeCol, true);
            gridMATarget.setSizes();
        }
    }
    else {
        $("#divMaintenanceDetail").SetViewMode(false);

        if (gridMATarget != undefined) {
            var detailCol = gridMATarget.getColIndexById("Detail");
            var removeCol = gridMATarget.getColIndexById("Remove");
            
            gridMATarget.setColumnHidden(detailCol, false);
            gridMATarget.setColumnHidden(removeCol, false);

            if (CheckFirstRowIsEmpty(gridMATarget) == false) {
                for (var i = 0; i < gridMATarget.getRowsNum(); i++) {
                    gridMATarget.setColspan(gridMATarget.getRowId(i), removeCol, 2);
                }
            }
        }
    }
}
/* ----------------------------------------------------------------------------------- */

function QUS030_06_ResetToNormalControl() {
    $("#divMaintenanceDetail").ResetToNormalControl();
}