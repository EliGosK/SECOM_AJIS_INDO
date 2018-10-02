var CTS140 = {
    MaintainScreenItems: function () {
        call_ajax_method_json("/Contract/CTS140_ValidateEnteringCondition", "",
            function (result, controls) {
                if (result != undefined) {
                    CTS140.DisabledSection(true);
                    CTS140.HideCommandControl();
                }
                else {
                    CTS140_01.InitialControl();
                    CTS140_02.InitialControl();
                    CTS140_03.InitialControl();
                    CTS140_04.InitialControl();
                    CTS140_05.InitialControl();
                    CTS140_06.InitialControl();
                    CTS140_07.InitialControl();
                    CTS140_08.InitialControl();
                    CTS140_09.InitialControl();
                    CTS140_10.InitialControl();

                    CTS140.RegisterCommandControl();
                }
            }
        );

        $("#divContractBasicSection").show();
        $("#divContractRelatedFeeInformationMain").show();
        $("#divContractRelation").show();
        $("#divContractAgreement").show();
        $("#divDepositInformation").show();
        $("#divContractDocumentInformation").show();
        $("#divProviderServiceInformation").hide();
        $("#AlarmProvidedServiceType").hide();
        $("#divSiteInformation").hide();
        $("#divCancelContractCondition").hide();
        $("#divMaintenanceInformation").hide();
        $("#divOtherInformation").show();

        call_ajax_method_json("/Contract/CTS140_GetConstantHideShowDIV", "",
            function (result, controls) {
                if (result != undefined) {
                    if (result.ProductTypeCode == result.C_PROD_TYPE_AL
                        || result.ProductTypeCode == result.C_PROD_TYPE_RENTAL_SALE) {
                        $("#divProviderServiceInformation").show();
                        $("#AlarmProvidedServiceType").show();
                        $("#divSiteInformation").show();

                        if (result.ContractStatus == result.C_CONTRACT_STATUS_CANCEL
                            || result.ContractStatus == result.C_CONTRACT_STATUS_END) {
                            $("#divCancelContractCondition").show();
                        }
                    }
                    else if (result.ProductTypeCode == result.C_PROD_TYPE_MA) {
                        $("#divMaintenanceInformation").show();
                        $("#divSiteInformation").show();
                        if (result.ContractStatus == result.C_CONTRACT_STATUS_CANCEL
                            || result.ContractStatus == result.C_CONTRACT_STATUS_END) {
                            $("#divCancelContractCondition").show();
                        }
                    }
                    else if (result.ProductTypeCode == result.C_PROD_TYPE_SG
                            || result.ProductTypeCode == result.C_PROD_TYPE_BE) {
                        if (result.ContractStatus == result.C_CONTRACT_STATUS_CANCEL
                            || result.ContractStatus == result.C_CONTRACT_STATUS_END) {
                            $("#divCancelContractCondition").show();
                        }
                    }
                    else if (result.ProductTypeCode == result.C_PROD_TYPE_ONLINE) {
                        $("#divProviderServiceInformation").show();
                        $("#AlarmProvidedServiceType").show();
                        $("#divSiteInformation").show();

                        $("#RelatedContractCode").attr("readonly", false);

                        if (result.ContractStatus == result.C_CONTRACT_STATUS_CANCEL
                            || result.ContractStatus == result.C_CONTRACT_STATUS_END) {
                            $("#divCancelContractCondition").show();
                        }
                    }
                }
            }
        );
    },
    DisabledSection: function (isDisabled) {
        CTS140_01.DisabledSection(isDisabled);
        CTS140_02.DisabledSection(isDisabled);
        CTS140_03.DisabledSection(isDisabled);
        CTS140_04.DisabledSection(isDisabled);
        CTS140_05.DisabledSection(isDisabled);
        CTS140_06.DisabledSection(isDisabled);
        CTS140_07.DisabledSection(isDisabled);
        CTS140_08.DisabledSection(isDisabled);
        CTS140_09.DisabledSection(isDisabled);
        CTS140_10.DisabledSection(isDisabled);
    },
    SetSectionMode: function (isView) {
        CTS140_01.SetSectionMode(isView);
        CTS140_02.SetSectionMode(isView);
        CTS140_03.SetSectionMode(isView);
        CTS140_04.SetSectionMode(isView);
        CTS140_05.SetSectionMode(isView);
        CTS140_06.SetSectionMode(isView);
        CTS140_07.SetSectionMode(isView);
        CTS140_08.SetSectionMode(isView);
        CTS140_09.SetSectionMode(isView);
        CTS140_10.SetSectionMode(isView);
    },

    /* --- Command ----------------------------------------------------- */
    GetValidateObject: function () {

        var billingType = $("#FeeType").val();

        var feeAmountCurrencyType = $("#FeeAmount").NumericCurrencyValue();
        var feeAmount = $("#FeeAmount").val();

        var contractCode_CounterBalance = $("#ContractCode_CounterBalance").val();
        var handlingType = $("#HandlingType").val();

        var taxAmountCurrencyType = $("#TaxAmount").NumericCurrencyValue();
        var taxAmount = $("#TaxAmount").val();

        var normalFeeAmountCurrencyType = $("#NormalFeeAmount").NumericCurrencyValue();
        var normalFeeAmount = $("#NormalFeeAmount").val();

        var startPeriodDate = $("#StartPeriodDateContract").val();
        var endPeriodDate = $("#EndPeriodDateContract").val();
        var remark = $("#Remark").val();
        var feeType = jQuery.trim($("#FeeType").val());

        var isContractCancelShow;
        if ($("#divCancelContractCondition").css('display') == "block") {
            isContractCancelShow = true;
        }
        else {
            isContractCancelShow = false;
        }

        //Check visible section------------------------------------------------------------

        //ContractRelation
        var firstSecurityStartDate = $("#FirstSecurityStartDate").val(); //Add by Jutarat A. on 18102013
        var startDealDate = $("#StartDealDate").val();
        var contractStartDate = $("#ContractStartDate").val();
        var contractEndDate = $("#ContractEndDate").val();
        var contractDurationMonth = $("#ContractDurationMonth").val();
        var autoRenewMonth = $("#AutoRenewMonth").val();
        var projectCode = $("#ProjectCode").val();

        if ($("#divContractRelation").css('display') != "block") {
            firstSecurityStartDate = "none"; //Add by Jutarat A. on 18102013
            startDealDate = "none";
            contractStartDate = "none";
            contractEndDate = "none";
            contractDurationMonth = "none";
            autoRenewMonth = "none";
            projectCode = "none";
        }

        //------------------------------------------------------------

        //ContractAgreementInformation                
        var contractOfficeCode = $("#ContractOfficeCode").val();
        var planCode = $("#PlanCode").val();
        var salesmanEmpNo1 = $("#SalesmanEmpNo1").val();
        var salesmanEmpNo2 = $("#SalesmanEmpNo2").val();
        var oldContractCode = $("#OldContractCode").val();
        var acquisitionTypeCode = $("#AcquisitionTypeCode").val();
        var motivationTypeCode = $("#MotivationTypeCode").val();
        var introducerCode = $("#IntroducerCode").val();
        var salesSupporterEmpNo = $("#SalesSupporterEmpNo").val();

        if ($("#divContractAgreement").css('display') != "block") {
            contractOfficeCode = "none";
            planCode = "none";
            salesmanEmpNo1 = "none";
            salesmanEmpNo2 = "none";
            oldContractCode = "none";
            acquisitionTypeCode = "none";
            motivationTypeCode = "none";
            introducerCode = "none";
            salesSupporterEmpNo = "none";
        }

        //------------------------------------------------------------

        //Deposit Information
        var normalDepositFeeCurrencyType = $("#NormalDepositFee").NumericCurrencyValue();
        var normalDepositFee = $("#NormalDepositFee").NumericValue();

        var orderDepositFeeCurrencyType = $("#OrderDepositFee").NumericCurrencyValue();
        var orderDepositFee = $("#OrderDepositFee").NumericValue();

        var exemptedDepositFeeCurrencyType = $("#ExemptedDepositFee").NumericCurrencyValue();
        var exemptedDepositFee = $("#ExemptedDepositFee").NumericValue();

        var counterBalanceOriginContractCode = $("#CounterBalanceOriginContractCode").val();

        if ($("#divDepositInformation").css('display') != "block") {
            normalDepositFee = "none";
            orderDepositFee = "none";
            exemptedDepositFee = "none";
            counterBalanceOriginContractCode = "none";
        }

        //------------------------------------------------------------

        //Contract document information
        var irregurationDocUsageFlag = $("#IrregurationDocUsageFlag").prop("checked"); //$("#IrregurationDocUsageFlag").val();

        if ($("#divContractDocumentInformation").css('display') != "block") {
            irregurationDocUsageFlag = "none";
        }

        //------------------------------------------------------------

        //Provide service information
        var fireMonitorFlag = $("#FireMonitoringFlag").prop("checked");
        var crimePreventFlag = $("#CrimePreventFlag").prop("checked");
        var emergencyReportFlag = $("#EmergencyReportFlag").prop("checked");
        var facilityMonitorFlag = $("#FacilityMonitorFlag").prop("checked");
        var phoneLineTypeCode1 = $("#TelephoneLineType1").val();
        var phoneLineOwnerTypeCode1 = $("#TelephoneLineOwner1").val();
        var phoneNo1 = $("#TelephoneNo1").val();
        var phoneLineTypeCode2 = $("#TelephoneLineType2").val();
        var phoneLineOwnerTypeCode2 = $("#TelephoneLineOwner2").val();
        var phoneNo2 = $("#TelephoneNo2").val();
        var phoneLineTypeCode3 = $("#TelephoneLineType3").val();
        var phoneLineOwnerTypeCode3 = $("#TelephoneLineOwner3").val();
        var phoneNo3 = $("#TelephoneNo3").val();
        var quotationNo = $("#QuotationNo").val();

        if ($("#divProviderServiceInformation").css('display') != "block") {
            fireMonitorFlag = "none";
            crimePreventFlag = "none";
            emergencyReportFlag = "none";
            facilityMonitorFlag = "none";
            phoneLineTypeCode1 = "none";
            phoneLineOwnerTypeCode1 = "none";
            phoneNo1 = "none";
            phoneLineTypeCode2 = "none";
            phoneLineOwnerTypeCode2 = "none";
            phoneNo2 = "none";
            phoneLineTypeCode3 = "none";
            phoneLineOwnerTypeCode3 = "none";
            phoneNo3 = "none";
        }

        //------------------------------------------------------------

        //Maintenanece Information
        var maintenanceTypeCode = $("#MaintenanceTypeCode").val();
        var maintenanceCycle = $("#MaintenanceCycle").val();
        var maintenanceContractStartMonth = $("#Month").val();
        var maintenanceContractStartYear = $("#Year").val();
        var maintenanceFeeTypeCode = $("#MaintenanceFeeType").val();

        if ($("#divMaintenanceInformation").css('display') != "block") {
            maintenanceTypeCode = "none";
            maintenanceCycle = "none";
            maintenanceContractStartMonth = "none";
            maintenanceContractStartYear = "none";
            maintenanceFeeTypeCode = "none";
        }

        //------------------------------------------------------------

        //Site Information
        var buildingTypeCode = $("#BuildingTypeCode").val();
        var siteBuildingArea = $("#SiteBulidingArea").NumericValue();
        var numOfBuilding = $("#NumberOfBuilding").NumericValue();
        var securityAreaFrom = $("#SecurityAreaFrom").NumericValue();
        var securityAreaTo = $("#SecurityAreaTo").NumericValue();
        var numOfFloor = $("#NumOfFloor").NumericValue();
        var mainStructureTypeCode = $("#MainStructureTypeCode").val();

        if ($("#divSiteInformation").css('display') != "block") {
            buildingTypeCode = "none";
            siteBuildingArea = "none";
            numOfBuilding = "none";
            securityAreaFrom = "none";
            securityAreaTo = "none";
            numOfFloor = "none";
            mainStructureTypeCode = "none";
        }

        //------------------------------------------------------------

        //Cancel Contract Conditoin
        var processAfterCounterBalanceType = null;
        if ($("#divProcessCounterBalance #Refund").prop("checked") == true) {
            processAfterCounterBalanceType = objCTS140.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND;
        }

        if ($("#divProcessCounterBalance #ReceiveAsRevenue").prop("checked") == true) {
            processAfterCounterBalanceType = objCTS140.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE;
        }

        if ($("#divProcessCounterBalance #Bill").prop("checked") == true) {
            processAfterCounterBalanceType = objCTS140.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL;
        }

        if ($("#divProcessCounterBalance #Exempt").prop("checked") == true) {
            processAfterCounterBalanceType = objCTS140.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT;
        }

        var processAfterCounterBalanceTypeUsd = null;
        if ($("#divProcessCounterBalanceUsd #RefundUsd").prop("checked") == true) {
            processAfterCounterBalanceTypeUsd = objCTS140.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND;
        }

        if ($("#divProcessCounterBalanceUsd #ReceiveAsRevenueUsd").prop("checked") == true) {
            processAfterCounterBalanceTypeUsd = objCTS140.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE;
        }

        if ($("#divProcessCounterBalanceUsd #BillUsd").prop("checked") == true) {
            processAfterCounterBalanceTypeUsd = objCTS140.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL;
        }

        if ($("#divProcessCounterBalanceUsd #ExemptUsd").prop("checked") == true) {
            processAfterCounterBalanceTypeUsd = objCTS140.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT;
        }

        //------------------------------------------------------------

        //Other Infromation
        var memo = $("#Memo").val();
        if ($("#divOtherInformation").css('display') != "block") {
            memo = "none";
        }

        //------------------------------------------------------------------------------

        var isShowContractRelatedInformation;
        var isShowContractAgreementInformation;
        var isShowDepositInformation;
        var isShowContractDocumentInformation;
        var isShowProvideServiceInformation;
        var isShowMaintenanceInformation;
        var isShowSiteInformation;
        var isShowCancelContractCondition;

        if ($("#divContractRelation").css('display') == "block")
            isShowContractRelatedInformation = true;
        else
            isShowContractRelatedInformation = false;

        if ($("#divContractAgreement").css('display') == "block")
            isShowContractAgreementInformation = true;
        else
            isShowContractAgreementInformation = false;

        if ($("#divDepositInformation").css('display') == "block")
            isShowDepositInformation = true;
        else
            isShowDepositInformation = false;

        if ($("#divContractDocumentInformation").css('display') == "block")
            isShowContractDocumentInformation = true;
        else
            isShowContractDocumentInformation = false;

        if ($("#divProviderServiceInformation").css('display') == "block")
            isShowProvideServiceInformation = true;
        else
            isShowProvideServiceInformation = false;

        if ($("#divMaintenanceInformation").css('display') == "block")
            isShowMaintenanceInformation = true;
        else
            isShowMaintenanceInformation = false;

        if ($("#divSiteInformation").css('display') == "block")
            isShowSiteInformation = true;
        else
            isShowSiteInformation = false;

        if ($("#divCancelContractCondition").css('display') == "block")
            isShowCancelContractCondition = true;
        else
            isShowCancelContractCondition = false;

        var validateRequireField = {
            BillingType: billingType,

            FeeAmountCurrencyType: feeAmountCurrencyType,
            FeeAmount: feeAmount,

            ContractCode_CounterBalance: contractCode_CounterBalance,
            HandlingType: handlingType,

            TaxAmountCurrencyType: taxAmountCurrencyType,
            TaxAmount: taxAmount,

            NormalFeeAmountCurrencyType: normalFeeAmountCurrencyType,
            NormalFeeAmount: normalFeeAmount,

            StartPeriodDate: startPeriodDate,
            EndPeriodDate: endPeriodDate,
            Remark: remark,
            IsContractCancelShow: isContractCancelShow,

            FirstSecurityStartDate: firstSecurityStartDate, //Add by Jutarat A. on 18102013
            StartDealDate: startDealDate,
            ContractStartDate: contractStartDate,
            ContractEndDate: contractEndDate,
            ContractDurationMonth: contractDurationMonth,
            AutoRenewMonth: autoRenewMonth,
            ProjectCode: projectCode,

            //ContractAgreementInformation                
            ContractOfficeCode: contractOfficeCode,
            PlanCode: planCode,
            SalesmanEmpNo1: salesmanEmpNo1,
            SalesmanEmpNo2: salesmanEmpNo2,
            OldContractCode: oldContractCode,
            AcquisitionTypeCode: acquisitionTypeCode,
            MotivationTypeCode: motivationTypeCode,
            IntroducerCode: introducerCode,
            SalesSupporterEmpNo: salesSupporterEmpNo,
            QuotationNo: quotationNo,

            //Deposit Information
            NormalDepositFeeCurrencyType: normalDepositFeeCurrencyType,
            NormalDepositFee: normalDepositFee,

            OrderDepositFeeCurrencyType: orderDepositFeeCurrencyType,
            OrderDepositFee: orderDepositFee,

            ExemptedDepositFeeCurrencyType: exemptedDepositFeeCurrencyType,
            ExemptedDepositFee: exemptedDepositFee,

            CounterBalanceOriginContractCode: counterBalanceOriginContractCode,

            //Contract document information
            IrregurationDocUsageFlag: irregurationDocUsageFlag,

            //Provide service information
            FireMonitorFlag: fireMonitorFlag,
            CrimePreventFlag: crimePreventFlag,
            EmergencyReportFlag: emergencyReportFlag,
            FacilityMonitorFlag: facilityMonitorFlag,
            PhoneLineTypeCode1: phoneLineTypeCode1,
            PhoneLineOwnerCode1: phoneLineOwnerTypeCode1,
            PhoneNo1: phoneNo1,
            PhoneLineTypeCode2: phoneLineTypeCode2,
            PhoneLineOwnerCode2: phoneLineOwnerTypeCode2,
            PhoneNo2: phoneNo2,
            PhoneLineTypeCode3: phoneLineTypeCode3,
            PhoneLineOwnerCode3: phoneLineOwnerTypeCode3,
            PhoneNo3: phoneNo3,

            //Maintenanece Information
            MaintenanceTypeCode: maintenanceTypeCode,
            MaintenanceCycle: maintenanceCycle,
            MaintenanceContractStartMonth: maintenanceContractStartMonth,
            MaintenanceContractStartYear: maintenanceContractStartYear,
            MaintenanceFeeTypeCode: maintenanceFeeTypeCode,

            //Site Information
            BuildingTypeCode: buildingTypeCode,
            SiteBuildingArea: siteBuildingArea,
            NumOfBuilding: numOfBuilding,
            SecurityAreaFrom: securityAreaFrom,
            SecurityAreaTo: securityAreaTo,
            NumOfFloor: numOfFloor,
            MainStructureTypeCode: mainStructureTypeCode,

            //Cancel Contract Conditoin
            ProcessAfterCounterBalanceType: processAfterCounterBalanceType,
            ProcessAfterCounterBalanceTypeUsd: processAfterCounterBalanceTypeUsd,

            OtherRemarks: $("#OtherRemarks").val(),

            TotalSlideAmt: $("#TotalSlideAmt").NumericValue(),
            TotalReturnAmt: $("#TotalRefundAmt").NumericValue(),
            TotalBillingAmt: $("#TotalBillingAmt").NumericValue(),
            TotalAmtAfterCounterBalance: $("#TotalAmtAfterCounterBalanceType").NumericValue(),

            TotalSlideAmtUsd: $("#TotalSlideAmtUsd").NumericValue(),
            TotalReturnAmtUsd: $("#TotalRefundAmtUsd").NumericValue(),
            TotalBillingAmtUsd: $("#TotalBillingAmtUsd").NumericValue(),
            TotalAmtAfterCounterBalanceUsd: $("#TotalAmtAfterCounterBalanceTypeUsd").NumericValue(),

            //Other Infromation
            Memo: memo,

            IsShowContractRelatedInformation: isShowContractRelatedInformation,
            IsShowContractAgreementInformation: isShowContractAgreementInformation,
            IsShowDepositInformation: isShowDepositInformation,
            IsShowContractDocumentInformation: isShowContractDocumentInformation,
            IsShowProvideServiceInformation: isShowProvideServiceInformation,
            IsShowMaintenanceInformation: isShowMaintenanceInformation,
            IsShowSiteInformation: isShowSiteInformation,
            IsShowCancelContractCondition: isShowCancelContractCondition,

            //Constant Validate
            C_ContractFeeValidate: $("#lblContractFeeValidate").val(),
            C_MaintenanceFeeValidate: $("#lblMaintenanceFeeValidate").val(),

            OperationOfficeCode: $("#OperationOffice").val(), //Add by Jutarat A. on 15082012
            RelatedContractCode: $("#RelatedContractCode").val(), //Add by Jutarat A. on 16082012
            UserCode: $("#UserCode").val(), //Add by Jutarat A. on 18102013
            PaymentDateIncentive: $("#PaymentDateIncentive").val()
        }

        return validateRequireField;
    },

    HideCommandControl: function () {
        SetRegisterCommand(false, null);
        SetResetCommand(false, null);
        SetConfirmCommand(false, null);
        SetBackCommand(false, null);
    },
    RegisterCommandControl: function () {
        SetRegisterCommand(true, CTS140.BtnRegisterClick);
        SetResetCommand(true, CTS140.BtnResetClick);
        SetConfirmCommand(false, null);
        SetBackCommand(false, null);
    },
    ConfirmCommandControl: function () {
        SetRegisterCommand(false, null);
        SetResetCommand(false, null);
        SetConfirmCommand(true, CTS140.BtnConfirmClick);
        SetBackCommand(true, CTS140.BtnBackClick);
    },

    BtnRegisterClick: function () {
        command_control.CommandControlMode(false);
        call_ajax_method_json("/Contract/CTS140_Register", CTS140.GetValidateObject(),
            function (result, controls) {
                if (controls != undefined) {
                    VaridateCtrl(["FirstSecurityStartDate", //Add by Jutarat A. on 18102013
                                    "StartDealDate",
                                    "ContractStartDate",
                                    "ContractEndDate",
                                    "ProjectCode",
                                    "SalesmanEmpNo1",
                                    "SalesmanEmpNo2",
                                    "SalesSupporterEmpNo",
                                    "OldContractCode",
                                    "IntroducerCode",
                                    "CounterBalanceOriginContractCode",
                                    "ContractOfficeCode",
                                    "MaintenanceTypeCode",
                                    "MaintenanceCycle",
                                    "Month",
                                    "Year",
                                    "OperationOffice", //Add by Jutarat A. on 15082012
                                    "RelatedContractCode"], //Add by Jutarat A. on 16082012
                                    controls);
                    return;
                }
                else if (result == true) {
                    CTS140.SetSectionMode(true);
                    CTS140.ConfirmCommandControl();
                }
            }
        );
    },
    BtnConfirmClick: function () {
        command_control.CommandControlMode(false);
        call_ajax_method_json("/Contract/CTS140_Confirm", CTS140.GetValidateObject(),
            function (result, controls) {
                if (result != undefined) {
                    if (result.Code == "MSG3124" || result.Code == "MSG3125" || result.Code == "MSG3292") {
                        if (result.Code == "MSG3125") {
                            OpenInformationMessageDialog(result.Code, result.Message,
                                function () {
                                    CTS140.SaveData(false);
                                }
                            );
                        }
                        else if (result.Code == "MSG3292") {
                            OpenWarningMessageDialog(result.Code, result.Message,
                                function () {
                                    CTS140.SaveData(false);
                                }
                            );
                        }
                        else {
                            OpenYesNoWarningMessageDialog(result.Code, result.Message,
                                function () {
                                    CTS140.SaveData(true);
                                }
                            );
                        }
                    }
                }
                else {
                    CTS140.SaveData(false);
                }
            }
        );
    },
    BtnResetClick: function () {
        /* --- Get Message --- */
        var obj = {
            module: "Common",
            code: "MSG0038"
        };
        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
            OpenOkCancelDialog(result.Code, result.Message,
            function () {
                ajax_method.CallScreenControllerWithAuthority("/Contract/CTS140", "", false, null);
            },
            null);
        });
    },
    BtnBackClick: function () {
        CTS140.SetSectionMode(false);
        CTS140.RegisterCommandControl();
    },

    SaveData: function (isUpdateRemovalFee) {
        var obj = {
            isUpdateRemovalFeeToBillingTemp: isUpdateRemovalFee
        };
        call_ajax_method("/Contract/CTS140_Save", obj, function (result) {
            if (result != undefined) {
                OpenInformationMessageDialog(result.Code, result.Message,
                    function () {
                        CTS140.SetSectionMode(false);
                        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS140", "", false, null);

                    }, null);
            }
        });
    }
    /* ----------------------------------------------------------------- */
}

