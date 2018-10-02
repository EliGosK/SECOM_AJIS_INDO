/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>

var CTS270_Script = {
    btnViewId: "CTS270ViewBtn",
    btnRegisterId: "CTS270RegisterBtn",
    btnDeleteId: "CTS270DeleteBtn",
    chkDownload: "CTS270DownloadChk",

    ResultGrid: null,
    PageRow: CTS270Data.PageRow,

    Initial: function () {
        //Hide "Result list" section
        $("#Search_Result").hide();
        $("#Search_Button").hide();

        if (CTS270Data.HasSlipRdoPermission == "False") {
            $("#MaintChkSlip").attr("disabled", true);
        } else {
            $("#MaintChkSlip").removeAttr("disabled");
        }

        if (CTS270Data.HasListRdoPermission == "False") {
            $("#MaintChkList").attr("disabled", true);
        } else {
            $("#MaintChkList").removeAttr("disabled");
        }
    },
    InitialControls: function () {
        /// <summary>Initial control event</summary>
        InitialTrimTextEvent([
            "ProductName",
            "SiteName",
            "UserCodeContractCode",
            "MAEmployeeName",
            "MACheckupNo"
        ]);

        if ($("#grid_result").length > 0) {
            CTS270_Script.ResultGrid = $("#grid_result").InitialGrid(
                CTS270_Script.PageRow, true,
                "/Contract/InitialGrid_CTS270");

            var funcDrawControl = function (rid) {
                var actionColInx = CTS270_Script.ResultGrid.getColIndexById("Actions");
                var checkboxColInx = CTS270_Script.ResultGrid.getColIndexById("Checkbox");
                var chkboxFlagColInx = CTS270_Script.ResultGrid.getColIndexById("EnableCheckboxFlag");

                var iChkbox = parseInt(CTS270_Script.ResultGrid.cells(rid, chkboxFlagColInx).getValue());

                if (CTS270_Script.ResultGrid.cells(rid, actionColInx).getValue() == "") { //Generate control if is empty.
                    var viewFlagColInx = CTS270_Script.ResultGrid.getColIndexById("EnableViewFlag");
                    var registerFlagColInx = CTS270_Script.ResultGrid.getColIndexById("EnableRegisterFlag");
                    var deleteFlagColInx = CTS270_Script.ResultGrid.getColIndexById("EnableDeleteFlag");

                    var iView = parseInt(CTS270_Script.ResultGrid.cells(rid, viewFlagColInx).getValue());
                    var iRegister = parseInt(CTS270_Script.ResultGrid.cells(rid, registerFlagColInx).getValue());
                    var iDelete = parseInt(CTS270_Script.ResultGrid.cells(rid, deleteFlagColInx).getValue());

                    var RenderButton = "";
                    RenderButton = RenderButton.concat(GenerateHtmlButton(CTS270_Script.btnViewId, rid, CTS270Data.ViewLabel, Boolean(iView)) + "<br>");
                    RenderButton = RenderButton.concat(GenerateHtmlButton(CTS270_Script.btnRegisterId, rid, CTS270Data.RegisterLabel, Boolean(iRegister)) + "<br>");
                    RenderButton = RenderButton.concat(GenerateHtmlButton(CTS270_Script.btnDeleteId, rid, CTS270Data.DeleteLabel, Boolean(iDelete)) + "<br>");

                    CTS270_Script.ResultGrid.cells(rid, actionColInx).setValue(RenderButton);
                    CTS270_Script.ResultGrid.cells(rid, checkboxColInx).setValue(GenerateCheckBox(CTS270_Script.chkDownload, rid, "checked", Boolean(iChkbox)));

                    BindGridHtmlButtonClickEvent(CTS270_Script.btnViewId, rid, function (eRid) {
                        CTS270_Script.DoViewAction(eRid);
                    });
                    BindGridHtmlButtonClickEvent(CTS270_Script.btnRegisterId, rid, function (eRid) {
                        CTS270_Script.DoRegisterAction(eRid);
                    });
                    BindGridHtmlButtonClickEvent(CTS270_Script.btnDeleteId, rid, function (eRid) {
                        CTS270_Script.DoDeleteAction(eRid);
                    });
                    BindGridCheckBoxClickEvent(CTS270_Script.chkDownload, rid, function (eRid, checked) {
                        CTS270_Script.ResultGrid.selectRow(CTS270_Script.ResultGrid.getRowIndex(eRid));
                        CTS270_Script.DoCheckBoxAction(eRid, checked);
                    });
                }
                if (Boolean(iChkbox) == true) {
                    var ctrl = $("#" + GenerateGridControlID(CTS270_Script.chkDownload, rid));
                    if (ctrl.length > 0) {
                        var flagColInx = CTS270_Script.ResultGrid.getColIndexById("CheckedFlag");
                        var checkedFlag = CTS270_Script.ResultGrid.cells(rid, flagColInx).getValue();
                        if (checkedFlag == "1") {
                            ctrl.attr("checked", true);
                        }
                        else {
                            ctrl.removeAttr("checked");
                        }
                    }
                }
            };

            SpecialGridControl(CTS270_Script.ResultGrid, ["Checkbox", "Actions"]);
            BindOnLoadedEventV2(CTS270_Script.ResultGrid, CTS270_Script.PageRow, true, true, funcDrawControl);
        }

        //Autocomplete
        $("#ProductName").InitialAutoComplete("/Master/GetProductName");
        $("#MAEmployeeName").InitialAutoComplete("/Master/MAS070_GetEmployeeName");
        $("#SiteName").InitialAutoComplete("/Master/GetSiteName");

        $("#btnSearch").click(CTS270_Script.SearchClick);
        $("#btnClear").click(CTS270_Script.ClearSearchClick);
        $("#btnSelectAll").click(CTS270_Script.SelectAllClick);
        $("#btnUnselectAll").click(CTS270_Script.UnSelectAllClick);
        $("#btnDownload").click(CTS270_Script.DoDownloadAction);

        CTS270_Script.Initial();

        if (CTS270Data.HasSessionData == "True") {
            CTS270_Script.DoSearchData();
        }
    },

    SearchClick: function () {
        /// <summary>Button search click event.</summary>

        CTS270Data.CurrentIndex = null;
        CTS270Data.CurrentSortColIndex = null;
        CTS270Data.CurrentSortType = null;
        master_event.LockWindow(true);

        var parameter = CreateObjectData($("#CTS270_Search").serialize());
        ajax_method.CallScreenController(
            "/Contract/CTS270_CheckReqField",
            parameter,
            function (result, controls) {
                master_event.LockWindow(false);

                if (controls != undefined) {
                    VaridateCtrl([
                        "CheckupInstructionMonthFrom",
                        "CheckupInstructionYearFrom",
                        "CheckupInstructionMonthTo",
                        "CheckupInstructionYearTo"], controls);
                    $("#CheckupInstructionMonthFrom").focus();
                    return;
                } else {
                    CTS270_Script.DoSearchData();
                }
            }
        );
    },
    DoSearchData: function () {
        /// <summary>Initial control event</summary>
        $("#Search_Criteria").ResetToNormalControl();

        //Load Data to Grid
        var obj = {
            RelatedContractType: $("#RelatedContractType").val(),
            OperationOffice: $("#OperationOffice").val(),
            ProductName: $("#ProductName").val(),
            CheckupInstructionMonthFrom: $("#CheckupInstructionMonthFrom").val(),
            CheckupInstructionYearFrom: $("#CheckupInstructionYearFrom").val(),
            CheckupInstructionMonthTo: $("#CheckupInstructionMonthTo").val(),
            CheckupInstructionYearTo: $("#CheckupInstructionYearTo").val(),
            SiteName: $("#SiteName").val(),
            UserCodeContractCode: $("#UserCodeContractCode").val(),
            MAEmployeeName: $("#MAEmployeeName").val(),
            MACheckupNo: $("#MACheckupNo").val(),
            HasCheckupResult: $("#HasCheckupResult").prop("checked"),
            HaveInstrumentMalfunction: $("#HaveInstrumentMalfunction").prop("checked"),
            NeedToContactSalesman: $("#NeedToContactSalesman").prop("checked"),
            MaintenanceCheckupSlipFlag: $("#MaintChkSlip").prop("checked"),
            MaintenanceCheckupListFlag: $("#MaintChkList").prop("checked")
        };

        $("#btnSearch").attr("disabled", true);
        master_event.LockWindow(true);

        $("#grid_result").LoadDataToGrid(CTS270_Script.ResultGrid, CTS270_Script.PageRow, true, "/Contract/CTS270_Search", obj, "dtSearchMACheckupResult", false,
            function (result, controls, isWarning) { // post-load
                $("#btnSearch").removeAttr("disabled");
                master_event.LockWindow(false);

                if (result != undefined) {
                    if (CTS270Data.HasSessionData == "True") {
                        CTS270_Script.ResultGrid.selectRow(CTS270Data.CurrentIndex);

                        if (CTS270Data.CurrentSortColIndex != undefined
                            && CTS270Data.CurrentSortColIndex >= 0) {
                            CTS270_Script.ResultGrid.setSortImgState(true, CTS270Data.CurrentSortColIndex, CTS270Data.CurrentSortType);

                            CurrentSortColIndex = CTS270Data.CurrentSortColIndex;
                            CurrentSortType = CTS270Data.CurrentSortType;
                        }

                        master_event.ScrollWindow("tr[idd='" + CTS270_Script.ResultGrid.getRowId(CTS270Data.CurrentIndex) + "']", false, true);
                    }
                    else {
                        master_event.ScrollWindow("#Search_Result");
                    }

                    var bChkListFlag = $("#MaintChkList").prop("checked");
                    var isEmpty = CheckFirstRowIsEmpty(CTS270_Script.ResultGrid);
                    if (bChkListFlag || isEmpty == true) {
                        $("#btnSelectAll").attr("disabled", true);
                        $("#btnUnselectAll").attr("disabled", true);

                        if (isEmpty == true) {
                            $("#btnDownload").attr("disabled", true);
                        }
                        else {
                            $("#btnDownload").removeAttr("disabled");
                        }
                    } else {
                        $("#btnSelectAll").removeAttr("disabled");
                        $("#btnUnselectAll").removeAttr("disabled");
                        $("#btnDownload").removeAttr("disabled");
                    }
                }
            },
            function (result, controls, isWarning) { //pre-load
                if (isWarning == undefined) {
                    $("#Search_Result").show();
                    $("#Search_Button").show();
                }
            }
        );
    },
    ClearSearchClick: function () {
        $("#Search_Criteria").ResetToNormalControl();

        //Set default checkbox
        $("#MaintChkSlip").attr("checked", false);
        $("#MaintChkList").attr("checked", false);
        if (CTS270Data.HasSlipRdoPermission == "True")
            $("#MaintChkSlip").attr("checked", true);
        if (CTS270Data.HasSlipRdoPermission == "False" && CTS270Data.HasListRdoPermission == "True")
            $("#MaintChkList").attr("checked", true);

        //Set default month/date
        var dt = new Date();
        $("#CheckupInstructionMonthFrom").val(dt.getMonth() + 1);
        $("#CheckupInstructionYearFrom").val(dt.getFullYear());
        $("#CheckupInstructionMonthTo").val(dt.getMonth() + 1);
        $("#CheckupInstructionYearTo").val(dt.getFullYear());

        //Set default to other control
        $("#RelatedContractType").val(CTS270Data.RelatedContractType);
        $("#OperationOffice").val("");
        $("#ProductName").val("");
        $("#SiteName").val("");
        $("#UserCodeContractCode").val("");
        $("#MAEmployeeName").val("");
        $("#RelatedContractType").val("");
        $("#MACheckupNo").val("");
        $("#HasCheckupResult").attr("checked", false);
        $("#HaveInstrumentMalfunction").attr("checked", false);
        $("#NeedToContactSalesman").attr("checked", false);

        CloseWarningDialog();
        CTS270_Script.Initial();
    },

    DoCheckBoxAction: function (rid, checked) {
        /// <summary>Checkbox event.</summary>
        /// <param name="rid" type="int">Row index</param>
        /// <param name="checked" type="bool">Checked</param>

        var flag = "";
        if (checked) {
            flag = "1";
        }

        var checkColIdx = CTS270_Script.ResultGrid.getColIndexById("CheckedFlag");
        CTS270_Script.ResultGrid.cells(rid, checkColIdx).setValue(flag);
    },
    DoViewAction: function (rid) {
        //hilight row
        CTS270_Script.ResultGrid.selectRow(CTS270_Script.ResultGrid.getRowIndex(rid));

        var lst = new Array();
        for (var i = 0; i < CTS270_Script.ResultGrid.getRowsNum(); i++) {
            var iobj = {
                ContractCode: CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('ContractCode')).getValue().toString(),
                ProductCode: CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('ProductCode')).getValue().toString(),
                InstructionDate: CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('InstructionDate')).getValue(),
                KeyIndex: CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('KeyIndex')).getValue(),
                CheckedFlag: CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('CheckedFlag')).getValue() == "1"
            };
            lst.push(iobj);
        }

        var param = {
            Mode: CTS270Data.FunctionIdView,
            CurrentIndex: CTS270_Script.ResultGrid.getRowIndex(rid),
            list: lst,
            CurrentSortColIndex: CurrentSortColIndex != undefined ? CurrentSortColIndex : -1,
            CurrentSortType: CurrentSortType != undefined ? CurrentSortType : "ASC"
        };
        ajax_method.CallScreenController(
            "/Contract/CTS270_SetBtnClickFlag",
            param,
            function (result, controls) {
                if (result != undefined) {
                    var obj = {
                        ContractCodeShow: CTS270_Script.ResultGrid.cells(rid, CTS270_Script.ResultGrid.getColIndexById('ContractCodeShow')).getValue().toString(),
                        ProductCode: CTS270_Script.ResultGrid.cells(rid, CTS270_Script.ResultGrid.getColIndexById('ProductCode')).getValue().toString(),
                        InstructionDate: CTS270_Script.ResultGrid.cells(rid, CTS270_Script.ResultGrid.getColIndexById('InstructionDate')).getValue(),
                        Mode: CTS270Data.FunctionIdView,
                        CallerSessionKey: ajax_method.GetKeyURL(null)
                    };
                    ajax_method.CallScreenControllerWithAuthority("/Contract/CTS280", obj);
                }
            }
        );
    },
    DoRegisterAction: function (rid) {
        //hilight row
        CTS270_Script.ResultGrid.selectRow(CTS270_Script.ResultGrid.getRowIndex(rid));
        master_event.LockWindow(true);

        var lst = new Array();
        for (var i = 0; i < CTS270_Script.ResultGrid.getRowsNum(); i++) {
            var iobj = {
                ContractCode: CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('ContractCode')).getValue().toString(),
                ProductCode: CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('ProductCode')).getValue().toString(),
                InstructionDate: CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('InstructionDate')).getValue(),
                KeyIndex: CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('KeyIndex')).getValue(),
                CheckedFlag: CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('CheckedFlag')).getValue() == "1"
            };
            lst.push(iobj);
        }

        var param = {
            Mode: CTS270Data.FunctionIdAdd,
            CurrentIndex: CTS270_Script.ResultGrid.getRowIndex(rid),
            list: lst,
            CurrentSortColIndex: CurrentSortColIndex != undefined ? CurrentSortColIndex : -1,
            CurrentSortType: CurrentSortType != undefined ? CurrentSortType : "ASC"
        };

        ajax_method.CallScreenController(
            '/Contract/CTS270_SetBtnClickFlag',
            param,
            function (result, controls) {
                if (result != undefined) {
                    var obj = {
                        ContractCodeShow: CTS270_Script.ResultGrid.cells(rid, CTS270_Script.ResultGrid.getColIndexById('ContractCodeShow')).getValue().toString(),
                        ProductCode: CTS270_Script.ResultGrid.cells(rid, CTS270_Script.ResultGrid.getColIndexById('ProductCode')).getValue().toString(),
                        InstructionDate: CTS270_Script.ResultGrid.cells(rid, CTS270_Script.ResultGrid.getColIndexById('InstructionDate')).getValue(),
                        Mode: CTS270Data.FunctionIdAdd,
                        CallerSessionKey: ajax_method.GetKeyURL(null)
                    };
                    ajax_method.CallScreenControllerWithAuthority("/Contract/CTS280", obj);
                }
            }
        );
    },
    DoDeleteAction: function (rid) {
        var index = CTS270_Script.ResultGrid.getRowIndex(rid);

        //hilight row
        CTS270_Script.ResultGrid.selectRow(CTS270_Script.ResultGrid.getRowIndex(rid));
        master_event.LockWindow(true);

        var param = {
            "module": "Contract",
            "code": "MSG3097",
            "param[0]": CTS270_Script.ResultGrid.cells(rid, CTS270_Script.ResultGrid.getColIndexById('CheckupNo')).getValue().toString(),
            "param[1]": CTS270_Script.ResultGrid.cells(rid, CTS270_Script.ResultGrid.getColIndexById('ContractCodeShow')).getValue().toString()
        };

        ajax_method.CallScreenController("/Shared/GetMessage", param,
            function (data) {
                master_event.LockWindow(false);

                OpenYesNoMessageDialog(data.Code, data.Message,
                    function () {
                        master_event.LockWindow(true);

                        var lst = new Array();
                        for (var i = 0; i < CTS270_Script.ResultGrid.getRowsNum(); i++) {
                            var iobj = {
                                ContractCode: CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('ContractCode')).getValue().toString(),
                                ProductCode: CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('ProductCode')).getValue().toString(),
                                InstructionDate: CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('InstructionDate')).getValue(),
                                KeyIndex: CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('KeyIndex')).getValue(),
                                CheckedFlag: CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('CheckedFlag')).getValue() == "1"
                            };
                            lst.push(iobj);
                        }

                        var deleteObj = {
                            DeleteItem: {
                                ContractCode: CTS270_Script.ResultGrid.cells(rid, CTS270_Script.ResultGrid.getColIndexById('ContractCode')).getValue().toString(),
                                ProductCode: CTS270_Script.ResultGrid.cells(rid, CTS270_Script.ResultGrid.getColIndexById('ProductCode')).getValue().toString(),
                                InstructionDate: CTS270_Script.ResultGrid.cells(rid, CTS270_Script.ResultGrid.getColIndexById('InstructionDate')).getValue()
                            },
                            CheckItemList: lst
                        };

                        ajax_method.CallScreenController(
                            "/Contract/CTS270_Delete/",
                            deleteObj,
                            function (result, controls) {
                                master_event.LockWindow(false);

                                if (result != undefined) {
                                    OpenInformationMessageDialog(result.Code, result.Message, function () {
                                        master_event.LockWindow(true);

                                        $("#grid_result").LoadDataToGrid(CTS270_Script.ResultGrid, CTS270_Script.PageRow, true,
                                                "/Contract/CTS270_GetResultList", "", "dtSearchMACheckupResult", false,

                                                function (result, controls, isWarning) { // post-load
                                                    master_event.LockWindow(false);

                                                    if (CTS270_Script.ResultGrid.getRowsNum() <= index) {
                                                        index = CTS270_Script.ResultGrid.getRowsNum() - 1;
                                                    }

                                                    if (CheckFirstRowIsEmpty(CTS270_Script.ResultGrid) == false) {
                                                        CTS270_Script.ResultGrid.selectRow(index);
                                                        if (CurrentSortColIndex != undefined) {
                                                            CTS270_Script.ResultGrid.setSortImgState(true, CurrentSortColIndex, CurrentSortType);
                                                        }

                                                        master_event.ScrollWindow("tr[idd='" + CTS270_Script.ResultGrid.getRowId(index) + "']", false, true);
                                                    }
                                                    else {
                                                        $("#btnSelectAll").attr("disabled", true);
                                                        $("#btnUnselectAll").attr("disabled", true);
                                                        $("#btnDownload").attr("disabled", true);
                                                    }
                                                },
                                                null);
                                    });
                                }
                            }
                        );
                    }
                );
            }
        );
    },
    DoDownloadAction: function () {
        master_event.LockWindow(true);

        var index = CTS270_Script.ResultGrid.getRowIndex(CTS270_Script.ResultGrid.getSelectedId());
        var bChkListFlag = $("#MaintChkList").prop("checked");

        var lst = new Array();
        if (bChkListFlag == false) {
            for (var i = 0; i < CTS270_Script.ResultGrid.getRowsNum(); i++) {
                var obj = {
                    ContractCode: CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('ContractCode')).getValue().toString(),
                    ProductCode: CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('ProductCode')).getValue().toString(),
                    InstructionDate: CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('InstructionDate')).getValue(),
                    KeyIndex: CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('KeyIndex')).getValue(),
                    CheckedFlag: (CTS270_Script.ResultGrid.cells2(i, CTS270_Script.ResultGrid.getColIndexById('CheckedFlag')).getValue() == "1") || bChkListFlag
                };
                lst.push(obj);
            }
        }

        var obj = {
            "bChkListFlag": bChkListFlag,
            "list": lst
        };
        ajax_method.CallScreenController("/Contract/CTS270_Download", obj, function (result, controls) {
            var nFunc = function () {
                var url = "/Contract/CTS270_DownloadSubmit";
                download_method.CallDownloadController("ifDownload", url, null);

                if (bChkListFlag == false) {
                    $("#grid_result").LoadDataToGrid(CTS270_Script.ResultGrid, CTS270_Script.PageRow, true,
                        "/Contract/CTS270_GetResultList", "", "dtSearchMACheckupResult", false,
                        function (result, controls, isWarning) { // post-load
                            master_event.LockWindow(false);
                            CTS270_Script.ResultGrid.selectRow(index);
                            if (CurrentSortColIndex != undefined) {
                                CTS270_Script.ResultGrid.setSortImgState(true, CurrentSortColIndex, CurrentSortType);
                            }

                            master_event.ScrollWindow("tr[idd='" + CTS270_Script.ResultGrid.getRowId(index) + "']", false, true);
                        },
                        null);
                }
                else {
                    master_event.LockWindow(false);
                }
            }

            if (result != undefined) {
                if (result == true) {
                    nFunc();
                }
                else {
                    master_event.LockWindow(false);
                    DetailMessageDialog.Open(result[0].Code, result[0].Message, result[1], function () {
                        master_event.LockWindow(true);
                        nFunc();
                    });
                }
            }
        }, false);
    },


    SelectAllClick: function () {
        CTS270_Script.DoSelectAllData(true);
    },
    UnSelectAllClick: function () {
        CTS270_Script.DoSelectAllData(false);
    },
    DoSelectAllData: function (selected) {
        var ind = CTS270_Script.ResultGrid.currentPage;
        var fInd = (ind - 1) * CTS270_Script.PageRow;

        for (var i = 0; i < CTS270_Script.ResultGrid.getRowsNum(); i++) {
            var rid = CTS270_Script.ResultGrid.getRowId(i);

            var chkboxFlagColInx = CTS270_Script.ResultGrid.getColIndexById('EnableCheckboxFlag');
            var enableFlag = CTS270_Script.ResultGrid.cells(rid, chkboxFlagColInx).getValue();
            if ('1' == enableFlag) {
                CTS270_Script.DoCheckBoxAction(rid, selected);

                if (i >= fInd && i <= (fInd + CTS270_Script.PageRow)) {
                    var ctrl = $("#" + GenerateGridControlID(CTS270_Script.chkDownload, rid));
                    if (ctrl.length > 0) {
                        if (selected == true) {
                            ctrl.attr("checked", true);
                        }
                        else {
                            ctrl.removeAttr("checked");
                        }
                    }
                }
            }
        }
    }

}

