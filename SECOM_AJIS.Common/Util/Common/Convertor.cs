using System;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Common.Util
{
    public partial class CommonUtil
    {
        public enum CONVERT_TYPE
        {
            TO_LONG,
            TO_SHORT
        }
        /// <summary>
        /// Convert code from long <-> short
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ctype"></param>
        /// <param name="short_digit"></param>
        /// <param name="long_digit"></param>
        /// <returns></returns>
        private string ConvertCode(string code, CONVERT_TYPE ctype, int short_digit, int long_digit)
        {
            if (code == null || code == string.Empty)
                return null;

            string input = code.Trim();
            string prefix = string.Empty;
            string no = string.Empty;
            string suffix = string.Empty;
            string result = string.Empty;

            int spStart = code.IndexOfAny("-".ToCharArray());
            if (spStart >= 0)
            {
                suffix = input.Substring(spStart);
                input = input.Substring(0, spStart);
            }

            int numStart = input.IndexOfAny("0123456789".ToCharArray());
            if (numStart >= 0)
            {
                prefix = input.Substring(0, numStart);
                no = input.Substring(numStart);
            }
            else
                prefix = input;

            if (ctype == CONVERT_TYPE.TO_LONG)
            {
                //result = prefix + no.PadLeft(digit, '0');
                string a_no = string.Empty;
                if (no != string.Empty)
                {
                    a_no = a_no.PadLeft(Math.Abs(long_digit - short_digit), '0') + no;
                    if (a_no.Length > long_digit)
                    {
                        throw ApplicationErrorException.ThrowErrorException(
                                    MessageUtil.MODULE_COMMON,
                                    MessageUtil.MessageList.MSG0135,
                                    new string[] { code });
                    }
                }

                result = prefix + a_no;
            }
            else
            {
                string txtno = string.Empty;

                int diff = Math.Abs(long_digit - short_digit);
                if (diff < no.Length)
                    txtno = no.Substring(diff);

                result = prefix + txtno;
            }

            return result + suffix;
        }
        /// <summary>
        /// Convert group code from long <-> short
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ctype"></param>
        /// <returns></returns>
        public string ConvertGroupCode(string code, CONVERT_TYPE ctype)
        {
            return ConvertCode(code, ctype, ConstantValue.CommonValue.GROUP_CODE_SHORT_DIGIT, ConstantValue.CommonValue.GROUP_CODE_LONG_DIGIT);
        }
        /// <summary>
        /// Convert customer code from long <-> short
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ctype"></param>
        /// <returns></returns>
        public string ConvertCustCode(string code, CONVERT_TYPE ctype)
        {
            return ConvertCode(code, ctype, ConstantValue.CommonValue.CUST_CODE_SHORT_DIGIT, ConstantValue.CommonValue.CUST_CODE_LONG_DIGIT);
        }
        /// <summary>
        /// Convert site code from long <-> short
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ctype"></param>
        /// <returns></returns>
        public string ConvertSiteCode(string code, CONVERT_TYPE ctype)
        {
            return ConvertCode(code, ctype, ConstantValue.CommonValue.SITE_CODE_SHORT_DIGIT, ConstantValue.CommonValue.SITE_CODE_LONG_DIGIT);
        }
        /// <summary>
        /// Convert contract code from long <-> short
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ctype"></param>
        /// <returns></returns>
        public string ConvertContractCode(string code, CONVERT_TYPE ctype)
        {
            if ((string.IsNullOrEmpty(code) == false) && code.Length > 0)
            {
                if (code.Substring(0,1).ToUpper() == "P") // Project code does not convert short-long
                {
                    return code;
                } 
            }

            return ConvertCode(code, ctype, ConstantValue.CommonValue.CONTRACT_CODE_SHORT_DIGIT, ConstantValue.CommonValue.CONTRACT_CODE_LONG_DIGIT);
        }
        /// <summary>
        /// Convert project code from long <-> short
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ctype"></param>
        /// <returns></returns>
        public string ConvertProjectCode(string code, CONVERT_TYPE ctype)
        {
            return ConvertCode(code, ctype, ConstantValue.CommonValue.PROJECT_CODE_SHORT_DIGIT, ConstantValue.CommonValue.PROJECT_CODE_LONG_DIGIT);
        }
        /// <summary>
        /// Convert billing target code from long <-> short
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ctype"></param>
        /// <returns></returns>
        public string ConvertBillingTargetCode(string code, CONVERT_TYPE ctype)
        {
            return ConvertCode(code, ctype, ConstantValue.CommonValue.BILLING_TARGET_CODE_SHORT_DIGIT, ConstantValue.CommonValue.BILLING_TARGET_CODE_LONG_DIGIT);
        }
        /// <summary>
        /// Convert billing code from long <-> short
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ctype"></param>
        /// <returns></returns>
        public string ConvertBillingCode(string code, CONVERT_TYPE ctype)
        {
            if (CommonUtil.IsNullOrEmpty(code) == false)
            {
                if (code.StartsWith("P") || code.StartsWith("p"))
                    return code;
            }

            return ConvertCode(code, ctype, ConstantValue.CommonValue.BILLING_CODE_SHORT_DIGIT, ConstantValue.CommonValue.BILLING_CODE_LONG_DIGIT);
        }
        /// <summary>
        /// Convert quotation target code from long <-> short
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ctype"></param>
        /// <returns></returns>
        public string ConvertQuotationTargetCode(string code, CONVERT_TYPE ctype)
        {  // Modified by Anancha T. 9/Nov/2011
           // return ConvertCode(code, ctype, ConstantValue.CommonValue.QUOTATION_TARGET_CODE_SHORT_DIGIT, ConstantValue.CommonValue.QUOTATION_TARGET_CODE_LONG_DIGIT);
            return ConvertCode(code, ctype, ConstantValue.CommonValue.CONTRACT_CODE_SHORT_DIGIT, ConstantValue.CommonValue.CONTRACT_CODE_LONG_DIGIT);
        }
        /// <summary>
        /// Convert billing client code from long <-> short
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ctype"></param>
        /// <returns></returns>
        public string ConvertBillingClientCode(string code, CONVERT_TYPE ctype)
        {
            return ConvertCode(code, ctype, ConstantValue.CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT, ConstantValue.CommonValue.BILLING_CLIENT_CODE_LONG_DIGIT);
        }
    }
}
