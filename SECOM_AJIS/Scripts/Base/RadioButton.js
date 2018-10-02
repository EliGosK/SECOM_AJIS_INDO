function radio_mapping_control(ctrl, func) {
    $(ctrl).each(function () {
        $(this).change(function () {
            if (func != null) {
                func($(this).attr("id"));
            }
        });
    });
}