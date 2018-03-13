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
using UFIDA.U9.PM.Enums;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    public class Receivement_AfterDefaultValue : UFSoft.UBF.Eventing.IEventSubscriber
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
            
            // 2018-03-07 wf 奥琦玮收货单 扩展字段 指导单价等值 
            SetAQWRcvPriceInfo(entity);
        }

        private void SetAQWRcvPriceInfo(Receivement entity)
        {
            // 新增，并且AQW单ID不为空
            if (entity.SysState == UFSoft.UBF.PL.Engine.ObjectState.Inserted
                //&& entity.DescFlexField.PrivateDescSeg1.IsNotNullOrWhiteSpace()
                )
            {
                foreach (RcvLine line in entity.RcvLines)
                {
                    if (line.DescFlexSegments.PrivateDescSeg10.IsNotNullOrWhiteSpace()
                        )
                    {
                        /*
    1、入库单价  
    2、指导价=入库单价  公共段3    (私有段的指导价不用了；)
    3、入库金额=入库单价*实到数量    私有段4
                         */
                        line.DescFlexSegments.PubDescSeg3 = line.FinallyPriceTC.GetStringRemoveZero();
                        line.DescFlexSegments.PrivateDescSeg4 = line.TotalMnyTC.GetStringRemoveZero();
                        line.DescFlexSegments.PrivateDescSeg5 = line.DescFlexSegments.PubDescSeg3;

                        DescFlexFieldHelper.SetPreDiscountPrice(line.DescFlexSegments, line.FinallyPriceTC);
                        DescFlexFieldHelper.SetDiscountRate(line.DescFlexSegments, 1);
                        DescFlexFieldHelper.SetDiscountLimit(line.DescFlexSegments, 0);

                    }
                }
            }
        }
    }
}
