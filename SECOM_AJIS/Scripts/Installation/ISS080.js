

//  jQuery
/// <reference path="../../Scripts/jquery-1.5.1-vsdoc.js"/>

// dhtmlx grid
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgrid.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_data.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgridcell.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js"/>
/// <reference path = "../../Scripts/Base/Master.js" />
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Common/Dialog.js"/>
/// <reference path="../../Scripts/json2.js" />


/// <reference path = "../../Scripts/Base/GridControl.js" />

var pageRow = ROWS_PER_PAGE_FOR_SEARCHPAGE;
var ISS080_GridEmail;
var ISS080_GridInstallationManagement;
var strNewRow = "NEWROW";
var strTrue = "true";
var strFalse = "false";

var conditionCancelAll = "CANCELALL";
var conditionCancelPO = "CANCELPO";
var conditionCancelPOAndSlip = "CANCELPOANDSLIP";
var conditionCancelSlipUsePrevious = "CANCELSLIPUSEPREVIOUS";
var conditionCancelSlip = "CANCELSLIP";

// Main
$(document).ready(function () {
    
    //$("#btnSearch").click(retrieve_installation_click);
    $("#btnSearch").click(retrieve_datatogrid);
    
    $("#btnClear").click(clear_condition_click);
    InitialDateFromToControl("#ProposedInstallationCompleteDateFrom", "#ProposedInstallationCompleteDateTo");
    InitialDateFromToControl("#InstallationCompleteDateFrom", "#InstallationCompleteDateTo");
    InitialDateFromToControl("#InstallationStartDateFrom", "#InstallationStartDateTo");
    InitialDateFromToControl("#InstallationFinishDateFrom", "#InstallationFinishDateTo");
    InitialDateFromToControl("#ExpectedInstallationStartDateFrom", "#ExpectedInstallationStartDateTo");
    InitialDateFromToControl("#ExpectedInstallationFinishDateFrom", "#ExpectedInstallationFinishDateTo");

    InitialDateFromToControl("#InstallationRequestDateFrom", "#InstallationRequestDateTo"); //Add by Jutarat A. on 22102013

    $("#SiteName").InitialAutoComplete("/Master/GetSiteName");
    $("#SiteAddress").InitialAutoComplete("/Master/GetSiteAddress");
    // intial grid
    ISS080_GridInstallationManagement = $("#gridInstallationManagement").InitialGrid(pageRow, true, "/Installation/ISS080_InitialGridManagementList");
    SpecialGridControl(ISS080_GridInstallationManagement, ["Detail"]);
    /* ===== binding event when finish load data ===== */
    BindOnLoadedEvent(ISS080_GridInstallationManagement, function () {
        var colInx = ISS080_GridInstallationManagement.getColIndexById('Detail');
        for (var i = 0; i < ISS080_GridInstallationManagement.getRowsNum(); i++) {
            var rowId = ISS080_GridInstallationManagement.getRowId(i);
            GenerateDetailButton(ISS080_GridInstallationManagement, "btnDetail", rowId, "Detail", true);

            // binding grid button event 
            BindGridButtonClickEvent("btnDetail", rowId, ViewInstallationManagement);

        }
        ISS080_GridInstallationManagement.setSizes();
        $("#divInstallationInstrumentInfo").show();
    });


//    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
//    call_ajax_method_json("/Installation/ISS080_GetAllInstallationtype", obj, function (result, controls) {

//        if (result.List.length != 1) {
//            regenerate_combo("#InstallationType", result);
//        }
//    });

    $("#All").attr("checked",true)
    $("#divInstallManageInfo").hide();
});

function clear_condition_click() { 
    $("#divSearchCondition").clearForm();
    $("#All").attr("checked",true)
    DeleteAllRow(ISS080_GridInstallationManagement);
    ISS080_GridInstallationManagement.setSizes();
    $("#divInstallManageInfo").hide();

    
    ClearDateFromToControl("#InstallationCompleteDateFrom", "#InstallationCompleteDateTo");
    ClearDateFromToControl("#InstallationStartDateFrom", "#InstallationStartDateTo");
    ClearDateFromToControl("#InstallationFinishDateFrom", "#InstallationFinishDateTo");
    ClearDateFromToControl("#ExpectedInstallationStartDateFrom", "#ExpectedInstallationStartDateTo");
    ClearDateFromToControl("#ExpectedInstallationFinishDateFrom", "#ExpectedInstallationFinishDateTo");

    ClearDateFromToControl("#InstallationRequestDateFrom", "#InstallationRequestDateTo"); //Add by Jutarat A. on 22102013
}

