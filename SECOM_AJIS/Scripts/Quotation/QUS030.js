/// <reference path="../../Base/Master.js" />

/* --- Inital Variables -------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
var ProductTypeMode;
/* ----------------------------------------------------------------------------------- */

/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    InitialCommandButton(0);
});
function InitialControl() {
    /// <summary>Method to initial control</summary>
    if (typeof (InitQuotationTargetInfo) == "function")
        InitQuotationTargetInfo();

    InitialCommandButton(0);
}
/* ----------------------------------------------------------------------------------- */

//function GetEmployeeNameData(src, dest) {
//    if (src != undefined && dest != undefined) {
//        $(src).blur(function () {
//            if (dest != undefined) {
//                $(dest).val("");
//                if ($.trim($(src).val()) != "") {
//                    /* --- Set Parameter --- */
//                    var obj = {
//                        empNo: $(src).val()
//                    };

//                    call_ajax_method_json("/Master/GetActiveEmployeeName", obj, function (result) {
//                        if (result != undefined) {
//                            $(dest).val(result);
//                        }
//                    });
//                }
//            }
//        });
//    }
//}

/* --- Command Methods --------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function InitialCommandButton(step) {
    /// <summary>Method to initial Command button</summary>
    /// <param name="step" type="int">Command Step ( 0 = Initial, 1 = Edit, 2 = Registered, 3 = Confirmed )</param>

    if (step == 0) {
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }
    else if (step == 1) {
        register_command.SetCommand(command_register_click);
        reset_command.SetCommand(command_reset_click);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }
    else if (step == 2) {
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(command_confirm_click);
        back_command.SetCommand(command_back_click);
    }
}

/* --- Events --- */
function command_register_click() {
    /// <summary>Event when click Register command</summary>
    //02 07 13
    if (ProductTypeMode.IsProductTypeSale)
        RegisterSaleData();
    //03 10 14
    else if (ProductTypeMode.IsProductTypeAL
                || ProductTypeMode.IsProductTypeRentalSale)
        RegisterALData();
    //04 10 15
    else if (ProductTypeMode.IsProductSaleOnline)
        RegisterSaleOnlineData();
    //05 11 15
    else if (ProductTypeMode.IsProductBeatGuard)
        RegisterBeatGuardData();
    //05 12 15
    else if (ProductTypeMode.IsProductSentryGuard)
        RegisterSentryGuardData();
    //05 06 15
    else if (ProductTypeMode.IsProductMaintenance)
        RegisterMaintenanceDetailData();
}
function command_reset_click() {
    InitialControl();
    ResetSection();

    ajax_method.CallScreenController("/Quotation/QUS030_ClearImportKey", "");
}

function command_confirm_click() {
    /// <summary>Event when click Confirm command</summary>

    /* --- Call Confirm method --- */
    /* --------------------------- */
    ajax_method.CallScreenController("/Quotation/QUS030_RegisterQuotationData", "", function (result) {
        if (result != undefined) {
            OpenInformationMessageDialog(result.Code, result.Message, function () {
                var step = ["QUS030_16"];
                CallMultiLoadScreen("Quotation", [step], function () {
                    InitialCommandButton(0);

                    $("#btnRegisterNextQuotationDetail").focus();
                    master_event.ScrollWindow("#divResultRegisterQuotationDetail");
                });
            });
        }
    });
    /* --------------------------- */
}
function command_back_click() {
    /// <summary>Event when click Back command</summary>
    if (ProductTypeMode.IsProductTypeSale)
        ChangeSaleSectionMode(false);
    else if (ProductTypeMode.IsProductTypeAL
        || ProductTypeMode.IsProductTypeRentalSale)
        ChangeALSectionMode(false);
    else if (ProductTypeMode.IsProductSaleOnline)
        ChangeSaleOnlineSectionMode(false);
    else if (ProductTypeMode.IsProductBeatGuard)
        ChangeBeatGuardSectionMode(false);
    else if (ProductTypeMode.IsProductSentryGuard)
        ChangeSentryGuardSectionMode(false);
    else if (ProductTypeMode.IsProductMaintenance)
        ChangeMaintenanceDetailSectionMode(false);

    InitialCommandButton(1);
}
/* ----------------------------------------------------------------------------------- */





