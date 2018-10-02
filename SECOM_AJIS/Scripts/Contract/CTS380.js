
/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/control_events.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>
/// <reference path="../Base/object/ajax_method.js" />
/// <reference path="../Base/object/command_event.js" />
/// <reference path="../Base/object/master_event.js" />

// ---------------------------------------------------------------------------------
// Initial
// ---------------------------------------------------------------------------------
var picDat = new Array();
var arDat = null;
var historyDat = "";
var historyWithUpdateDat = "";
//var interactCBB = "";
var picLOGDAT = new Array();
var btnRemovePICID = "btnRemovePIC";
var cbbWidth = "260px";
var btnRemoveAttachID = "btnRemoveAttach";
var btnDownloadAttachID = "btnDownloadAttach";
var _chkSendMailPICID = "_chkSendMailPIC";

var gridPIC = null;
var gridAttach = null;
var validatePIC = ['OfficeCode', 'DepartmentCode', 'ARRoleCode', 'EmployeeCode'];

//var validateForm = ['InteractionType', 'ARStatusAfterUpdate', 'AuditDetail', 'DueDate_Date', 'DueDate_Time', 'Deadline_Date', 'Deadline_Until'];
var validateForm = ['InteractionType', 'ARStatusAfterUpdate', 'AuditDetail'];

var isViewHistory = true;
var isInitInteractionCBB = false;
var isInitGrid = false;
var isLoadData = false;
var isInitAttachGrid = false;
var isAfterConfirm = false;

var hasAlert = false;
var alertMsg = "";

//Add by Jutarat A. on 26042013
var numBox_Length_10 = 12;
var numBox_Length_9 = 11;
var numBox_Decimal = 2;
var numBox_Min = 0;
var numBox_Max_10 = 999999999999.99;
var numBox_Max_9 = 99999999999.99;
var numBox_DefaultMin = false;
//End Add

$(document).ready(function () {
    InitEvent();
    InitialPage();
    InitialGrid();
});

function InitEvent() {
    //Comment by Jutarat A. on 09012014
    //$('#frmAttach').attr('src', 'CTS380_Upload?sK=' + param + '&k=' + _attach_k);
    //$('#frmAttach').load(RefreshAttachList);
    //End Comment

    $('#btnHistory').click(btnHistory_click);
    //$('#ARStatusAfterUpdate').change(ARStatusAfterUpdate_change);
//    $('#DueDate_Type').change(DueDate_DateTime_change);
//    $('#Deadline_Type').change(DueDate_DateTime_change);
    $('#OfficeCode').change(OfficeCode_change);
    $('#DepartmentCode').change(DepartmentCode_change);
    $('#ARRoleCode').change(ARRoleCode_change);
    $('#btnAddPIC').click(btnAddPIC_click);

//    $("#Deadline_Date").InitialDate();
//    $("#DueDate_Date").InitialDate();
//    $("#DueDate_Time").BindTimeBox();

    $('#ARStatusAfterUpdate').change(ARStatusAfterUpdate_change); //Add by Jutarat A. on 30082012

    //Add by Jutarat A. on 03042013
    $('#ContractQuotationFee').blur(CalculateGridFee1);
    $('#ContractARFee').blur(CalculateGridFee1);
    $('#DepositQuotationFee').blur(CalculateGridFee1);
    $('#DepositARFee').blur(CalculateGridFee1);
    $('#InstallQuotationFee').blur(CalculateGridFee1);
    $('#InstallARFee').blur(CalculateGridFee1);

    $('#ProductPriceQuotationFee').blur(CalculateGridFee2);
    $('#ProductPriceARFee').blur(CalculateGridFee2);
    $('#InstallFeeQuotationFee').blur(CalculateGridFee2);
    $('#InstallFeeARFee').blur(CalculateGridFee2);
    //End Add

    $('#MonthlyContractFee').BindNumericBox(numBox_Length_10, numBox_Decimal, numBox_Min, numBox_Max_10, numBox_DefaultMin);

    //Add by Jutarat A. on 26042013
    $('#ContractQuotationFee').BindNumericBox(numBox_Length_10, numBox_Decimal, numBox_Min, numBox_Max_10, numBox_DefaultMin);
    $('#ContractARFee').BindNumericBox(numBox_Length_10, numBox_Decimal, numBox_Min, numBox_Max_10, numBox_DefaultMin);
    $('#DepositQuotationFee').BindNumericBox(numBox_Length_10, numBox_Decimal, numBox_Min, numBox_Max_10, numBox_DefaultMin);
    $('#DepositARFee').BindNumericBox(numBox_Length_10, numBox_Decimal, numBox_Min, numBox_Max_10, numBox_DefaultMin);
    $('#InstallQuotationFee').BindNumericBox(numBox_Length_10, numBox_Decimal, numBox_Min, numBox_Max_10, numBox_DefaultMin);
    $('#InstallARFee').BindNumericBox(numBox_Length_10, numBox_Decimal, numBox_Min, numBox_Max_10, numBox_DefaultMin);
    $('#ProductPriceQuotationFee').BindNumericBox(numBox_Length_10, numBox_Decimal, numBox_Min, numBox_Max_9, numBox_DefaultMin);
    $('#ProductPriceARFee').BindNumericBox(numBox_Length_10, numBox_Decimal, numBox_Min, numBox_Max_9, numBox_DefaultMin);
    $('#InstallFeeQuotationFee').BindNumericBox(numBox_Length_10, numBox_Decimal, numBox_Min, numBox_Max_9, numBox_DefaultMin);
    $('#InstallFeeARFee').BindNumericBox(numBox_Length_10, numBox_Decimal, numBox_Min, numBox_Max_9, numBox_DefaultMin);
    //End Add

    $("#AuditDetail").SetMaxLengthTextArea(10000); //Add by Jutarat A. on 18102013
}

function InitialPage() {
    call_ajax_method_json("/Contract/CTS380_RetrieveInteractionCBB", "", function (result, controls) {
        if ((result != null) && (result.length > 0)) {
            $('#divInteractionCBB').html(result.replace('{BLANK_ID}', 'InteractionType'));
            //interactCBB = result.replace('{BLANK_ID}', 'InteractionType');
            $('#InteractionType').change(InteractionType_change);
        } else {
            $('#divInteractionCBB').html(CTS380_BlankComboBox.replace('{BLANK_ID}', 'InteractionType'));
        }

        $('#InteractionType').attr('style', 'width: ' + CTS380_DefaultCBBWidth + ';');
        isInitInteractionCBB = true;
        RetrieveData();
    });

    SetScreenToDefault();

    //RetrieveData();
}

function InitialGrid() {
    gridPIC = $('#CTS380_gridAssignPIC').InitialGrid(0, false, "/Contract/CTS380_InitialAssignPersonInChargeGrid", function()
    {
        isInitGrid = true;
        RetrieveData();
    });
    //gridPIC = $('#CTS380_gridAssignPIC').InitialGrid(1, false, "/Contract/CTS380_InitialAssignPersonInChargeGrid");
    
    //Modify by Jutarat A. on 09012013
    //gridAttach = $("#CTS380_gridAttachDocList").InitialGrid(0, false, "/Contract/CTS380_IntialGridAttachedDocList", function()
    //{
    //    isInitAttachGrid = true;
    //});
    gridAttach = $("#CTS380_gridAttachDocList").LoadDataToGridWithInitial(0, false, false, "/Contract/CTS380_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, false, null,
                    function () {
                        isInitAttachGrid = true;
                        $('#frmAttach').attr('src', 'CTS380_Upload?sK=' + param + '&k=' + _attach_k);
                        $('#frmAttach').load(RefreshAttachList);

                        if (hasAlert) {
                            hasAlert = false;
                            OpenWarningDialog(alertMsg);
                        }
                    });
    //End Modify

    SpecialGridControl(gridPIC, ["RemoveBtn"]);
    SpecialGridControl(gridAttach, ["removeButton", "downloadButton"]);

    BindOnLoadedEvent(gridPIC, gridPIC_binding);
    BindOnLoadedEvent(gridAttach, gridAttach_binding);

    RetrieveData();
}

// ---------------------------------------------------------------------------------
// Event
// ---------------------------------------------------------------------------------
function gridAttach_binding(gen_ctrl) {
    //if (isInitAttachGrid) {
    if (gridAttach != null && CheckFirstRowIsEmpty(gridAttach, false) == false) { //Modify by Jutarat A. on 09012014
        var _colRemoveBtn = gridAttach.getColIndexById("removeButton");

        for (var i = 0; i < gridAttach.getRowsNum(); i++) {
            var row_id = gridAttach.getRowId(i);
            if (gen_ctrl == true)
            {
                GenerateRemoveButton(gridAttach, btnRemoveAttachID, row_id, "removeButton", true);
                GenerateDownloadButton(gridAttach, btnDownloadAttachID, row_id, "downloadButton", true);
            }

            BindGridButtonClickEvent(btnRemoveAttachID, row_id, btnRemoveAttach_clicked);
            BindGridButtonClickEvent(btnDownloadAttachID, row_id, btnDownloadAttach_clicked);
        }
    } else {
        //isInitAttachGrid = true;
//        $('#frmAttach').attr('src', 'CTS380_Upload?sK=' + param);
//        $('#frmAttach').load(RefreshAttachList);
    }

    if (!arDat.CanSpecialViewEdit && !arDat.CanEdit) {
        var _colRemoveAttach = gridAttach.getColIndexById("removeButton");
        var _colDownloadAttach = gridAttach.getColIndexById("downloadButton");

        gridAttach.setColumnHidden(_colRemoveAttach, true);
        //gridAttach.setColumnHidden(_colDownloadAttach, true);
    }

    gridAttach.setSizes();
}

