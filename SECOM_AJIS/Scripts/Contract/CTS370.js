// ---------------------------------------------------------------------------------
// Initial
// ---------------------------------------------------------------------------------

var carGrid;
var carOccGrid;
var sarGrid;
var sarOccGrid;
var ctarGrid;
var parGrid;
var qarGrid;

var isCarGridInitial = false;
var isCarOccGridInitial = false;
var isSarGridInitial = false;
var isSarOccGridInitial = false;
var isCtarGridInitial = false;
var isParGridInitial = false;
var isQarGridInitial = false;

var defDueDate;
var defStatus;
var isFromParameter = false;
var isFromOwnSection = false;
//var pageRow = ROWS_PER_PAGE_FOR_SEARCHPAGE;
var pageRow = 5;
var isUserRequest = false;
var isGridBinding = false;
var isSubGridBinding = false;

var validateCriteria = ["txtCustomerCode", "txtSiteCode", "txtContractCode", "txtProjectCode", "txtQuotationCode"];

$(document).ready(function () {
    //$('#gridCARMain').hide();
    $('#divSelectARListFields').hide();
    LoadInitialDefaultValue();

    InitialScreen();
    InitialEvent();
})

function InitialScreen() {
    //SetScreenToDefault();
    CloseWarningDialog();
    ClearSelectTextbox();
    DisableAllSelectTextbox();

    $('#radCustomer').attr("checked", true);
    //radcustomer_click();    
    DisableAllSelectTextbox();
    $('#txtCustomerCode').removeAttr("readonly");

    HideAll();
    //ShowSelectARList();
    InitialGrid();
}

function InitialEvent() {
    $('#radCustomer').click(radcustomer_click);
    $('#radSite').click(radsite_click);
    $('#radContract').click(radcontract_click);
    $('#radProject').click(radproject_click);
    $('#radQuotation').click(radquotation_click);

    $("#btnRetrieveSelectARList").click(retrieveSelectAR_click);
    $("#btnClearSelectARList").click(clearSelectAR_click);

    $('#btnCARRetrieve').click(retrieveCAR_click);
    $('#btnSARRetrieve').click(retrieveSAR_click);
    $('#btnSARCustomerAR').click(retrieveSARCustomer_click);
    $('#btnCTARRetrieve').click(retrieveCTAR_click);
    $('#btnCTARCustomerAR').click(retrieveCTARCustomer_click);
    $('#btnCTARSiteAR').click(retrieveCTARSite_click);
    $('#btnPARRetrieve').click(retrievePAR_click);
    $('#btnQARRetrieve').click(retrieveQAR_click);

    BindOnLoadedEvent(carGrid, function (gen_ctrl) {
        arGridBinding(carGrid, "#gridCARMain", "CAR", gen_ctrl);
        carGrid.setSizes();
    });

    BindOnLoadedEvent(sarGrid, function (gen_ctrl) {
        arGridBinding(sarGrid, "#gridSARMain", "SAR", gen_ctrl);
        sarGrid.setSizes();
    });

    BindOnLoadedEvent(ctarGrid, function (gen_ctrl) {
        arGridBinding(ctarGrid, "#gridCTARMain", "CTAR", gen_ctrl);
        ctarGrid.setSizes();
    });

    BindOnLoadedEvent(parGrid, function (gen_ctrl) {
        arGridBinding(parGrid, "#gridPARMain", "PAR", gen_ctrl);
        parGrid.setSizes();
    });

    BindOnLoadedEvent(qarGrid, function (gen_ctrl) {
        arGridBinding(qarGrid, "#gridQARMain", "QAR", gen_ctrl);
        qarGrid.setSizes();
    });

    BindOnLoadedEvent(carOccGrid, function (gen_ctrl) {
        occcurringGridBinding(carOccGrid, "#gridCARSub", "SiteCode", "1", gen_ctrl);
        carOccGrid.setSizes();
    });

    BindOnLoadedEvent(sarOccGrid, function (gen_ctrl) {
        occcurringGridBinding(sarOccGrid, "#gridSARSub", "ContractCode", "2", gen_ctrl);
        sarOccGrid.setSizes();
    });
}

// ---------------------------------------------------------------------------------
// Event
// ---------------------------------------------------------------------------------

