/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/control_events.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>
/// <reference path="../../Scripts/Base/download_method.js" />

/// <reference path="../Base/object/ajax_method.js" />
/// <reference path="../Base/object/command_event.js" />
/// <reference path="../Base/object/master_event.js" />

var ICS130 = {

    DefaultPeriodFrom: null,
    DefaultPeriodTo: null,

    GridColumnID: {
    },

    CtrlID: {
        divSearchWHT: "#divSearchWHT",
        formSearchWHT: "#formSearchWHT",
        cboPeriodFrom: "#cboPeriodFrom",
        cboPeriodTo: "#cboPeriodTo",
        PeriodTo: "#PeriodTo",
        rdoIMS: "#rdoIMS",
        rdoAccount: "#rdoAccount",
        btnSearch: "#btnSearch",
        btnClear: "#btnClear",
        divSearchResult: "#divSearchResult",
        divGridIMS: "#divGridIMS",
        divGridAccount: "#divGridAccount",
        btnDownload: "#btnDownload"
    },

    Grids: {
        grdIMS: null,
        grdAccount: null
    },

    EventHandlers: {

        OnReportTypeChanged: function () {
            if ($(ICS130.CtrlID.rdoAccount).is(":checked")) {
                $(ICS130.CtrlID.PeriodTo).hide();
            }
            else {
                $(ICS130.CtrlID.PeriodTo).show();
            }
        },


        grdAccount_OnLoadedData: function () {
            var grid = ICS130.Grids.grdAccount;

            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);

            }

            grid.setSizes();
        },

        grdIMS_OnLoadedData: function () {
            var grid = ICS130.Grids.grdIMS;

            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);

            }

            grid.setSizes();
        },

        btnSearch_OnClick: function () {
            var params = $(ICS130.CtrlID.formSearchWHT).serializeObject2();

            if ($(ICS130.CtrlID.rdoAccount).is(":checked")) {

                var params = {
                    period: $(ICS130.CtrlID.cboPeriodFrom).val()
                }

                $(ICS130.CtrlID.btnSearch).attr("disabled", true);
                master_event.LockWindow(true);

                $(ICS130.CtrlID.divGridAccount).LoadDataToGrid(
                    ICS130.Grids.grdAccount,
                    ROWS_PER_PAGE_FOR_SEARCHPAGE,
                    true,
                    "/Income/ICS130_SearchWHTReportForAccount",
                    params,
                    "doWHTReportForAccount",
                    false,
                    function (result, controls, isWarning) { //post-load
                        $(ICS130.CtrlID.btnSearch).removeAttr("disabled");

                        if (CheckFirstRowIsEmpty(ICS130.Grids.grdAccount)) {
                            $(ICS130.CtrlID.btnDownload).attr("disabled", true);
                        }
                        else {
                            $(ICS130.CtrlID.btnDownload).removeAttr("disabled");
                        }

                        master_event.LockWindow(false);
                    },
                    function () {
                        $(ICS130.CtrlID.divGridIMS).hide();
                        $(ICS130.CtrlID.divGridAccount).show();
                        $(ICS130.CtrlID.divSearchResult).show();
                    }
                );
            }
            else if ($(ICS130.CtrlID.rdoIMS).is(":checked")) {

                var params = {
                    periodFrom: $(ICS130.CtrlID.cboPeriodFrom).val(),
                    periodTo: $(ICS130.CtrlID.cboPeriodTo).val()
                }

                $(ICS130.CtrlID.btnSearch).attr("disabled", true);
                master_event.LockWindow(true);

                $(ICS130.CtrlID.divGridIMS).LoadDataToGrid(
                    ICS130.Grids.grdIMS,
                    ROWS_PER_PAGE_FOR_SEARCHPAGE,
                    true,
                    "/Income/ICS130_SearchWHTReportForIMS",
                    params,
                    "doWHTReportForIMS",
                    false,
                    function (result, controls, isWarning) { //post-load
                        $(ICS130.CtrlID.btnSearch).removeAttr("disabled");

                        if (CheckFirstRowIsEmpty(ICS130.Grids.grdIMS)) {
                            $(ICS130.CtrlID.btnDownload).attr("disabled", true);
                        }
                        else {
                            $(ICS130.CtrlID.btnDownload).removeAttr("disabled");
                        }

                        master_event.LockWindow(false);
                    },
                    function () {
                        $(ICS130.CtrlID.divGridIMS).show();
                        $(ICS130.CtrlID.divGridAccount).hide();
                        $(ICS130.CtrlID.divSearchResult).show();
                    }
                );
            }
        },

        btnClear_OnClick: function () {
            ICS130.ClearScreen();
            return false;
        },

        btnDownload_OnClick: function () {

            $(ICS130.CtrlID.btnDownload).attr("disabled", true);
            ajax_method.CallScreenController("/Income/ICS130_ValidateDownloadReport", null, function (result, controls, isWarning) {

                if (result != null && result == true) {
                    download_method.CallDownloadController("ifDownload", "/Income/ICS130_DownloadReport");
                }


            }, false);

            $(ICS130.CtrlID.btnDownload).removeAttr("disabled");
        }
    },

    ClearScreen: function () {
        $(ICS130.CtrlID.divSearchWHT).clearForm();

        if (ICS130.Grids.grdIMS != null
            && !CheckFirstRowIsEmpty(ICS130.Grids.grdIMS)
            && ICS130.Grids.grdIMS.getRowsNum() > 0) {
            DeleteAllRow(ICS130.Grids.grdIMS);
            ICS130.Grids.grdIMS.setSizes();
        }

        if (ICS130.Grids.grdAccount != null
            && !CheckFirstRowIsEmpty(ICS130.Grids.grdAccount)
            && ICS130.Grids.grdAccount.getRowsNum() > 0) {
            DeleteAllRow(ICS130.Grids.grdAccount);
            ICS130.Grids.grdAccount.setSizes();
        }

        $(ICS130.CtrlID.cboPeriodFrom).val(ICS110.DefaultPeriodFrom);
        $(ICS130.CtrlID.cboPeriodTo).val(ICS110.DefaultPeriodTo);

        $(ICS130.CtrlID.rdoAccount).attr("checked", true);
        $(ICS130.CtrlID.rdoAccount).click();
        $(ICS130.CtrlID.divGridAccount).show();
        $(ICS130.CtrlID.divGridIMS).hide();

        $(ICS130.CtrlID.btnDownload).attr("disabled", true);

        $(ICS130.CtrlID.divSearchResult).hide();
    },

    InitializeGrid: function () {

        ICS130.Grids.grdIMS = $(ICS130.CtrlID.divGridIMS).InitialGrid(
            ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/income/ICS130_InitialSearchResultIMS"
            , function () {
                BindOnLoadedEvent(ICS130.Grids.grdIMS, ICS130.EventHandlers.grdIMS_OnLoadedData);
            }
        );

        ICS130.Grids.grdAccount = $(ICS130.CtrlID.divGridAccount).InitialGrid(
            ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/income/ICS130_InitialSearchResultAccount"
            , function () {
                BindOnLoadedEvent(ICS130.Grids.grdAccount, ICS130.EventHandlers.grdAccount_OnLoadedData);
            }
        );

    },

    InitializeScreen: function () {
        //ClearDateFromToControl(ICS130.CtrlID.txtDocumentDateFrom, ICS130.CtrlID.txtDocumentDateTo);
        //InitialDateFromToControl(ICS130.CtrlID.txtDocumentDateFrom, ICS130.CtrlID.txtDocumentDateTo);

        $(ICS130.CtrlID.rdoIMS).click(ICS130.EventHandlers.OnReportTypeChanged);
        $(ICS130.CtrlID.rdoAccount).click(ICS130.EventHandlers.OnReportTypeChanged);

        $(ICS130.CtrlID.btnSearch).click(ICS130.EventHandlers.btnSearch_OnClick);
        $(ICS130.CtrlID.btnClear).click(ICS130.EventHandlers.btnClear_OnClick);
        $(ICS130.CtrlID.btnDownload).click(ICS130.EventHandlers.btnDownload_OnClick);

        ICS110.DefaultPeriodFrom = $(ICS130.CtrlID.cboPeriodFrom).val();
        ICS110.DefaultPeriodTo = $(ICS130.CtrlID.cboPeriodTo).val();

        ICS130.ClearScreen();
    }
};

$(document).ready(function () {
    ICS130.InitializeScreen();
    ICS130.InitializeGrid();
});
