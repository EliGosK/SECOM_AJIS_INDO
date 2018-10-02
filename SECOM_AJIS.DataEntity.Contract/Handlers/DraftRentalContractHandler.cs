using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSI.WindsorHelper;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class DraftRentalContractHandler : BizCTDataEntities, IDraftRentalContractHandler
    {
        #region Get Data
        /// <summary>
        /// Get entire draft rental contract data
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public doDraftRentalContractData GetEntireDraftRentalContract(doDraftRentalContractCondition cond, doDraftRentalContractData.RENTAL_CONTRACT_MODE mode)
        {
            try
            {
                doDraftRentalContractData rentalData = null;

                if (mode == doDraftRentalContractData.RENTAL_CONTRACT_MODE.QUOTATION)
                {
                    CommonUtil cmm = new CommonUtil();
                    doGetQuotationDataCondition qcond = new doGetQuotationDataCondition();
                    qcond.QuotationTargetCode = cond.QuotationTargetCodeLong;
                    qcond.Alphabet = cond.Alphabet;
                    qcond.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;
                    qcond.TargetCodeTypeCode = TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE;
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

                        rentalData = new doDraftRentalContractData();

                        if (qData.dtTbt_QuotationTarget.UpdateDate != null)
                            rentalData.LastUpdateDateQuotationData = qData.dtTbt_QuotationTarget.UpdateDate.Value;

                        #region Set Draft Rental Contract

                        rentalData.doTbt_DraftRentalContrat = CommonUtil.CloneObject<tbt_QuotationBasic, tbt_DraftRentalContract>(qData.dtTbt_QuotationBasic);
                        if (rentalData.doTbt_DraftRentalContrat != null)
                        {
                            rentalData.doTbt_DraftRentalContrat.QuotationTargetCode = cond.QuotationTargetCodeLong;
                            rentalData.doTbt_DraftRentalContrat.Alphabet = cond.Alphabet;
                            rentalData.doTbt_DraftRentalContrat.ProductTypeCode = qData.dtTbt_QuotationTarget.ProductTypeCode;
                            rentalData.doTbt_DraftRentalContrat.BranchNameEN = qData.dtTbt_QuotationTarget.BranchNameEN;
                            rentalData.doTbt_DraftRentalContrat.BranchNameLC = qData.dtTbt_QuotationTarget.BranchNameLC;
                            rentalData.doTbt_DraftRentalContrat.BranchAddressEN = qData.dtTbt_QuotationTarget.BranchAddressEN;
                            rentalData.doTbt_DraftRentalContrat.BranchAddressLC = qData.dtTbt_QuotationTarget.BranchAddressLC;

                            rentalData.doTbt_DraftRentalContrat.ContractTargetMemo = qData.dtTbt_QuotationTarget.ContractTargetMemo;
                            rentalData.doTbt_DraftRentalContrat.RealCustomerMemo = qData.dtTbt_QuotationTarget.RealCustomerMemo;

                            foreach (tbt_QuotationCustomer cust in qData.dtTbt_QuotationCustomer)
                            {
                                if (cust.CustPartTypeCode == CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET)
                                    rentalData.doTbt_DraftRentalContrat.ContractTargetCustCode = cust.CustCode;
                                else if (cust.CustPartTypeCode == CustPartType.C_CUST_PART_TYPE_REAL_CUST)
                                    rentalData.doTbt_DraftRentalContrat.RealCustomerCustCode = cust.CustCode;
                            }
                            if (qData.dtTbt_QuotationSite != null)
                                rentalData.doTbt_DraftRentalContrat.SiteCode = qData.dtTbt_QuotationSite.SiteCode;

                            #region Normal Contract Fee

                            rentalData.doTbt_DraftRentalContrat.NormalContractFeeCurrencyType = qData.dtTbt_QuotationBasic.ContractFeeCurrencyType;
                            rentalData.doTbt_DraftRentalContrat.NormalContractFee = qData.dtTbt_QuotationBasic.ContractFee;
                            rentalData.doTbt_DraftRentalContrat.NormalContractFeeUsd = qData.dtTbt_QuotationBasic.ContractFeeUsd;

                            #endregion
                            #region Normal Install Fee

                            rentalData.doTbt_DraftRentalContrat.NormalInstallFeeCurrencyType = qData.dtTbt_QuotationBasic.InstallationFeeCurrencyType;
                            rentalData.doTbt_DraftRentalContrat.NormalInstallFee = qData.dtTbt_QuotationBasic.InstallationFee;
                            rentalData.doTbt_DraftRentalContrat.NormalInstallFeeUsd = qData.dtTbt_QuotationBasic.InstallationFeeUsd;

                            #endregion
                            #region Normal Deposit Fee

                            rentalData.doTbt_DraftRentalContrat.NormalDepositFeeCurrencyType = qData.dtTbt_QuotationBasic.DepositFeeCurrencyType;
                            rentalData.doTbt_DraftRentalContrat.NormalDepositFee = qData.dtTbt_QuotationBasic.DepositFee;
                            rentalData.doTbt_DraftRentalContrat.NormalDepositFeeUsd = qData.dtTbt_QuotationBasic.DepositFeeUsd;

                            #endregion

                            rentalData.doTbt_DraftRentalContrat.TotalFloorArea = 0;

                            rentalData.doTbt_DraftRentalContrat.QuotationStaffEmpNo = qData.dtTbt_QuotationTarget.QuotationStaffEmpNo;
                            rentalData.doTbt_DraftRentalContrat.QuotationOfficeCode = qData.dtTbt_QuotationTarget.QuotationOfficeCode;
                            rentalData.doTbt_DraftRentalContrat.OperationOfficeCode = qData.dtTbt_QuotationTarget.OperationOfficeCode;

                            //rentalData.doTbt_DraftRentalContrat.OrderContractFee = qData.dtTbt_QuotationBasic.ContractFee;
                            //rentalData.doTbt_DraftRentalContrat.OrderInstallFee = qData.dtTbt_QuotationBasic.InstallationFee;
                            //rentalData.doTbt_DraftRentalContrat.OrderDepositFee = qData.dtTbt_QuotationBasic.DepositFee;
                            rentalData.doTbt_DraftRentalContrat.OrderContractFee = null;
                            rentalData.doTbt_DraftRentalContrat.OrderInstallFee = null;
                            rentalData.doTbt_DraftRentalContrat.OrderDepositFee = null;

                            #region Insurance Converange

                            rentalData.doTbt_DraftRentalContrat.InsuranceCoverageAmountCurrencyType = qData.dtTbt_QuotationBasic.InsuranceCoverageAmountCurrencyType;
                            rentalData.doTbt_DraftRentalContrat.InsuranceCoverageAmount = qData.dtTbt_QuotationBasic.InsuranceCoverageAmount;
                            rentalData.doTbt_DraftRentalContrat.InsuranceCoverageAmountUsd = qData.dtTbt_QuotationBasic.InsuranceCoverageAmountUsd;

                            #endregion
                            #region Monthly Insurance Fee

                            rentalData.doTbt_DraftRentalContrat.MonthlyInsuranceFeeCurrencyType = qData.dtTbt_QuotationBasic.MonthlyInsuranceFeeCurrencyType;
                            rentalData.doTbt_DraftRentalContrat.MonthlyInsuranceFee= qData.dtTbt_QuotationBasic.MonthlyInsuranceFee;
                            rentalData.doTbt_DraftRentalContrat.MonthlyInsuranceFeeUsd = qData.dtTbt_QuotationBasic.MonthlyInsuranceFeeUsd;

                            #endregion

                            #region Additional Fee 1

                            rentalData.doTbt_DraftRentalContrat.AdditionalFee1CurrencyType = qData.dtTbt_QuotationBasic.AdditionalFee1CurrencyType;
                            rentalData.doTbt_DraftRentalContrat.AdditionalFee1 = qData.dtTbt_QuotationBasic.AdditionalFee1;
                            rentalData.doTbt_DraftRentalContrat.AdditionalFee1Usd = qData.dtTbt_QuotationBasic.AdditionalFee1Usd;

                            #endregion
                            #region Additional Fee 2

                            rentalData.doTbt_DraftRentalContrat.AdditionalFee2CurrencyType = qData.dtTbt_QuotationBasic.AdditionalFee2CurrencyType;
                            rentalData.doTbt_DraftRentalContrat.AdditionalFee2 = qData.dtTbt_QuotationBasic.AdditionalFee2;
                            rentalData.doTbt_DraftRentalContrat.AdditionalFee2Usd = qData.dtTbt_QuotationBasic.AdditionalFee2Usd;

                            #endregion
                            #region Additional Fee 3

                            rentalData.doTbt_DraftRentalContrat.AdditionalFee3CurrencyType = qData.dtTbt_QuotationBasic.AdditionalFee3CurrencyType;
                            rentalData.doTbt_DraftRentalContrat.AdditionalFee3 = qData.dtTbt_QuotationBasic.AdditionalFee3;
                            rentalData.doTbt_DraftRentalContrat.AdditionalFee3Usd = qData.dtTbt_QuotationBasic.AdditionalFee3Usd;

                            #endregion

                            rentalData.doTbt_DraftRentalContrat.OldContractCode = qData.dtTbt_QuotationTarget.OldContractCode;
                            rentalData.doTbt_DraftRentalContrat.AcquisitionTypeCode = qData.dtTbt_QuotationTarget.AcquisitionTypeCode;
                            rentalData.doTbt_DraftRentalContrat.IntroducerCode = qData.dtTbt_QuotationTarget.IntroducerCode;
                            rentalData.doTbt_DraftRentalContrat.MotivationTypeCode = qData.dtTbt_QuotationTarget.MotivationTypeCode;
                            
                            rentalData.doTbt_DraftRentalContrat.ApproveNo1 = null;
                            rentalData.doTbt_DraftRentalContrat.ApproveNo2 = null;
                            rentalData.doTbt_DraftRentalContrat.ApproveNo3 = null;
                            rentalData.doTbt_DraftRentalContrat.ApproveNo4 = null;
                            rentalData.doTbt_DraftRentalContrat.ApproveNo5 = null;

                            rentalData.doTbt_DraftRentalContrat.CreateBy = null;
                            rentalData.doTbt_DraftRentalContrat.CreateDate = null;
                            rentalData.doTbt_DraftRentalContrat.UpdateBy = null;
                            rentalData.doTbt_DraftRentalContrat.UpdateDate = null;

                            List<tbt_DraftRentalContract> contractLst = this.GetTbt_DraftRentalContract(cond.QuotationTargetCodeLong);
                            if (contractLst.Count > 0)
                            {
                                rentalData.doTbt_DraftRentalContrat.CreateBy = contractLst[0].CreateBy;
                                rentalData.doTbt_DraftRentalContrat.CreateDate = contractLst[0].CreateDate;
                                rentalData.doTbt_DraftRentalContrat.UpdateBy = contractLst[0].UpdateBy;
                                rentalData.doTbt_DraftRentalContrat.UpdateDate = contractLst[0].UpdateDate;
                            }

                            if (rentalData.doTbt_DraftRentalContrat.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE)
                            {
                                ISaleContractHandler shandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                                if (qData.dtTbt_QuotationBasic.SaleOnlineContractCode != null)
                                {
                                    doSaleContractData doSaleContract = shandler.GetSaleContractData(qData.dtTbt_QuotationBasic.SaleOnlineContractCode, null);
                                    if (doSaleContract != null)
                                    {
                                        rentalData.doTbt_DraftRentalContrat.SecurityAreaFrom = doSaleContract.dtTbt_SaleBasic.SecurityAreaFrom;
                                        rentalData.doTbt_DraftRentalContrat.SecurityAreaTo = doSaleContract.dtTbt_SaleBasic.SecurityAreaTo;
                                    }
                                }
                            }
                        }

                        #endregion
                        #region Set Draft Rental Customer

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
                                    rentalData.doContractCustomer = icust;
                                else if (cust.CustPartTypeCode == CustPartType.C_CUST_PART_TYPE_REAL_CUST)
                                    rentalData.doRealCustomer = icust;
                            }
                        }

                        ICommonHandler cmmhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        cmmhandler.MiscTypeMappingList(cmLst);

                        #endregion
                        #region Set Draft Rental Site

                        if (qData.dtTbt_QuotationSite != null)
                        {
                            if (qData.dtTbt_QuotationSite.SiteCode != null)
                            {
                                ISiteMasterHandler shandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                                List<doSite> lst = shandler.GetSite(qData.dtTbt_QuotationSite.SiteCode, null);
                                if (lst.Count > 0)
                                    rentalData.doSite = lst[0];
                            }
                            else
                            {
                                rentalData.doSite = CommonUtil.CloneObject<tbt_QuotationSite, doSite>(qData.dtTbt_QuotationSite);

                                IMasterHandler mhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                                List<tbm_BuildingUsage> blst =  mhandler.GetTbm_BiuldingUsage();
                                foreach (tbm_BuildingUsage b in blst)
                                {
                                    if (b.BuildingUsageCode == rentalData.doSite.BuildingUsageCode)
                                    {
                                        rentalData.doSite.BuildingUsageName = b.BuildingUsageName;
                                        break;
                                    }
                                }
                            }
                        }

                        #endregion
                        #region Set Draft Rental Instrument

                        rentalData.doTbt_DraftRentalInstrument = new List<tbt_DraftRentalInstrument>();
                        if (qData.dtTbt_QuotationInstrumentDetails != null)
                        {
                            foreach (tbt_QuotationInstrumentDetails inst in qData.dtTbt_QuotationInstrumentDetails)
                            {
                                tbt_DraftRentalInstrument dInst = new tbt_DraftRentalInstrument();
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

                                rentalData.doTbt_DraftRentalInstrument.Add(dInst);
                            }

                        }
                        if (qData.dtTbt_QuotationFacilityDetails != null)
                        {
                            foreach (tbt_QuotationFacilityDetails facility in qData.dtTbt_QuotationFacilityDetails)
                            {
                                tbt_DraftRentalInstrument dInst = new tbt_DraftRentalInstrument();
                                dInst.QuotationTargetCode = cond.QuotationTargetCode;
                                dInst.InstrumentCode = facility.FacilityCode;
                                dInst.InstrumentQty = facility.FacilityQty;
                                dInst.InstrumentTypeCode = InstrumentType.C_INST_TYPE_MONITOR;

                                rentalData.doTbt_DraftRentalInstrument.Add(dInst);
                            }
                        }

                        if (rentalData.doTbt_DraftRentalInstrument.Count > 0)
                        {
                            InstrumentMappingList instMappingLst = new InstrumentMappingList();
                            instMappingLst.AddInstrument(rentalData.doTbt_DraftRentalInstrument.ToArray());

                            IInstrumentMasterHandler ihandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                            ihandler.InstrumentListMapping(instMappingLst);
                        }

                        #endregion
                        #region Set Draft Rental Maintenance Details

                        if (qData.dtTbt_QuotationBasic != null)
                        {
                            rentalData.doTbt_DraftRentalMaintenanceDetails = new tbt_DraftRentalMaintenanceDetails()
                            {
                                MaintenanceTargetProductTypeCode = qData.dtTbt_QuotationBasic.MaintenanceTargetProductTypeCode,
                                MaintenanceTypeCode = qData.dtTbt_QuotationBasic.MaintenanceTypeCode,
                                MaintenanceMemo = qData.dtTbt_QuotationBasic.MaintenanceMemo
                            };
                        }

                        #endregion
                        #region Set Draft Rental Operation Type

                        if (qData.dtTbt_QuotationOperationType != null)
                        {
                            rentalData.doTbt_DraftRentalOperationType = new List<tbt_DraftRentalOperationType>();
                            foreach (tbt_QuotationOperationType opt in qData.dtTbt_QuotationOperationType)
                            {
                                tbt_DraftRentalOperationType rOpt = new tbt_DraftRentalOperationType()
                                {
                                    OperationTypeCode = opt.OperationTypeCode,
                                    QuotationTargetCode = opt.OperationTypeCode
                                };
                                rentalData.doTbt_DraftRentalOperationType.Add(rOpt);
                            }
                        }

                        #endregion
                        #region Set Draft Relation Type

                        rentalData.doTbt_RelationType = new List<tbt_RelationType>();

                        /* -- Sale Online --- */
                        if (CommonUtil.IsNullOrEmpty(qData.dtTbt_QuotationBasic.SaleOnlineContractCode) == false)
                        {
                            ISaleContractHandler scHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                            string SaleOnlineLastOCC = scHandler.GetLastOCC(qData.dtTbt_QuotationBasic.SaleOnlineContractCode);
                            rentalData.doTbt_RelationType.Add(new tbt_RelationType()
                            {
                                RelatedContractCode = qData.dtTbt_QuotationBasic.SaleOnlineContractCode,
                                RelatedOCC = SaleOnlineLastOCC,
                                OCC = null,
                                RelationType = RelationType.C_RELATION_TYPE_SALE
                            });
                        }

                        /* --- Linkage --- */
                        if (qData.dtTbt_QuotationMaintenanceLinkage != null)
                        {
                            List<string> mlst = new List<string>();
                            foreach (tbt_QuotationMaintenanceLinkage linkage in qData.dtTbt_QuotationMaintenanceLinkage)
                            {
                                if (CommonUtil.IsNullOrEmpty(linkage.ContractCode) == false)
                                    mlst.Add(linkage.ContractCode);
                            }
                            if (mlst.Count > 0)
                            {
                                ICommonContractHandler cmhandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                                List<tbt_RelationType> rLst = cmhandler.GenerateMaintenanceRelationType(null, mlst, true);
                                rentalData.doTbt_RelationType.AddRange(rLst.ToArray());

                            }
                        }
                        if (rentalData.doTbt_RelationType.Count > 0)
                        {
                            List<tbt_SaleBasic> lst = new List<tbt_SaleBasic>();
                            foreach (tbt_RelationType rm in rentalData.doTbt_RelationType)
                            {
                                if (CommonUtil.IsNullOrEmpty(rm.RelatedContractCode) == false)
                                {
                                    lst.Add(new tbt_SaleBasic()
                                    {
                                        ContractCode = rm.RelatedContractCode
                                    });
                                }
                            }

                            IContractHandler cthandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                            List<doContractHeader> ctLst = cthandler.GetContractHeaderDataByLanguage(lst);
                            if (ctLst.Count > 0)
                            {
                                foreach (tbt_RelationType rm in rentalData.doTbt_RelationType)
                                {
                                    foreach (doContractHeader ct in ctLst)
                                    {
                                        if (ct.ContractCode == rm.RelatedContractCode)
                                        {
                                            rm.ProductTypeCode = ct.ProductTypeCode;
                                            rm.ProductName = ct.ProductName;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        #endregion
                        #region Set Draft Rental BE detail

                        if (qData.dtTbt_QuotationBeatGuardDetails != null)
                        {
                            rentalData.doTbt_DraftRentalBEDetails = CommonUtil.CloneObject<tbt_QuotationBeatGuardDetails, tbt_DraftRentalBEDetails>(qData.dtTbt_QuotationBeatGuardDetails);
                            rentalData.doTbt_DraftRentalBEDetails.FreqOfGateUsage = qData.dtTbt_QuotationBeatGuardDetails.FreqOfGateUsage;
                            rentalData.doTbt_DraftRentalBEDetails.NotifyTime = qData.dtTbt_QuotationBeatGuardDetails.NotifyTime;
                            rentalData.doTbt_DraftRentalBEDetails.NumOfBeatStep = qData.dtTbt_QuotationBeatGuardDetails.NumOfBeatStep;
                            rentalData.doTbt_DraftRentalBEDetails.NumOfClockKey = qData.dtTbt_QuotationBeatGuardDetails.NumOfClockKey;
                            rentalData.doTbt_DraftRentalBEDetails.NumOfDate = qData.dtTbt_QuotationBeatGuardDetails.NumOfDate;
                            rentalData.doTbt_DraftRentalBEDetails.NumOfDayTimeSat = qData.dtTbt_QuotationBeatGuardDetails.NumOfDayTimeSat;
                            rentalData.doTbt_DraftRentalBEDetails.NumOfDayTimeSun = qData.dtTbt_QuotationBeatGuardDetails.NumOfDayTimeSun;
                            rentalData.doTbt_DraftRentalBEDetails.NumOfDayTimeWd = qData.dtTbt_QuotationBeatGuardDetails.NumOfDayTimeWd;
                            rentalData.doTbt_DraftRentalBEDetails.NumOfNightTimeSat = qData.dtTbt_QuotationBeatGuardDetails.NumOfNightTimeSat;
                            rentalData.doTbt_DraftRentalBEDetails.NumOfNightTimeSun = qData.dtTbt_QuotationBeatGuardDetails.NumOfNightTimeSun;
                            rentalData.doTbt_DraftRentalBEDetails.NumOfNightTimeWd = qData.dtTbt_QuotationBeatGuardDetails.NumOfNightTimeWd;
                            rentalData.doTbt_DraftRentalBEDetails.QuotationTargetCode = cond.QuotationTargetCodeLong;
                        }

                        #endregion
                        #region Set Draft sentry guard

                        rentalData.doTbt_DraftRentalSentryGuard = CommonUtil.CloneObject<tbt_QuotationBasic, tbt_DraftRentalSentryGuard>(qData.dtTbt_QuotationBasic);
                        rentalData.doTbt_DraftRentalSentryGuardDetails = new List<tbt_DraftRentalSentryGuardDetails>();
                        if (qData.dtTbt_QuotationSentryGuardDetails != null)
                        {
                            if (qData.dtTbt_QuotationSentryGuardDetails.Count > 0)
                            {
                                foreach (tbt_QuotationSentryGuardDetails sgd in qData.dtTbt_QuotationSentryGuardDetails)
                                {
                                    rentalData.doTbt_DraftRentalSentryGuardDetails.Add(new tbt_DraftRentalSentryGuardDetails()
                                    {
                                        SequenceNo = sgd.RunningNo,
                                        SecurityStartTime = sgd.SecurityStartTime,
                                        SecurityFinishTime = sgd.SecurityFinishTime,
                                        SentryGuardTypeCode = sgd.SentryGuardTypeCode,
                                        NumOfDate = sgd.NumOfDate,

                                        TimeUnitPriceCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL,
                                        TimeUnitPrice = sgd.CostPerHour,
                                        TimeUnitPriceUsd = null,

                                        WorkHourPerMonthCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL,
                                        WorkHourPerMonth = sgd.WorkHourPerMonth,
                                        WorkHourPerMonthUsd = null,

                                        NumOfSentryGuard = sgd.NumOfSentryGuard
                                    });
                                }
                            }
                        }

                        #endregion
                    }
                }
                else if (mode == doDraftRentalContractData.RENTAL_CONTRACT_MODE.DRAFT
                        || mode == doDraftRentalContractData.RENTAL_CONTRACT_MODE.APPROVE)
                {
                    rentalData = new doDraftRentalContractData();

                    #region Set Draft Rental Contract

                    List<tbt_DraftRentalContract> contractLst = this.GetTbt_DraftRentalContract(cond.QuotationTargetCodeLong);
                    if (contractLst.Count > 0)
                        rentalData.doTbt_DraftRentalContrat = contractLst[0];
                    else
                        return null;

                    #endregion
                    #region Check Authority

                    bool hasAuthority = false;
                    List<OfficeDataDo> officeLst = CommonUtil.dsTransData.dtOfficeData;
                    if (rentalData.doTbt_DraftRentalContrat != null
                        && officeLst.Count > 0)
                    {
                        foreach (OfficeDataDo office in officeLst)
                        {
                            if (office.OfficeCode == rentalData.doTbt_DraftRentalContrat.QuotationOfficeCode
                                || office.OfficeCode == rentalData.doTbt_DraftRentalContrat.OperationOfficeCode)
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

                    if (mode == doDraftRentalContractData.RENTAL_CONTRACT_MODE.DRAFT
                        && rentalData.doTbt_DraftRentalContrat.DraftRentalContractStatus != ApprovalStatus.C_APPROVE_STATUS_RETURNED)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3068, cond.QuotationTargetCode);
                    }
                    else if (mode == doDraftRentalContractData.RENTAL_CONTRACT_MODE.APPROVE)
                    {
                        if (rentalData.doTbt_DraftRentalContrat.DraftRentalContractStatus == ApprovalStatus.C_APPROVE_STATUS_APPROVED)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3246);
                        else if (rentalData.doTbt_DraftRentalContrat.DraftRentalContractStatus == ApprovalStatus.C_APPROVE_STATUS_REJECTED)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3244);
                        else if (rentalData.doTbt_DraftRentalContrat.DraftRentalContractStatus == ApprovalStatus.C_APPROVE_STATUS_RETURNED)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3245);
                    }

                    #endregion

                    #region Set Draft Rental E-mail

                    rentalData.doTbt_DraftRentalEmail = this.GetTbt_DraftRentalEmail(cond.QuotationTargetCodeLong);
                    if (rentalData.doTbt_DraftRentalEmail != null)
                    {
                        IEmployeeMasterHandler empHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                        
                        List<tbm_Employee> emps = new List<tbm_Employee>();
                        foreach (tbt_DraftRentalEmail email in rentalData.doTbt_DraftRentalEmail)
                        {
                            emps.Add(new tbm_Employee()
                            {
                                EmpNo = email.ToEmpNo
                            });
                        }
                        List<tbm_Employee> empList = empHandler.GetEmployeeList(emps);
                        if (empList.Count > 0)
                        {
                            foreach (tbt_DraftRentalEmail email in rentalData.doTbt_DraftRentalEmail)
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
                    #region Set Draft Rental Maintenance Details

                    List<tbt_DraftRentalMaintenanceDetails> maLst = this.GetTbt_DraftRentalMaintenanceDetails(cond.QuotationTargetCodeLong);
                    if (maLst.Count > 0)
                        rentalData.doTbt_DraftRentalMaintenanceDetails = maLst[0];

                    #endregion
                    #region Set Draft Rental Operation Type

                    rentalData.doTbt_DraftRentalOperationType = this.GetTbt_DraftRentalOperationType(cond.QuotationTargetCodeLong);

                    #endregion
                    #region Set Draft Rental Instrument

                    rentalData.doTbt_DraftRentalInstrument = this.GetTbt_DraftRentalInstrument(cond.QuotationTargetCodeLong);
                    if (rentalData.doTbt_DraftRentalInstrument.Count > 0)
                    {
                        InstrumentMappingList instMappingLst = new InstrumentMappingList();
                        instMappingLst.AddInstrument(rentalData.doTbt_DraftRentalInstrument.ToArray());

                        IInstrumentMasterHandler ihandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                        ihandler.InstrumentListMapping(instMappingLst);
                    }

                    #endregion
                    #region Set Draft Rental Billing Target

                    rentalData.doTbt_DraftRentalBillingTarget = this.GetTbt_DraftRentalBillingTarget(cond.QuotationTargetCodeLong);

                    #endregion
                    #region Set Draft Rental BE detail

                    List<tbt_DraftRentalBEDetails> draftBEDetailLst = this.GetTbt_DraftRentalBEDetails(cond.QuotationTargetCodeLong);
                    if (draftBEDetailLst.Count > 0)
                        rentalData.doTbt_DraftRentalBEDetails = draftBEDetailLst[0];
                    
                    #endregion
                    #region Set Draft Rental Sentryguard

                    List<tbt_DraftRentalSentryGuard> draftSentryGuardLst = this.GetTbt_DraftRentalSentryGuard(cond.QuotationTargetCodeLong);
                    if (draftSentryGuardLst.Count > 0)
                    {
                        rentalData.doTbt_DraftRentalSentryGuard = draftSentryGuardLst[0];
                        rentalData.doTbt_DraftRentalSentryGuardDetails = this.GetTbt_DraftRentalSentryGuardDetails(cond.QuotationTargetCodeLong);
                    }

                    #endregion
                    #region Set Contract Customer

                    ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                    if (CommonUtil.IsNullOrEmpty(rentalData.doTbt_DraftRentalContrat.ContractTargetCustCode) == false)
                    {
                        List<doCustomerWithGroup> custLst = custhandler.GetCustomerWithGroup(rentalData.doTbt_DraftRentalContrat.ContractTargetCustCode);
                        if (custLst.Count > 0)
                            rentalData.doContractCustomer = custLst[0];
                    }
                    if (CommonUtil.IsNullOrEmpty(rentalData.doTbt_DraftRentalContrat.RealCustomerCustCode) == false)
                    {
                        List<doCustomerWithGroup> custLst = custhandler.GetCustomerWithGroup(rentalData.doTbt_DraftRentalContrat.RealCustomerCustCode);
                        if (custLst.Count > 0)
                            rentalData.doRealCustomer = custLst[0];
                    }

                    #endregion
                    #region Set Site

                    ISiteMasterHandler shandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                    List<doSite> siteLst = shandler.GetSite(rentalData.doTbt_DraftRentalContrat.SiteCode, rentalData.doTbt_DraftRentalContrat.RealCustomerCustCode);
                    if (siteLst.Count > 0)
                        rentalData.doSite = siteLst[0];

                    #endregion
                    #region Set Relation type

                    IRentralContractHandler chandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    rentalData.doTbt_RelationType = chandler.GetTbt_RelationType(cond.QuotationTargetCodeLong, rentalData.doTbt_DraftRentalContrat.Alphabet, null);
                    
                    if (rentalData.doTbt_RelationType.Count > 0)
                    {
                        List<tbt_SaleBasic> lst = new List<tbt_SaleBasic>();
                        foreach (tbt_RelationType rm in rentalData.doTbt_RelationType)
                        {
                            if (CommonUtil.IsNullOrEmpty(rm.RelatedContractCode) == false)
                            {
                                lst.Add(new tbt_SaleBasic()
                                {
                                    ContractCode = rm.RelatedContractCode
                                });
                            }
                        }

                        IContractHandler cthandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                        List<doContractHeader> ctLst = cthandler.GetContractHeaderDataByLanguage(lst);
                        if (ctLst.Count > 0)
                        {
                            foreach (tbt_RelationType rm in rentalData.doTbt_RelationType)
                            {
                                foreach (doContractHeader ct in ctLst)
                                {
                                    if (ct.ContractCode == rm.RelatedContractCode)
                                    {
                                        rm.ProductTypeCode = ct.ProductTypeCode;
                                        rm.ProductName = ct.ProductName;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    doGetQuotationDataCondition qcond = new doGetQuotationDataCondition();
                    qcond.QuotationTargetCode = cond.QuotationTargetCodeLong;
                    qcond.Alphabet = rentalData.doTbt_DraftRentalContrat.Alphabet; //cond.Alphabet;
                    qcond.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;
                    qcond.TargetCodeTypeCode = TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE;
                    qcond.ContractFlag = true;

                    IQuotationHandler qhandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                    dsQuotationData qData = qhandler.GetQuotationData(qcond);
                    if (qData != null)
                    {
                        if (qData.dtTbt_QuotationTarget.UpdateDate != null)
                            rentalData.LastUpdateDateQuotationData = qData.dtTbt_QuotationTarget.UpdateDate.Value;
                    }
                }
                else if (mode == doDraftRentalContractData.RENTAL_CONTRACT_MODE.OTHER)
                {
                    rentalData = new doDraftRentalContractData();

                    #region Set Draft Rental Contract

                    List<tbt_DraftRentalContract> contractLst = this.GetTbt_DraftRentalContract(cond.QuotationTargetCodeLong);
                    if (contractLst.Count > 0)
                        rentalData.doTbt_DraftRentalContrat = contractLst[0];
                    else
                        return null;

                    #endregion
                    #region Check Authority

                    bool hasAuthority = false;
                    List<OfficeDataDo> officeLst = CommonUtil.dsTransData.dtOfficeData;
                    if (rentalData.doTbt_DraftRentalContrat != null
                        && officeLst.Count > 0)
                    {
                        foreach (OfficeDataDo office in officeLst)
                        {
                            if (office.OfficeCode == rentalData.doTbt_DraftRentalContrat.QuotationOfficeCode
                                || office.OfficeCode == rentalData.doTbt_DraftRentalContrat.OperationOfficeCode)
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

                    if (mode == doDraftRentalContractData.RENTAL_CONTRACT_MODE.DRAFT
                        && rentalData.doTbt_DraftRentalContrat.DraftRentalContractStatus != ApprovalStatus.C_APPROVE_STATUS_RETURNED)
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3068, cond.QuotationTargetCode);

                    #endregion

                    #region Set Draft Rental E-mail

                    rentalData.doTbt_DraftRentalEmail = this.GetTbt_DraftRentalEmail(cond.QuotationTargetCodeLong);
                    if (rentalData.doTbt_DraftRentalEmail != null)
                    {
                        IEmployeeMasterHandler empHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                        
                        List<tbm_Employee> emps = new List<tbm_Employee>();
                        foreach (tbt_DraftRentalEmail email in rentalData.doTbt_DraftRentalEmail)
                        {
                            emps.Add(new tbm_Employee()
                            {
                                EmpNo = email.ToEmpNo
                            });
                        }
                        List<tbm_Employee> empList = empHandler.GetEmployeeList(emps);
                        if (empList.Count > 0)
                        {
                            foreach (tbt_DraftRentalEmail email in rentalData.doTbt_DraftRentalEmail)
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
                    #region Set Draft Rental Maintenance Details

                    List<tbt_DraftRentalMaintenanceDetails> maLst = this.GetTbt_DraftRentalMaintenanceDetails(cond.QuotationTargetCodeLong);
                    if (maLst.Count > 0)
                        rentalData.doTbt_DraftRentalMaintenanceDetails = maLst[0];

                    #endregion
                    #region Set Draft Rental Operation Type

                    rentalData.doTbt_DraftRentalOperationType = this.GetTbt_DraftRentalOperationType(cond.QuotationTargetCodeLong);

                    #endregion
                    #region Set Draft Rental Instrument

                    rentalData.doTbt_DraftRentalInstrument = this.GetTbt_DraftRentalInstrument(cond.QuotationTargetCodeLong);
                    if (rentalData.doTbt_DraftRentalInstrument.Count > 0)
                    {
                        InstrumentMappingList instMappingLst = new InstrumentMappingList();
                        instMappingLst.AddInstrument(rentalData.doTbt_DraftRentalInstrument.ToArray());

                        IInstrumentMasterHandler ihandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                        ihandler.InstrumentListMapping(instMappingLst);
                    }

                    #endregion
                    #region Set Draft Rental Billing Target

                    rentalData.doTbt_DraftRentalBillingTarget = this.GetTbt_DraftRentalBillingTarget(cond.QuotationTargetCodeLong);

                    #endregion
                    #region Set Draft Rental BE detail

                    List<tbt_DraftRentalBEDetails> draftBEDetailLst = this.GetTbt_DraftRentalBEDetails(cond.QuotationTargetCodeLong);
                    if (draftBEDetailLst.Count > 0)
                        rentalData.doTbt_DraftRentalBEDetails = draftBEDetailLst[0];
                    
                    #endregion
                    #region Set Draft Rental Sentryguard

                    List<tbt_DraftRentalSentryGuard> draftSentryGuardLst = this.GetTbt_DraftRentalSentryGuard(cond.QuotationTargetCodeLong);
                    if (draftSentryGuardLst.Count > 0)
                    {
                        rentalData.doTbt_DraftRentalSentryGuard = draftSentryGuardLst[0];
                        rentalData.doTbt_DraftRentalSentryGuardDetails = this.GetTbt_DraftRentalSentryGuardDetails(cond.QuotationTargetCodeLong);
                    }

                    #endregion
                    #region Set Contract Customer

                    ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                    if (CommonUtil.IsNullOrEmpty(rentalData.doTbt_DraftRentalContrat.ContractTargetCustCode) == false)
                    {
                        List<doCustomerWithGroup> custLst = custhandler.GetCustomerWithGroup(rentalData.doTbt_DraftRentalContrat.ContractTargetCustCode);
                        if (custLst.Count > 0)
                            rentalData.doContractCustomer = custLst[0];
                    }
                    if (CommonUtil.IsNullOrEmpty(rentalData.doTbt_DraftRentalContrat.RealCustomerCustCode) == false)
                    {
                        List<doCustomerWithGroup> custLst = custhandler.GetCustomerWithGroup(rentalData.doTbt_DraftRentalContrat.RealCustomerCustCode);
                        if (custLst.Count > 0)
                            rentalData.doRealCustomer = custLst[0];
                    }

                    #endregion
                    #region Set Site

                    ISiteMasterHandler shandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                    List<doSite> siteLst = shandler.GetSite(rentalData.doTbt_DraftRentalContrat.SiteCode, rentalData.doTbt_DraftRentalContrat.RealCustomerCustCode);
                    if (siteLst.Count > 0)
                        rentalData.doSite = siteLst[0];

                    #endregion
                    #region Set Relation type

                    IRentralContractHandler chandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    rentalData.doTbt_RelationType = chandler.GetTbt_RelationType(cond.QuotationTargetCodeLong, rentalData.doTbt_DraftRentalContrat.Alphabet, null);
                    
                    if (rentalData.doTbt_RelationType.Count > 0)
                    {
                        List<tbt_SaleBasic> lst = new List<tbt_SaleBasic>();
                        foreach (tbt_RelationType rm in rentalData.doTbt_RelationType)
                        {
                            if (CommonUtil.IsNullOrEmpty(rm.RelatedContractCode) == false)
                            {
                                lst.Add(new tbt_SaleBasic()
                                {
                                    ContractCode = rm.RelatedContractCode
                                });
                            }
                        }

                        IContractHandler cthandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                        List<doContractHeader> ctLst = cthandler.GetContractHeaderDataByLanguage(lst);
                        if (ctLst.Count > 0)
                        {
                            foreach (tbt_RelationType rm in rentalData.doTbt_RelationType)
                            {
                                foreach (doContractHeader ct in ctLst)
                                {
                                    if (ct.ContractCode == rm.RelatedContractCode)
                                    {
                                        rm.ProductTypeCode = ct.ProductTypeCode;
                                        rm.ProductName = ct.ProductName;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    #endregion
                }


                if (rentalData != null)
                {
                    if (rentalData.doTbt_DraftRentalContrat != null)
                    {
                        #region Set Product Name

                        IProductMasterHandler mhandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                        List<View_tbm_Product> pLst = mhandler.GetTbm_ProductByLanguage(
                                rentalData.doTbt_DraftRentalContrat.ProductCode,
                                rentalData.doTbt_DraftRentalContrat.ProductTypeCode);

                        if (pLst.Count > 0)
                            rentalData.doTbt_DraftRentalContrat.ProductName = pLst[0].ProductName;

                        #endregion
                        #region Set Misc Name

                        MiscTypeMappingList miscLst = new MiscTypeMappingList();
                        miscLst.AddMiscType(rentalData.doTbt_DraftRentalContrat);
                        miscLst.AddMiscType(rentalData.doTbt_DraftRentalInstrument.ToArray());

                        if (rentalData.doTbt_DraftRentalMaintenanceDetails != null)
                            miscLst.AddMiscType(rentalData.doTbt_DraftRentalMaintenanceDetails);

                        ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        chandler.MiscTypeMappingList(miscLst);

                        #endregion
                        #region Set Employee Name

                        EmployeeMappingList empLst = new EmployeeMappingList();
                        empLst.AddEmployee(rentalData.doTbt_DraftRentalContrat);

                        IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                        emphandler.EmployeeListMapping(empLst);

                        #endregion
                    }
                }

                return rentalData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get draft rental contract information 
        /// </summary>
        /// <param name="strQuotationTargetCode"></param>
        /// <returns></returns>
        public List<doDraftRentalContractInformation> GetDraftRentalContractInformationData(string strQuotationTargetCode)
        {
            try
            {
                List<doDraftRentalContractInformation> doDraftRentalList = base.GetDraftRentalContractInformation(strQuotationTargetCode);
                if (doDraftRentalList == null)
                {
                    doDraftRentalList = new List<doDraftRentalContractInformation>();
                }
                else
                {
                    CommonUtil.MappingObjectLanguage<doDraftRentalContractInformation>(doDraftRentalList);
                }

                return doDraftRentalList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        #region Insert Data
        /// <summary>
        /// To create draft rental contract data
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        public bool CreateDraftRentalContractData(doDraftRentalContractData draft)
        {
            try
            {
                InsertTbt_DraftRentalContract(draft.doTbt_DraftRentalContrat);
                InsertTbt_DraftRentalBillingTarget(draft.doTbt_DraftRentalBillingTarget);
                InsertTbt_DraftRentalEmail(draft.doTbt_DraftRentalEmail);
                InsertTbt_DraftRentalOperationType(draft.doTbt_DraftRentalOperationType);

                ICommonContractHandler chandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                chandler.InsertTbt_RelationType(draft.doTbt_RelationType);

                //if (draft.doTbt_DraftRentalContrat.ProductTypeCode != ProductType.C_PROD_TYPE_ONLINE)
                //{
                    InsertTbt_DraftRentalInstrument(draft.doTbt_DraftRentalInstrument);
                //}

                if (draft.doTbt_DraftRentalContrat.ProductTypeCode == ProductType.C_PROD_TYPE_BE)
                {
                    InsertTbt_DraftRentalBEDetails(draft.doTbt_DraftRentalBEDetails);
                }
                else if (draft.doTbt_DraftRentalContrat.ProductTypeCode == ProductType.C_PROD_TYPE_SG)
                {
                    InsertTbt_DraftRentalSentryGuard(draft.doTbt_DraftRentalSentryGuard);
                    InsertTbt_DraftRentalSentryGuardDetails(draft.doTbt_DraftRentalSentryGuardDetails);
                }
                else if (draft.doTbt_DraftRentalContrat.ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                {
                    InsertTbt_DraftRentalMaintenanceDetails(draft.doTbt_DraftRentalMaintenanceDetails);
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert draft rental contract
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        private int InsertTbt_DraftRentalContract(tbt_DraftRentalContract draft)
        {
            try
            {
                List<tbt_DraftRentalContract> draftLst = new List<tbt_DraftRentalContract>();
                if (draft != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    draft.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                    draft.CreateBy = dsTrans.dtUserData.EmpNo;
                    draft.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                    draft.UpdateBy = dsTrans.dtUserData.EmpNo;

                    draftLst.Add(draft);
                }

                List<tbt_DraftRentalContract> res = this.InsertTbt_DraftRentalContract(
                    CommonUtil.ConvertToXml_Store<tbt_DraftRentalContract>(draftLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
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
        /// Insert draft rental BE detail
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        private int InsertTbt_DraftRentalBEDetails(tbt_DraftRentalBEDetails draft)
        {
            try
            {
                List<tbt_DraftRentalBEDetails> draftLst = new List<tbt_DraftRentalBEDetails>();
                if (draft != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    draft.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                    draft.CreateBy = dsTrans.dtUserData.EmpNo;
                    draft.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                    draft.UpdateBy = dsTrans.dtUserData.EmpNo;

                    draftLst.Add(draft);
                }

                List<tbt_DraftRentalBEDetails> res = this.InsertTbt_DraftRentalBEDetails(
                    CommonUtil.ConvertToXml_Store<tbt_DraftRentalBEDetails>(draftLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_DRF_RNT_BE,
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
        /// Insert draft rental billing target
        /// </summary>
        /// <param name="draftLst"></param>
        /// <returns></returns>
        private int InsertTbt_DraftRentalBillingTarget(List<tbt_DraftRentalBillingTarget> draftLst)
        {
            try
            {
                if (draftLst != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_DraftRentalBillingTarget draft in draftLst)
                    {
                        draft.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        draft.CreateBy = dsTrans.dtUserData.EmpNo;
                        draft.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        draft.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_DraftRentalBillingTarget> res = this.InsertTbt_DraftRentalBillingTarget(
                    CommonUtil.ConvertToXml_Store<tbt_DraftRentalBillingTarget>(draftLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_DRF_RNT_BILLING,
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
        /// Insert draft rental instrument
        /// </summary>
        /// <param name="draftLst"></param>
        /// <returns></returns>
        private int InsertTbt_DraftRentalInstrument(List<tbt_DraftRentalInstrument> draftLst)
        {
            try
            {
                if (draftLst != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_DraftRentalInstrument draft in draftLst)
                    {
                        draft.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        draft.CreateBy = dsTrans.dtUserData.EmpNo;
                        draft.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        draft.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_DraftRentalInstrument> res = this.InsertTbt_DraftRentalInstrument(
                    CommonUtil.ConvertToXml_Store<tbt_DraftRentalInstrument>(draftLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_DRF_RNT_INST,
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
        /// Insert draft rental maintenance detail
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        private int InsertTbt_DraftRentalMaintenanceDetails(tbt_DraftRentalMaintenanceDetails draft)
        {
            try
            {
                List<tbt_DraftRentalMaintenanceDetails> draftLst = new List<tbt_DraftRentalMaintenanceDetails>();
                if (draft != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    draft.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                    draft.CreateBy = dsTrans.dtUserData.EmpNo;
                    draft.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                    draft.UpdateBy = dsTrans.dtUserData.EmpNo;

                    draftLst.Add(draft);
                }

                List<tbt_DraftRentalMaintenanceDetails> res = this.InsertTbt_DraftRentalMaintenanceDetails(
                    CommonUtil.ConvertToXml_Store<tbt_DraftRentalMaintenanceDetails>(draftLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_DRF_RNT_MA,
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
        /// Insert draft rental operation type
        /// </summary>
        /// <param name="draftLst"></param>
        /// <returns></returns>
        private int InsertTbt_DraftRentalOperationType(List<tbt_DraftRentalOperationType> draftLst)
        {
            try
            {
                if (draftLst != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_DraftRentalOperationType draft in draftLst)
                    {
                        draft.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        draft.CreateBy = dsTrans.dtUserData.EmpNo;
                        draft.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        draft.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_DraftRentalOperationType> res = this.InsertTbt_DraftRentalOperationType(
                    CommonUtil.ConvertToXml_Store<tbt_DraftRentalOperationType>(draftLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_DRF_RNT_OPER_TYPE,
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
        /// Insert draft rental sentry guard
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        private int InsertTbt_DraftRentalSentryGuard(tbt_DraftRentalSentryGuard draft)
        {
            try
            {
                List<tbt_DraftRentalSentryGuard> draftLst = new List<tbt_DraftRentalSentryGuard>();
                if (draft != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    draft.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                    draft.CreateBy = dsTrans.dtUserData.EmpNo;
                    draft.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                    draft.UpdateBy = dsTrans.dtUserData.EmpNo;

                    draftLst.Add(draft);
                }

                List<tbt_DraftRentalSentryGuard> res = this.InsertTbt_DraftRentalSentryGuard(
                    CommonUtil.ConvertToXml_Store<tbt_DraftRentalSentryGuard>(draftLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_DRF_RNT_SG,
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
        /// Insert draft rental sentry guard detail
        /// </summary>
        /// <param name="draftLst"></param>
        /// <returns></returns>
        private int InsertTbt_DraftRentalSentryGuardDetails(List<tbt_DraftRentalSentryGuardDetails> draftLst)
        {
            try
            {
                if (draftLst != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_DraftRentalSentryGuardDetails draft in draftLst)
                    {
                        draft.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        draft.CreateBy = dsTrans.dtUserData.EmpNo;
                        draft.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        draft.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_DraftRentalSentryGuardDetails> res = this.InsertTbt_DraftRentalSentryGuardDetails(
                    CommonUtil.ConvertToXml_Store<tbt_DraftRentalSentryGuardDetails>(draftLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_DRF_RNT_SG_DET,
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
        /// Insert draft rental email
        /// </summary>
        /// <param name="draftLst"></param>
        /// <returns></returns>
        private int InsertTbt_DraftRentalEmail(List<tbt_DraftRentalEmail> draftLst)
        {
            try
            {
                if (draftLst != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_DraftRentalEmail draft in draftLst)
                    {
                        draft.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        draft.CreateBy = dsTrans.dtUserData.EmpNo;
                        draft.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        draft.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_DraftRentalEmail> res = this.InsertTbt_DraftRentalEmail(
                    CommonUtil.ConvertToXml_Store<tbt_DraftRentalEmail>(draftLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_DRF_RNT_EMAIL,
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
        /// To edit draft rental contract data
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        public bool EditDraftRentalContractData(doDraftRentalContractData draft)
        {
            try
            {
                UpdateTbt_DraftRentalContract(draft.doTbt_DraftRentalContrat);
                UpdateTbt_DraftRentalBillingTarget(draft.doTbt_DraftRentalContrat.QuotationTargetCode, draft.doTbt_DraftRentalBillingTarget);
                UpdateTbt_DraftRentalEmail(draft.doTbt_DraftRentalContrat.QuotationTargetCode, draft.doTbt_DraftRentalEmail);
                UpdateTbt_DraftRentalOperationType(draft.doTbt_DraftRentalContrat.QuotationTargetCode, draft.doTbt_DraftRentalOperationType);

                ICommonContractHandler chandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                chandler.UpdateTbt_RelationType(draft.doTbt_DraftRentalContrat.QuotationTargetCode, draft.doTbt_RelationType);

                if (draft.doTbt_DraftRentalContrat.ProductTypeCode != ProductType.C_PROD_TYPE_ONLINE)
                {
                    UpdateTbt_DraftRentalInstrument(draft.doTbt_DraftRentalContrat.QuotationTargetCode, draft.doTbt_DraftRentalInstrument);
                }

                if (draft.doTbt_DraftRentalContrat.ProductTypeCode == ProductType.C_PROD_TYPE_BE)
                {
                    UpdateTbt_DraftRentalBEDetails(draft.doTbt_DraftRentalBEDetails);
                }
                else if (draft.doTbt_DraftRentalContrat.ProductTypeCode == ProductType.C_PROD_TYPE_SG)
                {
                    UpdateTbt_DraftRentalSentryGuard(draft.doTbt_DraftRentalSentryGuard);
                    UpdateTbt_DraftRentalSentryGuardDetails(draft.doTbt_DraftRentalContrat.QuotationTargetCode, draft.doTbt_DraftRentalSentryGuardDetails);
                }
                else if (draft.doTbt_DraftRentalContrat.ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                {
                    UpdateTbt_DraftRentalMaintenanceDetails(draft.doTbt_DraftRentalMaintenanceDetails);
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To edit draft rental contract data for CTS010
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        public bool EditDraftRentalContractDataForCTS010(doDraftRentalContractData draft)
        {
            try
            {
                if (draft == null)
                    return false;

                DateTime updateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                string updateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                #region Draft rental contract

                if (draft.doTbt_DraftRentalContrat != null)
                {
                    draft.doTbt_DraftRentalContrat.CreateDate = updateDate;
                    draft.doTbt_DraftRentalContrat.CreateBy = updateBy;
                    draft.doTbt_DraftRentalContrat.UpdateDate = updateDate;
                    draft.doTbt_DraftRentalContrat.UpdateBy = updateBy;
                }
                string xml_DraftRentalContract = CommonUtil.ConvertToXml_Store<tbt_DraftRentalContract>(new List<tbt_DraftRentalContract>(){ draft.doTbt_DraftRentalContrat });

                #endregion
                #region Draft rental billing target

                if (draft.doTbt_DraftRentalBillingTarget != null)
                {
                    foreach (tbt_DraftRentalBillingTarget o in draft.doTbt_DraftRentalBillingTarget)
                    {
                        o.CreateDate = updateDate;
                        o.CreateBy = updateBy;
                        o.UpdateDate = updateDate;
                        o.UpdateBy = updateBy;
                    }
                }
                string xml_DraftRentalBillingTarget = CommonUtil.ConvertToXml_Store<tbt_DraftRentalBillingTarget>(draft.doTbt_DraftRentalBillingTarget);

                #endregion
                #region Draft rental email

                if (draft.doTbt_DraftRentalEmail != null)
                {
                    foreach (tbt_DraftRentalEmail o in draft.doTbt_DraftRentalEmail)
                    {
                        o.CreateDate = updateDate;
                        o.CreateBy = updateBy;
                        o.UpdateDate = updateDate;
                        o.UpdateBy = updateBy;
                    }
                }
                string xml_DraftRentalEmail = CommonUtil.ConvertToXml_Store<tbt_DraftRentalEmail>(draft.doTbt_DraftRentalEmail);

                #endregion
                #region Draft rental operation type

                if (draft.doTbt_DraftRentalOperationType != null)
                {
                    foreach (tbt_DraftRentalOperationType o in draft.doTbt_DraftRentalOperationType)
                    {
                        o.CreateDate = updateDate;
                        o.CreateBy = updateBy;
                        o.UpdateDate = updateDate;
                        o.UpdateBy = updateBy;
                    }
                }
                string xml_DraftRentalOperationType = CommonUtil.ConvertToXml_Store<tbt_DraftRentalOperationType>(draft.doTbt_DraftRentalOperationType);

                #endregion
                #region Draft relation type

                if (draft.doTbt_RelationType != null)
                {
                    foreach (tbt_RelationType o in draft.doTbt_RelationType)
                    {
                        o.CreateDate = updateDate;
                        o.CreateBy = updateBy;
                        o.UpdateDate = updateDate;
                        o.UpdateBy = updateBy;
                    }
                }
                string xml_RelationType = CommonUtil.ConvertToXml_Store<tbt_RelationType>(draft.doTbt_RelationType);

                #endregion
                #region Draft draft instrument

                if (draft.doTbt_DraftRentalInstrument != null)
                {
                    foreach (tbt_DraftRentalInstrument o in draft.doTbt_DraftRentalInstrument)
                    {
                        o.CreateDate = updateDate;
                        o.CreateBy = updateBy;
                        o.UpdateDate = updateDate;
                        o.UpdateBy = updateBy;
                    }
                }
                string xml_DraftRentalInstrument = CommonUtil.ConvertToXml_Store<tbt_DraftRentalInstrument>(draft.doTbt_DraftRentalInstrument);

                #endregion
                #region Draft rental beat guard detail

                List<tbt_DraftRentalBEDetails> beLst = null;
                if (draft.doTbt_DraftRentalBEDetails != null)
                {
                    draft.doTbt_DraftRentalBEDetails.CreateDate = updateDate;
                    draft.doTbt_DraftRentalBEDetails.CreateBy = updateBy;
                    draft.doTbt_DraftRentalBEDetails.UpdateDate = updateDate;
                    draft.doTbt_DraftRentalBEDetails.UpdateBy = updateBy;

                    beLst = new List<tbt_DraftRentalBEDetails>() { draft.doTbt_DraftRentalBEDetails };
                }
                string xml_DraftRentalBEDetails = CommonUtil.ConvertToXml_Store<tbt_DraftRentalBEDetails>(beLst);

                #endregion
                #region Draft rental sentry guard

                List<tbt_DraftRentalSentryGuard> sgLst = null;
                if (draft.doTbt_DraftRentalSentryGuard != null)
                {
                    draft.doTbt_DraftRentalSentryGuard.CreateDate = updateDate;
                    draft.doTbt_DraftRentalSentryGuard.CreateBy = updateBy;
                    draft.doTbt_DraftRentalSentryGuard.UpdateDate = updateDate;
                    draft.doTbt_DraftRentalSentryGuard.UpdateBy = updateBy;

                    sgLst = new List<tbt_DraftRentalSentryGuard>() { draft.doTbt_DraftRentalSentryGuard };
                }
                string xml_DraftRentalSentryGuard = CommonUtil.ConvertToXml_Store<tbt_DraftRentalSentryGuard>(sgLst);

                #endregion
                #region Draft rental sentry guard details

                if (draft.doTbt_DraftRentalSentryGuardDetails != null)
                {
                    foreach (tbt_DraftRentalSentryGuardDetails o in draft.doTbt_DraftRentalSentryGuardDetails)
                    {
                        o.CreateDate = updateDate;
                        o.CreateBy = updateBy;
                        o.UpdateDate = updateDate;
                        o.UpdateBy = updateBy;
                    }
                }
                string xml_DraftRentalSentryGuardDetails = CommonUtil.ConvertToXml_Store<tbt_DraftRentalSentryGuardDetails>(draft.doTbt_DraftRentalSentryGuardDetails);

                #endregion
                #region Draft rental maintenance details

                List<tbt_DraftRentalMaintenanceDetails> maLst = null;
                if (draft.doTbt_DraftRentalMaintenanceDetails != null)
                {
                    draft.doTbt_DraftRentalMaintenanceDetails.CreateDate = updateDate;
                    draft.doTbt_DraftRentalMaintenanceDetails.CreateBy = updateBy;
                    draft.doTbt_DraftRentalMaintenanceDetails.UpdateDate = updateDate;
                    draft.doTbt_DraftRentalMaintenanceDetails.UpdateBy = updateBy;

                    maLst = new List<tbt_DraftRentalMaintenanceDetails>() { draft.doTbt_DraftRentalMaintenanceDetails };
                }
                string xml_DraftRentalMaintenanceDetails = CommonUtil.ConvertToXml_Store<tbt_DraftRentalMaintenanceDetails>(maLst);

                #endregion

                this.EditDraftRentalContract(
                    draft.doTbt_DraftRentalContrat.QuotationTargetCode,
                    draft.doTbt_DraftRentalContrat.ProductTypeCode,
                    CommonUtil.dsTransData.dtOperationData.GUID,
                    CommonUtil.dsTransData.dtTransHeader.ScreenID,
                    updateDate,
                    updateBy,
                    ProductType.C_PROD_TYPE_ONLINE,
                    ProductType.C_PROD_TYPE_BE,
                    ProductType.C_PROD_TYPE_SG,
                    ProductType.C_PROD_TYPE_MA,
                    xml_DraftRentalContract,
                    xml_DraftRentalBillingTarget,
                    xml_DraftRentalEmail,
                    xml_DraftRentalOperationType,
                    xml_RelationType,
                    xml_DraftRentalInstrument,
                    xml_DraftRentalBEDetails,
                    xml_DraftRentalSentryGuard,
                    xml_DraftRentalSentryGuardDetails,
                    xml_DraftRentalMaintenanceDetails);

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update draft rental contract data
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        public int UpdateTbt_DraftRentalContract(tbt_DraftRentalContract draft)
        {
            try
            {
                List<tbt_DraftRentalContract> draftLst = new List<tbt_DraftRentalContract>();
                if (draft != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    draft.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                    draft.UpdateBy = dsTrans.dtUserData.EmpNo;

                    draftLst.Add(draft);
                }

                List<tbt_DraftRentalContract> res = this.UpdateTbt_DraftRentalContract(
                    CommonUtil.ConvertToXml_Store<tbt_DraftRentalContract>(draftLst));

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
        /// Update draft rental BE detail
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        private int UpdateTbt_DraftRentalBEDetails(tbt_DraftRentalBEDetails draft)
        {
            try
            {
                List<tbt_DraftRentalBEDetails> draftLst = new List<tbt_DraftRentalBEDetails>();
                if (draft != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    draft.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                    draft.UpdateBy = dsTrans.dtUserData.EmpNo;

                    draftLst.Add(draft);
                }

                List<tbt_DraftRentalBEDetails> res = this.UpdateTbt_DraftRentalBEDetails(
                    CommonUtil.ConvertToXml_Store<tbt_DraftRentalBEDetails>(draftLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_DRF_RNT_BE,
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
        /// Update draft rental billing target
        /// </summary>
        /// <param name="QuotationTargetCode"></param>
        /// <param name="draftLst"></param>
        /// <returns></returns>
        private int UpdateTbt_DraftRentalBillingTarget(string QuotationTargetCode, List<tbt_DraftRentalBillingTarget> draftLst)
        {
            try
            {
                List<tbt_DraftRentalBillingTarget> res = this.DeleteTbt_DraftRentalBillingTarget(QuotationTargetCode);

                #region Log

                doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Delete,
                        TableName = TableName.C_TBL_NAME_DRF_RNT_BILLING,
                        TableData = CommonUtil.ConvertToXml(res)
                    };

                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return InsertTbt_DraftRentalBillingTarget(draftLst);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update draft rental instrument
        /// </summary>
        /// <param name="QuotationTargetCode"></param>
        /// <param name="draftLst"></param>
        /// <returns></returns>
        private int UpdateTbt_DraftRentalInstrument(string QuotationTargetCode, List<tbt_DraftRentalInstrument> draftLst)
        {
            try
            {
                List<tbt_DraftRentalInstrument> res = this.DeleteTbt_DraftRentalInstrument(QuotationTargetCode);

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Delete,
                    TableName = TableName.C_TBL_NAME_DRF_RNT_INST,
                    TableData = CommonUtil.ConvertToXml(res)
                };

                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return InsertTbt_DraftRentalInstrument(draftLst);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update draft rental maintenance detail
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        private int UpdateTbt_DraftRentalMaintenanceDetails(tbt_DraftRentalMaintenanceDetails draft)
        {
            try
            {
                List<tbt_DraftRentalMaintenanceDetails> draftLst = new List<tbt_DraftRentalMaintenanceDetails>();
                if (draft != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    draft.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                    draft.UpdateBy = dsTrans.dtUserData.EmpNo;

                    draftLst.Add(draft);
                }

                List<tbt_DraftRentalMaintenanceDetails> res = this.UpdateTbt_DraftRentalMaintenanceDetails(
                    CommonUtil.ConvertToXml_Store<tbt_DraftRentalMaintenanceDetails>(draftLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_DRF_RNT_MA,
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
        /// Update draft rental operation type
        /// </summary>
        /// <param name="QuotationTargetCode"></param>
        /// <param name="draftLst"></param>
        /// <returns></returns>
        private int UpdateTbt_DraftRentalOperationType(string QuotationTargetCode, List<tbt_DraftRentalOperationType> draftLst)
        {
            try
            {
                List<tbt_DraftRentalOperationType> res = this.DeleteTbt_DraftRentalOperationType(QuotationTargetCode);

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Delete,
                    TableName = TableName.C_TBL_NAME_DRF_RNT_OPER_TYPE,
                    TableData = CommonUtil.ConvertToXml(res)
                };

                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return InsertTbt_DraftRentalOperationType(draftLst);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update draft rental sentry guard
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        private int UpdateTbt_DraftRentalSentryGuard(tbt_DraftRentalSentryGuard draft)
        {
            try
            {
                List<tbt_DraftRentalSentryGuard> draftLst = new List<tbt_DraftRentalSentryGuard>();
                if (draft != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    draft.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                    draft.UpdateBy = dsTrans.dtUserData.EmpNo;

                    draftLst.Add(draft);
                }

                List<tbt_DraftRentalSentryGuard> res = this.UpdateTbt_DraftRentalSentryGuard(
                    CommonUtil.ConvertToXml_Store<tbt_DraftRentalSentryGuard>(draftLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_DRF_RNT_SG,
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
        /// Update draft rental sentry guard detail
        /// </summary>
        /// <param name="QuotationTargetCode"></param>
        /// <param name="draftLst"></param>
        /// <returns></returns>
        private int UpdateTbt_DraftRentalSentryGuardDetails(string QuotationTargetCode, List<tbt_DraftRentalSentryGuardDetails> draftLst)
        {
            try
            {
                List<tbt_DraftRentalSentryGuardDetails> res = this.DeleteTbt_DraftRentalSentryGuardDetails(QuotationTargetCode);

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Delete,
                    TableName = TableName.C_TBL_NAME_DRF_RNT_SG_DET,
                    TableData = CommonUtil.ConvertToXml(res)
                };

                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return InsertTbt_DraftRentalSentryGuardDetails(draftLst);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update draft rental email
        /// </summary>
        /// <param name="QuotationTargetCode"></param>
        /// <param name="draftLst"></param>
        /// <returns></returns>
        private int UpdateTbt_DraftRentalEmail(string QuotationTargetCode, List<tbt_DraftRentalEmail> draftLst)
        {
            try
            {
                List<tbt_DraftRentalEmail> res = this.DeleteTbt_DraftRentalEmail(QuotationTargetCode);

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Delete,
                    TableName = TableName.C_TBL_NAME_DRF_RNT_EMAIL,
                    TableData = CommonUtil.ConvertToXml(res)
                };

                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return InsertTbt_DraftRentalEmail(draftLst);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
