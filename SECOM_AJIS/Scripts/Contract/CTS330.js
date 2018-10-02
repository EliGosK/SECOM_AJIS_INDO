// ---------------------------------------------------------------------------------
// Initial
// ---------------------------------------------------------------------------------
var pageRow = 5;

var CTS330_PICGrid = null;
var CTS330_PICDat = null;
var CTS330_PICMod = null;
var gridAttach = null;
var cbbWidth = "260px;"
var currHistoryView = 1;
var CTS330_RemovePICBtnID = "btnRemovePIC";
var hasChief = false;
var btnRemoveAttachID = "btnRemoveAttach";
var btnDownloadAttachID = "btnDownloadAttach";

var CTS330_ValidatePIC = ["BelongOfficeCode", "DepartmentCode", "IncidentRoleCode", "EmployeeCode"];
var CTS330_ValidateForm = ["InteractionType", "DueDate_Date", "DueDate_Time", "Deadline_Date", "Deadline_Until"];

var currentIncidentID = null;
var currentInficentData = null;

var isLoadOfficeCBB = false;
var isLoadInteractCBB = false;
var isFirstInitGrid = true;
var isInitAttachGrid = false;
var enablePIC = false;
var isInitGrid = false;

var isInitPICGrid = false

var hasAlert = false;
var alertMsg = "";

var tmpViewMode;

$(document).ready(function () {
//    HideAll();
//    HideIncidentDetail();

    InitialControl();
    InitialScreen();

    InitGrid();

    LoadOfficeComboBox();
    LoadInteractionComboBox();

//    LoadObjData();

//    setTimeout("RefreshAttachList();", 4000);
//    setTimeout(function () { $('#Confidential').attr('disabled', true); }, 3000);
//    setTimeout(function () { $('#Importance').attr('disabled', true); }, 3000);
//    setTimeout(function () {
//    if (GetDueDate_DeadLine() == "1") {
//            $('#Deadline_Date').EnableDatePicker(false);        
//            $('#Deadline_Until').attr("disabled", true);
//            $('#DueDate_Date').EnableDatePicker(true);
//            $('#DueDate_Time').removeAttr('readonly');
//        } else {
//            $('#DueDate_Date').EnableDatePicker(false);        
//            $('#DueDate_Time').attr('readonly', true);
//            $('#Deadline_Date').EnableDatePicker(true);
//            $('#Deadline_Until').removeAttr('disabled');
//        }
//    }, 3000);

    
});

function InitialScreen() {
//    $('#frmAttach').attr('src', 'CTS330_Upload');
//    $('#frmAttach').load(RefreshAttachList);
    $('#divIncidentInfo').hide();
    $('#divContractInfo').hide();
    $('#divSaleContractInfo').hide();
    $('#divIncidentDetail').hide();

    $("#MonthlyContractFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
}

function InitialControl() {
    $("#Deadline_Date").InitialDate();
    $("#DueDate_Date").InitialDate();
    $("#DueDate_Time").BindTimeBox();

    $('#btnAddPIC').click(btnAddPIC_clicked);
    $('#btnHistory').click(btnHistory_clicked);

    $('#DueDate_Type').change(DueDate_DeadLine_changed);
    $('#Deadline_Type').change(DueDate_DeadLine_changed);


}

function InitGrid() {
    //CTS330_PICGrid = $("#CTS330_gridAssignPIC").LoadDataToGridWithInitial(pageRow, false, false, "/Contract/CTS330_InitialAssignPersonInChargeGrid", "", "CTS330_PersonIncharge", false);
    CTS330_PICGrid = $("#CTS330_gridAssignPIC").LoadDataToGridWithInitial(0, false, false, "/Contract/CTS330_InitialAssignPersonInChargeGrid", "", "CTS330_PersonIncharge", false, null,
        //Add by Jutarat A. on 21082012
        function (result) {
            ajax_method.CallScreenController("/Contract/CTS330_LoadIncidentData", "", function (result, controls) {
                if (result != null) {
                    currentInficentData = result;
                }
            }, false);
        }
        //End Add
        , function () { isInitGrid = true; } //Add by Jutarat A. on 09012013
    );

    if (!isInitAttachGrid) {
        //Modify by Jutarat A. on 09012014
        /*gridAttach = $("#CTS330_gridAttachDocList").InitialGrid(10, false, "/Contract/CTS330_IntialGridAttachedDocList");

        SpecialGridControl(gridAttach, ["removeButton", "downloadButton"]);
        BindOnLoadedEvent(gridAttach, gridAttach_binding);
        gridAttach_binding(true);
        isInitGrid = true;
        LoadObjData();*/
        gridAttach = $("#CTS330_gridAttachDocList").LoadDataToGridWithInitial(0, false, false, "/Contract/CTS330_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, false, null,
                    function () {
                        isInitAttachGrid = true;
                        $('#frmAttach').attr('src', 'CTS330_Upload?sK=' + param + '&k=' + _attach_k);
                        $('#frmAttach').load(RefreshAttachList);

                        if (hasAlert) {
                            hasAlert = false;
                            OpenWarningDialog(alertMsg);
                        }
                        else {
                            if (tmpViewMode) {
                                if (gridAttach != null) {
                                    var _colRemoveAttach = gridAttach.getColIndexById("removeButton");
                                    var _colDownloadAttach = gridAttach.getColIndexById("downloadButton");

                                    gridAttach.setColumnHidden(_colRemoveAttach, true);
                                    gridAttach.setSizes();
                                }
                            }
                        }
                    });
        //End Modify
    }

    SpecialGridControl(CTS330_PICGrid, ["RemoveBtn"]);
    BindOnLoadedEvent(CTS330_PICGrid, CTS330_PICGrid_binding);

    //Add by Jutarat A. on 09012013
    SpecialGridControl(gridAttach, ["removeButton", "downloadButton"]); 
    BindOnLoadedEvent(gridAttach, gridAttach_binding);
    LoadObjData();
    //End Add
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

            if (gen_ctrl == true) {
                GenerateRemoveButton(gridAttach, btnRemoveAttachID, row_id, "removeButton", true);
                GenerateDownloadButton(gridAttach, btnDownloadAttachID, row_id, "downloadButton", true);
            }

            BindGridButtonClickEvent(btnRemoveAttachID, row_id, btnRemoveAttach_clicked);
            BindGridButtonClickEvent(btnDownloadAttachID, row_id, btnDownloadAttach_clicked);
        }
    }
    //Comment by Jutarat A. on 09012014
    //else {
      //  isInitAttachGrid = true;
      //  $('#frmAttach').attr('src', 'CTS330_Upload?sK=' + param + '&k=' + _attach_k);
      //  $('#frmAttach').load(RefreshAttachList);
    //}
    //End Comment

    gridAttach.setSizes();
}

