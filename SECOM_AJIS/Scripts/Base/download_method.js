var download_method = $.extend({
    CallDownloadController: function (iframeId, url, obj) {
        if (url.indexOf("k=") < 0) {

            var key = ajax_method.GetKeyURL(null);
            if (key != "") {
                if (url.indexOf("?") > 0) {
                    url = url + "&k=" + key;
                }
                else {
                    url = url + "?k=" + key;
                }
            }
        }

        url = ajax_method.GenerateURL(url);

        if (obj != undefined) {
            var objParam = decodeURIComponent($.param(obj));
            if (objParam.length > 0) {
                url = url + "&" + objParam;
            }
        }

//        call_ajax_method("/common/InitialDownloadFile", "", function () {
//            master_event.IsChangeLanguageEvent = true;
//            $("#" + iframeId).get(0).src = url;
//        });

        $("#" + iframeId).get(0).src = url;
    }
});