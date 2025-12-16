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
	public partial class ArrayResult : DataResult
	{
		public dynamic source;
		protected dynamic dummy = XVar.Pack(null);
		protected dynamic totalRecords = XVar.Pack(-1);
		protected dynamic recIdx = XVar.Pack(0);
		protected static bool skipArrayResultCtor = false;
		public ArrayResult(dynamic _param_source)
		{
			if(skipArrayResultCtor)
			{
				skipArrayResultCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic source = XVar.Clone(_param_source);
			#endregion

			this.source = source;
			this.fillColumnNames();
		}
		public override XVar fetchAssoc()
		{
			this.prepareRecord();
			++(this.recIdx);
			return this.data;
		}
		protected override XVar prepareRecord()
		{
			if(this.recIdx < this.count())
			{
				this.data = this.source[this.recIdx];
				return true;
			}
			else
			{
				this.data = this.dummy;
				return false;
			}

			return null;
		}
		public override XVar fetchNumeric()
		{
			this.prepareRecord();
			++(this.recIdx);
			return this.assocToNumeric((XVar)(this.data));
		}
		public override XVar numFields()
		{
			return MVCFunctions.count(this.fieldNames);
		}
		public override XVar fieldName(dynamic _param_offset)
		{
			#region pass-by-value parameters
			dynamic offset = XVar.Clone(_param_offset);
			#endregion

			return this.fieldNames[offset];
		}
		public override XVar seekRecord(dynamic _param_n)
		{
			#region pass-by-value parameters
			dynamic n = XVar.Clone(_param_n);
			#endregion

			this.recIdx = XVar.Clone(n);

			return null;
		}
		public override XVar eof()
		{
			this.prepareRecord();
			return !(XVar)(this.data);
		}
		protected override XVar fillColumnNames()
		{
			dynamic names = XVar.Array();
			if(XVar.Pack(this.fieldNames))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(this.source)))
			{
				return null;
			}
			names = XVar.Clone(MVCFunctions.array_keys((XVar)(this.source[0])));
			foreach (KeyValuePair<XVar, dynamic> fname in names.GetEnumerator())
			{
				this.fieldNames.InitAndSetArrayItem(fname.Value, null);
				this.fieldMap.InitAndSetArrayItem(fname.Key, fname.Value);
				this.upperMap.InitAndSetArrayItem(fname.Key, MVCFunctions.strtoupper((XVar)(fname.Value)));
			}

			return null;
		}
		public override XVar func_next()
		{
			++(this.recIdx);

			return null;
		}
		public override XVar getData()
		{
			if(XVar.Pack(!(XVar)(this.prepareRecord())))
			{
				return null;
			}
			return this.data;
		}
		public override XVar getNumData()
		{
			if(XVar.Pack(!(XVar)(this.prepareRecord())))
			{
				return null;
			}
			return this.assocToNumeric((XVar)(this.data));
		}
		public virtual XVar count()
		{
			if(0 <= this.totalRecords)
			{
				return this.totalRecords;
			}
			return MVCFunctions.count(this.source);
		}
		public override XVar reorder(dynamic _param_callback)
		{
			#region pass-by-value parameters
			dynamic callback = XVar.Clone(_param_callback);
			#endregion

			MVCFunctions.usort((XVar)(this.source), (XVar)(callback));
			this.recIdx = new XVar(0);
			return this;
		}
		public static XVar createFromResult(dynamic _param_result)
		{
			#region pass-by-value parameters
			dynamic result = XVar.Clone(_param_result);
			#endregion

			dynamic data = null, newData = XVar.Array();
			newData = XVar.Clone(XVar.Array());
			while(XVar.Pack(data = XVar.Clone(result.fetchAssoc())))
			{
				newData.InitAndSetArrayItem(data, null);
			}
			return new ArrayResult((XVar)(newData));
		}
		public override XVar randomAccess()
		{
			return true;
		}
		public override XVar position()
		{
			return this.recIdx;
		}
		public virtual XVar setTotalRecords(dynamic _param_n)
		{
			#region pass-by-value parameters
			dynamic n = XVar.Clone(_param_n);
			#endregion

			this.totalRecords = XVar.Clone(n);

			return null;
		}
	}
}
