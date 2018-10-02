/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>
/// <reference path="../Base/object/ajax_method.js" />
/// <reference path="../Base/object/master_event.js" />

/// <reference path="../../Scripts/json2.js"/>

/* -------------- variable -------------- */

var mygrid;
var dtDataprocess;
var pageRow = 100;

// Main
$(document).ready(function () {
    // Do somethinge

    /*---- Load batch list*/
    loadBatchList();


    $("#btnRunBatchAll").click(
        function () {

            // OpenYesNoMessageDialog
            var message;
            var param = { "module": "Common", "code": "MSG0059" };
            call_ajax_method("/Shared/GetMessage", param, function (data) {

                /* ====== Open confirm dialog =====*/
                OpenYesNoMessageDialog(data.Code, data.Message, doRunBatchAll);

            });

        }
     );


    // refresh grid every 5 sec.
    // var refreshId = setInterval(reload, 5000);

    // refresh grid every 10 sec.
       var refreshId = setInterval(reload, 10000);

});

/*------------ method ----------*/
function loadBatchList() {
    if ($.find("#mygrid_container").length > 0) {


        //$("#mygrid_container").LoadDataToGrid(mygrid, pageRow, true, "/Common/CMS050_SearchResponse", "", "View_dtBatchProcess", false);
        mygrid = $("#mygrid_container").LoadDataToGridWithInitial(pageRow, true, true, "/Common/CMS050_SearchResponse", "", "View_dtBatchProcess", false);


        /* ===== binding on row select===== */
        //mygrid.attachEvent("onRowSelect", doRunBatch);

        /*=========== Set hidden column =============*/
        var colHiddenIndex = mygrid.getColIndexById("Object");
        mygrid.setColumnHidden(colHiddenIndex, true);


        SpecialGridControl(mygrid, ["Button"]);

        /* ===== binding event when finish load data ===== */
        BindOnLoadedEvent(mygrid, function (gen_ctrl) {

            var isEnableRunBatchAllButton = false;

            dtDataprocess = new Array();
            var colInx = mygrid.getColIndexById('Button');
            for (var i = 0; i < mygrid.getRowsNum(); i++) {

                var row_id = mygrid.getRowId(i);

                if (gen_ctrl == true) {
                    /* --- Disable/enable "Run" button --- */
                    var bEnableRun = mygrid.cells2(i, mygrid.getColIndexById('EnableRun')).getValue();

                    if ($.trim(bEnableRun) == '1') { // enable
                        GenerateRunningButton(mygrid, "btnRun", row_id, "Button", true);

                        if (isEnableRunBatchAllButton == false) {
                            isEnableRunBatchAllButton = true;
                        }
                    }
                    else { // disable
                        GenerateRunningButton(mygrid, "btnRun", row_id, "Button", false);
                    }
                }

                // Add grid event button
                BindGridButtonClickEvent("btnRun", row_id, doRunBatch);

                /*Keep data table*/
                //var strObject = mygrid.cells2(i, mygrid.getColIndexById('Object')).getValue();
                //var dataRow = JSON.parse(strObject);
                //dtDataprocess.push(dataRow);




            }

            // Enable/Disable RunBatchAllButton
            $("#btnRunBatchAll").attr("disabled", !isEnableRunBatchAllButton);

            var colObjectInx = mygrid.getColIndexById('Object');
            mygrid.setColumnHidden(colObjectInx, true);

            mygrid.setSizes();

        });


    }
}

/*--------- event ---------*/
function doRunBatchAll() {


    call_ajax_method("/Common/CMS050_RunbatchAll", "", reload);

    //$.ajax({
    //    type: 'POST',
    //    dataType: "json",
    //    url: generate_url("/Common/CMS050_Runbatch"),
    //    contentType: 'application/json ',
    //    data: strDataJson,
    //    success: function (data) {

    //        // alert
    //        alert("call back");

    //        // get current status after update
    //        //$("#mygrid_container").LoadDataToGrid(mygrid, pageRow, true, "/Common/CMS050_SearchResponse", "", "View_dtBatchProcess", false);
    //    }
    //});

    return false;

}

/* ------------ event when click run button -------------*/
function doRunBatch(row_id) {
    /// id - id of the clicked row;
    /// ind - index of the clicked cell (column index)

    mygrid.selectRow(mygrid.getRowIndex(row_id));

    var selectedRowInx = mygrid.getRowIndex(row_id);
    var bEnableRun = $.trim(mygrid.cells2(selectedRowInx, mygrid.getColIndexById('EnableRun')).getValue());


    if (bEnableRun == 1) {

        // refresh data
        $("#mygrid_container").LoadDataToGrid(mygrid, pageRow, true, "/Common/CMS050_SearchResponse", "", "View_dtBatchProcess", false);

        var strSlectedObject = mygrid.cells2(selectedRowInx, mygrid.getColIndexById('Object')).getValue();
        strSlectedObject = htmlDecode(strSlectedObject);
        var strBatchName = mygrid.cells2(selectedRowInx, mygrid.getColIndexById('BatchName')).getValue();

        // OpenYesNoMessageDialog
        var message;
        var param = { "module": "Common", "code": "MSG0060", "param": strBatchName };
        call_ajax_method("/Shared/GetMessage", param, function (data) {

            /* ====== Open confirm dialog =====*/
            OpenYesNoMessageDialog(data.Code, data.Message, function () {



                var dataSelectRow = JSON.parse(strSlectedObject);
                dataSelectRow["isRunAll"] = false; // real-> false

                call_ajax_method_json("/Common/CMS050_Runbatch", dataSelectRow, reload);


                //// Run 1 batch (isRunAll = false)
                //dtDataprocess = new Array();
                //var dataSelectRow = JSON.parse(strSlectedObject);
                //dtDataprocess.push(dataSelectRow);
                //var dataJson = { "doBatchProcess": dtDataprocess, "isRunAll": false };
                //var strDataJson = JSON.stringify(dataJson);
                //cms050_call_ajax_method("/Common/CMS050_Runbatch", strDataJson, reload, "json", 'application/json');


            });

        });



        return false;

    }


}

// for support model as dataset
function cms050_call_ajax_method(url, obj, func, strDataType, strContentType) {
    /// <summary>Method to call back to server by ajax (support model as dataset)</summary>
    /// <param name="url" type="string">Controller Path</param>
    /// <param name="obj" type="string">Input Parameters</param>
    /// <param name="func" type="string">Function when can get result from server</param>
    /// <param name="strDataType" type="string">Data type ex: 'json' </param>
    /// <param name="strContentType" type="string">Content type ex: 'application/json' </param>

    $.ajax({
        type: "POST",
        dataType: strDataType,
        url: ajax_method.GenerateURL(url),
        contentType: strContentType,
        data: obj,
        success: function (result) {
            ajax_method.OnAjaxSendSuccess(result, func);
        },
        error: ajax_method.OnAjaxSendError
    });

}


function reload(data) {
    //alert(data + " : call back: run batch aleady !");
    $("#mygrid_container").LoadDataToGrid(mygrid, pageRow, true, "/Common/CMS050_SearchResponse", "", "View_dtBatchProcess", false);

    return false;
}
