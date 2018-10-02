/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

/* --- Variables --- */
var groupSummaryGrid;
var custGrid;
var siteGrid;
var contractGrid;
var pageRow; // = 5;
var btnCustDetailId = "CMS100CustDetailBtn";
var btnSiteDetailId = "CMS100SiteDetailBtn";
var btnContDetailId = "CMS100ContDetailBtn";

var custRoleType = "";

$(document).ready(function () {

    pageRow = CMS100Data.PageRow;

    /* --- Initial grid --- */
    if ($("#customer_grid_result").length > 0) {
        custGrid = $("#customer_grid_result").InitialGrid(pageRow, true, "/Common/CMS100_InitialCustomerGrid");
    }
    if ($("#site_grid_result").length > 0) {
        siteGrid = $("#site_grid_result").InitialGrid(pageRow, true, "/Common/CMS100_InitialSiteGrid");
    }
    if ($("#contract_grid_result").length > 0) {
        contractGrid = $("#contract_grid_result").InitialGrid(pageRow, true, "/Common/CMS100_InitialContractGrid");
        /*=========== Set hidden column =============*/
        contractGrid.setColumnHidden(contractGrid.getColIndexById("ServiceTypeCode"), true);
    }

    /* ===== binding event when finish load custGrid ===== */
    SpecialGridControl(custGrid, ["Detail"]);
    BindOnLoadedEvent(custGrid, function (gen_ctrl) {
        var detailColInx = custGrid.getColIndexById('Detail');

        for (var i = 0; i < custGrid.getRowsNum(); i++) {

            var row_id = custGrid.getRowId(i);

            if (gen_ctrl == true) {
                GenerateDetailButton(custGrid, btnCustDetailId, row_id, "Detail", true);
            }

            /* ===== Bind event onClick to button ===== */
            BindGridButtonClickEvent(btnCustDetailId, row_id, function (rid) {
                doCustDetailAction(rid);
            });
        }
    });

    /* ===== binding event when finish load siteGrid ===== */
    SpecialGridControl(siteGrid, ["Detail"]);
    BindOnLoadedEvent(siteGrid, function (gen_ctrl) {
        var detailColInx = siteGrid.getColIndexById('Detail');

        for (var i = 0; i < siteGrid.getRowsNum(); i++) {

            var row_id = siteGrid.getRowId(i);

            if (gen_ctrl == true) {
                GenerateDetailButton(siteGrid, btnSiteDetailId, row_id, "Detail", true);
            }

            /* ===== Bind event onClick to button ===== */
            BindGridButtonClickEvent(btnSiteDetailId, row_id, function (rid) {
                doSiteDetailAction(rid);
            });
        }
    });

    /* ===== binding event when finish load contractGrid ===== */
    SpecialGridControl(contractGrid, ["Detail"]);
    BindOnLoadedEvent(contractGrid, function (gen_ctrl) {
        var detailColInx = contractGrid.getColIndexById('Detail');

        for (var i = 0; i < contractGrid.getRowsNum(); i++) {

            var row_id = contractGrid.getRowId(i);

            if (gen_ctrl == true) {
                GenerateDetailButton(contractGrid, btnContDetailId, row_id, "Detail", true);
            }

            /* ===== Bind event onClick to button ===== */
            BindGridButtonClickEvent(btnContDetailId, row_id, function (rid) {
                doContractDetailAction(rid);
            });
        }
    });

    LoadGroupSummaryGrid();

    initial();
});

function initial() {
    $("#CustInfo_Section").show();
    $("#GroupSummary_Section").show();
    $("#CustomerList_Section").hide();
    $("#SiteList_Section").hide();
    $("#ContractList_Section").hide();
}

