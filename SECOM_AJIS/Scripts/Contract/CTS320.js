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
var ciGrid;
var ciOccGrid;
var siGrid;
var siOccGrid;
var ctiGrid;
var piGrid;

var ciGridInitial = false;
var ciOccGridInitial = false;
var siGridInitial = false;
var siOccGridInitial = false;
var ctiGridInitial = false;
var piGridInitial = false;

var defDueDate;
var defStatus;
var isFromParameter = false;
var isFromOwnSection = false;
//var pageRow = ROWS_PER_PAGE_FOR_SEARCHPAGE;
var pageRow = 5;
var isUserRequest = false;
var lastQueObj = null;

var validateCondition = ["txtCustomerCode", "txtSiteCode", "txtContractCode", "txtProjectCode"];

$(document).ready(function () {
    LoadInitialDefaultValue();

    InitialScreen();
    InitialEvent();

    //CheckingParameter();
})

function InitialScreen() {
    CloseWarningDialog();
    ClearSelectTextbox();
    DisableAllSelectTextbox();

    $('#radCustomer').attr("checked", true);
    radcustomer_click(true);

    HideAll();
    //SetScreenToDefault();
    InitialGrid();
}

function InitialEvent() {
    $('#radCustomer').click(function () {
        radcustomer_click(true);
    });
    $('#radSite').click(function () {
        radsite_click(true);
    });
    $('#radContract').click(function () {
        radcontract_click(true);
    });
    $('#radProject').click(function () {
        radproject_click(true);
    });

    $("#btnRetrieveSelectIncidentList").click(retrieveSelectIncident_click);
    $("#btnClearSelectIncidentList").click(clearSelectIncident_click);

    $('#btnCIRetrieve').click(retrieveCI_click);
    $('#btnSIRetrieve').click(retrieveSI_click);
    $('#btnSICustomerIncident').click(retrieveSICustomer_click);
    $('#btnCTIRetrieve').click(retrieveCTI_click);
    $('#btnCTICustomerIncident').click(retrieveCTICustomer_click);
    $('#btnCTISiteIncident').click(retrieveCTISite_click);
    $('#btnPIRetrieve').click(retrievePI_click);
}
// ---------------------------------------------------------------------------------
// Event
// ---------------------------------------------------------------------------------
function retrieveSelectIncident_click() {
    isUserRequest = true;

    var obj = {
        IncidentMode: "",
        CustomerCode: "",
        SiteCode: "",
        ContractCode: "",
        ProjectCode: "",
        IncidentType: "",
        DuedateDeadline: defDueDate,
        IncidentStatus: defStatus
    };

    if ($('#radCustomer')[0].checked) {
        obj.IncidentMode = "1";
        obj.CustomerCode = $('#txtCustomerCode').val();
    } else if ($('#radSite')[0].checked) {
        obj.IncidentMode = "2";
        obj.SiteCode = $('#txtSiteCode').val();
    } else if ($('#radContract')[0].checked) {
        obj.IncidentMode = "3";
        obj.ContractCode = $('#txtContractCode').val();
    } else if ($('#radProject')[0].checked) {
        obj.IncidentMode = "4";
        obj.ProjectCode = $('#txtProjectCode').val();
    }

    //$('#btnRetrieveSelectIncidentList').attr('disabled', 'disabled');
    DisableSearchSection();
    RetrieveIncidentData(obj, false);
}

function retrieveCI_click() {
    isUserRequest = true;

    var obj = {
        IncidentMode: "1",
        CustomerCode: $('#spCICode').text(),
        SiteCode: "",
        ContractCode: "",
        ProjectCode: "",
        IncidentType: $('#cbbCIIncidentType').val(),
        DuedateDeadline: $('#cbbCIDueDate').val(),
        IncidentStatus: $('#cbbCIStatus').val()
    };

    //SetFilterTo(obj);

    RetrieveIncidentData(obj, true);
}

function retrieveSI_click() {
    isUserRequest = true;

    var obj = {
        IncidentMode: "2",
        CustomerCode: "",
        SiteCode: $('#spSICode').text(),
        ContractCode: "",
        ProjectCode: "",
        IncidentType: $('#cbbSIIncidentType').val(),
        DuedateDeadline: $('#cbbSIDueDate').val(),
        IncidentStatus: $('#cbbSIStatus').val()
    };

    //SetFilterTo(obj);

    RetrieveIncidentData(obj, true);
}

