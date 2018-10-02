

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
var ISS010_GridEmail;
//var ISS010_GridAttachedDoc;
var ISS010_gridAttach;
var isInitAttachGrid = false;

var hasAlert = false;
var alertMsg = "";
// Main
$(document).ready(function () {

    var strContractProjectCode = $("#ContractCodeProjectCode").val();

    $("#EmailSuffix").val(strEmailSuffix);
    $("#NewBldMgmtCost").BindNumericBox(12, 2, 0, 999999999999.99, 0);

    //    if ($.find("#ProposeInstallStartDate").length > 0) {
    //        InitialDateFromToControl("#ProposeInstallStartDate", "#ProposeInstallCompleteDate");

    //    }
    $("#ProposeInstallStartDate").InitialDate();
    $("#ProposeInstallCompleteDate").InitialDate();
    if ($("#NewPhoneLineOpenDate").length > 0) {
        $("#NewPhoneLineOpenDate").InitialDate();
    }


    $("#RequestMemo").SetMaxLengthTextArea(4000);
    $("#btnRetrieveInstallation").click(retrieve_installation_click);
    $("#btnClearInstallation").click(clear_installation_click);

    // ** tt
    //$("#btnAdd").click(function () { BtnAddClick(); });

    $("#btnAdd").click(function () { BtnAddClick(); });

    $("#btnClear").click(function () { BtnClearClick(); });
    $("#btnSearchEmail").click(function () { $("#dlgCTS053").OpenCMS060Dialog("CTS053"); });

    $("#btnDownloadDocument").attr("disabled", true);
    $("#btnDownloadDocument").click(function () { openISR060Document(); });


    setInitialState();
    //================ GRID ATTACH ========================================    
    //$('#frmAttach').attr('src', 'ISS010_Upload');
    $('#frmAttach').attr('src', 'ISS010_Upload?k=' + _attach_k);
    
    //ISS010_gridAttach = $("#ISS010_gridAttachDocList").InitialGrid(10, false, "/Installation/ISS010_IntialGridAttachedDocList");
    InitLoadAttachList(); //Modify by Jutarat A. on 21032014

    SpecialGridControl(ISS010_gridAttach, ["removeButton"]);
    BindOnLoadedEvent(ISS010_gridAttach, ISS010_gridAttachBinding);
    //$('#frmAttach').load(RefreshAttachList); //Comment by Jutarat A. on 21032014
    //====================================================================

    // intial grid    
    ISS010_GridEmail = $("#gridEmail").InitialGrid(pageRow, false, "/Installation/ISS010_InitialGridEmail");
    SpecialGridControl(ISS010_GridEmail, ["ButtonRemove"]);
    /* ===== binding event when finish load data ===== */
    BindOnLoadedEvent(ISS010_GridEmail, function () {
        var colInx = ISS010_GridEmail.getColIndexById('ButtonRemove');
        for (var i = 0; i < ISS010_GridEmail.getRowsNum(); i++) {
            var rowId = ISS010_GridEmail.getRowId(i);
            GenerateRemoveButton(ISS010_GridEmail, "btnRemoveEmail", rowId, "ButtonRemove", true);

            // binding grid button event 
            BindGridButtonClickEvent("btnRemoveEmail", rowId, BtnRemoveEmailClick);

        }
        ISS010_GridEmail.setSizes();
    });
    enabledGridEmail();

    if (strContractProjectCode != "") {
        $("#btnRetrieveInstallation").attr("disabled", true);
        $("#ContractCodeProjectCode").val(strContractProjectCode);
        retrieve_installation_click();
    }
});



