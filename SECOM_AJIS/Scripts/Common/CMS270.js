/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>

/* --- Variables --- */
var mygrid_cms270;
var pageRow_cms270 = ROWS_PER_PAGE_FOR_SEARCHPAGE;

var CMS270_Count = 0;

/* -------- Main -------- */
$(document).ready(function () {

    /* ----- Initial page -----*/
    initialPage();

    /* --- Initial grid --- */
    if ($.find("#mygrid_container").length > 0) {

        mygrid_cms270 = $("#mygrid_container").InitialGrid(pageRow_cms270, true, "/Common/CMS270_InitialGrid");

        /* ===== binding on row select===== */
        //mygrid_cms270.attachEvent("onRowSelect", doSelect);

        /*=========== Set hidden column =============*/
        var colHiddenIndex = mygrid_cms270.getColIndexById("Object");
        mygrid_cms270.setColumnHidden(colHiddenIndex, true);

    }

    SpecialGridControl(mygrid_cms270, ["Button"]);

    /* ===== binding event when finish load data ===== */
    BindOnLoadedEvent(mygrid_cms270, function (gen_ctrl) {
        var colInx = mygrid_cms270.getColIndexById('Button');
        for (var i = 0; i < mygrid_cms270.getRowsNum(); i++) {

            var row_id = mygrid_cms270.getRowId(i);

            if (gen_ctrl == true) {
                GenerateSelectButton(mygrid_cms270, "btnSelect", row_id, "Button", true);
            }
            // binding grid button event 
            BindGridButtonClickEvent("btnSelect", row_id, doSelect);
        }

        mygrid_cms270.setSizes();

    });

    /* ===== binding event search button ===== */
    $("#btnSearch").click(function () {

        CMS270_Search();

    });



    /*==== event Customer Name keypress ====*/
    //$("#divSearchCtrl input[id=BillingClientName]").keyup(txtBillingClientName_keyup);
    $("#divSearchCtrl input[id=BillingClientName]").InitialAutoComplete("/Master/GetBillingClientName"); // Note : in Master/Controllers/BillingClientData.cs/GetBillingClientName()


    /*==== event Site Name keypress ====*/
    //$("#divSearchCtrl input[id=Address]").keyup(txtAddress_keyup);
    $("#divSearchCtrl input[id=Address]").InitialAutoComplete("/Master/GetBillingClientAddress"); // Note :  Master/Controllers/BillingClientData.cs/GetBillingClientName()



    /*==== event chkJuristic checked change ====*/
    $('#chkJuristic').change(function () {

        // if chkJuristic is check then enable cboCompanyType

        if ($('#chkJuristic').is(':checked')) {
            $('#CompanyTypeCode').attr('disabled', false);
        } else {
            $('#CompanyTypeCode').attr('disabled', true);
            $('#CompanyTypeCode').val('');
        }
    });


    /* =========== event Clear button ===========*/
    $("#btnClear").click(function () {
        $("#searchCondition").clearForm();
        initialPage();

        CMS270_Count = 0;
        CloseWarningDialog();
    });

    //    InitialTrimTextEvent([
    //        "BillingClientCode",
    //        "BillingClientName",
    //        "Address",
    //        "TelephoneNo"
    //    ]);


});

/* --- Methods --- */
function initialPage() {
    
    $("#divSearchResult").hide();

    $("#chkJuristic").attr("checked", true);
    $('#CompanyTypeCode').attr('disabled', false);
    $("#chkIndividual").attr("checked", true);
    $("#chkOther").attr("checked", true);
}

function CMS270_Search() {

    // disable search button
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    var data = $("#searchCondition").serialize();
    //alert(data);

    var parameter = CreateObjectData($("#searchCondition").serialize() + "&Counter=" + CMS270_Count ,true);
    $("#mygrid_container").LoadDataToGrid(mygrid_cms270, pageRow_cms270, true, "/Common/CMS270_SearchResponse", parameter, "View_dtBillingClientData", false,
        function (result, controls, isWarning) {
            // enable search button
            $("#btnSearch").attr("disabled", false);
            master_event.LockWindow(false);

            if (result != undefined) {
                master_event.ScrollWindow("#divSearchResult", true);
            }
        },
        function (result, controls, isWarning) {
            if (isWarning == undefined) {
                $("#divSearchResult").show();
            }
        });

}

/*---- Event txt BillingClientName keypress------*/

//function txtBillingClientName_keyup(e) {

//    if ($(this).val().length + 1 >= 3) {

//        var cond = $(this).val();  // + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#divSearchCtrl input[id=BillingClientName]",
//                                cond,
//                                "/Master/GetBillingClientName", // Note : in Master/Controllers/BillingClientData.cs/GetBillingClientName()
//                                {"cond": cond },
//                                "dtBillingClientName",
//                                "BillingClientName",
//                                "BillingClientName");

//    }
//}

/*---- Event txt Address keypress ------*/
//function txtAddress_keyup(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val(); // + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#divSearchCtrl input[id=Address]",
//                                cond,
//                                "/Master/GetBillingClientAddress", // Note :  Master/Controllers/BillingClientData.cs/GetBillingClientName()
//                                {"cond": cond },
//                                "dtBillingClientAddress",
//                                "Address",
//                                "Address");
//    }
//}

/*------- Event ---------*/
// ind = colum index
function doSelect(row_id) {


    mygrid_cms270.selectRow(mygrid_cms270.getRowIndex(row_id));

    /* ==== Create json object for string json ==== */
    var strJson = mygrid_cms270.cells2(mygrid_cms270.getRowIndex(row_id), mygrid_cms270.getColIndexById('Object')).getValue().toString();
    strJson = htmlDecode(strJson);
    var objSelcted = JSON.parse(strJson); // require json2.js

    //alert(strJson);

    if (typeof (CMS270Response) == "function") {
        CMS270Response(objSelcted);
    }



}


function CMS270Initial() {
    ChangeDialogButtonText(["Cancel"], [$('#btnCancel_CMS270').val()]);
}


