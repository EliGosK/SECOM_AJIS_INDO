/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

var mygrid_cms210;
var pageRow = ROWS_PER_PAGE_FOR_VIEWPAGE;


$(document).ready(function () {

    initialPage();


    // Get parameter data
    var objParam = "";
    var parameter = "";
    if (typeof (CMS210Object) == "function") {
        objParam = CMS210Object();
        parameter = {
            "ContractCode": objParam.ContractCode,
            "MATargetContractCode": objParam.MATargetContractCode,
            "ProductCode": objParam.ProductCode
        };

    }


    /* ---------- Load data to grid ------- */
    if ($.find("#mygrid_cms210").length > 0) {


        mygrid_cms210 = $("#mygrid_cms210").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, true, true, "/Common/CMS210_GetMaintCheckUpResultList", parameter, "View_dtMaintCheckUpResultList", false);

        SpecialGridControl(mygrid_cms210, ["Button"]);

        /* ==== Even when grid loading data is finished === */
        BindOnLoadedEvent(mygrid_cms210, function (gen_ctrl) {
            //var colInx = mygrid_cms210.getColIndexById('Button');

            for (var i = 0; i < mygrid_cms210.getRowsNum(); i++) {
                var row_id = mygrid_cms210.getRowId(i);

                if (gen_ctrl == true) {
                    GenerateDetailButton(mygrid_cms210, "btnDetail", row_id, "Button", true);

                    var year_month = mygrid_cms210.cells2(i, mygrid_cms210.getColIndexById('YearMonth')).getValue();

                    //htmlDecode
                    year_month = htmlDecode(year_month);

                    mygrid_cms210.cells2(i, mygrid_cms210.getColIndexById('YearMonth')).setValue(year_month);
                }

                // binding grid button event 
                BindGridButtonClickEvent("btnDetail", row_id, doSelect);

                if (i == mygrid_cms210.getRowsNum() - 1) {
                    mygrid_cms210.setSizes();
                }
            }


        });




    }

    // Set null value to "-"
    $("#divAll").SetEmptyViewData();

    if (txtRentalAttachImportanceFlag == true) {
        $("#ChkRentalAttachImportanceFlag").attr("checked", true);
    }
    if (txtSaleAttachImportanceFlag == true) {
        $("#ChkSaleAttachImportanceFlag").attr("checked", true);
    }
});




/* --- Methods --- */
function initialPage() {
    $("#DivMaintenanceCheckupInfo").hide();
}


/* -----------  event ----------- */
// ind = colum index
function doSelect(row_id) {


    mygrid_cms210.selectRow(mygrid_cms210.getRowIndex(row_id));

    /* ==== Create json object for string json ==== */
    var strJson = mygrid_cms210.cells2(mygrid_cms210.getRowIndex(row_id), mygrid_cms210.getColIndexById('Object')).getValue().toString();
    strJson = htmlDecode(strJson);
    var objDetail = JSON.parse(strJson); // require json2.js

    //alert(strJson);

    // Fill detail to section: Maintenance check-up informdation
    fillMaintenanceCheckupInfo(objDetail);

   
}

function fillMaintenanceCheckupInfo(data) {
    $("#DivMaintenanceCheckupInfo").show();

    $("#txtCheckupYearMonth").html(data.InstructionDate);
    $("#txtMaintenanceDate").html(data.MaintenanceDate);
    $("#txtExpectedDate").html(data.ExpectedMaintenanceDate);
    $("#txtSubcontractor").html(data.SubcontractName);
    $("#txtPersonInchargeOfSubcontractor").html(data.PICName);
    $("#txtMaintenamceEmployee").html(data.MaintenamceEmployee);
    $("#txtCheckupUsageTime").html(data.UsageTime);

    if (data.InstrumentMalfunctionFlag == "1") {
        $("#chkInstrumentMalfunction").attr('checked', true);
    }
    else {
        $("#chkInstrumentMalfunction").attr('checked', false);
    }

    if (data.NeedSalesmanFlag == "1") {
        $("#chkNeedContactSaleman").attr('checked', true);
    }
    else {
        $("#chkNeedContactSaleman").attr('checked', false);
    }


    $("#txtLocation").html(data.Location);
    $("#txtMalfunctionSalesmanInfo").html(data.MalfunctionDetail);
    $("#txtRemark").html(data.Remark);

    $("#DivMaintenanceCheckupInfo").SetEmptyViewData();

}

function CMS210Initial() {
    ChangeDialogButtonText(["Close"], [$('#btnClose_CMS210').val()]);
}
