/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxgrid.js" />
/// <reference path="../../Scripts/jss.js" />

/* --- Initial Grid ------------------------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */

function initMenu(fMenuClick) {
    menu = new dhtmlXMenuObject();
    menu.setIconsPath("../../Content/js/dhtmlxMenu/codebase/imgs/dhxmenu_dhx_skyblue/");
    menu.renderAsContextMenu();
    //menu.setOpenMode("web");
    if (fMenuClick != null)
        menu.attachEvent("onClick", fMenuClick);
    menu.loadXML("../../Content/js/dhtmlxgrid/_context.xml");



    return menu;
}

function gridMenuClick(pGrid, menuitemId, type) {
    //alert('menu click');
    var text = "";
    var c, r;
    var temp = "";
    var rid;
    if (menuitemId == "cell") {
        text = pGrid.cells(pGrid.getSelectedId(), pGrid.getSelectedCellIndex()).getTitle();
        //pGrid.cellToClipboard();
    }
    else if (menuitemId == "row") {

        rid = pGrid.getSelectedId();
        for (c = 0; c < pGrid.getColumnsNum(); c++) {
            temp = pGrid.cells(rid, c).getTitle();
            //check sub grid group
            if (temp == "click to expand|collapse") {
                temp = "+";
            }
            text += temp + "\t";
        }

        //pGrid.setCSVDelimiter("\t");
        //pGrid.rowToClipboard();
    }
    else if (menuitemId == "grid") {
        for (r = 0; r < pGrid.getRowsNum(); r++) {
            rid = pGrid.getRowId(r);
            for (c = 0; c < pGrid.getColumnsNum(); c++) {

                temp = pGrid.cells(rid, c).getTitle();
                //check sub grid group
                if (temp == "click to expand|collapse") {
                    temp = "+";
                }
                text += temp + "\t";
            }
            text += "\n";
        }
        //pGrid.setCSVDelimiter("\t");
        //pGrid.gridToClipboard();
    }
    if(text != undefined) //Modify by Atipan C. 23 April 2012
        window.clipboardData.setData("text", text);
}

