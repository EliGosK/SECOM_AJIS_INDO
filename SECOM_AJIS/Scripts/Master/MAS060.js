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
var btnDetailId = "MAS060DetailBtn";
var currentMode;
var detailRid;
var detailObj;
var resultListMode = true;
var isSelected = false;
var mas060_GroupCode = "";
var mas060_GroupName = "";


$(document).ready(function () {

    pageRow = ROWS_PER_PAGE_FOR_VIEWPAGE;

    //Set maxlenght for TextArea control
    $("#Memo").SetMaxLengthTextArea(500);

    /* --- Initial grid --- */
    if ($("#grid_result").length > 0) {
        mygrid = $("#grid_result").InitialGrid(pageRow, false, "/Master/InitialGrid_MAS060");

        /*=========== Set hidden column =============*/
        mygrid.setColumnHidden(mygrid.getColIndexById("DeleteFlag"), true);
        mygrid.setColumnHidden(mygrid.getColIndexById("GroupNameEN"), true);
        mygrid.setColumnHidden(mygrid.getColIndexById("GroupNameLC"), true);
        mygrid.setColumnHidden(mygrid.getColIndexById("GroupOfficeCode"), true);
        mygrid.setColumnHidden(mygrid.getColIndexById("GroupEmpNo"), true);
    }

    /*==== event Group Name keypress ====*/
    //$("#GroupNameEN").keypress(GroupNameEN_keypress);
    //$("#GroupNameLC").keypress(GroupNameLC_keypress);
    $("#Search_GroupName").InitialAutoComplete("/Master/GetGroupName"); //$("#Search_GroupName").keypress(Search_GroupName_keypress);

    /*==== event btnSearch click ====*/
    $("#btnSearch").click(function () {
        doSearchAction();
    });
    /*==== event btnClear click ====*/
    $("#btnClear").click(function () {
        doClearAction();
    });
    /*==== event btnNew click ====*/
    $("#btnNew").click(function () {
        currentMode = MAS060Data.AddMode;
        doNewAction();
    });
    /*==== event btnConfirm click ====*/
    //    $("#btnConfirm").click(function () {
    //        doConfirmAction();
    //    });
    /*==== event btnCancel click ====*/
    //    $("#btnCancel").click(function () {
    //        doCancelAction();
    //    });
    /*==== event btnCancel click ====*/
    $("#GroupEmpNo").keypress(function (e) {
        if (e.which == 0 || e.which == 8 || (e.which >= 48 && e.which <= 57)) {
            return true;
        } else {
            return false;
        }
    });
    /*==== event on lost focus at GroupEmpNo load GroupEmpName ====*/
    $("#GroupEmpNo").blur(function () {
        if ($("#GroupEmpNo").val() != "") {
            loadGroupEmpName($("#GroupEmpNo").val());
        }
        else {
            $("#GroupEmpName").val("");
        }
    });
    /*==== event on click at radio DeleteFlag ====*/
    $("#DeleteFlag").click(function () {
        var checked = $("#DeleteFlag").prop("checked");
        if (checked) {
            setDisableInputCriteria();
            $("#DeleteFlag").removeAttr("disabled"); //uncheck delete flag
        } else {
            if (MAS060Data.HasEditPermission == "True") {
                setEnableInputCriteria();
            }
        }
    });

    initial();

    /* ===== binding event when finish load data ===== */
    SpecialGridControl(mygrid, ["Detail"]);

    mygrid.attachEvent("onBeforeSelect", function (new_row, old_row) {
        if (isSelected == true)
            return false;
        else
            return true;
    });
    mygrid.attachEvent("onBeforeSorting", function (ind, type, direction) {
        if (isSelected == true)
            return false;
        else
            return true;
    });
    mygrid.attachEvent("onBeforePageChanged", function (ind, count) {
        if (isSelected == true)
            return false;
        else
            return true;
    });

    BindOnLoadedEvent(mygrid, function (gen_ctrl) {
        var detailColInx = mygrid.getColIndexById('Detail');

        for (var i = 0; i < mygrid.getRowsNum(); i++) {

            var row_id = mygrid.getRowId(i);

            if (gen_ctrl == true) {
                //mygrid.cells2(i, detailColInx).setValue(GenerateHtmlButton(btnDetailId, row_id, "Detail", true));
                GenerateEditButton(mygrid, btnDetailId, row_id, "Detail", true);
            }

            /* ===== Bind event onClick to button ===== */
            BindGridButtonClickEvent(btnDetailId, row_id, function (rid) {
                currentMode = MAS060Data.EditMode;
                doDetailAction(rid);
            });
        }

        setModeResultListSection(resultListMode);
    });
});

