

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
/// <reference path="../../Scripts/json2.js" />


/// <reference path = "../../Scripts/Base/GridControl.js" />

var pageRow = 0;
var ISS060_GridEmail;
var ISS060_GridInstrumentInfo;
var ISS060_gridAttach;
var isInitAttachGrid = false;
var hasAlert = false;
var alertMsg = "";
var strNewRow = "NEWROW";
var strTrue = "true";
var strFalse = "false";
var tempCalNormalContractFee = 0;

var onRetrieveNormalInstallFee;
// Main
$(document).ready(function () {


    var strContractProjectCode = $("#ContractCodeProjectCode").val();


    $("#chkCondHave1").click(clickHaveCondition);
    $("#chkCondHave2").click(clickHaveCondition);
    $("#chkCondHave3").click(clickHaveCondition);
    $("#chkCondHave4").click(clickHaveCondition);
    $("#chkCondHave5").click(clickHaveCondition);

    //$("#NewBldMgmtCost").BindNumericBox(10, 2, 0, 9999999999, 0);
    $("#BillingInstallFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#NormalInstallFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#SECOMPaymentFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#SECOMRevenueFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);

    $("#NormalContractFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#NewNormalContractFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);

    $("#NewBillingInstallFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#NewNormalInstallFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#NewSECOMPaymentFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#NewSECOMRevenueFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);

    $("#InstallFinishDate").InitialDate();
    $("#InstallStartDate").InitialDate();
    $("#InstallCompleteDate").InitialDate();
    
    $("#InstallCompleteDate").SetMinDate(C_INTALL_COMPLETE_MINDATE);
    $("#InstallCompleteDate").SetMaxDate(C_INTALL_COMPLETE_MAXDATE);
    
    // 20170217 nakajima modify start
    //$("#NewNormalInstallFee").blur(initialSECOMNPayment);
    //$("#NewBillingInstallFee").blur(initialSECOMNPayment);
    // 20170217 nakajima modify end
    //$("#btnClearInstallation").click(clear_installation_click);


    initialGridOnload();

    InitLoadAttachList();



    setInitialState();

    //================ GRID ATTACH ========================================    
    $('#frmAttach').attr('src', 'ISS060_Upload?k=' + _attach_k);

    SpecialGridControl(ISS060_gridAttach, ["removeButton"]);
    BindOnLoadedEvent(ISS060_gridAttach, ISS060_gridAttachBinding);
    //====================================================================

    if (strContractProjectCode != "") {
        $("#ContractCodeProjectCode").val(strContractProjectCode)
        setTimeout("retrieve_installation_click()", 2000);
    }

    $("#ChangeCustomerReason").attr("disabled", true);
    $("#ChangeSecomReason").attr("disabled", true);

    $("#ChangeContents").SetMaxLengthTextArea(4000);
    $("#Memo").SetMaxLengthTextArea(4000);
    $("#NewMemo").SetMaxLengthTextArea(4000);

    SpecialGridControl(ISS060_GridInstrumentInfo, ["AddRemovedQTY"]);
    SpecialGridControl(ISS060_GridInstrumentInfo, ["MovedQTY"]);
    SpecialGridControl(ISS060_GridInstrumentInfo, ["MAExchangeQTY"]);
    SpecialGridControl(ISS060_GridInstrumentInfo, ["NotInstallQTY"]);
    SpecialGridControl(ISS060_GridInstrumentInfo, ["UnremovableQTY"]);

    $("#divStockoutOriginAdditional").hide();

});

function setInitialState() {
    // --------------- Initial SCREEN ------------------
    $("#divContractBasicInfo").SetViewMode(false);
    $("#divInstallationInfo").SetViewMode(false);
    $("#divInstallationInstrumentInfo").SetViewMode(false);
    $("#divRegisterComplete").SetViewMode(false);


    enabledGridEmail();
    enabledGridAttach();

    $("#ContractCodeProjectCode").attr("readonly", false);
    $("#btnRetrieveInstallation").attr("disabled", false);
    $("#btnClearInstallation").attr("disabled", false);

    //disabledInputControls();

    InitialCommandButton(0);
    $("#divInputContractCode").clearForm();
    $("#divContractBasicInfo").clearForm();
    $("#divInstallationInstrumentInfo").clearForm();
    $("#divInstallationInfo").clearForm();
    $("#divRegisterComplete").clearForm();
    //$("#divResultRegisSlip").clearForm();

    $("#divInputContractCode").show();
    $("#divContractBasicInfo").show();
    $("#divInstallationInstrumentInfo").show();
    $("#divInstallationInfo").show();
    $("#divRegisterComplete").show();
    //$("#divResultRegisSlip").show();  
    //--------------------------------------------------  
}

function retrieve_installation_click() {
    command_control.CommandControlMode(false);
    //InitialCommandButton(1);
    //master_event.LockWindow(true);
    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
    call_ajax_method_json("/Installation/ISS060_RetrieveData", obj,
        function (result, controls) {
            command_control.CommandControlMode(true);
            //master_event.LockWindow(false);
            if (controls != undefined) {


                $("#divInputContractCode").clearForm();
                $("#divContractBasicInfo").clearForm();
                $("#divInstallationInstrumentInfo").clearForm();
                $("#divInstallationInfo").clearForm();
                $("#divRegisterComplete").clearForm();
                //$("#divResultRegisSlip").clearForm();

                return;
            }
            else if (result != undefined) {

                $("#NewNormalInstallFee").attr("readonly", true);

                if (result.blnValidateContractError == false) {
                    InitialCommandButton(0);
                }


                $("#divInputContractCode").clearForm();
                $("#divContractBasicInfo").clearForm();
                $("#divInstallationInstrumentInfo").clearForm();
                $("#divInstallationInfo").clearForm();
                $("#divRegisterComplete").clearForm();
                //$("#divResultRegisSlip").clearForm();

                $("#ServiceTypeCode").val(result.ServiceTypeCode);
                if (result.do_TbtInstallationSlip != null) {
                    result.do_TbtInstallationSlip.SlipIssueDate = ConvertDateToTextFormat(result.do_TbtInstallationSlip.SlipIssueDate.replace('/Date(', '').replace(')/', '') * 1);
                    if (result.do_TbtInstallationSlip.ExpectedInstrumentArrivalDate != null) {
                        result.do_TbtInstallationSlip.ExpectedInstrumentArrivalDate = ConvertDateToTextFormat(result.do_TbtInstallationSlip.ExpectedInstrumentArrivalDate.replace('/Date(', '').replace(')/', '') * 1);
                    }

                }


                if (result.dtRentalContractBasic != null) {
                    result.dtRentalContractBasic.ContractCode = $("#ContractCodeProjectCode").val();
                    //                    $("#NewBillingInstallFee").val(result.do_TbtInstallationBasic.BillingInstallFee)
                    //                    $("#NewNormalInstallFee").val(result.do_TbtInstallationBasic.NormalInstallFee)
                    $("#OperationOfficeCode").val(result.dtRentalContractBasic.OperationOfficeCode)
                    $("#divContractBasicInfo").bindJSON(result.dtRentalContractBasic);
                    $("#divInstallationInfo").bindJSON(result.dtRentalContractBasic);
                    //$("#AdditionalStockOutOfficeCode").val(result.dtRentalContractBasic.OperationOfficeCode);
                    $("#SalesmanEN1").val(result.dtRentalContractBasic.Salesman1);
                    $("#SalesmanEN2").val(result.dtRentalContractBasic.Salesman2);
                    SetRegisterState(1);
                }
                else if (result.dtSale != null) {
                    result.dtSale.ContractCode = $("#ContractCodeProjectCode").val();

                    $("#OperationOfficeCode").val(result.dtSale.OperationOfficeCode)
                    $("#divContractBasicInfo").bindJSON(result.dtSale);
                    $("#divInstallationInfo").bindJSON(result.dtSale);
                    $("#DocAuditResult").val(result.dtSale.DocAuditResult);
                    //$("#AdditionalStockOutOfficeCode").val(result.dtSale.OperationOfficeCode);
                    $("#SalesmanEN1").val(result.dtSale.Salesman1);
                    $("#SalesmanEN2").val(result.dtSale.Salesman2);
                    SetRegisterState(1);
                }


                $("#divInstallationInfo").bindJSON(result.do_TbtInstallationBasic);
                $("#divContractBasicInfo").bindJSON(result.do_TbtInstallationSlip);
                $("#divInstallationInfo").bindJSON(result.do_TbtInstallationSlip);
                $("#divInstallationInstrumentInfo").bindJSON(result.do_TbtInstallationSlip);
                $("#ContractCode").val(result.ContractCodeShort);


                $("#ContractCode").val(result.ContractProjectCodeForShow);
                //$("#ContractCode").val(result.ContractCodeShort);

                $("#NewApproveNo1").val($("#ApproveNo1").val());
                $("#NewApproveNo2").val($("#ApproveNo2").val());
                //var AmountAddInstallQty = 0;
                if (result.do_TbtInstallationSlip != null) {
                    // 20170217 nakajima modify start
                    if (result.do_TbtInstallationSlip.BillingInstallFeeCurrencyType == '2') {
                        $("#NewBillingInstallFee").val(result.do_TbtInstallationSlip.BillingInstallFeeUsd)
                    } else {
                        $("#NewBillingInstallFee").val(result.do_TbtInstallationSlip.BillingInstallFee)
                    }

                    if (result.do_TbtInstallationSlip.NormalInstallFeeCurrencyType == '2') {
                        $("#NewNormalInstallFee").val(result.do_TbtInstallationSlip.NormalInstallFeeUsd)
                    } else {
                        $("#NewNormalInstallFee").val(result.do_TbtInstallationSlip.NormalInstallFee)
                    }

                    onRetrieveNormalInstallFee = moneyConvert($("#NewBillingInstallFee").val());
                    // 20170217 nakajima modify end

                    $("#SlipIssueOfficeCode").val(result.do_TbtInstallationSlip.SlipIssueOfficeName);
                    //                    if (result.do_TbtInstallationSlip.SlipStatus == C_SLIP_STATUS_NOT_STOCK_OUT || result.do_TbtInstallationSlip.SlipStatus == C_SLIP_STATUS_PARTIAL_STOCK_OUT) {
                    //                        AmountAddInstallQty = result.do_TbtInstallationSlipDetails[i].AddInstalledQty;
                    //                    }
                    $("#SlipStatusName").val(result.LastSlipStatusName);

                    if (result.do_TbtInstallationSlip.ChangeReasonCode == C_INSTALL_CHANGE_REASON_SECOM) {
                        //$("#ChangeCustomerReason").attr("checked", false);
                        $("#ChangeSecomReason").attr("checked", true);
                    }
                    else {
                        $("#ChangeCustomerReason").attr("checked", true);
                        //$("#ChangeSecomReason").attr("checked", false);
                    }

                    if (result.do_TbtInstallationSlip.InstallationTypeRentalName != null) {
                        $("#InstallationType").val(result.do_TbtInstallationSlip.InstallationTypeRentalName);
                    }
                    else if (result.do_TbtInstallationSlip.InstallationTypeSaleName != null) {
                        $("#InstallationType").val(result.do_TbtInstallationSlip.InstallationTypeSaleName);
                    }
                    //---------------------------------------                    
                    if (result.do_TbtInstallationSlip.CauseReasonCustomerName != null) {
                        $("#CauseReason").val(result.do_TbtInstallationSlip.CauseReasonCustomerName);
                    }
                    else if (result.do_TbtInstallationSlip.CauseReasonSecomName != null) {
                        $("#CauseReason").val(result.do_TbtInstallationSlip.CauseReasonSecomName);
                    }
                    //---------------------------------------                   
                    $("#InstallFeeBillingType").val(result.do_TbtInstallationSlip.InstallFeeBillingTypeName);
                    $("#NewInstallFeeBillingType").val(result.do_TbtInstallationSlip.InstallFeeBillingType);
                    if ($("#NewInstallFeeBillingType").val() != "")
                        $("#NewInstallFeeBillingName").val($("#NewInstallFeeBillingType option:selected").text());
                    //----------------------------------------
                    obj = {
                        ValueCode: result.do_TbtInstallationSlip.SlipIssueOfficeCode
                    };
                    call_ajax_method('/Installation/ISS060_GetOfficeNameByCode', obj, function (result, controls) {
                        if (result != null) {
                            $("#SlipIssueOfficeCode").val(result);

                        }
                    });
                    //----------------------------------------

                    //======================================================
                }
                if (result.do_TbtInstallationBasic != null && result.do_TbtInstallationBasic != undefined) {

                    $("#Memo").val(result.Memo);
                    $("#InstallationMANo").val(result.do_TbtInstallationBasic.MaintenanceNo);
                    $("#InstallationBy").val(result.do_TbtInstallationBasic.InstallationByName);

                    $("#InstallationTypeCode").val(result.do_TbtInstallationBasic.InstallationType);

                    //================== TRS 24/05/2012 NEW SPEC =======================
                    //                    if (result.do_TbtInstallationBasic.InstallationType == C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                    //                        || result.do_TbtInstallationBasic.InstallationType == C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                    //                        || result.do_TbtInstallationBasic.InstallationType == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                    //                    ) {
                    //                        if (result.ServiceTypeCode == C_SERVICE_TYPE_RENTAL) {
                    //                            $("#AdditionalStockOutOfficeCode").val(result.dtRentalContractBasic.OperationOfficeCode);
                    //                        }
                    //                        else if (result.ServiceTypeCode == C_SERVICE_TYPE_SALE) {
                    //                            $("#AdditionalStockOutOfficeCode").val(result.dtSale.OperationOfficeCode);
                    //                        }
                    //                        if ($("#NewInstallFeeBillingType").val() != "")
                    //                            $("#AdditionalStockOutOfficeName").val($("#AdditionalStockOutOfficeCode option:selected").text());
                    //                    }
                    //                    else {
                    //                        $("#AdditionalStockOutOfficeCode").val("");
                    //                    }
                    //================== Phoomsak L. 19/06/2012 NEW SPEC =======================
                    $("#AdditionalStockOutOfficeCode").val("");
                    //==================================================================
                }
                /////////////// BIND EMAIl DATA //////////////////
                if (result.ListDOEmail != null) {
                    if (result.ListDOEmail.length > 0) {
                        for (var i = 0; i < result.ListDOEmail.length; i++) {
                            var emailList = [result.ListDOEmail[i].EmailAddress];

                            CheckFirstRowIsEmpty(ISS060_GridEmail, true);
                            AddNewRow(ISS060_GridEmail, emailList);

                        }
                    }
                }
                else {
                    DeleteAllRow(ISS060_GridEmail);
                }
                //////////////////////////////////////////////////

                /////////////// BIND Instrument DATA //////////////////
                if (result.do_TbtInstallationSlipDetails != null) {
                    if (result.do_TbtInstallationSlipDetails.length > 0) {
                        for (var i = 0; i < result.do_TbtInstallationSlipDetails.length; i++) {

                            //var InstrumentList = [result.do_TbtInstallationSlipDetails[i].InstrumentCode, result.arrayInstrumentName[i], result.do_TbtInstallationSlipDetails[i].ContractInstalledQty, result.do_TbtInstallationSlipDetails[i].TotalStockOutQty, AmountAddInstallQty + "", result.do_TbtInstallationSlipDetails[i].ReturnQty + "", result.do_TbtInstallationSlipDetails[i].AddRemovedQty + "", result.do_TbtInstallationSlipDetails[i].ContractInstalledQty + result.do_TbtInstallationSlipDetails[i].AddInstalledQty - result.do_TbtInstallationSlipDetails[i].AddRemovedQty + result.do_TbtInstallationSlipDetails[i].TotalStockOutQty, result.do_TbtInstallationSlipDetails[i].MoveQty + "", result.do_TbtInstallationSlipDetails[i].MAExchangeQty + "", "", "", "", "", "", result.do_TbtInstallationSlipDetails[i].PartialStockOutQty];
                            var InstrumentList = [//result.do_TbtInstallationSlipDetails[i].InstrumentCode
                                                    ConvertBlockHtml(result.do_TbtInstallationSlipDetails[i].InstrumentCode) //Modify by Jutarat A. on 28112013

                                                , result.do_TbtInstallationSlipDetails[i].ContractInstalledQty
                                                , result.do_TbtInstallationSlipDetails[i].TotalStockOutQty
                                                , result.do_TbtInstallationSlipDetails[i].ReturnQty
                                                , "0"
                                                , ((result.do_TbtInstallationSlipDetails[i].ContractInstalledQty * 1) + (result.do_TbtInstallationSlipDetails[i].TotalStockOutQty * 1) - (result.do_TbtInstallationSlipDetails[i].ReturnQty * 1) - (result.do_TbtInstallationSlipDetails[i].AddRemovedQty * 1)) + ""
                                                , result.do_TbtInstallationSlipDetails[i].AddRemovedQty
                                                , result.do_TbtInstallationSlipDetails[i].MoveQty + ""
                                                , result.do_TbtInstallationSlipDetails[i].MAExchangeQty + ""
                                                , "0", "0"
                                                , result.do_TbtInstallationSlipDetails[i].MoveQty + ""
                                                , result.do_TbtInstallationSlipDetails[i].MAExchangeQty + ""
                                                , result.do_TbtInstallationSlipDetails[i].ParentCode + ""
                                                , result.do_TbtInstallationSlipDetails[i].IsUnremovable + ""
                                                , result.do_TbtInstallationSlipDetails[i].IsParent + ""

                            //, result.do_TbtInstallationSlipDetails[i].InstrumentCode + ""
                                                , ConvertBlockHtml(result.do_TbtInstallationSlipDetails[i].InstrumentCode) + "" //Modify by Jutarat A. on 28112013

                                                , result.do_TbtInstallationSlipDetails[i].ReturnQty
                                                , "0"
                                                , result.do_TbtInstallationSlipDetails[i].AddRemovedQty
                                                , "0"
                                                , ((result.do_TbtInstallationSlipDetails[i].ContractInstalledQty * 1) + (result.do_TbtInstallationSlipDetails[i].TotalStockOutQty * 1) - (result.do_TbtInstallationSlipDetails[i].ReturnQty * 1) - (result.do_TbtInstallationSlipDetails[i].AddRemovedQty * 1)) + ""
                                                , result.do_TbtInstallationSlipDetails[i].ContractInstalledQty
                                                , result.do_TbtInstallationSlipDetails[i].TotalStockOutQty
                                                , result.do_TbtInstallationSlipDetails[i].InstrumentPrice
                                                ];
                            CheckFirstRowIsEmpty(ISS060_GridInstrumentInfo, true);
                            AddNewRow(ISS060_GridInstrumentInfo, InstrumentList);
                        }
                    }
                }
                else {
                    DeleteAllRow(ISS060_GridInstrumentInfo);
                }
                //////////////////////////////////////////////////



                $("#UnremoveApproveNo").attr("readonly", true);
                initialScreenOnRetrieve(result);

                //======================== INITIAL VALUE =========================================
                $("#m_blnbFirstTimeRegister").val(result.m_blnbFirstTimeRegister);

                //$("#NewNormalContractFee").val($("#NormalContractFee").val());
                //$("#OldNormalContractFee").val($("#NormalContractFee").val());
                if (result.dtRentalContractBasic != null) {
                    $("#ChangeType").val(result.dtRentalContractBasic.ChangeType);
                }
                if (result.do_TbtInstallationSlip != null) {
                    $("#SlipType").val(result.do_TbtInstallationSlip.SlipType);
                }
                //================================================================================

                setTimeout("manualInitialGridInstrument(null)", 1500);

                if (result.InstallType != undefined) {

                    if (result.dtRentalContractBasic != null) {
                        initialInstrumentDetail(result.do_TbtInstallationBasic.InstallationType, result.dtRentalContractBasic.ContractStatus);
                    }
                    else {
                        initialInstrumentDetail(result.do_TbtInstallationBasic.InstallationType, null);
                    }
                    if (result.do_TbtInstallationSlip != null) {
                        initialInstallationFeeBilling(result.do_TbtInstallationBasic.InstallationType, result.do_TbtInstallationSlip.InstallFeeBillingType);
                    }
                    else {
                        initialInstallationFeeBilling(result.do_TbtInstallationBasic.InstallationType, null);
                    }
                }

                if ($("#NewBldMgmtCost").NumericValue() != "")
                    $("#NewBldMgmtCost").val(moneyConvert($("#NewBldMgmtCost").val()));

                $("#BillingInstallFee").SetNumericCurrency(result.do_TbtInstallationSlip.BillingInstallFeeCurrencyType);
                if ($("#BillingInstallFee").NumericValue() != "")
                    $("#BillingInstallFee").val(moneyConvert($("#BillingInstallFee").val()));

                $("#NormalInstallFee").SetNumericCurrency(result.do_TbtInstallationSlip.NormalInstallFeeCurrencyType);
                if ($("#NormalInstallFee").NumericValue() != "")
                    $("#NormalInstallFee").val(moneyConvert($("#NormalInstallFee").val()));

                // 20170217 nakajima add start
                $("#NewBillingInstallFee").SetNumericCurrency(result.do_TbtInstallationSlip.BillingInstallFeeCurrencyType);
                if ($("#NewBillingInstallFee").NumericValue() != "")
                    $("#NewBillingInstallFee").val(moneyConvert($("#NewBillingInstallFee").val()));

                $("#NewNormalInstallFee").SetNumericCurrency(result.do_TbtInstallationSlip.NormalContractFeeCurrencyType);
                if ($("#NewNormalInstallFee").NumericValue() != "")
                    $("#NewNormalInstallFee").val(moneyConvert($("#NewNormalInstallFee").val()));
                // 20170217 nakajima add end
                $("#NormalContractFee").SetNumericCurrency(result.do_TbtInstallationSlip.NormalContractFeeCurrencyType);
                if ($("#NormalContractFee").NumericValue() != "")
                    $("#NormalContractFee").val(moneyConvert($("#NormalContractFee").val()));

                $("#NewNormalContractFee").SetNumericCurrency(result.do_TbtInstallationSlip.NormalContractFeeCurrencyType);
                $("#NewNormalContractFee").val(moneyConvert($("#NormalContractFee").val()));

                if ($("#NewSECOMPaymentFee").NumericValue() != "") {
                    //$("#NewSECOMPaymentFee").val(moneyConvert($("#NewSECOMPaymentFee").val()));
                    var tmpPaymentFee = Math.round($("#NewSECOMPaymentFee").NumericValue() * Math.pow(10, 2)) / Math.pow(10, 2);
                    $("#NewSECOMPaymentFee").val(moneyConvert(tmpPaymentFee));
                }

                if ($("#NewSECOMRevenueFee").NumericValue() != "") {
                    var tmpRevenueFee = Math.round($("#NewSECOMRevenueFee").NumericValue() * Math.pow(10, 2)) / Math.pow(10, 2);
                    $("#NewSECOMRevenueFee").val(moneyConvert(tmpRevenueFee));
                }

                $("#SECOMPaymentFee").SetNumericCurrency("1");
                if ($("#SECOMPaymentFee").NumericValue() != "") {
                    //$("#SECOMPaymentFee").val(moneyConvert($("#SECOMPaymentFee").val()));
                    var tmpPaymentFee = Math.round($("#SECOMPaymentFee").NumericValue() * Math.pow(10, 2)) / Math.pow(10, 2);
                    $("#SECOMPaymentFee").val(moneyConvert(tmpPaymentFee));
                }

                $("#SECOMRevenueFee").SetNumericCurrency("1");
                if ($("#SECOMRevenueFee").NumericValue() != "") {
                    //$("#SECOMRevenueFee").val(moneyConvert($("#SECOMRevenueFee").val()));
                    var tmpRevenueFee = Math.round($("#SECOMRevenueFee").NumericValue() * Math.pow(10, 2)) / Math.pow(10, 2);
                    $("#SECOMRevenueFee").val(moneyConvert(tmpRevenueFee));
                }

            }
            else {

                setInitialState();
            }
        }
    );
    $("#ChangeCustomerReason").attr("disabled", true);
    $("#ChangeSecomReason").attr("disabled", true);
}






function clearAllScreen() {
    setInitialState();
    btnClearEmailClick();
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
        SetResetCommand(true, command_reset_click);
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
    backCalculateGridInstrumentForRegisState();
    InitialCommandButton(1);
    $("#divContractBasicInfo").SetViewMode(false);
    $("#divProjectInfo").SetViewMode(false);
    $("#divInstallationInfo").SetViewMode(false);
    $("#divInstallationMANo").SetViewMode(false);
    $("#divInstallationInstrumentInfo").SetViewMode(false);
    $("#divRegisterComplete").SetViewMode(false);
    enabledGridEmail();
    enabledGridAttach();
    //$("#NormalContractFee").val($("#OldNormalContractFee").val());
    $("#ChangeCustomerReason").attr("disabled", true);
    $("#ChangeSecomReason").attr("disabled", true);

    $("#NewNormalContractFee").val(moneyConvert($("#NormalContractFee").val()));
}

function command_register_click() {
    //enableInputControls();
    command_control.CommandControlMode(false);
    SendGridSlipDetailsToObject(registerDataAjax);
}

function registerDataAjax() {
    //var obj = CreateObjectData($("#form1").serialize() + "&Memo=" + $("#NewMemo").val() + "&ChangeContents=" + $("#ChangeContents").val() + "&ExpectedInstrumentArrivalDate=" + $("#ExpectedInstrumentArrivalDate").val() + "&StockOutTypeCode=" + $("#StockOutTypeCode").val() + "&chkHaveUnremove=" + $("#chkCondHave2").prop("checked") + "&UnremoveApproveNo=" + $("#UnremoveApproveNo").val() + "&chkHaveAdditional=" + $("#chkCondHave5").prop("checked") + "&AdditionalStockOutOfficeCode=" + $("#AdditionalStockOutOfficeCode").val() + "&blnReadOnlyBillingInstallFee=" + $("#NewBillingInstallFee").prop("readonly") + "&NormalContractFee=" + $("#NormalContractFee").val() + "&NormalInstallFee=" + $("#NormalInstallFee").val());
	var obj = CreateObjectData($("#form1").serialize());
	obj.Memo = $("#NewMemo").val();
	obj.ChangeContents = $("#ChangeContents").val();
	obj.ExpectedInstrumentArrivalDate = $("#ExpectedInstrumentArrivalDate").val();
	obj.StockOutTypeCode = $("#StockOutTypeCode").val();
	obj.chkHaveUnremove = $("#chkCondHave2").prop("checked");
	obj.UnremoveApproveNo = $("#UnremoveApproveNo").val();
	obj.chkHaveAdditional = $("#chkCondHave5").prop("checked");
	obj.AdditionalStockOutOfficeCode = $("#AdditionalStockOutOfficeCode").val();
	obj.blnReadOnlyBillingInstallFee = $("#NewBillingInstallFee").prop("readonly");
	obj.NormalContractFeeCurrencyType = $("#NormalContractFee").NumericCurrencyValue();
	if (obj.NormalContractFeeCurrencyType == C_CURRENCY_LOCAL)
	{
	obj.NormalContractFee = $("#NormalContractFee").NumericValue();
	    obj.NormalContractFeeUsd = null;
	}
	else
	{
	    obj.NormalContractFee = null;
	    obj.NormalContractFeeUsd = $("#NormalContractFee").NumericValue();
	}
	obj.NormalInstallFeeCurrencyType = $("#NormalInstallFee").NumericCurrencyValue();
	if (obj.NormalInstallFeeCurrencyType == C_CURRENCY_LOCAL) {
	obj.NormalInstallFee = $("#NormalInstallFee").NumericValue();
	    obj.NormalInstallFeeUsd = null;
	}
	else {
	    obj.NormalInstallFee = null;
	    obj.NormalInstallFeeUsd = $("#NormalInstallFee").NumericValue();
	}
	
    var blnChkInstrument = ValidateInstrumentDetail();
    if (blnChkInstrument) {
        call_ajax_method_json("/Installation/ISS060_ValidateBeforeRegister", obj, function (result, controls) {

            if (controls != undefined) {

                for (var i = 0; i < controls.length; i++) {
                    var txt = controls[i];
                    var search = txt.search(/LineInstrumentCode/i);
                    if (search >= 0) {
                        var InstrumentCodeCol = ISS060_GridInstrumentInfo.getColIndexById("InstrumentCode");
                        for (var i = 0; i < ISS060_GridInstrumentInfo.getRowsNum(); i++) {
                            var InstrumentCode = ISS060_GridInstrumentInfo.cells2(i, InstrumentCodeCol).getValue()
                            var rowId = ISS060_GridInstrumentInfo.getRowId(i);
                            if (InstrumentCode == txt.replace("LineInstrumentCode", "")) {
                                ISS060_GridInstrumentInfo.selectRow(ISS060_GridInstrumentInfo.getRowIndex(rowId));
                            }
                        }
                    }
                }
                /* --- Higlight Text --- */
                /* --------------------- */
                VaridateCtrl(["NewApproveNo1", "AdditionalStockOutOfficeCode", "UnremoveApproveNo", "InstallStartDate", "InstallFinishDate", "InstallCompleteDate", "NewNormalInstallFee", "NewBillingInstallFee", "NewBillingOCC"], controls);
                /* --------------------- */

                return;
            }
            else if (result == null) {

            }
            else {

                if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL && ($("#chkCondHave1").prop("checked") == true || $("#chkCondHave3").prop("checked") == true)) {
                    CalculateNormalContractFee();
                } else {
                    tempCalNormalContractFee = $("#NormalContractFee").NumericValue();
                }

                validateWarningData();
            }

            setTimeout("command_control.CommandControlMode(true);", 1000);
        });
    }
    else {
        setTimeout("command_control.CommandControlMode(true);", 1000);
    }
}

function validateWarningData() {
    var strServiceTypeCode = $("#ServiceTypeCode").val();
    var strInstallationType = $("#InstallationTypeCode").val();
    var strDocAuditResult = $("#DocAuditResult").val();
    if ((strServiceTypeCode = C_SERVICE_TYPE_SALE) &&
         (strInstallationType == C_SALE_INSTALL_TYPE_NEW || strInstallationType == C_SALE_INSTALL_TYPE_ADD) &&
         (strDocAuditResult == "" || strDocAuditResult == C_DOC_AUDIT_RESULT_NOT_RECEIVED || strDocAuditResult == C_DOC_AUDIT_RESULT_NEED_FOLLOW || strDocAuditResult == C_DOC_AUDIT_RESULT_RETURNED)) {
        showYesNoDialog5049();
    }
    else if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL && tempCalNormalContractFee != $("#NormalContractFee").NumericValue()) {
        showYesNoDialog5114();
    }
    else if ($("#NewNormalInstallFee").val() != $("#NormalInstallFee").val()) {
        showYesNoDialog5115();
    }
    else {
        setConfirmState();
    }

}

function showYesNoDialog5049() {
    var obj = {
        module: "Installation",
        code: "MSG5049"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
			function () {
			    if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL && tempCalNormalContractFee != $("#NormalContractFee").NumericValue()) {
			        showYesNoDialog5114();
			    }
			    else if ($("#NewNormalInstallFee").val() != $("#NormalInstallFee").val()) {
			        showYesNoDialog5115();
			    }
			    else {
			        setConfirmState();
			    }
			});
    });
}

