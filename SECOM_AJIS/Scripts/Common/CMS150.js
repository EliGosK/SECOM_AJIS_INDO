//----------------------------------------------
// Create : Nattapong N.
//   Date : 8 Jul 2011
//
//---------------------------------------------
/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>

/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>
/// <reference path="../../Scripts/Base/AutoComplete.js"/>
/// <reference path="../jquery-1.5.1-vsdoc.js" />
/// <reference path="../Base/GridControl.js" />
/// <reference path="../../Views/Common/CMS150.cshtml" />
/// <reference path="../Base/control_events.js" />
/// <reference path="../Base/master.js" />


var specChange = new Array();
var specIncdAr = new Array();
var CMS150_CMS220ShowMode;
var RowPgs = ROWS_PER_PAGE_FOR_VIEWPAGE;
$(document).ready(function () {

    // TODO : Comment for next phase ---------------
    //    $("#btnViewIncdList").attr('disabled', 'disabled');
    //    $("#btnRegIncd").attr('disabled', 'disabled');
    //    $("#btnViewARlist").attr('disabled', 'disabled');
    //    $("#btnRegAR").attr('disabled', 'disabled');
    //---------------------------------------------

    //    $("#SpecifyIncdARType").attr('disabled', 'disabled');

    $("#btnSearch").click(function () {
        Filtering();
    });
    // ------------- End Search Button ----------------------

    initial_select_combo_list("#cbmChangeType", "#selChangeType", "#ChangeTypeAdd", "#ChangeTypeRemove");
    initial_select_combo_list("#cbmIncdAR", "#selIncdAR", "#IncdARAdd", "#IncdARRemove");



    $("#btnClear").click(function () {
        DisHistDigest();
        Filtering();
        call_ajax_method_json('/common/ListMiscChangeType', { 'serviceType': ServiceType }, function (data) {
            regenerate_combo('#cbmChangeType', data);
        });
        call_ajax_method_json('/common/ListMiscIncidentARType', '', function (data) {
            regenerate_combo('#cbmIncdAR', data);
        });
    });
    $("#SpecifyChgType").click(function () {
        if ($(this).prop('checked') == true) {
            $("#ChangeTypeAdd").removeAttr('disabled');
            $("#ChangeTypeRemove").removeAttr('disabled');
            $("#cbmChangeType").SetDisabled(false);
            $("#selChangeType").SetDisabled(false);
        } else {
            $("#ChangeTypeAdd").attr('disabled', 'disabled');
            $("#ChangeTypeRemove").attr('disabled', 'disabled');
            $("#cbmChangeType").SetDisabled(true);
            $("#selChangeType").SetDisabled(true);
        }
    });
    $("#SpecifyIncdARType").click(function () {
        if ($(this).prop('checked') == true) {
            $("#IncdARAdd").removeAttr('disabled');
            $("#IncdARRemove").removeAttr('disabled');
            $("#cbmIncdAR").SetDisabled(false);
            $("#selIncdAR").SetDisabled(false);
        } else {
            $("#IncdARAdd").attr('disabled', 'disabled');
            $("#IncdARRemove").attr('disabled', 'disabled');
            $("#cbmIncdAR").SetDisabled(true);
            $("#selIncdAR").SetDisabled(true);
        }
    });

    // ---------------------- Head Button -----------------------------
    $("#btnContractBillingTransfer").click(function () {
        ajax_method.CallScreenControllerWithAuthority("/common/CMS200", { strContractCode: strContractCode, strServiceTypeCode: ServiceType }, null);
    });
    $("#btnSalesContractBasic").click(function () {
        ajax_method.CallScreenControllerWithAuthority("/common/CMS160", { strContractCode: strContractCode }, null);

    });
    $("#btnContractBasic").click(function () {
        ajax_method.CallScreenControllerWithAuthority("/common/CMS120", { strContractCode: strContractCode }, false);
    });

    //----------------------For phase 2 ----------------------

    $("#btnHeader_Installation").click(function () {
        // go to CMS180

        var obj = { "ContractCode": strContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS180", obj, false);

    });

    $("#btnHeader_BillingBasic").click(function () {
        // go to CMS420

        var obj = { "ContractCode": strContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS420", obj, false);

    });

    // ---------------------- Head Button (End) -----------------------------


    //--------------------- Link ----------------------------------// 
    $("#lnkCustomerCodeContractTarget").click(function () {
        //pop up CMS220
        doContractInfoCondition = { "ContractCode": strContractCode, "OCC": OCC, "ContractTargetCode": ContractTargetCode };

        // tt**
        CMS150_CMS220ShowMode = "Contract";

        $("#diag").OpenCMS220Dialog();

        return false;
    });
    $("#lnkCustomerCodeRealCustomer").click(function () {
        //pop up CMS220
        doContractInfoCondition = { "ContractCode": strContractCode, "OCC": OCC, "RealCustomerCode": RealCustCode };

        // tt**
        CMS150_CMS220ShowMode = "Customer";

        $("#diag").OpenCMS220Dialog();
        return false;
    });
    $("#lnkSiteCode").click(function () {
        //pop up CMS220
        doContractInfoCondition = { "ContractCode": strContractCode, "OCC": OCC, "SiteCode": SiteCode };

        // tt**
        CMS150_CMS220ShowMode = "Site";

        $("#diag").OpenCMS220Dialog(); return false;
    });

    $("#lnkSaleSiteCode").click(function () {

        doContractInfoCondition = { "ContractCode": strContractCode, "OCC": OCC, "SiteCode": SiteCode };

        // tt**
        CMS150_CMS220ShowMode = "Site";

        $("#diag").OpenCMS220Dialog(); return false;
    });
    $('#lnkSaleCustCodePur').click(function () {

        //pop up CMS220
        doContractInfoCondition = { "ContractCode": strContractCode, "OCC": OCC, "PurchaserCustCode": CustCodePur };

        // tt**
        CMS150_CMS220ShowMode = "Purchaser";

        $("#diag").OpenCMS220Dialog(); return false;
    });
    $("#lnkSaleCustCodeRC").click(function () {
        //pop up CMS220
        doContractInfoCondition = { "ContractCode": strContractCode, "OCC": OCC, "RealCustomerCode": RealCustCode };

        // tt**
        CMS150_CMS220ShowMode = "Customer";

        $("#diag").OpenCMS220Dialog();

        return false;
    });
    //================= End Link ========================//


    InitGrid();
    /* ========= add event rigth button =========== */
    initialRightButton();
    DisHistDigest();



    if ($("#saleSection").length > 0) {
        $("#saleSection").SetEmptyViewData();
    } else if ($("#RentalSection").length > 0) {
        $("#RentalSection").SetEmptyViewData();
    }

    if (txtRentalAttachImportanceFlag == true) {
        $("#ChkRentalAttachImportanceFlag").attr("checked", true);
    }
    if (txtSaleAttachImportanceFlag == true) {
        $("#ChkSaleAttachImportanceFlag").attr("checked", true);
    }
});

function IsIn(arr, obj) {
    //return (arr.indexOf(obj) != -1);
    for (var i = 0; i < arr.length; i++) {
        if (arr[i] == obj)
            return true;
    }
    return false;
}


function DisHistDigest() {
    $("#cbmChangeType").SetDisabled(true);
    $("#cbmIncdAR").SetDisabled(true);
    $("#selChangeType").SetDisabled(true);
    $("#selIncdAR").SetDisabled(true);


    $('input:checkbox').removeAttr('checked');
    $("#selIncdAR").html('');
    $("#selChangeType").html('');


    $("#IncdARAdd").attr("disabled", "disabled");
    $("#IncdARRemove").attr("disabled", "disabled");

    $("#ChangeTypeAdd").attr("disabled", "disabled");
    $("#ChangeTypeRemove").attr("disabled", "disabled");

    if (ServiceType == cRentalType) {
        $("#btnSalesContractBasic").attr("disabled", "disabled");
        $("#saleSection").hide();
    }
    else {
        $("#btnContractBasic").attr("disabled", "disabled");
        $("#RentalSection").hide();
    }
}
function InitGrid() {
    if ($("#ResultList").length > 0) {
        if (ServiceType == cRentalType) {
            objParaRent = { "sType": ServiceType, "strLongContractCode": strLongContractCode };
            mygrid = $("#ResultList").LoadDataToGridWithInitial(RowPgs, true, true, "/Common/getRentalDigestCMS150", objParaRent, "View_dtRentalHistoryDigest", false);

            BindOnLoadedEvent(mygrid, function (gen_ctrl) {

                if (CheckFirstRowIsEmpty(mygrid, false) == false) { //Add by Jutarat A. on 15022013
                    for (var i = 0; i < mygrid.getRowsNum(); i++) {
                        var row_id = mygrid.getRowId(i);

                        var colContractType = mygrid.getColIndexById('ContractType');
                        var colOCC = mygrid.getColIndexById('OCC');

                        if (gen_ctrl == true) {
                            if (mygrid.cells(row_id, colContractType).getValue() == cConT_CONTACT) {
                                GenerateDetailButton(mygrid, "btnSecurityDetail", row_id, "SecurityDetail", true);
                                GenerateDetailButton(mygrid, "btnSecurityBasic", row_id, "SecurityBasic", true);
                                GenerateDetailButton(mygrid, "btnSecurityIncdAR", row_id, "IncidentAR", false);


                                var lblBtnSecurityDetail = GenerateGridControlID("btnSecurityDetail", row_id);
                                var lblBtnSecurityBasic = GenerateGridControlID("btnSecurityBasic", row_id);
                                var lblbtnSecurityIncdAR = GenerateGridControlID("btnSecurityIncdAR", row_id);

                                $("#" + lblBtnSecurityDetail).attr("title", cms150_SecDtl);
                                $("#" + lblBtnSecurityBasic).attr("title", cms150_SecBasc);
                                $("#" + lblbtnSecurityIncdAR).attr("title", cms150_IncdAR);



                            } else {
                                GenerateDetailButton(mygrid, "btnSecurityDetail", row_id, "SecurityDetail", false);
                                GenerateDetailButton(mygrid, "btnSecurityBasic", row_id, "SecurityBasic", false);
                                GenerateDetailButton(mygrid, "btnSecurityIncdAR", row_id, "IncidentAR", true);


                                var lblBtnSecurityDetail = GenerateGridControlID("btnSecurityDetail", row_id);
                                var lblBtnSecurityBasic = GenerateGridControlID("btnSecurityBasic", row_id);
                                var lblbtnSecurityIncdAR = GenerateGridControlID("btnSecurityIncdAR", row_id);

                                $("#" + lblBtnSecurityDetail).attr("title", cms150_SecDtl);
                                $("#" + lblBtnSecurityBasic).attr("title", cms150_SecBasc);
                                $("#" + lblbtnSecurityIncdAR).attr("title", cms150_IncdAR);



                            }
                        }

                        BindGridButtonClickEvent("btnSecurityDetail", row_id, function (rid) {
                            mygrid.selectRow(mygrid.getRowIndex(rid));
                            var OCC_Selected = mygrid.cells(rid, colOCC).getValue();
                            ajax_method.CallScreenControllerWithAuthority("/common/CMS140", { 'strContractCode': strContractCode, 'strOCC': OCC_Selected }, true);
                        });
                        BindGridButtonClickEvent("btnSecurityBasic", row_id, function (rid) {
                            mygrid.selectRow(mygrid.getRowIndex(rid));
                            var OCC_Selected = mygrid.cells(rid, colOCC).getValue();
                            ajax_method.CallScreenControllerWithAuthority("/common/CMS130", { 'strContractCode': strContractCode, 'strOCC': OCC_Selected }, true);
                        });
                        BindGridButtonClickEvent("btnSecurityIncdAR", row_id, function (rid) {
                            mygrid.selectRow(mygrid.getRowIndex(rid));
                            if (mygrid.cells(rid, colContractType).getValue() == C_CONTRACT_TYPE_INCIDENT) {
                                var IncidentID = mygrid.cells(rid, mygrid.getColIndexById('IncidentID')).getValue();
                                ajax_method.CallScreenControllerWithAuthority("/contract/CTS330", { 'strIncidentID': IncidentID }, true);
                            }
                            else if (mygrid.cells(rid, colContractType).getValue() == C_CONTRACT_TYPE_AR) {
                                var RequestNo = mygrid.cells(rid, mygrid.getColIndexById('RequestNo')).getValue();
                                ajax_method.CallScreenControllerWithAuthority("/contract/CTS380", { 'pRequestNo': RequestNo }, true);
                            }
                        });
                    }
                }

                mygrid.setSizes();
            });
            SpecialGridControl(mygrid, ["SecurityBasic", "SecurityDetail", "IncidentAR"]);
        } else {
            objParaSale = { "sType": ServiceType, "strLongContractCode": strLongContractCode, "OCC": CondOCC };
            mygrid = $("#ResultList").LoadDataToGridWithInitial(RowPgs, true, true, "/Common/getSaleDigestCMS150", objParaSale, "View_dtSaleHistoryDigestList", false);

            BindOnLoadedEvent(mygrid, function (gen_ctrl) {

                if (CheckFirstRowIsEmpty(mygrid, false) == false) { //Add by Jutarat A. on 15022013
                    for (var i = 0; i < mygrid.getRowsNum(); i++) {
                        var row_id = mygrid.getRowId(i);
                        var colContractType = mygrid.getColIndexById('ContractType')

                        if (gen_ctrl == true) {
                            if (mygrid.cells(row_id, colContractType).getValue() == cConT_CONTACT) {
                                GenerateDetailButton(mygrid, "btnSaleBasic", row_id, "SaleBasic", true);
                                GenerateDetailButton(mygrid, "btnIncdAR", row_id, "IncidentAR", false);

                                var lblBtnSaleBasic = GenerateGridControlID("btnSaleBasic", row_id);
                                var lblBtnIncdAR = GenerateGridControlID("btnIncdAR", row_id);

                                $("#" + lblBtnSaleBasic).attr("title", cms150_SaleBasic);
                                $("#" + lblBtnIncdAR).attr("title", cms150_IncdAR);


                            } else {
                                GenerateDetailButton(mygrid, "btnSaleBasic", row_id, "SaleBasic", false);
                                GenerateDetailButton(mygrid, "btnIncdAR", row_id, "IncidentAR", true);

                                var lblBtnSaleBasic = GenerateGridControlID("btnSaleBasic", row_id);
                                var lblBtnIncdAR = GenerateGridControlID("btnIncdAR", row_id);

                                $("#" + lblBtnSaleBasic).attr("title", cms150_SaleBasic);
                                $("#" + lblBtnIncdAR).attr("title", cms150_IncdAR);


                            }
                        }

                        BindGridButtonClickEvent("btnSaleBasic", row_id, function (rid) {
                            mygrid.selectRow(mygrid.getRowIndex(rid));
                            var OCC = mygrid.cells(rid, mygrid.getColIndexById('OCC')).getValue();
                            ajax_method.CallScreenControllerWithAuthority("/common/CMS160", { 'strContractCode': strContractCode, 'strOCC': OCC }, true);

                        });
                        BindGridButtonClickEvent("btnIncdAR", row_id, function (rid) {
                            mygrid.selectRow(mygrid.getRowIndex(rid));
                            if (mygrid.cells(rid, mygrid.getColIndexById('ContractType')).getValue() == C_CONTRACT_TYPE_INCIDENT) {
                                var IncidentID = mygrid.cells(rid, mygrid.getColIndexById('IncidentID')).getValue();
                                ajax_method.CallScreenControllerWithAuthority("/contract/CTS330", { 'strIncidentID': IncidentID }, true);
                            } else if (mygrid.cells(rid, mygrid.getColIndexById('ContractType')).getValue() == C_CONTRACT_TYPE_AR) {
                                var RequestNo = mygrid.cells(rid, mygrid.getColIndexById('RequestNo')).getValue();
                                ajax_method.CallScreenControllerWithAuthority("/contract/CTS380", { 'pRequestNo': RequestNo }, true);
                            }
                        });
                    }
                }

                mygrid.setSizes();
            });
            SpecialGridControl(mygrid, ["SaleBasic", "IncidentAR"]);
        }
    }
}

function initialRightButton() {

    $("#btnViewIncdList").click(function () {
        // New windos CTS320 : Incident list
       
        var obj = {
            strIncidentRelevantType: $("#C_INCIDENT_RELEVANT_TYPE_CONTRACT").val(),
            strIncidentRelevantCode: strContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS320", obj, true);
    });

    $("#btnViewARlist").click(function () {
        // New window CTS370 : AR list

        var obj = {
            strARRelevantType: $("#C_AR_RELEVANT_TYPE_CONTRACT").val(),
            strARRelevantCode: strContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS370", obj, true);
    });

    $("#btnRegIncd").click(function () {
        // New window CTS300 : Register new incident

        var obj = {
            strIncidentRelevantType: $("#C_INCIDENT_RELEVANT_TYPE_CONTRACT").val(),
            strIncidentRelevantCode: strContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS300", obj, true);
    });

    $("#btnRegAR").click(function () {

        // New window CTS350 : Register new AR

        var obj = {
            strARRelevantType: $("#C_AR_RELEVANT_TYPE_CONTRACT").val(),
            strARRelevantCode: strContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS350", obj, true);

    });
}
function Filtering() {

    specChange = [];
    specIncdAr = [];
    var numHiddenRow = 0;
    if ($("#SpecifyChgType").prop('checked') == true) {
        $("#selChangeType option").each(function (index, option) {
            specChange[index] = $(this).attr('value');
        });
        if (specChange.length < 1) {
            var obj = {
                module: "Common",
                code: "MSG0006"
            };
            call_ajax_method("/Shared/GetMessage", obj, function (res) {
                OpenWarningDialog(res.Message);

            });
            return false;
        }
    }

    if ($("#SpecifyIncdARType").prop('checked') == true) {
        $("#selIncdAR option").each(function (index) {
            specIncdAr[index] = $(this).attr('value');
        });
        if (specIncdAr.length < 1) {
            var obj = {
                module: "Common",
                code: "MSG0006"
            };
            call_ajax_method("/Shared/GetMessage", obj, function (res) {

                OpenWarningDialog(res.Message);
            });
            return false;
        }
    }

    //------------------------ Filter --------------------------------------------
    if ($("#SpecifyIncdARType").prop('checked') == true && $("#SpecifyChgType").prop('checked') == true) {
        mygrid.filterBy(mygrid.getColIndexById("ChangeIncidentARtype"), function (data) {
            return data.toString() != '' && (IsIn(specChange, data.toString()) || IsIn(specIncdAr, data.toString()));
        });

    } else if ($("#SpecifyChgType").prop('checked') == false && $("#SpecifyIncdARType").prop('checked') == true) {
        mygrid.filterBy(mygrid.getColIndexById("ChangeIncidentARtype"), function (data) {
            return data.toString() != '' && IsIn(specIncdAr, data.toString());
        });

    } else if ($("#SpecifyChgType").prop('checked') == true && $("#SpecifyIncdARType").prop('checked') == false) {
        mygrid.filterBy(mygrid.getColIndexById("ChangeIncidentARtype"), function (data) {
            return data.toString() != '' && IsIn(specChange, data.toString());
        });
    } else {
        mygrid.filterBy(1, function (data) { return true; });
    }
    //------------------------------ End filter -----------------------------------

    var idx = 0;

    var rowCount = mygrid.getRowsNum();

    for (var i = 0; i < rowCount; i++) {
        if (mygrid.getRowId(i) != undefined) {
            if (mygrid.getRowById(mygrid.getRowId(i)).style.display != "none") {
                if (i < RowPgs) {
                    mygrid.cells2(i, 0).setValue(idx + 1);
                }
                idx += 1;
            }
        }
    }

    var paging_info_name = "ResultList" + "_info_paging";
    var total = Math.max(1, Math.min(0, Math.ceil(idx / RowPgs)));
    $("#" + paging_info_name).html("Page 1 of " + total + " (" + idx + " items)");
    //Show error when no row has shown --------------------------------
    if (numHiddenRow == mygrid.getRowsNum()) {
        CreateEmptyRow(mygrid, false);
    }
}
function CMS220Object() {

    return {
        "ContractCode": strContractCode,
        "OCC": OCC,
        "ContractTargetCode": ContractTargetCode,
        "RealCustomerCode": RealCustCode,
        "SiteCode": SiteCode,
        "PurchaserCustCode": CustCodePur,
        "Mode": CMS150_CMS220ShowMode
    };
}