function retrieveSICustomer_click() {
    isUserRequest = true;

    var obj = {
        SiteCode: $('#spSICode').text()
    };
    call_ajax_method_json("/Contract/CTS320_RetrieveCustomerCode", obj, function (result, controls) {
        if (controls != undefined) {
            // Hilight Text
            //VaridateCtrl(["EntryContractCode"], ["EntryContractCode"]);
        }

        if (result != null) {
            var obj = {
                IncidentMode: "1",
                CustomerCode: result,
                SiteCode: "",
                ContractCode: "",
                ProjectCode: "",
                IncidentType: "",
                DuedateDeadline: defDueDate,
                IncidentStatus: defStatus
            };

            //SetFilterTo(obj);

            RetrieveIncidentData(obj, false);
        }
    });
}

function retrieveCTI_click() {
    isUserRequest = true;

    var obj = {
        IncidentMode: "3",
        CustomerCode: "",
        SiteCode: "",
        ContractCode: $('#spCTICode').text(),
        ProjectCode: "",
        IncidentType: $('#cbbCTIIncidentType').val(),
        DuedateDeadline: $('#cbbCTIDueDate').val(),
        IncidentStatus: $('#cbbCTIStatus').val()
    };

    //SetFilterTo(obj);

    RetrieveIncidentData(obj, true);
}

function retrieveCTICustomer_click() {
    isUserRequest = true;

    var obj = {
        ContractCode: $('#spCTICode').text()
    };
    call_ajax_method_json("/Contract/CTS320_RetrieveCustomerCode", obj, function (result, controls) {
        if (controls != undefined) {
            // Hilight Text
            //VaridateCtrl(["EntryContractCode"], ["EntryContractCode"]);
        }

        if (result != null) {
            var obj = {
                IncidentMode: "1",
                CustomerCode: result,
                SiteCode: "",
                ContractCode: "",
                ProjectCode: "",
                IncidentType: "",
                DuedateDeadline: defDueDate,
                IncidentStatus: defStatus
            };

            //SetFilterTo(obj);

            RetrieveIncidentData(obj, false);
        }
    });
}

function retrieveCTISite_click() {
    isUserRequest = true;

    var obj = {
        ContractCode: $('#spCTICode').text()
    };
    call_ajax_method_json("/Contract/CTS320_RetrieveSiteCode", obj, function (result, controls) {
        if (controls != undefined) {
            // Hilight Text
            //VaridateCtrl(["EntryContractCode"], ["EntryContractCode"]);
        }

        if (result != null) {
            var obj = {
                IncidentMode: "2",
                CustomerCode: "",
                SiteCode: result,
                ContractCode: "",
                ProjectCode: "",
                IncidentType: "",
                DuedateDeadline: defDueDate,
                IncidentStatus: defStatus
            };

            //SetFilterTo(obj);

            RetrieveIncidentData(obj, false);
        }
    });
}

function retrievePI_click() {
    isUserRequest = true;

    var obj = {
        IncidentMode: "4",
        CustomerCode: "",
        SiteCode: "",
        ContractCode: "",
        ProjectCode: $('#spPICode').text(),
        IncidentType: $('#cbbPIIncidentType').val(),
        DuedateDeadline: $('#cbbPIDueDate').val(),
        IncidentStatus: $('#cbbPIStatus').val()
    };

    //SetFilterTo(obj);

    RetrieveIncidentData(obj, true);
}

function clearSelectIncident_click() {
    EnableSearchSection();
    SetScreenToDefault();
    VaridateCtrl(["txtSiteCode", "txtCustomerCode", "txtContractCode", "txtProjectCode"], null);
}

function radcustomer_click(isNeedHideAll) {
    ClearSelectTextbox();
    DisableAllSelectTextbox();
    CloseWarningDialog();

    if (isNeedHideAll) {
        HideAll();
        ShowSelectIncidentList();
    }

    $('#txtCustomerCode').removeAttr("readonly");
    VaridateCtrl(["txtSiteCode", "txtCustomerCode", "txtContractCode", "txtProjectCode"], null);
}

