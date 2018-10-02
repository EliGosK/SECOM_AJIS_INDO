
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
/// <reference path="../../Scripts/Common/Dialog.js"/>

/// <reference path="../Base/object/ajax_method.js" />
/// <reference path="../Base/object/command_event.js" />
/// <reference path="../Base/object/master_event.js" />

var grdSeparateInvoiceGrid;
var grdSeparateInvoiceDetailGrid;

var grdCombineInvoiceGrid;
var grdCombineInvoiceDetailGrid;

var dtOldBillingDetailList;
var dtNewBillingDetailList;

var currency = {
    VatCurrencyCode: '',
    VatCurrency: '',
    WHTCurrencyCode: '',
    WHTCurrency: ''
}


$(document).ready(function () {
    // ..

    // Initial grid 1

    if ($.find("#BLS070_SeparateInvoice").length > 0) {

        grdSeparateInvoiceGrid = $("#BLS070_SeparateInvoice").InitialGrid(0, false, "/Billing/BLS070_InitialSeparateInvoiceGrid", function () {

            SpecialGridControl(grdSeparateInvoiceGrid, ["BLS070_SeparateInvoice"]);
        });

    }
    if ($.find("#BLS070_SeparateInvoiceDetail").length > 0) {

        grdSeparateInvoiceDetailGrid = $("#BLS070_SeparateInvoiceDetail").InitialGrid(999, false, "/Billing/BLS070_InitialSeparateInvoiceDetailGrid", function () {

            SpecialGridControl(grdSeparateInvoiceDetailGrid, ["BLS070_SeparateInvoiceDetail", "SelectSeparateDetail", "IssueInvoiceofSeparateDetail"]);

            BindOnLoadedEvent(grdSeparateInvoiceDetailGrid, function () {

                var colInx = grdSeparateInvoiceDetailGrid.getColIndexById('Button');

                for (var i = 0; i < grdSeparateInvoiceDetailGrid.getRowsNum(); i++) {

                    var rowId = grdSeparateInvoiceDetailGrid.getRowId(i);
                    //-----------------------------------------
                    // Col 1

                    var clt1 = "#" + GenerateGridControlID("chkSelectSeparateDetail", rowId);
                    var checkboxColInx = grdSeparateInvoiceDetailGrid.getColIndexById("SelectSeparateDetail");
                    grdSeparateInvoiceDetailGrid.cells2(i, checkboxColInx).setValue(GenerateCheckBox2("chkSelectSeparateDetail", rowId, "", true, $(clt1).prop("checked")));

                    BindGridCheckBoxClickEvent("chkSelectSeparateDetail", rowId, function (rowId, checked) {
                        chkSelectSeparateDetailGrid(rowId, checked);
                    });

                    var clt7 = "#" + GenerateGridControlID("cboIssueInvoiceofSeparateDetail", rowId);
                    GenerateGridCombobox(grdSeparateInvoiceDetailGrid, rowId, "cboIssueInvoiceofSeparateDetail", "IssueInvoiceofSeparateDetail"
                    , "/Billing/BLS070_GetComboBoxIssueInvoiceofSeparateDetail", $(clt7).val(), false);

                }

                grdSeparateInvoiceDetailGrid.setSizes();
            });

        });

    }

    if ($.find("#BLS070_CombineInvoice").length > 0) {

        grdCombineInvoiceGrid = $("#BLS070_CombineInvoice").InitialGrid(1, false, "/Billing/BLS070_InitialCombineInvoiceGrid", function () {

            SpecialGridControl(grdCombineInvoiceGrid, ["BLS070_CombineInvoice"]);
        });

    }
    if ($.find("#BLS070_CombineInvoiceDetail").length > 0) {

        grdCombineInvoiceDetailGrid = $("#BLS070_CombineInvoiceDetail").InitialGrid(999, false, "/Billing/BLS070_InitialCombineInvoiceDetailGrid", function () {

            SpecialGridControl(grdCombineInvoiceDetailGrid, ["BLS070_CombineInvoiceDetail"]);
        });

    }

    //Init Object Event
    // 1 Div Panel Body
    $("#btnRetrieve").click(btn_Retrieve_click);
    $("#btnCallScreenToSelectCombileDetail").click(function () { $("#dlgBLS070").OpenBLS071Dialog("BLS070"); });
    $("#rdoSeparateInvoice").change(rdoSeparateInvoice_Select);
    $("#rdoCombineInvoice").change(rdoCombineInvoice_Select);
    $("#rdoIssueSaleInvoice").change(rdoIssueSaleInvoice_Select);


    //Initial Page
    InitialPage();

    $("#txtSelSeparateFromInvoiceNo").focus();
});


function Add_SeparateInvoiceLine(intBefNumberOfDetail
                                , decBefAmount
                                , decBefAmountCurrency
                                , intAftNumberOfDetail
                                , decAftAmount
                                , decAftAmountCurrency) {

    var decBAmountCurrency = '';
    var decAAmountCurrency = '';
    decBAmountCurrency = decBefAmountCurrency + ' ' + decBefAmount.format(2, 3);
    decAAmountCurrency = decAftAmountCurrency + ' ' + decAftAmount.format(2, 3);
    grdSeparateInvoiceGrid.clearAll();
    AddNewRow(grdSeparateInvoiceGrid, [intBefNumberOfDetail,
                                       //decBefAmount,
                                       decBAmountCurrency,
                                       intAftNumberOfDetail,
                                       //decAftAmount
                                       decAAmountCurrency
                                       ]);

    grdSeparateInvoiceGrid.setSizes();

}

function Add_SeparateInvoiceDetailLine(doGetBillingDetailOfInvoiceList) {

    var BillingAmount = doGetBillingDetailOfInvoiceList.BillingAmountCurrencyTypeName + ' ' + doGetBillingDetailOfInvoiceList.BillingAmount.format(2, 3);
    CheckFirstRowIsEmpty(grdSeparateInvoiceDetailGrid, true);
    AddNewRow(grdSeparateInvoiceDetailGrid, ["",
    // Narupon
                                        doGetBillingDetailOfInvoiceList.ContractCodeShort + '-' + doGetBillingDetailOfInvoiceList.BillingOCC,

                                        doGetBillingDetailOfInvoiceList.ContractCode + '-' + doGetBillingDetailOfInvoiceList.BillingOCC,
                                        doGetBillingDetailOfInvoiceList.BillingDetailNo,
                                        doGetBillingDetailOfInvoiceList.BillingTypeCode + ': ' + doGetBillingDetailOfInvoiceList.InvoiceDescriptionEN,
                                        //doGetBillingDetailOfInvoiceList.BillingAmount,
                                        BillingAmount,
                                        '(1) ' + doGetBillingDetailOfInvoiceList.SiteNameEN + '<BR>(2) ' + doGetBillingDetailOfInvoiceList.SiteNameLC,
                                       "",

                                       doGetBillingDetailOfInvoiceList.ContractCode,  //--**
                                       doGetBillingDetailOfInvoiceList.BillingOCC, //--**
                                       doGetBillingDetailOfInvoiceList.BillingDetailNo //--**
                                       ]);

    var row_idx = grdSeparateInvoiceDetailGrid.getRowsNum() - 1;
    var row_id = grdSeparateInvoiceDetailGrid.getRowId(row_idx);

    BindGridCheckBoxClickEvent("chkSelectSeparateDetail", row_id, function (rowId, checked) {
        chkSelectSeparateDetailGrid(rowId, checked);
    });

    GenerateGridCombobox(grdSeparateInvoiceDetailGrid, row_id, "cboIssueInvoiceofSeparateDetail", "IssueInvoiceofSeparateDetail"
                    , "/Billing/BLS070_GetComboBoxIssueInvoiceofSeparateDetail", "", false);

    grdSeparateInvoiceDetailGrid.setSizes();

}