function retrieveSelectAR_click() {
    isUserRequest = true;
    isGirdBinding = false;
    isSubGridBinding = false;

    var obj = {
        ARMode: "",
        CustomerCode: "",
        SiteCode: "",
        ContractCode: "",
        ProjectCode: "",
        QuotationCode: "",
        ARType: "",
        //DuedateDeadline: defDueDate,
        DuedateDeadline: null,
        ARStatus: defStatus
    };

    if ($('#radCustomer')[0].checked) {
        obj.ARMode = _arType_Customer;
        obj.CustomerCode = $('#txtCustomerCode').val();
    } else if ($('#radSite')[0].checked) {
        obj.ARMode = _arType_Site;
        obj.SiteCode = $('#txtSiteCode').val();
    } else if ($('#radContract')[0].checked) {
        obj.ARMode = _arType_Contract;
        obj.ContractCode = $('#txtContractCode').val();
    } else if ($('#radProject')[0].checked) {
        obj.ARMode = _arType_Project;
        obj.ProjectCode = $('#txtProjectCode').val();
    } else if ($('#radQuotation')[0].checked) {
        obj.ARMode = _arType_Quotation;
        obj.QuotationCode = $('#txtQuotationCode').val();
    }

//    call_ajax_method_json("/Contract/CTS370_RetrieveARResult_NEW", obj, function (result, controls) {
//        if (controls != undefined) {
//            // Hilight Text
//            //VaridateCtrl(["EntryContractCode"], ["EntryContractCode"]);
//        }

//        if ((result != null) && (result.SearchResult)) {
//            var sObj = {
//            sName: result.SID,
//        };

//        ShowCustomerAR();

//        var aaagrid = $("#gridCARMain").LoadDataToGridWithInitial(pageRow, pageRow, true, "/Contract/CTS370_RetrieveARData_NEW", sObj, "CTS370_ARGridResult", false);
//        
//        
//        BindOnLoadedEvent(aaagrid, function () {
//            //$('#gridCARMain').show();
//            //arGridBinding(aaagrid, "#gridCARMain", "CAR");
//            //ShowCustomerAR();
//        });
//    }
//});
    //$('#btnRetrieveSelectARList').attr('disabled', 'disabled');
    DisableSearchSection();

    RetrieveARData(obj, false);

    $('#cbbCARType').val("");
    //$('#cbbCARDueDate').val(defDueDate);
    $('#cbbCARStatus').val(defStatus);
}

function retrieveCAR_click() {
    isUserRequest = true;
    isGirdBinding = false;
    isSubGridBinding = false;

    var obj = {
        ARMode: _arType_Customer,
        CustomerCode: $('#spCARCode').text(),
        SiteCode: "",
        ContractCode: "",
        ProjectCode: "",
        QuotationCode: "",
        ARType: $('#cbbCARType').val(),
        //DuedateDeadline: $('#cbbCARDueDate').val(),
        DuedateDeadline: null,
        ARStatus: $('#cbbCARStatus').val()
    };

    //SetFilterTo(obj);

    RetrieveARData(obj, true);
}

function retrieveSAR_click() {
    isUserRequest = true;
    isGirdBinding = false;
    isSubGridBinding = false;

    var obj = {
        ARMode: _arType_Site,
        CustomerCode: "",
        SiteCode: $('#spSARCode').text(),
        ContractCode: "",
        ProjectCode: "",
        QuotationCode: "",
        ARType: $('#cbbSARType').val(),
        //DuedateDeadline: $('#cbbSARDueDate').val(),
        DuedateDeadline: null,
        ARStatus: $('#cbbSARStatus').val()
    };

    //SetFilterTo(obj);

    RetrieveARData(obj, true);
}

function retrieveSARCustomer_click() {
    isUserRequest = true;
    isGirdBinding = false;
    isSubGridBinding = false;

    var obj = {
        SiteCode: $('#spSARCode').text()
    };
    call_ajax_method_json("/Contract/CTS370_RetrieveCustomerCode", obj, function (result, controls) {
        if (controls != undefined) {
            // Hilight Text
            //VaridateCtrl(["EntryContractCode"], ["EntryContractCode"]);
        }

        if (result != null) {
            var obj = {
                ARMode: _arType_Customer,
                CustomerCode: result,
                SiteCode: "",
                ContractCode: "",
                ProjectCode: "",
                QuotationCode: "",
                ARType: "",
                //DuedateDeadline: defDueDate,
                DuedateDeadline: null,
                ARStatus: defStatus
            };

            //SetFilterTo(obj);

            RetrieveARData(obj, false);
        }
    });
}

function retrieveCTAR_click() {
    isUserRequest = true;
    isGirdBinding = false;
    isSubGridBinding = false;

    var obj = {
        ARMode: _arType_Contract,
        CustomerCode: "",
        SiteCode: "",
        ContractCode: $('#spCTARCode').text(),
        ProjectCode: "",
        QuotationCode: "",
        ARType: $('#cbbCTARType').val(),
        //DuedateDeadline: $('#cbbCTARDueDate').val(),
        DuedateDeadline: null,
        ARStatus: $('#cbbCTARStatus').val()
    };

    //SetFilterTo(obj);

    RetrieveARData(obj, true);
}

