/*--- Main ---*/

var pageRow = 20;
var mode;
var eventCopyNameComeFrom;
var lastestDat = null;
var loadedBillingGrid = false;
var loadedChangePlanGrid = false;
var mygridCTS062 = null;
var mygridBillingCTS062 = null;
var validateRegisForm = ["ExpectedInstallCompleteDate"
, "ExpectedCustAcceptanceDate"
, "OrderProductPrice"
, "OrderInstallFee"
, "OrderSalePrice"
, "NegotiationStaffEmpNo1"
, "PaymethodCompleteInstallation"
, "ApproveNo1"
, "SalePrice_PaymentMethod_Acceptance"
, "InstallationFee_PaymentMethod_Acceptance"];

$(document).ready(function () {
    InitialControlProperty();
    MaintainScreenItemOnInit();
    DisableAllCommand();
    //$("#ImportantFlag").attr("disabled", true);
    $('#Alphabet').blur(function () {
        $('#Alphabet').val($('#Alphabet').val().toUpperCase());
    });

    $('#btnTest').click(btnTest_Click);
});

function btnTest_Click() {

}

function LessClick() {
    $('#SalemanSection2').hide();
    $('#SalemanSection3').hide();
    $("#More").show();
    $("#Less").hide();
    //        $("#SalemanSection3").hide();
    //        $("#Saleman3Section").hide();
    //        $("#SalesmanEmpNo3").hide();
    //        $("#SalesmanEmpName3").hide()
}

function MoreClick() {
    $('#SalemanSection2').show();
    $('#SalemanSection3').show();
    $("#More").hide();
    $("#Less").show();
    //        $("#SalemanSection2").show();
    //        $("#Saleman3Section").show();
    //        $("#SalemanSection3").show();
    //        $("#SalesmanEmpNo3").show(); 
    //        $("#SalesmanEmpName3").show() 
}

function InitialControlProperty() {
    $("#NormalProductPrice").BindNumericBox(14, 2, 0, 99999999999.99);
    $("#OrderProductPrice").BindNumericBox(14, 2, 0, 999999999999.99);
    $("#BillingAmt_ApproveContract").BindNumericBox(14, 2, 0, 999999999999.99);
    $("#BillingAmt_PartialFee").BindNumericBox(14, 2, 0, 999999999999.99);
    $("#BillingAmt_Acceptance").BindNumericBox(14, 2, 0, 999999999999.99);


    $("#NormalInstallFee").BindNumericBox(14, 2, 0, 999999999999.99);
    $("#OrderInstallFee").BindNumericBox(14, 2, 0, 999999999999.99);
    $("#BillingAmtInstallation_ApproveContract").BindNumericBox(14, 2, 0, 999999999999.99);
    $("#BillingAmtInstallation_PartialFee").BindNumericBox(14, 2, 0, 999999999999.99);
    $("#BillingAmtInstallation_Acceptance").BindNumericBox(14, 2, 0, 999999999999.99);

    //$("#NormalSalePrice").BindNumericBox(14, 2, 0, 999999999999.99);
    //$("#OrderSalePrice").BindNumericBox(14, 2, 0, 999999999999.99);
    //$("#PartialFee").BindNumericBox(14, 2, 0, 999999999999.99);
    
    $("#SalePrice_Approval").BindNumericBox(14, 2, 0, 999999999999.99);
    $("#SalePrice_Partial").BindNumericBox(14, 2, 0, 999999999999.99);
    $("#SalePrice_Acceptance").BindNumericBox(14, 2, 0, 999999999999.99);

    $("#InstallationFee_Approval").BindNumericBox(14, 2, 0, 999999999999.99);
    $("#InstallationFee_Partial").BindNumericBox(14, 2, 0, 999999999999.99);
    $("#InstallationFee_Acceptance").BindNumericBox(14, 2, 0, 999999999999.99);

    //$("#BillingTotal").BindNumericBox(14, 2, 0, 999999999999.99);

    // 20170208 nakajima add start
    var bfunc = function () {
        var id = $(this).attr("id");
        var val = $(this).val();
        var cval = $(this).NumericCurrencyValue();

        setTimeout(function () {
            if (id == "BillingAmt_ApproveContract") {
                $("#SalePrice_Approval").val(val);
                $("#SalePrice_Approval").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmt_PartialFee") {
                $("#SalePrice_Partial").val(val);
                $("#SalePrice_Partial").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmt_Acceptance") {
                $("#SalePrice_Acceptance").val(val);
                $("#SalePrice_Acceptance").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmtInstallation_ApproveContract") {
                $("#InstallationFee_Approval").val(val);
                $("#InstallationFee_Approval").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmtInstallation_PartialFee") {
                $("#InstallationFee_Partial").val(val);
                $("#InstallationFee_Partial").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmtInstallation_Acceptance") {
                $("#InstallationFee_Acceptance").val(val);
                $("#InstallationFee_Acceptance").SetNumericCurrency(cval);
            }

            if (typeof (SummaryBillingFee) == "function") {
                SummaryBillingFee();
            }
        }, 0);

    };

    $("#BillingAmt_ApproveContract").blur(bfunc);
    $("#BillingAmt_PartialFee").blur(bfunc);
    $("#BillingAmt_Acceptance").blur(bfunc);
    $("#BillingAmtInstallation_ApproveContract").blur(bfunc);
    $("#BillingAmtInstallation_PartialFee").blur(bfunc);
    $("#BillingAmtInstallation_Acceptance").blur(bfunc);

    var bcfunc = function () {
        var id = $(this).attr("id");
        var val = $("#" + id.replace("CurrencyType", "")).val();
        var cval = $(this).val();

        setTimeout(function () {
            if (id == "BillingAmt_ApproveContractCurrencyType") {
                $("#SalePrice_Approval").val(val);
                $("#SalePrice_Approval").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmt_PartialFeeCurrencyType") {
                $("#SalePrice_Partial").val(val);
                $("#SalePrice_Partial").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmt_AcceptanceCurrencyType") {
                $("#SalePrice_Acceptance").val(val);
                $("#SalePrice_Acceptance").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmtInstallation_ApproveContractCurrencyType") {
                $("#InstallationFee_Approval").val(val);
                $("#InstallationFee_Approval").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmtInstallation_PartialFeeCurrencyType") {
                $("#InstallationFee_Partial").val(val);
                $("#InstallationFee_Partial").SetNumericCurrency(cval);
            }
            else if (id == "BillingAmtInstallation_AcceptanceCurrencyType") {
                $("#InstallationFee_Acceptance").val(val);
                $("#InstallationFee_Acceptance").SetNumericCurrency(cval);
            }

            if (typeof (SummaryBillingFee) == "function") {
                SummaryBillingFee();
            }
        }, 0);
    };

    $("#BillingAmt_ApproveContract").NumericCurrency().blur(bcfunc);
    $("#BillingAmt_PartialFee").NumericCurrency().blur(bcfunc);
    $("#BillingAmt_Acceptance").NumericCurrency().blur(bcfunc);
    $("#BillingAmtInstallation_ApproveContract").NumericCurrency().blur(bcfunc);
    $("#BillingAmtInstallation_PartialFee").NumericCurrency().blur(bcfunc);
    $("#BillingAmtInstallation_Acceptance").NumericCurrency().blur(bcfunc);

    // 20170208 nakajima add end

    $("#btnViewQuotationDetail").attr("disabled", true);

    $("#ProductName").attr("readonly", true);
    $("#BillingOffice").attr("readonly", true);
    $("#SalesType").attr("readonly", true);
    $("#BillingOffice").attr("disabled", true);
    $("#DistributedOriginCode").attr("readonly", true);

    $("#ExpectedInstallCompleteDate").InitialDate();
    $("#ExpectedCustAcceptanceDate").InitialDate();
    $("#btnRetrieve").click(function () { BtnRetrieveClick(); });

    $("#btnNewEdit").attr("disabled", true);
    $("#btnSpecifyClear").attr("disabled", true);
    $("#btnClearBillingTarget").attr("disabled", true);
    $("#btnSpecifyClear").click(function () { SpecifyClearClick(); });

    $("#ChangePlanSection").hide();
    $("#BillingTargetDetailSection").hide();

    $('#SalemanSection2').hide();
    $('#SalemanSection3').hide();
    $("#More").show();

//    $("#SalesmanEmpNo3").hide();
//    $("#SalemanSection3").hide();
//    $("#Saleman3Section").hide();    
//    $("#SalesmanEmpName3").hide();
    $("#Less").click(LessClick);
    $("#More").click(MoreClick);

    GetEmployeeNameData("#NegotiationStaffEmpNo1", "#NegotiationStaffEmpName1");
    GetEmployeeNameData("#NegotiationStaffEmpNo2", "#NegotiationStaffEmpName2");

    InitialTrimTextEvent(["ApproveNo1", "ApproveNo2", "ApproveNo3", "ApproveNo4", "ApproveNo5"]);

//    $('#NegotiationStaffEmpNo1').blur(
//    function () {
//        ObjCTS062.EmpNo = $("#NegotiationStaffEmpNo1").val();
//        if ($("#NegotiationStaffEmpNo1").val().length == 0) {
//            $("#NegotiationStaffEmpName1").val("");
//        } else {
//            call_ajax_method_json('/Contract/GetActiveEmployee_CTS062', ObjCTS062,
//            function (result, controls) {
//                if (result != undefined) {
//                    if (result.Message != undefined && $("#NegotiationStaffEmpNo1").val() != "") {
//                        OpenErrorMessageDialog(result.Code, result.Message, function () { }, null);
//                        $("#NegotiationStaffEmpNo1").val("");
//                        $("#NegotiationStaffEmpName1").val("");
//                    }
//                    else {
//                        $("#NegotiationStaffEmpName1").val(result.EmpName);
//                    }
//                }
//            });
//        }
//    });

//    $('#NegotiationStaffEmpNo2').blur(
//    function () {
//        ObjCTS062.EmpNo = $("#NegotiationStaffEmpNo2").val();
//        if ($("#NegotiationStaffEmpNo2").val().length == 0) {
//            $("#NegotiationStaffEmpName2").val("");
//        } else {
//            call_ajax_method_json('/Contract/GetActiveEmployee_CTS062', ObjCTS062,
//            function (result, controls) {
//                if (result.Message != undefined) {
//                    if (result != undefined) {
//                        if (result.Message != undefined && $("#NegotiationStaffEmpNo2").val() != "") {
//                            OpenErrorMessageDialog(result.Code, result.Message, function () { }, null);
//                            $("#NegotiationStaffEmpNo2").val("");
//                            $("#NegotiationStaffEmpName2").val("");
//                        }
//                        else {
//                            $("#NegotiationStaffEmpName2").val(result.EmpName);
//                        }
//                    }
//                }
//                else {
//                    $("#NegotiationStaffEmpName2").val(result.EmpName);
//                }
//            });
//        }
//    });

    $("#btnSearchQuotation").click(function () {
        $("#dlgCTS062").OpenQUS010Dialog('CTS062');
        //DisableRegisterCommand(true);
    });

    $("#ConnectionFlag").click(function () {
        if ($("#ConnectionFlag").attr('checked')) {
            $("#ConnectTargetCode").attr("readonly", false);
            if (lastestDat != null) {
                $("#ConnectTargetCode").val(lastestDat.ConnectTargetCode);
            }
        }
        else {
            $("#ConnectTargetCode").attr("readonly", true);
            $("#ConnectTargetCode").val("");
        }
    })

    $("#rdoNotDistributed").click(function () {
        if ($("#rdoNotDistributed").prop("checked") == true) {
            $("#DistributedOriginCode").attr("readonly", true);
            $("#DistributedOriginCode").val("");
        }
        else {
            $("#DistributedOriginCode").attr("readonly", false);
        }
    })

    $("#rdoDistributedOrigin").click(function () {
        if ($("#rdoDistributedOrigin").prop("checked") == true) {
            $("#DistributedOriginCode").attr("readonly", true);
            $("#DistributedOriginCode").val("");
        }
        else {
            $("#DistributedOriginCode").attr("readonly", false);
        }
    })

    $("#rdoDistributedTarget").click(function () {
        if ($("#rdoDistributedTarget").prop("checked") == true) {
            $("#DistributedOriginCode").attr("readonly", false);
            if (lastestDat != null) {
                $("#DistributedOriginCode").val(lastestDat.DistributedCode);
            }
        }
        else {
            $("#DistributedOriginCode").attr("readonly", true);
        }
    })  
}

