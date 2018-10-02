/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Base/GridControl.js" />

/* --- Variable ------------------------------------------------------- */
/* -------------------------------------------------------------------- */
var gridEmailList = null;
var btnRemoveEmail = "btnRemoveEmail";
/* -------------------------------------------------------------------- */

function SetCTS010_11_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#divEmailInfo").SetViewMode(true);
        $("#divEmailInput").hide();

        if (gridEmailList != undefined) {
            var removeCol = gridEmailList.getColIndexById("Remove");

            gridEmailList.setColumnHidden(removeCol, true);
            gridEmailList.setSizes();
        }
    }
    else {
        $("#divEmailInfo").SetViewMode(false);
        $("#divEmailInput").show();

        if (gridEmailList != undefined) {
            var removeCol = gridEmailList.getColIndexById("Remove");
            gridEmailList.setColumnHidden(removeCol, false);
            for (var i = 0; i < gridEmailList.getRowsNum(); i++) {
                gridEmailList.setColspan(gridEmailList.getRowId(i), removeCol, 2);
            }
        }
    }
}
function SetCTS010_11_EnableSection(enable) {
    $("#divEmailInfo").SetEnableView(enable);
    $("#gridEmailList").find("img").each(function () {
        if (this.id != "" && this.id != undefined) {
            if (this.id.indexOf(btnRemoveEmail) == 0) {
                var row_id = GetGridRowIDFromControl(this);
                EnableGridButton(gridEmailList, btnRemoveEmail, row_id, "Remove", enable);
            }
        }
    });
}

/* --- Inital ------------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function AddEmail(mails, hilight, frompopup) {
    /* --- Set Parameter --- */
    /* --------------------- */
    var obj = {
        FromPopUp: frompopup,
        NewEmailAddressList: mails
    };
    /* --------------------- */

    /* --- Check and Add event --- */
    /* --------------------------- */
    call_ajax_method_json("/Contract/CTS010_CheckBeforeAddEmailAddress", obj, function (result, controls) {
        /* --- Higlight Text --- */
        /* --------------------- */
        if (hilight == true) {
            VaridateCtrl(["EmailAddress"], controls);
        }
        else {
            $("#EmailAddress").val("");
            $("#EmailAddress").ResetToNormalControl();
        }
        /* --------------------- */

        if (controls != undefined) {
            return;
        }
        else if (result != undefined) {
            /* --- Check Empty Row --- */
            /* ----------------------- */
            CheckFirstRowIsEmpty(gridEmailList, true);
            /* ----------------------- */

            for (var idx = 0; idx < result.length; idx++) {
                /* --- Add new row --- */
                /* ------------------- */
                AddNewRow(gridEmailList,
                    [result[idx].EmailAddress, ""]);
                /* ------------------- */

                var row_idx = gridEmailList.getRowsNum() - 1;
                var row_id = gridEmailList.getRowId(row_idx);
                    
                GenerateRemoveButton(gridEmailList, btnRemoveEmail, row_id, "Remove", true);

                /* --- Set Event --- */
                /* ----------------- */
                BindGridButtonClickEvent(btnRemoveEmail, row_id, function (rid) {
                    DeleteEmail(rid);
                });
                /* ----------------- */
            }
            gridEmailList.setSizes();

            /* --- Clear data --- */
            /* ------------------ */
            $("#EmailAddress").val("");
            $("#EmailAddress").ResetToNormalControl();
            /* ------------------ */
        }
    });
    /* --------------------------- */
}

function AddEmailFromPopup(mails, hilight, frompopup) {
    /* --- Set Parameter --- */
    /* --------------------- */
    var obj = {
        FromPopUp: frompopup,
        NewEmailAddressList: mails
    };
    /* --------------------- */

    /* --- Check and Add event --- */
    /* --------------------------- */
    call_ajax_method_json("/Contract/CTS010_AddEmailAddressFromPopup", obj, function (result, controls) {
        /* --- Higlight Text --- */
        /* --------------------- */
        if (hilight == true) {
            VaridateCtrl(["EmailAddress"], controls);
        }
        else {
            $("#EmailAddress").val("");
            $("#EmailAddress").ResetToNormalControl();
        }
        /* --------------------- */

        if (controls != undefined) {
            return;
        }
        else if (result != undefined) {
            /* --- Check Empty Row --- */
            /* ----------------------- */
            CheckFirstRowIsEmpty(gridEmailList, true);
            /* ----------------------- */

            for (var idx = 0; idx < result.length; idx++) {
                /* --- Add new row --- */
                /* ------------------- */
                AddNewRow(gridEmailList,
                    [result[idx].EmailAddress, "", "", result[idx].EmpNo]);
                /* ------------------- */

                var row_idx = gridEmailList.getRowsNum() - 1;
                var row_id = gridEmailList.getRowId(row_idx);

                GenerateRemoveButton(gridEmailList, btnRemoveEmail, row_id, "Remove", true);

                /* --- Set Event --- */
                /* ----------------- */
                BindGridButtonClickEvent(btnRemoveEmail, row_id, function (rid) {
                    DeleteEmail(rid);
                });
                /* ----------------- */
            }
            gridEmailList.setSizes();

            /* --- Clear data --- */
            /* ------------------ */
            $("#EmailAddress").val("");
            $("#EmailAddress").ResetToNormalControl();
            /* ------------------ */
        }
    });
    /* --------------------------- */
}

