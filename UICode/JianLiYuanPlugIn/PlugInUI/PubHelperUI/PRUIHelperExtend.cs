using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInUI.PubHelperUI
{
    public static class SOUIHelperExtend
    {
      

        /// <summary>
        /// 计算销售订单折扣，并将销售订单单头“是否已生成折扣”赋值为True
        /// </summary>
        /// <param name="so">销售订单单头ID</param>
        public static void CalcDiscount(long so)
        {
            //UFIDA.U9.Cust.GS.FT.SoBP.Proxy.ModifySOPricesProxy bp = new SoBP.Proxy.ModifySOPricesProxy();
            //bp.SO = so;
            //bp.IsAll = true;//整单折扣开关
            //bp.Do();
        }
       
    }
}