function chkSelectSeparateDetailGrid(rowId, checked) {


    var intBefNumberOfDetail = 0;
    var decBefAmount = 0;

    var intAftNumberOfDetailCal = 0;
    var decAftAmountCal = 0;

    var intAftNumberOfDetail = 0;
    var decAftAmount = 0;

    if (CheckFirstRowIsEmpty(grdSeparateInvoiceGrid) == false) {

        var row_id = grdSeparateInvoiceGrid.getRowId(0);


        intBefNumberOfDetail = parseFloat(grdSeparateInvoiceGrid.cells(row_id,
                                        grdSeparateInvoiceGrid.getColIndexById("BefNumberOfDetail")).getValue());
        var decBefAmountStr = grdSeparateInvoiceGrid.cells(row_id,
                                        grdSeparateInvoiceGrid.getColIndexById("BefAmount")).getValue().replace(currency.VatCurrency, '');
        decBefAmount = parseFloat(decBefAmountStr.replace(/,/g, ''));

    }


    //// ---- Waroon ----
    if (CheckFirstRowIsEmpty(grdSeparateInvoiceDetailGrid) == false) {

        for (var i = 0; i < grdSeparateInvoiceDetailGrid.getRowsNum(); i++) {

            var row_id = grdSeparateInvoiceDetailGrid.getRowId(i);
            var clt1 = "#" + GenerateGridControlID("chkSelectSeparateDetail", row_id);
            var clt7 = "#" + GenerateGridControlID("cboIssueInvoiceofSeparateDetail", row_id);

            if ($(clt1).prop('checked')) {

                intAftNumberOfDetailCal = intAftNumberOfDetailCal + 1;

                var gridBillingFeeAmtStr = grdSeparateInvoiceDetailGrid.cells(row_id,
                                            grdSeparateInvoiceDetailGrid.getColIndexById("BillingAmount")).getValue().replace(currency.VatCurrency, '');
                var gridBillingFeeAmt = parseFloat(gridBillingFeeAmtStr.replace(/,/g, ''));

                decAftAmountCal = decAftAmountCal + gridBillingFeeAmt;
                $(clt7).attr("disabled", false);
            }
            else {
                $(clt7).attr("disabled", true);
                $(clt7).prop('selectedIndex', 0);

                var gridBillingFeeAmtStr = grdSeparateInvoiceDetailGrid.cells(row_id,
                                            grdSeparateInvoiceDetailGrid.getColIndexById("BillingAmount")).getValue().replace(currency.VatCurrency, '');
                var gridBillingFeeAmt = parseFloat(gridBillingFeeAmtStr.replace(/,/g, ''));

            }
        }
    }




    var intAftNumberOfDetail = intBefNumberOfDetail - intAftNumberOfDetailCal;
    var decAftAmount = decBefAmount - decAftAmountCal;

    DeleteAllRow(grdSeparateInvoiceGrid);

    Add_SeparateInvoiceLine(intBefNumberOfDetail
                                , decBefAmount
                                , currency.VatCurrency
                                , intAftNumberOfDetail
                                , decAftAmount
                                , currency.VatCurrency);


}


function Add_CombineInvoiceLine(intBefNumberOfDetail
                                , decBefAmount
                                , decBefCurrenct
                                , intAftNumberOfDetail
                                , decAftAmount
                                , decAfCurrency) {
    // initial
    intBefNumberOfDetail = intBefNumberOfDetail == null ? "0" : intBefNumberOfDetail;
    decBefAmount = decBefAmount == null ? "0" : decBefAmount;
    intAftNumberOfDetail = intAftNumberOfDetail == null ? "0" : intAftNumberOfDetail;
    decAftAmount = decAftAmount == null ? "0" : decAftAmount;

    var decBefAmountCurrency = decBefCurrenct + ' ' + decBefAmount.format(2, 3);
    var decAfAmountCurrency = decAfCurrency + ' ' + decAftAmount.format(2, 3);

    //CheckFirstRowIsEmpty(grdCombineInvoiceGrid, true);
    grdCombineInvoiceGrid.clearAll();
    AddNewRow(grdCombineInvoiceGrid, [intBefNumberOfDetail.toString(),
                                       //decBefAmount.toString(),
                                       decBefAmountCurrency,
                                       intAftNumberOfDetail.toString(),
                                       //decAftAmount.toString()
                                       decAfAmountCurrency
                                       ]);

    grdCombineInvoiceGrid.setSizes();

}

function Add_CombineInvoiceDetailLine(doGetBillingDetailOfInvoiceList, isAddnew) { // ContractCodeShort

    isAddnew = isAddnew == null ? "" : isAddnew;
    var amountWithCurrency = '';

    if (doGetBillingDetailOfInvoiceList.BillingAmount != null)
        amountWithCurrency = doGetBillingDetailOfInvoiceList.BillingAmountCurrencyTypeName + ' '
            + doGetBillingDetailOfInvoiceList.BillingAmount.format(2, 3);

    CheckFirstRowIsEmpty(grdCombineInvoiceDetailGrid, true);
    AddNewRow(grdCombineInvoiceDetailGrid, [doGetBillingDetailOfInvoiceList.ContractCode + '-' + doGetBillingDetailOfInvoiceList.BillingOCC,

    //Narupon
                                        doGetBillingDetailOfInvoiceList.ContractCodeShort + '-' + doGetBillingDetailOfInvoiceList.BillingOCC,

                                        doGetBillingDetailOfInvoiceList.BillingDetailNo,
                                        doGetBillingDetailOfInvoiceList.BillingTypeCode + ': ' + doGetBillingDetailOfInvoiceList.InvoiceDescription,
                                        //doGetBillingDetailOfInvoiceList.BillingAmount,
                                        amountWithCurrency,
                                        '(1) ' + doGetBillingDetailOfInvoiceList.SiteNameEN + '<BR>(2) ' + doGetBillingDetailOfInvoiceList.SiteNameLC,

                                        isAddnew, //--*
                                        doGetBillingDetailOfInvoiceList.ContractCode, // -- **
                                        doGetBillingDetailOfInvoiceList.BillingOCC, // -- **
                                        doGetBillingDetailOfInvoiceList.BillingDetailNo // -- **
                                       ]);

    grdCombineInvoiceDetailGrid.setSizes();

}
function UpdateCombineInvoice() {

    var intBefNumberOfDetail = 0;
    var decBefAmount = 0;

    var intAftNumberOfDetail = 0;
    var decAftAmount = 0;

    if (CheckFirstRowIsEmpty(grdCombineInvoiceGrid) == false) {

        var row_id = grdCombineInvoiceGrid.getRowId(0);

        intBefNumberOfDetail = parseFloat(grdCombineInvoiceGrid.cells(row_id,
                                        grdCombineInvoiceGrid.getColIndexById("BefNumberOfDetail")).getValue());
        var d2 = grdCombineInvoiceGrid.cells(row_id,
                                        grdCombineInvoiceGrid.getColIndexById("BefAmount")).getValue().replace(currency.VatCurrency, '').trim();
        decBefAmount = parseFloat(d2.replace(/,/g, ''));

    }
    if (CheckFirstRowIsEmpty(grdCombineInvoiceDetailGrid) == false) {

        for (var i = 0; i < grdCombineInvoiceDetailGrid.getRowsNum(); i++) {

            var row_id = grdCombineInvoiceDetailGrid.getRowId(i);

            intAftNumberOfDetail = intAftNumberOfDetail + 1;
            var dStr = grdCombineInvoiceDetailGrid.cells(row_id,
                                        grdCombineInvoiceDetailGrid.getColIndexById("BillingAmount")).getValue();
            if (!dStr) dStr = "0";
            else var dStr = dStr.replace(currency.VatCurrency, '').trim();
            decAftAmount = decAftAmount + parseFloat(dStr.replace(/,/g, ''));
        }
    }

    DeleteAllRow(grdCombineInvoiceGrid);

    Add_CombineInvoiceLine(intBefNumberOfDetail
                                , decBefAmount
                                , currency.VatCurrency
                                , intAftNumberOfDetail
                                , decAftAmount
                                , currency.VatCurrency);

}

