

//  jQuery
/// <reference path="../../Scripts/jquery-1.5.1-vsdoc.js"/>

// dhtmlx grid
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgrid.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_data.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgridcell.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js"/>
/// <reference path = "../../Scripts/Base/Master.js" />
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Common/Dialog.js"/>
/// <reference path="../../Scripts/json2.js" />


/// <reference path = "../../Scripts/Base/GridControl.js" />



// Main
$(document).ready(function () {
    $("#btnCompleteInstallationRental").click(function () { BtnCompleteInstallRentalClick(); });
    $("#btnCompleteInstallationSale").click(function () { BtnCompleteInstallSaleClick(); });

});


function BtnCompleteInstallRentalClick() {
    var obj = { RentalContractCode: $("#RentalContractCode").val() };
    call_ajax_method_json("/Installation/ISS061_CompleteInstallRental", obj,
        function (result, controls) {
            var obj = {
                module: "Common",
                code: "MSG0046",
                param: ""
            };

            call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                OpenInformationMessageDialog(result.Code, result.Message);
            });
        }
        );
}

function BtnCompleteInstallSaleClick() {
    var obj = { SaleContractCode: $("#SaleContractCode").val() };
    call_ajax_method_json("/Installation/ISS061_CompleteInstallSale", obj,
        function (result, controls) {
            var obj = {
                module: "Common",
                code: "MSG0046",
                param: ""
            };

            call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                OpenInformationMessageDialog(result.Code, result.Message);
            });
        }
        );
}

    