function retrieveCTARCustomer_click() {
    isUserRequest = true;
    isGirdBinding = false;
    isSubGridBinding = false;

    var obj = {
        ContractCode: $('#spCTARCode').text()
    };
    call_ajax_method_json("/Contract/CTS370_RetrieveCustomerCode", obj, function (result, controls) {
        if (controls != undefined) {
            // Hilight Text
            //VaridateCtrl(["EntryContractCode"], ["EntryContractCode"]);
        }

        if (result != null) {
            var obj = {
                ARMode: _arType_Customer,
                CustomerCode: result,
                SiteCode: "",
                ContractCode: "",
                ProjectCode: "",
                QuotationCode: "",
                ARType: "",
                //DuedateDeadline: defDueDate,
                DuedateDeadline: null,
                ARStatus: defStatus
            };

            //SetFilterTo(obj);

            RetrieveARData(obj, false);
        }
    });
}

function retrieveCTARSite_click() {
    isUserRequest = true;
    isGirdBinding = false;
    isSubGridBinding = false;

    var obj = {
        ContractCode: $('#spCTARCode').text()
    };
    call_ajax_method_json("/Contract/CTS370_RetrieveSiteCode", obj, function (result, controls) {
        if (controls != undefined) {
            // Hilight Text
            //VaridateCtrl(["EntryContractCode"], ["EntryContractCode"]);
        }

        if (result != null) {
            var obj = {
                ARMode: _arType_Site,
                CustomerCode: "",
                SiteCode: result,
                ContractCode: "",
                ProjectCode: "",
                QuotationCode: "",
                ARType: "",
                //DuedateDeadline: defDueDate,
                DuedateDeadline: null,
                ARStatus: defStatus
            };

            //SetFilterTo(obj);

            RetrieveARData(obj, false);
        }
    });
}

function retrievePAR_click() {
    isUserRequest = true;
    isGirdBinding = false;
    isSubGridBinding = false;

    var obj = {
        ARMode: _arType_Project,
        CustomerCode: "",
        SiteCode: "",
        ContractCode: "",
        ProjectCode: $('#spPARCode').text(),
        QuotationCode: "",
        ARType: $('#cbbPARType').val(),
        //DuedateDeadline: $('#cbbPARDueDate').val(),
        DuedateDeadline: null,
        ARStatus: $('#cbbPARStatus').val()
    };

    //SetFilterTo(obj);

    RetrieveARData(obj, true);
}

function retrieveQAR_click() {
    isUserRequest = true;
    isGirdBinding = false;
    isSubGridBinding = false;

    var obj = {
        ARMode: _arType_Quotation,
        CustomerCode: "",
        SiteCode: "",
        ContractCode: "",
        ProjectCode: "",
        QuotationCode: $('#spQARCode').text(),
        ARType: $('#cbbQARType').val(),
        //DuedateDeadline: $('#cbbQARDueDate').val(),
        DuedateDeadline: null,
        ARStatus: $('#cbbQARStatus').val()
    };

    //SetFilterTo(obj);

    RetrieveARData(obj, true);
}

function clearSelectAR_click() {    
    EnableSearchSection();
    SetScreenToDefault();
}

function radcustomer_click() {
    VaridateCtrl(["txtCustomerCode","txtSiteCode","txtContractCode","txtProjectCode","txtQuotationCode"], null);
    CloseWarningDialog();
    //SetScreenToDefault();
    HideAll();
    //$('#divSelectARListFields').show();
    ShowSelectARList();
    ClearSelectTextbox();
    DisableAllSelectTextbox();

    $('#txtCustomerCode').removeAttr("readonly");
}

function radsite_click() {
    VaridateCtrl(["txtCustomerCode","txtSiteCode","txtContractCode","txtProjectCode","txtQuotationCode"], null);
    CloseWarningDialog();
    //SetScreenToDefault();
    HideAll();
    //$('#divSelectARListFields').show();
    ShowSelectARList();
    ClearSelectTextbox();
    DisableAllSelectTextbox();

    $('#txtSiteCode').removeAttr("readonly");
}

function radcontract_click() {
    VaridateCtrl(["txtCustomerCode","txtSiteCode","txtContractCode","txtProjectCode","txtQuotationCode"], null);
    CloseWarningDialog();
    //SetScreenToDefault();
    HideAll();
    //$('#divSelectARListFields').show();
    ShowSelectARList();
    ClearSelectTextbox();
    DisableAllSelectTextbox();

    $('#txtContractCode').removeAttr("readonly");
}

function radproject_click() {
    VaridateCtrl(["txtCustomerCode","txtSiteCode","txtContractCode","txtProjectCode","txtQuotationCode"], null);
    CloseWarningDialog();
    //SetScreenToDefault();
    HideAll();
    //$('#divSelectARListFields').show();
    ShowSelectARList();
    ClearSelectTextbox();
    DisableAllSelectTextbox();

    $('#txtProjectCode').removeAttr("readonly");
}

