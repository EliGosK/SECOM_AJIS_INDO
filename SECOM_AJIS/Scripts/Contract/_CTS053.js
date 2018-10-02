/*--- Main ---*/

var mygridBillingDetailCTS053;
var mygridCTS053;
var mygrid;
var pageRow = 20;
var mode;
var eventCopyNameComeFrom;
var btnRemove = "btnRemove";
var validateRegisForm = ["ChangeImplementDate"
, "ChangeContractFee"
, "NegotiationStaffEmpNo1"
, "ApproveNo1"];
var validateRetrieveBillingForm = ["BillingTargetCode", "BillingClientCode"];
var validateBillingForm = ["BillingContractFeeDetail"
, "FullNameEN"
, "AddressEN"
, "FullNameLC"
, "AddressLC"
, "BillingOffice"];
var validateEmailForm = ["EmailAddress"];

var currBillingOCC = "";
var currBillingClientCode = "";
var currBillingTargetCode = "";

$(document).ready(function () {
    $("#EmailAddress").attr("maxlength", c_email_length);
    ObjCTS053.Mode = 'Search';
    InitialControlProperty();
    MaintainScreenItemOnInit();
    ISDisableBillingTargetDetailSection(true);
    ISDisableNewRecordSection(true);
    ISHideBillingTargetDetailSection(true);
    ISDisableDivideContractFeeBillingFlag();
});

function InitialControlProperty() {
    $('#BillingContractFeeDetail').BindNumericBox(12, 2, 0, 9999999999);
    $("#ChangeContractFee").BindNumericBox(14, 2, 0, 9999999999);
    $("#ChangeImplementDate").InitialDate();
    $("#ReturnToOriginalFeeDate").InitialDate();

    $("#btnAdd").click(function () { BtnAddClick(); });
    $("#btnClear").click(function () { BtnClearClick(); });
    $("#btnNew").click(function () { NewClick(); });
    $("#ContractFee").attr("readonly", true);

    $("#DisplayAll").click(
    function () { GetBillingTargetInformation(); });

    $("#NegotiationStaffName1").attr("readonly", true);
    GetEmployeeNameData("#NegotiationStaffEmpNo1", "#NegotiationStaffName1");
//    $('#NegotiationStaffEmpNo1').blur(
//    function () {
//        ObjCTS053.EmpNo = $("#NegotiationStaffEmpNo1").val();
//        call_ajax_method_json('/Contract/GetActiveEmployee_CTS053', ObjCTS053,
//        function (result, controls) {
//            if (result != undefined) {
//                if (result.Message != undefined && $("#NegotiationStaffEmpNo1").val() != "") {
//                    OpenErrorMessageDialog(result.Code, result.Message, function () { }, null);                   
//                    $("#NegotiationStaffName1").val("");
//                }
//                else {
//                    $("#NegotiationStaffName1").val(result.EmpName);
//                }
//            }
//        });
//    });

    $("#NegotiationStaffName2").attr("readonly", true);
    GetEmployeeNameData("#NegotiationStaffEmpNo2", "#NegotiationStaffName2");
//    $('#NegotiationStaffEmpNo2').blur(
//    function () {
//        ObjCTS053.EmpNo = $("#NegotiationStaffEmpNo2").val();
//        call_ajax_method_json('/Contract/GetActiveEmployee_CTS053', ObjCTS053,
//        function (result, controls) {
//            if (result != undefined) {
//                if (result.Message != undefined && $("#NegotiationStaffEmpNo2").val() != "") {
//                    OpenErrorMessageDialog(result.Code, result.Message, function () { }, null);
//                    $("#NegotiationStaffName2").val("");
//                }
//                else {
//                    $("#NegotiationStaffName2").val(result.EmpName);
//                }
//            }
//        });
//    });

    $("#RDOTargetCode").click(function () {
        if ($("#RDOTargetCode").prop('checked')) {
            $("#BillingTargetCode").attr("readonly", false);
            $("#BillingClientCode").attr("readonly", true);

            $("#BillingTargetCode").val("");
            $("#BillingClientCode").val("");
        }
    })

    $("#BillingClientCode").attr("readonly", true);
    $("#RDOClientCode").click(function () {
        if ($("#RDOClientCode").prop('checked')) {
            $("#BillingClientCode").attr("readonly", false);
            $("#BillingTargetCode").attr("readonly", true);

            $("#BillingTargetCode").val("");
            $("#BillingClientCode").val("");
        }
    })

    $("#btnNewEdit").click(function () {
        $("#dlgCTS053").OpenMAS030Dialog("CTS053");
    })

    $("#btnRetrieveBilling").click(function () { BtnRetrieveBillingClick(); });
    $("#btnCopy").click(function () { BtnCopyNameClick(); });
    $("#btnAddUpdate").click(function () { BtnAddUpdateClick(); });
    $("#btnCancelCTS053").click(function () { BtnCancelClick(); })
    $("#btnClearBillingTarget").click(function () { BtnClearBillingDetailClick(); })
    $("#btnSearchBillingClient").click(function () { $("#dlgCTS053").OpenCMS270Dialog(); });
    $("#btnSearchEmail").click(function () { $("#dlgCTS053").OpenCMS060Dialog("CTS053"); });
    $("#ReturnToOriginalFeeDate").EnableDatePicker(true);
    $("#ChangeFee").click( function () { ChangeFeeNoExpirationCheck(); } );

    GetBillingTargetInformation();
    if (ObjCTS053.InstallationStatusCode == "99") {
        $("#btnViewInstalltionDetail").attr("disabled", true);
    }
}

