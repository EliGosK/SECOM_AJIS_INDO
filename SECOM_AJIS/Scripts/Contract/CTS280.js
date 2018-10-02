
/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>

/* --- Variables --- */
//var mygrid;
//var pageRow = 5;

/*--- Main ---*/
$(document).ready(function () {
    InitialTrimTextEvent([
            "PICName",
            "MaintEmpNo",
            "Location",
            "MalfunctionDetail",
            "Remark",
            "ApproveNo1"
        ]);

    /*==== Attach Datetime Picker ====*/
    $("#ExpectedMaintenanceDate").InitialDate();
    $("#MaintenanceDate").InitialDate();

    //Call this method to let datepicker known its value (Case focus on textbox and convert wrong value)
    //$("#ExpectedMaintenanceDate").datepicker("getDate");
    //$("#MaintenanceDate").datepicker("getDate");

    $("#MaintenanceFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#MaintainenceFeeInit").BindNumericBox(12, 2, 0, 999999999999.99);

    /*==== event btnSelectProcess click ====*/
    $("#btnSelectProcess").click(function () {
        renderEditMode();
    });

    //    $("#MaintEmpNo").keypress(function (e) {
    //        if (e.which == 0 || e.which == 8 || (e.which >= 48 && e.which <= 57)) {
    //            return true;
    //        } else {
    //            return false;
    //        }
    //    });

    $("#UsageTime").BindNumericBox(4, 2, 0, 9999.99);

    /*==== event radio process type change ====*/
    //    $("#rdoInputExpectedMaintenanceDate").change(function () {
    //        $("#rdoRegisterActualCheckupData").attr("checked", false);
    //        $("#ProcessType").val($("#rdoInputExpectedMaintenanceDate").val());
    //    });
    //    $("#rdoRegisterActualCheckupData").change(function () {
    //        $("#rdoInputExpectedMaintenanceDate").attr("checked", false);
    //        $("#ProcessType").val($("#rdoRegisterActualCheckupData").val());
    //    });

    /*==== event Malfunction Checkbox relate enable Location textbox ====*/
    $("#InstrumentMalfunctionFlag").change(function () {
        var isChecked = $("#InstrumentMalfunctionFlag").prop("checked");
        if (isChecked == true) {
            //            if (CTS280Data.Mode == CTS280Data.FunctionIdView) {
            //                $("#Location").attr("readonly", true);
            //            }
            //            else {
            $("#Location").removeAttr("readonly");
            //            }

            $("#Location").val($("#DefaultLocation").val());
        }
        else {
            $("#Location").attr("readonly", true);
            $("#Location").val("");
        }
    });

    /*==== event on lost focus at EmpNo load EmpName ====*/
    //    $("#MaintEmpNo").blur(function () {
    //        if ($("#MaintEmpNo").val() != "") {
    //            loadEmpName($("#MaintEmpNo").val());
    //        }
    //        else {
    //            $("#MaintEmpName").val("");
    //        }
    //    });
    GetEmployeeNameData("#MaintEmpNo", "#MaintEmpName");

    initial();

    $("#MalfunctionDetail").SetMaxLengthTextArea(500);
    $("#Remark").SetMaxLengthTextArea(500);
});

function initial() {
    /*==== initial section ====*/
    $("#Section_Information").show();
    //$("#Section_ProcessType").hide();
    //$("#Section_Result").hide();

    $("#Location").attr("readonly", false);
    //$("#ParamCurrentMode").val($("#ParamMode").val());
    //loadInformationSection();

    if (CTS280Data.Mode == CTS280Data.FunctionIdView)
        renderViewMode();
    else
        renderSelectProcessMode();
}

function clearSectionProcessType() {
    $("#InputExpectedMaintenanceDate").attr("checked", true);
    $("#RegisterActualCheckupData").attr("checked", false);
}

function disableSectionResult(value) {
    $("#ExpectedMaintenanceDate").EnableDatePicker(!value); //$("#ExpectedMaintenanceDate").attr("readonly", value);
    $("#MaintenanceDate").EnableDatePicker(!value); //$("#MaintenanceDate").attr("readonly", value);
    $("#MaintenanceFee").attr("readonly", value);
    $("#MaintenanceFee").NumericCurrency().attr("disabled", value);
    $("#ApproveNo1").attr("readonly", value);
    $("#SubcontractCode").attr("disabled", value);
    $("#PICName").attr("readonly", value);
    $("#MaintEmpNo").attr("readonly", value);
    $("#UsageTime").attr("readonly", value);
    $("#InstrumentMalfunctionFlag").attr("disabled", value);
    $("#Location").attr("readonly", value);
    $("#NeedSalesmanFlag").attr("disabled", value);
    $("#MalfunctionDetail").attr("readonly", value);
    $("#Remark").attr("readonly", value);
}

