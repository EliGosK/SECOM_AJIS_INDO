using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using  SECOM_AJIS.Common.Util;
using System.Reflection;

namespace SECOM_AJIS.Common.Models
{
    /// <summary>
    /// DO for application exception
    /// </summary>
    public class ApplicationErrorException: Exception
    {
        public ObjectResultData ErrorResult { get; set; }
        /// <summary>
        /// Add error exception
        /// </summary>
        /// <param name="module"></param>
        /// <param name="code"></param>
        /// <param name="param"></param>
        public void AddErrorException(string module, MessageUtil.MessageList code, params string[] param)
        {
            try
            {
                if (this.ErrorResult == null)
                    this.ErrorResult = new ObjectResultData();
                this.ErrorResult.AddErrorMessage(module, code, param);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Throw new exception
        /// </summary>
        /// <param name="module"></param>
        /// <param name="code"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static ApplicationErrorException ThrowErrorException(string module, MessageUtil.MessageList code, params string[] param)
        {
            try
            {
                ApplicationErrorException error = new ApplicationErrorException();
                error.AddErrorException(module, code, param);
                
                return error;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Check mandatory field
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="param"></param>
        public static void CheckMandatoryField(object obj, MessageParameter param = null)
        {
            try
            {
                ObjectResultData result = ValidatorUtil.BuildErrorMessage(obj, param);
                if (result != null)
                {
                    if (result.IsError)
                    {
                        ApplicationErrorException error = new ApplicationErrorException();
                        error.ErrorResult = result;
                        throw error;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Check mandatory field
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="validator"></param>
        /// <param name="param"></param>
        public static void CheckMandatoryField(object obj, ValidatorUtil validator, MessageParameter param = null)
        {
            try
            {
                ObjectResultData result = ValidatorUtil.BuildErrorMessage(validator, new object[]{obj}, param);
                if (result != null)
                {
                    if (result.IsError)
                    {
                        ApplicationErrorException error = new ApplicationErrorException();
                        error.ErrorResult = result;
                        throw error;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Check mandatory field
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="obj"></param>
        /// <param name="param"></param>
        public static void CheckMandatoryField<T,R>(T obj, MessageParameter param = null)
            where T: class
            where R: new()
        {
            try
            {
                R cond = CommonUtil.CloneObject<T, R>(obj);
                CheckMandatoryField(cond, param);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Check mandatory field
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="obj"></param>
        /// <param name="validator"></param>
        /// <param name="param"></param>
        public static void CheckMandatoryField<T, R>(T obj, ValidatorUtil validator, MessageParameter param = null)
            where T : class
            where R : new()
        {
            try
            {
                R cond = CommonUtil.CloneObject<T, R>(obj);
                CheckMandatoryField(cond, validator, param);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