function chkIssueDateByBillingDetailGrid(rowId, checked) {

    //ByBillingDetailGrid.selectRow(ByBillingDetailGrid.getRowIndex(rowId));
}
function chkIssueDateByInvoiceDetailGrid(rowId, checked) {

    //ByBillingDetailGrid.selectRow(ByBillingDetailGrid.getRowIndex(rowId));
}
function chkCancelBillingDetailGrid(rowId, checked) {
    //    DeleteRow(grdCancelBillingDetailGrid, rowId);
}
function InitialPage() {

    // Date

    $("#dtpCustomerAcceptanceDate").InitialDate();
    //InitialDateFromToControl("#dptAdjustBillingPeriodDateFrom", "#dptAdjustBillingPeriodDateTo");
    //Text Input
    $("#txtSelSeparateFromInvoiceNo").attr("maxlength", 12);
    $("#txtSelCombineToInvoiceNo").attr("maxlength", 12);
    $("#txtSelSaleOCC").attr("maxlength", 4);

    $("#txtSepInvoiceNo").attr("maxlength", 12);
    $("#txtComInvoiceNo").attr("maxlength", 12);



    setVisableTable(0);
    setFormMode(conModeInit);

    //default C_ISSUE_INV_NORMAL
    $("#cboPaymentMethodsOfSeparateFrom").val("0");
    $("#cboPaymentMethodsOfCombineToInvoice").val("0");

    //default C_ISSUE_INV_NORMAL
    $("#cboIssueInvoiceAfterCombine").val("0");
    $("#cboIssueInvoiceAfterSeparate").val("0");

    // save data
    ajax_method.CallScreenController("/Billing/BLS070_InitDataFromCMS210", "",
        function (result, controls, isWarning) {
            if (result.defSelectProcessType != undefined) {

                $("#txtSelContractCode").val(result.defstrContractCode);
                $("#txtSelSaleOCC").val(result.defstrBillingOCC);

                if (result.defSelectProcessType == conModeRadio1rdoSeparateInvoice) {
                    $("#rdoSeparateInvoice").attr("checked", true);
                    verModeRadio1 = conModeRadio1rdoSeparateInvoice;
                    setEnabledObjectByMode(verModeRadio1);
                }
                if (result.defSelectProcessType == conModeRadio1rdoCombineInvoice) {
                    $("#rdoCombineInvoice").attr("checked", true);
                    verModeRadio1 = conModeRadio1rdoCombineInvoice;
                    setEnabledObjectByMode(verModeRadio1);
                }

                if (result.defSelectProcessType == conModeRadio1rdoIssueSaleInvoice) {
                    $("#rdoIssueSaleInvoice").attr("checked", true);
                    verModeRadio1 = conModeRadio1rdoIssueSaleInvoice;
                    setEnabledObjectByMode(verModeRadio1);
                }

                $("#rdoSeparateInvoice").attr("disabled", true);
                $("#rdoCombineInvoice").attr("disabled", true);
                $("#rdoIssueSaleInvoice").attr("disabled", true);

                $("#txtSelSeparateFromInvoiceNo").attr("readonly", true);
                $("#txtSelCombineToInvoiceNo").attr("readonly", true);
                $("#txtSelContractCode").attr("readonly", true);
                $("#txtSelSaleOCC").attr("readonly", true);
            }
            else {
                VaridateCtrl(controls, controls);
            }
        });


}

function BLS071Object() {
    var objArr = new Array();
    var objDetail;
    var obj = {
        "BillingTargetCode": $("#txtComBillingTargetCode").val(),
        "FullNameEN": $("#txtComBliiingClientNameEN").val(),
        "FullNameLC": $("#txtComBliiingClientNameLC").val()
    };

    if (doGetBillingDetailOfInvoiceList != null) {
        for (var i = 0; i < doGetBillingDetailOfInvoiceList.length; ++i) {

            objDetail = {
                ContractCode: doGetBillingDetailOfInvoiceList[i].ContractCode,
                BillingOCC: doGetBillingDetailOfInvoiceList[i].BillingOCC,
                BillingDetailNo: doGetBillingDetailOfInvoiceList[i].BillingDetailNo,
                BillingTypeCode: doGetBillingDetailOfInvoiceList[i].BillingTypeCode
            };

            objArr.push(objDetail);
        }
    }

    

    var objBLS071 = {
        currency: currency.VatCurrency,
        currencyCode: currency.VatCurrencyCode,
        doBillingTarget: obj,
        doSelectedBillingDetailList: objArr,
        dtOldBillingDetailList: dtOldBillingDetailList
    };

    return objBLS071;

}

function BLS071Response(result) {

    doGetBillingDetailOfInvoiceList = result;
    dtNewBillingDetailList = result;

    $("#dlgBLS070").CloseDialog();

    //DeleteAllRow(grdCombineInvoiceDetailGrid);
    var listDeleteRowID = [];
    for (var i = 0; i < grdCombineInvoiceDetailGrid.getRowsNum(); i++) {
        var isAddNew_colIndex = grdCombineInvoiceDetailGrid.getColIndexById('isAddNew');
        var isAddNew = grdCombineInvoiceDetailGrid.cells2(i, isAddNew_colIndex).getValue();

        if (isAddNew == "true") {
            var row_id = grdCombineInvoiceDetailGrid.getRowId(i);
            listDeleteRowID.push(row_id);

        }
    }

    // delete only new row (isAddNew) == Narupon
    for (var i = 0; i < listDeleteRowID.length; i++) {
        grdCombineInvoiceDetailGrid.deleteRow(listDeleteRowID[i]);
    }

    if (doGetBillingDetailOfInvoiceList != null) {
        for (var i = 0; i < doGetBillingDetailOfInvoiceList.length; ++i) {
            Add_CombineInvoiceDetailLine(doGetBillingDetailOfInvoiceList[i], "true");
        }
    };
    grdCombineInvoiceDetailGrid.setSizes();

    UpdateCombineInvoice();
    grdCombineInvoiceGrid.setSizes();

}

// Form Mode Section

