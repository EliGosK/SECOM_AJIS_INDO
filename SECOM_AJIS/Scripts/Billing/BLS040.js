

//  jQuery
/// <reference path="../../Scripts/jquery-1.5.1-vsdoc.js"/>

// dhtmlx grid
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgrid.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_data.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgridcell.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js"/>
/// <reference path = "../../Scripts/Base/Master.js" />
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Common/Dialog.js"/>
/// <reference path="../../Scripts/Billing/Dialog.js"/>
/// <reference path="../../Scripts/json2.js" />
/// <reference path = "../../Scripts/Base/control_events.js" />

/// <reference path = "../../Scripts/Base/GridControl.js" />

var pageRow = 0;
var BLS040_GridBillingType;

var objDebtTracingOffice;
var IsQCodeContract = false;


var C_MSG6016 = "";
var C_MSG6017 = "";
var C_MSG6022 = "";

// Main
$(document).ready(function () {

    GetValidateMessage();


    $("#btnSearchBillingTarget").click(function () {
        $("#dlgBLS040").OpenCMS470Dialog("BLS040");
    });

    $("#ChangeDate").InitialDate();
    $("#AdjustEndDate").InitialDate();
    $("#BillingStartDate0").InitialDate();
    $("#BillingStartDate1").InitialDate();
    $("#BillingStartDate2").InitialDate();
    $("#BillingStartDate3").InitialDate();
    $("#BillingStartDate4").InitialDate();
    $("#BillingStartDate5").InitialDate();

    initialGridOnload();

    //Add by Jutarat A. on 25042013
    $("#MonthlyBillingAmount0").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#MonthlyBillingAmount1").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#MonthlyBillingAmount2").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#MonthlyBillingAmount3").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#MonthlyBillingAmount4").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#MonthlyBillingAmount5").BindNumericBox(12, 2, 0, 999999999999.99);
    //End Add


    $("#BillingCycle").BindNumericBox(10, 0, 0, 9999999999, 0);
    $("#CreditTerm").BindNumericBox(10, 0, 0, 9999999999, 0);

    $("#btnRetrieve").click(retrieve_billing_click);
    $("#btnAutoTransferInfo").click(function () {
        if (BLS040_ViewBag.HasRegisterAutoTransferPermission == "1") {
            $("#dlgBLS031").OpenBLS031Dialog("BLS031");
        }
    });

    $("#btnCreditCardInfo").click(function () {
        if (BLS040_ViewBag.HasRegisterCreditCardPermission == "1") {
            $("#dlgBLS032").OpenBLS032Dialog("BLS032");
        }
    });


    $("#PaymentMethod").change(change_payment_method);
    $("#btnAddUpdateBillingType").click(btnAddUpdateBillingType_click);
    $("#btnClearBillingTarget").click(btnClearBillingTarget_click);
    $("#btnRetrieveBillingTarget").click(btnRetrieveBillingTarget_click);
    $("#CarefulFlag").click(checkCarefulFlag);
    $("#btnCancelBillingType").click(btnCancelBillingType_click);

    // Additional validate
    $("#BillingCycle").change(ValidateBillingCycle);
    $("#CreditTerm").change(ValidateCreditTerm);
    $("#StopBillingFlag").change(ValidateStopBillingFlag);

    //SpecialGridControl(BLS040_GridBillingType, ["Edit", "Remove"]); //Comment by Jutarat A. on 20022013 (Move to initialGridOnload())


    enabledGridBillingType();

    // Narupon
    if (BLS040_ViewBag != undefined && BLS040_ViewBag.ContractProjectCode != "" && BLS040_ViewBag.BillingOCC != "") {

        $("#ContractCodeProjectCode").val(BLS040_ViewBag.ContractProjectCode);
        $("#BillingOCC").val(BLS040_ViewBag.BillingOCC);

        retrieve_billing_click();

    }
    else {
        setInitialState();
    }


});

function GetValidateMessage() {
    var obj = {
        module: "Billing",
        code: "MSG6016"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        C_MSG6016 = result.Message;
    });

    var obj = {
        module: "Billing",
        code: "MSG6017"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        C_MSG6017 = result.Message;
    });

    var obj = {
        module: "Billing",
        code: "MSG6022"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        C_MSG6022 = result.Message;
    });


}

// === Additonal validate ==== 

function ValidateBillingCycle() {

    if ($("#BillingCycle").NumericValue() > 60) {

        var obj = {
            module: "Billing",
            code: "MSG6018"
        };

        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
            OpenYesNoMessageDialog(result.Code, result.Message,
            function () {
                $("#CreditTerm").focus();
                $("#CreditTerm").select();
            }
            , function () {
                $("#BillingCycle").val("60");
                $("#BillingCycle").focus();
                $("#BillingCycle").select();

            });

        });
    }
}


function ValidateCreditTerm() {


    if ($("#CreditTerm").NumericValue() > 12) {
        var obj = {
            module: "Billing",
            code: "MSG6019"
        };

        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
            OpenYesNoMessageDialog(result.Code, result.Message,
            function () {
                $("#CalDailyFeeStatus").focus();

            }
            , function () {
                $("#CreditTerm").val("12");
                $("#CreditTerm").focus();
                $("#CreditTerm").select();

            });

        });

    }
}

function ValidateStopBillingFlag() {

    var oldBillingFlag = $("#OldBillingFlag").val().toLowerCase() == "true" ? true : false;


    if (oldBillingFlag == false && $("#StopBillingFlag").val() == "1") {
        var obj = {
            module: "Billing",
            code: "MSG6020"
        };

        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
            OpenYesNoMessageDialog(result.Code, result.Message, function () { }, function () {
                $("#StopBillingFlag").val("0");
                $("#StopBillingFlag").focus();
                $("#StopBillingFlag").select();

            });


        });
    }
    else if (oldBillingFlag == true && $("#StopBillingFlag").val() == "0") {

        var obj = {
            module: "Billing",
            code: "MSG6021"
        };

        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
            OpenYesNoMessageDialog(result.Code, result.Message, function () { }, function () {
                $("#StopBillingFlag").val("1");
                $("#StopBillingFlag").focus();
                $("#StopBillingFlag").select();
            });


        });
    }
}

function ValidatePaymentMothod_autoTransfer() {
    if ($("#OldPaymentMethod").val() == C_PAYMENT_METHOD_AUTO_TRANSFER && $("#OldPaymentMethod").val() != $("#PaymentMethod").val()) {
        return false;
    }

    return true;
}

function ValidatePaymentMothod_creditCard() {
    if ($("#OldPaymentMethod").val() == C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER && $("#OldPaymentMethod").val() != $("#PaymentMethod").val()) {
        return false;
    }

    return true;
}

function ValidateBillingStartDate() {
    if ($("#OldBillingStartDate0").val() != $("#BillingStartDate0").val()
        || $("#OldBillingStartDate1").val() != $("#BillingStartDate1").val()
        || $("#OldBillingStartDate2").val() != $("#BillingStartDate2").val()
        || $("#OldBillingStartDate3").val() != $("#BillingStartDate3").val()
        || $("#OldBillingStartDate4").val() != $("#BillingStartDate4").val()
        || $("#OldBillingStartDate5").val() != $("#BillingStartDate5").val()
    ) {
        return false
    }

    return true;
}

