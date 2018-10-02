/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/object/ajax_method.js"/>
/// <reference path="../Base/GridControl.js" />
/// <reference path="../Base/object/command_event.js" />

/// <reference path="../Base/DateTimePicker.js" />
/// <reference path="../Base/control_events.js" />

var PorderGrid = null,
    SpecialGrid = null,
    IntrumentGrid = null,
    SpecialQty = "SpecialQty",
    instRevQty = "NewReceiveQty",
    InstAreaID = "InstArea";

$(document).ready(function () {

    InitialDateFromToControl("from", "to");

    $("#txtPOStockInDate").InitialDate();
    $("#txtSpecialStockInDate").InitialDate();

    $("#txtPOStockInDate").SetMinDate(IVS010_Constants.MINDATE);
    $("#txtSpecialStockInDate").SetMinDate(IVS010_Constants.MINDATE);

    $("#txtPOStockInDate").SetMaxDate(IVS010_Constants.MAXDATE);
    $("#txtSpecialStockInDate").SetMaxDate(IVS010_Constants.MAXDATE);

    initScreen();
    initGrid();
    initButton();
    initEvent();

});

function initGrid() {
    var tmpPorderGrid = $("#PorderGrid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/PurchaseOrderGrid", function () {

        BindOnLoadedEvent(tmpPorderGrid, function (gen_ctrl) {
            for (var i = 0; i < tmpPorderGrid.getRowsNum(); i++) {
                var row_id = tmpPorderGrid.getRowId(i);

                if (gen_ctrl) {
                    //GenerateDetailButton(tmpPorderGrid, "btnSelect", row_id, "Select", true);
                    GenerateSelectButton(tmpPorderGrid, "btnSelect", row_id, "Select", true);
                }

                BindGridButtonClickEvent("btnSelect", row_id, function (rid) {
                    //DeleteAllRow(IntrumentGrid);
                    var col = tmpPorderGrid.getColIndexById("PurchaseOrderNo");
                    var purchaseOrderNo = tmpPorderGrid.cells(rid, col).getValue();
                    $("#IntrumentOrder").LoadDataToGrid(IntrumentGrid, 0, false, "/inventory/getPurchasOrderDetail", { purchaserOrder: purchaseOrderNo }, "doPurchaseOrderDetail", false, null, function () {
                        ajax_method.CallScreenController("/inventory/getSinglePurchasOrderDetail", { purchaserOrder: purchaseOrderNo }, function (res, controls) {
                            if (typeof (res) == "object" && res.length > 0) {
                                $("#DetOrderNo").val(res[0].PurchaseOrderNo);
                                $("#DetSupplierCode").val(res[0].SupplierCode);
                                $("#DetSupplierName").val(res[0].SupplierName);
                                $("#DetSupNation").val(res[0].RegionName);
                                $("#DetCurrency").val(res[0].CurrencyName);
                                $("#DetOrderStatus").val(res[0].PurchaseOrderStatusName);
                                $("#DetTransportType").val(res[0].TransportTypeName);
                                $("#DetSuppDeliveryOrderNo").focus();

                                $("#txtPOStockInDate").SetDate(new Date);

                                if (res[0].PurchaseOrderStatus == IVS010_Constants.C_PURCHASE_ORDER_STATUS_COMPLETE_RECEIVE) {
                                    register_command.SetCommand(null);
                                    $("#PurchaseStockInDetail").hide();
                                    $("#IntrumentOrder").find("input").SetDisabled(true);
                                    $("#IntrumentOrder").find("select").SetDisabled(true);
                                }
                                else {
                                    register_command.SetCommand(cmdReg); // SetRegisterCommand(true, cmdReg);
                                    $("#PurchaseStockInDetail").show();
                                }
                                $("#PurchaserOrderDetail").show();
                                $("#Pinstrument").show();
                            }
                        });
                    });
                });
            }
            tmpPorderGrid.setSizes();
        });

        SpecialGridControl(tmpPorderGrid, ["Select"]);

        PorderGrid = tmpPorderGrid;
    });

    var tmpIntrumentGrid = $("#IntrumentOrder").InitialGrid(0, false, "/inventory/InstrumentGrid", function () {
        var InstAreaHtml = $("#divTmp").html();
        var TmpInstArea = null;

        BindOnLoadedEvent(tmpIntrumentGrid, function (gen_ctrl) {
            var blnAjax = false;

            for (var i = 0; i < tmpIntrumentGrid.getRowsNum(); i++) {
                if (CheckFirstRowIsEmpty(tmpIntrumentGrid))
                    return false;
                var row_id = tmpIntrumentGrid.getRowId(i);
                var qtyCol = tmpIntrumentGrid.getColIndexById("NewReceiveQty");
                var val = GetValueFromLinkType(tmpIntrumentGrid, i, qtyCol);

                if (gen_ctrl) {
                    GenerateNumericBox2(tmpIntrumentGrid, instRevQty, row_id, "NewReceiveQty", val, 5, 0, 0, 99999, true, true);
                }

                var RemainQty = tmpIntrumentGrid.cells(row_id, tmpIntrumentGrid.getColIndexById("RemainQty")).getValue();
                var PurchaseQty = tmpIntrumentGrid.cells(row_id, tmpIntrumentGrid.getColIndexById("PurchaseQty")).getValue();

                //  TmpInstArea = InstAreaHtml.replace("InstrumentArea", GenerateGridControlID(InstAreaID, row_id));
                // TmpInstArea = InstAreaHtml.replace("InstrumentArea", GenerateGridControlID(InstAreaID, row_id) + "\" style=\"width:208px;");

                TmpInstArea = InstAreaHtml.replace(/tmpArea/g, GenerateGridControlID(InstAreaID, row_id));
                tmpIntrumentGrid.cells(row_id, tmpIntrumentGrid.getColIndexById("InstArea")).setValue(TmpInstArea);


                if (RemainQty == PurchaseQty) {
                    $("#" + GenerateGridControlID(instRevQty, row_id)).SetDisabled(false);
                    $("#" + GenerateGridControlID(InstAreaID, row_id)).SetDisabled(false);
                }

                if (RemainQty == 0) {
                    $("#" + GenerateGridControlID(instRevQty, row_id)).SetDisabled(true);
                    $("#" + GenerateGridControlID(InstAreaID, row_id)).SetDisabled(true);

                    var SourceAreaCode = tmpIntrumentGrid.cells(row_id, tmpIntrumentGrid.getColIndexById("SourceAreaCode")).getValue();
                    $("#" + GenerateGridControlID(InstAreaID, row_id)).val(SourceAreaCode);
                }

                if (RemainQty < PurchaseQty && RemainQty > 0) {
                    $("#" + GenerateGridControlID(instRevQty, row_id)).SetDisabled(false);
                    $("#" + GenerateGridControlID(InstAreaID, row_id)).SetDisabled(true);

                    var SourceAreaCode = tmpIntrumentGrid.cells(row_id, tmpIntrumentGrid.getColIndexById("SourceAreaCode")).getValue();
                    $("#" + GenerateGridControlID(InstAreaID, row_id)).val(SourceAreaCode);
                }
            }
        });

        SpecialGridControl(tmpIntrumentGrid, ["NewReceiveQty", "InstArea"]);

        tmpIntrumentGrid.attachEvent("onBeforeSelect", function () {
            return false;
        });

        IntrumentGrid = tmpIntrumentGrid;
    });

    var tmpSpecialGrid = $("#specialInstGrid").InitialGrid(0, false, "/inventory/SpecialGrid", function () {

        // Pending Remove by Non A. 27/Apr/2012
        //BindOnLoadedEvent(SpecialGrid, function () {
        //    for (var i = 0; i < SpecialGrid.getRowsNum(); i++) {
        //        var row_id = SpecialGrid.getRowId(i);

        //        GenerateSelectButton(SpecialGrid, "btnRemove", row_id, "Remove", true);
        //        BindGridButtonClickEvent("btnRemove", row_id, function (rid) {
        //            DeleteRow(SpecialGrid, rid);
        //        });

        //    }
        //    SpecialGrid.setSizes();
        //});

        SpecialGridControl(tmpSpecialGrid, ["StockInQty", "Remove"]);

        SpecialGrid = tmpSpecialGrid;
    });

}

