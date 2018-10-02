// ---------------------------------------------------------------------------------
// Initial
// ---------------------------------------------------------------------------------

var C_ACQUISITION_TYPE_CUST = "1";
var C_ACQUISITION_TYPE_INSIDE_COMPANY = "2";
var C_ACQUISITION_TYPE_SUBCONTRACTOR = "3";

var C_DISTRIBUTED_TYPE_NOT_DISTRIBUTED = "1";
var C_DISTRIBUTED_TYPE_ORIGIN = "2";

var pageRow = 5;

var currContractCode = "";
var currQuotationCode = "";
var currAlphabet = "";
var currMgmtCost = "";
var currChangeTypeCode = "";
var currDistributedOriginCode = "";

var instrumentgrid;
var subcontractgrid1;
var subcontractgrid2;

var instQtyAdded = "QtyAddedTxt";
var instQtyRemoved = "QtyRemovedTxt";
var instQtyTotal = "QtyTotalTxt";

var numBox_Length = 12;
var numBox_Decimal = 2;
var numBox_Min = 0;
var numBox_Max = 999999999999.9999;
var numBox_DefaultMin = false;

var curInstrumentData;
var curInstrumentCode;
var isValidAddInstrument = false;

var viewMode = null; //Add by Jutarat A. on 03042013

var validateControlList = ["SalesmanCode1", "SalesmanCode2", "SalesmanCode3", "SalesmanCode4", "SalesmanCode5", "SalesmanCode6", "SalesmanCode7", "SalesmanCode8", "SalesmanCode9", "SalesmanCode10", "DistributedOriginCode", "CustomerAcceptDate", "WaranteePeriodFrom", "WaranteePeriodTo", "NegotiationStaffCode", "CompleteRegistrantCode", "OnlineContractCode"];

$(document).ready(function () {
    LoadInitialDefaultValue();

    InitialGrid();
    InitialDatePicker();
    InitialScreen();
    InitialNumericBox();
    InitialEvent();
})

function LoadInitialDefaultValue() {
    call_ajax_method_json("/Contract/CTS150_RetrieveInitialDefaultValue", "", function (result, controls) {
        C_ACQUISITION_TYPE_CUST = result.C_ACQUISITION_TYPE_CUST;
        C_ACQUISITION_TYPE_INSIDE_COMPANY = result.C_ACQUISITION_TYPE_INSIDE_COMPANY;
        C_ACQUISITION_TYPE_SUBCONTRACTOR = result.C_ACQUISITION_TYPE_SUBCONTRACTOR;

        C_DISTRIBUTED_TYPE_NOT_DISTRIBUTED = result.C_DISTRIBUTED_TYPE_NOT_DISTRIBUTED;
        C_DISTRIBUTED_TYPE_ORIGIN = result.C_DISTRIBUTED_TYPE_ORIGIN;

        SetContractCode(result.ContractCode);
    });
}

function InitialGrid() {
    subcontractgrid1 = $("#InstallSubCTGrid1").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/Contract/CTS150_InitialSubcontractorGrid");
    subcontractgrid2 = $("#InstallSubCTGrid2").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/Contract/CTS150_InitialSubcontractorGrid");

    instrumentgrid = $("#InstrumentGrid").InitialGrid(5000, false, "/Contract/CTS150_InitialInstrumentDetail");
    SpecialGridControl(instrumentgrid, ["TxtQtyAdded", "TxtQtyRemoved", "QtyTotal"])
    BindOnLoadedEvent(instrumentgrid, instrumentGridBinding);

//    BindOnLoadedEvent(contractdetrailgrid, function () {
//        contractDetailGridBinding("ContractCostGrid");
//    });
}

function InitialDatePicker() {
    $('#DocRecieveDate').InitialDate();
    $('#BidGuaranteeReturnDate1').InitialDate();
    $('#BidGuaranteeReturnDate2').InitialDate();
    $('#ExpectedCompleteInstallDate').InitialDate();
    $('#ExpectedCustomerAccept').InitialDate();
    $('#InstrumentStockOutDate').InitialDate();
    $('#SubcontractCompleteInstallDate').InitialDate();
    $('#CompleteInstallDate').InitialDate();
    $('#CompleteInstallDate2').InitialDate();
    $('#CustomerAcceptDate').InitialDate();
    $('#DeliveryDocRecieve').InitialDate();
    $('#DeliveryDocRecieveDate').InitialDate();

    $('#PaymentDateIncentive').InitialDate();

//    InitialDateFromToControl("#DocRecieveDate", "#DocRecieveDate");
//    InitialDateFromToControl("#BidGuaranteeReturnDate1", "#BidGuaranteeReturnDate1");
//    InitialDateFromToControl("#BidGuaranteeReturnDate2", "#BidGuaranteeReturnDate2");

//    InitialDateFromToControl("#ExpectedCompleteInstallDate", "#ExpectedCompleteInstallDate");
//    InitialDateFromToControl("#ExpectedCustomerAccept", "#ExpectedCustomerAccept");
//    InitialDateFromToControl("#InstrumentStockOutDate", "#InstrumentStockOutDate");
//    InitialDateFromToControl("#SubcontractCompleteInstallDate", "#SubcontractCompleteInstallDate");
//    InitialDateFromToControl("#CompleteInstallDate", "#CompleteInstallDate");
//    InitialDateFromToControl("#CustomerAcceptDate", "#CustomerAcceptDate");
//    InitialDateFromToControl("#DeliveryDocRecieve", "#DeliveryDocRecieve");

//    InitialDateFromToControl("#DeliveryDocRecieveDate", "#DeliveryDocRecieveDate");

    InitialDateFromToControl("#WaranteePeriodFrom", "#WaranteePeriodTo");
    //InitialDateFromToControl("#WaranteePeriodTo", "#WaranteePeriodTo");
}

function InitialScreen() {
    SetScreenDefault();
}

function InitialNumericBox() {
    $('#BidGuaranteeAmount1').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#BidGuaranteeAmount1').setComma();
    $('#BidGuaranteeAmount2').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#BidGuaranteeAmount2').setComma();

    $('#NewBuildingMgmtCost').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#NewBuildingMgmtCost').setComma();

    $('#NormalInstallFee').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#NormalInstallFee').setComma();
    $('#OrderInstallFee').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#OrderInstallFee').setComma();
    $('#SecomPayment').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#SecomPayment').setComma();
    $('#SecomRevenue').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#SecomRevenue').setComma();

    $('#ProductBillingAmount').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#ProductBillingAmount').setComma();
    $('#ProductNormalAmount').BindNumericBox(12, numBox_Decimal, numBox_Min, 999999999999.99, numBox_DefaultMin);
    $('#ProductNormalAmount').setComma();
    $('#InstallBillingAmount').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#InstallBillingAmount').setComma();
    $('#InstallNormalAmount').BindNumericBox(12, numBox_Decimal, numBox_Min, 999999999999.99, numBox_DefaultMin);
    $('#InstallNormalAmount').setComma();
    $('#TotalBillingAmount').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#TotalBillingAmount').setComma();
    $('#TotalNormalAmount').BindNumericBox(numBox_Length, numBox_Decimal, numBox_Min, numBox_Max, numBox_DefaultMin);
    $('#TotalNormalAmount').setComma();

    $('#AddedQty').BindNumericBox(7, 0, numBox_Min, 9999999, numBox_DefaultMin);
    $('#AddedQty').setComma();
}