function btnDownloadAttach_clicked(row_id) {
    var _colID = gridAttach.getColIndexById("AttachFileID");
    var _targID = gridAttach.cells(row_id, _colID).getValue();

    var obj = {
        AttachID: _targID
    };

    //Modify by Jutarat A. on 29012013
//    var key = ajax_method.GetKeyURL(null);
//    var link = ajax_method.GenerateURL("/contract/CTS380_DownloadAttach" + "?AttachID=" + _targID + "&k=" + key);
//    //window.location.href(link);
//    window.open(link, "download");
    download_method.CallDownloadController("ifDownload", "/Contract/CTS380_DownloadAttach", obj);
    //End Modify
}

function btnRemoveAttach_clicked(row_id) {
    var _colID = gridAttach.getColIndexById("AttachFileID");
    var _targID = gridAttach.cells(row_id, _colID).getValue();

    var obj = {
        AttachID: _targID
    };
    call_ajax_method_json("/Contract/CTS380_RemoveAttach", obj, function (result, controls) {
        if (result != null) {
            RefreshAttachList();
        }
    });

}

function RefreshAttachList() {
    if ((gridAttach != null) && isInitAttachGrid) {
        $('#CTS380_gridAttachDocList').LoadDataToGrid(gridAttach, 0, false, "/Contract/CTS380_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, function ()
        {
            if (hasAlert)
            {
                hasAlert = false;
                OpenWarningDialog(alertMsg);
            }
        }, null)
    }
}

function RegisterCommand_clicked() {
    VaridateCtrl(validateForm, null);
    DisableRegisterCommand(true);
    DisableResetCommand(true);

    var obj = CreateFormObject();

    call_ajax_method_json("/Contract/CTS380_ValidateData", obj, function (result, controls) {
        VaridateCtrl(validateForm, controls);
        if ((result != null) && (result == true)) {
            DisableRegisterCommandPane();
            SetViewModeForm(true);
            EnableConfirmCommandPane();
        }

        DisableRegisterCommand(false);
        DisableResetCommand(false);
    });
    
}

function ResetCommand_clicked() {
    DisableRegisterCommand(true);
    doAskYesNo("Common", "MSG0038", null, function ()
    {
        RetrieveData();
        SetScreenToDefault();
        BindDataToForm();
        DisableRegisterCommand(false);
    },
    function ()
    {
        DisableRegisterCommand(false);
    });
}

function ConfirmCommand_clicked() {
    DisableConfirmCommand(true);
    DisableBackCommand(true); //Add by Jutarat A. on 03102012

    var obj = CreateFormObject();

    call_ajax_method_json("/Contract/CTS380_EditARData", obj, function (result, controls) {
        if ((result != null) && (result != null)) {
            var obj = {
                module: "Common",
                code: "MSG0046"
            };

            call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                OpenInformationMessageDialog(result.Code, result.Message, function () {
                    isInitInteractionCBB = true;
                    //isInitGrid = false;
                    isLoadData = false;

                    isAfterConfirm = true;

                    InitialPage();
                    //InitialGrid();
                    //RetrieveData();

                    DisableConfirmCommand(false);
                    DisableBackCommand(false); //Add by Jutarat A. on 03102012
                });
            });
        }

        DisableConfirmCommand(false);
        DisableBackCommand(false); //Add by Jutarat A. on 03102012
    });
    
}

function BackCommand_clicked() {
    DisableConfirmCommand(true);
    DisableConfirmCommandPane();
    EnableRegisterCommandPane();
    SetViewModeForm(false, true);
    DisableConfirmCommand(false);
}

function gridPIC_binding(gen_ctrl, chkCanEditSendMail) {
    if (!isInitGrid) {
//        isInitGrid = true;
//        RetrieveData();
    } else {
        var _colSendMail = gridPIC.getColIndexById("SendMail");
        var _colRemoveBtn = gridPIC.getColIndexById("RemoveBtn");
        var _colEmpNo = gridPIC.getColIndexById("EmpNo");

        var isEnbSendMail; //Add by Jutarat A. on 05092012

        for (var i = 0; i < gridPIC.getRowsNum(); i++) {
            var row_id = gridPIC.getRowId(i);
            var picObj = GetPIC(gridPIC.cells(row_id, _colEmpNo).getValue());

            if (picObj != null) {
                if (picObj.CanDelete == true) {
                    if (gen_ctrl == true)
                    {
                        GenerateRemoveButton(gridPIC, btnRemovePICID, row_id, "RemoveBtn", arDat.CanSpecialViewEdit || arDat.CanEdit);
                    }
                    if (arDat.CanSpecialViewEdit || arDat.CanEdit) {
                        BindGridButtonClickEvent(btnRemovePICID, row_id, RemovePIC_clicked);
                    }
                }

                //Add by Jutarat A. on 05092012
                isEnbSendMail = true;
                if (chkCanEditSendMail) {
                    isEnbSendMail = picObj.CanEditSendMail;
                }
                //End Add

                gridPIC.cells(row_id, _colSendMail).setValue(GenerateCheckBox(_chkSendMailPICID, row_id, "", isEnbSendMail)); //true)); //Modify by Jutarat A. on 05092012
                var chkID = GenerateGridControlID(_chkSendMailPICID, row_id);
                var ctrl = $('#' + chkID);

                if (picObj.SendMail) {
                    $(ctrl).attr('checked', 'checked');
                }

                $(ctrl).click(function () {
                    var rid = GetGridRowIDFromControl(this);
                    SendMailPIC_change(rid);
                });
            }
        }
    }

    gridPIC.setSizes();
}

function SendMailPIC_change(row_id) {
    var _colEmpNo = gridPIC.getColIndexById("EmpNo");
    var _targEmpNo = gridPIC.cells(row_id, _colEmpNo).getValue();

    var chkID = GenerateGridControlID(_chkSendMailPICID, row_id);
    var newVal = $('#' + chkID).is(':checked');

    var currObj = GetPIC(_targEmpNo);
    currObj.SendMail = newVal;
    UpdatePIC(currObj);
}

function RemovePIC_clicked(row_id, confirmed) {
    var _colEmpNo = gridPIC.getColIndexById("EmpNo");
    var _targEmpNo = gridPIC.cells(row_id, _colEmpNo).getValue();

    var objPIC = GetPIC(_targEmpNo);
    if (objPIC.ARRoleCode == _C_AR_ROLE_APPROVER) {
        if (!confirmed && _targEmpNo == _CurrentEmpNo) {
            var obj = {
                module: "Contract",
                code: "MSG3313"
            };

            call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                OpenYesNoMessageDialog(result.Code, result.Message, function () {
                    RemovePIC_clicked(row_id, true);
                });
            });

            return;
        }
    }

    if (hasPICMOD(_targEmpNo.toString())) {
        // Has log exists
        var tmpObj = GetPICMOD(_targEmpNo.toString());
        if (tmpObj.Action == "1") {
            // Has log 'ADD' >> Remove log
            RemovePICMOD(_targEmpNo.toString());
        } else {
            // Has log 'EDIT', 'REMOVE' >>> Set to 'REMOVE'
            tmpObj.Action = "3";
            RemovePICMOD(_targEmpNo.toString());
            AddPICMOD(tmpObj);
        }
    } else {
        // Insert new log
        var obj = {
            EmpNo: _targEmpNo.toString(),
            Action: "3"
        }
        AddPICMOD(obj);
    }

    if (hasPIC(_targEmpNo.toString())) {
        RemovePIC(_targEmpNo.toString());
    }

    DeleteRow(gridPIC, row_id);
}

function chkSendMailPICID() {
    var _colEmpNo = gridPIC.getColIndexById("EmpNo");
    var _targEmpNo = gridPIC.cells(row_id, _colEmpNo).getValue();

    var chkID = GenerateGridControlID(chkSendMailPICID, row_id);
    var newVal = $('#' + chkID).is(':checked');

    var currObj = GetPIC(_targEmpNo);
    currObj.SendMail = newVal;
    RemovePIC(_targEmpNo);
    AddPIC(currObj);
}

function RetrieveData() {
    if (isInitGrid && isInitInteractionCBB) {
        call_ajax_method_json("/Contract/CTS380_RetrieveARData", "", function (result, controls) {
            if (result != null) {
                arDat = result;

                if (isAfterConfirm) {
                    isAfterConfirm = false;
                    $('#frmAttach').unbind('load');
                }

                BindDataToForm();
            }
        });
    }
}

function btnHistory_click() {
    isViewHistory = !isViewHistory;

    if (!isViewHistory) {
        // View Normal
        $('#divRespondingProgress').html(historyDat);
    } else {
        // View History
        $('#divRespondingProgress').html(historyWithUpdateDat);
    }
}

function InteractionType_change() {
    var obj = {
        InteractionTypeCode: $('#InteractionType').val()
    };

    call_ajax_method_json("/Contract/CTS380_InteractionTypeChange", obj, function (result, controls) {
        if (result != null) {
            $('#ARStatusAfterUpdate').val(result.ARStatus);
            ARStatusAfterUpdate_change(); //Add by Jutarat A. on 31082012

            if (result.IsEnable) {
                $('#ARStatusAfterUpdate').removeAttr('disabled')
            } else {
                $('#ARStatusAfterUpdate').attr('disabled', 'true');
            }
        }
    });
}

