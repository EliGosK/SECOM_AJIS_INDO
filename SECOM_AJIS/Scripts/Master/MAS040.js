/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>
/// <reference path="../../Scripts/Base/control_events.js"/>

/* --- Initial -------------------------------------------------- */
/* -------------------------------------------------------------- */

function MAS040Initial() {
    ChangeDialogButtonText(
            ["OK", "Cancel"],
            [$("#btnOK").val(),
             $("#btnCancel").val()]);

    BindDialogButtonClick($("#btnOK").val(), function () {
        ConfirmData();
    });
    $("#Register #SiteNameEN").focus();
}

$(document).ready(function () {
    InitialTrimTextEvent([
        "Register #SiteNameEN",
        "Register #SiteNameLC",
        "Register #SECOMContactPerson",
        "Register #PersonInCharge",
        "Register #PhoneNo",
        "Address_form #AddressEN",
        "Address_form #AddressLC",
        "Address_form #AlleyEN",
        "Address_form #AlleyLC",
        "Address_form #RoadEN",
        "Address_form #RoadLC",
        "Address_form #SubDistrictEN",
        "Address_form #SubDistrictLC",
        "ZipCode"
    ]);

    InitialEvent();
    InitialControl();

    InitialNumericInputTextBox(["divAddressInfo input[id=ZipCode]"]);

    $("#UsageCode").change(getAttachImportanceFlag);
    getAttachImportanceFlag();
});

function InitialEvent() {
    $("#Register #SiteNameEN").InitialAutoComplete("/Master/GetSiteNameEN");
    $("#Register #SiteNameLC").InitialAutoComplete("/Master/GetSiteNameLC");
    $("#Address_form #AddressEN").InitialAutoComplete("/Master/GetSiteAddressEN");
    $("#Address_form #AddressLC").InitialAutoComplete("/Master/GetSiteAddressLC");
    $("#Address_form #AlleyEN").InitialAutoComplete("/Master/GetSiteAlleyEN");
    $("#Address_form #AlleyLC").InitialAutoComplete("/Master/GetSiteAlleyLC");
    $("#Address_form #RoadEN").InitialAutoComplete("/Master/GetSiteRoadEN");
    $("#Address_form #RoadLC").InitialAutoComplete("/Master/GetSiteRoadLC");
    $("#Address_form #SubDistrictEN").InitialAutoComplete("/Master/GetSiteSubDistrictEN");
    $("#Address_form #SubDistrictLC").InitialAutoComplete("/Master/GetSiteSubDistrictLC");

    $("#Address_form #ProvinceEN").RelateControlEvent(cboJangwatEN_change);
    $("#Address_form #ProvinceLC").RelateControlEvent(cboJangwatLC_change);
    $("#Address_form #DistrictEN").RelateControlEvent(ChangeLC);
    $("#Address_form #DistrictLC").RelateControlEvent(ChangeEN);
}
function InitialControl() {
    var Prov = {
        provinceCode: $("#ProvinceEN").val()
    };

    call_ajax_method_json("/Master/GetAmphorEN", Prov, function (result) {
        regenerate_combo("#DistrictEN", result);
        $("#DistrictEN").val($("#DefDistrictEN").val());

        call_ajax_method_json("/Master/GetAmphorLC", Prov, function (result) {
            regenerate_combo("#DistrictLC", result);
            $("#DistrictLC").val($("#DefDistrictLC").val());

            ValidateObjectData();
        });
    });
}

/* -------------------------------------------------------------- */

/* --- Events --------------------------------------------------- */
/* -------------------------------------------------------------- */

function cboJangwatEN_change(istab, isblur) {
    $("#Address_form #ProvinceLC").val($("#Address_form #ProvinceEN").val());
    $("#Address_form #ProvinceLC").ResetToNormalControl();

    if (isblur) {
        var Prov = {
            provinceCode: $("#Address_form #ProvinceEN").val()
        };

        call_ajax_method_json("/Master/GetAmphorEN", Prov, update_amperkedEN_combo);
        call_ajax_method_json("/Master/GetAmphorLC", Prov, update_amperkedLC_combo);
    }
}
function cboJangwatLC_change(istab, isblur) {
    $("#Address_form #ProvinceEN").val($("#Address_form #ProvinceLC").val());
    $("#Address_form #ProvinceEN").ResetToNormalControl();

    if (isblur) {
        var Prov = {
            provinceCode: $("#Address_form #ProvinceLC").val()
        };

        call_ajax_method_json("/Master/GetAmphorEN", Prov, update_amperkedEN_combo);
        call_ajax_method_json("/Master/GetAmphorLC", Prov, update_amperkedLC_combo);
    }
}
function ChangeLC() {
    $("#Address_form #DistrictLC").val($("#Address_form #DistrictEN").val());
    $("#Address_form #DistrictLC").ResetToNormalControl();
}
function ChangeEN() {
    $("#Address_form #DistrictEN").val($("#Address_form #DistrictLC").val());
    $("#Address_form #DistrictEN").ResetToNormalControl();
}