//Event------------------------------------------------

function BtnAddClick() {
    VaridateCtrl(validateEmailForm, null);
    var obj = { EmailAddress: $("#EmailAddress").val() + c_email_suffix }
    if (!IsValidEmail($("#EmailAddress").val())) {
        var obj = {
            module: "Common",
            code: "MSG0087",
            param: 'Notify email'
        };
        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
            OpenWarningDialog(result.Message);
            VaridateCtrl(validateEmailForm, ['EmailAddress']);
        });
    }
    else 
    {
        call_ajax_method_json('/Contract/ValidateEmail_CTS053', obj,
            function (result, controls) {
                VaridateCtrl(validateEmailForm, controls);
                if (result == undefined) {
                    // change pageRow to 0
                    mygrid = $("#gridEmail").LoadDataToGridWithInitial(0, false, false, "/Contract/GetEmail_CTS053", obj, "CTS053_DOEmailData", false);

                    BindOnLoadedEvent(mygrid, function () {
                        if (CheckFirstRowIsEmpty(mygrid, false) == false) {
                            var emailColinx = mygrid.getColIndexById('EmailAddress');
                            var removeColinx = mygrid.getColIndexById('Remove');

                            mygrid.setColumnHidden(mygrid.getColIndexById('EmpNo'), true);
                            for (var i = 0; i < mygrid.getRowsNum(); i++) {
                                var row_id = mygrid.getRowId(i);
                                GenerateRemoveButton(mygrid, btnRemove, row_id, "Remove", true);
                                //mygrid.cells2(i, removeColinx).setValue(btnRemove);
                            }
                        }
                    });

                    //                    mygrid.attachEvent("OnRowSelect", function (id, ind) {
                    //                        if (ind == mygrid.getColIndexById('Remove')) {
                    //                            BtnRemoveMailClick(mygrid.cells2(mygrid.getRowIndex(id), 0).getValue());
                    //                        }
                    //                    });

                    EnableRegisterCommand();
                }
            });
    }
}

function BtnClearBillingDetailClick() {
    ISDisableNewRecordSection(false);
    ClearBillingDetail();
    ClearBillingDetailGrid();
    call_ajax_method_json('/Contract/ClearBillingDetailClick_CTS053', "", function (result, controls) { }, null);
}

function BtnCopyNameClick() {

    var rdoType;
    if ($("#RDOContractTarget").prop('checked')) {
        rdoType = $("#RDOContractTarget").val();
    }

    if ($("#RDOBranchOfContractTarget").prop('checked')) {
        rdoType = $("#RDOBranchOfContractTarget").val();
    }

    if ($("#RDORealCustomer").prop('checked')) {
        rdoType = $("#RDORealCustomer").val();
    }

    if ($("#RDOSite").prop('checked')) {
        rdoType = $("#RDOSite").val();
    }

    copyNameParameter = { EventCopyNameComeFrom: eventCopyNameComeFrom, RdoType: rdoType, Mode: mode }
    call_ajax_method_json('/Contract/CopyNameClick_CTS053', copyNameParameter,
        function (result, controls) {
            GetBillingTargetInformationDetail(result);
            if (mode != "Update") {
                ObjCTS053.Sequence = "";
                eventCopyNameComeFrom = "New";
                //GetBillingTargetInformationDetailGrid_CTS053();
            }
            ISDisableNewRecordSection(true)
        }, null
     );
}

function BtnCancelClick() {
    ClearBillingDetail();
    ClearBillingDetailGrid();    
    ISDisableBillingTargetDetailSection(true);
    ISDisableBillingTargetSection(false);
    ISDisableRegister(false);
    GetBillingTargetInformation();
    ISHideBillingTargetDetailSection(true);
    EnableRegisterCommand();
}

function BtnClearClick() {
    $("#EmailAddress").val("");
}

