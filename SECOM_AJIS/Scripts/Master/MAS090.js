/// <reference path = "../../Scripts/jquery-1.5.1-vsdoc.js" />
/// <reference path = "../../Scripts/Base/GridControl.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxcommon.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgrid.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgridcell.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js" />

var newMode = false;
var detailMode = false;
var objMAS090;
var mygrid = null;
var btn_instrument_detail = "btnInstrumentDetail";
var selectedRowID;
$(document).ready(function () {
    $("#Search_Result").hide();
    $("#Result_Detail").hide();
    $("#req_lineup_type").hide();
    $("#req_expansion_type").hide();

    $("#txtInstrumentNameSearch").InitialAutoComplete("/Master/MAS090_GetInatrumentName");

    //$("#CreateDate").InitialDate();

    $("#SaleUnitPriceText").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#RentalUnitPriceText").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#AddUnitPriceText").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#RemoveUnitPriceText").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#MoveUnitPriceText").BindNumericBox(12, 2, 0, 999999999999.99);

    $("#NoInstallStaffText").BindNumericBox(9, 0, 0, 999999999);
    $("#TimeInstallText").BindNumericBox(9, 0, 0, 999999999);
    $("#WorkerRatioText").BindNumericBox(12, 2, 0, 999999999999.99);

    $("#btnSearch").click(instrumentSearch);
    $("#btnClear").click(clearSearchCriteria);
    $("#btnNew").click(newInstrument);
    //$("#InstrumentTypeCode").change(checkInstrumentType);

    $("#radioInstrumentFlagMain").change(function () {
        $("#radioInstrumentFlagOption").attr("checked", false);
        $("#InstrumentFlag").val(true);
    });
    $("#radioInstrumentFlagOption").change(function () {
        $("#radioInstrumentFlagMain").attr("checked", false);
        $("#InstrumentFlag").val(false);
    });

    $("#txtInstrumentCodeSearch").InitialAutoComplete("/Master/GetInstrumentCodeAll", null, true);

    if (permission.ADD == "False") {
        $("#btnNew").attr("disabled", true);
    }

    clearResultDetail();
    $("#ParentInstrumentCode").attr("readonly", true);
    $("#ParentInstrumentName").attr("readonly", true);

    //mygrid = $("#grid_result").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/Master/MAS090_InitGrid");
    mygrid = $("#grid_result").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/Master/MAS090_InitGrid");

    SpecialGridControl(mygrid, ["Edit"]);

    mygrid.attachEvent("onBeforeSelect", function (new_row, old_row) {
        return !(detailMode || newMode);
    });
    mygrid.attachEvent("onBeforeSorting", function (ind, type, direction) {
        return !(detailMode || newMode);
    });
    mygrid.attachEvent("onBeforePageChanged", function (ind, count) {
        return !(detailMode || newMode);
    });

    BindOnLoadedEvent(mygrid, function (gen_ctrl) {
        if (mygrid.getRowsNum() != 0) {
            for (var i = 0; i < mygrid.getRowsNum(); i++) {
                var row_id = mygrid.getRowId(i);

                if (gen_ctrl == true) {
                    if (newMode || detailMode) {
                        GenerateEditButton(mygrid, btn_instrument_detail, row_id, "Edit", false);
                    } else {
                        GenerateEditButton(mygrid, btn_instrument_detail, row_id, "Edit", true);
                    }
                }

                BindGridButtonClickEvent(btn_instrument_detail, row_id, function (rid) {
                    if (!newMode && !detailMode) {
                        var InstrumentCode = mygrid.cells(rid, mygrid.getColIndexById('InstrumentCode')).getValue();
                        instrumentSearchDetail(InstrumentCode);
                        selectedRowID = rid;
                    }
                    mygrid.selectRow(mygrid.getRowIndex(rid));
                });
            }
        }
    });
});

function checkInstrumentType() {
    if ($("#InstrumentTypeCode").val() == c_type_general) {
        $("#req_lineup_type").show();
        $("#req_expansion_type").show();
    } else {
        $("#req_lineup_type").hide();
        $("#req_expansion_type").hide();
    }
}

