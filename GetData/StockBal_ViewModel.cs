using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetData
{
    public class StockBal_ViewModel
    {
        /// <summary>
        /// SCODE
        /// </summary>		
        private string _scode;
        public string SCODE
        {
            get { return _scode; }
            set { _scode = value; }
        }
        /// <summary>
        /// LOC
        /// </summary>		
        private string _loc;
        public string LOC
        {
            get { return _loc; }
            set { _loc = value != null ? value.Trim() : ""; }
        }
        /// <summary>
        /// BALANCE
        /// </summary>		
        private int _balance;
        public int BALANCE
        {
            get { return _balance; }
            set { _balance = value; }
        }
    }
}