function showYesNoDialog5114() {
    var obj = {
        module: "Installation",
        code: "MSG5114"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
			function () {
			    if ($("#NewNormalInstallFee").val() != $("#NormalInstallFee").val()) {
			        showYesNoDialog5115();
			    } else {
			        setConfirmState();
			    }
			});
    });
}

function showYesNoDialog5115() {
    var obj = {
        module: "Installation",
        code: "MSG5115"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
			function () {
			    setConfirmState();
			});
    });
}


function setConfirmState() {
    $("#NewNormalContractFee").val(tempCalNormalContractFee);
    if ($("#NewNormalContractFee").NumericValue() != "")
        $("#NewNormalContractFee").val(moneyConvert($("#NewNormalContractFee").val()));

    calculateGridInstrumentForConfirmState();

    SendGridSlipDetailsToObject();
    InitialCommandButton(2);

    $("#divContractBasicInfo").SetViewMode(true);
    $("#divProjectInfo").SetViewMode(true);
    $("#divInstallationInfo").SetViewMode(true);
    $("#divInstallationMANo").SetViewMode(true);
    $("#divInstallationInstrumentInfo").SetViewMode(true);
    $("#divRegisterComplete").SetViewMode(true);
    disabledGridEmail();
    disabledGridAttach();
}

