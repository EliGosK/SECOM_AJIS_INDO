
/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>

/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

var CTS300_gridContractTarget;

var CTS300_IncidentRelevantInfoTxt = ["CustomerCode", "SiteCode", "ProjectCode", "UserCode_ContractCode"];

var CTS300_ContractTargetData = null;
var CTS300_ContractLost = true;
var btnRemoveContractID = "btnRemove";
var currentRelevCode = null;

var IsInit01 = false;
var IsInitContractGrid = false;

$(document).ready(function () {
    $('#rdoCustomer').change(function () {
        VaridateCtrl(CTS300_IncidentRelevantInfoTxt, null);
        SetScreenToDefault();
        SetRelevantType(_incType_Customer);
        IncidentRelevantType_changed();
    });

    $('#rdoSite').change(function () {
        VaridateCtrl(CTS300_IncidentRelevantInfoTxt, null);
        SetScreenToDefault();
        SetRelevantType(_incType_Site);
        IncidentRelevantType_changed();
    });

    $('#rdoProject').change(function () {
        VaridateCtrl(CTS300_IncidentRelevantInfoTxt, null);
        SetScreenToDefault();
        SetRelevantType(_incType_Project);
        IncidentRelevantType_changed();
    });

    $('#rdoContract').change(function () {
        VaridateCtrl(CTS300_IncidentRelevantInfoTxt, null);
        SetScreenToDefault();
        SetRelevantType(_incType_Contract);
        IncidentRelevantType_changed();
    });

    $('#btnRetrive_Customer').click(RetrieveCustomer_clicked);
    $('#btnRetrive_Site').click(RetrieveSite_clicked);
    $('#btnRetrive_Project').click(RetrieveProject_clicked);
    $('#btnRetrive_Contract').click(RetrieveContract_clicked);

    $('#btnSearch_Customer').click(SearchCustomer_clicked);
    $('#btnSearch_Site').click(SearchSite_clicked);
    $('#btnSearch_Project').click(SearchProject_clicked);
    $('#btnSearch_Contract').click(SearchContract_clicked);
    $('#btnClear_Contract').click(ClearContract_clicked);

    CTS300_gridContractTarget = $("#CTS300_gridContractTarget").InitialGrid(10, false, "/Contract/CTS300_IntialGridContractTarget", function () {
        IsInitContractGrid = true;
        MarkAsInit01();
    });
    SpecialGridControl(CTS300_gridContractTarget, ["RemoveBtn"]);
    BindOnLoadedEvent(CTS300_gridContractTarget, CTS300_gridContractTargetBinding);

});

function MarkAsInit01() {
    IsInit01 = IsInitContractGrid;
    LoadParameter();
}

function hasContract(contractCode) {
    if ((CTS300_ContractTargetData != null) && (CTS300_ContractTargetData != undefined)) {
        for (var i = 0; i < CTS300_ContractTargetData.length; i++) {
            if ((CTS300_ContractTargetData[i].ContractCode == contractCode) || (CTS300_ContractTargetData[i].UserCode == contractCode)) {
                return true;
            }
        }
    }

    return false;
}

function AddContract(obj) {
    if ((CTS300_ContractTargetData == null) || (CTS300_ContractTargetData == undefined)) {
        CTS300_ContractTargetData = new Array();
    }

    CTS300_ContractTargetData[CTS300_ContractTargetData.length] = obj;
}

function RemoveContract(contractCode) {
    for (var i = 0; i < CTS300_ContractTargetData.length; i++) {
        if (CTS300_ContractTargetData[i].ContractCode == contractCode) {
            CTS300_ContractTargetData.splice(i, 1);
        }
    }
}

function CTS300_gridContractTargetBinding(gen_ctrl) {
    var _colRemoveButton = CTS300_gridContractTarget.getColIndexById("RemoveBtn");

    for (var i = 0; i < CTS300_gridContractTarget.getRowsNum(); i++) {
        var row_id = CTS300_gridContractTarget.getRowId(i);

//        if (gen_ctrl == true) {
            GenerateRemoveButton(CTS300_gridContractTarget, btnRemoveContractID, row_id, "RemoveBtn", true);
//        }

        BindGridButtonClickEvent(btnRemoveContractID, row_id, RemoveContractTarget_clicked);
    }

    CTS300_gridContractTarget.setSizes();
}

