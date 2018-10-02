
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

/// <reference path="../Base/object/ajax_method.js" />
/// <reference path="../Base/object/command_event.js" />
/// <reference path="../Base/object/master_event.js" />

var pageRow = ROWS_PER_PAGE_FOR_VIEWPAGE;
var grdMoneyCollectionManagementInformationGrid;
var _doGetMoneyCollectionManagementInfoList;

var _strBillingOfficeCode;
var _strBillingOfficeName;
var _doBillingTargetDebtSummaryList;
var _doGetUnpaidInvoiceDebtSummaryByBillingTargetList;
var _doGetUnpaidDetailDebtSummaryByBillingCodeList;
var _doGetBillingCodeDebtSummaryList;
var _doGetDebtTracingMemoList;
var _doGetUnpaidDetailDebtSummary033;

var _InvoiceNo;
var _InvoiceOCC;
var _BillingCode;


var _BillingClientAddressEN;
var _BillingClientAddressLC;
var _BillingClientTelNo;
var _ContactPersonName;

var conYes = "";
var conNo = "";

$(document).ready(function () {
    // ..

    //Init Object Event
    // 1 Div Panel Body
    //    $("#btnSearch").click(btn_Search_click);
    //    $("#btnClear").click(btn_Clear_click);
    //    $("#btnDownloadFile").click(btn_Download_File_click);

    $("#btnBillingTargetViewUnpaidBillingDetail").click(function () {
        var objICS033param = {
            BillingOfficeCode: _strBillingOfficeCode,
            BillingOfficeName: _strBillingOfficeName,
            BillingClientNameEN: $("#txtBillingCilentNameEN").val(),
            BillingClientNameLC: $("#txtBillingCilentNameLC").val(),
            BillingClientAddressEN: _BillingClientAddressEN,
            BillingClientAddressLC: _BillingClientAddressLC,
            BillingClientTelNo: _BillingClientTelNo,
            ContactPersonName: _ContactPersonName,
            
            Mode: "GetByBillingTarget",

            BillingTargetCode: $("#txtBillingTargetCode").val(),
            InvoiceNo: _InvoiceNo,
            InvoiceOCC: _InvoiceOCC,
            BillingCode: _BillingCode
        };
        $("#dlgICS033").OpenICS033Dialog($("#CallerScreen").val(), objICS033param);
    });

    $("#btnInvoiceViewUnpaidBillingDetail").click(function () {
        var objICS033param = {
            BillingOfficeCode: _strBillingOfficeCode,
            BillingOfficeName: _strBillingOfficeName,
            
            BillingClientNameEN: $("#txtBillingCilentNameEN").val(),
            BillingClientNameLC: $("#txtBillingCilentNameLC").val(),
            BillingClientAddressEN: _BillingClientAddressEN,
            BillingClientAddressLC: _BillingClientAddressLC,
            BillingClientTelNo: _BillingClientTelNo,
            ContactPersonName: _ContactPersonName,
            
            Mode: "GetByInvoice",

            BillingTargetCode: $("#txtBillingTargetCode").val(),
            InvoiceNo: _InvoiceNo,
            InvoiceOCC: _InvoiceOCC,
            BillingCode: _BillingCode
        };
        $("#dlgICS033").OpenICS033Dialog($("#CallerScreen").val(), objICS033param);
    });
    
    $("#btnBillingCodeViewUnpaidBillingDetail").click(function () {
        var objICS033param = {
            BillingOfficeCode: _strBillingOfficeCode,
            BillingOfficeName: _strBillingOfficeName,
            
            BillingClientNameEN: $("#txtBillingCilentNameEN").val(),
            BillingClientNameLC: $("#txtBillingCilentNameLC").val(),
            BillingClientAddressEN: _BillingClientAddressEN,
            BillingClientAddressLC: _BillingClientAddressLC,
            BillingClientTelNo: _BillingClientTelNo,
            ContactPersonName: _ContactPersonName,
            
            Mode: "GetByBillingCode",
            
            BillingTargetCode: $("#txtBillingTargetCode").val(),
            InvoiceNo: _InvoiceNo,
            InvoiceOCC: _InvoiceOCC,
            BillingCode: _BillingCode
        };
        $("#dlgICS033").OpenICS033Dialog($("#CallerScreen").val(), objICS033param);
    });

    $("#rdo1Invoice").change(rdo1Invoice_Select);
    $("#rdo1BillingTarget").change(rdo1BillingTarget_Select);
    //Initial Page
    InitialPage();
});

