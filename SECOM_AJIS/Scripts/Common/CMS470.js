var gridBillingTarget;
$(document).ready(function () {

    CMS470_FirstLoadScreen();
    InitialBillingTargetGridData();

    ///// even change checkboox /////
    $("#CMS470_chkJuristic").change(SelectChangeJuristic)

    $("#btnSearch").click(function () {

        CMS470_Search();

    });

    $("#btnClear").click(CMS470_Clear);

    /*==== event Customer Name keypress ====*/
    $("#divSpecifySearchCondition input[id=CMS470_BillingClientName]").InitialAutoComplete("/Master/GetBillingClientName"); // Note : in Master/Controllers/BillingClientData.cs/GetBillingClientName()

    /*==== event Site Name keypress ====*/
    $("#divSpecifySearchCondition input[id=CMS470_Address]").InitialAutoComplete("/Master/GetBillingClientAddress"); // Note :Master/Controllers/BillingClientData.cs/GetBillingClientName()

    InitialNumericInputTextBox(["CMS470_TelephoneNo"], true);

});

function SelectChangeJuristic() {
    if ($("#CMS470_chkJuristic").prop("checked")) {
        $("#CMS470_CompanyTypeCode").attr("disabled", false);
    }
    else {
        $("#CMS470_CompanyTypeCode").attr("disabled", true);
    }
}
function CMS470_FirstLoadScreen() {
    $("#divSpecifySearchCondition").show();
    $("#divSearchResult").hide();
    $("#CMS470_divBillingTargetInfo").hide();

    $("#divSpecifySearchCondition").clearForm();

    $("#CMS470_chkJuristic").attr("checked", true);
    $("#CMS470_chkIndividual").attr("checked", true);
    $("#CMS470_chkAssociation").attr("checked", true);
    $("#CMS470_chkPublicOffice").attr("checked", true);
    $("#CMS470_chkOther").attr("checked", true);

}

function InitialBillingTargetGridData() {

    gridBillingTarget = $("#gridBillingTarget").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS470_InitialBillingTargetGrid");
    BindControlBillingTargetGrid(gridBillingTarget);
}

function BindControlBillingTargetGrid(myGrid) {
    SpecialGridControl(myGrid, ["Detail", "Select"]);
    BindOnLoadedEvent(myGrid,
    function (gen_ctrl) {
        if (CheckFirstRowIsEmpty(myGrid, false) == false) {
            for (var i = 0; i < myGrid.getRowsNum(); i++) {

                var rid = myGrid.getRowId(i);

                if (gen_ctrl == true) {
                    //----------- Generate Detail button        
                    GenerateDetailButton(myGrid, "btnDetail", rid, "Detail", true);
                    GenerateSelectButton(myGrid, "btnSelect", rid, "Select", true);

                }

                BindGridButtonClickEvent("btnDetail", rid, doDetailBillingtargetGrid);
                BindGridButtonClickEvent("btnSelect", rid, doSelectBillingtargetGrid);
            }
            //$("#chkHeader").unbind("click");
            //$("#chkHeader").click(selectAllCheckboxControl);
            myGrid.setSizes();
        }
    });
}

function CMS470Initial() {
    ChangeDialogButtonText(["Close"], [$('#btnClose_cms470').val()]);
}

function CMS470_Search() {

    // disable search button
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    var data = $("#SearchCondition").serialize();
    //alert(data);

    var parameter = CreateObjectData($("#SearchCondition").serialize()); // + "&Counter=" + CMS270_Count, true);
    $("#gridBillingTarget").LoadDataToGrid(gridBillingTarget, ROWS_PER_PAGE_FOR_SEARCHPAGE, true,
"/Common/CMS470_SearchResponse", parameter, "View_dtBillingTargetData", false,
        function () {
            // enable search button            
            //document.getElementById('divSearchResult').scrollIntoView();
            master_event.ScrollWindow("#divSearchResult", true);
            $("#btnSearch").attr("disabled", false);
            master_event.LockWindow(false);
            $("#CMS470_divBillingTargetInfo").hide();
        },
        function (result, controls, isWarning) {
            if (isWarning == undefined) {
                $("#divSearchResult").show();
            } else if (controls != undefined) {

            }
        });

}

