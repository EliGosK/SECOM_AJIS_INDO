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

var IVS220 = {};

IVS220.CtrlID = {
    divSearchCriteria: "#divSearchCriteria",
    formSearch: "#formSearch",
    cboSearchInvOffice: "#cboSearchInvOffice",
    cboSearchInvLocation: "#cboSearchInvLocation",
    txtSearchDateFrom: "#txtSearchDateFrom",
    txtSearchDateTo: "#txtSearchDateTo",
    txtSearchInstrumentCode: "#txtSearchInstrumentCode",
    txtSearchInstrumentName: "#txtSearchInstrumentName",
    cboSearchInstArea: "#cboSearchInstArea",
    txtSearchInvSlipNo: "#txtSearchInvSlipNo",
    txtSearchContractCode: "#txtSearchContractCode",
    txtSearchSupplierName: "#txtSearchSupplierName",
    cboTransferType: "#cboTransferType",
    btnSearch: "#btnSearch",
    btnClear: "#btnClear",
    divStockInOutList: "#divStockInOutList",
    divGrdStockInOutList: "#divGrdStockInOutList",
    divTotalTransferQty: "#divTotalTransferQty",
    txtTotalTransferQty: "#txtTotalTransferQty"
};

IVS220.Grids = {
    grdSearch: null
};

IVS220.GridSearchColId = {
    TransferDate: "TransferDate",
    InstrumentCodeAndName: "InstrumentCodeAndName",
    SupplierNameAndPurchaseNo: "SupplierNameAndPurchaseNo",
    AreaNameShort: "AreaNameShort",
    AreaName: "AreaName",
    TransferQty: "TransferQty",
    Costs: "Costs",
    SlipIdAndSlipNo: "SlipIdAndSlipNo",
    ContractCodeShow: "ContractCodeShow",
    InstrumentCode: "InstrumentCode",
    InstrumentName: "InstrumentName",
    SlipId: "SlipId",
    InventorySlipNo: "InventorySlipNo",
    ContractCode: "ContractCode",
    ContractCodeShort: "ContractCodeShort",
    CustFullName: "CustFullName",
    SupplierName: "SupplierName",
    PurchaseOrderNo: "PurchaseOrderNo",
    Memo: "Memo"
};

