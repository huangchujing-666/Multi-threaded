using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetData
{
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Transactions;
    public class productsourcestockDal
    {
        /// <summary>
        /// 本地服务器连接字符串
        /// </summary>
        public static string ClientConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["conStr"].ConnectionString.ToString();

        /// <summary>
        /// 服务器地址
        /// </summary>
        private string Server { get; set; }
        /// <summary>
        /// 服务器数据库
        /// </summary>
        private string Database { get; set; }
        /// <summary>
        /// 服务器uid
        /// </summary>
        private string Uid { get; set; }
        /// <summary>
        /// 服务器pwd
        /// </summary>
        private string Pwd { get; set; }
        /// <summary>
        /// 当前供应商下第几次更新
        /// </summary>
        private int SourceRenewIndex = 1;
        /// <summary>
        /// 供应商编号
        /// </summary>
        private string SourceCode;
        /// <summary>
        /// 连接超时次数 (更新本地数据)
        /// </summary>
        private static int SqlExceptionCount = 0;
        /// <summary>
        /// 连接异常次数 (更新本地数据)
        /// </summary>
        private static int InvalidOperationExceptionCount = 0;
        /// <summary>
        /// 连接超时次数  (获取服务器数据集合)
        /// </summary>
        private static int SqlExceptionServerCount = 0;
        /// <summary>
        /// 连接异常次数 (获取服务器数据集合)
        /// </summary>
        private static int InvalidOperationExceptionServerCount = 0;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public productsourcestockDal()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="SourceCode"></param>
        /// <param name="SourceRenewIndex"></param>
        public productsourcestockDal(string SourceCode, int SourceRenewIndex)
        {
            this.Server = "114.142.150.42,13120";//203.132.205.42,2019
            this.Database = "APOS2512";
            this.Uid = "china";
            this.Pwd = "china123";
            this.SourceRenewIndex = SourceRenewIndex;
            this.SourceCode = SourceCode;
        }

        #region 构造函数初始化数据源字段+productsourcestockDal(string SourcesAddress, string DataSources, string UserId, string UserPwd, int sourceRenewIndex)
        /// <summary>
        /// 初始化数据源字段
        /// </summary>
        /// <param name="SourcesAddress">服务器地址</param>
        /// <param name="DataSources">服务器数据库</param>
        /// <param name="UserId">服务器uid</param>
        /// <param name="UserPwd">服务器pwd</param>
        /// <param name="sourceRenewIndex">当前供应商下第几次更新</param>
        public productsourcestockDal(string SourcesAddress, string DataSources, string UserId, string UserPwd, int sourceRenewIndex, string SourceCode)
        {
            this.Server = SourcesAddress;
            this.Database = DataSources;
            this.Uid = UserId;
            this.Pwd = UserPwd;
            this.SourceRenewIndex = sourceRenewIndex;
            this.SourceCode = SourceCode;
        }
        #endregion

        #region 根据供应商编号获取数据源配置信息+productsourceConfigModel GetConfigu(string SourceCode)
        /// <summary>
        /// 根据供应商编号获取数据源配置信息
        /// </summary>
        /// <param name="SourceCode"></param>
        /// <returns></returns>
        public static productsourceConfigModel GetConfigu(string SourceCode)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ClientConnStr))
                {
                    conn.Open();
                    string sqlstr = "select * from productsourceConfig where SourceCode='" + SourceCode + "'";
                    SqlDataAdapter da = new SqlDataAdapter(sqlstr, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    productsourceConfigModel prosc = new productsourceConfigModel();
                    if (dt.Rows.Count > 0)
                    {
                        prosc.Id = int.Parse(dt.Rows[0]["Id"].ToString());
                        prosc.SourceCode = dt.Rows[0]["SourceCode"] == null ? "" : dt.Rows[0]["SourceCode"].ToString();
                        prosc.SourcesAddress = dt.Rows[0]["SourcesAddress"] == null ? "" : dt.Rows[0]["SourcesAddress"].ToString();
                        prosc.UserId = dt.Rows[0]["UserId"] == null ? "" : dt.Rows[0]["UserId"].ToString();
                        prosc.UserPwd = dt.Rows[0]["UserPwd"] == null ? "" : dt.Rows[0]["UserPwd"].ToString();
                        prosc.DataSources = dt.Rows[0]["DataSources"] == null ? "" : dt.Rows[0]["DataSources"].ToString();
                        prosc.DataSourcesLevel = dt.Rows[0]["DataSourcesLevel"] == null ? "" : dt.Rows[0]["DataSourcesLevel"].ToString();
                        prosc.TimeStart = dt.Rows[0]["TimeStart"] == null ? 0 : int.Parse(dt.Rows[0]["TimeStart"].ToString()) * 1000 * 60;
                        prosc.Def1 = dt.Rows[0]["Def1"] == null ? "" : dt.Rows[0]["Def1"].ToString();
                        prosc.Def2 = dt.Rows[0]["Def2"] == null ? "" : dt.Rows[0]["Def2"].ToString();
                        prosc.Def3 = dt.Rows[0]["Def3"] == null ? "" : dt.Rows[0]["Def3"].ToString();
                        prosc.Def4 = dt.Rows[0]["Def4"] == null ? "" : dt.Rows[0]["Def4"].ToString();
                        prosc.Def5 = dt.Rows[0]["Def5"] == null ? "" : dt.Rows[0]["Def5"].ToString();
                        return prosc;
                    }
                    return prosc;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region 判断远程连接服务器是否成功+IsRemoteCon(string SourcesAddress, string DataSources, string UserId, string UserPwd, out Exception es)
        /// <summary>
        /// 判断远程连接服务器是否成功
        /// </summary>
        /// <param name="SourcesAddress">服务亲地址</param>
        /// <param name="DataSources">数据库</param>
        /// <param name="UserId"></param>
        /// <param name="UserPwd"></param>
        /// <returns>连接是否成功</returns>
        public bool IsRemoteCon(string SourcesAddress, string DataSources, string UserId, string UserPwd, out Exception es)
        {

            string connstr = "server=" + SourcesAddress + ";database=" + DataSources + ";uid=" + UserId + ";pwd=" + UserPwd;
            try
            {
                using (SqlConnection conn = new SqlConnection(connstr))
                {
                    conn.Open();
                    es = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                es = ex;
                return false;
            }
        }
        #endregion
   
        #region 获取当前数据源下第几次更新本地数据+int GetSourceRenewIndex(string SourceCode, out string mess, out bool IsAddorUpdate)
        /// <summary>
        /// 根据供应商编号查询当前数据源下第几次更新本地数据
        /// </summary>
        /// <param name="SourceCode">供应商编号</param>
        /// <param name="mess"></param>
        /// <param name="IsAddorUpdate"></param>
        /// <returns></returns>
        public int GetSourceRenewIndex(string SourceCode, out string mess, out bool IsAddorUpdate)
        {
            mess = "";
            IsAddorUpdate = true;
            try
            {
                using (SqlConnection conn = new SqlConnection(ClientConnStr))
                {
                    conn.Open();
                    string sqlstr = "Select Def4 from errorlog where Def3='" + SourceCode + "' and Def4>0";
                    SqlCommand cmd = new SqlCommand(sqlstr, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    int counts = dt.Rows.Count;
                    if (counts > 0)
                    {
                        int index = (string.IsNullOrEmpty(dt.Rows[counts - 1]["Def4"] == null ? "" : dt.Rows[counts - 1]["Def4"].ToString())) ? 1 : int.Parse(dt.Rows[counts - 1]["Def4"].ToString()) + 1;
                        //查询上次更新是否完成
                        cmd.CommandText = "select count(*) from errorlog where Def3='" + SourceCode + "' and Def4='" + (index - 1) + "' and Def2='0'";
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            if (int.Parse(reader[0].ToString()) < 2)//上一次更新没有成功,继续更新上一次未完成的更新
                            {
                                index--;
                                IsAddorUpdate = false;//标记上次更新是否完成
                                UpdateErrorlog(SourceCode, index);
                                return index;
                            }
                        }
                        IsAddorUpdate = true;
                        return index;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                mess = ex.Message;
                return -1;
            }
        }
        #endregion

        #region 添加更新数据开始时间和结束时间+void AddErrorlog(string SourceCode, int addErrorRowNum, bool IsAddorUpdate)
        /// <summary>
        /// 添加数据开始时间和结束时间
        /// </summary>
        /// <param name="SourceCode">供应商编号</param>
        /// <param name="addErrorRowNum">第几次更新</param>
        /// <param name="IsAddorUpdate">是否为添加</param>
        public void AddErrorlog(string ErrorMsg,string SourceCode, int addErrorRowNum, bool IsAddorUpdate)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ClientConnStr))
                {
                    conn.Open();
                    string sqlstr = IsAddorUpdate ? "insert into errorlog (ErrorMsg,errorTime,operation,Def2,Def3,Def4) values('" + ErrorMsg + "','" + DateTime.Now + "','3','0','" + SourceCode + "','" + addErrorRowNum + "')" : "update errorlog set errorTime='" + DateTime.Now + "' where operation='3' and Def2='0' and Def3='" + SourceCode + "' and Def4='" + addErrorRowNum + "'";
                    SqlCommand cmd = new SqlCommand(sqlstr, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                AddErrorlog(ErrorMsg,SourceCode, addErrorRowNum, IsAddorUpdate);
            }
        }
        #endregion

        #region 新增错误日志+AddErrorlog(Exception ex, string ErrorDetails, int SourceRenewIndex, string SourceCode, int lineCount)
        /// <summary>
        /// 新增错误日志
        /// </summary>
        /// <param name="ex">错误信息</param>
        /// <param name="ErrorDetails">错误详情，包括查询错误或者更新某次数据错误</param>
        /// <param name="SourceRenewIndex">当前数据源更新的次数</param>
        /// <param name="SourceCode">供应商编号</param>
        /// <param name="lineCount">报错行数</param>
        private void AddErrorlog(Exception ex, string ErrorDetails, int SourceRenewIndex, string SourceCode, int lineCount)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ClientConnStr))
                {
                    string mess = "添加日志错误处行号：" + lineCount.ToString() + ex.Message.Replace(',', ' ');
                    conn.Open();
                    string sqlstr = "select Id from errorlog where Def2='" + ErrorDetails + "' and ErrorMsgDetails='" + mess + "' and Def3='" + SourceCode + "' and Def4='" + SourceRenewIndex + "'";
                    SqlCommand cmd = new SqlCommand(sqlstr, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        cmd.CommandText = "update errorlog set errorTime='" + DateTime.Now + "' where  Def2='" + ErrorDetails + "' and ErrorMsgDetails='" + mess + "' and Def3='" + SourceCode + "' and Def4='" + SourceRenewIndex + "'";
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string sqlstrs = "insert into errorlog (ErrorMsg,errorSrc,ErrorMsgDetails,errorTime,operation,Def2,Def3,Def4)" +
                            " values('" + ex.Source + "','GetData->productourcestockDal->" + ex.TargetSite + "','" + mess + "','" + DateTime.Now + "','3','" + ErrorDetails + "','" + SourceCode + "','" + SourceRenewIndex + "')";
                        cmd.CommandText = sqlstrs;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        #endregion
        /// <summary>
        /// 添加更新日志
        /// </summary>
        /// <param name="ErrorDetails"></param>
        /// <param name="SourceRenewIndex"></param>
        /// <param name="SourceCode"></param>
        /// <param name="lineCount"></param>
        private void AddErrorlog(string ErrorDetails, int SourceRenewIndex, string SourceCode, int lineCount,string Scode)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ClientConnStr))
                {
                    conn.Open();
                    string sqlstr = "select Id from errorlog where errorMsgDetails='" + ErrorDetails + "' and Def2='" + Scode + "' and Def3='" + SourceCode + "' and Def5='" + SourceRenewIndex + "'";
                    SqlCommand cmd = new SqlCommand(sqlstr, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        cmd.CommandText = "update errorlog set errorTime='" + DateTime.Now + "' where  errorMsgDetails='" + ErrorDetails + "' and Def2='"+Scode+"' and Def3='" + SourceCode + "' and Def5='" + SourceRenewIndex + "'";
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string sqlstrs = "insert into errorlog (errorTime,errorMsgDetails,Def2,Def3,Def5)" +
                            " values('"+DateTime.Now+"','" + ErrorDetails + "','"+Scode+"','" + SourceCode + "','" + SourceRenewIndex + "')";
                        cmd.CommandText = sqlstrs;
                        int i=cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        #region 更新错误日志+void UpdateErrorlog(string SourceCode, int SourceRenewIndex)
        /// <summary>
        /// 更新错误日志
        /// </summary>
        /// <param name="SourceCode">供应商编号</param>
        /// <param name="SourceRenewIndex">更新的次数</param>
        private void UpdateErrorlog(string SourceCode, int SourceRenewIndex)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ClientConnStr))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    string sqlstr = "update errorlog set errorTime='" + DateTime.Now + "' where operation='3' and Def2='0' and Def3='" + SourceCode + "' and Def4='" + SourceRenewIndex + "'";
                    SqlCommand cmd = new SqlCommand(sqlstr, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                DeleteErrorlog(SourceCode, SourceRenewIndex);
            }
        }
        #endregion

        #region 删除错误日志+DeleteErrorlog(string beginRow, int SourceRenewIndex, string SourceCode)
        /// <summary>
        /// 删除错误日志
        /// </summary>
        /// <param name="beginRow"></param>
        /// <param name="SourceRenewIndex">当前第几次查询</param>
        /// <param name="SourceCode">供应商编号</param>
        private void DeleteErrorlog(string beginRow, int SourceRenewIndex, string SourceCode)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ClientConnStr))
                {
                    conn.Open();
                    string sqlstr = "delete errorlog where Def2='" + beginRow + "' and Def3='" + SourceCode + "' and Def4='" + SourceRenewIndex + "'";
                    SqlCommand cmd = new SqlCommand(sqlstr, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 删除错误日志+void DeleteErrorlog(string SourceCode, int SourceRenewIndex)
        /// <summary>
        /// 删除错误日志
        /// </summary>
        /// <param name="SourceCode">供应商编号</param>
        /// <param name="SourceRenewIndex">当前第几次更新</param>
        private void DeleteErrorlog(string SourceCode, int SourceRenewIndex)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ClientConnStr))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    string sqlstr = "delete errorlog where operation='3' and Def2!='0' and Def3='" + SourceCode + "' and Def4='" + SourceRenewIndex + "'";
                    SqlCommand cmd = new SqlCommand();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //出现异常递归删除本次更新未开始时存在的数据
                DeleteErrorlog(SourceCode, SourceRenewIndex);
            }
        }
        #endregion

        #region 获取服务器数据量+int GetServerDBCount(out string mess)
        /// <summary>
        /// 获取服务器数据量
        /// </summary>
        /// <param name="mess">错误信息</param>
        /// <returns>数据量</returns>
        public int GetServerDBCount(out string mess)
        {
            mess = "";
            try
            {
                using (SqlConnection conn = new SqlConnection("Server=" + Server + ";DataBase=" + Database + ";uid=" + Uid + ";pwd=" + Pwd + ""))
                {
                    conn.Open();
                    string sqlstr = "SELECT COUNT(SCODE) FROM STOCK_VIEW WHERE STYLE is not null and STYLE<>''";
                    SqlCommand cmd = new SqlCommand(sqlstr, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    //将上一次更新未成功出现的同样错误信息珊瑚
                    DeleteErrorlog("获取远程服务器SCODE数量", SourceRenewIndex, SourceCode);
                    return dt.Rows.Count > 0 ? int.Parse(dt.Rows[0][0].ToString()) : 0;
                }
            }
            catch (Exception ex)
            {
                AddErrorlog(ex, "获取远程服务器SCODE数量", SourceRenewIndex, SourceCode, 424);
                mess = ex.Message;
                return 0;
            }
        }
        #endregion

        /// <summary>
        /// 服务器不存在，本地数据库存在的Scode数组
        /// </summary>
        List<string> RemoveClientScodeList = new List<string>();

        #region 将服务器已经不存在,本地存在的数据Scode放入Removescode集合+void GetDeleteClientDataList()
        /// <summary>
        /// 将服务器已经不存在,本地存在的数据Scode放入Removescode集合
        /// </summary>
        public void GetDeleteClientDataList()
        {
            SCODE Scode = GetServerClientSCODE();
            if (Scode.ServerSCODE.Count > 0)
            {
                foreach (string scode in Scode.ClientSCODE)
                {
                    if (!Scode.ServerSCODE.Contains(scode))
                    {
                        RemoveClientScodeList.Add(scode);
                    }
                }
            }
        }
        #endregion

        #region 得到被实例化服务器和本地数据库的SCODE集合的SCODE对象+SCODE GetServerClientSCODE()
        /// <summary>
        /// 得到被实例化服务器和本地数据库的SCODE集合的SCODE对象
        /// </summary>
        /// <returns></returns>
        private SCODE GetServerClientSCODE()
        {
            SCODE scode = new SCODE();
            try
            {
                SqlCommand cmd = new SqlCommand();
                using (SqlConnection conn = new SqlConnection(ClientConnStr))
                {
                    conn.Open();
                    cmd.CommandText = "select Scode from productsourcestock where Vencode='" + SourceCode + "'";
                    cmd.Connection = conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<string> cli = new List<string>();
                    while (reader.Read())
                    {
                        cli.Add(reader["Scode"].ToString());
                    }
                    scode.ClientSCODE = cli;
                }
                // //using (SqlConnection conns = new SqlConnection("server=" + Server + ";database=" + Database + ";uid=" + Uid + ";pwd=" + Pwd))
                // //{
                // SqlConnection conns = new SqlConnection("server=" + Server + ";database=" + Database + ";uid=" + Uid + ";pwd=" + Pwd);
                // conns.Open();
                // cmd.CommandText = "select SCODE from STOCK_VIEW";
                // //cmd.Connection = conns;
                // SqlDataReader readers = cmd.ExecuteReader();
                // List<string> sli = new List<string>();
                // while (readers.Read())
                // {
                //     sli.Add(readers["SCODE"].ToString());
                // }
                // scode.ServerSCODE = sli;
                //// }
                scode.ServerSCODE = GetServerScode();
                return scode;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region 获取远程服务器上面的scode字段集合+List<string> GetServerScode()
        /// <summary>
        /// 获取远程服务器上面的scode字段集合
        /// </summary>
        /// <returns></returns>
        public List<string> GetServerScode()
        {
            SqlConnection serverCon = new SqlConnection("server=" + Server + ";database=" + Database + ";uid=" + Uid + ";pwd=" + Pwd);
            try
            {
                if (serverCon.State == ConnectionState.Closed)
                    serverCon.Open();
                List<string> list = new List<string>();
                SqlCommand com = new SqlCommand();
                com.Connection = serverCon;
                com.CommandText = "select SCODE from STOCK_VIEW WHERE STYLE is not null and STYLE<>''";
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader["SCODE"].ToString().Trim());
                }
                com.Dispose();
                DeleteErrorlog("获取远程服务器所有SCODE", SourceRenewIndex, SourceCode);
                return list;
            }
            catch (Exception ex)
            {
                AddErrorlog(ex, "获取远程服务器所有SCODE", SourceRenewIndex, SourceCode, 220);
                return null;
            }
            finally
            {
                if (serverCon.State == ConnectionState.Open)
                    serverCon.Close();
            }
        }

        #endregion

        #region 删除本地数据库数据+void DelteClientProductsourcestock()
        /// <summary>
        /// 删除本地数据库数据
        /// </summary>
        public void DelteClientProductsourcestock()
        {
            try
            {
                if (RemoveClientScodeList.Count > 0)
                {
                    using (SqlConnection conn = new SqlConnection(ClientConnStr))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = conn;
                        foreach (string scode in RemoveClientScodeList)
                        {
                            cmd.CommandText = "delete productsourcestock where Scode='" + scode + "'";
                            cmd.ExecuteNonQuery();
                            //RemoveClientScodeList.Remove(scode);
                        }
                        RemoveClientScodeList.Clear();
                    }
                }
            }
            catch (Exception)
            {
                DelteClientProductsourcestock();
                return;
            }
        }
        #endregion

        #region 更新本地数据+bool UpdateClientDataTables(int skip, int take, out string message)
        /// <summary>
        /// 更新本地数据
        /// </summary>
        /// <param name="skip">跳过skip条数据</param>
        /// <param name="take">更新到第take条数据</param>
        /// <param name="message">错误信息</param>
        /// <returns>false则更新出错，true更新成功</returns>
        public bool UpdateClientDataTables(int skip, int take, out string message)
        {
            try
            {
                //创建数据表，存储要新增的行数据
                DataTable dt = CreateDataTable();
                ServerTables tables = new ServerTables();
                List<string> list = new List<string>();
                SqlConnection conn = new SqlConnection(ClientConnStr);
                //获取本地数据库数据
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                string sqlstr = "select SCODE from productsourcestock where Vencode='" + SourceCode + "'";
                SqlCommand cmd = new SqlCommand(sqlstr, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader["SCODE"].ToString());
                }
                reader.Close();
                //获取服务器数据集合
                tables.STOCK_VIEW_Model = GetServerScodeList(skip, take, out message);
                if (tables.STOCK_VIEW_Model == null)
                {
                    message = "GetData->productsourcestockDal->UpdateClientDataTables->ServerTables STOCK_VIEW_Model=null";
                    UpdateClientDataTables(skip, take, out message);
                    if (tables.STOCK_VIEW_Model == null)
                    {
                        return false;
                    }
                }
                foreach (STOCK_VIEW_Model st in tables.STOCK_VIEW_Model)
                {
                    if (!list.Contains(st.SCODE))//判断本地数据中是否已经存在服务器数据，是的话更改 否则新增
                    {
                        DataRow dr = dt.NewRow();
                        dr["Scode"] = st.SCODE != null ? st.SCODE : "";
                        dr["Bcode"] = st.BCODE != null ? st.BCODE : "";
                        dr["Bcode2"] = st.BCODE2 != null ? st.BCODE2 : "";
                        dr["Descript"] = st.DESCRIPT != null ? st.DESCRIPT : "";
                        dr["Cdescript"] = st.CDESCRIPT != null ? st.CDESCRIPT : "";
                        dr["Unit"] = st.UNIT != null ? st.UNIT : "";
                        dr["Currency"] = st.CURRENCY != null ? st.CURRENCY : "";
                        dr["Cat"] = st.CAT != null ? st.CAT : "";
                        dr["Cat1"] = st.CAT1 != null ? st.CAT1 : "";
                        dr["Cat2"] = st.CAT2 != null ? st.CAT2 : "";
                        dr["Clolor"] = st.COLOR != null ? st.COLOR : "";
                        dr["Size"] = st.SIZE != null ? st.SIZE : "";
                        dr["Style"] = st.STYLE != null ? st.STYLE : "";
                        dr["Pricea"] = st.PRICEA;
                        dr["Priceb"] = st.PRICEB;
                        dr["Pricec"] = st.PRICEC;
                        dr["Priced"] = st.PRICED;
                        dr["Pricee"] = st.PRICEE;
                        dr["Disca"] = st.DISCA;
                        dr["Discb"] = st.DISCB;
                        dr["Discc"] = st.DISCC;
                        dr["Discd"] = st.DISCD;
                        dr["Disce"] = st.DISCE;
                        dr["Vencode"] = SourceCode;
                        dr["Model"] = st.MODEL != null ? st.MODEL : "";
                        dr["Rolevel"] = st.RO_LEVEL;
                        dr["Roamt"] = st.RO_AMT;
                        dr["Stopsales"] = st.STOPSALES;
                        dr["Loc"] = st.LOC;
                        dr["Balance"] = st.BAL <= 0 ? 0 : st.BAL;
                        if (st.LAST_GRN_D == null)
                        {
                            dr["Lastgrnd"] = DBNull.Value;
                        }
                        else
                        {
                            dr["Lastgrnd"] = st.LAST_GRN_D;
                        }
                        dr["PrevStock"] = st.PrevStock;
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        st.BAL = st.BAL <= 0 ? 0 : st.BAL;
                        string sql = "update productsourcestock set Pricea=" + st.PRICEA + ",Priceb=" + st.PRICEB + ",Pricec=" + st.PRICEC + ",Priced=" + st.PRICED + ",Pricee=" + st.PRICEE + ",Disca=" + st.DISCA + ",Discb=" + st.DISCB + ",Discc=" + st.DISCC + ",Discd=" + st.DISCD + ",Disce=" + st.DISCE + ",PrevStock=Balance, Balance=" + st.BAL + " where SCODE='" + st.SCODE + "'";
                        cmd.CommandText = sql;
                        int i = cmd.ExecuteNonQuery();
                        if (i>0)
                        {
                           // AddErrorlog("SCODE为:" + st.SCODE.ToString() + ",Priceb=" + st.PRICEB + ",Pricec=" + st.PRICEC + ",Priced=" + st.PRICED + ",Pricee=" + st.PRICEE + ",Disca=" + st.DISCA + ",Discb=" + st.DISCB + ",Discc=" + st.DISCC + ",Discd=" + st.DISCD + ",Disce=" + st.DISCE + ",PrevStock=Balance, Balance=" + st.BAL +";当SCODE=" + st.SCODE.ToString() , SourceRenewIndex, true);
                           //  AddErrorlog("SCODE为:" + st.SCODE.ToString() + ",Priceb=" + st.PRICEB + ",Pricec=" + st.PRICEC + ",Priced=" + st.PRICED + ",Pricee=" + st.PRICEE + ",Disca=" + st.DISCA + ",Discb=" + st.DISCB + ",Discc=" + st.DISCC + ",Discd=" + st.DISCD + ",Disce=" + st.DISCE + ",PrevStock=Balance, Balance=" + st.BAL + ";当SCODE=" + st.SCODE.ToString(), SourceRenewIndex, SourceCode, 699,st.SCODE);
                        }
                        cmd.Dispose();
                    }
                    Thread.Sleep(60);
                }
                //批量更新
                if (!ExecuteTransactionScopeInsert(dt, out message))
                {
                    Exception ex = new Exception(message);
                    ex.Source = "ExecuteTransactionScopeInsert()批量提交";
                    AddErrorlog(ex, (skip + 1) + "-" + take, SourceRenewIndex, SourceCode, 628);
                    return false;
                }
                message = "";
                DeleteErrorlog((skip + 1) + "-" + take, SourceRenewIndex, SourceCode);//将上次更新数据产生的错误全部删除
                conn.Dispose();
                conn.Close();
                return true;
            }
            catch (SqlException ex)
            {
                if (SqlExceptionCount < 3)
                {
                    SqlExceptionCount++;
                    //连接超时 递归重新连接
                    AddErrorlog(ex, (skip + 1) + "-" + take, SourceRenewIndex, SourceCode, 692);
                    UpdateClientDataTables(skip, take, out message);
                    return true;
                }
                message = ex.Message;
                return false;
            }
            catch (InvalidOperationException ex)
            {
                if (InvalidOperationExceptionCount < 3)
                {
                    InvalidOperationExceptionCount++;
                    //连接异常  递归重新连接
                    AddErrorlog(ex, (skip + 1) + "-" + take, SourceRenewIndex, SourceCode, 692);
                    UpdateClientDataTables(skip, take, out message);
                    return true;
                }
                message = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                AddErrorlog(ex, (skip + 1) + "-" + take, SourceRenewIndex, SourceCode, 692);
                UpdateClientDataTables(skip, take, out message);
                message = ex.Message;
                return false;
            }
        }
        #endregion

        #region 获取源数据scode数据集合+List<STOCK_VIEW_Model> GetServerScodeList(int skip, int take, out string message)
        /// <summary>
        /// 获取源数据scode数据集合
        /// </summary>
        /// <param name="skip">从第skip条开始取数据</param>
        /// <param name="take">取到第take条</param>
        /// <param name="message">错误信息</param>
        /// <returns></returns>
        private List<STOCK_VIEW_Model> GetServerScodeList(int skip, int take, out string message)
        {
            message = "";
            List<STOCK_VIEW_Model> list = new List<STOCK_VIEW_Model>();
            try
            {
                using (SqlConnection conn = new SqlConnection("server=" + Server + ";database=" + Database + ";uid=" + Uid + ";pwd=" + Pwd))
                {
                    conn.Open();
                    string sqlstr = "select a.*,b.* from ( select row_number() over(order by SCODE) row_num," +
                                "SCODE,BCODE,BCODE2,DESCRIPT,CDESCRIPT,UNIT,CURRENCY,CAT,CAT1,CAT2,COLOR,SIZE,STYLE,PRICEA," +
                                "PRICEB,PRICEC,PRICED,PRICEE,DISCA,DISCB,DISCC,DISCD,DISCE,MODEL,RO_LEVEL,RO_AMT,STOPSALES," +
                                "LAST_GRN_D from STOCK_VIEW WHERE STYLE is not null and STYLE<>'') a left join " +
                                "(select StockBal_View.SCODE as SCODE1,SUM(StockBal_View.BALANCE) as BAL from StockBal_View WHERE LOC !='head' and LOC !='shop2' and LOC !='shop4' and LOC !='shop5' and LOC !='shop6' group by StockBal_View.SCODE) b on a.SCODE=b.SCODE1 " +
                                "where a.row_num between " + (skip + 1) + " and " + take;
                    SqlCommand cmd = new SqlCommand(sqlstr, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        STOCK_VIEW_Model stock = new STOCK_VIEW_Model();
                        stock.SCODE = reader["SCODE"] != null ? reader["SCODE"].ToString() : "";
                        stock.BCODE = reader["BCODE"] != null ? reader["BCODE"].ToString() : "";
                        stock.BCODE2 = reader["BCODE2"] != null ? reader["BCODE2"].ToString() : "";
                        stock.DESCRIPT = reader["DESCRIPT"] != null ? reader["DESCRIPT"].ToString() : "";
                        stock.CDESCRIPT = reader["CDESCRIPT"] != null ? reader["CDESCRIPT"].ToString() : "";
                        stock.UNIT = reader["UNIT"] != null ? reader["UNIT"].ToString() : "";
                        stock.CAT = reader["CAT"] != null ? reader["CAT"].ToString() : "";
                        stock.CAT1 = reader["CAT1"] != null ? reader["CAT1"].ToString() : "";
                        stock.CAT2 = reader["CAT2"] != null ? reader["CAT2"].ToString() : "";
                        stock.COLOR = reader["COLOR"] != null ? reader["COLOR"].ToString() : "";
                        stock.SIZE = reader["SIZE"] != null ? reader["SIZE"].ToString() : "";
                        stock.STYLE = reader["STYLE"] != null ? reader["STYLE"].ToString() : "";
                        stock.PRICEA = reader["PRICEA"] != null ? decimal.Parse(reader["PRICEA"].ToString()) : 0;
                        stock.PRICEB = reader["PRICEB"] != null ? decimal.Parse(reader["PRICEB"].ToString()) : 0;
                        stock.PRICEC = reader["PRICEC"] != null ? decimal.Parse(reader["PRICEC"].ToString()) : 0; ;
                        stock.PRICED = reader["PRICED"] != null ? decimal.Parse(reader["PRICED"].ToString()) : 0;
                        stock.PRICEE = reader["PRICEE"] != null ? decimal.Parse(reader["PRICEE"].ToString()) : 0;
                        stock.DISCA = reader["DISCA"] != null ? decimal.Parse(reader["DISCA"].ToString()) : 0;
                        stock.DISCB = reader["DISCB"] != null ? decimal.Parse(reader["DISCB"].ToString()) : 0;
                        stock.DISCC = reader["DISCC"] != null ? decimal.Parse(reader["DISCC"].ToString()) : 0;
                        stock.DISCD = reader["DISCD"] != null ? decimal.Parse(reader["DISCD"].ToString()) : 0;
                        stock.DISCE = reader["DISCE"] != null ? decimal.Parse(reader["DISCE"].ToString()) : 0;
                        stock.MODEL = reader["MODEL"] != null ? reader["MODEL"].ToString() : "";
                        stock.RO_LEVEL = reader["RO_LEVEL"] != null ? decimal.Parse(reader["RO_LEVEL"].ToString()) : 0;
                        stock.RO_AMT = reader["RO_AMT"] != null ? decimal.Parse(reader["RO_AMT"].ToString()) : 0;
                        if (reader["LAST_GRN_D"] != null && !string.IsNullOrWhiteSpace(reader["LAST_GRN_D"].ToString()))
                        {
                            stock.LAST_GRN_D = DateTime.Parse(reader["LAST_GRN_D"].ToString());
                        }
                        else
                        {
                            stock.LAST_GRN_D = null;
                        }
                        if (reader["BAL"] != null && !string.IsNullOrWhiteSpace(reader["BAL"].ToString()))
                        {
                            string str = reader["BAL"].ToString();
                            stock.BAL = int.Parse(str.Contains(".") ? str.Substring(0, str.Length - 5) : str);
                        }
                        else
                        {
                            stock.BAL = 0;
                        }
                        list.Add(stock);
                        //Thread.Sleep(10);
                    }
                    if (list == null || list.Count <= 0)
                    {
                        GetServerScodeList(skip, take, out message);
                    }
                    DeleteErrorlog((skip + 1) + "-" + take, SourceRenewIndex, SourceCode);
                    return list;
                }
            }
            catch (SqlException ex)
            {
                if (SqlExceptionServerCount < 3)
                {
                    SqlExceptionServerCount++;
                    GetServerScodeList(skip, take, out message);
                    return list;
                }
                message = ex.Message;
                return null;
            }
            catch (InvalidOperationException ex)
            {
                if (InvalidOperationExceptionServerCount < 3)
                {
                    InvalidOperationExceptionServerCount++;
                    GetServerScodeList(skip, take, out message);
                    return list;
                }
                message = ex.Message;
                return null;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                AddErrorlog(ex, (skip + 1) + "-" + take, SourceRenewIndex, SourceCode, 642);
                return null;
            }
        }
        #endregion

        private object locker = new object();
        #region 批量更新数据+bool ExecuteTransactionScopeInsert(DataTable dt, out string message)
        /// <summary>
        /// 批量更新数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="message"></param>
        /// <returns>false为更新失败，true为更新成功</returns>
        private bool ExecuteTransactionScopeInsert(DataTable dt, out string message)
        {
            message = "";
            bool flag = false;
            int count = dt.Rows.Count;
            if (count == 0)
            {
                return true;
            }
            int copyTimeout = 200;
            
            try
            {
                lock (locker)
                {
                    using (SqlConnection conn = new SqlConnection(ClientConnStr))
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            conn.Open();
                            using (SqlBulkCopy sbc = new SqlBulkCopy(conn))
                            {
                                sbc.DestinationTableName = "productsourcestock";
                                sbc.BatchSize = count;
                                sbc.BulkCopyTimeout = copyTimeout;
                                for (int i = 0; i < dt.Columns.Count; i++)
                                {
                                    //列映射定义数据源中的列和目标表中的列之间的关系 
                                    sbc.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                                }
                                sbc.WriteToServer(dt);
                                flag = true;
                                //有效的事务 
                                scope.Complete();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
            return flag;
        }
        #endregion

        #region 创建本地数据表+DataTable CreateDataTable()
        /// <summary>
        /// 创建本地数据表
        /// </summary>
        /// <returns></returns>
        private DataTable CreateDataTable()
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("Id", typeof(int));
            dt.Columns.Add(dc);
            DataColumn dc1 = new DataColumn("Scode", typeof(string));
            dt.Columns.Add(dc1);
            DataColumn dc2 = new DataColumn("Bcode", typeof(string));
            dt.Columns.Add(dc2);
            DataColumn dc3 = new DataColumn("Bcode2", typeof(string));
            dt.Columns.Add(dc3);
            DataColumn dc4 = new DataColumn("Descript", typeof(string));
            dt.Columns.Add(dc4);
            DataColumn dc5 = new DataColumn("Cdescript", typeof(string));
            dt.Columns.Add(dc5);
            DataColumn dc6 = new DataColumn("Unit", typeof(string));
            dt.Columns.Add(dc6);
            DataColumn dc7 = new DataColumn("Currency", typeof(string));
            dt.Columns.Add(dc7);
            DataColumn dc8 = new DataColumn("Cat", typeof(string));
            dt.Columns.Add(dc8);
            DataColumn dc9 = new DataColumn("Cat1", typeof(string));
            dt.Columns.Add(dc9);
            DataColumn dc10 = new DataColumn("Cat2", typeof(string));
            dt.Columns.Add(dc10);
            DataColumn dc11 = new DataColumn("Clolor", typeof(string));
            dt.Columns.Add(dc11);
            DataColumn dc12 = new DataColumn("Size", typeof(string));
            dt.Columns.Add(dc12);
            DataColumn dc13 = new DataColumn("Style", typeof(string));
            dt.Columns.Add(dc13);
            DataColumn dc14 = new DataColumn("Pricea", typeof(decimal));
            dt.Columns.Add(dc14);
            DataColumn dc15 = new DataColumn("Priceb", typeof(decimal));
            dt.Columns.Add(dc15);
            DataColumn dc16 = new DataColumn("Pricec", typeof(decimal));
            dt.Columns.Add(dc16);
            DataColumn dc17 = new DataColumn("Priced", typeof(decimal));
            dt.Columns.Add(dc17);
            DataColumn dc18 = new DataColumn("Pricee", typeof(decimal));
            dt.Columns.Add(dc18);
            DataColumn dc19 = new DataColumn("Disca", typeof(decimal));
            dt.Columns.Add(dc19);
            DataColumn dc20 = new DataColumn("Discb", typeof(decimal));
            dt.Columns.Add(dc20);
            DataColumn dc21 = new DataColumn("Discc", typeof(decimal));
            dt.Columns.Add(dc21);
            DataColumn dc22 = new DataColumn("Discd", typeof(decimal));
            dt.Columns.Add(dc22);
            DataColumn dc23 = new DataColumn("Disce", typeof(decimal));
            dt.Columns.Add(dc23);
            DataColumn dc24 = new DataColumn("Vencode", typeof(string));
            dt.Columns.Add(dc24);
            DataColumn dc25 = new DataColumn("Model", typeof(string));
            dt.Columns.Add(dc25);
            DataColumn dc26 = new DataColumn("Rolevel", typeof(int));
            dt.Columns.Add(dc26);
            DataColumn dc27 = new DataColumn("Roamt", typeof(int));
            dt.Columns.Add(dc27);
            DataColumn dc28 = new DataColumn("Stopsales", typeof(int));
            dt.Columns.Add(dc28);
            DataColumn dc29 = new DataColumn("Loc", typeof(string));
            dt.Columns.Add(dc29);
            DataColumn dc30 = new DataColumn("Balance", typeof(int));
            dt.Columns.Add(dc30);
            DataColumn dc31 = new DataColumn("Lastgrnd", typeof(DateTime));
            dt.Columns.Add(dc31);
            DataColumn dc32 = new DataColumn("Imagefile", typeof(string));
            dt.Columns.Add(dc32);
            DataColumn dc33 = new DataColumn("PrevStock", typeof(string));
            dt.Columns.Add(dc33);
            DataColumn dc34 = new DataColumn("Def2", typeof(string));
            dt.Columns.Add(dc34);
            DataColumn dc35 = new DataColumn("Def3", typeof(string));
            dt.Columns.Add(dc35);
            DataColumn dc36 = new DataColumn("Def4", typeof(string));
            dt.Columns.Add(dc36);
            DataColumn dc37 = new DataColumn("Def5", typeof(string));
            dt.Columns.Add(dc37);
            DataColumn dc38 = new DataColumn("Def6", typeof(string));
            dt.Columns.Add(dc38);
            DataColumn dc39 = new DataColumn("Def7", typeof(string));
            dt.Columns.Add(dc39);
            DataColumn dc40 = new DataColumn("Def8", typeof(string));
            dt.Columns.Add(dc40);
            DataColumn dc41 = new DataColumn("Def9", typeof(string));
            dt.Columns.Add(dc41);
            DataColumn dc42 = new DataColumn("Def10", typeof(string));
            dt.Columns.Add(dc42);
            DataColumn dc43 = new DataColumn("Def11", typeof(string));
            dt.Columns.Add(dc43);
            return dt;
        }
        #endregion

        #region 再次获取本次更新失败的数据List<int[]> AgainRenew(string SourceCode, int SourceRenewIndex, out string mess)
        /// <summary>
        /// 再次获取本次更新失败的数据
        /// </summary>
        /// <param name="SourceCode">供应商编号</param>
        /// <param name="SourceRenewIndex">更新次数</param>
        /// <param name="mess">错误信息</param>
        /// <returns>返回一个存有</returns>
        public List<int[]> AgainRenew(string SourceCode, int SourceRenewIndex, out string mess)
        {
            mess = "";
            List<int[]> list = new List<int[]>();
            try
            {
                using (SqlConnection conn = new SqlConnection(ClientConnStr))
                {
                    conn.Open();
                    string sqlstr = "select Def2 from errorlog where Def3='" + SourceCode + "' and Def4='" + SourceRenewIndex + "' and operation='3'";
                    SqlCommand cmd = new SqlCommand(sqlstr, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string str = reader["Def2"].ToString();
                        string[] strs = str.Split('-');
                        int[] index;
                        if (IsRowIndex(strs, out index))
                        {
                            list.Add(index);
                        }
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                mess = ex.Message;
                return null;
            }
        }
        #endregion

        #region 判断数组是否为int类型,是的话转化为int数组bool IsRowIndex(string[] strs, out int[] index)
        /// <summary>
        /// 判断数组是否为int类型,是的话转化为int数组
        /// </summary>
        /// <param name="strs">传入的数组</param>
        /// <param name="index">返回的int数组</param>
        /// <returns>返回是否能转换为数组集合，int数组集合</returns>
        public bool IsRowIndex(string[] strs, out int[] index)
        {
            index = new int[2];
            try
            {
                if (strs.Length == 2)
                {
                    index[0] = int.Parse(strs[0].ToString())-1;
                    index[1] = int.Parse(strs[1].ToString());
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
