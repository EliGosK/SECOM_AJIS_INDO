

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
var ISS070_GridEmail;
var ISS070_GridInstrumentInfo;
var strNewRow = "NEWROW";
var strTrue = "true";
var strFalse = "false";

var conditionCancelAll = "CANCELALL";
var conditionCancelPO = "CANCELPO";
var conditionCancelPOAndSlip = "CANCELPOANDSLIP";
var conditionCancelSlipUsePrevious = "CANCELSLIPUSEPREVIOUS";
var conditionCancelSlip = "CANCELSLIP";

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

    $("#NewBillingInstallFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#NewNormalInstallFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#NewSECOMPaymentFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#NewSECOMRevenueFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);

    $("#InstallFinishDate").InitialDate();
    $("#InstallStartDate").InitialDate();
    $("#InstallCompleteDate").InitialDate();

    $("#btnRetrieveInstallation").click(retrieve_installation_click);
    $("#btnClearInstallation").click(clear_installation_click);

        
    initialGridOnload();



    setInitialState();

    if (strContractProjectCode != "") {
        $("#btnRetrieveInstallation").attr("disabled", true);
        $("#ContractCodeProjectCode").val(strContractProjectCode)
        setTimeout("retrieve_installation_click()", 2000);
    }

    $("#ChangeCustomerReason").attr("disabled", true);
    $("#ChangeSecomReason").attr("disabled", true);

});

function setInitialState() {
    // --------------- Initial SCREEN ------------------
    $("#divContractBasicInfo").SetViewMode(false);
    $("#divInstallationInfo").SetViewMode(false);
    $("#divInstallationInstrumentInfo").SetViewMode(false);

    $("#divCancelCondition").SetViewMode(false);
    $("#divProjectInfo").SetViewMode(false);
    $("#divInputContractCode").SetViewMode(false);    

    $("#ContractCodeProjectCode").attr("disabled", false);
    $("#btnRetrieveInstallation").attr("disabled", false);
    $("#btnClearInstallation").attr("disabled", false);   

    InitialCommandButton(0);
    $("#divInputContractCode").clearForm();
    $("#divContractBasicInfo").clearForm();
    $("#divInstallationInstrumentInfo").clearForm();
    $("#divInstallationInfo").clearForm();
    $("#divCancelCondition").clearForm();
    $("#divProjectInfo").clearForm();

    $("#divInputContractCode").show();
    $("#divContractBasicInfo").hide();
    $("#divInstallationInstrumentInfo").hide();
    $("#divInstallationInfo").hide();
    $("#divCancelCondition").hide();
    $("#divProjectInfo").hide();

    $("#ContractCodeProjectCode").attr("readonly", false);
    //--------------------------------------------------  
}