/* --- Sale Methods ------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function InitSaleSection(func) {
    /// <summary>Method to Show Sale section</summary>
    var step = ["QUS030_02",
                 "QUS030_07",
                 "QUS030_13"];
    CallMultiLoadScreen("Quotation", [step], func);
}
function RegisterSaleData() {
    if (typeof (GetQUS030_02_SectionData) == "function"
        && typeof (GetQUS030_07_SectionData) == "function"
        && typeof (GetQUS030_13_SectionData) == "function") {

        QUS030_02_ResetToNormalControl();
        QUS030_07_ResetToNormalControl();
        QUS030_13_ResetToNormalControl();

        /* --- Generate Object --- */
        /* ----------------------- */
        var quotationBasic = GetQUS030_02_SectionData();
        var instLst = GetQUS030_07_SectionData();
        var mQuotationBasic = GetQUS030_13_SectionData();

        if (quotationBasic != undefined) {
            quotationBasic["ProductPrice"] = mQuotationBasic.ProductPrice;
            quotationBasic["ProductPriceCurrencyType"] = mQuotationBasic.ProductPriceCurrencyType;
            quotationBasic["InstallationFee"] = mQuotationBasic.InstallationFee;
            quotationBasic["InstallationFeeCurrencyType"] = mQuotationBasic.InstallationFeeCurrencyType;
        }
        else {
            quotationBasic = mQuotationBasic;
        }

        var obj = {
            quotationBasic: quotationBasic,
            instLst: instLst
        };
        /* ----------------------- */

        /* --- Call Register methods --- */
        /* ----------------------------- */
        ajax_method.CallScreenController("/Quotation/QUS030_CheckSaleQuotationData_P1", obj, function (result, controls) {
            /* --- Higlight Text --- */
            /* --------------------- */
            var hFunc = function (ctrls) {
                VaridateCtrl(["ProductCode",
                "PlanCode",
                "PlannerEmpNo",
                "PlanCheckerEmpNo",
                "PlanApproverEmpNo",
                "SalesmanEmpNo1",
                "SalesmanEmpNo2",
                "SalesmanEmpNo3",
                "SalesmanEmpNo4",
                "SalesmanEmpNo5",
                "SalesmanEmpNo6",
                "SalesmanEmpNo7",
                "SalesmanEmpNo8",
                "SalesmanEmpNo9",
                "SalesmanEmpNo10",
                "ProductPrice",
                "SiteBuildingArea",
                "SecurityAreaFrom",
                "SecurityAreaTo",
                "BuildingTypeCode",
                "NewBldMgmtCost",
                "BidGuaranteeAmount1",
                "BidGuaranteeAmount2",
                "ApproveNo1",
                "ProductPrice",
                "InstallationFee",
                "InstrumentCode",
                "InstrumentQty",

                "MainStructureTypeCode"], ctrls);

                if (typeof (GetQUS030_07_SectionValidation) == "function")
                    GetQUS030_07_SectionValidation(controls);
            };

            hFunc(controls);
            /* --------------------- */

            if (controls != undefined) {
                return;
            }
            else if (result != undefined) {
                /* --- Method when success --- */
                /* --------------------------- */
                var sale_success_method = function (result) {
                    if (result != undefined) {
                        /* --- Change to view mode --- */
                        /* --------------------------- */
                        ChangeSaleSectionMode(true);
                        /* --------------------------- */

                        /* --- Change Command Step --- */
                        /* --------------------------- */
                        InitialCommandButton(2);
                        /* --------------------------- */
                    }
                };
                /* --------------------------- */

                if (result.Code != undefined) {
                    OpenYesNoMessageDialog(result.Code, result.Message, function () {
                        ajax_method.CallScreenController("/Quotation/QUS030_CheckSaleQuotationData_P2", obj, function (result, controls) {
                            hFunc(controls);
                            if (controls != undefined)
                                return;

                            sale_success_method(result);
                        });
                    }, null);
                }
                else if (result == "ProdError") {
                    ajax_method.CallScreenController("/Quotation/QUS030_CheckSaleQuotationData_P2", obj, function (result, controls) {
                        hFunc(controls);
                        if (controls != undefined)
                            return;

                        sale_success_method(result);
                    });
                }
                else {
                    sale_success_method(result);
                }
            }
        });
        /* ----------------------------- */
    }
}
function ChangeSaleSectionMode(view_mode) {
    /// <summary>Method to change to View/Edit mode for Sale section</summary>
    /// <param name="view_mode" type="bool">Flag to set View mode</param>
    var screen = ["QUS030_01",
                    "QUS030_02",
                    "QUS030_07",
                    "QUS030_13"];
    ChangeAllSectionMode(screen, view_mode);
}
/* ----------------------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */





/* --- AL Methods -------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function InitALSection(func) {
    /// <summary>Method to Show AL section</summary>
    var inst = "QUS030_08";
    if (ProductTypeMode.IsShowInstrument01 == true) {
        inst = "QUS030_07";
    }

    var step = ["QUS030_03",
                inst,
                 "QUS030_10",
                 "QUS030_14"];
    CallMultiLoadScreen("Quotation", [step], func);
}
function RegisterALData() {
    /// <summary>Method to Register AL section</summary>
    if (typeof (GetQUS030_03_SectionData) == "function"
        && typeof (GetQUS030_10_SectionData) == "function"
        && typeof (GetQUS030_14_SectionData) == "function") {

        QUS030_03_ResetToNormalControl();
        QUS030_10_ResetToNormalControl();
        QUS030_14_ResetToNormalControl();

        /* --- Generate Object --- */
        /* ----------------------- */
        var quotationBasic = GetQUS030_03_SectionData();

        var instLst = null;
        if (ProductTypeMode.IsShowInstrument01 == true && typeof (GetQUS030_07_SectionData) == "function")
            instLst = GetQUS030_07_SectionData();
        else if (typeof (GetQUS030_08_SectionData) == "function")
            instLst = GetQUS030_08_SectionData();

        var fObj = GetQUS030_10_SectionData();

        var mQuotationBasic = GetQUS030_14_SectionData();

        if (quotationBasic != undefined) {
          
            quotationBasic["ContractFee"] = mQuotationBasic.ContractFee;
            quotationBasic["InstallationFee"] = mQuotationBasic.InstallationFee;
            quotationBasic["DepositFee"] = mQuotationBasic.DepositFee;
            quotationBasic["ContractFeeCurrencyType"] = mQuotationBasic.ContractFeeCurrencyType;
            quotationBasic["InstallationFeeCurrencyType"] = mQuotationBasic.InstallationFeeCurrencyType;
            quotationBasic["DepositFeeCurrencyType"] = mQuotationBasic.DepositFeeCurrencyType;
            quotationBasic["FacilityMemo"] = fObj.FacilityMemo;
        }
        else {
            quotationBasic = mQuotationBasic;
        }

        var obj = {
            quotationBasic: quotationBasic,
            instLst: instLst,
            facilityLst: fObj.FacilityLst
        };
        /* ----------------------- */

        /* --- Call register methods --- */
        /* ----------------------------- */
        ajax_method.CallScreenController("/Quotation/QUS030_CheckALQuotationData_P1", obj, function (result, controls) {
            /* --- Higlight Text --- */
            /* --------------------- */
            var hFunc = function (ctrls) {
                VaridateCtrl(["ProductCode",
                "PhoneLineTypeCode1",
                "PhoneLineOwnerTypeCode1",
                "PhoneLineTypeCode2",
                "PhoneLineOwnerTypeCode2",
                "PhoneLineTypeCode3",
                "PhoneLineOwnerTypeCode3",
                "PlanCode",
                "PlannerEmpNo",
                "PlanCheckerEmpNo",
                "PlanApproverEmpNo",
                "SalesmanEmpNo1",
                "SalesmanEmpNo2",
                "SalesSupporterEmpNo",
                "ContractFee",
                "SiteBuildingArea",
                "SecurityAreaFrom",
                "SecurityAreaTo",
                "BuildingTypeCode",
                "NewBldMgmtCost",
                "NumOfBuilding",
                "NumOfFloor",
                "InsuranceCoverageAmount",
                "MonthlyInsuranceFee",
                "MaintenanceFee1",
                "AdditionalFee1",
                "AdditionalFee2",
                "AdditionalFee3",
                "AdditionalApproveNo1",
                "AdditionalApproveNo2",
                "AdditionalApproveNo3",
                "ApproveNo1",
                "ContractFee",
                "InstallationFee",
                "DepositFee",
                "InstrumentCode",
                "InstrumentQty",
                "FacilityCode",
                "FacilityQty",

                "DispatchTypeCode",
                "PhoneLineTypeCode1",
                "PhoneLineOwnerTypeCode1",
                "PhoneLineTypeCode2",
                "PhoneLineOwnerTypeCode2",
                "PhoneLineTypeCode3",
                "PhoneLineOwnerTypeCode3",
                "MainStructureTypeCode",
                "MaintenanceCycle",
                "InsuranceTypeCode"], ctrls);

                if (typeof (GetQUS030_07_SectionValidation) == "function")
                    GetQUS030_07_SectionValidation(ctrls);
                if (typeof (GetQUS030_08_SectionValidation) == "function")
                    GetQUS030_08_SectionValidation(ctrls);
                if (typeof (GetQUS030_10_SectionValidation) == "function")
                    GetQUS030_10_SectionValidation(ctrls);
            };

            hFunc(controls);
            /* --------------------- */

            if (controls != undefined) {
                return;
            }
            else if (result != undefined) {
                /* --- Method when success --- */
                /* --------------------------- */
                var al_success_method = function (result) {
                    if (result != undefined) {
                        /* --- Change to view mode --- */
                        /* --------------------------- */
                        ChangeALSectionMode(true);
                        /* --------------------------- */

                        /* --- Change Command Step --- */
                        /* --------------------------- */
                        InitialCommandButton(2);
                        /* --------------------------- */
                    }
                };

                if (result.Code != undefined) {
                    /* --- Confirm Message --- */
                    /* ----------------------- */
                    OpenYesNoMessageDialog(result.Code, result.Message, function () {
                        ajax_method.CallScreenController("/Quotation/QUS030_CheckALQuotationData_P2", obj, function (result, controls) {
                            hFunc(controls);
                            if (controls != undefined)
                                return;

                            al_success_method(result);
                        });
                    }, null);
                    /* ----------------------- */
                }
                else if (result == "ProdError") {
                    ajax_method.CallScreenController("/Quotation/QUS030_CheckALQuotationData_P2", obj, function (result, controls) {
                        hFunc(controls);
                        if (controls != undefined)
                            return;

                        al_success_method(result);
                    });
                }
                else {
                    al_success_method(result);
                }
            }
        });
        /* ----------------------------- */
    }
}
function ChangeALSectionMode(view_mode) {
    /// <summary>Method to change to View/Edit mode for AL section</summary>
    /// <param name="view_mode" type="bool">Flag to set View mode</param>
    var inst = "QUS030_08";
    if (ProductTypeMode.IsShowInstrument01 == true) {
        inst = "QUS030_07";
    }

    var screen = ["QUS030_01",
                    "QUS030_03",
                    inst,
                    "QUS030_10",
                    "QUS030_14"];
    ChangeAllSectionMode(screen, view_mode);
}
/* ----------------------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */





/* --- Sale Online Methods ----------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function InitSaleOnlineSection(func) {
    /// <summary>Method to Show Sale Online section</summary>
    var step = ["QUS030_04",
                 "QUS030_09",
                 "QUS030_10",
                 "QUS030_15"];
    CallMultiLoadScreen("Quotation", [step], func);
}
function RegisterSaleOnlineData() {
    /// <summary>Method to Register AL section</summary>
    if (typeof (GetQUS030_04_SectionData) == "function"
        && typeof (GetQUS030_10_SectionData) == "function"
        && typeof (GetQUS030_15_SectionData) == "function") {

        QUS030_04_ResetToNormalControl();
        QUS030_10_ResetToNormalControl();
        QUS030_15_ResetToNormalControl();

        /* --- Generate Object --- */
        /* ----------------------- */
        var quotationBasic = GetQUS030_04_SectionData();

        var fObj = GetQUS030_10_SectionData();
        var mQuotationBasic = GetQUS030_15_SectionData();

        if (quotationBasic != undefined) {
            quotationBasic["ContractFee"] = mQuotationBasic.ContractFee;
            quotationBasic["DepositFee"] = mQuotationBasic.DepositFee;
            quotationBasic["ContractFeeCurrencyType"] = mQuotationBasic.ContractFeeCurrencyType;
            quotationBasic["DepositFeeCurrencyType"] = mQuotationBasic.DepositFeeCurrencyType;
            quotationBasic["FacilityMemo"] = fObj.FacilityMemo; 
        }
        else {
            quotationBasic = mQuotationBasic;
        }

        var obj = {
            quotationBasic: quotationBasic,
            facilityLst: fObj.FacilityLst
        };
        /* ----------------------- */

        /* --- Call register methods --- */
        /* ----------------------------- */
        ajax_method.CallScreenController("/Quotation/QUS030_CheckSaleOnlineQuotationData_P1", obj, function (result, controls) {
            /* --- Higlight Text --- */
            /* --------------------- */
            var hFunc = function (ctrls) {
                VaridateCtrl(["ProductCode",
                "PhoneLineTypeCode1",
                "PhoneLineOwnerTypeCode1",
                "PhoneLineTypeCode2",
                "PhoneLineOwnerTypeCode2",
                "PhoneLineTypeCode3",
                "PhoneLineOwnerTypeCode3",
                "SaleOnlineContractCode",
                "NumOfBuilding",
                "NumOfFloor",
                "MaintenanceCycle",
                "SalesmanEmpNo1",
                "SalesmanEmpNo2",
                "SalesSupporterEmpNo",
                "InsuranceCoverageAmount",
                "MonthlyInsuranceFee",
                "MaintenanceFee1",
                "AdditionalFee1",
                "AdditionalFee2",
                "AdditionalFee3",
                "AdditionalApproveNo1",
                "AdditionalApproveNo2",
                "AdditionalApproveNo3",
                "ContractFee",
                "DepositFee",
                "FacilityCode",
                "FacilityQty",

                "DispatchTypeCode",
                "PhoneLineTypeCode1",
                "PhoneLineOwnerTypeCode1",
                "PhoneLineTypeCode2",
                "PhoneLineOwnerTypeCode2",
                "PhoneLineTypeCode3",
                "PhoneLineOwnerTypeCode3",
                "MaintenanceCycle",
                "InsuranceTypeCode"], ctrls);

                if (typeof (GetQUS030_10_SectionValidation) == "function")
                    GetQUS030_10_SectionValidation(ctrls);
            };
            hFunc(controls);
            /* --------------------- */

            var online_success_method = function (result) {
                if (result != undefined) {
                    /* --- Change to view mode --- */
                    /* --------------------------- */
                    ChangeSaleOnlineSectionMode(true);
                    /* --------------------------- */

                    /* --- Change Command Step --- */
                    /* --------------------------- */
                    InitialCommandButton(2);
                    /* --------------------------- */
                }
            };

            if (controls != undefined) {
                return;
            }
            else if (result != undefined) {
                if (result == "ProdError") {
                    ajax_method.CallScreenController("/Quotation/QUS030_CheckSaleOnlineQuotationData_P2", obj, function (result, controls) {
                        hFunc(controls);
                        if (controls != undefined)
                            return;

                        online_success_method(result);
                    });
                }
                else {
                    online_success_method(result);
                }

            }
        });
        /* ----------------------------- */
    }
}
function ChangeSaleOnlineSectionMode(view_mode) {
    /// <summary>Method to change to View/Edit mode for Sale Online section</summary>
    /// <param name="view_mode" type="bool">Flag to set View mode</param>
    var screen = ["QUS030_01",
                    "QUS030_04",
                    "QUS030_09",
                    "QUS030_10",
                    "QUS030_15"];
    ChangeAllSectionMode(screen, view_mode);
}
/* ----------------------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */





/* --- Beat Guard Methods ------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function InitBeatGuardSection(func) {
    /// <summary>Method to Show Beat Guard section</summary>
    var step = ["QUS030_05",
                 "QUS030_11",
                 "QUS030_15"];
    CallMultiLoadScreen("Quotation", [step], func);
}
function RegisterBeatGuardData() {
    /// <summary>Method to Register Beat Guard section</summary>
    if (typeof (GetQUS030_05_SectionData) == "function"
        && typeof (GetQUS030_11_SectionData) == "function"
        && typeof (GetQUS030_15_SectionData) == "function") {

        QUS030_05_ResetToNormalControl();
        QUS030_11_ResetToNormalControl();
        QUS030_15_ResetToNormalControl();

        /* --- Generate Object --- */
        /* ----------------------- */
        var quotationBasic = GetQUS030_05_SectionData();

        var beatGuard = GetQUS030_11_SectionData();
        var mQuotationBasic = GetQUS030_15_SectionData();

        if (quotationBasic != undefined) {
            quotationBasic["ContractFee"] = mQuotationBasic.ContractFee;
            quotationBasic["DepositFee"] = mQuotationBasic.DepositFee;
            quotationBasic["ContractFeeCurrencyType"] = mQuotationBasic.ContractFeeCurrencyType;
            quotationBasic["DepositFeeCurrencyType"] = mQuotationBasic.DepositFeeCurrencyType;
        }
        else {
            quotationBasic = mQuotationBasic;
        }

        var obj = {
            quotationBasic: quotationBasic,
            doBeatGuardDetail: beatGuard
        };
        /* ----------------------- */

        /* --- Call register methods --- */
        /* ----------------------------- */
        ajax_method.CallScreenController("/Quotation/QUS030_CheckBeatGuardQuotationData_P1", obj, function (result, controls) {
            /* --- Higlight Text --- */
            /* --------------------- */
            var hFunc = function (ctrls) {
                VaridateCtrl(["ProductCode",
                                "SalesmanEmpNo1",
                                "SalesmanEmpNo2",
                                "SalesSupporterEmpNo",
                                "MaintenanceFee1",
                                "AdditionalFee1",
                                "AdditionalFee2",
                                "AdditionalFee3",
                                "AdditionalApproveNo1",
                                "AdditionalApproveNo2",
                                "AdditionalApproveNo3",
                                "NumOfDayTimeWd",
                                "NumOfNightTimeWd",
                                "NumOfDayTimeSat",
                                "NumOfNightTimeSat",
                                "NumOfDayTimeSun",
                                "NumOfNightTimeSun",
                                "FreqOfGateUsage",
                                "NumOfClockKey",
                                "NumOfBeatStep",
                                "NumOfDate",
                                "NotifyTime",
                                "ContractFee",
                                "DepositFee"], ctrls);
            };
            hFunc(controls);
            /* --------------------- */

            var beatguard_success_method = function (result) {
                if (result != undefined) {
                    /* --- Change to view mode --- */
                    /* --------------------------- */
                    ChangeBeatGuardSectionMode(true);
                    /* --------------------------- */

                    /* --- Change Command Step --- */
                    /* --------------------------- */
                    InitialCommandButton(2);
                    /* --------------------------- */
                }
            };

            if (controls != undefined) {
                return;
            }
            else if (result != undefined) {
                if (result == "ProdError") {
                    ajax_method.CallScreenController("/Quotation/QUS030_CheckBeatGuardQuotationData_P2", obj, function (result, controls) {
                        hFunc(controls);
                        if (controls != undefined)
                            return;

                        beatguard_success_method(result);
                    });
                }
                else {
                    beatguard_success_method(result);
                }
            }
        });
        /* ----------------------------- */
    }
}
function ChangeBeatGuardSectionMode(view_mode) {
    /// <summary>Method to change to View/Edit mode for Beat Guard section</summary>
    /// <param name="view_mode" type="bool">Flag to set View mode</param>
    var screen = ["QUS030_01",
                    "QUS030_05",
                    "QUS030_11",
                    "QUS030_15"];
    ChangeAllSectionMode(screen, view_mode);
}
/* ----------------------------------------------------------------------------------- */