function radquotation_click() {
    VaridateCtrl(["txtCustomerCode","txtSiteCode","txtContractCode","txtProjectCode","txtQuotationCode"], null);
    CloseWarningDialog();
    //SetScreenToDefault();
    HideAll();
    //$('#divSelectARListFields').show();
    ShowSelectARList();
    ClearSelectTextbox();
    DisableAllSelectTextbox();

    $('#txtQuotationCode').removeAttr("readonly");
}

function arGridBinding(grid, gridname, zonename, gen_ctrl) {
    var idxARStatusCol = grid.getColIndexById('ARStatus');
    var idxARFlagCol = grid.getColIndexById('ARFlag');
    var idxARIDCol = grid.getColIndexById('ARID');
    var idxARNoCol = grid.getColIndexById('ARNo');
    var idxHeadCodeCol = grid.getColIndexById('HeadCode');
    var idxHeadNameCol = grid.getColIndexById('HeadName');

    //if (isUserRequest && !isGridBinding) {
    if (!CheckFirstRowIsEmpty(grid)){
        if (grid.getRowsNum() != 0) {

            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);

                //grid.setRowTextStyle(row_id, "word-wrap: break-word;");

                if (i == 0) {
                    var headCode = grid.cells(row_id, idxHeadCodeCol).getValue();
                    var headName = grid.cells(row_id, idxHeadNameCol).getValue();

                    if ((headCode.length > 0) && (headName.length > 0)) {
                        $("#sp" + zonename + "Code").text(headCode);
                        $("#sp" + zonename + "Name").text(headName);
                    }
                }

                //            grid.setRowColor(row_id, "white");
                //            grid.setCellTextStyle(row_id, idxARStatusCol, "");
                //            grid.setCellTextStyle(row_id, idxARFlagCol, "");

                // Check New/Over and set color
                grid.setRowColor(row_id, "#ffffff");
                
                //Comment by Jutarat A. on 04092012
//                if ((grid.cells(row_id, idxARStatusCol).getValue() != null) && (grid.cells(row_id, idxARStatusCol).getValue().indexOf('ver') != -1)) {
//                    grid.setRowColor(row_id, "#ff9999");
//                    grid.setCellTextStyle(row_id, idxARStatusCol, "background-color: red; color: white; font-weight: bold;");
//                    grid.cells(row_id, idxARStatusCol).setValue(_markerOver);
//                } else 
                //End Comment
                
                if ((grid.cells(row_id, idxARStatusCol).getValue() != null) && (grid.cells(row_id, idxARStatusCol).getValue().indexOf('ew') != -1)) {
                    grid.setCellTextStyle(row_id, idxARStatusCol, "background-color: yellow; color: red; font-weight: bold;");
                    grid.cells(row_id, idxARStatusCol).setValue(_markerNew);
                }

                if ((grid.cells(row_id, idxARFlagCol).getValue() != null) && (grid.cells(row_id, idxARFlagCol).getValue().length > 0)) {
                    grid.setCellTextStyle(row_id, idxARFlagCol, "background-color: red; color: white; font-weight: bold;");
                    grid.cells(row_id, idxARFlagCol).setValue(_markerStar);
                }

                if (gen_ctrl)
                {
                    var ARID = grid.cells(row_id, idxARIDCol).getValue();
                    var ARNo = grid.cells(row_id, idxARNoCol).getValue();

                    var tagA = "<a href='#'>" + ARNo + "<input type='hidden' name='ARID' value='" + ARID + "'/></a>";
                    grid.cells(row_id, idxARNoCol).setValue(tagA);
                }
            }

            var gridA = $(gridname + " a");

            gridA.each(function () {
                $(this).unbind('click');
                
                $(this).click(function () {
                    var obj = {
                        pRequestNo: $(this).children("input:hidden[name=ARID]").val()
                    };
                    ajax_method.CallScreenControllerWithAuthority("/Contract/CTS380", obj, true);
                });
            });
            isGirdBinding = true;
            isUserRequest = !(isGirdBinding && isSubGridBinding);
        }
        
    }
}

