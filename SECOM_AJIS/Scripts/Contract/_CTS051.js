/*--- Main ---*/

var mygrid;
var mygridBilling;
var mygridBillingDetail;
var gridRemoveFlagLst = new Array();
var pageRow = 20;
var mode;
var eventCopyNameComeFrom;
var mas030Object = null;
var optionalItem = null;
var contractIsBefore = true;
var validateRegisterFormControl = ["ChangePlanNormalContractFee"
, "ChangePlanNormalInstallationFee"
, "ChangePlanNormalDepositFee"
, "ChangePlanOrderContractFee"
, "ChangePlanOrderInstallationFee"
, "ChangePlanOrderDepositFee"
, "ChangePlanCompleteInstallationFee"
, "ChangePlanStartInstallationFee"
, "NegotiationStaffEmpNo1"
, "BillingContractFeeDetail"
, "BillingDepositFee"
, "BillingCompleteInstallationFee"
, "BillingStartInstallationFee"
, "PayMethodCompleteFee"
, "PayMethodStartServiceFee"
, "PayMethodDepositFee"
, "BillingTimingType"
];
var validateRetrieveBillingFormControl = ["BillingTargetCode"];
var validateAddEditBillingFormControl = ["txtBillingContractFee"
, "txtBillingInstallationFee"
, "InstallationFee"
, "txtBillingApproveInstallationFee"
, "txtBillingCompleteInstallationFee"
, "CompleteInstallationFee"
, "txtBillingStartInstallationFee"
, "StartServiceInstallationFee"
, "txtAmountTotal"
, "txtBillingDepositFee"
, "DepositInstallationFee"
, "BillingOffice"];
var ChangePlanCompleteInstallationEnable = false;
var ChangePlanStartInstallationEnable = false;
var ChangePlanApproveInstallationEnable = false;

var currBillingOCC = null;
var currBillingClientCode = null;
var currBillingTargetCode = null;

$(document).ready(function () {
    LoadOptionalData();
});

function LoadOptionalData() {
    call_ajax_method_json('/Contract/LoadOptionalData_CTS051', "",
        function (result, controls) {
            optionalItem = result;
            InitOnLoad();
        });
}

function InitOnLoad() {
    DisableAllCommand();
    InitialControlProperty();
    ISDisableChangePlanDetailSection(true);
    ISDisableBillingTargetSection(true);
    ISDisableBillingTargetDetailSection(true);
    MaintainScreenItemOnInit();
    ISDisableNewRecordSection(true);
    $("#BillingTargetDetailSection").SetViewMode(false);
    $("#ContractChange").SetViewMode(false);

    ISHideChangePlanSectionSection(true);
    ISHideBillingTargetSection(true);
    ISHideBillingTargetDetailSection(true);

    $("#chkIsimportantCustomer").attr("readonly", true);
    $("#chkIsimportantCustomer").attr("disabled", true);

    $('#Alphabet').blur(function () {
        $('#Alphabet').val($('#Alphabet').val().toUpperCase());
    });

    $('#ContractDurationMonth').BindNumericBox(3, 0, 1, 999, false);
//    $('#ContractDurationMonth').setComma();
    $('#AutoRenewMonth').BindNumericBox(3, 0, 1, 999, false);
//    $('#AutoRenewMonth').setComma();

    // Init numeric Box in table
    $('#ChangePlanNormalContractFee').BindNumericBox(10, 2, 0, 9999999999, 0);
    $('#ChangePlanOrderContractFee').BindNumericBox(10, 2, 0, 9999999999, 0);
    $('#ChangePlanNormalInstallationFee').BindNumericBox(10, 2, 0, 9999999999, 0);
    $('#ChangePlanOrderInstallationFee').BindNumericBox(10, 2, 0, 9999999999, 0);
    $('#ChangePlanApproveInstallationFee').BindNumericBox(10, 2, 0, 9999999999, 0);
    $('#ChangePlanCompleteInstallationFee').BindNumericBox(10, 2, 0, 9999999999, 0);
    $('#ChangePlanStartInstallationFee').BindNumericBox(10, 2, 0, 9999999999, 0);
    $('#ChangePlanNormalDepositFee').BindNumericBox(10, 2, 0, 9999999999, 0);
    $('#ChangePlanOrderDepositFee').BindNumericBox(10, 2, 0, 9999999999, 0);
    $("#txtBillingContractFee").BindNumericBox(12, 2, 0, 9999999999);
    $("#txtBillingInstallationFee").BindNumericBox(12, 2, 0, 9999999999);
    $("#txtBillingApproveInstallationFee").BindNumericBox(12, 2, 0, 9999999999);
    $("#txtBillingCompleteInstallationFee").BindNumericBox(12, 2, 0, 9999999999);
    $("#txtBillingStartInstallationFee").BindNumericBox(12, 2, 0, 9999999999);
    $("#txtAmountTotal").BindNumericBox(12, 2, 0, 9999999999);
    $("#txtBillingDepositFee").BindNumericBox(12, 2, 0, 9999999999);

    $("#txtBillingCompleteInstallationFee").blur(function () { GetTotalBilling(); })
    $("#txtBillingStartInstallationFee").blur(function () { GetTotalBilling(); })
    $("#txtBillingDepositFee").blur(function () { GetTotalBilling(); })
}

function InitialControlProperty() {
    $("#QuotationTargetCode").attr("readonly", true);
    $("#ContractDurationMonth").attr("readonly", true);
    $("#AutoRenewMonth").attr("readonly", true);
    $("#AutoRenewMonth").attr("readonly", true);
    $("#NegotiationStaffEmpName1").attr("readonly", true);
    $("#NegotiationStaffEmpName2").attr("readonly", true);

    $("#ExpectedOperationDate").InitialDate();
    $("#EndContractDate").InitialDate();

    $("#ContractDurationFlag").click(
    function () {
        if ($('#ContractDurationFlag').prop('checked') == true) {
            $("#ContractDurationMonth").attr("readonly", false);
            $("#AutoRenewMonth").attr("readonly", false);
            //$("#EndContractDate").attr("readonly", false);
            $("#EndContractDate").EnableDatePicker(true);
        }
        else {
            $("#ContractDurationMonth").attr("readonly", true);
            $("#AutoRenewMonth").attr("readonly", true);
            //$("#EndContractDate").attr("readonly", true);
            $("#EndContractDate").EnableDatePicker(false);

            $("#ContractDurationMonth").val("");
            $("#AutoRenewMonth").val("");
            $("#EndContractDate").val("");

            call_ajax_method_json('/Contract/GetQuotationInformationDetail_CTS051', "",
            function (result, controls) {

                if (result.ContractDurationMonth != null)
                    $("#ContractDurationMonth").val(result.ContractDurationMonth);

                if (result.AutoRenewMonth != null)
                    $("#AutoRenewMonth").val(result.AutoRenewMonth);

                if (result.EndContractDate != null)
                    $("#EndContractDate").val(result.EndContractDate);

            });
        }
    })

    $("#DisplayAll").click(
    function () { GetBillingTargetInformation_CTS051(); });

    GetEmployeeNameData("#NegotiationStaffEmpNo1", "#NegotiationStaffEmpName1");
//    $('#NegotiationStaffEmpNo1').blur(
//    function () {
//        ObjCTS051.EmpNo = $("#NegotiationStaffEmpNo1").val();
//        call_ajax_method_json('/Contract/GetActiveEmployee_CTS051', ObjCTS051,
//        function (result, controls) {
//            if (result != null) {
//                if (result.Message != null && $("#NegotiationStaffEmpNo1").val() != "") {
//                    OpenErrorMessageDialog(result.Code, result.Message, function () { }, null);
//                    $("#NegotiationStaffEmpNo1").val("");
//                }
//                else {
//                    $("#NegotiationStaffEmpName1").val(result.EmpName);
//                }
//            }
//        });
    //    });

    GetEmployeeNameData("#NegotiationStaffEmpNo2", "#NegotiationStaffEmpName2");
//    $('#NegotiationStaffEmpNo2').blur(
//    function () {
//        ObjCTS051.EmpNo = $("#NegotiationStaffEmpNo2").val();
//        call_ajax_method_json('/Contract/GetActiveEmployee_CTS051', ObjCTS051,
//        function (result, controls) {
//            if (result != null) {
//                if (result.Message != null && $("#NegotiationStaffEmpNo2").val() != "") {
//                    OpenErrorMessageDialog(result.Code, result.Message, function () { }, null);
//                    $("#NegotiationStaffEmpNo2").val("");
//                }
//                else {
//                    $("#NegotiationStaffEmpName2").val(result.EmpName);
//                }
//            }
//        });
//    });

    $("#rdoTargetCode").click(function () {
        if ($("#rdoTargetCode").prop('checked')) {
            $("#BillingTargetCode").attr("readonly", false);
            $("#BillingClientCode").attr("readonly", true);

            $("#BillingTargetCode").val("");
            $("#BillingClientCode").val("");
        }
    })

    $("#BillingClientCode").attr("readonly", true);
    $("#rdoClientCode").click(function () {
        if ($("#rdoClientCode").prop('checked')) {
            $("#BillingClientCode").attr("readonly", false);
            $("#BillingTargetCode").attr("readonly", true);

            $("#BillingTargetCode").val("");
            $("#BillingClientCode").val("");
        }
    })

    $("#btnRetrieve").click(function () { RetrieveClick(); });
    $("#btnRetrieveBilling").click(function () { RetrieveBillingClick(); });
    $("#btnCopy").click(function () { CopyNameClick(); })
    $("#btnNew").click(function () { NewClick(); })
    $("#btnAddUpdate").click(function () { AddUpdateClick(); })
    $("#btnClearBillingTarget").click(function () { ClearBillingDetailClick(); })
    $("#btnCancelCTS051").click(function () { CancelClick(); })
    $("#btnSpecifyClear").click(function () { SpecifyClearClick(); })
    
    $("#btnSearchQuotation").click(function () {
        $("#dlgCTS051").OpenQUS010Dialog('CTS051');
    });

    $("#btnSearchBillingClient").click(function () {
        $("#dlgCTS051").OpenCMS270Dialog();
    });

    $("#btnNewEdit").click(function () {
        mas030Object = null;
        call_ajax_method_json('/Contract/GetMAS030Object_CTS051', "",
            function (result, controls) {
                mas030Object = result;
                $("#dlgCTS051").OpenMAS030Dialog("CTS051");
            }, null
        );
    })
    
    if (ObjCTS051.InstallationStatusCode == '99') {       
        $("#btnViewInstalltionDetail").attr('disabled',true);
    }
}

function MaintainScreenItemOnInit() {
//    SetRegisterCommand(true, null);
//    SetResetCommand(true, ResetClick);
//    SetConfirmCommand(false, ConfirmClick);

    //$('#btnCommandRegister').attr('disabled', true);
    $('#btnViewQuotationDetail').attr('disabled', true);
    $('#btnSpecifyClear').attr('disabled', true);
    ISDisableBillingTargetSection(true);
}

function InitialEvent() {
    $("#btnRetrieve").click(function () { RetrieveClick(); });
}

