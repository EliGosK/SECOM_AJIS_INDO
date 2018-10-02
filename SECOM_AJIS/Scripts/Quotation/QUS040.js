/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>

/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>
/// <reference path="../../Scripts/Base/AutoComplete.js"/>
/// <reference path="../jquery-1.5.1-vsdoc.js" />
/// <reference path="../Base/GridControl.js" />

function QUS040Initial() {
    ChangeDialogButtonText(["Close"], [$('#btnClose').val()]);
}

function GridInnitial() {
    if ($("#ResultListSection").length > 0) {

        mygrid = $("#ResultListSection").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Quotation/InitialGrid_QUS040");

        SpecialGridControl(mygrid, ["Select"]);
        BindOnLoadedEvent(mygrid, function (gen_ctrl) {

            for (var i = 0; i < mygrid.getRowsNum(); i++) {
                var row_id = mygrid.getRowId(i);

                if (gen_ctrl == true) {
                    GenerateSelectButton(mygrid, "btnSelect", row_id, "Select", true);
                }

                BindGridButtonClickEvent("btnSelect", row_id, function (rid) {
                    var col = mygrid.getColIndexById('QuotationTargetCode_short');
                    var objQUS040 = mygrid.cells(rid, col).getValue();
                    if (typeof (QUS040Response) == "function")
                        QUS040Response(objQUS040);
                });
            }
        });
    }
}

function Search() {    //------------------------------------------- Search() -------------------------------------------

    VaridateCtrl(["QuotationTargetCode", "QuotationDateFrom", "QuotationDateTo"], null);
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    $("#ResultListSection").LoadDataToGrid(mygrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Quotation/QUS040_XML", CreateObjectData($("#searchQuotationForm").serialize()), "View_dtSearchQuotationTargetListlResult", false,
    function (result, controls, isWarning) {
        $("#btnSearch").attr("disabled", false);
        master_event.LockWindow(false);

        if (controls != undefined) {

            VaridateCtrl(["QuotationTargetCode", "QuotationDateFrom", "QuotationDateTo"], controls);
        }
        else if (result != undefined) {
            master_event.ScrollWindow("#ResultSection", true);
        }
        
    },
    function (result, controls, isWarning) {

        if (isWarning == undefined)
            $("#ResultSection").show();
    });

}

function init() {
    CloseWarningDialog();
    $("input.highlight").removeClass('highlight');
    $("select.highlight").removeClass('highlight');
    ClearDateFromToControl("#QuotationDateFrom", "#QuotationDateTo");
    $("#searchQuotationForm input[type='text']").val("");
    $("#QuotationOfficeCode option:first").attr('selected', 'selected');
    $("#OperationOfficeCode option:first").attr('selected', 'selected');
    $("#ProductTypeCode option:first").attr('selected', 'selected');
    $("#ResultSection").hide();
}

$(document).ready(function () {
    init();
    GridInnitial();
    if ($("#QuotationDateFrom").length > 0) {
        InitialDateFromToControl("#QuotationDateFrom", "#QuotationDateTo");
    }
    $("#btnSearch").click(Search);
    $("#btnClear").click(init);


    InitialTrimTextEvent([
        "QuotationTargetCode",
        "ContractTargetCode",
        "ContractTargetName",
        "ContractTargetAddr",
        "SiteCode",
        "SiteName",
        "SiteAddr",
        "StaffCode",
        "StaffName"
    ]);

});