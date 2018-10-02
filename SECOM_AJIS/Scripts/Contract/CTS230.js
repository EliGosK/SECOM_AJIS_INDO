/// <reference path="../Base/Master.js" />
/// <reference path="../../Views/Contract/CTS230.cshtml" />

/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>
/// <reference path="../../Scripts/Base/AutoComplete.js"/>
/// <reference path="../Master/Dialog.js" />
/// <reference path="../Common/Dialog.js" />

/// <reference path="../jquery-1.5.1-vsdoc.js" />
/// <reference path="../Base/GridControl.js" />

var isSelected = false;
var OtherGrid;
var SysProdGrid;
var SupportStaffGrid;
var InstGrid;

var AttachDocGrid;
var CompanyNameOld = null;
var ProjRelRow = null;

var doProjectPurchaserData = null;
var dtNewInstrument = null;

var CTS230_gridAttach;
var isInitAttachGrid = false;

var hasAlert = false;
var alertMsg = "";

$(document).ready(function () {

    InitialScreenControls();
    InitButton();
    // InitGrid();

    //================ GRID ATTACH ========================================    
    //$('#frmAttach').attr('src', 'CTS230_Upload');
    // 15/05/2012 
    $('#frmAttach').attr('src', 'CTS230_Upload?k=' + _attach_k);
    CTS230_gridAttach = $("#CTS230_gridAttachDocList").InitialGrid(10, false, "/Contract/CTS230_IntialGridAttachedDocList");
    SpecialGridControl(CTS230_gridAttach, ["removeButton"]);
    BindOnLoadedEvent(CTS230_gridAttach, CTS230_gridAttachBinding);
    $('#frmAttach').load(RefreshAttachList);
    //====================================================================
});


