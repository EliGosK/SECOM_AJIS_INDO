using System;
using System.Web;
using System.Web.Mvc;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;
using System.Collections.Generic;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Models.EmailTemplates;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        [Initialize("CMS020")]
        public ActionResult ContractMenu(int type)
        {
            List<Dictionary<string, Menu>> lines = new List<Dictionary<string, Menu>>();
            try
            {
                List<Menu> MenuSession = ViewBag.lstMenu;
                MenuSession = MenuSession[1].SubMenu; //MenuSession[1] is Contract section

                if (type < MenuSession.Count)
                {
                    Dictionary<string, Menu> dic = null;
                    for (int idx = 0; idx < MenuSession[type].SubMenu.Count; idx++)
                    {
                        if (idx % 3 == 0)
                        {
                            dic = new Dictionary<string, Menu>();
                            lines.Add(dic);
                        }

                        Menu item = MenuSession[type].SubMenu[idx];
                        dic.Add(item.MenumKey, item);
                    }
                }
            }
            catch (Exception)
            {
            }

            ViewBag.ContractType = type;
            ViewBag.Lines = lines;
            return View("_ContractMenu");
        }

        public class SendMailObject
        {
            public List<doEmailProcess> EmailList { get; set; }
        }
        public void SendMail(doEmailTemplate template, List<string> mailAddress)
        {
            try
            {
                SendMailObject obj = new SendMailObject();
                if (template != null && mailAddress != null)
                {
                    obj.EmailList = new List<doEmailProcess>();
                    foreach (string addr in mailAddress)
                    {
                        doEmailProcess mail = new doEmailProcess()
                        {
                            MailTo = addr,
                            Subject = template.TemplateSubject,
                            Message = template.TemplateContent
                        };
                        obj.EmailList.Add(mail);
                    }
                }

                System.Threading.Thread t = new System.Threading.Thread(SendMail);
                t.Start(obj);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void SendMail(object o)
        {
            try
            {
                SendMailObject obj = o as SendMailObject;
                if (obj == null)
                    return;

                if (obj.EmailList != null)
                {
                    ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    foreach (doEmailProcess mail in obj.EmailList)
                    {
                        chandler.SendMail(mail);
                    }
                }
            }
            catch
            {
            }
        }


        public void CheckTimeLog(string txt)
        {
            try
            {
                string filePath = "c:\\time_log.txt";
                System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath, true);
                wr.WriteLine(string.Format("[{0}] {1}", DateTime.Now.ToString("dd/MM/yy HH:mm:ss:fff"), txt));
                wr.Dispose();
                wr.Close();
            }
            catch (Exception)
            {
            }
        }
    }
}
