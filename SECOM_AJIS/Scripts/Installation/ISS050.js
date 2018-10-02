

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
var ISS050_GridEmail;
var ISS050_GridPOInfo;
var ISS050_GridAttachedDoc;
var TotalNormalPOAmount = 0;
var TotalActualPOAmount = 0;
var DivTotal = '<DIV style="TEXT-ALIGN: right; COLOR: blue">' + lblTotal + '</DIV>';

var ISS050_gridAttach;
var isInitAttachGrid = false;

var hasAlert = false;
var alertMsg = "";
// Main
$(document).ready(function () {
    var strContractProjectCode = $("#ContractCodeProjectCode").val();

    initialGridOnload();
    //$("#NewBldMgmtCost").BindNumericBox(10, 2, 0, 9999999999, 0);

    $("#btnRetrieveInstallation").click(retrieve_installation_click);
    $("#btnClearInstallation").click(clear_installation_click);

    // ** tt
    //$("#btnAdd").click(function () { BtnAddClick(); });

    $("#btnAdd").click(function () { BtnAddClick(); });
    $("#btnPOAdd").click(function () { BtnAddPOInfoClick(); });
    $("#btnPOClear").click(function () { BtnClearPOInfoClick(); });

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


    $("#POMemo").SetMaxLengthTextArea(4000);

    $("#NormalSubPOAmount").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#ActualPOAmount").BindNumericBox(12, 2, 0, 999999999999.99, 0);

    //    if ($("#ExpectInstallStartDate").length > 0) {
    //        InitialDateFromToControl("#ExpectInstallStartDate", "#ExpectInstallCompleteDate");
    //    }
    $("#ExpectInstallStartDate").InitialDate();
    $("#ExpectInstallCompleteDate").InitialDate();



    setInitialState();

    //================ GRID ATTACH ========================================    
    //$('#frmAttach').attr('src', 'ISS010_Upload');
    $('#frmAttach').attr('src', 'ISS050_Upload?k=' + _attach_k);

    //ISS050_gridAttach = $("#ISS050_gridAttachDocList").InitialGrid(10, false, "/Installation/ISS050_IntialGridAttachedDocList");
    InitLoadAttachList(); //Modify by Jutarat A. on 21032014

    SpecialGridControl(ISS050_gridAttach, ["removeButton"]);
    BindOnLoadedEvent(ISS050_gridAttach, ISS050_gridAttachBinding);
    //$('#frmAttach').load(RefreshAttachList); //Comment by Jutarat A. on 21032014
    //====================================================================

    if (strContractProjectCode != "") {

        $("#ContractCodeProjectCode").val(strContractProjectCode)
        $("#btnRetrieveInstallation").attr("disabled", true);
        setTimeout("retrieve_installation_click()", 2000);
    }

});


function loadEmpName(MaintEmpNo, ctrlNo, ctrlName) {
    var parameter = { "MaintEmpNo": MaintEmpNo };
    call_ajax_method(
        '/Installation/ISS050_LoadEmployeeName/',
        parameter,
        function (result, controls) {
            if (result == null) {
                //VaridateCtrl(["MaintEmpNo"], controls);
                ctrlName.val("");
                //ctrlNo.focus();
                return;
            } else if (result != undefined) {
                ctrlName.val(result);
            }
        }
    );
}

function initialGridOnload() {
    // intial grid
    ISS050_GridEmail = $("#gridEmail").InitialGrid(pageRow, false, "/Installation/ISS050_InitialGridEmail");
    ISS050_GridPOInfo = $("#gridPOInfo").InitialGrid(pageRow, false, "/Installation/ISS050_InitialGridPOInfo");

    /* ===== binding event when finish load data ===== */
    BindOnLoadedEvent(ISS050_GridEmail, ISS050_gridEmailBinding);
    /* ===== binding event when finish load data ===== */
    BindOnLoadedEvent(ISS050_GridPOInfo, ISS050_gridPOBinding);
    //===============================================================

    SpecialGridControl(ISS050_GridPOInfo, ["ButtonPOIssue"]);
    SpecialGridControl(ISS050_GridPOInfo, ["ButtonPOEdit"]);
    SpecialGridControl(ISS050_GridPOInfo, ["ButtonPORemove"]);
}

function setInitialState() {
    // --------------- Initial SCREEN ------------------
    $("#divContractBasicInfo").SetViewMode(false);
    $("#divProjectInfo").SetViewMode(false);
    $("#divInstallationInfo").SetViewMode(false);
    $("#divInstallationPOInfo").SetViewMode(false);
    $("#divInstallationMANo").SetViewMode(false);



    $("#ContractCodeProjectCode").attr("readonly", false);
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
    $("#IEStaffEmpNo1").attr("readonly", true);
    $("#IEStaffEmpNo2").attr("readonly", true);
    $("#SubcontractorCode").attr("disabled", true);
    $("#SubConstractorGroupName").attr("readonly", true);
    $("#NormalSubPOAmount").attr("readonly", true);
    $("#ActualPOAmount").attr("readonly", true);
    $("#PONo").attr("readonly", true);
    $("#ExpectInstallStartDate").EnableDatePicker(false)
    $("#ExpectInstallCompleteDate").EnableDatePicker(false)
    $("#btnPOAdd").attr("disabled", true);
    $("#btnPOClear").attr("disabled", true);

    $("#EmailAddress").attr("readonly", true);
    $("#btnAdd").attr("disabled", true);
    $("#btnClear").attr("disabled", true);
    $("#btnSearchEmail").attr("disabled", true);

    $("#POMemo").attr("readonly", true);
    $("#btnSearchEmail").attr("disabled", true);
    $("#btnSearchEmail").attr("disabled", true);
    $("#uploadFrame1").attr("disabled", true);



    //####################################################

    InitialCommandButton(0);
    $("#divInputContractCode").clearForm();
    $("#divContractBasicInfo").clearForm();
    $("#divProjectInfo").clearForm();
    $("#divInstallationInfo").clearForm();
    $("#divInstallationPOInfo").clearForm();
    $("#divInstallationMANo").clearForm();

    $("#divInputContractCode").show();
    $("#divContractBasicInfo").show();
    $("#divProjectInfo").show();
    $("#divInstallationInfo").show();
    $("#divInstallationMANo").show();
    //--------------------------------------------------  
    $("#divAttachFrame").hide();
    $("#divAttachRemark").hide();
    //enabledGridEmail();
    //enabledGridPOInfo();
}