function retrieve_installation_click() {
    $("#btnRetrieveInstallation").attr("disabled", true);
    command_control.CommandControlMode(false);
    if (CheckFirstRowIsEmpty(ISS070_GridInstrumentInfo) == false) {
        DeleteAllRow(ISS070_GridInstrumentInfo);
    }
    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
    call_ajax_method_json("/Installation/ISS070_RetrieveData", obj,
        function (result, controls) {
            command_control.CommandControlMode(true);
            if (controls != undefined) {
                $("#btnRetrieveInstallation").attr("disabled", false);
                VaridateCtrl(["ContractCodeProjectCode"], controls);
                //$("#divInputContractCode").clearForm();
                $("#divContractBasicInfo").clearForm();
                $("#divInstallationInstrumentInfo").clearForm();
                $("#divInstallationInfo").clearForm();
                $("#divRegisterComplete").clearForm();
                $("#divCancelCondition").clearForm();
                $("#divProjectInfo").clearForm();

                return;
            }
            else if (result != undefined) {
                setInitialState();
                $("#ContractCodeProjectCode").val(result.ContractCodeShort);
                $("#btnRetrieveInstallation").attr("disabled", true);
                InitialCommandButton(1);
                //                if (result.blnValidateContractError == false) {
                //                    InitialCommandButton(0);
                //                    setInitialState();
                //                }

                //                if (result.InstallType != undefined) {
                //                    initialInstrumentDetail(result.do_TbtInstallationBasic.InstallationType);
                //                    initialInstallationFeeBilling(result.do_TbtInstallationBasic.InstallationType);
                //                }
                //$("#divInputContractCode").clearForm();
                //                $("#divContractBasicInfo").clearForm();
                //                $("#divInstallationInstrumentInfo").clearForm();
                //                $("#divInstallationInfo").clearForm();
                //                $("#divRegisterComplete").clearForm();
                //                $("#divCancelCondition").clearForm();
                //                $("#divProjectInfo").clearForm();
                if (result.ServiceTypeCode == C_SERVICE_TYPE_RENTAL || result.ServiceTypeCode == C_SERVICE_TYPE_SALE) {
                    SEARCH_CONDITION = {
                        ContractCode: result.ContractCodeShort
                    };
                }
                else {
                    SEARCH_CONDITION = {
                        ProjectCode: result.ContractCodeShort
                    };
                }

                $("#ServiceTypeCode").val(result.ServiceTypeCode);
                if (result.do_TbtInstallationSlip != null) {
                    result.do_TbtInstallationSlip.SlipIssueDate = ConvertDateToTextFormat(result.do_TbtInstallationSlip.SlipIssueDate.replace('/Date(', '').replace(')/', '') * 1);
                    if (result.do_TbtInstallationSlip.ExpectedInstrumentArrivalDate != null) {
                        result.do_TbtInstallationSlip.ExpectedInstrumentArrivalDate = ConvertDateToTextFormat(result.do_TbtInstallationSlip.ExpectedInstrumentArrivalDate.replace('/Date(', '').replace(')/', '') * 1);
                    }
                    $("#Memo").val(result.InstallationMemo);

                }
                if (result.dtRentalContractBasic != null) {
                    result.dtRentalContractBasic.ContractCode = $("#ContractCodeProjectCode").val();

                    $("#OperationOfficeCode").val(result.dtRentalContractBasic.OperationOfficeCode)
                    $("#divContractBasicInfo").bindJSON(result.dtRentalContractBasic);
                    $("#divInstallationInfo").bindJSON(result.dtRentalContractBasic);
                    $("#AdditionalStockOutOfficeCode").val(result.dtRentalContractBasic.OperationOfficeCode);
                    setConfirmState(1);
                }
                else if (result.dtSale != null) {
                    result.dtSale.ContractCode = $("#ContractCodeProjectCode").val();

                    $("#OperationOfficeCode").val(result.dtSale.OperationOfficeCode)
                    $("#divContractBasicInfo").bindJSON(result.dtSale);
                    $("#divInstallationInfo").bindJSON(result.dtSale);
                    $("#DocAuditResult").bindJSON(result.DocAuditResult);
                    $("#AdditionalStockOutOfficeCode").val(result.dtSale.OperationOfficeCode);
                    setConfirmState(1);
                } else if (result.dtProject != null) {

                    $("#divProjectInfo").bindJSON(result.dtProject);

                    setConfirmState(2);
                }

                $("#divInstallationInfo").bindJSON(result.do_TbtInstallationBasic);
                $("#divContractBasicInfo").bindJSON(result.do_TbtInstallationSlip);
                $("#NormalContractFee").val("");
                $("#divInstallationInfo").bindJSON(result.do_TbtInstallationSlip);
                $("#divInstallationInstrumentInfo").bindJSON(result.do_TbtInstallationSlip);
                $("#InstallationType").val("");
                //var AmountAddInstallQty = 0;

                $("#ContractCode").val(result.ContractCodeShort);
                if (result.do_TbtInstallationBasic != null) {
                    if (result.do_TbtInstallationBasic.InstallationTypeRentalName != null) {
                        $("#InstallationType").val(result.do_TbtInstallationBasic.InstallationTypeRentalName);
                    }
                    else if (result.do_TbtInstallationBasic.InstallationTypeSaleName != null) {
                        $("#InstallationType").val(result.do_TbtInstallationBasic.InstallationTypeSaleName);
                    }
                }
                if (result.do_TbtInstallationSlip != null) {

                    $("#ContractCodeProjectCode").val(result.ContractCodeShort);
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

                    //                    if (result.do_TbtInstallationSlip.InstallationTypeRentalName != null) {
                    //                        $("#InstallationType").val(result.do_TbtInstallationSlip.InstallationTypeRentalName);
                    //                    }
                    //                    else if (result.do_TbtInstallationSlip.InstallationTypeSaleName != null) {
                    //                        $("#InstallationType").val(result.do_TbtInstallationSlip.InstallationTypeSaleName);
                    //                    }

                    //---------------------------------------                    
                    if (result.do_TbtInstallationSlip.CauseReasonCustomerName != null) {
                        $("#CauseReason").val(result.do_TbtInstallationSlip.CauseReasonCustomerName);
                    }
                    else if (result.do_TbtInstallationSlip.CauseReasonSecomName != null) {
                        $("#CauseReason").val(result.do_TbtInstallationSlip.CauseReasonSecomName);
                    }

                    $("#InstallFeeBillingType").val(result.do_TbtInstallationSlip.InstallFeeBillingTypeName);
                    obj = {
                        ValueCode: result.do_TbtInstallationSlip.SlipIssueOfficeCode
                    };
                    call_ajax_method('/Installation/ISS070_GetOfficeNameByCode', obj, function (result, controls) {
                        if (result != null) {
                            $("#SlipIssueOfficeCode").val(result);

                        }
                    });
                    //----------------------------------------

                    //======================================================
                }
                if (result.do_TbtInstallationBasic != null && result.do_TbtInstallationBasic != undefined) {


                    $("#InstallationMANo").val(result.do_TbtInstallationBasic.MaintenanceNo);

                    $("#InstallationBy").val(result.do_TbtInstallationBasic.InstallationByName);
                }
                /////////////// BIND EMAIl DATA //////////////////
                if (result.ListDOEmail != null) {
                    if (result.ListDOEmail.length > 0) {
                        for (var i = 0; i < result.ListDOEmail.length; i++) {
                            var emailList = [result.ListDOEmail[i].EmailAddress];

                            CheckFirstRowIsEmpty(ISS070_GridEmail, true);
                            AddNewRow(ISS070_GridEmail, emailList);

                        }
                    }
                }
                //////////////////////////////////////////////////

                /////////////// BIND Instrument DATA //////////////////

                if (result.do_TbtInstallationSlipDetails != null) {
                    if (result.do_TbtInstallationSlipDetails.length > 0) {
                        for (var i = 0; i < result.do_TbtInstallationSlipDetails.length; i++) {
                            //====== Teerapong S. 16/08/2012 ==============
                            if (result.do_TbtInstallationSlip.SlipStatus == C_SLIP_STATUS_STOCK_OUT) {
                                tempAddInstallQty = 0;
                            }
                            else {
                                tempAddInstallQty = result.do_TbtInstallationSlipDetails[i].AddInstalledQty
                            }
                            //=============================================
                            var InstrumentList = [checkNullValue(result.do_TbtInstallationSlipDetails[i].InstrumentCode),
                                                  checkNullValue(result.arrayInstrumentName[i]),
                                                  qtyConvert(checkNullValue(result.do_TbtInstallationSlipDetails[i].ContractInstalledQty)) + "",
                                                  qtyConvert(checkNullValue(result.do_TbtInstallationSlipDetails[i].TotalStockOutQty)) + "",
                            //qtyConvert(checkNullValue(result.do_TbtInstallationSlipDetails[i].AddInstalledQty)) + "",
                                                  qtyConvert(checkNullValue(tempAddInstallQty)) + "",
                                                  qtyConvert(checkNullValue(result.do_TbtInstallationSlipDetails[i].ReturnQty)) + "",
                                                  qtyConvert(checkNullValue(result.do_TbtInstallationSlipDetails[i].AddRemovedQty)) + "",

                                                   qtyConvert((result.do_TbtInstallationSlipDetails[i].ContractInstalledQty +
                            //result.do_TbtInstallationSlipDetails[i].AddInstalledQty -
                                                  tempAddInstallQty -
                                                  result.do_TbtInstallationSlipDetails[i].ReturnQty -
                                                  result.do_TbtInstallationSlipDetails[i].AddRemovedQty +
                                                  result.do_TbtInstallationSlipDetails[i].TotalStockOutQty)) + '',

                                                  qtyConvert(checkNullValue(result.do_TbtInstallationSlipDetails[i].MoveQty)) + "",
                                                  qtyConvert(checkNullValue(result.do_TbtInstallationSlipDetails[i].MAExchangeQty)) + "",
                                                  "", "", "", "", "", "", "", checkNullValue(result.do_TbtInstallationSlipDetails[i].PartialStockOutQty)];

                            CheckFirstRowIsEmpty(ISS070_GridInstrumentInfo, true);
                            AddNewRow(ISS070_GridInstrumentInfo, InstrumentList);
                        }
                    }
                }
                //////////////////////////////////////////////////




                initialScreenOnRetrieve(result);
                $("#ChangeCustomerReason").attr("disabled", true);
                $("#ChangeSecomReason").attr("disabled", true);
                //======================== INITIAL VALUE =========================================
                //if ($("#NormalContractFee").NumericValue() != "")
                if (result.do_TbtInstallationSlip != null)
                {
                    if (result.do_TbtInstallationSlip.NormalContractFeeCurrencyType == C_CURRENCY_LOCAL || result.do_TbtInstallationSlip.NormalContractFeeCurrencyType == null)
                    {
                        $("#NormalContractFee").val(moneyConvert($("#NormalContractFee").NumericValue()));
                    }
                    else
                    {
                        $("#NormalContractFee").val(moneyConvert(result.do_TbtInstallationSlip.NormalContractFeeUsd));
                    }
                }

                    //                if ($("#SECOMPaymentFee").NumericValue() != "")
                    //                    $("#SECOMPaymentFee").val(moneyConvert($("#SECOMPaymentFee").NumericValue()));

                $("#m_blnbFirstTimeRegister").val(result.m_blnbFirstTimeRegister);
                $("#NewNormalContractFee").val($("#NormalContractFee").val());
                $("#OldNormalContractFee").val($("#NormalContractFee").val());
                if (result.dtRentalContractBasic != null) {
                    $("#ChangeType").val(result.dtRentalContractBasic.ChangeType);
                }
                if (result.do_TbtInstallationSlip != null) {
                    $("#SlipType").val(result.do_TbtInstallationSlip.SlipType);
                }
                //================================================================================

                setTimeout("manualInitialGridInstrument(null)", 1500);
                ///// TEST //////
                //SendGridSlipDetailsToObject();
                if ($("#NewBldMgmtCost").NumericValue() != "")
                    $("#NewBldMgmtCost").val(moneyConvert($("#NewBldMgmtCost").NumericValue()));
                if ($("#BillingInstallFee").NumericValue() != "")
                    $("#BillingInstallFee").val(moneyConvert($("#BillingInstallFee").NumericValue()));
                if ($("#NormalInstallFee").NumericValue() != "")
                    $("#NormalInstallFee").val(moneyConvert($("#NormalInstallFee").NumericValue()));
                if ($("#SECOMPaymentFee").NumericValue() != "")
                    $("#SECOMPaymentFee").val(moneyConvert($("#SECOMPaymentFee").NumericValue()));
                if ($("#SECOMRevenueFee").NumericValue() != "")
                    $("#SECOMRevenueFee").val(moneyConvert($("#SECOMRevenueFee").NumericValue()));

            }
            else {
                $("#btnRetrieveInstallation").attr("disabled", false);
                setInitialState();
            }
        }
    );
}



function checkNullValue(Value) { 
    if(Value == null)
    {
        return "";
    }
    else
    {
        return Value;
    }
}


function clearAllScreen() {
    /* --- Set condition --- */
    SEARCH_CONDITION = {
        ContractCode: "",
        ProjectCode: ""
    };
    /* --------------------- */
    setInitialState();
    btnClearEmailClick();

    var obj = null;
    call_ajax_method_json("/Installation/ISS070_ClearCommonContractCode", obj, function (result, controls) {


    });
}


function btnClearEmailClick() {
    DeleteAllRow(ISS070_GridEmail);
    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
    call_ajax_method_json("/Installation/ISS070_ClearInstallEmail", obj, function (result, controls) {


    });

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
        SetRegisterCommand(false, null);
        SetResetCommand(false, null);
        SetApproveCommand(false, null);
        SetRejectCommand(false, null);
        SetReturnCommand(false, null);
        SetCloseCommand(true, command_close_click);
        SetConfirmCommand(true, command_confirm_click);
        SetBackCommand(false, null);
    }    
}