function InitialEvent() {
    $('#btnRetrieve').click(btnRetrieve_click);
    $('#lnkMore').click(lnkMore_Click);
    $('#lnkLess').click(lnkLess_Click);
    $('#btnSearchInstrument').click(btnSearchInstrument_click);
    $('#btnAddInstrument').click(btnAddInstrument_click);
    $('#btnClearInstrument').click(btnClearInstrument_click);

    //$('#QuotationCode').click(QuotationCode_click);
    $('#AcquisitionType').change(AcquisitionType_change);
    $('#DistributedType').change(DistributeType_change);

    $('#QuotationCode').click(ViewQuotation);
    $('#InstallSlipNo1').click(function () {
        ViewInstallSlip($('#HidInstallSlipNo1').val());
    });
    $('#InstallSlipNo2').click(function () {
        ViewInstallSlip($('#HidInstallSlipNo2').val());
    });

    GetEmployeeNameData("#IEInChargeCode", "#IEInChargeName");
    GetEmployeeNameData("#NegotiationStaffCode", "#NegotiationStaffName");
    GetEmployeeNameData("#CompleteRegistrantCode", "#CompleteRegistrantName");

//    $('#IEInChargeCode').blur(function () {
//        GetEmployee('IEInCharge');
//    });

//    $('#NegotiationStaffCode').blur(function () {
//        GetEmployee('NagotiateStaff');
//    });

//    $('#CompleteRegistrantCode').blur(function () {
//        GetEmployee('CompleteRegis');
//    });

    $('#InstrumentCode').blur(function () {
        GetInstrument('Instrument');
    });

    $('#InstrumentCode').keypress(function () {
        $('#InstrumentName').val('');
        $('#InstrumentMaker').val('');
        $('#AddedQty').val('');
    });

//    $('#SalesmanCode1').blur(function () {
//        GetSalesman("1");
//    });
//    $('#SalesmanCode1').keypress(function () {
//        $('#SalesmanName1').val('');
//    });
//    $('#SalesmanCode2').blur(function () {
//        GetSalesman("2");
//    });
//    $('#SalesmanCode2').keypress(function () {
//        $('#SalesmanName2').val('');
//    });
//    $('#SalesmanCode3').blur(function () {
//        GetSalesman("3");
//    });
//    $('#SalesmanCode3').keypress(function () {
//        $('#SalesmanName3').val('');
//    });
//    $('#SalesmanCode4').blur(function () {
//        GetSalesman("4");
//    });
//    $('#SalesmanCode4').keypress(function () {
//        $('#SalesmanName4').val('');
//    });
//    $('#SalesmanCode5').blur(function () {
//        GetSalesman("5");
//    });
//    $('#SalesmanCode5').keypress(function () {
//        $('#SalesmanName5').val('');
//    });
//    $('#SalesmanCode6').blur(function () {
//        GetSalesman("6");
//    });
//    $('#SalesmanCode6').keypress(function () {
//        $('#SalesmanName6').val('');
//    });
//    $('#SalesmanCode7').blur(function () {
//        GetSalesman("7");
//    });
//    $('#SalesmanCode7').keypress(function () {
//        $('#SalesmanName7').val('');
//    });
//    $('#SalesmanCode8').blur(function () {
//        GetSalesman("8");
//    });
//    $('#SalesmanCode8').keypress(function () {
//        $('#SalesmanName8').val('');
//    });
//    $('#SalesmanCode9').blur(function () {
//        GetSalesman("9");
//    });
//    $('#SalesmanCode9').keypress(function () {
//        $('#SalesmanName9').val('');
//    });
//    $('#SalesmanCode10').blur(function () {
//        GetSalesman("10");
//    });
//    $('#SalesmanCode10').keypress(function () {
//        $('#SalesmanName10').val('');
    //    });

    GetEmployeeNameData("#SalesmanCode1", "#SalesmanName1");
    GetEmployeeNameData("#SalesmanCode2", "#SalesmanName2");
    GetEmployeeNameData("#SalesmanCode3", "#SalesmanName3");
    GetEmployeeNameData("#SalesmanCode4", "#SalesmanName4");
    GetEmployeeNameData("#SalesmanCode5", "#SalesmanName5");
    GetEmployeeNameData("#SalesmanCode6", "#SalesmanName6");
    GetEmployeeNameData("#SalesmanCode7", "#SalesmanName7");
    GetEmployeeNameData("#SalesmanCode8", "#SalesmanName8");
    GetEmployeeNameData("#SalesmanCode9", "#SalesmanName9");
    GetEmployeeNameData("#SalesmanCode10", "#SalesmanName10");

    $('#NewBuildMgmtType_Need').change(NewBuildMgmtType_change);
    $('#NewBuildMgmtType_NoNeed').change(NewBuildMgmtType_change);

    $('#ProductBillingAmount').blur(function () {
        CalculateOCDC("Product");
    });

    $('#ProductNormalAmount').blur(function () {
        CalculateOCDC("Product");
    });

    $('#InstallBillingAmount').blur(function () {
        CalculateOCDC("Install");
    });

    $('#InstallNormalAmount').blur(function () {
        CalculateOCDC("Install");
    });

    InitialTrimTextEvent(["ChangeApproveNo1", "ChangeApproveNo2"]);

//    $('#TotalBillingAmount').blur(function () {
//        CalculateOCDC("Total");
//    });

//    $('#TotalNormalAmount').blur(function () {
//        CalculateOCDC("Total");
//    });
}

//$.fn.OpenCMS170Dialog = function (callerID) {
//    var ctrl = $(this);
//    ajax_method.CallPopupScreenControllerWithAuthority(callerID, "/Common/CMS170", "", function (result) {
//        if (result != undefined) {

//            /* --- Button Events --- */
//            /* --------------------- */
//            var event = {
//                Close: function (data) {
//                    ctrl.CloseDialog();
//                }
//            };
//            /* --------------------- */

//            /* --- Event when open Dialog --- */
//            /* ------------------------------ */
//            var open_event = function (event, ui) {
//                if (typeof (CMS170Initial) == "function")
//                    CMS170Initial();
//            };
//            /* ------------------------------ */

//            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);
//        }
//    });
//}

// ---------------------------------------------------------------------------------
// Event
// ---------------------------------------------------------------------------------
function CMS170Response(dtNewInst) {
    $('#InstrumentCode').val(dtNewInst.InstrumentCode);
    $('#InstrumentName').val(dtNewInst.InstrumentName);
    $('#InstrumentMaker').val(dtNewInst.Maker);
    $("#dlgBox").CloseDialog();

    isValidAddInstrument = true;
}