function setSuccessRegisState(strServiceTypeCode) {
    // --------------- Initial SCREEN ------------------
    $("#divContractBasicInfo").SetViewMode(true);
    $("#divProjectInfo").SetViewMode(true);
    $("#divInstallationInfo").SetViewMode(true);
    $("#divInstallationMANo").SetViewMode(true);
    $("#divInstallationInstrumentInfo").SetViewMode(true);
    $("#divRegisterComplete").SetViewMode(true);
    disabledGridEmail();
    disabledGridAttach();

    $("#divInstallationMANo").show();
    $("#divResultRegisSlip").show();
    //    $("#ContractCodeProjectCode").attr("disabled", true);
    //    $("#btnRetrieveInstallation").attr("disabled", true);
    //    $("#btnClearInstallation").attr("disabled", false);

    //disabledInputControls();

    InitialCommandButton(0);
    doInformationAlert("Common", "MSG0046", "");

    // Report no need to issue in this screen 2012-06-11 Phoomsak L.
    //    printReport(strServiceTypeCode, $("#ContractCodeProjectCode").val());
}

function command_confirm_click() {
    command_control.CommandControlMode(false);

    //    $("#divContractBasicInfo").SetViewMode(false);
    //    $("#divProjectInfo").SetViewMode(false);
    //    $("#divInstallationInfo").SetViewMode(false);
    //    $("#divInstallationMANo").SetViewMode(false);
    //    $("#divInstallationInstrumentInfo").SetViewMode(false);
    //    $("#divRegisterComplete").SetViewMode(false);
    //    enabledGridEmail();

    //enableInputControls();

    //var obj = CreateObjectData($("#form1").serialize() + "&Memo=" + $("#NewMemo").val() + "&ChangeContents=" + $("#ChangeContents").val() + "&ExpectedInstrumentArrivalDate=" + $("#ExpectedInstrumentArrivalDate").val() + "&StockOutTypeCode=" + $("#StockOutTypeCode").val() + "&UnremoveApproveNo=" + $("#UnremoveApproveNo").val());
	var obj = $("#formSearch").serializeObject2();
	obj.Memo = $("#NewMemo").val();
	obj.ChangeContents = $("#ChangeContents").val();
	obj.ExpectedInstrumentArrivalDate = $("#ExpectedInstrumentArrivalDate").val();
	obj.StockOutTypeCode = $("#StockOutTypeCode").val();
	obj.UnremoveApproveNo = $("#UnremoveApproveNo").val();

	obj.NormalContractFeeCurrencyTpe = $("#NormalContractFee").NumericCurrencyValue();
	if (obj.NormalContractFeeCurrencyTpe == C_CURRENCY_LOCAL) {
	    obj.NormalContractFee = obj.NormalContractFee;
	    obj.NormalContractFeeUsd = null;
	}
	else {
	    obj.NormalContractFeeUsd = obj.NormalContractFee;
	    obj.NormalContractFee = null;
	}
	
    master_event.LockWindow(true);
    call_ajax_method_json("/Installation/ISS060_RegisterData", obj, function (result, controls) {
        command_control.CommandControlMode(true);
        master_event.LockWindow(false);
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["InstallationType", "CustomerStaffBelonging", "CustomerStaffName", "CustomerStaffPhoneNo"], controls);
            /* --------------------- */

            return;
        }
        else if (result != undefined && result != null) {
            /* --- Set View Mode --- */
            /* --------------------- */
            $("#InstallationSlipNo").val(result.SlipNo);
            setSuccessRegisState(result.ServiceTypeCode)

            /* -------------------------- */
            /////////////////////////// PRINT REPORT AFTER SUCCESS //////////////////////////////
            //************************ COMMENT FOR ADD REPORT
            //window.open("ISS060_QuotationForCancelContractMemorandum");
            //************************
            ////////////////////////////////////////////////////////////////////////////////////
        }
        else {
            setConfirmState();
        }
    });


}

function command_reset_click() {
    command_control.CommandControlMode(false);
    //    setInitialState();
    //    if ($("#ContractCodeProjectCode").val() != "") {       
    //        setTimeout("retrieve_installation_click()", 2000);
    //    }


    var obj = {
        module: "Common",
        code: "MSG0038"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    command_control.CommandControlMode(true);
                    setInitialState();
                    DeleteAllRow(ISS060_GridInstrumentInfo);
                    DeleteAllRow(ISS060_GridEmail);
                    if ($("#ContractCodeProjectCode").val() != "") {
                        setTimeout("retrieve_installation_click()", 2000);
                    }
                    ClearAllAttachFile();
                }, function () { command_control.CommandControlMode(true); });
    });
}

function SetRegisterState(cond) {

    InitialCommandButton(1);

    //enableInputControls();

    //    $("#ContractCodeProjectCode").attr("disabled", true);
    //    $("#btnRetrieveInstallation").attr("disabled", true);
    //    $("#btnClearInstallation").attr("disabled", false);
    if (cond == 1) {
        $("#InstallationType").attr("disabled", false);
        $("#divContractBasicInfo").show();
        $("#divProjectInfo").hide();
        $("#divInstallationInfo").show();
        $("#divInstallationMANo").hide();
    }
    else if (cond == 2) {
        $("#InstallationType").attr("disabled", true);
        $("#divContractBasicInfo").hide();
        $("#divProjectInfo").show();
        $("#divInstallationInfo").show();
        $("#divInstallationMANo").hide();
    }

}







function initialGridOnload() {
    // intial grid
    ISS060_GridEmail = $("#gridEmail").InitialGrid(pageRow, false, "/Installation/ISS060_InitialGridEmail");
    ISS060_GridInstrumentInfo = $("#gridInstrumentInfo").InitialGrid(pageRow, false, "/Installation/ISS060_InitialGridInstrumentInfo");

    //DeleteAllRow(ISS060_GridInstrumentInfo);

    /* ===== binding event when finish load data ===== */
    BindOnLoadedEvent(ISS060_GridInstrumentInfo, function () {
        manualInitialGridInstrument(null);



    });

    ISS060_GridInstrumentInfo.attachEvent("onRowSelect", function (id, ind) {
        var row_num = ISS060_GridInstrumentInfo.getRowIndex(id);
        if (ISS060_GridInstrumentInfo.cell.childNodes[0].tagName == "INPUT") {
            var txt = ISS060_GridInstrumentInfo.cell.childNodes[0];

            if (txt.disabled == false) {
                txt.focus();
                //txt.select();
            }
        }
    });

}





function disabledGridEmail() {
    //////// DISABLED BUTTON In EMAIL GRID ///////////
    for (var i = 0; i < ISS060_GridEmail.getRowsNum(); i++) {
        var row_id = ISS060_GridEmail.getRowId(i);
        EnableGridButton(ISS060_GridEmail, "btnRemoveEmail", row_id, "ButtonRemove", false);
    }
    //////////////////////////////////////////////////
}

function enabledGridEmail() {
    //////// ENABLED BUTTON In EMAIL GRID ///////////
    for (var i = 0; i < ISS060_GridEmail.getRowsNum(); i++) {
        var row_id = ISS060_GridEmail.getRowId(i);
        EnableGridButton(ISS060_GridEmail, "btnRemoveEmail", row_id, "ButtonRemove", true);
    }
    //////////////////////////////////////////////////
}



function ClearInstrumentInfo() {
    DeleteAllRow(ISS060_GridInstrumentInfo);
    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
    call_ajax_method_json("/Installation/ISS060_ClearInstrumentInfo", obj, function (result, controls) {


    });

}

function printReport(strServiceTypeCode, strContractCodeProjectCode) {
    var key = ajax_method.GetKeyURL(null);
    if (strServiceTypeCode == C_SERVICE_TYPE_RENTAL) {
        //        var obj = { strContractProjectCode: $("#ContractCodeProjectCode").val() };
        //        call_ajax_method_json("/Installation/ISS060_PrintISR110", obj, function (result, controls) {
        //        });
        var link = ajax_method.GenerateURL("/Installation/ISS060_PrintISR110" + "?strContractProjectCode=" + strContractCodeProjectCode + "&k=" + key);
        window.open(link, "download1");
    }
    else if (strServiceTypeCode == C_SERVICE_TYPE_SALE) {
        //        var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
        //        call_ajax_method_json("/Installation/ISS060_PrintISR090", obj, function (result, controls) {
        //        });
        var link = ajax_method.GenerateURL("/Installation/ISS060_PrintISR090" + "?strContractCode=" + strContractCodeProjectCode + "&k=" + key);
        window.open(link, "download1");
    }
}