function command_close_click() {
    window.location.href = generate_url("/common/CMS020");
}

function initialSECOMNPayment() {

    $("#SECOMPaymentFee").val("");
    $("#SECOMRevenueFee").val("");
    if ($("#BillingInstallFee").NumericValue() != "" && $("#NormalInstallFee").NumericValue() != "")
    {
        if ($("#BillingInstallFee").NumericValue() * 1 < $("#NormalInstallFee").NumericValue() * 1) {
            $("#SECOMPaymentFee").val((($("#NormalInstallFee").NumericValue() * 1) - ($("#BillingInstallFee").NumericValue() * 1)));
        }
        if ($("#BillingInstallFee").NumericValue() * 1 > $("#NormalInstallFee").NumericValue() * 1) {
            $("#SECOMRevenueFee").val((($("#BillingInstallFee").NumericValue() * 1) - ($("#NormalInstallFee").NumericValue() * 1)));
        }
    }
}

//function command_register_click() {
//    //enableInputControls();
//    SendGridSlipDetailsToObject();
//    var obj = CreateObjectData($("#form1").serialize() + "&Memo=" + $("#NewMemo").val() + "&ChangeContents=" + $("#ChangeContents").val() + "&ExpectedInstrumentArrivalDate=" + $("#ExpectedInstrumentArrivalDate").val() + "&StockOutTypeCode=" + $("#StockOutTypeCode").val() );
//    var blnChkInstrument = ValidateInstrumentDetail();
//    if (blnChkInstrument)
//    {
//        call_ajax_method_json("/Installation/ISS070_ValidateBeforeRegister", obj, function (result, controls) {
//            
//            if (controls != undefined) {
//                /* --- Higlight Text --- */
//                /* --------------------- */
//                VaridateCtrl(["InstallStartDate", "InstallFinishDate", "InstallCompleteDate", "NewNormalInstallFee"], controls);
//                /* --------------------- */

//                return;
//            }
//            else {
//                if ($("#NewBillingInstallFee").attr("disabled") == false && ($("#NewBillingInstallFee").val() == "" || $("#NewBillingInstallFee").val() == "0")) {
//                    doAlert("Common", "MSG0007", "Billing install fee");
//                    VaridateCtrl(["NewBillingInstallFee"], ["NewBillingInstallFee"]);
//                    return;
//                }
//                if ($("#NewBillingOCC").attr("disabled") == false && ($("#NewBillingOCC").val() == "" || $("#NewBillingOCC").val() == "0")) {
//                    doAlert("Common", "MSG0007", "Billing OCC");
//                    VaridateCtrl(["NewBillingOCC"], ["NewBillingOCC"]);
//                    return;
//                }
//                if ($("#chkCondHave2").prop("checked") == true && $("#ChangeApproveNo").val() == "") {
//                    doAlert("Common", "MSG0007", "Approve no.");
//                    VaridateCtrl(["ChangeApproveNo"], ["ChangeApproveNo"]);
//                    return;
//                }
//                if ($("#chkCondHave5").prop("checked") == true && $("#AdditionalStockOutOfficeCode").val() == "") {
//                    doAlert("Common", "MSG0007", "Stock-out origin of additional");
//                    VaridateCtrl(["AdditionalStockOutOfficeCode"], ["AdditionalStockOutOfficeCode"]);
//                    return;
//                }
//                //CalculateNormalContractFee();
//                //$("#NormalContractFee").attr("readonly", false);
//                //$("#NormalContractFee").val($("#NewNormalContractFee").val());
//                //$("#NormalContractFee").attr("readonly", true);
//                validateWarningData();
//            }


//        });   
//    }
//}

//function validateWarningData() {
//    var strServiceTypeCode = $("#ServiceTypeCode").val();
//    var strInstallationType = $("#InstallationType").val();
//    var strDocAuditResult = $("#DocAuditResult").val();
//    if ((strServiceTypeCode = C_SERVICE_TYPE_SALE) &&
//         (strInstallationType == C_SALE_INSTALL_TYPE_NEW || strInstallationType == C_SALE_INSTALL_TYPE_ADD) &&
//         (strDocAuditResult == C_DOC_AUDIT_RESULT_NOT_RECEIVED || strDocAuditResult == C_DOC_AUDIT_RESULT_NEED_FOLLOW || strDocAuditResult == C_DOC_AUDIT_RESULT_RETURNED)) {
//        // Get Message
//        var obj = {
//            module: "Installation",
//            code: "MSG5049"
//        };

//        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
//            OpenYesNoMessageDialog(result.Code, result.Message,
//                function () {
//                    setConfirmState();
//                });
//        });
//    }
//    else {
//        setConfirmState();
//    }

//}


function setConfirmState(cond) {

    $("#ContractCodeProjectCode").attr("readonly", true);
    if (cond == 1) {
        $("#InstallationType").attr("disabled", false);
        $("#divContractBasicInfo").show();
        $("#divProjectInfo").hide();
        $("#divInstallationInfo").show();
        $("#divCancelCondition").show();
        $("#divInputContractCode").show();
        $("#divInstallationInstrumentInfo").show();
    }
    else if (cond == 2) {
        $("#InstallationType").attr("disabled", true);
        $("#divContractBasicInfo").hide();
        $("#divProjectInfo").show();
        $("#divInstallationInfo").hide();
        $("#divCancelCondition").show();        
        $("#divInputContractCode").show();
        $("#divInstallationInstrumentInfo").hide();
    }
    
}

function setSuccessRegisState() {
    // --------------- Initial SCREEN ------------------
    $("#divContractBasicInfo").SetViewMode(true);
    $("#divProjectInfo").SetViewMode(true);
    $("#divInstallationInfo").SetViewMode(true);
    $("#divInstallationInstrumentInfo").SetViewMode(true);
    $("#divCancelCondition").SetViewMode(true);
    $("#divInputContractCode").SetViewMode(true);
    

    $("#divInstallationMANo").show();
    $("#divResultRegisSlip").show();
//    $("#ContractCodeProjectCode").attr("disabled", true);
//    $("#btnRetrieveInstallation").attr("disabled", true);
//    $("#btnClearInstallation").attr("disabled", false);

    //disabledInputControls();

    InitialCommandButton(0);
}

function command_confirm_click() {
//    command_control.CommandControlMode(false);
//    var strCode = "";
//    if ($("#CancelConditionAllInstall").prop("checked") == true) {
//        strCode = "MSG5074";
//    } else if ($("#CancelConditionPO").prop("checked") == true) {
//        strCode = "MSG5075";
//    } else if ($("#CancelConditionPOAndSlip").prop("checked") == true) {
//        strCode = "MSG5076";
//    } else if ($("#CancelConditionSlip").prop("checked") == true) {
//        strCode = "MSG5077";
//    } else if ($("#CancelConditionPrevious").prop("checked") == true) {
//        strCode = "MSG5078";
//    } else {
//        return false;
//    }

//    // Get Message
//    var obj = {
//        module: "Installation",
//        code: strCode
//    };

//    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
//        command_control.CommandControlMode(true);
//        OpenYesNoMessageDialog(result.Code, result.Message, function () {

//            call_ajax_method_json("/Installation/ISS070_ValidateBeforeRegister", obj, function (result, controls) {

//                if (controls != undefined) {
//                    /* --- Higlight Text --- */
//                    /* --------------------- */
//                    VaridateCtrl(["SlipStatusName", "ContractCodeProjectCode", "InstallCompleteDate", "NewNormalInstallFee"], controls);
//                    /* --------------------- */

//                    return;
//                }
//                else if(result) {

//                    confirm_data();

//                }


//            });   
//        
//        }, function () {

//        });

    //    });

    command_control.CommandControlMode(false);
    
    call_ajax_method_json("/Installation/ISS070_ValidateBeforeRegister", null, function (result, controls) {
        command_control.CommandControlMode(true);
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["SlipStatusName", "ContractCodeProjectCode", "InstallCompleteDate", "NewNormalInstallFee"], controls);
            /* --------------------- */

            return;
        }
        else if (result) {
            var strCode = "";
            if ($("#CancelConditionAllInstall").prop("checked") == true) {
                strCode = "MSG5074";
            } else if ($("#CancelConditionPO").prop("checked") == true) {
                strCode = "MSG5075";
            } else if ($("#CancelConditionPOAndSlip").prop("checked") == true) {
                strCode = "MSG5076";
            } else if ($("#CancelConditionSlip").prop("checked") == true) {
                strCode = "MSG5077";
            } else if ($("#CancelConditionPrevious").prop("checked") == true) {
                strCode = "MSG5078";
            } else {
                return false;
            }

            // Get Message
            var obj = {
                module: "Installation",
                code: strCode
            };
            command_control.CommandControlMode(false);
            call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                command_control.CommandControlMode(true);
                OpenYesNoMessageDialog(result.Code, result.Message, function () {
                    confirm_data();
                }, function () {

                });

            });
            

        }


    });
    
}

