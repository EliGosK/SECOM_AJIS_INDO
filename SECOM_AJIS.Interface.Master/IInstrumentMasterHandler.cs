using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Master
{
    public interface IInstrumentMasterHandler
    {
        /// <summary>
        /// Get instrument data for search.<br />
        /// - Get instrument data for search.<br />
        /// - Mapping language.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<doInstrumentData> GetInstrumentDataForSearch(doInstrumentSearchCondition cond);

        /// <summary>
        /// Get instrument expansion.<br />
        /// - Get instrument expansion.<br />
        /// - Mapping language.
        /// </summary>
        /// <param name="pchvnInstrumentCode"></param>
        /// <returns></returns>
        List<doInstrumentExpansion> GetInstrumentExpansion(string pchvnInstrumentCode);

        /// <summary>
        /// Get child instrument.<br />
        /// - Get instrument from given instrument code with expansion type Child.<br />
        /// - Mapping language.
        /// </summary>
        /// <param name="pchvnChildInstrumentCode"></param>
        /// <returns></returns>
        List<doInstrumentExpansion> GetChildInstrument(string pchvnChildInstrumentCode);

        /// <summary>
        /// Delete instrument expansion.<br />
        /// - Check update date.<br />
        /// - Delete instrument expansion.<br />
        /// - Write transaction log.
        /// </summary>
        /// <param name="deleteDo"></param>
        /// <returns></returns>
        List<tbm_InstrumentExpansion> DeleteInstrumentExpansion(tbm_InstrumentExpansion deleteDo);

        /// <summary>
        /// Add new instrument expansion.<br />
        /// - Inster instrument expansion.<br />
        /// - Write transaction log.
        /// </summary>
        /// <param name="insertDo"></param>
        /// <returns></returns>
        List<tbm_InstrumentExpansion> InsertInstrumentExpansion(tbm_InstrumentExpansion insertDo);

        int DeleteAllInstrument(string instrumentCode);

        /// <summary>
        /// Update instrument data.<br />
        /// - Check update date.<br />
        /// - Update instrument data.<br />
        /// - Write transaction log.
        /// </summary>
        /// <param name="instrument"></param>
        /// <returns></returns>
        List<tbm_Instrument> UpdateInstrument(tbm_Instrument instrument);

        List<Nullable<bool>> CheckExistInstrument(string pchvInstrumentCode);

        /// <summary>
        /// Check is parent/child instrument already relate.
        /// </summary>
        /// <param name="parentInstCode"></param>
        /// <param name="childInstList"></param>
        /// <returns></returns>
        List<tbm_InstrumentExpansion> CheckExistInstrumentExpansion(String parentInstCode, List<doInstrumentExpansion> childInstList);

        /// <summary>
        /// Get instrument and check expansion type must be Parent.
        /// </summary>
        /// <param name="pchvInstrumentCode"></param>
        /// <returns></returns>
        List<doParentInstrument> GetParentInstrument(string pchvInstrumentCode);

        /// <summary>
        /// Check exist instrument and expansion type is Parent.
        /// </summary>
        /// <param name="parentInstCode"></param>
        /// <returns></returns>
        bool CheckExistParentInstrument(string parentInstCode);

        /// <summary>
        /// Check is given instrument code already set expansion type.
        /// </summary>
        /// <param name="pchvInstrumentCode"></param>
        /// <returns></returns>
        List<Nullable<bool>> CheckExistParentChild(string pchvInstrumentCode);

        /// <summary>
        /// Search instrument.
        /// </summary>
        /// <param name="pchvInstrumentCode"></param>
        /// <param name="pchvInstrumentName"></param>
        /// <param name="pchrLineUpTypeCode"></param>
        /// <param name="pchvFieldName"></param>
        /// <returns></returns>
        List<dtInstrument> GetInstrument(string pchvInstrumentCode, string pchvInstrumentName, string pchrLineUpTypeCode, string pchvFieldName);

        /// <summary>
        /// Get detail of instrument.
        /// </summary>
        /// <param name="pchvInstrumentCode"></param>
        /// <param name="pchvFieldName"></param>
        /// <returns></returns>
        List<dtInstrumentDetail> GetInstrumentDetail(string pchvInstrumentCode, string pchvFieldName, string c_CURRENCY_LOCAL, string c_CURRENCY_US);

        /// <summary>
        /// Add new instrument.
        /// </summary>
        /// <param name="instrument"></param>
        /// <returns></returns>
        List<tbm_Instrument> InsertInstrument(tbm_Instrument instrument);

        /// <summary>
        /// Get list of instrument.
        /// </summary>
        /// <param name="xml_dtIntrumentCode"></param>
        /// <returns></returns>
        List<tbm_Instrument> GetIntrumentList(string xml_dtIntrumentCode);

        /// <summary>
        /// Get list of instrument.
        /// </summary>
        /// <param name="insLst"></param>
        /// <returns></returns>
        List<tbm_Instrument> GetIntrumentList(List<tbm_Instrument> insLst);

        /* --- Merge --- */
        /// <summary>
        /// Get list of instrument and mapping language.
        /// </summary>
        /// <param name="instLst"></param>
        /// <returns></returns>
        List<doInstrumentData> GetInstrumentListForView(List<tbm_Instrument> instLst);
        /* ------------- */

        /// <summary>
        /// Get update date of instrument.
        /// </summary>
        /// <param name="pchvInstrumentCode"></param>
        /// <returns></returns>
        List<Nullable<System.DateTime>> GetInstrumentUpdateDate(string pchvInstrumentCode);

        /// <summary>
        /// Get detail of instrument.
        /// </summary>
        /// <param name="instrumentCode"></param>
        /// <returns></returns>
        List<tbm_Instrument> GetTbm_Instrument(string instrumentCode);

        /// <summary>
        /// Get instrument in given list that is type general and expansion type is parent.
        /// </summary>
        /// <param name="insLst"></param>
        /// <returns></returns>
        List<doParentGeneralInstrument> GetParentGeneralInstrumentList(List<doParentGeneralInstrument> insLst);

        /// <summary>
        /// Get all instrument type monitoring.
        /// </summary>
        /// <param name="insLst"></param>
        /// <returns></returns>
        List<doMonitoringInstrument> GetMonitoringInstrumentList(List<doMonitoringInstrument> instLst);

        /// <summary>
        /// Get list of instrument in given instrumentMapping.
        /// </summary>
        /// <param name="insLst"></param>
        /// <returns></returns>
        void InstrumentListMapping(InstrumentMappingList instLst);

        /// <summary>
        /// Get instrument expansion that parent is strInstrumentCode or child is strChildInstrumentCode.
        /// </summary>
        /// <param name="strInstrumentCode"></param>
        /// <param name="strChildInstrumentCode"></param>
        /// <returns></returns>
        List<tbm_InstrumentExpansion> GetTbm_InstrumentExpansion(string strInstrumentCode, string strChildInstrumentCode);
    }
}