function renderViewMode() {
    $("#Section_ProcessType").fadeOut("fast", function () {
        $("#Section_Result").fadeIn("fast");
    });

    disableSectionResult(true);
    showOtherResultControl();

    ajax_method.CallScreenController(
        '/Contract/CTS280_RenderViewMode',
        "",
        function (result, controls) {
            if (result != undefined) {

                if (result.MaintenanceFeeFlag == true) {
                    $("#Section_MaintenanceFee").show();
                } else {
                    $("#Section_MaintenanceFee").hide();
                }

                edit_command.SetCommand(renderEditMode);
                close_command.SetCommand(doClose);
                register_command.SetCommand(null);
                reset_command.SetCommand(null);
                cancel_command.SetCommand(null);
                confirm_command.SetCommand(null);
                back_command.SetCommand(null);

                edit_command.EnableCommand(result.EnableEditButton);
            }
        }
    );

    //    SetEditCommand(true, renderEditMode);
    //    SetCloseCommand(true, doClose);
    //    SetRegisterCommand(false, null);
    //    SetResetCommand(false, null);
    //    SetCancelCommand(false, null);
    //    SetConfirmCommand(false, null);
    //    SetBackCommand(false, null);
}


function showOtherResultControl() {
    $("#Section_RequireExpectedMaintenanceDate").hide();
    $("#Section_RegisterDetail").show();
}

function hideOtherResultControl() {
    $("#Section_RequireExpectedMaintenanceDate").show();
    $("#Section_RegisterDetail").hide();
}

function renderSelectProcessMode() {
    $("#Section_Result").fadeOut("fast", function () {
        $("#Section_ProcessType").fadeIn("fast");
    });

    disableSectionResult(false);

    edit_command.SetCommand(null);
    close_command.SetCommand(null);
    register_command.SetCommand(null);
    reset_command.SetCommand(null);
    cancel_command.SetCommand(doCancel);
    confirm_command.SetCommand(null);
    back_command.SetCommand(null);
}

function renderEditMode() {
    $("#Section_ProcessType").fadeOut("fast", function () {
        $("#Section_Result").fadeIn("fast");
    });
    
    
    //$("#ParamCurrentMode").val("edit");

    disableSectionResult(false);
    $("#Location").attr("readonly", !$("#InstrumentMalfunctionFlag").prop("checked"));

    //    if ($("#ProcessType").val() == "" || $("#ProcessType").val() == $("#rdoInputExpectedMaintenanceDate").val()) {
    //        $("#ProcessType").val($("#rdoInputExpectedMaintenanceDate").val()); //for case "#ProcessType").val() == "" don't toggle
    //        hideOtherResultControl();
    //    } else {
    //        showOtherResultControl();
    //    }

//    if (CTS280Data.Mode == CTS280Data.FunctionIdView) {
//        showOtherResultControl();
//    } else {
//        var bExpectedChkFlag = $("#InputExpectedMaintenanceDate").prop("checked");
//        if (bExpectedChkFlag) {
//            hideOtherResultControl();
//        } else {
//            showOtherResultControl();
//        }
//    }

    var obj = {
        mode : 0
    }

    var bExpectedChkFlag = $("#InputExpectedMaintenanceDate").prop("checked");
    if (CTS280Data.Mode == CTS280Data.FunctionIdView) {
        obj.mode = 0;
    }
    else if (bExpectedChkFlag) {
        obj.mode = 1;
    }
    else {
        obj.mode = 2;
    }

    ajax_method.CallScreenController(
        '/Contract/CTS280_RenderEditMode',
        obj,
        function (result, controls) {
            if (result != undefined) {
                
                if (result.ShowOnlyExpectedMaintenanceDate == true) {
                    hideOtherResultControl();
                }
                else {
                    showOtherResultControl();
                }

                if (result.ShowMaintenanceFee == true) {
                    $("#Section_MaintenanceFee").show();
                } else {
                    $("#Section_MaintenanceFee").hide();
                }

                if (result.IsSetMaintenanceFee == true) {
                    $("#MaintenanceFee").SetNumericCurrency(result.MetenanceFeeCurrencyType);
                    $("#MaintenanceFee").val(SetNumericValue(result.MetenanceFee, 2));
                    $("#MaintenanceFee").setComma();
                }

                //Set Value to MaintenanceFeeInit
                //$("#MaintenanceFeeInit").val(result.MaintenanceFeeInit);

                edit_command.SetCommand(null);
                close_command.SetCommand(null);
                register_command.SetCommand(doRegister);
                reset_command.SetCommand(doReset);
                cancel_command.SetCommand(doCancel);
                confirm_command.SetCommand(null);
                back_command.SetCommand(null);
            }
        }
    );

    //    SetEditCommand(false, null);
    //    SetCloseCommand(false, null);
    //    SetRegisterCommand(true, doRegister);
    //    SetResetCommand(true, doReset);
    //    SetCancelCommand(true, doCancel);
    //    SetConfirmCommand(false, null);
    //    SetBackCommand(false, null);
}

