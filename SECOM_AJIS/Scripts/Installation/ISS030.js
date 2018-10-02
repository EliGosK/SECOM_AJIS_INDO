

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
var ISS030_GridEmail;
var ISS030_GridInstrumentInfo;
var ISS030_GridFacility;

var strNewRow = "NEWROW";
var strTrue = "true";
var strFalse = "false";
var doPreviousRentalInstrumentdataList;
var doPreviousSaleInstrumentdataList;
var keepBlnUseContractData;
var doPreviousTbtInstallationSlip;
var tempInstallationTypeCode;

var bLineUpStopSale;
var FlagCustomerReasonWithoutNewWork;

var doPreviousRentalInstrumentdataExchangeList; //Add by Jutarat A. on 17062013
var doPreviousSaleInstrumentdataExchangeList; //Add by Jutarat A. on 17062013


var isInitAttachGrid = false;
var hasAlert = false;
var ISS030 = {

    GridColumnID: {
        AttachFile: {
            FileName: "FileName",
            AttachFileID: "AttachFileID",
            BtnRemove: "removeButton", BtnRemoveID: "btnRemoveAttach",
            TmpColumn: "TmpColumn"
        }
    },

    Grids: {
        gridAttach: null
    },

    jQuery: {
        AttachFileSection: {
            divAttachRemark: function () { return $("#divAttachRemark"); },
            formAttach: function () { return $("#frmAttach"); },
            divAttachFrame: function () { return $("#divAttachFrame"); },
            gridAttachDocList: function () { return $("#ISS030_gridAttachDocList"); }
        }
    },

    Functions: {
        InitialAttachFileGrid: function () {
            ISS030.Grids.gridAttach = ISS030.jQuery.AttachFileSection.gridAttachDocList().LoadDataToGridWithInitial(
                                            0, false, false, "/Installation/ISS030_LoadGridAttachedDocList", "",
                                            "dtAttachFileForGridView", false, false, null,
                                            function () {
                                                if (hasAlert) {
                                                    hasAlert = false;
                                                    OpenWarningDialog(alertMsg);
                                                }
                                                ISS030.jQuery.AttachFileSection.formAttach().load(ISS030.Functions.RefreshAttachList);

                                                isInitAttachGrid = true;
                                            });

            SpecialGridControl(ISS030.Grids.gridAttach, [ISS030.GridColumnID.AttachFile.BtnRemove]);
            BindOnLoadedEvent(ISS030.Grids.gridAttach, ISS030.Functions.GridAttachBinding);
        },

        RefreshAttachList: function () {
            var grid = ISS030.Grids.gridAttach;

            if (grid != undefined && isInitAttachGrid) {

                ISS030.jQuery.AttachFileSection.gridAttachDocList().LoadDataToGrid(grid, 0, false, "/Installation/ISS030_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, function () {
                    if (hasAlert) {
                        hasAlert = false;
                        OpenWarningDialog(alertMsg);
                    }
                }, null)
            }
        },

        ClearAllAttachFile: function () {

            if (ISS030.Grids.gridAttach.getRowsNum() > 0)
                DeleteAllRow(ISS030.Grids.gridAttach);

            call_ajax_method_json("/Installation/ISS030_ClearAllAttach", null, null);
        },

        GridAttachBinding: function () {
            var grid = ISS030.Grids.gridAttach;

            if (grid != undefined) {

                for (var i = 0; i < grid.getRowsNum(); i++) {
                    var row_id = grid.getRowId(i);
                    GenerateRemoveButton(grid, ISS030.GridColumnID.AttachFile.BtnRemoveID, row_id, ISS030.GridColumnID.AttachFile.BtnRemove, true);
                    BindGridButtonClickEvent(ISS030.GridColumnID.AttachFile.BtnRemoveID, row_id, ISS030.EventHandlers.btnRemoveAttach_click);
                }
            }

            grid.setSizes();
        },

        EnabledGridAttach: function (enabled) {

            var grid = ISS030.Grids.gridAttach;
            if (grid != undefined) {
                for (var i = 0; i < grid.getRowsNum(); i++) {
                    var row_id = grid.getRowId(i);
                    EnableGridButton(grid, ISS030.GridColumnID.AttachFile.BtnRemoveID, row_id, ISS030.GridColumnID.AttachFile.BtnRemove, enabled);
                }
                //GridControl.SetDisabledButtonOnGrid(grid, ISS030.GridColumnID.AttachFile.BtnRemoveID, IVS020.GridColumnID.SearchResult.BtnEdit, !enabled);

                var colInx = grid.getColIndexById(ISS030.GridColumnID.AttachFile.BtnRemove)
                grid.setColumnHidden(colInx, !enabled);

                if (enabled)
                    GridControl.UnlockGrid(grid);
                else
                    GridControl.LockGrid(grid);

            }
        },

        ShowAttachFileSection: function (isShow) {
            if (isShow) {
                ISS030.jQuery.AttachFileSection.formAttach().show();
                ISS030.jQuery.AttachFileSection.divAttachRemark().show();
            }
            else {
                ISS030.jQuery.AttachFileSection.formAttach().hide();
                ISS030.jQuery.AttachFileSection.divAttachRemark().hide();
            }
        }
    },

    EventHandlers: {
        btnRemoveAttach_click: function (row_id) {

            var obj = {
                module: "Common",
                code: "MSG0142"
            };

            call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                OpenYesNoMessageDialog(result.Code, result.Message,
                        function () {
                            var _colID = ISS030.Grids.gridAttach.getColIndexById("AttachFileID")
                            var _targID = ISS030.Grids.gridAttach.cells(row_id, _colID).getValue();

                            var obj = {
                                AttachID: _targID
                            };
                            call_ajax_method_json("/Installation/ISS030_RemoveAttach", obj, function (result, controls) {
                                if (result != null) {
                                    ISS030.Functions.RefreshAttachList();
                                }
                            });
                        });
            });
        }
    }
}


