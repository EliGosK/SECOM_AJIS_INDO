
/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>

/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

//var CTS300_gridAttachDocList;
var CTS300_gridAssignPIC;
var CTS300_PICDat;
var CTS300_gridAttach;
var CTS300_ValidatePIC = ["BelongOfficeCode", "DepartmentCode", "IncidentRoleCode", "EmployeeCode"];
var CTS300_allctrlid = ["ReceivedDate", "ReceivedTime", "ContractName", "Department_Position", "IncidentTitle", "IncidentTypeCode", "ReasonTypeCode", "RecivedDetail", "DueDate", "ReceivedDueTime", "DeadLine", "DeadLineTimeType"];
var canAddPIC = false;
var hasChief = false;
var isInitAttachGrid = false;
var haveReasonType = false;
var cbbWidth = "260px;"
var CTS300_RemovePICBtnID = "btnRemovePIC";
var CTS300_RemoveAttachBtnID = "btnRemoveAttach";

var hasAlert = false;
var alertMsg = "";
var IsInit05 = false;

var IsInit05_Attach = false;
var IsInit05_PIC = false;


$(document).ready(function () {
    $('#frmAttach').attr('src', 'CTS300_Upload?k=' + _attach_k);
    $('#frmAttach').load(RefreshAttachList);

    $("#ReceivedDate").InitialDate();
    $("#DueDate").InitialDate();
    $("#DeadLine").InitialDate();

    //$('#DueDate').EnableDatePicker(false);
    $('#DeadLine').EnableDatePicker(false);

    $("#ReceivedTime").BindTimeBox();
    $("#ReceivedDueTime").BindTimeBox();
    CTS300_gridAttach = $("#CTS300_gridAttachDocList").InitialGrid(10, false, "/Contract/CTS300_IntialGridAttachedDocList", function () {
        SpecialGridControl(CTS300_gridAttach, ["removeButton"]);
        BindOnLoadedEvent(CTS300_gridAttach, CTS300_gridAttachBinding);
        IsInit05_Attach = true;
        MarkAsInit05();
    });
    CTS300_gridAssignPIC = $("#CTS300_gridAssignPIC").InitialGrid(10, false, "/Contract/CTS300_IntialGridAssignPIC", function () {
        SpecialGridControl(CTS300_gridAssignPIC, ["RemoveBtn"]);
        BindOnLoadedEvent(CTS300_gridAssignPIC, CTS300_gridAssignPICBinding);
        IsInit05_PIC = true;
        MarkAsInit05();
    });


    $('#IncidentTypeCode').change(IncidentTypeCode_changed);
    $('#BelongOfficeCode').change(BelongOfficeCode_changed);
    //    $('#DepartmentCode').change(DepartmentCode_changed);
    //    $('#IncidentRoleCode').change(IncidentRoleCode_changed);

    $('#rdoDeadLine').change(DueDate_DeadLine_changed);
    $('#rdoDueDate').change(DueDate_DeadLine_changed);

    $('#CTS300_btnAddPIC').click(CTS300_btnAddPIC_clicked);
});

function MarkAsInit05() {
    IsInit05 = IsInit05_PIC && IsInit05_Attach;
    LoadParameter();
}

function CTS300_gridAttachBinding(gen_ctrl) {
//    if (isInitAttachGrid) {
        if (!CheckFirstRowIsEmpty(CTS300_gridAttach, false)) {
            var _colRemoveBtn = CTS300_gridAttach.getColIndexById("removeButton");

            for (var i = 0; i < CTS300_gridAttach.getRowsNum(); i++) {
                var row_id = CTS300_gridAttach.getRowId(i);
                if (gen_ctrl == true) {
                    GenerateRemoveButton(CTS300_gridAttach, CTS300_RemoveAttachBtnID, row_id, "removeButton", true);
                }
                BindGridButtonClickEvent(CTS300_RemoveAttachBtnID, row_id, btnRemoveAttach_clicked);
            }

            //CTS300_gridAttach.setSizes();
        }
//    } else {
//        isInitAttachGrid = true;
//    }
}

