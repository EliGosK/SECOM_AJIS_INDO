/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>

var btn_history_detail = "btnHistoryDetail";
var Cms300_grid;
function CMS300Initial() {
    ChangeDialogButtonText(["Close"], [$('#btnClose').val()]);
}

$(document).ready(function () {
    $("#Contract_Customer_Detail").hide();
    $("#Real_Customer_Detail").hide();
    $("#Site_Detail").hide();
    //$("#Change_History_List").hide();

    innitialGrid();
});

function innitialGrid() {
    if ($("#history_list_grid").length > 0) {
        var parameter;
        if (typeof (CMS300Object) == "function") {
            var obj = CMS300Object();

            //ContractCode: 'N0000000001',
            //OCC: null,
            //CSCustCode: null,
            //RCCustCode: null,
            //SiteCode: null

            parameter = {
                strContractCode: obj.ContractCode,
                strOCC: obj.OCC,
                strCSCustCode: obj.CSCustCode,
                strRCCustCode: obj.RCCustCode,
                strSiteCode: obj.SiteCode
            }
        }

        //Cms300_grid = $("#history_list_grid").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/Common/CMS300_InitSiteGrid");

        Cms300_grid = $("#history_list_grid").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, true, true, "/Common/CMS300_Search", parameter, "dtChangedCustHistList", false);

        SpecialGridControl(Cms300_grid, ["Detail"]);
        BindOnLoadedEvent(Cms300_grid, function (gen_ctrl) {
            if (Cms300_grid.getRowsNum() != 0) {
                for (var i = 0; i < Cms300_grid.getRowsNum(); i++) {
                    var row_id = Cms300_grid.getRowId(i);

                    if (gen_ctrl == true) {
                        GenerateDetailButton(Cms300_grid, btn_history_detail, row_id, "Detail", true);

                        var csChange = Cms300_grid.cells(row_id, Cms300_grid.getColIndexById('CSChangeFlag')).getValue();
                        var rcChange = Cms300_grid.cells(row_id, Cms300_grid.getColIndexById('RCChangeFlag')).getValue();
                        var siteChange = Cms300_grid.cells(row_id, Cms300_grid.getColIndexById('SiteChangeFlag')).getValue();
                        //var signerChange = Cms300_grid.cells(row_id, Cms300_grid.getColIndexById('ContractSignerChangeFlag')).getValue(); //Add by Jutarat A. on 18092012

                        if (csChange == "1") {
                            //Modify by Jutarat A. on 19092012
                            //Cms300_grid.cells(row_id, Cms300_grid.getColIndexById('CSCustCode')).setBgColor("#ff9999");
                            //Cms300_grid.cells(row_id, Cms300_grid.getColIndexById('CSCustName')).setBgColor("#ff9999");
                            Cms300_grid.cells(row_id, Cms300_grid.getColIndexById('CSCustCodeName')).setBgColor("#ff9999");
                            //End Modify

                            Cms300_grid.cells(row_id, Cms300_grid.getColIndexById('ContractSignerTypeName')).setBgColor("#ff9999");
                        }

                        //Add by Jutarat A. on 18092012
                        //if (signerChange == "1") {
                        //    Cms300_grid.cells(row_id, Cms300_grid.getColIndexById('ContractSignerTypeName')).setBgColor("#ff9999");
                        //}
                        //End Add

                        if (rcChange == "1") {
                            //Modify by Jutarat A. on 19092012
                            //Cms300_grid.cells(row_id, Cms300_grid.getColIndexById('RCCustCode')).setBgColor("#ff9999");
                            //Cms300_grid.cells(row_id, Cms300_grid.getColIndexById('RCCustName')).setBgColor("#ff9999");
                            Cms300_grid.cells(row_id, Cms300_grid.getColIndexById('RCCustCodeName')).setBgColor("#ff9999");
                            //End Modify
                        }

                        if (siteChange == "1") {
                            //Modify by Jutarat A. on 19092012
                            //Cms300_grid.cells(row_id, Cms300_grid.getColIndexById('SiteCode')).setBgColor("#ff9999");
                            //Cms300_grid.cells(row_id, Cms300_grid.getColIndexById('SiteName')).setBgColor("#ff9999");
                            Cms300_grid.cells(row_id, Cms300_grid.getColIndexById('SiteCodeName')).setBgColor("#ff9999");
                            //End Modify
                        }
                    }

                    BindGridButtonClickEvent(btn_history_detail, row_id, function (rid) {
                        var contractCode = Cms300_grid.cells(rid, Cms300_grid.getColIndexById('ContractCode')).getValue();
                        var sequenceNo = Cms300_grid.cells(rid, Cms300_grid.getColIndexById('SequenceNo')).getValue();
                        Cms300_grid.selectRow(Cms300_grid.getRowIndex(rid));
                        getCustomerHistoryDetail(contractCode, sequenceNo);


                    });
                }
            }
        });

//        $("#history_list_grid").LoadDataToGrid(Cms300_grid, ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/Common/CMS300_Search", parameter, "dtChangedCustHistList", false, null,
//            function (result, controls, isWarning) {
//                if (isWarning == undefined) {
//                    $("#Change_History_List").show();
//                }
//            }
//        );
    }
}

function getCustomerHistoryDetail(contractCode, sequenceNo) {
    call_ajax_method(
        '/Common/CMS300_SearchDetail/',
        { ContractCode: contractCode, SequenceNo: sequenceNo },
        function (result, controls) {
            if (result != undefined) {

                //                $("#Contract_Customer_Detail").show();
                //                $("#Real_Customer_Detail").show();
                //                $("#Site_Detail").show();

                $("#CMS300_ContractCustomer").clearForm();
                $("#CMS300_RealCustomer").clearForm();
                $("#CMS300_Site").clearForm();

                $("#CMS300_ContractCustomer").bindJSON_ViewMode(result, "CMS300_");
                $("#CMS300_RealCustomer").bindJSON_ViewMode(result, "CMS300_");
                $("#CMS300_Site").bindJSON_ViewMode(result, "CMS300_");

                //                $("#CMS300_SiteCode").html(result.SiteCode);
                //                $("#SiteNameEN").html(result.SiteNameEN);
                //                $("#SiteAddressFullEN").html(result.SiteAddressFullEN);
                //                $("#SiteNameLC").html(result.SiteNameLC);
                //                $("#SiteAddressFullLC").html(result.SiteAddressFullLC);
                //                $("#SitePhoneNo").html(result.SitePhoneNo);
                //                $("#SiteBuildingUsageCode").html(result.SiteBuildingUsageCode);

                $("#Contract_Customer_Detail").show();
                $("#Real_Customer_Detail").show();
                $("#Site_Detail").show('normal',
                                function () { document.getElementById('Contract_Customer_Detail').scrollIntoView(); });

                $("#Contract_Customer_Detail").SetEmptyViewData();
                $("#Real_Customer_Detail").SetEmptyViewData();
                $("#Site_Detail").SetEmptyViewData();
            
                if (result.SpecialCareFlag == true) {
                    $("#CMS300_ChkAttachImportanceFlag").attr("checked", true);
                    
                }
                else {
                    $("#CMS300_ChkAttachImportanceFlag").attr("checked", false);
                   
                }

            }
        }
    );
}