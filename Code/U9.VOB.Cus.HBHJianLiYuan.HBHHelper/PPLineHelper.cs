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
    /// 价表行公共
    /// </summary>
    public class PPLineHelper
    {
        #region UI字段

        //// 折后价
        ///// <summary>
        ///// 折后价
        ///// </summary>
        //public const string SOLine_FinallyPriceUIField = "Price";

        #endregion


        #region 后台字段

        // 折后价
        /// <summary>
        /// 折后价
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static decimal GetFinallyPrice(PurPriceLine line)
        {
            if (line != null)
            {
                //return line.Price;
                decimal disRate = HBHHelper.DescFlexFieldHelper.GetDiscountRate(line.DescFlexField);
                decimal disLimit = HBHHelper.DescFlexFieldHelper.GetDiscountLimit(line.DescFlexField);
                decimal preDisPrice = GetPreDiscountPrice(line);

                // 如果折扣率非空，那么取折扣率，折前价格 * 折扣率 = 折后价；
                // 如果折扣率为空，折前价格 - 折扣额 = 折后价
                return GetFinallyPrice(preDisPrice ,disRate, disLimit);
            }
            return 0;
        }

        //// 折后价
        ///// <summary>
        ///// 折后价
        ///// </summary>
        ///// <param name="descSegments"></param>
        ///// <returns></returns>
        //public static void SetFinallyPrice(PurPriceLine line, decimal price)
        //{
        //    if (line != null)
        //    {
        //        line.Price = price;
        //    }
        //}


        // 折前价
        /// <summary>
        /// 折前价
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static decimal GetPreDiscountPrice(PurPriceLine line)
        {
            if (line != null)
            {
                return line.Price;
            }
            return 0;
        }


        // 根据 折前价、折扣率、折扣额，获得最终价
        /// <summary>
        /// 根据 折前价、折扣率、折扣额，获得最终价
        /// </summary>
        /// <param name="preDisPrice">折前价</param>
        /// <param name="disRate">折扣率</param>
        /// <param name="disLimit">折扣额</param>
        /// <returns>最终价</returns>
        public static decimal GetFinallyPrice(decimal preDisPrice, decimal disRate, decimal disLimit)
        {
            if (disRate != 0)
            {
                return preDisPrice * disRate;
            }
            else
            {
                return preDisPrice - disLimit;
            }
        }

        #endregion

    }
}