// tt
//function enable_grid_button(col_id, button_id , enable) {
//    for (var i = 0; i < mygrid.getRowsNum(); i++) {
//        var row_id = mygrid.getRowId(i);

//        var thisid = GenerateGridControlID(button_id, row_id); 
//        thisid = "#" + thisid;

//        var col = mygrid.getColIndexById(col_id);

//        if ($.find(thisid).length > 0) {

//            if (enable) {
//                var title = mygrid.cells(row_id, col).getAttribute("defToolTip");
//                $(thisid).attr("class", "row-image");
//                $(thisid).attr("title", title);
//            }
//            else {
//                $(thisid).attr("class", "row-image-disabled");
//                $(thisid).attr("title", "");
//            }
//        }
//    }
//}

function disableDetail(disable) {
    $("#MAS090_InstrumentDetail input").attr("readonly", disable);
    $("#MAS090_InstrumentDetail button").attr("disabled", disable);
    $("#MAS090_InstrumentDetail select").attr("disabled", disable);

    $("#RentalFlag").attr("disabled", disable);
    $("#SaleFlag").attr("disabled", disable);
    $("#MaintenanceFlag").attr("disabled", disable);
    $("#ControllerFlag").attr("disabled", disable);
    $("#ZeroBahtAssetFlag").attr("disabled", disable);
    $("#radioInstrumentFlagMain").attr("disabled", disable);
    $("#radioInstrumentFlagOption").attr("disabled", disable);

    //$("#CreateDate").EnableDatePicker(false);
    $("#CreateDate").attr("readonly", true);

    $("#ParentInstrumentCode").attr("readonly", true);
    $("#ParentInstrumentName").attr("readonly", true);
}

function confirmCommand() {

    if (newMode) {

        // disable comfirm button
        DisableConfirmCommand(true);

        confirmNew();
    }
    if (detailMode) {

        // disable comfirm button
        DisableConfirmCommand(true);

        confirmDetail();
    }
}

function cancelCommand() {
    var msgprm = { "module": "Common", "code": "MSG0140" };  //param: c_cancel };
    ajax_method.CallScreenController("/Shared/GetMessage", msgprm, function (data, ctrl) {
        OpenYesNoMessageDialog(data.Code, data.Message, function () {
            //9.1	Enable “Search instrument” section
            $("#MAS090_Search input").attr("readonly", false);
            $("#MAS090_Search button").attr("disabled", false);
            $("#MAS090_Search select").attr("disabled", false);
            if (permission.ADD == "False") {
                $("#btnNew").attr("disabled", true);
            }

            //9.2	Enable “Result List” section
            //$("button#btnDetail").attr("disabled", false);
            setEnableGridButton(true);

            //9.3	Hide “Maintain instrument” section
            clearResultDetail();
            $("#Result_Detail").hide();
            SetCancelCommand(false, cancelCommand);
            SetConfirmCommand(false, confirmCommand);

            $("#ParentInstrumentCode").attr("readonly", true);
            $("#ParentInstrumentName").attr("readonly", true);

            newMode = false;
            detailMode = false;

            // tt
            //            enable_grid_button("Edit", btn_instrument_detail, true);

        });
    });
}

function setEnableGridButton(enable) {
    if ($("#grid_result").length > 0) {
        if (mygrid.getRowsNum() != 0) {
            for (var i = 0; i < mygrid.getRowsNum(); i++) {
                var row_id = mygrid.getRowId(i);
                EnableGridButton(mygrid, btn_instrument_detail, row_id, "Edit", enable);
            }
        }

        // tt
        //        mygrid.attachEvent("onAfterSorting", function (index, type, direction) {
        //            for (var i = 0; i < mygrid.getRowsNum(); i++) {
        //                var row_id = mygrid.getRowId(i);
        //                EnableGridButton(mygrid, btn_instrument_detail, row_id, "Edit", enable);
        //            }
        //        });
    }

    //    var removeCol = mygrid.getColIndexById("Detail");
    //    mygrid.setColumnHidden(removeCol, !enable);
    //    mygrid.setSizes();

    //    // Akat K. : this is a really bad solution if there are better solution please tell me.
    //    $("#grid_result").hide();
    //    $("#grid_result").show();
}