function LoadGroupSummaryGrid() {
    /* --- Initial Grid --- */
    groupSummaryGrid = $("#group_grid_result").LoadDataToGridWithInitial(0, false, false,
                                "/Common/CMS100_LoadGroupSummary",
                                "",
                                "dtGroupSummaryForShow", false);

    /*=========== Set hidden column =============*/
    groupSummaryGrid.setColumnHidden(groupSummaryGrid.getColIndexById("RowPrefix"), true);
    
    /* ===== binding event when finish load data ===== */
    BindOnLoadedEvent(groupSummaryGrid, function (gen_ctrl) {

        var RowPrefix_ColIdx = groupSummaryGrid.getColIndexById("RowPrefix");
        var ContractAlarm_ColIdx = groupSummaryGrid.getColIndexById("ContractAlarm");
        var ContractMaint_ColIdx = groupSummaryGrid.getColIndexById("ContractMaintenance");
        var ContractGuard_ColIdx = groupSummaryGrid.getColIndexById("ContractGuard");
        var ContractSale_ColIdx = groupSummaryGrid.getColIndexById("ContractSale");
        var CustomerAlarm_ColIdx = groupSummaryGrid.getColIndexById("CustomerAlarm");
        var CustomerMaint_ColIdx = groupSummaryGrid.getColIndexById("CustomerMaintenance");
        var CustomerGuard_ColIdx = groupSummaryGrid.getColIndexById("CustomerGuard");
        var CustomerSale_ColIdx = groupSummaryGrid.getColIndexById("CustomerSale");

        for (var i = 0; i < groupSummaryGrid.getRowsNum(); i++) {

            var RowPrefix = groupSummaryGrid.cells2(i, RowPrefix_ColIdx).getValue();
            var headerName = "";

            if (gen_ctrl == true) {
                if (RowPrefix == "customer") {

                    //Contract Target Purchaser----------------------------------------------------------------------------
                    //Contract Target Purchaser - Alarm
                    var CT_Alarm_value = groupSummaryGrid.cells2(i, ContractAlarm_ColIdx).getValue();
                    if (CT_Alarm_value != '-') {
                        headerName = CMS100Data.ContractTargetDisplay + ' - ' + CMS100Data.AlarmDisplay;
                        var CT_Alarm_tagA = '<a href="#" onclick="loadCustomer(\'Cust_CT_Rental\',\'' + CMS100Data.AlarmPrefix + '\',\'' + headerName + '\',\'' + c_contract_target + '\')">' + CT_Alarm_value + '</a>';
                        groupSummaryGrid.cells2(i, ContractAlarm_ColIdx).setValue(CT_Alarm_tagA);
                    }

                    //Contract Target Purchaser - Maintenance
                    var CT_Maint_value = groupSummaryGrid.cells2(i, ContractMaint_ColIdx).getValue();
                    if (CT_Maint_value != '-') {
                        headerName = CMS100Data.ContractTargetDisplay + ' - ' + CMS100Data.MaintenanceDisplay;
                        var CT_Maint_tagA = '<a href="#" onclick="loadCustomer(\'Cust_CT_Rental\',\'' + CMS100Data.MaintPrefix + '\',\'' + headerName + '\',\'' + c_contract_target + '\')">' + CT_Maint_value + '</a>';
                        groupSummaryGrid.cells2(i, ContractMaint_ColIdx).setValue(CT_Maint_tagA);
                    }

                    //Contract Target Purchaser - Guard
                    var CT_Guard_value = groupSummaryGrid.cells2(i, ContractGuard_ColIdx).getValue();
                    if (CT_Guard_value != '-') {
                        headerName = CMS100Data.ContractTargetDisplay + ' - ' + CMS100Data.GuardDisplay;
                        var CT_Guard_tagA = '<a href="#" onclick="loadCustomer(\'Cust_CT_Rental\',\'' + CMS100Data.GuardPrefix + '\',\'' + headerName + '\',\'' + c_contract_target + '\')">' + CT_Guard_value + '</a>';
                        groupSummaryGrid.cells2(i, ContractGuard_ColIdx).setValue(CT_Guard_tagA);
                    }

                    //Contract Target Purchaser - Sale
                    var CT_Sale_value = groupSummaryGrid.cells2(i, ContractSale_ColIdx).getValue();
                    if (CT_Sale_value != '-') {
                        headerName = CMS100Data.ContractTargetDisplay + ' - ' + CMS100Data.SaleDisplay;
                        var CT_Sale_tagA = '<a href="#" onclick="loadCustomer(\'Cust_CT_Sale\',\'' + CMS100Data.SalePrefix + '\',\'' + headerName + '\',\'' + c_purchaser + '\')">' + CT_Sale_value + '</a>';
                        groupSummaryGrid.cells2(i, ContractSale_ColIdx).setValue(CT_Sale_tagA);
                    }

                    //Real Customer----------------------------------------------------------------------------------------
                    //Real Customer - Alarm
                    var RC_Alarm_value = groupSummaryGrid.cells2(i, CustomerAlarm_ColIdx).getValue();
                    if (RC_Alarm_value != '-') {
                        headerName = CMS100Data.RealCustomerDisplay + ' - ' + CMS100Data.AlarmDisplay;
                        var RC_Alarm_tagA = '<a href="#" onclick="loadCustomer(\'Cust_RC_Rental\',\'' + CMS100Data.AlarmPrefix + '\',\'' + headerName + '\',\'' + c_real_cust + '\')">' + RC_Alarm_value + '</a>';
                        groupSummaryGrid.cells2(i, CustomerAlarm_ColIdx).setValue(RC_Alarm_tagA);
                    }

                    //Real Customer - Maintenance
                    var RC_Maint_value = groupSummaryGrid.cells2(i, CustomerMaint_ColIdx).getValue();
                    if (RC_Maint_value != '-') {
                        headerName = CMS100Data.RealCustomerDisplay + ' - ' + CMS100Data.MaintenanceDisplay;
                        var RC_Maint_tagA = '<a href="#" onclick="loadCustomer(\'Cust_RC_Rental\',\'' + CMS100Data.MaintPrefix + '\',\'' + headerName + '\',\'' + c_real_cust + '\')">' + RC_Maint_value + '</a>';
                        groupSummaryGrid.cells2(i, CustomerMaint_ColIdx).setValue(RC_Maint_tagA);
                    }

                    //Real Customer - Guard
                    var RC_Guard_value = groupSummaryGrid.cells2(i, CustomerGuard_ColIdx).getValue();
                    if (RC_Guard_value != '-') {
                        headerName = CMS100Data.RealCustomerDisplay + ' - ' + CMS100Data.GuardDisplay;
                        var RC_Guard_tagA = '<a href="#" onclick="loadCustomer(\'Cust_RC_Rental\',\'' + CMS100Data.GuardPrefix + '\',\'' + headerName + '\',\'' + c_real_cust + '\')">' + RC_Guard_value + '</a>';
                        groupSummaryGrid.cells2(i, CustomerGuard_ColIdx).setValue(RC_Guard_tagA);
                    }

                    //Real Customer - Sale
                    var RC_Sale_value = groupSummaryGrid.cells2(i, CustomerSale_ColIdx).getValue();
                    if (RC_Sale_value != '-') {
                        headerName = CMS100Data.RealCustomerDisplay + ' - ' + CMS100Data.SaleDisplay;
                        var RC_Sale_tagA = '<a href="#" onclick="loadCustomer(\'Cust_RC_Sale\',\'' + CMS100Data.SalePrefix + '\',\'' + headerName + '\',\'' + c_real_cust + '\')">' + RC_Sale_value + '</a>';
                        groupSummaryGrid.cells2(i, CustomerSale_ColIdx).setValue(RC_Sale_tagA);
                    }

                } else if (RowPrefix == "site") {

                    //Contract Target Purchaser---------------------------------------------------------------------
                    //Contract Target Purchaser - Alarm
                    var CT_Alarm_value = groupSummaryGrid.cells2(i, ContractAlarm_ColIdx).getValue();
                    if (CT_Alarm_value != '-') {
                        headerName = CMS100Data.ContractTargetDisplay + ' - ' + CMS100Data.AlarmDisplay;
                        var CT_Alarm_tagA = '<a href="#" onclick="loadSite(\'Site_CT_Rental\',\'' + CMS100Data.AlarmPrefix + '\',\'' + headerName + '\')">' + CT_Alarm_value + '</a>';
                        groupSummaryGrid.cells2(i, ContractAlarm_ColIdx).setValue(CT_Alarm_tagA);
                    }

                    //Contract Target Purchaser Maintenance
                    var CT_Maint_value = groupSummaryGrid.cells2(i, ContractMaint_ColIdx).getValue();
                    if (CT_Maint_value != '-') {
                        headerName = CMS100Data.ContractTargetDisplay + ' - ' + CMS100Data.MaintenanceDisplay;
                        var CT_Maint_tagA = '<a href="#" onclick="loadSite(\'Site_CT_Rental\',\'' + CMS100Data.MaintPrefix + '\',\'' + headerName + '\')">' + CT_Maint_value + '</a>';
                        groupSummaryGrid.cells2(i, ContractMaint_ColIdx).setValue(CT_Maint_tagA);
                    }

                    //Contract Target Purchaser Guard
                    var CT_Guard_value = groupSummaryGrid.cells2(i, ContractGuard_ColIdx).getValue();
                    if (CT_Guard_value != '-') {
                        headerName = CMS100Data.ContractTargetDisplay + ' - ' + CMS100Data.GuardDisplay;
                        var CT_Guard_tagA = '<a href="#" onclick="loadSite(\'Site_CT_Rental\',\'' + CMS100Data.GuardPrefix + '\',\'' + headerName + '\')">' + CT_Guard_value + '</a>';
                        groupSummaryGrid.cells2(i, ContractGuard_ColIdx).setValue(CT_Guard_tagA);
                    }

                    //Contract Target Purchaser Sale
                    var CT_Sale_value = groupSummaryGrid.cells2(i, ContractSale_ColIdx).getValue();
                    if (CT_Sale_value != '-') {
                        headerName = CMS100Data.ContractTargetDisplay + ' - ' + CMS100Data.SaleDisplay;
                        var CT_Sale_tagA = '<a href="#" onclick="loadSite(\'Site_CT_Sale\',\'' + CMS100Data.SalePrefix + '\',\'' + headerName + '\')">' + CT_Sale_value + '</a>';
                        groupSummaryGrid.cells2(i, ContractSale_ColIdx).setValue(CT_Sale_tagA);
                    }

                    //Real Customer----------------------------------------------------------------------------------------
                    //Real Customer - Alarm
                    var RC_Alarm_value = groupSummaryGrid.cells2(i, CustomerAlarm_ColIdx).getValue();
                    if (RC_Alarm_value != '-') {
                        headerName = CMS100Data.RealCustomerDisplay + ' - ' + CMS100Data.AlarmDisplay;
                        var RC_Alarm_tagA = '<a href="#" onclick="loadSite(\'Site_RC_Rental\',\'' + CMS100Data.AlarmPrefix + '\',\'' + headerName + '\')">' + RC_Alarm_value + '</a>';
                        groupSummaryGrid.cells2(i, CustomerAlarm_ColIdx).setValue(RC_Alarm_tagA);
                    }

                    //Real Customer - Maintenance
                    var RC_Maint_value = groupSummaryGrid.cells2(i, CustomerMaint_ColIdx).getValue();
                    if (RC_Maint_value != '-') {
                        headerName = CMS100Data.RealCustomerDisplay + ' - ' + CMS100Data.MaintenanceDisplay;
                        var RC_Maint_tagA = '<a href="#" onclick="loadSite(\'Site_RC_Rental\',\'' + CMS100Data.MaintPrefix + '\',\'' + headerName + '\')">' + RC_Maint_value + '</a>';
                        groupSummaryGrid.cells2(i, CustomerMaint_ColIdx).setValue(RC_Maint_tagA);
                    }

                    //Real Customer - Guard
                    var RC_Guard_value = groupSummaryGrid.cells2(i, CustomerGuard_ColIdx).getValue();
                    if (RC_Guard_value != '-') {
                        headerName = CMS100Data.RealCustomerDisplay + ' - ' + CMS100Data.GuardDisplay;
                        var RC_Guard_tagA = '<a href="#" onclick="loadSite(\'Site_RC_Rental\',\'' + CMS100Data.GuardPrefix + '\',\'' + headerName + '\')">' + RC_Guard_value + '</a>';
                        groupSummaryGrid.cells2(i, CustomerGuard_ColIdx).setValue(RC_Guard_tagA);
                    }

                    //Real Customer - Sale
                    var RC_Sale_value = groupSummaryGrid.cells2(i, CustomerSale_ColIdx).getValue();
                    if (RC_Sale_value != '-') {
                        headerName = CMS100Data.RealCustomerDisplay + ' - ' + CMS100Data.SaleDisplay;
                        var RC_Sale_tagA = '<a href="#" onclick="loadSite(\'Site_RC_Sale\',\'' + CMS100Data.SalePrefix + '\',\'' + headerName + '\')">' + RC_Sale_value + '</a>';
                        groupSummaryGrid.cells2(i, CustomerSale_ColIdx).setValue(RC_Sale_tagA);
                    }

                } else if (RowPrefix == "contract") {

                    //Contract Target Purchaser---------------------------------------------------------------------
                    //Contract Target Purchaser - Alarm
                    var CT_Alarm_value = groupSummaryGrid.cells2(i, ContractAlarm_ColIdx).getValue();
                    if (CT_Alarm_value != '-') {
                        headerName = CMS100Data.ContractTargetDisplay + ' - ' + CMS100Data.AlarmDisplay;
                        var CT_Alarm_tagA = '<a href="#" onclick="loadContract(\'Contract_CT_Rental\',\'' + CMS100Data.AlarmPrefix + '\',\'' + headerName + '\')">' + CT_Alarm_value + '</a>';
                        groupSummaryGrid.cells2(i, ContractAlarm_ColIdx).setValue(CT_Alarm_tagA);
                    }

                    //Contract Target Purchaser Maintenance
                    var CT_Maint_value = groupSummaryGrid.cells2(i, ContractMaint_ColIdx).getValue();
                    if (CT_Maint_value != '-') {
                        headerName = CMS100Data.ContractTargetDisplay + ' - ' + CMS100Data.MaintenanceDisplay;
                        var CT_Maint_tagA = '<a href="#" onclick="loadContract(\'Contract_CT_Rental\',\'' + CMS100Data.MaintPrefix + '\',\'' + headerName + '\')">' + CT_Maint_value + '</a>';
                        groupSummaryGrid.cells2(i, ContractMaint_ColIdx).setValue(CT_Maint_tagA);
                    }

                    //Contract Target Purchaser Guard
                    var CT_Guard_value = groupSummaryGrid.cells2(i, ContractGuard_ColIdx).getValue();
                    if (CT_Guard_value != '-') {
                        headerName = CMS100Data.ContractTargetDisplay + ' - ' + CMS100Data.GuardDisplay;
                        var CT_Guard_tagA = '<a href="#" onclick="loadContract(\'Contract_CT_Rental\',\'' + CMS100Data.GuardPrefix + '\',\'' + headerName + '\')">' + CT_Guard_value + '</a>';
                        groupSummaryGrid.cells2(i, ContractGuard_ColIdx).setValue(CT_Guard_tagA);
                    }

                    //Contract Target Purchaser Sale
                    var CT_Sale_value = groupSummaryGrid.cells2(i, ContractSale_ColIdx).getValue();
                    if (CT_Sale_value != '-') {
                        headerName = CMS100Data.ContractTargetDisplay + ' - ' + CMS100Data.SaleDisplay;
                        var CT_Sale_tagA = '<a href="#" onclick="loadContract(\'Contract_CT_Sale\',\'' + CMS100Data.SalePrefix + '\',\'' + headerName + '\')">' + CT_Sale_value + '</a>';
                        groupSummaryGrid.cells2(i, ContractSale_ColIdx).setValue(CT_Sale_tagA);
                    }

                    //Real Customer----------------------------------------------------------------------------------------
                    //Real Customer - Alarm
                    var RC_Alarm_value = groupSummaryGrid.cells2(i, CustomerAlarm_ColIdx).getValue();
                    if (RC_Alarm_value != '-') {
                        headerName = CMS100Data.RealCustomerDisplay + ' - ' + CMS100Data.AlarmDisplay;
                        var RC_Alarm_tagA = '<a href="#" onclick="loadContract(\'Contract_RC_Rental\',\'' + CMS100Data.AlarmPrefix + '\',\'' + headerName + '\')">' + RC_Alarm_value + '</a>';
                        groupSummaryGrid.cells2(i, CustomerAlarm_ColIdx).setValue(RC_Alarm_tagA);
                    }

                    //Real Customer - Maintenance
                    var RC_Maint_value = groupSummaryGrid.cells2(i, CustomerMaint_ColIdx).getValue();
                    if (RC_Maint_value != '-') {
                        headerName = CMS100Data.RealCustomerDisplay + ' - ' + CMS100Data.MaintenanceDisplay;
                        var RC_Maint_tagA = '<a href="#" onclick="loadContract(\'Contract_RC_Rental\',\'' + CMS100Data.MaintPrefix + '\',\'' + headerName + '\')">' + RC_Maint_value + '</a>';
                        groupSummaryGrid.cells2(i, CustomerMaint_ColIdx).setValue(RC_Maint_tagA);
                    }

                    //Real Customer - Guard
                    var RC_Guard_value = groupSummaryGrid.cells2(i, CustomerGuard_ColIdx).getValue();
                    if (RC_Guard_value != '-') {
                        headerName = CMS100Data.RealCustomerDisplay + ' - ' + CMS100Data.GuardDisplay;
                        var RC_Guard_tagA = '<a href="#" onclick="loadContract(\'Contract_RC_Rental\',\'' + CMS100Data.GuardPrefix + '\',\'' + headerName + '\')">' + RC_Guard_value + '</a>';
                        groupSummaryGrid.cells2(i, CustomerGuard_ColIdx).setValue(RC_Guard_tagA);
                    }

                    //Real Customer - Sale
                    var RC_Sale_value = groupSummaryGrid.cells2(i, CustomerSale_ColIdx).getValue();
                    if (RC_Sale_value != '-') {
                        headerName = CMS100Data.RealCustomerDisplay + ' - ' + CMS100Data.SaleDisplay;
                        var RC_Sale_tagA = '<a href="#" onclick="loadContract(\'Contract_RC_Sale\',\'' + CMS100Data.SalePrefix + '\',\'' + headerName + '\')">' + RC_Sale_value + '</a>';
                        groupSummaryGrid.cells2(i, CustomerSale_ColIdx).setValue(RC_Sale_tagA);
                    }
                }
            }
        }
    });

    //change empty to -
    $("#CustInfo_Section").SetEmptyViewData();
}

