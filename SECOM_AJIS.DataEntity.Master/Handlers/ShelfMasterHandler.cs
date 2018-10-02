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
    public class ShelfMasterHandler : BizMADataEntities, IShelfMasterHandler
    {
      
        public List<tbm_Shelf> InsertShelf(tbm_Shelf shelf)
        {
            try
            {
                ICommonHandler commonHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (shelf != null)
                {
                    shelf.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    shelf.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    shelf.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    shelf.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }
                List<tbm_Shelf> insertList = new List<tbm_Shelf>();
                insertList.Add(shelf);
                string xml = CommonUtil.ConvertToXml_Store(insertList);

                using (TransactionScope t = new TransactionScope())
                {

                    // Insert tbm_Subcontractor
                    List<tbm_Shelf> listInserted = base.InsertShelf(xml);

                    // Update transaction log
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Insert,
                        TableName = TableName.C_TBL_NAME_SHELF,
                        TableData = CommonUtil.ConvertToXml(insertList)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);

                    t.Complete();

                }

                return insertList;

            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public List<tbm_Shelf> UpdateShelf(tbm_Shelf shelf)
        {
            //List<doShelf> curShelf = GetShelf(shelf.ShelfNo, null, null, null);
            //if ((curShelf.Count > 0) || DateTime.Compare(curShelf[0].UpdateDate.Value, shelf.UpdateDate.Value) != 0)    
            //{
            //    throw new Exception(MessageUtil.GetMessage(MessageUtil.MODULE_COMMON
            //        , MessageUtil.MessageList.MSG0019, null).Message);
            //}

            shelf.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            shelf.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

            List<tbm_Shelf> updateList = new List<tbm_Shelf>();
            updateList.Add(shelf);
            string xml = CommonUtil.ConvertToXml_Store(updateList);

            using (TransactionScope t = new TransactionScope())
            {

                List<tbm_Shelf> listUpdated = base.UpdateShelf(xml);

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_SHELF,
                    TableData = CommonUtil.ConvertToXml(listUpdated)
                };
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                t.Complete();
            }

            return updateList;
        }

        public bool CheckDuplicateShelf(string ShelfNo)
        {
            try
            {
                List<tbm_Shelf> result = base.CheckDuplicateShelf(ShelfNo);               

                if (result.Count > 0)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}