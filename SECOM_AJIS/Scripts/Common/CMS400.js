/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

/// <reference path="../Base/object/ajax_method.js" />

var myGridCMS400;
var gridBillingTarget;
var gridBillingBasic;
var gridBillingUncancelInvoice;
var objForCMS450;
$(document).ready(function () {

    if ($("#CMS400_CallerScreenId").val() == objCMS400Conts.C_SCREEN_ID_VIEW_BILLING_TARGET_INFORMATION) {
        FirstLoadScreenView();
    }
    else {
        FirstLoadScreenSearch();
        //InitialBillingTargetGrid();
        // InitialBillingBasicGrid();
        //InitialUncancelInvoiceGrid();
        $("#btnSearch").click(search_click);
        $("#btnClear").click(FirstLoadScreenSearch);

        /*==== event Customer Name keypress ====*/
        //$("#divSearchByCondition input[id=BillingClientName]").InitialAutoComplete("/Master/GetBillingClientName"); // Note : in Master/Controllers/BillingClientData.cs/GetBillingClientName()
        $("#BillingClientName").InitialAutoComplete("/Master/GetBillingClientName"); // Note : in Master/Controllers/BillingClientData.cs/GetBillingClientName()

        /*==== event Site Name keypress ====*/
        $("#BillingClientAddress").InitialAutoComplete("/Master/GetBillingClientAddress"); // Note :  Master/Controllers/BillingClientData.cs/GetBillingClientName()
    }

    //////// radio change
    $("#SpecifySearchCustomer").change(cms070_page_change);
    $("#SpecifySearchContract").change(cms070_page_change);

    IntialPage();
});

function InitialBillingTargetGrid() {

    myGridCMS400 = $("#gridBillingTarget").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, true, true, "/Common/CMS400_InitialBillingTargetGrid",
    "", "dtTbt_BillingTargetForView", false);

}

function InitialBillingBasicGrid() {

    myGridCMS400 = $("#gridBillingBasic").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, true, true, "/Common/CMS400_InitialBillingBasicGrid",
    "", "dtViewBillingBasicList", false);

    gridBillingBasic = myGridCMS400;
    SpecialGridControl(myGridCMS400, ["DetailBillingBasic", "DetailBillingDetail"]);
    BindOnLoadedEvent(myGridCMS400,
    function (gen_ctrl) {
        if (CheckFirstRowIsEmpty(myGridCMS400, false) == false) {
            for (var i = 0; i < myGridCMS400.getRowsNum(); i++) {

                var rid = myGridCMS400.getRowId(i);

                if (gen_ctrl == true) {
                    //----------- Generate Detail button        
                    GenerateDetailButton(myGridCMS400, "btnBillingBasic", rid, "DetailBillingBasic", true);
                    GenerateDetailButton(myGridCMS400, "btnBillingDetail", rid, "DetailBillingDetail", true);
                }

                BindGridButtonClickEvent("btnBillingBasic", rid, doSelectBillingBasicGridOnBillingBasic);
                BindGridButtonClickEvent("btnBillingDetail", rid, doSelectBillingBasicGridOnBasicDetail);
            }
            //$("#chkHeader").unbind("click");
            //$("#chkHeader").click(selectAllCheckboxControl);
            myGridCMS400.setSizes();
        }
    });

}

