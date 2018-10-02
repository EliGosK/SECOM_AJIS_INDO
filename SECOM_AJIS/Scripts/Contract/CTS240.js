
/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../Base/GridControl.js" />
/// <reference path="../Common/Dialog.js" />
/// <reference path="../Base/DateTimePicker.js" />
/// <reference path="../Base/control_events.js" />

var otherGrid; var isSelected = false;
var supportGrid;
var expectGrid;
var wipGrid;
var attachGrid;
var systemGrid;
var StockGrid;
var isViewState = false;
var objProjectCode = { strProjectCode: CTS240ProjectCode };
var doTbt_Project = null;
var doProjectPurchaserData = null;
var dtNewInstrument = null;
var BranchNo;
var CustActionFlag = null;
var lstID_viewState = ["btnCPNewEditCustomer", "btnCPClearCustomer", "btnNewOtherProject", "expectInput", "btnNewBranch", "systemInput", "supportInput"];

var PurchaserDone = false, ProjectDone = false,
OtherDone = false, ExpectDone = false, WipDone = false,
SupportDone = false, SystemDone = false, StockDone = false;
var instQty = "InstQuantity", currentQty = "currentQty";
var CTS240_gridAttach;
var isInitAttachGrid = false;

var hasAlert = false;
var alertMsg = "";

$(document).ready(function () {

    initScreen();
    initButton();

    //================ GRID ATTACH ========================================    
    //$('#frmAttach').attr('src', 'CTS240_Upload');
    $('#frmAttach').attr('src', 'CTS240_Upload?k=' + _attach_k);
    
    //CTS240_gridAttach = $("#CTS240_gridAttachDocList").InitialGrid(10, false, "/Contract/CTS240_IntialGridAttachedDocList");
    InitLoadAttachList(); //Modify by Jutarat A. on 22032014
    
    SpecialGridControl(CTS240_gridAttach, ["removeButton"]);
    SpecialGridControl(CTS240_gridAttach, ["downloadButton"]);
    BindOnLoadedEvent(CTS240_gridAttach, CTS240_gridAttachBinding);
    //$('#frmAttach').load(RefreshAttachList); //Comment by Jutarat A. on 22032014
    //====================================================================

    if (objProjectCode.strProjectCode == "") {
        $("#SelectProcessType").hide();
        $("#ProjectCode").SetReadOnly(false);
    }
    else {
        $("#ProjectCode").val(objProjectCode.strProjectCode);

        RetrieveProjectInformation();
        $("#ProjectCode").SetReadOnly(true);
        $("#btnRetrieveProject").SetDisabled(true);
        $("#btnSearchProject").SetDisabled(true);
    }

    $("#btnRetrieveProject").click(function () {
        objProjectCode.strProjectCode = $("#ProjectCode").val();
        RetrieveProjectInformation();
    });
    $("#btnSearchProject").click(function () {
        $("#dlgBox").OpenCMS290Dialog("CTS240");
    });
    $("#btnClearProject").click(function () {
        objProjectCode.strProjectCode = "";
        $("#SelectProcessType").hide();
        $("#ProjectCode").SetReadOnly(false);
        $("#ProjectCode").val("");
        $("#ProjectCode").ResetToNormalControl();
        $("#btnRetrieveProject").SetDisabled(false);
        $("#btnSearchProject").SetDisabled(false);

        view_State(false)
        $('#ddlProjectBranch >option').remove();

        //Comment by Jutarat A. on 17012013
        //call_ajax_method_json("/contract/CTS240_RegenBranchCombo", { ProjectCode: CTS240ProjectCode }, function (data) {
        //    regenerate_combo("#ddlProjectBranch", data);
        //});
        //End Comment

        initScreen();
        $("#ProjectInfo").clearForm();
        $("#SecomInfo").clearForm();

        /* --- Set condition --- */
        SEARCH_CONDITION = null;
        /* --------------------- */
    });
});

function CMS290Response(obj) {
    $("#dlgBox").CloseDialog();
    if ((obj != null) && (obj.ProjectCode != null) && (obj.ProjectCode.length > 0)) {
        objProjectCode.strProjectCode = obj.ProjectCode;
        $("#ProjectCode").val(obj.ProjectCode);

        RetrieveProjectInformation();
    }
}