function setInitialState() {
    // --------------- Initial SCREEN ------------------
    $("#divContractBasicInfo").SetViewMode(false);
    $("#divProjectInfo").SetViewMode(false);
    $("#divInstallationInfo").SetViewMode(false);
    //$("#divInstallationMANo").SetViewMode(false);

    

    $("#ContractCodeProjectCode").attr("readonly", false);
    $("#btnRetrieveInstallation").attr("disabled", false);
    $("#btnClearInstallation").attr("disabled", false);

    //########## DISABLED INPUT CONTROL #################
    $("#InstallationType").attr("disabled", true);
    $("#PlanCode").attr("readonly", true);
    //$("#ProposeInstallStartDate").attr("disabled", true);
    //$("#ProposeInstallCompleteDate").attr("disabled", true);

    $("#ProposeInstallStartDate").EnableDatePicker(false)
    $("#ProposeInstallCompleteDate").EnableDatePicker(false)

    $("#CustomerStaffBelonging").attr("readonly", true);
    $("#CustomerStaffName").attr("readonly", true);
    $("#CustomerStaffPhoneNo").attr("readonly", true);  
    $("#NewPhoneLineOpenDate").EnableDatePicker(false)
    $("#NewConnectionPhoneNo").attr("readonly", true);
    $("#NewPhoneLineOwnerTypeCode").attr("disabled", true);

    $("#EmailAddress").attr("readonly", true);
    $("#btnAdd").attr("disabled", true);
    $("#btnClear").attr("disabled", true);
    $("#btnSearchEmail").attr("disabled", true);

    $("#ApproveNo").attr("readonly", true); //Add by Jutarat A. on 17042013
    $("#RequestMemo").attr("readonly", true);
    $("#btnSearchEmail").attr("disabled", true);
    $("#btnSearchEmail").attr("disabled", true);
    $("#uploadFrame1").attr("disabled", true);

    

    //####################################################

    InitialCommandButton(0);
    $("#divInputContractCode").clearForm();
    $("#divContractBasicInfo").clearForm();
    $("#divProjectInfo").clearForm();
    $("#divInstallationInfo").clearForm();
    $("#divInstallationMANo").clearForm();

    $("#divInputContractCode").show();
    $("#divContractBasicInfo").show();
    $("#divProjectInfo").show();
    $("#divInstallationInfo").show();
    $("#divInstallationMANo").hide();

    $("#divInputEmail").show();
    $("#divAttachFrame").hide();
    $("#divAttachRemark").hide();
    
    //--------------------------------------------------  
}

function retrieve_installation_click() {
    //InitialCommandButton(1);
    $("#btnRetrieveInstallation").attr("disabled", true);
    command_control.CommandControlMode(false);
    var tmpCode = $("#ContractCodeProjectCode").val();
    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
    call_ajax_method_json("/Installation/ISS010_RetrieveData", obj,
        function (result, controls) {
            command_control.CommandControlMode(true);
            if (controls != undefined) {
                $("#btnRetrieveInstallation").attr("disabled", false);
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
                $("#divInstallationMANo").clearForm();

                return;
            }
            else if (result != undefined) {


                var obj = { strFieldName: "" };

                if (result.InstallType != undefined) {
                    obj.strFieldName = result.InstallType;
                    call_ajax_method('/Installation/ISS010_GetMiscInstallationtype', obj, function (result2, controls) {
                        if (result2.List.length != 1) {
                            regenerate_combo("#InstallationType", result2);

                            InitialInstallationType(result);
                        }
                    });
                }
                else {
                    $("#InstallationType").attr("disabled", true);
                }
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
                

                $("#divContractBasicInfo").clearForm();
                $("#divProjectInfo").clearForm();
                $("#divInstallationInfo").clearForm();
                $("#divInstallationMANo").clearForm();

                $("#ServiceTypeCode").val(result.ServiceTypeCode);
                $("#ContractCodeProjectCode").val(result.ContractProjectCodeForShow);

                if (result.dtRentalContractBasic != null) {
                    result.dtRentalContractBasic.ContractCode = $("#ContractCodeProjectCode").val();

                    $("#OperationOfficeCode").val(result.dtRentalContractBasic.OperationOfficeCode)
                    $("#divContractBasicInfo").bindJSON(result.dtRentalContractBasic);
                    $("#divInstallationInfo").bindJSON(result.dtRentalContractBasic);
                    //======= Money convert =================
                    $("#NewBldMgmtCost").SetNumericCurrency(result.dtRentalContractBasic.NewBldMgmtCostCurrencyType);
                    $("#NewBldMgmtCost").val(moneyConvert($("#NewBldMgmtCost").NumericValue()));
                    //=======================================
                    if (result.dtRentalContractBasic.BuildingTypeCode != null && result.dtRentalContractBasic.BuildingTypeName != null) {
                        $("#NewOldBuilding").val(result.dtRentalContractBasic.BuildingTypeCode + " : " + result.dtRentalContractBasic.BuildingTypeName);
                    }
                    if (result.dtRentalContractBasic.NewBldMgmtFlag) {
                        $("#NewBuildingMgmtType").val(lblNeed);
                    }
                    else {
                        $("#NewBuildingMgmtType").val(lblNoNeed);
                    }
                    $("#SalesmanEN1").val(result.dtRentalContractBasic.Salesman1);
                    $("#SalesmanEN2").val(result.dtRentalContractBasic.Salesman2);
                    SetRegisterState(1);
                }
                else if (result.dtSale != null) {
                    result.dtSale.ContractCode = $("#ContractCodeProjectCode").val();

                    $("#OperationOfficeCode").val(result.dtSale.OperationOfficeCode)
                    $("#divContractBasicInfo").bindJSON(result.dtSale);
                    $("#divInstallationInfo").bindJSON(result.dtSale);
                    if (result.dtSale.BuildingTypeCode != null && result.dtSale.BuildingTypeName != null) {
                        $("#NewOldBuilding").val(result.dtSale.BuildingTypeCode + " : " + result.dtSale.BuildingTypeName);
                    }
                    if (result.dtSale.NewBldMgmtFlag) {
                        $("#NewBuildingMgmtType").val(lblNeed);
                    }
                    else {
                        $("#NewBuildingMgmtType").val(lblNoNeed);
                    }
                    $("#SalesmanEN1").val(result.dtSale.Salesman1);
                    $("#SalesmanEN2").val(result.dtSale.Salesman2);
                    SetRegisterState(1);


                }
                else if (result.dtProject.ProjectCode != null) {

                    $("#divProjectInfo").bindJSON(result.dtProject);
                    //$("#ProductTypeCode").val(result.RentalContractBasicData.ProductTypeCode);

                    SetRegisterState(2);
                }


            }
            else {
                $("#btnRetrieveInstallation").attr("disabled", false);
                setInitialState();
                $("#ContractCodeProjectCode").val(tmpCode);
                VaridateCtrl(["ContractCodeProjectCode"], ["ContractCodeProjectCode"]);
            }
        }
    );
}