function instrumentSearch() {
    if ($("#grid_result").length > 0) {

        // disable search button
        $("#btnSearch").attr("disabled", true);
        master_event.LockWindow(true);

        var parameter = CreateObjectData($("#MAS090_Search").serialize());

        $("#grid_result").LoadDataToGrid(mygrid, ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/Master/MAS090_Search", parameter, "dtInstrument", false
            , function () {
                // enable search button
                $("#btnSearch").attr("disabled", false);
                master_event.LockWindow(false);
            }
            , function (result, controls, isWarning) {
                if (result != undefined && isWarning == undefined) {
                    $("#Search_Result").show();
                }
            }
        );
    }
    //$("#Search_Result").show();
}

function clearSearchCriteria() {
    // 4.1	Clear data in “Search instrument” section
    $("#txtInstrumentCodeSearch").val("");
    $("#txtInstrumentNameSearch").val("");
    //$("#cboLineUpTypeSearch").selectedIndex = 0;
    $("#cboLineUpTypeSearch").val("");

    //4.3	Enable [Search] button
    $("#btnSearch").attr("disabled", false);

    //4.4	Enable [New] button
    if (permission.ADD == "True") {
        $("#btnNew").attr("disabled", false);
    }

    //4.5	Hide "Result list" section
    $("#Search_Result").hide();
    if (mygrid != null) {
        DeleteAllRow(mygrid);
    }
}

function newInstrument() {
    // 5.1	Disable “Search instrument” section
    $("#MAS090_Search input").attr("readonly", true);
    $("#MAS090_Search button").attr("disabled", true);
    $("#MAS090_Search select").attr("disabled", true);

    //5.2	Disable “Result list” section
    //$("button#btnDetail").attr("disabled", "disabled")
    setEnableGridButton(false);
    disableDetail(false);

    //5.3.1	Clear data in “Maintain instrument” section
    clearResultDetail();
    //5.3	Show “Maintain instrument” section
    $("#Result_Detail").show();
    SetCancelCommand(true, cancelCommand);
    SetConfirmCommand(true, confirmCommand);

    // 5.4 current mode is true
    newMode = true;
}

