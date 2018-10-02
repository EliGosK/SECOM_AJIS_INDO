using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of customer target
    /// </summary>
    public class doCustomerTarget
    {
        private doCustomer _doCustomer;
        public doCustomer doCustomer
        {
            get { return this._doCustomer; }
            set { this._doCustomer = value; }
        }

        private List<dtCustomerGroup> _dtCustomerGroup;
        public List<dtCustomerGroup> dtCustomerGroup
        {
            get { return this._dtCustomerGroup; }
            set { this._dtCustomerGroup = value; }
        }

        private doSite _doSite;
        public doSite doSite
        {
            get { return this._doSite; }
            set { this._doSite = value; }
        }

    }
}