function InitialPage() {
    $("#txtBillingTargetUnpaidAmount").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtBillingTargetUnpaidAmountUsd").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtBillingTargetNumberOfUnpaidDetail").BindNumericBox(10, 2, 0, 999999999999.99);
    $("#txtInvoiceInvoiceAmount").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtInvoiceInvoiceAmountUsd").BindNumericBox(12, 2, 0, 999999999999.99);
    //$("#txtInvoiceInvoiceIssueDate").InitialDate();
    $("#txtInvoiceUnpaidAmount").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtInvoiceUnpaidAmountUsd").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtInvoiceNumberOfUnpaidDetail").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtBillingCodeUnpaidAmount").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtBillingCodeUnpaidAmountUsd").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtBillingCodeNumberOfUnpaidDetail").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#rdo1Invoice").attr("Checked", true);
    //$("#rdo1BillingTarget

    //$("#cboTracingResault cbo
    $("#dtpLastContractDate").InitialDate();
    $("#dtpExpectedPaymentdate").InitialDate();
    //$("#cboPaymentMethods cbo
    // example
    // Date
    //$("#dtpCustomerAcceptanceDate").InitialDate();
    //InitialDateFromToControl("#dptAdjustBillingPeriodDateFrom", "#dptAdjustBillingPeriodDateTo");
    //Text Input
    //$("#txtSelSeparateFromInvoiceNo").attr("maxlength", 12);
    // Number
    //$("#txtBillingAmount").BindNumericBox(10, 2, 0, 9999999999.99);

    $("#txtaMemo").SetMaxLengthTextArea(500);
    
    setVisableTable(conNo);
    setFormMode(conModeView);

    LoadData();
}