var conModeInit = 0;
var conModeView = 1;
var conModeEdit = 2;
var conModeConfirm = 9;

var conModeRadio1rdoSeparateInvoice = 1;
var conModeRadio1rdoCombineInvoice = 2;
var conModeRadio1rdoIssueSaleInvoice = 3;

var verModeRadio1 = 1;

var conNo = 0;
var conYes = 1;


function setFormMode(intMode) {
    // ModeInit
    if (intMode == conModeInit) {
        register_command.SetCommand(null);
        reset_command.SetCommand(null);

        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }

    // ModeView = 1;

    if (intMode == conModeView) {
        register_command.SetCommand(btn_Register_click);
        reset_command.SetCommand(btn_Reset_click);

        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }

    // ModeEdit = 2;

    if (intMode == conModeEdit) {
        register_command.SetCommand(null);
        reset_command.SetCommand(null);

        confirm_command.SetCommand(btn_Confirm_click);
        back_command.SetCommand(btn_Back_click);
    }

    // ModeConfirm = 9;

    if (intMode == conModeConfirm) {
        register_command.SetCommand(null);
        reset_command.SetCommand(null);

        confirm_command.SetCommand(btn_Confirm_click);
        back_command.SetCommand(btn_Back_click);
    }
}


// Mode Event
function btn_Register_click() {
    command_control.CommandControlMode(false); //Add by Jutarat A. on 25122013

    if (verModeRadio1 == conModeRadio1rdoSeparateInvoice) {
        Register_SeparateInvoice();
    }


    if (verModeRadio1 == conModeRadio1rdoCombineInvoice) {

        Register_ConbineInvoice();
    }

    if (verModeRadio1 == conModeRadio1rdoIssueSaleInvoice) {
        // check all input on Server
        var obj = GetUserAdjustData();

        ajax_method.CallScreenController("/Billing/BLS070_Register", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                if (result != "0") {

                    if (result.bAlertConfirmDialog == true) {

                        // OpenYesNoMessageDialog
                        var message;
                        var param = { "module": "Billing", "code": "MSG6072" };
                        call_ajax_method("/Shared/GetMessage", param, function (data) {

                            /* ====== Open confirm dialog =====*/
                            OpenYesNoMessageDialog(data.Code, data.Message, function () {
                                ajax_method.CallScreenController("/Billing/BLS070_UpdateIssuePartialFeeFlag", "", function () {

                                    // goto confirm state

                                    setFormMode(conModeEdit);

                                    $("#divSelectProcess").SetViewMode(true);
                                    $("#divSeparateInvoice").SetViewMode(true);
                                    $("#divCombineInvoice").SetViewMode(true);
                                    $("#divIssueSaleInvoice").SetViewMode(true);

                                });
                            });
                        });

                    } else {
                        // goto confirm state

                        setFormMode(conModeEdit);

                        $("#divSelectProcess").SetViewMode(true);
                        $("#divSeparateInvoice").SetViewMode(true);
                        $("#divCombineInvoice").SetViewMode(true);
                        $("#divIssueSaleInvoice").SetViewMode(true);
                    }

                }
            }

            if (controls != undefined) {
                VaridateCtrl(controls, controls);

            }

            command_control.CommandControlMode(true); //Add by Jutarat A. on 25122013
        });
    }



}

function Register_SeparateInvoice() {

    // check all input on Server
    var obj = GetUserAdjustData();

    ajax_method.CallScreenController("/Billing/BLS070_Register", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            if (result != "0") {


                if (result.Alert_SeparateAll) {

                    var objMsg = {
                        module: "Billing",
                        code: "MSG6070"
                    };

                    ajax_method.CallScreenController("/Shared/GetMessage", objMsg, function (data) {
                        OpenOkCancelDialog(data.Code, data.Message, function () {
                            SeparateInvoice();
                        });
                    });

                } else {
                    SeparateInvoice();
                }



            }
        }

        if (controls != undefined) {
            VaridateCtrl(controls, controls);

        }

        command_control.CommandControlMode(true); //Add by Jutarat A. on 25122013
    });

}

function SeparateInvoice() {

    if ($("#chkSepNotChangeInvoiceNo").prop('checked')) {
        var objMsg = {
            module: "Billing",
            code: "MSG6066"
        };

        ajax_method.CallScreenController("/Shared/GetMessage", objMsg, function (data) {
            OpenOkCancelDialog(data.Code, data.Message, function () {
                // goto confirm state

                setFormMode(conModeEdit);

                $("#divSelectProcess").SetViewMode(true);
                $("#divSeparateInvoice").SetViewMode(true);
                $("#divCombineInvoice").SetViewMode(true);
                $("#divIssueSaleInvoice").SetViewMode(true);

            });
        });


    } else {

        // goto confirm state

        setFormMode(conModeEdit);

        $("#divSelectProcess").SetViewMode(true);
        $("#divSeparateInvoice").SetViewMode(true);
        $("#divCombineInvoice").SetViewMode(true);
        $("#divIssueSaleInvoice").SetViewMode(true);
    }

}

function Register_ConbineInvoice() {

    // check all input on Server
    var obj = GetUserAdjustData();

    ajax_method.CallScreenController("/Billing/BLS070_Register", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            if (result != "0") {

                if ($("#chkComNotChangeInvoiceNo").prop('checked')) {

                    var objMsg = {
                        module: "Billing",
                        code: "MSG6069"
                    };

                    ajax_method.CallScreenController("/Shared/GetMessage", objMsg, function (data) {
                        OpenOkCancelDialog(data.Code, data.Message, function () {

                            // goto confirm state

                            setFormMode(conModeEdit);

                            $("#divSelectProcess").SetViewMode(true);
                            $("#divSeparateInvoice").SetViewMode(true);
                            $("#divCombineInvoice").SetViewMode(true);
                            $("#divIssueSaleInvoice").SetViewMode(true);

                        });
                    });
                } else {

                    // goto confirm state

                    setFormMode(conModeEdit);

                    $("#divSelectProcess").SetViewMode(true);
                    $("#divSeparateInvoice").SetViewMode(true);
                    $("#divCombineInvoice").SetViewMode(true);
                    $("#divIssueSaleInvoice").SetViewMode(true);
                }


            }
        }

        if (controls != undefined) {
            VaridateCtrl(controls, controls);

        }

        command_control.CommandControlMode(true); //Add by Jutarat A. on 25122013
    });

}