function InitialEnableDisableChangePlan_CTS051() {

    //2.8.2	In ‘Change plan (before/after)’ section
    call_ajax_method_json('/Contract/InitialEnableDisableChangePlan_CTS051', "",
    function (result, controls) {
//        $("#ChangePlanNormalContractFee").attr("readonly", true);
//        $("#ChangePlanOrderContractFee").attr("readonly", false);
//        $("#ChangePlanNormalInstallationFee").attr("readonly", false);
//        $("#ChangePlanNormalInstallationFee").attr("readonly", true);

        // Disable All
        $("#ChangePlanNormalContractFee").attr("readonly", true);
        $("#ChangePlanOrderContractFee").attr("readonly", true);
        $("#ChangePlanNormalInstallationFee").attr("readonly", true);
        $("#ChangePlanOrderInstallationFee").attr("readonly", true);
        $("#ChangePlanApproveInstallationFee").attr("readonly", true);
        $("#ChangePlanCompleteInstallationFee").attr("readonly", true);
        $("#ChangePlanStartInstallationFee").attr("readonly", true);
        $("#ChangePlanNormalDepositFee").attr("readonly", true);
        $("#ChangePlanOrderDepositFee").attr("readonly", true);
        $("#BillingTimingType").attr("disabled", true);

        // Following is always enable
        $("#ChangePlanOrderContractFee").attr("readonly", false);

        if (result.ProductTypeCode == result.Alarm) {
            if (result.ContractStatus == result.BeforeStart) {
                $("#ChangePlanOrderInstallationFee").attr("readonly", false);
                $("#ChangePlanCompleteInstallationFee").attr("readonly", false);
                $("#ChangePlanStartInstallationFee").attr("readonly", false);
                $("#ChangePlanOrderDepositFee").attr("readonly", false);
                $("#BillingTimingType").attr("disabled", false);
            } else if (result.ContractStatus == result.AfterStart) {
                $("#ChangePlanOrderInstallationFee").attr("readonly", false);
                $("#ChangePlanCompleteInstallationFee").attr("readonly", false);
                $("#ChangePlanOrderDepositFee").attr("readonly", false);
                $("#BillingTimingType").attr("disabled", false);
            }
        } else if (result.ProductTypeCode == result.SaleOnline) {
            if (result.ContractStatus == result.BeforeStart) {
                $("#ChangePlanOrderDepositFee").attr("readonly", false);
                $("#BillingTimingType").attr("disabled", false);
            } else if (result.ContractStatus == result.AfterStart) {
                $("#ChangePlanOrderDepositFee").attr("readonly", false);
                $("#BillingTimingType").attr("disabled", false);
            }
        }
//         else if (result.ProductTypeCode == result.Maintenance) {
//            if (result.ContractStatus == result.BeforeStart) { 
//            } else if (result.ContractStatus == result.AfterStart) {
//            }
//        } else if (result.ProductTypeCode == result.SentryGuard) {
//            if (result.ContractStatus == result.BeforeStart) {
//            } else if (result.ContractStatus == result.AfterStart) {
//            }
//        }

//        if (result.ProductTypeCode == result.Alarm) {
//            $("#ChangePlanOrderInstallationFee").attr("readonly", false);
//        }
//        else {
//            $("#ChangePlanOrderInstallationFee").attr("readonly", true);
//        }

//        $("#ChangePlanApproveInstallationFee").attr("readonly", true);

//        if (result.ProductTypeCode == result.Alarm) {
//            $("#ChangePlanCompleteInstallationFee").attr("readonly", false);
//        }
//        else {
//            $("#ChangePlanCompleteInstallationFee").attr("readonly", true);
//        }

//        if (result.ProductTypeCode == result.Alarm && result.ContractStatus == result.BeforeStart) {
//            $("#ChangePlanStartInstallationFee").attr("readonly", false);
//        }
//        else {
//            $("#ChangePlanStartInstallationFee").attr("readonly", true);
//        }

//        $("#ChangePlanNormalDepositFee").attr("readonly", true);

//        if (result.ProductTypeCode == result.Alarm || result.ProductTypeCode == result.SaleOnline) {
//            $("#ChangePlanOrderDepositFee").attr("readonly", false);
//            $("#BillingTimingType").attr("disabled", false);
//        }
//        else {
//            $("#ChangePlanOrderDepositFee").attr("readonly", true);
//            $("#BillingTimingType").attr("disabled", true);
//        }
    });
}

function InitialEnableDisableBillingDetailGrid_CTS051() {


}

function RetrieveClick() {
    ObjCTS051.QuotationTargetCode = $("#QuotationTargetCode").val();
    ObjCTS051.Alphabet = $("#Alphabet").val();

    call_ajax_method_json('/Contract/ValidateRetrieve_CTS051', ObjCTS051,
        function (result, controls) {
            if (controls != null) {
                VaridateCtrl(["Alphabet"], controls);
            }

            if (result == null) {
                ISDisableSearchQuotation(true);
                call_ajax_method_json('/Contract/RetrieveClick_CTS051', ObjCTS051,
                function (result, controls) {
                    if (result == true) {
                        GetQuotationInformation_CTS051();
                        mode = "Search";
                        //GetBillingTargetInformation_CTS051();
                        call_ajax_method_json('/Contract/InitialBillingTiming_CTS051', "", function (result, controls) {
                            if (result != null) {
                                regenerate_combo("#BillingTimingType", result);
                                $("#BillingTimingType").attr("disabled", true);
                                $("#btnViewQuotationDetail").initial_link(function (val) {
                                    var parameter = {
                                        QuotationTargetCode: $("#QuotationTargetCode").val(),
                                        Alphabet: $("#Alphabet").val(),
                                        HideQuotationTarget: true
                                    };

                                    $("#dlgCTS051").OpenQUS012Dialog(parameter);
                                });

                                $('#DisplayAll').removeAttr('checked');
                                GetBillingTargetInformation_CTS051();

                                ISDisableSpecifySection(false);
                                ISDisableChangePlanSection(false);
                                ISDisableChangePlanDetailSection(false);
                                ISDisableNewRecordSection(true);
                                ISDisableBillingTargetSection(false);
                                ISHideChangePlanSectionSection(false);
                                ISHideBillingTargetSection(false);
                                ISDisableSearchQuotation(true);

                                EnableRegisterCommand();

                                ISDisableDivideContractFeeBillingFlag();
                            }
                        });
                    }
                    else {
                        VaridateCtrl(["Alphabet"], controls);
                        ISDisableSearchQuotation(false);
                    }
                });
            }
        }, null);
}

function SpecifyClearClick() {
    ClearChangePlan();
    ClearBillingDetail(true);
    ClearBillingDetailGrid(true);

    $("#ChangePlanSection :input").val("");
    $("#Alphabet").val("");

    ISDisableChangePlanSection(true);
    ISDisableChangePlanDetailSection(true);
    ISDisableBillingTargetSection(true);
    ISDisableBillingTargetDetailSection(true);
    ISDisableRegister(true);
    ISHideChangePlanSectionSection(true);
    ISHideBillingTargetSection(true);
    ISHideBillingTargetDetailSection(true);
    ISDisableSearchQuotation(false);

    DisableAllCommand();

    $('#btnViewQuotationDetail').attr('disabled', true);
    $('#btnSpecifyClear').attr('disabled', true);
}

function RetrieveBillingClick() {
    VaridateCtrl(validateRetrieveBillingFormControl, null);
    var billingParameter;

    if ($("#rdoTargetCode").attr('checked')) {
        billingParameter = { BillingTargetCode: $("#BillingTargetCode").val(), Mode: mode }
        call_ajax_method_json('/Contract/ValidateRetrieveBillingTarget_CTS051', billingParameter,
            function (result, controls) {
                VaridateCtrl(validateRetrieveBillingFormControl, controls);
                if (result != null && result != false) {
                    call_ajax_method_json('/Contract/RetrieveBillingTargetClick_CTS051', billingParameter,
                        function (result, controls) {
                            if (result != null && result.Code != null) {
                                CheckEnableField();
                                ISDisableNewRecordSection(true);
                                ClearControlValueBillingTargetDetailSection();

                                ObjCTS051.BillingClientCode = result.BillingClientCodeDetail;
                                ObjCTS051.BillingOffice = result.BillingOffice;
                                ObjCTS051.Sequence = ""; //จะได้ไม่ไปดึงข้อมูลใน Billing temp ตัวเก่า

                                BindBillingDetail(result);
                                if (mode != "Update") {
                                    eventCopyNameComeFrom = "New";
                                    
                                    GetBillingTargetInformationDetailGrid_CTS051();
                                }

                            }

                        }, null
                     );
                }

            }, null
         );
    }
    else {
        billingParameter = { BillingClientCode: $("#BillingClientCode").val(), Mode: mode }
        call_ajax_method_json('/Contract/ValidateRetrieveBillingClient_CTS051', billingParameter,
            function (result, controls) {
                if (result == null) {
                    call_ajax_method_json('/Contract/RetrieveBillingClientClick_CTS051', billingParameter,
                        function (result, controls) {
                            if (result != null && result.Code != null) {
                                CheckEnableField();
                                ISDisableNewRecordSection(true);
                                ClearControlValueBillingTargetDetailSection();

                                ObjCTS051.BillingClientCode = result.BillingClientCodeDetail;
                                ObjCTS051.BillingOffice = result.BillingOffice;
                                ObjCTS051.Sequence = ""; //จะได้ไม่ไปดึงข้อมูลใน Billing temp ตัวเก่า

                                BindBillingDetail(result);
                                if (mode != "Update") {
                                    GetBillingTargetInformationDetailGrid_CTS051();
                                }
                            }

                        }, null
                     );
                }
            }, null
         );
    }

}

function ClearBillingDetailClick() {
    ISDisableNewRecordSection(false);
    //ISDisableSearchQuotation(true);
    ClearBillingDetail(false);
    ClearBillingDetailGrid(false);
    call_ajax_method_json('/Contract/ClearBillingDetailClick_CTS051', "", function (result, controls) { }, null);
}

function NewClick() {
    CheckEnableField();
    DisableAllCommand();
    mode = "New";
    MaintainScreenItemOnInit();
    $('#SeparatorBillingTargetDetail').attr('disabled', false);
    call_ajax_method_json('/Contract/NewClick_CTS051', "", function (result, controls) { }, null);
    ClearBillingDetail(true);
    ClearBillingDetailGrid(true);
    ISDisableSpecifySection(true);
    ISDisableChangePlanSection(true);
    ISDisableChangePlanDetailSection(true);
    ISDisableBillingTargetDetailSection(false);
    ISDisableNewRecordSection(false);
    ISDisableRegister(true);
    ISHideBillingTargetDetailSection(false);

    eventCopyNameComeFrom = "New";
    GetBillingTargetInformationDetailGrid_CTS051();

    GridControl.LockGrid(mygridBilling);
    IsDisableBillingGridButton(true);
    $('#BillingOffice').val('');
}

function CopyNameClick() {

    var rdoType;
    if ($("#rdoContractTarget").attr('checked')) {
        rdoType = $("#rdoContractTarget").val();
    }

    if ($("#rdoBranchOfContractTarget").attr('checked')) {
        rdoType = $("#rdoBranchOfContractTarget").val();
    }

    if ($("#rdoRealCustomer").attr('checked')) {
        rdoType = $("#rdoRealCustomer").val();
    }

    if ($("#rdoSite").attr('checked')) {
        rdoType = $("#rdoSite").val();
    }

    copyNameParameter = { EventCopyNameComeFrom: eventCopyNameComeFrom, RdoType: rdoType, Mode: mode }
    call_ajax_method_json('/Contract/CopyNameClick_CTS051', copyNameParameter,
        function (result, controls) {
            if (result != null) {
                BindBillingDetail(result);
                if (mode != "Update") {
                    ObjCTS051.Sequence = "";
                    //eventCopyNameComeFrom = "New";                
                    //GetBillingTargetInformationDetailGrid_CTS051();
                }
                ISDisableNewRecordSection(true)
            }
        }, null
     );
}

