/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>
/// <reference path="../../Scripts/Base/AutoComplete.js"/>
/// <reference path="../json.js" />
/// <reference path="../Contract/Dialog.js" />

/// <reference path="../Common/Dialog.js" />



$(document).ready(function () {


    /* ============= CMS210 =========== */
    $("#btnOpenCMS210").click(function () {

        //--doContractInfoCondition--
        //ContractCode
        //OCC
        //ContractTargetCode
        //PurchaserCustCode
        //RealCustomerCode
        //SiteCode
        //ServiceTypeCode  // 1: sale , 2: rental
        //MATargetContractCode
        //ProductCode

        var parameter = CMS210Object();
        $("#dlgTest").OpenCMS210Dialog("CMS020");

    });

    /* ================ CMS220 ============= */
    $("#btnOpenCMS220").click(function () {

        // ContractCode , OCC , CSCustCode , RCCustCode , SiteCode

        //--doContractInfoCondition--
        //ContractCode
        //OCC
        //ContractTargetCode
        //PurchaserCustCode
        //RealCustomerCode
        //SiteCode
        //ServiceTypeCode
        //MATargetContractCode
        //ProductCode

        var parameter = CMS220Object();
        $("#dlgTest").OpenCMS220Dialog("CMS020");

    });


    /* =============== CMS060 =============== */
    $("#btnOpenCMS060").click(function () {
        $("#dlgTest").OpenCMS060Dialog("CMS020");
    });
    /* =============== CMS170 =============== */
    $("#btnOpenCMS170").click(function () {
        $("#dlgTest").OpenCMS170Dialog();
    });

    /* =============== CMS290 =============== */
    $("#btnOpenCMS290").click(function () {
        $("#dlgTest").OpenCMS290Dialog();
    });
    /* =============== CMS250 =============== */
    $("#btnOpenCMS250").click(function () {

        $("#dlgTest").OpenCMS250Dialog("CMS020");

    });

    /* =============== CMS260 =============== */
    $("#btnOpenCMS260").click(function () {
        $("#dlgTest").OpenCMS260Dialog("CMS020");
    });

    /* =============== CMS270 =============== */
    $("#btnOpenCMS270").click(function () {
        $("#dlgTest").OpenCMS270Dialog("CMS020");
    });

    /* =============== CMS310 =============== */
    $("#btnOpenCMS310").click(function () {
        $("#dlgTest").OpenCMS310Dialog("CMS020");
    });

    /* =============== CMS131 =============== */
    $("#btnOpenCMS131").click(function () {

        $("#dlgTest").OpenCMS131Dialog("CMS020");
    });

    /* =============== CMS300 =============== */
    $("#btnOpenCMS300").click(function () {
        $("#dlgTest").OpenCMS300Dialog("CMS020");
    });

    $("#btnOpenQUS040").click(function () {
        $("#dlgTest").OpenQUS040Dialog("CMS020");
    });

    $("#btnOpenQUS010").click(function () {
        $("#dlgTest").OpenQUS010Dialog('CMS020');
    });
    $("#btnOpenWarning").click(function () {
        OpenWarningDialog("Error");
    });

    $("#btnOpenMAS050").click(function () {
        $("#dlgTest").OpenMAS050Dialog();
    });

    $("#btnOpenMAS030").click(function () {
        $("#dlgTest").OpenMAS030Dialog();
    });

    $("#btnOpenMAS040").click(function () {
        $("#dlgTest").OpenMAS040Dialog();
    });


    $("#btnOpenQUS011").click(function () {
        $("#dlgTest").OpenQUS011Dialog();
    });
    $("#btnOpenQUS012").click(function () {
        $("#dlgTest").OpenQUS012Dialog();
    });

    $("#btnOpenQUS050").click(function () {
        $("#dlgTest").OpenQUS050Dialog();
    });


    $('#btnOpenCTS261').click(function () {
        $("#dlgTest").OpenCTS261Dialog("CMS020");
    });

    //------------------- Contract Menu -------------------------

    $("#btnBeforeContract").click(function () {
        //$("#dlgTest").OpenBeforeContractMenu();
        //$("#subMenu").show();
        OpenContractMenuDialog("/contract/Contract_Before");
    });




    $("#btnAfterContract").click(function () {
        $("#dlgTest").OpenAfterContractMenu();
    });

    $("#btnIncidentARContract").click(function () {
        $("#dlgTest").OpenIncidentARContractMenu();
    });
    $("#btnARContract").click(function () {
        $("#dlgTest").OpenARContractMenu();
    });
    $("#btnProjectContract").click(function () {
        $("#dlgTest").OpenProjectContractMenu();
    });



    $("#btnOpenCTS010Edit").click(function () {
        var obj = {
            ScreenModeID: 1
        };
        CallScreenWithAuthority("/Contract/CTS010", obj, false);
    });
    $("#btnOpenCTS010Approve").click(function () {
        var obj = {
            ScreenModeID: 2,
            QuotationTargetCode: "FMA0000138"
        };
        CallScreenWithAuthority("/Contract/CTS010", obj, false);
    });

});