function change_payment_method() {
    var paymentMethod = $("#PaymentMethod").val();
    if (paymentMethod == C_PAYMENT_METHOD_AUTO_TRANSFER) {
        $("#divAutoTransferInformation").show();
        $("#divCreditCardInformation").hide();
    }
    else if (paymentMethod == C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER) {
        $("#divAutoTransferInformation").hide();
        $("#divCreditCardInformation").show();
    }
    else {
        $("#divAutoTransferInformation").hide();
        $("#divCreditCardInformation").hide();
    }
}



function BLS031Object() {

    var obj = {

        ContractCode: $("#ContractCodeProjectCode").val()
                , BillingOCC: $("#BillingOCC").val()
                , BillingClientCode: $("#BillingClientCode").val() + "-" + $("#BillingTargetNo").val()
                , BillingClientNameEN: $("#FullNameEN").val()
                , BillingClientNameLC: $("#FullNameLC").val()
                , BankCode: $("#BankCode").val()
                , BankName: $("#BankName").val()
                , BankBranchCode: $("#BankBranchCode").val()
                , BankBranchName: $("#BankBranchName").val()
                , AccountNo: $("#AccountNo").val()
                , AccountName: $("#AccountName").val()
                , AccountType: $("#AccountType").val()
                , AutoTransferDate: $("#AutoTransferDate").val()
                , LastestResult: $("#LastestResult").val()

    };

    return { "doAutoTransfer": obj };
}

function BLS032Object() {

    var strMonth = $("#ExpMonth").val();
    if ($("#ExpMonth").val() < 10) {
        strMonth = strMonth.substring(1);
    }

    var obj = {

        ContractCode: $("#ContractCodeProjectCode").val()
            , BillingOCC: $("#BillingOCC").val()
            , BillingClientCode: $("#BillingClientCode").val() + "-" + $("#BillingTargetNo").val()
            , BillingClientNameEN: $("#FullNameEN").val()
            , BillingClientNameLC: $("#FullNameLC").val()
            , CreditCardCompanyCode: $("#CreditCardCompanyCode").val()
            , CreditCardCompanyName: $("#CreditCardCompanyName").val()
            , CreditCardType: $("#CreditCardType").val()
            , CreditCardTypeName: $("#CreditCardTypeName").val()
            , CreditCardNo: $("#CreditCardNo").val()
            , ExpMonth: strMonth
            , ExpYear: $("#ExpYear").val()
            , CardName: $("#CardName").val()

    };


    return { "doCredit": obj };
}

function BLS031Response(result) {

    $("#dlgBLS031").CloseDialog();

    var strAutoTransferDate = result.AutoTransferDate;
    if (strAutoTransferDate == "1") {
        strAutoTransferDate = strAutoTransferDate + "st";
    }
    else if (strAutoTransferDate == "2") {
        strAutoTransferDate = strAutoTransferDate + "nd";
    }
    else if (strAutoTransferDate == "3") {
        strAutoTransferDate = strAutoTransferDate + "rd";
    }
    else {
        strAutoTransferDate = strAutoTransferDate + "th";
    }

    $("#AccountName").val(result.AccountName);
    $("#BankName").val(result.BankName);
    $("#BankBranchName").val(result.BankBranchName);
    $("#AccountTypeName").val(result.AccountTypeName);
    $("#AccountNo").val(result.AccountNo);

    // strAutoTransferDate
    $("#AutoTransferDateForView").val(strAutoTransferDate);
    $("#AutoTransferDate").val(result.AutoTransferDate);

    //$("#LastestResult").val(result.LastestResult);
    //$("#LastestResultName").val(result.LastestResult);

    $("#BankCode").val(result.BankCode);
    $("#BankBranchCode").val(result.BankBranchCode);
    $("#AccountType").val(result.AccountType);
}

function BLS032Response(result) {

    $("#dlgBLS032").CloseDialog();

    var strExpiredMonth = result.ExpMonth;
    if (result.ExpMonth < 10) {
        strExpiredMonth = "0" + result.ExpMonth;
    }

    $("#CreditCardTypeName").val(result.CreditCardTypeName);
    $("#CardName").val(result.CardName);
    $("#CreditCardCompanyName").val(result.CreditCardCompanyName);
    $("#CreditCardNo").val(result.CreditCardNo);
    $("#CreditCardNoForView").val(result.CreditCardNo);
    $("#ExpMonth").val(strExpiredMonth);
    $("#ExpYear").val(result.ExpYear);
    $("#CreditCardType").val(result.CreditCardType);
    $("#CreditCardCompanyCode").val(result.CreditCardCompanyCode);
}

function initialGridOnload() {
    //Modify by Jutarat A. on 20022013
    //BLS040_GridBillingType = $("#gridBillingType").InitialGrid(pageRow, false, "/Billing/BLS040_InitialGridBillingType");
    BLS040_GridBillingType = $("#gridBillingType").InitialGrid(pageRow, false, "/Billing/BLS040_InitialGridBillingType",
        function () {
            SpecialGridControl(BLS040_GridBillingType, ["Edit", "Remove"]);

            BindOnLoadedEvent(BLS040_GridBillingType,
                function (gen_ctrl) {
                    var colInx = BLS040_GridBillingType.getColIndexById('Remove');
                    var colInx = BLS040_GridBillingType.getColIndexById('Edit');

                    if (CheckFirstRowIsEmpty(BLS040_GridBillingType, false) == false) {

                        for (var i = 0; i < BLS040_GridBillingType.getRowsNum(); i++) {
                            var rowId = BLS040_GridBillingType.getRowId(i);

                            GenerateRemoveButton(BLS040_GridBillingType, "btnRemoveDetail", rowId, "Remove", true);
                            GenerateEditButton(BLS040_GridBillingType, "btnEditDetail", rowId, "Edit", true);

                            // binding grid button event 
                            BindGridButtonClickEvent("btnRemoveDetail", rowId, BtnRemoveBillTypeClick);
                            BindGridButtonClickEvent("btnEditDetail", rowId, BtnEditBillTypeClick);
                        }
                    }

                    BLS040_GridBillingType.setSizes();
                }
            );

        });
    //End Modify
}


function ClearValueHiddenField() {
    $("#CreditCardType").val("");
    $("#CreditCardCompanyCode").val("");
    $("#AutoTransferDate").val("");
    $("#BankCode").val("");
    $("#BankBranchCode").val("");
    $("#AccountType").val("");
    $("#LastestResult").val("");
    $("#OldPaymentMethod").val("");
    $("#OldBillingFlag").val("");
    $("#OldBillingStartDate0").val("");
    $("#OldBillingStartDate1").val("");
    $("#OldBillingStartDate2").val("");
    $("#OldBillingStartDate3").val("");
    $("#OldBillingStartDate4").val("");
    $("#OldBillingStartDate5").val("");
    $("#BillingOfficeCode").val("");
    $("#GridBillingTypeMode").val("");
    $("#EditingRowID").val("");
}

