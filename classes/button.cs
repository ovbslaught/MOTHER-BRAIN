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
	public partial class Button : XClass
	{
		public dynamic keys = XVar.Array();
		public dynamic currentKeys = XVar.Array();
		public dynamic selectedKeys = XVar.Array();
		public dynamic isManyKeys = XVar.Pack(false);
		public dynamic location = XVar.Pack("");
		public dynamic nextInd;
		public dynamic table;
		public dynamic page;
		public dynamic tempFileNames = XVar.Array();
		public dynamic masterTable;
		public dynamic masterKeys;
		public Button(dynamic var_params)
		{
			CommonFunctions.RunnerApply(this, (XVar)(var_params));
			this.nextInd = new XVar(0);
			this.modifyKeys();
			this.separateKeys();
		}
		public virtual XVar separateKeys()
		{
			if(this.location == "grid")
			{
				if(XVar.Pack(this.isManyKeys))
				{
					dynamic i = null;
					this.currentKeys = XVar.Clone(this.keys[0]);
					i = new XVar(1);
					for(;i < MVCFunctions.count(this.keys); i++)
					{
						this.selectedKeys.InitAndSetArrayItem(this.keys[i], i - 1);
					}
				}
				else
				{
					this.currentKeys = XVar.Clone(this.keys);
				}
			}
			if(this.location == Constants.PAGE_LIST)
			{
				this.selectedKeys = XVar.Clone(this.keys);
				this.currentKeys = XVar.Clone(this.keys);
			}
			if((XVar)(this.location == Constants.PAGE_EDIT)  || (XVar)(this.location == Constants.PAGE_VIEW))
			{
				this.currentKeys = XVar.Clone(this.keys);
			}

			return null;
		}
		public virtual XVar modifyKeys()
		{
			dynamic keys = XVar.Array();
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.table), new XVar(""), (XVar)(this.page)));
			keys = XVar.Clone(XVar.Array());
			if(XVar.Pack(this.keys))
			{
				dynamic j = null, tKeysNamesArr = XVar.Array();
				tKeysNamesArr = XVar.Clone(pSet.getTableKeys());
				if(XVar.Pack(this.isManyKeys))
				{
					foreach (KeyValuePair<XVar, dynamic> value in this.keys.GetEnumerator())
					{
						dynamic recKeyArr = XVar.Array();
						keys.InitAndSetArrayItem(XVar.Array(), value.Key);
						recKeyArr = XVar.Clone(MVCFunctions.explode(new XVar("&"), (XVar)(value.Value)));
						j = new XVar(0);
						for(;j < MVCFunctions.count(tKeysNamesArr); j++)
						{
							if(XVar.Pack(recKeyArr.KeyExists(j)))
							{
								keys.InitAndSetArrayItem(MVCFunctions.urldecode((XVar)(recKeyArr[j])), value.Key, tKeysNamesArr[j]);
							}
						}
					}
				}
				else
				{
					dynamic keysReady = null;
					keysReady = new XVar(true);
					foreach (KeyValuePair<XVar, dynamic> kf in tKeysNamesArr.GetEnumerator())
					{
						if(XVar.Pack(!(XVar)(this.keys.KeyExists(kf.Value))))
						{
							keysReady = new XVar(false);
							break;
						}
					}
					if(XVar.Pack(keysReady))
					{
						return null;
					}
					j = new XVar(0);
					for(;j < MVCFunctions.count(tKeysNamesArr); j++)
					{
						keys.InitAndSetArrayItem(MVCFunctions.urldecode((XVar)(this.keys[j])), tKeysNamesArr[j]);
					}
				}
			}
			this.keys = XVar.Clone(keys);

			return null;
		}
		public virtual XVar getKeys()
		{
			return this.keys;
		}
		public virtual XVar getCurrentRecord()
		{
			return this.getRecordData();
		}
		public virtual XVar getNextSelectedRecord()
		{
			if(this.nextInd < MVCFunctions.count(this.selectedKeys))
			{
				dynamic data = null;
				data = XVar.Clone(this.getRecordData((XVar)(this.selectedKeys[this.nextInd])));
				this.nextInd += 1;
				return data;
			}
			return false;
		}
		public virtual XVar getRecordData(dynamic _param_keys = null)
		{
			#region default values
			if(_param_keys as Object == null) _param_keys = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic data = null, dataSource = null, dc = null, fetchedArray = null;
			ProjectSettings pSet;
			if(XVar.Pack(!(XVar)(keys)))
			{
				keys = XVar.Clone(this.currentKeys);
			}
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.table), new XVar(""), (XVar)(this.page)));
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(Security.SelectCondition(new XVar("S"), (XVar)(pSet)));
			dc.keys = XVar.Clone(keys);
			dataSource = XVar.Clone(CommonFunctions.getDataSource((XVar)(this.table), (XVar)(pSet)));
			fetchedArray = XVar.Clone(dataSource.getSingle((XVar)(dc)).fetchAssoc());
			data = XVar.Clone(GlobalVars.cipherer.DecryptFetchedArray((XVar)(fetchedArray)));
			return data;
		}
		public virtual XVar getMasterData(dynamic _param_masterTable)
		{
			#region pass-by-value parameters
			dynamic masterTable = XVar.Clone(_param_masterTable);
			#endregion

			if(XVar.Pack(XSession.Session.KeyExists(MVCFunctions.Concat(masterTable, "_masterRecordData"))))
			{
				return XSession.Session[MVCFunctions.Concat(masterTable, "_masterRecordData")];
			}
			return false;
		}
		public virtual XVar saveTempFile(dynamic _param_contents)
		{
			#region pass-by-value parameters
			dynamic contents = XVar.Clone(_param_contents);
			#endregion

			dynamic filename = null;
			filename = XVar.Clone(MVCFunctions.tempnam(new XVar(""), new XVar("")));
			MVCFunctions.runner_save_file((XVar)(filename), (XVar)(contents));
			this.tempFileNames.InitAndSetArrayItem(filename, null);
			return filename;
		}
		public virtual XVar deleteTempFiles()
		{
			foreach (KeyValuePair<XVar, dynamic> f in this.tempFileNames.GetEnumerator())
			{
				MVCFunctions.unlink((XVar)(f.Value));
			}

			return null;
		}
		public virtual XVar getMasterRecord()
		{
			dynamic dc = null, filters = XVar.Array(), masterDs = null, mpSet = null;
			ProjectSettings pSet;
			if(XVar.Pack(!(XVar)(this.masterTable)))
			{
				return null;
			}
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.table), new XVar(""), (XVar)(this.page)));
			mpSet = XVar.Clone(new ProjectSettings((XVar)(this.masterTable), new XVar(Constants.PAGE_LIST)));
			masterDs = XVar.Clone(CommonFunctions.getDataSource((XVar)(this.masterTable), (XVar)(mpSet)));
			filters = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> master in pSet.getMasterTables().GetEnumerator())
			{
				if(this.masterTable != master.Value["table"])
				{
					continue;
				}
				foreach (KeyValuePair<XVar, dynamic> mKeyField in master.Value["masterKeys"].GetEnumerator())
				{
					filters.InitAndSetArrayItem(DataCondition.FieldEquals((XVar)(mKeyField.Value), (XVar)(this.masterKeys[mKeyField.Key + 1])), null);
				}
			}
			filters.InitAndSetArrayItem(Security.SelectCondition(new XVar("S"), (XVar)(mpSet)), null);
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(DataCondition._And((XVar)(filters)));
			dc.reccount = new XVar(1);
			return masterDs.getList((XVar)(dc)).fetchAssoc();
		}
	}
}