function occcurringGridBinding(grid, gridname, codeColName, gridtype, gen_ctrl) {
    var idxCodeCol = grid.getColIndexById(codeColName);
    //if (isUserRequest && !isSubGridBinding)
    if (!CheckFirstRowIsEmpty(grid))
    {
        if (grid.getRowsNum() != 0) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var code = grid.cells(row_id, idxCodeCol).getValue();

                if (gen_ctrl)
                {
                    var tagA = "<a href='#'>" + code + "<input type='hidden' name='" + codeColName + "' value='" + code + "'/></a>";
                    grid.cells(row_id, idxCodeCol).setValue(tagA);
                }
            }

            var gridA = $(gridname + " a");

            gridA.each(function () {
                $(this).unbind('click');

                $(this).click(function () {
//                    var obj = {
//                        ARMode: "",
//                        CustomerCode: "",
//                        SiteCode: "",
//                        ContractCode: "",
//                        ProjectCode: "",
//                        QuotationCode: "",
//                        ARType: "",
//                        DuedateDeadline: defDueDate,
//                        ARStatus: defStatus
//                    };

//                    isUserRequest = true;
//                    isGirdBinding = false;
//                    isSubGridBinding = false;

//                    if (gridtype == "1") {
//                        obj.ARMode = _arType_Site;
//                        obj.SiteCode = $(this).children("input:hidden[name=" + codeColName + "]").val();
//                    } else {
//                        obj.ARMode = _arType_Contract;
//                        obj.ContractCode = $(this).children("input:hidden[name=" + codeColName + "]").val();
//                    }

                    //SetFilterTo(obj);

//                    RetrieveARData(obj, false);
                    
                    var obj = {
                        strARRelevantType: "",
                        strARRelevantCode: ""
                    };

                    if (gridtype == "1") {
                        obj.strARRelevantType = _arType_Site;
                        obj.strARRelevantCode = $(this).children("input:hidden[name=" + codeColName + "]").val();
                    } else {
                        obj.strARRelevantType = _arType_Contract;
                        obj.strARRelevantCode = $(this).children("input:hidden[name=" + codeColName + "]").val();
                    }

                    ajax_method.CallScreenControllerWithAuthority("/Contract/CTS370", obj, true);
                });
            });
            isSubGridBinding = true;
            isUserRequest = !(isGirdBinding && isSubGridBinding);
        }
    }
}

// ---------------------------------------------------------------------------------
// Function
// ---------------------------------------------------------------------------------
function DisableSearchSection() {
    $('#btnRetrieveSelectARList').attr('disabled', 'disabled');
    $('#radCustomer').attr('disabled', 'disabled');
    $('#radSite').attr('disabled', 'disabled');
    $('#radContract').attr('disabled', 'disabled');
    $('#radProject').attr('disabled', 'disabled');
    $('#radQuotation').attr('disabled', 'disabled');

    $('#txtCustomerCode').attr('readonly', 'readonly');
    $('#txtSiteCode').attr('readonly', 'readonly');
    $('#txtContractCode').attr('readonly', 'readonly');
    $('#txtProjectCode').attr('readonly', 'readonly');
    $('#txtQuotationCode').attr('readonly', 'readonly');
}

function EnableSearchSection() {
    $('#btnRetrieveSelectARList').removeAttr('disabled');
    $('#radCustomer').removeAttr('disabled');
    $('#radSite').removeAttr('disabled');
    $('#radContract').removeAttr('disabled');
    $('#radProject').removeAttr('disabled');
    $('#radQuotation').removeAttr('disabled');

    $('#txtCustomerCode').removeAttr('readonly');
    $('#txtSiteCode').removeAttr('readonly');
    $('#txtContractCode').removeAttr('readonly');
    $('#txtProjectCode').removeAttr('readonly');
    $('#txtQuotationCode').removeAttr('readonly');
}

function CheckingParameter() {
    if (isCarGridInitial && isCarOccGridInitial && isCtarGridInitial && isSarGridInitial && isSarOccGridInitial && isParGridInitial && isQarGridInitial)
    {
        call_ajax_method_json("/Contract/CTS370_CheckParameter", "", function (result, controls) {
            if (result != null) {
                isFromParameter = true;
                isUserRequest = true;
                isGirdBinding = false;
                isSubGridBinding = false;
                RetrieveARData(result, false);
            }
            else
            {
                ShowSelectARList();
            }

            
        });
    }
}

function LoadInitialDefaultValue() {
    call_ajax_method_json("/Contract/CTS370_RetrieveInitialDefaultValue", "", function (result, controls) {
        defDueDate = result.DueDate;
        defStatus = result.Status;
    });
}

