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



var IVS240 = {};

IVS240.CtrlID = {
    formSearch: "#formSearch",
    txtSearchInstallSlipNo: "#txtSearchInstallSlipNo",
    txtSearchDateFrom: "#txtSearchDateFrom",
    txtSearchDateTo: "#txtSearchDateTo",
    txtSearchContractCode: "#txtSearchContractCode",
    txtSearchProjectCode: "#txtSearchProjectCode",
    cboSearchOperationOffice: "#cboSearchOperationOffice",
    txtSearchSubContractor: "#txtSearchSubContractor",
    btnSearch: "#btnSearch",
    btnClear: "#btnClear",
    divSearchCriteria: "#divSearchCriteria",
    divSearchResultGrid: "#divSearchResultGrid",
    dlInstallSlipNo: "#dlInstallSlipNo",
    divDetail: "#divDetail",
    divDetailResultGrid: "#divDetailResultGrid",
    divShowSlip: "#divShowSlip",
    txtShowSlipNo: "#txtShowSlipNo",
    btnDownloadSlip: "#btnDownloadSlip",
    btnNewRegister: "#btnNewRegister"
};

IVS240.Grids = {
    grdSearch: null,
    grdSearchIsBindEvent: false,
    grdDetail: null,
    grdDetailIsBindEvent: false,

    DetailCustomSort: function (a, b, order, aid, bid) {
        var grid = IVS240.Grids.grdDetail;
        if (a == b) {
            var a2 = grid.cells(aid, grid.getColIndexById(IVS240.GridDetailColId.InstrumentCode)).getValue();
            var b2 = grid.cells(bid, grid.getColIndexById(IVS240.GridDetailColId.InstrumentCode)).getValue();

            if (a2 == b2) {
                var a3 = grid.cells(aid, grid.getColIndexById(IVS240.GridDetailColId.SourceArea)).getValue();
                var b3 = grid.cells(bid, grid.getColIndexById(IVS240.GridDetailColId.SourceArea)).getValue();

                if (order == "asc")
                    return a3 > b3 ? 1 : -1;
                else
                    return a3 < b3 ? 1 : -1;
            }

            if (order == "asc")
                return a2 > b2 ? 1 : -1;
            else
                return a2 < b2 ? 1 : -1;
        }

        if (order == "asc")
            return a > b ? 1 : -1;
        else
            return a < b ? 1 : -1;
    }

};

IVS240.GridSearchColId = {
    SlipNo: "SlipNo",
    ExpectedDate: "ExpectedDateAndContractCode",
    ContractName: "ContractTargetAndPurchaserName",
    SiteName: "SiteName",
    OfficeName: "OperationOfficeAndSubContractor",
    SelectButton: "SelectButton"
};

IVS240.GridDetailColId = {
    SourceShelfNo: "SourceShelfNo",
    InstrumentCode: "InstrumentCode",
    InstrumentName: "InstrumentName",
    SourceAreaCode: "SourceAreaCode",
    SourceArea: "SourceArea",
    TransferQty: "TransferQty"
};