function InitialUncancelInvoiceGrid() {

    myGridCMS400 = $("#gridInvoice").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, true, true, "/Common/CMS400_InitialUncancelInvoiceGrid",
    "", "dtViewBillingInvoiceListOfLastInvoiceOcc", false);

    //-------- Generate Column image

    SpecialGridControl(myGridCMS400, ["CreditNoteIssueDetail", "NoOfBillingDetail"]);
    BindOnLoadedEvent(myGridCMS400,
    function (gen_ctrl) {
        if (CheckFirstRowIsEmpty(myGridCMS400, false) == false) {
            //Add by Jutarat A. on 22022013
            var billingDetailColInx = myGridCMS400.getColIndexById('NoOfBillingDetail_Text');
            var billingDetailDisplayColInx = myGridCMS400.getColIndexById('NoOfBillingDetail_TextDisplay');
            //End Add

            for (var i = 0; i < myGridCMS400.getRowsNum(); i++) {

                var rid = myGridCMS400.getRowId(i);
                var imageColInx = myGridCMS400.getColIndexById('CreditNoteIssueDetail');

                if (gen_ctrl == true) {
                    //----------- Generate image         
                    var creditFlag = myGridCMS400.cells2(i, myGridCMS400.getColIndexById("CreditNoteFlag")).getValue();

                    if (creditFlag == 1) {
                        //GenerateEditButton(myGridCMS400, "imgCredit", rid, "CreditNoteIssueDetail", true);
                        GenerateImageButtonToGrid(myGridCMS400, "imgCredit", rid, "CreditNoteIssueDetail", true, "creditNote.png", "CreditNote")
                        var imgCreditControl = GenerateGridControlID("imgCredit", rid);
                        var creditTooltip = myGridCMS400.cells2(i, myGridCMS400.getColIndexById("CreditNoteInvoiceForTooltip")).getValue();
                        $("#" + imgCreditControl).attr("title", creditTooltip.replace(new RegExp("_", "gi"), "\r\n")).css("width", "60%").css("height", "80%");
                    }

                    //Modify by Jutarat A. on 22022013
                    //----------- Generate Link
//                    var billingDetailColInx = myGridCMS400.getColIndexById('NoOfBillingDetail_Text');
//                    var textNoOfBilling = myGridCMS400.cells2(i, myGridCMS400.getColIndexById("NoOfBillingDetail_Text")).getValue();
//                    var linkNoOfBilling = '<a id="lnkNumber' + rid + '" href="#" onclick="doSelectInvoiceGridOnNumberLinkInitial(' + rid + ')" >' + textNoOfBilling + '</a>';
//                    myGridCMS400.cells2(i, billingDetailColInx).setValue(linkNoOfBilling);
                    var textNoOfBilling = myGridCMS400.cells2(i, billingDetailColInx).getValue();
                    var linkNoOfBilling = '<a id="lnkNumber' + rid + '" href="#" onclick="doSelectInvoiceGridOnNumberLink(' + rid + ')" >' + textNoOfBilling + '</a>';
                    myGridCMS400.cells2(i, billingDetailDisplayColInx).setValue(linkNoOfBilling);
                    //End Modify

                }
            }
            //$("#chkHeader").unbind("click");
            //$("#chkHeader").click(selectAllCheckboxControl);
            myGridCMS400.setSizes();
        }
    });
}

function FirstLoadScreenView() {
    if (objCMS400Conts.ShowInvoiceList == "True") {
        $("#divuncancelInvoiceInformationSearchResult").show();
        $("#divBillingBasicInformationSearchResult").hide();
        InitialUncancelInvoiceGrid();
    }
    else {
        InitialBillingBasicGrid();
        $("#divuncancelInvoiceInformationSearchResult").hide();
        $("#divBillingBasicInformationSearchResult").show();
    }
    $("#divSpecifySearchInformation").hide();
    $("#divSpecifySearchBillingInformation").hide();
    $("#divSearchByCondition").hide();
    $("#divBillingTargetInformationSearchResult").hide();

}

function IntialPage() {

    // Set null value to "-"  ***
    $("#divBillingtargetInformation").SetEmptyViewData();
}

function FirstLoadScreenSearch() {
    //Init Grid

    gridBillingTarget = $("#gridBillingTarget").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS400_InitialBillingTargetGrid");
    BindControlBillingTargetGrid(gridBillingTarget);

    gridBillingBasic = $("#gridBillingBasic").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS400_InitialBillingBasicGrid");
    BindControlBillingBasicGrid(gridBillingBasic);

    gridBillingUncancelInvoice = $("#gridInvoice").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS400_InitialUncancelInvoiceGrid");
    BindControlUncancelInvoiceGrid(gridBillingUncancelInvoice);

    $("#divSpecifySearchInformation").show();
    $("#divSpecifySearchBillingInformation").show();
    $("#divSearchByCondition").show();

    $("#divBillingTargetInformationSearchResult").hide();
    $("#divuncancelInvoiceInformationSearchResult").hide();
    $("#divBillingBasicInformationSearchResult").hide();
    $("#divBillingtargetInformation").hide();

    FirstLoadControl();
}

