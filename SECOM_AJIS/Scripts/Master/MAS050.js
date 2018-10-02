/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>
/// <reference path="../../Scripts/Base/control_events.js"/>



/* --- Valiables ------------------------------------------------ */
/* -------------------------------------------------------------- */
var gridCustomerGroup;
var currentCompany;
/* -------------------------------------------------------------- */

/* --- Initial -------------------------------------------------- */
/* -------------------------------------------------------------- */

function MAS050Initial() {
    ChangeDialogButtonText(
            ["OK", "Cancel"],
            [$("#btnOK").val(),
             $("#btnCancel").val()]);

    BindDialogButtonClick($("#btnOK").val(), function () {
        ConfirmData();
    });

    if ($("#Register #CustomerGroupCode").length > 0) {
        $("#Register #CustomerGroupCode").focus();
    }
    else {
        $("#CustomerType").focus();
    }
}

$(document).ready(function () {
    InitialTrimTextEvent([
        "Register #IDNo",
        "Register #CustNameEN",
        "Register #CustNameLC",
        "Register #RepPersonName",
        "Register #ContactPersonName",
        "Register #SECOMContactPerson",
        "Register #TelePhoneNo",
        "Register #FaxNo",
        "Register #URL",
        "Register #AddressEN",
        "Register #AddressLC",
        "Register #AlleyEN",
        "Register #AlleyLC",
        "Register #RoadEN",
        "Register #RoadLC",
        "Register #SubDistrictEN",
        "Register #SubDistrictLC",
        "Register #Zipcode"
    ]);
    
    if ($("#IsFullValidate").val() == "True")
        InitialGrid();

    InitialEvent();
    InitialControl();

    InitialNumericInputTextBox(["divCustInfo input[id=IDNo]", "divAddressInfo input[id=Zipcode]"]);

});

function InitialControl() {
    CustomerTypeChange(false);

    if ($("#IsFullValidate").val() == "True") {
        if ($("#Register #Dummy").prop("checked") == true) {
            $("#Register #IDNo").attr("readonly", true);
            $("#Register #IDNo").val("");
        }
    }

    var Prov = {
        provinceCode: $("#Register #ProvinceEN").val()
    };

    call_ajax_method_json("/Master/GetAmphorEN", Prov, function (result) {
        regenerate_combo("#Register #DistrictEN", result);
        $("#Register #DistrictEN").val($("#DefDistrictEN").val());

        call_ajax_method_json("/Master/GetAmphorLC", Prov, function (result) {
            regenerate_combo("#Register #DistrictLC", result);
            $("#Register #DistrictLC").val($("#DefDistrictLC").val());

            ValidateObjectData();
        });
    });
}
function InitialGrid() {
    gridCustomerGroup = $("#Result_Div_Add").LoadDataToGridWithInitial(0, false, false,
                                "/Master/MAS050_InitialGrid",
                                "",
                                "dtCustomeGroupData", false);
    gridCustomerGroup.enableAutoHeight(true, 84);
    SpecialGridControl(gridCustomerGroup, ["Remove"]);

    BindOnLoadedEvent(gridCustomerGroup, function (gen_ctrl) {
        for (var i = 0; i < gridCustomerGroup.getRowsNum(); i++) {
            var row_id = gridCustomerGroup.getRowId(i);

            if (gen_ctrl == true) {
                GenerateRemoveButton(gridCustomerGroup, "btnRemove", row_id, "Remove", true);
            }

            /* --- Set Event --- */
            /* ----------------- */
            BindGridButtonClickEvent("btnRemove", row_id, function (rid) {
                DeleteRow(gridCustomerGroup, rid);
            });
            /* ----------------- */
        }
        gridCustomerGroup.setSizes();
    });

    $("#Register #Add").click(function () {
        /* --- Create Parameter --- */
        /* ------------------------ */
        var arr = new Array();
        if (CheckFirstRowIsEmpty(gridCustomerGroup) == false) {
            for (var i = 0; i < gridCustomerGroup.getRowsNum(); i++) {
                var codeCol = gridCustomerGroup.getColIndexById("GroupCode");

                var iobj = {
                    GroupCode: gridCustomerGroup.cells2(i, codeCol).getValue()
                };

                arr.push(iobj);
            }
        }

        var obj = {
            CustCode: $("#Register #CustCode").val(),
            GroupCode: $("#Register #CustomerGroupCode").val(),
            CustomerGroupList: arr
        };
        /* ------------------------ */

        /* --- Check and Add event --- */
        /* --------------------------- */
        call_ajax_method_json("/Master/MAS050_CheckBeforeAddCustomerGroup", obj, function (result, controls) {
            if (controls != undefined) {
                /* --- Higlight Text --- */
                /* --------------------- */
                VaridateCtrl(["CustomerGroupCode"], controls);
                /* --------------------- */

                return;
            }
            else if (result != undefined) {
                /* --- Check Empty Row --- */
                /* ----------------------- */
                CheckFirstRowIsEmpty(gridCustomerGroup, true);
                /* ----------------------- */

                /* --- Add new row --- */
                /* ------------------- */
                AddNewRow(gridCustomerGroup,
                    [result.GroupCode,
                        result.GroupNameEN,
                        result.GroupNameLC,
                        ""]);
                /* ------------------- */

                var row_idx = gridCustomerGroup.getRowsNum() - 1;
                var row_id = gridCustomerGroup.getRowId(row_idx);

                GenerateRemoveButton(gridCustomerGroup, "btnRemove", row_id, "Remove", true);
                gridCustomerGroup.setSizes();

                /* --- Set Event --- */
                /* ----------------- */
                BindGridButtonClickEvent("btnRemove", row_id, function (rid) {
                    DeleteRow(gridCustomerGroup, rid);
                });
                /* ----------------- */
            }
        });
        /* --------------------------- */
    });
}



