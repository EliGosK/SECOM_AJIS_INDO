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
/// <reference path="../Base/ComboBox.js" />
/// <reference path="../Common/Dialog.js" />


var serachGrid;
var matchingGrid;

var isCall084by080;
var currentPageMode;
var isReloadics081 = true;

$(document).ready(function () {
    //Call Only one
    InitialPage();
    InitialGrid();
    InitialBindingEvent();

    var currentMode = $("#ScreenMode").val();
    if (currentMode == ICS080_Constant.ScreenMode.View) {
        //Initial only view mode
        InitialViewPaymentMatchingGrid();
    }
    SetScreenPageMode(ICS080_Constant.ScreenPageMode.ICS080);
}); 

//UI Common
function InitialPage() {
    //Main
    InitialDateFromToControl("#PaymentDateFrom", "#PaymentDateTo");
    $("#MatchableBalanceFrom").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#MatchableBalanceTo").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtChequeReturnDate").InitialDate();
}

//Confirm with sa, no need related field
//function PaymentTypeChanged() {
//    var paymentype = $("#PaymentType").val();
//    var obj = { paymentType: paymentype };

//    ajax_method.CallScreenController("/Income/ICS080_GetSECOMAccount", obj, function (data) {
//        regenerate_combo("#SECOMAccountID", data);
//    });
//}


//Grid
function InitialGrid() {
    if ($.find("#SearchGrid").length > 0) {
        serachGrid = $("#SearchGrid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Income/ICS080_InitialSearchGrid");
        BindingGridEvent();
    }
}

function InitialViewPaymentMatchingGrid() {
    if ($.find("#ics080viewPaymentMatchingGrid").length > 0) {
        matchingGrid = $("#ics080viewPaymentMatchingGrid").InitialGrid(0, false, "/Income/ICS080_InitialMatchGrid");
        BindingMatchingGridEvent();
    }
}


//Binding Event
function InitialBindingEvent() {
    $("#btnSearch").click(SerachPaymentData);
    $("#btnClear").click(ClearForm);
    $("#btnEncash").click(Encash);
    $("#btnBack").click(ics080BackPageMode);
    $("#rdoReturnCheque, #rdoEncash").click(function () {
        RefreshEncashEnabling();
    });
}


function SerachPaymentData() {
    $("#btnSearch").attr("disabled", true);
    $("#btnClear").attr("disabled", true);

    $("#SearchGrid").hide();
    $("#ICS080_ViewPaymentMatching").hide();



    var screenMode = $("#ScreenMode").val();
    var objCriteria = CreateObjectData($("#formSearch").serialize());
    objCriteria.MyPayment = $("#MyPayment").is(":checked");
    var obj = {
        ScreenMode: screenMode,
        PaymentSearchCriteria: objCriteria
    };

    $("#SearchGrid").LoadDataToGrid(serachGrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Income/ICS080_SearchPaymentData", obj, "doPayment", false,
                    function () { // post-load
                        $("#btnSearch").removeAttr("disabled");
                        $("#btnClear").removeAttr("disabled");

                        DecorateGrid();
                        document.getElementById('SearchGrid').scrollIntoView();
                    },
                    function (result, controls, isWarning) {
                        if (isWarning == undefined) {
                            $("#SearchGrid").show();
                        }
                    });
}

function ClearForm() {
    $("#divSearchCondition").clearForm();
    ClearDateFromToControl("#PaymentDateFrom", "#PaymentDateTo");
    CloseWarningDialog();
    DeleteAllRow(serachGrid);

    var currentMode = $("#ScreenMode").val();
    if (currentMode == ICS080_Constant.ScreenMode.Match) {
        //Set selected status for match mode
        $("#Status").val(ICS080_Constant.C_PAYMENT_STATUS_SERACH_HAVE_UNMATCHED_PAYMENT).attr('selected', true);
    }

    $("#SearchGrid").hide();
    $("#ICS080_ViewPaymentMatching").hide();
}