IVS240.EventHandlers = {

    grdSearch_OnLoadedData: function (gen_ctrl) {
        var grid = IVS240.Grids.grdSearch;

        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);
            if (gen_ctrl) {
                GenerateAddButton(grid, IVS240.GridSearchColId.SelectButton, row_id, IVS240.GridSearchColId.SelectButton, true);
            }
            BindGridButtonClickEvent(IVS240.GridSearchColId.SelectButton, row_id, function (rid) {
                IVS240.EventHandlers.btnSelect_OnClick(grid, rid);
            });
        }

        grid.setSizes();
    },

    grdDetail_OnLoadedData: function () {
        var grid = IVS240.Grids.grdDetail;
        grid.setSizes();
    },

    btnSearch_OnClick: function () {
        $(IVS240.CtrlID.btnSearch).attr("disabled", true);
        master_event.LockWindow(true);

        var params = $(IVS240.CtrlID.formSearch).serializeObject2();
        params.OperationOfficeCodeList = [];
        $(IVS240.CtrlID.cboSearchOperationOffice + " > option").each(function (index) {
            if (index > 0) {
                params.OperationOfficeCodeList.push($(this).val());
            }
        });

        $(IVS240.CtrlID.divSearchResultGrid).LoadDataToGrid(
            IVS240.Grids.grdSearch, ROWS_PER_PAGE_FOR_SEARCHPAGE
            , false, "/inventory/IVS240_SearchInstallationSlip"
            , params, "dtSearchInstallationSlipResult", false
            , function (result, controls) {
                $(IVS240.CtrlID.btnSearch).attr("disabled", false);
                master_event.LockWindow(false);

                master_event.ScrollWindow(IVS240.CtrlID.divSearchResultGrid);
            }
            , function (result, controls, isWarning) {
                //Do Nothing
            }
        );

        return false;
    },

    btnClear_OnClick: function () {
        $(IVS240.CtrlID.divSearchCriteria).clearForm();

        ClearDateFromToControl(IVS240.CtrlID.txtSearchDateFrom, IVS240.CtrlID.txtSearchDateTo);

        DeleteAllRow(IVS240.Grids.grdSearch);
    },

    btnSelect_OnClick: function (grid, rid) {
        var row_index = grid.getRowIndex(rid);
        grid.selectRow(row_index);

        $(IVS240.CtrlID.divDetail).show();
        $(IVS240.CtrlID.divShowSlip).hide();

        register_command.SetCommand(IVS240.EventHandlers.cmdRegister_OnClick);
        reset_command.SetCommand(IVS240.EventHandlers.cmdReset_OnClick);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);

        GridControl.LockGrid(grid);
        GridControl.SetDisabledButtonOnGrid(grid, IVS240.GridSearchColId.SelectButton, IVS240.GridSearchColId.SelectButton, true);

        var strSlipNo = grid.cells2(row_index, grid.getColIndexById(IVS240.GridSearchColId.SlipNo)).getValue();

        if (IVS240.DetailDataList.IsSelectedSlipNo(strSlipNo)) {
            var messageParam = { "module": "Inventory", "code": "MSG4062", "param": "" };
            call_ajax_method("/Shared/GetMessage", messageParam, function (data) {
                OpenWarningDialog(data.Message);

                GridControl.UnlockGrid(grid);
                GridControl.SetDisabledButtonOnGrid(grid, IVS240.GridSearchColId.SelectButton, IVS240.GridSearchColId.SelectButton, false);
            });
            return;
        }

        var params = {
            strInstallationSlipNo: strSlipNo
        };

        ajax_method.CallScreenController("/inventory/IVS240_GetStockOutByInstallationSlip", params, function (result, controls) {
            if (result.IsSuccess == true) {
                IVS240.AddDataList(strSlipNo, result.StockData);
                IVS240.AddDetailData(result.StockData);

                GridControl.UnlockGrid(grid);
                GridControl.SetDisabledButtonOnGrid(grid, IVS240.GridSearchColId.SelectButton, IVS240.GridSearchColId.SelectButton, false);
            }
        });
    },

    btnDownloadSlip_OnClick: function () {
        var param = {
            strPickingListNo: $(IVS240.CtrlID.txtShowSlipNo).val()
        };

        ajax_method.CallScreenController("/inventory/IVS240_DownloadPickingList", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);

                //var url = ajax_method.GenerateURL("/inventory/IVS240_DownloadPdfAndWriteLog?k=" + key + "&strDocumentNo=" + result.DocumentNo + "&documentOCC=" + result.DocumentOCC + "&strDocumentCode=" + result.DocumentCode + "&fileName=" + result.FilePath)
                var url = ajax_method.GenerateURL("/inventory/IVS240_DownloadPdfAndWriteLog?k=" + key) //Modify by Jutarat A. on 04122012              
                window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    },

    btnNewRegister_OnClick: function () {
        IVS240.InitialScreen();
    },

    cmdRegister_OnClick: function () {
        var params = {
            lstInstallationSlipNo: IVS240.DetailDataList.GetSelectedInstallSlipNo()
        };
        ajax_method.CallScreenController("/inventory/IVS240_RegisterPickingList", params, function (result, controls) {
            if (result.IsSuccess == false) {
                IVS240.DetailDataList.SetHighlight(result.ErrorInstallSlipNo);
            } else if (result.IsSuccess == true) {
                IVS240.DetailDataList.SetHighlight(null);

                var grid = IVS240.Grids.grdDetail;
//                grid.sortRows(grid.getColIndexById(IVS240.GridDetailColId.SourceShelfNo), undefined, "asc");
//                grid.setSortImgState(true, grid.getColIndexById(IVS240.GridDetailColId.SourceShelfNo), "asc");

                $(IVS240.CtrlID.divSearchCriteria).hide();
                IVS240.LockDataList();

                register_command.SetCommand(null);
                reset_command.SetCommand(null);
                confirm_command.SetCommand(IVS240.EventHandlers.cmdConfirm_OnClick);
                back_command.SetCommand(IVS240.EventHandlers.cmdBack_OnClick);
            }
        });
    },

    cmdReset_OnClick: function () {
        // Confirmation Dialog will process by common modules.
        //var obj = {
        //    module: "Common",
        //    code: "MSG0038"
        //};
        //call_ajax_method("/Shared/GetMessage", obj, function (result) {
        //    OpenOkCancelDialog(result.Code, result.Message, function () {
        //        IVS020.Functions.SetScreenMode(IVS020.ScreenMode.Search);
        //        IVS020.EventHandlers.btnClear_click();
        //    }, null);
        //});
        IVS240.InitialScreen();
    },

    cmdConfirm_OnClick: function () {
        // Akat K. modify
        //var params = IVS240.DetailDataList.GetSelectedInstallSlipNo();
        var params = {
            lstInstallationSlipNo: IVS240.DetailDataList.GetSelectedInstallSlipNo()
        };
        ajax_method.CallScreenController("/inventory/IVS240_ConfirmPickingList", params, function (result, controls) {
            if (result.IsSuccess == false) {
                IVS240.DetailDataList.SetHighlight(result.ErrorInstallSlipNo);
                //if (IVS240.EventHandlers.cmdBack_OnClick != undefined) {
                //    IVS240.EventHandlers.cmdBack_OnClick();
                //}
            } else if (result.IsSuccess == true) {
                IVS240.DetailDataList.SetHighlight(null);

                $(IVS240.CtrlID.txtShowSlipNo).val(result.PickingListNo);
                $(IVS240.CtrlID.divDetail).show();
                $(IVS240.CtrlID.divShowSlip).show();

                register_command.SetCommand(null);
                reset_command.SetCommand(null);
                confirm_command.SetCommand(null);
                back_command.SetCommand(null);
            }
        });
    },

    cmdBack_OnClick: function () {
        $(IVS240.CtrlID.divSearchCriteria).show();
        IVS240.UnlockDataList();

        register_command.SetCommand(IVS240.EventHandlers.cmdRegister_OnClick);
        reset_command.SetCommand(IVS240.EventHandlers.cmdReset_OnClick);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    },

    OnBeforeRemoveDetailItem: function (dt) {
        IVS240.RemoveDetailData(dt.data("data"));
        return true;
    },

    OnAfterRemoveDetailItem: function (dt) {
        //        if (IVS240.DetailDataList.GetSelectedInstallSlipNo().length == 0) {
        //            $(IVS240.CtrlID.divDetail).hide();
        //            $(IVS240.CtrlID.divShowSlip).hide();

        //            $(IVS240.CtrlID.divDetail).clearForm();

        //            register_command.SetCommand(null);
        //            reset_command.SetCommand(null);
        //            confirm_command.SetCommand(null);
        //            back_command.SetCommand(null);
        //        }
    }

};