function initial() {

    if (MAS060Data.HasAddPermission == "False") {
        $("#btnNew").attr("disabled", true);
    }
    //    else
    //        $("#btnNew").removeAttr("disabled");

    $("#SearchCustGroup_Section").show();
    $("#ResultList_Section").hide();
    $("#MaintainCustGroup_Section").hide();

    SetConfirmCommand(false, null);
    SetCancelCommand(false, null);
}

//function Search_GroupName_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#Search_GroupName",
//                                cond,
//                                "/Master/GetGroupName",
//                                { "cond": cond },
//                                "doGroupNameDataList",
//                                "GroupName",
//                                "GroupName");
//    }
//}

//function GroupNameEN_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#GroupNameEN",
//                                cond,
//                                "/Master/GetGroupName",
//                                { "cond": cond },
//                                "doGroupNameDataList",
//                                "GroupName",
//                                "GroupName");
//    }
//}

//function GroupNameLC_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#GroupNameLC",
//                                cond,
//                                "/Master/GetGroupName",
//                                { "cond": cond },
//                                "doGroupNameDataList",
//                                "GroupName",
//                                "GroupName");
//    }
//}

function loadGroupEmpName(GroupEmpNo) {
    var parameter = { "GroupEmpNo": GroupEmpNo };
    call_ajax_method(
        '/Master/MAS060_LoadGroupEmpName/',
        parameter,
        function (result, controls) {
            $("#GroupEmpName").val("");
            VaridateCtrl(["GroupEmpNo"], controls);

            if (result != undefined) {
                $("#GroupEmpName").val(result);
            }
        }
    );
}

function doSearchAction() {

    // Disable search button
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    //Load Data to Grid
    var parameter = {
        GroupCode: $("#Search_GroupCode").val(),
        GroupName: $("#Search_GroupName").val()
    }

    mas060_GroupCode = $("#Search_GroupCode").val();
    mas060_GroupName = $("#Search_GroupName").val();


    $("#grid_result").LoadDataToGrid(mygrid, pageRow, false, "/Master/MAS060_Search", parameter, "doGroup", false,
        function () { // Post-load

            // enable search button
            $("#btnSearch").attr("disabled", false);
            master_event.LockWindow(false);

        }, //post-load
        function (result, controls, isWarning) { //pre-load
            if (isWarning == undefined) {
                $("#ResultList_Section").show();
            }
        });
}

function doClearAction() {
    clearSearchCriteria();
    mygrid.clearAll();
    $("#ResultList_Section").hide();
    $("#MaintainCustGroup_Section").hide();
    SetConfirmCommand(false, null);
    SetCancelCommand(false, null);

    CloseWarningDialog();
}

function RemoveClassHighlight() {
    $("#GroupNameEN").ResetToNormalControl();
    $("#GroupNameLC").ResetToNormalControl();
    $("#GroupEmpNo").ResetToNormalControl();
}

function doNewAction() {
    setDisableSearchCustGroup(); //$("#SearchCustGroup_Section").attr("disabled", true);
    setModeResultListSection(false); //$("#ResultList_Section").attr("disabled", true);
    resultListMode = false;
    clearInputCriteria();
    setEnableInputCriteria();
    $("#DeleteFlag").attr("disabled", true);
    $("#MaintainCustGroup_Section").show();
    SetConfirmCommand(true, doConfirmAction);
    SetCancelCommand(true, doCancelAction);
}