function RetrieveARData(obj, fromOwnSection) {
    VaridateCtrl(["txtCustomerCode","txtSiteCode","txtContractCode","txtProjectCode","txtQuotationCode"], null);
    // Validate Condition
    var isValid = false;
    isFromOwnSection = fromOwnSection;
    VaridateCtrl(validateCriteria, null);
    var HeaderObj = null;

    call_ajax_method_json("/Contract/CTS370_RetrieveGridHeader", obj, function (result, controls) {
        if(controls != null)
        {
            VaridateCtrl(["txtCustomerCode","txtSiteCode","txtContractCode","txtProjectCode","txtQuotationCode"], controls);
        }
        if (result != null) {
            HeaderObj = result;
            if (obj.ARMode == _arType_Customer) {
                if ((obj.CustomerCode != null) && (obj.CustomerCode.length > 0))
                    isValid = true;

                if (isValid) {
                    if (!fromOwnSection) {
                        SetFilterWithDisableTo(obj);
                    }
                } else if (!isValid) {
                    SetFilterTo(obj);
                }

                call_ajax_method_json("/Contract/CTS370_RetrieveARResult", obj, function (result, controls) {
                    if ((result != null) && (result.SearchResult) && (result.SID.length > 0)) {
                        $('#spCARCode').text(HeaderObj.Code);
                        $('#spCARName').text(HeaderObj.Name);

                        var sObj = {
                            sName: result.SID,
                            occType: "1"
                        };
                
                        //ShowCustomerAR(!fromOwnSection);
                        $('#gridCARMain').LoadDataToGrid(carGrid, pageRow, true, "/Contract/CTS370_RetrieveARData", sObj, "CTS370_ARGridResult", false, null, function (result, controls, isWarnig) {
                            ShowCustomerAR();
                        });
                        $('#gridCARSub').LoadDataToGrid(carOccGrid, pageRow, true, "/Contract/CTS370_RetireveOccurringData", sObj, "CTS370_AROccurringSiteGridResult", false, null, function (result, controls, isWarnig) {
                            ShowCustomerAR();
                        });
                    } else
                    {
                        if (!isFromOwnSection)
                        {
                            VaridateCtrl(validateCriteria, ["txtCustomerCode"]);
                        }
                    }
                });
            } else if (obj.ARMode == _arType_Site) {
                if ((obj.SiteCode != null) && (obj.SiteCode.length > 0))
                    isValid = true;

                if (isValid) {
                    if (!fromOwnSection) {
                        SetFilterWithDisableTo(obj);
                    }
                    //ShowSiteAR(!fromOwnSection);
                } else if (!isValid) {
                    SetFilterTo(obj);
                }

                call_ajax_method_json("/Contract/CTS370_RetrieveARResult", obj, function (result, controls) {
                    if ((result != null) && (result.SearchResult) && (result.SID.length > 0)) {
                        $('#spSARCode').text(HeaderObj.Code);
                        $('#spSARName').text(HeaderObj.Name);

                        var sObj = {
                            sName: result.SID,
                            occType: "2"
                        }

                        //ShowSiteAR(!fromOwnSection);
                        $('#gridSARMain').LoadDataToGrid(sarGrid, pageRow, true, "/Contract/CTS370_RetrieveARData", sObj, "CTS370_ARGridResult", false, null, function (result, controls, isWarnig) {
                            ShowSiteAR();
                        });
                        $('#gridSARSub').LoadDataToGrid(sarOccGrid, pageRow, true, "/Contract/CTS370_RetireveOccurringData", sObj, "CTS370_AROccurringContractGridResult", false, null, function (result, controls, isWarnig) {
                            ShowSiteAR();
                        });
                    } else
                    {
                        if (!isFromOwnSection)
                        {
                            VaridateCtrl(validateCriteria, ["txtSiteCode"]);
                        }
                    }
                });
            } else if (obj.ARMode == _arType_Contract) {
                if ((obj.ContractCode != null) && (obj.ContractCode.length > 0))
                    isValid = true;

                if (isValid) {
                    if (!fromOwnSection) {
                        SetFilterWithDisableTo(obj);
                    }
                    //ShowContractAR(!fromOwnSection);
                } else if (!isValid) {
                    SetFilterTo(obj);
                }

                call_ajax_method_json("/Contract/CTS370_RetrieveARResult", obj, function (result, controls) {
                    if ((result != null) && (result.SearchResult) && (result.SID.length > 0)) {
                        $('#spCTARCode').text(HeaderObj.Code);
                        $('#spCTARName').text(HeaderObj.Name);

                        var sObj = {
                            sName: result.SID,
                            occType: "2"
                        };

                        //ShowContractAR(!fromOwnSection);
                        $('#gridCTARMain').LoadDataToGrid(ctarGrid, pageRow, true, "/Contract/CTS370_RetrieveARData", sObj, "CTS370_ARGridResult", false, null, function (result, controls, isWarnig) {
                            ShowContractAR();
                        });
                    } else
                    {
                        if (!isFromOwnSection)
                        {
                            VaridateCtrl(validateCriteria, ["txtContractCode"]);
                        }
                    }
                });
            } else if (obj.ARMode == _arType_Project) {
                if ((obj.ProjectCode != null) && (obj.ProjectCode.length > 0))
                    isValid = true;

                if (isValid) {
                    if (!fromOwnSection) {
                        SetFilterWithDisableTo(obj);
                    }
                    //ShowProjectAR(!fromOwnSection);
                } else if (!isValid) {
                    SetFilterTo(obj);
                }

                call_ajax_method_json("/Contract/CTS370_RetrieveARResult", obj, function (result, controls) {
                    if ((result != null) && (result.SearchResult) && (result.SID.length > 0)) {
                        $('#spPARCode').text(HeaderObj.Code);
                        $('#spPARName').text(HeaderObj.Name);

                        var sObj = {
                            sName: result.SID,
                            occType: "2"
                        };

                        //ShowProjectAR(!fromOwnSection);
                        $('#gridPARMain').LoadDataToGrid(parGrid, pageRow, true, "/Contract/CTS370_RetrieveARData", sObj, "CTS370_ARGridResult", false, null, function (result, controls, isWarnig) {
                            ShowProjectAR();
                        });
                    } else
                    {
                        if (!isFromOwnSection)
                        {
                            VaridateCtrl(validateCriteria, ["txtProjectCode"]);
                        }
                    }
                });
            } else if (obj.ARMode == _arType_Quotation) {
                if ((obj.QuotationCode != null) && (obj.QuotationCode.length > 0))
                    isValid = true;

                if (isValid) {
                    if (!fromOwnSection) {
                        SetFilterWithDisableTo(obj);
                    }
                    //ShowProjectAR(!fromOwnSection);
                } else if (!isValid) {
                    SetFilterTo(obj);
                }

                call_ajax_method_json("/Contract/CTS370_RetrieveARResult", obj, function (result, controls) {
                    if ((result != null) && (result.SearchResult) && (result.SID.length > 0)) {
                        $('#spQARCode').text(HeaderObj.Code);
                        $('#spQARName').text(HeaderObj.Name);

                        var sObj = {
                            sName: result.SID,
                            occType: "2",
                        };

                        //ShowQuotationAR(!fromOwnSection);
                        $('#gridQARMain').LoadDataToGrid(qarGrid, pageRow, true, "/Contract/CTS370_RetrieveARData", sObj, "CTS370_ARGridResult", false, null, function (result, controls, isWarnig) {
                            ShowQuotationAR();
                        });
                    } else
                    {
                        if (!isFromOwnSection)
                        {
                            VaridateCtrl(validateCriteria, ["txtQuotationCode"]);
                        }
                    }
                });
            }
        } else {
            SetFilterTo(obj)
        }
    });
}

