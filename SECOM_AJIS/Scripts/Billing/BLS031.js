var cbo_width = "214px";

$(document).ready(function () {

    //-------------------- initial even
    //$("#BLS031_BankCode").RelateControlEvent(bank_branch_change); 
    $("#BLS031_BankCode").change(bank_branch_change);
    InitialNumericInputTextBox(["BLS031_Account1"], false);
    //InitialNumericInputTextBox(["BLS031_Account1", "BLS031_Account2", "BLS031_Account3", "BLS031_Account4"], false);   
}
);

//----------------- set regenareate combo
function bank_branch_change(istab, isblur) {


    var Bank = { "BankCode": $("#BLS031_BankCode").val() };
    ajax_method.CallScreenController("/Billing/BLS031_BankBranchData", Bank,
     function (result, controls) {
         if (result != undefined) {
             update_bankBranch_combo(result);

         }
     });    

}
function update_bankBranch_combo(data) {
    regenerate_combo("#BLS031_BankBranchCode", data);

//     if (objBLS031.BankBranchCode != null && objBLS031.BankBranchCode != undefined && objBLS031.BankBranchCode != "") {
//         $("#BLS031_BankBranchCode").val(objBLS031.BankBranchCode);
//     }
}

//---------------------- set inintial popup screen
function BLS031Initial() {
    
   $(".popup-dialog").css("overflow-y" , "auto");

    ChangeDialogButtonText(
            ["OK", "Cancel"],
            [$("#btnOK").val(),
             $("#btnCancel").val()]);

    BindDialogButtonClick($("#btnOK").val(), function () {
        ConfirmData();
    });
    if (objBLS031 != undefined) {
        SetValueInitial();
    }
    else {
        $("#divRegisterAutoTransferInfo").clearForm();
    }
   $("#BLS031_AccountName").focus();
}

function SetValueInitial() {
    $("#BLS031_ContractCode").val(objBLS031.ContractCode);
    $("#BLS031_BillingClientCode").val(objBLS031.BillingClientCode);
    $("#BLS031_BillingClientNameEN").val(objBLS031.BillingClientNameEN);
    $("#BLS031_BillingClientNameLC").val(objBLS031.BillingClientNameLC);
    $("#BLS031_AccountName").val(objBLS031.AccountName);
    if (objBLS031.BankCode != null && objBLS031.BankCode != undefined && objBLS031.BankCode != "") {
        $("#BLS031_BankCode").val(objBLS031.BankCode);
        bank_branch_change_fromCallerScreen(objBLS031.BankCode , objBLS031.BankBranchCode);
    }
    
    $("#BLS031_AccountType").val(objBLS031.AccountType);
    $("#BLS031_LastestResult").val(objBLS031.LastestResult);
    $("#BLS031_Account1").val(objBLS031.Account1);
    //$("#BLS031_Account2").val(objBLS031.Account2);
    //$("#BLS031_Account3").val(objBLS031.Account3);
    //$("#BLS031_Account4").val(objBLS031.Account4);
    $("#BLS031_AutoTransferDate").val(objBLS031.AutoTransferDate);  
}

function ConfirmData() {
    //var obj = CreateObjectData($("#AutoTransfer").serialize());
    var obj = {
        AccountName: $("#BLS031_AccountName").val(),
        BankCode: $("#BLS031_BankCode").val(),
        BankName: $("#BLS031_BankCode").find("$:selected").text(),
        BankBranchCode: $("#BLS031_BankBranchCode").val(),
        BankBranchName: $("#BLS031_BankBranchCode").find("$:selected").text(),
        AccountType: $("#BLS031_AccountType").val(),
        AccountTypeName: $("#BLS031_AccountType").find(":selected").text(),
        AccountNo: $("#BLS031_Account1").val(),// + "-" + $("#BLS031_Account2").val() + "-" + $("#BLS031_Account3").val() + "-" + $("#BLS031_Account4").val(),
        AutoTransferDate: $("#BLS031_AutoTransferDate").val(),
        Account1: $("#BLS031_Account1").val(),
        Account2: '0',// $("#BLS031_Account2").val(),
        Account3: '0',//$("#BLS031_Account3").val(),
        Account4: '0',//$("#BLS031_Account4").val(),
    };

    // Add by Jirawat Jannet 2016-09-02
    if (obj.BankBranchCode == null) obj.BankBranchCode = objBLS031.BankBranchCode;
    if (obj.BankBranchName == null) obj.BankBranchName = '';
    // End add

      ajax_method.CallScreenController("/Billing/BLS031_ConfirmData", obj,
    function (result, controls  , isWarning) {

        if (controls != undefined) {
            VaridateCtrl([
            "BLS031_AccountName",
            "BLS031_BankCode",
            "BLS031_BankBranchCode",
            "BLS031_AccountType",
            "BLS031_Account1",
            //"BLS031_Account2",
            //"BLS031_Account3",
            //"BLS031_Account4",
            "BLS031_AutoTransferDate"], controls);
            return;
        }


        if (result != undefined && isWarning == undefined) {
            if (typeof (BLS031Response) == "function")
                BLS031Response(result);
        }
    }  
    );

}
function bank_branch_change_fromCallerScreen(strBankCode , bankBranchCode) {
    var Bank = { "BankCode": strBankCode };
    ajax_method.CallScreenController("/Billing/BLS031_BankBranchData", Bank,
     function (result, controls) {
         if (result != undefined) {
             update_bankBranch_combo(result);

             $("#BLS031_BankBranchCode").val(bankBranchCode);
         }
     });    
}








