using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Models
{
    [Serializable]
    public class TestModel
    {
        private string strCode;
        private string strDiaplayName;

        public bool Flag { get; set; }
        public string Code
        {
            get { return this.strCode; }
            set { this.strCode = value; }
        }
        public string DisplayName
        {
            get { return this.strDiaplayName; }
            set { this.strDiaplayName = value; }
        }
        public DateTime Date { get; set; }
        public decimal? NumDec { get; set; }
    }
}
