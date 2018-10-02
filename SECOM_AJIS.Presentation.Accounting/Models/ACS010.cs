using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Presentation.Accounting.Models.MetaData;
using SECOM_AJIS.DataEntity.Accounting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace SECOM_AJIS.Presentation.Accounting.Models
{
    public class ACS010_ScreenParameter : ScreenParameter
    {
        public string DocumentNo { get; set; }
        public string DocumentCode { get; set; }
        public string GenerateHQCode { get; set; }
        public string TargetPeriodFrom { get; set; }
        public string TargetPeriodTo { get; set; }
        public string FileName { get; set; }
        public Stream StreamReport { get; set; }

    }

    /// <summary>
    /// Parameter for search contract by code
    /// </summary>
    [MetadataType(typeof(ACS010_SearchCriteiria_MetaData))]
    public class ACS010_Search : doSearchInfoCondition
    {
    }

    /// <summary>
    /// Parameter for search contract by code
    /// </summary>
    [MetadataType(typeof(ACS010_GenerateCriteiria_MetaData))]
    public class ACS010_Generate : doGenerateInfoCondition
    {
    }
}
namespace SECOM_AJIS.Presentation.Accounting.Models.MetaData
{
    public class ACS010_SearchCriteiria_MetaData
    {
        public string SearchDocumentCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG8003, Module = MessageUtil.MODULE_ACCOUNTING)]
        public DateTime SearchTargetFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG8003, Module = MessageUtil.MODULE_ACCOUNTING)]
        public DateTime SearchTargetTo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG8003, Module = MessageUtil.MODULE_ACCOUNTING)]
        public DateTime SearchGenerateFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG8003, Module = MessageUtil.MODULE_ACCOUNTING)]
        public DateTime SearchGenerateTo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG8003, Module = MessageUtil.MODULE_ACCOUNTING)]
        public string SearchMonth { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG8003, Module = MessageUtil.MODULE_ACCOUNTING)]
        public string SearchYear { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG8003, Module = MessageUtil.MODULE_ACCOUNTING)]
        public string SearchDocumentNo { get; set; }
    }

    public class ACS010_GenerateCriteiria_MetaData
    {
        public string documentCode { get; set; }
        public DateTime generateTargetFrom { get; set; }
        public DateTime generateTargetTo { get; set; }

    }
}
