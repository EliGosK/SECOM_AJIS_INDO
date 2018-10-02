/// <reference path="../Base/Master.js" />

var CMS290_mygrid;
var CMS290_pageRow;
var btnSelectId = "CMS290SelectBtn";
var CMS290_searchCond;

$(document).ready(function () {

    CMS290_pageRow = ROWS_PER_PAGE_FOR_SEARCHPAGE;

    if ($("#cms290_grid_result").length > 0) {
        CMS290_mygrid = $("#cms290_grid_result").InitialGrid(CMS290_pageRow, true, "/Common/CMS290_InitialGrid");
        /*=========== Set hidden column =============*/
        CMS290_mygrid.setColumnHidden(CMS290_mygrid.getColIndexById("ToJson"), true);
    }

    /*==== event btnSearch click ====*/
    $("#cms290_btnSearchByCode").click(function () {
        CMS290_validateSearchCriteria_ByCode();
    });
    $("#cms290_btnSearchByCondition").click(function () {
        CMS290_validateSearchCriteria_ByCondition();
    });

    /*==== event btnClear click ====*/
    $("#cms290_btnClearByCode").click(function () {
        CMS290_clearSearchCriteria();
        showSearchByCodeOnly();
    });
    $("#cms290_btnClearByCondition").click(function () {
        CMS290_clearSearchCriteria();
        showSearchByConditionOnly();
    });

    /*==== event radio process type change ====*/
    $("#cms290_rdoSearchByCode").change(function () {
        CMS290_clearSearchCriteria();
        //$("#cms290_rdoSearchByCondition").attr("checked", false);
        showSearchByCodeOnly();
    });
    $("#cms290_rdoSearchByCondition").change(function () {
        CMS290_clearSearchCriteria();
        //$("#cms290_rdoSearchByCode").attr("checked", false);
        showSearchByConditionOnly();
    });

    /* ===== binding event when finish load data ===== */
    SpecialGridControl(CMS290_mygrid, ["Select"]);
    BindOnLoadedEvent(CMS290_mygrid, function (gen_ctrl) {
        var selectColInx = CMS290_mygrid.getColIndexById('Select');

        for (var i = 0; i < CMS290_mygrid.getRowsNum(); i++) {
            var row_id = CMS290_mygrid.getRowId(i);

            if (gen_ctrl == true) {
                /* ===== column Select button ===== */
                //CMS290_mygrid.cells2(i, selectColInx).setValue(GenerateHtmlButton(btnSelectId, row_id, "Select", true));
                GenerateSelectButton(CMS290_mygrid, btnSelectId, row_id, "Select", true);
            }

            /* ===== Bind event onClick to button ===== */
            //BindGridHtmlButtonClickEvent(btnSelectId, row_id, function (rid) {
            BindGridButtonClickEvent(btnSelectId, row_id, function (rid) {
                CMS290_doSelectAction(rid);
            });
        }
    });

    CMS290_OnInitial();
});

function showSearchByCodeOnly() {
    $("#CMS290_SearchByCode").show();
    $("#CMS290_SearchByCondition").hide();
    $("#CMS290_SearchResult").hide();
}

function showSearchByConditionOnly() {
    $("#CMS290_SearchByCode").hide();
    $("#CMS290_SearchByCondition").show();
    $("#CMS290_SearchResult").hide();
}

function CMS290_OnInitial() {
    
    //Clear Search Type radio
    //$("#cms290_rdoSearchByCode").attr("checked", true);
    //$("#cms290_rdoSearchByCondition").attr("checked", false);

    //Show/Hide Section
    showSearchByCodeOnly();
}

function CMS290Initial() {
    ChangeDialogButtonText(["Cancel"], [$('#cms290_btnCancel').val()]);
}

function CMS290_validateSearchCriteria_ByCode() {
    CMS290_searchCond = {
        ProjectCode: $.trim($("#cms290_ProjectCode").val()) ,
        ContractCode: $.trim($("#cms290_ContractCode").val()) 
    };

    call_ajax_method(
        '/Common/CMS290_CheckReqField',
        CMS290_searchCond,
        function (result, controls) {
            if (result != undefined) {
                CMS290_search(CMS290_searchCond);
            }
        }
    );
}

function CMS290_validateSearchCriteria_ByCondition() {

    CMS290_searchCond = {
        ProjectName: $.trim($("#cms290_ProjectName").val()) ,
        ProjectAddress: $.trim($("#cms290_ProjectAddress").val()) ,
        PJPurchaseName: $.trim($("#cms290_PJPurchaseName").val()) ,
        Owner1Name: $.trim($("#cms290_Owner1Name").val()) ,
        PJManagementCompanyName: $.trim($("#cms290_PJManagementCompanyName").val()) ,
        OtherProjectRelatedPersonName: $.trim($("#cms290_OtherProjectRelatedPersonName").val()) ,
        ProductCode: $.trim($("#cms290_ProductCode").val()) ,
        HeadSalesmanEmpName: $.trim($("#cms290_HeadSalesmanEmpName").val()) ,
        ProjectManagerEmpName: $.trim($("#cms290_ProjectManagerEmpName").val()) 
    };

    call_ajax_method(
        '/Common/CMS290_CheckReqField',
        CMS290_searchCond,
        function (result, controls) {
            if (result != undefined) {
                CMS290_search(CMS290_searchCond);
            }
        }
    );
}

function CMS290_search(CMS290_searchCond) {
    $("#cms290_btnSearchByCondition").attr("disabled", true);
    $("#cms290_btnSearchByCode").attr("disabled", true);
    master_event.LockWindow(true);

    $("#cms290_grid_result").LoadDataToGrid(CMS290_mygrid, CMS290_pageRow, true, "/Common/CMS290_Search", CMS290_searchCond, "dtProjectData", false,
        function (result, controls, isWarning) {
            $("#cms290_btnSearchByCondition").removeAttr("disabled");
            $("#cms290_btnSearchByCode").removeAttr("disabled");
            master_event.LockWindow(false);

            if (result != undefined) {
                master_event.ScrollWindow("#CMS290_SearchResult", true);
            }

        }, //post-load
        function (result, controls, isWarning) { //pre-load
            if (isWarning == undefined) {
                $("#CMS290_SearchResult").show();
            }
        });
}

function CMS290_clearSearchCriteria() {
    
    $("#cms290_ProjectCode").val("");
    $("#cms290_ContractCode").val("");
    $("#cms290_ProjectName").val("");
    $("#cms290_ProjectAddress").val("");
    $("#cms290_PJPurchaseName").val("");
    $("#cms290_Owner1Name").val("");
    $("#cms290_PJManagementCompanyName").val("");
    $("#cms290_OtherProjectRelatedPersonName").val("");
    $("#cms290_ProductCode").val("");
    $("#cms290_HeadSalesmanEmpName").val("");
    $("#cms290_ProjectManagerEmpName").val("");

    CloseWarningDialog();

    //initial();
}

function CMS290_doSelectAction(rid) {

    //hilight row
    CMS290_mygrid.selectRow(CMS290_mygrid.getRowIndex(rid));

    var col = CMS290_mygrid.getColIndexById('ToJson');
    var strJson = CMS290_mygrid.cells(rid, col).getValue().toString();
    strJson = htmlDecode(strJson);
    var cms290_object = JSON.parse(strJson);

    if (typeof (CMS290Response) == "function")
        CMS290Response(cms290_object);
}
