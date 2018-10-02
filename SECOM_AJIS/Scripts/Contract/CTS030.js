
/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>

/* --- Variables --- */
var mygrid;
var pageRow; // = 5;
var btnAuditId = "CTS030AuditBtn";

/*--- Main ---*/
$(document).ready(function () {

    pageRow = CTS030Data.PageRow;
    
    /* --- Initial grid --- */
    if ($("#grid_result").length > 0) {
        mygrid = $("#grid_result").InitialGrid(pageRow, true, "/Contract/InitialGrid_CTS030");

        /*=========== Set hidden column =============*/
        mygrid.setColumnHidden(mygrid.getColIndexById("ApprovalStatusCode"), true);
        mygrid.setColumnHidden(mygrid.getColIndexById("ProductTypeCode"), true);
        mygrid.setColumnHidden(mygrid.getColIndexById("ProductTypeCodeName"), true);
        mygrid.setColumnHidden(mygrid.getColIndexById("QuotationTargetCodeShow"), true);
        mygrid.setColumnHidden(mygrid.getColIndexById("OCC"), true);
    }

    /*==== Attach Datetime Picker ====*/
    InitialDateFromToControl("#RegistrationDateFrom", "#RegistrationDateTo");

    /*==== event Customer Name keypress ====*/
    $("#ContractTargetName").InitialAutoComplete("/Master/GetCustName"); //$("#ContractTargetName").keypress(ContractTargetName_keypress);

    /*==== event Employee Name keypress ====*/
    $("#Salesman1Name").InitialAutoComplete("/Master/MAS070_GetEmployeeName"); //$("#Salesman1Name").keypress(Salesman1Name_keypress);

    /*==== event Site Name keypress ====*/
    $("#SiteName").InitialAutoComplete("/Master/GetSiteName"); //$("#SiteName").keypress(SiteName_keypress);

    /*==== event btnSearch click ====*/
    $("#btnSearch").click(function () {
        CTS030Data.CurrentIndex = null;
        CTS030Data.CurrentSortColIndex = null;
        CTS030Data.CurrentSortType = null;

        validateSearchCriteria();
    });

    /*==== event btnClear click ====*/
    $("#btnClear").click(function () {
        clearSearchCriteria();
    });

    initial();

    if (CTS030Data.HasSessionData == "True") {
        search();
        //        var pageIdx = parseInt(CTS030Data.CurrentPage);
        //        mygrid.changePage(pageIdx);
    }

    /* ===== binding event when finish load data ===== */
    SpecialGridControl(mygrid, ["Audit"]);

    var funcDrawControl = function (rid) {
        var auditColInx = mygrid.getColIndexById('Audit');
        var approvalCodeColInx = mygrid.getColIndexById('ApprovalStatusCode');
        var contractCodeColInx = mygrid.getColIndexById('ContractCodeShow');
        var occColInx = mygrid.getColIndexById('OCC');
        var productTypeCodeNameColInx = mygrid.getColIndexById('ProductTypeCodeName');
        var url;
        var tagA;

        if (mygrid.cells(rid, auditColInx).getValue() == "") { //Generate control if is empty.
            /* ===== column Audit button ===== */
            if (mygrid.cells(rid, approvalCodeColInx).getValue() == 1) {
                mygrid.cells(rid, auditColInx).setValue(GenerateHtmlButton(btnAuditId, rid, CTS030Data.AuditLabel, true));
            }
            else {
                mygrid.cells(rid, auditColInx).setValue(GenerateHtmlButton(btnAuditId, rid, CTS030Data.AuditLabel, false));
            }

            var contractCode = mygrid.cells(rid, contractCodeColInx).getValue();
            var occ = mygrid.cells(rid, occColInx).getValue();
            var productTypeCodeName = mygrid.cells(rid, productTypeCodeNameColInx).getValue();
            if (contractCode != '-') {
                var tagA = "<a href='#'>" + contractCode + "<input type='hidden' name='contractCode' value='" + contractCode + "'/><input type='hidden' name='occ' value='" + occ + "'/><input type='hidden' name='productTypeCodeName' value='" + productTypeCodeName + "'/></a>";
                mygrid.cells(rid, contractCodeColInx).setValue(tagA);
            }

            /* ===== Bind event onClick to button ===== */
            BindGridHtmlButtonClickEvent(btnAuditId, rid, function (irid) {
                doAuditAction(irid);
            });
        }

        $("#grid_result a").each(function () {
            $(this).unbind("click");
            $(this).click(function () {
                if ($(this).children("input:hidden[name=productTypeCodeName]").val() == "sale") {
                    var objCMS160 =
                        {
                            strContractCode: $(this).children("input:hidden[name=contractCode]").val(),
                            strOCC: $(this).children("input:hidden[name=occ]").val()
                        };
                    ajax_method.CallScreenControllerWithAuthority("/Common/CMS160", objCMS160, true);
                } else {
                    var objCMS120 =
                        {
                            strContractCode: $(this).children("input:hidden[name=contractCode]").val()
                        };
                    ajax_method.CallScreenControllerWithAuthority("/Common/CMS120", objCMS120, true);
                }
            });
        });
    };
    BindOnLoadedEventV2(mygrid, pageRow, true, true, funcDrawControl);



    //    BindOnLoadedEvent(mygrid, function (gen_ctrl) {
    //        var auditColInx = mygrid.getColIndexById('Audit');
    //        var approvalCodeColInx = mygrid.getColIndexById('ApprovalStatusCode');
    //        var contractCodeColInx = mygrid.getColIndexById('ContractCodeShow');
    //        var occColInx = mygrid.getColIndexById('OCC');
    //        var productTypeCodeNameColInx = mygrid.getColIndexById('ProductTypeCodeName');
    //        var url;
    //        var tagA;
    //        for (var i = 0; i < mygrid.getRowsNum(); i++) {

    //            var row_id = mygrid.getRowId(i);

    //            if (gen_ctrl == true) {
    //                /* ===== column Audit button ===== */
    //                if (mygrid.cells2(i, approvalCodeColInx).getValue() == 1) {
    //                    //mygrid.cells2(i, auditColInx).setValue("<button id='btnAudit' style='width:auto' >Audit</button>");
    //                    mygrid.cells2(i, auditColInx).setValue(GenerateHtmlButton(btnAuditId, row_id, CTS030Data.AuditLabel, true));
    //                }
    //                else {
    //                    //mygrid.cells2(i, auditColInx).setValue("<button id='btnAudit' disabled='true' style='width:auto' >Audit</button>");
    //                    mygrid.cells2(i, auditColInx).setValue(GenerateHtmlButton(btnAuditId, row_id, CTS030Data.AuditLabel, false));
    //                }

    //                /* ===== column Contract Code link ===== */
    //                /*
    //                1: Sale
    //                2: Alarm　
    //                3: Sale online　
    //                4: Beat guard　
    //                5: Sentry guard　
    //                6: Maintenance　
    //                */
    //                var contractCode = mygrid.cells2(i, contractCodeColInx).getValue();
    //                var occ = mygrid.cells2(i, occColInx).getValue();
    //                var productTypeCodeName = mygrid.cells2(i, productTypeCodeNameColInx).getValue();
    //                if (contractCode != '-') {
    //                    var tagA = "<a href='#'>" + contractCode + "<input type='hidden' name='contractCode' value='" + contractCode + "'/><input type='hidden' name='occ' value='" + occ + "'/><input type='hidden' name='productTypeCodeName' value='" + productTypeCodeName + "'/></a>";
    //                    mygrid.cells2(i, contractCodeColInx).setValue(tagA);
    //                }
    //            }

    //            /* ===== Bind event onClick to button ===== */
    //            BindGridHtmlButtonClickEvent(btnAuditId, row_id, function (rid) {
    //                doAuditAction(rid);
    //            });
    //        }

    //        $("#grid_result a").each(function () {
    //            $(this).click(function () {
    //                if ($(this).children("input:hidden[name=productTypeCodeName]").val() == "sale") {
    //                    var objCMS160 =
    //                        {
    //                            strContractCode: $(this).children("input:hidden[name=contractCode]").val(),
    //                            strOCC: $(this).children("input:hidden[name=occ]").val()
    //                        };
    //                    ajax_method.CallScreenControllerWithAuthority("/Common/CMS160", objCMS160, true);
    //                } else {
    //                    var objCMS120 =
    //                        {
    //                            strContractCode: $(this).children("input:hidden[name=contractCode]").val()
    //                        };
    //                    ajax_method.CallScreenControllerWithAuthority("/Common/CMS120", objCMS120, true);
    //                }
    //            });
    //        });
    //    });
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

function CheckPermittedIPAddress() {
    call_ajax_method(
        '/Contract/isPermittedIPAddress',
        null,
        function (data) {
            if (data == false) {
                var param = { "module": "Common", "code": "MSG0053" };
                call_ajax_method("/Shared/GetMessage", param, function (data) {

                    /* ====== Open error dialog =====*/
                    OpenErrorDialog(data.Code, data.Message); //You do not have a permission to operate this screen.
                });
            }
        }
    );
}

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

    ClearDateFromToControl("#RegistrationDateFrom", "#RegistrationDateTo");
    $("#QuotationCode").removeClass("highlight");
    CloseWarningDialog();

    initial();

    var currentDate = ConvertDateObject(new Date());
    SetDateFromToData("#RegistrationDateFrom", "#RegistrationDateTo", null, currentDate);
}

function initial() {

    if (CTS030Data.HasSessionData == "False") {
        //Set default as Today 
        var currentDate = ConvertDateObject(new Date());
        SetDateFromToData("#RegistrationDateFrom", "#RegistrationDateTo", null, currentDate);
    }

    //Hide "Result list" section
    $("#Search_Result").hide();
}

function search() {

    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    //Load Data to Grid
    var parameter = CreateObjectData($("#CTS030_Search").serialize());
    $("#grid_result").LoadDataToGrid(mygrid, pageRow, true, "/Contract/CTS030_Search", parameter, "dtSearchDraftContractResult", false,
        function (result, controls, isWarning) { //post-load
            $("#btnSearch").removeAttr("disabled");
            master_event.LockWindow(false);

            if (result != undefined) {
                if (CTS030Data.HasSessionData == "True") {
                    mygrid.selectRow(CTS030Data.CurrentIndex);

                    if (CTS030Data.CurrentSortColIndex != undefined
                            && CTS030Data.CurrentSortColIndex >= 0) {
                        mygrid.setSortImgState(true, CTS030Data.CurrentSortColIndex, CTS030Data.CurrentSortType);

                        CurrentSortColIndex = CTS030Data.CurrentSortColIndex;
                        CurrentSortType = CTS030Data.CurrentSortType;

                        master_event.ScrollWindow("tr[idd='" + mygrid.getRowId(CTS030Data.CurrentIndex) + "']", false, true);
                    }
                    else {
                        master_event.ScrollWindow("#Search_Result");
                    }
                }

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

    var parameter = CreateObjectData($("#CTS030_Search").serialize());
    call_ajax_method(
        '/Contract/CTS030_CheckReqField',
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

function doAuditAction(rid) {

    //hilight row
    mygrid.selectRow(mygrid.getRowIndex(rid));

    var lst = new Array();
    var qIdx = mygrid.getColIndexById("KeyIndex");
    for (var i = 0; i < mygrid.getRowsNum(); i++) {
        var iobj = {
            KeyIndex: mygrid.cells2(i, qIdx).getValue().toString()
        };
        lst.push(iobj);
    }

    var param = {
        CurrentIndex: mygrid.getRowIndex(rid),
        list: lst,
        CurrentSortColIndex: CurrentSortColIndex != undefined ? CurrentSortColIndex : -1,
        CurrentSortType: CurrentSortType != undefined ? CurrentSortType : "ASC"
    };

    ajax_method.CallScreenController(
        '/Contract/CTS030_SetAuditBtnClickFlag',
        param,
        function (result, controls) {
            if (result != undefined) {

                var productTypeCodeNameColInx = mygrid.getColIndexById('ProductTypeCodeName');
                var productTypeCodeName = mygrid.cells(rid, productTypeCodeNameColInx).getValue();
                var quotationTargetCodeShowColInx = mygrid.getColIndexById('QuotationTargetCodeShow');
                var quotationTargetCode = mygrid.cells(rid, quotationTargetCodeShowColInx).getValue().toString();
                var currentSessionKey = ajax_method.GetKeyURL(null);

                var obj = {
                    QuotationTargetCode: quotationTargetCode,
                    CallerSessionKey: currentSessionKey
                };

                if (productTypeCodeName == "sale")
                    ajax_method.CallScreenControllerWithAuthority("/Contract/CTS020", obj, false, 2);
                else
                    ajax_method.CallScreenControllerWithAuthority("/Contract/CTS010", obj, false, 2);
            }
        }
    );
}