//function clear_installation_click() {
//    btnClearEmailClick();
//    setInitialState();    
//}

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
    setInitialState();
    //btnClearEmailClick();

    var obj = null;
    call_ajax_method_json("/Installation/ISS010_ClearCommonContractCode", obj, function (result, controls) {
        btnClearEmailClick();

        ClearAllAttachFile();
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
        var email = { "strEmail": $("#EmailAddress").val() + $("#EmailSuffix").val() };

        call_ajax_method_json("/Installation/ISS010_GetInstallEmail", email, function (result, controls, isWarning) {

            if (isWarning == undefined) { // No error and data(email) is exist

                if (result.length > 0) {
                    // Fill to grid
                    var emailList = [result[0].EmailAddress, "", result[0].EmpNo];

                    CheckFirstRowIsEmpty(ISS010_GridEmail, true);
                    AddNewRow(ISS010_GridEmail, emailList);

                    BindOnloadGridEmail();
                    $("#EmailAddress").val("");
                }

            }
            else {
                VaridateCtrl(["EmailAddress"], ["EmailAddress"]);
            }

        });
    }

}

function BtnRemoveEmailClick(row_id) {

    var obj = {
        module: "Common",
        code: "MSG0141"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    var selectedRowIndex = ISS010_GridEmail.getRowIndex(row_id);
                    var mail = ISS010_GridEmail.cells2(selectedRowIndex, ISS010_GridEmail.getColIndexById('EmailAddress')).getValue();
                    var obj = { EmailAddress: mail }
                    DeleteRow(ISS010_GridEmail, row_id);

                    call_ajax_method_json("/Installation/ISS010_RemoveMailClick", obj, function (result, controls, isWarning) {

                    });
                });
    });
    
  
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
//        call_ajax_method_json('/Installation/ValidateEmail_ISS010', obj,
//            function (result, controls) {
//                if (result == undefined) {
//                    mygrid = $("#gridEmail").LoadDataToGridWithInitial(pageRow, false, false, "/Installation/GetEmail_ISS010", obj, "ISS010_DOEmailData", false);

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
    //$("#divInstallationMANo").SetViewMode(false);
    $("#divInputEmail").show();
    $("#divAttachFrame").show();
    $("#divAttachRemark").show();
    $("#divInputContractCode").show();
    
    enabledGridEmail();
}

function command_register_click() {
    command_control.CommandControlMode(false);
	
	//var obj = CreateObjectData($("#form1").serialize() + "&RequestMemo=" + $("#RequestMemo").val() + "&InstallationType=" + $("#InstallationType").val() + "&ApproveNo=" + $("#ApproveNo").val()); //Add (ApproveNo) by Jutarat A. on 17042013
    var obj = CreateObjectData($("#form1").serialize());
	obj.RequestMemo = $("#RequestMemo").val(); 
	obj.InstallationType = $("#InstallationType").val(); 
	obj.ApproveNo = $("#ApproveNo").val(); //Add (ApproveNo) by Jutarat A. on 17042013
    call_ajax_method_json("/Installation/ISS010_ValidateBeforeRegister", obj, function (result, controls) {
        command_control.CommandControlMode(true);
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["InstallationType", "CustomerStaffBelonging", "CustomerStaffName", "CustomerStaffPhoneNo", "NewConnectionPhoneNo", "NewPhoneLineOpenDate", "ProposeInstallStartDate", "ProposeInstallCompleteDate", "NewPhoneLineOwnerTypeCode", "ApproveNo", "PlanCode"], controls); //Add (ApproveNo) by Jutarat A. on 17042013
            /* --------------------- */

            return;
        }
        else if(result != null) {
            validateWarningData();
        }
        //        else if (result == true) {
        //            setConfirmState();                      
        //        }
        //        else if (result != undefined) {
        //            
        //            OpenYesNoMessageDialog(result.Code, result.Message,
        //            function () {
        //                var obj = {
        //                    ObjectTypeID: 0
        //                };
        //                setConfirmState();

        //            },
        //            null);

        //        }

    });
}

