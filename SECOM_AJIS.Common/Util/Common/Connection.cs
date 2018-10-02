using System;
using System.Resources;
using System.Web;
using System.Globalization;
using System.Reflection;
using System.Xml;

using SECOM_AJIS.Common.Util.ConstantValue;
using System.Web.Mvc;

namespace SECOM_AJIS.Common.Util
{
    public partial class CommonUtil
    {
        private static string _WebPath;
        /// <summary>
        /// Get/Set web directory
        /// </summary>
        public static string WebPath
        {
            get
            {
                try
                {
                    // Edit by Narupon W. (30-Mar-2012): for keep web root path ,support case batch process (session death)
                    //return HttpContext.Current != null ? HttpContext.Current.Server.MapPath("~") : _WebPath; 
                    // 3 Apr 2012 - Change to initial value in Application_Start by Rachatanawee
                    return _WebPath;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            set 
            {
                _WebPath = value;
            }
        }
        /// <summary>
        /// Generate page URL
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string GenerateURL(string controller, string action, string parameter = null)
        {
            string url = string.Format("/{0}/{1}/{2}",
                                            CommonUtil.GetCurrentLanguageURL(),
                                            controller,
                                            action);
            if (parameter != null)
                url += "?" + parameter;

            return url;
        }
        /// <summary>
        /// Get currenct URL
        /// </summary>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static string GetCurrentHostURL(bool isSSL = false)
        {
            HttpRequest currentReq = HttpContext.Current.Request;
            string url = string.Format("{0}://{1}{2}/"
                , ((isSSL) ? "https" : "http")
                , currentReq.Url.Host
                , ((currentReq.Url.IsDefaultPort) ? "" : ":" + currentReq.Url.Port.ToString()));

            return url;
        }
        /// <summary>
        /// Generate full URL address
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string GenerateCompleteURL(string controller, string action, string parameter = null, string strLanguage = null) //Add strLanguage by Jutarat A. on 28092012
        {
            //Add by Jutarat A. on 28092012
            if (strLanguage == null)
                strLanguage = CommonUtil.GetCurrentLanguageURL();
            //End Add

            string url = string.Format("{0}{1}/{2}/{3}",
                                            GetCurrentHostURL(),
                                            strLanguage, //CommonUtil.GetCurrentLanguageURL(), //Modify by Jutarat A. on 28092012
                                            controller,
                                            action);
            if (parameter != null)
                url += "?" + parameter;

            return url;
        }
        /// <summary>
        /// Get resource data
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resource_namespace"></param>
        /// <param name="useCurrentCulture"></param>
        /// <returns></returns>
        public ResourceSet GetResource(string assembly, string resource_namespace, bool useCurrentCulture = true)
        {
            ResourceSet rs = null;
            try
            {
                string appFolder = CommonValue.ASSEMBLY_FOLDER;
                string filePath = string.Format("{0}{1}\\{2}", CommonUtil.WebPath, CommonValue.ASSEMBLY_FOLDER, assembly);
                Assembly asm = Assembly.LoadFrom(filePath);

                string basePath = string.Format("{0}.{1}", asm.GetName().Name, resource_namespace);
                if (basePath != null)
                {
                    ResourceManager rm = new ResourceManager(basePath, asm);
                    if (rm != null)
                    {
                        System.Globalization.CultureInfo cul = new System.Globalization.CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);
                        if (useCurrentCulture)
                            cul = System.Threading.Thread.CurrentThread.CurrentUICulture;
                        rs = rm.GetResourceSet(cul, true, true);
                    }
                }
            }
            catch (Exception)
            {
                rs = null;
            }

            return rs;
        }
        /// <summary>
        /// Get text from selected resource
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="screen"></param>
        /// <param name="labelName"></param>
        /// <returns></returns>
        public static string GetLabelFromResource(string controller, string screen, string labelName, bool isDefaultLanguage = false) //Add isDefaultLanguage by Jutarat A. on 07092012
        {
            string strLabelResult = string.Empty;

            if (string.IsNullOrEmpty(labelName) == false)
            {
                //Add by Jutarat A. on 07092012
                string lang;
                if (isDefaultLanguage)
                {
                    lang = string.Empty;
                }
                //End Add
                else
                {
                    lang = CommonUtil.GetCurrentLanguage();
                    if (lang == ConstantValue.CommonValue.DEFAULT_LANGUAGE_EN)
                        lang = string.Empty;
                    else
                        lang = "." + lang;
                }

                string resourcePath = string.Format("{0}{1}\\{2}\\{3}{4}.resx",
                                                                CommonUtil.WebPath,
                                                                ConstantValue.CommonValue.APP_GLOBAL_RESOURCE_FOLDER,
                                                                controller,
                                                                screen,
                                                                lang);
                XmlDocument rDoc = new XmlDocument();
                rDoc.Load(resourcePath);

                XmlNode rNode = rDoc.SelectSingleNode(string.Format("root/data[@name=\"{0}\"]/value", labelName.Trim()));
                if (rNode != null)
                    strLabelResult = rNode.InnerText;
            }

            return strLabelResult;
        }

        //Add by Jutarat A. on 16122013
        /// <summary>
        /// Get text from selected resource (by Language)
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="screen"></param>
        /// <param name="labelName"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static string GetLabelFromResource(string controller, string screen, string labelName, string lang) 
        {
            string strLabelResult = string.Empty;

            if (string.IsNullOrEmpty(labelName) == false)
            {
                if (String.IsNullOrEmpty(lang))
                    lang = CommonUtil.GetCurrentLanguage();

                if (lang == ConstantValue.CommonValue.DEFAULT_LANGUAGE_EN)
                    lang = string.Empty;
                else
                    lang = "." + lang;

                string resourcePath = string.Format("{0}{1}\\{2}\\{3}{4}.resx",
                                                                CommonUtil.WebPath,
                                                                ConstantValue.CommonValue.APP_GLOBAL_RESOURCE_FOLDER,
                                                                controller,
                                                                screen,
                                                                lang);
                XmlDocument rDoc = new XmlDocument();
                rDoc.Load(resourcePath);

                XmlNode rNode = rDoc.SelectSingleNode(string.Format("root/data[@name=\"{0}\"]/value", labelName.Trim()));
                if (rNode != null)
                    strLabelResult = rNode.InnerText;
            }

            return strLabelResult;
        }
        //End Add

    }
}