function SendGridSlipDetailsToObject(funcName) {
    manualInitialGridInstrument(null);

    if (ISS060_GridInstrumentInfo.hdr.rows.length > 0) {
        var objArray = new Array();

        if (CheckFirstRowIsEmpty(ISS060_GridInstrumentInfo) == false) {
            for (var i = 0; i < ISS060_GridInstrumentInfo.getRowsNum(); i++) {
                var rowId = ISS060_GridInstrumentInfo.getRowId(i);
                //================================= GetColumn Index =================================
                var InstrumentCodeCol = ISS060_GridInstrumentInfo.getColIndexById("InstrumentCode");
                var ContractAfterChangeCol = ISS060_GridInstrumentInfo.getColIndexById("ContractInstalledAfterChange");
                var ContractCurrentCol = ISS060_GridInstrumentInfo.getColIndexById("ContractInstalledQTY");
                var TotalStockCol = ISS060_GridInstrumentInfo.getColIndexById("TotalStockedOut");
                var UnUsedCol = ISS060_GridInstrumentInfo.getColIndexById("UnusedQTY");
                var RemovedCol = ISS060_GridInstrumentInfo.getColIndexById("RemovedQTY");
                var AddRemovedCol = ISS060_GridInstrumentInfo.getColIndexById("AddRemovedQTY");
                var MovedCol = ISS060_GridInstrumentInfo.getColIndexById("MovedQTY");
                var MAExchangeCol = ISS060_GridInstrumentInfo.getColIndexById("MAExchangeQTY");
                var NotInstallCol = ISS060_GridInstrumentInfo.getColIndexById("NotInstallQTY");
                var UnremovableCol = ISS060_GridInstrumentInfo.getColIndexById("UnremovableQTY");

                var ParentCodeCol = ISS060_GridInstrumentInfo.getColIndexById("ParentCode");
                var IsUnremovableCol = ISS060_GridInstrumentInfo.getColIndexById("IsUnremovable");
                var IsParentCol = ISS060_GridInstrumentInfo.getColIndexById("IsParent");
                var HiddenInstrumentCodeCol = ISS060_GridInstrumentInfo.getColIndexById("HiddenInstrumentCode");

                var ParentCodeValue = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, ParentCodeCol);
                var IsParentValue = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, IsParentCol);
                var IsUnremovableValue = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, IsUnremovableCol);
                var HiddenInstrumentCodeValue = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, HiddenInstrumentCodeCol);

                var AddRemovedQTYid = GenerateGridControlID("AddRemovedQTYBox", rowId);
                var MovedQTYid = GenerateGridControlID("MovedQTYBox", rowId);
                var MAExchangeQTYid = GenerateGridControlID("MAExchangeQTYBox", rowId);
                var NotInstallQTYid = GenerateGridControlID("NotInstallQTYBox", rowId);
                var UnremovableQTYid = GenerateGridControlID("UnremovableQTYBox", rowId);

                var amountAddRemovedQTY = 0;
                var amountMovedQTY = 0;
                var amountMAExchangeQTY = 0;
                var amountNotInstallQTY = 0;
                var amountUnremovableQTY = 0;

                //====================================================================================

                var amountRemove = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, RemovedCol);

                //=========== AddRemovedQTY ==============================
                if (ISS060_GridInstrumentInfo.hdr.rows.length > 0) {
                    var val = $("#" + AddRemovedQTYid).val();
                    if (val == undefined) {

                        val = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, AddRemovedCol);
                    }
                    amountAddRemovedQTY = val.replace(/,/g, "");
                }
                //========================================================
                //=========== MovedQTY ==============================
                if (ISS060_GridInstrumentInfo.hdr.rows.length > 0) {
                    var val = $("#" + MovedQTYid).val();
                    if (val == undefined) {
                        val = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, MovedCol);
                    }
                    amountMovedQTY = val.replace(/,/g, "");
                }
                //========================================================
                //=========== MAExchangeQTYid ==============================
                if (ISS060_GridInstrumentInfo.hdr.rows.length > 0) {
                    var val = $("#" + MAExchangeQTYid).val();
                    if (val == undefined) {
                        val = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, MAExchangeCol);
                    }
                    amountMAExchangeQTY = val.replace(/,/g, "");
                }
                //========================================================
                //=========== NotInstallQTY ==============================
                if (ISS060_GridInstrumentInfo.hdr.rows.length > 0) {
                    var val = $("#" + NotInstallQTYid).val();
                    if (val == undefined) {
                        val = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, NotInstallCol);
                    }
                    amountNotInstallQTY = val.replace(/,/g, "");
                }
                //========================================================
                //=========== UnremovableQTY ==============================
                if (ISS060_GridInstrumentInfo.hdr.rows.length > 0) {
                    var val = $("#" + UnremovableQTYid).val();
                    if (val == undefined) {
                        val = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, UnremovableCol);
                    }
                    amountUnremovableQTY = val.replace(/,/g, "");
                }
                //=======================================================


                var iobj = {
                    //InstrumentCode: ISS060_GridInstrumentInfo.cells2(i, InstrumentCodeCol).getValue()
                    InstrumentCode: HiddenInstrumentCodeValue
                , ContractInstallAfterChange: ISS060_GridInstrumentInfo.cells2(i, ContractAfterChangeCol).getValue().replace(/,/g, "")
                , ContractInstalledQty: ISS060_GridInstrumentInfo.cells2(i, ContractCurrentCol).getValue().replace(/,/g, "")
                , TotalStockOutQty: ISS060_GridInstrumentInfo.cells2(i, TotalStockCol).getValue().replace(/,/g, "")
                , ReturnQty: ISS060_GridInstrumentInfo.cells2(i, UnUsedCol).getValue().replace(/,/g, "")
                    //, AddRemovedQty: amountAddRemovedQTY
                , AddRemovedQty: amountRemove
                , MoveQty: amountMovedQTY
                , MAExchangeQty: amountMAExchangeQTY
                , NotinstalledQty: amountNotInstallQTY
                , UnremovableQty: amountUnremovableQTY
                , AddRemovedQTYTemp: amountAddRemovedQTY
                , ParentCode: ParentCodeValue
                , IsUnremovable: IsUnremovableValue
                , IsParent: IsParentValue
                };
                objArray.push(iobj);
            }
        }

        var obj = {
            do_TbtInstallationSlipDetails: objArray
            , ListInstrumentData: objArray
            , GridInstrumentForValid: objArray
        };
        /* --------------------- */

        /* --- Check and Add event --- */
        /* --------------------------- */
        command_control.CommandControlMode(false);
        call_ajax_method_json("/Installation/ISS060_SendGridSlipDetailsData", obj, function (result, controls) {

            if (controls != undefined) {
                /* --- Higlight Text --- */
                /* --------------------- */
                //VaridateCtrl(["EmailAddress"], controls);
                /* --------------------- */

                return;
            }
            else if (result != undefined) {

            }

            if (typeof (funcName) == "function")
                funcName();
        });
    }

}

function initialScreenOnRetrieve(result) {
    //        initialSaleman(result);
    //        initialChangeReason(result);
    //        initialInstallationType(result);
    //        initialInstallationCauseReason(result);
    //        initialNormalContractFee(result);
    //        initialNormalInstallationFee(result);
    initialSECOMNPayment();
    //        initialApproveNo(result)
}

function initialSECOMNPayment() {

    $("#SECOMPaymentFee").val("");
    $("#SECOMRevenueFee").val("");
    // 20170217 nakajima modify start
    //if ($("#BillingInstallFee").NumericValue() != "" && $("#NormalInstallFee").NumericValue() != "") {
    //    if ($("#BillingInstallFee").NumericValue() * 1 < $("#NormalInstallFee").NumericValue() * 1) {
    //        $("#SECOMPaymentFee").val((($("#NormalInstallFee").NumericValue() * 1) - ($("#BillingInstallFee").NumericValue() * 1)).toFixed(2));
    //        $("#SECOMPaymentFee").val(moneyConvert($("#SECOMPaymentFee").val()));
    //    }
    //    if ($("#BillingInstallFee").NumericValue() * 1 > $("#NormalInstallFee").NumericValue() * 1) {
    //        $("#SECOMRevenueFee").val((($("#BillingInstallFee").NumericValue() * 1) - ($("#NormalInstallFee").NumericValue() * 1)).toFixed(2));
    //        $("#SECOMRevenueFee").val(moneyConvert($("#SECOMRevenueFee").val()));
    //    }
    //}
    // 20170217 nakajima modify end
    $("#NewSECOMPaymentFee").val("");
    $("#NewSECOMRevenueFee").val("");
    // 20170217 nakajima modify start
    //if ($("#NewBillingInstallFee").NumericValue() != "" && $("#NewNormalInstallFee").NumericValue() != "") {
    //    if ($("#NewBillingInstallFee").NumericValue() * 1 < $("#NewNormalInstallFee").NumericValue() * 1) {
    //        $("#NewSECOMPaymentFee").val((($("#NewNormalInstallFee").NumericValue() * 1) - ($("#NewBillingInstallFee").NumericValue() * 1)).toFixed(2));
    //        $("#NewSECOMPaymentFee").val(moneyConvert($("#NewSECOMPaymentFee").val()));
    //    }
    //    if ($("#NewBillingInstallFee").NumericValue() * 1 > $("#NewNormalInstallFee").NumericValue() * 1) {
    //        $("#NewSECOMRevenueFee").val((($("#NewBillingInstallFee").NumericValue() * 1) - ($("#NewNormalInstallFee").NumericValue() * 1)).toFixed(2));
    //        $("#NewSECOMRevenueFee").val(moneyConvert($("#NewSECOMRevenueFee").val()));
    //    }
    //}
    // 20170217 nakajima modify end
}

function initialSaleman(result) {
    if (result.ServiceTypeCode == C_SERVICE_TYPE_RENTAL && result.dtRentalContractBasic.ContractStatus == C_CONTRACT_STATUS_BEF_START) {
        $("#SalesmanCode1").val(result.dtRentalContractBasic.SalesmanCode1);
        $("#SalesmanCode2").val(result.dtRentalContractBasic.SalesmanCode2);
        $("#SalesmanEN1").val(result.dtRentalContractBasic.SalesmanEN1);
        $("#SalesmanEN2").val(result.dtRentalContractBasic.SalesmanEN2);
    }
    else if (result.ServiceTypeCode == C_SERVICE_TYPE_SALE) {
        $("#SalesmanCode1").val(result.dtSale.SalesmanCode1);
        $("#SalesmanCode2").val(result.dtSale.SalesmanCode2);
        $("#SalesmanEN1").val(result.dtSale.SalesmanEN1);
        $("#SalesmanEN2").val(result.dtSale.SalesmanEN2);
    }
    else {
        $("#SalesmanCode1").val();
        $("#SalesmanCode2").val();
        $("#SalesmanEN1").val();
        $("#SalesmanEN2").val();
    }
}


function initialInstallationType(result) {
    if (result.do_TbtInstallationBasic != null) {
        if (result.do_TbtInstallationBasic.InstallationType != "") {
            $("#InstallationType").val(result.do_TbtInstallationBasic.InstallationType);
            $("#InstallationTypeCode").val(result.do_TbtInstallationBasic.InstallationType);
            $("#InstallationType").attr("disabled", true);
            $("#divInstallationInstrumentInfo").show();
        }
        else {
            $("#InstallationType").attr("disabled", false);
            $("#divInstallationInstrumentInfo").hide();
        }
    }
    else {
        $("#InstallationType").attr("disabled", false);
        $("#divInstallationInstrumentInfo").hide();
    }
}

function initialInstallationType2(InstallationType) {

    if (InstallationType != "") {
        $("#InstallationType").val(InstallationType);
        $("#InstallationType").attr("disabled", true);
    }
    else {
        $("#InstallationType").attr("disabled", false);
    }

}



function enableInputControls() {
    //########## ENABLED INPUT CONTROL #################
    $("#PlanCode").attr("disabled", false);
    $("#InstallationType").attr("disabled", false);
    $("#CauseReason").attr("disabled", false);
    $("#NormalInstallFee").attr("disabled", false);
    $("#InstallFeeBillingType").attr("disabled", false);
    $("#BillingInstallFee").attr("disabled", false);
    $("#BillingOCC").attr("disabled", false);
    $("#InstallationBy").attr("disabled", false);
    $("#SlipIssueDate").EnableDatePicker(true)
    $("#SlipIssueOfficeCode").attr("disabled", false);
    $("#ApproveNo1").attr("disabled", false);
    $("#ApproveNo2").attr("disabled", false);
    $("#EmailAddress").attr("disabled", false);
    $("#btnAdd").attr("disabled", false);
    $("#btnClear").attr("disabled", false);
    $("#btnSearchEmail").attr("disabled", false);
    $("#Memo").attr("disabled", false);
    $("#ChangeContents").attr("disabled", false);
    //####################################################
}

function disabledInputControls() {
    //########## DISABLED INPUT CONTROL #################   
    $("#PlanCode").attr("disabled", true);
    $("#InstallationType").attr("disabled", true);
    $("#CauseReason").attr("disabled", true);
    $("#NormalInstallFee").attr("disabled", true);
    $("#InstallFeeBillingType").attr("disabled", true);
    $("#BillingInstallFee").attr("disabled", true);
    $("#BillingOCC").attr("disabled", true);
    $("#InstallationBy").attr("disabled", true);
    $("#SlipIssueDate").EnableDatePicker(false)
    $("#SlipIssueOfficeCode").attr("disabled", true);
    $("#ApproveNo1").attr("disabled", true);
    $("#ApproveNo2").attr("disabled", true);
    $("#EmailAddress").attr("disabled", true);
    $("#btnAdd").attr("disabled", true);
    $("#btnClear").attr("disabled", true);
    $("#btnSearchEmail").attr("disabled", true);
    $("#Memo").attr("disabled", true);
    $("#ChangeContents").attr("disabled", true);
    //####################################################
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

function doInformationAlert(moduleCode, msgCode, paramObj) {
    var obj = {
        module: moduleCode,
        code: msgCode,
        param: paramObj
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        //OpenWarningDialog(result.Message, result.Message, null);
        OpenInformationMessageDialog(msgCode, result.Message);
    });
}