function AddUpdateClick() {
    var PayMethodCompleteFee;
    var PayMethodDepositFee;
    VaridateCtrl(validateAddEditBillingFormControl, null);

    if (jQuery.trim(ObjCTS051.ContractStatus) == "02") //After start// 
    {

        if (GetBillingInstallationFee() == "0.00")
            PayMethodCompleteFee = "None";
        else
            PayMethodCompleteFee = $("#InstallationFee").val();

        if ($("#txtBillingDepositFee").val() == "0.00")
            PayMethodDepositFee = "None";
        else
            PayMethodDepositFee = $("#DepositInstallationFee").val();

        var validateParameter = {
            BillingContractFeeDetail: $("#txtBillingContractFee").val(), 
            BillingInstallationCompleteFee: GetBillingInstallationFee(),
            BillingDepositFee: $("#txtBillingDepositFee").val(),
            BillingOfficeCode: $("#BillingOffice").val(),
            PayMethodCompleteFee: PayMethodCompleteFee,
            PayMethodDepositFee: PayMethodDepositFee,
            Mode: mode,
            FullNameEN: $("#FullNameEN").val(),
            FullNameLC: $("#FullNameLC").val(),
            AddressEN: $("#AddressEN").val(),
            AddressLC: $("#AddressLC").val(),

            BillingTargetCode: currBillingTargetCode,
            BillingClientCode: currBillingClientCode,
            BillingOCC: currBillingOCC
        };

        call_ajax_method_json('/Contract/ValidateAddUpdateRequireFieldAfterStart_CTS051', validateParameter,
            function (result, controls) {
                // Check validate control
                if ($.inArray("txtBillingInstallationFee", controls) > -1) {
                    controls[controls.length] = "txtBillingCompleteInstallationFee";
                }

                VaridateCtrl(validateAddEditBillingFormControl, controls);
                if (result == null) {
                    call_ajax_method_json('/Contract/AddUpdateAfterStartClick_CTS051', validateParameter, function (result, controls) {
                        if (result == null) {
                            EnableRegisterCommand();
                            GetBillingTargetInformation_CTS051();
                            ISDisableNewRecordSection(false);
                            ISDisableSpecifySection(false);
                            ISDisableChangePlanSection(false);
                            ISDisableChangePlanDetailSection(false);
                            //                            SetRegisterCommand(true, RegisterClick);
                            ClearBillingDetailGrid(true);
                            ClearBillingDetail(true);

                            ISDisableBillingTargetSection(false);
                            ISDisableBillingTargetDetailSection(true);
                            ISDisableRegister(false);
                            ISHideBillingTargetDetailSection(true);

                            GridControl.UnlockGrid(mygridBilling);
                            IsDisableBillingGridButton(false);
                        }
                    }, null);
                }
            }, null
         );
    }
    else {

        var PayMethodStartServiceFee;

        if (GetBillingInstallationFee() == "0.00")
            PayMethodCompleteFee = "";
        else
            PayMethodCompleteFee = $("#CompleteInstallationFee").val();

        if ($("#txtBillingDepositFee").val() == "0.00")
            PayMethodDepositFee = "";
        else
            PayMethodDepositFee = $("#DepositInstallationFee").val();

        if ($("#txtBillingStartInstallationFee").val() == "0.00")
            PayMethodStartServiceFee = "";
        else
            PayMethodStartServiceFee = $("#StartServiceInstallationFee").val();

        var validateParameter = {
            BillingContractFeeDetail: $("#txtBillingContractFee").val(),
            BillingInstallationCompleteFee: GetBillingInstallationFee(),
            BillingInstallationApproveFee: $("#txtBillingApproveInstallationFee").val(),
            BillingInstallationStartServiceFee: $("#txtBillingStartInstallationFee").val(),
            BillingDepositFee: $("#txtBillingDepositFee").val(),
            BillingOfficeCode: $("#BillingOffice").val(),
            PayMethodCompleteFee: PayMethodCompleteFee,
            PayMethodDepositFee: PayMethodDepositFee,
            PayMethodStartServiceFee: PayMethodStartServiceFee,
            Mode: mode,
            FullNameEN: $("#FullNameEN").val(),
            FullNameLC: $("#FullNameLC").val(),
            AddressEN: $("#AddressEN").val(),
            AddressLC: $("#AddressLC").val(),

            BillingTargetCode: currBillingTargetCode,
            BillingClientCode: currBillingClientCode,
            BillingOCC: currBillingOCC
        };

        call_ajax_method_json('/Contract/ValidateAddUpdateRequireFieldBeforeStart_CTS051', validateParameter,
            function (result, controls) {
                // Check validate control
                if ($.inArray("txtBillingInstallationFee", controls) > -1) {
                    controls[controls.length] = "txtBillingCompleteInstallationFee";
                }
                VaridateCtrl(validateAddEditBillingFormControl, controls);
                if (result == null) {
                    call_ajax_method_json('/Contract/AddUpdateBeforeStartClick_CTS051', validateParameter, function (result, controls) {
                        if (result == null) {
                            EnableRegisterCommand();                         
                            GetBillingTargetInformation_CTS051();
                            ISDisableNewRecordSection(false);
                            ISDisableSpecifySection(false);
                            ISDisableChangePlanSection(false);
                            ISDisableChangePlanDetailSection(false);
                            //SetRegisterCommand(true, RegisterClick);
                            ClearBillingDetailGrid(true);
                            ClearBillingDetail(true);

                            ISDisableBillingTargetSection(false);
                            ISDisableBillingTargetDetailSection(true);
                            ISDisableRegister(false);
                            ISHideBillingTargetDetailSection(true);
                            GridControl.UnlockGrid(mygridBilling);
                            IsDisableBillingGridButton(false);
                        }
                    }, null);
                }
            }, null
         );
    }
}

function CancelClick() {
    VaridateCtrl(validateRetrieveBillingFormControl, null);
    EnableRegisterCommand();
    ClearBillingDetail(true);
    ClearBillingDetailGrid(true);
    ISDisableSpecifySection(false);
    ISDisableChangePlanSection(false);
    ISDisableChangePlanDetailSection(false);
    ISDisableBillingTargetDetailSection(true);
    ISDisableBillingTargetSection(false);
    ISDisableRegister(false);
    ISHideBillingTargetDetailSection(true);
    ISDisableSearchQuotation(true);
    GetBillingTargetInformation_CTS051();
    GridControl.UnlockGrid(mygridBilling);
    IsDisableBillingGridButton(false);
}

function GetQuotationInformation_CTS051() {
    $('#gridChangePlan').clearForm();

    // Call Controller
    call_ajax_method_json('/Contract/GetQuotationInformation_CTS051', "",
        function (result, controls) {
            if (result != null) {
                $('#ChangePlanNormalContractFee').val(result[0].Normal);
                $('#ChangePlanOrderContractFee').val('');
                $('#ChangePlanNormalInstallationFee').val(result[1].Normal);
                $('#ChangePlanOrderInstallationFee').val('');
                $('#ChangePlanApproveInstallationFee').val(result[1].ApproveContract);

//                $('#ChangePlanCompleteInstallationFee').val(result[1].CompleteInstallation);
//                $('#ChangePlanStartInstallationFee').val(result[1].StartService);

                $('#ChangePlanCompleteInstallationFee').val('');
                $('#ChangePlanStartInstallationFee').val('');

                $('#ChangePlanNormalDepositFee').val(result[2].Normal);
                $('#ChangePlanOrderDepositFee').val('');

                $('#ChangePlanNormalContractFee').setComma();
                $('#ChangePlanOrderContractFee').setComma();
                $('#ChangePlanNormalInstallationFee').setComma();
                $('#ChangePlanOrderInstallationFee').setComma();
                $('#ChangePlanApproveInstallationFee').setComma();
                $('#ChangePlanCompleteInstallationFee').setComma();
                $('#ChangePlanStartInstallationFee').setComma();
                $('#ChangePlanNormalDepositFee').setComma();
                $('#ChangePlanOrderDepositFee').setComma();
            }
        });
    // Fill Value

    InitialEnableDisableChangePlan_CTS051();
}

//function GetQuotationInformation_CTS051EMP() {
//    mygridCTS051 = $("#gridChangePlan").LoadDataToGridWithInitial(0, false, false, "/Contract/GetQuotationInformation_CTS051",
//    "", "CTS051_DOChangePlanAndBillingTargetGridData", false);


//    BindOnLoadedEvent(mygridCTS051, function () {

//        mygridCTS051.cells2(0, 0).setValue($("#lblContractFeeAfterChange").val());
//        mygridCTS051.cells2(1, 0).setValue($("#lblInstallationFeeAfterChange").val());
//        mygridCTS051.cells2(2, 0).setValue($("#lblAdditionalDepositFee").val());

//        var val = mygridCTS051.cells2(0, 1).getValue();
////        mygridCTS051.cells2(0, 1).setValue(GenerateNumericBox("ChangePlanNormalContractFee", "", val, true) + " " + C_CURRENCY_UNIT + " ");
//        //        $("#ChangePlanNormalContractFee").BindNumericBox(14, 2, 0, 9999999999);
//        var rowID = mygridCTS051.getRowId(0);
//        GenerateNumericBox2(mygridCTS051, "ChangePlanNor$malContractFee", rowID, "Normal", val, 10, 2, 0, 9999999999, 0, false);
//        mygridCTS051.cells2(0, 1).setValue(mygridCTS051.cells2(0, 1).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("ChangePlanNormalContractFee", rowID);
//        $('#' + ctrlID).attr('id', 'ChangePlanNormalContractFee');
//        ctrlID = 'ChangePlanNormalContractFee';
//        $('#' + ctrlID).css('width', '80px');
//        //$("#ChangePlanNormalContractFee").css('width', '80px');

//        var val = mygridCTS051.cells2(0, 2).getValue();
////        mygridCTS051.cells2(0, 2).setValue(GenerateNumericBox("ChangePlanOrderContractFee", "", val, true) + " "+C_CURRENCY_UNIT+" " + "<span class='label-remark'>*</span>");
////        $("#ChangePlanOrderContractFee").BindNumericBox(14, 2, 0, 9999999999);
//        //        $("#ChangePlanOrderContractFee").css('width', '80px');
//        var rowID = mygridCTS051.getRowId(0);
//        GenerateNumericBox2(mygridCTS051, "ChangePlanOrderContractFee", rowID, "Order", val, 10, 2, 0, 9999999999, 0, false);
//        mygridCTS051.cells2(0, 2).setValue(mygridCTS051.cells2(0, 2).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("ChangePlanOrderContractFee", rowID);
//        $('#' + ctrlID).attr('id', 'ChangePlanOrderContractFee');
//        ctrlID = 'ChangePlanOrderContractFee';
//        $('#' + ctrlID).css('width', '80px');

//        var val = mygridCTS051.cells2(1, 1).getValue();
////        mygridCTS051.cells2(1, 1).setValue(GenerateNumericBox("ChangePlanNormalInstallationFee", "", val, true) + " " + C_CURRENCY_UNIT + " ");
////        $("#ChangePlanNormalInstallationFee").BindNumericBox(14, 2, 0, 9999999999);
//        //        $("#ChangePlanNormalInstallationFee").css('width', '80px');
//        var rowID = mygridCTS051.getRowId(1);
//        GenerateNumericBox2(mygridCTS051, "ChangePlanNormalInstallationFee", rowID, "Normal", val, 10, 2, 0, 9999999999, 0, false);
//        mygridCTS051.cells2(1, 1).setValue(mygridCTS051.cells2(1, 1).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("ChangePlanNormalInstallationFee", rowID);
//        $('#' + ctrlID).attr('id', 'ChangePlanNormalInstallationFee');
//        ctrlID = 'ChangePlanNormalInstallationFee';
//        $('#' + ctrlID).css('width', '80px');

//        var val = mygridCTS051.cells2(1, 2).getValue();
////        mygridCTS051.cells2(1, 2).setValue(GenerateNumericBox("ChangePlanOrderInstallationFee", "", val, true) + " " + C_CURRENCY_UNIT + " " + "<span class='label-remark'>*</span>");
////        $("#ChangePlanOrderInstallationFee").BindNumericBox(14, 2, 0, 9999999999);
//        //        $("#ChangePlanOrderInstallationFee").css('width', '80px');
//        var rowID = mygridCTS051.getRowId(1);
//        GenerateNumericBox2(mygridCTS051, "ChangePlanOrderInstallationFee", rowID, "Order", val, 10, 2, 0, 9999999999, 0, false);
//        mygridCTS051.cells2(1, 2).setValue(mygridCTS051.cells2(1, 2).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("ChangePlanOrderInstallationFee", rowID);
//        $('#' + ctrlID).attr('id', 'ChangePlanOrderInstallationFee');
//        ctrlID = 'ChangePlanOrderInstallationFee';
//        $('#' + ctrlID).css('width', '80px');