function loadCustomer(type, prefix, header, customerRole) {
    custRoleType = customerRole;

    var param = { 
        "type": type,
        "contractPrefix": prefix 
    };

    $("#customer_grid_result").LoadDataToGrid(custGrid, pageRow, true, "/Common/CMS100_LoadCustomer", param, "dtCustomerListGrp", false,
        null, //post-load
        function (result, controls, isWarning) { //pre-load
            if (isWarning == undefined) {
                $("#CustomerList_Section").show();
            }
        });

    //change title header
    $("#label_customer").text(CMS100Data.CustomerListHeader + ' (' + header + ')');

    $("#CustomerList_Section").show();
    $("#SiteList_Section").hide();
    $("#ContractList_Section").hide();
}

function loadSite(type, prefix, header) {
    
    var param = {
        "type": type,
        "contractPrefix": prefix
    };

    $("#site_grid_result").LoadDataToGrid(siteGrid, pageRow, true, "/Common/CMS100_LoadSite", param, "dtsiteListGrp", false,
        null, //post-load
        function (result, controls, isWarning) { //pre-load
            if (isWarning == undefined) {
                $("#SiteList_Section").show();
            }
        });

    //change title header
    $("#label_site").text(CMS100Data.SiteListHeader + ' (' + header + ')');

    $("#CustomerList_Section").hide();
    $("#SiteList_Section").show();
    $("#ContractList_Section").hide();
}