function retrieve_installation_click() {
    $("#btnRetrieveInstallation").attr("disabled", true);
    //InitialCommandButton(1);
    command_control.CommandControlMode(false);
    var tmpCode = $("#ContractCodeProjectCode").val();
    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
    call_ajax_method_json("/Installation/ISS050_RetrieveData", obj,
        function (result, controls) {
            command_control.CommandControlMode(true);
            if (controls != undefined) {
                $("#btnRetrieveInstallation").attr("disabled", false);
                /* --- Higlight Text --- */
                /* --------------------- */
                //VaridateCtrl(["txtSpecifyContractCode"], controls);                      
                //$("#divStartResumeOperation").clearForm();
                //SetInitialState();    
                //$("#divContractBasicInfo").hide();
                //$("#divProjectInfo").hide();
                //$("#divInstallationInfo").hide();
                //$("#divInstallationPOInfo").hide();
                $("#divContractBasicInfo").clearForm();
                $("#divProjectInfo").clearForm();
                $("#divInstallationInfo").clearForm();
                $("#divInstallationPOInfo").clearForm();
                $("#divInstallationMANo").clearForm();
                VaridateCtrl(["ContractCodeProjectCode"], ["ContractCodeProjectCode"]);
                return;
            }
            else if (result != undefined) {

                if (result.ServiceTypeCode == C_SERVICE_TYPE_RENTAL || result.ServiceTypeCode == C_SERVICE_TYPE_SALE) {
                    SEARCH_CONDITION = {
                        ContractCode: result.ContractProjectCodeForShow
                    };
                }
                else {
                    SEARCH_CONDITION = {
                        ProjectCode: result.ContractProjectCodeForShow
                    };
                }
                var obj = { strFieldName: "" };

                $("#divContractBasicInfo").clearForm();
                $("#divProjectInfo").clearForm();
                $("#divInstallationInfo").clearForm();
                $("#divInstallationPOInfo").clearForm();
                $("#divInstallationMANo").clearForm();

                $("#ServiceTypeCode").val(result.ServiceTypeCode);

                if (result.dtInstallationManagement != undefined) {
                    if (result.dtInstallationManagement.ProposeInstallStartDate != null)
                        result.dtInstallationManagement.ProposeInstallStartDate = ConvertDateToTextFormat(result.dtInstallationManagement.ProposeInstallStartDate.replace('/Date(', '').replace(')/', '') * 1);
                    if (result.dtInstallationManagement.ProposeInstallCompleteDate != null)
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
                $("#divInstallationInfo").bindJSON(result.dtInstallationBasic);
                $("#divInstallationInfo").bindJSON(result.dtInstallationManagement);

                //====================== Get Name From Code ====================
                if (result.dtInstallationBasic != null) {
                    if (result.dtInstallationBasic.InstallationTypeRentalName != null) {
                        $("#InstallationType").val(result.dtInstallationBasic.InstallationTypeRentalName);
                    }
                    else if (result.dtInstallationBasic.InstallationTypeSaleName != null) {
                        $("#InstallationType").val(result.dtInstallationBasic.InstallationTypeSaleName);
                    }
                }
                //==============================================================
                $("#ContractCodeProjectCode").val(result.ContractProjectCodeForShow);
                if (result.dtRentalContractBasic != null) {
                    result.dtRentalContractBasic.ContractCode = $("#ContractCodeProjectCode").val();

                    $("#OperationOfficeCode").val(result.dtRentalContractBasic.OperationOfficeCode)
                    $("#divContractBasicInfo").bindJSON(result.dtRentalContractBasic);
                    $("#divInstallationInfo").bindJSON(result.dtRentalContractBasic);
                    // $("#divInstallationInfo").bindJSON(result.dtInstallationBasic);
                    //$("#divInstallationInfo").bindJSON(result.dtInstallationManagement);

                    SetRegisterState(1);
                }
                else if (result.dtSale != null) {
                    result.dtSale.ContractCode = $("#ContractCodeProjectCode").val();

                    $("#OperationOfficeCode").val(result.dtSale.OperationOfficeCode);
                    $("#divContractBasicInfo").bindJSON(result.dtSale);
                    $("#divInstallationInfo").bindJSON(result.dtSale);
                    // $("#divInstallationInfo").bindJSON(result.dtInstallationBasic);
                    //$("#divInstallationInfo").bindJSON(result.dtInstallationManagement);
                    SetRegisterState(1);
                }
                else if (result.dtProject != null) {

                    $("#divProjectInfo").bindJSON(result.dtProject);
                    //$("#ProductTypeCode").val(result.RentalContractBasicData.ProductTypeCode);
                    //$("#divInstallationInfo").bindJSON(result.dtInstallationBasic);
                    //$("#divInstallationInfo").bindJSON(result.dtInstallationManagement);
                    SetRegisterState(2);
                }

                /////////////// BIND MEMO ////////////////////////

                //////////////////////////////////////////////////

                /////////////// BIND EMAIl DATA //////////////////
                if (result.ListDOEmail != null) {
                    if (result.ListDOEmail.length > 0) {
                        for (var i = 0; i < result.ListDOEmail.length; i++) {
                            var emailList = [result.ListDOEmail[i].EmailAddress, "", "", result.ListDOEmail[i].EmpNo];

                            CheckFirstRowIsEmpty(ISS050_GridEmail, true);
                            AddNewRow(ISS050_GridEmail, emailList);
                        }
                        ISS050_gridEmailBinding();
                    }
                }

                //////////////////////////////////////////////////

                /////////////// BIND PO DATA //////////////////
                if (result.ListPOInfo != null) {
                    if (result.ListPOInfo.length > 0) {
                        var lastRow = 0;
                        var TotalNormalPOAmount = 0;
                        var TotalActualPOAmount = 0;
                        for (var i = 0; i < result.ListPOInfo.length; i++) {
                            var strExpectInstallStartDate = "";
                            var strExpectInstallCompleteDate = "";
                            if (result.ListPOInfo[i].ExpectInstallStartDate != null)
                                strExpectInstallStartDate = ConvertDateToTextFormat(result.ListPOInfo[i].ExpectInstallStartDate.replace('/Date(', '').replace(')/', '') * 1);
                            if (result.ListPOInfo[i].ExpectInstallCompleteDate != null)
                                strExpectInstallCompleteDate = ConvertDateToTextFormat(result.ListPOInfo[i].ExpectInstallCompleteDate.replace('/Date(', '').replace(')/', '') * 1);
                            var POList = [result.arrayPOName[i], result.ListPOInfo[i].SubcontractorGroupName, result.ListPOInfo[i].TxtCurrencySubPOAmount + " " + moneyConvert(result.ListPOInfo[i].NormalSubPOAmount), result.ListPOInfo[i].TxtCurrencyActualPOAmount + " " + moneyConvert(result.ListPOInfo[i].ActualPOAmount), result.ListPOInfo[i].PONo, strExpectInstallStartDate, strExpectInstallCompleteDate, result.ListPOInfo[i].SubcontractorCode, "", "", "", "", "true"];

                            CheckFirstRowIsEmpty(ISS050_GridPOInfo, true);
                            AddNewRow(ISS050_GridPOInfo, POList);
                            TotalNormalPOAmount = TotalNormalPOAmount + result.ListPOInfo[i].NormalSubPOAmount;
                            TotalActualPOAmount = TotalActualPOAmount + result.ListPOInfo[i].ActualPOAmount;
                            lastRow = i;

                        }

                    }
                    ISS050_gridPOBinding();
                    AddNewRow(ISS050_GridPOInfo, ["", "", result.ListPOInfo[0].TxtCurrencySubPOAmount + " " + moneyConvert(TotalNormalPOAmount), result.ListPOInfo[0].TxtCurrencyActualPOAmount + " " + moneyConvert(TotalActualPOAmount), "", "", "", "", "", "", ""]);
                    rowID = ISS050_GridPOInfo.getRowId(lastRow + 1);
                    ISS050_GridPOInfo.setColspan(rowID, 0, 2);
                    //ISS050_GridPOInfo.setColspan(rowID, 4, 6);
                    ISS050_GridPOInfo.cells2(lastRow + 1, 1).setValue(DivTotal);

                }
                //////////////////////////////////////////////////
                if (result.dtInstallationBasic != undefined) {
                    $('#frmAttach').attr('src', 'ISS050_SendAttachKey?sK=' + result.dtInstallationBasic.MaintenanceNo);
                    $('#frmAttach').load(RefreshAttachList);
                }
            }
            else {

                setInitialState();
                $("#ContractCodeProjectCode").val(tmpCode);
                VaridateCtrl(["ContractCodeProjectCode"], ["ContractCodeProjectCode"]);
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
    /* --- Set condition --- */
    SEARCH_CONDITION = {
        ContractCode: "",
        ProjectCode: ""
    };
    /* --------------------- */
    command_back_click();
    setInitialState();
    //=== TRS 30/05/2012 Comment for fix error call ajax manytimes ====
    //btnClearEmailClick();
    //ClearPOInfo();
    //=================================================================
    if (ISS050_gridAttach.getRowsNum() > 0)
        DeleteAllRow(ISS050_gridAttach);
    var obj = null;
    call_ajax_method_json("/Installation/ISS050_ClearCommonContractCode", obj, function (result, controls) {
        //================ TRS 30/05/2012 =============
        DeleteAllRow(ISS050_GridEmail);
        var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
        call_ajax_method_json("/Installation/ISS050_ClearInstallEmail", obj, function (result, controls) {

            ClearPOInfo();
        });
        //==============================================
    });
}



function BtnAddClick() {

    // Is exist email
    // Fill to grid
    // Keep selected email to sesstion

    var strEmail = $("#EmailAddress").val();
    if (strEmail.replace(/ /g, "") == "") {
        doAlert("Common", "MSG0007", lblInstallationEmail);
        VaridateCtrl(["EmailAddress"], ["EmailAddress"]);
    }
    else {
        var email = { "strEmail": $("#EmailAddress").val() + strEmailSuffix };

        call_ajax_method_json("/Installation/ISS050_GetInstallEmail", email, function (result, controls, isWarning) {

            if (isWarning == undefined) { // No error and data(email) is exist

                if (result.length > 0) {
                    // Fill to grid
                    var emailList = [result[0].EmailAddress, "", result[0].EmpNo];

                    CheckFirstRowIsEmpty(ISS050_GridEmail, true);
                    AddNewRow(ISS050_GridEmail, emailList);
                    ISS050_gridEmailBinding();
                    $("#EmailAddress").val("");
                }

            }
            else {
                VaridateCtrl(["EmailAddress"], ["EmailAddress"]);
            }

        });
    }

}

function BtnAddPOInfoClick() {

    if ($("#EditingSubcontractorCode").val() != "") {
        UpdatePOInfo();

    }
    else {
        AddPOInfo();
    }

    command_control.CommandControlMode(true);
    ISS050_gridPOBinding();

}

function AddPOInfo() {

    if (convertDatetoYMD($("#ExpectInstallStartDate")) > convertDatetoYMD($("#ExpectInstallCompleteDate"))) {
        doAlert("Installation", "MSG5009", "");
        VaridateCtrl(["ExpectInstallStartDate", "ExpectInstallCompleteDate"], ["ExpectInstallStartDate", "ExpectInstallCompleteDate"]);
        return false;
    }

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

    obj.NormalSubPOAmountCurrencyType = $("#NormalSubPOAmount").NumericCurrencyValue();
    if (obj.NormalSubPOAmountCurrencyType == C_CURRENCY_LOCAL) {
        obj.NormalSubPOAmount = obj.NormalSubPOAmount;
        obj.NormalSubPOAmountUsd = null;
    }
    else {
        obj.NormalSubPOAmountUsd = obj.NormalSubPOAmount;
        obj.NormalSubPOAmount = null;
    }

    obj.ActualPOAmountCurrencyType = $("#ActualPOAmount").NumericCurrencyValue();
    if (obj.ActualPOAmountCurrencyType == C_CURRENCY_LOCAL) {
        obj.ActualPOAmount = obj.ActualPOAmount;
        obj.ActualPOAmountUsd = null;
    }
    else {
        obj.ActualPOAmountUsd = obj.ActualPOAmount;
        obj.ActualPOAmount = null;
    }

    call_ajax_method_json("/Installation/ISS050_AddPOInfo", obj, function (result, controls, isWarning) {

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
            var TxtCurrency = "Rp. ";
            var strExpectInstallStartDate = "";
            var strExpectInstallCompleteDate = "";
            if (result.ExpectInstallStartDate != null)
                strExpectInstallStartDate = ConvertDateToTextFormat(result.ExpectInstallStartDate.replace('/Date(', '').replace(')/', '') * 1);
            if (result.ExpectInstallCompleteDate != null)
                strExpectInstallCompleteDate = ConvertDateToTextFormat(result.ExpectInstallCompleteDate.replace('/Date(', '').replace(')/', '') * 1);
            var POList = [$("#SubcontractorCode option:selected").text(), result.SubcontractorGroupName, TxtCurrency + $("#NormalSubPOAmount").val(), TxtCurrency + $("#ActualPOAmount").val(), result.PONo, strExpectInstallStartDate, strExpectInstallCompleteDate, result.SubcontractorCode, "false"];

            CheckFirstRowIsEmpty(ISS050_GridPOInfo, true);
            AddNewRow(ISS050_GridPOInfo, POList);
            //======== Add Total ROW ===========================
            rowID = ISS050_GridPOInfo.getRowId(ISS050_GridPOInfo.getRowsNum() - 2);
            DeleteRow(ISS050_GridPOInfo, rowID);
            ISS050_gridPOBinding();
            AddNewRow(ISS050_GridPOInfo, ["", "", TxtCurrency + moneyConvert(TotalNormalPOAmount), TxtCurrency + moneyConvert(TotalActualPOAmount), "", "", "", "", "", "", ""]);
            rowID = ISS050_GridPOInfo.getRowId(ISS050_GridPOInfo.getRowsNum() - 1);
            ISS050_GridPOInfo.setColspan(rowID, 0, 2);
            //ISS050_GridPOInfo.setColspan(rowID, 4, 6);
            //ISS050_GridPOInfo.setColspan(rowID, ISS050_GridPOInfo.getColumnsNum()-2, 2);
            ISS050_GridPOInfo.cells2(ISS050_GridPOInfo.getRowsNum() - 1, 1).setValue("<div style='text-align:right;color:blue'>" + lblTotal + "</div>");
            ISS050_gridPOBinding();

            BtnClearPOInfoClick();
        }

    });


}

function UpdatePOInfo() {

    if (convertDatetoYMD($("#ExpectInstallStartDate")) > convertDatetoYMD($("#ExpectInstallCompleteDate"))) {
        doAlert("Installation", "MSG5009", "");
        VaridateCtrl(["ExpectInstallStartDate", "ExpectInstallCompleteDate"], ["ExpectInstallStartDate", "ExpectInstallCompleteDate"]);
        return false;
    }
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

    obj.NormalSubPOAmountCurrencyType = $("#NormalSubPOAmount").NumericCurrencyValue();
    if (obj.NormalSubPOAmountCurrencyType == C_CURRENCY_LOCAL) {
        obj.NormalSubPOAmount = obj.NormalSubPOAmount;
        obj.NormalSubPOAmountUsd = null;
    }
    else {
        obj.NormalSubPOAmountUsd = obj.NormalSubPOAmount;
        obj.NormalSubPOAmount = null;
    }

    obj.ActualPOAmountCurrencyType = $("#ActualPOAmount").NumericCurrencyValue();
    if (obj.ActualPOAmountCurrencyType == C_CURRENCY_LOCAL) {
        obj.ActualPOAmount = obj.ActualPOAmount;
        obj.ActualPOAmountUsd = null;
    }
    else {
        obj.ActualPOAmountUsd = obj.ActualPOAmount;
        obj.ActualPOAmount = null;
    }

    call_ajax_method_json("/Installation/ISS050_UpdatePOInfo", obj, function (result, controls, isWarning) {
        var TxtCurrency = "Rp. ";
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

            //CheckFirstRowIsEmpty(ISS050_GridPOInfo, true);
            //AddNewRow(ISS050_GridPOInfo, POList);            
            var selectedRowIndex = $("#EditingRowIndex").val();
            ISS050_GridPOInfo.cells2(selectedRowIndex, ISS050_GridPOInfo.getColIndexById('SubConCode')).setValue($("#SubcontractorCode option:selected").val())
            ISS050_GridPOInfo.cells2(selectedRowIndex, ISS050_GridPOInfo.getColIndexById('SubConGroupName')).setValue(result.SubcontractorGroupName);

            ISS050_GridPOInfo.cells2(selectedRowIndex, ISS050_GridPOInfo.getColIndexById('SubConPOAmount')).setValue(TxtCurrency + $("#NormalSubPOAmount").val());
            ISS050_GridPOInfo.cells2(selectedRowIndex, ISS050_GridPOInfo.getColIndexById('ActualPOAmount')).setValue(TxtCurrency + $("#ActualPOAmount").val());

            ISS050_GridPOInfo.cells2(selectedRowIndex, ISS050_GridPOInfo.getColIndexById('PONo')).setValue(result.PONo);
            ISS050_GridPOInfo.cells2(selectedRowIndex, ISS050_GridPOInfo.getColIndexById('InstallStartDate')).setValue($("#ExpectInstallStartDate").val());
            ISS050_GridPOInfo.cells2(selectedRowIndex, ISS050_GridPOInfo.getColIndexById('InstallCompleteDate')).setValue($("#ExpectInstallCompleteDate").val());
            //            $("#EditingRowIndex").val("");
            //            $("#EditingSubcontractorCode").val("");
            //            $("#btnPOAdd").text(lblAdd);
            //            $("#SubcontractorCode").attr("disabled", false);

            var TmpNormalPOAmount = 0;
            var TmpActualPOAmount = 0;
            TotalNormalPOAmount = 0;
            TotalActualPOAmount = 0;
            for (var i = 0; i < ISS050_GridPOInfo.getRowsNum()-1; i++) {
                var rowId = ISS050_GridPOInfo.getRowId(i);

                //if (ISS050_GridPOInfo.cells2(i, ISS050_GridPOInfo.getColIndexById('SubConName')).getValue() != ""
                //    && ISS050_GridPOInfo.cells2(i, ISS050_GridPOInfo.getColIndexById('SubConName')).getValue() != DivTotal) {

                    TmpNormalPOAmount = ((ISS050_GridPOInfo.cells2(i, ISS050_GridPOInfo.getColIndexById('SubConPOAmount')).getValue()).replace(/,/g, '').split(' '));
                    TmpActualPOAmount = ((ISS050_GridPOInfo.cells2(i, ISS050_GridPOInfo.getColIndexById('ActualPOAmount')).getValue()).replace(/,/g, '').split(' '));

                    TotalNormalPOAmount = TotalNormalPOAmount + TmpNormalPOAmount[1] * 1;
                    TotalActualPOAmount = TotalActualPOAmount + TmpActualPOAmount[1] * 1;


                //}
            }

            ISS050_GridPOInfo.cells2(ISS050_GridPOInfo.getRowsNum() - 1, 2).setValue(TxtCurrency + moneyConvert(TotalNormalPOAmount));
            ISS050_GridPOInfo.cells2(ISS050_GridPOInfo.getRowsNum() - 1, 3).setValue(TxtCurrency + moneyConvert(TotalActualPOAmount));

            BtnClearPOInfoClick();
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

    $("#EditingRowIndex").val("");
    $("#EditingSubcontractorCode").val("");
    $("#btnPOAdd").text(lblAdd);
    $("#SubcontractorCode").attr("disabled", false);
    VaridateCtrl(["SubcontractorCode", "NormalSubPOAmount", "ActualPOAmount", "ExpectInstallStartDate", "ExpectInstallCompleteDate"], null);


    command_control.CommandControlMode(true);
    ISS050_gridPOBinding();
}

function BtnRemoveEmailClick(row_id) {

    var obj = {
        module: "Common",
        code: "MSG0141"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    var selectedRowIndex = ISS050_GridEmail.getRowIndex(row_id);
                    var mail = ISS050_GridEmail.cells2(selectedRowIndex, ISS050_GridEmail.getColIndexById('EmailAddress')).getValue();
                    var obj = { EmailAddress: mail }
                    DeleteRow(ISS050_GridEmail, row_id);
                    call_ajax_method_json("/Installation/ISS050_RemoveMailClick", obj, function (result, controls, isWarning) {

                    });
                });
    });


}


function BtnRemovePOInfoClick(row_id) {
    var TxtCurrency = "Rp. ";
    var obj = {
        module: "Common",
        code: "MSG0141"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    var selectedRowIndex = ISS050_GridPOInfo.getRowIndex(row_id);
                    var strPONo = ISS050_GridPOInfo.cells2(selectedRowIndex, ISS050_GridPOInfo.getColIndexById('PONo')).getValue();
                    var obj = { PONo: strPONo }
                    TotalNormalPOAmount = 0;
                    TotalActualPOAmount = 0;
                    DeleteRow(ISS050_GridPOInfo, row_id);

                    call_ajax_method_json("/Installation/ISS050_RemovePOInfoClick", obj, function (result, controls, isWarning) {

                    });


                    ISS050_gridPOBinding();
                    //=====================================================

                    ISS050_GridPOInfo.cells2(ISS050_GridPOInfo.getRowsNum() - 1, 2).setValue(TxtCurrency + moneyConvert(TotalNormalPOAmount));
                    ISS050_GridPOInfo.cells2(ISS050_GridPOInfo.getRowsNum() - 1, 3).setValue(TxtCurrency + moneyConvert(TotalActualPOAmount));

                    if (TotalNormalPOAmount == 0 && TotalActualPOAmount == 0) {
                        DeleteAllRow(ISS050_GridPOInfo);
                    }
                });
    });

}