function LoadData() {
    var strRegistrant = $("#hidmemoRegistrant").val();
    var strContent = $("#hidmemoContent").val();
    var strTracingResult = $("#hidmemoTracingResult").val();
    var strExpectPaymentDate = $("#hidmemoExpectPaymentDate").val();
    var strLastContactDate = $("#hidmemoLastContactDate").val();
    var strPaymentMethod = $("#hidmemoPaymentMethod").val();
 
    // check all input on Server
    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Income/ICS032_SearchData", obj, function (result, controls, isWarning) {
        if (result != undefined) {

            // goto Idel state
            // Wired design

            $("#divViewRegisterTracingInformation").show();
            $("#divBillingTarget").show();
            $("#divInvoice").show();
            $("#divBillingCode").hide();

            $("#divMemoHistory").show();
            $("#divInput").show();

            if (result.strOpenFromListofUnpaidInvoiceByBillingTarget == 'N') {
                $("#divBillingCode").show();
                $("#divViewRegisterTracingInformationSiteName").show();
            } else {
                $("#divBillingCode").hide();
                $("#divViewRegisterTracingInformationSiteName").hide();
            }

            _strBillingOfficeCode = result.strBillingOfficeCode;
            _strBillingOfficeName = result.strBillingOfficeName;

            _doBillingTargetDebtSummaryList = result.doBillingTargetDebtSummaryList;
            _doGetUnpaidInvoiceDebtSummaryByBillingTargetList = result.doGetUnpaidInvoiceDebtSummaryByBillingTargetList;
            _doGetUnpaidDetailDebtSummaryByBillingCodeList = result.doGetUnpaidDetailDebtSummaryByBillingCodeList;
            _doGetBillingCodeDebtSummaryList = result.doGetBillingCodeDebtSummaryList;

            _doGetDebtTracingMemoList = result.doGetDebtTracingMemoList;


            if (_doBillingTargetDebtSummaryList != null) {
                if (_doBillingTargetDebtSummaryList[0] != undefined) {
                    //2.	Display billing client information
                    //$("#divBillingTarget").show();

                    $("#txtBillingCilentNameEN").val(_doBillingTargetDebtSummaryList[0].BillingClientNameEN);
                    $("#txtBillingCilentNameLC").val(_doBillingTargetDebtSummaryList[0].BillingClientNameLC);
                    $("#txtBillingOffice").val(_strBillingOfficeName);
                    //3.	Display billing target information
                    $("#txtBillingTargetCode").val(_doBillingTargetDebtSummaryList[0].BillingTargetCodeShort);

                    $("#txtBillingTargetUnpaidAmount").val(accounting.toFixed(_doBillingTargetDebtSummaryList[0].UnpaidAmount, 2));
                    $("#txtBillingTargetUnpaidAmount").NumericCurrency().val(ICS032_ViewBag.C_CURRENCY_LOCAL);
                    $("#txtBillingTargetUnpaidAmount").setComma();

                    $("#txtBillingTargetUnpaidAmountUsd").val(accounting.toFixed(_doBillingTargetDebtSummaryList[0].UnpaidAmountUsd, 2));
                    $("#txtBillingTargetUnpaidAmountUsd").NumericCurrency().val(ICS032_ViewBag.C_CURRENCY_US);
                    $("#txtBillingTargetUnpaidAmountUsd").setComma();

                    $("#txtBillingTargetNumberOfUnpaidDetail").val(_doBillingTargetDebtSummaryList[0].UnpaidDetailString);

                    _BillingClientAddressEN = _doBillingTargetDebtSummaryList[0].BillingClientAddressEN;
                    _BillingClientAddressLC = _doBillingTargetDebtSummaryList[0].BillingClientAddressLC;
                    _BillingClientTelNo = _doBillingTargetDebtSummaryList[0].BillingClientTelNo;
                    _ContactPersonName = _doBillingTargetDebtSummaryList[0].ContactPersonName;

                }
            }
            if (_doGetUnpaidDetailDebtSummaryByBillingCodeList != null) {
                if (_doGetUnpaidDetailDebtSummaryByBillingCodeList[0] != undefined) {
                    $("#txtSiteNameEN").val(_doGetUnpaidDetailDebtSummaryByBillingCodeList[0].SiteNameEN);
                    $("#txtSiteNameLC").val(_doGetUnpaidDetailDebtSummaryByBillingCodeList[0].SiteNameLC);

                    _BillingCode = _doGetUnpaidDetailDebtSummaryByBillingCodeList[0].BillingCode;
                }
            }
            if (_doGetUnpaidInvoiceDebtSummaryByBillingTargetList != null) {
                if (_doGetUnpaidInvoiceDebtSummaryByBillingTargetList[0] != undefined) {

                    //4.	Display invoice information
                    //$("#divInvoice").show();
                    $("#txtInvoiceNo").val(_doGetUnpaidInvoiceDebtSummaryByBillingTargetList[0].InvoiceNo);

                    $("#txtInvoiceInvoiceAmount").val(_doGetUnpaidInvoiceDebtSummaryByBillingTargetList[0].InvoiceAmount032String);
                    $("#txtInvoiceInvoiceAmount").NumericCurrency().val(ICS032_ViewBag.C_CURRENCY_LOCAL);

                    $("#txtInvoiceInvoiceAmountUsd").val(_doGetUnpaidInvoiceDebtSummaryByBillingTargetList[0].InvoiceAmount032UsdString);
                    $("#txtInvoiceInvoiceAmountUsd").NumericCurrency().val(ICS032_ViewBag.C_CURRENCY_US);

                    $("#txtInvoiceInvoiceIssueDate").val(ConvertDateToTextFormat(ConvertDateObject(_doGetUnpaidInvoiceDebtSummaryByBillingTargetList[0].IssueInvDate)));

                    $("#txtInvoiceUnpaidAmount").val(accounting.toFixed(_doGetUnpaidInvoiceDebtSummaryByBillingTargetList[0].UnpaidAmount, 2));
                    $("#txtInvoiceUnpaidAmount").NumericCurrency().val(ICS032_ViewBag.C_CURRENCY_LOCAL);
                    $("#txtInvoiceUnpaidAmount").setComma();

                    $("#txtInvoiceUnpaidAmountUsd").val(accounting.toFixed(_doGetUnpaidInvoiceDebtSummaryByBillingTargetList[0].UnpaidAmountUsd, 2));
                    $("#txtInvoiceUnpaidAmountUsd").NumericCurrency().val(ICS032_ViewBag.C_CURRENCY_US);
                    $("#txtInvoiceUnpaidAmountUsd").setComma();

                    $("#txtInvoiceNumberOfUnpaidDetail").val(_doGetUnpaidInvoiceDebtSummaryByBillingTargetList[0].NoOfBillingDetailString);

                    _InvoiceNo = _doGetUnpaidInvoiceDebtSummaryByBillingTargetList[0].InvoiceNo;
                    _InvoiceOCC = _doGetUnpaidInvoiceDebtSummaryByBillingTargetList[0].InvoiceOCC;
                }
            }
            // load in this screen section
            if (_doGetBillingCodeDebtSummaryList != null) {
                if (_doGetBillingCodeDebtSummaryList[0] != undefined) {

                    //$("#divBillingCode").show();
                    $("#txtBillingCode").val(_doGetBillingCodeDebtSummaryList[0].BillingCodeShort);

                    $("#txtBillingCodeUnpaidAmount").val(_doGetBillingCodeDebtSummaryList[0].UnpaidAmountString);
                    $("#txtBillingCodeUnpaidAmount").NumericCurrency().val(ICS032_ViewBag.C_CURRENCY_LOCAL);

                    $("#txtBillingCodeUnpaidAmountUsd").val(_doGetBillingCodeDebtSummaryList[0].UnpaidAmountUsdString);
                    $("#txtBillingCodeUnpaidAmountUsd").NumericCurrency().val(ICS032_ViewBag.C_CURRENCY_US);

                    $("#txtBillingCodeNumberOfUnpaidDetail").val(_doGetBillingCodeDebtSummaryList[0].NoOfBillingDetailString);

                }
            }

            var _strTemptxtaMemoHistory;
            if (_doGetDebtTracingMemoList != null) {
                if (_doGetDebtTracingMemoList[0] != undefined) {


                    $("#divMemoHistory").show();
                    _strTemptxtaMemoHistory = '';

 
                    for (var i = 0; i < _doGetDebtTracingMemoList.length; ++i) {
                        _strTemptxtaMemoHistory = _strTemptxtaMemoHistory +
                                                        _doGetDebtTracingMemoList[i].CreateDateString + ' ' + _doGetDebtTracingMemoList[i].DebtTracingLevelString + ' - ' +
                                                        ' ' + strRegistrant + ': ' + _doGetDebtTracingMemoList[i].RegistrantString + '\r\n' +
                                                        ' ' + strContent + ': ' + _doGetDebtTracingMemoList[i].Content + '\r\n' +
                                                        ' ' + strTracingResult + ': ' + _doGetDebtTracingMemoList[i].TracingResultString + '\r\n' +
                                                        ' ' + strExpectPaymentDate + ': ' + _doGetDebtTracingMemoList[i].ExpectPaymentDateString + '\r\n' +
                                                        ' ' + strLastContactDate + ': ' + _doGetDebtTracingMemoList[i].LastContactDateString + '\r\n' +
                                                        ' ' + strPaymentMethod + ': ' + (_doGetDebtTracingMemoList[i].PaymentMethodTypeString == null ? "" : _doGetDebtTracingMemoList[i].PaymentMethodTypeString) + '\r\n' +
                                                         '\r\n';
                    }

                    $("#txtaMemoHistory").val(_strTemptxtaMemoHistory);


                }
            }

        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });

}
// Form Mode Section

