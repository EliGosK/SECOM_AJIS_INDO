/// <reference path = "../../Scripts/jquery-1.5.1-vsdoc.js" />
/// <reference path = "../../Scripts/Base/GridControl.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxcommon.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgrid.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgridcell.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js" />
/// <reference path = "../../Scripts/Base/object/ajax_method.js" />
/// <reference path = "../../Scripts/Base/object/master_event.js" />

/// <reference path = "../Base/object/ajax_method.js" />

var mygrid = null;
var modeOfComfirmCommand;
var modeOfDlgBox;
var isSelected = false;
var selectedRowId;

$(document).ready(function () {

    $("#Result_Detail").hide();

    $("#NameEN").InitialAutoComplete("/Master/GetNameEN");
    $("#NameLC").InitialAutoComplete("/Master/GetNameLC");
    $("#BranchNameEN").InitialAutoComplete("/Master/GetBranchNameEN");
    $("#BranchNameLC").InitialAutoComplete("/Master/GetBranchNameLC");
    $("#AddressEN").InitialAutoComplete("/Master/GetAddressEN");
    $("#AddressLC").InitialAutoComplete("/Master/GetAddressLC");
    InitialNumericInputTextBox(["IDNo", "PhoneNo"]);
    InitialNumericInputTextBox(["BranchNo"], false);

    $("#btnRetrieve").click(function () {
        Retrieve();
    });

    $("#btnSearchBillingClient").click(function () {
        $("#dlgBox").OpenCMS270Dialog();
    });

    $("#DeleteFlag").click(function () {
        setDisableControlForDeleteFlag($("#DeleteFlag").prop("checked"));
    });

    $("#NameEN").blur(function () {
        ChangeCustomerFullEN();
    });

    $("#NameLC").blur(function () {
        ChangeCustomerFullLC();
    });

    $("#CompanyTypeCode").change(function () {
        ChangeCustomerFullEN();
        ChangeCustomerFullLC();
    });

    $("#CustTypeCode").change(function () {
        if ($("#CustTypeCode").val() == MAS020_Constant.C_CUST_TYPE_JURISTIC) {
            $("#CompanyTypeCode").attr("disabled", false);
        }
        else {
            $("#CompanyTypeCode").attr("disabled", true);
            $("#CompanyTypeCode").val("");
        }

        ChangeCustomerFullEN();
        ChangeCustomerFullLC();
    });


    $("#FullNameEN").attr("readonly", true);
    $("#FullNameLC").attr("readonly", true);

    // tt
    $("#AddressEN").SetMaxLengthTextArea(1600, 4);
    $("#AddressLC").SetMaxLengthTextArea(1600, 4);

    $("#rdoHeadOffice").change(function () { ChangeBranchType(); }); //Add by Jutarat A. on 16122013
    $("#rdoBranch").change(function () { ChangeBranchType(); }); //Add by Jutarat A. on 16122013
});

function Retrieve() {

    // disable button [Retrieve] , [Search BillingClient]
    $("#btnRetrieve").attr("disabled", true);
    $("#btnSearchBillingClient").attr("disabled", true);

    var param = { "BillingClientCodeSearch": $("#BillingClientCodeSearch").val() };

    ajax_method.CallScreenController(
        '/Master/MAS020_Retrieve',
        param,
        function (result, controls) {

            if (controls != undefined) {

                // enable button [Retrieve] , [Search BillingClient]
                $("#btnRetrieve").attr("disabled", false);
                $("#btnSearchBillingClient").attr("disabled", false);

                VaridateCtrl(["BillingClientCodeSearch"], controls);
                $("#BillingClientCodeSearch").focus();

                return;
            }
            else if (result != undefined) {

                setDisableSearchSection(true);

                //Add by Jutarat A. on 16122013
                $("#rdoBranch").attr("checked", true);

                $("#BranchNo").val("");
                $("#BranchNameEN").val("");
                $("#BranchNameLC").val("");

                if (permission.EDIT) {
                    $("#BranchNo").attr("readonly", false);
                    $("#BranchNameEN").attr("readonly", false);
                    $("#BranchNameLC").attr("readonly", false);
                }
                //End Add

                fillBillingClientData(result);
                ChangeCustomerFullEN();
                ChangeCustomerFullLC();

                $("#DeleteFlag").attr("disabled", !permission.DEL);
                setDisableControlForDeleteFlag(!permission.EDIT);

                //Add by Jutarat A. on 17122013
                if ($("#BranchNo").val() == "00000") {
                    $("#rdoHeadOffice").attr("checked", true);
                    ChangeBranchType();
                }
                //End Add

                $("#Result_Detail").show();

                if (permission.EDIT || permission.DEL) {
                    SetConfirmCommand(true, confirmCommand);

                }
                SetClearCommand(true, cancelCommand);
            }
            else {

                // enable button [Retrieve] , [Search customer]
                $("#btnRetrieve").attr("disabled", false);
                $("#btnSearchBillingClient").attr("disabled", false);

            }
        }
    );
}