function BtnEditPOInfoClick(row_id) {


    var selectedRowIndex = ISS050_GridPOInfo.getRowIndex(row_id);
    $("#EditingSubcontractorCode").val(ISS050_GridPOInfo.cells2(selectedRowIndex, ISS050_GridPOInfo.getColIndexById('SubConCode')).getValue());
    $("#EditingRowIndex").val(ISS050_GridPOInfo.getRowIndex(row_id));
    $("#SubcontractorCode").val(ISS050_GridPOInfo.cells2(selectedRowIndex, ISS050_GridPOInfo.getColIndexById('SubConCode')).getValue());
    $("#SubConstractorGroupName").val(ISS050_GridPOInfo.cells2(selectedRowIndex, ISS050_GridPOInfo.getColIndexById('SubConGroupName')).getValue());

    var TmpNormalPOAmount = 0;
    var TmpActualPOAmount = 0;
    TmpNormalPOAmount = (ISS050_GridPOInfo.cells2(selectedRowIndex, ISS050_GridPOInfo.getColIndexById('SubConPOAmount')).getValue()).replace(/,/g, '').split(' ');
    TmpActualPOAmount = (ISS050_GridPOInfo.cells2(selectedRowIndex, ISS050_GridPOInfo.getColIndexById('ActualPOAmount')).getValue()).replace(/,/g, '').split(' ');
    $("#NormalSubPOAmount").val(moneyConvert(TmpNormalPOAmount[1].replace(/,/g, "") * 1));
    $("#ActualPOAmount").val(moneyConvert(TmpActualPOAmount[1].replace(/,/g, "") * 1));

    $("#PONo").val(ISS050_GridPOInfo.cells2(selectedRowIndex, ISS050_GridPOInfo.getColIndexById('PONo')).getValue());


    $("#ExpectInstallStartDate").val(ISS050_GridPOInfo.cells2(selectedRowIndex, ISS050_GridPOInfo.getColIndexById('InstallStartDate')).getValue());
    $("#ExpectInstallStartDate").datepicker("setDate", $("#ExpectInstallStartDate").val());

    $("#ExpectInstallCompleteDate").val(ISS050_GridPOInfo.cells2(selectedRowIndex, ISS050_GridPOInfo.getColIndexById('InstallCompleteDate')).getValue());
    $("#ExpectInstallCompleteDate").datepicker("setDate", $("#ExpectInstallCompleteDate").val());

    $("#SubcontractorCode").attr("disabled", true);
    $("#btnPOAdd").text(lblUpdate);

    command_control.CommandControlMode(false);
    disabledGridColumn(ISS050_GridPOInfo, "btnIssuePOInfo", "ButtonPOIssue", true);
    disabledGridColumn(ISS050_GridPOInfo, "btnRemovePOInfo", "ButtonPORemove", true);
    disabledGridColumn(ISS050_GridPOInfo, "btnEditPOInfo", "ButtonPOEdit", true);
}