function MaintainScreenItemOnInit() {
    EnableRegisterCommand();

    $('#btnViewQuotationDetail').attr('disabled', true);
}

//Event------------------------------------------------

function BtnRetrieveClick() {

    var obj = { QuotationTargetCode: $("#QuotationTargetCode").val(), Alphabet: $("#Alphabet").val() };
    call_ajax_method_json('/Contract/ValidateRetrieve_CTS062', obj,
    function (result, controls) {
        if (result == undefined) {
            call_ajax_method_json('/Contract/RetrieveClick_CTS062', obj,
                function (result, controls) {
                    if ((result != null) && (result.MessageCode != null)) {
                        if (result.MessageCode == 'MSG3038') {
                            doWarningAlert("Contract", result.MessageCode, null);
                        } else {
                            doAlert("Contract", result.MessageCode, null);
                        }
                    }
                    //if (result == undefined) {
                    if ((result != null)
                        && (result.CanRetrieve)) {
                        call_ajax_method_json('/Contract/GetChangePlanANDSpecifyDetail_CTS062', "",
                        function (result, controls) {
                            if (result != null) {
                                lastestDat = result;
                                $('#SaleContractChangeSection').SetViewMode(false);
                                $('#ImportantFlag').attr('disabled', 'disabled');

                                $('#SpecifyQuotationSection').SetViewMode(false);
                                $('#ChangePlanSection').SetViewMode(false);
                                $('#BillingTargetDetailSection').SetViewMode(false);

                                $("#Alphabet").val(result.Alphabet);
                                $("#ProductName").val(result.ProductName);

                                if (result.ExpectedInstallCompleteDate != null && result.ExpectedInstallCompleteDate != "") {
                                    result.ExpectedInstallCompleteDate = ConvertDateObject(result.ExpectedInstallCompleteDate);
                                    $("#ExpectedInstallCompleteDate").val(ConvertDateToTextFormat(result.ExpectedInstallCompleteDate));
                                }

                                if (result.ExpectedCustAcceptanceDate != null && result.ExpectedCustAcceptanceDate != "") {
                                    result.ExpectedCustAcceptanceDate = ConvertDateObject(result.ExpectedCustAcceptanceDate);
                                    $("#ExpectedCustAcceptanceDate").val(ConvertDateToTextFormat(result.ExpectedCustAcceptanceDate));
                                }

                                //DistributeType---------------------------------------------

                                if (result.DistributedType == "0") {
                                    $("#rdoNotDistributed").attr('checked', true);
                                }

                                if (result.DistributedType == "1") {
                                    $("#rdoDistributedOrigin").attr('checked', true);
                                }

                                if (result.DistributedType == "2") {
                                    $("#rdoDistributedTarget").attr('checked', true);
                                    $("#DistributedOriginCode").val(result.DistributedCode);
                                }
                                //-----------------------------------------------------------

                                $('#ConnectionFlag').attr('checked', result.ConnectionFlag);
                                if (result.ConnectionFlag) {
                                    $("#ConnectTargetCode").attr("readonly", false);
                                    $("#ConnectTargetCode").val(result.ConnectTargetCode);
                                }
                                else {
                                    $("#ConnectTargetCode").attr("readonly", true);
                                }

                                $("#SalesmanEmpNo1").val(result.SalesmanEmpNo1);
                                $("#SalesmanEmpNo2").val(result.SalesmanEmpNo2);
                                $("#SalesmanEmpNo3").val(result.SalesmanEmpNo3);
                                $("#SalesmanEmpNo4").val(result.SalesmanEmpNo4);
                                $("#SalesmanEmpNo5").val(result.SalesmanEmpNo5);
                                $("#SalesmanEmpNo6").val(result.SalesmanEmpNo6);
                                $("#SalesmanEmpNo7").val(result.SalesmanEmpNo7);
                                $("#SalesmanEmpNo8").val(result.SalesmanEmpNo8);
                                $("#SalesmanEmpNo9").val(result.SalesmanEmpNo9);
                                $("#SalesmanEmpNo10").val(result.SalesmanEmpNo10);

                                $("#SalesmanEmpName1").val(result.SalesmanEmpName1);
                                $("#SalesmanEmpName2").val(result.SalesmanEmpName2);
                                $("#SalesmanEmpName3").val(result.SalesmanEmpName3);
                                $("#SalesmanEmpName4").val(result.SalesmanEmpName4);
                                $("#SalesmanEmpName5").val(result.SalesmanEmpName5);
                                $("#SalesmanEmpName6").val(result.SalesmanEmpName6);
                                $("#SalesmanEmpName7").val(result.SalesmanEmpName7);
                                $("#SalesmanEmpName8").val(result.SalesmanEmpName8);
                                $("#SalesmanEmpName9").val(result.SalesmanEmpName9);
                                $("#SalesmanEmpName10").val(result.SalesmanEmpName10);

                                $("#NegotiationStaffEmpNo1").val(result.NegotiationStaffEmpNo1);
                                $("#NegotiationStaffEmpNo2").val(result.NegotiationStaffEmpNo2);

                                $("#NegotiationStaffEmpName1").val(result.NegotiationStaffEmpName1);
                                $("#NegotiationStaffEmpName2").val(result.NegotiationStaffEmpName2);

                                $("#ApproveNo1").val(result.ApproveNo1);
                                $("#ApproveNo2").val(result.ApproveNo2);
                                $("#ApproveNo3").val(result.ApproveNo3);
                                $("#ApproveNo4").val(result.ApproveNo4);
                                $("#ApproveNo5").val(result.ApproveNo5);

                                $("#ChangePlanSection").show();
                                $("#BillingTargetDetailSection").show();

                                $("#btnViewQuotationDetail").attr("disabled", false);
                                $("#btnSpecifyClear").attr("disabled", false);

                                $("#btnViewQuotationDetail").unbind("click");
                                $("#btnViewQuotationDetail").click(function () {
                                    var parameter = {
                                        QuotationTargetCode: $("#QuotationTargetCode").val(),
                                        Alphabet: $("#Alphabet").val(),
                                        HideQuotationTarget: true
                                    };

                                    $("#dlgCTS062").OpenQUS011Dialog();
                                });

//                                $("#btnViewQuotationDetail").initial_link(function (val) {
//                                    //                                    var parameter = {
//                                    //                                        QuotationTargetCode: $("#QuotationTargetCode").val(),
//                                    //                                        Alphabet: $("#Alphabet").val(),
//                                    //                                        HideQuotationTarget: true
//                                    //                                    };

//                                    //                                    $("#dlgCTS062").OpenQUS011Dialog(parameter);
//                                    $("#dlgCTS062").OpenQUS011Dialog();
//                                });

                                GetChangePlanGrid();
                                GetBillingTargetInformationDetailGrid();
                                GetBillingTargetInformationDetail();
                                EnableRegisterCommand();
                                ISDisableDistributedOriginCode();
                                ISDisableSpecifyQuotation(true);

                                $('#SalemanSection2').hide();
                                $('#SalemanSection3').hide();
                                $("#More").show();
                                $("#Less").hide();
                            }
                        });

                    }
                });
        }
    });
}

