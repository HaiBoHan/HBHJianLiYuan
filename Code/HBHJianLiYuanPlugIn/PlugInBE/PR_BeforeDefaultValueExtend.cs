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
using UFIDA.U9.Cust.HBH.Common.CommonLibary;
using UFIDA.U9.CBO.SCM.Supplier;
using UFIDA.U9.Base;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    public class PR_BeforeDefaultValueExtend : UFSoft.UBF.Eventing.IEventSubscriber
    {
        public void Notify(params object[] args)
        {
            if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
                return;
            BusinessEntity.EntityKey key = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;

            if (key == null)
                return;
            PR pr = key.GetEntity() as UFIDA.U9.PR.PurchaseRequest.PR;//请购单实体
            if (pr == null)
                return;
            //赋值建议供应商
            #region 赋值建议供应商

            // 王希提： 1、请购单上指导单价是在单据提交后才带过去，需要调整为保存的时候就带过去单价（主要是为了满足：没有价格时候不允许提交）
            // 提交时机赋值吧，要不每次都调用，浪费；
            bool isSubmit = false;
            if (pr.OriginalData.Status == PRStatusEnum.OpenOpen 
                || pr.Status == PRStatusEnum.Approving
                )
            {
                isSubmit = true;
            }
            if (isSubmit)
            {
                U9.VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE.DeptItemSupplierLine deptLine = null;
                foreach (PRLine line in pr.PRLineList)
                {
                    if (line.ReqDept != null && line.ItemInfo != null)
                    {
                        deptLine = U9.VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE.DeptItemSupplierLine.Finder.Find("ItemMaster.Code='" + line.ItemInfo.ItemID.Code + "' and DeptItemSupplier.Department.Name='" + line.ReqDept.Name + "'");
                        if (deptLine != null
                            && deptLine.Supplier != null
                            )
                        {
                            // 提交强制赋值
                            //建议供应商
                            //if (line.SuggestedSupplier == null || line.SuggestedSupplier.Supplier == null)
                            {
                                string suptCode = deptLine.Supplier.Code;
                                Supplier sugSupt = Supplier.Finder.Find("Code=@Code and Org=@Org", new OqlParam(suptCode),new OqlParam(Context.LoginOrg.ID));

                                if (sugSupt != null)
                                {
                                    line.SuggestedSupplier = new UFIDA.U9.CBO.SCM.Supplier.SupplierMISCInfo();
                                    line.SuggestedSupplier.Code = sugSupt.Code;
                                    line.SuggestedSupplier.Name = sugSupt.Name;
                                    line.SuggestedSupplier.Supplier = sugSupt;
                                    // 不可以改这个，改这个等于改供应商实体的属性了
                                    //line.SuggestedSupplier.Supplier.Code = sugSupt.Code;
                                }
                            }
                            // 提交强制赋值
                            //建议价格
                            //if (line.SuggestedPrice == 0)
                            {
                                UFIDA.U9.PPR.PurPriceList.PurPriceLine purPriceLine = UFIDA.U9.PPR.PurPriceList.PurPriceLine.Finder.Find("ItemInfo.ItemID.Code='" + line.ItemInfo.ItemID.Code + "' and Active=1 and FromDate<=getdate() and ToDate >=getdate() and PurPriceList.Supplier.Code='" + deptLine.Supplier.Code + "' and PurPriceList.ID in (select PurchasePriceList from U9::VOB::Cus::HBHJianLiYuan::PPLDepartmentBE::PPLDepartment where Department.Name='" + line.ReqDept.Name + "')");
                                if (purPriceLine != null)
                                {
                                    // 修改为 取折前价，折后价不取
                                    decimal preDiscountPrice = HBHHelper.PPLineHelper.GetPreDiscountPrice(purPriceLine);
                                    decimal discountedPrice = HBHHelper.PPLineHelper.GetFinallyPrice(purPriceLine);

                                    if (pr.AC != null)
                                    {
                                        line.SuggestedPrice = pr.AC.PriceRound.GetRoundValue(preDiscountPrice);
                                        line.MoneyAC = pr.AC.MoneyRound.GetRoundValue(line.SuggestedPrice * line.ReqQtyTU);
                                    }
                                    else
                                    {
                                        line.SuggestedPrice = preDiscountPrice;
                                        line.MoneyAC = line.SuggestedPrice * line.ReqQtyTU;
                                    }

                                    line.SuggestPriceFC = line.SuggestedPrice;
                                    line.MoneyFC = line.MoneyAC;
                                    //line.DescFlexSegments.PrivateDescSeg1 = purPriceLine.PurPriceList.Code;
                                    //line.DescFlexSegments.PrivateDescSeg2 = purPriceLine.DocLineNo.ToString();
                                    //line.DescFlexSegments.PubDescSeg1 = purPriceLine.DescFlexField.PubDescSeg1;
                                    //line.DescFlexSegments.PubDescSeg2 = purPriceLine.DescFlexField.PubDescSeg2;

                                    PRLineHelper.SetSrcPPListCode(line.DescFlexSegments, purPriceLine.PurPriceList.Code);
                                    PRLineHelper.SetSrcPPLineNo(line.DescFlexSegments, purPriceLine.DocLineNo.ToString());

                                    // 后面考虑是不是在 PRLineHelper里加个方法实现这个；
                                    DescFlexFieldHelper.SetPreDiscountPrice(line.DescFlexSegments, preDiscountPrice);
                                    DescFlexFieldHelper.SetDiscountRate(line.DescFlexSegments, purPriceLine.DescFlexField);
                                    DescFlexFieldHelper.SetDiscountLimit(line.DescFlexSegments, purPriceLine.DescFlexField);

                                    // 赋值差额
                                    decimal dif = preDiscountPrice - discountedPrice;
                                    if (dif != HBHHelper.DescFlexFieldHelper.GetPriceDif(line.DescFlexSegments))
                                    {
                                        HBHHelper.DescFlexFieldHelper.SetPriceDif(line.DescFlexSegments, dif);
                                    }
                                }
                            }
                            Session.Current.InList(line);
                        }
                    }
                }

            #endregion

            }

            // 跨组织生成的PR，头需求部门为空
            if (pr.SysState == UFSoft.UBF.PL.Engine.ObjectState.Inserted)
            {
                string srcPRID = PRHeadHelper.GetSrcPPListID(pr.DescFlexField);

                if (!PubClass.IsNull(srcPRID)
                    && pr.PRLineList != null
                    && pr.PRLineList.Count > 0
                    )
                {
                    PRLine prline = pr.PRLineList[0];
                    if (prline != null)
                    {
                        if (pr.ReqDepartmentKey == null
                            && prline.ReqDeptKey != null
                            )
                        {
                            pr.ReqDepartmentKey = prline.ReqDeptKey;
                        }

                        if (pr.AccountOrgKey == null
                            && prline.AccountOrgKey != null
                            )
                        {
                            pr.AccountOrgKey = prline.AccountOrgKey;
                        }
                    }
                }
            }
        }
    }
}