function InitEvent() {
    $("input[id='InstrumentCode']").blur(function () {
        var InstCode = $.trim($(this).val());
        if (InstCode != '') {
            call_ajax_method_json('/contract/CTS230_GetInstrumentDataForSearch', { 'InstrumentCode': InstCode }, function (data) {
                if ($.trim(data) != '') {
                    dtNewInstrument = data;
                    $("#InstrumentName").val(dtNewInstrument.InstrumentName);
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
    $("input[type='radio'][name='Own1']").click(function () { PO1_event("click"); });
    $("input[type='radio'][name='Own2']").click(function () { PO2_event("click"); });

    bindStaffBlurEvent("stHeadSalesmanEmpNo", "stHeadSalesmanEmpFullName");
    bindStaffBlurEvent("stProjectManagerEmpNo", "stProjectManagerEmpFullName");
    bindStaffBlurEvent("stProjectSubManagerEmpNo", "stProjectSubManagerEmpFullName");
    bindStaffBlurEvent("stSecurityPlanningChiefEmpNo", "stSecurityPlanningChiefEmpFullName");
    bindStaffBlurEvent("stInstallationChiefEmpNo", "stInstallationChiefEmpFullName");

    $("#InstrumentQty").BindNumericBox(7, 0, 0, 9999999, 0);
}
function initScreen() {
    //$("#btnSelectProcessType").SetDisabled(true);
    CustActionFlag = null;
    $("#SelectProcessType").SetDisabled(false);
    ViewModeAll(false);
    $("#pjBiddingDate").InitialDate();
    $("#pjOverallBudgetAmount").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#pjReceivedBudgetAmount").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#pjReceivedInstallationFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#pjReceivedInstallationFeeUsd").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#pjReceivedInstrumentPrice").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#pjReceivedInstrumentPriceUsd").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#pjLastOrderAmount").BindNumericBox(12, 2, 0, 999999999999.99, 0);

    $("#PurchaserShow").ReadOnlyAllText(true);
    $("#StaffProjectSection").find("input[type='text'][id*='FullName']").each(function () {
        $(this).attr("readonly", true);
    });
    $("#PersonInfoSection").hide();
    
    $("#ProjectStatus").SetReadOnly(true);
    $("#SupportStaffName").SetReadOnly(true);
    $("#InstrumentName").SetReadOnly(true);
    $("#pjStockoutMemo").SetReadOnly(true);

    $("#pjReceivedInstallationFee").SetReadOnly(true);
    $("#pjReceivedInstallationFee").NumericCurrency().attr("disabled", true);
    $("#pjReceivedInstallationFeeUsd").SetReadOnly(true);
    $("#pjReceivedInstallationFeeUsd").NumericCurrency().attr("disabled", true);
    $("#pjReceivedInstrumentPrice").SetReadOnly(true);
    $("#pjReceivedInstrumentPrice").NumericCurrency().attr("disabled", true);
    $("#pjReceivedInstrumentPriceUsd").SetReadOnly(true);
    $("#pjReceivedInstrumentPriceUsd").NumericCurrency().attr("disabled", true);

    $("#ProjectInfo").hide();
    $("#SecomInfo").hide();
    $("#StockOutGridPlane").hide();
    //$("#UploadIframe").attr("src", "CTS240_Upload");

    $("#divAttachFrame").show();
    $("#divAttachRemark").show();
    if (CTS240_gridAttach != undefined)
    {
        colInx = CTS240_gridAttach.getColIndexById("downloadButton")
        CTS240_gridAttach.setColumnHidden(colInx, false);
        colInx = CTS240_gridAttach.getColIndexById("removeButton")
        CTS240_gridAttach.setColumnHidden(colInx, false);
    }
    

    InitEvent();
    PO1_event();
    PO2_event();

    doTbt_Project = null;
    doProjectPurchaserData = null;
    dtNewInstrument = null;
    BranchNo = null;
    PurchaserDone = false; ProjectDone = false;
    OtherDone = false; ExpectDone = false; WipDone = false;
    SupportDone = false; SystemDone = false; StockDone = false;
    initCommand();

    
}


function RetrieveProjectInformation() {
    call_ajax_method_json("/contract/CTS240_GetTbt_ProjectForView", objProjectCode, function (res, ctrls) {
        if (ctrls != undefined) {
            VaridateCtrl(["ProjectCode"], ctrls);
        }
        else if (res != null) {
            $("#SelectProcessType").show();
            $("#ProjectCode").SetReadOnly(true);
            $("#btnRetrieveProject").SetDisabled(true);
            $("#btnSearchProject").SetDisabled(true);

            call_ajax_method_json("/contract/CTS240_EditPermission", "", function (result, controls) {
                if (result != null)
                    $("#Mod").SetDisabled(!result);
                call_ajax_method_json("/contract/CTS240_LastCompletePermission", "", function (result, controls) {
                    if (result != null)
                        $("#Last").SetDisabled(!result);
                    call_ajax_method_json("/contract/CTS240_CancelPermission", "", function (result, controls) {
                        if (controls != undefined) {
                        }
                        if (result != null)
                            $("#Cancel").SetDisabled(!result);
                        $("input[name='ProcessType']:enabled:first").attr("checked", true);
                        //   $("#btnSelectProcessType").SetDisabled(false);

                        doTbt_Project = res;

                        /* --- Set condition --- */
                        SEARCH_CONDITION = {
                            ProjectCode: doTbt_Project.ProjectCode
                        };
                        /* --------------------- */

                        $("#ProjectCode").val(doTbt_Project.ProjectCode);
                        $("#ProjectStatus").val(doTbt_Project.ProjectStatusCodeName);

                        objProjectCode.strProjectCode = $("#ProjectCode").val(); //Add by Juatarat A. on 10072013

                        if (doTbt_Project.ProjectStatus == C_PROJECT_STATUS_LASTCOMPLETE) {
                            var obj = { module: "contract", code: "MSG3075" };
                            call_ajax_method("/Shared/GetMessage", obj, function (result) {
                                OpenInformationMessageDialog(result.Code, result.Message);
                                $("#SelectProcessType").SetDisabled(true);
                            });
                        } else if (doTbt_Project.ProjectStatus == C_PROJECT_STATUS_CANCEL) {
                            var obj = { module: "contract", code: "MSG3076" };
                            call_ajax_method("/Shared/GetMessage", obj, function (result) {
                                OpenInformationMessageDialog(result.Code, result.Message);
                                $("#SelectProcessType").SetDisabled(true);
                            });
                        }
                        initGrid();
                    });
                });
            });
        }
    });
}



function initGrid() {
    if ($("#OtherGridPlane").length > 0) {
        otherGrid = $("#OtherGridPlane").InitialGrid(0, false, "/contract/CTS240_Other", function () {
            if ($("#systemGridPlane").length > 0) {
                systemGrid = $("#systemGridPlane").InitialGrid(0, false, "/contract/CTS240_system", function () {
                    if ($("#SupportGridPlane").length > 0) {
                        supportGrid = $("#SupportGridPlane").InitialGrid(0, false, "/contract/CTS240_support", function () {
                            if ($("#ExpectGridPlane").length > 0) {
                                expectGrid = $("#ExpectGridPlane").InitialGrid(0, false, "/contract/CTS240_expect", function () {
                                    if ($("#WipGridPlane").length > 0) {
                                        wipGrid = $("#WipGridPlane").InitialGrid(0, false, "/contract/CTS240_wip", function () {
                                            if ($("#StockOutGridPlane").length > 0) {
                                                StockGrid = $("#StockOutGridPlane").InitialGrid(0, false, "/contract/CTS240_stock");
                                                BindOnLoadedEvent(StockGrid, function () {
                                                    for (var i = 0; i < StockGrid.getRowsNum(); i++) {
                                                        var row_id = StockGrid.getRowId(i);
                                                        // var numGridID = "CurrentQty";
                                                        var qtyCol = StockGrid.getColIndexById("AssignBranchQty");
                                                        var val = GetValueFromLinkType(StockGrid, i, qtyCol);
                                                        // StockGrid.cells2(i, qtyCol).setValue(GenerateNumericBox(numGridID, row_id, val, true));
                                                        if (val == "")
                                                            val = 0;
                                                        GenerateNumericBox2(StockGrid, currentQty, row_id, "AssignBranchQty", val, 6, 0, 0, 999999, true, !isViewState);

                                                        $("#" + GenerateGridControlID(currentQty, row_id)).CalBranchQty();


                                                    }
                                                    StockGrid.setSizes();
                                                });
                                                //$("#btnSelectProcessType").SetDisabled(false);
                                            }

                                        });

                                        BindOnLoadedEvent(wipGrid, function () {
                                            for (var i = 0; i < wipGrid.getRowsNum(); i++) {
                                                var row_id = wipGrid.getRowId(i);
                                            }
                                            wipGrid.setSizes();
                                        });
                                    }
                                });
                                SpecialGridControl(expectGrid, ["InstrumentQty", "Remove"]);
                                BindOnLoadedEvent(expectGrid, function () {
                                    for (var i = 0; i < expectGrid.getRowsNum(); i++) {
                                        var row_id = expectGrid.getRowId(i);

                                        //  var numGridID = "Qty";
                                        var qtyCol = expectGrid.getColIndexById("InstrumentQty");
                                        var val = GetValueFromLinkType(expectGrid, i, qtyCol);
                                        //               
                                        GenerateNumericBox2(expectGrid, instQty, row_id, "InstrumentQty", val, 6, 0, 0, 999999, true, true);

                                        var instQtyBox = "#" + GenerateGridControlID(instQty, row_id);
                                        $(instQtyBox).blur(function () {
                                            var tmpRow_id = GetGridRowIDFromControl(this);
                                            var obj = {
                                                InstrumentCode: expectGrid.cells(tmpRow_id, expectGrid.getColIndexById("InstrumentCode")).getValue(),
                                                InstrumentQty: $(this).NumericValue()
                                            };
                                            call_ajax_method_json("/contract/CTS240_UpdateInstrumentQty", obj, function (result, controls) {
                                                if (result) { }

                                            });
                                        });

                                        GenerateRemoveButton(expectGrid, "btnRemoveExpect", row_id, "Remove", true);
                                        BindGridButtonClickEvent("btnRemoveExpect", row_id, function (rid) {
                                            var col = expectGrid.getColIndexById("InstrumentCode");
                                            var InstCode = expectGrid.cells(rid, col).getValue();

                                            call_ajax_method_json("/contract/CTS240_delExpectInstrument", { InstrumentCode: InstCode, InstrumentName: $.trim($("#InstrumentName").val()), ProjectCode: CTS240ProjectCode }, function (res) {
                                                if (res) {
                                                    DeleteRow(expectGrid, rid);
                                                }
                                            });
                                        });
                                    }
                                    expectGrid.setSizes();
                                });
                            }
                        });
                        BindOnLoadedEvent(supportGrid, function () {
                            for (var i = 0; i < supportGrid.getRowsNum(); i++) {
                                var row_id = supportGrid.getRowId(i);
                                GenerateRemoveButton(supportGrid, "btnRemoveSupport", row_id, "Remove", true);
                                BindGridButtonClickEvent("btnRemoveSupport", row_id, function (rid) {
                                    var col = supportGrid.getColIndexById("EmpNo");
                                    var EmpNo = supportGrid.cells(rid, col).getValue();
                                    call_ajax_method_json("/contract/CTS240_delStaff", { EmpNo: EmpNo }, function (res) {
                                        if (res) {
                                            DeleteRow(supportGrid, rid);
                                        }
                                    });
                                });
                            }
                            supportGrid.setSizes();
                        });
                    }
                });
                BindOnLoadedEvent(systemGrid, function () {
                    for (var i = 0; i < systemGrid.getRowsNum(); i++) {
                        var row_id = systemGrid.getRowId(i);
                        GenerateRemoveButton(systemGrid, "btnRemoveSystem", row_id, "Remove", true);
                        BindGridButtonClickEvent("btnRemoveSystem", row_id, function (rid) {
                            var col = systemGrid.getColIndexById('ProductCode');
                            var sysProductCode = systemGrid.cells(rid, col).getValue();
                            call_ajax_method_json("/contract/CTS240_DelSystemProduct", { "ProductCode": sysProductCode }, function (res) {
                                if (res)
                                    DeleteRow(systemGrid, rid);
                            });
                        });
                    }
                    systemGrid.setSizes();
                });
            }




        });
        otherGrid.attachEvent("onBeforeSelect", function (new_row, old_row) {

            return !isSelected;
        });
        otherGrid.attachEvent("onBeforeSorting", function (ind, type, direction) {

            return !isSelected;
        });
        otherGrid.attachEvent("onBeforePageChanged", function (ind, count) {

            return !isSelected;



        });
        BindOnLoadedEvent(otherGrid, function () {
            for (var i = 0; i < otherGrid.getRowsNum(); i++) {
                var row_id = otherGrid.getRowId(i);
                GenerateEditButton(otherGrid, "btnEditOther", row_id, "Edit", true);
                BindGridButtonClickEvent("btnEditOther", row_id, function (rid) {
                    $('#PersonInfoSection').show();
                    DisableForNewProjectPerson(true);
                    $("#row_id").val(rid);
                    $("#CompNameOld").val(otherGrid.cells(rid, otherGrid.getColIndexById("CompanyName")).getValue());
                    $("#ProjRelCompName").val(otherGrid.cells(rid, otherGrid.getColIndexById("CompanyName")).getValue());
                    $("#ProjRelName").val(otherGrid.cells(rid, otherGrid.getColIndexById("Name")).getValue());
                    $("#ProjRelTelNo").val(otherGrid.cells(rid, otherGrid.getColIndexById("TelNo")).getValue());
                    $("#ProjRelRemark").val(otherGrid.cells(rid, otherGrid.getColIndexById("Remark")).getValue());

                    $("#SequenceNo").val(otherGrid.cells(rid, otherGrid.getColIndexById("SequenceNo")).getValue());
                });
                GenerateRemoveButton(otherGrid, "btnRemoveOther", row_id, "Remove", true);
                BindGridButtonClickEvent("btnRemoveOther", row_id, function () {
                    call_ajax_method_json("/contract/CTS240_delOther", { CompanyName: otherGrid.cells(row_id, otherGrid.getColIndexById("CompanyName")).getValue() }, function (res) {
                        if (res) {
                            DeleteRow(otherGrid, row_id);
                        }
                    });
                });
            }
            otherGrid.setSizes();
        });
        SpecialGridControl(otherGrid, ["Edit", "Remove"]);
    }





}
function initButton() {
    $("#btnNewOtherProject").click(function () {
        DisableForNewProjectPerson(true);
    });
    $("#ProjRelbtnAdd").click(function () {

        var lstCompanyName = new Array();
        var lstCompanyName_rowID = new Array();


        var OldRowId = $.trim($("#row_id").val());
        var ProjRelCompName = $.trim($("#ProjRelCompName").val());
        var ProjRelName = $.trim($("#ProjRelName").val());
        var ProjRelTelNo = $.trim($("#ProjRelTelNo").val());
        var ProjRelRemark = $.trim($("#ProjRelRemark").val());
        var SequenceNo = $.trim($("#SequenceNo").val());
        var ActionFlag = "1";
        if (OldRowId != "") {
            ActionFlag = "3";
            if (CheckFirstRowIsEmpty(otherGrid) == false) {
                var CompName = otherGrid.getColIndexById("CompanyName");
                for (var i = 0; i < otherGrid.getRowsNum(); i++) {
                    lstCompanyName.push(otherGrid.cells2(i, CompName).getValue());
                    lstCompanyName_rowID.push(otherGrid.getRowId(i));
                }
            }
        }
        var ObjAddProjRel = {
            ProjectCode: $.trim($("#ProjectCode").val()),
            "CompanyName": ProjRelCompName,
            "Name": ProjRelName,
            "ContractTelNo": ProjRelTelNo,
            "Remark": ProjRelRemark,
            "row_id": OldRowId,
            "lstCompanyName": lstCompanyName,
            "lstCompanyName_rowID": lstCompanyName_rowID,
            SequenceNo: SequenceNo,
            ActionFlag: ActionFlag
        };



        call_ajax_method_json('/contract/CTS240_CheckBeforeAddOther', ObjAddProjRel, function (result, controls) {

            if (controls != undefined) {
                VaridateCtrl(["ProjRelCompName"], controls);
            }

            if (result == true) {
                isSelected = false;
                setDisableGridButton(otherGrid, false, ["btnEditOther", "btnRemoveOther"],
    ["Edit", "Remove"]);
                CheckFirstRowIsEmpty(otherGrid, true);

                if (OldRowId == '') {
                    AddNewRow(otherGrid, [ObjAddProjRel.CompanyName, ObjAddProjRel.Name, ObjAddProjRel.ContractTelNo, ObjAddProjRel.Remark, "", ""]);
                    var RowID = otherGrid.getRowId(otherGrid.getRowsNum() - 1);
                    GenerateEditButton(otherGrid, "btnEditOther", RowID, "Edit", true);
                    BindGridButtonClickEvent("btnEditOther", RowID, function (rid) {
                        $('#PersonInfoSection').show();
                        DisableForNewProjectPerson(true);
                        $("#row_id").val(rid);
                        $("#CompNameOld").val(otherGrid.cells(rid, otherGrid.getColIndexById("CompanyName")).getValue());
                        $("#ProjRelCompName").val(otherGrid.cells(rid, otherGrid.getColIndexById("CompanyName")).getValue());
                        $("#ProjRelName").val(otherGrid.cells(rid, otherGrid.getColIndexById("Name")).getValue());
                        $("#ProjRelTelNo").val(otherGrid.cells(rid, otherGrid.getColIndexById("TelNo")).getValue());
                        $("#ProjRelRemark").val(otherGrid.cells(rid, otherGrid.getColIndexById("Remark")).getValue());

                        $("#SequenceNo").val(otherGrid.cells(rid, otherGrid.getColIndexById("SequenceNo")).getValue());
                    });
                    GenerateRemoveButton(otherGrid, "btnRemoveOther", RowID, "Remove", true);
                    BindGridButtonClickEvent("btnRemoveOther", RowID, function (rid) {
                        call_ajax_method_json("/contract/CTS240_delOther", { CompanyName: otherGrid.cells(rid, otherGrid.getColIndexById("CompanyName")).getValue() }, function (res) {
                            if (res) {
                                DeleteRow(otherGrid, rid);
                            }
                        });
                    });

                    otherGrid.setSizes();
                } else {
                    //alert("WWWW");
                    otherGrid.cells(OldRowId, otherGrid.getColIndexById("CompanyName")).setValue(ProjRelCompName);
                    otherGrid.cells(OldRowId, otherGrid.getColIndexById("Name")).setValue(ProjRelName);
                    otherGrid.cells(OldRowId, otherGrid.getColIndexById("TelNo")).setValue(ProjRelTelNo);
                    otherGrid.cells(OldRowId, otherGrid.getColIndexById("Remark")).setValue(ProjRelRemark);
                }
                $("#PersonInfoSection").clearForm();
                DisableForNewProjectPerson(false);
            }
        });
    });
    $("#ProjRelbtnCancel").click(function () {
        DisableForNewProjectPerson(false);
        $('#PersonInfoSection').clearForm();
    });
    $("#btnInstClear").click(ClearInstSection);
    $('#btnSearchInstrument').click(function () {
        $('#dlgBox').OpenCMS170Dialog("CTS240");
    });
    $("#btnInstAdd").click(function () {

        var InstCode = $.trim($('#InstrumentCode').val());
        var objInstAdd = {
            ProjectCode: $.trim($("#ProjectCode").val()),
            'InstrumentCode': InstCode,
            'InstrumentQty': ($('#InstrumentQty').NumericValue()),
            'dtNewInstrument': dtNewInstrument,
            LineUpTypeCode: dtNewInstrument.LineUpTypeCode
        };
        call_ajax_method_json('/contract/CTS240_CheckBeforeAddExpectInstrument', objInstAdd, function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["InstrumentCode", "InstrumentQty"], controls);
            }
            if (result == true) {
                CheckFirstRowIsEmpty(expectGrid, true);

                //Modify by Jutarat A. on 28112013
                //AddNewRow(expectGrid, [dtNewInstrument.InstrumentCode, dtNewInstrument.InstrumentName,
                //"", "", "", dtNewInstrument.LineUpTypeCode]);
                AddNewRow(expectGrid, [ConvertBlockHtml(dtNewInstrument.InstrumentCode)
                                        , ConvertBlockHtml(dtNewInstrument.InstrumentName)
                                        , "", "", "", dtNewInstrument.LineUpTypeCode]);
                //End Modify

                var qtyCol = expectGrid.getColIndexById("InstrumentQty");

                var row_idx = expectGrid.getRowsNum() - 1;
                var row_id = expectGrid.getRowId(row_idx);

                expectGrid.cells2(row_idx, qtyCol).setValue();

                GenerateRemoveButton(expectGrid, "btnRemoveInst", row_id, "Remove", true);
                // alert($("#InstrumentQty").NumericValue());
                GenerateNumericBox2(expectGrid, instQty, row_id, "InstrumentQty", $("#InstrumentQty").NumericValue(), 6, 0, 0, 999999, true, true)

                expectGrid.setSizes();
                var txt = "#" + GenerateGridControlID(instQty, row_id);


                BindGridButtonClickEvent("btnRemoveInst", row_id, function (rid) {
                    var col = expectGrid.getColIndexById("InstrumentCode");
                    var InstCode = expectGrid.cells(rid, col).getValue();

                    call_ajax_method_json("/contract/CTS240_delExpectInstrument", { InstrumentCode: InstCode, InstrumentName: $.trim($("#InstrumentName").val()), ProjectCode: CTS240ProjectCode }, function (res) {
                        if (res) {
                            DeleteRow(expectGrid, rid);
                            //alert(InstCode);
                        }
                    });
                });

                ClearInstSection();
            }
        });
    });
    $("#btnSelectProcessType").click(function () {
        master_event.LockWindow(true);
        var type = $("input[type='radio'][name='ProcessType']:checked").val();
        if (type == "mod") {
            RetrieveProjectData(false);

            SetRegisterCommand(true, RegisterCommand);
            SetResetCommand(true, ResetAction);

            $("#ProjectInfo").show();
            $("#SecomInfo").show();
            $("#SelectProcessType").SetDisabled(true);
            //  $("#pjProjectAddress").SetDisabled(true);
        }
        else if (type == "last") {
            ValidateBusinessLastComplete(function (res) {
                if (res) {
                    RetrieveProjectData(true);
                    $("#ProjectInfo").show();
                    $("#SecomInfo").show(); SetBackCommand(false, null);
                    SetRegisterCommand(false, null);
                    SetLastCompleteCommand(true, LastCompleteAction);
                    SetResetCommand(true, ResetAction);
                    $("#SelectProcessType").SetDisabled(true);
                    //$("#UploadIframe").attr("src", "CTS240_ViewUpload?strProjectCode=" + CTS240ProjectCode);

                    
                   
                }

            });
        } else if (type == "cancel") {
            ValidateBusinessCancel(function (res) {
                if (res) {
                    RetrieveProjectData(true);
                    $("#ProjectInfo").show();
                    $("#SecomInfo").show();
                    SetBackCommand(false, null);
                    SetRegisterCommand(false, null);
                    SetCancelPcodeCommand(true, CancelPcodeAction);
                    SetResetCommand(true, ResetAction);
                    $("#SelectProcessType").SetDisabled(true);
                    //$("#UploadIframe").attr("src", "CTS240_ViewUpload?strProjectCode=" + CTS240ProjectCode);

                    
                }
            });
        }

        //master_event.LockWindow(false);
    });
    $("#btnCPRetrieve").click(function () {
        RetrieveCustData($.trim($("#CPSearchCustCode").val()));
    });
    $("#btnCPClearCustomer").click(function () {
        doProjectPurchaserData = null;
        CustActionFlag = _Edit;

        InititalProjectPurchaserControls();

    });
    $("#btnCPSearchCustomer").click(function () {
        $("#dlgBox").OpenCMS250Dialog();

    });
    $("#btnCPNewEditCustomer").click(function () {
        CustActionFlag = _Edit;
        $("#dlgBox").OpenMAS050Dialog("QUS020");
    });
    $('#SysProdAdd').click(function () {


        var objSysProdAdd = {
            'Cond.ProjectCode': $.trim($("#ProjectCode").val()),

            'Cond.ProductCode': $.trim($("#SysProductName option:selected").val()),
            'Cond.tmpProductCodeName': $.trim($("#SysProductName option:selected").text()),
            'Cond.Action': "1"
        };
        call_ajax_method_json("/contract/CTS240_CheckBeforeAddSystemProduct", objSysProdAdd, function (result, controls) {
            if (controls != undefined)
                VaridateCtrl(["SysProductName"], controls);

            if (result) {
                CheckFirstRowIsEmpty(systemGrid, true);
                AddNewRow(systemGrid, [$("#SysProductName option:selected").text(), '', $("#SysProductName").val()]);


                var row_id = systemGrid.getRowId(systemGrid.getRowsNum() - 1);
                GenerateRemoveButton(systemGrid, "btnRemoveSystem", row_id, "Remove", true);
                BindGridButtonClickEvent("btnRemoveSystem", row_id, function (rid) {
                    var col = systemGrid.getColIndexById('ProductCode');
                    var sysProductCode = systemGrid.cells(rid, col).getValue();
                    call_ajax_method_json("/contract/CTS240_DelSystemProduct", { "ProductCode": sysProductCode }, function (res) {
                        if (res)
                            DeleteRow(systemGrid, rid);
                    });
                });
                systemGrid.setSizes();
            }
        });

    });
    $("#btnNewBranch").click(function () {
        if (wipGrid.getRowsNum() <= 1) {

        }


        call_ajax_method_json("/contract/CTS240_NewBranchNo", "", function (res) {
            if (res == null)
                return false;


            $.when($('#ddlProjectBranch').append($('<option></option>').val(CTS240ProjectCode + ":" + res).html(res)))
            .done($('#ddlProjectBranch').val(CTS240ProjectCode + ":" + res));

            $("#StockOutGridPlane").show();
            $("#StockOutGridPlane").LoadDataToGrid(StockGrid, 0, false,
            "/contract/GetStockOut", { strProjectCode: CTS240ProjectCode, "BranchNo": res },
             "View_dtTbt_ProjectStockoutBranchIntrumentDetailForView", false);








        });
    });
    $('#SupportAdd').click(function () {
        if (true) {
            var objStaff = {
                ProjectCode: $.trim($("#ProjectCode").val()),
                EmpNo: $.trim($("#SupportStaffCode").val()),
                Remark: $.trim($("#SupportREmark").val())

            };
            call_ajax_method_json("/contract/CTS240_CheckBeforeAddStaff", objStaff, function (result, controls) {
                if (controls != undefined) {
                    VaridateCtrl(["SupportStaffCode"], controls);
                }
                if (result) {
                    CheckFirstRowIsEmpty(supportGrid, true);
                    AddNewRow(supportGrid,
                    [$.trim($("#SupportStaffCode").val()),
                     $.trim($("#SupportStaffName").val()),
                      $.trim($("#SupportBelonging").val()),
                       $.trim($("#SupportREmark").val())], "");

                    var row_id = supportGrid.getRowId(supportGrid.getRowsNum() - 1);
                    GenerateRemoveButton(supportGrid, "btnRemoveSupport", row_id, "Remove", true);
                    BindGridButtonClickEvent("btnRemoveSupport", row_id, function (rid) {
                        var col = supportGrid.getColIndexById("EmpNo");
                        var EmpNo = supportGrid.cells(rid, col).getValue();
                        call_ajax_method_json("/contract/CTS240_delStaff", { EmpNo: EmpNo }, function (res) {
                            if (res) {
                                DeleteRow(supportGrid, rid);
                            }
                        });
                    });

                    supportGrid.setSizes();
                    ClearSupportStaff();
                }
            });
        }
    });

    $("#SupportClear").click(ClearSupportStaff);
}

function InititalProjectPurchaserControls() {
    $("#CPSearchCustCode").SetReadOnly(false);
    $("#btnCPRetrieve").SetDisabled(false);
    $("#btnCPSearchCustomer").SetDisabled(false);
    $("#btnCPNewEditCustomer").SetDisabled(false);
    $("#ProjectPurchaser").clearForm();
    if ($("input[name='Own1']:checked").val() == "same") {
        $("input[name^='pjOwner1']").clearForm();
    }
    if ($("input[name='Own2']:checked").val() == "same") {
        $("input[name^='pjOwner2']").clearForm();
    }
}
function setDisableGridButton(grid, isDisable, BtnName, ColName) {
    var isEnable = !isDisable;
    if (BtnName != null && ColName != null && (BtnName.length == ColName.length)) {
        if (grid.getRowsNum() != 0) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                for (var k = 0; k < BtnName.length; k++) {
                    EnableGridButton(grid, BtnName[k], row_id, ColName[k], isEnable);
                }
            }
        }
        grid.attachEvent("onAfterSorting"
            , function (index, type, direction) {
                for (var i = 0; i < grid.getRowsNum(); i++) {
                    var row_id = grid.getRowId(i);
                    for (var k = 0; k < BtnName.length; k++) {
                        EnableGridButton(grid, BtnName[k], row_id, ColName[k], isEnable);

                    }
                }
            }
        );
    }
}
function DisableForNewProjectPerson(IsDisable) {
    isSelected = IsDisable;
    setDisableGridButton(otherGrid, IsDisable, ["btnEditOther", "btnRemoveOther"],
    ["Edit", "Remove"]);
    setDisableGridButton(systemGrid, IsDisable, ["btnRemoveSystem"],
    ["Remove"]);
    setDisableGridButton(supportGrid, IsDisable, ["btnRemoveSupport"],
    ["Remove"]);
    setDisableGridButton(expectGrid, IsDisable, ["btnRemoveExpect"],
    ["Remove"]);


    if (IsDisable) {
        $('#PersonInfoSection').show();
        // $("#PersonInfoSection :input[type='text']").removeAttr('disabled');
        $('input[id^="ProjRel"').val('');
        SetRegisterCommand(false, null);
        SetResetCommand(false, null);
    } else {
        $('#PersonInfoSection').hide();
        $("#row_id").val('');

        // $("#PersonInfoSection :input[type='text']").attr('disabled', true);
        $('input[id^="ProjRel"]').val('');
        SetRegisterCommand(true, RegisterCommand);
        SetResetCommand(true, ResetAction);
    }
    colInx = CTS240_gridAttach.getColIndexById("downloadButton")
    CTS240_gridAttach.setColumnHidden(colInx, IsDisable);
    colInx = CTS240_gridAttach.getColIndexById("removeButton")
    CTS240_gridAttach.setColumnHidden(colInx, IsDisable);

    if (!IsDisable) {
        SetFitColumnForBackAction(CTS240_gridAttach, "TmpColumn");
        $("#divAttachFrame").show();
        $("#divAttachRemark").show();
    }
    else {
        $("#divAttachFrame").hide();
        $("#divAttachRemark").hide();
    }

    IsDisable = !IsDisable;
    $('#ProjectNameSection').SetEnableViewIfReadonly(IsDisable);
    $('#ProjectPurchaser').SetEnableViewIfReadonly(IsDisable);
    $('#ProjectOwnerSection').SetEnableViewIfReadonly(IsDisable);
    $('#ProjectManCompSection').SetEnableViewIfReadonly(IsDisable);
    $('#ProjectManCompSectionMini').SetEnableViewIfReadonly(IsDisable);
    $('#ProjectPersonSection').SetEnableViewIfReadonly(IsDisable);
    $('#SecomInfo').SetEnableViewIfReadonly(IsDisable);

    
    

}
function RegisterCommand() {
    var own1 = $("input[name='Own1'][type='radio']:checked").val();
    if (own1 == "diff")
        own1 = false;
    else
        own1 = true;

    var own2 = $("input[name='Own2'][type='radio']:checked").val();
    if (own2 == "diff")
        own2 = false;
    else
        own2 = true;
    /* --- Set Parameter --- */
    var objTbt_Project = {// ============= doTbt_project ===========
        ProjectName: $("#pjProjectName").val(),
        ProjectAddress: $("#pjProjectAddress").val(),

        Owner1Flag: own1,
        Owner1NameEN: $("#pjOwner1NameEN").val(),
        Owner1NameLC: $("#pjOwner1NameLC").val(),

        Owner2Flag: own2,
        Owner2NameEN: $("#pjOwner2NameEN").val(),
        Owner2NameLC: $("#pjOwner2NameLC").val(),

        ManagementCompanyNameEN: $("#pjManagementCompanyNameEN").val(),
        ManagementCompanyNameLC: $("#pjManagementCompanyNameLC").val(),

        OverallBudgetAmountCurrencyType: $("#pjOverallBudgetAmount").NumericCurrencyValue(),
        OverallBudgetAmount: $("#pjOverallBudgetAmount").NumericValue(),
        ReceivedBudgetAmountCurrencyType: $("#pjReceivedBudgetAmount").NumericCurrencyValue(),
        ReceivedBudgetAmount: $("#pjReceivedBudgetAmount").NumericValue(),

        LastOrderAmountCurrencyType: $("#pjLastOrderAmount").NumericCurrencyValue(),
        LastOrderAmount: $("#pjLastOrderAmount").NumericValue(),

        HeadSalesmanEmpNo: $("#stHeadSalesmanEmpNo").val(),
        ProjectManagerEmpNo: $("#stProjectManagerEmpNo").val(),
        ProjectSubManagerEmpNo: $("#stProjectSubManagerEmpNo").val(),
        SecurityPlanningChiefEmpNo: $("#stSecurityPlanningChiefEmpNo").val(),
        InstallationChiefEmpNo: $("#stInstallationChiefEmpNo").val(),
        BiddingDate: $("#pjBiddingDate").val()

        //============= End doTbt_project ===========
    };
    if (doProjectPurchaserData != null)
        doProjectPurchaserData.ActionFlag = CustActionFlag;
    call_ajax_method_json("/contract/CTS240_RegisterCommand", { doTbt_Project: objTbt_Project, doRegCust: doProjectPurchaserData }, function (res, controls) {
        VaridateCtrl(["pjProjectName", "pjProjectAddress", "stHeadSalesmanEmpNo",
             "stProjectManagerEmpNo", "stProjectSubManagerEmpNo", "stSecurityPlanningChiefEmpNo",
             "stInstallationChiefEmpNo"
             ], null);
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["pjProjectName", "pjProjectAddress", "stHeadSalesmanEmpNo",
             "stProjectManagerEmpNo", "stProjectSubManagerEmpNo", "stSecurityPlanningChiefEmpNo",
             "stInstallationChiefEmpNo"
             ], controls);
            /* --------------------- */

            return;
        }

        if (res == true) {

            ConfirmState();
        }
    });


}
function RetrieveCustData(CustCode) {
    call_ajax_method_json("/contract/CTS240_RetrievePurchaserData", { CustCode: CustCode }, function (result, controls) {
        if (controls != undefined) {
            VaridateCtrl(["CPSearchCustCode"], ["CPSearchCustCode"]);
            return;
        }
        if (result != null) {
            doProjectPurchaserData = result;
            ViewCustomer(doProjectPurchaserData);
        }
    });
}