function RemoveContractTarget_clicked(row_id) {
    var _colContractCode = CTS300_gridContractTarget.getColIndexById("ContracrCode");
    var _targContractCode = CTS300_gridContractTarget.cells(row_id, _colContractCode).getValue();

    if (hasContract(_targContractCode.toString())) {
        RemoveContract(_targContractCode.toString());
    }

    var needClearGrid = false;
    if (CTS300_gridContractTarget.getRowsNum() == 1) {
        needClearGrid = true;
    }

    DeleteRow(CTS300_gridContractTarget, row_id);

    if (needClearGrid) {
        CTS300_ContractTargetData = new Array();
        if (CTS300_gridContractTarget != null) {
            DeleteAllRow(CTS300_gridContractTarget);
        }
        //CTS300_gridContractTarget.clearAll();

        SetScreenToDefault();
        SetRelevantType(_incType_Contract);
        IncidentRelevantType_changed();
    }

    CTS300_gridContractTarget.setSizes();
}

function RetrieveCustomer_clicked() {
    VaridateCtrl(CTS300_IncidentRelevantInfoTxt, null);

    if ($('#CustomerCode').val().length == 0) {
        doAlert("Common", "MSG0007", $('#divLblCustomerCode').text());
        VaridateCtrl(CTS300_IncidentRelevantInfoTxt, ["CustomerCode"]);
    } else {
        var obj = {
            strCustomerCode: $('#CustomerCode').val()
        };
        call_ajax_method_json("/Contract/CTS300_RetrieveCustomerInfo", obj, function (result, controls) {
            if ((controls != null) || (controls != undefined)) {
                VaridateCtrl(CTS300_IncidentRelevantInfoTxt, controls);
            }

            if ((result != null) && (result != undefined)) {
                // Map data to form
                currentRelevCode = result.SearchResult.CustCode;
                $('#CustomerCode').val(result.SearchResult.CustCode);
                BindData_03(result.SearchResult);
                BindGrid_03("/Contract/CTS300_RetrieveCustomerGroupGrid", obj, "dtCustomeGroupData", null, function () {
                    SetShow_02(false);
                    SetShow_03(true);
                    SetShow_04(false);
                    SetShow_05(true);
                    SetShow_06(false);

                    SetShowInCharge_05(result.HasPermissionInCharge);

                    CTS300_gridCustomerGroup.setSizes();
                });
                EnableRegisterSection();
                DisableSearch();
                //$('#CustomerCode').val('');

                lastCondition = $('#CustomerCode').val();
            }
        });
    }
}

function RetrieveSite_clicked() {
    VaridateCtrl(CTS300_IncidentRelevantInfoTxt, null);

    if ($('#SiteCode').val().length == 0) {
        doAlert("Common", "MSG0007", $('#divLblSiteCode').text());
        VaridateCtrl(CTS300_IncidentRelevantInfoTxt, ["SiteCode"]);
    } else {
        var obj = {
            strSiteCode: $('#SiteCode').val()
        };

        call_ajax_method_json("/Contract/CTS300_RetrieveSiteInfo", obj, function (result, controls) {
            if ((controls != null) || (controls != undefined)) {
                VaridateCtrl(CTS300_IncidentRelevantInfoTxt, controls);
            }

            if ((result != null) && (result != undefined)) {
                // Map data to form
                currentRelevCode = result.SearchResult.SiteCode;
                $('#SiteCode').val(result.SearchResult.SiteCode);
                BindData_04(result.SearchResult);
                SetShow_02(false);
                SetShow_03(false);
                SetShow_04(true);
                SetShow_05(true);
                SetShow_06(false);

                SetShowInCharge_05(result.HasPermissionInCharge);
                EnableRegisterSection();
                DisableSearch();
                //$('#SiteCode').val('');

                lastCondition = $('#SiteCode').val();
            }
        });
    }
}

function RetrieveProject_clicked() {
    VaridateCtrl(CTS300_IncidentRelevantInfoTxt, null);

    if ($('#ProjectCode').val().length == 0) {
        doAlert("Common", "MSG0007", $('#divLblProjectCode').text());
        VaridateCtrl(CTS300_IncidentRelevantInfoTxt, ["ProjectCode"]);
    } else {
        var obj = {
            strProjectCode: $('#ProjectCode').val()
        };

        call_ajax_method_json("/Contract/CTS300_RetrieveProjectInfo", obj, function (result, controls) {
            if ((controls != null) || (controls != undefined)) {
                VaridateCtrl(CTS300_IncidentRelevantInfoTxt, controls);
            }

            if ((result != null) && (result != undefined)) {
                // Map data to form

                currentRelevCode = result.SearchResult.ProjectCode;
                $('#ProjectCode').val(result.SearchResult.ProjectCode);
                BindData_02(result.SearchResult);
                SetShow_02(true);
                SetShow_03(false);
                SetShow_04(false);
                SetShow_05(true);
                SetShow_06(false);

                SetShowInCharge_05(result.HasPermissionInCharge);
                EnableRegisterSection();
                DisableSearch();
                //$('#ProjectCode').val('');

                lastCondition = $('#ProjectCode').val();
            }
        });
    }
}