function manualInitialGridInstrument(chkControl) {

    //CheckFirstRowIsEmpty(ISS060_GridInstrumentInfo, true);
    $("#BlnTypeOneTimeOrTemp").val(strFalse);
    if (ISS060_GridInstrumentInfo.hdr.rows.length > 0) {

        for (var i = 0; i < ISS060_GridInstrumentInfo.getRowsNum(); i++) {
            var rowId = ISS060_GridInstrumentInfo.getRowId(i);



            var AddRemovedQTYid = GenerateGridControlID("AddRemovedQTYBox", rowId);
            var MovedQTYid = GenerateGridControlID("MovedQTYBox", rowId);
            var MAExchangeQTYid = GenerateGridControlID("MAExchangeQTYBox", rowId);
            var NotInstallQTYid = GenerateGridControlID("NotInstallQTYBox", rowId);
            var UnremovableQTYid = GenerateGridControlID("UnremovableQTYBox", rowId);

            var amountAddRemovedQTY = 0;
            var amountMovedQTY = 0;
            var amountMAExchangeQTY = 0;
            var amountNotInstallQTY = 0;
            var amountUnremovableQTY = 0;



            var blnEnableAddRemovedQTY = false;
            var blnEnableMovedQTY = false;
            var blnEnableMAExchangeQTY = false;
            var blnEnableNotInstallQTY = false;
            var blnEnableUnremovableQTY = false;


            if ($("#chkCondHave1").prop("checked") == true) {

                blnEnableNotInstallQTY = true;

            }
            if ($("#chkCondHave2").prop("checked") == true) {

                blnEnableUnremovableQTY = true;
                $("#UnremoveApproveNo").attr("readonly", false);
            }
            if ($("#chkCondHave3").prop("checked") == true) {
                blnEnableAddRemovedQTY = true;

            }
            if ($("#chkCondHave4").prop("checked") == true) {

                blnEnableMovedQTY = true;

            }
            if ($("#chkCondHave5").prop("checked") == true) {

                blnEnableMAExchangeQTY = true;

            }



            //================================= GetColumn Index =================================
            var ContractAfterChangeCol = ISS060_GridInstrumentInfo.getColIndexById("ContractInstalledAfterChange");
            var ContractCurrentCol = ISS060_GridInstrumentInfo.getColIndexById("ContractInstalledQTY");
            var TotalStockCol = ISS060_GridInstrumentInfo.getColIndexById("TotalStockedOut");
            var UnUsedCol = ISS060_GridInstrumentInfo.getColIndexById("UnusedQTY");
            var RemovedCol = ISS060_GridInstrumentInfo.getColIndexById("RemovedQTY");
            var AddRemovedCol = ISS060_GridInstrumentInfo.getColIndexById("AddRemovedQTY");
            var MovedCol = ISS060_GridInstrumentInfo.getColIndexById("MovedQTY");
            var MAExchangeCol = ISS060_GridInstrumentInfo.getColIndexById("MAExchangeQTY");
            var NotInstallCol = ISS060_GridInstrumentInfo.getColIndexById("NotInstallQTY");
            var UnremovableCol = ISS060_GridInstrumentInfo.getColIndexById("UnremovableQTY");

            //******************* TRS 22/05/2012 NEW SPEC ********************
            var InstrumentCodeCol = ISS060_GridInstrumentInfo.getColIndexById("InstrumentCode");
            var ParentCodeCol = ISS060_GridInstrumentInfo.getColIndexById("ParentCode");
            var IsUnremovableCol = ISS060_GridInstrumentInfo.getColIndexById("IsUnremovable");
            var IsParentCol = ISS060_GridInstrumentInfo.getColIndexById("IsParent");
            var HiddenInstrumentCodeCol = ISS060_GridInstrumentInfo.getColIndexById("HiddenInstrumentCode");

            IsParentValue = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, IsParentCol);
            IsUnremovableValue = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, IsUnremovableCol);

            //*******************************************************

            //************** Check for generate UNREMOVABLE NUMERIC BOX *********************
            if (IsUnremovableValue.toUpperCase() == "TRUE") {
                //=========== UnremovableQTY ==============================
                if (ISS060_GridInstrumentInfo.hdr.rows.length > 0) {
                    var val = $("#" + UnremovableQTYid).val();
                    if (val == undefined) {
                        val = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, UnremovableCol);
                    }
                    amountUnremovableQTY = val.replace(/,/g, "");
                }
                //=======================================================
                if (chkControl != null) {
                    blnChecked = $("#" + chkControl.id).prop("checked");
                    if (chkControl.id == "chkCondHave2" && blnChecked == false) {
                        amountUnremovableQTY = 0;
                        $("#UnremoveApproveNo").attr("readonly", true);
                    }
                }
                GenerateNumericBox2(ISS060_GridInstrumentInfo, "UnremovableQTYBox", rowId, "UnremovableQTY", amountUnremovableQTY, 10, 0, 0, 9999999999, true, blnEnableUnremovableQTY);
                $("#" + UnremovableQTYid).css('width', '60px');
                $("#" + UnremovableQTYid).attr("maxlength", 4);
                $("#" + UnremovableQTYid).blur(function () {
                    //manualInitialGridInstrument(null);
                });
                //ISS060_GridInstrumentInfo.cells(rowId, UnremovableCol).setValue("<div style='text-align:right'>" + ISS060_GridInstrumentInfo.cells(rowId, UnremovableCol).getValue() + "</div>");

            }
            else {
                ISS060_GridInstrumentInfo.cells(rowId, UnremovableCol).setValue("<div style='text-align:right'>-</div>");
            }

            //************** Check for generate PARENT NUMERIC BOX *********************
            if (IsParentValue.toUpperCase() == "TRUE") {
                //=========== AddRemovedQTY ==============================
                if (ISS060_GridInstrumentInfo.hdr.rows.length > 0) {
                    var val = $("#" + AddRemovedQTYid).val();
                    if (val == undefined) {

                        val = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, AddRemovedCol);
                    }
                    amountAddRemovedQTY = val.replace(/,/g, "");
                }
                //========================================================
                //=========== MovedQTY ==============================
                if (ISS060_GridInstrumentInfo.hdr.rows.length > 0) {
                    var val = $("#" + MovedQTYid).val();
                    if (val == undefined) {
                        val = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, MovedCol);
                    }
                    amountMovedQTY = val.replace(/,/g, "");
                }
                //========================================================
                //=========== MAExchangeQTYid ==============================
                if (ISS060_GridInstrumentInfo.hdr.rows.length > 0) {
                    var val = $("#" + MAExchangeQTYid).val();
                    if (val == undefined) {
                        val = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, MAExchangeCol);
                    }
                    amountMAExchangeQTY = val.replace(/,/g, "");
                }
                //========================================================
                //=========== NotInstallQTY ==============================
                if (ISS060_GridInstrumentInfo.hdr.rows.length > 0) {
                    var val = $("#" + NotInstallQTYid).val();
                    if (val == undefined) {
                        val = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, NotInstallCol);
                    }
                    amountNotInstallQTY = val.replace(/,/g, "");
                }
                //========================================================

                //============= GET OLD VALUE ===============================================
                var OLDMovedQTYCol = ISS060_GridInstrumentInfo.getColIndexById("OLDMovedQTY");
                var OLDMAExchangeQTYCol = ISS060_GridInstrumentInfo.getColIndexById("OLDMAExchangeQTY");
                var amounOLDMovedQTYCol = 0;
                var amountOLDMAExchangeQTYCol = 0;
                amounOLDMovedQTYCol = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, OLDMovedQTYCol);
                amountOLDMAExchangeQTYCol = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, OLDMAExchangeQTYCol);
                //===========================================================================

                if (amountMovedQTY == "") {
                    amountMovedQTY = amounOLDMovedQTYCol;
                }
                if (amountMAExchangeQTY == "") {
                    amountMAExchangeQTY = amountOLDMAExchangeQTYCol;
                }
                if (chkControl != null) {

                    blnChecked = $("#" + chkControl.id).prop("checked");
                    if (chkControl.id == "chkCondHave1" && blnChecked == false) {
                        amountNotInstallQTY = 0;
                    } else if (chkControl.id == "chkCondHave2" && blnChecked == false) {
                        amountUnremovableQTY = 0;
                        $("#UnremoveApproveNo").attr("readonly", true);
                    } else if (chkControl.id == "chkCondHave3" && blnChecked == false) {
                        amountAddRemovedQTY = 0;
                    } else if (chkControl.id == "chkCondHave4" && blnChecked == false) {
                        amountMovedQTY = amounOLDMovedQTYCol;
                    } else if (chkControl.id == "chkCondHave5" && blnChecked == false) {
                        amountMAExchangeQTY = amountOLDMAExchangeQTYCol;
                    }
                }

                //====================== Calculate Contract Installed After CHANGE ================              

                var amountContractInstalledCurrent = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, ContractCurrentCol);
                var amountTotalStock = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, TotalStockCol);
                var amountUnUsed = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, UnUsedCol);
                var amountRemove = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, RemovedCol);
                //var amountAddRemove = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, AddRemovedCol);

                //================================= Calculate =====================================
                //var amountContractInstallAfterChange = (amountContractInstalledCurrent * 1) + (amountTotalStock * 1) - (amountUnUsed * 1) - (amountRemove * 1) - (amountAddRemovedQTY * 1);
                //                            amountUnUsed = amountUnUsed + amountNotInstallQTY;
                //                            amountNotInstallQTY = 0;
                //                            amountRemove = amountRemove + amountAddRemovedQTY;
                //                            amountAddRemovedQTY = 0;
                //=================================================================================

                //ISS060_GridInstrumentInfo.cells2(i, ContractAfterChangeCol).setValue(amountContractInstallAfterChange + "");
                //Contract installed (after change)] = [Contract installed (Current)] + [Total stock-out instrument qty]
                // - [Unused instrument qty] - [Removed instrument qty] – [Additional removed qty];
                //=================================================================================

                //==================== Generate Numeric Box ==============================
                GenerateNumericBox2(ISS060_GridInstrumentInfo, "AddRemovedQTYBox", rowId, "AddRemovedQTY", amountAddRemovedQTY, 10, 0, 0, 9999999999, true, blnEnableAddRemovedQTY);
                $("#" + AddRemovedQTYid).css('width', '60px');
                $("#" + AddRemovedQTYid).attr("maxlength", 4);
                $("#" + AddRemovedQTYid).blur(function () {
                    //manualInitialGridInstrument(null);
                });
                //                            $("#" + AddRemovedQTYid).change(function () {
                //                                AddRemovedValueChange(AddRemovedQTYid, rowId);
                //                            });
                //ISS060_GridInstrumentInfo.cells(rowId, AddRemovedCol).setValue("<div style='text-align:right'>" + ISS060_GridInstrumentInfo.cells(rowId, AddRemovedCol).getValue() + "</div>");

                GenerateNumericBox2(ISS060_GridInstrumentInfo, "MovedQTYBox", rowId, "MovedQTY", amountMovedQTY, 10, 0, 0, 9999999999, true, blnEnableMovedQTY);
                $("#" + MovedQTYid).css('width', '60px');
                $("#" + MovedQTYid).attr("maxlength", 4);
                $("#" + MovedQTYid).blur(function () {
                    //manualInitialGridInstrument(null);
                });
                $("#" + MovedQTYid).change(
                            { "MovedQTYid": MovedQTYid, "rowId": rowId },
                            function (event) {
                                MovedValueChange(event.data.MovedQTYid, event.data.rowId);
                            });
                //ISS060_GridInstrumentInfo.cells(rowId, MovedCol).setValue("<div style='text-align:right'>" + ISS060_GridInstrumentInfo.cells(rowId, MovedCol).getValue() + "</div>");

                GenerateNumericBox2(ISS060_GridInstrumentInfo, "MAExchangeQTYBox", rowId, "MAExchangeQTY", amountMAExchangeQTY, 10, 0, 0, 9999999999, true, blnEnableMAExchangeQTY);
                $("#" + MAExchangeQTYid).css('width', '60px');
                $("#" + MAExchangeQTYid).attr("maxlength", 4);
                $("#" + MAExchangeQTYid).blur(function () {
                    //manualInitialGridInstrument(null);
                });
                $("#" + MAExchangeQTYid).change(
                            { "MAExchangeQTYid": MAExchangeQTYid, "rowId": rowId },
                            function (event) {
                                MAExchangeChange(event.data.MAExchangeQTYid, event.data.rowId);
                            });
                //ISS060_GridInstrumentInfo.cells(rowId, MAExchangeCol).setValue("<div style='text-align:right'>" + ISS060_GridInstrumentInfo.cells(rowId, MAExchangeCol).getValue() + "</div>");

                GenerateNumericBox2(ISS060_GridInstrumentInfo, "NotInstallQTYBox", rowId, "NotInstallQTY", amountNotInstallQTY, 10, 0, 0, 9999999999, true, blnEnableNotInstallQTY);
                $("#" + NotInstallQTYid).css('width', '60px');
                $("#" + NotInstallQTYid).attr("maxlength", 4);
                $("#" + NotInstallQTYid).blur(function () {
                    //manualInitialGridInstrument(null);
                });



                //$("#" + NotInstallQTYid).change({ "NotInstallQTYid": NotInstallQTYid, "rowId": rowId }, function (event) { NotInstallValueChange(event.data.NotInstallQTYid, event.data.rowId); });


                //ISS060_GridInstrumentInfo.cells(rowId, NotInstallCol).setValue("<div style='text-align:right'>" + ISS060_GridInstrumentInfo.cells(rowId, NotInstallCol).getValue() + "</div>");


                //========================================================================
            }
            else {
                ISS060_GridInstrumentInfo.cells(rowId, InstrumentCodeCol).setValue("+ " + ISS060_GridInstrumentInfo.cells(rowId, HiddenInstrumentCodeCol).getValue());

                ISS060_GridInstrumentInfo.cells(rowId, ContractCurrentCol).setValue("<div style='text-align:right'>-</div>");
                ISS060_GridInstrumentInfo.cells(rowId, TotalStockCol).setValue("<div style='text-align:right'>-</div>");
                ISS060_GridInstrumentInfo.cells(rowId, UnUsedCol).setValue("<div style='text-align:right'>-</div>");
                ISS060_GridInstrumentInfo.cells(rowId, RemovedCol).setValue("<div style='text-align:right'>-</div>");
                ISS060_GridInstrumentInfo.cells(rowId, AddRemovedCol).setValue("<div style='text-align:right'>-</div>");
                ISS060_GridInstrumentInfo.cells(rowId, ContractAfterChangeCol).setValue("<div style='text-align:right'>-</div>");
                ISS060_GridInstrumentInfo.cells(rowId, MovedCol).setValue("<div style='text-align:right'>-</div>");
                ISS060_GridInstrumentInfo.cells(rowId, MAExchangeCol).setValue("<div style='text-align:right'>-</div>");
                ISS060_GridInstrumentInfo.cells(rowId, NotInstallCol).setValue("<div style='text-align:right'>-</div>");


            }


            //====================================================================================



            ISS060_GridInstrumentInfo.setSizes();

        }

    }
}




