using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;

using CSI.WindsorHelper;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Web.Mvc;


namespace SECOM_AJIS.DataEntity.Installation
{

    #region ISP099_ManageBillingDetailMonthlyFee

    public class ISP099_ReprintInstallationReport : IBatchProcess
    {
        public doBatchProcessResult WorkProcess(string strUserId, DateTime dtBatchDate)
        {
            IBatchProcessHandler batchHand = ServiceContainer.GetService<IBatchProcessHandler>() as IBatchProcessHandler;
            IInstallationDocumentHandler insHand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;
            int failed = 0;
            int total = 0;
            try
            {
                CommonUtil.SetTransDataForJobScheduler(strUserId, dtBatchDate);

                List<tbt_InstallationReprint> lstSlipNo = insHand.GetTbt_InstallationReprint();
                total = lstSlipNo.Count;
                foreach (tbt_InstallationReprint slip in lstSlipNo)
                {
                    try
                    {
                        if (slip.DocumentCode == DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_RENTAL
                            || slip.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_INSTALL_SLIP
                            || slip.DocumentCode == DocumentCode.C_DOCUMENT_CODE_REMOVAL_INSTALL_SLIP
                            || slip.DocumentCode == DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_SALE)
                        {
                            Stream tmp = insHand.CreateInstallationReport(slip.SlipNo, slip.DocumentCode);

                            if (tmp != null)
                            {
                                batchHand.InsertTbt_BatchLog(dtBatchDate, "99", string.Format("Reprint installation report completed: {0}: {1} {2}", slip.Id, slip.DocumentCode, slip.SlipNo), false, strUserId);
                                tmp.Close();
                            }
                            else
                            {
                                batchHand.InsertTbt_BatchLog(dtBatchDate, "99", string.Format("Reprint installation report completed without file: {0}: {1} {2}", slip.Id, slip.DocumentCode, slip.SlipNo), true, strUserId);
                                failed++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        batchHand.InsertTbt_BatchLog(dtBatchDate, "99", string.Format("Reprint installation report error: {0}: {1} {2} - {3} {4} {5}", slip.Id, slip.DocumentCode, slip.SlipNo, ex.Message, ex.ToString(), ex.StackTrace), true, strUserId);
                        failed++;
                    }
                }
            }
            catch (Exception ex)
            {
                batchHand.InsertTbt_BatchLog(dtBatchDate, "99", string.Format("Reprint installation report error: {0} {1} {2}", ex.Message, ex.ToString(), ex.StackTrace), true, strUserId);
                failed = total = 0;
            }
            
            return new doBatchProcessResult()
            {
                Result = FlagType.C_FLAG_ON,
                BatchStatus = (failed == 0 ? BatchStatus.C_BATCH_STATUS_SUCCEEDED : BatchStatus.C_BATCH_STATUS_FAILED),
                Total = total,
                Complete = total - failed,
                Failed = failed,
                ErrorMessage = null,
                BatchUser = strUserId,
                BatchDate = dtBatchDate
            };
        }
    }

    #endregion
 

}