function retrieve_billing_click() {

    $("#ContractCodeProjectCode").val($("#ContractCodeProjectCode").val().toUpperCase());


    var obj = {
        ContractProjectCodeShort: $("#ContractCodeProjectCode").val(),
        BillingOCC: $("#BillingOCC").val()
    };

    ajax_method.CallScreenController("/Billing/BLS040_RetrieveData", obj, function (result, controls) {

        if (controls != undefined) {

            VaridateCtrl(controls, controls);

            return;
        }
        else if (result != undefined && result.doBillingTarget != null) {

            IsQCodeContract = result.IsQCodeContract;


            // Disable control
            $("#BillingClientCode").attr("readonly", true);
            $("#BillingTargetNo").attr("readonly", true);

            if (result.ProductTypeCode != undefined) {

                ajax_method.CallScreenController('/Billing/BLS040_GetBillingType', null, function (result, controls) {
                    if (result.List != null) {
                        regenerate_combo("#BillingTypeList", result);
                    }
                });
            }
            else {

            }

            $("#divBillingBasicInfo").clearForm();
            $("#divFeeInformation").clearForm();
            $("#divPaymentChange").clearForm();
            $("#divBillingHistory").clearForm();
            $("#divAutoTransferInformation").clearForm();
            $("#divCreditCardInformation").clearForm();
            ClearValueHiddenField();

            $("#divBillingBasicInfo").bindJSON(result.doBillingTarget);
            $("#divBillingBasicInfo").bindJSON(result.doBillingBasic);
            $("#divFeeInformation").bindJSON(result.doBillingBasic);

            if (result.doBillingBasic.StopBillingFlag) {
                $("#divFeeInformation #StopBillingFlag").val("1");
            } else {
                $("#divFeeInformation #StopBillingFlag").val("0");
            }

            $("#divPaymentChange").bindJSON(result.doBillingBasic);
            // set value for hidden field
            if (result.doBillingBasic != null) {
                $("#BillingOfficeCode").val(result.doBillingBasic.BillingOfficeCode);

            }


            $("#divAutoTransferInformation").bindJSON(result.dtAutoTransferForView);
            // set value for hidden field
            if (result.dtAutoTransferForView != null) {
                $("#AutoTransferDate").val(result.dtAutoTransferForView.AutoTransferDate);
                $("#BankCode").val(result.dtAutoTransferForView.BankCode);
                $("#BankBranchCode").val(result.dtAutoTransferForView.BankBranchCode);
                $("#AccountType").val(result.dtAutoTransferForView.AccountType);
                $("#LastestResult").val(result.dtAutoTransferForView.LastestResult);
            }


            $("#divCreditCardInformation").bindJSON(result.dtCreditCardForView);
            // set value for hidden field 
            if (result.dtCreditCardForView != null) {
                $("#CreditCardType").val(result.dtCreditCardForView.CreditCardType);
                $("#CreditCardCompanyCode").val(result.dtCreditCardForView.CreditCardCompanyCode);

                //CreditCardNoForView
                if (result.dtCreditCardForView.CreditCardNo != "") {
                    $("#CreditCardNoForView").val(result.dtCreditCardForView.CreditCardNo);
                    $("#CreditCardNo").val(result.dtCreditCardForView.CreditCardNo);

                }

            }



            $("#OldPaymentMethod").val(result.doBillingBasic.PaymentMethod);
            $("#OldBillingFlag").val(result.doBillingBasic.StopBillingFlag);


            if (result.doBillingBasic != undefined && result.doBillingBasic != null) {

                $("#MonthlyBillingAmount").val((new Number(result.doBillingBasic.MonthlyBillingAmount)).numberFormat("#,##0.00"));
                $('#MonthlyBillingAmount').NumericCurrency().val(result.doBillingBasic.MonthlyBillingAmountCurrencyType);
                if (result.doBillingBasic.CarefulFlag == true) {
                    $("#CarefulFlag").attr("checked", true);
                }
                else {
                    $("#CarefulFlag").attr("checked", false);
                }
                if (result.doBillingBasic.VATUnchargedFlag == true) {
                    $("#VATUnchargedFlag").attr("checked", true);
                }
                else {
                    $("#VATUnchargedFlag").attr("checked", false);
                }
                if (result.doBillingBasic.ResultBasedMaintenanceFlag == true) {
                    $("#ResultBasedMaintenanceFlag").attr("checked", true);
                }
                else {
                    $("#ResultBasedMaintenanceFlag").attr("checked", false);
                }

                $("#LastBillingDate").val(ConvertDateObjectToText(result.doBillingBasic.LastBillingDate));
                $("#AdjustEndDate").val(ConvertDateObjectToText(result.doBillingBasic.AdjustEndDate));
                $("#ChangeDate").val(ConvertDateObjectToText(result.doBillingBasic.ChangeDate));
                $("#LastChangeDate").val(ConvertDateObjectToText(result.doBillingBasic.LastChangeDate));

                $("#ChangeDate").datepicker("getDate");
                $("#AdjustEndDate").datepicker("getDate");

            }
            
            /////////////// BIND BILLING TYPE DETAIL DATA //////////////////
            if (result.doBillingTypeDetailList != null) {
                if (result.doBillingTypeDetailList.length > 0) {

                    //Modify by Jutarat A. on 20022013
//                    for (var i = 0; i < result.doBillingTypeDetailList.length; i++) {
//                        var DetailList = [result.doBillingTypeDetailList[i].BillingTypeCode + ": " + result.doBillingTypeDetailList[i].BillingTypeName + "<br />  ",
//                                          "(1) " + result.doBillingTypeDetailList[i].InvoiceDescriptionEN + "<br /> (2) " + result.doBillingTypeDetailList[i].InvoiceDescriptionLC,
//                                          "",
//                                          "",
//                                          "",
//                                          result.doBillingTypeDetailList[i].BillingTypeCode,
//                                          result.doBillingTypeDetailList[i].InvoiceDescriptionEN,  //result.doBillingTypeDetailList[i].BillingTypeNameEN, 
//                                          result.doBillingTypeDetailList[i].InvoiceDescriptionLC   // result.doBillingTypeDetailList[i].BillingTypeNameLC
//                                          ];

//                        CheckFirstRowIsEmpty(BLS040_GridBillingType, true);
//                        AddNewRow(BLS040_GridBillingType, DetailList);

//                        var colInx = BLS040_GridBillingType.getColIndexById('Remove');
//                        var colInx = BLS040_GridBillingType.getColIndexById('Edit');
//                        var rowId = BLS040_GridBillingType.getRowId(BLS040_GridBillingType.getRowsNum() - 1);
//                        GenerateRemoveButton(BLS040_GridBillingType, "btnRemoveDetail", rowId, "Remove", true);
//                        GenerateEditButton(BLS040_GridBillingType, "btnEditDetail", rowId, "Edit", true);
//                        // binding grid button event 
//                        BindGridButtonClickEvent("btnRemoveDetail", rowId, BtnRemoveBillTypeClick);
//                        BindGridButtonClickEvent("btnEditDetail", rowId, BtnEditBillTypeClick);
//                    }
                    $("#gridBillingType").LoadDataToGrid(BLS040_GridBillingType, pageRow, false, "/Billing/BLS040_GetBillingDetailListData", result.doBillingTypeDetailList, "doBillingTypeDetailList", false, null, null);
                    //End Modify
                }
            }
            //////////////////////////////////////////////////

            /////////////// BIND BILLING HISTORY //////////////////


            $("#BillingStartDate0").EnableDatePicker(false);
            $("#BillingStartDate1").EnableDatePicker(false);
            $("#BillingStartDate2").EnableDatePicker(false);
            $("#BillingStartDate3").EnableDatePicker(false);
            $("#BillingStartDate4").EnableDatePicker(false);
            $("#BillingStartDate5").EnableDatePicker(false);

            //Add by Jutarat A. on 25042013
            $("#MonthlyBillingAmount0").attr("readonly", true);
            $("#MonthlyBillingAmount1").attr("readonly", true);
            $("#MonthlyBillingAmount2").attr("readonly", true);
            $("#MonthlyBillingAmount3").attr("readonly", true);
            $("#MonthlyBillingAmount4").attr("readonly", true);
            $("#MonthlyBillingAmount5").attr("readonly", true);

            // Add by Jirawat jannet. on 2016-08-18
            $("#MonthlyBillingAmount0").NumericCurrency().attr("disabled", true);
            $("#MonthlyBillingAmount1").NumericCurrency().attr("disabled", true);
            $("#MonthlyBillingAmount2").NumericCurrency().attr("disabled", true);
            $("#MonthlyBillingAmount3").NumericCurrency().attr("disabled", true);
            $("#MonthlyBillingAmount4").NumericCurrency().attr("disabled", true);
            $("#MonthlyBillingAmount5").NumericCurrency().attr("disabled", true);

            //End Add


            if (result.doTbt_MonthlyBillingHistoryList != null) {
                if (result.doTbt_MonthlyBillingHistoryList.length > 0) {
                    for (var i = 0; i < result.doTbt_MonthlyBillingHistoryList.length; i++) {
                        if (i > 5) break;

                        var MonthlyBillingAmount = result.doTbt_MonthlyBillingHistoryList[i].MonthlyBillingAmount;
                        var BillingStartDate = result.doTbt_MonthlyBillingHistoryList[i].BillingStartDate;
                        var currencyVal = result.doTbt_MonthlyBillingHistoryList[i].MonthlyBillingAmountCurrencyType; // Add by Jirawat jannet. on 2016-08-18
                        if (currencyVal == null) currencyVal = '1'; // Add by Jirawat jannet. on 2016-08-18

                        $("#MonthlyBillingAmount" + i).NumericCurrency().val(currencyVal); // Add by Jirawat jannet. on 2016-08-18
                        $("#MonthlyBillingAmount" + i).val((new Number(MonthlyBillingAmount)).numberFormat("#,##0.00"));
                        $("#BillingStartDate" + i).val(ConvertDateObjectToText(BillingStartDate));
                        $("#OldBillingStartDate" + i).val(ConvertDateObjectToText(BillingStartDate));

                        // tt
                        $("#BillingStartDate" + i).datepicker("getDate");

                        // Enable
                        $("#BillingStartDate" + i).EnableDatePicker(true);

                        $("#MonthlyBillingAmount" + i).attr("readonly", false); //Add by Jutarat A. on 25042013
                        //$("#MonthlyBillingAmount" + i).NumericCurrency().attr("disabled", false); // Disabled wait for spect #####################
                        $("#MonthlyBillingAmount" + i).NumericCurrency().attr("disabled", true);
                    }
                }
            }

            //////////////////////////////////////////////////

            //================= Set screen after retrieve =========================
            $("#DebtTracingOfficeCode").SetDisabled(false);
            $("#divBillingType").SetDisabled(false);
            $("#PaymentMethod").SetDisabled(false);
            $("#BillingCycle").SetDisabled(false);
            $("#CreditTerm").SetDisabled(false);
            $("#CalDailyFeeStatus").SetDisabled(false);
            $("#AdjustEndDate").EnableDatePicker(true);
            $("#SortingType").SetDisabled(false);
            $("#StopBillingFlag").SetDisabled(false);
            $("#btnAutoTransferInfo").SetDisabled(false);
            $("#btnCreditCardInfo").SetDisabled(false);
            $("#ChangeDate").EnableDatePicker(true);
            $("#ApproveNo").SetDisabled(false);


            $("#VATUnchargedFlag").SetDisabled(false);
            $("#ResultBasedMaintenanceFlag").SetDisabled(false);
            //=====================================================================

            $("#ContractCodeProjectCode").val(result.ContractProjectCodeShort);
            $("#BillingClientCode").val(result.BillingClientCodeShort);

            $("#ContractCodeProjectCode").attr("readonly", true);
            $("#BillingOCC").attr("readonly", true);
            $("#btnRetrieve").attr("disabled", true);
            $("#btnRetrieveBillingTarget").attr("disabled", true);
            $("#btnSearchBillingTarget").attr("disabled", true);
            $("#btnClearBillingTarget").attr("disabled", false);
            //========== set screen from condition ==============


            if ($("#CarefulFlag").prop("checked") == true) {
                DisableControlForCrarefullSpacial();
            }



            change_payment_method();
            InitialCommandButton(1);


            $("#CarefulFlag").attr("disabled", !result.EnableCarefulSpecial);

            $("#btnAutoTransferInfo").attr("disabled", !result.EnableAutoTransfer);

            $("#btnCreditCardInfo").attr("disabled", !result.EnableCreditCard);



            // Check DebtTracingOfficeCode is exist in ddl (Narupon)
            ClearTempObjectDebtTracingOffice();
            if (result.doBillingBasic.DebtTracingOfficeCode != "" && result.doBillingBasic.DebtTracingOfficeCode != null) {


                if ($("#DebtTracingOfficeCode").find(":contains('" + result.doBillingBasic.DebtTracingOfficeCode + "')").length == 0) {

                    $('#DebtTracingOfficeCode').append($('<option></option>').val(result.doBillingBasic.DebtTracingOfficeCode).html(result.doBillingBasic.DebtTracingOfficeCodeName));

                    $("#DebtTracingOfficeCode").val(result.doBillingBasic.DebtTracingOfficeCode);
                    $("#DebtTracingOfficeCode").attr("disabled", true);

                    objDebtTracingOffice = {
                        DebtTracingOfficeCode: result.doBillingBasic.DebtTracingOfficeCode,
                        DebtTracingOfficeCodeName: result.doBillingBasic.DebtTracingOfficeCodeName
                    };

                } else {
                    $("#DebtTracingOfficeCode").val(result.doBillingBasic.DebtTracingOfficeCode);
                    $("#DebtTracingOfficeCode").attr("disabled", false);
                }

            }


            // tt -> if it is Q_Code Contract
            if (IsQCodeContract) {

                // disable
                $("#BillingCycle").SetDisabled(true);
                $("#CreditTerm").SetDisabled(true);
                $("#AdjustEndDate").EnableDatePicker(false);
                $("#StopBillingFlag").SetDisabled(true);

                // set value
                $("#BillingCycle").val("0");
                $("#CreditTerm").val("0");
                $("#AdjustEndDate").val("");

            }
        }
        //else {  // comment by Narupon
        //
        //    setInitialState();
        //}



    });

}