function InitEvent() {
    $('#AttachDocPath').change(function (e) {
        $in = $(this);
        t = $in.val().split("\\");

        $('#Docname').val(t[t.length - 1]);
    });

    $('#PersonInfoSection').hide();

    $("input[id^='sysin']").blur(function () {
        var ctrlName = ($(this).attr('id'));
        ctrlName = ctrlName.substring(5);
        var EmpCode = jQuery.trim($(this).val());

        if (EmpCode != '') {
            call_ajax_method_json('/contract/CTS230_getActiveEmployee', { 'EmpNo': EmpCode }, function (data) {

                $("input[id^='sysout'][id$='" + ctrlName + "']").val(data);
                $(this).val(EmpCode);
            });
            $(this).val(EmpCode);
        } else {
            $("input[id^='sysout'][id$='" + ctrlName + "']").val('');
            $(this).val(EmpCode);
        }


    });
    $("input[id='SupportStaffCode']").blur(function () {
        var EmpCode = $.trim($(this).val());

        if (EmpCode != '') {
            call_ajax_method_json('/contract/CTS230_getSupportStaff', { 'EmpNo': EmpCode }, function (data) {
                $('#SupportBelonging').val(data.BelongingOfficeDepart);
                $("#SupportStaffName").val(data.EmpFullName);
                $(this).val(EmpCode);

            });
            $(this).val(EmpCode);
        } else {
            $("#SupportStaffName").val('');
            $('#SupportBelonging').val('');
            $(this).val(EmpCode);
        }
    });
    $("input[id='InstrumentCode']").blur(function () {
        var InstCode = $.trim($(this).val());

        if (InstCode != '') {
            call_ajax_method_json('/contract/CTS230_GetInstrumentDataForSearch', { 'InstrumentCode': InstCode }, function (data) {
                if ($.trim(data) != '') {
                    dtNewInstrument = data;
                    $("#InstrumentName").val(dtNewInstrument.InstrumentName);
                    //$(this).val(InstCode);
                } else {
                    dtNewInstrument = null;
                    $("#InstrumentName").val('');
                }
            });
            $(this).val(InstCode);
        } else {
            $("#InstrumentName").val('');
            $(this).val(InstCode);
            dtNewInstrument = null;
        }

    });

    $("#frmProjOwner input[type='radio'][name='Own1']").click(function () {
        PO1_event();
    });
    $("#frmProjOwner input[type='radio'][name='Own2']").click(function () {
        PO2_event();
    });

    $("#OverallBudget").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#ReceiveBudget").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#InstrumentQty").BindNumericBox(7, 0, 0, 9999999, 0);
}
function InitButton() {

    $("#btnDeleteAttach").click(function () {
        // var $(
    });
    $("#btnRegNextProject").click(function () {
        call_ajax_method_json("/contract/ClearScreenParam", "", function () {
            InitialScreenControls();
            // Scroll to  top (Section: ProjectInformation)
            document.getElementById('divProjectInformation').scrollIntoView();
        });


    });
    $("#btnAddDocName").click(function () {
        if (CheckFirstRowIsEmpty(AttachDocGrid) == false) {
            var DocCol = AttachDocGrid.getColIndexById("DocumentName");
            var lstDoc = new Array();
            for (var i = 0; i < AttachDocGrid.getRowsNum(); i++) {
                lstDoc.push(AttachDocGrid.cells2(i, DocCol).getValue());
            }
        }
        var FileName = $.trim($('#Docname').val());
        var obj = { "FilePath": $('#AttachDocPath').val(),
            "FileName": FileName,
            "lstFileName": lstDoc
        };
        call_ajax_method_json("/contract/CTS230_AttachFile", obj, function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["AttachDocPath", "Docname"], controls);
            }
            if (result) {
                CheckFirstRowIsEmpty(AttachDocGrid, true);
                AddNewRow(AttachDocGrid, [FileName, ""]);
            }
        });
    });
    $("#ProjRelbtnAdd").click(function () {

        var lstCompanyName = new Array();
        var lstCompanyName_rowID = new Array();

        if (CheckFirstRowIsEmpty(OtherGrid) == false) {
            var CompName = OtherGrid.getColIndexById("CompanyName");
            for (var i = 0; i < OtherGrid.getRowsNum(); i++) {
                lstCompanyName.push(OtherGrid.cells2(i, CompName).getValue());

                lstCompanyName_rowID.push(OtherGrid.getRowId(i));
            }
        }
        var OldRowId = $.trim($("#row_id").val());
        var ProjRelCompName = $.trim($("#ProjRelCompName").val());
        var ProjRelName = $.trim($("#ProjRelName").val());
        var ProjRelTelNo = $.trim($("#ProjRelTelNo").val());
        var ProjRelRemark = $.trim($("#ProjRelRemark").val());

        var ObjAddProjRel = {

            "CompanyName": ProjRelCompName,
            "Name": ProjRelName,
            "ContractTelNo": ProjRelTelNo,
            "Remark": ProjRelRemark,
            "lstCompanyName": lstCompanyName,
            "lstCompanyName_rowID": lstCompanyName_rowID,
            "row_id": OldRowId
        };



        call_ajax_method_json('/contract/CTS230_CheckBeforeAddProjRelate', ObjAddProjRel, function (result, controls) {

            if (controls != undefined) {
                VaridateCtrl(["ProjRelCompName"], controls);
            }

            if (result == true) {
                isSelected = false;
                setDisableGridButton(false);
                CheckFirstRowIsEmpty(OtherGrid, true);

                if (OldRowId == '') {
                    AddNewRow(OtherGrid, [ObjAddProjRel.CompanyName, ObjAddProjRel.Name, ObjAddProjRel.ContractTelNo, ObjAddProjRel.Remark, "", ""]);
                } else {
                    //alert("WWWW");
                    OtherGrid.cells(OldRowId, OtherGrid.getColIndexById("CompanyName")).setValue(ProjRelCompName);
                    OtherGrid.cells(OldRowId, OtherGrid.getColIndexById("Name")).setValue(ProjRelName);
                    OtherGrid.cells(OldRowId, OtherGrid.getColIndexById("TelNo")).setValue(ProjRelTelNo);
                    OtherGrid.cells(OldRowId, OtherGrid.getColIndexById("Remark")).setValue(ProjRelRemark);
                }
                $("#PersonInfoSection").clearForm();
                DisableForNewProjectPerson(false);
            }
        });
    });
    $('#btnSearchInstrument').click(function () {
        $('#dlgBox').OpenCMS170Dialog("CTS230");
    });
    $('#btnInstClear').click(ClearInstSection);
    $('#btnInstAdd').click(function () {

        var InstCode = $.trim($('#InstrumentCode').val());
        var lstInstrumentCode = new Array();

        if (CheckFirstRowIsEmpty(InstGrid) == false) {
            var codeCol = InstGrid.getColIndexById("InstrumentCode");
            for (var i = 0; i < InstGrid.getRowsNum(); i++) {
                lstInstrumentCode.push(InstGrid.cells2(i, codeCol).getValue());
            }
        }

        var objInstAdd = {
            'InstrumentCode': InstCode,
            'InstrumentQty': ($('#InstrumentQty').NumericValue()),
            'InstrumentName': $.trim($('#InstrumentName').val()),
            'dtNewInstrument': dtNewInstrument,
            'lstInstrumentCode': lstInstrumentCode
        };

        //alert($('#InstrumentQty').val());

        call_ajax_method_json('/contract/CTS230_CheckBeforeAddInstrument', objInstAdd, function (result, controls) {

            if (controls != undefined) {
                VaridateCtrl(["InstrumentCode", "InstrumentQty"], controls);
            }

            if (result == true) {
                CheckFirstRowIsEmpty(InstGrid, true);
                if (dtNewInstrument != null)
                {
                    //Modify by Jutarat A. on 28112013
                    //AddNewRow(InstGrid, [dtNewInstrument.InstrumentCode, dtNewInstrument.InstrumentName,
                    //"", "", dtNewInstrument.LineUpTypeCode]);
                    AddNewRow(InstGrid, [ConvertBlockHtml(dtNewInstrument.InstrumentCode)
                                        , ConvertBlockHtml(dtNewInstrument.InstrumentName)
                                        , "", "", dtNewInstrument.LineUpTypeCode]);
                    //End Modify
                }
                var qtyCol = InstGrid.getColIndexById("InstrumentQty");

                var row_idx = InstGrid.getRowsNum() - 1;
                var row_id = InstGrid.getRowId(row_idx);

                //In//stGrid.cells2(row_idx, qtyCol).setValue(GenerateNumericBox("Qty", row_id, $("#InstrumentQty").val(), true));

                GenerateNumericBox2(InstGrid, "Qty", row_id, "InstrumentQty", $("#InstrumentQty").val(), 6, 0, 0, 999999, false, true);

                GenerateRemoveButton(InstGrid, "btnRemoveInst", row_id, "Remove", true);

                InstGrid.setSizes();




                BindGridButtonClickEvent("btnRemoveInst", row_id, function (rid) {
                    DeleteRow(InstGrid, rid);
                });

                ClearInstSection();
            }
        });
    });
    $('#btnNewOtherProject').click(function () {

        DisableForNewProjectPerson(true);
        isSelected = true;
        setDisableGridButton(true);
    });
    $('#ProjRelbtnCancel').click(function () {

        DisableForNewProjectPerson(false);
        CloseWarningDialog();
        $('#PersonInfoSection').clearForm();
        setDisableGridButton(false);

    });
    $('#btnCancelPsnInfo').click(ClearPersonalInfo);
    $("#btnCPRetrieve").click(function () {
        RetrieveContractCustomerData();
    });
    $("#btnCPSearchCustomer").click(function () {
        SearchCustomerData(function (data) {
            var code = data.CustCode;
            if (code == undefined)
                code = "";

            $("#CPSearchCustCode").val(code);
            RetrieveContractCustomerData();
        });
    });
    $("#btnCPClearCustomer").click(function () {
        clear_contract_customer_click();
    });
    $('#SupportAdd').click(function () {

        var lstStaffCode = new Array();
        if (CheckFirstRowIsEmpty(SupportStaffGrid) == false) {
            var StaffCodeCol = SupportStaffGrid.getColIndexById("EmpNo");
            for (var i = 0; i < SupportStaffGrid.getRowsNum(); i++) {
                lstStaffCode.push(SupportStaffGrid.cells2(i, StaffCodeCol).getValue());
            }
        }
        var objStaff = {
            StaffCode: $.trim($("#SupportStaffCode").val()),
            Remark: $.trim($("#SupportREmark").val()),
            lstStaffCode: lstStaffCode
        };
        call_ajax_method_json("/contract/CTS230_ChackBeforeAddSupportStaff", objStaff, function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["SupportStaffCode"], controls);
            }
            if (result) {
                CheckFirstRowIsEmpty(SupportStaffGrid, true);
                AddNewRow(SupportStaffGrid,
                    [$.trim($("#SupportStaffCode").val()),
                     $.trim($("#SupportStaffName").val()),
                      $.trim($("#SupportBelonging").val()),
                       $.trim($("#SupportREmark").val())]);

                ClearSupportStaff();
            }
        });



    });
    $('#SupportClear').click(ClearSupportStaff);
    $('#SysProdAdd').click(function () {
        var text = "";
        if ($.trim($('#SysProductName').val()) != '') {
            text = $.trim($("#SysProductName option:selected").text());
        }
        //      var InstCode = $.trim($('#InstrumentCode').val());
        var lstSystemProduct = new Array();

        if (CheckFirstRowIsEmpty(SysProdGrid) == false) {
            var SysProductNameCol = SysProdGrid.getColIndexById("SysProductName");
            for (var i = 0; i < SysProdGrid.getRowsNum(); i++) {
                lstSystemProduct.push(SysProdGrid.cells2(i, SysProductNameCol).getValue());
            }
        }

        var objSysProdAdd = {
            'SystemProductName': text,
            "lstSystemProductName": lstSystemProduct
        };

        call_ajax_method_json("/contract/CTS230_CheckBeforeAddSystemProduct", objSysProdAdd, function (result, controls) {
            if (controls != undefined)
                VaridateCtrl(["SysProductName"], controls);


            if (result) {
                CheckFirstRowIsEmpty(SysProdGrid, true);
                AddNewRow(SysProdGrid, [$("#SysProductName option:selected").text(), '', $("#SysProductName").val()]);
            }
        });

    });
    $('#btnCPNewEditCustomer').click(new_edit_contract_customer_click);
}
function InitGrid() {
    if ($("#OtherProjectGridPlane").length > 0) {
        OtherGrid = $("#OtherProjectGridPlane").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, false, '/contract/CTS230_Other');
        OtherGrid.attachEvent("onBeforeSelect", function (new_row, old_row) {

            return !isSelected;
        });
        OtherGrid.attachEvent("onBeforeSorting", function (ind, type, direction) {

            return !isSelected;
        });
        OtherGrid.attachEvent("onBeforePageChanged", function (ind, count) {

            return !isSelected;



        });
        BindOnLoadedEvent(OtherGrid, function () {

            for (var i = 0; i < OtherGrid.getRowsNum(); i++) {
                var row_id = OtherGrid.getRowId(i);
                GenerateRemoveButton(OtherGrid, "btnRemoveProjRel", row_id, "Remove", true);
                GenerateEditButton(OtherGrid, "btnEditProjRel", row_id, "Edit", true);

                BindGridButtonClickEvent("btnRemoveProjRel", row_id, function (rid) {
                    DeleteRow(OtherGrid, rid);
                });

                BindGridButtonClickEvent("btnEditProjRel", row_id, function (rid) {
                    $('#PersonInfoSection').show();
                    DisableForNewProjectPerson(true);
                    $("#row_id").val(rid);
                    $("#CompNameOld").val(OtherGrid.cells(rid, OtherGrid.getColIndexById("CompanyName")).getValue());

                    $("#ProjRelCompName").val(OtherGrid.cells(rid, OtherGrid.getColIndexById("CompanyName")).getValue());
                    $("#ProjRelName").val(OtherGrid.cells(rid, OtherGrid.getColIndexById("Name")).getValue());
                    $("#ProjRelTelNo").val(OtherGrid.cells(rid, OtherGrid.getColIndexById("TelNo")).getValue());
                    $("#ProjRelRemark").val(OtherGrid.cells(rid, OtherGrid.getColIndexById("Remark")).getValue());
                    setDisableGridButton(true);
                });
            }
            OtherGrid.setSizes();
        });

        SpecialGridControl(OtherGrid, ["Edit", "Remove"]);
    }

    if ($("#SystemProductGridPlane").length > 0) {
        SysProdGrid = $("#SystemProductGridPlane").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, false, '/contract/CTS230_SystemProduct');

        BindOnLoadedEvent(SysProdGrid, function () {

            for (var i = 0; i < SysProdGrid.getRowsNum(); i++) {
                var row_id = SysProdGrid.getRowId(i);
                GenerateRemoveButton(SysProdGrid, "btnRemoveSysProd", row_id, "Remove", true);
                BindGridButtonClickEvent("btnRemoveSysProd", row_id, function (rid) {
                    DeleteRow(SysProdGrid, rid);
                });
            }
            SysProdGrid.setSizes();
        });

        SpecialGridControl(SysProdGrid, ["Remove"]);
    }

    if ($("#SupportStaffGridPlane").length > 0) {
        SupportStaffGrid = $("#SupportStaffGridPlane").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, false, '/contract/CTS230_SupportStaff');

        BindOnLoadedEvent(SupportStaffGrid, function () {

            for (var i = 0; i < SupportStaffGrid.getRowsNum(); i++) {
                var row_id = SupportStaffGrid.getRowId(i);
                GenerateRemoveButton(SupportStaffGrid, "btnRemoveSuppStaff", row_id, "Remove", true);
                BindGridButtonClickEvent("btnRemoveSuppStaff", row_id, function (rid) {
                    DeleteRow(SupportStaffGrid, rid);
                });
            }
            SupportStaffGrid.setSizes();
        });

        SpecialGridControl(SupportStaffGrid, ["Remove"]);
    }

    if ($("#InstrumentGridPlane").length > 0) {

        InstGrid = $("#InstrumentGridPlane").InitialGrid(0, false, '/contract/CTS230_ExpectInstrumen');
        SpecialGridControl(InstGrid, ["InstrumentQty", "Remove"]);
        BindOnLoadedEvent(InstGrid, function () {

            for (var i = 0; i < InstGrid.getRowsNum(); i++) {
                var row_id = InstGrid.getRowId(i);
                var numGridID = "Qty";
                var qtyCol = InstGrid.getColIndexById("InstrumentQty");
                var val = GetValueFromLinkType(InstGrid, i, qtyCol);
                //                InstGrid.cells2(i, qtyCol).setValue(GenerateNumericBox(numGridID, row_id, val, true));

                //                var txt = "#" + GenerateGridControlID(numGridID, row_id);


                //                $(txt).BindNumericBox(4, 0, 0, 9999, true);
                //                if ($(txt).val() != undefined) {
                //                    $(txt).setComma();
                //                }

                GenerateNumericBox2(InstGrid, numGridID, row_id, qtyCol, val, 6, 0, 0, 999999, false, true);


                GenerateRemoveButton(InstGrid, "btnRemoveInst", row_id, "Remove", true);
                BindGridButtonClickEvent("btnRemoveInst", row_id, function (rid) {
                    DeleteRow(InstGrid, rid);
                });
            }
            InstGrid.setSizes();
        });


    }

    if ($("#AttachedDocListGridPlane").length > 0) {
        AttachDocGrid = $("#AttachedDocListGridPlane").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, false, '/contract/CTS230_AttachedDoc');
        BindOnLoadedEvent(AttachDocGrid, function () {

            for (var i = 0; i < AttachDocGrid.getRowsNum(); i++) {
                var row_id = AttachDocGrid.getRowId(i);
                GenerateRemoveButton(AttachDocGrid, "btnDeleteDoc", row_id, "Delete", true);
                BindGridButtonClickEvent("btnDeleteDoc", row_id, function (rid) {
                    DeleteRow(AttachDocGrid, rid);
                });
            }
            AttachDocGrid.setSizes();
        });
        SpecialGridControl(AttachDocGrid, ["Delete"]);
    }
}