function BindingGridEvent() {
    var currentMode = $("#ScreenMode").val();

    SpecialGridControl(serachGrid, ["Select"]);
    BindOnLoadedEvent(serachGrid, function () {
        var colInx = serachGrid.getColIndexById('Select');
        var colMatchBalance = serachGrid.getColIndexById('MatchableBalanceVal');

        for (var i = 0; i < serachGrid.getRowsNum(); i++) {
            var rowId = serachGrid.getRowId(i);

            // binding grid button event 
            if (currentMode == ICS080_Constant.ScreenMode.Match) {
                var matchBalance = serachGrid.cells2(i, colMatchBalance).getValue();
                if (matchBalance != "" && matchBalance != 0) {
                    //Show only matchbalance > 0
                    GenerateSelectButton(serachGrid, "btnSelect", rowId, "Select", true);
                    BindGridButtonClickEvent("btnSelect", rowId, SelectPayment);
                }
            }
            else if (currentMode == ICS080_Constant.ScreenMode.View) {
                GenerateSelectButton(serachGrid, "btnSelect", rowId, "Select", true);
                BindGridButtonClickEvent("btnSelect", rowId, ViewPaymentMatching);
            }
            else if (currentMode == ICS080_Constant.ScreenMode.Delete) {
                GenerateRemoveButton(serachGrid, "btnDelete", rowId, "Select", true);
                BindGridButtonClickEvent("btnDelete", rowId, DeletePayment);
            }
        }

        serachGrid.setSizes();
    });
}

function BindingMatchingGridEvent() {
    BindOnLoadedEvent(matchingGrid, function () {
        var colInx = matchingGrid.getColIndexById('InvoiceNo');
        var codlInvoiceOCC = matchingGrid.getColIndexById('InvoiceOCC');
        var colBillingTargetCode = matchingGrid.getColIndexById('BillingTargetCode');

        for (var i = 0; i < matchingGrid.getRowsNum(); i++) {
            //call CMS450 Dialog
            var invoiceNo = matchingGrid.cells2(i, colInx).getValue();

            if (invoiceNo != undefined && invoiceNo != "-") {
                var tagAcms450 = "<a href='#'>" + invoiceNo + "<input type='hidden' name='callScreenID' value='CMS450'/><input type='hidden' name='InvoiceNo' value='" + invoiceNo + "'/></a>";
                matchingGrid.cells2(i, colInx).setValue(tagAcms450);
            }

        }

        //Binding Event for TAG A
        $("#ics080viewPaymentMatchingGrid a").each(function () {
            $(this).click(function () {
                var callScreenID = $(this).children("input:hidden[name=callScreenID]").val();

                if (callScreenID == "CMS450") {
                    var cms450Object = {
                        InvoiceNo: $(this).children("input:hidden[name=InvoiceNo]").val()
                    };
                    //$("#ics080dlgBox").OpenCMS450Dialog("CMS450");        //Confirm PG (Nueang) Not use.
                    ajax_method.CallScreenControllerWithAuthority("/Common/CMS450", cms450Object, true);
                }
                return false;
            });
        });
        matchingGrid.setSizes();
    });
}

function DecorateGrid() {
    if (serachGrid.getRowsNum() != 0) {
        var colDeleteFlag = serachGrid.getColIndexById('DeleteFlag');

        for (var i = 0; i < serachGrid.getRowsNum(); i++) {
            var row_id = serachGrid.getRowId(i);
            if (serachGrid.cells(row_id, colDeleteFlag).getValue() == "1") {
                serachGrid.setRowColor(row_id, "#efefef");  //deleted payment - background color: gray
            } 
            else {
                serachGrid.setRowColor(row_id, "");
            }
        }
    }
}

function DecoratePaymentMatchingGrid() {
    if (matchingGrid.getRowsNum() != 0) {
        var colDecorateFlag = matchingGrid.getColIndexById('GirdBusinessDecorateFlag');

        for (var i = 0; i < matchingGrid.getRowsNum(); i++) {
            var row_id = matchingGrid.getRowId(i);
            if (matchingGrid.cells(row_id, colDecorateFlag).getValue() == "red") {
                matchingGrid.setRowTextStyle(row_id, "color: red;");
            } 
            else {
                matchingGrid.setRowTextStyle(row_id, "");
            }
        }
    }
}