// create all send to server data for check mendatory and save (in case all input data is ok)
function GetUserAdjustData() {

    var arr1 = new Array();
    var arr2 = new Array();

    var header = {
        rdoProcessSelect: verModeRadio1,

        txtSelSeparateFromInvoiceNo: $("#txtSelSeparateFromInvoiceNo").val(),
        txtSelCombineToInvoiceNo: $("#txtSelCombineToInvoiceNo").val(),
        txtSelContractCode: $("#txtSelContractCode").val(),
        txtSelSaleOCC: $("#txtSelSaleOCC").val()
    };

    var details1 = {
        txtSepBillingTargetCode: $("#txtSepBillingTargetCode").val(),
        txtSepBliiingClientNameEN: $("#txtSepBliiingClientNameEN").val(),
        txtSepBliiingClientNameLC: $("#txtSepBliiingClientNameLC").val(),
        txtSepInvoiceNo: $("#txtSepInvoiceNo").val(),
        chkSepNotChangeInvoiceNo: $("#chkSepNotChangeInvoiceNo").prop('checked'),
        cboPaymentMethodsOfSeparateFrom: $("#cboPaymentMethodsOfSeparateFrom").val(),
        cboIssueInvoiceAfterSeparate: $("#cboIssueInvoiceAfterSeparate").val()
    };

    var details2 = {
        txtComBillingTargetCode: $("#txtComBillingTargetCode").val(),
        txtComBliiingClientNameEN: $("#txtComBliiingClientNameEN").val(),
        txtComBliiingClientNameLC: $("#txtComBliiingClientNameLC").val(),

        txtComInvoiceNo: $("#txtComInvoiceNo").val(),
        chkComNotChangeInvoiceNo: $("#chkComNotChangeInvoiceNo").prop('checked'),
        cboPaymentMethodsOfCombineToInvoice: $("#cboPaymentMethodsOfCombineToInvoice").val(),
        cboIssueInvoiceAfterCombine: $("#cboIssueInvoiceAfterCombine").val()
    };

    var details3 = {
        txtIssContractCode: $("#txtIssContractCode").val(),
        txtIssSaleOCC: $("#txtIssSaleOCC").val(),
        txtBillingAmount: $("#txtBillingAmount").val().replace(currency.VatCurrency, ''),
        txtVATAmount: $("#txtVATAmount").val().replace(currency.VatCurrency, ''),
        txtWHTAmount: $("#txtWHTAmount").val().replace(currency.VatCurrency, ''),
        dtpCustomerAcceptanceDate: $("#dtpCustomerAcceptanceDate").val()
    };

    var details4 = {
        txtIssContractCode: $("#txtIssContractCode").val(),
        txtIssSaleOCC: $("#txtIssSaleOCC").val(),
        txtBillingAmount: $("#txtBillingAmount2").val().replace(currency.VatCurrency, ''),
        txtVATAmount: $("#txtVATAmount2").val().replace(currency.VatCurrency, ''),
        txtWHTAmount: $("#txtWHTAmount2").val().replace(currency.VatCurrency, ''),
        dtpCustomerAcceptanceDate: $("#dtpCustomerAcceptanceDate").val()
    };

    if (CheckFirstRowIsEmpty(grdSeparateInvoiceDetailGrid) == false) {

        for (var i = 0; i < grdSeparateInvoiceDetailGrid.getRowsNum(); i++) {

            var row_id = grdSeparateInvoiceDetailGrid.getRowId(i);
            // custom object in grid use object name
            var clt1 = "#" + GenerateGridControlID("chkSelectSeparateDetail", row_id);
            var clt7 = "#" + GenerateGridControlID("cboIssueInvoiceofSeparateDetail", row_id);
            // non custom object in grid use object name
            // use column name in XML not declare ass txt, dtp ect

            var obj1 = {

                chkSelectSeparateDetail: $(clt1).prop('checked'),
                BillingCode: grdSeparateInvoiceDetailGrid.cells(row_id,
                                        grdSeparateInvoiceDetailGrid.getColIndexById("BillingCode")).getValue(),
                RunningNo: grdSeparateInvoiceDetailGrid.cells(row_id,
                                        grdSeparateInvoiceDetailGrid.getColIndexById("RunningNo")).getValue(),
                BillingType: grdSeparateInvoiceDetailGrid.cells(row_id,
                                        grdSeparateInvoiceDetailGrid.getColIndexById("BillingType")).getValue(),
                BillingAmount: grdSeparateInvoiceDetailGrid.cells(row_id,
                                        grdSeparateInvoiceDetailGrid.getColIndexById("BillingAmount")).getValue().replace(currency.VatCurrency, ''),
                SiteName: grdSeparateInvoiceDetailGrid.cells(row_id,
                                        grdSeparateInvoiceDetailGrid.getColIndexById("SiteName")).getValue(),
                cboIssueInvoiceofSeparateDetail: $(clt7).val(),

                ContractCode: grdSeparateInvoiceDetailGrid.cells(row_id,
                                        grdSeparateInvoiceDetailGrid.getColIndexById("ContractCode")).getValue(),  // --**
                BillingOCC: grdSeparateInvoiceDetailGrid.cells(row_id,
                                        grdSeparateInvoiceDetailGrid.getColIndexById("BillingOCC")).getValue(),  // --**
                BillingDetailNo: grdSeparateInvoiceDetailGrid.cells(row_id,
                                        grdSeparateInvoiceDetailGrid.getColIndexById("BillingDetailNo")).getValue()  // --**

            };

            arr1.push(obj1);

        }
    }


    var detail1 = arr1;

    if (CheckFirstRowIsEmpty(grdCombineInvoiceDetailGrid) == false) {

        for (var i = 0; i < grdCombineInvoiceDetailGrid.getRowsNum(); i++) {

            var row_id = grdCombineInvoiceDetailGrid.getRowId(i);
            // custom object in grid use object name
            //var clt1 = "#" + GenerateGridControlID("chkSelectSeparateDetail", row_id);
            // non custom object in grid use object name
            // use column name in XML not declare ass txt, dtp ect

            var obj2 = {

                BillingCode: grdCombineInvoiceDetailGrid.cells(row_id,
                                        grdCombineInvoiceDetailGrid.getColIndexById("BillingCode")).getValue(),
                RunningNo: grdCombineInvoiceDetailGrid.cells(row_id,
                                        grdCombineInvoiceDetailGrid.getColIndexById("RunningNo")).getValue(),
                BillingType: grdCombineInvoiceDetailGrid.cells(row_id,
                                        grdCombineInvoiceDetailGrid.getColIndexById("BillingType")).getValue(),
                BillingAmount: grdCombineInvoiceDetailGrid.cells(row_id,
                                        grdCombineInvoiceDetailGrid.getColIndexById("BillingAmount")).getValue().replace(currency.VatCurrency, ''),
                SiteName: grdCombineInvoiceDetailGrid.cells(row_id,
                                        grdCombineInvoiceDetailGrid.getColIndexById("SiteName")).getValue(),

                ContractCode: grdCombineInvoiceDetailGrid.cells(row_id,
                                        grdCombineInvoiceDetailGrid.getColIndexById("ContractCode")).getValue(), //--**
                BillingOCC: grdCombineInvoiceDetailGrid.cells(row_id,
                                        grdCombineInvoiceDetailGrid.getColIndexById("BillingOCC")).getValue(), //--**
                BillingDetailNo: grdCombineInvoiceDetailGrid.cells(row_id,
                                        grdCombineInvoiceDetailGrid.getColIndexById("BillingDetailNo")).getValue() //--**

            };

            arr2.push(obj2);
        }
    }

    var detail2 = arr2;


    var returnObj = {
        Header: header,
        Details1: details1,
        Details2: details2,
        Details3: details3,
        Details4: details4,
        Detail1: detail1,
        Detail2: detail2,
        NewBillingDetail: dtNewBillingDetailList
    };

    return returnObj;

}