//        var val = mygridCTS051.cells2(1, 3).getValue();
////        mygridCTS051.cells2(1, 3).setValue(GenerateNumericBox("ChangePlanApproveInstallationFee", "", val, true) + " " + C_CURRENCY_UNIT + " ");
////        $("#ChangePlanApproveInstallationFee").BindNumericBox(14, 2, 0, 9999999999);
//        //        $("#ChangePlanApproveInstallationFee").css('width', '80px');
//        var rowID = mygridCTS051.getRowId(1);
//        GenerateNumericBox2(mygridCTS051, "ChangePlanApproveInstallationFee", rowID, "ApproveContract", val, 10, 2, 0, 9999999999, 0, false);
//        mygridCTS051.cells2(1, 3).setValue(mygridCTS051.cells2(1, 3).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("ChangePlanApproveInstallationFee", rowID);
//        $('#' + ctrlID).attr('id', 'ChangePlanApproveInstallationFee');
//        ctrlID = 'ChangePlanApproveInstallationFee';
//        $('#' + ctrlID).css('width', '80px');

//        var val = mygridCTS051.cells2(1, 4).getValue();
////        mygridCTS051.cells2(1, 4).setValue(GenerateNumericBox("ChangePlanCompleteInstallationFee", "", val, true) + " " + C_CURRENCY_UNIT + " " + "<span class='label-remark'>*</span>");
////        $("#ChangePlanCompleteInstallationFee").BindNumericBox(14, 2, 0, 9999999999);
//        //        $("#ChangePlanCompleteInstallationFee").css('width', '80px');
//        var rowID = mygridCTS051.getRowId(1);
//        GenerateNumericBox2(mygridCTS051, "ChangePlanCompleteInstallationFee", rowID, "CompleteInstallation", val, 10, 2, 0, 9999999999, 0, false);
//        mygridCTS051.cells2(1, 4).setValue(mygridCTS051.cells2(1, 4).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("ChangePlanCompleteInstallationFee", rowID);
//        $('#' + ctrlID).attr('id', 'ChangePlanCompleteInstallationFee');
//        ctrlID = 'ChangePlanCompleteInstallationFee';
//        $('#' + ctrlID).css('width', '80px');

//        var val = mygridCTS051.cells2(1, 5).getValue();
////        mygridCTS051.cells2(1, 5).setValue(GenerateNumericBox("ChangePlanStartInstallationFee", "", val, true) + " " + C_CURRENCY_UNIT + " " + "<span class='label-remark'>*</span>");
////        $("#ChangePlanStartInstallationFee").BindNumericBox(14, 2, 0, 9999999999);
//        //        $("#ChangePlanStartInstallationFee").css('width', '80px');
//        var rowID = mygridCTS051.getRowId(1);
//        GenerateNumericBox2(mygridCTS051, "ChangePlanStartInstallationFee", rowID, "StartService", val, 10, 2, 0, 9999999999, 0, false);
//        mygridCTS051.cells2(1, 5).setValue(mygridCTS051.cells2(1, 5).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("ChangePlanStartInstallationFee", rowID);
//        $('#' + ctrlID).attr('id', 'ChangePlanStartInstallationFee');
//        ctrlID = 'ChangePlanStartInstallationFee';
//        $('#' + ctrlID).css('width', '80px');

//        var val = mygridCTS051.cells2(2, 1).getValue();
////        mygridCTS051.cells2(2, 1).setValue(GenerateNumericBox("ChangePlanNormalDepositFee", "", val, true) + " " + C_CURRENCY_UNIT + " ");
////        $("#ChangePlanNormalDepositFee").BindNumericBox(14, 2, 0, 9999999999);
//        //        $("#ChangePlanNormalDepositFee").css('width', '80px');
//        var rowID = mygridCTS051.getRowId(2);
//        GenerateNumericBox2(mygridCTS051, "ChangePlanNormalDepositFee", rowID, "Normal", val, 10, 2, 0, 9999999999, 0, false);
//        mygridCTS051.cells2(2, 1).setValue(mygridCTS051.cells2(2, 1).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("ChangePlanNormalDepositFee", rowID);
//        $('#' + ctrlID).attr('id', 'ChangePlanNormalDepositFee');
//        ctrlID = 'ChangePlanNormalDepositFee';
//        $('#' + ctrlID).css('width', '80px');

//        var val = mygridCTS051.cells2(2, 2).getValue();
////        mygridCTS051.cells2(2, 2).setValue(GenerateNumericBox("ChangePlanOrderDepositFee", "", val, true) + " " + C_CURRENCY_UNIT + " " + "<span class='label-remark'>*</span>");
////        $("#ChangePlanOrderDepositFee").BindNumericBox(14, 2, 0, 9999999999);
//        //        $("#ChangePlanOrderDepositFee").css('width', '80px');
//        var rowID = mygridCTS051.getRowId(2);
//        GenerateNumericBox2(mygridCTS051, "ChangePlanOrderDepositFee", rowID, "Order", val, 10, 2, 0, 9999999999, 0, false);
//        mygridCTS051.cells2(2, 2).setValue(mygridCTS051.cells2(2, 2).getValue() + " " + C_CURRENCY_UNIT + " ");
//        var ctrlID = GenerateGridControlID("ChangePlanOrderDepositFee", rowID);
//        $('#' + ctrlID).attr('id', 'ChangePlanOrderDepositFee');
//        ctrlID = 'ChangePlanOrderDepositFee';
//        $('#' + ctrlID).css('width', '80px');

//        mygridCTS051.attachEvent("onRowSelect", function (id, ind) {
//            var row_num = mygridCTS051.getRowIndex(id);
//            if (mygridCTS051.cell.childNodes[0].tagName == "INPUT") {
//                var txt = mygridCTS051.cell.childNodes[0];

//                if (txt.disabled == false) {
//                    txt.focus();
//                }
//            }
//        });

//        InitialEnableDisableChangePlan_CTS051();
//    });

//    call_ajax_method_json('/Contract/GetQuotationInformationDetail_CTS051', "",
//    function (result, controls) {
//        $("#ApproveNo1").val(result.ApproveNo1);
//        $("#ApproveNo2").val(result.ApproveNo2);
//        $("#ApproveNo3").val(result.ApproveNo3);
//        $("#ApproveNo4").val(result.ApproveNo4);
//        $("#ApproveNo5").val(result.ApproveNo5);

//        $("#ContractDurationMonth").val(result.ContractDurationMonth);
//        $("#ContractDurationMonth").attr("readonly", true)
//        
//        $("#AutoRenewMonth").val(result.AutoRenewMonth);
//        $("#AutoRenewMonth").attr("readonly", true)

//        result.EndContractDate = ConvertDateObject(result.EndContractDate);
//        $("#EndContractDate").val(ConvertDateToTextFormat(result.EndContractDate));
//        $("#EndContractDate").attr("readonly", true)
//    });    
//}

function GetBillingTargetInformation_CTS051(event) {
    ObjCTS051.DisplayAll = $("#DisplayAll").prop('checked');
    if (event != "Remove") {
        mygridBilling = $("#gridChangePlanBillingTarget").LoadDataToGridWithInitial(0, false, false, "/Contract/GetBillingTargetInformation_CTS051", ObjCTS051, "CTS051_DOChangePlanAndBillingTargetGridData", false);
    }
    else {
        mygridBilling = $("#gridChangePlanBillingTarget").LoadDataToGridWithInitial(0, false, false, "/Contract/RemoveChangePlanGridBilling_CTS051",ObjCTS051, "CTS051_DOChangePlanAndBillingTargetGridData", false);
    }

    SpecialGridControl(mygridBilling, ["Detail", "Remove"]);

    BindOnLoadedEvent(mygridBilling, function () {
        mygridBilling.setColumnHidden(mygridBilling.getColIndexById('BillingOffice'), true);
        mygridBilling.setColumnHidden(mygridBilling.getColIndexById('Sequence'), true);
        mygridBilling.setColumnHidden(mygridBilling.getColIndexById('Show'), true);
        mygridBilling.setColumnHidden(mygridBilling.getColIndexById('Status'), true);

        var detailColinx = mygridBilling.getColIndexById('Detail');
        var removeColinx = mygridBilling.getColIndexById('Remove');

        if ((optionalItem.IsMAContract != null) && (optionalItem.IsMAContract)) {
            //            mygridBilling.setColumnHidden(mygridBilling.getColIndexById('InstallationFee'), true);
            //            mygridBilling.setColumnHidden(mygridBilling.getColIndexById('DepositFee'), true);
            //            mygridBilling.setColWidth(mygridBilling.getColIndexById('BillingTargetName'), 424);
        }

        mygridBilling.setSizes();
        gridRemoveFlagLst = new Array();

        for (var i = 0; i < mygridBilling.getRowsNum(); i++) {

            if (mygridBilling.cells2(i, mygridBilling.getColIndexById('Show')).getValue() == false) {
                mygridBilling.setRowHidden(mygridBilling.getRowId(i), true)
            }
            else {
                mygridBilling.setRowHidden(mygridBilling.getRowId(i), false)
            }

            //var enableRemove = (mygridBilling.cells2(i, mygridBilling.getColIndexById('BillingOCC')).getValue() != "" && mygridBilling.cells2(i, mygridBilling.getColIndexById('BillingOCC')).getValue() != null);
            var enableRemove = (mygridBilling.cells2(i, mygridBilling.getColIndexById('CanDelete')).getValue() == "1");
            GenerateDetailButton(mygridBilling, "btnDetail", mygridBilling.getRowId(i), "Detail", true);
            GenerateRemoveButton(mygridBilling, "btnRemove", mygridBilling.getRowId(i), "Remove", enableRemove);

            gridRemoveFlagLst[i] = enableRemove;

            BindGridButtonClickEvent("btnDetail", mygridBilling.getRowId(i), function (row_id) {
                ObjCTS051.BillingClientCode = mygridBilling.cells(row_id, mygridBilling.getColIndexById('BillingClientCode')).getValue();
                ObjCTS051.BillingOffice = mygridBilling.cells(row_id, mygridBilling.getColIndexById('BillingOffice')).getValue();
                ObjCTS051.Sequence = mygridBilling.cells(row_id, mygridBilling.getColIndexById('Sequence')).getValue();
                ObjCTS051.fromGrid = true;

                eventCopyNameComeFrom = "Detail";
                call_ajax_method_json('/Contract/GetBillingTargetInformationDetail_CTS051', ObjCTS051,
                    function (result, controls) {
                        if (result != null) {
                            DisableAllCommand();
                            mode = "Update";
                            //BindBillingDetail(result);
                            CheckEnableField();
                            GetBillingTargetInformationDetailGrid_CTS051();
                            ISDisableBillingTargetDetailSection(false);

                            if ((result.BillingOCC != null && result.BillingOCC != "")) {
                                ISDisableNewRecordSection(true);
                                ISDisableBTNNewEdit(true);
                                ISDisableBTNClearBillingTarget(true);
                                //ISDisableBillingOffice(true);
                            }
                            else {
                                ISDisableNewRecordSection(false);
                                if ((result.BillingTargetCode != null && result.BillingTargetCode != "")) {
                                    ISDisableBillingOffice(true);
                                    ISDisableBTNNewEdit(false);
                                    ISDisableBTNClearBillingTarget(false);
                                }
                                else {
                                    if ((result.BillingClientCodeDetail != null && result.BillingClientCodeDetail != "")) {
                                        ISDisableBillingOffice(true);
                                        ISDisableBTNNewEdit(false);
                                        ISDisableBTNClearBillingTarget(false);
                                    }
                                    else {
                                        ISDisableBillingOffice(false);
                                        ISDisableBTNNewEdit(false);
                                        ISDisableBTNClearBillingTarget(false);
                                    }
                                }
                            }

                            ISDisableRegister(true);
                            ISDisableBillingTargetSection(true);
                            ISHideBillingTargetDetailSection(false);


                            ISDisableSpecifySection(true);
                            GridControl.LockGrid(mygridBilling);
                            IsDisableBillingGridButton(true);
                            ISDisableChangePlanSection(true);
                            ISDisableChangePlanDetailSection(true);
                            ISDisableBillingTargetDetailSection(false);

                            BindBillingDetail(result);
                        }
                    });
            });

            BindGridButtonClickEvent("btnRemove", mygridBilling.getRowId(i), function (row_id) {
                ObjCTS051.BillingClientCode = mygridBilling.cells(row_id, mygridBilling.getColIndexById('BillingClientCode')).getValue();
                ObjCTS051.BillingOffice = mygridBilling.cells(row_id, mygridBilling.getColIndexById('BillingOffice')).getValue();
                ObjCTS051.Sequence = mygridBilling.cells(row_id, mygridBilling.getColIndexById('Sequence')).getValue();

                if (mygridBilling.cells2(mygridBilling.getRowIndex(row_id), mygridBilling.getColIndexById('BillingOCC')).getValue() == "" || mygridBilling.cells2(mygridBilling.getRowIndex(row_id), mygridBilling.getColIndexById('BillingOCC')).getValue() == null) {
                    RemoveChangePlanGridBilling_CTS051();
                }
            });

            //            mygridBilling.cells2(i, detailColinx).setValue("<button id='btnDetail" + i.toString() + "' style='width:65px' >Detail</button>");
            //            mygridBilling.cells2(i, removeColinx).setValue("<button id='btnRemove" + i.toString() + "' style='width:65px' >Remove</button>");

            //            if (mygridBilling.cells2(i, mygridBilling.getColIndexById('BillingOCC')).getValue() != "" && mygridBilling.cells2(i, mygridBilling.getColIndexById('BillingOCC')).getValue() != null) {
            //                $("#btnRemove" + i.toString()).attr('disabled', true);
            //            }
            //            else {
            //                $("#btnRemove" + i.toString()).attr('disabled', false);
            //            }
        }
    });

//    mygridBilling.attachEvent("onRowSelect", function (id, ind) {

//        ObjCTS051.BillingClientCode = mygridBilling.cells(id, mygridBilling.getColIndexById('BillingClientCode')).getValue();
//        ObjCTS051.BillingOffice = mygridBilling.cells(id, mygridBilling.getColIndexById('BillingOffice')).getValue();
//        ObjCTS051.Sequence = mygridBilling.cells(id, mygridBilling.getColIndexById('Sequence')).getValue();

//        if ($('#BillingTargetSection').prop('disabled') == false) {
//            if (ind == mygridBilling.getColIndexById('Detail')) {
//                eventCopyNameComeFrom = "Detail";
//                call_ajax_method_json('/Contract/GetBillingTargetInformationDetail_CTS051', ObjCTS051,
//                    function (result, controls) {
//                        mode = "Update";
//                        BindBillingDetail(result);
//                        GetBillingTargetInformationDetailGrid_CTS051();
//                        ISDisableBillingTargetDetailSection(false);

//                        if ((result.BillingOCC != null && result.BillingOCC != "")) {
//                            ISDisableNewRecordSection(true);
//                            ISDisableBTNNewEdit(true);
//                            ISDisableBTNClearBillingTarget(true);
//                            ISDisableBillingOffice(true);
//                        }
//                        else {
//                            ISDisableNewRecordSection(false);
//                            if ((result.BillingTargetCode != null && result.BillingTargetCode != "")) {
//                                ISDisableBillingOffice(true);
//                                ISDisableBTNNewEdit(false);
//                                ISDisableBTNClearBillingTarget(false);
//                            }
//                            else {
//                                if ((result.BillingClientCodeDetail != null && result.BillingClientCodeDetail != "")) {
//                                    ISDisableBillingOffice(true);
//                                    ISDisableBTNNewEdit(false);
//                                    ISDisableBTNClearBillingTarget(false);
//                                }
//                                else {
//                                    ISDisableBillingOffice(false);
//                                    ISDisableBTNNewEdit(false);
//                                    ISDisableBTNClearBillingTarget(false);
//                                }
//                            }
//                        }

//                        ISDisableRegister(true);
//                        ISHideBillingTargetDetailSection(false);

//                    });
//            }

//            if (ind == mygridBilling.getColIndexById('Remove')) {
//                if (mygridBilling.cells2(mygridBilling.getRowIndex(id), mygridBilling.getColIndexById('BillingOCC')).getValue() == "" || mygridBilling.cells2(mygridBilling.getRowIndex(id), mygridBilling.getColIndexById('BillingOCC')).getValue() == null) {
//                    RemoveChangePlanGridBilling_CTS051();
//                }
//            }
//        }
//    });
}

