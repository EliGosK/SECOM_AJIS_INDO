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


namespace SECOM_AJIS.DataEntity.Billing
{
    public partial class ViewBillingHandler : BizBLDataEntities, IViewBillingHandler
    {
        #region Method Override
        public override /// <summary>
            /// Get BillingBasic data for view
            /// </summary>
            /// <param name="strContractCode"></param>
            /// <param name="strBillingOCC"></param>
            /// <param name="strBillingClientCode"></param>
            /// <param name="strBillingTargetCode"></param>
            /// <param name="strBillingCilentname"></param>
            /// <param name="strAddress"></param>
            /// <returns></returns>
        List<dtViewBillingBasic> GetViewBillingBasic(string strContractCode, string strBillingOCC, string strBillingClientCode, string strBillingTargetCode, string strBillingCilentname, string strAddress)
        {
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<dtViewBillingBasic> result = base.GetViewBillingBasic(strContractCode, strBillingOCC, strBillingClientCode, strBillingTargetCode, strBillingCilentname, strAddress);

            // Misc Mapping  
            MiscTypeMappingList miscMapping = new MiscTypeMappingList();
            miscMapping.AddMiscType(result.ToArray());
            handlerCommon.MiscTypeMappingList(miscMapping);

            CommonUtil.MappingObjectLanguage<dtViewBillingBasic>(result);
            return result;
        }

