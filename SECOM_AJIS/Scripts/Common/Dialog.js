/* --- Global Variable --- */
//var objCMS060;
//var objCMS250;
//var objCMS260;
//var objCMS270;
//var objCMS310;
//var objCMS131;
//var objCMS210;
//var objCMS220;
var objCMS170;
var objCMS290;


/* --- CMS020_SearchBar ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. CMS020Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. CMS020Response(result) in Caller Screen ----------------- */
/* ----------- 3. CMS020Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
//$.fn.OpenCMS020Dialog = function (caller_id) {

//    var ctrl = $(this);

//    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Common/CMS020_SearchBar", "", function (result) {
//        if (result != undefined) {
//            var event = {
//                Close: function () {
//                    ctrl.CloseDialog();
//                }
//            };

//            var open_event = function (event, ui) {
//                if (typeof (CMS020Initial) == "function")
//                    CMS020Initial();
//            };

//            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);


//        }

//    });


//}


/* --- CMS060 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. CMS060Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. CMS060Response(result) in Caller Screen ----------------- */
/* ----------- 3. CMS060Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenCMS060Dialog = function (caller_id) {

    var ctrl = $(this);
    var objParam;
    var param;
    if (typeof (CMS060Object) == "function") {
        objParam = CMS060Object();
        param = { "EmailList": objParam.EmailList };
    }
    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Common/CMS060", param, function (result) {
        if (result != undefined) {
            var event = {
                OK: function () {
                    //ctrl.CloseDialog();
                }
                ,
                Cancel: function () {
                    ctrl.CloseDialog();
                }
            };

            var open_event = function (event, ui) {
                if (typeof (CMS060Initial) == "function")
                    CMS060Initial(param);
            };

            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);


        }

    });


}


/* --- CMS250 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. CMS250Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. CMS250Response(result) in Caller Screen ----------------- */
/* ----------- 3. CMS250Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenCMS250Dialog = function (caller_id) {


    var ctrl = $(this);

    var objParam = "";
    var param = { "bExistCustOnlyFlag": false }; // default

    if (typeof (CMS250Object) == "function") {
        objParam = CMS250Object();
        param = { "bExistCustOnlyFlag": objParam.bExistCustOnlyFlag };
    }

    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Common/CMS250", param, function (result) {
        if (result != undefined) {
            var event = {
                Cancel: function () {
                    ctrl.CloseDialog();
                }
            };

            var open_event = function (event, ui) {
                if (typeof (CMS250Initial) == "function")
                    CMS250Initial();
            };


            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);


        }

    });
}

/* --- CMS260 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. CMS260Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. CMS260Response(result) in Caller Screen ----------------- */
/* ----------- 3. CMS260Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenCMS260Dialog = function (caller_id) {

    var ctrl = $(this);

    var objParam = "";
    var param;
    if (typeof (CMS260Object) == "function") {
        objParam = CMS260Object();
        param = { "strRealCustomerCode": objParam.strRealCustomerCode };
    }

    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Common/CMS260", param, function (result) {
        if (result != undefined) {
            var event = {
                Cancel: function () {
                    ctrl.CloseDialog();
                }
            };

            var open_event = function (event, ui) {
                if (typeof (CMS260Initial) == "function")
                    CMS260Initial();
            };




            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);


        }

    });
}


/* --- CMS270 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. CMS270Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. CMS270Response(result) in Caller Screen ----------------- */
/* ----------- 3. CMS270Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenCMS270Dialog = function (caller_id) {

    var ctrl = $(this);

    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Common/CMS270", "", function (result) {
        if (result != undefined) {
            var event = {
                Cancel: function () {
                    ctrl.CloseDialog();
                }
            };

            var open_event = function (event, ui) {
                if (typeof (CMS270Initial) == "function")
                    CMS270Initial();
            };


            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);


        }

    });
}
/* --- CMS310 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. CMS310Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. CMS310Response(result) in Caller Screen ----------------- */
/* ----------- 3. CMS310Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenCMS310Dialog = function (caller_id) {

    var ctrl = $(this);

    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Common/CMS310", "", function (result) {
        if (result != undefined) {
            var event = {
                Cancel: function () {
                    ctrl.CloseDialog();
                }
            };

            var open_event = function (event, ui) {
                if (typeof (CMS310Initial) == "function")
                    CMS310Initial();
            };


            ctrl.OpenPopupDialog(result, "", 1100, 600, event, null, open_event);


        }

    });
}


/* --- CMS131 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. CMS131Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. CMS131Response(result) in Caller Screen ----------------- */
/* ----------- 3. CMS131Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenCMS131Dialog = function (caller_id) {


    // TODO : (Narupon) จุดนี้มักจะพลาด ต้องเช็คให้ดีว่า controller ทีเรียกรับ paramer หรือไม่ , controler CMS131 ไม่รับจึงส่ง null 

    var ctrl = $(this);

    var objParam = "";
    var param;
    if (typeof (CMS131Object) == "function") {
        objParam = CMS131Object();
        param = { "strContractCode": objParam.ContractCode, "strOCC": objParam.OCC };
    }


    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Common/CMS131", param, function (result) {
        if (result != undefined) {
            var event = {
                Close: function () {
                    ctrl.CloseDialog();
                }
            };

            var open_event = function (event, ui) {
                if (typeof (CMS131Initial) == "function")
                    CMS131Initial();
            };



            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);


        }

    });
}


/* --- CMS210 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. CMS210Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. CMS210Response(result) in Caller Screen ----------------- */
/* ----------- 3. CMS210Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenCMS210Dialog = function (caller_id) {


    // TODO : (Narupon) จุดนี้มักจะพลาด ต้องเช็คให้ดีว่า controller ทีเรียกรับ paramer หรือไม่ , controler CMS131 ไม่รับจึงส่ง null 

    var ctrl = $(this);

    var objParam = "";
    var param;
    if (typeof (CMS210Object) == "function") {
        objParam = CMS210Object();

        param =
                    {
                        ContractCode: objParam.ContractCode,
                        OCC: objParam.OCC,
                        ContractTargetCode: objParam.ContractTargetCode,
                        ServiceTypeCode: objParam.ServiceTypeCode,
                        MATargetContractCode: objParam.MATargetContractCode,
                        ProductCode: objParam.ProductCode
                    };
    }

    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Common/CMS210", param, function (result) {
        if (result != undefined) {
            var event = {
                Close: function () {
                    ctrl.CloseDialog();
                }
            };

            var open_event = function (event, ui) {
                if (typeof (CMS210Initial) == "function")
                    CMS210Initial();
            };




            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);


        }

    });
}


/* --- CMS220 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. CMS220Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. CMS220Response(result) in Caller Screen ----------------- */
/* ----------- 3. CMS220Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenCMS220Dialog = function (caller_id) {


    // TODO : (Narupon) จุดนี้มักจะพลาด ต้องเช็คให้ดีว่า controller ทีเรียกรับ paramer หรือไม่ , controler CMS131 ไม่รับจึงส่ง null 

    var ctrl = $(this);

    var param = "";
    if (typeof (CMS220Object) == "function") {
        param = CMS220Object();
    }

    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Common/CMS220", param, function (result) {
        if (result != undefined) {
            var event = {
                Close: function () {
                    ctrl.CloseDialog();
                }
            };

            var open_event = function (event, ui) {
                if (typeof (CMS220Initial) == "function")
                    CMS220Initial();
            };



            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);


        }

    });
}


/* --- CMS300 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. CMS300Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. CMS300Response(result) in Caller Screen ----------------- */
/* ----------- 3. CMS300Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenCMS300Dialog = function (caller_id) {
    /// <summary>Create QUS011 Dialog</summary>

    var ctrl = $(this);

    /* --- Set Parameters --- */
    var objParam = "";
    if (typeof (CMS300Object) == "function") {
        var objParam = CMS300Object();
    }

    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Common/CMS300", objParam, function (result) {
        if (result != undefined) {
            /* --- Button Events --- */
            /* --------------------- */
            var event = {
                Close: function (data) {
                    ctrl.CloseDialog();
                }
            };
            /* --------------------- */

            /* --- Event when open Dialog --- */
            /* ------------------------------ */
            var open_event = function (event, ui) {
                if (typeof (CMS300Initial) == "function")
                    CMS300Initial();
            };
            /* ------------------------------ */

            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);
        }
    });
}