function SelectPayment(rowId) {
    var rownum = serachGrid.getRowIndex(rowId);
    serachGrid.selectRow(rownum);
    var tranNo = serachGrid.cells2(rownum, serachGrid.getColIndexById('PaymentTransNo')).getValue();
    var invoiceNo = serachGrid.cells2(rownum, serachGrid.getColIndexById('RefInvoiceNo')).getValue();


    var obj = { paymentTranNo: tranNo };
    ajax_method.CallScreenController("/Income/ICS080_MatchPaymentNextStep", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            if (result == "ICS081") {
                var obj081 = { paymentTranNo: tranNo };

                //Check permission at this point to support multiload (page), to prevent blank screen.
                ajax_method.CallScreenController("/Income/ICS081_Authority", obj081, function (result, controls, isWarning) {
                    if (result != undefined) {
                        ajax_method.CallScreenController("/Income/ICS081_SetPaymentTransNo", obj081, function (result, controls, isWarning) {
                            isReloadics081 = true;
                            isCall084by080 = "false";
                            $("#divICS080").hide();
                            SetScreenPageMode(ICS080_Constant.ScreenPageMode.ICS081);
                        });
                    }
                });
            }
            else if (result == "ICS084") {
                var obj084 = {
                    ScreenCaller: "ICS080",
                    PaymentTransNo: tranNo,
                    InvoiceNo: invoiceNo

                };

                //Check permission at this point to support multiload (page), to prevent blank screen.
                ajax_method.CallScreenController("/Income/ICS084_Authority", obj084, function (result, controls, isWarning) {
                    if (result != undefined) {
                        ajax_method.CallScreenController("/Income/ICS084_SetScreenData", obj084, function (result, controls, isWarning) {
                            isCall084by080 = "true";
                            $("#divICS080").hide();
                            SetScreenPageMode(ICS080_Constant.ScreenPageMode.ICS084);
                        });
                    }
                });
            }
        }
        if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });
}

function ViewPaymentMatching(rowId) {
    var rownum = serachGrid.getRowIndex(rowId);
    serachGrid.selectRow(rownum);
    var tranNo = serachGrid.cells2(rownum, serachGrid.getColIndexById('PaymentTransNo')).getValue();
    ViewPaymentMatchingByTransNo(tranNo);
}

function ViewPaymentMatchingByTransNo(tranNo) {
    var obj = { paymentTranNo: tranNo };
    ajax_method.CallScreenController("/Income/ICS080_DisplayPaymentMatchingResult", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            DisplayPaymentMatching(result);
        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });
}

function DisplayPaymentMatching(paymentMatching) {
    $("#ics080viewPaymentTransNo").val(paymentMatching.doPayment.PaymentTransNo);
    $("#ics080viewPaymentType").val(paymentMatching.doPayment.PaymentTypeDisplay);
    $("#ics080viewPaymentDate").val(paymentMatching.doPayment.PaymentDateDisplay);
    $("#ics080viewPromissoryNoteNo").val(paymentMatching.doPayment.DocNo);
    $("#ics080viewPromissoryNoteDate").val(paymentMatching.doPayment.DocDateDisplay);
    $("#ics080viewSECOMBankBranch").val(paymentMatching.doPayment.SECOMBankFullName);
    $("#ics080viewSendingBankBranch").val(paymentMatching.doPayment.SendingBankFullName);
    $("#ics080viewMemo").val(paymentMatching.doPayment.Memo);
    $("#ics080viewPayerName").val(paymentMatching.doPayment.Payer);
    $("#ics080viewAccountNo").val(paymentMatching.doPayment.PayerBankAccNoDisplay);
    $("#ics080viewTelephoneNo").val(paymentMatching.doPayment.TelNo);
    var paymentAmountVal = (new Number(paymentMatching.doPayment.PaymentAmountVal)).numberFormat("#,##0.00");
    $("#ics080viewFirstPaymentAmount").val(paymentAmountVal);
    $("#ics080viewFirstPaymentAmount").NumericCurrency().val(paymentMatching.doPayment.PaymentAmountCurrencyType);
    var matchAmountVal = (new Number(paymentMatching.doPayment.MatchableBalanceVal)).numberFormat("#,##0.00");
    $("#ics080viewMatchableBalance").val(matchAmountVal);
    $("#ics080viewMatchableBalance").NumericCurrency().val(paymentMatching.doPayment.MatchableBalanceCurrencyType);
    
    $("#txtEncashStatus").val(paymentMatching.doPayment.ChequeStatus);
    $("#rdoReturnCheque").prop("checked", (paymentMatching.doPayment.EncashedFlag == 2));
    $("#rdoEncash").prop("checked", (paymentMatching.doPayment.EncashedFlag == 1));

    $("#txtChequeReturnDate").SetDate(ConvertDateObject(paymentMatching.doPayment.ChequeReturnDate));
    $("#cboChequeReturnReason").val(paymentMatching.doPayment.ChequeReturnReason);
    $("#txtChequeReturnRemark").val(paymentMatching.doPayment.ChequeReturnRemark);
    $("#txtChequeEncashRemark").val(paymentMatching.doPayment.ChequeEncashRemark);
    
    if (paymentMatching.IsEncashed == true) {
        //Already encashed
        $("#divEncashInfo").show();
        SetEnabledEncashedInfo(false);
    }
    else if (paymentMatching.IsEncashable == true) {
        $("#divEncashInfo").show();
        if (paymentMatching.IsPermissionEncash == true) {
            //Be able to click encash button
            SetEnabledEncashedInfo(true);
            RefreshEncashEnabling();
        }
        else {
            //No premission
            SetEnabledEncashedInfo(false);
        }
    }
    else {
        //Unable to click
        $("#divEncashInfo").hide();
    }
    
    var obj = { paymentTranNo: paymentMatching.doPayment.PaymentTransNo };
    $("#ics080viewPaymentMatchingGrid").LoadDataToGrid(matchingGrid, 0, false, "/Income/ICS080_SearchPaymentMatching", obj, "doPaymentMatchingResultDetail", false,
                    function () { // post-load
                        DecoratePaymentMatchingGrid();

                        //Display, Hide
                        $("#ICS080_ViewPaymentMatching").show();
                        document.getElementById('ICS080_ViewPaymentMatching').scrollIntoView(); 
                    },
                    function (result, controls, isWarning) {
                        if (isWarning == undefined) {
                        }
                    });
}