function doConfirmAction() {

    // disable confrim button
    DisableConfirmCommand(true);

    var checked = $("#DeleteFlag").prop("checked");
    if (checked) {
        doDeleteAction();
    } else {
        doAddEditAction();
    }

    isSelected = false;
}

function doDeleteAction() {
    var strGroupCode = $("#GroupCode").val();
    var param = { "module": "Master", "code": "MSG1008", "param": strGroupCode };
    call_ajax_method("/Shared/GetMessage", param, function (data) {

        /* ====== Open confirm dialog =====*/
        OpenYesNoMessageDialog(data.Code, data.Message, function () {
            var parameter = {
                GroupCode: $("#GroupCode").val()
            };
            call_ajax_method(
                '/Master/MAS060_Delete',
                parameter,
                function (result, controls) {
                    if (result != undefined) {
                        OpenInformationMessageDialog(result.Code, result.Message, function () {
                            DeleteRow(mygrid, detailRid);
                            setEnableSearchCustGroup(); //$("#SearchCustGroup_Section").removeAttr("disabled");
                            setModeResultListSection(true); //$("#ResultList_Section").removeAttr("disabled");
                            resultListMode = true;
                            $("#MaintainCustGroup_Section").hide();
                            SetConfirmCommand(false, null);
                            SetCancelCommand(false, null);
                        })
                    }
                    else {
                        // enable confrim button
                        DisableConfirmCommand(false);
                    }
                }
            );
        }, function () {
            // enable confrim button
            DisableConfirmCommand(false);
        });

        return false;
    });

    isSelected = false;
}

function doAddEditAction() {
    var officeText = "";
    var officeCodeText = $("#GroupOfficeCode option:selected").text();
    if (officeCodeText != undefined && officeCodeText != "") {
        var officeArr = officeCodeText.split(': ');
        if (officeArr.length > 0)
            officeText = officeArr[1];
    }

    detailObj = {
        GroupCode: $("#GroupCode").val(),
        GroupNameEN: $("#GroupNameEN").val(),
        GroupNameLC: $("#GroupNameLC").val(),
        Memo: $("#Memo").val(),
        GroupOfficeCode: $("#GroupOfficeCode").val(),
        GroupOfficeName: officeText,
        GroupEmpNo: $("#GroupEmpNo").val(),
        GroupEmpName: $("#GroupEmpName").val(),
        DeleteFlag: "False",
        CurrentMode: currentMode
    };

    call_ajax_method_json(
        '/Master/MAS060_AddEdit',
        detailObj,
        function (result, controls) {
            VaridateCtrl(["GroupEmpNo", "GroupNameEN"], controls);
            //, "GroupNameLC"
            
            if (result != undefined) {

                //set return group code 
                detailObj.GroupCode = result[0].GroupCode;

                OpenInformationMessageDialog(result[1].Code, result[1].Message, function () {

                    if (currentMode == MAS060Data.AddMode) {
                        //Add new detail row
                        AddDetailRow();
                    } else if (currentMode == MAS060Data.EditMode) {
                        //Update detail row
                        UpdateDetailRow();
                    }

                    setEnableSearchCustGroup(); //$("#SearchCustGroup_Section").removeAttr("disabled");
                    setModeResultListSection(true); //$("#ResultList_Section").removeAttr("disabled");
                    resultListMode = true;
                    $("#ResultList_Section").show();
                    $("#MaintainCustGroup_Section").hide();
                    SetConfirmCommand(false, null);
                    SetCancelCommand(false, null);


                });
            }
            else {
                // enable confrim button
                DisableConfirmCommand(false);
            }
        }
    );
}