function CreateGridControl(name, page_num, show_number, fix_number) {
    /// <summary>Method to Create Grid</summary>
    /// <param name="name" type="string">Control name</param>
    /// <param name="page_num" type="int">Page Number</param>
    /// <param name="show_number" type="bool">Show Number Flag</param>
    /// <param name="fix_number" type="bool">Fix Number Flag</param>

    /* --- Create Tag --- */
    /* ------------------ */
    var grid_name = name + "_grid";
    var paging_name = name + "_paging";
    var paging_info_name = name + "_info_paging";

    $("#" + name).append("<div id='" + grid_name + "'style='width:100%;'></div>");
    if (page_num > 0) {
        $("#" + name).append("<div id='" + paging_name + "'style='width:100%;margin-left:1px;'></div>");
        $("#" + name).append("<div><div id='" + paging_info_name + "' style='float:left;margin:-31px 0 0 20px;background-color:#FFFFFF;padding:0 5px 0 5px;font-family: Verdana, MS UI Gothic;font-size:10pt;'></div></div>");
    }
    /* ------------------ */

    /* --- Create Grid object --- */
    /* -------------------------- */

    //    function onButtonClick(menuitemId, type) {
    //        var data = mygrid.contextID.split("_");
    //        //rowId_colInd;
    //        mygrid.setRowTextStyle(data[0], "color:" + menuitemId.split("_")[1]);
    //        return true;
    //    }
    //    menu = new dhtmlXMenuObject();
    //    //menu.setIconsPath("../common/images/");
    //    menu.renderAsContextMenu();
    //    menu.attachEvent("onClick", onButtonClick);
    //    menu.loadXML("../../Content/js/dhtmlxgrid/_context.xml");

    var mygrid = new dhtmlXGridObject(grid_name);
    mygrid.setImagePath("../../Content/js/dhtmlxgrid/imgs/");
    mygrid.enableAutoHeight(true);
    mygrid.enableCollSpan(true);
    mygrid.setEditable(false);
    mygrid.setSkin("dhx_secom");
    var menu = initMenu(function (menuitemId, type) {
        gridMenuClick(mygrid, menuitemId, type);
        return true;
    })
    mygrid.enableContextMenu(menu);
    mygrid.attachEvent("onBeforeContextMenu", function (rowId, celInd, obj) {
        //alert(rowId);
        //mygrid.selectRow(mygrid.getRowIndex(rowId));
        mygrid.selectCell(mygrid.getRowIndex(rowId), celInd);

        if (mygrid.getSelectedId() == null)
            return false;
        return true;
    });

    mygrid.init();
    mygrid.enableResizing("false");

    if (page_num > 0) {
        mygrid.enablePaging(true, page_num, 10, paging_name, true);
        mygrid.setPagingSkin("bricks");

        mygrid.attachEvent("onPageChanged", function (ind, fInd, lInd) {
            if (show_number == true
                && CheckFirstRowIsEmpty(mygrid) == false) {
                var rowCount = mygrid.getRowsNum() - ((ind - 1) * page_num);
                var rid = null;
                for (var i = 0; i < page_num && i < rowCount; i++) {
                    rid = mygrid.getRowId(i + fInd);

                    if (fix_number == true) {
                        mygrid.cells(rid, 0).setValue(i + 1);
                    }
                    else {
                        mygrid.cells(rid, 0).setValue(i + fInd + 1);
                    }
                }
            }

            var total = Math.ceil(mygrid.rowsBuffer.length / mygrid.rowsBufferOutSize);
            $("#" + paging_info_name).html("Page " + ind + " of " + total + " (" + mygrid.rowsBuffer.length + " items)");
        });
        mygrid.attachEvent("onAfterSorting", function (index, type, direction) {
            var ind = mygrid.currentPage;

            if (show_number == true
                && CheckFirstRowIsEmpty(mygrid) == false) {
                var fInd = (ind - 1) * page_num;
                var rowCount = mygrid.getRowsNum() - fInd;

                for (var i = 0; i < page_num && i < rowCount; i++) {
                    var rid = mygrid.getRowId(i + fInd);

                    if (fix_number == true) {
                        mygrid.cells(rid, 0).setValue(i + 1);
                    }
                    else {
                        mygrid.cells(rid, 0).setValue(i + fInd + 1);
                    }
                }
            }

            var total = Math.ceil(mygrid.rowsBuffer.length / mygrid.rowsBufferOutSize);
            $("#" + paging_info_name).html("Page " + ind + " of " + total + " (" + mygrid.rowsBuffer.length + " items)");
        });

        $("#" + paging_name).hide();
        $("#" + paging_info_name).hide();
    }
    /* -------------------------- */

    //R2 - Fix bug prevent text selection on textbox inside the grid.
    mygrid.entBox.onselectstart = function () {
        return (window.event.srcElement.tagName == "INPUT" || window.event.srcElement.tagName == "TEXTAREA");
    };

    return mygrid;
}
function SetPagingSection(name, grid, page_num, show_number) {
    /// <summary>Method to Set Paging section</summary>
    /// <param name="name" type="string">Control name</param>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="page_num" type="int">Page Number</param>
    /// <param name="show_number" type="bool">Show Running Number Flag</param>

    var grid_name = name + "_grid";
    var paging_name = name + "_paging";
    var paging_info_name = name + "_info_paging";

    if (page_num > 0) {
        $("#" + paging_name).show();
        $("#" + paging_info_name).show();

        var total = Math.ceil(grid.rowsBuffer.length / grid.rowsBufferOutSize);
        $("#" + paging_info_name).html("Page 1 of " + total + " (" + grid.rowsBuffer.length + " items)");
    }
    if (show_number
        && CheckFirstRowIsEmpty(grid) == false) {
        var total = grid.getRowsNum();
        if (total > page_num && page_num > 0) {
            total = page_num;
        }

        for (var i = 0; i < total; i++) {
            grid.cells2(i, 0).setValue(i + 1);
        }
    }
}
function CreateEmptyRow(grid, show_error) {
    /// <summary>Method to Create Empty Row</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="show_error" type="bool">Show Error Message</param>

    if (grid == undefined)
        return;

    var obj;
    try {

        // Narupon W. (26/Mar/2012) : for case delete all row
        if (grid._msgdata != undefined) {
            obj = grid._msgdata;
        }
        else {
            obj = jQuery.parseJSON(grid.getUserData("", "msgdata"));
        }
    }
    catch (err) {
        obj = new Object();
        obj.Code = "";
        obj.Message = "";
    }

    if (show_error)
        OpenErrorMessageDialog(obj.Code, obj.Message);

    // Natthavat S.
    // Modified Date: 26/03/2012
    // Fix check first row is empty attribute

    var rowEmptyID = '_empty_' + grid._OwnerId;
    grid.addRow(grid.uid(), ["<div style='float:left;' id=" + rowEmptyID + " mode='empty'>" + obj.Message + "</div>"], grid.getRowsNum());
    grid.setColspan(grid.getRowId(0), 0, grid.getColumnsNum());
    //grid.setRowAttribute(grid.getRowId(0), "mode", "empty");
    grid.setSizes();
}
$.fn.InitialGrid = function (page_num, fix_number, url, func) {
    /// <summary>Method to Initial Empty Grid</summary>
    /// <param name="page_num" type="int">Page Number</param>
    /// <param name="fix_number" type="bool">Fix Number Flag</param>
    /// <param name="url" type="string">/Controller/Action</param>
    /// <param name="func" type="string">Callback function</param>

    var name = $(this).attr("id");
    var mygrid = CreateGridControl(name, page_num, fix_number, fix_number);

    /* --- Call Controller --- */
    /* ----------------------- */
    call_ajax_method_json(url, "", function (result) {

        // Added by Non A. 19/Jul/2012 : Required for celltype=ron2
        ron2_SetUpNumberFormat(mygrid, result); 

        mygrid.xml.top = "response";
        mygrid.xml.row = "./rows/object";
        mygrid.parse(result);

        // Narupon (for case in delete all row)
        mygrid._msgdata = jQuery.parseJSON(mygrid.getUserData("", "msgdata"));
        mygrid._OwnerId = name;

        CreateEmptyRow(mygrid);

        // --- Add by Narupon W.----
        if (typeof (func) == "function")
            func(result);
    });
    /* ----------------------- */

    return mygrid;
}
$.fn.LoadDataToGrid = function (grid, page_num, show_number, url, obj, obj_name, show_error, func, prefunc) {
    /// <summary>Method to Load data to Grid</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="page_num" type="int">Page Number</param>
    /// <param name="show_number" type="bool">Show Running Number Flag</param>
    /// <param name="url" type="string">/Controller/Action</param>
    /// <param name="obj" type="object">Parameter that send to controller</param>
    /// <param name="obj_name" type="string">Return Object Name</param>
    /// <param name="show_error" type="bool">Show Error Message</param>
    /// <param name="func" type="function">Function when draw grid complete</param>

    var name = $(this).attr("id");
    var paging_name = name + "_paging";
    var paging_info_name = name + "_info_paging";

    //    /* --- Hide Paging section --- */
    //    /* --------------------------- */
    //    if (page_num > 0) {
    //        $("#" + paging_name).hide();
    //        $("#" + paging_info_name).hide();
    //    }
    //    /* --------------------------- */

    /* --- Call Controller --- */
    /* ----------------------- */

    call_ajax_method_json(url, obj, function (result, controls, isWarnig) {
        if (isWarnig == undefined) {
            var isError = true;
            if (result != null || result != undefined) {
                try {

                    // Fixing paging problem.
                    // Add new step "Pre Binding" function
                    if (typeof (prefunc) == "function")
                        prefunc(result, controls, isWarnig);

                    // Added by Non A. 19/Jul/2012 : Required for celltype=ron2
                    ron2_SetUpNumberFormat(grid, result); 

                    grid.xml.top = "response";
                    grid.xml.row = "./rows/" + obj_name;
                    grid.parse(result, "xmlB");

                    // Narupon (for case in delete all row)
                    grid._msgdata = jQuery.parseJSON(grid.getUserData("", "msgdata"));

                    isError = false;
                }
                catch (err) {
                    isError = true;
                }
            }

            if (grid.getRowsNum() == 0) {
                CreateEmptyRow(grid, show_error);
                $("#" + paging_name).hide();
                $("#" + paging_info_name).hide();
            }
            else if (isError == false) {
                SetPagingSection(name, grid, page_num, show_number);
            }
            grid.setSizes();
        }
        if (typeof (func) == "function")
            func(result, controls, isWarnig);
    });
    /* ----------------------- */
}
$.fn.LoadDataToGridWithInitial = function (page_num, fix_number, show_number, url, obj, obj_name, show_error, skip_func, prefunc, func) { //Add (func) by Jutarat A. on 09012014
    /// <summary>Method to Initial/Load data to Grid</summary>
    /// <param name="page_num" type="int">Page Number</param>
    /// <param name="fix_number" type="bool">Fix Number Flag</param>
    /// <param name="show_number" type="bool">Show Running Number Flag</param>
    /// <param name="url" type="string">/Controller/Action</param>
    /// <param name="obj" type="object">Parameter that send to controller</param>
    /// <param name="obj_name" type="string">Return Object Name</param>
    /// <param name="show_error" type="bool">Show Error Message</param>
    /// <param name="skip_func" type="bool">Set true if you want to skip function when found error</param>

    var name = $(this).attr("id");
    var mygrid = CreateGridControl(name, page_num, show_number, fix_number);

    /* --- Call Controller --- */
    /* ----------------------- */
    call_ajax_method_json(url, obj, function (result, controls, isWarnig) {
        var isError = true;
        if (result != null || result != undefined) {
            try {
                if (typeof (prefunc) == "function")
                    prefunc(result, controls, isWarnig);

                // Added by Non A. 19/Jul/2012 : Required for celltype=ron2
                ron2_SetUpNumberFormat(mygrid, result); 

                mygrid.xml.top = "response";
                mygrid.xml.row = "./rows/" + obj_name;
                mygrid.parse(result, "xmlB");
                isError = false;
            }
            catch (err) {
                isError = true;
            }
        }

        // Narupon (for case in delete all row)
        mygrid._msgdata = jQuery.parseJSON(mygrid.getUserData("", "msgdata"));
        mygrid._OwnerId = name;

        if (mygrid.getRowsNum() == 0) {
            CreateEmptyRow(mygrid, show_error);
        }
        else if (isError == false) {
            SetPagingSection(name, mygrid, page_num, show_number);
        }

        //Add by Jutarat A. on 09012014
        if (typeof (func) == "function")
            func(result, controls, isWarnig);
        //End Add

    }, skip_func);
    /* ----------------------- */

    return mygrid;
}

