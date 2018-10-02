/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxgrid.js" />
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxgridcell.js" />


/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

/// <reference path="../../Scripts/Common/Dialog.js"/>

/* --- Variables --- */
var objParam;
var mygrid_cms250;
var pageRow_cms250 = ROWS_PER_PAGE_FOR_SEARCHPAGE;

var objGroup;

var CMS250_Count = 0;

/*--- Main ---*/
$(document).ready(function () {
    //master_event.LoadingEffect();
    /* --- set default value ---*/
    objParam = { "bExistCustOnlyFlag": false };

    /*-- keep IN parameter from caller page --*/
    if (typeof (CMS250Object) == "function") {
        objParam = CMS250Object();
    }


    initialPage();

    /* --- Initial grid --- */
    if ($.find("#gridCustomerDetail").length > 0) {

        mygrid_cms250 = $("#gridCustomerDetail").InitialGrid(pageRow_cms250, true, "/Common/CMS250_InitialGrid");

        /* ===== binding on row select===== */
        //mygrid_cms250.attachEvent("onRowSelect", doSelect);

        /*=========== Set hidden column =============*/
        var colHiddenIndex = mygrid_cms250.getColIndexById("Object");
        mygrid_cms250.setColumnHidden(colHiddenIndex, true);


    }

    SpecialGridControl(mygrid_cms250, ["Button"]);

    /* ===== binding event when finish load data ===== */
    BindOnLoadedEvent(mygrid_cms250, function (gen_ctrl) {
        var colInx = mygrid_cms250.getColIndexById('Button');
        for (var i = 0; i < mygrid_cms250.getRowsNum(); i++) {

            var row_id = mygrid_cms250.getRowId(i);

            if (gen_ctrl == true) {
                GenerateSelectButton(mygrid_cms250, "btnSelect", row_id, "Button", true);
            }

            // binding grid button event 
            BindGridButtonClickEvent("btnSelect", row_id, doSelect);
        }

        mygrid_cms250.setSizes();

    });

    /* ===== binding event search button ===== */
    $("#btnSearch_cms250").click(function () {

        CMS250_Search();

    });

    /*==== event Customer Name keypress ====*/
    //$("#divSearchAddrCtrl input[id=CustomerName]").keypress(txtCustomerName_keypress);
    //$("#divSearchAddrCtrl input[id=CustomerName]").keyup(txtCustomerName_keyup);
    $("#divSearchAddrCtrl input[id=CustomerName]").InitialAutoComplete("/Master/GetCustName"); // Note: in Master/Controllers/CustomerData.cs/GetCustName()

    /*==== event Gruop Name keypress ====*/
    //$("#divSearchAddrCtrl input[id=GroupName]").keypress(txtGroupName_keypress);
    //$("#divSearchAddrCtrl input[id=GroupName]").keyup(txtGroupName_keyup);
    $("#divSearchAddrCtrl input[id=GroupName]").InitialAutoComplete("/Master/GetGroupName"); // Note : in Master/Controllers/MAS060_MaintainCustomerGroupInformation.cs/GetGroupName()



    /*==== event chkJuristic checked change ====*/
    $('#chkJuristic').change(function () {

        // if chkJuristic is check then enable cboCompanyType

        if ($('#chkJuristic').is(':checked')) {
            $('#CompanyTypeCode').attr('disabled', false);
        } else {
            $('#CompanyTypeCode').val('');
            $('#CompanyTypeCode').attr('disabled', true);
        }
    });

    /* =========== event Clear button ===========*/
    $("#btnClear_cms250").click(function () {
        $("#searchCondition").clearForm();
        initialPage();

        CMS250_Count = 0;
        CloseWarningDialog();
    });


    //    InitialTrimTextEvent([
    //        "CustomerCode",
    //        "CustomerName",
    //        "IDNo",
    //        "Address",
    //        "Alley",
    //        "Road",
    //        "SubDistrict",
    //        "ZipCode",
    //        "GroupName"
    //    ]);

});