function doWarningAlert(moduleCode, msgCode, paramObj) {
    var obj = {
        module: moduleCode,
        code: msgCode,
        param: paramObj
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenWarningMessageDialog(result.Message, result.Message, null);
    });
}

function doAlert(moduleCode, msgCode, paramObj) {
    var obj = {
        module: moduleCode,
        code: msgCode,
        param: paramObj
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenWarningDialog(result.Message, result.Message, null);
    });
}

function GetChangePlanGrid() {
    BindingGridCalculate();
    call_ajax_method_json("/Contract/GetChangePlanGrid_CTS062", "", function (result) {
        if (result != null) {
            //$('#NormalProductPrice').val(result[0].NormalPrice);
            ////$('#OrderProductPrice').val(result[0].OrderPrice);
            //$('#OrderProductPrice').val('');

            //$('#NormalInstallFee').val(result[1].NormalPrice);
            ////$('#OrderInstallFee').val(result[1].OrderPrice);
            //$('#OrderInstallFee').val('');

            //$('#NormalSalePrice').val(result[2].NormalPrice);
            ////$('#OrderSalePrice').val(result[2].OrderPrice);
            //$('#OrderSalePrice').val('');

            //$('#BillingAmt_ApproveContract').val(result[2].BillingAmt_ApproveContract);
            //$('#PartialFee').val(result[2].PartialFee);
            //$('#BillingAmt_Acceptance').val(result[2].BillingAmt_Acceptance);

            $('#NormalProductPrice').SetNumericCurrency(result[0].NormalPriceCurrencyType);
            $('#NormalProductPrice').val(result[0].NormalPrice);
            $('#OrderProductPrice').SetNumericCurrency(result[0].OrderPriceCurrencyType);
            $('#OrderProductPrice').val(result[0].OrderPrice);
            $('#BillingAmt_ApproveContract').SetNumericCurrency(result[0].BillingAmt_ApproveContractCurrencyType);
            $('#BillingAmt_ApproveContract').val(result[0].BillingAmt_ApproveContract);
            $('#BillingAmt_PartialFee').SetNumericCurrency(result[0].BillingAmt_PartialFeeCurrencyType);
            $('#BillingAmt_PartialFee').val(result[0].BillingAmt_PartialFee);
            $('#BillingAmt_Acceptance').SetNumericCurrency(result[0].BillingAmt_AcceptanceCurrencyType);
            $('#BillingAmt_Acceptance').val(result[0].BillingAmt_Acceptance);

            $('#NormalInstallFee').SetNumericCurrency(result[1].NormalPriceCurrencyType);
            $('#NormalInstallFee').val(result[1].NormalPrice);
            $('#OrderInstallFee').SetNumericCurrency(result[1].OrderPriceCurrencyType);
            $('#OrderInstallFee').val(result[1].OrderPrice);
            $('#BillingAmtInstallation_ApproveContract').SetNumericCurrency(result[1].BillingAmt_ApproveContractCurrencyType);
            $('#BillingAmtInstallation_ApproveContract').val(result[1].BillingAmt_ApproveContract);
            $('#BillingAmtInstallation_PartialFee').SetNumericCurrency(result[1].BillingAmt_PartialFeeCurrencyType);
            $('#BillingAmtInstallation_PartialFee').val(result[1].BillingAmt_PartialFee);
            $('#BillingAmtInstallation_Acceptance').SetNumericCurrency(result[1].BillingAmt_AcceptanceCurrencyType);
            $('#BillingAmtInstallation_Acceptance').val(result[1].BillingAmt_Acceptance);

            loadedChangePlanGrid = true;
            MappingToBillingGrid();
        }
    });
}

//function GetChangePlanGrid() {
//    mygridCTS062 = $("#gridChangePlanBillingTarget").LoadDataToGridWithInitial(0, false, false, "/Contract/GetChangePlanGrid_CTS062",
//    "", "CTS062_DOChangePlan", false);

//    BindOnLoadedEvent(mygridCTS062, function () {

//        //Column1----------------------------------------------------------------------------

//        var val = mygridCTS062.cells2(0, 1).getValue();
//        //        mygridCTS062.cells2(0, 1).setValue(GenerateNumericBox("NormalProductPrice", "", val, true) + " " + C_CURRENCY_UNIT + " ");
//        //        $("#NormalProductPrice").BindNumericBox(14, 2, 0, 9999999999);
//        //        $("#NormalProductPrice").css('width', '108px');
//        //        $("#NormalProductPrice").attr("readonly", true);
//        var rowID = mygridCTS062.getRowId(0);
//        GenerateNumericBox2(mygridCTS062, "NormalProductPrice", rowID, "NormalPrice", val, 10, 2, 0, 9999999999, 0, false);
//        mygridCTS062.cells2(0, 1).setValue(mygridCTS062.cells2(0, 1).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("NormalProductPrice", rowID);
//        $('#' + ctrlID).attr('id', 'NormalProductPrice');
//        ctrlID = 'NormalProductPrice';
//        $('#' + ctrlID).css('width', '108px');

//        var val = mygridCTS062.cells2(1, 1).getValue();
//        //        mygridCTS062.cells2(1, 1).setValue(GenerateNumericBox("NormalInstallFee", "", val, true) + " " + C_CURRENCY_UNIT + " ");
//        //        $("#NormalInstallFee").BindNumericBox(14, 2, 0, 9999999999);
//        //        $("#NormalInstallFee").css('width', '108px');
//        //        $("#NormalInstallFee").attr("readonly", true);
//        var rowID = mygridCTS062.getRowId(1);
//        GenerateNumericBox2(mygridCTS062, "NormalInstallFee", rowID, "NormalPrice", val, 10, 2, 0, 9999999999, 0, false);
//        mygridCTS062.cells2(1, 1).setValue(mygridCTS062.cells2(1, 1).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("NormalInstallFee", rowID);
//        $('#' + ctrlID).attr('id', 'NormalInstallFee');
//        ctrlID = 'NormalInstallFee';
//        $('#' + ctrlID).css('width', '108px');

//        var val = mygridCTS062.cells2(2, 1).getValue();
//        //        mygridCTS062.cells2(2, 1).setValue(GenerateNumericBox("NormalSalePrice", "", val, true) + " " + C_CURRENCY_UNIT + " ");
//        //        $("#NormalSalePrice").BindNumericBox(14, 2, 0, 9999999999);
//        //        $("#NormalSalePrice").css('width', '108px');
//        //        $("#NormalSalePrice").attr("readonly", true);
//        var rowID = mygridCTS062.getRowId(2);
//        GenerateNumericBox2(mygridCTS062, "NormalSalePrice", rowID, "NormalPrice", val, 10, 2, 0, 9999999999, 0, false);
//        mygridCTS062.cells2(2, 1).setValue(mygridCTS062.cells2(2, 1).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("NormalSalePrice", rowID);
//        $('#' + ctrlID).attr('id', 'NormalSalePrice');
//        ctrlID = 'NormalSalePrice';
//        $('#' + ctrlID).css('width', '108px');

//        //-----------------------------------------------------------------------------------

//        //Column2----------------------------------------------------------------------------