function ViewQuotation() {
    //ajax_method.CallScreenControllerWithAuthority("/Common/QUS011", obj, true);
    $("#dlgBox").OpenQUS011Dialog("CTS150");
}

function ViewInstallSlip(slipNo) {
    var obj = {
        InstallationSlipNo: slipNo,
        ContractCode: $('#EntryContractCode').val()
    };
    ajax_method.CallScreenControllerWithAuthority("/Common/CMS180", obj, true);
}

$.fn.CalculateQty = function () {
    $(this).blur(function () {
        var _colCode = instrumentgrid.getColIndexById("InstrumentCode");
        var _colTotal = instrumentgrid.getColIndexById("QtyTotal");
        var _colHidAdded = instrumentgrid.getColIndexById("QtyAdded");
        var _colHidRemoved = instrumentgrid.getColIndexById("QtyRemoved");
        var row_id = GetGridRowIDFromControl(this);

        var txtAdded = "#" + GenerateGridControlID(instQtyAdded, row_id);
        var txtRemoved = "#" + GenerateGridControlID(instQtyRemoved, row_id);
        var txtTotal = "#" + GenerateGridControlID(instQtyTotal, row_id);

        var val1 = 0;
        if ($(txtAdded).NumericValue() != "")
            val1 = parseInt($(txtAdded).NumericValue());

        var val2 = 0;
        if ($(txtRemoved).NumericValue() != "")
            val2 = parseInt($(txtRemoved).NumericValue());
        
        var totalVal = val1 - val2;

        instrumentgrid.cells(row_id, _colHidAdded).setValue(val1);
        instrumentgrid.cells(row_id, _colHidRemoved).setValue(val2);

        $(txtTotal).val(totalVal).setComma();

        // Update in array data
        //curInstrumentData[curInstrumentCode[row_id]].total = totalVal;
        curInstrumentData[instrumentgrid.getRowIndex(row_id)].total = totalVal; //Modify by Jutarat A. on 09052013
        //CollectDataFromGrid();
    });
}

function instrumentGridBinding(gen_ctrl) {
    if (!CheckFirstRowIsEmpty(instrumentgrid)) {
        var _colQtyAdd = instrumentgrid.getColIndexById("QtyAdded");
        var _colQtyRemove = instrumentgrid.getColIndexById("QtyRemoved");
        var _colQtyTotal = instrumentgrid.getColIndexById("QtyTotal");

        for (var i = 0; i < instrumentgrid.getRowsNum(); i++) {
            var row_id = instrumentgrid.getRowId(i);

            if (gen_ctrl == true) {
                var val1 = GetValueFromLinkType(instrumentgrid, i, _colQtyAdd);
                var val2 = GetValueFromLinkType(instrumentgrid, i, _colQtyRemove);
                var val3 = GetValueFromLinkType(instrumentgrid, i, _colQtyTotal);

                GenerateNumericBox2(instrumentgrid, instQtyAdded, row_id, "TxtQtyAdded", val1, numBox_Length, 0, numBox_Min, numBox_Max, numBox_DefaultMin, true);
                GenerateNumericBox2(instrumentgrid, instQtyRemoved, row_id, "TxtQtyRemoved", val2, numBox_Length, 0, numBox_Min, numBox_Max, numBox_DefaultMin, true);
                GenerateNumericBox2(instrumentgrid, instQtyTotal, row_id, "QtyTotal", val3, numBox_Length, 0, numBox_Min, numBox_Max, numBox_DefaultMin, false);
            }

            var txtAdd = "#" + GenerateGridControlID(instQtyAdded, row_id);
            var txtRemove = "#" + GenerateGridControlID(instQtyRemoved, row_id);

            $(txtAdd).CalculateQty();
            $(txtRemove).CalculateQty();
        }

        instrumentgrid.setSizes();
    }
}

function GetEmployee(control_name) {
    var ctrl_code = control_name + "Code";
    var ctrl_name = control_name + "Name";

    if ($('#' + ctrl_code).val().length > 0) {
        var obj = {
            Code: $('#' + ctrl_code).val()
        };

        call_ajax_method_json("/Contract/CTS150_RetrieveEmployeeByCode", obj, function (result, controls) {
            if (result != null) {
                $('#' + ctrl_name).val(result.Name);
            } else {
                doAlert("Common", "MSG0095", $('#' + ctrl_code).val());
                //VaridateCtrl([ctrl_code], ctrl_code);
            }
        });
    } else {
        $('#' + ctrl_name).val('');
    }
}

function GetInstrument(control_name) {
    var ctrl_code = control_name + "Code";
    var ctrl_name = control_name + "Name";
    var ctrl_maker = control_name + "Maker";

    isValidAddInstrument = false;

    if ($('#' + ctrl_code).val().length > 0) {
        var obj = {
            Code: $('#' + ctrl_code).val()
        };

        call_ajax_method_json("/Contract/CTS150_RetrieveInstrumentByCode", obj, function (result, controls) {
            if (result != null) {
                $('#' + ctrl_name).val(result.Name);
                $('#' + ctrl_maker).val(result.Maker);

                isValidAddInstrument = true;
                //            } else {
                //                doAlert("Common", "MSG0082", $('#' + ctrl_code).val());
                //                VaridateCtrl([ctrl_code], [ctrl_code]);
                //            }
            }
        });
    } else {
        $('#' + ctrl_name).val('');
        $('#' + ctrl_maker).val('');
    }
}

function GetSalesman(control_no) {
    var control_name = "Salesman";
    var ctrl_code = control_name + "Code" + control_no;
    var ctrl_name = control_name + "Name" + control_no;

    if ($('#' + ctrl_code).val().length > 0) {
        var obj = {
            Code: $('#' + ctrl_code).val()
        };

        call_ajax_method_json("/Contract/CTS150_RetrieveEmployeeByCode", obj, function (result, controls) {
            if (result != null) {
                $('#' + ctrl_name).val(result.Name);
            } else {
                doAlert("Common", "MSG0095", $('#' + ctrl_code).val());
                VaridateCtrl([ctrl_code], [ctrl_code]);
            }
        });
    } else {
        $('#' + ctrl_name).val('');
    }
}

function DistributeType_change() {
    if (($('#DistributedType').val() == C_DISTRIBUTED_TYPE_NOT_DISTRIBUTED)
    || ($('#DistributedType').val() == C_DISTRIBUTED_TYPE_ORIGIN)
    || ($('#DistributedType').val() == "")) {
        DisableDistributeOriginCode();
    } else {
        EnableDistributeOriginCode();
        $('#DistributedOriginCode').val(currDistributedOriginCode);
    }
}

function NewBuildMgmtType_change() {
    if ($('input[name="NewBuildMgmtType"]:checked').val() == "1") {
        EnableNewBuildMgmtCost();
    } else {
        DisableNewBuildMgmtCost();
    }
}

function SetBuildMgmtType(setval) {
    var obj = $('input:radio[name=NewBuildMgmtType]');
    obj.filter('[value=' + setval + ']').attr('checked', true);
}