function fillBillingClientData(result) {
    if (result != undefined) {
        $("#DeleteFlag").attr("checked", result.DeleteFlag);
        $("#BillingClientCodeShort").val(result.BillingClientCodeShort);
        $("#CustTypeCode").val(result.CustTypeCode);
        $("#CompanyTypeCode").val(result.CompanyTypeCode);
        $("#IDNo").val(result.IDNo);
        $("#NameEN").val(result.NameEN);
        $("#NameLC").val(result.NameLC);
        $("#BranchNo").val(result.BranchNo);
        $("#BranchNameEN").val(result.BranchNameEN);
        $("#BranchNameLC").val(result.BranchNameLC);
        $("#AddressEN").val(result.AddressEN);
        $("#AddressLC").val(result.AddressLC);
        $("#RegionCode").val(result.RegionCode);
        $("#BusinessTypeCode").val(result.BusinessTypeCode);
        $("#PhoneNo").val(result.PhoneNo);
    }
}

function ChangeCustomerFullEN() {
    var param = { "NameEN": $("#NameEN").val(), "CompanyTypeCode": $("#CompanyTypeCode").val() };
    ajax_method.CallScreenController("/Master/MAS020_CustomerNameENChange"
        , param
        , function (result) {
            $("#FullNameEN").val(result);
        }
    );
}

function ChangeCustomerFullLC() {
    var param = { "NameLC": $("#NameLC").val(), "CompanyTypeCode": $("#CompanyTypeCode").val() };
    ajax_method.CallScreenController("/Master/MAS020_CustomerNameLCChange"
        , param
        , function (result) {
            $("#FullNameLC").val(result);
        }
    );
}

//Add by Jutarat A. on 16122013
function ChangeBranchType() {
    if ($("#rdoHeadOffice").prop("checked")) {
        $("#BranchNo").val("00000");
        $("#BranchNameEN").val(strHeadOfficeEN);
        $("#BranchNameLC").val(null);

        $("#BranchNo").attr("readonly", true);
        $("#BranchNameEN").attr("readonly", true);
        $("#BranchNameLC").attr("readonly", true);
    }
    else {
        $("#BranchNo").val("");
        $("#BranchNameEN").val("");
        $("#BranchNameLC").val("");

        if (permission.EDIT) {
            $("#BranchNo").attr("readonly", false);
            $("#BranchNameEN").attr("readonly", false);
            $("#BranchNameLC").attr("readonly", false);
        }
    }
}
//End Add

function confirmCommand() {
    setDisableControlForDeleteFlag(false);
    if (!$("#DeleteFlag").prop("checked")) { modeOfComfirmCommand = "edit"; } else { modeOfComfirmCommand = "delete"; }
    switch (modeOfComfirmCommand) {
        case "edit":
            confirmEdit();
            break;
        case "delete":
            confirmDelete();
            break;
    }
    setDisableControlForDeleteFlag($("#DeleteFlag").prop("checked"));
}

function cancelCommand() {
    var obj = {
        module: "Common",
        code: "MSG0044"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message, function () {
            SetConfirmCommand(false, confirmCommand);
            SetClearCommand(false, cancelCommand);
            $("#Result_Detail").hide();
            $("#Result_Detail").clearForm();
            setDisableSearchSection(false);
            $("#BillingClientCodeSearch").val("");
        });

    });

//        var param = { "module": "Common", "code": "MSG0044", param: msgCancel };
//        call_ajax_method_json("/Shared/GetMessage"
//            , param
//            , function (data, ctrl) {
//                OpenYesNoMessageDialog(data.Code, data.Message
//                    , function () {
//                        SetConfirmCommand(false, confirmCommand);
//                        SetClearCommand(false, cancelCommand);
//                        $("#Result_Detail").hide();
//                        $("#Result_Detail").clearForm();
//                        setDisableSearchSection(false);
//                        $("#BillingClientCodeSearch").val("");
//                    }
//                    , function () {

//                    }
//                );
//            }
//        );
}