//Add by Jutarat A. on 30082012
function ARStatusAfterUpdate_change() {
    var isChckAudit = false;
    var isChckRecep = false;
    var isChckReqst = false;
    var isChckApprv = false;

    var isEnbAudit = true;
    var isEnbRecep = true;
    var isEnbReqst = true;
    var isEnbApprv = true;

    if ($('#ARStatusAfterUpdate').val() == _C_AR_STATUS_WAIT_FOR_APPROVAL) {
        isChckApprv = true;
        isEnbApprv = false;
    }
    else if ($('#ARStatusAfterUpdate').val() == _C_AR_STATUS_AUDITING) {
        isChckAudit = true;
        isEnbAudit = false;

        isChckApprv = false;
        isEnbApprv = false;
    }
    else if ($('#ARStatusAfterUpdate').val() == _C_AR_STATUS_APPROVED
            || $('#ARStatusAfterUpdate').val() == _C_AR_STATUS_REJECTED
            || $('#ARStatusAfterUpdate').val() == _C_AR_STATUS_INSTRUCTED
            || $('#ARStatusAfterUpdate').val() == _C_AR_STATUS_RETURNED_REQUEST) {
        isChckRecep = true;
        isEnbRecep = true;

        isChckReqst = true;
        isEnbReqst = false;

        isChckApprv = false;
        isEnbApprv = false;
    }

    var isChckSendMail = false;
    var isEnbSendMail = true;

    var _colSendMail = gridPIC.getColIndexById("SendMail");
    var _colRemoveBtn = gridPIC.getColIndexById("RemoveBtn");
    var _colEmpNo = gridPIC.getColIndexById("EmpNo");

    for (var i = 0; i < gridPIC.getRowsNum(); i++) {
        var row_id = gridPIC.getRowId(i);
        var picObj = GetPIC(gridPIC.cells(row_id, _colEmpNo).getValue());

        if (picObj != null) {
            if (picObj.ARRoleCode == _C_AR_ROLE_AUDITOR) {
                isChckSendMail = isChckAudit;
                isEnbSendMail = isEnbAudit;
            }
            else if (picObj.ARRoleCode == _C_AR_ROLE_RECEIPIENT) {
                isChckSendMail = isChckRecep;
                isEnbSendMail = isEnbRecep;
            }
            else if (picObj.ARRoleCode == _C_AR_ROLE_REQUESTER) {
                isChckSendMail = isChckReqst;
                isEnbSendMail = isEnbReqst;
            }
            else if (picObj.ARRoleCode == _C_AR_ROLE_APPROVER) {
                isChckSendMail = isChckApprv;
                isEnbSendMail = isEnbApprv;
            }
            else {
                isChckSendMail = false;
                isEnbSendMail = true;
            }

            picObj.SendMail = isChckSendMail;
            picObj.CanEditSendMail = isEnbSendMail;

            gridPIC.cells(row_id, _colSendMail).setValue(GenerateCheckBox(_chkSendMailPICID, row_id, "", isEnbSendMail));

            var chkID = GenerateGridControlID(_chkSendMailPICID, row_id);
            var ctrl = $('#' + chkID);
            $(ctrl).attr('checked', isChckSendMail);

            $(ctrl).click(function () {
                var rid = GetGridRowIDFromControl(this);
                SendMailPIC_change(rid);
            });
        }
    }

    gridPIC.setSizes();

}

//function DueDate_DateTime_change() {
//    var currops = GetDueDateDateTime();

//    $('#divDueDate').clearForm();
//    $('#divDeadLine').clearForm();
//    SetDueDateDateTime(currops);

//    if (currops == "1") {
//        $('#Deadline_Date').EnableDatePicker(false);
//        $('#Deadline_Until').val('');
//        $('#Deadline_Until').attr("disabled", true);
//        $('#DueDate_Date').EnableDatePicker(true);
//        $('#DueDate_Time').removeAttr('readonly');
//    } else if (currops == "2") {
//        $('#DueDate_Date').EnableDatePicker(false);
//        $('#DueDate_Time').val('');
//        $('#DueDate_Time').attr('readonly', true);
//        $('#Deadline_Date').EnableDatePicker(true);
//        $('#Deadline_Until').removeAttr('disabled');
//    }
//}

function OfficeCode_change() {
    var obj = {
        OfficeCode: $('#OfficeCode').val()
    };

    //Modify by jutarat A. on 20082012
    $('#divCBBDepartment').html(CTS380_BlankComboBox.replace('{BLANK_ID}', 'DepartmentCode'));
    $('#divCBBARRole').html(CTS380_BlankComboBox.replace('{BLANK_ID}', 'ARRoleCode'));
    $('#divCBBEmployee').html(CTS380_BlankComboBox.replace('{BLANK_ID}', 'EmployeeCode'));

    //if ($('#OfficeCode').val().length > 0) {
        call_ajax_method_json("/Contract/CTS380_OfficeChange", obj, function (result, controls) {
            //if (result != null) {
            if ((result != null) && (result.length > 0)) { //Modify by jutarat A. on 20082012
                $('#divCBBDepartment').html(result.replace('{BLANK_ID}', 'DepartmentCode'));
                $('#DepartmentCode').change(DepartmentCode_change);
            }

            //Add by jutarat A. on 20082012
            if ((result != null) && (result == false)) {
                $('#DepartmentCode').attr("disabled", true);
                DepartmentCode_change();
            }
            //End Add

            $('#divCBBARRole').html(CTS380_BlankComboBox.replace('{BLANK_ID}', 'ARRoleCode'));
            $('#divCBBEmployee').html(CTS380_BlankComboBox.replace('{BLANK_ID}', 'EmployeeCode'));

            $('#DepartmentCode').attr('style', 'width: ' + cbbWidth + ";");
            $('#ARRoleCode').attr('style', 'width: ' + cbbWidth + ";");
            $('#EmployeeCode').attr('style', 'width: ' + cbbWidth + ";");
        });
    //} else {
    //    $('#divCBBDepartment').html(CTS380_BlankComboBox.replace('{BLANK_ID}', 'DepartmentCode'));
    //    $('#divCBBARRole').html(CTS380_BlankComboBox.replace('{BLANK_ID}', 'ARRoleCode'));
    //    $('#divCBBEmployee').html(CTS380_BlankComboBox.replace('{BLANK_ID}', 'EmployeeCode'));

    //    $('#DepartmentCode').attr('style', 'width: ' + cbbWidth + ";");
    //    $('#ARRoleCode').attr('style', 'width: ' + cbbWidth + ";");
    //    $('#EmployeeCode').attr('style', 'width: ' + cbbWidth + ";");
    //}
    //End Modify
}

function DepartmentCode_change() {
    var obj = {
        OfficeCode: $('#OfficeCode').val(),
        DepartmentCode: $('#DepartmentCode').val()
    };

    //Modify by jutarat A. on 20082012
    $('#divCBBARRole').html(CTS380_BlankComboBox.replace('{BLANK_ID}', 'ARRoleCode'));
    $('#divCBBEmployee').html(CTS380_BlankComboBox.replace('{BLANK_ID}', 'EmployeeCode'));

    //if ($('#DepartmentCode').val().length > 0) {
        call_ajax_method_json("/Contract/CTS380_DepartmentChange", obj, function (result, controls) {
            if (result != null) {
                $('#divCBBARRole').html(result.replace('{BLANK_ID}', 'ARRoleCode'));
                $('#ARRoleCode').change(ARRoleCode_change);
            }

            $('#divCBBEmployee').html(CTS380_BlankComboBox.replace('{BLANK_ID}', 'EmployeeCode'));

            $('#ARRoleCode').attr('style', 'width: ' + cbbWidth + ";");
            $('#EmployeeCode').attr('style', 'width: ' + cbbWidth + ";");
        });
    //} else {
    //    $('#divCBBARRole').html(CTS380_BlankComboBox.replace('{BLANK_ID}', 'ARRoleCode'));
    //    $('#divCBBEmployee').html(CTS380_BlankComboBox.replace('{BLANK_ID}', 'EmployeeCode'));

    //    $('#ARRoleCode').attr('style', 'width: ' + cbbWidth + ";");
    //    $('#EmployeeCode').attr('style', 'width: ' + cbbWidth + ";");
    //}
    //End Modify
}

function ARRoleCode_change() {
    var obj = {
        OfficeCode: $('#OfficeCode').val(),
        DepartmentCode: $('#DepartmentCode').val(),
        ARRoleCode: $('#ARRoleCode').val()
    };
    if ($('#ARRoleCode').val().length > 0) {
        call_ajax_method_json("/Contract/CTS380_ARRoleChange", obj, function (result, controls) {
            if (result != null) {
                $('#divCBBEmployee').html(result.replace('{BLANK_ID}', 'EmployeeCode'));

                $('#EmployeeCode').attr('style', 'width: ' + cbbWidth + ";");
            }
        });
    } else {
        $('#divCBBEmployee').html(CTS380_BlankComboBox.replace('{BLANK_ID}', 'EmployeeCode'));

        $('#EmployeeCode').attr('style', 'width: ' + cbbWidth + ";");
    }
}