function retrieve_installation_click() {

    
    DeleteAllRow(ISS080_GridInstallationManagement);
    ISS080_GridInstallationManagement.setSizes();
    //var obj = { strContractCode: $("#ContractCodeProjectCode").val() };

    
    var obj = CreateObjectData($("#form1").serialize());
    call_ajax_method_json("/Installation/ISS080_RetrieveData", obj,
        function (result, controls) {
            if (controls != undefined) {

                //                $("#divContractBasicInfo").clearForm();
                //                $("#divInstallationInstrumentInfo").clearForm();
                //                $("#divInstallationInfo").clearForm();
                //                $("#divRegisterComplete").clearForm();
                //                $("#divCancelCondition").clearForm();
                //                $("#divProjectInfo").clearForm();

                return;
            }
            else if (result != undefined) {
                
                /////////////// BIND  DATA //////////////////
                if (result.doSearchData != null) {
                $("#divInstallManageInfo").show();
                    if (result.doSearchData.length > 0) {
                        document.getElementById('divInstallManageInfo').scrollIntoView(true);
                        for (var i = 0; i < result.doSearchData.length; i++) {

                            if (result.doSearchData[i].ContractProjectCode == null)
                                result.doSearchData[i].ContractProjectCode = "-";

                            
                            if (result.doSearchData[i].InstallationType == null) {
                                result.doSearchData[i].InstallationType = "-";
                            }
                            
                            if (result.doSearchData[i].IEStaffName1 == null)
                                result.doSearchData[i].IEStaffName1 = "-";
                            if (result.doSearchData[i].IEStaffName2 == null)
                                result.doSearchData[i].IEStaffName2 = "-";
                            if (result.doSearchData[i].SubcontractorName == null)
                                result.doSearchData[i].SubcontractorName = "-";
                            if (result.doSearchData[i].SubcontractorGroupName == null)
                                result.doSearchData[i].SubcontractorGroupName = "-";

                            if (result.doSearchData[i].ProposedInstallationCompleteDate == null)
                                result.doSearchData[i].ProposedInstallationCompleteDate = "-";
                            else
                                result.doSearchData[i].ProposedInstallationCompleteDate = ConvertDateToTextFormat(result.doSearchData[i].ProposedInstallationCompleteDate.replace('/Date(', '').replace(')/', '') * 1)

                            if (result.doSearchData[i].ActualInstallationCompleteDate == null)
                                result.doSearchData[i].ActualInstallationCompleteDate = "-";
                            else
                                result.doSearchData[i].ActualInstallationCompleteDate = ConvertDateToTextFormat(result.doSearchData[i].ActualInstallationCompleteDate.replace('/Date(', '').replace(')/', '') * 1)

                            if (result.doSearchData[i].InstallationStartDate == null)
                                result.doSearchData[i].InstallationStartDate = "-";
                            else
                                result.doSearchData[i].InstallationStartDate = ConvertDateToTextFormat(result.doSearchData[i].InstallationStartDate.replace('/Date(', '').replace(')/', '') * 1)

                            if (result.doSearchData[i].InstallationFinishDate == null)
                                result.doSearchData[i].InstallationFinishDate = "-";
                            else
                                result.doSearchData[i].InstallationFinishDate = ConvertDateToTextFormat(result.doSearchData[i].InstallationFinishDate.replace('/Date(', '').replace(')/', '') * 1)

                            if (result.doSearchData[i].SiteCode == null)
                                result.doSearchData[i].SiteCode = "-";
                            if (result.doSearchData[i].SiteNameEN == null)
                                result.doSearchData[i].SiteNameEN = "-";
                            if (result.doSearchData[i].SiteNameLC == null)
                                result.doSearchData[i].SiteNameLC = "-";
                            if (result.doSearchData[i].OperationOfficeName == null)
                                result.doSearchData[i].OperationOfficeName = "-";
                            if (result.doSearchData[i].InstallationManagementStatus == null)
                                result.doSearchData[i].InstallationManagementStatus = "-";

                            var List = [i+1, 
                            result.doSearchData[i].ContractProjectCode + "<br />" + result.doSearchData[i].InstallationType, 
                            "(1) "+result.doSearchData[i].IEStaffName1 + "<br />" + "(2) "+result.doSearchData[i].IEStaffName2, 
                            "(1) "+result.doSearchData[i].SubcontractorName + "<br />" + "(2) "+result.doSearchData[i].SubcontractorGroupName, 
                            "(1) "+result.doSearchData[i].InstallationStartDate + "<br />" + "(2) "+result.doSearchData[i].InstallationFinishDate + "<br />(3) "+result.doSearchData[i].ProposedInstallationCompleteDate + "<br />" + "(4) "+result.doSearchData[i].ActualInstallationCompleteDate, 
                            "(1) "+result.doSearchData[i].SiteCode + "<br />" + "(2) "+result.doSearchData[i].SiteNameEN + "<br />" + "(3) "+result.doSearchData[i].SiteNameLC, 
                            result.doSearchData[i].OperationOfficeName, 
                            result.doSearchData[i].InstallationManagementStatus, "", result.doSearchData[i].MaintenanceNo];

                            CheckFirstRowIsEmpty(ISS080_GridInstallationManagement, true);
                            AddNewRow(ISS080_GridInstallationManagement, List);

                        }
                       
                        $("#gridInstallationManagement").show();           
                        $("#divInstallManageInfo").show();
                        SetPagingSection("gridInstallationManagement", ISS080_GridInstallationManagement, pageRow, true);
                        var paging_name = "gridInstallationManagement_paging";
                        var paging_info_name = "gridInstallationManagement_info_paging";
                        
                        $("#" + paging_name).show();
                        $("#" + paging_info_name).show();
                        
                        ISS080_GridInstallationManagement.setSizes();
                         
                        
                    }
                }
                else
                {
                    DeleteAllRow(ISS080_GridInstallationManagement);
                    var paging_name = "gridInstallationManagement_paging";
                    var paging_info_name = "gridInstallationManagement_info_paging";
                                        
                    $("#" + paging_name).hide();
                    $("#" + paging_info_name).hide();
                  
                    $("#divInstallManageInfo").show();
                   
                    ISS080_GridInstallationManagement.setSizes();
                }
                
                
            }

        }
    );
}