function confirm_data() {
    command_control.CommandControlMode(false);
    var conditionCancel = "";

    if ($("#CancelConditionAllInstall").prop("checked") == true) {
        conditionCancel = conditionCancelAll;
    } else if ($("#CancelConditionPO").prop("checked") == true) {
        conditionCancel = conditionCancelPO;
    } else if ($("#CancelConditionPOAndSlip").prop("checked") == true) {
        conditionCancel = conditionCancelPOAndSlip;
    } else if ($("#CancelConditionSlip").prop("checked") == true) {
        conditionCancel = conditionCancelSlip;
    } else if ($("#CancelConditionPrevious").prop("checked") == true) {
        conditionCancel = conditionCancelSlipUsePrevious;
    }
    var obj = CreateObjectData("conditionCancel=" + conditionCancel);
    //var obj = CreateObjectData($("#form1").serialize() + "&Memo=" + $("#NewMemo").val() + "&ChangeContents=" + $("#ChangeContents").val() + "&ExpectedInstrumentArrivalDate=" + $("#ExpectedInstrumentArrivalDate").val() + "&StockOutTypeCode=" + $("#StockOutTypeCode").val() );


    master_event.LockWindow(true);
    call_ajax_method_json("/Installation/ISS070_RegisterData", obj, function (result, controls) {
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
            setSuccessRegisState();
            /* -------------------------- */
            /////////////////////////// PRINT REPORT AFTER SUCCESS //////////////////////////////
            //************************ COMMENT FOR ADD REPORT
            //window.open("ISS070_QuotationForCancelContractMemorandum");
            //************************
            ////////////////////////////////////////////////////////////////////////////////////
            var obj = {
                module: "Common",
                code: "MSG0108",
                param: ""
            };

            call_ajax_method_json("/Shared/GetMessage", obj, function (result) {

                OpenInformationMessageDialog(result.Code, result.Message, function (result, controls) {
                    window.location.href = generate_url("/common/CMS020");
                });
                
            });


        }
    });

    
}

function command_reset_click() {

    setInitialState();
    $("#btnRetrieveInstallation").attr("disabled", true);
}

function SetRegisterState(cond) {

    InitialCommandButton(1);

    if (cond == 1) {
        $("#InstallationType").attr("disabled", false);
        $("#divContractBasicInfo").show();
        $("#divProjectInfo").hide();
        $("#divInstallationInfo").show();
    

        $("#divCancelCondition").show();
        $("#divProjectInfo").show();
        $("#divInputContractCode").show();
    }
    else if (cond == 2) {
        $("#InstallationType").attr("disabled", true);
        $("#divContractBasicInfo").hide();
        $("#divProjectInfo").show();
        $("#divInstallationInfo").show();

        $("#divCancelCondition").show();
        $("#divProjectInfo").show();
        $("#divInputContractCode").show();
         
    }

}







function initialGridOnload() {
    // intial grid
    ISS070_GridEmail = $("#gridEmail").InitialGrid(pageRow, false, "/Installation/ISS070_InitialGridEmail");
    ISS070_GridInstrumentInfo = $("#gridInstrumentInfo").InitialGrid(pageRow, false, "/Installation/ISS070_InitialGridInstrumentInfo");
    if (CheckFirstRowIsEmpty(ISS070_GridInstrumentInfo) == false) {
        DeleteAllRow(ISS070_GridInstrumentInfo);
    }

    /* ===== binding event when finish load data ===== */
    BindOnLoadedEvent(ISS070_GridInstrumentInfo, function () {
            manualInitialGridInstrument(null);

        
        
    });

//    ISS070_GridInstrumentInfo.attachEvent("onRowSelect", function (id, ind) {
//        var row_num = ISS070_GridInstrumentInfo.getRowIndex(id);
//        if (ISS070_GridInstrumentInfo.cell.childNodes[0].tagName == "INPUT") {
//            var txt = ISS070_GridInstrumentInfo.cell.childNodes[0];

//            if (txt.disabled == false) {
//                txt.focus();
//                //txt.select();
//            }
//        }
//    });
   
}





function disabledGridEmail() {
    //////// DISABLED BUTTON In EMAIL GRID ///////////
    for (var i = 0; i < ISS070_GridEmail.getRowsNum(); i++) {
        var row_id = ISS070_GridEmail.getRowId(i);
        EnableGridButton(ISS070_GridEmail, "btnRemoveEmail", row_id, "ButtonRemove", false);
    }
    //////////////////////////////////////////////////
}

function enabledGridEmail() {
    //////// ENABLED BUTTON In EMAIL GRID ///////////
    for (var i = 0; i < ISS070_GridEmail.getRowsNum(); i++) {
        var row_id = ISS070_GridEmail.getRowId(i);
        EnableGridButton(ISS070_GridEmail, "btnRemoveEmail", row_id, "ButtonRemove", true);
    }
    //////////////////////////////////////////////////
}



function ClearInstrumentInfo() {
    DeleteAllRow(ISS070_GridInstrumentInfo);
    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
    call_ajax_method_json("/Installation/ISS070_ClearInstrumentInfo", obj, function (result, controls) {


    });

}

function SendGridSlipDetailsToObject() {
    if (ISS070_GridInstrumentInfo.hdr.rows.length > 0) {
        var objArray = new Array();

        if (CheckFirstRowIsEmpty(ISS070_GridInstrumentInfo) == false) {
            for (var i = 0; i < ISS070_GridInstrumentInfo.getRowsNum(); i++) {
                var rowId = ISS070_GridInstrumentInfo.getRowId(i);
                //================================= GetColumn Index =================================
                var InstrumentCodeCol = ISS070_GridInstrumentInfo.getColIndexById("InstrumentCode");
                var ContractAfterChangeCol = ISS070_GridInstrumentInfo.getColIndexById("ContractInstalledAfterChange");
                var ContractCurrentCol = ISS070_GridInstrumentInfo.getColIndexById("ContractInstalledQTY");
                var TotalStockCol = ISS070_GridInstrumentInfo.getColIndexById("TotalStockedOut");
                var UnUsedCol = ISS070_GridInstrumentInfo.getColIndexById("UnusedQTY");
                var RemovedCol = ISS070_GridInstrumentInfo.getColIndexById("RemovedQTY");
                var AddRemovedCol = ISS070_GridInstrumentInfo.getColIndexById("AddRemovedQTY");
                var MovedCol = ISS070_GridInstrumentInfo.getColIndexById("MovedQTY");
                var MAExchangeCol = ISS070_GridInstrumentInfo.getColIndexById("MAExchangeQTY");
                var NotInstallCol = ISS070_GridInstrumentInfo.getColIndexById("NotInstallQTY");
                var UnremovableCol = ISS070_GridInstrumentInfo.getColIndexById("UnremovableQTY");

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

                //=========== AddRemovedQTY ==============================
                if (ISS070_GridInstrumentInfo.hdr.rows.length > 0) {
                    var val = $("#" + AddRemovedQTYid).val();
                    if (val == undefined) {

                        val = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, AddRemovedCol);
                    }
                    amountAddRemovedQTY = val.replace(/,/g, "");
                }
                //========================================================
                //=========== MovedQTY ==============================
                if (ISS070_GridInstrumentInfo.hdr.rows.length > 0) {
                    var val = $("#" + MovedQTYid).val();
                    if (val == undefined) {
                        val = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, MovedCol);
                    }
                    amountMovedQTY = val.replace(/,/g, "");
                }
                //========================================================
                //=========== MAExchangeQTYid ==============================
                if (ISS070_GridInstrumentInfo.hdr.rows.length > 0) {
                    var val = $("#" + MAExchangeQTYid).val();
                    if (val == undefined) {
                        val = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, MAExchangeCol);
                    }
                    amountMAExchangeQTY = val.replace(/,/g, "");
                }
                //========================================================
                //=========== NotInstallQTY ==============================
                if (ISS070_GridInstrumentInfo.hdr.rows.length > 0) {
                    var val = $("#" + NotInstallQTYid).val();
                    if (val == undefined) {
                        val = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, NotInstallCol);
                    }
                    amountNotInstallQTY = val.replace(/,/g, "");
                }
                //========================================================
                //=========== UnremovableQTY ==============================
                if (ISS070_GridInstrumentInfo.hdr.rows.length > 0) {
                    var val = $("#" + UnremovableQTYid).val();
                    if (val == undefined) {
                        val = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, UnremovableCol);
                    }
                    amountUnremovableQTY = val.replace(/,/g, "");
                }
                //=======================================================


                var iobj = {
                  InstrumentCode: ISS070_GridInstrumentInfo.cells2(i, InstrumentCodeCol).getValue()
                , ContractInstalledAfterChange: ISS070_GridInstrumentInfo.cells2(i, ContractAfterChangeCol).getValue()
                , ContractInstalledQty: ISS070_GridInstrumentInfo.cells2(i, ContractCurrentCol).getValue()
                , TotalStockOutQty: ISS070_GridInstrumentInfo.cells2(i, TotalStockCol).getValue()
                , ReturnQty: ISS070_GridInstrumentInfo.cells2(i, UnUsedCol).getValue()
                , AddRemovedQty: amountAddRemovedQTY
                , MoveQty: amountMovedQTY
                , MAExchangeQty: amountMAExchangeQTY
                , NotinstalledQty: amountNotInstallQTY
                , UnremovableQty: amountUnremovableQTY
               
                };
                objArray.push(iobj);
            }
        }

        var obj = {
            do_TbtInstallationSlipDetails: objArray
            , ListInstrumentData: objArray
        };
        /* --------------------- */

        /* --- Check and Add event --- */
        /* --------------------------- */
        call_ajax_method_json("/Installation/ISS070_SendGridSlipDetailsData", obj, function (result, controls) {
            if (controls != undefined) {
                /* --- Higlight Text --- */
                /* --------------------- */
                //VaridateCtrl(["EmailAddress"], controls);
                /* --------------------- */

                return;
            }
            else if (result != undefined) {

            }

        });
    }

    }

    function initialScreenOnRetrieve(result) {
        
        initialCancelCondition(result);
        initialSECOMNPayment(result);
    }

