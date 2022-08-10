using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 用于HiSql初始安装时使用
    /// author:tansar mail:tansar@126.com
    /// </summary>
    public class CodeFirst : ICodeFirst
    {
        public HiSqlClient SqlClient
        {
            get {
                return _sqlClient;
            }
            set { _sqlClient = value; }
        }

        private HiSqlClient _sqlClient;
        public CodeFirst(HiSqlClient sqlClient)
        {
            this._sqlClient = sqlClient;
        }
        public CodeFirst()
        { 
            
        }
        /// <summary>
        /// 暂不支持该功能
        /// </summary>
        public void CreateInitDataBase()
        {
            if (_sqlClient != null)
            {
                throw new Exception($"暂未支持该功能");
            }
            else
                throw new Exception($"请先指定数据库连接!");
        }

        /// <summary>
        /// 初次使用HiSql时请执行该方法
        /// 执行一次后不需要再执行
        /// </summary>
        public async Task InstallHisql()
        {
            if (_sqlClient != null)
            {
                bool _has_tabmodel = _sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_TabModel"]);
                bool _has_tabfield = _sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_FieldModel"]); 
                bool _has_domain = _sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_Domain"]);
                bool _has_element = _sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_DataElement"]);

                bool _isinstall = false;

                //系统表只有要一个表不存在就需要初始化安装
                if (!_has_tabmodel || !_has_tabfield || !_has_domain || !_has_element)
                {
                    installHisql();

                }
                else
                {
                    IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
                    idm.Context = this._sqlClient.Context;

                    IDbConfig dbConfig = Instance.CreateInstance<IDbConfig>($"{Constants.NameSpace}.{_sqlClient.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.Config.ToString()}");
                    dbConfig.Init();
                    DataSet ds_tab = idm.GetTabModelInfo(Constants.HiSysTable["Hi_TabModel"]);
                    DataSet ds_field = idm.GetTabModelInfo(Constants.HiSysTable["Hi_FieldModel"]);

                    //var lstfield= idm.GetIndexs(Constants.HiSysTable["Hi_FieldModel"]);
                    //string s1= idm.DropIndex(Constants.HiSysTable["Hi_FieldModel"], lstfield[0].IndexName, true);
                    //var lsttab = idm.GetIndexs(Constants.HiSysTable["Hi_TabModel"]);
                    //string s2=idm.DropIndex(Constants.HiSysTable["Hi_FieldModel"], lsttab[0].IndexName, true);

                    if (ds_tab.Tables.Count > 0)
                    {
                        if (!ds_tab.Tables[Constants.HiSysTable["Hi_FieldModel"]].Columns.Contains("DbServer") && !ds_field.Tables[Constants.HiSysTable["Hi_FieldModel"]].Columns.Contains("DbServer"))
                        {
                            //需要自定义升级


                            TabInfo tabInfo = idm.BuildTab(typeof(Hi_TabModel));

                            TabInfo tabInfo_2 = idm.GetTabStruct(Constants.HiSysTable["Hi_TabModel"]);

                            List<HiColumn> columns = new List<HiColumn>();

                            foreach (HiColumn col in tabInfo.GetColumns)
                            { 
                                HiColumn _col= tabInfo_2.Columns.Where(c=>c.FieldName.Equals(col.FieldName,StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                                if(_col != null)
                                    columns.Add(_col);
                                else
                                    columns.Add(col);
                            }
                            tabInfo.Columns = columns;
                            TabInfo tabInfo2=idm.BuildTab(typeof(Hi_FieldModel));

                            TabInfo tabInfo2_2 = idm.GetTabStruct(Constants.HiSysTable["Hi_FieldModel"]);

                            List<HiColumn> columns2 = new List<HiColumn>();

                            foreach (HiColumn col in tabInfo2.GetColumns)
                            {
                                HiColumn _col = tabInfo2_2.Columns.Where(c => c.FieldName.Equals(col.FieldName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                                if (_col != null)
                                    columns2.Add(_col);
                                else
                                    columns2.Add(col);
                            }
                            tabInfo2.Columns = columns2;



                            var tabresult = _sqlClient.DbFirst.ModiTable(tabInfo, OpLevel.Execute,true);
                            var fieldresult = _sqlClient.DbFirst.ModiTable(tabInfo2, OpLevel.Execute,true);

                            HiSqlCommProvider.RemoveTabInfoCache(tabInfo.TabModel.TabName);
                            HiSqlCommProvider.RemoveTabInfoCache(tabInfo2.TabModel.TabName);

                            _sqlClient.Context.DBO.ExecCommand(idm.BuildSqlCodeBlock( dbConfig.Delete_TabStruct.Replace("[$Schema$]", _sqlClient.CurrentConnectionConfig.Schema).Replace("[$TabName$]", Constants.HiSysTable["Hi_TabModel"])));
                            _sqlClient.Context.DBO.ExecCommand(idm.BuildSqlCodeBlock(dbConfig.Delete_TabStruct.Replace("[$Schema$]", _sqlClient.CurrentConnectionConfig.Schema).Replace("[$TabName$]", Constants.HiSysTable["Hi_FieldModel"])));

                            _sqlClient.Context.DMInitalize.GetTabStruct(Constants.HiSysTable["Hi_TabModel"]);
                            _sqlClient.Context.DMInitalize.GetTabStruct(Constants.HiSysTable["Hi_FieldModel"]);
                        }
                    }
                    
                }
               

                //如果启用了编号那么需要安装编号配置表
                if (Global.SnroOn)
                { 
                    bool _has_snro= _sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_Snro"]);
                    if (!_has_snro)
                    {
                        //如果不存在编号表则创建
                        TabInfo tabinfo_field = _sqlClient.Context.DMInitalize.BuildTab(typeof(Hi_Snro));

                        _sqlClient.DbFirst.CreateTable(tabinfo_field);
                    }
                }

                Tuple<bool, List<Hi_Version>> verinfo = checkVersion();
                upgradeVersions(_isinstall,verinfo.Item1, verinfo.Item2);


            }
            else
                throw new Exception($"请先指定数据库连接!");
        }



        bool installHisql()
        {
            
            _sqlClient.CommitTran();//提交之前的事务
            using (var hisqlClient = _sqlClient.CreateUnitOfWork())
            {
                try
                {
                    if (!_sqlClient.DbFirst.CheckTabExists(typeof(Hi_TabModel).Name))
                    {
                        hisqlClient.DbFirst.CreateTable(typeof(Hi_TabModel));
                    }
                    if (!_sqlClient.DbFirst.CheckTabExists(typeof(Hi_FieldModel).Name))
                    {
                        hisqlClient.DbFirst.CreateTable(typeof(Hi_FieldModel));
                    }

                    hisqlClient.Context.DMInitalize.GetTabStruct(typeof(Hi_TabModel).Name);
                    hisqlClient.Context.DMInitalize.GetTabStruct(typeof(Hi_FieldModel).Name);

                    if (!_sqlClient.DbFirst.CheckTabExists(typeof(Hi_Domain).Name))
                    {
                        hisqlClient.DbFirst.CreateTable(typeof(Hi_Domain));
                    }
                    if (!_sqlClient.DbFirst.CheckTabExists(typeof(Hi_DataElement).Name))
                    {
                        hisqlClient.DbFirst.CreateTable(typeof(Hi_DataElement));
                    }
                    hisqlClient.Context.DMInitalize.GetTabStruct(typeof(Hi_Domain).Name);
                    hisqlClient.Context.DMInitalize.GetTabStruct(typeof(Hi_DataElement).Name);
                    hisqlClient.CommitTran();
                }
                catch (Exception ex)
                {
                    hisqlClient.RollBackTran();
                    throw ex;
                }

            }


            return true;
        }



        /// <summary>
        /// 升级版本
        /// </summary>
        /// <param name="hasversion"></param>
        /// <param name="versions"></param>
        private void upgradeVersions(bool isinstall,bool hasversion, List<Hi_Version> versions)
        {
            string jssonver=HiSql.Properties.Resources.UpgradeVersion.ToString();
            List<Hi_UpgradeInfo> lstupgradeinfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Hi_UpgradeInfo>>(jssonver);
            if (lstupgradeinfo.Count > 0)
            {
                //当前版本号
                Version _curver = Constants.HiSqlVersion;
                if (!hasversion  )
                {
                    if (!isinstall)
                    {
                        //需要一个版本一个版本的升级
                        foreach (Hi_UpgradeInfo upgradeInfo in lstupgradeinfo)
                        {
                            upgradeVersion(_curver, upgradeInfo);
                        }
                    }
                }
                else
                {
                    List<Hi_UpgradeInfo> upgradeInfos = lstupgradeinfo.Where(u => u.MinVersion <= _curver && _curver < u.MaxVersion).ToList();
                    if (upgradeInfos.Count>0)
                    {
                        foreach (Hi_UpgradeInfo upgradeInfo in upgradeInfos)
                            upgradeVersion(_curver, upgradeInfo);

                        saveCurrVersion();
                    }
                }
            }
        }

        /// <summary>
        /// 升级到指定的版本
        /// </summary>
        /// <param name="curversion"></param>
        /// <param name="upgradeInfo"></param>
        private void upgradeVersion(Version curversion, Hi_UpgradeInfo upgradeInfo)
        {
            //逐个表更新
            foreach (Hi_UpgradeTab upgradeTab in upgradeInfo.UpgradTabs)
            {
                if (_sqlClient.DbFirst.CheckTabExists(upgradeTab.TabName))
                {
                    TabInfo tabinfo = _sqlClient.Context.DMInitalize.GetTabStruct(upgradeTab.TabName);
                    TabInfo tabinfo2 = tabinfo.CloneCopy();
                    foreach (Hi_UpgradeCol upgradeCol in upgradeTab.Columns)
                    {
                        if (upgradeCol.TabFieldAction == TabFieldAction.ADD || upgradeCol.TabFieldAction == TabFieldAction.MODI)
                        {
                            if (!tabinfo2.Columns.Any(c => c.FieldName.Equals(upgradeCol.ColumnInfo.FieldName, StringComparison.OrdinalIgnoreCase)))
                                tabinfo2.Columns.Add(upgradeCol.ColumnInfo);
                            else
                            {
                                for (int i = 0; i < tabinfo2.Columns.Count; i++)
                                {
                                    if (tabinfo2.Columns[i].FieldName.Equals(upgradeCol.ColumnInfo.FieldName, StringComparison.OrdinalIgnoreCase))
                                    {
                                        tabinfo2.Columns[i] = upgradeCol.ColumnInfo.CloneCopy();
                                    }
                                }
                            }
                        }
                        else if (upgradeCol.TabFieldAction == TabFieldAction.DELETE)
                        {
                            int _idx = 0;
                            bool _hascol = false;
                            for (int i = 0; i < tabinfo2.Columns.Count; i++)
                            {
                                if (tabinfo2.Columns[i].FieldName.Equals(upgradeCol.ColumnInfo.FieldName, StringComparison.OrdinalIgnoreCase))
                                {
                                    _idx = i;
                                    _hascol = true;
                                }
                            }
                            if (_hascol)
                                tabinfo2.Columns.RemoveAt(_idx);
                        }
                    }
                    var tuple = _sqlClient.DbFirst.ModiTable(tabinfo2, OpLevel.Execute);
                    Console.WriteLine(tuple.Item2);
                }
            }
        }


        /// <summary>
        /// 版本检测
        /// </summary>
        /// <returns></returns>
        private Tuple<bool, List<Hi_Version>> checkVersion()
        {
            bool _has_version = _sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_Version"]);
            Tuple<bool, List<Hi_Version>> rtn = new Tuple<bool, List<Hi_Version>>(false,null);
            List<Hi_Version> lstver = new List<Hi_Version>();
            if (!_has_version)
            {
                TabInfo tabinfo_field = _sqlClient.Context.DMInitalize.BuildTab(typeof(Hi_Version));
                _sqlClient.DbFirst.CreateTable(tabinfo_field);
                lstver=saveCurrVersion();
                rtn= new Tuple<bool, List<Hi_Version>>(false, lstver);
            }
            else
            {
                TabInfo tabinfo_field = _sqlClient.Context.DMInitalize.BuildTab(typeof(Hi_Version));
                lstver = _sqlClient.HiSql($"select * from {Constants.HiSysTable["Hi_Version"]} where HiPackName in (@HiPackName)",
                    new { HiPackName = new List<string> { Constants.NameSpace, _sqlClient.CurrentConnectionConfig.DbType.ToString() } })
                    .ToList<Hi_Version>();
                rtn = new Tuple<bool, List<Hi_Version>>(true, lstver);
            }
            return rtn;
        }

        /// <summary>
        /// 保存当前最新的版本号信息
        /// </summary>
        private List<Hi_Version> saveCurrVersion()
        {
            List<string> list = Constants.DbCurrentSupportList;
            Version version = Constants.HiSqlVersion;
            List<Hi_Version> lstver = new List<Hi_Version>();
            lstver.Add(new Hi_Version { HiPackName = Constants.NameSpace, Version = version.ToString(), VerNum = Convert.ToInt32(version.ToString().Replace(".", "")) });
            foreach (string n in list)
            {
                Version ver = Constants.GetDbTypeVersion(n);

          
                lstver.Add(new Hi_Version { HiPackName = n, Version = ver.ToString(),VerNum=Convert.ToInt32(ver.ToString().Replace(".", "")) });
            }
            if(lstver.Count > 0)
                _sqlClient.Modi(Constants.HiSysTable["Hi_Version"], lstver).ExecCommand();
            return lstver;
        }

        /// <summary>
        /// 创建表
        /// 可自行构建该类可实现动态创建类
        /// </summary>
        /// <param name="tabInfo"></param>
        /// <returns></returns>
        public bool CreateTable(TabInfo tabInfo)
        {
            if (_sqlClient != null)
            {
                return _sqlClient.Context.DMInitalize.BuildTabCreate(tabInfo) >0;

                
            }
            else
                throw new Exception($"请先指定数据库连接!");
        }

        /// <summary>
        /// 根据实体类型创建表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CreateTable(Type type)
        {
            if (_sqlClient != null)
            {
                TabInfo tabInfo = _sqlClient.Context.DMInitalize.BuildTab(type);
                return _sqlClient.Context.DMInitalize.BuildTabCreate(tabInfo) > 0;
            }
            else
                throw new Exception($"请先指定数据库连接!");
        }

        /// <summary>
        /// 修改表
        /// </summary>
        /// <param name="tabInfo"></param>
        /// <returns></returns>
        public bool ModiTable(TabInfo tabInfo)
        {
            if (_sqlClient != null)
            {
                throw new Exception($"暂未支持该功能");
            }
            else
                throw new Exception($"请先指定数据库连接!");
        }

        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public bool DropTable(string tabname,bool nolog=false)
        {
            if (_sqlClient != null)
            {
                var _table = new TableDefinition(tabname);
                if (_table.TableType == TableType.Entity)
                {
                    if (nolog)
                        _sqlClient.TrunCate(tabname).ExecCommand();
                    int v = _sqlClient.Drop(tabname).ExecCommand();
                    _sqlClient.Delete(Constants.HiSysTable["Hi_TabModel"].ToString(), new { TabName = tabname }).ExecCommand();
                    _sqlClient.Delete(Constants.HiSysTable["Hi_FieldModel"].ToString()).Where($"TabName='{tabname.ToSqlInject()}'").ExecCommand();
                    return v > 0;
                }
                else
                    return  _sqlClient.Drop(tabname).ExecCommand()>0;
            }
            else
                throw new Exception($"请先指定数据库连接!");
        }

        /// <summary>
        /// 清空表中数据
        /// 高风险操作
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public bool Truncate(string tabname)
        {
            if (_sqlClient != null)
            {
                _sqlClient.TrunCate(tabname).ExecCommand();
                return true;
            }
            else
                throw new Exception($"请先指定数据库连接!");
        }
    }
}