function ClearTempObjectDebtTracingOffice() {

    if (objDebtTracingOffice != undefined) {
        $("#DebtTracingOfficeCode option[value='" + objDebtTracingOffice.DebtTracingOfficeCode + "']").remove();
    }

    objDebtTracingOffice = undefined;
    $("#DebtTracingOfficeCode").val("");

}

function setInitialState() {
    // --------------- Initial SCREEN ------------------
    $("#divBillingBasicInfo").clearForm();
    $("#divFeeInformation").clearForm();
    $("#divPaymentChange").clearForm();
    $("#divBillingHistory").clearForm();
    $("#divAutoTransferInformation").clearForm();
    $("#divCreditCardInformation").clearForm();
    $("#CreditCardNoForView").val("");
    $("#CreditCardNo").val("");

    $("#ContractCodeProjectCode").attr("readonly", false);
    $("#BillingOCC").attr("readonly", false);

    $("#BillingClientCode").attr("readonly", true);
    $("#BillingTargetNo").attr("readonly", true);

    $("#btnRetrieve").attr("disabled", false);
    $("#btnRetrieveBillingTarget").attr("disabled", true);
    $("#btnSearchBillingTarget").attr("disabled", true);
    $("#btnClearBillingTarget").attr("disabled", true);

    $("#DebtTracingOfficeCode").attr("disabled", true);
    $("#divBillingType").SetDisabled(true);
    $("#PaymentMethod").SetDisabled(true);
    $("#BillingCycle").SetDisabled(true);
    $("#CreditTerm").SetDisabled(true);
    $("#CalDailyFeeStatus").SetDisabled(true);
    $("#AdjustEndDate").EnableDatePicker(false);
    $("#SortingType").SetDisabled(true);
    $("#StopBillingFlag").SetDisabled(true);
    $("#btnAutoTransferInfo").SetDisabled(true);
    $("#btnCreditCardInfo").SetDisabled(true);
    $("#ChangeDate").EnableDatePicker(false);
    $("#ApproveNo").SetDisabled(true);
    $("#BillingStartDate0").EnableDatePicker(false);
    $("#BillingStartDate1").EnableDatePicker(false);
    $("#BillingStartDate2").EnableDatePicker(false);
    $("#BillingStartDate3").EnableDatePicker(false);
    $("#BillingStartDate4").EnableDatePicker(false);
    $("#BillingStartDate5").EnableDatePicker(false);

    //Add by Jutarat A. on 25042013
    $("#MonthlyBillingAmount0").attr("readonly", true);
    $("#MonthlyBillingAmount1").attr("readonly", true);
    $("#MonthlyBillingAmount2").attr("readonly", true);
    $("#MonthlyBillingAmount3").attr("readonly", true);
    $("#MonthlyBillingAmount4").attr("readonly", true);
    $("#MonthlyBillingAmount5").attr("readonly", true);
    //End Add

    //Add by Jirawat Jannet on 2016-08-18
    $("#MonthlyBillingAmount0").NumericCurrency().attr("disabled", true);
    $("#MonthlyBillingAmount1").NumericCurrency().attr("disabled", true);
    $("#MonthlyBillingAmount2").NumericCurrency().attr("disabled", true);
    $("#MonthlyBillingAmount3").NumericCurrency().attr("disabled", true);
    $("#MonthlyBillingAmount4").NumericCurrency().attr("disabled", true);
    $("#MonthlyBillingAmount5").NumericCurrency().attr("disabled", true);
    //End Add

    $("#VATUnchargedFlag").SetDisabled(true);
    $("#ResultBasedMaintenanceFlag").SetDisabled(true);

    $("#CarefulFlag").SetDisabled(true);

    InitialCommandButton(0);


    // hide section
    $("#divAutoTransferInformation").hide();
    $("#divCreditCardInformation").hide();

    
}


