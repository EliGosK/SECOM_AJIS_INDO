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

/// <reference path="../Base/object/ajax_method.js" />
/// <reference path="../Base/object/command_event.js" />
/// <reference path="../Base/object/master_event.js" />




var gridCheckingDetail;
var IVS160 = {
    PageInfo: {
        CurrentPage: 0,
        TotalPage: 0,
        TotalItem: 0,
        TextPageInfo: ""
    },
    ScreenMode: {
        Register: "RegisterMode",
        Confirm: "ConfirmMode",
        Finish: "FinishMode",
        NewRegister: "NewRegisterMode",
        Reset: "Reset"
    }
};

var isViewMode = false; //Add by Jutarat A. on 27022013

$(document).ready(function () {

    // Initial grid
    if ($.find("#IVS160_RegisterInstrumentCheckingDetailGrid").length > 0) {
        gridCheckingDetail = $("#IVS160_RegisterInstrumentCheckingDetailGrid").InitialGrid(0, true, "/Inventory/IVS160_InitialSearchResultGrid");
    }


    //Binding  (adjustment detail)
    SpecialGridControl(gridCheckingDetail, ["CheckingQty"]);
    BindOnLoadedEvent(gridCheckingDetail, function () {
        //var colInx = gridCheckingDetail.getColIndexById('BtnRemove');
        for (var i = 0; i < gridCheckingDetail.getRowsNum(); i++) {
            var rowId = gridCheckingDetail.getRowId(i);

            //GenerateAddButton(gridCheckingDetail, "btnRemove", rowId, "BtnRemove", true);
            ////binding grid button event 
            //BindGridButtonClickEvent("btnRemove", rowId, RemoveReturnInstrument); // you must defind -> funtion RemoveReturnInstrument(){}

            // generate numeric text box in grid
            var qtyColIndex = gridCheckingDetail.getColIndexById("CheckingQty");
            var val = GetValueFromLinkType(gridCheckingDetail, i, qtyColIndex);
            GenerateNumericBox2(gridCheckingDetail, "txtCheckingQty", rowId, "CheckingQty", val, 5, 0, 0, 99999, false, true);

            var strJson = gridCheckingDetail.cells2(i, gridCheckingDetail.getColIndexById("ToJson")).getValue();
            var data = JSON.parse(htmlDecode(strJson));
            var txtCheckingQty_id = "#" + GenerateGridControlID("txtCheckingQty", rowId);
            $(txtCheckingQty_id).attr("default-value", data.DefaultCheckingQty);
        }

        $("input[id^=txtCheckingQty]").each(function () {
            $(this).on("focus", function () {
                $(this).css("color", "#000000");
                if ($(this).NumericValue() == $(this).attr("default-value")) {
                    $(this).val("");
                }
            });

            $(this).on("blur", function () {
                if ($(this).val() == "" || $(this).NumericValue() == $(this).attr("default-value")) {
                    $(this).prop("user-input", false);
                }
                else {
                    $(this).prop("user-input", true);
                }
                if ($(this).prop("user-input")) {
                    $(this).css("color", "#000000");
                }
                else {
                    $(this).css("color", "#a8a8a8");
                    $(this).val(SetNumericText($(this).attr("default-value"), 0));
                }
            });

            $(this).blur();
        });

        gridCheckingDetail.setSizes();
    });


    // Event binding
    $("#btnSearch").click(IVS160_Search);
    $("#btnAdd").click(AddInstrumentIntoList);
    $("#btnClearAddData").click(ClearAddData);
    $("#btnNewRegister").click(NewRegister);
    $("#InstrumentCode").blur(GetInstrumentName);
    $("#lnkBack").click(BackPage);
    $("#btnGoto").click(GotoPage);
    $("#lnkForward").click(ForwardPage)

    $("#OfficeCode option[value='9000']")
        .detach()
        .insertBefore($("#OfficeCode option:nth-child(2)"));

    $("#OfficeCode").change(function () {
        var selectedOfficeCode = $("#OfficeCode").val();

        if (selectedOfficeCode != "" && selectedOfficeCode != IVS160_ViewBag.Office_InventoryHQ) {
            $("#LocationCode").val(IVS160_ViewBag.Location_InStock);
        }
        else {
            $("#LocationCode").val("");
        }

        $("#LocationCode > option, #LocationCode > span").each(function () {
            var opt, selectedLocation, ishidden;

            if (this.tagName.toLowerCase() == "option") {
                opt = $(this);
                ishidden = false;
            }
            else {
                opt = $(this).find('option');
                ishidden = true;
            }

            selectedLocation = opt.val();

            if (selectedOfficeCode == ""
                || selectedLocation == ""
                || selectedLocation == IVS160_ViewBag.Location_InStock
                || selectedOfficeCode == IVS160_ViewBag.Office_InventoryHQ
            ) {
                if (ishidden) {
                    $(this).replaceWith(opt);
                    opt.hide();
                }
            }
            else {
                if (!ishidden) {
                    opt.wrap('<span>').show();
                }
            }
        });
    });

    // Open dialog screen
    $("#btnSearchInstrument").click(function () {
        $("#dlgBox").OpenCMS170Dialog("IVS160");
    });


    InitialPage();
});

