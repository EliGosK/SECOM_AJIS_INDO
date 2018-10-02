
//*********************************
// Create by: Teerapong
// Create date: 2/Nov/2011
// Update date: 2/Nov/2011
//*********************************



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Transactions;
using SECOM_AJIS.Presentation.Installation.Models;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Quotation;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;

using System.ComponentModel;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportSource;
using CrystalDecisions.CrystalReports.Engine;
using SECOM_AJIS.Common.Models.EmailTemplates;


using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;   

namespace SECOM_AJIS.Presentation.Installation.Controllers
{
    public partial class InstallationController : BaseController
    {

        public ActionResult ISS061_Authority(ISS061_ScreenParameter param)
        {
            // permission
            

            ObjectResultData res = new ObjectResultData();      
            // parameter
            //ISS061_Parameter param = new ISS061_Parameter();


            return InitialScreenEnvironment<ISS080_ScreenParameter>("ISS061", param, res);
            
        }

        [Initialize("ISS061")]
        public ActionResult ISS061()
        {
            //ISS061_ScreenParameter param = new ISS061_ScreenParameter();
            //ISS061_ScreenParameter param = GetScreenObject<ISS061_ScreenParameter>();
            //if(param != null)
            //{
            //    ViewBag.ContractProjectCode = param.ContractCodeShort;
            //}
            return View();
        }

        public ActionResult ISS061_CompleteInstallRental(string RentalContractCode)
        {
            ObjectResultData res = new ObjectResultData();            
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (CommonUtil.IsNullOrEmpty(RentalContractCode))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                            "ISS061",
                                            MessageUtil.MODULE_INSTALLATION,
                                            MessageUtil.MessageList.MSG0007,
                                            new string[] { "lblRentalContractCode" },
                                            new string[] { "RentalContractCode" });
                        return Json(res);
                    }

                    IInstallationHandler iHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                    iHandler.Temp_CompleteInstallation_Rental(RentalContractCode);

                    scope.Complete();
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        public ActionResult ISS061_CompleteInstallSale(string SaleContractCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (CommonUtil.IsNullOrEmpty(SaleContractCode))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                            "ISS061",
                                            MessageUtil.MODULE_INSTALLATION,
                                            MessageUtil.MessageList.MSG0007,
                                            new string[] { "lblRentalContractCode" },
                                            new string[] { "RentalContractCode" });
                        return Json(res);
                    }

                    IInstallationHandler iHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                    iHandler.Temp_CompleteInstallation_Sale(SaleContractCode);

                    scope.Complete();
                    return Json(res);                    
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
        

    }
}
