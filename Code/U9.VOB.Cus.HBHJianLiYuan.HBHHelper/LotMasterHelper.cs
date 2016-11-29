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
    /// 批号公共类
    /// </summary>
    public class LotMasterHelper
    {
        #region UI字段

        //// 单据最终价
        ///// <summary>
        ///// 单据最终价
        ///// </summary>
        //public const string LotMaster_FinallyPriceUIField = "DescFlexField_PrivateDescSeg3";

        #endregion


        #region 后台字段

        // 税率
        /// <summary>
        /// 税率
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static decimal GetTaxRate(DescFlexSegments descSegments)
        {
            if (descSegments != null)
            {
                return PubClass.GetDecimal(descSegments.PrivateDescSeg1);
            }
            return 0;
        }

        // 税率
        /// <summary>
        /// 税率
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static void SetTaxRate(DescFlexSegments descSegments, decimal value)
        {
            if (descSegments != null)
            {
                descSegments.PrivateDescSeg1 = PubClass.GetStringRemoveZero(value);
            }
        }

        // 最终价
        /// <summary>
        /// 最终价
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static decimal GetFinallyPrice(DescFlexSegments descSegments)
        {
            if (descSegments != null)
            {
                return PubClass.GetDecimal(descSegments.PrivateDescSeg2);
            }
            return 0;
        }

        // 最终价
        /// <summary>
        /// 最终价
        /// </summary>
        /// <param name="descSegments"></param>
        /// <returns></returns>
        public static void SetFinallyPrice(DescFlexSegments descSegments, decimal value)
        {
            if (descSegments != null)
            {
                descSegments.PrivateDescSeg2 = PubClass.GetStringRemoveZero(value);
            }
        }

        #endregion

    }
}