var conModeInit = 0;
var conModeView = 1;
var conModeEdit = 2;
var conModeConfirm = 9;

var conNo = 0;
var conYes = 1;

var bolViewMode = false;

var conModeRadio1rdo1Invoice = 1;
var conModeRadio1rdo1BillingTarget = 2;

var verModeRadio1 = 1;

function setFormMode(intMode) {
    if (intMode == conModeInit) {
        // ModeInit
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        approve_command.SetCommand(null);
        reject_command.SetCommand(null);
        return_command.SetCommand(null);
        close_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }
    else if (intMode == conModeView) {
        // ModeView = 1;
        register_command.SetCommand(btn_Register_click);
        reset_command.SetCommand(btn_Reset_click);
        approve_command.SetCommand(null);
        reject_command.SetCommand(null);
        return_command.SetCommand(null);
        close_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }
    else if (intMode == conModeEdit) {
        // ModeEdit = 2;
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        approve_command.SetCommand(null);
        reject_command.SetCommand(null);
        return_command.SetCommand(null);
        close_command.SetCommand(null);
        confirm_command.SetCommand(btn_Confirm_click);
        back_command.SetCommand(btn_Back_click);
    }
    else if (intMode == conModeConfirm) {
        // ModeConfirm = 9;
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        approve_command.SetCommand(null);
        reject_command.SetCommand(null);
        return_command.SetCommand(null);
        close_command.SetCommand(null);
        confirm_command.SetCommand(btn_Confirm_click);
        back_command.SetCommand(btn_Back_click);
    }
}