function clear_installation_click() {

    // Get Message
    var obj = {
        module: "Common",
        code: "MSG0044"
    };

    ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message, clearAllScreen, function () {

        });

    });

}

function clearAllScreen() {
    setInitialState();

}


function BtnRemoveBillTypeClick(row_id) {
    if ($("#GridBillingTypeMode").val() == "") {
        var selectedRowIndex = BLS040_GridBillingType.getRowIndex(row_id);
        
        //var BillingType = BLS040_GridBillingType.cells2(selectedRowIndex, BLS040_GridBillingType.getColIndexById('BillingType')).getValue();
        var BillingType = BLS040_GridBillingType.cells2(selectedRowIndex, BLS040_GridBillingType.getColIndexById('BillingTypeCode')).getValue(); //Modify by Jutarat A. on 20022013

        var obj = {
            BillingTypeCode: BillingType,
            BillingFlag: $("#StopBillingFlag").val()
        };

        ajax_method.CallScreenController("/Billing/BLS040_RemoveBillingTypeClick", obj, function (result, controls, isWarning) {
            if (result == true) {
                DeleteRow(BLS040_GridBillingType, row_id);
            }
        });
    }
}

function BtnRegisterClick() {
    var registerData_obj = {};
}



function BtnClearClick() {
    $("#EmailAddress").val("");

}



function InitialCommandButton(step) {
    if (step == 0) {
        SetRegisterCommand(false, null);
        SetResetCommand(false, null);
        SetApproveCommand(false, null);
        SetRejectCommand(false, null);
        SetReturnCommand(false, null);
        SetCloseCommand(false, null);
        SetConfirmCommand(false, null);
        SetBackCommand(false, null);
    }
    else if (step == 1) {
        SetRegisterCommand(true, command_register_click);
        //SetResetCommand(true, command_reset_click);
        reset_command.SetCommand(command_reset_click);
        SetApproveCommand(false, null);
        SetRejectCommand(false, null);
        SetReturnCommand(false, null);
        SetCloseCommand(false, null);
        SetConfirmCommand(false, null);
        SetBackCommand(false, null);
    }
    else if (step == 2) {
        SetRegisterCommand(false, null);
        SetResetCommand(false, null);
        SetApproveCommand(false, null);
        SetRejectCommand(false, null);
        SetReturnCommand(false, null);
        SetCloseCommand(false, null);
        SetConfirmCommand(true, command_confirm_click);
        SetBackCommand(true, command_back_click);
    }
    else if (step == 3) {
        SetRegisterCommand(false, null);
        SetResetCommand(false, null);
        SetApproveCommand(false, null);
        SetRejectCommand(false, null);
        SetReturnCommand(false, null);
        SetCloseCommand(false, null);
        SetConfirmCommand(false, null);
        SetBackCommand(false, null);
    }
    else if (step == 4) {
        SetRegisterCommand(false, null);
        SetResetCommand(false, null);
        SetApproveCommand(true, command_approve_click);
        SetRejectCommand(true, command_reject_click);
        SetReturnCommand(true, command_return_click);
        SetCloseCommand(true, command_close_click);
        SetConfirmCommand(false, null);
        SetBackCommand(false, null);
    }
}

function command_back_click() {

    $("#BillingTypeInput").show();

    InitialCommandButton(1);

    $("#divBillingBasicInfo").SetViewMode(false);
    $("#divFeeInformation").SetViewMode(false);
    $("#divPaymentChange").SetViewMode(false);
    $("#divBillingHistory").SetViewMode(false);
    $("#divAutoTransferInformation").SetViewMode(false);
    $("#divCreditCardInformation").SetViewMode(false);

    enabledGridBillingType();
}