function BtnRemoveMailClick(mail) {
    var obj = { EmailAddress: mail }
    // Akat K. change pageRow to 0
    mygrid = $("#gridEmail").LoadDataToGridWithInitial(0, false, false, "/Contract/RemoveMailClick_CTS053", obj, "CTS053_DOEmailData", false);

    BindOnLoadedEvent(mygrid, function () {
        var emailColinx = mygrid.getColIndexById('EmailAddress');
        var removeColinx = mygrid.getColIndexById('Remove');
        for (var i = 0; i < mygrid.getRowsNum(); i++) {

            var row_id = mygrid.getRowId(i);
            GenerateRemoveButton(mygrid, btnRemove, row_id, "Remove", true);
            //mygrid.cells2(i, removeColinx).setValue(btnRemove);
        }
    });

    mygrid.attachEvent("OnRowSelect", function (id, ind) {
        if (ind == mygrid.getColIndexById('Remove')) {
            BtnRemoveMailClick(mygrid.cells2(mygrid.getRowIndex(id), 0).getValue());
        }
    });

    mygrid.setColumnHidden(2, true);
}

function BtnRetrieveBillingClick() {
    VaridateCtrl(validateRetrieveBillingForm, null);
    var billingParameter;
    if ($("#RDOTargetCode").prop('checked')) {
        billingParameter = { BillingTargetCode: $("#BillingTargetCode").val(), Mode: mode }
        call_ajax_method_json('/Contract/ValidateRetrieveBillingTarget_CTS053', billingParameter,
            function (result, controls) {
                VaridateCtrl(validateRetrieveBillingForm, controls);
                if (result == undefined && result != false) {
                    call_ajax_method_json('/Contract/RetrieveBillingTargetClick_CTS053', billingParameter,
                        function (result, controls) {
                            VaridateCtrl(validateRetrieveBillingForm, controls);
                            if (result == undefined && result.Code == undefined) {
                                ISDisableNewRecordSection(true);
                                ClearControlValueBillingTargetDetailSection();
                                ObjCTS053.BillingClientCode = result.BillingClientCodeDetail;
                                ObjCTS053.BillingOffice = result.BillingOffice;
                                ObjCTS053.Sequence = ""; //จะได้ไม่ไปดึงข้อมูลใน Billing temp ตัวเก่า

                                GetBillingTargetInformationDetail(result);
                                if (mode != "Update") {
                                    eventCopyNameComeFrom = "New";
                                    GetBillingTargetInformationDetailGrid_CTS053();
                                }

                                EnableRegisterCommand();
                            }
                        }, null
                     );
                }
            }, null
         );
    }
    else {
        billingParameter = { BillingClientCode: $("#BillingClientCode").val(), Mode: mode }
        call_ajax_method_json('/Contract/ValidateRetrieveBillingClient_CTS053', billingParameter,
            function (result, controls) {
                VaridateCtrl(validateRetrieveBillingForm, controls);
                if (result == undefined) {
                    call_ajax_method_json('/Contract/RetrieveBillingClientClick_CTS053', billingParameter,
                        function (result, controls) {
                            VaridateCtrl(validateRetrieveBillingForm, controls);
                            if (result.Code == undefined) {
                                ISDisableNewRecordSection(true);
                                ClearControlValueBillingTargetDetailSection();
                                ObjCTS053.BillingClientCode = result.BillingClientCodeDetail;
                                ObjCTS053.BillingOffice = result.BillingOffice;
                                ObjCTS053.Sequence = ""; //จะได้ไม่ไปดึงข้อมูลใน Billing temp ตัวเก่า

                                GetBillingTargetInformationDetail(result);
                                if (mode != "Update") {
                                    GetBillingTargetInformationDetailGrid_CTS053();
                                }

                                EnableRegisterCommand();
                            }
                        }, null
                     );
                }                
            }, null
         );
    }
}

function NewClick() {
    mode = "New";
    MaintainScreenItemOnInit();
    call_ajax_method_json('/Contract/NewClick_CTS053', "", function (result, controls) { }, null);
    ClearBillingDetail();
    ClearBillingDetailGrid();
    ISDisableBillingTargetDetailSection(false);
    ISDisableNewRecordSection(false);
    ISDisableRegister(true);
    ISDisableBillingTargetSection(true);
    ISHideBillingTargetDetailSection(false);
    GetBillingTargetInformationDetailGrid_CTS053();
    DisableAllCommand();
}

