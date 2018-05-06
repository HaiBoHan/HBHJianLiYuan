using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFIDA.U9.PAY.SalaryItem;
using UFSoft.UBF.PL;

namespace U9.VOB.Cus.HBHJianLiYuan.HBHHelper
{
    // 薪资项目帮助
    /// <summary>
    /// 薪资项目帮助
    /// </summary>
    public class SalaryItemHelper
    {

        // 出勤天数 编码 = 015
        /// <summary>
        /// 出勤天数 编码 = 015
        /// </summary>
        public static string SalaryItemCode_CheckInDays = "015";

        // 出勤天数 编码 = 015
        /// <summary>
        /// 出勤天数 编码 = 015
        /// </summary>
        public static SalaryItem SalaryItem_CheckInDays
        {
            get
            {
                SalaryItem item = SalaryItem.Finder.Find("Code=@Code"
                    , new OqlParam(SalaryItemHelper.SalaryItemCode_CheckInDays)
                    );

                return item;
            }
        }

        // 工时 编码 = 037
        /// <summary>
        /// 工时 编码 = 037
        /// </summary>
        public static string SalaryItemCode_WorkHours = "037";

        // 工时 编码 = 037
        /// <summary>
        /// 工时 编码 = 037
        /// </summary>
        public static SalaryItem SalaryItem_WorkHours
        {
            get
            {
                SalaryItem item = SalaryItem.Finder.Find("Code=@Code"
                    , new OqlParam(SalaryItemHelper.SalaryItemCode_WorkHours)
                    );

                return item;
            }
        }

        // F考勤工时 编码 = F26
        /// <summary>
        /// F考勤工时 编码 = F26
        /// </summary>
        public static string SalaryItemCode_FWorkHours = "F26";

        // F考勤工时 编码 = F26
        /// <summary>
        /// F考勤工时 编码 = F26
        /// </summary>
        public static SalaryItem SalaryItem_FWorkHours
        {
            get
            {
                SalaryItem item = SalaryItem.Finder.Find("Code=@Code"
                    , new OqlParam(SalaryItemHelper.SalaryItemCode_FWorkHours)
                    );

                return item;
            }
        }

        // FJ工时 编码 = F57        = 取日考勤中钟点工出勤
        /// <summary>
        /// FJ工时 编码 = F57        = 取日考勤中钟点工出勤
        /// </summary>
        public static string SalaryItemCode_FJWorkHours = "F57";

        // FJ工时 编码 = F57        = 取日考勤中钟点工出勤
        /// <summary>
        /// FJ工时 编码 = F57        = 取日考勤中钟点工出勤
        /// </summary>
        public static SalaryItem SalaryItem_FJWorkHours
        {
            get
            {
                SalaryItem item = SalaryItem.Finder.Find("Code=@Code"
                    , new OqlParam(SalaryItemHelper.SalaryItemCode_FJWorkHours)
                    );

                return item;
            }
        }

        // 调动前部门 编码 : 取计薪期间内第一天所在部门 = 092
        /// <summary>
        /// 调动前部门 编码 : 取计薪期间内第一天所在部门 = 092
        /// </summary>
        public static string SalaryItemCode_BeforeDept = "092";

        // 调动后部门 编码：取计薪期间内调动后部门 = 093
        /// <summary>
        /// 调动后部门 编码：取计薪期间内调动后部门 = 093
        /// </summary>
        public static string SalaryItemCode_AfterDept = "093";

        // 调动天数 编码：取计薪期间内调动后部门出勤天数 = 094
        /// <summary>
        /// 调动天数 编码：取计薪期间内调动后部门出勤天数 = 094
        /// </summary>
        public static string SalaryItemCode_TransferDays = "094";


        // F调动前部门 编码：取计薪期间内第一天所在部门 = F35
        /// <summary>
        /// F调动前部门 编码：取计薪期间内第一天所在部门 = F35
        /// </summary>
        public static string SalaryItemCode_FBeforeDept = "F35";

        // F调动后部门 编码：取计薪期间内调动后部门 = F36
        /// <summary>
        /// F调动后部门 编码：取计薪期间内调动后部门 = F36
        /// </summary>
        public static string SalaryItemCode_FAfterDept = "F36";

        // F调动天数 编码：取计薪期间内调动后部门出勤天数 = F37
        /// <summary>
        /// F调动天数 编码：取计薪期间内调动后部门出勤天数 = F37
        /// </summary>
        public static string SalaryItemCode_FTransferDays = "F37";





        // 第一调动部门(全日制)	128	    ExtField186	取计薪期间内第一天所在部门
        /// <summary>
        /// 第一调动部门(全日制)
        /// </summary>
        public static string SalaryItemCode_FirstDept = "128";