function renderConfirmMode() {
    edit_command.SetCommand(null);
    close_command.SetCommand(null);
    register_command.SetCommand(null);
    reset_command.SetCommand(null);
    cancel_command.SetCommand(null);
    confirm_command.SetCommand(doConfirm);
    back_command.SetCommand(doBack);
}

function doBack() {
    SetTextOnlyMode(false);
    renderEditMode();
}

function doClose() {
    //    var url = generate_url($("#ParamCaller").val());
    //    $(location).attr('href', url);
    //CallScreenWithAuthority("/Contract/CTS270", "");

    ajax_method.CallScreenController("/Contract/CTS280_CallScreenURL", "", function (result) {
        if (result != undefined) {
            window.location = ajax_method.GenerateURL(result);
        }
    });
}

function doCancel() {
    //    var param = { "module": "Common", "code": "MSG0096" };
    //    ajax_method.CallScreenController("/Shared/GetMessage", param, function (data) {

    //        /* ====== Open error dialog =====*/
    //        OpenOkCancelDialog(data.Code, data.Message, function () {
    //            clearSectionProcessType();
    //            renderViewMode();
    //        }, null);

    //        return false;
    //    });

    var param = { "module": "Common", "code": "MSG0096" };
    ajax_method.CallScreenController("/Shared/GetMessage", param, function (data) {

        /* ====== Open error dialog =====*/
        OpenOkCancelDialog(data.Code, data.Message, function () {

            if (CTS280Data.Mode == CTS280Data.FunctionIdView) {
                //clearSectionProcessType();
                //renderViewMode();
                fillResultSection();
                renderViewMode();
            } else {
                doClose();
            }

        }, null);

        return false;
    });
}

function doReset() {
//    var param = { "module": "Common", "code": "MSG0038" };
//    ajax_method.CallScreenController("/Shared/GetMessage", param, function (data) {

//        /* ====== Open confirm dialog =====*/
//        OpenOkCancelDialog(data.Code, data.Message, function () {
            //initial();
            //renderEditMode();
            //$("#Location").attr("readonly", false);
            fillResultSection();
            renderEditMode();
//        }, null);

//        return false;
//    });
}

