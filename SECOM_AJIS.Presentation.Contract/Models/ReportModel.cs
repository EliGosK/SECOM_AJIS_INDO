using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using System.IO;
using SECOM_AJIS.DataEntity.Contract;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// DO for stored data of Change Notice report
    /// </summary>
    public class RPTChangeNoticeDo : DataEntity.Contract.RPTChangeNoticeDo { }

    /// <summary>
    /// DO for stored data of Change Memorandum report
    /// </summary>
    public class RPTChangeMemoDo : DataEntity.Contract.RPTChangeMemoDo { }

    /// <summary>
    /// DO for stored data of Contract report
    /// </summary>
    public class RPTContractReportDo : DataEntity.Contract.RPTContractReportDo { }

    /// <summary>
    /// DO for stored data of Confirm Current Instrument Memorandum report
    /// </summary>
    public class RPTConfirmCurrInstMemoDo : DataEntity.Contract.RPTConfirmCurrInstMemoDo { }

    /// <summary>
    /// DO for stored data of Cancel Contract Memorandum report
    /// </summary>
    public class RPTCancelContractMemoDo : DataEntity.Contract.RPTCancelContractMemoDo { }

    /// <summary>
    /// DO for stored data of Cancel Contract Memorandum Detail report
    /// </summary>
    public class RPTCancelContractMemoDetailDo : DataEntity.Contract.RPTCancelContractMemoDetailDo { }

    /// <summary>
    /// DO for stored data of Change Fee Memorandum report
    /// </summary>
    public class RPTChangeFeeMemoDo : DataEntity.Contract.RPTChangeFeeMemoDo { }

    /// <summary>
    /// DO for stored data of Cover Letter report
    /// </summary>
    public class RPTCoverLetterDo : DataEntity.Contract.RPTCoverLetterDo { }

    /// <summary>
    /// DO for stored data of Instrument Detail report
    /// </summary>
    public class RPTInstrumentDetailDo : DataEntity.Contract.RPTInstrumentDetailDo { }

    /// <summary>
    /// DO for stored data of Maintenance CheckUp Slip report
    /// </summary>
    public class RPTMACheckupSlipDo : DataEntity.Contract.RPTMACheckupSlipDo { }

    /// <summary>
    /// DO for stored data of Instrument CheckUp report
    /// </summary>
    public class RPTInstrumentCheckupDo : DataEntity.Contract.RPTInstrumentCheckupDo { }

    /// <summary>
    /// DO for stored data of Slip report
    /// </summary>
    public class RPTSlipReport : DataEntity.Contract.RPTSlipReport { }

    /// <summary>
    /// DO for Start/Resume Memorandum report
    /// </summary>
    public class RPTStartResumeMemoDo : DataEntity.Contract.RPTStartResumeMemoDo { }

    /// <summary>
    /// DO for CTR101
    /// </summary>
    public class RPTMaintenanceCheckSheetDo : DataEntity.Contract.doMaintenanceCheckSheetReportView { }
}
