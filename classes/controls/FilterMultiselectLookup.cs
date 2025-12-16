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
	public partial class FilterMultiselectLookup : FilterValuesList
	{
		protected static bool skipFilterMultiselectLookupCtor = false;
		public FilterMultiselectLookup(dynamic _param_fName, dynamic _param_pageObject, dynamic _param_id, dynamic _param_viewControls)
			:base((XVar)_param_fName, (XVar)_param_pageObject, (XVar)_param_id, (XVar)_param_viewControls)
		{
			if(skipFilterMultiselectLookupCtor)
			{
				skipFilterMultiselectLookupCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			dynamic id = XVar.Clone(_param_id);
			dynamic viewControls = XVar.Clone(_param_viewControls);
			#endregion

			this.aliases.InitAndSetArrayItem(this.fName, this.fName);
		}
		protected override XVar getDataCommand()
		{
			dynamic dc = null;
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(this.pageObject.getDataSourceFilterCriteria((XVar)(this.fName)));
			return dc;
		}
		protected override XVar addFilterBlocksFromDB(dynamic filterCtrlBlocks)
		{
			dynamic containsFilteredDependentOnDemandFilter = null, data = XVar.Array(), metaData = XVar.Array(), parentFiltersData = XVar.Array(), parentString = null, qResult = null, totalOption = null, values = XVar.Array(), visibilityClass = null;
			containsFilteredDependentOnDemandFilter = XVar.Clone((XVar)((XVar)(!(XVar)(this.dependent))  && (XVar)(!(XVar)(this.filtered)))  && (XVar)(this.hasFilteredDependentOnDemandFilter()));
			visibilityClass = XVar.Clone((XVar.Pack((XVar)(this.filtered)  && (XVar)(this.multiSelect == Constants.FM_ON_DEMAND)) ? XVar.Pack(this.onDemandHiddenItemClassName) : XVar.Pack("")));
			totalOption = XVar.Clone(this.pSet.getFilterFieldTotal((XVar)(this.fName)));
			qResult = XVar.Clone(this.dataSource.getList((XVar)(this.getDataCommand())));
			metaData = XVar.Clone(XVar.Array());
			while(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
			{
				this.decryptDataRow((XVar)(data));
				parentFiltersData = XVar.Clone(XVar.Array());
				parentString = new XVar("");
				if(XVar.Pack(this.dependent))
				{
					foreach (KeyValuePair<XVar, dynamic> pName in this.parentFiltersNames.GetEnumerator())
					{
						parentFiltersData.InitAndSetArrayItem(data[pName.Value], pName.Value);
					}
					parentString = XVar.Clone(MVCFunctions.runner_json_encode((XVar)(parentFiltersData)));
				}
				values = XVar.Clone(CommonFunctions.splitLookupValues((XVar)(data[this.fName])));
				foreach (KeyValuePair<XVar, dynamic> value in values.GetEnumerator())
				{
					dynamic hash = null;
					hash = XVar.Clone(MVCFunctions.md5((XVar)(MVCFunctions.Concat(parentString, value.Value))));
					if(XVar.Pack(!(XVar)(metaData[hash])))
					{
						dynamic total = null;
						total = XVar.Clone(data[this.totalsfName]);
						if(totalOption == Constants.FT_COUNT)
						{
							total = new XVar(1);
						}
						metaData.InitAndSetArrayItem(new XVar("rawValue", value.Value, "total", total, "parent", parentFiltersData), hash);
					}
					else
					{
						if(totalOption == Constants.FT_COUNT)
						{
							metaData.InitAndSetArrayItem(metaData[hash]["total"] + 1, hash, "total");
						}
						else
						{
							if(totalOption == Constants.FT_MIN)
							{
								metaData.InitAndSetArrayItem(MVCFunctions.min((XVar)(data[this.totalsfName]), (XVar)(metaData[hash]["total"])), hash, "total");
							}
							else
							{
								if(totalOption == Constants.FT_MAX)
								{
									metaData.InitAndSetArrayItem(MVCFunctions.max((XVar)(data[this.totalsfName]), (XVar)(metaData[hash]["total"])), hash, "total");
								}
							}
						}
					}
				}
			}
			foreach (KeyValuePair<XVar, dynamic> meta in metaData.GetEnumerator())
			{
				dynamic filterControl = null;
				data = XVar.Clone(XVar.Array());
				data.InitAndSetArrayItem(meta.Value["rawValue"], this.fName);
				data.InitAndSetArrayItem(meta.Value["total"], MVCFunctions.Concat(this.fName, "TOTAL"));
				this.valuesObtainedFromDB.InitAndSetArrayItem(meta.Value["rawValue"], null);
				filterControl = XVar.Clone(this.buildControl((XVar)(data), (XVar)(meta.Value["parent"])));
				if(XVar.Pack(containsFilteredDependentOnDemandFilter))
				{
					filterControl = XVar.Clone(MVCFunctions.Concat(this.getDelButtonHtml((XVar)(this.gfName), (XVar)(this.id), (XVar)(meta.Value["rawValue"])), filterControl));
				}
				filterCtrlBlocks.InitAndSetArrayItem(this.getFilterBlockStructure((XVar)(filterControl), (XVar)(visibilityClass), (XVar)(meta.Value["rawValue"]), (XVar)(meta.Value["parent"])), null);
			}

			return null;
		}
		public static XVar getFilterCondition(dynamic _param_fName, dynamic _param_value, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic value = XVar.Clone(_param_value);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			return DataCondition.FieldHasList((XVar)(fName), new XVar(Constants.dsopALL_IN_LIST), (XVar)(new XVar(0, value)));
		}
	}
}
