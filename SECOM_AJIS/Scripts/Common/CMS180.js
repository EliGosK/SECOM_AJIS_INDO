/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.7.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/control_events.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>
/// <reference path="../../Scripts/Base/object/ajax_method.js"/>

/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxgridcell.js" /> 
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js" />
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxgrid.js" />
/// <reference path="../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_data.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js"/>
/// <reference path="../json.js" />
/// <reference path="../json2.js" />
/// <reference path="../Base/object/command_event.js" />

var CMS170_mygrid;
var CMS170_pageRow;
var btnSelectId = "CMS170SelectBtn";
var resultInstallationGrid;
var subcontractorGrid;
var instrumentDetailGrid;
var objResult;
var flagScreenNotParam;


var hasAlert = false;
var CMS180 = {

    GridColumnID: {
        AttachFile: {
            FileName: "FileName",
            AttachFileID: "AttachFileID",
            BtnDownload: "downloadButton", BtnDownloadID: "btnDownloadAttach",
            TmpColumn: "TmpColumn"
        }
    },

    Grids: {
        gridAttach: null
    },

    jQuery: {
        AttachFileSection: {
            gridAttachDocList: function () { return $("#CMS180_gridAttachDocList"); },
            txtInstallationSlipHistory: function () { return $("#SlipNo"); }
        }
    },

    Functions: {
        InitialAttachFileGrid: function () {
            CMS180.Grids.gridAttach = CMS180.jQuery.AttachFileSection.gridAttachDocList().InitialGrid(0, false, "/Common/CMS180_InitialSearchGrid");

            SpecialGridControl(CMS180.Grids.gridAttach, [
                CMS180.GridColumnID.AttachFile.BtnDownload
            ]);

            BindOnLoadedEvent(CMS180.Grids.gridAttach, CMS180.Functions.GridAttachBinding);
        },

        GridAttachBinding: function () {
            var grid = CMS180.Grids.gridAttach;

            if (grid != undefined) {
                var _colRemoveBtn = grid.getColIndexById(CMS180.GridColumnID.AttachFile.BtnDownload);

                for (var i = 0; i < grid.getRowsNum(); i++) {
                    var row_id = grid.getRowId(i);
                    GenerateDownloadButton(grid, CMS180.GridColumnID.AttachFile.BtnDownloadID, row_id, CMS180.GridColumnID.AttachFile.BtnDownload, true);
                    BindGridButtonClickEvent(CMS180.GridColumnID.AttachFile.BtnDownloadID, row_id, CMS180.EventHandlers.btnDownloadAttach_click);
                }
            }

            grid.setSizes();
        },

        RetrieveAttachFileData: function(){

            var params = {
                installationSlipNo: CMS180.jQuery.AttachFileSection.txtInstallationSlipHistory().val(),
                maintenanceNo: $("#InstallationMaintenanceNoDetail").val()
            }

            CMS180.jQuery.AttachFileSection.gridAttachDocList().LoadDataToGrid(
                CMS180.Grids.gridAttach, 0, false, "/Common/CMS180_LoadGridAttachedDocList",
                params, "dtAttachFileForGridView", false, null, null);

        },

        EnabledGridAttach: function (enabled) {

            var grid = CMS180.Grids.gridAttach;
            if (grid != undefined) {
                for (var i = 0; i < grid.getRowsNum(); i++) {
                    var row_id = grid.getRowId(i);
                    EnableGridButton(grid, CMS180.GridColumnID.AttachFile.BtnDownloadID, row_id, CMS180.GridColumnID.AttachFile.BtnDownload, enabled);
                }

                var colInx = grid.getColIndexById(CMS180.GridColumnID.AttachFile.BtnDownload)
                grid.setColumnHidden(colInx, enabled);

                //GridControl.SetDisabledButtonOnGrid(grid, ISS030.GridColumnID.AttachFile.BtnRemoveID, IVS020.GridColumnID.SearchResult.BtnEdit, !enabled);
                if (enabled)
                    GridControl.UnlockGrid(grid);
                else
                    GridControl.LockGrid(grid);

            }
        }
    },

    EventHandlers: {
        btnDownloadAttach_click: function (row_id) {
            var grid = CMS180.Grids.gridAttach;
            var _colID = grid.getColIndexById("AttachFileID");
            var _targID = grid.cells(row_id, _colID).getValue();
            var _colRelatedID = grid.getColIndexById("RelatedID");
            var _relatedID = grid.cells(row_id, _colRelatedID).getValue();

            var key = ajax_method.GetKeyURL(null);
            var link = ajax_method.GenerateURL("/Common/CMS180_DownloadAttach" + "?attachID=" + _targID + "&installationSlipNo=" + _relatedID + "&k=" + key);

            window.open(link, "download");

        }
    }
}

