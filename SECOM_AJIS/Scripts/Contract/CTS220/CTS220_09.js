$(document).ready(function () {
    InitialControlPropertyCTS220_9();
    BindDOInstallationInformation()
    GetInstallation_CTS220("Load");
    //MaintainScreenItem();
});

function InitialControlPropertyCTS220_9() {
//    $("#InstallationCompleteDate").InitialDate();
    $("#btnAdd").click(function () { AddClick_CTS220() });
    $("#InstallationSlipNo").click(function () { OpenCMS180(); });

    $("#NormalInstallFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#OrderInstallFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#OrderInstallFee_ApproveContract").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#OrderInstallFee_CompleteInstall").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#OrderInstallFee_StartService").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#InstallFeePaidBySECOM").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#InstallFeeRevenueBySECOM").BindNumericBox(12, 2, 0, 999999999999.99);
}

function BindDOInstallationInformation() {
    call_ajax_method_json('/Contract/BindDOInstallationInformation_CTS220', "",
        function (result, controls) {
            $("#InstallationTypeCode").val(result.InstallationTypeCode);

            result.InstallationCompleteDate = ConvertDateObject(result.InstallationCompleteDate);
            $("#InstallationCompleteDate").val(ConvertDateToTextFormat(result.InstallationCompleteDate));

            $("#NormalInstallFee").SetNumericCurrency(result.NormalInstallFeeCurrencyType);
            $("#NormalInstallFee").val(result.NormalInstallFee);
            $("#OrderInstallFee").SetNumericCurrency(result.OrderInstallFeeCurrencyType);
            $("#OrderInstallFee").val(result.OrderInstallFee);
            $("#OrderInstallFee_ApproveContract").SetNumericCurrency(result.OrderInstallFee_ApproveContractCurrencyType);
            $("#OrderInstallFee_ApproveContract").val(result.OrderInstallFee_ApproveContract);
            $("#OrderInstallFee_CompleteInstall").SetNumericCurrency(result.OrderInstallFee_CompleteInstallCurrencyType);
            $("#OrderInstallFee_CompleteInstall").val(result.OrderInstallFee_CompleteInstall);
            $("#OrderInstallFee_StartService").SetNumericCurrency(result.OrderInstallFee_StartServiceCurrencyType);
            $("#OrderInstallFee_StartService").val(result.OrderInstallFee_StartService);
            $("#InstallFeePaidBySECOM").SetNumericCurrency(result.InstallFeePaidBySECOMCurrencyType);
            $("#InstallFeePaidBySECOM").val(result.InstallFeePaidBySECOM);
            $("#InstallFeeRevenueBySECOM").SetNumericCurrency(result.InstallFeeRevenueBySECOMCurrencyType);
            $("#InstallFeeRevenueBySECOM").val(result.InstallFeeRevenueBySECOM);

            $("#InstallationSlipNo").text(result.InstallationSlipNo);

            if (result.InstallationSlipNo == "-"
                || result.InstallationSlipNo == "") {
                $("#divInstallationSlipNo").SetViewMode(true);
            }
            else {
                $("#divInstallationSlipNo").SetViewMode(false);
            }

        });
}

function AddClick_CTS220() {
    var objAdd = { SubContractCode: $("#SubcontractCode").val(), ProcessType: processType };

    call_ajax_method_json('/Contract/AddClick_CTS220', objAdd,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["SubcontractCode"], controls);
                return;
            }
            else if (result == undefined) {
                GetInstallation_CTS220("Add");
                $("#SubcontractCode").val("");
            }
        });
}

