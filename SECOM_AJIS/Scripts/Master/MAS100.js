/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

/* --- Variables --- */
var mygrid;
var pageRow; // = 5;
var btnRemoveId = "MAS100RemoveBtn";
var removeRid;
var popupMode;

$(document).ready(function () {

    pageRow = MAS100Data.PageRow;

    $("#HiddenTextbox").hide(); //this hidden textbox use to solve Bug report MA-110 - MAS100-Press Enter on 'Child instrument code' text box have error

    /* --- Initial grid --- */
    if ($("#grid_result").length > 0) {
        mygrid = $("#grid_result").InitialGrid(pageRow, false, "/Master/InitialGrid_MAS100");
    }

    /*==== event btnRetrieve click ====*/
    $("#btnRetrieve").click(function () {
        $("#ParentInstrumentCode").autocomplete("close");
        doRetrieveAction();
    });
    /*==== event btnSearchParentInstrument click ====*/
    $("#btnSearchParentInstrument").click(function () {
        doSearchParentInstrument();
    });
    /*==== event btnAdd click ====*/
    $("#btnAdd").click(function () {
        $("#ChildInstrumentCode").autocomplete("close");
        doAddAction();
    });
    /*==== event btnSearchChildInstrument click ====*/
    $("#btnSearchChildInstrument").click(function () {
        doSearchChildInstrument();
    });

    initial();

    /* ===== binding event when finish load data ===== */
    SpecialGridControl(mygrid, ["Remove"]);
    BindOnLoadedEvent(mygrid, function (gen_ctrl) {
        var detailColInx = mygrid.getColIndexById('Remove');

        for (var i = 0; i < mygrid.getRowsNum(); i++) {

            var row_id = mygrid.getRowId(i);

            if (gen_ctrl == true) {
                if (MAS100Data.HasDeletePermission == "True") {
                    GenerateRemoveButton(mygrid, btnRemoveId, row_id, "Remove", true);
                } else {
                    GenerateRemoveButton(mygrid, btnRemoveId, row_id, "Remove", false);
                }
            }

            /* ===== Bind event onClick to button ===== */
            BindGridButtonClickEvent(btnRemoveId, row_id, function (rid) {
                doRemoveAction(rid);
            });
        }
    });
});

function initial() {

    $("#ParentInstrumentCode").removeClass("highlight");
    $("#ChildInstrumentCode").removeClass("highlight");

    $("#ParentInstrumentCode").InitialAutoComplete("/Master/GetInstrumentCodeExpansionParent", null, true);
    $("#ChildInstrumentCode").InitialAutoComplete("/Master/GetInstrumentCodeExpansionChild", null, true);

    $("#ParentInstrumentCode").val("");
    $("#ParentInstrumentName").val("");
    $("#ChildInstrumentCode").val("");

    $("#ChildInst_Section").hide();
    setEnableParentSection();
    $("#ParentInst_Section").show();
    
    SetConfirmCommand(false, null);
    SetClearCommand(false, null);
}

function doRetrieveAction() {

    // disable button btnRetrieve, btnSearchParentInstrument
    $("#btnRetrieve").attr("disabled", true);
    $("#btnSearchParentInstrument").attr("disabled", true);
    master_event.LockWindow(true);

    var parentInstCode = { "ParentInstrumentCode": $("#ParentInstrumentCode").val() };

    call_ajax_method(
        '/Master/MAS100_CheckReqField/',
        parentInstCode,
        function (result, controls) {

            // enable button btnRetrieve, btnSearchParentInstrument
            $("#btnRetrieve").attr("disabled", false);
            $("#btnSearchParentInstrument").attr("disabled", false);
            master_event.LockWindow(false);

            if (controls != undefined) {
                VaridateCtrl(["ParentInstrumentCode"], controls);
                $("#ParentInstrumentName").val("");
                $("#ParentInstrumentCode").focus();
                return;
            } else if (result != undefined) {
                $("#ParentInstrumentName").val(result);
                var param = null;
                retrieveData(param);
            }
        }
    );
}

function retrieveData(param) {

    $("#grid_result").LoadDataToGrid(mygrid, pageRow, false, "/Master/MAS100_Retrieve", param, "doInstrumentExpansion", false,
        null, //post-load
        function (result, controls, isWarning) { //pre-load
            if (isWarning == undefined) {
                $("#ChildInst_Section").show();
            }
        });

    if (MAS100Data.HasAddPermission == "False") {
        $("#ChildInstrumentCode").attr("disabled", true);
        $("#btnAdd").attr("disabled", true);
        $("#btnSearchChildInstrument").attr("disabled", true);
    } else {
        $("#ChildInstrumentCode").removeAttr("disabled");
        $("#btnAdd").removeAttr("disabled");
        $("#btnSearchChildInstrument").removeAttr("disabled");
    }
    $("#ChildInstrumentCode").val("");

    if (MAS100Data.HasAddPermission == "True" || MAS100Data.HasDeletePermission == "True") {
        SetConfirmCommand(true, doConfirmAction);
    } else {
        SetConfirmCommand(false, null);
    }

    SetClearCommand(true, doClearAction);

    setDisableParentSection();

    $("#ChildInst_Section").show();
}

function setEnableParentSection() {
    $("#ParentInstrumentCode").attr("readonly", false);
    $("#btnRetrieve").removeAttr("disabled");
    $("#btnSearchParentInstrument").removeAttr("disabled");
}

function setDisableParentSection() {
    $("#ParentInstrumentCode").attr("readonly", true);
    $("#btnRetrieve").attr("disabled", true);
    $("#btnSearchParentInstrument").attr("disabled", true);
}

