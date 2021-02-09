using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetData
{
    /// <summary>
    /// 线程传值类
    /// </summary>
    public class MSG
    {
        public SCODE scode { get; set; }
        public int i { get; set; }
        public List<int[]> list { get; set; }
    }
}
