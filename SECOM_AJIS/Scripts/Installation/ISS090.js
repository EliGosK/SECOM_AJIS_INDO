

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

var pageRow = 0;
var ISS090_GridEmail;
var ISS090_GridEmail2;
var ISS090_GridPOInfo;
var ISS090_GridAttachedDoc;
var ISS090_GridManagementInfo;

var enableViewButton;
var enableEditButton;
var enablePaidButton;
var enableIssueButton;
var DivTotal = '<DIV style="TEXT-ALIGN: right; COLOR: blue">' + lblTotal + '</DIV>';

var ISS090_gridAttach;
var isInitAttachGrid = false;

var blnHavetbtInstallationBasic = false;
var blnHaveContractListByProject = false;

var hasAlert = false;
var alertMsg = "";
var permissionDownload = false;
var permissionAdvancePay = false;

// Main
$(document).ready(function () {
    //================= Check permission download =======================
    var obj2 = {
                        ModeFunction: C_FUNC_ID_DOWNLOAD
                
                };
    call_ajax_method_json("/Installation/ISS090_CheckScreenModePermission", obj2, function (result) {                     
            if(result == C_FUNC_ID_DOWNLOAD)
            {                         
                permissionDownload = true;
            }
            else
            {
                permissionDownload = false;
            }                          
    });
    //===================================================================
    //================= Check permission Advance Payment Flag =======================
    obj2 = {
                        ModeFunction: C_FUNC_ID_INSTALL_ADVANCE_PAY
                
                };
    call_ajax_method_json("/Installation/ISS090_CheckScreenModePermission", obj2, function (result) {                     
            if(result == C_FUNC_ID_INSTALL_ADVANCE_PAY)
            {                         
                permissionAdvancePay = true;
            }
            else
            {
                permissionAdvancePay = false;
            }                          
    });
    //===================================================================
    var strContractProjectCode = $("#ContractCodeProjectCode").val();

    $("#divInputContractCode").hide();
    //$("#NewBldMgmtCost").BindNumericBox(10, 2, 0, 9999999999, 0);

    $("#LastInstallationfee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#MaterialFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);

    $("#ProfitAmount").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#TotalLastInstallFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);

    $("#NormalInstallFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#BillingInstallFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#SecomPayment").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#ManPower").BindNumericBox(10, 0, 0, 9999, 0);
    $("#IEManPower").BindNumericBox(10, 0, 0, 9999, 0);

    $("#InstallationMemoProjectHistory").SetMaxLengthTextArea(4000);
    $("#InstallationMemo").SetMaxLengthTextArea(4000);

    $("#ActualPOAmount").BindNumericBox(12, 2, 0, 999999999999.99, 0); //Add by Jutarat A. on 08112013

    //Add by Jutarat A. on 10102013
    $("#InstallationFee1").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#InstallationFee2").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#InstallationFee3").BindNumericBox(12, 2, 0, 999999999999.99, 0);

    $("#IMFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#OtherFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);

    $("#IMRemark").SetMaxLengthTextArea(4000);
    //End Add

    $("#btnRetrieveInstallation").click(retrieve_installation_click);
    $("#btnClearInstallation").click(clear_installation_click);

    $("#CheckChargeIECode").blur(function () {
        if ($("#CheckChargeIECode").val() != "") {
            loadEmpName($("#CheckChargeIECode").val(), $("#CheckChargeIECode"), $("#CheckChargeIEName"));
        }
        else {
            $("#CheckChargeIEName").val("");
        }
    });

    // ** tt
    //$("#btnAdd").click(function () { BtnAddClick(); });

    $("#btnAdd").click(function () { BtnAddClick(); });
    $("#btnPOAdd").click(function () { BtnAddPOInfoClick(); });
    $("#btnPOClear").click(function () { BtnClearPOInfoClick(); });

    $("#btnUpdate").click(function () {
        //UpdateInstallManagementSubsection();
        var rowIndex = $("#SelectedRowIndex").val();
        var colLastInstallationFee = ISS090_GridManagementInfo.getColIndexById("LastInstallationFee");

        var obj = {
            MaintenanceNo: $("#MaintenanceNo").val(),
            rowIndex: rowIndex,
            LastInstallationFee: $("#LastInstallationfee").NumericValue()
        };

        call_ajax_method_json("/Installation/ISS090_UpdateLastInstallation", obj, function (result, controls) {
            ISS090_GridManagementInfo.cells2(rowIndex, colLastInstallationFee).setValue(result[rowIndex].TextUpdateLastInstallation);
        });

        loadEmpNameBeforeUpdate($("#CheckChargeIECode").val(), $("#CheckChargeIECode"), $("#CheckChargeIEName"));
        command_control.CommandControlMode(true);
    });
    $("#btnCancel").click(function () { 
        CancelInstallManagementSubsection();
        ManualInitialGridManagementInfo(false); 
        command_control.CommandControlMode(true);
    });

    $("#IEFirstCheckDate").change(function () { changeIEFirstCheckDate(); });
    $("#Adjustment").change(function () { changeAdjustment(); });
    //$("#IEEvaluation").change(function () { changeIEEvaluation(); }); //Comment by Jutarat A. on 07112013

    $("#ChangeReasonCode").change(function () {
        ChangeReasonCode_Change();
    });
    $("#ChangeRequestorCode").change(function () {
        ChangeRequestorCode_Change();
    });


    $("#btnClear").click(function () { BtnClearClick(); });
    $("#btnSearchEmail").click(function () { $("#dlgCTS053").OpenCMS060Dialog("CTS053"); });

    $("#IEStaffEmpNo1").blur(function () {
        if ($("#IEStaffEmpNo1").val() != "") {
            loadEmpName($("#IEStaffEmpNo1").val(), $("#IEStaffEmpNo1"), $("#EmpFullName1"));
        }
        else {
            $("#EmpFullName1").val("");
        }
    });

    $("#IEStaffEmpNo2").blur(function () {
        if ($("#IEStaffEmpNo2").val() != "") {
            loadEmpName($("#IEStaffEmpNo2").val(), $("#IEStaffEmpNo2"), $("#EmpFullName2"));
        }
        else {
            $("#EmpFullName2").val("");
        }
    });

    initialGridOnload(initialAfterGridOnload);
});

function initialAfterGridOnload() {
    var strContractProjectCode = $("#ContractCodeProjectCode").val();

    SpecialGridControl(ISS090_GridManagementInfo, ["AdvancePaymentFlag","ButtonEdit","ButtonIssue","ButtonPaid"]);

    $("#NormalSubPOAmount").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#ActualPOAmount").BindNumericBox(12, 2, 0, 999999999999.99, 0);

    if ($("#ExpectInstallStartDate").length > 0) {
        InitialDateFromToControl("#ExpectInstallStartDate", "#ExpectInstallCompleteDate");
    }
    InitialDateFromToControl("#InstallationStartDate", "#InstallationCompleteDate");
    InitialDateFromToControl("#ExpectInstallationStartDate", "#ExpectInstallationCompleteDate");
    $("#IEFirstCheckDate").InitialDate();
    $("#LastInspectionDate").InitialDate();

    //Add by Jutarat A. on 10102013
    $("#chkNoCheck").click(NoCheck_click);

    $("#PaidDate1").InitialDate();
    $("#PaidDate2").InitialDate();
    $("#PaidDate3").InitialDate();
    $("#LastPaidDate").InitialDate();
    //End Add

    setInitialState();

    //================ GRID ATTACH ========================================    
    //$('#frmAttach').attr('src', 'ISS090_Upload');
    $('#frmAttach').attr('src', 'ISS090_Upload?k=' + _attach_k);

    //ISS090_gridAttach = $("#ISS090_gridAttachDocList").InitialGrid(10, false, "/Installation/ISS090_IntialGridAttachedDocList");
    InitLoadAttachList(); //Modify by Jutarat A. on 21032014

    SpecialGridControl(ISS090_gridAttach, ["removeButton"]);
    SpecialGridControl(ISS090_gridAttach, ["downloadButton"]);
    
    BindOnLoadedEvent(ISS090_gridAttach, ISS090_gridAttachBinding);
    //$('#frmAttach').load(RefreshAttachList); //Comment by Jutarat A. on 21032014
    //====================================================================

    if (strContractProjectCode != "") {
        $("#ContractCodeProjectCode").val(strContractProjectCode)
        setTimeout("retrieve_installation_click()", 1000);
    }

    //Add by Jutarat A. on 08112013
    $("#ActualPOAmount").blur(function () { CalLastSubcontractorFee(); });
    $("#InstallationFee1").blur(function () { CalLastSubcontractorFee(); });
    $("#InstallationFee2").blur(function () { CalLastSubcontractorFee(); });
    $("#InstallationFee3").blur(function () { CalLastSubcontractorFee(); });
    $("#OtherFee").blur(function () { CalLastSubcontractorFee(); });
    $("#IMFee").blur(function () { CalLastSubcontractorFee(); });
    //End Add
}


function loadEmpName(MaintEmpNo,ctrlNo,ctrlName) {
    var parameter = { "MaintEmpNo": MaintEmpNo };
    call_ajax_method(
        '/Installation/ISS090_LoadEmployeeName/',
        parameter,
        function (result, controls) {
            if (controls != undefined) {
                //VaridateCtrl(["MaintEmpNo"], controls);
                ctrlName.val("");
                ctrlNo.focus();
                return;
            } else if (result != undefined) {
                ctrlName.val(result);
            }
        }
    );
}

function initialGridOnload(callback) {
    // intial grid
    var initializedgrid = [];
    var onGridInitialized = function (grid)
    {
        initializedgrid.push(grid);
        if (initializedgrid.length >= 4) {
            callback();
        }
    };
    ISS090_GridEmail = $("#gridEmail").InitialGrid(pageRow, false, "/Installation/ISS090_InitialGridEmail"
        , function () {             
            /* ===== binding event when finish load data ===== */
            BindOnLoadedEvent(ISS090_GridEmail, function () {
                var colInx = ISS090_GridEmail.getColIndexById('ButtonRemove');
                for (var i = 0; i < ISS090_GridEmail.getRowsNum(); i++) {
                    var rowId = ISS090_GridEmail.getRowId(i);
                    //GenerateRemoveButton(ISS090_GridEmail, "btnRemoveEmail", rowId, "ButtonRemove", true);

                    // binding grid button event 
                    BindGridButtonClickEvent("btnRemoveEmail", rowId, BtnRemoveEmailClick);

                }
                ISS090_GridEmail.setSizes();
            });

            onGridInitialized(ISS090_GridEmail);
        }
    );
    ISS090_GridEmail2 = $("#gridEmail2").InitialGrid(pageRow, false, "/Installation/ISS090_InitialGridEmail"
        , function () { 
            onGridInitialized(ISS090_GridEmail2);
        }
    );
    ISS090_GridPOInfo = $("#gridPOInfo").InitialGrid(pageRow, false, "/Installation/ISS090_InitialGridPOInfo"
        , function () { 
            onGridInitialized(ISS090_GridPOInfo);
        }
    );
    ISS090_GridManagementInfo = $("#gridInstallManageInfo").InitialGrid(pageRow, false, "/Installation/ISS090_InitialGridManagementInfo"
        , function () { 
            /* ===== binding event when finish load data ===== */
            BindOnLoadedEvent(ISS090_GridManagementInfo, function () {
                ManualInitialGridManagementInfo(false);

                ISS090_GridManagementInfo.setSizes();
            });
            //===============================================================

            //for (var i = 0; i < ISS090_GridManagementInfo.getRowsNum() ; i++) {
            //    var rDX = ISS090_GridManagementInfo.getRowId(i);
            //    var tmp = ISS090_GridManagementInfo.cells(rDX, ISS090_GridManagementInfo.getColIndexById("LastInstallationFee")).getValue();
            //    alert(tmp);
            //    ISS090_GridManagementInfo.cells(rDX, ISS090_GridManagementInfo.getColIndexById("TransferAmount")).setValue(currencyTotal[0] + ' ' + chkNum(tmp));
            //}

            onGridInitialized(ISS090_GridManagementInfo);
        }
    );

}

function setInitialState() {
    // --------------- Initial SCREEN ------------------
    $("#divContractBasicInfo").SetViewMode(false);
    $("#divProjectInfo").SetViewMode(false);
    $("#divInstallationInfo").SetViewMode(false);
    $("#divInstallationPOInfo").SetViewMode(false);
    $("#divInstallationMANo").SetViewMode(false);

    enabledGridEmail();
    enabledGridPOInfo();

    $("#ContractCodeProjectCode").attr("disabled", false);
    $("#btnRetrieveInstallation").attr("disabled", false);
    $("#btnClearInstallation").attr("disabled", false);

    //########## DISABLED INPUT CONTROL #################
//    $("#InstallationType").attr("disabled", true);
//    $("#PlanCode").attr("disabled", true);
//    $("#ProposeInstallStartDate").attr("disabled", true);
//    $("#ProposeInstallCompleteDate").attr("disabled", true);
//    $("#CustomerStaffBelonging").attr("disabled", true);
//    $("#CustomerStaffName").attr("disabled", true);
//    $("#CustomerStaffPhoneNo").attr("disabled", true);
//    $("#NewPhoneLineOpenDate").attr("disabled", true);
//    $("#NewConnectionPhoneNo").attr("disabled", true);
    //    $("#NewPhoneLineOwnerTypeCode").attr("disabled", true);
    $("#IEStaffEmpNo1").attr("disabled", true);
    $("#IEStaffEmpNo2").attr("disabled", true);
    $("#SubcontractorCode").attr("disabled", true);
    $("#SubConstractorGroupName").attr("disabled", true);
    $("#NormalSubPOAmount").attr("disabled", true);
    $("#ActualPOAmount").attr("disabled", true);
    $("#PONo").attr("disabled", true);
    $("#ExpectInstallStartDate").attr("disabled", true);
    $("#ExpectInstallCompleteDate").attr("disabled", true);
    $("#btnPOAdd").attr("disabled", true);
    $("#btnPOClear").attr("disabled", true);

    $("#EmailAddress").attr("disabled", true);
    $("#btnAdd").attr("disabled", true);
    $("#btnClear").attr("disabled", true);
    $("#btnSearchEmail").attr("disabled", true);

    $("#POMemo").attr("disabled", true);
    $("#btnSearchEmail").attr("disabled", true);
    $("#btnSearchEmail").attr("disabled", true);
    $("#uploadFrame1").attr("disabled", true);

    

    //####################################################

    InitialCommandButton(0);
    //$("#divInputContractCode").clearForm();
    $("#divContractBasicInfo").clearForm();
    $("#divProjectInfo").clearForm();
    $("#divInstallationInfo").clearForm();
    $("#divInstallationPOInfo").clearForm();
    $("#divInstallationMANo").clearForm();
    $("#divRegisterInstallationManagement").clearForm();

    //$("#divInputContractCode").show();
    $("#divContractBasicInfo").show();
    $("#divProjectInfo").show();
    $("#divInstallationInfo").show();
    $("#divInstallationMANo").show();
    $("#divAttachFrame").hide();
    $("#divAttachRemark").hide();
    //--------------------------------------------------  
}

function retrieve_installation_click() {
    //InitialCommandButton(1);
    command_control.CommandControlMode(false);
    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
    call_ajax_method_json("/Installation/ISS090_RetrieveData", obj,
        function (result, controls) {

            command_control.CommandControlMode(true);
            if (controls != undefined) {
                /* --- Higlight Text --- */
                /* --------------------- */
                //VaridateCtrl(["txtSpecifyContractCode"], controls);                      
                //$("#divStartResumeOperation").clearForm();
                //SetInitialState();    
                $("#divContractBasicInfo").hide();
                $("#divProjectInfo").hide();
                $("#divInstallationInfo").hide();
                $("#divContractBasicInfo").clearForm();
                $("#divProjectInfo").clearForm();
                $("#divInstallationInfo").clearForm();
                $("#divInstallationPOInfo").clearForm();
                $("#divInstallationMANo").clearForm();

                return;
            }
            else if (result != null) {


                var obj = { strFieldName: "" };

                $("#divContractBasicInfo").clearForm();
                $("#divProjectInfo").clearForm();
                $("#divInstallationInfo").clearForm();
                $("#divInstallationPOInfo").clearForm();
                $("#divInstallationMANo").clearForm();
                $("#ServiceTypeCode").val(result.ServiceTypeCode);

                $("#InstallationMemoHistory").val(result.strMemoHistory);
                $("#InstallationMemoProjectHistory").val(result.strMemoHistory);

                if (result.dtInstallationManagement != undefined) {

                    $("#strMode").val(result.strMode);
                    $("#strInstallationManagementStatus").val(result.dtInstallationManagement.ManagementStatus);
                    //SetScreenMode();

                    if (result.dtInstallationManagement != undefined && result.dtInstallationManagement.ProposeInstallStartDate != null)
                        result.dtInstallationManagement.ProposeInstallStartDate = ConvertDateToTextFormat(result.dtInstallationManagement.ProposeInstallStartDate.replace('/Date(', '').replace(')/', '') * 1);
                    if (result.dtInstallationManagement != undefined && result.dtInstallationManagement.ProposeInstallCompleteDate != null)
                        result.dtInstallationManagement.ProposeInstallCompleteDate = ConvertDateToTextFormat(result.dtInstallationManagement.ProposeInstallCompleteDate.replace('/Date(', '').replace(')/', '') * 1);
                    $("#POMemo").val(result.dtInstallationManagement.POMemo);
                    $("#IEStaffEmpNo1").val(result.dtInstallationManagement.IEStaffEmpNo1);
                    $("#IEStaffEmpNo2").val(result.dtInstallationManagement.IEStaffEmpNo2);

                    if ($("#IEStaffEmpNo1").val() != "") {
                        loadEmpName($("#IEStaffEmpNo1").val(), $("#IEStaffEmpNo1"), $("#EmpFullName1"));
                    }
                    else {
                        $("#EmpFullName1").val("");
                    }
                    if ($("#IEStaffEmpNo2").val() != "") {
                        loadEmpName($("#IEStaffEmpNo2").val(), $("#IEStaffEmpNo2"), $("#EmpFullName2"));
                    }
                    else {
                        $("#EmpFullName2").val("");
                    }
                }

                if (result.dtInstallationBasic != null) {
                    $("#divInstallationInfo").bindJSON(result.dtInstallationBasic);
                    $("#divContractBasicInfo").bindJSON(result.dtInstallationBasic);
                    $("#InstallationType").val(result.dtInstallationBasic.InstallationTypeName);
                }

                $("#divInstallationInfo").bindJSON(result.dtInstallationManagement);
                $("#divRegisterInstallationManagement").bindJSON(result.dtInstallationManagement);


                //Modify by Jutarat A. on 31072013
                /*if (result.ServiceTypeCode == C_SERVICE_TYPE_RENTAL) {
                    //$("#divContractBasicInfo").bindJSON(result.dtRentalContractBasic);                    
                    if (result.dtRentalContractBasic.NormalInstallationFee != null && result.dtRentalContractBasic.NormalInstallationFee != "")
                        $("#NormalInstallFee").val(moneyConvert(result.dtRentalContractBasic.NormalInstallationFee));
                    if (result.dtRentalContractBasic.BillingInstallationFee != null && result.dtRentalContractBasic.BillingInstallationFee != "")
                        $("#BillingInstallFee").val(moneyConvert(result.dtRentalContractBasic.BillingInstallationFee));
                }
                else if (result.ServiceTypeCode == C_SERVICE_TYPE_SALE) {
                    //$("#divContractBasicInfo").bindJSON(result.dtSaleContractBasic);
                    if (result.dtSale.NormalInstallationFee != null && result.dtSale.NormalInstallationFee != "")
                        $("#NormalInstallFee").val(moneyConvert(result.dtSale.NormalInstallationFee));
                    if (result.dtSale.BillingInstallationFee != null && result.dtSale.BillingInstallationFee != "")
                        $("#BillingInstallFee").val(moneyConvert(result.dtSale.BillingInstallationFee));
                }*/
                $("#NormalInstallFee").val(moneyConvert(result.NormalInstallationFee));
                $("#BillingInstallFee").val(moneyConvert(result.BillingInstallationFee));
                //End Modify

                if (result.dtRentalContractBasic != null) {
                    result.dtRentalContractBasic.ContractCode = $("#ContractCodeProjectCode").val();

                    $("#OperationOfficeCode").val(result.dtRentalContractBasic.OperationOfficeCode)
                    $("#divContractBasicInfo").bindJSON(result.dtRentalContractBasic);
                    $("#divInstallationInfo").bindJSON(result.dtRentalContractBasic);

                    SetRegisterState(1);
                }
                else if (result.dtSale != null) {
                    result.dtSale.ContractCode = $("#ContractCodeProjectCode").val();

                    $("#OperationOfficeCode").val(result.dtSale.OperationOfficeCode)
                    $("#divContractBasicInfo").bindJSON(result.dtSale);
                    $("#divInstallationInfo").bindJSON(result.dtSale);

                    SetRegisterState(1);
                }
                else if (result.dtProject != null) {

                    $("#divProjectInfo").bindJSON(result.dtProject);

                    SetRegisterState(2);
                }

                var decSecomPayment = result.NormalInstallationFee - result.BillingInstallationFee; //Add by Jutarat A. on 07112013
                $("#SecomPayment").val(moneyConvert(decSecomPayment)); //Add by Jutarat A. on 07112013

                /////////////// BIND MEMO ////////////////////////

                //////////////////////////////////////////////////

                /////////////// BIND EMAIl DATA //////////////////
                if (result.ListDOEmail != null && result.ListDOEmail != undefined) {
                    if (result.ListDOEmail.length > 0) {
                        for (var i = 0; i < result.ListDOEmail.length; i++) {
                            var emailList = [result.ListDOEmail[i].EmailAddress];

                            CheckFirstRowIsEmpty(ISS090_GridEmail, true);
                            AddNewRow(ISS090_GridEmail, emailList);
                        }
                    }
                }
                //////////////////////////////////////////////////

                /////////////// BIND EMAIl APPROVE DATA //////////////////
                if (result.ListApproveEmail != null && result.ListApproveEmail != undefined) {
                    if (result.ListApproveEmail.length > 0) {
                        for (var i = 0; i < result.ListApproveEmail.length; i++) {
                            var emailList = [result.ListApproveEmail[i].EmailAddress];

                            CheckFirstRowIsEmpty(ISS090_GridEmail2, true);
                            AddNewRow(ISS090_GridEmail2, emailList);
                        }
                    }
                }
                //////////////////////////////////////////////////

                /////////////// BIND PO DATA //////////////////
                var TotalNormalPOAmount = 0;
                var TotalActualPOAmount = 0;
                
                if (result.ListPOInfo != null && result.ListPOInfo != undefined) {
                    if (result.ListPOInfo.length > 0) {
                        CheckFirstRowIsEmpty(ISS090_GridPOInfo, true);
                        for (var i = 0; i < result.ListPOInfo.length; i++) {
                            var POList = [
                            //result.ListPOInfo[i].SubcontractorCode + " : " + result.arrayPOName[i],
                                          result.ListPOInfo[i].SubContractorName,
                                          result.ListPOInfo[i].SubcontractorGroupName,
                                          result.ListPOInfo[i].TxtCurrencySubPOAmount + " " + moneyConvert(result.ListPOInfo[i].NormalSubPOAmount),
                                          result.ListPOInfo[i].TxtCurrencyActualPOAmount + " " + moneyConvert(result.ListPOInfo[i].ActualPOAmount),

                                          result.ListPOInfo[i].PONo,
                                          ConvertDateToTextFormat(result.ListPOInfo[i].ExpectInstallStartDate.replace('/Date(', '').replace(')/', '') * 1),
                                          ConvertDateToTextFormat(result.ListPOInfo[i].ExpectInstallCompleteDate.replace('/Date(', '').replace(')/', '') * 1),
                                          result.ListPOInfo[i].SubcontractorCode];

                            CheckFirstRowIsEmpty(ISS090_GridPOInfo, true);
                            AddNewRow(ISS090_GridPOInfo, POList);
                            TotalNormalPOAmount = TotalNormalPOAmount + result.ListPOInfo[i].NormalSubPOAmount;
                            TotalActualPOAmount = TotalActualPOAmount + result.ListPOInfo[i].ActualPOAmount;
                            lastRow = i;
                        }

                        //$("#TotalNormalPOAmount").val(moneyConvert(TotalNormalPOAmount));
                        //$("#TotalActualPOAmount").val(moneyConvert(TotalActualPOAmount));
                        
                        AddNewRow(ISS090_GridPOInfo, ["", "", result.ListPOInfo[lastRow].TxtCurrencySubPOAmount + " " + moneyConvert(TotalNormalPOAmount),
                            result.ListPOInfo[lastRow].TxtCurrencyActualPOAmount + " " + moneyConvert(TotalActualPOAmount), "", "", "", ""]);
                        rowID = ISS090_GridPOInfo.getRowId(lastRow + 1);
                        ISS090_GridPOInfo.setColspan(rowID, 0, 2);
                        ISS090_GridPOInfo.cells2(lastRow + 1, 1).setValue(DivTotal);
                    }
                }
                
                //////////////////////////////////////////////////

                /////////////// BIND Management DATA //////////////////
                
                if (result.ListPOInfo != null && result.ListPOInfo != undefined) {
                    if (result.ListPOInfo.length > 0) {
                        CheckFirstRowIsEmpty(ISS090_GridManagementInfo, true);

                        var blNoCheck = "false"; //Add by Jutarat A. on 14102013
                        for (var i = 0; i < result.ListPOInfo.length; i++) {

                            //Add by Jutarat A. on 14102013
                            if (result.ListPOInfo[i].IEFirstCheckDate == null || result.ListPOInfo[i].IEFirstCheckDate == "") {
                                blNoCheck = "true";
                            }
                            //End Add

                            var MngList = [
                                           result.ListPOInfo[i].AdvancePaymentFlag,
                            //result.ListPOInfo[i].SubcontractorCode + " : " + result.arrayPOName[i],
                                           result.ListPOInfo[i].SubContractorName,
                                           result.ListPOInfo[i].SubcontractorGroupName,
                                           "(1) " + ((ConvertDateObjectToText(result.ListPOInfo[i].InstallStartDate) == "") ? "-" : ConvertDateObjectToText(result.ListPOInfo[i].InstallStartDate)) + "<br />(2) " + ((ConvertDateObjectToText(result.ListPOInfo[i].InstallCompleteDate)== "") ? "-" : ConvertDateObjectToText(result.ListPOInfo[i].InstallCompleteDate)),
                                           ConvertDateObjectToText(result.ListPOInfo[i].InstallStartDate),
                                           ConvertDateObjectToText(result.ListPOInfo[i].InstallCompleteDate),
                                           "(1) " + ((ConvertDateObjectToText(result.ListPOInfo[i].IEFirstCheckDate) == "")?"-":ConvertDateObjectToText(result.ListPOInfo[i].IEFirstCheckDate)) + "<br />(2) " + ((ConvertDateObjectToText(result.ListPOInfo[i].IELastInspectionDate) == "")?"-":ConvertDateObjectToText(result.ListPOInfo[i].IELastInspectionDate)),
                                           ConvertDateObjectToText(result.ListPOInfo[i].IEFirstCheckDate),
                                           ConvertDateObjectToText(result.ListPOInfo[i].IELastInspectionDate),
                                           result.ListPOInfo[i].TxtCurrencyLastInstallationFee + " " + moneyConvert(result.ListPOInfo[i].LastInstallationFee),
                                           result.ListPOInfo[i].ManPower,
                                           result.ListPOInfo[i].ComplainCode,
                                           result.ListPOInfo[i].CheckChargeIEEmpNo,
                                           result.ListPOInfo[i].CheckChargeIEEmpName,
                                           result.ListPOInfo[i].IEEvaluationCode,
                                           result.ListPOInfo[i].AdjustmentCode,
                                           result.ListPOInfo[i].AdjustmentContentCode,
                                           result.ListPOInfo[i].SubcontractorCode,
                                           result.ListPOInfo[i].PaidFlag+"",result.ListPOInfo[i].NormalSubPOAmount+"",
                                           //Add by Jutarat A. on 10102013
                                           result.ListPOInfo[i].NoCheckFlag, //blNoCheck, //Modify by Jutarat A. on 07112013
                                           ConvertDateObjectToText(result.ListPOInfo[i].LastPaidDate),
			                               result.ListPOInfo[i].IMFee,
			                               result.ListPOInfo[i].OtherFee,
			                               result.ListPOInfo[i].IMRemark,
			                               result.ListPOInfo[i].InstallationFee1,
			                               result.ListPOInfo[i].ApproveNo1,
			                               ConvertDateObjectToText(result.ListPOInfo[i].PaidDate1),
			                               result.ListPOInfo[i].InstallationFee2,
			                               result.ListPOInfo[i].ApproveNo2,
			                               ConvertDateObjectToText(result.ListPOInfo[i].PaidDate2),
			                               result.ListPOInfo[i].InstallationFee3,
			                               result.ListPOInfo[i].ApproveNo3,
			                               ConvertDateObjectToText(result.ListPOInfo[i].PaidDate3),
                                           //End Add
                                           result.ListPOInfo[i].ActualPOAmount, //Add by Jutarat A. on 08112013
                                           "", "", "", 
                                           ConvertDateObjectToText(result.ListPOInfo[i].ExpectInstallStartDate),
                                           ConvertDateObjectToText(result.ListPOInfo[i].ExpectInstallCompleteDate)
                                           ];
                            
                            AddNewRow(ISS090_GridManagementInfo, MngList);
                        }
                    }
                    colInx = ISS090_GridManagementInfo.getColIndexById('ButtonIssue');
                    ISS090_GridManagementInfo.setColumnHidden(colInx, true);
                    ManualInitialGridManagementInfo(false);
                    ISS090_GridManagementInfo.setColumnHidden(colInx, false);
                    SetFitColumnForBackAction(ISS090_GridManagementInfo, "TempColumn");
                }




                //////////////////////////////////////////////////
                //$("#ReferKey").val(result.dtInstallationBasic.MaintenanceNo);
                //var $currentIFrame = $('#frmAttach');
                //$currentIFrame.contents().find("body #ReferKey").val(result.dtInstallationBasic.MaintenanceNo);
                $('#frmAttach').attr('src', 'ISS090_SendAttachKey?sK=' + result.MaintenanceNo);
                $('#frmAttach').load(RefreshAttachList);
                $("#MaintenanceNo").val(result.MaintenanceNo);
                
//                if(result.dtInstallationBasic == null)
//                    blnHavetbtInstallationBasic = false;
//                else
//                    blnHavetbtInstallationBasic = true;
                blnHavetbtInstallationBasic = result.blnHavetbtInstallationBasic;

                if(result.doContractCodeListByProject == null || result.doContractCodeListByProject.length == 0)
                    blnHaveContractListByProject = false;
                else
                    blnHaveContractListByProject = true;
                //InitialCommandButton(result.dtInstallationBasic);
                InitialCommandButton();

                setTimeout("SetScreenMode()", 1000);
                //SetScreenMode();

                ChangeReasonCode_Change();
                ChangeRequestorCode_Change();
            }
            else {

                setInitialState();
            }
        }
    );
}


function clear_installation_click() {

    // Get Message
    var obj = {
        module: "Common",
        code: "MSG0044"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message, clearAllScreen, function () {
            
        });

    });
    
}