function radsite_click(isNeedHideAll) {
    ClearSelectTextbox();
    DisableAllSelectTextbox();
    CloseWarningDialog();

    if (isNeedHideAll) {
        HideAll();
        ShowSelectIncidentList();
    }

    $('#txtSiteCode').removeAttr("readonly");
    VaridateCtrl(["txtSiteCode", "txtCustomerCode", "txtContractCode", "txtProjectCode"], null);
}

function radcontract_click(isNeedHideAll) {
    ClearSelectTextbox();
    DisableAllSelectTextbox();
    CloseWarningDialog();

    if (isNeedHideAll) {
        HideAll();
        ShowSelectIncidentList();
    }

    $('#txtContractCode').removeAttr("readonly");
    VaridateCtrl(["txtSiteCode", "txtCustomerCode", "txtContractCode", "txtProjectCode"], null);
}

function radproject_click(isNeedHideAll) {
    ClearSelectTextbox();
    DisableAllSelectTextbox();
    CloseWarningDialog();

    if (isNeedHideAll) {
        HideAll();
        ShowSelectIncidentList();
    }

    $('#txtProjectCode').removeAttr("readonly");
    VaridateCtrl(["txtSiteCode", "txtCustomerCode", "txtContractCode", "txtProjectCode"], null);
}

function incidentGridBinding(grid, gridname, zonename, gen_ctrl) {
    var idxIncidentStatusCol = grid.getColIndexById('IncidentStatus');
    var idxIncidentFlagCol = grid.getColIndexById('IncidentFlag');
    var idxIncidentIDCol = grid.getColIndexById('IncidentID');
    var idxIncidentNoCol = grid.getColIndexById('IncidentNo');
    var idxHeadCodeCol = grid.getColIndexById('HeadCode');
    var idxHeadNameCol = grid.getColIndexById('HeadName');

    if (isUserRequest) {
        if (grid.getRowsNum() != 0) {
//            if ((zonename == "CI")) {
//                ShowCustomerIncident();
//            } else if ((zonename == "SI")) {
//                ShowSiteIncident();
//            } else if ((zonename == "CTI")) {
//                ShowContractIncident();
//            } else if ((zonename == "PI")) {
//                ShowProjectIncident();
//            }

            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);

                //grid.setRowTextStyle(row_id, "word-wrap: break-word;");

//                if (i == 0) {
//                    var headCode = grid.cells(row_id, idxHeadCodeCol).getValue();
//                    var headName = grid.cells(row_id, idxHeadNameCol).getValue();

//                    if ((headCode.length > 0) && (headName.length > 0)) {
//                        $("#sp" + zonename + "Code").text(headCode);
//                        $("#sp" + zonename + "Name").text(headName);
//                    }
//                }

                //            grid.setRowColor(row_id, "white");
                //            grid.setCellTextStyle(row_id, idxIncidentStatusCol, "");
                //            grid.setCellTextStyle(row_id, idxIncidentFlagCol, "");
                grid.setRowColor(row_id, "#ffffff");
                // Check New/Over and set color
                if ((grid.cells(row_id, idxIncidentStatusCol).getValue() != null) && (grid.cells(row_id, idxIncidentStatusCol).getValue().indexOf('ver') != -1)) {
                    grid.setRowColor(row_id, "#ff9999");
                    grid.cells(row_id, idxIncidentStatusCol).setValue(_markerOver);
                    grid.setCellTextStyle(row_id, idxIncidentStatusCol, "background-color: red; color: white; font-weight: bold;");
                } else if ((grid.cells(row_id, idxIncidentStatusCol).getValue() != null) && (grid.cells(row_id, idxIncidentStatusCol).getValue().indexOf('ew') != -1)) {
                    grid.setCellTextStyle(row_id, idxIncidentStatusCol, "background-color: yellow; color: red; font-weight: bold;");
                    grid.cells(row_id, idxIncidentStatusCol).setValue(_markerNew);
                }

                if ((grid.cells(row_id, idxIncidentFlagCol).getValue() != null) && (grid.cells(row_id, idxIncidentFlagCol).getValue().length > 0)) {
                    grid.setCellTextStyle(row_id, idxIncidentFlagCol, "background-color: red; color: white; font-weight: bold;");
                    grid.cells(row_id, idxIncidentFlagCol).setValue(_markerStar)
                }

                if (gen_ctrl) {
                    var incidentID = grid.cells(row_id, idxIncidentIDCol).getValue();
                    var incidentNo = grid.cells(row_id, idxIncidentNoCol).getValue();

                    var tagA = "<a href='#'>" + incidentNo + "<input type='hidden' name='incidentID' value='" + incidentID + "'/></a>";
                    grid.cells(row_id, idxIncidentNoCol).setValue(tagA);
                }
            }

            var gridA = $(gridname + " a");

            gridA.each(function () {
                $(this).unbind('click');

                $(this).click(function () {
                    var obj = {
                        strIncidentID: $(this).children("input:hidden[name=incidentID]").val()
                    };
                    ajax_method.CallScreenControllerWithAuthority("/Contract/CTS330", obj, true);
                });
            });
        }
    }
}

