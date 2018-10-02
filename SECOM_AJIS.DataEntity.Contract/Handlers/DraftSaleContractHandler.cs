using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.Common.Util.ConstantValue;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class DraftSaleContractHandler : BizCTDataEntities, IDraftSaleContractHandler
    {
        #region Get data
        /// <summary>
        /// Get entire draft sale contract
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="mode"></param>
        /// <param name="procType"></param>
        /// <returns></returns>
        public doDraftSaleContractData GetEntireDraftSaleContract(doDraftSaleContractCondition cond, doDraftSaleContractData.SALE_CONTRACT_MODE mode, doDraftSaleContractData.PROCESS_TYPE procType)
        {
            try
            {
                doDraftSaleContractData saleData = null;

                if (mode == doDraftSaleContractData.SALE_CONTRACT_MODE.QUOTATION)
                {
                    CommonUtil cmm = new CommonUtil();
                    doGetQuotationDataCondition qcond = new doGetQuotationDataCondition();
                    qcond.QuotationTargetCode = cond.QuotationTargetCodeLong;
                    qcond.Alphabet = cond.Alphabet;
                    qcond.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;


                    if (procType == doDraftSaleContractData.PROCESS_TYPE.NEW)
                        qcond.TargetCodeTypeCode = TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE;
                    else
                        qcond.TargetCodeTypeCode = TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE;

                    qcond.ContractFlag = true;

                    IQuotationHandler qhandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                    dsQuotationData qData = qhandler.GetQuotationData(qcond);
                    if (qData != null)
                    {
                        #region Check Authority

                        bool hasAuthority = false;
                        List<OfficeDataDo> officeLst = CommonUtil.dsTransData.dtOfficeData;
                        if (qData.dtTbt_QuotationTarget != null
                            && officeLst.Count > 0)
                        {
                            foreach (OfficeDataDo office in officeLst)
                            {
                                if (office.OfficeCode == qData.dtTbt_QuotationTarget.QuotationOfficeCode
                                    || office.OfficeCode == qData.dtTbt_QuotationTarget.OperationOfficeCode)
                                {
                                    hasAuthority = true;
                                    break;
                                }
                            }
                        }
                        if (hasAuthority == false)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);

                        #endregion

                        saleData = new doDraftSaleContractData();

                        if (qData.dtTbt_QuotationTarget.UpdateDate != null)
                            saleData.LastUpdateDateQuotationData = qData.dtTbt_QuotationTarget.UpdateDate.Value;

                        #region Set Draft Sale Contract

                        saleData.doTbt_DraftSaleContract = CommonUtil.CloneObject<tbt_QuotationBasic, tbt_DraftSaleContract>(qData.dtTbt_QuotationBasic);
                        if (saleData.doTbt_DraftSaleContract != null)
                        {
                            saleData.doTbt_DraftSaleContract.QuotationTargetCode = cond.QuotationTargetCodeLong;
                            saleData.doTbt_DraftSaleContract.Alphabet = cond.Alphabet;
                            saleData.doTbt_DraftSaleContract.ProductTypeCode = qData.dtTbt_QuotationTarget.ProductTypeCode;
                            saleData.doTbt_DraftSaleContract.BranchNameEN = qData.dtTbt_QuotationTarget.BranchNameEN;
                            saleData.doTbt_DraftSaleContract.BranchNameLC = qData.dtTbt_QuotationTarget.BranchNameLC;
                            saleData.doTbt_DraftSaleContract.BranchAddressEN = qData.dtTbt_QuotationTarget.BranchAddressEN;
                            saleData.doTbt_DraftSaleContract.BranchAddressLC = qData.dtTbt_QuotationTarget.BranchAddressLC;

                            saleData.doTbt_DraftSaleContract.PurchaserMemo = qData.dtTbt_QuotationTarget.ContractTargetMemo;
                            saleData.doTbt_DraftSaleContract.RealCustomerMemo = qData.dtTbt_QuotationTarget.RealCustomerMemo;

                            foreach (tbt_QuotationCustomer cust in qData.dtTbt_QuotationCustomer)
                            {
                                if (cust.CustPartTypeCode == CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET)
                                    saleData.doTbt_DraftSaleContract.PurchaserCustCode = cust.CustCode;
                                else if (cust.CustPartTypeCode == CustPartType.C_CUST_PART_TYPE_REAL_CUST)
                                    saleData.doTbt_DraftSaleContract.RealCustomerCustCode = cust.CustCode;
                            }
                            if (qData.dtTbt_QuotationSite != null)
                                saleData.doTbt_DraftSaleContract.SiteCode = qData.dtTbt_QuotationSite.SiteCode;

                            saleData.doTbt_DraftSaleContract.ConnectTargetCode = qData.dtTbt_QuotationBasic.SaleOnlineContractCode;
                            if (CommonUtil.IsNullOrEmpty(saleData.doTbt_DraftSaleContract.ConnectTargetCode) == false)
                                saleData.doTbt_DraftSaleContract.ConnectionFlag = FlagType.C_FLAG_ON;
                            else
                                saleData.doTbt_DraftSaleContract.ConnectionFlag = FlagType.C_FLAG_OFF;


                            saleData.doTbt_DraftSaleContract.NormalProductPriceCurrencyType = qData.dtTbt_QuotationBasic.ProductPriceCurrencyType;
                            saleData.doTbt_DraftSaleContract.NormalProductPrice = qData.dtTbt_QuotationBasic.ProductPrice;
                            saleData.doTbt_DraftSaleContract.NormalProductPriceUsd = qData.dtTbt_QuotationBasic.ProductPriceUsd;

                            saleData.doTbt_DraftSaleContract.NormalInstallFeeCurrencyType = qData.dtTbt_QuotationBasic.InstallationFeeCurrencyType;
                            saleData.doTbt_DraftSaleContract.NormalInstallFee = qData.dtTbt_QuotationBasic.InstallationFee;
                            saleData.doTbt_DraftSaleContract.NormalInstallFeeUsd = qData.dtTbt_QuotationBasic.InstallationFeeUsd;

                            if (CommonUtil.IsNullOrEmpty(saleData.doTbt_DraftSaleContract.NormalProductPrice) == false
                                || CommonUtil.IsNullOrEmpty(saleData.doTbt_DraftSaleContract.NormalInstallFee) == false)
                            {
                                saleData.doTbt_DraftSaleContract.NormalSalePrice = 0;

                                //if (CommonUtil.IsNullOrEmpty(saleData.doTbt_DraftSaleContract.NormalProductPrice) == false)
                                //    saleData.doTbt_DraftSaleContract.NormalSalePrice += saleData.doTbt_DraftSaleContract.NormalProductPrice;
                                //if (CommonUtil.IsNullOrEmpty(saleData.doTbt_DraftSaleContract.NormalInstallFee) == false)
                                //    saleData.doTbt_DraftSaleContract.NormalSalePrice += saleData.doTbt_DraftSaleContract.NormalInstallFee;
                            }
                            saleData.doTbt_DraftSaleContract.NormalSpecialItemPrice = 0;
                            saleData.doTbt_DraftSaleContract.NormalOtherProdPrice = 0;
                            saleData.doTbt_DraftSaleContract.NormalOtherInstallFee = 0;

                            //saleData.doTbt_DraftSaleContract.OrderProductPrice = qData.dtTbt_QuotationBasic.ProductPrice;
                            //saleData.doTbt_DraftSaleContract.OrderInstallFee = qData.dtTbt_QuotationBasic.InstallationFee;
                            //saleData.doTbt_DraftSaleContract.OrderSalePrice = saleData.doTbt_DraftSaleContract.NormalSalePrice;

                            saleData.doTbt_DraftSaleContract.OrderProductPriceCurrencyType = qData.dtTbt_QuotationBasic.ProductPriceCurrencyType;
                            saleData.doTbt_DraftSaleContract.OrderProductPrice = null;
                            saleData.doTbt_DraftSaleContract.OrderProductPriceUsd = null;

                            saleData.doTbt_DraftSaleContract.OrderInstallFeeCurrencyType = qData.dtTbt_QuotationBasic.InstallationFeeCurrencyType;
                            saleData.doTbt_DraftSaleContract.OrderInstallFee = null;
                            saleData.doTbt_DraftSaleContract.OrderInstallFeeUsd = null;

                            saleData.doTbt_DraftSaleContract.OrderSalePrice = null;
                            
                            saleData.doTbt_DraftSaleContract.TotalSaleBilingAmt_Agreed = 0;

                            saleData.doTbt_DraftSaleContract.QuotationStaffEmpNo = qData.dtTbt_QuotationTarget.QuotationStaffEmpNo;
                            saleData.doTbt_DraftSaleContract.QuotationOfficeCode = qData.dtTbt_QuotationTarget.QuotationOfficeCode;
                            saleData.doTbt_DraftSaleContract.OperationOfficeCode = qData.dtTbt_QuotationTarget.OperationOfficeCode;
                            saleData.doTbt_DraftSaleContract.AcquisitionTypeCode = qData.dtTbt_QuotationTarget.AcquisitionTypeCode;
                            saleData.doTbt_DraftSaleContract.IntroducerCode = qData.dtTbt_QuotationTarget.IntroducerCode;
                            saleData.doTbt_DraftSaleContract.MotivationTypeCode = qData.dtTbt_QuotationTarget.MotivationTypeCode;

                            saleData.doTbt_DraftSaleContract.ApproveNo1 = null;
                            saleData.doTbt_DraftSaleContract.ApproveNo2 = null;
                            saleData.doTbt_DraftSaleContract.ApproveNo3 = null;
                            saleData.doTbt_DraftSaleContract.ApproveNo4 = null;
                            saleData.doTbt_DraftSaleContract.ApproveNo5 = null;

                            saleData.doTbt_DraftSaleContract.CreateBy = null;
                            saleData.doTbt_DraftSaleContract.CreateDate = null;
                            saleData.doTbt_DraftSaleContract.UpdateBy = null;
                            saleData.doTbt_DraftSaleContract.UpdateDate = null;

                            List<tbt_DraftSaleContract> contractLst = this.GetTbt_DraftSaleContract(cond.QuotationTargetCodeLong);
                            if (contractLst.Count > 0)
                            {
                                saleData.doTbt_DraftSaleContract.CreateBy = contractLst[0].CreateBy;
                                saleData.doTbt_DraftSaleContract.CreateDate = contractLst[0].CreateDate;
                                saleData.doTbt_DraftSaleContract.UpdateBy = contractLst[0].UpdateBy;
                                saleData.doTbt_DraftSaleContract.UpdateDate = contractLst[0].UpdateDate;
                            }

                            if (saleData.doTbt_DraftSaleContract.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE)
                            {
                                ISaleContractHandler shandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                                if (qData.dtTbt_QuotationBasic.SaleOnlineContractCode != null)
                                {
                                    doSaleContractData doSaleContract = shandler.GetSaleContractData(qData.dtTbt_QuotationBasic.SaleOnlineContractCode, null);
                                    if (doSaleContract != null)
                                    {
                                        saleData.doTbt_DraftSaleContract.SecurityAreaFrom = doSaleContract.dtTbt_SaleBasic.SecurityAreaFrom;
                                        saleData.doTbt_DraftSaleContract.SecurityAreaTo = doSaleContract.dtTbt_SaleBasic.SecurityAreaTo;
                                    }
                                }
                            }
                        }

                        #endregion
                        #region Set Draft Sale Customer

                        MiscTypeMappingList cmLst = new MiscTypeMappingList(); 
                        ICustomerMasterHandler chandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                        foreach (tbt_QuotationCustomer cust in qData.dtTbt_QuotationCustomer)
                        {
                            doCustomerWithGroup icust = null;
                            if (cust.CustCode != null)
                            {
                                List<doCustomerWithGroup> lst = chandler.GetCustomerWithGroup(cust.CustCode);
                                if (lst.Count > 0)
                                    icust = lst[0];
                            }
                            else
                            {
                                icust = CommonUtil.CloneObject<tbt_QuotationCustomer, doCustomerWithGroup>(cust);
                                if (icust != null)
                                    icust.CustomerGroupData = new List<dtCustomeGroupData>();

                                IMasterHandler mhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                                if (icust.BusinessTypeCode != null)
                                {
                                    List<tbm_BusinessType> btLst = mhandler.GetTbm_BusinessType();
                                    foreach (tbm_BusinessType bt in btLst)
                                    {
                                        if (bt.BusinessTypeCode == icust.BusinessTypeCode)
                                        {
                                            icust.BusinessTypeName = bt.BusinessTypeName;
                                            break;
                                        }
                                    }
                                }
                                if (icust.RegionCode != null)
                                {
                                    List<tbm_Region> rLst = mhandler.GetTbm_Region();
                                    foreach (tbm_Region r in rLst)
                                    {
                                        if (r.RegionCode == icust.RegionCode)
                                        {
                                            icust.Nationality = r.Nationality;
                                            break;
                                        }
                                    }
                                }
                                if (icust.CustTypeCode != null)
                                {
                                    cmLst.AddMiscType(icust);
                                }
                            }
                            if (icust != null)
                            {
                                if (cust.CustPartTypeCode == CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET)
                                    saleData.doPurchaserCustomer = icust;
                                else if (cust.CustPartTypeCode == CustPartType.C_CUST_PART_TYPE_REAL_CUST)
                                    saleData.doRealCustomer = icust;
                            }
                        }

                        ICommonHandler cmmhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        cmmhandler.MiscTypeMappingList(cmLst);

                        #endregion
                        #region Set Draft Sale Site

                        if (qData.dtTbt_QuotationSite != null)
                        {
                            if (qData.dtTbt_QuotationSite.SiteCode != null)
                            {
                                ISiteMasterHandler shandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                                List<doSite> lst = shandler.GetSite(qData.dtTbt_QuotationSite.SiteCode, null);
                                if (lst.Count > 0)
                                    saleData.doSite = lst[0];
                            }
                            else
                            {
                                saleData.doSite = CommonUtil.CloneObject<tbt_QuotationSite, doSite>(qData.dtTbt_QuotationSite);

                                IMasterHandler mhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                                List<tbm_BuildingUsage> blst = mhandler.GetTbm_BiuldingUsage();
                                foreach (tbm_BuildingUsage b in blst)
                                {
                                    if (b.BuildingUsageCode == saleData.doSite.BuildingUsageCode)
                                    {
                                        saleData.doSite.BuildingUsageName = b.BuildingUsageName;
                                        break;
                                    }
                                }
                            }
                        }

                        #endregion
                        #region Set Draft Sale Instrument

                        saleData.doTbt_DraftSaleInstrument = new List<tbt_DraftSaleInstrument>();
                        if (qData.dtTbt_QuotationInstrumentDetails != null)
                        {
                            foreach (tbt_QuotationInstrumentDetails inst in qData.dtTbt_QuotationInstrumentDetails)
                            {
                                tbt_DraftSaleInstrument dInst = new tbt_DraftSaleInstrument();
                                dInst.QuotationTargetCode = cond.QuotationTargetCodeLong;
                                dInst.InstrumentCode = inst.InstrumentCode;
                                dInst.InstrumentQty = inst.InstrumentQty;
                                dInst.InstrumentTypeCode = InstrumentType.C_INST_TYPE_GENERAL;

                                if (CommonUtil.IsNullOrEmpty(dInst.InstrumentQty) == false)
                                {
                                    if (CommonUtil.IsNullOrEmpty(inst.AddQty) == false)
                                        dInst.InstrumentQty += inst.AddQty;
                                    if (CommonUtil.IsNullOrEmpty(inst.RemoveQty) == false)
                                        dInst.InstrumentQty -= inst.RemoveQty;
                                }

                                saleData.doTbt_DraftSaleInstrument.Add(dInst);
                            }

                        }
                        if (qData.dtTbt_QuotationFacilityDetails != null)
                        {
                            foreach (tbt_QuotationFacilityDetails facility in qData.dtTbt_QuotationFacilityDetails)
                            {
                                tbt_DraftSaleInstrument dInst = new tbt_DraftSaleInstrument();
                                dInst.QuotationTargetCode = cond.QuotationTargetCode;
                                dInst.InstrumentCode = facility.FacilityCode;
                                dInst.InstrumentQty = facility.FacilityQty;
                                dInst.InstrumentTypeCode = InstrumentType.C_INST_TYPE_MONITOR;

                                saleData.doTbt_DraftSaleInstrument.Add(dInst);
                            }
                        }

                        if (saleData.doTbt_DraftSaleInstrument.Count > 0)
                        {
                            InstrumentMappingList instMappingLst = new InstrumentMappingList();
                            instMappingLst.AddInstrument(saleData.doTbt_DraftSaleInstrument.ToArray());

                            IInstrumentMasterHandler ihandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                            ihandler.InstrumentListMapping(instMappingLst);
                        }

                        #endregion
                        #region Set Draft Relation Type

                        saleData.doTbt_RelationType = new List<tbt_RelationType>();

                        /* -- Sale Online --- */
                        if (CommonUtil.IsNullOrEmpty(qData.dtTbt_QuotationBasic.SaleOnlineContractCode) == false)
                        {
                            saleData.doTbt_RelationType.Add(new tbt_RelationType()
                            {
                                RelatedContractCode = qData.dtTbt_QuotationBasic.SaleOnlineContractCode,
                                RelatedOCC = qData.dtTbt_QuotationBasic.LastOccNo,
                                OCC = null,
                                RelationType = RelationType.C_RELATION_TYPE_SALE
                            });
                        }

                        #endregion
                    }
                }
                else if (mode == doDraftSaleContractData.SALE_CONTRACT_MODE.DRAFT
                        || mode == doDraftSaleContractData.SALE_CONTRACT_MODE.APPROVE)
                {
                    saleData = new doDraftSaleContractData();

                    #region Set Draft Sale Contract

                    List<tbt_DraftSaleContract> contractLst = this.GetTbt_DraftSaleContract(cond.QuotationTargetCodeLong);
                    if (contractLst.Count > 0)
                        saleData.doTbt_DraftSaleContract = contractLst[0];
                    else
                        return null;

                    #endregion
                    #region Check Authority

                    bool hasAuthority = false;
                    List<OfficeDataDo> officeLst = CommonUtil.dsTransData.dtOfficeData;
                    if (saleData.doTbt_DraftSaleContract != null
                        && officeLst.Count > 0)
                    {
                        foreach (OfficeDataDo office in officeLst)
                        {
                            if (office.OfficeCode == saleData.doTbt_DraftSaleContract.QuotationOfficeCode
                                || office.OfficeCode == saleData.doTbt_DraftSaleContract.OperationOfficeCode)
                            {
                                hasAuthority = true;
                                break;
                            }
                        }
                    }
                    if (hasAuthority == false)
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);

                    #endregion
                    #region Check Contract status

                    if (mode == doDraftSaleContractData.SALE_CONTRACT_MODE.DRAFT
                        && saleData.doTbt_DraftSaleContract.DraftSaleContractStatus != ApprovalStatus.C_APPROVE_STATUS_RETURNED)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3100);
                    }
                    else if (mode == doDraftSaleContractData.SALE_CONTRACT_MODE.APPROVE)
                    {
                        if (saleData.doTbt_DraftSaleContract.DraftSaleContractStatus == ApprovalStatus.C_APPROVE_STATUS_APPROVED)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3246);
                        else if (saleData.doTbt_DraftSaleContract.DraftSaleContractStatus == ApprovalStatus.C_APPROVE_STATUS_REJECTED)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3244);
                        else if (saleData.doTbt_DraftSaleContract.DraftSaleContractStatus == ApprovalStatus.C_APPROVE_STATUS_RETURNED)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3245);
                    }

                    #endregion

                    #region Set Draft Sale E-mail

                    saleData.doTbt_DraftSaleEmail = this.GetTbt_DraftSaleEmail(cond.QuotationTargetCodeLong);
                    if (saleData.doTbt_DraftSaleEmail != null)
                    {
                        IEmployeeMasterHandler empHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                        List<tbm_Employee> emps = new List<tbm_Employee>();
                        foreach (tbt_DraftSaleEmail email in saleData.doTbt_DraftSaleEmail)
                        {
                            emps.Add(new tbm_Employee()
                            {
                                EmpNo = email.ToEmpNo
                            });
                        }
                        List<tbm_Employee> empList = empHandler.GetEmployeeList(emps);
                        if (empList.Count > 0)
                        {
                            foreach (tbt_DraftSaleEmail email in saleData.doTbt_DraftSaleEmail)
                            {
                                foreach (tbm_Employee emp in empList)
                                {
                                    if (emp.EmpNo == email.ToEmpNo)
                                    {
                                        email.EmailAddress = emp.EmailAddress;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    #endregion
                    #region Set Draft Sale Instrument

                    saleData.doTbt_DraftSaleInstrument = this.GetTbt_DraftSaleInstrument(cond.QuotationTargetCodeLong);
                    if (saleData.doTbt_DraftSaleInstrument.Count > 0)
                    {
                        InstrumentMappingList instMappingLst = new InstrumentMappingList();
                        instMappingLst.AddInstrument(saleData.doTbt_DraftSaleInstrument.ToArray());

                        IInstrumentMasterHandler ihandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                        ihandler.InstrumentListMapping(instMappingLst);
                    }

                    #endregion
                    #region Set Draft Sale Billing Target

                    saleData.doTbt_DraftSaleBillingTarget = this.GetTbt_DraftSaleBillingTarget(cond.QuotationTargetCodeLong);

                    #endregion

                    #region Set Contract Customer

                    ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                    if (CommonUtil.IsNullOrEmpty(saleData.doTbt_DraftSaleContract.PurchaserCustCode) == false)
                    {
                        List<doCustomerWithGroup> custLst = custhandler.GetCustomerWithGroup(saleData.doTbt_DraftSaleContract.PurchaserCustCode);
                        if (custLst.Count > 0)
                            saleData.doPurchaserCustomer = custLst[0];
                    }
                    if (CommonUtil.IsNullOrEmpty(saleData.doTbt_DraftSaleContract.RealCustomerCustCode) == false)
                    {
                        List<doCustomerWithGroup> custLst = custhandler.GetCustomerWithGroup(saleData.doTbt_DraftSaleContract.RealCustomerCustCode);
                        if (custLst.Count > 0)
                            saleData.doRealCustomer = custLst[0];
                    }

                    #endregion
                    #region Set Site

                    ISiteMasterHandler shandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                    List<doSite> siteLst = shandler.GetSite(saleData.doTbt_DraftSaleContract.SiteCode, saleData.doTbt_DraftSaleContract.RealCustomerCustCode);
                    if (siteLst.Count > 0)
                        saleData.doSite = siteLst[0];

                    #endregion

                    doGetQuotationDataCondition qcond = new doGetQuotationDataCondition();
                    qcond.QuotationTargetCode = cond.QuotationTargetCodeLong;
                    qcond.Alphabet = saleData.doTbt_DraftSaleContract.Alphabet;//cond.Alphabet;
                    qcond.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;

                    if (saleData.doTbt_DraftSaleContract.SaleProcessType == SaleProcessType.C_SALE_PROCESS_TYPE_ADD_SALE)
                        qcond.TargetCodeTypeCode = TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE;
                    else
                        qcond.TargetCodeTypeCode = TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE;

                    qcond.ContractFlag = true;

                    IQuotationHandler qhandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                    dsQuotationData qData = qhandler.GetQuotationData(qcond);
                    if (qData != null)
                    {
                        if (qData.dtTbt_QuotationTarget.UpdateDate != null)
                            saleData.LastUpdateDateQuotationData = qData.dtTbt_QuotationTarget.UpdateDate.Value;
                    }
                }

                if (saleData != null)
                {
                    if (saleData.doTbt_DraftSaleContract != null)
                    {
                        #region Set Product Name

                        IProductMasterHandler mhandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                        List<View_tbm_Product> pLst = mhandler.GetTbm_ProductByLanguage(
                                saleData.doTbt_DraftSaleContract.ProductCode,
                                saleData.doTbt_DraftSaleContract.ProductTypeCode);

                        if (pLst.Count > 0)
                            saleData.doTbt_DraftSaleContract.ProductName = pLst[0].ProductName;

                        #endregion
                        #region Set Misc Name

                        MiscTypeMappingList miscLst = new MiscTypeMappingList();
                        miscLst.AddMiscType(saleData.doTbt_DraftSaleContract);
                        miscLst.AddMiscType(saleData.doTbt_DraftSaleInstrument.ToArray());

                        ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        chandler.MiscTypeMappingList(miscLst);

                        #endregion
                        #region Set Employee Name

                        EmployeeMappingList empLst = new EmployeeMappingList();
                        empLst.AddEmployee(saleData.doTbt_DraftSaleContract);

                        IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                        emphandler.EmployeeListMapping(empLst);

                        #endregion
                    }
                }

                return saleData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        #region Insert data
        /// <summary>
        /// To create draft sale contract data
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        public bool CreateDraftSaleContractData(doDraftSaleContractData draft)
        {
            try
            {
                InsertTbt_DraftSaleContract(draft.doTbt_DraftSaleContract);
                InsertTbt_DraftSaleBillingTarget(draft.doTbt_DraftSaleBillingTarget);
                InsertTbt_DraftSaleInstrument(draft.doTbt_DraftSaleInstrument);
                InsertTbt_DraftSaleEmail(draft.doTbt_DraftSaleEmail);

                ICommonContractHandler chandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                chandler.InsertTbt_RelationType(draft.doTbt_RelationType);

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert draft sale contract data
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        private int InsertTbt_DraftSaleContract(tbt_DraftSaleContract draft)
        {
            try
            {
                List<tbt_DraftSaleContract> draftLst = new List<tbt_DraftSaleContract>();
                if (draft != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    draft.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                    draft.CreateBy = dsTrans.dtUserData.EmpNo;
                    draft.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                    draft.UpdateBy = dsTrans.dtUserData.EmpNo;

                    draftLst.Add(draft);
                }

                List<tbt_DraftSaleContract> res = this.InsertTbt_DraftSaleContract(
                    CommonUtil.ConvertToXml_Store<tbt_DraftSaleContract>(draftLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_DRF_SALE_CONTRACT,
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
        /// Insert draft sale billing target
        /// </summary>
        /// <param name="draftLst"></param>
        /// <returns></returns>
        private int InsertTbt_DraftSaleBillingTarget(List<tbt_DraftSaleBillingTarget> draftLst)
        {
            try
            {
                if (draftLst != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_DraftSaleBillingTarget draft in draftLst)
                    {
                        draft.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        draft.CreateBy = dsTrans.dtUserData.EmpNo;
                        draft.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        draft.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_DraftSaleBillingTarget> res = this.InsertTbt_DraftSaleBillingTarget(
                    CommonUtil.ConvertToXml_Store<tbt_DraftSaleBillingTarget>(draftLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_DRF_SALE_BILLING,
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
        /// Insert draft sale instrument
        /// </summary>
        /// <param name="draftLst"></param>
        /// <returns></returns>
        private int InsertTbt_DraftSaleInstrument(List<tbt_DraftSaleInstrument> draftLst)
        {
            try
            {
                if (draftLst != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_DraftSaleInstrument draft in draftLst)
                    {
                        draft.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        draft.CreateBy = dsTrans.dtUserData.EmpNo;
                        draft.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        draft.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_DraftSaleInstrument> res = this.InsertTbt_DraftSaleInstrument(
                    CommonUtil.ConvertToXml_Store<tbt_DraftSaleInstrument>(draftLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_DRF_SALE_INST,
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
        /// Insert draft sale email
        /// </summary>
        /// <param name="draftLst"></param>
        /// <returns></returns>
        private int InsertTbt_DraftSaleEmail(List<tbt_DraftSaleEmail> draftLst)
        {
            try
            {
                if (draftLst != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_DraftSaleEmail draft in draftLst)
                    {
                        draft.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        draft.CreateBy = dsTrans.dtUserData.EmpNo;
                        draft.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        draft.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_DraftSaleEmail> res = this.InsertTbt_DraftSaleEmail(
                    CommonUtil.ConvertToXml_Store<tbt_DraftSaleEmail>(draftLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_DRF_SALE_EMAIL,
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
        #region Update Data
        /// <summary>
        /// To edit draft sale contract data
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        public bool EditDraftSaleContractData(doDraftSaleContractData draft)
        {
            try
            {
                UpdateTbt_DraftSaleContract(draft.doTbt_DraftSaleContract);
                UpdateTbt_DraftSaleBillingTarget(draft.doTbt_DraftSaleContract.QuotationTargetCode, draft.doTbt_DraftSaleBillingTarget);
                UpdateTbt_DraftSaleInstrument(draft.doTbt_DraftSaleContract.QuotationTargetCode, draft.doTbt_DraftSaleInstrument);
                UpdateTbt_DraftSaleEmail(draft.doTbt_DraftSaleContract.QuotationTargetCode, draft.doTbt_DraftSaleEmail);

                ICommonContractHandler chandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                chandler.UpdateTbt_RelationType(draft.doTbt_DraftSaleContract.QuotationTargetCode, draft.doTbt_RelationType);

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update draft sale contract
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        public int UpdateTbt_DraftSaleContract(tbt_DraftSaleContract draft)
        {
            try
            {
                List<tbt_DraftSaleContract> draftLst = new List<tbt_DraftSaleContract>();
                if (draft != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    draft.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                    draft.UpdateBy = dsTrans.dtUserData.EmpNo;

                    draftLst.Add(draft);
                }

                List<tbt_DraftSaleContract> res = this.UpdateTbt_DraftSaleContract(
                    CommonUtil.ConvertToXml_Store<tbt_DraftSaleContract>(draftLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_DRF_RNT_CONTRACT,
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
        /// Update draft sale billing target
        /// </summary>
        /// <param name="QuotationTargetCode"></param>
        /// <param name="draftLst"></param>
        /// <returns></returns>
        private int UpdateTbt_DraftSaleBillingTarget(string QuotationTargetCode, List<tbt_DraftSaleBillingTarget> draftLst)
        {
            try
            {
                List<tbt_DraftSaleBillingTarget> res = this.DeleteTbt_DraftSaleBillingTarget(QuotationTargetCode);

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Delete,
                    TableName = TableName.C_TBL_NAME_DRF_SALE_BILLING,
                    TableData = CommonUtil.ConvertToXml(res)
                };

                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return InsertTbt_DraftSaleBillingTarget(draftLst);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update draft sale instrument
        /// </summary>
        /// <param name="QuotationTargetCode"></param>
        /// <param name="draftLst"></param>
        /// <returns></returns>
        private int UpdateTbt_DraftSaleInstrument(string QuotationTargetCode, List<tbt_DraftSaleInstrument> draftLst)
        {
            try
            {
                List<tbt_DraftSaleInstrument> res = this.DeleteTbt_DraftSaleInstrument(QuotationTargetCode);

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Delete,
                    TableName = TableName.C_TBL_NAME_DRF_SALE_INST,
                    TableData = CommonUtil.ConvertToXml(res)
                };

                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return InsertTbt_DraftSaleInstrument(draftLst);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update draft sale email
        /// </summary>
        /// <param name="QuotationTargetCode"></param>
        /// <param name="draftLst"></param>
        /// <returns></returns>
        private int UpdateTbt_DraftSaleEmail(string QuotationTargetCode, List<tbt_DraftSaleEmail> draftLst)
        {
            try
            {
                List<tbt_DraftSaleEmail> res = this.DeleteTbt_DraftSaleEmail(QuotationTargetCode);

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Delete,
                    TableName = TableName.C_TBL_NAME_DRF_SALE_EMAIL,
                    TableData = CommonUtil.ConvertToXml(res)
                };

                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return InsertTbt_DraftSaleEmail(draftLst);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