function btn_Search_click() {
    // check all input on Server
    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Income/ICS032_SearchData", obj, function (result, controls, isWarning) {
        if (result != undefined) {

            // goto Idel state
            // Wired design
            setVisableTable(conYes);
            setFormMode(conModeInit);
            bolViewMode = true;

            _doGetMoneyCollectionManagementInfoList = result.doGetMoneyCollectionManagementInfo;
            if (_doGetMoneyCollectionManagementInfoList != null) {
                for (var i = 0; i < _doGetMoneyCollectionManagementInfoList.length; ++i) {
                    Add_MoneyCollectionManagementInformationBlankLine(_doGetMoneyCollectionManagementInfoList[i]);
                }
            };

            $("#dtpExpectedCollectDateFrom").attr("disabled", true);
            $("#dtpExpectedCollectDateTo").attr("disabled", true);
            $("#chklCollectionArea").attr("disabled", true);

            $("#btnSearch").attr("disabled", true);
            $("#btnClear").attr("disabled", false);

            $("#btnDownloadFile").attr("disabled", false);

        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });
}

function btn_Clear_click() {

    /* --- Get Message --- */
    var obj = {
        module: "Common",
        code: "MSG0038"
    };
    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenOkCancelDialog(result.Code, result.Message,
        function () {

            $("#dtpExpectedCollectDateFrom").attr("disabled", false);
            $("#dtpExpectedCollectDateTo").attr("disabled", false);
            $("#chklCollectionArea").attr("disabled", false);

            $("#dtpExpectedCollectDateFrom").val("");
            $("#dtpExpectedCollectDateTo").val("");

            $("#chklCollectionArea").find("input:checkbox").each(function () {
                if ($(this).prop("checked") == true) {
                    $(this).attr("checked", false);
                }
            });

            setVisableTable(conNo);

            DeleteAllRow(grdMoneyCollectionManagementInformationGrid);
            _doGetMoneyCollectionManagementInfoList = null;

            $("#btnSearch").attr("disabled", false);
            $("#btnClear").attr("disabled", true);

            $("#btnDownloadFile").attr("disabled", false);


            $("#divInput").clearForm();

            CloseWarningDialog();
        },
        null);
    });


}