function command_register_click() {

    command_control.CommandControlMode(false);



    var obj = CreateObjectData($("#form1").serialize() + "&ContractProjectCodeShort=" + $("#ContractCodeProjectCode").val());

    obj.VATUnchargedFlag = $("#VATUnchargedFlag").val();
    obj.ResultBasedMaintenanceFlag = $("#ResultBasedMaintenanceFlag").val();
    obj.PaymentMethod = $("#PaymentMethod").val();
    obj.CalDailyFeeStatus = $("#CalDailyFeeStatus").val();
    obj.StopBillingFlag = $("#StopBillingFlag").val();

    if (obj.StopBillingFlag == "1") {
        obj.StopBillingFlag = "True";
    }
    else {
        obj.StopBillingFlag = "False";
    }

    ajax_method.CallScreenController("/Billing/BLS040_ValidateBeforeRegister", obj, function (result, controls, isWarning) {
        command_control.CommandControlMode(true);


        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(controls, controls); //VaridateCtrl(["ContractCodeProjectCode"], controls); //Modify by Jutarat A. on 14062013
            /* --------------------- */

            return;
        }
        else if (result == true && isWarning == undefined) {


            var message_list = [];
            var messageCode = "";

            if (ValidatePaymentMothod_autoTransfer() == false) {
                message_list.push(C_MSG6016);
                messageCode = "MSG6016";
            }

            if (ValidatePaymentMothod_creditCard() == false) {
                message_list.push(C_MSG6017);
                messageCode = "MSG6017";
            }

            if (ValidateBillingStartDate() == false) {
                message_list.push(C_MSG6022);
                messageCode = "MSG6022";
            }

            var strMessage = "";



            for (var i = 0; i < message_list.length; i++) {
                strMessage += message_list[i] + "</br></br>";
            }

            if (message_list.length > 0) {
                OpenYesNoMessageDialog(messageCode, strMessage, function () { setConfirmState(); });

            } else {
                setConfirmState();
            }


        }


    });
}





function setConfirmState() {
    InitialCommandButton(2);

    $("#BillingTypeInput").hide();

    $("#divBillingBasicInfo").SetViewMode(true);
    $("#divFeeInformation").SetViewMode(true);
    $("#divPaymentChange").SetViewMode(true);
    $("#divBillingHistory").SetViewMode(true);
    $("#divAutoTransferInformation").SetViewMode(true);
    $("#divCreditCardInformation").SetViewMode(true);

    disabledGridBillingType();
}

function setSuccessRegisState() {
    // --------------- Initial SCREEN ------------------

    $("#BillingTypeInput").show();

    $("#divBillingBasicInfo").SetViewMode(false);
    $("#divFeeInformation").SetViewMode(false);
    $("#divPaymentChange").SetViewMode(false);
    $("#divBillingHistory").SetViewMode(false);
    $("#divAutoTransferInformation").SetViewMode(false);
    $("#divCreditCardInformation").SetViewMode(false);

    InitialCommandButton(0);
    setInitialState();
    DeleteAllRow(BLS040_GridBillingType);

    // tt
    enabledGridBillingType();

    // tt
    IsQCodeContract = false;
}

function command_confirm_click() {
    command_control.CommandControlMode(false);
    var CarefulFlag = $("#CarefulFlag").prop("checked");
    var VATUnchargedFlag = $("#VATUnchargedFlag").prop("checked");
    var ResultBasedMaintenanceFlag = $("#ResultBasedMaintenanceFlag").prop("checked");

    var obj = CreateObjectData($("#form1").serialize() + "&ContractProjectCodeShort=" + $("#ContractCodeProjectCode").val() + "&CarefulFlag=" + CarefulFlag + "&VATUnchargedFlag=" + VATUnchargedFlag + "&ResultBasedMaintenanceFlag=" + ResultBasedMaintenanceFlag);

    obj.PaymentMethod = $("#PaymentMethod").val();
    obj.CalDailyFeeStatus = $("#CalDailyFeeStatus").val();
    obj.StopBillingFlag = $("#StopBillingFlag").val();

    //Add by Jutarat A. on 25042013
    obj.MonthlyBillingAmount0 = $("#MonthlyBillingAmount0").NumericValue();
    obj.MonthlyBillingAmount1 = $("#MonthlyBillingAmount1").NumericValue();
    obj.MonthlyBillingAmount2 = $("#MonthlyBillingAmount2").NumericValue();
    obj.MonthlyBillingAmount3 = $("#MonthlyBillingAmount3").NumericValue();
    obj.MonthlyBillingAmount4 = $("#MonthlyBillingAmount4").NumericValue();
    obj.MonthlyBillingAmount5 = $("#MonthlyBillingAmount5").NumericValue();

    //End Add

    // Add bt Jirawat Jannet o 2016-08-18
    obj.MonthlyBillingAmount0MulC = getCurrencyVal('#MonthlyBillingAmount0');
    obj.MonthlyBillingAmount1MulC = getCurrencyVal('#MonthlyBillingAmount1');
    obj.MonthlyBillingAmount2MulC = getCurrencyVal('#MonthlyBillingAmount2');
    obj.MonthlyBillingAmount3MulC = getCurrencyVal('#MonthlyBillingAmount3');
    obj.MonthlyBillingAmount4MulC = getCurrencyVal('#MonthlyBillingAmount4');
    obj.MonthlyBillingAmount5MulC = getCurrencyVal('#MonthlyBillingAmount5');
    // End Add

    if (obj.StopBillingFlag == "1") {
        obj.StopBillingFlag = "True";
    }
    else {
        obj.StopBillingFlag = "False";
    }

    ajax_method.CallScreenController("/Billing/BLS040_RegisterData", obj, function (result, controls, isWarning) {

        command_control.CommandControlMode(true);

        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["ContractCodeProjectCode"], controls);
            /* --------------------- */
            return;
        }
        else if (result != undefined && isWarning == undefined) {

            var obj = {
                module: "Common",
                code: "MSG0046",
                param: ""
            };

            ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {

                OpenInformationMessageDialog(result.Code, result.Message);
            });

            setSuccessRegisState();

            
        }
    });


}

function command_reset_click() {


    ajax_method.CallScreenController("/Billing/BLS040_ClearBillingTarget", "", function (result, controls, isWarning) {
        setInitialState();
        DeleteAllRow(BLS040_GridBillingType);

        // tt
        IsQCodeContract = false;
    });

}



function convertDatetoYMD(ctrl) {
    var ctxt = ctrl.val();
    if (ctxt != "") {
        var instance = ctrl.data("datepicker");
        var ddate = instance.selectedDay;
        if (ddate < 10)
            ddate = "0" + ddate;
        var dmonth = instance.selectedMonth + 1;
        if (dmonth < 10)
            dmonth = "0" + dmonth;
        var dyear = instance.selectedYear;

        var txt = "" + dyear + dmonth + ddate;
        return txt;
    }
}

function getCurrentDateFormatYMD() {
    var myNow = new Date();
    var ddate = myNow.getDate();
    if (ddate < 10)
        ddate = "0" + ddate;
    var dmonth = myNow.getMonth() + 1;
    if (dmonth < 10)
        dmonth = "0" + dmonth;
    var dyear = myNow.getFullYear();

    var txt = "" + dyear + dmonth + ddate;
    return txt;
}

function disabledGridBillingType() {
    //////// DISABLED BUTTON In EMAIL GRID ///////////
    var colInx = BLS040_GridBillingType.getColIndexById("Remove");
    BLS040_GridBillingType.setColumnHidden(colInx, true);
    var colInx = BLS040_GridBillingType.getColIndexById("Edit");
    BLS040_GridBillingType.setColumnHidden(colInx, true);
    //////////////////////////////////////////////////
}

