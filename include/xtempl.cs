using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using runnerDotNet;
namespace runnerDotNet
{
	public partial class XTempl : XTempl_Base
	{
        public delegate XVar xtFuncDelegate( XVar _params );
        public delegate XVar xtMethodDelegate( XVar _params );

		public ViewContext currentViewContext;
		public XVar view_filename; // overriden view name
		

		public XTempl() : base()
		{
		}

		
		public XVar report_error(XVar _param_message)
		{
			XVar message = _param_message.Clone();
			MVCFunctions.EchoToOutput(message);
			return null;
		}
		
		public static XVar create_function_assignment( XVar func, XVar _params, XVar obj = null )
		{
			var ret = XVar.Array();
			ret["func"] = (XVar)MVCFunctions.AssignFunction( func, _params, obj);
			return ret;
		}
		
		public static XVar create_method_assignment( XVar method, object var_object, XVar _params = null )
		{
			var ret = XVar.Array();
			ret["method"] = (XVar)MVCFunctions.AssignMethod( method, var_object, _params);
			return ret;
		}
/*

		public override XVar xt_doevent(dynamic _param_params)
		{
			XVar var_params = _param_params.Clone();
			XVar eventName, eventObj;
			if(this.xt_events.KeyExists(var_params["custom1"]))
			{
				XVar eventArr;
				eventArr = this.xt_events[var_params["custom1"]];
				if(eventArr.KeyExists("method"))
				{
					XVar method;
					var_params = new XVar();
					if(eventArr.KeyExists("params"))
						var_params = eventArr["params"];
					method = eventArr["method"];
					return eventArr["object"].Invoke(method, var_params);
				}
			}
			
			eventObj = GlobalVars.globalEvents;
			if(!eventObj)
				return null;
			eventName = var_params["custom1"];
			if(!eventObj.Invoke("exists", eventName))
				return null;
			return eventObj.Invoke("event_" + MVCFunctions.GoodFieldName( eventName ).ToString(), var_params);
		}
*/		
		
		public override XVar fetchVar(dynamic _param_varName)
		{
			return processVarInternal(getVar(_param_varName), new XVar());
		}
		
		public XVar call_func(XVar _param_var)
		{
			XVar var_var = _param_var.Clone();

			XVar var_out;
			if(!var_var.IsArray() || !var_var["func"].Length())
			{
				return "";
			}
			MVCFunctions.ob_start();

            XVar res = ((XTempl.xtFuncDelegate)var_var["func"].Value)(new XVar());
			string resString = res as object == null ? "" : res.ToString();
			var_out = MVCFunctions.ob_get_contents() + resString;
			MVCFunctions.ob_end_clean();
			return var_out;

		}		
/*		
		public override XVar set_template(dynamic _param_template)
		{
			XVar template = _param_template.Clone();

            if ( true || this.jsonMode)
            {
				if (MVCFunctions.substr(template, 0, 1) == "/")
					template = MVCFunctions.substr(template, 1);
				this.template_file = template.Replace("/", "_").Replace(".cshtml", ".json").Replace(".htm", ".json");
				this.template = MVCFunctions.myfile_get_contents(MVCFunctions.getabspath(MVCFunctions.Concat("pdf/", this.template_file)), "r");
				return null;
            }

			XVar templatesPath;
			templatesPath = "";
		
            if (MVCFunctions.substr(template, 0, 1) == "/")
                template = MVCFunctions.substr(template, 1);
			this.template_file = template.Replace("/", "_").Replace(".cshtml", "").Replace(".htm", "");
			set_layout();


			view_filename = "";
			
			//	try to locate template file
			if ( MVCFunctions.file_exists( MVCFunctions.Concat("Views/", MVCFunctions.Concat(templatesPath, template) )) )
				view_filename = MVCFunctions.Concat(templatesPath, template);
			else if ( MVCFunctions.file_exists( MVCFunctions.Concat("Views/", MVCFunctions.Concat(templatesPath, "Global/", template) ) ) )
				view_filename = MVCFunctions.Concat(templatesPath, "Global/", template);
			else if ( MVCFunctions.file_exists( MVCFunctions.Concat("Views/", template.Replace("_", "/") ) ) )
				view_filename = template.Replace("_", "/");
			else if ( MVCFunctions.file_exists( MVCFunctions.Concat("Views/Shared/", template.Replace(".htm", ".cshtml") ) ) )
				view_filename = MVCFunctions.Concat("Shared/", template.Replace(".htm", ".cshtml") );
				

			if ( view_filename != "" )
				this.template = MVCFunctions.myfile_get_contents(MVCFunctions.getabspath(MVCFunctions.Concat("Views/", view_filename)), "r");

			view_filename = view_filename.Replace(".cshtml", "");
			assign_headers();
			return null;
		}
*/		
		public override XVar display_loaded(dynamic filtertag = null)
		{
 			if( true || this.jsonMode ) {
				return base.display_loaded((XVar)filtertag);
			}
			if(filtertag as Object == null)
			    filtertag = new XVar();
			Testing();
			//MVCFunctions.xt_process_template(this, str);
			MVCFunctions.Echo(MVCFunctions.RenderSectionToStringFromString(this.template, filtertag, this));
			return null;
		}

        public override XVar display(dynamic template)
        {
			load_template(template);
			if( true || this.jsonMode ) {
				process_template( this.template );
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			return null;
        }

		public override XVar displayPartial(dynamic template)
		{
            StreamReader sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "Views/" + template.ToString(), Encoding.GetEncoding((int) GlobalVars.cCodePage));
            string templateString = sr.ReadToEnd();
            sr.Close();
			load_template(template);
			Testing();
			
			MVCFunctions.Echo( RunnerRazor.RenderRazorString<XTempl>(this, templateString) );
			return null;
		}
		
		
		public override XVar processVar(dynamic _param_var_var, dynamic _param_varparams)
		{
			XVar fparams = XVar.Array();
			transformFuncParams( _param_varparams, fparams );
			MVCFunctions.Echo( processVarInternal( _param_var_var , fparams ) );
			return null;
		}
		