function occcurringGridBinding(grid, gridname, codeColName, gridtype, gen_ctrl) {
    var idxCodeCol = grid.getColIndexById(codeColName);
    if (grid.getRowsNum() != 0) {
        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);
            var code = grid.cells(row_id, idxCodeCol).getValue();

            if (gen_ctrl) {
                var tagA = "<a href='#'>" + code + "<input type='hidden' name='" + codeColName + "' value='" + code + "'/></a>";
                grid.cells(row_id, idxCodeCol).setValue(tagA);
            }
        }

        var gridA = $(gridname + " a");

        gridA.each(function () {
            $(this).unbind('click');

            $(this).click(function () {
//                var obj = {
//                    IncidentMode: "",
//                    CustomerCode: "",
//                    SiteCode: "",
//                    ContractCode: "",
//                    ProjectCode: "",
//                    IncidentType: "",
//                    DuedateDeadline: defDueDate,
//                    IncidentStatus: defStatus
                //                };

                var obj = {
                    strIncidentRelevantType: "",
                    strIncidentRelevantCode: ""
                };

                if (gridtype == "1") {
                    obj.strIncidentRelevantType = _incType_Site;
                    obj.strIncidentRelevantCode = $(this).children("input:hidden[name=" + codeColName + "]").val();
                } else {
                    obj.strIncidentRelevantType = _incType_Contract;
                    obj.strIncidentRelevantCode = $(this).children("input:hidden[name=" + codeColName + "]").val();
                }

//                if (gridtype == "1") {
//                    obj.IncidentMode = "2";
//                    obj.SiteCode = $(this).children("input:hidden[name=" + codeColName + "]").val();
//                } else {
//                    obj.IncidentMode = "3";
//                    obj.ContractCode = $(this).children("input:hidden[name=" + codeColName + "]").val();
//                }

                //SetFilterTo(obj);

                //RetrieveIncidentData(obj, false);
                
                ajax_method.CallScreenControllerWithAuthority("/Contract/CTS320", obj, true);
            });
        });
    }
}

function gridStyleChecking() {
}

// ---------------------------------------------------------------------------------
// Function
// ---------------------------------------------------------------------------------
function CheckingParameter() {
    if (ciGrid && ciOccGrid && siGrid && siOccGrid && ctiGrid && piGrid) {
        call_ajax_method_json("/Contract/CTS320_CheckParameter", "", function (result, controls) {
            if (result != null) {
                isFromParameter = true;
                isUserRequest = true;
                RetrieveIncidentData(result, false);
            } else {
                ShowSelectIncidentList();
            }
        });
    }
}

function LoadInitialDefaultValue() {
    call_ajax_method_json("/Contract/CTS320_RetrieveInitialDefaultValue", "", function (result, controls) {
        defDueDate = result.DueDate;
        defStatus = result.Status;
    });
}

