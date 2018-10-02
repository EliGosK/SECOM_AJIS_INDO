/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

/// <reference path="../Base/object/ajax_method.js" />


var mygridBillingOCC;
var mygridBillingTypeDetail;

var BillingOCC;
var strBillingOCC
$(document).ready(function () {

    if (CMS420.CallerScreenID != 'CMS400') {
        IntialGridBillingOCC();
        $("#BillingBasicInformation").hide();

        mygridBillingTypeDetail = $("#gridBillingTypeDetail").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/Common/CMS420_InitGrid_BillingTypeDetail");

        if ($("#gridBillingTypeDetail").length > 0) {
            $("#gridBillingTypeDetail").LoadDataToGrid(mygridBillingTypeDetail, ROWS_PER_PAGE_FOR_VIEWPAGE
            , false
            , "/Common/CMS420_LoadGridBillingTypeDetail"
            , null
            , "doBillingTypeDetailList"
            , false
            , function () { }
            , function () { }
        );
        }
    }
    else {
        IntialGridBillingTypeDetail();
        $("#BillingOccurrenceList").hide();
    }

    InitialTextLink();

    IntialButtonLink();

    BillingOCC = CMS420.BillingOCC;
    $("#divAll").SetEmptyViewData();

    if (CMS420.IsSpecialCareful == "1") {
        $("#headerFeeInformationSpecialCareful").show();
        $("#headerFeeInfo").hide();
    }
    else {
        $("#headerFeeInformationSpecialCareful").hide();
        $("#headerFeeInfo").show();
    }

    if (CMS420.PaymentMethod == "1") {
        $("#btnAutoTransferInformation").show();
        $("#btnCreditCardInformation").hide();
    }
    else if (CMS420.PaymentMethod == "2") {
        $("#btnAutoTransferInformation").hide();
        $("#btnCreditCardInformation").show();
    }
    else {
        $("#btnAutoTransferInformation").hide();
        $("#btnCreditCardInformation").hide();
    }

    if (CMS420.txtAdjustBillingPeriodStartDate == null && CMS420.txtAdjustBillingPeriodEndDate == null) {
        $("#IsPeriodDate").hide();
        $("#IsNoPeriodDate").show();
    }
    else {
        $("#IsPeriodDate").show();
        $("#IsNoPeriodDate").hide();
    }

    initialHeaderButton();
});

