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
	public partial class DataSourceREST : DataSource
	{
		protected dynamic cipherer;
		protected static bool skipDataSourceRESTCtor = false;
		public DataSourceREST(dynamic _param_name, dynamic _param_pSet_packed, dynamic _param_connection)
			:base((XVar)_param_name)
		{
			if(skipDataSourceRESTCtor)
			{
				skipDataSourceRESTCtor = false;
				return;
			}
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic connection = XVar.Clone(_param_connection);
			#endregion

			this.pSet = XVar.UnPackProjectSettings(pSet);
			this.connection = XVar.Clone(connection);
			this.opDescriptors = XVar.Clone(this.pSet.getDataSourceOps());
		}
		protected override XVar getListImpl(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic result = null;
			result = XVar.Clone(this.getListData((XVar)(dc)));
			if(XVar.Pack(!(XVar)(result)))
			{
				return result;
			}
			if(XVar.Pack(!(XVar)(this.codeOp(new XVar("selectList")))))
			{
				result.seekRecord((XVar)(dc.startRecord));
			}
			return result;
		}
		public override XVar updateSingle(dynamic _param_dc, dynamic _param_requireKeys = null)
		{
			#region default values
			if(_param_requireKeys as Object == null) _param_requireKeys = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic requireKeys = XVar.Clone(_param_requireKeys);
			#endregion

			dynamic op = null, ret = null;
			op = new XVar("update");
			if(XVar.Pack(this.codeOp((XVar)(op))))
			{
				return this.callCodeOp((XVar)(op), (XVar)(dc));
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.count(dc.values))))
			{
				return true;
			}
			ret = XVar.Clone(this.runRequest((XVar)(op), (XVar)(dc)));
			return !XVar.Equals(XVar.Pack(ret), XVar.Pack(false));
		}
		public override XVar insertSingle(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic op = null, ret = null;
			op = new XVar("insert");
			if(XVar.Pack(this.codeOp((XVar)(op))))
			{
				return this.callCodeOp((XVar)(op), (XVar)(dc));
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.count(dc.values))))
			{
				this.setError(new XVar("nothing to insert"));
				return false;
			}
			ret = XVar.Clone(this.runRequest((XVar)(op), (XVar)(dc)));
			return (XVar.Pack(!XVar.Equals(XVar.Pack(ret), XVar.Pack(false))) ? XVar.Pack(dc.values) : XVar.Pack(ret));
		}
		public override XVar deleteSingle(dynamic _param_dc, dynamic _param_requireKeys = null)
		{
			#region default values
			if(_param_requireKeys as Object == null) _param_requireKeys = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic requireKeys = XVar.Clone(_param_requireKeys);
			#endregion

			dynamic op = null, ret = null;
			op = new XVar("delete");
			if(XVar.Pack(this.codeOp((XVar)(op))))
			{
				return this.callCodeOp(new XVar("deleteRecord"), (XVar)(dc));
			}
			if((XVar)(!(XVar)(MVCFunctions.count(dc.keys)))  && (XVar)(requireKeys))
			{
				return true;
			}
			ret = XVar.Clone(this.runRequest((XVar)(op), (XVar)(dc)));
			return !XVar.Equals(XVar.Pack(ret), XVar.Pack(false));
		}
		protected override XVar getSingleImpl(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic op = null, result = null;
			op = new XVar("selectOne");
			if(XVar.Pack(this.codeOp((XVar)(op))))
			{
				result = XVar.Clone(this.callCodeOp((XVar)(op), (XVar)(dc)));
				if(XVar.Equals(XVar.Pack(result), XVar.Pack(false)))
				{
					MVCFunctions.trigger_error((XVar)(this.lastError()), new XVar(Constants.E_USER_ERROR));
					return false;
				}
				return result;
			}
			if(XVar.Pack(this.opDescriptors.KeyExists(op)))
			{
				dynamic opDesc = XVar.Array();
				opDesc = XVar.Clone(this.opDescriptors[op]);
				result = XVar.Clone(this.runRequest((XVar)(op), (XVar)(dc)));
				if(!XVar.Equals(XVar.Pack(result), XVar.Pack(false)))
				{
					result = XVar.Clone(this.resultFromJson((XVar)(result), new XVar(false)));
				}
				if(XVar.Equals(XVar.Pack(result), XVar.Pack(false)))
				{
					MVCFunctions.trigger_error((XVar)(this.lastError()), new XVar(Constants.E_USER_ERROR));
					return false;
				}
				if(XVar.Pack(!(XVar)(opDesc["skipFilter"])))
				{
					result = XVar.Clone(this.filterResult((XVar)(result), (XVar)(dc.filter)));
				}
			}
			else
			{
				result = XVar.Clone(this.getListData((XVar)(dc)));
			}
			return result;
		}
		protected override XVar getListData(dynamic _param_dc, dynamic _param_listRequest = null)
		{
			#region default values
			if(_param_listRequest as Object == null) _param_listRequest = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic listRequest = XVar.Clone(_param_listRequest);
			#endregion

			dynamic op = null, opDesc = XVar.Array(), res = null;
			if(XVar.Pack(dc._cache["listData"]))
			{
				dc._cache["listData"].seekRecord((XVar)(dc._cache["listDataPos"]));
				return dc._cache["listData"];
			}
			if(XVar.Pack(this.falseCondition((XVar)(dc.filter))))
			{
				dc._cache.InitAndSetArrayItem(new ArrayResult((XVar)(XVar.Array())), "listData");
				dc._cache.InitAndSetArrayItem(0, "listDataPos");
				return dc._cache["listData"];
			}
			dc.filter = XVar.Clone(this.addKeysToFilter((XVar)(dc)));
			op = new XVar("selectList");
			opDesc = XVar.Clone(this.opDescriptors[op]);
			if(XVar.Pack(this.codeOp((XVar)(op))))
			{
				res = XVar.Clone(this.callCodeOp((XVar)(op), (XVar)(dc)));
			}
			else
			{
				res = XVar.Clone(this.runRequest(new XVar("selectList"), (XVar)(dc)));
				if(!XVar.Equals(XVar.Pack(res), XVar.Pack(false)))
				{
					res = XVar.Clone(this.resultFromJson((XVar)(res), new XVar(true)));
				}
			}
			if(XVar.Equals(XVar.Pack(res), XVar.Pack(false)))
			{
				MVCFunctions.trigger_error((XVar)(this.lastError()), new XVar(Constants.E_USER_ERROR));
				return false;
			}
			if(XVar.Pack(this.codeOp((XVar)(op))))
			{
				res = XVar.Clone(this.addExtraColumns((XVar)(res), (XVar)(dc)));
			}
			else
			{
				res = XVar.Clone(this.addExtraColumns((XVar)(res), (XVar)(dc)));
				if(XVar.Pack(!(XVar)(opDesc["skipFilter"])))
				{
					res = XVar.Clone(this.filterResult((XVar)(res), (XVar)(dc.filter)));
				}
				if(XVar.Pack(!(XVar)(opDesc["skipOrder"])))
				{
					this.reorderResult((XVar)(dc), (XVar)(res));
				}
			}
			dc._cache.InitAndSetArrayItem(res, "listData");
			dc._cache.InitAndSetArrayItem(res.position(), "listDataPos");
			return dc._cache["listData"];
		}
		public override XVar getCount(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic op = null, ret = null;
			op = new XVar("count");
			if(XVar.Pack(this.codeOp((XVar)(op))))
			{
				return this.callCodeOp((XVar)(op), (XVar)(dc));
			}
			ret = XVar.Clone(this.getListData((XVar)(dc)));
			if(XVar.Pack(ret))
			{
				return ret.count();
			}
			return 0;
		}
		public virtual XVar preparePayload(dynamic _param_payload, dynamic _param_skipBody = null)
		{
			#region default values
			if(_param_skipBody as Object == null) _param_skipBody = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic payload = XVar.Clone(_param_payload);
			dynamic skipBody = XVar.Clone(_param_skipBody);
			#endregion

			dynamic payloadForm = XVar.Array(), payloadHeaders = XVar.Array(), payloadUrl = XVar.Array();
			payloadForm = XVar.Clone(XVar.Array());
			payloadUrl = XVar.Clone(XVar.Array());
			payloadHeaders = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> p in payload.GetEnumerator())
			{
				dynamic value = null;
				value = XVar.Clone(RunnerContext.PrepareRest((XVar)(p.Value["value"]), new XVar(false)));
				if((XVar)(p.Value["skipEmpty"])  && (XVar)(value == XVar.Pack("")))
				{
					continue;
				}
				if(p.Value["location"] == "url")
				{
					payloadUrl.InitAndSetArrayItem(value, p.Value["name"]);
				}
				else
				{
					if(p.Value["location"] == "header")
					{
						payloadHeaders.InitAndSetArrayItem(value, p.Value["name"]);
					}
					else
					{
						if(XVar.Pack(!(XVar)(skipBody)))
						{
							payloadForm.InitAndSetArrayItem(value, p.Value["name"]);
						}
					}
				}
			}
			return new XVar("url", payloadUrl, "form", payloadForm, "header", payloadHeaders);
		}
		protected virtual XVar runRequest(dynamic _param_op, dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic op = XVar.Clone(_param_op);
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic conn = null, desc = XVar.Array(), payload = XVar.Array(), responseData = null, var_request = null, var_response = XVar.Array();
			desc = XVar.Clone(this.opDescriptors[op]);
			conn = XVar.Clone(this.getConnection());
			RunnerContext.pushDataCommandContext((XVar)(dc));
			var_request = XVar.Clone(new HttpRequest((XVar)(RunnerContext.PrepareRest((XVar)(MVCFunctions.Concat(conn.url, desc["request"])))), (XVar)(desc["method"])));
			payload = XVar.Clone(this.preparePayload((XVar)(this.opDescriptors[op]["payload"]), (XVar)(!(XVar)(!(XVar)(this.opDescriptors[op]["rawPayload"])))));
			var_request.headers = XVar.Clone(payload["header"]);
			var_request.urlParams = XVar.Clone(payload["url"]);
			if(XVar.Pack(this.opDescriptors[op]["rawPayload"]))
			{
				var_request.body = XVar.Clone(RunnerContext.PrepareRest((XVar)(desc["payloadString"]), new XVar(false)));
			}
			else
			{
				var_request.postPayload = XVar.Clone(payload["form"]);
			}
			RunnerContext.pop();
			if(desc["payloadFormat"] == Constants.pfJSON)
			{
				var_request.setHeader(new XVar("Content-Type"), new XVar("application/json"));
			}
			conn.requestAddAuth((XVar)(var_request));
			var_response = XVar.Clone(var_request.run());
			if(XVar.Pack(var_response["error"]))
			{
				this.setError((XVar)(var_response["errorMessage"]));
				return false;
			}
			responseData = XVar.Clone(HttpRequest.parseResponseArray((XVar)(var_response)));
			if(XVar.Equals(XVar.Pack(responseData), XVar.Pack(null)))
			{
				this.setError((XVar)(MVCFunctions.Concat("Unable to parse JSON result\n\n", var_response["content"])));
				return false;
			}
			return responseData;
		}
		public virtual XVar getFieldDateFormats()
		{
			dynamic ret = XVar.Array();
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> f in this.pSet.getFieldsList().GetEnumerator())
			{
				dynamic format = null, var_type = null;
				var_type = XVar.Clone(this.pSet.getFieldType((XVar)(f.Value)));
				if(XVar.Pack(!(XVar)(CommonFunctions.IsDateFieldType((XVar)(var_type)))))
				{
					continue;
				}
				format = XVar.Clone(this.pSet.getRestDateFormat((XVar)(f.Value)));
				if(XVar.Pack(!(XVar)(format)))
				{
					continue;
				}
				ret.InitAndSetArrayItem(format, f.Value);
			}
			return ret;
		}
		public virtual XVar normalizeDateValues(ref dynamic data)
		{
			dynamic formats = XVar.Array(), res = XVar.Array();
			formats = XVar.Clone(this.getFieldDateFormats());
			if((XVar)(!(XVar)(data))  || (XVar)(!(XVar)(formats)))
			{
				return data;
			}
			res = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> row in data.GetEnumerator())
			{
				dynamic resRow = XVar.Array();
				resRow = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> value in row.Value.GetEnumerator())
				{
					dynamic parsed = null;
					resRow.InitAndSetArrayItem(value.Value, value.Key);
					if(XVar.Pack(!(XVar)(formats[value.Key])))
					{
						continue;
					}
					parsed = XVar.Clone(CommonFunctions.parseRestDate((XVar)(value.Value), (XVar)(formats[value.Key])));
					if(XVar.Pack(parsed))
					{
						resRow.InitAndSetArrayItem(parsed, value.Key);
					}
				}
				res.InitAndSetArrayItem(resRow, null);
			}
			return res;
		}
		public virtual XVar resultFromJson(dynamic data, dynamic _param_listRequest)
		{
			#region pass-by-value parameters
			dynamic listRequest = XVar.Clone(_param_listRequest);
			#endregion

			dynamic flatten = null, ret = null;
			flatten = XVar.Clone(new FlattenObject((XVar)(data), (XVar)(this.getFieldPaths((XVar)(listRequest)))));
			ret = XVar.Clone(flatten.flatten());
			if(XVar.Equals(XVar.Pack(ret), XVar.Pack(null)))
			{
				this.setError((XVar)(MVCFunctions.runner_json_encode((XVar)(data))));
				return false;
			}
			ret = XVar.Clone(this.normalizeDateValues(ref ret));
			return new ArrayResult((XVar)(ret));
		}
		public virtual XVar resultFromJson_old(dynamic data, dynamic _param_listRequest)
		{
			#region pass-by-value parameters
			dynamic listRequest = XVar.Clone(_param_listRequest);
			#endregion

			dynamic fieldPaths = XVar.Array(), foundAny = null, recCount = null, recNo = null, record = XVar.Array(), ret = XVar.Array();
			if(XVar.Equals(XVar.Pack(data), XVar.Pack(false)))
			{
				return data;
			}
			fieldPaths = XVar.Clone(this.getFieldPaths((XVar)(listRequest)));
			recNo = new XVar(0);
			recCount = XVar.Clone(-1);
			ret = XVar.Clone(XVar.Array());
			foundAny = new XVar(false);
			while(XVar.Pack(true))
			{
				record = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> path in fieldPaths.GetEnumerator())
				{
					dynamic foundValue = null, pointer = XVar.Array();
					foundValue = new XVar(true);
					pointer = data;
					foreach (KeyValuePair<XVar, dynamic> pi in path.Value.GetEnumerator())
					{
						dynamic p = null;
						p = XVar.Clone(pi.Value);
						if(p == "*")
						{
							if(recCount == -1)
							{
								recCount = XVar.Clone(MVCFunctions.count(pointer));
							}
							p = XVar.Clone(recNo);
							foundAny = new XVar(true);
						}
						if(XVar.Pack(!(XVar)(pointer.KeyExists(p))))
						{
							foundValue = new XVar(false);
							break;
						}
						pointer = pointer[p];
					}
					if(XVar.Pack(foundValue))
					{
						foundAny = new XVar(true);
						record.InitAndSetArrayItem(pointer, path.Key);
					}
				}
				if(XVar.Pack(record))
				{
					ret.InitAndSetArrayItem(record, null);
				}
				++(recNo);
				if(recCount <= recNo)
				{
					break;
				}
			}
			if(XVar.Pack(!(XVar)(foundAny)))
			{
				this.setError((XVar)(MVCFunctions.runner_json_encode((XVar)(data))));
				return false;
			}
			return new ArrayResult((XVar)(ret));
		}
		protected virtual XVar getFieldPaths(dynamic _param_listRequest)
		{
			#region pass-by-value parameters
			dynamic listRequest = XVar.Clone(_param_listRequest);
			#endregion

			dynamic ret = XVar.Array();
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> f in this.pSet.getFieldsList().GetEnumerator())
			{
				dynamic source = null;
				source = XVar.Clone(this.pSet.getFieldSource((XVar)(f.Value), (XVar)(listRequest)));
				if((XVar)(!(XVar)(source))  && (XVar)(!(XVar)(listRequest)))
				{
					source = XVar.Clone(this.pSet.getFieldSource((XVar)(f.Value), new XVar(true)));
				}
				if(XVar.Pack(!(XVar)(source)))
				{
					continue;
				}
				ret.InitAndSetArrayItem(MVCFunctions.explode(new XVar("/"), (XVar)(source)), f.Value);
			}
			return ret;
		}
		public override XVar checkAuthorization()
		{
			return this.connection.checkAuthorization();
		}
		public override XVar getAuthorizationInfo()
		{
			return this.connection.getAuthorizationRequest();
		}
		public virtual XVar runCustomRequest()
		{
			dynamic command = null, dataSource = null, dsconn = null, method = null, responseBody = null, responseData = null, responseHeaders = null, rs = null, url = null, value = null, value1 = null, var_request = null, var_response = XVar.Array();
			dataSource = XVar.Clone(this);
			method = new XVar("GET");
			url = new XVar("xxx");
			url = XVar.Clone(RunnerContext.PrepareRest((XVar)(url)));
			var_request = XVar.Clone(new HttpRequest((XVar)(url), (XVar)(method)));
			value = XVar.Clone(RunnerContext.PrepareRest(new XVar("value")));
			if(value != XVar.Pack(""))
			{
				var_request.urlParams.InitAndSetArrayItem(value, "name");
			}
			var_request.urlParams.InitAndSetArrayItem(RunnerContext.PrepareRest(new XVar("value1")), "name1");
			value = XVar.Clone(RunnerContext.PrepareRest(new XVar("value")));
			if(value != XVar.Pack(""))
			{
				var_request.headers.InitAndSetArrayItem(value, "name");
			}
			var_request.headers.InitAndSetArrayItem(RunnerContext.PrepareRest(new XVar("value1")), "name1");
			var_request.body = XVar.Clone(RunnerContext.PrepareRest(new XVar("aaa")));
			value = XVar.Clone(RunnerContext.PrepareRest(new XVar("value")));
			if(value != XVar.Pack(""))
			{
				var_request.postPayload.InitAndSetArrayItem(value, "name");
			}
			var_request.postPayload.InitAndSetArrayItem(value1, "name1");
			dsconn = XVar.Clone(dataSource.getConnection());
			var_response = XVar.Clone(dsconn.requestWithAuth());
			if(XVar.Pack(var_response["error"]))
			{
				dataSource.setError((XVar)(var_response["errorMessage"]));
				return false;
			}
			responseHeaders = XVar.Clone(HttpRequest.parseHeaders((XVar)(var_response)));
			responseBody = XVar.Clone(var_response["content"]);
			responseData = XVar.Clone(HttpRequest.parseResponseArray((XVar)(var_response)));
			if(XVar.Equals(XVar.Pack(responseData), XVar.Pack(null)))
			{
				dataSource.setError((XVar)(MVCFunctions.Concat("Unable to parse JSON result\n\n", var_response["content"])));
				return false;
			}
			rs = XVar.Clone(dataSource.resultFromJson((XVar)(responseData), new XVar(true)));
			if(XVar.Pack(!(XVar)(rs)))
			{
				return false;
			}
			rs = XVar.Clone(dataSource.filterResult((XVar)(rs), (XVar)(command.filter)));
			rs = XVar.Clone(dataSource.reorderResult((XVar)(command), (XVar)(rs)));
			rs.seekRecord((XVar)(command.startRecord));
			return rs;
		}
	}
	public partial class FlattenObject : XClass
	{
		protected dynamic source;
		protected dynamic fields;
		protected dynamic starRoot;
		public FlattenObject(dynamic source, dynamic _param_fields)
		{
			#region pass-by-value parameters
			dynamic fields = XVar.Clone(_param_fields);
			#endregion

			this.source = source;
			this.fields = fields;
			this.starRoot = XVar.Clone(new StarNode());
			this.createStarTree();
		}
		public virtual XVar flatten()
		{
			dynamic record = XVar.Array(), ret = XVar.Array(), rootObject = XVar.Array();
			ret = XVar.Clone(XVar.Array());
			rootObject = XVar.Clone(XVar.Array());
			rootObject.InitAndSetArrayItem(this.source, null);
			while(XVar.Pack(true))
			{
				record = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> path in this.fields.GetEnumerator())
				{
					record.InitAndSetArrayItem(this.starRoot.getFieldValue((XVar)(path.Value), (XVar)(rootObject)), path.Key);
				}
				ret.InitAndSetArrayItem(record, null);
				if(XVar.Pack(!(XVar)(this.starRoot.func_next())))
				{
					break;
				}
			}
			if(XVar.Pack(!(XVar)(this.starRoot.foundAnyData())))
			{
				return null;
			}
			return ret;
		}
		protected virtual XVar createStarTree()
		{
			foreach (KeyValuePair<XVar, dynamic> path in this.fields.GetEnumerator())
			{
				dynamic nodePath = XVar.Array(), starNode = null;
				starNode = XVar.Clone(this.starRoot);
				nodePath = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> p in path.Value.GetEnumerator())
				{
					if(p.Value == "*")
					{
						starNode = XVar.Clone(starNode.addStar((XVar)(nodePath)));
						nodePath = XVar.Clone(XVar.Array());
					}
					else
					{
						nodePath.InitAndSetArrayItem(p.Value, null);
					}
				}
			}

			return null;
		}
	}
	public partial class StarNode : XClass
	{
		protected dynamic path;
		protected dynamic source;
		protected dynamic currentSource;
		protected dynamic keys = XVar.Array();
		protected dynamic currentIndex = XVar.Pack(0);
		protected dynamic foundData = XVar.Pack(false);
		protected dynamic children = XVar.Array();
		public StarNode(dynamic _param_path = null)
		{
			#region default values
			if(_param_path as Object == null) _param_path = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			this.path = XVar.Clone(path);
		}
		public virtual XVar addStar(dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			dynamic pathString = null;
			pathString = XVar.Clone(MVCFunctions.implode(new XVar("/"), (XVar)(path)));
			if(XVar.Pack(!(XVar)(this.children[pathString])))
			{
				this.children.InitAndSetArrayItem(new StarNode((XVar)(path)), pathString);
			}
			return this.children[pathString];
		}
		public virtual XVar getFieldValue(dynamic _param_path, dynamic source)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			dynamic nodePath = XVar.Array(), ptr = XVar.Array();
			this.updateSource((XVar)(source));
			ptr = this.currentSource;
			nodePath = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> p in path.GetEnumerator())
			{
				if(p.Value == "*")
				{
					dynamic child = null;
					child = XVar.Clone(this.getChild((XVar)(nodePath)));
					this.foundData = new XVar(true);
					return child.getFieldValue((XVar)(MVCFunctions.array_slice((XVar)(path), (XVar)(p.Key + 1))), (XVar)(ptr));
				}
				nodePath.InitAndSetArrayItem(p.Value, null);
				if((XVar)(ptr)  && (XVar)(ptr.KeyExists(p.Value)))
				{
					ptr = ptr[p.Value];
				}
				else
				{
					return null;
				}
			}
			this.foundData = new XVar(true);
			return ptr;
		}
		public virtual XVar foundAnyData()
		{
			return this.foundData;
		}
		protected virtual XVar resetCounter()
		{
			this.currentIndex = new XVar(0);
			if((XVar)(this.source)  && (XVar)(MVCFunctions.is_array((XVar)(this.source))))
			{
				this.currentSource = this.source[this.keys[0]];
			}
			else
			{
				dynamic nil = null;
				nil = new XVar(null);
				this.currentSource = nil;
			}

			return null;
		}
		protected virtual XVar updateSource(dynamic source)
		{
			if(XVar.Equals(XVar.Pack(this.source), XVar.Pack(source)))
			{
				return null;
			}
			this.source = source;
			if((XVar)(this.source)  && (XVar)(MVCFunctions.is_array((XVar)(this.source))))
			{
				this.keys = XVar.Clone(MVCFunctions.array_keys((XVar)(this.source)));
			}
			else
			{
				this.keys = XVar.Clone(XVar.Array());
			}
			this.resetCounter();

			return null;
		}
		protected virtual XVar getChild(dynamic path)
		{
			return this.children[MVCFunctions.implode(new XVar("/"), (XVar)(path))];
		}
		public virtual XVar func_next()
		{
			dynamic prevChildren = XVar.Array();
			prevChildren = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> child in this.children.GetEnumerator())
			{
				if(XVar.Pack(child.Value.func_next()))
				{
					foreach (KeyValuePair<XVar, dynamic> ch in prevChildren.GetEnumerator())
					{
						ch.Value.resetCounter();
					}
					return true;
				}
				else
				{
					prevChildren.InitAndSetArrayItem(child.Value, null);
				}
			}
			++(this.currentIndex);
			if(XVar.Pack(!(XVar)(this.keys.KeyExists(this.currentIndex))))
			{
				return false;
			}
			this.currentSource = this.source[this.keys[this.currentIndex]];
			return true;
		}
	}
}
