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
InstrumentQty = "InstrumentGrid";

var grdSearchResult = null;

$(document).ready(function () {
    InitialDateFromToControl("#txtSearchCompleteDateFrom", "#txtSearchCompleteDateTo");

    initScreen();
    initButton();
    initEvent();
});

function initScreen() {
    $("#ReceivingReturnInstrument").clearForm();

    $("#Detmemo").SetMaxLengthTextArea(1000);

    $("#ReceivingReturnInstrument").hide();
    SetConfirmCommand(false, cmdConfirm);
    SetCancelCommand(false, cmdCancel);

    if (InstGrid == null) {
        InstGrid = $("#InstrumentGrid").InitialGrid(0, false, "/inventory/IVS030_GetHeaderSlipDetail");
    }
    else {
        DeleteAllRow(InstGrid);
    }

    $("#frmSearchSlipNo").clearForm();
    ClearDateFromToControl("#txtSearchCompleteDateFrom", "#txtSearchCompleteDateTo");

    if (grdSearchResult == null) {
        grdSearchResult = $("#divSearchResultGrid").InitialGrid(
            ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS030_InitialSearchResultGrid"
            , function () {
                BindOnLoadedEvent(grdSearchResult, grdSearchResult_OnLoadedData);
            }
        );
    }
    else {
        DeleteAllRow(grdSearchResult);
    }
    $("#divSearchResult").hide();

    $("#SlipNo").SetDisabled(true);
    $("#ShowSlipNo").hide();
}

function initButton() {
    $("#btnSearch").click(function () {
        master_event.LockWindow(true);

        var obj = $("#frmSearchSlipNo").serializeObject2();

        $("#divSearchResultGrid").LoadDataToGrid(grdSearchResult, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS030_SearchSlip", obj, "doSearchReceiveSlipResult", false
            , function (res) {
                master_event.LockWindow(false);
            }
            , function (result, controls, isWarning) { //pre-load
                if (isWarning == undefined) {
                    $("#divSearchResult").show();
                }
            }
        );

        return false;
    });

    $("#btnClear").click(function () {
        initScreen();
        return false;
    });

    $("#NewRegister").click(function () {
        initScreen();
    });

    $("#DownloadInvSlip").click(function () {
        var param = {
            strInstallationSlipNo: $("#SlipNo").val()
        };

        ajax_method.CallScreenController("/inventory/IVS030_DownloadDocument", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/inventory/IVS030_DownloadPdfAndWriteLog?k=" + key + "&strDocumentNo=" + result.DocumentNo + "&documentOCC=" + result.DocumentOCC + "&strDocumentCode=" + result.DocumentCode + "&fileName=" + result.FilePath)
                window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    });

}

function initEvent() {
}

function cmdConfirm() {
    master_event.LockWindow(true);
    command_control.CommandControlMode(false);

    var obj = {
        ApproveNo: $("#txtApproveNo").val(),
        Memo: $("#Detmemo").val()
    };

    ajax_method.CallScreenController("/inventory/IVS030_cmdConfirm", obj, function (result, controls) {
        master_event.LockWindow(false);
        if (!result) {
            if (controls != undefined) {
                if (controls[0] == "Detmemo") {
                    VaridateCtrl_AtLeast(["Detmemo"], controls);
                }
                else {
                    VaridateCtrl_AtLeast(["SlipNo"], controls);
                }
            }
        }
        else {
            var objMsg = {
                module: "Inventory",
                code: "MSG4096"
            };
            call_ajax_method("/Shared/GetMessage", objMsg, function (result) {
                OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    master_event.LockWindow(true);
                    command_control.CommandControlMode(false);
                    ajax_method.CallScreenController("/inventory/IVS030_cmdConfirm_Cont", obj, function (result, controls) {
                        if (result != undefined) {
                            $("#SlipNo").val(result.SlipNo);
                            master_event.LockWindow(false);
                            OpenInformationMessageDialog(result.Message.Code, result.Message.Message, function () {
                                $("#Detmemo").SetDisabled(true);
                                $("#txtApproveNo").SetDisabled(true);
                                $("#ShowSlipNo").show();
                                master_event.ScrollWindow("#ShowSlipNo");
                                SetConfirmCommand(false, cmdConfirm);
                                SetCancelCommand(false, cmdCancel);
                            });
                        }
                    });
                },
                null);
            });
        }
    });

    command_control.CommandControlMode(true);
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


function grdSearchResult_OnLoadedData(gen_ctrl) {
    var grid = grdSearchResult;

    for (var i = 0; i < grid.getRowsNum(); i++) {
        var row_id = grid.getRowId(i);
        if (gen_ctrl) {
            GenerateDetailButton(grid, "DetailButton", row_id, "DetailButton", true);
        }

        BindGridButtonClickEvent("DetailButton", row_id, grdSearchResult_OnDetail);
    }

    grid.setSizes();
}

function grdSearchResult_OnDetail(row_id) {
    master_event.LockWindow(true);

    $("#Detmemo").SetDisabled(false);
    $("#txtApproveNo").SetDisabled(false);
    $("#ShowSlipNo").hide();

    var grid = grdSearchResult;
    var row_index = grid.getRowIndex(row_id);
    grid.selectRow(row_index);

    var InstallationSlipNo = grid.cells(row_id, grid.getColIndexById("InstallationSlipNo")).getValue();
    var ProjectReturnSlipNo = grid.cells(row_id, grid.getColIndexById("ProjectCode")).getValue();
    var slip = '';
    var type = "0";

    if (InstallationSlipNo != "") {
        slip = InstallationSlipNo;
        type = "1";
    }
    else if (ProjectReturnSlipNo != "") {
        slip = ProjectReturnSlipNo;
        type = "2";
    }

    var objSlipNo = { SlipNo: slip, SlipSelectType: type };

    ajax_method.CallScreenController("/inventory/IVS030_RetrieveSlipData", objSlipNo, function (result, controls) {
        if (controls != undefined) {
            var con = new Array();
            VaridateCtrl_AtLeast(["SlipNo"], controls);

            master_event.LockWindow(false);
        }
        else if (controls == undefined && result != null) {
            $("#ReceivingReturnInstrument").show();
            $("#ContractCode").val(result.ContractCode);
            $("#SiteName").val(result.SiteName);
            $("#InstallationType").val(result.InstallationTypeName);
            $("#ApproveNo1").val(result.ApproveNo1);
            $("#ApproveNo2").val(result.ApproveNo2);

            if (type == "1") {
                $("#InstrumentGrid").LoadDataToGrid(InstGrid, 0, false, "/inventory/IVS030_GetReturnInstrumentByInstallationSlip", objSlipNo, "doResultReturnInstrument", false
                    , function () { }
                    , function () { }
                );
            }
            else if (type == "2") {
                $("#InstrumentGrid").LoadDataToGrid(InstGrid, 0, false, "/inventory/IVS030_GetTbt_InventorySlipDetailForView", objSlipNo, "doTbt_InventorySlipDetailForView", false
                    , function () { }
                    , function () { }
                );
            }

            master_event.LockWindow(false);

            SetConfirmCommand(true, cmdConfirm);
            SetCancelCommand(true, cmdCancel);
        }
    });
}