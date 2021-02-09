using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GetData
{
    class Program
    {
        /// <summary>
        /// 本地服务器连接字符串
        /// </summary>
        private static string ClientConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["conStr"].ConnectionString.ToString();
        /// <summary>
        /// 存储香港服务器配置信息 实体对象
        /// </summary>
        private static productsourceConfigModel prosc;
        /// <summary>
        /// 是否第一次请求查询
        /// </summary>
        private static bool IsFirstStart = true;
        /// <summary>
        /// 供应商编号（主键）默认香港数据库 来自供应商表
        /// </summary>
        private static string SourceCode = "1";
        /// <summary>
        /// 当前供应商下第几次更新本地
        /// </summary>
        private static int SourceRenewIndex = -1;
        /// <summary>
        /// 每次更新的数据量
        /// </summary>
        private static int SetRenewalRowCount = 100;
        /// <summary>
        /// 设置同时开启多少个线程
        /// </summary>
        private static int SetThreadCount = 5;
        /// <summary>
        /// 是否单次更新
        /// </summary>
        private static bool IsRepetitionUpdate = true;
        /// <summary>
        /// 设置更新间隔
        /// </summary>
        private static int SleepTime = 2000;
        static void Main(string[] args)//
        {
            //string[] args = new string[2];
            //args[0] = "1";
            //args[1] = "true";
            Console.WriteLine("本地数据库：" + ClientConnStr);

            #region 1.当用户第一次请求查询，连接数据库，获取远程服务器配置信息
            if (IsFirstStart)
            {
                IsFirstStart = false;
                if (args.Length > 0 && args[0] != null)
                {
                    //1.1获取源数据数据库配置信息
                    prosc = productsourcestockDal.GetConfigu(args[0]);
                    if (prosc != null)
                    {
                        Console.WriteLine("获取数据数据源信息成功！" + (bool.Parse(args[1]) ? "更新一次" : "更新多次"));
                    }
                    else
                    {
                        Console.WriteLine("获取数据数据源信息失败！");
                        return;
                    }
                    productsourcestockDal prodal = new productsourcestockDal();
                    Exception es;
                    //1.2判断远程连接是否成功
                    bool IsCon = prodal.IsRemoteCon(prosc.SourcesAddress, prosc.DataSources, prosc.UserId, prosc.UserPwd, out es);
                    if (IsCon)
                    {
                        Console.WriteLine("数据库连接测试成功!");
                        SleepTime = prosc.TimeStart;//3600000毫秒，一小时自动更新一次
                        Console.WriteLine("数据源：server=" + prosc.SourcesAddress + ";database=" + prosc.DataSources + ";uid=" + prosc.UserId + ";pwd=" + prosc.UserPwd);
                    }
                    else
                    {
                        Console.WriteLine("数据库连接测试失败!失败原因：" + es.Message);
                    }
                }
                else
                {
                    Console.WriteLine("获取数据源信息失败！");
                    return;
                }
            }
            #endregion

            //2.是否为单次更新
            IsRepetitionUpdate = bool.Parse(args[1]);

            //3.修改控制台标题
            Console.Title = "更新数据库原始数据信息表" + prosc.SourcesAddress + ((args[1] == "true") ? " Once" : " 多次更新");

            //4.获取当前供应商下为第几次更新数据
            GetSourceRenewIndex();
            Console.WriteLine("当前数据源操作下第" + SourceRenewIndex + "次更新数据中...请勿关闭！");

            //5.开始更新本地数据库
            Run();

            //6.当第5步更新完毕之后，到错误日志表查询更新错误的记录
            #region MyRegion
            while (true)
            {
                if (threadCount == 0 && !IsDelete)
                {
                    
                    Thread thread = new Thread(NewRun);
                    thread.IsBackground = false;//
                    thread.Start();
                    thread.Join();//等待上面线程执行完毕
                    Console.WriteLine("结束时间:" + DateTime.Now);
                    productsourcestockDal prostds;
                    if (prosc != null)
                    {
                        prostds = new productsourcestockDal(prosc.SourcesAddress, prosc.DataSources, prosc.UserId, prosc.UserPwd, SourceRenewIndex, SourceCode);
                    }
                    else
                    {
                        prostds = new productsourcestockDal(SourceCode, SourceRenewIndex);
                    }
                    prostds.AddErrorlog("第" + SourceRenewIndex + "次更结束",SourceCode, SourceRenewIndex, true);//本次更新添加结束时间
                    count = 0;
                    if (!IsRepetitionUpdate)
                    {
                        Thread.Sleep(1000);// Thread.Sleep(SleepTime);暂停一秒
                        Main(args);//args
                    }
                    return;
                }
            }
            #endregion

        }

        #region 获取当前供应商下为第几次更新数据+void GetSourceRenewIndex()
        /// <summary>
        /// 获取当前供应商下为第几次更新数据
        /// </summary>
        private static void GetSourceRenewIndex()
        {
            Console.WriteLine("正在获取当前供应商下第几次更新信息...");
            productsourcestockDal pro = new productsourcestockDal(prosc.SourcesAddress, prosc.DataSources, prosc.UserId, prosc.UserPwd, 0, SourceCode);
            string mess;
            bool IsAddorUpdate;//标记上次更新是否完成   true为完成  false未完成
            SourceRenewIndex = pro.GetSourceRenewIndex(SourceCode, out mess, out IsAddorUpdate);
            if (SourceRenewIndex == -1)
            {
                Console.WriteLine("获取当前供应商下第几次更新数据源 错误：" + mess);
                Console.Clear();
                Console.WriteLine("正在尝试重新请求...");
            }
            else
            {
                //根据IsAddorUpdate判断上次更新是否已经完成，如果未完成，将错误日志里面的开始更新时间改为当前系统时间
                pro.AddErrorlog("第" + SourceRenewIndex + "次更新开始",SourceCode, SourceRenewIndex, IsAddorUpdate);
            }
            Console.WriteLine("获取成功！");
        }
        #endregion

        /// <summary>
        /// 已经开启的线程数
        /// </summary>
        static int threadCount = 0;
        /// <summary>
        /// 远程服务器数据的数量
        /// </summary>
        static int k = 0;
        /// <summary>
        /// 是否进行删除操作
        /// </summary>
        static bool IsDelete = false;
        /// <summary>
        /// 记录已经更新完的总数
        /// </summary>
        static int count = 0;
        /// <summary>
        /// 更新本地数据库
        /// </summary>
        private static void Run()
        {
            //5.1 从远程服务器获取数据，并且计算更新次数
            productsourcestockDal prostd;
            if (prosc != null)
            {
                prostd = new productsourcestockDal(prosc.SourcesAddress, prosc.DataSources, prosc.UserId, prosc.UserPwd, SourceRenewIndex, SourceCode);
            }
            else
            {
                prostd = new productsourcestockDal(SourceCode, SourceRenewIndex);
            }
            Console.WriteLine("开始时间：" + DateTime.Now);
            string mess;
            k = prostd.GetServerDBCount(out mess);
            if (k == 0)
            {
                while (true)
                {
                    //如果取不到远程服务器上则递归调用，直到取到数据为止
                    k = prostd.GetServerDBCount(out mess);
                    if (k == 0)
                    {
                        Console.WriteLine(mess);
                        Console.WriteLine("正在重新请求！");
                    }
                    else
                    {
                        break;
                    }
                }
            }
            k = (k % SetRenewalRowCount == 0 ? k / SetRenewalRowCount : k / SetRenewalRowCount + 1);
            //5.2 查询本地数据库中存在，在服务器中不存在的数据，并且删除(删除操作)
            Thread thread1 = new Thread(delegate(object obj)
            {
                IsDelete = true;
                productsourcestockDal prostds = new productsourcestockDal(prosc.SourcesAddress, prosc.DataSources, prosc.UserId, prosc.UserPwd, SourceRenewIndex, SourceCode);
                prostds.GetDeleteClientDataList();
                prostds.DelteClientProductsourcestock();
                IsDelete = false;
            });
            thread1.IsBackground = true;
            thread1.Start();
            int j = 0;
            //5.3 从远程服务器每次获取100条数据，并且更新本地数据库(更新操作)
            while (j < k)
            {
                while (threadCount < SetThreadCount)
                {
                    threadCount++;
                    Thread thread = new Thread(TH);
                    thread.IsBackground = true;
                    MSG msg = new MSG();
                    msg.i = j;
                    thread.Start(msg);
                    j++;
                }
                Thread.Sleep(100);
            }
            //while (true)
            //{
            //    if (threadCount >= SetThreadCount)//setThreadCount同时开启的线程数5
            //    {
            //        continue;
            //    }
            //    if (j >= k)
            //    {
            //        //Console.WriteLine("OVER！");
            //        break;
            //    }
            //    //if(j>=
            //    threadCount++;

            //    Thread thread = new Thread(TH);
            //    thread.IsBackground = true;
            //    MSG msg = new MSG();
            //    msg.i = j;
            //    //msg.isAgainRenew = false;
            //    // msg.scode = scode;
            //    thread.Start(msg);
            //    j++;
            //    Thread.Sleep(100);
            //}
        }
        

        #region 传入线程的方法+void TH(object obj)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj">Msg</param>
        private static void TH(object obj)
        {
            MSG msg = obj as MSG;
            productsourcestockDal proo;
            if (prosc != null)
            {
                proo = new productsourcestockDal(prosc.SourcesAddress, prosc.DataSources, prosc.UserId, prosc.UserPwd, SourceRenewIndex, SourceCode);
            }
            else
            {
                proo = new productsourcestockDal(SourceCode, SourceRenewIndex);
            }
            string message;
            if (!proo.UpdateClientDataTables(msg.i * SetRenewalRowCount, (msg.i + 1) * SetRenewalRowCount, out message))
            {
                Console.WriteLine("错误：" + (count + SetRenewalRowCount) + "条，" + DateTime.Now.ToString() + "第：" + (msg.i + 1) * SetRenewalRowCount + "行添加失败，失败原因：" + message);
            }
            else
            {
                Console.WriteLine("已更新：" + (count += SetRenewalRowCount) + "条，" + DateTime.Now.ToString() + ":第" + (msg.i * SetRenewalRowCount+1) + "-" + (msg.i + 1) * SetRenewalRowCount + "条数据更新成功");
                Console.WriteLine("正在更新数据库中，请勿关闭...");
            }
            threadCount--;
        }
        #endregion

        /// <summary>
        /// 回调次数
        /// </summary>
        static int NewRunIndex = 0;

        /// <summary>
        /// 再次更新
        /// </summary>
        static void NewRun()
        {
            productsourcestockDal pro;
            if (prosc != null)
            {
                pro = new productsourcestockDal(prosc.SourcesAddress, prosc.DataSources, prosc.UserId, prosc.UserPwd, SourceRenewIndex, SourceCode);
            }
            else
            {
                pro = new productsourcestockDal(SourceCode, SourceRenewIndex);
            }
            string mess;
            List<int[]> list = pro.AgainRenew(SourceCode, SourceRenewIndex, out mess);
            if (list == null)
            {
                Console.WriteLine(mess);
                if (NewRunIndex < 3)//回调3次
                {
                    NewRunIndex++;
                    NewRun();
                }
                else
                {
                    Console.WriteLine("获取本次更新失败的数据再次重新更新这些数据失败！");
                }
            }
            else
            {
                int j = 0;
                int k = list.Count;
                while (j < k)
                {
                    if (k<SetThreadCount)
                    {
                        while (threadCount < SetThreadCount)
                        {
                            threadCount++;
                            Thread thread = new Thread(NewTH);
                            thread.IsBackground = true;
                            MSG msg = new MSG();
                            msg.i = j;
                            msg.list = list;
                            thread.Start(msg);
                            j++;
                        }
                    }
                    else
                    {
                        while (threadCount < SetThreadCount)
                        {
                            threadCount++;
                            Thread thread = new Thread(NewTH);
                            thread.IsBackground = true;
                            MSG msg = new MSG();
                            msg.i = j;
                            msg.list = list;
                            thread.Start(msg);
                            j++;
                        }
                    }            
                }
            }
        }

        /// <summary>
        /// 第二次更新
        /// </summary>
        /// <param name="obj"></param>
        static void NewTH(object obj)
        {
            string mess;
            productsourcestockDal pro;
            if (prosc != null)
                pro = new productsourcestockDal(prosc.SourcesAddress, prosc.DataSources, prosc.UserId, prosc.UserPwd, SourceRenewIndex, SourceCode);
            else
                pro = new productsourcestockDal(SourceCode, SourceRenewIndex);
            MSG msg = obj as MSG;
            if (!pro.UpdateClientDataTables(msg.list[msg.i][0], msg.list[msg.i][1], out mess))
            {
                Console.WriteLine("错误：" + (count + SetRenewalRowCount) + "条，" + DateTime.Now.ToString() + ":第" + msg.list[msg.i][0] + "行添加失败！\r\n 错误详情：" + mess);
            }
            else
            {
                Console.WriteLine("已更新：" + (count += SetRenewalRowCount) + "条，" + DateTime.Now.ToString() + ":第" + msg.list[msg.i][0] + "-" + msg.list[msg.i][1] + "条更新数据成功！");
                Console.WriteLine("更新数据库中....请勿关闭！");
            }
            threadCount--;
        }
    }
}
