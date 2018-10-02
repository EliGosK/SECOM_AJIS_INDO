using System;
using System.Collections.Generic;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;
using System.Linq;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public class QuotationHandler : BizQUDataEntities, IQuotationHandler
    {
        #region Get Methods
        
        /// <summary>
        /// Search quotation target data (Call stored procedure: sp_QU_SearchQuotationTargetList)
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public List<dtSearchQuotationTargetListResult> SearchQuotationTargetList(doSearchQuotationTargetListCondition Cond)
        {
            try
            {

                List<dtSearchQuotationTargetListResult> lst = base.SearchQuotationTargetList(Cond.QuotationTargetCode,
                                                        Cond.ProductTypeCode,
                                                        Cond.QuotationOfficeCode,
                                                        Cond.OperationOfficeCode,
                                                        Cond.ContractTargetCode,
                                                        Cond.ContractTargetName,
                                                        Cond.ContractTargetAddr,
                                                        Cond.SiteCode,
                                                        Cond.SiteName,
                                                        Cond.SiteAddr,
                                                        Cond.StaffCode,
                                                        Cond.StaffName,
                                                        Cond.QuotationDateFrom,
                                                        Cond.QuotationDateTo,
                                                        CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET,
                                                        TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE,
                                                        ContractTransferStatus.C_CONTRACT_TRANS_STATUS_CONTRACT_APP,
                                                        TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE,
                                                        CommonUtil.ConvertToXml_Store<OfficeDataDo>(CommonUtil.dsTransData.dtOfficeData));

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Search quotation data (Call stored procedure: sp_QU_SearchQuotationList)
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public List<dtSearchQuotationListResult> SearchQuotationList(doSearchQuotationListCondition Cond)
        {
            return base.SearchQuotationList(CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET,
                                            Cond.QuotationTargetCode,
                                            Cond.Alphabet,
                                            Cond.ProductTypeCode,
                                            Cond.LockStatus,
                                            Cond.QuotationOfficeCode,
                                            Cond.OperationOfficeCode,
                                            Cond.ContractTargetCode,
                                            Cond.ContractTargetName,
                                            Cond.ContractTargetAddr,
                                            Cond.SiteCode,
                                            Cond.SiteName,
                                            Cond.SiteAddr,
                                            Cond.EmpNo,
                                            Cond.EmpName,
                                            Cond.QuotationDateFrom,
                                            Cond.QuotationDateTo,
                                            Cond.ServiceTypeCode,
                                            Cond.TargetCodeTypeCode,
                                            Cond.ContractTransferStatus);
        }
        /// <summary>
        /// Get data from tbt_QuotationTarget (Call stored procedure: sp_QU_GetTbt_QuotationTarget)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<tbt_QuotationTarget> GetTbt_QuotationTarget(doGetQuotationDataCondition cond)
        {
            try
            {
                /* --- Check Mandatory --- */
                /* ----------------------- */
                ApplicationErrorException.CheckMandatoryField<doGetQuotationDataCondition, doQuotationDataForGetTbt_TargetCondition>(cond);
                /* ----------------------- */

                List<tbt_QuotationTarget> lst = this.GetTbt_QuotationTarget(cond.QuotationTargetCode,
                                                                            cond.ServiceTypeCode,
                                                                            cond.TargetCodeTypeCode,
                                                                            null);
                if (lst.Count > 0)
                    CommonUtil.MappingObjectLanguage<tbt_QuotationTarget>(lst);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get data from tbt_QuotationTarget (Call stored procedure: sp_QU_GetTbt_QuotationTarget)
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        public List<tbt_QuotationTarget> GetTbt_QuotationTargetByContractCode(string contractCode)
        {
            try
            {
                List<tbt_QuotationTarget> lst = this.GetTbt_QuotationTarget(null, null, null, contractCode);
                if (lst.Count > 0)
                    CommonUtil.MappingObjectLanguage<tbt_QuotationTarget>(lst);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get quotation data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public dsQuotationData GetQuotationData(doGetQuotationDataCondition cond)
        {
            try
            {
                if (cond == null)
                    cond = new doGetQuotationDataCondition();

                if (cond.ContractFlag == true)
                    ApplicationErrorException.CheckMandatoryField<doGetQuotationDataCondition, doQuotationDataForContractCondition>(cond);

                dsQuotationData qData = new dsQuotationData();

                //initial list
                qData.dtTbt_QuotationCustomer = new List<tbt_QuotationCustomer>();
                qData.dtTbt_QuotationInstrumentDetails = new List<tbt_QuotationInstrumentDetails>();
                qData.dtTbt_QuotationOperationType = new List<tbt_QuotationOperationType>();
                qData.dtTbt_QuotationFacilityDetails = new List<tbt_QuotationFacilityDetails>();
                qData.dtTbt_QuotationSentryGuardDetails = new List<tbt_QuotationSentryGuardDetails>();
                qData.dtTbt_QuotationMaintenanceLinkage = new List<tbt_QuotationMaintenanceLinkage>();

                #region Quotation Target Data

                List<tbt_QuotationTarget> qtList = this.GetTbt_QuotationTarget(cond);
                if (qtList.Count <= 0)
                    return null;
                else
                    qData.dtTbt_QuotationTarget = qtList[0];

                #endregion
                #region Quotation Basic Data

                List<tbt_QuotationBasic> qbList = this.GetQuotationBasicData(cond);
                if (qbList.Count <= 0)
                    return null;
                else
                {
                    CommonUtil c = new CommonUtil();
                    if (cond.ContractFlag == true && qbList[0].LockStatus == LockStatus.C_LOCK_STATUS_LOCK)
                        throw ApplicationErrorException.ThrowErrorException(
                            MessageUtil.MODULE_QUOTATION,
                            MessageUtil.MessageList.MSG2007,
                            c.ConvertQuotationTargetCode(cond.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT));

                    qData.dtTbt_QuotationBasic = qbList[0];
                }

                #endregion
                #region Quotation Customer Data

                qData.dtTbt_QuotationCustomer = this.GetTbt_QuotationCustomer(cond);

                #endregion
                #region Quotation Site Data

                List<tbt_QuotationSite> qsList = this.GetTbt_QuotationSite(cond);
                if (qsList.Count > 0)
                    qData.dtTbt_QuotationSite = qsList[0];

                #endregion
                #region Quotation Instrument Details

                if (qData.dtTbt_QuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_SALE
                    || qData.dtTbt_QuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                    || qData.dtTbt_QuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                {
                    qData.dtTbt_QuotationInstrumentDetails = this.GetTbt_QuotationInstrumentDetails(cond);
                }

                #endregion
                #region Quotation Operation Type / Facility Details

                if (qData.dtTbt_QuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                    || qData.dtTbt_QuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE
                    || qData.dtTbt_QuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                {
                    qData.dtTbt_QuotationOperationType = this.GetTbt_QuotationOperationType(cond);
                    qData.dtTbt_QuotationFacilityDetails = this.GetTbt_QuotationFacilityDetails(cond);
                }

                #endregion
                #region Quotation Beat Guard Details

                if (qData.dtTbt_QuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_BE)
                {
                    List<tbt_QuotationBeatGuardDetails> qbgList = this.GetTbt_QuotationBeatGuardDetails(cond);
                    if (qbgList.Count > 0)
                        qData.dtTbt_QuotationBeatGuardDetails = qbgList[0];
                }

                #endregion
                #region Quotation Sentry Guard Details

                if (qData.dtTbt_QuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_SG)
                    qData.dtTbt_QuotationSentryGuardDetails = this.GetTbt_QuotationSentryGuardDetails(cond);

                #endregion
                #region Quotation Maintenance Linkage

                if (qData.dtTbt_QuotationBasic != null)
                {
                    if (qData.dtTbt_QuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_MA
                        && qData.dtTbt_QuotationBasic.MaintenanceTargetProductTypeCode == MaintenanceTargetProductType.C_MA_TARGET_PROD_TYPE_SECOM)
                    {
                        qData.dtTbt_QuotationMaintenanceLinkage = this.GetTbt_QuotationMaintenanceLinkage(cond);
                    }
                }

                #endregion

                return qData;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get sale quotation data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public doSaleQuotationData GetSaleQuotationData(doGetQuotationDataCondition cond)
        {
            try
            {
                doSaleQuotationData sqData = new doSaleQuotationData();

                #region Set Quotation Basic Data

                List<tbt_QuotationBasic> qbLst = this.GetQuotationBasicData(cond);
                if (qbLst.Count == 0)
                {
                    CommonUtil cmm = new CommonUtil();

                    throw ApplicationErrorException.ThrowErrorException(
                                MessageUtil.MODULE_QUOTATION,
                                MessageUtil.MessageList.MSG2006,
                                    CommonUtil.TextCodeName(
                                            cmm.ConvertQuotationTargetCode(cond.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                                            cond.Alphabet,
                                            "-"));
                }
                sqData.dtTbt_QuotationBasic = qbLst[0];

                #endregion
                #region Set Quotation Header Data

                sqData.doQuotationHeaderData = this.GetQuotationHeaderData(cond);
                if (sqData.doQuotationHeaderData == null)
                {
                    CommonUtil cmm = new CommonUtil();

                    throw ApplicationErrorException.ThrowErrorException(
                                    MessageUtil.MODULE_QUOTATION,
                                    MessageUtil.MessageList.MSG2003,
                                    cmm.ConvertQuotationTargetCode(cond.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT));
                }

                #endregion

                if (sqData.doQuotationHeaderData.doQuotationTarget != null)
                {
                    #region Set Instrument Detail

                    cond.ProductTypeCode = sqData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode;
                    sqData.InstrumentDetailList = this.GetInstrumentDetail(cond);

                    #endregion
                    #region Set Product Name

                    IProductMasterHandler mhandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                    List<View_tbm_Product> pLst = mhandler.GetTbm_ProductByLanguage(
                            sqData.dtTbt_QuotationBasic.ProductCode,
                            null);

                    if (pLst.Count > 0)
                        sqData.dtTbt_QuotationBasic.ProductName = pLst[0].ProductName;

                    #endregion
                }

                #region Set Misc Name

                MiscTypeMappingList miscLst = new MiscTypeMappingList();
                miscLst.AddMiscType(sqData.dtTbt_QuotationBasic);

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                chandler.MiscTypeMappingList(miscLst);

                #endregion
                #region Set Employee Name

                EmployeeMappingList empLst = new EmployeeMappingList();
                empLst.AddEmployee(sqData.dtTbt_QuotationBasic);

                IEmployeeMasterHandler ehandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                ehandler.EmployeeListMapping(empLst);

                #endregion

                return sqData;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get rental quotation data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public doRentalQuotationData GetRentalQuotationData(doGetQuotationDataCondition cond)
        {
            try
            {
                doRentalQuotationData qData = new doRentalQuotationData();

                #region Set Quotation Basic.

                List<tbt_QuotationBasic> qbLst = this.GetQuotationBasicData(cond);
                if (qbLst.Count == 0)
                {
                    CommonUtil cmm = new CommonUtil();

                    throw ApplicationErrorException.ThrowErrorException(
                                MessageUtil.MODULE_QUOTATION,
                                MessageUtil.MessageList.MSG2006,
                                    CommonUtil.TextCodeName(
                                            cmm.ConvertQuotationTargetCode(cond.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                                            cond.Alphabet,
                                            "-"));
                }
                qData.dtTbt_QuotationBasic = qbLst[0];

                #endregion
                #region Set Quotation Header Data.

                qData.doQuotationHeaderData = this.GetQuotationHeaderData(cond);
                if (qData.doQuotationHeaderData == null)
                {
                    CommonUtil cmm = new CommonUtil();

                    throw ApplicationErrorException.ThrowErrorException(
                                    MessageUtil.MODULE_QUOTATION,
                                    MessageUtil.MessageList.MSG2003,
                                    cmm.ConvertQuotationTargetCode(cond.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT));
                }

                #endregion

                if (qData.doQuotationHeaderData.doQuotationTarget != null)
                {
                    #region Set Quotation Operation Type / Instrument detail / Facility Details

                    if (qData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                        || qData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE
                        || qData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                    {
                        #region Get Operation Type

                        qData.OperationTypeList = this.GetQuotationOperationType(cond);

                        #endregion
                        #region Get Instrument

                        if (qData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                            || qData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                        {
                            cond.ProductTypeCode = qData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode;
                            qData.InstrumentDetailList = this.GetInstrumentDetail(cond);
                        }

                        #endregion
                        #region Get Facility

                        qData.FacilityDetailList = this.GetFacilityDetail(cond);

                        #endregion
                    }

                    #endregion
                    #region Set Beat guard detail

                    if (qData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_BE)
                    {
                        qData.doBeatGuardDetail = this.GetBeatGuardDetail(cond);
                    }

                    #endregion
                    #region Set Senty guard detail

                    if (qData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_SG)
                        qData.SentryGuardDetailList = this.GetSentryGuardDetail(cond);

                    #endregion
                    #region Set maintenance linkage contract

                    if (qData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_MA
                        && qData.dtTbt_QuotationBasic.MaintenanceTargetProductTypeCode == MaintenanceTargetProductType.C_MA_TARGET_PROD_TYPE_SECOM)
                    {
                        List<tbt_QuotationMaintenanceLinkage> qMain = this.GetTbt_QuotationMaintenanceLinkage(cond);
                        if (qMain.Count > 0)
                        {
                            List<tbt_SaleBasic> contractLst = new List<tbt_SaleBasic>();
                            foreach (tbt_QuotationMaintenanceLinkage qm in qMain)
                            {
                                contractLst.Add(new tbt_SaleBasic()
                                {
                                    ContractCode = qm.ContractCode
                                });
                            }

                            IContractHandler handler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                            qData.MaintenanceTargetList = handler.GetContractHeaderDataByLanguage(contractLst);
                        }
                    }

                    #endregion
                    #region Set Installation information

                    if ((qData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                            || qData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                        && qData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE)
                    {
                        IRentralContractHandler rhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                        qData.FirstInstallCompleteFlag = rhandler.IsFirstInstallComplete(cond.QuotationTargetCode);
                    }

                    #endregion
                    #region Set linkage sale contract data

                    if (qData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE)
                    {
                        qData.doLinkageSaleContractData = null;
                        ISaleContractHandler shandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                        if (qData.dtTbt_QuotationBasic.SaleOnlineContractCode != null)
                        {
                            doSaleContractData doSaleContract = shandler.GetSaleContractData(qData.dtTbt_QuotationBasic.SaleOnlineContractCode, null);
                            if (doSaleContract != null)
                                qData.doLinkageSaleContractData = this.InitLinkageSaleContractData(doSaleContract);
                        }
                    }

                    #endregion
                    #region Set Product Name

                    IProductMasterHandler mhandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                    List<View_tbm_Product> pLst = mhandler.GetTbm_ProductByLanguage(
                            qData.dtTbt_QuotationBasic.ProductCode,
                           null);

                    if (pLst.Count > 0)
                        qData.dtTbt_QuotationBasic.ProductName = pLst[0].ProductName;

                    #endregion
                }

                #region Set Misc Name

                MiscTypeMappingList miscLst = new MiscTypeMappingList();
                miscLst.AddMiscType(qData.dtTbt_QuotationBasic);

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                chandler.MiscTypeMappingList(miscLst);

                #endregion
                #region Set Employee Name

                EmployeeMappingList empLst = new EmployeeMappingList();
                empLst.AddEmployee(qData.dtTbt_QuotationBasic);

                IEmployeeMasterHandler ehandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                ehandler.EmployeeListMapping(empLst);

                #endregion

                return qData;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get quotation header data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public doQuotationHeaderData GetQuotationHeaderData(doGetQuotationDataCondition cond)
        {
            try
            {
                /* --- Check Mandatory Field. --- */
                /* ------------------------------ */
                ApplicationErrorException.CheckMandatoryField(cond);
                /* ------------------------------ */

                doQuotationHeaderData qHData = new doQuotationHeaderData();

                #region Set Quotation Target.

                List<doQuotationTarget> qtLst = this.GetQuotationTarget(MiscType.C_ACQUISITION_TYPE,
                                                                        MiscType.C_MOTIVATION_TYPE,
                                                                        cond.QuotationTargetCode);
                if (qtLst.Count > 0)
                {
                    CommonUtil.MappingObjectLanguage(qtLst[0]);
                    qHData.doQuotationTarget = qtLst[0];
                }

                #endregion
                #region Set Contract Target / Read Customer

                List<doQuotationCustomer> cLst = this.GetQuotationCustomer(cond.QuotationTargetCode);
                if (cLst.Count > 0)
                {
                    foreach (doQuotationCustomer cDo in cLst)
                    {
                        if (cDo.CustPartTypeCode == CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET)
                        {
                            if (qHData.doContractTarget == null)
                                qHData.doContractTarget = cDo;
                        }
                        else if (cDo.CustPartTypeCode == CustPartType.C_CUST_PART_TYPE_REAL_CUST)
                        {
                            if (qHData.doRealCustomer == null)
                                qHData.doRealCustomer = cDo;
                        }
                    }
                }

                #endregion
                #region Set Quotation Site

                List<doQuotationSite> qsLst = this.GetQuotationSite(cond.QuotationTargetCode);
                if (qsLst.Count > 0)
                {
                    qHData.doQuotationSite = qsLst[0];
                    if (qHData.doQuotationSite.SiteCode != null)
                    {
                        if (qHData.doQuotationSite.SiteCode.Length >= 4)
                            qHData.doQuotationSite.SiteNo = qHData.doQuotationSite.SiteCode.Substring(qHData.doQuotationSite.SiteCode.Length - 4);
                    }
                }

                #endregion

                #region Set Quotation Installation Detail

                if (qHData.doQuotationTarget != null)
                {
                    var qid = this.GetTbt_QuotationInstallationDetail(qHData.doQuotationTarget.QuotationTargetCode, qHData.doQuotationTarget.LastAlphabet);
                    if (qid.Count > 0)
                    {
                        qHData.doQuotationInstallationDetail = qid.FirstOrDefault();
                    }
                }

                #endregion

                if (qHData.doContractTarget == null
                    && qHData.doQuotationSite == null
                    && qHData.doQuotationTarget == null
                    && qHData.doRealCustomer == null)
                    qHData = null;

                return qHData;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get default instrument data (Call stored procedure: sp_QU_GetDefaultInstrument)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<doDefaultInstrument> GetDefaultInstrument(doDefaultInstrumentCondition cond)
        {
            try
            {
                /* --- Check Mandatory Field --- */
                /* ----------------------------- */
                ApplicationErrorException.CheckMandatoryField(cond);
                /* ----------------------------- */

                List<doDefaultInstrument> lst = base.GetDefaultInstrument(cond.ProductCode, cond.ProductTypeCode,
                                                    SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_LINE_UP_TYPE,
                                                    SECOM_AJIS.Common.Util.ConstantValue.LineUpType.C_LINE_UP_TYPE_STOP_SALE,
                                                    SECOM_AJIS.Common.Util.ConstantValue.LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE,
                                                    SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_SALE,
                                                    SECOM_AJIS.Common.Util.ConstantValue.InstrumentType.C_INST_TYPE_GENERAL,
                                                    SECOM_AJIS.Common.Util.ConstantValue.ExpansionType.C_EXPANSION_TYPE_PARENT,
                                                    cond.SaleFlag,
                                                    cond.RentalFlag);

                if (lst == null)
                    lst = new List<doDefaultInstrument>();
                else
                    CommonUtil.MappingObjectLanguage<doDefaultInstrument>(lst);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get default facility data (Call stroed procedure: sp_QU_GetDefaultFacility)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<doDefaultFacility> GetDefaultFacility(doDefaultFacilityCondition cond)
        {
            try
            {
                /* --- Check Mandatory Field --- */
                ApplicationErrorException.CheckMandatoryField(cond);

                List<doDefaultFacility> lst = base.GetDefaultFacility(cond.ProductCode,
                                                    SECOM_AJIS.Common.Util.ConstantValue.LineUpType.C_LINE_UP_TYPE_STOP_SALE,
                                                    SECOM_AJIS.Common.Util.ConstantValue.LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE,
                                                    SECOM_AJIS.Common.Util.ConstantValue.InstrumentType.C_INST_TYPE_MONITOR);

                if (lst == null)
                    lst = new List<doDefaultFacility>();
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Count quotation basic (Call stored procedure: sp_QU_CountQuotationBasic)
        /// </summary>
        /// <param name="strQuotationTargetCode"></param>
        /// <returns></returns>
        private int CountQuotationBasic(string strQuotationTargetCode)
        {
            try
            {
                if (CommonUtil.IsNullOrEmpty(strQuotationTargetCode))
                {
                    throw ApplicationErrorException.ThrowErrorException(
                       MessageUtil.MODULE_COMMON,
                       MessageUtil.MessageList.MSG0007,
                                   "QuotationTargetCode");
                }

                return Convert.ToInt32(base.CountQuotationBasicSQL(strQuotationTargetCode)[0]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get quotation basic data (Call stroed procedure: sp_QU_GetQuotationBasicData)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<tbt_QuotationBasic> GetQuotationBasicData(doGetQuotationDataCondition cond)
        {
            try
            {
                /* --- Check Mandatory --- */
                /* ----------------------- */
                ApplicationErrorException.CheckMandatoryField<doGetQuotationDataCondition, doQuotationDataForQuotationBasicData>(cond);
                /* ----------------------- */

                List<tbt_QuotationBasic> lst = this.GetQuotationBasicData(cond.QuotationTargetCode,
                                                    cond.Alphabet);
                if (lst == null)
                    lst = new List<tbt_QuotationBasic>();
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get instrument detail data (Call stored procedure: sp_QU_GetInstrumentDetail)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<doInstrumentDetail> GetInstrumentDetail(doGetQuotationDataCondition cond)
        {
            try
            {
                /* --- Check Mandatory --- */
                /* ----------------------- */
                ApplicationErrorException.CheckMandatoryField<doGetQuotationDataCondition, doQuotationDataForInstrumentCondition>(cond);
                /* ----------------------- */

                List<doInstrumentDetail> lst = this.GetInstrumentDetail(MiscType.C_LINE_UP_TYPE,
                                                                        ProductType.C_PROD_TYPE_SALE,
                                                                        cond.QuotationTargetCode,
                                                                        cond.Alphabet,
                                                                        cond.ProductTypeCode);

                if (lst == null)
                    lst = new List<doInstrumentDetail>();
                else
                {
                    /* --- Update Filed by Current Language --- */
                    /* ---------------------------------------- */
                    CommonUtil.MappingObjectLanguage<doInstrumentDetail>(lst);
                    /* ---------------------------------------- */
                }

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get data from tbt_QuotationCustomer (Call stored procedure: sp_QU_GetTbt_QuotationCustomer)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<tbt_QuotationCustomer> GetTbt_QuotationCustomer(doGetQuotationDataCondition cond)
        {
            try
            {
                /* --- Check Mandatory --- */
                /* ----------------------- */
                ApplicationErrorException.CheckMandatoryField<doGetQuotationDataCondition, doQuotationDataForGetTbt_CustomerCondition>(cond);
                /* ----------------------- */

                return this.GetTbt_QuotationCustomer(cond.QuotationTargetCode);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get data from tbt_QuotationSite (Call stored procedure: sp_QU_GetTbt_QuotationSite)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<tbt_QuotationSite> GetTbt_QuotationSite(doGetQuotationDataCondition cond)
        {
            try
            {
                /* --- Check Mandatory --- */
                /* ----------------------- */
                ApplicationErrorException.CheckMandatoryField<doGetQuotationDataCondition, doQuotationDataForGetTbt_SiteCondition>(cond);
                /* ----------------------- */

                return this.GetTbt_QuotationSite(cond.QuotationTargetCode);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get data from tbt_QuotationInstrumentDetails (Call stored procedure: sp_QU_GetTbt_QuotationInstrumentDetials)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<tbt_QuotationInstrumentDetails> GetTbt_QuotationInstrumentDetails(doGetQuotationDataCondition cond)
        {
            try
            {
                /* --- Check Mandatory --- */
                /* ----------------------- */
                ApplicationErrorException.CheckMandatoryField<doGetQuotationDataCondition, doQuotationDataForGetTbt_InstDetailsCondition>(cond);
                /* ----------------------- */

                List<tbt_QuotationInstrumentDetails> lst = this.GetTbt_QuotationInstrumentDetails(cond.QuotationTargetCode,
                                                                cond.Alphabet);
                if (lst == null)
                    lst = new List<tbt_QuotationInstrumentDetails>();

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get data from tbt_QuotationOperationType (Call stored procedure: sp_QU_GetTbt_QuotationOperationType)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<tbt_QuotationOperationType> GetTbt_QuotationOperationType(doGetQuotationDataCondition cond)
        {
            try
            {
                /* --- Check Mandatory --- */
                /* ----------------------- */
                ApplicationErrorException.CheckMandatoryField<doGetQuotationDataCondition, doQuotationDataForGetTbt_OperationTypeCondition>(cond);
                /* ----------------------- */

                List<tbt_QuotationOperationType> lst = this.GetTbt_QuotationOperationType(cond.QuotationTargetCode,
                                                            cond.Alphabet);
                if (lst == null)
                    lst = new List<tbt_QuotationOperationType>();

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get data from tbt_QuotationFacilityDetails (Call stored procedure: sp_QU_GetTbt_QuotationFacilityDetails)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<tbt_QuotationFacilityDetails> GetTbt_QuotationFacilityDetails(doGetQuotationDataCondition cond)
        {
            try
            {
                /* --- Check Mandatory --- */
                /* ----------------------- */
                ApplicationErrorException.CheckMandatoryField<doGetQuotationDataCondition, doQuotationDataForGetTbt_FacilityDetailsCondition>(cond);
                /* ----------------------- */


                List<tbt_QuotationFacilityDetails> lst = this.GetTbt_QuotationFacilityDetails(cond.QuotationTargetCode,
                                                            cond.Alphabet);
                if (lst == null)
                    lst = new List<tbt_QuotationFacilityDetails>();

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get data from tbt_QuotationSentryGuardDetails (Call stored procedure: sp_QU_GetTbt_QuotationSentryGuardDetails)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<tbt_QuotationSentryGuardDetails> GetTbt_QuotationSentryGuardDetails(doGetQuotationDataCondition cond)
        {
            try
            {
                /* --- Check Mandatory --- */
                /* ----------------------- */
                ApplicationErrorException.CheckMandatoryField<doGetQuotationDataCondition, doQuotationDataForGetTbt_SentryGuardDetailsCondition>(cond);
                /* ----------------------- */

                List<tbt_QuotationSentryGuardDetails> lst = this.GetTbt_QuotationSentryGuardDetails(cond.QuotationTargetCode,
                                                                cond.Alphabet);
                if (lst == null)
                    lst = new List<tbt_QuotationSentryGuardDetails>();

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get data from tbt_QuotationBeatGuardDetails (Call stored procedure: sp_QU_GetTbt_QuotationBeatGuardDetails)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<tbt_QuotationBeatGuardDetails> GetTbt_QuotationBeatGuardDetails(doGetQuotationDataCondition cond)
        {
            try
            {
                /* --- Check Mandatory --- */
                /* ----------------------- */
                ApplicationErrorException.CheckMandatoryField<doGetQuotationDataCondition, doQuotationDataForGetTbt_BeatGuardDetailsCondition>(cond);
                /* ----------------------- */


                List<tbt_QuotationBeatGuardDetails> lst = this.GetTbt_QuotationBeatGuardDetails(cond.QuotationTargetCode,
                                                             cond.Alphabet);
                if (lst == null)
                    lst = new List<tbt_QuotationBeatGuardDetails>();

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get quotation operation type data (Call stored procedure: sp_QU_GetQuotationOperationType
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<doQuotationOperationType> GetQuotationOperationType(doGetQuotationDataCondition cond)
        {
            try
            {
                /* --- Check Mandatory Field --- */
                /* ----------------------------- */
                ApplicationErrorException.CheckMandatoryField<doGetQuotationDataCondition, doQuotationDataForOperationTypeCondition>(cond);
                /* ----------------------------- */

                List<doQuotationOperationType> lst = this.GetQuotationOperationType(
                                                                cond.QuotationTargetCode,
                                                                cond.Alphabet,
                                                                MiscType.C_OPERATION_TYPE);
                if (lst == null)
                    lst = new List<doQuotationOperationType>();
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get facility detail data (Call stored procedure: sp_QU_GetFacilityDetail)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<doFacilityDetail> GetFacilityDetail(doGetQuotationDataCondition cond)
        {
            try
            {
                /* --- Check Mandatory Field --- */
                /* ----------------------------- */
                ApplicationErrorException.CheckMandatoryField<doGetQuotationDataCondition, doQuotationDataForFacilityCondition>(cond);
                /* ----------------------------- */

                List<doFacilityDetail> lst = this.GetFacilityDetail(cond.QuotationTargetCode,
                                                    cond.Alphabet);
                if (lst == null)
                    lst = new List<doFacilityDetail>();

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get beat guard detail data (Call stored procedure: sp_QU_GetBeatGuardDetail)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public doBeatGuardDetail GetBeatGuardDetail(doGetQuotationDataCondition cond)
        {
            try
            {
                /* --- Check Mandatory Field. --- */
                /* ------------------------------ */
                ApplicationErrorException.CheckMandatoryField<doGetQuotationDataCondition, doQuotationDataForBeatGuardCondition>(cond);
                /* ------------------------------ */

                List<doBeatGuardDetail> lst = this.GetBeatGuardDetail(
                                                    cond.QuotationTargetCode,
                                                    cond.Alphabet,
                                                    SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_NUM_OF_DATE);

                doBeatGuardDetail dob = null;
                if (lst != null)
                {
                    if (lst.Count > 0)
                    {
                        dob = lst[0];
                        CommonUtil.MappingObjectLanguage(dob);
                    }
                }
                return dob;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get sentry guard detail data (Call stored procedure: sp_QU_GetSentryGuardDetail)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<doSentryGuardDetail> GetSentryGuardDetail(doGetQuotationDataCondition cond)
        {
            try
            {
                /* --- Check Mandatory Field. --- */
                /* ------------------------------ */
                ApplicationErrorException.CheckMandatoryField<doGetQuotationDataCondition, doQuotationDataForSentryGuardCondition>(cond);
                /* ------------------------------ */

                List<doSentryGuardDetail> lst = this.GetSentryGuardDetail(
                                                    cond.QuotationTargetCode,
                                                    cond.Alphabet,
                                                    SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_SG_TYPE);

                if (lst == null)
                    lst = new List<doSentryGuardDetail>();
                else
                    CommonUtil.MappingObjectLanguage<doSentryGuardDetail>(lst);

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get data from tbt_QuotationMaintenanceLinkage (Call stored procedure: sp_QU_GetTbt_QuotationMaintenanceLinkage)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<tbt_QuotationMaintenanceLinkage> GetTbt_QuotationMaintenanceLinkage(doGetQuotationDataCondition cond)
        {
            try
            {
                /* --- Check Mandatory --- */
                /* ----------------------- */
                ApplicationErrorException.CheckMandatoryField<doGetQuotationDataCondition, doQuotationDataForGetTbt_MaintLinkageCondition>(cond);
                /* ----------------------- */

                List<tbt_QuotationMaintenanceLinkage> lst = this.GetTbt_QuotationMaintenanceLinkage(
                                                                cond.QuotationTargetCode,
                                                                cond.Alphabet);
                if (lst == null)
                    lst = new List<tbt_QuotationMaintenanceLinkage>();
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public doQuotationOneShotFlag GetQuotationOneShotFlagData(doGetQuotationDataCondition cond)
        {
            try
            {
                List<doQuotationOneShotFlag> lst = this.GetQuotationOneShotFlag(
                                                                cond.QuotationTargetCode,
                                                                cond.Alphabet);
                if (lst != null)
                {
                    if (lst.Count > 0)
                        return lst[0];
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        #region Initial Methods

        /// <summary>
        /// Generate quotation target code
        /// </summary>
        /// <param name="strProductTypeCode"></param>
        /// <returns></returns>
        public string GenerateQuotationTargetCode(string strProductTypeCode)
        {
            try
            {
                if (CommonUtil.IsNullOrEmpty(strProductTypeCode))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, "strProductTypeCode");
                }
                string quotationTargetCode = null;

                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handler != null)
                {
                    /* --- Get Product Type Data --- */
                    List<tbs_ProductType> lst = handler.GetTbs_ProductType(null, strProductTypeCode);
                    ListMandatoryField lstMf = new ListMandatoryField();
                    lstMf.Index = 0;
                    lstMf.AddRows("QuotationPrefix");

                    string[][] res = CommonUtil.CheckMandatoryFiled(lst, lstMf);
                    if (res != null)
                    {
                        if (res[0].Length > 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2011);
                        }
                    }


                    /* --- Get Next running no. --- */
                    string runningNo = string.Empty;
                    List<doRunningNo> nLst = handler.GetNextRunningCode(SECOM_AJIS.Common.Util.ConstantValue.NameCode.C_NAME_CODE_QTN_TARGET_CODE, true);
                    if (nLst != null)
                    {
                        if (nLst.Count > 0)
                            runningNo = nLst[0].RunningNo;
                    }
                    #region QUP020 Mock GetNextRunningNo
                    //if (strProductTypeCode == "1")
                    //    runningNo = "000000001";
                    //else if (strProductTypeCode == "6")
                    //    runningNo = "000000208";
                    #endregion

                    /* --- Generate check digit --- */
                    string checkDigit = handler.GenerateCheckDigit(runningNo);
                    #region QUP020 Mock GenerateCheckDigit
                    //string checkDigit = string.Empty;
                    //if (strProductTypeCode == "1")
                    //    checkDigit = "7";
                    //else if (strProductTypeCode == "6")
                    //    checkDigit = "3";
                    #endregion

                    quotationTargetCode = lst[0].QuotationPrefix + runningNo + checkDigit;
                }

                return quotationTargetCode;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Generate quotation alphabet
        /// </summary>
        /// <param name="strQuotationTargetCode"></param>
        /// <returns></returns>
        public string GenerateQuotationAlphabet(string strQuotationTargetCode)
        {
            if (CommonUtil.IsNullOrEmpty(strQuotationTargetCode))
            {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, "strQuotationTargetCode");
            }
            string strAlphabet = null;


            IQuotationHandler hand = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition()
            {
                QuotationTargetCode = strQuotationTargetCode
            };
            //2.1
            List<tbt_QuotationTarget> lstQuotationTarget = hand.GetTbt_QuotationTarget(cond);
            //2.2
            if (lstQuotationTarget.Count <= 0)
            {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2003, strQuotationTargetCode);
            }

            if (CommonUtil.IsNullOrEmpty(lstQuotationTarget[0].LastAlphabet))
            {
                strAlphabet = SECOM_AJIS.Common.Util.ConstantValue.Quotation.C_START_ALPHABET;
            }
            else
            {
                string LastAlphabet = lstQuotationTarget[0].LastAlphabet.Trim();
                if (LastAlphabet.Length == 2)
                {
                    int lChar = (int)char.Parse(LastAlphabet.Substring(0, 1));
                    int rChar = (int)char.Parse(LastAlphabet.Substring(1, 1));

                    if ((lChar >= 65 && lChar <= 90) && (rChar >= 65 && rChar <= 90))
                    {
                        if (lChar == 90 && rChar == 90)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2010);
                        }
                        else
                        {
                            if (rChar >= 65 && rChar < 90)
                            {
                                rChar = rChar + 1;
                            }
                            else if (rChar == 90)
                            {
                                lChar = lChar + 1;
                                rChar = 65;
                            }
                            strAlphabet = string.Concat(Convert.ToChar(lChar), Convert.ToChar(rChar));
                        }
                    }
                    else
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2009);
                    }
                }
                else
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2008);
                }
            }
            return strAlphabet;
        }
        /// <summary>
        /// Get linkage sale contract data
        /// </summary>
        /// <param name="strSaleContractCode"></param>
        /// <returns></returns>
        public doLinkageSaleContractData GetLinkageSaleContractData(string strSaleContractCode)
        {
            try
            {
                ISaleContractHandler schandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                doSaleContractData doSaleContractData = schandler.GetSaleContractData(strSaleContractCode, null);
                if (doSaleContractData != null)
                {
                    IQuotationHandler qheader = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                    return qheader.InitLinkageSaleContractData(doSaleContractData);
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Initial linkage sale contract data
        /// </summary>
        /// <param name="doSaleContractData"></param>
        /// <returns></returns>
        public doLinkageSaleContractData InitLinkageSaleContractData(doSaleContractData doSaleContractData)
        {
            try
            {
                #region Create Linkage Sale Contract

                doLinkageSaleContractData doLinkage = CommonUtil.CloneObject
                    <tbt_SaleBasic, doLinkageSaleContractData>(doSaleContractData.dtTbt_SaleBasic);

                #endregion
                #region Create Sale Instrument Detail

                doLinkage.SaleInstrumentDetailList = new List<doInstrumentDetail>();
                if (doSaleContractData.dtTbt_SaleInstrumentDetails != null)
                {
                    #region Instrument Master

                    List<tbm_Instrument> iLst = new List<tbm_Instrument>();
                    foreach (dsSaleInstrumentDetails sInst in doSaleContractData.dtTbt_SaleInstrumentDetails)
                    {
                        iLst.Add(new tbm_Instrument()
                        {
                            InstrumentCode = sInst.InstrumentCode
                        });
                    };
                    IInstrumentMasterHandler inshandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                    List<tbm_Instrument> insLst = inshandler.GetIntrumentList(iLst);

                    #endregion

                    foreach (dsSaleInstrumentDetails sInst in doSaleContractData.dtTbt_SaleInstrumentDetails)
                    {
                        doInstrumentDetail id = new doInstrumentDetail()
                        {
                            InstrumentCode = sInst.InstrumentCode,
                            InstrumentQty = sInst.InstrumentQty
                        };
                        doLinkage.SaleInstrumentDetailList.Add(id);

                        if (insLst.Count > 0)
                        {
                            foreach (tbm_Instrument inst in insLst)
                            {
                                if (sInst.InstrumentCode.ToUpper().Trim() == inst.InstrumentCode.ToUpper().Trim())
                                {
                                    id.InstrumentName = inst.InstrumentName;
                                    id.MaintenanceFlag = inst.MaintenanceFlag;
                                    id.LineUpTypeCode = inst.LineUpTypeCode;
                                    id.ControllerFlag = inst.ControllerFlag;
                                    id.InstrumentFlag = inst.InstrumentFlag;
                                    id.SaleFlag = insLst[0].SaleFlag;
                                    id.RentalFlag = inst.RentalFlag;
                                    break;
                                }
                            }
                        }
                    }
                }

                #endregion
                #region Set Misc Name

                MiscTypeMappingList miscLst = new MiscTypeMappingList();
                miscLst.AddMiscType(doLinkage);
                if (doLinkage.SaleInstrumentDetailList.Count > 0)
                    miscLst.AddMiscType(doLinkage.SaleInstrumentDetailList.ToArray());

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                chandler.MiscTypeMappingList(miscLst);

                #endregion
                #region Set Employee Name

                EmployeeMappingList empLst = new EmployeeMappingList();
                empLst.AddEmployee(doLinkage);

                IEmployeeMasterHandler ehandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                ehandler.EmployeeListMapping(empLst);

                #endregion

                return doLinkage;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Generate Quotation
        /// </summary>
        /// <param name="GenerateData"></param>
        /// <returns></returns>
        public string GenerateQuotation(dsGenerateData GenerateData)
        {
            ValidatorUtil validator = new ValidatorUtil();
            string strAlphabet = null; //Add by Jutarat A. on 08032013
            
            try
            {
                bool ErrFlag = true;
                if (GenerateData.dtInstrumentDetails != null)
                {
                    if (GenerateData.dtInstrumentDetails.Count >= 1)
                    {
                        ErrFlag = false;
                    }
                }
                if (ErrFlag)
                    validator.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, "Instrument", new string[] { "Instrument" });

                //ApplicationErrorException.CheckMandatoryField(GenerateData.dtHeader, validator);

                for (int i = 0; i < GenerateData.dtInstrumentDetails.Count; i++)
                {
                    List<string> errLst = new List<string>();
                    if (CommonUtil.IsNullOrEmpty(GenerateData.dtInstrumentDetails[i].InstrumentCode))
                        errLst.Add("InstrumentCode");
                    if (CommonUtil.IsNullOrEmpty(GenerateData.dtInstrumentDetails[i].InstallQty))
                        errLst.Add("InstallQty");

                    if (errLst.Count > 0)
                    {
                        throw ApplicationErrorException.ThrowErrorException(
                            MessageUtil.MODULE_QUOTATION,
                            MessageUtil.MessageList.MSG2079,
                            i.ToString(),
                            CommonUtil.TextList(errLst.ToArray()));
                    }
                }

                if (GenerateData.dtFacilityDetails != null)
                {
                    for (int i = 0; i < GenerateData.dtFacilityDetails.Count; i++)
                    {
                        List<string> errLst = new List<string>();
                        if (CommonUtil.IsNullOrEmpty(GenerateData.dtFacilityDetails[i].FacilityCode))
                        {
                            errLst.Add("FacilityCode");
                        }
                        if (CommonUtil.IsNullOrEmpty(GenerateData.dtFacilityDetails[i].FacilityQty))
                        {
                            errLst.Add("FacilityQty");
                        }

                        if (errLst.Count > 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(
                                    MessageUtil.MODULE_QUOTATION,
                                    MessageUtil.MessageList.MSG2075,
                                    i.ToString(),
                                    CommonUtil.TextList(errLst.ToArray()));
                        }
                    }
                }

                IRentralContractHandler handRental = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                dsRentalContractData RentalContractData = handRental.GetEntireContract(GenerateData.dtHeader.ContractCode, null);

                if (RentalContractData == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2085, new CommonUtil().ConvertContractCode(GenerateData.dtHeader.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT));


                List<tbt_RentalContractBasic> rcBasic = RentalContractData.dtTbt_RentalContractBasic;
                List<tbt_RentalSecurityBasic> rsBasic = RentalContractData.dtTbt_RentalSecurityBasic;
                List<tbt_RentalOperationType> rOper = RentalContractData.dtTbt_RentalOperationType;
                List<tbt_RentalInstrumentDetails> rInst = RentalContractData.dtTbt_RentalInstrumentDetails;
                //2.3	Check product type of contract 
                if (rcBasic[0].ProductTypeCode != ProductType.C_PROD_TYPE_AL
                    && rcBasic[0].ProductTypeCode != ProductType.C_PROD_TYPE_RENTAL_SALE)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2014);
                }

                // 3.	Get instruments data
                List<dtIntrumenCode> lsIntrumentCode = new List<dtIntrumenCode>();
                for (int i = 0; i < GenerateData.dtInstrumentDetails.Count; i++)
                {
                    //lsIntrumentCode[i].InstrumentCode = GenerateData.dtInstrumentDetails[i].InstrumentCode;
                    dtIntrumenCode dt = new dtIntrumenCode();
                    dt.InstrumentCode = GenerateData.dtInstrumentDetails[i].InstrumentCode;
                    lsIntrumentCode.Add(dt);
                }
                IInstrumentMasterHandler hand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                List<tbm_Instrument> dtTbm_Instrument = hand.GetIntrumentList(CommonUtil.ConvertToXml_Store(lsIntrumentCode));


                // Check getInstrument return lsit is equal input list (lsIntrumentCode[])
                List<string> codeLst = new List<string>();
                for (int rIdx = 0; rIdx < lsIntrumentCode.Count; rIdx++)
                {
                    bool found = false;
                    foreach (tbm_Instrument t in dtTbm_Instrument)
                    {
                        if (lsIntrumentCode[rIdx].InstrumentCode == t.InstrumentCode)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        throw ApplicationErrorException.ThrowErrorException(
                            MessageUtil.MODULE_QUOTATION,
                            MessageUtil.MessageList.MSG2015,
                            lsIntrumentCode[rIdx].InstrumentCode);
                    }
                }

                //Comment by Jutarat A. on 31032014
                /*foreach (var item in dtTbm_Instrument)
                {
                    //item.RentalUnitPrice
                    if (CommonUtil.IsNullOrEmpty(item.RentalUnitPrice))
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2016, item.InstrumentCode);
                    }
                }*/
                //End Comment

                //4. Caculate Conttract fee

                //decimal ContractFee = 0;
                //decimal oPrice = (decimal)(rcBasic[0].LastNormalContractFee ?? 0);
                //decimal aPrice = 0;
                //decimal dPrice = 0;
                //for (int i = 0; i < GenerateData.dtInstrumentDetails.Count; i++)
                //{
                //    for (int j = 0; j < dtTbm_Instrument.Count; j++)
                //    {
                //        if (GenerateData.dtInstrumentDetails[i].InstrumentCode == dtTbm_Instrument[j].InstrumentCode)
                //        {
                //            aPrice = aPrice + (decimal)((GenerateData.dtInstrumentDetails[i].AcmAddQty ?? 0) * dtTbm_Instrument[j].RentalUnitPrice);
                //            dPrice = dPrice + (decimal)((GenerateData.dtInstrumentDetails[i].AcmRemoveQty ?? 0) * dtTbm_Instrument[j].RentalUnitPrice);
                //            break;
                //        }
                //    }
                //}

                //ContractFee = oPrice + aPrice - dPrice;

                // 5 Get quotation target data
                List<tbt_QuotationTarget> dtTbt_Quotationtarget = GetTbt_QuotationTarget(GenerateData.dtHeader.ContractCode, null, null, null);

                //6. Create quotation target data if qTarget[].rows.count<=0
                if (dtTbt_Quotationtarget.Count <= 0)
                {
                    doRegisterQuotationTargetData RegisterQuotationTargetData = new doRegisterQuotationTargetData();
                    tbt_QuotationTarget doTbt_QuotationTarget = new tbt_QuotationTarget();
                    doTbt_QuotationTarget.QuotationTargetCode = GenerateData.dtHeader.ContractCode;
                    doTbt_QuotationTarget.BranchNameEN = rcBasic[0].BranchNameEN;
                    doTbt_QuotationTarget.BranchNameLC = rcBasic[0].BranchNameLC;
                    doTbt_QuotationTarget.BranchAddressEN = rcBasic[0].BranchAddressEN;
                    doTbt_QuotationTarget.BranchAddressLC = rcBasic[0].BranchAddressLC;
                    doTbt_QuotationTarget.ProductTypeCode = rcBasic[0].ProductTypeCode;
                    doTbt_QuotationTarget.PrefixCode = rcBasic[0].PrefixCode;
                    doTbt_QuotationTarget.ServiceTypeCode = rcBasic[0].ServiceTypeCode;
                    doTbt_QuotationTarget.TargetCodeTypeCode = TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE;

                    if (rcBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                    {
                        doTbt_QuotationTarget.QuotationOfficeCode = rcBasic[0].ContractOfficeCode;
                    }
                    else
                    {
                        doTbt_QuotationTarget.QuotationOfficeCode = rcBasic[0].OperationOfficeCode;
                    }
                    doTbt_QuotationTarget.OperationOfficeCode = rcBasic[0].OperationOfficeCode;
                    doTbt_QuotationTarget.AcquisitionTypeCode = rcBasic[0].AcquisitionTypeCode;
                    doTbt_QuotationTarget.IntroducerCode = rcBasic[0].IntroducerCode;
                    doTbt_QuotationTarget.MotivationTypeCode = rcBasic[0].MotivationTypeCode;
                    doTbt_QuotationTarget.OldContractCode = rcBasic[0].OldContractCode;
                    doTbt_QuotationTarget.QuotationStaffEmpNo = rcBasic[0].QuotationStaffEmpNo;
                    doTbt_QuotationTarget.LastAlphabet = null;
                    doTbt_QuotationTarget.ContractTransferStatus = ContractTransferStatus.C_CONTRACT_TRANS_STATUS_CONTRACT_APP;
                    doTbt_QuotationTarget.ContractCode = null;
                    doTbt_QuotationTarget.TransferDate = null;
                    doTbt_QuotationTarget.TransferAlphabet = null;


                    tbt_QuotationCustomer doTtbt_QuotataionCustomer1 = new tbt_QuotationCustomer();
                    doTtbt_QuotataionCustomer1.QuotationTargetCode = GenerateData.dtHeader.ContractCode;
                    doTtbt_QuotataionCustomer1.CustPartTypeCode = CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET;
                    doTtbt_QuotataionCustomer1.CustCode = rcBasic[0].ContractTargetCustCode;

                    tbt_QuotationCustomer doTbt_QuotationCustomer2 = new tbt_QuotationCustomer();
                    doTbt_QuotationCustomer2.QuotationTargetCode = GenerateData.dtHeader.ContractCode;
                    doTbt_QuotationCustomer2.CustPartTypeCode = CustPartType.C_CUST_PART_TYPE_REAL_CUST;
                    doTbt_QuotationCustomer2.CustCode = rcBasic[0].RealCustomerCustCode;

                    tbt_QuotationSite doTbt_QuotationSite = new tbt_QuotationSite();
                    doTbt_QuotationSite.QuotationTargetCode = GenerateData.dtHeader.ContractCode;
                    doTbt_QuotationSite.SiteCode = rcBasic[0].SiteCode;
                    doTbt_QuotationSite.SiteNo = rcBasic[0].SiteCode.Substring(rcBasic[0].SiteCode.Length - 4, 4);

                    RegisterQuotationTargetData.doTbt_QuotationCustomer1 = doTtbt_QuotataionCustomer1;
                    RegisterQuotationTargetData.doTbt_QuotationCustomer2 = doTbt_QuotationCustomer2;
                    RegisterQuotationTargetData.doTbt_QuotationSite = doTbt_QuotationSite;
                    RegisterQuotationTargetData.doTbt_QuotationTarget = doTbt_QuotationTarget;

                    CreateQuotationTargetData(RegisterQuotationTargetData);

                }
                // 7
                strAlphabet = GenerateQuotationAlphabet(GenerateData.dtHeader.ContractCode);

                //8 Insert quotationdetail
                tbt_QuotationBasic doTbt_QuotationBasic = new tbt_QuotationBasic();
                doTbt_QuotationBasic.QuotationTargetCode = GenerateData.dtHeader.ContractCode;
                doTbt_QuotationBasic.Alphabet = strAlphabet;
                doTbt_QuotationBasic.OriginateProgramId = null;
                doTbt_QuotationBasic.OriginateRefNo = GenerateData.dtHeader.InstallationSlipNo;
                doTbt_QuotationBasic.ProductCode = rsBasic[0].ProductCode;
                doTbt_QuotationBasic.SecurityTypeCode = rsBasic[0].SecurityTypeCode;
                doTbt_QuotationBasic.DispatchTypeCode = rsBasic[0].DispatchTypeCode;
                doTbt_QuotationBasic.ContractDurationMonth = rsBasic[0].ContractDurationMonth;
                doTbt_QuotationBasic.AutoRenewMonth = rsBasic[0].AutoRenewMonth;
                // doTbt_QuotationBasic.LastValidDate = null;
                //doTbt_QuotationBasic.ContractTransferStatus = ContractTransferStatus.C_CONTRACT_TRANS_STATUS_CONTRACT_APP;
                doTbt_QuotationBasic.ContractTransferStatus = ContractTransferStatus.C_CONTRACT_TRANS_STATUS_QTN_REG;
                doTbt_QuotationBasic.LockStatus = LockStatus.C_LOCK_STATUS_UNLOCK;
                doTbt_QuotationBasic.LastOccNo = rcBasic[0].LastOCC;
                doTbt_QuotationBasic.CurrentSecurityTypeCode = rsBasic[0].SecurityTypeCode;
                doTbt_QuotationBasic.PhoneLineTypeCode1 = rsBasic[0].PhoneLineTypeCode1;
                doTbt_QuotationBasic.PhoneLineTypeCode2 = rsBasic[0].PhoneLineTypeCode2;
                doTbt_QuotationBasic.PhoneLineTypeCode3 = rsBasic[0].PhoneLineTypeCode3;
                doTbt_QuotationBasic.PhoneLineOwnerTypeCode1 = rsBasic[0].PhoneLineOwnerTypeCode1;
                doTbt_QuotationBasic.PhoneLineOwnerTypeCode2 = rsBasic[0].PhoneLineOwnerTypeCode2;
                doTbt_QuotationBasic.PhoneLineOwnerTypeCode3 = rsBasic[0].PhoneLineOwnerTypeCode3; //ตกตัว r
                doTbt_QuotationBasic.FireMonitorFlag = rsBasic[0].FireMonitorFlag;
                doTbt_QuotationBasic.CrimePreventFlag = rsBasic[0].CrimePreventFlag;
                doTbt_QuotationBasic.EmergencyReportFlag = rsBasic[0].EmergencyReportFlag;
                doTbt_QuotationBasic.FacilityMonitorFlag = rsBasic[0].FacilityMonitorFlag;
                doTbt_QuotationBasic.BeatGuardFlag = FlagType.C_FLAG_OFF;
                doTbt_QuotationBasic.SentryGuardFlag = FlagType.C_FLAG_OFF;
                doTbt_QuotationBasic.MaintenanceFlag = FlagType.C_FLAG_OFF;
                doTbt_QuotationBasic.SaleOnlineContractCode = null;
                doTbt_QuotationBasic.PlanCode = rsBasic[0].PlanCode;
                doTbt_QuotationBasic.SpecialInstallationFlag = rsBasic[0].SpecialInstallationFlag;
                doTbt_QuotationBasic.PlannerEmpNo = GenerateData.dtHeader.InstallationEngineerEmpNo;
                doTbt_QuotationBasic.PlanCheckerEmpNo = GenerateData.dtHeader.InstallationEngineerEmpNo;

                doTbt_QuotationBasic.PlanCheckDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doTbt_QuotationBasic.PlanApproverEmpNo = GenerateData.dtHeader.InstallationEngineerEmpNo;
                doTbt_QuotationBasic.PlanApproveDate = null;
                doTbt_QuotationBasic.SiteBuildingArea = rsBasic[0].SiteBuildingArea;
                doTbt_QuotationBasic.SecurityAreaFrom = rsBasic[0].SecurityAreaFrom;
                doTbt_QuotationBasic.SecurityAreaTo = rsBasic[0].SecurityAreaTo;
                doTbt_QuotationBasic.MainStructureTypeCode = rsBasic[0].MainStructureTypeCode;
                doTbt_QuotationBasic.BuildingTypeCode = rsBasic[0].BuildingTypeCode;

                doTbt_QuotationBasic.NewBldMgmtFlag = rsBasic[0].NewBldMgmtFlag;
                doTbt_QuotationBasic.NewBldMgmtCost = rsBasic[0].NewBldMgmtCost;
                doTbt_QuotationBasic.NewBldMgmtCostCurrencyType = rsBasic[0].NewBldMgmtCostCurrencyType;

                doTbt_QuotationBasic.NumOfBuilding = rsBasic[0].NumOfBuilding;
                doTbt_QuotationBasic.NumOfFloor = rsBasic[0].NumOfFloor;
                doTbt_QuotationBasic.FacilityPassYear = rsBasic[0].FacilityPassYear;
                doTbt_QuotationBasic.FacilityPassMonth = rsBasic[0].FacilityPassMonth;
                doTbt_QuotationBasic.SalesmanEmpNo1 = GenerateData.dtHeader.InstallationEngineerEmpNo;
                doTbt_QuotationBasic.SalesmanEmpNo2 = rsBasic[0].SalesmanEmpNo2;
                doTbt_QuotationBasic.SalesmanEmpNo3 = null;
                doTbt_QuotationBasic.SalesmanEmpNo4 = null;
                doTbt_QuotationBasic.SalesmanEmpNo5 = null;
                doTbt_QuotationBasic.SalesmanEmpNo6 = null;
                doTbt_QuotationBasic.SalesmanEmpNo7 = null;
                doTbt_QuotationBasic.SalesmanEmpNo8 = null;
                doTbt_QuotationBasic.SalesmanEmpNo9 = null;
                doTbt_QuotationBasic.SalesmanEmpNo10 = null;
                doTbt_QuotationBasic.SalesSupporterEmpNo = rsBasic[0].SalesSupporterEmpNo;
                doTbt_QuotationBasic.InsuranceTypeCode = rsBasic[0].InsuranceTypeCode;
                doTbt_QuotationBasic.InsuranceCoverageAmount = rsBasic[0].InsuranceCoverageAmount;
                doTbt_QuotationBasic.MonthlyInsuranceFee = rsBasic[0].MonthlyInsuranceFee;
                doTbt_QuotationBasic.MaintenanceFee1 = rsBasic[0].MaintenanceFee1;
                doTbt_QuotationBasic.MaintenanceFee2 = rsBasic[0].MaintenanceFee2;
                doTbt_QuotationBasic.BidGuaranteeAmount1 = null;
                doTbt_QuotationBasic.BidGuaranteeAmount2 = null;
                doTbt_QuotationBasic.AdditionalFee1 = null;
                doTbt_QuotationBasic.AdditionalFee2 = null;
                doTbt_QuotationBasic.AdditionalFee3 = null;
                doTbt_QuotationBasic.AdditionalApproveNo1 = null;
                doTbt_QuotationBasic.AdditionalApproveNo2 = null;
                doTbt_QuotationBasic.AdditionalApproveNo3 = null;
                doTbt_QuotationBasic.ApproveNo1 = GenerateData.dtHeader.ApproveNo1;
                doTbt_QuotationBasic.ApproveNo2 = GenerateData.dtHeader.ApproveNo2;
                doTbt_QuotationBasic.ApproveNo3 = null;
                doTbt_QuotationBasic.ApproveNo4 = null;
                doTbt_QuotationBasic.ApproveNo5 = null;
                doTbt_QuotationBasic.ContractFee = GenerateData.dtHeader.NormalContractFee;
                doTbt_QuotationBasic.ContractFeeUsd = GenerateData.dtHeader.NormalContractFeeUsd;
                doTbt_QuotationBasic.ContractFeeCurrencyType = GenerateData.dtHeader.NormalContractFeeCurrencyType;
                doTbt_QuotationBasic.ProductPrice = null;
                doTbt_QuotationBasic.InstallationFee = GenerateData.dtHeader.InstallationFee;
                doTbt_QuotationBasic.InstallationFeeUsd = GenerateData.dtHeader.InstallationFeeUsd;
                doTbt_QuotationBasic.InstallationFeeCurrencyType = GenerateData.dtHeader.InstallationFeeCurrencyType;
                doTbt_QuotationBasic.DepositFee = null;
                doTbt_QuotationBasic.FacilityMemo = rsBasic[0].FacilityMemo;
                doTbt_QuotationBasic.MaintenanceMemo = null;
                doTbt_QuotationBasic.SecurityItemFee = null;
                doTbt_QuotationBasic.OtherItemFee = null;
                doTbt_QuotationBasic.SentryGuardAreaTypeCode = null;
                doTbt_QuotationBasic.SentryGuardFee = null;
                doTbt_QuotationBasic.TotalSentryGuardFee = null;
                doTbt_QuotationBasic.MaintenanceTargetProductTypeCode = null;
                doTbt_QuotationBasic.MaintenanceTypeCode = null;
                doTbt_QuotationBasic.MaintenanceCycle = rsBasic[0].MaintenanceCycle;

                //8.13 Insert quotation basic
                int rowInsertedTbt_QuotationBasic = InsertQuotationBasic(doTbt_QuotationBasic);

                //8.2	Insert quotation operation type if rOper[].Rows.Count > 0
                for (int i = 0; i < rOper.Count; i++)
                {
                    tbt_QuotationOperationType doTbt_QuotationOperationType = new tbt_QuotationOperationType();
                    doTbt_QuotationOperationType.QuotationTargetCode = GenerateData.dtHeader.ContractCode;
                    doTbt_QuotationOperationType.Alphabet = strAlphabet;
                    doTbt_QuotationOperationType.OperationTypeCode = rOper[i].OperationTypeCode;
                    int rowInserted_Tbt_QuotationOperationType = InsertQuotationOperationType(doTbt_QuotationOperationType);
                }

                for (int i = 0; i < GenerateData.dtInstrumentDetails.Count; i++)
                {
                    tbt_QuotationInstrumentDetails doTbt_QuotationInstrumentDetails = new tbt_QuotationInstrumentDetails();
                    doTbt_QuotationInstrumentDetails.QuotationTargetCode = GenerateData.dtHeader.ContractCode;
                    doTbt_QuotationInstrumentDetails.Alphabet = strAlphabet;
                    doTbt_QuotationInstrumentDetails.InstrumentCode = GenerateData.dtInstrumentDetails[i].InstrumentCode;
                    doTbt_QuotationInstrumentDetails.InstrumentQty = GenerateData.dtInstrumentDetails[i].InstallQty;
                    doTbt_QuotationInstrumentDetails.AddQty = GenerateData.dtInstrumentDetails[i].AcmAddQty;
                    doTbt_QuotationInstrumentDetails.RemoveQty = GenerateData.dtInstrumentDetails[i].AcmRemoveQty;
                    int rowInsertedTbt_QuotationInstrumentDetails = InsertQuotationInstrumentDetails(doTbt_QuotationInstrumentDetails);

                }

                //8.4	Insert quotation facility
                //int num = 0;
                if (GenerateData.dtFacilityDetails != null)
                {
                    foreach (dtFacilityDetails facility in GenerateData.dtFacilityDetails)
                    {
                        tbt_QuotationFacilityDetails doTbt_QuotationFacilityDetails = new tbt_QuotationFacilityDetails()
                        {
                            QuotationTargetCode = GenerateData.dtHeader.ContractCode,
                            Alphabet = strAlphabet,
                            FacilityCode = facility.FacilityCode,
                            FacilityQty = facility.FacilityQty
                        };
                        int rowdtInsertedTbt_QuotationFacilityDetails = InsertQuotationFacilityDetails(doTbt_QuotationFacilityDetails);
                    }
                }


                //9.	Update last alphabet in quotation target

                doUpdateQuotationTargetData UpdateQuotationTargetData = new doUpdateQuotationTargetData();
                UpdateQuotationTargetData.QuotationTargetCode = GenerateData.dtHeader.ContractCode;
                UpdateQuotationTargetData.LastAlphabet = strAlphabet;

                int rowdtUpdatedTbt_QuotationTarget = UpdateQuotationTarget(UpdateQuotationTargetData);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strAlphabet;
        }

        #endregion
        #region Insert Methods

        /// <summary>
        /// Create quotation target data
        /// </summary>
        /// <param name="doRegQuoTarData"></param>
        /// <returns></returns>
        public int CreateQuotationTargetData(doRegisterQuotationTargetData doRegQuoTarData)
        {
            ILogHandler handlog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            //1.	Insert quotation target 
            int rowdtInsertedTbt_QuotationTarget = InsertQuotationTarget(doRegQuoTarData.doTbt_QuotationTarget);

            //2.	Insert contract target customer
            int rowdtInsertedTbt_QuotationCustomer = InsertQuotationCustomer(doRegQuoTarData.doTbt_QuotationCustomer1);


            //3.Insert real customer if doTbt_QuotationCustomer2 is not null
            if (doRegQuoTarData.doTbt_QuotationCustomer2 != null)
                rowdtInsertedTbt_QuotationCustomer = InsertQuotationCustomer(doRegQuoTarData.doTbt_QuotationCustomer2);


            //4.	Insert quotation site
            int RowdtInsertedTbt_QuotationSite = InsertQuotationSite(doRegQuoTarData.doTbt_QuotationSite);


            return 1;
        }
        /// <summary>
        /// Insert quotation beat guard detail data (Call stored procedure: sp_QU_InsertQuotationBeatGuardDetails)
        /// </summary>
        /// <param name="tbt_QuotationBeatGuardDetails"></param>
        /// <returns></returns>
        public int InsertQuotationBeatGuardDetails(tbt_QuotationBeatGuardDetails tbt_QuotationBeatGuardDetails)
        {
            try
            {
                /* --- Check Mandatory --- */
                /* ----------------------- */
                ApplicationErrorException.CheckMandatoryField(tbt_QuotationBeatGuardDetails);
                /* ----------------------- */

                #region Insert Data

                tbt_QuotationBeatGuardDetails.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                tbt_QuotationBeatGuardDetails.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                tbt_QuotationBeatGuardDetails.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                tbt_QuotationBeatGuardDetails.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_QuotationBeatGuardDetails> lst = new List<tbt_QuotationBeatGuardDetails>()
                {
                    tbt_QuotationBeatGuardDetails
                };
                List<tbt_QuotationBeatGuardDetails> res = base.InsertQuotationBeatGuardDetails(
                                                                CommonUtil.ConvertToXml_Store<tbt_QuotationBeatGuardDetails>(lst)
                                                            );

                #endregion
                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_QTN_BE,
                    TableData = CommonUtil.ConvertToXml(res)
                };
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return res.Count;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Insert quotation operation type data (Call stored procedure: sp_QU_InsertQuotationOperationType)
        /// </summary>
        /// <param name="doTbt_QuotationOperationType"></param>
        /// <returns></returns>
        public int InsertQuotationOperationType(tbt_QuotationOperationType doTbt_QuotationOperationType)
        {
            try
            {
                ApplicationErrorException.CheckMandatoryField(doTbt_QuotationOperationType);
                List<tbt_QuotationOperationType> dtInsertedTbt_QuotationOperationType = base.InsertQuotationOperationType(doTbt_QuotationOperationType.QuotationTargetCode, doTbt_QuotationOperationType.Alphabet, doTbt_QuotationOperationType.OperationTypeCode, CommonUtil.dsTransData.dtOperationData.ProcessDateTime, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime, CommonUtil.dsTransData.dtUserData.EmpNo);
                if (dtInsertedTbt_QuotationOperationType.Count > 0)
                {
                    ILogHandler hand2 = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog InsertOperationTypeLog = new doTransactionLog();
                    InsertOperationTypeLog.TransactionType = doTransactionLog.eTransactionType.Insert;
                    InsertOperationTypeLog.TableName = TableName.C_TBL_NAME_QTN_OPER_TYPE;
                    InsertOperationTypeLog.TableData = CommonUtil.ConvertToXml(dtInsertedTbt_QuotationOperationType);
                    hand2.WriteTransactionLog(InsertOperationTypeLog);
                    return dtInsertedTbt_QuotationOperationType.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// Insert quotation instrument details data (Call stored procedure: sp_QU_InsertQuotationInstrumentDetails)
        /// </summary>
        /// <param name="doTbt_QuotationInstrumentDetails"></param>
        /// <returns></returns>
        public int InsertQuotationInstrumentDetails(tbt_QuotationInstrumentDetails doTbt_QuotationInstrumentDetails)
        {
            ApplicationErrorException.CheckMandatoryField<tbt_QuotationInstrumentDetails, InsertQuotationInstrumentDetailsValidator>(doTbt_QuotationInstrumentDetails);
            try
            {
                List<tbt_QuotationInstrumentDetails> dtInsertedTbt_QuotationInstrumentDetails = base.InsertQuotationInstrumentDetails(doTbt_QuotationInstrumentDetails.QuotationTargetCode, doTbt_QuotationInstrumentDetails.Alphabet, doTbt_QuotationInstrumentDetails.InstrumentCode, doTbt_QuotationInstrumentDetails.InstrumentQty, doTbt_QuotationInstrumentDetails.AddQty, doTbt_QuotationInstrumentDetails.RemoveQty, CommonUtil.dsTransData.dtOperationData.ProcessDateTime, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime, CommonUtil.dsTransData.dtUserData.EmpNo);
                if (dtInsertedTbt_QuotationInstrumentDetails.Count > 0)
                {
                    ILogHandler hand2 = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog InsertIntrumentDetailLog = new doTransactionLog();
                    InsertIntrumentDetailLog.TransactionType = doTransactionLog.eTransactionType.Insert;
                    InsertIntrumentDetailLog.TableName = TableName.C_TBL_NAME_QTN_INST;
                    InsertIntrumentDetailLog.TableData = CommonUtil.ConvertToXml(dtInsertedTbt_QuotationInstrumentDetails);
                    hand2.WriteTransactionLog(InsertIntrumentDetailLog);
                    return dtInsertedTbt_QuotationInstrumentDetails.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Insert quotation facility details data (Call stored procedure: sp_QU_InsertQuotationFacilityDetails)
        /// </summary>
        /// <param name="doTbt_QuotationFacilityDetails"></param>
        /// <returns></returns>
        public int InsertQuotationFacilityDetails(tbt_QuotationFacilityDetails doTbt_QuotationFacilityDetails)
        {
            ApplicationErrorException.CheckMandatoryField(doTbt_QuotationFacilityDetails);
            try
            {
                List<tbt_QuotationFacilityDetails> dtInsertedTbt_QuotationFacilityDetails = base.InsertQuotationFacilityDetails(doTbt_QuotationFacilityDetails.QuotationTargetCode, doTbt_QuotationFacilityDetails.Alphabet, doTbt_QuotationFacilityDetails.FacilityCode, doTbt_QuotationFacilityDetails.FacilityQty, CommonUtil.dsTransData.dtOperationData.ProcessDateTime, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime, CommonUtil.dsTransData.dtUserData.EmpNo);
                if (dtInsertedTbt_QuotationFacilityDetails.Count > 0)
                {
                    ILogHandler handlog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog TransLogFacilityDetail = new doTransactionLog();
                    TransLogFacilityDetail.TransactionType = doTransactionLog.eTransactionType.Insert;
                    TransLogFacilityDetail.TableName = TableName.C_TBL_NAME_QTN_FAC;
                    TransLogFacilityDetail.TableData = CommonUtil.ConvertToXml(dtInsertedTbt_QuotationFacilityDetails);
                    handlog.WriteTransactionLog(TransLogFacilityDetail);
                    return dtInsertedTbt_QuotationFacilityDetails.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// Insert quotation basic data (Call stored procedure: sp_QU_InsertQuotationBasic)
        /// </summary>
        /// <param name="doTbt_QuotationBasic"></param>
        /// <returns></returns>
        public int InsertQuotationBasic(tbt_QuotationBasic doTbt_QuotationBasic)
        {
            doTbt_QuotationBasic.OriginateProgramId = CommonUtil.dsTransData.dtTransHeader.ScreenID;
            doTbt_QuotationBasic.LastValidDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime.AddMonths(3);
            doTbt_QuotationBasic.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            doTbt_QuotationBasic.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            doTbt_QuotationBasic.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            doTbt_QuotationBasic.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

            ApplicationErrorException.CheckMandatoryField<tbt_QuotationBasic, InsertQuotationBasicCondition>(doTbt_QuotationBasic);

            List<tbt_QuotationBasic> lst_doTbt_QuotationBasic = new List<tbt_QuotationBasic>();
            lst_doTbt_QuotationBasic.Add(doTbt_QuotationBasic);
            try
            {
                List<tbt_QuotationBasic> dtInsertedTbt_QuotationBasic = base.InsertQuotationBasic(CommonUtil.ConvertToXml_Store<tbt_QuotationBasic>(lst_doTbt_QuotationBasic));
                if (dtInsertedTbt_QuotationBasic.Count > 0)
                {
                    ILogHandler handlog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog logUpdateQuotationBasic = new doTransactionLog();
                    logUpdateQuotationBasic.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logUpdateQuotationBasic.TableName = TableName.C_TBL_NAME_QTN_BASIC;
                    logUpdateQuotationBasic.TableData = CommonUtil.ConvertToXml(dtInsertedTbt_QuotationBasic);
                    handlog.WriteTransactionLog(logUpdateQuotationBasic);

                    return dtInsertedTbt_QuotationBasic.Count;
                }
                else { return -1; }
            }
            catch (Exception ex)
            {
                throw ex;
            }



        }
        /// <summary>
        /// Insert quotation target data (Call stored procedure: sp_QU_InsertQuotationTarget)
        /// </summary>
        /// <param name="doTbt_QuotationTarget"></param>
        /// <returns>List<tbt_QuotationTarget> </returns>
        public int InsertQuotationTarget(tbt_QuotationTarget doTbt_QuotationTarget)
        {
            ApplicationErrorException.CheckMandatoryField(doTbt_QuotationTarget);
            doTbt_QuotationTarget.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            doTbt_QuotationTarget.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            doTbt_QuotationTarget.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            doTbt_QuotationTarget.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

            List<tbt_QuotationTarget> lst_tbt_QuotationTarget = new List<tbt_QuotationTarget>();
            lst_tbt_QuotationTarget.Add(doTbt_QuotationTarget);

            List<tbt_QuotationTarget> dtInsertedTbt_QuotationTarget = base.InsertQuotationTarget(CommonUtil.ConvertToXml_Store(lst_tbt_QuotationTarget));
            if (dtInsertedTbt_QuotationTarget.Count > 0)
            {
                ILogHandler handlog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog logInsertQuotationTarget = new doTransactionLog();
                logInsertQuotationTarget.TransactionType = doTransactionLog.eTransactionType.Insert;
                logInsertQuotationTarget.TableName = TableName.C_TBL_NAME_QTN_TARGET;
                logInsertQuotationTarget.TableData = CommonUtil.ConvertToXml(dtInsertedTbt_QuotationTarget);
                handlog.WriteTransactionLog(logInsertQuotationTarget);
                return dtInsertedTbt_QuotationTarget.Count;
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// Insert quotation customer data (Call stored procedure: sp_QU_InsertQuotationCustomer)
        /// </summary>
        /// <param name="doTbt_QuotationCustomer"></param>
        /// <returns></returns>
        public int InsertQuotationCustomer(tbt_QuotationCustomer doTbt_QuotationCustomer)
        {
            try
            {
                ApplicationErrorException.CheckMandatoryField(doTbt_QuotationCustomer);

                doTbt_QuotationCustomer.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doTbt_QuotationCustomer.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doTbt_QuotationCustomer.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doTbt_QuotationCustomer.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_QuotationCustomer> lst_doTbt_QuotationCustomer = new List<tbt_QuotationCustomer>();
                lst_doTbt_QuotationCustomer.Add(doTbt_QuotationCustomer);

                List<tbt_QuotationCustomer> dtInsertedTbt_QuotationCustomer = base.InsertQuotationCustomer(CommonUtil.ConvertToXml_Store<tbt_QuotationCustomer>(lst_doTbt_QuotationCustomer));
                if (dtInsertedTbt_QuotationCustomer.Count > 0)
                {
                    ILogHandler handlog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog logInsertQuotationCustomer = new doTransactionLog();
                    logInsertQuotationCustomer.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logInsertQuotationCustomer.TableName = TableName.C_TBL_NAME_QTN_CUST;
                    logInsertQuotationCustomer.TableData = CommonUtil.ConvertToXml(dtInsertedTbt_QuotationCustomer);
                    handlog.WriteTransactionLog(logInsertQuotationCustomer);
                    return dtInsertedTbt_QuotationCustomer.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        /// <summary>
        /// Insert quotation site data (Call stored procedure: sp_QU_InsertQuotationSite)
        /// </summary>
        /// <param name="doRegQuoTarData"></param>
        /// <returns></returns>
        public int InsertQuotationSite(tbt_QuotationSite doTbt_QuotationSite)
        {
            try
            {
                ApplicationErrorException.CheckMandatoryField(doTbt_QuotationSite);

                doTbt_QuotationSite.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doTbt_QuotationSite.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doTbt_QuotationSite.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doTbt_QuotationSite.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_QuotationSite> lst_doTbt_QuotationSite = new List<tbt_QuotationSite>();
                lst_doTbt_QuotationSite.Add(doTbt_QuotationSite);

                List<tbt_QuotationSite> dtInsertedTbt_QuotationSite = base.InsertQuotationSite(CommonUtil.ConvertToXml_Store<tbt_QuotationSite>(lst_doTbt_QuotationSite));
                if (dtInsertedTbt_QuotationSite.Count > 0)
                {
                    ILogHandler handlog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog logInsertQuotationLog = new doTransactionLog();
                    logInsertQuotationLog.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logInsertQuotationLog.TableName = TableName.C_TBL_NAME_QTN_SITE;
                    logInsertQuotationLog.TableData = CommonUtil.ConvertToXml(dtInsertedTbt_QuotationSite);
                    handlog.WriteTransactionLog(logInsertQuotationLog);
                    return dtInsertedTbt_QuotationSite.Count;
                }
                else { return -1; }
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Insert quotation sentry guard details data (Call stored procedure: sp_QU_InsertQuotationSentryGuardDetails)
        /// </summary>
        /// <param name="tbt_QuotationSentryGuardDetails"></param>
        /// <returns></returns>
        public int InsertQuotationSentryGuardDetails(tbt_QuotationSentryGuardDetails tbt_QuotationSentryGuardDetails)
        {
            try
            {
                /* --- Check Mandatory --- */
                /* ----------------------- */
                ApplicationErrorException.CheckMandatoryField(tbt_QuotationSentryGuardDetails);
                /* ----------------------- */

                #region Insert Data

                tbt_QuotationSentryGuardDetails.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                tbt_QuotationSentryGuardDetails.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                tbt_QuotationSentryGuardDetails.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                tbt_QuotationSentryGuardDetails.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_QuotationSentryGuardDetails> res = base.InsertQuotationSentryGuardDetails(
                    tbt_QuotationSentryGuardDetails.QuotationTargetCode,
                    tbt_QuotationSentryGuardDetails.Alphabet,
                    tbt_QuotationSentryGuardDetails.RunningNo,
                    tbt_QuotationSentryGuardDetails.SentryGuardTypeCode,
                    tbt_QuotationSentryGuardDetails.NumOfDate,
                    tbt_QuotationSentryGuardDetails.SecurityStartTime,
                    tbt_QuotationSentryGuardDetails.SecurityFinishTime,
                    tbt_QuotationSentryGuardDetails.WorkHourPerMonth,
                    tbt_QuotationSentryGuardDetails.CostPerHour,
                    tbt_QuotationSentryGuardDetails.NumOfSentryGuard,
                    tbt_QuotationSentryGuardDetails.CreateDate,
                    tbt_QuotationSentryGuardDetails.CreateBy,
                    tbt_QuotationSentryGuardDetails.UpdateDate,
                    tbt_QuotationSentryGuardDetails.UpdateBy,
                    tbt_QuotationSentryGuardDetails.CostPerHourUsd,
                    tbt_QuotationSentryGuardDetails.CostPerHourCurrencyType);

                #endregion
                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_QTN_SG,
                    TableData = CommonUtil.ConvertToXml(res)
                };
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return res.Count;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Insert quotation maintenance linkage data (Call stored procedure: sp_QU_InsertQuotationMaintenanceLinkage)
        /// </summary>
        /// <param name="tbt_QuotationMaintenanceLinkage"></param>
        /// <returns></returns>
        public int InsertQuotationMaintenanceLinkage(tbt_QuotationMaintenanceLinkage tbt_QuotationMaintenanceLinkage)
        {
            try
            {
                /* --- Check Mandatory --- */
                /* ----------------------- */
                ApplicationErrorException.CheckMandatoryField(tbt_QuotationMaintenanceLinkage);
                /* ----------------------- */

                #region Insert Data

                tbt_QuotationMaintenanceLinkage.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                tbt_QuotationMaintenanceLinkage.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                tbt_QuotationMaintenanceLinkage.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                tbt_QuotationMaintenanceLinkage.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_QuotationMaintenanceLinkage> res = base.InsertQuotationMaintenanceLinkage(
                    tbt_QuotationMaintenanceLinkage.QuotationTargetCode,
                    tbt_QuotationMaintenanceLinkage.Alphabet,
                    tbt_QuotationMaintenanceLinkage.ContractCode,
                    tbt_QuotationMaintenanceLinkage.CreateDate,
                    tbt_QuotationMaintenanceLinkage.CreateBy,
                    tbt_QuotationMaintenanceLinkage.UpdateDate,
                    tbt_QuotationMaintenanceLinkage.UpdateBy);

                #endregion
                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_QTN_MA,
                    TableData = CommonUtil.ConvertToXml(res)
                };
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return res.Count;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Insert quotation installation detail (Call stored procedure: sp_QU_InsertTbt_QuotationInstallationDetail)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int InsertQuotationInstallationDetail(List<tbt_QuotationInstallationDetail> datalist)
        {
            try
            {
                #region Insert Data

                var res = base.InsertTbt_QuotationInstallationDetail(
                    CommonUtil.ConvertToXml_Store<tbt_QuotationInstallationDetail>(datalist)
                );

                #endregion
                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_QTN_INSTALL_DTL,
                    TableData = CommonUtil.ConvertToXml(res)
                };
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return res.Count;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        #region Update Methods

        /// <summary>
        /// Lock quotation
        /// </summary>
        /// <param name="strQuotationTargetCode"></param>
        /// <param name="strAlphabet"></param>
        /// <param name="strLockStyleCode"></param>
        /// <returns></returns>
        public bool LockQuotation(string strQuotationTargetCode, string strAlphabet, string strLockStyleCode)
        {

            try
            {
                if (CommonUtil.IsNullOrEmpty(strLockStyleCode))
                {
                    throw ApplicationErrorException.ThrowErrorException(
                       MessageUtil.MODULE_COMMON,
                       MessageUtil.MessageList.MSG0007,
                                   "strLockStyleCode");
                }
                //2.1
                int intRowCount = CountQuotationBasic(strQuotationTargetCode);
                //2.2
                if (intRowCount <= 0)
                {
                    //throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_QUOTATION,
                    //                                                   MessageUtil.MessageList.MSG2012,
                    //                                                   strQuotationTargetCode);
                    return true;
                }

                List<tbt_QuotationBasic> dtUpdatedTbt_QuotationBasic = null;

                //3.1
                if (strLockStyleCode == LockStyle.C_LOCK_STYLE_ALL)
                {
                    //dtUpdatedTbt_QuotationBasic = LockAll(
                    //    LockStatus.C_LOCK_STATUS_LOCK,
                    //    LockStatus.C_LOCK_STATUS_UNLOCK,
                    //       CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    //       CommonUtil.dsTransData.dtUserData.EmpNo,
                    //    strQuotationTargetCode);

                    dtUpdatedTbt_QuotationBasic = this.LockAll(strQuotationTargetCode);
                }
                else if (strLockStyleCode == LockStyle.C_LOCK_STYLE_BACKWARD)
                {
                    //dtUpdatedTbt_QuotationBasic = LockBackward(
                    // LockStatus.C_LOCK_STATUS_LOCK,
                    // LockStatus.C_LOCK_STATUS_UNLOCK,
                    //     CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    //    CommonUtil.dsTransData.dtUserData.EmpNo,
                    // strQuotationTargetCode,
                    // strAlphabet);

                    dtUpdatedTbt_QuotationBasic = this.LockBackward(strQuotationTargetCode, strAlphabet);
                }
                else if (strLockStyleCode == LockStyle.C_LOCK_STYLE_INDIVIDUAL)
                {
                    //dtUpdatedTbt_QuotationBasic = LockIndividual(
                    //    strQuotationTargetCode,
                    //    strAlphabet,
                    //    LockStatus.C_LOCK_STATUS_LOCK,
                    //    LockStatus.C_LOCK_STATUS_UNLOCK,
                    //    CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    //       CommonUtil.dsTransData.dtUserData.EmpNo);

                    dtUpdatedTbt_QuotationBasic = this.LockIndividual(strQuotationTargetCode, strAlphabet);
                }

                if (dtUpdatedTbt_QuotationBasic != null)
                {
                    if (dtUpdatedTbt_QuotationBasic.Count > 0)
                    {
                        ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        doTransactionLog logUpdateQuotationBasic = new doTransactionLog();
                        logUpdateQuotationBasic.TransactionType = doTransactionLog.eTransactionType.Update;
                        logUpdateQuotationBasic.TableName = TableName.C_TBL_NAME_QTN_BASIC;
                        logUpdateQuotationBasic.TableData = CommonUtil.ConvertToXml(dtUpdatedTbt_QuotationBasic);
                        hand.WriteTransactionLog(logUpdateQuotationBasic);
                        return true;
                    }

                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Lock all quotation (Call stored procedure: sp_QU_LockAll)
        /// </summary>
        /// <param name="strQuotationTargetCode"></param>
        /// <returns></returns>
        public List<tbt_QuotationBasic> LockAll(string strQuotationTargetCode)
        {
            try
            {
                if (CommonUtil.IsNullOrEmpty(strQuotationTargetCode))
                {
                    throw ApplicationErrorException.ThrowErrorException(
                       MessageUtil.MODULE_COMMON,
                       MessageUtil.MessageList.MSG0007,
                                   "strQuotationTargetCode");
                }

                List<tbt_QuotationBasic> dtUpdatedTbt_QuotationBasic
                    = base.LockAll(LockStatus.C_LOCK_STATUS_LOCK
                            , LockStatus.C_LOCK_STATUS_UNLOCK
                            , CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                            , CommonUtil.dsTransData.dtUserData.EmpNo
                            , strQuotationTargetCode);

                return dtUpdatedTbt_QuotationBasic;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Lock backward quotation (Call stored procedure: sp_QU_LockBackward)
        /// </summary>
        /// <param name="strQuotationTargetCode"></param>
        /// <param name="strAlphabet"></param>
        /// <returns></returns>
        public List<tbt_QuotationBasic> LockBackward(string strQuotationTargetCode, string strAlphabet)
        {
            try
            {
                //Validate data input
                doLockCondition cond = new doLockCondition();
                cond.QuotationTargetCode = strQuotationTargetCode;
                cond.Alphabet = strAlphabet;
                ApplicationErrorException.CheckMandatoryField(cond);

                List<tbt_QuotationBasic> dtUpdatedTbt_QuotationBasic
                    = base.LockBackward(LockStatus.C_LOCK_STATUS_LOCK
                                    , LockStatus.C_LOCK_STATUS_UNLOCK
                                    , CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                                    , CommonUtil.dsTransData.dtUserData.EmpNo
                                    , strQuotationTargetCode
                                    , strAlphabet);

                return dtUpdatedTbt_QuotationBasic;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Lock individual quotation (Call stored procedure: sp_QU_LockIndividual)
        /// </summary>
        /// <param name="strQuotationTargetCode"></param>
        /// <param name="strAlphabet"></param>
        /// <returns></returns>
        public List<tbt_QuotationBasic> LockIndividual(string strQuotationTargetCode, string strAlphabet)
        {
            try
            {
                //Validate data input
                doLockCondition cond = new doLockCondition();
                cond.QuotationTargetCode = strQuotationTargetCode;
                cond.Alphabet = strAlphabet;
                ApplicationErrorException.CheckMandatoryField(cond);

                List<tbt_QuotationBasic> dtUpdatedTbt_QuotationBasic
                    = base.LockIndividual(strQuotationTargetCode
                                    , strAlphabet
                                    , LockStatus.C_LOCK_STATUS_LOCK
                                    , LockStatus.C_LOCK_STATUS_UNLOCK
                                    , CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                                    , CommonUtil.dsTransData.dtUserData.EmpNo);

                return dtUpdatedTbt_QuotationBasic;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Update quotation target data (Call stored procedure: sp_QU_UpdateQuotationTarget)
        /// </summary>
        /// <param name="UpdateQuotationTargetData"></param>
        /// <returns></returns>
        public int UpdateQuotationTarget(doUpdateQuotationTargetData UpdateQuotationTargetData)
        {

            try
            {
                ApplicationErrorException.CheckMandatoryField(UpdateQuotationTargetData);

                List<tbt_QuotationTarget> dtUpdatedTbt_QuotationTarget = base.UpdateQuotationTarget(
                    UpdateQuotationTargetData.QuotationOfficeCode,
                    UpdateQuotationTargetData.LastAlphabet,
                    UpdateQuotationTargetData.ContractTransferStatus,
                    UpdateQuotationTargetData.ContractCode,
                    UpdateQuotationTargetData.TransferDate,
                    UpdateQuotationTargetData.TransferAlphabet,
                    CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    CommonUtil.dsTransData.dtUserData.EmpNo,
                    UpdateQuotationTargetData.QuotationTargetCode,
                    UpdateQuotationTargetData.OperationOfficeCode);
                if (dtUpdatedTbt_QuotationTarget.Count > 0)
                {
                    ILogHandler handlog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog UpdateQuotationTargetLog = new doTransactionLog();
                    UpdateQuotationTargetLog.TransactionType = doTransactionLog.eTransactionType.Update;
                    UpdateQuotationTargetLog.TableName = TableName.C_TBL_NAME_QTN_TARGET;
                    UpdateQuotationTargetLog.TableData = CommonUtil.ConvertToXml(dtUpdatedTbt_QuotationTarget);
                    handlog.WriteTransactionLog(UpdateQuotationTargetLog);
                    return dtUpdatedTbt_QuotationTarget.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Update quotation data
        /// </summary>
        /// <param name="uData"></param>
        /// <returns></returns>
        public int UpdateQuotationData(doUpdateQuotationData uData)
        {
            try
            {
                if (CommonUtil.IsNullOrEmpty(uData.ActionTypeCode))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "ActionTypeCode" });
                }
                //ApplicationErrorException.CheckMandatoryField<doUpdateQuotationData, doUpdateQuotationData_Validate>(uData);
                // 2
                if (uData.ActionTypeCode == ActionType.C_ACTION_TYPE_DRAFT || uData.ActionTypeCode == ActionType.C_ACTION_TYPE_APPROVE || uData.ActionTypeCode == ActionType.C_ACTION_TYPE_CHANGE)
                {
                    doGetQuotationDataCondition QuotaionDataCodition = new doGetQuotationDataCondition();
                    QuotaionDataCodition.QuotationTargetCode = uData.QuotationTargetCode;
                    QuotaionDataCodition.Alphabet = uData.Alphabet;

                    //2.1
                    List<tbt_QuotationBasic> dtTbt_QuotationBasic = GetQuotationBasicData(QuotaionDataCodition);
                    //2.2
                    if (dtTbt_QuotationBasic.Count <= 0)
                    {
                        CommonUtil c = new CommonUtil();
                        //string txt = uData.QuotationTargetCode + "-" + uData.Alphabet;
                        string txt = CommonUtil.TextCodeName(
                                            c.ConvertQuotationTargetCode(uData.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                                            uData.Alphabet,
                                            "-");
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2006, txt);
                    }

                    doUpdateQuotationBasicData UpdateQuotationBasicData = new doUpdateQuotationBasicData();
                    UpdateQuotationBasicData.QuotationTargetCode = uData.QuotationTargetCode;
                    UpdateQuotationBasicData.Alphabet = uData.Alphabet;

                    if (uData.ActionTypeCode == ActionType.C_ACTION_TYPE_DRAFT)
                        UpdateQuotationBasicData.ContractTransferStatus = ContractTransferStatus.C_CONTRACT_TRANS_STATUS_DRAFT_CRE;
                    else
                    {
                        if (uData.ActionTypeCode == ActionType.C_ACTION_TYPE_APPROVE || uData.ActionTypeCode == ActionType.C_ACTION_TYPE_CHANGE)
                            UpdateQuotationBasicData.ContractTransferStatus = ContractTransferStatus.C_CONTRACT_TRANS_STATUS_CONTRACT_APP;
                        else
                            UpdateQuotationBasicData.ContractTransferStatus = null;
                    }
                    List<tbt_QuotationBasic> dtUpdatedTbt_QuotationBasic = UpdateQuotationBasic(CommonUtil.dsTransData.dtOperationData.ProcessDateTime, CommonUtil.dsTransData.dtUserData.EmpNo, UpdateQuotationBasicData.Alphabet, UpdateQuotationBasicData.ContractTransferStatus, UpdateQuotationBasicData.QuotationTargetCode);
                    if (dtUpdatedTbt_QuotationBasic.Count > 0)
                    {
                        doTransactionLog doTranLog = new doTransactionLog();
                        doTranLog.TransactionType = doTransactionLog.eTransactionType.Update;
                        doTranLog.TableName = TableName.C_TBL_NAME_QTN_BASIC;
                        doTranLog.TableData = CommonUtil.ConvertToXml(dtUpdatedTbt_QuotationBasic);
                        ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        hand.WriteTransactionLog(doTranLog);

                    }
                }
                // 3
                // 3.1
                List<tbt_QuotationTarget> dtTbt_QuotationTarget = GetTbt_QuotationTarget(uData.QuotationTargetCode, null, null, null);

                if (dtTbt_QuotationTarget.Count <= 0)
                {
                    if (uData.ActionTypeCode == ActionType.C_ACTION_TYPE_CANCEL
                        || uData.ActionTypeCode == ActionType.C_ACTION_TYPE_START)
                        return 1;

                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2003, uData.QuotationTargetCode);
                }

                // 3.2


                if (uData.ActionTypeCode == ActionType.C_ACTION_TYPE_DRAFT)
                {
                    if (CommonUtil.IsNullOrEmpty(uData.LastUpdateDate) || uData.LastUpdateDate == DateTime.MinValue)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "LastUpdateDate" });
                    }
                    if (DateTime.Compare(uData.LastUpdateDate, dtTbt_QuotationTarget[0].UpdateDate.Value) != 0)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                    }
                }

                //3.3 
                if (uData.ActionTypeCode == ActionType.C_ACTION_TYPE_APPROVE || uData.ActionTypeCode == ActionType.C_ACTION_TYPE_CHANGE)
                {
                    if (CommonUtil.IsNullOrEmpty(uData.ContractCode))
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "ContractCode" });
                    }
                }

                if (uData.ActionTypeCode == ActionType.C_ACTION_TYPE_START)
                {
                    if (CommonUtil.IsNullOrEmpty(uData.QuotationOfficeCode))
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "QuotationOfficeCode" });
                    }
                }

                //3.4
                doUpdateQuotationTargetData UpQuoTargetDT = new doUpdateQuotationTargetData();

                UpQuoTargetDT.QuotationTargetCode = uData.QuotationTargetCode;
                if (uData.ActionTypeCode == ActionType.C_ACTION_TYPE_DRAFT)
                    UpQuoTargetDT.ContractTransferStatus = ContractTransferStatus.C_CONTRACT_TRANS_STATUS_DRAFT_CRE;
                else if (uData.ActionTypeCode == ActionType.C_ACTION_TYPE_APPROVE)
                    UpQuoTargetDT.ContractTransferStatus = ContractTransferStatus.C_CONTRACT_TRANS_STATUS_CONTRACT_APP;
                else if (uData.ActionTypeCode == ActionType.C_ACTION_TYPE_CANCEL)
                    UpQuoTargetDT.ContractTransferStatus = ContractTransferStatus.C_CONTRACT_TRANS_STATUS_CONTRACT_CAN;
                else
                    UpQuoTargetDT.ContractTransferStatus = null;

                if (uData.ActionTypeCode == ActionType.C_ACTION_TYPE_APPROVE || uData.ActionTypeCode == ActionType.C_ACTION_TYPE_CHANGE)
                {
                    UpQuoTargetDT.ContractCode = uData.ContractCode;
                    UpQuoTargetDT.TransferDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    UpQuoTargetDT.TransferAlphabet = uData.Alphabet;
                }
                else
                {
                    UpQuoTargetDT.ContractCode = null;
                    UpQuoTargetDT.TransferDate = null;
                    UpQuoTargetDT.TransferAlphabet = null;
                }

                if (uData.ActionTypeCode == ActionType.C_ACTION_TYPE_START)
                {
                    UpQuoTargetDT.QuotationOfficeCode = uData.QuotationOfficeCode;
                }

                List<tbt_QuotationTarget> dtUpdatedTbt_QuotationTarget = UpdateQuotationTarget(
                    UpQuoTargetDT.QuotationOfficeCode,
                    UpQuoTargetDT.LastAlphabet,
                    UpQuoTargetDT.ContractTransferStatus,
                    UpQuoTargetDT.ContractCode,
                    UpQuoTargetDT.TransferDate,
                    UpQuoTargetDT.TransferAlphabet,
                    CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    CommonUtil.dsTransData.dtUserData.EmpNo,
                    UpQuoTargetDT.QuotationTargetCode,
                    UpQuoTargetDT.OperationOfficeCode);

                if (dtUpdatedTbt_QuotationTarget.Count > 0)
                {
                    doTransactionLog doTranLog = new doTransactionLog();
                    doTranLog.TransactionType = doTransactionLog.eTransactionType.Update;
                    doTranLog.TableName = TableName.C_TBL_NAME_QTN_TARGET;
                    doTranLog.TableData = CommonUtil.ConvertToXml(dtUpdatedTbt_QuotationTarget);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(doTranLog);
                }
                return dtUpdatedTbt_QuotationTarget.Count;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Update quotation installation detail (Call stored procedure: sp_QU_UpdateTbt_QuotationInstallationDetail)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int UpdateQuotationInstallationDetail(List<tbt_QuotationInstallationDetail> datalist)
        {
            try
            {
                #region Insert Data

                var res = base.UpdateTbt_QuotationInstallationDetail(
                    CommonUtil.ConvertToXml_Store<tbt_QuotationInstallationDetail>(datalist)
                );

                #endregion
                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_QTN_INSTALL_DTL,
                    TableData = CommonUtil.ConvertToXml(res)
                };
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return res.Count;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        #region Delete Methods

        /// <summary>
        /// Delete quotation data (Call stored procedure: sp_QU_DeleteQuotation)
        /// </summary>
        /// <returns></returns>
        public List<dtBatchProcessResult> DeleteQuotation()
        {
            try
            {
                return base.DeleteQuotation(FlagType.C_FLAG_ON, FlagType.C_FLAG_OFF);
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

        /// <summary>
        /// Check site data is aleady used (Call stored procedure: sp_QU_IsUsedSite)
        /// </summary>
        /// <param name="siteCode"></param>
        /// <returns></returns>
        public bool IsUsedSiteData(string siteCode)
        {
            try
            {
                if (CommonUtil.IsNullOrEmpty(siteCode))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "SiteCode" });
                }

                List<int?> list = base.IsUsedSite(siteCode);

                bool result = false;
                if (list.Count > 0)
                {
                    if (list[0].Value == 1)
                        result = true;
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Check customer data is aleady used (Call stored procedure: sp_QU_IsUsedCustomer)
        /// </summary>
        /// <param name="custCode"></param>
        /// <returns></returns>
        public bool IsUsedCustomerData(string custCode)
        {
            try
            {
                if (CommonUtil.IsNullOrEmpty(custCode))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "CustCode" });
                }

                List<int?> list = base.IsUsedCustomer(custCode);

                bool result = false;
                if (list.Count > 0)
                {
                    if (list[0].Value == 1)
                        result = true;
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