//    function initialSaleman(result) {
//        if (result.ServiceTypeCode == C_SERVICE_TYPE_RENTAL && result.dtRentalContractBasic.ContractStatus == C_CONTRACT_STATUS_BEF_START) {
//            $("#SalesmanCode1").val(result.dtRentalContractBasic.SalesmanCode1);
//            $("#SalesmanCode2").val(result.dtRentalContractBasic.SalesmanCode2);
//            $("#SalesmanEN1").val(result.dtRentalContractBasic.SalesmanEN1);
//            $("#SalesmanEN2").val(result.dtRentalContractBasic.SalesmanEN2);
//        }
//        else if(result.ServiceTypeCode == C_SERVICE_TYPE_SALE)  { 
//            $("#SalesmanCode1").val(result.dtSale.SalesmanCode1);
//            $("#SalesmanCode2").val(result.dtSale.SalesmanCode2);
//            $("#SalesmanEN1").val(result.dtSale.SalesmanEN1);
//            $("#SalesmanEN2").val(result.dtSale.SalesmanEN2);
//        }
//        else
//        {
//            $("#SalesmanCode1").val();
//            $("#SalesmanCode2").val();
//            $("#SalesmanEN1").val();
//            $("#SalesmanEN2").val();
//        }
//    }


    function initialCancelCondition(result) {
        $("#CancelConditionAllInstall").attr("checked", true);
        $("#CancelConditionAllInstall").attr("disabled",false);
        $("#CancelConditionPO").attr("disabled",true);
        $("#CancelConditionPOAndSlip").attr("disabled", true);
        $("#CancelConditionSlip").attr("disabled", true);
        $("#CancelConditionPrevious").attr("disabled", true);
        if(result.ServiceTypeCode == C_SERVICE_TYPE_RENTAL || result.ServiceTypeCode == C_SERVICE_TYPE_SALE)
        {
            if (result.do_TbtInstallationBasic.MaintenanceNo != null && result.doTbt_InstallationPOManagement != null && result.doTbt_InstallationPOManagement.length > 0)
            {
                $("#CancelConditionPO").attr("disabled",false);
            }
            if (result.do_TbtInstallationSlip != null) {
                if (result.do_TbtInstallationBasic.MaintenanceNo != null && result.do_TbtInstallationSlip.SlipNo != null
                 && result.doTbt_InstallationPOManagement != null && result.doTbt_InstallationPOManagement.length > 0
               && result.do_TbtInstallationSlip.PreviousSlipNo == null
               && (result.do_TbtInstallationSlip.SlipStatus == C_SLIP_STATUS_NO_NEED_TO_STOCK_OUT || result.do_TbtInstallationSlip.SlipStatus == C_SLIP_STATUS_NOT_STOCK_OUT)) {
                    $("#CancelConditionPOAndSlip").attr("disabled", false);
                }
                if (result.do_TbtInstallationSlip.SlipNo != null
               && result.do_TbtInstallationSlip.PreviousSlipNo == null
               && result.do_TbtInstallationBasic.MaintenanceNo != null 
               && (result.do_TbtInstallationSlip.SlipStatus == C_SLIP_STATUS_NO_NEED_TO_STOCK_OUT || result.do_TbtInstallationSlip.SlipStatus == C_SLIP_STATUS_NOT_STOCK_OUT)) {
                    $("#CancelConditionSlip").attr("disabled", false);
                }
                if (result.do_TbtInstallationSlip.SlipNo != null
               && result.do_TbtInstallationSlip.PreviousSlipNo != null
               && (result.do_TbtInstallationSlip.SlipStatus == C_SLIP_STATUS_NO_NEED_TO_STOCK_OUT || result.do_TbtInstallationSlip.SlipStatus == C_SLIP_STATUS_NOT_STOCK_OUT)) {
                    $("#CancelConditionPrevious").attr("disabled", false);
                }
            }
        }
        else if(result.ServiceTypeCode == C_SERVICE_TYPE_PROJECT)
        {
            if (result.do_TbtInstallationBasic.MaintenanceNo != null && result.doTbt_InstallationPOManagement != null && result.doTbt_InstallationPOManagement.length > 0)
            {
                $("#CancelConditionPO").attr("disabled",false);
            }
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

//    function manualInitialGridInstrument(chkControl) {
//        
//        //CheckFirstRowIsEmpty(ISS070_GridInstrumentInfo, true);
//        $("#BlnTypeOneTimeOrTemp").val(strFalse);
//        if (ISS070_GridInstrumentInfo.hdr.rows.length > 0) {
//       
//            for (var i = 0; i < ISS070_GridInstrumentInfo.getRowsNum(); i++) {
//                var rowId = ISS070_GridInstrumentInfo.getRowId(i);


//                if (ISS070_GridInstrumentInfo.rowsCol.length != 1)
//                {
//                        var AddRemovedQTYid = GenerateGridControlID("AddRemovedQTYBox", rowId);
//                        var MovedQTYid = GenerateGridControlID("MovedQTYBox", rowId);
//                        var MAExchangeQTYid = GenerateGridControlID("MAExchangeQTYBox", rowId);
//                        var NotInstallQTYid = GenerateGridControlID("NotInstallQTYBox", rowId);
//                        var UnremovableQTYid = GenerateGridControlID("UnremovableQTYBox", rowId);
//                               
//                        var amountAddRemovedQTY = 0;
//                        var amountMovedQTY = 0;
//                        var amountMAExchangeQTY = 0;
//                        var amountNotInstallQTY = 0;
//                        var amountUnremovableQTY = 0;



//                        var blnEnableAddRemovedQTY = false;
//                        var blnEnableMovedQTY = false;
//                        var blnEnableMAExchangeQTY = false;
//                        var blnEnableNotInstallQTY = false;
//                        var blnEnableUnremovableQTY = false;


//                        if ($("#chkCondHave1").prop("checked") == true) {
//                    
//                            blnEnableNotInstallQTY = true;
//                    
//                        }
//                        if ($("#chkCondHave2").prop("checked") == true) {
//                    
//                            blnEnableUnremovableQTY = true;
//                            $("#ChangeApproveNo").attr("disabled", false);
//                        }
//                        if ($("#chkCondHave3").prop("checked") == true) {
//                            blnEnableAddRemovedQTY = true;
//                 
//                        }
//                        if ($("#chkCondHave4").prop("checked") == true) {
//               
//                            blnEnableMovedQTY = true;
//                  
//                        }
//                        if ($("#chkCondHave5").prop("checked") == true) {
//                   
//                            blnEnableMAExchangeQTY = true;
//                 
//                        }  
//                    
//                

//                        //================================= GetColumn Index =================================
//                        var ContractAfterChangeCol = ISS070_GridInstrumentInfo.getColIndexById("ContractInstalledAfterChange");
//                        var ContractCurrentCol = ISS070_GridInstrumentInfo.getColIndexById("ContractInstalledQTY");
//                        var TotalStockCol = ISS070_GridInstrumentInfo.getColIndexById("TotalStockedOut");
//                        var UnUsedCol = ISS070_GridInstrumentInfo.getColIndexById("UnusedQTY");
//                        var RemovedCol = ISS070_GridInstrumentInfo.getColIndexById("RemovedQTY");
//                        var AddRemovedCol = ISS070_GridInstrumentInfo.getColIndexById("AddRemovedQTY");
//                        var MovedCol = ISS070_GridInstrumentInfo.getColIndexById("MoveQTY");
//                        var MAExchangeCol = ISS070_GridInstrumentInfo.getColIndexById("MAExchangeQTY");
//                        var NotInstallCol = ISS070_GridInstrumentInfo.getColIndexById("NotInstallQTY");
//                        var UnremovableCol = ISS070_GridInstrumentInfo.getColIndexById("UnremovableQTY");
//               
//                        //====================================================================================

//                        //=========== AddRemovedQTY ==============================
//                        if (ISS070_GridInstrumentInfo.hdr.rows.length > 0) {
//                            var val = $("#" + AddRemovedQTYid).val();
//                            if (val == undefined) {

//                                val = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, AddRemovedCol);
//                            }
//                            amountAddRemovedQTY = val.replace(/,/g, "");
//                        }                
//                        //========================================================
//                        //=========== MovedQTY ==============================
//                        if (ISS070_GridInstrumentInfo.hdr.rows.length > 0) {
//                            var val = $("#" + MovedQTYid).val();
//                            if (val == undefined) {
//                                val = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, MovedCol);
//                            }
//                            amountMovedQTY = val.replace(/,/g, "");
//                        }                
//                        //========================================================
//                        //=========== MAExchangeQTYid ==============================
//                        if (ISS070_GridInstrumentInfo.hdr.rows.length > 0) {
//                            var val = $("#" + MAExchangeQTYid).val();
//                            if (val == undefined) {
//                                val = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, MAExchangeCol);
//                            }
//                            amountMAExchangeQTY = val.replace(/,/g, "");
//                        }                
//                        //========================================================
//                        //=========== NotInstallQTY ==============================
//                        if (ISS070_GridInstrumentInfo.hdr.rows.length > 0) {
//                            var val = $("#" + NotInstallQTYid).val();
//                            if (val == undefined) {
//                                val = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, NotInstallCol);
//                            }
//                            amountNotInstallQTY = val.replace(/,/g, "");
//                        }                
//                        //========================================================
//                        //=========== UnremovableQTY ==============================
//                        if (ISS070_GridInstrumentInfo.hdr.rows.length > 0) {
//                            var val = $("#" + UnremovableQTYid).val();
//                            if (val == undefined) {
//                                val = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, UnremovableCol);
//                            }
//                            amountUnremovableQTY = val.replace(/,/g, "");
//                        }
//                        //=======================================================

//                        if (chkControl != null) {

//                            //============= GET OLD VALUE ===============================================
//                            var OLDMovedQTYCol = ISS070_GridInstrumentInfo.getColIndexById("OLDMovedQTY");
//                            var OLDMAExchangeQTYCol = ISS070_GridInstrumentInfo.getColIndexById("OLDMAExchangeQTY");
//                            var amounOLDMovedQTYCol = 0;
//                            var amountOLDMAExchangeQTYCol = 0;
//                            amounOLDMovedQTYCol = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, OLDMovedQTYCol);
//                            amountOLDMAExchangeQTYCol = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, OLDMAExchangeQTYCol);
//                            //===========================================================================
//                            blnChecked = $("#" + chkControl.id).prop("checked");
//                            if (chkControl.id == "chkCondHave1" && blnChecked == false) {
//                                amountNotInstallQTY = 0;
//                            } else if (chkControl.id == "chkCondHave2" && blnChecked == false) {
//                                amountUnremovableQTY = 0;
//                                $("#ChangeApproveNo").attr("disabled", true);
//                            } else if (chkControl.id == "chkCondHave3" && blnChecked == false) {
//                                amountAddRemovedQTY = 0;
//                            } else if (chkControl.id == "chkCondHave4" && blnChecked == false) {
//                                amountMovedQTY = amounOLDMovedQTYCol;
//                            } else if (chkControl.id == "chkCondHave5" && blnChecked == false) {
//                                amountMAExchangeQTY = amountOLDMAExchangeQTYCol;
//                            }
//                        }

//                        //====================== Calculate Contract Installed After CHANGE ================
//                

//                        var amountContractInstalledCurrent = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, ContractCurrentCol);
//                        var amountTotalStock = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, TotalStockCol);
//                        var amountUnUsed = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, UnUsedCol);
//                        var amountRemove = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, RemovedCol);
//                        //var amountAddRemove = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, AddRemovedCol);

//                        var amountContractInstallAfterChange = (amountContractInstalledCurrent * 1) + (amountTotalStock * 1) - (amountUnUsed * 1) - (amountRemove * 1) - (amountAddRemovedQTY * 1);
//                        ISS070_GridInstrumentInfo.cells2(i, ContractAfterChangeCol).setValue(amountContractInstallAfterChange+"");
//                        //Contract installed (after change)] = [Contract installed (Current)] + [Total stock-out instrument qty]
//                        // - [Unused instrument qty] - [Removed instrument qty] – [Additional removed qty];
//                        //=================================================================================

//                        //==================== Generate Numeric Box ==============================
//                        GenerateNumericBox2(ISS070_GridInstrumentInfo, "AddRemovedQTYBox", rowId, "AddRemovedQTY", amountAddRemovedQTY, 10, 0, 0, 9999999999, true, blnEnableAddRemovedQTY);
//                        $("#" + AddRemovedQTYid).css('width', '50px');
//                        $("#" + AddRemovedQTYid).blur(function () {
//                            manualInitialGridInstrument(null);
//                        });

//                        GenerateNumericBox2(ISS070_GridInstrumentInfo, "MovedQTYBox", rowId, "MovedQTY", amountMovedQTY, 10, 0, 0, 9999999999, true, blnEnableMovedQTY);
//                        $("#" + MovedQTYid).css('width', '50px');
//                        $("#" + MovedQTYid).blur(function () {
//                            //manualInitialGridInstrument(null);
//                        });

//                        GenerateNumericBox2(ISS070_GridInstrumentInfo, "MAExchangeQTYBox", rowId, "MAExchangeQTY", amountMAExchangeQTY, 10, 0, 0, 9999999999, true, blnEnableMAExchangeQTY);
//                        $("#" + MAExchangeQTYid).css('width', '50px');
//                        $("#" + MAExchangeQTYid).blur(function () {
//                            //manualInitialGridInstrument(null);
//                        });

//                        GenerateNumericBox2(ISS070_GridInstrumentInfo, "NotInstallQTYBox", rowId, "NotInstallQTY", amountNotInstallQTY, 10, 0, 0, 9999999999, true, blnEnableNotInstallQTY);
//                        $("#" + NotInstallQTYid).css('width', '50px');
//                        $("#" + NotInstallQTYid).blur(function () {
//                            //manualInitialGridInstrument(null);
//                        });

//                        GenerateNumericBox2(ISS070_GridInstrumentInfo, "UnremovableQTYBox", rowId, "UnremovableQTY", amountUnremovableQTY, 10, 0, 0, 9999999999, true, blnEnableUnremovableQTY);
//                        $("#" + UnremovableQTYid).css('width', '50px');
//                        $("#" + UnremovableQTYid).blur(function () {
//                            //manualInitialGridInstrument(null);
//                        });
//                        //========================================================================
//                                
//                        ISS070_GridInstrumentInfo.setSizes();
//                }
//            }
//            
//        }
//    }


    function manualInitialGridInstrument() {

        $("#BlnHaveNewRow").val(strFalse);
        $("#BlnTypeOneTimeOrTemp").val(strFalse);
        if (ISS070_GridInstrumentInfo.hdr.rows.length > 0) {

            for (var i = 0; i < ISS070_GridInstrumentInfo.getRowsNum(); i++) {
                var rowId = ISS070_GridInstrumentInfo.getRowId(i);
               

                var AddInstalledQTYid = GenerateGridControlID("AddInstalledQTYBox", rowId);
                var ReturnQTYid = GenerateGridControlID("ReturnQTYBox", rowId);
                var AddRemovedQTYid = GenerateGridControlID("AddRemovedQTYBox", rowId);
                var MoveQTYid = GenerateGridControlID("MoveQTYBox", rowId);
                var MAExchangeQTYid = GenerateGridControlID("MAExchangeQTYBox", rowId);

                var amountContractInstallCurrent = 0;
                var amountAdditionalInstalled = 0;
                var amountRemoved = 0;
                var amountReturn = 0;
                var amountMove = 0;
                var amountMAExch = 0;

                
                //============ AddInstall QTY ==============
                if (ISS070_GridInstrumentInfo.hdr.rows.length > 0) {
                    var val = $("#" + AddInstalledQTYid).val();
                    if (val == undefined) {
                       
                        val = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, 4);
                    }
                    amountAdditionalInstalled = val.replace(/,/g, "");
                }                
                //============================================
                //============== Return QTY ==================
                if (ISS070_GridInstrumentInfo.hdr.rows.length > 0) {
                    val = $("#" + ReturnQTYid).val();
                    if (val == undefined) {
                        val = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, 5);
                    }
                }
                amountReturn = val.replace(/,/g, "");
               
                //=============================================
                //=============== Removed QTY =================
                if (ISS070_GridInstrumentInfo.hdr.rows.length > 0) {
                    val = $("#" + AddRemovedQTYid).val();
                    if (val == undefined) {
                        val = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, 6);
                    }
                }
                amountRemoved = val.replace(/,/g, "");
                
                //==============================================
                //================ Move QTY ====================
                if (ISS070_GridInstrumentInfo.hdr.rows.length > 0) {
                    val = $("#" + MoveQTYid).val();
                    if (val == undefined) {
                        val = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, 8);
                    }
                    amountMove = val.replace(/,/g, "");
                }
               

                //===============================================
                //================= MA Exch QTY =================
                if (ISS070_GridInstrumentInfo.hdr.rows.length > 0) {
                    val = $("#" + MAExchangeQTYid).val();
                    if (val == undefined && ISS070_GridInstrumentInfo.hdr.rows.length > 0) {
                        val = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, 9);
                    }
                    amountMAExch = val.replace(/,/g, "");
                }
                               
                //================================================
                //================ TotalStockOutQty ==============
                var TotalStockedOutCol = ISS070_GridInstrumentInfo.getColIndexById("TotalStockedOut");

                var amountTotalStockedOut = ISS070_GridInstrumentInfo.cells2(i, TotalStockedOutCol).getValue();
                amountTotalStockedOut = amountTotalStockedOut.replace(/,/g, "");
                //================================================

                amountContractInstallCurrent = ISS070_GridInstrumentInfo.cells2(i, 2).getValue().replace(/,/g, "");
                if (amountContractInstallCurrent == "") { amountContractInstallCurrent = 0; }
                if (amountAdditionalInstalled == "") { amountAdditionalInstalled = 0; }
                if (amountRemoved == "") { amountRemoved = 0; }
                if (amountReturn == "") { amountReturn = 0; }
                if (amountTotalStockedOut == "") { amountTotalStockedOut = 0; }

                //ISS070_GridInstrumentInfo.cells2(i, 7).setValue(qtyConvert((amountContractInstallCurrent * 1) + (amountAdditionalInstalled * 1) - (amountReturn * 1) - (amountRemoved * 1) + (amountTotalStockedOut * 1)));

               

               
                ISS070_GridInstrumentInfo.setSizes();
            }

        }
    }

    function ChangeInstallationType() {
        if($("#OldInstallationType").val() != "" )
        {
            DeleteAllRow(ISS070_GridInstrumentInfo);
        }
        
        
        if($("#InstallationType").val() != "")
        {
            $("#divInstallationInstrumentInfo").show();
            manualInitialGridInstrument(null);
        }
        else
        {
            $("#divInstallationInstrumentInfo").hide();
        }
        $("#OldInstallationType").val($("#InstallationType").val());

    }

    function ResetAdditional() {

        DeleteAllRow(ISS070_GridInstrumentInfo);
        var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
        call_ajax_method_json("/Installation/ISS070_ClearInstrumentInfo", obj, function (result, controls) {


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

        manualInitialGridInstrument(this);
    }

//    function initialInstrumentDetail(strInstallType) { 
//        
//        if( strInstallType == C_RENTAL_INSTALL_TYPE_NEW || strInstallType == C_SALE_INSTALL_TYPE_NEW || strInstallType == C_SALE_INSTALL_TYPE_ADD )
//        {
//            $("#divInstallationInstrumentInfo").show();
//            $("#divCondHave1").hide();
//            $("#divCondHave2").hide();
//            $("#ChangeApproveNo").hide();
//            $("#divCondHave3").hide();
//            $("#divCondHave4").hide();
//            $("#divCondHave5").hide();
//        }
//        else if(strInstallType == C_RENTAL_INSTALL_TYPE_CHANGE_WIRING || strInstallType ==  C_SALE_INSTALL_TYPE_CHANGE_WIRING)
//        {
//            $("#divInstallationInstrumentInfo").hide();
//        }
//        else if(strInstallType == C_RENTAL_INSTALL_TYPE_MOVE || strInstallType ==  C_SALE_INSTALL_TYPE_MOVE)
//        {
//            $("#divInstallationInstrumentInfo").show();
//            $("#divCondHave1").hide();
//            $("#divCondHave2").hide();
//            $("#ChangeApproveNo").hide();
//            $("#divCondHave3").hide();
//            $("#divCondHave4").show();
//            $("#divCondHave5").hide();
//        }
//        else if(strInstallType == C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE || strInstallType ==  C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE)
//        {
//            $("#divInstallationInstrumentInfo").show();
//            $("#divCondHave1").hide();
//            $("#divCondHave2").hide();
//            $("#ChangeApproveNo").hide();
//            $("#divCondHave3").hide();
//            $("#divCondHave4").hide();
//            $("#divCondHave5").show();
//        }       
//        else
//        {
//            $("#divInstallationInstrumentInfo").show();
//            $("#divCondHave1").show();
//            $("#divCondHave2").show();
//            $("#ChangeApproveNo").show();
//            $("#divCondHave3").show();
//            $("#divCondHave4").hide();
//            $("#divCondHave5").hide();
//        }

//    }

//    function initialInstallationFeeBilling(strInstallType) {

//        if (strInstallType == C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
//            strInstallType == C_RENTAL_INSTALL_TYPE_MOVE ||
//            strInstallType == C_RENTAL_INSTALL_TYPE_CHANGE_WIRING ||
//            strInstallType == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
//            strInstallType == C_SALE_INSTALL_TYPE_MOVE ||
//            strInstallType == C_SALE_INSTALL_TYPE_CHANGE_WIRING ||
//            strInstallType == C_SALE_INSTALL_TYPE_PARTIAL_REMOVE ||
//            strInstallType == C_SALE_INSTALL_TYPE_REMOVE_ALL) {
//            $("#NewBillingInstallFee").attr("disabled", false);
//            $("#NewBillingOCC").attr("disabled", false);

//        }
//        else {
//            $("#NewBillingInstallFee").attr("disabled", true);
//            $("#NewBillingOCC").attr("disabled", true); 
//        }

//    }

function ValidateInstrumentDetail() {
    var blnValidateInstrument = true; 
        for (var i = 0; i < ISS070_GridInstrumentInfo.getRowsNum(); i++) {
            var NotInstallQTYCol = ISS070_GridInstrumentInfo.getColIndexById("NotInstallQTY");
            var TotalStockedOutCol = ISS070_GridInstrumentInfo.getColIndexById("TotalStockedOut");
            var UnremovableQTYCol = ISS070_GridInstrumentInfo.getColIndexById("UnremovableQTY");
            var AddRemovedQTYCol = ISS070_GridInstrumentInfo.getColIndexById("AddRemovedQTY");
            var RemovedQTYCol = ISS070_GridInstrumentInfo.getColIndexById("RemovedQTY");
            var MAExchangeQTYCol = ISS070_GridInstrumentInfo.getColIndexById("MAExchangeQTY");
            var ContractInstalledAfterChangeCol = ISS070_GridInstrumentInfo.getColIndexById("ContractInstalledAfterChange");
            var ContractInstalledQTYCol = ISS070_GridInstrumentInfo.getColIndexById("ContractInstalledQTY");
            var UnusedQTYCol = ISS070_GridInstrumentInfo.getColIndexById("UnusedQTY");
                   
            var rowId = ISS070_GridInstrumentInfo.getRowId(i);

            var NotInstallQTYid = GenerateGridControlID("NotInstallQTYBox", rowId);                
            var UnremovableQTYid = GenerateGridControlID("UnremovableQTYBox", rowId);
            var AddRemovedQTYid = GenerateGridControlID("AddRemovedQTYBox", rowId);
            var RemovedQTYid = GenerateGridControlID("RemovedQTYBox", rowId);
            var MAExchangeQTYid = GenerateGridControlID("MAExchangeQTYBox", rowId);
            var ContractInstalledQTYid = GenerateGridControlID("ContractInstalledQTYBox", rowId);
            var RemovedQTYid = GenerateGridControlID("RemovedQTYBox", rowId);
            var UnusedQTYid = GenerateGridControlID("UnusedQTYBox", rowId);

            var amountNotInstallQTY = 0;
            var amountTotalStockedOut = 0;
            var amountUnremovableQTY = 0;
            var amountAddRemovedQTY = 0;
            var amountRemovedQTY = 0;
            var amountMAExchangeQTY = 0;
            var amountContractInstalledAfterChange = 0;
            var amountContractInstalledQTY = 0;
            var amountUnusedQTY = 0;

            //============ amountNotInstallQTY ==============
            amountNotInstallQTY = $("#" + NotInstallQTYid).val();
            if (amountNotInstallQTY == undefined) {
                amountNotInstallQTY = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, NotInstallQTYCol);
            }
            amountNotInstallQTY = amountNotInstallQTY.replace(",","")
            //============ amountTotalStockedOut ==============
            amountTotalStockedOut = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, TotalStockedOutCol);
            amountTotalStockedOut = amountTotalStockedOut.replace(",","")
            //============ amountUnremovableQTY ==============
            amountUnremovableQTY = $("#" + UnremovableQTYid).val();
            if (amountUnremovableQTY == undefined) {
                amountUnremovableQTY = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, UnremovableQTYCol);
            }
            amountUnremovableQTY = amountUnremovableQTY.replace(",", "")
            //============ amountAddRemovedQTY ==============
            amountAddRemovedQTY = $("#" + AddRemovedQTYid).val();
            if (amountAddRemovedQTY == undefined) {
                amountAddRemovedQTY = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, AddRemovedQTYCol);
            }
            amountAddRemovedQTY = amountAddRemovedQTY.replace(",", "")
            //============ amountRemovedQTY ==============
            amountRemovedQTY = $("#" + RemovedQTYid).val();
            if (amountRemovedQTY == undefined) {
                amountRemovedQTY = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, RemovedQTYCol);
            }
            amountRemovedQTY = amountRemovedQTY.replace(",", "")
            //============ amountMAExchangeQTY ==============
            amountMAExchangeQTY = $("#" + MAExchangeQTYid).val();
            if (amountMAExchangeQTY == undefined) {
                amountMAExchangeQTY = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, MAExchangeQTYCol);
            }
            amountMAExchangeQTY = amountMAExchangeQTY.replace(",", "")

            //============ amountContractInstalledAfterChange ==============
            amountContractInstalledAfterChange = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, ContractInstalledAfterChangeCol);
            amountContractInstalledAfterChange = amountContractInstalledAfterChange.replace(",","")
            //============ amountContractInstalledQTY ==============
            amountContractInstalledQTY = $("#" + ContractInstalledQTYid).val();
            if (amountContractInstalledQTY == undefined) {
                amountContractInstalledQTY = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, ContractInstalledQTYCol);
            }
            amountMAExchangeQTY = amountMAExchangeQTY.replace(",", "")
            //============ amountUnusedQTY ==============
            amountUnusedQTY = $("#" + amountUnusedQTY).val();
            if (amountUnusedQTY == undefined) {
                amountUnusedQTY = GetValueFromLinkType(ISS070_GridInstrumentInfo, i, UnusedQTYCol);
            }
            amountUnusedQTY = amountUnusedQTY.replace(",", "")
            //===========================================

            amountNotInstallQTY = amountNotInstallQTY*1;
            amountTotalStockedOut = amountTotalStockedOut*1;
            amountUnremovableQTY = amountUnremovableQTY*1;
            amountAddRemovedQTY = amountAddRemovedQTY*1;
            amountRemovedQTY = amountRemovedQTY*1;
            amountMAExchangeQTY = amountMAExchangeQTY*1;
            amountContractInstalledAfterChange = amountContractInstalledAfterChange*1;
            amountContractInstalledQTY = amountContractInstalledQTY*1;
            amountUnusedQTY = amountUnusedQTY*1;


            if ($("#chkCondHave1").prop("checked") == true && amountNotInstallQTY > amountTotalStockedOut)                       
            {
                doAlert("Installation", "MSG5040", "Installation");
                ISS070_GridInstrumentInfo.selectRow(ISS070_GridInstrumentInfo.getRowIndex(rowId));
                blnValidateInstrument = false;
            }
            //if ($("#chkCondHave2").prop("checked") == true && (amountUnremovableQTY > (amountAddRemovedQTY + amountRemovedQTY) || amountUnremovableQTY > amountMAExchangeQTY))
            if ($("#chkCondHave2").prop("checked") == true && (amountUnremovableQTY > (amountAddRemovedQTY + amountRemovedQTY)))
            {
                doAlert("Installation", "MSG5041", "Installation");
                ISS070_GridInstrumentInfo.selectRow(ISS070_GridInstrumentInfo.getRowIndex(rowId));
                blnValidateInstrument = false;
            }
                    
            if( amountContractInstalledAfterChange != (amountContractInstalledQTY + amountTotalStockedOut - amountUnusedQTY - amountNotInstallQTY - amountRemovedQTY - amountAddRemovedQTY  ))
            {
                doAlert("Installation", "MSG5044", "Installation");
                ISS070_GridInstrumentInfo.selectRow(ISS070_GridInstrumentInfo.getRowIndex(rowId));
                blnValidateInstrument = false;
            }

            if ($("#chkCondHave2").prop("checked") == true
            && ($("#InstallationTypeCode").val() == C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE || $("#InstallationTypeCode").val() == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE)
            && amountUnremovableQTY > amountMAExchangeQTY) {
                doAlert("Installation", "MSG5073", "Installation");
                ISS070_GridInstrumentInfo.selectRow(ISS070_GridInstrumentInfo.getRowIndex(rowId));
                blnValidateInstrument = false;
            }

            


        }
        return blnValidateInstrument;
    }

    function clear_installation_click() {

        // Get Message
        var obj = {
            module: "Common",
            code: "MSG0044"
        };

        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
            OpenYesNoMessageDialog(result.Code, result.Message, clearAllScreen, function () {
                
            });

        });

    }

    function qtyConvert(value) {
        if (value != null) {
            var buf = "";
            var sBuf = "";
            var j = 0;
            value = String(value);

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
                value = sBuf + value.substring(value.indexOf("."));
            }
            else {
                value = sBuf;
            }
            return value;
        }
    }

    function moneyConvert(value) {
        if (value != null) {
            var buf = "";
            var sBuf = "";
            var j = 0;
            value = String(value);

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
                value = sBuf + value.substring(value.indexOf("."),value.indexOf(".") + 3);
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
    