function btnRemoveAttach_clicked(row_id) {
    var _colID = CTS300_gridAttach.getColIndexById("AttachFileID");
    var _targID = CTS300_gridAttach.cells(row_id, _colID).getValue();

    var obj = {
        AttachID: _targID
    };
    call_ajax_method_json("/Contract/CTS300_RemoveAttach", obj, function (result, controls) {
        if (result != null) {
            RefreshAttachList();
        }
    });
    
}

function RefreshAttachList() {
    if (CTS300_gridAttach != null) {
        $('#CTS300_gridAssignPIC').LoadDataToGrid(CTS300_gridAttach, 0, false, "/Contract/CTS300_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, function () {
            if (hasAlert) {
                hasAlert = false;
                OpenWarningDialog(alertMsg);
            }
        }, null)
    }
}

function hasPIC(empNo) {
    if ((CTS300_PICDat != null) && (CTS300_PICDat != undefined))
    {
        for (var i = 0; i < CTS300_PICDat.length; i++) {
            if (CTS300_PICDat[i].EmpNo == empNo) {
                return true;
            }
        }
    }

    return false;
}

function GetPIC(empNo) {
    if ((CTS300_PICDat != null) && (CTS300_PICDat != undefined)) {
        for (var i = 0; i < CTS300_PICDat.length; i++) {
            if (CTS300_PICDat[i].EmpNo == empNo) {
                return CTS300_PICDat[i];
            }
        }
    }

    return null;
}

function AddPIC(obj) {
    if ((CTS300_PICDat == null) || (CTS300_PICDat == undefined)) {
        CTS300_PICDat = new Array();
    }

    CTS300_PICDat[CTS300_PICDat.length] = obj;
}

function RemovePIC(empNo) {
    for (var i = 0; i < CTS300_PICDat.length; i++) {
        if (CTS300_PICDat[i].EmpNo == empNo) {
            CTS300_PICDat.splice(i, 1);
        }
    }
}

function IncidentTypeCode_changed() {
    var obj = {
        strIncidentType: $('#IncidentTypeCode').val()
    };

    // Old
//    call_ajax_method_json("/Contract/CTS300_RetrieveReceivedDetailPattern", obj, function (result, controls) {
//        if ((result != null) && (result.length > 0)) {
//            $('#RecivedDetail').val(result);
//        }
//    });

    call_ajax_method_json("/Contract/CTS300_RetrieveReceivedDetailPatternAndReason", obj, function (result, controls) {
        if ((result != null)) {
            haveReasonType = result.HaveReasonType;
            $('#RecivedDetail').val(result.RecievedDetail);
            if (result.ReasonType.length > 0) {
                $('#spanReasonTypeCode').html(result.ReasonType.replace('{BlankID}', 'ReasonTypeCode'));
                $('#ReasonTypeCode').removeAttr("disabled");
            } else {
                $('#spanReasonTypeCode').html(CTS300_defaultEmptyCombobox.replace('{BlankID}', 'ReasonTypeCode'));
                $('#ReasonTypeCode').attr("disabled", "disabled");
            }

            $('#ReasonTypeCode').attr('style', 'width:300px;');
            $('#ReasonTypeCode').val('');
        }
    });
}

