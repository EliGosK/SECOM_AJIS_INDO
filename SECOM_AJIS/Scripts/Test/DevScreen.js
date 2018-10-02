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
var objRetrun = "";


$(document).ready(function () {
    $("#divModule").children("div").each(function () {
        var controller = $(this).attr("id");
        $(this).find("a").each(function () {
            $(this).click(function () {
                var action = $(this).html();
                var url = "/" + controller + "/" + action;

                var sb = "0";
                var obj;
                if (typeof (window[action]) == "function") {
                    obj = window[action]();
                    if (obj.SubObjectID != undefined) {
                        sb = obj.SubObjectID;
                    }
                }
                if (obj == undefined)
                    obj = "";
                else if (obj == "dialog")
                    return false;

                ajax_method.CallScreenControllerWithAuthority(url, obj, false, sb);
                return false;
            });
        });
    });
});

function CMS080() {
    return {
        strCustomerCode: $("#CustomerCode").val(),
        strCustomerRole: $("#CustomerRole").val()
    };
}
function CMS120() {
    return {
        strContractCode: $("#ContractCode").val()
    };
}
function CMS130() {
    return {
        strContractCode: $("#ContractCode").val(),
        strOCC: $("#OCC").val()
    };
}
function CMS140() {
    return {
        strContractCode: $("#ContractCode").val(),
        strOCC: $("#OCC").val()
    };
}
function CMS150() {
    return {
        ContractCode: $("#ContractCode").val(),
        ServiceTypeCode: $("#ServiceTypeCode").val(),
        OCC:$("#OCC").val()
    };
}
function CMS160() {
    return {
        strContractCode: $("#ContractCode").val(),
        strOCC: $("#OCC").val()
    };
}
function CMS190() {
    return {
        strContractCode: $("#ContractCode").val(),
        strServiceTypeCode: $("#ServiceTypeCode").val()
    };
}
function CMS200() {
    return {
        strContractCode: $("#ContractCode").val(),
        strServiceTypeCode: $("#ServiceTypeCode").val()
    };
}
function CMS280() {
    return {
        strSiteCode: $("#SiteCode").val()
    };
}

/*------------------------- CMS060 -----------------------*/
/*--------------------------------------------------------*/
function CMS060() {
    $("#dlgTest").OpenCMS060Dialog($("#CallerScreen").val());
    return "dialog";
}
function CMS060Response(result) {
    objRetrun = result;
    $("#dlgTest").CloseDialog();


}
/*--------------------------------------------------------*/

/*------------------------- CMS131 -----------------------*/
/*--------------------------------------------------------*/
function CMS131() {
    $("#dlgTest").OpenCMS131Dialog($("#CallerScreen").val());
    return "dialog";
}
function CMS131Object() {
    return { ContractCode:  $("#ContractCode").val() , // "N0000005"
        OCC:  $("#OCC").val() // "0001"
    };
}
/*--------------------------------------------------------*/

/*------------------------- CMS170 -----------------------*/
/*--------------------------------------------------------*/
function CMS170() {
    $("#dlgTest").OpenCMS170Dialog($("#CallerScreen").val());
    return "dialog";
}
function CMS170Object() {
    return { 
                bExpTypeHas: true,
                bExpTypeNo: true,
                bProdTypeSale: true,
                bProdTypeAlarm: false,
                bInstTypeGen: true,
                bInstTypeMonitoring: true,
                bInstTypeMat: true
           };
}
/*--------------------------------------------------------*/

/*------------------------- CMS210 -----------------------*/
/*--------------------------------------------------------*/
function CMS210() {
    $("#dlgTest").OpenCMS210Dialog($("#CallerScreen").val());
    return "dialog";
}
function CMS210Object() {
    return {
        ContractCode: $("#ContractCode").val(),
        OCC: $("#OCC").val(),
        ContractTargetCode: $("#ContractTargetCode").val(),
        ServiceTypeCode: $("#ServiceTypeCode").val(),
        MATargetContractCode: $("#MATargetContractCode").val(),
        ProductCode: $("#ProductCode").val()
    };
}
/*--------------------------------------------------------*/