function ChangeInstallationType() {
    if ($("#OldInstallationType").val() != "") {
        DeleteAllRow(ISS060_GridInstrumentInfo);
    }


    if ($("#InstallationType").val() != "") {
        $("#divInstallationInstrumentInfo").show();
        manualInitialGridInstrument(null);
    }
    else {
        $("#divInstallationInstrumentInfo").hide();
    }
    $("#OldInstallationType").val($("#InstallationType").val());

}

function ResetAdditional() {

    DeleteAllRow(ISS060_GridInstrumentInfo);
    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
    call_ajax_method_json("/Installation/ISS060_ClearInstrumentInfo", obj, function (result, controls) {


    });
}

function ClearInstrumentInput() {
    $("#InstrumentCode").val("");
    $("#InstrumentName").val("");
    $("#InstrumentQty").val("");
}

function clickHaveCondition() {
    //        var blnChecked = $("#" + this.id).prop("checked");

    //        $("#chkCondHave1").attr("checked", false);
    //        $("#chkCondHave2").attr("checked", false);
    //        $("#chkCondHave3").attr("checked", false);
    //        $("#chkCondHave4").attr("checked", false);
    //        $("#chkCondHave5").attr("checked", false);

    //        $("#" + this.id).attr("checked", blnChecked);
    if ($("#chkCondHave2").prop("checked") != true) {
        $("#UnremoveApproveNo").val("");
    }
    manualInitialGridInstrument(this);
    InitialNormalInstallationFee();
}

function initialInstrumentDetail(strInstallType, ContractStatus) {

    //if( strInstallType == C_RENTAL_INSTALL_TYPE_NEW || strInstallType == C_SALE_INSTALL_TYPE_NEW || strInstallType == C_SALE_INSTALL_TYPE_ADD )
    if (strInstallType == C_RENTAL_INSTALL_TYPE_NEW || strInstallType == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW) {
        $("#divInstallationInstrumentInfo").show();
        $("#divCondHave1").show();
        $("#divCondHave2").hide();
        $("#UnremoveApproveNo").hide();
        $("#divCondHave3").hide();
        $("#divCondHave4").hide();
        $("#divCondHave5").hide();
    }
    else if (strInstallType == C_SALE_INSTALL_TYPE_NEW || strInstallType == C_SALE_INSTALL_TYPE_ADD) {
        $("#divInstallationInstrumentInfo").show();
        $("#divCondHave1").hide();
        $("#divCondHave2").hide();
        $("#UnremoveApproveNo").hide();
        $("#divCondHave3").hide();
        $("#divCondHave4").hide();
        $("#divCondHave5").hide();
    }
    else if (strInstallType == C_RENTAL_INSTALL_TYPE_CHANGE_WIRING || strInstallType == C_SALE_INSTALL_TYPE_CHANGE_WIRING) {
        $("#divInstallationInstrumentInfo").hide();
    }
    else if (strInstallType == C_RENTAL_INSTALL_TYPE_MOVE || strInstallType == C_SALE_INSTALL_TYPE_MOVE) {
        $("#divInstallationInstrumentInfo").show();
        $("#divCondHave1").hide();
        $("#divCondHave2").hide();
        $("#UnremoveApproveNo").hide();
        $("#divCondHave3").hide();
        $("#divCondHave4").show();
        $("#divCondHave5").hide();
    }
    else if (strInstallType == C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                || strInstallType == C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE) {
        $("#divInstallationInstrumentInfo").show();
        $("#divCondHave1").hide();
        //            $("#divCondHave2").hide();
        //            $("#UnremoveApproveNo").hide();
        $("#divCondHave3").hide();
        $("#divCondHave4").hide();
        //Phoomsak L.  2012-06-19: show -> hide
        $("#divCondHave5").hide();
        if (($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL && ContractStatus == C_CONTRACT_STATUS_BEF_START)
            || $("#ServiceTypeCode").val() == C_SERVICE_TYPE_SALE) {
            $("#divCondHave2").hide();
            $("#UnremoveApproveNo").hide();
        }
        else {
            $("#divCondHave2").show();
            $("#UnremoveApproveNo").show();
        }
    }
    else if (strInstallType == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE) {
        $("#divInstallationInstrumentInfo").show();
        $("#divCondHave1").show();
        $("#divCondHave3").hide();
        $("#divCondHave4").hide();
        $("#divCondHave5").hide();
        if (($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL && ContractStatus == C_CONTRACT_STATUS_BEF_START)
            || $("#ServiceTypeCode").val() == C_SERVICE_TYPE_SALE) {
            $("#divCondHave2").hide();
            $("#UnremoveApproveNo").hide();
        }
        else {
            $("#divCondHave2").show();
            $("#UnremoveApproveNo").show();
        }
    }
    else if (strInstallType == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW) {
        $("#divInstallationInstrumentInfo").show();
        $("#divCondHave1").show();
        $("#divCondHave3").show();
        $("#divCondHave4").hide();
        $("#divCondHave5").hide();
        if (($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL && ContractStatus == C_CONTRACT_STATUS_BEF_START)
            || $("#ServiceTypeCode").val() == C_SERVICE_TYPE_SALE) {
            $("#divCondHave2").hide();
            $("#UnremoveApproveNo").hide();
        }
        else {
            $("#divCondHave2").show();
            $("#UnremoveApproveNo").show();
        }
    }
    else if (strInstallType == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL
        || strInstallType == C_SALE_INSTALL_TYPE_PARTIAL_REMOVE) {
        $("#divInstallationInstrumentInfo").show();
        $("#divCondHave1").hide();
        $("#divCondHave3").show();
        $("#divCondHave4").hide();
        $("#divCondHave5").hide();
        if (($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL && ContractStatus == C_CONTRACT_STATUS_BEF_START)
            || $("#ServiceTypeCode").val() == C_SERVICE_TYPE_SALE) {
            $("#divCondHave2").hide();
            $("#UnremoveApproveNo").hide();
        }
        else {
            $("#divCondHave2").show();
            $("#UnremoveApproveNo").show();
        }
    }
    else if (strInstallType == C_RENTAL_INSTALL_TYPE_REMOVE_ALL
        || strInstallType == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
        || strInstallType == C_SALE_INSTALL_TYPE_REMOVE_ALL) {
        $("#divInstallationInstrumentInfo").show();
        $("#divCondHave1").hide();
        $("#divCondHave3").hide();
        $("#divCondHave4").hide();
        $("#divCondHave5").hide();
        if (($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL && ContractStatus == C_CONTRACT_STATUS_BEF_START)
            || $("#ServiceTypeCode").val() == C_SERVICE_TYPE_SALE) {
            $("#divCondHave2").hide();
            $("#UnremoveApproveNo").hide();
        }
        else {
            $("#divCondHave2").show();
            $("#UnremoveApproveNo").show();
        }
    }
    //        else
    //        {
    //            $("#divInstallationInstrumentInfo").show();
    //            $("#divCondHave1").show();
    //            //$("#divCondHave2").show();
    //            //$("#UnremoveApproveNo").show();
    //            $("#divCondHave3").show();
    //            $("#divCondHave4").hide();
    //            $("#divCondHave5").hide();
    //        }

    //        if ( ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL && ContractStatus == C_CONTRACT_STATUS_BEF_START)
    //            || $("#ServiceTypeCode").val() == C_SERVICE_TYPE_SALE )
    //        {
    //            $("#divCondHave2").hide();
    //            $("#UnremoveApproveNo").hide();
    //        }
    //        else
    //        {
    //            $("#divCondHave2").show();
    //            $("#UnremoveApproveNo").show();          
    //        }


}

function initialInstallationFeeBilling(strInstallType, InstallFeeBillingType) {

    if ((strInstallType == C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
            strInstallType == C_RENTAL_INSTALL_TYPE_MOVE ||
            strInstallType == C_RENTAL_INSTALL_TYPE_CHANGE_WIRING ||
            strInstallType == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL ||
            strInstallType == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL ||
            strInstallType == C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE ||

            strInstallType == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
            strInstallType == C_SALE_INSTALL_TYPE_MOVE ||
            strInstallType == C_SALE_INSTALL_TYPE_CHANGE_WIRING ||
            strInstallType == C_SALE_INSTALL_TYPE_PARTIAL_REMOVE ||
            strInstallType == C_SALE_INSTALL_TYPE_REMOVE_ALL) && InstallFeeBillingType != C_INSTALL_FEE_BILLING_TYPE_PAY_ALL_AMOUNT) {
        $("#NewBillingInstallFee").attr("readonly", false);
        $("#NewBillingOCC").attr("disabled", false);

    }
    else {
        $("#NewBillingInstallFee").attr("readonly", true);
        $("#NewBillingOCC").attr("disabled", true);
    }

}

