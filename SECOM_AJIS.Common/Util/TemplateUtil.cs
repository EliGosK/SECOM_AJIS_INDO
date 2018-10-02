using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using SECOM_AJIS.Common.Models.EmailTemplates;

namespace SECOM_AJIS.Common.Util
{
    /// <summary>
    /// Email template management
    /// </summary>
    public class EmailTemplateUtil
    {
        private string TemplateName { get; set; }

        public EmailTemplateUtil(string TemplateName)
        {
            this.TemplateName = TemplateName;
        }

        /// <summary>
        /// Load template
        /// </summary>
        /// <param name="content"></param>
        /// <param name="admin_content"></param>
        /// <returns></returns>
        public doEmailTemplate LoadTemplate(ATemplateObject content, ATemplateObject admin_content = null)
        {
            try
            {
                doEmailTemplate template = new doEmailTemplate();
                template.TemplateName = this.TemplateName;

                string resourcePath = string.Format("{0}{1}\\{2}.xml",
                                                            CommonUtil.WebPath,
                                                            ConstantValue.CommonValue.TEMPLATE_FOLDER,
                                                            template.TemplateName);
                XmlDocument rDoc = new XmlDocument();
                rDoc.Load(resourcePath);

                #region Set data to Template

                foreach (PropertyInfo prop in template.GetType().GetProperties())
                {
                    XmlNode rNode = rDoc.SelectSingleNode(string.Format("Template/{0}", prop.Name));
                    if (rNode != null)
                        prop.SetValue(template, rNode.InnerText, null);
                }
                    
                #endregion
                #region Mapping Content

                //if (content != null && template.TemplateContent != null)
                if (content != null && (template.TemplateContent != null || template.TemplateSubject != null)) //Modify by Jutarat A. on 11072013
                {
                    foreach (PropertyInfo prop in content.GetType().GetProperties())
                    {
                        string key = string.Format("[{0}]", prop.Name);

                        string txt = string.Empty;
                        object o = prop.GetValue(content, null);
                        if (o != null)
                            txt = o.ToString();

                        if (template.TemplateContent != null) //Add by Jutarat A. on 11072013
                            template.TemplateContent = template.TemplateContent.Replace(key, txt);

                        if (template.TemplateSubject != null) //Add by Jutarat A. on 11072013
                            template.TemplateSubject = template.TemplateSubject.Replace(key, txt); //Add by Jutarat A. on 11072013
                    }
                }

                #endregion
                #region Mapping Admin Content

                if (admin_content != null && template.AdminContent != null)
                {
                    foreach (PropertyInfo prop in admin_content.GetType().GetProperties())
                    {
                        string key = string.Format("[{0}]", prop.Name);

                        string txt = string.Empty;
                        object o = prop.GetValue(admin_content, null);
                        if (o != null)
                            txt = o.ToString();
                        template.AdminContent = template.AdminContent.Replace(key, txt);
                    }
                }

                #endregion


                return template;
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