function btnDownloadAttach_clicked(row_id) {
    var _colID = gridAttach.getColIndexById("AttachFileID");
    var _targID = gridAttach.cells(row_id, _colID).getValue();

    var obj = {
        AttachID: _targID
    };

    //Modify by Jutarat A. on 31012013
//    var key = ajax_method.GetKeyURL(null);
//    var link = ajax_method.GenerateURL("/contract/CTS330_DownloadAttach" + "?AttachID=" + _targID + "&k=" + key);
//    //window.location.href(link);
//    window.open(link, "download");
    download_method.CallDownloadController("ifDownload", "/Contract/CTS330_DownloadAttach", obj);
    //End Modify


//    call_ajax_method_json("/Contract/CTS330_DownloadAttach", obj, function (result, controls) {
//        if (result != null) {
//            RefreshAttachList();
//        }
//    }); 
}

function btnRemoveAttach_clicked(row_id) {
    var _colID = gridAttach.getColIndexById("AttachFileID");
    var _targID = gridAttach.cells(row_id, _colID).getValue();

    var obj = {
        AttachID: _targID
    };
    call_ajax_method_json("/Contract/CTS330_RemoveAttach", obj, function (result, controls) {
        if (result != null) {
            RefreshAttachList();
        }
    });

}

function RefreshAttachList() {
    if (gridAttach != null) {
        $('#CTS330_gridAttachDocList').LoadDataToGrid(gridAttach, 0, false, "/Contract/CTS330_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, function () {
            if (hasAlert) {
                hasAlert = false;
                OpenWarningDialog(alertMsg);
            }
            else {
                if (tmpViewMode) {
                    setTimeout(function () {
                        if (gridAttach != null) {
                            var _colRemoveAttach = gridAttach.getColIndexById("removeButton");
                            var _colDownloadAttach = gridAttach.getColIndexById("downloadButton");

                            gridAttach.setColumnHidden(_colRemoveAttach, true);
                            //gridAttach.setColumnHidden(_colDownloadAttach, true);
                            gridAttach.setSizes();
                        }
                    }, 1000);
                }
            }
        }, null);
    }
}

function InteractionType_changed() {
    var obj = {
        strInteractionTypeCode: $('#InteractionType').val()
    };
    if ($('#InteractionType').val() == _C_INTERACTION_TYPE_ADMINISTRATOR) {
        $('#IncidentStatusAfterUpdate').attr("disabled", false);
    }
    else {
        $('#IncidentStatusAfterUpdate').attr("disabled", true);
    }
    call_ajax_method_json("/Contract/CTS330_LoadSelectStatusCombobox", obj, function (result, controls) {
        if ((result != null) && (result.length > 0)) {
            $('#IncidentStatusAfterUpdate').val(result);
        }
    });
}

function BelongOfficeCode_changed() {
    var obj = {
        strOfficeCode: $('#BelongOfficeCode').val()
    };

    //Modify by jutarat A. on 20082012
    $('#divCBBDepartment').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "DepartmentCode"));
    $('#divCBBIncidentRole').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "IncidentRoleCode"));
    $('#divCBBEmployee').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "EmployeeCode"));

    //if (obj.strOfficeCode.length > 0) {
        call_ajax_method_json("/Contract/CTS330_LoadDepartmentCombobox", obj, function (result, controls) {
            if ((result != null) && (result.length > 0)) {
                $('#divCBBDepartment').html(result.replace("{BlankID}", "DepartmentCode"));
                $('#DepartmentCode').change(DepartmentCode_changed);
            } else {
                $('#divCBBDepartment').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "DepartmentCode"));
            }

            //Add by jutarat A. on 20082012
            if ((result != null) && (result == false)) {
                $('#DepartmentCode').attr("disabled", true);
                DepartmentCode_changed();
            }
            //End Add

            $('#divCBBIncidentRole').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "IncidentRoleCode"));
            $('#divCBBEmployee').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "EmployeeCode"));

            $('#DepartmentCode').attr("style", "width: " + cbbWidth);
            $('#IncidentRoleCode').attr("style", "width: " + cbbWidth);
            $('#EmployeeCode').attr("style", "width: " + cbbWidth);
        });
    //} else {
    //    $('#divCBBDepartment').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "DepartmentCode"));
    //    $('#divCBBIncidentRole').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "IncidentRoleCode"));
    //    $('#divCBBEmployee').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "EmployeeCode"));
    //}
    //End Modify
}