function RetrieveProjectData(IsNeedDone) {

    call_ajax_method_json("/contract/CTS240_GetProjectPurchaser", objProjectCode, function (res) {

        //if (res != null)
        if (res != null && res.length > 0) { //Modify by Jutarat A. on 17012013 

            $("#ProjectPurchaser").bindJSON_Prefix("CP", res[0]);

            //================== Teerapong S. 14/09/2012 ===================== 
            CTS240ProjectCode = objProjectCode.strProjectCode;


            PurchaserDone = true;
            doProjectPurchaserData = res[0];
            doProjectPurchaserData.ProjectCode = CTS240ProjectCode;

            btnPurchaserPack(doProjectPurchaserData);

            $('#frmAttach').attr('src', 'CTS240_SendAttachKey?sK=' + CTS240ProjectCode + "&k=" + _attach_k);
            $('#frmAttach').load(RefreshAttachList);

            call_ajax_method_json("/contract/CTS240_GetProjectForView", objProjectCode, function (res) {
                $("#ProjectStatus").val(res[0].ProjectStatusCodeName);
                if (res[0].Owner1Flag == true) {
                    $("#chkSameOwn1").attr("checked", true);
                    $("#pjOwner1NameEN").SetReadOnly(true);
                    $("#pjOwner1NameLC").SetReadOnly(true);
                }
                else {
                    $("#chkDiffOwn1").attr("checked", true);
                    $("#pjOwner1NameEN").SetReadOnly(false);
                    $("#pjOwner1NameLC").SetReadOnly(false);
                }
                if (res[0].Owner2Flag == true) {
                    $("#chkSameOwn2").attr("checked", true);
                    $("#pjOwner2NameEN").SetReadOnly(true);
                    $("#pjOwner2NameLC").SetReadOnly(true);
                }
                else {
                    $("#chkDiffOwn2").attr("checked", true);
                    $("#pjOwner2NameEN").SetReadOnly(false);
                    $("#pjOwner2NameLC").SetReadOnly(false);
                }
                $("#ProjectNameSection").bindJSON_Prefix("pj", res[0]);
                $("#ProjectOwnerSection").bindJSON_Prefix("pj", res[0]);
                $("#ProjectManCompSection").bindJSON_Prefix("pj", res[0]);
                $("#ProjectManCompSectionMini").bindJSON_Prefix("pj", res[0]);
                $("input.numeric-box:text").SetNumericValue_Prefix("pj", res[0], 2);

                if (res[0].OverallBudgetAmountCurrencyType == undefined)
                    res[0].OverallBudgetAmountCurrencyType = C_CURRENCY_LOCAL;
                if (res[0].ReceivedBudgetAmountCurrencyType == undefined)
                    res[0].ReceivedBudgetAmountCurrencyType = C_CURRENCY_LOCAL;
                if (res[0].LastOrderAmountCurrencyType == undefined)
                    res[0].LastOrderAmountCurrencyType = C_CURRENCY_LOCAL;

                $("#pjOverallBudgetAmount").SetNumericCurrency(res[0].OverallBudgetAmountCurrencyType);
                $("#pjReceivedBudgetAmount").SetNumericCurrency(res[0].ReceivedBudgetAmountCurrencyType);
                $("#pjReceivedInstallationFee").SetNumericCurrency(C_CURRENCY_LOCAL);
                $("#pjReceivedInstallationFeeUsd").SetNumericCurrency(C_CURRENCY_US);
                $("#pjReceivedInstrumentPrice").SetNumericCurrency(C_CURRENCY_LOCAL);
                $("#pjReceivedInstrumentPriceUsd").SetNumericCurrency(C_CURRENCY_US);
                $("#pjLastOrderAmount").SetNumericCurrency(res[0].LastOrderAmountCurrencyType);

                if (res[0].OverallBudgetAmountCurrencyType == C_CURRENCY_US)
                    $("#pjOverallBudgetAmount").val(SetNumericValue(res[0].OverallBudgetAmountUsd, 2));
                else
                    $("#pjOverallBudgetAmount").val(SetNumericValue(res[0].OverallBudgetAmount, 2));

                if (res[0].ReceivedBudgetAmountCurrencyType == C_CURRENCY_US)
                    $("#pjReceivedBudgetAmount").val(SetNumericValue(res[0].ReceivedBudgetAmountUsd, 2));
                else
                    $("#pjReceivedBudgetAmount").val(SetNumericValue(res[0].ReceivedBudgetAmount, 2));

                if (res[0].LastOrderAmountCurrencyType == C_CURRENCY_US)
                    $("#pjLastOrderAmount").val(SetNumericValue(res[0].LastOrderAmountUsd, 2));
                else
                    $("#pjLastOrderAmount").val(SetNumericValue(res[0].LastOrderAmount, 2));
                
                $("#SecomInfo").bindJSON_Prefix("pj", res[0]);
                $("#pjBiddingDate").val(ConvertDateToTextFormat(ConvertDateObject(res[0].BiddingDate)));
                $("#pjBiddingDate").datepicker("getDate");
                $("#StaffProjectSection").bindJSON_Prefix("st", res[0]);
                ProjectDone = true;
                $("#OtherGridPlane").LoadDataToGrid(otherGrid, 0, false, "/contract/CTS240_GetOtherRelateForView", objProjectCode, "tbt_ProjectOtherRalatedCompany", false, function () {
                    $("#systemGridPlane").LoadDataToGrid(systemGrid, 0, false, "/contract/CTS240_GetSystemDetailForView", objProjectCode, "dtTbt_ProjectSystemDetailForView", false, function () {
                        $("#SupportGridPlane").LoadDataToGrid(supportGrid, 0, false, "/contract/CTS240_GetSupportStaffForView", objProjectCode, "dtTbt_ProjectSupportStaffDetailForView", false, function () {
                            $("#ExpectGridPlane").LoadDataToGrid(expectGrid, 0, false, "/contract/CTS240_GetExpectIntrumentDetailForView", objProjectCode, "dtTbt_ProjectExpectedInstrumentDetailsForView", false, function () {
                                $("#WipGridPlane").LoadDataToGrid(wipGrid, 0, false, "/contract/CTS240_GetProjectWIPForView", objProjectCode, "dtTbt_ProjectStockoutIntrumentForView", false, function () {

                                    $("#ddlProjectBranch").change(function () {
                                        $("#StockOutGridPlane").show();
                                        var Branch = $("#ddlProjectBranch option:selected").val();
                                        if ($("#StockOutGridPlane").length > 0) {
                                            if (Branch != "") {
                                                Branch = Branch.split(":");
                                                BranchNo = Branch[1];
                                                $("#StockOutGridPlane").LoadDataToGrid(StockGrid, 0, false, "/contract/GetStockOut", { strProjectCode: CTS240ProjectCode, "BranchNo": Branch[1] }, "View_dtTbt_ProjectStockoutBranchIntrumentDetailForView", false);

                                            } else {
                                                $("#StockOutGridPlane").hide();
                                                DeleteAllRow(StockGrid);
                                            }
                                        }

                                    });

                                    //----------------------------------------------------------------------
                                    call_ajax_method_json("/contract/CTS240_GetStockOutMemo", { ProjectCode: doProjectPurchaserData.ProjectCode }, function (result, controls) {
                                        if (result != null) {
                                            $("#pjStockoutMemo").val(result);
                                        }

                                        //Add by Jutarat A. on 17012013
                                        call_ajax_method_json("/contract/CTS240_RegenBranchCombo", { ProjectCode: doProjectPurchaserData.ProjectCode }, function (data) {
                                            regenerate_combo("#ddlProjectBranch", data);
                                        });
                                        //End Add
                                    });
                                    //----------------------------------------------------------------------

                                    if (IsNeedDone) {

                                        view_State(true);
                                    }

                                    master_event.LockWindow(false);
                                });
                            });
                        });
                    });
                });
            });
        }
        else {
            master_event.LockWindow(false); //Add by Jutarat A. on 17012013
        }
    });
}