function RetrieveContract_clicked() {
    VaridateCtrl(CTS300_IncidentRelevantInfoTxt, null);

    if ($('#UserCode_ContractCode').val().length == 0) {
        doAlert("Common", "MSG0007", $('#divLblUserCode_ContractCode').text());
        VaridateCtrl(CTS300_IncidentRelevantInfoTxt, ["UserCode_ContractCode"]);
    } else if ((CTS300_ContractTargetData != null) && (CTS300_ContractTargetData != undefined) && (hasContract($('#UserCode_ContractCode').val()))) {
        doAlert("Contract", "MSG3194", $('#UserCode_ContractCode').val());
        VaridateCtrl(CTS300_IncidentRelevantInfoTxt, ["UserCode_ContractCode"]);
    } else {
        var obj = {
            strContractCode: $('#UserCode_ContractCode').val()
        }
        call_ajax_method_json("/Contract/CTS300_RetrieveContractInfo", obj, function (result, controls) {
            if ((controls != null) || (controls != undefined)) {
                VaridateCtrl(CTS300_IncidentRelevantInfoTxt, ["UserCode_ContractCode"]);
            }

            if ((result != null) && (result.SearchResult != null)) {
                //$('#UserCode_ContractCode').val(result.SearchResult.ContractCode);
                currentRelevCode = null;
                if (CTS300_ContractLost) {
                    SetShow_05(true);
                    CTS300_ContractLost = false;
                }
                SetShowInCharge_05(result.HasPermissionInCharge);
                var newObj = {
                    ContractName: result.SearchResult.ContractName,
                    SiteName: result.SearchResult.SiteName,
                    ContractCode: result.SearchResult.ContractCode,
                    UserCode: result.SearchResult.UserCode,
                    ContractFrom: result.SearchResult.ContractFrom
                };

                if (CTS300_ContractTargetData == null) {
                    CTS300_ContractTargetData = new Array();
                }

                CheckFirstRowIsEmpty(CTS300_gridContractTarget, true);
                AddContract(newObj);
                AddNewRow(CTS300_gridContractTarget, [result.SearchResult.ContractName, result.SearchResult.SiteName, result.SearchResult.UserCode, result.SearchResult.ContractCode, ""]);
                EnableRegisterSection();
                DisableRelevantRadio();
                //DisableSearch();
                //$('#UserCode_ContractCode').val('');

                lastCondition = $('#UserCode_ContractCode').val();
            }
        });
    }
}

function SearchCustomer_clicked() {
    VaridateCtrl(CTS300_IncidentRelevantInfoTxt, null);
    $("#dlgBox").OpenCMS250Dialog("CTS300");
}

function SearchSite_clicked() {
    VaridateCtrl(CTS300_IncidentRelevantInfoTxt, null);
    $("#dlgBox").OpenCMS260Dialog("CTS300");
}

function SearchProject_clicked() {
    VaridateCtrl(CTS300_IncidentRelevantInfoTxt, null);
    $("#dlgBox").OpenCMS290Dialog("CTS300");
}

function SearchContract_clicked() {
    VaridateCtrl(CTS300_IncidentRelevantInfoTxt, null);
    $("#dlgBox").OpenCMS310Dialog("CTS300");
}

function ClearContract_clicked() {
    VaridateCtrl(CTS300_IncidentRelevantInfoTxt, null);
    $('#btnClear_Contract').attr('disabled', 'disabled');
    doAskYesNo("Common", "MSG0044", null, function () {
        lastCondition = null;
        hasParameter = false;
        currentRelevCode = null;
        SetScreenToDefault();
        $('#btnClear_Contract').removeAttr('disabled');
    }, function () {
        $('#btnClear_Contract').removeAttr('disabled');
    });
}