$(document).ready(function () {
    CTS140.MaintainScreenItems();
});




//var isValidateEnteringCondition;

//$(document).ready(function () {
//    //InitModeSection();
//    MaintainScreenItems();
//});

//function hideAll() {
//    $("#divContractBasicSection").hide();
//    $("#divContractRelatedFeeInformationMain").hide();
//    $("#divContractRelation").hide();
//    $("#divContractRelation").hide();
//    $("#divContractAgreement").hide();
//    $("#divDepositInformation").hide();
//    $("#divContractDocumentInformation").hide();
//    $("#divProviderServiceInformation").hide();
//    $("#divMaintenanceInformation").hide();
//    $("#divSiteInformation").hide();
//    $("#divCancelContractCondition").hide();
//    $("#divOtherInformation").hide();
//}

//function InitModeSection() {

//    var step = [
//                "CTS140_02",
//                "CTS140_03",
//                "CTS140_04",
//                "CTS140_05",
//                "CTS140_06",
//                "CTS140_07",
//                "CTS140_08",
//                "CTS140_09",
//                "CTS140_10"
//               ];

//    CallMultiLoadScreen("Contract", [step], MaintainScreenItems);
//}

//function RegisterCommandControl() {
//    SetRegisterCommand(true, BtnRegisterClick);
//    SetResetCommand(true, BtnResetClick);
//    SetConfirmCommand(false, null);
//    SetBackCommand(false, null);
//}