// Main
$(document).ready(function () {


    var strContractProjectCode = $("#ContractCodeProjectCode").val();
    $("#btnSearchFacility").click(search_facility_click);
    $("#NewBldMgmtCost").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#BillingInstallFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#NormalInstallFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#NormalContractFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#InstrumentQty").BindNumericBox(4, 0, 0, 9999.99, true);
    $("#FacilityQuantity").BindNumericBox(4, 0, 0, 9999.99, true);

    $("#SECOMPaymentFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#SECOMRevenueFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);

    $("#SlipIssueDate").InitialDate();
    $("#ExpectedInstrumentArrivalDate").InitialDate();

    if ($("#NewPhoneLineOpenDate").length > 0) {
        $("#NewPhoneLineOpenDate").InitialDate();
    }


    $("#ChangeContents").SetMaxLengthTextArea(4000);
    $("#Memo").SetMaxLengthTextArea(4000);


    //$("#btnRetrieveInstallation").click(retrieve_installation_click);
    //$("#btnClearInstallation").click(clear_installation_click);

    $("#btnAdd").click(function () { BtnAddClick(); });

    $("#btnClear").click(function () { BtnClearClick(); });
    $("#btnSearchEmail").click(function () { $("#dlgCTS053").OpenCMS060Dialog("CTS053"); });

    $("#ChangeCustomerReason").click(function () { initalCauseReasonDropdownlist(); });
    $("#ChangeSecomReason").click(function () { initalCauseReasonDropdownlist(); });

    $("#btnSearchInstrument").click(search_instrument_click);
    $("#btnAddInstrument").click(AddInstrumentInfo);
    $("#btnResetAdditional").click(OnResetAdditional);
    $("#btnClearInstrument").click(ClearInstrumentInput);

    $("#InstrumentCode").blur(GetInstrumentInfoData);
    $("#InstallationType").change(function () { ChangeInstallationType(); }); //ResetAdditional(); }); //Move to ChangeInstallationType() by Jutarat A. on 17062013
    //$("#NormalInstallFee").change(initialSECOMNPayment);

    // 20170217 nakajima modify start
    //$("#NormalInstallFee").blur(initialSECOMNPayment);
    //$("#BillingInstallFee").blur(initialSECOMNPayment);
    // 20170217 nakajima modify end

    //$("#InstallationBy").change(ChangeInstallationBy);

    $("#FacilityCode").blur(FacilityCode_Blur);
    $("#btnAddFacility").click(Add_Facility_Click);
    $("#btnClearFacility").click(Clear_Facility_Click);

    $("#divResultRegisSlip").hide();
    // intial grid
    initialGridOnload();

    if (strContractProjectCode != "") {
        $("#ContractCodeProjectCode").val(strContractProjectCode)
        setTimeout("retrieve_installation_click()", 2000);
    }

    //================ ATTACH FILE SECTION ========================================    
    ISS030.jQuery.AttachFileSection.formAttach().attr('src', 'ISS030_Upload?k=' + _attach_k);
    //====================================================================

    setInitialState();

});

function setInitialState() {
    // --------------- Initial SCREEN ------------------
    $("#divContractBasicInfo").SetViewMode(false);
    $("#divInstallationInfo").SetViewMode(false);
    $("#divInstallationInstrumentInfo").SetViewMode(false);


    enabledGridEmail();

    $("#ContractCodeProjectCode").attr("readonly", false);
    $("#btnRetrieveInstallation").attr("disabled", false);
    $("#btnClearInstallation").attr("disabled", false);

    //disabledInputControls();

    InitialCommandButton(0);
    $("#divInputContractCode").clearForm();
    $("#divContractBasicInfo").clearForm();
    $("#divInstallationInstrumentInfo").clearForm();
    $("#divInstallationInfo").clearForm();
    $("#divResultRegisSlip").clearForm();

    $("#divInputContractCode").show();
    $("#divContractBasicInfo").show();
    $("#divInstallationInstrumentInfo").show();
    $("#divInstallationInfo").show();
    //$("#divResultRegisSlip").show();  
    //--------------------------------------------------

    // - Add by Nontawat L. on 03-Jul-2014
    ISS030.jQuery.AttachFileSection.gridAttachDocList().show();
    ISS030.Functions.EnabledGridAttach(true);
    ISS030.Functions.ShowAttachFileSection(true);
    // - End add     
}





function retrieve_installation_click() {
    //InitialCommandButton(1);
    command_control.CommandControlMode(false);
    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
    call_ajax_method_json("/Installation/ISS030_RetrieveData", obj,
        function (result, controls) {
            command_control.CommandControlMode(true);
            if (controls != undefined) {


                $("#divInputContractCode").clearForm();
                $("#divContractBasicInfo").clearForm();
                $("#divInstallationInstrumentInfo").clearForm();
                $("#divInstallationInfo").clearForm();
                $("#divResultRegisSlip").clearForm();

                return;
            }
            else if (result != undefined) {

                DeleteAllRow(ISS030_GridEmail);
                var obj = { strFieldName: "" };

                $("#divInputContractCode").clearForm();
                $("#divContractBasicInfo").clearForm();
                $("#divInstallationInstrumentInfo").clearForm();
                $("#divInstallationInfo").clearForm();
                $("#divResultRegisSlip").clearForm();

                $("#ServiceTypeCode").val(result.ServiceTypeCode);
                initialInstallationType(result);

                if (result.InstallType != undefined) {
                    obj.strFieldName = result.InstallType;
                    call_ajax_method('/Installation/ISS030_GetMiscInstallationtype', obj, function (result2, controls) {
                        if (result2 != null) {
                            if (result2.List.length != 1) {
                                regenerate_combo("#InstallationType", result2);
                                initialInstallationType(result);
                                initialInstallationType2($("#InstallationTypeCode").val());
                            }
                        }
                        initialScreenOnRetrieve(result);
                    });
                }
                else {
                    $("#InstallationType").attr("disabled", true);
                }


                if (result.do_TbtInstallationSlip != null) {
                    if (result.do_TbtInstallationSlip.SlipIssueDate != null)
                        result.do_TbtInstallationSlip.SlipIssueDate = ConvertDateToTextFormat(result.do_TbtInstallationSlip.SlipIssueDate.replace('/Date(', '').replace(')/', '') * 1);
                    if (result.do_TbtInstallationSlip.ExpectedInstrumentArrivalDate != null)
                        result.do_TbtInstallationSlip.ExpectedInstrumentArrivalDate = ConvertDateToTextFormat(result.do_TbtInstallationSlip.ExpectedInstrumentArrivalDate.replace('/Date(', '').replace(')/', '') * 1);


                }
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
                $("#divInstallationInfo").bindJSON(result.do_TbtInstallationBasic);
                $("#divContractBasicInfo").bindJSON(result.do_TbtInstallationSlip);
                $("#divInstallationInfo").bindJSON(result.do_TbtInstallationSlip);
                $("#divInstallationInstrumentInfo").bindJSON(result.do_TbtInstallationSlip);





                //$("#ContractCode").val(result.ContractCodeShort);
                $("#ContractCode").val(result.ContractProjectCodeForShow);
                var AmountAddInstallQty = 0;
                if (result.do_TbtInstallationSlip != null) {
                    $("#SlipStatus").val(result.do_TbtInstallationSlip.SlipStatus);
                    //                    if (result.do_TbtInstallationSlip.SlipStatus == C_SLIP_STATUS_NOT_STOCK_OUT || result.do_TbtInstallationSlip.SlipStatus == C_SLIP_STATUS_PARTIAL_STOCK_OUT) {
                    //                        AmountAddInstallQty = result.do_TbtInstallationSlipDetails[i].AddInstalledQty;
                    //                    }
                    $("#SlipStatusName").val(result.LastSlipStatusName);
                }
                if (result.do_TbtInstallationBasic != null && result.do_TbtInstallationBasic != undefined) {
                    $("#InstallationMANo").val(result.do_TbtInstallationBasic.MaintenanceNo);
                }
                /////////////// BIND EMAIl DATA //////////////////
                if (result.ListDOEmail != null) {
                    if (result.ListDOEmail.length > 0) {
                        for (var i = 0; i < result.ListDOEmail.length; i++) {
                            var emailList = [result.ListDOEmail[i].EmailAddress, "", result.ListDOEmail[i].EmpNo];

                            CheckFirstRowIsEmpty(ISS030_GridEmail, true);
                            AddNewRow(ISS030_GridEmail, emailList);

                        }

                        BindOnloadGridEmail();
                    }
                }
                //////////////////////////////////////////////////

                /////////////// BIND Instrument DATA //////////////////
                //                if (result.do_TbtInstallationSlipDetails != null) {
                //                    if (result.do_TbtInstallationSlipDetails.length > 0) {
                //                        for (var i = 0; i < result.do_TbtInstallationSlipDetails.length; i++) {

                //                            var InstrumentList = [result.do_TbtInstallationSlipDetails[i].InstrumentCode, result.arrayInstrumentName[i], qtyConvert(result.do_TbtInstallationSlipDetails[i].ContractInstalledQty), qtyConvert(result.do_TbtInstallationSlipDetails[i].TotalStockOutQty), AmountAddInstallQty + "", result.do_TbtInstallationSlipDetails[i].ReturnQty + "", result.do_TbtInstallationSlipDetails[i].AddRemovedQty + "", result.do_TbtInstallationSlipDetails[i].ContractInstalledQty + result.do_TbtInstallationSlipDetails[i].AddInstalledQty - result.do_TbtInstallationSlipDetails[i].AddRemovedQty + result.do_TbtInstallationSlipDetails[i].TotalStockOutQty, result.do_TbtInstallationSlipDetails[i].MoveQty + "", result.do_TbtInstallationSlipDetails[i].MAExchangeQty + "", "", "", "", "", "", result.do_TbtInstallationSlipDetails[i].PartialStockOutQty];

                //                            CheckFirstRowIsEmpty(ISS030_GridInstrumentInfo, true);
                //                            AddNewRow(ISS030_GridInstrumentInfo, InstrumentList);
                //                        }
                //                    }
                //                    else {
                //                        setTimeout('DeleteAllRow(ISS030_GridInstrumentInfo)', 2000);
                //                    }
                //                }
                //                else {
                //                    setTimeout('DeleteAllRow(ISS030_GridInstrumentInfo)', 2000);
                //                }

                doPreviousRentalInstrumentdataList = result.doRentalInstrumentdataList;
                doPreviousSaleInstrumentdataList = result.doSaleInstrumentdataList;
                keepBlnUseContractData = result.blnUseContractData;
                doPreviousTbtInstallationSlip = result.do_TbtInstallationSlip;

                doPreviousRentalInstrumentdataExchangeList = result.doRentalInstrumentdataExchangeList; //Add by Jutarat A. on 17062013
                doPreviousSaleInstrumentdataExchangeList = result.doSaleInstrumentdataExchangeList; //Add by Jutarat A. on 17062013

                if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL) {
                    if (result.doRentalInstrumentdataList.length > 0) {
                        for (var i = 0; i < result.doRentalInstrumentdataList.length; i++) {

                            var tempAdditionalInstalled = 0;
                            if (result.blnUseContractData == true) {
                                //tempAdditionalInstalled = result.doRentalInstrumentdataList[i].CAdditionalInstrumentQty;
                                if (result.doRentalInstrumentdataList[i].ITotalStockOutQty < result.doRentalInstrumentdataList[i].CAdditionalInstrumentQty) {
                                    tempAdditionalInstalled = (result.doRentalInstrumentdataList[i].CAdditionalInstrumentQty - result.doRentalInstrumentdataList[i].ITotalStockOutQty);
                                }
                                else {
                                    tempAdditionalInstalled = 0;
                                }
                            }
                            else if (result.do_TbtInstallationSlip != null && result.do_TbtInstallationSlip.SlipStatus == C_SLIP_STATUS_STOCK_OUT) {
                                tempAdditionalInstalled = 0;
                            }
                            //Add by Jutarat A. on 16082013
                            else if (result.do_TbtInstallationSlip != null && result.do_TbtInstallationSlip.SlipStatus == C_SLIP_STATUS_PARTIAL_STOCK_OUT) {
                                tempAdditionalInstalled = (result.doRentalInstrumentdataList[i].IAddInstalledQty - result.doRentalInstrumentdataList[i].ICurrentStockOutQty);
                            }
                            //End Add
                            else {
                                tempAdditionalInstalled = result.doRentalInstrumentdataList[i].IAddInstalledQty;
                            }

                            var tempReturn = 0;
                            if (result.blnUseContractData == true) {
                                if (result.doRentalInstrumentdataList[i].ITotalStockOutQty > result.doRentalInstrumentdataList[i].CAdditionalInstrumentQty) {
                                    tempReturn = (result.doRentalInstrumentdataList[i].ITotalStockOutQty - result.doRentalInstrumentdataList[i].CAdditionalInstrumentQty);
                                }
                                else {
                                    tempReturn = 0;
                                }
                            }
                            else {
                                tempReturn = result.doRentalInstrumentdataList[i].IReturnQty;
                            }

                            //                            var tempRemoved = 0;
                            //                            if (result.blnUseContractData == true) {
                            //                                tempRemoved = result.doRentalInstrumentdataList[i].CRemovalInstrumentQty;
                            //                            }
                            //                            else {
                            //                                tempRemoved = result.doRentalInstrumentdataList[i].IAddRemovedQty;
                            //                            }
                            var tempRemoved = 0;
                            if ($("#InstallationTypeCode").val() == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
                                || $("#InstallationTypeCode").val() == C_RENTAL_INSTALL_TYPE_REMOVE_ALL
                                || $("#InstallationTypeCode").val() == C_SALE_INSTALL_TYPE_REMOVE_ALL
                            ) {
                                tempRemoved = result.doRentalInstrumentdataList[i].CInstrumentQty
                            }
                            else {
                                if (result.blnUseContractData == true) {
                                    tempRemoved = result.doRentalInstrumentdataList[i].CRemovalInstrumentQty;
                                }
                                else {
                                    tempRemoved = result.doRentalInstrumentdataList[i].IAddRemovedQty;
                                }
                            }
                            var tempContractInstalledAfter = result.doRentalInstrumentdataList[i].CInstrumentQty + result.doRentalInstrumentdataList[i].ITotalStockOutQty + tempAdditionalInstalled - tempReturn - tempRemoved;

                            var InstrumentList = [  //Modify by Jutarat A. on 27112013
                                                    //result.doRentalInstrumentdataList[i].InstrumentCode
                                                    //, result.doRentalInstrumentdataList[i].InstrumentName
                                                    ConvertBlockHtml(result.doRentalInstrumentdataList[i].InstrumentCode)
                                                    , ConvertBlockHtml(result.doRentalInstrumentdataList[i].InstrumentName)
                                                    //End Modify

                                                 , qtyConvert(result.doRentalInstrumentdataList[i].CInstrumentQty)
                                                 , qtyConvert(result.doRentalInstrumentdataList[i].ITotalStockOutQty)
                                                 , tempAdditionalInstalled + ""
                                                 , tempReturn + ""
                                                 , tempRemoved + ""
                                                 , tempContractInstalledAfter + ""
                                                 , qtyConvert(result.doRentalInstrumentdataList[i].IMoveQty)
                                                 , qtyConvert(result.doRentalInstrumentdataList[i].IMAExchangeQty), result.doRentalInstrumentdataList[i].LineUpTypeCode, result.doRentalInstrumentdataList[i].InstrumentTypeCode, "", "", ""
                                                 , "", result.doRentalInstrumentdataList[i].InstrumentPrice, ""
                                                 , result.doRentalInstrumentdataList[i].ICurrentStockOutQty //Add by Jutarat A. on 08072013
                                                 , tempAdditionalInstalled + ""
                                                 , tempRemoved + ""
                                                 , tempReturn + ""
                                                 ];
                            CheckFirstRowIsEmpty(ISS030_GridInstrumentInfo, true);
                            AddNewRow(ISS030_GridInstrumentInfo, InstrumentList);
                        }
                    }
                    else {
                        setTimeout('DeleteAllRow(ISS030_GridInstrumentInfo)', 2000);
                    }
                }
                else if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_SALE) {
                    if (result.doSaleInstrumentdataList.length > 0) {
                        for (var i = 0; i < result.doSaleInstrumentdataList.length; i++) {

                            //========= calculate Remove Qty =============
                            var tempRemoved = 0;
                            if ($("#InstallationTypeCode").val() == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
                                $("#InstallationTypeCode").val() == C_SALE_INSTALL_TYPE_MOVE ||
                                $("#InstallationTypeCode").val() == C_SALE_INSTALL_TYPE_PARTIAL_REMOVE) {
                                tempRemoved = result.doSaleInstrumentdataList[i].IAddRemovedQty;
                            }
                            else if ($("#InstallationTypeCode").val() == C_SALE_INSTALL_TYPE_REMOVE_ALL) {
                                tempRemoved = result.doSaleInstrumentdataList[i].CInstrumentQty;
                            }
                            else {
                                tempRemoved = result.doSaleInstrumentdataList[i].CRemovalInstrumentQty;
                            }

                            //========= calculate Additional Qty =============
                            var tempAdditionalInstall = 0;

                            //Modify by Jutarat A. on 14062013
                            //                            //                            if (result.do_TbtInstallationSlip != null && result.do_TbtInstallationSlip.SlipStatus == C_SLIP_STATUS_STOCK_OUT) {
                            //                            //                                tempAdditionalInstall = 0;
                            //                            //                            }
                            //                            //                            else {
                            //                            //                                tempAdditionalInstall = result.doSaleInstrumentdataList[i].CAdditionalInstrumentQty;
                            //                            //                            }
                            //                            if ($("#InstallationTypeCode").val() == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
                            //                                $("#InstallationTypeCode").val() == C_SALE_INSTALL_TYPE_MOVE ||
                            //                                $("#InstallationTypeCode").val() == C_SALE_INSTALL_TYPE_PARTIAL_REMOVE ||
                            //                                $("#InstallationTypeCode").val() == C_SALE_INSTALL_TYPE_REMOVE_ALL) {
                            //                                tempAdditionalInstall = 0;
                            //                            }
                            //                            else if (result.doSaleInstrumentdataList[i].CAdditionalInstrumentQty > result.doSaleInstrumentdataList[i].ITotalStockOutQty) {
                            //                                tempAdditionalInstall = (result.doSaleInstrumentdataList[i].CAdditionalInstrumentQty - result.doSaleInstrumentdataList[i].ITotalStockOutQty);
                            //                            } else {
                            //                                tempAdditionalInstall = 0;
                            //                            }
                            if ($("#InstallationTypeCode").val() == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE) {
                                tempAdditionalInstall = (result.doSaleInstrumentdataList[i].IAddInstalledQty - result.doSaleInstrumentdataList[i].ITotalStockOutQty);
                            }
                            else if (result.doSaleInstrumentdataList[i].ITotalStockOutQty < result.doSaleInstrumentdataList[i].CAdditionalInstrumentQty) {
                                tempAdditionalInstall = (result.doSaleInstrumentdataList[i].CAdditionalInstrumentQty - result.doSaleInstrumentdataList[i].ITotalStockOutQty);
                            }
                            else {
                                tempAdditionalInstall = 0;
                            }
                            //End Modify

                            //========= calculate Return Qty =============
                            var tempReturnQty = 0;

                            //Modify by Jutarat A. on 14062013
                            //                            //                            if (result.do_TbtInstallationSlip != null && result.do_TbtInstallationSlip.SlipStatus == C_SLIP_STATUS_STOCK_OUT) {
                            //                            //                                tempReturnQty = (result.doSaleInstrumentdataList[i].ITotalStockOutQty == null ? 0 : result.doSaleInstrumentdataList[i].ITotalStockOutQty) - (result.doSaleInstrumentdataList[i].CAdditionalInstrumentQty == null ? 0 : result.doSaleInstrumentdataList[i].CAdditionalInstrumentQty);
                            //                            //                            }
                            //                            //                            else {
                            //                            //                                tempReturnQty = 0;
                            //                            //                            }
                            //                            if ($("#InstallationTypeCode").val() == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
                            //                                $("#InstallationTypeCode").val() == C_SALE_INSTALL_TYPE_MOVE ||
                            //                                $("#InstallationTypeCode").val() == C_SALE_INSTALL_TYPE_PARTIAL_REMOVE ||
                            //                                $("#InstallationTypeCode").val() == C_SALE_INSTALL_TYPE_REMOVE_ALL) {
                            //                                tempReturnQty = 0;
                            //                            }
                            //                            else if (result.doSaleInstrumentdataList[i].CAdditionalInstrumentQty < result.doSaleInstrumentdataList[i].ITotalStockOutQty) {
                            //                                tempReturnQty = (result.doSaleInstrumentdataList[i].ITotalStockOutQty - result.doSaleInstrumentdataList[i].CAdditionalInstrumentQty);
                            //                            } else {
                            //                                tempReturnQty = 0;
                            //                            }
                            if (result.doSaleInstrumentdataList[i].ITotalStockOutQty > result.doSaleInstrumentdataList[i].CAdditionalInstrumentQty) {
                                tempReturnQty = (result.doSaleInstrumentdataList[i].ITotalStockOutQty - result.doSaleInstrumentdataList[i].CAdditionalInstrumentQty);
                            }
                            else {
                                tempReturnQty = 0;
                            }
                            //End Modify

                            var tempContractInstalledAfter = result.doSaleInstrumentdataList[i].CInstrumentQty + result.doSaleInstrumentdataList[i].ITotalStockOutQty + tempAdditionalInstall - tempReturnQty - tempRemoved;

                            var InstrumentList = [  //Modify by Jutarat A. on 27112013
                                                    //result.doSaleInstrumentdataList[i].InstrumentCode
                                                    //, result.doSaleInstrumentdataList[i].InstrumentName
                                                    ConvertBlockHtml(result.doSaleInstrumentdataList[i].InstrumentCode)
                                                    , ConvertBlockHtml(result.doSaleInstrumentdataList[i].InstrumentName)
                                                    //End Modify
                                                 
                                                 , qtyConvert(result.doSaleInstrumentdataList[i].CInstrumentQty)
                                                 , qtyConvert(result.doSaleInstrumentdataList[i].ITotalStockOutQty)
                                                 , tempAdditionalInstall + ""
                                                 , tempReturnQty + ""
                                                 , tempRemoved + ""
                                                 , tempContractInstalledAfter + ""
                                                 , qtyConvert(result.doSaleInstrumentdataList[i].IMoveQty)
                                                 , qtyConvert(result.doSaleInstrumentdataList[i].IMAExchangeQty), result.doSaleInstrumentdataList[i].LineUpTypeCode, result.doSaleInstrumentdataList[i].InstrumentTypeCode, "", "", ""
                                                 , "", result.doSaleInstrumentdataList[i].InstrumentPrice, ""
                                                 , result.doSaleInstrumentdataList[i].ICurrentStockOutQty //Add by Jutarat A. on 08072013
                                                 , tempAdditionalInstall + ""
                                                 , tempRemoved + ""
                                                 , tempReturnQty + ""
                                                 ];
                            CheckFirstRowIsEmpty(ISS030_GridInstrumentInfo, true);
                            AddNewRow(ISS030_GridInstrumentInfo, InstrumentList);
                        }
                    }
                    else {
                        setTimeout('DeleteAllRow(ISS030_GridInstrumentInfo)', 2000);
                    }
                }
                else {
                    setTimeout('DeleteAllRow(ISS030_GridInstrumentInfo)', 2000);
                }


                //////////////////////////////////////////////////

                ////////////// BIND FACILITY DETAIL //////////////////
                if (result.do_TbtInstallationSlipDetailsForFacility != null && result.do_TbtInstallationSlipDetailsForFacility.length > 0) {
                    for (var i = 0; i < result.do_TbtInstallationSlipDetailsForFacility.length; i++) {

                        //Modify by Jutarat A. on 28112013
                        //var FacilityList = [result.do_TbtInstallationSlipDetailsForFacility[i].InstrumentCode, result.do_TbtInstallationSlipDetailsForFacility[i].InstrumentName, qtyConvert(result.do_TbtInstallationSlipDetailsForFacility[i].AddInstalledQty), ""];
                        var FacilityList = [ConvertBlockHtml(result.do_TbtInstallationSlipDetailsForFacility[i].InstrumentCode)
                                            , ConvertBlockHtml(result.do_TbtInstallationSlipDetailsForFacility[i].InstrumentName)
                                            , qtyConvert(result.do_TbtInstallationSlipDetailsForFacility[i].AddInstalledQty)
                                            , ""];
                        //End Modify

                        CheckFirstRowIsEmpty(ISS030_GridFacility, true);
                        AddNewRow(ISS030_GridFacility, FacilityList);
                    }
                }
                else if (result.dtRentalInstrumentDetailsForFacility != null && result.dtRentalInstrumentDetailsForFacility.length > 0) {
                    for (var i = 0; i < result.dtRentalInstrumentDetailsForFacility.length; i++) {

                        //Modify by Jutarat A. on 28112013
                        //var FacilityList = [result.dtRentalInstrumentDetailsForFacility[i].InstrumentCode, result.dtRentalInstrumentDetailsForFacility[i].InstrumentName, qtyConvert(result.dtRentalInstrumentDetailsForFacility[i].InstrumentQty), ""];
                        var FacilityList = [ConvertBlockHtml(result.dtRentalInstrumentDetailsForFacility[i].InstrumentCode)
                                            , ConvertBlockHtml(result.dtRentalInstrumentDetailsForFacility[i].InstrumentName)
                                            , qtyConvert(result.dtRentalInstrumentDetailsForFacility[i].InstrumentQty)
                                            , ""];
                        //End Modify

                        CheckFirstRowIsEmpty(ISS030_GridFacility, true);
                        AddNewRow(ISS030_GridFacility, FacilityList);
                    }
                }
                else {
                    setTimeout('DeleteAllRow(ISS030_GridFacility)', 2000);
                }
                //////////////////////////////////////////////////////


                //initialScreenOnRetrieve(result);

                //======================== INITIAL VALUE =========================================
                $("#m_blnbFirstTimeRegister").val(result.m_blnbFirstTimeRegister);
                //                $("#NewNormalContractFee").val($("#NormalContractFee").val());
                //                $("#OldNormalContractFee").val($("#NormalContractFee").val
                if (result.doRentalFeeResult != null) {
                    $("#NewNormalContractFee").val(result.doRentalFeeResult.NormalContractFee);
                    $("#OldNormalContractFee").val(result.doRentalFeeResult.NormalContractFee);
                }
                //                if (result.dtRentalContractBasic != null)
                //                {
                //                    $("#OldNormalContractFee").val(result.dtRentalContractBasic.NormalContractFee);
                //                }

                if (result.dtRentalContractBasic != null) {
                    $("#ChangeType").val(result.dtRentalContractBasic.ChangeType);
                }
                if (result.do_TbtInstallationSlip != null) {
                    $("#SlipType").val(result.do_TbtInstallationSlip.SlipType);
                }
                //================================================================================

                setTimeout("manualInitialGridInstrument()", 1500);
                setTimeout("manualInitialGridFacility()", 1500);

                ///// TEST //////
                //SendGridSlipDetailsToObject();

                //$("#NewBldMgmtCost").focus();
                //                $("#BillingInstallFee").focus();
                //                $("#NormalInstallFee").focus();
                //                $("#SECOMPaymentFee").focus();
                //                $("#SECOMRevenueFee").focus();
                if ($("#NewBldMgmtCost").NumericValue() != "")
                    $("#NewBldMgmtCost").val(moneyConvert($("#NewBldMgmtCost").NumericValue()));

                if ($("#SECOMPaymentFee").NumericValue() != "")
                    $("#SECOMPaymentFee").val(moneyConvert($("#SECOMPaymentFee").NumericValue()));

                if ($("#SECOMRevenueFee").NumericValue() != "")
                    $("#SECOMRevenueFee").val(moneyConvert($("#SECOMRevenueFee").NumericValue()));

                // Add By Pachara S. 04082016
                if (result.ServiceTypeCode == C_SERVICE_TYPE_RENTAL)
                {
                    $("#NormalContractFee").SetNumericCurrency(result.doRentalFeeResult.NormalContractFeeCurrencyType);
                    $("#NormalContractFee").val(moneyConvert($("#NormalContractFee").NumericValue()));

                    if (result.dtRentalContractBasic.NormalInstallationFee != null && result.dtRentalContractBasic.NormalInstallationFee != "") {
                        $("#NormalInstallFee").SetNumericCurrency(result.dtRentalContractBasic.NormalInstallationFeeCurrencyType);
                        $("#NormalInstallFee").val(moneyConvert($("#NormalInstallFee").NumericValue()));
                    }

                    if (result.dtRentalContractBasic.BillingInstallationFee != null && result.dtRentalContractBasic.BillingInstallationFee != "") {
                        $("#BillingInstallFee").SetNumericCurrency(result.dtRentalContractBasic.BillingInstallationFeeCurrencyType);
                        $("#BillingInstallFee").val(moneyConvert(result.dtRentalContractBasic.BillingInstallationFee));
                    }

                }
                else
                {
                    // Type Sale Don't have NormalContractFee
                    if ($("#NormalContractFee").val() != null && $("#NormalContractFee").val() != "") {
                        $("#NormalContractFee").val(moneyConvert($("#NormalContractFee").NumericValue()));
                    }

                    if (result.dtSale.NormalInstallationFee != null && result.dtSale.NormalInstallationFee != "") {
                        $("#NormalInstallFee").SetNumericCurrency(result.dtSale.NormalInstallFeeCurrencyType);
                        $("#NormalInstallFee").val(moneyConvert(result.dtSale.NormalInstallationFee));
                    }

                    //BillingInstallFeeCurrencyType is use OrderInstallFeeCurrencyType
                    if (result.dtSale.BillingInstallationFee != null && result.dtSale.BillingInstallationFee != "") {
                        $("#BillingInstallFee").SetNumericCurrency(result.dtSale.OrderInstallFeeCurrencyType);
                        $("#BillingInstallFee").val(moneyConvert(result.dtSale.BillingInstallationFee));
                    }
                }
                
                //$("#BillingInstallFee").SetNumericCurrency(1);
                //if ($("#BillingInstallFee").val() != null && $("#BillingInstallFee").val() != "") {
                //    $("#BillingInstallFee").val(moneyConvert($("#BillingInstallFee").NumericValue()));
                //}

                //if ($("#NormalInstallFee").val() != null && $("#NormalInstallFee").val() != "") {
                //    $("#NormalInstallFee").val(moneyConvert($("#NormalInstallFee").NumericValue()));
                //}

                $("#SECOMRevenueFee").blur();
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
    setInitialState();
    btnClearEmailClick();
}



function BtnAddClick() {

    // Is exist email
    // Fill to grid
    // Keep selected email to sesstion

    var strEmail = $("#EmailAddress").val();
    if (strEmail.replace(/ /g, "") == "") {
        doAlert("Common", "MSG0007", lblEmailAddress);
        VaridateCtrl(["EmailAddress"], ["EmailAddress"]);
    }
    else {
        var email = { "strEmail": $("#EmailAddress").val() + strEmailSuffix };

        call_ajax_method_json("/Installation/ISS030_GetInstallEmail", email, function (result, controls, isWarning) {

            if (isWarning == undefined) { // No error and data(email) is exist

                if (result.length > 0) {
                    // Fill to grid
                    var emailList = [result[0].EmailAddress, "", result[0].EmpNo];

                    CheckFirstRowIsEmpty(ISS030_GridEmail, true);
                    AddNewRow(ISS030_GridEmail, emailList);
                    var colInx = ISS030_GridEmail.getColIndexById('ButtonRemove');
                    for (var i = 0; i < ISS030_GridEmail.getRowsNum(); i++) {
                        var rowId = ISS030_GridEmail.getRowId(i);
                        GenerateRemoveButton(ISS030_GridEmail, "btnRemoveEmail", rowId, "ButtonRemove", true);

                        // binding grid button event 
                        BindGridButtonClickEvent("btnRemoveEmail", rowId, BtnRemoveEmailClick);

                    }
                    ISS030_GridEmail.setSizes();
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
                    var selectedRowIndex = ISS030_GridEmail.getRowIndex(row_id);
                    var mail = ISS030_GridEmail.cells2(selectedRowIndex, ISS030_GridEmail.getColIndexById('EmailAddress')).getValue();
                    var obj = { EmailAddress: mail }
                    DeleteRow(ISS030_GridEmail, row_id);

                    call_ajax_method_json("/Installation/ISS030_RemoveMailClick", obj, function (result, controls, isWarning) {

                    });
                });
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
    $("#divInstallationMANo").SetViewMode(false);
    $("#divInstallationInstrumentInfo").SetViewMode(false);
    enabledGridEmail();

    // - Add by Nontawat L. on 03-Jul-2014
    ISS030.Functions.EnabledGridAttach(true);
    ISS030.Functions.ShowAttachFileSection(true);
    // - End

    $("#NormalContractFee").val($("#OnRetrieveNormalContractFee").val());

    $("#divAddEmail").show();

    //Modify by Jutarat A. on 11042013
//    if ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_NEW
//            || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGE_WIRING
//            || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_NEW
//            || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_ADD
//            || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_CHANGE_WIRING
//        ) {
//        $("#divInstrumentAdd").hide();

//    }
//    else {
//        $("#divInstrumentAdd").show();
//    }
    CheckShowInstrumentAdd();
    //End Modify

    var colInx = ISS030_GridFacility.getColIndexById("FacilityRemove")
    ISS030_GridFacility.setColumnHidden(colInx, false);
    SetFitColumnForBackAction(ISS030_GridFacility, "TempColumn");
    $("#divInsertFacilityDetail").show();
    $("#divRegisterFacilityDetail").SetViewMode(false);
}

//Add by Jutarat A. on 11042013
function CheckShowInstrumentAdd() {
    var strInstallationType = $("#InstallationType").val();
    if (strInstallationType == ""
            || strInstallationType == C_RENTAL_INSTALL_TYPE_CHANGE_WIRING
            || strInstallationType == C_SALE_INSTALL_TYPE_CHANGE_WIRING
            || strInstallationType == C_RENTAL_INSTALL_TYPE_NEW
            || strInstallationType == C_SALE_INSTALL_TYPE_NEW
            || strInstallationType == C_SALE_INSTALL_TYPE_ADD
            || strInstallationType == C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
            || strInstallationType == C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
            || strInstallationType == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE
            || strInstallationType == C_RENTAL_INSTALL_TYPE_MOVE
            || strInstallationType == C_SALE_INSTALL_TYPE_MOVE
            || strInstallationType == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
            || strInstallationType == C_RENTAL_INSTALL_TYPE_REMOVE_ALL
            || strInstallationType == C_SALE_INSTALL_TYPE_REMOVE_ALL) {
        $("#divInstrumentAdd").hide();
    }
    else {
        $("#divInstrumentAdd").show();
    }
}
//End Add

function command_register_click() {
    command_control.CommandControlMode(false);
    VaridateCtrl(["BillingInstallFee", "InstallationType", "CustomerStaffBelonging", "CustomerStaffName", "CustomerStaffPhoneNo", "NewConnectionPhoneNo", "NewPhoneLineOpenDate", "ProposeInstallStartDate", "NewPhoneLineOwnerTypeCode", "CauseReason", "ChangeReasonCode", "InstallationBy", "SlipIssueOfficeCode", "NormalInstallFee", "InstallFeeBillingType", "PlanCode"], null); //Add (InstallFeeBillingType) by Jutarat A. on 11042013
    //enableInputControls();
    SendGridSlipDetailsToObject();
    var causeByCode = "";
    if ($("#ChangeCustomerReason").prop("checked") == true) {
        causeByCode = C_INSTALL_CHANGE_REASON_CUS;
    }
    else if ($("#ChangeSecomReason").prop("checked") == true) {
        causeByCode = C_INSTALL_CHANGE_REASON_SECOM;
    }

    var obj = CreateObjectData($("#form1").serialize()
                                //+ "&Memo=" + $("#Memo").val() + "&ChangeContents=" + $("#ChangeContents").val() 
                                + "&" + $("#form2").serialize()
                                + "&ExpectedInstrumentArrivalDate="
                                + $("#ExpectedInstrumentArrivalDate").val() + "&StockOutTypeCode="
                                + $("#StockOutTypeCode").val() + "&BlnHaveNewRow=" + $("#BlnHaveNewRow").val()
                                + "&InstallationType=" + $("#InstallationType").val()
                                + "&ChangeCustomerReason=" + $("#ChangeCustomerReason").val()
                                + "&ChangeSecomReason=" + $("#ChangeSecomReason").val()
                                + "&CauseReason=" + $("#CauseReason").val()
                                + "&InstallFeeBillingType=" + $("#InstallFeeBillingType").val()
                                + "&BillingOCC=" + $("#BillingOCC").val()
                                + "&ChangeReasonCode=" + causeByCode
                                + "&MaintenanceNo=" + $("#InstallationMANo").val()
                                + "&InstallationBy=" + $("#InstallationBy").val()
                                + "&disabledInstallFeeBillingType=" + $("#InstallFeeBillingType").prop("disabled")
                                 );

    obj.NormalContractFeeCurrencyType = $("#NormalContractFee").NumericCurrencyValue();
    if (obj.NormalContractFeeCurrencyType == C_CURRENCY_LOCAL) {
        obj.NormalContractFee = obj.NormalContractFee;
        obj.NormalContractFeeUsd = null;
    }
    else {
        obj.NormalContractFeeUsd = obj.NormalContractFee;
        obj.NormalContractFee = null;
    }

    obj.NormalInstallFeeCurrencyType = $("#NormalInstallFee").NumericCurrencyValue();
    if (obj.NormalInstallFeeCurrencyType == C_CURRENCY_LOCAL) {
        obj.InstallationFee = obj.InstallationFee;
        obj.InstallationFeeUsd = null;
    }
    else {
        obj.InstallationFeeUsd = obj.InstallationFee;
        obj.InstallationFee = null;
    }

    obj.BillingInstallFeeCurrencyType = $("#BillingInstallFee").NumericCurrencyValue();
    if (obj.BillingInstallFeeCurrencyType == C_CURRENCY_LOCAL) {
        obj.BillingInstallFee = obj.BillingInstallFee;
        obj.BillingInstallFeeUsd = null;
    }
    else {
        obj.BillingInstallFeeUsd = obj.BillingInstallFee;
        obj.BillingInstallFee = null;
    }

    call_ajax_method_json("/Installation/ISS030_ValidateBeforeRegister", obj, function (result, controls) {
        command_control.CommandControlMode(true);
        if (controls != undefined || result != true) {
            //            if ($("#InstallFeeBillingType").prop("disabled") == false && ($("#BillingInstallFee").val() == "" || $("#BillingInstallFee").val() == "0")) {
            //                //doAlert("Common", "MSG0007", "Billing install fee");
            //                //VaridateCtrl(["BillingInstallFee"], ["BillingInstallFee"]);
            //                controls.push("BillingInstallFee");
            //            }
        }
        else {
//            if ($("#InstallFeeBillingType").prop("disabled") == false && ($("#BillingInstallFee").val() == "" || $("#BillingInstallFee").val() == "0")) {
//                doAlert("Common", "MSG0007", "Billing install fee");
//                VaridateCtrl(["BillingInstallFee"], ["BillingInstallFee"]);
//                //controls.push("BillingInstallFee");
//                return false;
//            }
        }
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["BillingInstallFee", "InstallationType", "CustomerStaffBelonging", "CustomerStaffName", "CustomerStaffPhoneNo", "NewConnectionPhoneNo", "NewPhoneLineOpenDate", "ProposeInstallStartDate", "NewPhoneLineOwnerTypeCode", "CauseReason", "ChangeReasonCode", "InstallationBy", "SlipIssueOfficeCode", "NormalInstallFee", "InstallFeeBillingType", "PlanCode"], controls); //Add (InstallFeeBillingType) by Jutarat A. on 11042013
            /* --------------------- */
            for (var j = 0; j < controls.length; j++) {
                var checkInstrumentErr = controls[i];
                //if (checkInstrumentErr > -1) {
                var rowError = controls[j].replace("InstrumentRow", "");

                for (var i = 0; i < ISS030_GridInstrumentInfo.getRowsNum(); i++) {
                    var rowId = ISS030_GridInstrumentInfo.getRowId(i);
                    //var InstrumentCode = ISS030_GridInstrumentInfo.getColIndexById("InstrumentCode");
                    var InstrumentCode = ISS030_GridInstrumentInfo.cells2(i, ISS030_GridInstrumentInfo.getColIndexById("InstrumentCode")).getValue();
                    if (InstrumentCode == rowError) {
                        ISS030_GridInstrumentInfo.selectRow(ISS030_GridInstrumentInfo.getRowIndex(rowId));
                    }

                }
                //}
            }
            return;
        }
        else if (result == true) {
            if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL) {

                if ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW
                || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW
                || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_ALL
                || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL
                || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL)
                {
                    CalculateNormalContractFee();
                    $("#NewNormalContractFee").val($("#CalculatedNormalContractFee").val());
                }

                //$("#NormalContractFee").attr("readonly", false);                
            }
            //$("#NormalContractFee").attr("readonly", true);
            validateWarningData();
//            $("#NewNormalContractFee").val($("#CalculatedNormalContractFee").val());
//            if ($("#NewNormalContractFee").val() != "" || $("#NewNormalContractFee").val() != undefined)
//                $("#NormalContractFee").val(moneyConvert($("#NewNormalContractFee").val()));
        }


    });
    //disabledInputControls();

}

// AKAT K. MODIFY ####################################################################################################################################
function validateWarningData() {
    var obj = {
        temp: ""
    };
    call_ajax_method_json("/Installation/ISS030_GetCheckCP12", obj, function (resultCP12, controls) {

        //if ($("#InstallFeeBillingType").prop("disabled") == false && $("#ChangeType").val() == C_RENTAL_CHANGE_TYPE_PLAN_CHANGE) {
        if (resultCP12 &&
                ($("#InstallationType").val() != C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                && $("#InstallationType").val() != C_RENTAL_INSTALL_TYPE_CHANGE_WIRING
                && $("#InstallationType").val() != C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                && $("#InstallationType").val() != C_RENTAL_INSTALL_TYPE_MOVE
                )) {
            showYesNoDialog5006();
        } else if ($("#BlnHaveNewRow").val() == strTrue && $("#BlnTypeOneTimeOrTemp").val() == strTrue) {
            showYesNoDialog5007();
        } else if ($("#m_blnbFirstTimeRegister").val() == strFalse && $("#SlipStatus").val() == C_SLIP_STATUS_NOT_STOCK_OUT) {
            showYesNoDialog5067();
        } else if ($("#OldNormalContractFee").val() != $("#CalculatedNormalContractFee").val() &&
                     $("#ChangeCustomerReason").prop("checked") == true &&
                         ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW
                            || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW
                            || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL)) {
            showYesNoDialog5068();
        }
        else if (bLineUpStopSale) {
            showYesNoDialog5116();
        } 
        else {
            setConfirmState();
        }

    });
}