function FirstLoadControl() {
    $("#SpecifySearchBilling").attr("checked", true);
    $("#SpecifySearchBillingTargetInfo").attr("checked", true);
    $("#btnSearch").attr("disabled", false);
    $("#btnClear").attr("disabled", false);
    $("#divSearchByCondition").clearForm();
}

function search_click() {
    var obj = {
        strBillingClientCode: $("#BillingClientCode").val(),
        strBillingClientCode2: $("#BillingClientCode2").val(),
        strBillingTargetNo: $("#BillingTargetNo").val(),
        strBillingClientName: $("#BillingClientName").val(),
        strBillingClientAddress: $("#BillingClientAddress").val(),
        strInvoiceNo: $("#InvoiceNo").val(),
        strTaxIDNo: $("#TaxIDNo").val()
    };

    // Validate control
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);
    ajax_method.CallScreenController("/Common/CMS400_ValidateControl", obj,
        function (result, controls) {
            if (result) {
                if ($("#SpecifySearchBillingTargetInfo").prop("checked")) {
                    obj.Flag = 0;
                    if ($("#gridBillingTarget").length > 0) {
                        $("#gridBillingTarget").LoadDataToGrid(gridBillingTarget, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS400_SearchDataToGrid", obj, "doBillingTargetList", false,
                 function () {

                     //document.getElementById('divBillingTargetInformationSearchResult').scrollIntoView();
                     $("#btnSearch").attr("disabled", false);
                     master_event.LockWindow(false);
                     master_event.ScrollWindow("#divBillingTargetInformationSearchResult", false);
                 },
                function () {
                    $("#divBillingTargetInformationSearchResult").show();
                    $("#divuncancelInvoiceInformationSearchResult").hide();
                    $("#divBillingBasicInformationSearchResult").hide();
                }
                );
                    }

                }
                else if ($("#SpecifySearchBillingBasicInfo").prop("checked")) {
                    obj.Flag = 1;
                    if ($("#gridBillingBasic").length > 0) {
                        $("#gridBillingBasic").LoadDataToGrid(gridBillingBasic, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS400_SearchDataToGrid", obj, "dtViewBillingBasicList", false,
                 function () {
                     //document.getElementById('divBillingBasicInformationSearchResult').scrollIntoView();

                     $("#btnSearch").attr("disabled", false);
                     master_event.LockWindow(false);
                     $("#divBillingTargetInformationSearchResult").hide();
                     $("#divuncancelInvoiceInformationSearchResult").hide();
                     $("#divBillingBasicInformationSearchResult").show();
                     master_event.ScrollWindow("#divBillingBasicInformationSearchResult", false);
                 },
                null);
                    }

                }
                else if ($("#SpecifySearchInvoiceInfo").prop("checked")) {
                    obj.Flag = 2;
                    obj.strInvoiceNo = $("#InvoiceNo").val();
                    if ($("#gridInvoice").length > 0) {
                        $("#gridInvoice").LoadDataToGrid(gridBillingUncancelInvoice, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS400_SearchDataToGrid", obj, "dtViewBillingInvoiceListOfLastInvoiceOcc", false,
                 function () {
                     $("#btnSearch").attr("disabled", false);
                     master_event.LockWindow(false);
                     master_event.ScrollWindow("#divuncancelInvoiceInformationSearchResult", false);
                 },
                  function () {
                      $("#divBillingTargetInformationSearchResult").hide();
                      $("#divuncancelInvoiceInformationSearchResult").show();
                      $("#divBillingBasicInformationSearchResult").hide();
                  });
                    }


                    $("#btnSearch").attr("disabled", false);
                    master_event.LockWindow(false);
                }

            }
            else {
                $("#divBillingTargetInformationSearchResult").hide();
                $("#divuncancelInvoiceInformationSearchResult").hide();
                $("#divBillingBasicInformationSearchResult").hide();

                $("#btnSearch").attr("disabled", false);
                master_event.LockWindow(false);
            }

        });
}