function BtnAddUpdateClick() {
    VaridateCtrl(validateBillingForm, null);
    var validateParameter = {
        BillingContractFeeDetail: $("#BillingContractFeeDetail").val(),
        //PayMethodContractFeeDetail: $("#PayMethodContractFeeDetail").val(),
        BillingOfficeCode: $("#BillingOffice").val(),
        Mode: mode,
        FullNameEN: $("#FullNameEN").val(),
        FullNameLC: $("#FullNameLC").val(),
        AddressEN: $("#AddressEN").val(),
        AddressLC: $("#AddressLC").val()
    };

    call_ajax_method_json('/Contract/ValidateAddUpdateRequireField_CTS053', validateParameter,
        function (result, controls) {
            VaridateCtrl(validateBillingForm, controls);
            if (result == undefined) {
                call_ajax_method_json('/Contract/AddUpdateClick_CTS053', validateParameter, function (result, controls) {
                    if (result == undefined) {
                        GetBillingTargetInformation();
                        ISDisableNewRecordSection(false);
                        //SetRegisterCommand(true, BtnRegisterClick);
                        ClearBillingDetailGrid();
                        ClearBillingDetail();

                        ISDisableBillingTargetSection(false);
                        ISDisableBillingTargetDetailSection(true);
                        ISDisableRegister(false);
                        ISHideBillingTargetDetailSection(true);
                        EnableRegisterCommand();
                    }
                }, null);
            }
        }, null
    );
}

function BtnRegisterClick() {
    VaridateCtrl(validateRegisForm, null);
    call_ajax_method_json('/Contract/RegisterClick_CTS053', GetValidateObject(),
            function (result, controls) {
                VaridateCtrl(validateRegisForm, controls);
                if (result == undefined) {
                    EnableConfirmCommand();

                    $("#ContractChange").SetViewMode(true);
                    $("#ChangePlanSection").SetViewMode(true);
                    ISDisableBillingTargetSection(true);
                    IsDisableNotifyTargetSection(true);
                    IsDisableOtherControl(true);
                }
            });
}

function BtnConfirmClick() {
    var obj = GetValidateObject();

    call_ajax_method_json('/Contract/ConfirmClick_CTS053', obj, function (result, controls) {
        if (result.Code == "MSG0046" || result.Code == "MSG3043") {
            OpenInformationMessageDialog(result.Code, result.Message, function () {
                window.location.href = generate_url("/common/CMS020");
            }, null);            
        }       
    });       
}

function BtnBackClick() {
    /* --- Set Command Button --- */
    EnableRegisterCommand();

    $("#ContractChange").SetViewMode(false);
    $("#ChangePlanSection").SetViewMode(false);
    ISDisableBillingTargetSection(false);
    IsDisableNotifyTargetSection(false);
    IsDisableOtherControl(false);
}

function ResetClick() {  
    var obj = {
        module: "Common",
        code: "MSG0038"
    };
    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenOkCancelDialog(result.Code, result.Message,
        function () {
            call_ajax_method_json('/Contract/ResetData_CTS053', "", function (result, controls) {
                if ((result != null)) {
                    ClearChangeContract();
                    ISHideBillingTargetDetailSection(true);
                    MaintainScreenItemOnInit();
                    ObjCTS053.Mode = "RESET";
                    GetBillingTargetInformation();
                    ObjCTS053.Mode = "";
                    EnableRegisterCommand();

                    ReBindingData(result);
                }
            });   
        },
        null);
    });
}