function BtnIssuePOInfoClick(row_id) {

    var selectedRowIndex = ISS050_GridPOInfo.getRowIndex(row_id);
    var strSubContractorCode = ISS050_GridPOInfo.cells2(selectedRowIndex, ISS050_GridPOInfo.getColIndexById('SubConCode')).getValue();
    //window.open("ISS050_CreateReportInstallationPOandSubPrice?strSubContractorCode=" + strSubContractorCode);

    var key = ajax_method.GetKeyURL(null);
        var link = ajax_method.GenerateURL("/Installation/ISS050_CreateReportInstallationPOandSubPrice" + "?strSubContracttorCode=" + strSubContractorCode + "&k=" + key);
        window.open(link, "download1");
    //    link = ajax_method.GenerateURL("/Installation/ISS050_CreateReportInstallationSpecCompleteData" + "?strSubContracttorCode=" + strSubContractorCode + "&k=" + key);
    //    window.open(link, "download2");
    //    link = ajax_method.GenerateURL("/Installation/ISS050_CreateReportIECheckSheet" + "?strSubContracttorCode=" + strSubContractorCode + "&k=" + key);
    //    window.open(link, "download3");

    //var link = ajax_method.GenerateURL("/Installation/ISS050_CreateReportMergedAll" + "?strSubContracttorCode=" + strSubContractorCode + "&k=" + key);
    //window.open(link, "download1");

    var obj = { SubcontractorCode: strSubContractorCode }
    call_ajax_method_json("/Installation/ISS050_IssueButtonClick", obj, function (result, controls) {

    });
}



