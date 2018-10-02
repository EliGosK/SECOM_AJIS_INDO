/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

/*--- Main ---*/
var pageRow = ROWS_PER_PAGE_FOR_SEARCHPAGE;
var gridDocumentListCTS210;
var cFlagOff;

$(document).ready(function () {
    InitialEvent();
    SetInitialState();
});

function InitialEvent() {
    $("#btnRetrieve").click(retrieve_button_click);
    $("#btnClear").click(clear_button_click);
}

function SetInitialState() {
    $("#divSpecifyContractCode").clearForm();
    $("#divContractInfo").clearForm();

    $("#txtContractQuotTgtCode").attr("readonly", false);
    $("#txtOccAlphabet").attr("readonly", false);
    $("#btnRetrieve").attr("disabled", false);
    //$("#btnClear").attr("disabled", true);

    $("#divContractInfo").hide();
    $("#divDocumentList").hide();

    gridDocumentListCTS210 = $("#gridDocumentList").InitialGrid(pageRow, false, "/Contract/CTS210_InitialGrid",
        function () {
            SpecialGridControl(gridDocumentListCTS210, ["Download"]);

            BindOnLoadedEvent(gridDocumentListCTS210,
                function (gen_ctrl) {
                    gridDocumentListCTS210.setColumnHidden(gridDocumentListCTS210.getColIndexById("IsEnableDownload"), true);
                    gridDocumentListCTS210.setColumnHidden(gridDocumentListCTS210.getColIndexById("DocID"), true);

                    for (var i = 0; i < gridDocumentListCTS210.getRowsNum(); i++) {

                        var isEnableColinx = gridDocumentListCTS210.getColIndexById("IsEnableDownload");
                        var row_id = gridDocumentListCTS210.getRowId(i);

                        if (gen_ctrl == true) {
                            var isEnable = gridDocumentListCTS210.cells(row_id, isEnableColinx).getValue();
                            GenerateDownloadButton(gridDocumentListCTS210, "btnDownload", row_id, "Download", isEnable);
                        }

                        /* --- Set Event --- */
                        /* ----------------- */
                        BindGridButtonClickEvent("btnDownload", row_id,
                            function (row_id) {
                                DownloadDocumentReport(row_id);
                            }
                        );


//                        var strGenerateDate = gridDocumentListCTS210.cells2(i, gridDocumentListCTS210.getColIndexById('GenerateDate')).getValue();
//                        strGenerateDate = htmlDecode(strGenerateDate); //htmlDecode

//                        gridDocumentListCTS210.cells2(i, gridDocumentListCTS210.getColIndexById('GenerateDate')).setValue(strGenerateDate);


                        /* ----------------- */
                    }

                    gridDocumentListCTS210.setSizes();
                }
            );

        });
}

function retrieve_button_click() {
    $("#btnRetrieve").attr("disabled", true);
    master_event.LockWindow(true);

    var objCond = {
        ContractCode: $("#txtContractQuotTgtCode").val(),
        OCC: $("#txtOccAlphabet").val()
    }

    $("#gridDocumentList").LoadDataToGrid(gridDocumentListCTS210, pageRow, false, "/Contract/CTS210_GetDocumentListData", objCond, "CTS210_DocumentListGridData", false,
        function (result, controls, isWarning) {
            if (controls != undefined) {
                /* --- Highlight Text --- */
                /* --------------------- */
                VaridateCtrl(["txtContractQuotTgtCode", "txtOccAlphabet"], controls);

                master_event.LockWindow(false);
                $("#btnRetrieve").attr("disabled", false);
                return;
            }
            else if (isWarning == undefined) {

                if (CheckFirstRowIsEmpty(gridDocumentListCTS210, false) == false) {
                    $("#divDocumentList").show();
                    //document.getElementById('divDocumentList').scrollIntoView();
                    master_event.ScrollWindow("#divDocumentList", false);

                    $("#txtContractQuotTgtCode").attr("readonly", true);
                    $("#txtOccAlphabet").attr("readonly", true);
                    $("#btnRetrieve").attr("disabled", true);
                    //$("#btnClear").attr("disabled", false);

                    call_ajax_method_json("/Contract/CTS210_GetContractInfo", "",
                        function (result) {
                            if (result != undefined) {
                                $("#divContractInfo").bindJSON(result);
                                $("#divContractInfo").show();
                            }
                            else {
                                $("#divContractInfo").hide();
                            }

                            master_event.LockWindow(false);
                        }
                    );
                }
                else {
                    $("#divDocumentList").hide();
                    $("#divContractInfo").hide();
                    $("#btnRetrieve").attr("disabled", false);

                    master_event.LockWindow(false);
                }
            }
            else {
                master_event.LockWindow(false);
            }
        }
        , function () {
            $("#divDocumentList").show();
        });

//    SpecialGridControl(gridDocumentListCTS210, ["Download"]);

//    BindOnLoadedEvent(gridDocumentListCTS210,
//        function () {
//            gridDocumentListCTS210.setColumnHidden(gridDocumentListCTS210.getColIndexById("IsEnableDownload"), true);
//            gridDocumentListCTS210.setColumnHidden(gridDocumentListCTS210.getColIndexById("DocID"), true);

//            for (var i = 0; i < gridDocumentListCTS210.getRowsNum(); i++) {

//                var isEnableColinx = gridDocumentListCTS210.getColIndexById("IsEnableDownload");
//                var row_id = gridDocumentListCTS210.getRowId(i);

//                var isEnable = gridDocumentListCTS210.cells(row_id, isEnableColinx).getValue();
//                GenerateDownloadButton(gridDocumentListCTS210, "btnDownload", row_id, "Download", isEnable);

//                /* --- Set Event --- */
//                /* ----------------- */
//                BindGridButtonClickEvent("btnDownload", row_id,
//                    function (row_id) {
//                        DownloadDocumentReport(row_id);
//                    }
//                );


//                var strGenerateDate = gridDocumentListCTS210.cells2(i, gridDocumentListCTS210.getColIndexById('GenerateDate')).getValue();
//                strGenerateDate = htmlDecode(strGenerateDate); //htmlDecode

//                gridDocumentListCTS210.cells2(i, gridDocumentListCTS210.getColIndexById('GenerateDate')).setValue(strGenerateDate);


//                /* ----------------- */
//            }

//            gridDocumentListCTS210.setSizes();
//        }
//    );

    gridDocumentListCTS210.setSizes();
}

function DownloadDocumentReport(row_id) {
    gridDocumentListCTS210.selectRow(gridDocumentListCTS210.getRowIndex(row_id));

    var docIDCol = gridDocumentListCTS210.getColIndexById("DocID");
    var DocID = gridDocumentListCTS210.cells(row_id, docIDCol).getValue();

    var obj = { iDocID: DocID };
    call_ajax_method_json("/Contract/CTS210_DownloadDocumentReport", obj,
        function (result) {
            if (result != undefined) {
                if (result) {
                    $("#gridDocumentList").LoadDataToGrid(gridDocumentListCTS210, pageRow, false, "/Contract/CTS210_RefreshDocumentReport", obj, "CTS210_DocumentListGridData", false, null, null);
                }

                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Contract/CTS210_OpenContractDocument?k=" + key);
                //window.open(url, "ContractDocumentReport", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
                window.open(url, "", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        }
    );

}

function clear_button_click() {
    CloseWarningDialog();
    SetInitialState();
}