function initEvent() {
    $("input[id='InstCode']").blur(function () {
        if ($(this).autocomplete('widget').is(':visible')) {
            return;
        }

        var InstCode = $.trim($(this).val());

        if (InstCode != '') {
            ajax_method.CallScreenController('/inventory/getInstrumentName', { 'InstrumentCode': InstCode }, function (instrumentSelect) {
                if (instrumentSelect != null && $.trim(instrumentSelect.InstrumentName) != '') {
                    $("#InstName").val(instrumentSelect.InstrumentName);
                    $("#InstCode").val(instrumentSelect.InstrumentCode);
                } else {
                    $("#InstName").val('');
                    $("#InstCode").val('');
                }
            });
            $(this).val(InstCode);
        } else {
            $("#InstrumentName").val('');
            $(this).val(InstCode);
        }
    });

    $("#DetMemo").SetMaxLengthTextArea(1000);
    $("#SpcMemo").SetMaxLengthTextArea(1000);
}

function initButton() {
    $("#NewRegister").click(function () {
        initScreen();
    });

    $("#DownloadSlip").click(function () {
        var param = {
            strInvSlipNo: $("#SlipNo").val()
        };

        ajax_method.CallScreenController("/inventory/IVS010_DownloadDocument", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/inventory/IVS010_DownloadPdfAndWriteLog?k=" + key + "&strDocumentNo=" + result.DocumentNo + "&documentOCC=" + result.DocumentOCC + "&strDocumentCode=" + result.DocumentCode + "&fileName=" + result.FilePath)
                window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

    $("#btnSelectType").click(function () {
        var Type = $("input[name='type']:checked").val();

        if (Type == "0") {
            reset_command.SetCommand(cmdReset); // SetResetCommand(true, cmdReset);
            $("#SpecifyStockIn").SetViewMode(true);
            $("#SearchPurchaseOrder").show();
        } else if (Type == "1") {
            reset_command.SetCommand(cmdReset); // SetResetCommand(true, cmdReset);
            $("#SpecifyStockIn").SetViewMode(true);
            $("#SpecialStockin").show();
            $("#Sinstrument").show();
            $("#instData").show();

            $("#txtSpecialStockInDate").SetDate(new Date);
        }

    });
    $("#btnClear").click(function () {
        CloseWarningDialog();
        $("#SearchPurchaseOrder").clearForm();
        //  $("#SearchPurchaseOrder").ResetToNormalControl();
        DeleteAllRow(PorderGrid);

    });
    $("#btnSearch").click(function () {
        var doPurchaseOrder = {
            PurchaseOrderNo: $("#PurchaseOrderNo").val(),
            PurchaseOrderStatus: $("#PurchaseOrderStatus").val(),
            SupplierCode: $("#SupplierCode").val(),
            SupplierName: $("#SupplierName").val(),
            TransportType: $("#TransportType").val()
        }
        $("#PorderGrid").LoadDataToGrid(PorderGrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/searchPurchaseOrder", doPurchaseOrder, "doPurchaseOrder", false,

                function (result, controls) {
                    VaridateCtrl_AtLeast(["PurchaseOrderNo", "SupplierCode", "SupplierName", "PurchaseOrderStatus", "TransportType"], null);
                    if (controls != undefined) {
                        //    VaridateCtrl_AtLeast(["PurchaseOrderNo", "SupplierCode", "SupplierName", "PurchaseOrderStatus", "TransportType"], controls);
                    }

                }
                , function () {
                    //                    if (CheckFirstRowIsEmpty(PorderGrid, false)) {
                    //                        $("#btnSearch").SetDisabled(false);
                    //                    }
                    //                    else {
                    //                        $("#btnSearch").SetDisabled(true);
                    //                    }
                });

    });
    $('#btnSearchInstrument').click(function () {
        $('#dlgBox').OpenCMS170Dialog("IVS010");
    });
    $("#InstCancel").click(function () {
        CloseWarningDialog()
        $("#instData").clearForm();

    });
    $("#InstAdd").click(function () {
        $("#Sinstrument").find(".highlight").toggleClass("highlight", false);

        var objInst = {
            InstrumentCode: $.trim($("#InstCode").val()),
            InstrumentName: $("#InstName").val(),
            InstrumentQty: $("#InstrumentQty").NumericValue(),
            InstrumentArea: $.trim($("#InstrumentArea option:selected").val())
        };

        ajax_method.CallScreenController("/inventory/checkSpecialAdd", objInst, function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["InstCode", "InstrumentQty", "InstrumentArea"], controls);
                InstQty();
            }
            if (result == true) {
                CheckFirstRowIsEmpty(SpecialGrid, true);

                var inst_name = $("#InstName").val();
                var inst_areaname = $("#InstrumentArea option:selected").text();

                if (objInst.InstrumentQty <= 0) {
                    var messageParam = { "module": "Inventory", "code": "MSG4040", "param": "" };
                    call_ajax_method("/Shared/GetMessage", messageParam, function (data) {
                        VaridateCtrl(["InstrumentQty"], ["InstrumentQty"]);
                        OpenWarningDialog(data.Message);
                    });
                    return;
                }

                if (IsExistedInstrument(SpecialGrid, objInst.InstrumentCode, inst_name, inst_areaname)) {
                    var messageParam = { "module": "Inventory", "code": "MSG4039", "param": "" };
                    call_ajax_method("/Shared/GetMessage", messageParam, function (data) {
                        OpenWarningDialog(data.Message);
                    });
                    return;
                }

                //AddNewRow(SpecialGrid, [objInst.InstrumentCode, inst_name, "", inst_areaname, ""]);
                AddNewRow(SpecialGrid, [ConvertBlockHtml(objInst.InstrumentCode), ConvertBlockHtml(inst_name), "", inst_areaname, ""]); //Modify by Jutarat A. on 28112013
                
                var qtyCol = SpecialGrid.getColIndexById("StockInQty");

                var row_idx = SpecialGrid.getRowsNum() - 1;
                var row_id = SpecialGrid.getRowId(row_idx);

                GenerateNumericBox2(SpecialGrid, SpecialQty, row_id, "StockInQty", objInst.InstrumentQty, 5, 0, 0, 99999, 0, true);
                GenerateRemoveButton(SpecialGrid, "btnRemoveInst", row_id, "Remove", true);

                SpecialGrid.setSizes();
                BindGridButtonClickEvent("btnRemoveInst", row_id, function (rid) {
                    DeleteRow(SpecialGrid, rid);
                    if (SpecialGrid.getRowsNum() <= 0 || CheckFirstRowIsEmpty(SpecialGrid)) {
                        register_command.SetCommand(null); // SetRegisterCommand(false, null);
                    }
                });

                if (SpecialGrid.getRowsNum() > 0) {
                    register_command.SetCommand(cmdRegSpecial); // SetRegisterCommand(true, cmdRegSpecial);
                }

                $("#instData").clearForm();

            }

        });

    });
}