/*------------------------- CMS220 -----------------------*/
/*--------------------------------------------------------*/
function CMS220() {
    $("#dlgTest").OpenCMS220Dialog($("#CallerScreen").val());
    return "dialog";
}
function CMS220Object() {
    return {
        "ContractCode": "N0000008",
        "OCC": "0001",
        "ContractTargetCode": "C0000147",
        "PurchaserCustCode": "C0000147",
        "RealCustomerCode": "C0000032",
        "SiteCode": "S0000059-0003",
        "Mode": "Customer"
    };
}
/*--------------------------------------------------------*/

/*------------------------- CMS250 -----------------------*/
/*--------------------------------------------------------*/
function CMS250() {
    $("#dlgTest").OpenCMS250Dialog($("#CallerScreen").val());
    return "dialog";
}
function CMS250Object() {
    return {
        "bExistCustOnlyFlag": true
    };
}
function CMS250Response(result) {
    objRetrun = result;
    $("#dlgTest").CloseDialog();
}
/*--------------------------------------------------------*/

/*------------------------- CMS260 -----------------------*/
/*--------------------------------------------------------*/
function CMS260() {
    $("#dlgTest").OpenCMS260Dialog($("#CallerScreen").val());
    return "dialog";
}
function CMS260Object() {
    return {
        "strRealCustomerCode": $("#CustomerCode").val() //"C0000014"
    };
}
function CMS260Response(result) {
    objRetrun = result;
    $("#dlgTest").CloseDialog();
}
/*--------------------------------------------------------*/

/*------------------------- CMS261 -----------------------*/
/*--------------------------------------------------------*/
function CMS261() {
    $("#dlgTest").OpenCMS261Dialog($("#CallerScreen").val());
    return "dialog";
}
function CTS261Object() {
    return { strProjectCode: "P0000013" };
}
/*--------------------------------------------------------*/

/*------------------------- CMS270 -----------------------*/
/*--------------------------------------------------------*/
function CMS270() {
    $("#dlgTest").OpenCMS270Dialog($("#CallerScreen").val());
    return "dialog";
}
function CMS270Response(result) {
    objRetrun = result;
    $("#dlgTest").CloseDialog();
}
/*--------------------------------------------------------*/

/*------------------------- CMS290 -----------------------*/
/*--------------------------------------------------------*/
function CMS290() {
    $("#dlgTest").OpenCMS290Dialog($("#CallerScreen").val());
    return "dialog";
}
/*--------------------------------------------------------*/

/*------------------------- CMS300 -----------------------*/
/*--------------------------------------------------------*/
function CMS300() {
    $("#dlgTest").OpenCMS300Dialog($("#CallerScreen").val());
    return "dialog";
}
function CMS300Object() {
    return {
        ContractCode: $("#ContractCode").val(),
        OCC: "",
        CSCustCode: "",
        RCCustCode: "",
        SiteCode: "",
        ServiceTypeCode: $("#ServiceTypeCode").val()
    };
}
/*--------------------------------------------------------*/

/*------------------------- CMS310 -----------------------*/
/*--------------------------------------------------------*/
function CMS310() {
    $("#dlgTest").OpenCMS310Dialog($("#CallerScreen").val());
    return "dialog";
}
function CMS310Response(result) {
    objRetrun = result;
    $("#dlgTest").CloseDialog();
}
/*--------------------------------------------------------*/

/*------------------------- CMS410 -----------------------*/
/*--------------------------------------------------------*/
function CMS410() {
    return {
        BillingTargetCode: $("#BillingTargetCode").val()
    };
}
/*--------------------------------------------------------*/

/*------------------------- CMS420 -----------------------*/
/*--------------------------------------------------------*/
function CMS420() {
    return {
        ContractCode : $("#ContractCode").val(),
        BillingOCC : $("#OCC").val()
    };
}
/*--------------------------------------------------------*/                                                                                                                                                                                                                                                        

/*------------------------- CMS421 -----------------------*/
/*--------------------------------------------------------*/
function CMS421() {
    return {
        ContractCode : $("#ContractCode").val(),
        BillingOCC : $("#OCC").val()
    };
}
/*--------------------------------------------------------*/  

/*------------------------- CMS422 -----------------------*/
/*--------------------------------------------------------*/