function InitialEvent() {
    $("#Register #CustomerType").RelateControlEvent(CustomerTypeChange);
    $("#Register #CompanyType").RelateControlEvent(CompanyTypeChange);
    
    if ($("#IsFullValidate").val() == "True")
        $("#Register #Dummy").change(DummyFlagChange);

    $("#Register #CustNameEN").InitialAutoComplete("/Master/GetCustomerNameEN");
    $("#Register #CustNameEN").keyup(CustomerNameEN_change);
    $("#Register #CustNameEN").blur(CustomerNameEN_change);
    $("#Register #CustNameLC").InitialAutoComplete("/Master/GetCustomerNameLC");
    $("#Register #CustNameLC").keyup(CustomerNameLC_change);
    $("#Register #CustNameLC").blur(CustomerNameLC_change);

    $("#Register #AddressEN").InitialAutoComplete("/Master/GetCustomerAddressEN");
    $("#Register #AddressLC").InitialAutoComplete("/Master/GetCustomerAddressLC");
    $("#Register #AlleyEN").InitialAutoComplete("/Master/GetCustomerAlleyEN");
    $("#Register #AlleyLC").InitialAutoComplete("/Master/GetCustomerAlleyLC");
    $("#Register #RoadEN").InitialAutoComplete("/Master/GetCustomerRoadEN");
    $("#Register #RoadLC").InitialAutoComplete("/Master/GetCustomerRoadLC");
    $("#Register #SubDistrictEN").InitialAutoComplete("/Master/GetCustomerSubDistrictEN");
    $("#Register #SubDistrictLC").InitialAutoComplete("/Master/GetCustomerSubDistrictLC");


    $("#Register #ProvinceEN").RelateControlEvent(cboJangwatEN_change);
    $("#Register #ProvinceLC").RelateControlEvent(cboJangwatLC_change);
    $("#Register #DistrictEN").RelateControlEvent(ChangeLC);
    $("#Register #DistrictLC").RelateControlEvent(ChangeEN);
}

/* -------------------------------------------------------------- */

/* --- Events --------------------------------------------------- */
/* -------------------------------------------------------------- */
function CustomerTypeChange(istab) {
    if ($("#Register #CustomerType").val() == "0") {
        $("#Register #CompanyType").removeAttr("disabled");

        if (istab) {
            $("#Register #CompanyType").focus();
        }
    }
    else {
        $("#Register #CompanyType").val("");
        $("#Register #CompanyType").attr("disabled", true);

        if (istab)
            $("#Register #IDNo").focus();
    }
    CompanyTypeChange(false);
}
function CompanyTypeChange(istab) {
    if ($("#Register #CompanyType").val() == "04") {
        $("#Register #FinancialMarketType").removeAttr("disabled");

        if (istab)
            $("#Register #FinancialMarketType").focus();
    }
    else {
        $("#Register #FinancialMarketType").val("");
        $("#Register #FinancialMarketType").attr("disabled", true);

        if (istab)
            $("#Register #IDNo").focus();
    }

    var obj = {
        CompanyTypeCode: $("#Register #CompanyType").val()
    };
    call_ajax_method_json("/Master/MAS050_GetCompanyData", obj, function (result) {
        currentCompany = null;
        if (result != undefined) {
            currentCompany = result;
        }

        DisplayFullNameEN();
        DisplayFullNameLC();
    });
}