function IsExistedInstrument(grid, inst_code, inst_name, inst_area) {
    for (var i = 0; i < grid.getRowsNum(); i++) {
        var tmp_inst_code = grid.cells2(i, grid.getColIndexById("InstCode")).getValue();
        var tmp_inst_name = grid.cells2(i, grid.getColIndexById("InstName")).getValue();
        var tmp_inst_area = grid.cells2(i, grid.getColIndexById("InstArea")).getValue();

        // Akat K. : check duplicate not have to check instrument name
        // Name in database when send to grid '  ' will reduce to ' '
        //if (tmp_inst_code == inst_code && tmp_inst_name == inst_name && tmp_inst_area == inst_area) {
        if (tmp_inst_code == inst_code && tmp_inst_area == inst_area) {
            return true;
        }
    }

    return false;
}

function VaridateCtrl_AtLeast(ctrl_lst, null_ctrl) {
    if (ctrl_lst != null) {
        for (var idx = 0; idx < ctrl_lst.length; idx++) {
            var ctrl = $("#" + ctrl_lst[idx]);
            if (ctrl.length > 0) {
                ctrl.removeClass("highlight");


                if (ctrl[0].tagName.toLowerCase() == "select") {
                    var unb = function () {
                        for (var i = 0; i < ctrl_lst.length; i++) {
                            CloseWarningDialog();
                            $("#" + ctrl_lst[i]).removeClass("highlight");
                            $("#" + ctrl_lst[i]).unbind("change", unb);
                        }
                    };
                    ctrl.change(unb);
                }
                else {
                    var unb = function () {
                        for (var i = 0; i < ctrl_lst.length; i++) {
                            CloseWarningDialog();
                            $("#" + ctrl_lst[i]).removeClass("highlight");
                            $("#" + ctrl_lst[i]).unbind("keyup", unb);
                        }
                        // $(this).removeClass("highlight");
                        //  $(this).unbind("keyup", unb);
                    };
                    ctrl.keyup(unb);


                }
            }
        }
    }
    if (null_ctrl != null) {
        for (var idx = 0; idx < null_ctrl.length; idx++) {
            if (null_ctrl[idx] != "") {
                var ctrl = $("#" + null_ctrl[idx]);
                if (ctrl.length > 0) {
                    ctrl.addClass("highlight");
                }
            }
        }
    }
}

