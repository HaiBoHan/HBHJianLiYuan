using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFIDA.U9.Base.FlexField.DescFlexField;
using UFIDA.U9.Cust.HBH.Common.CommonLibary;

namespace U9.VOB.Cus.HBHJianLiYuan.HBHHelper
{
    public class RcvLineHelper
    {
        #region UI字段

        // 折前价
        /// <summary>
        /// 折前价
        /// </summary>
        public const string DescFlexSegments_PreDiscountPriceUIField = "DescFlexSegments_PubDescSeg3";
        // 折扣率
        /// <summary>
        /// 折扣率
        /// </summary>
        public const string DescFlexSegments_DiscountRateUIField = "DescFlexSegments_PubDescSeg4";
        // 折扣额
        /// <summary>
        /// 折扣额
        /// </summary>
        public const string DescFlexSegments_DiscountLimitUIField = "DescFlexSegments_PubDescSeg5";
        // 单价差额
        /// <summary>
        /// 单价差额
        /// </summary>
        public const string DescFlexSegments_PriceDifUIField = "DescFlexSegments_PubDescSeg6";

        #endregion
    }
}
