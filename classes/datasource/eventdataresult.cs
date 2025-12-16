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
	public partial class EventDataResult : DataResult
	{
		protected dynamic dataResult = XVar.Pack(null);
		protected dynamic eventsObject = XVar.Pack(null);
		protected static bool skipEventDataResultCtor = false;
		public EventDataResult(dynamic _param_dataResult, dynamic _param_eventsObject)
		{
			if(skipEventDataResultCtor)
			{
				skipEventDataResultCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic dataResult = XVar.Clone(_param_dataResult);
			dynamic eventsObject = XVar.Clone(_param_eventsObject);
			#endregion

			this.dataResult = XVar.Clone(dataResult);
			this.eventsObject = XVar.Clone(eventsObject);
		}
		public override XVar getQueryHandle()
		{
			return this.dataResult.getQueryHandle();
		}
		public override XVar fetchAssoc()
		{
			dynamic data = null;
			data = XVar.Clone(this.dataResult.fetchAssoc());
			if(XVar.Pack(data))
			{
				this.eventsObject.ProcessRecord((XVar)(data));
			}
			return data;
		}
		public override XVar fetchNumeric()
		{
			return this.dataResult.fetchNumeric();
		}
		public override XVar closeQuery()
		{
			this.dataResult.closeQuery();

			return null;
		}
		public override XVar numFields()
		{
			return this.dataResult.numFields();
		}
		public override XVar fieldName(dynamic _param_offset)
		{
			#region pass-by-value parameters
			dynamic offset = XVar.Clone(_param_offset);
			#endregion

			return this.dataResult.fieldName((XVar)(offset));
		}
		public override XVar seekRecord(dynamic _param_n)
		{
			#region pass-by-value parameters
			dynamic n = XVar.Clone(_param_n);
			#endregion

			this.dataResult.seekRecord((XVar)(n));

			return null;
		}
		public override XVar eof()
		{
			return this.dataResult.eof();
		}
		public override XVar func_next()
		{
			this.dataResult.func_next();

			return null;
		}
		public override XVar getData()
		{
			this.dataResult.getData();

			return null;
		}
		public override XVar getNumData()
		{
			this.dataResult.getNumData();

			return null;
		}
		public override XVar value(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.dataResult.value((XVar)(field));
		}
		public override XVar seekPage(dynamic _param_pageSize, dynamic _param_page)
		{
			#region pass-by-value parameters
			dynamic pageSize = XVar.Clone(_param_pageSize);
			dynamic page = XVar.Clone(_param_page);
			#endregion

			this.dataResult.seekRecord((XVar)(pageSize * (page - 1)));

			return null;
		}
		public override XVar reorder(dynamic _param_callback)
		{
			#region pass-by-value parameters
			dynamic callback = XVar.Clone(_param_callback);
			#endregion

			this.dataResult.reorder((XVar)(callback));
			return this;
		}
		public override XVar fieldExists(dynamic _param_fieldName)
		{
			#region pass-by-value parameters
			dynamic fieldName = XVar.Clone(_param_fieldName);
			#endregion

			return this.dataResult.fieldExists((XVar)(fieldName));
		}
		public override XVar setFieldSubstitutions(dynamic _param_fieldSubs)
		{
			#region pass-by-value parameters
			dynamic fieldSubs = XVar.Clone(_param_fieldSubs);
			#endregion

			this.dataResult.setFieldSubstitutions((XVar)(fieldSubs));

			return null;
		}
		public override XVar substituteFields(ref dynamic data)
		{
			return this.dataResult.substituteFields(ref data);
		}
		public override XVar randomAccess()
		{
			return this.dataResult.randomAccess();
		}
		public override XVar position()
		{
			return this.dataResult.position();
		}
	}
}