function InstQty() {
    if ($("#InstrumentQty").length > 0)
        $("#InstrumentQty").BindNumericBox(5, 0, 0, 99999, 0);
}

function cmdReset() {
    ajax_method.CallScreenController("/inventory/cmdReset", "", function (result, controls) {
        if (result) {
            $("select").prop('selectedIndex', 0);
            initScreen();
        }
    });
}

function cmdReg() {
    var receiveQtyIDList = new Array();
    var InstList = new Array();
    for (var i = 0; i < IntrumentGrid.getRowsNum(); i++) {
        var row_id = IntrumentGrid.getRowId(i);

        var Inst = {
            InstrumentCode: IntrumentGrid.cells(row_id, IntrumentGrid.getColIndexById("InstrumentCode")).getValue(),
            PurchaseQty: IntrumentGrid.cells(row_id, IntrumentGrid.getColIndexById("PurchaseQty")).getValue(),
            RemainQty: IntrumentGrid.cells(row_id, IntrumentGrid.getColIndexById("RemainQty")).getValue(),
            NewReceiveQty: $("#" + GenerateGridControlID(instRevQty, row_id)).NumericValue(),
            InstrumentArea: $("#" + GenerateGridControlID(InstAreaID, row_id)).val(),
            NewReceiveQtyID: GenerateGridControlID(instRevQty, row_id),
            InstrumentAreaID: GenerateGridControlID(InstAreaID, row_id),
            row_id: row_id
        };
        InstList.push(Inst);
        receiveQtyIDList.push(GenerateGridControlID(InstAreaID, row_id));
    }

    var obj = {
        PurchaseOrderNo: $.trim($("#DetOrderNo").val()),
        ApproveNo: $.trim($("#DetApproveNo").val()),
        Memo: $("#DetMemo").val(),
        SupplierDeliveryOrderNo: $.trim($("#DetSuppDeliveryOrderNo").val()),
        numKeyEnter: "2",
        StockInDate: $("#txtPOStockInDate").val(),
        StockInInstrument: InstList
    };

    $("#IntrumentOrder").find(".highlight").toggleClass("highlight", false);

    ajax_method.CallScreenController("/inventory/cmdRegPurchase", obj, function (result, controls) {

        if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }

        if (result == true) {
            //$("#SpecifyStockIn").hide(); //$("#SpecifyStockIn").SetViewMode(true);
            $("#SearchPurchaseOrder").hide(); //$("#SearchPurchaseOrder").SetViewMode(true);
            $("#PurchaserOrderDetail").SetViewMode(true);
            $("#Pinstrument").SetViewMode(true);

            register_command.SetCommand(null); // SetRegisterCommand(false, null);
            reset_command.SetCommand(null); // SetResetCommand(false, null);
            confirm_command.SetCommand(cmdConfirmPurcahse); // SetConfirmCommand(true, cmdConfirmPurcahse);
            back_command.SetCommand(cmdBackPurchase); // SetBackCommand(true, cmdBackPurchase);


        }
    });

}