//function ConfirmCommandControl() {
//    SetRegisterCommand(false, null);
//    SetResetCommand(false, null);
//    SetConfirmCommand(true, BtnConfirmClick);
//    SetBackCommand(true, BtnBackClick);
//}

//function HideCommandControl() {
//    SetRegisterCommand(false, null);
//    SetResetCommand(false, null);
//    SetConfirmCommand(false, null);
//    SetBackCommand(false, null);
//}

//function MaintainScreenItems() {

//    hideAll();

//    call_ajax_method_json('/Contract/ValidateEnteringCondition_CTS140', "",
//        function (result, controls) {
//            if (result != undefined) {
//               // $("#divContractBasicSection").SetEnableView(false);
////                $("#divContractRelatedFeeInformationMain").SetEnableView(false);
////                $("#divContractRelation").SetEnableView(false);
////                $("#divContractRelation").attr("disabled", true);
////                $("#divContractAgreement").attr("disabled", true);
////                $("#divDepositInformation").attr("disabled", true);
////                $("#divContractDocumentInformation").attr("disabled", true);
////                $("#divProviderServiceInformation").attr("disabled", true);
////                $("#divMaintenanceInformation").attr("disabled", true);
////                $("#divSiteInformation").attr("disabled", true);
////                $("#divCancelContractCondition").attr("disabled", true);
////                $("#divOtherInformation").attr("disabled", true);