function RemoveChangePlanGridBilling_CTS051() {
    GetBillingTargetInformation_CTS051("Remove");
}

//function GetBillingTargetInformationDetailGrid_CTS051() {

//    mygridBillingDetail = $("#gridChangePlanBillingDetail").LoadDataToGridWithInitial(0, false, false, "/Contract/GetBillingTargetInformationDetailGrid_CTS051",
//    ObjCTS051, "CTS051_DOBillingTargetDetailGridData", false);

//    var rowID = "";
//    var paymentMethodParameter = "";
//    var val = "";

//    mygridBillingDetail.setColumnHidden(4, true);
//    BindOnLoadedEvent(mygridBillingDetail, function () {

//        if (jQuery.trim(ObjCTS051.ContractStatus) == "02") {

//            if (mode == "Update") {
//                val = IsNullToZero(mygridBillingDetail.cells2(0, 2).getValue());
//            }

//            mygridBillingDetail.cells2(0, 0).setValue($("#lblBillingContractFee").val());
//            mygridBillingDetail.cells2(1, 0).setValue($("#lblBillingInstallationFee").val());
//            mygridBillingDetail.cells2(2, 0).setValue($("#lblBillingDepositFee").val());

//            mygridBillingDetail.cells2(0, 2).setValue(GenerateNumericBox("txtBillingContractFee", "", val, true) + " " + C_CURRENCY_UNIT + " " + "<span class='label-remark'>*</span>");
//            $("#txtBillingContractFee").BindNumericBox(14, 2, 0, 9999999999);
//            $("#txtBillingContractFee").css('width', '120px');

//            //Installation Fee and Installation Payment

//            if (mode == "Update") {
//                val = IsNullToZero(mygridBillingDetail.cells2(1, 2).getValue());
//            }
//            if (mygridBillingDetail.cells2(1, 4).getValue() == false) {
//                mygridBillingDetail.cells2(1, 3).setValue("<select id='PaymentMethodInstallationFee' name='PaymentMethodInstallationFee' select><option value='0' selected='selected'>None</option></select>");
//                rowID = mygridBillingDetail.getRowId(1);
//                mygridBillingDetail.setRowHidden(rowID, true)
//            }
//            else {
//                mygridBillingDetail.cells2(1, 2).setValue(GenerateNumericBox("txtBillingCompleteInstallationFee", "", val, true) + " " + C_CURRENCY_UNIT + " " + "<span class='label-remark'>*</span>");
//                paymentMethodParameter = { id: "PaymentMethodInstallationFee" }
//                call_ajax_method_json('/Contract/GetComboBoxPaymentMethod_CTS051', paymentMethodParameter,
//                function (result, controls) {
//                    var payMethodCode = mygridBillingDetail.cells2(1, 3).getValue();
//                    payMethodCode = payMethodCode.replace("^null", "");
//                    mygridBillingDetail.cells2(1, 3).setValue(result + "<span class='label-remark'>*</span>");
//                    $("#PaymentMethodInstallationFee").val(payMethodCode);
//                });
//            }
//            $("#txtBillingCompleteInstallationFee").BindNumericBox(14, 2, 0, 9999999999);
//            $("#txtBillingCompleteInstallationFee").css('width', '120px');

//            //Deposit Fee and Deposit Payment

//            if (mode == "Update") {
//                val = IsNullToZero(mygridBillingDetail.cells2(2, 2).getValue());
//            }
//            if (mygridBillingDetail.cells2(2, 4).getValue() == false) {
//                mygridBillingDetail.cells2(2, 2).setValue(GenerateNumericBox("txtBillingDepositFee", "", "0", true) + " " + C_CURRENCY_UNIT + " " + "<span class='label-remark'>*</span>");
//                mygridBillingDetail.cells2(2, 3).setValue("<select id='PaymentMethodDepositFee' name='PaymentMethodDepositFee' select><option value='0' selected='selected'>None</option></select>");
//                rowID = mygridBillingDetail.getRowId(2);
//                mygridBillingDetail.setRowHidden(rowID, true)
//            }
//            else {
//                mygridBillingDetail.cells2(2, 2).setValue(GenerateNumericBox("txtBillingDepositFee", "", val, true) + " " + C_CURRENCY_UNIT + " " + "<span class='label-remark'>*</span>");
//                paymentMethodParameter = { id: "PaymentMethodDepositFee" }
//                call_ajax_method_json('/Contract/GetComboBoxPaymentMethod_CTS051', paymentMethodParameter,
//                function (result, controls) {
//                    var payMethodCode = mygridBillingDetail.cells2(2, 3).getValue();
//                    payMethodCode = payMethodCode.replace("^null", "");
//                    mygridBillingDetail.cells2(2, 3).setValue(result + "<span class='label-remark'>*</span>");
//                    $("#PaymentMethodDepositFee").val(payMethodCode);
//                });
//            }
//            $("#txtBillingDepositFee").BindNumericBox(14, 2, 0, 9999999999);
//            $("#txtBillingDepositFee").css('width', '120px');

//            mygridBillingDetail.enableColSpan(true);
//            rowID = mygridBillingDetail.getRowId(0);
//            mygridBillingDetail.setColspan(rowID, 0, 2);
//            rowID = mygridBillingDetail.getRowId(1);
//            mygridBillingDetail.setColspan(rowID, 0, 2);
//            rowID = mygridBillingDetail.getRowId(2);
//            mygridBillingDetail.setColspan(rowID, 0, 2);

//            //--------------------------------------------
//        }
//        else {
//            // #####################################################################################################################
//            if (mode == "Update") {
//                val = IsNullToZero(mygridBillingDetail.cells2(0, 2).getValue());
//            }

//            mygridBillingDetail.cells2(0, 0).setValue($("#lblBillingContractFee").val());

//            mygridBillingDetail.cells2(1, 0).setValue($("#lblBillingInstallationFee").val());
//            mygridBillingDetail.cells2(1, 1).setValue($("#lblApproval").val());
//            mygridBillingDetail.cells2(2, 1).setValue($("#lblCompleteInstallation").val());
//            mygridBillingDetail.cells2(3, 1).setValue($("#lblStartService").val());

//            mygridBillingDetail.cells2(4, 0).setValue($("#lblTotal").val());
//            mygridBillingDetail.cells2(5, 0).setValue($("#lblBillingDepositFee").val());

//            mygridBillingDetail.cells2(0, 2).setValue(GenerateNumericBox("txtBillingContractFee", "", val, true) + " " + C_CURRENCY_UNIT + " " + "<span class='label-remark'>*</span>");
//            $("#txtBillingContractFee").BindNumericBox(14, 2, 0, 9999999999);
//            $("#txtBillingContractFee").css('width', '120px');

//            // Approval Installation Fee and Approval Installation Payment

//            if (mode == "Update") {
//                val = IsNullToZero(mygridBillingDetail.cells2(1, 2).getValue());
//            }
//            if (mygridBillingDetail.cells2(1, 4).getValue() == false) {
//                mygridBillingDetail.cells2(1, 2).setValue(GenerateNumericBox("txtBillingApproveInstallationFee", "", "0", true) + " " + C_CURRENCY_UNIT + " ");
//                rowID = mygridBillingDetail.getRowId(1);
//                mygridBillingDetail.setRowHidden(rowID, true)
//            }
//            else {
//                mygridBillingDetail.cells2(1, 2).setValue(GenerateNumericBox("txtBillingApproveInstallationFee", "", val, true) + " " + C_CURRENCY_UNIT + " ");
//            }
//            $("#txtBillingApproveInstallationFee").BindNumericBox(14, 2, 0, 9999999999);
//            $("#txtBillingApproveInstallationFee").css('width', '120px');
//            $("#txtBillingApproveInstallationFee").attr("readonly", true);