//        var val = mygridCTS062.cells2(0, 2).getValue();
//        //        mygridCTS062.cells2(0, 2).setValue(GenerateNumericBox("OrderProductPrice", "", "", true) + " " + C_CURRENCY_UNIT + " " + _lblReq);
//        //        $("#OrderProductPrice").BindNumericBox(14, 2, 0, 9999999999);
//        //        $("#OrderProductPrice").css('width', '108px');
//        var rowID = mygridCTS062.getRowId(0);
//        GenerateNumericBox2(mygridCTS062, "OrderProductPrice", rowID, "OrderPrice", val, 10, 2, 0, 9999999999, 0, true);
//        mygridCTS062.cells2(0, 2).setValue(mygridCTS062.cells2(0, 2).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("OrderProductPrice", rowID);
//        $('#' + ctrlID).attr('id', 'OrderProductPrice');
//        ctrlID = 'OrderProductPrice';
//        $('#' + ctrlID).css('width', '108px');

//        var val = mygridCTS062.cells2(1, 2).getValue();
//        //        mygridCTS062.cells2(1, 2).setValue(GenerateNumericBox("OrderInstallFee", "", "", true) + " " + C_CURRENCY_UNIT + " " + _lblReq);
//        //        $("#OrderInstallFee").BindNumericBox(14, 2, 0, 9999999999);
//        //        $("#OrderInstallFee").css('width', '108px');
//        var rowID = mygridCTS062.getRowId(1);
//        GenerateNumericBox2(mygridCTS062, "OrderInstallFee", rowID, "OrderPrice", val, 10, 2, 0, 9999999999, 0, true);
//        mygridCTS062.cells2(1, 2).setValue(mygridCTS062.cells2(1, 2).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("OrderInstallFee", rowID);
//        $('#' + ctrlID).attr('id', 'OrderInstallFee');
//        ctrlID = 'OrderInstallFee';
//        $('#' + ctrlID).css('width', '108px');

//        var val = mygridCTS062.cells2(2, 2).getValue();
//        //        mygridCTS062.cells2(2, 2).setValue(GenerateNumericBox("OrderSalePrice", "", "", true) + " " + C_CURRENCY_UNIT + " " + _lblReq);
//        //        $("#OrderSalePrice").BindNumericBox(14, 2, 0, 9999999999);
//        //        $("#OrderSalePrice").css('width', '108px');
//        var rowID = mygridCTS062.getRowId(2);
//        GenerateNumericBox2(mygridCTS062, "OrderSalePrice", rowID, "OrderPrice", val, 10, 2, 0, 9999999999, 0, true);
//        mygridCTS062.cells2(2, 2).setValue(mygridCTS062.cells2(2, 2).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("OrderSalePrice", rowID);
//        $('#' + ctrlID).attr('id', 'OrderSalePrice');
//        ctrlID = 'OrderSalePrice';
//        $('#' + ctrlID).css('width', '108px');

//        //-----------------------------------------------------------------------------------

//        //Column3----------------------------------------------------------------------------

//        var val = mygridCTS062.cells2(2, 3).getValue();
//        //        mygridCTS062.cells2(2, 3).setValue(GenerateNumericBox("BillingAmt_ApproveContract", "", val, true) + " " + C_CURRENCY_UNIT + " ");
//        //        $("#BillingAmt_ApproveContract").BindNumericBox(14, 2, 0, 9999999999);
//        //        $("#BillingAmt_ApproveContract").css('width', '108px');
//        //        $("#BillingAmt_ApproveContract").attr("readonly", true);
//        var rowID = mygridCTS062.getRowId(2);
//        GenerateNumericBox2(mygridCTS062, "BillingAmt_ApproveContract", rowID, "BillingAmt_ApproveContract", val, 10, 2, 0, 9999999999, 0, false);
//        mygridCTS062.cells2(2, 3).setValue(mygridCTS062.cells2(2, 3).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("BillingAmt_ApproveContract", rowID);
//        $('#' + ctrlID).attr('id', 'BillingAmt_ApproveContract');
//        ctrlID = 'BillingAmt_ApproveContract';
//        $('#' + ctrlID).css('width', '108px');

//        //-----------------------------------------------------------------------------------

//        //Column4----------------------------------------------------------------------------

//        var val = mygridCTS062.cells2(2, 4).getValue();
//        //        mygridCTS062.cells2(2, 4).setValue(GenerateNumericBox("PartialFee", "", val, true) + " " + C_CURRENCY_UNIT + " ");
//        //        $("#PartialFee").BindNumericBox(14, 2, 0, 9999999999);
//        //        $("#PartialFee").css('width', '108px');
//        //        $("#PartialFee").attr("readonly", true);

//        var rowID = mygridCTS062.getRowId(2);
//        GenerateNumericBox2(mygridCTS062, "PartialFee", rowID, "PartialFee", val, 10, 2, 0, 9999999999, 0, false);
//        mygridCTS062.cells2(2, 4).setValue(mygridCTS062.cells2(2, 4).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("PartialFee", rowID);
//        $('#' + ctrlID).attr('id', 'PartialFee');
//        ctrlID = 'PartialFee';
//        $('#' + ctrlID).css('width', '108px');

//        //-----------------------------------------------------------------------------------

//        //Column5----------------------------------------------------------------------------

//        var val = mygridCTS062.cells2(2, 5).getValue();
//        //        mygridCTS062.cells2(2, 5).setValue(GenerateNumericBox("BillingAmt_Acceptance", "", val, true) + " " + C_CURRENCY_UNIT + " ");
//        //        $("#BillingAmt_Acceptance").BindNumericBox(14, 2, 0, 9999999999);
//        //        $("#BillingAmt_Acceptance").css('width', '108px');

//        var rowID = mygridCTS062.getRowId(2);
//        GenerateNumericBox2(mygridCTS062, "BillingAmt_Acceptance", rowID, "BillingAmt_Acceptance", val, 10, 2, 0, 9999999999, 0, true);
//        mygridCTS062.cells2(2, 5).setValue(mygridCTS062.cells2(2, 5).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("BillingAmt_Acceptance", rowID);
//        $('#' + ctrlID).attr('id', 'BillingAmt_Acceptance');
//        ctrlID = 'BillingAmt_Acceptance';
//        $('#' + ctrlID).css('width', '108px');

//        //-----------------------------------------------------------------------------------

//        mygridCTS062.attachEvent("onRowSelect", function (id, ind) {
//            var row_num = mygridCTS062.getRowIndex(id);
//            if (mygridCTS062.cell.childNodes[0].tagName == "INPUT") {
//                var txt = mygridCTS062.cell.childNodes[0];

//                if (txt.disabled == false) {
//                    txt.focus();
//                }
//            }
//        });

//        loadedChangePlanGrid = true;
//        BindingGridCalculate();

//    });
//}

function GetBillingTargetInformationDetailGrid() {
    BindingGridCalculate();
    call_ajax_method_json("/Contract/GetBillingTargetInformationDetailGrid_CTS062", "", function (result) {
        if (result != null) {
            //$('#BillingApprove').val(result[0].Amount);
            //$('#BillingPartialFee').val(result[1].Amount);
            ////$('#BillingAmt_CompleteInstallation').val(result[2].Amount);
            //$('#PaymethodCompleteInstallation').val(result[2].PayMethod);
            //$('#BillingTotal').val(result[3].Amount);

            $('#SalePrice_Approval').SetNumericCurrency(result[0].AmountCurrencyType);
            $('#SalePrice_Approval').val(result[0].Amount);
            $('#SalePrice_Partial').SetNumericCurrency(result[1].AmountCurrencyType);
            $('#SalePrice_Partial').val(result[1].Amount);
            $('#SalePrice_Acceptance').SetNumericCurrency(result[2].AmountCurrencyType);
            $('#SalePrice_Acceptance').val(result[2].Amount);
            $('#SalePrice_PaymentMethod_Acceptance').val(result[2].PayMethod);

            $('#InstallationFee_Approval').SetNumericCurrency(result[3].AmountCurrencyType);
            $('#InstallationFee_Approval').val(result[3].Amount);
            $('#InstallationFee_Partial').SetNumericCurrency(result[4].AmountCurrencyType);
            $('#InstallationFee_Partial').val(result[4].Amount);
            $('#InstallationFee_Acceptance').SetNumericCurrency(result[5].AmountCurrencyType);
            $('#InstallationFee_Acceptance').val(result[5].Amount);
            $('#InstallationFee_PaymentMethod_Acceptance').val(result[5].PayMethod);

            loadedBillingGrid = true;
            MappingToBillingGrid();
        }
    });
}

//function GetBillingTargetInformationDetailGrid() {
////    mygridBillingCTS062 = $("#gridChangePlanBillingDetail").LoadDataToGridWithInitial(0, false, false, "/Contract/GetBillingTargetInformationDetailGrid_CTS062",
////    "", "CTS062_DOBillingTargetDetailGridData", false);

