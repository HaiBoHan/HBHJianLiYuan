using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Business;
using UFIDA.U9.PM.PO;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    public class PO_BeforeDefaultValueExtend : UFSoft.UBF.Eventing.IEventSubscriber
    {
        public void Notify(params object[] args)
        {
            if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
                return;
            BusinessEntity.EntityKey key = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;

            if (key == null)
                return;
            UFIDA.U9.PM.PO.PurchaseOrder po = key.GetEntity() as UFIDA.U9.PM.PO.PurchaseOrder;//采购订单实体
            if (po == null)
                return;
            VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE.DeptItemSupplierLine deptLine = null;
            foreach (POLine line in po.POLines)
            {
                if (line.OrderPriceTC == 0)
                {
                    if (po.PurDept != null && line.ItemInfo != null)
                    {
                        deptLine = VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE.DeptItemSupplierLine.Finder.Find("ItemMaster=" + line.ItemInfo.ItemID.ID + " and DeptItemSupplier.Department=" + po.PurDept.ID + "");
                        if (deptLine != null)
                        {
                            UFIDA.U9.PPR.PurPriceList.PurPriceLine purPriceLine = UFIDA.U9.PPR.PurPriceList.PurPriceLine.Finder.Find("ItemInfo.ItemID.ID=" + line.ItemInfo.ItemID.ID + " and Active=1 and FromDate<=getdate() and ToDate >=getdate() and PurPriceList.Supplier.ID=" + deptLine.Supplier.ID + " and PurPriceList.ID in (select PurchasePriceList from U9::VOB::Cus::HBHJianLiYuan::PPLDepartmentBE::PPLDepartment where Department.ID=" + po.PurDept.ID + ")");
                            if (purPriceLine != null)
                            {
                                line.OrderPriceTC = purPriceLine.Price;
                                line.DescFlexSegments.PubDescSeg1 = purPriceLine.DescFlexField.PubDescSeg1;
                                line.DescFlexSegments.PubDescSeg2 = purPriceLine.DescFlexField.PubDescSeg2;
                            }
                        }
                    }
                }
                else
                {
                    if (line.PRLineID > 0)
                    {
                        UFIDA.U9.PR.PurchaseRequest.PRLine prLine = UFIDA.U9.PR.PurchaseRequest.PRLine.Finder.FindByID(line.PRLineID);
                        if (prLine != null)
                        {
                            line.DescFlexSegments.PubDescSeg1 = prLine.DescFlexSegments.PubDescSeg1;
                            line.DescFlexSegments.PubDescSeg2 = prLine.DescFlexSegments.PubDescSeg2;
                        }
                    }
                }
            }
        }
    }
}