function DepartmentCode_changed() {
    var obj = {
        strOfficeCode: $('#BelongOfficeCode').val(),
        strDepartmentCode: $('#DepartmentCode').val()
    };

    //Modify by jutarat A. on 20082012
    $('#divCBBIncidentRole').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "IncidentRoleCode"));
    $('#divCBBEmployee').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "EmployeeCode"));
    
    //if (obj.strDepartmentCode.length > 0) {
        call_ajax_method_json("/Contract/CTS330_LoadIncidentRoleCombobox", obj, function (result, controls) {
            if ((result != null) && (result.length > 0)) {
                $('#divCBBIncidentRole').html(result.replace("{BlankID}", "IncidentRoleCode"));
                $('#IncidentRoleCode').change(IncidentRoleCode_changed);
            } else {
                $('#divCBBIncidentRole').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "IncidentRoleCode"));
            }

            $('#divCBBEmployee').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "EmployeeCode"));

            $('#IncidentRoleCode').attr("style", "width: " + cbbWidth);
            $('#EmployeeCode').attr("style", "width: " + cbbWidth);
        });
    //} else {
    //    $('#divCBBIncidentRole').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "IncidentRoleCode"));
    //    $('#divCBBEmployee').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "EmployeeCode"));
    //}
    //End Modify
}

function IncidentRoleCode_changed() {
    var obj = {
        strOfficeCode: $('#BelongOfficeCode').val(),
        strDepartmentCode: $('#DepartmentCode').val(),
        strIncidentRoleCode: $('#IncidentRoleCode').val()
    };

    //Modify by jutarat A. on 20082012
    $('#divCBBEmployee').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "EmployeeCode"));

    //if (obj.strDepartmentCode.length > 0) {
        call_ajax_method_json("/Contract/CTS330_LoadEmployeeCombobox", obj, function (result, controls) {
            if ((result != null) && (result.length > 0)) {
                $('#divCBBEmployee').html(result.replace("{BlankID}", "EmployeeCode"));
            } else {
                $('#divCBBEmployee').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "EmployeeCode"));
            }

            $('#EmployeeCode').attr("style", "width: " + cbbWidth);
        });
    //} else {
    //    $('#divCBBEmployee').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "EmployeeCode"));
    //}
    //End Modify
}

function DueDate_DeadLine_changed() {
    var curSelect = GetDueDate_DeadLine();

    $('#divDueDate').clearForm();
    $('#divDeadLine').clearForm();
    SetDueDate_DeadLine(curSelect);

    if (GetDueDate_DeadLine() == "1") {
        $('#Deadline_Date').EnableDatePicker(false);
        $('#Deadline_Until').val('');
        $('#Deadline_Until').attr("disabled", true);
        $('#DueDate_Date').EnableDatePicker(true);
        $('#DueDate_Time').removeAttr('readonly');
    } else {
        $('#DueDate_Date').EnableDatePicker(false);
        $('#DueDate_Time').val('');
        $('#DueDate_Time').attr('readonly', true);
        $('#Deadline_Date').EnableDatePicker(true);
        $('#Deadline_Until').removeAttr('disabled');
    }
}

function btnHistory_clicked() {
    if (currHistoryView == 1) {
        // Change to view all mode
        currHistoryView = 2;
        $('#divRespondingProgress').html(currentInficentData.RespondingProgress_All);
    } else {
        // Change back to normal mode
        currHistoryView = 1;
        $('#divRespondingProgress').html(currentInficentData.RespondingProgress_Normal);
    }
}

function btnAddPIC_clicked() {
    VaridateCtrl(CTS330_ValidatePIC, null);
    var isValid = true;



    var obj = {
        OfficeCode: $('#BelongOfficeCode').val(),
        DepartmentCode: $('#DepartmentCode').val(),
        IncidentRoleCode: $('#IncidentRoleCode').val(),
        EmpNo: $('#EmployeeCode').val()
    }

    if (obj.OfficeCode.length == 0) {
        isValid = false;
        VaridateCtrl(CTS330_ValidatePIC, ["BelongOfficeCode"]);
        doAlert("Common", "MSG0007", _lblOffice);
    //} else if (obj.DepartmentCode.length == 0) {
    } else if (obj.DepartmentCode.length == 0 && $('#DepartmentCode').prop("disabled") == false) { //Modify by jutarat A. on 20082012
        isValid = false;
        VaridateCtrl(CTS330_ValidatePIC, ["DepartmentCode"]);
        doAlert("Common", "MSG0007", _lblDepartment);
    } else if (obj.IncidentRoleCode.length == 0) {
        isValid = false;
        VaridateCtrl(CTS330_ValidatePIC, ["IncidentRoleCode"]);
        doAlert("Common", "MSG0007", _lblIncidentRole);
    } else if (obj.EmpNo.length == 0) {
        isValid = false;
        VaridateCtrl(CTS330_ValidatePIC, ["EmployeeCode"]);
        doAlert("Common", "MSG0007", _lblEmployeeName);
    } else {
        if (!hasChief && (obj.IncidentRoleCode != _C_INCIDENT_ROLE_CONTROL_CHIEF)) {
                isValid = false;
                doAlert("Contract", "MSG3275", null);
        } else {
            if (hasChief && (obj.IncidentRoleCode == _C_INCIDENT_ROLE_CONTROL_CHIEF)) {
                isValid = false;
                VaridateCtrl(CTS330_ValidatePIC, ["IncidentRoleCode"]);
                doAlert("Contract", "MSG3020", null);
            } else if (hasPIC(obj.EmpNo)) {
                isValid = false;
                VaridateCtrl(CTS330_ValidatePIC, ["EmployeeCode"]);
                doAlert("Contract", "MSG3021", null);
            }
        }
    }

    if (isValid) {

        AddPIC(obj);
        if (hasPICMOD(obj.EmpNo)) {
            var tmpObj = GetPICMOD(obj.EmpNo);
            if (tmpObj.Flag == "3") { // It is Remove Flag
                tmpObj.Flag = "2" // Set to Edit

                UpdatePICMOD(tmpObj);
            }
        } else {
            var obj = {
                EmpNo: obj.EmpNo,
                Flag: "1", //Add
                OfficeCode: $('#BelongOfficeCode').val(),
                DepartmentCode: $('#DepartmentCode').val(),
                IncidentRoleCode: $('#IncidentRoleCode').val()
            };
            AddPICMOD(obj);
        }

        if (!hasChief) {
            if ($('#IncidentRoleCode').val() == _C_INCIDENT_ROLE_CONTROL_CHIEF) {
                hasChief = true;
            }
        }

        
        var TmpBelongOfficeText = $('#BelongOfficeCode option:selected').text().split(":");
        var TmpDepartmentText = $('#DepartmentCode option:selected').text().split(":");
        var TmpIncidentRoleText = $('#IncidentRoleCode option:selected').text().split(":");
        var TmpEmpText = $('#EmployeeCode option:selected').text().split(":");

        var BelongOfficeText = "";
        if (TmpBelongOfficeText.length = 2) {
            BelongOfficeText = TmpBelongOfficeText[1];
        }
        var DepText = "";
        if (TmpDepartmentText.length = 2) {
            DepText = TmpDepartmentText[1];
        }
        var IncidentRoleText = "";
        if (TmpIncidentRoleText.length = 2) {
            IncidentRoleText = TmpIncidentRoleText[1];
        }
        var EmpText = "";
        if (TmpEmpText.length = 2) {
            EmpText = TmpEmpText[1];
        }

        //Add by jutarat A. on 20082012
        if ($("#DepartmentCode").prop("disabled")) {
            DepText = "";
        }
        //End Add

        CheckFirstRowIsEmpty(CTS330_PICGrid,true);

        AddNewRow(CTS330_PICGrid, [BelongOfficeText.replace(/' '/g, '')
            , $('#BelongOfficeCode option:selected').val()
            , DepText.replace(/' '/g, '')
            , $('#DepartmentCode option:selected').val()
            , IncidentRoleText.replace(/' '/g, '')
            , $('#IncidentRoleCode option:selected').val()
            , EmpText.replace(/' '/g, '')
            , $('#EmployeeCode option:selected').val()
            , ""]);

        SetPICToDefault();


        var row_id = CTS330_PICGrid.getRowId(CTS330_PICGrid.getRowsNum() - 1);
        GenerateRemoveButton(CTS330_PICGrid, CTS330_RemovePICBtnID, row_id, "RemoveBtn", true);
        BindGridButtonClickEvent(CTS330_RemovePICBtnID, row_id, btnRemovePIC_clicked);
        CTS330_PICGrid.setSizes();
    }
}

