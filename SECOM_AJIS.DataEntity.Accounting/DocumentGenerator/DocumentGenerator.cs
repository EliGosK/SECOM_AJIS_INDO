using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Accounting.Handlers;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Accounting
{
    public class DocumentGenerator
    {
        public static doGenerateDocumentResult Generate(DocumentContext context)
        {
            AccountingHandler handler = new AccountingHandler();
            doGenerateDocumentResult result = new doGenerateDocumentResult();
            try
            {

                Assembly assembly = Assembly.Load(context.AssemblyName + ", Version=0.0.0.0, PublicKeyToken=null,Culture=neutral");

                Type[] typelist = assembly.GetTypes();
                string s = "";
                for(int i = 0; i< typelist.Length-1; i++)
                {
                    s = s + typelist[i].FullName + ",";
                }


                Type type = assembly.GetType(context.TypeName, true);
                IDocumentGenerator generator = (IDocumentGenerator)Activator.CreateInstance(type);
                result = generator.GenerateDocument(context);
                result.DocumentContext = context;

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public interface IDocumentGenerator
    {
        doGenerateDocumentResult GenerateDocument(DocumentContext context);
    }

    public class ACC002_AgingReport : IDocumentGenerator
    {
        public doGenerateDocumentResult GenerateDocument(DocumentContext context)
        {
            AccountingHandler handler = new AccountingHandler();
            return handler.ACC002_AgingReport(context);
        }
    }

    public class ACC004_PaymentInfo : IDocumentGenerator
    {
        public doGenerateDocumentResult GenerateDocument(DocumentContext context)
        {
            AccountingHandler handler = new AccountingHandler();
            return handler.ACC004_PaymentInfo(context);
        }
    }

    public class ACC006_PaymentMatchingInfo : IDocumentGenerator
    {
        public doGenerateDocumentResult GenerateDocument(DocumentContext context)
        {
            AccountingHandler handler = new AccountingHandler();
            return handler.ACC006_PaymentMatchingInfo(context);
        }
    }

    public class ACC008_VATReport : IDocumentGenerator
    {
        public doGenerateDocumentResult GenerateDocument(DocumentContext context)
        {
            AccountingHandler handler = new AccountingHandler();
            return handler.ACC008_VATReport(context);
        }
    }

    public class ACC009_NewOperationRentalReport : IDocumentGenerator
    {
        public doGenerateDocumentResult GenerateDocument(DocumentContext context)
        {
            AccountingHandler handler = new AccountingHandler();
            return handler.ACC009_NewOperationRentalReport(context);
        }
    }

    public class ACC010_NewOperationSalesReport : IDocumentGenerator
    {
        public doGenerateDocumentResult GenerateDocument(DocumentContext context)
        {
            AccountingHandler handler = new AccountingHandler();
            return handler.ACC010_NewOperationSalesReport(context);
        }
    }

    public class ACC011_PaymentListReport : IDocumentGenerator
    {
        public doGenerateDocumentResult GenerateDocument(DocumentContext context)
        {
            AccountingHandler handler = new AccountingHandler();
            return handler.ACC011_PaymentListReport(context);
        }
    }

    public class ACC012_NotPaymentListReport : IDocumentGenerator
    {
        public doGenerateDocumentResult GenerateDocument(DocumentContext context)
        {
            AccountingHandler handler = new AccountingHandler();
            return handler.ACC012_NotPaymentListReport(context);
        }
    }

    public class DocumentContext
    {
        public string DocumentCode { get; set; }
        public string DocumentGeneratorName { get; set; }
        public string DocumentTimingTypeDesc { get; set; }
        public DateTime? GenerateDate { get; set; }
        public DateTime? TargetPeriodFrom { get; set; }
        public DateTime? TargetPeriodTo { get; set; }
        public string UserID { get; set; }
        public string UserHQCode { get; set; }

        public string AssemblyName
        {
            get
            {
                if (string.IsNullOrEmpty(this.DocumentGeneratorName) == false)
                    return this.DocumentGeneratorName.Split(',')[0];
                return string.Empty;
            }
        }
        public string TypeName
        {
            get
            {
                if (string.IsNullOrEmpty(this.DocumentGeneratorName) == false)
                    return this.AssemblyName + '.' + this.DocumentGeneratorName.Split(',')[1];
                return string.Empty;
            }
        }
    }

    public class doGenerateDocumentResult
    {
        public DocumentContext DocumentContext { get; set; }
        public bool ErrorFlag { get; set; }
        public int Total { get; set; }
        public int Complete { get; set; }
        public int Failed { get; set; }
        public MessageUtil.MessageList ErrorCode{ get; set; }
        public string ResultDocumentNoList { get; set; }
    }
}