function InitialPage() {

    $("#divRegisterInstrumentChecking").show();
    $("#divAddInstrument").hide();
    $("#divRegisterInstumentCheckingDetail").hide();
    $("#divShowSlipNo").hide();

    $("#CheckingDate").InitialDate();

    var now = new Date();
    $("#CheckingDate").val(ConvertDateToTextFormat(ConvertDateObject(now)));

    InitialNumericInputTextBox(["Page"], false);

    // back
    $("#divBack").hide();
    $("#divBackText").show();

    // forward
    $("#divForward").hide();
    $("divForwardText").show();

    GetPageInfo();
}


//Search
function IVS160_Search() {

    // For prevent click this button more than one time
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    var obj = {
        CheckingYearMonth: $("#CheckingYearMonth").val(),
        OfficeCode: $("#OfficeCode").val(),
        LocationCode: $("#LocationCode").val(),
        AreaCode: $("#AreaCode").val()
    };
    $("#IVS160_RegisterInstrumentCheckingDetailGrid").LoadDataToGrid(gridCheckingDetail, 0, true, "/Inventory/IVS160_SearchResponse", obj, "dtCheckingDetailList", false,
        function (result, controls) { // post-load

            if (!(
                $("#OfficeCode").val() == IVS160_ViewBag.Office_InventoryHQ
                && $("#LocationCode").val() == IVS160_ViewBag.Location_InStock
            )) {
                $("#ShelfNo").val(IVS160_ViewBag.C_INV_SHELF_NO_OTHER_LOCATION);
                $("#ShelfNo").SetDisabled(true);
            }

            // Get page info
            GetPageInfo();

            if (result != undefined) {
                SetScreenMode(IVS160.ScreenMode.Register);
                master_event.LockWindow(false);
            }
            else if (controls != undefined) {
                VaridateCtrl(["OfficeCode", "LocationCode"], controls);
                // For prevent click this button more than one time
                $("#btnSearch").attr("disabled", false);
                master_event.LockWindow(false);
            }
        },
        function (result, controls, isWarning) { // pre-load
            if (isWarning == undefined) {
                $("#divRegisterInstumentCheckingDetail").show();
            }
        });

}




// AddInstrument
function AddInstrumentIntoList() {
    if (CheckFirstRowIsEmpty(gridCheckingDetail)) {
        return false;
    }

    $("#btnAdd").attr("disabled", true);

    var dataGridCurrentPage = GetUserAdjustData();
    var addedData = GetAddData();

    var obj = {
        OfficeCode: $("#OfficeCode").val(),
        gridCurrentPage: dataGridCurrentPage,
        data: addedData
    };

    $("#IVS160_RegisterInstrumentCheckingDetailGrid").LoadDataToGrid(gridCheckingDetail, 0, true, "/Inventory/IVS160_AddInstrumentToList", obj, "dtCheckingDetailList", false,
        function (result, controls, isWarning) { // post-load
            // For prevent click this button more than one time
            $("#btnAdd").attr("disabled", false);

            if (result != undefined) {

                for (var i = 0; i < gridCheckingDetail.getRowsNum(); i++) {
                    var row_id = gridCheckingDetail.getRowId(i);
                    var strJson = gridCheckingDetail.cells2(i, gridCheckingDetail.getColIndexById("ToJson")).getValue();
                    var data = JSON.parse(htmlDecode(strJson));

                    if (data.AddFlag) {
                        gridCheckingDetail.setRowColor(row_id, "#FFFFCC")
                    }
                }

                GetPageInfo();
                $("#formAddInstrument").clearForm();
                $("#InstrumentCode").focus();

                if (!(
                    $("#OfficeCode").val() == IVS160_ViewBag.Office_InventoryHQ
                    && $("#LocationCode").val() == IVS160_ViewBag.Location_InStock
                )) {
                    $("#ShelfNo").val(IVS160_ViewBag.C_INV_SHELF_NO_OTHER_LOCATION);
                    $("#ShelfNo").SetDisabled(true);
                }
            }
            else if (controls != undefined) {
                VaridateCtrl(["InstrumentCode", "InstrumentArea", "ShelfNo"], controls);
            }

            // Enable Comman Register button
            if (CheckFirstRowIsEmpty(gridCheckingDetail, false) == true) {
                register_command.SetCommand(null);
            }
            else {
                register_command.SetCommand(command_register_click);
            }

        },
        function (result, controls, isWarning) { // pre-load
            if (isWarning == undefined) {

            }
        });

}