function RetrieveIncidentData(obj, fromOwnSection) {
    // Validate Condition
    var isValid = false;
    isFromOwnSection = fromOwnSection;
    lastQueObj = obj;
    var HeaderObj = null;

    call_ajax_method_json("/Contract/CTS320_RetrieveResultGridHeader", lastQueObj, function (result, controls) {
        if (controls != null) {
            VaridateCtrl(["txtCustomerCode", "txtSiteCode", "txtContractCode", "txtProjectCode"], controls);
        }
        if (result != null) {
            HeaderObj = result;

            if (obj.IncidentMode == "1") {
                if ((obj.CustomerCode != null) && (obj.CustomerCode.length > 0))
                    isValid = true;

                if (isValid && fromOwnSection) {
                    //SetScreenToDefault();
                    SetFilterWithDisableTo(obj);
                    //ShowCustomerIncident(!fromOwnSection);
                } else if (!isValid) {
                    SetFilterTo(obj);
                }

                //================= Catch Error =====================
                //                call_ajax_method_json("/Contract/CTS320_RetireveIncidentData", null, function (result, controls) {
                //                });
                //===================================================

                $('#gridCIMain').LoadDataToGrid(ciGrid, pageRow, true, "/Contract/CTS320_RetireveIncidentData", obj, "CTS320_IncidentGridResult", false,
                 function (result, controls, isWarning) {
                     if (isWarning != null) {
                         VaridateCtrl(validateCondition, ["txtCustomerCode"]);
                     }
                 },
                function (result, controls, isWarning) {
                    if (isWarning != null) {
                        VaridateCtrl(validateCondition, ["txtCustomerCode"]);
                    }

                    $("#spCICode").text(HeaderObj.Code);
                    $("#spCIName").text(HeaderObj.Name);

                    ShowCustomerIncident();
                    $('#gridCISub').LoadDataToGrid(ciOccGrid, pageRow, true, "/Contract/CTS320_RetireveOccurringData", obj, "CTS320_IncidentOccurringSiteGridResult", false, null,
                    function (result, controls, isWarning) {
                        ShowCustomerIncident();

                        //=============== TRS 04/05/2012 =====================
                        $('#txtCustomerCode').val("");
                        $('#txtSiteCode').val("");
                        $('#txtContractCode').val("");
                        $('#txtProjectCode').val("");

                        //                        $('#txtCustomerCode').attr("readonly", true);
                        //                        $('#txtSiteCode').attr("readonly", true);
                        //                        $('#txtContractCode').attr("readonly", true);
                        //                        $('#txtProjectCode').attr("readonly", true);

                        //                        $('#radCustomer').attr("checked", true);
                        //                        $('#txtCustomerCode').attr("readonly", false);
                        $('#txtCustomerCode').val(obj.CustomerCode);
                        //====================================================
                    });
                });
            } else if (obj.IncidentMode == "2") {
                if ((obj.SiteCode != null) && (obj.SiteCode.length > 0))
                    isValid = true;

                if (isValid && fromOwnSection) {
                    //SetScreenToDefault();
                    SetFilterWithDisableTo(obj);
                    //ShowSiteIncident(!fromOwnSection);
                } else if (!isValid) {
                    SetFilterTo(obj);
                }

                $('#gridSIMain').LoadDataToGrid(siGrid, pageRow, true, "/Contract/CTS320_RetireveIncidentData", obj, "CTS320_IncidentGridResult", false,
                function (result, controls, isWarning) {
                    if (isWarning != null) {
                        VaridateCtrl(validateCondition, ["txtSiteCode"]);
                    }
                },
                function (result, controls, isWarning) {
                    if (isWarning != null) {
                        VaridateCtrl(validateCondition, ["txtSiteCode"]);
                    }

                    $("#spSICode").text(HeaderObj.Code);
                    $("#spSIName").text(HeaderObj.Name);

                    ShowSiteIncident();
                    $('#gridSISub').LoadDataToGrid(siOccGrid, pageRow, true, "/Contract/CTS320_RetireveOccurringData", obj, "CTS320_IncidentOccurringContractGridResult", false, null,
                    function (result, controls, isWarning) {


                        ShowSiteIncident();

                        //=============== TRS 04/05/2012 =====================
                        $('#txtCustomerCode').val("");
                        $('#txtSiteCode').val("");
                        $('#txtContractCode').val("");
                        $('#txtProjectCode').val("");

                        //                        $('#txtCustomerCode').attr("readonly", true);
                        //                        $('#txtSiteCode').attr("readonly", true);
                        //                        $('#txtContractCode').attr("readonly", true);
                        //                        $('#txtProjectCode').attr("readonly", true);

                        //                        $('#radSite').attr("checked", true);
                        //                        $('#txtSiteCode').attr("readonly", false);
                        $('#txtSiteCode').val(obj.SiteCode);
                        //====================================================
                    });

                    //                    if ((siGrid.getRowsNum() == 0) || ((siGrid.getRowsNum() > 0) && (CheckFirstRowIsEmpty(siGrid)))) {
                    //                        $('#btnSICustomerIncident').removeAttr('enable');
                    //                    } else {
                    //                        $('#btnSICustomerIncident').attr('enable', 'enable');
                    //                    }
                });
            } else if (obj.IncidentMode == "3") {
                if ((obj.ContractCode != null) && (obj.ContractCode.length > 0))
                    isValid = true;

                if (isValid && fromOwnSection) {
                    //SetScreenToDefault();
                    SetFilterWithDisableTo(obj);
                    //ShowContractIncident(!fromOwnSection);
                } else if (!isValid) {
                    SetFilterTo(obj);
                }

                $('#gridCTIMain').LoadDataToGrid(ctiGrid, pageRow, true, "/Contract/CTS320_RetireveIncidentData", obj, "CTS320_IncidentGridResult", false,
                function (result, controls, isWarning) {
                    if (isWarning != null) {
                        VaridateCtrl(validateCondition, ["txtContractCode"]);
                    }
                },
                function (result, controls, isWarning) {
                    if (isWarning != null) {
                        VaridateCtrl(validateCondition, ["txtContractCode"]);
                    }

                    $("#spCTICode").text(HeaderObj.Code);
                    $("#spCTIName").text(HeaderObj.Name);

                    ShowContractIncident();

                    //=============== TRS 04/05/2012 =====================
                    $('#txtCustomerCode').val("");
                    $('#txtSiteCode').val("");
                    $('#txtContractCode').val("");
                    $('#txtProjectCode').val("");

                    //                    $('#txtCustomerCode').attr("readonly", true);
                    //                    $('#txtSiteCode').attr("readonly", true);
                    //                    $('#txtContractCode').attr("readonly", true);
                    //                    $('#txtProjectCode').attr("readonly", true);

                    //                    $('#radContract').attr("checked", true);
                    //                    $('#txtContractCode').attr("readonly", false);
                    $('#txtContractCode').val(obj.ContractCode);
                    //====================================================

                    //                    if ((ctiGrid.getRowsNum() == 0) || ((ctiGrid.getRowsNum() > 0) && (CheckFirstRowIsEmpty(siGrid)))) {
                    //                        $('#btnCTICustomerIncident').removeAttr('enable');
                    //                        $('#btnCTISiteIncident').removeAttr('enable');
                    //                    } else {
                    //                        $('#btnCTICustomerIncident').attr('enable', 'enable');
                    //                        $('#btnCTISiteIncident').attr('enable', 'enable');
                    //                    }
                });
            } else if (obj.IncidentMode == "4") {
                if ((obj.ProjectCode != null) && (obj.ProjectCode.length > 0))
                    isValid = true;

                if (isValid && fromOwnSection) {
                    //SetScreenToDefault();
                    SetFilterWithDisableTo(obj);
                    //ShowProjectIncident(!fromOwnSection);
                } else if (!isValid) {
                    SetFilterTo(obj);
                }

                $('#gridPIMain').LoadDataToGrid(piGrid, pageRow, true, "/Contract/CTS320_RetireveIncidentData", obj, "CTS320_IncidentGridResult", false,
                function (result, controls, isWarning) {
                    if (isWarning != null) {
                        VaridateCtrl(validateCondition, ["txtProjectCode"]);
                    }
                },
                function (result, controls, isWarning) {
                    if (isWarning != null) {
                        VaridateCtrl(validateCondition, ["txtProjectCode"]);
                    }

                    $("#spPICode").text(HeaderObj.Code);
                    $("#spPIName").text(HeaderObj.Name);

                    ShowProjectIncident();

                    //=============== TRS 04/05/2012 =====================
                    $('#txtCustomerCode').val("");
                    $('#txtSiteCode').val("");
                    $('#txtContractCode').val("");
                    $('#txtProjectCode').val("");

                    //                    $('#txtCustomerCode').attr("readonly", true);
                    //                    $('#txtSiteCode').attr("readonly", true);
                    //                    $('#txtContractCode').attr("readonly", true);
                    //                    $('#txtProjectCode').attr("readonly", true);

                    //                    $('#radProject').attr("checked", true);
                    //                    $('#txtProjectCode').attr("readonly", false);
                    $('#txtProjectCode').val(obj.ProjectCode);
                    //====================================================
                });
            }
        } else {
            SetFilterTo(obj);
        }
    });
}

