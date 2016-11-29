using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFIDA.U9.Base.FlexField.DescFlexField;
using UFIDA.U9.PPR.PurPriceList;
using HBH.DoNet.DevPlatform.EntityMapping;

namespace U9.VOB.Cus.HBHJianLiYuan.HBHHelper
{
    /// <summary>
    /// 请购行公共
    /// </summary>
    public class PRLineHelper
    {
        #region UI字段

        // 来源价表编码
        /// <summary>
        /// 来源价表编码
        /// </summary>
        public const string PRLine_SrcPPListCodeUIField = "DescFlexField_PrivateDescSeg1";

        // 来源价表行号
        /// <summary>
        /// 来源价表行号
        /// </summary>
        public const string PRLine_SrcPPLineNoUIField = "DescFlexField_PrivateDescSeg2";

        #endregion


        #region 后台字段

        // 来源价表编码
        /// <summary>
        /// 来源价表编码
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static string GetSrcPPListCode(DescFlexSegments descSegments)
        {
            if (descSegments != null)
            {
                return descSegments.PrivateDescSeg1;
            }
            return string.Empty;
        }

        // 来源价表编码
        /// <summary>
        /// 来源价表编码
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static void SetSrcPPListCode(DescFlexSegments descSegments, string value)
        {
            if (descSegments != null)
            {
                descSegments.PrivateDescSeg1 = value;
            }
        }

        // 来源价表行号
        /// <summary>
        /// 来源价表行号
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static string GetSrcPPLineNo(DescFlexSegments descSegments)
        {
            if (descSegments != null)
            {
                return descSegments.PrivateDescSeg2;
            }
            return string.Empty;
        }

        // 来源价表行号
        /// <summary>
        /// 来源价表行号
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static void SetSrcPPLineNo(DescFlexSegments descSegments, string value)
        {
            if (descSegments != null)
            {
                descSegments.PrivateDescSeg2 = value;
            }
        }



        #endregion

    }
}
