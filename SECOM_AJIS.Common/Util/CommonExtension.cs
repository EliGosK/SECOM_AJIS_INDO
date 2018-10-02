using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util
{
    public static class CommonExtension
    {
        /// <summary>
        /// Clone object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static List<T> Clone<T>(this List<T> lst) where T : ICloneable
        {
            var rst = new List<T>();

            rst.AddRange(lst.Select(p => (T)p.Clone()));

            return rst;
        }
    }
}