//                //$("#divContractBasicSection").attr("disabled", true);

////                $("#divContractRelatedFeeInformationMain").attr("disabled", true);
////                $("#divContractRelation").attr("disabled", true);
////                $("#divContractRelation").attr("disabled", true);
////                $("#divContractAgreement").attr("disabled", true);
////                $("#divDepositInformation").attr("disabled", true);
////                $("#divContractDocumentInformation").attr("disabled", true);
////                $("#divProviderServiceInformation").attr("disabled", true);
////                $("#divMaintenanceInformation").attr("disabled", true);
////                $("#divSiteInformation").attr("disabled", true);
////                $("#divCancelContractCondition").attr("disabled", true);
////                $("#divOtherInformation").attr("disabled", true);

//                //                $("#divContractBasicSection").attr("readonly", true);
//                //                $("#divContractRelatedFeeInformationMain").attr("readonly", true);
//                //                $("#divContractRelation").attr("readonly", true);
//                //                $("#divContractRelation").attr("readonly", true);
//                //                $("#divContractAgreement").attr("readonly", true);
//                //                $("#divDepositInformation").attr("readonly", true);
//                //                $("#divContractDocumentInformation").attr("readonly", true);
//                //                $("#divProviderServiceInformation").attr("readonly", true);
//                //                $("#divMaintenanceInformation").attr("readonly", true);
//                //                $("#divSiteInformation").attr("readonly", true);
//                //                $("#divCancelContractCondition").attr("readonly", true);
//                //                $("#divOtherInformation").attr("readonly", true);