function cmdRegSpecial() {

    var InstListGrid = new Array();

    for (var i = 0; i < SpecialGrid.getRowsNum(); i++) {
        var row_id = SpecialGrid.getRowId(i);
        var Inst = {
            InstrumentCode: SpecialGrid.cells(row_id, SpecialGrid.getColIndexById("InstCode")).getValue(),
            StockInQty: $("#" + GenerateGridControlID(SpecialQty, row_id)).NumericValue(),
            InstrumentArea: SpecialGrid.cells(row_id, SpecialGrid.getColIndexById("InstArea")).getValue(),
            StockInQtyID: GenerateGridControlID(SpecialQty, row_id),
            row_id: row_id
        };
        InstListGrid.push(Inst);
    }

    var obj = {
        SupplierDeliveryOrderNo: $("#SpcOrderNo").val(),
        ApproveNo: $.trim($("#SpcApproveNo").val()),
        Memo: $("#SpcMemo").val(),
        StockInDate: $("#txtSpecialStockInDate").val(),
        StockInstrumentSPC: InstListGrid
    };

    $("#SpecialStockin").find(".highlight").toggleClass("highlight", false);
    $("#Sinstrument").find(".highlight").toggleClass("highlight", false);

    ajax_method.CallScreenController("/inventory/cmdRegSpecial", obj, function (result, controls) {
        if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }

        if (result) {
            register_command.SetCommand(null); // SetRegisterCommand(false, null);
            reset_command.SetCommand(null); // SetResetCommand(false, null);
            confirm_command.SetCommand(cmdConfirmSpecial); // SetConfirmCommand(true, cmdConfirmSpecial);
            back_command.SetCommand(cmdBackSpecial); // SetBackCommand(true, cmdBackSpecial);

            $("#instData").hide();

            //$("#SpecifyStockIn").SetViewMode(true);
            $("#SpecialStockin").SetViewMode(true);
            $("#Sinstrument").SetViewMode(true);
            var removeCol = SpecialGrid.getColIndexById("Remove");
            SpecialGrid.setColumnHidden(removeCol, true);
        }


    });

}

