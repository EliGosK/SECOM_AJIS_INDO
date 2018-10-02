/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.7.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/control_events.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>
/// <reference path="../../Scripts/Base/object/ajax_method.js"/>

/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxgridcell.js" /> 
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js" />
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxgrid.js" />
/// <reference path="../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_data.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js"/>
/// <reference path="../json.js" />
/// <reference path="../json2.js" />
/// <reference path="../number-functions.js" />
/// <reference path="../Base/object/command_event.js" />



var IVS230 = {};

IVS230.CtrlID = {
    divSearchCriteria: "#divSearchCriteria",
    formSearch: "#formSearch",
    txtSearchInvSlipNo: "#txtSearchInvSlipNo",
    cboSearchSlipStatus: "#cboSearchSlipStatus",
    cboSearchCreateOffice: "#cboSearchCreateOffice",
    txtSearchDateFrom: "#txtSearchDateFrom",
    txtSearchDateTo: "#txtSearchDateTo",
    txtSearchEmpNo: "#txtSearchEmpNo",
    txtSearchProjectCode: "#txtSearchProjectCode",
    rdoStockOutTypeAll: "#rdoStockOutTypeAll",
    rdoStockOutType1: "#rdoStockOutType1",
    rdoStockOutType2: "#rdoStockOutType2",
    rdoStockOutType3: "#rdoStockOutType3",
    rdoStockOutType4: "#rdoStockOutType4",
    rdoStockOutType5: "#rdoStockOutType5",
    txtSearchContractCode: "#txtSearchContractCode",
    txtSearchInstrumentCode: "#txtSearchInstrumentCode",
    btnSearch: "#btnSearch",
    btnClear: "#btnClear",
    divInvSlipList: "#divInvSlipList",
    divGrdInvSlipList: "#divGrdInvSlipList",
    divDetail: "#divDetail",
    txtDtlInvSlipNo: "#txtDtlInvSlipNo",
    txtDtlSlipStatus: "#txtDtlSlipStatus",
    txtDtlStockOutDate: "#txtDtlStockOutDate",
    txtDtlReceiveDate: "#txtDtlReceiveDate",
    txtDtlStockOutOffice: "#txtDtlStockOutOffice",
    txtDtlReceiveOffice: "#txtDtlReceiveOffice",
    txtDtlSourceLocation: "#txtDtlSourceLocation",
    txtDtlDestinationLocation: "#txtDtlDestinationLocation",
    txtDtlApproveNo: "#txtDtlApproveNo",
    txtDtlStockOutType: "#txtDtlStockOutType",
    txtDtlMemo: "#txtDtlMemo",
    divGrdInvSlipDtlList: "#divGrdInvSlipDtlList"
};

IVS230.Grids = {
    grdSearch: null,
    grdSearchIsBindEvent: false,
    grdSlipDetail: null,
    grdSlipDetailIsBindEvent: false
};

IVS230.GridSearchColId = {
    InventorySlipNoLink: "InventorySlipNoLink",
    TransferType: "TransferType",
    SlipStatus: "SlipStatus",
    SourceOfficeName: "SourceOfficeName",
    CreateDate: "CreateDate",
    EmpFullName: "EmpFullName",
    DetailButton: "DetailButton",
    ToJson: "ToJson",
    InventorySlipNo: "InventorySlipNo"
};

IVS230.GridDtlColId = {
    InstrumentCode: "InstrumentCode",
    InstrumentName: "InstrumentName",
    SourceAreaName: "SourceAreaName",
    DestAreaName: "DestAreaName",
    TransferQty: "TransferQty"
};