//                DisabledSection(true);

//                HideCommandControl();
//            }
//            else {
//                isValidateEnteringCondition = true;

//                $("#StartDealDate").InitialDate();
//                InitialDateFromToControl("#ContractStartDate", "#ContractEndDate");

//                InitialDateFromToControl("#StartPeriodDateContract", "#EndPeriodDateContract");

//                RegisterCommandControl();
//            }
//        });

//    call_ajax_method_json('/Contract/GetConstantHideShowDIV', "",
//        function (result, controls) {
//            if (result != undefined) {

//                if (result.ProductTypeCode == result.C_PROD_TYPE_AL
//                    || result.ProductTypeCode == result.C_PROD_TYPE_RENTAL_SALE) {

//                    $("#divContractBasicSection").show();

//                    $("#divContractRelatedFeeInformationMain").show();
//                    $("#divContractRelation").show();
//                    $("#divContractAgreement").show();
//                    $("#divDepositInformation").show();
//                    $("#divContractDocumentInformation").show();

//                    $("#divProviderServiceInformation").show();
//                    $("#AlarmProvidedServiceType").show();
//                    $("#divSiteInformation").show();

//                    if (result.ContractStatus == result.C_CONTRACT_STATUS_CANCEL || result.ContractStatus == result.C_CONTRACT_STATUS_END) {
//                        $("#divCancelContractCondition").show();
//                    }

