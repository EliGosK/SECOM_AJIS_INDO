//----------------------------------------------
// Create : Nattapong N.
//   Date : 8 Jul 2011
//
//---------------------------------------------
/// <reference path="../Base/Master.js" />
/// <reference path="../Base/MessageDialog.js" />
/// <reference path="../Base/GridControl.js" />
/// <reference path="../../Views/Common/CMS280.cshtml" />
/// <reference path="../Base/control_events.js" />


var mygrid;
$(document).ready(function () {

    $("#btnViewIncdList").click(function () {
        /*2.1	Check authority
        2.1.1	Call		CTS320.hasAuthority
        Parameter      	-
        Return		bHasAuthority

        2.2	Show new window of CTS320: Incident list 
        2.2.1	Call		CTS320: Incident list 
        Parameter         	strIncidentRelevantType	= C_INCIDENT_RELEVANT_TYPE_SITE
        strIncidentRelevantCode	= Site[0].SiteCode
        Return		-
        */

        var obj = {
            strIncidentRelevantType: CMS280_ViewBag.IncidentRelevantType_Site,
            strIncidentRelevantCode: CMS280_ViewBag.SiteCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS320", obj, true);
    });
    $("#btnViewARlist").click(function () {
        /*
        3.1	Check authority
        3.1.1	Call		CTS370.hasAuthority
        Parameter      	-
        Return		bHasAuthority

        3.2	Show new window of CTS370: AR list 
        3.2.1	Call		CTS370: AR list 
        Parameter         	strARRelevantType	= C_AR_RELEVANT_TYPE_SITE
        strARRelevantCode	= Site[0].SiteCode
        Return		-


        */

        var obj = {
            strARRelevantType: CMS280_ViewBag.ARRelevantType_Site,
            strARRelevantCode: CMS280_ViewBag.SiteCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS370", obj, true);
    });
    $("#btnRegIncd").click(function () {
        /*
        4.1	Check authority
        4.1.1	Call		CTS300.hasAuthority
        Parameter      	-
        Return		bHasAuthority

        4.2	Show new window of CTS300: Register new incident
        4.2.1	Call	          	CTS300: Register new incident
        Parameter         	strIncidentRelevantType	= C_INCIDENT_RELEVANT_TYPE_SITE
        strIncidentRelevantCode	= Site[0].SiteCode
        Return	          	-

        */

        var obj = {
            strIncidentRelevantType: CMS280_ViewBag.IncidentRelevantType_Site,
            strIncidentRelevantCode: CMS280_ViewBag.SiteCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS300", obj, true);
    });
    $("#btnRegAR").click(function () {
        /*
        5.1	Check authority
        5.1.1	Call		CTS350.hasAuthority
        Parameter      	-
        Return		bHasAuthority

        5.2	Show new window of CTS350: Register new AR
        5.2.1	Call	          	CTS350: Register new AR
        Parameter         	strARRelevantType	= C_AR_RELEVANT_TYPE_SITE
        strARRelevantCode	= Site[0].SiteCode
        Return		-

        */

        var obj = {
            strARRelevantType: CMS280_ViewBag.ARRelevantType_Site,
            strARRelevantCode: CMS280_ViewBag.SiteCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS350", obj, true);
    });

//    $("#btnViewIncdList").attr("disabled", "disabled");
//    $("#btnViewARlist").attr("disabled", "disabled");
//    $("#btnRegIncd").attr("disabled", "disabled");
//    $("#btnRegAR").attr("disabled", "disabled");
    Init();

    if (txtAttachImportanceFlag == true) {
        $("#ChkAttachImportanceFlag").attr("checked", true);
    }
});


function Init() {
    $("#DetailSection").SetEmptyViewData();
    GridInit();
}

function GridInit() {
    if ($("#ResultContractList").length > 0) {

        var obj = { siteCode: CMS280_ViewBag.SiteCode };
        mygrid = $("#ResultContractList").LoadDataToGridWithInitial(
            ROWS_PER_PAGE_FOR_VIEWPAGE,
            true,
            true,
            "/Common/CMS280_GetContract",
            obj,
            "View_dtContractSameSite",
            false);

        BindOnLoadedEvent(mygrid, function (gen_ctrl) {
            for (var i = 0; i < mygrid.getRowsNum(); i++) {
                var row_id = mygrid.getRowId(i);

                if (gen_ctrl == true) {
                    GenerateDetailButton(mygrid, 'btnDetail', row_id, 'Detail', true);
                }

                BindGridButtonClickEvent('btnDetail', row_id, function (rid) {
                    mygrid.selectRow(mygrid.getRowIndex(rid));
                    var col_ContractCode = mygrid.getColIndexById('ContractCode_Short');
                    var col_ServiceTypeCode = mygrid.getColIndexById('ServiceTypeCode');
                    var ContractCode = mygrid.cells(rid, col_ContractCode).getValue();
                    var ServiceTypeCode = mygrid.cells(rid, col_ServiceTypeCode).getValue();
                    ajax_method.CallScreenControllerWithAuthority('/common/CMS190', { strContractCode: ContractCode, strServiceTypeCode: ServiceTypeCode }, true);
                });


            }
            mygrid.setSizes();
        });
        SpecialGridControl(mygrid, ["Detail"]);
        //$("#ResultContractList").LoadDataToGrid(mygrid, pageRow, true, "/Common/CMS280_GetContract", { 'siteCode': CMS280_ViewBag.SiteCode }, "View_dtContractSameSite", false);

        //                window.open(generate_url('/common/CMS190') +
        //                    "?strContractCode=" + strContractCode
        //                    + "&strServiceTypeCode=" + strServiceTypeCode, null, null);

    }

}