/* ----------------------------------------------------------------------------------- */





/* --- Grid Event -------------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */

function BindOnLoadedEvent(grid, func) {
    /// <summary>Method to bind on Loaded event</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="func" type="function">Function when grid Loaded</param>

    var drawFunc = function (gen_ctrl) {
        if (CheckFirstRowIsEmpty(grid) == false) {
            if (typeof (func) == "function")
                func(gen_ctrl);
        }
    };

    grid.attachEvent("onXLE", function () {
        drawFunc(true);
    });
    grid.attachEvent("onPageChanged", function (ind, fInd, lInd) {
        drawFunc(false);
    });

    /* --- Merge --- */
    grid.attachEvent("onAfterSorting", function (index, type, direction) {
        drawFunc(false);
    });
    /* ------------- */
}


var CurrentSortColIndex;
var CurrentSortType;
function BindOnLoadedEventV2(grid, page_num, show_number, fix_number, func) {
    /// <summary>Method to bind on Loaded event</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="page_num" type="int">Page Number</param>
    /// <param name="show_number" type="bool">Show Running Number Flag</param>
    /// <param name="fix_number" type="bool">Fix Number Flag</param>
    /// <param name="func" type="function">Function when grid Loaded</param>


    var funcLoopCurrentRows = function (ind, fInd) {
        if (CheckFirstRowIsEmpty(grid) == true)
            return null;

        var rowCount = grid.getRowsNum() - ((ind - 1) * page_num);
        var rid = null;
        for (var i = 0; (i < page_num || page_num == 0) && i < rowCount; i++) {
            rid = grid.getRowId(i + fInd);

            if (show_number == true) {
                if (fix_number == true) {
                    grid.cells(rid, 0).setValue(i + 1);
                }
                else {
                    grid.cells(rid, 0).setValue(i + fInd + 1);
                }
            }

            if (typeof (func) == "function") {
                func(rid);
            }
        }
        grid.setSizes();

        if (page_num > 0) {
            var paging_info_name = grid._OwnerId + "_info_paging";

            var total = Math.ceil(grid.rowsBuffer.length / grid.rowsBufferOutSize);
            $("#" + paging_info_name).html("Page 1 of " + total + " (" + grid.rowsBuffer.length + " items)");
        }

        return rid;
    }
    grid.attachEvent("onXLE", function () {
        funcLoopCurrentRows(1, 0);
    });
    grid.attachEvent("onPageChanged", function (ind, fInd, lInd) {
        var rid = funcLoopCurrentRows(ind, fInd);
//        if (rid != undefined) {
//            master_event.ScrollWindow("tr[idd='" + rid + "']", false, true);
//        }
    });

    /* --- Merge --- */
    grid.attachEvent("onAfterSorting", function (index, type, direction) {
        var ind = grid.currentPage;
        var fInd = (ind - 1) * page_num;
        funcLoopCurrentRows(ind, fInd);

        CurrentSortColIndex = index;
        CurrentSortType = direction;
    });
    /* ------------- */
}




function SpecialGridControl(grid, lst_col) {
    /// <summary>Method to set column that use custom control</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="lst_col" type="Array">List of column that has custom control</param>

    grid.attachEvent("onMouseOver", function (id, ind) {
        for (var i = 0; i < lst_col.length; i++) {
            if (ind == grid.getColIndexById(lst_col[i]))
                return false;
        }
        return true;
    });
}
/* ----------------------------------------------------------------------------------- */





/* --- Row Manipulate ---------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */

function CheckFirstRowIsEmpty(grid, clear) {
    /// <summary>Method to check row is empty</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="clear" type="bool">Clear Flag</param>

    var row_num = grid.getRowsNum();
    if (row_num < 1) {
        return true;
    }
    if (row_num == 1) {
        var rowEmptyID = '_empty_' + grid._OwnerId;
        var emptyRowAttr = $('#' + rowEmptyID).attr('mode');

        // Natthavat S.
        // Modified Date: 26/03/2012
        // Fix check first row is empty attribute
            
        //var attr = grid.getRowAttribute(grid.getRowId(0), "mode");
        //if (attr == "empty") {
        //    if (clear == true) {
        //        grid.deleteRow(grid.getRowId(0));
        //        grid.setSizes();
        //    }
        //    return true;
        //}

        if (emptyRowAttr == 'empty') {
            if (clear == true) {
                grid.deleteRow(grid.getRowId(0));
                grid.setSizes();
            }
            return true;
        }
    }
    return false;
}
function AddNewRow(grid, data) {
    /// <summary>Method to add new row to grid</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="data" type="Array">Data in each column</param>

    /* --- Merge --- */
    /* grid.addRow(grid.uid(),
                    data,
                    grid.getRowsNum());
    grid.setSizes(); */
    if (data != undefined) {
        for (var i = 0; i < data.length; i++) {
            if (typeof (data[i]) == "string" && data[i] != undefined) {
                data[i] = ConvertSSH(data[i]);
            }
        }
    }

    grid.addRow(grid.uid(),
                    data,
                    grid.getRowsNum());
    grid.setSizes();
    /* ------------- */
}
function DeleteRow(grid, row_id) {
    /// <summary>Method to delete row</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="row_id" type="string">Row ID</param>

    grid.deleteRow(row_id);
    if (grid.getRowsNum() == 0) {
        CreateEmptyRow(grid);
    }
    grid.setSizes();
}
function DeleteAllRow(grid) {
    /// <summary>Method to delete all row in grid</summary>
    /// <param name="grid" type="object">Grid object</param>

    // ---- comment by Narupon W.----

    //var idlst = new Array();
    //for (var i = 0; i < grid.getRowsNum(); i++) {
    //    idlst.push(grid.getRowId(i));
    //}
    //for (var i = 0; i < idlst.length; i++) {
    //    grid.deleteRow(idlst[i]);
    //}

    // Narupon (26 Mar 2012) : in case delete all row
    grid.clearAll();

    var paging_name = grid._OwnerId + "_paging";
    var paging_info_name = grid._OwnerId + "_info_paging";

    $("#" + paging_name).hide();
    $("#" + paging_info_name).hide();

    CreateEmptyRow(grid);
    grid.setSizes();
}

/* ----------------------------------------------------------------------------------- */





/* --- Control in Grid --------------------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */

function GenerateGridControlID(id, row_id) {
    /// <summary>Method to create control id</summary>
    /// <param name="id" type="string">Control's id</param>
    /// <param name="row_id" type="string">Grid row's id</param>

    var ctrl_id = id;
    if (row_id != "" && row_id != undefined)
        ctrl_id += "_" + row_id;

    return ctrl_id;
}
function GetGridRowIDFromControl(ctrl) {
    var rids = ctrl.id.split("_");
    return rids[rids.length - 1];
}
function GetValueFromLinkType(grid, row, col) {
    /// <summary>Method to get value from column in grid that are column's type = link</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="row" type="int">Row index</param>
    /// <param name="col" type="int">Column index</param>

    var val = grid.cells2(row, col).getValue();
    val = val.replace("^null", "");

    return val;
}
//========= Teerapong 06/02/2012 ==================
function SetFitColumnForBackAction(grid, tempColumnID) {
    var row_id, Colinx;
    for (var i = 0; i < grid.getRowsNum(); i++) {

        row_id = grid.getRowId(i);
        Colinx = grid.getColIndexById(tempColumnID);
        if (Colinx != undefined && !CheckFirstRowIsEmpty(grid)) {
            grid.setColspan(row_id, Colinx - 1, 2);
        }
    }
}
/* ----------------------------------------------------------------------------------- */