//            // Complete Installation Fee and Complete Installation Payment

//            if (mode == "Update") {
//                val = IsNullToZero(mygridBillingDetail.cells2(2, 2).getValue());
//            }
//            if (mygridBillingDetail.cells2(2, 4).getValue() == false) {
//                mygridBillingDetail.cells2(2, 2).setValue(GenerateNumericBox("txtBillingCompleteInstallationFee", "", "0", true) + " " + C_CURRENCY_UNIT + " " + "<span class='label-remark'>*</span>");
//                mygridBillingDetail.cells2(2, 3).setValue("<select id='CompleteInstallationFee' name='CompleteInstallationFee' select><option value='0' selected='selected'>None</option></select>");
//                rowID = mygridBillingDetail.getRowId(2);
//                mygridBillingDetail.setRowHidden(rowID, true)
//            }
//            else {
//                mygridBillingDetail.cells2(2, 2).setValue(GenerateNumericBox("txtBillingCompleteInstallationFee", "", val, true) + " " + C_CURRENCY_UNIT + " " + "<span class='label-remark'>*</span>");
//                var paymentMethodParameter = { id: "CompleteInstallationFee" }
//                call_ajax_method_json('/Contract/GetComboBoxPaymentMethod_CTS051', paymentMethodParameter,
//                function (result, controls) {
//                    var payMethodCode = mygridBillingDetail.cells2(2, 3).getValue();
//                    payMethodCode = payMethodCode.replace("^null", "");
//                    mygridBillingDetail.cells2(2, 3).setValue(result + "<span class='label-remark'>*</span>");
//                    $("#CompleteInstallationFee").val(payMethodCode);
//                });
//            }
//            $("#txtBillingCompleteInstallationFee").BindNumericBox(14, 2, 0, 9999999999);
//            $("#txtBillingCompleteInstallationFee").css('width', '120px');
//            $("#txtBillingCompleteInstallationFee").blur(function () { GetTotalBilling(); })

//            // Start Service Installation Fee and Start Service Installation Payment

//            if (mode == "Update") {
//                val = IsNullToZero(mygridBillingDetail.cells2(3, 2).getValue());
//            }
//            if (mygridBillingDetail.cells2(3, 4).getValue() == false) {
//                mygridBillingDetail.cells2(3, 2).setValue(GenerateNumericBox("txtBillingStartInstallationFee", "", "0", true) + " " + C_CURRENCY_UNIT + " " + "<span class='label-remark'>*</span>");
//                mygridBillingDetail.cells2(3, 3).setValue("<select id='StartServiceInstallationFee' name='StartServiceInstallationFee' select><option value='0' selected='selected'>None</option></select>");
//                rowID = mygridBillingDetail.getRowId(3);
//                mygridBillingDetail.setRowHidden(rowID, true)
//            }
//            else {
//                mygridBillingDetail.cells2(3, 2).setValue(GenerateNumericBox("txtBillingStartInstallationFee", "", val, true) + " " + C_CURRENCY_UNIT + " " + "<span class='label-remark'>*</span>");
//                var paymentMethodParameter = { id: "StartServiceInstallationFee" }
//                call_ajax_method_json('/Contract/GetComboBoxPaymentMethod_CTS051', paymentMethodParameter,
//                function (result, controls) {
//                    var payMethodCode = mygridBillingDetail.cells2(3, 3).getValue();
//                    payMethodCode = payMethodCode.replace("^null", "");
//                    mygridBillingDetail.cells2(3, 3).setValue(result + "<span class='label-remark'>*</span>");
//                    $("#StartServiceInstallationFee").val(payMethodCode);
//                });
//            }
//            $("#txtBillingStartInstallationFee").BindNumericBox(14, 2, 0, 9999999999);
//            $("#txtBillingStartInstallationFee").css('width', '120px');
//            $("#txtBillingStartInstallationFee").blur(function () { GetTotalBilling(); })

//            // Deposit Installation Fee and Deposit Installation Payment

//            if (mode == "Update") {
//                val = IsNullToZero(mygridBillingDetail.cells2(5, 2).getValue());
//            }
//            if (mygridBillingDetail.cells2(5, 4).getValue() == false) {
//                mygridBillingDetail.cells2(5, 2).setValue(GenerateNumericBox("txtBillingDepositFee", "", "0", true) + " " + C_CURRENCY_UNIT + " " + "<span class='label-remark'>*</span>");
//                mygridBillingDetail.cells2(5, 3).setValue("<select id='DepositInstallationFee' name='DepositInstallationFee' select><option value='0' selected='selected'>None</option></select>");
//                rowID = mygridBillingDetail.getRowId(5);
//                mygridBillingDetail.setRowHidden(rowID, true)
//            }
//            else {
//                mygridBillingDetail.cells2(5, 2).setValue(GenerateNumericBox("txtBillingDepositFee", "", val, true) + " " + C_CURRENCY_UNIT + " " + "<span class='label-remark'>*</span>");
//                var paymentMethodParameter = { id: "DepositInstallationFee" }
//                call_ajax_method_json('/Contract/GetComboBoxPaymentMethod_CTS051', paymentMethodParameter,
//                function (result, controls) {
//                    var payMethodCode = mygridBillingDetail.cells2(5, 3).getValue();
//                    payMethodCode = payMethodCode.replace("^null", "");
//                    mygridBillingDetail.cells2(5, 3).setValue(result + "<span class='label-remark'>*</span>");
//                    $("#DepositInstallationFee").val(payMethodCode);
//                });
//            }
//            $("#txtBillingDepositFee").BindNumericBox(14, 2, 0, 9999999999);
//            $("#txtBillingDepositFee").css('width', '120px');
//            $("#txtBillingDepositFee").blur(function () { GetTotalBilling(); })

//            // Total Installation Fee

//            if (mode == "Update") {
//                val = IsNullToZero(mygridBillingDetail.cells2(4, 2).getValue());
//            }
//            if (mygridBillingDetail.cells2(4, 4).getValue() == false) {
//                mygridBillingDetail.cells2(4, 2).setValue(GenerateNumericBox("txtAmountTotal", "", "0", true) + " " + C_CURRENCY_UNIT + " ");
//                rowID = mygridBillingDetail.getRowId(4);
//                mygridBillingDetail.setRowHidden(rowID, true)
//            }
//            else {
//                mygridBillingDetail.cells2(4, 2).setValue(GenerateNumericBox("txtAmountTotal", "", val, true) + " " + C_CURRENCY_UNIT + " ");
//                rowID = mygridBillingDetail.getRowId(4);
//                mygridBillingDetail.setColspan(rowID, 0, 2);
//            }
//            $("#txtAmountTotal").BindNumericBox(14, 2, 0, 9999999999);
//            $("#txtAmountTotal").css('width', '120px');
//            $("#txtAmountTotal").attr("readonly", true);
//        }

//        mygridBillingDetail.enableColSpan(true);
//        rowID = mygridBillingDetail.getRowId(0);
//        mygridBillingDetail.setColspan(rowID, 0, 2);
//        rowID = mygridBillingDetail.getRowId(5);
//        mygridBillingDetail.setColspan(rowID, 0, 2);

//        rowID = mygridBillingDetail.getRowId(1);
//        mygridBillingDetail.setRowspan(rowID, 0, 3);

//        mygridBillingDetail.cells2(1, 0).cell.style.verticalAlign = 'middle';
//        mygridBillingDetail.cells2(1, 0).cell.style.textAlign = 'center';

//        mygridBillingDetail.attachEvent("onRowSelect", function (id, ind) {
//            var row_num = mygridBillingDetail.getRowIndex(id);
//            if (mygridBillingDetail.cell.childNodes[0].tagName == "INPUT") {
//                var txt = mygridBillingDetail.cell.childNodes[0];
//                if (txt.disabled == false) {
//                    txt.focus();
//                }
//            }
//        });
//    });

//}

function GetBillingTargetInformationDetailGrid_CTS051() {
    call_ajax_method_json('/Contract/GetBillingTargetInformationDetailGrid_CTS051', ObjCTS051,
        function (result, controls) {
            if (result != null) {
                if (result.length == 3) {
                    // After
                    contractIsBefore = false;
                    ShowAfterBillingDetailGrid();
                    $('#txtBillingContractFee').val(result[0].Amount);
                    $('#txtBillingInstallationFee').val(result[1].Amount);
                    $('#InstallationFee').val(result[1].PayMethod);
                    $('#txtBillingDepositFee').val(result[2].Amount);
                    $('#PaymentMethodDepositFee').val(result[2].PayMethod);
                } else if (result.length == 6) {
                    // Before
                    contractIsBefore = true;
                    ShowBeforeBillingDetailGrid();
                    $('#txtBillingContractFee').val(result[0].Amount);
                    $('#txtBillingApproveInstallationFee').val(result[1].Amount);
                    $('#txtBillingCompleteInstallationFee').val(result[2].Amount);
                    $('#CompleteInstallationFee').val(result[2].PayMethod);
                    $('#txtBillingStartInstallationFee').val(result[3].Amount);
                    $('#StartServiceInstallationFee').val(result[3].PayMethod);
                    $('#txtAmountTotal').val(result[4].Amount);
                    $('#txtBillingDepositFee').val(result[5].Amount);
                    $('#DepositInstallationFee').val(result[5].PayMethod);

                    if (ChangePlanApproveInstallationEnable) {
                        $('#txtBillingApproveInstallationFee').attr('readonly', 'true');
                        $('#txtBillingApproveInstallationFee').val('0.00');
                    } else {
                        $('#txtBillingApproveInstallationFee').removeAttr('readonly');
                    }

                    if (!ChangePlanCompleteInstallationEnable) {
                        $('#txtBillingCompleteInstallationFee').attr('readonly', 'true');
                        $('#txtBillingCompleteInstallationFee').val('0.00');
                    } else {
                        $('#txtBillingCompleteInstallationFee').removeAttr('readonly');
                    }

                    if (!ChangePlanStartInstallationEnable) {
                        $('#txtBillingStartInstallationFee').attr('readonly', 'true');
                        $('#txtBillingStartInstallationFee').val('0.00');
                    } else {
                        $('#txtBillingStartInstallationFee').removeAttr('readonly');
                    }

                    $("#txtAmountTotal").attr("readonly", true);
                    $("#txtBillingApproveInstallationFee").attr("readonly", true);

                    GetTotalBilling();
                }
            }
        });
    }

    function CheckEnableField() {
        ChangePlanCompleteInstallationEnable = ($('#ChangePlanCompleteInstallationFee').attr('readonly') == null);
        ChangePlanStartInstallationEnable = ($('#ChangePlanStartInstallationFee').attr('readonly') == null);
        ChangePlanApproveInstallationEnable = ($('#ChangePlanApproveInstallationFee').attr('readonly') == null);
    }

    function GetBillingInstallationFee() {
        var res = null;

        if (contractIsBefore) {
            // Get from Complete Install
            if ($('#txtBillingCompleteInstallationFee') != null) {
                res = $('#txtBillingCompleteInstallationFee').val();
            }
        } else {
            // Get from Install
            if ($('#txtBillingInstallationFee') != null) {
                res = $('#txtBillingInstallationFee').val();
            }
        }

        return res;
    }

    function GetBillingInstallationPaymentMethod() {
        var res = null;

        if (contractIsBefore) {
            // Get from Complete Install
            if ($('#CompleteInstallationFee') != null) {
                res = $('#CompleteInstallationFee').val();
            }
        } else {
            // Get from Install
            if ($('#InstallationFee') != null) {
                res = $('#InstallationFee').val();
            }
        }

        return res;
    }