/* --- CMS170 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. CMS170Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. CMS170Response(result) in Caller Screen ----------------- */
/* ----------- 3. CMS170Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenCMS170Dialog = function (caller_id) {
    //    var objParam = "";
    //    if (typeof (CMS170Object) == "function")
    //        objParam = CMS1700bject();

    //    var event = {
    //        Cancel: function () {
    //            $(this).CloseDialog();
    //        }
    //    };

    //    var open_event = function (event, ui) {
    //        if (typeof (CMS170Initial) == "function")
    //            CMS170Initial();
    //    };

    //    $(this).OpenPopupDialog("/Common/CMS170", objParam, 900, 600, event, null, open_event);

    var ctrl = $(this);

    var cms170_input = "";
    var cms170_object = CallBackCallerFunction("CMS170Object");
    if (cms170_object != undefined) {

        cms170_input = {
            bExpTypeHas: cms170_object.bExpTypeHas,
            bExpTypeNo: cms170_object.bExpTypeNo,
            bProdTypeSale: cms170_object.bProdTypeSale,
            bProdTypeAlarm: cms170_object.bProdTypeAlarm,
            bInstTypeGen: cms170_object.bInstTypeGen,
            bInstTypeMonitoring: cms170_object.bInstTypeMonitoring,
            bInstTypeMat: cms170_object.bInstTypeMat
        }
    }

    //    cms170_input = {
    //        bExpTypeHas: true,
    //        bExpTypeNo: true,
    //        bProdTypeSale: true,
    //        bProdTypeAlarm: false,
    //        bInstTypeGen: true,
    //        bInstTypeMonitoring: true,
    //        bInstTypeMat: true
    //    }

    /* --- Get Data from Server --- */
    /* ---------------------------- */
    var objParam = { param: { InputData: cms170_input} };

    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Common/CMS170", objParam, function (result) {
        if (result != undefined) {
            /* --- Button Events --- */
            /* --------------------- */
            var event = {
                Cancel: function () {
                    ctrl.CloseDialog();
                }
            };
            /* --------------------- */

            /* --- Event when open Dialog --- */
            /* ------------------------------ */
            var open_event = function (event, ui) {
                if (typeof (CMS170Initial) == "function")
                    CMS170Initial();
            };
            /* ------------------------------ */

            ctrl.OpenPopupDialog(result, "", 1065, 600, event, null, open_event);
        }
    });
    /* ---------------------------- */
}

