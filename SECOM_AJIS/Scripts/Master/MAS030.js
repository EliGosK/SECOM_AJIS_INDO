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

var mas030_result;

$(document).ready(function () {

    $("#mas030_CustNameEN").InitialAutoComplete("/Master/GetNameEN"); //$("#mas030_CustNameEN").keypress(NameEn_keypress);  
    $("#mas030_CustNameLC").InitialAutoComplete("/Master/GetNameLC"); //$("#mas030_CustNameLC").keypress(NameLc_keypress);
    $("#mas030_BranchNameEN").InitialAutoComplete("/Master/GetBranchNameEN"); //$("#mas030_BranchNameEN").keypress(BranchNameEn_keypress);
    $("#mas030_BranchNameLC").InitialAutoComplete("/Master/GetBranchNameLC"); //$("#mas030_BranchNameLC").keypress(BranchNameLc_keypress);
    $("#mas030_AddressEN").InitialAutoComplete("/Master/GetAddressEN"); //$("#mas030_AddressEN").keypress(AddressEn_keypress);
    $("#mas030_AddressLC").InitialAutoComplete("/Master/GetAddressLC"); //$("#mas030_AddressLC").keypress(AddressLc_keypress);

    //Set maxlenght for TextArea control
    $("#mas030_AddressEN").SetMaxLengthTextArea(1600);
    $("#mas030_AddressLC").SetMaxLengthTextArea(1600);

    /*==== event on change customerType combo --> enable/disable companyType combo ====*/
    $("#mas030_CustomerType").RelateControlEvent(doCustomerChange_related);

    /*==== event on change customerType combo --> enable/disable companyType combo ====*/
    $("#mas030_CompanyType").RelateControlEvent(doCompanyChange_related);

    /*==== event on lost focus at name --> fill fullname ====*/
    $("#mas030_CustNameEN").blur(function () {
        doCustNameENChange();
    });
    $("#mas030_CustNameLC").blur(function () {
        doCustNameLCChange();
    });

    $("#mas030_CompanyType").attr("disabled", true);
    doCustomerChange();

    if ($("#mas030_ValidateBillingClient").val().toUpperCase() == "TRUE") {
        ValidateBillingClient();
    }


    // For intial numberic texbox for Zipcode , Taxno ,Id no ,...
    InitialNumericInputTextBox(["mas030_IDNo", "mas030_TelephoneNo"]);
    InitialNumericInputTextBox(["mas030_BranchNo"], false);
    ValidateObjectData();

    //Add by Jutarat A. on 16122013
    if ($("#mas030_BranchNo").val() == "00000" || $("#mas030_BranchNo").val() == "") {
        $("#rdoHeadOffice").attr("checked", true);
        ChangeBranchType();
    }
    else {
        $("#rdoBranch").attr("checked", true);

        $("#mas030_BranchNo").attr("readonly", false);
        $("#mas030_BranchNameEN").attr("readonly", false);
        $("#mas030_BranchNameLC").attr("readonly", false);
    }

    $("#rdoHeadOffice").change(function () { ChangeBranchType(); });
    $("#rdoBranch").change(function () { ChangeBranchType(); }); //Add by Jutarat A. on 16122013
    //End Add
});


function doCustomerChange() {

    var customerTypeCode = $("#mas030_CustomerType").val();
    var custTypeJuristic = $("#mas030_CustTypeJuristic").val();

    if (customerTypeCode == custTypeJuristic) {
        $("#mas030_CompanyType").removeAttr("disabled");
    } else {
        $("#mas030_CompanyType").attr("disabled", true);
        $("#mas030_CompanyType").val(""); //Reset CompanyType
        doCompanyChange(); //Generate new full name
    }
}

// doCustomerChange_related


function doCustomerChange_related(istab, isblur) {

    doCustomerChange();

    if (istab && isblur) {

        var customerTypeCode = $("#mas030_CustomerType").val();
        var custTypeJuristic = $("#mas030_CustTypeJuristic").val();

        if (customerTypeCode == custTypeJuristic) {
            $("#mas030_CompanyType").focus();
        } else {
            $("#mas030_IDNo").focus();
        }
    }

}