function btnRemovePIC_clicked(rowID) {
    _colEmpNo = CTS330_PICGrid.getColIndexById("EmpNo");
    var isAddFlag = false;
    var roleCode = "";

    if (hasPICMOD(CTS330_PICGrid.cells(rowID, _colEmpNo).getValue())) {
        var tmpObj = GetPICMOD(CTS330_PICGrid.cells(rowID, _colEmpNo).getValue());
        if (tmpObj != null) {
            roleCode = tmpObj.IncidentRoleCode;

            if (tmpObj.Flag == "1") { // It is Add Flag
                RemovePICMOD(CTS330_PICGrid.cells(rowID, _colEmpNo).getValue()); // Remove It
                isAddFlag = true;
            } else {
                tmpObj.Flag = '3';
                UpdatePICMOD(tmpObj);
            }
        }
    } else {
        var picObj = GetPIC(CTS330_PICGrid.cells(rowID, _colEmpNo).getValue());
        if (picObj != null) {
            roleCode = picObj.IncidentRoleCode;

            picObj.Flag = '3';
            AddPICMOD(picObj);
        }
    }
//     else {
//        var obj = {
//            EmpNo: obj.EmpNo,
//            Flag: "3" //Add
//        };
//        AddPICMOD(obj);
//    }
    var picObj = GetPIC(CTS330_PICGrid.cells(rowID, _colEmpNo).getValue());
    RemovePIC(picObj.EmpNo);

    //RemovePIC(CTS330_PICGrid.cells(rowID, _colEmpNo).getValue());

    if (hasChief) {
        if (roleCode == _C_INCIDENT_ROLE_CONTROL_CHIEF) {
            hasChief = false;
        }
//        if (GetPIC(CTS330_PICGrid.cells(rowID, _colEmpNo).getValue()) != null) {
//            if (GetPIC(CTS330_PICGrid.cells(rowID, _colEmpNo).getValue()).IncidentRoleCode == _C_INCIDENT_ROLE_CONTROL_CHIEF) {
//                hasChief = false;
//            }
//        }
    }

    if (CTS330_PICGrid.getRowsNum() == 1) {
        //CTS330_PICGrid.clearAll();
        DeleteAllRow(CTS330_PICGrid);
    } else {
        DeleteRow(CTS330_PICGrid, rowID);
    }
}