// Mode Event
function btn_Register_click() {


var strRegistrant = $("#hidmemoRegistrant").val();
var strContent = $("#hidmemoContent").val();
var strTracingResult = $("#hidmemoTracingResult").val();
var strExpectPaymentDate = $("#hidmemoExpectPaymentDate").val();
var strLastContactDate = $("#hidmemoLastContactDate").val();
var strPaymentMethod = $("#hidmemoPaymentMethod").val();
 
    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Income/ICS032_Register", obj, function (result, controls, isWarning) {
 
        if (result != undefined) {
            if (result == "1") {

//                $("#divInput").SetViewMode(true);
//                $("#divInput").ResetToNormalControl(false);
                //                setFormMode(conModeEdit);

                // save data
                ajax_method.CallScreenController("/Income/ICS032_Confirm", "",
                function (result, controls, isWarning) {
                    if (result != undefined) {
                        if (result == "1") {

                            // Success
                            var objMsg = {
                                module: "Income",
                                code: "MSG7008"
                            };


                            call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                                OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {

                                    // goto confirm state
//                                    setVisableTable(conNo);
//                                    setFormMode(conModeView);
//                                    bolViewMode = false;
//                                    $("#divInput").SetViewMode(false);
//                                    $("#divInput").ResetToNormalControl(true);

                                    //                                    ClearScreenToInitStage();
                                    var obj = null;
                                    ajax_method.CallScreenController("/Income/ICS032_SearchMEMO", obj, function (result, controls, isWarning) {
                                        if (result != undefined) {

                                            // goto Idel state
                                            // Wired design

                                            _doGetDebtTracingMemoList = result.doGetDebtTracingMemoList;


                                            var _strTemptxtaMemoHistory;
                                            if (_doGetDebtTracingMemoList != null) {
                                                if (_doGetDebtTracingMemoList[0] != undefined) {

                                                    setVisableTable(conNo);
                                                    setFormMode(conModeView);
                                                    bolViewMode = false;
                                                    $("#divInput").SetViewMode(false);
                                                    $("#divInput").ResetToNormalControl(true);

                                                    ClearScreenToInitStage();

                                                    $("#divMemoHistory").show();
                                                    _strTemptxtaMemoHistory = '';
 
                                                    for (var i = 0; i < _doGetDebtTracingMemoList.length; ++i) {
                                                        _strTemptxtaMemoHistory = _strTemptxtaMemoHistory +
                                                        _doGetDebtTracingMemoList[i].CreateDateString + ' ' + _doGetDebtTracingMemoList[i].DebtTracingLevelString + ' - ' +
                                                        ' ' + strRegistrant + ': ' + _doGetDebtTracingMemoList[i].RegistrantString + '\r\n' +
                                                        ' ' + strContent +': ' + _doGetDebtTracingMemoList[i].Content + '\r\n' +
                                                        ' ' + strTracingResult +': ' + _doGetDebtTracingMemoList[i].TracingResultString + '\r\n' +
                                                        ' ' + strExpectPaymentDate +': ' + _doGetDebtTracingMemoList[i].ExpectPaymentDateString + '\r\n' +
                                                        ' ' + strLastContactDate +': ' + _doGetDebtTracingMemoList[i].LastContactDateString + '\r\n' +
                                                        ' ' + strPaymentMethod + ': ' + (_doGetDebtTracingMemoList[i].PaymentMethodTypeString == null ? "" : _doGetDebtTracingMemoList[i].PaymentMethodTypeString) + '\r\n' +
                                                         '\r\n';
                                                    }

                                                    $("#txtaMemoHistory").val(_strTemptxtaMemoHistory);
                                                }
                                            }

                                        }
                                        else if (controls != undefined) {
                                            VaridateCtrl(controls, controls);
                                        }
                                    });


                                });
                            });

                        }
                    }
                    else {
                        VaridateCtrl(controls, controls);
                    }
                });
 
            }
            else {
                VaridateCtrl(controls, controls);
            }
        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }

    });
 
}

