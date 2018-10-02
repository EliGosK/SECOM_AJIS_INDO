using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SECOM_AJIS.DataEntity.Quotation;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using System.Diagnostics;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.Controllers
{
    public class SwtIVP020Controller : SwtCommonController
    {
        //
        // GET: /SwtIVP020/

        public string Index()
        {
            List<String> result = new List<string>();

            result.Add(Case01());
            result.Add(Case02());
            result.Add(Case03());
            result.Add(Case04());
            result.Add(Case05());

            return CommonUtil.TextList(result.ToArray(), "<br />");
        }

        ///<summary>
        ///Purpose   : IF  Dep.ContactCode isn't Empty
        ///Parameters: Input No 1
        ///Expected  : Expect Result 1
        ///</summary>
        ///
        public string Case01()
        {
            IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            string res = invenhandler.InsertDepreciationData(Input01());

            return String.Format("Cast 1 Result: {0}", res);
        }

        ///<summary>
        ///Purpose   : change Config of depreciation period
        ///Parameters: Input No 2
        ///Expected  : Expect Result 2
        ///</summary>
        ///
        public string Case02()
        {
            IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            string res = invenhandler.InsertDepreciationData(Input02());

            return String.Format("Cast 2 Result: {0}", res);
        }

        ///<summary>
        ///Purpose   : change  SCRAP value
        ///Parameters: Input No 3
        ///Expected  : Expect Result 3
        ///</summary>
        ///
        public string Case03()
        {
            IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            string res = invenhandler.InsertDepreciationData(Input03());

            return String.Format("Cast 3 Result: {0}", res);
        }

        ///<summary>
        ///Purpose   : previous period preocess
        ///Parameters: -
        ///Expected  : Expect Result update 1
        ///</summary>
        ///
        public string Case04()
        {
            return "Case 4: Run batch from CMS050";
        }

        ///<summary>
        ///Purpose   : previous period preocess
        ///Parameters: -
        ///Expected  : Expect Result update 2
        ///</summary>
        ///
        public string Case05()
        {
            return "Case 5: Run batch from CMS050";
        }

        #region Input Data
        public doInsertDepreciationData Input01()
        {
            doInsertDepreciationData result = new doInsertDepreciationData()
            {
                ContractCode = "MA0000000513", 
                InstrumentCode = "1001",
                StartYearMonth = "032012",
                DepreciationPeriod = 6,
                MovingAveragePrice = 100
            };

            return result;
        }

        public doInsertDepreciationData Input02()
        {
            doInsertDepreciationData result = new doInsertDepreciationData()
            {
                ContractCode = null,
                InstrumentCode = "1001",
                StartYearMonth = "032012",
                DepreciationPeriod = 15,
                MovingAveragePrice = 100
            };

            return result;
        }

        public doInsertDepreciationData Input03()
        {
            doInsertDepreciationData result = new doInsertDepreciationData()
            {
                ContractCode = null,
                InstrumentCode = "1002",
                StartYearMonth = "032012",
                DepreciationPeriod = 60,
                MovingAveragePrice = 200
            };

            return result;
        }

        public doInsertDepreciationData Input04()
        {
            doInsertDepreciationData result = new doInsertDepreciationData()
            {
                ContractCode = null,
                InstrumentCode = "1002",
                StartYearMonth = "032012",
                DepreciationPeriod = 60,
                MovingAveragePrice = 200
            };

            return result;
        }
        #endregion
    }
}