function SetDefault_01(isClearRelevantType) {
    VaridateCtrl(CTS300_IncidentRelevantInfoTxt, null);
    CTS300_ContractTargetData = null;
    currentRelevCode = null;

    if (isClearRelevantType != false) {
        SetRelevantType(_incType_Customer);
        IncidentRelevantType_changed();
    }
}

function IncidentRelevantType_changed() {
    //SetScreenToDefault();
    VaridateCtrl(CTS300_IncidentRelevantInfoTxt, null);
    currentRelevCode = null;

    if (CTS300_gridContractTarget != null) {
        DeleteAllRow(CTS300_gridContractTarget);
    }

    if (GetRelevantType() == _incType_Customer) {
        // Customer
        DisableAllControl_01();
        $('#CustomerCode').removeAttr('readonly');

        $('#btnRetrive_Customer').removeAttr('disabled');
        $('#btnSearch_Customer').removeAttr('disabled');
        $('#rdoRelatedCustomer').removeAttr('disabled');
        $('#rdoRelatedAllSite').removeAttr('disabled');

        SetCustomerRelateType(1);
        CTS300_ContractLost = true;
    } else if (GetRelevantType() == _incType_Site) {
        // Site
        DisableAllControl_01();
        $('#SiteCode').removeAttr('readonly');

        $('#btnRetrive_Site').removeAttr('disabled');
        $('#btnSearch_Site').removeAttr('disabled');
        $('#rdoRelatedSite').removeAttr('disabled');
        $('#rdoRelatedAllContract').removeAttr('disabled');

        SetSiteRelateType(1);
        CTS300_ContractLost = true;
    } else if (GetRelevantType() == _incType_Project) {
        // Project
        DisableAllControl_01();
        $('#ProjectCode').removeAttr('readonly');

        $('#btnRetrive_Project').removeAttr('disabled');
        $('#btnSearch_Project').removeAttr('disabled');

        CTS300_ContractLost = true;
    } else if (GetRelevantType() == _incType_Contract) {
        // Contract
        DisableAllControl_01();
        $('#UserCode_ContractCode').removeAttr('readonly');

        $('#btnRetrive_Contract').removeAttr('disabled');
        $('#btnSearch_Contract').removeAttr('disabled');
        $('#CTS300_gridContractTarget').show();
        //$('#btnClear_Contract').show();
    }

    EnableRelevantRadio();
}

function DisableAllControl_01() {
    $('#CustomerCode').val('');
    $('#SiteCode').val('');
    $('#ProjectCode').val('');
    $('#UserCode_ContractCode').val('');

    $('#CustomerCode').attr('readonly', true);
    $('#SiteCode').attr('readonly', true);
    $('#ProjectCode').attr('readonly', true);
    $('#UserCode_ContractCode').attr('readonly', true);

    $('#btnRetrive_Customer').attr("disabled", true);
    $('#btnSearch_Customer').attr("disabled", true);
    $('#rdoRelatedCustomer').attr("disabled", true);
    $('#rdoRelatedAllSite').attr("disabled", true);
    $('#rdoRelatedCustomer').removeAttr("checked");
    $('#rdoRelatedAllSite').removeAttr("checked");
    //SetCustomerRelateType(-1);

    $('#btnRetrive_Site').attr("disabled", true);
    $('#btnSearch_Site').attr("disabled", true);
    $('#rdoRelatedSite').attr("disabled", true);
    $('#rdoRelatedAllContract').attr("disabled", true);
    $('#rdoRelatedSite').removeAttr("checked");
    $('#rdoRelatedAllContract').removeAttr("checked");
    //SetSiteRelateType(-1);

    $('#btnRetrive_Project').attr("disabled", true);
    $('#btnSearch_Project').attr("disabled", true);

    $('#btnRetrive_Contract').attr("disabled", true);
    $('#btnSearch_Contract').attr("disabled", true);

    if (CTS300_gridContractTarget != null) {
        //CTS300_gridContractTarget.clearAll();
        //DeleteAllRow(CTS300_gridContractTarget);
    }
    $('#CTS300_gridContractTarget').hide();
    //$('#btnClear_Contract').hide();
}

function SetRelevantType(setval) {
    var obj = $('input:radio[name=IncidentRelevantType]');
    obj.filter('[value=' + setval + ']').attr('checked', true);
}

function GetRelevantType() {
    return $('input[name="IncidentRelevantType"]:checked').val();
}

function SetCustomerRelateType(setval) {
    var obj = $('input:radio[name=CustomerRelateType]');
    obj.filter('[value=' + setval + ']').attr('checked', true);
}