function validateWarningData() {

    if (convertDatetoYMD($("#ProposeInstallStartDate")) * 1 < getCurrentDateFormatYMD() * 1) {
        // Get Message
        var obj = {
            module: "Installation",
            code: "MSG5010"
        };

        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
            OpenYesNoMessageDialog(result.Code, result.Message,
                    function () {
                        if ($("#ProposeInstallCompleteDate").val() != "") {
                            if (convertDatetoYMD($("#ProposeInstallCompleteDate")) * 1 < getCurrentDateFormatYMD() * 1) {
                                // Get Message
                                var obj = {
                                    module: "Installation",
                                    code: "MSG5011"
                                };

                                call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                                    OpenYesNoMessageDialog(result.Code, result.Message,
                                    function () {
                                        if (CheckFirstRowIsEmpty(ISS010_GridEmail, false) == true) {
                                            // Get Message
                                            var obj = {
                                                module: "Installation",
                                                code: "MSG5012"
                                            };

                                            call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                                                OpenYesNoMessageDialog(result.Code, result.Message,
                                                function () {
                                                    setConfirmState();
                                                }, function () {

                                                });

                                            });
                                        }
                                        else {
                                            setConfirmState();
                                        }
                                    }, function () {

                                    });

                                });
                            }
                            else {
                                if (CheckFirstRowIsEmpty(ISS010_GridEmail, false) == true) {
                                    // Get Message
                                    var obj = {
                                        module: "Installation",
                                        code: "MSG5012"
                                    };

                                    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                                        OpenYesNoMessageDialog(result.Code, result.Message,
                                                function () {
                                                    setConfirmState();
                                                }, function () {

                                                });

                                    });
                                }
                                else {
                                    setConfirmState();
                                }
                            }
                        }
                        else {
                            if (CheckFirstRowIsEmpty(ISS010_GridEmail, false) == true) {
                                // Get Message
                                var obj = {
                                    module: "Installation",
                                    code: "MSG5012"
                                };

                                call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                                    OpenYesNoMessageDialog(result.Code, result.Message,
                                                function () {
                                                    setConfirmState();
                                                }, function () {

                                                });

                                });
                            }
                            else {
                                            setConfirmState();
                            }
                        }
                    }, function () {

                    });

        });
    }
    else {
        if ($("#ProposeInstallCompleteDate").val() != "") {
            if (convertDatetoYMD($("#ProposeInstallCompleteDate")) * 1 < getCurrentDateFormatYMD() * 1) {
                // Get Message
                var obj = {
                    module: "Installation",
                    code: "MSG5011"
                };

                call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                    OpenYesNoMessageDialog(result.Code, result.Message,
                                    function () {
                                        if (CheckFirstRowIsEmpty(ISS010_GridEmail, false) == true) {
                                            // Get Message
                                            var obj = {
                                                module: "Installation",
                                                code: "MSG5012"
                                            };

                                            call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                                                OpenYesNoMessageDialog(result.Code, result.Message,
                                                function () {
                                                    setConfirmState();
                                                }, function () {

                                                });

                                            });
                                        }
                                        else {
                                            setConfirmState();
                                        }
                                    }, function () {

                                    });

                });
            }
            else {
                if (CheckFirstRowIsEmpty(ISS010_GridEmail, false) == true) {
                    // Get Message
                    var obj = {
                        module: "Installation",
                        code: "MSG5012"
                    };

                    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                        OpenYesNoMessageDialog(result.Code, result.Message,
                                                function () {
                                                    setConfirmState();
                                                }, function () {

                                                });

                    });
                }
                else {
                    setConfirmState();
                }

            }
        }
        else {
            if (CheckFirstRowIsEmpty(ISS010_GridEmail, false) == true) {
                // Get Message
                var obj = {
                    module: "Installation",
                    code: "MSG5012"
                };

                call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                    OpenYesNoMessageDialog(result.Code, result.Message,
                                                function () {
                                                    setConfirmState();
                                                }, function () {

                                                });

                });
            }
            else {
                setConfirmState();
            }
        }
    }        
}

function setConfirmState() {
    InitialCommandButton(2);  

    $("#divContractBasicInfo").SetViewMode(true);
    $("#divProjectInfo").SetViewMode(true);
    $("#divInstallationInfo").SetViewMode(true);
    //$("#divInstallationMANo").SetViewMode(true);
    $("#divInputEmail").hide();
    $("#divAttachFrame").hide();
    $("#divAttachRemark").hide();
    $("#divInputContractCode").hide();
    disabledGridEmail();
}