function initialHeaderButton() {
    var myurl = "";

    $("#btnContractBasic").click(function () {
        // go to CMS120
        var obj = { "strContractCode": CMS420.ContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS120", obj, false);
    });

    $("#btnHistoryDigest").click(function () {
        // go to CMS150
        var obj = { "ContractCode": CMS420.ContractCode, "ServiceTypeCode": CMS420.ServiceTypeCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS150", obj, false);
    });

    $("#btnHeader_Installation").click(function () {
        // go to CMS180
        var obj = { "ContractCode": CMS420.ContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS180", obj, false);
    });

    $("#btnSalesContractBasic").click(function () {
        // go to CMS160
        var obj = { "strContractCode": CMS420.ContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS160", obj, false);
    });

    $("#btnContractBillingTransfer").click(function () {
        // go to CMS200
        var obj = { "strContractCode": CMS420.ContractCode, "strServiceTypeCode": CMS420.ServiceTypeCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS200", obj, false);
    });

    $("#btnContractBillingTransfer").click(function () {
        // go to CMS200
        var obj = { "strContractCode": CMS420.ContractCode, "strServiceTypeCode": CMS420.ServiceTypeCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS200", obj, false);
    });

    $("#btnHeader_BillingBasic").click(function () {
        // go to CMS420
        var obj = { "ContractCode": CMS420.ContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS420", obj, false);
    });
}

function IntialGridBillingOCC() {

    mygridBillingOCC = $("#gridBillingOCC").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, true, true, "/Common/CMS420_LoadGridBillingOCC", "", "dtViewBillingOccList", false);

    SpecialGridControl(mygridBillingOCC, ["Detail"]);
    BindOnLoadedEvent(mygridBillingOCC
        , function (gen_ctrl) {
            if (mygridBillingOCC.getRowsNum() != 0) {
                for (var i = 0; i < mygridBillingOCC.getRowsNum(); i++) {
                    var row_id = mygridBillingOCC.getRowId(i);

                    if (gen_ctrl == true) {
                        GenerateDetailButton(mygridBillingOCC, "btnGridDetail", row_id, "Detail", true);
                    }

                    BindGridButtonClickEvent("btnGridDetail", row_id
                        , function (rid) {
                            btnGridDetailClick(rid);
                        }
                    );
                }
            }
        }
    );
}

function IntialGridBillingTypeDetail() {

    mygridBillingTypeDetail = $("#gridBillingTypeDetail").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, false, false, "/Common/CMS420_LoadGridBillingTypeDetailWithInitial", "", "doBillingTypeDetailList", false);

}

function InitialTextLink() {

    if ($("#BillingTargetCode").text().length > 0) {

        $("#BillingTargetCode").show();

        $("#BillingTargetCode").click(function () {
            var obj = { "BillingTargetCode": $("#BillingTargetCode").text() };
            ajax_method.CallScreenControllerWithAuthority("/Common/CMS410", obj, true);
        });

    }
    else {
        $("#BillingTargetCode").parent().html("<div id='BillingTargetCode' class='usr-label label-view' style='width:200px;'>-</div>");
    }

    if ($("#PreviousBillingTargetCode").text().length > 0) {

        $("#PreviousBillingTargetCode").show();

        $("#PreviousBillingTargetCode").click(function () {
            var obj = { "BillingTargetCode": $("#PreviousBillingTargetCode").text() };
            ajax_method.CallScreenControllerWithAuthority("/Common/CMS410", obj, true);
        });

    }
    else {
        $("#PreviousBillingTargetCode").parent().html("<div id='PreviousBillingTargetCode' class='usr-label label-view' style='width:280px; text-align:left;'>-</div>");
    }

}


function IntialButtonLink() {

    $("#btnBillingDetail").click(function () {
        var obj = { "ContractCode": CMS420.ContractCode, "BillingOCC": BillingOCC };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS450", obj, true);
    });

    $("#btnEditInformation").click(function () {
        var obj = { "ContractCode": CMS420.ContractCode, "BillingOCC": BillingOCC };
        ajax_method.CallScreenControllerWithAuthority("/Billing/BLS040", obj, true);
    });

    $("#btnDepositInformation").click(function () {
        var obj = { "ContractCode": CMS420.ContractCode,
            "BillingOCC": strBillingOCC
         };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS430", obj, true);
    });

    $("#btnAutoTransferInformation").click(function () {
        $("#dlgBox").OpenCMS421Dialog("CMS420");
    });

    $("#btnCreditCardInformation").click(function () {
        $("#dlgBox").OpenCMS422Dialog("CMS420");
    });
}

function btnGridDetailClick(rid) {
    mygridBillingOCC.selectRow(mygridBillingOCC.getRowIndex(rid));
    BillingOCC = mygridBillingOCC.cells(rid, mygridBillingOCC.getColIndexById('BillingOCC')).getValue();

    var param = { BillingOCC: BillingOCC };
    strBillingOCC = BillingOCC;
    if ($("#gridBillingTypeDetail").length > 0) {
        $("#gridBillingTypeDetail").LoadDataToGrid(mygridBillingTypeDetail, ROWS_PER_PAGE_FOR_VIEWPAGE
            , false
            , "/Common/CMS420_LoadGridBillingTypeDetail"
            , param
            , "doBillingTypeDetailList"
            , false
            , function () { }
            , function () { $("#BillingBasicInformation").show(); }
        );
    }

    call_ajax_method_json('/Common/CMS420_LoadBillingBasicInformation'
        , param
        , function (result, controls) {
            if (result != undefined) {

                if (result.length == 4) {
                    $("#BillingCode").text(result[0].BillingCode == null ? "" : result[0].BillingCode);
                    $("#BillingOffice").text(result[0].BillingOffice == null ? "" : result[0].BillingOffice);
                    $("#DebtTracingOffice").text(result[0].DebtTracingOffice == null ? "" : result[0].DebtTracingOffice);
                    $("#CustomerType").text(result[0].CustomerType == null ? "" : result[0].CustomerType);
                    $("#BillingClientNameEN").text(result[0].BillingClientNameEN == null ? "" : result[0].BillingClientNameEN);
                    $("#BillingClientBranchNameEN").text(result[0].BillingClientBranchNameEN == null ? "" : result[0].BillingClientBranchNameEN);
                    $("#BillingClientAddressEN").text(result[0].BillingClientAddressEN == null ? "" : result[0].BillingClientAddressEN);
                    $("#BillingClientNameLC").text(result[0].BillingClientNameLC == null ? "" : result[0].BillingClientNameLC);
                    $("#BillingClientBranchNameLC").text(result[0].BillingClientBranchNameLC == null ? "" : result[0].BillingClientBranchNameLC);
                    $("#BillingClientAddressLC").text(result[0].BillingClientAddressLC == null ? "" : result[0].BillingClientAddressLC);
                    $("#MonthlyBillingAmount").text(result[0].MonthlyBillingAmount == null ? "" : result[0].MonthlyBillingAmount);
                    $("#PaymentMethod").text(result[0].PaymentMethod == null ? "" : result[0].PaymentMethod);
                    $("#BillingCycle").text(result[0].BillingCycle == null ? "" : result[0].BillingCycle);
                    $("#CreditTerm").text(result[0].CreditTerm == null ? "" : result[0].CreditTerm);
                    $("#CalculationDailyFee").text(result[0].CalculationDailyFee == null ? "" : result[0].CalculationDailyFee);
                    $("#LastBillingDate").text(result[0].LastBillingDate == null ? "" : result[0].LastBillingDate);
                    $("#ManagementCodeForSortDetails").text(result[0].ManagementCodeForSortDetails == null ? "" : result[0].ManagementCodeForSortDetails);
                    $("#AdjustEndingDateOfBillingPeriod").text(result[0].AdjustEndingDateOfBillingPeriod == null ? "" : result[0].AdjustEndingDateOfBillingPeriod);
                    $("#BillingFlag").text(result[0].BillingFlag == null ? "" : result[0].BillingFlag);
                    $("#VATUnchargedBillingTarget").attr("checked", result[0].VATUnchargedBillingTarget);
                    $("#BalanceOfDepositFee").text(result[0].BalanceOfDepositFee == null ? "" : result[0].BalanceOfDepositFee);
                    $("#MonthlyFeeBeforeStop").text(result[0].MonthlyFeeBeforeStop == null ? "" : result[0].MonthlyFeeBeforeStop);
                    $("#ResultBasedMaintenanceBillingFlag").attr("checked", result[0].ResultBasedMaintenanceBillingFlag);
                    $("#LastPaymentConditionChangingDate").text(result[0].LastPaymentConditionChangingDate == null ? "" : result[0].LastPaymentConditionChangingDate);
                    $("#RegisteringDateOfLastChanging").text(result[0].RegisteringDateOfLastChanging == null ? "" : result[0].RegisteringDateOfLastChanging);
                    $("#ApproveNo").text(result[0].ApproveNo == null ? "" : result[0].ApproveNo);
                    $("#DocumentReceiving").text(result[0].DocumentReceiving == null ? "" : result[0].DocumentReceiving);
                    $("#LastMonthlyBillingAmount").text(result[1].LastMonthlyBillingAmount == null ? "" : result[1].LastMonthlyBillingAmount);
                    $("#LastDate").text(result[1].LastDate == null ? "" : result[1].LastDate);
                    $("#BillingAmountBeforeChanging1").text(result[1].BillingAmountBeforeChanging1 == null ? "" : result[1].BillingAmountBeforeChanging1);
                    $("#DateBeforeChanging1").text(result[1].DateBeforeChanging1 == null ? "" : result[1].DateBeforeChanging1);
                    $("#BillingAmountBeforeChanging2").text(result[1].BillingAmountBeforeChanging2 == null ? "" : result[1].BillingAmountBeforeChanging2);
                    $("#DateBeforeChanging2").text(result[1].DateBeforeChanging2 == null ? "" : result[1].DateBeforeChanging2);
                    $("#BillingAmountBeforeChanging3").text(result[1].BillingAmountBeforeChanging3 == null ? "" : result[1].BillingAmountBeforeChanging3);
                    $("#DateBeforeChanging3").text(result[1].DateBeforeChanging3 == null ? "" : result[1].DateBeforeChanging3);
                    $("#BillingAmountBeforeChanging4").text(result[1].BillingAmountBeforeChanging4 == null ? "" : result[1].BillingAmountBeforeChanging4);
                    $("#DateBeforeChanging4").text(result[1].DateBeforeChanging4 == null ? "" : result[1].DateBeforeChanging4);
                    $("#BillingAmountBeforeChanging5").text(result[1].BillingAmountBeforeChanging5 == null ? "" : result[1].BillingAmountBeforeChanging5);
                    $("#DateBeforeChanging5").text(result[1].DateBeforeChanging5 == null ? "" : result[1].DateBeforeChanging5);
                    $("#AdjustBillingAmount").text(result[0].AdjustBillingAmount == null ? "" : result[0].AdjustBillingAmount);
                    $("#txtAdjustBillingPeriodStartDate").text(result[0].AdjustBillingPeriodStartDate == null ? "" : result[0].AdjustBillingPeriodStartDate);
                    $("#txtAdjustBillingPeriodEndDate").text(result[0].AdjustBillingPeriodEndDate == null ? "" : result[0].AdjustBillingPeriodEndDate);

                    $("#IDNo").text(result[0].IDNo == null ? "" : result[0].IDNo); //Add by Jutarat A. on 12122013
                    
                    if (result[0].AdjustBillingPeriodStartDate == null && result[0].AdjustBillingPeriodEndDate == null) {
                        $("#IsPeriodDate").hide();
                        $("#IsNoPeriodDate").show();
                    }
                    else {
                        $("#IsPeriodDate").show();
                        $("#IsNoPeriodDate").hide();
                    }

                    // ---- initial link ------

                    // # 1.
                    if (result[0].BillingTargetCode != undefined && result[0].BillingTargetCode != "" && result[0].BillingTargetCode != null) {

                        $("#BillingTargetCode").show();

                        $("#BillingTargetCode").unbind("click", BillingTargetCode_click);

                        $("#BillingTargetCode").parent().html("<a id='BillingTargetCode' name='BillingTargetCode' href='#' style='width:200px;'>" + result[0].BillingTargetCode + "</a>");

                        $("#BillingTargetCode").click(BillingTargetCode_click);

                    }
                    else {
                        $("#BillingTargetCode").parent().html("<div id='BillingTargetCode' class='usr-label label-view' style='width:200px;'>-</div>");
                    }


                    // #2
                    if (result[0].PreviousBillingTargetCode != undefined && result[0].PreviousBillingTargetCode != "" && result[0].PreviousBillingTargetCode != null) {

                        $("#PreviousBillingTargetCode").show();

                        $("#PreviousBillingTargetCode").unbind("click", PreviousBillingTargetCode_click);

                        $("#PreviousBillingTargetCode").parent().html("<a id='PreviousBillingTargetCode' name='PreviousBillingTargetCode' href='#' style='width:200px;'>" + result[0].PreviousBillingTargetCode + "</a>");

                        $("#PreviousBillingTargetCode").click(PreviousBillingTargetCode_click);

                    }
                    else {
                        $("#PreviousBillingTargetCode").parent().html("<div id='PreviousBillingTargetCode' class='usr-label label-view' style='width:280px; text-align:left;'>-</div>");
                    }


                    //$("#BillingTargetCode").text(result[0].BillingTargetCode); // -- link
                    //$("#PreviousBillingTargetCode").text(result[0].PreviousBillingTargetCode); // -- link



                    //$("#BillingBasicInformation").show();

                    if (result[2] != undefined) {
                        if (result[2] == true) {
                            $("#headerFeeInformationSpecialCareful").show();
                            $("#headerFeeInfo").hide();
                        }
                        else {
                            $("#headerFeeInformationSpecialCareful").hide();
                            $("#headerFeeInfo").show();
                        }
                    }

                    if (result[3] != undefined) {
                        if (result[3] == "1") {
                            $("#btnAutoTransferInformation").show();
                            $("#btnCreditCardInformation").hide();
                        }
                        else if (result[3] == "2") {
                            $("#btnAutoTransferInformation").hide();
                            $("#btnCreditCardInformation").show();
                        }
                        else {
                            $("#btnAutoTransferInformation").hide();
                            $("#btnCreditCardInformation").hide();
                        }
                    }

                }
                $("#divAll").SetEmptyViewData();
            }
        }
    );
}

function BillingTargetCode_click() {
    var obj = { "BillingTargetCode": $("#BillingTargetCode").text() };
    ajax_method.CallScreenControllerWithAuthority("/Common/CMS410", obj, true);
}

function PreviousBillingTargetCode_click() {
    var obj = { "BillingTargetCode": $("#PreviousBillingTargetCode").text() };
    ajax_method.CallScreenControllerWithAuthority("/Common/CMS410", obj, true);
}


function CMS421Object() {
    var obj = {
        ContractCode: CMS420.ContractCode,
        BillingOCC: BillingOCC
    };

    return obj;
}

function CMS421Response(result) {

}

function CMS422Object() {
    var obj = {
        ContractCode: CMS420.ContractCode,
        BillingOCC: BillingOCC
    };

    return obj;
}

function CMS422Response(result) {

}