function GetBuildMgmtType() {
    return $('input[name="NewBuildMgmtType"]:checked').val();
}

function QuotationCode_click() {
    var obj = {
        QuotationTargetCode: currQuotationCode,
        Alphabet: currAlphabet,
        HideQuotationTargetFlag: true
    };
    ajax_method.CallScreenControllerWithAuthority("/Quotation/QUS011", obj, true);
//    if (($('#DistributedType').val() == C_DISTRIBUTED_TYPE_NOT_DISTRIBUTED)
//    || ($('#DistributedType').val() == C_DISTRIBUTED_TYPE_ORIGIN)) {
//        DisableDistributeOriginCode();
//    } else {
//        EnableDistributeOriginCode();
//    }

    return false;
}

function AcquisitionType_change() {
    if (($('#AcquisitionType').val() == C_ACQUISITION_TYPE_CUST) ||
    ($('#AcquisitionType').val() == C_ACQUISITION_TYPE_INSIDE_COMPANY) ||
    ($('#AcquisitionType').val() == C_ACQUISITION_TYPE_SUBCONTRACTOR)) {
        EnableIntroduceCode();
    } else {
        DisableIntroduceCode();
    }
}

function btnSearchInstrument_click() {
    $("#dlgBox").OpenCMS170Dialog("CTS150");
}

function btnAddInstrument_click() {
    var destCode = $('#InstrumentCode').val().toUpperCase();
    var destName = $('#InstrumentName').val();
    var destMaker = $('#InstrumentMaker').val();
    var destQty = $('#AddedQty').val().replace(/ /g, "").replace(/,/g, "");

    if ((destCode == undefined)
        || (destCode.length == 0)) {
        //isValidAddInstrument = false;
        doAlert("Common", "MSG0007", _lblInstrumentCode);
        VaridateCtrl(["InstrumentCode", "AddedQty"], ["InstrumentCode"]);
    } else if ((destQty == undefined)
        || (destQty.length == 0)) {
        //isValidAddInstrument = false;
        doAlert("Common", "MSG0007", _lblInstrumentAddedQty);
        VaridateCtrl(["InstrumentCode", "AddedQty"], ["AddedQty"]);
    } else if (destQty <= 0) {
        //isValidAddInstrument = false;
        doAlert("Common", "MSG0084", _lblInstrumentAddedQty);
        VaridateCtrl(["InstrumentCode", "AddedQty"], ["AddedQty"]);
    } else if (isValidAddInstrument == false) {
        doAlert("Common", "MSG0082", destCode);
        VaridateCtrl(["InstrumentCode", "AddedQty"], ["InstrumentCode"]);
    } else if (isValidAddInstrument) {
        //if (curInstrumentData[destCode] != undefined) {
        if (CheckDupInstrument(destCode) == true) { //Modify by Jutarat A. on 09052013
            doAlert("Contract", "MSG3163", destCode);

            // code exists
//            curInstrumentData[destCode].total = parseInt(curInstrumentData[destCode].total) + parseInt(destQty);

//            var _colCode = instrumentgrid.getColIndexById("InstrumentCode");
//            var _colTotal = instrumentgrid.getColIndexById("QtyTotal");
//            var _colTxtAdded = instrumentgrid.getColIndexById("TxtQtyAdded");
//            var _colAdded = instrumentgrid.getColIndexById("QtyAdded");

//            for (var i = 0; i < instrumentgrid.getRowsNum(); i++) {
//                var row_id = instrumentgrid.getRowId(i);

//                if (instrumentgrid.cells(row_id, _colCode).getValue() == destCode) {
//                    var txtAdded = "#" + GenerateGridControlID(instQtyAdded, row_id);
//                    var txtRemoved = "#" + GenerateGridControlID(instQtyRemoved, row_id);

//                    var val1 = parseInt($(txtAdded).val().replace(/ /g, "").replace(/,/g, "")) + parseInt(destQty);
//                    var val2 = parseInt($(txtRemoved).val().replace(/ /g, "").replace(/,/g, ""));

//                    if (val1.length == 0) {
//                        val1 = 0;
//                    }

//                    if (val2.length == 0) {
//                        val2 = 0;
//                    }

//                    var totalVal = val1 - val2;

//                    $(txtAdded).val(val1);
//                    instrumentgrid.cells(row_id, _colTotal).setValue(totalVal);
//                    instrumentgrid.cells(row_id, _colAdded).setValue(val1);
//                }
//            }
        } else {
            var obj = {
                Code: destCode
            };

            call_ajax_method_json("/Contract/CTS150_AddInstrument", obj, function (result, controls) {
                if (result != undefined) {
                    // add new
                    var rowObj = {
                        code: destCode,
                        total: destQty
                    };

                    //curInstrumentData[destCode] = rowObj;
                    curInstrumentData[instrumentgrid.getRowsNum()] = rowObj; //Modify by Jutarat A. on 09052013

                    //instrumentgrid.addRow(instrumentgrid.uid(), [destName, destCode, destMaker, destQty, "0", destQty]);
                    CheckFirstRowIsEmpty(instrumentgrid, true);

                    AddNewRow(instrumentgrid,
                        [   ConvertBlockHtml(destName), //destName, //Modify by Jutarat A. on 28112013
                            ConvertBlockHtml(destCode), //destCode, //Modify by Jutarat A. on 28112013
                            destMaker,
                            destQty,
                            "0",
                            "",
                            "", 
                            ""]);

                    var row_idx = instrumentgrid.getRowsNum() - 1;
                    var row_id = instrumentgrid.getRowId(row_idx);

                    GenerateNumericBox2(instrumentgrid, instQtyAdded, row_id, "TxtQtyAdded", destQty, numBox_Length, 0, numBox_Min, numBox_Max, numBox_DefaultMin, true);
                    GenerateNumericBox2(instrumentgrid, instQtyRemoved, row_id, "TxtQtyRemoved", 0, numBox_Length, 0, numBox_Min, numBox_Max, numBox_DefaultMin, true);
                    GenerateNumericBox2(instrumentgrid, instQtyTotal, row_id, "QtyTotal", destQty, numBox_Length, 0, numBox_Min, numBox_Max, numBox_DefaultMin, false);
                    instrumentgrid.setSizes();

                    var lastRowID = instrumentgrid.getRowId(instrumentgrid.getRowsNum() - 1);
                    curInstrumentCode[lastRowID] = rowObj.code;

                    var txtAdd = "#" + GenerateGridControlID(instQtyAdded, row_id);
                    var txtRemove = "#" + GenerateGridControlID(instQtyRemoved, row_id);

                    $(txtAdd).CalculateQty();
                    $(txtRemove).CalculateQty();
                }
            });
        }

        ClearInstrumentAdd();
        isValidAddInstrument = false;
    }
}

//Add by Jutarat A. on 09052013
function CheckDupInstrument(destCode) {
    for (i = 0; i < instrumentgrid.getRowsNum(); i++) {
        if (curInstrumentData[i].code == destCode) {
            return true;
        }
    }

    return false;
}
//End Add

function btnClearInstrument_click() {
    ClearInstrumentAdd();
}

