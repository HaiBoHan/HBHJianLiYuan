﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Business;
using UFIDA.U9.SM.SO;
using UFSoft.UBF.PL;
using UFIDA.U9.PM.Rcv;
using HBH.DoNet.DevPlatform.EntityMapping;
using UFIDA.U9.PPR.PurPriceList;
using U9.VOB.Cus.HBHJianLiYuan.PPLDepartmentBE;
using UFIDA.U9.PPR.PurPriceAdjustment;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    class PurPriceList_AfterValidateExtend : UFSoft.UBF.Eventing.IEventSubscriber
    {

        //#region IEventSubscriber 成员

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
            PurPriceList entity = key.GetEntity() as PurPriceList;

            if (entity == null)
                return;

            // 提交，如果价表没有对应部门，则报错
            if (entity.Status == UFIDA.U9.PPR.Enums.Status.Approving)
            {
                string opath = string.Format("PurchasePriceList=@PurPriceList");
                PPLDepartment dept = PPLDepartment.Finder.Find(opath,new OqlParam(entity.ID));

                if (dept == null)
                {
                    string msg = string.Format("提交失败,价表[{0}]必须设置归属部门。",entity.Code);
                    throw new BusinessException(msg);
                }
            }


            // 调价单，调价新增价表行.扩展字段 = 调价单行扩展字段；
            foreach (PurPriceLine line in entity.PurPriceLines)
            {
                if (line != null
                    && line.SysState == UFSoft.UBF.PL.Engine.ObjectState.Inserted
                    // && line.SrcDoc != null
                    && line.SrcType == PurPLSrcTypeEnum.Adjustment
                    )
                {
                    long srcLineID = line.SrcRowID;

                    PurPriceAdjustLine srcLine = PurPriceAdjustLine.Finder.FindByID(srcLineID);

                    if (srcLine != null)
                    {
                        string priSeg = "PrivateDescSeg";
                        string pubSeg = "PubDescSeg";

                        for (int i = 1; i <= 50; i++)
                        {
                            string propName = pubSeg + i.ToString();
                            string strDesc = PubClass.GetString(line.DescFlexField.GetValue(propName));
                            if (PubClass.IsNull(strDesc))
                            {
                                line.DescFlexField.SetValue(propName, srcLine.DescFlexField.GetValue(propName));
                            }
                        }

                        for (int i = 1; i <= 30; i++)
                        {
                            string propName = priSeg + i.ToString();
                            string strDesc = PubClass.GetString(line.DescFlexField.GetValue(propName));
                            if (PubClass.IsNull(strDesc))
                            {
                                line.DescFlexField.SetValue(propName, srcLine.DescFlexField.GetValue(propName));
                            }
                        }

                    }
                }
            }
        }
        //#endregion
    }
}
