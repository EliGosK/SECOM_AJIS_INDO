using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    public partial class doCSVInvDepreciationAcc
    {
        public enum eSeqNo
        {
            InstrumentCode = 1,
            InstrumentName,
            DepreciationPeriod,
            StartDepreciationMonth,
            EndDepreciationMonth,
            AccquisitionCost,
            RemainValueContact,
            RemainValueRevenue,
            DepreciationCostOfthismonthContact,
            DepreciationCostOfthismonthRevenue,
            AccumulateDepreciationAmount,
            AccumulateDepreciationAmountRevenue,
            Quantity,
            TotalAccquisitionCost,
            TotalRemainValueContract,
            TotalRemainValueRevenue,
            TotalDepreciationCostOfthismonthContact,
            TotalDepreciationCostOfthismonthRevenue,
            TotalAccumulateDepreciationAmount,
            TotalAccumulateDepreciationAmountRevenue, 
            StartType,
        }

        [CSVMapping(HeaderName = "InstrumentCode", SequenceNo = (int)eSeqNo.InstrumentCode, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Formula)]
        public string InstrumentCodeCsv
        {
            get { return this.InstrumentCode; }
        }

        [CSVMapping(HeaderName = "InstrumentName", SequenceNo = (int)eSeqNo.InstrumentName, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Formula)]
        public string InstrumentNameCsv
        {
            get { return this.InstrumentName; }
        }

        [CSVMapping(HeaderName = "DepreciationPeriod", SequenceNo = (int)eSeqNo.DepreciationPeriod, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public Nullable<int> DepreciationPeriodCsv
        {
            get { return this.DepreciationPeriod; }
        }

        [CSVMapping(HeaderName = "StartDepreciationMonth", SequenceNo = (int)eSeqNo.StartDepreciationMonth, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public string StartDepreciationMonthCsv
        {
            get { return this.StartDepreciationMonth; }
        }

        [CSVMapping(HeaderName = "EndDepreciationMonth", SequenceNo = (int)eSeqNo.EndDepreciationMonth, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public string EndDepreciationMonthCsv
        {
            get { return this.EndDepreciationMonth; }
        }

        [CSVMapping(HeaderName = "AccquisitionCost", SequenceNo = (int)eSeqNo.AccquisitionCost, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public Nullable<decimal> AccquisitionCostCsv
        {
            get { return this.AccquisitionCost; }
        }

        [CSVMapping(HeaderName = "RemainValueContact", SequenceNo = (int)eSeqNo.RemainValueContact, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public Nullable<decimal> RemainValueContactCsv
        {
            get { return this.RemainValueContact; }
        }

        [CSVMapping(HeaderName = "RemainValueRevenue", SequenceNo = (int)eSeqNo.RemainValueRevenue, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public Nullable<decimal> RemainValueRevenueCsv
        {
            get { return this.RemainValueRevenue; }
        }

        [CSVMapping(HeaderName = "DepreciationCostOfthismonthContact", SequenceNo = (int)eSeqNo.DepreciationCostOfthismonthContact, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public Nullable<decimal> DepreciationCostOfthismonthContactCsv
        {
            get { return this.DepreciationCostOfthismonthContact; }
        }

        [CSVMapping(HeaderName = "DepreciationCostOfthismonthRevenue", SequenceNo = (int)eSeqNo.DepreciationCostOfthismonthRevenue, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public Nullable<decimal> DepreciationCostOfthismonthRevenueCsv
        {
            get { return this.DepreciationCostOfthismonthRevenue; }
        }

        [CSVMapping(HeaderName = "AccumulateDepreciationAmount", SequenceNo = (int)eSeqNo.AccumulateDepreciationAmount, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public Nullable<decimal> AccumulateDepreciationAmountCsv
        {
            get { return this.AccumulateDepreciationAmount; }
        }

        [CSVMapping(HeaderName = "AccumulateDepreciationAmountRevenue", SequenceNo = (int)eSeqNo.AccumulateDepreciationAmountRevenue, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public Nullable<decimal> AccumulateDepreciationAmountRevenueCsv
        {
            get { return this.AccumulateDepreciationAmountRevenue; }
        }

        [CSVMapping(HeaderName = "Quantity", SequenceNo = (int)eSeqNo.Quantity, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public Nullable<decimal> QtyCsv
        {
            get { return this.Qty; }
        }

        [CSVMapping(HeaderName = "TotalAccquisitionCost", SequenceNo = (int)eSeqNo.TotalAccquisitionCost, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public Nullable<decimal> TotalAccquisitionCostCsv
        {
            get { return this.TotalAccquisitionCost; }
        }

        [CSVMapping(HeaderName = "TotalRemainValueContract", SequenceNo = (int)eSeqNo.TotalRemainValueContract, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public Nullable<decimal> TotalRemainValueContractCsv
        {
            get { return this.TotalRemainValueContract; }
        }

        [CSVMapping(HeaderName = "TotalRemainValueRevenue", SequenceNo = (int)eSeqNo.TotalRemainValueRevenue, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public Nullable<decimal> TotalRemainValueRevenueCsv
        {
            get { return this.TotalRemainValueRevenue; }
        }

        [CSVMapping(HeaderName = "TotalDepreciationCostOfthismonthContact", SequenceNo = (int)eSeqNo.TotalDepreciationCostOfthismonthContact, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public Nullable<decimal> TotalDepreciationCostOfthismonthContactCsv
        {
            get { return this.TotalDepreciationCostOfthismonthContact; }
        }

        [CSVMapping(HeaderName = "TotalDepreciationCostOfthismonthRevenue", SequenceNo = (int)eSeqNo.TotalDepreciationCostOfthismonthRevenue, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public Nullable<decimal> TotalDepreciationCostOfthismonthRevenueCsv
        {
            get { return this.TotalDepreciationCostOfthismonthRevenue; }
        }

        [CSVMapping(HeaderName = "TotalAccumulateDepreciationAmount", SequenceNo = (int)eSeqNo.TotalAccumulateDepreciationAmount, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public Nullable<decimal> TotalAccumulateDepreciationAmountCsv
        {
            get { return this.TotalAccumulateDepreciationAmount; }
        }

        [CSVMapping(HeaderName = "TotalAccumulateDepreciationAmountRevenue", SequenceNo = (int)eSeqNo.TotalAccumulateDepreciationAmountRevenue, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public Nullable<decimal> TotalAccumulateDepreciationAmountRevenueCsv
        {
            get { return this.TotalAccumulateDepreciationAmountRevenue; }
        }

        [CSVMapping(HeaderName = "StartType", SequenceNo = (int)eSeqNo.StartType, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Formula)]
        public string StarttypeCsv
        {
            get { return this.StartType; }
        }

    }
}
