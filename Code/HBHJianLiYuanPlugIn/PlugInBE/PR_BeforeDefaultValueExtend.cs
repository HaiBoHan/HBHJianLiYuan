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

            // 提交时机赋值吧，要不每次都调用，浪费；
            bool isSubmit = false;
            if (pr.OriginalData.Status == PRStatusEnum.OpenOpen 
                && pr.Status == PRStatusEnum.Approving
                )
            {
                isSubmit = true;
            }
            if (isSubmit)
            {
                VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE.DeptItemSupplierLine deptLine = null;
                foreach (PRLine line in pr.PRLineList)
                {
                    if (line.ReqDept != null && line.ItemInfo != null)
                    {
                        deptLine = VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE.DeptItemSupplierLine.Finder.Find("ItemMaster=" + line.ItemInfo.ItemID.ID + " and DeptItemSupplier.Department=" + line.ReqDept.ID + "");
                        if (deptLine != null)
                        {
                            //建议供应商
                            if (line.SuggestedSupplier == null || line.SuggestedSupplier.Supplier == null)
                            {
                                line.SuggestedSupplier = new UFIDA.U9.CBO.SCM.Supplier.SupplierMISCInfo();
                                line.SuggestedSupplier.Code = deptLine.Supplier.Code;
                                line.SuggestedSupplier.Name = deptLine.Supplier.Name;
                                line.SuggestedSupplier.Supplier = new UFIDA.U9.CBO.SCM.Supplier.Supplier();
                                // 不可以改这个，改这个等于改供应商实体的属性了
                                //line.SuggestedSupplier.Supplier.Code = deptLine.Supplier.Code;
                            }
                            //建议价格
                            if (line.SuggestedPrice == 0)
                            {
                                UFIDA.U9.PPR.PurPriceList.PurPriceLine purPriceLine = UFIDA.U9.PPR.PurPriceList.PurPriceLine.Finder.Find("ItemInfo.ItemID.ID=" + line.ItemInfo.ItemID.ID + " and Active=1 and FromDate<=getdate() and ToDate >=getdate() and PurPriceList.Supplier.ID=" + deptLine.Supplier.ID + " and PurPriceList.ID in (select PurchasePriceList from U9::VOB::Cus::HBHJianLiYuan::PPLDepartmentBE::PPLDepartment where Department.ID=" + line.ReqDept.ID + ")");
                                if (purPriceLine != null)
                                {
                                    line.SuggestedPrice = purPriceLine.Price;
                                    //line.DescFlexSegments.PrivateDescSeg1 = purPriceLine.PurPriceList.Code;
                                    //line.DescFlexSegments.PrivateDescSeg2 = purPriceLine.DocLineNo.ToString();
                                    //line.DescFlexSegments.PubDescSeg1 = purPriceLine.DescFlexField.PubDescSeg1;
                                    //line.DescFlexSegments.PubDescSeg2 = purPriceLine.DescFlexField.PubDescSeg2;

                                    PRLineHelper.SetSrcPPListCode(line.DescFlexSegments, purPriceLine.PurPriceList.Code);
                                    PRLineHelper.SetSrcPPLineNo(line.DescFlexSegments, purPriceLine.DocLineNo.ToString());

                                    // 后面考虑是不是在 PRLineHelper里加个方法实现这个；
                                    DescFlexFieldHelper.SetPreDiscountPrice(line.DescFlexSegments, DescFlexFieldHelper.GetPreDiscountPrice(purPriceLine.DescFlexField));
                                    DescFlexFieldHelper.SetDiscountRate(line.DescFlexSegments, DescFlexFieldHelper.GetDiscountRate(purPriceLine.DescFlexField));
                                    DescFlexFieldHelper.SetDiscountLimit(line.DescFlexSegments, DescFlexFieldHelper.GetDiscountLimit(purPriceLine.DescFlexField));
                                }
                            }

                        }
                    }
                    Session.Current.InList(line);
                }

            #endregion

            }
        }
    }
}
