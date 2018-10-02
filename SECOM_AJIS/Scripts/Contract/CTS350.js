// ---------------------------------------------------------------------------------
// Initial
// ---------------------------------------------------------------------------------
//var hasParameter = true;
var lastCondition = null;
var finFirstLoad = false;
var hasChange = false;
var hasCheckParameter = false;
var lastARType = "";
var lastARTitle = "";

var gridContract = null;
var gridUserGroup = null;
var gridPIC = null;
var gridAttach = null;
var pageRow = 0;
var btnRemoveContractID = "btnRemoveContract";
var btnRemovePICID = "btnRemovePIC";
var chkSendMailPICID = "chkSendMailPIC";
var btnRemoveAttachID = "btnRemoveAttach";

var initContractGrid = false;
var initUserGroupGrid = false;
var initPICGrid = false;
var isInitAttachGrid = false;

var picDat = new Array();
var contractDAT = new Array();

var validateRelevant = ['CustomerCode', 'SiteCode', 'QuotationCode', 'ProjectCode', 'UserCode_ContractCode'];
var validatePIC = ['OfficeCode', 'DepartmentCode', 'ARRoleCode', 'EmployeeCode'];

//var validateAR = ['ARType', 'ARTitle', 'ARSubTitle', 'ARPurpose', 'DueDate_Date', 'DueDate_Time', 'Deadline_Date', 'Deadline_Until', 'ARRoleCode'];
var validateAR = ['ARType', 'ARTitle', 'ARSubTitle', 'ARPurpose', 'ARRoleCode'];

var numBox_Length_10 = 12;
var numBox_Length_9 = 11;
var numBox_Decimal = 2;
var numBox_Min = 0;
var numBox_Max_10 = 999999999999.99;
var numBox_Max_9 = 99999999999.99;
var numBox_DefaultMin = false;

var hasAlert = false;
var alertMsg = "";

$(document).ready(function () {
    InitialPage();
    InitialGrid();
});

function InitialPage() {
    $('#frmAttach').attr('src', 'CTS350_Upload?k=' + _attach_k);
    $('#frmAttach').load(RefreshAttachList);

    $('#ContractQuotationFee').BindNumericBox(numBox_Length_10, numBox_Decimal, numBox_Min, numBox_Max_10, numBox_DefaultMin);
    $('#ContractQuotationFee').setComma();

    $('#ContractARFee').BindNumericBox(numBox_Length_10, numBox_Decimal, numBox_Min, numBox_Max_10, numBox_DefaultMin);
    $('#ContractARFee').setComma();

    $('#DepositQuotationFee').BindNumericBox(numBox_Length_10, numBox_Decimal, numBox_Min, numBox_Max_10, numBox_DefaultMin);
    $('#DepositQuotationFee').setComma();

    $('#DepositARFee').BindNumericBox(numBox_Length_10, numBox_Decimal, numBox_Min, numBox_Max_10, numBox_DefaultMin);
    $('#DepositARFee').setComma();

    $('#InstallQuotationFee').BindNumericBox(numBox_Length_10, numBox_Decimal, numBox_Min, numBox_Max_10, numBox_DefaultMin);
    $('#InstallQuotationFee').setComma();

    $('#InstallARFee').BindNumericBox(numBox_Length_10, numBox_Decimal, numBox_Min, numBox_Max_10, numBox_DefaultMin);
    $('#InstallARFee').setComma();

    $('#ProductPriceQuotationFee').BindNumericBox(numBox_Length_9, numBox_Decimal, numBox_Min, numBox_Max_10, numBox_DefaultMin);
    $('#ProductPriceQuotationFee').setComma();

    $('#ProductPriceARFee').BindNumericBox(numBox_Length_9, numBox_Decimal, numBox_Min, numBox_Max_10, numBox_DefaultMin);
    $('#ProductPriceARFee').setComma();

    $('#InstallFeeQuotationFee').BindNumericBox(numBox_Length_9, numBox_Decimal, numBox_Min, numBox_Max_10, numBox_DefaultMin);
    $('#InstallFeeQuotationFee').setComma();

    $('#InstallFeeARFee').BindNumericBox(numBox_Length_9, numBox_Decimal, numBox_Min, numBox_Max_10, numBox_DefaultMin);
    $('#InstallFeeARFee').setComma();

    $('#rdoCustomer').change(ARRelevantType_changed);
    $('#rdoSite').change(ARRelevantType_changed);
    $('#rdoQuotation').change(ARRelevantType_changed);
    $('#rdoProject').change(ARRelevantType_changed);
    $('#rdoContract').change(ARRelevantType_changed);

    $('#btnRetrive_Customer').click(btnRetrieve_click);
    $('#btnSearch_Customer').click(btnSearch_Customer_click);

    $('#btnRetrive_Site').click(btnRetrieve_click);
    $('#btnSearch_Site').click(btnSearch_Site_click);

    $('#btnRetrive_Quotation').click(btnRetrieve_click);

    $('#btnRetrive_Project').click(btnRetrieve_click);
    $('#btnSearch_Project').click(btnSearch_Project_click);

    $('#btnRetrive_Contract').click(btnRetrieve_click);
    $('#btnSearch_Contract').click(btnSearch_Contract_click);

    $('#btnClear_Contract').click(btnClear_Contract_click)

    $('#ARType').change(ARType_change);
    $('#ARTitle').change(ARTitle_change);

    //$('#rdoDueDate').change(DueDateDeadline_changed);
    //$('#rdoDeadLine').change(DueDateDeadline_changed);

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

    // 2017.02.09 nakajima add start 
    $('#ContractQuotationFeeCurrencyType').change(CalculateGridFee1);
    $('#ContractARFeeCurrencyType').change(CalculateGridFee1);
    $('#DepositQuotationFeeCurrencyType').change(CalculateGridFee1);
    $('#DepositARFeeCurrencyType').change(CalculateGridFee1);
    $('#InstallQuotationFeeCurrencyType').change(CalculateGridFee1);
    $('#InstallARFeeCurrencyType').change(CalculateGridFee1);

    $('#ProductPriceQuotationFeeCurrencyType').change(CalculateGridFee2);
    $('#ProductPriceARFeeCurrencyType').change(CalculateGridFee2);
    $('#InstallFeeQuotationFeeCurrencyType').change(CalculateGridFee2);
    $('#InstallFeeARFeeCurrencyType').change(CalculateGridFee2);
    // 2017.02.09 nakajima add end

//    $("#Deadline_Date").InitialDate();
//    $("#DueDate_Date").InitialDate();
//    $("#DueDate_Time").BindTimeBox();

    $('#btnViewEditARDetail').click(btnViewEditARDetail_click);
    $('#btnRegisterNextAR').click(btnRegisterNextAR_click);

    $('#OfficeCode').change(OfficeCode_change);
    $('#btnAddPIC').click(btnAddPIC_click);

    SetScreenToDefault();

    $("#ARPurpose").SetMaxLengthTextArea(10000); //Add by Jutarat A. on 11122013
}

function InitialGrid() {
    gridContract = $("#CTS350_gridContractTarget").InitialGrid(0, false, "/Contract/CTS350_InitialContractGrid", function () {
        initContractGrid = true;
        finFirstLoad = (initContractGrid && initPICGrid && initUserGroupGrid);
        SetScreenToDefault();
    });
    gridUserGroup = $("#CTS350_gridCustomerGroup").InitialGrid(0, false, "/Contract/CTS350_InitialUserGroupGrid", function () {
        initUserGroupGrid = true;
        finFirstLoad = (initContractGrid && initPICGrid && initUserGroupGrid);
        SetScreenToDefault();
        gridUserGroup.setSizes();
    });
//    gridPIC = $("#CTS350_gridAssignPIC").InitialGrid(0, false, "/Contract/CTS350_InitialAssignPersonInChargeGrid", function () {
//        initPICGrid = true;
//        finFirstLoad = (initContractGrid && initPICGrid && initUserGroupGrid);
//        SetScreenToDefault();
//    });
    ReLoadingPICGrid(false);
    gridAttach = $("#CTS350_gridAttachDocList").InitialGrid(0, false, "/Contract/CTS350_IntialGridAttachedDocList", function () {
        isInitAttachGrid = true;
    });

    SpecialGridControl(gridContract, ["RemoveBtn"]);
    
    SpecialGridControl(gridAttach, ["removeButton"]);

    BindOnLoadedEvent(gridContract, gridContract_binding);
    
    BindOnLoadedEvent(gridUserGroup, gridUserGroup_binding);
    BindOnLoadedEvent(gridAttach, gridAttach_binding);
}

// ---------------------------------------------------------------------------------
// Event
// ---------------------------------------------------------------------------------
function ReLoadingPICGrid(isNeedReAdding) {
    gridPIC = $("#CTS350_gridAssignPIC").InitialGrid(0, false, "/Contract/CTS350_InitialAssignPersonInChargeGrid", function () {
        initPICGrid = true;
        finFirstLoad = (initContractGrid && initPICGrid && initUserGroupGrid);
        if (!isNeedReAdding) {
            SetScreenToDefault();
        }
        SpecialGridControl(gridPIC, ["RemoveBtn"]);
        BindOnLoadedEvent(gridPIC, gridPIC_binding);

        if (isNeedReAdding && (picDat != null)) {
            CheckFirstRowIsEmpty(gridPIC, true);
            for (var i = 0; i < picDat.length; i++) {
                var obj = picDat[i];

                AddNewRow(gridPIC, [obj.OfficeName
                , obj.OfficeCode
                , obj.DepartmentName
                , obj.DepartmentCode
                , obj.ARRoleCode
                , obj.ARRoleName
                , obj.EmployeeName
                , obj.EmpNo
                , obj.SendMail
                , ""]);

                gridPIC_binding(true, true); //Add by Jutarat A. on 30082012
            }
        }
    });
}

