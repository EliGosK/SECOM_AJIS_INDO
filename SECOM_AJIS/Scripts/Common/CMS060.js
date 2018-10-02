

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


var pageRow = ROWS_PER_PAGE_FOR_SEARCHPAGE;
var mygrid_cms060;
var doEmailAddress = [];
var CMS060_Count = 0;


$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name]) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

// Main
$(document).ready(function () {
    if ($.find("#mygrid_container").length > 0) {
        /*----initial grid-----*/
        mygrid_cms060 = $("#mygrid_container").InitialGrid(pageRow, true, "/Common/CMS060_InitialGrid");

    }
    $("#divSearchResult").hide();

    SpecialGridControl(mygrid_cms060, ["Button"]);

    /* ===== binding event when finish load data ===== */
    BindOnLoadedEvent(mygrid_cms060, function (gen_ctrl) {
        var colInx = mygrid_cms060.getColIndexById('Button');
        for (var i = 0; i < mygrid_cms060.getRowsNum(); i++) {
            var rowId = mygrid_cms060.getRowId(i);

            if (gen_ctrl == true) {
                GenerateAddButton(mygrid_cms060, "btnAdd", rowId, "Button", true);
            }

            // binding grid button event 
            BindGridButtonClickEvent("btnAdd", rowId, AddEmailList);

        }
        mygrid_cms060.setSizes();
    });

    // Event binding for btnSearch , btnClear , btnOk , btnCancle
    $("#btnSearch_cms060").click(
                function () {

                    CMS060_Search();

                }
            );

    $("#btnClear_cms060").click(
                function () {
                    doClearAll();
                    CloseWarningDialog();
                }
            );

    //    InitialTrimTextEvent([
    //        "EmployeeName",
    //        "EmailEddress"
    //    ]);



});



// ========= Private Function ============

function CMS060_Search() {

    $("#btnSearch_cms060").attr("disabled", true);
    master_event.LockWindow(true);

    var parameter = CreateObjectData($("#formSearch").serialize() + "&Counter=" + CMS060_Count, true);

    //var parameter = $("#formSearch").serializeObject();

    $("#mygrid_container").LoadDataToGrid(mygrid_cms060, pageRow, true, "/Common/CMS060_SearchResponse", parameter, "View_dtEmailAddress", false,
                    function () {
                        $("#btnSearch_cms060").attr("disabled", false);
                        master_event.LockWindow(false);
                    }, // post-load
                    function (result, controls, isWarning) { // pre-load

                        if (isWarning == undefined) {
                            $("#divSearchResult").show();
                        }

                    });
}

function doClearAll() {


    $("#divSearchCondition input[id=EmployeeName]").val('');
    $("#divSearchCondition input[id=EmailEddress]").val('');
    $("#divSearchCondition select[id=Office]").get(0).selectedIndex = 0;
    $("#divSearchCondition select[id=Department]").get(0).selectedIndex = 0;

    $("#divSearchResult").hide();

    CMS060_Count = 0;
}

function doOk() {
    if (typeof (CMS060Response) == "function") {
        CMS060Response(doEmailAddress);
    }
}