IVS230.EventHandlers = {

    grdSearch_OnLoadedData: function (gen_ctrl) {
        var grid = IVS230.Grids.grdSearch;

        if (!CheckFirstRowIsEmpty(grid)) {
            var slipNoLinkName = "SlipNoLink";
            var idxInventorySlipNoLink = grid.getColIndexById(IVS230.GridSearchColId.InventorySlipNoLink);
            var idxInventorySlipNo = grid.getColIndexById(IVS230.GridSearchColId.InventorySlipNo);

            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                if (gen_ctrl) {
                    var slipNo = grid.cells(row_id, idxInventorySlipNo).getValue();
                    var idLinkSlipNo = GenerateGridControlID("lnkDownloadInventorySlipNo", row_id);
                    var lnkSlipNo = $("<a href='#'></a>")
                        .attr("id", idLinkSlipNo)
                        .attr("name", slipNoLinkName)
                        .attr("slipno", slipNo)
                        .append((slipNo ? slipNo : "-"));
                    var divSlip = $("<div></div>")
                        .append((slipNo ? lnkSlipNo : "-"));
                    grid.cells(row_id, idxInventorySlipNoLink).setValue($("<p></p>").append(divSlip).html());
                    
                    GenerateEditButton(grid, IVS230.GridSearchColId.DetailButton, row_id, IVS230.GridSearchColId.DetailButton, true);
                }
                BindGridButtonClickEvent(IVS230.GridSearchColId.DetailButton, row_id, function (rid) {
                    IVS230.EventHandlers.btnDetail_click(grid, rid);
                });
            }

            $(IVS230.CtrlID.divGrdInvSlipList + " a[name=" + slipNoLinkName + "]")
                .unbind("click")
                .click(function () {
                    var inventorySlipNo = $(this).attr("slipno");
                    IVS230.EventHandlers.lnkDownloadInventorySlipNo_Click(inventorySlipNo);
                });

        }

        grid.setSizes();
    },

    grdSlipDetail_OnLoadedData: function () {
        var grid = IVS230.Grids.grdSlipDetail;
        grid.setSizes();
    },

    btnSearch_OnClick: function () {
        master_event.LockWindow(true);
        $(IVS230.CtrlID.btnSearch).attr("disabled", true);
        master_event.LockWindow(true);

        var params = $(IVS230.CtrlID.formSearch).serializeObject2();

        /// Convert OfficeCode to Array
        if (params.OfficeCode != "") {
            params.OfficeCode = [params.OfficeCode];
        } else {
            params.OfficeCode = [];
            $(IVS230.CtrlID.cboSearchCreateOffice + " > option").each(function (index) {
                if (index > 0) {
                    params.OfficeCode.push($(this).val());
                }
            });
        }

        // Akat K. add
        params.cboSearchCreateOffice = $(IVS230.CtrlID.cboSearchCreateOffice).val();

        $(IVS230.CtrlID.divGrdInvSlipList).LoadDataToGrid(
            IVS230.Grids.grdSearch, ROWS_PER_PAGE_FOR_SEARCHPAGE
            , false, "/inventory/IVS230_GetInventorySlipIVS230"
            , params, "dtResultInventorySlipIVS230", false
            , function (result, controls) {
                master_event.LockWindow(false);
                $(IVS230.CtrlID.btnSearch).attr("disabled", false);
                master_event.LockWindow(false);
                //$(IVS230.CtrlID.divInvSlipList).each(function () {
                //    this.scrollIntoView();
                //});
                master_event.ScrollWindow(IVS230.CtrlID.divInvSlipList);
            }
            , function (result, controls, isWarning) {
                //                if (isWarning == undefined) {
                //                    $(IVS230.CtrlID.divStockInOutList).show();
                //                }
            }
        );

        return false;
    },

    btnClear_OnClick: function () {
        IVS230.InitialScreen();

        return false;
    },

    btnDetail_click: function (grid, rid) {

        var row_index = grid.getRowIndex(rid);
        grid.selectRow(row_index);

        $(IVS230.CtrlID.divDetail).show();

        GridControl.LockGrid(grid);
        GridControl.SetDisabledButtonOnGrid(grid, IVS230.GridSearchColId.DetailButton, IVS230.GridSearchColId.DetailButton, true);

        var strJSONText = grid.cells2(row_index, grid.getColIndexById("ToJson")).getValue();
        var objSlipDetail = JSON.parse(htmlDecode(strJSONText));

        $(IVS230.CtrlID.txtDtlInvSlipNo).val(objSlipDetail.InventorySlipNo);
        $(IVS230.CtrlID.txtDtlSlipStatus).val(objSlipDetail.SlipStatus);
        $(IVS230.CtrlID.txtDtlStockOutDate).val(objSlipDetail.StockOutDateText);
        $(IVS230.CtrlID.txtDtlReceiveDate).val(objSlipDetail.StockInDateText);
        $(IVS230.CtrlID.txtDtlStockOutOffice).val(objSlipDetail.SourceOfficeName);
        $(IVS230.CtrlID.txtDtlReceiveOffice).val(objSlipDetail.DestOfficeName);
        $(IVS230.CtrlID.txtDtlSourceLocation).val(objSlipDetail.SourceLocationName);
        $(IVS230.CtrlID.txtDtlDestinationLocation).val(objSlipDetail.DestLocationName);
        $(IVS230.CtrlID.txtDtlApproveNo).val(objSlipDetail.ApproveNo);
        $(IVS230.CtrlID.txtDtlStockOutType).val(objSlipDetail.TransferType);
        $(IVS230.CtrlID.txtDtlMemo).val(objSlipDetail.Memo);

        DeleteAllRow(IVS230.Grids.grdSlipDetail);

        var params = {
            strInventorySlipNo: objSlipDetail.InventorySlipNo
        };
        $(IVS230.CtrlID.divGrdInvSlipDtlList).LoadDataToGrid(
            IVS230.Grids.grdSlipDetail, 0
            , false, "/inventory/IVS230_GetInventorySlipDetail"
            , params, "dtResultInventorySlipDetail", false
            , function (result, controls) {
                GridControl.UnlockGrid(grid);
                GridControl.SetDisabledButtonOnGrid(grid, IVS230.GridSearchColId.DetailButton, IVS230.GridSearchColId.DetailButton, false);

                //$(IVS230.CtrlID.divDetail).each(function () {
                //    this.scrollIntoView();
                //});
                master_event.ScrollWindow(IVS230.CtrlID.divDetail);
            }
            , null
        );

    },

    lnkDownloadInventorySlipNo_Click: function (inventorySlipNo) {

        var objParam = { inventorySlipNo: inventorySlipNo }
        ajax_method.CallScreenController("/Inventory/IVS230_CheckExistFile", objParam, function (data) {

            if (data != undefined) {
                if (data == "1") {
                    var key = ajax_method.GetKeyURL(null);
                    var link = ajax_method.GenerateURL("/Inventory/IVS230_DownloadInventorySlip?inventorySlipNo=" + inventorySlipNo + "&k=" + key);

                    window.open(link, "download");
                }
                else {

                    var param = { "module": "Common", "code": "MSG0112" };
                    call_ajax_method_json("/Shared/GetMessage", param, function (data) {

                        /* ====== Open info dialog =====*/
                        OpenInformationMessageDialog(param.code, data.Message);
                    });

                }
            }
        });
    },

};

