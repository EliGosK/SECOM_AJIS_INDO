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
    public class SubcontractorMasterHandler : BizMADataEntities, ISubcontractorMasterHandler
    {

        public List<doSubcontractor> InsertSubcontractor(doSubcontractor data)
        {
            try
            {
                //================= Change DDS SubContractorCode generate by companycode + teamcode =========
                //ICommonHandler commonHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                //List<doRunningNo> runningNoList = commonHand.GetNextRunningCode(NameCode.C_NAME_CODE_SUBCONTRACTOR_CODE);
                //if (runningNoList.Count <= 0 || string.IsNullOrEmpty(runningNoList[0].RunningNo))
                //{
                //    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3131);
                //}
                //data.SubContractorCode = runningNoList[0].RunningNo;
                //============================================================================================

                List<doSubcontractor> insertList = new List<doSubcontractor>();
                insertList.Add(data);

                string xml = CommonUtil.ConvertToXml_Store(insertList);

                List<doSubcontractor> listInserted = null;
                using (TransactionScope t = new TransactionScope())
                {
                    listInserted = base.InsertSubcontractor(xml);

                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Insert,
                        TableName = TableName.C_TBL_NAME_SUBCONTRACTOR,
                        TableData = CommonUtil.ConvertToXml(listInserted)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);

                    t.Complete();
                }

                return listInserted;

            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public List<doSubcontractor> UpdateSubcontractor(doSubcontractor data)
        {
            try
            {
                
                List<doSubcontractor> curSubcontractor = GetSubcontractor(data.SubContractorCode, null, null, null);
                if (!(curSubcontractor.Count > 0) || DateTime.Compare(curSubcontractor[0].UpdateDate.Value, data.UpdateDate.Value) != 0)
                {
                    throw new Exception(MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019, null).Message);
                }
                data.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                data.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                List<doSubcontractor> updateList = new List<doSubcontractor>();
                updateList.Add(data);

                string xml = CommonUtil.ConvertToXml_Store(updateList);

                List<doSubcontractor> listUpdated = null;
                using (TransactionScope t = new TransactionScope())
                {
                    listUpdated = base.UpdateSubcontractor(xml);

                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_SUBCONTRACTOR,
                        TableData = CommonUtil.ConvertToXml(listUpdated)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);

                    t.Complete();
                }

                return listUpdated;

            }
            catch (Exception)
            {
                throw;
            }

        }

        public List<doSubcontractor> GetSubcontractor(string SubcontractorCode, string CoCompanyCode, string InstallationTeam, string SubcontractorName)
        {
            try
            {
                List<doSubcontractor> result = base.GetSubcontractor(SubcontractorCode, CoCompanyCode, InstallationTeam, SubcontractorName);

                // Misc Mapping  
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                miscMapping.AddMiscType(result.ToArray());
                handlerCommon.MiscTypeMappingList(miscMapping);
                
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}