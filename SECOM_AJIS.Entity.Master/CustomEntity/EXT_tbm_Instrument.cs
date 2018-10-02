using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Master
{
    public partial class tbm_Instrument
    {
    }

    #region Instrument Mapping

    public class InstrumentMappingList
    {
        #region Inner class

        private class InstrumentMappingData
        {
            public string InstrumentCode { get; set; }
            private object InstrumentObject { get; set; }
            private PropertyInfo[] InstrumentProperty { get; set; }

            public InstrumentMappingData(string key, object obj, params PropertyInfo[] fields)
            {
                InstrumentCode = key;
                InstrumentObject = obj;
                InstrumentProperty = fields;
            }

            public void SetInstrumentData(tbm_Instrument inst)
            {
                if (inst == null
                    || InstrumentProperty == null
                    || InstrumentObject == null)
                    return;

                foreach (PropertyInfo prop in InstrumentProperty)
                {
                    PropertyInfo ipop = inst.GetType().GetProperty(prop.Name);
                    if (ipop != null)
                        prop.SetValue(InstrumentObject, ipop.GetValue(inst, null), null);
                }
            }
        }

        #endregion
        #region Variables

        private List<InstrumentMappingData> InstrumentList { get; set; }

        #endregion
        #region Initial Instrument

        public void AddInstrument(params object[] instLst)
        {
            try
            {
                if (instLst != null)
                {
                    if (InstrumentList == null)
                        InstrumentList = new List<InstrumentMappingData>();

                    foreach (object inst in instLst)
                    {
                        Dictionary<string, InstrumentMappingAttribute> instAttr = CommonUtil.CreateAttributeDictionary<InstrumentMappingAttribute>(inst);
                        foreach (KeyValuePair<string, InstrumentMappingAttribute> attr in instAttr)
                        {
                            PropertyInfo prop = inst.GetType().GetProperty(attr.Key);
                            if (prop != null)
                            {
                                object val = prop.GetValue(inst, null);
                                if (CommonUtil.IsNullOrEmpty(val) == true)
                                    continue;

                                if (attr.Value.InstrumentField != null)
                                {
                                    List<PropertyInfo> pLst = new List<PropertyInfo>();
                                    foreach (string field in attr.Value.InstrumentField)
                                    {
                                        PropertyInfo p = inst.GetType().GetProperty(field);
                                        if (p != null)
                                            pLst.Add(p);
                                    }

                                    if (pLst.Count > 0)
                                    {
                                        InstrumentMappingData instData = new InstrumentMappingData(val.ToString(), inst, pLst.ToArray());
                                        InstrumentList.Add(instData);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        #region Database Mapping

        public List<tbm_Instrument> GetInstrumentList()
        {
            try
            {
                List<tbm_Instrument> instLst = new List<tbm_Instrument>();
                if (InstrumentList != null)
                {
                    foreach (InstrumentMappingData instData in InstrumentList)
                    {
                        tbm_Instrument inst = null;
                        foreach (tbm_Instrument i in instLst)
                        {
                            if (i.InstrumentCode == instData.InstrumentCode)
                            {
                                inst = i;
                                break;
                            }
                        }
                        if (inst == null)
                        {
                            inst = new tbm_Instrument()
                            {
                                InstrumentCode = instData.InstrumentCode
                            };
                            instLst.Add(inst);
                        }
                    }
                }

                return instLst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SetInstrumentValue(List<tbm_Instrument> instLst)
        {
            try
            {
                if (instLst != null
                    && InstrumentList != null)
                {
                    foreach (InstrumentMappingData instData in InstrumentList)
                    {
                        foreach (tbm_Instrument inst in instLst)
                        {
                            if (instData.InstrumentCode == inst.InstrumentCode)
                            {
                                instData.SetInstrumentData(inst);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }

    #endregion
}