IVS230.InitializeGrid = function () {

    IVS230.Grids.grdSearch = $(IVS230.CtrlID.divGrdInvSlipList).InitialGrid(
        ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS230_InitialSearchGrid"
        , function () {
            BindOnLoadedEvent(IVS230.Grids.grdSearch, IVS230.EventHandlers.grdSearch_OnLoadedData);
            IVS230.Grids.grdSearchIsBindEvent = true;
        }
    );

    SpecialGridControl(IVS230.Grids.grdSearch, [IVS230.GridSearchColId.DetailButton]);

    IVS230.Grids.grdSlipDetail = $(IVS230.CtrlID.divGrdInvSlipDtlList).InitialGrid(
        0, false, "/inventory/IVS230_InitialSlipDetailGrid"
        , function () {
            BindOnLoadedEvent(IVS230.Grids.grdSlipDetail, IVS230.EventHandlers.grdSlipDetail_OnLoadedData);
            IVS230.Grids.grdSlipDetailIsBindEvent = true;
        }
    );

};

IVS230.InitializeScreen = function () {
    ClearDateFromToControl(IVS230.CtrlID.txtSearchDateFrom, IVS230.CtrlID.txtSearchDateTo);
    InitialDateFromToControl(IVS230.CtrlID.txtSearchDateFrom, IVS230.CtrlID.txtSearchDateTo);

    $(IVS230.CtrlID.btnSearch).click(IVS230.EventHandlers.btnSearch_OnClick);
    $(IVS230.CtrlID.btnClear).click(IVS230.EventHandlers.btnClear_OnClick);

    $(IVS230.CtrlID.txtSearchInvSlipNo).attr("name", "InventorySlipno");
    $(IVS230.CtrlID.cboSearchSlipStatus).attr("name", "SlipStatus");
    $(IVS230.CtrlID.cboSearchCreateOffice).attr("name", "OfficeCode");
    $(IVS230.CtrlID.txtSearchDateFrom).attr("name", "DateFrom");
    $(IVS230.CtrlID.txtSearchDateTo).attr("name", "DateTo");
    $(IVS230.CtrlID.txtSearchEmpNo).attr("name", "EmpNo");
    $(IVS230.CtrlID.txtSearchProjectCode).attr("name", "ProjectCode");
    $(IVS230.CtrlID.rdoStockOutTypeAll).attr("name", "StockOutType");
    $(IVS230.CtrlID.rdoStockOutType1).attr("name", "StockOutType");
    $(IVS230.CtrlID.rdoStockOutType2).attr("name", "StockOutType");
    $(IVS230.CtrlID.rdoStockOutType3).attr("name", "StockOutType");
    $(IVS230.CtrlID.rdoStockOutType4).attr("name", "StockOutType");
    $(IVS230.CtrlID.rdoStockOutType5).attr("name", "StockOutType");
    $(IVS230.CtrlID.txtSearchContractCode).attr("name", "ContractCode");
    $(IVS230.CtrlID.txtSearchInstrumentCode).attr("name", "InstrumentCode");    
    $(IVS230.CtrlID.txtSearchInstrumentCode).InitialAutoComplete("/Master/GetInstrumentCodeAll", null, true);
    $(IVS230.CtrlID.txtDtlInvSlipNo).SetDisabled(true);
    $(IVS230.CtrlID.txtDtlSlipStatus).SetDisabled(true);
    $(IVS230.CtrlID.txtDtlStockOutDate).SetDisabled(true);
    $(IVS230.CtrlID.txtDtlReceiveDate).SetDisabled(true);
    $(IVS230.CtrlID.txtDtlStockOutOffice).SetDisabled(true);
    $(IVS230.CtrlID.txtDtlReceiveOffice).SetDisabled(true);
    $(IVS230.CtrlID.txtDtlSourceLocation).SetDisabled(true);
    $(IVS230.CtrlID.txtDtlDestinationLocation).SetDisabled(true);
    $(IVS230.CtrlID.txtDtlApproveNo).SetDisabled(true);
    $(IVS230.CtrlID.txtDtlStockOutType).SetDisabled(true);
    $(IVS230.CtrlID.txtDtlMemo).SetDisabled(true);

    $(IVS230.CtrlID.divDetail).css("visibility", "inherit");
};