function DisableSearchSection() {
    $('#btnRetrieveSelectIncidentList').attr('disabled', 'disabled');
    $('#radCustomer').attr('disabled', 'disabled');
    $('#radSite').attr('disabled', 'disabled');
    $('#radContract').attr('disabled', 'disabled');
    $('#radProject').attr('disabled', 'disabled');

    $('#txtCustomerCode').attr('readonly', 'readonly');
    $('#txtSiteCode').attr('readonly', 'readonly');
    $('#txtContractCode').attr('readonly', 'readonly');
    $('#txtProjectCode').attr('readonly', 'readonly');
}

function EnableSearchSection() {
    $('#btnRetrieveSelectIncidentList').removeAttr('disabled');
    $('#radCustomer').removeAttr('disabled');
    $('#radSite').removeAttr('disabled');
    $('#radContract').removeAttr('disabled');
    $('#radProject').removeAttr('disabled');

    $('#txtCustomerCode').removeAttr('readonly');
    $('#txtSiteCode').removeAttr('readonly');
    $('#txtContractCode').removeAttr('readonly');
    $('#txtProjectCode').removeAttr('readonly');
}

function SetScreenToDefault() {
    CloseWarningDialog();
    ClearSelectTextbox();
    DisableAllSelectTextbox();

    $('#radCustomer').attr("checked", true);
    radcustomer_click(true);

    HideAll();
    ShowSelectIncidentList();
}

