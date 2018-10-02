/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>

/// <reference path="../../Scripts/Base/GridControl.js"/>

/* --- Variables --- */
var CMS310_RowPerPage = ROWS_PER_PAGE_FOR_SEARCHPAGE;
var mygrid_cms310;

var CMS310_Count = 0;

$(document).ready(function () {



    initialPage();

    /* --- Initial grid --- */
    if ($.find("#mygrid_container").length > 0) {

        mygrid_cms310 = $("#mygrid_container").InitialGrid(CMS310_RowPerPage, true, "/Common/CMS310_InitialGrid");


        /*=========== Set hidden column =============*/
        var colHiddenIndex = mygrid_cms310.getColIndexById("Object");
        mygrid_cms310.setColumnHidden(colHiddenIndex, true);

    }

    SpecialGridControl(mygrid_cms310, ["Button"]);

    /* ===== binding event when finish load data ===== */
    BindOnLoadedEvent(mygrid_cms310, function (gen_ctrl) {
        var colInx = mygrid_cms310.getColIndexById('Button');
        for (var i = 0; i < mygrid_cms310.getRowsNum(); i++) {

            var row_id = mygrid_cms310.getRowId(i);

            if (gen_ctrl == true) {
                GenerateSelectButton(mygrid_cms310, "btnSelect", row_id, "Button", true);
            }

            // binding grid button event 
            BindGridButtonClickEvent("btnSelect", row_id, doSelect);
        }

        mygrid_cms310.setSizes();

    });

    /* ===== binding event search button ===== */
    $("#btnSearch").click(function () {

        CMS310_Search();

    });

    /* =========== event Clear button ===========*/
    $("#btnClear").click(function () {
        $("#searchCondition").clearForm();
        initialPage();

        CMS310_Count = 0;
        CloseWarningDialog();
    });

    /*==== event Customer Name keypress ====*/
    //$("#divSearchAddrCtrl input[id=CustomerName]").keyup(txtCustomerName_keyup);
    $("#divSearchAddrCtrl input[id=CustomerName]").InitialAutoComplete("/Master/GetCustName"); // Note: in Master/Controllers/CustomerData.cs/GetCustName()



    /*==== event Branch Name keypress ====*/
    //$("#divSearchAddrCtrl input[id=BranchName]").keyup(txtBranchName_keyup);
    $("#divSearchAddrCtrl input[id=BranchName]").InitialAutoComplete("/Contract/GetContractBranchName"); // Note : in Contract/Controllers/ContractData.cs/GetContractBranchName()


    //    InitialTrimTextEvent([
    //        "CustomerName",
    //        "BranchName",
    //        "Address",
    //        "Alley",
    //        "Road",
    //        "SubDistrict",
    //        "ZipCode"
    //    ]);


});


/* --- Methods --- */
function initialPage() {
    $("#divSearchResult").hide();

}

function CMS310_Search() {

    // disable search button
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    var parameter = CreateObjectData($("#searchCondition").serialize() + "&Counter=" + CMS310_Count ,true);

    $("#mygrid_container").LoadDataToGrid(mygrid_cms310, CMS310_RowPerPage, true, "/Common/CMS310_SearchResponse", parameter, "View_dtContractData", false,
        function (result, controls, isWarning) {
            // enable search button
            $("#btnSearch").attr("disabled", false);
            master_event.LockWindow(false);

            if (result != undefined) {
                //document.getElementById('divSearchResult').scrollIntoView();
                master_event.ScrollWindow("#divSearchResult", true);
            }
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
//        var cond = $(this).val(); //  + String.fromCharCode(e.which);
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
//function txtBranchName_keyup(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val(); // + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#divSearchAddrCtrl input[id=BranchName]",
//                                cond,
//                                "/Contract/GetContractBranchName",  // Note : in Contract/Controllers/ContractData.cs/GetContractBranchName()
//                                {"cond": cond },
//                                "dtContractBranchName",
//                                "BranchName",
//                                "BranchName");
//    }
//}

/*---- Event ------*/
/* -----------  event ----------- */
// ind = colum index
function doSelect(row_id) {


    mygrid_cms310.selectRow(mygrid_cms310.getRowIndex(row_id));

    /* ==== Create json object for string json ==== */
    var strJson = mygrid_cms310.cells2(mygrid_cms310.getRowIndex(row_id), mygrid_cms310.getColIndexById('Object')).getValue().toString();
    strJson = htmlDecode(strJson);
    var objSelcted = JSON.parse(strJson); // require json2.js

    //alert(strJson);

    if (typeof (CMS310Response) == "function") {
        CMS310Response(objSelcted);
    }



}

function CMS310Initial() {
    ChangeDialogButtonText(["Cancel"], [$('#btnCancel_CMS310').val()]);
}