IVS240.DetailDataList = {

    AddItem: function (dl, val, txt_dt, data, is_showremovelink, func_beforeremove, function_afterremove) {
        var $dl = $(dl);

        var id = $dl.attr("id") + "_" + (new Date()).getTime();
        var $dt = $("<dt></dt>");
        $dt.attr("id", id);
        $dt.val(val);
        $dt.text(txt_dt);
        $dt.data("data", data);

        $dl.append($dt);

        var $a = $('<a class="removelink" href="#">[remove]</a>');
        $a.click(function () {
            if (func_beforeremove != undefined && typeof func_beforeremove == "function") {
                if (func_beforeremove($dt)) {
                    $dt.remove();
                }
            }
            else {
                $dt.remove();
            }

            if (function_afterremove != undefined && typeof function_afterremove == "function") {
                function_afterremove($dt);
            }
        });
        if (is_showremovelink === true) {
            $a.show();
        }
        else {
            $a.hide();
        }

        $dt.append($a);

        return $dt;
    },

    ShowRemoveLink: function (dl) {
        $(".removelink").each(function () {
            $(this).show();
        });
    },

    HideRemoveLink: function (dl) {
        $(".removelink").each(function () {
            $(this).hide();
        });
    },

    SetHighlight: function (list) {
        var IsValueInList = function (val) {
            if (list == undefined || list == null || list.length == 0) {
                return false;
            }
            for (var i = 0; i < list.length; i++) {
                if (list[i] == val) {
                    return true;
                }
            }
        };

        $(IVS240.CtrlID.dlInstallSlipNo + "> dt").each(function () {
            if (IsValueInList($(this).val())) {
                $(this).css("background-color", "#FFE2E2");
            }
            else {
                $(this).css("background-color", "");
            }
        });
    },

    GetSelectedInstallSlipNo: function () {
        var val = [];
        $(IVS240.CtrlID.dlInstallSlipNo + "> dt").each(function () {
            val.push($(this).val());
        });
        return val;
    },

    IsSelectedSlipNo: function (strSlipNo) {
        return $(IVS240.CtrlID.dlInstallSlipNo + "> dt[value='" + strSlipNo + "']").length > 0;
    }

}