function ValidateInstrumentDetail() {
    var blnValidateInstrument = true;

    var NotInstallQTYCol = ISS060_GridInstrumentInfo.getColIndexById("NotInstallQTY");
    var TotalStockedOutCol = ISS060_GridInstrumentInfo.getColIndexById("TotalStockedOut");
    var UnremovableQTYCol = ISS060_GridInstrumentInfo.getColIndexById("UnremovableQTY");
    var AddRemovedQTYCol = ISS060_GridInstrumentInfo.getColIndexById("AddRemovedQTY");
    var RemovedQTYCol = ISS060_GridInstrumentInfo.getColIndexById("RemovedQTY");
    var MAExchangeQTYCol = ISS060_GridInstrumentInfo.getColIndexById("MAExchangeQTY");
    var ContractInstalledAfterChangeCol = ISS060_GridInstrumentInfo.getColIndexById("ContractInstalledAfterChange");
    var ContractInstalledQTYCol = ISS060_GridInstrumentInfo.getColIndexById("ContractInstalledQTY");
    var UnusedQTYCol = ISS060_GridInstrumentInfo.getColIndexById("UnusedQTY");
    var InstrumentCodeCol = ISS060_GridInstrumentInfo.getColIndexById("InstrumentCode");
    var MovedCol = ISS060_GridInstrumentInfo.getColIndexById("MovedQTY");
    var ParentRemoveQty = 0;
    var ParentMAExchangeQty = 0;
    //============== counter quantity ========================
    var iNotInstallQty = 0;
    var iUnremoveQty = 0;
    var iAddRemoveQty = 0;
    var iTotalStockoutQty = 0;
    var iUnusedQty = 0;

    var bAddMoveQty = false;
    //=========================================================
    for (var i = 0; i < ISS060_GridInstrumentInfo.getRowsNum(); i++) {

        var rowId = ISS060_GridInstrumentInfo.getRowId(i);

        var NotInstallQTYid = GenerateGridControlID("NotInstallQTYBox", rowId);
        var UnremovableQTYid = GenerateGridControlID("UnremovableQTYBox", rowId);
        var AddRemovedQTYid = GenerateGridControlID("AddRemovedQTYBox", rowId);
        var RemovedQTYid = GenerateGridControlID("RemovedQTYBox", rowId);
        var MAExchangeQTYid = GenerateGridControlID("MAExchangeQTYBox", rowId);
        var ContractInstalledQTYid = GenerateGridControlID("ContractInstalledQTYBox", rowId);
        var RemovedQTYid = GenerateGridControlID("RemovedQTYBox", rowId);
        var UnusedQTYid = GenerateGridControlID("UnusedQTYBox", rowId);
        var MovedQTYid = GenerateGridControlID("MovedQTYBox", rowId);


        var amountNotInstallQTY = 0;
        var amountTotalStockedOut = 0;
        var amountUnremovableQTY = 0;
        var amountAddRemovedQTY = 0;
        var amountRemovedQTY = 0;
        var amountMAExchangeQTY = 0;
        var amountContractInstalledAfterChange = 0;
        var amountContractInstalledQTY = 0;
        var amountUnusedQTY = 0;

        var InstrumentCode = ISS060_GridInstrumentInfo.cells2(i, InstrumentCodeCol).getValue();
        //============ amountNotInstallQTY ==============
        amountNotInstallQTY = $("#" + NotInstallQTYid).val();
        if (amountNotInstallQTY == undefined) {
            amountNotInstallQTY = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, NotInstallQTYCol);
        }
        amountNotInstallQTY = amountNotInstallQTY.replace(/,/g, "");
        //============ amountTotalStockedOut ==============
        amountTotalStockedOut = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, TotalStockedOutCol);
        amountTotalStockedOut = amountTotalStockedOut.replace(/,/g, "");
        //============ amountUnremovableQTY ==============
        amountUnremovableQTY = $("#" + UnremovableQTYid).val();
        if (amountUnremovableQTY == undefined) {
            amountUnremovableQTY = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, UnremovableQTYCol);
        }
        amountUnremovableQTY = amountUnremovableQTY.replace(/,/g, "");
        //============ amountAddRemovedQTY ==============
        amountAddRemovedQTY = $("#" + AddRemovedQTYid).val();
        if (amountAddRemovedQTY == undefined) {
            amountAddRemovedQTY = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, AddRemovedQTYCol);
        }
        amountAddRemovedQTY = amountAddRemovedQTY.replace(/,/g, "");
        //============ amountRemovedQTY ==============
        amountRemovedQTY = $("#" + RemovedQTYid).val();
        if (amountRemovedQTY == undefined) {
            amountRemovedQTY = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, RemovedQTYCol);
        }
        amountRemovedQTY = amountRemovedQTY.replace(/,/g, "");
        //============ amountMAExchangeQTY ==============
        amountMAExchangeQTY = $("#" + MAExchangeQTYid).val();
        if (amountMAExchangeQTY == undefined) {
            amountMAExchangeQTY = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, MAExchangeQTYCol);
        }
        amountMAExchangeQTY = amountMAExchangeQTY.replace(/,/g, "");

        //============ amountContractInstalledAfterChange ==============
        amountContractInstalledAfterChange = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, ContractInstalledAfterChangeCol);
        amountContractInstalledAfterChange = amountContractInstalledAfterChange.replace(/,/g, "");
        //============ amountContractInstalledQTY ==============
        amountContractInstalledQTY = $("#" + ContractInstalledQTYid).val();
        if (amountContractInstalledQTY == undefined) {
            amountContractInstalledQTY = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, ContractInstalledQTYCol);
        }
        amountMAExchangeQTY = amountMAExchangeQTY.replace(/,/g, "");
        //============ amountUnusedQTY ==============
        amountUnusedQTY = $("#" + amountUnusedQTY).val();
        if (amountUnusedQTY == undefined) {
            amountUnusedQTY = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, UnusedQTYCol);
        }
        amountUnusedQTY = amountUnusedQTY.replace(/,/g, "");
        //===========================================
        //=========== MovedQTY ==============================           
        amountMovedQTY = $("#" + MovedQTYid).val();
        if (amountMovedQTY == undefined) {
            amountMovedQTY = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, MovedCol);
        }
        amountMovedQTY = amountMovedQTY.replace(/,/g, "");
        //========================================================

        amountNotInstallQTY = amountNotInstallQTY * 1;
        amountTotalStockedOut = amountTotalStockedOut * 1;
        amountUnremovableQTY = amountUnremovableQTY * 1;
        amountAddRemovedQTY = amountAddRemovedQTY * 1;
        amountRemovedQTY = amountRemovedQTY * 1;
        amountMAExchangeQTY = amountMAExchangeQTY * 1;
        amountContractInstalledAfterChange = amountContractInstalledAfterChange * 1;
        amountContractInstalledQTY = amountContractInstalledQTY * 1;
        amountUnusedQTY = amountUnusedQTY * 1;
        amountMovedQTY = amountMovedQTY * 1;

        //******************* TRS 22/05/2012 *************************************************
        var ParentCodeCol = ISS060_GridInstrumentInfo.getColIndexById("ParentCode");
        var IsUnremovableCol = ISS060_GridInstrumentInfo.getColIndexById("IsUnremovable");
        var IsParentCol = ISS060_GridInstrumentInfo.getColIndexById("IsParent");
        var HiddenInstrumentCodeCol = ISS060_GridInstrumentInfo.getColIndexById("HiddenInstrumentCode");
        var OLDMovedQTYCol = ISS060_GridInstrumentInfo.getColIndexById("OLDMovedQTY");
        var IsParentValue = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, IsParentCol);
        var IsUnremovableValue = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, IsUnremovableCol);
        var ParentCodeValue = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, ParentCodeCol);
        var HiddenInstrumentCodeValue = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, HiddenInstrumentCodeCol);
        //*************************************************************************************

        //==================== Counter quantity =======================
        if (IsParentValue.toUpperCase() == "TRUE") {
            var amounOLDMovedQTYCol = 0;
            amounOLDMovedQTYCol = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, OLDMovedQTYCol);

            iNotInstallQty = iNotInstallQty + amountNotInstallQTY;
            iAddRemoveQty = iAddRemoveQty + amountAddRemovedQTY;
            iTotalStockoutQty = iTotalStockoutQty + amountTotalStockedOut;
            iUnusedQty = iUnusedQty + amountUnusedQTY;

            if (amountMovedQTY != amounOLDMovedQTYCol) {
                bAddMoveQty = true;
            }
        }

        if (IsUnremovableValue.toUpperCase() == "TRUE") {
            iUnremoveQty = iUnremoveQty + amountUnremovableQTY;
        }

        //=============================================================

        if (IsParentValue.toUpperCase() == "TRUE") {
            ParentRemoveQty = amountAddRemovedQTY + amountRemovedQTY;
            ParentMAExchangeQty = amountMAExchangeQTY;
        }
        if (HiddenInstrumentCodeValue == ParentCodeValue && $("#chkCondHave1").prop("checked") == true && amountNotInstallQTY + amountUnusedQTY > amountTotalStockedOut) {
            doAlert("Installation", "MSG5040", InstrumentCode);
            ISS060_GridInstrumentInfo.selectRow(ISS060_GridInstrumentInfo.getRowIndex(rowId));
            blnValidateInstrument = false;
            return;
        }
        //if ($("#chkCondHave2").prop("checked") == true && (amountUnremovableQTY > (amountAddRemovedQTY + amountRemovedQTY) || amountUnremovableQTY > amountMAExchangeQTY))
        if ($("#chkCondHave2").prop("checked") == true
                && IsUnremovableValue.toUpperCase() == "TRUE"
                && amountUnremovableQTY > 0
                && (amountUnremovableQTY > (ParentRemoveQty))) {
            doAlert("Installation", "MSG5041", InstrumentCode);
            ISS060_GridInstrumentInfo.selectRow(ISS060_GridInstrumentInfo.getRowIndex(rowId));
            blnValidateInstrument = false;
            return;
        }

        //            if( amountContractInstalledAfterChange != (amountContractInstalledQTY + amountTotalStockedOut - amountUnusedQTY - amountNotInstallQTY - amountRemovedQTY - amountAddRemovedQTY  ))
        //            {
        //                doAlert("Installation", "MSG5044", "Installation");
        //                ISS060_GridInstrumentInfo.selectRow(ISS060_GridInstrumentInfo.getRowIndex(rowId));
        //                blnValidateInstrument = false;
        //            }

        if ($("#chkCondHave2").prop("checked") == true
            && IsUnremovableValue.toUpperCase() == "TRUE"
            && amountUnremovableQTY > 0
            && ($("#InstallationTypeCode").val() == C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
                $("#InstallationTypeCode").val() == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
                $("#InstallationTypeCode").val() == C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE)
            && amountUnremovableQTY > ParentMAExchangeQty) {
            doAlert("Installation", "MSG5073", "Installation");
            ISS060_GridInstrumentInfo.selectRow(ISS060_GridInstrumentInfo.getRowIndex(rowId));
            blnValidateInstrument = false;
            return;
        }

        if ($("#chkCondHave4").prop("checked") == true && IsParentValue.toUpperCase() == "TRUE" && amountMovedQTY > amountContractInstalledQTY) {
            doAlert("Installation", "MSG5099", InstrumentCode);
            ISS060_GridInstrumentInfo.selectRow(ISS060_GridInstrumentInfo.getRowIndex(rowId));
            blnValidateInstrument = false;
            return;
        }

        if ($("#chkCondHave3").prop("checked") == true && IsParentValue.toUpperCase() == "TRUE" && (amountAddRemovedQTY + amountRemovedQTY) > amountContractInstalledQTY) {
            doAlert("Installation", "MSG5104", InstrumentCode);
            ISS060_GridInstrumentInfo.selectRow(ISS060_GridInstrumentInfo.getRowIndex(rowId));
            blnValidateInstrument = false;
            return;
        }
    }

    if ($("#chkCondHave1").prop("checked") == true && iNotInstallQty == 0) {
        doAlert("Installation", "MSG5106", "");
        blnValidateInstrument = false;
        return;
    }
    if ($("#chkCondHave2").prop("checked") == true && iUnremoveQty == 0) {
        doAlert("Installation", "MSG5107", "");
        blnValidateInstrument = false;
        return;
    }
    if ($("#chkCondHave3").prop("checked") == true && iAddRemoveQty == 0) {
        doAlert("Installation", "MSG5108", "");
        blnValidateInstrument = false;
        return;
    }
    if ($("#chkCondHave4").prop("checked") == true && bAddMoveQty == false) {
        doAlert("Installation", "MSG5109", "");
        blnValidateInstrument = false;
        return;
    }

    if ((iNotInstallQty + iUnusedQty) > 0 && (iNotInstallQty + iUnusedQty) == iTotalStockoutQty) {
        doAlert("Installation", "MSG5121", "");
        blnValidateInstrument = false;
        return;
    }


    return blnValidateInstrument;
}

function moneyConvert(value) {
    if (value != null) {
        var buf = "";
        var sBuf = "";
        var j = 0;
        value = String(value);

        value = value.replace(/,/g, "")

        if (value.indexOf(".") > 0) {
            buf = value.substring(0, value.indexOf("."));
        } else {
            buf = value;
        }
        if (buf.length % 3 != 0 && (buf.length / 3 - 1) > 0) {
            sBuf = buf.substring(0, buf.length % 3) + ",";
            buf = buf.substring(buf.length % 3);
        }
        j = buf.length;
        for (var i = 0; i < (j / 3 - 1); i++) {
            sBuf = sBuf + buf.substring(0, 3) + ",";
            buf = buf.substring(3);
        }
        sBuf = sBuf + buf;
        if (value.indexOf(".") > 0) {
            value = sBuf + value.substring(value.indexOf("."), value.indexOf(".") + 3);
        }
        else {
            if (sBuf != "") {
                value = sBuf + ".00";
            }
            else {
                value = "0.00";
            }

        }
        return value;
    }
}

function MovedValueChange(controlID, rowId) {

    if ($("#" + controlID).val() == "") {
        var selectedRowIndex = ISS060_GridInstrumentInfo.getRowIndex(rowId);
        //============= GET OLD VALUE ===============================================
        var OLDMovedQTYCol = ISS060_GridInstrumentInfo.getColIndexById("OLDMovedQTY");
        var amounOLDMovedQTYCol = 0;
        amounOLDMovedQTYCol = GetValueFromLinkType(ISS060_GridInstrumentInfo, selectedRowIndex, OLDMovedQTYCol);
        //===========================================================================
        $("#" + controlID).val(amounOLDMovedQTYCol);

    }

}

function MAExchangeChange(controlID, rowId) {

    if ($("#" + controlID).val() == "") {
        var selectedRowIndex = ISS060_GridInstrumentInfo.getRowIndex(rowId);
        //============= GET OLD VALUE ===============================================        
        var OLDMAExchangeQTYCol = ISS060_GridInstrumentInfo.getColIndexById("OLDMAExchangeQTY");
        var amountOLDMAExchangeQTYCol = 0;
        amountOLDMAExchangeQTYCol = GetValueFromLinkType(ISS060_GridInstrumentInfo, selectedRowIndex, OLDMAExchangeQTYCol);
        //===========================================================================     
        $("#" + controlID).val(amountOLDMAExchangeQTYCol);
    }
}

function AddRemovedValueChange(controlID, rowId) {

    var selectedRowIndex = ISS060_GridInstrumentInfo.getRowIndex(rowId);
    var OLDAddRemovedQTYCol = ISS060_GridInstrumentInfo.getColIndexById("OLDAddRemovedQty");
    ISS060_GridInstrumentInfo.cells2(selectedRowIndex, OLDAddRemovedQTYCol).setValue($("#" + controlID).val() + "");

}

function NotInstallValueChange(controlID, rowId) {

    var selectedRowIndex = ISS060_GridInstrumentInfo.getRowIndex(rowId);
    var OLDNotInstallQTYCol = ISS060_GridInstrumentInfo.getColIndexById("OLDNotInstalledQty");
    ISS060_GridInstrumentInfo.cells2(selectedRowIndex, OLDNotInstallQTYCol).setValue($("#" + controlID).val() + "");

}

function calculateGridInstrumentForConfirmState() {
    if (CheckFirstRowIsEmpty(ISS060_GridInstrumentInfo) == false) {
        for (var i = 0; i < ISS060_GridInstrumentInfo.getRowsNum(); i++) {
            var rowId = ISS060_GridInstrumentInfo.getRowId(i);

            var IsParentCol = ISS060_GridInstrumentInfo.getColIndexById("IsParent");
            var IsParentValue = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, IsParentCol);

            if (IsParentValue.toUpperCase() == "TRUE") {
                var UnUsedCol = ISS060_GridInstrumentInfo.getColIndexById("UnusedQTY");
                var NotInstallCol = ISS060_GridInstrumentInfo.getColIndexById("NotInstallQTY");
                var RemovedCol = ISS060_GridInstrumentInfo.getColIndexById("RemovedQTY");
                var AddRemovedCol = ISS060_GridInstrumentInfo.getColIndexById("AddRemovedQTY");
                var ContractAfterChangeCol = ISS060_GridInstrumentInfo.getColIndexById("ContractInstalledAfterChange");
                var ContractCurrentCol = ISS060_GridInstrumentInfo.getColIndexById("ContractInstalledQTY");
                var TotalStockCol = ISS060_GridInstrumentInfo.getColIndexById("TotalStockedOut");

                var NotInstallQTYid = GenerateGridControlID("NotInstallQTYBox", rowId);
                var AddRemovedQTYid = GenerateGridControlID("AddRemovedQTYBox", rowId);

                var amountUnUsed = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, UnUsedCol);
                var amountRemove = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, RemovedCol);
                var amountContractAfterChange = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, ContractAfterChangeCol);
                var amountContractCurrent = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, ContractCurrentCol);
                var amountTotalStock = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, TotalStockCol);
                //=========== NotInstallQTY ==============================
                if (ISS060_GridInstrumentInfo.hdr.rows.length > 0) {
                    var val = $("#" + NotInstallQTYid).val();
                    if (val == undefined) {
                        val = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, NotInstallCol);
                    }
                    amountNotInstallQTY = val.replace(/,/g, "");
                }
                //========================================================
                //=========== AddRemovedQTY ==============================
                if (ISS060_GridInstrumentInfo.hdr.rows.length > 0) {
                    var val = $("#" + AddRemovedQTYid).val();
                    if (val == undefined) {

                        val = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, AddRemovedCol);
                    }
                    amountAddRemovedQTY = val.replace(/,/g, "");
                }
                //========================================================


                var CalamounUnusedQty = amountUnUsed * 1 + amountNotInstallQTY * 1;
                var CalRemovedQty = amountAddRemovedQTY * 1 + amountRemove * 1;
                var CalContractInstallAfterChange = amountContractCurrent * 1 + amountTotalStock * 1 - CalamounUnusedQty * 1 - CalRemovedQty * 1;

                ISS060_GridInstrumentInfo.cells2(i, UnUsedCol).setValue(CalamounUnusedQty + "");
                ISS060_GridInstrumentInfo.cells2(i, RemovedCol).setValue(CalRemovedQty + "");
                ISS060_GridInstrumentInfo.cells2(i, ContractAfterChangeCol).setValue(CalContractInstallAfterChange + "");

                //=================== Keep Value to OLD Column =========================                    
                var OldUnusedQtyCol = ISS060_GridInstrumentInfo.getColIndexById("OLDUnusedQty");
                var OldNotInstalledQtyCol = ISS060_GridInstrumentInfo.getColIndexById("OLDNotInstalledQty");
                var OldRemovedQtyCol = ISS060_GridInstrumentInfo.getColIndexById("OLDRemovedQty");
                var OldAddRemovedQtyCol = ISS060_GridInstrumentInfo.getColIndexById("OLDAddRemovedQty");
                var OldContractInstalledAfterChangeCol = ISS060_GridInstrumentInfo.getColIndexById("OLDContractInstalledAfterChange");
                var OldContractInstalledCol = ISS060_GridInstrumentInfo.getColIndexById("OLDContractInstalled");
                var OldTotalStockoutCol = ISS060_GridInstrumentInfo.getColIndexById("OLDTotalStockout");

                ISS060_GridInstrumentInfo.cells2(i, OldUnusedQtyCol).setValue(amountUnUsed + "");
                ISS060_GridInstrumentInfo.cells2(i, OldNotInstalledQtyCol).setValue(amountNotInstallQTY + "");
                ISS060_GridInstrumentInfo.cells2(i, OldRemovedQtyCol).setValue(amountRemove + "");
                ISS060_GridInstrumentInfo.cells2(i, OldAddRemovedQtyCol).setValue(amountAddRemovedQTY + "");
                ISS060_GridInstrumentInfo.cells2(i, OldContractInstalledAfterChangeCol).setValue(amountContractAfterChange + "");
                ISS060_GridInstrumentInfo.cells2(i, OldContractInstalledCol).setValue(amountContractCurrent + "");
                ISS060_GridInstrumentInfo.cells2(i, OldTotalStockoutCol).setValue(amountTotalStock + "");
                //======================================================================


                var blnEnableNotInstallQTY = false;
                var blnEnableAddRemovedQTY = false;
                if ($("#chkCondHave1").prop("checked") == true) {
                    blnEnableNotInstallQTY = true;
                }
                if ($("#chkCondHave3").prop("checked") == true) {
                    blnEnableAddRemovedQTY = true;
                }

                GenerateNumericBox2(ISS060_GridInstrumentInfo, "NotInstallQTYBox", rowId, "NotInstallQTY", 0, 10, 0, 0, 9999999999, true, blnEnableNotInstallQTY);
                $("#" + NotInstallQTYid).css('width', '60px');
                $("#" + NotInstallQTYid).attr("maxlength", 4);

                //$("#" + NotInstallQTYid).change({ "NotInstallQTYid": NotInstallQTYid, "rowId": rowId }, function (event) { NotInstallValueChange(event.data.NotInstallQTYid, event.data.rowId); });


                GenerateNumericBox2(ISS060_GridInstrumentInfo, "AddRemovedQTYBox", rowId, "AddRemovedQTY", 0, 10, 0, 0, 9999999999, true, blnEnableAddRemovedQTY);
                $("#" + AddRemovedQTYid).css('width', '60px');
                $("#" + AddRemovedQTYid).attr("maxlength", 4);

                //$("#" + AddRemovedQTYid).change(function () {
                //    AddRemovedValueChange(AddRemovedQTYid, rowId);
                //});
            }
        }
    }
}