function BelongOfficeCode_changed() {
    // Load Department combobox
    VaridateCtrl(CTS300_ValidatePIC, null);
    canAddPIC = false;
    var obj = {
        strOfficeCode: $('#BelongOfficeCode').val()
    };

    $('#divCBBDepartment').html(CTS300_defaultEmptyCombobox.replace("{BlankID}", "DepartmentCode"));
    $('#divCBBIncidentRole').html(CTS300_defaultBlankComboBox.replace("{BlankID}", "IncidentRoleCode"));
    $('#divCBBEmployee').html(CTS300_defaultBlankComboBox.replace("{BlankID}", "EmployeeCode"));

    call_ajax_method_json("/Contract/CTS300_RetrieveDepartmentComboBox", obj, function (result, controls) {
        if ((result != null) && (result.length > 0)) {
            $('#divCBBDepartment').html(result.replace("{BlankID}", "DepartmentCode"));
            $('#DepartmentCode').change(DepartmentCode_changed);
            $('#divCBBIncidentRole').html(CTS300_defaultBlankComboBox.replace("{BlankID}", "IncidentRoleCode"));
            $('#divCBBEmployee').html(CTS300_defaultBlankComboBox.replace("{BlankID}", "EmployeeCode"));
        }
        //        else {
        //            $('#divCBBDepartment').html(CTS300_defaultEmptyCombobox.replace("{BlankID}", "DepartmentCode"));
        //            $('#divCBBIncidentRole').html(CTS300_defaultBlankComboBox.replace("{BlankID}", "IncidentRoleCode"));
        //            $('#divCBBEmployee').html(CTS300_defaultBlankComboBox.replace("{BlankID}", "EmployeeCode"));
        //        }'

        //Add by jutarat A. on 20082012
        if ((result != null) && (result == false)) {
            $('#DepartmentCode').attr("disabled", true);
            DepartmentCode_changed();
        }
        //End Add

        $('#DepartmentCode').attr("style", "width: " + cbbWidth);
        $('#IncidentRoleCode').attr("style", "width: " + cbbWidth);
        $('#EmployeeCode').attr("style", "width: " + cbbWidth);
    });
}

function CTS300_gridAssignPICBinding(gen_ctrl) {
    var _colRemoveBtn = CTS300_gridAssignPIC.getColIndexById("RemoveBtn");

    for (var i = 0; i < CTS300_gridAssignPIC.getRowsNum(); i++) {
        var row_id = CTS300_gridAssignPIC.getRowId(i);
        //var ctrlID = GenerateGridControlID(CTS300_RemovePICBtnID, row_id);
        //var ctrlHtml = CTS300_defaultRemoveButton.replace("{BlankID}", ctrlID);
        //CTS300_gridAssignPIC.cells(row_id, _colRemoveBtn).setValue(ctrlHtml);

//        if (gen_ctrl == true) {
            GenerateRemoveButton(CTS300_gridAssignPIC, CTS300_RemovePICBtnID, row_id, "RemoveBtn", true);
//        }

        BindGridButtonClickEvent(CTS300_RemovePICBtnID, row_id, btnRemovePIC_clicked);
    }

    CTS300_gridAssignPIC.setSizes();

//    var ctrlInGrid = $("#CTS300_gridAssignPIC input:button");

//    ctrlInGrid.each(function () {
//        $(this).click(function () {
//            btnRemovePIC_clicked(GetGridRowIDFromControl(this));
//        });
//    });
}

function btnRemovePIC_clicked(rowID) {
    _colEmpNo = CTS300_gridAssignPIC.getColIndexById("EmpNo");

    if (hasChief) {
        if (GetPIC(CTS300_gridAssignPIC.cells(rowID, _colEmpNo).getValue()).IncidentRoleCode == _C_INCIDENT_ROLE_CONTROL_CHIEF) {
            hasChief = false;
        }
    }

    RemovePIC(CTS300_gridAssignPIC.cells(rowID, _colEmpNo).getValue());

    if (CTS300_gridAssignPIC.getRowsNum() == 1) {
        //CTS300_gridAssignPIC.clearAll();
        DeleteAllRow(CTS300_gridAssignPIC);
    } else {
        DeleteRow(CTS300_gridAssignPIC, rowID);
    }
}

function DepartmentCode_changed() {
    // Load Incident Role combobox
    VaridateCtrl(CTS300_ValidatePIC, null);
    canAddPIC = false;
    var obj = {
        strOfficeCode: $('#BelongOfficeCode').val(),
        strDepartmentCode: $('#DepartmentCode').val()
    };

    $('#divCBBIncidentRole').html(CTS300_defaultEmptyCombobox.replace("{BlankID}", "IncidentRoleCode"));
    $('#divCBBEmployee').html(CTS300_defaultBlankComboBox.replace("{BlankID}", "EmployeeCode"));

    call_ajax_method_json("/Contract/CTS300_RetrieveIncidentRoleComboBox", obj, function (result, controls) {
        if ((result != null) && (result.length > 0)) {
            $('#divCBBIncidentRole').html(result.replace("{BlankID}", "IncidentRoleCode"));
            $('#divCBBEmployee').html(CTS300_defaultBlankComboBox.replace("{BlankID}", "EmployeeCode"));
            $('#IncidentRoleCode').change(IncidentRoleCode_changed);
        }
//        else {
//            $('#divCBBIncidentRole').html(CTS300_defaultEmptyCombobox.replace("{BlankID}", "IncidentRoleCode"));
//            $('#divCBBEmployee').html(CTS300_defaultBlankComboBox.replace("{BlankID}", "EmployeeCode"));
//        }

        $('#IncidentRoleCode').attr("style", "width: " + cbbWidth);
        $('#EmployeeCode').attr("style", "width: " + cbbWidth);
    });
}