$(document).ready(function () {


    //---- Auto Complete ------
    /*==== event Customer Name keypress ====*/
    $("#divSearchCriteria input[id=ContractTargetPurchaserName]").InitialAutoComplete("/Master/GetCustName"); // Note : in Master/Controllers/CustomerData.cs/GetCustName()

    /*==== event Customer Name keypress ====*/
    $("#divSearchCriteria input[id=SiteName]").InitialAutoComplete("/Master/GetSiteName"); // Note : in Master/Controllers/SiteData.cs/GetSiteName()

    /*==== event Customer Name keypress ====*/
    $("#divSearchCriteria input[id=SiteAddress]").InitialAutoComplete("/Master/GetSiteAddress"); // Note : in Master/Controllers/SiteData.cs/GetSiteAddress()

    //--------- initial date ------------//
    //$("#SlipIssueDateFrom").InitialDate();
    //$("#SlipIssueDateTo").InitialDate();
    var now = new Date();
    //$("#SlipIssueDateFrom").val(ConvertDateToTextFormat(ConvertDateObject(now)));
    //$("#SlipIssueDateTo").val(ConvertDateToTextFormat(ConvertDateObject(now)));


    //-------- initial grid ----------//
    //InitialResultInstallationGrid(); //Comment by Jutarat A. on 26022014 (Move)
    InitialSubcontractorGrid();
    InitialInstrumentDetailGrid();
    CMS180.Functions.InitialAttachFileGrid();       // - Add by Nontawat L. on 03-Jul-2014

    if ((CMS180_ViewBag.ContractCode != undefined && CMS180_ViewBag.ContractCode != null && CMS180_ViewBag.ContractCode != "")
     || (CMS180_ViewBag.InstallationSlipNo != undefined && CMS180_ViewBag.InstallationSlipNo != null && CMS180_ViewBag.InstallationSlipNo != "")) {

        ResultGridWithinitial();
    }
    else {
        InitialResultInstallationGrid(); //Add by Jutarat A. on 26022014 (Move)
        $("#divSearchResult").hide();
    }


    //----- even -------//
    $("#SlipNo").change(slipno_change);
    $("#btnSearch").click(Search);
    $("#btnDownload").click(download_form);
    $("#btnClear").click(function () {
        Clear();
        $("#divSearchResult").hide();
    });
    Clear();


    initialHeaderButton();
});


