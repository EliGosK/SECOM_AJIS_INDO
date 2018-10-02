//----------------------------------------------
// Create : Nattapong N.
//   Date : 23 Jun 2011
// Update : 23 Jun 2011
//---------------------------------------------
/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>

/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>
/// <reference path="../Base/GridControl.js" />

/// <reference path="../../Scripts/Base/AutoComplete.js"/>
/// <reference path="../jquery-1.5.1-vsdoc.js" />
/// <reference path="Dialog.js" />
/// <reference path="../../Views/Quotation/QUS010.cshtml" />
/// <reference path="../Base/control_events.js" />


var QUS010_Method = {
    ResultGrid: null,
    SelectIdx: null,

    Initial: function () {
        if (ViewMode != undefined && ViewMode != "2") {
            _CallerScreenID = C_SCREEN_ID_MAIN;
        }

        if ((_CallerScreenID == C_SCREEN_ID_MAIN)
            || (_CallerScreenID == C_SCREEN_ID_FN99)
            || (_CallerScreenID == C_SCREEN_ID_FQ99)) {
            $("#SearchCriteriaSection input[type='text']").val("");
            $("select").each(function () {
                $(this).children("option").removeAttr('selected');
                $(this).children("option:first").attr('selected', 'selected');
            });
            $("#SearchCriteriaSection #QuotationTargetCode").SetDisabled(false);

            if ((_CallerScreenID == C_SCREEN_ID_FN99)
                || (_CallerScreenID == C_SCREEN_ID_FQ99)) {
                $("#LockStatus option[value=" + C_LOCK_STATUS_UNLOCK + "]").attr("selected", "selected");
                $("#LockStatus").attr("disabled", "disabled");

                if (_QuotationTargetCode != "") {
                    $("#SearchCriteriaSection #QuotationTargetCode").val(_QuotationTargetCode);
                    $("#SearchCriteriaSection #QuotationTargetCode").SetDisabled(true);
                }
            }
        }
    },
    InitialControl: function () {
        InitialTrimTextEvent([
            "QuotationTargetCode",
            "Alphabet",
            "ContractTargetCode",
            "ContractTargetName",
            "ContractTargetAddr",
            "SiteCode",
            "SiteName",
            "SiteAddr",
            "EmpNo",
            "EmpName"
        ]);

        InitialDateFromToControl("#QuotationDateFrom", "#QuotationDateTo");

        if ((_CallerScreenID == C_SCREEN_ID_MAIN)
            || (_CallerScreenID == C_SCREEN_ID_FN99)
            || (_CallerScreenID == C_SCREEN_ID_FQ99)) {

            QUS010_Method.ResultGrid = $("#SearchResultList").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Quotation/InitialGrid_QUS010_nonSel");
            $("#subSearchResult").hide();
        }
        else if ((_CallerScreenID == C_SCREEN_ID_CP12_PLAN)
            || (_CallerScreenID == C_SCREEN_ID_CP12_INST)
            || (_CallerScreenID == C_SCREEN_ID_CQ12)) {

            $("#SearchCriteriaSection").hide();
            $("#subSearchResult").hide();

            var obj = CreateObjectData(
                "&ScreenCallerID=" + _CallerScreenID +
                "&TargetCodeTypeCode=" + _TargetCodeTypeCode +
                "&ServiceTypeCode=" + _ServiceTypeCode +
                "&QuotationTargetCode=" + _QuotationTargetCode +
                "&LockStatus=" + C_LOCK_STATUS_UNLOCK);

            QUS010_Method.ResultGrid = $("#SearchResultList").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, true,
                "/Quotation/QUS010_XML", obj, "View_dtSearchQuotationListResult", false, null, 
                function (result, controls, isWarning) { // pre-load
                    if (isWarning == undefined) {
                        $("#subSearchResult").show();
                    }
                });
        }
        else {

            QUS010_Method.ResultGrid = $("#SearchResultList").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Quotation/InitialGrid_QUS010");
            $("#subSearchResult").hide();
        }

        QUS010_Method.InitialGrid();


        $("#btnSearch").click(QUS010_Method.SearchClick);
        $("#btnClear").click(QUS010_Method.ClearClick);
    },
    InitialGrid: function () {
        if (QUS010_Method.ResultGrid != undefined) {
            SpecialGridControl(QUS010_Method.ResultGrid, ["Select", "Detail"]);
            BindOnLoadedEventV2(QUS010_Method.ResultGrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, true, function (rid) {
                if (ViewMode == "2") {
                    GenerateSelectButton(QUS010_Method.ResultGrid, "btnSelect", rid, "Select", true);
                    BindGridButtonClickEvent("btnSelect", rid, function (irid) {
                        var col = QUS010_Method.ResultGrid.getColIndexById('QuotationCode_Short');
                        var colAl = QUS010_Method.ResultGrid.getColIndexById('Alphabet');
                        var objQUS010 = {
                            'QuotationTargetCode': QUS010_Method.ResultGrid.cells(irid, col).getValue(),
                            'Alphabet': QUS010_Method.ResultGrid.cells(irid, colAl).getValue()
                        };

                        if (typeof (QUS010Response) == "function")
                            QUS010Response(objQUS010);
                    });
                }

                GenerateDetailButton(QUS010_Method.ResultGrid, "btnDetail", rid, "Detail", true);
                BindGridButtonClickEvent("btnDetail", rid, function (irid) {
                    QUS010_Method.ResultGrid.selectRow(QUS010_Method.ResultGrid.getRowIndex(rid));
                    QUS010_Method.SelectIdx = irid;

                    var col = QUS010_Method.ResultGrid.getColIndexById("ProductTypeCode");
                    var Prod_Type = QUS010_Method.ResultGrid.cells(irid, col).getValue();

                    if (Prod_Type == C_PROD_TYPE_SALE) {
                        $("#dlgQUS").OpenQUS011Dialog("QUS010");
                    } else {
                        $("#dlgQUS").OpenQUS012Dialog("QUS010");
                    }
                });
            });
        }
    },

    SearchClick: function () {
        $("#SearchCriteriaSection").ResetToNormalControl();

        $("#btnSearch").attr("disabled", true);
        master_event.LockWindow(true);

        var strQuerySearch = $("#SearCriteria").serialize() +
        "&TargetCodeTypeCode=" + _TargetCodeTypeCode +
        "&ServiceTypeCode=" + _ServiceTypeCode;
        if ($("#LockStatus").is(":disabled")) {
            strQuerySearch = strQuerySearch +
            "&LockStatus=" + $('#LockStatus').val();
        }
        var obj = CreateObjectData(strQuerySearch);
        $("#SearchResultList").LoadDataToGrid(QUS010_Method.ResultGrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Quotation/QUS010_XML", obj, "View_dtSearchQuotationListResult", false
            , function (result, controls, isWarning) { // post-load
                // Enable search button
                $("#btnSearch").attr("disabled", false);
                master_event.LockWindow(false);

                if (controls != undefined) {
                    VaridateCtrl(["#SearchCriteriaSection #QuotationTargetCode"], controls);
                }
                else if (result != undefined) {
                    master_event.ScrollWindow("#subSearchResult", (ViewMode == 2));
                }
            }
            , function (result, controls, isWarning) { // pre-load
                if (isWarning == undefined) {
                    $("#subSearchResult").show();
                }
            });
    },
    ClearClick: function () {
        $("#SearchCriteriaSection").ResetToNormalControl();
        $("#SearchCriteriaSection").clearForm();
        ClearDateFromToControl("#QuotationDateFrom", "#QuotationDateTo");
        $("#subSearchResult").hide();

        QUS010_Method.Initial();
    }
};

