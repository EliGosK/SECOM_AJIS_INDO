function regenerate_combo(ctrl, data) {
    /// <summary>Method to generate Combo Box</summary>
    /// <param name="ctrl" type="string">Control Id (Select)</param>
    /// <param name="data" type="string">Data (must be ComboBoxModel)</param>

    //---Save Current Width ---
    var bwidth = $(ctrl).css("width").replace("px", "");
    bwidth = parseInt(bwidth) + 2;
    bwidth = bwidth + "px";

    //---Remove Option ---
    $(ctrl).children("option").remove();

    //--- Regenerate ---
    var opt = "";
    for (var idx = 0; idx < data.List.length; idx++) {
        var display = data.List[idx].Display;
        var val = data.List[idx].Value;
        opt += "<option value='" + val + "'>" + display + "</option>";
    }
    $(ctrl).html(opt);

    //--- Set Width
    $(ctrl).css("width", bwidth);
}

function initial_select_combo_list(ctrl_from, ctrl_to, btn_add, btn_remove) {
    $(btn_add).click(function () {
        $(ctrl_from).children("option:selected").each(function () {
            var key = $(this).attr("value");
            var value = $(this).html();
            $(ctrl_to).append($("<option></option>").attr("value", key).text(value));
            $(this).remove();
        });
    });
    $(btn_remove).click(function () {
        if ($(ctrl_to).children().length > 0) {
            $(ctrl_to).children("option:selected").each(function () {
                var key = $(this).attr("value");
                var value = $(this).html();
                $(ctrl_from).append($("<option></option>").attr("value", key).text(value));
                $(this).remove();
            });
            $(ctrl_from).children("option").sortElements(function (a, b) {
                return $(a).text() > $(b).text() ? 1 : -1;
            });
        }

    });
}

$.fn.initial_link_list = function (func) {
    $(this).find("a").initial_link(func);
}

// Nattapong N.  add plugin for sort element 18/11/2011
//Credit : http://james.padolsey.com/javascript/sorting-elements-with-jquery/
jQuery.fn.sortElements = (function () {

    var sort = [].sort;

    return function (comparator, getSortable) {

        getSortable = getSortable || function () { return this; };

        var placements = this.map(function () {

            var sortElement = getSortable.call(this),
                parentNode = sortElement.parentNode,

            // Since the element itself will change position, we have
            // to have some way of storing its original position in
            // the DOM. The easiest way is to have a 'flag' node:
                nextSibling = parentNode.insertBefore(
                    document.createTextNode(''),
                    sortElement.nextSibling
                );

            return function () {

                if (parentNode === this) {
                    throw new Error(
                        "You can't sort elements if any one is a descendant of another."
                    );
                }

                // Insert before flag:
                parentNode.insertBefore(this, nextSibling);
                // Remove flag:
                parentNode.removeChild(nextSibling);

            };

        });

        return sort.call(this, comparator).each(function (i) {
            placements[i].call(getSortable.call(this));
        });

    };

})();