function BtnRegisterClick() {
    var registerData_obj = {};
}


function BtnClearClick() {
    $("#EmailAddress").val("");
    VaridateCtrl(["EmailAddress"], null);
}

function IsValidEmail(email) {
    //var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    //return emailReg.test(email);
    return true;
}

function InitialCommandButton(step) {
    if (step == 0) {
        SetRegisterCommand(false, null);
        SetResetCommand(false, null);
        SetApproveCommand(false, null);
        SetRejectCommand(false, null);
        SetReturnCommand(false, null);
        SetCloseCommand(false, null);
        SetConfirmCommand(false, null);
        SetBackCommand(false, null);
    }
    else if (step == 1) {
        SetRegisterCommand(true, command_register_click);
        SetResetCommand(true, command_reset_click);
        SetApproveCommand(false, null);
        SetRejectCommand(false, null);
        SetReturnCommand(false, null);
        SetCloseCommand(false, null);
        SetConfirmCommand(false, null);
        SetBackCommand(false, null);
    }
    else if (step == 2) {
        SetRegisterCommand(false, null);
        SetResetCommand(false, null);
        SetApproveCommand(false, null);
        SetRejectCommand(false, null);
        SetReturnCommand(false, null);
        SetCloseCommand(false, null);
        SetConfirmCommand(true, command_confirm_click);
        SetBackCommand(true, command_back_click);
    }
    else if (step == 3) {
        SetRegisterCommand(false, null);
        SetResetCommand(false, null);
        SetApproveCommand(false, null);
        SetRejectCommand(false, null);
        SetReturnCommand(false, null);
        SetCloseCommand(false, null);
        SetConfirmCommand(false, null);
        SetBackCommand(false, null);
    }
    else if (step == 4) {
        SetRegisterCommand(false, null);
        SetResetCommand(false, null);
        SetApproveCommand(true, command_approve_click);
        SetRejectCommand(true, command_reject_click);
        SetReturnCommand(true, command_return_click);
        SetCloseCommand(true, command_close_click);
        SetConfirmCommand(false, null);
        SetBackCommand(false, null);
    }
}

function command_back_click() {
    InitialCommandButton(1);
    $("#divContractBasicInfo").SetViewMode(false);
    $("#divProjectInfo").SetViewMode(false);
    $("#divInstallationInfo").SetViewMode(false);
    $("#divInstallationPOInfo").SetViewMode(false);
    $("#divInstallationMANo").SetViewMode(false);
    $("#divAttachFrame").show();
    $("#divAttachRemark").show();
    enabledGridEmail();
    enabledGridPOInfo();
    $("#divInputContractCode").show();
    $("#divAddUpdatePOInfo").show();
    $("#divAddEmail").show();
}

function command_register_click() {
    command_control.CommandControlMode(false);
    //var obj = CreateObjectData($("#form1").serialize() + "&RequestMemo=" + $("#RequestMemo").val());
    var obj = { strContractCode: $("#ContractCodeProjectCode").val()
                , IEStaffEmpNo1: $("#IEStaffEmpNo1").val()
                , IEStaffEmpNo2: $("#IEStaffEmpNo2").val()
    };
    call_ajax_method_json("/Installation/ISS050_ValidateBeforeRegister", obj, function (result, controls) {
        command_control.CommandControlMode(true);
        if (result == true) {
            setConfirmState();
        }
        else {
            VaridateCtrl(["NewPhoneLineOpenDate", "IEStaffEmpNo1", "IEStaffEmpNo2"], controls);
        }


    });
}

