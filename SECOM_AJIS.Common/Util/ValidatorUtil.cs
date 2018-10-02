using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.Common.Util
{
    /// <summary>
    /// Validator management
    /// </summary>
    public class ValidatorUtil
    {
        #region Inner Class
        
        private class iValidatorUtil : Controller
        {
            public bool IsValid { get; set; }
            
            public iValidatorUtil() 
            {
                if (System.Web.HttpContext.Current == null)
                    this.ControllerContext = new ControllerContext();
                else
                    this.ControllerContext = new ControllerContext( new RequestContext( new HttpContextWrapper(System.Web.HttpContext.Current), new RouteData()), this); 
            }
            public iValidatorUtil(Controller ctrl)
            {
                this.ControllerContext = ctrl.ControllerContext;
            }

            public void Validate(object model)
            {
                this.IsValid = TryValidateModel(model);
            }
        }
        private class ValidateMessage
        {
            public class ParameterObject
            {
                public int Order { get; set; }
                public string Parameter { get; set; }
            }

            public string Controller { get; set; }
            public string Screen { get; set; }
            public string Module { get; set; }
            public string Code { get; set; }
            public List<string> Params {
                get
                {
                    List<string> lst = new List<string>();
                    if (this.ParamList != null)
                    {
                        this.ParamList.Sort(delegate(ParameterObject p1, ParameterObject p2)
                            {
                                if (p1 == null || p2 == null)
                                    return 0;

                                if (p1.Order < p2.Order)
                                    return -1;
                                else if (p1.Order > p2.Order)
                                    return 1;
                                
                                return 0;
                            });

                        foreach (ParameterObject p in this.ParamList)
                        {
                            lst.Add(p.Parameter);
                        }
                    }

                    return lst;
                }
            }
            public List<string> Controls { get; set; }
            public bool HasNullValue { get; set; }

            private List<ParameterObject> ParamList { get; set; }
            public void SetParameter(int order, string param)
            {
                if (CommonUtil.IsNullOrEmpty(param))
                    return;

                if (this.ParamList == null)
                    this.ParamList = new List<ParameterObject>();

                bool isfound = false;
                foreach (ParameterObject p in this.ParamList)
                {
                    if (p.Parameter == param)
                    {
                        isfound = true;
                        break;
                    }
                }
                if (isfound == false)
                {
                    this.ParamList.Add(new ParameterObject()
                    {
                        Order = order,
                        Parameter = param
                    });
                }
            }
            public void ClearParameter()
            {
                this.ParamList = null;
            }
        }

        #endregion
        #region Variables

        private iValidatorUtil _iValidatorUtil;
        public const string SPLIT_TEMPLATE_MESSAGE = ";";

        #endregion
        #region Constructor

        public ValidatorUtil()
        {
            this._iValidatorUtil = new iValidatorUtil();
            this.IsValid = true;
        }
        public ValidatorUtil(Controller control)
        {
            this._iValidatorUtil = new iValidatorUtil(control);
            this._iValidatorUtil.IsValid = control.ModelState.IsValid;
            this._iValidatorUtil.ModelState.Merge(control.ModelState);
            this.IsValid = _iValidatorUtil.IsValid;
        }

        #endregion
        #region Static Methods

        /// <summary>
        /// Bulig error message
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="param"></param>
        /// <param name="first_message"></param>
        /// <returns></returns>
        public static ObjectResultData BuildErrorMessage(object obj, MessageParameter param = null, bool first_message = true)
        {
            try
            {
                ObjectResultData result = new ObjectResultData();
                ValidatorUtil validator = new ValidatorUtil();
                BuildErrorMessage(result, validator, new object[] { obj }, param, first_message);

                if (result.IsError == false)
                    return null;

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Bulig error message
        /// </summary>
        /// <param name="validator"></param>
        /// <param name="obj"></param>
        /// <param name="param"></param>
        /// <param name="first_message"></param>
        /// <returns></returns>
        public static ObjectResultData BuildErrorMessage(ValidatorUtil validator, object[] obj, MessageParameter param = null, bool first_message = true)
        {
            try
            {
                ObjectResultData result = new ObjectResultData();
                BuildErrorMessage(result, validator, obj, param, first_message);

                if (result.IsError == false)
                    return null;

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Bulig error message
        /// </summary>
        /// <param name="res"></param>
        /// <param name="obj"></param>
        /// <param name="param"></param>
        /// <param name="first_message"></param>
        /// <returns></returns>
        public static ObjectResultData BuildErrorMessage(ObjectResultData res, object[] obj, MessageParameter param = null, bool first_message = true)
        {
            try
            {
                ValidatorUtil validator = new ValidatorUtil();
                return BuildErrorMessage(res, validator, obj, param, first_message);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Bulig error message
        /// </summary>
        /// <param name="res"></param>
        /// <param name="controller"></param>
        /// <param name="obj"></param>
        /// <param name="param"></param>
        /// <param name="first_message"></param>
        /// <returns></returns>
        public static ObjectResultData BuildErrorMessage(ObjectResultData res, Controller controller, object[] obj = null, MessageParameter param = null, bool first_message = true)
        {
            try
            {
                if (controller == null)
                    return null;
                ValidatorUtil validator = new ValidatorUtil(controller);
                return BuildErrorMessage(res, validator, obj, param, first_message);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Bulig error message
        /// </summary>
        /// <param name="res"></param>
        /// <param name="validator"></param>
        /// <param name="obj"></param>
        /// <param name="param"></param>
        /// <param name="first_message"></param>
        /// <returns></returns>
        public static ObjectResultData BuildErrorMessage(ObjectResultData res, ValidatorUtil validator, object[] obj = null, MessageParameter param = null, bool first_message = true)
        {
            try
            {
                if (obj != null)
                {
                    foreach (object o in obj)
                    {
                        if (o != null)
                        {
                            validator.Validate(o);
                            validator.ValidateAtLeast1Field(o);
                        }
                    }
                }
                if (validator.IsValid == false)
                {
                    List<ValidateMessage> msgLst = validator.ConvertToMessage();
                    foreach (ValidateMessage msg in msgLst)
                    {
                        string[] pm = null;
                        if (param != null)
                            pm = param.GetParameter(msg.Code);
                        else if (msg.Params != null)
                            pm = new string[] { CommonUtil.TextList(msg.Params.ToArray()) };

                        MessageUtil.MessageList msgCode;
                        if (Enum.TryParse<MessageUtil.MessageList>(msg.Code, out msgCode))
                            res.AddErrorMessage(msg.Controller, msg.Screen, msg.Module, msgCode, pm, msg.Controls == null ? null : msg.Controls.ToArray());

                        if (first_message == true)
                            break;
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        #region Model State Methods

        /// <summary>
        /// Add error message
        /// </summary>
        /// <param name="module"></param>
        /// <param name="code"></param>
        /// <param name="id"></param>
        /// <param name="param"></param>
        public void AddErrorMessage(string module, MessageUtil.MessageList code, string id, params string[] param)
        {
            AddErrorMessage(null, null, module, code, id, param);
        }
        /// <summary>
        /// Add error message
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="screen"></param>
        /// <param name="module"></param>
        /// <param name="code"></param>
        /// <param name="id"></param>
        /// <param name="param"></param>
        public void AddErrorMessage(string controller, string screen, string module, MessageUtil.MessageList code, string id, params string[] param)
        {
            try
            {
                if (_iValidatorUtil.ModelState != null)
                {
                    string template = string.Empty;
                    
                    if (CommonUtil.IsNullOrEmpty(controller) == false)
                        template += controller;
                    template += SPLIT_TEMPLATE_MESSAGE;
                    if (CommonUtil.IsNullOrEmpty(screen) == false)
                        template += screen;
                    template += SPLIT_TEMPLATE_MESSAGE;

                    template += module + SPLIT_TEMPLATE_MESSAGE;
                    template += code.ToString() + SPLIT_TEMPLATE_MESSAGE;

                    if (param != null)
                    {
                        foreach (string pm in param)
                        {
                            template += pm + SPLIT_TEMPLATE_MESSAGE;
                        }
                    }
                    ModelState state = new System.Web.Mvc.ModelState();
                    state.Errors.Add(template);
                    _iValidatorUtil.ModelState.Add(id, state);
                    _iValidatorUtil.IsValid = false;
                    this.IsValid = false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        #region Private Methods

        private bool IsValid { get; set; }
        private void Validate(object model)
        {
            _iValidatorUtil.Validate(model);
            if (_iValidatorUtil.IsValid == false)
                this.IsValid = false;
        }
        private List<ValidateMessage> ConvertToMessage(string code = null)
        {
            try
            {
                List<ValidateMessage> msgLst = new List<ValidateMessage>();

                foreach (ModelState ms in this._iValidatorUtil.ModelState.Values)
                {
                    foreach (ModelError mErr in ms.Errors)
                    {
                        string mTxt = mErr.ErrorMessage;
                        if (mTxt == null)
                            continue;

                        string[] mspTxt = mTxt.Split(SPLIT_TEMPLATE_MESSAGE.ToCharArray());
                        if (mspTxt.Length < 4)
                            continue;

                        if (code != null)
                        {
                            if (code != mspTxt[3])
                                continue;
                        }

                        ValidateMessage msg = null;
                        if (msgLst == null)
                            msgLst = new List<ValidateMessage>();
                        foreach (ValidateMessage m in msgLst)
                        {
                            if (m.Controller == mspTxt[0]
                                && m.Screen == mspTxt[1]
                                && m.Module == mspTxt[2]
                                && m.Code == mspTxt[3])
                            {
                                msg = m;
                                break;
                            }
                        }
                        if (msg == null)
                        {
                            msg = new ValidateMessage();
                            msg.Controller = mspTxt[0];
                            msg.Screen = mspTxt[1];
                            msg.Module = mspTxt[2];
                            msg.Code = mspTxt[3];
                            msgLst.Add(msg);
                        }
                        if (mspTxt.Length > 4)
                        {
                            //if (msg.Params == null)
                            //    msg.Params = new List<string>();
                            //if (msg.Params.Contains(mspTxt[4]) == false)
                            //    msg.Params.Add(mspTxt[4]);

                            int order = 0;
                            string param = string.Empty;

                            if (mspTxt.Length > 5)
                            {
                                if (msg.Controls == null)
                                    msg.Controls = new List<string>();
                                if (msg.Controls.Contains(mspTxt[5]) == false)
                                {
                                    string[] ctrls = mspTxt[5].Split(",".ToCharArray());
                                    foreach (string ctrl in ctrls)
                                    {
                                        msg.Controls.Add(ctrl.Trim());
                                    }
                                }
                            }
                            if (mspTxt.Length > 6)
                            {
                                int.TryParse(mspTxt[6], out order);
                            }

                            msg.SetParameter(order, mspTxt[4]);
                        }
                    }
                }

                return msgLst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void ValidateAtLeast1Field(object obj)
        {
            try
            {
                List<ValidateMessage> msgLst = new List<ValidateMessage>();
                Dictionary<string, AtLeast1FieldNotNullOrEmptyAttribute> langAttr =
                    CommonUtil.CreateAttributeDictionary<AtLeast1FieldNotNullOrEmptyAttribute>(obj);
                foreach (KeyValuePair<string, AtLeast1FieldNotNullOrEmptyAttribute> attr in langAttr)
                {
                    ValidateMessage nm = null;
                    foreach(ValidateMessage m in msgLst)
                    {
                        if (m.Controller == attr.Value.Controller
                            && m.Screen == attr.Value.Screen
                            && m.Module == attr.Value.Module
                            && m.Code == attr.Value.MessageCode.ToString())
                        {
                            nm = m;
                            break;
                        }
                    }
                    if (nm == null)
                    {
                        nm = new ValidateMessage()
                        {
                            Controller = attr.Value.Controller,
                            Screen = attr.Value.Screen,
                            Module = attr.Value.Module,
                            Code = attr.Value.MessageCode.ToString(),
                            
                            HasNullValue = true
                        };
                        msgLst.Add(nm);
                    }
                    if (nm.HasNullValue == false)
                        continue;

                    PropertyInfo prop = obj.GetType().GetProperty(attr.Key);
                    if (prop != null)
                    {
                        object val = prop.GetValue(obj, null);
                        if (CommonUtil.IsNullOrEmpty(val) == false)
                        {
                            nm.HasNullValue = false;
                            nm.ClearParameter();
                        }
                        else if (attr.Value.UseControl == true)
                        {
                            nm.SetParameter(0, CommonUtil.IsNullOrEmpty(attr.Value.ControlName) ? attr.Key : attr.Value.ControlName);
                        }
                    }
                }
                if (msgLst.Count > 0)
                {
                    foreach (ValidateMessage msg in msgLst)
                    {
                        if (msg.HasNullValue == true)
                        {
                            MessageUtil.MessageList msgCode;
                            if (Enum.TryParse<MessageUtil.MessageList>(msg.Code, out msgCode))
                            {
                                string template = CommonUtil.TextList(msg.Params == null? null : msg.Params.ToArray());
                                this.AddErrorMessage(msg.Controller, msg.Screen, msg.Module, msgCode, obj.GetHashCode().ToString(), "", template);
                                this.IsValid = false;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}