////    BindOnLoadedEvent(mygridBillingCTS062, function () {
////        var val = mygridBillingCTS062.cells2(0, 1).getValue();
////        mygridBillingCTS062.cells2(0, 1).setValue(GenerateNumericBox("BillingApprove", "", val, true) + " " + C_CURRENCY_UNIT + " ");
//        //        $("#BillingApprove").BindNumericBox(14, 2, 0, 9999999999);
//        var rowID = mygridBillingCTS062.getRowId(0);
//        GenerateNumericBox2(mygridBillingCTS062, "BillingApprove", rowID, "Amount", val, 10, 2, 0, 9999999999, 0, false);
//        mygridBillingCTS062.cells2(0, 1).setValue(mygridBillingCTS062.cells2(0, 1).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("BillingApprove", rowID);
//        $('#' + ctrlID).attr('id', 'BillingApprove');
//        ctrlID = 'BillingApprove';
//        $("#" + ctrlID).css('width', '108px');
//        //$("#BillingApprove").attr("readonly", true);

//        var val = mygridBillingCTS062.cells2(1, 1).getValue();
////        mygridBillingCTS062.cells2(1, 1).setValue(GenerateNumericBox("BillingPartialFee", "", val, true) + " " + C_CURRENCY_UNIT + " ");
////        $("#BillingPartialFee").BindNumericBox(14, 2, 0, 9999999999);
//        var rowID = mygridBillingCTS062.getRowId(1);
//        GenerateNumericBox2(mygridBillingCTS062, "BillingPartialFee", rowID, "Amount", val, 10, 2, 0, 9999999999, 0, false);
//        mygridBillingCTS062.cells2(1, 1).setValue(mygridBillingCTS062.cells2(1, 1).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("BillingPartialFee", rowID);
//        $('#' + ctrlID).attr('id', 'BillingPartialFee');
//        ctrlID = 'BillingPartialFee';
//        $("#" + ctrlID).css('width', '108px');
////        $("#BillingPartialFee").attr("readonly", true);

//        var val = mygridBillingCTS062.cells2(2, 1).getValue();
////        mygridBillingCTS062.cells2(2, 1).setValue(GenerateNumericBox("BillingAmt_CompleteInstallation", "", val, true) + " " + C_CURRENCY_UNIT + " ");
////        $("#BillingAmt_CompleteInstallation").BindNumericBox(14, 2, 0, 9999999999);
//        //        $("#BillingAmt_CompleteInstallation").css('width', '108px');
//        var rowID = mygridBillingCTS062.getRowId(2);
//        GenerateNumericBox2(mygridBillingCTS062, "BillingAmt_CompleteInstallation", rowID, "Amount", val, 10, 2, 0, 9999999999, 0, false);
//        mygridBillingCTS062.cells2(2, 1).setValue(mygridBillingCTS062.cells2(2, 1).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("BillingAmt_CompleteInstallation", rowID);
//        $('#' + ctrlID).attr('id', 'BillingAmt_CompleteInstallation');
//        ctrlID = 'BillingAmt_CompleteInstallation';
//        $("#" + ctrlID).css('width', '108px');
//        //$("#BillingAmt_CompleteInstallation").attr("readonly", true);

//        var paymentMethodParameter = { id: "PaymethodCompleteInstallation" }
//        call_ajax_method_json('/Contract/GetComboBoxPaymentMethod_CTS062', paymentMethodParameter,
//                function (result, controls) {
//                    var payMethodCode = mygridBillingCTS062.cells2(2, 2).getValue();
//                    payMethodCode = payMethodCode.replace("^null", "");
//                    mygridBillingCTS062.cells2(2, 2).setValue(result + _lblReq);
//                    $("#PaymethodCompleteInstallation").val(payMethodCode);
//                });


//        var val = mygridBillingCTS062.cells2(3, 1).getValue();
////        mygridBillingCTS062.cells2(3, 1).setValue(GenerateNumericBox("BillingTotal", "", val, true) + " " + C_CURRENCY_UNIT + " ");
////        $("#BillingTotal").BindNumericBox(14, 2, 0, 9999999999);
//        //        $("#BillingTotal").css('width', '100px');
//        var rowID = mygridBillingCTS062.getRowId(3);
//        GenerateNumericBox2(mygridBillingCTS062, "BillingTotal", rowID, "Amount", val, 10, 2, 0, 9999999999, 0, false);
//        mygridBillingCTS062.cells2(3, 1).setValue(mygridBillingCTS062.cells2(3, 1).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("BillingTotal", rowID);
//        $('#' + ctrlID).attr('id', 'BillingTotal');
//        ctrlID = 'BillingTotal';
//        $("#" + ctrlID).css('width', '108px');
//        //$("#BillingTotal").attr("readonly", true);

//        loadedBillingGrid = true;
//        BindingGridCalculate();
//    }

function GetBillingTargetInformationDetail() {
    call_ajax_method_json('/Contract/GetBillingTargetInformationDetail_CTS062', "",
    function (result, controls) {
        BindBillingDetail(result);
    });
}

function BindBillingDetail(result) {
    $("#BillingTargetCodeDetail").val(result.BillingTargetCodeDetail);
    $("#BillingClientCodeDetail").val(result.BillingClientCodeDetail);
    $("#FullNameEN").val(result.FullNameEN);
    $("#BranchNameEN").val(result.BranchNameEN);
    $("#AddressEN").val(result.AddressEN);
    $("#FullNameLC").val(result.FullNameLC);
    $("#BranchNameLC").val(result.BranchNameLC);
    $("#AddressLC").val(result.AddressLC);
    $("#Nationality").val(result.Nationality);
    $("#PhoneNo").val(result.PhoneNo);
    $("#BusinessType").val(result.BusinessType);
    $("#BillingOffice").val(result.BillingOffice);
}