// create all send to server data for check mendatory and save (in case all input data is ok)
function GetUserAdjustData() {
 

    var header = {

        rdoProcessType : verModeRadio1,

        cboTracingResault : $('#cboTracingResault').val() ,
        dtpLastContractDate : $('#dtpLastContractDate').val() ,
        dtpExpectedPaymentdate : $('#dtpExpectedPaymentdate').val() ,
        cboPaymentMethods : $('#cboPaymentMethods').val() ,
        txtaMemo : $('#txtaMemo').val()

    };

    var returnObj = {
        Header: header
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

            //setVisableTable(conNo);
            setFormMode(conModeView);
            bolViewMode = true;

            // set view mode only this DIV
            $("#divInput").SetViewMode(false);
            $("#divInput").ResetToNormalControl(true);

            ClearScreenToInitStage();
//        },
//        null);
//    });
}

function btn_Confirm_click() {
    // save data
    ajax_method.CallScreenController("/Income/ICS032_Confirm", "",
        function (result, controls, isWarning) {
            if (result != undefined) {
                if (result == "1") {
                    // goto confirm state
                    setVisableTable(conNo);
                    setFormMode(conModeView);
                    bolViewMode = false;
                    $("#divInput").SetViewMode(false);
                    $("#divInput").ResetToNormalControl(true);
                    
                    ClearScreenToInitStage();
                }
            }
            else {
                VaridateCtrl(controls, controls);
            }
        });
}
function btn_Back_click() {
    setFormMode(conModeView);
    bolViewMode = false;
    $("#divInput").SetViewMode(false);

    $("#divInput").ResetToNormalControl(true);
}

// Clear Screen
function ClearScreenToInitStage() {
    $("#rdo1Invoice").attr("checked", true);
    //$("#rdo1BillingTarget").attr("checked", true);
    verModeRadio1 = conModeRadio1rdo1Invoice;

    $("#cboTracingResault").val("");
    $("#dtpLastContractDate").val("");
    $("#dtpExpectedPaymentdate").val("");
    $("#cboPaymentMethods").val("");
    $("#txtaMemo").val("");
}
// Enable Obj On Screen

// Visable Obj On Screen
function setVisableTable(intMode) {

//    if (intMode == conYes) {
//        $("#divResaultList").show();
//    }
//    else if (intMode == conNo) {
//        $("#divResaultList").hide();
//    }
//    else {
//        $("#divResaultList").hide();
//    };

}
// Radio Select

function rdo1Invoice_Select() {
    verModeRadio1 = conModeRadio1rdo1Invoice;
}

function rdo1BillingTarget_Select() {
    verModeRadio1 = conModeRadio1rdo1BillingTarget;
}

function number_format(number, decimals, dec_point, thousands_sep) {

    number = (number + '').replace(/[^0-9+\-Ee.]/g, '');
    var n = !isFinite(+number) ? 0 : +number,
        prec = !isFinite(+decimals) ? 0 : Math.abs(decimals), sep = (typeof thousands_sep === 'undefined') ? ',' : thousands_sep,
        dec = (typeof dec_point === 'undefined') ? '.' : dec_point,
        s = '',
        toFixedFix = function (n, prec) {
            var k = Math.pow(10, prec); return '' + Math.round(n * k) / k;
        };
    // Fix for IE parseFloat(0.55).toFixed(0) = 0;
    s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.');
    if (s[0].length > 3) {
        s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
    }
    if ((s[1] || '').length < prec) {
        s[1] = s[1] || '';
        s[1] += new Array(prec - s[1].length + 1).join('0');
    }
    return s.join(dec);
}
