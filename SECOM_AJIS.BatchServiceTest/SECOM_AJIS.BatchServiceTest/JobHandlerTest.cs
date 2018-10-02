using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SECOM_AJIS.BatchService;
using System.Threading;
using System.Diagnostics;

namespace SECOM_AJIS.BatchServiceTest
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
                Console.WriteLine("### テスト完了 ###");
            }
        }

        [TestMethod]
        public void MailTestJobHandler()
        {
            try
            {
                new JobHandler().sendMail();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                //
                Console.WriteLine("###  メール テスト完了 ###");
            }
        }

    }
}
