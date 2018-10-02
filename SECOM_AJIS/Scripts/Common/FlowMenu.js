var FlowMenu_Method = {
    ConditionObject: null,
    ScreenLink: null,

    Initial: function () {
        var contractCode = $("#ContractCode").val();
        if (contractCode != "") {
            FlowMenu_Method.RetrieveClick();
        }
    },
    InitialControl: function () {
        $("#btnRetrieve").click(FlowMenu_Method.RetrieveClick);
        $("#PurchaserCode").click(FlowMenu_Method.PurchaserClick);
        $("#RealCustomerCode").click(FlowMenu_Method.RealCustomerClick);
        $("#SiteCode").click(FlowMenu_Method.SiteClick);

        $("#divFlowTable a").each(function () {
            $(this).click(function () {
                var param = {
                    LinkID: $(this).attr("id")
                };

                ajax_method.CallScreenController("/Common/SelectFlowMenuID", param,
                    function (result, controls) {
                        if (result != undefined) {
                            if (result.ObjectID == "CMS026") {
                                FlowMenu_Method.ScreenLink = result;
                                $("#dlgBox").OpenCMS026Dialog();
                            }
                            else {
                                var link = "/" + result.Controller + "/" + result.ObjectID;

                                var obj = "";
                                if (result.Parameters != undefined) {
                                    obj = new Object();
                                    for (var i = 0; i < result.Parameters.length; i++) {
                                        obj[result.Parameters[i]] = result.Values[i];
                                    }
                                }
                                ajax_method.CallScreenControllerWithAuthority(link, obj, true, result.SubObjectID);
                            }
                        }
                    }
                );

                return false;
            });
        });
    },

    RetrieveClick: function () {
        $("#divSaleContractBasicInformation").ResetToNormalControl();
        $("#btnRetrieve").attr("disabled", true);

        var param = {
            ContractCode: $("#ContractCode").val()
        };

        ajax_method.CallScreenController("/Common/RetrieveFlowMenu", param,
            function (result, controls) {
                $("#btnRetrieve").removeAttr("disabled");

                if (controls != undefined) {
                    VaridateCtrl(["ContractCode"], controls);

                }
                else if (result != undefined) {
                    $("#divSaleContractBasicInformation").clearForm();

                    $("#ContractCode").val(result.ContractCodeShort);
                    $("#OCC").text(result.OCC);

                    if (result.ContractTargetCustCodeShort != undefined) {
                        $("#PurchaserCode").text(result.ContractTargetCustCodeShort);
                    }
                    else {
                        $("#PurchaserCode").text(result.PurchaserCodeShort);
                    }

                    $("#RealCustomerCode").text(result.RealCustomerCodeShort);
                    $("#SiteCode").text(result.SiteCodeShort);
                    $("#CustFullNameEN").text(result.CustFullNameEN);
                    $("#SiteNameEN").text(result.SiteNameEN);
                    $("#CustFullNameLC").text(result.CustFullNameLC);
                    $("#SiteNameLC").text(result.SiteNameLC);

                    $("#divOCC").show();
                    $("#divInformation").show();
                    //$("#divFlowTable").show();
                    FlowMenu_Method.ConditionObject = result;

                    /* --- Set condition --- */
                    SEARCH_CONDITION = {
                        ContractCode: result.ContractCodeShort
                    };
                    /* --------------------- */
                }
            }
        );
    },
    PurchaserClick: function () {
        FlowMenu_Method.ConditionObject.Mode = "Purchaser";
        if (FlowMenu_Method.ConditionObject.ContractTargetCustCodeShort != undefined) {
            FlowMenu_Method.ConditionObject.Mode = "Contract";
        }

        $("#dlgBox").OpenCMS220Dialog();
        return false;
    },
    RealCustomerClick: function () {
        FlowMenu_Method.ConditionObject.Mode = "Customer";
        $("#dlgBox").OpenCMS220Dialog();
        return false;
    },
    SiteClick: function () {
        FlowMenu_Method.ConditionObject.Mode = "Site";
        $("#dlgBox").OpenCMS220Dialog();
        return false;
    }
};

$(document).ready(function () {
    FlowMenu_Method.Initial();
    FlowMenu_Method.InitialControl();
});

function CMS220Object() {
    return {
        ContractCode: FlowMenu_Method.ConditionObject.ContractCodeShort,
        OCC: FlowMenu_Method.ConditionObject.OCC,
        ContractTargetCode: FlowMenu_Method.ConditionObject.ContractTargetCustCodeShort,
        PurchaserCustCode:  FlowMenu_Method.ConditionObject.PurchaserCodeShort,
        RealCustomerCode: FlowMenu_Method.ConditionObject.RealCustomerCodeShort,
        SiteCode: FlowMenu_Method.ConditionObject.SiteCodeShort,
        Mode: FlowMenu_Method.ConditionObject.Mode
    };
}
function CMS026Object() {
    return {
        PopupSubMenuID: FlowMenu_Method.ScreenLink.PopupSubMenuID
    };
}
function CMS026Response(id) {
    $("#dlgBox").CloseDialog();

    if (FlowMenu_Method.ScreenLink != undefined) {
        var subController = null;
        if (FlowMenu_Method.ScreenLink.SubControllerList != undefined) {
            for (var idx = 0; idx < FlowMenu_Method.ScreenLink.SubControllerList.length; idx++) {
                if (id.indexOf(FlowMenu_Method.ScreenLink.SubControllerList[idx].ModulePrefix) >= 0) {
                    subController = FlowMenu_Method.ScreenLink.SubControllerList[idx].SubController;
                    break;
                }
            }
        }
        if (subController != null) {
            var link = "/" + subController + "/" + id;

            var obj = "";
            if (FlowMenu_Method.ScreenLink.Parameters != undefined) {
                obj = new Object();
                for (var i = 0; i < FlowMenu_Method.ScreenLink.Parameters.length; i++) {
                    obj[FlowMenu_Method.ScreenLink.Parameters[i]] = FlowMenu_Method.ScreenLink.Values[i];
                }
            }
            ajax_method.CallScreenControllerWithAuthority(link, obj, true, FlowMenu_Method.ScreenLink.SubObjectID);
        }
    }
}