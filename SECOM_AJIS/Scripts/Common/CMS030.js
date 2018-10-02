/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>
/// <reference path="../../Scripts/Base/AutoComplete.js"/>

/// <reference path="../../Scripts/Base/master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>
/// <reference path="../Base/object/ajax_method.js" />
/// <reference path="../Base/object/command_event.js" />
/// <reference path="../Base/object/master_event.js" />



var myDocType;
var myDocCode;
var CMS030_Count = 0;
var pageRow = ROWS_PER_PAGE_FOR_SEARCHPAGE;
var mygrid;
var mode = 0;
var documentName = "";

// Add by Jirawat Jannet on 2016-12-13
var validateChangePlanDetail = ['GenerateDateFrom', 'GenerateDateTo'];


$(document).ready(function () {

    ////----------------------- disable for phase 1 , so phase 2 is enable------------
    //$("#btnInvoicePrintingService").attr("disabled",true);
    ////-----------------------------------------------------------------------------


    // --- Date Picker ---
    InitialDateFromToControl("#GenerateDateFrom", "#GenerateDateTo");


    /* --- Event binding ---*/
    //$("#DocumentType").RelateControlEvent(cboDocumentType_change);
    //$("#DocumentCode").RelateControlEvent(cboDocumentName_change);

    $("#DocumentType").change(cboDocumentType_change);
    $("#DocumentCode").change(cboDocumentName_change);


    $("#btnSearch").click(function () {

        CMS030_Search();

    });

    $("#btnDownloadCTR095").click(function () {

        doDownload_CTR095();

    });


    $("#btnClear").click(function () {
        $("#formSearch").clearForm();
        initialPage();

        CMS030_Count = 0;
        CloseWarningDialog();

        //Enable invoice printing service button
        $("#btnInvoicePrintingService").removeAttr("disabled");
    });

    /* --- Invoice Printing Service (for next phase) -----*/
    $("#btnInvoicePrintingService").click(function () {
        // Open screen CMS460
        var obj = "";
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS460", obj, true);
    });


    initialPage();

    //    InitialTrimTextEvent([
    //        "DocumentCode",
    //        "QuotationTargetCode",
    //        "Alphabet",
    //        "ProjectCode",
    //        "ContractCode",
    //        "OCC",
    //        "InstrumentCode",
    //        "BillingTargetCode"
    //    ]);



});

//function GotoNextTabBreak(controls) {
//    control.each(function () {
//        var type = this.type, tag = this.tagName.toLowerCase();

//        if (tag == 'form' || tag == 'div')
//            return $(':input', this).GotoNextTabBreak();
//        if (type == 'text' || type == 'password' || tag == 'textarea') {
//            if (this.prop("readonly") == false) {
//                this.focus();
//            }
//        }
//        else if (tag == 'select') {
//            if (this.prop("readonly") == false) {
//                this.focus();
//            }
//        }
//    });
//}

function CMS030_Search() {
    var box = $('#mygrid_container');
    var grid = $('#mygrid_container_grid');
    VaridateCtrl(validateChangePlanDetail, null);

    // disable button search
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    var iMode = getDisplayMode();

    var parameter = CreateObjectData($("#formSearch").serialize() + "&Mode=" + iMode, true);

    call_ajax_method_json("/Common/CMS030_Validate", parameter, function (result, controls) {

        // enable button search
        $("#btnSearch").attr("disabled", false);
        master_event.LockWindow(false);

        if (controls != undefined) {
            VaridateCtrl(["DocumentType", "DocumentCode"], controls);
        }

        if (result == true) {

            mygrid = CreateGridControl("mygrid_container", pageRow, true);




            var url;

            if (iMode == 0) {
                url = "/Common/InitialGrid_CMS030";
            } else if (iMode == 1) {
                url = "/Common/InitialGrid_CMS030_Mode1";
            }
            else if (iMode == 2) {
                url = "/Common/InitialGrid_CMS030_Mode2";
            }
            else if (iMode == 3) {
                url = "/Common/InitialGrid_CMS030_Mode3";
            }
            else if (iMode == 4) {
                url = "/Common/InitialGrid_CMS030_Mode4";
            }
            else if (iMode == 5) {
                url = "/Common/InitialGrid_CMS030_Mode5";
            }
            else if (iMode == 6) {
                url = "/Common/InitialGrid_CMS030_Mode6";
            }
            else if (iMode == 7) {
                url = "/Common/InitialGrid_CMS030_Mode7";
            }
            else if (iMode == 8) {
                url = "/Common/InitialGrid_CMS030_Mode8";
            }
            else if (iMode == 9) {
                url = "/Common/InitialGrid_CMS030_Mode8";
            }

            call_ajax_method_json(url, "", function (result) {
                mygrid.xml.top = "response";
                mygrid.xml.row = "./rows/object";
                mygrid.parse(result);

                SpecialGridControl(mygrid, ["Button"]);

                /* ===== binding event when finish load data ===== */
                BindOnLoadedEvent(mygrid, function (gen_ctrl) {

                    for (var i = 0; i < mygrid.getRowsNum() ; i++) {

                        var row_id = mygrid.getRowId(i);

                        if (gen_ctrl == true) {
                            GenerateDownloadButton(mygrid, "btnDownload", row_id, "Button", true);
                        }

                        // edit by jirawat jannet on 2016-12-13
                        // ถ้า documeny code ไม่ใช่ BLR060 ให้ทำงานด้านล่าง
                        if (iMode != 9) {

                            BindGridButtonClickEvent("btnDownload", row_id, doDownload);

                            var DocumentNoIndex = mygrid.getColIndexById("DocumentNo");

                            if (DocumentNoIndex != undefined) {
                                mygrid.setColumnLabel(DocumentNoIndex, documentName);
                            }
                        } else {
                            BindGridButtonClickEvent("btnDownload", row_id, doDownload_BLR060);
                        }


                        /* Show/Hide column depend of Document type and Document name */
                        if (i == mygrid.getRowsNum() - 1) {
                            mygrid.setSizes();
                        }
                    }
                    mygrid.setSizes();
                });

                if (iMode == 9) {
                    box.css('width', '600px');
                    grid.css('width', '600px');
                } else {
                    box.css('width', '99%');
                    grid.css('width', '99%');
                }

                $("#mygrid_container").LoadDataToGrid(mygrid, pageRow, true, "/Common/CMS030_SearchResponse", parameter, "dtDocumentData", false,
                function (result, controls, isWarning) {

                    if (isWarning == undefined) {
                        $("#divSearchResult").show();
                    } else {
                        $("#divSearchResult").hide();
                    }
                    mygrid.setSizes();

                });

            });


        }
    });



}