// ClearAddData
function ClearAddData() {
    $("#formAddInstrument").clearForm();

    if (!(
        $("#OfficeCode").val() == IVS160_ViewBag.Office_InventoryHQ
        && $("#LocationCode").val() == IVS160_ViewBag.Location_InStock
    )) {
        $("#ShelfNo").val(IVS160_ViewBag.C_INV_SHELF_NO_OTHER_LOCATION);
        $("#ShelfNo").SetDisabled(true);
    }
}

// NewRegister
function NewRegister() {
    SetScreenMode(IVS160.ScreenMode.NewRegister);
}


// Register click
function command_register_click() {

    var obj = {
        gridCurrentPage: GetUserAdjustData(),
        checkingDate: $("#CheckingDate").val()
    };

    ajax_method.CallScreenController("/Inventory/IVS160_Register", obj,
    function (result, controls, isWaning) {
        if (result != undefined) {

            //var param = { isShowAtCurrentPage: false };
            var param = { isShowAtCurrentPage: true }; //Test

            $("#IVS160_RegisterInstrumentCheckingDetailGrid").LoadDataToGrid(gridCheckingDetail, 0, true, "/Inventory/IVS160_GetAllDetailForView", param, "dtCheckingDetailList", false,
            function (data, controls, isWaning) { // post-load
                if (data != undefined) {
                    SetScreenMode(IVS160.ScreenMode.Confirm);
                }
            },
            function (data, controls, isWaning) {// pre -load
                if (isWaning == undefined) {
                    $("#divRegisterInstumentCheckingDetail").show();
                }
            });
        }
        else {
            VaridateCtrl(controls, controls);
        }
    });

}

// Confrim click
function command_confirm_click() {
    // Call ajax to save
    ajax_method.CallScreenController("/Inventory/IVS160_Confirm", "",
        function (result, controls, isWarning) {
            if (result != undefined) {
                if (result != "") {
                    // Set slip no to textbox
                    $("#SlipNo").val(result);
                    SetScreenMode(IVS160.ScreenMode.Finish);
                }
            }
            else if (controls != undefined) {
                // Validate again -> but not pass then set screen to input mode and hilight controls
                SetScreenMode(IVS160.ScreenMode.Register);
                VaridateCtrl(controls, controls);
            }
        });

}

// Reset click
function command_reset_click() {
    SetScreenMode(IVS160.ScreenMode.Reset);
}

// Back click
function command_back_click() {
    //SetScreenMode(IVS160.ScreenMode.Register); //Move by Jutarat A. on 27022013
    var param = { isShowAtCurrentPage: true };
    $("#IVS160_RegisterInstrumentCheckingDetailGrid").LoadDataToGrid(gridCheckingDetail, 0, true, "/Inventory/IVS160_GetAllDetailForView", param, "dtCheckingDetailList", false,
            function (data, controls, isWaning) { // post-load
                if (data != undefined) {
                    SetScreenMode(IVS160.ScreenMode.Register); //Move by Jutarat A. on 27022013
                }

            },
            function (data, controls, isWaning) {// pre -load
                if (isWaning == undefined) {
                    $("#divRegisterInstumentCheckingDetail").show();
                }
            });
}

function BackPage() {
    CallGotoPage("back");
    $("#Page").val("");
}

function GotoPage() {
    var page = $("#Page").val();
    if (page != "") {
        var iPage = parseInt(page);
        if ((iPage > 0 && iPage <= IVS160.PageInfo.TotalPage) && iPage != IVS160.PageInfo.CurrentPage) {
            CallGotoPage("goto", page);
        }
    }
    $("#Page").val("");
}