function CTS330_PICGrid_binding(gen_ctrl) {
    if ((CTS330_PICGrid.getRowsNum() > 0) && (!CheckFirstRowIsEmpty(CTS330_PICGrid, false))) {
        var mustGenRemoveBtn = true;
        var _colbtnRemove = CTS330_PICGrid.getColIndexById("RemoveBtn");
        var _colOfficeCode = CTS330_PICGrid.getColIndexById("OfficeCode");
        var _colDepartmentCode = CTS330_PICGrid.getColIndexById("DepartmentCode");
        var _colIncidentRoleCode = CTS330_PICGrid.getColIndexById("IncidentRoleCode");
        var _colEmpNo = CTS330_PICGrid.getColIndexById("EmpNo");

        if ((currentInficentData != null) && currentInficentData.CanModPIC && currentInficentData.CanViewPIC && !currentInficentData.IsViewMode) {
            mustGenRemoveBtn = true;
        } else {
            mustGenRemoveBtn = false;
        }

        if (isFirstInitGrid) {
            CTS330_PICDat = new Array();
        }

        if (mustGenRemoveBtn) {
            //CTS330_PICGrid.setColumnHidden(_colbtnRemove, false);




            for (var i = 0; i < CTS330_PICGrid.getRowsNum(); i++) {
                var row_id = CTS330_PICGrid.getRowId(i);
                //var ctrlID = GenerateGridControlID(CTS330_RemovePICBtnID, row_id);
                //var ctrlHtml = CTS330_defaultRemoveButton.replace("{BlankID}", ctrlID);
                var curEmpNo = CTS330_PICGrid.cells(row_id, _colEmpNo).getValue();



                if (isFirstInitGrid) {
                    var obj = {
                        OfficeCode: CTS330_PICGrid.cells(row_id, _colOfficeCode).getValue(),
                        DepartmentCode: CTS330_PICGrid.cells(row_id, _colDepartmentCode).getValue(),
                        IncidentRoleCode: CTS330_PICGrid.cells(row_id, _colIncidentRoleCode).getValue(),
                        EmpNo: CTS330_PICGrid.cells(row_id, _colEmpNo).getValue()
                    }

                    if (!hasChief && (obj.IncidentRoleCode == _C_INCIDENT_ROLE_CONTROL_CHIEF)) {
                        hasChief = true;
                    }

                    AddPIC(obj);
                }

                var enableButton = ((currentInficentData.ExceptedEmp.indexOf(curEmpNo) == -1) || (curEmpNo.length == 0));
                if (gen_ctrl == true) {
                    GenerateRemoveButton(CTS330_PICGrid, CTS330_RemovePICBtnID, row_id, "RemoveBtn", enableButton);
                }
                BindGridButtonClickEvent(CTS330_RemovePICBtnID, row_id, btnRemovePIC_clicked);
            }

            if (isFirstInitGrid) {
                isFirstInitGrid = false;

            }
        } else {
            //CTS330_PICGrid.setColumnHidden(_colbtnRemove, true);
        }





        CTS330_PICGrid.setSizes();
    }

    //ResetScreenToDefault();
}

function RegisterCommand_clicked() {
    DisableRegisterCommand(true);
    DisableResetCommand(true);
    VaridateCtrl(CTS330_ValidateForm, null);
    var obj = createDataObject();

    call_ajax_method_json("/Contract/CTS330_ValidateBusiness", obj, function (result, controls) {
        if ((result != null) && (result == true)) {
            SwitchViewMode(true);
            EnableConfirmCommandPane();
            DisableRegisterCommandPane();
        } else {
            VaridateCtrl(CTS330_ValidateForm, controls);
        }

        DisableRegisterCommand(false);
        DisableResetCommand(false);
    });
}

function ResetCommand_clicked() {
    DisableRegisterCommand(true);
    DisableResetCommand(true);
    var obj = {
        module: "Common",
        code: "MSG0038"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message, function () {
            DisableRegisterCommand(false);
            DisableResetCommand(false);
            SetScreenToDefault();
            LoadObjDataWithReset();
        }, function () {
            DisableRegisterCommand(false);
            DisableResetCommand(false);
        });

    });
}

function ConfirmCommand_clicked() {
    DisableConfirmCommand(true);
    DisableBackCommand(true); //Add by Jutarat A. on 03102012

    var obj = createDataObject();

    call_ajax_method_json("/Contract/CTS330_RegisterData", obj, function (result, controls) {
        if ((result != null) && (result == true)) {
            var obj = {
                module: "Common",
                code: "MSG0046"
            };

            call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                OpenInformationMessageDialog(result.Code, result.Message, function () {
                    SetScreenToDefault();
                    LoadObjDataWithReset();
                });
            });
        }

        DisableConfirmCommand(false); //Add by Jutarat A. on 03102012
        DisableBackCommand(false); //Add by Jutarat A. on 03102012
    });
}

function BackCommand_clicked() {
    DisableConfirmCommand(true);
    SwitchViewMode(false);
    DisableConfirmCommandPane();
    EnableRegisterCommandPane();
    DisableConfirmCommand(false);
    SetFitColumnForBackAction(CTS330_PICGrid, "TmpColumn");
}

// ---------------------------------------------------------------------------------
// Method
// ---------------------------------------------------------------------------------
function LoadObjData() {
    if (isLoadInteractCBB && isLoadOfficeCBB && isInitGrid) {
        call_ajax_method_json("/Contract/CTS330_LoadIncidentData", "", function (result, controls) {
            if (result != null) {
                currentInficentData = result;
                SetScreenToDefault();
                ResetScreenToDefault();
                RefreshAttachList();
                //InitGrid();
            }
        });
    }
}

