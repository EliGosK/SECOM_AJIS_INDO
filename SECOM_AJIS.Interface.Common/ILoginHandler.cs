using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Common
{
  public  interface ILoginHandler
    {
      /// <summary>
      /// To keep login history
      /// </summary>
      /// <param name="pchrEmpNo"></param>
      /// <param name="pchrLogType"></param>
      /// <returns></returns>
      bool KeepHistory(string pchrEmpNo, string pchrLogType);
      /// <summary>
      /// Check user name with login-domain
      /// </summary>
      /// <param name="Cond"></param>
      /// <returns></returns>
      bool LoginDomain(doLogin Cond);

      bool IsLockedEmployee(string empNo);
    }
}
