using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SECOM_AJIS.PrintService;

namespace SECOM_AJIS.PrintServiceTest
{
    [TestClass]
    public class JobHandlerTest
    {
        [TestMethod]
        public void TestJobHandler()
        {
            try
            {
                DateTime nextRunFrom = DateTime.Now;
                DateTime nextRunTo = nextRunFrom.AddMinutes(10);
                new JobHandler().Execute(nextRunFrom, nextRunTo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Assert.Fail(ex.Message);
            }
            finally
            {
                //
            }
        }
    }
}