function btnRetrieve_click() {
    DisableRegisterCommand(true);
    //$('#OccurrenceCode').val('0002');

    if (($('#EntryContractCode').val().length > 0) && ($('#OccurrenceCode').val().length > 0)) {
        $('#EntryContractCode').removeClass("highlight");
        $('#OccurrenceCode').removeClass("highlight");

        $('#btnRetrieve').attr("disabled", true);

        var obj = {
            strContractCode: $('#EntryContractCode').val(),
            strOccurrenceCode: $('#OccurrenceCode').val()
        };

        call_ajax_method_json("/Contract/CTS150_RetrieveAllContractInformation", obj, function (result, controls) {
            $('#btnRetrieve').removeAttr("disabled");

            if (controls != undefined) {
                // Hilight Text
                VaridateCtrl(["EntryContractCode", "OccurrenceCode"], controls);
            }

            if (result != null) {
                $('#OccurrenceCode').attr("readonly", true);
                $('#btnRetrieve').attr("disabled", true);

                ClearAllData();
                MappingDataToForm(result);
                LoadGridData(obj, result.ViewMode);
                SetEditMode();

                DisableRegisterCommand(false);
            }
        });
    } else {
        // Error
        if ($('#EntryContractCode').val().length == 0) {
            VaridateCtrl(["EntryContractCode"], ["EntryContractCode"]);
            doAlert("Common", "MSG0007", _lblContractCode);
        } else {
            VaridateCtrl(["OccurrenceCode"], ["OccurrenceCode"]);
            doAlert("Common", "MSG0007", _lblOccurrenceCode);
        }
    }
}

function lnkMore_Click() {
    ShowSaleman();
}

function lnkLess_Click() {
    HideSaleman();
}

function Register_Click() {
    command_control.CommandControlMode(false);

    DisableRegisterCommand(true);
    var obj = GetObjectFromData();

    call_ajax_method_json("/Contract/CTS150_ValidateBusiness", obj, function (result) {
        if (result != undefined) {
            if (result.IsValid) {
                HideRegisterPane();
                ShowConfirmPane();
                SetViewMode();
            } else {
                OpenWarningDialog(result.InvalidMessage, result.InvalidMessage, null);
                VaridateCtrl(validateControlList, result.InvalidControl);
            }
        }

        DisableRegisterCommand(false);
    });
}

function Reset_Click() {
    DisableRegisterCommand(true);

    var obj = {
        module: "Common",
        code: "MSG0038"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message, function () {
            SetScreenDefault();
        }, function () {
            DisableRegisterCommand(false);
        });
    });
}

function Confirm_Click() {
    command_control.CommandControlMode(false);

    DisableRegisterCommand(true);

    var obj = GetObjectFromData();

    call_ajax_method_json("/Contract/CTS150_RegisterCQ31", obj, function (result) {
        if (result != undefined) {
            if (result.IsValid) {
                // Get Message
                var obj = {
                    module: "Common",
                    code: "MSG0046"
                };

                call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                    OpenInformationMessageDialog(result.Code, result.Message, function () {
                        SetScreenDefault();
                    });

                });
            }
        }

        DisableRegisterCommand(false);
    });
}

function Back_Click() {
    DisableRegisterCommand(true);
    HideConfirmPane();
    ShowRegisterPane();
    SetEditMode()
    DisableRegisterCommand(false);
}

// ---------------------------------------------------------------------------------
// Method
// ---------------------------------------------------------------------------------