function SetFilterWithDisableTo(obj) {
    SetFilterTo(obj);
    DisableSearchSection();
}

function SetFilterTo(obj) {
    EnableSearchSection();
    ClearSelectTextbox();

    if (obj.IncidentMode == "1") {
        $('#radCustomer').attr("checked", true);
        radcustomer_click(false);
        $('#txtCustomerCode').val(obj.CustomerCode);
    } else if (obj.IncidentMode == "2") {
        $('#radSite').attr("checked", true);
        radsite_click(false);
        $('#txtSiteCode').val(obj.SiteCode);
    } else if (obj.IncidentMode == "3") {
        $('#radContract').attr("checked", true);
        radcontract_click(false);
        $('#txtContractCode').val(obj.ContractCode);
    } else if (obj.IncidentMode == "4") {
        $('#radProject').attr("checked", true);
        radproject_click(false);
        $('#txtProjectCode').val(obj.ProjectCode);
    }
}

function InitialGrid() {
    ciGrid = $("#gridCIMain").InitialGrid(pageRow, true, "/Contract/CTS320_InitialGridIncident", function () {
        BindOnLoadedEvent(ciGrid, function (gen_ctrl) {
            incidentGridBinding(ciGrid, "#gridCIMain", "CI", gen_ctrl);
            ciGrid.setSizes();
        });

        ciGridInitial = true;
        CheckingParameter();
    });
    ciOccGrid = $("#gridCISub").InitialGrid(pageRow, true, "/Contract/CTS320_InitialGridOccurringSite", function () {
        BindOnLoadedEvent(ciOccGrid, function (gen_ctrl) {
            occcurringGridBinding(ciOccGrid, "#gridCISub", "SiteCode", "1", gen_ctrl);
            ciOccGrid.setSizes();
        });

        ciOccGridInitial = true;
        CheckingParameter();
    });

    siGrid = $("#gridSIMain").InitialGrid(pageRow, true, "/Contract/CTS320_InitialGridIncident", function () {
        BindOnLoadedEvent(siGrid, function (gen_ctrl) {
            incidentGridBinding(siGrid, "#gridSIMain", "SI", gen_ctrl);
            siGrid.setSizes();
        });

        siGridInitial = true;
        CheckingParameter();
    });
    siOccGrid = $("#gridSISub").InitialGrid(pageRow, true, "/Contract/CTS320_InitialGridOccurringContract", function () {
        BindOnLoadedEvent(siOccGrid, function (gen_ctrl) {
            occcurringGridBinding(siOccGrid, "#gridSISub", "ContractCode", "2", gen_ctrl);
            siOccGrid.setSizes();
        });

        siOccGridInitial = true;
        CheckingParameter();
    });

    ctiGrid = $("#gridCTIMain").InitialGrid(pageRow, true, "/Contract/CTS320_InitialGridIncident", function () {
        BindOnLoadedEvent(ctiGrid, function (gen_ctrl) {
            incidentGridBinding(ctiGrid, "#gridCTIMain", "CTI", gen_ctrl);
            ctiGrid.setSizes();
        });

        ctiGridInitial = true;
        CheckingParameter();
    });

    piGrid = $("#gridPIMain").InitialGrid(pageRow, true, "/Contract/CTS320_InitialGridIncident", function () {
        BindOnLoadedEvent(piGrid, function (gen_ctrl) {
            incidentGridBinding(piGrid, "#gridPIMain", "PI", gen_ctrl);
            piGrid.setSizes();
        });

        piGridInitial = true;
        CheckingParameter();
    });
}

