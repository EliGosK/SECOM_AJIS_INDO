/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>

function InitialAutoCompleteControl(ctrl, key_val, url, obj, func) {
    /// <summary>Add jQuery AutoComplete to TextBox</summary>
    /// <param name="ctrl" type="string">Control Id</param>
    /// <param name="url" type="string">Controller Path</param>
    /// <param name="obj" type="string">Input Parameters</param>
    /// <param name="func" type="function">Function when finish</param>

    call_ajax_method_json(url, obj, function (data) {
        $(ctrl).autocomplete({
            minLength: 3,
            source: data,
            close: function (event, ui) {
                if (typeof (func) == "function")
                    func();
            }
        });
        if (key_val != null)
            $(ctrl).autocomplete("search", key_val);
    });
}

$.fn.InitialAutoComplete = function (url, func, highlight, maxheight) {
    $(this).attr("autocomplete-url", url);
    $(this).attr("autocomplete-hightlight", (highlight ? true : false));
    $(this).attr("autocomplete-maxheight", (maxheight ? maxheight : "300px"));
    
    var options = {
        minLength: 3,
        source: [],
        close: function (event, ui) {
            if (typeof (func) == "function")
                func();
        }
    };
                
    $(this).autocomplete(options);

    var SearchData = function(ctrl, keyword) {
        var obj = {
            cond: keyword
        };
        var searchUrl = ctrl.attr("autocomplete-url");
        call_ajax_method_json(searchUrl, obj, function (data) {
            if ($(ctrl).attr("autocomplete-hightlight") == "true")
            {
                $(ctrl).data("autocomplete")._renderItem = function (ul, item) {
                    var idx = 0
                    var keyword = ctrl.val().toUpperCase();
                    var display = item.label;
                    var boldStart = "<b style='color: blue'>";
                    var boldEnd = "</b>";

                    idx = display.toUpperCase().indexOf(keyword, idx);
                    while(idx > -1) {
                        display = display.substr(0, idx) + boldStart + display.substr(idx, keyword.length) + boldEnd + display.substr(idx + keyword.length);
                        idx = display.toUpperCase().indexOf(keyword, idx + boldStart.length + boldEnd.length);
                    } 

                    return $("<li></li>")
                        .data("item.autocomplete", item)
                        .attr( "data-value", item.value )
                        .append(
                            $("<a></a>").append(display)
                        )
                        .appendTo(ul);
                };
                $(ctrl).data("autocomplete")._renderMenu = function(ul, items ) {
                    var that = this;
                    $.each(items, function( index, item ) {
                        that._renderItem( ul, item );
                    });

                    var maxheight = $(ctrl).attr("autocomplete-maxheight");
                    $(ul).css("max-height", (maxheight ? maxheight : "300px"))
                        .css("overflow-y", "scroll");
                };
            }

            ctrl.autocomplete({ source: data });
            ctrl.autocomplete("search", ctrl.val());
        });
    };

    $(this).keyup(function (e) {
        // Escape for [Up key],[Down key],[Left key],[Right key],[Enter key],[Alt key]
        if ((e.which == 38) // Up
            || (e.which == 40) // Down
            || (e.which == 37) // Left
            || (e.which == 39) // Right
            || (e.which == 13) // Enter 
            || (e.which == 16) // Shift
            || (e.which == 17) // Ctrl
            || (e.which == 18) // Alt
            || (e.which == 19) // Pause
            || (e.which == 45) // Insert
            || (e.which == 91) // Windows
            || (e.which == 93) // Context-Menu
            || (e.which == 144) // Num-Lock
            || (e.which == 145) // Scroll-Lock
            || (e.which == 20) // Cap-Lock
            || (e.which == 27) // Esc
            || (e.which == 9) // Tab
        ) {
            return;
        }

        var ctrl = $(this);

        if (ctrl.val().length >= 3) {
            var keyword = ctrl.val();
            setTimeout(function() {
                if (keyword == ctrl.val() && ctrl.is(":focus"))
                {
                    SearchData(ctrl, keyword);
                }
            }, 500);
        }
    });
}