function DummyFlagChange() {
    if ($(this).prop("checked") == true) {
        $("#Register #IDNo").attr("readonly", true);
        $("#Register #IDNo").val("");
    }
    else {
        $("#Register #IDNo").removeAttr("readonly");
    }
}
function CustomerNameEN_change() {
    DisplayFullNameEN();
}
function CustomerNameLC_change() {
    DisplayFullNameLC();
}
function cboJangwatEN_change(istab, isblur) {
    $("#Register #ProvinceLC").val($("#Register #ProvinceEN").val());
    $("#Register #ProvinceLC").ResetToNormalControl();

    if (isblur) {
        var Prov = {
            provinceCode: $("#Register #ProvinceEN").val()
        };

        call_ajax_method_json("/Master/GetAmphorEN", Prov, update_amperkedEN_combo);
        call_ajax_method_json("/Master/GetAmphorLC", Prov, update_amperkedLC_combo);
    }
}
function cboJangwatLC_change(istab, isblur) {
    $("#Register #ProvinceEN").val($("#Register #ProvinceLC").val());
    $("#Register #ProvinceEN").ResetToNormalControl();

    if (isblur) {
        var Prov = {
            provinceCode: $("#Register #ProvinceLC").val()
        };

        call_ajax_method_json("/Master/GetAmphorEN", Prov, update_amperkedEN_combo);
        call_ajax_method_json("/Master/GetAmphorLC", Prov, update_amperkedLC_combo);
    }
}
function ChangeLC() {
    $("#Register #DistrictLC").val($("#Register #DistrictEN").val());
    $("#Register #DistrictLC").ResetToNormalControl();
}
function ChangeEN() {
    $("#Register #DistrictEN").val($("#Register #DistrictLC").val());
    $("#Register #DistrictEN").ResetToNormalControl();
}