function clearAllScreen() {
    var obj = {
        module: "Common",
        code: "MSG0038"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    setInitialState();
                    //btnClearEmailClick();
                    ClearPOInfo();
                    DeleteAllRow(ISS090_GridManagementInfo);
                    DeleteAllRow(ISS090_gridAttach);
                    DeleteAllRow(ISS090_GridEmail2);                                   
                    DeleteAllRow(ISS090_GridEmail);
                    setTimeout("retrieve_installation_click()", 1000);
                });
    });
    
}



function BtnAddClick() {

    // Is exist email
    // Fill to grid
    // Keep selected email to sesstion


    var email = { "strEmail": $("#EmailAddress").val() + "@secom.co.th" };
    
    call_ajax_method_json("/Installation/ISS090_GetInstallEmail", email, function (result, controls, isWarning) {

        if (isWarning == undefined) { // No error and data(email) is exist

            if (result.length > 0) {
                // Fill to grid
                var emailList = [result[0].EmailAddress, "", result[0].EmpNo];

                CheckFirstRowIsEmpty(ISS090_GridEmail, true);
                AddNewRow(ISS090_GridEmail, emailList);

            }

        }

    });


}

function BtnAddPOInfoClick() {

    if ($("#EditingSubcontractorCode").val() != "") {
        UpdatePOInfo();
    }
    else {
        AddPOInfo();
    }
    
}

function AddPOInfo() {   
    
    var obj = {
        SubcontractorCode: $("#SubcontractorCode").val(),
        SubcontractorName: $("#SubcontractorCode option:selected").text(),
        SubcontractorGroupName: $("#SubConstractorGroupName").val(),
        NormalSubPOAmount: $("#NormalSubPOAmount").NumericValue(),
        ActualPOAmount: $("#ActualPOAmount").NumericValue(),
        PONo: $("#PONo").val(),
        ExpectInstallStartDate: $("#ExpectInstallStartDate").val(),
        ExpectInstallCompleteDate: $("#ExpectInstallCompleteDate").val()
    };

    call_ajax_method_json("/Installation/ISS090_AddPOInfo", obj, function (result, controls, isWarning) {

        if (isWarning != undefined) { // No error and data(email) is exist

            if (controls != undefined) {
                /* --- Higlight Text --- */
                /* --------------------- */
                VaridateCtrl(["SubcontractorCode", "NormalSubPOAmount", "ActualPOAmount", "ExpectInstallStartDate", "ExpectInstallCompleteDate"], controls);
                /* --------------------- */

                return;
            }
        }
        else if (result != null) {
            // Fill to grid
            var POList = [$("#SubcontractorCode option:selected").text(), result.SubcontractorGroupName, $("#NormalSubPOAmount").val(), $("#ActualPOAmount").val(), result.PONo, ConvertDateToTextFormat(result.ExpectInstallStartDate.replace('/Date(', '').replace(')/', '') * 1), ConvertDateToTextFormat(result.ExpectInstallCompleteDate.replace('/Date(', '').replace(')/', '') * 1), result.SubcontractorCode];

            CheckFirstRowIsEmpty(ISS090_GridPOInfo, true);
            AddNewRow(ISS090_GridPOInfo, POList);
        }

    });


}