function enabledGridBillingType() {
    var colInx = BLS040_GridBillingType.getColIndexById("Remove");
    BLS040_GridBillingType.setColumnHidden(colInx, false);
    var colInx = BLS040_GridBillingType.getColIndexById("Edit");
    BLS040_GridBillingType.setColumnHidden(colInx, false);
    SetFitColumnForBackAction(BLS040_GridBillingType, "TempColumn");
    //////////////////////////////////////////////////
}

function doAlert(moduleCode, msgCode, paramObj) {
    var obj = {
        module: moduleCode,
        code: msgCode,
        param: paramObj
    };

    ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
        OpenWarningDialog(result.Code, result.Message, null);
    });
}

function add_billing_type_click() {

    var obj = {
        BillingTypeCode: $("#BillingTypeList").val(),
        InvoiceDescriptionEN: $("#InvoiceDescriptionEN").val(),
        InvoiceDescriptionLC: $("#InvoiceDescriptionLC").val()
    };

    ajax_method.CallScreenController("/Billing/BLS040_AddBillingType", obj, function (result, controls) {
        if (controls != undefined || result == null) {

        }
        else if (result.length > 0) {
            //var BillingTypeLst = [result[0].BillingTypeCode + ": " + result[0].BillingTypeName + "<br />  ", "(1) " + result[0].BillingTypeNameEN + "<br /> (2) " + result[0].BillingTypeNameLC, "", result[0].BillingTypeCode]
            var BillingTypeLst = [result[0].BillingTypeCode + ": " + result[0].BillingTypeName + "<br />  ", "(1) " + result[0].BillingTypeNameEN + "<br /> (2) " + result[0].BillingTypeNameLC, "", "", "", result[0].BillingTypeCode, result[0].BillingTypeNameEN, result[0].BillingTypeNameLC];
            CheckFirstRowIsEmpty(BLS040_GridBillingType, true);
            AddNewRow(BLS040_GridBillingType, BillingTypeLst);

            var colInx = BLS040_GridBillingType.getColIndexById('Remove');
            var colInx = BLS040_GridBillingType.getColIndexById('Edit');
            var rowId = BLS040_GridBillingType.getRowId(BLS040_GridBillingType.getRowsNum() - 1);
            GenerateRemoveButton(BLS040_GridBillingType, "btnRemoveDetail", rowId, "Remove", true);
            GenerateEditButton(BLS040_GridBillingType, "btnEditDetail", rowId, "Edit", true);
            // binding grid button event 
            BindGridButtonClickEvent("btnRemoveDetail", rowId, BtnRemoveBillTypeClick);
            BindGridButtonClickEvent("btnEditDetail", rowId, BtnEditBillTypeClick);



            BLS040_GridBillingType.setSizes();

            $("#BillingTypeList").val("");
            $("#InvoiceDescriptionEN").val("");
            $("#InvoiceDescriptionLC").val("");
        }
    });

}

function update_billing_type() {

    var obj = {
        BillingTypeCode: $("#BillingTypeList").val()
        , InvoiceDescriptionEN: $("#InvoiceDescriptionEN").val()
        , InvoiceDescriptionLC: $("#InvoiceDescriptionLC").val()
    };

    ajax_method.CallScreenController("/Billing/BLS040_UpdateBillingType", obj, function (result, controls) {
        if (controls != undefined || result == null) {

        }
        else if (result.length > 0) {

            BLS040_GridBillingType.setSizes();
        }
    });

}

function BtnEditBillTypeClick(row_id) {
    if ($("#GridBillingTypeMode").val() == "") {
        var selectedRowIndex = BLS040_GridBillingType.getRowIndex(row_id);

        //var BillingType = BLS040_GridBillingType.cells2(selectedRowIndex, BLS040_GridBillingType.getColIndexById('BillingType')).getValue();
        var BillingType = BLS040_GridBillingType.cells2(selectedRowIndex, BLS040_GridBillingType.getColIndexById('BillingTypeCode')).getValue(); //Modify by Jutarat A. on 20022013

        var InvoiceDescriptionEN = BLS040_GridBillingType.cells2(selectedRowIndex, BLS040_GridBillingType.getColIndexById('InvoiceDescriptionEN')).getValue();
        var InvoiceDescriptionLC = BLS040_GridBillingType.cells2(selectedRowIndex, BLS040_GridBillingType.getColIndexById('InvoiceDescriptionLC')).getValue();
        $("#BillingTypeList").val(BillingType);
        $("#InvoiceDescriptionEN").val(InvoiceDescriptionEN);
        $("#InvoiceDescriptionLC").val(InvoiceDescriptionLC);
        $("#GridBillingTypeMode").val("EDIT");
        $("#BillingTypeList").attr("disabled", true);
        $("#EditingRowID").val(row_id);
    }
}

function btnAddUpdateBillingType_click() {
    if ($("#GridBillingTypeMode").val() == "EDIT") {

        update_billing_type();

        var row_id = $("#EditingRowID").val();
        var selectedRowIndex = BLS040_GridBillingType.getRowIndex(row_id);

        //BLS040_GridBillingType.cells2(selectedRowIndex, BLS040_GridBillingType.getColIndexById('BillingType')).setValue($("#BillingTypeList").val());
        BLS040_GridBillingType.cells2(selectedRowIndex, BLS040_GridBillingType.getColIndexById('BillingTypeCode')).setValue($("#BillingTypeList").val()); //Modify by Jutarat A. on 20022013

        BLS040_GridBillingType.cells2(selectedRowIndex, BLS040_GridBillingType.getColIndexById('InvoiceDescription')).setValue("(1) " + $("#InvoiceDescriptionEN").val() + "<br />(2) " + $("#InvoiceDescriptionLC").val());
        BLS040_GridBillingType.cells2(selectedRowIndex, BLS040_GridBillingType.getColIndexById('InvoiceDescriptionEN')).setValue($("#InvoiceDescriptionEN").val());
        BLS040_GridBillingType.cells2(selectedRowIndex, BLS040_GridBillingType.getColIndexById('InvoiceDescriptionLC')).setValue($("#InvoiceDescriptionLC").val());
        $("#GridBillingTypeMode").val("");
        $("#EditingRowID").val("");
        $("#InvoiceDescriptionEN").val("");
        $("#InvoiceDescriptionLC").val("");
        $("#BillingTypeList").val("");
        $("#BillingTypeList").attr("disabled", false);

        //Unlock
        BLS040_IsEditingBillingType = false;
    }
    else {
        add_billing_type_click();
    }
}

function ConvertDateObjectToText(strDate) {
    if (strDate != null) {
        strDate = ConvertDateToTextFormat(strDate.replace('/Date(', '').replace(')/', '') * 1);
        return strDate;
    }
    else {
        return "";
    }
}