//                    $("#divOtherInformation").show();
//                }

//                if (result.ProductTypeCode == result.C_PROD_TYPE_MA) {
//                    $("#divContractBasicSection").show();

//                    $("#divContractRelatedFeeInformationMain").show();
//                    $("#divContractRelation").show();
//                    $("#divContractAgreement").show();
//                    $("#divDepositInformation").show();
//                    $("#divContractDocumentInformation").show();

//                    $("#divMaintenanceInformation").show();
//                    $("#divSiteInformation").show();
//                    if (result.ContractStatus == result.C_CONTRACT_STATUS_CANCEL || result.ContractStatus == result.C_CONTRACT_STATUS_END) {
//                        $("#divCancelContractCondition").show();
//                    }
//                    $("#divOtherInformation").show();
//                }

//                if (result.ProductTypeCode == result.C_PROD_TYPE_SG || result.ProductTypeCode == result.C_PROD_TYPE_BE) {
//                    $("#divContractBasicSection").show();

//                    $("#divContractRelatedFeeInformationMain").show();
//                    $("#divContractRelation").show();
//                    $("#divContractAgreement").show();
//                    $("#divDepositInformation").show();
//                    $("#divContractDocumentInformation").show();

//                    if (result.ContractStatus == result.C_CONTRACT_STATUS_CANCEL || result.ContractStatus == result.C_CONTRACT_STATUS_END) {
//                        $("#divCancelContractCondition").show();
//                    }
//                    $("#divOtherInformation").show();
//                }