$.fn.OpenCMS290Dialog = function (caller_id) {
//    var objParam = "";
//    if (typeof (CMS290Object) == "function")
//        objParam = CMS290bject();

//    var event = {
//        Cancel: function () {
//            $(this).CloseDialog();
//        }
//    };

//    var open_event = function (event, ui) {
//        if (typeof (CMS290Initial) == "function")
//            CMS290Initial();
//    };

//    $(this).OpenPopupDialog("/Common/CMS290", objParam, 900, 600, event, null, open_event);

    var ctrl = $(this);

    var cms290_input = "";

    /* --- Get Data from Server --- */
    /* ---------------------------- */
    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Common/CMS290", cms290_input, function (result) {
        if (result != undefined) {
            /* --- Button Events --- */
            /* --------------------- */
            var event = {
                Cancel: function () {
                    ctrl.CloseDialog();
                }
            };
            /* --------------------- */

            /* --- Event when open Dialog --- */
            /* ------------------------------ */
            var open_event = function (event, ui) {
                if (typeof (CMS290Initial) == "function")
                    CMS290Initial();
            };
            /* ------------------------------ */

            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);
        }
    });
    /* ---------------------------- */
}


/* --- CMS421 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. CMS421Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. CMS421Response(result) in Caller Screen ----------------- */
/* ----------- 3. CMS421Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenCMS421Dialog = function (caller_id) {
    /// <summary>Create QUS011 Dialog</summary>

    var ctrl = $(this);

    /* --- Set Parameters --- */
    var objParam = "";
    if (typeof (CMS421Object) == "function") {
        var objParam = CMS421Object();
    }

    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Common/CMS421", objParam, function (result) {
        if (result != undefined) {
            /* --- Button Events --- */
            /* --------------------- */
            var event = {
                Close: function (data) {
                    ctrl.CloseDialog();
                }
            };
            /* --------------------- */

            /* --- Event when open Dialog --- */
            /* ------------------------------ */
            var open_event = function (event, ui) {
                if (typeof (CMS421Initial) == "function")
                    CMS421Initial();
            };
            /* ------------------------------ */

            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);
        }
    });
}

/* --- CMS422 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. CMS422Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. CMS422Response(result) in Caller Screen ----------------- */
/* ----------- 3. CMS422Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenCMS422Dialog = function (caller_id) {
    /// <summary>Create QUS011 Dialog</summary>

    var ctrl = $(this);

    /* --- Set Parameters --- */
    var objParam = "";
    if (typeof (CMS422Object) == "function") {
        var objParam = CMS422Object();
    }

    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Common/CMS422", objParam, function (result) {
        if (result != undefined) {
            /* --- Button Events --- */
            /* --------------------- */
            var event = {
                Close: function (data) {
                    ctrl.CloseDialog();
                }
            };
            /* --------------------- */

            /* --- Event when open Dialog --- */
            /* ------------------------------ */
            var open_event = function (event, ui) {
                if (typeof (CMS422Initial) == "function")
                    CMS422Initial();
            };
            /* ------------------------------ */

            ctrl.OpenPopupDialog(result, "", 1000, 600, event, null, open_event);
        }
    });
}