$(document).ready(function () {
    QUS010_Method.Initial();
    QUS010_Method.InitialControl();
});

function QUS010Initial() {
    ChangeDialogButtonText(["Close"], [$('#lblBtnClose').val()]);
}

/* --- Popup input object --- */
/* -------------------------- */
function QUS010_QUS012Object() {
    var colInxQuotationTargetCode = QUS010_Method.ResultGrid.getColIndexById("QuotationCode_Short");
    var colInxAlphabet = QUS010_Method.ResultGrid.getColIndexById("Alphabet");

    var QuotationTargetCode = QUS010_Method.ResultGrid.cells(QUS010_Method.SelectIdx, colInxQuotationTargetCode).getValue();
    var Alphabet = QUS010_Method.ResultGrid.cells(QUS010_Method.SelectIdx, colInxAlphabet).getValue();

    return {
        QuotationTargetCode: QuotationTargetCode,
        Alphabet: Alphabet,
        HideQuotationTarget: false
    };
}
function QUS010_QUS011Object() {
    var colInxQuotationTargetCode = QUS010_Method.ResultGrid.getColIndexById("QuotationCode_Short");
    var colInxAlphabet = QUS010_Method.ResultGrid.getColIndexById("Alphabet");

    var QuotationTargetCode = QUS010_Method.ResultGrid.cells(QUS010_Method.SelectIdx, colInxQuotationTargetCode).getValue();
    var Alphabet = QUS010_Method.ResultGrid.cells(QUS010_Method.SelectIdx, colInxAlphabet).getValue();
    return {
        QuotationTargetCode: QuotationTargetCode,
        Alphabet: Alphabet,
        HideQuotationTarget: false
    };
}
/* -------------------------- */