function btn_Reset_click() {



    //    /* --- Get Message --- */
    //    var obj = {
    //        module: "Common",
    //        code: "MSG0038"
    //    };
    //    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
    //        OpenOkCancelDialog(result.Code, result.Message,
    //        function () {

    //            setVisableTable(0);
    //            setFormMode(conModeInit);

    //            $("#divSelectProcess").SetViewMode(false);
    //            $("#divSeparateInvoice").SetViewMode(false);
    //            $("#divCombineInvoice").SetViewMode(false);
    //            $("#divIssueSaleInvoice").SetViewMode(false);

    //            $("#divSelectProcess").ResetToNormalControl(true);
    //            $("#divSeparateInvoice").ResetToNormalControl(true);
    //            $("#divCombineInvoice").ResetToNormalControl(true);
    //            $("#divIssueSaleInvoice").ResetToNormalControl(true);

    //            ClearScreenToInitStage();

    //            // tt
    //            dtNewBillingDetailList = null;

    //        },
    //        null);
    //    });



    setVisableTable(0);
    setFormMode(conModeInit);

    $("#divSelectProcess").SetViewMode(false);
    $("#divSeparateInvoice").SetViewMode(false);
    $("#divCombineInvoice").SetViewMode(false);
    $("#divIssueSaleInvoice").SetViewMode(false);

    $("#divSelectProcess").ResetToNormalControl(true);
    $("#divSeparateInvoice").ResetToNormalControl(true);
    $("#divCombineInvoice").ResetToNormalControl(true);
    $("#divIssueSaleInvoice").ResetToNormalControl(true);

    ClearScreenToInitStage();

    // tt
    dtNewBillingDetailList = null;

}
function btn_Approve_click() {
}
function btn_Reject_click() {
}
function btn_Return_click() {
}
function btn_Close_click() {
}
function btn_Confirm_click() {
    command_control.CommandControlMode(false); //Add by Jutarat A. on 25122013

    DisableConfirmCommand(true);
    DisableBackCommand(true);
    // save data
    ajax_method.CallScreenController("/Billing/BLS070_Confirm", "",
        function (result, controls, isWarning) {
            if (result != undefined) {
                if (result != "0") {

                    // goto confirm state

                    setFormMode(conModeInit);

                    $("#divSelectProcess").SetViewMode(false);
                    $("#divSeparateInvoice").SetViewMode(false);
                    $("#divCombineInvoice").SetViewMode(false);
                    $("#divIssueSaleInvoice").SetViewMode(false);

                    $("#divSelectProcess").ResetToNormalControl(true);
                    $("#divSeparateInvoice").ResetToNormalControl(true);
                    $("#divCombineInvoice").ResetToNormalControl(true);
                    $("#divIssueSaleInvoice").ResetToNormalControl(true);

                    ClearScreenToInitStage();

                    DisableConfirmCommand(false);
                    DisableBackCommand(false);

                    // Success
                    var objMsg = {
                        module: "Common",
                        code: "MSG0046"
                    };

                    call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                        OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {
                            // In case real time print , result.isIssue will equal true

                            if (result.isIssue == true) {
                                if (result.strFilePath != "") {

                                    var filenameObj = { "fileName": result.strFilePath };
                                    ajax_method.CallScreenController("/Billing/BLS070_CheckExistFile", filenameObj, function (result2) {
                                        if (result2 != undefined) {

                                            if (result2 == "1") {
                                                var key = ajax_method.GetKeyURL(null);
                                                var url = ajax_method.GenerateURL("/Billing/BLS070_GetInvoiceReport?k=" + key + "&fileName=" + result.strFilePath);
                                                window.open(url, "", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
                                            }
                                            else {
                                                // alert file not found !!
                                            }

                                        }

                                    });

                                }

                            }

                        }); // end show information message
                    });

                }
            }

            command_control.CommandControlMode(true); //Add by Jutarat A. on 25122013
        });

}

function btn_Back_click() {
    setFormMode(conModeView);

    $("#divSelectProcess").SetViewMode(false);
    $("#divSeparateInvoice").SetViewMode(false);
    $("#divCombineInvoice").SetViewMode(false);
    $("#divIssueSaleInvoice").SetViewMode(false);

    $("#divSelectProcess").ResetToNormalControl(true);
    $("#divSeparateInvoice").ResetToNormalControl(true);
    $("#divCombineInvoice").ResetToNormalControl(true);
    $("#divIssueSaleInvoice").ResetToNormalControl(true);

    // disable radio button
    $("#rdoSeparateInvoice").attr("disabled", true);
    $("#rdoCombineInvoice").attr("disabled", true);
    $("#rdoIssueSaleInvoice").attr("disabled", true);

}

// Clear Screen
function ClearScreenToInitStage() {

    $("#rdoSeparateInvoice").attr("disabled", false);
    $("#rdoCombineInvoice").attr("disabled", false);
    $("#rdoIssueSaleInvoice").attr("disabled", false);

    $("#rdoSeparateInvoice").attr("checked", true);
    //$("#rdoCombineInvoice").attr("checked", true);
    //$("#rdoIssueSaleInvoice").attr("checked", true);

    $("#txtSelSeparateFromInvoiceNo").attr("readonly", false);
    $("#txtSelCombineToInvoiceNo").attr("readonly", false);
    $("#txtSelContractCode").attr("readonly", false);
    $("#txtSelSaleOCC").attr("readonly", false);

    $("#txtSelSeparateFromInvoiceNo").val("");
    $("#txtSelCombineToInvoiceNo").val("");
    $("#txtSelContractCode").val("");
    $("#txtSelSaleOCC").val("");

    $("#txtSepBillingTargetCode").val("");
    $("#txtSepBliiingClientNameEN").val("");
    $("#txtSepBliiingClientNameLC").val("");

    $("#txtSepInvoiceNo").val("");
    $("#chkSepNotChangeInvoiceNo").attr("checked", false);
    $("#cboPaymentMethodsOfSeparateFrom").val("");
    $("#cboIssueInvoiceAfterSeparate").val("");

    $("#txtComBillingTargetCode").val("");
    $("#txtComBliiingClientNameEN").val("");
    $("#txtComBliiingClientNameLC").val("");

    $("#txtComInvoiceNo").val("");
    $("#chkComNotChangeInvoiceNo").attr("checked", false);
    $("#cboPaymentMethodsOfCombineToInvoice").val("");
    $("#cboIssueInvoiceAfterCombine").val("");

    $("#txtIssContractCode").val("");
    $("#txtIssSaleOCC").val("");

    $("#txtBillingAmount").val("");
    $("#txtVATAmount").val("");
    $("#txtWHTAmount").val("");

    $("#dtpCustomerAcceptanceDate").attr("disabled", false);
    $("#dtpCustomerAcceptanceDate").val("");

    $("#btnRetrieve").attr("disabled", false);
    $("#btnCallScreenToSelectCombileDetail").attr("disabled", false);

    DeleteAllRow(grdCombineInvoiceGrid);
    DeleteAllRow(grdCombineInvoiceDetailGrid);
    DeleteAllRow(grdSeparateInvoiceGrid);
    DeleteAllRow(grdSeparateInvoiceDetailGrid);

    verModeRadio1 = conModeRadio1rdoSeparateInvoice;

    setVisableTable(0);
    setEnabledObjectByMode(verModeRadio1);
    bolAlreadyAddNewRowToGRID1 = false;
    bolAlreadyAddNewRowToGRID2 = false;

    $("#divSeparateInv_sub").clearForm();
    $("#divCombineInv_sub").clearForm();
}
// Enable Obj On Screen

