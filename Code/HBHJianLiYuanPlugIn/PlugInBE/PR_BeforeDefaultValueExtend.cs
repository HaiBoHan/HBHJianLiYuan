using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Business;
using UFIDA.U9.SM.SO;
using UFSoft.UBF.PL;
using UFIDA.U9.SM.Enums;
using UFIDA.U9.PR.PurchaseRequest;

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
           VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE.DeptItemSupplierLine deptLine = null;
         
           foreach (PRLine line in pr.PRLineList)
           {
               if (line.ReqDept == null && line.ItemInfo != null && line.SuggestedSupplier == null)
               {
                   deptLine = VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE.DeptItemSupplierLine.Finder.Find("ItemMaster=" + line.ItemInfo.ItemID.ID + " and DeptItemSupplier.Department=" + line.ReqDept.ID + "");
                   if (deptLine != null)
                   {
                       //建议供应商
                       line.SuggestedSupplier = new UFIDA.U9.CBO.SCM.Supplier.SupplierMISCInfo();
                       line.SuggestedSupplier.Code = deptLine.Supplier.Code;
                      
                       //建议价格
                       if (line.SuggestedPrice == 0)
                       {
                           UFIDA.U9.PPR.PurPriceList.PurPriceLine purPriceLine = UFIDA.U9.PPR.PurPriceList.PurPriceLine.Finder.Find("ItemInfo.ItemID.ID=" + line.ItemInfo.ItemID.ID + " and Active=1 and FromDate<=getdate() and ToDate >=getdate() and PurPriceList.Supplier.ID=" + deptLine.Supplier.ID + " and PurPriceList.ID in (select PurchasePriceList from U9::VOB::Cus::HBHJianLiYuan::PPLDepartmentBE::PPLDepartment where Department.ID=" + line.ReqDept.ID + ")");
                           if (purPriceLine != null)
                           {
                               if (purPriceLine.DescFlexField != null && purPriceLine.DescFlexField.PubDescSeg1.ToString() != "")
                               {
                                   line.SuggestedPrice = Math.Round(purPriceLine.Price * Decimal.Parse(purPriceLine.DescFlexField.PubDescSeg1), line.TradeCurrency.PriceRound.Precision);
                               }
                               else if (purPriceLine.DescFlexField != null && purPriceLine.DescFlexField.PubDescSeg2.ToString() != "")
                               {
                                   line.SuggestedPrice = purPriceLine.Price - Decimal.Parse(purPriceLine.DescFlexField.PubDescSeg1);
                               }
                               else
                               {
                                   line.SuggestedPrice = purPriceLine.Price;
                               }
                               line.DescFlexSegments.PrivateDescSeg1 = purPriceLine.PurPriceList.Code;
                               line.DescFlexSegments.PrivateDescSeg2 = purPriceLine.DocLineNo.ToString();
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
