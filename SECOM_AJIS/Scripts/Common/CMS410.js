/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

/// <reference path="../Base/object/ajax_method.js" />




$(document).ready(function () {

    // Initial grid

    // Binding gird event

    // Binding event ex. button
    

    IntialPage();

    IntialLink();

});

function IntialPage() {

    // Set null value to "-"  ***
    $("#divAll").SetEmptyViewData();
}

function IntialLink() { 

    $("#btnBillingBasicList").click(function () {
        var obj = { "BillingTargetCode": CMS410.BillingTargetCode, "isEnableBtnShowInvoiceList": false };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS400", obj, true);
    });

    $("#btnInvoiceList").click(function () {
        var obj = { "BillingTargetCode": CMS410.BillingTargetCode, "isEnableBtnShowInvoiceList": true };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS400", obj, true);
    });

    $("#btnBillingDetailList").click(function () {
        var obj = { "BillingTargetCode": CMS410.BillingTargetCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS450", obj, true);
    });

    $("#btnEditInformation").click(function () {
        var obj = { "BillingTargetCode": CMS410.BillingTargetCode };
        ajax_method.CallScreenControllerWithAuthority("/Billing/BLS020", obj, true);

    });
}