function Encash() {
    var param = { "module": "Common", "code": "MSG0028", "param": $("#btnEncash").text() };
    call_ajax_method("/Shared/GetMessage", param, function (data) {
        OpenYesNoMessageDialog(data.Code, data.Message, function () {
            SetEnabledEncashedInfo(false);
            var obj = {
                PaymentTransNo: $("#ics080viewPaymentTransNo").val(),
                EncashedFlag: (
                    $("#rdoReturnCheque").prop("checked") ? 2 :
                    $("#rdoEncash").prop("checked") ? 1 : 0
                ),
                ChequeReturnDate: $("#txtChequeReturnDate").val(),
                ChequeReturnReason: $("#cboChequeReturnReason").val(),
                ChequeReturnRemark: $("#txtChequeReturnRemark").val(),
                ChequeEncashRemark: $("#txtChequeEncashRemark").val()
            };
            ajax_method.CallScreenController("/Income/ICS080_Encash", obj, function (result, controls, isWarning) {
                if (controls) {
                    VaridateCtrl(controls, controls);
                }
                if (result == "1") {
                    // Success
                    var objMsg = { module: "Income", code: "MSG7008" };
                    call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                        OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {
                            ViewPaymentMatchingByTransNo($("#ics080viewPaymentTransNo").val());
                        });
                    });
                }
                else {
                    SetEnabledEncashedInfo(true);
                    RefreshEncashEnabling();
                }
            });
        }, null);
    });
    return false;
}


function DeletePayment(rowId) {
    var param = { "module": "Common", "code": "MSG0142" };
    call_ajax_method("/Shared/GetMessage", param, function (data) {
        OpenYesNoMessageDialog(data.Code, data.Message, function () {
            var rownum = serachGrid.getRowIndex(rowId);
            serachGrid.selectRow(rownum);
            var tranNo = serachGrid.cells2(rownum, serachGrid.getColIndexById('PaymentTransNo')).getValue();

            var obj = { paymentTranNo: tranNo };
            ajax_method.CallScreenController("/Income/ICS080_DeletePayment", obj, function (result, controls, isWarning) {
                if (result == "1") {
                    // Success
                    var objMsg = { "module": "Income", "code": "MSG7008"};
                    call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                        OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {
                            DeleteRow(serachGrid, rowId);
                        });
                    });
                }
                else if (controls != undefined) {
                    VaridateCtrl(controls, controls);
                }
            });
        }, null);
    });
}


