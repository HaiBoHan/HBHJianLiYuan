using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace U9.VOB.Cus.HBHJianLiYuan.HBHHelper
{
    public class SalaryItemHelper
    {

        // 出勤天数 编码 = 015
        /// <summary>
        /// 出勤天数 编码 = 015
        /// </summary>
        public const string SalaryItemCode_CheckInDays = "015";

        // 工时 编码 = 037
        /// <summary>
        /// 工时 编码 = 037
        /// </summary>
        public const string SalaryItemCode_WorkHours = "037";

        // F考勤工时 编码 = F26
        /// <summary>
        /// F考勤工时 编码 = F26
        /// </summary>
        public const string SalaryItemCode_FWorkHours = "F26";

        // 调动前部门 编码 : 取计薪期间内第一天所在部门 = 092
        /// <summary>
        /// 调动前部门 编码 : 取计薪期间内第一天所在部门 = 092
        /// </summary>
        public const string SalaryItemCode_BeforeDept = "092";

        // 调动后部门 编码：取计薪期间内调动后部门 = 093
        /// <summary>
        /// 调动后部门 编码：取计薪期间内调动后部门 = 093
        /// </summary>
        public const string SalaryItemCode_AfterDept = "093";

        // 调动天数 编码：取计薪期间内调动后部门出勤天数 = 094
        /// <summary>
        /// 调动天数 编码：取计薪期间内调动后部门出勤天数 = 094
        /// </summary>
        public const string SalaryItemCode_TransferDays = "094";


        // F调动前部门 编码：取计薪期间内第一天所在部门 = F35
        /// <summary>
        /// F调动前部门 编码：取计薪期间内第一天所在部门 = F35
        /// </summary>
        public const string SalaryItemCode_FBeforeDept = "F35";

        // F调动后部门 编码：取计薪期间内调动后部门 = F36
        /// <summary>
        /// F调动后部门 编码：取计薪期间内调动后部门 = F36
        /// </summary>
        public const string SalaryItemCode_FAfterDept = "F36";

        // F调动天数 编码：取计薪期间内调动后部门出勤天数 = F37
        /// <summary>
        /// F调动天数 编码：取计薪期间内调动后部门出勤天数 = F37
        /// </summary>
        public const string SalaryItemCode_FTransferDays = "F37";

    }
}
