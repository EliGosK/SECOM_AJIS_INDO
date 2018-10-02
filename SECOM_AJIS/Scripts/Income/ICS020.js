/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/control_events.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>

/// <reference path="../../Scripts/Base/ComboBox.js"/>
/// <reference path="../Base/object/ajax_method.js" />
/// <reference path="../Base/object/command_event.js" />
/// <reference path="../Base/object/master_event.js" />

var ics020_var = {
    isShowContentAllSuccess: false,
    objAutoTranferGrid: null,
    objBankTranferGird: null
};
ics020_var.screenMode = {
    importMode: "ImportMode",
    finishMode: "FinishMode"
};
ics020_var.selectProcess = {
    autoTransfer: "AutoTransfer",
    bankTransfer: "BankTransfer"
};
ics020_var.controls = {
    divSelectProcess: "#divSelectProcess",
    divBankBranch: "#divBankBranch",
    divFileName: "#divFileName",
    divAutoTransfer: "#divAutoTransfer",
    divBankTransfer: "#divBankTransfer",
    frmSelectProcess: "#frmSelectProcess",
    rdoAutoTransfer: "#rdoAutoTransfer",
    rdobankTransfer: "#rdoBankTransfer",
    bankBranch: "#SECOMAccountID",
    autoTransferGrid: "#AutoTransferGrid",
    bankTransferGrid: "#BankTransferGrid",
    btnNextAutoImport: "#btnNextAutoImport",
    btnNextBankImport: "#btnNextBankImport",

    iFrameAttach: "#iframeAttach",
    btnShowContent: "#ICS020_btnShowContent",

    CurrencyType: "#CurrencyType"
};

ics020_var.iframeAttachControls = {
    formUpload: "#formUpload",
    fileSelect: "#fileSelect",
    documentName: "#DocumentName"
};