function gridAttach_binding(gridtype) {
    if (isInitAttachGrid) {
        var _colRemoveBtn = gridAttach.getColIndexById("removeButton");

        for (var i = 0; i < gridAttach.getRowsNum(); i++) {
            var row_id = gridAttach.getRowId(i);
            if (gridtype == true) {
                GenerateRemoveButton(gridAttach, btnRemoveAttachID, row_id, "removeButton", true);
            }
            BindGridButtonClickEvent(btnRemoveAttachID, row_id, btnRemoveAttach_clicked);
        }
    } else {
        isInitAttachGrid = true;
    }

    gridAttach.setSizes();
}

function btnRemoveAttach_clicked(row_id) {
    var _colID = gridAttach.getColIndexById("AttachFileID");
    var _targID = gridAttach.cells(row_id, _colID).getValue();

    var obj = {
        AttachID: _targID
    };
    call_ajax_method_json("/Contract/CTS350_RemoveAttach", obj, function (result, controls) {
        if (result != null) {
            RefreshAttachList();
        }
    });

}

function RefreshAttachList() {
    if (gridAttach != null) {
        $('#CTS350_gridAttachDocList').LoadDataToGrid(gridAttach, 0, false, "/Contract/CTS350_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, function ()
        {
            if (hasAlert)
            {
                hasAlert = false;
                OpenWarningDialog(alertMsg);
            }
        }, null)
    }
}

function ARRoleCode_change() {
    var obj = {
        OfficeCode: $('#OfficeCode').val(),
        DepartmentCode: $('#DepartmentCode').val(),
        ARRoleCode: $('#ARRoleCode').val()
    };

    if (obj.OfficeCode.length > 0) {
        call_ajax_method_json("/Contract/CTS350_RetrieveEmployeeCBB", obj, function (result, controls) {
            if ((result != null) && (result != "")) {
                $('#divCBBEmployee').html(result.replace('{BLANK_ID}', 'EmployeeCode'));
                $('#EmployeeCode').attr('style', 'width: ' + CTS350_DefaultCBBWidth);
            }
        });
    } else {
        $('#divCBBEmployee').html(CTS350_BlankComboBox.replace('{BLANK_ID}', 'EmployeeCode'));
        $('#EmployeeCode').attr('style', 'width: ' + CTS350_DefaultCBBWidth);
    }
}

function DepartmentCode_change() {
    var obj = {
        OfficeCode: $('#OfficeCode').val(),
        DepartmentCode: $('#DepartmentCode').val()
    };

    if (obj.OfficeCode.length > 0) {
        call_ajax_method_json("/Contract/CTS350_RetrieveARRoleCBB", obj, function (result, controls) {
            if ((result != null) && (result != "")) {
                $('#divCBBARRole').html(result.replace('{BLANK_ID}', 'ARRoleCode'));
                $('#ARRoleCode').attr('style', 'width: ' + CTS350_DefaultCBBWidth);
                $('#ARRoleCode').change(ARRoleCode_change);
            }

            $('#divCBBEmployee').html(CTS350_BlankComboBox.replace('{BLANK_ID}', 'EmployeeCode'));
            $('#EmployeeCode').attr('style', 'width: ' + CTS350_DefaultCBBWidth);
        });
    } else {
        $('#divCBBARRole').html(CTS350_BlankComboBox.replace('{BLANK_ID}', 'ARRoleCode'));
        $('#divCBBEmployee').html(CTS350_BlankComboBox.replace('{BLANK_ID}', 'EmployeeCode'));

        $('#ARRoleCode').attr('style', 'width: ' + CTS350_DefaultCBBWidth);
        $('#EmployeeCode').attr('style', 'width: ' + CTS350_DefaultCBBWidth);
    }
}

function OfficeCode_change() {
    var obj = {
        OfficeCode: $('#OfficeCode').val()
    };

    //Modify by jutarat A. on 20082012
    $('#divCBBDepartment').html(CTS350_BlankComboBox.replace('{BLANK_ID}', 'DepartmentCode'));
    $('#divCBBARRole').html(CTS350_BlankComboBox.replace('{BLANK_ID}', 'ARRoleCode'));
    $('#divCBBEmployee').html(CTS350_BlankComboBox.replace('{BLANK_ID}', 'EmployeeCode'));

    //if (obj.OfficeCode.length > 0) {
        call_ajax_method_json("/Contract/CTS350_RetrieveDepartmentCBB", obj, function (result, controls) {
            //if (result != null) {
            if ((result != null) && (result.length > 0)) { //Modify by jutarat A. on 20082012
                $('#divCBBDepartment').html(result.replace('{BLANK_ID}', 'DepartmentCode'));
                $('#DepartmentCode').attr('style', 'width: ' + CTS350_DefaultCBBWidth);
                $('#DepartmentCode').change(DepartmentCode_change);
            }

            //Add by jutarat A. on 20082012
            if ((result != null) && (result == false)) {
                $('#DepartmentCode').attr("disabled", true);
                DepartmentCode_change();
            }
            //End Add

            $('#divCBBARRole').html(CTS350_BlankComboBox.replace('{BLANK_ID}', 'ARRoleCode'));
            $('#divCBBEmployee').html(CTS350_BlankComboBox.replace('{BLANK_ID}', 'EmployeeCode'));

            $('#DepartmentCode').attr('style', 'width: ' + CTS350_DefaultCBBWidth);
            $('#ARRoleCode').attr('style', 'width: ' + CTS350_DefaultCBBWidth);
            $('#EmployeeCode').attr('style', 'width: ' + CTS350_DefaultCBBWidth);
        });
    //} else {
    //    $('#divCBBDepartment').html(CTS350_BlankComboBox.replace('{BLANK_ID}', 'DepartmentCode'));
    //    $('#divCBBARRole').html(CTS350_BlankComboBox.replace('{BLANK_ID}', 'ARRoleCode'));
    //    $('#divCBBEmployee').html(CTS350_BlankComboBox.replace('{BLANK_ID}', 'EmployeeCode'));

    //    $('#DepartmentCode').attr('style', 'width: ' + CTS350_DefaultCBBWidth);
    //    $('#ARRoleCode').attr('style', 'width: ' + CTS350_DefaultCBBWidth);
    //    $('#EmployeeCode').attr('style', 'width: ' + CTS350_DefaultCBBWidth);
    //}
    //End Modify
}

function btnAddPIC_click() {
    var isValid = false; //Add by jutarat A. on 20082012

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
        SendMail: false,
        CanRemove: true
    };

    if (obj.OfficeCode.length == 0) {
        VaridateCtrl(validatePIC, ['OfficeCode']);
        doAlert("Common", "MSG0007", [$('#divlblOffice').text()]);
    //} else if (obj.DepartmentCode.length == 0) {
    } else if (obj.DepartmentCode.length == 0 && $('#DepartmentCode').prop("disabled") == false) { //Modify by jutarat A. on 20082012
        VaridateCtrl(validatePIC, ["DepartmentCode"]);
        doAlert("Common", "MSG0007", [$('#divlblDepartment').text()]);
    } else if (obj.ARRoleCode.length == 0) {
        VaridateCtrl(validatePIC, ["ARRoleCode"]);
        doAlert("Common", "MSG0007", [$('#divlblARRole').text()]);
    } else if (obj.EmpNo.length == 0) {
        VaridateCtrl(validatePIC, ["EmployeeCode"]);
        doAlert("Common", "MSG0007", [$('#divlvlEmpName').text()]);
    } else {
        if (hasPIC(obj.EmpNo)) {
            VaridateCtrl(validatePIC, ["OfficeCode"]);
            doAlert("Contract", "MSG3021", null);
        } 
        //else if (hasPICRole(obj.ARRoleCode)) {
        else if (obj.ARRoleCode != _C_AR_ROLE_RECEIPIENT && hasPICRole(obj.ARRoleCode)) { //Modify by Jutarat A. on 30082012
            VaridateCtrl(validatePIC, ["ARRoleCode"]);
            doAlert("Contract", "MSG3036", null);
        } 
        //Add by Jutarat A. on 30082012        
        else if (obj.ARRoleCode == _C_AR_ROLE_RECEIPIENT && hasMaxReceipient()) {
            VaridateCtrl(validatePIC, ["ARRoleCode"]);
            doAlert("Contract", "MSG3302", null);
        }
        //End Add 
        else {
            CheckFirstRowIsEmpty(gridPIC, true);
            AddPIC(obj);
            AddNewRow(gridPIC, [obj.OfficeName
                , obj.OfficeCode
                , obj.DepartmentName
                , obj.DepartmentCode
                , obj.ARRoleCode
                , obj.ARRoleName
                , obj.EmployeeName
                , obj.EmpNo
                , obj.SendMail
                , ""]);

            gridPIC_binding(); //Add by Jutarat A. on 30082012

            isValid = true; //Add by jutarat A. on 20082012
        }
    }

    if (isValid) { //Add by jutarat A. on 20082012
        SetPICToDefault();
        SetDefaultSendMail(); //Add by Jutarat A. on 30082012
    }
}

