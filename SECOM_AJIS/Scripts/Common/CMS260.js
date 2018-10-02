/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>

/// <reference path="../../Scripts/Common/Dialog.js"/>

/* --- Variables --- */

var objParam;
var mygrid_cms260;
var pageRow_cms260 = ROWS_PER_PAGE_FOR_SEARCHPAGE;

var CMS260_Count = 0;
var CMS260_isSearchByRealCust = false;

/* --- Initial --- */

/*-------- Main ---------*/
$(document).ready(function () {

    /* --- set default value */
    objParam = { "strRealCustomerCode": "" };

    /*-- keep IN parameter from caller page --*/
    if (typeof (CMS260Object) == "function") {
        objParam = CMS260Object();
    }

    /*-- Initial search condition controls-- */
    initalPage();

    /*-- Initial grid-- */

    mygrid_cms260 = $("#mygrid_container").InitialGrid(pageRow_cms260, true, "/Common/CMS260_InitialGrid");

    /*===== binding on row select===== */
    //mygrid_cms260.attachEvent("onRowSelect", doSelect);

    /*=========== Set hidden column =============*/
    var colHiddenIndex = mygrid_cms260.getColIndexById("Object");
    mygrid_cms260.setColumnHidden(colHiddenIndex, true);

    SpecialGridControl(mygrid_cms260, ["Button"]);

    /* ===== binding event when finish load data ===== */
    BindOnLoadedEvent(mygrid_cms260, function (gen_ctrl) {
        var colInx = mygrid_cms260.getColIndexById('Button');
        for (var i = 0; i < mygrid_cms260.getRowsNum(); i++) {

            var row_id = mygrid_cms260.getRowId(i);

            if (gen_ctrl == true) {
                GenerateSelectButton(mygrid_cms260, "btnSelect", row_id, "Button", true);
            }

            // binding grid button event 
            BindGridButtonClickEvent("btnSelect", row_id, doSelect);
        }

        var colObjectInx = mygrid_cms260.getColIndexById('Object');
        mygrid_cms260.setColumnHidden(colObjectInx, true);

        mygrid_cms260.setSizes();

    });

    $("#btnSearch").click(function () {

        CMS260_Search();

    });

    $("#btnClear").click(function () {
        $("#searchCondition").clearForm();
        initalPage();

        CMS260_Count = 0;
        CloseWarningDialog();
    });

    /*==== event Customer Name keypress ====*/
    //$("#divSearchAddrCtrl input[id=CustomerName]").keyup(txtCustomerName_keyup);
    $("#divSearchAddrCtrl input[id=CustomerName]").InitialAutoComplete("/Master/GetCustName");  // Note : in Master/Controllers/CustomerData.cs/GetCustName()


    /*==== event Site Name keypress ====*/
    //$("#divSearchAddrCtrl input[id=SiteName]").keyup(txtSiteName_keyup);
    $("#divSearchAddrCtrl input[id=SiteName]").InitialAutoComplete("/Master/GetSiteName"); // Note : in Master/Controllers/SiteData.cs/GetSiteName()


    //    InitialTrimTextEvent([
    //        "CustomerCode",
    //        "CustomerName",
    //        "SiteCode",
    //        "SiteName",
    //        "Address",
    //        "Alley",
    //        "Road",
    //        "SubDistrict",
    //        "ZipCode"
    //    ]);

});


function CMS260_Search() {

    // disable search button
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    var parameter = CreateObjectData($("#searchCondition").serialize() + "&isSearchByRealCust=" + CMS260_isSearchByRealCust + "&Counter=" + CMS260_Count ,true);

    if (objParam.strRealCustomerCode != "") {
        parameter.CustomerCode = objParam.strRealCustomerCode;
    }

    $("#mygrid_container").LoadDataToGrid(mygrid_cms260, pageRow_cms260, true, "/Common/CMS260_SearchResponse", parameter, "View_dtSiteData", false,
        function () {
            // enable search button
            $("#btnSearch").attr("disabled", false);
            master_event.LockWindow(false);

            //document.getElementById('divSearchResult').scrollIntoView();
            master_event.ScrollWindow("#divSearchResult", true);
        },
        function (result, controls, isWarning) {
            if (isWarning == undefined) {
                $("#divSearchResult").show();
            }
        });

}

/*---- Event CustomerName keypress------*/

//function txtCustomerName_keyup(e) {

//    if ($(this).val().length + 1 >= 3) {

//        var cond = $(this).val(); // + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#divSearchAddrCtrl input[id=CustomerName]",
//                                cond,
//                                "/Master/GetCustName", // Note : in Master/Controllers/CustomerData.cs/GetCustName()
//                                {"cond": cond },
//                                "dtCustName",
//                                "CustName",
//                                "CustName");

//    }
//}

/*---- Event SiteName keypress ------*/
//function txtSiteName_keyup(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val();//  + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#divSearchAddrCtrl input[id=SiteName]",
//                                cond,
//                                "/Master/GetSiteName", // Note : in Master/Controllers/SiteData.cs/GetSiteName()
//                                {"cond": cond },
//                                "dtSiteName",
//                                "SiteName",
//                                "SiteName");
//    }
//}



/* --- Methods --- */
function initalPage() {

    $("#divSearchResult").hide();

    if (objParam.strRealCustomerCode == "" || objParam.strRealCustomerCode == null /*"undefined"*/) {

        //**
        CMS260_isSearchByRealCust = true;

        /* == if not send strRealCustomerCode then hind (1) show (2) , (3) */
        $("#SelectRealCustomer_Section").hide();
        $("#space1").hide();
        $("#SearchByCustomer_Section").show();
        $("#space2").show();
        $("#SearchBySite_Section").show();

        // chkNewCustomer
        $("#chkNewCustomer").attr("checked", true);

        // chkExistingCustomer
        $("#chkExistingCustomer").attr("checked", true);
    }
    else {

        
        /* == if send strRealCustomerCode then hind (2) show (1) , (3) */
        $("#SelectRealCustomer_Section").show();
        $("#space1").show();
        $("#SearchByCustomer_Section").hide();
        $("#space2").hide();
        $("#SearchBySite_Section").show();
    }
}


/* -----------  event ----------- */
// ind = colum index
function doSelect(row_id) {


    mygrid_cms260.selectRow(mygrid_cms260.getRowIndex(row_id));

    /* ==== Create json object for string json ==== */
    var strJson = mygrid_cms260.cells2(mygrid_cms260.getRowIndex(row_id), mygrid_cms260.getColIndexById('Object')).getValue().toString();
    strJson = htmlDecode(strJson);
    var objSelcted = JSON.parse(strJson); // require json2.js

    //alert(strJson);

    if (typeof (CMS260Response) == "function") {
        CMS260Response(objSelcted);
    }



}

function CMS260Initial() {
    ChangeDialogButtonText(["Cancel"], [$('#btnCancel_CMS260').val()]);
}


