using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Models.EmailTemplates
{
    /// <summary>
    /// Abstract for email template
    /// </summary>
    public abstract class ATemplateObject
    {
    }
    /// <summary>
    /// DO for email template
    /// </summary>
    public class doEmailTemplate
    {
        public string TemplateName { get; set; }
        public string AdminEmail { get; set; }
        public string AdminSubject { get; set; }
        public string AdminContent { get; set; }
        public string TemplateSubject { get; set; }
        public string TemplateContent { get; set; }
    }
    
}