function GetCustomerRelateType() {
    return $('input[name="CustomerRelateType"]:checked').val();
}

function SetSiteRelateType(setval) {
    var obj = $('input:radio[name=SiteRelateType]');
    obj.filter('[value=' + setval + ']').attr('checked', true);
}

function GetSiteRelateType() {
    return $('input[name="SiteRelateType"]:checked').val();
}

function EnableRelevantRadio() {
    $('#rdoCustomer').removeAttr('disabled');
    $('#rdoSite').removeAttr('disabled');
    $('#rdoContract').removeAttr('disabled');
    $('#rdoProject').removeAttr('disabled');
}

function EnableSearch() {
    EnableRelevantRadio();

    if (GetRelevantType() == _incType_Customer) {
        $('#CustomerCode').removeAttr('readonly');
        $('#btnRetrive_Customer').removeAttr("disabled");
        $('#btnSearch_Customer').removeAttr("disabled");
    } else if (GetRelevantType() == _incType_Site) {
        $('#SiteCode').removeAttr('readonly');
        $('#btnRetrive_Site').removeAttr("disabled");
        $('#btnSearch_Site').removeAttr("disabled");
    } else if (GetRelevantType() == _incType_Project) {
        $('#ProjectCode').removeAttr('readonly');
        $('#btnRetrive_Project').removeAttr("disabled");
        $('#btnSearch_Project').removeAttr("disabled");
    } else if (GetRelevantType() == _incType_Contract) {
        $('#UserCode_ContractCode').removeAttr('readonly');
        $('#btnRetrive_Contract').removeAttr("disabled");
        $('#btnSearch_Contract').removeAttr("disabled");
    }
}

function DisableRelevantRadio() {
    $('#rdoCustomer').attr('disabled', 'disabled');
    $('#rdoSite').attr('disabled', 'disabled');
    $('#rdoContract').attr('disabled', 'disabled');
    $('#rdoProject').attr('disabled', 'disabled');
}

function DisableSearch() {
    DisableRelevantRadio();

    if (GetRelevantType() == _incType_Customer) {
        $('#CustomerCode').attr('readonly', true);
        $('#btnRetrive_Customer').attr("disabled", true);
        $('#btnSearch_Customer').attr("disabled", true);
    } else if (GetRelevantType() == _incType_Site) {
        $('#SiteCode').attr('readonly', true);
        $('#btnRetrive_Site').attr("disabled", true);
        $('#btnSearch_Site').attr("disabled", true);
    } else if (GetRelevantType() == _incType_Project) {
        $('#ProjectCode').attr('readonly', true);
        $('#btnRetrive_Project').attr("disabled", true);
        $('#btnSearch_Project').attr("disabled", true);
    } else if (GetRelevantType() == _incType_Contract) {
        $('#UserCode_ContractCode').attr('readonly', true);
        $('#btnRetrive_Contract').attr("disabled", true);
        $('#btnSearch_Contract').attr("disabled", true);
    }
}

function GetConditionDataObject() {
    var obj = {
        RelevantType: GetRelevantType(),
        RelevantCode: currentRelevCode,
        ContractRelevant: CTS300_ContractTargetData,
        CustomerRelateType: GetCustomerRelateType(),
        SiteRelateType: GetSiteRelateType()
    };

//    if (GetRelevantType() == 1) {
//        obj.RelevantCode = $('#CustomerCode').val();
//    } else if (GetRelevantType() == 2) {
//        obj.RelevantCode = $('#SiteCode').val();
//    } else if (GetRelevantType() == 3) {
//        obj.RelevantCode = $('#ProjectCode').val();
//    } 
//    else if (GetRelevantType() == 4) {
//        obj.RelevantCode = $('#UserCode_ContractCode').val();
//    }

    return obj;
}