function setDisableGridButton(isDisable) {
    var isEnable = !isDisable;
    if ($("#OtherProjectGridPlane").length > 0) {
        if (OtherGrid.getRowsNum() != 0) {
            for (var i = 0; i < OtherGrid.getRowsNum(); i++) {
                var row_id = OtherGrid.getRowId(i);
                EnableGridButton(OtherGrid, "btnEditProjRel", row_id, "Edit", isEnable);
                EnableGridButton(OtherGrid, "btnRemoveProjRel", row_id, "Remove", isEnable);
            }
        }
        OtherGrid.attachEvent("onAfterSorting"
            , function (index, type, direction) {
                for (var i = 0; i < OtherGrid.getRowsNum(); i++) {
                    var row_id = OtherGrid.getRowId(i);
                    EnableGridButton(OtherGrid, "btnEditProjRel", row_id, "Edit", isEnable);
                    EnableGridButton(OtherGrid, "btnRemoveProjRel", row_id, "Remove", isEnable);
                }
            }
        );
    }
}
function ConfirmState() {
    $("#divAttachFrame").hide();
    $("#divAttachRemark").hide();
    $("#SupportInput").hide();
    $("#InstData").hide();
    $("#owner1").hide();
    $("#owner2").hide();
    $("#SystemInput").hide();
    $("#divSpecifyContractTarget").hide();
    colInx = CTS230_gridAttach.getColIndexById("removeButton")
    CTS230_gridAttach.setColumnHidden(colInx, true);

    ProjectNameSection_setViewMode(true);
    ProjectOwnerSection_setViewMode(true);
    divContractTargetPurchaserInfo_setViewMode(true);
    ProjectManCompSection_setViewMode(true);
    ProjectManCompSectionMini_setViewMode(true);
    ProjectPersonSection_setViewMode(true);
    Page2_setViewMode(true);

    SetRegisterCommand(false, function () { });
    SetResetCommand(false, null);
    SetConfirmCommand(true, function () {
        call_ajax_method_json('/contract/CTS230_ConfirmRegisterProject/', "", function (res) {
            if (res != "error" && res != null) {
                var obj = { module: "Common", code: "MSG0046" };
                call_ajax_method("/Shared/GetMessage", obj, function (result) {
                    OpenInformationMessageDialog(result.Code, result.Message, function () {
                        $("#resOfRegSection").show();
                        setTimeout("document.getElementById('resOfRegSection').scrollIntoView();", 1000);
                        SetConfirmCommand(false, "");
                        SetBackCommand(false, null);
                    });
                });
                $("#ProjectCode").val(res);
            } else {

            }
        });

    });
    SetBackCommand(true, back_Command);
}
function back_Command() {
    ProjectNameSection_setViewMode(false);
    ProjectOwnerSection_setViewMode(false);
    divContractTargetPurchaserInfo_setViewMode(false);
    ProjectManCompSection_setViewMode(false);
    ProjectManCompSectionMini_setViewMode(false);
    ProjectPersonSection_setViewMode(false);
    Page2_setViewMode(false);
    $("#divSpecifyContractTarget").show();
    $("#owner1").show();
    $("#owner2").show();
    $("#SystemInput").show();
    $("#SupportInput").show();
    $("#InstData").show();
    SetConfirmCommand(false, null);
    SetRegisterCommand(true, register_Command);
    SetBackCommand(false, null);
    SetResetCommand(true, command_reset_click);

    $("#divAttachFrame").show();
    $("#divAttachRemark").show();
    CTS230_gridAttach.setColumnHidden(colInx, false);
    SetFitColumnForBackAction(CTS230_gridAttach, "TmpColumn");
}
function register_Command() {
    var own1 = $("#frmProjOwner input[name='Own1']:checked").val();
    if (own1 == "diff")
        own1 = false;
    else
        own1 = true;

    var own2 = $("#frmProjOwner input[name='Own2']:checked").val();
    if (own2 == "diff")
        own2 = false;
    else
        own2 = true;
    /* --- Set Parameter --- */
    var objTbt_Project = {// ============= doTbt_project ===========
        ProjectName: $("#ProjectName").val(),
        ProjectAddress: $("#ProjectRepresentAddr").val(),

        Owner1Flag: own1,
        Owner1NameEN: $("#po1NameEn").val(),
        Owner1NameLC: $("#po1NameLC").val(),

        Owner2Flag: own2,
        Owner2NameEN: $("#po2NameEn").val(),
        Owner2NameLC: $("#po2NameLC").val(),

        ManagementCompanyNameEN: $("#ProjManaCompEn").val(),
        ManagementCompanyNameLC: $("#ProjManaCompLC").val(),

        OverallBudgetAmountCurrencyType: $("#OverallBudget").NumericCurrencyValue(),
        OverallBudgetAmount: $("#OverallBudget").NumericValue(),
        ReceivedBudgetAmountCurrencyType: $("#ReceiveBudget").NumericCurrencyValue(),
        ReceivedBudgetAmount: $("#ReceiveBudget").NumericValue(),

        HeadSalesmanEmpNo: $("#sysinHeadSalesmanEmpNo").val(),
        ProjectManagerEmpNo: $("#sysinProjectManagerEmpNo").val(),
        ProjectSubManagerEmpNo: $("#sysinProjectSubManagerEmpNo").val(),
        SecurityPlanningChiefEmpNo: $("#sysinSecurityPlanningChiefEmpNo").val(),
        InstallationChiefEmpNo: $("#sysinInstallationChiefEmpNo").val(),
        Memo: $("#MemoSecom").val()

        //============= End doTbt_project ===========

    };
    //--------------------- Expect Intrument ----------------------------
    var dtInst = new Array();
    var InstCodeCol = InstGrid.getColIndexById("InstrumentCode");
    var InstQtyCol = InstGrid.getColIndexById("InstrumentQty");
    var LineUpTypeCol = InstGrid.getColIndexById("LineUpTypeCode");
    if (!CheckFirstRowIsEmpty(InstGrid)) {
        for (var i = 0; i < InstGrid.getRowsNum(); i++) {
            var row_uid = InstGrid.getRowId(i);
            var txt = "#" + GenerateGridControlID("Qty", row_uid);
            var doInst = {
                ProjectCode: "",
                InstrumentCode: InstGrid.cells2(i, InstCodeCol).getValue(),
                InstrumentQty: $(txt).NumericValue(),
                LineUpTypeCode: InstGrid.cells2(i, LineUpTypeCol).getValue()
            };
            dtInst.push(doInst);
            //alert(val);
        }
    }
    //----------------------- Other grid ----------------------
    var dtProjOtherRel = new Array();
    if (!CheckFirstRowIsEmpty(OtherGrid)) {
        for (var i = 0; i < OtherGrid.getRowsNum(); i++) {
            var doProjOtherRel = {
                ProjectCode: "",
                SequenceNo: i + 1,
                CompanyName: OtherGrid.cells2(i, OtherGrid.getColIndexById("CompanyName")).getValue(),
                Name: OtherGrid.cells2(i, OtherGrid.getColIndexById("Name")).getValue(),
                TelNo: OtherGrid.cells2(i, OtherGrid.getColIndexById("TelNo")).getValue(),
                Remark: OtherGrid.cells2(i, OtherGrid.getColIndexById("Remark")).getValue()
            };
            dtProjOtherRel.push(doProjOtherRel);
        }
    }
    //----------------------- Support staff  grid ----------------------
    var dtSupportStaff = new Array();
    if (!CheckFirstRowIsEmpty(SupportStaffGrid)) {
        for (var i = 0; i < SupportStaffGrid.getRowsNum(); i++) {
            var doSupportStaff = {
                ProjectCode: "",
                EmpNo: SupportStaffGrid.cells2(i, SupportStaffGrid.getColIndexById("EmpNo")).getValue(),
                Remark: SupportStaffGrid.cells2(i, SupportStaffGrid.getColIndexById("Remark")).getValue()
            };
            dtSupportStaff.push(doSupportStaff);
        }
    }
    //----------------------- System Product grid ----------------------
    var dtSystemProduct = new Array();
    if (!CheckFirstRowIsEmpty(SysProdGrid)) {
        for (var i = 0; i < SysProdGrid.getRowsNum(); i++) {
            var doSysProd = {
                ProjectCode: "",
                ProductCode: SysProdGrid.cells2(i, SysProdGrid.getColIndexById("SysProductCode")).getValue()
            };
            dtSystemProduct.push(doSysProd);
        }
    }
    if (doProjectPurchaserData == null) {
        doProjectPurchaserData = { ProjectCode: "" };
    }


    var doRegProjectData = {
        doTbt_Project: objTbt_Project,
        doTbt_ProjectExpectedInstrumentDetail: dtInst,
        doTbt_ProjectOtherRalatedCompany: dtProjOtherRel,
        doTbt_ProjectPurchaserCustomer: doProjectPurchaserData,
        doTbt_ProjectSupportStaffDetails: dtSupportStaff,
        doTbt_ProjectSystemDetails: dtSystemProduct
    }


    call_ajax_method_json("/contract/CTS230_RegisterNewProject", doRegProjectData, function (result, controls) {
        VaridateCtrl(["ProjectName", "ProjectRepresentAddr",
        "sysinHeadSalesmanEmpNo", "sysinProjectManagerEmpNo",
        "sysinProjectSubManagerEmpNo", "sysinSecurityPlanningChiefEmpNo"
        , "sysinInstallationChiefEmpNo", "CPSearchCustCode"], null);
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["ProjectName", "ProjectRepresentAddr",
        "sysinHeadSalesmanEmpNo", "sysinProjectManagerEmpNo",
        "sysinProjectSubManagerEmpNo", "sysinSecurityPlanningChiefEmpNo"
        , "sysinInstallationChiefEmpNo", "CPSearchCustCode"], controls);
            /* --------------------- */

            return;
        }

        if (result) {
            ConfirmState();
            
        }
    });
}
function InitialScreenControls() {
    SetRegisterCommand(true, register_Command);
    SetResetCommand(true, command_reset_click);
    InitEvent();
    InitGrid();
    ProjectNameSection_setViewMode(false);
    ProjectOwnerSection_setViewMode(false);
    divContractTargetPurchaserInfo_setViewMode(false);
    ProjectManCompSection_setViewMode(false);
    ProjectManCompSectionMini_setViewMode(false);
    ProjectPersonSection_setViewMode(false);
    Page2_setViewMode(false);

    $("#ProjectNameSection").clearForm();
    $('#ProjectOwnerSection').clearForm();
    $('#divContractTargetPurchaserInfo').clearForm();
    $('#ProjectManCompSection').clearForm();
    $('#ProjectManCompSectionMini').clearForm();
    $('#ProjectPersonSection').clearForm();
    $('#Page2').clearForm();
    $("#resOfRegSection").clearForm();

    $("#CPSearchCustCode").SetReadOnly(false);
    $("#btnCPSearchCustomer").SetDisabled(false);
    $("#btnCPRetrieve").SetDisabled(false);
    $("#btnCPNewEditCustomer").SetDisabled(false);

    $("#chkDiffOwn1").attr("checked", true);
    $("#chkDiffOwn2").attr("checked", true);

    PO1_event();
    PO2_event();

    $("#resOfRegSection").show();
    $("#divSpecifyContractTarget").show();
    $("#SupportInput").show();
    $("#InstData").show();
    $("#SystemInput").show();
    if (CTS230_gridAttach != undefined) {
        DeleteAllRow(CTS230_gridAttach);
        colInx = CTS230_gridAttach.getColIndexById("removeButton")
        CTS230_gridAttach.setColumnHidden(colInx, false);
    }

    ProjRelRow = null;
    CompanyNameOld = null;
    doProjectPurchaserData = null;
    dtNewInstrument = null;

    $("#resOfRegSection").hide();

    $("#ProjectName").focus();

    
    

    $("#divAttachFrame").show();
    $("#divAttachRemark").show();
    $("#owner1").show();
    $("#owner2").show();
    
}
function ProjectNameSection_setViewMode(IsViewMode) {
    $("#ProjectNameSection").SetViewMode(IsViewMode);
}
function ProjectOwnerSection_setViewMode(IsViewMode) {
    $('#ProjectOwnerSection').SetViewMode(IsViewMode);
}
function divContractTargetPurchaserInfo_setViewMode(IsViewMode) {
    $('#divContractTargetPurchaserInfo').SetViewMode(IsViewMode);
}
function ProjectManCompSection_setViewMode(IsViewMode) {
    $('#ProjectManCompSection').SetViewMode(IsViewMode);
}
function ProjectManCompSectionMini_setViewMode(IsViewMode) {
    $('#ProjectManCompSectionMini').SetViewMode(IsViewMode);
}
function ProjectPersonSection_setViewMode(IsViewMode) {
    $('#ProjectPersonSection').SetViewMode(IsViewMode);
    if (IsViewMode) {
        if (OtherGrid != undefined) {
            var removeCol = OtherGrid.getColIndexById("Remove");
            var EditCol = OtherGrid.getColIndexById("Edit");
            OtherGrid.setColumnHidden(removeCol, true);
            OtherGrid.setColumnHidden(EditCol, true);
            OtherGrid.setSizes();
        }
    } else {
        if (OtherGrid != undefined) {
            var removeCol = OtherGrid.getColIndexById("Remove");
            var EditCol = OtherGrid.getColIndexById("Remove");
            OtherGrid.setColumnHidden(removeCol, false);
            OtherGrid.setColumnHidden(EditCol, false);
            OtherGrid.setSizes();
        }
    }

}
function Page2_setViewMode(IsViewMode) {
    $('#Page2').SetViewMode(IsViewMode);
    if (IsViewMode) {
        if (SysProdGrid != undefined) {
            var removeCol = SysProdGrid.getColIndexById("Remove");
            SysProdGrid.setColumnHidden(removeCol, true);
            SysProdGrid.setSizes();
        }
        if (SupportStaffGrid != undefined) {
            var removeCol = SupportStaffGrid.getColIndexById("Remove");
            SupportStaffGrid.setColumnHidden(removeCol, true);
            SupportStaffGrid.setSizes();
        }
        if (InstGrid != undefined) {
            var removeCol = InstGrid.getColIndexById("Remove");
            InstGrid.setColumnHidden(removeCol, true);
            InstGrid.setSizes();
        }

    } else {
        if (SysProdGrid != undefined) {
            var removeCol = SysProdGrid.getColIndexById("Remove");
            SysProdGrid.setColumnHidden(removeCol, false);
            SysProdGrid.setSizes();
        }
        if (SupportStaffGrid != undefined) {
            var removeCol = SupportStaffGrid.getColIndexById("Remove");
            SupportStaffGrid.setColumnHidden(removeCol, false);
            SupportStaffGrid.setSizes();
        }
        if (InstGrid != undefined) {
            var removeCol = InstGrid.getColIndexById("Remove");
            InstGrid.setColumnHidden(removeCol, false);
            InstGrid.setSizes();
        }

    }
}
function command_reset_click() {
    /* --- Get Message --- */
    /* ------------------- */
    var obj = {
        module: "Common",
        code: "MSG0038"
    };
    call_ajax_method("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
        function () {

            //call_ajax_method_json("/contract/CTS230_RemoveNotCompleteAttachFile", "", function () { })


            $("input[type='text']").val('');
            InitialScreenControls();
            // Scroll to  top (Section: ProjectInformation)
            document.getElementById('divProjectInformation').scrollIntoView();
            $("#UploadIframe").attr("src", $("#UploadIframe").attr("src"));
        },
        null);
    });
    /* ------------------- */
}
function ClearPersonalInfo() {

    $("#PersonInfoSection :input[type='text']").val('');
    $("#PersonInfoSection :input[type='text']").attr('disabled', true);

}
function ClearInstSection() {
    $("#InstData :input[type='text']").val('');
    $("#InstData").clearForm();
    dtNewInstrument = null;
}
function ClearSupportStaff() {
    CloseWarningDialog();
    $('#SupportStaffCode').val('');
    $('#SupportStaffName').val('');
    $('#SupportBelonging').val('');
    $('#SupportREmark').val('');
    VaridateCtrl(["SupportStaffCode"], null);

}