function CMS422() {
    $("#dlgTest").OpenCMS422Dialog($("#CallerScreen").val());
    return "dialog";
}
function CMS422Object() {
    return {
              ContractCode : $("#ContractCode").val(),
              BillingOCC : $("#OCC").val()
            };
}
/*--------------------------------------------------------*/  

/*------------------------- CMS450 -----------------------*/
/*--------------------------------------------------------*/

function CMS450() {
    return {
              ContractCode : $("#ContractCode").val(),
              BillingOCC: $("#OCC").val()              
            };
}
/*--------------------------------------------------------*/  

/*------------------------- CMS430 -----------------------*/
/*--------------------------------------------------------*/

function CMS430() {
    return {
              ContractCode : $("#ContractCode").val()              
            };
}
/*--------------------------------------------------------*/  

/*------------------------- CMS450 -----------------------*/
/*--------------------------------------------------------*/

function CMS450() {
    return {
              ContractCode : $("#ContractCode").val(),
              BillingOCC: $("#ContractCode").val()              
            };
}
/*--------------------------------------------------------*/  

/*------------------------- CMS470 -----------------------*/
/*--------------------------------------------------------*/

function CMS470() {
    $("#dlgTest").OpenCMS470Dialog($("#CallerScreen").val());
    return "dialog";
}
function CMS470Object() {
    return {
              ContractCode : $("#ContractCode").val()              
            };
}
/*--------------------------------------------------------*/  
/*------------------------- MAS030 -----------------------*/
/*--------------------------------------------------------*/
function MAS030() {
    $("#dlgTest").OpenMAS030Dialog($("#CallerScreen").val());
    return "dialog";
}
function MAS030Object() {
    return {
                BillingClientCode: "000001",
                CustTypeCode: "0", //0:Juristic
                CompanyTypeCode: "01",
                IDNo: "123456789",
                NameEN: "ASSI",
                NameLC: "เอเอสเอสไอ",
                BranchNameEN: "Asoke",
                BranchNameLC: "อโศก",
                AddressEN: "338 Exchange Tower, Bangkok, Thailand",
                AddressLC: "338 อาคารเอ๊กซ์เช้น, กรุงเทพ, ประเทศไทย",
                RegionCode: "PT",
                BusinessTypeCode: "001",
                PhoneNo: "026637099"
            };
}
function MAS030Response(result) {
    objRetrun = result;
    $("#dlgTest").CloseDialog();
}
/*--------------------------------------------------------*/

/*------------------------- MAS040 -----------------------*/
/*--------------------------------------------------------*/
function MAS040() {
    $("#dlgTest").OpenMAS040Dialog($("#CallerScreen").val());
    return "dialog";
}
/*--------------------------------------------------------*/

/*------------------------- MAS050 -----------------------*/
/*--------------------------------------------------------*/
function MAS050() {
    $("#dlgTest").OpenMAS050Dialog($("#CallerScreen").val());
    return "dialog";
}
/*--------------------------------------------------------*/



function CTS010() {
    return {
        SubObjectID: $("#Mode").val(),
        QuotationTargetCode: $("#QuotationTargetCode").val()
    };
}
function CTS020() {
    return {
        SubObjectID: $("#Mode").val(),
        QuotationTargetCode: $("#QuotationTargetCode").val()
    };
}
function CTS051() {
    return {
        ContractCode: $("#ContractCode").val()
    };
}
function CTS052() {
    return {
        ContractCode: $("#ContractCode").val()
    };
}
function CTS053() {
    return {
        ContractCode: $("#ContractCode").val()
    };
}
function CTS054() {
    return {
        ContractCode: $("#ContractCode").val()
    };
}
function CTS055() {
    return {
        ContractCode: $("#ContractCode").val()
    };
}
function CTS061() {
    return {
        ContractCode: $("#ContractCode").val()
    };
}
function CTS062() {
    return {
        ContractCode: $("#ContractCode").val()
    };
}
function CTS070() {
    return {
        ContractCode: $("#ContractCode").val()
    };
}
function CTS080() {
    return {
        ContractCode: $("#ContractCode").val()
    };
}
function CTS090() {
    return {
        ContractCode: $("#ContractCode").val()
    };
}
function CTS100() {
    return {
        ContractCode: $("#ContractCode").val()
    };
}
function CTS110() {
    return {
        ContractCode: $("#ContractCode").val()
    };
}
function CTS120() {
    return {
        strContractCode: $("#ContractCode").val()
    };
}
function CTS130() {
    return {
        ContractCode: $("#ContractCode").val()
    };
}
function CTS140() {
    return {
        contractCode: $("#ContractCode").val()
    };
}
function CTS160() {
    return {
        ContractCode: $("#ContractCode").val()
    };
}
function CTS220() {
    return {
        ContractCode: $("#ContractCode").val()
    };
}
function CTS240() {
    return {
        strProjectCode: $("#strProjectCode").val()
    };
}
function CTS260() {
    return {
        strProjectCode: $("#strProjectCode").val()
    };
}
/*------------------------- CTS261 -----------------------*/
/*--------------------------------------------------------*/
function CTS261() {
    $("#dlgTest").OpenCTS261Dialog($("#CallerScreen").val());
    return "dialog";
}
function CTS261Response(result) {
    objRetrun = result;
    $('#dlgTest').CloseDialog();

    alert(objRetrun);
}
/*--------------------------------------------------------*/

