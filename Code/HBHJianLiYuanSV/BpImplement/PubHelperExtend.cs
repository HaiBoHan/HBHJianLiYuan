using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFIDA.U9.CBO.DTOs;
using UFIDA.U9.Base.UOM;

namespace U9.VOB.Cus.HBHJianLiYuan
{
    internal static class PubHelperExtend
    {
        // 领料单（出货单）单据类型
        /// <summary>
        /// 领料单（出货单）单据类型
        /// </summary>
        public const string Const_ShipDocTypeCode = "SM1";
        // 领料单客户编码
        /// <summary>
        /// 领料单客户编码
        /// </summary>
        public const string Const_ShipCustomerCode = "99999";

        public static UOMInfoDTOData GetUomData(this UOM uom)
        {
            if (uom != null)
            {
                long uomID = uom.ID;
                long baseUomID = uom.BaseUOMKey != null ? uom.BaseUOMKey.ID : uom.ID;
                decimal rate = uom.BaseUOMKey != null ? uom.RatioToBase : 1;

                return new UOMInfoDTOData(uomID, baseUomID, rate);
            }
            return null;
        }

    }
}
