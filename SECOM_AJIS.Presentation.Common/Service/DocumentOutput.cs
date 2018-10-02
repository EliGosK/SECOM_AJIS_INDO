using CSI.WindsorHelper;
using SECOM_AJIS.DataEntity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM_AJIS.Presentation.Common.Service
{
    // Create by Jirawat Jannet on 2016-12-06
    public class DocumentOutput
    {
        public static sp_CM_GetTbs_DocumentOutput_Result getDocumentData(string documentCode, Nullable<int> documentCodeSeq, Nullable<System.DateTime> startDay)
        {
            ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            var datas = handler.GetTbs_DocumentOutput(documentCode, documentCodeSeq, startDay);

            if (datas != null && datas.Count > 0)
                return datas.First();
            else return null;
        }

        public static string getCompanyName(string documentCode, Nullable<int> documentCodeSeq, Nullable<System.DateTime> startDay)
        {
            sp_CM_GetTbs_DocumentOutput_Result data = getDocumentData(documentCode, documentCodeSeq, startDay);
            if (data != null)
            {
                return data.CompanyName;
            }
            else
                return string.Empty;
        }
        public static string getImageSignaturePath(string documentCode, Nullable<int> documentCodeSeq, Nullable<System.DateTime> startDay)
        {
            sp_CM_GetTbs_DocumentOutput_Result data = getDocumentData(documentCode, documentCodeSeq, startDay);
            if (data != null)
            {
                return data.ImageSignaturePath;
            }
            else
                return string.Empty;
        }
    }
}
