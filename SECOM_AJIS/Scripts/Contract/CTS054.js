/*--- Main ---*/
$(document).ready(function () {
    EnableRegisterCommand();
    /*==== Attach Datetime Picker ====*/
    $("#dpExpectOperationDate").InitialDate();
    $("#ContractChange").SetViewMode(false);

    $("#chkIsimportantCustomer").attr("readonly", true);
    $("#chkIsimportantCustomer").attr("disabled", true);
    $("#btnViewInstalltionDetail").click(function () { ViewInstalltionDetailClick(); });

    BindExpectedOperationDate();
});

function InitialCommandControl() {
    SetRegisterCommand(true, RegisterClick);
    SetResetCommand(true, ResetClick);
}

/* --- Events --- */

function InitialEvent() {    
   
}

function BindExpectedOperationDate() {
    call_ajax_method_json('/Contract/GetExpectedOperationDate_CTS054', "",
    function (result, controls) {
        //$("#dpExpectOperationDate").datepicker("getDate");
        result.ExpectedOperationDate = ConvertDateObject(result.ExpectedOperationDate);
        $("#dpExpectOperationDate").val(ConvertDateToTextFormat(result.ExpectedOperationDate));
        if (result.InstallationStatusCode == "99") 
        {
            $("#btnViewInstalltionDetail").attr("disabled", true);
        }

    });
}

function RegisterClick() {
    VaridateCtrl(['dpExpectOperationDate'], null);
    DisableRegisterCommand(true);
    DisableResetCommand(true);
    var obj = {
        ContractCode: ObjCTS054.ContractCode,
        OCC: ObjCTS054.OCC,
        ExpectedOperationDate: $("#dpExpectOperationDate").val()
    };

    call_ajax_method_json('/Contract/ValidateALL_CTS054', obj,
    function (result, controls) {
        VaridateCtrl(['dpExpectOperationDate'], controls);
        if (result == undefined) {
            EnableConfirmCommand();
            $("#ContractChange").SetViewMode(true);
            $("#ExpectOperationDateChange").SetViewMode(true);
        }
        DisableRegisterCommand(false);
        DisableResetCommand(false);
    });

}

function ConfirmClick() {
    DisableConfirmCommand(true);
    var obj = {
                ContractCode: ObjCTS054.ContractCode,
                OCC: ObjCTS054.OCC,
                ExpectedOperationDate: $("#dpExpectOperationDate").val()
              };

              call_ajax_method_json('/Contract/ValidateALL_CTS054', obj,
              function (result, controls) {
                  if (result == undefined) {
                      call_ajax_method_json('/Contract/ConfirmClick_CTS054', obj, function (result, controls) {
                          if (result != undefined) {
                              OpenInformationMessageDialog(result.Code, result.Message, function () { window.location.href = generate_url("/common/CMS020"); });
                          }
                      });
                  }                 
              });
}

function ResetClick() {
    DisableRegisterCommand(true);
    DisableResetCommand(true);
    /* --- Get Message --- */
    var obj = {
        module: "Common",
        code: "MSG0038"
    };
    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        DisableRegisterCommand(false);
        DisableResetCommand(false);
        OpenOkCancelDialog(result.Code, result.Message,
        function () {
            call_ajax_method_json('/Contract/ResetClick_CTS054', ObjCTS054,
              function (result, controls) {
                  if (result != null) {
                      //$("#dpExpectOperationDate").val(result);
                      EnableRegisterCommand();
                      ReBindingData(result);
                  }
              });
        },
        null);
    });
  }

  function ReBindingData(dataObj) {
      ObjCTS054 = { ContractCodeShort: dataObj.ContractCodeShort
        , ContractCode: dataObj.ContractCode
        , OCC: dataObj.OCC
        , ContractStatus: dataObj.ContractStatus
        , QuotationTargetCode: dataObj.QuotationTargetCode
        , Alphabet: ""
        , EmpNo: ""
        , EmpName: ""
        , DisplayAll: ""
        , BillingClientCode: ""
        , BillingOffice: ""
        , PaymentMethod: ""
        , Sequence: ""
        , ServiceTypeCode: dataObj.ServiceTypeCode
        , TargetCodeType: dataObj.TargetCodeType
        , EndContractDate: dataObj.EndContractDate
        , InstallationStatusCode: dataObj.InstallationStatusCode
      }

      $('#txtContractCode').val(dataObj.ContractCodeShort);
      $('#txtUserCode').val(dataObj.UserCode);
      $('#txtCustomerCode').val(dataObj.CustomerCode);
      $('#txtCustomerCodeReal').val(dataObj.RealCustomerCode);
      $('#txtSiteCode').val(dataObj.SiteCode);
      if (dataObj.ImportantFlag) {
          $('#chkIsimportantCustomer').attr('checked', 'checked');
      } else {
          $('#chkIsimportantCustomer').removeAttr('checked');
      }
      $('#txtContractTargetName').val(dataObj.CustFullNameEN);
      $('#txtContractTargetAddress').val(dataObj.AddressFullEN);
      $('#txtSiteName').val(dataObj.SiteName);
      $('#txtSiteAddress').val(dataObj.SiteAddress);
      $('#txtCustFullNameLC').val(dataObj.CustFullNameLC);
      $('#txtContractTargetAddressLocal').val(dataObj.AddressFullLC);
      $('#txtSiteNameLocal').val(dataObj.SiteNameLC);
      $('#txtSiteAddressLocal').val(dataObj.SiteAddressLC);
      $('#txtInstallationStatus').val(dataObj.InstallationStatus);
      $('#txtOperationOffice').val(dataObj.OfficeName);
      $('#QuotationTargetCode').val(dataObj.QuotationTargetCode);
      $("#dpExpectOperationDate").val(dataObj.ExpectOperationDate);
  }

function BackClick() {
    DisableConfirmCommand(true);
    /* --- Set Command Button --- */
    EnableRegisterCommand();
    $("#ContractChange").SetViewMode(false);
    $("#chkIsimportantCustomer").attr("disabled", true);
    $("#chkIsimportantCustomer").attr("readonly", true);

    $("#ExpectOperationDateChange").SetViewMode(false);

    DisableConfirmCommand(false);
}

function ViewInstalltionDetailClick() {
    //$("#dlgCTS055").OpenCMS180Dialog('CTS054');
    var obj = {
        ContractCode: $('#txtContractCode').val()
    };
    ajax_method.CallScreenControllerWithAuthority("/Common/CMS180", obj, true);
}

function EnableRegisterCommand() {
    DisableAllCommand();
    SetRegisterCommand(true, RegisterClick);
    SetResetCommand(true, ResetClick);
}

function EnableConfirmCommand() {
    DisableAllCommand();
    SetConfirmCommand(true, ConfirmClick);
    SetBackCommand(true, BackClick);
}

function DisableAllCommand() {
    SetRegisterCommand(false, null);
    SetResetCommand(false, null);
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
}

function CMS180Object() {
    return {
        strCallerScreenID: "CTS055",
        ContractCode: $("#lblContractCode").val()       
    };
}

