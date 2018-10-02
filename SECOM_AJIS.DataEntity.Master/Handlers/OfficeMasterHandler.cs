using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Master
{
    public class OfficeMasterHandler : BizMADataEntities, IOfficeMasterHandler
    {
        public List<tbm_Office> GetTbm_Office()
        {
            try
            {
                var result = base.GetTbm_Office(null);
                CommonUtil.MappingObjectLanguage(result);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public override List<tbm_Office> GetTbm_Office(string OfficeCode)
        {
            var result = base.GetTbm_Office(OfficeCode);
            CommonUtil.MappingObjectLanguage(result);
            return result;
        }
        public List<dtOffice> GetFunctionQuatation(string pchrC_FUNC_QUATATION_NO)
        {
            return base.GetFunctionQuotaion(pchrC_FUNC_QUATATION_NO);
        }

        public List<doOfficeList> GetOfficeList(List<tbm_Office> lst)
        {
            try
            {
                List<doOfficeList> olst = this.GetOfficeList(SECOM_AJIS.Common.Util.CommonUtil.ConvertToXml_Store<tbm_Office>(lst, "OfficeCode"));
                return SECOM_AJIS.Common.Util.CommonUtil.ConvertObjectbyLanguage<doOfficeList, doOfficeList>(olst, "OfficeName");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void OfficeListMapping(OfficeMappingList officeLst)
        {
            try
            {
                if (officeLst == null)
                    return;

                List<doOfficeList> lst = this.GetOfficeList(officeLst.GetOfficeList());
                if (lst.Count > 0)
                    officeLst.SetOfficeValue(lst);
            }
            catch (Exception)
            {
                throw;
            }

        }
        public bool CheckHeadOffice(string strOfficeCode)
        {
            bool result = false;

            var res = this.CheckHeadOffice(strOfficeCode, SECOM_AJIS.Common.Util.ConstantValue.InventoryHeadOffice.C_OFFICELEVEL_HEAD);
            if (res.Count == 1)
            {
                if (res[0].HeadOfficeFlag == 1)
                {
                    result = true;
                }
            }

            return result;
        }

        public List<doFunctionBilling> GetFunctionBilling()
        {
            try
            {
                List<doFunctionBilling> lst = GetFunctionBilling(SECOM_AJIS.Common.Util.ConstantValue.FunctionBilling.C_FUNC_BILLING_NO);
                if (lst.Count > 0)
                    CommonUtil.MappingObjectLanguage<doFunctionBilling>(lst);

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<dtOffice> GetFunctionSecurity()
        {
            try
            {
                return GetFunctionSecurity(SECOM_AJIS.Common.Util.ConstantValue.FunctionSecurity.C_FUNC_SECURITY_NO);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<dtOffice> GetFunctionLogistic()
        {
            try
            {
                return GetFunctionLogistic(SECOM_AJIS.Common.Util.ConstantValue.FunctionLogistic.C_FUNC_LOGISTIC_NO);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool CheckInventoryHeadOffice(string officeCode)
        {
            List<int?> result = base.CheckInventoryHeadOffice(officeCode, FunctionLogistic.C_FUNC_LOGISTIC_HQ);
            if (result.Count <= 0)
                return false;
            else if (result.Count > 0)
                return true;
            else
                return false;
        }
    }
}