function cboDocumentType_change(istab, isblur) {



    $("#DocumentType").removeClass("highlight");
    $("#DocumentCode").removeClass("highlight");

    /* set defult */
    $('#DocumentCode >option').remove();
    //$('#IssueOfficeCode >option').remove();
    $("#lblDocumentNo").html($("#CMS030_lbldocumnetNo").val());


    ClearDateFromToControl("#GenerateDateFrom", "#GenerateDateTo");


    // keep docType , docCode
    myDocType = $.trim($("#DocumentType").val());
    myDocCode = "";

    var parameter = { "strDocumentType": $("#DocumentType").val() }
    call_ajax_method("/Common/GetDocumentName", parameter, update_documentName_combo);

    // firstly , hide all search controls
    hideAllSearchControls(true);


    //if (istab && isblur) {
    //    $("#DocumentCode").focus();
    //}

    var obj = { "strDocumentType": $("#DocumentType").val() }
    call_ajax_method('/Common/CMS030_GetOperationOffice', obj, function (result2, controls) {
        if (result2 != null) {
            if (result2.List.length != 1) {
                regenerate_combo("#OperationOfficeCode", result2);
            }
        }
    });

}

/*--- Methods ---*/
function update_documentName_combo(data) {
    regenerate_combo("#DocumentCode", data);
}


function cboDocumentName_change(istab, isblur) {

    $("#DocumentCode").removeClass("highlight");

    // keep docCode
    myDocCode = $.trim($("#DocumentCode").val());

    // set label document no.
    var docCode = { "strDocumentCode": $("#DocumentCode").val() };
    call_ajax_method("/Common/CMS030_GetDocumentNoName", docCode, update_lblDocumentNo);

    // binding cboIssueOffice
    var docType = { "strDocumentType": $("#DocumentType").val() };
    call_ajax_method("/Common/GetIssueOfficeCode", docType, update_issueOffice_combo);

    //Enable invoice printing service button
    $("#btnInvoicePrintingService").removeAttr("disabled");

    if ($("#DocumentCode").val() == "") {

        hideAllSearchControls(true);
    }
    else {
        // Show/hide search controls
        showSearchControls();
    }

    //Add MA By Pachara S.
    if (myDocCode == "CTR950") {
        hideAllSearchControls_CTR950(false);
        $("#btnInvoicePrintingService").hide();
        $("#btnSearch").hide();
        $("#btnDownloadCTR095").show();
    }
    else {
        $("#btnInvoicePrintingService").show();
        $("#btnSearch").show();
        $("#btnDownloadCTR095").hide();
    }

    if (istab && isblur) {

    }

}

/*--- Methods ---*/
function update_lblDocumentNo(data) {
    if (data[0] != null) {
        $("#lblDocumentNo").html(data[0].DocumentNoName);

        // Keep docuemnt name for assign to column header
        documentName = data[0].DocumentNoName;
    }
    else {
        $("#lblDocumentNo").html($("#CMS030_lbldocumnetNo").val());

        // Keep docuemnt name for assign to column header
        documentName = $("#CMS030_lbldocumnetNo").val();
    }

}