function showYesNoDialog5006() {
    var obj = {
        module: "Installation",
        code: "MSG5006"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
			function () {
			    if ($("#BlnHaveNewRow").val() == strTrue && $("#BlnTypeOneTimeOrTemp").val() == strTrue) {
			        showYesNoDialog5007();
			    } else if ($("#m_blnbFirstTimeRegister").val() == strFalse && $("#SlipStatus").val() == C_SLIP_STATUS_NOT_STOCK_OUT) {
			        showYesNoDialog5067();
			    } else if ($("#OldNormalContractFee").val() != $("#NewNormalContractFee").val() && 
                             $("#ChangeCustomerReason").prop("checked") == true &&
                                ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW
                                   || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW
                                   || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL)) {
			        showYesNoDialog5068();
			    }
			    else if (bLineUpStopSale) {
			        showYesNoDialog5116();
			    } 
                else {
			        setConfirmState();
			    }
			});
    });
}

function showYesNoDialog5007() {
    var obj = {
        module: "Installation",
        code: "MSG5007"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
			function () {
			    if ($("#m_blnbFirstTimeRegister").val() == strFalse && $("#SlipStatus").val() == C_SLIP_STATUS_NOT_STOCK_OUT) {
			        showYesNoDialog5067();
			    } else if ($("#OldNormalContractFee").val() != $("#NewNormalContractFee").val() && 
                             $("#ChangeCustomerReason").prop("checked") == true &&
                                ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW
                                   || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW
                                   || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL)) {
			        showYesNoDialog5068();
			    }
			    else if (bLineUpStopSale) {
			        showYesNoDialog5116();
			    } 
                else {
			        setConfirmState();
			    }
			});
    });
}

function showYesNoDialog5067() {
    var obj = {
        module: "Installation",
        code: "MSG5067"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
			function () {
			    if ($("#OldNormalContractFee").val() != $("#NewNormalContractFee").val()  &&
                       $("#ChangeCustomerReason").prop("checked") == true &&
                       ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW
                       || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW
                       || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL)) {
			        showYesNoDialog5068();
			    }
			    else if (bLineUpStopSale) {
			        showYesNoDialog5116();
			    } 
                else {
			        setConfirmState();
			    }
			});
    });
}

function showYesNoDialog5068() {
    var obj = {
        module: "Installation",
        code: "MSG5068"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
			function () {
			    if (bLineUpStopSale) {
			        showYesNoDialog5116();
			    }
			    else {
			        setConfirmState();
			    }
			});
    });
}

function showYesNoDialog5116() {
    var obj = {
        module: "Installation",
        code: "MSG5116"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
			function () {
			    setConfirmState();
			});
    });
}
// AKAT K. MODIFY ####################################################################################################################################


function setConfirmState() {
    
    if ($("#NewNormalContractFee").val() != "" && $("#NewNormalContractFee").val() != undefined)
        $("#NormalContractFee").val(moneyConvert($("#NewNormalContractFee").val()));

    SendGridSlipDetailsToObject();
    InitialCommandButton(2);

    $("#divContractBasicInfo").SetViewMode(true);
    $("#divProjectInfo").SetViewMode(true);
    $("#divInstallationInfo").SetViewMode(true);
    $("#divInstallationMANo").SetViewMode(true);
    $("#divInstallationInstrumentInfo").SetViewMode(true);
    $("#divRegisterFacilityDetail").SetViewMode(true);

    var colInx = ISS030_GridFacility.getColIndexById("FacilityRemove")
    ISS030_GridFacility.setColumnHidden(colInx, true);
    $("#divInsertFacilityDetail").hide();
    disabledGridEmail();

    // - Add by Nontawat L. on 03-Jul-2014
    ISS030.Functions.EnabledGridAttach(false);
    ISS030.Functions.ShowAttachFileSection(false);
    // - End add

    $("#divAddEmail").hide();
    $("#divInstrumentAdd").hide();
}

function setSuccessRegisState() {
    // --------------- Initial SCREEN ------------------
    $("#divContractBasicInfo").SetViewMode(true);
    $("#divProjectInfo").SetViewMode(true);
    $("#divInstallationInfo").SetViewMode(true);
    $("#divInstallationMANo").SetViewMode(true);
    $("#divInstallationInstrumentInfo").SetViewMode(true);
    disabledGridEmail();

    // - Add by Nontawat L. on 03-Jul-2014
    ISS030.Functions.EnabledGridAttach(false);
    ISS030.Functions.ShowAttachFileSection(false);
    // - End

    $("#divInstallationMANo").show();
    $("#divResultRegisSlip").show();
    //    $("#ContractCodeProjectCode").attr("disabled", true);
    //    $("#btnRetrieveInstallation").attr("disabled", true);
    //    $("#btnClearInstallation").attr("disabled", false);

    //disabledInputControls();

    InitialCommandButton(0);
}

function command_confirm_click() {
    command_control.CommandControlMode(false);
    //    $("#divContractBasicInfo").SetViewMode(false);
    //    $("#divProjectInfo").SetViewMode(false);
    //    $("#divInstallationInfo").SetViewMode(false);
    //    $("#divInstallationMANo").SetViewMode(false);
    //    $("#divInstallationInstrumentInfo").SetViewMode(false);
    //    enabledGridEmail();
    //enableInputControls();

    var causeByCode = "";
    if ($("#ChangeCustomerReason").prop("checked") == true) {
        causeByCode = C_INSTALL_CHANGE_REASON_CUS;
    }
    else if ($("#ChangeSecomReason").prop("checked") == true) {
        causeByCode = C_INSTALL_CHANGE_REASON_SECOM;
    }

    var obj = CreateObjectData($("#form1").serialize() 
                                //+ "&Memo=" + $("#Memo").val() + "&ChangeContents=" + $("#ChangeContents").val() 
                                + "&" + $("#form2").serialize() 
                                + "&ExpectedInstrumentArrivalDate="
                                + $("#ExpectedInstrumentArrivalDate").val()
                                + "&StockOutTypeCode=" + $("#StockOutTypeCode").val()
                                + "&BlnHaveNewRow=" + $("#BlnHaveNewRow").val() + "&FacilityList=''"
                                + "&InstallationType=" + $("#InstallationType").val()
                                + "&ChangeCustomerReason=" + $("#ChangeCustomerReason").val()
                                + "&ChangeSecomReason=" + $("#ChangeSecomReason").val()
                                + "&CauseReason=" + $("#CauseReason").val()
                                + "&InstallFeeBillingType=" + $("#InstallFeeBillingType").val()
                                + "&BillingOCC=" + $("#BillingOCC").val()
                                + "&ChangeReasonCode=" + causeByCode
                                + "&MaintenanceNo=" + $("#InstallationMANo").val()
                                + "&InstallationBy=" + $("#InstallationBy").val()
                                + "&NormalInstallFee=" + $("#NormalInstallFee").val()
                                );

    obj.NormalContractFeeCurrencyType = $("#NormalContractFee").NumericCurrencyValue();
    if (obj.NormalContractFeeCurrencyType == C_CURRENCY_LOCAL) {
        obj.NormalContractFee = obj.NormalContractFee;
        obj.NormalContractFeeUsd = null;
    }
    else {
        obj.NormalContractFeeUsd = obj.NormalContractFee;
        obj.NormalContractFee = null;
    }

    obj.NormalInstallFeeCurrencyType = $("#NormalInstallFee").NumericCurrencyValue();
    if (obj.NormalInstallFeeCurrencyType == C_CURRENCY_LOCAL) {
        obj.NormalInstallFee = obj.NormalInstallFee;
        obj.NormalInstallFeeUsd = null;
    }
    else {
        obj.NormalInstallFeeUsd = obj.NormalInstallFee;
        obj.NormalInstallFee = null;
    }

    obj.BillingInstallFeeCurrencyType = $("#BillingInstallFee").NumericCurrencyValue();
    if (obj.BillingInstallFeeCurrencyType == C_CURRENCY_LOCAL) {
        obj.BillingInstallFee = obj.BillingInstallFee;
        obj.BillingInstallFeeUsd = null;
    }
    else {
        obj.BillingInstallFeeUsd = obj.BillingInstallFee;
        obj.BillingInstallFee = null;
    }


    ////////////////// Facility List //////////////////////

    var objArray = new Array();
    if (!CheckFirstRowIsEmpty(ISS030_GridFacility)) {
        for (var i = 0; i < ISS030_GridFacility.getRowsNum(); i++) {
            var rowId = ISS030_GridFacility.getRowId(i);
            var FacilityQTYid = GenerateGridControlID("FacilityQuantityBox", rowId);
            //============ AddInstall QTY ==============
            var AmountFacilityQTY = $("#" + FacilityQTYid).val();
            AmountFacilityQTY = AmountFacilityQTY.replace(/,/g, "")
            //============================================
            var iobj = {
                InstrumentCode: ISS030_GridFacility.cells2(i, 0).getValue()
                    , AddInstalledQty: AmountFacilityQTY
            };
            objArray.push(iobj);
        }
    }
    obj.FacilityList = objArray; 
    ///////////////////////////////////////////////////////
    master_event.LockWindow(true);
    call_ajax_method_json("/Installation/ISS030_RegisterData", obj, function (result, controls) {
        command_control.CommandControlMode(true);
        master_event.LockWindow(false);
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["InstallationType", "CustomerStaffBelonging", "CustomerStaffName", "CustomerStaffPhoneNo"], controls);
            /* --------------------- */

            return;
        }
        else if (result != undefined && result != null) {
            command_control.CommandControlMode(false);
            $("#divContractBasicInfo").SetViewMode(false);
            $("#divProjectInfo").SetViewMode(false);
            $("#divInstallationInfo").SetViewMode(false);
            $("#divInstallationMANo").SetViewMode(false);
            $("#divInstallationInstrumentInfo").SetViewMode(false);
            enabledGridEmail();

            //ISS030.Functions.EnabledGridAttach(true);      // - Add by Nontawat L. on 03-Jul-2014

            /* --- Set View Mode --- */
            /* --------------------- */
            $("#InstallationSlipNo").val(result.SlipNo);
            setSuccessRegisState();

            document.getElementById('divResultRegisSlip').scrollIntoView();
            var obj = {
                module: "Common",
                code: "MSG0046",
                param: ""
            };

            call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                //OpenWarningDialog(result.Message, result.Message, null);
                OpenInformationMessageDialog(result.Code, result.Message);
            });
            /* -------------------------- */
            /////////////////////////// PRINT REPORT AFTER SUCCESS //////////////////////////////
            //************************ COMMENT FOR ADD REPORT
            //window.open("ISS030_QuotationForCancelContractMemorandum");
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
                    btnClearEmailClick();
                    ClearInstrumentInfo();
                    setInitialState();
                    retrieve_installation_click();
                    DeleteAllRow(ISS030_GridFacility);
                    ISS030.Functions.ClearAllAttachFile();      // - Add by Nontawat L. on 03-Jul-2014
                }, function () { command_control.CommandControlMode(true); });
    });
}

function SetRegisterState(cond) {

    InitialCommandButton(1);

    //enableInputControls();

//    $("#ContractCodeProjectCode").attr("disabled", true);
//    $("#btnRetrieveInstallation").attr("disabled", true);
//    $("#btnClearInstallation").attr("disabled", false);
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

    $("#divAddEmail").show();

    //Modify by Jutarat A. on 17042013
//    if ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_NEW
//            || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGE_WIRING
//            || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_NEW
//            || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_ADD
//            || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_CHANGE_WIRING
//        ) {
//        $("#divInstrumentAdd").hide();

//    }
//    else {
//        $("#divInstrumentAdd").show();
//    }
    CheckShowInstrumentAdd();
    //End Modify
}


//function CMS060Response(result) {

//    $("#dlgCTS053").CloseDialog();
//    var emailColinx;
//    var removeColinx;
//    //var mygridCTS053 = $("#gridEmail").LoadDataToGridWithInitial(pageRow, false, false, "/Contract/GetEmailList_CTS053", result, "CTS053_DOEmailData", false);
//    var mygridCTS053 = $("#gridEmail").LoadDataToGridWithInitial(pageRow, false, false, "/Installation/GetEmailList_ISS030", result, "ISS030_DOEmailData", false);
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

function CMS060Response(result) {

    $("#dlgCTS053").CloseDialog();
    var emailColinx;
    var removeColinx;
    //var mygridCTS053 = $("#gridEmail").LoadDataToGridWithInitial(pageRow, false, false, "/Contract/GetEmailList_CTS053", result, "CTS053_DOEmailData", false);
    //var mygridCTS053 = $("#gridEmail").LoadDataToGridWithInitial(pageRow, false, false, "/Installation/GetEmailList_ISS010", result, "ISS010_DOEmailData", false);

    //btnClearEmailClick();
    DeleteAllRow(ISS030_GridEmail);
    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
    call_ajax_method_json("/Installation/ISS030_ClearInstallEmail", obj, function (res, controls) {

        call_ajax_method_json("/Installation/GetEmailList_ISS030", result, function (result, controls) {
            if (result != null && result.length > 0) {
                for (var i = 0; i < result.length; i++) {
                    // Fill to grid
                    var emailList = [result[i].EmailAddress, "", result[i].EmpNo];

                    CheckFirstRowIsEmpty(ISS030_GridEmail, true);
                    AddNewRow(ISS030_GridEmail, emailList);
                }
                BindOnloadGridEmail();
            }
        });
    });
}

function CMS060Object() {
    var objArray = new Array();
    if (CheckFirstRowIsEmpty(ISS030_GridEmail) == false) {
        for (var i = 0; i < ISS030_GridEmail.getRowsNum(); i++) {
            var rowId = ISS030_GridEmail.getRowId(i);
            var selectedRowIndex = ISS030_GridEmail.getRowIndex(rowId);
            var mail = ISS030_GridEmail.cells2(selectedRowIndex, ISS030_GridEmail.getColIndexById('EmailAddress')).getValue();
            var EmpNo = ISS030_GridEmail.cells2(selectedRowIndex, ISS030_GridEmail.getColIndexById('EmpNo')).getValue();
            var iobj = {
                EmailAddress: mail,
                EmpNo: EmpNo
            };
            objArray.push(iobj);
        }
    }

    return { "EmailList": objArray };
}