function doRegister() {
    
    //Set value for business validate section

    //ProcessType
    var processType = $("#RegisterActualCheckupData").val();
    if ($("#InputExpectedMaintenanceDate").prop("checked") == true) {
        processType = $("#InputExpectedMaintenanceDate").val();
    }

    //Checkbox InstrumentMalfunctionFlag and NeedSalesmanFlag 
    $("#InstrumentMalfunctionFlagData").val($('#InstrumentMalfunctionFlag').attr('checked'));
    $("#NeedSalesmanFlagData").val($('#NeedSalesmanFlag').attr('checked'));

    var parameter =
    {
        ExpectedMaintenanceDate : $("#ExpectedMaintenanceDate").val(),
        MaintenanceDate: $("#MaintenanceDate").val(),

        MaintenanceFeeCurrencyType: $("#MaintenanceFee").NumericCurrencyValue(),
        MaintenanceFee: $("#MaintenanceFee").NumericValue(),

        ApproveNo1 : $("#ApproveNo1").val(),
        SubcontractCode : $("#SubcontractCode").val(),
        PICName : $("#PICName").val(),
        MaintEmpNo : $("#MaintEmpNo").val(),
        UsageTime: $("#UsageTime").NumericValue(),
        Location: $("#Location").val(),
        MalfunctionDetail: $("#MalfunctionDetail").val(),
        Remark: $("#Remark").val(),
        ProcessType: processType
    }
    if ($("#InstrumentMalfunctionFlag").attr("checked") == "checked")
        parameter.InstrumentMalfunctionFlagData = true;
    if ($("#NeedSalesmanFlag").attr("checked") == "checked")
        parameter.NeedSalesmanFlagData = true;

    //var parameter = $("#CTS280_Result").serialize();
    ajax_method.CallScreenController(
        '/Contract/CTS280_RegisterAction/',
        parameter,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["ExpectedMaintenanceDate"
                            , "MaintenanceDate"
                            , "MaintenanceFee"
                            , "Location"
                            , "ApproveNo1"
                            , "UsageTime"
                            , "MaintEmpNo"
                            , "SubcontractCode"
                            , "PICName"], controls);
                return;
            } else if (result == true) {
                renderConfirmMode();
                SetTextOnlyMode(true);
            } 
    });
}

function doConfirm() {

    //var inputData = CreateObjectData($("#CTS280_InputParam").serialize());
    //    var resultData = CreateObjectData($("#CTS280_Result").serialize());
    //var parameter = {"inputData": inputData, "resultData": resultData};
    //    var parameter = { "resultData": resultData };

    //    ajax_method.CallScreenController_json(
    //        '/Contract/CTS280_ConfirmAction/',
    //        parameter,
    //        function (result, controls) {
    //            if (result != undefined) {
    //                OpenInformationMessageDialog(result.Code, result.Message, function () {
    //                    SetTextOnlyMode(false);
    //                    clearSectionProcessType();
    //                    renderViewMode();
    //                });
    //            }
    //        }
    //    );

    var processType = $("#RegisterActualCheckupData").val();
    if ($("#InputExpectedMaintenanceDate").prop("checked") == true) {
        processType = $("#InputExpectedMaintenanceDate").val();
    }

    var parameter =
    {
        ExpectedMaintenanceDate: $("#ExpectedMaintenanceDate").val(),
        MaintenanceDate: $("#MaintenanceDate").val(),
        MaintenanceFee: $("#MaintenanceFee").NumericValue(),
        ApproveNo1: $("#ApproveNo1").val(),
        SubcontractCode: $("#SubcontractCode").val(),
        PICName: $("#PICName").val(),
        MaintEmpNo: $("#MaintEmpNo").val(),
        UsageTime: $("#UsageTime").NumericValue(),
        Location: $("#Location").val(),
        MalfunctionDetail: $("#MalfunctionDetail").val(),
        Remark: $("#Remark").val(),
        ProcessType: processType,
        MaintenanceFeeCurrencyType: $("#MaintenanceFee").NumericCurrencyValue() // Add Narut T. 2017-02-10
    }
    if ($("#InstrumentMalfunctionFlag").attr("checked") == "checked")
        parameter.InstrumentMalfunctionFlagData = true;
    if ($("#NeedSalesmanFlag").attr("checked") == "checked")
        parameter.NeedSalesmanFlagData = true;

    //var parameter = $("#CTS280_Result").serialize();
    ajax_method.CallScreenController(
        '/Contract/CTS280_ConfirmAction/',
        parameter,
        function (result, controls) {
            if (result != undefined) {
                OpenInformationMessageDialog(result.Code, result.Message, function () {

                    if (CTS280Data.Mode == CTS280Data.FunctionIdView) {
                        SetTextOnlyMode(false);
                        clearSectionProcessType();
                        renderViewMode();

                        fillResultSection();
                    } else {
                        doClose();
                    }

                });
            }
        }
    );
}

function SetTextOnlyMode(bValue) {
    $("#Section_Information").SetViewMode(bValue);
    $("#Section_Result").SetViewMode(bValue);

    if (bValue) {
        $("#divWarning").hide();
    }
    else {
        $("#divWarning").show();
    }
}