function AddDetailRow() {
    /* --- Check Empty Row --- */
    /* ----------------------- */
    CheckFirstRowIsEmpty(mygrid, true);
    /* ----------------------- */

    /* --- Add new row --- */
    /* ------------------- */
    var GroupNameShow = "(1) " + detailObj.GroupNameEN + "<br/>(2) " + detailObj.GroupNameLC;
    AddNewRow(mygrid,
        [detailObj.GroupCode,
        GroupNameShow,
        detailObj.Memo,
        detailObj.GroupOfficeName,
        detailObj.GroupEmpName,
        "",
        detailObj.DeleteFlag,
        detailObj.GroupNameEN,
        detailObj.GroupNameLC,
        detailObj.GroupOfficeCode,
        detailObj.GroupEmpNo]);
    /* ------------------- */

    var row_idx = mygrid.getRowsNum() - 1;
    var row_id = mygrid.getRowId(row_idx);

    //GenerateHtmlButton(mygrid, btnDetailId, row_id, "Detail", true);
    GenerateEditButton(mygrid, btnDetailId, row_id, "Detail", true);

    mygrid.setSizes();

    /* --- Set Event --- */
    /* ----------------- */
    //BindGridHtmlButtonClickEvent(btnDetailId, row_id, function (rid) {
    BindGridButtonClickEvent(btnDetailId, row_id, function (rid) {
        currentMode = MAS060Data.EditMode;
        doDetailAction(rid);
    });
    /* ----------------- */

    //Set grid to last page
    mygrid.changePage(Math.ceil(mygrid.getRowsNum() / pageRow));
}

function UpdateDetailRow() {
    var GroupNameShow = "(1) " + detailObj.GroupNameEN + "<BR />(2) " + detailObj.GroupNameLC;

    GridControl.UpdateCells(mygrid, detailRid,
        [["GroupCode", detailObj.GroupCode],
            ["GroupNameShow", GroupNameShow],
            ["Memo", detailObj.Memo],
            ["OfficeName", detailObj.GroupOfficeName],
            ["GroupEmpName", detailObj.GroupEmpName],
            ["DeleteFlag", detailObj.DeleteFlag],
            ["GroupNameEN", detailObj.GroupNameEN],
            ["GroupNameLC", detailObj.GroupNameLC],
            ["GroupOfficeCode", detailObj.GroupOfficeCode],
            ["GroupEmpNo", detailObj.GroupEmpNo]]);



    //Shown Data
    //    mygrid.cells2(mygrid.getRowIndex(detailRid), mygrid.getColIndexById('GroupCode')).setValue(detailObj.GroupCode);
    //    
    //    mygrid.cells2(mygrid.getRowIndex(detailRid), mygrid.getColIndexById('GroupNameShow')).setValue(GroupNameShow);
    //    mygrid.cells2(mygrid.getRowIndex(detailRid), mygrid.getColIndexById('Memo')).setValue(detailObj.Memo);
    //    mygrid.cells2(mygrid.getRowIndex(detailRid), mygrid.getColIndexById('OfficeName')).setValue(detailObj.GroupOfficeName);
    //    mygrid.cells2(mygrid.getRowIndex(detailRid), mygrid.getColIndexById('GroupEmpName')).setValue(detailObj.GroupEmpName);

    //    //Hidden Data
    //    mygrid.cells2(mygrid.getRowIndex(detailRid), mygrid.getColIndexById('DeleteFlag')).setValue(detailObj.DeleteFlag);
    //    mygrid.cells2(mygrid.getRowIndex(detailRid), mygrid.getColIndexById('GroupNameEN')).setValue(detailObj.GroupNameEN);
    //    mygrid.cells2(mygrid.getRowIndex(detailRid), mygrid.getColIndexById('GroupNameLC')).setValue(detailObj.GroupNameLC);
    //    mygrid.cells2(mygrid.getRowIndex(detailRid), mygrid.getColIndexById('GroupOfficeCode')).setValue(detailObj.GroupOfficeCode);
    //    mygrid.cells2(mygrid.getRowIndex(detailRid), mygrid.getColIndexById('GroupEmpNo')).setValue(detailObj.GroupEmpNo);
}