function DisableForNewProjectPerson(IsDisable) {

    if (IsDisable) {
        $('#PersonInfoSection').show();
        // $("#PersonInfoSection :input[type='text']").removeAttr('disabled');
        $('input[id^="ProjRel"').val('');
    } else {
        $('#PersonInfoSection').hide();
        $("#row_id").val('');

        // $("#PersonInfoSection :input[type='text']").attr('disabled', true);
        $('input[id^="ProjRel"]').val('');
    }

    if (!CheckFirstRowIsEmpty(CTS230_gridAttach))
    {
//        colInx = CTS230_gridAttach.getColIndexById("downloadButton")
//        CTS230_gridAttach.setColumnHidden(colInx, IsDisable);
        colInx = CTS230_gridAttach.getColIndexById("removeButton")
        CTS230_gridAttach.setColumnHidden(colInx, IsDisable);

        if (!IsDisable) {
            SetFitColumnForBackAction(CTS230_gridAttach, "TmpColumn");            
        }        
    }

    if (!IsDisable) {      
        $("#divAttachFrame").show();
        $("#divAttachRemark").show();
    }
    else {
        $("#divAttachFrame").hide();
        $("#divAttachRemark").hide();
    }

    IsDisable = !IsDisable;
    $('#ProjectNameSection').SetEnableViewIfReadonly(IsDisable);
    $('#ProjectOwnerSection').SetEnableViewIfReadonly(IsDisable);
    $('#divContractTargetPurchaserInfo').SetEnableViewIfReadonly(IsDisable);
    $('#ProjectManCompSection').SetEnableViewIfReadonly(IsDisable);
    $('#ProjectManCompSectionMini').SetEnableViewIfReadonly(IsDisable);
    $('#ProjectPersonSection').SetEnableViewIfReadonly(IsDisable);
    $('#Page2').SetEnableViewIfReadonly(IsDisable);

}
var searchFunction = null;
function SearchCustomerData(func) {
    searchFunction = func;
    $("#dlgBox").OpenCMS250Dialog("CTS230");
}

