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

var IVS020 = {

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
            ProjectCode: "ProjectCode",
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
        objEditingSlip: null,
        ProjectStatus: null
    },

    jQuery: {
        SearchSection: {
            divSearchSection: function () { return $("#divSearchInstallSlip"); },
            divSearchCriteria: function () { return $("#divSearchInstallSlipCriteria"); },
            divSearchResult: function () { return $("#divSearchInstallSlipGrid"); },
            txtSearchInstallationSlipNo: function () { return $("#txtSearchInstallationSlipNo"); },
            txtSearchProjectCode: function () { return $("#txtSearchProjectCode"); },
            txtSearchContractCode: function () { return $("#txtSearchContractCode"); },
            txtSearchContractTargetName: function () { return $("#txtSearchContractTargetName"); },
            txtSearchSiteName: function () { return $("#txtSearchSiteName"); },
            txtSearchExpectedStockOutDateFrom: function () { return $("#txtSearchExpectedStockOutDateFrom"); },
            txtSearchExpectedStockOutDateTo: function () { return $("#txtSearchExpectedStockOutDateTo"); },
            cboSearchInventoryOffice: function () { return $("#cboSearchInventoryOffice"); },
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
            txtRegProjectCode: function () { return $("#txtRegProjectCode"); },
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

            IVS020.jQuery.SearchSection.btnSearch().attr("disabled", true);
            master_event.LockWindow(true);

            //            var params = CreateObjectData($("#formSearch").serialize(), true);
            var params = $("#formSearch").serializeObject2();

            /// Convert OfficeCode to Array
            params.OfficeCodeList = [];
            IVS020.jQuery.SearchSection.cboSearchInventoryOffice().find("option").each(function (index) {
                if (index > 0) {
                    params.OfficeCodeList.push($(this).val());
                }
            });

            IVS020.jQuery.SearchSection.divSearchInstallSlipGrid().LoadDataToGrid(
                IVS020.Grids.grdSearch, ROWS_PER_PAGE_FOR_SEARCHPAGE
                , false, "/inventory/IVS020_SearchInstallationSlipForStockOut"
                , params, "doResultInstallationSlipForStockOut", false
                , function () {
                    IVS020.jQuery.SearchSection.btnSearch().attr("disabled", false);
                    master_event.LockWindow(false);
                    //IVS020.jQuery.SearchSection.divSearchSection().each(function () {
                    //    this.scrollIntoView();
                    //});
                    master_event.ScrollWindow("#divSearchInstallSlipGrid");
                }
                , null
            );

            return false;
        },

        btnClear_click: function () {
            IVS020.jQuery.SearchSection.divSearchCriteria().clearForm();
            IVS020.jQuery.SearchSection.divSearchResult().clearForm();

            ClearDateFromToControl("#txtSearchExpectedStockOutDateFrom", "#txtSearchExpectedStockOutDateTo");
            //SetDateFromToData("#txtSearchExpectedStockOutDateFrom", "#txtSearchExpectedStockOutDateTo", new Date(), new Date());

            DeleteAllRow(IVS020.Grids.grdSearch);

            return false;
        },

        btnEditSlip_click: function (grid, rid) {
            var row_index = grid.getRowIndex(rid);
            grid.selectRow(row_index);

            IVS020.Functions.SetScreenMode(IVS020.ScreenMode.Register);
            var strSlipNo = grid.cells2(row_index, grid.getColIndexById(IVS020.GridColumnID.SearchResult.SlipNo)).getValue();

            GridControl.LockGrid(grid);
            GridControl.SetDisabledButtonOnGrid(grid, IVS020.GridColumnID.SearchResult.BtnEdit, IVS020.GridColumnID.SearchResult.BtnEdit, true);

            var strSlipData = grid.cells2(row_index, grid.getColIndexById("ToJson")).getValue();
            var objSlipData = JSON.parse(htmlDecode(strSlipData));
            IVS020.jQuery.RegisterSection.txtRegContractCode().val(objSlipData.ContractCodeShort);
            IVS020.jQuery.RegisterSection.txtRegProjectCode().val(objSlipData.ProjectCode);
            IVS020.jQuery.RegisterSection.txtRegInstallSlipNo().val(objSlipData.SlipNo);
            IVS020.jQuery.RegisterSection.txtRegInstallType().val(objSlipData.InstallationTypeName);
            IVS020.jQuery.RegisterSection.txtRegContractTargetNameEN().val(objSlipData.CustFullNameEN);
            IVS020.jQuery.RegisterSection.txtRegContractTargetNameLC().val(objSlipData.CustFullNameLC);
            IVS020.jQuery.RegisterSection.txtRegSiteNameEN().val(objSlipData.SiteNameEN);
            IVS020.jQuery.RegisterSection.txtRegSiteNameLC().val(objSlipData.SiteNameLC);
            IVS020.jQuery.RegisterSection.txtRegInventoryOffice().val(objSlipData.OfficeName);
            IVS020.jQuery.RegisterSection.txtRegExpectedOutDate().val(objSlipData.SlipIssueDateDisplay);
            IVS020.jQuery.RegisterSection.txtRegMemo().val("");
            IVS020.jQuery.RegisterSection.txtRegStockOutDate().SetDate(new Date);

            IVS020.Vars.objEditingSlip = objSlipData;

            DeleteAllRow(IVS020.Grids.grdRegister);

            var params = {
                SlipNo: strSlipNo,
                OfficeCode: objSlipData.OfficeCode,
                strProjectCode: objSlipData.ProjectCode
            };

            //            IVS020.jQuery.RegisterSection.divRegisterGrid().LoadDataToGrid(
            //                IVS020.Grids.grdRegister, 0
            //                , false, "/inventory/IVS020_SearchInstallationDetailForStockOut"
            //                , params, "doResultInstallationDetailForStockOut", false
            //                , function () {
            //                    GridControl.UnlockGrid(IVS020.Grids.grdSearch);
            //                    GridControl.SetDisabledButtonOnGrid(IVS020.Grids.grdSearch, IVS020.GridColumnID.SearchResult.BtnEdit, IVS020.GridColumnID.SearchResult.BtnEdit, false);
            //                }
            //                , null
            //            );

            master_event.ScrollWindow("#divRegisterStockOut");

            ajax_method.CallScreenController("/inventory/IVS020_SearchInstallationDetailForStockOut", params, function (result, controls) {
                if (result != null) {
                    IVS020.Functions.LoadDataToDetailGrid(result);
                    GridControl.UnlockGrid(IVS020.Grids.grdSearch);
                    GridControl.SetDisabledButtonOnGrid(IVS020.Grids.grdSearch, IVS020.GridColumnID.SearchResult.BtnEditID, IVS020.GridColumnID.SearchResult.BtnEdit, false);
                }
            });
        },

        cmdRegister_Click: function () {
            var params = {
                header: IVS020.Vars.objEditingSlip,
                details: IVS020.Functions.GetInstallDetailForRegister(),
                Memo: IVS020.jQuery.RegisterSection.txtRegMemo().val(),
                StockOutDate: IVS020.jQuery.RegisterSection.txtRegStockOutDate().val()
            };

            if (IVS020.Vars.ProjectStatus) {
                params.header.ProjectCode = null;
            }

            ajax_method.CallScreenController("/inventory/IVS020_RegisterInstallationForStockOut", params, function (result, controls) {

                IVS020.jQuery.RegisterSection.divRegisterGrid().find(".highlight").toggleClass("highlight", false);
                VaridateCtrl(controls, controls);

                if (result != null && result.IsSuccess) {
                    IVS020.Functions.SetScreenMode(IVS020.ScreenMode.Confirm);
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
            //        IVS020.Functions.SetScreenMode(IVS020.ScreenMode.Search);
            //        IVS020.EventHandlers.btnClear_click();
            //    }, null);
            //});
            IVS020.Functions.SetScreenMode(IVS020.ScreenMode.Search);
            IVS020.EventHandlers.btnClear_click();
        },

        btnNewRegister_click: function () {
            IVS020.Functions.SetScreenMode(IVS020.ScreenMode.Search);
            IVS020.EventHandlers.btnClear_click();
        },

        btnDownloadSlip_click: function () {
            IVS020.jQuery.ShowSlipSection.btnDownloadSlip().SetDisabled(true);
            master_event.LockWindow(true);

            var param = {
                strInvSlipNo: IVS020.jQuery.ShowSlipSection.txtShowSlipNo().val()
            };

            ajax_method.CallScreenController("/inventory/IVS020_DownloadDocument", param, function (result, controls) {
                if (result != null) {
                    var key = ajax_method.GetKeyURL(null);
                    var url = ajax_method.GenerateURL("/inventory/IVS020_DownloadPdfAndWriteLog?k=" + key + "&strDocumentNo=" + result.DocumentNo + "&documentOCC=" + result.DocumentOCC + "&strDocumentCode=" + result.DocumentCode + "&fileName=" + result.FilePath)
                    window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
                }
                IVS020.jQuery.ShowSlipSection.btnDownloadSlip().SetDisabled(false);
                master_event.LockWindow(false);
            });
        },

        cmdBack_Click: function () {
            IVS020.Functions.SetScreenMode(IVS020.ScreenMode.Register);
        },

        cmdConfirm_Click: function () {
            var params = {
                header: IVS020.Vars.objEditingSlip,
                details: IVS020.Functions.GetInstallDetailForRegister(),
                Memo: IVS020.jQuery.RegisterSection.txtRegMemo().val(),
                ProjectStatus: IVS020.Vars.ProjectStatus,
                StockOutDate: IVS020.jQuery.RegisterSection.txtRegStockOutDate().val()
            };

            if (IVS020.Vars.ProjectStatus) {
                params.header.ProjectCode = null;
            }

            ajax_method.CallScreenController("/inventory/IVS020_ConfirmInstallationForStockOut", params, function (result, controls) {

                IVS020.jQuery.RegisterSection.divRegisterGrid().find(".highlight").toggleClass("highlight", false);
                VaridateCtrl(controls, controls);

                if (result != null && result.IsSuccess) {
                    IVS020.Functions.SetScreenMode(IVS020.ScreenMode.ShowSlipNo);

                    if (IVS020.Vars.objEditingSlip.ProjectCode == "" || IVS020.Vars.objEditingSlip.ProjectCode == null) {
                        IVS020.jQuery.ShowSlipSection.btnDownloadSlip().SetDisabled(true);
                    } else {
                        IVS020.jQuery.ShowSlipSection.btnDownloadSlip().SetDisabled(false);
                    }
                    IVS020.jQuery.ShowSlipSection.txtShowSlipNo().val(result.InvSlipNo);

                    master_event.ScrollWindow("#divShowSlip");
                }
                //else {
                //    IVS020.Functions.SetScreenMode(IVS020.ScreenMode.Register);
                //}
            });
        },

        grdSearch_OnLoadedData: function (gen_ctrl) {
            for (var i = 0; i < IVS020.Grids.grdSearch.getRowsNum(); i++) {
                var row_id = IVS020.Grids.grdSearch.getRowId(i);
                if (gen_ctrl) {
                    //GenerateEditButton(IVS020.Grids.grdSearch, IVS020.GridColumnID.SearchResult.BtnEditID, row_id, IVS020.GridColumnID.SearchResult.BtnEdit, true);
                    GenerateSelectButton(IVS020.Grids.grdSearch, IVS020.GridColumnID.SearchResult.BtnEditID, row_id, IVS020.GridColumnID.SearchResult.BtnEdit, true);
                }
                BindGridButtonClickEvent(IVS020.GridColumnID.SearchResult.BtnEditID, row_id, function (rid) {
                    IVS020.EventHandlers.btnEditSlip_click(IVS020.Grids.grdSearch, rid);
                });
            }

            IVS020.Grids.grdSearch.setSizes();
        }

    },

    Colors: {
        NormalRow: "#FFFFFF",
        ChildRow: "#E3EFFF"
    },

    Functions: {
        InitializeGrid: function () {
            IVS020.Grids.grdSearch = IVS020.jQuery.SearchSection.divSearchInstallSlipGrid().InitialGrid(
                ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS020_InitialSearchGrid"
                , function () {
                    BindOnLoadedEvent(IVS020.Grids.grdSearch, IVS020.EventHandlers.grdSearch_OnLoadedData);
                }
            );

            SpecialGridControl(IVS020.Grids.grdSearch, [IVS020.GridColumnID.SearchResult.BtnEdit]);

            IVS020.Grids.grdRegister = IVS020.jQuery.RegisterSection.divRegisterGrid().InitialGrid(
                0, false, "/inventory/IVS020_InitialRegisterGrid");

            IVS020.Grids.grdRegister.attachEvent("onSelectionChange", function (id, ind) {
                return true;
            });

            SpecialGridControl(IVS020.Grids.grdRegister, [
                IVS020.GridColumnID.Register.NewInstSale,
                IVS020.GridColumnID.Register.NewInstSample,
                IVS020.GridColumnID.Register.NewInstRental,
                IVS020.GridColumnID.Register.SecondhandInstRental
            ]);
        },

        InitializeScreen: function () {
            ClearDateFromToControl("#txtSearchExpectedStockOutDateFrom", "#txtSearchExpectedStockOutDateTo");
            InitialDateFromToControl("#txtSearchExpectedStockOutDateFrom", "#txtSearchExpectedStockOutDateTo");
            //SetDateFromToData("#txtSearchExpectedStockOutDateFrom", "#txtSearchExpectedStockOutDateTo", new Date(), new Date());

            IVS020.jQuery.SearchSection.btnSearch().click(IVS020.EventHandlers.btnSearch_click);
            IVS020.jQuery.SearchSection.btnClear().click(IVS020.EventHandlers.btnClear_click);
            IVS020.jQuery.ShowSlipSection.btnNewRegister().click(IVS020.EventHandlers.btnNewRegister_click);
            IVS020.jQuery.ShowSlipSection.btnDownloadSlip().click(IVS020.EventHandlers.btnDownloadSlip_click);

            IVS020.jQuery.SearchSection.divSearchSection().css("visibility", "inherit");
            IVS020.jQuery.RegisterSection.divRegisterSection().css("visibility", "inherit");
            IVS020.jQuery.ShowSlipSection.divShowSlipSection().css("visibility", "inherit");

            IVS020.jQuery.SearchSection.txtSearchInstallationSlipNo().attr("name", "InstallationSlipNo");
            IVS020.jQuery.SearchSection.txtSearchProjectCode().attr("name", "ProjectCode");
            IVS020.jQuery.SearchSection.txtSearchContractCode().attr("name", "ContractCode");
            IVS020.jQuery.SearchSection.txtSearchContractTargetName().attr("name", "ContractTargetName");
            IVS020.jQuery.SearchSection.txtSearchSiteName().attr("name", "SiteName");
            IVS020.jQuery.SearchSection.cboSearchInventoryOffice().attr("name", "OfficeCode");
            IVS020.jQuery.SearchSection.cboSearchInstallationType().attr("name", "InstallationTypeCode");
            IVS020.jQuery.SearchSection.txtSearchExpectedStockOutDateFrom().attr("name", "ExpectedStockOutDateFrom");
            IVS020.jQuery.SearchSection.txtSearchExpectedStockOutDateTo().attr("name", "ExpectedStockOutDateTo");

            IVS020.jQuery.RegisterSection.txtRegContractCode().SetReadOnly(true);
            IVS020.jQuery.RegisterSection.txtRegProjectCode().SetReadOnly(true);
            IVS020.jQuery.RegisterSection.txtRegInstallSlipNo().SetReadOnly(true);
            IVS020.jQuery.RegisterSection.txtRegInstallType().SetReadOnly(true);
            IVS020.jQuery.RegisterSection.txtRegContractTargetNameEN().SetReadOnly(true);
            IVS020.jQuery.RegisterSection.txtRegContractTargetNameLC().SetReadOnly(true);
            IVS020.jQuery.RegisterSection.txtRegSiteNameEN().SetReadOnly(true);
            IVS020.jQuery.RegisterSection.txtRegSiteNameLC().SetReadOnly(true);
            IVS020.jQuery.RegisterSection.txtRegInventoryOffice().SetReadOnly(true);
            IVS020.jQuery.RegisterSection.txtRegExpectedOutDate().SetReadOnly(true);

            IVS020.jQuery.ShowSlipSection.txtShowSlipNo().SetReadOnly(true);

            IVS020.jQuery.RegisterSection.txtRegMemo().SetMaxLengthTextArea(1000, 3);
            IVS020.jQuery.RegisterSection.txtRegStockOutDate().InitialDate();
            IVS020.jQuery.RegisterSection.txtRegStockOutDate().SetMinDate(IVS020_Constant.MINDATE);
            IVS020.jQuery.RegisterSection.txtRegStockOutDate().SetMaxDate(IVS020_Constant.MAXDATE);
        },

        SetScreenMode: function (mode) {
            if (mode == IVS020.ScreenMode.Search) {
                IVS020.jQuery.SearchSection.divSearchSection().show();
                IVS020.jQuery.SearchSection.divSearchResult().show();
                IVS020.jQuery.RegisterSection.divRegisterSection().hide();
                IVS020.jQuery.ShowSlipSection.divShowSlipSection().hide();

                register_command.SetCommand(null);
                reset_command.SetCommand(null);
                confirm_command.SetCommand(null);
                back_command.SetCommand(null);

            } else if (mode == IVS020.ScreenMode.Register) {
                IVS020.jQuery.SearchSection.divSearchSection().show();
                IVS020.jQuery.SearchSection.divSearchResult().show();
                IVS020.jQuery.RegisterSection.divRegisterSection().show();
                IVS020.jQuery.ShowSlipSection.divShowSlipSection().hide();

                register_command.SetCommand(IVS020.EventHandlers.cmdRegister_Click);
                reset_command.SetCommand(IVS020.EventHandlers.cmdReset_Click);
                confirm_command.SetCommand(null);
                back_command.SetCommand(null);

                IVS020.jQuery.RegisterSection.divRegisterSection().SetViewMode(false);

            } else if (mode == IVS020.ScreenMode.Confirm) {
                IVS020.jQuery.SearchSection.divSearchSection().hide();
                IVS020.jQuery.SearchSection.divSearchResult().hide();
                IVS020.jQuery.RegisterSection.divRegisterSection().show();
                IVS020.jQuery.ShowSlipSection.divShowSlipSection().hide();

                register_command.SetCommand(null);
                reset_command.SetCommand(null);
                confirm_command.SetCommand(IVS020.EventHandlers.cmdConfirm_Click);
                back_command.SetCommand(IVS020.EventHandlers.cmdBack_Click);

                IVS020.jQuery.RegisterSection.divRegisterSection().SetViewMode(true);

            } else if (mode == IVS020.ScreenMode.ShowSlipNo) {
                IVS020.jQuery.SearchSection.divSearchSection().hide();
                IVS020.jQuery.SearchSection.divSearchResult().hide();
                IVS020.jQuery.RegisterSection.divRegisterSection().show();
                IVS020.jQuery.ShowSlipSection.divShowSlipSection().show();

                IVS020.jQuery.ShowSlipSection.txtShowSlipNo().SetReadOnly(true);

                register_command.SetCommand(null);
                reset_command.SetCommand(null);
                confirm_command.SetCommand(null);
                back_command.SetCommand(null);
            }
        },

        SetDisabledSearchSection: function (isDisabled) {
            IVS020.jQuery.SearchSection.txtSearchInstallationSlipNo().SetDisabled(isDisabled);
            IVS020.jQuery.SearchSection.txtSearchProjectCode().SetDisabled(isDisabled);
            IVS020.jQuery.SearchSection.txtSearchContractCode().SetDisabled(isDisabled);
            IVS020.jQuery.SearchSection.txtSearchContractTargetName().SetDisabled(isDisabled);
            IVS020.jQuery.SearchSection.txtSearchSiteName().SetDisabled(isDisabled);
            IVS020.jQuery.SearchSection.txtSearchExpectedStockOutDateFrom().SetDisabled(isDisabled);
            IVS020.jQuery.SearchSection.txtSearchExpectedStockOutDateFrom().EnableDatePicker(!isDisabled);
            IVS020.jQuery.SearchSection.txtSearchExpectedStockOutDateTo().SetDisabled(isDisabled);
            IVS020.jQuery.SearchSection.txtSearchExpectedStockOutDateTo().EnableDatePicker(!isDisabled);
            IVS020.jQuery.SearchSection.cboSearchInventoryOffice().SetDisabled(isDisabled);
            IVS020.jQuery.SearchSection.cboSearchInstallationType().SetDisabled(isDisabled);
            IVS020.jQuery.SearchSection.btnClear().SetDisabled(isDisabled);
            IVS020.jQuery.SearchSection.btnSearch().SetDisabled(isDisabled);
            if (isDisabled) {
                GridControl.LockGrid(IVS020.Grids.grdSearch);
            } else {
                GridControl.UnlockGrid(IVS020.Grids.grdSearch);
            }
            GridControl.SetDisabledButtonOnGrid(IVS020.Grids.grdSearch, IVS020.GridColumnID.SearchResult.BtnEdit, IVS020.GridColumnID.SearchResult.BtnEdit, !isDisabled);
        },

        GetInstallDetailForRegister: function () {
            if (IVS020.Grids.grdRegister == undefined) {
                return null;
            }

            var grid = IVS020.Grids.grdRegister;
            var arrTmp = new Array();

            if (!CheckFirstRowIsEmpty(grid)) {
                for (var i = 0; i < grid.getRowsNum(); i++) {
                    if (grid.cells2(i, grid.getColIndexById(IVS020.GridColumnID.Register.InstrumentCode)).getAttribute("child-main")) {
                        var row_id = grid.getRowId(i);
                        var txtNewInstSaleID = GenerateGridControlID(IVS020.GridColumnID.Register.NewInstSaleID, row_id);
                        var txtNewInstSampleID = GenerateGridControlID(IVS020.GridColumnID.Register.NewInstSampleID, row_id);
                        var txtNewInstRentalID = GenerateGridControlID(IVS020.GridColumnID.Register.NewInstRentalID, row_id);
                        var txtSecondhandInstRentalID = GenerateGridControlID(IVS020.GridColumnID.Register.SecondhandInstRentalID, row_id);
                        var txtRemarkID = GenerateGridControlID(IVS020.GridColumnID.Register.RemarkID, row_id);

                        var strSlipDtl = grid.cells2(i, grid.getColIndexById(IVS020.GridColumnID.Register.ToJson)).getValue();
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

        LoadDataToDetailGrid: function (result) {
            var grid = IVS020.Grids.grdRegister;
            var data = result.ResultData;

            DeleteAllRow(grid);

            if (data == null || data == undefined || data.length == 0) {
                return;
            }

            CheckFirstRowIsEmpty(grid, true);

            var lastParentInstCode = null;
            for (var i = 0; i < data.length; i++) {
                var row_id = null;
                var instrument_row_id = null;
                var idxInstrumentCode = grid.getColIndexById(IVS020.GridColumnID.Register.InstrumentCode);

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
                        , IVS020.Functions.grdRegister_NewData(data[i].ParentInstrumentCode + " : " + data[i].ParentInstrumentName)
                        , grid.getRowsNum()
                    );
                    grid.setColspan(row_id, idxInstrumentCode, 7);
                    grid.setCellTextStyle(row_id, idxInstrumentCode, 'text-align:left');
                    grid.cells(row_id, idxInstrumentCode).setAttribute("parent-header", true);
                    grid.setRowColor(row_id, IVS020.Colors.NormalRow)
                    lastParentInstCode = data[i].ParentInstrumentCode;
                }

                row_id = instrument_row_id = grid.uid();
                grid.addRow(row_id
                    , IVS020.Functions.grdRegister_NewData(
                        (data[i].ParentInstrumentCode != null ? "+ " : "") + data[i].ChildInstrumentCode,
                        data[i].ChildInstrumentName,
                        data[i].AddInstalledQty,
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
                    grid.setRowColor(row_id, IVS020.Colors.NormalRow);
                }
                else {
                    grid.setRowColor(row_id, IVS020.Colors.ChildRow);
                }

                var enableNewInstSale = false;
                var enableNewInstSample = false;
                var enableNewInstRental = false;
                var enableSecondhandInstRental = false;

                if (IVS020.Vars.objEditingSlip.InstallationType == IVS020_InstallType.C_SALE_INSTALL_TYPE_NEW
                    || IVS020.Vars.objEditingSlip.InstallationType == IVS020_InstallType.C_SALE_INSTALL_TYPE_ADD
                    || IVS020.Vars.objEditingSlip.InstallationType == IVS020_InstallType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                ) {

                    enableNewInstSale = true;
                    enableNewInstSample = true;

                } else if (IVS020.Vars.objEditingSlip.InstallationType == IVS020_InstallType.C_RENTAL_INSTALL_TYPE_NEW
                    || IVS020.Vars.objEditingSlip.InstallationType == IVS020_InstallType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW
                    || IVS020.Vars.objEditingSlip.InstallationType == IVS020_InstallType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW
                    || IVS020.Vars.objEditingSlip.InstallationType == IVS020_InstallType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                    || IVS020.Vars.objEditingSlip.InstallationType == IVS020_InstallType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                ) {

                    enableNewInstSample = true;
                    enableNewInstRental = true;
                    enableSecondhandInstRental = true;

                }

                if (result.ProjectStatus) {
                    IVS020.jQuery.RegisterSection.txtRegProjectCode().val("");
                    IVS020.Vars.ProjectStatus = true;
                }
                else {
                    IVS020.Vars.ProjectStatus = false;
                }

                GenerateNumericBox2(grid, IVS020.GridColumnID.Register.NewInstSaleID,
                    row_id, IVS020.GridColumnID.Register.NewInstSale,
                    "", 5, 0, 0, 99999, false, enableNewInstSale);

                GenerateNumericBox2(grid, IVS020.GridColumnID.Register.NewInstSampleID,
                    row_id, IVS020.GridColumnID.Register.NewInstSample,
                    "", 5, 0, 0, 99999, false, enableNewInstSample);

                GenerateNumericBox2(grid, IVS020.GridColumnID.Register.NewInstRentalID,
                    row_id, IVS020.GridColumnID.Register.NewInstRental,
                    "", 5, 0, 0, 99999, false, enableNewInstRental);

                GenerateNumericBox2(grid, IVS020.GridColumnID.Register.SecondhandInstRentalID,
                    row_id, IVS020.GridColumnID.Register.SecondhandInstRental,
                    "", 5, 0, 0, 99999, false, enableSecondhandInstRental);

                // Add 2nd row of detail for S/N input.
                row_id = grid.uid();
                grid.addRow(row_id
                    , IVS020.Functions.grdRegister_NewData(
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

                grid.setColspan(row_id, idxInstrumentCode, 7);
                grid.setCellTextStyle(row_id, idxInstrumentCode, 'text-align:right');
                grid.cells(row_id, idxInstrumentCode).setAttribute("child-remark", true);

                var idRemark = GenerateGridControlID(IVS020.GridColumnID.Register.RemarkID, instrument_row_id);
                var txtRemark = $("<textarea></textarea>")
                    .attr("id", idRemark)
                    .attr("row_id", row_id)
                    .attr("name", IVS020.GridColumnID.Register.Remark)
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
                    grid.setRowColor(row_id, IVS020.Colors.NormalRow);
                }
                else {
                    grid.setRowColor(row_id, IVS020.Colors.ChildRow);
                }
            }

            $("#divRegisterGrid textarea[name=Remark]")
                .unbind("focus")
                .focus(function () {
                    IVS020.Grids.grdRegister.selectRowById($(this).attr("row_id"));
                })
                .change(function () {
                    var row_id = $(this).attr("row_id");
                    var idxInstrumentCode = grid.getColIndexById(IVS020.GridColumnID.Register.InstrumentCode);
                    IVS020.Grids.grdRegister.cells(row_id, idxInstrumentCode).setAttribute("title", $(this).val());
                });

            $("#lnkShowSN")
                .unbind("click")
                .click(function () {
                    $("textarea[name=" + IVS020.GridColumnID.Register.Remark + "]").each(function () {
                        var row_id = $(this).attr("row_id");
                        IVS020.Grids.grdRegister.setRowHidden(row_id, false);
                    });
                    IVS020.Grids.grdRegister.setSizes();
                    return false;
                });

            $("#lnkHideSN")
                .unbind("click")
                .click(function () {
                    $("textarea[name=" + IVS020.GridColumnID.Register.Remark + "]").each(function () {
                        var row_id = $(this).attr("row_id");
                        IVS020.Grids.grdRegister.setRowHidden(row_id, true);
                    });
                    IVS020.Grids.grdRegister.setSizes();
                    return false;
                });

            grid.setSizes();
        },

        grdRegister_NewData: function (InstrumentCode, InstrumentName, AddInstalledQty, NewInstSale, NewInstSample, NewInstRental, SecondhandInstRental, ToJson) {
            return [
                "",
                String(InstrumentCode),
                String(InstrumentName),
                String(AddInstalledQty),
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
    IVS020.Functions.InitializeGrid();
    IVS020.Functions.InitializeScreen();
    IVS020.Functions.SetScreenMode(IVS020.ScreenMode.Search);
});