function UpdatePOInfo() {
    
    var obj = {
        SubcontractorCode: $("#SubcontractorCode").val(),
        SubcontractorName: $("#SubcontractorCode option:selected").text(),
        SubcontractorGroupName: $("#SubConstractorGroupName").val(),
        NormalSubPOAmount: $("#NormalSubPOAmount").NumericValue(),
        ActualPOAmount: $("#ActualPOAmount").NumericValue(),
        PONo: $("#PONo").val(),
        ExpectInstallStartDate: $("#ExpectInstallStartDate").val(),
        ExpectInstallCompleteDate: $("#ExpectInstallCompleteDate").val()
    };

    call_ajax_method_json("/Installation/ISS090_UpdatePOInfo", obj, function (result, controls, isWarning) {

        if (isWarning != undefined) { // No error and data(email) is exist

            if (controls != undefined) {
                /* --- Higlight Text --- */
                /* --------------------- */
                VaridateCtrl(["SubcontractorCode", "NormalSubPOAmount", "ActualPOAmount", "ExpectInstallStartDate", "ExpectInstallCompleteDate"], controls);
                /* --------------------- */

                return;
            }
        }
        else if (result != null) {
            // Fill to grid
            //var POList = [$("#SubcontractorCode option:selected").text(), result.SubcontractorGroupName, $("#NormalSubPOAmount").val(), $("#ActualPOAmount").val(), result.PONo, ConvertDateToTextFormat(result.ExpectInstallStartDate.replace('/Date(', '').replace(')/', '') * 1), ConvertDateToTextFormat(result.ExpectInstallCompleteDate.replace('/Date(', '').replace(')/', '') * 1), result.SubcontractorCode];

            //CheckFirstRowIsEmpty(ISS090_GridPOInfo, true);
            //AddNewRow(ISS090_GridPOInfo, POList);            
            var selectedRowIndex = $("#EditingRowIndex").val();
            ISS090_GridPOInfo.cells2(selectedRowIndex, ISS090_GridPOInfo.getColIndexById('SubConCode')).setValue($("#SubcontractorCode option:selected").text())
            ISS090_GridPOInfo.cells2(selectedRowIndex, ISS090_GridPOInfo.getColIndexById('SubConGroupName')).setValue(result.SubcontractorGroupName);
            ISS090_GridPOInfo.cells2(selectedRowIndex, ISS090_GridPOInfo.getColIndexById('SubConPOAmount')).setValue($("#NormalSubPOAmount").val());
            ISS090_GridPOInfo.cells2(selectedRowIndex, ISS090_GridPOInfo.getColIndexById('ActualPOAmount')).setValue($("#ActualPOAmount").val());
            ISS090_GridPOInfo.cells2(selectedRowIndex, ISS090_GridPOInfo.getColIndexById('PONo')).setValue(result.PONo);
            ISS090_GridPOInfo.cells2(selectedRowIndex, ISS090_GridPOInfo.getColIndexById('InstallStartDate')).setValue($("#ExpectInstallStartDate").val());
            ISS090_GridPOInfo.cells2(selectedRowIndex, ISS090_GridPOInfo.getColIndexById('InstallCompleteDate')).setValue($("#ExpectInstallCompleteDate").val());
            $("#EditingRowIndex").val("");
            $("#EditingSubcontractorCode").val("");
            $("#btnPOAdd").text("Add/update");
            $("#SubcontractorCode").attr("disabled", false);
        }

    });


}

function BtnClearPOInfoClick() { 
        $("#SubcontractorCode").val(""),
        $("#SubConstractorGroupName").val(""),
        $("#NormalSubPOAmount").val(""),
        $("#ActualPOAmount").val("");
        $("#PONo").val("");
        ClearDateFromToControl("#ExpectInstallStartDate", "#ExpectInstallCompleteDate");
}

function BtnRemoveEmailClick(row_id) {

    var obj = {
        module: "Common",
        code: "MSG0141"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
			                        var selectedRowIndex = ISS090_GridEmail.getRowIndex(row_id);
                                    var mail = ISS090_GridEmail.cells2(selectedRowIndex, ISS090_GridEmail.getColIndexById('EmailAddress')).getValue();
                                    var obj = { EmailAddress: mail }
                                    DeleteRow(ISS090_GridEmail, row_id);

                                    call_ajax_method_json("/Installation/ISS090_RemoveMailClick", obj, function (result, controls, isWarning) {

                                    });
                });
    });

    
}


function BtnRemovePOInfoClick(row_id) {


    var selectedRowIndex = ISS090_GridPOInfo.getRowIndex(row_id);
    var strPONo = ISS090_GridPOInfo.cells2(selectedRowIndex, ISS090_GridPOInfo.getColIndexById('PONo')).getValue();
    var obj = { PONo: strPONo }
    DeleteRow(ISS090_GridPOInfo, row_id);

    call_ajax_method_json("/Installation/ISS090_RemovePOInfoClick", obj, function (result, controls, isWarning) {
       
    });

}

function BtnEditPOInfoClick(row_id) {


    var selectedRowIndex = ISS090_GridPOInfo.getRowIndex(row_id);
    $("#EditingSubcontractorCode").val(ISS090_GridPOInfo.cells2(selectedRowIndex, ISS090_GridPOInfo.getColIndexById('SubConCode')).getValue());
    $("#EditingRowIndex").val(ISS090_GridPOInfo.getRowIndex(row_id));   
    $("#SubcontractorCode").val(ISS090_GridPOInfo.cells2(selectedRowIndex, ISS090_GridPOInfo.getColIndexById('SubConCode')).getValue());
    $("#SubConstractorGroupName").val(ISS090_GridPOInfo.cells2(selectedRowIndex, ISS090_GridPOInfo.getColIndexById('SubConGroupName')).getValue());
    $("#NormalSubPOAmount").val(ISS090_GridPOInfo.cells2(selectedRowIndex, ISS090_GridPOInfo.getColIndexById('SubConPOAmount')).getValue());
    $("#ActualPOAmount").val(ISS090_GridPOInfo.cells2(selectedRowIndex, ISS090_GridPOInfo.getColIndexById('ActualPOAmount')).getValue());
    $("#PONo").val(ISS090_GridPOInfo.cells2(selectedRowIndex, ISS090_GridPOInfo.getColIndexById('PONo')).getValue());
    $("#ExpectInstallStartDate").val(ISS090_GridPOInfo.cells2(selectedRowIndex, ISS090_GridPOInfo.getColIndexById('InstallStartDate')).getValue());
    $("#ExpectInstallCompleteDate").val(ISS090_GridPOInfo.cells2(selectedRowIndex, ISS090_GridPOInfo.getColIndexById('InstallCompleteDate')).getValue());

    $("#SubcontractorCode").attr("disabled", true);
    $("#btnPOAdd").text("Update");
}



function BtnRegisterClick() {
    var registerData_obj = {};
}

//function BtnAddClick1() {

//    var obj = { EmailAddress: $("#EmailAddress").val() }
//    if (!IsValidEmail($("#EmailAddress").val())) {
//        var obj = {
//            module: "Common",
//            code: "MSG0087",
//            param: 'Notify email'
//        };
//        call_ajax_method("/Shared/GetMessage", obj, function (result) {
//            OpenErrorMessageDialog(result.Code, result.Message,
//            function () {
//            },
//            null);
//        });
//    }
//    else {
//        call_ajax_method_json('/Installation/ValidateEmail_ISS090', obj,
//            function (result, controls) {
//                if (result == undefined) {
//                    mygrid = $("#gridEmail").LoadDataToGridWithInitial(pageRow, false, false, "/Installation/GetEmail_ISS090", obj, "ISS090_DOEmailData", false);

//                    BindOnLoadedEvent(mygrid, function () {
//                        var emailColinx = mygrid.getColIndexById('EmailAddress');
//                        var removeColinx = mygrid.getColIndexById('Remove');

//                        mygrid.setColumnHidden(mygrid.getColIndexById('EmpNo'), true);
//                        for (var i = 0; i < mygrid.getRowsNum(); i++) {
//                            mygrid.cells2(i, removeColinx).setValue("<button id='btnRemove" + i.toString() + "' style='width:65px' >Remove</button>");
//                        }
//                    });

//                    mygrid.attachEvent("OnRowSelect", function (id, ind) {
//                        if (ind == mygrid.getColIndexById('Remove')) {
//                            BtnRemoveMailClick(mygrid.cells2(ind - 1, 0).getValue());
//                        }
//                    });
//                }
//                else {
//                    if (result != false) {
//                        OpenErrorMessageDialog(result.Code, result.Message, function () { }, null);
//                    }
//                }
//            });
//    }
//}

//function BtnRemoveMailClick(mail) {
//    var obj = { EmailAddress: mail }
//    var mygrid = $("#gridEmail").LoadDataToGridWithInitial(pageRow, false, false, "/Installation/RemoveMailClick_ISS010", obj, "ISS010_DOEmailData", false);

//    BindOnLoadedEvent(mygrid, function () {
//        var emailColinx = mygrid.getColIndexById('EmailAddress');
//        var removeColinx = mygrid.getColIndexById('Remove');
//        for (var i = 0; i < mygrid.getRowsNum(); i++) {
//            mygrid.cells2(i, removeColinx).setValue("<button id='btnRemove" + i.toString() + "' style='width:65px' >Remove</button>");
//        }
//    });

//    mygrid.attachEvent("OnRowSelect", function (id, ind) {
//        if (ind == mygrid.getColIndexById('Remove')) {
//            BtnRemoveMailClick(mygrid.cells2(ind - 1, 0).getValue());
//        }
//    });
//}

function BtnClearClick() {
    $("#EmailAddress").val("");

}

function IsValidEmail(email) {
    //var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    //return emailReg.test(email);
    return true;
}



function InitialCommandButton() { 
    var strMode = $("#strMode").val();
    if (strMode == C_FUNC_ID_VIEW || strMode == C_FUNC_ID_COMPLETE)
    {
        SetRegisterCommand(false, null);
        SetResetCommand(false, null);
        SetApproveCommand(false, null);
        SetRejectCommand(false, null);
        SetReturnCommand(false, null);
        SetCloseCommand(false, null);
        SetConfirmCommand(false, null);
        SetBackCommand(false, null);
    }
    else if (strMode == C_FUNC_ID_OPERATE)
    {
        if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL || $("#ServiceTypeCode").val() == C_SERVICE_TYPE_SALE) 
        {
            if(!blnHavetbtInstallationBasic)
            {
                SetRegisterCommand(true, command_register_click);
                SetResetCommand(true, command_reset_click);
                SetApproveCommand(false, null);
                SetRejectCommand(false, null);
                SetReturnCommand(false, null);
                SetCloseCommand(false, null);
                SetConfirmCommand(false, null);
                SetBackCommand(false, null);
                SetRequestApproveCommand(true, command_requestapprove_click);
            }
            else
            {
                SetRegisterCommand(true, command_register_click);
                SetResetCommand(true, command_reset_click);
                SetApproveCommand(false, null);
                SetRejectCommand(false, null);
                SetReturnCommand(false, null);
                SetCloseCommand(false, null);
                SetConfirmCommand(false, null);
                SetBackCommand(false, null);
                SetRequestApproveCommand(false, null);
            }
        }
        else
        {
            if(!blnHaveContractListByProject)
            {
                SetRegisterCommand(true, command_register_click);
                SetResetCommand(true, command_reset_click);
                SetApproveCommand(false, null);
                SetRejectCommand(false, null);
                SetReturnCommand(false, null);
                SetCloseCommand(false, null);
                SetConfirmCommand(false, null);
                SetBackCommand(false, null);
                SetRequestApproveCommand(true, command_requestapprove_click);
            }
            else
            {
                SetRegisterCommand(true, command_register_click);
                SetResetCommand(true, command_reset_click);
                SetApproveCommand(false, null);
                SetRejectCommand(false, null);
                SetReturnCommand(false, null);
                SetCloseCommand(false, null);
                SetConfirmCommand(false, null);
                SetBackCommand(false, null);
                SetRequestApproveCommand(false, null);
            }
        }
    }
    else if(strMode == C_FUNC_ID_APPROVE )
    {
        SetRegisterCommand(false, null);
        SetResetCommand(false, null);
        SetApproveCommand(true, command_approve_click);
        SetRejectCommand(false, null);
        SetReturnCommand(true, command_reject_click);
        SetCloseCommand(false, null);
        SetConfirmCommand(false, null);
        SetBackCommand(false, null);
    }
}

function command_back_click() {
//    SetRegisterCommand(true, command_register_click);
//    SetResetCommand(true, command_reset_click);
//    SetApproveCommand(false, null);
//    SetRejectCommand(false, null);
//    SetReturnCommand(false, null);
//    SetCloseCommand(false, null);
//    SetConfirmCommand(false, null);
//    SetBackCommand(false, null);
    InitialCommandButton();

    $("#divContractBasicInfo").SetViewMode(false);
    $("#divProjectInfo").SetViewMode(false);
    $("#divInstallationInfo").SetViewMode(false);
    $("#divInstallationPOInfo").SetViewMode(false);
    $("#divRegisterInstallationManagement").SetViewMode(false);
    $("#divAttachFrame").show();
    $("#divAttachRemark").show();

    $("#divEditInstallManagement").show();

    $("#btnUpdate").hide();
    $("#btnCancel").hide();
    ISS090_GridManagementInfo.setColumnHidden(ISS090_GridManagementInfo.getColIndexById('ButtonEdit'), false);
    ISS090_GridManagementInfo.setColumnHidden(ISS090_GridManagementInfo.getColIndexById('ButtonPaid'), false);
    ISS090_GridManagementInfo.setColumnHidden(ISS090_GridManagementInfo.getColIndexById('ButtonIssue'), false);
    SetFitColumnForBackAction(ISS090_GridManagementInfo, "TempColumn");

    ISS090_gridAttach.setColumnHidden(ISS090_gridAttach.getColIndexById('downloadButton'), false);
    ISS090_gridAttach.setColumnHidden(ISS090_gridAttach.getColIndexById('removeButton'), false);
    SetFitColumnForBackAction(ISS090_gridAttach, "TmpColumn");
}

function command_register_click() {
    command_control.CommandControlMode(false);
    //var obj = CreateObjectData($("#form1").serialize() + "&RequestMemo=" + $("#RequestMemo").val());
    var obj = { strContractCode: $("#ContractCodeProjectCode").val()
                ,ChangeReasonCode: $("#ChangeReasonCode").val()
                ,ChangeRequestorCode: $("#ChangeRequestorCode").val()
              };
    call_ajax_method_json("/Installation/ISS090_ValidateBeforeRegister", obj, function (result, controls) {
        command_control.CommandControlMode(true);
        if (result == true) {
            setConfirmState();
            $("#divEditInstallManagement").hide();
                              
        }        
        else{
            VaridateCtrl(["ChangeReasonCode", "ChangeRequestorCode"], controls);
        }
        
    });
}

function setConfirmState() {
    SetRegisterCommand(false, null);
    SetResetCommand(false, null);
    SetApproveCommand(false, null);
    SetRejectCommand(false, null);
    SetReturnCommand(false, null);
    SetCloseCommand(false, null);
    SetConfirmCommand(true, command_confirm_click);
    SetBackCommand(true, command_back_click);
    SetRequestApproveCommand(false, null);

    $("#divContractBasicInfo").SetViewMode(true);
    $("#divProjectInfo").SetViewMode(true);
    $("#divInstallationInfo").SetViewMode(true);
    $("#divInstallationPOInfo").SetViewMode(true);
    $("#divRegisterInstallationManagement").SetViewMode(true);
    $("#divAttachFrame").hide();
    $("#divAttachRemark").hide();
    //disabledGridEmail();
    //disabledGridPOInfo();
    ISS090_GridManagementInfo.setColumnHidden(ISS090_GridManagementInfo.getColIndexById('ButtonEdit'), true);
    ISS090_GridManagementInfo.setColumnHidden(ISS090_GridManagementInfo.getColIndexById('ButtonPaid'), true);
    ISS090_GridManagementInfo.setColumnHidden(ISS090_GridManagementInfo.getColIndexById('ButtonIssue'), true);
    //SetFitColumnForBackAction(ISS090_GridManagementInfo, "TempColumn2");

    ISS090_gridAttach.setColumnHidden(ISS090_gridAttach.getColIndexById('downloadButton'), true);
    ISS090_gridAttach.setColumnHidden(ISS090_gridAttach.getColIndexById('removeButton'), true);
}

function disabledGridEmail() {
    //////// DISABLED BUTTON In EMAIL GRID ///////////
    for (var i = 0; i < ISS090_GridEmail.getRowsNum(); i++) {
        var row_id = ISS090_GridEmail.getRowId(i);
        EnableGridButton(ISS090_GridEmail, "btnRemoveEmail", row_id, "ButtonRemove", false);
    }
    //////////////////////////////////////////////////
}

function enabledGridEmail() {
    //////// ENABLED BUTTON In EMAIL GRID ///////////
    for (var i = 0; i < ISS090_GridEmail.getRowsNum(); i++) {
        var row_id = ISS090_GridEmail.getRowId(i);
        EnableGridButton(ISS090_GridEmail, "btnRemoveEmail", row_id, "ButtonRemove", true);
    }
    //////////////////////////////////////////////////
}

function disabledGridPOInfo() {
    //////// DISABLED BUTTON In PO GRID ///////////
    for (var i = 0; i < ISS090_GridPOInfo.getRowsNum(); i++) {
        var row_id = ISS090_GridPOInfo.getRowId(i);
        EnableGridButton(ISS090_GridPOInfo, "btnRemovePOInfo", row_id, "ButtonPORemove", false);
        EnableGridButton(ISS090_GridPOInfo, "btnEditPOInfo", row_id, "ButtonPOEdit", false);
        EnableGridButton(ISS090_GridPOInfo, "btnIssuePOInfo", row_id, "ButtonPOIssue", false);
    }
    //////////////////////////////////////////////////
}

function enabledGridPOInfo() {
    //////// ENABLED BUTTON In PO GRID ///////////
    for (var i = 0; i < ISS090_GridPOInfo.getRowsNum(); i++) {
        var row_id = ISS090_GridPOInfo.getRowId(i);
        EnableGridButton(ISS090_GridPOInfo, "btnRemovePOInfo", row_id, "ButtonPORemove", true);
        EnableGridButton(ISS090_GridPOInfo, "btnEditPOInfo", row_id, "ButtonPOEdit", true);
        EnableGridButton(ISS090_GridPOInfo, "btnIssuePOInfo", row_id, "ButtonPOIssue", true);
    }
    //////////////////////////////////////////////////
}