function IncidentRoleCode_changed() {
    // Load Employee combobox
    VaridateCtrl(CTS300_ValidatePIC, null);
    canAddPIC = true;
    var obj = {
        strOfficeCode: $('#BelongOfficeCode').val(),
        strDepartmentCode: $('#DepartmentCode').val(),
        strIncidentRoleCode: $('#IncidentRoleCode').val()
    };

    $('#divCBBEmployee').html(CTS300_defaultBlankComboBox.replace("{BlankID}", "EmployeeCode"));

    call_ajax_method_json("/Contract/CTS300_RetrieveEmployeeComboBox", obj, function (result, controls) {
        if ((result != null) && (result.length > 0)) {
            $('#divCBBEmployee').html(result.replace("{BlankID}", "EmployeeCode"));
        }
//        else {
//            $('#divCBBEmployee').html(CTS300_defaultEmptyCombobox.replace("{BlankID}", "EmployeeCode"));
//        }

        $('#EmployeeCode').attr("style", "width: " + cbbWidth);
    });
}

function DueDate_DeadLine_changed() {
    var curSelect = GetDueDate_DeadLine();

    $('#divDueDate').clearForm();
    $('#divDeadLine').clearForm();
    SetDueDate_DeadLine(curSelect);

    if (GetDueDate_DeadLine() == "1") {
        //$('#DeadLine').attr("disabled", true);
        $('#DeadLine').EnableDatePicker(false);
        $('#DeadLineTimeType').val('');
        $('#DeadLineTimeType').attr("disabled", true);
        //$('#DueDate').removeAttr('disabled');
        $('#DueDate').EnableDatePicker(true);
        $('#ReceivedDueTime').removeAttr('readonly');
    } else {
        $('#DueDate').EnableDatePicker(false);
        //$('#DueDate').attr("disabled", true);
        $('#ReceivedDueTime').val('');
        $('#ReceivedDueTime').attr('readonly', true);
        //$('#DeadLine').removeAttr('disabled');
        $('#DeadLine').EnableDatePicker(true);
        $('#DeadLineTimeType').removeAttr('disabled');
    }
}