function AddEmailList(row_id) {
    //alert(row_id);

    mygrid_cms060.selectRow(mygrid_cms060.getRowIndex(row_id));

    var EmailAddress = mygrid_cms060.cells2(mygrid_cms060.getRowIndex(row_id), mygrid_cms060.getColIndexById('EmailAddress')).getValue();
    var EmpNo = mygrid_cms060.cells2(mygrid_cms060.getRowIndex(row_id), mygrid_cms060.getColIndexById('EmpNo')).getValue();

    // trim
    EmailAddress = $.trim(EmailAddress);
    EmpNo = $.trim(EmpNo);


    if (EmailAddress == "") {
        var messageParam = { "module": "Common", "code": "MSG0131", "param": "" };
        call_ajax_method("/Shared/GetMessage", messageParam, function (data) {
            OpenWarningDialog(data.Message);
        });
        return;
    }

    // Check exist email in selected list.
    var oldHtml = $("#selectedemail").html();

    //if (oldHtml.toString().search(EmpNo) != -1) {
    if (oldHtml.toString().toUpperCase().search(EmailAddress.toUpperCase()) != -1) { //Modify by Jutarat A. on 21112012

        //alert(oldHtml.toString().search(EmpNo) );

        // MSG0107 : "E-mail , {0}, already exists"

        var param = { "module": "Common", "code": "MSG0107", "param": EmailAddress };
        call_ajax_method("/Shared/GetMessage", param, function (data) {

            /* ====== Open waning dialog =====*/
            //OpenInformationMessageDialog(param.code, data.Message);
            OpenWarningDialog(data.Message);

        });

        return;
    }


    var EmpNameEN = mygrid_cms060.cells2(mygrid_cms060.getRowIndex(row_id), mygrid_cms060.getColIndexById('EmpNameEN')).getValue().toString();
    var EmpNameLC = mygrid_cms060.cells2(mygrid_cms060.getRowIndex(row_id), mygrid_cms060.getColIndexById('EmpNameLC')).getValue().toString();
    var OfficeCode = mygrid_cms060.cells2(mygrid_cms060.getRowIndex(row_id), mygrid_cms060.getColIndexById('OfficeCode')).getValue().toString();
    var DepartmentCode = mygrid_cms060.cells2(mygrid_cms060.getRowIndex(row_id), mygrid_cms060.getColIndexById('DepartmentCode')).getValue().toString();


    var strEmailObject = mygrid_cms060.cells2(mygrid_cms060.getRowIndex(row_id), mygrid_cms060.getColIndexById('Object')).getValue().toString();
    //alert(strEmailObject);
    strEmailObject = htmlDecode(strEmailObject);

    /* ==== Add selected email object list ==== */
    var objEmail = JSON.parse(strEmailObject); // require json2.js


    ajax_method.CallScreenController("/Common/CMS060_CheckEmailSuffix", [objEmail], function (result) {
        if (result == true) {
            doEmailAddress.push(objEmail);


            // Add selected email to "To" section (as display) (li)
            //var htmlEmail = "<li><span class='email'>" + EmailAddress + "<a id='remove'  class='remove'   href=''>X</a><span style='display:none;'>" + EmpNo + "</span></li>";

            var htmlEmail = "<dt class='email' id='" + EmpNo + "' style='margin-left:2px ;' >" + EmailAddress + "<a id='remove'  class='remove' style='cursor:pointer; ' >X</a><span style='display:none;'>" + EmpNo + "</span></dt>";

            $("#selectedemail").html($("#selectedemail").html() + htmlEmail);


            // ====== Event when click x of each email in "To" selection ======
            $("#selectedemail a").click(function () {

                // === 1. Remove email from list object
                var empNoVal = $(this).next("span").html();

                var removedIdx = search_array_index(doEmailAddress, "EmpNo", empNoVal);

                //alert("removedIdx = " + removedIdx + " doEmailAddress[removedIdx] = " + doEmailAddress[removedIdx].EmpNo + " empNoVal = " + empNoVal);

                // Remark: return value from search_array_index -1 mean search not found , -2 mean key column name is invalid.
                if (removedIdx > -1) {
                    doEmailAddress.splice(removedIdx, 1);

                    // === 2. Remove from "To" section (as display)
                    //var element = $(this).parent().parent();
                    var element = $(this).parent();
                    element.html("");
                    element.remove();

                    //alert("remove ok");

                }



                return false;
            });
        }
    });

    
}

function CMS060Initial(param) {

    if (param != undefined) {

        for (var i = 0; i < param.EmailList.length; i++) {
            var EmailAddress = param.EmailList[i].EmailAddress;
            var EmpNo = param.EmailList[i].EmpNo;
            //alert(param.EmailList[i].EmailAddress);
            //alert(param.EmailList[i].EmpNo);
            var htmlEmail = "<dt class='email' id='" + EmpNo + "' style='margin-left:2px ;' >" + EmailAddress + "<a id='remove'  class='remove' style='cursor:pointer; ' >X</a><span style='display:none;'>" + EmpNo + "</span></dt>";

            $("#selectedemail").html($("#selectedemail").html() + htmlEmail);

            var strEmailObject = { "EmpNo": EmpNo, "EmailAddress": EmailAddress };
            //strEmailObject = htmlDecode(strEmailObject+"");

            /* ==== Add selected email object list ==== */
            //var objEmail = JSON.parse(strEmailObject); // require json2.js
            doEmailAddress.push(strEmailObject);


            // ====== Event when click x of each email in "To" selection ======
            $("#selectedemail a").click(function () {

                // === 1. Remove email from list object
                var empNoVal = $(this).next("span").html();

                var removedIdx = search_array_index(doEmailAddress, "EmpNo", empNoVal);

                //alert("removedIdx = " + removedIdx + " doEmailAddress[removedIdx] = " + doEmailAddress[removedIdx].EmpNo + " empNoVal = " + empNoVal);

                // Remark: return value from search_array_index -1 mean search not found , -2 mean key column name is invalid.
                if (removedIdx > -1) {
                    doEmailAddress.splice(removedIdx, 1);

                    // === 2. Remove from "To" section (as display)
                    //var element = $(this).parent().parent();
                    var element = $(this).parent();
                    element.html("");
                    element.remove();

                    //alert("remove ok");

                }



                return false;
            });
        }
    }


    ChangeDialogButtonText(["OK"], [$('#btnOk_cms060').val()]);
    ChangeDialogButtonText(["Cancel"], [$('#btnCancel_cms060').val()]);
    BindDialogButtonClick($('#btnOk_cms060').val(), function () {

        doOk();
    });
}