function view_State(IsView) {
    isViewState = IsView;
    if (IsView) {
        $("#ProjectInfo").SetEnableViewIfReadonly(!IsView);
        $("#SecomInfo").SetEnableViewIfReadonly(!IsView);

        if (otherGrid != undefined) {
            setColHide(otherGrid, "Edit", IsView);
            setColHide(otherGrid, "Remove", IsView);
        }
        if (systemGrid != undefined) {
            setColHide(systemGrid, "Remove", IsView);
        }
        if (supportGrid != undefined) {
            setColHide(supportGrid, "Remove", IsView);
        }
        if (expectGrid != undefined) {
            setColHide(expectGrid, "Remove", IsView);
        }

        $("#SubSpecPurchaser").hide();
        for (var i = 0; i < lstID_viewState.length; i++) {
            $("#" + lstID_viewState[i]).hide();
        }
    } else {
        $("#ProjectInfo").SetEnableViewIfReadonly(!IsView);
        $("#SecomInfo").SetEnableViewIfReadonly(!IsView);

        if (otherGrid != undefined) {
            setColHide(otherGrid, "Edit", IsView);
            setColHide(otherGrid, "Remove", IsView);
            SetFitColumnForBackAction(otherGrid, "tmpCol");
        }
        if (systemGrid != undefined) {
            setColHide(systemGrid, "Remove", IsView);
            SetFitColumnForBackAction(systemGrid, "tmpCol");
        }
        if (supportGrid != undefined) {
            setColHide(supportGrid, "Remove", IsView);
            SetFitColumnForBackAction(supportGrid, "tmpCol");
        }
        if (expectGrid != undefined) {
            setColHide(expectGrid, "Remove", IsView);
            SetFitColumnForBackAction(expectGrid, "tmpCol");
        }
        

        $("#SubSpecPurchaser").show();
        for (var i = 0; i < lstID_viewState.length; i++) {
            $("#" + lstID_viewState[i]).show();
        }
    }
    $("#ddlProjectBranch").SetDisabled(false);
}