/* --- Sentry Guard Methods ---------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function InitSentryGuardSection(func) {
    /// <summary>Method to Show Sentry Guard section</summary>
    var step = ["QUS030_05",
                 "QUS030_12",
                 "QUS030_15"];
    CallMultiLoadScreen("Quotation", [step], func);
}
function RegisterSentryGuardData() {
    /// <summary>Method to Register Sentry section</summary>
    if (typeof (GetQUS030_05_SectionData) == "function"
        && typeof (GetQUS030_12_SectionData) == "function"
        && typeof (GetQUS030_15_SectionData) == "function") {

        QUS030_05_ResetToNormalControl();
        QUS030_12_ResetToNormalControl();
        QUS030_15_ResetToNormalControl();

        /* --- Generate Object --- */
        /* ----------------------- */
        var quotationBasic = GetQUS030_05_SectionData();

        var sentryGuard = GetQUS030_12_SectionData();
        var mQuotationBasic = GetQUS030_15_SectionData();

        if (quotationBasic != undefined) {
            quotationBasic["ContractFee"] = mQuotationBasic.ContractFee;
            quotationBasic["DepositFee"] = mQuotationBasic.DepositFee;

            quotationBasic["SecurityItemFee"] = sentryGuard.QuotationBasic.SecurityItemFee;
            quotationBasic["OtherItemFee"] = sentryGuard.QuotationBasic.OtherItemFee;

            quotationBasic["ContractFeeCurrencyType"] = mQuotationBasic.ContractFeeCurrencyType;
            quotationBasic["DepositFeeCurrencyType"] = mQuotationBasic.DepositFeeCurrencyType;

            quotationBasic["SecurityItemFeeCurrencyType"] = sentryGuard.QuotationBasic.SecurityItemFeeCurrencyType;
            quotationBasic["OtherItemFeeCurrencyType"] = sentryGuard.QuotationBasic.OtherItemFeeCurrencyType;
            quotationBasic["SentryGuardAreaTypeCode"] = sentryGuard.QuotationBasic.SentryGuardAreaTypeCode;

            quotationBasic.IsEditMode = sentryGuard.IsEditMode;
        }

        var obj = {
            quotationBasic: quotationBasic
        };
        /* ----------------------- */

        /* --- Call register methods --- */
        /* ----------------------------- */
        ajax_method.CallScreenController("/Quotation/QUS030_CheckSentryGuardQuotationData_P1", obj, function (result, controls) {
            /* --- Higlight Text --- */
            /* --------------------- */
            var hFunc = function (ctrls) {
                VaridateCtrl(["ProductCode",
                                "SalesmanEmpNo1",
                                "SalesmanEmpNo2",
                                "SalesSupporterEmpNo",
                                "MaintenanceFee1",
                                "AdditionalFee1",
                                "AdditionalFee2",
                                "AdditionalFee3",
                                "AdditionalApproveNo1",
                                "AdditionalApproveNo2",
                                "AdditionalApproveNo3",
                                "SecurityItemFee",
                                "OtherItemFee",
                                "ContractFee",
                                "DepositFee"], ctrls);
            };
            hFunc(controls);
            /* --------------------- */

            var sentryguard_success_method = function (result) {
                if (result != undefined) {
                    /* --- Change to view mode --- */
                    /* --------------------------- */
                    ChangeSentryGuardSectionMode(true);
                    /* --------------------------- */

                    /* --- Change Command Step --- */
                    /* --------------------------- */
                    InitialCommandButton(2);
                    /* --------------------------- */
                }
            };

            if (controls != undefined) {
                return;
            }
            else if (result != undefined) {
                if (result == "ProdError") {
                    ajax_method.CallScreenController("/Quotation/QUS030_CheckSentryGuardQuotationData_P2", obj, function (result, controls) {
                        hFunc(controls);
                        if (controls != undefined)
                            return;

                        sentryguard_success_method(result);
                    });
                }
                else {
                    sentryguard_success_method(result);
                }
            }
        });
        /* ----------------------------- */
    }
}
function ChangeSentryGuardSectionMode(view_mode) {
    /// <summary>Method to change to View/Edit mode for Sentry Guard section</summary>
    /// <param name="view_mode" type="bool">Flag to set View mode</param>
    var screen = ["QUS030_01", "QUS030_05", "QUS030_12", "QUS030_15"];
    ChangeAllSectionMode(screen, view_mode);
}
/* ----------------------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */





/* --- Maintenance Detail Methods ---------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function InitMaintenanceDetailSection(func) {
    /// <summary>Method to Show Maintenance Detail section</summary>
    var step = ["QUS030_05",
                 "QUS030_06",
                 "QUS030_15"];
    CallMultiLoadScreen("Quotation", [step], func);
}
function RegisterMaintenanceDetailData() {
    /// <summary>Method to Register Sentry section</summary>
    if (typeof (GetQUS030_05_SectionData) == "function"
        && typeof (GetQUS030_06_SectionData) == "function"
        && typeof (GetQUS030_15_SectionData) == "function") {

        QUS030_05_ResetToNormalControl();
        QUS030_06_ResetToNormalControl();
        QUS030_15_ResetToNormalControl();

        /* --- Generate Object --- */
        /* ----------------------- */
        var quotationBasic = GetQUS030_05_SectionData();

        var maintenanceDetail = GetQUS030_06_SectionData();
        var mQuotationBasic = GetQUS030_15_SectionData();

        if (quotationBasic != undefined) {
            quotationBasic["ContractFee"] = mQuotationBasic.ContractFee;
            quotationBasic["DepositFee"] = mQuotationBasic.DepositFee;
            quotationBasic["ContractFeeCurrencyType"] = mQuotationBasic.ContractFeeCurrencyType;
            quotationBasic["DepositFeeCurrencyType"] = mQuotationBasic.DepositFeeCurrencyType;

            quotationBasic["MaintenanceTargetProductTypeCode"] = maintenanceDetail.QuotationBasic.MaintenanceTargetProductTypeCode;
            quotationBasic["MaintenanceTypeCode"] = maintenanceDetail.QuotationBasic.MaintenanceTypeCode;
            quotationBasic["MaintenanceCycle"] = maintenanceDetail.QuotationBasic.MaintenanceCycle;
            quotationBasic["MaintenanceMemo"] = maintenanceDetail.QuotationBasic.MaintenanceMemo;
        }

        var obj = {
            quotationBasic: quotationBasic,
            maLst: maintenanceDetail.MaintenanceList
        };
        /* ----------------------- */

        /* --- Call register methods --- */
        /* ----------------------------- */
        ajax_method.CallScreenController("/Quotation/QUS030_CheckMaintenanceDetailQuotationData_P1", obj, function (result, controls) {
            /* --- Higlight Text --- */
            /* --------------------- */
            var hFunc = function (ctrls) {
                VaridateCtrl(["ProductCode",
                                "SalesmanEmpNo1",
                                "SalesmanEmpNo2",
                                "SalesSupporterEmpNo",
                                "MaintenanceFee1",
                                "AdditionalFee1",
                                "AdditionalFee2",
                                "AdditionalFee3",
                                "AdditionalApproveNo1",
                                "AdditionalApproveNo2",
                                "AdditionalApproveNo3",
                                "MaintenanceTargetProductTypeCode",
                                "MaintenanceCycle",
                                "MaintenanceTypeCode",
                                "MaintenanceTargetContractCode",
                                "ContractFee",
                                "DepositFee"], ctrls);
            };
            hFunc(controls);
            /* --------------------- */

            var ma_success_method = function (result) {
                if (result != undefined) {
                    /* --- Change to view mode --- */
                    /* --------------------------- */
                    ChangeMaintenanceDetailSectionMode(true);
                    /* --------------------------- */

                    /* --- Change Command Step --- */
                    /* --------------------------- */
                    InitialCommandButton(2);
                    /* --------------------------- */
                }
            };

            if (controls != undefined) {
                return;
            }
            else if (result != undefined) {
                if (result == "ProdError") {
                    ajax_method.CallScreenController("/Quotation/QUS030_CheckMaintenanceDetailQuotationData_P2", obj, function (result, controls) {
                        hFunc(controls);
                        if (controls != undefined)
                            return;

                        ma_success_method(result);
                    });
                }
                else {
                    ma_success_method(result);
                }
            }
        });
        /* ----------------------------- */
    }
}
function ChangeMaintenanceDetailSectionMode(view_mode) {
    /// <summary>Method to change to View/Edit mode for Maintenance Detail section</summary>
    /// <param name="view_mode" type="bool">Flag to set View mode</param>
    var screen = ["QUS030_01", "QUS030_05", "QUS030_06", "QUS030_15"];
    ChangeAllSectionMode(screen, view_mode);
}
/* ----------------------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */






/* --- Screen Methods ---------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function ResetSection() {
    /// <summary>Method to clear all section</summary>
    var screen = ["QUS030_02",
                  "QUS030_03",
                  "QUS030_04",
                  "QUS030_05",
                  "QUS030_06",
                  "QUS030_07",
                  "QUS030_08",
                  "QUS030_09",
                  "QUS030_10",
                  "QUS030_11",
                  "QUS030_12",
                  "QUS030_13",
                  "QUS030_14",
                  "QUS030_15",
                  "QUS030_16"];

    ResetAllScreen(screen);
}

//function CallQUS030_02(clear, func) {
//    /// <summary>Method to create Quotation detail for SALE section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divQUS030_02", "Quotation", "QUS030_02", "", clear, func);
//}
//function CallQUS030_03(clear, func) {
//    /// <summary>Method to create Quotation detail for AL section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divQUS030_03", "Quotation", "QUS030_03", "", clear, func);
//}
//function CallQUS030_04(clear, func) {
//    /// <summary>Method to create Quotation detail for ONLINE section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divQUS030_04", "Quotation", "QUS030_04", "", clear, func);
//}
//function CallQUS030_05(clear, func) {
//    /// <summary>Method to create Quotation detail for BE, SG, MA section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divQUS030_05", "Quotation", "QUS030_05", "", clear, func);
//}
//function CallQUS030_06(clear, func) {
//    /// <summary>Method to create Maintenance detail section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divQUS030_06", "Quotation", "QUS030_06", "", clear, func);
//}
//function CallQUS030_07(clear, func) {
//    /// <summary>Method to create Instrument detail for SALE, AL (before 1st complete) section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divQUS030_07", "Quotation", "QUS030_07", "", clear, func);
//}
//function CallQUS030_08(clear, func) {
//    /// <summary>Method to create Instrument detail for AL (after 1st complete) section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divQUS030_08", "Quotation", "QUS030_08", "", clear, func);
//}
//function CallQUS030_09(clear, func) {
//    /// <summary>Method to create Instrument detail for ONLINE section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divQUS030_09", "Quotation", "QUS030_09", "", clear, func);
//}
//function CallQUS030_10(clear, func) {
//    /// <summary>Method to create Facility detail section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divQUS030_10", "Quotation", "QUS030_10", "", clear, func);
//}
//function CallQUS030_11(clear, func) {
//    /// <summary>Method to create Beat guard detail section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divQUS030_11", "Quotation", "QUS030_11", "", clear, func);
//}
//function CallQUS030_12(clear, func) {
//    /// <summary>Method to create Sentry guard detail section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divQUS030_12", "Quotation", "QUS030_12", "", clear, func);
//}
//function CallQUS030_13(clear, func) {
//    /// <summary>Method to create Fee information for SALE section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divQUS030_13", "Quotation", "QUS030_13", "", clear, func);
//}
//function CallQUS030_14(clear, func) {
//    /// <summary>Method to create Fee information for AL section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divQUS030_14", "Quotation", "QUS030_14", "", clear, func);
//}
//function CallQUS030_15(clear, func) {
//    /// <summary>Method to create Fee information for ONLINE, BE, SG, MA section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divQUS030_15", "Quotation", "QUS030_15", "", clear, func);
//}
//function CallQUS030_16(clear, func) {
//    /// <summary>Method to create Result of register quotation detail section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divQUS030_16", "Quotation", "QUS030_16", "", clear, func);
//}
/* ----------------------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */



var instrumentObject;
var searchInstReturnFunction;
function SearchInstrument(obj, func) {
    instrumentObject = obj;
    searchInstReturnFunction = func;

    $("#dlgBox").OpenCMS170Dialog("QUS030");
}
function CMS170Object() {
    return instrumentObject;
}
function CMS170Response(result) {
    $("#dlgBox").CloseDialog();

    if (typeof (searchInstReturnFunction) == "function")
        searchInstReturnFunction(result);
}