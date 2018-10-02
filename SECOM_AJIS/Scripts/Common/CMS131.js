/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>

/// <reference path="../../Scripts/Common/Dialog.js"/>

/* --- Variables --- */
var mygrid;

/*-------- Main ---------*/
$(document).ready(function () {


    /* --- Initial page --- */
    initalPage();

    /*-- Initial grid-- */
    var pageRow = ROWS_PER_PAGE_FOR_VIEWPAGE;

    //mygrid = $("#mygrid_container").InitialGrid(pageRow, true, "/Common/CMS131_InitialGrid");

    if ($.find("#mygrid_container").length > 0) {

        var obj = CMS131Object();
        var parameter = { "strContractCode": obj.ContractCode, "strOCC": obj.OCC };
        //$("#mygrid_container").LoadDataToGrid(mygrid, pageRow, true, "/Common/CMS131_SearchResponse", parameter, "View_dtContractDocument", false);

        mygrid = $("#mygrid_container").LoadDataToGridWithInitial(pageRow, true, true, "/Common/CMS131_SearchResponse", parameter, "View_dtContractDocument", false);
    }


    // Set null value to "-"
    $("#divAll_CMS131").SetEmptyViewData();

});


/* --- Methods --- */
function initalPage() {

}

function CMS131Initial() {
    ChangeDialogButtonText(["Close"], [$('#btnClose').val()]);
}