function BindControlBillingTargetGrid(myGridCMS400) {
    SpecialGridControl(myGridCMS400, ["DetailBillingTarget", "DetailBillingBasic"]);
    BindOnLoadedEvent(myGridCMS400,
    function (gen_ctrl) {
        if (CheckFirstRowIsEmpty(myGridCMS400, false) == false) {
            for (var i = 0; i < myGridCMS400.getRowsNum(); i++) {

                var rid = myGridCMS400.getRowId(i);

                if (gen_ctrl == true) {
                    //----------- Generate Detail button        
                    GenerateDetailButton(myGridCMS400, "btnBillingTarget", rid, "DetailBillingTarget", true);
                    GenerateDetailButton(myGridCMS400, "btnBillingBasic", rid, "DetailBillingBasic", true);
                }

                BindGridButtonClickEvent("btnBillingTarget", rid, doSelectBillingtargetGrid);
                BindGridButtonClickEvent("btnBillingBasic", rid, doSelectBillingtargetGridOnBillinBasicButton);
            }
            //$("#chkHeader").unbind("click");
            //$("#chkHeader").click(selectAllCheckboxControl);
            myGridCMS400.setSizes();
        }
    });
}

function BindControlBillingBasicGrid(myGridCMS400) {
    SpecialGridControl(myGridCMS400, ["DetailBillingBasic", "DetailBillingDetail"]);
    BindOnLoadedEvent(myGridCMS400,
    function (gen_ctrl) {
        if (CheckFirstRowIsEmpty(myGridCMS400, false) == false) {
            for (var i = 0; i < myGridCMS400.getRowsNum(); i++) {

                var rid = myGridCMS400.getRowId(i);

                if (gen_ctrl == true) {
                    //----------- Generate Detail button        
                    GenerateDetailButton(myGridCMS400, "btnBillingBasic", rid, "DetailBillingBasic", true);
                    GenerateDetailButton(myGridCMS400, "btnBillingDetail", rid, "DetailBillingDetail", true);
                }

                BindGridButtonClickEvent("btnBillingBasic", rid, doSelectBillingBasicGridOnBillingBasic);
                BindGridButtonClickEvent("btnBillingDetail", rid, doSelectBillingBasicGridOnBasicDetail);
            }
            //$("#chkHeader").unbind("click");
            //$("#chkHeader").click(selectAllCheckboxControl);
            myGridCMS400.setSizes();
        }
    });
}