function doCompanyChange() {

    var param = {
        "NameEN": $("#mas030_CustNameEN").val(),
        "NameLC": $("#mas030_CustNameLC").val(),
        "CompanyTypeCode": $("#mas030_CompanyType").val()
    };

    ajax_method.CallScreenController(
        '/Master/MAS030_CompanyChange/',
        param,
        function (result, controls) {
            if (result != undefined) {
                $("#mas030_CustFullNameEN").val(result.FullNameEN);
                $("#mas030_CustFullNameLC").val(result.FullNameLC);
            }
        }
    );
}

// doCompanyChange_related

function doCompanyChange_related(istab, isblur) {

    doCompanyChange();

}

function doCustNameENChange() {
    var param = {
        "NameEN": $("#mas030_CustNameEN").val(),
        "CompanyTypeCode": $("#mas030_CompanyType").val()
    };
    ajax_method.CallScreenController(
        '/Master/MAS030_CustNameENChange/',
        param,
        function (result, controls) {
            if (result != undefined) {
                $("#mas030_CustFullNameEN").val(result.FullNameEN);
            }
        }
    );
}

function doCustNameLCChange() {
    var param = {
        "NameLC": $("#mas030_CustNameLC").val(),
        "CompanyTypeCode": $("#mas030_CompanyType").val()
    };
    ajax_method.CallScreenController(
        '/Master/MAS030_CustNameLCChange/',
        param,
        function (result, controls) {
            if (result != undefined) {
                $("#mas030_CustFullNameLC").val(result.FullNameLC);
            }
        }
    );
}

//Add by Jutarat A. on 16122013
function ChangeBranchType() {
    if ($("#rdoHeadOffice").prop("checked")) {
        $("#mas030_BranchNo").val("00000");
        $("#mas030_BranchNameEN").val($("#hidHeadOfficeEN").val());
        $("#mas030_BranchNameLC").val($(null).val());

        $("#mas030_BranchNo").attr("readonly", true);
        $("#mas030_BranchNameEN").attr("readonly", true);
        $("#mas030_BranchNameLC").attr("readonly", true);
    }
    else {
        $("#mas030_BranchNo").val("");
        $("#mas030_BranchNameEN").val("");
        $("#mas030_BranchNameLC").val("");

        $("#mas030_BranchNo").attr("readonly", false);
        $("#mas030_BranchNameEN").attr("readonly", false);
        $("#mas030_BranchNameLC").attr("readonly", false);
    }
}
//End Add

function MAS030Initial() {
    ChangeDialogButtonText(["OK"], [$('#mas030_btnOK').val()]);
    ChangeDialogButtonText(["Cancel"], [$('#mas030_btnCancel').val()]);
    BindDialogButtonClick($("#mas030_btnOK").val(), function () {
        doOKAction();
    });
}

function getInputData() {
    //Check combobox Customer Type
    var custCodeType = $("#mas030_CustomerType").val();
    var custTypeName;
    if (custCodeType == "")
        custTypeName = "";
    else
        custTypeName = $("#mas030_CustomerType option:selected").text();

    //Check combobox Company Type
    var companyTypeCode = $("#mas030_CompanyType").val();
    var companyTypeName;
    if ($("#mas030_CompanyType").prop("disabled") || companyTypeCode == "") {
        companyTypeCode = "";
        companyTypeName = "";
    } else {
        custTypeName = $("#mas030_CustomerType option:selected").text();
    }

    //Check combobox Nationality Type
    var regionCode = $("#mas030_Nationality").val();
    var nationality;
    if (regionCode == "") {
        nationality = "";
    } else {
        nationality = $("#mas030_Nationality option:selected").text();
    }

    //Check combobox Business Type
    var businessTypeCode = $("#mas030_BusinessType").val();
    var businessTypeName;
    if (businessTypeCode == "") {
        businessTypeName = "";
    } else {
        businessTypeName = $("#mas030_BusinessType option:selected").text();
    }

    //    alert("custCodeType = " + custCodeType + ", custTypeName = " + custTypeName);
    //    alert("companyTypeCode = " + companyTypeCode + ", companyTypeName = " + companyTypeName);
    //    alert("regionCode = " + regionCode + ", nationality = " + nationality);
    //    alert("businessTypeCode = " + businessTypeCode + ", businessTypeName = " + businessTypeName);

    mas030_result = {
        BillingClientCode: $("#mas030_BillingClientCode").val(),
        CustTypeCode: custCodeType,
        CustTypeName: custTypeName,
        CompanyTypeCode: companyTypeCode,
        CompanyTypeName: companyTypeName,
        IDNo: $("#mas030_IDNo").val(),
        NameEN: $("#mas030_CustNameEN").val(),
        FullNameEN: $("#mas030_CustFullNameEN").val(),
        NameLC: $("#mas030_CustNameLC").val(),
        FullNameLC: $("#mas030_CustFullNameLC").val(),
        BranchType: ($("#rdoHeadOffice").prop("checked") ? "1" : ($("#rdoBranch").prop("checked") ? "2" : null)),
        BranchNo: $("#mas030_BranchNo").val(),
        BranchNameEN: $("#mas030_BranchNameEN").val(),
        BranchNameLC: $("#mas030_BranchNameLC").val(),
        AddressEN: $("#mas030_AddressEN").val(),
        AddressLC: $("#mas030_AddressLC").val(),
        RegionCode: regionCode,
        Nationality: nationality,
        BusinessTypeCode: businessTypeCode,
        BusinessTypeName: businessTypeName,
        PhoneNo: $("#mas030_TelephoneNo").val()
    }

    return mas030_result;
}