function btnClearBillingTarget_click() {
    $("#btnClearBillingTarget").attr("disabled", true);
    ajax_method.CallScreenController("/Billing/BLS040_ClearBillingTarget", "", function (result, controls, isWarning) {

        // tt
        $("#BillingOfficeCodeName").val("");
        $("#BillingOfficeCode").val("");

        $("#BillingClientCode").val("");
        $("#BillingTargetNo").val("");
        $("#CustTypeName").val("");
        $("#FullNameEN").val("");
        $("#BranchNameEN").val("");
        $("#AddressEN").val("");
        $("#FullNameLC").val("");
        $("#BranchNameLC").val("");
        $("#AddressLC").val("");

        $("#BillingClientCode").attr("readonly", false);
        $("#BillingTargetNo").attr("readonly", false);

        $("#btnRetrieveBillingTarget").attr("disabled", false);
        $("#btnSearchBillingTarget").attr("disabled", false);

        var carefulFlag = $("#CarefulFlag").prop("checked");
        if (carefulFlag == false) {
            $("#btnClearBillingTarget").attr("disabled", false);
        }

    });
}

function checkCarefulFlag() {

    var carefulFlag = $("#CarefulFlag").prop("checked");
    if (carefulFlag) {

        var obj = {
            module: "Billing",
            code: "MSG6015"
        };

        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
            OpenYesNoMessageDialog(result.Code, result.Message, function () {

                DisableControlForCrarefullSpacial();

            }, function () {
                $("#CarefulFlag").attr("checked", false);
                $("#AdjustEndDate").focus();
            });

        });

    }
    else {


        $("#DebtTracingOfficeCode").SetDisabled(false);
        $("#divBillingType").SetDisabled(false);
        $("#PaymentMethod").SetDisabled(false);

        if (IsQCodeContract == false) {
            $("#BillingCycle").SetDisabled(false);
            $("#CreditTerm").SetDisabled(false);
            $("#AdjustEndDate").EnableDatePicker(true);
            $("#StopBillingFlag").SetDisabled(false);
        }

        $("#CalDailyFeeStatus").SetDisabled(false);
        $("#SortingType").SetDisabled(false);
        $("#btnAutoTransferInfo").SetDisabled(false);
        $("#btnCreditCardInfo").SetDisabled(false);

        $("#ChangeDate").EnableDatePicker(true);
        $("#ApproveNo").SetDisabled(false);
        if ($("#BillingStartDate0").val() != "")
            $("#BillingStartDate0").EnableDatePicker(true);
        if ($("#BillingStartDate1").val() != "")
            $("#BillingStartDate1").EnableDatePicker(true);
        if ($("#BillingStartDate2").val() != "")
            $("#BillingStartDate2").EnableDatePicker(true);
        if ($("#BillingStartDate3").val() != "")
            $("#BillingStartDate3").EnableDatePicker(true);
        if ($("#BillingStartDate4").val() != "")
            $("#BillingStartDate4").EnableDatePicker(true);
        if ($("#BillingStartDate5").val() != "")
            $("#BillingStartDate5").EnableDatePicker(true);

        $("#VATUnchargedFlag").SetDisabled(false);
        $("#ResultBasedMaintenanceFlag").SetDisabled(false);

        $("#btnClearBillingTarget").SetDisabled(false);

        var colInx = BLS040_GridBillingType.getColIndexById("Remove");
        BLS040_GridBillingType.setColumnHidden(colInx, false);
        var colInx = BLS040_GridBillingType.getColIndexById("Edit");
        BLS040_GridBillingType.setColumnHidden(colInx, false);

        SetFitColumnForBackAction(BLS040_GridBillingType, "TempColumn");
    }
}

function DisableControlForCrarefullSpacial() {
    // disable control

    $("#DebtTracingOfficeCode").SetDisabled(true);
    $("#divBillingType").SetDisabled(true);
    $("#PaymentMethod").SetDisabled(true);
    $("#BillingCycle").SetDisabled(true);
    $("#CreditTerm").SetDisabled(true);
    $("#CalDailyFeeStatus").SetDisabled(true);
    $("#AdjustEndDate").EnableDatePicker(false);
    $("#SortingType").SetDisabled(true);
    $("#StopBillingFlag").SetDisabled(true);
    $("#btnAutoTransferInfo").SetDisabled(true);
    $("#btnCreditCardInfo").SetDisabled(true);

    $("#ChangeDate").EnableDatePicker(false);
    $("#ApproveNo").SetDisabled(true);
    $("#BillingStartDate0").EnableDatePicker(false);
    $("#BillingStartDate1").EnableDatePicker(false);
    $("#BillingStartDate2").EnableDatePicker(false);
    $("#BillingStartDate3").EnableDatePicker(false);
    $("#BillingStartDate4").EnableDatePicker(false);
    $("#BillingStartDate5").EnableDatePicker(false);

    //Add by Jutarat A. on 25042013
    $("#MonthlyBillingAmount0").attr("readonly", true);
    $("#MonthlyBillingAmount1").attr("readonly", true);
    $("#MonthlyBillingAmount2").attr("readonly", true);
    $("#MonthlyBillingAmount3").attr("readonly", true);
    $("#MonthlyBillingAmount4").attr("readonly", true);
    $("#MonthlyBillingAmount5").attr("readonly", true);
    //End Add

    $("#VATUnchargedFlag").SetDisabled(true);
    $("#ResultBasedMaintenanceFlag").SetDisabled(true);

    $("#btnClearBillingTarget").SetDisabled(true);

    var colInx = BLS040_GridBillingType.getColIndexById("Remove");
    BLS040_GridBillingType.setColumnHidden(colInx, true);
    var colInx = BLS040_GridBillingType.getColIndexById("Edit");
    BLS040_GridBillingType.setColumnHidden(colInx, true);
}

function btnRetrieveBillingTarget_click() {

    $("#BillingClientCode").val($("#BillingClientCode").val().toUpperCase());


    var obj = {
        BillingClientCode: $("#BillingClientCode").val()
        , BillingTargetRunningNo: $("#BillingTargetNo").val()
    };

    ajax_method.CallScreenController("/Billing/BLS040_GetBillingTypeTargetNew", obj, function (result, controls, isWarning) {
        if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
        else if (result != undefined && isWarning == undefined) {
            $("#divBillingTargetForView").bindJSON(result);

            $("#BillingClientCode").attr("readonly", true);
            $("#BillingTargetNo").attr("readonly", true);

            $("#btnRetrieveBillingTarget").attr("disabled", true);
            $("#btnSearchBillingTarget").attr("disabled", true);

            var str = result.BillingTargetCode_Short.split("-");
            if (str.length == 2) {
                $("#BillingClientCode").val(str[0]);
                $("#BillingTargetNo").val(str[1]);
            }

            // tt
            $("#BillingOfficeCode").val(result.BillingOfficeCode);

        }
    });
}

function btnCancelBillingType_click() {

    $("#GridBillingTypeMode").val("");
    $("#EditingRowID").val("");
    $("#InvoiceDescriptionEN").val("");
    $("#InvoiceDescriptionLC").val("");
    $("#BillingTypeList").val("");
    $("#BillingTypeList").attr("disabled", false);
}

function CMS470Response(result_BillingTarget) {

    var obj = result_BillingTarget;
    $("#dlgBLS040").CloseDialog();
    if (obj == null) {
        return;
    }
    else {
        var str = obj.BillingTargetCode_Short.split("-");
        if (str.length == 2) {
            $("#BillingClientCode").val(str[0]);
            $("#BillingTargetNo").val(str[1]);
        }
    }
}



// Add function by Jirawat Jannet on 2017-08-18
function getCurrencyVal(id) {
    var controlObj = $(id).NumericCurrency();
    var iputObj = $(id);
    if (iputObj.is('[readonly]')) return null;
    else return controlObj.val()
}