function loadContract(type, prefix, header) {

    var param = {
        "type": type,
        "contractPrefix": prefix
    };

    $("#contract_grid_result").LoadDataToGrid(contractGrid, pageRow, true, "/Common/CMS100_LoadContract", param, "dtContractListGrp", false,
        null, //post-load
        function (result, controls, isWarning) { //pre-load
            if (isWarning == undefined) {
                $("#ContractList_Section").show();
            }
        });

    //change title header
    $("#label_contract").text(CMS100Data.ContractListHeader + ' (' + header + ')');

    $("#CustomerList_Section").hide();
    $("#SiteList_Section").hide();
    $("#ContractList_Section").show();
}

function doCustDetailAction(rid) {
    //hilight row
    custGrid.selectRow(custGrid.getRowIndex(rid));

    
    custRoleType = "";

    var custCode = custGrid.cells2(custGrid.getRowIndex(rid), custGrid.getColIndexById('CustCodeShort')).getValue();
    var obj = {
        strCustomerCode: custCode,
        strCustomerRole: custRoleType
    };

    ajax_method.CallScreenControllerWithAuthority("/Common/CMS080", obj, true);
}

function doSiteDetailAction(rid) {
    //hilight row
    siteGrid.selectRow(siteGrid.getRowIndex(rid));

    var siteCode = siteGrid.cells2(siteGrid.getRowIndex(rid), siteGrid.getColIndexById('SiteCodeShort')).getValue();
    var obj = {
        strSiteCode: siteCode
    };

    ajax_method.CallScreenControllerWithAuthority("/Common/CMS280", obj, true);
}

function doContractDetailAction(rid) {
    //hilight row
    contractGrid.selectRow(contractGrid.getRowIndex(rid));

    var contractCode = contractGrid.cells2(contractGrid.getRowIndex(rid), contractGrid.getColIndexById('ContractCodeShort')).getValue();
    var serviceTypeCode = contractGrid.cells2(contractGrid.getRowIndex(rid), contractGrid.getColIndexById('ServiceTypeCode')).getValue();
    var obj = {
        strContractCode: contractCode,
        strServiceTypeCode: serviceTypeCode
    };

    ajax_method.CallScreenControllerWithAuthority("/Common/CMS190", obj, true);
}