var ics020_process = {
    GetSelectProcess: function () {
        var selectProcess = null;
        if ($(ics020_var.controls.rdoAutoTransfer).prop("checked") == true) {
            selectProcess = ics020_var.selectProcess.autoTransfer;
        }
        else if ($(ics020_var.controls.rdobankTransfer).prop("checked") == true) {
            selectProcess = ics020_var.selectProcess.bankTransfer;
        }
        return selectProcess;
    },

    SelectProcess_OnChanged: function () {
        var obj = { SelectProcess: ics020_process.GetSelectProcess() };
        ajax_method.CallScreenController("/Income/ICS020_GetBankBranch", obj, function (data) {
            regenerate_combo(ics020_var.controls.bankBranch, data);
        });
    },

    ShowContent_OnClick: function () {
        ics020_var.isShowContentAllSuccess = false;
        //Hide grid
        $(ics020_var.controls.divAutoTransfer).hide();
        $(ics020_var.controls.divBankTransfer).hide();

        //Validate
        var ctrl = new Array();
        var controlname = "";

        if ($(ics020_var.controls.bankBranch).val() == "") {
            var lblBankBranch = $(ics020_var.controls.divBankBranch).html();
            controlname += lblBankBranch;
            ctrl.push(ics020_var.controls.bankBranch.substring(1, ics020_var.controls.bankBranch.length));
        }
        var inputfile = $(ics020_var.controls.iFrameAttach).contents().find(ics020_var.iframeAttachControls.fileSelect);
        if (inputfile == undefined || inputfile.val() == "") {
            var lblFileName = $(ics020_var.controls.divFileName).html();
            controlname += (controlname == "" ? "" : ",") + lblFileName;
            ctrl.push(ics020_var.iframeAttachControls.fileSelect.substring(1, ics020_var.iframeAttachControls.fileSelect.length));
        }

        if (controlname != "") {
            //Show warning dialog
            var objMsg = { module: "Common", code: "MSG0007", param: controlname };
            call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                OpenWarningDialog(resultMsg.Message);
                VaridateCtrl(ctrl, ctrl);
            });
        }
        else {
            //Submit "attach file" first, 
            //if success ICS020_upload.cshtml will call function SubmitShowContent_OnClick(), If fail will call function SubmitShowContent_OnError()
            $(ics020_var.controls.btnShowContent).SetDisabled(true);
            $(ics020_var.controls.iFrameAttach).contents().find(ics020_var.iframeAttachControls.formUpload).submit();
        }
    },

    SubmitShowContent_OnClick: function () {
        //Called by ics020_upload.cshtml
        //Already upload file to server

        //Disable control
        $(ics020_var.controls.rdoAutoTransfer).SetDisabled(true);
        $(ics020_var.controls.rdobankTransfer).SetDisabled(true);
        $(ics020_var.controls.bankBranch).SetDisabled(true);
        $(ics020_var.controls.iFrameAttach).contents().find(ics020_var.iframeAttachControls.fileSelect).SetDisabled(true);
        $(ics020_var.controls.divAutoTransfer).hide();
        $(ics020_var.controls.divBankTransfer).hide();


        //Show content to grid
        var selectProcess = ics020_process.GetSelectProcess();
        var objparm = {
            SelectProcess: selectProcess,
            SECOMAccountID: $(ics020_var.controls.bankBranch).val(),
            CurrencyType: $(ics020_var.controls.CurrencyType).val()
        };

        ajax_method.CallScreenController("/Income/ICS020_SubmitShowContentToGrid", objparm, function (result, controls, isWarning) {
            if (controls != undefined) {
                VaridateCtrl(controls, controls);
            }

            if (result == "1") {
                //Success
                var gridControl;
                var gridObject;
                var gridDiv;
                var selectProcess = ics020_process.GetSelectProcess();
                if (selectProcess == ics020_var.selectProcess.autoTransfer) {
                    gridControl = $(ics020_var.controls.autoTransferGrid);
                    gridObject = ics020_var.objAutoTranferGrid;
                    gridDiv = $(ics020_var.controls.divAutoTransfer);
                }
                else if (selectProcess == ics020_var.selectProcess.bankTransfer) {
                    gridControl = $(ics020_var.controls.bankTransferGrid);
                    gridObject = ics020_var.objBankTranferGird;
                    gridDiv = $(ics020_var.controls.divBankTransfer);
                }

                gridControl.LoadDataToGrid(gridObject, 0, false, "/Income/ICS020_ShowContentToGrid", "", "tbt_tmpImportContent", false,
                    function (result, controls, isWarning) {
                        if (controls != undefined) {
                            VaridateCtrl(controls, controls);
                        }
                    },
                    function (result, controls, isWarning) {
                    });

                gridDiv.show();
                ics020_var.isShowContentAllSuccess = true;
                $(ics020_var.controls.iFrameAttach).contents().find(ics020_var.iframeAttachControls.fileSelect).hide();
                $(ics020_var.controls.iFrameAttach).contents().find(ics020_var.iframeAttachControls.documentName).show();
            }
            else {
                //Enable control
                $(ics020_var.controls.rdoAutoTransfer).SetDisabled(false);
                $(ics020_var.controls.rdobankTransfer).SetDisabled(false);
                $(ics020_var.controls.bankBranch).SetDisabled(false);
                $(ics020_var.controls.iFrameAttach).contents().find(ics020_var.iframeAttachControls.fileSelect).SetDisabled(false);
                $(ics020_var.controls.btnShowContent).SetDisabled(false);
            }
        });
    },

    SubmitShowContent_OnError: function (msgCode, msg) {
        OpenErrorMessageDialog(msgCode, msg, function () {
            //Enable show content button
            $(ics020_var.controls.btnShowContent).SetDisabled(false);
        });
    },



    CmdImport_OnClick: function () {
        //Validate
        if (ics020_var.isShowContentAllSuccess == false) {
            var objMsg = { module: "Income", code: "MSG7074" };
            call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                OpenErrorMessageDialog("MSG7074", resultMsg.Message, function () { });
            });
        }
        else {
            var obj = {
                module: "Common",
                code: "MSG0028",
                param: [$("#btnCommandImport").html()]
            };
            ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
                OpenYesNoMessageDialog(result.Code, result.Message,
                    function () {
                        command_control.CommandControlMode(false);

                        //Import
                        var selectProcess = ics020_process.GetSelectProcess();

                        ajax_method.CallScreenController("/Income/ICS020_CmdImport", "", function (result, controls, isWarning) {
                            if (controls != undefined) {
                                VaridateCtrl(controls, controls);
                            }

                            var gridControl;
                            var gridObject;
                            var gridDiv;
                            if (selectProcess == ics020_var.selectProcess.autoTransfer) {
                                gridControl = $(ics020_var.controls.autoTransferGrid);
                                gridObject = ics020_var.objAutoTranferGrid;
                                gridDiv = $(ics020_var.controls.divAutoTransfer);
                            }
                            else if (selectProcess == ics020_var.selectProcess.bankTransfer) {
                                gridControl = $(ics020_var.controls.bankTransferGrid);
                                gridObject = ics020_var.objBankTranferGird;
                                gridDiv = $(ics020_var.controls.divBankTransfer);
                            }

                            gridControl.LoadDataToGrid(gridObject, 0, false, "/Income/ICS020_ShowContentToGrid", "", "tbt_tmpImportContent", false,
                                    function (result, controls, isWarning) {
                                        if (controls != undefined) {
                                            VaridateCtrl(controls, controls);
                                        }
                                    },
                                    function (result, controls, isWarning) {
                                    });

                            if (result == "1") {
                                var objMsg = { module: "Income", code: "MSG7008" };
                                call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                                    OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {
                                        //Same as reset screen
                                        ics020_process.SetScreenMode(ics020_var.screenMode.finishMode);
                                    });
                                });
                            }
                        });

                    }
                    , null);
            });
        }
    },

    CmdReset_OnClick: function () {
        //Clear input
        ics020_var.isShowContentAllSuccess = false;

        $(ics020_var.controls.divSelectProcess).clearForm();
        $(ics020_var.controls.rdobankTransfer).prop("checked", true);
        var inputfile = $(ics020_var.controls.iFrameAttach).contents().find(ics020_var.iframeAttachControls.fileSelect);
        if ($.browser.msie) {
            inputfile.replaceWith(inputfile.clone());
        }
        else {
            inputfile.val('');
        }
        $(ics020_var.controls.iFrameAttach).contents().find(ics020_var.iframeAttachControls.documentName).val('');

        //Enable/Disable control
        $(ics020_var.controls.rdoAutoTransfer).SetDisabled(false);
        $(ics020_var.controls.rdobankTransfer).SetDisabled(false);
        $(ics020_var.controls.bankBranch).SetDisabled(false);
        $(ics020_var.controls.iFrameAttach).contents().find(ics020_var.iframeAttachControls.fileSelect).SetDisabled(false);
        $(ics020_var.controls.btnShowContent).SetDisabled(false);

        //Clear grid
        var selectProcess = ics020_process.GetSelectProcess();
        var obj = { SelectProcess: ics020_process.GetSelectProcess() };
        var gridName = null;
        var gridObj = null;
        if (selectProcess == ics020_var.selectProcess.autoTransfer) {
            gridName = ics020_var.controls.autoTransferGrid;
            gridObj = ics020_var.objAutoTranferGrid;
        }
        else if (selectProcess == ics020_var.selectProcess.bankTransfer) {
            gridName = ics020_var.controls.bankTransferGrid;
            gridObj = ics020_var.objBankTranferGird;
        }

        //$(gridName).LoadDataToGrid(gridObj, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Income/ICS020_CmdReset", obj, "tbt_tmpImportContent", false,
        $(gridName).LoadDataToGrid(gridObj, 0, false, "/Income/ICS020_CmdReset", obj, "tbt_tmpImportContent", false, //Modify by Jutarat A. on 31052013
                function () { // post-load
                    ics020_process.SetScreenMode(ics020_var.screenMode.importMode);
                }, null);
    },

    InitialScreen: function () {
        ics020_process.InitialGrid();
        ics020_process.BidingEvent();
        ics020_process.SetScreenMode(ics020_var.screenMode.importMode);
    },

    InitialGrid: function () {
        if ($.find(ics020_var.controls.autoTransferGrid).length > 0) {
            //ics020_var.objAutoTranferGrid = $(ics020_var.controls.autoTransferGrid).InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Income/ICS020_InitialAutoTransferGrid");
            ics020_var.objAutoTranferGrid = $(ics020_var.controls.autoTransferGrid).InitialGrid(0, false, "/Income/ICS020_InitialAutoTransferGrid"); //Modify by Jutarat A. on 31052013
        }
        if ($.find(ics020_var.controls.bankTransferGrid).length > 0) {
            //ics020_var.objBankTranferGird = $(ics020_var.controls.bankTransferGrid).InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Income/ICS020_InitialBankTransferGrid");
            ics020_var.objBankTranferGird = $(ics020_var.controls.bankTransferGrid).InitialGrid(0, false, "/Income/ICS020_InitialBankTransferGrid"); //Modify by Jutarat A. on 31052013
        }

        BindOnLoadedEvent(ics020_var.objAutoTranferGrid, function () {
            if (ics020_var.objAutoTranferGrid.getRowsNum() != 0) {
                var colImportErrorReason = ics020_var.objAutoTranferGrid.getColIndexById('ImportErrorReason');
                for (var i = 0; i < ics020_var.objAutoTranferGrid.getRowsNum(); i++) {
                    var row_id = ics020_var.objAutoTranferGrid.getRowId(i);
                    if (ics020_var.objAutoTranferGrid.cells(row_id, colImportErrorReason).getValue() != "") {
                        ics020_var.objAutoTranferGrid.setRowColor(row_id, "#ff9999");  //Have error, set background color as red
                    }
                }
            }
        });

        BindOnLoadedEvent(ics020_var.objBankTranferGird, function () {
            if (ics020_var.objBankTranferGird.getRowsNum() != 0) {
                var colImportErrorReason = ics020_var.objBankTranferGird.getColIndexById('ImportErrorReason');
                for (var i = 0; i < ics020_var.objBankTranferGird.getRowsNum(); i++) {
                    var row_id = ics020_var.objBankTranferGird.getRowId(i);
                    if (ics020_var.objBankTranferGird.cells(row_id, colImportErrorReason).getValue() != "") {
                        ics020_var.objBankTranferGird.setRowColor(row_id, "#ff9999");  //Have error, set background color as red
                    }
                }
            }
        });
    },

    BidingEvent: function () {
        $(ics020_var.controls.rdoAutoTransfer).change(ics020_process.SelectProcess_OnChanged);
        $(ics020_var.controls.rdobankTransfer).change(ics020_process.SelectProcess_OnChanged);

        $(ics020_var.controls.btnShowContent).click(ics020_process.ShowContent_OnClick);

        //Next = Reset
        $(ics020_var.controls.btnNextAutoImport).click(ics020_process.CmdReset_OnClick);
        $(ics020_var.controls.btnNextBankImport).click(ics020_process.CmdReset_OnClick);
    },




    SetScreenMode: function (mode) {
        if (mode == ics020_var.screenMode.importMode) {
            $(ics020_var.controls.divAutoTransfer).hide();
            $(ics020_var.controls.divBankTransfer).hide();
            $(ics020_var.controls.btnNextAutoImport).hide();
            $(ics020_var.controls.btnNextBankImport).hide();
            $(ics020_var.controls.iFrameAttach).contents().find(ics020_var.iframeAttachControls.fileSelect).show();
            $(ics020_var.controls.iFrameAttach).contents().find(ics020_var.iframeAttachControls.documentName).hide();

            import_command.SetCommand(this.CmdImport_OnClick);
            reset_command.SetCommand(this.CmdReset_OnClick);

            ics020_process.SelectProcess_OnChanged();
        }
        else if (mode == ics020_var.screenMode.finishMode) {
            $(ics020_var.controls.btnNextAutoImport).show();
            $(ics020_var.controls.btnNextBankImport).show();

            import_command.SetCommand(null);
            reset_command.SetCommand(null);
        }
    }
};

$(document).ready(function () {
    ics020_process.InitialScreen();
});