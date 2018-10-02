/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../Base/GridControl.js" />

/// <reference path="../Base/DateTimePicker.js" />
/// <reference path="../Base/control_events.js" />
var InstGrid = null;
InstrumentQty = "TransferQty";

$(document).ready(function () {
    initScreen();
    initButton();
    initEvent();
});

function initScreen() {
    $("#SlipNo").val("");
    $("#RepairRequestInstrument").clearForm();    
    $("#SlipNo").removeAttr("disabled");
    $("#btnRetrieve").removeAttr("disabled");
    $("#RepairRequestInstrument").hide();
    SetConfirmCommand(false, cmdConfirm);
    SetCancelCommand(false, cmdCancel);
    InstGrid = $("#InstrumentGrid").InitialGrid(0, false, "/inventory/IVS110_GetHeaderSlipDetail"); 
}

function initButton() {
    $("#btnRetrieve").click(function () {
        master_event.LockWindow(true);
        $("#btnRetrieve").attr("disabled", true);
        var Slip = $("#SlipNo").val();
        var objSlipNo = { SlipNo: Slip };

        ajax_method.CallScreenController("/inventory/IVS110_RetrieveRequestInSlip", objSlipNo, function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl_AtLeast(["SlipNo"], controls);

                $("#btnRetrieve").attr("disabled", false);
                master_event.LockWindow(false);
            }
            else if(controls == undefined && result != null){
                $("#RepairRequestInstrument").show();

                $("#SourceLocation").val(result.SourceLocationName);
                $("#SourceLocationCode").val(result.SourceLocationCode);
                $("#DestinationLocation").val(result.DestinationLocationName);
                $("#DestinationLocationCode").val(result.DestinationLocationCode);
                $("#ApproveNo").val(result.ApproveNo);
                $("#RepairSubConTractor").val(result.RepairSubcontractor);
                $("#Detmemo").val(result.Memo);

                $("#InstrumentGrid").LoadDataToGrid(InstGrid, 0, false, "/inventory/IVS110_RetrieveRequestSlipDetail", objSlipNo, "doInventorySlipDetail", false, null, null);

                master_event.LockWindow(false);
                $("#SlipNo").attr("disabled", true);                
                SetConfirmCommand(true, cmdConfirm);
                SetCancelCommand(true, cmdCancel);
            }
        });
    });
}

function initEvent() {    
}

function cmdConfirm() {
    command_control.CommandControlMode(false);
    var InstList = new Array();
    for (var i = 0; i < InstGrid.getRowsNum(); i++) {
        var row_id = InstGrid.getRowId(i);
        var Inst = {
            InstrumentCode: InstGrid.cells(row_id, InstGrid.getColIndexById("InstrumentCode")).getValue(),
            InstrumentName: InstGrid.cells(row_id, InstGrid.getColIndexById("InstrumentName")).getValue(),
            DestinationAreaCode: InstGrid.cells(row_id, InstGrid.getColIndexById("DestinationAreaCode")).getValue(),
            DestinationShelfNo: InstGrid.cells(row_id, InstGrid.getColIndexById("DestinationShelfNo")).getValue(),
            TransferQty: InstGrid.cells(row_id, InstGrid.getColIndexById("TransferQty")).getValue(),
            row_id: row_id
        };
        InstList.push(Inst);
    }


    var obj = { StockInInstrument: InstList
    };

    ajax_method.CallScreenController("/inventory/IVS110_cmdConfirm", obj, function (result, controls) {
        if (result != null && result != "") {
            InstGrid.selectRowById(result);
        }
        else {
            var objMsg = {
                module: "Inventory",
                code: "MSG4013"
            };
            call_ajax_method("/Shared/GetMessage", objMsg, function (result) {
                OpenYesNoMessageDialog(result.Code, result.Message,
            function () {
                command_control.CommandControlMode(false);              
                ajax_method.CallScreenController("/inventory/IVS110_cmdConfirm_Cont", null, function (result, controls) {
                    if (result != undefined) {
                        OpenInformationMessageDialog(result.Code, result.Message, function () {
                            initScreen();
                        });
                    }
                });
            },
            null);
            });
    }
    command_control.CommandControlMode(true);
    });
}

function cmdCancel() {
    var obj = {
        module: "Common",
        code: "MSG0038"
    };
    call_ajax_method("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
        function () {
            initScreen();
        },
        null);
    });
}

function VaridateCtrl_AtLeast(ctrl_lst, null_ctrl) {
    if (ctrl_lst != null) {
        for (var idx = 0; idx < ctrl_lst.length; idx++) {
            var ctrl = $("#" + ctrl_lst[idx]);
            if (ctrl.length > 0) {
                ctrl.removeClass("highlight");


                if (ctrl[0].tagName.toLowerCase() == "select") {
                    var unb = function () {
                        for (var i = 0; i < ctrl_lst.length; i++) {
                            $("#" + ctrl_lst[i]).removeClass("highlight");
                            $("#" + ctrl_lst[i]).unbind("change", unb);
                        }
                    };
                    ctrl.change(unb);
                }
                else {
                    var unb = function () {
                        for (var i = 0; i < ctrl_lst.length; i++) {
                            $("#" + ctrl_lst[i]).removeClass("highlight");
                            $("#" + ctrl_lst[i]).unbind("keyup", unb);
                        }
                        // $(this).removeClass("highlight");
                        //  $(this).unbind("keyup", unb);
                    };
                    ctrl.keyup(unb);


                }
            }
        }
    }
    if (null_ctrl != null) {
        for (var idx = 0; idx < null_ctrl.length; idx++) {
            if (null_ctrl[idx] != "") {
                var ctrl = $("#" + null_ctrl[idx]);
                if (ctrl.length > 0) {
                    ctrl.addClass("highlight");
                }
            }
        }
    }

}