function setSuccessRegisState() {
    // --------------- Initial SCREEN ------------------
    $("#divContractBasicInfo").SetViewMode(true);
    $("#divProjectInfo").SetViewMode(true);
    $("#divInstallationInfo").SetViewMode(true);
    $("#divInstallationPOInfo").SetViewMode(true);
    $("#divInstallationMANo").SetViewMode(true);

    disabledGridEmail();
    disabledGridPOInfo();

    $("#divInstallationMANo").show();  

    $("#ContractCodeProjectCode").attr("disabled", true);
    $("#btnRetrieveInstallation").attr("disabled", true);
    $("#btnClearInstallation").attr("disabled", false);

    //########## DISABLED INPUT CONTROL #################
//    $("#InstallationType").attr("disabled", true);
//    $("#PlanCode").attr("disabled", true);
//    $("#ProposeInstallStartDate").attr("disabled", true);
//    $("#ProposeInstallCompleteDate").attr("disabled", true);
//    $("#CustomerStaffBelonging").attr("disabled", true);
//    $("#CustomerStaffName").attr("disabled", true);
//    $("#CustomerStaffPhoneNo").attr("disabled", true);
//    $("#NewPhoneLineOpenDate").attr("disabled", true);
//    $("#NewConnectionPhoneNo").attr("disabled", true);
    //    $("#NewPhoneLineOwnerTypeCode").attr("disabled", true);
    $("#IEStaffEmpNo1").attr("disabled", true);
    $("#IEStaffEmpNo2").attr("disabled", true);
    $("#SubcontractorCode").attr("disabled", true);
    $("#SubConstractorGroupName").attr("disabled", true);
    $("#NormalSubPOAmount").attr("disabled", true);
    $("#ActualPOAmount").attr("disabled", true);
    $("#PONo").attr("disabled", true);
    $("#ExpectInstallStartDate").attr("disabled", true);
    $("#ExpectInstallCompleteDate").attr("disabled", true);
    $("#btnPOAdd").attr("disabled", true);
    $("#btnPOClear").attr("disabled", true);

    $("#EmailAddress").attr("disabled", true);
    $("#btnAdd").attr("disabled", true);
    $("#btnClear").attr("disabled", true);
    $("#btnSearchEmail").attr("disabled", true);

    $("#POMemo").attr("disabled", true);
    $("#btnSearchEmail").attr("disabled", true);
    $("#btnSearchEmail").attr("disabled", true);
    $("#uploadFrame1").attr("disabled", true);
    //####################################################

    InitialCommandButton(0);
}

function command_confirm_click() {
    command_control.CommandControlMode(false);
    SendGridDetailsToObject();

//    $("#divContractBasicInfo").SetViewMode(false);
//    $("#divProjectInfo").SetViewMode(false);
//    $("#divInstallationInfo").SetViewMode(false);
//    $("#divInstallationPOInfo").SetViewMode(false);
//    $("#divInstallationMANo").SetViewMode(false);

//    enabledGridEmail();
//    enabledGridPOInfo();

    var obj = CreateObjectData($("#form1").serialize());
    master_event.LockWindow(true);
    call_ajax_method_json("/Installation/ISS090_RegisterData", obj, function (result, controls) {
        command_control.CommandControlMode(true);
        master_event.LockWindow(false);
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["InstallationType", "CustomerStaffBelonging", "CustomerStaffName", "CustomerStaffPhoneNo"], controls);
            /* --------------------- */

            return;
        }
        else if (result != undefined)  {

            SetConfirmCommand(false, null);
            SetBackCommand(false, null);
            $("#strInstallationManagementStatus").val(C_INSTALL_MANAGE_STATUS_PROCESSING);
            //SetScreenMode();
            /* -------------------------- */
            var obj = {
                module: "Common",
                code: "MSG0046",
                param: ""
            };

            call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                //OpenWarningDialog(result.Message, result.Message, null);
                OpenInformationMessageDialog(result.Code, result.Message);
            });
            
           
        }
    });

    
}

function command_reset_click() {
//    if ($("#ContractCodeProjectCode").val() == "") {
//        setInitialState();
//    }
//    else {
//        if ($("#InstallationType").attr("disabled") == "true") {
//            SetRegisterState(2)
//        }
//        else {
//            SetRegisterState(1)
//        }
    //    }
    clearAllScreen();
    
}

function SetRegisterState(cond) {

    InitialCommandButton(1);

    //########## ENABLED INPUT CONTROL #################
//    $("#InstallationType").attr("disabled", false);
//    $("#PlanCode").attr("disabled", false);
//    $("#ProposeInstallStartDate").attr("disabled", false);
//    $("#ProposeInstallCompleteDate").attr("disabled", false);
//    $("#CustomerStaffBelonging").attr("disabled", false);
//    $("#CustomerStaffName").attr("disabled", false);
//    $("#CustomerStaffPhoneNo").attr("disabled", false);
//    $("#NewPhoneLineOpenDate").attr("disabled", false);
//    $("#NewConnectionPhoneNo").attr("disabled", false);
    //    $("#NewPhoneLineOwnerTypeCode").attr("disabled", false);
    $("#IEStaffEmpNo1").attr("disabled", false);
    $("#IEStaffEmpNo2").attr("disabled", false);
    $("#SubcontractorCode").attr("disabled", false);
    $("#SubConstractorGroupName").attr("disabled", false);
    $("#NormalSubPOAmount").attr("disabled", false);
    $("#ActualPOAmount").attr("disabled", false);
    $("#PONo").attr("disabled", false);
    $("#ExpectInstallStartDate").attr("disabled", false);
    $("#ExpectInstallCompleteDate").attr("disabled", false);
    $("#btnPOAdd").attr("disabled", false);
    $("#btnPOClear").attr("disabled", false);

    $("#EmailAddress").attr("disabled", false);
    $("#btnAdd").attr("disabled", false);
    $("#btnClear").attr("disabled", false);
    $("#btnSearchEmail").attr("disabled", false);

    $("#POMemo").attr("disabled", false);
    $("#btnSearchEmail").attr("disabled", false);
    $("#btnSearchEmail").attr("disabled", false);

    $("#uploadFrame1").attr("disabled", false);         
    //####################################################

    $("#ContractCodeProjectCode").attr("disabled", true);
    $("#btnRetrieveInstallation").attr("disabled", true);
    $("#btnClearInstallation").attr("disabled", false);
    if (cond == 1) {
        $("#InstallationType").attr("disabled", false);
        $("#divContractBasicInfo").show();
        $("#divProjectInfo").hide();
        $("#divInstallationInfo").show();
        $("#divInstallationMANo").hide();    
    }
    else if (cond == 2) {
        $("#InstallationType").attr("disabled", true);
        $("#divContractBasicInfo").hide();
        $("#divProjectInfo").show();
        $("#divInstallationInfo").show();
        $("#divInstallationMANo").hide();    
    }
    $("#divAttachFrame").show();
    $("#divAttachRemark").show();

    $("#divEditInstallManagement").show();
    ISS090_GridManagementInfo.setColumnHidden(ISS090_GridManagementInfo.getColIndexById('ButtonEdit'), false);
    ISS090_GridManagementInfo.setColumnHidden(ISS090_GridManagementInfo.getColIndexById('ButtonPaid'), false);
    ISS090_GridManagementInfo.setColumnHidden(ISS090_GridManagementInfo.getColIndexById('ButtonIssue'), false);
    SetFitColumnForBackAction(ISS090_GridManagementInfo, "TempColumn");
}


//function CMS060Response(result) {

//    $("#dlgCTS053").CloseDialog();
//    var emailColinx;
//    var removeColinx;
//    //var mygridCTS053 = $("#gridEmail").LoadDataToGridWithInitial(pageRow, false, false, "/Contract/GetEmailList_CTS053", result, "CTS053_DOEmailData", false);
//    var mygridCTS053 = $("#gridEmail").LoadDataToGridWithInitial(pageRow, false, false, "/Installation/GetEmailList_ISS090", result, "ISS090_DOEmailData", false);
//    BindOnLoadedEvent(mygridCTS053, function () {
//        emailColinx = mygridCTS053.getColIndexById('EmailAddress');
//        removeColinx = mygridCTS053.getColIndexById('Remove');
//        if (emailColinx != undefined) {
//            for (var i = 0; i < mygridCTS053.getRowsNum(); i++) {
//                mygridCTS053.cells2(i, removeColinx).setValue("<button id='btnRemove" + i.toString() + "' style='width:65px' >Remove</button>");
//            }
//        }
//    });

//    if (emailColinx != undefined) {
//        mygridCTS053.attachEvent("OnRowSelect", function (id, ind) {
//            if (ind == mygridCTS053.getColIndexById('Remove')) {
//                BtnRemoveMailClick(mygridCTS053.cells2(ind - 1, 0).getValue());
//            }
//        });
//    }
//}



//function CMS060Response(result) {

//    $("#dlgCTS053").CloseDialog();
//    var emailColinx;
//    var removeColinx;
//    //var mygridCTS053 = $("#gridEmail").LoadDataToGridWithInitial(pageRow, false, false, "/Contract/GetEmailList_CTS053", result, "CTS053_DOEmailData", false);
//    //var mygridCTS053 = $("#gridEmail").LoadDataToGridWithInitial(pageRow, false, false, "/Installation/GetEmailList_ISS010", result, "ISS010_DOEmailData", false);


//    call_ajax_method_json("/Installation/GetEmailList_ISS090", result, function (result, controls) {
//        if (result != null && result.length > 0) {
//            for (var i = 0; i < result.length; i++) {
//                // Fill to grid
//                var emailList = [result[i].EmailAddress, "", result[i].EmpNo];

//                CheckFirstRowIsEmpty(ISS090_GridEmail, true);
//                AddNewRow(ISS090_GridEmail, emailList);
//            }
//        }
//    });


//}

//function btnClearEmailClick() {
//    DeleteAllRow(ISS090_GridEmail);
//    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
//    call_ajax_method_json("/Installation/ISS090_ClearInstallEmail", obj, function (result, controls) {        

//            
//    });

//}

function ClearPOInfo() {
    DeleteAllRow(ISS090_GridPOInfo);
    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
    call_ajax_method_json("/Installation/ISS090_ClearPOInfo", obj, function (result, controls) {


    });

}

//Comment by Jutarat A. on 27032014
/*function moneyConvert(value) {
    if (value != null) {
        var buf = "";
        var sBuf = "";
        var j = 0;
        value = String(value);

        if (value.indexOf(".") > 0) {
            buf = value.substring(0, value.indexOf("."));
        } else {
            buf = value;
        }
        if (buf.length % 3 != 0 && (buf.length / 3 - 1) > 0) {
            sBuf = buf.substring(0, buf.length % 3) + ",";
            buf = buf.substring(buf.length % 3);
        }
        j = buf.length;
        for (var i = 0; i < (j / 3 - 1); i++) {
            sBuf = sBuf + buf.substring(0, 3) + ",";
            buf = buf.substring(3);
        }
        sBuf = sBuf + buf;
        if (value.indexOf(".") > 0) {
            value = sBuf + value.substring(value.indexOf("."),value.indexOf(".") + 3);
        }
        else {
            value = sBuf + ".00";
        }
        return value;
    }
}*/
//End Comment

function ConvertDateObjectToText(strDate) {
    if (strDate != null) {
        strDate = ConvertDateToTextFormat(strDate.replace('/Date(', '').replace(')/', '') * 1);
        return strDate;
    }
    else {
        return "";
    }
}

function SetScreenMode() {
    var strMode = $("#strMode").val();
    var strInstallationManagementStatus = $("#strInstallationManagementStatus").val();

    if (strMode == C_FUNC_ID_VIEW || strMode == C_FUNC_ID_APPROVE)
    {   
        DisabledInstallManageMainSection(true);
        //ISS090_GridManagementInfo.setColumnHidden(ISS090_GridManagementInfo.getColIndexById('ButtonEdit'), true);
        disabledGridColumn(ISS090_GridManagementInfo, 'btnEdit', 'ButtonEdit', true);
        enableViewButton = true;
        enableEditButton = false;

        enableIssueButton = false;
        enablePaidButton = false;

        disabledGridColumn(ISS090_gridAttach, 'btnRemoveAttach', 'removeButton', true);
        $("#divAttachFrame").hide();
        $("#divAttachRemark").hide();
        $("#ChangeReasonCode").attr("disabled", true);
        $("#ChangeReasonOther").attr("readonly", true);
        $("#ChangeRequestorCode").attr("disabled", true);
        $("#ChangeRequestorOther").attr("readonly", true);
    }
    else if(strMode == C_FUNC_ID_OPERATE) {
//        if(blnHavetbtInstallationBasic == false)
//        {
//            enableViewButton = true;
//            enableEditButton = false;
//        }
//        else
//        {
            enableViewButton = false;
            enableEditButton = true;
//        }
        DisabledInstallManageMainSection(false);
        //enablePaidButton = false;
        DisabledInstallManageSubSection(true);
        $("#TotalLastInstallFee").attr("readonly", true);
        $("#ProfitAmount").attr("readonly", true);
        $("#CostRate").attr("readonly", true);
        $("#MaterialFee").attr("readonly", true); //Add by Jutarat A. on 08112013
    }
    else if (strMode == C_FUNC_ID_COMPLETE) {
        //enableIssueButton = true;
        enablePaidButton = false;

        enableViewButton = true;
        enableEditButton = false;
        DisabledInstallManageMainSection(true);
        //ISS090_GridManagementInfo.setColumnHidden(ISS090_GridManagementInfo.getColIndexById('ButtonPaid'), true);
        disabledGridColumn(ISS090_GridManagementInfo, 'btnPaid', 'ButtonPaid', false);
        disabledGridColumn(ISS090_gridAttach, 'btnRemoveAttach', 'removeButton', true); 
        $("#divAttachFrame").hide();
        $("#divAttachRemark").hide();
        $("#ChangeReasonCode").attr("disabled", true);
        $("#ChangeReasonOther").attr("readonly", true);
        $("#ChangeRequestorCode").attr("disabled", true);
        $("#ChangeRequestorOther").attr("readonly", true);
    }

    if (permissionDownload && (strInstallationManagementStatus == C_INSTALL_MANAGE_STATUS_APPROVED || strInstallationManagementStatus == C_INSTALL_MANAGE_STATUS_COMPLETED)) {
        enableIssueButton = true;
    }
    else {
        enableIssueButton = false;
    }


    if(strMode == C_FUNC_ID_OPERATE)
    {
       $("#divApproveEmail").show();
    }
    else
    {
        $("#divApproveEmail").hide();
    }

    ManualInitialGridManagementInfo(false);
}

function DisabledInstallManageSubSection(disable)
{
//    $("#SubcontractorName").attr("disabled",disable);
//    $("#SubcontractorGroupName").attr("disabled",disable);
    var enable; 
    if(disable)
    {
        enable = false;
        $("#btnUpdate").hide();
        $("#btnCancel").hide();
    }
    else
    {
        enable = true;
        if(enableEditButton)
        {
            $("#btnUpdate").show();
            $("#btnCancel").show();
        }        
    }
    $("#InstallationStartDate").EnableDatePicker(enable);
    $("#InstallationCompleteDate").EnableDatePicker(enable);
    $("#ExpectInstallationStartDate").EnableDatePicker(enable);
    $("#ExpectInstallationCompleteDate").EnableDatePicker(enable);
    $("#IEFirstCheckDate").EnableDatePicker(enable);
    $("#LastInspectionDate").EnableDatePicker(enable);

    $("#ManPower").attr("readonly", disable);
    $("#CheckChargeIECode").attr("readonly",disable);
    $("#IEEvaluation").attr("disabled",disable);
    $("#Complain").attr("disabled",disable);
    $("#Adjustment").attr("disabled",disable);
    $("#AdjustmentContents").attr("disabled",disable);    
    //$("#LastInstallationfee").attr("readonly", disable); //Comment by Jutarat A. on 08112013

    //Add by Jutarat A. on 11102013
    $("#chkNoCheck").attr("disabled", disable);
    $("#LastPaidDate").EnableDatePicker(enable);
    $("#IMFee").attr("readonly", disable);
    $("#OtherFee").attr("readonly", disable);
    $("#IMRemark").attr("readonly", disable);
    $("#ActualPOAmount").attr("readonly", disable); //Add by Jutarat A. on 08112013
    $("#InstallationFee1").attr("readonly", disable);
    $("#ApproveNo1").attr("readonly", disable);
    $("#PaidDate1").EnableDatePicker(enable);
    $("#InstallationFee2").attr("readonly", disable);
    $("#ApproveNo2").attr("readonly", disable);
    $("#PaidDate2").EnableDatePicker(enable);
    $("#InstallationFee3").attr("readonly", disable);
    $("#ApproveNo3").attr("readonly", disable);
    $("#PaidDate3").EnableDatePicker(enable);

//    if (enable) {
//        if ($("#IEFirstCheckDate").val() != null && $("#IEFirstCheckDate").val() != "") {
//            $("#chkNoCheck").attr("checked", false);
//            $("#IEFirstCheckDate").EnableDatePicker(true);
//        }
//        else {
//            $("#chkNoCheck").attr("checked", true);
//            $("#IEFirstCheckDate").EnableDatePicker(false);
//        }
//    }
//    else {
//        $("#chkNoCheck").attr("checked", false);
//    }

     if (enable) {
        if ($("#chkNoCheck").prop("checked")) {
            $("#IEFirstCheckDate").EnableDatePicker(false);

            //Add by Jutarat A. on 07112013
            $("#Adjustment").attr("disabled",true);
            $("#AdjustmentContents").attr("disabled",true);
            $("#LastInspectionDate").EnableDatePicker(false);

            $("#InstallationStartDate").EnableDatePicker(false);
            $("#InstallationCompleteDate").EnableDatePicker(false);
            $("#ExpectInstallationStartDate").EnableDatePicker(false);
            $("#ExpectInstallationCompleteDate").EnableDatePicker(false);
            $("#ManPower").SetDisabled(true);
            $("#Complain").attr("disabled",true);
            $("#IEEvaluation").attr("disabled",true);

            //$("#LastInstallationfee").SetDisabled(false); //Comment by Jutarat A. on 08112013
            $("#CheckChargeIECode").SetDisabled(true);
            //End Add
        }
        else {
            $("#IEFirstCheckDate").EnableDatePicker(true);

            //Add by Jutarat A. on 07112013
            $("#InstallationStartDate").EnableDatePicker(true);
            $("#InstallationCompleteDate").EnableDatePicker(true);
            $("#ExpectInstallationStartDate").EnableDatePicker(true);
            $("#ExpectInstallationCompleteDate").EnableDatePicker(true);
            $("#ManPower").SetDisabled(false);
            $("#Complain").attr("disabled",false);
            $("#IEEvaluation").attr("disabled",false);

            //changeIEEvaluation(); //Comment by Jutarat A. on 08112013
            $("#CheckChargeIECode").SetDisabled(false);
            //End Add
        }
    }
    //End Add
}