function clear_contract_customer_click() {
    $("input[name^='pjOwner']").clearForm();
    //$("input[name^='po2']").clearForm();
    InitialContractCustomerSection();
    //ClearRegisterData(1);
}
function setColHide(grid, Col, IsHide) {
    grid.setColumnHidden(grid.getColIndexById(Col), IsHide);


}


function ClearPersonalInfo() {

    $("#PersonInfoSection :input[type='text']").val('');
    $("#PersonInfoSection :input[type='text']").attr('disabled', true);

}
function initCommand() {
    SetRegisterCommand(false, null);
    SetResetCommand(false, null);
    SetLastCompleteCommand(false, null);
    SetCancelPcodeCommand(false, null);
}
function LastCompleteAction() {
    ValidateBusinessLastComplete(function () {
        var obj = {
            module: "Contract",
            code: "MSG3220"
        };
        call_ajax_method("/Shared/GetMessage", obj, function (result) {
            OpenYesNoMessageDialog(result.Code, result.Message, function () {
                call_ajax_method_json("/contract/CTS240_RegisterLastComplete", objProjectCode, function (result) {
                    if (result != null) {
                        $("#ProjectStatus").val(result);
                        // JS Call Message

                        var obj = {
                            module: "Contract",
                            code: "MSG3222"
                        };
                        call_ajax_method("/Shared/GetMessage", obj, function (result) {
                            OpenInformationMessageDialog(result.Code, result.Message, null);
                        });

                        SetLastCompleteCommand(false, null);
                        SetResetCommand(false, null);
                    }
                });
            }, null);
        });
    });
}
function ResetAction() {

    var obj = {
        module: "Common",
        code: "MSG0038"
    };
    call_ajax_method("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
        function () {
            view_State(false)
            $('#ddlProjectBranch >option').remove();
            call_ajax_method_json("/contract/CTS240_RegenBranchCombo", { ProjectCode: CTS240ProjectCode }, function (data) {
                regenerate_combo("#ddlProjectBranch", data);
            });
            initScreen();
            $("#ProjectInfo").clearForm();
            $("#SecomInfo").clearForm();
        },
        null);
    });
}
function CancelPcodeAction() {
    ValidateBusinessCancel(function () {

        var obj = {
            module: "Contract",
            code: "MSG3221"
        };
        call_ajax_method("/Shared/GetMessage", obj, function (result) {
            OpenYesNoMessageDialog(result.Code, result.Message, function () {
                call_ajax_method_json("/contract/CTS240_RegisterCancel", objProjectCode, function (result) {
                    if (result != null) {
                        $("#ProjectStatus").val(result);
                        // JS Call Message

                        var obj = {
                            module: "Contract",
                            code: "MSG3223"
                        };
                        call_ajax_method("/Shared/GetMessage", obj, function (result) {
                            OpenInformationMessageDialog(result.Code, result.Message, null);
                        });
                        SetCancelPcodeCommand(false, null);
                        SetResetCommand(false, null);
                    }
                });
            }, null);
        });
    });
}