function CTS300_btnAddPIC_clicked() {
    VaridateCtrl(CTS300_ValidatePIC, null);

    if (canAddPIC && ($('#EmployeeCode').val() != '')) {
        var isValidPerson = true;

        if ((CTS300_PICDat == null) || (CTS300_PICDat == undefined)) {
            CTS300_PICDat = new Array();
        }

        if (!hasChief && ($('#IncidentRoleCode').val() != _C_INCIDENT_ROLE_CONTROL_CHIEF)) {
            isValidPerson = false;
            doAlert("Contract", "MSG3275", null);
        } else {
            if (hasChief && ($('#IncidentRoleCode').val() == _C_INCIDENT_ROLE_CONTROL_CHIEF)) {
                VaridateCtrl(CTS300_ValidatePIC, ["IncidentRoleCode"]);
                doAlert("Contract", "MSG3020", null);
                isValidPerson = false;
            }
            else if (hasPIC($('#EmployeeCode').val())) {
                // Duplicate Employee
                isValidPerson = false;
                VaridateCtrl(CTS300_ValidatePIC, ["EmployeeCode"]);
                doAlert("Contract", "MSG3021", null);
            }
        }

        if (isValidPerson) {
            var obj = {
                OfficeCode: $('#BelongOfficeCode').val(),
                DepartmentCode: $('#DepartmentCode').val(),
                IncidentRoleCode: $('#IncidentRoleCode').val(),
                EmpNo: $('#EmployeeCode').val()
            };

            if (!hasChief)
            {
                hasChief = ($('#IncidentRoleCode').val() == _C_INCIDENT_ROLE_CONTROL_CHIEF);
            }

            var tmpArr_Office = $('#BelongOfficeCode option:selected').text().split(': ');
            var tmpArr_Incident = $('#IncidentRoleCode option:selected').text().split(': ');
            var tmpArr_Department = $('#DepartmentCode option:selected').text().split(': ');
            var tmpArr_Employee = $('#EmployeeCode option:selected').text().split(': ');

            //Add by jutarat A. on 20082012
            if ($("#DepartmentCode").prop("disabled")) {
                tmpArr_Department = " ";
            }
            //End Add

            CheckFirstRowIsEmpty(CTS300_gridAssignPIC, true);

            AddNewRow(CTS300_gridAssignPIC, [tmpArr_Office[tmpArr_Office.length - 1], tmpArr_Department[tmpArr_Department.length - 1], tmpArr_Incident[tmpArr_Incident.length - 1], tmpArr_Employee[tmpArr_Employee.length - 1], $('#EmployeeCode').val(), ""]);
            AddPIC(obj);

            ClearSection5_AssignPIC();
        }
    } else {
        if ($('#BelongOfficeCode').val().length == 0) {
            VaridateCtrl(CTS300_ValidatePIC, ["BelongOfficeCode"]);
            doAlert("Common", "MSG0007", _lblOffice);
        //} else if ($('#DepartmentCode').val().length == 0) {
        } else if ($('#DepartmentCode').val().length == 0 && $('#DepartmentCode').prop("disabled") == false) { //Modify by jutarat A. on 20082012
            VaridateCtrl(CTS300_ValidatePIC, ["DepartmentCode"]);
            doAlert("Common", "MSG0007", _lblDepartment);
        } else if ($('#IncidentRoleCode').val().length == 0) {
            VaridateCtrl(CTS300_ValidatePIC, ["IncidentRoleCode"]);
            doAlert("Common", "MSG0007", _lblIncidentRole);
        } else if ($('#EmployeeCode').val().length == 0) {
            VaridateCtrl(CTS300_ValidatePIC, ["EmployeeCode"]);
            doAlert("Common", "MSG0007", _lblEmployeeName);
        }
    }
}

function ClearSection5() {
    $("#divContractTargetPurchaserInfo").clearForm();
    $('#DeadLine').EnableDatePicker(false);
    $('#DueDate').EnableDatePicker(false);

    VaridateCtrl(CTS300_allctrlid, null);
    VaridateCtrl(CTS300_ValidatePIC, null);

    call_ajax_method_json("/Contract/CTS300_ClearAttach", "", function (result, controls) {
        if (result != null) {
            RefreshAttachList();
        }
    });

    ClearSection5_AssignPIC();

    SetReceiveMethod(1);
    SetDueDate_DeadLine(1);

    $('#DeadLine').EnableDatePicker(false);

    DueDate_DeadLine_changed();
    if (CTS300_gridAssignPIC != null) {
        //CTS300_gridAssignPIC.clearAll();
        DeleteAllRow(CTS300_gridAssignPIC);
    }
    CTS300_PICDat = null;
    hasChief = false;
    haveReasonType = false;
}

function ClearSection5_AssignPIC() {
    $('#BelongOfficeCode').val('');
    $('#divCBBDepartment').html(CTS300_defaultBlankComboBox.replace("{BlankID}", "DepartmentCode"));
    $('#divCBBIncidentRole').html(CTS300_defaultBlankComboBox.replace("{BlankID}", "IncidentRoleCode"));
    $('#divCBBEmployee').html(CTS300_defaultBlankComboBox.replace("{BlankID}", "EmployeeCode"));

    $('#DepartmentCode').attr("style", "width: " + cbbWidth);
    $('#IncidentRoleCode').attr("style", "width: " + cbbWidth);
    $('#EmployeeCode').attr("style", "width: " + cbbWidth);

    canAddPIC = false;
}

