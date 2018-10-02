using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Net.Sockets;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class ApprovalPermissionHandler : IApprovalPermissionHandler
    {
        /// <summary>
        /// Check the permitted IP Address of client for approve draft contract
        /// </summary>
        /// <returns></returns>
        public bool isPermittedIPAddress()
        {
            //1.	Load the list of permitted IP Address from config file
            //--Load permitted ip list from config file
            string filePath = string.Format("{0}{1}\\{2}.xml",
                                            CommonUtil.WebPath,
                                            CommonValue.PERMITTED_IPADDR_FOLDER,
                                            CommonValue.PERMITTED_IPADDR_FILE);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filePath);
            XmlNodeList xmlnode = xmldoc.GetElementsByTagName("ipaddress");
            List<String> permittedIPList = new List<string>();
            for (int i = 0; i < xmlnode.Count; i++)
            {
                permittedIPList.Add(xmlnode[i].InnerText);
            }

            //--Load local ip list
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            List<String> localIPList = new List<string>();

            localIPList.Add("localhost");
            localIPList.Add("127.0.0.1");

            foreach (IPAddress hostIP in host.AddressList)
            {

                if (hostIP.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIPList.Add(hostIP.ToString());
                }
            }

            //2.	If client’s IP Address is existing in the list of permitted IP Address
            //Return true
            //      Else
            //Return False
            foreach (String localIP in localIPList)
            {
                foreach (String permittedIP in permittedIPList)
                {
                    if (localIP == permittedIP)
                        return true;
                }
            }

            return false;
        }
    }
}
