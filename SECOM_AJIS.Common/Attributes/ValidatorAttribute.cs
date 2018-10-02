using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.Common.CustomAttribute
{
    /// <summary>
    /// Abstract for validate attribute
    /// </summary>
    public abstract class AValidatorAttribute : ValidationAttribute
    {
        public string Controller { get; set; }
        public string Screen { get; set; }

        public string Module { get; set; }
        public MessageUtil.MessageList MessageCode { get; set; }
        public string ControlName { get; set; }
        public string Parameter { get; set; }
        public int Order { get; set; }

        public AValidatorAttribute()
        {
            this.MessageCode = MessageUtil.MessageList.MSG0007;
            this.Module = MessageUtil.MODULE_COMMON;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string template = string.Empty;
            if (validationContext != null)
            {
                if (CommonUtil.IsNullOrEmpty(this.Controller) == false)
                    template += this.Controller;
                template += ValidatorUtil.SPLIT_TEMPLATE_MESSAGE;
                if (CommonUtil.IsNullOrEmpty(this.Screen) == false)
                    template += this.Screen;
                template += ValidatorUtil.SPLIT_TEMPLATE_MESSAGE;

                template += this.Module + ValidatorUtil.SPLIT_TEMPLATE_MESSAGE;
                template += this.MessageCode.ToString() + ValidatorUtil.SPLIT_TEMPLATE_MESSAGE;
                template += ((this.Parameter != null) ? this.Parameter : validationContext.DisplayName) + ValidatorUtil.SPLIT_TEMPLATE_MESSAGE;
                template += ((this.ControlName != null) ? this.ControlName : string.Empty) + ValidatorUtil.SPLIT_TEMPLATE_MESSAGE;
                template += this.Order;
            }

            return new ValidationResult(template);
        }
    }
    /// <summary>
    /// Attribute for validate object is null?
    /// </summary>
    public class NotNullOrEmptyAttribute : AValidatorAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CommonUtil.IsNullOrEmpty(value))
                return base.IsValid(value, validationContext);

            return null;
        }
    }
    /// <summary>
    /// Attribute for validate relate object is correct?
    /// </summary>
    public class RelateObjectAttribute : AValidatorAttribute
    {
        private string tmpParameter { get; set; }
        private string tmpControlName { get; set; }

        public string RelateProperty { get; set; }
        public string RelateParameter { get; set; }
        public string RelateControlName { get; set; }

        public RelateObjectAttribute(string RelateProperty)
        {
            this.RelateProperty = RelateProperty;

            if (this.RelateParameter == null)
                this.RelateParameter = RelateProperty;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (this.RelateProperty != null)
            {
                bool isInputEmptyData = CommonUtil.IsNullOrEmpty(value);
                bool isRelateEmptyData = true;

                PropertyInfo prop = validationContext.ObjectInstance.GetType().GetProperty(this.RelateProperty);
                if (prop != null)
                {
                    if (CommonUtil.IsNullOrEmpty(prop.GetValue(validationContext.ObjectInstance, null)) == false)
                        isRelateEmptyData = false;
                }

                if (isRelateEmptyData != isInputEmptyData)
                {
                    if (this.tmpParameter == null)
                        this.tmpParameter = this.Parameter;
                    if (this.tmpControlName == null)
                        this.tmpControlName = this.ControlName;

                    if (isInputEmptyData == false)
                    {
                        this.Parameter = this.RelateParameter;
                        this.ControlName = this.RelateControlName;
                    }
                    else
                    {
                        this.Parameter = this.tmpParameter;
                        this.ControlName = this.tmpControlName;
                    }

                    return base.IsValid(value, validationContext);
                }
            }

            return null;
        }
    }
    /// <summary>
    /// Attribute for validate japanese charactor is correct?
    /// </summary>
    public class JapaneseCharAttribute : AValidatorAttribute
    {
        static private string InvalidChar = "①②③④⑤⑥⑦⑧⑨⑩⑪⑫⑬⑭⑮⑯⑰⑱⑲⑳ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩ㍉㌔㌢㍍㌘㌧㌃㌶㍑㍗㌍㌦㌣㌫㍊㌻㎜㎝㎞㎎㎏㏄㎡㍻〝〟№㏍℡㊤㊥㊦㊧㊨㈱㈲㈹㍾㍽㍼≒≡∫∮∑√⊥∠∟⊿∵∩∪纊褜鍈銈蓜俉炻昱棈鋹曻彅丨仡仼伀伃伹佖侒侊侚侔俍偀倢俿倞偆偰偂傔僴僘兊兤冝冾凬刕劜劦勀勛匀匇匤卲厓厲叝﨎咜咊咩哿喆坙坥垬埈埇﨏塚增墲夋奓奛奝奣妤妺孖寀甯寘寬尞岦岺峵崧嵓﨑嵂嵭嶸嶹巐弡弴彧德忞恝悅悊惞惕愠惲愑愷愰憘戓抦揵摠撝擎敎昀昕昻昉昮昞昤晥晗晙晴晳暙暠暲暿曺朎朗杦枻桒柀栁桄棏﨓楨﨔榘槢樰橫橆橳橾櫢櫤毖氿汜沆汯泚洄涇浯涖涬淏淸淲淼渹湜渧渼溿澈澵濵瀅瀇瀨炅炫焏焄煜煆煇凞燁燾犱犾猤猪獷玽珉珖珣珒琇珵琦琪琩琮瑢璉璟甁畯皂皜皞皛皦益睆劯砡硎硤硺礰礼神祥禔福禛竑竧靖竫箞精絈絜綷綠緖繒罇羡羽茁荢荿菇菶葈蒴蕓蕙蕫﨟薰蘒﨡蠇裵訒訷詹誧誾諟諸諶譓譿賰賴贒赶﨣軏﨤逸遧郞都鄕鄧釚釗釞釭釮釤釥鈆鈐鈊鈺鉀鈼鉎鉙鉑鈹鉧銧鉷鉸鋧鋗鋙鋐﨧鋕鋠鋓錥錡鋻﨨錞鋿錝錂鍰鍗鎤鏆鏞鏸鐱鑅鑈閒隆﨩隝隯霳霻靃靍靏靑靕顗顥飯飼餧館馞驎髙髜魵魲鮏鮱鮻鰀鵰鵫鶴鸙黑ⅰⅱⅲⅳⅴⅵⅶⅷⅸⅹ¬￤＇＂ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩ㈱№℡∵｡｢｣､･ｦｧｨｩｪｫｬｭｮｯｰｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜﾝﾞﾟ";
        static byte[] InvalidTable = new byte[65536];

        static JapaneseCharAttribute()
        {
            JapaneseCharAttribute.InitTable();
        }

        public static void InitTable()
        {
            for (int i = 0; i < InvalidTable.Length; i++)
                InvalidTable[i] = 0;
            char[] cc = InvalidChar.ToCharArray();
            foreach (char ic in cc)
                InvalidTable[(int)ic] = 1;
        }

        public static bool IsValidCharactor(string text)
        {
            char[] cc = text.ToCharArray();
            foreach (char c in cc)
                if (0 != InvalidTable[(int)c])
                    return false;

            return true;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (false == IsValidCharactor(value as string))
                return base.IsValid(value, validationContext);

            return null;
        }
    }
    /// <summary>
    /// Attribute for validate number is not more than limit
    /// </summary>
    public class NotMoreThanAttribute : AValidatorAttribute
    {
        public string RelateProperty { get; set; }

        public NotMoreThanAttribute()
            : base()
        {
            this.Module = MessageUtil.MODULE_QUOTATION;
            this.MessageCode = MessageUtil.MessageList.MSG2063;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (this.RelateProperty != null)
            {
                decimal dec1 = 0;
                decimal dec2 = 0;

                if (CommonUtil.IsNullOrEmpty(value) == false)
                {
                    if (Decimal.TryParse(value.ToString(), out dec1) == false)
                        dec1 = 0;
                }

                PropertyInfo prop = validationContext.ObjectInstance.GetType().GetProperty(this.RelateProperty);
                if (prop != null)
                {
                    object ovalue = prop.GetValue(validationContext.ObjectInstance, null);
                    if (ovalue != null)
                    {
                        if (Decimal.TryParse(ovalue.ToString(), out dec2) == false)
                            dec2 = 0;
                    }
                }

                if (dec1 > dec2)
                {
                    this.Parameter = null;
                    return base.IsValid(value, validationContext);
                }
            }

            return null;
        }
    }
    /// <summary>
    /// Attribute for validate value must more than minimum value
    /// </summary>
    public class MinimumValueAttribute : AValidatorAttribute
    {
        public double Min { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool isPass = false;
            if (CommonUtil.IsNullOrEmpty(value) == false)
            {
                double dec = 0;
                if (double.TryParse(value.ToString(), out dec) == true)
                {
                    if (dec >= Min)
                        isPass = true;
                }
            }

            if (isPass == false)
                return base.IsValid(value, validationContext);

            return null;
        }
    }
    /// <summary>
    /// Attribute for validate number is between minimum and maximum value
    /// </summary>
    public class RangeNumberValueAttribute : AValidatorAttribute
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public RangeNumberValueAttribute(double Min, double Max)
        {
            this.Min = Min;
            this.Max = Max;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CommonUtil.IsNullOrEmpty(value) == false)
            {
                double dec = 0;
                if (double.TryParse(value.ToString(), out dec) == true)
                {
                    if (dec >= Min && dec <= Max)
                        return null;
                }
            }

            return base.IsValid(value, validationContext);
        }
    }
    /// <summary>
    /// Attribute for validate time data
    /// </summary>
    public class TimeValueAttribute : AValidatorAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CommonUtil.IsNullOrEmpty(value) == false)
            {
                TimeSpan? time = null;
                if (value is TimeSpan)
                    time = (TimeSpan)value;
                else if (value is TimeSpan?)
                    time = (TimeSpan?)value;
                if (time != null)
                {
                    if (time.HasValue)
                    {
                        if (time.Value.Ticks >= 0 && time.Value.Ticks <= 863400000000)
                            return null;
                    }
                }
            }

            return base.IsValid(value, validationContext);
        }
    }
    /// <summary>
    /// Attribute for validate code data (if code is empty, detail must empty)
    /// </summary>
    public class CodeNullOtherNullAttribute : AValidatorAttribute
    {
        public string Code { get; set; }
        public CodeNullOtherNullAttribute(string Code)
        {
            this.Code = Code;
            this.Parameter = Code;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            object obj = null;
            if (this.Code != null)
            {
                PropertyInfo cprop = validationContext.ObjectInstance.GetType().GetProperty(this.Code);
                if (cprop != null)
                    obj = cprop.GetValue(validationContext.ObjectInstance, null);
            }
            if (CommonUtil.IsNullOrEmpty(obj) == true
                && CommonUtil.IsNullOrEmpty(value) == false)
            {
                return base.IsValid(value, validationContext);
            }
            return null;
        }
    }
    /// <summary>
    /// Attribute for validate code data (if code is empty, detail must fill)
    /// </summary>
    public class CodeNullOtherNotNullAttribute : AValidatorAttribute
    {
        public string Code { get; set; }
        public CodeNullOtherNotNullAttribute(string Code)
        {
            this.Code = Code;
            this.Parameter = Code;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            object obj = null;
            if (this.Code != null)
            {
                PropertyInfo cprop = validationContext.ObjectInstance.GetType().GetProperty(this.Code);
                if (cprop != null)
                    obj = cprop.GetValue(validationContext.ObjectInstance, null);
            }
            if (CommonUtil.IsNullOrEmpty(obj) == true
                && CommonUtil.IsNullOrEmpty(value) == true)
            {
                return base.IsValid(value, validationContext);
            }

            return null;
        }
    }
    /// <summary>
    /// Attribute for validate code data (if fill code, detail must empty)
    /// </summary>
    public class CodeNotNullOtherNullAttribute : AValidatorAttribute
    {
        public string Code { get; set; }
        public CodeNotNullOtherNullAttribute(string Code)
        {
            this.Code = Code;
            this.Parameter = Code;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            object obj = null;
            if (this.Code != null)
            {
                PropertyInfo cprop = validationContext.ObjectInstance.GetType().GetProperty(this.Code);
                if (cprop != null)
                    obj = cprop.GetValue(validationContext.ObjectInstance, null);
            }
            if (CommonUtil.IsNullOrEmpty(obj) == false
                && CommonUtil.IsNullOrEmpty(value) == false)
            {
                return base.IsValid(value, validationContext);
            }
            return null;
        }
    }
    /// <summary>
    /// Attribute for validate code data (if fill code, detail must fill too)
    /// </summary>
    public class CodeNotNullOtherNotNullAttribute : AValidatorAttribute
    {
        public string Code { get; set; }
        public CodeNotNullOtherNotNullAttribute(string Code)
        {
            this.Code = Code;
            this.Parameter = Code;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            object obj = null;
            if (this.Code != null)
            {
                PropertyInfo cprop = validationContext.ObjectInstance.GetType().GetProperty(this.Code);
                if (cprop != null)
                    obj = cprop.GetValue(validationContext.ObjectInstance, null);
            }
            if (CommonUtil.IsNullOrEmpty(obj) == false
                && CommonUtil.IsNullOrEmpty(value) == true)
            {
                return base.IsValid(value, validationContext);
            }

            return null;
        }
    }
    /// <summary>
    /// Attribute for validate code data and name of code must have data
    /// </summary>
    public class CodeHasValueAttribute : AValidatorAttribute
    {
        public string ValueField { get; set; }
        public CodeHasValueAttribute(string ValueField)
        {
            this.ValueField = ValueField;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CommonUtil.IsNullOrEmpty(value) == false)
            {
                if (this.ValueField != null)
                {
                    PropertyInfo cprop = validationContext.ObjectInstance.GetType().GetProperty(this.ValueField);
                    if (cprop != null)
                    {
                        object obj = cprop.GetValue(validationContext.ObjectInstance, null);
                        if (CommonUtil.IsNullOrEmpty(obj))
                            return base.IsValid(value, validationContext);
                    }
                }
            }
            return null;
        }
    }
    /// <summary>
    /// Attribute for validate office code is in role
    /// </summary>
    public class OfficeInRoleAttribute : AValidatorAttribute
    {
        public string FunctionQuatation { get; set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CommonUtil.IsNullOrEmpty(value) == false)
            {
                List<OfficeDataDo> clst = CommonUtil.dsTransData.dtOfficeData;
                if (clst != null)
                {
                    foreach (OfficeDataDo off in clst)
                    {
                        if ((off.FunctionQuatation == FunctionQuatation || FunctionQuatation == null)
                            && off.OfficeCode == (string)value)
                            return null;
                    }
                }

                return base.IsValid(value, validationContext);
            }

            return null;
        }
    }
    /// <summary>
    /// Attribute for validate quotation office code must in role
    /// </summary>
    public class QuotationOfficeInRoleAttribute : OfficeInRoleAttribute
    {
        public QuotationOfficeInRoleAttribute()
        {
            this.FunctionQuatation = SECOM_AJIS.Common.Util.ConstantValue.FunctionQuotation.C_FUNC_QUOTATION_YES;
        }
    }
    //public class OperationOfficeInRoleAttribute : OfficeInRoleAttribute
    //{
    //    public OperationOfficeInRoleAttribute()
    //    {
    //        this.FunctionQuatation = SECOM_AJIS.Common.Util.ConstantValue.FunctionSecurity.C_FUNC_SECURITY_NO;
    //    }
    //}
    /// <summary>
    /// Attribute for validate operation office code must in role
    /// </summary>
    public class OperationOfficeInRoleAttribute : AValidatorAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CommonUtil.IsNullOrEmpty(value) == false)
            {
                List<OfficeDataDo> clst = CommonUtil.dsTransData.dtOfficeData;
                if (clst != null)
                {
                    foreach (OfficeDataDo off in clst)
                    {
                        if (off.FunctionSecurity != FunctionSecurity.C_FUNC_SECURITY_NO && off.OfficeCode == (string)value)
                            return null;
                    }
                }

                return base.IsValid(value, validationContext);
            }

            return null;
        }
    }
    /// <summary>
    /// Attribute for validate contract office code must in role
    /// </summary>
    public class ContractOfficeInRoleAttribute : AValidatorAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CommonUtil.IsNullOrEmpty(value) == false)
            {
                List<OfficeDataDo> clst = CommonUtil.dsTransData.dtOfficeData;
                if (clst != null)
                {
                    foreach (OfficeDataDo off in clst)
                    {
                        if (off.FunctionSale == FunctionSale.C_FUNC_SALE_YES && off.OfficeCode == (string)value)
                            return null;
                    }
                }

                return base.IsValid(value, validationContext);
            }

            return null;
        }
    }
    /// <summary>
    /// Attribute for validate group data must fill at least 1 field
    /// </summary>
    public class AtLeast1FieldNotNullOrEmptyAttribute : Attribute
    {
        public string Controller { get; set; }
        public string Screen { get; set; }
        public string Module { get; set; }
        public MessageUtil.MessageList MessageCode { get; set; }
        public string ControlName { get; set; }
        public string Parameter { get; set; }
        public int Order { get; set; }
        public bool UseControl { get; set; }
        public AtLeast1FieldNotNullOrEmptyAttribute()
        {
            this.Module = MessageUtil.MODULE_COMMON;
            this.MessageCode = MessageUtil.MessageList.MSG0007;
            this.UseControl = false;
        }
    }
    /// <summary>
    /// Attribute for validate text length must less than maximum length
    /// </summary>
    public class MaxTextLengthAttribute : Attribute
    {
        public int MaxLength { get; set; }

        public MaxTextLengthAttribute(int maxLength)
        {
            this.MaxLength = maxLength;
        }
    }
    /// <summary>
    /// Attribute for validate text length must less than maximum length in case of customer code
    /// </summary>
    public class MaxCustomerCodeLengthAttribute : MaxTextLengthAttribute
    {
        public MaxCustomerCodeLengthAttribute() : base(0)
        {
            this.MaxLength = int.Parse(SECOM_AJIS.Common.Util.ConstantValue.CommonValue.C_MAXLENGTH_CUSTOMER_CODE);
        }
    }
    /// <summary>
    /// Attribute for validate text length must less than maximum length in case of contract code
    /// </summary>
    public class MaxContractCodeLengthAttribute : MaxTextLengthAttribute
    {
        public MaxContractCodeLengthAttribute()
            : base(0)
        {
            this.MaxLength = int.Parse(SECOM_AJIS.Common.Util.ConstantValue.CommonValue.C_MAXLENGTH_CONTRACT_CODE);
        }
    }
}
