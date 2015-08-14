﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Business;
using UFIDA.U9.SM.Ship;
using U9.VOB.Cus.HBHJianLiYuan.HBHHelper;
using UFIDA.U9.PM.Rcv;
using UFIDA.U9.PM.PO;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    public class Rcv_AfterUpdated : UFSoft.UBF.Eventing.IEventSubscriber
    {
       public void Notify(params object[] args)
       {
           if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
               return;
           BusinessEntity.EntityKey key = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;

           if (key == null)
               return;
           Receivement entity = key.GetEntity() as Receivement;//出货单实体
           if (entity == null)
               return;
           bool isUpdateLot = false;
           if (entity.OriginalData.Status != RcvStatusEnum.Closed
               && entity.Status == RcvStatusEnum.Closed
               )
           {
               isUpdateLot = true;
           }
           //提交，单价写入批号主档
           using (ISession session = Session.Open())
           {
               bool isUpdated = false;
               foreach (RcvLine line in entity.RcvLines)
               {
                   if (isUpdateLot)
                   {
                       if (line.RcvLot != null)
                       {
                           UFIDA.U9.Lot.LotMaster lotMaster = line.RcvLot;
                           UpdateLotPrice(lotMaster, line);

                           isUpdated = true;
                       }

                       // ShangLuo用的是InvLot，估计这里也是
                       if (line.InvLot != null && line.InvLot != null)
                       {
                           // UFIDA.U9.Lot.LotMaster lotMaster = UFIDA.U9.Lot.LotMaster.Finder.FindByID(line.LotInfo.LotMaster.ID);
                           // lotMaster.DescFlexSegments.PrivateDescSeg1 = line.FinallyPriceTC.ToString();
                           UFIDA.U9.Lot.LotMaster lotMaster = line.InvLot;
                           UpdateLotPrice(lotMaster, line);

                           isUpdated = true;
                       }
                   }
               }

               // 更新来源订单为关闭状态
               // UFIDA.U9.PM.PurchaseOrderUIModel.PurchaseOrderMainUIFormWebPart       MenuClose
               // MenuClose_Click_Extend
               List<long> lstPO = new List<long>();
               foreach (RcvLine line in entity.RcvLines)
               {
                   if (line != null
                       && line.SrcDoc != null
                       && line.SrcDoc.SrcDoc != null
                       && line.SrcDoc.SrcDoc.EntityID > 0
                       )
                   { 
                       long srcPOID = line.SrcDoc.SrcDoc.EntityID;

                       if (!lstPO.Contains(srcPOID))
                       {
                           lstPO.Add(srcPOID);
                       }
                   }
               }

               if (lstPO.Count > 0)
               {
                   foreach (long srcPOID in lstPO)
                   {
                       PurchaseOrder po = PurchaseOrder.Finder.FindByID(srcPOID);

                       // 已审核，则关闭
                       if (po != null
                           && po.Status == PODOCStatusEnum.Approved
                           )
                       {
                           po.Status = PODOCStatusEnum.Closed;
                           
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

       private static void UpdateLotPrice(UFIDA.U9.Lot.LotMaster lotMaster,RcvLine line)
       {
           LotMasterHelper.SetTaxRate(lotMaster.DescFlexSegments, line.TaxRate);
           LotMasterHelper.SetFinallyPrice(lotMaster.DescFlexSegments, line.FinallyPriceTC);
           DescFlexFieldHelper.SetPreDiscountPrice(lotMaster.DescFlexSegments, line.DescFlexSegments);
           DescFlexFieldHelper.SetDiscountRate(lotMaster.DescFlexSegments, line.DescFlexSegments);
           DescFlexFieldHelper.SetDiscountLimit(lotMaster.DescFlexSegments, line.DescFlexSegments);
       }
    }
}