function ClearSelectTextbox() {
    $('#txtCustomerCode').val('');
    $('#txtSiteCode').val('');
    $('#txtContractCode').val('');
    $('#txtProjectCode').val('');
}

function DisableAllSelectTextbox() {
    $("#txtCustomerCode").attr("readonly", true);
    $("#txtSiteCode").attr("readonly", true);
    $("#txtContractCode").attr("readonly", true);
    $("#txtProjectCode").attr("readonly", true);
}

function ShowSelectIncidentList() {
    if (!isFromParameter) {
        $('#divSelectIncidentListFields').show();
    }
}

function ShowCustomerIncident() {
    HideAll();
    ShowSelectIncidentList();
    $('#divCustomerIncident').show();

    if (!isFromOwnSection) {
        $('#cbbCIIncidentType').val("");
        $('#cbbCIDueDate').val(defDueDate);
        $('#cbbCIStatus').val(defStatus);
    }
}

function ShowSiteIncident() {
    HideAll();
    ShowSelectIncidentList();
    $('#divSiteIncident').show();

    if (!isFromOwnSection) {
        $('#cbbSIIncidentType').val("");
        $('#cbbSIDueDate').val(defDueDate);
        $('#cbbSIStatus').val(defStatus);
    }
}

function ShowContractIncident() {
    HideAll();
    ShowSelectIncidentList();
    $('#divContractIncident').show();

    if (!isFromOwnSection) {
        $('#cbbCTIIncidentType').val("");
        $('#cbbCTIDueDate').val(defDueDate);
        $('#cbbCTIStatus').val(defStatus);
    }
}

function ShowProjectIncident() {
    HideAll();
    ShowSelectIncidentList();
    $('#divProjectIncident').show();

    if (!isFromOwnSection) {
        $('#cbbPIIncidentType').val("");
        $('#cbbPIDueDate').val(defDueDate);
        $('#cbbPIStatus').val(defStatus);
    }
}

function HideSelectIncidentList() {
    $('#divSelectIncidentListFields').hide();
}

function HideCustomerIncident() {
    $('#divCustomerIncident').hide();
}

function HideSiteIncident() {
    $('#divSiteIncident').hide();
}

function HideContractIncident() {
    $('#divContractIncident').hide();
}

function HideProjectIncident() {
    $('#divProjectIncident').hide();
}

function HideAll() {
    HideSelectIncidentList();
    HideContractIncident();
    HideCustomerIncident();
    HideProjectIncident();
    HideSiteIncident();
}

//function LoadGridDataFromXML(grid, gridname, xmlstr, obj_name) {
//    var name = gridname;
//    var paging_name = name + "_paging";
//    var paging_info_name = name + "_info_paging";

//    grid.clearAll();
//    grid.xml.top = "response";
//    grid.xml.row = "./rows/" + obj_name;
//    grid.parse(xmlstr, "xmlB");

//    if (grid.getRowsNum() == 0) {
//        CreateEmptyRow(grid, show_error);
//        $("#" + paging_name).hide();
//        $("#" + paging_info_name).hide();
//    } else {
//        SetPagingSection(gridname, grid, pageRow, true);
//    }

//    grid.setSizes();
//}
// ---------------------------------------------------------------------------------