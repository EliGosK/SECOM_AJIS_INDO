using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of office list
    /// </summary>
    public partial class doOfficeList
    {
        public string OfficeName { get; set; }

    }

    #region Office Mapping
    /// <summary>
    /// Do Of office mapping list
    /// </summary>
    public class OfficeMappingList
    {
        #region Inner class

        private class OfficeMappingData
        {
            public string OfficeNo { get; set; }
            private object OfficeObject { get; set; }
            private PropertyInfo OfficeNameProperty { get; set; }

            public OfficeMappingData(string key, object obj, PropertyInfo field)
            {
                OfficeNo = key;
                OfficeObject = obj;
                OfficeNameProperty = field;
            }
            public void SetOfficeData(doOfficeList office)
            {
                if (office == null
                    || OfficeNameProperty == null
                    || OfficeObject == null)
                    return;

                OfficeNameProperty.SetValue(OfficeObject, office.OfficeName, null);
            }
        }

        #endregion
        #region Variables

        private List<OfficeMappingData> OfficeList { get; set; }

        #endregion
        #region Initial Office

        public void AddOffice(params object[] office)
        {
            try
            {
                if (office != null)
                {
                    if (OfficeList == null)
                        OfficeList = new List<OfficeMappingData>();

                    foreach (object o in office)
                    {
                        Dictionary<string, OfficeMappingAttribute> officeAttr = CommonUtil.CreateAttributeDictionary<OfficeMappingAttribute>(o);
                        foreach (KeyValuePair<string, OfficeMappingAttribute> attr in officeAttr)
                        {
                            PropertyInfo prop = o.GetType().GetProperty(attr.Key);
                            if (prop != null)
                            {
                                object val = prop.GetValue(o, null);
                                if (CommonUtil.IsNullOrEmpty(val) == true)
                                    continue;

                                PropertyInfo field = o.GetType().GetProperty(attr.Value.OfficeNameField);
                                if (field != null)
                                {
                                    OfficeMappingData officeData = new OfficeMappingData(val.ToString(), o, field);
                                    OfficeList.Add(officeData);
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

        public List<tbm_Office> GetOfficeList()
        {
            try
            {
                List<tbm_Office> officeLst = new List<tbm_Office>();
                if (OfficeList != null)
                {
                    foreach (OfficeMappingData officeData in OfficeList)
                    {
                        tbm_Office o = new tbm_Office()
                        {
                            OfficeCode = officeData.OfficeNo
                        };
                        officeLst.Add(o);
                    }
                }

                return officeLst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SetOfficeValue(List<doOfficeList> officeLst)
        {
            try
            {
                if (officeLst != null
                    && OfficeList != null)
                {
                    foreach (OfficeMappingData officeData in OfficeList)
                    {
                        foreach (doOfficeList o in officeLst)
                        {
                            if (officeData.OfficeNo == o.OfficeCode)
                            {
                                officeData.SetOfficeData(o);
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
    #region Attribute
    /// <summary>
    /// Do Of office mapping attribute
    /// </summary>
    public class OfficeMappingAttribute : Attribute
    {
        public string OfficeNameField { get; set; }
        
        public OfficeMappingAttribute(string OfficeNameField)
        {
            this.OfficeNameField = OfficeNameField;
        }
    }

    #endregion
}
