using System;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;

using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Common.Util
{
    public partial class CommonUtil
    {
        public enum LANGUAGE_LIST
        {
            LANGUAGE_1,
            LANGUAGE_2,
            LANGUAGE_3
        }
        /// <summary>
        /// Get current language
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentLanguage()
        {
            return Thread.CurrentThread.CurrentUICulture.Name;
        }
        /// <summary>
        /// Get current language from URL
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentLanguageURL()
        {
            string lang = GetCurrentLanguage();

            if (lang == CommonValue.DEFAULT_LANGUAGE_EN
                    || lang.ToLower() == CommonValue.DEFAULT_SHORT_LANGUAGE_EN)
            {
                lang = CommonValue.DEFAULT_SHORT_LANGUAGE_EN;
            }
            else if (lang == CommonValue.DEFAULT_LANGUAGE_JP
                || lang.ToLower() == CommonValue.DEFAULT_SHORT_LANGUAGE_JP)
            {
                lang = CommonValue.DEFAULT_SHORT_LANGUAGE_JP;
            }
            else if (lang == CommonValue.DEFAULT_LANGUAGE_LC
               || lang.ToLower() == CommonValue.DEFAULT_SHORT_LANGUAGE_LC)
            {
                lang = CommonValue.DEFAULT_SHORT_LANGUAGE_LC;
            }

            return lang;
        }
        /// <summary>
        /// Get current language
        /// </summary>
        /// <param name="includeLang2"></param>
        /// <returns></returns>
        public static LANGUAGE_LIST CurrentLanguage(bool includeLang2 = true)
        {
            dsTransDataModel dsTrans = dsTransData;
            if (dsTrans != null)
            {
                if (dsTrans.dtTransHeader != null)
                {
                    if (includeLang2 && dsTrans.dtTransHeader.Language == CommonValue.DEFAULT_LANGUAGE_JP)
                        return LANGUAGE_LIST.LANGUAGE_2;
                    else if (dsTrans.dtTransHeader.Language == CommonValue.DEFAULT_LANGUAGE_EN
                            || dsTrans.dtTransHeader.Language == CommonValue.DEFAULT_LANGUAGE_JP)
                        return LANGUAGE_LIST.LANGUAGE_1;
                    else
                        return LANGUAGE_LIST.LANGUAGE_3;
                }
            }
            return LANGUAGE_LIST.LANGUAGE_1;
        }
        /// <summary>
        /// Mapping current language
        /// </summary>
        /// <param name="obj"></param>
        public static void MappingObjectLanguage(object obj)
        {
            try
            {
                if (obj != null)
                {
                    CommonUtil.LANGUAGE_LIST lang = CurrentLanguage();

                    Dictionary<string, LanguageMappingAttribute> langAttr = CreateAttributeDictionary<LanguageMappingAttribute>(obj);
                    foreach (KeyValuePair<string, LanguageMappingAttribute> attr in langAttr)
                    {
                        PropertyInfo prop = obj.GetType().GetProperty(attr.Key);
                        if (prop != null)
                        {
                            if (lang == LANGUAGE_LIST.LANGUAGE_1
                                || lang == LANGUAGE_LIST.LANGUAGE_2)
                            {
                                PropertyInfo propEN = obj.GetType().GetProperty(prop.Name + CommonValue.LANGUAGE_EN);
                                PropertyInfo propJP = obj.GetType().GetProperty(prop.Name + CommonValue.LANGUAGE_JP);

                                if (propJP != null && lang == LANGUAGE_LIST.LANGUAGE_2)
                                    prop.SetValue(obj, propJP.GetValue(obj, null), null);
                                else if (propEN != null)
                                    prop.SetValue(obj, propEN.GetValue(obj, null), null);
                            }
                            else
                            {
                                PropertyInfo propLC = obj.GetType().GetProperty(prop.Name + CommonValue.LANGUAGE_LC);
                                if (propLC != null)
                                    prop.SetValue(obj, propLC.GetValue(obj, null), null);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Mapping current language
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        public static void MappingObjectLanguage<T>(List<T> lst) where T : class
        {
            try
            {
                if (lst != null)
                {
                    foreach (T obj in lst)
                    {
                        MappingObjectLanguage(obj);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}