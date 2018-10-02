function CMS026Initial() {
    ChangeDialogButtonText(
            ["Close"],
            [$("#btnClose").val()]);
}

var CMS026_Method = {
    InitialControl: function () {
        $("#divPopupSubMenuList button").each(function () {
            $(this).click(function () {
                var id = $(this).attr("id");

                if (typeof (CMS026Response) == "function")
                    CMS026Response(id);
            });
        });
    }
}

$(document).ready(function () {
    CMS026_Method.InitialControl();
});