function SetViewMode_05(viewMode) {
    if (CTS300_gridAssignPIC != null) {
        var _colRemove = CTS300_gridAssignPIC.getColIndexById("RemoveBtn");

        CTS300_gridAssignPIC.setColumnHidden(_colRemove, viewMode);
        CTS300_gridAssignPIC.setSizes();

        if (!viewMode) {
            SetFitColumnForBackAction(CTS300_gridAssignPIC, "TmpColumn");
        }
    }

    if (CTS300_gridAttach != null) {
        var _colRemove = CTS300_gridAttach.getColIndexById("removeButton");

        CTS300_gridAttach.setColumnHidden(_colRemove, viewMode);
        CTS300_gridAttach.setSizes();

        if (!viewMode) {
            SetFitColumnForBackAction(CTS300_gridAttach, "TmpColumn");
        }
    }

    $('#divContractTargetPurchaserInfo').SetViewMode(viewMode);

    if (viewMode) {
        $('#divDueDateRemark').hide();
        $('#divAttachRemark').hide();
        $('#divSelectPIC').hide();
        $('#divAttachFrame').hide();
    } else {
        $('#divDueDateRemark').show();
        $('#divAttachRemark').show();
        $('#divSelectPIC').show();
        $('#divAttachFrame').show();
    }
}

function SetDefault_05() {
    ClearSection5();
    SetShow_05(false);
}

function SetShow_05(isShow) {
    if (isShow) {
        $('#divContractTargetPurchaserInfo').show();
    } else {
        $('#divContractTargetPurchaserInfo').hide();
    }

    SetShowInCharge_05(isShow);
    if (CTS300_gridAttach != null) {
        RefreshAttachList();
    }
}

function SetShowInCharge_05(isShow) {
    if (isShow) {
        $('#divAssignPIC').show();
    } else {
        $('#divAssignPIC').hide();
    }
}

function GetReceiveMethod() {
    return $('input[name="ReceiveMethod"]:checked').val();
}

function SetReceiveMethod(setval) {
    var obj = $('input:radio[name=ReceiveMethod]');
    obj.filter('[value=' + setval + ']').attr('checked', true);
}

function GetDueDate_DeadLine() {
    return $('input[name="DueDate_DeadLine"]:checked').val();
}

function SetDueDate_DeadLine(setval) {
    var obj = $('input:radio[name=DueDate_DeadLine]');
    obj.filter('[value=' + setval + ']').attr('checked', true);
}

function GetFormDataObject() {
    var obj = {
        ReceivedDate: $('#ReceivedDate').val(),
        ReceivedTime: $('#ReceivedTime').val(),
        ReceivedMethod: GetReceiveMethod(),
        ContactName: $('#ContractName').val(),
        Department: $('#Department_Position').val(),
        IncidentTitle: $('#IncidentTitle').val(),
        IncidentType: $('#IncidentTypeCode').val(),
        ReasonType: $('#ReasonTypeCode').val(),
        IsSpecialInfo: $('#chkConfidential').is(':checked'),
        IsImportance: $('#chkImportance').is(':checked'),
        ReceivedDetail: $('#RecivedDetail').val(),
        DueDateDeadLineType: GetDueDate_DeadLine(),
        DueDate_Date: $('#DueDate').val(),
        DueDate_Time: $('#ReceivedDueTime').val(),
        Deadline_Date: $('#DeadLine').val(),
        Deadline_Until: $('#DeadLineTimeType').val(),
        InChargeList: CTS300_PICDat,
        HaveReasonType: haveReasonType
    };

    // Fix null datetime
    if (obj.ReceivedDate.length == 0) {
        obj.ReceivedDate = null;
    }

    if (obj.ReceivedTime.length == 0) {
        obj.ReceivedTime = null;
    }

    if (obj.DueDate_Date.length == 0) {
        obj.DueDate_Date = null;
    }

    if (obj.DueDate_Time.length == 0) {
        obj.DueDate_Time = null;
    }

    if (obj.Deadline_Date.length == 0) {
        obj.Deadline_Date = null;
    }

    return obj;
}

function ValidateSection_05(ctrl_to_validate) {
    VaridateCtrl(CTS300_allctrlid, ctrl_to_validate);
}