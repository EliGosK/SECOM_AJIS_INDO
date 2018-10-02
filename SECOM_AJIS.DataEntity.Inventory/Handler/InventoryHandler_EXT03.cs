using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using CSI.WindsorHelper;
using System.Transactions;
using System.IO;


namespace SECOM_AJIS.DataEntity.Inventory
{
    // Using by Atipan
    partial class InventoryHandler : BizIVDataEntities, IInventoryHandler
    {
        public doGetShelfOfArea GetShelfOfArea(string strAreaCode, string strInstrumentCode)
        {
            List<doGetShelfOfArea> lst = base.GetShelfOfArea(strAreaCode, strInstrumentCode, ShelfType.C_INV_SHELF_TYPE_NORMAL);

            if(lst != null && lst.Count > 0)
                return lst[0];
            else
            return null;
        }

        public bool UpdateAccountTransferSecondhandInstrumentIVS180(doGroupSecondhandInstrument doGroupSecondhand)
        {
            try
            {
                //1.	Check Instrument Qty from source location 
                List<tbt_AccountInstalled> doTbt_AccountInstalled = GetTbt_AccountInstalled(doGroupSecondhand.SourceOfficeCode, doGroupSecondhand.SourceLocationCode, doGroupSecondhand.Instrumentcode, null);

                //2.	Get Instrument data
                List<doFIFOInstrument> doFIFOInstrument = new List<doFIFOInstrument>();

                if (doTbt_AccountInstalled != null && doTbt_AccountInstalled.Count > 0) //Modify by Jutarat A. on 23012013
                {
                    //2.1	Change FIFO data to LIFO data When user transfer instrument from Buffer W/H to Instock W/H
                    if (doGroupSecondhand.SourceLocationCode == InstrumentLocation.C_INV_LOC_BUFFER)
                    {
                        doFIFOInstrument = GetLIFOInstrument(doGroupSecondhand.SourceOfficeCode, doGroupSecondhand.SourceLocationCode, doGroupSecondhand.Instrumentcode);
                    }
                    else
                    {
                        doFIFOInstrument = GetFIFOInstrument(doGroupSecondhand.SourceOfficeCode, doGroupSecondhand.SourceLocationCode, doGroupSecondhand.Instrumentcode);
                    }
                }

                //2.2	Update Instrument data in source location
                int iTransferQty = doGroupSecondhand.TransferQty.Value;
                int iSourceQty = 0;
                int iDestinationQty = 0;
                List<tbt_AccountInstalled> lstDestMatchAcc = GetTbt_AccountInstalled(doGroupSecondhand.DestinationOfficeCode, doGroupSecondhand.DestinationLocationCode, doGroupSecondhand.Instrumentcode, null);
                List<tbt_AccountInstalled> lstSrcMatchAcc = GetTbt_AccountInstalled(doGroupSecondhand.SourceOfficeCode, doGroupSecondhand.SourceLocationCode, doGroupSecondhand.Instrumentcode, null);

                //Add by Jutarat A. on 28112013
                if ((lstDestMatchAcc == null || lstDestMatchAcc.Count == 0)
                    && (lstSrcMatchAcc == null || lstSrcMatchAcc.Count == 0))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4143, new string[] { doGroupSecondhand.Instrumentcode });
                }
                //End Add