function doCancelAction() {

    var param = { "module": "Common", "code": "MSG0140" }; //Cancel
    call_ajax_method("/Shared/GetMessage", param, function (data) {

        /* ====== Open confirm dialog =====*/
        OpenYesNoMessageDialog(data.Code, data.Message, function () {
            clearInputCriteria();
            setDisableInputCriteria();
            setEnableSearchCustGroup(); // $("#SearchCustGroup_Section").removeAttr("disabled");
            setModeResultListSection(true); //$("#ResultList_Section").removeAttr("disabled");
            resultListMode = true;
            $("#MaintainCustGroup_Section").hide();
            SetConfirmCommand(false, null);
            SetCancelCommand(false, null);
        }, null);

        return false;
    });

    isSelected = false;
}

function doDetailAction(rid) {

    //hilight row
    mygrid.selectRow(mygrid.getRowIndex(rid));
    detailRid = rid;
    isSelected = true;

    setDisableSearchCustGroup(); //$("#SearchCustGroup_Section").attr("disabled", true);
    setModeResultListSection(false); //$("#ResultList_Section").attr("disabled", true);
    resultListMode = false;

    //Set Value from Do to Grid
    //    var col = mygrid.getColIndexById('ToJson');
    //    var strJson = mygrid.cells(rid, col).getValue().toString();
    //    var rowObj = JSON.parse(strJson);

    //    if (rowObj.DeleteFlag == "1") {
    //        $("#DeleteFlag").attr("checked", true);
    //    } else {
    //        $("#DeleteFlag").attr("checked", false);
    //    }
    //    $("#GroupCode").val(rowObj.GroupCode);
    //    $("#GroupNameEN").val(rowObj.GroupNameEN);
    //    $("#GroupNameLC").val(rowObj.GroupNameLC);
    //    $("#Memo").val(rowObj.Memo);
    //    $("#GroupOfficeCode").val(rowObj.GroupOfficeCode);
    //    $("#GroupEmpNo").val(rowObj.GroupEmpNo);
    //    $("#GroupEmpName").val(rowObj.GroupEmpName);

    //--Set Value from Grid to Maintain section ----------------------------------------
    clearInputCriteria();


    var parameter = {
        GroupCode: htmlDecode(mygrid.cells2(mygrid.getRowIndex(detailRid), mygrid.getColIndexById('GroupCode')).getValue())
    }

    call_ajax_method("/Master/MAS060_SearchByGroupCode", parameter, function (data) {

        //tt

        var vDeleteFlag = "";
        var vGroupCode = "";
        var vGroupNameEN = "";
        var vGroupNameLC = "";
        var vMemo = "";
        var vGroupOfficeCode = "";
        var vGroupEmpNo = "";
        var vGroupEmpName = "";

        if (data.length > 0) {
            vDeleteFlag = data[0].DeleteFlag;
            vGroupCode = data[0].GroupCode;
            vGroupNameEN = data[0].GroupNameEN;
            vGroupNameLC = data[0].GroupNameLC;
            vMemo = data[0].Memo;
            vGroupOfficeCode = data[0].GroupOfficeCode;
            vGroupEmpNo = data[0].GroupEmpNo;
            vGroupEmpName = data[0].GroupEmpName;
        }

        if (vDeleteFlag == true) {
            $("#DeleteFlag").attr("checked", true);
        } else {
            $("#DeleteFlag").attr("checked", false);
        }
        $("#GroupCode").val(vGroupCode);
        $("#GroupNameEN").val(vGroupNameEN);
        $("#GroupNameLC").val(vGroupNameLC);
        $("#Memo").val(vMemo);
        $("#GroupOfficeCode").val(vGroupOfficeCode);
        $("#GroupEmpNo").val(vGroupEmpNo);
        $("#GroupEmpName").val(vGroupEmpName);

        //--Render Enable/Disable ----------------------------------------
        setDisableInputCriteria(); //set default to disabled all
        SetConfirmCommand(false, null); // $("#btnConfirm").attr("disabled", true);

        var checked = $("#DeleteFlag").prop("checked");
        if (checked) {
            if (MAS060Data.HasDeletePermission == "True") {
                $("#DeleteFlag").removeAttr("disabled");
                SetConfirmCommand(true, doConfirmAction); //$("#btnConfirm").removeAttr("disabled");
            }
        } else {
            if (MAS060Data.HasEditPermission == "True") {
                setEnableInputCriteria();
                SetConfirmCommand(true, doConfirmAction); //$("#btnConfirm").removeAttr("disabled");
            }

            if (MAS060Data.HasDeletePermission == "True") {
                $("#DeleteFlag").removeAttr("disabled");
                SetConfirmCommand(true, doConfirmAction); //$("#btnConfirm").removeAttr("disabled");
            }
        }
        $("#MaintainCustGroup_Section").show();
        $("#GroupNameEN").focus();

        SetCancelCommand(true, doCancelAction);

        // tt

    });


}