function ConfirmData() {
    var obj = CreateObjectData($("#Register").serialize());
    obj.PhoneNo = $("#Register #TelePhoneNo").val();

    if ($("#Register #Dummy").length > 0)
        obj.DummyIDFlag = $("#Register #Dummy").prop("checked");

    obj.IsFullValidate = $("#IsFullValidate").val();

    obj.CustTypeCode = "";
    obj.CustTypeName = "";
    if ($("#Register #CustomerType").val() != "") {
        obj.CustTypeCode = $("#Register #CustomerType").val();
        obj.CustTypeName = $("#Register #CustomerType option:selected").text();
    }
    obj.CompanyTypeCode = "";
    obj.CompanyTypeName = "";
    if ($("#Register #CompanyType").val() != "") {
        obj.CompanyTypeCode = $("#Register #CompanyType").val();
        obj.CompanyTypeName = $("#Register #CompanyType option:selected").text();
    }

    obj.FinancialMarketTypeCode = "";
    obj.FinancialMaketTypeName = "";
    if ($("#Register #FinancialMarketType").val() != "") {
        obj.FinancialMarketTypeCode = $("#Register #FinancialMarketType").val();
        obj.FinancialMaketTypeName = $("#Register #FinancialMarketType option:selected").text();
    }

    obj.RegionCode = "";
    obj.Nationality = "";
    if ($("#Register #Nationality").val() != "") {
        obj.RegionCode = $("#Register #Nationality").val();
        obj.Nationality = $("#Register #Nationality option:selected").text();
    }

    obj.BusinessTypeCode = "";
    obj.BusinessTypeName = "";
    if ($("#Register #BusinessType").val() != "") {
        obj.BusinessTypeCode = $("#Register #BusinessType").val();
        obj.BusinessTypeName = $("#Register #BusinessType option:selected").text();
    }

    obj.ProvinceCode = "";
    obj.ProvinceNameEN = "";
    obj.ProvinceNameLC = "";
    if ($("#Register #ProvinceEN").val() != "") {
        obj.ProvinceCode = $("#Register #ProvinceEN").val();
        obj.ProvinceNameEN = $("#Register #ProvinceEN option:selected").text();

        if ($("#Register #ProvinceLC").val() != "")
            obj.ProvinceNameLC = $("#Register #ProvinceLC option:selected").text();
    }

    obj.DistrictCode = "";
    obj.DistrictNameEN = "";
    obj.DistrictNameLC = "";
    if ($("#Register #DistrictEN").val() != "") {
        obj.DistrictCode = $("#Register #DistrictEN").val();
        obj.DistrictNameEN = $("#Register #DistrictEN option:selected").text();

        if ($("#Register #DistrictLC").val() != "")
            obj.DistrictNameLC = $("#Register #DistrictLC option:selected").text();
    }

    if (gridCustomerGroup != undefined) {
        var vv = new Array();
        if (CheckFirstRowIsEmpty(gridCustomerGroup) == false) {
            var codeCol = gridCustomerGroup.getColIndexById("GroupCode");
            var nameENCol = gridCustomerGroup.getColIndexById("GroupNameEN");
            var nameLCCol = gridCustomerGroup.getColIndexById("GroupNameLC");

            for (var i = 0; i < gridCustomerGroup.getRowsNum(); i++) {
                var oobj = {
                    GroupCode: gridCustomerGroup.cells2(i, codeCol).getValue(),
                    GroupNameEN: gridCustomerGroup.cells2(i, nameENCol).getValue(),
                    GroupNameLC: gridCustomerGroup.cells2(i, nameLCCol).getValue()
                };
                vv.push(oobj);
            }
        }
        obj.CustomerGroupData = vv;
    }

    call_ajax_method_json("/Master/MAS050_ConfirmData", obj,
    function (result, controls) {
        VaridateCtrl([
            "IDNo",
            "CustomerType",
            "IDNo",
            "CustNameEN",
            "CustNameLC",
            "Nationality",
            "BusinessType",
            "AddressEN",
            "RoadEN",
            "SubDistrictEN",
            "ProvinceEN",
            "DistrictEN",
            "AddressLC",
            "RoadLC",
            "SubDistrictLC",
            "ProvinceLC",
            "DistrictLC"], controls);

        if (controls != undefined) {
            return;
        }
        if (result != undefined) {
            if (MAS050Response != undefined)
                MAS050Response(result);
        }
    }
    );
}
/* -------------------------------------------------------------- */

/* --- Methods -------------------------------------------------- */
/* -------------------------------------------------------------- */

function ValidateObjectData() {
    call_ajax_method_json("/Master/MAS050_ValidateData", "",
    function (result, controls) {
        VaridateCtrl([
            "CustomerType",
            "CompanyType",
            "FinancialMarketType",
            "Nationality",
            "BusinessType",
            "AddressEN",
            "RoadEN",
            "SubDistrictEN",
            "ProvinceEN",
            "DistrictEN",
            "AddressLC",
            "RoadLC",
            "SubDistrictLC",
            "ProvinceLC",
            "DistrictLC"], controls);
    });
}

function DisplayFullNameEN() {
    var name = $("#Register #CustNameEN").val();
    if (name != "") {
        if (currentCompany != undefined) {
            if (currentCompany.CustNamePrefixEN != undefined) {
                name = currentCompany.CustNamePrefixEN + " " + name;
            }
            if (currentCompany.CustNameSuffixEN != undefined) {
                name = name + " " + currentCompany.CustNameSuffixEN;
            }
        }
    }
    $("#Register #CustFullNameEN").val(name);
}
function DisplayFullNameLC() {
    var name = $("#Register #CustNameLC").val();
    if (name != "") {
        if (currentCompany != undefined) {
            if (currentCompany.CustNamePrefixLC != undefined) {
                name = currentCompany.CustNamePrefixLC + " " + name;
            }
            if (currentCompany.CustNameSuffixLC != undefined) {
                name = name + " " + currentCompany.CustNameSuffixLC;
            }
        }
    }
    $("#Register #CustFullNameLC").val(name);
}
function update_amperkedEN_combo(data) {
    var sval = $("#Register #DistrictEN").val();
    regenerate_combo("#Register #DistrictEN", data);
    $("#Register #DistrictEN").val(sval);
}
function update_amperkedLC_combo(data) {
    var sval = $("#Register #DistrictLC").val();
    regenerate_combo("#Register #DistrictLC", data);
    $("#Register #DistrictLC").val(sval);
}

/* -------------------------------------------------------------- */