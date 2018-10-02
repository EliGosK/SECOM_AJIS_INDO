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

/// <reference path="../Base/object/command_event.js" />

var IVS201 = {};

IVS201._showAllLocation = null;

IVS201.CtrlID = {
    divSearchCriteria: "#divSearchCriteria",
    formSearch: "#formSearch",
    cboSearchInvOffice: "#cboSearchInvOffice",
    cboSearchInvLocation: "#cboSearchInvLocation",
    cboSearchInstArea: "#cboSearchInstArea",
    txtSearchShelfNoFrom: "#txtSearchShelfNoFrom",
    txtSearchShelfNoTo: "#txtSearchShelfNoTo",
    txtSearchInstrumentCode: "#txtSearchInstrumentCode",
    txtSearchInstrumentName: "#txtSearchInstrumentName",
    btnSearch: "#btnSearch",
    btnClear: "#btnClear",
    divInstQtyList: "#divInstQtyList",
    divGrdInstQtyList: "#divGrdInstQtyList"
};

IVS201.Grids = {
    grdSearch: null
};

IVS201.GridSearchColId = {
    OfficeName: "OfficeName",
    LocationName: "LocationName",
    InstrumentCode: "InstrumentCode",
    InstrumentName: "InstrumentName",
    AreaNameShort: "AreaNameShort",
    AreaName: "AreaName",
    ShelfNo: "ShelfNo",
    CurrentQty: "CurrentQty"
};

IVS201.EventHandlers = {

    grdSearch_OnLoadedData: function () {
        var grid = IVS201.Grids.grdSearch;
        grid.setSizes();
    },

    btnSearch_OnClick: function () {
        $(IVS201.CtrlID.btnSearch).attr("disabled", true);
        master_event.LockWindow(true);

        var params = $(IVS201.CtrlID.formSearch).serializeObject2();

        params.OfficeCodeList = [];
        $(IVS201.CtrlID.cboSearchInvOffice + " > option[value!='']").each(function (index) {
            params.OfficeCodeList.push($(this).val());
        });

        params.LocationCodeList = [];
        $(IVS201.CtrlID.cboSearchInvLocation + " > option[value!='']").each(function (index) {
            params.LocationCodeList.push($(this).val());
        });

        params.txtSearchShelfNoFrom = $(IVS201.CtrlID.txtSearchShelfNoFrom).attr("id");
        params.txtSearchShelfNoTo = $(IVS201.CtrlID.txtSearchShelfNoTo).attr("id");

        $(IVS201.CtrlID.divGrdInstQtyList).LoadDataToGrid(
            IVS201.Grids.grdSearch, ROWS_PER_PAGE_FOR_SEARCHPAGE
            , false, "/inventory/IVS201_GetIVS201"
            , params, "doResultIVS201", false
            , function (result, controls) {
                $(IVS201.CtrlID.btnSearch).attr("disabled", false);
                master_event.LockWindow(false);
                //$(IVS201.CtrlID.divInstQtyList).each(function () {
                //    this.scrollIntoView();
                //});
                master_event.ScrollWindow(IVS201.CtrlID.divInstQtyList);

                if (controls != undefined) {
                    VaridateCtrl(controls, controls);
                } else {
                    $(IVS201.CtrlID.txtSearchShelfNoFrom).removeClass("highlight");
                    $(IVS201.CtrlID.txtSearchShelfNoTo).removeClass("highlight");

                };
            }
            , function (result, controls, isWarning) {
                //                if (isWarning == undefined) {
                //                    $(IVS201.CtrlID.divInstQtyList).show();
                //                }
            }
        );

        return false;
    },

    btnClear_OnClick: function () {
        IVS201.ClearScreen();
        return false;
    },

    cboSearchInvOffice_OnChange: function () {
        var selectedOfficeCode = $(IVS201.CtrlID.cboSearchInvOffice).val();
        var showAllLocation = ((selectedOfficeCode || "") == "" || selectedOfficeCode == IVS201_Constant.HeadOfficeCode);
        if (IVS201._showAllLocation == null || IVS201._showAllLocation != showAllLocation) {
            if (showAllLocation) {
                $(IVS201.CtrlID.cboSearchInvLocation).find("span>option").unwrap();
            }
            else {
                var selector = ">option[value!='" + IVS201_Constant.C_INV_LOC_INSTOCK + "'][value!='" + IVS201_Constant.C_INV_LOC_TRANSFER + "'][value!='']";
                $(IVS201.CtrlID.cboSearchInvLocation).find(selector).each(function () {
                    var opt_value = $(this).val();
                    if (opt_value != IVS201_Constant.C_INV_LOC_INSTOCK
                        && opt_value != IVS201_Constant.C_INV_LOC_TRANSFER) {
                        $(this).wrap($("<span></span>").hide());
                    }
                });
            }
            $(IVS201.CtrlID.cboSearchInvLocation).val(null);
            IVS201._showAllLocation = showAllLocation;
        }
    }

};

IVS201.InitializeGrid = function () {

    IVS201.Grids.grdSearch = $(IVS201.CtrlID.divGrdInstQtyList).InitialGrid(
        ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS201_InitialSearchGrid"
        , function () { 
            BindOnLoadedEvent(IVS201.Grids.grdSearch, IVS201.EventHandlers.grdSearch_OnLoadedData);
        }
    );

};

IVS201.InitializeScreen = function () {
    $(IVS201.CtrlID.btnSearch).click(IVS201.EventHandlers.btnSearch_OnClick);
    $(IVS201.CtrlID.btnClear).click(IVS201.EventHandlers.btnClear_OnClick);

    $(IVS201.CtrlID.cboSearchInvOffice).change(IVS201.EventHandlers.cboSearchInvOffice_OnChange);

    $(IVS201.CtrlID.cboSearchInvOffice).attr("name", "OfficeCode");
    $(IVS201.CtrlID.cboSearchInvLocation).attr("name", "LocationCode");
    $(IVS201.CtrlID.cboSearchInstArea).attr("name", "AreaCode");
    $(IVS201.CtrlID.txtSearchShelfNoFrom).attr("name", "ShelfNoFrom");
    $(IVS201.CtrlID.txtSearchShelfNoTo).attr("name", "ShelfNoTo");
    $(IVS201.CtrlID.txtSearchInstrumentCode).attr("name", "InstrumentCode");
    $(IVS201.CtrlID.txtSearchInstrumentName).attr("name", "InstrumentName");

    $(IVS201.CtrlID.divInstQtyList).css("visibility", "inherit");

    IVS201.ClearScreen();

    //$(IVS201.CtrlID.txtSearchInstrumentCode).InitialAutoComplete("/Master/GetInstrumentCode"); //Add by Jutarat A. on 25032014
    $(IVS201.CtrlID.txtSearchInstrumentCode).InitialAutoComplete("/Master/GetInstrumentCodeAll", null, true);
};

IVS201.ClearScreen = function () {
    $(IVS201.CtrlID.divSearchCriteria).clearForm();
    $(IVS201.CtrlID.divInstQtyList).clearForm();
    if (IVS201.Grids.grdSearch != null && !CheckFirstRowIsEmpty(IVS201.Grids.grdSearch) && IVS201.Grids.grdSearch.getRowsNum() > 0) {
        DeleteAllRow(IVS201.Grids.grdSearch);
        IVS201.Grids.grdSearch.setSizes();
    }

    $(IVS201.CtrlID.cboSearchInvOffice).change();
    //    $(IVS201.CtrlID.divInstQtyList).hide();
};

$(document).ready(function () {
    IVS201.InitializeGrid();
    IVS201.InitializeScreen();
});