function setEnabledObjectByMode(intMode) {

    if (intMode == conModeRadio1rdoSeparateInvoice) {
        $("#txtSelSeparateFromInvoiceNo").attr("readonly", false);
        $("#txtSelCombineToInvoiceNo").attr("readonly", true);
        $("#txtSelContractCode").attr("readonly", true);
        $("#txtSelSaleOCC").attr("readonly", true);
    }
    else if (intMode == conModeRadio1rdoCombineInvoice) {
        $("#txtSelSeparateFromInvoiceNo").attr("readonly", true);
        $("#txtSelCombineToInvoiceNo").attr("readonly", false);
        $("#txtSelContractCode").attr("readonly", true);
        $("#txtSelSaleOCC").attr("readonly", true);
    }
    else {
        $("#txtSelSeparateFromInvoiceNo").attr("readonly", true);
        $("#txtSelCombineToInvoiceNo").attr("readonly", true);
        $("#txtSelContractCode").attr("readonly", false);
        $("#txtSelSaleOCC").attr("readonly", false);
    };

}

// Visable Obj On Screen
function setVisableTable(intMode) {

    if (intMode == conModeRadio1rdoSeparateInvoice) {
        $("#divSelectProcess").show();
        $("#divSeparateInvoice").show();
        $("#divCombineInvoice").hide();
        $("#divIssueSaleInvoice").hide();
    }
    else if (intMode == conModeRadio1rdoCombineInvoice) {
        $("#divSelectProcess").show();
        $("#divSeparateInvoice").hide();
        $("#divCombineInvoice").show();
        $("#divIssueSaleInvoice").hide();
    }
    else if (intMode == conModeRadio1rdoIssueSaleInvoice) {
        $("#divSelectProcess").show();
        $("#divSeparateInvoice").hide();
        $("#divCombineInvoice").hide();
        $("#divIssueSaleInvoice").show();
    }
    else {
        $("#divSelectProcess").show();
        $("#divSeparateInvoice").hide();
        $("#divCombineInvoice").hide();
        $("#divIssueSaleInvoice").hide();
    };

}

var bolAlreadyAddNewRowToGRID1 = false;
var bolAlreadyAddNewRowToGRID2 = false;

var doGetBillingDetailOfInvoiceList = null;

function btn_Retrieve_click() {

    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Billing/BLS070_RetrieveData", obj, function (result, controls, isWarning) {
        if (result != undefined) {

            // Add by Jirawat Jannet @ 2016-08-24
            currency.VatCurrencyCode = result.VatCurrencyCode;
            currency.WHTCurrencyCode = result.WHTCurrencyCode;
            currency.VatCurrency = result.VatCurrency;
            currency.WHTCurrency = result.WHTCurrency;
            // End add

            // set mode for open dynamic gen object in GRID
            setFormMode(conModeView);
            setVisableTable(verModeRadio1);


            if (verModeRadio1 == conModeRadio1rdoSeparateInvoice) {
                //$("#txtSelSeparateFromInvoiceNo").val("");
                $("#txtSelCombineToInvoiceNo").val("");
                $("#txtSelContractCode").val("");
                $("#txtSelSaleOCC").val("");

                $("#txtSepBillingTargetCode").val(result.doGetInvoiceWithBillingClientName.BillingTargetCodeShortFormat);
                $("#txtSepBliiingClientNameEN").val(result.doGetInvoiceWithBillingClientName.BillingClientNameEN);
                $("#txtSepBliiingClientNameLC").val(result.doGetInvoiceWithBillingClientName.BillingClientNameLC);

                $("#txtSepInvoiceNo").val(result.doGetInvoiceWithBillingClientName.InvoiceNo);

                if (result.decInvoiceTotal != null) {
                    Add_SeparateInvoiceLine(result.intInvoiceCount
                                    , result.decInvoiceTotal
                                    , currency.VatCurrency
                                    , result.intInvoiceCount
                                    , result.decInvoiceTotal
                                    , currency.VatCurrency);
                };

                if (result.doGetBillingDetailOfInvoiceList != null) {
                    for (var i = 0; i < result.doGetBillingDetailOfInvoiceList.length; ++i) {
                        Add_SeparateInvoiceDetailLine(result.doGetBillingDetailOfInvoiceList[i]);
                    }
                };
                grdSeparateInvoiceDetailGrid.setSizes();
            }

            if (verModeRadio1 == conModeRadio1rdoCombineInvoice) {

                $("#txtSelSeparateFromInvoiceNo").val("");
                //$("#txtSelCombineToInvoiceNo").val("");
                $("#txtSelContractCode").val("");
                $("#txtSelSaleOCC").val("");

                $("#txtComBillingTargetCode").val(result.doGetInvoiceWithBillingClientName.BillingTargetCodeShortFormat);
                $("#txtComBliiingClientNameEN").val(result.doGetInvoiceWithBillingClientName.BillingClientNameEN);
                $("#txtComBliiingClientNameLC").val(result.doGetInvoiceWithBillingClientName.BillingClientNameLC);

                $("#txtComInvoiceNo").val(result.doGetInvoiceWithBillingClientName.InvoiceNo);

                if (result.decInvoiceTotal != null) {
                    Add_CombineInvoiceLine(result.intInvoiceCount
                                    , result.decInvoiceTotal
                                    , currency.VatCurrency
                                    , result.intInvoiceCount
                                    , result.decInvoiceTotal
                                    , currency.VatCurrency);
                };

                if (result.doGetBillingDetailOfInvoiceList != null) {
                    for (var i = 0; i < result.doGetBillingDetailOfInvoiceList.length; ++i) {
                        Add_CombineInvoiceDetailLine(result.doGetBillingDetailOfInvoiceList[i] ,true);
                    }

                    doGetBillingDetailOfInvoiceList = result.doGetBillingDetailOfInvoiceList;

                    // Narupon
                    dtOldBillingDetailList = result.doGetBillingDetailOfInvoiceList;
                };
                grdCombineInvoiceDetailGrid.setSizes();

                UpdateCombineInvoice();
                grdCombineInvoiceGrid.setSizes();
            }

            if (verModeRadio1 == conModeRadio1rdoIssueSaleInvoice) {

                //$("#txtSelSeparateFromInvoiceNo").val("");
                //$("#txtSelCombineToInvoiceNo").val("");

                // Narupon
                //$("#txtSelContractCode").val("");
                //$("#txtSelSaleOCC").val("");

                $("#txtIssContractCode").val(result.ContractCode_short);
                $("#txtIssSaleOCC").val(result.doGetSaleDataForIssueInvoice.OCC);

                var numformat = "#,###.00";
                var BillingAmountCurrency = result.doGetSaleDataForIssueInvoice[0].BillingAmtCurrencyType;
                if (BillingAmountCurrency == C_CURRENCY_US)
                    var BillingAmt_val = (new Number(result.doGetSaleDataForIssueInvoice[0].BillingAmtUsd)).numberFormat(numformat ? numformat : "");
                else
                    var BillingAmt_val = (new Number(result.doGetSaleDataForIssueInvoice[0].BillingAmt)).numberFormat(numformat ? numformat : "");

                var InstallationAmtCurrency = result.doGetSaleDataForIssueInvoice[1].BillingAmtCurrencyType;
                if (InstallationAmtCurrency == C_CURRENCY_US)
                    var InstallationAmt_val = (new Number(result.doGetSaleDataForIssueInvoice[1].BillingAmtUsd)).numberFormat(numformat ? numformat : "");
                else
                    var InstallationAmt_val = (new Number(result.doGetSaleDataForIssueInvoice[1].BillingAmt)).numberFormat(numformat ? numformat : "");
                
                $("#txtBillingAmount").NumericCurrency().val(BillingAmountCurrency);
                $("#txtBillingAmount").val(BillingAmt_val);
                $("#txtBillingAmount2").NumericCurrency().val(InstallationAmtCurrency);
                $("#txtBillingAmount2").val(InstallationAmt_val);
                $("#txtVATAmount").NumericCurrency().val(currency.VatCurrencyCode);
                $("#txtVATAmount").val(result.txtVATAmount);
                $("#txtWHTAmount").NumericCurrency().val(currency.WHTCurrencyCode);
                $("#txtWHTAmount").val(result.txtWHTAmount);
                $("#txtVATAmount2").NumericCurrency().val(currency.VatCurrencyCode);
                $("#txtVATAmount2").val(result.txtVATAmount);
                $("#txtWHTAmount2").NumericCurrency().val(currency.WHTCurrencyCode);
                $("#txtWHTAmount2").val(result.txtWHTAmount);
            }

            $("#rdoSeparateInvoice").attr("disabled", true);
            $("#rdoCombineInvoice").attr("disabled", true);
            $("#rdoIssueSaleInvoice").attr("disabled", true);

            $("#txtSelSeparateFromInvoiceNo").attr("readonly", true);
            $("#txtSelCombineToInvoiceNo").attr("readonly", true);
            $("#txtSelContractCode").attr("readonly", true);
            $("#txtSelSaleOCC").attr("readonly", true);

            $("#chkSepNotChangeInvoiceNo").attr("checked", false);
            $("#chkComNotChangeInvoiceNo").attr("checked", false);

            $("#btnRetrieve").attr("disabled", true);

        }
        else {
            // after show Error MSG Layer on Right on Screen then go to Init Mode
            setFormMode(conModeInit);
            VaridateCtrl(controls, controls);
        }
    });

}