IVS220.EventHandlers = {

    grdSearch_OnLoadedData: function () {
        var grid = IVS220.Grids.grdSearch;

        if (grid != undefined) {
            var slipNoLinkName = "SlipNoLink";
            var idxSupplierNameAndPurchaseNo = grid.getColIndexById(IVS220.GridSearchColId.SupplierNameAndPurchaseNo);
            var idxPurchaseOrderNo = grid.getColIndexById(IVS220.GridSearchColId.PurchaseOrderNo);
            var idxSlipIdAndSlipNo = grid.getColIndexById(IVS220.GridSearchColId.SlipIdAndSlipNo);
            var idxSlipId = grid.getColIndexById(IVS220.GridSearchColId.SlipId);
            var idxInventorySlipNo = grid.getColIndexById(IVS220.GridSearchColId.InventorySlipNo);

            var contractLinkName = "ContractLink";
            var idxContractCodeShow = grid.getColIndexById(IVS220.GridSearchColId.ContractCodeShow);
            var idxContractCode = grid.getColIndexById(IVS220.GridSearchColId.ContractCode);
            var idxContractCodeShort = grid.getColIndexById(IVS220.GridSearchColId.ContractCodeShort);
            var idxContractTarget = grid.getColIndexById(IVS220.GridSearchColId.CustFullName);


            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);

                var pono = grid.cells(row_id, idxPurchaseOrderNo).getValue();
                if (pono && pono != "-") {

                    var link = $("<a></a>");
                    link.attr("name", "PurchaseOrderNoLink");
                    link.attr("href", "#");
                    link.text(pono);

                    var strSupplierNameAndPurchaseNo = grid.cells(row_id, idxSupplierNameAndPurchaseNo).getValue();
                    strSupplierNameAndPurchaseNo = strSupplierNameAndPurchaseNo.replace(pono, $("<p></p>").append(link).html());
                    grid.cells(row_id, idxSupplierNameAndPurchaseNo).setValue(strSupplierNameAndPurchaseNo);
                }

                var slipId = grid.cells(row_id, idxSlipId).getValue();
                var slipNo = grid.cells(row_id, idxInventorySlipNo).getValue();
                var idLinkSlipNo = GenerateGridControlID("lnkDownloadInventorySlipNo", row_id);
                var lnkSlipNo = $("<a href='#'></a>")
                    .attr("id", idLinkSlipNo)
                    .attr("name", slipNoLinkName)
                    .attr("slipno", slipNo)
                    .append((slipNo ? slipNo : "-"));
                var divSlip = $("<div></div>")
                    .append("(1) " + (slipId ? slipId : "-"))
                    .append("<br/>")
                    .append("(2) ")
                    .append((slipNo ? lnkSlipNo : "-"));
                grid.cells(row_id, idxSlipIdAndSlipNo).setValue($("<p></p>").append(divSlip).html());

                var contractCode = grid.cells(row_id, idxContractCode).getValue();
                var contractCodeShort = grid.cells(row_id, idxContractCodeShort).getValue();
                var contractTarget = grid.cells(row_id, idxContractTarget).getValue();
                var idLinkContract = GenerateGridControlID("lnkContractDigest", row_id);
                var lnkContract = $("<a href='#'></a>")
                    .attr("id", idLinkContract)
                    .attr("name", contractLinkName)
                    .attr("contractcode", contractCode)
                    .attr("contractcodeshort", contractCodeShort)
                    .append((contractCodeShort ? contractCodeShort : "-"));
                var divContract = $("<div></div>")
                    .append("(1) ")
                    .append((contractCodeShort ? lnkContract : "-"))
                    .append("<br/>")
                    .append("(2) " + (contractTarget ? contractTarget : "-"));
                grid.cells(row_id, idxContractCodeShow).setValue($("<p></p>").append(divContract).html());
            }

            $(IVS220.CtrlID.divGrdStockInOutList + " a[name=PurchaseOrderNoLink]")
                .unbind("click")
                .click(function () {
                    var param = {
                        strPurchaseOrderNo: $(this).text(),
                        strReportID: null
                    };

                    ajax_method.CallScreenController("/inventory/IVS220_PrepareDownloadPO", param, function (result, controls) {
                        if (result == true) {
                            var key = ajax_method.GetKeyURL(null);
                            var url = ajax_method.GenerateURL("/inventory/IVS220_DownloadPreparedPO?k=" + key)
                            window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
                        }
                    });
                });


            $(IVS220.CtrlID.divGrdStockInOutList + " a[name=" + slipNoLinkName + "]")
                .unbind("click")
                .click(function () {
                    var inventorySlipNo = $(this).attr("slipno");
                    //ajax_method.CallScreenControllerWithAuthority("/Common/CMS190", obj, true);
                    IVS220.EventHandlers.lnkDownloadInventorySlipNo_Click(inventorySlipNo);
                });

            $(IVS220.CtrlID.divGrdStockInOutList + " a[name=" + contractLinkName + "]")
                .unbind("click")
                .click(function () {
                    var contractCode = $(this).attr("contractcodeshort");
                    //ajax_method.CallScreenControllerWithAuthority("/Common/CMS190", obj, true);
                    IVS220.EventHandlers.lnkContractDigest_Click(contractCode);
                });

        }

        grid.setSizes();
    },

    btnSearch_OnClick: function () {
        $(IVS220.CtrlID.btnSearch).attr("disabled", true);
        master_event.LockWindow(true);

        var params = $(IVS220.CtrlID.formSearch).serializeObject2();
        params.LocationCode = $(IVS220.CtrlID.cboSearchInvLocation).val();

        /// Convert OfficeCode to Array
        if (params.OfficeCode != "") {
            params.OfficeCode = [params.OfficeCode];
        } else {
            params.OfficeCode = [];
            $(IVS220.CtrlID.cboSearchInvOffice + " > option").each(function (index) {
                if (index > 0) {
                    params.OfficeCode.push($(this).val());
                }
            });
        }

        $(IVS220.CtrlID.divGrdStockInOutList).LoadDataToGrid(
            IVS220.Grids.grdSearch, ROWS_PER_PAGE_FOR_SEARCHPAGE
            , false, "/inventory/IVS220_GetIVS220"
            , params, "dtResultIVS220", false
            , function (result, controls) {
                $(IVS220.CtrlID.btnSearch).attr("disabled", false);
                master_event.LockWindow(false);
                //$(IVS220.CtrlID.divStockInOutList).each(function () {
                //    this.scrollIntoView();
                //});

                // - Add by Nontawat L. on 03-Jul-2014
                // - End
                IVS220.RefreshTotalTransferQty();

                master_event.ScrollWindow(IVS220.CtrlID.divStockInOutList);
            }
            , function (result, controls, isWarning) {
                //                if (isWarning == undefined) {
                //                    $(IVS220.CtrlID.divStockInOutList).show();
                //                }
            }
        );

        return false;
    },

    btnClear_OnClick: function () {
        IVS220.ClearScreen();
        return false;
    },

    cboSearchInvOffice_OnChange: function () {
        //        var params = {
        //            strOfficeCode: $(IVS220.CtrlID.cboSearchInvOffice).val()
        //        };

        //        ajax_method.CallScreenController("/inventory/IVS220_CheckHeadOffice", params, function (result, controls) {
        //            if (params.strOfficeCode == "") {
        //                $(IVS220.CtrlID.cboSearchInvLocation).SetDisabled(false);
        //            } else if (result === false) {
        //                $(IVS220.CtrlID.cboSearchInvLocation).val(IVS220_Constant.C_INV_LOC_INSTOCK);
        //                $(IVS220.CtrlID.cboSearchInvLocation).SetDisabled(true);
        //            } else {
        //                $(IVS220.CtrlID.cboSearchInvLocation).SetDisabled(false);
        //            }
        //        });
        var strOfficeCode = $(IVS220.CtrlID.cboSearchInvOffice).val();
        if (strOfficeCode == "") {
            $(IVS220.CtrlID.cboSearchInvLocation).SetDisabled(false);
        } else if (strOfficeCode != IVS220_Constant.HeadOfficeCode) {
            $(IVS220.CtrlID.cboSearchInvLocation).val(IVS220_Constant.C_INV_LOC_INSTOCK);
            $(IVS220.CtrlID.cboSearchInvLocation).SetDisabled(true);
        } else {
            $(IVS220.CtrlID.cboSearchInvLocation).SetDisabled(false);
        }
    },

    lnkDownloadInventorySlipNo_Click: function (inventorySlipNo) {

        var objParam = { inventorySlipNo: inventorySlipNo }
        ajax_method.CallScreenController("/Inventory/IVS220_CheckExistFile", objParam, function (data) {

            if (data != undefined) {
                if (data == "1") {
                    var key = ajax_method.GetKeyURL(null);
                    var link = ajax_method.GenerateURL("/Inventory/IVS220_DownloadInventorySlip?inventorySlipNo=" + inventorySlipNo + "&k=" + key);

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

    lnkContractDigest_Click: function (contractCode) {
        var obj = {
            strContractCode: contractCode
        };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS190", obj, true);
    }
};

IVS220.InitializeGrid = function () {

    IVS220.Grids.grdSearch = $(IVS220.CtrlID.divGrdStockInOutList).InitialGrid(
        ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS220_InitialSearchGrid"
        , function () {
            BindOnLoadedEvent(IVS220.Grids.grdSearch, IVS220.EventHandlers.grdSearch_OnLoadedData);
        }
    );

    SpecialGridControl(IVS220.Grids.grdSearch, ["InventorySlipNo"]);
};

IVS220.InitializeScreen = function () {
    ClearDateFromToControl(IVS220.CtrlID.txtSearchDateFrom, IVS220.CtrlID.txtSearchDateTo);
    InitialDateFromToControl(IVS220.CtrlID.txtSearchDateFrom, IVS220.CtrlID.txtSearchDateTo);

    $(IVS220.CtrlID.btnSearch).click(IVS220.EventHandlers.btnSearch_OnClick);
    $(IVS220.CtrlID.btnClear).click(IVS220.EventHandlers.btnClear_OnClick);

    $(IVS220.CtrlID.cboSearchInvOffice).attr("name", "OfficeCode");
    $(IVS220.CtrlID.cboSearchInvLocation).attr("name", "LocationCode");
    $(IVS220.CtrlID.txtSearchDateFrom).attr("name", "DateFrom");
    $(IVS220.CtrlID.txtSearchDateTo).attr("name", "DateTo");
    $(IVS220.CtrlID.txtSearchInstrumentCode).attr("name", "InstrumentCode");
    $(IVS220.CtrlID.txtSearchInstrumentName).attr("name", "InstrumentName");
    $(IVS220.CtrlID.cboSearchInstArea).attr("name", "AreaCode");
    $(IVS220.CtrlID.txtSearchInvSlipNo).attr("name", "InventorySlipNo");
    $(IVS220.CtrlID.txtSearchContractCode).attr("name", "ContractCode");
    $(IVS220.CtrlID.txtSearchSupplierName).attr("name", "SupplierName");
    $(IVS220.CtrlID.cboTransferType).attr("name", "TransferType");

    $(IVS220.CtrlID.divStockInOutList).css("visibility", "inherit");

    if ($(IVS220.CtrlID.cboSearchInvOffice).find("option[value=\"" + IVS220_Constant.HeadOfficeCode + "\"]").length > 0) {
        $(IVS220.CtrlID.cboSearchInvOffice).change(IVS220.EventHandlers.cboSearchInvOffice_OnChange);
    }
    else {
        $(IVS220.CtrlID.cboSearchInvLocation).val(IVS220_Constant.C_INV_LOC_INSTOCK);
        $(IVS220.CtrlID.cboSearchInvLocation).SetDisabled(true);
    }

    IVS220.ClearScreen();

    //$(IVS220.CtrlID.txtSearchInstrumentCode).InitialAutoComplete("/Master/GetInstrumentCode"); //Add by Jutarat A. on 25032014
    $(IVS220.CtrlID.txtSearchInstrumentCode).InitialAutoComplete("/Master/GetInstrumentCodeAll", null, true);

    $(IVS220.CtrlID.txtTotalTransferQty).SetReadOnly(true);
};

IVS220.ClearScreen = function () {
    //$(IVS220.CtrlID.cboSearchInvLocation).SetDisabled(false);
    $(IVS220.CtrlID.cboSearchInvOffice).val(null);

    $(IVS220.CtrlID.divSearchCriteria).clearForm();
    $(IVS220.CtrlID.divStockInOutList).clearForm();

    var dateTmp = new Date();
    dateTmp.setDate(1);
    SetDateFromToData(IVS220.CtrlID.txtSearchDateFrom, IVS220.CtrlID.txtSearchDateTo, dateTmp, new Date());

    if (IVS220.Grids.grdSearch != null && !CheckFirstRowIsEmpty(IVS220.Grids.grdSearch) && IVS220.Grids.grdSearch.getRowsNum() > 0) {
        DeleteAllRow(IVS220.Grids.grdSearch);
        IVS220.Grids.grdSearch.setSizes();
    }
    //    $(IVS220.CtrlID.divStockInOutList).hide();
    IVS220.RefreshTotalTransferQty();
};

IVS220.RefreshTotalTransferQty = function () {
    var grid = IVS220.Grids.grdSearch;
    if (CheckFirstRowIsEmpty(grid, false) == false) {
        var totalrows = grid.getRowsNum();
        var totalqty = 0;
        for (var i = 0; i < totalrows; i++) {
            var row_id = grid.getRowId(i);
            var qty = grid.cells(row_id, grid.getColIndexById(IVS220.GridSearchColId.TransferQty)).getValue();
            qty = parseInt(qty.replace(/[,]/gi, "").trim())
            if (qty != NaN) {
                totalqty += qty;
            }
        }
        $(IVS220.CtrlID.txtTotalTransferQty).val(SetNumericText(totalqty, 0));
        $(IVS220.CtrlID.divTotalTransferQty).show();
    }
    else {
        $(IVS220.CtrlID.divTotalTransferQty).hide();
        $(IVS220.CtrlID.txtTotalTransferQty).val("");
    }
};

$(document).ready(function () {
    IVS220.InitializeGrid();
    IVS220.InitializeScreen();
});