function CMS250Response(result) {
    $("#dlgBox").CloseDialog();

    if (typeof (searchFunction) == "function") {
        searchFunction(result.CustomerData);
    }
}
function RetrieveContractCustomerData() {
    var obj = {
        CustCode: $("#CPSearchCustCode").val(),
        CustType: 1
    };
    /* --------------------- */

    /* --- Call Event --- */
    /* ------------------ */
    call_ajax_method_json("/contract/CTS230_RetrieveCustomer", obj, function (result, controls) {

        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["CPSearchCustCode"], ["CPSearchCustCode"]);
            /* --------------------- */
            return;
        }
        if (result != null) {
            /* --- Set Data --- */
            /* ---------------- */

            //================== TRS ========================
            //SetCustomer(result, GetCustomer(function (result) {
            //    doProjectPurchaserData = result;
            //}));

            SetCustomer(result, function (result) {
                GetCustomer(function (result) {
                    doProjectPurchaserData = result;
                })
            });
            
            //===============================================

            ViewContractCustomerData(result);
            /* ---------------- */

            
        }
    });



    // RetrieveCustomerData();
}
function ViewContractCustomerData(contractData) {
    if (contractData != null) {
        $("#divContractTargetPurchaserInfo").clearForm();

        /* --- Fill Data to Control --- */
        /* ---------------------------- */
        $("#divContractTargetPurchaserInfo").bindJSON_Prefix("CP", contractData);
        $("#CPSearchCustCode").val(contractData.CustCodeShort);
        /* ---------------------------- */

        /* --- Disable Control --- */
        /* ----------------------- */
        $("#CPSearchCustCode").attr("readonly", true);
        $("#btnCPRetrieve").attr("disabled", true);
        $("#btnCPSearchCustomer").attr("disabled", true);

        if ($("#CPCustCodeShort").val() != "")
            $("#btnCPNewEditCustomer").attr("disabled", true);
        /* ----------------------- */

        PO1_event();
        PO2_event();
    }
}
function InitialContractCustomerSection() {
    /* --- Enable Control --- */
    $("#CPSearchCustCode").removeAttr("readonly");
    $("#btnCPRetrieve").removeAttr("disabled");
    $("#btnCPSearchCustomer").removeAttr("disabled");
    $("#btnCPNewEditCustomer").removeAttr("disabled");

    /* --- Clear Data from Control --- */
    $("#divContractTargetPurchaserInfo").clearForm();
    //ContractBranchInfoVisible(false);
}
function clear_contract_customer_click() {

    InitialContractCustomerSection();
    doProjectPurchaserData = null;
    PO1_event();
    PO2_event();
}