function GetObjectFromData() {

    //Add by Jutarat A. on 03042013
    var installCompleteDate = null;

    if (viewMode == 1) {
        installCompleteDate = $('#CompleteInstallDate').val();
    }
    else if (viewMode == 3) {
        installCompleteDate = $('#CompleteInstallDate2').val();
    }
    //End Add

    var obj = {
        ContractCode: $('#EntryContractCode').val(),
        OCCCode: $('#OccurrenceCode').val(),

        ContractOfficeCode: $('#ContractOffice').val(),
        OperationOfficeCode: $('#OperationOffice').val(),
        SalesOfficeCode: $('#SalesOffice').val(),
        QuotationNo: $("#QuotationNo").val(),
        SaleType: $('#SaleType').val(),
        DocAuditResult: $('#DocAuditResult').val(),
        DocRecieveDate: $('#DocRecieveDate').val(),
        DistributedType: $('#DistributedType').val(),
        DistributedOriginCode: $('#DistributedOriginCode').val(),
        OnlineContractCode: $('#OnlineContractCode').val(),

        BillingProductPriceCurrencyType: $('#ProductBillingAmount').NumericCurrencyValue(),
        BillingProductPrice: parseFloat($('#ProductBillingAmount').val().replace(/ /g, "").replace(/,/g, "")).toFixed(2),

        NormalProductPriceCurrencyType: $('#ProductNormalAmount').NumericCurrencyValue(),
        NormalProductPrice: parseFloat($('#ProductNormalAmount').val().replace(/ /g, "").replace(/,/g, "")).toFixed(2),

        BillingInstallFeeCurrencyType: $('#InstallBillingAmount').NumericCurrencyValue(),
        BillingInstallFee: parseFloat($('#InstallBillingAmount').val().replace(/ /g, "").replace(/,/g, "")).toFixed(2),

        NormalInstallFeeCurrencyType: $('#InstallNormalAmount').NumericCurrencyValue(),
        NormalInstallFee: parseFloat($('#InstallNormalAmount').val().replace(/ /g, "").replace(/,/g, "")).toFixed(2),
        
        BidGuaranteeAmount1CurrencyType: $('#BidGuaranteeAmount1').NumericCurrencyValue(),
        BidGuaranteeAmount1: parseFloat($('#BidGuaranteeAmount1').val().replace(/ /g, "").replace(/,/g, "")).toFixed(2),

        BidGuaranteeAmount2CurrencyType: $('#BidGuaranteeAmount2').NumericCurrencyValue(),
        BidGuaranteeAmount2: parseFloat($('#BidGuaranteeAmount2').val().replace(/ /g, "").replace(/,/g, "")).toFixed(2),

        BidGuaranteeReturnDate1: $('#BidGuaranteeReturnDate1').val(),
        BidGuaranteeReturnDate2: $('#BidGuaranteeReturnDate2').val(),
        PurchaseReasonType: $('#PurchaseReasonType').val(),
        AcquisitionType: $('#AcquisitionType').val(),
        IntroducerCode: $('#IntroducerCode').val(),
        ContractApproveNo1: $('#ContractApproveNo1').val(),
        ContractApproveNo2: $('#ContractApproveNo2').val(),
        ContractApproveNo3: $('#ContractApproveNo3').val(),
        ContractApproveNo4: $('#ContractApproveNo4').val(),
        ContractApproveNo5: $('#ContractApproveNo5').val(),
        SalesmanCode: [$('#SalesmanCode1').val()
            , $('#SalesmanCode2').val()
            , $('#SalesmanCode3').val()
            , $('#SalesmanCode4').val()
            , $('#SalesmanCode5').val()
            , $('#SalesmanCode6').val()
            , $('#SalesmanCode7').val()
            , $('#SalesmanCode8').val()
            , $('#SalesmanCode9').val()
            , $('#SalesmanCode10').val()],
//        Salesman1Code: $('#SalesmanCode1').val(),
//        Salesman2Code: $('#SalesmanCode2').val(),
//        Salesman3Code: $('#SalesmanCode3').val(),
//        Salesman4Code: $('#SalesmanCode4').val(),
//        Salesman5Code: $('#SalesmanCode5').val(),
//        Salesman6Code: $('#SalesmanCode6').val(),
//        Salesman7Code: $('#SalesmanCode7').val(),
//        Salesman8Code: $('#SalesmanCode8').val(),
//        Salesman9Code: $('#SalesmanCode9').val(),
//        Salesman10Code: $('#SalesmanCode10').val(),

        ProcessMgmtStatus: $('#ProcessManagementStatus').val(),
        ExpectedCompleteInstallDate: $('#ExpectedCompleteInstallDate').val(),
        ExpectedCustomerAcceptDate: $('#ExpectedCustomerAccept').val(),
        InstrumentStockOutDate: $('#InstrumentStockOutDate').val(),
        SubcontractCompleteInstallDate: $('#SubcontractCompleteInstallDate').val(),
        
        //CompleteInstallDate: $('#CompleteInstallDate').val(),
        CompleteInstallDate: installCompleteDate, //Modify by Jutarat A. on 03042013

        CustomerAcceptDate: $('#CustomerAcceptDate').val(),
        DeliveryDocRecieveDate: $('#DeliveryDocRecieveDate').val(),
        NewOldBuilding: $('#NewOldBuilding').val(),
        NewBuildingMgmtType: GetBuildMgmtType(),
        NewBuildingMgmtCost: parseFloat($('#NewBuildingMgmtCost').val().replace(/ /g, "").replace(/,/g, "")).toFixed(2),
        IEInCargeCode: $('#IEInChargeCode').val(),

        ChangeNormalInstallFeeCurrencyType: $('#NormalInstallFee').NumericCurrencyValue(),
        ChangeNormalInstallFee: parseFloat($('#NormalInstallFee').val().replace(/ /g, "").replace(/,/g, "")).toFixed(2),
        OrderInstallFeeCurrencyType: $('#OrderInstallFee').NumericCurrencyValue(),
        OrderInstallFee: parseFloat($('#OrderInstallFee').val().replace(/ /g, "").replace(/,/g, "")).toFixed(2),
        
        SecomPaymentCurrencyType: $('#SecomPayment').NumericCurrencyValue(),
        SecomPayment: parseFloat($('#SecomPayment').val().replace(/ /g, "").replace(/,/g, "")).toFixed(2),
        SecomRevenueCurrencyType: $('#SecomRevenue').NumericCurrencyValue(),
        SecomRevenue: parseFloat($('#SecomRevenue').val().replace(/ /g, "").replace(/,/g, "")).toFixed(2),


        //ChangeCompleteInstallDate: $('#CompleteInstallDate2').val(),
        ChangeCompleteInstallDate: installCompleteDate, //Modify by Jutarat A. on 03042013

        ChangeApproveNo1: $('#ChangeApproveNo1').val(),
        ChangeApproveNo2: $('#ChangeApproveNo2').val(),
        NegotiationStaffCode: $('#NegotiationStaffCode').val(),
        CompleteRegistrantCode: $('#CompleteRegistrantCode').val(),

        WaranteePeriodFrom: $('#WaranteePeriodFrom').val(),
        WaranteePeriodTo: $('#WaranteePeriodTo').val(),
        InstrumentDetail: curInstrumentData,
        ChangeType: currChangeTypeCode,

        PaymentDateIncentive: $('#PaymentDateIncentive').val(),
    };

    return obj;
}

function SetViewMode() {
    $('#divInstrumentAdd1').hide();
    $('#divInstrumentAdd2').hide();

    $('#divContractBasicInfo').SetViewMode(true);
    $('#divContractDetailInfo').SetViewMode(true);
    $('#divInstallInfo').SetViewMode(true);
    $('#divMaintenanceInfo').SetViewMode(true);
    $('#divChangeInfo').SetViewMode(true);
    $('#divInstrumentInfo').SetViewMode(true);

    $('#IsImportant').attr("disabled", true);

    $("#divQuotationCode").css({ "margin-top": "0px" });
    $("#divInstallSlipNo1").css({ "margin-top": "0px" });
    $("#divInstallSlipNo2").css({ "margin-top": "0px" });

    if ($("#NegotiationStaffCode").val() == "") {
        $("#divNegotiationStaffName").html("");
    }
    if ($("#CompleteRegistrantCode").val() == "") {
        $("#divCompleteRegistrantName").html("");
    }
}

function SetEditMode() {
    $('#divInstrumentAdd1').show();
    $('#divInstrumentAdd2').show();

    $('#divContractBasicInfo').SetViewMode(false);
    $('#divContractDetailInfo').SetViewMode(false);
    $('#divInstallInfo').SetViewMode(false);
    $('#divMaintenanceInfo').SetViewMode(false);
    $('#divChangeInfo').SetViewMode(false);
    $('#divInstrumentInfo').SetViewMode(false);

    $('#IsImportant').attr("disabled", true);
}

function CollectDataFromGrid() {
    var _colCode = instrumentgrid.getColIndexById("InstrumentCode");
    var _colTotal = instrumentgrid.getColIndexById("QtyTotal");

    curInstrumentData = new Array();
    curInstrumentCode = new Array();

    if (!CheckFirstRowIsEmpty(instrumentgrid)) { //Add by Jutarat A. on 08052013

        for (var i = 0; i < instrumentgrid.getRowsNum(); i++) {
            var row_id = instrumentgrid.getRowId(i);

            var qtyTotal = "#" + GenerateGridControlID(instQtyTotal, row_id); //Add by Jutarat A. on 08052013

            var rowObj = {
                code: instrumentgrid.cells(row_id, _colCode).getValue(),
                //total: instrumentgrid.cells(row_id, _colTotal).getValue().replace(/ /g, "").replace(/,/g, "")
                total: $(qtyTotal).NumericValue() //Modify by Jutarat A. on 08052013
            };

            //curInstrumentData[instrumentgrid.cells(row_id, _colCode).getValue()] = instrumentgrid.cells(row_id, _colTotal).getValue().replace(/ /g, "").replace(/,/g, "");
            //curInstrumentData[instrumentgrid.cells(row_id, _colCode).getValue()] = rowObj;
            curInstrumentData[i] = rowObj; //Modify by Jutarat A. on 08052013

            curInstrumentCode[row_id] = rowObj.code;
        }
    }
}