/* ----------------------------------------------------------------------------------- */
/* --- Dialog Methods (CMS170) ------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
var doInstrumentParam = null;
function CMS170Object() {
    return doInstrumentParam;
}

function doSearchParentInstrument() {

    // disable
    //$("#btnSearchParentInstrument").attr("disabled", true);

    popupMode = MAS100Data.InstExpParent;
    var obj = {
        bExpTypeHas: true,
        bExpTypeNo: false,
        bProdTypeSale: false,
        bProdTypeAlarm: false,
        bInstTypeGen: false,
        bInstTypeMonitoring: false,
        bInstTypeMat: false
    };
    doInstrumentParam = obj;
    $("#dlgMAS100").OpenCMS170Dialog("MAS100");

    // enable button (set delay 3 sec.)
    //setTimeout(
    //function () {
    //    $("#btnSearchParentInstrument").attr("disabled", false);
    //}, 3000);
}
function doSearchChildInstrument() {

    $("#ChildInstrumentCode").removeClass("highlight");
    
    popupMode = MAS100Data.InstExpChild;
    var obj = {
        bExpTypeHas: false,
        bExpTypeNo: true,
        bProdTypeSale: false,
        bProdTypeAlarm: false,
        bInstTypeGen: false,
        bInstTypeMonitoring: false,
        bInstTypeMat: false
    };
    doInstrumentParam = obj;
    $("#dlgMAS100").OpenCMS170Dialog("MAS100");

    
}

function CMS170Response(result) {
    $("#dlgMAS100").CloseDialog();

    if (popupMode == MAS100Data.InstExpParent) {
        $("#ParentInstrumentCode").val(result.InstrumentCode);
        $("#ParentInstrumentName").val(result.InstrumentName);

        var param = { "ParentInstrumentCode": result.InstrumentCode };
        retrieveData(param);

    } else if (popupMode == MAS100Data.InstExpChild) {
        $("#ChildInstrumentCode").val(result.InstrumentCode);
    }
}
/* ----------------------------------------------------------------------------------- */

function doAddAction() {

    $("#ChildInstrumentCode").removeClass("highlight");

    var childInstCode = { "ChildInstrumentCode": $("#ChildInstrumentCode").val() };

    call_ajax_method(
        '/Master/MAS100_Add/',
        childInstCode,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["ChildInstrumentCode"], controls);
                $("#ChildInstrumentCode").focus();
                return;
            } else if (result != undefined) {
                AddDataToGrid(result);
                $("#ChildInstrumentCode").val("");
                $("#ChildInstrumentCode").focus();
            }
        }
    );
}

function AddDataToGrid(result) {
    /* --- Check Empty Row --- */
    /* ----------------------- */
    CheckFirstRowIsEmpty(mygrid, true);
    /* ----------------------- */

    /* --- Add new row --- */
    /* ------------------- */
    AddNewRow(mygrid,
        [ConvertBlockHtml(result.InstrumentCode), //result.InstrumentCode, //Modify by Jutarat A. on 28112013
        ConvertBlockHtml(result.InstrumentName), //result.InstrumentName, //Modify by Jutarat A. on 28112013
        result.InstrumentNameForSupplier,
        result.InstrumentCategory1,
        result.InstrumentCategory2,
        result.InstrumentCategory3,
        result.LineUpTypeName,
        ""]);
    /* ------------------- */

    var row_idx = mygrid.getRowsNum() - 1;
    var row_id = mygrid.getRowId(row_idx);

    if (MAS100Data.HasDeletePermission == "True") {
        GenerateRemoveButton(mygrid, btnRemoveId, row_id, "Remove", true);
    } else {
        GenerateRemoveButton(mygrid, btnRemoveId, row_id, "Remove", false);
    }

    mygrid.setSizes();

    /* --- Set Event --- */
    /* ----------------- */
    BindGridButtonClickEvent(btnRemoveId, row_id, function (rid) {
        doRemoveAction(rid);
    });
    /* ----------------- */

    //Set grid to last page
    mygrid.changePage(Math.ceil(mygrid.getRowsNum() / pageRow));
}

function doRemoveAction(rid) {

    //hilight row
    mygrid.selectRow(mygrid.getRowIndex(rid));

    removeRid = rid;
    
    var code = mygrid.cells2(mygrid.getRowIndex(removeRid), mygrid.getColIndexById('InstrumentCode')).getValue();
    var childInstCode = { "ChildInstrumentCode": code };

    call_ajax_method(
        '/Master/MAS100_Remove/',
        childInstCode,
        function (result, controls) {
            if (result != undefined) {
                DeleteRow(mygrid, removeRid);
            }
        }
    );
}

function doConfirmAction() {

    // disable confrim button
    DisableConfirmCommand(true);

    call_ajax_method(
        '/Master/MAS100_Confirm/',
        "",
        function (result, controls) {

            if (result != undefined) {
                OpenInformationMessageDialog(result.Code, result.Message, function () {
                    initial();

                    // enable confrim button
                    DisableConfirmCommand(false);
                })
            }
            else {
                // enable confrim button
                DisableConfirmCommand(false);
            }
        }
    );
}

function doClearAction() {

    var param = { "module": "Common", "code": "MSG0044" }; // "param": "clear" };
    call_ajax_method("/Shared/GetMessage", param, function (data) {

        /* ====== Open confirm dialog =====*/
        OpenYesNoMessageDialog(data.Code, data.Message, function () {
            initial();
        }, null);

        return false;
    });
}