//Add by Jutarat A. on 30082012
function SetDefaultSendMail() {
    var isChckAudit = false;
    var isChckRecep = false;
    var isChckReqst = false;
    var isChckApprv = false;

    var isEnbAudit = true;
    var isEnbRecep = true;
    var isEnbReqst = true;
    var isEnbApprv = true;

    if (hasPICRole(_C_AR_ROLE_APPROVER)) {
        isChckApprv = true;
        isEnbApprv = false;
    }

    if (hasPICRole(_C_AR_ROLE_AUDITOR)) {
        isChckAudit = true;
        isEnbAudit = false;

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

            gridPIC.cells(row_id, _colSendMail).setValue(GenerateCheckBox(chkSendMailPICID, row_id, "", isEnbSendMail));
            
            var chkID = GenerateGridControlID(chkSendMailPICID, row_id);
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
//End Add

function ARType_change() {
    if (($('#ARTitle').val().length > 0) || ($('#ARSubTitle').val().length > 0) || ($('#ARPurpose').val().length > 0)) {
        doAskYesNo("Common", "MSG0028", [CTS350_AskARTypeLbl], LoadARTitle, function () {
            $('#ARType').val(lastARType);
        });
    } else {
        LoadARTitle();
    }
}

function ARTitle_change() {
    if (($('#ARSubTitle').val().length > 0) || ($('#ARPurpose').val().length > 0)) {
        doAskYesNo("Common", "MSG0028", [CTS350_AskARTitleLbl], LoadARSubTitle, function () {
            $('#ARTitle').val(lastARTitle);
        });
    } else {
        LoadARSubTitle();
    }
}

function btnRegisterNextAR_click() {
    lastCondition = null;
    SetScreenToDefault();
}

function btnViewEditARDetail_click() {
    var obj = {
        pRequestNo: $('#RequestNo').val()
    };
    ajax_method.CallScreenControllerWithAuthority("/Contract/CTS380", obj, true);
}

function gridContract_binding(gridtype) {
    if (!finFirstLoad && !initContractGrid) {
        //initContractGrid = true;
        finFirstLoad = (initContractGrid && initPICGrid && initUserGroupGrid);
        SetScreenToDefault();
    } else if (finFirstLoad && initContractGrid) {
        var _colRemoveButton = gridContract.getColIndexById("RemoveBtn");

        for (var i = 0; i < gridContract.getRowsNum(); i++) {
            var row_id = gridContract.getRowId(i);

//            if (gridtype == true) {
//                GenerateRemoveButton(gridContract, btnRemoveContractID, row_id, "RemoveBtn", true);
            //            }
            GenerateRemoveButton(gridContract, btnRemoveContractID, row_id, "RemoveBtn", true);
            BindGridButtonClickEvent(btnRemoveContractID, row_id, RemoveContractTarget_clicked);
        }
    }

    gridContract.setSizes();
}

function gridPIC_binding(gridtype, chkCanEditSendMail) {
    if (!finFirstLoad && !initPICGrid) {
        //initPICGrid = true;
        finFirstLoad = (initContractGrid && initPICGrid && initUserGroupGrid);
        SetScreenToDefault();
    } else if (finFirstLoad && initPICGrid) {
        var _colSendMail = gridPIC.getColIndexById("SendMail");
        var _colRemoveBtn = gridPIC.getColIndexById("RemoveBtn");
        var _colEmpNo = gridPIC.getColIndexById("EmpNo");

        var isEnbSendMail; //Add by Jutarat A. on 05092012

        for (var i = 0; i < gridPIC.getRowsNum(); i++) {
            var row_id = gridPIC.getRowId(i);
            var picObj = GetPIC(gridPIC.cells(row_id, _colEmpNo).getValue());

            if (picObj != null) {
                if (picObj.CanRemove == true) {
//                    if (gridtype == true) {
//                        GenerateRemoveButton(gridPIC, btnRemovePICID, row_id, "RemoveBtn", true);
//                    }

                    GenerateRemoveButton(gridPIC, btnRemovePICID, row_id, "RemoveBtn", true);
                    BindGridButtonClickEvent(btnRemovePICID, row_id, RemovePIC_clicked);
                }

                //Add by Jutarat A. on 05092012
                isEnbSendMail = true;
                if (chkCanEditSendMail) {
                    isEnbSendMail = picObj.CanEditSendMail;
                }
                //End Add

                gridPIC.cells(row_id, _colSendMail).setValue(GenerateCheckBox(chkSendMailPICID, row_id, "", isEnbSendMail)); //true)); //Modify by Jutarat A. on 05092012
                var chkID = GenerateGridControlID(chkSendMailPICID, row_id);
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

function gridUserGroup_binding(gridtype) {
    if (!finFirstLoad && !initUserGroupGrid) {
        //initUserGroupGrid = true;
        finFirstLoad = (initContractGrid && initPICGrid && initUserGroupGrid);
        SetScreenToDefault();

        gridUserGroup.setSizes();
    }
}

function SendMailPIC_change(row_id) {
    var _colEmpNo = gridPIC.getColIndexById("EmpNo");
    var _targEmpNo = gridPIC.cells(row_id, _colEmpNo).getValue();

    var chkID = GenerateGridControlID(chkSendMailPICID, row_id);
    var newVal = $('#' + chkID).is(':checked');

    var currObj = GetPIC(_targEmpNo);
    currObj.SendMail = newVal;
    UpdatePIC(currObj);
}

function RemovePIC_clicked(row_id) {
    var _colEmpNo = gridPIC.getColIndexById("EmpNo");
    var _targEmpNo = gridPIC.cells(row_id, _colEmpNo).getValue();

    if (hasPIC(_targEmpNo.toString())) {
        RemovePIC(_targEmpNo.toString());
    }

    if (gridPIC != null) {
        DeleteRow(gridPIC, row_id);
        SetDefaultSendMail(); //Add by Jutarat A. on 30082012
    }
}


function RemoveContractTarget_clicked(row_id) {
    var _colContractCode = gridContract.getColIndexById("ContractCode");
    var _targContractCode = gridContract.cells(row_id, _colContractCode).getValue();
    
    if (hasContract(_targContractCode.toString())) {
        RemoveContract(_targContractCode.toString());
    }

    var needClearGrid = false;
    if (gridContract.getRowsNum() == 1) {
        needClearGrid = true;
    }

    DeleteRow(gridContract, row_id);

    if (needClearGrid) {
        contractDAT = new Array();
        //gridContract.clearAll();
        if (gridContract != null) {
            DeleteAllRow(gridContract);
        }
        SetScreenToDefaultWithoutReloading();
        SetARRelevantType(_arType_Contract);
        ARRelevantType_changed();
    }
}

function btnClear_Contract_click() {
    $('#btnClear_Contract').attr('disabled', 'disabled');
    doAskYesNo("Common", "MSG0044", [CTS350_AskARTypeLbl], function () {
        lastCondition = null;
        SetScreenToDefault();
        $('#btnClear_Contract').removeAttr('disabled');
    }, function () {
        $('#btnClear_Contract').removeAttr('disabled');
    });
    
//    if (($('#ARTitle').val().length > 0) || ($('#ARSubTitle').val().length > 0) || ($('#ARPurpose').val().length > 0)) {
//        doAskYesNo("Common", "MSG0028", [CTS350_AskARTypeLbl], function () {
//            lastCondition = null;
//            SetScreenToDefault();
//        }, null);
//    } else {
//        lastCondition = null;
//        SetScreenToDefault();
//    }
}

function SetDisableSearch() {
    var currops = GetARRelevantType();
    if (((GetARRelevantType() == _arType_Customer) && ($('#btnRetrive_Customer').prop('disabled')))
        || ((GetARRelevantType() == _arType_Site) && ($('#btnRetrive_Site').prop('disabled')))
        || ((GetARRelevantType() == _arType_Contract) && ($('#btnRetrive_Contract').prop('disabled')))
        || ((GetARRelevantType() == _arType_Project) && ($('#btnRetrive_Project').prop('disabled')))
        || ((GetARRelevantType() == _arType_Quotation) && ($('#btnRetrive_Quotation').prop('disabled')))) {

        if (currops == _arType_Customer) {
            $('#btnRetrive_Customer').attr('disabled', 'true');
            $('#btnSearch_Customer').attr('disabled', 'true');
            $('#CustomerCode').attr('readonly', 'true');

            $('#rdoCustomer').attr('disabled', 'disabled');
            $('#rdoSite').attr('disabled', 'disabled');
            $('#rdoContract').attr('disabled', 'disabled');
            $('#rdoProject').attr('disabled', 'disabled');
            $('#rdoQuotation').attr('disabled', 'disabled');
        } else if (currops == _arType_Site) {
            $('#btnRetrive_Site').attr('disabled', 'true');
            $('#btnSearch_Site').attr('disabled', 'true');
            $('#SiteCode').attr('readonly', 'true');

            $('#rdoCustomer').attr('disabled', 'disabled');
            $('#rdoSite').attr('disabled', 'disabled');
            $('#rdoContract').attr('disabled', 'disabled');
            $('#rdoProject').attr('disabled', 'disabled');
            $('#rdoQuotation').attr('disabled', 'disabled');
        } else if (currops == _arType_Quotation) {
            $('#btnRetrive_Quotation').attr('disabled', 'true');
            $('#QuotationCode').attr('readonly', 'true');

            $('#rdoCustomer').attr('disabled', 'disabled');
            $('#rdoSite').attr('disabled', 'disabled');
            $('#rdoContract').attr('disabled', 'disabled');
            $('#rdoProject').attr('disabled', 'disabled');
            $('#rdoQuotation').attr('disabled', 'disabled');
        } else if (currops == _arType_Project) {
            $('#btnRetrive_Project').attr('disabled', 'true');
            $('#btnSearch_Project').attr('disabled', 'true');
            $('#ProjectCode').attr('readonly', 'true');

            $('#rdoCustomer').attr('disabled', 'disabled');
            $('#rdoSite').attr('disabled', 'disabled');
            $('#rdoContract').attr('disabled', 'disabled');
            $('#rdoProject').attr('disabled', 'disabled');
            $('#rdoQuotation').attr('disabled', 'disabled');
        } else if (currops == _arType_Contract) {
            $('#rdoCustomer').attr('disabled', 'disabled');
            $('#rdoSite').attr('disabled', 'disabled');
            $('#rdoContract').attr('disabled', 'disabled');
            $('#rdoProject').attr('disabled', 'disabled');
            $('#rdoQuotation').attr('disabled', 'disabled');
        }

        if (currops == _arType_Customer) {
            $('#rdoRelatedCustomer').removeAttr('disabled');
            $('#rdoRelatedAllSite').removeAttr('disabled');
            $('#rdoRelatedSite').attr('disabled', 'disabled');
            $('#rdoRelatedAllContract').attr('disabled', 'disabled');
        } else if (currops == _arType_Site) {
            $('#rdoRelatedCustomer').attr('disabled', 'disabled');
            $('#rdoRelatedAllSite').attr('disabled', 'disabled');
            $('#rdoRelatedSite').removeAttr('disabled');
            $('#rdoRelatedAllContract').removeAttr('disabled');
        }
    }
}

function btnRetrieve_click() {
    var currops = GetARRelevantType();
    var isValid = true;

    var newCondition =
    {
        ARRelevantType: GetARRelevantType(),
        ARRelevantCode: ""
    };

    VaridateCtrl(validateRelevant, null);

    if ((currops == _arType_Customer) && ($('#CustomerCode').val().length == 0)) {
        VaridateCtrl(validateRelevant, ["CustomerCode"]);
        isValid = false;
        doAlert("Common", "MSG0007", [$('#divLblCustomerCode').text()]);
    } else if ((currops == _arType_Site) && ($('#SiteCode').val().length == 0)) {
        VaridateCtrl(validateRelevant, ["SiteCode"]);
        isValid = false;
        doAlert("Common", "MSG0007", [$('#divLblSiteCode').text()]);
    } else if ((currops == _arType_Quotation) && ($('#QuotationCode').val().length == 0)) {
        VaridateCtrl(validateRelevant, ["QuotationCode"]);
        isValid = false;
        doAlert("Common", "MSG0007", [$('#divLblQuotationCode').text()]);
    } else if ((currops == _arType_Project) && ($('#ProjectCode').val().length == 0)) {
        VaridateCtrl(validateRelevant, ["ProjectCode"]);
        isValid = false;
        doAlert("Common", "MSG0007", [$('#divLblProjectCode').text()]);
    } else if ((currops == _arType_Contract) && ($('#UserCode_ContractCode').val().length == 0)) {
        VaridateCtrl(validateRelevant, ["UserCode_ContractCode"]);
        isValid = false;
        doAlert("Common", "MSG0007", [$('#divLblUserCode_ContractCode').text()]);
    }

    if (isValid) {
        //HideAll();

        if (currops == _arType_Customer) {
            RetrieveData(function () {
                if (currops == _arType_Customer) {
                    $('#btnRetrive_Customer').attr('disabled', 'true');
                    $('#btnSearch_Customer').attr('disabled', 'true');
                    $('#CustomerCode').attr('readonly', 'true');

                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                } else if (currops == _arType_Site) {
                    $('#btnRetrive_Site').attr('disabled', 'true');
                    $('#btnSearch_Site').attr('disabled', 'true');
                    $('#SiteCode').attr('readonly', 'true');

                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                } else if (currops == _arType_Quotation) {
                    $('#btnRetrive_Quotation').attr('disabled', 'true');
                    $('#QuotationCode').attr('readonly', 'true');

                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                } else if (currops == _arType_Project) {
                    $('#btnRetrive_Project').attr('disabled', 'true');
                    $('#btnSearch_Project').attr('disabled', 'true');
                    $('#ProjectCode').attr('readonly', 'true');

                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                } else if (currops == _arType_Contract) {
                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                }

                newCondition.ARRelevantCode = $('#CustomerCode').val();
                lastCondition = newCondition;
                ShowCustomerInfo();
                //DueDateDeadline_changed();
            });
        } else if (currops == _arType_Site) {
            RetrieveData(function () {
                if (currops == _arType_Customer) {
                    $('#btnRetrive_Customer').attr('disabled', 'true');
                    $('#btnSearch_Customer').attr('disabled', 'true');
                    $('#CustomerCode').attr('readonly', 'true');

                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                } else if (currops == _arType_Site) {
                    $('#btnRetrive_Site').attr('disabled', 'true');
                    $('#btnSearch_Site').attr('disabled', 'true');
                    $('#SiteCode').attr('readonly', 'true');

                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                } else if (currops == _arType_Quotation) {
                    $('#btnRetrive_Quotation').attr('disabled', 'true');
                    $('#QuotationCode').attr('readonly', 'true');

                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                } else if (currops == _arType_Project) {
                    $('#btnRetrive_Project').attr('disabled', 'true');
                    $('#btnSearch_Project').attr('disabled', 'true');
                    $('#ProjectCode').attr('readonly', 'true');

                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                } else if (currops == _arType_Contract) {
                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                }

                newCondition.ARRelevantCode = $('#SiteCode').val();
                lastCondition = newCondition;
                ShowSiteInfo();
                //DueDateDeadline_changed();
        });
        } else if (currops == _arType_Quotation) {
            RetrieveData(function () {
                if (currops == _arType_Customer) {
                    $('#btnRetrive_Customer').attr('disabled', 'true');
                    $('#btnSearch_Customer').attr('disabled', 'true');
                    $('#CustomerCode').attr('readonly', 'true');

                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                } else if (currops == _arType_Site) {
                    $('#btnRetrive_Site').attr('disabled', 'true');
                    $('#btnSearch_Site').attr('disabled', 'true');
                    $('#SiteCode').attr('readonly', 'true');

                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                } else if (currops == _arType_Quotation) {
                    $('#btnRetrive_Quotation').attr('disabled', 'true');
                    $('#QuotationCode').attr('readonly', 'true');

                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                } else if (currops == _arType_Project) {
                    $('#btnRetrive_Project').attr('disabled', 'true');
                    $('#btnSearch_Project').attr('disabled', 'true');
                    $('#ProjectCode').attr('readonly', 'true');

                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                } else if (currops == _arType_Contract) {
                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                }

                newCondition.ARRelevantCode = $('#QuotationCode').val();
                lastCondition = newCondition;
                ShowQuotationInfo();
                //DueDateDeadline_changed();
            });
        } else if (currops == _arType_Project) {
            RetrieveData(function () {
                if (currops == _arType_Customer) {
                    $('#btnRetrive_Customer').attr('disabled', 'true');
                    $('#btnSearch_Customer').attr('disabled', 'true');
                    $('#CustomerCode').attr('readonly', 'true');

                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                } else if (currops == _arType_Site) {
                    $('#btnRetrive_Site').attr('disabled', 'true');
                    $('#btnSearch_Site').attr('disabled', 'true');
                    $('#SiteCode').attr('readonly', 'true');

                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                } else if (currops == _arType_Quotation) {
                    $('#btnRetrive_Quotation').attr('disabled', 'true');
                    $('#QuotationCode').attr('readonly', 'true');

                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                } else if (currops == _arType_Project) {
                    $('#btnRetrive_Project').attr('disabled', 'true');
                    $('#btnSearch_Project').attr('disabled', 'true');
                    $('#ProjectCode').attr('readonly', 'true');

                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                } else if (currops == _arType_Contract) {
                    $('#rdoCustomer').attr('disabled', 'disabled');
                    $('#rdoSite').attr('disabled', 'disabled');
                    $('#rdoContract').attr('disabled', 'disabled');
                    $('#rdoProject').attr('disabled', 'disabled');
                    $('#rdoQuotation').attr('disabled', 'disabled');
                }

                newCondition.ARRelevantCode = $('#ProjectCode').val();
                lastCondition = newCondition;
                ShowProjectInfo();
                //DueDateDeadline_changed();
            });
        } else if (currops == _arType_Contract) {
            if (hasContract($('#UserCode_ContractCode').val())) {
                doAlert("Contract", "MSG3194", [$('#UserCode_ContractCode').val()]);
                VaridateCtrl(validateRelevant, ["UserCode_ContractCode"]);
            } else {
                RetrieveData(function () {
                    //                $('#btnRetrive_Contract').attr('disabled', 'true');
                    //                $('#btnSearch_Contract').attr('disabled', 'true');
                    //                $('#UserCode_ContractCode').attr('readonly', 'true');
                    newCondition.ARRelevantCode = $('#UserCode_ContractCode').val();

                    if (currops == _arType_Customer) {
                        $('#btnRetrive_Customer').attr('disabled', 'true');
                        $('#btnSearch_Customer').attr('disabled', 'true');
                        $('#CustomerCode').attr('readonly', 'true');

                        $('#rdoCustomer').attr('disabled', 'disabled');
                        $('#rdoSite').attr('disabled', 'disabled');
                        $('#rdoContract').attr('disabled', 'disabled');
                        $('#rdoProject').attr('disabled', 'disabled');
                        $('#rdoQuotation').attr('disabled', 'disabled');
                    } else if (currops == _arType_Site) {
                        $('#btnRetrive_Site').attr('disabled', 'true');
                        $('#btnSearch_Site').attr('disabled', 'true');
                        $('#SiteCode').attr('readonly', 'true');

                        $('#rdoCustomer').attr('disabled', 'disabled');
                        $('#rdoSite').attr('disabled', 'disabled');
                        $('#rdoContract').attr('disabled', 'disabled');
                        $('#rdoProject').attr('disabled', 'disabled');
                        $('#rdoQuotation').attr('disabled', 'disabled');
                    } else if (currops == _arType_Quotation) {
                        $('#btnRetrive_Quotation').attr('disabled', 'true');
                        $('#QuotationCode').attr('readonly', 'true');

                        $('#rdoCustomer').attr('disabled', 'disabled');
                        $('#rdoSite').attr('disabled', 'disabled');
                        $('#rdoContract').attr('disabled', 'disabled');
                        $('#rdoProject').attr('disabled', 'disabled');
                        $('#rdoQuotation').attr('disabled', 'disabled');
                    } else if (currops == _arType_Project) {
                        $('#btnRetrive_Project').attr('disabled', 'true');
                        $('#btnSearch_Project').attr('disabled', 'true');
                        $('#ProjectCode').attr('readonly', 'true');

                        $('#rdoCustomer').attr('disabled', 'disabled');
                        $('#rdoSite').attr('disabled', 'disabled');
                        $('#rdoContract').attr('disabled', 'disabled');
                        $('#rdoProject').attr('disabled', 'disabled');
                        $('#rdoQuotation').attr('disabled', 'disabled');
                    } else if (currops == _arType_Contract) {
                        $('#rdoCustomer').attr('disabled', 'disabled');
                        $('#rdoSite').attr('disabled', 'disabled');
                        $('#rdoContract').attr('disabled', 'disabled');
                        $('#rdoProject').attr('disabled', 'disabled');
                        $('#rdoQuotation').attr('disabled', 'disabled');
                    }

                    lastCondition = newCondition;
                    $('#CTS350_gridContractTarget').show();
                    //DueDateDeadline_changed();
                });
            }
        }
    }
}

function btnSearch_Contract_click() {
    VaridateCtrl(validateRelevant, null);
    $("#dlgBox").OpenCMS310Dialog("CTS350");
}

function btnSearch_Project_click() {
    VaridateCtrl(validateRelevant, null);
    $("#dlgBox").OpenCMS290Dialog("CTS350");
}

function btnSearch_Site_click() {
    VaridateCtrl(validateRelevant, null);
    $("#dlgBox").OpenCMS260Dialog("CTS350");
}

function btnSearch_Customer_click() {
    VaridateCtrl(validateRelevant, null);
    $("#dlgBox").OpenCMS250Dialog("CTS350");
}

function CMS250Response(obj) {
    $("#dlgBox").CloseDialog();
    if ((obj != null) && (obj.CustomerData != null) && (obj.CustomerData.CustCode != null) && (obj.CustomerData.CustCode.length > 0)) {
        SetScreenToDefault();
        SetARRelevantType(_arType_Customer);
        ARRelevantType_changed();

        $('#CustomerCode').val(obj.CustomerData.CustCode);
        btnRetrieve_click();
    }
}

function CMS260Response(obj) {
    $("#dlgBox").CloseDialog();
    if ((obj != null) && (obj.SiteCode != null) && (obj.SiteCode.length > 0)) {
        SetScreenToDefault();
        SetARRelevantType(_arType_Site);
        ARRelevantType_changed();

        $('#SiteCode').val(obj.SiteCode);
        btnRetrieve_click();
    }
}

function CMS290Response(obj) {
    $("#dlgBox").CloseDialog();
    if ((obj != null) && (obj.ProjectCode != null) && (obj.ProjectCode.length > 0)) {
        SetScreenToDefault();
        SetARRelevantType(_arType_Project);
        ARRelevantType_changed();

        $('#ProjectCode').val(obj.ProjectCode);
        btnRetrieve_click();
    }
}

function CMS310Response(obj) {
    $("#dlgBox").CloseDialog();
    if ((obj != null) && (obj.ContractCode != null) && (obj.ContractCode.length > 0)) {
        if (!hasContract(obj.ContractCode)) {
            if (GetARRelevantType() != _arType_Contract) {
                SetScreenToDefault();
                SetARRelevantType(_arType_Contract);
                ARRelevantType_changed();
            }
            
            $('#UserCode_ContractCode').val(obj.ContractCode);
            btnRetrieve_click();
        } else {
            doAlert("Contract", "MSG3194", obj.ContractCode);
            VaridateCtrl(validateRelevant, ["UserCode_ContractCode"]);
        }
    }
}

function ARRelevantType_changed() {
    VaridateCtrl(validateRelevant, null);
    var currops = GetARRelevantType();

    HideAll();

    $('#CustomerCode').val('');
    $('#SiteCode').val('');
    $('#QuotationCode').val('');
    $('#ProjectCode').val('');
    $('#UserCode_ContractCode').val('');

    $('#CustomerCode').attr('readonly', 'true');
    $('#SiteCode').attr('readonly', 'true');
    $('#QuotationCode').attr('readonly', 'true');
    $('#ProjectCode').attr('readonly', 'true');
    $('#UserCode_ContractCode').attr('readonly', 'true');

    $('#btnRetrive_Customer').attr('disabled', 'true');
    $('#btnSearch_Customer').attr('disabled', 'true');
    $('#rdoRelatedCustomer').attr('disabled', 'true');
    $('#rdoRelatedAllSite').attr('disabled', 'true');

    $('#btnRetrive_Site').attr('disabled', 'true');
    $('#btnSearch_Site').attr('disabled', 'true');
    $('#rdoRelatedSite').attr('disabled', 'true');
    $('#rdoRelatedAllContract').attr('disabled', 'true');

    $('#btnRetrive_Quotation').attr('disabled', 'true');

    $('#btnRetrive_Project').attr('disabled', 'true');
    $('#btnSearch_Project').attr('disabled', 'true');

    $('#btnRetrive_Contract').attr('disabled', 'true');
    $('#btnSearch_Contract').attr('disabled', 'true');

    $('#CTS350_gridContractTarget').hide();

    SetCustomerRelateType(1);
    SetSiteRelateType(1);

    DisableAllCommandPane();

    if (currops == _arType_Customer) {
        $('#CustomerCode').removeAttr('readonly');
        $('#btnRetrive_Customer').removeAttr('disabled');
        $('#btnSearch_Customer').removeAttr('disabled');
        $('#rdoRelatedCustomer').removeAttr('disabled');
        $('#rdoRelatedAllSite').removeAttr('disabled');
    } else if (currops == _arType_Site) {
        $('#SiteCode').removeAttr('readonly');
        $('#btnRetrive_Site').removeAttr('disabled');
        $('#btnSearch_Site').removeAttr('disabled');
        $('#rdoRelatedSite').removeAttr('disabled');
        $('#rdoRelatedAllContract').removeAttr('disabled');
    } else if (currops == _arType_Quotation) {
        $('#QuotationCode').removeAttr('readonly');
        $('#btnRetrive_Quotation').removeAttr('disabled');
    } else if (currops == _arType_Project) {
        $('#ProjectCode').removeAttr('readonly');
        $('#btnRetrive_Project').removeAttr('disabled');
        $('#btnSearch_Project').removeAttr('disabled');
    } else if (currops == _arType_Contract) {
        $('#UserCode_ContractCode').removeAttr('readonly');
        $('#btnRetrive_Contract').removeAttr('disabled');
        $('#btnSearch_Contract').removeAttr('disabled');

        $('#CTS350_gridContractTarget').show();
    }

    contractDAT = new Array();
    if (gridContract != null) {
        DeleteAllRow(gridContract);
    }
}

//function DueDateDeadline_changed() {
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

function RegisterCommand_clicked() {
    DisableRegisterCommand(true);
    DisableResetCommand(true);
    VaridateCtrl(validateAR, null);
    var obj = CreateFormObject();

    call_ajax_method_json("/Contract/CTS350_ValidateData", obj, function (result, controls) {
        if (controls != null) {
            VaridateCtrl(validateAR, controls);
        }

        if ((result != null) && (result == true)) {
            SetViewModeForm(true);
            DisableRegisterCommandPane();
            EnableConfirmCommandPane();
        }

        DisableRegisterCommand(false);
        DisableResetCommand(false);
    });
}

function ResetCommand_clicked() {
//    if (($('#ARTitle').val().length > 0) || ($('#ARSubTitle').val().length > 0) || ($('#ARPurpose').val().length > 0)) {
//        doAskYesNo("Common", "MSG0028", [CTS350_AskARTypeLbl], doReset, null);
//    }
//    else {
//        doReset();
    //    }
    DisableRegisterCommand(true);
    DisableResetCommand(true);
    doAskYesNo("Common", "MSG0038", null, function () {
        DisableRegisterCommand(false);
        DisableResetCommand(false);
        doReset();
    }, function () {
        DisableRegisterCommand(false);
        DisableResetCommand(false);
    });
}

function doReset() {
    //lastCondition = null;
    SetScreenToDefault();

    VaridateCtrl(validateRelevant, null);
    VaridateCtrl(validatePIC, null);
    VaridateCtrl(validateAR, null);

    //    if (hasParameter) {
    //        RetrieveDataFromParameter();
    //    }
}

function ConfirmCommand_clicked() {
    DisableConfirmCommand(true);
    DisableBackCommand(true); //Add by Jutarat A. on 27092012

    var obj = CreateFormObject();
    obj = ReCorrenctingData(obj);

    call_ajax_method_json("/Contract/CTS350_RegisNewAR", obj, function (result, controls) {
        if (result != null) {
            if ((result.RequestNo == null) || (result.RequestNo.length == 0)) {
                $('#RequestNo').val('');
                $('#btnViewEditARDetail').attr('disabled', 'disabled');
            } else {
                $('#RequestNo').val(result.RequestNo);
                $('#btnViewEditARDetail').removeAttr('disabled');
            }

            $('#ARStatus').val(result.RegisStatus);
            DisableConfirmCommandPane();
            DisableConfirmCommand(false);
            DisableBackCommand(false); //Add by Jutarat A. on 27092012

            ShowRegisResult();

            setTimeout(function () {
                master_event.ScrollWindow($('#divResultRegister'), false);
            }, 500);
        }
    });
}

function ReCorrenctingData(objDat) {
    objDat.ContractFee_Quotation = objDat.ContractFee_Quotation.replace(/ /g, "").replace(/,/g, "");
    objDat.ContractFee_AR = objDat.ContractFee_AR.replace(/ /g, "").replace(/,/g, "");
    objDat.Deposit_Quotation = objDat.Deposit_Quotation.replace(/ /g, "").replace(/,/g, "");
    objDat.Deposit_AR = objDat.Deposit_AR.replace(/ /g, "").replace(/,/g, "");
    objDat.Installation_Quotation = objDat.Installation_Quotation.replace(/ /g, "").replace(/,/g, "");
    objDat.Installation_AR = objDat.Installation_AR.replace(/ /g, "").replace(/,/g, "");

    objDat.ProductPrice_Quotation = objDat.ProductPrice_Quotation.replace(/ /g, "").replace(/,/g, "");
    objDat.ProductPrice_AR = objDat.ProductPrice_AR.replace(/ /g, "").replace(/,/g, "");
    objDat.InstallFee_Quotation = objDat.InstallFee_Quotation.replace(/ /g, "").replace(/,/g, "");
    objDat.InstallFee_AR = objDat.InstallFee_AR.replace(/ /g, "").replace(/,/g, "");

    return objDat;
}

function BackCommand_clicked() {
    DisableConfirmCommand(true);
    SetViewModeForm(false, true);
    EnableRegisterCommandPane();
    DisableConfirmCommandPane();
    DisableConfirmCommand(false);
}

// ---------------------------------------------------------------------------------
// Method
// ---------------------------------------------------------------------------------
function SetScreenToDefaultWithoutReloading() {
    call_ajax_method_json("/Contract/CTS300_ClearAttach", "", function (result, controls) {
        if (result != null) {
            RefreshAttachList();
        }
    });

    SetViewModeForm(false, false);
    CloseWarningDialog();

    $("#divQuotationTargetInfo").clearForm();
    $("#divProjectInformation").clearForm();
    $("#divCustomerInformation").clearForm();
    $("#divSiteInformation").clearForm();
    $("#divARForm").clearForm();

    $('#divFeeGrid1').hide();
    $('#divFeeGrid2').hide();

    $('#rdoCustomer').removeAttr('disabled');
    $('#rdoSite').removeAttr('disabled');
    $('#rdoContract').removeAttr('disabled');
    $('#rdoProject').removeAttr('disabled');
    $('#rdoQuotation').removeAttr('disabled');

    VaridateCtrl(validateRelevant, null);
    VaridateCtrl(validatePIC, null);

    CalculateGridFee1();
    CalculateGridFee2();

    SetPICToDefault();

    HideAll();

    if (finFirstLoad) {
        if (gridContract != null) {
            //gridContract.clearAll();
            DeleteAllRow(gridContract);
        }

        if (gridUserGroup != null) {
            //gridUserGroup.clearAll();
            DeleteAllRow(gridUserGroup);
        }

        if (gridPIC != null) {
            //gridPIC.clearAll();
            DeleteAllRow(gridPIC);
        }

        picDat = new Array();
        contractDAT = new Array();

//        SetARRelevantType(_arType_Customer);
        SetARRelevantType(_arType_Contract);
        ARRelevantType_changed();

        //SetDueDateDateTime(1);
        //        DueDateDeadline_changed();

        hasChange = false;

        DisableAllCommandPane();
    }
}

function LoadARSubTitle() {
    lastARTitle = $('#ARTitle').val();

    var obj = {
        cond: CreateConditionObject(),
        contractLst: contractDAT,
        strARType: $('#ARType').val(),
        strARTitle: $('#ARTitle').val()
    };

    call_ajax_method_json("/Contract/CTS350_RetrieveARPurpose", obj, function (result, controls) {
        if (result != null) {
            $('#ARPurpose').val(result.ARPurpose);
            if (result.EnableGrid1) {
                $('#divFeeGrid1').show();
            } else {
                $('#divFeeGrid1').hide();
            }

            if (result.EnableGrid2) {
                $('#divFeeGrid2').show();
            } else {
                $('#divFeeGrid2').hide();
            }
        } else {
            $('#ARPurpose').val('');
            $('#divFeeGrid1').hide();
            $('#divFeeGrid2').hide();
        }

        if ($('#ARTitle').val().length > 0) {
            $('#ARSubTitle').val($('#ARTitle option:selected').text());
        } else {
            $('#ARSubTitle').val('');
        }
    })

    $('#divFeeGrid1').hide();
    $('#divFeeGrid2').hide();
    $('#divFeeGrid1').clearForm();
    $('#divFeeGrid2').clearForm();
    CalculateGridFee1();
    CalculateGridFee2();

    $('#ARPurpose').val('');
    $('#ARSubTitle').val('');
}

function LoadARTitle() {
    lastARType = $('#ARType').val();

    var obj = {
        ARType: $('#ARType').val()
    };

    if (obj.ARType.length > 0) {
        call_ajax_method_json("/Contract/CTS350_RetrieveARTitle", obj, function (result, controls) {
            if (result != null) {
                $('#divARTitle').html(result.replace('{BLANK_ID}', 'ARTitle') + ' ' + CTS350_RequireElement);
                $('#ARTitle').change(ARTitle_change);
            } else {
                $('#divARTitle').html(CTS350_BlankComboBox.replace('{BLANK_ID}', 'ARTitle') + ' ' + CTS350_RequireElement);
            }

            $('#ARTitle').attr("style", "width: 715px;")
        })
    }

    $('#divFeeGrid1').hide();
    $('#divFeeGrid2').hide();
    $('#divFeeGrid1').clearForm();
    $('#divFeeGrid2').clearForm();
    CalculateGridFee1();
    CalculateGridFee2();

    $('#divARTitle').html(CTS350_BlankComboBox.replace('{BLANK_ID}', 'ARTitle') + ' ' + CTS350_RequireElement);
    $('#ARTitle').attr("style", "width: 715px;")
    $('#ARPurpose').val('');
    $('#ARSubTitle').val('');
}

function RetrieveDataFromParameter() {
    call_ajax_method_json("/Contract/CTS350_RetrieveARDataFromParameter", "", function (result, controls) {
        hasCheckParameter = true;

        if (result != null) {
            var newCondition =
            {
                ARRelevantType: result.ARRelevantType,
                ARRelevantCode: ""
            };

            if (result.ARRelevantType == _arType_Customer) {
                newCondition.ARRelevantCode = result.CustomerCode;
            } else if (result.ARRelevantType == _arType_Site) {
                newCondition.ARRelevantCode = result.SiteCode;
            } else if (result.ARRelevantType == _arType_Quotation) {
                newCondition.ARRelevantCode = result.QuotationCode;
            } else if (result.ARRelevantType == _arType_Project) {
                newCondition.ARRelevantCode = result.ProjectCode;
            } else if (result.ARRelevantType == _arType_Contract) {
                newCondition.ARRelevantCode = result.ContractCode;
            }

            lastCondition = newCondition;
            BindingConditionFromObj();
            //btnRetrieve_click();
        } else {

        }
    })
}

function RetrieveData(showFunc) {
    var obj = CreateConditionObject();

    LoadingData(obj, showFunc);
}

function BindingConditionFromObj() {
    if (lastCondition != null) {
        SetARRelevantType(lastCondition.ARRelevantType);
        ARRelevantType_changed();

        if (GetARRelevantType() == _arType_Customer) {
            $('#CustomerCode').val(lastCondition.ARRelevantCode);
        } else if (GetARRelevantType() == _arType_Site) {
            $('#SiteCode').val(lastCondition.ARRelevantCode);
        } else if (GetARRelevantType() == _arType_Quotation) {
            $('#QuotationCode').val(lastCondition.ARRelevantCode);
        } else if (GetARRelevantType() == _arType_Project) {
            $('#ProjectCode').val(lastCondition.ARRelevantCode);
        } else if (GetARRelevantType() == _arType_Contract) {
            $('#UserCode_ContractCode').val(lastCondition.ARRelevantCode);
        }

        btnRetrieve_click();
    }
}

function LoadingData(condition, showFunc) {
    call_ajax_method_json("/Contract/CTS350_RetrieveARData", condition, function (result, controls) {
        if (controls != null) {
            VaridateCtrl(validateRelevant, controls);
        }

        if ((result != null) && (result.PersonInCharge != null) && ((result.CustomerData != null) || (result.SiteData != null) || (result.QuotationData != null) || (result.ProjectData != null) || (result.ContractData != null))) {
            if (typeof (showFunc) == "function") {
                showFunc();
            }

            if ((GetARRelevantType() == _arType_Customer) && (result.CustomerData != null)) {
                // Binding to Customer
                $('#divCustomerInformation').bindJSON(result.CustomerData);
                $('#CustomerCode').val(result.CustomerData.CustCode);
                LoadCustomerGroup(condition.CustomerCode);
            } else if ((GetARRelevantType() == _arType_Site) && (result.SiteData != null)) {
                // Binding to Site
                $('#divSiteInformation').bindJSON(result.SiteData);
                $('#SiteCode').val(result.SiteData.SiteCode);
            } else if ((GetARRelevantType() == _arType_Quotation) && (result.QuotationData != null)) {
                // Binding to Quotation
                $('#divQuotationTargetInfo').bindJSON(result.QuotationData);
                $('#QuotationCode').val(result.QuotationData.QuotationCode);
            } else if ((GetARRelevantType() == _arType_Project) && (result.ProjectData != null)) {
                // Binding to Project
                $('#divProjectInformation').bindJSON(result.ProjectData);
                $('#ProjectCode').val(result.ProjectData.ProjectCode);
            } else if ((GetARRelevantType() == _arType_Contract) && (result.ContractData != null)) {
                // Adding to Contract target
                //$('#UserCode_ContractCode').val(result.ContractData.ShortContractCode);
                CheckFirstRowIsEmpty(gridContract, true);
                AddContract(result.ContractData);
                AddNewRow(gridContract, [result.ContractData.ContractName
                        , result.ContractData.SiteName
                        , result.ContractData.UserCode
                        , result.ContractData.ShortContractCode
                        , ""]);

                gridContract_binding(); //Add by Jutarat A. on 30082012
            }

            // Add PIC
            if ((result.PersonInCharge != null)) {
                if ((GetARRelevantType() == _arType_Contract) && (result.ContractData != null) && (picDat.length > 0)) {
                    // Do Nothing
                } else {
                    //gridPIC.clearAll();
                    DeleteAllRow(gridPIC);
                    picDat = new Array();
                    CheckFirstRowIsEmpty(gridPIC, true);
                    AddPIC(result.PersonInCharge);
                    AddNewRow(gridPIC, [result.PersonInCharge.OfficeName
                            , result.PersonInCharge.OfficeCode
                            , result.PersonInCharge.DepartmentName
                            , result.PersonInCharge.DepartmentCode
                            , result.PersonInCharge.ARRoleCode
                            , result.PersonInCharge.ARRoleName
                            , result.PersonInCharge.EmployeeName
                            , result.PersonInCharge.EmpNo
                            , result.PersonInCharge.SendMail
                            , ""]);

                    gridPIC_binding(); //Add by Jutarat A. on 30082012
                }
            }

            ShowARForm();
            EnableRegisterCommandPane();
        }
    });
}

function LoadCustomerGroup(custCode) {
    var obj = {
        strCustomerCode: custCode
    }

    $('#CTS350_gridCustomerGroup').LoadDataToGrid(gridUserGroup, 0, false, "/Contract/CTS350_RetrieveCustomerGroup", obj, "dtCustomeGroupData", false, null, function () {
        $('#CTS350_gridCustomerGroup').show();
        gridUserGroup.setSizes();
    });
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

        if ((arFee > 0) && (quotationFee > 0)) {
            outRate = ((arFee - quotationFee) / quotationFee) * 100;
        } else if ((arFee == 0) && (quotationFee == 0)) {
            outRate = 0;
        } else {
            if (arFee == 0) {
                outRate = -100;
            } else {
                outRate = 100;
            }
        }

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

        if ((arFee > 0) && (quotationFee > 0)) {
            outRate = ((arFee - quotationFee) / quotationFee) * 100;
        } else if ((arFee == 0) && (quotationFee == 0)) {
            outRate = 0;
        } else {
            if (arFee == 0) {
                outRate = -100;
            } else {
                outRate = 100;
            }
        }

        if (isNaN(outRate))
            outRate = 0;
    }

    if (outRate < 0)
    {
        $('#spResDepositFee').text(lblDC);
        $('#DepositOCDCRate').val(Math.abs(outRate).toFixed(2));
        $('#DepositOCDCRate').setComma();
    } else if (outRate > 0)
    {
        $('#spResDepositFee').text(lblOR);
        $('#DepositOCDCRate').val(Math.abs(outRate).toFixed(2));
        $('#DepositOCDCRate').setComma();
    }
     else
    {
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

        if ((arFee > 0) && (quotationFee > 0)) {
            outRate = ((arFee - quotationFee) / quotationFee) * 100;
        } else if ((arFee == 0) && (quotationFee == 0)) {
            outRate = 0;
        } else {
            if (arFee == 0) {
                outRate = -100;
            } else {
                outRate = 100;
            }
        }

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

        if ((arFee > 0) && (quotationFee > 0)) {
            outRate = ((arFee - quotationFee) / quotationFee) * 100;
        } else if ((arFee == 0) && (quotationFee == 0)) {
            outRate = 0;
        } else {
            if (arFee == 0) {
                outRate = -100;
            } else {
                outRate = 100;
            }
        }

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

        if ((arFee > 0) && (quotationFee > 0)) {
            outRate = ((arFee - quotationFee) / quotationFee) * 100;
        } else if ((arFee == 0) && (quotationFee == 0)) {
            outRate = 0;
        } else {
            if (arFee == 0) {
                outRate = -100;
            } else {
                outRate = 100;
            }
        }

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

    //if ((sumAR > 0) && (sumQuotation > 0)) {
    //    outRate = ((sumAR - sumQuotation) / sumQuotation) * 100;
    //} else if ((sumAR == 0) && (sumQuotation == 0)) {
    //    outRate = 0;
    //} else {
    //    if (sumAR == 0) {
    //        outRate = -100;
    //    } else {
    //        outRate = 100;
    //    }
    //}

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

function SetScreenToDefault(isNeedReloadPIC) {
    call_ajax_method_json("/Contract/CTS300_ClearAttach", "", function (result, controls) {
        if (result != null) {
            RefreshAttachList();
        }
    });

    SetViewModeForm(false, isNeedReloadPIC);
    CloseWarningDialog();

    $("#divQuotationTargetInfo").clearForm();
    $("#divProjectInformation").clearForm();
    $("#divCustomerInformation").clearForm();
    $("#divSiteInformation").clearForm();
    $("#divARForm").clearForm();

    $('#divFeeGrid1').hide();
    $('#divFeeGrid2').hide();

    $('#rdoCustomer').removeAttr('disabled');
    $('#rdoSite').removeAttr('disabled');
    $('#rdoContract').removeAttr('disabled');
    $('#rdoProject').removeAttr('disabled');
    $('#rdoQuotation').removeAttr('disabled');

    VaridateCtrl(validateRelevant, null);
    VaridateCtrl(validatePIC, null);

    CalculateGridFee1();
    CalculateGridFee2();

    SetPICToDefault();

    HideAll();

    if (finFirstLoad) {
        if (gridContract != null) {
            //gridContract.clearAll();
            DeleteAllRow(gridContract);
        }

        if (gridUserGroup != null) {
            //gridUserGroup.clearAll();
            DeleteAllRow(gridUserGroup);
        }

        if (gridPIC != null) {
            //gridPIC.clearAll();
            DeleteAllRow(gridPIC);
        }

        picDat = new Array();
        contractDAT = new Array();

//        SetARRelevantType(_arType_Customer);
        SetARRelevantType(_arType_Contract);
        ARRelevantType_changed();

//        SetDueDateDateTime(1);
//        DueDateDeadline_changed();

        hasChange = false;

        DisableAllCommandPane();
    }

    if (finFirstLoad && !hasCheckParameter) {
        RetrieveDataFromParameter();
    } else {
        if (lastCondition != null) {
            BindingConditionFromObj();
        }
    }
}

function SetPICToDefault() {
    $('#OfficeCode').val('');

    $('#divCBBDepartment').html(CTS350_BlankComboBox.replace('{BLANK_ID}', 'DepartmentCode'));
    $('#divCBBARRole').html(CTS350_BlankComboBox.replace('{BLANK_ID}', 'ARRoleCode'));
    $('#divCBBEmployee').html(CTS350_BlankComboBox.replace('{BLANK_ID}', 'EmployeeCode'));

    $('#DepartmentCode').attr('style', 'width: ' + CTS350_DefaultCBBWidth);
    $('#ARRoleCode').attr('style', 'width: ' + CTS350_DefaultCBBWidth);
    $('#EmployeeCode').attr('style', 'width: ' + CTS350_DefaultCBBWidth);
}

function SetARRelevantType(val) {
    var obj = $('input:radio[name=ARRelevantType]');
    obj.filter('[value=' + val + ']').attr('checked', true);
}

function GetARRelevantType() {
    return $('input[name="ARRelevantType"]:checked').val();
}

function SetCustomerRelateType(val) {
    var obj = $('input:radio[name=CustomerRelateType]');
    obj.filter('[value=' + val + ']').attr('checked', true);
}

function GetCustomerRelateType() {
    return $('input[name="CustomerRelateType"]:checked').val();
}

function SetSiteRelateType(val) {
    var obj = $('input:radio[name=SiteRelateType]');
    obj.filter('[value=' + val + ']').attr('checked', true);
}

function GetSiteRelateType() {
    return $('input[name="SiteRelateType"]:checked').val();
}

//function SetDueDateDateTime(val) {
//    var obj = $('input:radio[name=DueDate_DeadLine]');
//    obj.filter('[value=' + val + ']').attr('checked', true);
//}

//function GetDueDateDateTime() {
//    return $('input[name="DueDate_DeadLine"]:checked').val();
//}

function ShowQuotationInfo() {
    $("#divQuotationTargetInfo").show();
}

function ShowProjectInfo() {
    $("#divProjectInformation").show();
}

function ShowCustomerInfo() {
    $("#divCustomerInformation").show();
}

function ShowSiteInfo() {
    $("#divSiteInformation").show();
}

function ShowARForm() {
    $("#divARForm").show();
}

function ShowRegisResult() {
    $('#divResultRegister').show(10);
}

function HideAll() {
    $("#divQuotationTargetInfo").hide();
    $("#divProjectInformation").hide();
    $("#divCustomerInformation").hide();
    $("#divSiteInformation").hide();
    $("#divARForm").hide();
    $('#divResultRegister').hide();
}

function SetViewModeForm(mode, isNeedReloadPIC) {
    if (isNeedReloadPIC == null) {
        isNeedReloadPIC = false;
    }

    if (gridContract != null) {
        var _colRemoveContract = gridContract.getColIndexById("RemoveBtn");

        gridContract.setColumnHidden(_colRemoveContract, mode);
        gridContract.setSizes();

        if (!mode) {
            SetFitColumnForBackAction(gridContract, "TmpColumn");
        }
    }

    if (gridPIC != null) {
        var _colRemovePIC = gridPIC.getColIndexById("RemoveBtn");

        gridPIC.setColumnHidden(_colRemovePIC, mode);
        gridPIC.setSizes();

        if (!mode) {
            SetFitColumnForBackAction(gridContract, "TmpColumn");
        }
    }

    if (gridAttach != null) {
        var _colRemoveAttach = gridAttach.getColIndexById("removeButton");

        gridAttach.setColumnHidden(_colRemoveAttach, mode);
        gridAttach.setSizes();
    }
    
    $("#ARRelevantInfo").SetViewMode(mode);
    $("#divQuotationTargetInfo").SetViewMode(mode);
    $("#divProjectInformation").SetViewMode(mode);
    $("#divCustomerInformation").SetViewMode(mode);
    $("#divSiteInformation").SetViewMode(mode);
    $("#divARForm").SetViewMode(mode);

    if (mode) {
        
        $('#divAttachFile').hide();
        $('#divAttachRemark').hide();
        $('#divSelectPIC').hide();

        $('#divContractQuotationFee').attr('style', 'text-align:right;');
        $('#divContractARFee').attr('style', 'text-align:right;');
        $('#divDepositQuotationFee').attr('style', 'text-align:right;');
        $('#divDepositARFee').attr('style', 'text-align:right;');
        $('#divInstallQuotationFee').attr('style', 'text-align:right;');
        $('#divInstallARFee').attr('style', 'text-align:right;');
        $('#divProductPriceQuotationFee').attr('style', 'text-align:right;');
        $('#divProductPriceARFee').attr('style', 'text-align:right;');
        $('#divInstallFeeQuotationFee').attr('style', 'text-align:right;');
        $('#divInstallFeeARFee').attr('style', 'text-align:right;');
        $('#divSalePriceQuotationFee').attr('style', 'text-align:right;');
        $('#divSalePriceARFee').attr('style', 'text-align:right;');
    } else {
        
        $('#divAttachFile').show();
        $('#divAttachRemark').show();
        $('#divSelectPIC').show();

        if (isNeedReloadPIC) {
            ReLoadingPICGrid(true);
        }

        SetDisableSearch();
    }
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
    //OpenWarningDialog(msgText);
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

function hasContract(contractCode) {
    if ((contractDAT != null) && (contractDAT != undefined)) {
        for (var i = 0; i < contractDAT.length; i++) {
            if ((contractDAT[i].ShortContractCode == contractCode) || (contractDAT[i].UserCode == contractCode)) {
                return true;
            }
        }
    }

    return false;
}

function GetContract(contractCode) {
    if ((contractDAT != null) && (contractDAT != undefined)) {
        for (var i = 0; i < contractDAT.length; i++) {
            if (contractDAT[i].ShortContractCode == contractCode) {
                return contractDAT[i];
            }
        }
    }

    return null;
}

function AddContract(obj) {
    if ((contractDAT == null) || (contractDAT == undefined)) {
        contractDAT = new Array();
    }

    contractDAT[contractDAT.length] = obj;
}

function RemoveContract(contractCode) {
    for (var i = 0; i < contractDAT.length; i++) {
        if (contractDAT[i].ShortContractCode == contractCode) {
            contractDAT.splice(i, 1);
        }
    }
}

function CreateConditionObject() {
    var obj = {
        ARRelevantType: GetARRelevantType(),
        CustomerCode: $('#CustomerCode').val(),
        SiteCode: $('#SiteCode').val(),
        QuotationCode: $('#QuotationCode').val(),
        ProjectCode: $('#ProjectCode').val(),
        ContractCode: $('#UserCode_ContractCode').val()
    };

    return obj;
}

function CreateFormObject() {
    var obj = {
        ARRelevantType: GetARRelevantType(),
        ARTypeCode: $('#ARType').val(),
        ARTitle: $('#ARTitle').val(),
        ARSubTitle: $('#ARSubTitle').val(),
        ARPurpose: $('#ARPurpose').val(),
        IsImportance: $('#ImportanceFlag').is(':checked'),

//        DueDate_Deadline_Type: GetDueDateDateTime(),
//        DueDate_Date: $('#DueDate_Date').val(),
//        DueDate_Time: $('#DueDate_Time').val(),
//        Deadline_Date: $('#Deadline_Date').val(),
//        Deadline_Until: $('#Deadline_Until').val(),

        ContractData: contractDAT,
        PersonInChargeData: picDat,
        Condition: CreateConditionObject(),
        CustomerRelateType: GetCustomerRelateType(),
        SiteRelateType: GetSiteRelateType(),

        ContractFee_QuotationCurrencyType: $('#ContractQuotationFee').NumericCurrencyValue(),
        ContractFee_Quotation: $('#ContractQuotationFee').val(),
        ContractFee_ARCurrencyType: $('#ContractARFee').NumericCurrencyValue(),
        ContractFee_AR: $('#ContractARFee').val(),
        Deposit_QuotationCurrencyType: $('#DepositQuotationFee').NumericCurrencyValue(),
        Deposit_Quotation: $('#DepositQuotationFee').val(),
        Deposit_ARCurrencyType: $('#DepositARFee').NumericCurrencyValue(),
        Deposit_AR: $('#DepositARFee').val(),
        Installation_QuotationCurrencyType: $('#InstallQuotationFee').NumericCurrencyValue(),
        Installation_Quotation: $('#InstallQuotationFee').val(),
        Installation_ARCurrencyType: $('#InstallARFee').NumericCurrencyValue(),
        Installation_AR: $('#InstallARFee').val(),

        ProductPrice_QuotationCurrencyType: $('#ProductPriceQuotationFee').NumericCurrencyValue(),
        ProductPrice_Quotation: $('#ProductPriceQuotationFee').val(),
        ProductPrice_ARCurrencyType: $('#ProductPriceARFee').NumericCurrencyValue(),
        ProductPrice_AR: $('#ProductPriceARFee').val(),
        InstallFee_QuotationCurrencyType: $('#InstallFeeQuotationFee').NumericCurrencyValue(),
        InstallFee_Quotation: $('#InstallFeeQuotationFee').val(),
        InstallFee_ARCurrencyType: $('#InstallFeeARFee').NumericCurrencyValue(),
        InstallFee_AR: $('#InstallFeeARFee').val()

    };

    return obj;
}