function btnClearEmailClick() {
    DeleteAllRow(ISS030_GridEmail);
    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
    call_ajax_method_json("/Installation/ISS030_ClearInstallEmail", obj, function (result, controls) {
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
    for (var i = 0; i < ISS030_GridEmail.getRowsNum(); i++) {
        var row_id = ISS030_GridEmail.getRowId(i);
        EnableGridButton(ISS030_GridEmail, "btnRemoveEmail", row_id, "ButtonRemove", false);
    }
    //////////////////////////////////////////////////
}

function enabledGridEmail() {
    //////// ENABLED BUTTON In EMAIL GRID ///////////
    for (var i = 0; i < ISS030_GridEmail.getRowsNum(); i++) {
        var row_id = ISS030_GridEmail.getRowId(i);
        EnableGridButton(ISS030_GridEmail, "btnRemoveEmail", row_id, "ButtonRemove", true);
    }
    //////////////////////////////////////////////////
}

function initalCauseReasonDropdownlist() {
    var causeBy = "";
    if ($("#ChangeCustomerReason").prop("checked") == true) {
        causeBy = C_CUSTOMER_REASON;
    }
    else if ($("#ChangeSecomReason").prop("checked") == true) {
        causeBy = C_SECOM_REASON;
    }

    var obj = { strFieldName: "" };

    
    if (causeBy != "") {
        obj.strFieldName = causeBy;
        call_ajax_method('/Installation/ISS030_GetMiscInstallationtype', obj, function (result, controls) {
            if (result.List.length != 1) {
                regenerate_combo("#CauseReason", result);
                //================== Teerapong 27/08/2012 ================
                if (causeBy == C_CUSTOMER_REASON) {
                    if (FlagCustomerReasonWithoutNewWork != true) {
                        $("#CauseReason").val(C_CUSTOMER_REASON_NEW_WORK);
                        if ($("#m_blnbFirstTimeRegister").val() == strTrue) {
                            $("#CauseReason").attr("disabled", true);
                        }
                        else {
                            $("#CauseReason").attr("disabled", false);
                        }
                    }
                    else {

                        $("#CauseReason").attr("disabled", false);
                        var obj = { strFieldName: "" };
                        call_ajax_method('/Installation/ISS030_GetCustomerReasonWithoutNewWork', obj, function (result, controls) {
                            if (result.List.length != 1) {
                                regenerate_combo("#CauseReason", result);
                            }
                        });
                    }
                }
                //========================================================
            }
        });
    }
}

function initalCauseReasonDropdownlistOnretrieve(resultData) {
    var causeBy = "";
    if ($("#ChangeCustomerReason").prop("checked") == true) {
        causeBy = C_CUSTOMER_REASON;
    }
    else if ($("#ChangeSecomReason").prop("checked") == true) {
        causeBy = C_SECOM_REASON;
    }

    var obj = { strFieldName: "" };

    if (causeBy != "") {
        obj.strFieldName = causeBy;
        call_ajax_method('/Installation/ISS030_GetMiscInstallationtype', obj, function (result, controls) {
            if (result.List.length != 1) {
                regenerate_combo("#CauseReason", result);
                initialInstallationCauseReason(resultData);
            }
        });
    }
}

//========================== DIALOG INSTRUMENT ===============================

function SearchInstrument(obj, func) {
    instrumentObject = obj;
    searchInstReturnFunction = func;

    $("#dlgBox").OpenCMS170Dialog("ISS030");
}

function search_instrument_click() {
    var bProdTypeAlarmTemp = false;
    var bProdTypeSaleTemp = false;
    if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL ) {
        bProdTypeSaleTemp = false;
        bProdTypeAlarmTemp = true;
    }
    else if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_SALE ) {
        bProdTypeSaleTemp = true;
        bProdTypeAlarmTemp = false;
    }
    var obj = {
        bExpTypeHas: true,
        bExpTypeNo: false,
        bProdTypeSale: bProdTypeSaleTemp,
        bProdTypeAlarm: bProdTypeAlarmTemp,
        bInstTypeGen: true,
        bInstTypeMonitoring: false,
        bInstTypeMat: false
    };
    var func = function (result) {
        dtNewInstrument = result;
        $("#InstrumentCode").val(result.InstrumentCode);
        $("#InstrumentName").val(result.InstrumentName);

        $("#LineUpTypeCode").val(result.LineUpTypeCode);
        $("#InstrumentTypeCode").val(result.InstrumentTypeCode);
        $("#RentalFlag").val(result.RentalFlag);
        $("#SaleFlag").val(result.SaleFlag);
        $("#ExpansionTypeCode").val(result.ExpansionTypeCode);
        if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL) {
            $("#InstrumentPrice").val(result.RentalUnitPrice);
        } else if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_SALE) {
            $("#InstrumentPrice").val(result.SaleUnitPrice);
        }
    };

    SearchInstrument(obj, func);
}

function CMS170Object() {
    return instrumentObject;
}

function CMS170Response(result) {
    $("#dlgBox").CloseDialog();

    if (typeof (searchInstReturnFunction) == "function")
        searchInstReturnFunction(result);
}

//========================== END DIALOG INSTRUMENT ============================

function initialGridOnload() {
    // intial grid
    ISS030_GridEmail = $("#gridEmail").InitialGrid(pageRow, false, "/Installation/ISS030_InitialGridEmail");
    ISS030_GridInstrumentInfo = $("#gridInstrumentInfo").InitialGrid(pageRow, false, "/Installation/ISS030_InitialGridInstrumentInfo");
    ISS030_GridFacility = $("#gridFacility").InitialGrid(pageRow, false, "/Installation/ISS030_InitialGridFacility");


    ISS030.Functions.InitialAttachFileGrid();


    SpecialGridControl(ISS030_GridFacility, ["FacilityQuantity"]);
    SpecialGridControl(ISS030_GridInstrumentInfo, ["AddInstalledQTY"]);
    SpecialGridControl(ISS030_GridInstrumentInfo, ["ReturnQTY"]);
    SpecialGridControl(ISS030_GridInstrumentInfo, ["AddRemovedQTY"]);
    SpecialGridControl(ISS030_GridInstrumentInfo, ["MoveQTY"]);
    SpecialGridControl(ISS030_GridInstrumentInfo, ["MAExchangeQTY"]);
    /* ===== binding event when finish load data ===== */
    BindOnLoadedEvent(ISS030_GridEmail, function () {
        var colInx = ISS030_GridEmail.getColIndexById('ButtonRemove');
        for (var i = 0; i < ISS030_GridEmail.getRowsNum(); i++) {
            var rowId = ISS030_GridEmail.getRowId(i);
            GenerateRemoveButton(ISS030_GridEmail, "btnRemoveEmail", rowId, "ButtonRemove", true);

            // binding grid button event 
            BindGridButtonClickEvent("btnRemoveEmail", rowId, BtnRemoveEmailClick);

        }
        ISS030_GridEmail.setSizes();
    });

    /* ===== binding event when finish load data ===== */
    BindOnLoadedEvent(ISS030_GridInstrumentInfo, function () {
            manualInitialGridInstrument();
            
    });

//    BindOnLoadedEvent(ISS030_GridFacility, function () {
//        var colInx = ISS030_GridFacility.getColIndexById('FacilityRemove');
//        for (var i = 0; i < ISS030_GridFacility.getRowsNum(); i++) {
//            var rowId = ISS030_GridFacility.getRowId(i);
//            GenerateRemoveButton(ISS030_GridFacility, "btnRemoveFacility", rowId, "FacilityRemove", true);

//            // binding grid button event 
//            BindGridButtonClickEvent("btnRemoveFacility", rowId, BtnRemoveFacilityClick);

//        }
//        ISS030_GridFacility.setSizes();
//    });   
}


function AddInstrumentInfo() {

    var obj = {
        InstrumentCode : $("#InstrumentCode").val(),
        InstrumentName : $("#InstrumentName").val(),
        AddInstalledQTY: $("#InstrumentQty").val(),
        LineUpTypeCode : $("#LineUpTypeCode").val(),
        InstrumentTypeCode : $("#InstrumentTypeCode").val(),
        RentalFlag : $("#RentalFlag").val(),
        SaleFlag : $("#SaleFlag").val(),
        ExpansionTypeCode : $("#ExpansionTypeCode").val()
    };
    var countInstrument = ISS030_GridInstrumentInfo.getRowsNum() ;
    var blnExistInstrumentCode = false;
    if(countInstrument > 0){
        var tempInstrumentCode = "";
        var InstrumentCodeCol = 0;
        for (var i = 0; i < countInstrument; i++) {            
            InstrumentCodeCol = ISS030_GridInstrumentInfo.getColIndexById("InstrumentCode");
            tempInstrumentCode = ISS030_GridInstrumentInfo.cells2(i, InstrumentCodeCol).getValue();

            if (tempInstrumentCode == $("#InstrumentCode").val())
            {
                blnExistInstrumentCode = true;
            }
        }
    }


    if ($("#InstrumentCode").val().replace(/ /g, "") == "") {
        doAlert("Common", "MSG0081", "Instrument Code");
        VaridateCtrl(["InstrumentCode"], ["InstrumentCode"]);
    }
    else if ($("#InstrumentName").val() == "") {
        doAlert("Common", "MSG0082", $("#InstrumentCode").val());
        VaridateCtrl(["InstrumentCode"], ["InstrumentCode"]);
    }
    else if(blnExistInstrumentCode) {
        doAlert("Common", "MSG0083", $("#InstrumentCode").val());
        VaridateCtrl(["InstrumentCode"], ["InstrumentCode"]);
    }
    else if ($("#InstrumentQty").val() <= 0) {
        doAlert("Common", "MSG0084", "Additional quantity");
        VaridateCtrl(["InstrumentQty"], ["InstrumentQty"]);
    }
    //============================ TRS 06/08/2012 CRC11 ==================================================
    //    else if ($("#LineUpTypeCode").val() == C_LINE_UP_TYPE_LOGICAL_DELETE || $("#LineUpTypeCode").val() == C_LINE_UP_TYPE_STOP_SALE) {
    //        doAlert("Common", "MSG0086", $("#InstrumentCode").val());
    //        VaridateCtrl(["InstrumentCode"], ["InstrumentCode"]);
    //    }
    else if(($("#InstrumentTypeCode").val() != C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE && $("#InstrumentTypeCode").val() != C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE && $("#InstrumentTypeCode").val() != C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE)
        &&
        ($("#LineUpTypeCode").val() == C_LINE_UP_TYPE_LOGICAL_DELETE || $("#LineUpTypeCode").val() == C_LINE_UP_TYPE_STOP_SALE))
        {
        doAlert("Common", "MSG0086", $("#InstrumentCode").val());
        VaridateCtrl(["InstrumentCode"], ["InstrumentCode"]);
    }
    else if (($("#InstrumentTypeCode").val() == C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE || $("#InstrumentTypeCode").val() == C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE || $("#InstrumentTypeCode").val() == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE)
        &&
        ($("#LineUpTypeCode").val() == C_LINE_UP_TYPE_LOGICAL_DELETE))
    {
        doAlert("Common", "MSG0086", $("#InstrumentCode").val());
        VaridateCtrl(["InstrumentCode"], ["InstrumentCode"]);
    }
    //==================================================================================================
    else if ($("#InstrumentTypeCode").val() != C_INST_TYPE_GENERAL) {
        doAlert("Common", "MSG0014", $("#InstrumentCode").val());
        VaridateCtrl(["InstrumentCode"], ["InstrumentCode"]);
    }
    else if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL && $("#RentalFlag").val() != strTrue) {
        doAlert("Common", "MSG0085", $("#InstrumentCode").val());
        VaridateCtrl(["InstrumentCode"], ["InstrumentCode"]);
    }
    else if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_SALE && $("#SaleFlag").val() != strTrue) {
        doAlert("Common", "MSG0016", $("#InstrumentCode").val());
        VaridateCtrl(["InstrumentCode"], ["InstrumentCode"]);
    }
    else if (  ($("#InstrumentTypeCode").val() != C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE && $("#InstrumentTypeCode").val() != C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE && $("#InstrumentTypeCode").val() != C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE) 
                 && ($("#ExpansionTypeCode").val() == C_EXPANSION_TYPE_CHILD) ) {
        doAlert("Common", "MSG0015", $("#InstrumentCode").val());
        VaridateCtrl(["InstrumentCode"], ["InstrumentCode"]);
    }
    else if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL && $("#InstrumentPrice").val() == "") {
        doAlert("Installation", "MSG5069", $("#InstrumentCode").val());
    }
    else {
        /////////   Fill to grid
        var AmountAdditionalInstalled = 0;
        var AmountReturn = 0;
        var AmountRemoved = 0;
        var AmountMoved = 0;
        var AmountMaintenanceExchange = 0;
        if ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW) {
            AmountAdditionalInstalled = $("#InstrumentQty").val();
        }
        else if ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_ALL || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_PARTIAL_REMOVE || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_REMOVE_ALL) {
            AmountRemoved = $("#InstrumentQty").val();
        }        
        else if ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_MOVE || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_MOVE) {
            AmountMoved = $("#InstrumentQty").val();
        }
        else if ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE ) {
            AmountMaintenanceExchange = $("#InstrumentQty").val();
        }

        //Modify by Jutarat A. on 28112013
        //var InstrumentList = [$("#InstrumentCode").val(), $("#InstrumentName").val(), "0", "0", AmountAdditionalInstalled + "", AmountReturn + "", AmountRemoved + "", "0", AmountMoved + "", AmountMaintenanceExchange + "", $("#LineUpTypeCode").val(), $("#InstrumentTypeCode").val(), $("#RentalFlag").val(), $("#SaleFlag").val(), $("#ExpansionTypeCode").val(), strNewRow, $("#InstrumentPrice").val(), "0", "0", "0", "0", "0"];
        var InstrumentList = [ConvertBlockHtml($("#InstrumentCode").val()), ConvertBlockHtml($("#InstrumentName").val()), "0", "0", AmountAdditionalInstalled + "", AmountReturn + "", AmountRemoved + "", "0", AmountMoved + "", AmountMaintenanceExchange + "", $("#LineUpTypeCode").val(), $("#InstrumentTypeCode").val(), $("#RentalFlag").val(), $("#SaleFlag").val(), $("#ExpansionTypeCode").val(), strNewRow, $("#InstrumentPrice").val(), "0", "0", "0", "0", "0"];
        //End Modify
        
        CheckFirstRowIsEmpty(ISS030_GridInstrumentInfo, true);
        AddNewRow(ISS030_GridInstrumentInfo, InstrumentList);

        $("#BlnHaveNewRow").val(strTrue);
        if ($("#LineUpTypeCode").val() == C_LINE_UP_TYPE_ONE_TIME || $("#LineUpTypeCode").val() == C_LINE_UP_TYPE_TEMPORARY)
        {
            $("#BlnTypeOneTimeOrTemp").val(strTrue);
        }
        manualInitialGridInstrument();
        ISS030_GridInstrumentInfo.setSizes();

        $("#InstrumentCode").val("");
        $("#InstrumentName").val("");
        $("#InstrumentQty").val("");
    }
}

function disabledGridEmail() {
    //////// DISABLED BUTTON In EMAIL GRID ///////////
    colInx = ISS030_GridEmail.getColIndexById("ButtonRemove")
    ISS030_GridEmail.setColumnHidden(colInx, true);
//    for (var i = 0; i < ISS030_GridEmail.getRowsNum(); i++) {
//        var row_id = ISS030_GridEmail.getRowId(i);
//        EnableGridButton(ISS030_GridEmail, "btnRemoveEmail", row_id, "ButtonRemove", false);
//    }
    //////////////////////////////////////////////////
}

function enabledGridEmail() {

    colInx = ISS030_GridEmail.getColIndexById("ButtonRemove");
    ISS030_GridEmail.setColumnHidden(colInx, false);
    //////// ENABLED BUTTON In EMAIL GRID ///////////
    for (var i = 0; i < ISS030_GridEmail.getRowsNum(); i++) {
        var row_id = ISS030_GridEmail.getRowId(i);
        //EnableGridButton(ISS030_GridEmail, "btnRemoveEmail", row_id, "ButtonRemove", true);

        if (ISS030_GridEmail.rowsCol.length > 1) {
            ISS030_GridEmail.setColspan(row_id, 1, 2);
        }
    }
    //////////////////////////////////////////////////
}

function disabledGridInstrumentInfo() {
    //////// DISABLED BUTTON In Instrument GRID ///////////
    for (var i = 0; i < ISS030_GridInstrumentInfo.getRowsNum(); i++) {
        var row_id = ISS030_GridInstrumentInfo.getRowId(i);
        EnableGridButton(ISS030_GridInstrumentInfo, "btnRemoveInstrumentInfo", row_id, "ButtonInstrumentRemove", false);
        EnableGridButton(ISS030_GridInstrumentInfo, "btnEditInstrumentInfo", row_id, "ButtonInstrumentEdit", false);
        EnableGridButton(ISS030_GridInstrumentInfo, "btnIssueInstrumentInfo", row_id, "ButtonInstrumentIssue", false);
    }
    //////////////////////////////////////////////////
}

function enabledGridInstrumentInfo() {
    //////// ENABLED BUTTON In Instrument GRID ///////////
    for (var i = 0; i < ISS030_GridInstrumentInfo.getRowsNum(); i++) {
        var row_id = ISS030_GridInstrumentInfo.getRowId(i);
        EnableGridButton(ISS030_GridInstrumentInfo, "btnRemoveInstrumentInfo", row_id, "ButtonInstrumentRemove", true);
        EnableGridButton(ISS030_GridInstrumentInfo, "btnEditInstrumentInfo", row_id, "ButtonInstrumentEdit", true);
        EnableGridButton(ISS030_GridInstrumentInfo, "btnIssueInstrumentInfo", row_id, "ButtonInstrumentIssue", true);
    }
    //////////////////////////////////////////////////
}

function ClearInstrumentInfo() {
    DeleteAllRow(ISS030_GridInstrumentInfo);
    var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
    call_ajax_method_json("/Installation/ISS030_ClearInstrumentInfo", obj, function (result, controls) {


    });

}