function setSuccessRegisState() {
    // --------------- Initial SCREEN ------------------
    $("#divContractBasicInfo").SetViewMode(true);
    $("#divProjectInfo").SetViewMode(true);
    $("#divInstallationInfo").SetViewMode(true);
    //$("#divInstallationMANo").SetViewMode(true);

    disabledGridEmail();

    $("#divInstallationMANo").show();

    $("#ContractCodeProjectCode").attr("readonly", true);
    $("#btnRetrieveInstallation").attr("disabled", true);
    $("#btnClearInstallation").attr("disabled", false);

    //########## DISABLED INPUT CONTROL #################
    $("#InstallationType").attr("disabled", true);
    $("#PlanCode").attr("readonly", true);

    //$("#ProposeInstallStartDate").attr("disabled", true);
    //$("#ProposeInstallCompleteDate").attr("disabled", true);

    $("#ProposeInstallStartDate").EnableDatePicker(false)
    $("#ProposeInstallCompleteDate").EnableDatePicker(false)

    $("#CustomerStaffBelonging").attr("readonly", true);
    $("#CustomerStaffName").attr("readonly", true);
    $("#CustomerStaffPhoneNo").attr("readonly", true);
    $("#NewPhoneLineOpenDate").EnableDatePicker(false)
    $("#NewConnectionPhoneNo").attr("readonly", true);
    $("#NewPhoneLineOwnerTypeCode").attr("disabled", true);

    $("#EmailAddress").attr("readonly", true);
    $("#btnAdd").attr("disabled", true);
    $("#btnClear").attr("disabled", true);
    $("#btnSearchEmail").attr("disabled", true);

    $("#ApproveNo").attr("readonly", true); //Add by Jutarat A. on 17042013
    $("#RequestMemo").attr("readonly", true);
    $("#btnSearchEmail").attr("disabled", true);
    $("#btnSearchEmail").attr("disabled", true);
    $("#uploadFrame1").attr("disabled", true);
    //####################################################

    InitialCommandButton(0);
}

function command_confirm_click() {
    command_control.CommandControlMode(false);
//    $("#divContractBasicInfo").SetViewMode(false);
//    $("#divProjectInfo").SetViewMode(false);
//    $("#divInstallationInfo").SetViewMode(false);
//    //$("#divInstallationMANo").SetViewMode(false);

    //enabledGridEmail();

	//var obj = CreateObjectData($("#form1").serialize() + "&RequestMemo=" + $("#RequestMemo").val() + "&InstallationType=" + $("#InstallationType").val() + "&ApproveNo=" + $("#ApproveNo").val()); //Add (ApproveNo) by Jutarat A. on 17042013
    var obj = CreateObjectData($("#form1").serialize());
	obj.RequestMemo = $("#RequestMemo").val();
	obj.InstallationType = $("#InstallationType").val();
	obj.ApproveNo = $("#ApproveNo").val(); //Add (ApproveNo) by Jutarat A. on 17042013
	
    master_event.LockWindow(true);
    call_ajax_method_json("/Installation/ISS010_RegisterData", obj, function (result, controls) {

        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["InstallationType", "CustomerStaffBelonging", "CustomerStaffName", "CustomerStaffPhoneNo"], controls);
            /* --------------------- */

            return;
        }
        else if (result != undefined) {
            /* --- Set View Mode --- */
            /* --------------------- */
            $("#InstallationMANo").val(result.MaintenanceNo);
            setSuccessRegisState()
            $("#divInputContractCode").show();
            document.getElementById('divInstallationMANo').scrollIntoView();
            var obj = {
                module: "Common",
                code: "MSG0046",
                param: ""
            };

            /* -------------------------- */
            /////////////////////////// PRINT REPORT AFTER SUCCESS //////////////////////////////
            //************************ COMMENT FOR ADD REPORT
            //window.open("ISS010_QuotationForCancelContractMemorandum");
            //************************

            $("#divContractBasicInfo").SetViewMode(false);
            $("#divProjectInfo").SetViewMode(false);
            $("#divInstallationInfo").SetViewMode(false);

            //CreateReportISR060(result.MaintenanceNo);
            CreateReportISR060AndReturnPath(result.MaintenanceNo, function () {
                call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                    //OpenWarningDialog(result.Message, result.Message, null);
                    OpenInformationMessageDialog(result.Code, result.Message);
                });
            });
            ////////////////////////////////////////////////////////////////////////////////////
        }
        command_control.CommandControlMode(true);
        master_event.LockWindow(false);
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
    command_control.CommandControlMode(false);
    var obj = {
        module: "Common",
        code: "MSG0038"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    command_control.CommandControlMode(true);
                    call_ajax_method_json("/Installation/ISS010_ResetButtonClick", obj, function (result) {
                        setInitialState();
                        $("#btnRetrieveInstallation").attr("disabled", true);
                        btnClearEmailClick();

                        ClearAllAttachFile();

                        document.getElementById('divInputContractCode').scrollIntoView();

                        if (result != null && result != "") {
                            $("#ContractCodeProjectCode").val(result);
                            retrieve_installation_click();
                        }
                    });
                }, function () { command_control.CommandControlMode(true); });
    });



        }

    