function instrumentSearchDetail(InstrumentCode) {
    //6.1	Set dtInstrument to “Maintain instrument” section
    ajax_method.CallScreenController(
        '/Master/MAS090_GetInstrumentDetail/',
        { InstrumentCode: InstrumentCode },
        function (result, controls) {
            if (result != undefined) {
                $("#MAS090_InstrumentDetail").clearForm();
                $("#MAS090_InstrumentDetail").bindJSON(result);

                $("#RentalFlag").attr("checked", result.RentalFlag ? true : false);
                $("#SaleFlag").attr("checked", result.SaleFlag ? true : false);
                $("#MaintenanceFlag").attr("checked", result.MaintenanceFlag ? true : false);
                $("#ControllerFlag").attr("checked", result.ControllerFlag ? true : false);
                $("#ZeroBahtAssetFlag").attr("checked", result.ZeroBahtAssetFlag ? true : false);

                $("#LineUpTypeCode").val(result.LineUpTypeCode);

                $("#radioInstrumentFlagMain").attr("checked", result.InstrumentFlag ? true : false);
                $("#radioInstrumentFlagOption").attr("checked", result.InstrumentFlag ? false : true);

                $("#SupplierCode").val(result.SupplierCode);
                $("#InstrumentTypeCode").val(result.InstrumentTypeCode);
                $("#ExpansionTypeCode").val(result.ExpansionTypeCode);

                $("#CurrentExpansionTypeCode").val($("#ExpansionTypeCode").val());

                //6.2	Disable “Search instrument” section
                $("#MAS090_Search input").attr("readonly", true);
                $("#MAS090_Search button").attr("disabled", true);
                $("#MAS090_Search select").attr("disabled", true);

                //6.3	Disable “Result list” section
                //$("button#btnDetail").attr("disabled", "disabled")
                setEnableGridButton(false);
                if (permission.EDIT == "False") {
                    disableDetail(true);
                }
                //6.4	Disable [Instrument Code] text box
                $("#InstrumentCode").attr("readonly", true);
                //$("#InstrumentName").attr("readonly", true);

                result.CreateDate = ConvertDateObject(result.CreateDate);
                $("#CreateDate").val(ConvertDateToTextFormat(result.CreateDate));
                //$("#CreateDate").datepicker("getDate");
                //$("#CreateDate").EnableDatePicker(false);
                $("#CreateDate").attr("readonly", true);

                $("#SaleUnitPriceText").val(result.SaleUnitPrice);
                $("#RentalUnitPriceText").val(result.RentalUnitPrice);
                $("#AddUnitPriceText").val(result.AddUnitPrice);
                $("#RemoveUnitPriceText").val(result.RemoveUnitPrice);
                $("#MoveUnitPriceText").val(result.MoveUnitPrice);

                $("#SaleUnitPriceText").NumericCurrency().val(result.SaleUnitPriceCurrencyType);
                $("#RentalUnitPriceText").NumericCurrency().val(result.RentalUnitPriceCurrencyType);
                $("#AddUnitPriceText").NumericCurrency().val(result.AddUnitPriceCurrencyType);
                $("#RemoveUnitPriceText").NumericCurrency().val(result.RemoveUnitPriceCurrencyType);
                $("#MoveUnitPriceText").NumericCurrency().val(result.MoveUnitPriceCurrencyType);

                $("#NoInstallStaffText").val(result.NoInstallStaff);
                $("#TimeInstallText").val(result.TimeInstall);
                $("#WorkerRatioText").val(result.WorkerRatio);

                $("#SaleUnitPriceText").blur();
                $("#RentalUnitPriceText").blur();
                $("#AddUnitPriceText").blur();
                $("#RemoveUnitPriceText").blur();
                $("#MoveUnitPriceText").blur();
                $("#NoInstallStaffText").blur();
                $("#TimeInstallText").blur();
                $("#WorkerRatioText").blur();

                $("#UpdateDate").val(ConvertDateObject(result.UpdateDate));
                $("#UpdateBy").val(result.UpdateBy);

                //checkInstrumentType();

                //6.5	Show “Maintain instrument” section
                detailMode = true;
                $("#Result_Detail").show();
                SetCancelCommand(true, cancelCommand);
                if (permission.EDIT == "True") {
                    SetConfirmCommand(true, confirmCommand);
                } else {
                    SetConfirmCommand(false, confirmCommand);
                }
            }
        }
    );
}

function clearResultDetail() {
    $("#MAS090_InstrumentDetail").clearForm();
    $("#chkRental").attr("checked", false);
    $("#chkSale").attr("checked", false);
    $("#chkMaintenance").attr("checked", false);
    $("#chkController").attr("checked", false);
    $("#chkZeroBaht").attr("checked", false);
    $("#radioInstrumentFlagMain").attr("checked", true);
    $("#radioInstrumentFlagOption").attr("checked", false);
    //checkInstrumentType();
}

