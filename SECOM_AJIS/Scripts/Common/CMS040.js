/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>


// Main
$(document).ready(function () {
    // Do somethinge


    $("#btnSuspendServiceNow").click(
        function () {

            // OpenYesNoMessageDialog
            var message;
            var param = { "module": "Common", "code": "MSG0056" };
            call_ajax_method("/Shared/GetMessage", param, function (data) {

                /* ====== Open confirm dialog =====*/
                OpenYesNoMessageDialog(data.Code, data.Message, doSuspendServiceNow);

            });


        }
     );


    $("#btnResumeServiceNow").click(
        function () {

            // OpenYesNoMessageDialog
            var message;
            var param = { "module": "Common", "code": "MSG0057" };
            call_ajax_method("/Shared/GetMessage", param, function (data) {

                /* ====== Open confirm dialog =====*/
                OpenYesNoMessageDialog(data.Code, data.Message, doResumeServiceNow);
            });


        }
     );

    $("#btnUpdateSuspendTime").click(
        function () {
            doUpdateSuspendTime();

        }
     );


    $("#btnUpdateResumeTime").click(
        function () {
            doUpdateResumeTime();

        }
     );



    var status = CMS040_Object.SystemStatus;

    if (status.toUpperCase() == "OFFLINE") {
        $("#btnSuspendServiceNow").attr("disabled", true);
        $("#btnResumeServiceNow").attr("disabled", false);

    } else if (status.toUpperCase() == "ONLINE") {
        $("#btnSuspendServiceNow").attr("disabled", false);
        $("#btnResumeServiceNow").attr("disabled", true);
    }


});


/*--------- event ---------*/

// Suspend Now
function doSuspendServiceNow() {
    var parameter = { "UpdateType": "SUSPEND" }
    call_ajax_method("/Common/CMS040_UpdateSystemStatus", parameter, displaySystemStatus);

}

// Resume Now
function doResumeServiceNow() {
    var parameter = { "UpdateType": "RESUME" }
    call_ajax_method("/Common/CMS040_UpdateSystemStatus", parameter, displaySystemStatus);
}

// Update Suspend Time
function doUpdateSuspendTime() {
    // CMS040_UpdateSystemConfig
    // ex. // $("#Office").get(0).selectedIndex > 0;
    if ($("#SuspendServiceTime").get(0).selectedIndex > 0) {
        var parameter = { "UpdateType": "SUSPEND", "ServiceUpdateTime": $("#SuspendServiceTime").val().toString() }
        call_ajax_method("/Common/CMS040_UpdateSystemConfig", parameter, displaySystemStatus);
    }

}

// doUpdate Resume Time
function doUpdateResumeTime() {
    if ($("#ResumeServiceTime").get(0).selectedIndex > 0) {
        var parameter = { "UpdateType": "RESUME", "ServiceUpdateTime": $("#ResumeServiceTime").val().toString() }
        call_ajax_method("/Common/CMS040_UpdateSystemConfig", parameter, displaySystemStatus);
    }

}

/* ------------ method -------------*/
function displaySystemStatus(data) {

    if (data != undefined && data != null) {

        var isComplete = data[0].CompleteFlag; // is completed or not?

        if (isComplete) {
            $("#SystemStatus").val(data[0].SystemStatusDisplayName);
            $("#NextSuspendServiceTime").val(data[0].NextSuspendServiceDateTime);
            $("#NextResumeServiceTime").val(data[0].NextResumeServiceDateTime);

            var status = data[0].SystemStatus;

            if (status.toUpperCase() == "OFFLINE") {
                $("#btnSuspendServiceNow").attr("disabled", true);
                $("#btnResumeServiceNow").attr("disabled", false);

            } else if (status.toUpperCase() == "ONLINE") {
                $("#btnSuspendServiceNow").attr("disabled", false);
                $("#btnResumeServiceNow").attr("disabled", true);
            }

            // Show message - update system complete
            var param = "";
            if (data[0].UpdateType.toUpperCase() == "SUSPEND") {
                param = { "module": "Common", "code": "MSG0143" };
            }
            else if (data[0].UpdateType.toUpperCase() == "RESUME") {
                param = { "module": "Common", "code": "MSG0144" };
            }

            call_ajax_method_json("/Shared/GetMessage", param, function (data) {
                /* ====== Open information dialog =====*/
                OpenInformationMessageDialog(data.Code, data.Message);
            });


        }
        else {

            // Show message - update system unsuccesful.
            var param = "";
            if (data[0].UpdateType.toUpperCase() == "SUSPEND") {
                param = { "module": "Common", "code": "MSG0145" };
            }
            else if (data[0].UpdateType.toUpperCase() == "RESUME") {
                param = { "module": "Common", "code": "MSG0146" };
            }

            call_ajax_method_json("/Shared/GetMessage", param, function (data) {
                /* ====== Open error dialog =====*/
                OpenErrorMessageDialog(data.Code, data.Message);
            });

        }

        

    }

}