		public XVar processVarInternal(dynamic _param_var_var, dynamic _param_varparams)
		{
			#region pass-by-value parameters
			dynamic varparams = _param_varparams as Object == null ? new XVar() : (_param_varparams).Clone();
			dynamic var_var = _param_var_var as Object == null ? new XVar() : (_param_var_var).Clone();
			#endregion			XVar var_var = new XVar( _param_var_var );

			if(!var_var.IsArray())
			{
				return var_var.ToString();
			}
			else if(var_var.KeyExists("func"))
			{
				MVCFunctions.ob_start();
				var result = ((xtFuncDelegate)var_var["func"].Value)(varparams);
				var resultStr = result as object == null ? "" : result.ToString();
				return MVCFunctions.GetBuferContentAndClearBufer().ToString() + resultStr;
			}
			else if(var_var.KeyExists("method"))
			{
				MVCFunctions.ob_start();
				var result = ((xtMethodDelegate)var_var["method"].Value)( varparams );
				var resultStr = result as object == null ? "" : result.ToString();
				return MVCFunctions.GetBuferContentAndClearBufer().ToString() + resultStr;
			}
			else
			{
				report_error("Incorrect variable value");
				return "";
			}
		}
		
/*		
        public System.Collections.IEnumerable GetSection(string sectionName, object page)
        {
            XVar sectionValue = getVar(sectionName);
            XSection xSection = new XSection();

            if (sectionValue != null)
            {
                if (sectionValue.Type == typeof(bool) && sectionValue)
                    yield return sectionName;
                else if (sectionValue.IsArray())
                {
                    if (sectionValue.KeyExists("begin"))
                        WriteToPage(page, sectionValue["begin"]);
                    if (!sectionValue.KeyExists("data")
                        || !sectionValue["data"].IsArray())
                        yield return sectionName;
                    else
                    {
                        xSection.End = null;
                        foreach (var dataItem in sectionValue["data"].GetEnumerator())
                        {
                            if (dataItem.Value.IsArray())
                            {
                                xt_stack.Add(dataItem.Value);
                                if (dataItem.Value.KeyExists("begin"))
                                    WriteToPage(page, dataItem.Value["begin"]);
                            }
                            yield return sectionName;
                            if (dataItem.Value.IsArray())
                            {
                                xt_stack.Remove(xt_stack.Count() - 1);
                                if (dataItem.Value.KeyExists("end"))
                                    WriteToPage(page, dataItem.Value["end"]);
                            }
                            xSection.Begin = null;
                        }
                    }
                    if (sectionValue.KeyExists("end"))
                        WriteToPage(page, sectionValue["end"]);
                }
                else if (sectionValue.Type != typeof(bool))
                {
                    yield return sectionName;
                }
            }
        }
*/		

        public MvcHtmlString displayVar(string name)
        {
            string[] parameters = name.Split(' ');
            XVar var = getVar(parameters[0]);
            string result = "";
            if(var != null)
            {
                XVar customParameters = new XVar();
                for(int i = 1; i < parameters.Length; i++)
                {
					if (parameters[i].Length > 0)
						customParameters["custom" + i.ToString()] = parameters[i];
                }
                result = processVarInternal(var, customParameters);
            }
            return MvcHtmlString.Create(result);
        }

/*
        private void WriteToPage(object page, XVar objectToWrite)
        {
            if (objectToWrite.IsArray())
            {
                    if (objectToWrite.KeyExists("method"))
                    {
                        XVar parameters = objectToWrite.KeyExists("params") ? objectToWrite["params"] : new XVar();
                        xtMethodDelegate invokedMethod = (xtMethodDelegate)objectToWrite["method"].Value;

                        MVCFunctions.ob_start();
						var result = invokedMethod.Invoke( parameters );
						var resultStr = result as object == null ? "" : result.ToString();

				
                        MvcHtmlString textToWrite = MvcHtmlString.Create(MVCFunctions.ob_get_contents() + resultStr );
                        MVCFunctions.ob_end_clean();

                        page.GetType().GetMethod("Write", new Type[] { typeof(MvcHtmlString) }).Invoke(page, new object[] { textToWrite });
                    }
            }
            else
               page.GetType().GetMethod("Write", new Type[] { typeof(MvcHtmlString) }).Invoke(page, new object[] { MvcHtmlString.Create(objectToWrite.ToString()) });
        }
*/		
		
		public string GetViewPath()
        {
			if (!string.IsNullOrEmpty(view_filename)) // view can be overridden
				return "../" + view_filename.ToString();
				
			return null;
            //throw ApplicationException("No view found");
        }
		
		public XVar mlang_message(XVar _param_params)
		{
			if (_param_params == null)
				_param_params = new XVar();
				
			return processMessageParams( CommonFunctions.mlang_message(_param_params) );
		}
	}

    public class XSection
    {
        object m_begin;
        public object Begin
        {
            get { return m_begin; }
            set { m_begin = value; }
        }

        object m_end;
        public object End
        {
            get { return m_end; }
            set { m_end = value; }
        }

        object m_data;
        public object Data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        public XSection() { }

        public XSection(object begin, object end, object data)
        {
            m_begin = begin;
            m_end = end;
            m_data = data;
        }
    }
}