function fillResultSection() {
   //var parameter = $("#CTS280_InputParam").serialize()
    ajax_method.CallScreenController(
        '/Contract/CTS280_FillResultSection/',
        "",
        function (result, controls) {
            if (result != undefined) {
                $("#CTS280_Information").clearForm();
                $("#CTS280_Result").clearForm();

                $("#CTS280_Information").bindJSON(result.doMaintCheckupInformation);
                $("#CTS280_Result").bindJSON(result.dtMaintenanceCheckup);

                $("#ContractCode").val(result.doMaintCheckupInformation.ContractCodeShort);
                $("#RealCustomerCustCode").val(result.doMaintCheckupInformation.RealCustomerCustCodeShow);
                $("#SiteCode").val(result.doMaintCheckupInformation.SiteCodeShow);

                $("#MaintenanceFee").val(SetNumericValue(result.dtMaintenanceCheckup.MaintenanceFee, 2));
                $("#MaintenanceFee").setComma();

                if (result.dtMaintenanceCheckup.InstrumentMalfunctionFlag == true)
                    $("#InstrumentMalfunctionFlag").attr("checked", true);

                if (result.dtMaintenanceCheckup.NeedSalesmanFlag == true)
                    $("#NeedSalesmanFlag").attr("checked", true);

                //Set Date
                var exptedDateTxt = ConvertDateObject(result.dtMaintenanceCheckup.ExpectedMaintenanceDate, true);
                if (exptedDateTxt != undefined) {
                    result.dtMaintenanceCheckup.ExpectedMaintenanceDate = new Date(exptedDateTxt);
                }

                var maDateTxt = ConvertDateObject(result.dtMaintenanceCheckup.MaintenanceDate, true);
                if (maDateTxt != undefined) {
                    result.dtMaintenanceCheckup.MaintenanceDate = new Date(maDateTxt);
                }

                $("#ExpectedMaintenanceDate").SetDate(result.dtMaintenanceCheckup.ExpectedMaintenanceDate);
                $("#MaintenanceDate").SetDate(result.dtMaintenanceCheckup.MaintenanceDate);

                $("#DefaultLocation").val($("#Location").val());

                if ($("#InstrumentMalfunctionFlag").prop("disabled") == true) {
                    $("#Location").attr("readonly", true);
                }
                else {
                    $("#Location").attr("readonly", !$("#InstrumentMalfunctionFlag").prop("checked"));
                }
            }
        }
    );
}

//function loadInformationSection() {
//    var parameter = $("#CTS280_InputParam").serialize()
//    ajax_method.CallScreenController(
//        '/Contract/CTS280_LoadInformationSection/',
//        parameter,
//        function (result, controls) {
//            if (result != undefined) {
//                $("#CTS280_Information").clearForm();
//                $("#CTS280_Information").bindJSON(result);

//                loadResultSection();
//            }
//        }
//    );
//}

//function loadResultSection() {
//   var parameter = $("#CTS280_InputParam").serialize()
//   ajax_method.CallScreenController(
//        '/Contract/CTS280_LoadResultSection/',
//        parameter,
//        function (result, controls) {
//            if (result != undefined) {

//                $("#CTS280_Result").clearForm();
//                $("#CTS280_Result").bindJSON(result);8

//                if ($("#ParamMode").val() == "view") {
//                    renderViewMode();
//                } else if ($("#ParamMode").val() == "edit") {
//                    renderSelectProcessMode();
//                }

//                if (result.MaintenanceFeeFlag == true) {
//                    $("#Section_MaintenanceFee").show();
//                } else {
//                    $("#Section_MaintenanceFee").hide();
//                }

//                //Set Date
//                result.ExpectedMaintenanceDate = ConvertDateObject(result.ExpectedMaintenanceDate);
//                result.MaintenanceDate = ConvertDateObject(result.MaintenanceDate);
//                $("#ExpectedMaintenanceDate").val(ConvertDateToTextFormat(result.ExpectedMaintenanceDate));
//                $("#MaintenanceDate").val(ConvertDateToTextFormat(result.MaintenanceDate));
//            }
//        }
//    );
//}

function loadEmpName(MaintEmpNo) {
    var parameter = { "MaintEmpNo": MaintEmpNo };
    ajax_method.CallScreenController(
        '/Contract/CTS280_LoadEmployeeName/',
        parameter,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["MaintEmpNo"], controls);
                $("#MaintEmpName").val("");
                $("#MaintEmpNo").focus();
                return;
            } else if (result != undefined) {
                $("#MaintEmpName").val(result);
            }
        }
    );
}
