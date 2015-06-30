using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Business;
using UFIDA.U9.SM.SO;
using UFSoft.UBF.PL;
using UFIDA.U9.PM.Rcv;
using UFIDA.U9.Cust.HBH.Common.CommonLibary;

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
            Receivement rcv = key.GetEntity() as Receivement;
            //修改后的单价不能高于采购订单中的单价
            if (rcv == null || rcv.SrcDocType.Value != 1)
                return;
            foreach (RcvLine line in rcv.RcvLines)
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
                    }
                }
            }

        }
        #endregion
    }
}