function GetTotalBilling() {
    var parameterGetTotalBilling = { completeAmount: GetBillingInstallationFee(), startAmount: $("#txtBillingStartInstallationFee").val() }

    call_ajax_method_json('/Contract/GetTotalBilling_CTS051', parameterGetTotalBilling,
    function (result, controls) {
        $("#txtAmountTotal").val(result.Total);
    });
}

function BindBillingDetail(result) {
    currBillingOCC = result.BillingOCC;
    currBillingClientCode = result.BillingClientCodeDetail;
    currBillingTargetCode = result.BillingTargetCodeDetail;

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

    if (result.BillingOfficeCode != null && result.BillingOfficeCode != "") {
        $("#BillingOffice").val(result.BillingOfficeCode);
    }

    if (result.BillingTargetCodeDetail != null && result.BillingTargetCodeDetail != "") {
        $("#BillingOffice").attr('disabled', 'disabled');
    } else {
        $("#BillingOffice").removeAttr('disabled');
    }

    $("#IDNo").val(result.IDNo);
}

function ShowBeforeBillingDetailGrid() {
    $('#trInstallFee').hide();
    $('#trApproval').show();
    $('#trCompleteInstall').show();
    $('#trStartService').show();
    $('#trTotal').show();
    $('#trDepositFee').attr('class', 'odd_dhx_secom');
}

function ShowAfterBillingDetailGrid() {
    $('#trInstallFee').show();
    $('#trApproval').hide();
    $('#trCompleteInstall').hide();
    $('#trStartService').hide();
    $('#trTotal').hide();
    $('#trDepositFee').removeAttr('class');
}

function ClearBillingDetailGrid() {
    $('#gridChangePlanBillingDetail').clearForm();
}

function ClearChangePlan() {
    $("#ChangePlanNormalContractFee").val();
    $("#ChangePlanNormalInstallationFee").val();
    $("#ChangePlanNormalDepositFee").val();
    $("#ChangePlanOrderContractFee").val();
    $("#ChangePlanOrderInstallationFee").val();
    $("#ChangePlanOrderDepositFee").val();
    $("#ChangePlanApproveInstallationFee").val();
    $("#ChangePlanCompleteInstallationFee").val();
    $("#ChangePlanStartInstallationFee").val();
}

function ClearBillingDetail(clearAll) {
    $("#BillingTargetCodeDetail").val("");
    $("#BillingTargetCodeDetail").val("");
    $("#BillingTargetCodeDetail").val("");
    $("#BillingClientCodeDetail").val("");
    $("#FullNameEN").val("");
    $("#BranchNameEN").val("");
    $("#AddressEN").val("");
    $("#FullNameLC").val("");
    $("#BranchNameLC").val("");
    $("#AddressLC").val("");
    $("#Nationality").val("");
    $("#PhoneNo").val("");
    $("#BusinessType").val("");
    $("#IDNo").val("");

    $("#BillingTargetCode").val("");
    $("#BillingClientCode").val("");
    $("#rdoTargetCode").attr('checked', true)
    $("#rdoContractTarget").attr('checked', true)

    if ((clearAll != null) && (clearAll))
    {
        if ($("#BillingOffice").prop("disabled") == false) {
            $("#BillingOffice").val("");
        }   
    }
}

function ClearBillingDetailGrid(clearAll) {
    if (jQuery.trim(ObjCTS051.ContractStatus) == "02") {
        $("#txtBillingContractFee").val("");
        $("#txtBillingCompleteInstallationFee").val("");
        $("#txtBillingInstallationFee").val("");
        $("#txtBillingDepositFee").val("");

        $("#InstallationFee").val("");
        $("#DepositInstallationFee").val("");
    }
    else {
        if ((clearAll != null) && (clearAll))
        {
            $("#txtBillingContractFee").val("");
            $("#txtBillingApproveInstallationFee").val("");
            $("#txtBillingCompleteInstallationFee").val("");
            $("#txtBillingInstallationFee").val("");
            $("#txtBillingStartInstallationFee").val("");
            $("#txtBillingDepositFee").val("");
            $("#txtAmountTotal").val("");

            $("#CompleteInstallationFee").val("");
            $("#StartServiceInstallationFee").val("");
            $("#DepositInstallationFee").val("");
        }
    }
}

function RegisterClick() {
    VaridateCtrl(validateRegisterFormControl, null);
    //$(this).removeClass("highlight");

    call_ajax_method_json('/Contract/ValidateRegister_CTS051', GetValidateObject(),
    function (result, controls) {
        VaridateCtrl(validateRegisterFormControl, controls);
        if (result == null) {
            call_ajax_method_json('/Contract/RegisterClick_CTS051', GetValidateObject(),
            function (result, controls) {
                if (result == null) {
                    EnableConfirmCommand();
                    $("#ContractChange").SetViewMode(true);
                    ISDisableSpecifySection(true);
                    ISDisableChangePlanSection(true);
                    ISDisableChangePlanDetailSection(true);
                    ISDisableBillingTargetSection(true);

                    $('#divQuotationTargetCode').html($('#QuotationTargetCode').val() + '-' + $('#Alphabet').val());
                }
            });
        }
    });
}

function ConfirmClick() {
    var obj = GetValidateObject();
    call_ajax_method_json('/Contract/ValidateConfirm_CTS051', obj, function (result, controls) {
        if (result == null) {
            call_ajax_method_json('/Contract/ConfirmClick_CTS051', obj, function (result, controls) {
                OpenInformationMessageDialog(result.Code, result.Message, function () {
                        window.location.href = generate_url("/common/CMS020");                    
                }, null);                
            });
        }        
    });
}

function ResetClick() {
    /* --- Get Message --- */
    var obj = {
        module: "Common",
        code: "MSG0038"
    };
    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenOkCancelDialog(result.Code, result.Message,
        function () {
            call_ajax_method_json('/Contract/ResetData_CTS051', "", function (result, controls) {
                if ((result != null)) {
                    $('#Alphabet').val("");
                    CancelClick();
                    ISHideChangePlanSectionSection(true);
                    ISHideBillingTargetSection(true);
                    ISHideBillingTargetDetailSection(true);
                    ISDisableSearchQuotation(false);
                    MaintainScreenItemOnInit();

                    DisableAllCommand();

                    ReBindingData(result);
                }
            });
        },
        null);
    });
}

function BackClick() {
    /* --- Set Command Button --- */
    EnableRegisterCommand();
    $("#ContractChange").SetViewMode(false);
    ISDisableSpecifySection(false);
    ISDisableChangePlanSection(false);
    ISDisableChangePlanDetailSection(false);
    ISDisableBillingTargetSection(false);
    GetBillingTargetInformation_CTS051();
}

function ReBindingData(dataObj) {
    ObjCTS051 = { ContractCodeShort: dataObj.ContractCodeShort
        , ContractCode: dataObj.ContractCode
        , OCC: dataObj.OCC
        , ContractStatus: dataObj.ContractStatus
        , QuotationTargetCode: dataObj.QuotationTargetCode
        , Alphabet: ""
        , EmpNo: ""
        , EmpName: ""
        , DisplayAll: ""
        , BillingClientCode: ""
        , BillingOffice: ""
        , PaymentMethod: ""
        , Sequence: ""
        , ServiceTypeCode: dataObj.ServiceTypeCode
        , TargetCodeType: dataObj.TargetCodeType
        , EndContractDate: dataObj.EndContractDate
        , InstallationStatusCode: dataObj.InstallationStatusCode
    }

    $('#txtContractCode').val(dataObj.ContractCodeShort);
    $('#txtUserCode').val(dataObj.UserCode);
    $('#txtCustomerCode').val(dataObj.CustomerCode);
    $('#txtCustomerCodeReal').val(dataObj.RealCustomerCode);
    $('#txtSiteCode').val(dataObj.SiteCode);
    if (dataObj.ImportantFlag) {
        $('#chkIsimportantCustomer').attr('checked', 'checked');
    } else {
        $('#chkIsimportantCustomer').removeAttr('checked');
    }
    $('#txtContractTargetName').val(dataObj.CustFullNameEN);
    $('#txtContractTargetAddress').val(dataObj.AddressFullEN);
    $('#txtSiteName').val(dataObj.SiteName);
    $('#txtSiteAddress').val(dataObj.SiteAddress);
    $('#txtCustFullNameLC').val(dataObj.CustFullNameLC);
    $('#txtContractTargetAddressLocal').val(dataObj.AddressFullLC);
    $('#txtSiteNameLocal').val(dataObj.SiteNameLC);
    $('#txtSiteAddressLocal').val(dataObj.SiteAddressLC);
    $('#txtInstallationStatus').val(dataObj.InstallationStatus);
    $('#txtOperationOffice').val(dataObj.OfficeName);
    $('#QuotationTargetCode').val(dataObj.QuotationTargetCode);
}

function GenerateLabel(id, value) {
    return "<input class='numeric-box' id='" + id + "' name='" + id + "' style='width:100%;margin-left:-2px;' type='text' value='" + value + "' " + disabledxt + " />";
}

function DisabledTarget(el) {
    try {
        el.disabled = el.disabled ? false : true;
    }
    catch (E) {
    }
    if (el.childNodes && el.childNodes.length > 0) {
        for (var x = 0; x < el.childNodes.length; x++) {
            toggleDisabled(el.childNodes[x]);
        }
    }
}