/* --- Control in Grid  (Image button) ----------------------------------------------- */
/* ----------------------------------------------------------------------------------- */

function BindGridButtonClickEvent(id, row_id, func) {
    /// <summary>Method to bind image button event</summary>
    /// <param name="id" type="string">Control's id</param>
    /// <param name="row_id" type="string">Grid row's id</param>
    /// <param name="func" type="function">Function when click [function(row_id)]</param>
    var ctrl = "#" + GenerateGridControlID(id, row_id);

    $(ctrl).unbind("click");
    $(ctrl).click(function () {
        if (this.className.indexOf("row-image-disabled") < 0) {
            if (typeof (func) == "function") {
                var rid = GetGridRowIDFromControl(this);
                func(rid);
            }
        }
        return false;
    });
}
function BindGridButtonClickEvent2(id, row_id, func) {
    /// <summary>Method to bind image button event</summary>
    /// <param name="id" type="string">Control's id</param>
    /// <param name="row_id" type="string">Grid row's id</param>
    /// <param name="func" type="function">Function when click [function(row_id)]</param>
    var ctrl = "#" + GenerateGridControlID(id, row_id);

    $(ctrl).unbind("click");
    $(ctrl).click(function () {
        if (this.className.indexOf("row-image-disabled") < 0) {
            if (typeof (func) == "function") {
                var rid = GetGridRowIDFromControl(this);
                func(rid, $(this));
            }
        }
        return false;
    });
}

function BindGridHtmlButtonClickEvent(id, row_id, func) {
    /// <summary>Method to bind image button event</summary>
    /// <param name="id" type="string">Control's id</param>
    /// <param name="row_id" type="string">Grid row's id</param>
    /// <param name="func" type="function">Function when click [function(row_id)]</param>
    var ctrl = "#" + GenerateGridControlID(id, row_id);

    $(ctrl).unbind("click");
    $(ctrl).click(function () {
        if (typeof (func) == "function") {
            var rid = GetGridRowIDFromControl(this);
            func(rid);
        }
        return false;
    });
}

function EnableGridButton(grid, id, row_id, col_id, enable) {
    /// <summary>Method to Enable/Disable image button event</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="id" type="string">Control's id</param>
    /// <param name="row_id" type="string">Grid row's id</param>
    /// <param name="col_id" type="string">Column id</param>
    /// <param name="enable" type="bool">enable flag</param>

    var ctrl = $("#" + GenerateGridControlID(id, row_id));
    if (ctrl.length > 0) {
        if (enable) {
            var col = grid.getColIndexById(col_id);

            var defDisabled = grid.cells(row_id, col).getAttribute("defDisabled");
            if (defDisabled != "1") {
                var title = grid.cells(row_id, col).getAttribute("defToolTip");

                ctrl.attr("class", "row-image");
                ctrl.attr("title", title);
            }
        }
        else {
            ctrl.attr("class", "row-image-disabled");
            ctrl.attr("title", "");
        }
    }
}

function GenerateImageButtonToGrid(grid, id, row_id, col_id, enable, src, def_title) {
    /// <summary>Method to create Image button and set to grid</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="col_id" type="string">Column id</param>
    /// <param name="enable" type="bool">enable flag</param>
    /// <param name="src" type="string">Image name</param>
    /// <param name="def_title" type="string">Default Tooltip</param>

    var col = grid.getColIndexById(col_id);
    var title = grid.getColumnLabel(col, 0);
    if (title == undefined || title == "") {
        title = def_title
    }

    var img = GenerateImageButton(id, row_id, enable, src, title);
    grid.cells(row_id, col).setValue(img);

    /* --- Set Default Button Mode --- */
    /* ------------------------------- */
    var defDisabled = "0";
    if (enable == false)
        defDisabled = "1";
    grid.cells(row_id, col).setAttribute("defDisabled", defDisabled);
    /* ------------------------------- */

    /* --- Set ToolTip --- */
    /* ------------------- */
    grid.cells(row_id, col).setAttribute("defToolTip", title);
    /* ------------------- */
}
function GenerateImageButton(id, row_id, enable, src, title) {
    /// <summary>Method to create Image button</summary>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="enable" type="bool">enable flag</param>
    /// <param name="src" type="string">Image name</param>
    /// <param name="title" type="string">Tooltip</param>

    var titile_txt = title;
    var class_disabled = "row-image";
    if (enable == false) {
        titile_txt = "";
        class_disabled = "row-image-disabled";
    }

    var fid = GenerateGridControlID(id, row_id);
    return "<img title='" + titile_txt + "' id='" + fid + "' class='" + class_disabled + "' src='../../Content/images/icon/" + src + "' width='20px' height='20px' style='margin-top:-1px;'>";
}

function GenerateEditButton(grid, id, row_id, col_id, enable) {
    /// <summary>Method to create Edit button</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="col_id" type="string">Column id</param>
    /// <param name="enable" type="bool">enable flag</param>

    GenerateImageButtonToGrid(grid, id, row_id, col_id, enable, "edit.png", "Edit");
}
function GenerateDetailButton(grid, id, row_id, col_id, enable) {
    /// <summary>Method to create Detail button</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="col_id" type="string">Column id</param>
    /// <param name="enable" type="bool">enable flag</param>

    GenerateImageButtonToGrid(grid, id, row_id, col_id, enable, "detail.png", "Detail");
}
function GenerateRemoveButton(grid, id, row_id, col_id, enable) {
    /// <summary>Method to create Remove button</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="col_id" type="string">Column id</param>
    /// <param name="enable" type="bool">enable flag</param>

    GenerateImageButtonToGrid(grid, id, row_id, col_id, enable, "remove.png", "Remove");
}
function GenerateSelectButton(grid, id, row_id, col_id, enable) {
    /// <summary>Method to create Remove button</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="col_id" type="string">Column id</param>
    /// <param name="enable" type="bool">enable flag</param>

    GenerateImageButtonToGrid(grid, id, row_id, col_id, enable, "select.png", "Select");
}
function GenerateAddButton(grid, id, row_id, col_id, enable) {
    /// <summary>Method to create Add button</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="col_id" type="string">Column id</param>
    /// <param name="enable" type="bool">enable flag</param>

    GenerateImageButtonToGrid(grid, id, row_id, col_id, enable, "add.png", "Add");
}
function GenerateRunningButton(grid, id, row_id, col_id, enable) {
    /// <summary>Method to create Running button</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="col_id" type="string">Column id</param>
    /// <param name="enable" type="bool">enable flag</param>

    GenerateImageButtonToGrid(grid, id, row_id, col_id, enable, "running.png", "Running");
}
function GenerateDownloadButton(grid, id, row_id, col_id, enable) {
    /// <summary>Method to create Running button</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="col_id" type="string">Column id</param>
    /// <param name="enable" type="bool">enable flag</param>

    GenerateImageButtonToGrid(grid, id, row_id, col_id, enable, "download.png", "Download");
}
function GeneratePrinterButton(grid, id, row_id, col_id, enable) {
    /// <summary>Method to create Running button</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="col_id" type="string">Column id</param>
    /// <param name="enable" type="bool">enable flag</param>

    GenerateImageButtonToGrid(grid, id, row_id, col_id, enable, "printer.png", "Printer");
}
function GenerateCreditNoteButton(grid, id, row_id, col_id, enable) {
    /// <summary>Method to create Running button</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="col_id" type="string">Column id</param>
    /// <param name="enable" type="bool">enable flag</param>

    GenerateImageButtonToGrid(grid, id, row_id, col_id, enable, "creditNote.png", "Credit note");
}
function GenerateFirstFeeButton(grid, id, row_id, col_id, enable) {
    /// <summary>Method to create Running button</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="col_id" type="string">Column id</param>
    /// <param name="enable" type="bool">enable flag</param>

    GenerateImageButtonToGrid(grid, id, row_id, col_id, enable, "firstFee.PNG", "First fee");
}
// Akat K. : for Inventory
function GenerateCalculateButton(grid, id, row_id, col_id, enable) {
    var title = C_CALCULATE_TITLE;
    if (title == undefined || title == "") {
        col_title = grid.getColIndexById(col_id);
        title = grid.getColumnLabel(col_title, 0);
        if (title == undefined || title == "") {
            title = "Cal"
        }
    }

    var col = grid.getColIndexById(col_id);
    var img = GenerateImageButton(id, row_id, enable, "calculator.png", title);
    grid.cells(row_id, col).setValue(img);

    /* --- Set Default Button Mode --- */
    /* ------------------------------- */
    var defDisabled = "0";
    if (enable == false)
        defDisabled = "1";
    grid.cells(row_id, col).setAttribute("defDisabled", defDisabled);
    /* ------------------------------- */

    /* --- Set ToolTip --- */
    /* ------------------- */
    grid.cells(row_id, col).setAttribute("defToolTip", title);
    /* ------------------- */
}