function SendGridSlipDetailsToObject() {
    bLineUpStopSale = false;
    if (ISS030_GridInstrumentInfo.hdr.rows.length > 0) {
        var objArray = new Array();
        if (CheckFirstRowIsEmpty(ISS030_GridInstrumentInfo) == false) {
            var InstrumentCodeCol = ISS030_GridInstrumentInfo.getColIndexById("InstrumentCode");
            var InstrumentNameCol = ISS030_GridInstrumentInfo.getColIndexById("InstrumentName");
            var ContractInstalledQTYCol = ISS030_GridInstrumentInfo.getColIndexById("ContractInstalledQTY");
            var TotalStockedOutCol = ISS030_GridInstrumentInfo.getColIndexById("TotalStockedOut");
            var AddInstalledQTYCol = ISS030_GridInstrumentInfo.getColIndexById("AddInstalledQTY");
            var ReturnQTYCol = ISS030_GridInstrumentInfo.getColIndexById("ReturnQTY");
            var AddRemovedQTYCol = ISS030_GridInstrumentInfo.getColIndexById("AddRemovedQTY");
            var ContractInstalledAfterChangeCol = ISS030_GridInstrumentInfo.getColIndexById("ContractInstalledAfterChange");
            var MoveQTYCol = ISS030_GridInstrumentInfo.getColIndexById("MoveQTY");
            var MAExchangeQTYCol = ISS030_GridInstrumentInfo.getColIndexById("MAExchangeQTY");
            var LineUpTypeCodeCol = ISS030_GridInstrumentInfo.getColIndexById("LineUpTypeCode");
            var DescriptionCol = ISS030_GridInstrumentInfo.getColIndexById("Description");
            var InstrumentPriceCol = ISS030_GridInstrumentInfo.getColIndexById("InstrumentPrice");
            var InstrumentTypeCodeCol = ISS030_GridInstrumentInfo.getColIndexById("InstrumentTypeCode");
            var CurrentStockOutQtyCol = ISS030_GridInstrumentInfo.getColIndexById("CurrentStockOutQty"); //Add by Jutarat A. on 08072013

            for (var i = 0; i < ISS030_GridInstrumentInfo.getRowsNum(); i++) {
                var rowId = ISS030_GridInstrumentInfo.getRowId(i);
                var AddInstalledQTYid = GenerateGridControlID("AddInstalledQTYBox", rowId);
                var ReturnQTYid = GenerateGridControlID("ReturnQTYBox", rowId);
                var AddRemovedQTYid = GenerateGridControlID("AddRemovedQTYBox", rowId);
                var MoveQTYid = GenerateGridControlID("MoveQTYBox", rowId);
                var MAExchangeQTYid = GenerateGridControlID("MAExchangeQTYBox", rowId);

                var AmountAddInstallQTY = 0;
                var AmountReturnQTY = 0;
                var AmountAddRemovedQTY = 0;
                var AmountMoveQTY = 0;
                var AmountMAExchangeQTY = 0;
                //============ AddInstall QTY ==============
                AmountAddInstallQTY = $("#" + AddInstalledQTYid).val();
                if (AmountAddInstallQTY == undefined) {
                    AmountAddInstallQTY = GetValueFromLinkType(ISS030_GridInstrumentInfo, i, AddInstalledQTYCol);
                }
                AmountAddInstallQTY = AmountAddInstallQTY.replace(/,/g,"")
                //============ Return QTY ==============
                AmountReturnQTY = $("#" + ReturnQTYid).val();
                if (AmountReturnQTY == undefined) {
                    AmountReturnQTY = GetValueFromLinkType(ISS030_GridInstrumentInfo, i, ReturnQTYCol);
                }
                AmountReturnQTY = AmountReturnQTY.replace(/,/g, "")
                //============ AddRemoved QTY ==============
                AmountAddRemovedQTY = $("#" + AddRemovedQTYid).val();
                if (AmountAddRemovedQTY == undefined) {
                    AmountAddRemovedQTY = GetValueFromLinkType(ISS030_GridInstrumentInfo, i, AddRemovedQTYCol);
                }
                AmountAddRemovedQTY = AmountAddRemovedQTY.replace(/,/g, "")
                //============ MOVE QTY ==============
                AmountMoveQTY = $("#" + MoveQTYid).val();
                if (AmountMoveQTY == undefined) {
                    AmountMoveQTY = GetValueFromLinkType(ISS030_GridInstrumentInfo, i, MoveQTYCol);
                }
                AmountMoveQTY = AmountMoveQTY.replace(/,/g, "")
                //============ AddInstall QTY ==============
                AmountMAExchangeQTY = $("#" + MAExchangeQTYid).val();
                if (AmountMAExchangeQTY == undefined) {
                    AmountMAExchangeQTY = GetValueFromLinkType(ISS030_GridInstrumentInfo, i, MAExchangeQTYCol);
                }
                AmountMAExchangeQTY = AmountMAExchangeQTY.replace(/,/g, "")

                if (AmountAddInstallQTY > 0 && ISS030_GridInstrumentInfo.cells2(i, LineUpTypeCodeCol).getValue() == C_LINE_UP_TYPE_STOP_SALE)
                {
                    bLineUpStopSale = true;
                }

                var iobj = {
                    InstrumentCode: ISS030_GridInstrumentInfo.cells2(i, InstrumentCodeCol).getValue()
                , ContractInstalledQty: ISS030_GridInstrumentInfo.cells2(i, ContractInstalledQTYCol).getValue().replace(/,/g, "")
                , TotalStockOutQty: ISS030_GridInstrumentInfo.cells2(i, TotalStockedOutCol).getValue().replace(/,/g, "")
                , AddInstalledQty: AmountAddInstallQTY
                , ReturnQty: AmountReturnQTY
                , AddRemovedQty: AmountAddRemovedQTY
                , MoveQty: AmountMoveQTY
                , MAExchangeQty: AmountMAExchangeQTY
                , LineUpTypeCode: ISS030_GridInstrumentInfo.cells2(i, LineUpTypeCodeCol).getValue()
                , Description: ISS030_GridInstrumentInfo.cells2(i, DescriptionCol).getValue()
                , InstrumentPrice: ISS030_GridInstrumentInfo.cells2(i, InstrumentPriceCol).getValue().replace(/,/g, "")
                , InstrumentTypeCode: ISS030_GridInstrumentInfo.cells2(i, InstrumentTypeCodeCol).getValue()
                , CurrentStockOutQty: ISS030_GridInstrumentInfo.cells2(i, CurrentStockOutQtyCol).getValue() //Add by Jutarat A. on 08072013
                };
                objArray.push(iobj);
            }
        }

        var obj = {
            do_TbtInstallationSlipDetails: objArray
            , ListInstrumentData: objArray
        };
        /* --------------------- */

        /* --- Check and Add event --- */
        /* --------------------------- */
        call_ajax_method_json("/Installation/ISS030_SendGridSlipDetailsData", obj, function (result, controls) {
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

    function initialScreenOnRetrieve(result) {
        initialChangeReason(result); 
        initialSaleman(result);       
        //initialInstallationCauseReason(result);
        initialNormalContractFee(result);
        initialNormalInstallationFee(result);
        initialSECOMNPayment();
        initialApproveNo(result);
        InitiaInstallationBy(result);

        InitialnstallationFeeBilling(result);
        initalCauseReasonDropdownlistOnretrieve(result);
        if (result.ServiceTypeCode == C_SERVICE_TYPE_RENTAL){ 
            if ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_NEW
            || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW
            || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW
            || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_ALL
            || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL
            || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
        ) {
                $("#divRegisterFacilityDetail").show();

            }
            else {
                $("#divRegisterFacilityDetail").hide();
            }
        }
        else if (result.ServiceTypeCode == C_SERVICE_TYPE_SALE) {
            $("#divRegisterFacilityDetail").hide();
        }
        ////////// InitialInstallationSlipOutputTarget //////////////
        if (result.InstallationOfficeOutputTarget != null && result.InstallationOfficeOutputTarget != "")
        {
            $("#SlipIssueOfficeCode").val(result.InstallationOfficeOutputTarget);
        }
        /////////////////////////////////////////////////////////////

        if ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_NEW
            || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGE_WIRING
            || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_NEW
            || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_ADD
            || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_CHANGE_WIRING
        ) {
            $("#divInstrumentAdd").hide();

        }
        else {
            $("#divInstrumentAdd").show();
        }

        $("#divInsertFacilityDetail").show();
        $("#divRegisterFacilityDetail").SetViewMode(false);

    }

    function initialSaleman(result) {
        if (result.ServiceTypeCode == C_SERVICE_TYPE_RENTAL && result.dtRentalContractBasic.ContractStatus == C_CONTRACT_STATUS_BEF_START) {
            $("#SalesmanCode1").val(result.dtRentalContractBasic.SalesmanCode1);
            $("#SalesmanCode2").val(result.dtRentalContractBasic.SalesmanCode2);
            $("#SalesmanEN1").val(result.dtRentalContractBasic.Salesman1);
            $("#SalesmanEN2").val(result.dtRentalContractBasic.Salesman2);
        }
        else if(result.ServiceTypeCode == C_SERVICE_TYPE_SALE)  { 
            $("#SalesmanCode1").val(result.dtSale.SalesmanCode1);
            $("#SalesmanCode2").val(result.dtSale.SalesmanCode2);
            $("#SalesmanEN1").val(result.dtSale.Salesman1);
            $("#SalesmanEN2").val(result.dtSale.Salesman2);
        }
        else
        {
            $("#SalesmanCode1").val("");
            $("#SalesmanCode2").val("");
            $("#SalesmanEN1").val("");
            $("#SalesmanEN2").val("");
        }
    }

    function initialChangeReason(result){
        var blnRentalCondition = false;
        var blnSaleCondition = false;
        if (result.dtRentalContractBasic != null)
            blnRentalCondition = result.dtRentalContractBasic.FirstInstallCompleteFlag != C_FLAG_ON;
        if (result.dtSale != null)
            blnSaleCondition = ((result.dtSale.ChangeType == C_SALE_CHANGE_TYPE_NEW_SALE || result.dtSale.ChangeType == C_SALE_CHANGE_TYPE_ADD_SALE) && result.dtSale.InstallationCompleteFlag != C_FLAG_ON);

        if (blnRentalCondition || blnSaleCondition)
        {
            $("#ChangeCustomerReason").prop("checked",true);
            initalCauseReasonDropdownlist();
            if(result.m_blnbFirstTimeRegister){
                $("#ChangeCustomerReason").attr("disabled", true);
                $("#ChangeSecomReason").attr("disabled", true);  
            }
            else{
                $("#ChangeCustomerReason").attr("disabled", false);
                $("#ChangeSecomReason").attr("disabled", false);  
            }
        }
        else {
            $("#ChangeCustomerReason").prop("checked",true);
            initalCauseReasonDropdownlist();

            $("#ChangeCustomerReason").attr("disabled",false);
            $("#ChangeSecomReason").attr("disabled", false);

        }
    }

    function initialInstallationType(result){

        var obj = { strFieldName: "" };
       //========= Create $("#InstallationTypeCode") for keep data before $("#InstallationType") success regenerated 
       if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL) {

            //In case new installation and first time
           if (result.m_blnbFirstTimeRegister && result.dtRentalContractBasic.FirstInstallCompleteFlag != C_FLAG_ON) {
               $("#InstallationType").val(C_RENTAL_INSTALL_TYPE_NEW);
               $("#InstallationTypeCode").val(C_RENTAL_INSTALL_TYPE_NEW);
               $("#InstallationType").attr("disabled", true);
               //$("#divInstallationInstrumentInfo").show();
           }
           //In case new installation and not first time
           else if (!result.m_blnbFirstTimeRegister && result.dtRentalContractBasic.FirstInstallCompleteFlag != C_FLAG_ON) {
               $("#InstallationType").val(C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW);
               $("#InstallationTypeCode").val(C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW);
               $("#InstallationType").attr("disabled", true);
               //$("#divInstallationInstrumentInfo").show();
           }
           //In case not new installation and not first time
           else if (!result.m_blnbFirstTimeRegister && result.dtRentalContractBasic.FirstInstallCompleteFlag == C_FLAG_ON) {
               $("#InstallationType").val(result.do_TbtInstallationSlip.InstallationType);
               $("#InstallationTypeCode").val(result.do_TbtInstallationSlip.InstallationType);
               $("#InstallationType").attr("disabled", true);
               //$("#divInstallationInstrumentInfo").show();
           }
           //In case already register installation request or replace slip
           else if (result.do_TbtInstallationBasic != null && result.do_TbtInstallationBasic.InstallationType != "") {
               $("#InstallationType").val(result.do_TbtInstallationBasic.InstallationType);
               $("#InstallationTypeCode").val(result.do_TbtInstallationBasic.InstallationType);
               $("#InstallationType").attr("disabled", true);
               //$("#divInstallationInstrumentInfo").show();
           }
           else if (result.blnCheckCP12 == true) {
               $("#InstallationType").val(C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW);
               $("#InstallationTypeCode").val(C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW);
               $("#InstallationType").attr("disabled", true);
           }
           else if (result.dtRentalContractBasic != null
                    && (result.dtRentalContractBasic.ContractStatus == C_CONTRACT_STATUS_CANCEL
                    || result.dtRentalContractBasic.ContractStatus == C_CONTRACT_STATUS_END)) {
               $("#InstallationType").val(C_RENTAL_INSTALL_TYPE_REMOVE_ALL);
               $("#InstallationTypeCode").val(C_RENTAL_INSTALL_TYPE_REMOVE_ALL);
               $("#InstallationType").attr("disabled", true);
           }
           else if (result.RentalContactBasicLastChangeType != C_RENTAL_CHANGE_TYPE_CANCEL
                && result.RentalContactBasicLastChangeType != C_RENTAL_CHANGE_TYPE_STOP
                && result.RentalContactBasicLastChangeType != C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP
                && result.RentalContactBasicLastChangeType != C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START
            ) {
               obj.strFieldName = result.InstallType;
               call_ajax_method('/Installation/ISS030_GetRentalInstalltypeExceptRemoveAll', obj, function (cbodata, controls) {
                   if (cbodata.List.length != 1) {
                       regenerate_combo("#InstallationType", cbodata);
                   }
               });
           }
           //Other    
           else {
               $("#InstallationType").val("");
               $("#InstallationTypeCode").val("");
               obj.strFieldName = result.InstallType;
               call_ajax_method('/Installation/ISS030_GetRentalInstalltypeOtherCase', obj, function (result3, controls) {
                   if (result3.List.length != 1) {
                       regenerate_combo("#InstallationType", result3);
                   }
               });
               $("#InstallationType").attr("disabled", false);
               //$("#divInstallationInstrumentInfo").hide();
           }

        }
        else if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_SALE) {

            if (result.do_TbtInstallationBasic != null && result.do_TbtInstallationBasic.InstallationType != "" && result.do_TbtInstallationBasic.InstallationType != null) {
                $("#InstallationType").val(result.do_TbtInstallationBasic.InstallationType);
                $("#InstallationTypeCode").val(result.do_TbtInstallationBasic.InstallationType);
                $("#InstallationType").attr("disabled", true);
            }
            //In case not first time
            else if (!result.m_blnbFirstTimeRegister) {
                $("#InstallationType").val(result.do_TbtInstallationSlip.InstallationType);
                $("#InstallationTypeCode").val(result.do_TbtInstallationSlip.InstallationType);
                $("#InstallationType").attr("disabled", true);
                //$("#divInstallationInstrumentInfo").show();
            }
            //
            else if (result.dtSale.ChangeType == C_SALE_CHANGE_TYPE_NEW_SALE && result.dtSale.InstallationCompleteFlag != C_FLAG_ON) {
                $("#InstallationType").val(C_SALE_INSTALL_TYPE_NEW);
                $("#InstallationTypeCode").val(C_SALE_INSTALL_TYPE_NEW);
                $("#InstallationType").attr("disabled", true);
                //$("#divInstallationInstrumentInfo").show();
            }
            else if (result.dtSale.ChangeType == C_SALE_CHANGE_TYPE_ADD_SALE && result.dtSale.InstallationCompleteFlag != C_FLAG_ON) {
                $("#InstallationType").val(C_SALE_INSTALL_TYPE_ADD);
                $("#InstallationTypeCode").val(C_SALE_INSTALL_TYPE_ADD);
                $("#InstallationType").attr("disabled", true);
                //$("#divInstallationInstrumentInfo").show();
            }
            else {
                $("#InstallationType").val("");
                $("#InstallationTypeCode").val("");
                obj.strFieldName = result.InstallType;
                call_ajax_method('/Installation/ISS030_GetSaleInstalltypeOtherCase', obj, function (result4, controls) {
                    if (result4.List.length != 1) {
                        regenerate_combo("#InstallationType", result4);
                    }
                });
                $("#InstallationType").attr("disabled", false);
                //$("#divInstallationInstrumentInfo").hide();
            }
        }              
        
//        else
//        {
//            $("#InstallationType").attr("disabled", false);
//            $("#divInstallationInstrumentInfo").hide();
//        }
   }

   function initialInstallationType2(InstallationType) {
       
           if (InstallationType != "") {
               $("#InstallationType").val(InstallationType);
               $("#InstallationType").attr("disabled", true);
           }
           else {
               $("#InstallationType").attr("disabled", false);
           }
      
   }

    function initialInstallationCauseReason(result){
        
        //if(result.do_TbtInstallationBasic != null){
        //if(result.do_TbtInstallationBasic.InstallationType == C_RENTAL_INSTALL_TYPE_NEW || result.do_TbtInstallationBasic.InstallationType == C_SALE_INSTALL_TYPE_NEW )
        FlagCustomerReasonWithoutNewWork = false;
            if (result.dtRentalContractBasic != null && result.dtRentalContractBasic.FirstInstallCompleteFlag != C_FLAG_ON || (result.dtSale != null && (result.dtSale.ChangeType == C_SALE_CHANGE_TYPE_NEW_SALE || result.dtSale.ChangeType == C_SALE_CHANGE_TYPE_ADD_SALE) && result.dtSale.InstallationCompleteFlag != C_FLAG_ON))
            {
                //setTimeout('$("#CauseReason").val(C_CUSTOMER_REASON_NEW_WORK);', 2000);
                $("#CauseReason").val(C_CUSTOMER_REASON_NEW_WORK);
                if (result.m_blnbFirstTimeRegister) {
                    $("#CauseReason").attr("disabled", true);
                }
                else {
                    $("#CauseReason").attr("disabled", false);
                }
            }
            else
            {
                FlagCustomerReasonWithoutNewWork = true;
                $("#CauseReason").attr("disabled", false);
                var obj = { strFieldName: "" };
                call_ajax_method('/Installation/ISS030_GetCustomerReasonWithoutNewWork', obj, function (result, controls) {
                    if (result.List.length != 1) {
                        regenerate_combo("#CauseReason", result);
                    }
                });
            }
//        }
//        else {            
//            $("#CauseReason").attr("disabled", false);
//            var obj = { strFieldName: "" };
//            call_ajax_method('/Installation/ISS030_GetCustomerReasonWithoutNewWork', obj, function (result, controls) {
//                if (result.List.length != 1) {
//                    regenerate_combo("#CauseReason", result);                    
//                }
//            });
//        }
    }

    function initialNormalContractFee(result){
        if(result.ServiceTypeCode == C_SERVICE_TYPE_SALE) {
            $("#NormalContractFee").attr("readonly", true);
            $("#NormalContractFee").val("");
        }
        else if (result.ServiceTypeCode == C_SERVICE_TYPE_RENTAL) {
            $("#NormalContractFee").attr("readonly", true);
//            if (result.m_blnbFirstTimeRegister) {
//                $("#NormalContractFee").val(result.dtRentalContractBasic.NormalContractFee);
//                $("#OnRetrieveNormalContractFee").val(result.dtRentalContractBasic.NormalContractFee);

//            }
//            else {
//                if (result.do_TbtInstallationSlip != null) {
//                    $("#NormalContractFee").val(result.do_TbtInstallationSlip.NormalContractFee);
//                    $("#OnRetrieveNormalContractFee").val(result.do_TbtInstallationSlip.NormalContractFee);
//                }
//            }
            $("#NormalContractFee").val(result.doRentalFeeResult.NormalContractFee);
            $("#OnRetrieveNormalContractFee").val(result.doRentalFeeResult.NormalContractFee);
            $("#OnRetrieveNormalContractFeeForCal").val(result.doRentalFeeResult.CalNormalContractFee);
        }
        if ($("#NormalContractFee").NumericValue() != "")
            $("#NormalContractFee").val(moneyConvert($("#NormalContractFee").NumericValue()));
        if ($("#OnRetrieveNormalContractFee").val() != "")
            $("#OnRetrieveNormalContractFee").val(moneyConvert($("#OnRetrieveNormalContractFee").val()));
        if ($("#OnRetrieveNormalContractFeeForCal").val() != "")
            $("#OnRetrieveNormalContractFeeForCal").val(moneyConvert($("#OnRetrieveNormalContractFeeForCal").val()));        
    }

    function initialNormalInstallationFee(result) {
//        if (result.m_blnbFirstTimeRegister) {
//            if (result.ServiceTypeCode == C_SERVICE_TYPE_RENTAL) {
//                $("#NormalInstallFee").val(result.dtRentalContractBasic.NormalInstallationFee);
//            }
//            else if (result.ServiceTypeCode == C_SERVICE_TYPE_SALE) {
//                $("#NormalInstallFee").val(result.dtSale.NormalInstallationFee);
//            }
//            $("#NormalInstallFee").attr("readonly", true);
//        }
//        else {
//            if (result.do_TbtInstallationBasic != null) {
//                $("#NormalInstallFee").val(result.do_TbtInstallationBasic.NormalInstallFee);
//            }
//            $("#NormalInstallFee").attr("readonly", false);
//        }
        if (result.ServiceTypeCode == C_SERVICE_TYPE_RENTAL) {
            if (result.dtRentalContractBasic.FirstInstallCompleteFlag != C_FLAG_ON) {
                $("#NormalInstallFee").val(result.dtRentalContractBasic.NormalInstallationFee);
                $("#NormalInstallFee").attr("readonly", true);
            }
            //====================== Teerapong S. CRC-7 =========================
            //else if (result.dtRentalContractBasic.ContractStatus == C_CONTRACT_STATUS_CANCEL || result.dtRentalContractBasic.ContractStatus == C_CONTRACT_STATUS_END) {
            //    $("#NormalInstallFee").val(result.dtRentalContractBasic.NormalInstallationFee);
            //    $("#NormalInstallFee").attr("readonly", true);
            //}
            //===================================================================
            else {
                $("#NormalInstallFee").val(result.doRentalFeeResult.NormalInstallFee);
                $("#NormalInstallFee").attr("readonly", false);
            }
        }
        else if (result.ServiceTypeCode == C_SERVICE_TYPE_SALE) {
            $("#NormalInstallFee").val(result.dtSale.NormalInstallationFee);
            $("#NormalInstallFee").attr("readonly", false);
        }

        if ($("#NormalInstallFee").val() != null && $("#NormalInstallFee").val() != "") {
            $("#NormalInstallFee").val(moneyConvert($("#NormalInstallFee").NumericValue()));
        }

    }

    function initialSECOMNPayment() {

        $("#SECOMPaymentFee").val("");
        $("#SECOMRevenueFee").val("");

        // 20170217 nakajima modify start
        //if ($("#BillingInstallFee").NumericValue() != "" && $("#NormalInstallFee").NumericValue() != "") {
        //    if ($("#BillingInstallFee").NumericValue() * 1 < $("#NormalInstallFee").NumericValue() * 1) {
                

        //        $("#SECOMPaymentFee").val((($("#NormalInstallFee").NumericValue() * 1) - ($("#BillingInstallFee").NumericValue() * 1)).toFixed(2));
        //        $("#SECOMPaymentFee").val(moneyConvert($("#SECOMPaymentFee").NumericValue()));
        //        $("#SECOMRevenueFee").val("0.00");
        //    }
        //    if ($("#BillingInstallFee").NumericValue() * 1 > $("#NormalInstallFee").NumericValue() * 1) {
        //        $("#SECOMRevenueFee").val((($("#BillingInstallFee").NumericValue() * 1) - ($("#NormalInstallFee").NumericValue() * 1)).toFixed(2));
        //        $("#SECOMRevenueFee").val(moneyConvert($("#SECOMRevenueFee").NumericValue()));
        //        $("#SECOMPaymentFee").val("0.00");
        //    }
        //}
        // 20170217 nakajima modify end
    }

    function InitiaInstallationBy(result) {
        if (result.do_TbtInstallationBasic != null) 
        {
            if (result.do_TbtInstallationBasic.InstallationBy == null) {
                $("#InstallationBy").attr("disabled", false);
            }
            else {
                $("#InstallationBy").val(result.do_TbtInstallationBasic.InstallationBy);
                $("#InstallationBy").attr("disabled", true);
            }
        }
        else {
            $("#InstallationBy").attr("disabled", false);
        }
    }    

    function initialApproveNo(result) {
        if (result.m_blnbFirstTimeRegister) {
            if (result.ServiceTypeCode == C_SERVICE_TYPE_RENTAL) {
                if (result.dtRentalContractBasic.FirstInstallCompleteFlag != C_FLAG_ON) {
                    if (result.do_TbtInstallationBasic != null) {
                        $("#ApproveNo1").val(result.do_TbtInstallationBasic.ApproveNo1);
                        $("#ApproveNo2").val(result.do_TbtInstallationBasic.ApproveNo2);
                    }
                    else {
                        $("#ApproveNo1").val("");
                        $("#ApproveNo2").val("");
                    }
                }else {
                    $("#ApproveNo1").val("");
                    $("#ApproveNo2").val("");
                }
            }
            else if (result.ServiceTypeCode == C_SERVICE_TYPE_SALE) {
                if ((result.dtSale.ChangeType == C_SALE_CHANGE_TYPE_NEW_SALE || result.dtSale.ChangeType == C_SALE_CHANGE_TYPE_ADD_SALE) && result.dtSale.InstallationCompleteFlag != C_FLAG_ON) {
                    if (result.do_TbtInstallationBasic != null) {
                        $("#ApproveNo1").val(result.do_TbtInstallationBasic.ApproveNo1);
                        $("#ApproveNo2").val(result.do_TbtInstallationBasic.ApproveNo2);
                    }
                    else {
                        $("#ApproveNo1").val("");
                        $("#ApproveNo2").val("");
                    }
                } else {
                    $("#ApproveNo1").val("");
                    $("#ApproveNo2").val("");
                }
            }
            $("#ApproveNo1").attr("readonly", false);
            $("#ApproveNo2").attr("readonly", false);
        }
        else {
            if (result.do_TbtInstallationSlip != null) {
                $("#ApproveNo1").val(result.do_TbtInstallationSlip.ApproveNo1);
                $("#ApproveNo2").val(result.do_TbtInstallationSlip.ApproveNo2);
            }
            else {
                $("#ApproveNo1").val("");
                $("#ApproveNo2").val("");
            }
            $("#ApproveNo1").attr("readonly", false);
            $("#ApproveNo2").attr("readonly", false);
        }
    }

    function enableInputControls() {
        //########## ENABLED INPUT CONTROL #################
        $("#PlanCode").attr("readonly", false);
        $("#InstallationType").attr("disabled", false);
        $("#CauseReason").attr("disabled", false);
        $("#NormalInstallFee").attr("readonly", false);
        $("#InstallFeeBillingType").attr("disabled", false);
        $("#BillingInstallFee").attr("readonly", false);
        $("#BillingOCC").attr("disabled", false);
        $("#InstallationBy").attr("disabled", false);
        $("#SlipIssueDate").EnableDatePicker(true)
        $("#SlipIssueOfficeCode").attr("disabled", false);
        $("#ApproveNo1").attr("readonly", false);
        $("#ApproveNo2").attr("readonly", false);
        $("#EmailAddress").attr("readonly", false);
        $("#btnAdd").attr("disabled", false);
        $("#btnClear").attr("disabled", false);
        $("#btnSearchEmail").attr("disabled", false);
        $("#Memo").attr("readonly", false);
        $("#ChangeContents").attr("readonly", false);
        //####################################################
    }

    function disabledInputControls() {
        //########## DISABLED INPUT CONTROL #################   
        $("#PlanCode").attr("readonly", true);
        $("#InstallationType").attr("disabled", true);
        $("#CauseReason").attr("disabled", true);
        $("#NormalInstallFee").attr("readonly", true);
        $("#InstallFeeBillingType").attr("disabled", true);
        $("#BillingInstallFee").attr("readonly", true);
        $("#BillingOCC").attr("disabled", true);
        $("#InstallationBy").attr("disabled", true);
        $("#SlipIssueDate").EnableDatePicker(false)
        $("#SlipIssueOfficeCode").attr("disabled", true);
        $("#ApproveNo1").attr("readonly", true);
        $("#ApproveNo2").attr("readonly", true);
        $("#EmailAddress").attr("readonly", true);
        $("#btnAdd").attr("disabled", true);
        $("#btnClear").attr("disabled", true);
        $("#btnSearchEmail").attr("disabled", true);
        $("#Memo").attr("readonly", true);
        $("#ChangeContents").attr("readonly", true);
        //####################################################
    }

    function GetInstrumentInfoData() {
        $("#InstrumentName").val("");
        $("#LineUpTypeCode").val("");
        $("#InstrumentTypeCode").val("");
        $("#RentalFlag").val("");
        $("#SaleFlag").val("");
        $("#ExpansionTypeCode").val("");
        $("#InstrumentPrice").val("");
        var code = $(this).val();
        code = $.trim(code);
        $(this).val(code);
        if ($.trim($(this).val()) != "") {
            /* --- Set Parameter --- */
            var obj = {
                InstrumentCode: $(this).val()
            };

            call_ajax_method_json("/Installation/ISS030_GetInstrumentDetailInfo", obj, function (result) {
                if (result != undefined) {
                    dtNewInstrument = result;
                    $("#InstrumentCode").val(dtNewInstrument.InstrumentCode);
                    $("#InstrumentName").val(dtNewInstrument.InstrumentName);
                    $("#LineUpTypeCode").val(result.LineUpTypeCode);
                    $("#InstrumentTypeCode").val(result.InstrumentTypeCode);
                    $("#RentalFlag").val(result.RentalFlag);
                    $("#SaleFlag").val(result.SaleFlag);
                    $("#ExpansionTypeCode").val(result.ExpansionTypeCode);
                    if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL) {
                        $("#InstrumentPrice").val(result.RentalUnitPrice);
                    } else if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_SALE) {
                        $("#InstrumentPrice").val(result.SaleUnitPrice);
                    }
                }
                else {
                    dtNewInstrument = null;
                }
            });
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

    function manualInitialGridInstrument() {

        $("#BlnHaveNewRow").val(strFalse);
        $("#BlnTypeOneTimeOrTemp").val(strFalse);

        //Move by Jutarat A. on 11042013
        var blnEnableAdditionalInstalled = false;
        var blnEnableReturn = false;
        var blnEnableRemoved = false;
        var blnEnableMoved = false;
        var blnEnableMaintenanceExchange = false;
        //===================== TRS 01/06/2012 InitialInstrumentDetail ===============================
        //                if ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_NEW || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_NEW || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_ADD) {
        //                    blnEnableAdditionalInstalled = false;
        //                    blnEnableReturn = false;
        //                    blnEnableRemoved = false;
        //                    blnEnableMoved = false;
        //                    blnEnableMaintenanceExchange = false;
        //                }
        //                else if ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW) {
        //                    blnEnableAdditionalInstalled = true;
        //                    blnEnableReturn = true;
        //                    blnEnableRemoved = false;
        //                    blnEnableMoved = false;
        //                    blnEnableMaintenanceExchange = false;
        //                }
        //                else if ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW) {
        //                    blnEnableAdditionalInstalled = true;
        //                    blnEnableReturn = true;
        //                    blnEnableRemoved = true;
        //                    blnEnableMoved = false;
        //                    blnEnableMaintenanceExchange = false;
        //                }
        //                else if ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE) {
        //                    blnEnableAdditionalInstalled = false;
        //                    blnEnableReturn = false;
        //                    blnEnableRemoved = false;
        //                    blnEnableMoved = false;
        //                    blnEnableMaintenanceExchange = true;
        //                }
        //                else if ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_MOVE || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_MOVE) {
        //                    blnEnableAdditionalInstalled = false;
        //                    blnEnableReturn = false;
        //                    blnEnableRemoved = false;
        //                    blnEnableMoved = true;
        //                    blnEnableMaintenanceExchange = false;
        //                }
        //                else if ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_ALL || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_PARTIAL_REMOVE || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_REMOVE_ALL) {
        //                    blnEnableAdditionalInstalled = false;
        //                    blnEnableReturn = false;
        //                    blnEnableRemoved = true;
        //                    blnEnableMoved = false;
        //                    blnEnableMaintenanceExchange = false;
        //                }
        var strInstallationType = $("#InstallationType").val();
        if (strInstallationType == ""
                    || strInstallationType == C_RENTAL_INSTALL_TYPE_CHANGE_WIRING
                    || strInstallationType == C_SALE_INSTALL_TYPE_CHANGE_WIRING) {
            $("#divInstallationInstrumentInfo").hide();
        }
        else if (strInstallationType == C_RENTAL_INSTALL_TYPE_NEW
                        || strInstallationType == C_SALE_INSTALL_TYPE_NEW
                        || strInstallationType == C_SALE_INSTALL_TYPE_ADD) {
            $("#divInstallationInstrumentInfo").show();
            $("#divInstrumentAdd").hide();
            $("#divArrivalInformation").show();
            blnEnableAdditionalInstalled = false;
            blnEnableReturn = false;
            blnEnableRemoved = false;
            blnEnableMoved = false;
            blnEnableMaintenanceExchange = false;
        }
        else if (strInstallationType == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW) {
            $("#divInstallationInstrumentInfo").show();
            $("#divInstrumentAdd").show();
            $("#divArrivalInformation").show();
            blnEnableAdditionalInstalled = true;
            blnEnableReturn = true;
            blnEnableRemoved = false;
            blnEnableMoved = false;
            blnEnableMaintenanceExchange = false;
        }
        else if (strInstallationType == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW) {
            $("#divInstallationInstrumentInfo").show();
            $("#divInstrumentAdd").show();
            $("#divArrivalInformation").show();
            blnEnableAdditionalInstalled = true;
            blnEnableReturn = true;
            blnEnableRemoved = true;
            blnEnableMoved = false;
            blnEnableMaintenanceExchange = false;
        }
        else if (strInstallationType == C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                        || strInstallationType == C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                        || strInstallationType == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE) {
            $("#divInstallationInstrumentInfo").show();
            $("#divInstrumentAdd").hide();
            $("#divArrivalInformation").show();
            blnEnableAdditionalInstalled = false;
            blnEnableReturn = false;
            blnEnableRemoved = false;
            blnEnableMoved = false;
            blnEnableMaintenanceExchange = true;
        }
        else if (strInstallationType == C_RENTAL_INSTALL_TYPE_MOVE
                        || strInstallationType == C_SALE_INSTALL_TYPE_MOVE) {
            $("#divInstallationInstrumentInfo").show();
            $("#divInstrumentAdd").hide();
            $("#divArrivalInformation").hide();
            blnEnableAdditionalInstalled = false;
            blnEnableReturn = false;
            blnEnableRemoved = false;
            blnEnableMoved = true;
            blnEnableMaintenanceExchange = false;
        }
        //                else if (strInstallationType == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL 
        //                        || strInstallationType == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL
        //                        || strInstallationType == C_RENTAL_INSTALL_TYPE_REMOVE_ALL
        //                        || strInstallationType == C_SALE_INSTALL_TYPE_PARTIAL_REMOVE
        //                        || strInstallationType == C_SALE_INSTALL_TYPE_REMOVE_ALL ){
        //                    $("#divInstallationInstrumentInfo").show();
        //                    $("#divInstrumentAdd").hide();
        //                    $("#divArrivalInformation").hide();
        //                    blnEnableAdditionalInstalled = false;
        //                    blnEnableReturn = false;
        //                    blnEnableRemoved = true;
        //                    blnEnableMoved = false;
        //                    blnEnableMaintenanceExchange = false;                   
        //                }     
        else if (strInstallationType == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL
                        || strInstallationType == C_SALE_INSTALL_TYPE_PARTIAL_REMOVE
                        ) {
            $("#divInstallationInstrumentInfo").show();
            $("#divInstrumentAdd").show();
            $("#divArrivalInformation").hide();
            blnEnableAdditionalInstalled = false;
            blnEnableReturn = false;
            blnEnableRemoved = true;
            blnEnableMoved = false;
            blnEnableMaintenanceExchange = false;
        }
        else if (strInstallationType == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
                        || strInstallationType == C_RENTAL_INSTALL_TYPE_REMOVE_ALL
                        || strInstallationType == C_SALE_INSTALL_TYPE_REMOVE_ALL) {
            $("#divInstallationInstrumentInfo").show();
            $("#divInstrumentAdd").hide();
            $("#divArrivalInformation").hide();
            blnEnableAdditionalInstalled = false;
            blnEnableReturn = false;
            blnEnableRemoved = false;
            blnEnableMoved = false;
            blnEnableMaintenanceExchange = false;
        }
        //========================================================================================
        //End Move

        if (!CheckFirstRowIsEmpty(ISS030_GridInstrumentInfo)) {

            for (var i = 0; i < ISS030_GridInstrumentInfo.getRowsNum(); i++) {
                var rowId = ISS030_GridInstrumentInfo.getRowId(i);
                //            GenerateNumericBox("AddInstalledQTY", rowId, "0", true);
                //            GenerateNumericBox("ReturnQTY", rowId, "0", true);
                //            GenerateNumericBox("AddRemovedQTY", rowId, "0", true);
                //            GenerateNumericBox("MoveQTY", rowId, "0", true);
                //            GenerateNumericBox("MAExchangeQTY", rowId, "0", true);

                //var val = ISS030_GridInstrumentInfo.cells2(i, 4).getValue();

                var AddInstalledQTYid = GenerateGridControlID("AddInstalledQTYBox", rowId);
                var ReturnQTYid = GenerateGridControlID("ReturnQTYBox", rowId);
                var AddRemovedQTYid = GenerateGridControlID("AddRemovedQTYBox", rowId);
                var MoveQTYid = GenerateGridControlID("MoveQTYBox", rowId);
                var MAExchangeQTYid = GenerateGridControlID("MAExchangeQTYBox", rowId);

                var amountContractInstallCurrent = 0;
                var amountAdditionalInstalled = 0;
                var amountRemoved = 0;
                var amountReturn = 0;
                var amountMove = 0;
                var amountMAExch = 0;

                //============ AddInstall QTY ==============
                if (ISS030_GridInstrumentInfo.hdr.rows.length > 0) {
                    var val = $("#" + AddInstalledQTYid).val();
                    if (val == undefined) {
                        //val = ISS030_GridInstrumentInfo.cells2(i, 4).getValue()
                        val = GetValueFromLinkType(ISS030_GridInstrumentInfo, i, 4);
                    }
                    amountAdditionalInstalled = val.replace(/,/g, "");
                }
                GenerateNumericBox2(ISS030_GridInstrumentInfo, "AddInstalledQTYBox", rowId, "AddInstalledQTY", amountAdditionalInstalled, 10, 0, 0, 9999999999, true, blnEnableAdditionalInstalled);
                //$("#" + AddInstalledQTYid).BindNumericBox(14, 0, 0, 9999999999);
                $("#" + AddInstalledQTYid).css('width', '50px');
                $("#" + AddInstalledQTYid).attr("maxlength", 4);
                $("#" + AddInstalledQTYid).blur(function () {
                    //manualInitialGridInstrument();
                    CalculateGridInstrument();
                });
                //============================================
                //============== Return QTY ==================
                if (ISS030_GridInstrumentInfo.hdr.rows.length > 0) {
                    val = $("#" + ReturnQTYid).val();
                    if (val == undefined) {
                        val = GetValueFromLinkType(ISS030_GridInstrumentInfo, i, 5);
                    }
                }
                amountReturn = val.replace(/,/g, "");
                GenerateNumericBox2(ISS030_GridInstrumentInfo, "ReturnQTYBox", rowId, "ReturnQTY", amountReturn, 10, 0, 0, 9999999999, true, blnEnableReturn);
                //$("#" + ReturnQTYid).BindNumericBox(14, 0, 0, 9999999999);
                $("#" + ReturnQTYid).css('width', '50px');
                $("#" + ReturnQTYid).attr("maxlength", 4);
                $("#" + ReturnQTYid).blur(function () {
                    //manualInitialGridInstrument();
                    CalculateGridInstrument();
                });
                //=============================================
                //=============== Removed QTY =================
                if (ISS030_GridInstrumentInfo.hdr.rows.length > 0) {
                    val = $("#" + AddRemovedQTYid).val();
                    if (val == undefined) {
                        val = GetValueFromLinkType(ISS030_GridInstrumentInfo, i, 6);
                    }
                }
                amountRemoved = val.replace(/,/g, "");
                GenerateNumericBox2(ISS030_GridInstrumentInfo, "AddRemovedQTYBox", rowId, "AddRemovedQTY", amountRemoved, 10, 0, 0, 9999999999, true, blnEnableRemoved);
                //$("#" + AddRemovedQTYid).BindNumericBox(14, 0, 0, 9999999999);
                $("#" + AddRemovedQTYid).css('width', '50px');
                $("#" + AddRemovedQTYid).attr("maxlength", 4);
                $("#" + AddRemovedQTYid).blur(function () {
                    //manualInitialGridInstrument();
                    CalculateGridInstrument();
                });
                //==============================================
                //================ Move QTY ====================
                if (ISS030_GridInstrumentInfo.hdr.rows.length > 0) {
                    val = $("#" + MoveQTYid).val();
                    if (val == undefined) {
                        val = GetValueFromLinkType(ISS030_GridInstrumentInfo, i, 8);
                    }
                    amountMove = val.replace(/,/g, "");
                }
                GenerateNumericBox2(ISS030_GridInstrumentInfo, "MoveQTYBox", rowId, "MoveQTY", amountMove, 10, 0, 0, 9999999999, true, blnEnableMoved);
                //$("#" + MoveQTYid).BindNumericBox(14, 0, 0, 9999999999);
                $("#" + MoveQTYid).css('width', '50px');
                $("#" + MoveQTYid).attr("maxlength", 4);
                //===============================================
                //================= MA Exch QTY =================
                if (ISS030_GridInstrumentInfo.hdr.rows.length > 0) {
                    val = $("#" + MAExchangeQTYid).val();
                    if (val == undefined && ISS030_GridInstrumentInfo.hdr.rows.length > 0) {
                        val = GetValueFromLinkType(ISS030_GridInstrumentInfo, i, 9);
                    }
                    amountMAExch = val.replace(/,/g, "");
                }

                GenerateNumericBox2(ISS030_GridInstrumentInfo, "MAExchangeQTYBox", rowId, "MAExchangeQTY", amountMAExch, 10, 0, 0, 9999999999, true, blnEnableMaintenanceExchange);
                //$("#" + MAExchangeQTYid).BindNumericBox(14, 0, 0, 9999999999);
                $("#" + MAExchangeQTYid).css('width', '50px');
                $("#" + MAExchangeQTYid).attr("maxlength", 4);
                //================================================
                //================ TotalStockOutQty ==============
                var TotalStockedOutCol = ISS030_GridInstrumentInfo.getColIndexById("TotalStockedOut");

                var amountTotalStockedOut = ISS030_GridInstrumentInfo.cells2(i, TotalStockedOutCol).getValue(); 
                amountTotalStockedOut = amountTotalStockedOut.replace(/,/g, "");              
                //================================================

                amountContractInstallCurrent = ISS030_GridInstrumentInfo.cells2(i, 2).getValue().replace(/,/g, "");
                if (amountContractInstallCurrent == "") { amountContractInstallCurrent = 0; }
                if (amountAdditionalInstalled == "") { amountAdditionalInstalled = 0; }
                if (amountRemoved == "") { amountRemoved = 0; }
                if (amountReturn == "") { amountReturn = 0; }
                if (amountTotalStockedOut == "") { amountTotalStockedOut = 0; }

                ISS030_GridInstrumentInfo.cells2(i, 7).setValue(qtyConvert((amountContractInstallCurrent * 1) + (amountAdditionalInstalled * 1) - (amountReturn * 1) - (amountRemoved * 1) + (amountTotalStockedOut * 1)) );

                // binding grid button event 
                //BindGridButtonClickEvent("btnRemoveEmail", rowId, BtnRemoveEmailClick);

                //==================== Check Have Newline and line Up Type ====================
                var DescriptionCol = ISS030_GridInstrumentInfo.getColIndexById("Description");
                var LineUpTypeCodeCol = ISS030_GridInstrumentInfo.getColIndexById("LineUpTypeCode");

                ValueDescriptionCol = ISS030_GridInstrumentInfo.cells2(i, DescriptionCol).getValue();
                ValueLineUpTypeCodeCol = ISS030_GridInstrumentInfo.cells2(i, LineUpTypeCodeCol).getValue();

                if (ValueDescriptionCol == strNewRow) {
                    $("#BlnHaveNewRow").val(strTrue);
                }

                if (ValueLineUpTypeCodeCol == C_LINE_UP_TYPE_ONE_TIME || ValueLineUpTypeCodeCol == C_LINE_UP_TYPE_TEMPORARY) {
                    $("#BlnTypeOneTimeOrTemp").val(strTrue);
                }
                //=============================================================================
                ISS030_GridInstrumentInfo.setSizes();
            }
            
        }
    }

    function manualInitialGridFacility() {
        if (!CheckFirstRowIsEmpty(ISS030_GridFacility))
        {
            for (var i = 0; i < ISS030_GridFacility.getRowsNum(); i++) {
                var rowId = ISS030_GridFacility.getRowId(i);

                var FacilityQtyId = GenerateGridControlID("FacilityQuantityBox", rowId);
                var RemoveButtonId = GenerateGridControlID("btnFacilityRemove", rowId);
                var amountFacilityQty = 0;
                //============ Facility QTY ==============
                if (ISS030_GridFacility.hdr.rows.length > 0) {
                    var val = $("#" + FacilityQtyId).val();
                    if (val == undefined) {
                    
                        val = GetValueFromLinkType(ISS030_GridFacility, i, 2);
                    }
                    amountFacilityQty = val.replace(/,/g, "");
                }
                GenerateNumericBox2(ISS030_GridFacility, "FacilityQuantityBox", rowId, "FacilityQuantity", amountFacilityQty, 10, 0, 0, 9999999, true, true);

                $("#" + FacilityQtyId).css('width', '90px');
                //============================================
                GenerateRemoveButton(ISS030_GridFacility, "btnFacilityRemove", rowId, "FacilityRemove", true);

                // binding grid button event 
                BindGridButtonClickEvent("btnFacilityRemove", rowId, BtnRemoveFacilityClick);
            }
            ISS030_GridFacility.setSizes();
            //SetFitColumnForBackAction(ISS030_GridFacility, "TempColumn");
        }
    }

    function BtnRemoveFacilityClick(row_id) {
        var obj = {
            module: "Common",
            code: "MSG0141"
        };

        call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
            OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    DeleteRow(ISS030_GridFacility, row_id);
                });
        });


        
    }

    function CalculateGridInstrument() {
        
        $("#BlnHaveNewRow").val(strFalse);
        $("#BlnTypeOneTimeOrTemp").val(strFalse);
        if (ISS030_GridInstrumentInfo.hdr.rows.length > 0) {

            for (var i = 0; i < ISS030_GridInstrumentInfo.getRowsNum(); i++) {
//                if (rowIndex == i)
//                {
                    var rowId = ISS030_GridInstrumentInfo.getRowId(i);                

                    var AddInstalledQTYid = GenerateGridControlID("AddInstalledQTYBox", rowId);
                    var ReturnQTYid = GenerateGridControlID("ReturnQTYBox", rowId);
                    var AddRemovedQTYid = GenerateGridControlID("AddRemovedQTYBox", rowId);
                    var MoveQTYid = GenerateGridControlID("MoveQTYBox", rowId);
                    var MAExchangeQTYid = GenerateGridControlID("MAExchangeQTYBox", rowId);

                    var amountContractInstallCurrent = 0;
                    var amountAdditionalInstalled = 0;
                    var amountRemoved = 0;
                    var amountReturn = 0;
                    var amountMove = 0;
                    var amountMAExch = 0;
                
                    //============ AddInstall QTY ==============
                    if (ISS030_GridInstrumentInfo.hdr.rows.length > 0) {
                        var val = $("#" + AddInstalledQTYid).val();
                        if (val == undefined) {
                            //val = ISS030_GridInstrumentInfo.cells2(i, 4).getValue()
                            val = GetValueFromLinkType(ISS030_GridInstrumentInfo, i, 4);
                        }
                        amountAdditionalInstalled = val.replace(/,/g, "");
                    }                   
                    //============================================
                    //============== Return QTY ==================
                    if (ISS030_GridInstrumentInfo.hdr.rows.length > 0) {
                        val = $("#" + ReturnQTYid).val();
                        if (val == undefined) {
                            val = GetValueFromLinkType(ISS030_GridInstrumentInfo, i, 5);
                        }
                    }
                    amountReturn = val.replace(/,/g, "");
                   
                    //=============================================
                    //=============== Removed QTY =================
                    if (ISS030_GridInstrumentInfo.hdr.rows.length > 0) {
                        val = $("#" + AddRemovedQTYid).val();
                        if (val == undefined) {
                            val = GetValueFromLinkType(ISS030_GridInstrumentInfo, i, 6);
                        }
                    }
                    amountRemoved = val.replace(/,/g, "");
                    
                    //==============================================
                    //================ Move QTY ====================
                    if (ISS030_GridInstrumentInfo.hdr.rows.length > 0) {
                        val = $("#" + MoveQTYid).val();
                        if (val == undefined) {
                            val = GetValueFromLinkType(ISS030_GridInstrumentInfo, i, 8);
                        }
                        amountMove = val.replace(/,/g, "");
                    }   

                    //===============================================
                    //================= MA Exch QTY =================
                    if (ISS030_GridInstrumentInfo.hdr.rows.length > 0) {
                        val = $("#" + MAExchangeQTYid).val();
                        if (val == undefined && ISS030_GridInstrumentInfo.hdr.rows.length > 0) {
                            val = GetValueFromLinkType(ISS030_GridInstrumentInfo, i, 9);
                        }
                        amountMAExch = val.replace(/,/g, "");
                    }                   
                    //================================================
                    //================ TotalStockOutQty ==============
                    var TotalStockedOutCol = ISS030_GridInstrumentInfo.getColIndexById("TotalStockedOut");

                    var amountTotalStockedOut = ISS030_GridInstrumentInfo.cells2(i, TotalStockedOutCol).getValue();
                    amountTotalStockedOut = amountTotalStockedOut.replace(/,/g, "");
                    //================================================

                    amountContractInstallCurrent = ISS030_GridInstrumentInfo.cells2(i, 2).getValue().replace(/,/g, "");
                    if (amountContractInstallCurrent == "") { amountContractInstallCurrent = 0; }
                    if (amountAdditionalInstalled == "") { amountAdditionalInstalled = 0; }
                    if (amountRemoved == "") { amountRemoved = 0; }
                    if (amountReturn == "") { amountReturn = 0; }
                    if (amountTotalStockedOut == "") { amountTotalStockedOut = 0; }

                    ISS030_GridInstrumentInfo.cells2(i, 7).setValue(qtyConvert((amountContractInstallCurrent * 1) + (amountAdditionalInstalled * 1) - (amountReturn * 1) - (amountRemoved * 1) + (amountTotalStockedOut * 1)));

                    // binding grid button event 
                    //BindGridButtonClickEvent("btnRemoveEmail", rowId, BtnRemoveEmailClick);                    
                }
            //}

        }
    }

    function CalculateNormalContractFee() {
        var intNormalContractFee = $("#OldNormalContractFee").val();
        //var intNormalContractFee = $("#OnRetrieveNormalContractFeeForCal").val();

        if (intNormalContractFee == "" || intNormalContractFee == undefined) { intNormalContractFee = "0"; }
            intNormalContractFee = intNormalContractFee.replace(/,/g, "");

        var intChangeNormalContractFee = 0;

        if (CheckFirstRowIsEmpty(ISS030_GridInstrumentInfo, false) == false) {  //Add by Jutarat A. on 08032013

            for (var i = 0; i < ISS030_GridInstrumentInfo.getRowsNum(); i++) {
                var rowId = ISS030_GridInstrumentInfo.getRowId(i);

                var InstrumentCodeCol = ISS030_GridInstrumentInfo.getColIndexById("InstrumentCode");
                var InstrumentNameCol = ISS030_GridInstrumentInfo.getColIndexById("InstrumentName");
                var ContractInstalledQTYCol = ISS030_GridInstrumentInfo.getColIndexById("ContractInstalledQTY");
                var TotalStockedOutCol = ISS030_GridInstrumentInfo.getColIndexById("TotalStockedOut");
                var AddInstalledQTYCol = ISS030_GridInstrumentInfo.getColIndexById("AddInstalledQTY");
                var ReturnQTYCol = ISS030_GridInstrumentInfo.getColIndexById("ReturnQTY");
                var AddRemovedQTYCol = ISS030_GridInstrumentInfo.getColIndexById("AddRemovedQTY");
                var ContractInstalledAfterChangeCol = ISS030_GridInstrumentInfo.getColIndexById("ContractInstalledAfterChange");
                var MoveQTYCol = ISS030_GridInstrumentInfo.getColIndexById("MoveQTY");
                var MAExchangeQTYCol = ISS030_GridInstrumentInfo.getColIndexById("MAExchangeQTY");
                var LineUpTypeCodeCol = ISS030_GridInstrumentInfo.getColIndexById("LineUpTypeCode");
                var DescriptionCol = ISS030_GridInstrumentInfo.getColIndexById("Description");
                var InstrumentPriceCol = ISS030_GridInstrumentInfo.getColIndexById("InstrumentPrice");

                var OldAddInstalledQTYCol = ISS030_GridInstrumentInfo.getColIndexById("OldAddInstalledQTY");
                var OldAddRemovedQTYCol = ISS030_GridInstrumentInfo.getColIndexById("OldAddRemovedQTY");
                var OldReturnQTYCol = ISS030_GridInstrumentInfo.getColIndexById("OldReturnQTY");

                intInstrumentPrice = ISS030_GridInstrumentInfo.cells2(i, InstrumentPriceCol).getValue();
                if (intInstrumentPrice == "" || intInstrumentPrice == undefined) { intInstrumentPrice = 0; }
                intInstrumentPrice = intInstrumentPrice.replace(/,/g, "");
                intContractInstalledAfterChange = ISS030_GridInstrumentInfo.cells2(i, ContractInstalledAfterChangeCol).getValue();
                if(intContractInstalledAfterChange == ""){intContractInstalledAfterChange = 0;}
                intContractInstalledAfterChange = intContractInstalledAfterChange.replace(/,/g, "");
                intContractInstalledQTY = ISS030_GridInstrumentInfo.cells2(i, ContractInstalledQTYCol).getValue();
                if(intContractInstalledQTY == ""){intContractInstalledQTY = 0;}
                intContractInstalledQTY = intContractInstalledQTY.replace(/,/g, "");


                intOldAddInstallQTY = ISS030_GridInstrumentInfo.cells2(i, OldAddInstalledQTYCol).getValue();
                if (intOldAddInstallQTY == "" || intOldAddInstallQTY == undefined) { intOldAddInstallQTY = 0; }
                intOldAddInstallQTY = intOldAddInstallQTY.replace(/,/g, "");

                intOldAddRemovedQTY = ISS030_GridInstrumentInfo.cells2(i, OldAddRemovedQTYCol).getValue();
                if (intOldAddRemovedQTY == "" || intOldAddRemovedQTY == undefined) { intOldAddRemovedQTY = 0; }
                intOldAddRemovedQTY = intOldAddRemovedQTY.replace(/,/g, "");

                intOldReturnQTY = ISS030_GridInstrumentInfo.cells2(i, OldReturnQTYCol).getValue();
                if (intOldReturnQTY == "" || intOldReturnQTY == undefined) { intOldReturnQTY = 0; }
                intOldReturnQTY = intOldReturnQTY.replace(/,/g, "");
                //-------------------------------------------------------------------------------------------
                var AddInstalledQTYid = GenerateGridControlID("AddInstalledQTYBox", rowId);
                var intAddQty = $("#" + AddInstalledQTYid).val();
                if (intAddQty == undefined) {
                    intAddQty = GetValueFromLinkType(ISS030_GridInstrumentInfo, i, AddInstalledQTYCol);
                }
                intAddQty = intAddQty.replace(/,/g, "")


                var AddRemovedQTYid = GenerateGridControlID("AddRemovedQTYBox", rowId);
                var intRemovedQty = $("#" + AddRemovedQTYid).val();
                if (intRemovedQty == undefined) {
                    intRemovedQty = GetValueFromLinkType(ISS030_GridInstrumentInfo, i, AddRemovedQTYCol);
                }
                intRemovedQty = intRemovedQty.replace(/,/g, "")

                var ReturnQTYid = GenerateGridControlID("ReturnQTYBox", rowId);
                var intReturnQty = $("#" + ReturnQTYid).val();
                if (intReturnQty == undefined) {
                    intReturnQty = GetValueFromLinkType(ISS030_GridInstrumentInfo, i, ReturnQTYCol);
                }
                intReturnQty = intReturnQty.replace(/,/g, "")
    //            intAddQty = ISS030_GridInstrumentInfo.cells2(i, AddInstalledQTYCol).getValue();
    //            if (intAddQty == "") { intAddQty = 0; }
    //            intAddQty = intAddQty.replace(/,/g, "");
                //-------------------------------------------------------------------------------------------
    //            intRemoveQty = ISS030_GridInstrumentInfo.cells2(i, AddRemovedQTYCol).getValue();
    //            if (intRemoveQty == "") { intRemoveQty = 0; }
    //            intRemoveQty = intRemoveQty.replace(/,/g, "");


    //            if ((intContractInstalledAfterChange - intContractInstalledQTY) > 0) {
    //                intChangeNormalContractFee = intChangeNormalContractFee + (intInstrumentPrice * (intContractInstalledAfterChange - intContractInstalledQTY));
    //            }
    //            if (((intAddQty - intOldAddInstallQTY) - (intRemovedQty - intOldAddRemovedQTY)) > 0) {
                intChangeNormalContractFee = intChangeNormalContractFee + (intInstrumentPrice * ((intAddQty - intOldAddInstallQTY) - (intRemovedQty - intOldAddRemovedQTY) - (intReturnQty - intOldReturnQTY)));
    //            }
    //            else {
    //                intChangeNormalContractFee = intChangeNormalContractFee + (intInstrumentPrice * 0);
    //            }

    //            intChangeNormalContractFee = intChangeNormalContractFee + (intInstrumentPrice * (intAddQty - intRemoveQty));

            }     
                
        }

        var CalCulatedNormalContractFee = 0;
        CalCulatedNormalContractFee = intNormalContractFee * 1 + intChangeNormalContractFee * 1;
        if (CalCulatedNormalContractFee < 0) { CalCulatedNormalContractFee = 0; }
        $("#CalculatedNormalContractFee").val(CalCulatedNormalContractFee);
    }


    function ChangeInstallationType() {
//        if($("#OldInstallationType").val() != "" )
//        {
//            DeleteAllRow(ISS030_GridInstrumentInfo);
//        }
        
        
        if($("#InstallationType").val() != "")
        {
            //Modify by Jutarat A. on 17062013
            if ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE) {
                
                ResetAdditional(doPreviousRentalInstrumentdataExchangeList, doPreviousSaleInstrumentdataExchangeList);
            }
            else {
                ResetAdditional(doPreviousRentalInstrumentdataList, doPreviousSaleInstrumentdataList);
            }
            
            $("#divInstallationInstrumentInfo").show();
            //manualInitialGridInstrument();
            //End Modify
        }
        else
        {
            $("#divInstallationInstrumentInfo").hide();
        }
        $("#OldInstallationType").val($("#InstallationType").val());

        InitialnstallationFeeBilling();

        if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL) {
            if ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_NEW
            || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW
            || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW
            || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_ALL
            || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL
            || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
        ) {
                $("#divRegisterFacilityDetail").show();

            }
            else {
                $("#divRegisterFacilityDetail").hide();
            }
        }
        else if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_SALE) {
            $("#divRegisterFacilityDetail").hide();
        }
    }

    function InitialnstallationFeeBilling(result) {
        var InstallationTypeCode = "";
        if ($("#InstallationType").val() == null || $("#InstallationType").val() == "") {
            InstallationTypeCode = $("#InstallationTypeCode").val();
        }
        else {
            InstallationTypeCode = $("#InstallationType").val();
        }

        if (($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL && (
                InstallationTypeCode == C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
                InstallationTypeCode == C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE ||
                InstallationTypeCode == C_RENTAL_INSTALL_TYPE_MOVE ||
                InstallationTypeCode == C_RENTAL_INSTALL_TYPE_CHANGE_WIRING ||
                InstallationTypeCode == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL ||
				InstallationTypeCode == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
                ))
           || ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_SALE && (
                InstallationTypeCode == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
                InstallationTypeCode == C_SALE_INSTALL_TYPE_MOVE ||
                InstallationTypeCode == C_SALE_INSTALL_TYPE_CHANGE_WIRING ||
                InstallationTypeCode == C_SALE_INSTALL_TYPE_PARTIAL_REMOVE ||
                InstallationTypeCode == C_SALE_INSTALL_TYPE_REMOVE_ALL))) {
            $("#InstallFeeBillingType").attr("disabled", false);
            $("#BillingInstallFee").attr("readonly", false);
            $("#BillingOCC").attr("disabled", false);
            if (result != null && result.do_TbtInstallationSlip != null)
            {
                $("#InstallFeeBillingType").val(result.do_TbtInstallationSlip.InstallFeeBillingType);
                $("#BillingInstallFee").val(result.do_TbtInstallationSlip.BillingInstallFee);
                $("#BillingOCC").val(result.do_TbtInstallationSlip.BillingOCC);

                if ($("#BillingInstallFee").val() != null && $("#BillingInstallFee").val() != "") {
                    $("#BillingInstallFee").val(moneyConvert($("#BillingInstallFee").NumericValue()));
                }
            }
            
        }
        else {
            $("#InstallFeeBillingType").val("");
            $("#InstallFeeBillingType").attr("disabled", true);
            $("#BillingInstallFee").val("");
            $("#BillingInstallFee").attr("readonly", true);
            $("#BillingOCC").val("");
            $("#BillingOCC").attr("disabled", true);
        }
    }

    function OnResetAdditional() {

        /*Modify by Jutarat A. on 17062013
        DeleteAllRow(ISS030_GridInstrumentInfo);
        //        var obj = { strContractCode: $("#ContractCodeProjectCode").val() };
        //        call_ajax_method_json("/Installation/ISS030_ClearInstrumentInfo", obj, function (result, controls) {
        //        });

        if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL) {
            if (doPreviousRentalInstrumentdataList != null && doPreviousRentalInstrumentdataList.length > 0) {
                for (var i = 0; i < doPreviousRentalInstrumentdataList.length; i++) {

                    var tempAdditionalInstalled = 0;
                    if (keepBlnUseContractData == true) {
                        //tempAdditionalInstalled = doPreviousRentalInstrumentdataList[i].CAdditionalInstrumentQty;
                        if (doPreviousRentalInstrumentdataList[i].ITotalStockOutQty < doPreviousRentalInstrumentdataList[i].CAdditionalInstrumentQty) {
                            tempAdditionalInstalled = (doPreviousRentalInstrumentdataList[i].CAdditionalInstrumentQty - doPreviousRentalInstrumentdataList[i].ITotalStockOutQty);
                        }
                        else {
                            tempAdditionalInstalled = 0;
                        }
                    }
                    else if (doPreviousTbtInstallationSlip != null && doPreviousTbtInstallationSlip.SlipStatus == C_SLIP_STATUS_STOCK_OUT) {
                        tempAdditionalInstalled = 0;
                    }
                    else {
                        tempAdditionalInstalled = doPreviousRentalInstrumentdataList[i].IAddInstalledQty;
                    }

                    var tempReturn = 0;
                    if (keepBlnUseContractData == true) {
                        if (doPreviousRentalInstrumentdataList[i].ITotalStockOutQty > doPreviousRentalInstrumentdataList[i].CAdditionalInstrumentQty) {
                            tempReturn = (doPreviousRentalInstrumentdataList[i].ITotalStockOutQty - doPreviousRentalInstrumentdataList[i].CAdditionalInstrumentQty);
                        }
                        else {
                            tempReturn = 0;
                        }
                    }
                    else {
                        tempReturn = doPreviousRentalInstrumentdataList[i].IReturnQty;
                    }

//                    var tempRemoved = 0;
//                    if (keepBlnUseContractData == true) {
//                        tempRemoved = doPreviousRentalInstrumentdataList[i].CRemovalInstrumentQty;
//                    }
//                    else {
//                        tempRemoved = doPreviousRentalInstrumentdataList[i].IAddRemovedQty;
                    //                    }
                    var tempRemoved = 0;
                    if ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
                                || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_ALL
                                || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_REMOVE_ALL
                            ) {
                        tempRemoved = doPreviousRentalInstrumentdataList[i].CInstrumentQty
                    }
                    else {
                        if (keepBlnUseContractData == true) {
                            tempRemoved = doPreviousRentalInstrumentdataList[i].CRemovalInstrumentQty;
                        }
                        else {
                            tempRemoved = doPreviousRentalInstrumentdataList[i].IAddRemovedQty;
                        }
                    }

                    var tempContractInstalledAfter = doPreviousRentalInstrumentdataList[i].CInstrumentQty + doPreviousRentalInstrumentdataList[i].ITotalStockOutQty + tempAdditionalInstalled - tempReturn - tempRemoved;

                    var InstrumentList = [doPreviousRentalInstrumentdataList[i].InstrumentCode
                                                 , doPreviousRentalInstrumentdataList[i].InstrumentName
                                                 , qtyConvert(doPreviousRentalInstrumentdataList[i].CInstrumentQty)
                                                 , qtyConvert(doPreviousRentalInstrumentdataList[i].ITotalStockOutQty)
                                                 , tempAdditionalInstalled + ""
                                                 , tempReturn + ""
                                                 , tempRemoved + ""
                                                 , tempContractInstalledAfter + ""
                                                 , qtyConvert(doPreviousRentalInstrumentdataList[i].IMoveQty)
                                                 , qtyConvert(doPreviousRentalInstrumentdataList[i].IMAExchangeQty), "", "", "", "", ""
                                                 , "", doPreviousRentalInstrumentdataList[i].InstrumentPrice, ""
                                                 , tempAdditionalInstalled + ""
                                                 , tempRemoved + ""
                                                 , tempReturn + ""
                                                 ];
                    CheckFirstRowIsEmpty(ISS030_GridInstrumentInfo, true);
                    AddNewRow(ISS030_GridInstrumentInfo, InstrumentList);
                }
                setTimeout("manualInitialGridInstrument()", 1000);
            }
            else {
                setTimeout('DeleteAllRow(ISS030_GridInstrumentInfo)', 2000);
            }
        }
        else if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_SALE) {
            if (doPreviousSaleInstrumentdataList != null && doPreviousSaleInstrumentdataList.length > 0) {
                for (var i = 0; i < doPreviousSaleInstrumentdataList.length; i++) {

                    var tempRemoved = 0;
                    if ($("#InstallationType").val() == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
                                $("#InstallationType").val() == C_SALE_INSTALL_TYPE_MOVE ||
                                $("#InstallationType").val() == C_SALE_INSTALL_TYPE_PARTIAL_REMOVE) {
                        tempRemoved = doPreviousSaleInstrumentdataList[i].IAddRemovedQty;
                    }
                    else if ($("#InstallationType").val() == C_SALE_INSTALL_TYPE_REMOVE_ALL) {
                        tempRemoved = doPreviousSaleInstrumentdataList[i].CInstrumentQty;
                    }
                    else {
                        tempRemoved = doPreviousSaleInstrumentdataList[i].CRemovalInstrumentQty;
                    }

                  
                    //--------------------------------------------
                    var tempAdditionalInstall = 0;
//                    if ($("#SlipStatus").val() == C_SLIP_STATUS_STOCK_OUT) {
//                        tempAdditionalInstall = 0;
//                    }
//                    else {
//                        tempAdditionalInstall = doPreviousSaleInstrumentdataList[i].CAdditionalInstrumentQty;
//                    }
                     if ($("#InstallationType").val() == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
                                $("#InstallationType").val() == C_SALE_INSTALL_TYPE_MOVE ||
                                $("#InstallationType").val() == C_SALE_INSTALL_TYPE_PARTIAL_REMOVE ||
                                $("#InstallationType").val() == C_SALE_INSTALL_TYPE_REMOVE_ALL) {
                        tempAdditionalInstall = 0;
                    }
                    else if (doPreviousSaleInstrumentdataList[i].CAdditionalInstrumentQty > doPreviousSaleInstrumentdataList[i].ITotalStockOutQty) {
                        tempAdditionalInstall = (doPreviousSaleInstrumentdataList[i].CAdditionalInstrumentQty - doPreviousSaleInstrumentdataList[i].ITotalStockOutQty);
                    } else {
                        tempAdditionalInstall = 0;
                    }

                    var tempReturnQty = 0
//                    if ($("#SlipStatus").val() == C_SLIP_STATUS_STOCK_OUT) {
//                        tempReturnQty = (doPreviousSaleInstrumentdataList[i].ITotalStockOutQty == null ? 0 : doPreviousSaleInstrumentdataList[i].ITotalStockOutQty) - (doPreviousSaleInstrumentdataList[i].CAdditionalInstrumentQty == null ? 0 : doPreviousSaleInstrumentdataList[i].CAdditionalInstrumentQty);
//                    }
//                    else {
//                        tempReturnQty = 0;
                    //                    }
                    if ($("#InstallationType").val() == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
                        $("#InstallationType").val() == C_SALE_INSTALL_TYPE_MOVE ||
                        $("#InstallationType").val() == C_SALE_INSTALL_TYPE_PARTIAL_REMOVE ||
                        $("#InstallationType").val() == C_SALE_INSTALL_TYPE_REMOVE_ALL) {
                        tempReturnQty = 0;
                    }
                    else if (doPreviousSaleInstrumentdataList[i].CAdditionalInstrumentQty < doPreviousSaleInstrumentdataList[i].ITotalStockOutQty) {
                        tempReturnQty = (doPreviousSaleInstrumentdataList[i].ITotalStockOutQty - doPreviousSaleInstrumentdataList[i].CAdditionalInstrumentQty);
                    } else {
                        tempReturnQty = 0;
                    }

                    var tempContractInstalledAfter = doPreviousSaleInstrumentdataList[i].CInstrumentQty + doPreviousSaleInstrumentdataList[i].ITotalStockOutQty + tempAdditionalInstall - tempReturnQty - tempRemoved;

                    var InstrumentList = [doPreviousSaleInstrumentdataList[i].InstrumentCode
                                                 , doPreviousSaleInstrumentdataList[i].InstrumentName
                                                 , qtyConvert(doPreviousSaleInstrumentdataList[i].CInstrumentQty)
                                                 , qtyConvert(doPreviousSaleInstrumentdataList[i].ITotalStockOutQty)
                                                 , tempAdditionalInstall + ""
                                                 , tempReturnQty + ""
                                                 , tempRemoved + ""
                                                 , tempContractInstalledAfter + ""
                                                 , qtyConvert(doPreviousSaleInstrumentdataList[i].IMoveQty)
                                                 , qtyConvert(doPreviousSaleInstrumentdataList[i].IMAExchangeQty), "", "", "", "", ""
                                                 , "", doPreviousSaleInstrumentdataList[i].InstrumentPrice, ""
                                                 , tempAdditionalInstall + ""
                                                 , tempRemoved + ""
                                                 , tempReturnQty + ""
                                                 ];
                    CheckFirstRowIsEmpty(ISS030_GridInstrumentInfo, true);
                    AddNewRow(ISS030_GridInstrumentInfo, InstrumentList);
                }
                setTimeout("manualInitialGridInstrument()", 1000);
            }
            else {
                setTimeout('DeleteAllRow(ISS030_GridInstrumentInfo)', 2000);
            }
        }
        else {
            setTimeout('DeleteAllRow(ISS030_GridInstrumentInfo)', 2000);
        }
        */
        ResetAdditional(doPreviousRentalInstrumentdataList, doPreviousSaleInstrumentdataList);
        //End Modify
    }

    //Add by Jutarat A. on 17062013
    function ResetAdditional(doRentalInstrumentdataList, doSaleInstrumentdataList) {

        DeleteAllRow(ISS030_GridInstrumentInfo);

        if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_RENTAL) {
            if (doRentalInstrumentdataList != null && doRentalInstrumentdataList.length > 0) {
                for (var i = 0; i < doRentalInstrumentdataList.length; i++) {

                    var tempAdditionalInstalled = 0;
                    if (keepBlnUseContractData == true) {

                        if (doRentalInstrumentdataList[i].ITotalStockOutQty < doRentalInstrumentdataList[i].CAdditionalInstrumentQty) {
                            tempAdditionalInstalled = (doRentalInstrumentdataList[i].CAdditionalInstrumentQty - doRentalInstrumentdataList[i].ITotalStockOutQty);
                        }
                        else {
                            tempAdditionalInstalled = 0;
                        }
                    }
                    else if (doPreviousTbtInstallationSlip != null && doPreviousTbtInstallationSlip.SlipStatus == C_SLIP_STATUS_STOCK_OUT) {
                        tempAdditionalInstalled = 0;
                    }
                    else {
                        tempAdditionalInstalled = doRentalInstrumentdataList[i].IAddInstalledQty;
                    }

                    var tempReturn = 0;
                    if (keepBlnUseContractData == true) {
                        if (doRentalInstrumentdataList[i].ITotalStockOutQty > doRentalInstrumentdataList[i].CAdditionalInstrumentQty) {
                            tempReturn = (doRentalInstrumentdataList[i].ITotalStockOutQty - doRentalInstrumentdataList[i].CAdditionalInstrumentQty);
                        }
                        else {
                            tempReturn = 0;
                        }
                    }
                    else {
                        tempReturn = doRentalInstrumentdataList[i].IReturnQty;
                    }

                    var tempRemoved = 0;
                    if ($("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
                                || $("#InstallationType").val() == C_RENTAL_INSTALL_TYPE_REMOVE_ALL
                                || $("#InstallationType").val() == C_SALE_INSTALL_TYPE_REMOVE_ALL
                            ) {
                        tempRemoved = doRentalInstrumentdataList[i].CInstrumentQty
                    }
                    else {
                        if (keepBlnUseContractData == true) {
                            tempRemoved = doRentalInstrumentdataList[i].CRemovalInstrumentQty;
                        }
                        else {
                            tempRemoved = doRentalInstrumentdataList[i].IAddRemovedQty;
                        }
                    }

                    var tempContractInstalledAfter = doRentalInstrumentdataList[i].CInstrumentQty + doRentalInstrumentdataList[i].ITotalStockOutQty + tempAdditionalInstalled - tempReturn - tempRemoved;

                    var InstrumentList = [      //Modify by Jutarat A. on 28112013
                                                //doRentalInstrumentdataList[i].InstrumentCode
                                                // , doRentalInstrumentdataList[i].InstrumentName
                                                ConvertBlockHtml(doRentalInstrumentdataList[i].InstrumentCode)
                                                 , ConvertBlockHtml(doRentalInstrumentdataList[i].InstrumentName)
                                                //End Modify
                                                 
                                                 , qtyConvert(doRentalInstrumentdataList[i].CInstrumentQty)
                                                 , qtyConvert(doRentalInstrumentdataList[i].ITotalStockOutQty)
                                                 , tempAdditionalInstalled + ""
                                                 , tempReturn + ""
                                                 , tempRemoved + ""
                                                 , tempContractInstalledAfter + ""
                                                 , qtyConvert(doRentalInstrumentdataList[i].IMoveQty)
                                                 , qtyConvert(doRentalInstrumentdataList[i].IMAExchangeQty), doRentalInstrumentdataList[i].LineUpTypeCode, doRentalInstrumentdataList[i].InstrumentTypeCode, "", "", ""
                                                 , "", doRentalInstrumentdataList[i].InstrumentPrice, ""
                                                 , doRentalInstrumentdataList[i].ICurrentStockOutQty //Add by Jutarat A. on 08072013
                                                 , tempAdditionalInstalled + ""
                                                 , tempRemoved + ""
                                                 , tempReturn + ""
                                                 ];
                    CheckFirstRowIsEmpty(ISS030_GridInstrumentInfo, true);
                    AddNewRow(ISS030_GridInstrumentInfo, InstrumentList);
                }
                setTimeout("manualInitialGridInstrument()", 1000);
            }
            else {
                setTimeout('DeleteAllRow(ISS030_GridInstrumentInfo)', 2000);
            }
        }
        else if ($("#ServiceTypeCode").val() == C_SERVICE_TYPE_SALE) {
            if (doSaleInstrumentdataList != null && doSaleInstrumentdataList.length > 0) {
                for (var i = 0; i < doSaleInstrumentdataList.length; i++) {

                    var tempRemoved = 0;
                    if ($("#InstallationType").val() == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
                                $("#InstallationType").val() == C_SALE_INSTALL_TYPE_MOVE ||
                                $("#InstallationType").val() == C_SALE_INSTALL_TYPE_PARTIAL_REMOVE) {
                        tempRemoved = doSaleInstrumentdataList[i].IAddRemovedQty;
                    }
                    else if ($("#InstallationType").val() == C_SALE_INSTALL_TYPE_REMOVE_ALL) {
                        tempRemoved = doSaleInstrumentdataList[i].CInstrumentQty;
                    }
                    else {
                        tempRemoved = doSaleInstrumentdataList[i].CRemovalInstrumentQty;
                    }


                    //--------------------------------------------
                    var tempAdditionalInstall = 0;
                    if ($("#InstallationType").val() == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
                                $("#InstallationType").val() == C_SALE_INSTALL_TYPE_MOVE ||
                                $("#InstallationType").val() == C_SALE_INSTALL_TYPE_PARTIAL_REMOVE ||
                                $("#InstallationType").val() == C_SALE_INSTALL_TYPE_REMOVE_ALL) {
                        tempAdditionalInstall = 0;
                    }
                    else if (doSaleInstrumentdataList[i].ITotalStockOutQty < doSaleInstrumentdataList[i].CAdditionalInstrumentQty) {
                        tempAdditionalInstall = (doSaleInstrumentdataList[i].CAdditionalInstrumentQty - doSaleInstrumentdataList[i].ITotalStockOutQty);
                    }
                    else {
                        tempAdditionalInstall = 0;
                    }


                    var tempReturnQty = 0
                    if ($("#InstallationType").val() == C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
                        $("#InstallationType").val() == C_SALE_INSTALL_TYPE_MOVE ||
                        $("#InstallationType").val() == C_SALE_INSTALL_TYPE_PARTIAL_REMOVE ||
                        $("#InstallationType").val() == C_SALE_INSTALL_TYPE_REMOVE_ALL) {
                        tempReturnQty = 0;
                    }
                    else if (doSaleInstrumentdataList[i].ITotalStockOutQty > doSaleInstrumentdataList[i].CAdditionalInstrumentQty) {
                        tempReturnQty = (doSaleInstrumentdataList[i].ITotalStockOutQty - doSaleInstrumentdataList[i].CAdditionalInstrumentQty);
                    }
                    else {
                        tempReturnQty = 0;
                    }


                    var tempContractInstalledAfter = doSaleInstrumentdataList[i].CInstrumentQty + doSaleInstrumentdataList[i].ITotalStockOutQty + tempAdditionalInstall - tempReturnQty - tempRemoved;

                    var InstrumentList = [      //Modify by Jutarat A. on 28112013
                                                //doSaleInstrumentdataList[i].InstrumentCode
                                                // , doSaleInstrumentdataList[i].InstrumentName
                                                ConvertBlockHtml(doSaleInstrumentdataList[i].InstrumentCode)
                                                 , ConvertBlockHtml(doSaleInstrumentdataList[i].InstrumentName)
                                                //End Modify

                                                 , qtyConvert(doSaleInstrumentdataList[i].CInstrumentQty)
                                                 , qtyConvert(doSaleInstrumentdataList[i].ITotalStockOutQty)
                                                 , tempAdditionalInstall + ""
                                                 , tempReturnQty + ""
                                                 , tempRemoved + ""
                                                 , tempContractInstalledAfter + ""
                                                 , qtyConvert(doSaleInstrumentdataList[i].IMoveQty)
                                                 , qtyConvert(doSaleInstrumentdataList[i].IMAExchangeQty), doSaleInstrumentdataList[i].LineUpTypeCode, doSaleInstrumentdataList[i].InstrumentTypeCode, "", "", ""
                                                 , "", doSaleInstrumentdataList[i].InstrumentPrice, ""
                                                 , doSaleInstrumentdataList[i].ICurrentStockOutQty //Add by Jutarat A. on 08072013
                                                 , tempAdditionalInstall + ""
                                                 , tempRemoved + ""
                                                 , tempReturnQty + ""
                                                 ];
                    CheckFirstRowIsEmpty(ISS030_GridInstrumentInfo, true);
                    AddNewRow(ISS030_GridInstrumentInfo, InstrumentList);
                }
                setTimeout("manualInitialGridInstrument()", 1000);
            }
            else {
                setTimeout('DeleteAllRow(ISS030_GridInstrumentInfo)', 2000);
            }
        }
        else {
            setTimeout('DeleteAllRow(ISS030_GridInstrumentInfo)', 2000);
        }

    }
    //End Add
    
    function ClearInstrumentInput() {
        $("#InstrumentCode").val("");
        $("#InstrumentName").val("");
        $("#InstrumentQty").val("");
        VaridateCtrl(["InstrumentCode", "InstrumentQty"], null);
    }

    function ChangeInstallationBy() {
        if ($("#InstallationBy").val() == C_INSTALLATION_BY_OTHER) {
            $("#ChangeCustomerReason").attr("checked", true)
            initalCauseReasonDropdownlist();
            setTimeout('$("#CauseReason").val(C_CUSTOMER_REASON_OTHER)', 500);

            $("#ChangeCustomerReason").attr("disabled", true);
            $("#ChangeSecomReason").attr("disabled", true);  
        }
        else {
            $("#ChangeCustomerReason").attr("disabled", false);
            $("#ChangeSecomReason").attr("disabled", false);

            $("#CauseReason").attr("disabled", false);
        }
    }

    function moneyConvert(value) {
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
                value = sBuf + value.substring(value.indexOf("."), value.indexOf(".") + 3);
            }
            else {
                if (sBuf != "") {
                    value = sBuf + ".00";
                }
                else{
                    value = "0.00";
                }
                
            }
            return value;
        }
    }

    function qtyConvert(value) {
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
                value = sBuf ;
            }
            return value;
        }
    }


    function BindOnloadGridEmail() {
        //============= TRS Add ===================
        var colInx = ISS030_GridEmail.getColIndexById('ButtonRemove');
        for (var i = 0; i < ISS030_GridEmail.getRowsNum(); i++) {
            var rowId = ISS030_GridEmail.getRowId(i);
            GenerateRemoveButton(ISS030_GridEmail, "btnRemoveEmail", rowId, "ButtonRemove", true);

            // binding grid button event 
            BindGridButtonClickEvent("btnRemoveEmail", rowId, BtnRemoveEmailClick);

        }
        ISS030_GridEmail.setSizes();
        //=========================================
    }

    function search_facility_click() {
        var obj = {
            bExpTypeHas: false,
            bExpTypeNo: false,
            bProdTypeSale: false,
            bProdTypeAlarm: false,
            bInstTypeGen: false,
            bInstTypeMonitoring: true,
            bInstTypeMat: false
        };
        var func = function (result) {
           

            //doNewFacility = result;
            $("#FacilityCode").val(result.InstrumentCode);
            $("#FacilityName").val(result.InstrumentName);
            $("#InstrumentTypeCode").val(result.InstrumentTypeCode);
        };

        SearchInstrument(obj, func);
    }

    function FacilityCode_Blur() {
        var obj = { FacilityCode: $("#FacilityCode").val() };
        call_ajax_method_json("/Installation/ISS030_GetFacilityCode", obj, function (result, controls) {
            if (result != null && result.length > 0) {
                $("#FacilityCode").val(result[0].InstrumentCode);
                $("#FacilityName").val(result[0].InstrumentName);
                $("#InstrumentTypeCode").val(result[0].InstrumentTypeCode);
            }
            else {
                //$("#FacilityCode").val("");
                $("#FacilityName").val("");
                $("#InstrumentTypeCode").val("");
            }
        });
    }

    function Clear_Facility_Click() {
        $("#FacilityCode").val("");
        $("#FacilityName").val("");
        $("#FacilityQuantity").val("");
        $("#InstrumentTypeCode").val("");
        VaridateCtrl(["FacilityCode", "FacilityName", "InstrumentTypeCode", "FacilityQuantity"], null);
    }

    function Add_Facility_Click() {
        VaridateCtrl(["FacilityCode", "FacilityName", "InstrumentTypeCode", "FacilityQuantity"], null);
        if ($("#FacilityCode").val() == null || $("#FacilityCode").val() == "")
        {
            doAlert("Common", "MSG0022", lblFacilityCode);
            VaridateCtrl(["FacilityCode"], ["FacilityCode"]);
            return false;
        }
        if ($("#FacilityName").val() == null || $("#FacilityName").val() == "") {
            doAlert("Common", "MSG0023", $("#FacilityCode").val());
            VaridateCtrl(["FacilityCode"], ["FacilityCode"]);
            return false;
        }
        for (var i = 0; i < ISS030_GridFacility.getRowsNum(); i++) {
            var val = GetValueFromLinkType(ISS030_GridFacility, i, 0);
            var FacilityCode = $("#FacilityCode").val();
            if (val.replace(/" "/g, "") == FacilityCode.replace(/" "/g, ""))
            {
                doAlert("Common", "MSG0024", $("#FacilityCode").val());
                VaridateCtrl(["FacilityCode"], ["FacilityCode"]);
                return false;
            }
        }
        if ($("#FacilityQuantity").val() == null || ($("#FacilityQuantity").val() * 1 <= 0)) {
            doAlert("Common", "MSG0025", $("#FacilityQuantity").val());
            VaridateCtrl(["FacilityQuantity"], ["FacilityQuantity"]);
            return false;
        }
        if ($("#InstrumentTypeCode ").val() != C_INST_TYPE_MONITOR ) {
            doAlert("Common", "MSG0026", $("#FacilityCode").val());
            VaridateCtrl(["FacilityCode"], ["FacilityCode"]);
            return false;
        }

        //Modify by Jutarat A. on 28112013
        //var FacilityList = [$("#FacilityCode").val(), $("#FacilityName").val(), qtyConvert($("#FacilityQuantity").val()), ""];
        var FacilityList = [ConvertBlockHtml($("#FacilityCode").val()), ConvertBlockHtml($("#FacilityName").val()), qtyConvert($("#FacilityQuantity").val()), ""];
        //End Modify

        CheckFirstRowIsEmpty(ISS030_GridFacility, true);
        AddNewRow(ISS030_GridFacility, FacilityList);
        var LastRowIndex = ISS030_GridFacility.getRowsNum() - 1;
        setTimeout("GridFacilityGenControlLastRow("+LastRowIndex+")", 600);
        //ISS030_GridFacility.setSizes();
        Clear_Facility_Click();
    }

    function GridFacilityGenControlLastRow(LastRowIndex) {
        var rowId = ISS030_GridFacility.getRowId(LastRowIndex);
        var FacilityQtyId = GenerateGridControlID("FacilityQuantityBox", rowId);
        //============ Facility QTY ==============
        val = GetValueFromLinkType(ISS030_GridFacility, LastRowIndex, 2);
        amountFacilityQty = val.replace(/,/g, "");        
        GenerateNumericBox2(ISS030_GridFacility, "FacilityQuantityBox", rowId, "FacilityQuantity", amountFacilityQty, 10, 0, 0, 9999999, true, true);
        $("#" + FacilityQtyId).css('width', '90px');
        //============================================        
        GenerateRemoveButton(ISS030_GridFacility, "btnRemoveFacility", rowId, "FacilityRemove", true);
        // binding grid button event 
        BindGridButtonClickEvent("btnRemoveFacility", rowId, BtnRemoveFacilityClick);
        ISS030_GridFacility.setSizes();
    }