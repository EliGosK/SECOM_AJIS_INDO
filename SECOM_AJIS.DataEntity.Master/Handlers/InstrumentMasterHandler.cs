using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util.ConstantValue;
using CSI.WindsorHelper;

namespace SECOM_AJIS.DataEntity.Master
{
    public class InstrumentMasterHandler : BizMADataEntities, IInstrumentMasterHandler
    {
        public override List<dtInstrument> GetInstrument(string pchvInstrumentCode, string pchvInstrumentName, string pchrLineUpTypeCode, string pchvFieldName)
        {
            return base.GetInstrument(pchvInstrumentCode, pchvInstrumentName, pchrLineUpTypeCode, pchvFieldName);
        }
        
        
        public List<tbm_Instrument> GetIntrumentList(List<tbm_Instrument> insLst)
        {
            try
            {
                return this.GetIntrumentList(
                    SECOM_AJIS.Common.Util.CommonUtil.ConvertToXml_Store<tbm_Instrument>(insLst, "InstrumentCode"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /* --- Merge --- */
        public List<doInstrumentData> GetInstrumentListForView(List<tbm_Instrument> instLst)
        {
            try
            {
                List<doInstrumentData> lst = this.GetInstrumentListForView(
                        SECOM_AJIS.Common.Util.CommonUtil.ConvertToXml_Store<tbm_Instrument>(instLst, "InstrumentCode"),
                        MiscType.C_LINE_UP_TYPE, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);

                if (lst.Count > 0)
                    CommonUtil.MappingObjectLanguage<doInstrumentData>(lst);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /* ------------- */

        public void InstrumentListMapping(InstrumentMappingList instLst)
        {
            try
            {
                if (instLst == null)
                    return;

                List<tbm_Instrument> lst = this.GetIntrumentList(instLst.GetInstrumentList());
                if (lst.Count > 0)
                    instLst.SetInstrumentValue(lst);
            }
            catch (Exception)
            {
                throw;
            }

        }


        public List<doParentGeneralInstrument> GetParentGeneralInstrumentList(List<doParentGeneralInstrument> insLst)
        {
            try
            {
                List<doParentGeneralInstrument> lst = this.GetParentGeneralInstrumentList(
                    SECOM_AJIS.Common.Util.CommonUtil.ConvertToXml_Store<doParentGeneralInstrument>(insLst, "InstrumentCode"),
                    MiscType.C_LINE_UP_TYPE,
                    InstrumentType.C_INST_TYPE_GENERAL,
                    ExpansionType.C_EXPANSION_TYPE_PARENT,
                    LineUpType.C_LINE_UP_TYPE_STOP_SALE,
                    LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE);

                if (lst.Count > 0)
                    CommonUtil.MappingObjectLanguage<doParentGeneralInstrument>(lst);

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<doMonitoringInstrument> GetMonitoringInstrumentList(List<doMonitoringInstrument> instLst)
        {
            try
            {
                return GetMonitoringInstrumentList(
                    SECOM_AJIS.Common.Util.CommonUtil.ConvertToXml_Store<doMonitoringInstrument>(instLst, "InstrumentCode"),
                    MiscType.C_LINE_UP_TYPE,
                    InstrumentType.C_INST_TYPE_MONITOR);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public List<doInstrumentData> GetInstrumentDataForSearch(doInstrumentSearchCondition cond)
        {
            try
            {
                List<doInstrumentData> lst = null;
                if (cond != null)
                {
                    if (cond.InstrumentFlag == null)
                        cond.InstrumentFlag = new List<int?>();
                    for (int idx = cond.InstrumentFlag.Count; idx < 2; idx++)
                    {
                        cond.InstrumentFlag.Add(null);
                    }
                    if (cond.ExpansionType == null)
                        cond.ExpansionType = new List<string>();
                    for (int idx = cond.ExpansionType.Count; idx < 2; idx++)
                    {
                        cond.ExpansionType.Add(null);
                    }
                    if (cond.InstrumentType == null)
                        cond.InstrumentType = new List<string>();
                    for (int idx = cond.InstrumentType.Count; idx < 3; idx++)
                    {
                        cond.InstrumentType.Add(null);
                    }

                   
                    lst = this.GetInstrumentDataForSearch(cond.InstrumentCode,
                                                            cond.InstrumentName,
                                                            cond.Maker,
                                                            cond.SupplierCode,
                                                            cond.LineUpTypeCode,
                                                            cond.InstrumentFlag[0],
                                                            cond.InstrumentFlag[1],
                                                            cond.ExpansionType[0],
                                                            cond.ExpansionType[1], 
                                                            cond.SaleFlag, 
                                                            cond.RentalFlag,
                                                            cond.InstrumentType[0],
                                                            cond.InstrumentType[1],
                                                            cond.InstrumentType[2],                                                           
                                                            SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_LINE_UP_TYPE,
                                                            CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US
                                                            );

                }
                if (lst == null)
                    lst = new List<doInstrumentData>();
                lst = CommonUtil.ConvertObjectbyLanguage<doInstrumentData, doInstrumentData>(lst, "LineUpTypeName");

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<tbm_Instrument> InsertInstrument(tbm_Instrument instrument) {

            //Add by Jutarat A. on 14082013
            if (String.IsNullOrEmpty(instrument.InstrumentCode) == false)
                instrument.InstrumentCode = instrument.InstrumentCode.Trim();
            //End Add

            List<tbm_Instrument> insertList = new List<tbm_Instrument>();
            insertList.Add(instrument);
            string xml = CommonUtil.ConvertToXml_Store(insertList);
            List<tbm_Instrument> listInserted = base.InsertInstrument(xml);

            if (insertList == null || insertList.Count == 0) {
                return null;
            } else {
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_INSTRUMENT,
                    TableData = CommonUtil.ConvertToXml(insertList)
                };
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return insertList;
        }
        
        public List<tbm_Instrument> UpdateInstrument(tbm_Instrument instrument) {
            List<DateTime?> updateDateList = base.GetInstrumentUpdateDate(instrument.InstrumentCode);
            if (updateDateList == null || updateDateList.Count == 0 || updateDateList[0] == null
                || instrument.UpdateDate == null || !instrument.UpdateDate.HasValue)
            {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "UpdateDate" });
            }
            if (updateDateList[0].Value.CompareTo(instrument.UpdateDate.Value) != 0) {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019, new string[] { instrument.InstrumentCode });
            }

            instrument.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            instrument.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

            //8.5	Update instrument data
            List<tbm_Instrument> updateList = new List<tbm_Instrument>();
            updateList.Add(instrument);
            string xml = CommonUtil.ConvertToXml_Store(updateList);
            List<tbm_Instrument> listUpdated = base.UpdateInstrument(xml);

            if (listUpdated == null || listUpdated.Count == 0) {
                return null;
            } else {
                doTransactionLog logData = new doTransactionLog() {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_INSTRUMENT,
                    TableData = CommonUtil.ConvertToXml(listUpdated)
                };
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return listUpdated;
        }

        public List<tbm_InstrumentExpansion> InsertInstrumentExpansion(tbm_InstrumentExpansion doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbm_InstrumentExpansion> insertList = base.InsertInstrumentExpansion(
                                                    doInsert.InstrumentCode
                                                    , doInsert.ChildInstrumentCode
                                                    , doInsert.CreateDate
                                                    , doInsert.CreateBy
                                                    , doInsert.UpdateDate
                                                    , doInsert.UpdateBy);

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_INSTRUMENT_EXPANSION;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<tbm_InstrumentExpansion> DeleteInstrumentExpansion(tbm_InstrumentExpansion doDelete)
        {
            try
            {
                //Check whether this record is the most updated data
                List<tbm_InstrumentExpansion> rList = this.GetTbm_InstrumentExpansion(doDelete.InstrumentCode, doDelete.ChildInstrumentCode);
                if (rList == null || rList.Count == 0 || rList[0] == null
                    || rList[0].UpdateDate == null || !rList[0].UpdateDate.HasValue
                    || doDelete.UpdateDate == null || !doDelete.UpdateDate.HasValue)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "UpdateDate" });
                }
                if (DateTime.Compare(rList[0].UpdateDate.Value, doDelete.UpdateDate.Value) != 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019, new string[] { doDelete.InstrumentCode });
                }

                //set updateDate and updateBy
                //doDelete.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                //doDelete.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbm_InstrumentExpansion> deletedList = base.DeleteInstrumentExpansion(doDelete.ChildInstrumentCode
                                                                                            , doDelete.InstrumentCode);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_INSTRUMENT_EXPANSION;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<doInstrumentExpansion> GetInstrumentExpansion(string InstrumentCode)
        {
            try
            {
                List<doInstrumentExpansion> lst = base.GetInstrumentExpansion(InstrumentCode, MiscType.C_LINE_UP_TYPE);

                if (lst == null)
                    lst = new List<doInstrumentExpansion>();
                else
                    CommonUtil.MappingObjectLanguage<doInstrumentExpansion>(lst);

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<doInstrumentExpansion> GetChildInstrument(string ChildInstrumentCode)
        {
            try
            {
                List<doInstrumentExpansion> lst = base.GetChildInstrument(ChildInstrumentCode, MiscType.C_LINE_UP_TYPE, ExpansionType.C_EXPANSION_TYPE_CHILD);

                if (lst == null)
                    lst = new List<doInstrumentExpansion>();
                else
                    CommonUtil.MappingObjectLanguage<doInstrumentExpansion>(lst);

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<tbm_InstrumentExpansion> CheckExistInstrumentExpansion(string parentInstCode, List<doInstrumentExpansion> childInstList)
        {
            List<tbm_InstrumentExpansion> result = new List<tbm_InstrumentExpansion>();
            try
            {
                StringBuilder sbChildInstCode = new StringBuilder("");
                foreach (doInstrumentExpansion inst in childInstList)
                {
                    sbChildInstCode.AppendFormat("\'{0}\',", inst.InstrumentCode);
                }

                result = base.CheckExistInstrumentExpansion(parentInstCode, sbChildInstCode.ToString());
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public bool CheckExistParentInstrument(string parentInstCode)
        {
            bool result = false;
            try
            {
                List<int?> lst = base.CheckExistParentInstrument(parentInstCode, ExpansionType.C_EXPANSION_TYPE_PARENT);

                if (lst.Count > 0)
                {
                    if (lst[0] == 1)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public List<doParentInstrument> GetParentInstrument(string pchvInstrumentCode)
        {
             return base.GetParentInstrument(pchvInstrumentCode, ExpansionType.C_EXPANSION_TYPE_PARENT);
        }
    }
}