function SetRegisterState(cond) {

    InitialCommandButton(1);

    //########## ENABLED INPUT CONTROL #################
    $("#InstallationType").attr("disabled", false);
    $("#PlanCode").attr("readonly", false);

    //$("#ProposeInstallStartDate").attr("disabled", false);
    //$("#ProposeInstallCompleteDate").attr("disabled", false);

    $("#ProposeInstallStartDate").EnableDatePicker(true)
    $("#ProposeInstallCompleteDate").EnableDatePicker(true)

    $("#CustomerStaffBelonging").attr("readonly", false);
    $("#CustomerStaffName").attr("readonly", false);
    $("#CustomerStaffPhoneNo").attr("readonly", false);
    $("#NewPhoneLineOpenDate").EnableDatePicker(true)
    $("#NewConnectionPhoneNo").attr("readonly", false);
    $("#NewPhoneLineOwnerTypeCode").attr("disabled", false);

    $("#EmailAddress").attr("readonly", false);
    $("#btnAdd").attr("disabled", false);
    $("#btnClear").attr("disabled", false);
    $("#btnSearchEmail").attr("disabled", false);

    $("#ApproveNo").attr("readonly", false); //Add by Jutarat A. on 17042013
    $("#RequestMemo").attr("readonly", false);
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
}

function ClearAllAttachFile() {

    if (ISS010_gridAttach.getRowsNum() > 0)
        DeleteAllRow(ISS010_gridAttach);

    var obj = { strContractCode: "" };
    call_ajax_method_json("/Installation/ISS010_ClearAllAttach", obj, function (result, controls) {


    });
}


function CMS060Response(result) {
    
    $("#dlgCTS053").CloseDialog();
    var emailColinx;
    var removeColinx;
    //var mygridCTS053 = $("#gridEmail").LoadDataToGridWithInitial(pageRow, false, false, "/Contract/GetEmailList_CTS053", result, "CTS053_DOEmailData", false);
    //var mygridCTS053 = $("#gridEmail").LoadDataToGridWithInitial(pageRow, false, false, "/Installation/GetEmailList_ISS010", result, "ISS010_DOEmailData", false);

    //btnClearEmailClick();
    DeleteAllRow(ISS010_GridEmail);
    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
    call_ajax_method_json("/Installation/ISS010_ClearInstallEmail", obj, function (res, controls) {

        call_ajax_method_json("/Installation/GetEmailList_ISS010", result, function (result, controls) {
            if (result != null && result.length > 0) {
                for (var i = 0; i < result.length; i++) {
                    // Fill to grid
                    var emailList = [result[i].EmailAddress, "", "", result[i].EmpNo];

                    CheckFirstRowIsEmpty(ISS010_GridEmail, true);
                    AddNewRow(ISS010_GridEmail, emailList);
                }
                BindOnloadGridEmail();
            }

        });
    });
    

        

}