function GenerateStarImage() {
    /// <summary>Method to create Remove button</summary>
    return "<img src='../../Content/images/icon/star.png' style='margin-top:0px;' />";
}

/* ----------------------------------------------------------------------------------- */
/* --- Control in Grid  (Text button) ------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */

function GenerateNumericBox(id, row_id, value, enable) {
    /// <summary>Method to create Numeric box</summary>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="value" type="string">value</param>
    /// <param name="enable" type="bool">enable flag</param>
    var disabled_txt = "";
    if (enable == false)
        disabled_txt = "readonly='readonly'";

    var fid = GenerateGridControlID(id, row_id);
    return "<input class='numeric-box' id='" + fid + "' name='" + fid + "' style='width:100%;margin-left:-2px;' type='text' value='" + value + "' " + disabled_txt + " />";
}
function GenerateNumericBox2(grid, id, row_id, col_id, value, before, dec, min, max, defaultmin, enable, width) {
    var disabled_txt = "";
    if (enable == false)
        disabled_txt = "readonly='readonly'";
    if (!width) {
        width = "100%";
    }

    var col = grid.getColIndexById(col_id);
    var fid = GenerateGridControlID(id, row_id);

    var txt = "<input class='numeric-box' id='" + fid + "' name='" + fid + "' style='width:" + width + ";margin-left:-2px;' type='text' value='" + value + "' " + disabled_txt + " />";
    grid.cells(row_id, col).setValue(txt);

    fid = "#" + fid;
    $(fid).parent().parent().css({ "text-overflow": "clip" });
    $(fid).BindNumericBox(before, dec, min, max, defaultmin);
    $(fid).setComma();

    $(fid).focus(function () {
        grid.selectRowById(row_id);
    });
}
// Add by Jirawat Jannet on 2016-10-27
// Initial numeric textbox with currency textbox


function GenerateNumericBoxWithUnit(id, row_id, value, width, unit, require, enable, require_space) {
    /// <summary>Method to create Numeric box</summary>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="value" type="string">value</param>
    /// <param name="width" type="string">text width</param>
    /// <param name="unit" type="string">unit text</param>
    /// <param name="require" type="bool">require flag</param>
    /// <param name="enable" type="bool">enable flag</param>
    var disabled_txt = "";
    if (enable == false)
        disabled_txt = "readonly='readonly'";

    var fid = GenerateGridControlID(id, row_id);
    var ctrl = "<div class='object-unit'><input class='numeric-box' id='" + fid + "' name='" + fid + "' style='width:" + width + ";margin-left:-2px;' type='text' value='" + value + "' " + disabled_txt + " /></div>";
    if (unit != undefined) {
        ctrl = ctrl + "<div id='unit" + fid + "' class='label-unit'>" + unit + "</div>";
    }
    if (require == true) {
        ctrl = ctrl + "<div class='label-remark' style='float:left;margin-left:5px;'>*</div>";
    }
    else if (require_space == true) {
        ctrl = ctrl + "<div class='label-remark' style='float:left;margin-left:5px;width:9px;'>&nbsp;</div>";
    }

    return "<div style='float:right;'>" + ctrl + "</div>";
}

function GenerateNumericBoxWithUnit2(grid, id, row_id, col_id, value, before, dec, min, max, defaultmin, width, unit, require, enable) {
    /// <summary>Method to create Numeric box with Unit</summary>
    /// <param name="grid" type="string">Grid</param>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="col_id" type="string">Grid col' id</param>
    /// <param name="value" type="string">value</param>
    /// <param name="before" type="string">digit before point</param>
    /// <param name="dec" type="string">digit after point</param>
    /// <param name="min" type="string">min value</param>
    /// <param name="max" type="string">max value</param>
    /// <param name="defaultmin" type="string">show min value flag</param>
    /// <param name="width" type="string">text width</param>
    /// <param name="unit" type="string">unit text</param>
    /// <param name="require" type="bool">require flag</param>
    /// <param name="enable" type="bool">enable flag</param>

    var disabled_txt = "";
    if (enable == false)
        disabled_txt = "readonly='readonly'";

    var col = grid.getColIndexById(col_id);
    var fid = GenerateGridControlID(id, row_id);

    var txt = "<div class='object-unit'><input class='numeric-box' id='" + fid + "' name='" + fid + "' style='width:" + width + ";margin-left:-2px;' type='text' value='" + value + "' " + disabled_txt + " /></div>";
    if (unit != undefined) {
        txt = txt + "<div id='unit" + fid + "' class='label-unit'>" + unit + "</div>";
    }
    if (require == true) {
        txt = txt + "<div class='label-remark' style='float:left;margin-left:5px;'>*</div>";
    }

    txt = "<div style='float:right;'>" + txt + "</div>";

    grid.cells(row_id, col).setValue(txt);

    fid = "#" + fid;
    $(fid).parent().parent().css({ "text-overflow": "clip" });
    $(fid).BindNumericBox(before, dec, min, max, defaultmin);
    $(fid).setComma();

    $(fid).focus(function () {
        grid.selectRowById(row_id);
    });
}