/* --- CMS450 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. CMS450Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. CMS450Response(result) in Caller Screen ----------------- */
/* ----------- 3. CMS450Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenCMS450Dialog = function (caller_id) {
    /// <summary>Create QUS011 Dialog</summary>

    var ctrl = $(this);
    var objParam;
    /* --- Set Parameters --- */
    var objParam = "";
    if (typeof (CMS450Object) == "function") {
        objParam = CMS450Object();
    }

    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Common/CMS450", objParam, function (result) {
        if (result != undefined) {
            /* --- Button Events --- */
            /* --------------------- */
            var event = {
                Close: function (data) {
                    ctrl.CloseDialog();
                }
            };
            /* --------------------- */

            /* --- Event when open Dialog --- */
            /* ------------------------------ */
            var open_event = function (event, ui) {
                if (typeof (CMS450Initial) == "function")
                    CMS450Initial();
            };
            /* ------------------------------ */

            ctrl.OpenPopupDialog(result, "", 1100, 600, event, null, open_event);
        }
    });
}

/* --- CMS430 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. CMS430Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. CMS430Response(result) in Caller Screen ----------------- */
/* ----------- 3. CMS430Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenCMS430Dialog = function (caller_id) {
    /// <summary>Create QUS011 Dialog</summary>

    var ctrl = $(this);
    var objParam;
    /* --- Set Parameters --- */
    var objParam = "";
    if (typeof (CMS430Object) == "function") {
        objParam = CMS430Object();
    }

    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Common/CMS430", objParam, function (result) {
        if (result != undefined) {
            /* --- Button Events --- */
            /* --------------------- */
            var event = {
                Close: function (data) {
                    ctrl.CloseDialog();
                }
            };
            /* --------------------- */

            /* --- Event when open Dialog --- */
            /* ------------------------------ */
            var open_event = function (event, ui) {
                if (typeof (CMS430Initial) == "function")
                    CMS430Initial();
            };
            /* ------------------------------ */

            ctrl.OpenPopupDialog(result, "", 1100, 600, event, null, open_event);
        }
    });
}

/* --- CMS470 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. CMS470Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. CMS470Response(result) in Caller Screen ----------------- */
/* ----------- 3. CMS470Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenCMS470Dialog = function (caller_id) {
    /// <summary>Create QUS011 Dialog</summary>

    var ctrl = $(this);
    var objParam;
    /* --- Set Parameters --- */
    var objParam = "";
    if (typeof (CMS470Object) == "function") {
        objParam = CMS470Object();
    }

    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Common/CMS470", objParam, function (result) {
        if (result != undefined) {
            /* --- Button Events --- */
            /* --------------------- */
            var event = {
                Close: function (data) {
                    ctrl.CloseDialog();
                }
            };
            /* --------------------- */

            /* --- Event when open Dialog --- */
            /* ------------------------------ */
            var open_event = function (event, ui) {
                if (typeof (CMS470Initial) == "function")
                    CMS470Initial();
            };
            /* ------------------------------ */

            ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);
        }
    });
}


/* --- CMS026 ------------------------------------------------------------- */
/* -------- you must declare function ------------------------------------- */
/* ----------- 1. CMS026Initial() in Popup Screen (Set Button name etc.) -- */
/* ----------- 2. CMS026Response(result) in Caller Screen ----------------- */
/* ----------- 3. CMS026Object() in Caller Screen ------------------------- */
/* ------------------------------------------------------------------------ */
$.fn.OpenCMS026Dialog = function (caller_id) {
    /// <summary>Create CMS026 Dialog</summary>
    /// <param name="caller_id" type="string">Caller Screen ID</param>

    var param = "";
    if (typeof (CMS026Object) == "function") {
        param = CMS026Object();
    }

    var ctrl = $(this);
    ajax_method.CallPopupScreenControllerWithAuthority(caller_id, "/Common/CMS026", param, function (result) {
        /* --- Event when open Dialog --- */
        var event = {
            Close: function () {
                ctrl.CloseDialog();
            }
        };

        /* --- Event when open Dialog --- */
        var open_event = function (event, ui) {
            if (typeof (CMS026Initial) == "function")
                CMS026Initial();
        };

        ctrl.OpenPopupDialog(result, "", 900, 600, event, null, open_event);
    });

}
/* ------------------------------------------------------------------------ */