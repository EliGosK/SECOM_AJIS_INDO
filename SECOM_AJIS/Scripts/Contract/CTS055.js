/*--- Main ---*/
$(document).ready(function () {
    InitialCommandControl();

    /*==== Attach Datetime Picker ====*/
    InitialDateFromToControl("#dpExpectOperationDate", "#dpExpectOperationDate");
    $("#ContractChange").SetViewMode(false);

    $("#chkIsimportantCustomer").attr("readonly", true);
    $("#chkIsimportantCustomer").attr("disabled", true);
    $("#btnViewInstalltionDetail").click(function () { ViewInstalltionDetailClick(); });

    BindValidateBusiness();
    $("#chkIsimportantCustomer").attr("readonly", true);
    $("#chkIsimportantCustomer").attr("disabled", true);
});

function InitialCommandControl() {
    SetOKCommand(true, OKClick);
    SetCancelCommand(true, CancelClick);
}

/* --- Events --- */

function InitialEvent() {    
   
}

function BindValidateBusiness() {
    call_ajax_method_json('/Contract/GetValidateBusiness_CTS055', "",
    function (result, controls) {
        if (result.InstallationStatusCode == "99") {
            $("#btnViewInstalltionDetail").attr("disabled", true);
        }
    });
}


function OKClick() {
    VaridateCtrl(['dpExpectOperationDate'], null);
    DisableRegisterCommand(true);
    var obj = {
                ContractCode: ObjCTS055.ContractCode,
                OCC: ObjCTS055.OCC,
                ExpectedOperationDate: $("#dpExpectOperationDate").val()
              };

              call_ajax_method_json('/Contract/ValidateALL_CTS055', obj,
              function (result, controls) {
                  VaridateCtrl(['dpExpectOperationDate'], controls);
                  if (result == undefined) {
                      call_ajax_method_json('/Contract/OKClick_CTS055', obj, function (result, controls) {
                          if (result != undefined) {
                              OpenInformationMessageDialog(result.Code, result.Message,
                              function () {
                                  window.location.href = generate_url("/common/CMS020");
                              }, null);
                          }
                      });
                  } else {
                      DisableRegisterCommand(false);
                  }
              });
}

function CancelClick() {
    window.location.href = generate_url("/common/CMS020");
}

function ViewInstalltionDetailClick() {
    //$("#dlgCTS055").OpenCMS180Dialog('CTS055');
    var obj = {
        ContractCode: $('#txtContractCode').val()
    };
    ajax_method.CallScreenControllerWithAuthority("/Common/CMS180", obj, true);
}