$(document).ready(function () {
    CTS270_Script.InitialControls();

    
});


function cts270_ma_ext_custom(a, b, order) {
    var lst = [a, b];
    var date = [null, null];
    for (var idx = 0; idx < lst.length; idx++) {
        var isNull = true;

        if (lst[idx] != undefined && lst[idx] != "") {
            var st = lst[idx].indexOf("(1) ") + 4;
            var fn = lst[idx].indexOf("<BR>");
            var l1 = lst[idx].substring(st, fn)
            if (l1 != "-") {
                var dd = l1.substring(0, 2);
                var mm = l1.substring(3, 6);
                var yy = l1.substring(7, 11);
                date[idx] = new Date(yy + "/" + mm + "/" + dd);

                isNull = false;
            }
        }

        if (isNull == true) {
            date[idx] = new Date("1000/01/01");
        }
    }

    if (order == "asc")
        return (date[0] > date[1] ? 1 : -1);
    else
        return (date[0] > date[1] ? -1 : 1);
}


var DetailMessageDialog = {
    IsShow: false,

    Open: function (code, msg, msg_detail, func) {
        $("#msg_error_detail_dialog").dialog({
            modal: true,
            resizable: false,
            title: code,
            width: 400,
            height: 250,
            buttons: {
                Detail: function () {
                    if (DetailMessageDialog.IsShow == false) {
                        $("#msg_detail_info").show();
                        $("#msg_error_detail_dialog").dialog("option", "height", 380);
                        DetailMessageDialog.IsShow = true;

                        $("#msg_error_detail_dialog").parent().children(":nth-child(3)").children().children("button").each(function () {
                            var txtD = $("#btnDetail").val();
                            if ($(this).text().indexOf(txtD) >= 0) {
                                $(this).text(txtD + " <<");
                            }
                        });
                    }
                    else {
                        $("#msg_detail_info").hide();
                        $("#msg_error_detail_dialog").dialog("option", "height", 250);
                        DetailMessageDialog.IsShow = false;

                        $("#msg_error_detail_dialog").parent().children(":nth-child(3)").children().children("button").each(function () {
                            var txtD = $("#btnDetail").val();
                            if ($(this).text().indexOf(txtD) >= 0) {
                                $(this).text(txtD + " >>");
                            }
                        });
                    }
                },
                OK: function () {
                    DetailMessageDialog.Close();

                    $("#msg_detail_info").hide();
                    DetailMessageDialog.IsShow = false;
                    $("#msg_error_detail_dialog").parent().children(":nth-child(3)").children().children("button").each(function () {
                        var txtD = $("#btnDetail").val();
                        if ($(this).text().indexOf(txtD) >= 0) {
                            $(this).text(txtD + " >>");
                        }
                    });

                    func();
                }
            }
        });

        var img = "<img src=\"../../Content/images/dialog/error.png\" />";
        $("#msg_detail_img").html(img);
        $("#msg_detail_msg").html(ConvertSSH(msg));
        $("#msg_detail_info").html(ConvertSSH(msg_detail));

        $("#msg_error_detail_dialog").parent().children(":nth-child(1)").children("a").css("display", "none");
        $("#msg_error_detail_dialog").SetDialogStyle();

        var ctrl_name = ["OK", "Detail"];
        var new_ctrl_name = [
        $("#DialogBtnOK").val(),
        $("#btnDetail").val()];

        $("#msg_error_detail_dialog").parent().children(":nth-child(3)").children().children("button").each(function () {
            for (var idx = 0; idx < ctrl_name.length; idx++) {
                if ($(this).text() == ctrl_name[idx]) {
                    $(this).text(new_ctrl_name[idx])
                }
            }

            var txtD = $("#btnDetail").val();
            if ($(this).text().indexOf(txtD) >= 0) {
                $(this).text(txtD + " >>");
                $(this).css("margin-right", "165px");
            }
        });
    },
    Close: function () {
        $("#msg_error_detail_dialog").dialog("close");
        $("#msg_error_detail_dialog").dialog("destroy");
    }
}