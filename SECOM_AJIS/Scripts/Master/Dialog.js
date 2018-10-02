var objMAS090;

/* --- MAS030 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. MAS030Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. MAS030Response(result) in Caller Screen ----------------- */
/* ----------- 3. MAS030Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
/* ------------------------------------------------------------------------ */
$.fn.OpenMAS030Dialog = function (caller_id) {
    /// <summary>Create MAS030 Dialog</summary>
    var ctrl = $(this);

    /* --- Set Parameters --- */
    /* ---------------------- */
    var mas030_input = "";
    var mas030_object = CallBackCallerFunction("MAS030Object");
    if (mas030_object != undefined) {

        mas030_input = {
            BillingClientCode: (mas030_object.BillingClientCodeShort ? mas030_object.BillingClientCodeShort : mas030_object.BillingClientCode),
            CustTypeCode: mas030_object.CustTypeCode,
            CompanyTypeCode: mas030_object.CompanyTypeCode,
            IDNo: mas030_object.IDNo,
            NameEN: mas030_object.NameEN,
            NameLC: mas030_object.NameLC,
            BranchNo: mas030_object.BranchNo,
            BranchNameEN: mas030_object.BranchNameEN,
            BranchNameLC: mas030_object.BranchNameLC,
            AddressEN: mas030_object.AddressEN,
            AddressLC: mas030_object.AddressLC,
            RegionCode: mas030_object.RegionCode,
            BusinessTypeCode: mas030_object.BusinessTypeCode,
            PhoneNo: mas030_object.PhoneNo
        };

        ////Mock Data
        //mas030_input = {
        //    BillingClientCode: "000001",
        //    CustTypeCode: "0", //0:Juristic
        //    CompanyTypeCode: "01",
        //    IDNo: "123456789",
        //    NameEN: "ASSI",
        //    NameLC: "เอเอสเอสไอ",
        //    BranchNameEN: "Asoke",
        //    BranchNameLC: "อโศก",
        //    AddressEN: "338 Exchange Tower, Bangkok, Thailand",
        //    AddressLC: "338 อาคารเอ๊กซ์เช้น, กรุงเทพ, ประเทศไทย",
        //    RegionCode: "PT",
        //    BusinessTypeCode: "001",
        //    PhoneNo: "026637099"
        //};
    }

    /* ---------------------- */

    /* --- Get Data from Server --- */
    /* ---------------------------- */
    var objParam = { InputData: mas030_input };

    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Master/MAS030", objParam, function (result) {
        if (result != undefined) {
            /* --- Button Events --- */
            /* --------------------- */
            var event = {
                OK: function () {
                    alert("OK");
                },
                Cancel: function () {
                    ctrl.CloseDialog();
                }
            };
            /* --------------------- */

            /* --- Event when open Dialog --- */
            /* ------------------------------ */
            var open_event = function (event, ui) {
                if (typeof (MAS030Initial) == "function")
                    MAS030Initial();
            };
            /* ------------------------------ */

            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);
        }
    });
    /* ---------------------------- */
}

/* --- MAS040 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. MAS040Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. MAS040Response(result) in Caller Screen ----------------- */
/* ----------- 3. MAS040Object() in Caller Screen ------------------------- */
/* ----------- 4. MAS040ClearPopupSession() in Popup Screen --------------- */
/* ------------------------------------------------------------------------ */
/* ------------------------------------------------------------------------ */
$.fn.OpenMAS040Dialog = function (caller_id) {
    /// <summary>Create MAS040 Dialog</summary>

    var objParam = "";
    if (typeof (MAS040Object) == "function") {
        objParam = {
            doSite: MAS040Object()
        };
    }

    /* --- Get Data from Server --- */
    /* ---------------------------- */
    var ctrl = $(this);
    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Master/MAS040", objParam, function (result) {
        if (result != undefined) {
            var event = {
                OK: function () {
                    $(this).CloseDialog();
                },
                Cancel: function () {
                    $(this).CloseDialog();
                }
            };

            var open_event = function (event, ui) {
                if (typeof (MAS040Initial) == "function")
                    MAS040Initial();
            };

            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);
        }
    });
    /* ---------------------------- */
}
/* ------------------------------------------------------------------------ */

/* --- MAS050 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. MAS050Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. MAS050Response(result) in Caller Screen ----------------- */
/* ----------- 3. MAS050Object() in Caller Screen ------------------------- */
/* ----------- 4. MAS050ClearPopupSession() in Popup Screen --------------- */
/* ------------------------------------------------------------------------ */
/* ------------------------------------------------------------------------ */
$.fn.OpenMAS050Dialog = function (caller_id) {
    /// <summary>Create MAS050 Dialog</summary>

    var objParam = "";
    if (typeof (MAS050Object) == "function")
        objParam = MAS050Object();

    /* --- Get Data from Server --- */
    /* ---------------------------- */
    var ctrl = $(this);
    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Master/MAS050", objParam, function (result) {
        if (result != undefined) {
            var event = {
                OK: function () {
                    $(this).CloseDialog();
                },
                Cancel: function () {
                    $(this).CloseDialog();
                }
            };

            var open_event = function (event, ui) {
                if (typeof (MAS050Initial) == "function")
                    MAS050Initial();
            };

            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);
        }
    });
    /* ---------------------------- */
}
/* ------------------------------------------------------------------------ */

$.fn.OpenMAS090Dialog = function () {
    /// <summary>Create QUS011 Dialog</summary>
    /// <param name="strQuotationTargetCode" type="string">Quotation Target Code</param>
    /// <param name="strAlphabet" type="string">Alphabet</param>
    /// <param name="blnHideQuotationTarget" type="bool">Hide Quotation Target</param>
    objMAS090 = {

};

var event = {
    Close: function (data) {
        $(this).CloseDialog();
    }
};

$(this).OpenPopupDialog("/Master/MAS090", "" /*objMAS090*/, 900, 600, event);
}