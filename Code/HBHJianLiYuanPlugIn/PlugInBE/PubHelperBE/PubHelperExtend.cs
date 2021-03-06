﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.PL;
using UFIDA.U9.Base;
using UFSoft.UBF.Business;
using UFIDA.U9.Base.Doc;
using UFIDA.U9.CBO.DTOs;
using UFIDA.U9.Base.UOM;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    internal static class PubHelperExtend
    {
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