function LoadObjDataWithReset() {
    if (isLoadInteractCBB && isLoadOfficeCBB) {
        call_ajax_method_json("/Contract/CTS330_ReLoadIncidentData", "", function (result, controls) {
            if (result != null) {
                isFirstInitGrid = true;
                currentInficentData = result;
                ResetScreenToDefault();
                InitGrid();
                RefreshAttachList();
            }
        });
    }
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

function ResetScreenToDefault() {
    // bind JSON
    
    ShowIncidentDetail();

    if (currentInficentData != null) {
        $('#divIncidentInfo').bindJSON(currentInficentData);
        $('#divContractInfo').bindJSON(currentInficentData);
        $('#divSaleContractInfo').bindJSON(currentInficentData);
        $('#divIncidentDetail').bindJSON(currentInficentData);

        // Manual Set (for some field)
        SetRelevantType(currentInficentData.IncidentRelevantType);
        SetDueDate_DeadLine(currentInficentData.DueDateDeadLineType);
        $('#OldContractCode2').val(currentInficentData.OldContractCode);
        $('#OperationOffice1').val(currentInficentData.OperationOffice);
        $('#ProductName1').val(currentInficentData.ProductName);
        $('#ContractCode1').val(currentInficentData.ContractCode);
        $('#ContractCode3').val(currentInficentData.ContractCode);
        $('#ProductName2').val(currentInficentData.ProductName);
        $('#OperationOffice2').val(currentInficentData.OperationOffice);

        if (currentInficentData.IsConfidential) {
            $('#Confidential').attr('checked', 'checked');
        } else {
            $('#Confidential').removeAttr('checked');
        }

        if (currentInficentData.IsImportance) {
            $('#Importance').attr('checked', 'checked');
        } else {
            $('#Importance').removeAttr('checked');
        }

        $('#divRespondingProgress').html(currentInficentData.RespondingProgress_Normal);
        currHistoryView = 1;

        if (currentInficentData.ContractDataFrom == 1) {
            ShowContractInfo();
        } else if (currentInficentData.ContractDataFrom == 2) {
            ShowSaleContractInfo();
        }

        $('#divIncidentInfo').show();
        $('#divIncidentDetail').show();

        if (currentInficentData.CanViewPIC) {
            $('#divAssignPIC').show();

            if (currentInficentData.CanModPIC) {
                $('#BelongOfficeCode').removeAttr("disabled");
                $('#DepartmentCode').removeAttr("disabled");
                $('#IncidentRoleCode').removeAttr("disabled");
                $('#EmployeeCode').removeAttr("disabled");
                $('#btnAddPIC').removeAttr("disabled");
            } else {
                $('#BelongOfficeCode').attr("disabled", "true");
                $('#DepartmentCode').attr("disabled", "true");
                $('#IncidentRoleCode').attr("disabled", "true");
                $('#EmployeeCode').attr("disabled", "true");
                $('#btnAddPIC').attr("disabled", "true");
            }
        } else {
            $('#divAssignPIC').hide();
        }

        if (GetDueDate_DeadLine() == "1") {
            $('#Deadline_Date').EnableDatePicker(false);
            $('#Deadline_Until').val('');
            $('#Deadline_Until').attr("disabled", true);
            $('#DueDate_Date').EnableDatePicker(true);
            $('#DueDate_Time').removeAttr('readonly');
        } else {
            $('#DueDate_Date').EnableDatePicker(false);
            $('#DueDate_Time').val('');
            $('#DueDate_Time').attr('readonly', true);
            $('#Deadline_Date').EnableDatePicker(true);
            $('#Deadline_Until').removeAttr('disabled');
        }
        InteractionType_changed();

        tmpViewMode = currentInficentData.IsViewMode;
        if (tmpViewMode) {
            $('#divAttachFrame').hide();
        }
        else {
            $('#divAttachFrame').show();
        }
        if (currentInficentData.IsViewMode) {
            //        $('#divIncidentInfo').SetViewMode(currentInficentData.IsViewMode);
            //        $('#divContractInfo').SetViewMode(currentInficentData.IsViewMode);
            //        $('#divSaleContractInfo').SetViewMode(currentInficentData.IsViewMode);
            //        $('#divIncidentDetail').SetViewMode(currentInficentData.IsViewMode);

            //        $('#PICSelect1').hide();
            //        $('#PICSelect2').hide();
            //        $('#PICSelect3').hide();

            if (gridAttach != null) {
                var _colRemoveAttach = gridAttach.getColIndexById("removeButton");
                var _colDownloadAttach = gridAttach.getColIndexById("downloadButton");

                gridAttach.setColumnHidden(_colRemoveAttach, true);
                //gridAttach.setColumnHidden(_colDownloadAttach, true);
                gridAttach.setSizes();
                //SetFitColumnForBackAction(gridAttach, "TmpColumn");
            }

            if (CTS330_PICGrid != null) {
                var _colRemoveContract = CTS330_PICGrid.getColIndexById("RemoveBtn");

                CTS330_PICGrid.setColumnHidden(_colRemoveContract, true);
                CTS330_PICGrid.setSizes();
                //SetFitColumnForBackAction(CTS330_PICGrid, "TmpColumn");
            }

            DisableAllCommandPane();

            MakeViewOnly('#divIncidentInfo');
            MakeViewOnly('#divContractInfo');
            MakeViewOnly('#divSaleContractInfo');
            MakeViewOnly('#divIncidentDetail');

            $('#divAttachFrame').hide();
            $('#divAttachRemark').hide();
            $('#DeadlineRemark').hide();

            $('#PICSelect1').hide();
            $('#PICSelect2').hide();
            $('#PICSelect3').hide();

            $('#btnHistory').show();
            $('#btnHistory').removeAttr('disabled');
        } else {
            $('#divIncidentInfo').SetViewMode(currentInficentData.IsViewMode);
            $('#divContractInfo').SetViewMode(currentInficentData.IsViewMode);
            $('#divSaleContractInfo').SetViewMode(currentInficentData.IsViewMode);
            $('#divIncidentDetail').SetViewMode(currentInficentData.IsViewMode);

            $('#PICSelect1').show();
            $('#PICSelect2').show();
            $('#PICSelect3').show();

            EnableRegisterCommandPane();
        }

        $('#rdoCustomer').attr('disabled', 'disabled');
        $('#rdoSite').attr('disabled', 'disabled');
        $('#rdoProject').attr('disabled', 'disabled');
        $('#rdoContract').attr('disabled', 'disabled');

        $('#Confidential').attr('disabled', true);
        $('#Importance').attr('disabled', true);

        hasChief = currentInficentData.HasChief;
    }
}

function LoadInteractionComboBox() {
    call_ajax_method_json("/Contract/CTS330_LoadInteractionCombobox", "", function (result, controls) {
        if ((result != null) && (result.length > 0)) {
            $('#divInteractionCBB').html(result.replace("{BlankID}", "InteractionType"));
            $('#InteractionType').change(InteractionType_changed);
        } else {
            $('#divInteractionCBB').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "InteractionType"));
        }

        $('#InteractionType').attr("style", "width: 235px;");

        isLoadInteractCBB = true;
        LoadObjData();
    });
}

