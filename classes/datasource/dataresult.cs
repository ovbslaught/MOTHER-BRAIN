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
	public partial class DataResult : XClass
	{
		protected dynamic fieldNames = XVar.Array();
		protected dynamic upperMap = XVar.Array();
		protected dynamic fieldMap = XVar.Array();
		protected dynamic fieldSubs = XVar.Array();
		protected dynamic data;
		public DataResult()
		{
		}
		public virtual XVar getQueryHandle()
		{
			return null;
		}
		public virtual XVar fetchAssoc()
		{
			return null;
		}
		public virtual XVar fetchNumeric()
		{
			return null;
		}
		public virtual XVar closeQuery()
		{

			return null;
		}
		public virtual XVar numFields()
		{
			return 0;
		}
		public virtual XVar fieldName(dynamic _param_offset)
		{
			#region pass-by-value parameters
			dynamic offset = XVar.Clone(_param_offset);
			#endregion

			return "";
		}
		public virtual XVar seekRecord(dynamic _param_n)
		{
			#region pass-by-value parameters
			dynamic n = XVar.Clone(_param_n);
			#endregion


			return null;
		}
		public virtual XVar eof()
		{
			return true;
		}
		public virtual XVar func_next()
		{

			return null;
		}
		public virtual XVar getData()
		{

			return null;
		}
		public virtual XVar getNumData()
		{

			return null;
		}
		protected virtual XVar prepareRecord()
		{
			return false;
		}
		protected virtual XVar fillColumnNames()
		{

			return null;
		}
		protected virtual XVar numericToAssoc(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic i = null, nFields = null, ret = XVar.Array();
			if(XVar.Pack(!(XVar)(data)))
			{
				return data;
			}
			ret = XVar.Clone(XVar.Array());
			nFields = XVar.Clone(this.numFields());
			i = new XVar(0);
			for(;i < nFields; ++(i))
			{
				ret.InitAndSetArrayItem(data[i], this.fieldNames[i]);
			}
			if(XVar.Pack(this.fieldSubs))
			{
				return this.substituteFields(ref ret);
			}
			return ret;
		}
		protected virtual XVar assocToNumeric(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic i = null, nFields = null, ret = XVar.Array();
			if(XVar.Pack(!(XVar)(data)))
			{
				return data;
			}
			ret = XVar.Clone(XVar.Array());
			nFields = XVar.Clone(this.numFields());
			i = new XVar(0);
			for(;i < nFields; ++(i))
			{
				ret.InitAndSetArrayItem(data[this.fieldNames[i]], i);
			}
			return ret;
		}
		public virtual XVar value(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			if(XVar.Pack(!(XVar)(this.prepareRecord())))
			{
				return null;
			}
			if(XVar.Pack(MVCFunctions.IsNumeric(field)))
			{
				return this.data[field];
			}
			if(XVar.Pack(this.fieldMap.KeyExists(field)))
			{
				return this.data[this.fieldMap[field]];
			}
			if(XVar.Pack(this.upperMap.KeyExists(MVCFunctions.strtoupper((XVar)(field)))))
			{
				return this.data[this.upperMap[MVCFunctions.strtoupper((XVar)(field))]];
			}
			return null;
		}
		public virtual XVar seekPage(dynamic _param_pageSize, dynamic _param_page)
		{
			#region pass-by-value parameters
			dynamic pageSize = XVar.Clone(_param_pageSize);
			dynamic page = XVar.Clone(_param_page);
			#endregion

			this.seekRecord((XVar)(pageSize * (page - 1)));

			return null;
		}
		public virtual XVar setFieldSubstitutions(dynamic _param_fieldSubs)
		{
			#region pass-by-value parameters
			dynamic fieldSubs = XVar.Clone(_param_fieldSubs);
			#endregion

			this.fieldSubs = XVar.Clone(fieldSubs);

			return null;
		}
		public virtual XVar substituteFields(ref dynamic data)
		{
			dynamic ret = XVar.Array();
			if(XVar.Pack(!(XVar)(this.fieldSubs)))
			{
				return data;
			}
			if(XVar.Pack(!(XVar)(data)))
			{
				return data;
			}
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> srcField in this.fieldSubs.GetEnumerator())
			{
				if(XVar.Pack(data.KeyExists(srcField.Value)))
				{
					ret.InitAndSetArrayItem(data[srcField.Value], srcField.Key);
				}
			}
			return ret;
		}
		public virtual XVar randomAccess()
		{
			return false;
		}
		public virtual XVar position()
		{
			return 0;
		}
		public virtual XVar reorder(dynamic _param_callback)
		{
			#region pass-by-value parameters
			dynamic callback = XVar.Clone(_param_callback);
			#endregion

			return this;
		}
		public virtual XVar fieldExists(dynamic _param_fieldName)
		{
			#region pass-by-value parameters
			dynamic fieldName = XVar.Clone(_param_fieldName);
			#endregion

			this.fillColumnNames();
			return this.fieldMap.KeyExists(fieldName);
		}
	}
}
