using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Business;
using UFIDA.U9.SM.Ship;
using U9.VOB.Cus.HBHJianLiYuan.HBHHelper;
using UFIDA.U9.PM.Rcv;
using UFIDA.U9.InvDoc.WhInit;
using UFIDA.U9.InvDoc.Enums;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    public class WhInit_AfterUpdated : UFSoft.UBF.Eventing.IEventSubscriber
    {
       public void Notify(params object[] args)
       {
           if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
               return;
           BusinessEntity.EntityKey key = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;

           if (key == null)
               return;
           WhInit entity = key.GetEntity() as WhInit;//出货单实体
           if (entity == null)
               return;
           bool isUpdateLot = false;
           if (entity.OriginalData.Status != INVDocStatus.Approved
               && entity.Status == INVDocStatus.Approved
               )
           {
               isUpdateLot = true;
           }
           //提交，单价写入批号主档
           using (ISession session = Session.Open())
           {
               bool isUpdated = false;
               foreach (WhInitLine line in entity.WhInitLines)
               {
                   if (isUpdateLot)
                   {
                       if (line.LotMaster != null
                           && line.LotMaster.LotMaster != null
                           )
                       {
                           UFIDA.U9.Lot.LotMaster lotMaster = line.LotMaster.LotMaster;
                           UpdateLotPrice(lotMaster, line);

                           isUpdated = true;
                       }
                   }
               }

               // 甚至都不需要开Session,后面可以试试
               // Updated ,应该需要开Session
               if (isUpdated)
               {
                   session.Commit();
               }
           }
       }

       private static void UpdateLotPrice(UFIDA.U9.Lot.LotMaster lotMaster, WhInitLine line)
       {
           decimal price = GetPrice(line);

           LotMasterHelper.SetTaxRate(lotMaster.DescFlexSegments, 0);
           LotMasterHelper.SetFinallyPrice(lotMaster.DescFlexSegments, price);
           DescFlexFieldHelper.SetPreDiscountPrice(lotMaster.DescFlexSegments, line.DescFlexSegments);
           //DescFlexFieldHelper.SetDiscountRate(lotMaster.DescFlexSegments, line.DescFlexSegments);
           //DescFlexFieldHelper.SetDiscountLimit(lotMaster.DescFlexSegments, line.DescFlexSegments);
       }

       private static decimal GetPrice(WhInitLine line)
       {
           if (line != null
               && line.WhInitLineCosts != null
               && line.WhInitLineCosts.Count > 0
               )
           {
               return line.WhInitLineCosts[0].CostPrice;
           }
           return 0;
       }
    }
}