function NumericTextBoxWithMultipleCurrency(grid, id, row_id, col_id, value, curr_value, before, dec, min, max, defaultmin, require, enable) {

    var col = grid.getColIndexById(col_id);
    var fid = GenerateGridControlID(id, row_id);
    
    var input = $("<input>")
                    .attr("type", "text")
                    .attr("id", fid)
                    .attr("name", fid)
                    .addClass("numeric-box")
                    .css({ "width": "140px" });
    if (enable == false)
        input.attr("readonly", true);
    
    var select = $("<select>")
                    .attr("id", fid + "CurrencyType")
                    .attr("name", fid + "CurrencyType");
    if (C_CURRENCY_LIST != undefined) {
        for (var idx = 0; idx < C_CURRENCY_LIST.length; idx++) {
            var opt = $("<option>")
                        .attr("value", C_CURRENCY_LIST[idx].ValueCode)
                        .text(C_CURRENCY_LIST[idx].ValueDisplayEN);
            if ((idx == 0 && curr_value == undefined)
                || (C_CURRENCY_LIST[idx].ValueCode == curr_value)) {
                opt.attr("selected", true);
            }
            
            select.append(opt);
        }
    }
    if (enable == false)
        select.attr("disabled", true)

    var combo = $("<div>")
                    .addClass("combo-unit")
                    .append(select);

    var unit = $("<div>")
                    .addClass("object-unit")
                    .append(input);

    if (require == true) {
        unit.append($("<div>")
                        .addClass("label-remark")
                        .text("*"));
    }

    grid.cells(row_id, col).setValue(combo[0].outerHTML + unit[0].outerHTML);
    
    fid = "#" + fid;
    $(fid).parent().parent().css({ "text-overflow": "clip" });
    $(fid).val(value);
    $(fid).BindNumericBox(before, dec, min, max, defaultmin);
    $(fid).setComma();

    $(fid).focus(function () {
        grid.selectRowById(row_id);
    });
}


function GenerateTextBox(grid, id, row_id, col_id, value, enable, maxlength) { //Modify (Add maxlength) by Jutarat A. on 28102013
    var disabled_txt = "";
    if (enable == false)
        disabled_txt = "readonly='readonly'";

    //Add by Jutarat A. on 28102013
    var maxlength_txt = "";
    if (maxlength != undefined && maxlength != null)
        maxlength_txt = "maxlength=" + maxlength.toString();
    //End Add

    var col = grid.getColIndexById(col_id);
    var fid = GenerateGridControlID(id, row_id);

    //var txt = "<input id='" + fid + "' name='" + fid + "' style='width:100%;margin-left:-2px;' type='text' value='" + value + "' " + disabled_txt + " />";
    var txt = "<input id='" + fid + "' name='" + fid + "' style='width:100%;margin-left:-2px;' type='text' value='" + value + "' " + disabled_txt + maxlength_txt + " />"; //Modify (Add maxlength) by Jutarat A. on 28102013
    grid.cells(row_id, col).setValue(txt);

    fid = "#" + fid;
    $(fid).parent().parent().css({ "text-overflow": "clip" });

    $(fid).focus(function () {
        grid.selectRowById(row_id);
    });
}

function GenerateHtmlButton(id, row_id, value, enable) {
    /// <summary>Method to create Html Button</summary>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="value" type="string">value</param>
    /// <param name="enable" type="bool">enable flag</param>
    var disabled_txt = "";
    if (enable == false)
        disabled_txt = "disabled='disabled'";

    var fid = GenerateGridControlID(id, row_id);
    return "<button id='" + fid + "' name='" + fid + "' style='width:100%;margin-left:-2px;'" + disabled_txt + " >" + value + "</button>";
}

/* ----------------------------------------------------------------------------------- */
/* --- Control in Grid  (Checkbox) --------------------------------------------------- */
/* ----------------------------------------------------------------------------------- */
function GenerateCheckBox(id, row_id, value, enable) {
    /// <summary>Method to create Numeric box</summary>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="value" type="string">value</param>
    /// <param name="enable" type="bool">enable flag</param>
    var disabled_txt = "";
    if (enable == false)
        disabled_txt = "disabled='disabled'";

    var fid = GenerateGridControlID(id, row_id);
    return "<input id='" + fid + "' name='" + fid + "' style='margin-left:-3px;' type='checkbox' value='" + value + "' " + disabled_txt + "/>";
}
function GenerateCheckBox2(id, row_id, value, enable, checked) {
    /// <summary>Method to create Numeric box</summary>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="value" type="string">value</param>
    /// <param name="enable" type="bool">enable flag</param>
    /// <param name="checked" type="bool">default checked flag</param>
    var disabled_txt = "";
    if (enable == false)
        disabled_txt = "disabled='disabled'";

    var fid = GenerateGridControlID(id, row_id);
    if (checked) {
        return "<input id='" + fid + "' name='" + fid + "' style='margin-left:-3px;' type='checkbox' value='" + value + "' " + disabled_txt + " checked/>";
    } else {
        return "<input id='" + fid + "' name='" + fid + "' style='margin-left:-3px;' type='checkbox' value='" + value + "' " + disabled_txt + "/>";
    }
}
function BindGridCheckBoxClickEvent(id, row_id, func) {
    /// <summary>Method to bind image button event</summary>
    /// <param name="id" type="string">Control's id</param>
    /// <param name="row_id" type="string">Grid row's id</param>
    /// <param name="func" type="function">Function when click [function(row_id)]</param>
    var ctrl = "#" + GenerateGridControlID(id, row_id);

    $(ctrl).unbind("click");
    $(ctrl).click(function () {
        if (typeof (func) == "function") {
            var rid = GetGridRowIDFromControl(this);
            var checked = $(ctrl).prop("checked")
            func(rid, checked);
        }
    });
}