function btnAddPIC_click() {
    VaridateCtrl(validatePIC, null);
    var tmpArr_Office = $('#OfficeCode option:selected').text().split(': ');
    var tmpArr_Role = $('#ARRoleCode option:selected').text().split(': ');
    var tmpArr_Department = $('#DepartmentCode option:selected').text().split(': ');
    var tmpArr_Emp = $('#EmployeeCode option:selected').text().split(': ');

    //Add by jutarat A. on 20082012
    if ($("#DepartmentCode").prop("disabled")) {
        tmpArr_Department = " ";
    }
    //End Add

    var obj = {
        OfficeCode: $('#OfficeCode').val(),
        OfficeName: tmpArr_Office[tmpArr_Office.length - 1],
        DepartmentCode: $('#DepartmentCode').val(),
        DepartmentName: tmpArr_Department[tmpArr_Department.length - 1],
        ARRoleCode: $('#ARRoleCode').val(),
        ARRoleName: tmpArr_Role[tmpArr_Role.length - 1],
        EmpNo: $('#EmployeeCode').val(),
        EmployeeName: tmpArr_Emp[tmpArr_Emp.length - 1],
        CanDelete: true
    };

    //if (($('#OfficeCode').val().length > 0) && ($('#DepartmentCode').val().length > 0) && ($('#ARRoleCode').val().length > 0) && ($('#EmployeeCode').val().length > 0))
    if (($('#OfficeCode').val().length > 0) && (($('#DepartmentCode').val().length > 0) || $('#DepartmentCode').prop("disabled")) && ($('#ARRoleCode').val().length > 0) && ($('#EmployeeCode').val().length > 0)) //Modify by jutarat A. on 20082012    
    {
        if (hasPIC(obj.EmpNo))
        {
            VaridateCtrl(validatePIC, ['EmployeeCode']);
            doAlert("Contract", "MSG3021", [$('#divlvlEmpName').html()]);
        } 
        //else if (hasPICRole(obj.ARRoleCode)) {
        else if (obj.ARRoleCode != _C_AR_ROLE_RECEIPIENT && hasPICRole(obj.ARRoleCode)) { //Modify by Jutarat A. on 30082012
            VaridateCtrl(validatePIC, ['ARRoleCode']);
            doAlert("Contract", "MSG3036", [$('#divlblARRole').html()]);
        }
        //Add by Jutarat A. on 30082012        
        else if (obj.ARRoleCode == _C_AR_ROLE_RECEIPIENT && hasMaxReceipient()) {
            VaridateCtrl(validatePIC, ["ARRoleCode"]);
            doAlert("Contract", "MSG3302", null);
        }
        //End Add 
        else {
            AddPIC(obj);

            AddNewRow(gridPIC, [obj.OfficeName
                , obj.OfficeCode
                , obj.DepartmentName
                , obj.DepartmentCode
                , obj.ARRoleCode
                , obj.ARRoleName
                , obj.EmployeeName
                , obj.EmpNo
                , ""
                , ""]);

            //gridPIC_binding(true);
            SetDefaultSendMailAfterAddData(); //Modify by Jutarat A. on 31082012

            var logObj = {
                EmpNo: obj.EmpNo,
                Action: ""
            };

            if (hasPICMOD(obj.EmpNo)) {
                var tmpLog = GetPICMOD(obj.EmpNo);
                if (tmpLog.Action == "3") {
                    // Old is Remove >>> Set to Modify
                    tmpLog.Action = "2";
                    RemovePICMOD(obj.EmpNo);
                    AddPICMOD(tmpLog);
                } else {
                    // Old is Modify >>> No Change
                }
            } else {
                // Add
                logObj.Action = "1"
                AddPICMOD(logObj);
            }

            SetPICToDefault(); //Add by jutarat A. on 20082012
        }
    } else {
        var targValidateCtrl = "";
        var targErrorCtrl = "";

        if ($('#OfficeCode').val().length == 0) {
            targValidateCtrl += "OfficeCode";
            targErrorCtrl = $('#divlblOffice').html();
        //} else if ($('#DepartmentCode').val().length == 0) {
        } else if ($('#DepartmentCode').val().length == 0 && $('#DepartmentCode').prop("disabled") == false) { //Modify by jutarat A. on 20082012
            targValidateCtrl += "DepartmentCode";
            targErrorCtrl = $('#divlblDepartment').html();
        } else if ($('#ARRoleCode').val().length == 0) {
            targValidateCtrl += "ARRoleCode";
            targErrorCtrl = $('#divlblARRole').html();
        } else if ($('#EmployeeCode').val().length == 0) {
            targValidateCtrl += "EmployeeCode";
            targErrorCtrl = $('#divlvlEmpName').html();
        }

        VaridateCtrl(validatePIC, [targValidateCtrl]);
        doAlert("Common", "MSG0007", [targErrorCtrl]);
    }
}

// ---------------------------------------------------------------------------------
// Method
// ---------------------------------------------------------------------------------

//Add by Jutarat A. on 31082012
function SetDefaultSendMailAfterAddData() {
    var isChckSendMail = false;
    var isEnbSendMail = true;

    var _colSendMail = gridPIC.getColIndexById("SendMail");
    var _colRemoveBtn = gridPIC.getColIndexById("RemoveBtn");
    var _colEmpNo = gridPIC.getColIndexById("EmpNo");

    if (gridPIC != null) {

        for (var i = 0; i < gridPIC.getRowsNum(); i++) {
            var row_id = gridPIC.getRowId(i);
            var picObj = GetPIC(gridPIC.cells(row_id, _colEmpNo).getValue());

            if (picObj != null) {
                if (picObj.CanDelete == true) {
                    GenerateRemoveButton(gridPIC, btnRemovePICID, row_id, "RemoveBtn", arDat.CanSpecialViewEdit || arDat.CanEdit);
                    if (arDat.CanSpecialViewEdit || arDat.CanEdit) {
                        BindGridButtonClickEvent(btnRemovePICID, row_id, RemovePIC_clicked);
                    }
                }

                if (i == (gridPIC.getRowsNum() - 1)) {
                    if ($('#ARStatusAfterUpdate').val() == _C_AR_STATUS_WAIT_FOR_APPROVAL) {
                        if (picObj.ARRoleCode == _C_AR_ROLE_AUDITOR) {
                            isChckSendMail = false;
                            isEnbSendMail = true;
                        }
                        else if (picObj.ARRoleCode == _C_AR_ROLE_RECEIPIENT) {
                            isChckSendMail = false;
                            isEnbSendMail = true;
                        }
                        else if (picObj.ARRoleCode == _C_AR_ROLE_APPROVER) {
                            isChckSendMail = true;
                            isEnbSendMail = false;
                        }
                    }
                    else if ($('#ARStatusAfterUpdate').val() == _C_AR_STATUS_AUDITING) {
                        if (picObj.ARRoleCode == _C_AR_ROLE_AUDITOR) {
                            isChckSendMail = true;
                            isEnbSendMail = false;
                        }
                        else if (picObj.ARRoleCode == _C_AR_ROLE_RECEIPIENT) {
                            isChckSendMail = false;
                            isEnbSendMail = true;
                        }
                        else if (picObj.ARRoleCode == _C_AR_ROLE_APPROVER) {
                            isChckSendMail = false;
                            isEnbSendMail = false;
                        }
                    }
                    else if ($('#ARStatusAfterUpdate').val() == _C_AR_STATUS_APPROVED
                            || $('#ARStatusAfterUpdate').val() == _C_AR_STATUS_REJECTED
                            || $('#ARStatusAfterUpdate').val() == _C_AR_STATUS_INSTRUCTED
                            || $('#ARStatusAfterUpdate').val() == _C_AR_STATUS_RETURNED_REQUEST) {
                        if (picObj.ARRoleCode == _C_AR_ROLE_AUDITOR) {
                            isChckSendMail = false;
                            isEnbSendMail = true;
                        }
                        else if (picObj.ARRoleCode == _C_AR_ROLE_RECEIPIENT) {
                            isChckSendMail = true;
                            isEnbSendMail = true;
                        }
                        else if (picObj.ARRoleCode == _C_AR_ROLE_APPROVER) {
                            isChckSendMail = false;
                            isEnbSendMail = false;
                        }
                    }

                    picObj.SendMail = isChckSendMail;
                    picObj.CanEditSendMail = isEnbSendMail;
                }
                else {
                    isChckSendMail = picObj.SendMail;
                    isEnbSendMail = picObj.CanEditSendMail;
                }

                gridPIC.cells(row_id, _colSendMail).setValue(GenerateCheckBox(_chkSendMailPICID, row_id, "", isEnbSendMail));

                var chkID = GenerateGridControlID(_chkSendMailPICID, row_id);
                var ctrl = $('#' + chkID);
                $(ctrl).attr('checked', isChckSendMail);

                $(ctrl).click(function () {
                    var rid = GetGridRowIDFromControl(this);
                    SendMailPIC_change(rid);
                });
            }

        }
    }

    gridPIC.setSizes();
}
//End Add