function confirmNew() {
    var parameter = CreateObjectData(
        $("#MAS090_InstrumentDetail").serialize()
            + "&SaleUnitPrice=" + $("#SaleUnitPriceText").NumericValue()
            + "&RentalUnitPrice=" + $("#RentalUnitPriceText").NumericValue()
            + "&AddUnitPrice=" + $("#AddUnitPriceText").NumericValue()
            + "&RemoveUnitPrice=" + $("#RemoveUnitPriceText").NumericValue()
            + "&MoveUnitPrice=" + $("#MoveUnitPriceText").NumericValue()
            + "&NoInstallStaff=" + $("#NoInstallStaffText").NumericValue()
            + "&TimeInstall=" + $("#TimeInstallText").NumericValue()
            + "&WorkerRatio=" + $("#WorkerRatioText").NumericValue()
            + "&InstrumentFlag= " + ($("#radioInstrumentFlagMain").prop("checked"))
    );
    ajax_method.CallScreenController(
        '/Master/MAS090_InsertInstrument/',
        parameter,
        function (result, controls) {

            // enable confrim button
            DisableConfirmCommand(false);

            VaridateCtrl(["InstrumentCode",
                                "ApproveNo",
                                "InstrumentName",
                                "InstrumentNameForSupplier",
                                "InstrumentTypeCode",
                                "LineUpTypeCode",
                                "ExpansionTypeCode",
                                "InstrumentNameForCustomerLC"], controls);

            if (result != undefined) {
                newMode = false;

                //                if (mygrid.getRowsNum() != 0) {
                $("#Search_Result").show();
                var lineuptype = "";
                if ($("#LineUpTypeCode").val() != "") {
                    lineuptype = $("#LineUpTypeCode option[selected='selected']").text();
                }

                CheckFirstRowIsEmpty(mygrid, true);
                AddNewRow(mygrid, [
                // Remove column
			            ConvertBlockHtml($("#InstrumentCode").val()) //$("#InstrumentCode").val() //Modify by Jutarat A. on 28112013
                       , ConvertBlockHtml($("#InstrumentName").val()) //$("#InstrumentName").val() //Modify by Jutarat A. on 28112013
                       , $("#InstrumentNameForSupplier").val()
                       , $("#InstrumentCategory1").val()
                       , $("#InstrumentCategory2").val()
                       , $("#InstrumentCategory3").val()
                       , lineuptype.substr(lineuptype.indexOf(":") + 1, lineuptype.length)
                       , ""]);

                var row_id = mygrid.getRowId(mygrid.getRowsNum() - 1);
                GenerateEditButton(mygrid, btn_instrument_detail, row_id, "Edit", true);

                //                }
                //Set grid to last page
                mygrid.changePage(Math.ceil(mygrid.getRowsNum() / ROWS_PER_PAGE_FOR_VIEWPAGE));

                //7.5	Hide “Maintain instrument” section
                clearResultDetail();
                $("#Result_Detail").hide();
                SetCancelCommand(false, cancelCommand);
                SetConfirmCommand(false, confirmCommand);

                //7.5.1	Disable “Search instrument” section
                $("#MAS090_Search input").attr("readonly", false);
                $("#MAS090_Search button").attr("disabled", false);
                $("#MAS090_Search select").attr("disabled", false);
                if (permission.ADD == "False") {
                    $("#btnNew").attr("disabled", true);
                }

                //7.5.2	Disable “Result list” section
                //$("button#btnDetail").attr("disabled", false)
                setEnableGridButton(true);

                //7.6	Show info message dialog [OK];
                var param = { "module": "Common", "code": "MSG0046" };
                ajax_method.CallScreenController("/Shared/GetMessage", param, function (data) {
                    OpenInformationMessageDialog(data.Code, data.Message, function () {

                        // tt
                        //                        enable_grid_button("Edit", btn_instrument_detail ,true);

                    });

                });
            }
        }
    );
}

