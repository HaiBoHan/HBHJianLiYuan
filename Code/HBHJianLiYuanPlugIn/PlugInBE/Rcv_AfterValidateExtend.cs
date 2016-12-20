using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Business;
using UFIDA.U9.SM.SO;
using UFSoft.UBF.PL;
using UFIDA.U9.PM.Rcv;
using HBH.DoNet.DevPlatform.EntityMapping;
using UFIDA.U9.PM.Enums;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    class Rcv_AfterValidateExtend : UFSoft.UBF.Eventing.IEventSubscriber
    {

        #region IEventSubscriber 成员

        public void Notify(params object[] args)
        {
            //判断入口参数的有效性
            if (args == null || args.Length == 0 || !(args[0] is EntityEvent))
            {
                return;
            }
            //将入口参数的第一个参数转化为EntityEvent,并取EntityKey存入key
            BusinessEntity.EntityKey key = ((EntityEvent)args[0]).EntityKey;
            if (key == null)
            {
                return;
            }
            //转成所需实体，同时判断有效性
            Receivement entity = key.GetEntity() as Receivement;
            //修改后的单价不能高于采购订单中的单价
            //if (entity == null || entity.SrcDocType.Value != 1)
            //    return;

            foreach (RcvLine line in entity.RcvLines)
            {
                if (line.SrcPO != null)
                {
                    UFIDA.U9.PM.PO.POLine poLine = UFIDA.U9.PM.PO.POLine.Finder.FindByID(line.SrcPO.SrcDocLine.EntityID);
                    if (poLine != null)
                    {
                        decimal rcvPrePrice = HBHHelper.DescFlexFieldHelper.GetPreDiscountPrice(line.DescFlexSegments);
                        decimal poPrePrice = HBHHelper.DescFlexFieldHelper.GetPreDiscountPrice(poLine.DescFlexSegments);

                        //if (line.FinallyPriceTC > poLine.FinallyPriceTC)
                        if(poPrePrice > 0
                            && rcvPrePrice > poPrePrice
                            )
                        {
                            string msg = string.Format("单[{0}]行[{1}]价格[{2}]不能高于来源订单行价格[{3}]",line.Receivement.DocNo,line.DocLineNo
                                , PubClass.GetStringRemoveZero(rcvPrePrice)
                                , PubClass.GetStringRemoveZero(poPrePrice)
                                );
                            throw new Exception(msg);
                        }

                        // 如果新增，则把订单指导价 赋值给 收货单行；因为指导价可以修改，后面可以对比  订单指导价、和 收货指导价  差额
                        if (entity.SysState == UFSoft.UBF.PL.Engine.ObjectState.Inserted)
                        {
                            HBHHelper.DescFlexFieldHelper.SetRcvLinePoPreDiscountPrice(line.DescFlexSegments, poPrePrice);
                        }
                    }
                }


                // 手工创建、则赋值 手工录入的 指导价，保证价差为 0
                if (entity.SrcDocType == RcvSrcDocTypeEnum.CreateManual
                    || entity.SrcDocType == RcvSrcDocTypeEnum.Empty
                    )
                {
                    if (entity.Status == RcvStatusEnum.Opened
                        || entity.Status == RcvStatusEnum.Empty
                        || entity.Status == RcvStatusEnum.Approving
                        )
                    {
                        decimal rcvPrePrice = HBHHelper.DescFlexFieldHelper.GetPreDiscountPrice(line.DescFlexSegments);
                        HBHHelper.DescFlexFieldHelper.SetRcvLinePoPreDiscountPrice(line.DescFlexSegments, rcvPrePrice);
                    }
                }
            }

        }
        #endregion
    }
}
