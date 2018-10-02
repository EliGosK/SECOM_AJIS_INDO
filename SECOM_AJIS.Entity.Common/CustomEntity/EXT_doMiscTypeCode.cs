using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Common
{
    public partial class doMiscTypeCode
    {
        public string ValueDisplay { get; set; }
        public string ValueCodeDisplay
        {
            get
            {
                return SECOM_AJIS.Common.Util.CommonUtil.TextCodeName(this.ValueCode, this.ValueDisplay);
            }
        }
    }

    #region Misc Type Mapping

    public class MiscTypeMappingList
    {
        #region Inner class

        private class MiscTypeMappingData
        {
            public string MiscTypeKey {get;set;}
            public string MiscTypeCode { get; set; }
            private object MiscTypeObject { get; set; }
            private bool IsEachLanguage { get; set; }
            private PropertyInfo[] MiscTypeNameProperty { get; set; }

            public MiscTypeMappingData(string key, string code, object obj, PropertyInfo[] field)
            {
                MiscTypeKey = key;
                MiscTypeCode = code;
                MiscTypeObject = obj;
                MiscTypeNameProperty = field;
            }
            public void SetMiscTypeData(doMiscTypeCode misc)
            {
                if (misc == null
                    || MiscTypeObject == null
                    || MiscTypeNameProperty == null)
                    return;

                foreach (PropertyInfo prop in MiscTypeNameProperty)
                {
                    if (prop.Name.EndsWith("EN"))
                        prop.SetValue(MiscTypeObject, misc.ValueDisplayEN, null);
                    else if (prop.Name.EndsWith("JP"))
                        prop.SetValue(MiscTypeObject, misc.ValueDisplayJP, null);
                    else if (prop.Name.EndsWith("LC"))
                        prop.SetValue(MiscTypeObject, misc.ValueDisplayLC, null);
                    else
                        prop.SetValue(MiscTypeObject, misc.ValueDisplay, null);
                }
            }
        }

        #endregion
        #region Variables

        private List<MiscTypeMappingData> MiscTypeList { get; set; }

        #endregion
        #region Initial Misc Type Code

        public void AddMiscType(params object[] misc)
        {
            try
            {
                if (misc != null)
                {
                    if (MiscTypeList == null)
                        MiscTypeList = new List<MiscTypeMappingData>();

                    foreach (object m in misc)
                    {
                        Dictionary<string, AMiscTypeMappingAttribute> miscAttr = CommonUtil.CreateAttributeDictionary<AMiscTypeMappingAttribute>(m);
                        foreach(KeyValuePair<string, AMiscTypeMappingAttribute> attr in miscAttr)
                        {
                            PropertyInfo prop = m.GetType().GetProperty(attr.Key);
                            if (prop != null)
                            {
                                object val = prop.GetValue(m, null);
                                if (CommonUtil.IsNullOrEmpty(val) == true)
                                    continue;

                                List<PropertyInfo> propLst = new List<PropertyInfo>();
                                if (attr.Value.SetEachLanguage)
                                {
                                    PropertyInfo enfield = m.GetType().GetProperty(attr.Value.MiscTypeNameField + "EN");
                                    if (enfield != null)
                                        propLst.Add(enfield);
                                    PropertyInfo jpfield = m.GetType().GetProperty(attr.Value.MiscTypeNameField + "JP");
                                    if (jpfield != null)
                                        propLst.Add(jpfield);
                                    PropertyInfo lcfield = m.GetType().GetProperty(attr.Value.MiscTypeNameField + "LC");
                                    if (lcfield != null)
                                        propLst.Add(lcfield);
                                }
                                
                                PropertyInfo field = m.GetType().GetProperty(attr.Value.MiscTypeNameField);
                                if (field != null)
                                    propLst.Add(field);
                                if (propLst.Count > 0)
                                {
                                    MiscTypeMappingData mData = new MiscTypeMappingData(attr.Value.MiscTypeKey, val.ToString(), m, propLst.ToArray());
                                    MiscTypeList.Add(mData);
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

        public List<doMiscTypeCode> GetMiscTypeList()
        {
            try
            {
                List<doMiscTypeCode> mLst = new List<doMiscTypeCode>();
                if (MiscTypeList != null)
                {
                    foreach (MiscTypeMappingData mData in MiscTypeList)
                    {
                        doMiscTypeCode m = null;
                        foreach (doMiscTypeCode mm in mLst)
                        {
                            if (mm.FieldName == mData.MiscTypeKey
                                && mm.ValueCode == mData.MiscTypeCode)
                                m = mm;
                        }
                        if (m == null)
                        {
                            m = new doMiscTypeCode()
                            {
                                FieldName = mData.MiscTypeKey,
                                ValueCode = mData.MiscTypeCode
                            };

                            if (CommonUtil.IsNullOrEmpty(mData.MiscTypeCode) || mData.MiscTypeCode.Trim() == string.Empty)
                                m.ValueCode = "%";
                            mLst.Add(m);
                        }
                    }
                }

                return mLst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SetMiscTypeValue(List<doMiscTypeCode> miscLst)
        {
            try
            {
                if (miscLst != null
                    && MiscTypeList != null)
                {
                    foreach (MiscTypeMappingData mData in MiscTypeList)
                    {
                        foreach (doMiscTypeCode misc in miscLst)
                        {
                            if (mData.MiscTypeKey == misc.FieldName
                                && mData.MiscTypeCode == misc.ValueCode)
                            {
                                mData.SetMiscTypeData(misc);
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
