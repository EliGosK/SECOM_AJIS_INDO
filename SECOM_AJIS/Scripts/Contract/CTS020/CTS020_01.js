/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
$(document).ready(function () {
    InitialTrimTextEvent([
        "ContractCode"
    ]);

    InitSpecifyProcessType();
    InitSpecifyProcessTypeEvents();

    //$("#rdoProcessTypeAdd").attr("disabled", true);
});

function InitSpecifyProcessType() {
    if ($("#ScreenMode").val() == 0) {  //New
        $("#divSpecifyProcessType").show();
    }
    else {                              //Edit,Approve
        $("#divSpecifyProcessType").hide();
    }

    process_type_change();
}
function InitSpecifyProcessTypeEvents() {
    $("#rdoProcessTypeNew").change(process_type_change);
    $("#rdoProcessTypeAdd").change(process_type_change);
    $("#btnSelectProcess").click(select_process_click);
}
/* ----------------------------------------------------------------------------------- */

function process_type_change() {
    if ($("#rdoProcessTypeAdd").prop("checked") == true) {
        $("#ContractCode").removeAttr("readonly");
    }
    else {
        $("#ContractCode").val("");
        $("#ContractCode").attr("readonly", true);
    }
}
function select_process_click() {
    processType = 1;
    if ($("#rdoProcessTypeAdd").prop("checked") == true)
        processType = 2;

    var obj = {
        ProcessType: processType,
        ContractCode: $("#ContractCode").val()
    };
    call_ajax_method_json("/Contract/CTS020_SelectProcess", obj, function (result, controls) {
        if (controls != undefined) {
            VaridateCtrl(["ContractCode"], controls);
        }
        else if (typeof (result) == "string") {
            ShowSpecifyQuotationSection(true);
            SetCTS020_01_EnableSection(false);
            InitialCommandButton(4);

            if ($("#rdoProcessTypeAdd").prop("checked") == true) {
                $("#ContractCode").val(result);
                $("#QuotationTargetCode").val(result);
                $("#QuotationTargetCode").attr("readonly", true);
            }
        }
    });
}


function SetCTS020_01_SectionMode(view_mode) {
    $("#divSpecifyProcessType").SetViewMode(view_mode);
    if (view_mode == true) {
        $("#divProcessEditMode").hide();
        $("#divProcessViewMode").show();

        var css = "label-readonly-view";
        if ($("#ContractCode").prop("readonly") == false)
            css = "label-edit-view";

        var process = $("#spanrdoProcessTypeNew").html();
        if ($("#rdoProcessTypeAdd").prop("checked") == true)
            process = $("#spanrdoProcessTypeAdd").html();

        $("#ProcessTypeView").html(process);
        $("#ContractCodeView").html($("#ContractCode").val());
    }
    else {
        $("#divProcessEditMode").show();
        $("#divProcessViewMode").hide();
    }
}
function SetCTS020_01_EnableSection(enable) {
    $("#divSpecifyProcessType").SetEnableView(enable);
    //$("#rdoProcessTypeAdd").attr("disabled", true);

    if (enable) {
        process_type_change();
    }
}