function SetScreenToDefault() {
    VaridateCtrl(["txtCustomerCode","txtSiteCode","txtContractCode","txtProjectCode","txtQuotationCode"], null);
    CloseWarningDialog();
    ClearSelectTextbox();
    DisableAllSelectTextbox();

    $('#radCustomer').attr("checked", true);
    //radcustomer_click();    
    DisableAllSelectTextbox();
    $('#txtCustomerCode').removeAttr("readonly");
    //$('#btnRetrieveSelectARList').removeAttr('disabled');

    HideAll();
    ShowSelectARList();
}

function SetFilterWithDisableTo(obj) {
    SetFilterTo(obj);
    DisableSearchSection();
}

function SetFilterTo(obj) {
    EnableSearchSection();
    ClearSelectTextbox();

    if (obj.ARMode == _arType_Customer) {
        $('#radCustomer').attr("checked", true);
        radcustomer_click();
        $('#txtCustomerCode').val(obj.CustomerCode);
    } else if (obj.ARMode == _arType_Site) {
        $('#radSite').attr("checked", true);
        radsite_click();
        $('#txtSiteCode').val(obj.SiteCode);
    } else if (obj.ARMode == _arType_Contract) {
        $('#radContract').attr("checked", true);
        radcontract_click();
        $('#txtContractCode').val(obj.ContractCode);
    } else if (obj.ARMode == _arType_Project) {
        $('#radProject').attr("checked", true);
        radproject_click();
        $('#txtProjectCode').val(obj.ProjectCode);
    } else if (obj.ARMode == _arType_Quotation) {
        $('#radQuotation').attr("checked", true);
        radquotation_click();
        $('#txtQuotationCode').val(obj.QuotationCode);
    }
}

