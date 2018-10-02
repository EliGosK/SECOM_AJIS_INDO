using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Master
{
    public interface ISubcontractorMasterHandler
    {
        /// <summary>
        /// Retrieve subcontractor data according to search condition (sp_MA_GetSubcontractor)
        /// </summary>
        /// <param name="SubcontractorCode"></param>
        /// <param name="CoCompanyCode"></param>
        /// <param name="InstallationTeam"></param>
        /// <param name="SubcontractorName"></param>
        /// <returns></returns>
        List<doSubcontractor> GetSubcontractor(string SubcontractorCode, string CoCompanyCode, string InstallationTeam, string SubcontractorName);

        /// <summary>
        /// Getting subcontractor detail (sp_MA_GetSubcontractorDetail)
        /// </summary>
        /// <param name="SubcontractorCode"></param>
        /// <returns></returns>
        List<doSubcontractor> GetSubcontractorDetail(string SubcontractorCode);

        /// <summary>
        /// Insert Subcontractor data (sp_MA_InsertSubcontractor)
        /// </summary>
        /// <param name="subcontractor"></param>
        /// <returns></returns>
        List<doSubcontractor> InsertSubcontractor(doSubcontractor subcontractor);

        /// <summary>
        /// Update subcontractor information (sp_MA_UpdateSubcontractor)
        /// </summary>
        /// <param name="subcontractor"></param>
        /// <returns></returns>
        List<doSubcontractor> UpdateSubcontractor(doSubcontractor subcontractor);

    }
}
