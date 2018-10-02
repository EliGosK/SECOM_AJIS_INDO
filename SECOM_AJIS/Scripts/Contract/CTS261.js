
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>
/// <reference path="../../Scripts/Base/AutoComplete.js"/>
/// <reference path="../jquery-1.5.1-vsdoc.js" />
/// <reference path="../Base/GridControl.js" />

var CTS261_ContractDetailGrid;
$(document).ready(function () {

    InitGrid();
});



function InitGrid() {
    if ($('#GridViewContractDetail').length > 0) {

        CTS261_ContractDetailGrid = $('#GridViewContractDetail').LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, true, true, '/Contract/CTS261_GetContractDetail', '', 'doProjectContractDetail', false);

        BindOnLoadedEvent(CTS261_ContractDetailGrid, function () {

            for (var i = 0; i < CTS261_ContractDetailGrid.getRowsNum(); i++) {
                var row_id = CTS261_ContractDetailGrid.getRowId(i);

                GenerateDetailButton(CTS261_ContractDetailGrid, "btnView", row_id, "View", true);

                SpecialGridControl(CTS261_ContractDetailGrid, ["View"]);

                BindGridButtonClickEvent("btnView", row_id, function (rid) {
                    var colContract = CTS261_ContractDetailGrid.getColIndexById('ContractCode_Short');
                    var ContractCode = CTS261_ContractDetailGrid.cells(rid, colContract).getValue();
                    var ColServiceTypeCode = CTS261_ContractDetailGrid.getColIndexById('ServiceTypeCode');
                    var ServiceTypeCode = CTS261_ContractDetailGrid.cells(rid, ColServiceTypeCode).getValue();
                    var ContractInfoCond = { 'strContractCode': ContractCode, 'strServiceTypeCode': ServiceTypeCode };

                  
                    ajax_method.CallScreenControllerWithAuthority('/Common/CMS190', ContractInfoCond, true);
                });
            }
            CTS261_ContractDetailGrid.setSizes();
        });

        
    }
}

function CTS261Initial() {
    ChangeDialogButtonText(["Cancel"], [$('#btnCancel_CTS261').val()]);
}