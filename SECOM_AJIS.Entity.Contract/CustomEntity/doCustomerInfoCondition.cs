using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class doCustomerInfoCondition
    {
        // Long code format
        private string _ContractCode;
        /// <summary>
        /// Long code format
        /// </summary>
        public string ContractCode
        {
            get
            {

                CommonUtil c = new CommonUtil();
                return c.ConvertContractCode(this._ContractCode ,CommonUtil.CONVERT_TYPE.TO_LONG);

            }
            set { _ContractCode = value; }
        }
        public string OCC { get; set; }

        // Long code format
        private string _ContractTargetCode;
        /// <summary>
        /// Long code format
        /// </summary>
        public string ContractTargetCode
        {
            get
            {

                CommonUtil c = new CommonUtil();
                return c.ConvertCustCode(_ContractTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);

            }

            set { _ContractTargetCode = value; }
        }

        // Long code format
        private string _PurchaserCustCode;
        /// <summary>
        /// Long code format
        /// </summary>
        public string PurchaserCustCode
        {
            get
            {

                CommonUtil c = new CommonUtil();
                return c.ConvertCustCode( _PurchaserCustCode , CommonUtil.CONVERT_TYPE.TO_LONG) ;

            }

            set { _PurchaserCustCode = value; }
        }

        // Long code format
        private string _RealCustomerCode;
        /// <summary>
        /// Long code format
        /// </summary>
        public string RealCustomerCode
        {
            get
            {

                CommonUtil c = new CommonUtil();
                return  c.ConvertCustCode(_RealCustomerCode,CommonUtil.CONVERT_TYPE.TO_LONG) ;

            }
            set
            {
                _RealCustomerCode = value;
            }
        }

        // Long code format
        private string _SiteCode;
        /// <summary>
        /// Long code format
        /// </summary>
        public string SiteCode
        {
            get
            {

                CommonUtil c = new CommonUtil();
                return c.ConvertSiteCode(_SiteCode ,CommonUtil.CONVERT_TYPE.TO_LONG) ;

            }
            set { _SiteCode = value; }
        }
        public string ServiceTypeCode { get; set; }

        // Long code format
        private string _MATargetContractCode;
        /// <summary>
        /// Long code format
        /// </summary>
        public string MATargetContractCode   // use convert customer code
        {
            get
            {

                CommonUtil c = new CommonUtil();
                return c.ConvertCustCode(_MATargetContractCode , CommonUtil.CONVERT_TYPE.TO_LONG);

            }
            set { _MATargetContractCode = value; }
        }
        public string ProductCode { get; set; }

    }
}