function new_edit_contract_customer_click() {
    $("#dlgBox").OpenMAS050Dialog("QUS020");
}
function GetCustomer(func) {
    call_ajax_method_json("/Contract/CTS230_GetCustomer", "", function (result) {
        if (typeof (func) == "function")
            func(result);
    });
}
function SetCustomer(object, func) {
    call_ajax_method_json("/contract/CTS230_SetDoCustomer", object, function (result) {
        if (result == true) {
            if (typeof (func) == "function")
                func();
        }
    });
}
function MAS050Response(doCustomer) {
    $("#dlgBox").CloseDialog();

    /* --- Reset Control --- */
    /* --------------------- */
    $("#CPSearchCustCode").val("");
    /* --------------------- */
    /* --- Set Data --- */
    /* ---------------- */
    doProjectPurchaserData = doCustomer;
    SetCustomer(doCustomer, function () {
        ViewContractCustomerData(doCustomer);
    });
    /* ---------------- */
}
function MAS050Object() {
    return {
        doCustomer: doProjectPurchaserData
    };
}
function PO1_event() {
    var chk = $("#frmProjOwner input[name='Own1']:checked").val();
    //alert(chk);
    if (chk == "same") {
        $("input[name^='po1']").clearForm();
        $('#po1NameEn').val($('#CPCustFullNameEN').val());
        $('#po1NameLC').val($('#CPCustFullNameLC').val());
        $("input[name^='po1']").attr("readonly", true);
    }
    else {
        // $("input[name^='po1']").clearForm();
        $("input[name^='po1']").removeAttr("readonly");
    }
}
function PO2_event() {
    var chk = $("#frmProjOwner input[name='Own2']:checked").val();
    //alert(chk);
    if (chk == "same") {
        $("input[name^='po2']").clearForm();
        $('#po2NameEn').val($('#CPCustFullNameEN').val());
        $('#po2NameLC').val($('#CPCustFullNameLC').val());
        $("input[name^='po2']").attr("readonly", true);
    }
    else {
        //  $("input[name^='po2']").clearForm();
        $("input[name^='po2']").removeAttr("readonly");
    }
}
function CMS170Object() {
    return { bExpTypeHas: true,
        bExpTypeNo: false,
        bProdTypeSale: false,
        bProdTypeAlarm: false,
        bInstTypeGen: true,
        bInstTypeMonitoring: false,
        bInstTypeMat: false
    };
}
function CMS170Response(dtNewInst) {

    dtNewInstrument = dtNewInst;
    $("#InstrumentCode").val(dtNewInstrument.InstrumentCode);
    $("#InstrumentName").val(dtNewInstrument.InstrumentName);
    $("#InstrumentQty").focus();
    $("#dlgBox").CloseDialog();

}
function AlertIframe(MsgCode, msg) {
    OpenErrorMessageDialog(MsgCode, msg);
}

