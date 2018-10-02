/// <reference path="../Base/Master.js" />

var CMS170_mygrid;
var CMS170_pageRow;
var btnSelectId = "CMS170SelectBtn";

$(document).ready(function () {

    CMS170_pageRow = ROWS_PER_PAGE_FOR_SEARCHPAGE;

    if ($("#cms170_grid_result").length > 0) {
        CMS170_mygrid = $("#cms170_grid_result").InitialGrid(CMS170_pageRow, true, "/Common/CMS170_InitialGrid");

        /*=========== Set hidden column =============*/
        CMS170_mygrid.setColumnHidden(CMS170_mygrid.getColIndexById("ToJson"), true);
    }

    /*==== event Customer Name keypress ====*/
    $("#cms170_InstrumentName").InitialAutoComplete("/Common/GetInstrumentName"); //$("#cms170_InstrumentName").keypress(InstrumentName_keypress);

    /*==== event Employee Name keypress ====*/
    $("#cms170_Maker").InitialAutoComplete("/Common/GetMaker"); //$("#cms170_Maker").keypress(Maker_keypress);

    /*==== event btnSearch click ====*/
    $("#cms170_btnSearch").click(function () {
        CMS170_validateSearchCriteria();
    });

    /*==== event btnClear click ====*/
    $("#cms170_btnClear").click(function () {
        CMS170_clearSearchCriteria();
    });

    /* ===== binding event when finish load data ===== */
    SpecialGridControl(CMS170_mygrid, ["Select"]);
    BindOnLoadedEvent(CMS170_mygrid, function (gen_ctrl) {
        var selectColInx = CMS170_mygrid.getColIndexById('Select');

        for (var i = 0; i < CMS170_mygrid.getRowsNum(); i++) {
            var row_id = CMS170_mygrid.getRowId(i);

            if (gen_ctrl == true) {
                /* ===== column Select button ===== */
                //CMS170_mygrid.cells2(i, selectColInx).setValue(GenerateHtmlButton(btnSelectId, row_id, "Select", true));
                GenerateSelectButton(CMS170_mygrid, btnSelectId, row_id, "Select", true);
            }

            /* ===== Bind event onClick to button ===== */
            //BindGridHtmlButtonClickEvent(btnSelectId, row_id, function (rid) {
            BindGridButtonClickEvent(btnSelectId, row_id, function (rid) {
                CMS170_doSelectAction(rid);
            });
        }
    });

    setDisableCheckbox();

    CMS170_OnInitial();

    $("#cms170_InstTypeGen").change(function () { RemoveClassHighlight(); });
    $("#cms170_InstTypeMon").change(function () { RemoveClassHighlight(); });
    $("#cms170_InstTypeMat").change(function () { RemoveClassHighlight(); });

    //$("#cms170_InstrumentCode").InitialAutoComplete("/Master/GetInstrumentCode"); //Add by Jutarat A. on 25032014
    $("#cms170_InstrumentCode").InitialAutoComplete("/Master/GetInstrumentCodeAll", null, true);
});

function setDisableCheckbox() {

    if ($("#cms170_DisableExpType").val() == "True") {
        $("#cms170_ExpTypeHas").attr("disabled", true);
        $("#cms170_ExpTypeNo").attr("disabled", true);
    }

    if ($("#cms170_DisableProdType").val() == "True") {
        $("#cms170_ProdTypeSale").attr("disabled", true);
        $("#cms170_ProdTypeAlarm").attr("disabled", true);
    }

    if ($("#cms170_DisableInstType").val() == "True") {
        $("#cms170_InstTypeGen").attr("disabled", true);
        $("#cms170_InstTypeMon").attr("disabled", true);
        $("#cms170_InstTypeMat").attr("disabled", true);
    }
}

function CMS170_OnInitial() {

    $("#CMS170_SearchResult").hide();
}

function CMS170Initial() {
    ChangeDialogButtonText(["Cancel"], [$('#cms170_btnCancel').val()]);
}

function CMS170_validateSearchCriteria() {
    var cond = {
        InstFlagMain: $("#cms170_InstFlagMain").prop("checked"),
        InstFlagOption: $("#cms170_InstFlagOption").prop("checked"),
        ExpTypeHas: $("#cms170_ExpTypeHas").prop("checked"),
        ExpTypeNo: $("#cms170_ExpTypeNo").prop("checked"),
        ProdTypeSale: $("#cms170_ProdTypeSale").prop("checked"),
        ProdTypeAlarm: $("#cms170_ProdTypeAlarm").prop("checked"),
        InstTypeGen: $("#cms170_InstTypeGen").prop("checked"),
        InstTypeMon: $("#cms170_InstTypeMon").prop("checked"),
        InstTypeMat: $("#cms170_InstTypeMat").prop("checked")
    };

    call_ajax_method(
        '/Common/CMS170_CheckReqField',
        cond,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["cms170_InstFlagMain"
                            , "cms170_InstFlagOption"
                            , "cms170_ExpTypeHas"
                            , "cms170_ExpTypeNo"
                            , "cms170_ProdTypeSale"
                            , "cms170_ProdTypeAlarm"
                            , "cms170_InstTypeGen"
                            , "cms170_InstTypeMon"
                            , "cms170_InstTypeMat"]
                            , controls);
                $("#cms170_btnSearch").focus();
                return;
            } else {
                CMS170_search();
            }
        }
    );
}