function btnClearEmailClick() {
    DeleteAllRow(ISS010_GridEmail);
    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
    call_ajax_method_json("/Installation/ISS010_ClearInstallEmail", obj, function (result, controls) {        

            
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

function getCurrentDateFormatYMD() {
    var myNow = new Date();
    var ddate = myNow.getDate();
    if (ddate < 10)
        ddate = "0" + ddate;
    var dmonth = myNow.getMonth() + 1;
    if (dmonth < 10)
        dmonth = "0" + dmonth;
    var dyear = myNow.getFullYear();

    var txt = "" + dyear + dmonth + ddate;
    return txt;
}

function disabledGridEmail() {
    //////// DISABLED BUTTON In EMAIL GRID ///////////
    colInx = ISS010_GridEmail.getColIndexById("ButtonRemove")
    ISS010_GridEmail.setColumnHidden(colInx, true);
    colInx = ISS010_gridAttach.getColIndexById("removeButton")
    ISS010_gridAttach.setColumnHidden(colInx, true);
//    for (var i = 0; i < ISS010_GridEmail.getRowsNum(); i++) {
//        var row_id = ISS010_GridEmail.getRowId(i);
//        EnableGridButton(ISS010_GridEmail, "btnRemoveEmail", row_id, "ButtonRemove", false);
//    }
    //////////////////////////////////////////////////
}

function enabledGridEmail() {
    colInx = ISS010_GridEmail.getColIndexById("ButtonRemove")
    ISS010_GridEmail.setColumnHidden(colInx, false);
    //////// ENABLED BUTTON In EMAIL GRID ///////////
//    for (var i = 0; i < ISS010_GridEmail.getRowsNum(); i++) {
//        var row_id = ISS010_GridEmail.getRowId(i);
//        //        EnableGridButton(ISS010_GridEmail, "btnRemoveEmail", row_id, "ButtonRemove", true);
//        //=================== TEST hide cloumn when back have scroll ====================
//        if (ISS010_GridEmail.rowsCol.length > 1) {
//            ISS010_GridEmail.setColspan(row_id, 1, 2);
//        }
//        //===============================================================================
    //    }
    SetFitColumnForBackAction(ISS010_GridEmail, "TempColumn");
    colInx = ISS010_gridAttach.getColIndexById("removeButton")
    ISS010_gridAttach.setColumnHidden(colInx, false);
    SetFitColumnForBackAction(ISS010_gridAttach, "TmpColumn");
    //////////////////////////////////////////////////
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


function btnRemoveAttach_click(row_id) {

    var obj = {
        module: "Common",
        code: "MSG0142"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    var _colID = ISS010_gridAttach.getColIndexById("AttachFileID");
                    var _targID = ISS010_gridAttach.cells(row_id, _colID).getValue();

                    var obj = {
                        AttachID: _targID
                    };
                    call_ajax_method_json("/Installation/ISS010_RemoveAttach", obj, function (result, controls) {
                        if (result != null) {
                            RefreshAttachList();
                        }
                    });
                });
    });

    
}

//Add by Jutarat A. on 21032014
function InitLoadAttachList() {

    ISS010_gridAttach = $("#ISS010_gridAttachDocList").LoadDataToGridWithInitial(0, false, false, "/Installation/ISS010_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, false, null,
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
    
    //if (ISS010_gridAttach != null) {
    if (ISS010_gridAttach != undefined && isInitAttachGrid) { //Modify by Jutarat A. on 21032014

        $('#ISS010_gridAttachDocList').LoadDataToGrid(ISS010_gridAttach, 0, false, "/Installation/ISS010_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, function () {
            if (hasAlert) {
                hasAlert = false;
                OpenWarningDialog(alertMsg);
            }
        }, null)
    }
    //ISS010_gridAttachBinding(); //Comment by Jutarat A. on 21032014
}

function ISS010_gridAttachBinding() {
    //if (isInitAttachGrid) {
    if (ISS010_gridAttach != undefined) { //Modify by Jutarat A. on 21032014
        var _colRemoveBtn = ISS010_gridAttach.getColIndexById("removeButton");

        for (var i = 0; i < ISS010_gridAttach.getRowsNum(); i++) {
            var row_id = ISS010_gridAttach.getRowId(i);
            GenerateRemoveButton(ISS010_gridAttach, "btnRemoveAttach", row_id, "removeButton", true);
            BindGridButtonClickEvent("btnRemoveAttach", row_id, btnRemoveAttach_click);
        }
    }
    //Comment by Jutarat A. on 21032014
    //} else {
    //    isInitAttachGrid = true;
        //}
    //End Comment

    ISS010_gridAttach.setSizes();
}

function BindOnloadGridEmail() {
    //============= TRS Add ===================
    var colInx = ISS010_GridEmail.getColIndexById('ButtonRemove');
    for (var i = 0; i < ISS010_GridEmail.getRowsNum(); i++) {
        var rowId = ISS010_GridEmail.getRowId(i);
        GenerateRemoveButton(ISS010_GridEmail, "btnRemoveEmail", rowId, "ButtonRemove", true);

        // binding grid button event 
        BindGridButtonClickEvent("btnRemoveEmail", rowId, BtnRemoveEmailClick);

    }
    ISS010_GridEmail.setSizes();
    //=========================================
}

function CreateReportISR060(MaintenanceNo) {

    var key = ajax_method.GetKeyURL(null);
    var link = ajax_method.GenerateURL("/Installation/ISS010_CreateReportInstallationRequest?MaintenanceNo=" + MaintenanceNo + "&k=" + key);
    window.open(link, "download");
}

function CreateReportISR060AndReturnPath(MaintenanceNo, onsuccess) {
    var obj = {
        MaintenanceNo: MaintenanceNo
    };

    call_ajax_method_json("/Installation/ISS010_CreateReportInstallationRequestFilePath", obj, function (result) {
        $("#DocumentFilePath").val(result);
        $("#btnDownloadDocument").attr("disabled", false);
        if (onsuccess) {
            onsuccess();
        }
    });
}

function openISR060Document() {
    //window.open($("#DocumentFilePath").val());
    //ISS010_DownloadPdfReport

    var key = ajax_method.GetKeyURL(null);
    var url = ajax_method.GenerateURL("/Installation/ISS010_DownloadPdfReport?k=" + key + "&filePath=" + $("#DocumentFilePath").val())

    window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");

}

function doAlertWithMessage(msgCode, msgText) {
    hasAlert = true;
    alertMsg = msgText;
}

function CMS060Object() {
    var objArray = new Array();
    if (CheckFirstRowIsEmpty(ISS010_GridEmail) == false) {
        for (var i = 0; i < ISS010_GridEmail.getRowsNum(); i++) {
            var rowId = ISS010_GridEmail.getRowId(i);
            var selectedRowIndex = ISS010_GridEmail.getRowIndex(rowId);
            var mail = ISS010_GridEmail.cells2(selectedRowIndex, ISS010_GridEmail.getColIndexById('EmailAddress')).getValue();
            var EmpNo = ISS010_GridEmail.cells2(selectedRowIndex, ISS010_GridEmail.getColIndexById('EmpNo')).getValue();
            var iobj = {
                EmailAddress: mail,
                EmpNo: EmpNo
            };
            objArray.push(iobj);
        }
    }

     return { "EmailList": objArray };
 }

 function InitialInstallationType(result) {
     var obj = { strFieldName: "" };
     if (result.ServiceTypeCode == C_SERVICE_TYPE_RENTAL) {
         if (result.doTbt_InstallationBasic != null && result.doTbt_InstallationBasic.InstallationType != "" && result.doTbt_InstallationBasic.InstallationType != null) {
             $("#InstallationType").val(result.doTbt_InstallationBasic.InstallationType);
             $("#InstallationType").attr("disabled", true);
         }
         else if (result.dtRentalContractBasic.FirstInstallCompleteFlag != C_FLAG_ON) {
             $("#InstallationType").val(C_RENTAL_INSTALL_TYPE_NEW);
             $("#InstallationType").attr("disabled", true);
         }
         else if (result.blnCheckCP12 == true) {
             $("#InstallationType").val(C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW);
             $("#InstallationType").attr("disabled", true);
         }
         else if (result.dtRentalContractBasic.ContractStatus == C_CONTRACT_STATUS_CANCEL || result.dtRentalContractBasic.ContractStatus == C_CONTRACT_STATUS_END) {
             $("#InstallationType").val(C_RENTAL_INSTALL_TYPE_REMOVE_ALL);
             $("#InstallationType").attr("disabled", true);
         }
         else {
             obj.strFieldName = result.InstallType;
             call_ajax_method('/Installation/ISS010_GetRentalInstalltypeOtherCase', obj, function (result3, controls) {
                 if (result3.List.length != 1) {
                     regenerate_combo("#InstallationType", result3);
                 }
             });
             $("#InstallationType").attr("disabled", false);
         }
     }
     else if (result.ServiceTypeCode == C_SERVICE_TYPE_SALE) {
         if (result.doTbt_InstallationBasic != null && result.doTbt_InstallationBasic.InstallationType != "" && result.doTbt_InstallationBasic.InstallationType != null) {
             $("#InstallationType").val(result.doTbt_InstallationBasic.InstallationType);
             $("#InstallationType").attr("disabled", true);
         }
         else if (result.dtSale.ChangeType == C_SALE_CHANGE_TYPE_NEW_SALE
                                        && result.dtSale.InstallationCompleteFlag != C_FLAG_ON
                                ) {
             $("#InstallationType").val(C_SALE_INSTALL_TYPE_NEW);
             $("#InstallationType").attr("disabled", true);
         }
         else if (result.dtSale.ChangeType == C_SALE_CHANGE_TYPE_ADD_SALE
                                        && result.dtSale.InstallationCompleteFlag != C_FLAG_ON
                                ) {
             $("#InstallationType").val(C_SALE_INSTALL_TYPE_ADD);
             $("#InstallationType").attr("disabled", true);
         }
         else {
             obj.strFieldName = result.InstallType;
             call_ajax_method('/Installation/ISS010_GetSaleInstalltypeOtherCase', obj, function (result4, controls) {
                 if (result4.List.length != 1) {
                     regenerate_combo("#InstallationType", result4);
                 }
             });
             $("#InstallationType").attr("disabled", false);
         }
     }
 }

 function moneyConvert(value) {
     var blnMinusValue = false;
     if (value < 0) {
         value = String(value).substring(String(value).indexOf("-") + 1);
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
         if (blnMinusValue)
             value = "-" + value
         return value;
     }
 }