//                if (result.ProductTypeCode == result.C_PROD_TYPE_ONLINE) {
//                    $("#divContractBasicSection").show();

//                    $("#divContractRelatedFeeInformationMain").show();
//                    $("#divContractRelation").show();
//                    $("#divContractAgreement").show();
//                    $("#divDepositInformation").show();
//                    $("#divContractDocumentInformation").show();

//                    $("#divProviderServiceInformation").show();
//                    $("#AlarmProvidedServiceType").show();

//                    $("#divSiteInformation").show();
//                    if (result.ContractStatus == result.C_CONTRACT_STATUS_CANCEL || result.ContractStatus == result.C_CONTRACT_STATUS_END) {
//                        $("#divCancelContractCondition").show();
//                    }
//                    $("#divOtherInformation").show();
//                }

//            }
//        });

//}


//function BtnRegisterClick() {
//    command_control.CommandControlMode(false);

//    call_ajax_method_json('/Contract/Register_CTS140', GetValidateObject(),
//    function (result, controls) {
//        if (controls != undefined) {
//            VaridateCtrl(["StartDealDate",
//                            "ContractStartDate",
//                            "ContractEndDate",
//                            "ProjectCode",
//                            "SalesmanEmpNo1",
//                            "SalesmanEmpNo2",
//                            "SalesSupporterEmpNo",
//                            "OldContractCode",
//                            "IntroducerCode",
//                            "CounterBalanceOriginContractCode",
//                            "ContractOfficeCode",
//                            "MaintenanceTypeCode",
//                            "MaintenanceCycle",
//                            "Month",
//                            "Year"], controls);
//            return;
//        }
//        else if (result == true) {
//            SetSectionMode(true);
//            ConfirmCommandControl();
//        }
//    });

//    //command_control.CommandControlMode(true);
//}

//function BtnConfirmClick() {
//    command_control.CommandControlMode(false);