function CMS170_search() {
    //Load Data to Grid
    var cond = {
        InstrumentCode: $("#cms170_InstrumentCode").val(),
        InstrumentName: $("#cms170_InstrumentName").val(),
        Maker: $("#cms170_Maker").val(),
        SupplierCode: $("#cms170_SupplierType").val(),
        LineUpTypeCode: $("#cms170_LineUpType").val(),
        InstFlagMain: $("#cms170_InstFlagMain").prop("checked"),
        InstFlagOption: $("#cms170_InstFlagOption").prop("checked"),
        ExpTypeHas: $("#cms170_ExpTypeHas").prop("checked"),
        ExpTypeNo: $("#cms170_ExpTypeNo").prop("checked"),
        ProdTypeSale: $("#cms170_ProdTypeSale").prop("checked"),
        ProdTypeAlarm: $("#cms170_ProdTypeAlarm").prop("checked"),
        InstTypeGen: $("#cms170_InstTypeGen").prop("checked"),
        InstTypeMon: $("#cms170_InstTypeMon").prop("checked"),
        InstTypeMat: $("#cms170_InstTypeMat").prop("checked")
    };

    $("#cms170_btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    $("#cms170_grid_result").LoadDataToGrid(CMS170_mygrid, CMS170_pageRow, true, "/Common/CMS170_Search", cond, "doInstrumentData", false,
        function (result, controls, isWarning) {
            $("#cms170_btnSearch").removeAttr("disabled");
            master_event.LockWindow(false);

            if (result != undefined) {
                //document.getElementById('CMS170_SearchResult').scrollIntoView();
                master_event.ScrollWindow("#CMS170_SearchResult", true);
            }
        }, //post-load
        function (result, controls, isWarning) { //pre-load
            if (isWarning == undefined) {
                $("#CMS170_SearchResult").show();

                //if (!$("#CMS170_SearchResult").is(':visible')) {
                //    $("#CMS170_SearchResult").show('slow', function () {
                //        document.getElementById('CMS170_SearchResult').scrollIntoView();
                //    });
                //}
                //else {
                //    document.getElementById('CMS170_SearchResult').scrollIntoView();
                //}
            }
        });

    //Show Search Result
    //$("#CMS170_SearchResult").show();
}

function CMS170_clearSearchCriteria() {
    $("#cms170_InstrumentCode").val("");
    $("#cms170_InstrumentName").val("");
    $("#cms170_Maker").val("");
    $("#cms170_SupplierType").val("");
    $("#cms170_LineUpType").val("");
    $("#cms170_InstFlagMain").attr("checked", false);
    $("#cms170_InstFlagOption").attr("checked", false);

    if ($("#cms170_DisableExpType").val() == "False") {
        $("#cms170_ExpTypeHas").attr("checked", false);
        $("#cms170_ExpTypeNo").attr("checked", false);
    }
    if ($("#cms170_DisableProdType").val() == "False") {
        $("#cms170_ProdTypeSale").attr("checked", false);
        $("#cms170_ProdTypeAlarm").attr("checked", false);
    }
    if ($("#cms170_DisableInstType").val() == "False") {
        $("#cms170_InstTypeGen").attr("checked", false);
        $("#cms170_InstTypeMon").attr("checked", false);
        $("#cms170_InstTypeMat").attr("checked", false);
    }

    CloseWarningDialog();
    RemoveClassHighlight();

    CMS170_OnInitial();
}

function RemoveClassHighlight() {
    $("#cms170_InstFlagMain").removeClass("highlight");
    $("#cms170_InstFlagOption").removeClass("highlight");
    $("#cms170_ExpTypeHas").removeClass("highlight");
    $("#cms170_ExpTypeNo").removeClass("highlight");
    $("#cms170_ProdTypeSale").removeClass("highlight");
    $("#cms170_ProdTypeAlarm").removeClass("highlight");
    $("#cms170_InstTypeGen").removeClass("highlight");
    $("#cms170_InstTypeMon").removeClass("highlight");
    $("#cms170_InstTypeMat").removeClass("highlight");
}

function CMS170_doSelectAction(rid) {

    //hilight row
    CMS170_mygrid.selectRow(CMS170_mygrid.getRowIndex(rid));

    var col = CMS170_mygrid.getColIndexById('ToJson');
    var strJson = CMS170_mygrid.cells(rid, col).getValue().toString();

    strJson = htmlDecode(strJson);

    var cms170_object = JSON.parse(strJson); 

    if (typeof (CMS170Response) == "function")
        CMS170Response(cms170_object);
}

//function InstrumentName_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#cms170_InstrumentName",
//                                cond,
//                                "/Common/GetInstrumentName",
//                                { "cond": cond },
//                                "doInstrumentName",
//                                "InstrumentName",
//                                "InstrumentName");
//    }
//}

//function Maker_keypress(e) {
//    if ($(this).val().length + 1 >= 3) {
//        var cond = $(this).val() + String.fromCharCode(e.which);
//        InitialAutoCompleteControl("#cms170_Maker",
//                                cond,
//                                "/Common/GetMaker",
//                                { "cond": cond },
//                                "dtGetInstrumentMaker",
//                                "Maker",
//                                "Maker");
//    }
//}