IVS230.InitialScreen = function () {
    $(IVS230.CtrlID.divDetail).hide();

    $(IVS230.CtrlID.divSearchCriteria).clearForm();
    $(IVS230.CtrlID.divInvSlipList).clearForm();
    $(IVS230.CtrlID.divDetail).clearForm();

    ClearDateFromToControl(IVS230.CtrlID.txtSearchDateFrom, IVS230.CtrlID.txtSearchDateTo);

    if (IVS230.Grids.grdSearchIsBindEvent) {
        DeleteAllRow(IVS230.Grids.grdSearch);
    }
    if (IVS230.Grids.grdSlipDetailIsBindEvent) {
        DeleteAllRow(IVS230.Grids.grdSlipDetail);
    }

    $(IVS230.CtrlID.rdoStockOutTypeAll).attr("checked", "checked");
};

//IVS230.ClearScreen = function () {
//    $(IVS230.CtrlID.divSearchCriteria).clearForm();
//    $(IVS230.CtrlID.divStockInOutList).clearForm();
//    if (IVS230.Grids.grdSearch != null && !CheckFirstRowIsEmpty(IVS230.Grids.grdSearch) && IVS230.Grids.grdSearch.getRowsNum() > 0) {
//        DeleteAllRow(IVS230.Grids.grdSearch);
//        IVS230.Grids.grdSearch.setSizes();
//    }
//    //    $(IVS230.CtrlID.divStockInOutList).hide();
//};

$(document).ready(function () {
    IVS230.InitializeGrid();
    IVS230.InitializeScreen();
    IVS230.InitialScreen();
});