function BindDataToForm() {
    if (arDat != null) {
        SetScreenToDefault();

        $('#divARInfo').bindJSON(arDat.ARInfo);
        SetARRelevantType(arDat.ARInfo.ARRelevantType);

        //Modify by Jutarat A. on 28092012
//        if (arDat.ARInfo.ContractType == 1) {
//            // Sale
//            $('#divSaleContractInfo').bindJSON(arDat.SaleContract);
//            ShowSaleContract();
//        } else if (arDat.ARInfo.ContractType == 2) {
//            // Rental
//            $('#divContractInfo').bindJSON(arDat.RentalContract);
//            ShowRentalContract();
//        }
        if (arDat.ARInfo.ARRelevantType == _arType_Contract) {
            if (arDat.RentalContract != undefined && arDat.RentalContract != null) {
                // Rental
                $('#divContractInfo').bindJSON(arDat.RentalContract);

                $("#MonthlyContractFee").SetNumericCurrency(arDat.RentalContract.MonthlyContractFeeCurrencyType);
                if (arDat.RentalContract.MonthlyContractFeeCurrencyType == C_CURRENCY_US) {
                    $("#MonthlyContractFee").val(arDat.RentalContract.MonthlyContractFeeUsd);
                }
                else {
                    $("#MonthlyContractFee").val(arDat.RentalContract.MonthlyContractFee);
                }                

                ShowRentalContract();
            } else if (arDat.SaleContract != undefined && arDat.SaleContract != null) {
                // Sale
                $('#divSaleContractInfo').bindJSON(arDat.SaleContract);
                ShowSaleContract();
            }
        }
        //End Modify

//        SetDueDateDateTime(arDat.ARDetail.DueDateDeadlineType);
//        DueDate_DateTime_change();

        $('#divARDetail').bindJSON(arDat.ARDetail);

        $("#ContractQuotationFee").SetNumericCurrency(arDat.ARDetail.ContractQuotationFeeCurrencyType);
        if (arDat.ARDetail.ContractQuotationFeeCurrencyType == C_CURRENCY_US) {
            if (arDat.ARDetail.ContractQuotationFeeUsd != undefined) {
                $("#ContractQuotationFee").val(arDat.ARDetail.ContractQuotationFeeUsd.toFixed(2)).setComma();
            }
        }
        else {
            if (arDat.ARDetail.ContractQuotationFee != undefined) {
                $("#ContractQuotationFee").val(arDat.ARDetail.ContractQuotationFee.toFixed(2)).setComma();
            }
        }

        $("#ContractARFee").SetNumericCurrency(arDat.ARDetail.ContractARFeeCurrencyType);
        if (arDat.ARDetail.ContractARFeeCurrencyType == C_CURRENCY_US) {
            if (arDat.ARDetail.ContractARFeeUsd != undefined) {
                $("#ContractARFee").val(arDat.ARDetail.ContractARFeeUsd.toFixed(2)).setComma();
            }
        }
        else {
            if (arDat.ARDetail.ContractARFee != undefined) {
                $("#ContractARFee").val(arDat.ARDetail.ContractARFee.toFixed(2)).setComma();
            }
        }
        
        $("#DepositQuotationFee").SetNumericCurrency(arDat.ARDetail.DepositQuotationFeeCurrencyType);
        if (arDat.ARDetail.DepositQuotationFeeCurrencyType == C_CURRENCY_US) {
            if (arDat.ARDetail.DepositQuotationFeeUsd != undefined) {
                $("#DepositQuotationFee").val(arDat.ARDetail.DepositQuotationFeeUsd.toFixed(2)).setComma();
            }
        }
        else {
            if (arDat.ARDetail.DepositQuotationFee != undefined) {
                $("#DepositQuotationFee").val(arDat.ARDetail.DepositQuotationFee.toFixed(2)).setComma();
            }
        }

        $("#DepositARFee").SetNumericCurrency(arDat.ARDetail.DepositARFeeCurrencyType);
        if (arDat.ARDetail.DepositARFeeCurrencyType == C_CURRENCY_US) {
            if (arDat.ARDetail.DepositARFeeUsd != undefined) {
                $("#DepositARFee").val(arDat.ARDetail.DepositARFeeUsd.toFixed(2)).setComma();
            }
        }
        else {
            if (arDat.ARDetail.DepositARFee != undefined) {
                $("#DepositARFee").val(arDat.ARDetail.DepositARFee.toFixed(2)).setComma();
            }
        }

        $("#InstallQuotationFee").SetNumericCurrency(arDat.ARDetail.InstallQuotationFeeCurrencyType);
        if (arDat.ARDetail.InstallQuotationFeeCurrencyType == C_CURRENCY_US) {
            if (arDat.ARDetail.InstallQuotationFeeUsd != undefined) {
                $("#InstallQuotationFee").val(arDat.ARDetail.InstallQuotationFeeUsd.toFixed(2)).setComma();
            }
        }
        else {
            if (arDat.ARDetail.InstallQuotationFee != undefined) {
                $("#InstallQuotationFee").val(arDat.ARDetail.InstallQuotationFee.toFixed(2)).setComma();
            }
        }

        $("#InstallARFee").SetNumericCurrency(arDat.ARDetail.InstallARFeeCurrencyType);
        if (arDat.ARDetail.InstallARFeeCurrencyType == C_CURRENCY_US) {
            if (arDat.ARDetail.InstallARFeeUsd != undefined) {
                $("#InstallARFee").val(arDat.ARDetail.InstallARFeeUsd.toFixed(2)).setComma();
            }
        }
        else {
            if (arDat.ARDetail.InstallARFee != undefined) {
                $("#InstallARFee").val(arDat.ARDetail.InstallARFee.toFixed(2)).setComma();
            }
        }

        $("#ProductPriceQuotationFee").SetNumericCurrency(arDat.ARDetail.ProductPriceQuotationFeeCurrencyType);
        if (arDat.ARDetail.ProductPriceQuotationFeeCurrencyType == C_CURRENCY_US) {
            if (arDat.ARDetail.ProductPriceQuotationFeeUsd != undefined) {
                $("#ProductPriceQuotationFee").val(arDat.ARDetail.ProductPriceQuotationFeeUsd.toFixed(2)).setComma();
            }
        }
        else {
            if (arDat.ARDetail.ProductPriceQuotationFee != undefined) {
                $("#ProductPriceQuotationFee").val(arDat.ARDetail.ProductPriceQuotationFee.toFixed(2)).setComma();
            }
        }

        $("#InstallFeeQuotationFee").SetNumericCurrency(arDat.ARDetail.InstallFeeQuotationFeeCurrencyType);
        if (arDat.ARDetail.InstallFeeQuotationFeeCurrencyType == C_CURRENCY_US) {
            if (arDat.ARDetail.InstallFeeQuotationFeeUsd != undefined) {
                $("#InstallFeeQuotationFee").val(arDat.ARDetail.InstallFeeQuotationFeeUsd.toFixed(2)).setComma();
            }
        }
        else {
            if (arDat.ARDetail.InstallFeeQuotationFee != undefined) {
                $("#InstallFeeQuotationFee").val(arDat.ARDetail.InstallFeeQuotationFee.toFixed(2)).setComma();
            }
        }
       
        $("#ProductPriceARFee").SetNumericCurrency(arDat.ARDetail.ProductPriceARFeeCurrencyType);
        if (arDat.ARDetail.ProductPriceARFeeCurrencyType == C_CURRENCY_US) {
            if (arDat.ARDetail.ProductPriceARFeeUsd != undefined) {
                $("#ProductPriceARFee").val(arDat.ARDetail.ProductPriceARFeeUsd.toFixed(2)).setComma();
            }
        }
        else {
            if (arDat.ARDetail.ProductPriceARFee != undefined) {
                $("#ProductPriceARFee").val(arDat.ARDetail.ProductPriceARFee.toFixed(2)).setComma();
            }
        }

        $("#InstallFeeARFee").SetNumericCurrency(arDat.ARDetail.InstallFeeARFeeCurrencyType);
        if (arDat.ARDetail.InstallFeeARFeeCurrencyType == C_CURRENCY_US) {
            if (arDat.ARDetail.InstallFeeARFeeUsd != undefined) {
                $("#InstallFeeARFee").val(arDat.ARDetail.InstallFeeARFeeUsd.toFixed(2)).setComma();
            }
        }
        else {
            if (arDat.ARDetail.InstallFeeARFee != undefined) {
                $("#InstallFeeARFee").val(arDat.ARDetail.InstallFeeARFee.toFixed(2)).setComma();
            }
        }

        CalculateGridFee1();
        CalculateGridFee2();

//        $('#DueDate_Date').val(arDat.ARDetail.DueDate_Date);
//        $('#DueDate_Time').val(arDat.ARDetail.DueDate_Time);
//        $('#Deadline_Date').val(arDat.ARDetail.Deadline_Date);
//        $('#Deadline_Time').val(arDat.ARDetail.Deadline_Until);

        if (arDat.ARDetail.ImportantFlag)
        {
            $('#ImportantFlag').attr('checked', 'checked');
        } else
        {
            $('#ImportantFlag').removeAttr('checked');
        }

        if (arDat.ARDetail.DueDateDeadlineType == "1") {
            $('#divDeadLine').clearForm();
        } else {
            $('#divDueDate').clearForm();
        }

        historyDat = arDat.ARDetail.AuditDetailHistory;
        historyWithUpdateDat = arDat.ARDetail.AuditDetailHistoryWithUpdate;

        isViewHistory = true;
        btnHistory_click();

        for (var i = 0; i < arDat.ARRole.length; i++) {
            var tmpArr_Department = arDat.ARRole[i].DepartmentName.split(': ');

            AddPIC(arDat.ARRole[i]);
            CheckFirstRowIsEmpty(gridPIC, true);
            AddNewRow(gridPIC, [arDat.ARRole[i].OfficeName
                , arDat.ARRole[i].OfficeCode
                , tmpArr_Department[tmpArr_Department.length - 1]
                , arDat.ARRole[i].DepartmentCode
                , arDat.ARRole[i].ARRoleCode
                , arDat.ARRole[i].ARRoleName
                , arDat.ARRole[i].EmployeeName
                , arDat.ARRole[i].EmpNo
                , false
                , ""]);
        }

        gridPIC_binding(true);

        if ((!arDat.CanSpecialViewEdit && !arDat.CanEdit)
            || (arDat.CanSpecialViewEdit && (arDat.ARDetail.ARStatus == _C_AR_STATUS_APPROVED || arDat.ARDetail.ARStatus == _C_AR_STATUS_INSTRUCTED || arDat.ARDetail.ARStatus == _C_AR_STATUS_REJECTED))) { //Add by Jutarat A. on 23082012

            // Disable all control
//            $('#InteractionType').attr('disabled', 'true');
//            $('#ARStatusAfterUpdate').attr('disabled', 'true');
//            $('#CurrentRespondingDetail').attr('readonly', 'true');
//            $('#DueDate_Type').attr('disabled', 'true');
//            $('#ImportantFlag').attr('disabled', 'true');
//            $('#DueDate_Date').EnableDatePicker(false);
//            $('#DueDate_Date').EnableDatePicker(false);
//            $('#Deadline_Until').attr('disabled', 'true');
//            $('#DueDate_Time').attr('readonly', 'true');
//            $('#OfficeCode').attr('disabled', 'true');
//            $('#DepartmentCode').attr('disabled', 'true');
//            $('#ARRoleCode').attr('disabled', 'true');
//            $('#EmployeeCode').attr('disabled', 'true');
//            $('#btnAddPIC').attr('disabled', 'true');

            if (gridPIC != null) {
                var _colRemovePIC = gridPIC.getColIndexById("RemoveBtn");

                gridPIC.setColumnHidden(_colRemovePIC, true);
                gridPIC.setSizes();
            }

            if (gridAttach != null) {
                var _colRemoveAttach = gridAttach.getColIndexById("removeButton");
                var _colDownloadAttach = gridAttach.getColIndexById("downloadButton");

                gridAttach.setColumnHidden(_colRemoveAttach, true);
                //gridAttach.setColumnHidden(_colDownloadAttach, true);
                gridAttach.setSizes();
            }

            //Add by Jutarat A. on 23082012
            if (arDat.CanSpecialViewEdit && (arDat.ARDetail.ARStatus == _C_AR_STATUS_APPROVED || arDat.ARDetail.ARStatus == _C_AR_STATUS_INSTRUCTED || arDat.ARDetail.ARStatus == _C_AR_STATUS_REJECTED)) {
                $("#divARInfo").SetDisabled(true);
                $("#divContractInfo").SetDisabled(true);
                $("#divSaleContractInfo").SetDisabled(true);
                $("#divARDetail").SetDisabled(true);
//                $("#DueDate_Date").EnableDatePicker(false)

                $("#ARStatusAfterUpdate").attr("disabled", false);

                EnableRegisterCommandPane();

                $("#InteractionType").val(_C_AR_INTERACTION_TYPE_REGISTER_BY_ADMIN); //Add by Jutarat A. on 28082012
            }
            //End Add
            else {
                MakeViewOnly('#divARInfo');
                MakeViewOnly('#divContractInfo');
                MakeViewOnly('#divSaleContractInfo');
                MakeViewOnly('#divARDetail');
            }

            $('#divContractQuotationFee').attr('style', 'text-align: right;');
            $('#divContractARFee').attr('style', 'text-align: right;');
            $('#divDepositQuotationFee').attr('style', 'text-align: right;');
            $('#divDepositARFee').attr('style', 'text-align: right;');
            $('#divInstallQuotationFee').attr('style', 'text-align: right;');
            $('#divInstallARFee').attr('style', 'text-align: right;');

            $('#divProductPriceQuotationFee').attr('style', 'text-align: right;');
            $('#divInstallFeeQuotationFee').attr('style', 'text-align: right;');
            $('#divProductPriceARFee').attr('style', 'text-align: right;');
            $('#divInstallFeeARFee').attr('style', 'text-align: right;');
            $('#divSalePriceQuotationFee').attr('style', 'text-align: right;');
            $('#divSalePriceARFee').attr('style', 'text-align: right;');

            $('#btnHistory').show();
            $('#btnHistory').removeAttr('disabled');

            $('#divDueDateRemark').hide();
            $('#divAttachFrame').hide();
            $('#divAttachRemark').hide();
            //$('#divSelectPIC').hide();

            setTimeout(function () {
                $('#divSelectPIC').hide();
            }, 200);

        }
        else {

            //Add by Jutarat A. on 03042013
            if (arDat.ARDetail.ARStatus == _C_AR_STATUS_RETURNED_REQUEST) {
                $("#ContractQuotationFee").SetDisabled(!arDat.IsEnableAlarm);
                $("#ContractARFee").SetDisabled(!arDat.IsEnableAlarm);
                $("#InstallQuotationFee").SetDisabled(!arDat.IsEnableAlarm);
                $("#InstallARFee").SetDisabled(!arDat.IsEnableAlarm);
                $("#DepositQuotationFee").SetDisabled(!arDat.IsEnableAlarm);
                $("#DepositARFee").SetDisabled(!arDat.IsEnableAlarm);

                $("#ProductPriceQuotationFee").SetDisabled(!arDat.IsEnableSale);
                $("#ProductPriceARFee").SetDisabled(!arDat.IsEnableSale);
                $("#InstallFeeQuotationFee").SetDisabled(!arDat.IsEnableSale);
                $("#InstallFeeARFee").SetDisabled(!arDat.IsEnableSale);
            }
            //End Add

            if (arDat.CanModPIC) {
                $('#OfficeCode').removeAttr("disabled");
                $('#DepartmentCode').removeAttr("disabled");
                $('#ARRoleCode').removeAttr("disabled");
                $('#EmployeeCode').removeAttr("disabled");
                $('#btnAddPIC').removeAttr("disabled");
            } else {
                $('#OfficeCode').attr("disabled", "true");
                $('#DepartmentCode').attr("disabled", "true");
                $('#ARRoleCode').attr("disabled", "true");
                $('#EmployeeCode').attr("disabled", "true");
                $('#btnAddPIC').attr("disabled", "true");
            }

            EnableRegisterCommandPane();
            $('#frmAttach').unbind('load');
            $('#frmAttach').load(RefreshAttachList);
        }
    }
}