//----------------------------------------------------------------------------------------------------------------
function ViewModeAll(IsViewMode) {
    if (IsViewMode) {
        $("#SubSpecPurchaser").hide();
        $("#ProjectInfo").SetViewMode(true);
        $("#SecomInfo").SetViewMode(true);
        if (otherGrid != undefined) {
            setColHide(otherGrid, "Edit", true);
            setColHide(otherGrid, "Remove", true);
        }

        if (systemGrid != undefined) {
            setColHide(systemGrid, "Remove", true);
        }
        if (supportGrid != undefined) {
            setColHide(supportGrid, "Remove", true);
        }
        if (expectGrid != undefined) {
            setColHide(expectGrid, "Remove", true);
        }
    } else {
        $("#SubSpecPurchaser").show();
        $("#ProjectInfo").SetViewMode(false);
        $("#SecomInfo").SetViewMode(false);
        if (otherGrid != undefined) {
            setColHide(otherGrid, "Edit", false);
            setColHide(otherGrid, "Remove", false);
        }

        if (systemGrid != undefined) {
            setColHide(systemGrid, "Remove", false);

        }
        if (supportGrid != undefined) {
            setColHide(supportGrid, "Remove", false);
            SetFitColumnForBackAction(supportGrid, "tmpCol");
        }
        if (expectGrid != undefined) {
            setColHide(expectGrid, "Remove", false);
            SetFitColumnForBackAction(expectGrid, "tmpCol");
        }

    }

}
function ValidateBusinessLastComplete(fnc) {
    call_ajax_method_json("/contract/CTS240_ValidateLastComplete", objProjectCode, function (res) {
        if (res != true) {
            if (typeof (fnc) == "function")
                fnc(true);
        } else {
            if (typeof (fnc) == "function")
                fnc(false);
        }
    });
}
function ValidateBusinessCancel(fnc) {
    call_ajax_method_json("/contract/CTS240_ValidateBusinessCancel", objProjectCode, function (res) {
        if (res != true) {
            if (typeof (fnc) == "function")
                fnc(true);
        } else {
            if (typeof (fnc) == "function")
                fnc(false);
        }
    });
}
function btnPurchaserPack(doCust) {
    if ($.trim(doCust.CustCodeShort) != "") {
        $("#btnCPNewEditCustomer").SetDisabled(true);
        $("#btnCPSearchCustomer").SetDisabled(true);
        $("#btnCPRetrieve").SetDisabled(true);
        $("#CPSearchCustCode").SetDisabled(true);
    } else if ($.trim(doCust.CustNameEN) != "") {
        $("#btnCPNewEditCustomer").SetDisabled(false);
        $("#btnCPSearchCustomer").SetDisabled(true);
        $("#btnCPRetrieve").SetDisabled(true);
        $("#CPSearchCustCode").SetDisabled(true);
    } else {
        $("#btnCPNewEditCustomer").SetDisabled(false);
        $("#btnCPSearchCustomer").SetDisabled(false);
        $("#btnCPRetrieve").SetDisabled(false);
        $("#CPSearchCustCode").SetDisabled(false);
    }


}
function CMS250Response(result) {
    $("#dlgBox").CloseDialog();
    if (result != null) {
        $("#CPSearchCustCode").val(result.CustomerData.CustCode);
        RetrieveCustData(result.CustomerData.CustCode);
    }
}
function PO1_event(prm) {
    var chk = $("input[name='Own1'][type='radio']:checked").val();
    //alert(chk);
    if (chk == "same") {
        $("input[name^='pjOwner1']").clearForm();
        $('#pjOwner1NameEN').val($('#CPCustFullNameEN').val());
        $('#pjOwner1NameLC').val($('#CPCustFullNameLC').val());
        $("input[name^='pjOwner1']").attr("readonly", true);
    }
    else {
        if (prm != null)
            $("input[name^='pjOwner1']").clearForm();
        $("input[name^='pjOwner1']").removeAttr("readonly");
    }
}
function PO2_event(prm) {
    var chk = $("input[type='radio'][name='Own2']:checked").val();
    //alert(chk);
    if (chk == "same") {
        $("input[name^='pjOwner2']").clearForm();
        $('#pjOwner2NameEN').val($('#CPCustFullNameEN').val());
        $('#pjOwner2NameLC').val($('#CPCustFullNameLC').val());
        $("input[name^='pjOwner2']").attr("readonly", true);
    }
    else {
        if (prm != null)
            $("input[name^='pjOwner2']").clearForm();
        $("input[name^='pjOwner2']").removeAttr("readonly");
    }
}
function ClearSupportStaff() {
    CloseWarningDialog();
    $('#SupportStaffCode').val('');
    $('#SupportStaffName').val('');
    $('#SupportBelonging').val('');
    $('#SupportREmark').val('');
    VaridateCtrl(["SupportStaffCode"], null);

}
function ClearInstSection() {
    CloseWarningDialog();
    VaridateCtrl(["InstrumentCode", "InstrumentQty"], null);
    $("#InstData :input[type='text']").val('');
    dtNewInstrument = null;

}
function MAS050Response(doCustomer) {
    $("#dlgBox").CloseDialog();
    doProjectPurchaserData = doCustomer;
    $("#CPSearchCustCode").val("");
    ViewCustomer(doCustomer);
}
function MAS050Object() {
    return {
        doCustomer: doProjectPurchaserData
    };
}
function ViewCustomer(doCustomer) {
    btnPurchaserPack(doCustomer)

    $("#ProjectPurchaser").bindJSON_Prefix("CP", doProjectPurchaserData);
    $("#CPSearchCustCode").val(doProjectPurchaserData.CustCodeShort);
    PO1_event();
    PO2_event();
}

