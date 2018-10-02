using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util.ConstantValue;
using CSI.WindsorHelper;
using System.Text.RegularExpressions;
namespace SECOM_AJIS.DataEntity.Master
{
    public class GroupMasterHandler : BizMADataEntities, IGroupMasterHandler
    {
        /// <summary>
        /// Getting group data
        /// </summary>
        /// <param name="inputDo"></param>
        /// <returns></returns>
        public List<doGroup> GetGroup(doGroup inputDo)
        {
            try
            {
                List<doGroup> lst = this.GetGroup(inputDo.GroupCode, inputDo.GroupName);

                if (lst == null)
                    lst = new List<doGroup>();
               else
                    CommonUtil.MappingObjectLanguage<doGroup>(lst);
               
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert group
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbm_Group> InsertGroup(doGroup doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbm_Group> insertList = base.InsertGroup(
                                                    doInsert.GroupCode
                                                    , doInsert.GroupNameEN
                                                    , doInsert.GroupNameLC
                                                    , doInsert.Memo
                                                    , doInsert.GroupOfficeCode
                                                    , doInsert.GroupEmpNo
                                                    , doInsert.DeleteFlag
                                                    , doInsert.CreateDate
                                                    , doInsert.CreateBy
                                                    , doInsert.UpdateDate
                                                    , doInsert.UpdateBy);

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_GROUP;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check group code is used by other system
        /// </summary>
        /// <param name="GroupCode"></param>
        /// <returns></returns>
        public bool IsUsedGroupData(string GroupCode)
        {
            try
            {
                List<int?> list = base.IsUsedGroup(GroupCode);

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
        /// Update group data.<br />
        /// - Check update date.<br />
        /// - Call UpdateGroup stored procedure.<br />
        /// - Write transaction log.
        /// </summary>
        /// <param name="doUpdate"></param>
        /// <returns></returns>
        public List<tbm_Group> UpdateGroup(doGroup doUpdate)
        {
            try
            {
                //Check whether this record is the most updated data
                List<tbm_Group> rList = this.GetTbm_Group(doUpdate.GroupCode);
                if (DateTime.Compare(rList[0].UpdateDate.Value, doUpdate.UpdateDate.Value) != 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                }

                //set updateDate and updateBy
                doUpdate.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doUpdate.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbm_Group> updatedList = base.UpdateGroup(
                                                    doUpdate.GroupCode
                                                    , doUpdate.GroupNameEN
                                                    , doUpdate.GroupNameLC
                                                    , doUpdate.Memo
                                                    , doUpdate.GroupOfficeCode
                                                    , doUpdate.GroupEmpNo
                                                    , doUpdate.DeleteFlag
                                                    , doUpdate.UpdateDate
                                                    , doUpdate.UpdateBy);

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_GROUP;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList;

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check is GroupNameLC and GroupCode already exist.
        /// </summary>
        /// <param name="GroupNameLC"></param>
        /// <param name="GroupCode"></param>
        /// <returns></returns>
        public bool CheckDuplicateGroupData(string GroupNameLC, string GroupCode)
        {
            try
            {
                //32  AND 46 	= 	space !"#$%&'()*+,-.
                //58  AND 64 	= 	:;<=>?@
                //91  AND 96 	= 	[\]^_`
                //123 AND 125 	= 	{|}
                
                if (CommonUtil.IsNullOrEmpty(GroupNameLC) == false)
                    GroupNameLC = Regex.Replace(GroupNameLC, "[ !\"#$%&'()*+,-.:;<=>?@[\\]^_`{|}]", "");

                List<int?> list = base.CheckDuplicateGroup(GroupNameLC, GroupCode,FlagType.C_FLAG_OFF);

                if (list.Count > 0 && list[0] > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Generate unique GroupCode.<br />
        /// - Get next running code.<br />
        /// - Generate check digit.<br />
        /// - Combind GroupCode Prefix + runningNo + check digit
        /// </summary>
        /// <returns></returns>
        public string GenerateGroupCode()
        {
            string strGroupCode = string.Empty;

            //1.	Get next running no
            ICommonHandler commHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doRunningNo> runningList = commHand.GetNextRunningCode(NameCode.C_NAME_CODE_GROUP_CODE);

            //2.	Generate check digit 
            string strRunningNo = runningList[0].RunningNo;
            if(runningList.Count > 0)
            {
                string strCheckDigit = commHand.GenerateCheckDigit(strRunningNo);
                strGroupCode = GroupCode.C_GROUP_CODE_PREFIX + strRunningNo + strCheckDigit;
            }

            return strGroupCode;
        }
    }
}