function BindControlUncancelInvoiceGrid(myGridInvoiceCMS400) {
    SpecialGridControl(myGridInvoiceCMS400, ["CreditNoteIssueDetail", "NoOfBillingDetail"]);
    BindOnLoadedEvent(myGridInvoiceCMS400,
    function (gen_ctrl) {
        if (CheckFirstRowIsEmpty(myGridInvoiceCMS400, false) == false) {
            //For credit note
            var creditNoteFlagColInx = myGridInvoiceCMS400.getColIndexById("CreditNoteFlag");
            var imageColInx = myGridInvoiceCMS400.getColIndexById('CreditNoteIssueDetail');
            var creditNoteTooltipColInx = myGridInvoiceCMS400.getColIndexById("CreditNoteInvoiceForTooltip")
            //For billing details
            var billingDetailColInx = myGridInvoiceCMS400.getColIndexById('NoOfBillingDetail_Text');
            var billingDetailDisplayColInx = myGridInvoiceCMS400.getColIndexById('NoOfBillingDetail_TextDisplay');
            //For first issue
            var firstIssueInvFlagColInx = myGridInvoiceCMS400.getColIndexById('FirstIssueInvFlag');
            var firstIssueInvDisplayColInx = myGridInvoiceCMS400.getColIndexById('FirstIssueInvDisplay');

            for (var i = 0; i < myGridInvoiceCMS400.getRowsNum(); i++) {
                var rid = myGridInvoiceCMS400.getRowId(i);

                //if (gen_ctrl == true) {
                    //----------- Generate image         
                    var creditFlag = myGridInvoiceCMS400.cells2(i, creditNoteFlagColInx).getValue();

                    if (creditFlag == 1) {
                        //GenerateEditButton(myGridInvoiceCMS400, "imgCredit", rid, "CreditNoteIssueDetail", true);
                        GenerateImageButtonToGrid(myGridInvoiceCMS400, "imgCredit", rid, "CreditNoteIssueDetail", true, "creditNote.png", "CreditNote")
                        var imgCreditControl = GenerateGridControlID("imgCredit", rid);
                        var creditTooltip = myGridInvoiceCMS400.cells2(i, creditNoteTooltipColInx).getValue();
                        $("#" + imgCreditControl).attr("title", creditTooltip.replace(new RegExp("_", "gi"), "\r\n")).css("width", "60%").css("height", "80%");
                    }

                    //----------- Generate Link
                    var textNoOfBilling = myGridInvoiceCMS400.cells2(i, billingDetailColInx).getValue();
                    var linkNoOfBilling = '<a id="lnkNumber' + rid + '" href="#" onclick="doSelectInvoiceGridOnNumberLink(' + rid + ')" >' + textNoOfBilling + '</a>';
                    myGridInvoiceCMS400.cells2(i, billingDetailDisplayColInx).setValue(linkNoOfBilling);

                    //----------- Display FirstIssueInvFlag (Print) column
                    var firstIssueInvFlagValue = myGridInvoiceCMS400.cells2(i, firstIssueInvFlagColInx).getValue();
                    if (firstIssueInvFlagValue == true) {
                        myGridInvoiceCMS400.cells2(i, firstIssueInvDisplayColInx).setValue(objCMS400Conts.FirstIssueInvFlagDone);
                    }
                    else {
                        myGridInvoiceCMS400.cells2(i, firstIssueInvDisplayColInx).setValue(objCMS400Conts.FirstIssueInvFlagNotYet);
                    }
                //}
            }
            //$("#chkHeader").unbind("click");
            //$("#chkHeader").click(selectAllCheckboxControl);
            myGridInvoiceCMS400.setSizes();
        }
    });
}

//////////// Even Click BillingTargetGrid ///////////////
function doSelectBillingtargetGrid(row_id) {

    gridBillingTarget.selectRow(gridBillingTarget.getRowIndex(row_id));

    var strBillingTargetCode = gridBillingTarget.cells2(gridBillingTarget.getRowIndex(row_id), gridBillingTarget.getColIndexById('BillingTargetCode_Short')).getValue().toString();
    var obj = {
        BillingTargetCode: strBillingTargetCode
    };
    ajax_method.CallScreenControllerWithAuthority("/Common/CMS410", obj, true);
}

function doSelectBillingtargetGridOnBillinBasicButton(row_id) {
    master_event.LockWindow(true);
    gridBillingTarget.selectRow(gridBillingTarget.getRowIndex(row_id));
    //var strBillingTargetCode = gridBillingTarget.cells2(gridBillingTarget.getRowIndex(row_id), gridBillingTarget.getColIndexById('BillingTargetCode_Short')).getValue().toString();
    var obj = {
        strBillingTargetCode: gridBillingTarget.cells2(gridBillingTarget.getRowIndex(row_id), gridBillingTarget.getColIndexById('BillingTargetCode_Short')).getValue().toString()
    };
    obj.Flag = 1;
    if ($("#gridBillingBasic").length > 0) {
        $("#gridBillingBasic").LoadDataToGrid(gridBillingBasic, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS400_GetViewBillingBasicForGrid", obj, "dtViewBillingBasicList", false,
                 function () {
                     document.getElementById('divBillingBasicInformationSearchResult').scrollIntoView();
                 },
                null);
    }
    $("#divBillingBasicInformationSearchResult").show();
    master_event.ScrollWindow("#divBillingBasicInformationSearchResult", false);
    master_event.LockWindow(false);
}


