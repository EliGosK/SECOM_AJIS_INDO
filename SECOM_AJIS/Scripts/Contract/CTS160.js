var CTS160_Method = {
    Initial: function () {
        if ($("#ContractCode").val() != "") {
            $("#ContractCode").attr("readonly", true);
            $("#OCCAlphabet").attr("readonly", true);
            $("#btnRetrieve").attr("disabled", true);

            $("#divDocumentTemplate").show();

            $("#rdoContract").DisableControl();
            $("#rdoMemorandum").DisableControl();
            $("#rdoNotice").DisableControl();
            $("#rdoCoverLetter").DisableControl();

            CTS160_Method.GenerateRentalDocumentComboBox();
        }

        $("#ServiceType").SetEnableView(false);

        CTS160_Method.InitialControl();
        CTS160_Method.DocumentTemplateTypeChange();
        CTS160_Method.SetCommandMode(0);
    },
    InitialControl: function () {
        $("#ContractMemo").SetMaxLengthTextArea(500);

        $("#btnRetrieve").click(CTS160_Method.RetrieveClick);
        $("#btnClear").click(CTS160_Method.ClearClick);
        $("#btnSelect").click(CTS160_Method.SelectClick);
        $("#btnRegisterNextDocument").click(CTS160_Method.RegisterNextClick);

        $("#rdoContract").change(CTS160_Method.DocumentTemplateTypeChange);
        $("#rdoMemorandum").change(CTS160_Method.DocumentTemplateTypeChange);
        $("#rdoNotice").change(CTS160_Method.DocumentTemplateTypeChange);
        $("#rdoCoverLetter").change(CTS160_Method.DocumentTemplateTypeChange);

        

        InitialTrimTextEvent(["ApproveNo1", "ApproveNo2", "ApproveNo3"]);
    },

    RetrieveClick: function () {
        $("#btnRetrieve").attr("disabled", true);

        var param = {
            ContractCode: $("#ContractCode").val(),
            OCC: $("#OCCAlphabet").val()
        };

        ajax_method.CallScreenController("/Contract/CTS160_RetrieveContractData", param,
            function (result, controls) {
                $("#btnRetrieve").removeAttr("disabled");

                if (controls != undefined) {
                    VaridateCtrl(["ContractCode", "OCCAlphabet"], controls);


                }
                else if (result != undefined) {
                    $("#OCCAlphabet").val(result[0].OCCAlphabet); //Add by Jutarat A. on 02052013

                    $("#ContractCode").attr("readonly", true);
                    $("#OCCAlphabet").attr("readonly", true);
                    $("#btnRetrieve").attr("disabled", true);

                    $("#divContractTargetPurchaserInfo").bindJSON(result[0]);
                    $("#ContractType").val(result[0].ContractType);
                    $("#DisabledrdoContract").val(!result[1].ContractReportFlag);
                    $("#DisabledrdoMemorandum").val(!result[1].MemorandumFlag);
                    $("#DisabledrdoNotice").val(!result[1].NoticeFlag);
                    $("#DisabledrdoCoverLetter").val(!result[1].CoverLetterFlag);

                    $("#rdoContract").removeAttr("disabled");
                    $("#rdoMemorandum").removeAttr("disabled");
                    $("#rdoNotice").removeAttr("disabled");
                    $("#rdoCoverLetter").removeAttr("disabled");

                    $("#rdoContract").DisableControl();
                    $("#rdoMemorandum").DisableControl();
                    $("#rdoNotice").DisableControl();
                    $("#rdoCoverLetter").DisableControl();

                    $("#divDocumentTemplate").show();
                    CTS160_Method.DocumentTemplateTypeChange();

                    /* --- Set condition --- */
                    SEARCH_CONDITION = {
                        ContractCode: param.ContractCode
                    };
                    /* --------------------- */

                    CTS160_Method.GenerateRentalDocumentComboBox();
                }
            }
        );
    },

    ClearClick: function () {
        //Clear data.
        ajax_method.CallScreenController("/Contract/CTS160_ClearContractCode", "", function () {
            SEARCH_CONDITION = null;
        });

        $("#divContractTargetPurchaserInfo").clearForm();
        $("#ContractCode").removeAttr("readonly");
        $("#OCCAlphabet").removeAttr("readonly");
        $("#btnRetrieve").removeAttr("disabled");


        $("#btnSelect").removeAttr("disabled");
        $("#divDocumentTemplate").SetEnableView(true);

        $("#divCoverLetterInformation").clearForm();
        $("#divCoverLetterInformation").hide();

        $("#divDocumentTemplate").clearForm();
        $("#divDocumentTemplate").hide();

        $("#rdoContract").removeAttr("checked");
        $("#rdoMemorandum").removeAttr("checked");
        $("#rdoNotice").removeAttr("checked");
        $("#rdoCoverLetter").removeAttr("checked");

        CTS160_Method.DocumentTemplateTypeChange();
        CTS160_Method.SetCommandMode(0);


    },

    SelectClick: function () {
        $("#btnSelect").attr("disabled", true);

        var docName = $("#RentalDocumentName").val();
        if ($("#ContractType").val() == 2) {    //Sale
            docName = $("#SaleDocumentName").val();
        }

        var param = {
            ContractReportFlag: $("#rdoContract").prop("checked"),
            MemorandumFlag: $("#rdoMemorandum").prop("checked"),
            NoticeFlag: $("#rdoNotice").prop("checked"),
            CoverLetterFlag: $("#rdoCoverLetter").prop("checked"),
            SignatureFlag: $("#ShowSignature").prop("checked"),
            ContractLanguage: $("#ContractDocumentLanguage").val(),
            DocumentName: docName
        };

        ajax_method.CallScreenController("/Contract/CTS160_SelectDocumentTemplate", param,
            function (result, controls) {
                $("#btnSelect").removeAttr("disabled");

                if (controls != undefined) {
                    VaridateCtrl(["ContractDocumentLanguage", "RentalDocumentName", "SaleDocumentName"], controls);

                }
                else if (result != undefined) {
                    $("#btnSelect").attr("disabled", true);
                    $("#divDocumentTemplate").SetEnableView(false);
                    $("#divCoverLetterInformation").show();

                    $("#divCoverLetterInformation").bindJSON(result);

                    $("#QuotationFee").val(SetNumericValue(result.QuotationFee, 2));
                    $("#QuotationFee").setComma();
                    $("#ContractFee").val(SetNumericValue(result.ContractFee, 2));
                    $("#ContractFee").setComma();
                    $("#ReceivedDepositFee").val(SetNumericValue(result.ReceivedDepositFee, 2));
                    $("#ReceivedDepositFee").setComma();

                    if (result.FireMonitoringFlag == true) {
                        $("#ServiceType_1").attr("checked", true);
                    }
                    else {
                        $("#ServiceType_1").removeAttr("checked");
                    }
                    if (result.CrimePreventionFlag == true) {
                        $("#ServiceType_2").attr("checked", true);
                    }
                    else {
                        $("#ServiceType_2").removeAttr("checked");
                    }
                    if (result.EmergencyReportFlag == true) {
                        $("#ServiceType_3").attr("checked", true);
                    }
                    else {
                        $("#ServiceType_3").removeAttr("checked");
                    }
                    if (result.FacilityMonitoringFlag == true) {
                        $("#ServiceType_4").attr("checked", true);
                    }
                    else {
                        $("#ServiceType_4").removeAttr("checked");
                    }

                    $("#gridInstrumentList").LoadDataToGridWithInitial(0, false, false,
                    "/Contract/CTS160_GetInstrumentListData", "",
                    "CTS160_InstrumentDetail", false);

                    CTS160_Method.SetCommandMode(1);
                }
            }
        );
    },

    DocumentTemplateTypeChange: function () {
        if ($("#rdoContract").prop("checked") == true) {
            $("#ContractDocumentLanguage").removeAttr("disabled");
            $("#ContractDocumentLanguage").val($("#C_DOC_LANGUAGE_EN").val());
        }
        else {
            $("#ContractDocumentLanguage").attr("disabled", true);
            $("#ContractDocumentLanguage").val(CTS160_CONST.C_DOC_LANGUAGE_EN);
        }

        if ($("#ContractType").val() == 2) {    //Sale
            $("#RentalDocumentName").hide();
            $("#SaleDocumentName").show();
        }
        else {
            $("#RentalDocumentName").show();
            $("#SaleDocumentName").hide();
        }

        if ($("#rdoCoverLetter").prop("checked") == true) {
            $("#RentalDocumentName").removeAttr("disabled");
            $("#SaleDocumentName").removeAttr("disabled");
        }
        else {
            $("#RentalDocumentName").attr("disabled", true);
            $("#SaleDocumentName").attr("disabled", true);

            $("#RentalDocumentName").val("");
            $("#SaleDocumentName").val("");
        }
    },

    GenerateRentalDocumentComboBox: function () {
        call_ajax_method_json('/Contract/CTS160_InitialRentalDocumentName', "",
            function (result, controls) {
                regenerate_combo("#RentalDocumentName", result);
            }
        );
    },

    SetCommandMode: function (mode) {
        if (mode == 0) {    //Initital
            register_command.SetCommand(null);
            reset_command.SetCommand(null);
            confirm_command.SetCommand(null);
            back_command.SetCommand(null);
        }
        else if (mode == 1) {   //Register
            register_command.SetCommand(CTS160_Method.RegisterCommandClick);
            reset_command.SetCommand(CTS160_Method.ResetCommandClick);
            confirm_command.SetCommand(null);
            back_command.SetCommand(null);
        }
        else if (mode == 2) {   //Confirm
            register_command.SetCommand(null);
            reset_command.SetCommand(null);
            confirm_command.SetCommand(CTS160_Method.ConfirmCommandClick);
            back_command.SetCommand(CTS160_Method.BackCommandClick);
        }
    },

    RegisterCommandClick: function () {
        ajax_method.CallScreenController("/Contract/CTS160_RegisterDocumentTemplate", "",
            function (result) {
                if (result == true) {
                    $("#divContractTargetPurchaserInfo").SetViewMode(true);
                    $("#divDocumentTemplate").SetViewMode(true);
                    $("#divCoverLetterInformation").SetViewMode(true);

                    if ($("#ContractType").val() == 2) {    //Sale
                        $("#divRentalDocumentName").attr("display", "none");
                    }
                    else {
                        $("#divSaleDocumentName").attr("display", "none");
                    }

                    $("#divSelectCommand").attr("display", "none");
                    CTS160_Method.SetCommandMode(2);
                }
            }
        );
    },

    ResetCommandClick: function () {
        $("#btnSelect").removeAttr("disabled");
        $("#divDocumentTemplate").SetEnableView(true);

        $("#divCoverLetterInformation").clearForm();
        $("#divCoverLetterInformation").hide();

        $("#divDocumentTemplate").clearForm();

        $("#rdoContract").removeAttr("checked");
        $("#rdoMemorandum").removeAttr("checked");
        $("#rdoNotice").removeAttr("checked");
        $("#rdoCoverLetter").removeAttr("checked");

        $("#rdoContract").DisableControl();
        $("#rdoMemorandum").DisableControl();
        $("#rdoNotice").DisableControl();
        $("#rdoCoverLetter").DisableControl();

        CTS160_Method.DocumentTemplateTypeChange();
        CTS160_Method.SetCommandMode(0);
    },

    ConfirmCommandClick: function () {
        var docName = $("#RentalDocumentName").val();
        if ($("#ContractType").val() == 2) {    //Sale
            docName = $("#SaleDocumentName").val();
        }

        var param = {
            template: {
                ContractReportFlag: $("#rdoContract").prop("checked"),
                MemorandumFlag: $("#rdoMemorandum").prop("checked"),
                NoticeFlag: $("#rdoNotice").prop("checked"),
                CoverLetterFlag: $("#rdoCoverLetter").prop("checked"),
                SignatureFlag: $("#ShowSignature").prop("checked"),
                ContractLanguage: $("#ContractDocumentLanguage").val(),
                DocumentName: docName
            },
            data: CreateObjectData($("#formCoverLetterInformation").serialize())
        };

        ajax_method.CallScreenController("/Contract/CTS160_ConfirmDocumentTemplate", param,
            function (result) {
                if (result != undefined) {
                    OpenInformationMessageDialog(result[0].Code, result[0].Message,
                        function () {
                            $("#divResultOfRegister").show();
                            master_event.ScrollWindow("#divResultOfRegister");

                            $("#ContractDocumentNo").val(result[1]);
                            $("#btnRegisterNextDocument").focus();

                            CTS160_Method.SetCommandMode(0);
                        }
                    );
                }
            }
        );
    },

    BackCommandClick: function () {
        $("#divSelectCommand").removeAttr("display");

        $("#divContractTargetPurchaserInfo").SetViewMode(false);
        $("#divDocumentTemplate").SetViewMode(false);
        $("#divCoverLetterInformation").SetViewMode(false);

        $("#rdoContract").DisableControl();
        $("#rdoMemorandum").DisableControl();
        $("#rdoNotice").DisableControl();
        $("#rdoCoverLetter").DisableControl();

        $("#ServiceType").SetEnableView(false);

        CTS160_Method.DocumentTemplateTypeChange();
        CTS160_Method.SetCommandMode(1);
    },

    RegisterNextClick: function () {
        $("#ContractCode").removeAttr("readonly");
        $("#OCCAlphabet").removeAttr("readonly");
        $("#btnRetrieve").removeAttr("disabled");
        $("#btnSelect").removeAttr("disabled");
        $("#divDocumentTemplate").SetEnableView(true);

        $("#divContractTargetPurchaserInfo").SetViewMode(false);
        $("#divDocumentTemplate").SetViewMode(false);
        $("#divCoverLetterInformation").SetViewMode(false);

        $("#divCoverLetterInformation").clearForm();
        $("#divDocumentTemplate").clearForm();
        $("#divContractTargetPurchaserInfo").clearForm();

        $("#rdoContract").removeAttr("checked");
        $("#rdoMemorandum").removeAttr("checked");
        $("#rdoNotice").removeAttr("checked");
        $("#rdoCoverLetter").removeAttr("checked");

        $("#rdoContract").DisableControl();
        $("#rdoMemorandum").DisableControl();
        $("#rdoNotice").DisableControl();
        $("#rdoCoverLetter").DisableControl();
        CTS160_Method.DocumentTemplateTypeChange();

        $("#divCoverLetterInformation").hide();
        $("#divDocumentTemplate").hide();
        $("#divResultOfRegister").hide();
        $("#divSelectCommand").removeAttr("display");

        CTS160_Method.SetCommandMode(0);

        $("#ContractCode").focus();
    }

};


$(document).ready(function () {
    CTS160_Method.Initial();
});


reset_command.SetCommand = function (func) {
    var ctrlC = $("#" + this.Control);

    ctrlC.unbind("click");
    if (func != undefined && typeof (func) == "function") {
        ctrlC.show();
        ctrlC.removeAttr("disabled");
        ctrlC.click(function () {
            command_control.CommandControlMode(false);

            var obj = {
                module: "Common",
                code: "MSG0038"
            };
            ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
                OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    func();
                },
                function () {
                    command_control.CommandControlMode(true);
                });
            });
        });
    }
    else {
        ctrlC.hide();
    }
}