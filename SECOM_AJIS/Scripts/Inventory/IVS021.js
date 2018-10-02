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

var IVS021 = {

    ScreenMode: {
        Search: 0,
        Register: 1,
        Confirm: 2,
        ShowSlipNo: 3
    },

    GridColumnID: {
        SearchResult: {
            SlipNo: "SlipNo",
            ContractCode: "ContractCode",
            ContractTargetName: "CustFullName",
            ExpectedStockOutDate: "SiteName",
            InventoryOffice: "SlipIssueDate",
            InstallationType: "OfficeName",
            InstallationTypeName: "InstallationTypeName",
            BtnEdit: "BtnEdit", BtnEditID: "btnBtnEdit",
            ToJson: "ToJson"
        },
        Register: {
            InstrumentCode: "InstrumentCode",
            InstrumentName: "InstrumentName",
            AddInstalledQty: "AddInstalledQty",
            RemainQty: "RemainQty",
            NewInstSale: "NewInstSale", NewInstSaleID: "txtNewInstSale",
            NewInstSample: "NewInstSample", NewInstSampleID: "txtNewInstSample",
            NewInstRental: "NewInstRental", NewInstRentalID: "txtNewInstRental",
            SecondhandInstRental: "SecondhandInstRental", SecondhandInstRentalID: "txtSecondhandInstRental",
            Remark: "Remark", RemarkID: "txtRemark",
            ToJson: "ToJson"
        }
    },

    Grids: {
        grdSearch: null,
        grdRegister: null
    },

    Vars: {
        objEditingSlip: null
    },

    jQuery: {
        SearchSection: {
            divSearchSection: function () { return $("#divSearchInstallSlip"); },
            divSearchCriteria: function () { return $("#divSearchInstallSlipCriteria"); },
            divSearchResult: function () { return $("#divSearchInstallSlipGrid"); },
            txtSearchInstallationSlipNo: function () { return $("#txtSearchInstallationSlipNo"); },
            txtSearchContractCode: function () { return $("#txtSearchContractCode"); },
            txtSearchContractTargetName: function () { return $("#txtSearchContractTargetName"); },
            txtSearchSiteName: function () { return $("#txtSearchSiteName"); },
            txtSearchExpectedStockOutDateFrom: function () { return $("#txtSearchExpectedStockOutDateFrom"); },
            txtSearchExpectedStockOutDateTo: function () { return $("#txtSearchExpectedStockOutDateTo"); },
            txtSearchInventoryOffice: function () { return $("#txtSearchInventoryOffice"); },
            cboSearchInstallationType: function () { return $("#cboSearchInstallationType"); },
            btnSearch: function () { return $("#btnSearch"); },
            btnClear: function () { return $("#btnClear"); },
            divSearchInstallSlipGrid: function () { return $("#divSearchInstallSlipGrid"); },
            formSearch: function () { return $("#formSearch"); }
        },
        RegisterSection: {
            divRegisterSection: function () { return $("#divRegisterStockOut"); },
            divRegisterHeader: function () { return $("#divRegisterStockOutHeader"); },
            txtRegContractCode: function () { return $("#txtRegContractCode"); },
            txtRegInstallSlipNo: function () { return $("#txtRegInstallSlipNo"); },
            txtRegInstallType: function () { return $("#txtRegInstallType"); },
            txtRegContractTargetNameEN: function () { return $("#txtRegContractTargetNameEN"); },
            txtRegContractTargetNameLC: function () { return $("#txtRegContractTargetNameLC"); },
            txtRegSiteNameEN: function () { return $("#txtRegSiteNameEN"); },
            txtRegSiteNameLC: function () { return $("#txtRegSiteNameLC"); },
            txtRegInventoryOffice: function () { return $("#txtRegInventoryOffice"); },
            txtRegExpectedOutDate: function () { return $("#txtRegExpectedOutDate"); },
            txtRegMemo: function () { return $("#txtRegMemo"); },
            txtRegStockOutDate: function () { return $("#txtStockOutDate"); },
            divRegisterStockOutDetail: function () { return $("#divRegisterStockOutDetail"); },
            divRegisterGrid: function () { return $("#divRegisterGrid"); }
        },
        ShowSlipSection: {
            divShowSlipSection: function () { return $("#divShowSlip"); },
            txtShowSlipNo: function () { return $("#txtShowSlipNo"); },
            btnDownloadSlip: function () { return $("#btnDownloadSlip"); },
            btnNewRegister: function () { return $("#btnNewRegister"); }
        }
    },

    EventHandlers: {
        btnSearch_click: function () {

            IVS021.jQuery.SearchSection.btnSearch().attr("disabled", true);
            master_event.LockWindow(true);

            //var params = CreateObjectData($("#formSearch").serialize(), true);
            var params = $("#formSearch").serializeObject2();
            params.OfficeCode = IVS021_ViewBag.InvHeadOfficeCode;
            params.OfficeName = IVS021_ViewBag.InvHeadOfficeName;

            IVS021.jQuery.SearchSection.divSearchInstallSlipGrid().LoadDataToGrid(
                IVS021.Grids.grdSearch, ROWS_PER_PAGE_FOR_SEARCHPAGE
                , false, "/inventory/IVS021_SearchInstallationSlipForStockOut"
                , params, "doResultInstallationSlipForStockOut", false
                , function () {
                    IVS021.jQuery.SearchSection.btnSearch().attr("disabled", false);
                    master_event.LockWindow(false);
                    master_event.ScrollWindow("#divSearchInstallSlipGrid");
                }
                , null
            );

            return false;
        },

        btnClear_click: function () {
            IVS021.jQuery.SearchSection.divSearchCriteria().clearForm();
            IVS021.jQuery.SearchSection.divSearchResult().clearForm();

            ClearDateFromToControl("#txtSearchExpectedStockOutDateFrom", "#txtSearchExpectedStockOutDateTo");
            //SetDateFromToData("#txtSearchExpectedStockOutDateFrom", "#txtSearchExpectedStockOutDateTo", new Date(), new Date());
            IVS021.jQuery.SearchSection.txtSearchInventoryOffice().val(IVS021_ViewBag.InvHeadOfficeName);

            DeleteAllRow(IVS021.Grids.grdSearch);

            return false;
        },

        btnEditSlip_click: function (grid, rid) {
            var row_index = grid.getRowIndex(rid);
            grid.selectRow(row_index);

            IVS021.Functions.SetScreenMode(IVS021.ScreenMode.Register);
            var strSlipNo = grid.cells2(row_index, grid.getColIndexById(IVS021.GridColumnID.SearchResult.SlipNo)).getValue();

            GridControl.LockGrid(grid);
            GridControl.SetDisabledButtonOnGrid(grid, IVS021.GridColumnID.SearchResult.BtnEditID, IVS021.GridColumnID.SearchResult.BtnEdit, true);

            var strSlipData = grid.cells2(row_index, grid.getColIndexById(IVS021.GridColumnID.SearchResult.ToJson)).getValue();
            var objSlipData = JSON.parse(htmlDecode(strSlipData));
            IVS021.jQuery.RegisterSection.txtRegContractCode().val(objSlipData.ContractCodeShort);
            IVS021.jQuery.RegisterSection.txtRegInstallSlipNo().val(objSlipData.SlipNo);
            IVS021.jQuery.RegisterSection.txtRegInstallType().val(objSlipData.InstallationTypeName);
            IVS021.jQuery.RegisterSection.txtRegContractTargetNameEN().val(objSlipData.CustFullNameEN);
            IVS021.jQuery.RegisterSection.txtRegContractTargetNameLC().val(objSlipData.CustFullNameLC);
            IVS021.jQuery.RegisterSection.txtRegSiteNameEN().val(objSlipData.SiteNameEN);
            IVS021.jQuery.RegisterSection.txtRegSiteNameLC().val(objSlipData.SiteNameLC);
            IVS021.jQuery.RegisterSection.txtRegInventoryOffice().val(objSlipData.OfficeName);
            IVS021.jQuery.RegisterSection.txtRegExpectedOutDate().val(objSlipData.SlipIssueDateDisplay);
            IVS021.jQuery.RegisterSection.txtRegMemo().val("");
            IVS021.jQuery.RegisterSection.txtRegStockOutDate().SetDate(new Date);

            IVS021.Vars.objEditingSlip = objSlipData;

            DeleteAllRow(IVS021.Grids.grdRegister);

            var params = {
                SlipNo: strSlipNo,
                OfficeCode: objSlipData.OfficeCode
            };

            //            IVS021.jQuery.RegisterSection.divRegisterGrid().LoadDataToGrid(
            //                IVS021.Grids.grdRegister, 0
            //                , false, "/inventory/IVS021_SearchInstallationDetailForStockOut"
            //                , params, "doResultInstallationDetailForStockOut", false
            //                , function () {
            //                    GridControl.UnlockGrid(IVS021.Grids.grdSearch);
            //                    GridControl.SetDisabledButtonOnGrid(IVS021.Grids.grdSearch, IVS021.GridColumnID.SearchResult.BtnEditID, IVS021.GridColumnID.SearchResult.BtnEdit, false);
            //                }
            //                , null
            //            );

            master_event.ScrollWindow("#divRegisterStockOut");

            ajax_method.CallScreenController("/inventory/IVS021_SearchInstallationDetailForStockOut", params, function (result, controls) {
                if (result != null) {
                    IVS021.Functions.LoadDataToDetailGrid(result);
                    GridControl.UnlockGrid(IVS021.Grids.grdSearch);
                    GridControl.SetDisabledButtonOnGrid(IVS021.Grids.grdSearch, IVS021.GridColumnID.SearchResult.BtnEditID, IVS021.GridColumnID.SearchResult.BtnEdit, false);
                }
            });
        },

        cmdRegister_Click: function () {
            var params = {
                header: IVS021.Vars.objEditingSlip,
                details: IVS021.Functions.GetInstallDetailForRegister(),
                Memo: IVS021.jQuery.RegisterSection.txtRegMemo().val(),
                StockOutDate: IVS021.jQuery.RegisterSection.txtRegStockOutDate().val()
            };

            ajax_method.CallScreenController("/inventory/IVS021_RegisterInstallationForStockOut", params, function (result, controls) {

                IVS021.jQuery.RegisterSection.divRegisterGrid().find(".highlight").toggleClass("highlight", false);
                VaridateCtrl(controls, controls);

                if (result != null && result.IsSuccess) {
                    IVS021.Functions.SetScreenMode(IVS021.ScreenMode.Confirm);
                }
            });
        },

        cmdReset_Click: function () {
            // Confirmation Dialog will process by common modules.
            //var obj = {
            //    module: "Common",
            //    code: "MSG0038"
            //};
            //call_ajax_method("/Shared/GetMessage", obj, function (result) {
            //    OpenOkCancelDialog(result.Code, result.Message, function () {
            //        IVS021.Functions.SetScreenMode(IVS021.ScreenMode.Search);
            //        IVS021.EventHandlers.btnClear_click();
            //    }, null);
            //});
            IVS021.Functions.SetScreenMode(IVS021.ScreenMode.Search);
            IVS021.EventHandlers.btnClear_click();
        },

        btnNewRegister_click: function () {
            IVS021.Functions.SetScreenMode(IVS021.ScreenMode.Search);
            IVS021.EventHandlers.btnClear_click();
        },

        btnDownloadSlip_click: function () {
            IVS021.jQuery.ShowSlipSection.btnDownloadSlip().SetDisabled(true);
            master_event.LockWindow(true);

            var param = {
                strInvSlipNo: IVS021.jQuery.ShowSlipSection.txtShowSlipNo().val()
            };

            ajax_method.CallScreenController("/inventory/IVS021_DownloadDocument", param, function (result, controls) {
                if (result != null) {
                    var key = ajax_method.GetKeyURL(null);
                    var url = ajax_method.GenerateURL("/inventory/IVS021_DownloadPdfAndWriteLog?k=" + key + "&strDocumentNo=" + result.DocumentNo + "&documentOCC=" + result.DocumentOCC + "&strDocumentCode=" + result.DocumentCode + "&fileName=" + result.FilePath)
                    window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
                }
                IVS021.jQuery.ShowSlipSection.btnDownloadSlip().SetDisabled(false);
                master_event.LockWindow(false);
            });
        },

        cmdBack_Click: function () {
            IVS021.Functions.SetScreenMode(IVS021.ScreenMode.Register);
        },

        cmdConfirm_Click: function () {
            var params = {
                header: IVS021.Vars.objEditingSlip,
                details: IVS021.Functions.GetInstallDetailForRegister(),
                Memo: IVS021.jQuery.RegisterSection.txtRegMemo().val(),
                StockOutDate: IVS021.jQuery.RegisterSection.txtRegStockOutDate().val()
            };

            ajax_method.CallScreenController("/inventory/IVS021_ConfirmInstallationForStockOut", params, function (result, controls) {

                IVS021.jQuery.RegisterSection.divRegisterGrid().find(".highlight").toggleClass("highlight", false);
                VaridateCtrl(controls, controls);

                if (result != null && result.IsSuccess) {
                    IVS021.Functions.SetScreenMode(IVS021.ScreenMode.ShowSlipNo);
                    IVS021.jQuery.ShowSlipSection.txtShowSlipNo().val(result.InvSlipNo);

                    master_event.ScrollWindow("#divShowSlip");
                }
                //else {
                //    IVS021.Functions.SetScreenMode(IVS021.ScreenMode.Register);
                //}
            });
        },

        grdSearch_OnLoadedData: function (gen_ctrl) {
            for (var i = 0; i < IVS021.Grids.grdSearch.getRowsNum(); i++) {
                var row_id = IVS021.Grids.grdSearch.getRowId(i);
                if (gen_ctrl) {
                    //GenerateEditButton(IVS021.Grids.grdSearch, IVS021.GridColumnID.SearchResult.BtnEditID, row_id, IVS021.GridColumnID.SearchResult.BtnEdit, true);
                    GenerateSelectButton(IVS021.Grids.grdSearch, IVS021.GridColumnID.SearchResult.BtnEditID, row_id, IVS021.GridColumnID.SearchResult.BtnEdit, true);
                }
                BindGridButtonClickEvent(IVS021.GridColumnID.SearchResult.BtnEditID, row_id, function (rid) {
                    IVS021.EventHandlers.btnEditSlip_click(IVS021.Grids.grdSearch, rid);
                });
            }

            IVS021.Grids.grdSearch.setSizes();
        }

    },


    Colors: {
        NormalRow: "#FFFFFF",
        ChildRow: "#E3EFFF"
    },

    Functions: {
        InitializeGrid: function () {
            IVS021.Grids.grdSearch = IVS021.jQuery.SearchSection.divSearchInstallSlipGrid().InitialGrid(
                ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS021_InitialSearchGrid"
                , function () {
                    BindOnLoadedEvent(IVS021.Grids.grdSearch, IVS021.EventHandlers.grdSearch_OnLoadedData);
                });

            SpecialGridControl(IVS021.Grids.grdSearch, [IVS021.GridColumnID.SearchResult.BtnEdit]);

            IVS021.Grids.grdRegister = IVS021.jQuery.RegisterSection.divRegisterGrid().InitialGrid(
                0, false, "/inventory/IVS021_InitialRegisterGrid");

            SpecialGridControl(IVS021.Grids.grdRegister, [
                IVS021.GridColumnID.Register.NewInstSale,
                IVS021.GridColumnID.Register.NewInstSample,
                IVS021.GridColumnID.Register.NewInstRental,
                IVS021.GridColumnID.Register.SecondhandInstRental
            ]);
        },

        InitializeScreen: function () {
            ClearDateFromToControl("#txtSearchExpectedStockOutDateFrom", "#txtSearchExpectedStockOutDateTo");
            InitialDateFromToControl("#txtSearchExpectedStockOutDateFrom", "#txtSearchExpectedStockOutDateTo");
            //SetDateFromToData("#txtSearchExpectedStockOutDateFrom", "#txtSearchExpectedStockOutDateTo", new Date(), new Date());
            IVS021.jQuery.SearchSection.txtSearchInventoryOffice().val(IVS021_ViewBag.InvHeadOfficeName);

            IVS021.jQuery.SearchSection.btnSearch().click(IVS021.EventHandlers.btnSearch_click);
            IVS021.jQuery.SearchSection.btnClear().click(IVS021.EventHandlers.btnClear_click);
            IVS021.jQuery.ShowSlipSection.btnNewRegister().click(IVS021.EventHandlers.btnNewRegister_click);
            IVS021.jQuery.ShowSlipSection.btnDownloadSlip().click(IVS021.EventHandlers.btnDownloadSlip_click);

            IVS021.jQuery.SearchSection.divSearchSection().css("visibility", "inherit");
            IVS021.jQuery.RegisterSection.divRegisterSection().css("visibility", "inherit");
            IVS021.jQuery.ShowSlipSection.divShowSlipSection().css("visibility", "inherit");

            IVS021.jQuery.SearchSection.txtSearchInstallationSlipNo().attr("name", "InstallationSlipNo");
            IVS021.jQuery.SearchSection.txtSearchContractCode().attr("name", "ContractCode");
            IVS021.jQuery.SearchSection.txtSearchContractTargetName().attr("name", "ContractTargetName");
            IVS021.jQuery.SearchSection.txtSearchSiteName().attr("name", "SiteName");
            IVS021.jQuery.SearchSection.txtSearchInventoryOffice().attr("name", "OfficeName");
            IVS021.jQuery.SearchSection.cboSearchInstallationType().attr("name", "InstallationTypeCode");
            IVS021.jQuery.SearchSection.txtSearchExpectedStockOutDateFrom().attr("name", "ExpectedStockOutDateFrom");
            IVS021.jQuery.SearchSection.txtSearchExpectedStockOutDateTo().attr("name", "ExpectedStockOutDateTo");

            IVS021.jQuery.SearchSection.txtSearchInventoryOffice().SetReadOnly(true);

            IVS021.jQuery.RegisterSection.txtRegContractCode().SetReadOnly(true);
            IVS021.jQuery.RegisterSection.txtRegInstallSlipNo().SetReadOnly(true);
            IVS021.jQuery.RegisterSection.txtRegInstallType().SetReadOnly(true);
            IVS021.jQuery.RegisterSection.txtRegContractTargetNameEN().SetReadOnly(true);
            IVS021.jQuery.RegisterSection.txtRegContractTargetNameLC().SetReadOnly(true);
            IVS021.jQuery.RegisterSection.txtRegSiteNameEN().SetReadOnly(true);
            IVS021.jQuery.RegisterSection.txtRegSiteNameLC().SetReadOnly(true);
            IVS021.jQuery.RegisterSection.txtRegInventoryOffice().SetReadOnly(true);
            IVS021.jQuery.RegisterSection.txtRegExpectedOutDate().SetReadOnly(true);

            IVS021.jQuery.ShowSlipSection.txtShowSlipNo().SetReadOnly(true);

            IVS021.jQuery.RegisterSection.txtRegMemo().SetMaxLengthTextArea(1000, 3);
            IVS021.jQuery.RegisterSection.txtRegStockOutDate().InitialDate();
            IVS021.jQuery.RegisterSection.txtRegStockOutDate().SetMinDate(IVS021_ViewBag.MINDATE);
            IVS021.jQuery.RegisterSection.txtRegStockOutDate().SetMaxDate(IVS021_ViewBag.MAXDATE);
        },

        SetScreenMode: function (mode) {
            if (mode == IVS021.ScreenMode.Search) {
                IVS021.jQuery.SearchSection.divSearchSection().show();
                IVS021.jQuery.SearchSection.divSearchResult().show();
                IVS021.jQuery.RegisterSection.divRegisterSection().hide();
                IVS021.jQuery.ShowSlipSection.divShowSlipSection().hide();

                register_command.SetCommand(null);
                reset_command.SetCommand(null);
                confirm_command.SetCommand(null);
                back_command.SetCommand(null);

            } else if (mode == IVS021.ScreenMode.Register) {
                IVS021.jQuery.SearchSection.divSearchSection().show();
                IVS021.jQuery.SearchSection.divSearchResult().show();
                IVS021.jQuery.RegisterSection.divRegisterSection().show();
                IVS021.jQuery.ShowSlipSection.divShowSlipSection().hide();

                register_command.SetCommand(IVS021.EventHandlers.cmdRegister_Click);
                reset_command.SetCommand(IVS021.EventHandlers.cmdReset_Click);
                confirm_command.SetCommand(null);
                back_command.SetCommand(null);

                IVS021.jQuery.RegisterSection.divRegisterSection().SetViewMode(false);

            } else if (mode == IVS021.ScreenMode.Confirm) {
                IVS021.jQuery.SearchSection.divSearchSection().hide();
                IVS021.jQuery.SearchSection.divSearchResult().hide();
                IVS021.jQuery.RegisterSection.divRegisterSection().show();
                IVS021.jQuery.ShowSlipSection.divShowSlipSection().hide();

                register_command.SetCommand(null);
                reset_command.SetCommand(null);
                confirm_command.SetCommand(IVS021.EventHandlers.cmdConfirm_Click);
                back_command.SetCommand(IVS021.EventHandlers.cmdBack_Click);

                IVS021.jQuery.RegisterSection.divRegisterSection().SetViewMode(true);

            } else if (mode == IVS021.ScreenMode.ShowSlipNo) {
                IVS021.jQuery.SearchSection.divSearchSection().hide();
                IVS021.jQuery.SearchSection.divSearchResult().hide();
                IVS021.jQuery.RegisterSection.divRegisterSection().show();
                IVS021.jQuery.ShowSlipSection.divShowSlipSection().show();

                IVS021.jQuery.ShowSlipSection.txtShowSlipNo().SetReadOnly(true);

                register_command.SetCommand(null);
                reset_command.SetCommand(null);
                confirm_command.SetCommand(null);
                back_command.SetCommand(null);
            }
        },

        SetDisabledSearchSection: function (isDisabled) {
            IVS021.jQuery.SearchSection.txtSearchInstallationSlipNo().SetDisabled(isDisabled);
            IVS021.jQuery.SearchSection.txtSearchContractCode().SetDisabled(isDisabled);
            IVS021.jQuery.SearchSection.txtSearchContractTargetName().SetDisabled(isDisabled);
            IVS021.jQuery.SearchSection.txtSearchSiteName().SetDisabled(isDisabled);
            IVS021.jQuery.SearchSection.txtSearchExpectedStockOutDateFrom().SetDisabled(isDisabled);
            IVS021.jQuery.SearchSection.txtSearchExpectedStockOutDateFrom().EnableDatePicker(!isDisabled);
            IVS021.jQuery.SearchSection.txtSearchExpectedStockOutDateTo().SetDisabled(isDisabled);
            IVS021.jQuery.SearchSection.txtSearchExpectedStockOutDateTo().EnableDatePicker(!isDisabled);
            IVS021.jQuery.SearchSection.cboSearchInstallationType().SetDisabled(isDisabled);
            IVS021.jQuery.SearchSection.btnClear().SetDisabled(isDisabled);
            IVS021.jQuery.SearchSection.btnSearch().SetDisabled(isDisabled);
            if (isDisabled) {
                GridControl.LockGrid(IVS021.Grids.grdSearch);
            } else {
                GridControl.UnlockGrid(IVS021.Grids.grdSearch);
            }
            GridControl.SetDisabledButtonOnGrid(IVS021.Grids.grdSearch, IVS021.GridColumnID.SearchResult.BtnEdit, IVS021.GridColumnID.SearchResult.BtnEdit, !isDisabled);
        },

        GetInstallDetailForRegister: function () {
            if (IVS021.Grids.grdRegister == undefined) {
                return null;
            }

            var grid = IVS021.Grids.grdRegister;
            var arrTmp = new Array();

            if (!CheckFirstRowIsEmpty(grid)) {
                for (var i = 0; i < grid.getRowsNum(); i++) {
                    if (grid.cells2(i, grid.getColIndexById(IVS021.GridColumnID.Register.InstrumentCode)).getAttribute("child-main")) {
                        var row_id = grid.getRowId(i);
                        var txtNewInstSaleID = GenerateGridControlID(IVS021.GridColumnID.Register.NewInstSaleID, row_id);
                        var txtNewInstSampleID = GenerateGridControlID(IVS021.GridColumnID.Register.NewInstSampleID, row_id);
                        var txtNewInstRentalID = GenerateGridControlID(IVS021.GridColumnID.Register.NewInstRentalID, row_id);
                        var txtSecondhandInstRentalID = GenerateGridControlID(IVS021.GridColumnID.Register.SecondhandInstRentalID, row_id);
                        var txtRemarkID = GenerateGridControlID(IVS021.GridColumnID.Register.RemarkID, row_id);

                        var strSlipDtl = grid.cells2(i, grid.getColIndexById(IVS021.GridColumnID.Register.ToJson)).getValue();
                        var objSlipDtl = JSON.parse(htmlDecode(strSlipDtl));

                        var obj = {
                            SlipNo: objSlipDtl.SlipNo,
                            ParentInstrumentCode: objSlipDtl.ParentInstrumentCode,
                            ParentInstrumentName: objSlipDtl.ParentInstrumentName,
                            ChildInstrumentCode: objSlipDtl.ChildInstrumentCode,
                            ChildInstrumentName: objSlipDtl.ChildInstrumentName,
                            AddInstalledQty: objSlipDtl.AddInstalledQty,
                            RemainQty: objSlipDtl.RemainQty,
                            TotalStockOutQty: objSlipDtl.TotalStockOutQty,
                            NewInstSale: $("#" + txtNewInstSaleID).NumericValue(),
                            NewInstSample: $("#" + txtNewInstSampleID).NumericValue(),
                            NewInstRental: $("#" + txtNewInstRentalID).NumericValue(),
                            SecondhandInstRental: $("#" + txtSecondhandInstRentalID).NumericValue(),
                            SaleShelfNo: objSlipDtl.SaleShelfNo,
                            RentalShelfNo: objSlipDtl.RentalShelfNo,
                            SampleShelfNo: objSlipDtl.SampleShelfNo,
                            SecondShelfNo: objSlipDtl.SecondShelfNo,
                            NewInstSaleCtrlID: txtNewInstSaleID,
                            NewInstSampleCtrlID: txtNewInstSampleID,
                            NewInstRentalCtrlID: txtNewInstRentalID,
                            SecondhandInstRentalCtrlID: txtSecondhandInstRentalID,
                            Remark: $("#" + txtRemarkID).val(),
                            RemarkCtrlID: txtRemarkID
                        };
                        arrTmp.push(obj);
                    }
                }
            }

            if (arrTmp.length > 0)
                return arrTmp;
            else
                return null;
        },

        LoadDataToDetailGrid: function (data) {
            var grid = IVS021.Grids.grdRegister;

            DeleteAllRow(grid);

            if (data == null || data == undefined || data.length == 0) {
                return;
            }

            CheckFirstRowIsEmpty(grid, true);

            var lastParentInstCode = null;
            for (var i = 0; i < data.length; i++) {
                var row_id = null;
                var instrument_row_id = null;
                var idxInstrumentCode = grid.getColIndexById(IVS021.GridColumnID.Register.InstrumentCode);

                //Add by Jutarat A. on 28112013
                data[i].ParentInstrumentCode = ConvertBlockHtml(data[i].ParentInstrumentCode);
                data[i].ParentInstrumentName = ConvertBlockHtml(data[i].ParentInstrumentName);
                data[i].ChildInstrumentCode = ConvertBlockHtml(data[i].ChildInstrumentCode);
                data[i].ChildInstrumentName = ConvertBlockHtml(data[i].ChildInstrumentName);
                //End Add

                if (data[i].ParentInstrumentCode != null && lastParentInstCode != data[i].ParentInstrumentCode) {
                    row_id = grid.uid();
                    grid.addRow(
                        row_id
                        , IVS021.Functions.grdRegister_NewData(data[i].ParentInstrumentCode + " : " + data[i].ParentInstrumentName)
                        , grid.getRowsNum()
                    );
                    grid.setColspan(row_id, idxInstrumentCode, 8);
                    grid.setCellTextStyle(row_id, idxInstrumentCode, 'text-align:left');
                    grid.cells(row_id, idxInstrumentCode).setAttribute("parent-header", true);
                    grid.setRowColor(row_id, IVS021.Colors.NormalRow)
                    lastParentInstCode = data[i].ParentInstrumentCode;
                }

                row_id = instrument_row_id = grid.uid();
                grid.addRow(row_id
                    , IVS021.Functions.grdRegister_NewData(
                        (data[i].ParentInstrumentCode != null ? "+ " : "") + data[i].ChildInstrumentCode,
                        data[i].ChildInstrumentName,
                        data[i].AddInstalledQty,
                        data[i].RemainQty,
                        "",
                        "",
                        "",
                        "",
                        JSON.parse(htmlDecode(data[i].ToJson))
                    )
                    , grid.getRowsNum()
                );
                grid.cells(row_id, idxInstrumentCode).setAttribute("child-main", true);

                if (data[i].ParentInstrumentCode == null) {
                    grid.setRowColor(row_id, IVS021.Colors.NormalRow);
                }
                else {
                    grid.setRowColor(row_id, IVS021.Colors.ChildRow);
                }

                var enableNewInstSale = false;
                var enableNewInstSample = false;
                var enableNewInstRental = false;
                var enableSecondhandInstRental = false;

                if (data[i].RemainQty == 0) {

                    enableNewInstSale = false;
                    enableNewInstSample = false;
                    enableNewInstRental = false;
                    enableSecondhandInstRental = false;

                } else if (IVS021.Vars.objEditingSlip.InstallationType == IVS021_InstallType.C_SALE_INSTALL_TYPE_NEW
                    || IVS021.Vars.objEditingSlip.InstallationType == IVS021_InstallType.C_SALE_INSTALL_TYPE_ADD
                    || IVS021.Vars.objEditingSlip.InstallationType == IVS021_InstallType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                ) {

                    enableNewInstSale = true;
                    enableNewInstSample = true;

                } else if (IVS021.Vars.objEditingSlip.InstallationType == IVS021_InstallType.C_RENTAL_INSTALL_TYPE_NEW
                    || IVS021.Vars.objEditingSlip.InstallationType == IVS021_InstallType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW
                    || IVS021.Vars.objEditingSlip.InstallationType == IVS021_InstallType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW
                    || IVS021.Vars.objEditingSlip.InstallationType == IVS021_InstallType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                    || IVS021.Vars.objEditingSlip.InstallationType == IVS021_InstallType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                ) {

                    enableNewInstSample = true;
                    enableNewInstRental = true;
                    enableSecondhandInstRental = true;

                }

                GenerateNumericBox2(grid, IVS021.GridColumnID.Register.NewInstSaleID,
                    row_id, IVS021.GridColumnID.Register.NewInstSale,
                    "", 5, 0, 0, 99999, false, enableNewInstSale);

                GenerateNumericBox2(grid, IVS021.GridColumnID.Register.NewInstSampleID,
                    row_id, IVS021.GridColumnID.Register.NewInstSample,
                    "", 5, 0, 0, 99999, false, enableNewInstSample);

                GenerateNumericBox2(grid, IVS021.GridColumnID.Register.NewInstRentalID,
                    row_id, IVS021.GridColumnID.Register.NewInstRental,
                    "", 5, 0, 0, 99999, false, enableNewInstRental);

                GenerateNumericBox2(grid, IVS021.GridColumnID.Register.SecondhandInstRentalID,
                    row_id, IVS021.GridColumnID.Register.SecondhandInstRental,
                    "", 5, 0, 0, 99999, false, enableSecondhandInstRental);

                // Add 2nd row of detail for S/N input.
                row_id = grid.uid();
                grid.addRow(row_id
                    , IVS021.Functions.grdRegister_NewData(
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        JSON.parse(htmlDecode(data[i].ToJson))
                    )
                    , grid.getRowsNum()
                );

                grid.cells(instrument_row_id, idxInstrumentCode).setAttribute("child-row-id", row_id);

                grid.setColspan(row_id, idxInstrumentCode, 8);
                grid.setCellTextStyle(row_id, idxInstrumentCode, 'text-align:right');
                grid.cells(row_id, idxInstrumentCode).setAttribute("child-remark", true);

                var idRemark = GenerateGridControlID(IVS021.GridColumnID.Register.RemarkID, instrument_row_id);
                var txtRemark = $("<textarea></textarea>")
                    .attr("id", idRemark)
                    .attr("row_id", row_id)
                    .attr("name", IVS021.GridColumnID.Register.Remark)
                    .attr("maxlength", 150)
                    .css("width", "542px")
                    .css("height", "70px")
                    .css("margin-right", "-2px")
                    .css("overflow", "auto")
                    .css("word-wrap", "normal")
                    .css("white-space", "pre");

                var divSN = $("<div></div>")
                    .attr("class", "usr-label")
                    .css("width", "50px")
                    .append("S/N ");
                var divRemark = $("<div></div>")
                    .attr("class", "usr-object")
                    .css("width", "550px")
                    .css("text-align", "left")
                    .append(txtRemark);
                var divRemarkRow = $("<div></div>")
                    .attr("class", "usr-row")
                    .css("padding-left", "340px")
                    .append(divSN)
                    .append(divRemark);
                grid.cells(row_id, idxInstrumentCode).setValue($("<p>").append(divRemarkRow).html());

                $("#" + idRemark).SetMaxLengthTextArea(150, 3);

                $("tr[idd=" + row_id + "]").css("height", "83px");
                grid.setRowHidden(row_id, true);

                if (data[i].ParentInstrumentCode == null) {
                    grid.setRowColor(row_id, IVS021.Colors.NormalRow);
                }
                else {
                    grid.setRowColor(row_id, IVS021.Colors.ChildRow);
                }
            }

            $("#divRegisterGrid textarea[name=Remark]")
                .unbind("focus")
                .focus(function () {
                    IVS021.Grids.grdRegister.selectRowById($(this).attr("row_id"));
                })
                .change(function () {
                    var row_id = $(this).attr("row_id");
                    var idxInstrumentCode = grid.getColIndexById(IVS021.GridColumnID.Register.InstrumentCode);
                    IVS021.Grids.grdRegister.cells(row_id, idxInstrumentCode).setAttribute("title", $(this).val());
                });

            $("#lnkShowSN")
                .unbind("click")
                .click(function () {
                    $("textarea[name=" + IVS021.GridColumnID.Register.Remark + "]").each(function () {
                        var row_id = $(this).attr("row_id");
                        IVS021.Grids.grdRegister.setRowHidden(row_id, false);
                    });
                    IVS021.Grids.grdRegister.setSizes();
                    return false;
                });

            $("#lnkHideSN")
                .unbind("click")
                .click(function () {
                    $("textarea[name=" + IVS021.GridColumnID.Register.Remark + "]").each(function () {
                        var row_id = $(this).attr("row_id");
                        IVS021.Grids.grdRegister.setRowHidden(row_id, true);
                    });
                    IVS021.Grids.grdRegister.setSizes();
                    return false;
                });

            grid.setSizes();
        },

        grdRegister_NewData: function (InstrumentCode, InstrumentName, AddInstalledQty, RemainQty, NewInstSale, NewInstSample, NewInstRental, SecondhandInstRental, ToJson) {
            return [
                "",
                String(InstrumentCode),
                String(InstrumentName),
                String(AddInstalledQty),
                String(RemainQty),
                String(NewInstSale),
                String(NewInstSample),
                String(NewInstRental),
                String(SecondhandInstRental),
                JSON.stringify(ToJson)
            ];
        }

    }

}

$(document).ready(function () {
    IVS021.Functions.InitializeGrid();
    IVS021.Functions.InitializeScreen();
    IVS021.Functions.SetScreenMode(IVS021.ScreenMode.Search);
});