function confirmEdit() {
    command_control.CommandControlMode(false);
    var param = CreateObjectData($("#MAS020_ResultDetail").serialize(), true);
    ajax_method.CallScreenController('/Master/MAS020_Update'
        , param
        , function (result, controls) {
            command_control.CommandControlMode(true);
            if (controls != undefined) {
                VaridateCtrl(["CustTypeCode"], controls);
                VaridateCtrl(["NameEN"], controls);
                VaridateCtrl(["NameLC"], controls);
                VaridateCtrl(["AddressEN"], controls);
                VaridateCtrl(["AddressLC"], controls);
                VaridateCtrl(["CompanyTypeCode"], controls);

                VaridateCtrl(["BranchNo"], controls); //Add by Non A. on 2014-11-13
                VaridateCtrl(["BranchNameEN"], controls); //Add by Jutarat A. on 12122013
                VaridateCtrl(["BranchNameLC"], controls); //Add by Jutarat A. on 12122013
                VaridateCtrl(["IDNo"], controls); //Add by Jutarat A. on 25122013
            }
            else if (result != undefined) {
                SetConfirmCommand(false, null);
                SetClearCommand(false, null);
                var param = { "module": "Common", "code": "MSG0046" };
                call_ajax_method_json("/Shared/GetMessage"
                    , param
                    , function (data) {
                        OpenInformationMessageDialog(data.Code, data.Message
                            , function () {

                            }
                        );
                    }
                );
                $("#Result_Detail").hide();
                setDisableSearchSection(false);
                $("#BillingClientCodeSearch").val("");
            }

        }
    );
}

function confirmDelete() {
    command_control.CommandControlMode(false);
    // Get Message
    var obj = {
        module: "Master",
        code: "MSG1027"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        command_control.CommandControlMode(true);
        OpenYesNoMessageDialog(result.Code, result.Message, function () {
            var param = CreateObjectData($("#MAS020_ResultDetail").serialize() + "&CustTypeCode=" + $("#CustTypeCode").val() + "&CompanyTypeCode=" + $("#CompanyTypeCode").val() + "&RegionCode=" + $("#RegionCode").val() + "&BusinessTypeCode=" + $("#BusinessTypeCode").val(), true);
            ajax_method.CallScreenController('/Master/MAS020_Update'
                , param
                , function (result, controls) {
                    if (controls != undefined) {
                        var param = { "module": "Master", "code": "MSG1028", param: [$("#BillingClientCodeSearch").val()] };
                        call_ajax_method_json("/Shared/GetMessage"
                            , param
                            , function (data) {
                                OpenInformationMessageDialog(data.Code, data.Message
                                    , function () {

                                    }
                                );
                            }
                        );
                    }
                    if (result != undefined) {
                        SetConfirmCommand(false, null);
                        SetClearCommand(false, null);
                        var param = { "module": "Master", "code": "MSG1029", param: [$("#BillingClientCodeSearch").val()] };
                        call_ajax_method_json("/Shared/GetMessage"
                            , param
                            , function (data) {
                                OpenInformationMessageDialog(data.Code, data.Message
                                    , function () {

                                    }
                                );
                            }
                        );
                    }
                    $("#Result_Detail").hide();
                    setDisableSearchSection(false);
                    $("#MAS020_SearchCriteria").clearForm();
                    $("#BillingClientCodeSearch").val("");
                }
            );
        });

    });

    
}

function setDisableSearchSection(isDisable) {
    $("#MAS020_SearchCriteria input").attr("readonly", isDisable);
    $("#MAS020_SearchCriteria button").attr("disabled", isDisable);
    $("#MAS020_SearchCriteria select").attr("disabled", isDisable);
////    if (!isDisable) {
////        $("#btnRetrieve").attr("disabled", !permission.View);
////    }
}

function setDisableControlForDeleteFlag(isDisable) {
    $("#MAS020_ResultDetail input").attr("readonly", isDisable);
    $("#MAS020_ResultDetail select").attr("disabled", isDisable);
    $("#BillingClientCodeShort").attr("readonly", true);

    $("#FullNameEN").attr("readonly", true);
    $("#FullNameLC").attr("readonly", true);

    //Add by Jutarat A. on 16122013
    if ($("#rdoHeadOffice").prop("checked")) {
        $("#BranchNo").attr("readonly", true);
        $("#BranchNameEN").attr("readonly", true);
        $("#BranchNameLC").attr("readonly", true);
    }
    else {
        if (!isDisable) {
            $("#BranchNo").attr("readonly", false);
            $("#BranchNameEN").attr("readonly", false);
            $("#BranchNameLC").attr("readonly", false);
        }
    }
    //End Add

    //Add by Jutarat A. on 03012014
    if ($("#CustTypeCode").val() == MAS020_Constant.C_CUST_TYPE_JURISTIC) {
        $("#CompanyTypeCode").attr("disabled", isDisable);
    }
    else {
        $("#CompanyTypeCode").attr("disabled", true);
        $("#CompanyTypeCode").val("");
    }
    //End Add
}

function CMS270Response(result_BillingClient) {
    $("#BillingClientCodeSearch").val(result_BillingClient.BillingClientCode);
    $("#dlgBox").CloseDialog();
    Retrieve();
}