function BtnRegisterClick() {
    VaridateCtrl(validateRegisForm, null);
    DisableRegisterCommand(true);
    DisableResetCommand(true);
    var distributetype;
    if ($("#rdoNotDistributed").prop('checked') == true) {
        distributetype = $("#rdoNotDistributed").val();       
    }                                                                              

    if ($("#rdoDistributedOrigin").prop('checked') == true) {
        distributetype = $("#rdoDistributedOrigin").val();        
    }

    if ($("#rdoDistributedTarget").prop('checked') == true) {
        distributetype = $("#rdoDistributedTarget").val();   
    }
   
    var obj = {
        ExpectedInstallCompleteDate: $("#ExpectedInstallCompleteDate").val(),
        ExpectedCustAcceptanceDate: $("#ExpectedCustAcceptanceDate").val(),
        //NormalSalePrice: $("#NormalSalePrice").val(),
        //OrderProductPrice: $("#OrderProductPrice").val(),
        //OrderInstallFee: $("#OrderInstallFee").val(),
        //OrderSalePrice: $("#OrderSalePrice").val(),
        //PartialFee: $("#PartialFee").val(),
        NegotiationStaffEmpNo1: $("#NegotiationStaffEmpNo1").val(),
        //BillingAmt_Acceptance: $("#BillingAmt_Acceptance").val(),
        //BillingAmt_ApproveContract: $("#BillingAmt_ApproveContract").val(),
        //BillingAmt_CompleteInstallation: $("#BillingAmt_CompleteInstallation").val(),
        //PaymethodCompleteInstallation: $("#PaymethodCompleteInstallation").val(),

        OrderProductPriceCurrencyType: $("#OrderProductPrice").NumericCurrencyValue(),
        OrderProductPrice: $("#OrderProductPrice").val(),
        BillingAmt_ApproveContractCurrencyType: $("#BillingAmt_ApproveContract").NumericCurrencyValue(),
        BillingAmt_ApproveContract: $("#BillingAmt_ApproveContract").val(),
        BillingAmt_PartialFeeCurrencyType: $("#BillingAmt_PartialFee").NumericCurrencyValue(),
        BillingAmt_PartialFee: $("#BillingAmt_PartialFee").val(),
        BillingAmt_AcceptanceCurrencyType: $("#BillingAmt_Acceptance").NumericCurrencyValue(),
        BillingAmt_Acceptance: $("#BillingAmt_Acceptance").val(),

        OrderInstallFeeCurrencyType: $("#OrderInstallFee").NumericCurrencyValue(),
        OrderInstallFee: $("#OrderInstallFee").val(),
        BillingAmtInstallation_ApproveContractCurrencyType: $("#BillingAmtInstallation_ApproveContract").NumericCurrencyValue(),
        BillingAmtInstallation_ApproveContract: $("#BillingAmtInstallation_ApproveContract").val(),
        BillingAmtInstallation_PartialFeeCurrencyType: $("#BillingAmtInstallation_PartialFee").NumericCurrencyValue(),
        BillingAmtInstallation_PartialFee: $("#BillingAmtInstallation_PartialFee").val(),
        BillingAmtInstallation_AcceptanceCurrencyType: $("#BillingAmtInstallation_Acceptance").NumericCurrencyValue(),
        BillingAmtInstallation_Acceptance: $("#BillingAmtInstallation_Acceptance").val(),

        NegotiationStaffEmpNo1: $("#NegotiationStaffEmpNo1").val(),
        NegotiationStaffEmpNo2: $("#NegotiationStaffEmpNo2").val(),
        DistributedType: distributetype,
        DistributedCode: $("#DistributedOriginCode").val(),
        ConnectionFlag: $("#ConnectionFlag").prop("checked"),
        ConnectTargetCode: $("#ConnectTargetCode").val(),
        ApproveNo1: $("#ApproveNo1").val(),
        ApproveNo2: $("#ApproveNo2").val(),
        ApproveNo3: $("#ApproveNo3").val(),
        ApproveNo4: $("#ApproveNo4").val(),
        ApproveNo5: $("#ApproveNo5").val(),

        SalePrice_ApprovalCurrencyType: $("#SalePrice_Approval").NumericCurrencyValue(),
        SalePrice_Approval: $("#SalePrice_Approval").val(),
        SalePrice_PartialCurrencyType: $("#SalePrice_Partial").NumericCurrencyValue(),
        SalePrice_Partial: $("#SalePrice_Partial").val(),
        SalePrice_AcceptanceCurrencyType: $("#SalePrice_Acceptance").NumericCurrencyValue(),
        SalePrice_Acceptance: $("#SalePrice_Acceptance").val(),
        SalePrice_PaymentMethod_Acceptance: $("#SalePrice_PaymentMethod_Acceptance").val(),

        InstallationFee_ApprovalCurrencyType: $("#InstallationFee_Approval").NumericCurrencyValue(),
        InstallationFee_Approval: $("#InstallationFee_Approval").val(),
        InstallationFee_PartialCurrencyType: $("#InstallationFee_Partial").NumericCurrencyValue(),
        InstallationFee_Partial: $("#InstallationFee_Partial").val(),
        InstallationFee_AcceptanceCurrencyType: $("#InstallationFee_Acceptance").NumericCurrencyValue(),
        InstallationFee_Acceptance: $("#InstallationFee_Acceptance").val(),
        InstallationFee_PaymentMethod_Acceptance: $("#InstallationFee_PaymentMethod_Acceptance").val()
    };
        call_ajax_method_json('/Contract/ValidateRequireField_CTS062', obj,
        function (result, controls) {
            VaridateCtrl(validateRegisForm, controls);
            if (result == undefined) {
                call_ajax_method_json('/Contract/RegisterClick_CTS062', obj,
                function (result, controls) {
                    VaridateCtrl(validateRegisForm, controls);
                    if (result == undefined) {
                        EnableConfirmCommand();

                        var isLess = $("#SalemanSection2").is(':hidden');

                        $('#SaleContractChangeSection').SetViewMode(true);
                        $('#SpecifyQuotationSection').SetViewMode(true);
                        $('#ChangePlanSection').SetViewMode(true);
                        $('#BillingTargetDetailSection').SetViewMode(true);

                        $('#divPaymethodCompleteInstallation').attr('style', 'width: 160px;');

                        FixEmpNameViewMode();

                        $('#divQuotationTargetCode').html($('#QuotationTargetCode').val() + '-' + $('#Alphabet').val());
                        $('#divAlphabet').hide();
                        $('#divMore').hide();
                        $('#divLess').hide();

                        if (isLess) {
                            LessClick();
                        } else {
                            MoreClick();
                        }

                        DisableRegisterCommand(false);
                        DisableResetCommand(false);
                    }
                });
            }

            
        });
    }

function FixEmpNameViewMode() {
    for (var i = 1; i <= 10; i++) {
        if ($('#SalesmanEmpNo' + i.toString()).val().length == 0) {
            $('#divSalesmanEmpName' + i.toString()).text('');
        } else {
            $('#divSalesmanEmpName' + i.toString()).attr('style', 'padding-left: 4px;');
        }
    }

    if ($('#NegotiationStaffEmpNo1').val().length == 0) {
        $('#divNegotiationStaffEmpName1').text('');
    }

    if ($('#NegotiationStaffEmpNo2').val().length == 0) {
        $('#divNegotiationStaffEmpName2').text('');
    }

    $('#divNegotiationStaffEmpName1').attr('style', 'padding-left:4px;');
    $('#divNegotiationStaffEmpName2').attr('style', 'padding-left:4px;');
}

function BtnConfirmClick() {
//    var obj = { ExpectedInstallCompleteDate: $("#ExpectedInstallCompleteDate").val(),
//        ExpectedCustAcceptanceDate: $("#ExpectedCustAcceptanceDate").val(),
//        NormalSalePrice: $("#NormalSalePrice").val(),
//        OrderProductPrice: $("#OrderProductPrice").val(),
//        OrderInstallFee: $("#OrderInstallFee").val(),
//        OrderSalePrice: $("#OrderSalePrice").val(),
//        OrderSalePrice: $("#OrderSalePrice").val(),
//        PartialFee: $("#PartialFee").val(),
//        NegotiationStaffEmpNo1: $("#NegotiationStaffEmpNo1").val(),
//        BillingAmt_Acceptance: $("#BillingAmt_Acceptance").val(),
//        BillingAmt_ApproveContract: $("#BillingAmt_ApproveContract").val(),
//        BillingAmt_CompleteInstallation: $("#BillingAmt_CompleteInstallation").val(),
//        PaymethodCompleteInstallation: $("#PaymethodCompleteInstallation").val(),
//        NegotiationStaffEmpNo1: $("#NegotiationStaffEmpNo1").val(),
//        NegotiationStaffEmpNo2: $("#NegotiationStaffEmpNo2").val()
    //    };
    DisableConfirmCommand(true);
    var distributetype;
    if ($("#rdoNotDistributed").prop('checked') == true) {
        distributetype = $("#rdoNotDistributed").val();
    }

    if ($("#rdoDistributedOrigin").prop('checked') == true) {
        distributetype = $("#rdoDistributedOrigin").val();
    }

    if ($("#rdoDistributedTarget").prop('checked') == true) {
        distributetype = $("#rdoDistributedTarget").val();
    }

    //var obj = { ExpectedInstallCompleteDate: $("#ExpectedInstallCompleteDate").val(),
    //    ExpectedCustAcceptanceDate: $("#ExpectedCustAcceptanceDate").val(),
    //    NormalSalePrice: $("#NormalSalePrice").val(),
    //    OrderProductPrice: $("#OrderProductPrice").val(),
    //    OrderInstallFee: $("#OrderInstallFee").val(),
    //    OrderSalePrice: $("#OrderSalePrice").val(),
    //    OrderSalePrice: $("#OrderSalePrice").val(),
    //    PartialFee: $("#PartialFee").val(),
    //    NegotiationStaffEmpNo1: $("#NegotiationStaffEmpNo1").val(),
    //    BillingAmt_Acceptance: $("#BillingAmt_Acceptance").val(),
    //    BillingAmt_ApproveContract: $("#BillingAmt_ApproveContract").val(),
    //    BillingAmt_CompleteInstallation: $("#BillingAmt_CompleteInstallation").val(),
    //    PaymethodCompleteInstallation: $("#PaymethodCompleteInstallation").val(),
    //    NegotiationStaffEmpNo1: $("#NegotiationStaffEmpNo1").val(),
    //    NegotiationStaffEmpNo2: $("#NegotiationStaffEmpNo2").val(),
    //    DistributedType: distributetype,
    //    DistributedCode: $("#DistributedOriginCode").val(),
    //    ConnectionFlag: $("#ConnectionFlag").prop("checked"),
    //    ConnectTargetCode: $("#ConnectTargetCode").val(),
    //    ApproveNo1: $("#ApproveNo1").val(),
    //    ApproveNo2: $("#ApproveNo2").val(),
    //    ApproveNo3: $("#ApproveNo3").val(),
    //    ApproveNo4: $("#ApproveNo4").val(),
    //    ApproveNo5: $("#ApproveNo5").val()
    //};
    var obj = {
        ExpectedInstallCompleteDate: $("#ExpectedInstallCompleteDate").val(),
        ExpectedCustAcceptanceDate: $("#ExpectedCustAcceptanceDate").val(),
        //NormalSalePrice: $("#NormalSalePrice").val(),
        //OrderProductPrice: $("#OrderProductPrice").val(),
        //OrderInstallFee: $("#OrderInstallFee").val(),
        //OrderSalePrice: $("#OrderSalePrice").val(),
        //PartialFee: $("#PartialFee").val(),
        NegotiationStaffEmpNo1: $("#NegotiationStaffEmpNo1").val(),
        //BillingAmt_Acceptance: $("#BillingAmt_Acceptance").val(),
        //BillingAmt_ApproveContract: $("#BillingAmt_ApproveContract").val(),
        //BillingAmt_CompleteInstallation: $("#BillingAmt_CompleteInstallation").val(),
        //PaymethodCompleteInstallation: $("#PaymethodCompleteInstallation").val(),

        OrderProductPriceCurrencyType: $("#OrderProductPrice").NumericCurrencyValue(),
        OrderProductPrice: $("#OrderProductPrice").val(),
        BillingAmt_ApproveContractCurrencyType: $("#BillingAmt_ApproveContract").NumericCurrencyValue(),
        BillingAmt_ApproveContract: $("#BillingAmt_ApproveContract").val(),
        BillingAmt_PartialFeeCurrencyType: $("#BillingAmt_PartialFee").NumericCurrencyValue(),
        BillingAmt_PartialFee: $("#BillingAmt_PartialFee").val(),
        BillingAmt_AcceptanceCurrencyType: $("#BillingAmt_Acceptance").NumericCurrencyValue(),
        BillingAmt_Acceptance: $("#BillingAmt_Acceptance").val(),

        OrderInstallFeeCurrencyType: $("#OrderInstallFee").NumericCurrencyValue(),
        OrderInstallFee: $("#OrderInstallFee").val(),
        BillingAmtInstallation_ApproveContractCurrencyType: $("#BillingAmtInstallation_ApproveContract").NumericCurrencyValue(),
        BillingAmtInstallation_ApproveContract: $("#BillingAmtInstallation_ApproveContract").val(),
        BillingAmtInstallation_PartialFeeCurrencyType: $("#BillingAmtInstallation_PartialFee").NumericCurrencyValue(),
        BillingAmtInstallation_PartialFee: $("#BillingAmtInstallation_PartialFee").val(),
        BillingAmtInstallation_AcceptanceCurrencyType: $("#BillingAmtInstallation_Acceptance").NumericCurrencyValue(),
        BillingAmtInstallation_Acceptance: $("#BillingAmtInstallation_Acceptance").val(),

        NegotiationStaffEmpNo1: $("#NegotiationStaffEmpNo1").val(),
        NegotiationStaffEmpNo2: $("#NegotiationStaffEmpNo2").val(),
        DistributedType: distributetype,
        DistributedCode: $("#DistributedOriginCode").val(),
        ConnectionFlag: $("#ConnectionFlag").prop("checked"),
        ConnectTargetCode: $("#ConnectTargetCode").val(),
        ApproveNo1: $("#ApproveNo1").val(),
        ApproveNo2: $("#ApproveNo2").val(),
        ApproveNo3: $("#ApproveNo3").val(),
        ApproveNo4: $("#ApproveNo4").val(),
        ApproveNo5: $("#ApproveNo5").val(),

        SalePrice_ApprovalCurrencyType: $("#SalePrice_Approval").NumericCurrencyValue(),
        SalePrice_Approval: $("#SalePrice_Approval").val(),
        SalePrice_PartialCurrencyType: $("#SalePrice_Partial").NumericCurrencyValue(),
        SalePrice_Partial: $("#SalePrice_Partial").val(),
        SalePrice_AcceptanceCurrencyType: $("#SalePrice_Acceptance").NumericCurrencyValue(),
        SalePrice_Acceptance: $("#SalePrice_Acceptance").val(),
        SalePrice_PaymentMethod_Acceptance: $("#SalePrice_PaymentMethod_Acceptance").val(),

        InstallationFee_ApprovalCurrencyType: $("#InstallationFee_Approval").NumericCurrencyValue(),
        InstallationFee_Approval: $("#InstallationFee_Approval").val(),
        InstallationFee_PartialCurrencyType: $("#InstallationFee_Partial").NumericCurrencyValue(),
        InstallationFee_Partial: $("#InstallationFee_Partial").val(),
        InstallationFee_AcceptanceCurrencyType: $("#InstallationFee_Acceptance").NumericCurrencyValue(),
        InstallationFee_Acceptance: $("#InstallationFee_Acceptance").val(),
        InstallationFee_PaymentMethod_Acceptance: $("#InstallationFee_PaymentMethod_Acceptance").val()
    };

    call_ajax_method_json('/Contract/ConfirmClick_CTS062', obj, function (result, controls) {
        if ((result != null) && (result != false)) {
            OpenInformationMessageDialog(result.Code, result.Message, function () {
                window.location.href = generate_url("/common/CMS020");
            }, null);
        }

        DisableConfirmCommand(false);
    });
}