function DeleteEmail(rid) {
    var codeCol = gridEmailList.getColIndexById("EmailAddress");

    var obj = {
        EmailAddress: gridEmailList.cells(rid, codeCol).getValue()
    };

    call_ajax_method_json("/Contract/CTS010_RemoveEmailAddress", obj, function (result, controls) {
        DeleteRow(gridEmailList, rid);
    });
}

function CTS010_11_InitialGrid() {
    gridEmailList = $("#gridEmailList").LoadDataToGridWithInitial(0, false, false,
                                "/Contract/CTS010_GetEmailList",
                                "",
                                "tbt_DraftRentalEmail", false);
    SpecialGridControl(gridEmailList, ["Remove"]);

    /* --- Bind Grid events --- */
    /* ------------------------ */
    BindOnLoadedEvent(gridEmailList, function (gen_ctrl) {
        for (var i = 0; i < gridEmailList.getRowsNum(); i++) {
            var row_id = gridEmailList.getRowId(i);

            if (gen_ctrl == true) {
                GenerateRemoveButton(gridEmailList, btnRemoveEmail, row_id, "Remove", true);
            }

            /* --- Set Event --- */
            /* ----------------- */
            BindGridButtonClickEvent(btnRemoveEmail, row_id, function (rid) {
                DeleteEmail(rid);
            });
            /* ----------------- */
        }
        gridEmailList.setSizes();
    });
    /* ------------------------ */

    /* --- Add Events --- */
    /* ------------------ */
    $("#btnAddEmail").click(function () {
        var mails = new Array();
        mails.push({
            EmailAddress: $("#EmailAddress").val()
        });
        AddEmail(mails, true, false);
        return false;
    });
    /* ------------------ */

    /* --- Clear Events --- */
    /* -------------------- */
    $("#btnClearEmail").click(function () {
        /* --- Clear data --- */
        /* ------------------ */
        $("#EmailAddress").val("");
        /* ------------------ */

        /* --- Clear Event --- */
        /* ------------------- */
        VaridateCtrl(["EmailAddress"], null);
        /* ------------------- */

        return false;
    });
    /* -------------------- */

    $("#btnSearchEmail").click(function () {
        SearchEmailAddress();
    });
}
/* ----------------------------------------------------------------------------------- */

/* --- Dialog Methods (CMS060) ------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function SearchEmailAddress() {
    $("#EmailAddress").val("");
    $("#EmailAddress").ResetToNormalControl();

    $("#dlgBox").OpenCMS060Dialog("CTS010");
}
function CMS060Response(doEmailAddress) {
    $("#dlgBox").CloseDialog();
    
    DeleteAllRow(gridEmailList);
    
    if (gridEmailList != undefined && doEmailAddress != undefined) {
        if (doEmailAddress.length == 0)
            return;
        AddEmailFromPopup(doEmailAddress, false, true);
    }
}

function CMS060Object() {
    var objArray = new Array();
    if (CheckFirstRowIsEmpty(gridEmailList) == false) {
        for (var i = 0; i < gridEmailList.getRowsNum(); i++) {
            var rowId = gridEmailList.getRowId(i);
            var selectedRowIndex = gridEmailList.getRowIndex(rowId);
            var mail = gridEmailList.cells2(selectedRowIndex, gridEmailList.getColIndexById('EmailAddress')).getValue();
            var EmpNo = gridEmailList.cells2(selectedRowIndex, gridEmailList.getColIndexById('EmpNo')).getValue();
            var iobj = {
                EmailAddress: mail,
                EmpNo: EmpNo
            };
            objArray.push(iobj);
        }
    }

    return { "EmailList": objArray };
}
/* ----------------------------------------------------------------------------------- */


$(document).ready(function () {
    InitialTrimTextEvent([
        "EmailAddress"
    ]);
});