function DisabledInstallManageMainSection(disable)
{    
    //ISS090_GridManagementInfo.setColumnHidden(ISS090_GridManagementInfo.getColIndexById('ButtonEdit'), disable);
    //ISS090_GridManagementInfo.setColumnHidden(ISS090_GridManagementInfo.getColIndexById('ButtonPaid'), disable);
    //ISS090_GridManagementInfo.setColumnHidden(ISS090_GridManagementInfo.getColIndexById('ButtonIssue'), disable);
    disabledGridColumn(ISS090_GridManagementInfo, 'btnEdit', 'ButtonEdit', disable);
    disabledGridColumn(ISS090_GridManagementInfo, 'btnPaid', 'ButtonPaid', disable);
    disabledGridColumn(ISS090_GridManagementInfo, 'btnIssue', 'ButtonIssue', disable);

    DisabledInstallManageSubSection(disable);
    if(disable){
        $("#btnUpdate").hide();
        $("#btnCancel").hide();
    }
    else{
        $("#btnUpdate").show();
        $("#btnCancel").show();
    }

    $("#IEManPower").attr("readonly", disable);
    $("#MaterialFee").attr("readonly", disable);
    $("#TotalLastInstallFee").attr("readonly", disable);
    $("#ProfitAmount").attr("readonly", disable);
    $("#CostRate").attr("readonly", disable);
    $("#InstallationMemo").attr("readonly", disable);
    $("#ChangeReasonCode").attr("disabled", disable);
    $("#ChangeRequestorCode").attr("disabled", disable);
}

function changeIEFirstCheckDate()
{
    if($("#IEFirstCheckDate").val() != "")
    {
        $("#Adjustment").attr("disabled",false);
    }
    else
    {
        $("#Adjustment").val("");
        $("#Adjustment").attr("disabled",true);

        $("#LastInspectionDate").val(""); //Add by Jutarat A. on 11102013
        $("#LastInspectionDate").datepicker("setDate", $("#LastInspectionDate").val()); //Add by Jutarat A. on 11102013
        
        //Add by Jutarat A. on 11112013
        $("#LastInspectionDate").EnableDatePicker(false);
        $("#AdjustmentContents").val("");
        $("#AdjustmentContents").attr("disabled", true);
        //End Add
    }
    if ($("#Adjustment").val() == C_INSTALL_ADJUSTMENT_NO_HAVE) {
        
        $("#LastInspectionDate").val($("#IEFirstCheckDate").val());
        $("#LastInspectionDate").datepicker("setDate", $("#IEFirstCheckDate").val());
        $("#LastInspectionDate").focus();
        $("#LastInspectionDate").blur();
    }
}


function copyToViewData(rowId) {
    var rowIndex = ISS090_GridManagementInfo.getRowIndex(rowId);
    $("#SelectedRowIndex").val(rowIndex);

    var colSubConName = ISS090_GridManagementInfo.getColIndexById("SubConName");
    var colSubConCode = ISS090_GridManagementInfo.getColIndexById("SubConCode");
    var colSubConGroupName = ISS090_GridManagementInfo.getColIndexById("SubConGroupName");
    var colInstallStartDate = ISS090_GridManagementInfo.getColIndexById("InstallStartDate");
    var colInstallCompleteDate = ISS090_GridManagementInfo.getColIndexById("InstallCompleteDate");
    var colExpectInstallStartDate = ISS090_GridManagementInfo.getColIndexById("ExpectInstallStartDate");
    var colExpectInstallCompleteDate = ISS090_GridManagementInfo.getColIndexById("ExpectInstallCompleteDate");
    var colManPower = ISS090_GridManagementInfo.getColIndexById("ManPower");
    var colIEFirstCheckDate = ISS090_GridManagementInfo.getColIndexById("IEFirstCheckDate");
    var colCheckChargeIEEmpNo = ISS090_GridManagementInfo.getColIndexById("CheckChargeIEEmpNo");
    var colEmpName = ISS090_GridManagementInfo.getColIndexById("EmpName");
    var colIEEvaluationCode = ISS090_GridManagementInfo.getColIndexById("IEEvaluationCode");
    var colComplainCode = ISS090_GridManagementInfo.getColIndexById("ComplainCode");
    var colAdjustmentCode = ISS090_GridManagementInfo.getColIndexById("AdjustmentCode");
    var colAdjustmentContentCode = ISS090_GridManagementInfo.getColIndexById("AdjustmentContentCode");
    var colIELastInspectionDate = ISS090_GridManagementInfo.getColIndexById("IELastInspectionDate");
    var colLastInstallationFee = ISS090_GridManagementInfo.getColIndexById("LastInstallationFee");

    //Add by Jutarat A. on 10102013
    var colNoCheck = ISS090_GridManagementInfo.getColIndexById("NoCheckFlag");
    var colLastPaidDate = ISS090_GridManagementInfo.getColIndexById("LastPaidDate");
    var colIMFee = ISS090_GridManagementInfo.getColIndexById("IMFee");
    var colOtherFee = ISS090_GridManagementInfo.getColIndexById("OtherFee");
    var colIMRemark = ISS090_GridManagementInfo.getColIndexById("IMRemark");
    var colActualPOAmount = ISS090_GridManagementInfo.getColIndexById("ActualPOAmount"); //Add by Jutarat A. on 08112013
    var colInstallationFee1 = ISS090_GridManagementInfo.getColIndexById("InstallationFee1");
    var colApproveNo1 = ISS090_GridManagementInfo.getColIndexById("ApproveNo1");
    var colPaidDate1 = ISS090_GridManagementInfo.getColIndexById("PaidDate1");
    var colInstallationFee2 = ISS090_GridManagementInfo.getColIndexById("InstallationFee2");
    var colApproveNo2 = ISS090_GridManagementInfo.getColIndexById("ApproveNo2");
    var colPaidDate2 = ISS090_GridManagementInfo.getColIndexById("PaidDate2");
    var colInstallationFee3 = ISS090_GridManagementInfo.getColIndexById("InstallationFee3");
    var colApproveNo3 = ISS090_GridManagementInfo.getColIndexById("ApproveNo3");
    var colPaidDate3 = ISS090_GridManagementInfo.getColIndexById("PaidDate3");
    //End Add

    amountSubConName = GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colSubConName);
    amountSubConCode = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colSubConCode);
    amountSubConGroupName = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colSubConGroupName);
    amountInstallStartDate = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colInstallStartDate);
    amountInstallCompleteDate = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colInstallCompleteDate);
    var valExpectInstallStartDate = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colExpectInstallStartDate);
    var valExpectInstallCompleteDate = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colExpectInstallCompleteDate);
    amountManPower = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colManPower);
    amountIEFirstCheckDate = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colIEFirstCheckDate);
    amountCheckChargeIEEmpNo = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colCheckChargeIEEmpNo);
    amountEmpName = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colEmpName);
    amountIEEvaluationCode = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colIEEvaluationCode);
    amountComplainCode = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colComplainCode);
    amountAdjustmentCode = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colAdjustmentCode);
    amountAdjustmentContentCode = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colAdjustmentContentCode);
    amountIELastInspectionDate = GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colIELastInspectionDate);

    var tmpLastInstallation = (GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colLastInstallationFee)).split(" ");
    amountLastInstallationFee = tmpLastInstallation[1];
    //amountLastInstallationFee = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colLastInstallationFee);

    //Add by Jutarat A. on 10102013
    var valNoCheck = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colNoCheck);
    var valLastPaidDate = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colLastPaidDate);
    var valIMFee = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colIMFee);
    var valOtherFee = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colOtherFee);
    var valIMRemark = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colIMRemark);
    var valActualPOAmount = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colActualPOAmount); //Add by Jutarat A. on 08112013
    var valInstallationFee1 = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colInstallationFee1);
    var valApproveNo1 = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colApproveNo1);
    var valPaidDate1 = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colPaidDate1);
    var valInstallationFee2 = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colInstallationFee2);
    var valApproveNo2 = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colApproveNo2);
    var valPaidDate2 = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colPaidDate2);
    var valInstallationFee3 = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colInstallationFee3);
    var valApproveNo3 = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colApproveNo3);
    var valPaidDate3 = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colPaidDate3);
    //End Add

    //Add by Jutarat A. on 08112013
    var decActualPOAmount = valActualPOAmount == "" ? 0 : valActualPOAmount;
    var decInstallationFee1 = valInstallationFee1 == "" ? 0 : valInstallationFee1;
    var decInstallationFee2 = valInstallationFee2 == "" ? 0 : valInstallationFee2;
    var decInstallationFee3 = valInstallationFee3 == "" ? 0 : valInstallationFee3;
    var decOtherFee = valOtherFee == "" ? 0 : valOtherFee;
    var decIMFee = valIMFee == "" ? 0 : valIMFee;

    var  valLastSubcontractorFee = decActualPOAmount - decInstallationFee1 - decInstallationFee2 - decInstallationFee3 - decOtherFee - decIMFee;
    //End Add

    $("#SubcontractorName").val(amountSubConName.substring(amountSubConName.indexOf(":")+1));
    $("#SubcontractorGroupName").val(amountSubConGroupName);

//    $("#InstallationStartDate").val(amountInstallStartDate);
//    $("#InstallationStartDate").datepicker("setDate", amountInstallStartDate);
//    $("#InstallationCompleteDate").val(amountInstallCompleteDate);
//    $("#InstallationCompleteDate").datepicker("setDate", amountInstallCompleteDate);

    SetDateFromToData("#InstallationStartDate", "#InstallationCompleteDate", amountInstallStartDate, amountInstallCompleteDate);
    SetDateFromToData("#ExpectInstallationStartDate", "#ExpectInstallationCompleteDate", valExpectInstallStartDate, valExpectInstallCompleteDate);

    $("#ManPower").val(amountManPower);
    $("#IEFirstCheckDate").val(amountIEFirstCheckDate);    
    $("#IEFirstCheckDate").datepicker("setDate", $("#IEFirstCheckDate").val());
    $("#CheckChargeIECode").val(amountCheckChargeIEEmpNo);
    $("#CheckChargeIEName").val(amountEmpName);
    $("#IEEvaluation").val(amountIEEvaluationCode);
    $("#Complain").val(amountComplainCode);
    $("#Adjustment").val(amountAdjustmentCode);
    $("#AdjustmentContents").val(amountAdjustmentContentCode);
    $("#LastInspectionDate").val(amountIELastInspectionDate);    
    $("#LastInspectionDate").datepicker("setDate", $("#LastInspectionDate").val());

    //Modify by Jutarat A. on 26032014
    ////$("#LastInstallationfee").val(amountLastInstallationFee);
    //$("#LastInstallationfee").val(SetNumericText(valLastSubcontractorFee, 2)); //Modify by Jutarat A. on 08112013
    if (enableEditButton) {
        $("#LastInstallationfee").val(SetNumericText(valLastSubcontractorFee, 2));
    }
    else {
        $("#LastInstallationfee").val(amountLastInstallationFee);
    }
    //End Modify

    //Add by Jutarat A. on 10102013
    valIMFee = valIMFee == "" ? valIMFee : SetNumericText(valIMFee, 2);
    valOtherFee = valOtherFee == "" ? valOtherFee : SetNumericText(valOtherFee, 2);
    valActualPOAmount = valActualPOAmount == "" ? valActualPOAmount : SetNumericText(valActualPOAmount, 2); //Add by Jutarat A. on 08112013
    valInstallationFee1 = valInstallationFee1 == "" ? valInstallationFee1 : SetNumericText(valInstallationFee1, 2);
    valInstallationFee2 = valInstallationFee2 == "" ? valInstallationFee2 : SetNumericText(valInstallationFee2, 2);
    valInstallationFee3 = valInstallationFee3 == "" ? valInstallationFee3 : SetNumericText(valInstallationFee3, 2);
    //End Add

    //Add by Jutarat A. on 10102013
    $("#chkNoCheck").attr("checked", valNoCheck == "true"? true:false);
    $("#LastPaidDate").val(valLastPaidDate);
    $("#LastPaidDate").datepicker("setDate", $("#LastPaidDate").val());
    $("#IMFee").val(valIMFee);
    $("#OtherFee").val(valOtherFee);
    $("#IMRemark").val(valIMRemark);
    $("#ActualPOAmount").val(valActualPOAmount); //Add by Jutarat A. on 08112013
    $("#InstallationFee1").val(valInstallationFee1);
    $("#ApproveNo1").val(valApproveNo1);
    $("#PaidDate1").val(valPaidDate1);
    $("#PaidDate1").datepicker("setDate", $("#PaidDate1").val());
    $("#InstallationFee2").val(valInstallationFee2);
    $("#ApproveNo2").val(valApproveNo2);
    $("#PaidDate2").val(valPaidDate2);
    $("#PaidDate2").datepicker("setDate", $("#PaidDate2").val());
    $("#InstallationFee3").val(valInstallationFee3);
    $("#ApproveNo3").val(valApproveNo3);
    $("#PaidDate3").val(valPaidDate3);
    $("#PaidDate3").datepicker("setDate", $("#PaidDate3").val());
    //End Add
}


function changeAdjustment() {
    if ($("#Adjustment").val() == C_INSTALL_ADJUSTMENT_HAVE) {
        $("#LastInspectionDate").EnableDatePicker(true);
        $("#AdjustmentContents").attr("disabled", false);
    }
    else if ($("#Adjustment").val() == C_INSTALL_ADJUSTMENT_NO_HAVE) {
        $("#LastInspectionDate").EnableDatePicker(false);
        $("#AdjustmentContents").attr("disabled", true);
        $("#AdjustmentContents").val("");
        $("#LastInspectionDate").val($("#IEFirstCheckDate").val());
        $("#LastInspectionDate").datepicker("setDate", $("#IEFirstCheckDate").val());
        $("#LastInspectionDate").focus();
        $("#LastInspectionDate").blur();
    }
    else {
        $("#LastInspectionDate").EnableDatePicker(false);
        $("#AdjustmentContents").attr("disabled", true);
        $("#AdjustmentContents").val("");
        $("#LastInspectionDate").val("");
        $("#LastInspectionDate").datepicker("setDate", $("#LastInspectionDate").val());
    }
}

//Comment by Jutarat A. on 07112013
/*function changeIEEvaluation() { 
    if($("#IEEvaluation").val() == "")
    {
        $("#LastInstallationfee").attr("readonly", true);
        $("#LastInstallationfee").val("");
    }
    else
    {
        $("#LastInstallationfee").attr("readonly", false);
        
    }
}*/
//End Comment

//Add by Jutarat A. on 08112013
function CalLastSubcontractorFee() {
    var decActualPOAmount = $("#ActualPOAmount").val() == "" ? 0 : $("#ActualPOAmount").NumericValue();
    var decInstallationFee1 = $("#InstallationFee1").val() == "" ? 0 : $("#InstallationFee1").NumericValue();
    var decInstallationFee2 = $("#InstallationFee2").val() == "" ? 0 : $("#InstallationFee2").NumericValue();
    var decInstallationFee3 = $("#InstallationFee3").val() == "" ? 0 : $("#InstallationFee3").NumericValue();
    var decOtherFee = $("#OtherFee").val() == "" ? 0 : $("#OtherFee").NumericValue();
    var decIMFee = $("#IMFee").val() == "" ? 0 : $("#IMFee").NumericValue();

    var  valLastSubcontractorFee = decActualPOAmount - decInstallationFee1 - decInstallationFee2 - decInstallationFee3 - decOtherFee - decIMFee;

    $("#LastInstallationfee").val(SetNumericText(valLastSubcontractorFee, 2));
}

function CalTotalActualPOAmount() {
    var decActualPOAmount = 0;
    var TotalActualPOAmount = 0;
    var currency = "";
    var colActualPOInfoAmount = ISS090_GridPOInfo.getColIndexById("ActualPOAmount");
    for (var i = 0; i < ISS090_GridPOInfo.getRowsNum()-1; i++) {
        decActualPOAmount = GetValueFromLinkType(ISS090_GridPOInfo, i, colActualPOInfoAmount);
        if (decActualPOAmount != undefined) {
            decActualPOAmount = decActualPOAmount.split(' ');

            if (decActualPOAmount.length >= 2) {
                TotalActualPOAmount = TotalActualPOAmount + decActualPOAmount[1].replace(/,/g, "") * 1;
                currency = decActualPOAmount[0];
            }
        }
    }

    ISS090_GridPOInfo.cells2(ISS090_GridPOInfo.getRowsNum() - 1, colActualPOInfoAmount).setValue(currency + " " + SetNumericText(TotalActualPOAmount, 2));

    var TotalLastInstalltion = 0;
    var colLastInstallation = ISS090_GridManagementInfo.getColIndexById("LastInstallationFee");
    for (var i = 0; i < ISS090_GridManagementInfo.getRowsNum() - 1; i++) {
        decLastInstallation = GetValueFromLinkType(ISS090_GridManagementInfo, i, colLastInstallation);
        if (decLastInstallation != undefined) {
            decLastInstallation = decLastInstallation.split(' ');

            if (decLastInstallation.length >= 2) {
                TotalLastInstalltion = TotalLastInstalltion + decLastInstallation[1].replace(/,/g, "") * 1;
            }
        }
    }
    $("#TotalLastInstallFee").val(SetNumericText(TotalLastInstalltion, 2));

    var ProfitAmount = 0;
    var CostRate = 0;
    if ($("#BillingInstallFee").val() != "" && $("#BillingInstallFee").NumericValue() != 0)
    {
        ProfitAmount = $("#BillingInstallFee").NumericValue() - TotalActualPOAmount;
        CostRate = (TotalActualPOAmount / $("#BillingInstallFee").NumericValue()) * 100;
    }

    if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL || $("#ServiceTypeCode").val() == C_SERVICE_TYPE_SALE) {
        $("#ProfitAmount").val(SetNumericText(ProfitAmount, 2));
        $("#CostRate").val(CostRate.toFixed(2) + "%");
    }
    else {
        $("#ProfitAmount").val("");
        $("#CostRate").val("");
    }
}
//End Add

