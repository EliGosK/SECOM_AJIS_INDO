/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Base/GridControl.js" />

/* --- Variable ------------------------------------------------------- */
/* -------------------------------------------------------------------- */
var gridMaintenanceList = null;
var btnMaintenanceDetail = "btnMaintenanceDetail";
/* -------------------------------------------------------------------- */




function CTS010_05_InitialGrid() {
    gridMaintenanceList = $("#gridMaintenanceList").LoadDataToGridWithInitial(0, false, false,
                                "/Contract/CTS010_GetMaintenanceList",
                                "",
                                "tbt_RelationType", false);
    SpecialGridControl(gridMaintenanceList, ["Detail"]);

    /* --- Bind Grid events --- */
    /* ------------------------ */
    BindOnLoadedEvent(gridMaintenanceList, function (gen_ctrl) {
        for (var i = 0; i < gridMaintenanceList.getRowsNum(); i++) {
            var row_id = gridMaintenanceList.getRowId(i);

            if (gen_ctrl == true) {
                GenerateDetailButton(gridMaintenanceList, btnMaintenanceDetail, row_id, "Detail", true);
            }

            /* --- Set Event --- */
            /* ----------------- */
            BindGridButtonClickEvent(btnMaintenanceDetail, row_id, function (rid) {
                var codeCol = gridMaintenanceList.getColIndexById("RelatedContractCodeShort");
                var ptCol = gridMaintenanceList.getColIndexById("ProductTypeCode");

                var obj = {
                    strContractCode: gridMaintenanceList.cells(rid, codeCol).getValue()
                };
                if (gridMaintenanceList.cells(rid, ptCol).getValue() == $("#C_PROD_TYPE_SALE").val()) {
                    ajax_method.CallScreenControllerWithAuthority("/Common/CMS160", obj, true);
                }
                else {
                    ajax_method.CallScreenControllerWithAuthority("/Common/CMS140", obj, true);
                }
            });
            /* ----------------- */
        }
        gridMaintenanceList.setSizes();
    });
    /* ------------------------ */
}
function GetCTS010_05_SectionData() {
    return {
        MaintenanceContractStartMonth: $("#MaintenanceContractStartMonth").val(),
        MaintenanceContractStartYear: $("#MaintenanceContractStartYear").val(),
        MaintenanceFeeTypeCode: $("#MaintenanceFeeTypeCode").val(),
        MaintenanceMemo: $("#MaintenanceMemo").val()
    };
}
function SetCTS010_05_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#divMaintenanceInfo_MA").SetViewMode(true);

        if (gridMaintenanceList != undefined) {
            var detailCol = gridMaintenanceList.getColIndexById("Detail");

            gridMaintenanceList.setColumnHidden(detailCol, true);
            gridMaintenanceList.setSizes();
        }
    }
    else {
        $("#divMaintenanceInfo_MA").SetViewMode(false);

        if (gridMaintenanceList != undefined) {
            var detailCol = gridMaintenanceList.getColIndexById("Detail");

            gridMaintenanceList.setColumnHidden(detailCol, false);
            gridMaintenanceList.setSizes();
        }
    }
}
function SetCTS010_05_EnableSection(enable) {
    $("#divMaintenanceInfo_MA").SetEnableView(enable);
    $("#gridMaintenanceList").find("img").each(function () {
        if (this.id != "" && this.id != undefined) {
            if (this.id.indexOf(btnMaintenanceDetail) == 0) {
                var row_id = GetGridRowIDFromControl(this);
                EnableGridButton(gridMaintenanceList, btnMaintenanceDetail, row_id, "Detail", enable);
            }
        }
    });

    if (enable) {
        $("#MaintenanceTargetProductTypeCodeName").attr("readonly", true);
        $("#MaintenanceTypeCodeName").attr("readonly", true);
        $("#MaintenanceCycle").attr("readonly", true);
    }
}



/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "MaintenanceMemo"
    ]);

    var ChangeMaintenanceFeeType = function () {
        if ($("#MaintenanceFeeTypeCode").val() == $("#C_MA_FEE_TYPE_RESULT_BASED").val()) {
            $("#DevideContractFeeBillingFlag").attr("disabled", true);
            $("#DevideContractFeeBillingFlag").removeAttr("checked");
        }
        else {
            $("#DevideContractFeeBillingFlag").removeAttr("disabled");
            OrderContractfeeChange();
        }
    };

    $("#MaintenanceFeeTypeCode").change(ChangeMaintenanceFeeType);
    ChangeMaintenanceFeeType();
});


/* ----------------------------------------------------------------------------------- */