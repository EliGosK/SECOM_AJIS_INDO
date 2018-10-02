using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Master
{
    public interface IOfficeMasterHandler
    {
        List<tbm_Office> GetTbm_Office();
        List<tbm_Office> GetTbm_Office(string OfficeCode);
        //   List<tbm_Office> GetFunctionSecurity(string pcharC_FUNC_SECURITY_NO);
        List<dtOffice> GetFunctionQuatation(string pchrC_FUNC_QUATATION_NO);
        List<dtOffice> GetFunctionSecurity(string pchrC_FUNC_SECURITY_NO);
        List<dtOffice> GetFunctionSale(string pchrC_FUNC_SALE_NO);
        List<dtAuthorizeOffice> GetAuthorizeOffice(string xml_dtBelonging);

        /// <summary>
        /// Get office list match to given lst and mapping language.
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        List<doOfficeList> GetOfficeList(List<tbm_Office> lst);

        /// <summary>
        /// Get office list and map to officeLst.
        /// </summary>
        /// <param name="officeLst"></param>
        /// <returns></returns>
        void OfficeListMapping(OfficeMappingList officeLst);

        bool CheckHeadOffice(string strOfficeCode);

        /// <summary>
        /// Get office that have function billing and mapping language.
        /// </summary>
        /// <returns></returns>
        List<doFunctionBilling> GetFunctionBilling();

        List<dtOffice> GetFunctionSecurity();
        List<dtOffice> GetFunctionLogistic();
        List<tbm_Office> GetIncidentOffice();
        List<tbm_Office> GetAROffice();
        bool CheckInventoryHeadOffice(string officeCode);
    }
}