function confirmDetail() {
    //$("#InstrumentCode").attr("readonly", false);
    //$("#InstrumentName").attr("readonly", false);
    var parameter = CreateObjectData(
        $("#MAS090_InstrumentDetail").serialize()
            + "&SaleUnitPrice=" + $("#SaleUnitPriceText").NumericValue()
            + "&SaleUnitPriceCurrencyType=" + $("#SaleUnitPriceText").NumericCurrencyValue()
            + "&RentalUnitPrice=" + $("#RentalUnitPriceText").NumericValue()
            + "&RentalUnitPriceCurrencyType=" + $("#RentalUnitPriceText").NumericCurrencyValue()
            + "&AddUnitPrice=" + $("#AddUnitPriceText").NumericValue()
            + "&AddUnitPriceCurrencyType=" + $("#AddUnitPriceText").NumericCurrencyValue()
            + "&RemoveUnitPrice=" + $("#RemoveUnitPriceText").NumericValue()
            + "&RemoveUnitPriceCurrencyType=" + $("#RemoveUnitPriceText").NumericCurrencyValue()
            + "&MoveUnitPrice=" + $("#MoveUnitPriceText").NumericValue()
            + "&MoveUnitPriceCurrencyType=" + $("#MoveUnitPriceText").NumericCurrencyValue()
            + "&NoInstallStaff=" + $("#NoInstallStaffText").NumericValue()
            + "&TimeInstall=" + $("#TimeInstallText").NumericValue()
            + "&WorkerRatio=" + $("#WorkerRatioText").NumericValue()
            + "&InstrumentFlag= " + ($("#radioInstrumentFlagMain").prop("checked"))
    );
    ajax_method.CallScreenController(
        '/Master/MAS090_UpdateInstrument/',
        parameter,
        function (result, controls) {

            // enable confrim button
            DisableConfirmCommand(false);

            VaridateCtrl(["InstrumentCode",
                                "ApproveNo",
                                "InstrumentName",
                                "InstrumentNameForSupplier",
                                "InstrumentTypeCode",
                                "LineUpTypeCode",
                                "ExpansionTypeCode",
                                "InstrumentNameForCustomerLC"], controls);

            if (controls != undefined) {

                $("#InstrumentCode").attr("readonly", true);
                //$("#InstrumentName").attr("readonly", true);
                $("#CreateDate").attr("readonly", true);
                //$("#CreateDate").EnableDatePicker(false);
            }

            if (result != undefined) {
                detailMode = false;

                var lineuptype = "";
                if ($("#LineUpTypeCode").val() != "") {
                    lineuptype = $("#LineUpTypeCode option[selected='selected']").text();
                }
                if (mygrid.getRowsNum() != 0) {
                    mygrid.cells(selectedRowID, mygrid.getColIndexById('InstrumentCode')).setValue($("#InstrumentCode").val());
                    mygrid.cells(selectedRowID, mygrid.getColIndexById('InstrumentName')).setValue($("#InstrumentName").val());
                    mygrid.cells(selectedRowID, mygrid.getColIndexById('InstrumentNameForSupplier')).setValue($("#InstrumentNameForSupplier").val());
                    mygrid.cells(selectedRowID, mygrid.getColIndexById('InstrumentCategory1')).setValue($("#InstrumentCategory1").val());
                    mygrid.cells(selectedRowID, mygrid.getColIndexById('InstrumentCategory2')).setValue($("#InstrumentCategory2").val());
                    mygrid.cells(selectedRowID, mygrid.getColIndexById('InstrumentCategory3')).setValue($("#InstrumentCategory3").val());
                    mygrid.cells(selectedRowID, mygrid.getColIndexById('LineUpTypeCode')).setValue(lineuptype.substr(lineuptype.indexOf(":") + 1, lineuptype.length));
                    GenerateEditButton(mygrid, btn_instrument_detail, selectedRowID, "Edit", true);
                    BindGridButtonClickEvent(btn_instrument_detail, selectedRowID, function (rid) {
                        if (!newMode && !detailMode) {
                            var InstrumentCode = mygrid.cells(rid, mygrid.getColIndexById('InstrumentCode')).getValue();
                            instrumentSearchDetail(InstrumentCode);
                            selectedRowID = rid;
                        }
                    });
                }

                //8.6	Hide “Maintain instrument” section
                clearResultDetail();
                $("#Result_Detail").hide();
                SetCancelCommand(false, cancelCommand);
                SetConfirmCommand(false, confirmCommand);

                //8.6.1	Disable “Search instrument” section
                $("#MAS090_Search input").attr("readonly", false);
                $("#MAS090_Search button").attr("disabled", false);
                $("#MAS090_Search select").attr("disabled", false);
                if (permission.ADD == "False") {
                    $("#btnNew").attr("disabled", true);
                }

                //8.6.2	Disable “Result list” section
                //$("button#btnDetail").attr("disabled", false)
                setEnableGridButton(true);

                //8.7	Show info message dialog [OK];
                var param = { "module": "Common", "code": "MSG0046" };
                ajax_method.CallScreenController("/Shared/GetMessage", param, function (data) {
                    OpenInformationMessageDialog(data.Code, data.Message, function () {

                        // tt
                        //enable_grid_button("Edit", btn_instrument_detail ,true);

                    });

                });

            }
        }
    );
}