function btn_Select_Process_click() {

    setFormMode(conModeView);
    setVisableTable(verModeRadio1);

    if (verModeRadio2 == conModeRadio2rdoByBillingDetails) {

        if (!(bolAlreadyAddNewRowToGRID1)) {

            Add_ByBillingBlankLine();
            Add_ByBillingBlankLine();
            Add_ByBillingBlankLine();
            bolAlreadyAddNewRowToGRID1 = true;
        }

    } else {
        if (!(bolAlreadyAddNewRowToGRID2)) {

            Add_ByInvoiceBlankLine();
            Add_ByInvoiceBlankLine();
            Add_ByInvoiceBlankLine();
            bolAlreadyAddNewRowToGRID2 = true;
        }

    }
    $("#rdoSwitchToAutoTransferCreditCard").attr("disabled", true);
    $("#rdoStopAutoTransferCreditCard").attr("disabled", true);
    $("#rdoChangeExpectedIssueDateAutoTransferDate").attr("disabled", true);

    $("#rdoByBillingDetails").attr("disabled", true);
    $("#rdoByInvoice").attr("disabled", true);

    $("#btnSelectProcess").attr("disabled", true);
}

// Radio Select

function rdoSeparateInvoice_Select() {
    verModeRadio1 = conModeRadio1rdoSeparateInvoice;
    setEnabledObjectByMode(verModeRadio1);

    // Waroon
    //    //$("#txtSelSeparateFromInvoiceNo").val("");
    //    $("#txtSelCombineToInvoiceNo").val("");
    //    $("#txtSelContractCode").val("");
    //    $("#txtSelSaleOCC").val("");


    // Narupon
    $("#txtSelSeparateFromInvoiceNo").val("");
    $("#txtSelCombineToInvoiceNo").val("");
    $("#txtSelContractCode").val("");
    $("#txtSelSaleOCC").val("");

    $("#txtSelSeparateFromInvoiceNo").focus();

}
function rdoCombineInvoice_Select() {
    verModeRadio1 = conModeRadio1rdoCombineInvoice;
    setEnabledObjectByMode(verModeRadio1);

    // Waroon
    //    $("#txtSelSeparateFromInvoiceNo").val("");
    //    //$("#txtSelCombineToInvoiceNo").val("");
    //    $("#txtSelContractCode").val("");
    //    $("#txtSelSaleOCC").val("");

    // Narupon
    $("#txtSelSeparateFromInvoiceNo").val("");
    $("#txtSelCombineToInvoiceNo").val("");
    $("#txtSelContractCode").val("");
    $("#txtSelSaleOCC").val("");

    $("#txtSelCombineToInvoiceNo").focus();
}
function rdoIssueSaleInvoice_Select() {
    verModeRadio1 = conModeRadio1rdoIssueSaleInvoice;
    setEnabledObjectByMode(verModeRadio1);

    // Waroon
    //    //$("#txtSelSeparateFromInvoiceNo").val("");
    //    //$("#txtSelCombineToInvoiceNo").val("");
    //    $("#txtSelContractCode").val("");
    //    $("#txtSelSaleOCC").val("");

    // Narupon
    $("#txtSelSeparateFromInvoiceNo").val("");
    $("#txtSelCombineToInvoiceNo").val("");
    $("#txtSelContractCode").val("");
    $("#txtSelSaleOCC").val("");

    $("#txtSelContractCode").focus();


}

function ConvertNumber(number, format) {
    var tail = format.lastIndexOf('.'); number = number.toString();
    tail = tail > -1 ? format.substr(tail) : '';
    if (tail.length > 0) {
        if (tail.charAt(1) == '#') {
            tail = number.substr(number.lastIndexOf('.'), tail.length);
        }
    }
    number = number.replace(/\..*|[^0-9]/g, '').split('');
    format = format.replace(/\..*/g, '').split('');
    for (var i = format.length - 1; i > -1; i--) {
        if (format[i] == '#') { format[i] = number.pop() }
    }
    return number.join('') + format.join('') + tail;
}

Number.prototype.format = function (n, x) {
    var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\.' : '$') + ')';
    return this.toFixed(Math.max(0, ~~n)).replace(new RegExp(re, 'g'), '$&,');
};

//function number_format(number, decimals, dec_point, thousands_sep) {

//    if (number == '') {
//        number = '0'
//    }

//    if (number == undefined) {
//        number = '0'
//    }

//    number = (number + '').replace(/[^0-9+\-Ee.]/g, '');
//    var n = !isFinite(+number) ? 0 : +number,
//        prec = !isFinite(+decimals) ? 0 : Math.abs(decimals), sep = (typeof thousands_sep === 'undefined') ? ',' : thousands_sep,
//        dec = (typeof dec_point === 'undefined') ? '.' : dec_point,
//        s = '',
//        toFixedFix = function (n, prec) {
//            var k = Math.pow(10, prec); return '' + Math.round(n * k) / k;
//        };
//    // Fix for IE parseFloat(0.55).toFixed(0) = 0;
//    s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.');
//    if (s[0].length > 3) {
//        s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
//    }
//    if ((s[1] || '').length < prec) {
//        s[1] = s[1] || '';
//        s[1] += new Array(prec - s[1].length + 1).join('0');
//    }
//    return s.join(dec);
//}