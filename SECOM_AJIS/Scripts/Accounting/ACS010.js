/// <reference path="../../Scripts/Base/DateTimePicker.js"/>
jQuery(function () {
    var pageRow = ROWS_PER_PAGE_FOR_SEARCHPAGE;
    var mygrid;

    $(document).ready(function () {


    // --- Date Picker ---
        InitialDateFromToControl("#SearchTargetFrom", "#SearchTargetTo");
        InitialDateFromToControl("#SearchGenerateFrom", "#SearchGenerateTo");
        InitialDateFromToControl("#GenerateTargetFrom", "#GenerateTargetTo");

        initialPage();

        $('#accoountingDocumentReport').change(function () {
            var selectedDocumentCode = this.value;
            if (selectedDocumentCode == "") {
                initialPage();
                return;
            }
            var obj = {
                documentCode: selectedDocumentCode
            };
            call_ajax_method("/Accounting/GetDocumentTimingName", obj, function (data) {
                $("#documentTimingType").text(data.TimingTypeName);
                var currentDate = ConvertDateObject(new Date());
                if (data.TimingType == 'D2') {
                    showControls_GenerateTargetFromTo(true,false, true);
                    SetDateFromToData("#GenerateTargetFrom", "#GenerateTargetTo", currentDate, currentDate);
                } else if (data.TimingType == 'MM') {
                    showControls_GenerateTargetFromTo(false,null, true);
                    SetDateFromToData("#GenerateTargetFrom", "#GenerateTargetTo", null, currentDate);
                } else if (data.TimingType == 'DT') {
                    showControls_GenerateTargetFromTo(false,null, false);
                    SetDateFromToData("#GenerateTargetFrom", "#GenerateTargetTo", null, currentDate);
                } else {
                    showControls_GenerateTargetFromTo(true,true, true);
                    SetDateFromToData("#GenerateTargetFrom", "#GenerateTargetTo", currentDate, currentDate);
                }
                showCriteriaSections();
            });

        });
        $("#btnRun").click(function () {
            $("#accoountingDocumentReport").attr("disabled", true);
            $("#btnSearch").attr("disabled", true);
            $("#btnRun").attr("disabled", true);
            master_event.LockWindow(true);
            generateReport();

        });

        /*==== event btnSearch click ====*/
        $("#btnSearch").click(function () {
            $("#accoountingDocumentReport").attr("disabled", true);
            $("#btnSearch").attr("disabled", true);
            $("#btnRun").attr("disabled", true);
            master_event.LockWindow(true);


            searchReport();
        });

    });

    var showCriteriaSections = function () {
        $('#Generate_Criteria').show();
        $('#Search_Criteria').show();
    }

    var hideCriteriaSections = function () {
        $('#Generate_Criteria').hide();
        $('#Search_Criteria').hide();
    }

    // GenerateTargetFrom,GenerateTargetTo
    function showControls_GenerateTargetFromTo(isFromShow, isFromReadOnly, isToReadOnly) {
        if (isFromShow)
        {
            $("#GenerateTargetPeriodFrom").show();
            $("#GenerateTargetTo").show();
            if (isFromReadOnly) {
                $("#GenerateTargetFrom").EnableDatePicker(false);
            }
            else {
                $("#GenerateTargetFrom").EnableDatePicker(true);
            }
        }
        else
        {
            $("#GenerateTargetPeriodFrom").hide();
            $("#GenerateTargetTo").show();
        }
        if (isToReadOnly) {
            $("#GenerateTargetTo").EnableDatePicker(false);
        }
        else {
            $("#GenerateTargetTo").EnableDatePicker(true);
        }
    }

    function initialPage() {

        hideCriteriaSections();
        $("#divSearchResult").hide();

    }
    function generateReportBK() {

        var selectedDocumentCode = $('#accoountingDocumentReport').val();
        var obj = {
            documentCode: selectedDocumentCode
            , generateTargetFrom: $("#GenerateTargetFrom").val()
            , generateTargetTo: $("#GenerateTargetTo").val()
            
        };
        call_ajax_method(
            '/Accounting/ACS010_GenerateReport',
            obj,
            function (result, controls) {
                if (controls != undefined) {
                    VaridateCtrl(["GenerateTargetTo"], controls);
                    $("#GenerateTargetTo").focus();
                } else {

                }
                $("#accoountingDocumentReport").attr("disabled", false);
                $("#btnSearch").attr("disabled", false);
                $("#btnRun").attr("disabled", false);
                master_event.LockWindow(false);

                return;
            }
        );
    }
    function generateReport() {

        var selectedDocumentCode = $('#accoountingDocumentReport').val();
        var obj = {
            documentCode: selectedDocumentCode
            , generateTargetFrom: $("#GenerateTargetFrom").val()
            , generateTargetTo: $("#GenerateTargetTo").val()

        };

        mygrid = CreateGridControl("mygrid_container", pageRow, true);
        call_ajax_method_json(
            '/Accounting/InitialGrid_ACS010', "", function (result) {
                mygrid.xml.top = "response";
                mygrid.xml.row = "./rows/object";
                mygrid.parse(result);

                SpecialGridControl(mygrid, ["Button"]);
                /* ===== binding event when finish load data ===== */
                BindOnLoadedEvent(mygrid, function (gen_ctrl) {

                    for (var i = 0; i < mygrid.getRowsNum(); i++) {

                        var row_id = mygrid.getRowId(i);

                        if (gen_ctrl == true) {
                            GenerateDownloadButton(mygrid, "btnDownload", row_id, "Button", true);
                        }

                        BindGridButtonClickEvent("btnDownload", row_id, doDownload);

                        var DocumentNoIndex = mygrid.getColIndexById("DocumentNo");

                        /* Show/Hide column depend of Document type and Document name */
                        if (i == mygrid.getRowsNum() - 1) {
                            mygrid.setSizes();
                        }
                    }
                });

                $("#mygrid_container").LoadDataToGrid(mygrid, pageRow, true, "/Accounting/ACS010_GenerateReport", obj, "dtAccountingDocumentList", false, "",
                                    function (result, controls, isWarning) {
                                        if (controls != undefined) {
                                            VaridateCtrl(["GenerateTargetTo"], controls);
                                            $("#GenerateTargetTo").focus();
                                        }
                                        if (isWarning == undefined) {
                                            $("#divSearchResult").show();


                                            return;
                                        }
                                    });
                $("#accoountingDocumentReport").attr("disabled", false);
                $("#btnSearch").attr("disabled", false);
                $("#btnRun").attr("disabled", false);
                master_event.LockWindow(false);
            });
    }
    function searchReport() {
        var selectedDocumentCode = $('#accoountingDocumentReport').val();
        var obj = {
            SearchDocumentCode: selectedDocumentCode
            , SearchTargetFrom: $("#SearchTargetFrom").val()
            , SearchTargetTo: $("#SearchTargetTo").val()
            , SearchGenerateFrom: $("#SearchGenerateFrom").val()
            , SearchGenerateTo: $("#SearchGenerateTo").val()
            , SearchMonth: $("#SearchMonth").val()
            , SearchYear: $("#SearchYear").val()
            , SearchDocumentNo: $("#SearchDocumentNo").val()
        };

        call_ajax_method(
            '/Accounting/ACS010_CheckSearchReqField',
            obj,
            function (result, controls) {
                if (controls != undefined) {
                    VaridateCtrl(["SearchTargetTo"], controls);
                    $("#SearchTargetTo").focus();
                }
               // if (result == true) {

                    mygrid = CreateGridControl("mygrid_container", pageRow, true);
                    call_ajax_method_json(
                        '/Accounting/InitialGrid_ACS010', "", function (result) {
                        mygrid.xml.top = "response";
                        mygrid.xml.row = "./rows/object";
                        mygrid.parse(result);

                        SpecialGridControl(mygrid, ["Button"]);
                    /* ===== binding event when finish load data ===== */
                        BindOnLoadedEvent(mygrid, function (gen_ctrl) {

                            for (var i = 0; i < mygrid.getRowsNum(); i++) {

                                var row_id = mygrid.getRowId(i);

                                if (gen_ctrl == true) {
                                    GenerateDownloadButton(mygrid, "btnDownload", row_id, "Button", true);
                                }

                                BindGridButtonClickEvent("btnDownload", row_id, doDownload);

                                var DocumentNoIndex = mygrid.getColIndexById("DocumentNo");


                    /* Show/Hide column depend of Document type and Document name */
                                                    if (i == mygrid.getRowsNum() - 1) {
                                                        mygrid.setSizes();
                                                    }
                                                }
                                            });
                    
                        $("#mygrid_container").LoadDataToGrid(mygrid, pageRow, true, "/Accounting/ACS010_Search", obj, "dtAccountingDocumentList", false, "",
                                            function (result, controls, isWarning) {
                                                if (isWarning == undefined) {
                                                    $("#divSearchResult").show();
                                                }
                    
                                            });
                    
                                        });
               // }
                $("#accoountingDocumentReport").attr("disabled", false);
                $("#btnSearch").attr("disabled", false);
                $("#btnRun").attr("disabled", false);
                master_event.LockWindow(false);
                return;
            }
        );
    }


    /*--- Event --- */   // ind = colum index  
    // doDownload(id, ind)
    function doDownload(row_id) {

        mygrid.selectRow(mygrid.getRowIndex(row_id));

        var fileName = mygrid.cells2(mygrid.getRowIndex(row_id), mygrid.getColIndexById('FilePath')).getValue();

        var objParam = {
            fileName: fileName
        };

        ajax_method.CallScreenController("/Accounting/ACS010_CheckExistFile", objParam, function (data) {

            if (data != undefined) {
                if (data == "1") {

                    if (fileName != null && fileName.length > 4
                        && fileName.substring(fileName.length - 4, fileName.length).toUpperCase() == '.CSV') {

                        ajax_method.CallScreenController("/Accounting/ACS010_DownloadDocument", null, function (data) {
                            download_method.CallDownloadController("ifDownload", "/Accounting/ACS010_DownloadAsCSV", null);
                        }, false);

                    }
                    else {

                        download_method.CallDownloadController("ifDownload", "/Accounting/ACS010_DownloadDocument", null);
                    }
                }
                else {

                    var param = { "module": "Common", "code": "MSG0112" };
                    call_ajax_method_json("/Shared/GetMessage", param, function (data) {

                        /* ====== Open info dialog =====*/
                        OpenInformationMessageDialog(param.code, data.Message);
                    });

                }
            }
        }, false);
    }
});