using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Models
{
    public class MandatoryField
    {
        public string FieldName { get; set; }
        public string ControlName { get; set; }
        public string MandatoryMessage { get; set; }
    }
    public class ObjectMandatoryField : MandatoryField
    {
        public List<MandatoryField> Properties { get; set; }

        public void AddProperty(string property, string message = null, string control=null)
        {
            MandatoryField mf = new MandatoryField();
            mf.FieldName = property;

            if (message != null)
                mf.MandatoryMessage = message;
            else
                mf.MandatoryMessage = property;

            if (control != null)
                mf.ControlName = control;
            else
                mf.ControlName = property;

            AddProperty(mf);
        }
        public void AddProperty(MandatoryField mf)
        {
            if (mf != null)
            {
                if (this.Properties == null)
                    this.Properties = new List<MandatoryField>();
                this.Properties.Add(mf);
            }
        }
    }
    public class ListMandatoryField : MandatoryField
    {
        public bool IsBreakLoop { get; set; }
        public int? Index { get; set; }

        public ObjectMandatoryField Field { get; set; }
        public List<MandatoryField> Rows { get; set; }

        public void AddRows(string property, string message = null, string control=null)
        {
            MandatoryField mf = new MandatoryField();
            mf.FieldName = property;

            if (message != null)
                mf.MandatoryMessage = message;
            else
                mf.MandatoryMessage = property;

            if (control != null)
                mf.ControlName = control;
            else
                mf.ControlName = property;

            if (this.Rows == null)
                this.Rows = new List<MandatoryField>();
            this.Rows.Add(mf);
        }

    }
}