function ManualInitialGridManagementInfo(blnCheckStatusComplete) {

    if (!CheckFirstRowIsEmpty(ISS090_GridManagementInfo)) {
        var TotallastInstallFee = 0;
        var ProfitAmount = 0;
        var CostRate = 0;
        var StatusCompletedFlag = true;
        var TotalOtherFee = 0; //Add by Jutarat A. on 08112013
        
        //Move by Jutarat A. on 08112013
        var colLastInstallFee = ISS090_GridManagementInfo.getColIndexById("LastInstallationFee");
        var colPaidFlag = ISS090_GridManagementInfo.getColIndexById("PaidFlag");
        var colNormalSubPOAmount = ISS090_GridManagementInfo.getColIndexById("NormalSubPOAmount");
        var colIELastInspectionDate = ISS090_GridManagementInfo.getColIndexById("IELastInspectionDate");
        //End Move

        var colLastPaidDate = ISS090_GridManagementInfo.getColIndexById("LastPaidDate"); //Add by Jutarat A. on 10102013
        var colOtherFee = ISS090_GridManagementInfo.getColIndexById("OtherFee"); //Add by Jutarat A. on 08112013
        var colIMFee = ISS090_GridManagementInfo.getColIndexById("IMFee"); //Add by Jutarat A. on 12112013

        for (var i = 0; i < ISS090_GridManagementInfo.getRowsNum(); i++) {
            var rowId = ISS090_GridManagementInfo.getRowId(i);

            var valLastInstallFee = GetValueFromLinkType(ISS090_GridManagementInfo, i, colLastInstallFee);
            var valPaidFlag = GetValueFromLinkType(ISS090_GridManagementInfo, i, colPaidFlag);
            var valNormalSubPOAmount = GetValueFromLinkType(ISS090_GridManagementInfo, i, colNormalSubPOAmount);
            var valIELastInspectionDate = GetValueFromLinkType(ISS090_GridManagementInfo, i, colIELastInspectionDate);
            var valLastPaidDate = GetValueFromLinkType(ISS090_GridManagementInfo, i, colLastPaidDate); //Add by Jutarat A. on 10102013

            if (valLastInstallFee.replace(/,/, "") * 1 > 0 && valNormalSubPOAmount.replace(/,/g, "") * 1 > 0)
            {
                TotallastInstallFee = TotallastInstallFee + (valLastInstallFee.replace(/,/, "") * 1);
                if ( StatusCompletedFlag != false && (valPaidFlag + "").toUpperCase() != C_FLAG_ON.toUpperCase())
                {
                    StatusCompletedFlag = false;
                }
            }

            //Add by Jutarat A. on 08112013
            var valOtherFee = GetValueFromLinkType(ISS090_GridManagementInfo, i, colOtherFee);
            var valIMFee = GetValueFromLinkType(ISS090_GridManagementInfo, i, colIMFee); //Add by Jutarat A. on 12112013
             
            TotalOtherFee = TotalOtherFee + (valOtherFee*1) + (valIMFee*1);
            //End Add

            if($("#strMode").val() == C_FUNC_ID_OPERATE)
            {
                enablePaidButton = false;
            }
            else if ($("#strMode").val() == C_FUNC_ID_COMPLETE)
            {
                //if (valLastInstallFee.replace(/,/, "") * 1 > 0 && (valPaidFlag + "").toUpperCase() != C_FLAG_ON.toUpperCase() && valNormalSubPOAmount.replace(/,/g, "") * 1 > 0) 
                if (valLastInstallFee.replace(/,/, "") * 1 > 0 && (valPaidFlag + "").toUpperCase() != C_FLAG_ON.toUpperCase() && valLastPaidDate != null && valLastPaidDate != "") //Modify by Jutarat A. on 10102013
                {
                    enablePaidButton = true;
                }
                else {
                    enablePaidButton = false;
                }
            }

            //===================== Generate CheckBox AdvancePaymentFlag =====================
            var enableAdvancePayment = false;
            //if ( $("#strMode").val() == C_FUNC_ID_OPERATE && permissionAdvancePay && valIELastInspectionDate != "" && valLastInstallFee.replace(/,/, "") * 1 > 0 && valNormalSubPOAmount.replace(/,/g, "") * 1 > 0 && (valPaidFlag + "").toUpperCase() != C_FLAG_ON.toUpperCase())
            if ( $("#strMode").val() == C_FUNC_ID_OPERATE && permissionAdvancePay && valIELastInspectionDate != "" && valLastInstallFee.replace(/,/, "") * 1 > 0 && (valPaidFlag + "").toUpperCase() != C_FLAG_ON.toUpperCase()) //Modify by Jutarat A. on 10102013
            {
                enableAdvancePayment = true;
            }

            var AdvancePaymentChkId = GenerateGridControlID("AdvancePaymentChk", rowId);
            var FlagAdvancePayment = false;
            if(ISS090_GridManagementInfo.cells2(i, ISS090_GridManagementInfo.getColIndexById('AdvancePaymentFlag')).getValue() == "true")
            {
                ISS090_GridManagementInfo.cells2(i, ISS090_GridManagementInfo.getColIndexById('AdvancePaymentFlag')).setValue(GenerateCheckBox2("AdvancePaymentChk", rowId, "", enableAdvancePayment,true));    
            }
            else
            {
                if($("#"+AdvancePaymentChkId).prop("checked"))
                {
                    ISS090_GridManagementInfo.cells2(i, ISS090_GridManagementInfo.getColIndexById('AdvancePaymentFlag')).setValue(GenerateCheckBox2("AdvancePaymentChk", rowId, "", enableAdvancePayment,true));    
                }
                else
                {
                    ISS090_GridManagementInfo.cells2(i, ISS090_GridManagementInfo.getColIndexById('AdvancePaymentFlag')).setValue(GenerateCheckBox2("AdvancePaymentChk", rowId, "", enableAdvancePayment,false));    
                }                
            }

            BindGridCheckBoxClickEvent("AdvancePaymentChk", rowId, AdvancePayment_Click);
            $("#"+AdvancePaymentChkId).attr("title",lblAdvancePayment)
            
            if($("#"+AdvancePaymentChkId).prop("checked") && (valPaidFlag + "").toUpperCase() != C_FLAG_ON.toUpperCase())
            {
                FlagAdvancePayment = true;
            }
            //================================================================================

            
            var strViewEditBtn = "";
            if (enableEditButton) {
                strViewEditBtn = "Edit";
            }
            else if (enableViewButton) { 
                strViewEditBtn = "View";
            }

            //ISS090_GridManagementInfo.cells2(i, ISS090_GridManagementInfo.getColIndexById('ButtonEdit')).setValue(GenerateHtmlButton("btnEdit", rowId, strViewEditBtn, true));
            blnCanEdit = false;
            if($("#btnUpdate").is(':hidden'))
            {
                blnCanEdit = true;
            }
            //GenerateEditButton(ISS090_GridManagementInfo, "btnEdit", rowId, "ButtonEdit", blnCanEdit);
            GenerateImageButtonToGrid(ISS090_GridManagementInfo, "btnEdit", rowId, "ButtonEdit", blnCanEdit, "editBlue.png", "Edit");
            
            if(valNormalSubPOAmount.replace(/,/g, "") * 1 > 0)
            {                
                ISS090_GridManagementInfo.cells2(i, ISS090_GridManagementInfo.getColIndexById('ButtonPaid')).setValue(GenerateHtmlButton("btnPaid", rowId, lblPaid, enablePaidButton));
                GenerateDownloadButton(ISS090_GridManagementInfo, "btnIssue", rowId, "ButtonIssue", enableIssueButton);
            }
            //Comment by Jutarat A. on 10102013
            //else
            //{
            //    ISS090_GridManagementInfo.cells2(i, ISS090_GridManagementInfo.getColIndexById('ButtonPaid')).setValue(GenerateHtmlButton("btnPaid", rowId, lblPaid, false));
            //    GenerateDownloadButton(ISS090_GridManagementInfo, "btnIssue", rowId, "ButtonIssue", false);
            //}
            //End Comment

            if(FlagAdvancePayment)
            {
                ISS090_GridManagementInfo.cells2(i, ISS090_GridManagementInfo.getColIndexById('ButtonPaid')).setValue(GenerateHtmlButton("btnPaid", rowId, lblPaid, true));
            }

            //ISS090_GridManagementInfo.cells2(i, ISS090_GridManagementInfo.getColIndexById('ButtonIssue')).setValue(GenerateHtmlButton("btnIssue", rowId, "Issue", enableIssueButton));
//            BindGridHtmlButtonClickEvent("btnEdit", rowId, function (rid) {
//                if (enableEditButton) {
//                    DisabledInstallManageSubSection(false);
//                }
//                copyToViewData(rid);
//            });

            

            BindGridButtonClickEvent("btnEdit", rowId, function (rid) {
                copyToViewData(rid);
                //=============== Check subconpoamount to disable all button ===================
                var rowIndex = ISS090_GridManagementInfo.getRowIndex(rid);
                var colNormalSubPOAmount = ISS090_GridManagementInfo.getColIndexById("NormalSubPOAmount");
                var valNormalSubPOAmount = GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colNormalSubPOAmount);
                //==============================================================================
                if (enableEditButton && valNormalSubPOAmount.replace(/,/g, "") * 1 > 0 && (valPaidFlag + "").toUpperCase() != C_FLAG_ON.toUpperCase()) {
                    VaridateCtrl(["IEFirstCheckDate", "InstallationStartDate", "LastInspectionDate","Complain"], null);
                    DisabledInstallManageSubSection(false);

                    changeIEFirstCheckDate();
                    changeAdjustment();

                    //Comment by Jutarat A. on 07112013
                    /*//Modify by Jutarat A. on 07112013
                    //changeIEEvaluation();
                    if ($("#chkNoCheck").prop("checked")) {
                        $("#LastInstallationfee").SetDisabled(false);
                    }
                    else {
                        changeIEEvaluation();
                    }
                    //End Modify*/
                    //End Comment

                    ManualInitialGridManagementInfo(false);
                    
                    command_control.CommandControlMode(false);
                }

            });
            var btnPaidId = GenerateGridControlID("btnPaid", rowId);
            $("#" + btnPaidId).css('width', '60px');

            BindGridHtmlButtonClickEvent("btnPaid", rowId, function (rid) {
                PaidButton_Click(rid);
            });
//            BindGridHtmlButtonClickEvent("btnIssue", rowId, function (rid) {
//                IssueButton_Click(rid);
            //            });
            BindGridButtonClickEvent("btnIssue", rowId, IssueButton_Click);

        }

        if (StatusCompletedFlag && blnCheckStatusComplete &&  $("#strMode").val() == C_FUNC_ID_COMPLETE) {
            $("#strMode").val(C_FUNC_ID_COMPLETE);
            UpdateManagementStatus(C_INSTALL_MANAGE_STATUS_COMPLETED);
            SetScreenMode();

            SetRegisterCommand(false, null);
            SetResetCommand(false, null);
            SetApproveCommand(false, null);
            SetRejectCommand(false, null);
            SetReturnCommand(false, null);
            SetCloseCommand(false, null);
            SetConfirmCommand(false, null);
            SetBackCommand(false, null);

            // Get Message
            var obj = {
                module: "Installation",
                code: "MSG5095",
                
            };
            call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                OpenInformationMessageDialog(result.Code, result.Message, function () {
                            
                });
            });
        }
        
        //Modify by Jutarat A. on 08112013
        /*if ($("#BillingInstallFee").val() != "" && $("#BillingInstallFee").NumericValue() != 0)
        {
            ProfitAmount = $("#BillingInstallFee").NumericValue() - TotallastInstallFee;
            CostRate = (TotallastInstallFee / $("#BillingInstallFee").NumericValue()) * 100;
            CostRate = CostRate.toFixed(2);
        }
        $("#TotalLastInstallFee").val(moneyConvert(TotallastInstallFee));

        if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL || $("#ServiceTypeCode").val() == C_SERVICE_TYPE_SALE) {
            $("#ProfitAmount").val(moneyConvert(ProfitAmount.toFixed(2)));
            $("#CostRate").val(CostRate + "%");
        }
        else {
            $("#ProfitAmount").val("");
            $("#CostRate").val("");
        }*/
        CalTotalActualPOAmount();
        //End Modify

        $("#MaterialFee").val(SetNumericText(TotalOtherFee, 2)); //Add by Jutarat A. on 08112013
    }
}