function GetValidateObject() {

    var changePlanNormalContractFee;
    var changePlanNormalInstallationFee;
    var changePlanNormalDepositFee;
    var changePlanOrderContractFee;
    var changePlanOrderInstallationFee;
    var changePlanOrderDepositFee;
    var changePlanApproveInstallationFee;
    var changePlanCompleteInstallationFee;
    var changePlanStartInstallationFee;

    var billingContractFeeDetail;
    var billingCompleteInstallationFee;
    var billingDepositFee
    var billingStartInstallationFee = "0";

    var payCompleteInstallationFee;
    var payStartServiceInstallationFee;
    var payDepositInstallationFee;

    if ($('#ChangePlanNormalContractFee').attr('readonly') == true) {
        changePlanNormalContractFee = 0;
    }
    else {
        changePlanNormalContractFee = $('#ChangePlanNormalContractFee').val();
    }

    if ($('#ChangePlanNormalInstallationFee').attr('readonly') == true) {
        changePlanNormalInstallationFee = 0;
    }
    else {
        changePlanNormalInstallationFee = $('#ChangePlanNormalInstallationFee').val();
    }

    if ($('#ChangePlanNormalDepositFee').attr('readonly') == true) {
        changePlanNormalDepositFee = 0;
    }
    else {
        changePlanNormalDepositFee = $('#ChangePlanNormalDepositFee').val();
    }

    if ($('#ChangePlanApproveInstallationFee').attr('readonly') == true) {
        changePlanApproveInstallationFee = 0;
    }
    else {
        changePlanApproveInstallationFee = $('#ChangePlanApproveInstallationFee').val();
    }

    if ($('#ChangePlanOrderContractFee').attr('readonly') == true) {
        changePlanOrderContractFee = 0;
    }
    else {
        changePlanOrderContractFee = $('#ChangePlanOrderContractFee').val();
    }

    if ($('#ChangePlanOrderInstallationFee').attr('readonly') == true) {
        changePlanOrderInstallationFee = 0;
    }
    else {
        changePlanOrderInstallationFee = $('#ChangePlanOrderInstallationFee').val();
    }

    if ($('#ChangePlanOrderDepositFee').attr('readonly') == true) {
        changePlanOrderDepositFee = 0;
    }
    else {
        changePlanOrderDepositFee = $('#ChangePlanOrderDepositFee').val();
    }

    if ($('#ChangePlanCompleteInstallationFee').attr('readonly') == true) {
        changePlanCompleteInstallationFee = 0;
    }
    else {
        changePlanCompleteInstallationFee = $('#ChangePlanCompleteInstallationFee').val();
    }

    if ($('#ChangePlanStartInstallationFee').attr('readonly') == true) {
        changePlanStartInstallationFee = 0;
    }
    else {
        changePlanStartInstallationFee = $('#ChangePlanStartInstallationFee').val();
    }

    if ($("#txtBillingContractFee").attr('readonly') == true) {
        billingContractFeeDetail = 0;
    }
    else {
        billingContractFeeDetail = $("#txtBillingContractFee").val();
    }

//    if ($("#txtBillingCompleteInstallationFee").attr('readonly') == true) {
//        billingCompleteInstallationFee = 0;
//    }
//    else {
//        billingCompleteInstallationFee = $("#txtBillingCompleteInstallationFee").val();
//    }

    billingCompleteInstallationFee = GetBillingInstallationFee();

    if ($("#txtBillingDepositFee").attr('readonly') == true) {
        billingDepositFee = 0;
    }
    else {
        billingDepositFee = $("#txtBillingDepositFee").val();
    }

    if (ContractStatus = '01') {
        if ($("#BillingTargetDetailSection").attr('disabled') == true) {
            billingStartInstallationFee = 0;
            billingContractFeeDetail = 0;
            billingDepositFee = 0;
            billingCompleteInstallationFee = 0;
            billingStartInstallationFee = 0;
            payCompleteInstallationFee = 0;
            payStartServiceInstallationFee = 0;
            payDepositInstallationFee = 0;
        }
        else {
            billingStartInstallationFee = $("#txtBillingStartInstallationFee").val();
            payCompleteInstallationFee = GetBillingInstallationPaymentMethod();
            payStartServiceInstallationFee = $("#StartServiceInstallationFee").val();
            payDepositInstallationFee = $("#DepositInstallationFee").val();
        }
    }
    else {

        if ($("#BillingTargetDetailSection").attr('disabled') == true) {
            billingStartInstallationFee = 0;
            billingContractFeeDetail = 0;
            billingDepositFee = 0;
            billingCompleteInstallationFee = 0;
            billingStartInstallationFee = 0;
            payCompleteInstallationFee = 0;s
            payStartServiceInstallationFee = 0;
            payDepositInstallationFee = 0;
        }
        else {
            billingStartInstallationFee = "0";
            payCompleteInstallationFee = GetBillingInstallationPaymentMethod();
            payStartServiceInstallationFee = "0"
            payDepositInstallationFee = $("#DepositInstallationFee").val();
        }       
    }

    var registerValidationObject = {
        ChangePlanNormalContractFee: changePlanNormalContractFee,
        ChangePlanNormalInstallationFee: changePlanNormalInstallationFee,
        ChangePlanNormalDepositFee: changePlanNormalDepositFee,
        ChangePlanOrderContractFee: changePlanOrderContractFee,
        ChangePlanOrderInstallationFee: changePlanOrderInstallationFee,
        ChangePlanOrderDepositFee: changePlanOrderDepositFee,
        ChangePlanApproveInstallationFee: changePlanApproveInstallationFee,
        ChangePlanCompleteInstallationFee: changePlanCompleteInstallationFee,
        ChangePlanStartInstallationFee: changePlanStartInstallationFee,
        BillingContractFeeDetail: billingContractFeeDetail,
        BillingDepositFee: billingDepositFee,
        BillingCompleteInstallationFee: billingCompleteInstallationFee,
        BillingStartInstallationFee: billingStartInstallationFee,
        NegotiationStaffEmpNo1: $("#NegotiationStaffEmpNo1").val(),
        ApproveNo1: $("#ApproveNo1").val(),
        ApproveNo2: $("#ApproveNo2").val(),
        ApproveNo3: $("#ApproveNo3").val(),
        ApproveNo4: $("#ApproveNo4").val(),
        ApproveNo5: $("#ApproveNo5").val(),
        PayMethodCompleteFee: payCompleteInstallationFee,
        PayMethodStartServiceFee: payStartServiceInstallationFee,
        PayMethodDepositFee: payDepositInstallationFee,
        ContractDurationFlag: $('#ContractDurationFlag').attr('checked'),
        ContractDurationMonth: $("#ContractDurationMonth").val(),
        AutoRenewMonth: $("#AutoRenewMonth").val(),
        EndContractDate: $("EndContractDate").val(),
        ExpectOperationDate: $("#ExpectedOperationDate").val(),
        DivideBillingContractFee: $("#DivideContractFeeBillingFlag").val(),
        // Akat K. add
        BillingTimingType: $("#BillingTimingType").prop("disabled") ? "" : (($("#BillingTimingType").val().length > 0) ? $("#BillingTimingType").val() : null)
    }

    return registerValidationObject;
}

function ISDisableDivideContractFeeBillingFlag() {
    call_ajax_method_json('/Contract/GetIsDisableDivideContract_CTS051', "",
    function (result, controls) {
        if (result == true) {
            $("#DivideContractFeeBillingFlag").attr("disabled", true);
            $("#DivideContractFeeBillingFlag").attr("readonly", true);
        }
        else {
            $("#DivideContractFeeBillingFlag").attr("disabled", false);
            $("#DivideContractFeeBillingFlag").attr("readonly", false);
        }
    });
}

function ISDisableNewRecordSection(status) {
    $("BillingTargetCode").val();
    $("BillingClientCode").val();

    $('#specifyCode :input').attr('disabled', status);
    $('#specifyCode :input').attr('readonly', status);

    $('#copyName :input').attr('readonly', status);
    $('#copyName :input').attr('disabled', status);

    if ($("#rdoTargetCode").attr('checked')) {
        $("#BillingTargetCode").attr("readonly", false);
        $("#BillingClientCode").attr("readonly", true);

        $("#BillingTargetCode").val("");
        $("#BillingClientCode").val("");
    }
}

function ISDisableChangePlanSection(status) {
    $('#ChangePlanSection').attr('disabled', status);
    $('#ChangePlanSection :input').attr('disabled', status);
    $('#ChangePlanSection :input').attr('readonly', status);
}

function ISDisableSpecifySection(status){
    $('#SpecifySection').attr('disabled', status);
    $('#SpecifySection :input').attr('disabled', status);
}

function ISDisableSearchQuotation(status) {
    $('#btnRetrieve').attr('disabled', status);
    $('#btnSearchQuotation').attr('disabled', status);
    $('#Alphabet').attr('readonly', status);
}

function ISDisableChangePlanDetailSection(status) {
    $('#ChangePlanDetailSection').attr('disabled', status);
    $('#ChangePlanDetailSection :input').attr('disabled', status);
    $('#ChangePlanDetailSection :input').attr('readonly', status);

    if ($('#ContractDurationFlag').attr('checked') == true) {
        $("#ContractDurationMonth").attr("readonly", false);
        $("#AutoRenewMonth").attr("readonly", false);
        $("#EndContractDate").EnableDatePicker(true);
    }
    else 
    {
        $("#ContractDurationMonth").attr("readonly", true);
        $("#AutoRenewMonth").attr("readonly", true);
        $("#EndContractDate").EnableDatePicker(false);
    }

    $("#NegotiationStaffEmpName1").attr("readonly", true);
    $("#NegotiationStaffEmpName2").attr("readonly", true);
    InitialEnableDisableChangePlan_CTS051();
}

function ISDisableBillingTargetSection(status) {
    $('#BillingTargetSection').attr('disabled', status);
    $('#BillingTargetSection :input').attr('disabled', status);
    $('#BillingTargetSection :input').attr('readonly', status);
}

function ISDisableBillingTargetDetailSection(status) {
    $('#BillingTargetDetailSection').attr('disabled', status);
    $('#BillingTargetDetailSection :input').attr('disabled', status);
}

function ISDisableRegister(status) {
//    $('#btnCommandRegister').attr('disabled', status);
//    if (status == false) {
//        SetRegisterCommand(true, RegisterClick);
//    }
}

function ISDisableBillingOffice(status) {
    $("#BillingOffice").attr("disabled",status);
}

function ISDisableBTNNewEdit(status) {
    $("#btnNewEdit").attr("disabled", status);
}

function ISDisableBTNClearBillingTarget(status) {
    $("#btnClearBillingTarget").attr("disabled", status);
}

function ISHideChangePlanSectionSection(status) {
    if (status == true) {
        $('#ChangePlanSection').hide();
    }
    else {
        $('#ChangePlanSection').show();
    }
}

function ISHideBillingTargetDetailSection(status) {
    if (status == true) {
        $('#BillingTargetDetailSection').hide();
    }
    else {
        $('#BillingTargetDetailSection').show();
    }
}

function ISHideBillingTargetSection(status) {
    if (status == true) {
        $('#BillingTargetSection').hide();
    }
    else {
        $('#BillingTargetSection').show();
    }
}

function IsNullToZero(val) {
    if (val == "" || val == null) {
        return "0.00";
    }
    else {
        return val;
    }
}

function IsNullToString(val) {
    if (val == "" || val == null) {
        return "-";
    }
    else {
        return val;
    }
}

function ClearControlValueBillingTargetDetailSection() {
    ClearBillingDetail(true);
    $('#rdoContractTarget').attr('checked', true);
}

function QUS012Object() {
    return {
        "QuotationTargetCode": $("#QuotationTargetCode").val(),
        "Alphabet": $("#Alphabet").val(),
        "HideQuotationTarget": true
    };
}

function CMS270Response(result) {
    billingParameter = { BillingClientCode: result.BillingClientCode, Mode: mode }
    call_ajax_method_json('/Contract/ValidateRetrieveBillingClient_CTS051', billingParameter,
            function (result, controls) {
                if (result == null) {
                    call_ajax_method_json('/Contract/RetrieveBillingClientClick_CTS051', billingParameter,
                        function (result, controls) {
                            if (result != null && result.Code != null) {
                                $('#specifyCode :input').attr('disabled', true);
                                $('#copyName :input').attr('disabled', true);

                                ObjCTS051.BillingClientCode = result.BillingClientCodeDetail;
                                ObjCTS051.BillingOffice = result.BillingOffice;
                                ObjCTS051.Sequence = "";

                                BindBillingDetail(result);
                                if (mode != "Update") {
                                    CheckEnableField();
                                    GetBillingTargetInformationDetailGrid_CTS051();
                                }
                            }

                        }, null
                     );
                }
            }, null
    );

    $("#dlgCTS051").CloseDialog();
}

function QUS010Object() {
    return {
        strCallerScreenID: "CTS051"
        , ViewMode: '2'
        , strServiceTypeCode: ObjCTS051.ServiceTypeCode
        , strTargetCodeTypeCode: ObjCTS051.TargetCodeType
        , strQuotationTargetCode: $("#QuotationTargetCode").val()
    };
}

function QUS010Response(result) {
    $("#Alphabet").val(result.Alphabet);
    $("#dlgCTS051").CloseDialog();

    RetrieveClick();
}

function MAS030Object() {
//    var mas030Object = null;
//    call_ajax_method_json('/Contract/GetMAS030Object_CTS051', "",
//            function (result, controls) {
//                mas030Object = result;
//            }, null
//    );

    return mas030Object;
}

function MAS030Response(res) {
    $("#dlgCTS051").CloseDialog();
    call_ajax_method_json('/Contract/UpdateDataFromMAS030Object_CTS051', res,
            function (result, controls) {
                BindBillingDetail(result);
            }, null
    );
}

function EnableRegisterCommand() {
    DisableAllCommand();
    SetRegisterCommand(true, RegisterClick);
    SetResetCommand(true, ResetClick);
}

function EnableConfirmCommand() {
    DisableAllCommand();
    SetConfirmCommand(true, ConfirmClick);
    SetBackCommand(true, BackClick);
}

function DisableAllCommand() {
    SetRegisterCommand(false, null);
    SetResetCommand(false, null);
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
}

function IsDisableBillingGridButton(isdisable) {
    for (var i = 0; i < mygridBilling.getRowsNum(); i++) {
        var row_id = mygridBilling.getRowId(i);

        EnableGridButton(mygridBilling, "btnDetail", row_id, "Detail", !isdisable);
        if (!isdisable && gridRemoveFlagLst[i]) {
            EnableGridButton(mygridBilling, "btnRemove", row_id, "Remove", true);
        } else {
            EnableGridButton(mygridBilling, "btnRemove", row_id, "Remove", false);
        }
    }
}