//////////// Even Click BillingBasicGrid ///////////////
function doSelectBillingBasicGridOnBillingBasic(row_id) {
    gridBillingBasic.selectRow(gridBillingBasic.getRowIndex(row_id));

    var obj = {
        ContractCode: gridBillingBasic.cells2(gridBillingBasic.getRowIndex(row_id), gridBillingBasic.getColIndexById('ContractCode_Short')).getValue().toString(),
        BillingOCC: gridBillingBasic.cells2(gridBillingBasic.getRowIndex(row_id), gridBillingBasic.getColIndexById('BillingOCC')).getValue().toString()
    };
    ajax_method.CallScreenControllerWithAuthority("/Common/CMS420", obj, true);
}

function doSelectBillingBasicGridOnBasicDetail(row_id) {
    gridBillingBasic.selectRow(gridBillingBasic.getRowIndex(row_id));

    objForCMS450 = {
        ContractCode: gridBillingBasic.cells2(gridBillingBasic.getRowIndex(row_id), gridBillingBasic.getColIndexById('ContractCode_Short')).getValue().toString(),
        BillingOCC: gridBillingBasic.cells2(gridBillingBasic.getRowIndex(row_id), gridBillingBasic.getColIndexById('BillingOCC')).getValue().toString()
    };
    ajax_method.CallScreenControllerWithAuthority("/Common/CMS450", objForCMS450, true);
    //$("#dlgCMS400").OpenCMS450Dialog("CMS450");
}
function CMS450Object() {
    return objForCMS450;
}

//////////// Even Click UncancelInvoiceGrid ///////////////
function doSelectInvoiceGridOnNumberLink(row_id) {
    //Modify by Jutarat A. on 22022013
//    objForCMS450 = {
//        InvoiceNo: gridBillingUncancelInvoice.cells2(gridBillingUncancelInvoice.getRowIndex(row_id), gridBillingUncancelInvoice.getColIndexById('InvoiceNo')).getValue().toString()
//    };
    objForCMS450 = null;
    if (objCMS400Conts.ShowInvoiceList == "True") {
        objForCMS450 = {
            InvoiceNo: myGridCMS400.cells2(myGridCMS400.getRowIndex(row_id), myGridCMS400.getColIndexById('InvoiceNo')).getValue().toString()
        };
    }
    else {
        objForCMS450 = {
            InvoiceNo: gridBillingUncancelInvoice.cells2(gridBillingUncancelInvoice.getRowIndex(row_id), gridBillingUncancelInvoice.getColIndexById('InvoiceNo')).getValue().toString()
        };
    }
    //End Modify

    ajax_method.CallScreenControllerWithAuthority("/Common/CMS450", objForCMS450, true);
    //$("#dlgCMS400").OpenCMS450Dialog("CMS450");
}

function cms070_page_change() {
    var strRadio;
    if ($("#SpecifySearchContract").prop("checked")) {
        strRadio = "Contract";
    }
    else if ($("#SpecifySearchCustomer").prop("checked")) {
        strRadio = "Customer";
    }
    var obj = {
        radioDefault: strRadio
    };
    ajax_method.CallScreenControllerWithAuthority("/Common/CMS070", obj, false);
}

function doSelectInvoiceGridOnNumberLinkInitial(row_id) {
    objForCMS450 = {
        InvoiceNo: myGridCMS400.cells2(myGridCMS400.getRowIndex(row_id), myGridCMS400.getColIndexById('InvoiceNo')).getValue().toString()
    };
    ajax_method.CallScreenControllerWithAuthority("/Common/CMS450", objForCMS450, true);
    //$("#dlgCMS400").OpenCMS450Dialog("CMS450");
}