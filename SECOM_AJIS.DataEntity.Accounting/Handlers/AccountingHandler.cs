using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SECOM_AJIS.Common.Util;
using System.Collections.Specialized;
using SECOM_AJIS.Common.Models;
using System.Data.Objects;
using System.IO;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Accounting.Handlers
{
    public partial class AccountingHandler
    {
        private List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }

        private string CurrencyName(string currencyType)
        {
            string txt = string.Empty;
            if (this.Currencies != null)
            {
                DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == currencyType);
                if (curr != null)
                {
                    txt = curr.ValueDisplayEN;
                }
            }
            return txt;
        }

        ACDataEntities db = new ACDataEntities();

        public List<doAccountingDocumentList> getAccountingReportList()
        {
            var list = db.tbm_AccountingDocument;
            List<doAccountingDocumentList> documentList = new List<doAccountingDocumentList>();
            if (CommonUtil.IsNullOrEmpty(list) || list.Count() < 1)
            {
                return documentList;
            }

            foreach (var item in list)
            {
                doAccountingDocumentList acd = new doAccountingDocumentList();
                acd.DocumentCode = item.DocumentCode;
                acd.DocumentNameEN = item.DocumentNameEN;
                acd.DocumentNameENWithCode = item.DocumentCode + ":" + item.DocumentNameEN;
                acd.DocumentTimingType = item.DocumentTimingType;


                documentList.Add(acd);
            }

            return documentList;
        }

        public doDocumentTiming getDocumentTimingByDocumentCode(string documentCode)
        {
            var acd = db.tbm_AccountingDocument.Where(e => e.DocumentCode == documentCode).First();
            string documetTimingType = acd.DocumentTimingType;
            return getDocumentTimingNameByTimingType(documetTimingType);
        }

        public doDocumentTiming getDocumentTimingNameByTimingType(string timingType)
        {
            doDocumentTiming dt = null;
            var acc = db.GetAccountingConfig(AccountingConfig.C_ACC_CONFIG_GROUP_DOCUMENT_TIMING_TYPE, timingType).ToList();
            if (acc.Count() < 1)
            {
                return dt;
            }

            dt = new doDocumentTiming();
            dt.TimingType = timingType;
            dt.TimingTypeName = acc.First().ConfigValue;

            return dt;
        }

        public List<dtAccountingDocument> GetAccountingDocument(string documentCode)
        {
            return db.GetAccountingDocument(documentCode).ToList();

        }

        public List<tbt_AccountingDocumentList> Insert_tbt_AccountingDocumentList(string xmlTbt_AccountingDocumentList)
        {
            return db.InsertTbt_AccountingDocumentList(xmlTbt_AccountingDocumentList).ToList();
        }

        public dtAccountingConfig getAccountingConfig(string configGroup, string configName)
        {
            dtAccountingConfig dt = null;
            var acc = db.GetAccountingConfig(configGroup, configName).ToList();
            if (acc.Count() < 1)
            {
                return dt;
            }

            dt = new dtAccountingConfig();
            dt = acc.First();

            return dt;
        }

        public List<dtAccountingConfig> getAccountingConfigList(string configGroup, string configName)
        {
            return db.GetAccountingConfig(configGroup, configName).ToList();
           
        }

        public List<dtAccountingDocumentList> GetAccountingDocumentList(doSearchInfoCondition searchCondition, string HQCode)
        {

            return db.GetAccountingDocumentList(searchCondition.SearchDocumentCode
                                                , searchCondition.SearchDocumentNo
                                                , searchCondition.SearchTargetFrom
                                                , searchCondition.SearchTargetTo
                                                , searchCondition.SearchGenerateFrom
                                                , searchCondition.SearchGenerateTo
                                                , HQCode
                                                , searchCondition.SearchYear
                                                , searchCondition.SearchMonth
                                                ).ToList();
        }
    }
}