function InitialGrid() {

    carGrid = $("#gridCARMain").InitialGrid(pageRow, true, "/Contract/CTS370_InitialGridAR", function ()
    {
        isCarGridInitial = true;
        CheckingParameter();
    });
    carOccGrid = $("#gridCARSub").InitialGrid(pageRow, true, "/Contract/CTS370_InitialGridOccurringSite", function ()
    {
        isCarOccGridInitial = true;
        CheckingParameter();
    });

    sarGrid = $("#gridSARMain").InitialGrid(pageRow, true, "/Contract/CTS370_InitialGridAR", function ()
    {
        isSarGridInitial = true;
        CheckingParameter();
    });
    sarOccGrid = $("#gridSARSub").InitialGrid(pageRow, true, "/Contract/CTS370_InitialGridOccurringContract", function ()
    {
        isSarOccGridInitial = true;
        CheckingParameter();
    });

    ctarGrid = $("#gridCTARMain").InitialGrid(pageRow, true, "/Contract/CTS370_InitialGridAR", function ()
    {
        isCtarGridInitial = true;
        CheckingParameter();
    });

    parGrid = $("#gridPARMain").InitialGrid(pageRow, true, "/Contract/CTS370_InitialGridAR", function ()
    {
        isParGridInitial = true;
        CheckingParameter();
    });

    qarGrid = $("#gridQARMain").InitialGrid(pageRow, true, "/Contract/CTS370_InitialGridAR", function ()
    {
        isQarGridInitial = true;
        CheckingParameter();
    });
}

function ClearSelectTextbox() {
    $('#txtCustomerCode').val('');
    $('#txtSiteCode').val('');
    $('#txtContractCode').val('');
    $('#txtProjectCode').val('');
    $('#txtQuotationCode').val('');
}

function DisableAllSelectTextbox() {
    $("#txtCustomerCode").attr("readonly", true);
    $("#txtSiteCode").attr("readonly", true);
    $("#txtContractCode").attr("readonly", true);
    $("#txtProjectCode").attr("readonly", true);
    $("#txtQuotationCode").attr("readonly", true);
}

function ShowSelectARList() {
    if (!isFromParameter) {
        $('#divSelectARListFields').show();
    }
}

function ShowCustomerAR() {
    HideAll();
    ShowSelectARList();
    $('#divCustomerAR').show();

    if (!isFromOwnSection) {
        $('#cbbCARType').val("");
        //$('#cbbCARDueDate').val(defDueDate);
        $('#cbbCARStatus').val(defStatus);
    }
}

function ShowSiteAR() {
    HideAll();
    ShowSelectARList();
    $('#divSiteAR').show();

    if (!isFromOwnSection) {
        $('#cbbSARType').val("");
        //$('#cbbSARDueDate').val(defDueDate);
        $('#cbbSARStatus').val(defStatus);
    }

//    if ((sarGrid.getRowsNum() == 0) || ((sarGrid.getRowsNum() > 0) && (CheckFirstRowIsEmpty(sarGrid)))) {
//        $('#btnSARCustomerAR').removeAttr('enable');
//    } else {
//        $('#btnSARCustomerAR').attr('enable', 'enable');
//    }
}

function ShowContractAR() {
    HideAll();
    ShowSelectARList();
    $('#divContractAR').show();

    if (!isFromOwnSection) {
        $('#cbbCTARType').val("");
        //$('#cbbCTARDueDate').val(defDueDate);
        $('#cbbCTARStatus').val(defStatus);
    }

//    if ((ctarGrid.getRowsNum() == 0) || ((ctarGrid.getRowsNum() > 0) && (CheckFirstRowIsEmpty(ctarGrid)))) {
//        $('#btnCTARCustomerAR').attr('disabled', 'disabled');
//        $('#btnCTARSiteAR').attr('disabled', 'disabled');
//    } else {
//        $('#btnCTARCustomerAR').removeAttr('disabled');
//        $('#btnCTARSiteAR').removeAttr('disabled');
//    }
}

function ShowProjectAR() {
    HideAll();
    ShowSelectARList();
    $('#divProjectAR').show();

    if (!isFromOwnSection) {
        $('#cbbPARType').val("");
        //$('#cbbPARDueDate').val(defDueDate);
        $('#cbbPARStatus').val(defStatus);
    }
}

function ShowQuotationAR() {
    HideAll();
    ShowSelectARList();
    $('#divQuotationAR').show();

    if (!isFromOwnSection) {
        $('#cbbQARType').val("");
        //$('#cbbQARDueDate').val(defDueDate);
        $('#cbbQARStatus').val(defStatus);
    }
}

function HideSelectARList() {
    $('#divSelectARListFields').hide();
}

function HideCustomerAR() {
    $('#divCustomerAR').hide();
}

function HideSiteAR() {
    $('#divSiteAR').hide();
}

function HideContractAR() {
    $('#divContractAR').hide();
}

function HideProjectAR() {
    $('#divProjectAR').hide();
}

function HideQuotationAR() {
    $('#divQuotationAR').hide();
}

function HideAll() {
    HideSelectARList();
    HideContractAR();
    HideCustomerAR();
    HideProjectAR();
    HideSiteAR();
    HideQuotationAR();
}