function GetInstallation_CTS220(e) {
    var objEvent = { from: e }

    mygridCTS220_09 = $("#gridInstallation").LoadDataToGridWithInitial(0, false, false, "/Contract/GetSubcontractor_CTS220",
    objEvent, "dtTbt_RentalInstSubContractorListForView", false);

    SpecialGridControl(mygridCTS220_09, ["Remove"]);

    BindOnLoadedEvent(mygridCTS220_09, function (gen_ctrl) {

        //        var removeColinx = mygridCTS220_09.getColIndexById('Remove');
        //        for (var i = 0; i < mygridCTS220_09.getRowsNum(); i++) {
        //            //mygridCTS220_09.cells2(i, removeColinx).setValue("<button id='btnRemove" + i.toString() + "' style='width:65px' >Remove</button>");

        //            var row_id = mygridCTS220_09.getRowId(i);
        //            GenerateRemoveButton(mygridCTS220_09, "btnRemove", row_id, "Remove", true);

        //            BindGridButtonClickEvent("btnRemove", row_id,
        //                        function (row_id) {
        //                            var doSubcontractorGrid = { SubContractCode: mygridCTS220_09.cells(row_id, 0).getValue() };
        //                            call_ajax_method('/Contract/RemoveClick_CTS220', doSubcontractorGrid,
        //                                function (result, controls) {
        //                                    GetInstallation_CTS220("Remove");
        //                                }, null);
        //                        }
        //                    );
        //        }

        //        //        mygridCTS220_09.attachEvent("onRowSelect", function (id, ind) {
        //        //            var row_num = mygridCTS220_09.getRowIndex(id);
        //        //            if (ind == mygridCTS220_09.getColIndexById('Remove')) {
        //        //                var doSubcontractorGrid = { SubContractCode: mygridCTS220_09.cells2(row_num, 0).getValue() };
        //        //                call_ajax_method('/Contract/RemoveClick_CTS220', doSubcontractorGrid,
        //        //                  function (result, controls) {
        //        //                      GetInstallation_CTS220("Remove");
        //        //                  }, null);
        //        //            }
        //        //        });

        BindInstallationGrid(true);
    });
}

function BindInstallationGrid(isEnbBtn) {
    var removeColinx = mygridCTS220_09.getColIndexById('Remove');
    for (var i = 0; i < mygridCTS220_09.getRowsNum(); i++) {
        var row_id = mygridCTS220_09.getRowId(i);
        GenerateRemoveButton(mygridCTS220_09, "btnRemove", row_id, "Remove", isEnbBtn);

        BindGridButtonClickEvent("btnRemove", row_id,
                        function (row_id) {
                            var doSubcontractorGrid = { SubContractCode: mygridCTS220_09.cells(row_id, 0).getValue() };
                            call_ajax_method('/Contract/RemoveClick_CTS220', doSubcontractorGrid,
                                function (result, controls) {
                                    GetInstallation_CTS220("Remove");
                                }, null);
                        }
                    );
    }
}

function OpenCMS180() {
    var obj = {
        ContractCode: $("#ContractCode").val(),
        InstallationSlipNo: $("#InstallationSlipNo").text()
    };

    ajax_method.CallScreenControllerWithAuthority("/Common/CMS180", obj, true);
}

function SetSectionModeCTS220_09(isView) {
    $("#divInstallationSection").SetViewMode(isView);

    if (isView) {
        $("#divSubContractorName").hide();

        if (mygridCTS220_09 != undefined) {
            var removeCol = mygridCTS220_09.getColIndexById("Remove");
            mygridCTS220_09.setColumnHidden(removeCol, true);
            mygridCTS220_09.setSizes();

            $("#gridInstallation").hide();
            $("#gridInstallation").show();
        }
    }
    else {
        $("#divSubContractorName").show();

        if (mygridCTS220_09 != undefined) {
            var removeCol = mygridCTS220_09.getColIndexById("Remove");
            mygridCTS220_09.setColumnHidden(removeCol, false);
            mygridCTS220_09.setSizes();

            $("#gridInstallation").hide();

            var objEvent = { from: "Remove" }
            $("#gridInstallation").LoadDataToGrid(mygridCTS220_09, 0, false, "/Contract/GetSubcontractor_CTS220",
                                                objEvent, "dtTbt_RentalInstSubContractorListForView", false, null, null);

            $("#gridInstallation").show();
        }
    }
}