//    call_ajax_method_json('/Contract/Confirm_CTS140', GetValidateObject(),
//    function (result, controls) {
//        if (result != undefined) {
//            if (result.Code == "MSG3124" || result.Code == "MSG3125") {
//                var obj = {
//                    module: "Contract",
//                    code: result.Code
//                };

//                call_ajax_method("/Shared/GetMessage", obj, function (result) {
//                    OpenYesNoWarningMessageDialog(result.Code, result.Message,
//                            function () {
//                                //                                var obj = { isUpdateRemovalFeeToBillingTemp: true };
//                                //                                call_ajax_method("/Contract/Save_CTS140", obj, function (result) {
//                                //                                    if (result != false) {
//                                //                                        OpenInformationMessageDialog(result.Code, result.Message, function () {
//                                //                                        }, null);
//                                //                                    }
//                                //                                });

//                                SaveData(obj.code == "MSG3124");
//                            });
//                });
//            }
//        }
//        else {
//            //            var obj = { isUpdateRemovalFeeToBillingTemp: true };
//            //            call_ajax_method("/Contract/Save_CTS140", obj, function (result) {
//            //                OpenInformationMessageDialog(result.Code, result.Message, function () { }, null);
//            //            });

//            SaveData(false);
//        }
//    });

//    //command_control.CommandControlMode(true);
//}

//function SaveData(isUpdateRemovalFee) {
//    var obj = { isUpdateRemovalFeeToBillingTemp: isUpdateRemovalFee };
//    call_ajax_method("/Contract/Save_CTS140", obj, function (result) {
//        if (result != undefined) {
//            OpenInformationMessageDialog(result.Code, result.Message,
//                function () {
//                    SetSectionMode(false);
//                    //SetResetState();

//                    var objParam = { contractCode: objCTS140_09.ContractCode };
//                    ajax_method.CallScreenControllerWithAuthority("/Contract/CTS140", objParam, false, null);

//                }, null);
//        }
//    });
//}

//function BtnResetClick() {
//    /* --- Get Message --- */
//    var obj = {
//        module: "Common",
//        code: "MSG0038"
//    };
//    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
//        OpenOkCancelDialog(result.Code, result.Message,
//        function () {
//            //            var obj = { ContractCode: objCTS140_09.ContractCode };
//            //            call_ajax_method("/Contract/InitialScreen_CTS140", obj, function (result) { });

//            //            BindDOContractRelateInformationDateField();
//            //            BindDOContractAgreementInformationDateField();
//            //            BindDODepositInformation();
//            //            BindDOContractDocumentInformation();
//            //            BindDOProvideServiceInformation();
//            //            BindDOMaintenanceInformation();
//            //            BindDOSiteInformation();
//            //            BindDOOtherInformation();
//            //            ClearControl();

//            //            GetMaintenanceGrid();
//            //            GetCancelGrid();

//            //SetResetState();
//            var obj = { ContractCode: objCTS140_09.ContractCode };
//            ajax_method.CallScreenControllerWithAuthority("/Contract/CTS140", obj, false, null);
//        },
//        null);
//    });
//}

//function SetResetState() {
//    var obj = { ContractCode: objCTS140_09.ContractCode };
//    call_ajax_method("/Contract/InitialScreen_CTS140", obj, function () { });

//    BindDOContractRelateInformationDateField();
//    BindDOContractAgreementInformationDateField();
//    BindDODepositInformation();
//    BindDOContractDocumentInformation();
//    BindDOProvideServiceInformation();
//    BindDOMaintenanceInformation();
//    BindDOSiteInformation();
//    BindDOOtherInformation();
//    ClearControl();

//    GetMaintenanceGrid();
//    GetCancelGrid();
//}

//function BtnBackClick() {
//    SetSectionMode(false);
//    RegisterCommandControl();
//}

//function DisabledSection(isDisabled) {
//    DisabledSectionCTS140_2(isDisabled);
//    DisabledSectionCTS140_3(isDisabled);
//    DisabledSectionCTS140_4(isDisabled);
//    DisabledSectionCTS140_5(isDisabled);
//    DisabledSectionCTS140_6(isDisabled);
//    DisabledSectionCTS140_7(isDisabled);
//    DisabledSectionCTS140_8(isDisabled);
//    DisabledSectionCTS140_9(isDisabled);
//    DisabledSectionCTS140_10(isDisabled);
//}

//function SetSectionMode(isView) {
//    SetSectionModeCTS140_01(isView);
//    SetSectionModeCTS140_02(isView);
//    SetSectionModeCTS140_03(isView);
//    SetSectionModeCTS140_04(isView);
//    SetSectionModeCTS140_05(isView);
//    SetSectionModeCTS140_06(isView);
//    SetSectionModeCTS140_07(isView);
//    SetSectionModeCTS140_08(isView);
//    SetSectionModeCTS140_09(isView);
//    SetSectionModeCTS140_10(isView);
//}