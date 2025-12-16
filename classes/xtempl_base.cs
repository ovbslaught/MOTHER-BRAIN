using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using runnerDotNet;
namespace runnerDotNet
{
	public partial class XTempl_Base : XClass
	{
		public dynamic xt_vars = XVar.Array();
		public dynamic xt_stack;
		public dynamic xt_events = XVar.Array();
		public dynamic template;
		public dynamic template_file;
		public dynamic charsets = XVar.Array();
		public dynamic testingFlag = XVar.Pack(false);
		public dynamic eventsObject;
		public dynamic hiddenBricks = XVar.Array();
		public dynamic hiddenItems = XVar.Array();
		public dynamic hiddenRecordItems = XVar.Array();
		public dynamic preparedContainers = XVar.Array();
		public dynamic layout;
		public dynamic pageId = XVar.Pack(1);
		public dynamic pageObject = XVar.Pack(null);
		protected dynamic messageParamStack = XVar.Array();
		public dynamic cssFiles = XVar.Array();
		public dynamic jsonMode = XVar.Pack(false);
		public virtual XVar getVar(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			return MVCFunctions.xt_getvar(this, (XVar)(name));
		}
		public virtual XVar recTesting(dynamic arr)
		{
			foreach (KeyValuePair<XVar, dynamic> v in arr.GetEnumerator())
			{
				if(XVar.Pack(MVCFunctions.is_array((XVar)(v.Value))))
				{
					this.recTesting((XVar)(arr[v.Key]));
				}
				else
				{
					if(XVar.Pack(GlobalVars.testingLinks.KeyExists(v.Key)))
					{
						arr[v.Key] = MVCFunctions.Concat(arr[v.Key], " func=\"", GlobalVars.testingLinks[v.Key], "\"");
					}
				}
			}

			return null;
		}
		public virtual XVar Testing()
		{
			if(XVar.Pack(!(XVar)(this.testingFlag)))
			{
				return null;
			}
			this.recTesting((XVar)(this.xt_vars));

			return null;
		}
		public virtual XVar report_error(dynamic _param_message)
		{
			#region pass-by-value parameters
			dynamic message = XVar.Clone(_param_message);
			#endregion

			MVCFunctions.Echo(message);
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		protected virtual XVar assign_headers()
		{
			dynamic ext = null, templates = null;
			ext = XVar.Clone((XVar.Pack(ProjectSettings.getProjectValue(new XVar("ext")) == "php") ? XVar.Pack("php") : XVar.Pack("htm")));
			if(XVar.Pack(this.xt_vars.KeyExists("header")))
			{
				return null;
			}
			templates = XVar.Clone(ProjectSettings.getProjectValue(new XVar("customTemplates")));
			if(!XVar.Equals(XVar.Pack(MVCFunctions.array_search(new XVar("header"), (XVar)(templates))), XVar.Pack(false)))
			{
				MVCFunctions.xtempl_include_header(this, new XVar("header"), (XVar)(MVCFunctions.Concat("include/header.", ext)));
			}
			if(!XVar.Equals(XVar.Pack(MVCFunctions.array_search(new XVar("footer"), (XVar)(templates))), XVar.Pack(false)))
			{
				MVCFunctions.xtempl_include_header(this, new XVar("footer"), (XVar)(MVCFunctions.Concat("include/footer.", ext)));
			}

			return null;
		}
		public XTempl_Base()
		{
			dynamic html_attrs = null;
			this.xt_vars = XVar.Clone(XVar.Array());
			this.xt_stack = XVar.Clone(XVar.Array());
			this.xt_stack.InitAndSetArrayItem(this.xt_vars, null);
			this.assign_method(new XVar("event"), this, new XVar("xt_event"), (XVar)(XVar.Array()));
			this.assign_function(new XVar("label"), new XVar("xt_label"), (XVar)(XVar.Array()));
			this.assign_function(new XVar("tooltip"), new XVar("xt_tooltip"), (XVar)(XVar.Array()));
			this.assign_function(new XVar("tooltipAttr"), new XVar("xt_tooltipAttr"), (XVar)(XVar.Array()));
			this.assign_method(new XVar("custom"), this, new XVar("customLabel"), (XVar)(XVar.Array()));
			this.assign_function(new XVar("htmlcustom"), new XVar("xt_htmlcustom"), (XVar)(XVar.Array()));
			this.assign_function(new XVar("cl_length"), new XVar("xt_cl_length"), (XVar)(XVar.Array()));
			this.assign_function(new XVar("caption"), new XVar("xt_caption"), (XVar)(XVar.Array()));
			this.assign_function(new XVar("logo"), new XVar("printProjectLogo"), (XVar)(XVar.Array()));
			this.assign_function(new XVar("home_link"), new XVar("printHomeLink"), (XVar)(XVar.Array()));
			this.assign_function(new XVar("file_url"), new XVar("getFileUrl"), (XVar)(XVar.Array()));
			this.assign_function(new XVar("jscaption"), new XVar("xt_jscaption"), (XVar)(XVar.Array()));
			this.assign_function(new XVar("jslabel"), new XVar("xt_jslabel"), (XVar)(XVar.Array()));
			this.assign_function(new XVar("pdf_image"), new XVar("getPdfImageObject"), (XVar)(XVar.Array()));
			this.assign_function(new XVar("pdf_chart"), new XVar("getPdfChartObject"), (XVar)(XVar.Array()));
			this.assign_method(new XVar("map"), this, new XVar("xt_event_map"), (XVar)(XVar.Array()));
			this.assign(new XVar("projectPath"), (XVar)(MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.projectPath()))));
			this.assign_method(new XVar("mlp_push"), this, new XVar("mlpPush"), (XVar)(XVar.Array()));
			this.assign_method(new XVar("mlp_pop"), this, new XVar("mlpPop"), (XVar)(XVar.Array()));
			this.assign_method(new XVar("mlparam"), this, new XVar("mlpParam"), (XVar)(XVar.Array()));
			this.assign_method(new XVar("mltext"), this, new XVar("mlText"), (XVar)(XVar.Array()));
			this.assign_method(new XVar("messagepart"), this, new XVar("messagePart"), (XVar)(XVar.Array()));
			html_attrs = new XVar("");
			if(XVar.Pack(CommonFunctions.isRTL()))
			{
				this.assign(new XVar("RTL_block"), new XVar(true));
				this.assign(new XVar("rtlCSS"), new XVar(true));
				html_attrs = MVCFunctions.Concat(html_attrs, "dir=\"RTL\" ");
			}
			else
			{
				this.assign(new XVar("LTR_block"), new XVar(true));
			}
			if(CommonFunctions.mlang_getcurrentlang() == "English")
			{
				html_attrs = MVCFunctions.Concat(html_attrs, "lang=\"en\"");
			}
			this.assign(new XVar("html_attrs"), (XVar)(html_attrs));
			this.assign(new XVar("menu_block"), new XVar(true));
		}
		public virtual XVar assign(dynamic _param_name, dynamic _param_val)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic val = XVar.Clone(_param_val);
			#endregion