function setConfirmState() {
    InitialCommandButton(2);

    $("#divContractBasicInfo").SetViewMode(true);
    $("#divProjectInfo").SetViewMode(true);
    $("#divInstallationInfo").SetViewMode(true);
    $("#divInstallationPOInfo").SetViewMode(true);
    $("#divInstallationPOInfo").SetViewMode(true);
    $("#divAttachFrame").hide();
    $("#divAttachRemark").hide();
    disabledGridEmail();
    disabledGridPOInfo();

    $("#divInputContractCode").hide();
    $("#divAddUpdatePOInfo").hide();
    $("#divAddEmail").hide();
}

function disabledGridEmail() {
    //////// DISABLED BUTTON In EMAIL GRID ///////////
    //    for (var i = 0; i < ISS050_GridEmail.getRowsNum(); i++) {
    //        var row_id = ISS050_GridEmail.getRowId(i);
    //EnableGridButton(ISS050_GridEmail, "btnRemoveEmail", row_id, "ButtonRemove", false);
    colInx = ISS050_GridEmail.getColIndexById("ButtonRemove")
    ISS050_GridEmail.setColumnHidden(colInx, true);
    //    }
    //////////////////////////////////////////////////

    colInx = ISS050_gridAttach.getColIndexById("removeButton")
    ISS050_gridAttach.setColumnHidden(colInx, true);
}

function enabledGridEmail() {
    //////// ENABLED BUTTON In EMAIL GRID ///////////
    colInx = ISS050_GridEmail.getColIndexById("ButtonRemove")
    ISS050_GridEmail.setColumnHidden(colInx, false);
    SetFitColumnForBackAction(ISS050_GridEmail, "TempColumn");
    //    for (var i = 0; i < ISS050_GridEmail.getRowsNum(); i++) {
    //        row_id = ISS050_GridEmail.getRowId(i);
    //        Colinx = ISS050_GridEmail.getColIndexById("TempColumn");
    //        if (Colinx != undefined) {
    //            ISS050_GridEmail.setColspan(row_id, Colinx - 1, 2);
    //        }
    //    }
    //////////////////////////////////////////////////
    colInx = ISS050_gridAttach.getColIndexById("removeButton")
    ISS050_gridAttach.setColumnHidden(colInx, false);
    SetFitColumnForBackAction(ISS050_gridAttach, "TmpColumn");
}



function disabledGridPOInfo() {
    //////// DISABLED BUTTON In PO GRID ///////////
    for (var i = 0; i < ISS050_GridPOInfo.getRowsNum(); i++) {
        var row_id = ISS050_GridPOInfo.getRowId(i);
        //        EnableGridButton(ISS050_GridPOInfo, "btnRemovePOInfo", row_id, "ButtonPORemove", false);
        //        EnableGridButton(ISS050_GridPOInfo, "btnEditPOInfo", row_id, "ButtonPOEdit", false);
        //        EnableGridButton(ISS050_GridPOInfo, "btnIssuePOInfo", row_id, "ButtonPOIssue", false);
    }
    //////////////////////////////////////////////////
    colInx = ISS050_GridPOInfo.getColIndexById("ButtonPORemove")
    ISS050_GridPOInfo.setColumnHidden(colInx, true);
    colInx = ISS050_GridPOInfo.getColIndexById("ButtonPOEdit")
    ISS050_GridPOInfo.setColumnHidden(colInx, true);
    colInx = ISS050_GridPOInfo.getColIndexById("ButtonPOIssue")
    ISS050_GridPOInfo.setColumnHidden(colInx, true);
}

function enabledGridPOInfo() {
    //    //////// ENABLED BUTTON In PO GRID ///////////
    colInx = ISS050_GridPOInfo.getColIndexById("ButtonPORemove")
    ISS050_GridPOInfo.setColumnHidden(colInx, false);
    colInx = ISS050_GridPOInfo.getColIndexById("ButtonPOEdit")
    ISS050_GridPOInfo.setColumnHidden(colInx, false);
    colInx = ISS050_GridPOInfo.getColIndexById("ButtonPOIssue")
    ISS050_GridPOInfo.setColumnHidden(colInx, false);

    SetFitColumnForBackAction(ISS050_GridPOInfo, "TempColumn");

}



function setSuccessRegisState() {
    // --------------- Initial SCREEN ------------------
    $("#divInputContractCode").show();
    $("#divContractBasicInfo").SetViewMode(true);
    $("#divProjectInfo").SetViewMode(true);
    $("#divInstallationInfo").SetViewMode(true);
    $("#divInstallationPOInfo").SetViewMode(true);
    $("#divInstallationMANo").SetViewMode(true);

    disabledGridEmail();
    disabledGridPOInfo();

    $("#divInstallationMANo").show();

    $("#ContractCodeProjectCode").attr("readonly", true);
    $("#btnRetrieveInstallation").attr("disabled", true);
    $("#btnClearInstallation").attr("disabled", false);

    //########## DISABLED INPUT CONTROL #################

    $("#IEStaffEmpNo1").attr("readonly", true);
    $("#IEStaffEmpNo2").attr("readonly", true);
    $("#SubcontractorCode").attr("disabled", true);
    $("#SubConstractorGroupName").attr("readonly", true);
    $("#NormalSubPOAmount").attr("readonly", true);
    $("#ActualPOAmount").attr("readonly", true);
    $("#PONo").attr("readonly", true);
    $("#ExpectInstallStartDate").EnableDatePicker(false)
    $("#ExpectInstallCompleteDate").EnableDatePicker(false)
    $("#btnPOAdd").attr("disabled", true);
    $("#btnPOClear").attr("disabled", true);

    $("#EmailAddress").attr("readonly", true);
    $("#btnAdd").attr("disabled", true);
    $("#btnClear").attr("disabled", true);
    $("#btnSearchEmail").attr("disabled", true);

    $("#POMemo").attr("readonly", true);
    $("#btnSearchEmail").attr("disabled", true);
    $("#btnSearchEmail").attr("disabled", true);
    $("#uploadFrame1").attr("disabled", true);
    //####################################################

    InitialCommandButton(0);

    colInx = ISS050_GridPOInfo.getColIndexById("ButtonPORemove")
    ISS050_GridPOInfo.setColumnHidden(colInx, false);
    colInx = ISS050_GridPOInfo.getColIndexById("ButtonPOEdit")
    ISS050_GridPOInfo.setColumnHidden(colInx, false);
    colInx = ISS050_GridPOInfo.getColIndexById("ButtonPOIssue")
    ISS050_GridPOInfo.setColumnHidden(colInx, false);

    SetFitColumnForBackAction(ISS050_GridPOInfo, "TempColumn");

    colInx = ISS050_GridPOInfo.getColIndexById("ButtonPORemove")
    ISS050_GridPOInfo.setColumnHidden(colInx, true);
    colInx = ISS050_GridPOInfo.getColIndexById("ButtonPOEdit")
    ISS050_GridPOInfo.setColumnHidden(colInx, true);

    SetFitColumnForBackAction(ISS050_GridPOInfo, "ButtonPOEdit");



}