function UpdateInstallManagementSubsection() {
    
    VaridateCtrl(["IEFirstCheckDate", "InstallationStartDate", "LastInspectionDate","Complain"], null);
    if (convertDatetoYMD($("#IEFirstCheckDate")) * 1 < convertDatetoYMD($("#InstallationStartDate")) * 1) {
        //doAlert("Installation", "MSG5037", "");
        doAlert("Installation", "MSG5037", [lblIEFirstCheckDate, lblInstallationStartDate]);
        VaridateCtrl(["IEFirstCheckDate", "InstallationStartDate"], ["IEFirstCheckDate", "InstallationStartDate"]);
        return false;
    }else if (convertDatetoYMD($("#LastInspectionDate")) * 1 < convertDatetoYMD($("#IEFirstCheckDate")) * 1) {
        //doAlert("Installation", "MSG5037", "");
        doAlert("Installation", "MSG5037", [lblIELastInspectionDate, lblIEFirstCheckDate]);
        VaridateCtrl(["IEFirstCheckDate", "LastInspectionDate"], ["IEFirstCheckDate", "LastInspectionDate"]);
        return false;
    }

    if ($("#CheckChargeIECode").val() != "" && $("#CheckChargeIEName").val() == "") {
        doAlert("Common", "MSG0095", [$("#CheckChargeIECode").val()]);
        return false;
    }

    if($("#IEFirstCheckDate").val() == "" && $("#Complain").val() == C_INSTALL_COMPLAIN_HAVE)
    {
        doAlert("Installation", "MSG5105", "");
        VaridateCtrl(["IEFirstCheckDate", "Complain"], ["IEFirstCheckDate", "Complain"]);
        return false;
    }
//    if ($("#CheckChargeIECode").val() != "") {
//        loadEmpName($("#CheckChargeIECode").val(), $("#CheckChargeIECode"), $("#CheckChargeIEName"));
//    }
//    else {
//        $("#CheckChargeIEName").val("");
//    }
    var rowIndex = $("#SelectedRowIndex").val();
    var colSubConCode = ISS090_GridManagementInfo.getColIndexById("SubConCode");
    var colSubConGroupName = ISS090_GridManagementInfo.getColIndexById("SubConGroupName");
    var colInstallStartDate = ISS090_GridManagementInfo.getColIndexById("InstallStartDate");
    var colInstallCompleteDate = ISS090_GridManagementInfo.getColIndexById("InstallCompleteDate");
    var colExpectInstallStartDate = ISS090_GridManagementInfo.getColIndexById("ExpectInstallStartDate");
    var colExpectInstallCompleteDate = ISS090_GridManagementInfo.getColIndexById("ExpectInstallCompleteDate");
    var colManPower = ISS090_GridManagementInfo.getColIndexById("ManPower");
    var colIEFirstCheckDate = ISS090_GridManagementInfo.getColIndexById("IEFirstCheckDate");
    var colCheckChargeIEEmpNo = ISS090_GridManagementInfo.getColIndexById("CheckChargeIEEmpNo");
    var colEmpName = ISS090_GridManagementInfo.getColIndexById("EmpName");
    var colIEEvaluationCode = ISS090_GridManagementInfo.getColIndexById("IEEvaluationCode");
    var colComplainCode = ISS090_GridManagementInfo.getColIndexById("ComplainCode");
    var colAdjustmentCode = ISS090_GridManagementInfo.getColIndexById("AdjustmentCode");
    var colAdjustmentContentCode = ISS090_GridManagementInfo.getColIndexById("AdjustmentContentCode");
    var colIELastInspectionDate = ISS090_GridManagementInfo.getColIndexById("IELastInspectionDate");
    var colLastInstallationFee = ISS090_GridManagementInfo.getColIndexById("LastInstallationFee");

    var colInstallDateDisplay = ISS090_GridManagementInfo.getColIndexById("InstallDateDisplay");
    var colIEDateDisplay = ISS090_GridManagementInfo.getColIndexById("IEDateDisplay");

    //Add by Jutarat A. on 10102013
    var colNoCheck = ISS090_GridManagementInfo.getColIndexById("NoCheckFlag");
    var colLastPaidDate = ISS090_GridManagementInfo.getColIndexById("LastPaidDate");
    var colIMFee = ISS090_GridManagementInfo.getColIndexById("IMFee");
    var colOtherFee = ISS090_GridManagementInfo.getColIndexById("OtherFee");
    var colIMRemark = ISS090_GridManagementInfo.getColIndexById("IMRemark");
    var colActualPOAmount = ISS090_GridManagementInfo.getColIndexById("ActualPOAmount"); //Add by Jutarat A. on 08112013
    var colInstallationFee1 = ISS090_GridManagementInfo.getColIndexById("InstallationFee1");
    var colApproveNo1 = ISS090_GridManagementInfo.getColIndexById("ApproveNo1");
    var colPaidDate1 = ISS090_GridManagementInfo.getColIndexById("PaidDate1");
    var colInstallationFee2 = ISS090_GridManagementInfo.getColIndexById("InstallationFee2");
    var colApproveNo2 = ISS090_GridManagementInfo.getColIndexById("ApproveNo2");
    var colPaidDate2 = ISS090_GridManagementInfo.getColIndexById("PaidDate2");
    var colInstallationFee3 = ISS090_GridManagementInfo.getColIndexById("InstallationFee3");
    var colApproveNo3 = ISS090_GridManagementInfo.getColIndexById("ApproveNo3");
    var colPaidDate3 = ISS090_GridManagementInfo.getColIndexById("PaidDate3");
    //End Add

//    ISS090_GridManagementInfo.cells2(rowIndex, colSubConCode).setValue($("#SubcontractorName").val());
//    ISS090_GridManagementInfo.cells2(rowIndex, colSubConGroupName).setValue($("#SubcontractorGroupName").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colInstallStartDate).setValue($("#InstallationStartDate").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colInstallCompleteDate).setValue($("#InstallationCompleteDate").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colExpectInstallStartDate).setValue($("#ExpectInstallationStartDate").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colExpectInstallCompleteDate).setValue($("#ExpectInstallationCompleteDate").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colManPower).setValue($("#ManPower").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colIEFirstCheckDate).setValue($("#IEFirstCheckDate").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colCheckChargeIEEmpNo).setValue($("#CheckChargeIECode").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colEmpName).setValue($("#CheckChargeIEName").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colIEEvaluationCode).setValue($("#IEEvaluation").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colComplainCode).setValue($("#Complain").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colAdjustmentCode).setValue($("#Adjustment").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colAdjustmentContentCode).setValue($("#AdjustmentContents").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colIELastInspectionDate).setValue($("#LastInspectionDate").val());
    //ISS090_GridManagementInfo.cells2(rowIndex, colLastInstallationFee).setValue($("#LastInstallationfee").val());
    
    ISS090_GridManagementInfo.cells2(rowIndex, colInstallDateDisplay).setValue("(1) " + (($("#InstallationStartDate").val()=="")?"-":$("#InstallationStartDate").val())+"<br />(2) "+(($("#InstallationCompleteDate").val()=="")?"-":$("#InstallationCompleteDate").val()));
    ISS090_GridManagementInfo.cells2(rowIndex, colIEDateDisplay).setValue("(1) " + (($("#IEFirstCheckDate").val()== "")?"-":$("#IEFirstCheckDate").val())+"<br />(2) "+(($("#LastInspectionDate").val()=="")?"-":$("#LastInspectionDate").val()));
    
    //Add by Jutarat A. on 10102013
    ISS090_GridManagementInfo.cells2(rowIndex, colNoCheck).setValue(($("#chkNoCheck").prop("checked")).toString());
    ISS090_GridManagementInfo.cells2(rowIndex, colLastPaidDate).setValue($("#LastPaidDate").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colIMFee).setValue($("#IMFee").NumericValue());
    ISS090_GridManagementInfo.cells2(rowIndex, colOtherFee).setValue($("#OtherFee").NumericValue());
    ISS090_GridManagementInfo.cells2(rowIndex, colIMRemark).setValue($("#IMRemark").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colActualPOAmount).setValue($("#ActualPOAmount").NumericValue()); //Add by Jutarat A. on 08112013
    ISS090_GridManagementInfo.cells2(rowIndex, colInstallationFee1).setValue($("#InstallationFee1").NumericValue());
    ISS090_GridManagementInfo.cells2(rowIndex, colApproveNo1).setValue($("#ApproveNo1").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colPaidDate1).setValue($("#PaidDate1").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colInstallationFee2).setValue($("#InstallationFee2").NumericValue());
    ISS090_GridManagementInfo.cells2(rowIndex, colApproveNo2).setValue($("#ApproveNo2").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colPaidDate2).setValue($("#PaidDate2").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colInstallationFee3).setValue($("#InstallationFee3").NumericValue());
    ISS090_GridManagementInfo.cells2(rowIndex, colApproveNo3).setValue($("#ApproveNo3").val());
    ISS090_GridManagementInfo.cells2(rowIndex, colPaidDate3).setValue($("#PaidDate3").val());
    //End Add

    //Add by Jutarat A. on 08112013
    var colActualPOInfoAmount = ISS090_GridPOInfo.getColIndexById("ActualPOAmount");
    var valActualPOInfoAmount = $("#ActualPOAmount").val() == "" ? 0 : $("#ActualPOAmount").NumericValue();

    ISS090_GridPOInfo.cells2(rowIndex, colActualPOInfoAmount).setValue($("#ActualPOAmount").NumericCurrencyText() + " " + SetNumericText(valActualPOInfoAmount, 2));
    //End Add

    VaridateCtrl(["IEFirstCheckDate", "InstallationStartDate", "LastInspectionDate","Complain"], null);
    
    CancelInstallManagementSubsection();
    ManualInitialGridManagementInfo(false);

    var TotalLastInstalltion = 0;
    var colLastInstallation = ISS090_GridManagementInfo.getColIndexById("LastInstallationFee");
    for (var i = 0; i < ISS090_GridManagementInfo.getRowsNum() ; i++) {
        decLastInstallation = GetValueFromLinkType(ISS090_GridManagementInfo, i, colLastInstallation);
        if (decLastInstallation != undefined) {
            decLastInstallation = decLastInstallation.split(' ');

            if (decLastInstallation.length >= 2) {
                TotalLastInstalltion = TotalLastInstalltion + decLastInstallation[1].replace(/,/g, "") * 1;
            }
        }
    }
    $("#TotalLastInstallFee").val(SetNumericText(TotalLastInstalltion, 2));
}


function CancelInstallManagementSubsection() {

    $("#SubcontractorName").val("");
    $("#SubcontractorGroupName").val("");
    $("#InstallationStartDate").val("");    
    $("#InstallationStartDate").datepicker("setDate", $("#InstallationStartDate").val());
    $("#InstallationCompleteDate").val("");
    $("#InstallationCompleteDate").datepicker("setDate", $("#InstallationCompleteDate").val());
    $("#ExpectInstallationStartDate").val("");    
    $("#ExpectInstallationStartDate").datepicker("setDate", $("#ExpectInstallationStartDate").val());
    $("#ExpectInstallationCompleteDate").val("");
    $("#ExpectInstallationCompleteDate").datepicker("setDate", $("#ExpectInstallationCompleteDate").val());
    $("#ManPower").val("");
    $("#IEFirstCheckDate").val("");
    $("#IEFirstCheckDate").datepicker("setDate", $("#IEFirstCheckDate").val());
    $("#CheckChargeIECode").val("");
    $("#CheckChargeIEName").val("");
    $("#IEEvaluation").val("");
    $("#Complain").val("");
    $("#Adjustment").val("");
    $("#AdjustmentContents").val("");
    $("#LastInspectionDate").val("");
    $("#LastInspectionDate").datepicker("setDate", $("#LastInspectionDate").val());
    $("#LastInstallationfee").val("");

    //Add by Jutarat A. on 10102013
    $("#chkNoCheck").attr("checked", false)
    $("#LastPaidDate").val("");
    $("#LastPaidDate").datepicker("setDate", $("#LastPaidDate").val());
    $("#IMFee").val("");
    $("#OtherFee").val("");
    $("#IMRemark").val("");
    $("#ActualPOAmount").val(""); //Add by Jutarat A. on 08112013
    $("#InstallationFee1").val("");
    $("#ApproveNo1").val("");
    $("#PaidDate1").val("");
    $("#PaidDate1").datepicker("setDate", $("#PaidDate1").val());
    $("#InstallationFee2").val("");
    $("#ApproveNo2").val("");
    $("#PaidDate2").val("");
    $("#PaidDate2").datepicker("setDate", $("#PaidDate2").val());
    $("#InstallationFee3").val("");
    $("#ApproveNo3").val("");
    $("#PaidDate3").val("");
    $("#PaidDate3").datepicker("setDate", $("#PaidDate3").val());
    //End Add

    VaridateCtrl(["IEFirstCheckDate", "InstallationStartDate", "LastInspectionDate","Complain"], null);

    ClearDateFromToControl("#InstallationStartDate", "#InstallationCompleteDate");
    ClearDateFromToControl("#ExpectInstallationStartDate", "#ExpectInstallationCompleteDate");
    
    DisabledInstallManageSubSection(true);
}

function PaidButton_Click(rowId) {
    // Get Message
    var obj = {
        module: "Installation",
        code: "MSG5081"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    var rowIndex = ISS090_GridManagementInfo.getRowIndex(rowId);
                    var colPaidFlag = ISS090_GridManagementInfo.getColIndexById("PaidFlag");
                    var colSubConCode = ISS090_GridManagementInfo.getColIndexById("SubConCode");
                    var strSubConCode = GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colSubConCode);
                    ISS090_GridManagementInfo.cells2(rowIndex, colPaidFlag).setValue(C_FLAG_ON);
                    var obj = { SubcontractorCode: strSubConCode }
                    call_ajax_method_json("/Installation/ISS090_PaidButtonClick", obj, function (result, controls) {
                        ManualInitialGridManagementInfo(true);
                    });
                    
                });
    });
}

function IssueButton_Click(rowId) {
    var selectedRowIndex = ISS090_GridManagementInfo.getRowIndex(rowId);
    var colSubConCode = ISS090_GridManagementInfo.getColIndexById("SubConCode");
    var strSubConCode = GetValueFromLinkType(ISS090_GridManagementInfo, selectedRowIndex, colSubConCode);

    var key = ajax_method.GetKeyURL(null);
    var link = ajax_method.GenerateURL("/Installation/ISS090_CreateReportAcceptInspecNotice?MaintenanceNo=" + $("#MaintenanceNo").val() + "&strSubContracttorCode=" + strSubConCode + "&k=" + key);
    window.open(link, "download1");
    
//    var obj = { SubcontractorCode: strSubConCode }
//    call_ajax_method_json("/Installation/ISS090_IssueButtonClick", obj, function (result, controls) {
//          
//    });
}


function SendGridDetailsToObject() {
    if (ISS090_GridManagementInfo.hdr.rows.length > 0) {
        var objArray = new Array();

        if (CheckFirstRowIsEmpty(ISS090_GridManagementInfo) == false) {
            
            //Add by Jutarat A. on 11102013
            var colNoCheck = ISS090_GridManagementInfo.getColIndexById("NoCheckFlag");
            var colLastPaidDate = ISS090_GridManagementInfo.getColIndexById("LastPaidDate");
            var colIMFee = ISS090_GridManagementInfo.getColIndexById("IMFee");
            var colOtherFee = ISS090_GridManagementInfo.getColIndexById("OtherFee");
            var colIMRemark = ISS090_GridManagementInfo.getColIndexById("IMRemark");
            var colActualPOAmount = ISS090_GridManagementInfo.getColIndexById("ActualPOAmount"); //Add by Jutarat A. on 08112013
            var colInstallationFee1 = ISS090_GridManagementInfo.getColIndexById("InstallationFee1");
            var colApproveNo1 = ISS090_GridManagementInfo.getColIndexById("ApproveNo1");
            var colPaidDate1 = ISS090_GridManagementInfo.getColIndexById("PaidDate1");
            var colInstallationFee2 = ISS090_GridManagementInfo.getColIndexById("InstallationFee2");
            var colApproveNo2 = ISS090_GridManagementInfo.getColIndexById("ApproveNo2");
            var colPaidDate2 = ISS090_GridManagementInfo.getColIndexById("PaidDate2");
            var colInstallationFee3 = ISS090_GridManagementInfo.getColIndexById("InstallationFee3");
            var colApproveNo3 = ISS090_GridManagementInfo.getColIndexById("ApproveNo3");
            var colPaidDate3 = ISS090_GridManagementInfo.getColIndexById("PaidDate3");
            //End Add

            for (var i = 0; i < ISS090_GridManagementInfo.getRowsNum(); i++) {
                var rowId = ISS090_GridManagementInfo.getRowId(i);
                var rowIndex = i;
                //================================= GetColumn Index =================================
                var colSubConCode = ISS090_GridManagementInfo.getColIndexById("SubConCode");
                var colSubConGroupName = ISS090_GridManagementInfo.getColIndexById("SubConGroupName");
                var colInstallStartDate = ISS090_GridManagementInfo.getColIndexById("InstallStartDate");
                var colInstallCompleteDate = ISS090_GridManagementInfo.getColIndexById("InstallCompleteDate");
                var colExpectInstallStartDate = ISS090_GridManagementInfo.getColIndexById("ExpectInstallStartDate");
                var colExpectInstallCompleteDate = ISS090_GridManagementInfo.getColIndexById("ExpectInstallCompleteDate");
                var colManPower = ISS090_GridManagementInfo.getColIndexById("ManPower");
                var colIEFirstCheckDate = ISS090_GridManagementInfo.getColIndexById("IEFirstCheckDate");
                var colCheckChargeIEEmpNo = ISS090_GridManagementInfo.getColIndexById("CheckChargeIEEmpNo");
                var colEmpName = ISS090_GridManagementInfo.getColIndexById("EmpName");
                var colIEEvaluationCode = ISS090_GridManagementInfo.getColIndexById("IEEvaluationCode");
                var colComplainCode = ISS090_GridManagementInfo.getColIndexById("ComplainCode");
                var colAdjustmentCode = ISS090_GridManagementInfo.getColIndexById("AdjustmentCode");
                var colAdjustmentContentCode = ISS090_GridManagementInfo.getColIndexById("AdjustmentContentCode");
                var colIELastInspectionDate = ISS090_GridManagementInfo.getColIndexById("IELastInspectionDate");
                var colLastInstallationFee = ISS090_GridManagementInfo.getColIndexById("LastInstallationFee");
          
                var AdvancePaymentChkId = GenerateGridControlID("AdvancePaymentChk", rowId);
                

                amountSubConCode = GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colSubConCode);
                amountSubConGroupName = GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colSubConGroupName);
                amountInstallStartDate = GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colInstallStartDate);
                amountInstallCompleteDate = GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colInstallCompleteDate);
                valExpectInstallStartDate = GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colExpectInstallStartDate);
                valExpectInstallCompleteDate = GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colExpectInstallCompleteDate);
                amountManPower = GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colManPower);
                amountIEFirstCheckDate = GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colIEFirstCheckDate);
                amountCheckChargeIEEmpNo = GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colCheckChargeIEEmpNo);
                amountEmpName = GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colEmpName);
                amountIEEvaluationCode = GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colIEEvaluationCode);
                amountComplainCode = GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colComplainCode);
                amountAdjustmentCode = GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colAdjustmentCode);
                amountAdjustmentContentCode = GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colAdjustmentContentCode);
                amountIELastInspectionDate = GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colIELastInspectionDate);

                var tmpLastInstallation = (GetValueFromLinkType(ISS090_GridManagementInfo, rowIndex, colLastInstallationFee)).split(" ");
                amountLastInstallationFee = tmpLastInstallation[1];
                //amountLastInstallationFee = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colLastInstallationFee);

                //Add by Jutarat A. on 11102013
                var valNoCheck = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colNoCheck);
                var valLastPaidDate = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colLastPaidDate);
                var valIMFee = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colIMFee);
                var valOtherFee = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colOtherFee);
                var valIMRemark = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colIMRemark);
                var valActualPOAmount = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colActualPOAmount); //Add by Jutarat A. on 08112013
                var valInstallationFee1 = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colInstallationFee1);
                var valApproveNo1 = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colApproveNo1);
                var valPaidDate1 = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colPaidDate1);
                var valInstallationFee2 = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colInstallationFee2);
                var valApproveNo2 = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colApproveNo2);
                var valPaidDate2 = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colPaidDate2);
                var valInstallationFee3 = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colInstallationFee3);
                var valApproveNo3 = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colApproveNo3);
                var valPaidDate3 = GetValueFromLinkType(ISS090_GridManagementInfo,rowIndex, colPaidDate3);
                //End Add

                if(amountLastInstallationFee != null)
                    amountLastInstallationFee = amountLastInstallationFee.replace(/,/g,"");
                if(amountManPower != null)
                    amountManPower = amountManPower.replace(/,/g,"");

                var iobj = {
                    AdvancePaymentFlag: $("#"+AdvancePaymentChkId).prop("checked") 
                    ,SubcontractorCode: amountSubConCode
                    , SubcontractorGroupName: amountSubConGroupName
                    ,InstallStartDate: amountInstallStartDate
                    ,InstallCompleteDate: amountInstallCompleteDate
                    ,ExpectInstallStartDate: valExpectInstallStartDate
                    ,ExpectInstallCompleteDate: valExpectInstallCompleteDate
                    ,ManPower: amountManPower
                    ,IEFirstCheckDate: amountIEFirstCheckDate
                    ,CheckChargeIEEmpNo: amountCheckChargeIEEmpNo
                    ,IEEvaluationCode: amountIEEvaluationCode
                    ,ComplainCode: amountComplainCode
                    ,AdjustmentCode: amountAdjustmentCode
                    ,AdjustmentContentCode: amountAdjustmentContentCode
                    ,IELastInspectionDate: amountIELastInspectionDate
                    , LastInstallationFee: amountLastInstallationFee
                    //Add by Jutarat A. on 11102013
                    , NoCheckFlag: valNoCheck
                    , LastPaidDate: valLastPaidDate
                    , IMFee: valIMFee
                    , OtherFee: valOtherFee
                    , IMRemark: valIMRemark
                    , InstallationFee1: valInstallationFee1
                    , ApproveNo1: valApproveNo1
                    , PaidDate1: valPaidDate1
                    , InstallationFee2: valInstallationFee2
                    , ApproveNo2: valApproveNo2
                    , PaidDate2: valPaidDate2
                    , InstallationFee3: valInstallationFee3
                    , ApproveNo3: valApproveNo3
                    , PaidDate3: valPaidDate3
                    //End Add
                    , ActualPOAmount: valActualPOAmount //Add byjutarat A. on 08112013
                };
                objArray.push(iobj);
            }
        }

        var obj = {
            ListPOInfo: objArray        
        };
        /* --------------------- */

        /* --- Check and Add event --- */
        /* --------------------------- */
        call_ajax_method_json("/Installation/ISS090_SendGridDetailsData", obj, function (result, controls) {
            if (controls != undefined) {
                /* --- Higlight Text --- */
                /* --------------------- */
                //VaridateCtrl(["EmailAddress"], controls);
                /* --------------------- */

                return;
            }
            else if (result != undefined) {

            }

        });
    }

}