function backCalculateGridInstrumentForRegisState() {
    if (CheckFirstRowIsEmpty(ISS060_GridInstrumentInfo) == false) {
        for (var i = 0; i < ISS060_GridInstrumentInfo.getRowsNum(); i++) {
            var rowId = ISS060_GridInstrumentInfo.getRowId(i);

            var IsParentCol = ISS060_GridInstrumentInfo.getColIndexById("IsParent");
            var IsParentValue = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, IsParentCol);

            if (IsParentValue.toUpperCase() == "TRUE") {
                //================================= GetColumn Index =================================
                var OldUnusedQtyCol = ISS060_GridInstrumentInfo.getColIndexById("OLDUnusedQty");
                var OldNotInstalledQtyCol = ISS060_GridInstrumentInfo.getColIndexById("OLDNotInstalledQty");
                var OldRemovedQtyCol = ISS060_GridInstrumentInfo.getColIndexById("OLDRemovedQty");
                var OldAddRemovedQtyCol = ISS060_GridInstrumentInfo.getColIndexById("OLDAddRemovedQty");
                var OldContractInstalledAfterChangeCol = ISS060_GridInstrumentInfo.getColIndexById("OLDContractInstalledAfterChange");
                var OldContractInstalledCol = ISS060_GridInstrumentInfo.getColIndexById("OLDContractInstalled");
                var OldTotalStockoutCol = ISS060_GridInstrumentInfo.getColIndexById("OLDTotalStockout");

                var amounOldUnusedQty = 0;
                var amounOldNotInstalledQty = 0;
                var amounOldRemovedQty = 0;
                var amounOldAddRemovedQty = 0;
                var amounOldContractInstalledAfterChange = 0;
                var amounOldContractInstalled = 0;
                var amounOldTotalStockout = 0;

                amounOldUnusedQty = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, OldUnusedQtyCol);
                amounOldNotInstalledQty = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, OldNotInstalledQtyCol);
                amounOldRemovedQty = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, OldRemovedQtyCol);
                amounOldAddRemovedQty = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, OldAddRemovedQtyCol);
                amounOldContractInstalledAfterChange = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, OldContractInstalledAfterChangeCol);
                amounOldContractInstalled = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, OldContractInstalledCol);
                amounOldTotalStockout = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, OldTotalStockoutCol);

                //=============== Set Value ===========================================================
                var UnusedQtyCol = ISS060_GridInstrumentInfo.getColIndexById("UnusedQTY");
                ISS060_GridInstrumentInfo.cells2(i, UnusedQtyCol).setValue(amounOldUnusedQty + "");

                var RemovedQTYCol = ISS060_GridInstrumentInfo.getColIndexById("RemovedQTY");
                ISS060_GridInstrumentInfo.cells2(i, RemovedQTYCol).setValue(amounOldRemovedQty + "");

                var ContractInstalledAfterChangeCol = ISS060_GridInstrumentInfo.getColIndexById("ContractInstalledAfterChange");
                ISS060_GridInstrumentInfo.cells2(i, ContractInstalledAfterChangeCol).setValue(amounOldContractInstalledAfterChange + "");

                var ContractInstalledQTYCol = ISS060_GridInstrumentInfo.getColIndexById("ContractInstalledQTY");
                ISS060_GridInstrumentInfo.cells2(i, ContractInstalledQTYCol).setValue(amounOldContractInstalled + "");

                var TotalStockedOutCol = ISS060_GridInstrumentInfo.getColIndexById("TotalStockedOut");
                ISS060_GridInstrumentInfo.cells2(i, TotalStockedOutCol).setValue(amounOldTotalStockout + "");



                var NotInstallQTYid = GenerateGridControlID("NotInstallQTYBox", rowId);
                var AddRemovedQTYid = GenerateGridControlID("AddRemovedQTYBox", rowId);

                var blnEnableNotInstallQTY = false;
                var blnEnableAddRemovedQTY = false;
                if ($("#chkCondHave1").prop("checked") == true) {
                    blnEnableNotInstallQTY = true;
                }
                if ($("#chkCondHave3").prop("checked") == true) {
                    blnEnableAddRemovedQTY = true;
                }

                GenerateNumericBox2(ISS060_GridInstrumentInfo, "NotInstallQTYBox", rowId, "NotInstallQTY", amounOldNotInstalledQty, 10, 0, 0, 9999999999, true, blnEnableNotInstallQTY);
                $("#" + NotInstallQTYid).css('width', '60px');
                $("#" + NotInstallQTYid).attr("maxlength", 4);

                //$("#" + NotInstallQTYid).change({ "NotInstallQTYid": NotInstallQTYid, "rowId": rowId }, function (event) { NotInstallValueChange(event.data.NotInstallQTYid, event.data.rowId); });


                GenerateNumericBox2(ISS060_GridInstrumentInfo, "AddRemovedQTYBox", rowId, "AddRemovedQTY", amounOldAddRemovedQty, 10, 0, 0, 9999999999, true, blnEnableAddRemovedQTY);
                $("#" + AddRemovedQTYid).css('width', '60px');
                $("#" + AddRemovedQTYid).attr("maxlength", 4);
                //$("#" + AddRemovedQTYid).change(AddRemovedValueChange(AddRemovedQTYid, rowId));
            }
        }
    }
}

function CalculateNormalContractFee() {

    var intNormalContractFee = 0;
    var intChangeNormalContractFee = 0;

    intNormalContractFee = $("#NormalContractFee").NumericValue();

    if (CheckFirstRowIsEmpty(ISS060_GridInstrumentInfo) == false) {
        for (var i = 0; i < ISS060_GridInstrumentInfo.getRowsNum(); i++) {
            var rowId = ISS060_GridInstrumentInfo.getRowId(i);

            var IsParentCol = ISS060_GridInstrumentInfo.getColIndexById("IsParent");
            var IsParentValue = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, IsParentCol);

            if (IsParentValue.toUpperCase() == "TRUE") {
                var NotInstallCol = ISS060_GridInstrumentInfo.getColIndexById("NotInstallQTY");
                var AddRemovedCol = ISS060_GridInstrumentInfo.getColIndexById("AddRemovedQTY");
                var InstrumentPriceCol = ISS060_GridInstrumentInfo.getColIndexById("InstrumentPrice");

                var NotInstallQTYid = GenerateGridControlID("NotInstallQTYBox", rowId);
                var AddRemovedQTYid = GenerateGridControlID("AddRemovedQTYBox", rowId);

                var amountInstrumentPrice = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, InstrumentPriceCol);
                if (amountInstrumentPrice == "" || amountInstrumentPrice == undefined || amountInstrumentPrice == null) {
                    amountInstrumentPrice = "0";
                }
                amountInstrumentPrice = amountInstrumentPrice.replace(/,/g, "");
                //=========== NotInstallQTY ==============================
                var amountNotInstallQTY = 0;
                var val = $("#" + NotInstallQTYid).val();
                if (val == undefined) {
                    val = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, NotInstallCol);
                }
                amountNotInstallQTY = val.replace(/,/g, "");
                //========================================================
                //=========== AddRemovedQTY ==============================
                var amountAddRemovedQTY = 0;
                var val = $("#" + AddRemovedQTYid).val();
                if (val == undefined) {
                    val = GetValueFromLinkType(ISS060_GridInstrumentInfo, i, AddRemovedCol);
                }
                amountAddRemovedQTY = val.replace(/,/g, "");
                //========================================================
                intChangeNormalContractFee = intChangeNormalContractFee - (amountInstrumentPrice * ((amountNotInstallQTY * 1) + (amountAddRemovedQTY * 1)));

            }

        }
    }
    var CalNormalContractFee = (intNormalContractFee * 1) + intChangeNormalContractFee

    if (CalNormalContractFee < 0)
        tempCalNormalContractFee = 0;
    else
        tempCalNormalContractFee = CalNormalContractFee;
}

function InitialNormalInstallationFee() {
    if ($("#chkCondHave1").prop("checked") == true || $("#chkCondHave2").prop("checked") == true || $("#chkCondHave3").prop("checked") == true || $("#chkCondHave4").prop("checked") == true || $("#chkCondHave5").prop("checked") == true) {
        $("#NewNormalInstallFee").attr("readonly", false);
    }
    else {
        $("#NewNormalInstallFee").val(onRetrieveNormalInstallFee);
        $("#NewNormalInstallFee").attr("readonly", true);
    }
}


function InitLoadAttachList() {

    ISS060_gridAttach = $("#ISS060_gridAttachDocList").LoadDataToGridWithInitial(0, false, false, "/Installation/ISS060_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, false, null,
                            function () {
                                if (hasAlert) {
                                    hasAlert = false;
                                    OpenWarningDialog(alertMsg);
                                }
                                $('#frmAttach').load(RefreshAttachList);

                                isInitAttachGrid = true;
                            });
}

function RefreshAttachList() {

    if (ISS060_gridAttach != undefined && isInitAttachGrid) {

        $('#ISS060_gridAttachDocList').LoadDataToGrid(ISS060_gridAttach, 0, false, "/Installation/ISS060_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, function () {
            if (hasAlert) {
                hasAlert = false;
                OpenWarningDialog(alertMsg);
            }
        }, null)
    }

}

function ISS060_gridAttachBinding() {

    if (ISS060_gridAttach != undefined) {
        var _colRemoveBtn = ISS060_gridAttach.getColIndexById("removeButton");

        for (var i = 0; i < ISS060_gridAttach.getRowsNum(); i++) {
            var row_id = ISS060_gridAttach.getRowId(i);
            GenerateRemoveButton(ISS060_gridAttach, "btnRemoveAttach", row_id, "removeButton", true);
            BindGridButtonClickEvent("btnRemoveAttach", row_id, btnRemoveAttach_click);
        }
    }

    ISS060_gridAttach.setSizes();
}

function ClearAllAttachFile() {

    if (ISS060_gridAttach.getRowsNum() > 0)
        DeleteAllRow(ISS060_gridAttach);

    var obj = { strContractCode: "" };
    call_ajax_method_json("/Installation/ISS060_ClearAllAttach", obj, function (result, controls) {


    });
}

function disabledGridAttach() {
    colInx = ISS060_gridAttach.getColIndexById("removeButton")
    ISS060_gridAttach.setColumnHidden(colInx, true);
    $("#divAttachFrame").hide();
    $("#divAttachRemark").hide();
}

function enabledGridAttach() {
    colInx = ISS060_gridAttach.getColIndexById("removeButton")
    ISS060_gridAttach.setColumnHidden(colInx, false);
    SetFitColumnForBackAction(ISS060_gridAttach, "TmpColumn");
    $("#divAttachFrame").show();
    $("#divAttachRemark").show();
}

function btnRemoveAttach_click(row_id) {

    var obj = {
        module: "Common",
        code: "MSG0142"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    var _colID = ISS060_gridAttach.getColIndexById("AttachFileID");
                    var _targID = ISS060_gridAttach.cells(row_id, _colID).getValue();

                    var obj = {
                        AttachID: _targID
                    };
                    call_ajax_method_json("/Installation/ISS060_RemoveAttach", obj, function (result, controls) {
                        if (result != null) {
                            RefreshAttachList();
                        }
                    });
                });
    });


}

function doAlertWithMessage(msgCode, msgText) {
    hasAlert = true;
    alertMsg = msgText;
}