function ValidateBillingClient() {
    var mas030_result = getInputData();

    ajax_method.CallScreenController(
        '/Master/MAS030_CheckReqField/',
        mas030_result,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["mas030_CustomerType", "mas030_CustNameEN", "mas030_CustNameLC", "mas030_AddressEN", "mas030_AddressLC"], controls);
                return;
            }
        }
    );
}

function doOKAction() {

    var mas030_result = getInputData();

    ajax_method.CallScreenController(
        '/Master/MAS030_CheckReqField/',
        mas030_result,
        function (result, controls) {
            if (controls != undefined) {
                //VaridateCtrl(["mas030_CustomerType", "mas030_CustNameEN", "mas030_CustNameLC", "mas030_AddressEN", "mas030_AddressLC"], controls);
                VaridateCtrl(["mas030_CustomerType", "mas030_CustNameEN", "mas030_CustNameLC", "mas030_AddressEN", "mas030_AddressLC", "mas030_BranchNo", "mas030_BranchNameEN", "mas030_BranchNameLC", "mas030_IDNo"], controls); //Modify by Jutarat A. on 12122013
                
                //$("#mas030_btnOK").focus();
                return;

            } else if (result != undefined) {

                //clear BillingClientCode if data in controls <> input data
                if (result == false) {
                    mas030_result.BillingClientCode = "";
                }

                if (typeof (MAS030Response) == "function")
                    MAS030Response(mas030_result);
            }
        }
    );
}

//function NameEn_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#mas030_CustNameEN",
//                                cond,
//                                "/Master/GetNameEN",
//                                { "cond": cond },
//                                "doBillingNameEN",
//                                "NameEN",
//                                "NameEN");
//    }
//}
//function NameLc_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#mas030_CustNameLC",
//                                cond,
//                                "/Master/GetNameLC",
//                                { "cond": cond },
//                                "doGetBillingNameLC",
//                                "NameLC",
//                                "NameLC");
//    }
//}
//function BranchNameEn_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#mas030_BranchNameEN",
//                                cond,
//                                "/Master/GetBranchNameEN",
//                                { "cond": cond },
//                                "doBillingBranchNameEN",
//                                "BranchNameEN",
//                                "BranchNameEN");
//    }
//}

//function BranchNameLc_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#mas030_BranchNameLC",
//                                cond,
//                                "/Master/GetBranchNameLC",
//                                { "cond": cond },
//                                "doBillingBranchNameLC",
//                                "BranchNameLC",
//                                "BranchNameLC");
//    }
//}

//function AddressEn_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#mas030_AddressEN",
//                                cond,
//                                "/Master/GetAddressEN",
//                                { "cond": cond },
//                                "doBillingAddressNameEN",
//                                "AddressEN",
//                                "AddressEN");
//    }
//}

//function AddressLc_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#mas030_AddressLC",
//                                cond,
//                                "/Master/GetAddressLC",
//                                { "cond": cond },
//                                "doBillingAddressNameLC",
//                                "AddressLC",
//                                "AddressLC");
//    }
//}

function ValidateObjectData() {
    call_ajax_method_json("/Master/MAS030_ValidateData", "",
    function (result, controls) {
        VaridateCtrl([
            "mas030_CustomerType", "mas030_CustNameEN", "mas030_CustNameLC", "mas030_AddressEN", "mas030_AddressLC"], controls);
    });
}