function LoadGridData(obj, viewMode) {
    if (viewMode == 1) {
        // Load Subcontrator (Install)
        $('#InstallSubCTGrid1').LoadDataToGrid(subcontractgrid1, 0, false, "/Contract/CTS150_RetrieveSubcontractorData", obj, "CTS150_SubContractorResult", false, null, null);
        // Load Instrument Grid
        $('#InstrumentGrid').LoadDataToGrid(instrumentgrid, 0, false, "/Contract/CTS150_RetrieveInstrumentData", obj, "CTS150_InstrumentResult", false, function () {
            CollectDataFromGrid();
        }, null);
    } else if (viewMode == 2) {
        // No load
    } else if (viewMode == 3) {
        // Load Subcontrator (Change)
        $('#InstallSubCTGrid2').LoadDataToGrid(subcontractgrid2, 0, false, "/Contract/CTS150_RetrieveSubcontractorData", obj, "CTS150_SubContractorResult", false, null, null);
        // Load Instrument Grid
        $('#InstrumentGrid').LoadDataToGrid(instrumentgrid, 0, false, "/Contract/CTS150_RetrieveInstrumentData", obj, "CTS150_InstrumentResult", false, function () {
            CollectDataFromGrid();
        }, null);
    }
}

function MappingDataToForm(formData) {
    viewMode = formData.ViewMode; //Add by Jutarat A. on 03042013


    //Comment by Jutarat A. on 21022013 (Map by ChangeType)
//    // Map data
//    $('#divContractBasicInfo').bindJSON(formData);
//    $('#divContractDetailInfo').bindJSON(formData);
//    $('#divInstallInfo').bindJSON(formData);
//    $('#divMaintenanceInfo').bindJSON(formData);
//    $('#divChangeInfo').bindJSON(formData);
//    $('#divInstrumentInfo').bindJSON(formData);
    //End Comment

    $('#QuotationCode').text(formData.QuotationCode);
    $('#InstallSlipNo1').text(formData.InstallSlipNo);
    $('#InstallSlipNo2').text(formData.InstallSlipNo);

    $('#HidInstallSlipNo1').val(formData.InstallSlipNo);
    $('#HidInstallSlipNo2').val(formData.InstallSlipNo);
    currQuotationCode = formData.RawQuotationCode;
    currAlphabet = formData.RawAlphabet;
    currMgmtCost = formData.NewBuildingMgmtCost;
    currChangeTypeCode = formData.ChangeTypeCode;
    currDistributedOriginCode = formData.DistributedOriginCode;


    SetDateFromToData("#WaranteePeriodFrom", "#WaranteePeriodTo", formData.WaranteePeriodFrom, formData.WaranteePeriodTo)


    //CalculateOCDC("Product"); //Comment by Jutarat A. on 21022013 (Map by ChangeType)
    //CalculateOCDC("Install"); //Comment by Jutarat A. on 21022013 (Map by ChangeType)

    $('#IsImportant').attr("checked", formData.IsImportant);

    if (formData.NewBuildMgmtType) {
        SetBuildMgmtType(1);
    } else {
        SetBuildMgmtType(0);
    }

    DistributeType_change();
    NewBuildMgmtType_change();
    AcquisitionType_change();
    //QuotationCode_click();

    // Set screen
    HideAll();

    $('#divContractBasicInfo').bindJSON(formData); //Add by Jutarat A. on 21022013
    ShowContractBasicInfo();

    if (formData.ViewMode == 1) {
        //Add by Jutarat A. on 21022013 (Map by ChangeType)
        $('#divContractDetailInfo').bindJSON(formData);
        $('#divInstallInfo').bindJSON(formData);
        $('#divInstrumentInfo').bindJSON(formData);

        CalculateOCDC("Product");
        CalculateOCDC("Install");
        //End Add

        ShowContractDetailInfo();
        ShowInstallInfo();
        ShowInstrumentInfo();
        HideChangeInfo();

        ShowRegisterPane();
    } else if (formData.ViewMode == 2) {
        HideContractDetailInfo();
        HideInstallInfo();
        HideInstrumentInfo();
        HideChangeInfo();

        //Modify by Jutarat A. on 26072012
        //HideRegisterPane();
        ShowResetPane();

    } else if (formData.ViewMode == 3) {
        //Add by Jutarat A. on 21022013 (Map by ChangeType)
        $('#divChangeInfo').bindJSON(formData);
        $('#divInstrumentInfo').bindJSON(formData);
        //End Add

        ShowChangeInfo();
        ShowInstrumentInfo();

        HideContractDetailInfo();
        HideInstallInfo();

        ShowRegisterPane();
    }

    if (formData.HasMA) {
        $('#divMaintenanceInfo').bindJSON(formData); //Add by Jutarat A. on 21022013
        ShowMaintenanceInfo();
    } else {
        HideMaintenanceInfo();
    }
}

