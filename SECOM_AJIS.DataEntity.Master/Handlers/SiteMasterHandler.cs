using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util.ConstantValue;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using System.Text.RegularExpressions;


namespace SECOM_AJIS.DataEntity.Master
{
    public class SiteMasterHandler : BizMADataEntities, ISiteMasterHandler
    {
        #region Override Methods

        public override List<doSite> GetSite(string siteCode, string custCode) {
            // Akat K. 2011-10-27 : add mandatory check
            // Akat K. 2011-11-01 : remove because MAS010 using with siteCode = null
            //if (siteCode == null) {
            //    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, "siteCode");
            //}

            try {
                List<doSite> lst = base.GetSite(siteCode, custCode);
                if (lst.Count > 0)
                    CommonUtil.MappingObjectLanguage<doSite>(lst);
                return lst;
            } catch (Exception) {
                throw;
            }
        }

        #endregion
        #region Methods

        public void ValidateSiteData(doSite doSite)
        {
            if (doSite == null)
                return;

            doSite.ValidateSiteData = true;
            try
            {
                IMasterHandler mhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                #region Building Usage 

                if (CommonUtil.IsNullOrEmpty(doSite.BuildingUsageCode) == false)
                {
                    List<tbm_BuildingUsage> blst = mhandler.GetTbm_BiuldingUsage();
                    if (blst.Count > 0)
                    {
                        foreach (tbm_BuildingUsage b in blst)
                        {
                            if (doSite.BuildingUsageCode == b.BuildingUsageCode)
                            {
                                doSite.BuildingUsageName = b.BuildingUsageName;
                                doSite.BuildingUsageNameEN = b.BuildingUsageNameEN;
                                doSite.BuildingUsageNameLC = b.BuildingUsageNameLC;
                                doSite.BuildingUsageNameJP = b.BuildingUsageNameJP;
                                break;
                            }
                        }
                    }
                }

                #endregion
                #region Province Data

                if (CommonUtil.IsNullOrEmpty(doSite.ProvinceCode) == false)
                {
                    List<tbm_Province> plst = mhandler.GetTbm_Province();
                    if (plst.Count > 0)
                    {
                        foreach (tbm_Province pv in plst)
                        {
                            if (doSite.ProvinceCode == pv.ProvinceCode)
                            {
                                doSite.ProvinceNameEN = pv.ProvinceNameEN;
                                doSite.ProvinceNameLC = pv.ProvinceNameLC;
                                break;
                            }
                        }
                    }
                }

                #endregion
                #region District

                if (CommonUtil.IsNullOrEmpty(doSite.DistrictCode) == false)
                {
                    List<tbm_District> dlst = mhandler.GetTbm_District(doSite.ProvinceCode);
                    if (dlst.Count > 0)
                    {
                        foreach (tbm_District d in dlst)
                        {
                            if (doSite.ProvinceCode == d.ProvinceCode
                                && doSite.DistrictCode == d.DistrictCode)
                            {
                                doSite.DistrictNameEN = d.DistrictNameEN;
                                doSite.DistrictNameLC = d.DistrictNameLC;
                                break;
                            }
                        }
                    }
                }

                #endregion

                ApplicationErrorException.CheckMandatoryField<doSite, ValidateSite>(doSite);
            }
            catch
            {
                doSite.ValidateSiteData = false;
            }
            try
            {
                doCustomer cust = CommonUtil.CloneObject<doSite, doCustomer>(doSite);

                IMasterHandler mhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                mhandler.CreateAddressFull(cust);

                doSite.AddressFullEN = cust.AddressFullEN;
                doSite.AddressFullLC = cust.AddressFullLC;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        public List<dtSiteData> GetSiteDataForSearch(doSiteSearchCondition cond)
        {
            try
            {
                return base.GetSiteDataForSearch( MiscType.C_CUST_STATUS  ,
                                                 cond.CustomerCode,
                                                 cond.CustomerName,
                                                 cond.SiteCode,
                                                 cond.SiteName,
                                                 cond.BuildingUsageCode,
                                                 cond.Address,
                                                 cond.Alley,
                                                 cond.Road,
                                                 cond.SubDistrict,
                                                 cond.DistrictCode,
                                                 cond.ProvinceCode,
                                                 cond.ZipCode,
                                                 cond.CustStatus);  // pXmlCustStatus
            }
            catch ( Exception ex)
            {
                
                throw ex;
            }
        }

        public void UpdateSite(string strSiteCode, doSite doSite)
        {
            ISiteMasterHandler check = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
            List<CheckUpdateDate> chk  =  check.CheckUpdateDate(strSiteCode);
            //if (chk[0].UpdateDate != dtSite.UpdateDate)
            if (DateTime.Compare(chk[0].UpdateDate.Value, doSite.UpdateDate.Value) != 0) 
            {
                //throw ApplicationErrorException.ThrowErrorException(MessageUtil.Common_MessageList.MSG0019);
            }
            else
            {
                ISiteMasterHandler update = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                update.UpdateSite(strSiteCode, doSite.CustCode, doSite.SiteNo, doSite.SiteNameEN, doSite.SiteNameLC, doSite.SECOMContactPerson, doSite.PersonInCharge
                , doSite.PhoneNo, doSite.BuildingUsageCode, doSite.AddressEN, doSite.AlleyEN, doSite.RoadEN, doSite.SubDistrictEN, doSite.AddressFullEN, doSite.AddressLC, doSite.AlleyLC, doSite.RoadLC, doSite.SubDistrictLC, doSite.AddressFullLC, doSite.DistrictCode, doSite.ProvinceCode, doSite.ZipCode, doSite.UpdateDate, doSite.UpdateBy);
               
            }
        }

        public bool CheckExistSiteData(string siteCode, string custCode)
        {
            try
            {
                List<int?> lst = this.CheckExistSite(siteCode, custCode);
                if (lst != null)
                {
                    if (lst.Count > 0)
                        return lst[0] == 1;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckDuplicateSiteData(doSite doSite)
        {
            try
            {
                doSite site = CommonUtil.CloneObject<doSite, doSite>(doSite);
                if (site != null)
                {
                    //32  AND 46 	= 	space !"#$%&'()*+,-.
                    //58  AND 64 	= 	:;<=>?@
                    //91  AND 96 	= 	[\]^_`
                    //123 AND 125 	= 	{|}

                    if (CommonUtil.IsNullOrEmpty(site.SiteNameLC) == false)
                        site.SiteNameLC = Regex.Replace(site.SiteNameLC, "[ !\"#$%&'()*+,-.:;<=>?@[\\]^_`{|}]", "");
                    if (CommonUtil.IsNullOrEmpty(site.AddressFullLC) == false)
                        site.AddressFullLC = Regex.Replace(site.AddressFullLC, "[ !\"#$%&'()*+,-.:;<=>?@[\\]^_`{|}]", "");

                    List<int?> lst = base.CheckDuplicateSite(
                                        site.SiteCode
                                        , site.CustCode
                                        , site.SiteNameLC, 
                                        site.AddressFullLC);
                    
                    if (lst.Count > 0 && lst[0] > 0)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<tbm_Site> InsertSite(doSite site)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                dsTransDataModel dsTrans = CommonUtil.dsTransData;
                site.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                site.CreateBy = dsTrans.dtUserData.EmpNo;
                site.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                site.UpdateBy = dsTrans.dtUserData.EmpNo;

                ApplicationErrorException.CheckMandatoryField<doSite, tbm_SiteCondition>(site);

                List<tbm_Site> lst = this.InsertSite(site.SiteCode,
                                                        site.CustCode,
                                                        site.SiteNo,
                                                        site.SiteNameEN,
                                                        site.SiteNameLC,
                                                        site.SECOMContactPerson,
                                                        site.PersonInCharge,
                                                        site.PhoneNo,
                                                        site.BuildingUsageCode,
                                                        site.AddressEN,
                                                        site.AlleyEN,
                                                        site.RoadEN,
                                                        site.SubDistrictEN,
                                                        site.AddressFullEN,
                                                        site.AddressLC,
                                                        site.AlleyLC,
                                                        site.RoadLC,
                                                        site.SubDistrictLC,
                                                        site.AddressFullLC,
                                                        site.DistrictCode,
                                                        site.ProvinceCode,
                                                        site.ZipCode,
                                                        site.CreateDate,
                                                        site.CreateBy,
                                                        site.UpdateDate,
                                                        site.UpdateBy);
                //Insert Log
                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_SITE;
                    logData.TableData = CommonUtil.ConvertToXml(lst);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<tbm_Site> UpdateSite(doSite site)
        {
            try
            {
                ApplicationErrorException.CheckMandatoryField<doSite, tbm_SiteCondition>(site);
                             
                //Check whether this record is the most updated data
                List<doGetTbm_Site> sList = this.GetTbm_Site(site.SiteCode);
                if (DateTime.Compare(sList[0].UpdateDate.Value, site.UpdateDate.Value) != 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                }

                dsTransDataModel dsTrans = CommonUtil.dsTransData;
                site.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                site.CreateBy = dsTrans.dtUserData.EmpNo;
                site.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                site.UpdateBy = dsTrans.dtUserData.EmpNo;

                List<tbm_Site> lst = this.UpdateSite(site.SiteCode,
                                                        site.CustCode,
                                                        site.SiteNo,
                                                        site.SiteNameEN,
                                                        site.SiteNameLC,
                                                        site.SECOMContactPerson,
                                                        site.PersonInCharge,
                                                        site.PhoneNo,
                                                        site.BuildingUsageCode,
                                                        site.AddressEN,
                                                        site.AlleyEN,
                                                        site.RoadEN,
                                                        site.SubDistrictEN,
                                                        site.AddressFullEN,
                                                        site.AddressLC,
                                                        site.AlleyLC,
                                                        site.RoadLC,
                                                        site.SubDistrictLC,
                                                        site.AddressFullLC,
                                                        site.DistrictCode,
                                                        site.ProvinceCode,
                                                        site.ZipCode,
                                                        site.UpdateDate,
                                                        site.UpdateBy);

                //Insert Log
                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_SITE;
                    logData.TableData = CommonUtil.ConvertToXml(lst);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<tbm_Site> DeleteSite(doSite site)
        {
            try
            {
                ApplicationErrorException.CheckMandatoryField<doSite, tbm_SiteCondition>(site);

                //Check whether this record is the most updated data
                List<doGetTbm_Site> sList = this.GetTbm_Site(site.SiteCode);
                if (DateTime.Compare(sList[0].UpdateDate.Value, site.UpdateDate.Value) != 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                }

                List<tbm_Site> lst = this.DeleteSite(site.SiteCode);

                //Insert Log
                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_SITE;
                    logData.TableData = CommonUtil.ConvertToXml(lst);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