function CTS150() {
    return {
        strContractCode: $("#ContractCode").val()
    };
}

function CTS300() {
    var paramObj = {
        strIncidentRelevantType: $("#IncedentRelevantType").val(),
        strIncidentRelevantCode: $("#IncedentRelevantCode").val()
    };
    return paramObj;
}

function CTS320() {
    return {
        strIncidentRelevantType: $("#IncedentRelevantType").val(),
        strIncidentRelevantCode: $("#IncedentRelevantCode").val()
    };
}

function CTS330() {
    return {
        strIncidentID: $("#IncidentID").val()
    };
}

function CTS350() {
    return {
        strARRelevantType: $("#ARRelevantType").val(),
        strARRelevantCode: $("#ARRelevantCode").val()
    };
}

function CTS370() {
    return {
        strARRelevantType: $("#ARRelevantType").val(),
        strARRelevantCode: $("#ARRelevantCode").val()
    };
}

function CTS380() {
    return {
        pRequestNo: $("#RequestNo").val(),
    };
}

/*------------------------- QUS010 -----------------------*/
/*--------------------------------------------------------*/
function QUS010() {
    if ($("#Mode").val() == 1) {
        return {
            ViewMode: $("#Mode").val()
        };
    }
    else {
        $("#dlgTest").OpenQUS010Dialog($("#CallerScreen").val());
        return "dialog";
    }
}
function QUS010Object() {
    return {};
}
/*--------------------------------------------------------*/

/*------------------------- QUS011 -----------------------*/
/*--------------------------------------------------------*/
function QUS011() {
    $("#dlgTest").OpenQUS011Dialog($("#CallerScreen").val());
    return "dialog";
}
function QUS011Object() {
    return {
        QuotationTargetCode: "Q0620022",
        Alphabet: "AA",
        HideQuotationTarget: false
    };
}
/*--------------------------------------------------------*/

/*------------------------- QUS012 -----------------------*/
/*--------------------------------------------------------*/
function QUS012() {
    $("#dlgTest").OpenQUS012Dialog($("#CallerScreen").val());
    return "dialog";
}
function QUS012Object() {
    return {
        QuotationTargetCode: $("#QuotationTargetCode").val(),
        Alphabet: "AA",
        HideQuotationTarget: false
    };
}
/*--------------------------------------------------------*/

/*------------------------- QUS040 -----------------------*/
/*--------------------------------------------------------*/
function QUS040() {
    $("#dlgTest").OpenQUS040Dialog($("#CallerScreen").val());
    return "dialog";
}
function QUS040Object() {
    return {};
}
function QUS040Response(result) {
    objRetrun = result;
    $('#dlgTest').CloseDialog();

    alert(objRetrun);
}
/*--------------------------------------------------------*/

/*------------------------- QUS050 -----------------------*/
/*--------------------------------------------------------*/
function QUS050() {
    $("#dlgTest").OpenQUS050Dialog($("#CallerScreen").val());
    return "dialog";
}
function QUS050Object() {
    return {
        ScreenID: "QUS020"
    };
}
function QUS050Response(dsImportData) {
    alert(dsImportData);
}
/*--------------------------------------------------------*/

/*--------------------------------------------------------*/