function ForwardPage() {
    CallGotoPage("forward");
    $("#Page").val("");
}

function CallGotoPage(type, page) {

    var dataGridCurrentPage = GetUserAdjustData();
    var obj = {
        gridCurrentPage: dataGridCurrentPage,
        type: type,
        page: page
    };

    $("#IVS160_RegisterInstrumentCheckingDetailGrid").LoadDataToGrid(gridCheckingDetail, 0, true, "/Inventory/IVS160_GotoPage", obj, "dtCheckingDetailList", false,
        function (result, controls, isWarning) { // post-load
            if (result != undefined) {
                GetPageInfo();

                //Add by Jutarat A. on 27022013
                if (isViewMode) {
                    $("#divRegisterInstumentCheckingDetailGrid").SetViewMode(true);
                }
                //End Add
            }
        },
        function (result, controls, isWarning) { // pre-load
            if (isWarning == undefined) {

            }
        });

}


//---- For CMS170 dialog ----
function CMS170Object() {
    return { bExpTypeHas: true,
        bExpTypeNo: true,
        bProdTypeSale: false,
        bProdTypeAlarm: false,
        bInstTypeGen: true,
        bInstTypeMonitoring: true,
        bInstTypeMat: true
    };
}

//---- For CMS170 dialog ----
function CMS170Response(result) {
    $("#dlgBox").CloseDialog();

    if (result != undefined) {
        $("#InstrumentCode").val(result.InstrumentCode);
        $("#InstrumentName").val(result.InstrumentName);
        $("#btnSearchInstrument").focus();
    }

}

//-------- Screen mode -------------

function SetScreenMode(mode) {
    isViewMode = false; //Add by Jutarat A. on 27022013

    if (mode == IVS160.ScreenMode.Register) {

        $("#CheckingYearMonthSection").show();
        DisableSearchSection(true);
        $("#divRegisterInstrumentChecking").show();
        //$("#divAddInstrument").show();
        $("#divRegisterInstumentCheckingDetail").show();
        $("#divShowSlipNo").hide();


        if (CheckFirstRowIsEmpty(gridCheckingDetail, false) == true) {
            register_command.SetCommand(null);
            $("#divAddInstrument").hide();
        }
        else {
            register_command.SetCommand(command_register_click);
            $("#divAddInstrument").show();
        }


        reset_command.SetCommand(command_reset_click);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);


        $("#divRegisterInstumentCheckingDetail").SetViewMode(false);

        // Page info
        $("#divPageInfo").show();
        $("#PageInfo").show();
    }
    else if (mode == IVS160.ScreenMode.Confirm) {

        $("#CheckingYearMonthSection").hide();
        DisableSearchSection(true);
        $("#divRegisterInstrumentChecking").hide();
        $("#divAddInstrument").hide();
        $("#divRegisterInstumentCheckingDetail").show();
        $("#divShowSlipNo").hide();

        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(command_confirm_click);
        back_command.SetCommand(command_back_click);

        //Modify by Jutarat A. on 27022013
        //$("#divRegisterInstumentCheckingDetail").SetViewMode(true);
        $("#divCheckingDate").SetViewMode(true);
        $("#divRegisterInstumentCheckingDetailGrid").SetViewMode(true);

//        // Page info
//        $("#divPageInfo").hide();
//        $("#PageInfo").hide();
        //End Modify

        isViewMode = true; //Add by Jutarat A. on 27022013
    }
    else if (mode == IVS160.ScreenMode.Finish) {

        $("#CheckingYearMonthSection").hide();
        DisableSearchSection(false);
        $("#divRegisterInstrumentChecking").hide();
        $("#divAddInstrument").hide();
        $("#divRegisterInstumentCheckingDetail").show();
        $("#divShowSlipNo").show();

        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);

        // Page info
        $("#divPageInfo").hide();
        $("#PageInfo").hide();

        master_event.ScrollWindow("#divShowSlipNo");
    }
    else if ((mode == IVS160.ScreenMode.NewRegister) || (mode == IVS160.ScreenMode.Reset)) {

        $("#CheckingYearMonthSection").show();
        DisableSearchSection(false);
        $("#divRegisterInstrumentChecking").show();
        $("#divAddInstrument").hide();
        $("#divRegisterInstumentCheckingDetail").hide();
        $("#divShowSlipNo").hide();

        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);

        $("#divRegisterInstumentCheckingDetail").SetViewMode(false);


        $("#formSearchCriteria").clearForm();
        $("#formAddInstrument").clearForm();

        $("#ShelfNo").SetDisabled(false);
        
        // Clear row
        DeleteAllRow(gridCheckingDetail);

        // Page info
        $("#divPageInfo").show();
        $("#PageInfo").show();
    }

}