function cmdConfirmPurcahse() {
    ajax_method.CallScreenController("/inventory/cmdConfirmPurchase", "", function (res) {
        if (res != null) {
            $("#ShowSlipNo").show();
            $("#SlipNo").val(res);
            confirm_command.SetCommand(null); // SetConfirmCommand(false, "");
            back_command.SetCommand(null); // SetBackCommand(false, null);
        }
    });
}

function cmdConfirmSpecial() {
    ajax_method.CallScreenController("/inventory/cmdConfirmSpecial", "", function (res) {
        if (res != null) {
            var obj = { module: "Common", code: "MSG0046" };
            ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
                OpenInformationMessageDialog(result.Code, result.Message, function () {
                    $("#ShowSlipNo").show();
                    $("#SlipNo").val(res);
                    confirm_command.SetCommand(null); // SetConfirmCommand(false, null);
                    back_command.SetCommand(null); // SetBackCommand(false, null);
                });
            });
        }
    })
}

function cmdBackPurchase() {
    //$("#SpecifyStockIn").show(); //$("#SpecifyStockIn").SetViewMode(false);
    $("#SearchPurchaseOrder").show(); //$("#SearchPurchaseOrder").SetViewMode(false);
    $("#PurchaserOrderDetail").SetViewMode(false);
    $("#Pinstrument").SetViewMode(false);

    register_command.SetCommand(cmdReg); // SetRegisterCommand(true, cmdReg);
    reset_command.SetCommand(cmdReset); // SetResetCommand(true, cmdReset);
    confirm_command.SetCommand(null); // SetConfirmCommand(false, null);
    back_command.SetCommand(null); // SetBackCommand(false, null);
}

