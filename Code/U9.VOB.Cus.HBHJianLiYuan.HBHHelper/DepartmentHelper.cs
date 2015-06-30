using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFIDA.U9.Base.FlexField.DescFlexField;
using UFIDA.U9.PPR.PurPriceList;

namespace U9.VOB.Cus.HBHJianLiYuan.HBHHelper
{
    /// <summary>
    /// 部门档案公共
    /// </summary>
    public class DepartmentHelper
    {
        #region UI字段

        // 需求组织
        /// <summary>
        /// 需求组织
        /// </summary>
        public const string Department_RequireOrgCodeUIField = "DescFlexField_PrivateDescSeg1";

        #endregion


        #region 后台字段

        // 获得需求组织
        /// <summary>
        /// 获得需求组织
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static string GetRequireOrgCode(DescFlexSegments descSegments)
        {
            if (descSegments != null)
            {
                return descSegments.PrivateDescSeg1;
            }
            return string.Empty;
        }


        #endregion
    }
}