function ics080RefreshMatchableBalance() {
    var rownum = serachGrid.getRowIndex(serachGrid.getSelectedId());
    var tranNo = serachGrid.cells2(rownum, serachGrid.getColIndexById('PaymentTransNo')).getValue();    
    var obj = { paymentTranNo: tranNo };

    ajax_method.CallScreenController("/Income/ICS080_GetMatchableBalance", obj, function (result, controls, isWarning) {
        if (result != undefined) {

            if (result == 0) {
                serachGrid.cells2(rownum, serachGrid.getColIndexById('MatchableBalanceVal')).setValue("0.00");

                //Hide select button for match mode
                var currentMode = $("#ScreenMode").val();
                if (currentMode == ICS080_Constant.ScreenMode.Match) {
                    serachGrid.cells2(rownum, serachGrid.getColIndexById('Select')).setValue("");
                }
            } 
            else {
                serachGrid.cells2(rownum, serachGrid.getColIndexById('MatchableBalanceVal')).setValue(result);
            }
        }
    });
}


function SetScreenPageMode(mode) {
    currentPageMode = mode;
    ics080HideBackButton();

    if (mode == ICS080_Constant.ScreenPageMode.ICS080) {
        $(".screen-name")[0].innerHTML = ICS080_Constant.ScreenPageName.ICS080;
        $(".screen-code")[0].innerHTML = ICS080_Constant.ScreenPageMode.ICS080;
        $("#divICS080").show();
        $("#ICS080_ViewPaymentMatching").hide();
        $("#divICS081").hide();
        $("#divICS084").hide();
        ics080HideBackButton();
    }
    else if (mode == ICS080_Constant.ScreenPageMode.ICS081) {
        $(".screen-name")[0].innerHTML = ICS080_Constant.ScreenPageName.ICS081;
        $(".screen-code")[0].innerHTML = ICS080_Constant.ScreenPageMode.ICS081;
        $("#divICS080").hide();
        $("#ICS080_ViewPaymentMatching").hide();

        if (isReloadics081 == true) {
            var resetscreen = ["ICS081"];
            ResetAllScreen(resetscreen);
            var showscreen = ["ICS081"];
            CallMultiLoadScreen("Income", [showscreen], null);
        }
        else {
            $("#divICS081").show();
            ics080ShowBackButton();
        }
        $("#divICS084").hide();
    }
    else if (mode == ICS080_Constant.ScreenPageMode.ICS084) {
        $(".screen-name")[0].innerHTML = ICS080_Constant.ScreenPageName.ICS084;
        $(".screen-code")[0].innerHTML = ICS080_Constant.ScreenPageMode.ICS084;
        $("#divICS080").hide();
        $("#ICS080_ViewPaymentMatching").hide();
        $("#divICS081").hide();
        //$("#divICS084").show();
        var resetscreen = ["ICS084"];
        ResetAllScreen(resetscreen);
        var showscreen = ["ICS084"];
        CallMultiLoadScreen("Income", [showscreen], null);
        //ics080ShowBackButton();
    }
}

function ics080ShowBackButton() {
    $("#divBack").show();
}
function ics080HideBackButton() {
    $("#divBack").hide();
}

function ics080BackPageMode() {
    register_command.SetCommand(null);
    reset_command.SetCommand(null);
    confirm_command.SetCommand(null);
    back_command.SetCommand(null);

    if (currentPageMode == ICS080_Constant.ScreenPageMode.ICS080) {
        //Show search result, Hide View matching detail
        $("#divICS080").show();
        $("#ICS080_ViewPaymentMatching").hide();
    }
    else if (currentPageMode == ICS080_Constant.ScreenPageMode.ICS081) {
        SetScreenPageMode(ICS080_Constant.ScreenPageMode.ICS080);
    }
    else if (currentPageMode == ICS080_Constant.ScreenPageMode.ICS084) {
        if (isCall084by080 == "true") {
            SetScreenPageMode(ICS080_Constant.ScreenPageMode.ICS080);
        }
        else {
            isReloadics081 = false;
            SetScreenPageMode(ICS080_Constant.ScreenPageMode.ICS081);
        }
    }
}

function SetEnabledEncashedInfo(enabled) {
    $("#divEncashInfo").SetDisabled(!enabled);
    $("#txtChequeReturnDate").EnableDatePicker(enabled);
}

function RefreshEncashEnabling() {
    var selectedReturn = $("#rdoReturnCheque").prop("checked");
    var selectedEncash = $("#rdoEncash").prop("checked");

    $("#txtEncashStatus").SetDisabled(true);
    $("#txtChequeReturnDate").EnableDatePicker(selectedReturn);
    $("#cboChequeReturnReason").SetDisabled(!selectedReturn);
    $("#txtChequeReturnRemark").SetDisabled(!selectedReturn);
    $("#txtChequeEncashRemark").SetDisabled(!selectedEncash);
}
