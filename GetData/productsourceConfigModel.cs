using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetData
{
    /// <summary>
    /// 数据源配置信息
    /// </summary>
    public class productsourceConfigModel
    {
        /// <summary>
        /// 编号
        /// </summary>		
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// 供应商编号
        /// </summary>		
        private string _sourcecode;
        public string SourceCode
        {
            get { return _sourcecode; }
            set { _sourcecode = value; }
        }
        /// <summary>
        /// 服务器ip地址
        /// </summary>		
        private string _sourcesaddress;
        public string SourcesAddress
        {
            get { return _sourcesaddress; }
            set { _sourcesaddress = value; }
        }
        /// <summary>
        /// 用户名
        /// </summary>		
        private string _userid;
        public string UserId
        {
            get { return _userid; }
            set { _userid = value; }
        }
        /// <summary>
        /// 密码
        /// </summary>		
        private string _userpwd;
        public string UserPwd
        {
            get { return _userpwd; }
            set { _userpwd = value; }
        }
        /// <summary>
        /// DataSources
        /// </summary>		
        private string _datasources;
        public string DataSources
        {
            get { return _datasources; }
            set { _datasources = value; }
        }
        /// <summary>
        /// DataSourcesLevel
        /// </summary>		
        private string _datasourceslevel;
        public string DataSourcesLevel
        {
            get { return _datasourceslevel; }
            set { _datasourceslevel = value; }
        }
        /// <summary>
        /// TimeStart
        /// </summary>		
        private int _timestart;
        public int TimeStart
        {
            get { return _timestart; }
            set { _timestart = value; }
        }
        /// <summary>
        /// Def1
        /// </summary>		
        private string _def1;
        public string Def1
        {
            get { return _def1; }
            set { _def1 = value; }
        }
        /// <summary>
        /// Def2
        /// </summary>		
        private string _def2;
        public string Def2
        {
            get { return _def2; }
            set { _def2 = value; }
        }
        /// <summary>
        /// Def3
        /// </summary>		
        private string _def3;
        public string Def3
        {
            get { return _def3; }
            set { _def3 = value; }
        }
        /// <summary>
        /// Def4
        /// </summary>		
        private string _def4;
        public string Def4
        {
            get { return _def4; }
            set { _def4 = value; }
        }
        /// <summary>
        /// Def5
        /// </summary>		
        private string _def5;
        public string Def5
        {
            get { return _def5; }
            set { _def5 = value; }
        }       
    }
}