function command_confirm_click() {
    command_control.CommandControlMode(false);
    //    $("#divContractBasicInfo").SetViewMode(false);
    //    $("#divProjectInfo").SetViewMode(false);
    //    $("#divInstallationInfo").SetViewMode(false);
    //    $("#divInstallationPOInfo").SetViewMode(false);
    //    $("#divInstallationMANo").SetViewMode(false);

    //    enabledGridEmail();
    //    enabledGridPOInfo();

    //var obj = CreateObjectData($("#form1").serialize() + "&POMemo=" + $("#POMemo").val() + "&IEStaffEmpNo1=" + $("#IEStaffEmpNo1").val() + "&IEStaffEmpNo2=" + $("#IEStaffEmpNo2").val());
    var obj = CreateObjectData($("#form1").serialize());
    obj.POMemo = $("#POMemo").val();
    obj.IEStaffEmpNo1 = $("#IEStaffEmpNo1").val();
    obj.IEStaffEmpNo2 = $("#IEStaffEmpNo2").val();

    master_event.LockWindow(true);
    call_ajax_method_json("/Installation/ISS050_RegisterData", obj, function (result, controls) {
        command_control.CommandControlMode(true);
        master_event.LockWindow(false);
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["InstallationType", "CustomerStaffBelonging", "CustomerStaffName", "CustomerStaffPhoneNo", "IEStaffEmpNo1", "IEStaffEmpNo2"], controls);
            /* --------------------- */

            return;
        }
        else if (result != undefined) {
            /* --- Set View Mode --- */
            /* --------------------- */
            $("#InstallationMANo").val(result.MaintenanceNo);
            setSuccessRegisState()
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

            ISS050_gridPOBinding(true);
            //setTimeout('disabledGridColumn(ISS050_GridPOInfo, "btnIssuePOInfo", "ButtonPOIssue", false)', 2000);
            /////////////////////////// PRINT REPORT AFTER SUCCESS //////////////////////////////
            //************************ COMMENT FOR ADD REPORT
            //************************
            ////////////////////////////////////////////////////////////////////////////////////
        }
    });


}

function command_reset_click() {
    command_control.CommandControlMode(false);
    var obj = {
        module: "Common",
        code: "MSG0038"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    command_control.CommandControlMode(true);
                    var strContractCode = $("#ContractCodeProjectCode").val();

                    setInitialState();
                    $("#btnRetrieveInstallation").attr("disabled", true);
                    btnClearEmailClick();
                    ClearPOInfo();
                    if (ISS050_gridAttach.getRowsNum() > 0)
                        DeleteAllRow(ISS050_gridAttach);

                    document.getElementById('divInputContractCode').scrollIntoView(true);

                    if (strContractCode != "") {
                        $("#ContractCodeProjectCode").val(strContractCode)
                        setTimeout("retrieve_installation_click()", 2000);
                    }
                }, function () { command_control.CommandControlMode(true); });
    });

}

function SetRegisterState(cond) {

    InitialCommandButton(1);

    //########## ENABLED INPUT CONTROL #################

    $("#IEStaffEmpNo1").attr("readonly", false);
    $("#IEStaffEmpNo2").attr("readonly", false);
    $("#SubcontractorCode").attr("disabled", false);
    $("#SubConstractorGroupName").attr("readonly", false);
    $("#NormalSubPOAmount").attr("readonly", false);
    $("#ActualPOAmount").attr("readonly", false);
    $("#PONo").attr("readonly", false);
    $("#ExpectInstallStartDate").EnableDatePicker(true)
    $("#ExpectInstallCompleteDate").EnableDatePicker(true)
    $("#btnPOAdd").attr("disabled", false);
    $("#btnPOClear").attr("disabled", false);

    $("#EmailAddress").attr("readonly", false);
    $("#btnAdd").attr("disabled", false);
    $("#btnClear").attr("disabled", false);
    $("#btnSearchEmail").attr("disabled", false);

    $("#POMemo").attr("readonly", false);
    $("#btnSearchEmail").attr("disabled", false);
    $("#btnSearchEmail").attr("disabled", false);

    $("#uploadFrame1").attr("disabled", false);
    //####################################################

    $("#ContractCodeProjectCode").attr("readonly", true);
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
    $("#divInputContractCode").show();
    $("#divAddUpdatePOInfo").show();
    $("#divAddEmail").show();

}

function CMS060Object() {
    var objArray = new Array();
    if (CheckFirstRowIsEmpty(ISS050_GridEmail) == false) {
        for (var i = 0; i < ISS050_GridEmail.getRowsNum(); i++) {
            var rowId = ISS050_GridEmail.getRowId(i);
            var selectedRowIndex = ISS050_GridEmail.getRowIndex(rowId);
            var mail = ISS050_GridEmail.cells2(selectedRowIndex, ISS050_GridEmail.getColIndexById('EmailAddress')).getValue();
            var EmpNo = ISS050_GridEmail.cells2(selectedRowIndex, ISS050_GridEmail.getColIndexById('EmpNo')).getValue();
            var iobj = {
                EmailAddress: mail,
                EmpNo: EmpNo
            };
            objArray.push(iobj);
        }
    }

    return { "EmailList": objArray };
}

function CMS060Response(result) {

    $("#dlgCTS053").CloseDialog();
    var emailColinx;
    var removeColinx;
    //var mygridCTS053 = $("#gridEmail").LoadDataToGridWithInitial(pageRow, false, false, "/Contract/GetEmailList_CTS053", result, "CTS053_DOEmailData", false);
    //var mygridCTS053 = $("#gridEmail").LoadDataToGridWithInitial(pageRow, false, false, "/Installation/GetEmailList_ISS010", result, "ISS010_DOEmailData", false);
    //btnClearEmailClick();
    DeleteAllRow(ISS050_GridEmail);
    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
    call_ajax_method_json("/Installation/ISS050_ClearInstallEmail", obj, function (res, controls) {

        call_ajax_method_json("/Installation/GetEmailList_ISS050", result, function (result, controls) {
            if (result != null && result.length > 0) {
                for (var i = 0; i < result.length; i++) {
                    // Fill to grid
                    var emailList = [result[i].EmailAddress, "", "", result[i].EmpNo];

                    CheckFirstRowIsEmpty(ISS050_GridEmail, true);
                    AddNewRow(ISS050_GridEmail, emailList);
                    ISS050_gridEmailBinding();
                }
            }
            BindOnloadGridEmail();
        });
    });



}

function btnClearEmailClick() {
    DeleteAllRow(ISS050_GridEmail);
    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
    call_ajax_method_json("/Installation/ISS050_ClearInstallEmail", obj, function (result, controls) {


    });

}

function ClearPOInfo() {
    DeleteAllRow(ISS050_GridPOInfo);
    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
    call_ajax_method_json("/Installation/ISS050_ClearPOInfo", obj, function (result, controls) {


    });

}


function moneyConvert(value) {
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
        value = sBuf + value.substring(value.indexOf("."), value.indexOf(".") + 3);
    }
    else {
        value = sBuf + ".00";
    }
    return value;
}


function btnRemoveAttach_click(row_id) {

    var obj = {
        module: "Common",
        code: "MSG0142"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    var _colID = ISS050_gridAttach.getColIndexById("AttachFileID");
                    var _targID = ISS050_gridAttach.cells(row_id, _colID).getValue();

                    var obj = {
                        AttachID: _targID
                    };
                    call_ajax_method_json("/Installation/ISS050_RemoveAttach", obj, function (result, controls) {
                        if (result != null) {
                            RefreshAttachList();
                        }
                    });
                });
    });

}