function ClearAllData() {
    // Clear Form
    $('#divContractBasicInfo').clearForm();
    $('#divContractDetailInfo').clearForm();
    $('#divInstallInfo').clearForm();
    $('#divMaintenanceInfo').clearForm();
    $('#divChangeInfo').clearForm();
    $('#divInstrumentInfo').clearForm();

    // Clear Grid
    //subcontractgrid1.clearAll();
    DeleteAllRow(subcontractgrid1);
    //subcontractgrid2.clearAll();
    DeleteAllRow(subcontractgrid2);
    //instrumentgrid.clearAll();
    DeleteAllRow(instrumentgrid);
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

function CalculateOCDC(ctrlID) {
    var billingID = "#" + ctrlID + "BillingAmount";
    var notmalID = "#" + ctrlID + "NormalAmount";
    var totalID = "#" + ctrlID + "ResultAmount";
    var spTextID = "#sp" + ctrlID + "ResTxt";

    var billingVal = parseFloat($(billingID).val().replace(/ /g, "").replace(/,/g, ""));
    var normalVal = parseFloat($(notmalID).val().replace(/ /g, "").replace(/,/g, ""));

    var res = 0;

    if ((billingVal.length == 0) || isNaN(billingVal))
        billingVal = 0;

    if ((normalVal.length == 0) || isNaN(normalVal))
        normalVal = 0;

    if ((billingVal > 0) && (normalVal > 0)) {
        res = ((normalVal - billingVal) / normalVal) * 100;
    } else if ((billingVal == 0) && (normalVal == 0)) {
        res = 0;
    } else {
        if (billingVal == 0) {
            res = 100;
        } else {
            res = -100;
        }
    }

    res = res.toFixed(2);

    //var res = (((normalVal - billingVal) / billingVal) * 100).toFixed(2);

    if (res > 0) {
        $(totalID).val(SetNumericValue(res, 2));
        $(spTextID).text(_lblDiscount);
    } else if (res < 0) {
        $(totalID).val(SetNumericValue(Math.abs(res), 2));
        $(spTextID).text(_lblOverRecieve);
    } else {
        $(totalID).val(SetNumericValue(0, 2));
        $(spTextID).text(_lblDat);
    }

    $(totalID).setComma();

    CalculateTotalOCDC();
}

function CalculateTotalOCDC() {
    var billingProductID = "#ProductBillingAmount";
    var normalProductID = "#ProductNormalAmount";

    var billingInstallID = "#InstallBillingAmount";
    var normalInstallID = "#InstallNormalAmount";

    var billingTotalID = "#TotalBillingAmount";
    var normalTotalID = "#TotalNormalAmount"; 

    var totalID = "#TotalResultAmount";
    var spTotalTextID = "#spTotalResTxt";

    var billingVal = parseFloat($(billingProductID).val().replace(/ /g, "").replace(/,/g, "")) + parseFloat($(billingInstallID).val().replace(/ /g, "").replace(/,/g, ""));
    var normalVal = parseFloat($(normalProductID).val().replace(/ /g, "").replace(/,/g, "")) + parseFloat($(normalInstallID).val().replace(/ /g, "").replace(/,/g, ""));

    var res = 0;

    if ((billingVal.length == 0) || isNaN(billingVal))
        billingVal = 0;

    if ((normalVal.length == 0) || isNaN(normalVal))
        normalVal = 0;

    if ((billingVal > 0) && (normalVal > 0)) {
        res = ((normalVal - billingVal) / normalVal) * 100;
    } else if ((billingVal == 0) && (normalVal == 0)) {
        res = 0;
    } else {
        if (billingVal == 0) {
            res = 100;
        } else {
            res = -100;
        }
    }

    res = res.toFixed(2);

    //var res = (((normalVal - billingVal) / billingVal) * 100).toFixed(2);

    $(billingTotalID).val(SetNumericValue(billingVal, 2));
    $(normalTotalID).val(SetNumericValue(normalVal, 2));

    if (res > 0) {
        $(totalID).val(SetNumericValue(res, 2));
        $(spTotalTextID).text(_lblDiscount);
    } else if (res < 0) {
        $(totalID).val(SetNumericValue(Math.abs(res), 2));
        $(spTotalTextID).text(_lblOverRecieve);
    } else {
        $(totalID).val(SetNumericValue(0, 2));
        $(spTotalTextID).text(_lblDat);
    }

    $(billingTotalID).setComma();
    $(normalTotalID).setComma();
    $(totalID).setComma();
}

function getElementID(htmltxt, prefixID) {
    var tmpID = htmltxt.substring(htmltxt.indexOf(prefixID));
    tmpID = tmpID.substring(0, tmpID.indexOf(" "));
    return tmpID;
}

function SetContractCode(contractCode) {
    currContractCode = contractCode;
    $('#EntryContractCode').val(currContractCode);
}

function EnableDistributeOriginCode() {
    $('#DistributedOriginCode').removeAttr("readonly");
}

function DisableDistributeOriginCode() {
    $('#DistributedOriginCode').attr("readonly", true);
    $('#DistributedOriginCode').val('');
}

function EnableNewBuildMgmtCost() {
    $('#NewBuildingMgmtCost').removeAttr("readonly");
    $('#NewBuildingMgmtCost').val(currMgmtCost);
}

function DisableNewBuildMgmtCost() {
    $('#NewBuildingMgmtCost').attr("readonly", true);
    $('#NewBuildingMgmtCost').val('');
}

function EnableIntroduceCode() {
    $('#IntroducerCode').removeAttr("readonly");
}

function DisableIntroduceCode() {
    $('#IntroducerCode').attr("readonly", true);
    $('#IntroducerCode').val('');
}

function ClearSaleman() {
    for (i = 1; i <= 10; i++) {
        $('#SalesmanCode' + i.toString()).val('');
        $('#SalesmanName' + i.toString()).val('');
    }
}

function ClearSaleOccurrence() {
    //$('#EntryContractCode').val('');
    $('#OccurrenceCode').val('');
}

function ClearInstrumentAdd() {
    $('#InstrumentCode').val('');
    $('#InstrumentName').val('');
    $('#AddedQty').val('');
    $('#InstrumentMaker').val('');
}

function ShowSaleman() {
    $('#lnkMore').hide();
    $('#lnkLess').show();

    for (i = 4; i <= 10; i++) {
        $('#divSaleRow' + i.toString()).show();
    }
}

function HideSaleman() {
    $('#lnkMore').show();
    $('#lnkLess').hide();

    for (i = 4; i <= 10; i++) {
        $('#divSaleRow' + i.toString()).hide();
    }
}

function ShowRegisterPane() {
    SetRegisterCommand(true, Register_Click);
    SetResetCommand(true, Reset_Click);
}

function HideRegisterPane() {
    SetRegisterCommand(false, null);
    SetResetCommand(false, null);
}

function ShowConfirmPane() {
    SetConfirmCommand(true, Confirm_Click);
    SetBackCommand(true, Back_Click);
}

function HideConfirmPane() {
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
}

function ShowResetPane() {
    SetRegisterCommand(false, null);
    SetResetCommand(true, Reset_Click);
}

function ShowContractBasicInfo() {
    //HideAll();
    $('#divContractBasicInfo').show();
}

function ShowContractDetailInfo() {
    //HideAll();
    $('#divContractDetailInfo').show();
}

function ShowInstallInfo() {
    //HideAll();
    $('#divInstallInfo').show();
}

function ShowMaintenanceInfo() {
    //HideAll();
    $('#divMaintenanceInfo').show();
}

function ShowChangeInfo() {
    //HideAll();
    $('#divChangeInfo').show();
}

function ShowInstrumentInfo() {
    //HideAll();
    $('#divInstrumentInfo').show();
}

function HideContractBasicInfo() {
    $('#divContractBasicInfo').hide();
}

function HideContractDetailInfo() {
    $('#divContractDetailInfo').hide();
}

function HideInstallInfo() {
    $('#divInstallInfo').hide();
}

function HideMaintenanceInfo() {
    $('#divMaintenanceInfo').hide();
}

function HideChangeInfo() {
    $('#divChangeInfo').hide();
}

function HideInstrumentInfo() {
    $('#divInstrumentInfo').hide();
}

function HideAll() {
    HideContractBasicInfo();
    HideContractDetailInfo();
    HideInstallInfo();
    HideMaintenanceInfo();
    HideChangeInfo();
    HideInstrumentInfo();

    HideRegisterPane();
    HideConfirmPane();
}

function SetScreenDefault() {
    HideAll();
    HideSaleman();
    HideRegisterPane();
    HideConfirmPane();
    ClearInstrumentAdd();
    ClearSaleOccurrence();
    ClearSaleman();
    SetBuildMgmtType("0");
    DisableIntroduceCode();
    DisableNewBuildMgmtCost();
    DisableDistributeOriginCode();

    $('#OccurrenceCode').removeAttr("readonly");
    $('#btnRetrieve').removeAttr("disabled");
}

function CMS170Object() {
    var obj = { bProdTypeSale: true
    , bProdTypeAlarm: false
    , bExpTypeHas: true
    , bExpTypeNo: false
    , bInstTypeGen: true
    , bInstTypeMonitoring: false
    , bInstTypeMat: false
    };
    return obj;
}

function QUS011Object() {
    var obj = {
        QuotationTargetCode: currQuotationCode,
        Alphabet: currAlphabet,
        HideQuotationTarget: true
    };

    return obj;
}