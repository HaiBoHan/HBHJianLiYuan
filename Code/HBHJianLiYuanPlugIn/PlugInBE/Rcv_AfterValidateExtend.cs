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
using U9.VOB.Cus.HBHJianLiYuan.HBHHelper;
using UFIDA.U9.CBO.SCM.Enums;
using HBH.DoNet.DevPlatform.U9Mapping;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    class Rcv_AfterValidateExtend : UFSoft.UBF.Eventing.IEventSubscriber
    {
        // 是否控制计划价物料不可无来源出货
        private const string Const_IsControlPlanPrice = "HBHJianLiYuan_IsControlPlanPrice";

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

            // 2018-03-07 wf
            SetAQWRcvPriceInfo(entity);

            // 收货才赋值，退货等不赋值
            if (entity.RcvDocType != null
                && entity.RcvDocType.ReceivementType != null
                && entity.RcvDocType.ReceivementType == ReceivementTypeEnum.RCV
                )
            {
                StringBuilder sbError = new StringBuilder();
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
                            if (poPrePrice > 0
                                && rcvPrePrice > poPrePrice
                                )
                            {
                                string msg = string.Format("收货单[{0}]行[{1}]价格[{2}]不能高于来源订单行价格[{3}]", line.Receivement.DocNo, line.DocLineNo
                                    , PubClass.GetStringRemoveZero(rcvPrePrice)
                                    , PubClass.GetStringRemoveZero(poPrePrice)
                                    );
                                throw new Exception(msg);
                            }

                            // 如果新增，则把订单指导价 赋值给 收货单行；因为指导价可以修改，后面可以对比  订单指导价、和 收货指导价  差额
                            //if (entity.SysState == UFSoft.UBF.PL.Engine.ObjectState.Inserted)
                            if (line.SysState == UFSoft.UBF.PL.Engine.ObjectState.Inserted)
                            {
                                HBHHelper.DescFlexFieldHelper.SetRcvLinePoPreDiscountPrice(line.DescFlexSegments, poPrePrice);
                            }

                            //decimal poFinallyPrice = poLine.FinallyPriceTC;
                            //decimal rcvFinallyPrice = line.FinallyPriceTC;
                            // 2017-09-05 wf  by  李震林 哪个？要校验的吗？是指导价，就是折前价
                            // 米面油入库时价格不允许修改
                            // 2017-09-16 wf 李震林，有组织不启用计划价控制     HBHJianLiYuan_IsControlPlanPrice
                            if (entity.Status == RcvStatusEnum.Opened
                                || entity.Status == RcvStatusEnum.Empty
                                || entity.Status == RcvStatusEnum.Approving
                                )
                            {
                                if (IsPlanPrice(line))
                                {
                                    if (rcvPrePrice != poPrePrice)
                                    {
                                        sbError.Append(string.Format("收货单行[{0}]料品[{1}]指导价[{2}]与订单指导价[{3}]不一致,不允许修改!"
                                            , line.DocLineNo
                                            , line.ItemInfo.ItemName
                                            , rcvPrePrice.GetStringRemoveZero()
                                            , poPrePrice.GetStringRemoveZero()
                                            ));
                                    }
                                }
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

                            if (IsPlanPrice(line))
                            {
                                sbError.Append(string.Format("收货单行[{0}]料品[{1}]不允许手工创建，必须来源订单才可收货!"
                                    , line.DocLineNo
                                    , line.ItemInfo.ItemName
                                    ));
                            }
                        }
                    }
                }

                // 提交时， 检查  折前价、折扣率、折扣额 与  折后价 如果不匹配，报错
                if (entity.OriginalData != null
                    && entity.OriginalData.Status == RcvStatusEnum.Opened
                    && entity.Status == RcvStatusEnum.Approving
                    )
                {
                    foreach (RcvLine line in entity.RcvLines)
                    {
                        decimal preDiscount = DescFlexFieldHelper.GetPreDiscountPrice(line.DescFlexSegments);
                        decimal discountRate = DescFlexFieldHelper.GetDiscountRate(line.DescFlexSegments);
                        decimal discountLimit = DescFlexFieldHelper.GetDiscountLimit(line.DescFlexSegments);
                        // 计算的折后价
                        decimal discountedPrice = PPLineHelper.GetFinallyPrice(preDiscount, discountRate, discountLimit);
                        // 单据上的最终价
                        decimal finallyPrice = line.FinallyPriceTC;

                        // 考虑尾差，判断差额大于0.05 吧
                        //if (discountedPrice != finallyPrice)
                        if (Math.Abs(discountedPrice - finallyPrice) > 0.05M)
                        {
                            sbError.Append(string.Format("收货单[{0}]行[{1}]最终价[{2}]与折扣计算的最终价[{3}]不一致! \r\n "
                                , entity.DocNo
                                , line.DocLineNo
                                , finallyPrice.GetStringRemoveZero()
                                , discountedPrice.GetStringRemoveZero()
                                ));
                        }
                    }
                }

                if (sbError.Length > 0)
                {
                    throw new BusinessException(sbError.ToString());
                }
            }
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

        private bool IsPlanPrice(RcvLine line)
        {
            bool isControl = U9Helper.GetSysParam(Const_IsControlPlanPrice).GetBool();

            if (isControl)
            {
                if (line != null
                    && line.ItemInfo != null
                    && line.ItemInfo.ItemID != null
                    && line.ItemInfo.ItemID.MainItemCategory != null
                    && ListPlanPriceCategoryCode.Contains(line.ItemInfo.ItemID.MainItemCategory.Code)
                    )
                {
                    return true;
                }
            }
            return false;
        }

        // 计划价的米面油的 物料分类编码
        private List<string> lstPlanPriceCategoryCode = null;
        /// <summary>
        /// 计划价的米面油的 物料分类编码
        /// </summary>
        public List<string> ListPlanPriceCategoryCode
        {
            get
            {
                if (lstPlanPriceCategoryCode == null)
                {
                    lstPlanPriceCategoryCode = new List<string>();

                    lstPlanPriceCategoryCode.Add("0101");
                    lstPlanPriceCategoryCode.Add("0102");
                    lstPlanPriceCategoryCode.Add("0103");
                }

                return lstPlanPriceCategoryCode;
            }
            //set { lstPlanPriceCategoryCode = value; }
        }



        #endregion
    }
}