function DisableSearchSection(disable) {
    $("#OfficeCode").attr("disabled", disable);
    $("#LocationCode").attr("disabled", disable);
    $("#AreaCode").attr("disabled", disable);
    $("#btnSearch").attr("disabled", disable);
}


//--------- [Private fucntion] ---------

function GetAddData() {

    var strAreaCodeName = $("#InstrumentArea option[selected='selected']").text();
    var arrAreaCodeName = strAreaCodeName.split(":");
    var strAreaName;
    if (arrAreaCodeName.length >= 2) {
        strAreaName = $.trim(arrAreaCodeName[1]);
    }

    var obj = {
        InstrumentCode: $("#InstrumentCode").val(),
        AreaCode: $("#InstrumentArea").val(),
        ShelfNo: $("#ShelfNo").val(),

        // extra field
        InstrumentName: $("#InstrumentName").val(),
        AreaCodeName: strAreaCodeName,
        AreaName: strAreaName
    };

    return obj;
}

function GetUserAdjustData() {
    var arr = new Array();
    var grid = gridCheckingDetail;

    if (CheckFirstRowIsEmpty(grid) == false) {
        for (var i = 0; i < grid.getRowsNum(); i++) {

            var row_id = grid.getRowId(i);
            var txtCheckingQty_id = GenerateGridControlID("txtCheckingQty", row_id);

            var strJson = grid.cells2(i, grid.getColIndexById("ToJson")).getValue();
            var data = JSON.parse(htmlDecode(strJson));

            var obj = {
                txtCheckingQtyID: txtCheckingQty_id,
                ShelfNo: data.ShelfNo,
                AreaCode: data.AreaCode,
                InstrumentCode: data.InstrumentCode,
                CheckingQty: ($("#" + txtCheckingQty_id).val() == "", null, $("#" + txtCheckingQty_id).NumericValue())
            };

            arr.push(obj);
        }
    }

    return arr;

}

function GetInstrumentName() {
    if ($("#InstrumentCode").val() != "") {
        var obj = { InstrumentCode: $("#InstrumentCode").val() };
        ajax_method.CallScreenController("/Inventory/IVS160_GetInstrumentName", obj, function (result) {
            if (result != undefined) {
                $("#InstrumentCode").val(result.InstrumentCode);
                $("#InstrumentName").val(result.InstrumentName);
            }
            else {
                $("#InstrumentName").val("");
                $("#InstrumentCode").val("");
            }
        });
    }
    else {
        $("#InstrumentName").val("");
    }
}

function GetPageInfo() {
    ajax_method.CallScreenController("/Inventory/IVS160_GetPageInfo", "", function (result) {
        if (result != undefined) {
            IVS160.PageInfo.CurrentPage = result.CurrentPage;
            IVS160.PageInfo.TotalPage = result.TotalPage;
            IVS160.PageInfo.TotalItem = result.TotalItem;
            IVS160.PageInfo.TextPageInfo = result.TextPageInfo;
        }

        $("#PageInfo").html(IVS160.PageInfo.TextPageInfo);

        if (IVS160.PageInfo.TotalPage > 0) {
            // Back
            if (IVS160.PageInfo.CurrentPage > 1) {
                $("#divBack").show();
                $("#divBackText").hide();
            }
            else {
                $("#divBack").hide();
                $("#divBackText").show();
            }

            //Forward
            if (IVS160.PageInfo.CurrentPage < IVS160.PageInfo.TotalPage) {
                $("#divForward").show();
                $("#divForwardText").hide();
            }
            else {
                $("#divForward").hide();
                $("#divForwardText").show();
            }

            // Goto
            $("#btnGoto").attr("disabled", !(IVS160.PageInfo.TotalPage > 1));
        }
        else {
            $("#divBack").hide();
            $("#divBackText").show();

            $("#divForward").hide();
            $("#divForwardText").show();

            $("#btnGoto").attr("disabled", true);
        }
    });
}