/* ----------------------------------------------------------------------------------- */
/* --- Control in Grid  (RadioButton) ------------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function GenerateRadioButton(id, row_id, value, name, enable) {
    /// <summary>Method to create Numeric box</summary>
    /// <param name="id" type="string">control' id</param>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="value" type="string">value</param>
    /// <param name="name" type="string">Group name</param>
    /// <param name="enable" type="bool">enable flag</param>
    var disabled_txt = "";
    if (enable == false)
        disabled_txt = "disabled='disabled'";

    var fid = GenerateGridControlID(id, row_id);
    return "<input id='" + fid + "' name='" + name + "' style='width:97%;margin:2px 0 0 -2px;' type='radio' value='" + value + "' " + disabled_txt + "/>";
}

function BindGridRadioButtonClickEvent(id, row_id, func) {
    /// <summary>Method to bind image button event</summary>
    /// <param name="id" type="string">Control's id</param>
    /// <param name="row_id" type="string">Grid row's id</param>
    /// <param name="func" type="function">Function when click [function(row_id)]</param>
    var ctrl = "#" + GenerateGridControlID(id, row_id);

    $(ctrl).unbind("click");
    $(ctrl).click(function () {
        if (typeof (func) == "function") {
            var rid = GetGridRowIDFromControl(this);
            var checked = $(ctrl).prop("checked")
            func(rid, checked);
        }
    });
}


/* ----------------------------------------------------------------------------------- */
/* --- Control in Grid  (DateTimePicker) --------------------------------------------- */
/* --- Create by Narupon W. on Feb 23 ,2012 ------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function GenerateGridDateTimePicker(grid, row_id, id, col_id, value, enable, width) {
    /// <summary>Method to create datetime picker in grid</summary>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="id" type="string">control' id</param>
    /// <param name="col_id" type="string">column id</param>
    /// <param name="value" type="string">default value</param>
    /// <param name="enable" type="bool">enable flag</param>
    /// <param name="width" type="string">width of datetime picker</param>

    if (!width) {
        width = "80%";
    }

    var col = grid.getColIndexById(col_id);
    var fid = GenerateGridControlID(id, row_id);
    var txt = "<input  id='" + fid + "' name='" + fid + "' style='width:" + width + ";margin-left:-2px;' type='text'  />";
    grid.cells(row_id, col).setValue(txt);

    fid = "#" + fid;
    $(fid).InitialDate();

    // Set value
    $(fid).val(value);

    // Enable
    if (enable == false)
        $(fid).EnableDatePicker(false);
    else
        $(fid).EnableDatePicker(true);

    $(fid).focus(function () {
        grid.selectRowById(row_id);
    });
}


/* ----------------------------------------------------------------------------------- */
/* --- Control in Grid  (DateTimePicker From-To) ------------------------------------- */
/* --- Create by Narupon W. on Feb 23 ,2012 ------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function GenerateGridDateTimePickerFromTo(grid, row_id, id_from, col_id_from, value_from, id_to, col_id_to, value_to, enable, width1, width2) {
    /// <summary>Method to create datetime picker in grid</summary>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="id_from" type="string">control' id (From)</param>
    /// <param name="col_id_from" type="string">column id (From)</param>
    /// <param name="value_from" type="string">default value (From)</param>
    /// <param name="id_to" type="string">control' id (To)</param>
    /// <param name="col_id_to" type="string">column id (To)</param>
    /// <param name="value_to" type="string">default value (To)</param>
    /// <param name="enable" type="bool">enable flag</param>
    /// <param name="width1" type="string">width of datetime picker (from)</param>
    /// <param name="width2" type="bool">width of datetime picker (to)</param>

    if (!width1) {
        width1 = "80%";
    }
    if (!width2) {
        width2 = width1;
    }

    // from
    var col_from = grid.getColIndexById(col_id_from);
    var fid_from = GenerateGridControlID(id_from, row_id);
    var txt_from = "<input  id='" + fid_from + "' name='" + fid_from + "' style='width:" + width1 + ";margin-left:-2px;' type='text'  />";
    grid.cells(row_id, col_from).setValue(txt_from);

    // to
    var col_to = grid.getColIndexById(col_id_to);
    var fid_to = GenerateGridControlID(id_to, row_id);
    var txt_to = "<input  id='" + fid_to + "' name='" + fid_to + "' style='width:" + width2 + ";margin-left:-2px;' type='text'  />";
    grid.cells(row_id, col_to).setValue(txt_to);

    fid_from = "#" + fid_from;
    fid_to = "#" + fid_to;


    InitialDateFromToControl(fid_from, fid_to);

    // Setvalue
    $(fid_from).val(value_from);
    $(fid_to).val(value_to);

    // Enable
    if (enable == false) {
        $(fid_from).EnableDatePicker(false);
        $(fid_to).EnableDatePicker(false);
    }
    else {
        $(fid_from).EnableDatePicker(true);
        $(fid_to).EnableDatePicker(true);
    }


    $(fid_from).focus(function () {
        grid.selectRowById(row_id);
    });

    $(fid_to).focus(function () {
        grid.selectRowById(row_id);
    });
}


/* ----------------------------------------------------------------------------------- */
/* --- Control in Grid  (DateTimePicker From-To within single cell) ------------------ */
/* --- Create by Narupon W. on Feb 23 ,2012 ------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function GenerateGridDateTimePickerFromToSingleCell(grid, row_id, col_id, id_from, value_from, id_to, value_to, enable, width1, width2) {
    /// <summary>Method to create datetime picker in grid</summary>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="col_id" type="string">column id</param>
    /// <param name="id_from" type="string">control' id (From)</param>
    /// <param name="value_from" type="string">default value (From)</param>
    /// <param name="id_to" type="string">control' id (To)</param>
    /// <param name="value_to" type="string">default value (To)</param>
    /// <param name="enable" type="bool">enable flag</param>
    /// <param name="width1" type="string">width of datetime picker (from)</param>
    /// <param name="width2" type="bool">width of datetime picker (to)</param>

    if (!width1) {
        width1 = "90px";
    }
    if (!width2) {
        width2 = width1;
    }

    var id = "divGridDatePicker" + col_id + "_" + id_from + "_" + id_to;
    var col = grid.getColIndexById(col_id);

    // from
    var fid_from = GenerateGridControlID(id_from, row_id);
    var txt_from = "<div class='usr-object' style='padding-bottom: 5px'><input class='grid-datepicker-from' id='" + fid_from + "' name='" + fid_from + "' style='width:" + width1 + ";margin-left:-2px;' type='text'  /></div>";

    var fid_to = GenerateGridControlID(id_to, row_id);
    var txt_to = "<div class='usr-object'> - <input class='grid-datepicker-to' id='" + fid_to + "' name='" + fid_to + "' style='width:" + width2 + ";margin-left:-2px;' type='text'  /></div>";

    grid.cells(row_id, col).setValue("<div id='" + id + "' class='grid-datepicker-fromto'>" + txt_from + txt_to + "</div>");

    fid_from = "#" + fid_from;
    fid_to = "#" + fid_to;

    InitialDateFromToControl(fid_from, fid_to);

    // Setvalue
    $(fid_from).val(value_from);
    $(fid_to).val(value_to);

    // Enable
    if (enable == false) {
        $(fid_from).EnableDatePicker(false);
        $(fid_to).EnableDatePicker(false);
    }
    else {
        $(fid_from).EnableDatePicker(true);
        $(fid_to).EnableDatePicker(true);
    }


    $(fid_from).focus(function () {
        grid.selectRowById(row_id);
    });

    $(fid_to).focus(function () {
        grid.selectRowById(row_id);
    });
}

function ClearGridComboboxCache(grid, id) {
    /// <summary>Method to remove combobox's datalist cache</summary>
    /// <param name="grid" type="string">grid control</param>
    /// <param name="id" type="string">control' id</param>

    var dataListKey = "DataList_" + id;
    if (grid[dataListKey] != undefined) {
        delete grid[dataListKey];
    }
}

/* ----------------------------------------------------------------------------------- */
/* --- Control in Grid  (Combobox) --------------------------------------------------- */
/* --- Create by Narupon W. on Feb 23 ,2012 ------------------------------------------ */
/* ----------------------------------------------------------------------------------- */
function GenerateGridCombobox(grid, row_id, id, col_id, url, val, enable) {
    /// <summary>Method to create combobox in grid</summary>
    /// <param name="row_id" type="string">Grid row' id</param>
    /// <param name="id" type="string">control' id</param>
    /// <param name="col_id" type="string">column id</param>
    /// <param name="url" type="string">Url for get item of combobox</param>
    /// <param name="enable" type="bool">enable flag</param>

    var dataListKey = "DataList_" + id;
    var style = "width:99%;margin-left:-2px;";

    var col = grid.getColIndexById(col_id);
    var fid = GenerateGridControlID(id, row_id);
    var txt = "<select  id='" + fid + "' name='" + fid + "' style='" + style + "'></select>";
    grid.cells(row_id, col).setValue(txt);


    var obj = { "id": row_id };
    clt = "#" + fid;

    if (grid[dataListKey] != undefined) {
        var data = grid[dataListKey];
        regenerate_combo(clt, data);
    }
    else {

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
        if (url.indexOf("?") > 0) {
            url = url + "&ajax=1";
        }
        else {
            url = url + "?ajax=1";
        }

        var objJson = $.toJSON("");
        objJson = ajax_method.ConvertIncorrectText(objJson);

        var data = $.ajax({
            type: "POST"
                      , data: objJson
                      , dataType: 'json'
                      , url: ajax_method.GenerateURL(url)
                      , async: false
        }).responseText;

        data = JSON.parse(data);

        command_control.CommandControlMode(true);
        ajax_method.OnAjaxSendSuccess(data, function (x) {
            // Keep data
            grid[dataListKey] = x;
            regenerate_combo(clt, x);
        });

    }


    fid = "#" + fid;

    // set value
    $(fid).val(val);

    // enable
    if (enable == false)
        $(fid).attr("disabled", true);
    else
        $(fid).attr("disabled", false);

    $(fid).focus(function () {
        grid.selectRowById(row_id);
    });
}