function SetViewMode_01(viewMode) {
    if (CTS300_gridContractTarget != null) {
        var _colRemove = CTS300_gridContractTarget.getColIndexById("RemoveBtn");

        CTS300_gridContractTarget.setColumnHidden(_colRemove, viewMode);
        CTS300_gridContractTarget.setSizes();
    }

    $('#dIncidentRelevantInfo').SetViewMode(viewMode);
    $('#divContractView').SetViewMode(viewMode);
    $('#divSelectRelevant').SetViewMode(viewMode);
    $('#divCustRelate1').SetViewMode(viewMode);
    $('#divCustRelate2').SetViewMode(viewMode);
    $('#divSiteRelate1').SetViewMode(viewMode);
    $('#divSiteRelate2').SetViewMode(viewMode);

    if (viewMode) {
        if (GetRelevantType() == _incType_Contract) {
            DisableSearch();
        }
        $('#btnClear_Contract').hide();
    } else {
        if (((GetRelevantType() == _incType_Customer) && ($('#btnRetrive_Customer').prop('disabled')))
        || ((GetRelevantType() == _incType_Site) && ($('#btnRetrive_Site').prop('disabled')))
        || ((GetRelevantType() == _incType_Contract) && ($('#btnRetrive_Contract').prop('disabled')))
        || ((GetRelevantType() == _incType_Project) && ($('#btnRetrive_Project').prop('disabled')))) {
            DisableSearch();

            if (GetRelevantType() == _incType_Customer) {
                $('#rdoRelatedCustomer').removeAttr('disabled');
                $('#rdoRelatedAllSite').removeAttr('disabled');
                $('#rdoRelatedSite').attr('disabled', 'disabled');
                $('#rdoRelatedAllContract').attr('disabled', 'disabled');
            } else if (GetRelevantType() == _incType_Site) {
                $('#rdoRelatedCustomer').attr('disabled', 'disabled');
                $('#rdoRelatedAllSite').attr('disabled', 'disabled');
                $('#rdoRelatedSite').removeAttr('disabled');
                $('#rdoRelatedAllContract').removeAttr('disabled');
            } 
        }

        if (CTS300_gridContractTarget != null) {
            SetFitColumnForBackAction(CTS300_gridContractTarget, "TmpColumn");
        }
        $('#btnClear_Contract').show();
        if (GetRelevantType() == _incType_Contract) {
            EnableSearch();
        }
    }
}

function CMS250Response(obj) {
    $("#dlgBox").CloseDialog();
    if ((obj != null) && (obj.CustomerData != null) && (obj.CustomerData.CustCode != null) && (obj.CustomerData.CustCode.length > 0)) {
        SetScreenToDefault();
        SetRelevantType(_incType_Customer);
        IncidentRelevantType_changed();

        $('#CustomerCode').val(obj.CustomerData.CustCode);
        RetrieveCustomer_clicked();
    }
}

function CMS260Response(obj) {
    $("#dlgBox").CloseDialog();
    if ((obj != null) && (obj.SiteCode != null) && (obj.SiteCode.length > 0)) {
        SetScreenToDefault();
        SetRelevantType(_incType_Site);
        IncidentRelevantType_changed();

        $('#SiteCode').val(obj.SiteCode);
        RetrieveSite_clicked();
    }
}

function CMS290Response(obj) {
    $("#dlgBox").CloseDialog();
    if ((obj != null) && (obj.ProjectCode != null) && (obj.ProjectCode.length > 0)) {
        SetScreenToDefault();
        SetRelevantType(_incType_Project);
        IncidentRelevantType_changed();

        $('#ProjectCode').val(obj.ProjectCode);
        RetrieveProject_clicked();
    }
}

function CMS310Response(obj) {
    $("#dlgBox").CloseDialog();
    if ((obj != null) && (obj.ContractCode != null) && (obj.ContractCode.length > 0)) {
        if (!hasContract(obj.ContractCode)) {
            if (GetRelevantType() != _incType_Contract) {
                SetScreenToDefault();
                SetRelevantType(_incType_Contract);
                IncidentRelevantType_changed();
            }

            $('#UserCode_ContractCode').val(obj.ContractCode);
            RetrieveContract_clicked();
        } else {
            doAlert("Contract", "MSG3194", obj.ContractCode);
            VaridateCtrl(CTS300_IncidentRelevantInfoTxt, ["UserCode_ContractCode"]);
        }
    }
}

function SetObject_01(obj) {
    var relateType = parseInt(obj.strIncidentRelevantType);
    SetRelevantType(relateType);
    IncidentRelevantType_changed();
    if (obj.strIncidentRelevantType == _incType_Customer) {
        $('#CustomerCode').val(obj.strIncidentRelevantCode);
    } else if (obj.strIncidentRelevantType == _incType_Site) {
        $('#SiteCode').val(obj.strIncidentRelevantCode);
    } else if (obj.strIncidentRelevantType == _incType_Project) {
        $('#ProjectCode').val(obj.strIncidentRelevantCode);
    } else if (obj.strIncidentRelevantType == _incType_Contract) {
        $('#UserCode_ContractCode').val(obj.strIncidentRelevantCode);
    }
}