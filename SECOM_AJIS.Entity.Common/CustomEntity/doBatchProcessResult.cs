using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Common
{
    public class doBatchProcessResultCommon
    {
        private bool strResult; public bool Result { get { return this.strResult; } set { this.strResult = value; } }
        private string strBatchStatus; public string BatchStatus { get { return this.strBatchStatus; } set { this.strBatchStatus = value; } }
        private int intTotal; public int Total { get { return this.intTotal; } set { this.intTotal = value; } }
        private int intComplete; public int Complete { get { return this.intComplete; } set { this.intComplete = value; } }
        private int intFailed; public int Failed { get { return this.intFailed; } set { this.intFailed = value; } }
        private string strErrorMessage; public string ErrorMessage { get { return this.strErrorMessage; } set { this.strErrorMessage = value; } } 

    }
}