function BindGridComboboxChengeEvent(id, row_id, func) {
    /// <summary>Method to bind combobox event</summary>
    /// <param name="id" type="string">Control's id</param>
    /// <param name="row_id" type="string">Grid row's id</param>
    /// <param name="func" type="function">Function when change [function(row_id)]</param>
    var ctrl = "#" + GenerateGridControlID(id, row_id);

    $(ctrl).unbind("change");
    $(ctrl).change(function () {
        if (typeof (func) == "function") {
            var rid = GetGridRowIDFromControl(this);
            func(rid);
        }
    });
}

var GridControl = {


    PrepareLockableGrid: function (grid, default_isLocked) {
        /// <summary>Plug-in grid for lock funciton.</summary>
        /// <param name="grid" type="dhtmlXGridObject">Control's id</param>
        /// <param name="default_is_locked" type="bool">default value for locking</param>
        grid._isLocked = default_isLocked;

        grid.attachEvent("onBeforeSelect", function (new_row, old_row) {
            return !this._isLocked;
        });
        grid.attachEvent("onBeforeSorting", function (ind, type, direction) {
            return !this._isLocked;
        });
        grid.attachEvent("onBeforePageChanged", function (ind, count) {
            return !this._isLocked;
        });
    },

    LockGrid: function (grid) {
        /// <summary>Method to bind image button event</summary>
        /// <param name="grid" type="dhtmlXGridObject">Control's id</param>
        if (grid._isLocked == undefined) {
            GridControl.PrepareLockableGrid(grid, true);
        }
        grid._isLocked = true;
    },

    UnlockGrid: function (grid) {
        /// <summary>Method to bind image button event</summary>
        /// <param name="grid" type="dhtmlXGridObject">Control's id</param>
        if (grid._isLocked == undefined) {
            GridControl.PrepareLockableGrid(grid, false);
        }
        grid._isLocked = false;
    },

    SetDisabledButtonOnGrid: function (grid, button_id, col_id, isDisabled) {
        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);
            EnableGridButton(grid, button_id, row_id, col_id, !isDisabled);
        }
        //        grid.attachEvent("onAfterSorting", function (index, type, direction) {
        //            for (var i = 0; i < grid.getRowsNum(); i++) {
        //                var row_id = grid.getRowId(i);
        //                EnableGridButton(grid, button_id, row_id, col_id, enable);
        //            }
        //        });
    },

    /* --- Merge --- */
    UpdateCell: function (grid, row_id, col_name, data) {
        if (data != undefined && typeof (data) == "string") {
            data = ConvertSSH(data);
        }
        grid.cells2(grid.getRowIndex(row_id), grid.getColIndexById(col_name)).setValue(data);
    },
    UpdateCells: function (grid, row_id, lst_col_data) {
        for (var i = 0; i < lst_col_data.length; i++) {
            if (lst_col_data[i].length >= 2) {
                GridControl.UpdateCell(grid, row_id, lst_col_data[i][0], lst_col_data[i][1]);
            }
        }
    },
    /* ------------- */

    DisableSelectionHighlight: function () {
        jss("DIV.gridbox_dhx_secom TABLE.obj TR.rowselected TD").remove();
        jss("DIV.gridbox_dhx_secom TABLE.obj TR.rowselected TD.cellselected").remove();
    }
}

/* --- Merge --- */
function str_custom(a, b, order) {    // the name of the function must be > than 5 chars
    if (order == "asc")
        return (a.toLowerCase() > b.toLowerCase() ? 1 : -1);
    else
        return (a.toLowerCase() > b.toLowerCase() ? -1 : 1);
}
/* ------------- */

function date_custom(a, b, order) {    // the name of the function must be > than 5 chars
    var lst = [a,b];
    var date = [null, null];
    for (var idx = 0; idx < lst.length; idx++) {
        if (lst[idx] != undefined && lst[idx] != "" && lst[idx] != "-") {
            var dd = lst[idx].substring(0, 2);
            var mm = lst[idx].substring(3, 6);
            var yy = lst[idx].substring(7, 11);
            date[idx] = new Date(yy + "/" + mm + "/" + dd);
        }
        else
            date[idx] = new Date("1000/01/01");
    }

    if (order == "asc")
        return (date[0] > date[1] ? 1 : -1);
    else
        return (date[0] > date[1] ? -1 : 1);
}

function link_custom(a, b, order) {    // the name of the function must be > than 5 chars
    var atxt = GetLinkTxt(a);
    var btxt = GetLinkTxt(b);

    if (order == "asc")
        return (atxt.toLowerCase() > btxt.toLowerCase() ? 1 : -1);
    else
        return (atxt.toLowerCase() > btxt.toLowerCase() ? -1 : 1);
}

function GetLinkTxt(l) {
//    var txt = l;

//    while (txt.indexOf("\">") >= 0) {
//        var b_idx = txt.indexOf("\">");
//        txt = txt.substring(b_idx + 2, txt.length);
//    }

//    var e_idx = txt.indexOf("<INPUT");
//    if (e_idx >= 0) {
//        txt = txt.substring(0, e_idx);
//    }

//    return txt;
    return $(l).text();
}


function date_my_custom(a, b, order) {
    var lst = [a, b];
    var date = [null, null];
    for (var idx = 0; idx < lst.length; idx++) {
        if (lst[idx] != undefined && lst[idx] != "") {
            var mm = lst[idx].substring(0, 3);
            var yy = lst[idx].substring(4, 8);
            date[idx] = new Date(yy + "/" + mm + "/01");
        }
        else
            date[idx] = new Date("1000/01/01");
    }

    if (order == "asc")
        return (date[0] > date[1] ? 1 : -1);
    else
        return (date[0] > date[1] ? -1 : 1);
}

// Added by Non A. 19/Jul/2012 : Required for celltype=ron2
function ron2_SetUpNumberFormat(grid, result) {

    if (result && result.match(/"ron2"/)) {
        grid._numberformat = [];

        var xmlDoc;
        if (window.DOMParser) {
            var parser = new DOMParser();
            xmlDoc = parser.parseFromString(result, "text/xml");
        }
        else // Internet Explorer
        {
            xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
            xmlDoc.async = false;
            xmlDoc.loadXML(result);
        }

        var columns = xmlDoc.getElementsByTagName("column");
        for (var col_idx = 0; col_idx < columns.length; col_idx++) {
            var column = columns[col_idx];
            grid._numberformat.push(column.getAttribute("numberformat"));
        }
    }

}

function decimal_custom(a, b, order) {    // the name of the function must be > than 5 chars
    var decA = parseFloat(a.replace(/^\+/, "").replace(/,/g, ""))
    var decB = parseFloat(b.replace(/^\+/, "").replace(/,/g, ""))

    if (order == "asc")
        return (decA > decB ? 1 : -1);
    else
        return (decA > decB ? -1 : 1);
}