//======================== BLS030 ============================
function BLS030() {
    return {
        ContractProjectCodeShort: $("#ContractCode").val()
        ,BillingTargetCode: $("#BillingTargetCode").val()
    };
}
//===========================================================
//======================== BLS040 ============================
function BLS040() {
    return {
        ContractProjectCodeShort: $("#ContractCode").val()
        ,BillingOCC: $("#OCC").val()
    };
}
//===========================================================

/*------------------------- BLS031 -----------------------*/
/*--------------------------------------------------------*/
function BLS031() {
    $("#dlgTest").OpenBLS031Dialog($("#CallerScreen").val());
    return "dialog";
}


/*--------------------------------------------------------*/


/*------------------------- BLS032 -----------------------*/
/*--------------------------------------------------------*/
function BLS032() {
    $("#dlgTest").OpenBLS032Dialog($("#CallerScreen").val());
    return "dialog";
}

function BLS032Object() {
    var obj = { "ContractCode": $("#ContractCode").val()
            , "BillingClientCode": "00000000004-001"
            , "BillingClientNameEN": "Siripoj"
            , "BillingClientNameLC": "ศิริพจน์"
    };
    return { "doCredit": obj };
}
/*--------------------------------------------------------*/



/*------------------------- BLS071 -----------------------*/
/*--------------------------------------------------------*/
function BLS071() {
    $("#dlgTest").OpenBLS071Dialog($("#CallerScreen").val());
    return "dialog";
}

function BLS071Object() {
          var objArr = new Array();   
          var objDetail ;
          var obj = {
            "BillingTargetCode": $("#BillingTargetCode").val(),
            "FullNameEN": "CSI ASIA",
            "FullNameLC": "ซีเอสไอ เอเชีย"
           };

          objDetail = {
                ContractCode: "N0002700112",
                BillingOCC: "01",
                BillingDetailNo: 1
          };
          objArr.push(objDetail);
          objDetail = {
                ContractCode: "N0002700112",
                BillingOCC: "05",
                BillingDetailNo: 1
          };
          objArr.push(objDetail);
          
          var objBLS071Dev = {
                doBillingTarget: obj,
                doSelectedBillingDetailList: objArr
           };
   
         return  objBLS071Dev;
            //return doBillingtraget;
}
/*--------------------------------------------------------*/


function ISS010() {
    return {
        strContractProjectCode: $("#ContractCode").val()
    };
}

function ISS050() {
    return {
        strContractProjectCode: $("#ContractCode").val()
    };
}

function ISS030() {
    return {
        strContractCode: $("#ContractCode").val()
    };
}

function ISS060() {
    return {
        strContractCode: $("#ContractCode").val()
    };
}

function ISS070() {
    return {
        strContractProjectCode: $("#ContractCode").val()
    };
}

function ISS090() {
    return {
        strContractProjectCode: $("#ContractCode").val()
    };
}


/*-------------------------INVENTORY-------------------------------*/
//function IVS140(){

//}

//function IVS150(){

//}

//function IVS160(){

//}

//function IVS170(){

//}

//function IVS180(){

//}

//function IVS190(){

//}


//------------------Income--------------
function ICS080() {
    return {
        SubObjectID: $("#Mode").val()
    };
}
function ICS033() {
    $("#dlgTest").OpenICS033Dialog($("#CallerScreen").val());
    return "dialog";
}
 
function ICS033Object() {
    var arr1 = new Array();
   
            var obj1 = {
                BillingTargetCode : "BillingTargetCode",
                BillingClientNameEN : "BillingClientNameEN",
                BillingClientNameLC : "BillingClientNameLC",
                BillingClientAddressEN : "BillingClientAddressEN",
                BillingClientAddressLC : "BillingClientAddressLC",
                BillingClientTelNo : "BillingClientTelNo",
                ContactPersonName : "ContactPersonName",
                UnpaidAmount : 999
	}

            arr1.push(obj1);

    var objICS033 = {
 
                BillingOfficeName : "BillingOfficeName",
                BillingOfficeCode : "BillingOfficeCode",
                strMode : 1,

        	    doBillingTargetDebtSummary: arr1,

	            InvoiceNo : "InvoiceNo",
                InvoiceOCC : 0,
                BillingCode : "BillingCode"
    };

    return objICS033;

}
