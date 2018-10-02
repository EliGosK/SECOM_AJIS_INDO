using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Transactions;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util.ConstantValue;
using CSI.WindsorHelper;

namespace SECOM_AJIS.DataEntity.Master
{
    public class BillingClientMasterHandler : BizMADataEntities, IBillingClientMasterHandler
    {

        //public List<tbm_SafetyStock> InsertSafetyStock(tbm_SafetyStock data)
        //{
        //    try
        //    {
        //        List<tbm_SafetyStock> insertList = new List<tbm_SafetyStock>();
        //        insertList.Add(data);

        //        string xml = CommonUtil.ConvertToXml_Store(insertList);

        //        List<tbm_SafetyStock> listInserted = null;
        //        using (TransactionScope t = new TransactionScope())
        //        {
        //            listInserted = base.InsertSafetyStock(xml);

        //            doTransactionLog logData = new doTransactionLog()
        //            {
        //                TransactionType = doTransactionLog.eTransactionType.Insert,
        //                TableName = TableName.C_TBL_NAME_SAFETY_STOCK,
        //                TableData = CommonUtil.ConvertToXml(listInserted)
        //            };

        //            ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
        //            hand.WriteTransactionLog(logData);

        //            t.Complete();
        //        }

        //        return listInserted;

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //}

        /// <summary>
        /// Update billing client data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<tbm_BillingClient> UpdateBillingClient(tbm_BillingClient data)
        {
            try
            {
                List<tbm_BillingClient> updateList = new List<tbm_BillingClient>();
                updateList.Add(data);

                string xml = CommonUtil.ConvertToXml_Store(updateList);

                List<tbm_BillingClient> listUpdated = null;
                using (TransactionScope t = new TransactionScope())
                {
                    listUpdated = base.UpdateBillingClient(xml);

                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_BILLING_CLIENT,
                        TableData = CommonUtil.ConvertToXml(listUpdated)
                    };
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);

                    t.Complete();
                }

                return updateList;

            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}