function ReBindingData(dataObj) {
    ObjCTS053 = { ContractCodeShort: dataObj.ContractCodeShort
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

function ConfirmClick() {
    
}

function ChangeFeeNoExpirationCheck() {
    var isEnable = !$("#ReturnToOriginalFeeDate").prop("readonly");
    $("#ReturnToOriginalFeeDate").EnableDatePicker(isEnable);
    if (isEnable == true) {
        $("#ReturnToOriginalFeeDate").val("");
    }

    if ($("#ChangeFee").prop("checked")) {
        $("#NotifyTargetSection").hide();
        IsDisableNotifyTargetSection(true);
    }
    else {
        $("#NotifyTargetSection").show();
        IsDisableNotifyTargetSection(false);
    }
}

//-----------------------------------------------------

function GetBillingTargetInformation(event) {
    ObjCTS053.DisplayAll = $("#DisplayAll").prop('checked');
    if (event != "Remove") {
        mygridCTS053 = $("#gridBilling").LoadDataToGridWithInitial(pageRow, false, false, "/Contract/GetBillingTargetInformation_CTS053", ObjCTS053, "CTS053_DOBillingTargetGridData", false);
    }
    else {
        mygridCTS053 = $("#gridBilling").LoadDataToGridWithInitial(pageRow, false, false, "/Contract/RemoveChangePlanGridBilling_CTS053", ObjCTS053, "CTS053_DOBillingTargetGridData", false);
    }

    mygridCTS053.setColumnHidden(11, true);
    SpecialGridControl(mygridCTS053, ["Detail", "Remove"]);

    BindOnLoadedEvent(mygridCTS053, function () {
        if (CheckFirstRowIsEmpty(mygridCTS053, false) == false) {
            mygridCTS053.setColumnHidden(mygridCTS053.getColIndexById('Sequence'), true);
            mygridCTS053.setColumnHidden(mygridCTS053.getColIndexById('Show'), true);
            mygridCTS053.setColumnHidden(mygridCTS053.getColIndexById('StatusGrid'), true);

            var detailColinx = mygridCTS053.getColIndexById('Detail');
            var removeColinx = mygridCTS053.getColIndexById('Remove');

            for (var i = 0; i < mygridCTS053.getRowsNum(); i++) {
                //mygridCTS053.cells2(i, detailColinx).setValue("<button id='btnDetail" + i.toString() + "' style='width:65px' >Detail</button>");

                var row_id = mygridCTS053.getRowId(i);
                var enableRemove = (mygridCTS053.cells2(i, mygridCTS053.getColIndexById('BillingOCC')).getValue() != "" && mygridCTS053.cells2(i, mygridCTS053.getColIndexById('BillingOCC')).getValue() != null);

                GenerateDetailButton(mygridCTS053, "btnDetail", row_id, "Detail", true);
                GenerateRemoveButton(mygridCTS053, btnRemove, row_id, "Remove", enableRemove);
                //mygridCTS053.cells2(i, removeColinx).setValue(btnRemove);

                if (mygridCTS053.cells2(i, mygridCTS053.getColIndexById('Show')).getValue() == false) {
                    mygridCTS053.setRowHidden(mygridCTS053.getRowId(i), true)
                }
                else {
                    mygridCTS053.setRowHidden(mygridCTS053.getRowId(i), false)
                }

                BindGridButtonClickEvent("btnDetail", row_id, function (row_id) {
                    ObjCTS053.BillingClientCode = mygridCTS053.cells(row_id, mygridCTS053.getColIndexById('BillingClientCode')).getValue();
                    ObjCTS053.BillingOffice = mygridCTS053.cells(row_id, mygridCTS053.getColIndexById('BillingOffice')).getValue();
                    ObjCTS053.Sequence = mygridCTS053.cells(row_id, mygridCTS053.getColIndexById('Sequence')).getValue();
                    ObjCTS053.fromGrid = true;

                    eventCopyNameComeFrom = "Detail";
                    call_ajax_method_json('/Contract/GetBillingTargetInformationDetail_CTS053', ObjCTS053,
                function (result, controls) {
                    mode = "Update";
                    GetBillingTargetInformationDetail(result);
                    GetBillingTargetInformationDetailGrid_CTS053();
                    ISDisableBillingTargetDetailSection(false);

                    if ((result.BillingOCC != null && result.BillingOCC != "" && result.BillingOCC != "-")) {
                        ISDisableNewRecordSection(true);
                        ISDisableBTNNewEdit(true);
                        ISDisableBTNClearBillingTarget(true);
                        ISDisableBillingOffice(true);
                    }
                    else {
                        ISDisableNewRecordSection(false);
                        if ((result.BillingTargetCode != null && result.BillingTargetCode != "" && result.BillingTargetCode != "-")) {
                            ISDisableBillingOffice(true);
                            ISDisableBTNNewEdit(false);
                            ISDisableBTNClearBillingTarget(false);
                        }
                        else {
                            if ((result.BillingClientCodeDetail != null && result.BillingClientCodeDetail != "" && result.BillingClientCodeDetail != "-")) {
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
                    ISHideBillingTargetDetailSection(false);
                    ISDisableRegister(true);
                });
                });

                BindGridButtonClickEvent(btnRemove, row_id, function (row_id) {
                    ObjCTS053.BillingClientCode = mygridCTS053.cells(row_id, mygridCTS053.getColIndexById('BillingClientCode')).getValue();
                    ObjCTS053.BillingOffice = mygridCTS053.cells(row_id, mygridCTS053.getColIndexById('BillingOffice')).getValue();
                    ObjCTS053.Sequence = mygridCTS053.cells(row_id, mygridCTS053.getColIndexById('Sequence')).getValue();

                    if (mygridCTS053.cells2(mygridCTS053.getRowIndex(row_id), mygridCTS053.getColIndexById('BillingOCC')).getValue() == "" || mygridCTS053.cells2(mygridCTS053.getRowIndex(row_id), mygridCTS053.getColIndexById('BillingOCC')).getValue() == "-" || mygridCTS053.cells2(mygridCTS053.getRowIndex(row_id), mygridCTS053.getColIndexById('BillingOCC')).getValue() == null) {
                        RemoveChangePlanGridBilling_CTS053();
                    }
                });

                //            if (mygridCTS053.cells2(i, mygridCTS053.getColIndexById('BillingOCC')).getValue() != "" && mygridCTS053.cells2(i, mygridCTS053.getColIndexById('BillingOCC')).getValue() != null) {
                //                $("#btnRemove" + i.toString()).attr('disabled', true);
                //            }
                //            else {
                //                $("#btnRemove" + i.toString()).attr('disabled', false);
                //            }
            }
        }
    });

//    mygridCTS053.attachEvent("onRowSelect", function (id, ind) {

//        ObjCTS053.BillingClientCode = mygridCTS053.cells(id, mygridCTS053.getColIndexById('BillingClientCode')).getValue();
//        ObjCTS053.BillingOffice = mygridCTS053.cells(id, mygridCTS053.getColIndexById('BillingOffice')).getValue();
//        ObjCTS053.Sequence = mygridCTS053.cells(id, mygridCTS053.getColIndexById('Sequence')).getValue();

//        if (ind == mygridCTS053.getColIndexById('Detail')) {
//            eventCopyNameComeFrom = "Detail";
//            call_ajax_method_json('/Contract/GetBillingTargetInformationDetail_CTS053', ObjCTS053,
//            function (result, controls) {
//                mode = "Update";
//                GetBillingTargetInformationDetail(result);
//                GetBillingTargetInformationDetailGrid_CTS053();
//                ISDisableBillingTargetDetailSection(false);

//                if ((result.BillingOCC != null && result.BillingOCC != "" && result.BillingOCC != "-")) {
//                    ISDisableNewRecordSection(true);
//                    ISDisableBTNNewEdit(true);
//                    ISDisableBTNClearBillingTarget(true);
//                    ISDisableBillingOffice(true);
//                }
//                else {
//                    ISDisableNewRecordSection(false);
//                    if ((result.BillingTargetCode != null && result.BillingTargetCode != "" && result.BillingTargetCode != "-")) {
//                        ISDisableBillingOffice(true);
//                        ISDisableBTNNewEdit(false);
//                        ISDisableBTNClearBillingTarget(false);
//                    }
//                    else {
//                        if ((result.BillingClientCodeDetail != null && result.BillingClientCodeDetail != "" && result.BillingClientCodeDetail != "-")) {
//                            ISDisableBillingOffice(true);
//                            ISDisableBTNNewEdit(false);
//                            ISDisableBTNClearBillingTarget(false);
//                        }
//                        else {
//                            ISDisableBillingOffice(false);
//                            ISDisableBTNNewEdit(false);
//                            ISDisableBTNClearBillingTarget(false);
//                        }
//                    }
//                }

//                ISDisableRegister(true);
//                ISHideBillingTargetDetailSection(false);
//                ISDisableRegister(true);
//           });
//        }

//        if (ind == mygridCTS053.getColIndexById('Remove')) {
//            if (mygridCTS053.cells2(mygridCTS053.getRowIndex(id), mygridCTS053.getColIndexById('BillingOCC')).getValue() == "" || mygridCTS053.cells2(mygridCTS053.getRowIndex(id), mygridCTS053.getColIndexById('BillingOCC')).getValue() == null) {
//                RemoveChangePlanGridBilling_CTS053();
//            }
//        }
//    });
}

function GetBillingTargetInformationDetail(result) {
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
    if (result.BillingOffice != null && result.BillingOffice != "") {
        $("#BillingOffice").val(result.BillingOffice);
    }

    if (result.BillingTargetCodeDetail != null && result.BillingTargetCodeDetail != "") {
        $("#BillingOffice").attr('disabled', 'disabled');
    } else {
        $("#BillingOffice").removeAttr('disabled');
    }
    
    $("#IDNo").val(result.IDNo);
}

function GetBillingTargetInformationDetailGrid_CTS053() {
    call_ajax_method_json('/Contract/GetBillingTargetInformationDetailGrid_CTS053', ObjCTS053,
        function (result, controls) {
            if ((result != null) && (result.length == 1)) {
                $('#BillingContractFeeDetail').val(result[0].Amount);
            }
        }
    );
}

//function GetBillingTargetInformationDetailGrid_CTS053() {

//    var rowID = "";
//    var paymentMethodParameter = "";
//    var val = "";

//    mygridDetailCTS053 = $("#gridBillingDetail").LoadDataToGridWithInitial(0, false, false, "/Contract/GetBillingTargetInformationDetailGrid_CTS053",
//    ObjCTS053, "CTS053_DOBillingTargetDetailGridData", false);

//    mygridDetailCTS053.setColumnHidden(2, true);
//    BindOnLoadedEvent(mygridDetailCTS053, function () {

//    if (mode == "Update") {
//        val = mygridDetailCTS053.cells2(0, 1).getValue();
//    }

    //mygridDetailCTS053.setColumnHidden(2, true);

//    if (mygridDetailCTS053.cells2(0, 2).getValue() == false) {
//        mygridDetailCTS053.cells2(0, 2).setValue("<select id='PayMethodContractFeeDetail' name='PayMethodContractFeeDetail' select><option value='0' selected='selected'>None</option></select>");
//        rowID = mygridDetailCTS053.getRowId(0);
//        mygridDetailCTS053.setRowHidden(rowID, true)
//    }
//    else {
        //mygridDetailCTS053.cells2(0, 1).setValue(GenerateNumericBox("BillingContractFeeDetail", "", val, true) + " " + C_CURRENCY_UNIT + " " + "<span class='label-remark'>*</span>");

//        var rowID = mygridDetailCTS053.getRowId(0);
//        GenerateNumericBox2(mygridDetailCTS053, "BillingContractFeeDetail", rowID, "Amount", val, 10, 2, 0, 9999999999, 0, true);
//        mygridDetailCTS053.cells2(0, 1).setValue(mygridDetailCTS053.cells2(0, 1).getValue() + " " + C_CURRENCY_UNIT + " " + _lblReq);
//        var ctrlID = GenerateGridControlID("BillingContractFeeDetail", rowID);
//        $('#' + ctrlID).attr('id', 'BillingContractFeeDetail');
//        ctrlID = 'BillingContractFeeDetail';
//        $('#' + ctrlID).css('width', '120px');

//        paymentMethodParameter = { id: "PayMethodContractFeeDetail" }
//        call_ajax_method_json('/Contract/GetComboBoxPaymentMethod_CTS053', paymentMethodParameter,
//                function (result, controls) {
//                    var payMethodCode = mygridDetailCTS053.cells2(0, 2).getValue();
//                    payMethodCode = payMethodCode.replace("^null", "");
//                    mygridDetailCTS053.cells2(0, 2).setValue(result + "<span class='label-remark'>*</span>");
//                    $("#PayMethodContractFeeDetail").val(payMethodCode);
//                });
//    }
//    $("#BillingContractFeeDetail").BindNumericBox(14, 2, 0, 9999999999);
//    $("#BillingContractFeeDetail").css('width', '120px');

    //--------------------------------------------

//    mygridDetailCTS053.attachEvent("onRowSelect", function (id, ind) {
//        var row_num = mygridDetailCTS053.getRowIndex(id);
//        if (mygridDetailCTS053.cell.childNodes[0].tagName == "INPUT") {
//            var txt = mygridDetailCTS053.cell.childNodes[0];
//            if (txt.disabled == false) {
//                txt.focus();
//            }
//        }
//    });
//    });
//}

function RemoveChangePlanGridBilling_CTS053() {
    GetBillingTargetInformation("Remove");
}

function ISDisableDivideContractFeeBillingFlag() {
    call_ajax_method_json('/Contract/GetIsDisableDivideContract_CTS053', "",
    function (result, controls) {
        if (result == true) {
            $("#DivideBillingContractFee").attr("disabled", true);
            $("#DivideBillingContractFee").attr("readonly", true);
        }
        else {
            $("#DivideBillingContractFee").attr("disabled", false);
            $("#DivideBillingContractFee").attr("readonly", false);
        }
    });
} 

function ISDisableNewRecordSection(status) {
    $("BillingTargetCode").val();
    $("BillingClientCode").val();
    $('#specifyCode :input').attr('disabled', status);
    $('#specifyCode :input').attr('readonly', status);
    $('#copyName :input').attr('disabled', status);
    $('#copyName :input').attr('readonly', status);

    if ($("#RDOTargetCode").prop('checked')) {
        $("#BillingTargetCode").attr("readonly", false);
        $("#BillingClientCode").attr("readonly", true);

        $("#BillingTargetCode").val("");
        $("#BillingClientCode").val("");
    }
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
    
}

function ISDisableBillingOffice(status) {
    $("#BillingOffice").attr("disabled", status);
}

function ISDisableBTNNewEdit(status) {
    $("#btnNewEdit").attr("disabled", status);
}

function ISDisableBTNClearBillingTarget(status) {
    $("#btnClearBillingTarget").attr("disabled", status);
}

function ClearControlValueBillingTargetDetailSection() {
    ClearBillingDetail();
    $('#RDOContractTarget').attr('checked', true);
}

function ClearChangeContract() {
    $("#ChangeImplementDate").val("");
    $("#ChangeContractFee").val("");
    $("#NegotiationStaffEmpNo1").val("");
    $("#NegotiationStaffName1").val("");
    $("#NegotiationStaffEmpNo2").val("");
    $("#NegotiationStaffName2").val("");
    $("#ApproveNo1").val("");
    $("#ApproveNo2").val("");
    $("#ApproveNo3").val("");
    $("#ApproveNo4").val("");
    $("#ApproveNo5").val("");
    $("#ApproveNo5").val("");
}

function ClearBillingDetail() {
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
    $("#RDOTargetCode").attr('checked', true)
    $("#RDOContractTarget").attr('checked', true)
    //$("#BillingOffice").val("");
}

function ClearBillingDetailGrid() {
    //$("#BillingContractFeeDetail").val("");
    $("#PayMethodContractFeeDetail").val("");
}

//--------------------------------------------------------------------------------

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
        return "0";
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

function IsValidEmail(email) {
    email = email + '@secom.co.th';
    var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    return emailReg.test(email);
}

function MaintainScreenItemOnInit() {
    EnableRegisterCommand();
}

function GetValidateObject() {

    var ChangeContractFee = $("#ChangeContractFee").val().toString();
    var registerValidationObject = {
        ChangeImplementDate: $("#ChangeImplementDate").val(),
        ReturnToOriginalFeeDate: $("#ReturnToOriginalFeeDate").val(),
        ChangeFeeNoExpiration: $("#ChangeFee").prop("checked"),
        ChangeContractFee: ChangeContractFee,
        NegotiationStaffEmpNo1: $("#NegotiationStaffEmpNo1").val(),
        NegotiationStaffEmpNo2: $("#NegotiationStaffEmpNo2").val(),
        ApproveNo1: $("#ApproveNo1").val(),
        ApproveNo2: $("#ApproveNo2").val(),
        ApproveNo3: $("#ApproveNo3").val(),
        ApproveNo4: $("#ApproveNo4").val(),
        ApproveNo5: $("#ApproveNo5").val(),
        DivideBillingContractFee: $("#DivideBillingContractFee").val(),
        BillingContractFeeDetail: $("#BillingContractFeeDetail").val(),
        //PayMethodContractFeeDetail: $("#PayMethodContractFeeDetail").val(),
        BillingOffice: $("#BillingOffice").val()
    }

    return registerValidationObject;
}

function CMS270Response(result) {
    billingParameter = { BillingClientCode: result.BillingClientCode, Mode: mode }
    call_ajax_method_json('/Contract/ValidateRetrieveBillingClient_CTS053', billingParameter,
            function (result, controls) {
                if (result == undefined) {
                    call_ajax_method_json('/Contract/RetrieveBillingClientClick_CTS053', billingParameter,
                        function (result, controls) {
                                ISDisableNewRecordSection(true);
                                ObjCTS053.BillingClientCode = result.BillingClientCodeDetail;
                                ObjCTS053.BillingOffice = result.BillingOffice;
                                ObjCTS053.Sequence = "";
                                GetBillingTargetInformationDetail(result);
                                if (mode != "Update") {
                                    GetBillingTargetInformationDetailGrid_CTS053();
                            }
                        }, null
                     );
                }
            }, null
    );

    $("#dlgCTS053").CloseDialog();
}

function CMS060Response(result) {

    $("#dlgCTS053").CloseDialog();
    var emailColinx;
    var removeColinx;
    // Akat K. change 'pageRow' to 0
    mygridCTS053 = $("#gridEmail").LoadDataToGridWithInitial(0, false, false, "/Contract/GetEmailList_CTS053", result, "CTS053_DOEmailData", false);
    BindOnLoadedEvent(mygridCTS053, function () {
        emailColinx = mygridCTS053.getColIndexById('EmailAddress');
        removeColinx = mygridCTS053.getColIndexById('Remove');
        if (emailColinx != undefined) {
            for (var i = 0; i < mygridCTS053.getRowsNum(); i++) {

                var row_id = mygridCTS053.getRowId(i);
                GenerateRemoveButton(mygridCTS053, btnRemove, row_id, "Remove", true);
                //mygridCTS053.cells2(i, removeColinx).setValue(btnRemove);
            }
        }
    });

    mygridCTS053.attachEvent("OnRowSelect", function (id, ind) {
        if (ind == mygridCTS053.getColIndexById('Remove')) {
            BtnRemoveMailClick(mygridCTS053.cells2(mygrid.getRowIndex(id), 0).getValue());
        }
    });

    mygridCTS053.setColumnHidden(2, true);
}

function IsDisableOtherControl(status) {
    $("#ChangeImplementDate").EnableDatePicker(status);
    $("#ReturnToOriginalFeeDate").EnableDatePicker(status);
}

function IsDisableNotifyTargetSection(status) {
    $('#NotifyTargetSection').attr('disabled', status);
    $('#NotifyTargetSection :input').attr('disabled', status);
    $('#NotifyTargetSection :input').attr('readonly', status);
}

function MAS030Object() {
    var mas030Object = null;
    call_ajax_method_json('/Contract/GetMAS030Object_CTS053', "",
            function (result, controls) {
                mas030Object = result;
            }, null
    );

    return mas030Object;
}

function MAS030Response(res) {
    $("#dlgCTS053").CloseDialog();
    call_ajax_method_json('/Contract/UpdateDataFromMAS030Object_CTS053', res,
            function (result, controls) {
                GetBillingTargetInformationDetail(result);
            }, null
    );
}

function EnableRegisterCommand() {
    DisableAllCommand();
    SetRegisterCommand(true, BtnRegisterClick);
    SetResetCommand(true, ResetClick);
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