			this.xt_vars.InitAndSetArrayItem(val, name);

			return null;
		}
		public virtual XVar assignbyref(dynamic _param_name, dynamic var_var)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			this.xt_vars.InitAndSetArrayItem(var_var, name);

			return null;
		}
		public virtual XVar bulk_assign(dynamic _param_arr)
		{
			#region pass-by-value parameters
			dynamic arr = XVar.Clone(_param_arr);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> value in arr.GetEnumerator())
			{
				this.xt_vars.InitAndSetArrayItem(value.Value, value.Key);
			}

			return null;
		}
		public virtual XVar enable_section(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			if(XVar.Pack(!(XVar)(this.xt_vars.KeyExists(name))))
			{
				this.xt_vars.InitAndSetArrayItem(true, name);
			}
			else
			{
				if(this.xt_vars[name] == false)
				{
					this.xt_vars.InitAndSetArrayItem(true, name);
				}
			}

			return null;
		}
		public virtual XVar assign_section(dynamic _param_name, dynamic _param_begin, dynamic _param_end)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic begin = XVar.Clone(_param_begin);
			dynamic var_end = XVar.Clone(_param_end);
			#endregion

			dynamic arr = XVar.Array();
			arr = XVar.Clone(XVar.Array());
			arr.InitAndSetArrayItem(begin, "begin");
			arr.InitAndSetArrayItem(var_end, "end");
			this.xt_vars.InitAndSetArrayItem(arr, name);

			return null;
		}
		public virtual XVar assign_loopsection(dynamic _param_name, dynamic data)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			dynamic arr = XVar.Array();
			arr = XVar.Clone(XVar.Array());
			arr.InitAndSetArrayItem(data, "data");
			this.xt_vars.InitAndSetArrayItem(arr, name);

			return null;
		}
		public virtual XVar assign_array(dynamic _param_name, dynamic _param_innername, dynamic _param__arr)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic innername = XVar.Clone(_param_innername);
			dynamic _arr = XVar.Clone(_param__arr);
			#endregion

			dynamic arr = XVar.Array();
			arr = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> a in _arr.GetEnumerator())
			{
				arr.InitAndSetArrayItem(new XVar(innername, a.Value), null);
			}
			this.xt_vars.InitAndSetArrayItem(new XVar("data", arr), name);

			return null;
		}
		public virtual XVar assign_loopsection_byValue(dynamic _param_name, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic arr = XVar.Array();
			arr = XVar.Clone(XVar.Array());
			arr.InitAndSetArrayItem(data, "data");
			this.xt_vars.InitAndSetArrayItem(arr, name);

			return null;
		}
		public virtual XVar assign_function(dynamic _param_name, dynamic _param_func, dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic func = XVar.Clone(_param_func);
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.xt_vars.InitAndSetArrayItem(XTempl.create_function_assignment((XVar)(func), (XVar)(var_params)), name);

			return null;
		}
		public static XVar create_function_assignment(dynamic _param_func, dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic func = XVar.Clone(_param_func);
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			return new XVar("func", func, "params", var_params);
		}
		public virtual XVar assign_method(dynamic _param_name, dynamic var_object, dynamic _param_method, dynamic _param_params = null)
		{
			#region default values
			if(_param_params as Object == null) _param_params = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic method = XVar.Clone(_param_method);
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.xt_vars.InitAndSetArrayItem(XTempl.create_method_assignment((XVar)(method), (XVar)(var_object), (XVar)(var_params)), name);

			return null;
		}
		public static XVar create_method_assignment(dynamic _param_method, dynamic var_object, dynamic _param_params = null)
		{
			#region default values
			if(_param_params as Object == null) _param_params = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic method = XVar.Clone(_param_method);
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			return new XVar("method", method, "params", var_params, "object", var_object);
		}
		public virtual XVar unassign(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			this.xt_vars.Remove(name);

			return null;
		}
		public virtual XVar assign_event(dynamic _param_name, dynamic var_object, dynamic _param_method, dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic method = XVar.Clone(_param_method);
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.xt_events.InitAndSetArrayItem(new XVar("method", method, "params", var_params), name);
			this.xt_events.InitAndSetArrayItem(var_object, name, "object");

			return null;
		}
		public virtual XVar xt_event(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			if(XVar.Pack(this.jsonMode))
			{
				dynamic var_out = null;
				MVCFunctions.ob_start();
				this.xt_doevent((XVar)(var_params));
				var_out = XVar.Clone(CommonFunctions.jsreplace((XVar)(MVCFunctions.ob_get_contents())));
				MVCFunctions.ob_end_clean();
				MVCFunctions.Echo(var_out);
				return null;
			}
			return this.xt_doevent((XVar)(var_params));
		}
		public virtual XVar customLabel(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic ret = null;
			ret = XVar.Clone(CommonFunctions.GetCustomLabel((XVar)(var_params["custom1"])));
			ret = XVar.Clone(this.processMessageParams((XVar)(ret)));
			if(XVar.Pack(this.jsonMode))
			{
				ret = XVar.Clone(CommonFunctions.jsreplace((XVar)(ret)));
			}
			MVCFunctions.Echo(ret);

			return null;
		}
		public virtual XVar xt_event_map(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic var_out = null;
			if(XVar.Pack(!(XVar)(this.jsonMode)))
			{
				return this.xt_doevent((XVar)(var_params));
			}
			MVCFunctions.ob_start();
			this.xt_doevent((XVar)(var_params));
			var_out = XVar.Clone(MVCFunctions.ob_get_contents());
			MVCFunctions.ob_end_clean();
			MVCFunctions.Echo(var_out);

			return null;
		}
		public virtual XVar xt_doevent(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic eventName = null, eventObj = null, funcName = null;
			if(XVar.Pack(this.xt_events.KeyExists(var_params["custom1"])))
			{
				dynamic eventArr = XVar.Array();
				eventArr = XVar.Clone(this.xt_events[var_params["custom1"]]);
				if(XVar.Pack(eventArr.KeyExists("method")))
				{
					dynamic method = null;
					var_params = XVar.Clone(XVar.Array());
					if(XVar.Pack(eventArr.KeyExists("params")))
					{
						var_params = XVar.Clone(eventArr["params"]);
					}
					method = XVar.Clone(eventArr["method"]);
					eventArr["object"].Invoke(method, (XVar)(var_params));
					return null;
				}
			}
			if(XVar.Pack(this.eventsObject))
			{
				eventObj = XVar.Clone(this.eventsObject);
			}
			else
			{
				eventObj = XVar.Clone(GlobalVars.globalEvents);
			}
			if(XVar.Pack(!(XVar)(eventObj)))
			{
				return null;
			}
			eventName = XVar.Clone(var_params["custom1"]);
			if(XVar.Pack(!(XVar)(eventObj.snippetExists((XVar)(eventName)))))
			{
				return null;
			}
			funcName = XVar.Clone(MVCFunctions.Concat("event_", eventName));
			eventObj.Invoke(funcName, (XVar)(var_params));

			return null;
		}
		public virtual XVar fetchVar(dynamic _param_varName)
		{
			#region pass-by-value parameters
			dynamic varName = XVar.Clone(_param_varName);
			#endregion

			dynamic varParams = null, var_out = null;
			MVCFunctions.ob_start();
			varParams = XVar.Clone(XVar.Array());
			this.processVar((XVar)(this.getVar((XVar)(varName))), (XVar)(varParams));
			var_out = XVar.Clone(MVCFunctions.ob_get_contents());
			MVCFunctions.ob_end_clean();
			return var_out;
		}
		public virtual XVar fetch_loadedJSON(dynamic _param_filtertag = null)
		{
			#region default values
			if(_param_filtertag as Object == null) _param_filtertag = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic filtertag = XVar.Clone(_param_filtertag);
			#endregion

			this.jsonMode = new XVar(true);
			return this.fetch_loaded((XVar)(filtertag));
		}
		public virtual XVar fetch_loaded(dynamic _param_filtertag = null)
		{
			#region default values
			if(_param_filtertag as Object == null) _param_filtertag = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic filtertag = XVar.Clone(_param_filtertag);
			#endregion

			dynamic var_out = null;
			MVCFunctions.ob_start();
			this.display_loaded((XVar)(filtertag));
			var_out = XVar.Clone(MVCFunctions.ob_get_contents());
			MVCFunctions.ob_end_clean();
			return var_out;
		}
		public virtual XVar call_func(dynamic _param_var)
		{
			#region pass-by-value parameters
			dynamic var_var = XVar.Clone(_param_var);
			#endregion


			return null;
		}
		public virtual XVar set_template(dynamic _param_template)
		{
			#region pass-by-value parameters
			dynamic template = XVar.Clone(_param_template);
			#endregion

			dynamic templatesPath = null;
			this.template_file = XVar.Clone(MVCFunctions.basename((XVar)(template), new XVar(".htm")));
			this.set_layout();
			templatesPath = new XVar("templates/");
			if(XVar.Pack(this.jsonMode))
			{
				templatesPath = new XVar("pdf/");
				template = XVar.Clone(MVCFunctions.Concat(this.template_file, ".json"));
			}
			if(XVar.Pack(MVCFunctions.file_exists((XVar)(MVCFunctions.getabspath((XVar)(MVCFunctions.Concat(templatesPath, template)))))))
			{
				this.template = XVar.Clone(MVCFunctions.myfile_get_contents((XVar)(MVCFunctions.getabspath((XVar)(MVCFunctions.Concat(templatesPath, template)))), new XVar("r")));
			}
			this.assign_headers();

			return null;
		}
		public virtual XVar set_layout()
		{
			this.layout = XVar.Clone(CommonFunctions.getLayoutByFilename((XVar)(this.template_file)));

			return null;
		}
		public virtual XVar prepare_template(dynamic _param_template)
		{
			#region pass-by-value parameters
			dynamic template = XVar.Clone(_param_template);
			#endregion

			this.prepareContainers();

			return null;
		}
		public virtual XVar load_templateJSON(dynamic _param_template)
		{
			#region pass-by-value parameters
			dynamic template = XVar.Clone(_param_template);
			#endregion

			this.jsonMode = new XVar(true);
			this.load_template((XVar)(template));

			return null;
		}
		public virtual XVar display_loaded(dynamic _param_filtertag = null)
		{
			#region default values
			if(_param_filtertag as Object == null) _param_filtertag = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic filtertag = XVar.Clone(_param_filtertag);
			#endregion

			dynamic str = null;
			str = XVar.Clone(this.template);
			if(XVar.Pack(filtertag))
			{
				dynamic pos1 = null, pos2 = null;
				if((XVar)(!(XVar)(this.xt_vars.KeyExists(filtertag)))  || (XVar)(XVar.Equals(XVar.Pack(this.xt_vars[filtertag]), XVar.Pack(false))))
				{
					return null;
				}
				pos1 = XVar.Clone(MVCFunctions.strpos((XVar)(this.template), (XVar)(MVCFunctions.Concat("{BEGIN ", filtertag, "}"))));
				pos2 = XVar.Clone(MVCFunctions.strpos((XVar)(this.template), (XVar)(MVCFunctions.Concat("{END ", filtertag, "}"))));
				if((XVar)(XVar.Equals(XVar.Pack(pos1), XVar.Pack(false)))  || (XVar)(XVar.Equals(XVar.Pack(pos2), XVar.Pack(false))))
				{
					return null;
				}
				pos2 += MVCFunctions.strlen((XVar)(MVCFunctions.Concat("{END ", filtertag, "}")));
				str = XVar.Clone(MVCFunctions.substr((XVar)(this.template), (XVar)(pos1), (XVar)(pos2 - pos1)));
			}
			this.Testing();
			this.process_template((XVar)(str));

			return null;
		}
		public virtual XVar load_template(dynamic _param_template)
		{
			#region pass-by-value parameters
			dynamic template = XVar.Clone(_param_template);
			#endregion

			this.set_template((XVar)(template));
			this.prepareContainers();

			return null;
		}
		public virtual XVar display(dynamic _param_template)
		{
			#region pass-by-value parameters
			dynamic template = XVar.Clone(_param_template);
			#endregion


			return null;
		}
		public virtual XVar displayJSON(dynamic _param_template)
		{
			#region pass-by-value parameters
			dynamic template = XVar.Clone(_param_template);
			#endregion

			this.jsonMode = new XVar(true);
			this.display((XVar)(template));

			return null;
		}
		public virtual XVar displayPartial(dynamic _param_template)
		{
			#region pass-by-value parameters
			dynamic template = XVar.Clone(_param_template);
			#endregion

			dynamic savedTemplate = null;
			savedTemplate = XVar.Clone(this.template);
			this.display((XVar)(template));
			this.template = XVar.Clone(savedTemplate);

			return null;
		}
		public virtual XVar processVar(dynamic var_var, dynamic varparams)
		{

			return null;
		}
		public virtual XVar displayItemsHidden(dynamic _param_items, dynamic _param_recordId = null)
		{
			#region default values
			if(_param_recordId as Object == null) _param_recordId = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic items = XVar.Clone(_param_items);
			dynamic recordId = XVar.Clone(_param_recordId);
			#endregion

			if(XVar.Pack(!(XVar)(items)))
			{
				return null;
			}
			foreach (KeyValuePair<XVar, dynamic> name in items.GetEnumerator())
			{
				this.displayItemHidden((XVar)(name.Value), (XVar)(recordId));
			}

			return null;
		}
		public virtual XVar showHiddenItems(dynamic _param_items)
		{
			#region pass-by-value parameters
			dynamic items = XVar.Clone(_param_items);
			#endregion

			if(XVar.Pack(!(XVar)(items)))
			{
				return null;
			}
			foreach (KeyValuePair<XVar, dynamic> name in items.GetEnumerator())
			{
				this.hiddenItems.Remove(name.Value);
			}

			return null;
		}
		public virtual XVar displayItemHidden(dynamic _param_name, dynamic _param_recordId = null)
		{
			#region default values
			if(_param_recordId as Object == null) _param_recordId = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic recordId = XVar.Clone(_param_recordId);
			#endregion

			if(XVar.Pack(!(XVar)(recordId)))
			{
				this.hiddenItems.InitAndSetArrayItem(true, name);
			}
			else
			{
				if(XVar.Pack(!(XVar)(this.hiddenRecordItems.KeyExists(name))))
				{
					this.hiddenRecordItems.InitAndSetArrayItem(XVar.Array(), name);
				}
				this.hiddenRecordItems.InitAndSetArrayItem(true, name, recordId);
			}

			return null;
		}
		public virtual XVar showHiddenItem(dynamic _param_name, dynamic _param_recordId = null)
		{
			#region default values
			if(_param_recordId as Object == null) _param_recordId = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic recordId = XVar.Clone(_param_recordId);
			#endregion

			if(XVar.Pack(!(XVar)(recordId)))
			{
				this.hiddenItems.Remove(name);
			}
			else
			{
				if(XVar.Pack(this.hiddenRecordItems.KeyExists(name)))
				{
					this.hiddenRecordItems[name].Remove(recordId);
				}
			}

			return null;
		}
		public virtual XVar hideAllBricksExcept(dynamic _param_arrExceptBricks)
		{
			#region pass-by-value parameters
			dynamic arrExceptBricks = XVar.Clone(_param_arrExceptBricks);
			#endregion


			return null;
		}
		public virtual XVar showBrick(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			if(this.layout.version == Constants.PD_BS_LAYOUT)
			{
				return null;
			}
			foreach (KeyValuePair<XVar, dynamic> container in this.layout.containers.GetEnumerator())
			{
				foreach (KeyValuePair<XVar, dynamic> brick in container.Value.GetEnumerator())
				{
					if(brick.Value["name"] == name)
					{
						this.assign((XVar)(brick.Value["block"]), new XVar(true));
					}
				}
			}

			return null;
		}
		private XVar setContainerDisplayed(dynamic _param_cname, dynamic _param_show, dynamic _param_firstContainerSubstyle, dynamic _param_lastContainerSubstyle)
		{
			#region pass-by-value parameters
			dynamic cname = XVar.Clone(_param_cname);
			dynamic show = XVar.Clone(_param_show);
			dynamic firstContainerSubstyle = XVar.Clone(_param_firstContainerSubstyle);
			dynamic lastContainerSubstyle = XVar.Clone(_param_lastContainerSubstyle);
			#endregion

			dynamic styleString = null;
			if(this.layout.version == Constants.BOOTSTRAP_LAYOUT)
			{
				this.assign((XVar)(MVCFunctions.Concat("container_", cname)), new XVar(true));
				if(XVar.Pack(!(XVar)(show)))
				{
					this.assign((XVar)(MVCFunctions.Concat(cname, "_chiddenattr")), new XVar("data-hidden"));
				}
				return null;
			}
			this.prepareContainerAttrs((XVar)(cname));
			if(XVar.Pack(show))
			{
				styleString = XVar.Clone(this.preparedContainers[cname]["showString"]);
				this.unassign((XVar)(MVCFunctions.Concat("wrapperclass_", cname)));
			}
			else
			{
				styleString = XVar.Clone(this.preparedContainers[cname]["hideString"]);
				this.assign((XVar)(MVCFunctions.Concat("wrapperclass_", cname)), new XVar("rnr-hiddencontainer"));
			}
			this.assign_section((XVar)(MVCFunctions.Concat("container_", cname)), (XVar)(MVCFunctions.Concat("<div ", styleString, ">")), new XVar("</div>"));
			this.assign((XVar)(MVCFunctions.Concat("cheaderclass_", cname)), (XVar)(firstContainerSubstyle));
			this.assign((XVar)(MVCFunctions.Concat("cfooterclass_", cname)), (XVar)(lastContainerSubstyle));

			return null;
		}
		private XVar getPageStyle()
		{
			if(XVar.Pack(MVCFunctions.postvalue(new XVar("pdf"))))
			{
				return this.layout.pdfStyle();
			}
			return this.layout.style;
		}
		private XVar prepareContainerAttrs(dynamic _param_cname)
		{
			#region pass-by-value parameters
			dynamic cname = XVar.Clone(_param_cname);
			#endregion

			dynamic hiddenStyleString = null, pageStyle = null, styleString = null;
			pageStyle = XVar.Clone(this.getPageStyle());
			if(XVar.Pack(this.preparedContainers.KeyExists(cname)))
			{
				return null;
			}
			this.preparedContainers.InitAndSetArrayItem(XVar.Array(), cname);
			hiddenStyleString = new XVar("");
			styleString = new XVar("");
			if(XVar.Pack(this.layout.skins.KeyExists(cname)))
			{
				dynamic buttonsClass = null, buttonsType = null, printClass = null, printMode = null, skin = null;
				skin = XVar.Clone(this.layout.skins[cname]);
				buttonsType = XVar.Clone(this.layout.skinsparams[skin]["button"]);
				buttonsClass = XVar.Clone((XVar.Pack(buttonsType == "button2") ? XVar.Pack(" aslinks") : XVar.Pack(" asbuttons")));
				printMode = XVar.Clone(this.layout.container_properties[cname]["print"]);
				printClass = new XVar("");
				if(printMode == "repeat")
				{
					printClass = new XVar(" rp-repeat");
				}
				else
				{
					if(printMode == "none")
					{
						printClass = new XVar(" rp-noprint");
					}
				}
				if(this.layout.version == 1)
				{
					styleString = XVar.Clone(MVCFunctions.Concat(" class=\"rnr-cw-", cname, " runner-s-", skin, " ", pageStyle));
				}
				else
				{
					styleString = XVar.Clone(MVCFunctions.Concat(" class=\"rnr-cw-", cname, " rnr-s-", skin, buttonsClass, " ", pageStyle, printClass));
				}
				hiddenStyleString = XVar.Clone(MVCFunctions.Concat(styleString, " rnr-hiddencontainer"));
				styleString = MVCFunctions.Concat(styleString, "\"");
				hiddenStyleString = MVCFunctions.Concat(hiddenStyleString, "\"");
				this.preparedContainers.InitAndSetArrayItem(new XVar("showString", styleString, "hideString", hiddenStyleString), cname);
			}

			return null;
		}
		public virtual XVar prepareContainers()
		{
			if(XVar.Pack(!(XVar)(this.layout)))
			{
				return null;
			}
			this.layout.prepareForms(this, (XVar)(this.hiddenItems), (XVar)(this.hiddenRecordItems), (XVar)(this.pageObject));

			return null;
		}
		public virtual XVar hideField(dynamic _param_fieldName)
		{
			#region pass-by-value parameters
			dynamic fieldName = XVar.Clone(_param_fieldName);
			#endregion

			if(this.layout.version == 1)
			{
				this.assign((XVar)(MVCFunctions.Concat("fielddispclass_", MVCFunctions.GoodFieldName((XVar)(fieldName)))), new XVar("runner-hiddenfield"));
			}
			else
			{
				this.assign((XVar)(MVCFunctions.Concat("fielddispclass_", MVCFunctions.GoodFieldName((XVar)(fieldName)))), new XVar("rnr-hiddenfield"));
			}

			return null;
		}
		public virtual XVar showField(dynamic _param_fieldName)
		{
			#region pass-by-value parameters
			dynamic fieldName = XVar.Clone(_param_fieldName);
			#endregion

			this.assign((XVar)(MVCFunctions.Concat("fielddispclass_", MVCFunctions.GoodFieldName((XVar)(fieldName)))), new XVar(""));

			return null;
		}
		public virtual XVar setPage(dynamic _param_page)
		{
			#region pass-by-value parameters
			dynamic page = XVar.Clone(_param_page);
			#endregion

			this.pageObject = XVar.Clone(page);

			return null;
		}
		public virtual XVar transformFuncParams(dynamic _param_varparams, dynamic var_params)
		{
			#region pass-by-value parameters
			dynamic varparams = XVar.Clone(_param_varparams);
			#endregion

			dynamic key = null;
			key = new XVar(1);
			foreach (KeyValuePair<XVar, dynamic> val in varparams.GetEnumerator())
			{
				if(XVar.Pack(MVCFunctions.strlen((XVar)(val.Value))))
				{
					var_params.InitAndSetArrayItem(val.Value, MVCFunctions.Concat("custom", key++));
				}
			}
			return var_params;
		}
		public virtual XVar mlpPush(dynamic _param_params = null)
		{
			#region default values
			if(_param_params as Object == null) _param_params = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.messageParamStack.InitAndSetArrayItem(XVar.Array(), null);

			return null;
		}
		public virtual XVar mlpPop(dynamic _param_params = null)
		{
			#region default values
			if(_param_params as Object == null) _param_params = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			MVCFunctions.array_pop((XVar)(this.messageParamStack));

			return null;
		}
		public virtual XVar mlpHead()
		{
			if(XVar.Pack(this.messageParamStack))
			{
				return this.messageParamStack[MVCFunctions.count(this.messageParamStack) - 1];
			}
			return XVar.Array();
		}
		public virtual XVar processMessageParams(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic matches = XVar.Array(), offset = null, pattern = null, stackFrame = XVar.Array(), var_out = null;
			stackFrame = this.mlpHead();
			if(XVar.Pack(!(XVar)(stackFrame)))
			{
				return str;
			}
			pattern = new XVar("/\\%(\\w)+\\%/");
			matches = XVar.Clone(MVCFunctions.findMatches((XVar)(pattern), (XVar)(str)));
			var_out = new XVar("");
			offset = new XVar(0);
			foreach (KeyValuePair<XVar, dynamic> m in matches.GetEnumerator())
			{
				dynamic paramName = null;
				var_out = MVCFunctions.Concat(var_out, MVCFunctions.substr((XVar)(str), (XVar)(offset), (XVar)(m.Value["offset"] - offset)));
				paramName = XVar.Clone(MVCFunctions.substr((XVar)(m.Value["match"]), new XVar(1), (XVar)(MVCFunctions.strlen((XVar)(m.Value["match"])) - 2)));
				offset = XVar.Clone((m.Value["offset"] + MVCFunctions.strlen((XVar)(paramName))) + 2);
				var_out = MVCFunctions.Concat(var_out, stackFrame[paramName]);
			}
			var_out = MVCFunctions.Concat(var_out, MVCFunctions.substr((XVar)(str), (XVar)(offset)));
			return var_out;
		}
		public virtual XVar mlpParam(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic name = null, stackFrame = XVar.Array(), value = null, var_type = null;
			stackFrame = this.mlpHead();
			var_type = XVar.Clone(var_params["custom2"]);
			name = XVar.Clone(var_params["custom1"]);
			value = new XVar("");
			if(var_type == "text")
			{
				dynamic text = null;
				text = XVar.Clone(var_params["custom3"]);
				stackFrame.InitAndSetArrayItem(XTempl_Base.unmaskText((XVar)(text)), name);
			}
			if(var_type == "custom")
			{
				dynamic labelName = null;
				labelName = XVar.Clone(var_params["custom3"]);
				stackFrame.InitAndSetArrayItem(CommonFunctions.GetCustomLabel((XVar)(labelName)), name);
			}
			else
			{
				if(var_type == "var")
				{
					dynamic varName = null;
					varName = XVar.Clone(var_params["custom3"]);
					stackFrame.InitAndSetArrayItem(this.getVar((XVar)(varName)), name);
				}
			}

			return null;
		}
		public static XVar unmaskText(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			return MVCFunctions.str_replace((XVar)(new XVar(0, "\\_", 1, "\\[", 2, "\\]", 3, "\\\\")), (XVar)(new XVar(0, " ", 1, "{", 2, "}", 3, "\\")), (XVar)(str));

			return null;
		}
		public virtual XVar mlText(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic text = null;
			text = XVar.Clone(var_params["custom1"]);
			MVCFunctions.Echo(this.processMessageParams((XVar)(XTempl_Base.unmaskText((XVar)(text)))));

			return null;
		}
		public virtual XVar messagePart(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic fullMessage = null, i = null, m = XVar.Array(), matches = XVar.Array(), partN = null, partOffset = null, parts = XVar.Array(), pattern = null, tag = null;
			tag = XVar.Clone(var_params["custom1"]);
			partN = XVar.Clone(var_params["custom2"]);
			fullMessage = XVar.Clone(CommonFunctions.mlang_message((XVar)(tag)));
			pattern = new XVar("/%[^%\\W]+%/i");
			partOffset = new XVar(0);
			parts = XVar.Clone(XVar.Array());
			matches = XVar.Clone(MVCFunctions.findMatches((XVar)(pattern), (XVar)(fullMessage)));
			i = new XVar(0);
			for(;i < MVCFunctions.count(matches); ++(i))
			{
				m = XVar.Clone(matches[i]);
				if(0 < m["offset"])
				{
					parts.InitAndSetArrayItem(MVCFunctions.substr((XVar)(fullMessage), (XVar)(partOffset), (XVar)(m["offset"] - partOffset)), null);
				}
				partOffset = XVar.Clone(m["offset"] + MVCFunctions.strlen((XVar)(m["match"])));
			}
			if(partOffset < MVCFunctions.strlen((XVar)(fullMessage)))
			{
				parts.InitAndSetArrayItem(MVCFunctions.substr((XVar)(fullMessage), (XVar)(partOffset)), null);
			}
			if(partN <= MVCFunctions.count(parts))
			{
				MVCFunctions.Echo(parts[partN - 1]);
			}
			MVCFunctions.Echo("");

			return null;
		}
		public virtual XVar process_template(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic endpos = null, len = null, literal = null, message = null, pos = null, section = null, start = null, var_var = XVar.Array(), varparams = XVar.Array();
			varparams = XVar.Clone(XVar.Array());
			start = new XVar(0);
			literal = new XVar(false);
			len = XVar.Clone(MVCFunctions.strlen((XVar)(str)));
			while(XVar.Pack(true))
			{
				pos = XVar.Clone(MVCFunctions.strpos((XVar)(str), new XVar("{"), (XVar)(start)));
				if(XVar.Equals(XVar.Pack(pos), XVar.Pack(false)))
				{
					MVCFunctions.Echo(MVCFunctions.substr((XVar)(str), (XVar)(start), (XVar)(len - start)));
					break;
				}
				section = new XVar(false);
				var_var = new XVar(false);
				message = new XVar(false);
				if(MVCFunctions.substr((XVar)(str), (XVar)(pos + 1), new XVar(6)) == "BEGIN ")
				{
					section = new XVar(true);
				}
				else
				{
					if(MVCFunctions.substr((XVar)(str), (XVar)(pos + 1), new XVar(1)) == "$")
					{
						var_var = new XVar(true);
					}
					else
					{
						if(MVCFunctions.substr((XVar)(str), (XVar)(pos + 1), new XVar(14)) == "mlang_message ")
						{
							message = new XVar(true);
						}
						else
						{
							MVCFunctions.Echo(MVCFunctions.substr((XVar)(str), (XVar)(start), (XVar)((pos - start) + 1)));
							start = XVar.Clone(pos + 1);
							continue;
						}
					}
				}
				MVCFunctions.Echo(MVCFunctions.substr((XVar)(str), (XVar)(start), (XVar)(pos - start)));
				if(XVar.Pack(section))
				{
					dynamic begin = null, endpos1 = null, endtag = null, sectionVar = XVar.Array(), section_name = null, var_end = null;
					endpos = XVar.Clone(MVCFunctions.strpos((XVar)(str), new XVar("}"), (XVar)(pos)));
					if(XVar.Equals(XVar.Pack(endpos), XVar.Pack(false)))
					{
						this.report_error(new XVar("Page is broken"));
						return null;
					}
					section_name = XVar.Clone(MVCFunctions.trim((XVar)(MVCFunctions.substr((XVar)(str), (XVar)(pos + 7), (XVar)((endpos - pos) - 7)))));
					endtag = XVar.Clone(MVCFunctions.Concat("{END ", section_name, "}"));
					endpos1 = XVar.Clone(MVCFunctions.strpos((XVar)(str), (XVar)(endtag), (XVar)(endpos)));
					if(XVar.Equals(XVar.Pack(endpos1), XVar.Pack(false)))
					{
						MVCFunctions.Echo(MVCFunctions.Concat("End tag not found:", MVCFunctions.runner_htmlspecialchars((XVar)(endtag))));
						this.report_error(new XVar("Page is broken"));
						return null;
					}
					section = XVar.Clone(MVCFunctions.substr((XVar)(str), (XVar)(endpos + 1), (XVar)((endpos1 - endpos) - 1)));
					start = XVar.Clone(endpos1 + MVCFunctions.strlen((XVar)(endtag)));
					sectionVar = XVar.Clone(MVCFunctions.xt_getvar(this, (XVar)(section_name)));
					if(XVar.Equals(XVar.Pack(sectionVar), XVar.Pack(false)))
					{
						continue;
					}
					begin = new XVar("");
					var_end = new XVar("");
					if(XVar.Pack(MVCFunctions.is_array((XVar)(sectionVar))))
					{
						begin = XVar.Clone(sectionVar["begin"]);
						var_end = XVar.Clone(sectionVar["end"]);
						var_var = XVar.Clone(sectionVar["data"]);
					}
					else
					{
						var_var = XVar.Clone(sectionVar);
					}
					if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(var_var)))))
					{
						MVCFunctions.Echo(begin);
						if(XVar.Pack(MVCFunctions.is_array((XVar)(sectionVar))))
						{
							this.xt_stack.InitAndSetArrayItem(sectionVar, null);
						}
						this.process_template((XVar)(section));
						if(XVar.Pack(MVCFunctions.is_array((XVar)(sectionVar))))
						{
							MVCFunctions.array_pop((XVar)(this.xt_stack));
						}
						this.processVar((XVar)(var_end), (XVar)(varparams));
					}
					else
					{
						dynamic keys = XVar.Array();
						MVCFunctions.Echo(begin);
						keys = XVar.Clone(MVCFunctions.array_keys((XVar)(var_var)));
						foreach (KeyValuePair<XVar, dynamic> i in keys.GetEnumerator())
						{
							this.xt_stack.InitAndSetArrayItem(var_var[i.Value], null);
							if((XVar)(MVCFunctions.is_array((XVar)(var_var[i.Value])))  && (XVar)(var_var[i.Value].KeyExists("begin")))
							{
								MVCFunctions.Echo(var_var[i.Value]["begin"]);
							}
							this.process_template((XVar)(section));
							MVCFunctions.array_pop((XVar)(this.xt_stack));
							if((XVar)(MVCFunctions.is_array((XVar)(var_var[i.Value])))  && (XVar)(var_var[i.Value].KeyExists("end")))
							{
								MVCFunctions.Echo(var_var[i.Value]["end"]);
							}
						}
						this.processVar((XVar)(var_end), (XVar)(varparams));
					}
				}
				else
				{
					if(XVar.Pack(var_var))
					{
						dynamic var_name = null;
						endpos = XVar.Clone(MVCFunctions.strpos((XVar)(str), new XVar("}"), (XVar)(pos)));
						if(XVar.Equals(XVar.Pack(endpos), XVar.Pack(false)))
						{
							this.report_error(new XVar("Page is broken"));
							return null;
						}
						varparams = XVar.Clone(XVar.Array());
						var_name = XVar.Clone(MVCFunctions.trim((XVar)(MVCFunctions.substr((XVar)(str), (XVar)(pos + 2), (XVar)((endpos - pos) - 2)))));
						if(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(var_name), new XVar(" "))), XVar.Pack(false)))
						{
							varparams = XVar.Clone(MVCFunctions.explode(new XVar(" "), (XVar)(var_name)));
							var_name = XVar.Clone(varparams[0]);
							varparams.Remove(0);
						}
						start = XVar.Clone(endpos + 1);
						var_var = XVar.Clone(MVCFunctions.xt_getvar(this, (XVar)(var_name)));
						if(XVar.Equals(XVar.Pack(var_var), XVar.Pack(false)))
						{
							continue;
						}
						this.processVar((XVar)(var_var), (XVar)(varparams));
					}
					else
					{
						if(XVar.Pack(message))
						{
							dynamic tag = null;
							endpos = XVar.Clone(MVCFunctions.strpos((XVar)(str), new XVar("}"), (XVar)(pos)));
							if(XVar.Equals(XVar.Pack(endpos), XVar.Pack(false)))
							{
								this.report_error(new XVar("Page is broken"));
								return null;
							}
							tag = XVar.Clone(MVCFunctions.trim((XVar)(MVCFunctions.substr((XVar)(str), (XVar)(pos + 15), (XVar)((endpos - pos) - 15)))));
							start = XVar.Clone(endpos + 1);
							MVCFunctions.Echo(MVCFunctions.runner_htmlspecialchars((XVar)(this.processMessageParams((XVar)(CommonFunctions.mlang_message((XVar)(tag)))))));
						}
					}
				}
			}

			return null;
		}
	}
}