function command_requestapprove_click() {
    command_control.CommandControlMode(false);
    SendGridDetailsToObject();
    var obj = CreateObjectData($("#form1").serialize());

    call_ajax_method_json("/Installation/ISS090_ValidateBeforeRequestApprove", obj, function (result, controls) {
        command_control.CommandControlMode(true);
        if (controls != undefined) {
            VaridateCtrl(["MaterialFee","IEManPower"], controls);
        }
        else if (result == true) {

            // Get Message
            var obj = {
                module: "Installation",
                code: "MSG5005",
                
            };

            call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                OpenYesNoMessageDialog(result.Code, result.Message, function () {
                    var obj2 = CreateObjectData($("#form1").serialize());
                    call_ajax_method_json("/Installation/ISS090_RequestApprove", obj2, function (result, controls) {
                       
                    });
                     $("#strInstallationManagementStatus").val(C_INSTALL_MANAGE_STATUS_REQUEST_APPROVE);
                    SetScreenMode();
                    
                    // Get Message
                    var obj = {
                        module: "Installation",
                        code: "MSG5090",
                
                    };
                    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                        OpenInformationMessageDialog(result.Code, result.Message, function () {
                            
                        });
                    });
                    
                    SetRegisterCommand(false, null);
                    SetResetCommand(false, null);
                    SetRequestApproveCommand(false, null);
                    var obj2 = {
                        ModeFunction: C_FUNC_ID_APPROVE    
                
                    };
                    call_ajax_method_json("/Installation/ISS090_CheckScreenModePermission", obj2, function (result) {                     
                            if(result == C_FUNC_ID_APPROVE)
                            {                         
                                $("#strMode").val(C_FUNC_ID_APPROVE);
                                SetScreenMode();
                                InitialCommandButton();
                            }
                            else
                            {
                                $("#strMode").val(C_FUNC_ID_VIEW);
                                SetScreenMode();
                                InitialCommandButton();
                            }                          
                    });


                });

            });

        }

    });
}


function command_approve_click() {
    command_control.CommandControlMode(false);
    var obj = CreateObjectData($("#form1").serialize());
    call_ajax_method_json("/Installation/ISS090_ValidateBeforeApprove", obj, function (result, controls) {
        command_control.CommandControlMode(true);
        if (result == true) {

             // Get Message
            var obj = {
                module: "Installation",
                code: "MSG5082",
                param: [""]
            };
//            var obj = {
//                module: "Common",
//                code: "MSG0028",
//                param: ["approve"]
//            };

            call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                OpenYesNoMessageDialog(result.Code, result.Message, function () {

                    call_ajax_method_json("/Installation/ISS090_Approve", obj, function (result, controls) {
                        $("#strInstallationManagementStatus").val(C_INSTALL_MANAGE_STATUS_APPROVED);

                        SetScreenMode();

                        // Get Message
                        var obj = {
                            module: "Installation",
                            code: "MSG5091",
                
                        };
                        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                            OpenInformationMessageDialog(result.Code, result.Message, function () {
                            
                            });
                        });
                        SetApproveCommand(false, null);
                        SetReturnCommand(false, null);
//                         $("#strMode").val(C_FUNC_ID_COMPLETE);
                        
                        var obj2 = {
                            ModeFunction: C_FUNC_ID_COMPLETE    
                
                        };
                        call_ajax_method_json("/Installation/ISS090_CheckScreenModePermission", obj2, function (result) {                     
                                if(result == C_FUNC_ID_COMPLETE)
                                {                         
                                    $("#strMode").val(C_FUNC_ID_COMPLETE);
                                    SetScreenMode();
                                    InitialCommandButton();
                                }    
                                else
                                {
                                    $("#strMode").val(C_FUNC_ID_VIEW);
                                    SetScreenMode();
                                    InitialCommandButton();
                                }                   
                        });
                        ManualInitialGridManagementInfo(true);


                    });

                });

            });

        }

    });
}


function command_reject_click() {
    command_control.CommandControlMode(false);
    var obj = CreateObjectData($("#form1").serialize());

    call_ajax_method_json("/Installation/ISS090_ValidateBeforeReject", obj, function (result, controls) {
        command_control.CommandControlMode(true);
        if (result == true) {

            // Get Message
            var obj = {
                module: "Installation",
                code: "MSG5083",
                param: [""]
            };

            call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                OpenYesNoMessageDialog(result.Code, result.Message, function () {

                    call_ajax_method_json("/Installation/ISS090_Reject", obj, function (result, controls) {
                        $("#strInstallationManagementStatus").val(C_INSTALL_MANAGE_STATUS_REJECTED);
                        SetScreenMode();

                        // Get Message
                        var obj = {
                            module: "Installation",
                            code: "MSG5092",
                
                        };
                        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                            OpenInformationMessageDialog(result.Code, result.Message, function () {
                            
                            });
                        });
                        SetApproveCommand(false, null);
                        SetReturnCommand(false, null);

                        var obj2 = {
                        ModeFunction: C_FUNC_ID_OPERATE    
                
                        };
                        call_ajax_method_json("/Installation/ISS090_CheckScreenModePermission", obj2, function (result) {                     
                                if(result == C_FUNC_ID_OPERATE)
                                {                         
                                    $("#strMode").val(C_FUNC_ID_OPERATE);
                                    SetScreenMode();
                                    InitialCommandButton();
                                }          
                                else
                                {
                                    $("#strMode").val(C_FUNC_ID_VIEW);
                                    SetScreenMode();
                                    InitialCommandButton();
                                }                
                        });

                    });

                });

            });

        }

    });
}

function SetScreenForSubsection() {

}

function loadEmpName(MaintEmpNo, ctrlNo, ctrlName) {
    var parameter = { "MaintEmpNo": MaintEmpNo };
    call_ajax_method(
        '/Installation/ISS050_LoadEmployeeName/',
        parameter,
        function (result, controls) {
            if (controls != undefined) {
                //VaridateCtrl(["MaintEmpNo"], controls);
                ctrlName.val("");
                ctrlNo.focus();
                return;
            } else if (result != undefined) {
                ctrlName.val(result);
            }
            else {
                ctrlName.val("");
            }
        }
    );
}

function loadEmpNameBeforeUpdate(MaintEmpNo, ctrlNo, ctrlName) {
    var parameter = { "MaintEmpNo": MaintEmpNo };
    call_ajax_method(
    '/Installation/ISS050_LoadEmployeeName/',
    parameter,
    function (result, controls) {
        if (controls != undefined) {
            ctrlName.val("");
        } else if (result != undefined) {
            ctrlName.val(result);
        }
        else {
            ctrlName.val("");
        }
        UpdateInstallManagementSubsection();
    }
);
}

//    function checkExistEmployee(MaintEmpNo) {
//        var parameter = { "MaintEmpNo": MaintEmpNo };
//        call_ajax_method(
//        '/Installation/ISS050_LoadEmployeeName/',
//        parameter,
//        function (result, controls) {
//            if (result != undefined) {

//            }
//            else {
//                doAlert("Common", "MSG0095", [MaintEmpNo]);
//                return false;
//            }
//        }
//    );
//    }

function moneyConvert(value) {
    //Modify by Jutarat A. on 27032014
    /*var blnMinusValue = false; 
    if(value < 0)
    {
        value = String(value).substring(String(value).indexOf("-")+1);
        blnMinusValue = true;
    }
    if (value != null) {
        var buf = "";
        var sBuf = "";
        var j = 0;
        value = String(value);

        if (value.indexOf(".") > 0) {
            buf = value.substring(0, value.indexOf("."));
        } else {
            buf = value;
        }
        if (buf.length % 3 != 0 && (buf.length / 3 - 1) > 0) {
            sBuf = buf.substring(0, buf.length % 3) + ",";
            buf = buf.substring(buf.length % 3);
        }
        j = buf.length;
        for (var i = 0; i < (j / 3 - 1); i++) {
            sBuf = sBuf + buf.substring(0, 3) + ",";
            buf = buf.substring(3);
        }
        sBuf = sBuf + buf;
        if (value.indexOf(".") > 0) {
            value = sBuf + value.substring(value.indexOf("."));
        }
        else {
            if (sBuf != "") {
                value = sBuf + ".00";
            }
            else {
                value = "0.00";
            }

        }
        if(blnMinusValue)
            value = "-"+value
        return value;
    }*/
    if (value != null)
        value = SetNumericText(value, 2);

    return value;
    //End Modify
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

function convertDatetoYMD(ctrl) {
    var ctxt = ctrl.val();
    if (ctxt != "") {
        var instance = ctrl.data("datepicker");
        var ddate = instance.selectedDay;
        if (ddate < 10)
            ddate = "0" + ddate;
        var dmonth = instance.selectedMonth + 1;
        if (dmonth < 10)
            dmonth = "0" + dmonth;
        var dyear = instance.selectedYear;

        var txt = "" + dyear + dmonth + ddate;
        return txt;
    }
}


function UpdateManagementStatus(strManagementStatus) {
            
    $("#strInstallationManagementStatus").val(strManagementStatus);
    var obj = { ManagementStatus: strManagementStatus };
    call_ajax_method_json("/Installation/ISS090_UpdateManagementStatus", obj, function (result, controls) {
            
    });

}

function btnRemoveAttach_click(row_id) {

    var obj = {
    module: "Common",
    code: "MSG0142"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
			                        var _colID = ISS090_gridAttach.getColIndexById("AttachFileID");
                                    var _targID = ISS090_gridAttach.cells(row_id, _colID).getValue();

                                    var obj = {
                                        AttachID: _targID
                                    };
                                    call_ajax_method_json("/Installation/ISS090_RemoveAttach", obj, function (result, controls) {
                                        if (result != null) {
                                            RefreshAttachList();
                                                
                                        }
                                    });
                });
    });

        
}

//Add by Jutarat A. on 21032014
function InitLoadAttachList() {

    ISS090_gridAttach = $("#ISS090_gridAttachDocList").LoadDataToGridWithInitial(0, false, false, "/Installation/ISS090_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, false, null,
                        function () {
                                if (hasAlert) {
                                    hasAlert = false;
                                    OpenWarningDialog(alertMsg);
                                }
                                //CheckFirstRowIsEmpty(ISS090_gridAttach,true); 
                                $('#frmAttach').load(RefreshAttachList);   
                                            
                                isInitAttachGrid = true;   
                        });
}
//End Add

function RefreshAttachList() {

    //if (ISS090_gridAttach != null) {       
    if (ISS090_gridAttach != undefined && isInitAttachGrid) { //Modify by Jutarat A. on 21032014

        $('#ISS090_gridAttachDocList').LoadDataToGrid(ISS090_gridAttach, 0, false, "/Installation/ISS090_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, function () {
        if (hasAlert) {
            hasAlert = false;
            OpenWarningDialog(alertMsg);
        }
        //CheckFirstRowIsEmpty(ISS090_gridAttach,true); //Comment by Jutarat A. on 21032014
    }, null)
    }
    //ISS090_gridAttachBinding(); //Comment by Jutarat A. on 21032014
}

function ISS090_gridAttachBinding() {
    //if (isInitAttachGrid) {
    if (ISS090_gridAttach != undefined) { //Modify by Jutarat A. on 21032014
        var _colRemoveBtn = ISS090_gridAttach.getColIndexById("removeButton");

        for (var i = 0; i < ISS090_gridAttach.getRowsNum(); i++) {
            var row_id = ISS090_gridAttach.getRowId(i);
            GenerateRemoveButton(ISS090_gridAttach, "btnRemoveAttach", row_id, "removeButton", true);
            GenerateDownloadButton(ISS090_gridAttach, "btnDownloadAttach", row_id, "downloadButton", true);
            BindGridButtonClickEvent("btnRemoveAttach", row_id, btnRemoveAttach_click);
            BindGridButtonClickEvent("btnDownloadAttach", row_id, btnDownloadAttach_clicked);
        }
    }
    //Comment by Jutarat A. on 21032014
    //} else {
    //    isInitAttachGrid = true;
    //}
    //End Comment

    ISS090_gridAttach.setSizes();
}

function btnDownloadAttach_clicked(row_id) {
    var _colID = ISS090_gridAttach.getColIndexById("AttachFileID");
    var _targID = ISS090_gridAttach.cells(row_id, _colID).getValue();

    var obj = {
        AttachID: _targID
    };
    var key = ajax_method.GetKeyURL(null);
    var link = ajax_method.GenerateURL("/Installation/ISS090_DownloadAttach" + "?AttachID=" + _targID + "&k=" + key);
      
    window.open(link, "download");

}

function disabledGridColumn(grid,ButtonName,xmlColumnName,disableFlag) {
    var enableFlag = true;
    if (disableFlag) {
        enableFlag = false;
    }
    for (var i = 0; i < grid.getRowsNum(); i++) 
    {
        var row_id = grid.getRowId(i);
        EnableGridButton(grid, ButtonName, row_id, xmlColumnName, enableFlag);
        
    }
}

function ChangeReasonCode_Change() {
    if ($("#ChangeReasonCode").val() == C_INSTALL_BEFORE_CHANGE_REASON_OTHER) {
        $("#ChangeReasonOther").attr("readonly", false);
    }
    else {
        $("#ChangeReasonOther").attr("readonly", true);
        $("#ChangeReasonOther").val("");
    }
}

function ChangeRequestorCode_Change() {
    if ($("#ChangeRequestorCode").val() == C_INSTALL_BEFORE_CHANGE_REQUESTER_OTHER) {
        $("#ChangeRequestorOther").attr("readonly", false);
    }
    else {
        $("#ChangeRequestorOther").attr("readonly", true);
        $("#ChangeRequestorOther").val("");
    }
}

function doAlertWithMessage(msgCode, msgText) {
    hasAlert = true;
    alertMsg = msgText;
}

function AdvancePayment_Click(rowId)
{
//        var selectedRowIndex = ISS090_GridEmail.getRowIndex(rowId);
//        var AdvancePaymentChkId = GenerateGridControlID("AdvancePaymentChk", rowId);
//        var FlagAdvancePayment2 = false;

//        if($("#"+AdvancePaymentChkId).prop("checked") && ($("#"+AdvancePaymentChkId).prop("disabled") == false))
//        {
//            FlagAdvancePayment2 = true;
//        }

//        if(FlagAdvancePayment2)
//        {
//            ISS090_GridManagementInfo.cells2(i, ISS090_GridManagementInfo.getColIndexById('ButtonPaid')).setValue(GenerateHtmlButton("btnPaid", rowId, lblPaid, true));
//        }
    ManualInitialGridManagementInfo(false);
}

//Add by Jutarat A. on 10102013
function NoCheck_click()
{
    if ($("#chkNoCheck").prop("checked")) {
        $("#IEFirstCheckDate").val("");    
        $("#IEFirstCheckDate").datepicker("setDate", $("#IEFirstCheckDate").val());
        $("#IEFirstCheckDate").EnableDatePicker(false);

        //Add by Jutarat A. on 11102013
        $("#Adjustment").val("");
        $("#Adjustment").attr("disabled",true);
        $("#AdjustmentContents").val("");
        $("#AdjustmentContents").attr("disabled",true);
        $("#LastInspectionDate").val(""); 
        $("#LastInspectionDate").datepicker("setDate", $("#LastInspectionDate").val());
        $("#LastInspectionDate").EnableDatePicker(false);
        //End Add

        //Add by Jutarat A. on 07112013
        $("#InstallationStartDate").val("");    
        $("#InstallationStartDate").datepicker("setDate", $("#InstallationStartDate").val());
        $("#InstallationStartDate").EnableDatePicker(false);

        $("#InstallationCompleteDate").val("");    
        $("#InstallationCompleteDate").datepicker("setDate", $("#InstallationCompleteDate").val());
        $("#InstallationCompleteDate").EnableDatePicker(false);

        $("#ExpectInstallationStartDate").val("");    
        $("#ExpectInstallationStartDate").datepicker("setDate", $("#ExpectInstallationStartDate").val());
        $("#ExpectInstallationStartDate").EnableDatePicker(false);

        $("#ExpectInstallationCompleteDate").val("");    
        $("#ExpectInstallationCompleteDate").datepicker("setDate", $("#ExpectInstallationCompleteDate").val());
        $("#ExpectInstallationCompleteDate").EnableDatePicker(false);

        $("#ManPower").val("");
        $("#ManPower").SetDisabled(true);

        $("#Complain").val("");
        $("#Complain").attr("disabled",true);

        $("#IEEvaluation").val("");
        $("#IEEvaluation").attr("disabled",true);

        //$("#LastInstallationfee").SetDisabled(false); //Comment by Jutarat A. on 07112013

        $("#CheckChargeIEName").val("");
        $("#CheckChargeIECode").val("");
        $("#CheckChargeIECode").SetDisabled(true);
        //End Add
    }
    else {
        $("#IEFirstCheckDate").EnableDatePicker(true);

        //Add by Jutarat A. on 07112013
        $("#InstallationStartDate").EnableDatePicker(true);
        $("#InstallationCompleteDate").EnableDatePicker(true);
        $("#ExpectInstallationStartDate").EnableDatePicker(true);
        $("#ExpectInstallationCompleteDate").EnableDatePicker(true);
        $("#ManPower").SetDisabled(false);
        $("#Complain").attr("disabled",false);
        $("#IEEvaluation").attr("disabled",false);

        //changeIEEvaluation(); //Comment by Jutarat A. on 07112013
        $("#CheckChargeIECode").SetDisabled(false);
        //End Add
    }
}
//End Add