function CMS170Object() {
    return { bExpTypeHas: true,
        bExpTypeNo: false,

        bInstTypeGen: true,
        bInstTypeMonitoring: true,
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

function GetStaff(EmpNo, fnc) {

    EmpNo = $.trim(EmpNo);

    if (EmpNo != '')
        call_ajax_method_json('/contract/CTS230_getActiveEmployee', { 'EmpNo': EmpNo }, function (data) {
            if (typeof (fnc == "function")) {
                fnc(data);
            }
        });



}
function ConfirmState() {
    SetRegisterCommand(false, null);
    SetResetCommand(false, null);
    ViewModeAll(true);
    $("#divAttachFrame").hide();
    $("#divAttachRemark").hide();
    colInx = CTS240_gridAttach.getColIndexById("removeButton")
    CTS240_gridAttach.setColumnHidden(colInx, true);
    colInx = CTS240_gridAttach.getColIndexById("downloadButton")
    CTS240_gridAttach.setColumnHidden(colInx, true);

    SetConfirmCommand(true, function () {
        call_ajax_method_json('/contract/CTS240_ConfirmCommand/', "", function (res) {
            if (res == true) {
                var obj = { module: "Common", code: "MSG0046" };
                call_ajax_method("/Shared/GetMessage", obj, function (result) {
                    OpenInformationMessageDialog(result.Code, result.Message, function () {


                    });
                    SetConfirmCommand(false, null);
                    SetBackCommand(false, null);

                });
            } else {

            }
        });

    });
    SetBackCommand(true, function () {
        ViewModeAll(false);
        //$("#UploadIframe").attr("src", "CTS240_Upload?strProjectCode=" + CTS240ProjectCode);
        SetConfirmCommand(false, null);
        SetRegisterCommand(true, RegisterCommand);
        SetResetCommand(true, ResetAction);
        SetBackCommand(false, null);

        $("#divAttachFrame").show();
        $("#divAttachRemark").show();
        $("#divProjectCondition").show();
        $("#SelectProcessType").show();

        colInx = CTS240_gridAttach.getColIndexById("downloadButton")
        CTS240_gridAttach.setColumnHidden(colInx, false);
        colInx = CTS240_gridAttach.getColIndexById("removeButton")
        CTS240_gridAttach.setColumnHidden(colInx, false);
        SetFitColumnForBackAction(CTS240_gridAttach, "TmpColumn");

        for (var i = 0; i < lstID_viewState.length; i++) {
            $("#" + lstID_viewState[i]).show();
        }
        SetFitColumnForBackAction(otherGrid, "tmpCol");
        SetFitColumnForBackAction(systemGrid, "tmpCol");
        SetFitColumnForBackAction(supportGrid, "tmpCol");
        SetFitColumnForBackAction(expectGrid, "tmpCol");
    });
    //$("#UploadIframe").attr("src", "CTS240_ViewUpload?strProjectCode=" + CTS240ProjectCode);

    $("#divProjectCondition").hide();
    $("#SelectProcessType").hide();
    for (var i = 0; i < lstID_viewState.length; i++) {
        $("#" + lstID_viewState[i]).hide();
    }
    SetFitColumnForBackAction(otherGrid, "tmpCol");
    SetFitColumnForBackAction(systemGrid, "tmpCol");
    SetFitColumnForBackAction(supportGrid, "tmpCol");
    SetFitColumnForBackAction(expectGrid, "tmpCol");
}
function bindStaffBlurEvent(EmpNoID, EmpFullNameID) {

    $("#" + EmpNoID).blur(function () {
        if ($.trim($(this).val()) != "") {
            GetStaff($(this).val(), function (res) {
                $("#" + EmpFullNameID).val(res);
            });
        } else {
            $("#" + EmpFullNameID).val('');
        }
        $(this).val($.trim($(this).val()));
    });

}

$.fn.ReadOnlyAllText = function (IsReadOnly) {
    $(this).find("input[type='text']").each(function () {
        var tag = this.tagName.toLowerCase();
        if (IsReadOnly) {
            if (tag == "input" && $(this).attr("type") == "text") {
                if (!$(this).prop("readonly"))
                    $(this).attr("readonly", true);
            }
        } else {
            if ($(this).prop("readonly"))
                $(this).removeAttr("readonly");
        }

    });

}
$.fn.CalBranchQty = function () {
    $(this).blur(function () {
        var row_id = GetGridRowIDFromControl(this);
        var val = $(this).NumericValue();

        if (val == "") {
            val = 0;
            $(this).setCurrentBranchQty(row_id, val);
        } else {
            if (parseInt(val) > parseInt(StockGrid.cells(row_id, StockGrid.getColIndexById("AssignBranchQtyBefore")).getValue()) + parseInt(StockGrid.cells(row_id, StockGrid.getColIndexById("SumNotAssign")).getValue())) {
                $(this).setCurrentBranchQty(row_id, parseInt(StockGrid.cells(row_id, StockGrid.getColIndexById("AssignBranchQtyBefore")).getValue()));
            } else {
                $(this).setCurrentBranchQty(row_id, val);
            }
        }

    });

}
function setStockGridValue(row_id, Column, Value) {
    StockGrid.cells(row_id, StockGrid.getColIndexById(Column)).setValue(Value);
}
$.fn.setCurrentBranchQty = function (row_id, Value) {
    var Branch = $("#ddlProjectBranch option:selected").val();
    if (Branch != "") {
        Branch = Branch.split(":");
        BranchNo = Branch[1];
    } else {
        $("#StockOutGridPlane").hide();
        DeleteAllRow(StockGrid);
        return false;
    }
    call_ajax_method_json("/contract/UpdateStockBranchForView", { BranchNO: BranchNo,
        InstrumentCode: StockGrid.cells(row_id, StockGrid.getColIndexById("InstrumentCode")).getValue()
    , AssignQty: Value
    }, function () {

        //var numGridID = "CurrentQty";
        var qtyCol = StockGrid.getColIndexById("AssignBranchQty");
        var val = Value;
        //StockGrid.cells(row_id, qtyCol).setValue(GenerateNumericBox(numGridID, row_id, val, true));
        if (val == "")
            val = 0;
        GenerateNumericBox2(StockGrid, currentQty, row_id, "AssignBranchQty", val, 6, 0, 0, 999999, true, !isViewState);

        var txt = "#" + GenerateGridControlID(currentQty, row_id);
        $(txt).CalBranchQty();



        var AssignedBranchQty = StockGrid.cells(row_id, StockGrid.getColIndexById("SumAssignQtyBefore")).getValue();
        var SumNotAssignQtyBefore = StockGrid.cells(row_id, StockGrid.getColIndexById("SumNotAssignQtyBefore")).getValue();
        var AssignBranchQty = StockGrid.cells(row_id, StockGrid.getColIndexById("AssignBranchQtyBefore")).getValue();

        var NewAssignedBranchQty = parseInt(AssignedBranchQty) - parseInt(AssignBranchQty) + parseInt(Value);
        var NewSumNotAssignQty = parseInt(SumNotAssignQtyBefore) + parseInt(AssignBranchQty) - parseInt(Value);

        if (NewAssignedBranchQty == 0)
            NewAssignedBranchQty = "0";

        if (NewSumNotAssignQty == 0)
            NewSumNotAssignQty = "0";

        setStockGridValue(row_id, "SumAssignQty", NewAssignedBranchQty);
        setStockGridValue(row_id, "SumNotAssign", NewSumNotAssignQty);

        setStockGridValue(row_id, "SumAssignQtyBefore", NewAssignedBranchQty);
        setStockGridValue(row_id, "SumNotAssignQtyBefore", NewSumNotAssignQty);
        setStockGridValue(row_id, "AssignBranchQtyBefore", Value);

    });

};

function btnRemoveAttach_click(row_id) {
    var _colID = CTS240_gridAttach.getColIndexById("AttachFileID");
    var _targID = CTS240_gridAttach.cells(row_id, _colID).getValue();

    var obj = {
        AttachID: _targID
    };
    call_ajax_method_json("/Contract/CTS240_RemoveAttach", obj, function (result, controls) {
        if (result != null) {
            RefreshAttachList();
        }
    });
}

//Add by Jutarat A. on 22032014
function InitLoadAttachList() {

    CTS240_gridAttach = $("#CTS240_gridAttachDocList").LoadDataToGridWithInitial(0, false, false, "/Contract/CTS240_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, false, null,
                            function () {
                                if (hasAlert) {
                                    hasAlert = false;
                                    OpenWarningDialog(alertMsg);
                                }
                                else {
                                    var type = $("input[type='radio'][name='ProcessType']:checked").val();

                                    if (type == "mod") {
                                        colInx = CTS240_gridAttach.getColIndexById("downloadButton")
                                        CTS240_gridAttach.setColumnHidden(colInx, false);
                                        colInx = CTS240_gridAttach.getColIndexById("removeButton")
                                        CTS240_gridAttach.setColumnHidden(colInx, false);
                                        $("#divAttachFrame").show();
                                        $("#divAttachRemark").show();
                                    }
                                    else {
                                        colInx = CTS240_gridAttach.getColIndexById("downloadButton")
                                        CTS240_gridAttach.setColumnHidden(colInx, true);
                                        colInx = CTS240_gridAttach.getColIndexById("removeButton")
                                        CTS240_gridAttach.setColumnHidden(colInx, true);
                                        $("#divAttachFrame").hide();
                                        $("#divAttachRemark").hide();
                                    }
                                }
                                $('#frmAttach').load(RefreshAttachList);

                                isInitAttachGrid = true;
                            });
}
//End Add

function RefreshAttachList() {

    //if (CTS240_gridAttach != null) {
    if (CTS240_gridAttach != undefined && isInitAttachGrid) { //Modify by Jutarat A. on 22032014
        $('#CTS240_gridAttachDocList').LoadDataToGrid(CTS240_gridAttach, 0, false, "/Contract/CTS240_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, function () {
            if (hasAlert) {
                hasAlert = false;
                OpenWarningDialog(alertMsg);
            }
            else {
                var type = $("input[type='radio'][name='ProcessType']:checked").val();

                if (type == "mod") {
                    colInx = CTS240_gridAttach.getColIndexById("downloadButton")
                    CTS240_gridAttach.setColumnHidden(colInx, false);
                    colInx = CTS240_gridAttach.getColIndexById("removeButton")
                    CTS240_gridAttach.setColumnHidden(colInx, false);
                    $("#divAttachFrame").show();
                    $("#divAttachRemark").show();
                }
                else {
                    colInx = CTS240_gridAttach.getColIndexById("downloadButton")
                    CTS240_gridAttach.setColumnHidden(colInx, true);
                    colInx = CTS240_gridAttach.getColIndexById("removeButton")
                    CTS240_gridAttach.setColumnHidden(colInx, true);
                    $("#divAttachFrame").hide();
                    $("#divAttachRemark").hide();
                }
            }
        }, null)
    }
}

function CTS240_gridAttachBinding() {
    //if (isInitAttachGrid) { 
    if (CTS240_gridAttach != undefined) { //Modify by Jutarat A. on 22032014
        var _colRemoveBtn = CTS240_gridAttach.getColIndexById("removeButton");

        for (var i = 0; i < CTS240_gridAttach.getRowsNum(); i++) {
            var row_id = CTS240_gridAttach.getRowId(i);
            GenerateRemoveButton(CTS240_gridAttach, "btnRemoveAttach", row_id, "removeButton", true);
            GenerateDownloadButton(CTS240_gridAttach, "btnDownloadAttach", row_id, "downloadButton", true);
            BindGridButtonClickEvent("btnRemoveAttach", row_id, btnRemoveAttach_click);
            BindGridButtonClickEvent("btnDownloadAttach", row_id, btnDownloadAttach_clicked);
        }
    }
    //Comment by Jutarat A. on 22032014
    //} else {
    //    isInitAttachGrid = true;
    //}
    //End Comment

    CTS240_gridAttach.setSizes();
}

function btnDownloadAttach_clicked(row_id) {
    var _colID = CTS240_gridAttach.getColIndexById("AttachFileID");
    var _targID = CTS240_gridAttach.cells(row_id, _colID).getValue();

    var obj = {
        AttachID: _targID
    };

    //Modify by Jutarat A. on 31012013
//    var key = ajax_method.GetKeyURL(null);
//    var link = ajax_method.GenerateURL("/Contract/CTS240_DownloadAttach" + "?AttachID=" + _targID + "&k=" + key);

//    window.open(link, "download");
    download_method.CallDownloadController("ifDownload", "/Contract/CTS240_DownloadAttach", obj);
    //End Modify
}

function doAlertWithMessage(msgCode, msgText) {
    hasAlert = true;
    alertMsg = msgText;
}