function LoadOfficeComboBox() {
    call_ajax_method_json("/Contract/CTS330_LoadOfficeCombobox", "", function (result, controls) {
        if ((result != null) && (result.CBBMarkup.length > 0) && (result.IsEnablePIC == true)) {
            $('#divCBBOffice').html(result.CBBMarkup.replace("{BlankID}", "BelongOfficeCode"));
            $('#BelongOfficeCode').change(BelongOfficeCode_changed);
        } else {
            $('#divCBBOffice').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "BelongOfficeCode"));
        }

        enablePIC = result.IsEnablePIC;

        $('#divCBBDepartment').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "DepartmentCode"));
        $('#divCBBIncidentRole').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "IncidentRoleCode"));
        $('#divCBBEmployee').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "EmployeeCode"));

        $('#BelongOfficeCode').attr("style", "width: " + cbbWidth);
        $('#DepartmentCode').attr("style", "width: " + cbbWidth);
        $('#IncidentRoleCode').attr("style", "width: " + cbbWidth);
        $('#EmployeeCode').attr("style", "width: " + cbbWidth);

        if (!enablePIC) {
            $('#BelongOfficeCode').attr('disabled', 'disabled');
            $('#DepartmentCode').attr('disabled', 'disabled');
            $('#IncidentRoleCode').attr('disabled', 'disabled');
            $('#EmployeeCode').attr('disabled', 'disabled');
            $('#btnAddPIC').attr('disabled', 'disabled');
        }

        isLoadOfficeCBB = true;
        LoadObjData();
    });
}

function SetPICToDefault() {
    $('#BelongOfficeCode').val('');
    $('#divCBBDepartment').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "DepartmentCode"));
    $('#divCBBIncidentRole').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "IncidentRoleCode"));
    $('#divCBBEmployee').html(CTS330_defaultBlankComboBox.replace("{BlankID}", "EmployeeCode"));

    $('#BelongOfficeCode').attr("style", "width: " + cbbWidth);
    $('#DepartmentCode').attr("style", "width: " + cbbWidth);
    $('#IncidentRoleCode').attr("style", "width: " + cbbWidth);
    $('#EmployeeCode').attr("style", "width: " + cbbWidth);
}

function SetScreenToDefault() {
    CloseWarningDialog();
    $('#divIncidentInfo').clearForm();
    $('#divContractInfo').clearForm();
    $('#divSaleContractInfo').clearForm();
    $('#divIncidentDetail').clearForm();

    SetDueDate_DeadLine(1);
    DueDate_DeadLine_changed();

    EnableStatusAfterUpdate();

    SetPICToDefault();

    SwitchViewMode(false);

    //if (CTS330_PICDat != null) {
    if (CTS330_PICDat != null && isFirstInitGrid) { //Modify by Jutarat A. on 22082012
        CTS330_PICDat = new Array();
    }

    if (CTS330_PICMod != null) {
        CTS330_PICMod = new Array();
    }

    if ((CTS330_PICGrid != null) && isInitPICGrid) {
        //CTS330_PICGrid.clearAll();
        DeleteAllRow(CTS330_PICGrid);
    }

    HideAll();
    DisableAllCommandPane();
}

function EnableRegisterCommandPane() {
    SetRegisterCommand(true, RegisterCommand_clicked);
    SetResetCommand(true, ResetCommand_clicked);
    //DisableRegisterCommand(false);
}

