
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
/// <reference path="../../Scripts/Common/Dialog.js"/>

/// <reference path="../Base/object/ajax_method.js" />
/// <reference path="../Base/object/command_event.js" />
/// <reference path="../Base/object/master_event.js" />

var gridBillingDetailBLS071;
var doSelectCheckBillingDetail = new Array();

$(document).ready(function () {
    if (objBLS071 != null)
        $("#currency").val(objBLS071.currency);
});

//inintial popup screen
function BLS071Initial(objFromBLS071) {
    ChangeDialogButtonText(
            ["OK", "Cancel"],
            [$("#btnOK").val(),
             $("#btnCancel").val()]);

    BindDialogButtonClick($("#btnOK").val(), function () {
        ConfirmData();
    });

    if (objFromBLS071 != undefined) {

        SetValueInitial(objFromBLS071.doBillingTarget);
        InitialBillingDetailGridData(objFromBLS071);

    }
    else {
        $("#divSelectBilingDetail").clearForm();
    }

}

function SetValueInitial(obj) {

    if (obj != undefined) {
        $("#BillingTargetCode2").val(obj.BillingTargetCode);
        $("#FullNameEN").val(obj.FullNameEN);
        $("#FullNameLC").val(obj.FullNameLC);
    }
}


function InitialBillingDetailGridData(objFromBLS071) {

    gridBillingDetailBLS071 = $("#gridBillingDetail").LoadDataToGridWithInitial(0, false, false, "/Billing/BLS071_GetBillingDetailForCombine",
    "", "BLS071_BillingDetail", false);

    //-------- Get Column CheckBox

    SpecialGridControl(gridBillingDetailBLS071, ["Select"]);
    BindOnLoadedEvent(gridBillingDetailBLS071,
        function () {
            if (CheckFirstRowIsEmpty(gridBillingDetailBLS071, false) == false) {
                for (var i = 0; i < gridBillingDetailBLS071.getRowsNum() ; i++) {

                    var rid = gridBillingDetailBLS071.getRowId(i);
                    var checkboxColInx = gridBillingDetailBLS071.getColIndexById("Select");

                    //----------- Generate CheckBox                   
                    gridBillingDetailBLS071.cells2(i, checkboxColInx).setValue(GenerateCheckBox("ChkBox", rid, "checked", true));

                    /* ===== Bind event onClick to checkbox ===== */
                    BindGridCheckBoxClickEvent("ChkBox", rid, function (rid, checked) {
                        doCheckBoxAction(rid, checked);
                    });



                    var BillingCodeInx = gridBillingDetailBLS071.getColIndexById("BillingCode");
                    var BillingDetailNoInx = gridBillingDetailBLS071.getColIndexById("BillingDetailNo");

                    var BillingCodeVal = gridBillingDetailBLS071.cells2(i, BillingCodeInx).getValue();
                    var BillingDetailNoVal = gridBillingDetailBLS071.cells2(i, BillingDetailNoInx).getValue();

                    // for all apply
                    if (objFromBLS071.doSelectedBillingDetailList != undefined) {
                        for (var j = 0; j < objFromBLS071.doSelectedBillingDetailList.length; j++) {

                            var strBillingCode = objFromBLS071.doSelectedBillingDetailList[j].ContractCode + "-" + objFromBLS071.doSelectedBillingDetailList[j].BillingOCC;

                            if (strBillingCode == BillingCodeVal &&
                                objFromBLS071.doSelectedBillingDetailList[j].BillingDetailNo == BillingDetailNoVal

                            ) {
                                var ckeckboxId = GenerateGridControlID("ChkBox", rid);
                                $("#" + ckeckboxId).attr("checked", true);

                                break;
                            }
                        }

                    }

                    // for disable check box
                    if (objFromBLS071.dtOldBillingDetailList != undefined) {
                        for (var j = 0; j < objFromBLS071.dtOldBillingDetailList.length; j++) {

                            var strBillingCode = objFromBLS071.dtOldBillingDetailList[j].ContractCode + "-" + objFromBLS071.dtOldBillingDetailList[j].BillingOCC;

                            if (strBillingCode == BillingCodeVal &&
                                objFromBLS071.dtOldBillingDetailList[j].BillingDetailNo == BillingDetailNoVal

                            ) {
                                var ckeckboxId = GenerateGridControlID("ChkBox", rid);
                                $("#" + ckeckboxId).attr("disabled", true);

                                break;
                            }
                        }
                    }

                }

                $("#chkHeader").unbind("click");
                $("#chkHeader").click(selectAllCheckboxControl);
                gridBillingDetailBLS071.setSizes();
            }



        });

}

function doCheckBoxAction(rid, checked) {

    //hilight row
    gridBillingDetailBLS071.selectRow(gridBillingDetailBLS071.getRowIndex(rid));
    /*
    var iChecked;
    if (checked) {
    strChecked = "1";
    chkCount++;
    } else {
    strChecked = "0";
    chkCount--;
    }*/
}

function ConfirmData() {

    if (CheckFirstRowIsEmpty(gridBillingDetailBLS071, false) == false) {

        doSelectCheckBillingDetail = new Array();
        var BillingCode = gridBillingDetailBLS071.getColIndexById("BillingCode");
        var BillingDetailNo = gridBillingDetailBLS071.getColIndexById("BillingDetailNo");

        for (var i = 0; i < gridBillingDetailBLS071.getRowsNum(); i++) {
            var rid = gridBillingDetailBLS071.getRowId(i);
            var checkboxColInx = gridBillingDetailBLS071.getColIndexById("Select");
            var SelectChkBox = GenerateGridControlID("ChkBox", rid);
            var val = $("#" + SelectChkBox).prop("checked");

            if (val) {
                var objGrid = {
                    BillingCode: gridBillingDetailBLS071.cells2(i, BillingCode).getValue(),
                    BillingDetailNo: gridBillingDetailBLS071.cells2(i, BillingDetailNo).getValue()
                };
                doSelectCheckBillingDetail.push(objGrid);
            }

        }
    }

    if (doSelectCheckBillingDetail.length > 0) {
        ajax_method.CallScreenController("/Billing/BLS071_ConfirmData", doSelectCheckBillingDetail,
        function (result, controls) {
            if (result != undefined) {
                //if (typeof (BLS071Response) == "function")
                BLS071Response(result);
            }
        });
    }
    else {
        BLS071Response();
    }


}

function selectAllCheckboxControl() {
    var val = $("#chkHeader").prop("checked");
    loopCheckGridControl(val);
}

function loopCheckGridControl(flag) {
    if (CheckFirstRowIsEmpty(gridBillingDetailBLS071, false) == false) {
        for (var i = 0; i < gridBillingDetailBLS071.getRowsNum(); i++) {
            var rid = gridBillingDetailBLS071.getRowId(i);
            var SelectChkBox = GenerateGridControlID("ChkBox", rid);

            if ($("#" + SelectChkBox).prop("disabled") == false) {
                $("#" + SelectChkBox).attr("checked", flag);
            }


        }
    }
}
