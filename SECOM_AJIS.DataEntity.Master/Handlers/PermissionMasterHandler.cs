using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using System.Transactions;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util.ConstantValue;
using CSI.WindsorHelper;

namespace SECOM_AJIS.DataEntity.Master
{
    public class PermissionMasterHandler : BizMADataEntities, IPermissionMasterHandler {
        public List<tbm_PermissionGroup> DeletePermissionTypeOffice(string permissionGroupCode, DateTime updateDate) {
            List<DateTime?> updateDateList = base.GetPermissionGroupUpdateDate(permissionGroupCode);
            if (updateDateList == null || updateDateList.Count == 0 || updateDateList[0] == null
                || updateDate == null)
            {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "UpdateDate" });
            }

            if (updateDateList[0].Value.CompareTo(updateDate) != 0) {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019, new string[] { permissionGroupCode });
            }

            List<tbm_PermissionGroup> result = null;
            using (TransactionScope scope = new TransactionScope()) {
                try {
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog logData = new doTransactionLog() {
                        TransactionType = doTransactionLog.eTransactionType.Delete,
                        TableName = null,
                        TableData = null
                    };

                    List<tbm_PermissionIndividualDetail> listIndDetail = base.DeletePermissionIndividualDetail(permissionGroupCode, null);
                    if (listIndDetail.Count != 0) {
                        logData.TableData = CommonUtil.ConvertToXml(listIndDetail);
                        logData.TableName = TableName.C_TBL_NAME_PERMISSION_IND_DETAIL;
                        hand.WriteTransactionLog(logData);
                    }

                    List<tbm_PermissionIndividual> listInd = base.DeletePermissionIndividual(permissionGroupCode, null);
                    if (listInd.Count != 0) {
                        logData.TableData = CommonUtil.ConvertToXml(listInd);
                        logData.TableName = TableName.C_TBL_NAME_PERMISSION_IND;
                        hand.WriteTransactionLog(logData);
                    }

                    List<tbm_PermissionDetail> listDetail = base.DeletePermissionDetail(permissionGroupCode, null);
                    if (listDetail.Count != 0) {
                        logData.TableData = CommonUtil.ConvertToXml(listDetail);
                        logData.TableName = TableName.C_TBL_NAME_PERMISSION_DETAIL;
                        hand.WriteTransactionLog(logData);
                    }

                    result = base.DeletePermissionGroup(permissionGroupCode);
                    if (result.Count != 0) {
                        logData.TableData = CommonUtil.ConvertToXml(result);
                        logData.TableName = TableName.C_TBL_NAME_PERMISSION_GROUP;
                        hand.WriteTransactionLog(logData);
                    }
                } catch (Exception ex) {
                    throw ex;
                }

                scope.Complete();
            }
            return result;
        }

        public List<tbm_PermissionDetail> DeletePermissionTypeIndividual(string permissionGroupCode, string permissionIndividualCode, DateTime updateDate) {
            List<DateTime?> updateDateList = base.GetPermissionIndividualUpdateDate(permissionGroupCode, permissionIndividualCode);
            if (updateDateList == null || updateDateList.Count == 0 || updateDateList[0] == null
                || updateDate == null)
            {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "UpdateDate" });
            }

            if (updateDateList[0].Value.CompareTo(updateDate) != 0) {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019, new string[] { permissionIndividualCode });
            }

            List<tbm_PermissionDetail> result = null;
            using (TransactionScope scope = new TransactionScope()) {
                try {
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog logData = new doTransactionLog() {
                        TransactionType = doTransactionLog.eTransactionType.Delete,
                        TableName = null,
                        TableData = null
                    };

                    List<tbm_PermissionIndividualDetail> listIndDetail = base.DeletePermissionIndividualDetail(permissionGroupCode, permissionIndividualCode);
                    if (listIndDetail.Count != 0) {
                        logData.TableData = CommonUtil.ConvertToXml(listIndDetail);
                        logData.TableName = TableName.C_TBL_NAME_PERMISSION_IND_DETAIL;
                        hand.WriteTransactionLog(logData);
                    }

                    List<tbm_PermissionIndividual> listInd = base.DeletePermissionIndividual(permissionGroupCode, permissionIndividualCode);
                    if (listInd.Count != 0) {
                        logData.TableData = CommonUtil.ConvertToXml(listInd);
                        logData.TableName = TableName.C_TBL_NAME_PERMISSION_IND;
                        hand.WriteTransactionLog(logData);
                    }

                    result = base.DeletePermissionDetail(permissionGroupCode, permissionIndividualCode);
                    if (result.Count != 0) {
                        logData.TableData = CommonUtil.ConvertToXml(result);
                        logData.TableName = TableName.C_TBL_NAME_PERMISSION_DETAIL;
                        hand.WriteTransactionLog(logData);
                    }
                } catch (Exception ex) {
                    throw ex;
                }

                scope.Complete();
            }
            return result;
        }

        public List<dtPermissionHeader> GetPermission(doPermission condition)
        {
            List<tbm_ObjectFunction> ofLst = new List<tbm_ObjectFunction>();
            if (CommonUtil.IsNullOrEmpty(condition.ObjectFunction) == false)
            {
                string[] sp = condition.ObjectFunction.Split(",".ToArray());
                foreach (string s in sp)
                {
                    if (CommonUtil.IsNullOrEmpty(s) == false)
                    {
                        string[] ssp = s.Split("-".ToArray());
                        if (ssp.Length == 2)
                        {
                            tbm_ObjectFunction of = new tbm_ObjectFunction()
                            {
                                ObjectID = ssp[1],
                                FunctionID = int.Parse(ssp[0])
                            };
                            ofLst.Add(of);
                        }
                    }
                }
            }

            string xml = null;
            if (ofLst.Count > 0)
                xml =  CommonUtil.ConvertToXml_Store<tbm_ObjectFunction>(ofLst);

            return base.GetPermission(
                condition.TypeOffice, 
                condition.TypeIndividual, 
                condition.PermissionGroupName, 
                condition.OfficeCode, 
                condition.DepartmentCode, 
                condition.PositionCode, 
                xml, 
                condition.EmpNo,
                MiscType.C_PERMISSION_TYPE,
                PermissionType.C_PERMISSION_TYPE_OFFICE,
                PermissionType.C_PERMISSION_TYPE_INDIVIDUAL);
        }

        public List<tbm_PermissionGroup> AddPermissionTypeOffice(doPermission permission) {
            List<tbm_PermissionGroup> result = null;

            using (TransactionScope scope = new TransactionScope()) {
                try {
                    List<string> groupCodeList = base.GeneratePermissionGroupCode();
                    if (groupCodeList.Count != 0 && groupCodeList[0] != null) {
                        permission.PermissionGroupCode = groupCodeList[0];
                    } else {
                        return null;
                    }

                    result = base.InsertPermissionGroup(
                            permission.PermissionGroupCode,
                            permission.PermissionGroupName,
                            permission.OfficeCode,
                            permission.DepartmentCode,
                            permission.PositionCode, permission.CreateDate, permission.CreateBy);

                    if (result == null || result.Count == 0) {
                        return null;
                    } else {
                        doTransactionLog logData = new doTransactionLog() {
                            TransactionType = doTransactionLog.eTransactionType.Insert,
                            TableName = TableName.C_TBL_NAME_PERMISSION_GROUP,
                            TableData = CommonUtil.ConvertToXml(result)
                        };
                        ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        hand.WriteTransactionLog(logData);
                    }

                    if (permission.ObjectFunction != null && permission.ObjectFunction != "") {
                        List<tbm_PermissionDetail> detail = base.InsertPermissionDetailFromSelectedFunction(
                            permission.PermissionGroupCode,
                            permission.ObjectFunction,
                            permission.PermissionIndividualCode,
                            permission.CreateDate,
                            permission.CreateBy);

                        if (detail == null || detail.Count == 0) {
                            return null;
                        } else {
                            doTransactionLog logData = new doTransactionLog() {
                                TransactionType = doTransactionLog.eTransactionType.Insert,
                                TableName = TableName.C_TBL_NAME_PERMISSION_DETAIL,
                                TableData = CommonUtil.ConvertToXml(detail)
                            };
                            ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                            hand.WriteTransactionLog(logData);
                        }
                    }
                } catch (Exception e) {
                    throw e;
                }

                scope.Complete();
            }

            return result;
        }

        public List<tbm_PermissionIndividual> AddPermissionTypeIndividual(doPermission permission) {
            List<tbm_PermissionIndividual> result = null;

            using (TransactionScope scope = new TransactionScope()) {
                try {
                    List<string> individualCodeList = base.GeneratePermissionIndividualCode();
                    if (individualCodeList.Count != 0 && individualCodeList[0] != null) {
                        permission.PermissionIndividualCode = individualCodeList[0];
                    } else {
                        return null;
                    }

                    result = base.InsertPermissionIndividual(
                        permission.PermissionGroupCode,
                        permission.PermissionIndividualCode,
                        permission.PermissionIndividualName,
                        permission.CreateDate,
                        permission.CreateBy);

                    if (result == null || result.Count == 0) {
                        return null;
                    } else {
                        permission.PermissionIndividualCode = result[0].PermissionIndividualCode;

                        doTransactionLog logData = new doTransactionLog() {
                            TransactionType = doTransactionLog.eTransactionType.Insert,
                            TableName = TableName.C_TBL_NAME_PERMISSION_IND,
                            TableData = CommonUtil.ConvertToXml(result)
                        };
                        ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        hand.WriteTransactionLog(logData);
                    }

                    List<tbm_PermissionIndividualDetail> list = base.InsertPermissionIndividualDetailByEmpNo(
                        permission.PermissionGroupCode,
                        permission.PermissionIndividualCode,
                        permission.EmpNo,
                        permission.CreateDate,
                        permission.CreateBy);
                    if (list == null || list.Count == 0) {
                        return null;
                    } else {
                        doTransactionLog logData = new doTransactionLog() {
                            TransactionType = doTransactionLog.eTransactionType.Insert,
                            TableName = TableName.C_TBL_NAME_PERMISSION_IND_DETAIL,
                            TableData = CommonUtil.ConvertToXml(list)
                        };
                        ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        hand.WriteTransactionLog(logData);
                    }

                    // Akat K. 2012-03-14
                    // Akat K. 2012-03-06
                    //List<tbm_PermissionDetail> copyResult = base.CopyPermissionFromGroupToIndividual(
                    //        permission.PermissionGroupCode,
                    //        permission.PermissionIndividualCode,
                    //        permission.CreateDate,
                    //        permission.CreateBy);

                    //if (copyResult != null && copyResult.Count != 0) {
                    //    doTransactionLog logData = new doTransactionLog() {
                    //        TransactionType = doTransactionLog.eTransactionType.Insert,
                    //        TableName = TableName.C_TBL_NAME_PERMISSION_DETAIL,
                    //        TableData = CommonUtil.ConvertToXml(copyResult)
                    //    };
                    //    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    //    hand.WriteTransactionLog(logData);
                    //}

                    if (permission.ObjectFunction != null && permission.ObjectFunction != "") {
                        List<tbm_PermissionDetail> detail = base.InsertPermissionDetailFromSelectedFunction(
                            permission.PermissionGroupCode,
                            permission.ObjectFunction,
                            permission.PermissionIndividualCode,
                            permission.CreateDate,
                            permission.CreateBy);

                        if (detail == null || detail.Count == 0) {
                            return null;
                        } else {
                            doTransactionLog logData = new doTransactionLog() {
                                TransactionType = doTransactionLog.eTransactionType.Insert,
                                TableName = TableName.C_TBL_NAME_PERMISSION_DETAIL,
                                TableData = CommonUtil.ConvertToXml(detail)
                            };
                            ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                            hand.WriteTransactionLog(logData);
                        }
                    }
                } catch (Exception e) {
                    throw e;
                }

                scope.Complete();
            }

            return result;
        }

        private List<tbm_PermissionDetail> deletePermisssionDetail(string permissionGroupCode, string permissionIndividualCode, DateTime updatedate) {
            //List<DateTime?> updateDateList = base.GetPermissionDetailUpdateDate(permissionGroupCode, permissionIndividualCode);
            //if (updateDateList == null || updateDateList.Count == 0 || updateDateList[0] == null
            //    || updateDateList[0].Value.CompareTo(updatedate) != 0)
            //{
            //    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019, new string[] { permissionGroupCode });
            //}

            return base.DeletePermissionDetail(permissionGroupCode, permissionIndividualCode);
        }

        private List<tbm_PermissionGroup> updatePermissionGroup(doPermission permission) {
            List<DateTime?> updateDateList = base.GetPermissionGroupUpdateDate(permission.PermissionGroupCode);
            if (updateDateList == null || updateDateList.Count == 0 || updateDateList[0] == null
                || permission.UpdateDate == null || !permission.UpdateDate.HasValue)
            {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "UpdateDate" });
            }

            if (updateDateList[0].Value.CompareTo(permission.UpdateDate) != 0) {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019, new string[] { permission.PermissionGroupCode });
            }

            return base.UpdatePermissionGroup(
                permission.PermissionGroupCode, 
                permission.PermissionGroupName, 
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime, 
                CommonUtil.dsTransData.dtUserData.EmpNo);
        }

        public List<tbm_PermissionGroup> EditPermissionTypeOffice(doPermission permission) {
            List<tbm_PermissionGroup> result = null;

            using (TransactionScope scope = new TransactionScope()) {
                result = this.updatePermissionGroup(permission);
                if (result == null || result.Count == 0) {
                    return null;
                } else {
                    doTransactionLog logData = new doTransactionLog() {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_PERMISSION_GROUP,
                        TableData = CommonUtil.ConvertToXml(result)
                    };
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                List<tbm_PermissionDetail> deleteList = this.deletePermisssionDetail(
                    permission.PermissionGroupCode,
                    permission.PermissionIndividualCode,
                    permission.UpdateDate.Value);

                // Akat K. : Current permission may have no detail
                //if (deleteList == null || deleteList.Count == 0) {
                //    return null;
                //} else {
                if (deleteList != null && deleteList.Count != 0) {
                    doTransactionLog logData = new doTransactionLog() {
                        TransactionType = doTransactionLog.eTransactionType.Delete,
                        TableName = TableName.C_TBL_NAME_PERMISSION_DETAIL,
                        TableData = CommonUtil.ConvertToXml(deleteList)
                    };
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                permission.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                permission.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                if (permission.ObjectFunction != null && permission.ObjectFunction != "") {
                    List<tbm_PermissionDetail> detail = base.InsertPermissionDetailFromSelectedFunction(
                        permission.PermissionGroupCode,
                        permission.ObjectFunction,
                        permission.PermissionIndividualCode,
                        permission.CreateDate,
                        permission.CreateBy);

                    if (detail == null || detail.Count == 0) {
                        return null;
                    } else {
                        doTransactionLog logData = new doTransactionLog() {
                            TransactionType = doTransactionLog.eTransactionType.Insert,
                            TableName = TableName.C_TBL_NAME_PERMISSION_DETAIL,
                            TableData = CommonUtil.ConvertToXml(detail)
                        };
                        ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        hand.WriteTransactionLog(logData);
                    }
                }

                scope.Complete();
            }

            return result;
        }

        private List<tbm_PermissionIndividual> updatePermissionIndividual(doPermission permission) {
            List<DateTime?> updateDateList = base.GetPermissionIndividualUpdateDate(permission.PermissionGroupCode, permission.PermissionIndividualCode);
            if (updateDateList == null || updateDateList.Count == 0 || updateDateList[0] == null
                || permission.UpdateDate == null || !permission.UpdateDate.HasValue)
            {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "UpdateDate" });
            }

            if (updateDateList[0].Value.CompareTo(permission.UpdateDate.Value) != 0) {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019, new string[] { permission.PermissionIndividualCode });
            }

            return base.UpdatePermissionIndividual(permission.PermissionGroupCode, 
                permission.PermissionIndividualCode, 
                permission.PermissionIndividualName, 
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime, 
                CommonUtil.dsTransData.dtUserData.EmpNo);
        }

        private List<tbm_PermissionIndividualDetail> deletePermissionIndividualDetail(doPermission permission) {
            //List<DateTime?> updateDateList = base.GetPermissionIndividualDetailUpdateDate(permission.PermissionGroupCode, permission.PermissionIndividualCode);
            //if (updateDateList == null || updateDateList.Count == 0 || updateDateList[0] == null
            //    || updateDateList[0].Value.CompareTo(permission.UpdateDate.Value) != 0)
            //{
            //    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019, new string[] { permission.PermissionIndividualCode });
            //}

            return base.DeletePermissionIndividualDetailByEmpNo(permission.PermissionGroupCode, permission.PermissionIndividualCode, permission.DelEmpNo);
        }

        public List<tbm_PermissionIndividual> EditPermissionTypeIndividual(doPermission permission) {
            List<tbm_PermissionIndividual> result = null;
            using (TransactionScope scope = new TransactionScope()) {
                result = this.updatePermissionIndividual(permission);

                if (result == null || result.Count == 0) {
                    return null;
                } else {
                    doTransactionLog logData = new doTransactionLog() {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_PERMISSION_IND,
                        TableData = CommonUtil.ConvertToXml(result)
                    };
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                if (permission.DelEmpNo != null) {
                    List<tbm_PermissionIndividualDetail> deletedDetail = this.deletePermissionIndividualDetail(permission);
                    if (deletedDetail == null || deletedDetail.Count == 0) {
                        return null;
                    } else {
                        doTransactionLog logData = new doTransactionLog() {
                            TransactionType = doTransactionLog.eTransactionType.Delete,
                            TableName = TableName.C_TBL_NAME_PERMISSION_IND_DETAIL,
                            TableData = CommonUtil.ConvertToXml(deletedDetail)
                        };
                        ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        hand.WriteTransactionLog(logData);
                    }
                }

                if (permission.EmpNo != null) {
                    List<tbm_PermissionIndividualDetail> insertDetail = base.InsertPermissionIndividualDetailByEmpNo(
                        permission.PermissionGroupCode,
                        permission.PermissionIndividualCode,
                        permission.EmpNo,
                        CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                        CommonUtil.dsTransData.dtUserData.EmpNo);

                    if (insertDetail == null || insertDetail.Count == 0) {
                        return null;
                    } else {
                        doTransactionLog logData = new doTransactionLog() {
                            TransactionType = doTransactionLog.eTransactionType.Insert,
                            TableName = TableName.C_TBL_NAME_PERMISSION_IND_DETAIL,
                            TableData = CommonUtil.ConvertToXml(insertDetail)
                        };
                        ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        hand.WriteTransactionLog(logData);
                    }
                }

                List<tbm_PermissionDetail> deleteList = this.deletePermisssionDetail(
                    permission.PermissionGroupCode,
                    permission.PermissionIndividualCode,
                    permission.UpdateDate.Value);

                // Akat K. : Current permission may have no detail
                //if (deleteList == null || deleteList.Count == 0) {
                //    return null;
                //} else {
                if (deleteList != null && deleteList.Count != 0) {
                    doTransactionLog logData = new doTransactionLog() {
                        TransactionType = doTransactionLog.eTransactionType.Delete,
                        TableName = TableName.C_TBL_NAME_PERMISSION_DETAIL,
                        TableData = CommonUtil.ConvertToXml(deleteList)
                    };
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                // Akat K. 2012-03-14
                // Akat K. 2012-03-06
                //List<tbm_PermissionDetail> copyResult = base.CopyPermissionFromGroupToIndividual(
                //        permission.PermissionGroupCode,
                //        permission.PermissionIndividualCode,
                //        permission.CreateDate,
                //        permission.CreateBy);

                //if (copyResult != null && copyResult.Count != 0) {
                //    doTransactionLog logData = new doTransactionLog() {
                //        TransactionType = doTransactionLog.eTransactionType.Insert,
                //        TableName = TableName.C_TBL_NAME_PERMISSION_DETAIL,
                //        TableData = CommonUtil.ConvertToXml(copyResult)
                //    };
                //    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                //    hand.WriteTransactionLog(logData);
                //}

                if (permission.ObjectFunction != null && permission.ObjectFunction != "") {
                    List<tbm_PermissionDetail> detail = base.InsertPermissionDetailFromSelectedFunction(
                        permission.PermissionGroupCode,
                        permission.ObjectFunction,
                        permission.PermissionIndividualCode,
                        permission.CreateDate,
                        permission.CreateBy);

                    if (detail == null || detail.Count == 0) {
                        return null;
                    } else {
                        doTransactionLog logData = new doTransactionLog() {
                            TransactionType = doTransactionLog.eTransactionType.Insert,
                            TableName = TableName.C_TBL_NAME_PERMISSION_DETAIL,
                            TableData = CommonUtil.ConvertToXml(detail)
                        };
                        ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        hand.WriteTransactionLog(logData);
                    }
                }

                scope.Complete();
            }

            return result;
        }
    }
}