function SetScreenToDefault() {
    DisableAllCommandPane();
    SetViewModeForm(false);
    CloseWarningDialog();
    isViewHistory = true;

    // Clear form
    $('#divARInfo').clearForm();
    $('#divContractInfo').clearForm();
    $('#divSaleContractInfo').clearForm();
    $('#divARDetail').clearForm();

    $('#ARStatusAfterUpdate').attr('disabled', 'true');

//    // Reset date to default
//    SetDueDateDateTime(1);
//    DueDate_DateTime_change();

    // Clear grid dat
    picDat = new Array();
    if (gridPIC != null) {
        //gridPIC.clearAll();
        DeleteAllRow(gridPIC);
    }

    picLOGDAT = new Array();

    HideAll();
    CalculateGridFee1();
    CalculateGridFee2();
}

function SetPICToDefault() {
    $('#OfficeCode').val('');

    $('#divCBBDepartment').html(CTS380_BlankComboBox.replace('{BLANK_ID}', 'DepartmentCode'));
    $('#divCBBARRole').html(CTS380_BlankComboBox.replace('{BLANK_ID}', 'ARRoleCode'));
    $('#divCBBEmployee').html(CTS380_BlankComboBox.replace('{BLANK_ID}', 'EmployeeCode'));

    $('#DepartmentCode').attr('style', 'width: ' + CTS380_DefaultCBBWidth);
    $('#ARRoleCode').attr('style', 'width: ' + CTS380_DefaultCBBWidth);
    $('#EmployeeCode').attr('style', 'width: ' + CTS380_DefaultCBBWidth);
}