        public override List<dtViewBillingDetailList> GetViewBillingDetailList(string contractCode, string billingOCC)
        {
            try
            {
                List<dtViewBillingDetailList> result = base.GetViewBillingDetailList(contractCode, billingOCC);

                // Misc Mapping  
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                miscMapping.AddMiscType(result.ToArray());
                handlerCommon.MiscTypeMappingList(miscMapping);

                CommonUtil.MappingObjectLanguage<dtViewBillingDetailList>(result);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get billing detail list by target code
        /// </summary>
        /// <param name="billingTargetCode">Billing target code</param>
        /// <returns></returns>
        public override List<dtViewBillingDetailList> GetViewBillingDetailListByTargetCode(string billingTargetCode)
        {
            try
            {
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<dtViewBillingDetailList> result = base.GetViewBillingDetailListByTargetCode(billingTargetCode);

                // Misc Mapping  
                MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                miscMapping.AddMiscType(result.ToArray());
                handlerCommon.MiscTypeMappingList(miscMapping);

                CommonUtil.MappingObjectLanguage<dtViewBillingDetailList>(result);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Get billing detail list of last invoice OCC
        /// </summary>
        /// <param name="invoiceNo">Invoice no</param>
        /// <param name="billingClientCode">Billing client code</param>
        /// <param name="billingTargetCode">Billing target code</param>
        /// <param name="billingCilentname">Billing client name</param>
        /// <param name="address">Address</param>
        /// <returns></returns>
        public override List<dtViewBillingDetailListOfLastInvoiceOCC> GetViewBillingDetailListOfLastInvoiceOCC(string invoiceNo, string billingClientCode, string billingTargetCode, string billingCilentname, string address)
        {
            try
            {
                List<dtViewBillingDetailListOfLastInvoiceOCC> result = base.GetViewBillingDetailListOfLastInvoiceOCC(invoiceNo, billingClientCode, billingTargetCode, billingCilentname, address);

                // Misc Mapping  
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                miscMapping.AddMiscType(result.ToArray());
                handlerCommon.MiscTypeMappingList(miscMapping);
                
                CommonUtil.MappingObjectLanguage<dtViewBillingDetailListOfLastInvoiceOCC>(result);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<dtViewDepositDetailInformation> GetViewDepositDetailInformation(string contractCode, string billingOCC)
        {
            try
            {
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<dtViewDepositDetailInformation> result = base.GetViewDepositDetailInformation(contractCode, billingOCC);

                // Misc Mapping  
                MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                miscMapping.AddMiscType(result.ToArray());
                handlerCommon.MiscTypeMappingList(miscMapping);

                CommonUtil.MappingObjectLanguage<dtViewDepositDetailInformation>(result);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get View Billing Basic List
        /// </summary>
        /// <param name="billingClientCode">Billing client code</param>
        /// <param name="billingTargetCode">Billing target code</param>
        /// <param name="billingClientName">Billing client name</param>
        /// <param name="address"></param>
        /// <returns></returns>
        public override List<dtViewBillingBasicList> GetViewBillingBasicList(string billingClientCode, string billingTargetCode, string billingClientName, string address, string invoiceNo, string taxIDNo)
        {
            try
            {
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<dtViewBillingBasicList> result = base.GetViewBillingBasicList(billingClientCode, billingTargetCode, billingClientName, address, invoiceNo, taxIDNo);

                CommonUtil.MappingObjectLanguage<dtViewBillingBasicList>(result);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get View Billing Invoice List Of Last Invoice Occ
        /// </summary>
        /// <param name="billingClientCode">Billing client code</param>
        /// <param name="billingTargetCode">Billing target code</param>
        /// <param name="billingCilentname">Billing client code</param>
        /// <param name="address">Address</param>
        /// <param name="invoiceNo">Invoice no.</param>
        /// <returns></returns>
        public override List<dtViewBillingInvoiceListOfLastInvoiceOcc> GetViewBillingInvoiceListOfLastInvoiceOcc(string billingClientCode, string billingTargetCode, string billingCilentname, string address, string invoiceNo, string taxIDNo)
        {
            try
            {
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<dtViewBillingInvoiceListOfLastInvoiceOcc> result = base.GetViewBillingInvoiceListOfLastInvoiceOcc(billingClientCode, billingTargetCode, billingCilentname, address, invoiceNo, taxIDNo);

                // Misc Mapping  
                MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                miscMapping.AddMiscType(result.ToArray());
                handlerCommon.MiscTypeMappingList(miscMapping);

                CommonUtil.MappingObjectLanguage<dtViewBillingInvoiceListOfLastInvoiceOcc>(result);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Get View Billing Target ist
        /// </summary>
        /// <param name="billingClientCode">Billing client code</param>
        /// <param name="billingTargetCode">Billing target code</param>
        /// <param name="billingClientName">Billing client name</param>
        /// <param name="address"></param>
        /// <returns></returns>
        public override List<doBillingTargetList> GetViewBillingTargetList(string billingClientCode, string billingTargetCode, string billingClientName, string address, string invoiceNo, string taxIDNo)
        {
            try
            {
                //ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doBillingTargetList> result = base.GetViewBillingTargetList(billingClientCode, billingTargetCode, billingClientName, address, invoiceNo, taxIDNo);

                CommonUtil.MappingObjectLanguage<doBillingTargetList>(result);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Get view billing occ list
        /// </summary>
        /// <param name="strContractCode">Contract code</param>
        /// <returns></returns>
        public override List<dtViewBillingOccList> GetViewBillingOccList(string strContractCode)
        {
            try
            {
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<dtViewBillingOccList> result = base.GetViewBillingOccList(strContractCode);

                // Misc Mapping  
                MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                miscMapping.AddMiscType(result.ToArray());
                handlerCommon.MiscTypeMappingList(miscMapping);

                CommonUtil.MappingObjectLanguage<dtViewBillingOccList>(result);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get view billing occ list for deposit free
        /// </summary>
        /// <param name="strContractCode">Contract code</param>
        /// <returns></returns>
        public override List<dtViewBillingOccList> GetViewBillingOCCListForDepositFree(string strContractCode)
        {
            try
            {
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<dtViewBillingOccList> result = base.GetViewBillingOCCListForDepositFree(strContractCode);

                // Misc Mapping  
                MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                miscMapping.AddMiscType(result.ToArray());
                handlerCommon.MiscTypeMappingList(miscMapping);

                CommonUtil.MappingObjectLanguage<dtViewBillingOccList>(result);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get View Billing Target Data ForSearch
        /// </summary>
        /// <param name="cond">Billing target data for search</param>
        /// <returns></returns>
        public List<dtBillingTargetData> GetViewBillingTargetDataForSearch(doBillingTargetDataSearchCondition cond)
        {
            try
            {
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<dtBillingTargetData> lst = base.GetViewBillingTargetDataForSearch(
                                                           cond.CMS470_BillingClientCode,
                                                           cond.CMS470_BillingOffice,
                                                           cond.CMS470_BillingClientName,
                                                           cond.CMS470_CustomerTypeCode,
                                                           cond.CMS470_CompanyTypeCode,
                                                           cond.CMS470_RegionCode,
                                                           cond.CMS470_BusinessTypeCode,
                                                           cond.CMS470_Address,
                                                           cond.CMS470_TelephoneNo
                                                           );

                // Misc Mapping  
                MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                miscMapping.AddMiscType(lst.ToArray());
                handlerCommon.MiscTypeMappingList(miscMapping);

                CommonUtil.MappingObjectLanguage<dtBillingTargetData>(lst);
                return lst;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

    }
}