function initialHeaderButton() {
    var myurl = "";

    $("#btnContractBasic").click(function () {
        // go to CMS120
        var obj = { "strContractCode": CMS180_ViewBag.ContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS120", obj, false);
    });

    $("#btnHistoryDigest").click(function () {
        // go to CMS150
        var obj = { "ContractCode": CMS180_ViewBag.ContractCode, "ServiceTypeCode": CMS180_ViewBag.ServiceTypeCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS150", obj, false);
    });

    //Alway disable btnHeader_Installation button 

    $("#btnSalesContractBasic").click(function () {
        // go to CMS160
        var obj = { "strContractCode": CMS180_ViewBag.ContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS160", obj, false);
    });

    $("#btnContractBillingTransfer").click(function () {
        // go to CMS200
        var obj = { "strContractCode": CMS180_ViewBag.ContractCode, "strServiceTypeCode": CMS180_ViewBag.ServiceTypeCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS200", obj, false);
    });

    $("#btnHeader_BillingBasic").click(function () {
        // go to CMS420
        var obj = { "ContractCode": CMS180_ViewBag.ContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS420", obj, false);
    });
}

function InitialResultInstallationGrid() {
    resultInstallationGrid = $("#gridResultList").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Common/CMS180_InitialResultinstallation");
    BindControlResultInstallationGrid(resultInstallationGrid);
}
function ResultGridWithinitial() {
    if ($("#gridResultList").length > 0) {

//Modify by Jutarat A. on 26022014
//        $("#gridResultList").LoadDataToGrid(resultInstallationGrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Common/CMS180_SearchResultWithInitial", "", "dtInstallation", false,
//                 function () {
//                     //document.getElementById('divBillingTargetInformationSearchResult').scrollIntoView();                   
//                     setValueToSiteInfoWithInitial();
//                 },
//                  function () {
//                      $("#divSearchResult").show();
//                  });

        resultInstallationGrid = $("#gridResultList").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, false, "/Common/CMS180_SearchResultWithInitial",
                        "", "dtInstallation", false, false,
                        function () { $("#divSearchResult").show(); }
                        , function () { setValueToSiteInfoWithInitial(); });

        BindControlResultInstallationGrid(resultInstallationGrid);
//End Modify
    }
}
function BindControlResultInstallationGrid(myGridCMS180) {
    SpecialGridControl(myGridCMS180, ["Detail", "Register"]);
    BindOnLoadedEvent(myGridCMS180,
    function (gen_ctrl) {
        if (CheckFirstRowIsEmpty(myGridCMS180, false) == false) {
            var registerColInx = myGridCMS180.getColIndexById('Register');
            var nextStepButtonNameColInx = myGridCMS180.getColIndexById('NextStepButtonName');
            var starColInx = myGridCMS180.getColIndexById('StarStatus');
            var strInstallationStatusColInx = myGridCMS180.getColIndexById('InstallationStatus');
            var strSlipStatusColInx = myGridCMS180.getColIndexById('SlipStatus');
            var strSlipNoColInx = myGridCMS180.getColIndexById('SlipNo');
            var strContractStatusColInx = myGridCMS180.getColIndexById('ContractStatus');
            var strInstallationTypeColInx = myGridCMS180.getColIndexById('InstallationType');
            var NextStepButtonNameCoiInx = myGridCMS180.getColIndexById('NextStepButtonName');

            var RegisterTextButtonCoiInx = myGridCMS180.getColIndexById('CMS180_RegisterTextButton');
            var RegisterFlagButtonCoiInx = myGridCMS180.getColIndexById('CMS180_RegisterFlagButton');
            var ViewDetailCoiInx = myGridCMS180.getColIndexById('CMS180_ViewDetail');



            for (var i = 0; i < myGridCMS180.getRowsNum(); i++) {
                var rid = myGridCMS180.getRowId(i);
                var registerTextButton = "";
                var flag = true;
                var flagViewDetail = true;
                var RegisterButton = "";


// Move this logic to server for tuning up display time
               
//                var strInstallationStatus = myGridCMS180.cells(rid, strInstallationStatusColInx).getValue();
//                var strSlipStatus = myGridCMS180.cells(rid, strSlipStatusColInx).getValue();
//                var strSlipNo = myGridCMS180.cells(rid, strSlipNoColInx).getValue();
//                var strContractStatus = myGridCMS180.cells(rid, strContractStatusColInx).getValue();
//                var strInstallationType = myGridCMS180.cells(rid, strInstallationTypeColInx).getValue();
//                var flag = true;
//                var flagViewDetail = true;

//                if (gen_ctrl == true) {
//                    var RegisterButton = "";
//                    if (CMS180_ViewBag.C_INSTALL_STATUS_INSTALL_NOT_REQUESTED == strInstallationStatus) {
//                        registerTextButton = CMS180_ViewBag.C_INSTALL_STATUS_REQUEST
//                    }
//                    else if (CMS180_ViewBag.C_INSTALL_STATUS_INSTALL_REQUESTED_PO_NOT_REGISTERED == strInstallationStatus) {
//                        registerTextButton = CMS180_ViewBag.C_INSTALL_STATUS_PO
//                    }
//                    else if (CMS180_ViewBag.C_INSTALL_STATUS_INSTALL_REQUESTED_PO_REGISTERED == strInstallationStatus) {
//                        if (strSlipStatus == null || strSlipStatus == undefined || strSlipStatus == "") {
//                            registerTextButton = CMS180_ViewBag.C_INSTALL_STATUS_SLIP
//                        }
//                        else {
//                            registerTextButton = CMS180_ViewBag.C_INSTALL_STATUS_COMPLETE
//                            if (strSlipStatus != CMS180_ViewBag.C_SLIP_STATUS_NO_NEED_TO_STOCK_OUT && strSlipStatus != CMS180_ViewBag.C_SLIP_STATUS_STOCK_OUT) {
//                                flag = false;
//                            }
//                        }
//                    }
//                    //                    else if (CMS180_ViewBag.C_INSTALL_STATUS_UNDER_INSTALLATION == strInstallationStatus) {
//                    //                        registerTextButton = CMS180_ViewBag.C_INSTALL_STATUS_COMPLETE
//                    //                    }
//                    else if (CMS180_ViewBag.C_INSTALL_STATUS_COMPLETED == strInstallationStatus) {
//                        registerTextButton = CMS180_ViewBag.C_INSTALL_STATUS_REGISTER
//                        flag = false;
//                    }
//                    else if (CMS180_ViewBag.C_INSTALL_STATUS_INSTALL_CANCELLED == strInstallationStatus) {
//                        registerTextButton = CMS180_ViewBag.C_INSTALL_STATUS_REGISTER
//                        flag = false;
//                    }
//                    else if (strContractStatus == CMS180_ViewBag.C_CONTRACT_STATUS_CANCEL) {
//                        if (CMS180_ViewBag.C_INSTALL_STATUS_COMPLETED != strInstallationStatus
//                                && CMS180_ViewBag.C_INSTALL_STATUS_INSTALL_CANCELLED != strInstallationStatus
//                                && CMS180_ViewBag.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED != strInstallationStatus
//                                && CMS180_ViewBag.C_INSTALL_STATUS_NO_INSTALLATION != strInstallationStatus
//                                && CMS180_ViewBag.C_RENTAL_INSTALL_TYPE_REMOVE_ALL != strInstallationType
//                                && CMS180_ViewBag.C_SALE_INSTALL_TYPE_REMOVE_ALL != strInstallationType
//                                ) {
//                            registerTextButton = CMS180_ViewBag.C_INSTALL_STATUS_CANCEL
//                            flag = true;
//                        } else {
//                            registerTextButton = "-";
//                        }
//                    }
//                    else {
//                        registerTextButton = "-";
//                    }

//                    if (strSlipNo == undefined || strSlipNo == "" || strSlipNo == null) {
//                        flagViewDetail = false;
//                    }
//                    else {
//                        flagViewDetail = true;
//                    }
// End moving

                if (gen_ctrl == true) {
                    registerTextButton = myGridCMS180.cells(rid, RegisterTextButtonCoiInx).getValue();
                    if (registerTextButton == CMS180_ViewBag.C_INSTALL_STATUS_COMPLETE_CNT) {
                        registerTextButton = CMS180_ViewBag.C_INSTALL_STATUS_COMPLETE;
                    } else if (registerTextButton == CMS180_ViewBag.C_INSTALL_STATUS_REQUEST_CNT) {
                        registerTextButton = CMS180_ViewBag.C_INSTALL_STATUS_REQUEST;
                    } else if (registerTextButton == CMS180_ViewBag.C_INSTALL_STATUS_PO_CNT) {
                        registerTextButton = CMS180_ViewBag.C_INSTALL_STATUS_PO;
                    } else if (registerTextButton == CMS180_ViewBag.C_INSTALL_STATUS_SLIP_CNT) {
                        registerTextButton = CMS180_ViewBag.C_INSTALL_STATUS_SLIP;
                    } else if (registerTextButton == CMS180_ViewBag.C_INSTALL_STATUS_REGISTER_CNT) {
                        registerTextButton = CMS180_ViewBag.C_INSTALL_STATUS_REGISTER;
                    } else if (registerTextButton == CMS180_ViewBag.C_INSTALL_STATUS_CANCEL_CNT) {
                        registerTextButton = CMS180_ViewBag.C_INSTALL_STATUS_CANCEL;
                    }

                    flag = myGridCMS180.cells(rid, RegisterFlagButtonCoiInx).getValue();
                    flagViewDetail = myGridCMS180.cells(rid, ViewDetailCoiInx).getValue();

                    
                    //----------- Generate Register & Detail button
                    GenerateDetailButton(myGridCMS180, "btnDetail", rid, "Detail", flagViewDetail);
                    if (registerTextButton != "-" && registerTextButton != "") {
                        RegisterButton = GenerateHtmlButton("btnRegister", rid, registerTextButton, flag);
                    }
                    else {
                        RegisterButton = "-";
                    }
                    myGridCMS180.cells(rid, registerColInx).setValue(RegisterButton);
                    myGridCMS180.cells(rid, NextStepButtonNameCoiInx).setValue(registerTextButton);

                    if (myGridCMS180.cells(rid, starColInx).getValue() == "*C") {
                        myGridCMS180.cells(rid, starColInx).setValue(GenerateStarImage());
                        myGridCMS180.setRowColor(rid, "#ff9999");
                    } else if (myGridCMS180.cells(rid, starColInx).getValue() == "*N") {
                        myGridCMS180.cells(rid, starColInx).setValue(GenerateStarImage());
                    }
                }


                /* ===== Detail Action Button ===== */
                BindGridButtonClickEvent("btnDetail", rid, doSelectResultGrid);

                /* ===== Register Action Button ===== */
                BindGridButtonClickEvent("btnRegister", rid, doSelectRegisterButton);

            }
            //$("#chkHeader").unbind("click");
            //$("#chkHeader").click(selectAllCheckboxControl);
            myGridCMS180.setSizes();
        }
    });
}
function InitialSubcontractorGrid() {
    //subcontractorGrid = $("#gridSubContractor").InitialGrid(0, true, "/Common/CMS180_InitialSubcontractor");
    subcontractorGrid = $("#gridSubContractor").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Common/CMS180_InitialSubcontractor");
}
function InitialInstrumentDetailGrid() {
    instrumentDetailGrid = $("#gridInstrumentDetail").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Common/CMS180_InitialInstrumentDetail");
}


function setValueToSiteInfoWithInitial() {
    if (resultInstallationGrid.getRowsNum() >= 0) {

        /* ==== Create json object for string json ==== */
        if (CheckFirstRowIsEmpty(resultInstallationGrid) == false) {
            var strJson = resultInstallationGrid.cells2(0, resultInstallationGrid.getColIndexById('ToJson')).getValue().toString();
            strJson = htmlDecode(strJson);
            var objSelcted = JSON.parse(strJson); // require json2.js
            objResult = objSelcted;
            $("#divSiteInfo").show();
            $("#divShowDetail").show();
            $("#divInstrumentDetail").show();

            setValurtoControl(objSelcted);
            $("#divSearchResult").show();
        }
        else {
            //$("#divSearchResult").hide();
            $("#divSiteInfo").hide();
            $("#divShowDetail").hide();
            $("#divInstrumentDetail").hide();

        }
    }
}
function doSelectResultGrid(row_id) {
    master_event.LockWindow(true);
    resultInstallationGrid.selectRow(resultInstallationGrid.getRowIndex(row_id));
    if (resultInstallationGrid.getRowsNum() >= 0) {

        /* ==== Create json object for string json ==== */
        var strJson = resultInstallationGrid.cells2(resultInstallationGrid.getRowIndex(row_id), resultInstallationGrid.getColIndexById('ToJson')).getValue().toString();
        strJson = htmlDecode(strJson);
        var objSelcted = JSON.parse(strJson); // require json2.js
        objResult = objSelcted;
        document.getElementById('divSiteInfo').scrollIntoView();
        setValurtoControl(objSelcted);

        master_event.LockWindow(false);
        master_event.ScrollWindow("#divSiteInfo", false);
    }
}
function setValurtoControl(obj) {
    //----- Site info ----//
    $("#divSiteInfo").bindJSON(obj);
    $("#ContractCode1").val(obj.ContractCode_Short_Text);
    $("#UserCodeSiteInfo").val(obj.UserCode);
    $("#OperationOfficeSiteInfo").val(obj.OperationOfficeName);


    //----- Show Detail Info -----//

    $("#InstallationMaintenanceNoDetail").val(obj.MaintenanceNo);
    GetMemo(obj);
    GenerateHistoryCombo(obj);

    //---- Subcontractor ---//
    SubcontractorGrid(obj);

    $("#divSiteInfo").show();
    $("#divShowDetail").show();
    $("#divInstrumentDetail").show();
}

function GenerateHistoryCombo(data) {
    var obj = { "ContractProjectCode": data.ContractProjectCode,
        "MaintenanceNo": data.MaintenanceNo,
        "SlipNo": data.SlipNo
    };
    ajax_method.CallScreenController("/Common/CMS180_InstallationHistory", obj,
     function (result, controls) {
         if (result != undefined) {
             update_history_combo(result);
         }
     });
}
function update_history_combo(data) {
    regenerate_combo("#SlipNo", data);
    $("#SlipNo>option").last().attr("selected", true);
    slipno_change();
}

function slipno_change() {
    var obj = { "ContractProjectCode": objResult.ContractProjectCode,
        "MaintenanceNo": objResult.MaintenanceNo,
        "SlipNo": $("#SlipNo").val()
    };
    ajax_method.CallScreenController("/Common/CMS180_GetInstallationSlipHistory", obj,
     function (result, controls) {
         if (result != undefined) {
             //$("#divShowDetail").clearForm();
             setValueSlipHistory(result);
         }
         else {
             ClearSlipHistory();

             //InitialInstrumentDetailGrid();
             DeleteAllRow(instrumentDetailGrid); //Modify by Jutarat A. on 26022014
             DeleteAllRow(subcontractorGrid); //Add by Jutarat A. on 26022014
         }

         CMS180.Functions.RetrieveAttachFileData();

     });
}

function setValueSlipHistory(result) {

    $("#InstallationMaintenanceNoDetail").val(result.MaintenanceNo);
    $("#PlanCodeDetailInfo").val(result.PlanCode);
    $("#SlipStatus").val(result.InstallationStatusName);
    $("#SalesmanEmpNo1").val(result.SalesmanEmpNo1);
    $("#SalesmanEmpName1").val(result.SalesmanEmpName1);
    $("#SalesmanEmpNo2").val(result.SalesmanEmpNo2);
    $("#SalesmanEmpName2").val(result.SalesmanEmpName2);
    $("#InstallationTypeName").val(result.InstallationTypeName);
    $("#InstallationCauseReason").val(result.CauseReasonName);
    $("#NormalContractFee").val(result.NormalContractFeeNumeric);
    $("#NormalContractFee").SetNumericCurrency(result.NormalContractFeeCurrencyType);
    $("#NormalInstallFee").val(result.NormalInstallFeeNumeric);
    $("#NormalInstallFee").SetNumericCurrency(result.NormalInstallFeeCurrencyType);
    $("#InstallationFeeBillingFlag").val(result.InstallationFeeBillingFlag);
    $("#BillingInstallFee").val(result.BillingInstallFeeNumeric);
    $("#BillingInstallFee").SetNumericCurrency(result.BillingInstallFeeCurrencyType);
    $("#BillingOCC").val(result.BillingOCC);

    if (result.NormalInstallFeeCurrencyType != result.BillingInstallFeeCurrencyType) {
        $("#SecomPayment").val('');
        $("#SecomPayment").NumericCurrency().empty();

        $("#SecomRevenue").val('');
        $("#SecomRevenue").NumericCurrency().empty();
    } else {
        $("#SecomPayment").val(result.SecomPaymentNumeric);
        $("#SecomPayment").SetNumericCurrency(result.SecomPaymentCurrencyType);

        $("#SecomRevenue").val(result.SecomRevenueNumeric);
        $("#SecomRevenue").SetNumericCurrency(result.SecomRevenueCurrencyType);
    }

    $("#ApproveNo1").val(result.ApproveNo1);
    $("#ApproveNo2").val(result.ApproveNo2);
    $("#SlipIssueDate").val(result.SlipIssueDate_Text);
    $("#InstrumentStockOutDate").val(result.StockOutDate_Text);
    $("#InstallationCompleteDate").val(result.InstallationCompleteDate_Text);
    $("#ReturnInstrumentReceiveDate").val(result.ReturnReceiveDate_Text);

    if (result.ChangeReasonTypeCode == '00') {
        $("#rdoCustomerReason").attr('checked', true);
    }
    else if (result.ChangeReasonTypeCode == '01') {
        $("#rdoSecomReason").attr('checked', true);
    }
    else {
        $("#rdoCustomerReason").attr('checked', true);
    }

    GetMemo(result); //Add by Jutarat A. on 26022014
    SubcontractorGrid(result); //Add by Jutarat A. on 26022014

    setValueinstrumentDetailGrid(result);
}

function setValueinstrumentDetailGrid(result) {
    if ($("#gridInstrumentDetail").length > 0) {
        $("#gridInstrumentDetail").LoadDataToGrid(instrumentDetailGrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Common/CMS180_InstrumentDetailGrid", result, "dtInstallationSlipDetailsForView", false,
                 function () {
                     //document.getElementById('divBillingTargetInformationSearchResult').scrollIntoView();

                 },
                null);
    }
}

function SubcontractorGrid(result) {

    if ($("#gridSubContractor").length > 0) {
        $("#gridSubContractor").LoadDataToGrid(subcontractorGrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Common/CMS180_SubcontractorGrid", result, "dtInstallationPOManagementForView", false,
                 function () {
                     //document.getElementById('divBillingTargetInformationSearchResult').scrollIntoView();
                     //setValueToSiteInfoWithInitial();
                 },
                null);
    }
}
function GetMemo(result) {
    var obj = {
        "ContractProjectCode": result.ContractProjectCode ,
        "MaintenanceNo": result.MaintenanceNo,
        "SlipNo": result.SlipNo
    };
    ajax_method.CallScreenController("/Common/CMS180_GetInstallationMemoForview", obj,
     function (result, controls) {
         if (result != undefined) {
             //update_history_combo(result);
             var strMemo = "";
             for (var i = 0; i < result.length; i++) {
                 strMemo += (result[i].Memo_Text + "\r\n");
             }
             $("#Memo").val(strMemo);
         }
     });
}

function Search() {
    // disable search button
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    var parameter = CreateObjectData($("#SearchCriteria").serialize()); // + "&Counter=" + CMS270_Count, true);
    parameter.SlipNo = $("#InstallationSlipNo").val();

    $("#gridResultList").LoadDataToGrid(resultInstallationGrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Common/CMS180_SearchResult", parameter, "dtInstallation", false,
        function () {

            master_event.ScrollWindow("#divSearchResult", false);
            //document.getElementById('divSearchResult').scrollIntoView();
            //setValueToSiteInfoWithInitial();
            if ((CMS180_ViewBag.ContractCode != undefined && CMS180_ViewBag.ContractCode != null && CMS180_ViewBag.ContractCode != "")
             || (CMS180_ViewBag.InstallationSlipNo != undefined && CMS180_ViewBag.InstallationSlipNo != null && CMS180_ViewBag.InstallationSlipNo != "")) {

                setValueToSiteInfoWithInitial();
                $("#divSiteInfo").show();
                $("#divShowDetail").show();
                $("#divInstrumentDetail").show();
            }
            else {
                $("#divSiteInfo").hide();
                $("#divShowDetail").hide();
                $("#divInstrumentDetail").hide();
            }
            $("#btnSearch").attr("disabled", false);
            master_event.LockWindow(false);
        },
        function () {
            //--------------------------------
            $("#divSearchResult").show();
            ////----------
        });
}
function Clear() {

    $("#divSearchCriteria").clearForm();
    $("#divSiteInfo").clearForm();
    $("#divShowDetail").clearForm();

    $("#divSiteInfo").hide();
    $("#divShowDetail").hide();
    $("#divInstrumentDetail").hide();
    //$("#divSearchResult").hide();
    defaultCheck();
    //$("#rdoCustomerReason").attr('checked', true);
    $("#rdoCustomerReason").attr('disabled', true);
    $("#rdoSecomReason").attr('disabled', true);
}
function doSelectRegisterButton(row_id) {
    resultInstallationGrid.selectRow(resultInstallationGrid.getRowIndex(row_id));
    if (resultInstallationGrid.getRowsNum() >= 0) {

        var btnTextName = resultInstallationGrid.cells2(resultInstallationGrid.getRowIndex(row_id), resultInstallationGrid.getColIndexById('NextStepButtonName')).getValue().toString();
        var ContractCode = resultInstallationGrid.cells2(resultInstallationGrid.getRowIndex(row_id), resultInstallationGrid.getColIndexById('ContractCode_Short_Text')).getValue().toString();
        var obj = { strContractCode: ContractCode,
            strContractProjectCode: ContractCode
        };
        if (btnTextName == CMS180_ViewBag.C_INSTALL_STATUS_REQUEST) {
            ajax_method.CallScreenControllerWithAuthority("/Installation/ISS010", obj, true);
        }
        else if (btnTextName == CMS180_ViewBag.C_INSTALL_STATUS_PO) {
            ajax_method.CallScreenControllerWithAuthority("/Installation/ISS050", obj, true);
        }
        else if (btnTextName == CMS180_ViewBag.C_INSTALL_STATUS_SLIP) {
            ajax_method.CallScreenControllerWithAuthority("/Installation/ISS030", obj, true); /// strContractCode
        }
        else if (btnTextName == CMS180_ViewBag.C_INSTALL_STATUS_COMPLETE) {
            ajax_method.CallScreenControllerWithAuthority("/Installation/ISS060", obj, true);
        }
        else if (btnTextName == CMS180_ViewBag.C_INSTALL_STATUS_CANCEL) {
            ajax_method.CallScreenControllerWithAuthority("/Installation/ISS070", obj, true);
        }

    }
}

function download_form() {
    ajax_method.CallScreenController("/Common/CMS180_GetInstallationForCSV", "",
     function (result, controls) {
         if (result != undefined) {
             var url = "/Common/CMS180_DownloadAsCSV";
             download_method.CallDownloadController("ifDownload", url, null); //result); //Modify by Jutarat A. on 25062013
         }
    }, false);

    //download_method.CallDownloadController("ifDownload", "/Common/CMS180_DownloadAsCSV", "");
}

function defaultCheck() {
//    $("#chkInstallationNotRegistered").attr('checked', true);
//    $("#chkInstallationRequestedAndPoRegistered").attr('checked', true);
//    $("#chkInstallationNotRequest").attr('checked', true);
//    $("#chkInstallationCompleted").attr('checked', false);
//    $("#chkInstallationRequestButPoNotRegistered").attr('checked', true);
//    $("#chkInstallationCancelled").attr('checked', false);

//    $("#chkNotStockOut").attr('checked', true);
//    $("#chkNoNeedToStockOut").attr('checked', true);
//    $("#chkReturned").attr('checked', true);
//    $("#chkPartialStockOut").attr('checked', true);
//    $("#chkInstallationSlipCanceled").attr('checked', true);
//    $("#chkNoNeedToReturn").attr('checked', true);
//    $("#chkStockOut").attr('checked', true);
//    $("#chkWaitForReturn").attr('checked', true);
//    $("#chkReplaced").attr('checked', true);

//    $("#chkProcessing").attr('checked', true);
//    $("#chkApproved").attr('checked', true);
//    $("#chkCompleted").attr('checked', true);
//    $("#chkRequestApprove").attr('checked', true);
//    $("#chkRejected").attr('checked', true);
//    $("#chkCanceled").attr('checked', true);

    // Modify by Sommai P., Nov 4, 2013 for default checkbox to checked
    $("#chkInstallationNotRegistered").attr('checked', true);
    $("#chkInstallationRequestedAndPoRegistered").attr('checked', true);
    $("#chkInstallationNotRequest").attr('checked', true);
    $("#chkInstallationUnderInstall").attr('checked', true); //Add by Jutarat A. on 27032014
    $("#chkInstallationCompleted").attr('checked', true);
    $("#chkInstallationRequestButPoNotRegistered").attr('checked', true);
    $("#chkInstallationCancelled").attr('checked', true);

    $("#chkNotStockOut").attr('checked', true);
    $("#chkNoNeedToStockOut").attr('checked', true);
    $("#chkReturned").attr('checked', true);
    $("#chkPartialStockOut").attr('checked', true);
    $("#chkInstallationSlipCanceled").attr('checked', true);
    $("#chkNoNeedToReturn").attr('checked', true);
    $("#chkStockOut").attr('checked', true);
    $("#NotRegisteredYetSlipFlag").attr('checked', true);
    $("#chkWaitForReturn").attr('checked', true);
    $("#chkReplaced").attr('checked', true);

    $("#chkProcessing").attr('checked', true);
    $("#chkApproved").attr('checked', true);
    $("#chkCompleted").attr('checked', true);
    $("#chkRequestApprove").attr('checked', true);
    $("#chkRejected").attr('checked', true);
    $("#chkCanceled").attr('checked', true);
    $("#NotRegisteredYetManagementFlag").attr('checked', true);
    // End Modify
}

function ClearSlipHistory() {
    //$("#InstallationMaintenanceNoDetail").val(result.MaintenanceNo);
    $("#PlanCodeDetailInfo").val("");
    $("#SlipStatus").val("");
    $("#SalesmanEmpNo1").val("");
    $("#SalesmanEmpName1").val("");
    $("#SalesmanEmpNo2").val("");
    $("#SalesmanEmpName2").val("");
    $("#InstallationTypeName").val("");
    $("#InstallationCauseReason").val("");
    $("#NormalContractFee").val("");
    $("#NormalInstallFee").val("");
    $("#InstallationFeeBillingFlag").val("");
    $("#BillingInstallFee").val("");
    $("#BillingOCC").val("");
    $("#SecomPayment").val("");
    $("#SecomRevenue").val("");
    $("#ApproveNo1").val("");
    $("#ApproveNo2").val("");
    $("#SlipIssueDate").val("");
    $("#InstrumentStockOutDate").val("");
    $("#InstallationCompleteDate").val("");
    $("#ReturnInstrumentReceiveDate").val("");
    $("#rdoCustomerReason").attr('checked', false);
    $("#rdoSecomReason").attr('checked', false);
    $("#Memo").val(""); //Add by Jutarat A. on 26022014
}