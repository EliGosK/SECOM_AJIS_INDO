
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxgridcell.js" /> 
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js" />
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxgrid.js" />
/// <reference path="../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_data.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js"/>

/// <reference path="../number-functions.js" />

function eXcell_ron2(cell) {

    this.cell = cell;
    this.grid = this.cell.parentNode.grid;
    this.edit = function () { };
    this.isDisabled = function () {
        return true
    };
    this.getValue = function () {
        if (this.cell._clearCell) {
            return "";
        }
        return this.getAttribute("numbervalue");
    };
    this.setValue = function (val) {
        if (val === 0) {
        }
        else if (!val || val.toString()._dhx_trim() == "") {
            this.setCValue("&nbsp;");
            return this.cell._clearCell = true
        };

        if (val) {
            var numformat = this.numberFormat() || "";
            var newVal = (new Number(val)).numberFormat(numformat);
            this.setAttribute("numbervalue", val);
            this.setCValue(newVal, val);
        }
        else {
            this.setCValue("0", val);
        }
        this.cell._clearCell = false;
    };
    this.numberFormat = function () {
        // 2016.11.22 modify nakajima start
        if (this.grid._numberformat && this.grid._numberformat[this.cell._cellIndex]) {
            return this.grid._numberformat[this.cell._cellIndex];
        }
        
        //if (this.grid._numberformat && this.grid._numberformat[this.cell.getAttribute("_cellIndex")]) {
        //    return this.grid._numberformat[this.cell.getAttribute("_cellIndex")];
        //}
        // 2016.11.22 modify nakajima end
    }
};
eXcell_ron2.prototype = new eXcell;