/*--- Methods ---*/
function update_issueOffice_combo(data) {
    regenerate_combo("#IssueOfficeCode", data);
}

function initialPage() {
    myDocType = "";
    myDocCode = "";

    // firstly , hide all search controls
    hideAllSearchControls(true);

    $("#divSearchResult").hide();
    $("#btnDownloadCTR095").hide();

    /* set defult */
    $('#DocumentCode >option').remove();
    $('#IssueOfficeCode >option').remove();
    $("#lblDocumentNo").html($("#CMS030_lbldocumnetNo").val());

    var parameter = { "strDocumentType": $("#DocumentType").val() }
    call_ajax_method("/Common/GetDocumentName", parameter, update_documentName_combo);

    var docType = { "strDocumentType": "dummy" };
    call_ajax_method("/Common/GetIssueOfficeCode", docType, update_issueOffice_combo);

}

/*--- Event --- */   // ind = colum index  
// doDownload(id, ind)
function doDownload(row_id) {

    mygrid.selectRow(mygrid.getRowIndex(row_id));

    var documentOCC = mygrid.cells2(mygrid.getRowIndex(row_id), mygrid.getColIndexById('DocumentOCC')).getValue();
    var documentNo = mygrid.cells2(mygrid.getRowIndex(row_id), mygrid.getColIndexById('DocumentNo')).getValue();
    var documentCode = mygrid.cells2(mygrid.getRowIndex(row_id), mygrid.getColIndexById('DocumentCode')).getValue();
    var fileName = mygrid.cells2(mygrid.getRowIndex(row_id), mygrid.getColIndexById('FilePath')).getValue();


    var objParam = {
        "strDocumentNo": documentNo,
        "documentOCC": documentOCC,
        "strDocumentCode": documentCode,
        "fileName": fileName
    };

    ajax_method.CallScreenController("/Common/CMS030_CheckExistFile", objParam, function (data) {

        if (data != undefined) {
            if (data == "1") {

                //Modify by Jutarat A. on 17082012
                /* --- download and write log --- */
                //var key = ajax_method.GetKeyURL(null);
                //var url = ajax_method.GenerateURL("/Common/CMS030_DownloadPdfAndWriteLog?k=" + key + "&strDocumentNo=" + documentNo + "&documentOCC=" + documentOCC + "&strDocumentCode=" + documentCode + "&fileName=" + fileName)
                //window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
                if (fileName != null && fileName.length > 4
                    && fileName.substring(fileName.length - 4, fileName.length).toUpperCase() == '.CSV') {

                    ajax_method.CallScreenController("/Common/CMS030_DownloadPdfAndWriteLog", objParam, function (data) {
                        download_method.CallDownloadController("ifDownload", "/Common/CMS030_DownloadAsCSV", null);
                    }, false);

                }
                else {

                    var key = ajax_method.GetKeyURL(null);
                    var url = ajax_method.GenerateURL("/Common/CMS030_DownloadPdfAndWriteLog?k=" + key + "&strDocumentNo=" + documentNo + "&documentOCC=" + documentOCC + "&strDocumentCode=" + documentCode + "&fileName=" + fileName)
                    window.open(url, "", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
                }
                //End Modify
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

// add by jirawat jannet on 2016-12-13
// doDownload_BLR060(id, ind)
function doDownload_BLR060(row_id) {

    mygrid.selectRow(mygrid.getRowIndex(row_id));

    var GenerateDateFrom = mygrid.cells2(mygrid.getRowIndex(row_id), mygrid.getColIndexById('GenerateDateFrom')).getValue();
    var GenerateDateTo = mygrid.cells2(mygrid.getRowIndex(row_id), mygrid.getColIndexById('GenerateDateTo')).getValue();

    var objParam = {
        "GenerateDateFrom": GenerateDateFrom,
        "GenerateDateTo": GenerateDateTo
    };

    download_method.CallDownloadController("ifDownload", "/Common/CMW030_DownloadBLR060CsvReport", objParam);

}

//Add MA By Pachara S. 29032017
function doDownload_CTR095() {

    var GenerateDateFrom = $("#GenerateDateFrom").val();
    var GenerateDateTo = $("#GenerateDateTo").val();

    var objParam = {
        "GenerateDateFrom": GenerateDateFrom,
        "GenerateDateTo": GenerateDateTo
    };

    download_method.CallDownloadController("ifDownload", "/Common/CMS030_DownloadCTR950CsvReport", objParam);

}


/* --- Show/Hide column depend on docTypeCode and docCode --- */
function showColumn() {

    // firstly , hide all columns
    hideAllColumn(true);


    if (myDocType == "00" || myDocType == "01") { /* C_DOCUMENT_TYPE_CONTRACT = "00"  */ /* C_DOCUMENT_TYPE_MA = "01"  */
        showColGenerateDate(true);
        showColDocumentNo(true);
        showColOfficeName_Con(true);
        showColOfficeName_Oper(true);
        showColButton(true);
    }
    else if (myDocType == "02") { /* C_DOCUMENT_TYPE_INSTALLATION = "02"  */
        showColGenerateDate(true);
        showColDocumentNo(true);
        showColOfficeName_Oper(true);
        showColOfficeName_Issue(true);
        showColButton(true);
    }
    else if (myDocType == "03") { /* C_DOCUMENT_TYPE_INVENTORY = "03"  */
        showColGenerateDate(true);
        showColDocumentNo(true);
        showColOfficeName_Issue(true);
        showColButton(true);
    }
        /* C_DOCUMENT_TYPE_INCOME = "04"  */
    else if (myDocType == "04" &&
                    (
                        myDocCode == "BLR010" ||
                        myDocCode == "BLR020" ||
                        myDocCode == "ICR010" ||
                        myDocCode == "ICR020" ||
                        myDocCode == "ICR030" ||
                        myDocCode == "ICR040"

                    )
            ) {

        showColGenerateDate(true);
        showColDocumentNo(true);
        showColOfficeName_Bill(true);
        showColButton(true);
    }
        /* C_DOCUMENT_TYPE_COMMON = "05"  */
    else if (/*myDocType == "05"*/myDocType == "04" &&
                    (
                        myDocCode == "CMR010"
                    )
            ) {
        showColGenerateDate(true);
        showColButton(true);
    }
    else {
        // show all column
        hideAllColumn(false);
    }

}


/* ---- For support : Cannot set hidden column (technique of grid display problem - -")*/
function getDisplayMode() {


    if (myDocType == "00" || myDocType == "01") { /* C_DOCUMENT_TYPE_CONTRACT = "00"  */ /* C_DOCUMENT_TYPE_MA = "01"  */
        mode = 1;
    }
    else if (myDocType == "02") { /* C_DOCUMENT_TYPE_INSTALLATION = "02"  */
        mode = 2;
    }
    else if (myDocType == "03" && (myDocCode == "IVR090" || myDocCode == "IVR110")) { /* C_DOCUMENT_TYPE_INVENTORY = "03"  */
        mode = 6;
    }
    else if (myDocType == "03" &&
                    (
                        myDocCode == "IVR200" ||
                        myDocCode == "IVR201" ||
                        myDocCode == "IVR202" ||
                        myDocCode == "IVR203"
                    )
            ) {
        mode = 8;
    }
    else if (myDocType == "03") { /* C_DOCUMENT_TYPE_INVENTORY = "03"  */
        mode = 3;
    }
        /* C_DOCUMENT_TYPE_INCOME = "04"  */
    else if (myDocType == "04" &&
                    (
                          myDocCode == "XX"
        //                        myDocCode == "BLR010" ||
        //                        myDocCode == "BLR020" ||
        //                        myDocCode == "ICR010" ||
        //                        myDocCode == "ICR020" ||
        //                        myDocCode == "ICR030"

                    )
            ) {

        mode = 4;
    }
        /* C_DOCUMENT_TYPE_INVENTORY = "05"  */
    else if (/*myDocType == "05"*/myDocType == "04" &&
                    (
                        myDocCode == "CMR010" ||
                        myDocCode == "BLR010" ||
                        myDocCode == "BLR020" ||
                        myDocCode == "ICR010" ||
                        myDocCode == "ICR020" ||
                        myDocCode == "ICR030" ||
                        myDocCode == "ICR040" ||
                        myDocCode == "BLR030" ||
                        myDocCode == "BLR040"
                    )
            ) {
        mode = 5;
    }
    else if (myDocType == "05" &&
                    (
                        myDocCode == "CMR020"
                    )
            ) {
        mode = 7;
    }
    else if (myDocType == '04' && myDocCode == 'BLR060') {
        mode = 9;
    }
    else {
        // show all column
        mode = 0;
    }

    return mode;

}

/* --- hide column --- */
function hideAllColumn(isHide) {
    var colInx;

    // DocumentNo
    colInx = mygrid.getColIndexById('DocumentNo');
    mygrid.setColumnHidden(colInx, isHide);

    // OfficeName_Con
    colInx = mygrid.getColIndexById('ConOfficeCodeName');
    mygrid.setColumnHidden(colInx, isHide);

    // OfficeName_Oper
    colInx = mygrid.getColIndexById('OperOfficeCodeName');
    mygrid.setColumnHidden(colInx, isHide);

    // OfficeName_Bill
    colInx = mygrid.getColIndexById('BillOfficeCodeName');
    mygrid.setColumnHidden(colInx, isHide);


    // OfficeName_Issue
    colInx = mygrid.getColIndexById('IssueOfficeCodeName');
    mygrid.setColumnHidden(colInx, isHide);

    // Button downdload
    colInx = mygrid.getColIndexById('Button');
    mygrid.setColumnHidden(colInx, isHide);


}

// GenerateDate
function showColGenerateDate(isShow) {
    var colInx;
    colInx = mygrid.getColIndexById('GenerateDate');
    //mygrid.setColumnHidden(colInx, !isShow);

}

// DocumentNo
function showColDocumentNo(isShow) {
    var colInx;
    colInx = mygrid.getColIndexById('DocumentNo');
    mygrid.setColumnHidden(colInx, !isShow);

}


// OfficeName_Con
function showColOfficeName_Con(isShow) {
    var colInx;
    colInx = mygrid.getColIndexById('ConOfficeCodeName');
    mygrid.setColumnHidden(colInx, !isShow);

}

// OfficeName_Oper
function showColOfficeName_Oper(isShow) {
    var colInx;
    colInx = mygrid.getColIndexById('OperOfficeCodeName');
    mygrid.setColumnHidden(colInx, !isShow);


}

// OfficeName_Bill
function showColOfficeName_Bill(isShow) {
    var colInx;
    colInx = mygrid.getColIndexById('BillOfficeCodeName');
    mygrid.setColumnHidden(colInx, !isShow);

}

// OfficeName_Issue
function showColOfficeName_Issue(isShow) {
    var colInx;
    colInx = mygrid.getColIndexById('IssueOfficeCodeName');
    mygrid.setColumnHidden(colInx, !isShow);

}

// Button
function showColButton(isShow) {
    var colInx;
    colInx = mygrid.getColIndexById('Button');
    mygrid.setColumnHidden(colInx, !isShow);

}

/* --- Show/Hide search condition controls depend on docTypeCode and docCode --- */
function showSearchControls() {

    // firstly , hide all search controls
    hideAllSearchControls(true);

    if (getCondition_Set1()) {
        showControl_Set1();
    }
    else if (getCondition_Set2()) {
        showControl_Set2();
    }
    else if (getCondition_Set3()) {
        showControl_Set3();
    }
    else if (getCondition_Set4()) {
        showControl_Set4();
    }
    else if (getCondition_Set5()) {
        showControl_Set5();
    }
    else if (getCondition_Set6()) {
        showControl_Set6();
    }
    else if (getCondition_Set7()) {
        showControl_Set7();
    }
    else if (getCondition_Set8()) {
        showControl_Set8();
    }
    else if (getCondition_Set9()) {
        showControl_Set9();
    }
    else if (getCondition_Set10()) {
        showControl_Set10();
    }
    else if (getCondition_Set11()) {
        showControl_Set11();
    }
    else if (getCondition_Set12()) {
        showControl_Set12();
    }
        // Add by Jirawat Jannet on 2016-12-13
    else if (getCondition_Set13()) {
        showControl_Set13();
    }
        // Add by Jirawat Jannet on 2016-12-13
    else if (getCondition_Set14()) {
        showControl_Set14();
    }
    else {
        // hind all controls
        hideAllSearchControls(true);
    }


}

/* --- C_DOCUMENT_TYPE_CONTRACT = "00" --- */
/* --- C_DOCUMENT_TYPE_MA = "01" --- */
/* --- C_DOCUMENT_TYPE_INSTALLATION = "02" --- */
/* --- C_DOCUMENT_TYPE_INVENTORY = "03" --- */
/* --- C_DOCUMENT_TYPE_INCOME = "04" --- */
/* --- C_DOCUMENT_TYPE_COMMON = "05" --- */ // TODO : (Narupon) Wait for SA reply

/* --- Condition Set ---*/

// set 1
function getCondition_Set1() {
    return (
        myDocType == "00" || myDocType == "01"
    );
}

// set 2
function getCondition_Set2() {
    return (
        myDocType == "02" &&
        (
            myDocCode == "ISR010" ||
            myDocCode == "ISR011" ||
            myDocCode == "ISR012" ||
            myDocCode == "ISR013" ||
            myDocCode == "ISR020" ||
            myDocCode == "ISR021" ||
            myDocCode == "ISR022" ||
            myDocCode == "ISR023" ||
            myDocCode == "ISR030" ||
            myDocCode == "ISR031" ||
            myDocCode == "ISR032" ||
            myDocCode == "ISR033" ||
            myDocCode == "ISR040" ||
            myDocCode == "ISR041" ||
            myDocCode == "ISR042" ||
            myDocCode == "ISR043" ||
            myDocCode == "ISR044" ||
            myDocCode == "ISR045"



        )
    );
}

// set 3
function getCondition_Set3() {
    return (
        myDocType == "02" &&
        (
            myDocCode == "ISR050" ||
            myDocCode == "ISR051" ||
            myDocCode == "ISR060" ||
            myDocCode == "ISR070" ||
            myDocCode == "ISR080" ||
            myDocCode == "ISR090" ||
            myDocCode == "ISR100"
        )
    );
}

// set 4
function getCondition_Set4() {
    return (
        myDocType == "03" &&
        (
            myDocCode == "IVR010" ||
            myDocCode == "IVR020" ||
            myDocCode == "IVR030" ||
            myDocCode == "IVR040" ||
            myDocCode == "IVR050" ||
            myDocCode == "IVR060" ||
            myDocCode == "IVR070" ||
            myDocCode == "IVR080" ||
            myDocCode == "IVR090" ||
    //myDocCode == "IVR110" ||  // move to Set 10
            myDocCode == "IVR130" ||
            myDocCode == "IVR170" ||
            myDocCode == "IVR190" ||
            myDocCode == "IVR191" ||
            myDocCode == "IVR192" ||
            myDocCode == "IVR120" ||
            myDocCode == "IVR210"

        )
    );
}

// set 5
function getCondition_Set5() {
    return (
        myDocType == "03" && myDocCode == "IVR140"

    );
}

// set 6
function getCondition_Set6() {
    return (
        myDocType == "03" && myDocCode == "IVR150"

    );
}

// set 7
function getCondition_Set7() {
    return (
        myDocType == "03" && myDocCode == "IVR180"

    );
}

// set 8
function getCondition_Set8() {
    return (
        myDocType == "04" &&
        (
              myDocCode == "XX"
    //            myDocCode == "BLR010" ||
    //            myDocCode == "BLR020" ||
    //            myDocCode == "ICR010" ||
    //            myDocCode == "ICR020" ||
    //            myDocCode == "ICR030"

        )
    );
}

// set 9
function getCondition_Set9() {
    return (
    /*myDocType == "05"*/myDocType == "04" &&
        (
            myDocCode == "CMR010" ||    // List of issued invoice/tax invoice/receipt
            //myDocCode == "BLR010" ||  // Commeny by Jirawat Jannet on 2016-12-13
            //myDocCode == "BLR020" ||  // Commeny by Jirawat Jannet on 2016-12-13
            myDocCode == "ICR010" ||
            myDocCode == "ICR020" ||
            myDocCode == "ICR030" ||
            myDocCode == "ICR040"
        )
    );
}

// set 10
function getCondition_Set10() {
    return (
        myDocType == "03" &&
        (
            myDocCode == "IVR100" ||
            myDocCode == "IVR110"
        )
    );
}

// set 11
function getCondition_Set11() {
    return (
        myDocType == "05" &&
        (
            myDocCode == "CMR020"
        )
    );
}

// set 12
function getCondition_Set12() {
    return (
        myDocType == "03" &&
        (
            myDocCode == "IVR200" ||
            myDocCode == "IVR201" ||
            myDocCode == "IVR202" ||
            myDocCode == "IVR203"

        )
    );
}


// set 13 : add by Jirawat Jannet on 2016-12-13
function getCondition_Set13() {
    return (
        myDocType == "04" &&
        (
            myDocCode == "BLR010" ||
            myDocCode == "BLR020" ||
            myDocCode == "BLR030" ||
            myDocCode == "BLR040"

        )
    );
}

// set 14 : add by Jirawat Jannet on 2016-12-13
function getCondition_Set14() {
    return (
        myDocType == "04" && myDocCode == "BLR060"
    );
}

/*  --- Hide search condition controls -- */
function hideAllSearchControls(isHide) {
    showControls_GenerateDate(!isHide);
    showControls_MonthYear(!isHide);
    showControls_ContractOfficeCode(!isHide);
    showControls_OperationOfficeCode(!isHide);
    showControls_BillingOfficeCode(!isHide);
    showControls_IssueOfficeCode(!isHide);
    showControls_DocumentNo(!isHide);
    showControls_QuotationTagetCodeAlphabet(!isHide);
    showControls_ContractCode(!isHide);
    showControls_OCC(!isHide);
    showControls_ProjectCode(!isHide);
    showControls_InstrumentCode(!isHide);
    showControls_BillingTargetCode(!isHide);

    showControls_LocationCode(!isHide);

}

function hideAllSearchControls_CTR950(isHide) {
    showControls_MonthYear(isHide);
    showControls_ContractOfficeCode(isHide);
    showControls_OperationOfficeCode(isHide);
    showControls_BillingOfficeCode(isHide);
    showControls_IssueOfficeCode(isHide);
    showControls_DocumentNo(isHide);
    showControls_QuotationTagetCodeAlphabet(isHide);
    showControls_ContractCode(isHide);
    showControls_OCC(isHide);
    showControls_ProjectCode(isHide);
    showControls_InstrumentCode(isHide);
    showControls_BillingTargetCode(isHide);
    showControls_LocationCode(isHide);
}

// GenerateDateFrom ,GenerateDateTo // lblGenerateDate,lblddmmyyyy
function showControls_GenerateDate(isShow) {
    if (isShow) {

        $("#GenerateDateFrom").EnableDatePicker(true);
        $("#GenerateDateTo").EnableDatePicker(true);
    }
    else {

        $("#GenerateDateFrom").EnableDatePicker(false);
        $("#GenerateDateTo").EnableDatePicker(false);

        ClearDateFromToControl("#GenerateDateFrom", "#GenerateDateTo");
    }
}

// Month ,Year // lblMonthYear
function showControls_MonthYear(isShow) {
    if (isShow) {
        //$("#Month").show();
        //$("#Year").show();
        //$("#lblMonthYear").show();

        $("#Month").attr("disabled", false);
        $("#Year").attr("disabled", false);
    }
    else {
        //$("#Month").hide();
        //$("#Year").hide();
        //$("#lblMonthYear").hide();

        $("#Month").attr("disabled", true);
        $("#Year").attr("disabled", true);

        $("#Month").val("");
        $("#Year").val("");
    }
}

// ContractOfficeCode // lblContractOffice
function showControls_ContractOfficeCode(isShow) {
    if (isShow) {
        //$("#ContractOfficeCode").show();
        //$("#lblContractOffice").show();

        $("#ContractOfficeCode").attr("disabled", false);
    }
    else {
        //$("#ContractOfficeCode").hide();
        //$("#lblContractOffice").hide();

        $("#ContractOfficeCode").attr("disabled", true);
        $("#ContractOfficeCode").val("");
    }
}


// OperationOfficeCode //  lblOperationOffice
function showControls_OperationOfficeCode(isShow) {
    if (isShow) {
        //$("#OperationOfficeCode").show();
        //$("#lblOperationOffice").show();

        $("#OperationOfficeCode").attr("disabled", false);
    }
    else {
        //$("#OperationOfficeCode").hide();
        //$("#lblOperationOffice").hide();

        $("#OperationOfficeCode").attr("disabled", true);
        $("#OperationOfficeCode").val("");
    }
}


// BillingOfficeCode // lblBillingOffice
function showControls_BillingOfficeCode(isShow) {
    if (isShow) {
        //$("#BillingOfficeCode").show();
        //$("#lblBillingOffice").show();

        $("#BillingOfficeCode").attr("disabled", false);
    }
    else {
        //$("#BillingOfficeCode").hide();
        //$("#lblBillingOffice").hide();

        $("#BillingOfficeCode").attr("disabled", true);
        $("#BillingOfficeCode").val("");
    }
}


// IssueOfficeCode // lblIssueOffice
function showControls_IssueOfficeCode(isShow) {
    if (isShow) {
        //$("#IssueOfficeCode").show();
        //$("#lblIssueOffice").show();

        $("#IssueOfficeCode").attr("disabled", false);
    }
    else {
        //$("#IssueOfficeCode").hide();
        //$("#lblIssueOffice").hide();

        $("#IssueOfficeCode").attr("disabled", true);
        $("#IssueOfficeCode").val("");
    }
}


// DocumentNo // lblDocumentNo
function showControls_DocumentNo(isShow) {
    if (isShow) {
        //$("#DocumentNo").show();
        //$("#lblDocumentNo").show();

        $("#DocumentNo").attr("readonly", false);
    }
    else {
        //$("#DocumentNo").hide();
        //$("#lblDocumentNo").hide();


        $("#DocumentNo").attr("readonly", true);
        $("#DocumentNo").val("");
    }
}


// QuotationTargetCode , Alphabet // lblQuotationTagetCodeAlphabet
function showControls_QuotationTagetCodeAlphabet(isShow) {
    if (isShow) {
        //$("#QuotationTargetCode").show();
        //$("#Alphabet").show();
        //$("#lblQuotationTagetCodeAlphabet").show();

        $("#QuotationTargetCode").attr("readonly", false);
        $("#Alphabet").attr("readonly", false);
    }
    else {
        //$("#QuotationTargetCode").hide();
        //$("#Alphabet").hide();
        //$("#lblQuotationTagetCodeAlphabet").hide();

        $("#QuotationTargetCode").attr("readonly", true);
        $("#QuotationTargetCode").val("");
        $("#Alphabet").attr("readonly", true);
        $("#Alphabet").val("");
    }
}


// ContractCode  // lblContractCodeOcc
function showControls_ContractCode(isShow) {
    if (isShow) {
        //$("#ContractCode").show();
        //$("#lblContractCodeOcc").show();

        $("#ContractCode").attr("readonly", false);
    }
    else {
        //$("#ContractCode").hide();
        //$("#lblContractCodeOcc").hide();

        $("#ContractCode").attr("readonly", true);
        $("#ContractCode").val("");
    }
}

// OCC
function showControls_OCC(isShow) {
    if (isShow) {


        $("#OCC").attr("readonly", false);
    }
    else {

        $("#OCC").attr("readonly", true);
        $("#OCC").val("");

    }
}

// ProjectCode // lblProjectCode , lblRemarkStar
function showControls_ProjectCode(isShow) {
    if (isShow) {
        //$("#ProjectCode").show();
        //$("#lblProjectCode").show();
        //$("#lblRemarkStar").show();

        $("#ProjectCode").attr("readonly", false);
    }
    else {
        //$("#ProjectCode").hide();
        //$("#lblProjectCode").hide();
        //$("#lblRemarkStar").hide();

        $("#ProjectCode").attr("readonly", true);
        $("#ProjectCode").val("");
    }
}


// InstrumentCode // lblInstrumentCode
function showControls_InstrumentCode(isShow) {
    if (isShow) {
        //$("#InstrumentCode").show();
        //$("#lblInstrumentCode").show();

        $("#InstrumentCode").attr("readonly", false);
    }
    else {
        //$("#InstrumentCode").hide();
        //$("#lblInstrumentCode").hide();

        $("#InstrumentCode").attr("readonly", true);
        $("#InstrumentCode").val("");
    }
}


// BillingTargetCode // lblBillingTargetCode
function showControls_BillingTargetCode(isShow) {
    if (isShow) {
        //$("#BillingTargetCode").show();
        //$("#lblBillingTargetCode").show();

        $("#BillingTargetCode").attr("readonly", false);
    }
    else {
        //$("#BillingTargetCode").hide();
        //$("#lblBillingTargetCode").hide();

        $("#BillingTargetCode").attr("readonly", true);
        $("#BillingTargetCode").val("");
    }
}


// LocationCode 
function showControls_LocationCode(isShow) {
    if (isShow) {
        $("#LocationCode").attr("disabled", false);
    }
    else {
        $("#LocationCode").attr("disabled", true);
        $("#LocationCode").val("");
    }
}


/* --- Show controls set --- */
function showControl_Set1() {
    showControls_GenerateDate(true);
    showControls_ContractOfficeCode(true);
    showControls_OperationOfficeCode(true);
    showControls_DocumentNo(true);
    showControls_ContractCode(true);
    showControls_OCC(true);
    showControls_QuotationTagetCodeAlphabet(true);
}
function showControl_Set2() {
    showControls_GenerateDate(true);
    showControls_OperationOfficeCode(true);
    showControls_IssueOfficeCode(true);
    showControls_DocumentNo(true);
    showControls_ContractCode(true);
}
function showControl_Set3() {
    showControls_GenerateDate(true);
    showControls_OperationOfficeCode(true);
    showControls_IssueOfficeCode(true);
    showControls_DocumentNo(true);
    showControls_ContractCode(true);
    showControls_ProjectCode(true);

}
function showControl_Set4() {
    showControls_GenerateDate(true);
    showControls_IssueOfficeCode(true);
    showControls_DocumentNo(true);
}
function showControl_Set5() {
    showControls_GenerateDate(true);
    showControls_MonthYear(true);
    showControls_IssueOfficeCode(true);
    showControls_DocumentNo(true);
}
function showControl_Set6() {
    showControls_GenerateDate(true);
    showControls_MonthYear(true);
    showControls_DocumentNo(true);
    showControls_InstrumentCode(true);

    //** Nov 4, 2011 order by buggenie CM-249 
    showControls_IssueOfficeCode(true);

}
function showControl_Set7() {
    showControls_GenerateDate(true);
    showControls_IssueOfficeCode(true);
    showControls_DocumentNo(true);
    showControls_ProjectCode(true);
}
function showControl_Set8() {
    showControls_GenerateDate(true);
    showControls_BillingOfficeCode(true);
    showControls_DocumentNo(true);
    showControls_ProjectCode(true);
    showControls_ContractCode(true);
    showControls_BillingTargetCode(true);
}
function showControl_Set9() {
    showControls_GenerateDate(true);
    showControls_DocumentNo(true);
    showControls_BillingOfficeCode(true);
    showControls_BillingTargetCode(true);
}

function showControl_Set10() {
    showControls_GenerateDate(true);
    showControls_MonthYear(true);
    showControls_IssueOfficeCode(true);
    showControls_LocationCode(true);
}

function showControl_Set11() {
    showControls_GenerateDate(true);
    //CMR020
    showControls_MonthYear(true);
    showControls_DocumentNo(true);

    //disable invoice printing service button
    //$("#btnInvoicePrintingService").attr("disabled", true);
}
function showControl_Set12() {
    showControls_GenerateDate(true);
    showControls_MonthYear(true);
}

// add by Jirawat Jannet on 2016-12-13
function showControl_Set13() {
    showControls_GenerateDate(true);
    showControls_BillingOfficeCode(true);
    showControls_DocumentNo(true);
    showControls_ContractCode(true);
    showControls_OCC(true);
    showControls_BillingTargetCode(true);
}

// add by Jirawat Jannet on 2016-12-13
function showControl_Set14() {
    showControls_GenerateDate(true);
}