function CMS250_Search() {

    // disable search button
    $("#btnSearch_cms250").attr("disabled" , true);
    master_event.LockWindow(true);

    // keep group name 
    $("#CMS250_groupName").val($.trim($("#divSearchCondition input[id=GroupName]").val()));
    
    var data = $("#searchCondition").serialize();
    //alert(data);

    var parameter = CreateObjectData($("#searchCondition").serialize() + "&Counter=" + CMS250_Count , true);
    $("#gridCustomerDetail").LoadDataToGrid(mygrid_cms250, pageRow_cms250, true, "/Common/CMS250_SearchResponse", parameter, "View_dtCustomerData2", false,
        function () {
            // enable search button
            $("#btnSearch_cms250").attr("disabled", false);
            master_event.LockWindow(false);

            //document.getElementById('divSearchResult').scrollIntoView();
            master_event.ScrollWindow("#divSearchResult", true);

        }, //post-load
        function (result, controls, isWarning) { //pre-load
            if (isWarning == undefined) {
                $("#divSearchResult").show();
            }
        });

}



/*---- Event ------*/
//function txtCustomerName_keyup(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val();  //+ String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#divSearchAddrCtrl input[id=CustomerName]",
//                                cond,
//                                "/Master/GetCustName", // Note: in Master/Controllers/CustomerData.cs/GetCustName()
//                                {"cond": cond },
//                                "dtCustName",
//                                "CustName",
//                                "CustName");
//    }
//}


/*---- Event ------*/
//function txtGroupName_keyup(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val();// + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#divSearchAddrCtrl input[id=GroupName]",
//                                cond,
//                                "/Master/GetGroupName",  // Note : in Master/Controllers/MAS060_MaintainCustomerGroupInformation.cs/GetGroupName()
//                                {"cond": cond },
//                                "doGroupNameDataList",
//                                "GroupName",
//                                "GroupName");
//    }
//}

/*---- Event ------*/

// ind = colum index
function doSelect(row_id) {

    mygrid_cms250.selectRow(mygrid_cms250.getRowIndex(row_id));

    var customerCode = mygrid_cms250.cells2(mygrid_cms250.getRowIndex(row_id), mygrid_cms250.getColIndexById('CustCode')).getValue();

    var groupName = $("#CMS250_groupName").val();

    /* ==== Create json object for string json ==== */
    var strJson = mygrid_cms250.cells2(mygrid_cms250.getRowIndex(row_id), mygrid_cms250.getColIndexById('Object')).getValue();
    strJson = htmlDecode(strJson);
    var objSelcted = JSON.parse(strJson); // require json2.js


    var param = { "strCustomerCode": customerCode, "strGroupName": groupName };

    call_ajax_method("/Common/CMS250_GetCustomerGroup", param, function (data) {

        objGroup = data;
        var result = { "CustomerData": objSelcted, "CustomerGroupList": objGroup };

        if (typeof (CMS250Response) == "function") {
            CMS250Response(result);
        }

    });



}

/*---- Method ------*/
function initialPage() {

    // set checkbox to "checked"
    $('#chkNewCustomer').attr('checked', true);
    $('#chkExistCustomer').attr('checked', true);
    $('#chkJuristic').attr('checked', true);
    $('#chkOther').attr('checked', true);

    //**
    $('#DummyIDFlag').attr('checked', false);

    // if ExistCustOnlyFlag = true then disable chkNewCustomer
    if (objParam.bExistCustOnlyFlag == true) {
        $("#chkNewCustomer").attr("disabled", true);
        $('#chkNewCustomer').attr('checked', false);

    }
    else {

        $("#chkNewCustomer").attr("disabled", false);
    }

    $("#divSearchResult").hide();

}

function CMS250Initial() {
    ChangeDialogButtonText(["Cancel"], [$('#btnCancel_CMS250').val()]);
}


