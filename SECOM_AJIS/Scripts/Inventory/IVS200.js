// ---------------------------------------------------------------------------------
// Initial
// ---------------------------------------------------------------------------------
var gridInstrument = null;
var initData = null;
var IVS200_OfficeCode = null;
var gridInstrumentDetail = null;
$(document).ready(function () {
    InitialPage();
    InitialGrid();
 
});

function InitialPage() {
    SetScreenToDefault();
    RetrieveInitialData();

    $('#btnSearch').click(btnSearch_click);
    $('#btnClear').click(btnClear_click);

    //$("#InstrumentCode").InitialAutoComplete("/Master/GetInstrumentCode"); //Add by Jutarat A. on 25032014
    $("#InstrumentCode").InitialAutoComplete("/Master/GetInstrumentCodeAll", null, true);
}

function InitialGrid() {
    gridInstrument = $("#gridInstrumentQuantityList").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Inventory/IVS200_InitialInstrumentQuantityGrid");
    BindOnLoadedEvent(gridInstrument, gridInstrument_binding);

}


// ---------------------------------------------------------------------------------
// Event
// ---------------------------------------------------------------------------------
var listArray = [];
function gridInstrument_binding() {
    //gridInstrument.addRow("", "");
    var idxEffectCol = gridInstrument.getColIndexById('EffectiveQty');
    var idxSafetyCol = gridInstrument.getColIndexById('SafetyQty');
    var idxInstrumentCode = gridInstrument.getColIndexById('InstrumentCode');

    for (var i = 0; i < gridInstrument.getRowsNum(); i++) {
        var row_id = gridInstrument.getRowId(i);
        //gridInstrument.setColspan(row_id, idxInstrumentCode, 7);
        var effectQty = gridInstrument.cells(row_id, idxEffectCol).getValue();
        var safetyQty = gridInstrument.cells(row_id, idxSafetyCol).getValue();
        var rowDetailID = gridInstrument.cells(row_id, idxInstrumentCode).getValue();

        if (effectQty < safetyQty) {
            gridInstrument.setCellTextStyle(row_id, idxEffectCol, "color: red;");
        }

        GenerateDetailButton(gridInstrument, "btnBtnEdit", row_id, "BtnEdit", true);
        //add onclick
        BindGridButtonClickEvent2("btnBtnEdit", row_id, function (rid, sender) {
            var tr = sender.parent().parent();
            var d = gridInstrument.cells(rid, idxInstrumentCode).getValue();

            var datacheck = $("#" + rid + "").val();
            if (datacheck == undefined) {

                $("<tr id='" + rid + "'><td colspan='8'><div id='Detail_" + rid + "' style='padding:10px;'></div></td></tr>").insertAfter(tr);
                var divID = "#Detail_" + rid;
                var divTrID = "#" + rid;
                
                var parameter = CreateObjectData($(divID).serialize());
                parameter.InstrumentCode = d;

                master_event.LockWindow(true);
                gridInstrumentDetail = $(divID).InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Inventory/IVS200_InitialInstrumentQuantityGridDetail",
                 function () {
                     $(divID).LoadDataToGrid(gridInstrumentDetail, ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/Inventory/IVS200_RetrieveInstrumentQuantityListDetail", parameter, "doInventoryBookingDetail", false,
                         function () {

                             master_event.LockWindow(false);
                             gridInstrument.setSizes();
                             master_event.ScrollWindow(divTrID);

                         });
                 });


            }
            else {
                if ($("#" + rid + "").css('display') == 'none') {
                    $("#" + rid + "").css('display', 'block');
                    gridInstrument.setSizes();
                }
                else {
                    $("#" + rid + "").css('display', 'none');
                    gridInstrument.setSizes();
                }

            }


        });

  
    }
 



}

function test() {
    alert();
}

function btnSearch_click() {
    var obj = CreateObjectData($('#formSearchCondition').serialize());

    obj.HaveOrder = $('#HaveOrder').prop('checked');
    obj.BelowSafety = $('#BelowSafety').prop('checked');
    obj.Minus = $('#Minus').prop('checked');
    obj.OfficeCode = IVS200_OfficeCode;

    $('#btnSearch').SetDisabled(true);
    master_event.LockWindow(true);

   var loadnewgridtest = $("#gridInstrumentQuantityList").LoadDataToGrid(gridInstrument, ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/Inventory/IVS200_RetrieveInstrumentQuantityList", obj, "doResultIVS200", false,
    function () {
        $('#btnSearch').SetDisabled(false);
        master_event.LockWindow(false);
        //$('#divInstrumentQuantityList').each(function () {
        //    this.scrollIntoView();
        //});
        master_event.ScrollWindow("#divInstrumentQuantityList");
        //$('#divInstrumentQuantityList').show(); //Comment by Non A. 5/Apr/2012 : to always show the list by SA request.
    })
}

function btnClear_click() {
    SetScreenToDefault();
}

// ---------------------------------------------------------------------------------
// Method
// ---------------------------------------------------------------------------------
function SetScreenToDefault() {
    var oldInsArea = $('#InstrumentArea').val();
    $('#divSearchCriteria').clearForm();
    $('#divInstrumentQuantityList').clearForm();
    //$('#divInstrumentQuantityList').hide(); //Comment by Non A. 5/Apr/2012 : to always show the list by SA request.

    if (gridInstrument != null) {
        DeleteAllRow(gridInstrument);
    }

    if (initData != null) {
        $('#divSearchCriteria').bindJSON(initData);
    }

    $('#InstrumentArea').val(oldInsArea);
}

function RetrieveInitialData() {
    var oldInsArea = $('#InstrumentArea').val();
    call_ajax_method_json("/Inventory/IVS200_RetrieveInitialData", "", function (result, controls) {
        if (result != null) {
            result.Office = result.OfficeName;
            IVS200_OfficeCode = result.OfficeCode;
            initData = result;
            $('#divSearchCriteria').bindJSON(initData);
            $('#InstrumentArea').val(oldInsArea);
        }
    });
}