function SetViewModeForm(mode, isNeedReloadPIC) {
    if (gridPIC != null) {
        var _colRemovePIC = gridPIC.getColIndexById("RemoveBtn");

        gridPIC.setColumnHidden(_colRemovePIC, mode);
        gridPIC.setSizes();

        if (!mode)
        {
            SetFitColumnForBackAction(gridPIC, "TmpColumn");
        }
    }

    if (gridAttach != null) {
        if (mode) {
            var _colRemoveAttach = gridAttach.getColIndexById("removeButton");
            var _colDownloadAttach = gridAttach.getColIndexById("downloadButton");

            gridAttach.setColumnHidden(_colRemoveAttach, mode);
            //gridAttach.setColumnHidden(_colDownloadAttach, mode);
            gridAttach.setSizes();

            if (!mode) {
                SetFitColumnForBackAction(gridAttach, "TmpColumn");
            }
        } else {
            RefreshAttachList();
        }
    }

    $('#divARInfo').SetViewMode(mode);
    $('#divContractInfo').SetViewMode(mode);
    $('#divSaleContractInfo').SetViewMode(mode);
    $('#divARDetail').SetViewMode(mode);

    if (mode) {
        $('#divDueDateRemark').hide();
        $('#divAttachFrame').hide();
        $('#divAttachRemark').hide();
        //$('#divSelectPIC').hide();

        setTimeout(function () {
            $('#divSelectPIC').hide();
        }, 200);
    } else
    {
        $('#divDueDateRemark').show();
        $('#divAttachFrame').show();
        $('#divAttachRemark').show();
        $('#divSelectPIC').show();

        if (isNeedReloadPIC) {
            gridPIC_binding(true, true);
        }
    }

    $('#rdoCustomer').attr('disabled', 'disabled');
    $('#rdoSite').attr('disabled', 'disabled');
    $('#rdoQuotation').attr('disabled', 'disabled');
    $('#rdoProject').attr('disabled', 'disabled');
    $('#rdoContract').attr('disabled', 'disabled');

    $('#ImportantFlag').attr('disabled', 'disabled');

    $('#divContractQuotationFee').attr('style', 'text-align: right;');
    $('#divContractARFee').attr('style', 'text-align: right;');
    $('#divDepositQuotationFee').attr('style', 'text-align: right;');
    $('#divDepositARFee').attr('style', 'text-align: right;');
    $('#divInstallQuotationFee').attr('style', 'text-align: right;');
    $('#divInstallARFee').attr('style', 'text-align: right;');

    $('#divProductPriceQuotationFee').attr('style', 'text-align: right;');
    $('#divInstallFeeQuotationFee').attr('style', 'text-align: right;');
    $('#divProductPriceARFee').attr('style', 'text-align: right;');
    $('#divInstallFeeARFee').attr('style', 'text-align: right;');
    $('#divSalePriceQuotationFee').attr('style', 'text-align: right;');
    $('#divSalePriceARFee').attr('style', 'text-align: right;');
}

//function SetDueDateDateTime(val) {
//    var obj = $('input:radio[name=DueDateDeadLineType]');
//    obj.filter('[value=' + val + ']').attr('checked', true);
//}

function GetDueDateDateTime() {
    return $('input[name="DueDateDeadLineType"]:checked').val();
}

function SetARRelevantType(val) {
    var obj = $('input:radio[name=ARRelevantType]');
    obj.filter('[value=' + val + ']').attr('checked', true);
}

function GetARRelevantType() {
    return $('input[name="ARRelevantType"]:checked').val();
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

function doAlertWithMessage(msgCode, msgText) {
    hasAlert = true;
    alertMsg = msgText;
}

function doAskYesNo(moduleCode, msgCode, paramObj, yesFunc, noFunc) {
    var obj = {
        module: moduleCode,
        code: msgCode,
        param: paramObj
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        if ((yesFunc != null) && (typeof (yesFunc) != "function")) {
            yesFunc = null;
        }

        if ((noFunc != null) && (typeof (noFunc) != "function")) {
            noFunc = null;
        }

        OpenYesNoMessageDialog(result.Code, result.Message, yesFunc, noFunc);
    });
}

function hasPIC(empNo) {
    if ((picDat != null) && (picDat != undefined)) {
        for (var i = 0; i < picDat.length; i++) {
            if (picDat[i].EmpNo == empNo) {
                return true;
            }
        }
    }

    return false;
}

function hasPICRole(roleCode) {
    if ((picDat != null) && (picDat != undefined)) {
        for (var i = 0; i < picDat.length; i++) {
            if (picDat[i].ARRoleCode == roleCode) {
                return true;
            }
        }
    }

    return false;
}

//Add by Jutarat A. on 30082012
function hasMaxReceipient() {
    if ((picDat != null) && (picDat != undefined)) {
        var iCount = 0;
        for (var i = 0; i < picDat.length; i++) {
            if (picDat[i].ARRoleCode == _C_AR_ROLE_RECEIPIENT) {
                iCount++;
                if (iCount == _C_AR_MAXIMUM_RECEIPIENT) {
                    return true;
                }
            }
        }
    }

    return false;
}
//End Add

function GetPIC(empNo) {
    if ((picDat != null) && (picDat != undefined)) {
        for (var i = 0; i < picDat.length; i++) {
            if (picDat[i].EmpNo == empNo) {
                return picDat[i];
            }
        }
    }

    return null;
}

function AddPIC(obj) {
    if ((picDat == null) || (picDat == undefined)) {
        picDat = new Array();
    }

    picDat[picDat.length] = obj;
}

function UpdatePIC(obj) {
    for (var i = 0; i < picDat.length; i++) {
        if (picDat[i].EmpNo == obj.EmpNo) {
            picDat[i] = obj;
        }
    }
}

function RemovePIC(empNo) {
    for (var i = 0; i < picDat.length; i++) {
        if (picDat[i].EmpNo == empNo) {
            picDat.splice(i, 1);
        }
    }
}

function hasPICMOD(empNo) {
    if ((picLOGDAT != null) && (picLOGDAT != undefined)) {
        for (var i = 0; i < picLOGDAT.length; i++) {
            if (picLOGDAT[i].EmpNo == empNo) {
                return true;
            }
        }
    }

    return false;
}

function GetPICMOD(empNo) {
    if ((picLOGDAT != null) && (picLOGDAT != undefined)) {
        for (var i = 0; i < picLOGDAT.length; i++) {
            if (picLOGDAT[i].EmpNo == empNo) {
                return picLOGDAT[i];
            }
        }
    }

    return null;
}

function AddPICMOD(obj) {
    if ((picLOGDAT == null) || (picLOGDAT == undefined)) {
        picLOGDAT = new Array();
    }

    picLOGDAT[picLOGDAT.length] = obj;
}

function RemovePICMOD(empNo) {
    for (var i = 0; i < picLOGDAT.length; i++) {
        if (picLOGDAT[i].EmpNo == empNo) {
            picLOGDAT.splice(i, 1);
        }
    }
}

function CalculateGridFee1() {
    var arFee = 0;
    var quotationFee = 0;
    var outRate = 0;

    // Calculate Contract fee
    outRate = 0;
    if ($('#ContractQuotationFee').NumericCurrencyValue() == $('#ContractARFee').NumericCurrencyValue()) {
        quotationFee = parseFloat($('#ContractQuotationFee').val().replace(/ /g, "").replace(/,/g, ""));
        arFee = parseFloat($('#ContractARFee').val().replace(/ /g, "").replace(/,/g, ""));

        if ((quotationFee.length == 0) || isNaN(quotationFee))
            quotationFee = 0;

        if ((arFee.length == 0) || isNaN(arFee))
            arFee = 0;

        outRate = ((arFee - quotationFee) / quotationFee) * 100;

        if (isNaN(outRate))
            outRate = 0;
    }

    if (outRate < 0) {
        $('#spResContractFee').text(lblDC);
        $('#ContractOCDCRate').val(Math.abs(outRate).toFixed(2));
        $('#ContractOCDCRate').setComma();
    } else if (outRate > 0) {
        $('#spResContractFee').text(lblOR);
        $('#ContractOCDCRate').val(Math.abs(outRate).toFixed(2));
        $('#ContractOCDCRate').setComma();
    }
    else {
        $('#spResContractFee').text('-');
        $('#ContractOCDCRate').val('-');
    }

  

    // Calculate Deposit fee
    outRate = 0;
    if ($('#DepositQuotationFee').NumericCurrencyValue() == $('#DepositARFee').NumericCurrencyValue()) {
        quotationFee = parseFloat($('#DepositQuotationFee').val().replace(/ /g, "").replace(/,/g, ""));
        arFee = parseFloat($('#DepositARFee').val().replace(/ /g, "").replace(/,/g, ""));

        if ((quotationFee.length == 0) || isNaN(quotationFee))
            quotationFee = 0;

        if ((arFee.length == 0) || isNaN(arFee))
            arFee = 0;

        outRate = ((arFee - quotationFee) / quotationFee) * 100;

        if (isNaN(outRate))
            outRate = 0;
    }

    if (outRate < 0) {
        $('#spResDepositFee').text(lblDC);
        $('#DepositOCDCRate').val(Math.abs(outRate).toFixed(2));
        $('#DepositOCDCRate').setComma();
    } else if (outRate > 0) {
        $('#spResDepositFee').text(lblOR);
        $('#DepositOCDCRate').val(Math.abs(outRate).toFixed(2));
        $('#DepositOCDCRate').setComma();
    }
    else {
        $('#spResDepositFee').text('-');
        $('#DepositOCDCRate').val('-');
    }



    // Calculate Installation fee
    outRate = 0;
    if ($('#InstallQuotationFee').NumericCurrencyValue() == $('#InstallARFee').NumericCurrencyValue()) {
        quotationFee = parseFloat($('#InstallQuotationFee').val().replace(/ /g, "").replace(/,/g, ""));
        arFee = parseFloat($('#InstallARFee').val().replace(/ /g, "").replace(/,/g, ""));

        if ((quotationFee.length == 0) || isNaN(quotationFee))
            quotationFee = 0;

        if ((arFee.length == 0) || isNaN(arFee))
            arFee = 0;

        outRate = ((arFee - quotationFee) / quotationFee) * 100;

        if (isNaN(outRate))
            outRate = 0;
    }

    if (outRate < 0) {
        $('#spResInstallFee').text(lblDC);
        $('#InstallOCDCRate').val(Math.abs(outRate).toFixed(2));
        $('#InstallOCDCRate').setComma();
    } else if (outRate > 0) {
        $('#spResInstallFee').text(lblOR);
        $('#InstallOCDCRate').val(Math.abs(outRate).toFixed(2));
        $('#InstallOCDCRate').setComma();
    }
    else {
        $('#spResInstallFee').text('-');
        $('#InstallOCDCRate').val('-');
    }


}

