jQuery(function() {
    var command_reset_click = function () {
        $('#txtOldPassword').val('');
        $('#txtNewPassword').val('');
        $('#txtConfirmNewPassword').val('');
    };

    var command_register_click = function () {
        if ($('#txtNewPassword').val() == '') {
            OpenErrorMessageDialog("", "New password is required.");
            return;
        }

        if ($('#txtNewPassword').val() != $('#txtConfirmNewPassword').val()) {
            OpenErrorMessageDialog("", "New password does not match Confirm new password");
            return;
        };

        var params = {
            oldPassword: $('#txtOldPassword').val(),
            newPassword: $('#txtNewPassword').val()
        };

        call_ajax_method("/Master/UpdatePassword", params, function (data) {
            if (data == 0) {
                OpenErrorMessageDialog("MSGM071", "Old password is not correct.");
            } else {
                var obj = {
                    module: "Common",
                    code: "MSG0046",
                    param: ""
                };

                ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
                    OpenInformationMessageDialog(result.Code, result.Message, command_reset_click);
                });                
            };
        });
        
    };

    SetRegisterCommand(true, command_register_click);
    SetResetCommand(true, command_reset_click);
})