function cmdBackSpecial() {
    //$("#SpecifyStockIn").SetViewMode(false);
    $("#SpecialStockin").SetViewMode(false);
    $("#Sinstrument").SetViewMode(false);

    $("#instData").clearForm();
    $("#instData").show();

    register_command.SetCommand(cmdRegSpecial); // SetRegisterCommand(true, cmdReg);
    reset_command.SetCommand(cmdReset); // SetResetCommand(true, cmdReset);
    confirm_command.SetCommand(null); // SetConfirmCommand(false, null);
    back_command.SetCommand(null); // SetBackCommand(false, null);

    var removeCol = SpecialGrid.getColIndexById("Remove");
    SpecialGrid.setColumnHidden(removeCol, false);
}

function initScreen() {

    $("#DetOrderNo").SetDisabled(true);
    $("#DetSupplierCode").SetDisabled(true);
    $("#DetSupplierName").SetDisabled(true);
    $("#DetSupNation").SetDisabled(true);
    $("#DetCurrency").SetDisabled(true);

    $("#DetOrderStatus").SetDisabled(true);
    $("#DetTransportType").SetDisabled(true);
    $("#InstName").SetDisabled(true);
    $("#SlipNo").SetDisabled(true);

    $("#SpecifyStockIn").clearForm();
    $("#SearchPurchaseOrder").clearForm();
    $("#PurchaserOrderDetail").clearForm();
    $("#SpecialStockin").clearForm();
    $("#Sinstrument").clearForm();
    $("#ShowSlipNo").clearForm();

    $("#InstCode").InitialAutoComplete("/Master/GetInstrumentCodeAll", null, true);

    register_command.SetCommand(null); // SetRegisterCommand(false, null);
    reset_command.SetCommand(null); // SetResetCommand(false, null);
    confirm_command.SetCommand(null); // SetConfirmCommand(false, null);
    back_command.SetCommand(null); // SetBackCommand(false, null);

    $("#SpecifyStockIn").show();
    $("#SearchPurchaseOrder").hide();
    $("#PurchaserOrderDetail").hide();
    $("#Pinstrument").hide();
    $("#ShowSlipNo").hide();
    $("#SpecialStockin").hide();
    $("#Sinstrument").hide();

    $("#SearchPurchaseOrder").SetViewMode(false);
    $("#PurchaserOrderDetail").SetViewMode(false);
    $("#Pinstrument").SetViewMode(false);
    $("#SpecialStockin").SetViewMode(false);
    $("#Sinstrument").SetViewMode(false);

    $("#divTmp").hide();
    $("#SpecifyStockIn").SetViewMode(false);

    InstQty();

    if (PorderGrid != null) {
        DeleteAllRow(PorderGrid);
    }

    if (IntrumentGrid != null) {
        DeleteAllRow(IntrumentGrid);
    }

    if (SpecialGrid != null) {
        DeleteAllRow(SpecialGrid);
    }

    $("#Stock0").attr("checked", true);

    ajax_method.CallScreenController("/inventory/IVS010_CanSpeacialStockIn", "", function (result, controls) {
        if (result != null) {
            if (result)
                $("#Stock1").SetDisabled(false);
            else
                $("#Stock1").SetDisabled(true);
        }

    });

}

function CMS170Object() {
    return { bExpTypeHas: true,
        bExpTypeNo: true,
        bProdTypeSale: false,
        bProdTypeAlarm: false,
        bInstTypeGen: true,
        bInstTypeMonitoring: false,
        bInstTypeMat: true
    };
}

function CMS170Respone() {

}

function CMS170Response(dtNewInst) {

    $("#dlgBox").CloseDialog();

    //$("#InstCode").val(dtNewInst.InstrumentCode);

    if (dtNewInst.InstrumentCode != '') {
        ajax_method.CallScreenController('/inventory/getInstrumentName', { 'InstrumentCode': dtNewInst.InstrumentCode }, function (instrumentSelect) {
            if (instrumentSelect != null && $.trim(instrumentSelect.InstrumentName) != '') {
                $("#InstName").val(instrumentSelect.InstrumentName);
                $("#InstCode").val(instrumentSelect.InstrumentCode);
            } else {
                $("#InstName").val('');
            }
        });
    }

}

function clearUnderPart() {

    DeleteAllRow(IntrumentGrid);
    $("#PurchaserOrderDetail").clearForm();
}