        // 第一调动部门(全日制)
        /// <summary>
        /// 第一调动部门(全日制)
        /// </summary>
        public static SalaryItem SalaryItem_FirstDept
        {
            get
            { 
                SalaryItem item = SalaryItem.Finder.Find("Code=@Code"
                    , new OqlParam(SalaryItemHelper.SalaryItemCode_FirstDept)
                    );

                return item;
            }
        }

        // 第二调动部门(全日制)	129	ExtField187	取计薪期间内最后一个调动单的调动前部门or取计薪期间内“非第一调动部门”和“非第三调动部门”的部门
        /// <summary>
        /// 第二调动部门(全日制)
        /// </summary>
        public static string SalaryItemCode_SecondDept = "129";

        // 第二调动部门(全日制)
        /// <summary>
        /// 第二调动部门(全日制)
        /// </summary>
        public static SalaryItem SalaryItem_SecondDept
        {
            get
            {
                SalaryItem item = SalaryItem.Finder.Find("Code=@Code"
                    , new OqlParam(SalaryItemHelper.SalaryItemCode_SecondDept)
                    );

                return item;
            }
        }

        // 第三调动部门(全日制)	130	ExtField188	取计薪期间内最后一天所在部门
        /// <summary>
        /// 第三调动部门(全日制)
        /// </summary>
        public static string SalaryItemCode_ThirdDept = "130";

        // 第三调动部门(全日制)
        /// <summary>
        /// 第三调动部门(全日制)
        /// </summary>
        public static SalaryItem SalaryItem_ThirdDept
        {
            get
            {
                SalaryItem item = SalaryItem.Finder.Find("Code=@Code"
                    , new OqlParam(SalaryItemHelper.SalaryItemCode_ThirdDept)
                    );

                return item;
            }
        }

        // 第二部门工作天数(全日制)	121	ExtField102	取计薪期间内第一部门的工作天数
        /// <summary>
        /// 第二部门工作天数(全日制)
        /// </summary>
        public static string SalaryItemCode_SecondDeptDays = "121";

        // 第二部门工作天数(全日制)
        /// <summary>
        /// 第二部门工作天数(全日制)
        /// </summary>
        public static SalaryItem SalaryItem_SecondDeptDays
        {
            get
            {
                SalaryItem item = SalaryItem.Finder.Find("Code=@Code"
                    , new OqlParam(SalaryItemHelper.SalaryItemCode_SecondDeptDays)
                    );

                return item;
            }
        }

        // 第三部门工作天数(全日制)	122	ExtField104	取计薪期间内第三部门的工作天数
        /// <summary>
        /// 第三部门工作天数(全日制)
        /// </summary>
        public static string SalaryItemCode_ThirdDeptDays = "122";

        // 第三部门工作天数(全日制)
        /// <summary>
        /// 第三部门工作天数(全日制)
        /// </summary>
        public static SalaryItem SalaryItem_ThirdDeptDays
        {
            get
            {
                SalaryItem item = SalaryItem.Finder.Find("Code=@Code"
                    , new OqlParam(SalaryItemHelper.SalaryItemCode_ThirdDeptDays)
                    );

                return item;
            }
        }

        // 第一调动部门   加班工时(全日制) 编码 = 
        /// <summary>
        /// 第一调动部门   加班工时(全日制) 编码 = 
        /// </summary>
        public static string SalaryItemCode_FirstDeptWorkHours = "134";

        // 第一调动部门   加班工时(全日制)
        /// <summary>
        /// 第一调动部门   加班工时(全日制)
        /// </summary>
        public static SalaryItem SalaryItem_FirstDeptWorkHours
        {
            get
            {
                SalaryItem item = SalaryItem.Finder.Find("Code=@Code"
                    , new OqlParam(SalaryItemHelper.SalaryItemCode_FirstDeptWorkHours)
                    );

                return item;
            }
        }

        // 第二调动部门   加班工时(全日制) 编码 = 
        /// <summary>
        /// 第二调动部门   加班工时(全日制) 编码 = 
        /// </summary>
        public static string SalaryItemCode_SecondDeptWorkHours = "135";

        // 第二调动部门   加班工时(全日制)
        /// <summary>
        /// 第二调动部门   加班工时(全日制)
        /// </summary>
        public static SalaryItem SalaryItem_SecondDeptWorkHours
        {
            get
            {
                SalaryItem item = SalaryItem.Finder.Find("Code=@Code"
                    , new OqlParam(SalaryItemHelper.SalaryItemCode_SecondDeptWorkHours)
                    );

                return item;
            }
        }

        // 第三调动部门   加班工时(全日制) 编码 = 
        /// <summary>
        /// 第三调动部门   加班工时(全日制) 编码 = 
        /// </summary>
        public static string SalaryItemCode_ThirdDeptWorkHours = "136";