                foreach (doFIFOInstrument i in doFIFOInstrument)
                {
                    if (iTransferQty <= 0)
                    {
                        break;
                    }

                    if (i.InstrumentQty <= iTransferQty)
                    {
                        iSourceQty = 0;
                        iDestinationQty = Convert.ToInt32(i.InstrumentQty);
                    }
                    else
                    {
                        iSourceQty = Convert.ToInt32(i.InstrumentQty) - iTransferQty;
                        iDestinationQty = iTransferQty;
                    }

                    List<tbt_AccountInstalled> matchSrcAcc = (from c in lstSrcMatchAcc where c.LotNo == i.LotNo select c).ToList<tbt_AccountInstalled>();
                    if (matchSrcAcc != null && matchSrcAcc.Count > 0) //Modify by Jutarat A. on 23012013
                    {
                        matchSrcAcc[0].OfficeCode = doGroupSecondhand.SourceOfficeCode;
                        matchSrcAcc[0].LocationCode = doGroupSecondhand.SourceLocationCode;
                        matchSrcAcc[0].LotNo = i.LotNo;
                        matchSrcAcc[0].InstrumentCode = doGroupSecondhand.Instrumentcode;
                        matchSrcAcc[0].InstrumentQty = iSourceQty;
                        matchSrcAcc[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        matchSrcAcc[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        List<tbt_AccountInstalled> lstForUpdate = new List<tbt_AccountInstalled>();
                        lstForUpdate.Add(matchSrcAcc[0]);
                        List<tbt_AccountInstalled> resultUpdate = UpdateTbt_AccountInstalled(lstForUpdate);
                        if (resultUpdate != null && resultUpdate.Count <= 0) //Modify by Jutarat A. on 23012013
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                        }
                    }

                    //2.3	Update Instrument data in destination location
                    List<tbt_AccountInstalled> matchDestAcc = (from c in lstDestMatchAcc where c.LotNo == i.LotNo select c).ToList<tbt_AccountInstalled>();
                    if (matchDestAcc != null && matchDestAcc.Count <= 0) //Modify by Jutarat A. on 23012013
                    {
                        //Inset new record for destination
                        tbt_AccountInstalled AccForInsert = new tbt_AccountInstalled();
                        AccForInsert.OfficeCode = doGroupSecondhand.DestinationOfficeCode;
                        AccForInsert.LocationCode = doGroupSecondhand.DestinationLocationCode;
                        AccForInsert.LotNo = i.LotNo;
                        AccForInsert.InstrumentCode = doGroupSecondhand.Instrumentcode;
                        AccForInsert.InstrumentQty = iDestinationQty;
                        AccForInsert.AccquisitionCost = i.AccquisitionCost;
                        AccForInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        AccForInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        AccForInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        AccForInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        matchDestAcc.Add(AccForInsert);
                        List<tbt_AccountInstalled> resultInsert = InsertTbt_AccountInstalled(matchDestAcc);
                        if (resultInsert != null && resultInsert.Count == 0) //Modify by Jutarat A. on 23012013
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                        }
                    }
                    else
                    {
                        //Udpate record for destination 
                        matchDestAcc[0].OfficeCode = doGroupSecondhand.DestinationOfficeCode;
                        matchDestAcc[0].LocationCode = doGroupSecondhand.DestinationLocationCode;
                        matchDestAcc[0].LotNo = i.LotNo;
                        matchDestAcc[0].InstrumentCode = doGroupSecondhand.Instrumentcode;
                        matchDestAcc[0].InstrumentQty = matchDestAcc[0].InstrumentQty + iDestinationQty;
                        matchDestAcc[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        matchDestAcc[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        List<tbt_AccountInstalled> lstForUpDestAcc = new List<tbt_AccountInstalled>();
                        lstForUpDestAcc.Add(matchDestAcc[0]);
                        List<tbt_AccountInstalled> resultUpdate = UpdateTbt_AccountInstalled(lstForUpDestAcc);
                        if (resultUpdate != null && resultUpdate.Count == 0) //Modify by Jutarat A. on 23012013
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                        }
                    }

                    iTransferQty = iTransferQty - Convert.ToInt32(i.InstrumentQty);
                }

                //3.	Update remaining data to tbt_accountinstalled
                if (iTransferQty > 0)
                {
                    //3.1	Get oldest lot from  destination location 
                    List<doFIFOInstrument> FifoInstrument = GetFIFOInstrument(doGroupSecondhand.DestinationOfficeCode, doGroupSecondhand.DestinationLocationCode, doGroupSecondhand.Instrumentcode);

                    //Add by Jutarat A. on 23012013
                    string strLotNo = null;
                    if (FifoInstrument != null && FifoInstrument.Count > 0)
                        strLotNo = FifoInstrument[0].LotNo;
                    //End Add

                    //3.2	Check existing lot No. in source location
                    List<tbt_AccountInstalled> SrcAccInt = GetTbt_AccountInstalled(doGroupSecondhand.SourceOfficeCode, doGroupSecondhand.SourceLocationCode, doGroupSecondhand.Instrumentcode, strLotNo); //FifoInstrument[0].LotNo); //Modify by Jutarat A. on 23012013

                    if (SrcAccInt != null && SrcAccInt.Count > 0) //Modify by Jutarat A. on 23012013
                    {
                        //3.2.2 Update remaining data to source location 
                        SrcAccInt[0].OfficeCode = doGroupSecondhand.SourceOfficeCode;
                        SrcAccInt[0].LocationCode = doGroupSecondhand.SourceLocationCode;
                        SrcAccInt[0].LotNo = strLotNo; //FifoInstrument[0].LotNo; //Modify by Jutarat A. on 23012013
                        SrcAccInt[0].InstrumentCode = doGroupSecondhand.Instrumentcode;
                        SrcAccInt[0].InstrumentQty = SrcAccInt[0].InstrumentQty - iTransferQty;
                        SrcAccInt[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        SrcAccInt[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        List<tbt_AccountInstalled> lstForUpSrcAcc = new List<tbt_AccountInstalled>();
                        lstForUpSrcAcc.Add(SrcAccInt[0]);
                        List<tbt_AccountInstalled> resultUpdate = UpdateTbt_AccountInstalled(lstForUpSrcAcc);
                        if (resultUpdate != null && resultUpdate.Count == 0) //Modify by Jutarat A. on 23012013
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                        }
                    }
                    else
                    {
                        tbt_AccountInstalled doTbt_AccInstalled = new tbt_AccountInstalled();
                        doTbt_AccInstalled.OfficeCode = doGroupSecondhand.SourceOfficeCode;
                        doTbt_AccInstalled.LocationCode = doGroupSecondhand.SourceLocationCode;
                        //doTbt_AccInstalled.LotNo = FifoInstrument[0].LotNo;
                        doTbt_AccInstalled.InstrumentCode = doGroupSecondhand.Instrumentcode;
                        doTbt_AccInstalled.InstrumentQty = 0 - iTransferQty;
                        //doTbt_AccInstalled.AccquisitionCost = FifoInstrument[0].AccquisitionCost;
                        doTbt_AccInstalled.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        doTbt_AccInstalled.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        doTbt_AccInstalled.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        doTbt_AccInstalled.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        if (FifoInstrument != null && FifoInstrument.Count > 0) //Add by Jutarat A. on 23012013
                        {
                            doTbt_AccInstalled.LotNo = FifoInstrument[0].LotNo;
                            doTbt_AccInstalled.AccquisitionCost = FifoInstrument[0].AccquisitionCost;
                        }

                        List<tbt_AccountInstalled> lstForInstSrcAcc = new List<tbt_AccountInstalled>();
                        lstForInstSrcAcc.Add(doTbt_AccInstalled);
                        List<tbt_AccountInstalled> resultInsert = InsertTbt_AccountInstalled(lstForInstSrcAcc);
                        if (resultInsert != null && resultInsert.Count == 0) //Modify by Jutarat A. on 23012013
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                        }
                    }

                    //3.2.3 Update remaining data to destination location 
                    List<tbt_AccountInstalled> DestAccInt = GetTbt_AccountInstalled(doGroupSecondhand.DestinationOfficeCode, doGroupSecondhand.DestinationLocationCode, doGroupSecondhand.Instrumentcode, strLotNo); //FifoInstrument[0].LotNo); //Modify by Jutarat A. on 23012013

                    if (DestAccInt != null && DestAccInt.Count > 0) //Modify by Jutarat A. on 23012013
                    {
                        DestAccInt[0].OfficeCode = doGroupSecondhand.DestinationOfficeCode;
                        DestAccInt[0].LocationCode = doGroupSecondhand.DestinationLocationCode;
                        DestAccInt[0].LotNo = strLotNo; //FifoInstrument[0].LotNo; //Modify by Jutarat A. on 23012013
                        DestAccInt[0].InstrumentCode = doGroupSecondhand.Instrumentcode;
                        DestAccInt[0].InstrumentQty = DestAccInt[0].InstrumentQty + iTransferQty;
                        DestAccInt[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        DestAccInt[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        List<tbt_AccountInstalled> lstForUpDestAcc = new List<tbt_AccountInstalled>();
                        lstForUpDestAcc.Add(DestAccInt[0]);
                        List<tbt_AccountInstalled> resultUpdate = UpdateTbt_AccountInstalled(lstForUpDestAcc);
                        if (resultUpdate != null && resultUpdate.Count == 0) //Modify by Jutarat A. on 23012013
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool UpdateAccountTransferSecondhandInstrumentIVS190(doGroupSecondhandInstrument doGroupSecondhand)
        {
            try
            {
                //1.    Get first-in first-out data for transfer oldest instrument
                List<doFIFOInstrument> FifoInstrument = GetFIFOInstrument(doGroupSecondhand.SourceOfficeCode, doGroupSecondhand.SourceLocationCode, doGroupSecondhand.Instrumentcode);

                //2.	Prepare Data for transfer
                if (doGroupSecondhand.TransferType == false)
                {
                    doGroupSecondhand.TransferQty = doGroupSecondhand.TransferQty * -1;
                }

                //3.	Set Clear QTY in all of lot No. in tbt_accountInstalled
                doClearQtyAllLot doClearQty = new doClearQtyAllLot();
                doClearQty.OfficeCode = doGroupSecondhand.SourceOfficeCode;
                doClearQty.LocationCode = doGroupSecondhand.SourceLocationCode;
                doClearQty.InstrumentCode = doGroupSecondhand.Instrumentcode;

                doClearQtyAllLot doClearResult = ClearQtyInAllLot(doClearQty);

                if (doClearResult.blnResult == false)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                }

                //4.	Update qty to lastest of lot No. in tbt_accountInstalled
                //4.1	Calculate Instrument Qty of lastest lot No
                int TTQty = 0;
                int SumInstrument = 0;

                foreach (doFIFOInstrument i in FifoInstrument)
                {
                    if(i.InstrumentQty != null)
                        SumInstrument += i.InstrumentQty.Value;
                }

                TTQty = SumInstrument - doGroupSecondhand.TransferQty.Value;

                //4.3	Update Qty data to lastest of lot No.
                if (FifoInstrument.Count > 0)
                {
                    List<tbt_AccountInstalled> doTbt_InventoryAccountInstalled = GetTbt_AccountInstalled(doGroupSecondhand.SourceOfficeCode, doGroupSecondhand.SourceLocationCode, doGroupSecondhand.Instrumentcode, FifoInstrument[FifoInstrument.Count - 1].LotNo);

                    if (doTbt_InventoryAccountInstalled.Count > 0)
                    {
                        doTbt_InventoryAccountInstalled[0].InstrumentQty = TTQty;
                        doTbt_InventoryAccountInstalled[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        doTbt_InventoryAccountInstalled[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                        List<tbt_AccountInstalled> dotbt_InvAccUpdate = new List<tbt_AccountInstalled>();
                        dotbt_InvAccUpdate.Add(doTbt_InventoryAccountInstalled[0]);

                        List<tbt_AccountInstalled> lstResult = UpdateTbt_AccountInstalled(dotbt_InvAccUpdate);
                        if (lstResult.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public doClearQtyAllLot ClearQtyInAllLot(doClearQtyAllLot doClear)
        {
            doClearQtyAllLot doClearReturn = new doClearQtyAllLot{blnResult = false};

            try
            {
                List<tbt_AccountInstalled> lst = this.ClearQtyInAllLot(doClear.OfficeCode, doClear.LocationCode, doClear.InstrumentCode);
                doClearReturn.blnResult = true;

                return doClearReturn;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CheckTransferFromBuffer(string strLocationCode, string strInstrumentCode)
        {
            List<int?> output = base.CheckTransferFromBuffer(strLocationCode
                                                , strInstrumentCode
                                                , InstrumentLocation.C_INV_LOC_BUFFER
                                                , InstrumentArea.C_INV_AREA_SE_RENTAL
                                                , InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO
                                                , InstrumentArea.C_INV_AREA_SE_LENDING_DEMO);

            if (output.Count > 0)
                return output[0] == 1;
            else
                return false;
        }
    }
}