function ConfirmData() {
    var obj = CreateObjectData($("#Register").serialize() + "&" + $("#Address_form").serialize());
    obj.SiteNameEN = $("#Register #SiteNameEN").val();
    obj.SiteNameLC = $("#Register #SiteNameLC").val();
    obj.PhoneNo = $("#Register #PhoneNo").val();

    obj.BuildingUsageCode = "";
    obj.BuildingUsageName = "";
    if ($("#UsageCode").val() != "") {
        obj.BuildingUsageCode = $("#UsageCode").val();
        obj.BuildingUsageName = $("#UsageCode option:selected").text();
    }

    obj.ProvinceCode = "";
    obj.ProvinceNameEN = "";
    obj.ProvinceNameLC = "";
    if ($("#ProvinceEN").val() != "") {
        obj.ProvinceCode = $("#ProvinceEN").val();
        obj.ProvinceNameEN = $("#ProvinceEN option:selected").text();

        if ($("#ProvinceLC").val() != "")
            obj.ProvinceNameLC = $("#ProvinceLC option:selected").text();
    }

    obj.DistrictCode = "";
    obj.DistrictNameEN = "";
    obj.DistrictNameLC = "";
    if ($("#DistrictEN").val() != "") {
        obj.DistrictCode = $("#DistrictEN").val();
        obj.DistrictNameEN = $("#DistrictEN option:selected").text();

        if ($("#DistrictLC").val() != "")
            obj.DistrictNameLC = $("#DistrictLC option:selected").text();
    }

    call_ajax_method_json("/Master/MAS040_ConfirmData", obj,
    function (result, controls) {
        VaridateCtrl([
            "Register #SiteNameEN",
            "Register #SiteNameLC",
            "UsageCode",
            "AddressEN",
            "SubDistrictEN",
            "ProvinceEN",
            "DistrictEN",
            "AddressLC",
            "SubDistrictLC",
            "ProvinceLC",
            "DistrictLC"], controls);

        if (controls != undefined) {
            return;
        }
        if (result != undefined) {
            if (MAS040Response != undefined)
                MAS040Response(result);
        }
    }
    );
}

/* -------------------------------------------------------------- */

/* --- Methods -------------------------------------------------- */
/* -------------------------------------------------------------- */

function ValidateObjectData() {
    call_ajax_method_json("/Master/MAS040_ValidateData", "",
    function (result, controls) {
        VaridateCtrl([
            "Register #SiteNameEN",
            "Register #SiteNameLC",
            "UsageCode",
            "AddressEN",
            "SubDistrictEN",
            "ProvinceEN",
            "DistrictEN",
            "AddressLC",
            "SubDistrictLC",
            "ProvinceLC",
            "DistrictLC"], controls);
    });
}

function getAttachImportanceFlag() {
    var obj = { UsageCode: $("#UsageCode").val() };
    call_ajax_method_json("/Master/MAS040_GetAttachImportanceFlag", obj,
    function (result, controls) {
        if (result == true) {
            $("#ChkAttachImportanceFlag").attr("checked", true);
        }
        else {
            $("#ChkAttachImportanceFlag").attr("checked", false);
        }
    });
}

function update_amperkedEN_combo(data) {
    var sval = $("#Address_form #DistrictEN").val();
    regenerate_combo("#Address_form #DistrictEN", data);
    $("#Address_form #DistrictEN").val(sval);
}
function update_amperkedLC_combo(data) {
    var sval = $("#Address_form #DistrictLC").val();
    regenerate_combo("#Address_form #DistrictLC", data);
    $("#Address_form #DistrictLC").val(sval);
}

/* -------------------------------------------------------------- */