//Add by Jutarat A. on 21032014
function InitLoadAttachList() {

    ISS050_gridAttach = $("#ISS050_gridAttachDocList").LoadDataToGridWithInitial(0, false, false, "/Installation/ISS050_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, false, null,
                            function () {
                                if (hasAlert) {
                                    hasAlert = false;
                                    OpenWarningDialog(alertMsg);
                                }
                                $('#frmAttach').load(RefreshAttachList);

                                isInitAttachGrid = true;
                            });
}
//End Add

function RefreshAttachList() {

    //if (ISS050_gridAttach != null) {
    if (ISS050_gridAttach != undefined && isInitAttachGrid) { //Modify by Jutarat A. on 21032014

        $('#ISS050_gridAttachDocList').LoadDataToGrid(ISS050_gridAttach, 0, false, "/Installation/ISS050_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, function () {
            if (hasAlert) {
                hasAlert = false;
                OpenWarningDialog(alertMsg);
            }
        }, null)
    }
    //ISS050_gridAttachBinding(); //Comment by Jutarat A. on 21032014
}

function ISS050_gridAttachBinding() {
    //if (isInitAttachGrid) {
    if (ISS050_gridAttach != undefined) { //Modify by Jutarat A. on 21032014
        var _colRemoveBtn = ISS050_gridAttach.getColIndexById("removeButton");

        for (var i = 0; i < ISS050_gridAttach.getRowsNum(); i++) {
            var row_id = ISS050_gridAttach.getRowId(i);
            GenerateRemoveButton(ISS050_gridAttach, "btnRemoveAttach", row_id, "removeButton", true);
            BindGridButtonClickEvent("btnRemoveAttach", row_id, btnRemoveAttach_click);
        }
    }
    //Comment by Jutarat A. on 21032014
    //} else {
    //    isInitAttachGrid = true;
    //}
    //End Comment

    ISS050_gridAttach.setSizes();
}

function ISS050_gridEmailBinding() {
    var colInx = ISS050_GridEmail.getColIndexById('ButtonRemove');
    if (!CheckFirstRowIsEmpty(ISS050_GridEmail)) {
        for (var i = 0; i < ISS050_GridEmail.getRowsNum(); i++) {
            var rowId = ISS050_GridEmail.getRowId(i);
            GenerateRemoveButton(ISS050_GridEmail, "btnRemoveEmail", rowId, "ButtonRemove", true);

            // binding grid button event 
            BindGridButtonClickEvent("btnRemoveEmail", rowId, BtnRemoveEmailClick);

        }
        ISS050_GridEmail.setSizes();
    }
}

function ISS050_gridPOBinding(showIssueFlag) {

    var colInx = ISS050_GridPOInfo.getColIndexById('ButtonPORemove');
    TotalNormalPOAmount = 0;
    TotalActualPOAmount = 0;
    var decNormalPOAmount;
    var decActualPOAmount;
    for (var i = 0; i < ISS050_GridPOInfo.getRowsNum(); i++) {
        var rowId = ISS050_GridPOInfo.getRowId(i);

        var blnOldPO = (ISS050_GridPOInfo.cells2(i, ISS050_GridPOInfo.getColIndexById('OldPOData')).getValue() == "true");
        var strSubContractorCode = ISS050_GridPOInfo.cells2(i, ISS050_GridPOInfo.getColIndexById('SubConCode')).getValue();

        if (strSubContractorCode != "") {
            if (ISS050_GridPOInfo.cells2(i, ISS050_GridPOInfo.getColIndexById('SubConName')).getValue() != "" && ISS050_GridPOInfo.cells2(i, ISS050_GridPOInfo.getColIndexById('SubConName')).getValue() != DivTotal) {
                if (blnOldPO)
                    GenerateRemoveButton(ISS050_GridPOInfo, "btnRemovePOInfo", rowId, "ButtonPORemove", false);
                else
                    GenerateRemoveButton(ISS050_GridPOInfo, "btnRemovePOInfo", rowId, "ButtonPORemove", true);

                GenerateEditButton(ISS050_GridPOInfo, "btnEditPOInfo", rowId, "ButtonPOEdit", true);
                var EnablePrintReportPO = false;
                if (ISS050_GridPOInfo.cells2(i, ISS050_GridPOInfo.getColIndexById('OldPOData')).getValue() == "true" || showIssueFlag == true) {
                    EnablePrintReportPO = true;
                }

                GenerateDownloadButton(ISS050_GridPOInfo, "btnIssuePOInfo", rowId, "ButtonPOIssue", EnablePrintReportPO);

                decNormalPOAmount = 0;
                decActualPOAmount = 0;
                decNormalPOAmount = (ISS050_GridPOInfo.cells2(i, ISS050_GridPOInfo.getColIndexById('SubConPOAmount')).getValue()).replace(/,/g, '').split(' ');
                decActualPOAmount = (ISS050_GridPOInfo.cells2(i, ISS050_GridPOInfo.getColIndexById('ActualPOAmount')).getValue()).replace(/,/g, '').split(' ');
                TotalNormalPOAmount = TotalNormalPOAmount + (decNormalPOAmount[1].replace(/,/g, "") * 1);
                TotalActualPOAmount = TotalActualPOAmount + (decActualPOAmount[1].replace(/,/g, '') * 1);
            }
        }

        // binding grid button event 
        BindGridButtonClickEvent("btnRemovePOInfo", rowId, BtnRemovePOInfoClick);
        BindGridButtonClickEvent("btnEditPOInfo", rowId, BtnEditPOInfoClick);
        BindGridButtonClickEvent("btnIssuePOInfo", rowId, BtnIssuePOInfoClick);

    }



    ISS050_GridPOInfo.setSizes();
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

function BindOnloadGridEmail() {
    //============= TRS Add ===================
    var colInx = ISS050_GridEmail.getColIndexById('ButtonRemove');
    if (!CheckFirstRowIsEmpty(ISS050_GridEmail)) {
        for (var i = 0; i < ISS050_GridEmail.getRowsNum(); i++) {
            var rowId = ISS050_GridEmail.getRowId(i);
            GenerateRemoveButton(ISS050_GridEmail, "btnRemoveEmail", rowId, "ButtonRemove", true);

            // binding grid button event 
            BindGridButtonClickEvent("btnRemoveEmail", rowId, BtnRemoveEmailClick);

        }
        ISS050_GridEmail.setSizes();
    }
    //=========================================
}
function disabledGridColumn(grid, ButtonName, xmlColumnName, disableFlag) {
    var enableFlag = true;
    if (disableFlag) {
        enableFlag = false;
    }
    for (var i = 0; i < grid.getRowsNum(); i++) {
        var row_id = grid.getRowId(i);
        EnableGridButton(grid, ButtonName, row_id, xmlColumnName, enableFlag);

    }
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

function doAlertWithMessage(msgCode, msgText) {
    hasAlert = true;
    alertMsg = msgText;
}

//function ClearAllAttachFile() {

//    if (ISS050_gridAttach.getRowsNum() > 0)
//        DeleteAllRow(ISS050_gridAttach);

//    var obj = { strContractCode: "" };
//    call_ajax_method_json("/Installation/ISS050_ClearAllAttach", obj, function (result, controls) {


//    });
//}