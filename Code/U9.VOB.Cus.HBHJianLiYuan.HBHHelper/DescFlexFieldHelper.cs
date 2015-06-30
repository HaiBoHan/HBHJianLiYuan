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
    /// 扩展字段公共
    /// </summary>
    public class DescFlexFieldHelper
    {
        #region UI字段

        // 折前价
        /// <summary>
        /// 折前价
        /// </summary>
        public const string DescFlexField_PreDiscountPriceUIField = "DescFlexField_PubDescSeg3";
        // 折扣率
        /// <summary>
        /// 折扣率
        /// </summary>
        public const string DescFlexField_DiscountRateUIField = "DescFlexField_PubDescSeg4";
        // 折扣额
        /// <summary>
        /// 折扣额
        /// </summary>
        public const string DescFlexField_DiscountLimitUIField = "DescFlexField_PubDescSeg5";

        #endregion


        #region 后台字段

        // 折前价
        /// <summary>
        /// 折前价
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static decimal GetPreDiscountPrice(DescFlexSegments descSegments)
        {
            if (descSegments != null)
            {
                return PubClass.GetDecimal(descSegments.PubDescSeg3);
            }
            return 0;
        }

        // 折前价
        /// <summary>
        /// 折前价
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static void SetPreDiscountPrice(DescFlexSegments descSegments, decimal price)
        {
            if (descSegments != null)
            {
                descSegments.PubDescSeg3 = PubClass.GetStringRemoveZero(price);
            }
        }

        // 折前价
        /// <summary>
        /// 折前价
        /// </summary>
        /// <param name="targetSegments">目标扩展字段</param>
        /// <param name="srcSegments">来源扩展字段</param>
        /// <returns></returns>
        public static void SetPreDiscountPrice(DescFlexSegments targetSegments,DescFlexSegments srcSegments)
        {
            SetPreDiscountPrice(targetSegments, GetPreDiscountPrice(srcSegments));
        }


        // 折扣率
        /// <summary>
        /// 折扣率
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static decimal GetDiscountRate(DescFlexSegments descSegments)
        {
            if (descSegments != null)
            {
                return PubClass.GetDecimal(descSegments.PubDescSeg4);
            }
            return 0;
        }

        // 折扣率
        /// <summary>
        /// 折扣率
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static void SetDiscountRate(DescFlexSegments descSegments, decimal rate)
        {
            if (descSegments != null)
            {
                descSegments.PubDescSeg4 = PubClass.GetStringRemoveZero(rate);
            }
        }

        // 折扣率
        /// <summary>
        /// 折扣率
        /// </summary>
        /// <param name="targetSegments">目标扩展字段</param>
        /// <param name="srcSegments">来源扩展字段</param>
        /// <returns></returns>
        public static void SetDiscountRate(DescFlexSegments targetSegments, DescFlexSegments srcSegments)
        {
            SetDiscountRate(targetSegments, GetDiscountRate(srcSegments));
        }

        // 折扣额
        /// <summary>
        /// 折扣额
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static decimal GetDiscountLimit(DescFlexSegments descSegments)
        {
            if (descSegments != null)
            {
                return PubClass.GetDecimal(descSegments.PubDescSeg5);
            }
            return 0;
        }

        // 折扣额
        /// <summary>
        /// 折扣额
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static void SetDiscountLimit(DescFlexSegments descSegments, decimal disLimit)
        {
            if (descSegments != null)
            {
                descSegments.PubDescSeg5 = PubClass.GetStringRemoveZero(disLimit);
            }
        }

        // 折扣额
        /// <summary>
        /// 折扣额
        /// </summary>
        /// <param name="targetSegments">目标扩展字段</param>
        /// <param name="srcSegments">来源扩展字段</param>
        /// <returns></returns>
        public static void SetDiscountLimit(DescFlexSegments targetSegments, DescFlexSegments srcSegments)
        {
            SetDiscountLimit(targetSegments, GetDiscountLimit(srcSegments));
        }


        // 单价差额
        /// <summary>
        /// 单价差额
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static decimal GetPriceDif(DescFlexSegments descSegments)
        {
            if (descSegments != null)
            {
                return PubClass.GetDecimal(descSegments.PubDescSeg6);
            }
            return 0;
        }

        // 单价差额
        /// <summary>
        /// 单价差额
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static void SetPriceDif(DescFlexSegments descSegments, decimal price)
        {
            if (descSegments != null)
            {
                descSegments.PubDescSeg6 = PubClass.GetStringRemoveZero(price);
            }
        }

        #endregion

    }
}