IVS240.AddDataList = function (strSlipNo, objDetailData) {
    IVS240.DetailDataList.AddItem(IVS240.CtrlID.dlInstallSlipNo, strSlipNo, strSlipNo, objDetailData, true, 
        IVS240.EventHandlers.OnBeforeRemoveDetailItem, IVS240.EventHandlers.OnAfterRemoveDetailItem);
}

IVS240.LockDataList = function () {
    IVS240.DetailDataList.HideRemoveLink(IVS240.CtrlID.dlInstallSlipNo);
}

IVS240.UnlockDataList = function () {
    IVS240.DetailDataList.ShowRemoveLink(IVS240.CtrlID.dlInstallSlipNo);
}

IVS240.AddDetailData = function (objDetailData) {
    var grid = IVS240.Grids.grdDetail;

    for (var dtl_idx = 0; dtl_idx < objDetailData.length; dtl_idx++) {
        var isFound = false;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum() && !isFound; i++) {
                var row_id = grid.getRowId(i);
                var strSourceShelfNo = grid.cells2(i, grid.getColIndexById(IVS240.GridDetailColId.SourceShelfNo)).getValue();
                var strInstrumentCode = grid.cells2(i, grid.getColIndexById(IVS240.GridDetailColId.InstrumentCode)).getValue();
                var strSourceAreaCode = grid.cells2(i, grid.getColIndexById(IVS240.GridDetailColId.SourceAreaCode)).getValue();
                var intTransferQty = grid.cells2(i, grid.getColIndexById(IVS240.GridDetailColId.TransferQty)).getValue();

                if (objDetailData[dtl_idx].SourceShelfNo == strSourceShelfNo
                && objDetailData[dtl_idx].InstrumentCode == strInstrumentCode
                && objDetailData[dtl_idx].SourceAreaCode == strSourceAreaCode) {

                    intTransferQty += objDetailData[dtl_idx].TransferQty;
                    grid.cells2(i, grid.getColIndexById(IVS240.GridDetailColId.TransferQty)).setValue(intTransferQty);
                    isFound = true;
                }
            }
        }

        if (!isFound) {
            CheckFirstRowIsEmpty(grid, true);
            AddNewRow(grid, [
                objDetailData[dtl_idx].SourceShelfNo,
                ConvertBlockHtml(objDetailData[dtl_idx].InstrumentCode), //objDetailData[dtl_idx].InstrumentCode, //Modify by Jutarat A. on 28112013
                ConvertBlockHtml(objDetailData[dtl_idx].InstrumentName), //objDetailData[dtl_idx].InstrumentName, //Modify by Jutarat A. on 28112013
                objDetailData[dtl_idx].SourceAreaCode,
                objDetailData[dtl_idx].SourceArea,
                objDetailData[dtl_idx].TransferQty
            ]);
        }
    }
};

IVS240.HighlightDetailData = function (list) {
    IVS240.DetailDataList.SetHighlight(list);
}

IVS240.RemoveDetailData = function (objDetailData) {
    var grid = IVS240.Grids.grdDetail;

    for (var dtl_idx = 0; dtl_idx < objDetailData.length; dtl_idx++) {
        var isFound = false;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = grid.getRowsNum() - 1; i >= 0 && !isFound; i--) {
                var row_id = grid.getRowId(i);
                var strSourceShelfNo = grid.cells2(i, grid.getColIndexById(IVS240.GridDetailColId.SourceShelfNo)).getValue();
                var strInstrumentCode = grid.cells2(i, grid.getColIndexById(IVS240.GridDetailColId.InstrumentCode)).getValue();
                var strSourceAreaCode = grid.cells2(i, grid.getColIndexById(IVS240.GridDetailColId.SourceAreaCode)).getValue();
                var intTransferQty = grid.cells2(i, grid.getColIndexById(IVS240.GridDetailColId.TransferQty)).getValue();

                if (objDetailData[dtl_idx].SourceShelfNo == strSourceShelfNo
                && objDetailData[dtl_idx].InstrumentCode == strInstrumentCode
                && objDetailData[dtl_idx].SourceAreaCode == strSourceAreaCode) {

                    intTransferQty -= objDetailData[dtl_idx].TransferQty;
                    if (intTransferQty == 0) {
                        DeleteRow(grid, row_id);
                    }
                    else {
                        grid.cells2(i, grid.getColIndexById(IVS240.GridDetailColId.TransferQty)).setValue(intTransferQty);
                    }
                    isFound = true;
                }
            }
        }

        if (!isFound) {
            alert("unable to remove data from detail: \n"
                + "SourceShelfNo=" + objDetailData[dtl_idx].SourceShelfNo + "\n"
                + "InstrumentCode=" + objDetailData[dtl_idx].InstrumentCode + "\n"
                + "SourceAreaCode=" + objDetailData[dtl_idx].SourceAreaCode + "\n"
            );
        }
    }
};

