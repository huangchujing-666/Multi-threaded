using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetData
{
    public class STOCK_VIEW_Model
    {

        /// <summary>
        /// SCODE
        /// </summary>		
        private string _scode;
        public string SCODE
        {
            get { return _scode; }
            set { _scode = value != null ? value.Trim() : null; }
        }
        /// <summary>
        /// BCODE
        /// </summary>		
        private string _bcode;
        public string BCODE
        {
            get { return _bcode; }
            set { _bcode = value != null ? value.Trim() : null; }
        }
        /// <summary>
        /// BCODE2
        /// </summary>		
        private string _bcode2;
        public string BCODE2
        {
            get { return _bcode2; }
            set { _bcode2 = value != null ? value.Trim() : null; }
        }
        /// <summary>
        /// DESCRIPT
        /// </summary>		
        private string _descript;
        public string DESCRIPT
        {
            get { return _descript; }
            set { _descript = value != null ? value.Trim() : null; }
        }
        /// <summary>
        /// CDESCRIPT
        /// </summary>		
        private string _cdescript;
        public string CDESCRIPT
        {
            get { return _cdescript; }
            set { _cdescript = value != null ? value.Trim() : null; }
        }
        /// <summary>
        /// UNIT
        /// </summary>		
        private string _unit;
        public string UNIT
        {
            get { return _unit; }
            set { _unit = value != null ? value.Trim() : null; }
        }
        /// <summary>
        /// CURRENCY
        /// </summary>		
        private string _currency;
        public string CURRENCY
        {
            get { return _currency; }
            set { _currency = value != null ? value.Trim() : null; }
        }
        /// <summary>
        /// CAT
        /// </summary>		
        private string _cat;
        public string CAT
        {
            get { return _cat; }
            set { _cat = value != null ? value.Trim() : null; }
        }
        /// <summary>
        /// CAT1
        /// </summary>		
        private string _cat1;
        public string CAT1
        {
            get { return _cat1; }
            set { _cat1 = value != null ? value.Trim() : null; }
        }
        /// <summary>
        /// CAT2
        /// </summary>		
        private string _cat2;
        public string CAT2
        {
            get { return _cat2; }
            set { _cat2 = value != null ? value.Trim() : null; }
        }
        /// <summary>
        /// COLOR
        /// </summary>		
        private string _color;
        public string COLOR
        {
            get { return _color; }
            set { _color = value != null ? value.Trim() : null; }
        }
        /// <summary>
        /// SIZE
        /// </summary>		
        private string _size;
        public string SIZE
        {
            get { return _size; }
            set { _size = value != null ? value.Trim() : null; }
        }
        /// <summary>
        /// STYLE
        /// </summary>		
        private string _style;
        public string STYLE
        {
            get { return _style; }
            set { _style = value != null ? value.Trim() : null; }
        }
        /// <summary>
        /// PRICEA
        /// </summary>		
        private decimal _pricea;
        public decimal PRICEA
        {
            get { return _pricea; }
            set { _pricea = value; }
        }
        /// <summary>
        /// PRICEB
        /// </summary>		
        private decimal _priceb;
        public decimal PRICEB
        {
            get { return _priceb; }
            set { _priceb = value; }
        }
        /// <summary>
        /// PRICEC
        /// </summary>		
        private decimal _pricec;
        public decimal PRICEC
        {
            get { return _pricec; }
            set { _pricec = value; }
        }
        /// <summary>
        /// PRICED
        /// </summary>		
        private decimal _priced;
        public decimal PRICED
        {
            get { return _priced; }
            set { _priced = value; }
        }
        /// <summary>
        /// PRICEE
        /// </summary>		
        private decimal _pricee;
        public decimal PRICEE
        {
            get { return _pricee; }
            set { _pricee = value; }
        }
        /// <summary>
        /// DISCA
        /// </summary>		
        private decimal _disca;
        public decimal DISCA
        {
            get { return _disca; }
            set { _disca = value; }
        }
        /// <summary>
        /// DISCB
        /// </summary>		
        private decimal _discb;
        public decimal DISCB
        {
            get { return _discb; }
            set { _discb = value; }
        }
        /// <summary>
        /// DISCC
        /// </summary>		
        private decimal _discc;
        public decimal DISCC
        {
            get { return _discc; }
            set { _discc = value; }
        }
        /// <summary>
        /// DISCD
        /// </summary>		
        private decimal _discd;
        public decimal DISCD
        {
            get { return _discd; }
            set { _discd = value; }
        }
        /// <summary>
        /// DISCE
        /// </summary>		
        private decimal _disce;
        public decimal DISCE
        {
            get { return _disce; }
            set { _disce = value; }
        }
        /// <summary>
        /// MODEL
        /// </summary>		
        private string _model;
        public string MODEL
        {
            get { return _model; }
            set { _model = value != null ? value.Trim() : null; }
        }
        /// <summary>
        /// RO_LEVEL
        /// </summary>		
        private decimal _ro_level;
        public decimal RO_LEVEL
        {
            get { return _ro_level; }
            set { _ro_level = value; }
        }
        /// <summary>
        /// RO_AMT
        /// </summary>		
        private decimal _ro_amt;
        public decimal RO_AMT
        {
            get { return _ro_amt; }
            set { _ro_amt = value; }
        }
        /// <summary>
        /// STOPSALES
        /// </summary>		
        private int _stopsales;
        public int STOPSALES
        {
            get { return _stopsales; }
            set { _stopsales = value; }
        }
        /// <summary>
        /// LAST_GRN_D
        /// </summary>		
        private DateTime? _last_grn_d;
        public DateTime? LAST_GRN_D
        {
            get { return _last_grn_d; }
            set { _last_grn_d = value; }
        }
        /// <summary>
        /// IMAGE_FILE
        /// </summary>		
        private byte[] _image_file;
        public byte[] IMAGE_FILE
        {
            get { return _image_file; }
            set { _image_file = value; }
        }
        private int _prevStock;
        public int PrevStock
        {
            get { return _prevStock; }
            set { _prevStock = value; }
        }
        public int BAL { get; set; }
        public string LOC { get; set; }
    }
}
