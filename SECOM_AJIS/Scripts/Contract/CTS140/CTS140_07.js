var CTS140_07 = {
    gridMaintenance: null,

    InitialControl: function () {
        CTS140_07.GetMaintenanceGrid();

        $("#btnAdd").click(CTS140_07.BtnAddClick);
        $("#btnClear").click(CTS140_07.BtnClearClick);

        //Add by Jutarat A. on 14082013
        $("#MaintenanceTypeCode").val($("#MaintenanceTypeCodeValue").val());
        $("#MaintenanceCycle").val($("#MaintenanceCycleValue").val());
        $("#Month").val($("#MaintenanceContractStartMonthValue").val());
        $("#Year").val($("#MaintenanceContractStartYearValue").val());
        $("#MaintenanceFeeType").val($("#MaintenanceFeeTypeCodeValue").val());
        $("#MaintenanceTargetContractCode").val("");
        //End Add
    },
    SetSectionMode: function (isView) {
        $("#divMaintenanceInformation").SetViewMode(isView);

        //gridMaintenance
        if (isView) {
            if (CTS140_07.gridMaintenance != undefined) {
                var detailCol = CTS140_07.gridMaintenance.getColIndexById("Detail");
                var removeCol = CTS140_07.gridMaintenance.getColIndexById("Remove");
                CTS140_07.gridMaintenance.setColumnHidden(detailCol, true);
                CTS140_07.gridMaintenance.setColumnHidden(removeCol, true);
                CTS140_07.gridMaintenance.setSizes();
            }
        }
        else {
            if (CTS140_07.gridMaintenance != undefined) {
                var detailCol = CTS140_07.gridMaintenance.getColIndexById("Detail");
                var removeCol = CTS140_07.gridMaintenance.getColIndexById("Remove");
                CTS140_07.gridMaintenance.setColumnHidden(detailCol, false);
                CTS140_07.gridMaintenance.setColumnHidden(removeCol, false);
                CTS140_07.gridMaintenance.setSizes();

                $("#gridMaintenance").LoadDataToGrid(CTS140_07.gridMaintenance, 0, false, "/Contract/CTS140_GetMaintenanceGrid",
                                                "", "CTS140_DOMaintenanceGrid", false, null, null);
            }
        }
    },
    DisabledSection: function (isDisabled) {
        $("#divMaintenanceInformation").SetEnableView(!isDisabled);

        //Diable mygridCTS140Maintenance
        if (CTS140_07.gridMaintenance != undefined) {
            if (CheckFirstRowIsEmpty(CTS140_07.gridMaintenance, false) == false) {
                CTS140_07.BindMaintenanceGrid(false, true);
            }
        }
    },

    GetMaintenanceGrid: function () {
        CTS140_07.gridMaintenance = $("#gridMaintenance").LoadDataToGridWithInitial(0, false, false, "/Contract/CTS140_GetMaintenanceGrid", "", "CTS140_DOMaintenanceGrid", false);
        SpecialGridControl(CTS140_07.gridMaintenance, ["Detail", "Remove"]);
        BindOnLoadedEvent(CTS140_07.gridMaintenance, function (gen_ctrl) {
            CTS140_07.BindMaintenanceGrid(true, gen_ctrl);
        });
    },
    BindMaintenanceGrid: function (isEnbBtn, gen_ctrl) {
        var detailColinx = CTS140_07.gridMaintenance.getColIndexById('Detail');
        var removeColinx = CTS140_07.gridMaintenance.getColIndexById('Remove');

        var row_id;
        for (var i = 0; i < CTS140_07.gridMaintenance.getRowsNum(); i++) {
            row_id = CTS140_07.gridMaintenance.getRowId(i);
            if (gen_ctrl == true) {
                GenerateDetailButton(CTS140_07.gridMaintenance, "btnDetail", row_id, "Detail", isEnbBtn);
                GenerateRemoveButton(CTS140_07.gridMaintenance, "btnRemove", row_id, "Remove", isEnbBtn);
            }

            BindGridButtonClickEvent("btnDetail", row_id,
                function (row_id) {
                    CTS140_07.gridMaintenance.selectRow(CTS140_07.gridMaintenance.getRowIndex(row_id));

                    var obj = { "ContractCode": CTS140_07.gridMaintenance.cells(row_id, 0).getValue() };
                    call_ajax_method('/Contract/CTS140_GetScreenCode', obj,
                            function (result, controls) {
                                var obj2 = { "strContractCode": CTS140_07.gridMaintenance.cells(row_id, 0).getValue() };
                                ajax_method.CallScreenControllerWithAuthority("/Common/" + result, obj2, true);
                            }, null);
                }
            );
            BindGridButtonClickEvent("btnRemove", row_id,
                function (row_id) {
                    CTS140_07.gridMaintenance.selectRow(CTS140_07.gridMaintenance.getRowIndex(row_id));

                    var doMaintenanceGrid = {
                        ContractCode: CTS140_07.gridMaintenance.cells(row_id, 0).getValue(),
                        ProductName: ""
                    };
                    call_ajax_method('/Contract/CTS140_Remove', doMaintenanceGrid,
                        function (result, controls) {
                            CTS140_07.GetMaintenanceGrid();
                        }, null);
                }
            );
        }
    },

    BtnAddClick: function () {
        $("#MaintenanceTargetContractCode").ResetToNormalControl();

        var doMaintenanceGrid = {
            ContractCode: $("#MaintenanceTargetContractCode").val()
        };
        call_ajax_method_json('/Contract/CTS140_Add', doMaintenanceGrid,
            function (result, controls) {
                if (controls != undefined) {
                    VaridateCtrl(["MaintenanceTargetContractCode"], controls);
                    return;
                }
                else if (result != undefined) {
                    CTS140_07.GetMaintenanceGrid();
                    $("#MaintenanceTargetContractCode").val("");
                }
            });
    },
    BtnClearClick: function () {
        $("#MaintenanceTargetContractCode").clearForm();
        CloseWarningDialog();
    }
}



//function BindDOMaintenanceInformation() {
//    call_ajax_method_json('/Contract/BindDOMaintenanceInformation_CTS140', "", function (result, controls) {
//        if (result != undefined) {
//            //            $("#MaintenanceTypeCode").val("");
//            //            $("#MaintenanceCycle").val("1");
//            //            $("#Month").val("");
//            //            $("#Year").val("");
//            //            $("#MaintenanceFeeType").val("0");
//            //            $("#MaintenanceTargetContractCode").val("");
//            $("#MaintenanceTypeCode").val(result.MaintenanceTypeCode);
//            $("#MaintenanceCycle").val(result.MaintenanceCycle);
//            $("#Month").val(result.MaintenanceContractStartMonth);
//            $("#Year").val(result.MaintenanceContractStartYear);
//            $("#MaintenanceFeeType").val(result.MaintenanceFeeTypeCode);
//            $("#MaintenanceTargetContractCode").val("");
//        }
//    });
//}