function CalculateGridFee2() {
    var arFee = 0;
    var quotationFee = 0;
    var outRate = 0;

    var sumQuotation = 0;
    var sumAR = 0;

    // Calculate Product Price
    outRate = 0;
    if ($('#ProductPriceQuotationFee').NumericCurrencyValue() == $('#ProductPriceARFee').NumericCurrencyValue()) {
        quotationFee = parseFloat($('#ProductPriceQuotationFee').val().replace(/ /g, "").replace(/,/g, ""));
        arFee = parseFloat($('#ProductPriceARFee').val().replace(/ /g, "").replace(/,/g, ""));

        if ((quotationFee.length == 0) || isNaN(quotationFee))
            quotationFee = 0;

        if ((arFee.length == 0) || isNaN(arFee))
            arFee = 0;

        sumQuotation += quotationFee;
        sumAR += arFee;

        outRate = ((arFee - quotationFee) / quotationFee) * 100;

        if (isNaN(outRate))
            outRate = 0;
    }

    if (outRate < 0) {
        $('#spResProductPrice1').text(lblDC);
        $('#ProductPriceOCDCRate').val(Math.abs(outRate).toFixed(2));
        $('#ProductPriceOCDCRate').setComma();
    } else if (outRate > 0) {
        $('#spResProductPrice1').text(lblOR);
        $('#ProductPriceOCDCRate').val(Math.abs(outRate).toFixed(2));
        $('#ProductPriceOCDCRate').setComma();
    }
    else {
        $('#spResProductPrice1').text('-');
        $('#ProductPriceOCDCRate').val('-');
    }



    // Calculate Install Fee
    outRate = 0;
    if ($('#InstallFeeQuotationFee').NumericCurrencyValue() == $('#InstallFeeARFee').NumericCurrencyValue()) {
        quotationFee = parseFloat($('#InstallFeeQuotationFee').val().replace(/ /g, "").replace(/,/g, ""));
        arFee = parseFloat($('#InstallFeeARFee').val().replace(/ /g, "").replace(/,/g, ""));

        if ((quotationFee.length == 0) || isNaN(quotationFee))
            quotationFee = 0;

        if ((arFee.length == 0) || isNaN(arFee))
            arFee = 0;

        sumQuotation += quotationFee;
        sumAR += arFee;

        outRate = ((arFee - quotationFee) / quotationFee) * 100;

        if (isNaN(outRate))
            outRate = 0;
    }

    if (outRate < 0) {
        $('#spResInstallFee2').text(lblDC);
        $('#InstallFeeOCDCRate').val(Math.abs(outRate).toFixed(2));
        $('#InstallFeeOCDCRate').setComma();
    } else if (outRate > 0) {
        $('#spResInstallFee2').text(lblOR);
        $('#InstallFeeOCDCRate').val(Math.abs(outRate).toFixed(2));
        $('#InstallFeeOCDCRate').setComma();
    }
    else {
        $('#spResInstallFee2').text('-');
        $('#InstallFeeOCDCRate').val('-');
    }

    // Calculate Sale Price
    //$('#SalePriceQuotationFee').val(sumQuotation.toFixed(2));
    //$('#SalePriceQuotationFee').setComma();

    //$('#SalePriceARFee').val(sumAR.toFixed(2));
    //$('#SalePriceARFee').setComma();

    ////outRate = ((arFee - quotationFee) / quotationFee) * 100;
    //outRate = ((sumAR - sumQuotation) / sumQuotation) * 100; //Modify by Jutarat A. on 22022013

    //if (isNaN(outRate))
    //    outRate = 0;

    //if (outRate < 0) {
    //    $('#spResSalePrice12').text(lblDC);
    //} else if (outRate > 0) {
    //    $('#spResSalePrice12').text(lblOR);
    //}
    //else {
    //    $('#spResSalePrice12').text('-');
    //}

    //$('#SalePriceOCDCRate').val(Math.abs(outRate).toFixed(2));
    //$('#SalePriceOCDCRate').setComma();
}

function EnableRegisterCommandPane() {
    SetRegisterCommand(true, RegisterCommand_clicked);
    SetResetCommand(true, ResetCommand_clicked);
}

function EnableConfirmCommandPane() {
    SetConfirmCommand(true, ConfirmCommand_clicked);
    SetBackCommand(true, BackCommand_clicked);
}

function DisableRegisterCommandPane() {
    SetRegisterCommand(false, null);
    SetResetCommand(false, null);
}

function DisableConfirmCommandPane() {
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
}

function DisableAllCommandPane() {
    DisableRegisterCommandPane();
    DisableConfirmCommandPane();
}

function HideGrid1() {
    $('#divFeeGrid1').hide();
}

function HideGrid2() {
    $('#divFeeGrid2').hide();
}

function ShowGrid1() {
    $('#divFeeGrid1').show();
}

function ShowGrid2() {
    $('#divFeeGrid2').show();
}

function HideAll() {
    HideSaleContract();
    HideRentalContract();
}

function HideSaleContract() {
    $('#divSaleContractInfo').hide();
}

function HideRentalContract() {
    $('#divContractInfo').hide();
}

function ShowSaleContract() {
    $('#divSaleContractInfo').show();
}

function ShowRentalContract() {
    $('#divContractInfo').show();
}

function CreateFormObject() {
    var obj = {
        InteractionType: $('#InteractionType').val(),
        StatusAfterUpdate: $('#ARStatusAfterUpdate').val(),
        DueDate_DeadlineType: GetDueDateDateTime(),
        AuditDetail: $('#AuditDetail').val(),
//        DueDate_Date: $('#DueDate_Date').val(),
//        DueDate_Time: $('#DueDate_Time').val(),
//        Deadline_Date: $('#Deadline_Date').val(),
//        Deadline_Until: $('#Deadline_Until').val(),
        ARRole: picDat,
        HistoryList: picLOGDAT,
        OriginList: picDat,

        //Add by Jutarat A. on 03042013
        ContractFee_Quotation: $('#ContractQuotationFee').NumericValue(),
        ContractFee_AR: $('#ContractARFee').NumericValue(),
        Deposit_Quotation: $('#DepositQuotationFee').NumericValue(),
        Deposit_AR: $('#DepositARFee').NumericValue(),
        Installation_Quotation: $('#InstallQuotationFee').NumericValue(),
        Installation_AR: $('#InstallARFee').NumericValue(),

        ProductPrice_Quotation: $('#ProductPriceQuotationFee').NumericValue(),
        ProductPrice_AR: $('#ProductPriceARFee').NumericValue(),
        InstallFee_Quotation: $('#InstallFeeQuotationFee').NumericValue(),
        InstallFee_AR: $('#InstallFeeARFee').NumericValue()
        //End Add
    };

    return obj;
}

function MakeViewOnly(elementID) {
    $(elementID).find("input[type=text],input[type=checkbox],input[type=radio],textarea,button,select,span[class=label-remark],div[class=label-remark],a").each(function () {
        if ($(this).attr('class') == 'label-remark') {
            $(this).hide();
        }

        if ($(this).attr("id") == undefined) {
            var isctrl = true;
            var tag = this.tagName.toLowerCase();
            if ((tag == "input") || tag == "a") {
                isctrl = false;
            }

            //            if (isctrl) {
            //                if (isview) {
            //                    $(this).hide();
            //                }
            //                else {
            //                    $(this).show();
            //                }
            //            }
        } else {
            var div = "div" + $(this).attr("id");
            var tag = this.tagName.toLowerCase();
            var readonly = this.readOnly;

            if (tag == "input" && $(this).attr("type") == "checkbox") {
                $(this).attr("disabled", true);
            } else if (tag == "input" && $(this).attr("type") == "radio") {
                $(this).attr("disabled", true);
            } else {
                $(this).hide();
                var unitDate = $("#unitDate" + $(this).attr("id"));
                if (unitDate.length > 0) {
                    unitDate.hide();
                }

                if (tag == "input" || tag == "select" || tag == "textarea" || tag == "a") {
                    /* --- For Datetime Picker --- */
                    $(this).parent().find("img").hide();

                    var css = "label-view";
                    //                        if (readonly == false || readonly == undefined || editonly == true)
                    //                            css = "label-edit-view";

                    var val = $(this).val();
                    if (tag == "select") {
                        if (val != "") {
                            val = $(this).find(":selected").text();
                        }
                    }

                    //Add by Jutarat A. (15/02/2012) 
                    //For support link
                    if (tag == "a") {
                        css = "label-view";
                        val = $(this).text();
                    }
                    //End Add

                    if ($.trim(val) == "") {
                        val = "-";
                        //                        var unit = $("#unit" + $(this).attr("id"));
                        //                        if (unit.length > 0) {
                        //                            unit.hide();
                        //                        }
                    }

                    if (tag == "textarea") {
                        val = val.replace(/\n/g, "<br/>");
                    }

                    $(this).parent().find("#" + div).remove();
                    $(this).parent().append("<div id='" + div + "' class='" + css + "'>" + val + "</div>");
                }
            }
        }
    });
}