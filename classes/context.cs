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
	public partial class RunnerContextItem : XClass
	{
		public dynamic var_type;
		public dynamic pageObj;
		public dynamic data;
		public dynamic oldData;
		public dynamic newData;
		public dynamic detailsKeys;
		public dynamic dc;
		public dynamic searchClause;
		public dynamic masterData;
		public RunnerContextItem(dynamic _param_type, dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			CommonFunctions.RunnerApply(this, (XVar)(var_params));
			this.var_type = XVar.Clone(var_type);
		}
		public virtual XVar getType()
		{
			return this.var_type;
		}
		public virtual XVar getValues()
		{
			if(XVar.Pack(this.data))
			{
				return this.data;
			}
			if(XVar.Pack(this.dc))
			{
				return this.dc.values;
			}
			if(XVar.Pack(this.pageObj))
			{
				return this.pageObj.getCurrentRecord();
			}
			return XVar.Array();
		}
		public virtual XVar getFieldValue(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic data = null;
			data = XVar.Clone(this.getValues());
			return CommonFunctions.getArrayElementNC((XVar)(data), (XVar)(field));
		}
		public virtual XVar getSearchValue(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.searchClause._getFieldValue((XVar)(field), new XVar(null), new XVar(false), new XVar(true));
		}
		public virtual XVar getAllSearchValue()
		{
			return this.searchClause.getAllFieldsSearchValue();
		}
		public virtual XVar getOldValues()
		{
			if(XVar.Pack(this.oldData))
			{
				return this.oldData;
			}
			if(XVar.Pack(this.pageObj))
			{
				return this.pageObj.getOldRecordData();
			}
			return XVar.Array();
		}
		public virtual XVar getKeyValue(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			if(XVar.Pack(this.dc))
			{
				return this.dc.keys[field];
			}
			return null;
		}
		public virtual XVar getOrderValue(dynamic _param_key)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			#endregion

			dynamic idx = null, order = XVar.Array(), param = null;
			if(XVar.Pack(!(XVar)(this.dc)))
			{
				return null;
			}
			idx = new XVar(0);
			if(MVCFunctions.substr((XVar)(key), new XVar(0), new XVar(5)) == "field")
			{
				param = new XVar("field");
				idx = XVar.Clone((int)MVCFunctions.substr((XVar)(key), new XVar(5)));
			}
			else
			{
				if(MVCFunctions.substr((XVar)(key), new XVar(0), new XVar(3)) == "dir")
				{
					param = new XVar("dir");
					idx = XVar.Clone((int)MVCFunctions.substr((XVar)(key), new XVar(3)));
				}
			}
			if((XVar)(!(XVar)(idx))  || (XVar)(idx < 1))
			{
				return null;
			}
			if(MVCFunctions.count(this.dc.order) <= idx)
			{
				return null;
			}
			order = XVar.Clone(this.dc.order[idx - 1]);
			if(param == "field")
			{
				return order["column"];
			}
			if(param == "dir")
			{
				return order["dir"];
			}
			return null;
		}
		public virtual XVar getFilterValue(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			if(XVar.Pack(this.dc))
			{
				return this.dc.findFieldFilterValue((XVar)(field));
			}

			return null;
		}
		public virtual XVar getLimitValue(dynamic _param_key)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			#endregion

			if(XVar.Pack(!(XVar)(this.dc)))
			{
				return "";
			}
			if(key == "record_from")
			{
				return this.dc.startRecord + 1;
			}
			if(key == "record_to")
			{
				return this.dc.startRecord + this.dc.reccount;
			}
			if(key == "record_count")
			{
				return this.dc.reccount;
			}
			if(key == "record_offset")
			{
				return this.dc.startRecord;
			}

			return null;
		}
		public virtual XVar getOldFieldValue(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic oldData = null;
			oldData = XVar.Clone(this.getOldValues());
			return CommonFunctions.getArrayElementNC((XVar)(oldData), (XVar)(field));
		}
		public virtual XVar getNewFieldValue(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			if(XVar.Pack(this.newData))
			{
				return CommonFunctions.getArrayElementNC((XVar)(this.newData), (XVar)(field));
			}
			if(XVar.Pack(this.dc))
			{
				return CommonFunctions.getArrayElementNC((XVar)(this.dc.values), (XVar)(field));
			}
			return this.getFieldValue((XVar)(field));
		}
		public virtual XVar getMasterValues()
		{
			if(XVar.Pack(this.masterData))
			{
				return this.masterData;
			}
			if(XVar.Pack(this.pageObj))
			{
				return this.pageObj.getMasterRecord();
			}
			return XVar.Array();
		}
		public virtual XVar getMasterFieldValue(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic masterData = null;
			masterData = XVar.Clone(this.getMasterValues());
			if(XVar.Pack(!(XVar)(masterData)))
			{
				return "";
			}
			return CommonFunctions.getArrayElementNC((XVar)(masterData), (XVar)(field));
		}
		public virtual XVar getUserValue(dynamic _param_key)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			#endregion

			return CommonFunctions.getArrayElementNC((XVar)(Security.currentUserData()), (XVar)(key));
		}
		public virtual XVar getSessionValue(dynamic _param_key)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			#endregion

			return CommonFunctions.getSessionElementNC((XVar)(key));
		}
		public virtual XVar getDetailsKeyValue(dynamic _param_key)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			#endregion

			return this.detailsKeys[key];
		}
		public virtual XVar hasScope(dynamic _param_scope)
		{
			#region pass-by-value parameters
			dynamic scope = XVar.Clone(_param_scope);
			#endregion

			if(scope == "master")
			{
				return (XVar)(this.masterData)  || (XVar)(this.var_type == Constants.CONTEXT_PAGE);
			}
			if(scope == "session")
			{
				return true;
			}
			if(scope == "user")
			{
				return true;
			}
			if(scope == "old")
			{
				return (XVar)(this.oldData)  || (XVar)(this.var_type == Constants.CONTEXT_PAGE);
			}
			if(scope == "keys")
			{
				return this.var_type == Constants.CONTEXT_COMMAND;
			}
			if(scope == "order")
			{
				return this.var_type == Constants.CONTEXT_COMMAND;
			}
			if(scope == "new")
			{
				return (XVar)((XVar)(this.newData)  || (XVar)(this.var_type == Constants.CONTEXT_PAGE))  || (XVar)(this.var_type == Constants.CONTEXT_COMMAND);
			}
			if(scope == "global")
			{
				return true;
			}
			if(scope == "details")
			{
				return (XVar)(this.var_type == Constants.CONTEXT_PAGE)  || (XVar)(this.var_type == Constants.CONTEXT_MASTER);
			}
			if(scope == "values")
			{
				return (XVar)((XVar)(!(XVar)(!(XVar)(this.data)))  || (XVar)(this.var_type == Constants.CONTEXT_PAGE))  || (XVar)(this.var_type == Constants.CONTEXT_COMMAND);
			}
			if(scope == "search")
			{
				return this.var_type == Constants.CONTEXT_SEARCH;
			}
			if(scope == "filter")
			{
				return this.var_type == Constants.CONTEXT_COMMAND;
			}
			if(scope == "request")
			{
				return true;
			}
			if(scope == "limit")
			{
				return this.var_type == Constants.CONTEXT_COMMAND;
			}
			if(scope == "all_field_search")
			{
				return this.var_type == Constants.CONTEXT_SEARCH;
			}

			return null;
		}
		public virtual XVar getContextValue(dynamic _param_scope, dynamic _param_key)
		{
			#region pass-by-value parameters
			dynamic scope = XVar.Clone(_param_scope);
			dynamic key = XVar.Clone(_param_key);
			#endregion

			if(scope == "master")
			{
				return this.getMasterFieldValue((XVar)(key));
			}
			if(scope == "session")
			{
				return this.getSessionValue((XVar)(key));
			}
			if(scope == "user")
			{
				return this.getUserValue((XVar)(key));
			}
			if(scope == "old")
			{
				return this.getOldFieldValue((XVar)(key));
			}
			if(scope == "keys")
			{
				return this.getKeyValue((XVar)(key));
			}
			if(scope == "order")
			{
				return this.getOrderValue((XVar)(key));
			}
			if(scope == "new")
			{
				return this.getNewFieldValue((XVar)(key));
			}
			if((XVar)(scope == "global")  && (XVar)(key == "language"))
			{
				return CommonFunctions.mlang_getcurrentlang();
			}
			if(scope == "details")
			{
				if(this.var_type == Constants.CONTEXT_PAGE)
				{
					return this.pageObj.getDetailsKeyValue((XVar)(key));
				}
				if(this.var_type == Constants.CONTEXT_MASTER)
				{
					return this.getDetailsKeyValue((XVar)(key));
				}
			}
			if(scope == "values")
			{
				return this.getFieldValue((XVar)(key));
			}
			if(scope == "search")
			{
				return this.getSearchValue((XVar)(key));
			}
			if(scope == "filter")
			{
				return this.getFilterValue((XVar)(key));
			}
			if(scope == "request")
			{
				return MVCFunctions.postvalue((XVar)(key));
			}
			if(scope == "limit")
			{
				return this.getLimitValue((XVar)(key));
			}
			if(scope == "all_field_search")
			{
				return this.getAllSearchValue();
			}
			return false;
		}
	}
	public partial class RunnerContext : XClass
	{
		protected dynamic stack = XVar.Array();
		public RunnerContext()
		{
			dynamic context = null;
			context = XVar.Clone(new RunnerContextItem(new XVar(Constants.CONTEXT_GLOBAL), (XVar)(XVar.Array())));
			this.stack.InitAndSetArrayItem(context, MVCFunctions.count(this.stack));
		}
		public static XVar push(dynamic _param_context)
		{
			#region pass-by-value parameters
			dynamic context = XVar.Clone(_param_context);
			#endregion

			GlobalVars.contextStack.stack.InitAndSetArrayItem(context, MVCFunctions.count(GlobalVars.contextStack.stack));

			return null;
		}
		public static XVar current()
		{
			return GlobalVars.contextStack.stack[MVCFunctions.count(GlobalVars.contextStack.stack) - 1];
		}
		public static XVar pop()
		{
			dynamic context = null;
			if(XVar.Pack(!(XVar)(GlobalVars.contextStack.stack)))
			{
				return null;
			}
			context = XVar.Clone(GlobalVars.contextStack.stack[MVCFunctions.count(GlobalVars.contextStack.stack) - 1]);
			GlobalVars.contextStack.stack.Remove(MVCFunctions.count(GlobalVars.contextStack.stack) - 1);
			return context;
		}
		public static XVar pushPageContext(dynamic _param_pageObj)
		{
			#region pass-by-value parameters
			dynamic pageObj = XVar.Clone(_param_pageObj);
			#endregion

			RunnerContext.push((XVar)(new RunnerContextItem(new XVar(Constants.CONTEXT_PAGE), (XVar)(new XVar("pageObj", pageObj)))));

			return null;
		}
		public static XVar pushRecordContext(dynamic _param_record, dynamic _param_pageObj)
		{
			#region pass-by-value parameters
			dynamic record = XVar.Clone(_param_record);
			dynamic pageObj = XVar.Clone(_param_pageObj);
			#endregion

			RunnerContext.push((XVar)(new RunnerContextItem(new XVar(Constants.CONTEXT_ROW), (XVar)(new XVar("pageObj", pageObj, "data", record)))));

			return null;
		}
		public static XVar pushDataCommandContext(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			RunnerContext.push((XVar)(new RunnerContextItem(new XVar(Constants.CONTEXT_COMMAND), (XVar)(new XVar("dc", dc)))));

			return null;
		}
		public static XVar pushSearchContext(dynamic _param_searchClause)
		{
			#region pass-by-value parameters
			dynamic searchClause = XVar.Clone(_param_searchClause);
			#endregion

			RunnerContext.push((XVar)(new RunnerContextItem(new XVar(Constants.CONTEXT_SEARCH), (XVar)(new XVar("searchClause", searchClause)))));

			return null;
		}
		public static XVar pushMasterContext(dynamic _param_detailsKeys)
		{
			#region pass-by-value parameters
			dynamic detailsKeys = XVar.Clone(_param_detailsKeys);
			#endregion

			RunnerContext.push((XVar)(new RunnerContextItem(new XVar(Constants.CONTEXT_MASTER), (XVar)(new XVar("detailsKeys", detailsKeys)))));

			return null;
		}
		public static XVar getMasterValues()
		{
			dynamic ctx = null;
			ctx = XVar.Clone(RunnerContext.current());
			return ctx.getMasterValues();
		}
		public static XVar getValues()
		{
			dynamic ctx = null;
			ctx = XVar.Clone(RunnerContext.current());
			return ctx.getValues();
		}
		public static XVar PrepareRest(dynamic _param_str, dynamic _param_urlenc = null)
		{
			#region default values
			if(_param_urlenc as Object == null) _param_urlenc = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			dynamic urlenc = XVar.Clone(_param_urlenc);
			#endregion

			dynamic context = null, replacements = XVar.Array(), tokens = XVar.Array();
			context = XVar.Clone(RunnerContext.current());
			tokens = XVar.Clone(DB.scanTokenString((XVar)(str)));
			replacements = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> match in tokens["matches"].GetEnumerator())
			{
				dynamic args = XVar.Array(), offset = null, repl = XVar.Array(), token = null, val = null;
				offset = XVar.Clone(tokens["offsets"][match.Key]);
				token = XVar.Clone(tokens["tokens"][match.Key]);
				repl = XVar.Clone(new XVar("offset", offset, "len", MVCFunctions.strlen((XVar)(match.Value))));
				val = new XVar("");
				if((XVar)(MVCFunctions.IsNumeric(token))  && (XVar)(token < MVCFunctions.count(args)))
				{
					val = XVar.Clone(args[(int)token]);
				}
				else
				{
					val = XVar.Clone(RunnerContext.getValue((XVar)(token)));
				}
				if(XVar.Pack(urlenc))
				{
					val = XVar.Clone(MVCFunctions.RawUrlEncode((XVar)(val)));
				}
				repl.InitAndSetArrayItem(val, "insert");
				replacements.InitAndSetArrayItem(repl, null);
			}
			return RunnerContext.doReplacements((XVar)(str), (XVar)(replacements));
		}
		protected static XVar getOptionalBlocks(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic endPos = null, newPos = null, pos = null, snippetStack = XVar.Array(), snippets = XVar.Array(), stackIdx = null, tail = null, tailLen = null;
			if(XVar.Pack(!(XVar)(MVCFunctions.is_string((XVar)(str)))))
			{
				return XVar.Array();
			}
			snippetStack = XVar.Clone(XVar.Array());
			snippets = XVar.Clone(XVar.Array());
			pos = XVar.Clone(MVCFunctions.strpos((XVar)(str), new XVar("<?")));
			if(XVar.Equals(XVar.Pack(pos), XVar.Pack(false)))
			{
				return XVar.Array();
			}
			while(XVar.Pack(true))
			{
				snippetStack.InitAndSetArrayItem(pos, null);
				newPos = XVar.Clone(MVCFunctions.strpos((XVar)(str), new XVar("<?"), (XVar)(pos + 1)));
				tailLen = XVar.Clone(((XVar.Pack(!XVar.Equals(XVar.Pack(newPos), XVar.Pack(false))) ? XVar.Pack(newPos) : XVar.Pack(MVCFunctions.strlen((XVar)(str))))) - pos);
				tail = XVar.Clone(MVCFunctions.substr((XVar)(str), (XVar)(pos), (XVar)(tailLen)));
				endPos = new XVar(0);
				while((XVar)(!XVar.Equals(XVar.Pack(endPos = XVar.Clone(MVCFunctions.strpos((XVar)(tail), new XVar("?>"), (XVar)(endPos + 1)))), XVar.Pack(false)))  && (XVar)(MVCFunctions.count(snippetStack)))
				{
					stackIdx = XVar.Clone(MVCFunctions.count(snippetStack) - 1);
					snippets.InitAndSetArrayItem(new XVar("offset", snippetStack[stackIdx], "len", ((endPos + pos) + 2) - snippetStack[stackIdx]), null);
					snippetStack = XVar.Clone(MVCFunctions.array_slice((XVar)(snippetStack), new XVar(0), (XVar)(stackIdx)));
				}
				if(XVar.Equals(XVar.Pack(newPos), XVar.Pack(false)))
				{
					break;
				}
				pos = XVar.Clone(newPos);
			}
			return snippets;
		}
		public static XVar doReplacements(dynamic _param_str, dynamic _param_replacements)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			dynamic replacements = XVar.Clone(_param_replacements);
			#endregion

			dynamic i = null, offsetDelta = null, offsetShift = null, s = XVar.Array(), snippets = XVar.Array();
			if(XVar.Pack(!(XVar)(MVCFunctions.is_string((XVar)(str)))))
			{
				return str;
			}
			snippets = XVar.Clone(RunnerContext.getOptionalBlocks((XVar)(str)));
			i = new XVar(0);
			for(;i < MVCFunctions.count(snippets); ++(i))
			{
				s = snippets[i];
				s.InitAndSetArrayItem(true, "empty");
				foreach (KeyValuePair<XVar, dynamic> r in replacements.GetEnumerator())
				{
					if((XVar)(s["offset"] < r.Value["offset"])  && (XVar)(r.Value["offset"] < s["offset"] + s["len"]))
					{
						if(r.Value["insert"] != "")
						{
							s.InitAndSetArrayItem(false, "empty");
							break;
						}
					}
				}
			}
			offsetShift = new XVar(0);
			foreach (KeyValuePair<XVar, dynamic> r in replacements.GetEnumerator())
			{
				str = XVar.Clone(MVCFunctions.substr_replace((XVar)(str), (XVar)(r.Value["insert"]), (XVar)(r.Value["offset"] + offsetShift), (XVar)(r.Value["len"])));
				offsetDelta = XVar.Clone(MVCFunctions.strlen((XVar)(r.Value["insert"])) - r.Value["len"]);
				RunnerContext.updateOptionalBlockOffset((XVar)(snippets), (XVar)(r.Value["offset"]), (XVar)(offsetDelta));
				offsetShift += offsetDelta;
			}
			i = new XVar(0);
			for(;i < MVCFunctions.count(snippets); ++(i))
			{
				s = snippets[i];
				if(XVar.Pack(s["empty"]))
				{
					str = XVar.Clone(MVCFunctions.substr_replace((XVar)(str), new XVar(""), (XVar)(s["offset"]), (XVar)(s["len"])));
					offsetDelta = XVar.Clone(-s["len"]);
				}
				else
				{
					str = XVar.Clone(MVCFunctions.substr_replace((XVar)(str), (XVar)(MVCFunctions.substr((XVar)(str), (XVar)(s["offset"] + 2), (XVar)(s["len"] - 4))), (XVar)(s["offset"]), (XVar)(s["len"])));
					offsetDelta = XVar.Clone(-4);
				}
				RunnerContext.updateOptionalBlockOffset((XVar)(snippets), (XVar)(s["offset"]), (XVar)(offsetDelta));
			}
			return str;
		}
		protected static XVar updateOptionalBlockOffset(dynamic snippets, dynamic _param_offset, dynamic _param_delta)
		{
			#region pass-by-value parameters
			dynamic offset = XVar.Clone(_param_offset);
			dynamic delta = XVar.Clone(_param_delta);
			#endregion

			dynamic i = null, s = XVar.Array();
			i = new XVar(0);
			for(;i < MVCFunctions.count(snippets); ++(i))
			{
				s = snippets[i];
				if(offset < s["offset"])
				{
					s["offset"] += delta;
				}
				else
				{
					if(offset < s["offset"] + s["len"])
					{
						s["len"] += delta;
					}
				}
			}

			return null;
		}
		public static XVar getValue(dynamic _param_key)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			#endregion

			dynamic dotPos = null, prefix = null, scope = null;
			prefix = new XVar("");
			dotPos = XVar.Clone(MVCFunctions.strpos((XVar)(key), new XVar(".")));
			if(!XVar.Equals(XVar.Pack(dotPos), XVar.Pack(false)))
			{
				scope = XVar.Clone(MVCFunctions.strtolower((XVar)(MVCFunctions.substr((XVar)(key), new XVar(0), (XVar)(dotPos)))));
				key = XVar.Clone(MVCFunctions.substr((XVar)(key), (XVar)(dotPos + 1)));
			}
			else
			{
				if(XVar.Equals(XVar.Pack(key), XVar.Pack("language")))
				{
					scope = new XVar("global");
				}
				else
				{
					if(key == "all_field_search")
					{
						scope = XVar.Clone(key);
					}
					else
					{
						scope = new XVar("values");
					}
				}
			}
			return RunnerContext._getValue((XVar)(scope), (XVar)(key));
		}
		protected static XVar _getValue(dynamic _param_scope, dynamic _param_key)
		{
			#region pass-by-value parameters
			dynamic scope = XVar.Clone(_param_scope);
			dynamic key = XVar.Clone(_param_key);
			#endregion

			dynamic ctx = null, idx = null;
			idx = XVar.Clone(MVCFunctions.count(GlobalVars.contextStack.stack));
			while(XVar.Pack(0) < idx)
			{
				ctx = XVar.Clone(GlobalVars.contextStack.stack[--(idx)]);
				if(XVar.Pack(ctx.hasScope((XVar)(scope))))
				{
					return ctx.getContextValue((XVar)(scope), (XVar)(key));
				}
			}
			return false;
		}
	}
	public partial class TempContext : XClass
	{
		public TempContext(dynamic _param_context)
		{
			#region pass-by-value parameters
			dynamic context = XVar.Clone(_param_context);
			#endregion

			RunnerContext.push((XVar)(context));
		}
		public virtual XVar __destruct()
		{
			RunnerContext.pop();

			return null;
		}
	}
}
