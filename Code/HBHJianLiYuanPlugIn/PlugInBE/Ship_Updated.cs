using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Business;
using UFIDA.U9.SM.Ship;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
   public class Ship_Updated : UFSoft.UBF.Eventing.IEventSubscriber
    {
       public void Notify(params object[] args)
       {
           if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
               return;
           BusinessEntity.EntityKey key = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;

           if (key == null)
               return;
           Ship ship = key.GetEntity() as Ship;//出货单实体
           if (ship == null)
               return;
           //提交，单价写入批号主档
           using (ISession session = Session.Open())
           {
               foreach (ShipLine line in ship.ShipLines)
               {
                   if (line.LotInfo != null && line.LotInfo.LotMaster != null)
                   {
                       UFIDA.U9.Lot.LotMaster lotMaster = UFIDA.U9.Lot.LotMaster.Finder.FindByID(line.LotInfo.LotMaster.ID);
                       lotMaster.DescFlexSegments.PrivateDescSeg1 = line.FinallyPriceTC.ToString();
                   }
                  
               }
               session.Commit();
           }
       }
    }
}
