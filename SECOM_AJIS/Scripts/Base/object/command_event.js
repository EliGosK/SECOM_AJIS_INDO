var command_control = $.extend({
    DiabledControls: null,

    CommandControlMode: function (enable) {
        $("#divCommandControl button").each(function () {
            if (enable == false) {
                if ($(this).prop("disabled") == true) {
                    var id = $(this).attr("id");
                    var addNew = true;
                    if (command_control.DiabledControls == undefined) {
                        command_control.DiabledControls = new Array();
                    }
                    else {
                        for (var i = 0; i < command_control.DiabledControls.length; i++) {
                            if (command_control.DiabledControls[i] == id) {
                                addNew = false;
                                break;
                            }
                        }
                    }
                    if (addNew == true) {
                        command_control.DiabledControls.push(id);
                    }
                }
                else {
                    $(this).attr("disabled", true);
                }
            }
            else {
                var disabledIdx = -1;
                if (command_control.DiabledControls != undefined) {
                    var id = $(this).attr("id");
                    for (var i = 0; i < command_control.DiabledControls.length; i++) {
                        if (command_control.DiabledControls[i] == id) {
                            disabledIdx = i;
                            break;
                        }
                    }
                }

                if (disabledIdx == -1) {
                    $(this).removeAttr("disabled");
                }
                else {
                    command_control.DiabledControls.splice(disabledIdx, 1);
                    if (command_control.DiabledControls.length == 0)
                        command_control.DiabledControls = null;
                }
            }
        });
    },
    CommandControlEvent: function (ctrl) {
        this.Control = ctrl;
        this.SetCommand = function (func) {
            var ctrlC = $("#" + this.Control);

            ctrlC.unbind("click");
            if (func != undefined && typeof (func) == "function") {
                ctrlC.show();
                this.EnableCommand(true);

                ctrlC.click(function () {
                    command_control.CommandControlMode(false);
                    func();
                });
            }
            else {
                ctrlC.hide();
            }
        };
        this.EnableCommand = function (enable) {
            var ctrlC = $("#" + this.Control);

            if (enable) {
                ctrlC.removeAttr("disabled");

                //                if (command_control.DiabledControls != undefined) {
                //                    var id = ctrlC.attr("id");
                //                    var idx = -1;
                //                    for (var i = 0; i < command_control.DiabledControls.length; i++) {
                //                        if (command_control.DiabledControls[i] == id) {
                //                            idx = i;
                //                            break;
                //                        }
                //                    }

                //                    if (idx >= 0) {
                //                        command_control.DiabledControls.splice(idx, 1);
                //                    }
                //                }
            }
            else {
                ctrlC.attr("disabled", true);

                //                var id = ctrlC.attr("id");
                //                var addNew = true;
                //                if (command_control.DiabledControls == undefined) {
                //                    command_control.DiabledControls = new Array();
                //                }
                //                else {
                //                    for (var i = 0; i < command_control.DiabledControls.length; i++) {
                //                        if (command_control.DiabledControls[i] == id) {
                //                            addNew = false;
                //                            break;
                //                        }
                //                    }
                //                }
                //                if (addNew == true) {
                //                    command_control.DiabledControls.push(id);
                //                }
            }
        };
    }
});


var register_command = new command_control.CommandControlEvent("btnCommandRegister");

/* --- Merge --- */
/* var required_approve_command = new command_control.CommandControlEvent("btnCommandRequestApprove"); */
var request_approve_command = new command_control.CommandControlEvent("btnCommandRequestApprove");
/* ------------- */

var reset_command = new command_control.CommandControlEvent("btnCommandReset");

reset_command.SetCommand = function (func) {
    var ctrlC = $("#" + this.Control);

    ctrlC.unbind("click");
    if (func != undefined && typeof (func) == "function") {
        ctrlC.show();
        ctrlC.removeAttr("disabled");
        ctrlC.click(function () {
            command_control.CommandControlMode(false);

            var obj = {
                module: "Common",
                code: "MSG0038"
            };
            ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
                OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    ajax_method.CallScreenController("/Shared/ResetSessionData", "", function () {
                        func();
                    });
                },
                function () {
                    command_control.CommandControlMode(true);
                });
            });
        });
    }
    else {
        ctrlC.hide();
    }
}

var last_complete_command = new command_control.CommandControlEvent("btnCommandLastComplete");
var cancel_pcode_command = new command_control.CommandControlEvent("btnCommandCancelPcode");
var approve_command = new command_control.CommandControlEvent("btnCommandApprove");
var reject_command = new command_control.CommandControlEvent("btnCommandReject");
var confirm_command = new command_control.CommandControlEvent("btnCommandConfirm");
var back_command = new command_control.CommandControlEvent("btnCommandBack");
var clear_command = new command_control.CommandControlEvent("btnCommandClear");
var ok_command = new command_control.CommandControlEvent("btnCommandOK");
var cancel_command = new command_control.CommandControlEvent("btnCommandCancel");
var purge_command = new command_control.CommandControlEvent("btnCommandPurge");
var edit_command = new command_control.CommandControlEvent("btnCommandEdit");
var return_command = new command_control.CommandControlEvent("btnCommandReturn");
var close_command = new command_control.CommandControlEvent("btnCommandClose");

/* --- Merge --- */
var import_command = new command_control.CommandControlEvent("btnCommandImport"); // Additional by P'Budd (4/4/2012)
var confirmCancel_command = new command_control.CommandControlEvent("btnCommandConfirmCancel"); // Additional by P'Budd (17/May/2012)
/* ------------- */

