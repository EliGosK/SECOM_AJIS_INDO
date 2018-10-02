using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class dtSearchMACheckupResult
    {
        CommonUtil c = new CommonUtil();

        //Maintenance check-up no. column
        public string CheckupNoShow
        {
           get
           {
               //return (CheckupNo == null) ? null : CheckupNo.ToString().PadLeft(7, '0');
               string txt = null;
               if (this.CheckupNo != null)
               {
                   txt = this.CheckupNo;
                   //txt = txt.Substring(0, 2) + txt.Substring(2).PadLeft(5, '0');
               }

               return txt;
           }
        }

        [LanguageMapping]
        public string OfficeName { get; set; }

        //Site code column
        public string SiteCodeShow
        {
            get
            {
                //return c.ConvertSiteCode(this.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                string[] siteCode = c.ConvertSiteCode(this.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT).Split("-".ToCharArray());
                if (siteCode.Length >= 2)
                {
                    return string.Format("{0}<br/>-{1}",
                        siteCode[0],
                        siteCode[1]);
                }

                return c.ConvertSiteCode(this.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        //Real customer Column
        public string RealCustomerShow
        {
            get 
            {
                return CommonUtil.TextLineFormat(this.CustFullNameEN, this.CustFullNameLC);
            }
        }

        //Site name Column
        public string SiteNameShow
        {
            get 
            {
                return CommonUtil.TextLineFormat(this.SiteNameEN, this.SiteNameLC);
            }
        }

        //UserCode ContractCode show Column
        public string UserCodeContractCodeShow
        {
            get
            {
                return string.Format("{0}/<br/>{1}",
                    this.UserCode,
                    c.ConvertContractCode( this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT)); 
            }
        }

        //Product Name Column
        [LanguageMapping]
        public string ProductName { get; set; }

        //Instruction MONTH-YEAR Column
        public string InstructionDateShow
        {
            get 
            { 
                return this.InstructionDate.ToString("MMM-yyyy"); 
            }
        }

        //public string ToJson
        //{
        //    get
        //    {
        //        return CommonUtil.CreateJsonString(this);
        //    }
        //}

        //ContractCodeShow
        public string ContractCodeShow
        {
            get 
            { 
                return c.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT); 
            }
        }

        //Expected Maintenance Date
        public string ExpectedMaintenanceDateShow
        {
            get 
            {
                return (this.ExpectedMaintenanceDate.HasValue)? this.ExpectedMaintenanceDate.Value.ToString("dd-MMM-yyyy") : string.Empty; 
            }
        }

        //Actual Maintenance Date
        public string MaintenanceDateShow
        {
            get 
            { 
                return (this.MaintenanceDate.HasValue) ? this.MaintenanceDate.Value.ToString("dd-MMM-yyyy") : string.Empty; 
            }
        }

        //MaintenanceDate Expected&Actual
        public string MaintenanceDateExpectActualShow
        {
            get
            {
                return CommonUtil.TextLineFormat(this.ExpectedMaintenanceDateShow, this.MaintenanceDateShow);
            }
        }


        public string EnableViewFlag { get; set; }
        public string EnableDeleteFlag { get; set; }
        public string EnableRegisterFlag { get; set; }
        public string EnableCheckboxFlag { get; set; }
        //public string EnableSelectButtonFlag { get; set; }
        public string CheckedFlag { get; set; }


        public string KeyIndex
        {
            get
            {
                return string.Format("{0}:{1}:{2}",
                    this.ContractCode,
                    this.ProductCode,
                    this.InstructionDate.ToString("yyyyMMdd"));
            }
        }
    }
}