IVS240.InitializeGrid = function () {

    IVS240.Grids.grdSearch = $(IVS240.CtrlID.divSearchResultGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/inventory/IVS240_InitialSearchGrid"
        , function () {
            BindOnLoadedEvent(IVS240.Grids.grdSearch, IVS240.EventHandlers.grdSearch_OnLoadedData);
            IVS240.Grids.grdSearchIsBindEvent = true;
        }
    );

    SpecialGridControl(IVS240.Grids.grdSearch, [IVS240.GridSearchColId.SelectButton]);

    IVS240.Grids.grdDetail = $(IVS240.CtrlID.divDetailResultGrid).InitialGrid(
        0, false, "/inventory/IVS240_InitialDetailGrid"
        , function () {
            BindOnLoadedEvent(IVS240.Grids.grdDetail, IVS240.EventHandlers.grdDetail_OnLoadedData);
            IVS240.Grids.grdDetailIsBindEvent = true;
            IVS240.Grids.grdDetail.setCustomSorting(IVS240.Grids.DetailCustomSort, IVS240.Grids.grdDetail.getColIndexById(IVS240.GridDetailColId.SourceShelfNo));
        }
    );

};

IVS240.InitializeScreen = function () {
    ClearDateFromToControl(IVS240.CtrlID.txtSearchDateFrom, IVS240.CtrlID.txtSearchDateTo);
    InitialDateFromToControl(IVS240.CtrlID.txtSearchDateFrom, IVS240.CtrlID.txtSearchDateTo);

    $(IVS240.CtrlID.btnSearch).click(IVS240.EventHandlers.btnSearch_OnClick);
    $(IVS240.CtrlID.btnClear).click(IVS240.EventHandlers.btnClear_OnClick);
    $(IVS240.CtrlID.btnDownloadSlip).click(IVS240.EventHandlers.btnDownloadSlip_OnClick);
    $(IVS240.CtrlID.btnNewRegister).click(IVS240.EventHandlers.btnNewRegister_OnClick);

    $(IVS240.CtrlID.txtShowSlipNo).SetDisabled(true);

    $(IVS240.CtrlID.txtSearchInstallSlipNo).attr("name", "InstallationSlipNo");
    $(IVS240.CtrlID.txtSearchDateFrom).attr("name", "ExpectedStockOutDateFrom");
    $(IVS240.CtrlID.txtSearchDateTo).attr("name", "ExpectedStockOutDateTo");
    $(IVS240.CtrlID.txtSearchContractCode).attr("name", "ContractCode");
    $(IVS240.CtrlID.txtSearchProjectCode).attr("name", "ProjectCode");
    $(IVS240.CtrlID.cboSearchOperationOffice).attr("name", "OperationOfficeCode");
    $(IVS240.CtrlID.txtSearchSubContractor).attr("name", "SubContractorName");

    $(IVS240.CtrlID.divDetail).css("visibility", "inherit");
    $(IVS240.CtrlID.divShowSlip).css("visibility", "inherit");
};

IVS240.InitialScreen = function () {
    $(IVS240.CtrlID.divSearchCriteria).show();
    $(IVS240.CtrlID.divDetail).hide();
    $(IVS240.CtrlID.divShowSlip).hide();

    $(IVS240.CtrlID.divSearchCriteria).clearForm();
    $(IVS240.CtrlID.divDetail).clearForm();
    $(IVS240.CtrlID.divShowSlip).clearForm();

    $(IVS240.CtrlID.dlInstallSlipNo).children().remove();

    register_command.SetCommand(null);
    reset_command.SetCommand(null);
    confirm_command.SetCommand(null);
    back_command.SetCommand(null);

    if (IVS240.Grids.grdSearchIsBindEvent) {
        DeleteAllRow(IVS240.Grids.grdSearch);
    }
    if (IVS240.Grids.grdDetailIsBindEvent) {
        DeleteAllRow(IVS240.Grids.grdDetail);
    }
};

$(document).ready(function () {
    IVS240.InitializeGrid();
    IVS240.InitializeScreen();
    IVS240.InitialScreen();
});
