using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFIDA.U9.Base.FlexField.DescFlexField;
using UFIDA.U9.PPR.PurPriceList;
using UFIDA.U9.Cust.HBH.Common.CommonLibary;

namespace U9.VOB.Cus.HBHJianLiYuan.HBHHelper
{
    /// <summary>
    /// 请购行公共
    /// </summary>
    public class PRHeadHelper
    {
        #region UI字段
     
        // 来源请购单ID
        /// <summary>
        /// 来源请购单ID
        /// </summary>
        public const string PRLine_SrcPPListIDUIField = "DescFlexField_PrivateDescSeg1";

        // 来源请购单号
        /// <summary>
        /// 来源请购单号
        /// </summary>
        public const string PRLine_SrcPPListDocNoUIField = "DescFlexField_PrivateDescSeg2";

        #endregion

        #region 后台字段字段

        // 来源请购单ID
        /// <summary>
        /// 来源请购单ID
        /// </summary>
        public const string PRLine_SrcPPListIDBEField = "DescFlexField.PrivateDescSeg1";

        // 来源请购单号
        /// <summary>
        /// 来源请购单号
        /// </summary>
        public const string PRLine_SrcPPListDocNoBEField = "DescFlexField.PrivateDescSeg2";

        #endregion


        #region 后台字段

        // 来源请购单ID
        /// <summary>
        /// 来源请购单ID
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static string GetSrcPPListID(DescFlexSegments descSegments)
        {
            if (descSegments != null)
            {
                return descSegments.PrivateDescSeg1;
            }
            return string.Empty;
        }

        // 来源请购单ID
        /// <summary>
        /// 来源请购单ID
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static void SetSrcPPListID(DescFlexSegments descSegments, string value)
        {
            if (descSegments != null)
            {
                descSegments.PrivateDescSeg1 = value;
            }
        }

        // 来源请购单ID
        /// <summary>
        /// 来源请购单ID
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static string GetSrcPPListID(DescFlexSegmentsData descSegments)
        {
            if (descSegments != null)
            {
                return descSegments.PrivateDescSeg1;
            }
            return string.Empty;
        }

        // 来源请购单ID
        /// <summary>
        /// 来源请购单ID
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static void SetSrcPPListID(DescFlexSegmentsData descSegments, string value)
        {
            if (descSegments != null)
            {
                descSegments.PrivateDescSeg1 = value;
            }
        }


        // 来源请购单号
        /// <summary>
        /// 来源请购单号
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static string GetSrcPPListDocNo(DescFlexSegments descSegments)
        {
            if (descSegments != null)
            {
                return descSegments.PrivateDescSeg2;
            }
            return string.Empty;
        }

        // 来源请购单号
        /// <summary>
        /// 来源请购单号
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static void SetSrcPPListDocNo(DescFlexSegments descSegments, string value)
        {
            if (descSegments != null)
            {
                descSegments.PrivateDescSeg2 = value;
            }
        }

        // 来源请购单号
        /// <summary>
        /// 来源请购单号
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static string GetSrcPPListDocNo(DescFlexSegmentsData descSegments)
        {
            if (descSegments != null)
            {
                return descSegments.PrivateDescSeg2;
            }
            return string.Empty;
        }

        // 来源请购单号
        /// <summary>
        /// 来源请购单号
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static void SetSrcPPListDocNo(DescFlexSegmentsData descSegments, string value)
        {
            if (descSegments != null)
            {
                descSegments.PrivateDescSeg2 = value;
            }
        }



        #endregion

    }
}
