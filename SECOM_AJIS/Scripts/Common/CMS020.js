/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>
/// <reference path="../../Scripts/Base/AutoComplete.js"/>
/// <reference path="../jquery-1.5.1-vsdoc.js" />

/// <reference path="../Base/Master.js" />
/// <reference path="../Base/object/ajax_method.js" />



var ss;

$(document).ready(function () {

    $("#ContractCode_Searbar").keyup(function (event) {
        if (event.which == 13) {
            $("#btnRetrieve_Searchbar").click();
            event.preventDefault();
        }
    });

    $("#UserCode_Searbar").keyup(function (event) {
        if (event.which == 13) {
            $("#btnRetrieve_Searchbar").click();
            event.preventDefault();
        }
    });

    $("#InvoiceNo_Searbar").keyup(function (event) {
        if (event.which == 13) {
            $("#btnRetrieve_Searchbar").click();
            event.preventDefault();
        }
    });

    $("#ProjectCode_Searbar").keyup(function (event) {
        if (event.which == 13) {
            $("#btnRetrieve_Searchbar").click();
            event.preventDefault();
        }
    });

    // Retrieve  --> Open CMS190(2) , CTS260  
    $("#btnRetrieve_Searchbar").click(function () {

        // disable button
        $("#btnRetrieve_Searchbar").attr("disabled", true);
        $("#btnSearch_Searchbar").attr("disabled", true);


        // Check user input
        var param = {
            "ContractCode": $("#ContractCode_Searbar").val(),
            "UserCode": $("#UserCode_Searbar").val(),
            "InvoiceNo": $("#InvoiceNo_Searbar").val(),
            "ProjectCode": $("#ProjectCode_Searbar").val()
        };

        call_ajax_method("/Common/CMS020_Retrive", param, function (result) {

            // enable button
            $("#btnRetrieve_Searchbar").attr("disabled", false);
            $("#btnSearch_Searchbar").attr("disabled", false);

            if (result != undefined) {
                /* --- Set condition --- */
                SEARCH_CONDITION = {
                    ContractCode: result.ContractCode,
                    ProjectCode: result.ProjectCode
                };
                /* --------------------- */

                // Mode
                var screenParam;

                if (result.Mode == "CMS190") {

                    screenParam = {
                        "strContractCode": result.ContractCode,
                        "strServiceTypeCode": result.ServiceType
                    };

                    ajax_method.CallScreenControllerWithAuthority("/Common/CMS190", screenParam, false);
                }
                else if (result.Mode == "CTS260") {

                    screenParam = {
                        "strProjectCode": result.ProjectCode
                    };

                    ajax_method.CallScreenControllerWithAuthority("/Contract/CTS260", screenParam, false);

                }
                else if (result.Mode == "CMS450") {
                    screenParam = {
                        "InvoiceNo": result.InvoiceNo
                    };

                    ajax_method.CallScreenControllerWithAuthority("/Common/CMS450", screenParam, false);
                }
            }
        });

    });

    // Search  --> Open CMS070 (like a link)
    $("#btnSearch_Searchbar").click(function () {

        // disable button
        $("#btnRetrieve_Searchbar").attr("disabled", true);
        $("#btnSearch_Searchbar").attr("disabled", true);

        ajax_method.CallScreenControllerWithAuthority("/Common/CMS070", false);

        // enable button
        $("#btnRetrieve_Searchbar").attr("disabled", false);
        $("#btnSearch_Searchbar").attr("disabled", false);
    });

});