/* ====================== Oject as parameter ====================== */
function QUS010Object() {
    return {
        strCallerScreenID: "CTS052"
        , ViewMode: '2'
        , strServiceTypeCode: "2"
        , strTargetCodeTypeCode: "2"
        , strQuotationTargetCode: "N0000221"
    };
}
function QUS011Object() {
    return {
        QuotationTargetCode: "FQ0000073",
        Alphabet: "AA",
        HideQuotationTarget: false
    };
}
function QUS040Object() {
    return {


};
}
function QUS012Object() {
    return {
        QuotationTargetCode: "FSG0000115",
        Alphabet: "AA",
        HideQuotationTarget: false
    };
}
function QUS050Object() {
    return {
        ScreenID: "QUS020"
    };
}
function QUS050Response(dsImportData) {
    alert(dsImportData);
}
function CMS300Object() {
    return {
        ContractCode: 'N0000001',
        OCC: "",
        CSCustCode: "",
        RCCustCode: "",
        SiteCode: ""
    };
}

function CMS131Object() {
    return { ContractCode: "N0000005",
        OCC: "0001"
    };
}

function CMS210Object() {
    return { ContractCode: "N0000008",
        OCC: "0001",
        ContractTargetCode: "C0000108",
        ServiceTypeCode: "2",
        MATargetContractCode: "N0000008",
        ProductCode: "001"
    };
}

function CMS220Object() {
    return {
        "ContractCode": "N0000008",
        "OCC": "0001",
        "ContractTargetCode": "C0000147",
        "PurchaserCustCode": "C0000147",
        "RealCustomerCode": "C0000032",
        "SiteCode": "S0000059-0003" ,
        "Mode" : "Customer"
    };
}

function CMS250Object() {
    return {
        "bExistCustOnlyFlag": true
    };
}

function CMS260Object() {
    return {
        "strRealCustomerCode": "C0000014"
    };
}


function CTS261Object() {
    return { strProjectCode: "P0000013" };
}

/* ====================== Response fucntion ====================== */

var objRetrun = "";

function CMS060Response(result) {
    objRetrun = result;
    $("#dlgTest").CloseDialog();


}

function CMS250Response(result) {
    objRetrun = result;
    $("#dlgTest").CloseDialog();
}

function CMS260Response(result) {
    objRetrun = result;
    $("#dlgTest").CloseDialog();
}

function CMS270Response(result) {
    objRetrun = result;
    $("#dlgTest").CloseDialog();
}

function CMS310Response(result) {
    objRetrun = result;
    $("#dlgTest").CloseDialog();
}
function QUS040Response(result) {
    objRetrun = result;
    $('#dlgTest').CloseDialog();

    alert(objRetrun);
}
function CTS261Response(result) {
    objRetrun = result;
    $('#dlgTest').CloseDialog();

    alert(objRetrun);
}
/* ====================== Callback fucntion ====================== */