function BtnBackClick() {
    DisableConfirmCommand(true);
    EnableRegisterCommand();

    var isLess = $("#SalemanSection2").is(':hidden');

    $('#SaleContractChangeSection').SetViewMode(false);
    $('#ImportantFlag').attr('disabled', 'disabled');

    $('#SpecifyQuotationSection').SetViewMode(false);
    $('#ChangePlanSection').SetViewMode(false);
    $('#BillingTargetDetailSection').SetViewMode(false);

    if (isLess) {
        LessClick();
    } else {
        MoreClick();
    }

    DisableConfirmCommand(false);
}

function BtnResetClick() {
    DisableRegisterCommand(true);
    DisableResetCommand(true);

    var obj = {
        module: "Common",
        code: "MSG0038"
    };
    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenOkCancelDialog(result.Code, result.Message,
        function () {
            call_ajax_method_json('/Contract/ResetData_CTS062', "", function (result, controls) {
                if ((result != null)) {
                    DisableRegisterCommand(true);
                    $("#ChangePlanSection").hide();
                    $("#BillingTargetDetailSection").hide();
                    $("#Alphabet").val("");
                    $("#btnViewQuotationDetail").attr("disabled", true);
                    $("#btnSpecifyClear").attr("disabled", true);

                    $("#ProductName").val("");
                    ISDisableSpecifyQuotation(false);

                    $('#SaleContractChangeSection').SetViewMode(false);
                    $('#SpecifyQuotationSection').SetViewMode(false);
                    $('#ChangePlanSection').SetViewMode(false);
                    $('#BillingTargetDetailSection').SetViewMode(false);

                    $('#SalemanSection2').hide();
                    $('#SalemanSection3').hide();
                    $("#More").show();

                    ReBindingData(result);
                    DisableAllCommand();
                }
            });
        },
        null);

        DisableRegisterCommand(false);
        DisableResetCommand(false);
    });
}

function ReBindingData(dataObj) {
    ObjCTS062 = { ServiceTypeCode: dataObj.ServiceTypeCode,
        TargetCodeType: dataObj.TargetCodeType,
        LastOCC: dataObj.LastOCC,
        ExpectedInstallCompleteDate: dataObj.ExpectedInstallCompleteDate, 
        EmpNo: "" }

    if (dataObj.ImportantFlag) {
        $('#chkIsimportantCustomer').attr('checked', 'checked');
    } else {
        $('#chkIsimportantCustomer').removeAttr('checked');
    }

    $('#ContractCode').val(dataObj.ContractCodeShort);
    $('#PurchaserCustCode').val(dataObj.PurchaserCustCode);
    $('#RealCustomerCustCode').val(dataObj.RealCustomerCustCode);
    $('#SiteCode').val(dataObj.SiteCode);
    $('#PurchaserNameEN').val(dataObj.PurchaserNameEN);
    $('#PurchaserAddressEN').val(dataObj.PurchaserAddressEN);
    $('#SiteNameEN').val(dataObj.SiteNameEN);
    $('#SiteAddressEN').val(dataObj.SiteAddressEN);
    $('#CustFullNameLC').val(dataObj.PurchaserNameLC);
    $('#PurchaserAddressLocal').val(dataObj.PurchaserAddressLC);
    $('#SiteNameLC').val(dataObj.SiteNameLC);
    $('#SiteAddressLC').val(dataObj.SiteAddressLC);
    //$('#InstallationStatusName').val(dataObj.InstallationStatusCodeName);
    $('#OperationOffice').val(dataObj.OperationOfficeName);
}

//function MaintainScreenItemOnInit() {
//    SetRegisterCommand(true, BtnRegisterClick);
//    SetResetCommand(true, BtnResetClick);
//    SetConfirmCommand(false, BtnConfirmClick);
//    SetBackCommand(false, BtnConfirmClick);
//}

//function ISDisableRegister(status) {    
//    if (status == false) {
//        SetRegisterCommand(true, BtnRegisterClick);
//    }

//    $('#btnCommandRegister').attr('disabled', status);
//}

function ISDisableDistributedOriginCode() {
    if ($("#rdoDistributedTarget").prop("checked") == true) {
        $("#DistributedOriginCode").attr("readonly", false);
    }
}

function ISDisableSpecifyQuotation(status) {
    $("#btnRetrieve").attr("disabled", status);
    $("#btnSearchQuotation").attr("disabled", status);
    $("#Alphabet").attr("readonly", status);
}

function EnableRegisterCommand() {
    DisableAllCommand();
    SetRegisterCommand(true, BtnRegisterClick);
    SetResetCommand(true, BtnResetClick);
}

function EnableConfirmCommand() {
    DisableAllCommand();
    SetConfirmCommand(true, BtnConfirmClick);
    SetBackCommand(true, BtnBackClick);
}

function DisableAllCommand() {
    SetRegisterCommand(false, null);
    SetResetCommand(false, null);
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
}