function retrieve_datatogrid() {

     DeleteAllRow(ISS080_GridInstallationManagement);
    ISS080_GridInstallationManagement.setSizes();    
    var obj = CreateObjectData($("#form1").serialize());
    //ISS080_GridInstallationManagement = $("#gridInstallationManagement").LoadDataToGridWithInitial(pageRow, false, false, "/Installation/ISS080_SearchDataToGrid", obj, "doSearchInstallManagementResult", false);
    master_event.LockWindow(true);
    $("#gridInstallationManagement").LoadDataToGrid(ISS080_GridInstallationManagement, pageRow, true, "/Installation/ISS080_SearchDataToGrid", obj, "doSearchInstallManagementResult", false, null,
            function (result, controls, isWarning) {
                master_event.LockWindow(false);
                if (isWarning == undefined) {
                    $("#divInstallManageInfo").show();
                   
                    //setTimeout("document.getElementById('divInstallManageInfo').scrollIntoView()",1500);
                    setTimeout('master_event.ScrollWindow("#divInstallManageInfo", false);',1500);
                    
                }
            }
        );
 
}

function showPaging(){
    SetPagingSection("gridInstallationManagement", ISS080_GridInstallationManagement, pageRow, true);
    var paging_name = "gridInstallationManagement_paging";
    var paging_info_name = "gridInstallationManagement_info_paging";
                        
    $("#" + paging_name).show();
    $("#" + paging_info_name).show();
}


function ViewInstallationManagement(rowId) {
    var MaintenanceNoCol = ISS080_GridInstallationManagement.getColIndexById("MaintenanceNo");
    var strMaintenanceNo = GetValueFromLinkType(ISS080_GridInstallationManagement, ISS080_GridInstallationManagement.getRowIndex(rowId), MaintenanceNoCol);
    
    var obj = {
        MaintenanceNo: strMaintenanceNo,        
    };
    ajax_method.CallScreenControllerWithAuthority("/Installation/ISS090", obj, true);
}

function doAlert(moduleCode, msgCode, paramObj) {
    var obj = {
        module: moduleCode,
        code: msgCode,
        param: paramObj
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenWarningDialog(result.Message, result.Message, null);
    });
}

    