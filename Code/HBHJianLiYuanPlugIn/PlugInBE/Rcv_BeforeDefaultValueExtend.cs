using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Business;
using UFIDA.U9.SM.SO;
using UFSoft.UBF.PL;
using UFIDA.U9.SM.Enums;
using UFIDA.U9.PR.PurchaseRequest;
using U9.VOB.Cus.HBHJianLiYuan.HBHHelper;
using HBH.DoNet.DevPlatform.EntityMapping;
using UFIDA.U9.CBO.SCM.Supplier;
using UFIDA.U9.Base;
using UFIDA.U9.PM.Rcv;
using UFIDA.U9.CBO.SCM.Enums;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    public class Rcv_BeforeDefaultValueExtend : UFSoft.UBF.Eventing.IEventSubscriber
    {
        public void Notify(params object[] args)
        {
            if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
                return;
            BusinessEntity.EntityKey key = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;

            if (key == null)
                return;
            Receivement entity = key.GetEntity() as Receivement;//请购单实体
            if (entity == null)
                return;

            // 收货才赋值，退货等不赋值
            if (entity.RcvDocType != null
                && entity.RcvDocType.ReceivementType != null
                && entity.RcvDocType.ReceivementType == ReceivementTypeEnum.RCV
                )
            {
                // OBA导入，赋值最终价
                // 最终价为0，则赋值
                if (entity.SysState == UFSoft.UBF.PL.Engine.ObjectState.Inserted)
                {
                    foreach (RcvLine line in entity.RcvLines)
                    {
                        if (line.FinallyPriceTC == 0)
                        {
                            decimal preDiscount = DescFlexFieldHelper.GetPreDiscountPrice(line.DescFlexSegments);
                            decimal discountRate = DescFlexFieldHelper.GetDiscountRate(line.DescFlexSegments);
                            decimal discountLimit = DescFlexFieldHelper.GetDiscountLimit(line.DescFlexSegments);
                            // 计算的折后价
                            decimal discountedPrice = PPLineHelper.GetFinallyPrice(preDiscount, discountRate, discountLimit);

                            if (line.FinallyPriceTC != discountedPrice)
                            {
                                line.FinallyPriceTC = discountedPrice;
                            }
                        }
                    }
                }
                

                // 提交时机赋值吧，要不每次都调用，浪费；
                bool isSubmit = false;
                if (entity.OriginalData.Status == RcvStatusEnum.Opened
                    && entity.Status == RcvStatusEnum.Approving
                    )
                {
                    isSubmit = true;
                }
                if (isSubmit)
                {
                    foreach (RcvLine line in entity.RcvLines)
                    {
                        // 赋值差额
                        decimal dif = HBHHelper.DescFlexFieldHelper.GetPreDiscountPrice(line.DescFlexSegments) - line.FinallyPriceTC;
                        if (dif != HBHHelper.DescFlexFieldHelper.GetPriceDif(line.DescFlexSegments))
                        {
                            HBHHelper.DescFlexFieldHelper.SetPriceDif(line.DescFlexSegments, dif);
                        }
                    }
                }

            }
        }
    }
}
