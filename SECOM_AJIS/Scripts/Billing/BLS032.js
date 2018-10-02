
$(document).ready(function () {

    InitialNumericInputTextBox(["BLS032_CreditCardNo1", "BLS032_CreditCardNo2", "BLS032_CreditCardNo3", "BLS032_CreditCardNo4" ,"BLS032_ExpYear"], false);

}
);

//---------------------- set inintial popup screen
function BLS032Initial() {
    $(".popup-dialog").css("overflow-y" , "auto");
    ChangeDialogButtonText(
            ["OK", "Cancel"],
            [$("#btnOK").val(),
             $("#btnCancel").val()]);
    
    BindDialogButtonClick($("#btnOK").val(), function () {
        ConfirmData();
    });
    if (objBLS032 != undefined) {
        SetValueInitial();
    }
    else {
        $("#divRegisterCreditCardInfo").clearForm();
    }

}

function SetValueInitial() {
   // alert(objBLS032.ContractCode);
    $("#BLS032_ContractCode").val(objBLS032.ContractCode);
    $("#BLS032_BillingClientCode").val(objBLS032.BillingClientCode);
    $("#BLS032_BillingClientNameEN").val(objBLS032.BillingClientNameEN);
    $("#BLS032_BillingClientNameLC").val(objBLS032.BillingClientNameLC);
    $("#BLS032_CreditCardType").val(objBLS032.CreditCardType);
    $("#BLS032_CardName").val(objBLS032.CardName);
    $("#BLS032_CreditCardCompanyCode").val(objBLS032.CreditCardCompanyCode);
    $("#BLS032_CreditCardNo1").val(objBLS032.CreditCardNo1);
    $("#BLS032_CreditCardNo2").val(objBLS032.CreditCardNo2);
    $("#BLS032_CreditCardNo3").val(objBLS032.CreditCardNo3);
    $("#BLS032_CreditCardNo4").val(objBLS032.CreditCardNo4);
    $("#BLS032_ExpYear").val(objBLS032.ExpYear);
    $("#BLS032_ExpMonth").val(objBLS032.ExpMonth);

}

function ConfirmData() {
    //var obj = CreateObjectData($("#CreditCard").serialize());
    var obj = {
            CreditCardType: $("#BLS032_CreditCardType").val(),
            CreditCardTypeName: $("#BLS032_CreditCardType").find(":selected").text(),
            CardName: $("#BLS032_CardName").val(),
            CreditCardCompanyCode: $("#BLS032_CreditCardCompanyCode").val(),
            CreditCardCompanyName: $("#BLS032_CreditCardCompanyCode").find(":selected").text(),
            CreditCardNo: $("#BLS032_CreditCardNo1").val() + "-" + $("#BLS032_CreditCardNo2").val() + "-" + $("#BLS032_CreditCardNo3").val() + "-" + $("#BLS032_CreditCardNo4").val(),
            ExpYear: $("#BLS032_ExpYear").val(),
            ExpMonth: $("#BLS032_ExpMonth").val(),
            CreditCardNo1: $("#BLS032_CreditCardNo1").val(),
            CreditCardNo2: $("#BLS032_CreditCardNo2").val(),
            CreditCardNo3: $("#BLS032_CreditCardNo3").val(),
            CreditCardNo4: $("#BLS032_CreditCardNo4").val(),
    };
    
    ajax_method.CallScreenController("/Billing/BLS032_ConfirmData", obj,
    function (result, controls , isWarning) {

        if (controls != undefined) {
            VaridateCtrl([
            "BLS032_CreditCardType",
            "BLS032_CardName",
            "BLS032_CreditCardCompanyCode",
            "BLS032_CreditCardNo1",
            "BLS032_CreditCardNo2",
            "BLS032_CreditCardNo3",
            "BLS032_CreditCardNo4",
            "BLS032_ExpMonth",
            "BLS032_ExpYear"], controls);
            return;
        }
        



        if (result != undefined && isWarning == undefined) {
            if (typeof (BLS031Response) == "function")
                BLS032Response(result);
        }
    }
    );

}

