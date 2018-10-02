/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

$(document).ready(function () {
    InitialControlCTS180_01();
    InitialEventCTS180_01();
    VisibleDocumentSectionCTS180_01(false);
});

function InitialEventCTS180_01() {

}

function InitialControlCTS180_01() {

}

function InitialInputDataCTS180_01() {
    $("#divSpecifySearchCondition").clearForm();

    DiableSpecifySearchCondition(false);

//    $("#chkNotIssued").attr("checked", true);
//    $("#chkIssuedButNoRegist").attr("checked", true);
//    $("#chkCollectionRegist").attr("checked", true);
//    $("#chkIssuedButNotUsed").attr("checked", true);

    gridDocumentListCTS180 = $("#gridDocumentList").InitialGrid(pageRow, true, "/Contract/CTS180_InitialGridDocumentList",
        function (gen_ctrl) {
            SpecialGridControl(gridDocumentListCTS180, ["Select"]);

            BindOnLoadedEvent(gridDocumentListCTS180,
                function (gen_ctrl) {
                    gridDocumentListCTS180.setColumnHidden(gridDocumentListCTS180.getColIndexById("IsEnableSelect"), true);
                    gridDocumentListCTS180.setColumnHidden(gridDocumentListCTS180.getColIndexById("DocID"), true);
                    gridDocumentListCTS180.setColumnHidden(gridDocumentListCTS180.getColIndexById("DocumentCode"), true);

                    GenerateSelectButtonDocumentList(isViewMode == false, gen_ctrl);
                }
            );
    });
}

function SetDocumentSectionModeCTS180_01(isView) {
    $("#divSpecifySearchCondition").SetViewMode(isView);

    if (isView == false) {
        if (isViewMode) {
            DiableSpecifySearchCondition(true);
        }
    }
}

function VisibleDocumentSectionCTS180_01(isVisible) {
    if (isVisible) {
        $("#divDocumentList").show();
    }
    else {
        $("#divDocumentList").hide();
    }
}