function GetValidateObject() {
    //var obj = { ExpectedInstallCompleteDate: $("#ExpectedInstallCompleteDate").val(),
    //    ExpectedCustAcceptanceDate: $("#ExpectedCustAcceptanceDate").val(),
    //    NormalSalePrice: $("#NormalSalePrice").val(),
    //    OrderProductPrice: $("#OrderProductPrice").val(),
    //    OrderInstallFee: $("#OrderInstallFee").val(),
    //    OrderSalePrice: $("#OrderSalePrice").val(),
    //    OrderSalePrice: $("#OrderSalePrice").val(),
    //    PartialFee: $("#PartialFee").val(),
    //    NegotiationStaffEmpNo1: $("#NegotiationStaffEmpNo1").val(),
    //    BillingAmt_Acceptance: $("#BillingAmt_Acceptance").val(),
    //    BillingAmt_ApproveContract: $("#BillingAmt_ApproveContract").val(),
    //    BillingAmt_CompleteInstallation: $("#BillingAmt_CompleteInstallation").val(),
    //    PaymethodCompleteInstallation: $("#PaymethodCompleteInstallation").val(),
    //    NegotiationStaffEmpNo1: $("#NegotiationStaffEmpNo1").val(),
    //    NegotiationStaffEmpNo2: $("#NegotiationStaffEmpNo2").val()
    //};

    var obj = {
        ExpectedInstallCompleteDate: $("#ExpectedInstallCompleteDate").val(),
        ExpectedCustAcceptanceDate: $("#ExpectedCustAcceptanceDate").val(),
        //NormalSalePrice: $("#NormalSalePrice").val(),
        //OrderProductPrice: $("#OrderProductPrice").val(),
        //OrderInstallFee: $("#OrderInstallFee").val(),
        //OrderSalePrice: $("#OrderSalePrice").val(),
        //PartialFee: $("#PartialFee").val(),
        NegotiationStaffEmpNo1: $("#NegotiationStaffEmpNo1").val(),
        //BillingAmt_Acceptance: $("#BillingAmt_Acceptance").val(),
        //BillingAmt_ApproveContract: $("#BillingAmt_ApproveContract").val(),
        //BillingAmt_CompleteInstallation: $("#BillingAmt_CompleteInstallation").val(),
        //PaymethodCompleteInstallation: $("#PaymethodCompleteInstallation").val(),

        OrderProductPriceCurrencyType: $("#OrderProductPrice").NumericCurrencyValue(),
        OrderProductPrice: $("#OrderProductPrice").val(),
        BillingAmt_ApproveContractCurrencyType: $("#BillingAmt_ApproveContract").NumericCurrencyValue(),
        BillingAmt_ApproveContract: $("#BillingAmt_ApproveContract").val(),
        BillingAmt_PartialFeeCurrencyType: $("#BillingAmt_PartialFee").NumericCurrencyValue(),
        BillingAmt_PartialFee: $("#BillingAmt_PartialFee").val(),
        BillingAmt_AcceptanceCurrencyType: $("#BillingAmt_Acceptance").NumericCurrencyValue(),
        BillingAmt_Acceptance: $("#BillingAmt_Acceptance").val(),

        OrderInstallFeeCurrencyType: $("#OrderInstallFee").NumericCurrencyValue(),
        OrderInstallFee: $("#OrderInstallFee").val(),
        BillingAmtInstallation_ApproveContractCurrencyType: $("#BillingAmtInstallation_ApproveContract").NumericCurrencyValue(),
        BillingAmtInstallation_ApproveContract: $("#BillingAmtInstallation_ApproveContract").val(),
        BillingAmtInstallation_PartialFeeCurrencyType: $("#BillingAmtInstallation_PartialFee").NumericCurrencyValue(),
        BillingAmtInstallation_PartialFee: $("#BillingAmtInstallation_PartialFee").val(),
        BillingAmtInstallation_AcceptanceCurrencyType: $("#BillingAmtInstallation_Acceptance").NumericCurrencyValue(),
        BillingAmtInstallation_Acceptance: $("#BillingAmtInstallation_Acceptance").val(),

        NegotiationStaffEmpNo1: $("#NegotiationStaffEmpNo1").val(),
        NegotiationStaffEmpNo2: $("#NegotiationStaffEmpNo2").val(),
        DistributedType: distributetype,
        DistributedCode: $("#DistributedOriginCode").val(),
        ConnectionFlag: $("#ConnectionFlag").prop("checked"),
        ConnectTargetCode: $("#ConnectTargetCode").val(),
        ApproveNo1: $("#ApproveNo1").val(),
        ApproveNo2: $("#ApproveNo2").val(),
        ApproveNo3: $("#ApproveNo3").val(),
        ApproveNo4: $("#ApproveNo4").val(),
        ApproveNo5: $("#ApproveNo5").val(),

        SalePrice_ApprovalCurrencyType: $("#SalePrice_Approval").NumericCurrencyValue(),
        SalePrice_Approval: $("#SalePrice_Approval").val(),
        SalePrice_PartialCurrencyType: $("#SalePrice_Partial").NumericCurrencyValue(),
        SalePrice_Partial: $("#SalePrice_Partial").val(),
        SalePrice_AcceptanceCurrencyType: $("#SalePrice_Acceptance").NumericCurrencyValue(),
        SalePrice_Acceptance: $("#SalePrice_Acceptance").val(),
        SalePrice_PaymentMethod_Acceptance: $("#SalePrice_PaymentMethod_Acceptance").val(),

        InstallationFee_ApprovalCurrencyType: $("#InstallationFee_Approval").NumericCurrencyValue(),
        InstallationFee_Approval: $("#InstallationFee_Approval").val(),
        InstallationFee_PartialCurrencyType: $("#InstallationFee_Partial").NumericCurrencyValue(),
        InstallationFee_Partial: $("#InstallationFee_Partial").val(),
        InstallationFee_AcceptanceCurrencyType: $("#InstallationFee_Acceptance").NumericCurrencyValue(),
        InstallationFee_Acceptance: $("#InstallationFee_Acceptance").val(),
        InstallationFee_PaymentMethod_Acceptance: $("#InstallationFee_PaymentMethod_Acceptance").val()
    };
}

function SpecifyClearClick() {
    ISDisableSpecifyQuotation(false);
    DisableRegisterCommand(true);

    loadedBillingGrid = false;
    loadedChangePlanGrid = false;

    DisableAllCommand();

    $('#btnSpecifyClear').attr('disabled', 'true');
    $('#btnViewQuotationDetail').attr('disabled', 'true');

    $("#Alphabet").val("");
    $("#ProductName").val("");    
    $("#ChangePlanSection").hide();
    $("#BillingTargetDetailSection").hide();
}

function QUS011Object() {
    return {
        "QuotationTargetCode": $("#QuotationTargetCode").val(),
        "Alphabet": $("#Alphabet").val(),
        "HideQuotationTarget": true
    };
}

function QUS010Object() {
    return {
        strCallerScreenID: "CTS062"
        , ViewMode: '2'
        , strServiceTypeCode: ObjCTS062.ServiceTypeCode
        , strTargetCodeTypeCode: ObjCTS062.TargetCodeType
        , strQuotationTargetCode: $("#QuotationTargetCode").val()
    };
}

function QUS010Response(result) {
    $("#dlgCTS062").CloseDialog();
    $("#Alphabet").val(result.Alphabet);
    BtnRetrieveClick();
}

function BindingGridCalculate() {
    //if (loadedBillingGrid && loadedChangePlanGrid) {
        $("#BillingAmt_Acceptance").blur(MappingToBillingGrid);

        // First time load
        MappingToBillingGrid();
    //}
}

function MappingToBillingGrid() {
    var _BillingAmt_ApproveContract = $('#BillingAmt_ApproveContract').val();
    var _PartialFee = $('#PartialFee').val();
    var _BillingAmt_Acceptance = $('#BillingAmt_Acceptance').val();

    $('#BillingApprove').val(_BillingAmt_ApproveContract);
    $('#BillingPartialFee').val(_PartialFee);
    $('#BillingAmt_CompleteInstallation').val(_BillingAmt_Acceptance);

    CalculateBillingTotal();
}

function CalculateBillingTotal() {
    //var approveVal = ($('#BillingApprove').val().length > 0) ? parseFloat($('#BillingApprove').val().replace(/ /g, "").replace(/,/g, "")) : 0;
    //var partialVal = ($('#BillingPartialFee').val().length > 0) ? parseFloat($('#BillingPartialFee').val().replace(/ /g, "").replace(/,/g, "")) : 0;
    //var completeVal = ($('#BillingAmt_CompleteInstallation').val().length > 0) ? parseFloat($('#BillingAmt_CompleteInstallation').val().replace(/ /g, "").replace(/,/g, "")) : 0;

    //var res = 0;
    //res = approveVal + partialVal + completeVal;

    //var resTxt = res = res.toFixed(2);
    //$('#BillingTotal').val(resTxt);
    //$('#BillingTotal').setComma();
}