function EnableConfirmCommandPane() {
    SetConfirmCommand(true, ConfirmCommand_clicked);
    SetBackCommand(true, BackCommand_clicked);
    //DisableRegisterCommand(false);
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

function GetDueDate_DeadLine() {
    return $('input[name="DueDateDeadLineType"]:checked').val();
}

function SetDueDate_DeadLine(val) {
    var obj = $('input:radio[name=DueDateDeadLineType]');
    obj.filter('[value=' + val + ']').attr('checked', true);
}

function SetRelevantType(val) {
    var obj = $('input:radio[name=IncidentRelevantType]');
    obj.filter('[value=' + val + ']').attr('checked', true);
}

function GetRelevantType() {
    return $('input[name="IncidentRelevantType"]:checked').val();
}

function HideContractInfo() {
    $('#divContractInfo').hide();
}

function ShowContractInfo() {
    $('#divContractInfo').show();
}

function HideSaleContractInfo() {
    $('#divSaleContractInfo').hide();
}

function ShowSaleContractInfo() {
    $('#divSaleContractInfo').show();
}

function HideIncidentDetail() {
    $('#divIncidentDetail').hide();
}

function ShowIncidentDetail() {
    $('#divIncidentDetail').show();
}

function HideAll() {
    HideContractInfo();
    HideSaleContractInfo();
    //HideIncidentDetail();
}

function SwitchViewMode(isenable) {
    if (gridAttach != null) {
        if (isenable) {
            var _colRemoveAttach = gridAttach.getColIndexById("removeButton");
            var _colDownloadAttach = gridAttach.getColIndexById("downloadButton");

            gridAttach.setColumnHidden(_colRemoveAttach, isenable);
            //gridAttach.setColumnHidden(_colDownloadAttach, isenable);
            gridAttach.setSizes();

            if (isenable) {
                SetFitColumnForBackAction(gridAttach, "TmpColumn");
            }
        } else {
            RefreshAttachList();
        }
    }

    if (CTS330_PICGrid != null) {
        var _colRemoveContract = CTS330_PICGrid.getColIndexById("RemoveBtn");

        CTS330_PICGrid.setColumnHidden(_colRemoveContract, isenable);
        CTS330_PICGrid.setSizes();

        if (isenable)
        {
            SetFitColumnForBackAction(CTS330_PICGrid, "TmpColumn");
        }
    }

    $('#divIncidentInfo').SetViewMode(isenable);
    $('#divContractInfo').SetViewMode(isenable);
    $('#divSaleContractInfo').SetViewMode(isenable);
    $('#divIncidentDetail').SetViewMode(isenable);

    if (!isenable) {
        $('#divAttachFrame').show();
        $('#divAttachRemark').show();
        $('#DeadlineRemark').show();

        $('#PICSelect1').show();
        $('#PICSelect2').show();
        $('#PICSelect3').show();

        if (!enablePIC) {
            $('#BelongOfficeCode').attr('disabled', 'disabled');
            $('#DepartmentCode').attr('disabled', 'disabled');
            $('#IncidentRoleCode').attr('disabled', 'disabled');
            $('#EmployeeCode').attr('disabled', 'disabled');
            $('#btnAddPIC').attr('disabled', 'disabled');
        }
    } else {
        $('#divAttachFrame').hide();
        $('#divAttachRemark').hide();
        $('#DeadlineRemark').hide();
        
        $('#PICSelect1').hide();
        $('#PICSelect2').hide();
        $('#PICSelect3').hide();
    }

    $('#rdoCustomer').attr('disabled', 'disabled');
    $('#rdoSite').attr('disabled', 'disabled');
    $('#rdoProject').attr('disabled', 'disabled');
    $('#rdoContract').attr('disabled', 'disabled');

    $('#Confidential').attr('disabled', true);
    $('#Importance').attr('disabled', true);
}

function hasPIC(empNo) {
    if ((CTS330_PICDat != null) && (CTS330_PICDat != undefined)) {
        for (var i = 0; i < CTS330_PICDat.length; i++) {
            if (CTS330_PICDat[i].EmpNo == empNo) {
                return true;
            }
        }
    }

    return false;
}

function hasPICMOD(empNo) {
    if ((CTS330_PICMod != null) && (CTS330_PICMod != undefined)) {
        for (var i = 0; i < CTS330_PICMod.length; i++) {
            if (CTS330_PICMod[i].EmpNo == empNo) {
                return true;
            }
        }
    }

    return false;
}

function GetPIC(empNo) {
    if ((CTS330_PICDat != null) && (CTS330_PICDat != undefined)) {
        for (var i = 0; i < CTS330_PICDat.length; i++) {
            if (CTS330_PICDat[i].EmpNo == empNo) {
                return CTS330_PICDat[i];
            }
        }
    }

    return null;
}

function GetPICMOD(empNo) {
    if ((CTS330_PICMod != null) && (CTS330_PICMod != undefined)) {
        for (var i = 0; i < CTS330_PICMod.length; i++) {
            if (CTS330_PICMod[i].EmpNo == empNo) {
                return CTS330_PICMod[i];
            }
        }
    }

    return null;
}

function AddPIC(obj) {
    if ((CTS330_PICDat == null) || (CTS330_PICDat == undefined)) {
        CTS330_PICDat = new Array();
    }

    CTS330_PICDat[CTS330_PICDat.length] = obj;
}

function AddPICMOD(obj) {
    if ((CTS330_PICMod == null) || (CTS330_PICMod == undefined)) {
        CTS330_PICMod = new Array();
    }

    CTS330_PICMod[CTS330_PICMod.length] = obj;
}

function UpdatePIC(obj) {
    if ((CTS330_PICDat != null) && (CTS330_PICDat != undefined)) {
        if (hasPIC(obj.EmpNo)) {
            RemovePIC(obj.EmpNo);
        }

        AddPIC(obj);
    }
}

function UpdatePICMOD(obj) {
    if ((CTS330_PICMod != null) && (CTS330_PICMod != undefined)) {
        if (hasPICMOD(obj.EmpNo)) {
            RemovePICMOD(obj.EmpNo);
        }

        AddPICMOD(obj);
    }
}

function RemovePIC(empNo) {
    for (var i = 0; i < CTS330_PICDat.length; i++) {
        if (CTS330_PICDat[i].EmpNo == empNo) {
            CTS330_PICDat.splice(i, 1);
        }
    }
}

function RemovePICMOD(empNo) {
    for (var i = 0; i < CTS330_PICMod.length; i++) {
        if (CTS330_PICMod[i].EmpNo == empNo) {
            CTS330_PICMod.splice(i, 1);
        }
    }
}

function EnableStatusAfterUpdate() {
    $('#cbbStatusAfterUpdate').val('');
    $('#cbbStatusAfterUpdate').removeAttr('disabled');
}

function DisableStatusAfterUpdate() {
    $('#cbbStatusAfterUpdate').val('');
    $('#cbbStatusAfterUpdate').attr('disabled', "true");
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

function createDataObject() {
    var obj = {
        IncidentRoleList: CTS330_PICDat,
        OriginList: CTS330_PICDat,
        HistoryList: CTS330_PICMod,
        InteractionType: $('#InteractionType').val(),
        StatusAfterUpdate: $('#IncidentStatusAfterUpdate').val(),
        CurrentRespondingDetail: $('#CurrentRespondingDetail').val(),
        DueDateDeadLineType: GetDueDate_DeadLine(),
        DueDate_Date: $('#DueDate_Date').val(),
        DueDate_Time: $('#DueDate_Time').val(),
        Deadline_Date: $('#Deadline_Date').val(),
        Deadline_Until: $('#Deadline_Until').val()
    };

    return obj;
}