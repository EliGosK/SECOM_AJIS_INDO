/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>

/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>
/// <reference path="../../Scripts/Base/AutoComplete.js"/>
/// <reference path="../jquery-1.5.1-vsdoc.js" />
/// <reference path="../Base/Master.js" />




//TODO: Function for Unit test,please remove after test : Nattapong N.
function LoginForTest() {
    var Cond = { "Cond.EmployeeNo": "420022", "Cond.Password": "XXXXXX" };
    call_ajax_method_json('/Common/Login', Cond, function (result) {
        if (result != null) {
            window.location.href = ajax_method.GenerateURL(result);
        }
    });
}
$(document).ready(function () {
    //  LoginForTest();
    if (window.location.search.replace("?timeout=", "") == 1) {
        $("#SessionError").show();
    }
    else {

        $("#SessionError").hide();
    }

    call_ajax_method_json("/common/CMS010_CHECK", "", function (result, controls) {
        if (result != null) {
            if (typeof (result) == "string") {
                window.location.href = ajax_method.GenerateURL(result);
            }
        }
    });

    $("#btnLogin").click(function () {
        var Cond = { "Cond.EmployeeNo": $("#EmpNo").val(), "Cond.Password": $("#Password").val() };
        call_ajax_method_json('/Common/Login', Cond, function (result) {
            if (result != null) {
                if (typeof (result) == "string") {
                    window.location.href = ajax_method.GenerateURL(result);
                }
                else {
                    var link = "/" + result.Controller + "/" + result.ScreenID;

                    var obj = "";
                    if (result.Parameters != undefined) {
                        obj = new Object();
                        for (var i = 0; i < result.Parameters.length; i++) {
                            obj[result.Parameters[i]] = result.Values[i];
                        }
                    }
                    ajax_method.CallScreenControllerWithAuthority(link, obj, false);
                }
            }
        });
    });

    $("#btnClear").click(function () {
        $("#EmpNo").val("");
        $("#Password").val("");
    });
    $("#EmpNo").focus();

});