        // 第三调动部门   加班工时(全日制)
        /// <summary>
        /// 第三调动部门   加班工时(全日制)
        /// </summary>
        public static SalaryItem SalaryItem_ThirdDeptWorkHours
        {
            get
            {
                SalaryItem item = SalaryItem.Finder.Find("Code=@Code"
                    , new OqlParam(SalaryItemHelper.SalaryItemCode_ThirdDeptWorkHours)
                    );

                return item;
            }
        }


        // F第一调动部门(非全日制)	131	ExtField189	取计薪期间内第一天所在部门
        /// <summary>
        /// F第一调动部门(非全日制)
        /// </summary>
        public static string SalaryItemCode_FFirstDept = "131";

        // F第一调动部门(非全日制)
        /// <summary>
        /// F第一调动部门(非全日制)
        /// </summary>
        public static SalaryItem SalaryItem_FFirstDept
        {
            get
            {
                SalaryItem item = SalaryItem.Finder.Find("Code=@Code"
                    , new OqlParam(SalaryItemHelper.SalaryItemCode_FFirstDept)
                    );

                return item;
            }
        }

        // F第二调动部门(非全日制)	132	ExtField190	取计薪期间内最后一个调动单的调动前部门or取计薪期间内“非F第一调动部门”和“非F第三调动部门”的部门
        /// <summary>
        /// F第二调动部门(非全日制)
        /// </summary>
        public static string SalaryItemCode_FSecondDept = "132";

        // F第二调动部门(非全日制)
        /// <summary>
        /// F第二调动部门(非全日制)
        /// </summary>
        public static SalaryItem SalaryItem_FSecondDept
        {
            get
            {
                SalaryItem item = SalaryItem.Finder.Find("Code=@Code"
                    , new OqlParam(SalaryItemHelper.SalaryItemCode_FSecondDept)
                    );

                return item;
            }
        }

        // F第三调动部门(非全日制)	133	ExtField191	取计薪期间内最后一天所在部门
        /// <summary>
        /// F第三调动部门(非全日制)
        /// </summary>
        public static string SalaryItemCode_FThirdDept = "133";

        // F第三调动部门(非全日制)
        /// <summary>
        /// F第三调动部门(非全日制)
        /// </summary>
        public static SalaryItem SalaryItem_FThirdDept
        {
            get
            {
                SalaryItem item = SalaryItem.Finder.Find("Code=@Code"
                    , new OqlParam(SalaryItemHelper.SalaryItemCode_FThirdDept)
                    );

                return item;
            }
        }

        // F第二部门工作天数(非全日制)	126	ExtField184	取计薪期间内第一部门的工作天数
        /// <summary>
        /// F第二部门工作天数(非全日制)
        /// </summary>
        public static string SalaryItemCode_FSecondDeptDays = "126";

        // F第二部门工作天数(非全日制)
        /// <summary>
        /// F第二部门工作天数(非全日制)
        /// </summary>
        public static SalaryItem SalaryItem_FSecondDeptDays
        {
            get
            {
                SalaryItem item = SalaryItem.Finder.Find("Code=@Code"
                    , new OqlParam(SalaryItemHelper.SalaryItemCode_FSecondDeptDays)
                    );

                return item;
            }
        }

        // F第三部门工作天数(非全日制)	127	ExtField185	取计薪期间内第三部门的工作天数
        /// <summary>
        /// F第三部门工作天数(非全日制)
        /// </summary>
        public static string SalaryItemCode_FThirdDeptDays = "127";

        // F第三部门工作天数(非全日制)
        /// <summary>
        /// F第三部门工作天数(非全日制)
        /// </summary>
        public static SalaryItem SalaryItem_FThirdDeptDays
        {
            get
            {
                SalaryItem item = SalaryItem.Finder.Find("Code=@Code"
                    , new OqlParam(SalaryItemHelper.SalaryItemCode_FThirdDeptDays)
                    );

                return item;
            }
        }





        // 2018-05-06 wf 区域应兑现
        /// <summary>
        /// 2018-05-06 wf 区域应兑现
        /// </summary>
        public static string SalaryItemCode_AreaShouldBeCashed = "155";

        // 2018-05-06 wf 区域应兑现
        /// <summary>
        /// 2018-05-06 wf 区域应兑现
        /// </summary>
        public static SalaryItem SalaryItem_AreaShouldBeCashed
        {
            get
            {
                SalaryItem item = SalaryItem.Finder.Find("Code=@Code"
                    , new OqlParam(SalaryItemHelper.SalaryItemCode_AreaShouldBeCashed)
                    );

                return item;
            }
        }


    }
}