function doDetailBillingtargetGrid(row_id) {

    gridBillingTarget.selectRow(gridBillingTarget.getRowIndex(row_id));

    var strBillingTargetCode = gridBillingTarget.cells2(gridBillingTarget.getRowIndex(row_id), gridBillingTarget.getColIndexById
('BillingTargetCode_Short')).getValue().toString();
    var obj = {
        BillingTargetCode: strBillingTargetCode
    };

    ajax_method.CallScreenController("/Common/CMS470_GetBillingTargetForView", obj,
        function (result) {
            if (result != undefined && result != "") {
                SetDataToControl(result)
            }
            else {
                ClearDataBillingTargetInfo();
            }
            IntialPage();

        });

}

function IntialPage() {

    // Set null value to "-"  ***
    $("#CMS470_divBillingTargetInfo").show();
    //document.getElementById('CMS470_divBillingTargetInfo').scrollIntoView();
    master_event.ScrollWindow("#CMS470_divBillingTargetInfo", true);
    $("#CMS470_divBillingTargetInfo").SetEmptyViewData();
}

function SetDataToControl(obj) {   
    $("#CMS470_BillingTargetCodeInfo").text(obj[0].BillingTargetCode_Short);
    $("#CMS470_BillingOfficeInfo").text(obj[0].BillingOfficeCode + ':' + obj[0].OfficeName);
    $("#CMS470_CustomerTypeInfo").text(obj[0].CustTypeCode + ':' + obj[0].CustTypeName);
    $("#CMS470_NameEnglishInfo").text(obj[0].FullNameEN);
    $("#CMS470_BranchNameEnglishInfo").text(obj[0].BranchNameEN);
    $("#CMS470_AddressEnglishInfo").text(obj[0].AddressEN);
    $("#CMS470_NameLocalInfo").text(obj[0].FullNameLC);
    $("#CMS470_BranchNameLocalInfo").text(obj[0].BranchNameLC);
    $("#CMS470_AddressLocalInfo").text(obj[0].AddressLC);
    $("#CMS470_NationalityInfo").text(obj[0].Nationality);
    $("#CMS470_TelephoneNoInfo").text(obj[0].PhoneNo);
    $("#CMS470_IdNoTaxIdNoInfo").text(obj[0].IDNo);
    $("#CMS470_BusinessTypeInfo").text(obj[0].BusinessTypeName);
    $("#CMS470_BillingClientContactPersonInfo").text(obj[0].ContactPersonName);
    $("#CMS470_MemoInfo").text(obj[0].Memo);


}

function CMS470_Clear() {
    CMS470_FirstLoadScreen();
}

function doSelectBillingtargetGrid(row_id) {


    gridBillingTarget.selectRow(gridBillingTarget.getRowIndex(row_id));

    /* ==== Create json object for string json ==== */
    var strJson = gridBillingTarget.cells2(gridBillingTarget.getRowIndex(row_id), gridBillingTarget.getColIndexById('Object')).getValue().toString();
    strJson = htmlDecode(strJson);
    var objSelcted = JSON.parse(strJson); // require json2.js

    //alert(strJson);

    if (typeof (CMS470Response) == "function") {
        CMS470Response(objSelcted);
    }
}

function ClearDataBillingTargetInfo() {
    $("#CMS470_BillingTargetCodeInfo").text("");
    $("#CMS470_BillingOfficeInfo").text("");
    $("#CMS470_CustomerTypeInfo").text("");
    $("#CMS470_NameEnglishInfo").text("");
    $("#CMS470_BranchNameEnglishInfo").text("");
    $("#CMS470_AddressEnglishInfo").text("");
    $("#CMS470_NameLocalInfo").text("");
    $("#CMS470_BranchNameLocalInfo").text("");
    $("#CMS470_AddressLocalInfo").text("");
    $("#CMS470_NationalityInfo").text("");
    $("#CMS470_TelephoneNoInfo").text("");
    $("#CMS470_IdNoTaxIdNoInfo").text("");
    $("#CMS470_BusinessTypeInfo").text("");
    $("#CMS470_BillingClientContactPersonInfo").text("");
    $("#CMS470_MemoInfo").text("");
}