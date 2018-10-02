
/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>

/* --- Variables --- */
var mygrid;
var pageRow;

/*--- Main ---*/
$(document).ready(function () {

    pageRow = CTS040Data.PageRow;

    /* --- Initial grid --- */
    if ($("#grid_result").length > 0) {
        mygrid = $("#grid_result").InitialGrid(pageRow, true, "/Contract/InitialGrid_CTS040");

        /*=========== Set hidden column =============*/
        mygrid.setColumnHidden(mygrid.getColIndexById("ServiceTypeCode"), true);
    }

    /*==== Attach Datetime Picker ====*/
    InitialDateFromToControl("#RegistrationDateFrom", "#RegistrationDateTo");
    InitialDateFromToControl("#ApproveDateFrom", "#ApproveDateTo");

    /*==== event Customer Name keypress ====*/
    $("#ContractTargetName").InitialAutoComplete("/Master/GetCustName"); //$("#ContractTargetName").keypress(ContractTargetName_keypress);

    /*==== event Employee Name keypress ====*/
    $("#Salesman1Name").InitialAutoComplete("/Master/MAS070_GetEmployeeName"); //$("#Salesman1Name").keypress(Salesman1Name_keypress);

    /*==== event Site Name keypress ====*/
    $("#SiteName").InitialAutoComplete("/Master/GetSiteName"); //$("#SiteName").keypress(SiteName_keypress);

    /*==== event btnSearch click ====*/
    $("#btnSearch").click(function () {
        validateSearchCriteria();
    });

    /*==== event btnClear click ====*/
    $("#btnClear").click(function () {
        clearSearchCriteria();
    });

    initial();

    /* ===== binding event when finish load data ===== */
    BindOnLoadedEvent(mygrid, function (gen_ctrl) {
        var contractCodeColInx = mygrid.getColIndexById('ContractCodeShow');
        var serviceTypeCodeColInx = mygrid.getColIndexById('ServiceTypeCode');

        for (var i = 0; i < mygrid.getRowsNum(); i++) {
            if (gen_ctrl == true) {
                var contractCode = mygrid.cells2(i, contractCodeColInx).getValue();
                var serviceTypeCode = mygrid.cells2(i, serviceTypeCodeColInx).getValue();
                if (contractCode != '-') {
                    if (contractCode.indexOf("<A") >= 0
                    || contractCode.indexOf("<a") >= 0) {
                        contractCode = contractCode.substring(contractCode.indexOf(">") + 1);
                        contractCode = contractCode.substring(0, contractCode.indexOf("<"));
                    }

                    var tagA = "<a href='#'>" + contractCode + "<input type='hidden' name='contractCode' value='" + contractCode + "'/><input type='hidden' name='serviceTypeCode' value='" + serviceTypeCode + "'/></a>";
                    mygrid.cells2(i, contractCodeColInx).setValue(tagA);
                }
            }
        }

        $("#grid_result a").each(function () {
            $(this).unbind("click");
            $(this).click(function () {
                var obj =
                {
                    strContractCode: $(this).children("input:hidden[name=contractCode]").val(),
                    strServiceTypeCode: $(this).children("input:hidden[name=serviceTypeCode]").val()
                };
                ajax_method.CallScreenControllerWithAuthority("/Common/CMS190", obj, true);
            });
        });
    });
});

/*---- Event ------*/
//function ContractTargetName_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#ContractTargetName",
//                                cond,
//                                "/Master/GetCustName", // Note: in Master/Controllers/CustomerData.cs/GetCustName()
//                                {"cond": cond },
//                                "dtCustName",
//                                "CustName",
//                                "CustName");
//    }
//}

//function Salesman1Name_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#Salesman1Name",
//                                cond,
//                                "/Master/MAS070_GetEmployeeName",
//                                { "cond": cond },
//                                "dtEmployeeName",
//                                "EmpName",
//                                "EmpName");
//    }
//}

//function SiteName_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#SiteName",
//                                cond,
//                                "/Master/GetSiteName", // Note : in Master/Controllers/SiteData.cs/GetSiteName()
//                                {"cond": cond },
//                                "dtSiteName",
//                                "SiteName",
//                                "SiteName");
//    }
//}

function clearSearchCriteria() {
    //Clear data in “Search Criteria” section
    $("#QuotationCode").val("");
    $("#Alphabet").val("");
    //    $("#RegistrationDateFrom").val("");
    //    $("#RegistrationDateTo").val("");
    $("#Salesman1Code").val("");
    $("#Salesman1Name").val("");
    $("#ContractTargetName").val("");
    $("#SiteName").val("");
    $("#ContractOfficeCode").val("");
    $("#OperationOfficeCode").val("");
    $("#ApproveContractStatus").val("");
    //    $("#ApproveDateFrom").val("");
    //    $("#ApproveDateTo").val("");

    ClearDateFromToControl("#RegistrationDateFrom", "#RegistrationDateTo");
    ClearDateFromToControl("#ApproveDateFrom", "#ApproveDateTo");
    $("#QuotationCode").removeClass("highlight");
    CloseWarningDialog();

    initial();
}

function initial() {
    //Set default as Today 
    var currentDate = new Date();
    SetDateFromToData("#ApproveDateFrom", "#ApproveDateTo", null, currentDate);

    //Hide "Result list" section
    $("#Search_Result").hide();
}

function search() {
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    //Load Data to Grid
    var parameter = CreateObjectData($("#CTS040_Search").serialize());
    $("#grid_result").LoadDataToGrid(mygrid, pageRow, true, "/Contract/CTS040_Search", parameter, "dtSearchDraftContractResult", false,
        function (result, controls, isWarning) { //post-load
            $("#btnSearch").removeAttr("disabled");
            master_event.LockWindow(false);

            if (result != undefined) {
                master_event.ScrollWindow("#Search_Result");
            }
        },
        function (result, controls, isWarning) { //pre-load
            if (isWarning == undefined) {
                //Show Search Result
                $("#Search_Result").show();
            }
        });
}

function validateSearchCriteria() {
    var parameter = CreateObjectData($("#CTS040_Search").serialize());
    call_ajax_method(
        '/Contract/CTS040_CheckReqField',
        parameter,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["QuotationCode"], controls);
                $("#QuotationCode").focus();
                return;
            } else {
                $("#QuotationCode").removeClass("highlight");
                search();
            }
        }
    );
}


