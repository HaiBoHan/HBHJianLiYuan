using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace U9.VOB.Cus.HBHJianLiYuan.HBHHelper.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // 3-2-2-3-2-2
            string deptCode = "0000321011";


            string strFl = deptCode.Substring(3, 2);







            //DateTime dt = new DateTime(2016,8, 1);
            //DateTime dt = new DateTime(2017,3, 1);
            DateTime dt = new DateTime(2017,6, 1);
            //// 时分秒 毫秒；
            //string str = dt.ToString("yyyy-MM-dd hh:mm:ss.fff");

            long expire = dt.ToFileTime(); ;

            Console.WriteLine(expire);

            Console.ReadKey();
        }
    }
}
