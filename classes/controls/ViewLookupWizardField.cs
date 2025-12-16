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
	public partial class ViewLookupWizardField : ViewControl
	{
		public dynamic nLookupType;
		public dynamic lookupTable;
		public dynamic displayFieldName;
		public dynamic linkFieldName;
		public ProjectSettings pSet;
		public dynamic lookupPSet;
		public dynamic cipherer;
		public dynamic lookupQueryObj;
		public dynamic displayFieldIndex;
		public dynamic LookupSQL;
		public dynamic resolvedLookupValues = XVar.Array();
		public dynamic resolvedLinkLookupValues = XVar.Array();
		public dynamic linkFieldIndex;
		protected dynamic lookupDataSource;
		protected static bool skipViewLookupWizardFieldCtor = false;
		public ViewLookupWizardField(dynamic _param_field, dynamic _param_container, dynamic _param_pageObject)
			:base((XVar)_param_field, (XVar)_param_container, (XVar)_param_pageObject)
		{
			if(skipViewLookupWizardFieldCtor)
			{
				skipViewLookupWizardFieldCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic container = XVar.Clone(_param_container);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			#endregion

			this.lookupPSet = new XVar(null);
			this.cipherer = new XVar(null);
			if(this.container.pSet.getEditFormat((XVar)(field)) != Constants.EDIT_FORMAT_LOOKUP_WIZARD)
			{
				this.pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.container.pSet._table)));
				this.pSet.setPageType((XVar)(this.container.pageType));
				this.pSet.setPageType((XVar)(this.container.pSet.getPageTypeByFieldEditFormat((XVar)(field), new XVar(Constants.EDIT_FORMAT_LOOKUP_WIZARD))));
			}
			else
			{
				this.pSet = XVar.UnPackProjectSettings(this.container.pSet);
			}
			this.nLookupType = XVar.Clone(this.pSet.getLookupType((XVar)(this.field)));
			this.lookupTable = XVar.Clone(this.pSet.getLookupTable((XVar)(this.field)));
			this.lookupDataSource = XVar.Clone(CommonFunctions.getLookupDataSource((XVar)(this.field), (XVar)(this.pSet)));
			this.displayFieldName = XVar.Clone(this.pSet.getDisplayField((XVar)(this.field)));
			this.linkFieldName = XVar.Clone(this.pSet.getLinkField((XVar)(this.field)));
			this.linkAndDisplaySame = XVar.Clone(this.displayFieldName == this.linkFieldName);
			if(this.nLookupType == Constants.LT_QUERY)
			{
				this.lookupPSet = XVar.Clone(new ProjectSettings((XVar)(this.lookupTable), (XVar)(this.container.pageType)));
				this.cipherer = XVar.Clone(new RunnerCipherer((XVar)(this.lookupTable)));
			}
			else
			{
				this.cipherer = XVar.Clone(new RunnerCipherer((XVar)(this.pSet._table)));
			}
			this.localControlsContainer = XVar.Clone(new ViewControlsContainer((XVar)(this.pSet), (XVar)(this.container.pageType), (XVar)(pageObject)));
			this.localControlsContainer.isLocal = new XVar(true);
		}
		protected virtual XVar getLookupCommand(dynamic _param_values, dynamic _param_withoutWhere)
		{
			#region pass-by-value parameters
			dynamic values = XVar.Clone(_param_values);
			dynamic withoutWhere = XVar.Clone(_param_withoutWhere);
			#endregion

			dynamic dc = null, displayField = null, displayFieldAlias = null, filters = XVar.Array(), linkField = null, multiselect = null, valueConditions = XVar.Array();
			ProjectSettings pSet;
			dc = XVar.Clone(new DsCommand());
			pSet = XVar.UnPackProjectSettings(this.pSet);
			displayField = XVar.Clone(pSet.getDisplayField((XVar)(this.field)));
			if(XVar.Pack(this.pSet.getCustomDisplay((XVar)(this.field))))
			{
				displayFieldAlias = XVar.Clone(CommonFunctions.generateAlias());
				dc.extraColumns.InitAndSetArrayItem(new DsFieldData((XVar)(displayField), (XVar)(displayFieldAlias), new XVar("")), null);
			}
			else
			{
				displayFieldAlias = XVar.Clone(displayField);
			}
			filters = XVar.Clone(XVar.Array());
			linkField = XVar.Clone(pSet.getLinkField((XVar)(this.field)));
			multiselect = XVar.Clone(pSet.multiSelect((XVar)(this.field)));
			valueConditions = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> v in values.GetEnumerator())
			{
				valueConditions.InitAndSetArrayItem(DataCondition.FieldEquals((XVar)(linkField), (XVar)(v.Value)), null);
			}
			filters.InitAndSetArrayItem(DataCondition._Or((XVar)(valueConditions)), null);
			if(XVar.Pack(!(XVar)(withoutWhere)))
			{
				filters.InitAndSetArrayItem(DataCondition.SQLCondition((XVar)(CommonFunctions.prepareLookupWhere((XVar)(this.field), (XVar)(pSet)))), null);
			}
			filters.InitAndSetArrayItem(Security.SelectCondition(new XVar("S"), (XVar)(this.lookupPSet), new XVar(true)), null);
			dc.filter = XVar.Clone(DataCondition._And((XVar)(filters)));
			return new XVar("command", dc, "displayAlias", displayFieldAlias);
		}
		protected virtual XVar getDecryptLookupValue(dynamic _param_lookupValue)
		{
			#region pass-by-value parameters
			dynamic lookupValue = XVar.Clone(_param_lookupValue);
			#endregion

			if((XVar)(this.nLookupType == Constants.LT_QUERY)  || (XVar)(this.linkAndDisplaySame))
			{
				return this.cipherer.DecryptField((XVar)((XVar.Pack(this.nLookupType == Constants.LT_QUERY) ? XVar.Pack(this.displayFieldName) : XVar.Pack(this.field))), (XVar)(lookupValue));
			}
			return lookupValue;
		}
		protected virtual XVar getLookupValues(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic resolved = XVar.Array(), ret = XVar.Array(), unresolved = XVar.Array(), values = XVar.Array();
			if(XVar.Pack(this.pSet.multiSelect((XVar)(this.field))))
			{
				values = XVar.Clone(CommonFunctions.splitLookupValues((XVar)(value)));
			}
			else
			{
				values = XVar.Clone(new XVar(0, value));
			}
			unresolved = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> v in values.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(this.resolvedLookupValues[v.Value])))
				{
					unresolved.InitAndSetArrayItem(v.Value, null);
				}
			}
			if((XVar)(unresolved)  && (XVar)(this.pSet.isLookupWhereSet((XVar)(this.field))))
			{
				dynamic oldUnresolved = XVar.Array();
				resolved = XVar.Clone(this.fetchLookupValues((XVar)(unresolved), new XVar(false)));
				if(XVar.Pack(resolved))
				{
					foreach (KeyValuePair<XVar, dynamic> r in resolved.GetEnumerator())
					{
						this.resolvedLookupValues.InitAndSetArrayItem(r.Value["display"], r.Value["link"]);
					}
				}
				oldUnresolved = XVar.Clone(unresolved);
				unresolved = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> v in oldUnresolved.GetEnumerator())
				{
					if(XVar.Pack(!(XVar)(this.resolvedLookupValues[v.Value])))
					{
						unresolved.InitAndSetArrayItem(v.Value, null);
					}
				}
			}
			if(XVar.Pack(unresolved))
			{
				resolved = XVar.Clone(this.fetchLookupValues((XVar)(unresolved), new XVar(true)));
				if(XVar.Equals(XVar.Pack(resolved), XVar.Pack(false)))
				{
					MVCFunctions.showError((XVar)(this.lookupDataSource.lastError()));
				}
				else
				{
					foreach (KeyValuePair<XVar, dynamic> r in resolved.GetEnumerator())
					{
						this.resolvedLookupValues.InitAndSetArrayItem(r.Value["display"], r.Value["link"]);
					}
				}
			}
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> v in values.GetEnumerator())
			{
				if(XVar.Pack(this.resolvedLookupValues[v.Value]))
				{
					ret.InitAndSetArrayItem(new XVar("link", v.Value, "display", this.resolvedLookupValues[v.Value]), null);
				}
				else
				{
					ret.InitAndSetArrayItem(new XVar("link", v.Value, "display", v.Value), null);
				}
			}
			return ret;
		}
		protected virtual XVar fetchLookupValues(dynamic _param_values, dynamic _param_withoutWhere)
		{
			#region pass-by-value parameters
			dynamic values = XVar.Clone(_param_values);
			dynamic withoutWhere = XVar.Clone(_param_withoutWhere);
			#endregion

			dynamic data = XVar.Array(), dc = null, dispAlias = null, fetchedLinkValues = XVar.Array(), linkValue = null, lookupCommand = XVar.Array(), ret = XVar.Array(), rs = null;
			lookupCommand = XVar.Clone(this.getLookupCommand((XVar)(values), (XVar)(withoutWhere)));
			dc = XVar.Clone(lookupCommand["command"]);
			dispAlias = XVar.Clone(lookupCommand["displayAlias"]);
			rs = XVar.Clone(this.lookupDataSource.getList((XVar)(dc)));
			if(XVar.Pack(!(XVar)(rs)))
			{
				MVCFunctions.showError((XVar)(this.lookupDataSource.lastError()));
				return false;
			}
			ret = XVar.Clone(XVar.Array());
			fetchedLinkValues = XVar.Clone(XVar.Array());
			while(XVar.Pack(data = XVar.Clone(rs.fetchAssoc())))
			{
				linkValue = XVar.Clone(data[this.linkFieldName]);
				if(XVar.Pack(!(XVar)(fetchedLinkValues[linkValue])))
				{
					ret.InitAndSetArrayItem(new XVar("link", linkValue, "display", data[dispAlias]), null);
					fetchedLinkValues.InitAndSetArrayItem(true, linkValue);
				}
			}
			return ret;
		}
		public override XVar showDBValue(dynamic data, dynamic _param_keylink, dynamic _param_html = null)
		{
			#region default values
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			dynamic localData = XVar.Array(), lookupValues = XVar.Array(), outValues = XVar.Array(), value = null;
			value = XVar.Clone(data[this.field]);
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(value)))))
			{
				return "";
			}
			if(XVar.Pack(!(XVar)(this.lookupTable)))
			{
				return value;
			}
			outValues = XVar.Clone(XVar.Array());
			localData = XVar.Clone(data);
			lookupValues = XVar.Clone(this.getLookupValues((XVar)(value)));
			foreach (KeyValuePair<XVar, dynamic> lv in lookupValues.GetEnumerator())
			{
				dynamic displayValue = null, linkValue = null, outputValue = null;
				linkValue = XVar.Clone(lv.Value["link"]);
				displayValue = XVar.Clone(lv.Value["display"]);
				this.localControlsContainer.linkFieldValues.InitAndSetArrayItem(data[this.field], this.field);
				this.localControlsContainer.originlinkValues.InitAndSetArrayItem(linkValue, this.field);
				if(this.pSet.getViewFormat((XVar)(this.field)) != "Custom")
				{
					localData.InitAndSetArrayItem(displayValue, this.field);
				}
				outputValue = XVar.Clone(this.localControlsContainer.showDBValue((XVar)(this.field), (XVar)(localData), (XVar)(keylink), (XVar)(displayValue), (XVar)(html)));
				if((XVar)(html)  && (XVar)(!(XVar)(this.container.forExport)))
				{
					outputValue = XVar.Clone(MVCFunctions.Concat("<span class=\"r-lookup-value\">", outputValue, "</span>"));
				}
				outValues.InitAndSetArrayItem(outputValue, null);
			}
			return MVCFunctions.implode((XVar)((XVar.Pack(html) ? XVar.Pack("<span class=\"r-lookup-sep\">,</span>") : XVar.Pack(","))), (XVar)(outValues));
		}
		public override XVar getTextValue(dynamic data)
		{
			dynamic localData = XVar.Array(), lookupValues = XVar.Array(), textValues = XVar.Array(), value = null;
			value = XVar.Clone(data[this.field]);
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(value)))))
			{
				return "";
			}
			textValues = XVar.Clone(XVar.Array());
			localData = XVar.Clone(data);
			lookupValues = XVar.Clone(this.getLookupValues((XVar)(value)));
			foreach (KeyValuePair<XVar, dynamic> lv in lookupValues.GetEnumerator())
			{
				dynamic displayValue = null, linkValue = null;
				linkValue = XVar.Clone(lv.Value["link"]);
				displayValue = XVar.Clone(lv.Value["display"]);
				if(this.pSet.getViewFormat((XVar)(this.field)) != "Custom")
				{
					localData.InitAndSetArrayItem(displayValue, this.field);
				}
				textValues.InitAndSetArrayItem(this.localControlsContainer.getControl((XVar)(this.field)).getTextValue((XVar)(localData)), null);
			}
			return MVCFunctions.implode(new XVar(","), (XVar)(textValues));
		}
		public override XVar getExportValue(dynamic data, dynamic _param_keylink = null, dynamic _param_html = null)
		{
			#region default values
			if(_param_keylink as Object == null) _param_keylink = new XVar("");
			if(_param_html as Object == null) _param_html = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			this.localControlsContainer.setForExportVar((XVar)(this.container.forExport));
			return this.showDBValue((XVar)(data), (XVar)(keylink), (XVar)(html));
		}
	}
}