function btnRemoveAttach_click(row_id) {
    var _colID = CTS230_gridAttach.getColIndexById("AttachFileID");
    var _targID = CTS230_gridAttach.cells(row_id, _colID).getValue();

    var obj = {
        AttachID: _targID
    };
    call_ajax_method_json("/Contract/CTS230_RemoveAttach", obj, function (result, controls) {
        if (result != null) {
            RefreshAttachList();

        }
        CheckFirstRowIsEmpty(CTS230_gridAttach, true);
    });
}

function doAlertWithMessage(msgCode, msgText) {
    hasAlert = true;
    alertMsg = msgText;
}

function RefreshAttachList() {

    if (CTS230_gridAttach != null) {
        $('#CTS230_gridAttachDocList').LoadDataToGrid(CTS230_gridAttach, 0, false, "/Contract/CTS230_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, function () {
            if (hasAlert) {
                hasAlert = false;
                OpenWarningDialog(alertMsg);
            }
            // 15/05/2012
            else {
                if (CheckFirstRowIsEmpty(CTS230_gridAttach) == false) {
                    CTS230_gridAttachBinding();
                }
            }
        }, null)
    }
}

function CTS230_gridAttachBinding() {
    if (isInitAttachGrid) {
        var _colRemoveBtn = CTS230_gridAttach.getColIndexById("removeButton");

        for (var i = 0; i < CTS230_gridAttach.getRowsNum(); i++) {
            var row_id = CTS230_gridAttach.getRowId(i);
            GenerateRemoveButton(CTS230_gridAttach, "btnRemoveAttach", row_id, "removeButton", true);
            BindGridButtonClickEvent("btnRemoveAttach", row_id, btnRemoveAttach_click);
        }
    } else {
        isInitAttachGrid = true;
    }

    CTS230_gridAttach.setSizes();
}