function clearSearchCriteria() {
    $("#Search_GroupCode").val("");
    $("#Search_GroupName").val("");
}

function clearInputCriteria() {
    $("#DeleteFlag").attr("checked", false);
    $("#GroupCode").val("");
    $("#GroupNameEN").val("");
    $("#GroupNameLC").val("");
    $("#Memo").val("");
    $("#GroupOfficeCode").val("");
    $("#GroupEmpNo").val("");
    $("#GroupEmpName").val("");

    RemoveClassHighlight();
}

function setEnableInputCriteria() {
    if (MAS060Data.HasDeletePermission == "True") {
        $("#DeleteFlag").removeAttr("disabled");
    }
    $("#GroupNameEN").attr("readonly", false);
    $("#GroupNameLC").attr("readonly", false);
    $("#Memo").attr("readonly", false);
    $("#GroupOfficeCode").removeAttr("disabled");
    $("#GroupEmpNo").attr("readonly", false);
}

function setDisableInputCriteria() {
    $("#DeleteFlag").attr("disabled", true);
    $("#GroupNameEN").attr("readonly", true);
    $("#GroupNameLC").attr("readonly", true);
    $("#Memo").attr("readonly", true);
    $("#GroupOfficeCode").attr("disabled", true);
    $("#GroupEmpNo").attr("readonly", true);
}

function setEnableSearchCustGroup() {
    $("#Search_GroupCode").attr("readonly", false);
    $("#Search_GroupName").attr("readonly", false);
    $("#btnSearch").removeAttr("disabled");
    $("#btnClear").removeAttr("disabled");
    if (MAS060Data.HasAddPermission == "True") {
        $("#btnNew").removeAttr("disabled");
    }
}

function setDisableSearchCustGroup() {
    $("#Search_GroupCode").attr("readonly", true);
    $("#Search_GroupName").attr("readonly", true);
    $("#btnSearch").attr("disabled", true);
    $("#btnClear").attr("disabled", true);
    $("#btnNew").attr("disabled", true);
}

function setModeResultListSection(enable) {

    //$("#grid_result").find("img").each(function () {
    //    if (this.id != "" && this.id != undefined) {
    //        if (this.id.indexOf(btnDetailId) == 0) {
    //            var row_id = GetGridRowIDFromControl(this);
    //            EnableGridButton(mygrid, btnDetailId, row_id, "Detail", enable);
    //        }
    //    }
    //});

    for (var i = 0; i < mygrid.getRowsNum(); i++) {

        var row_id = mygrid.getRowId(i);
        EnableGridButton(mygrid, btnDetailId, row_id, "Detail", enable);
    }

    mygrid.attachEvent("onAfterSorting", function (index, type, direction) {
        for (var i = 0; i < mygrid.getRowsNum(); i++) {

            var row_id = mygrid.getRowId(i);
            EnableGridButton(mygrid, btnDetailId, row_id, "Detail", enable);

        }
    });
}

