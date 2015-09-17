using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Business;
using UFIDA.U9.SM.Ship;
using U9.VOB.Cus.HBHJianLiYuan.HBHHelper;
using UFIDA.U9.CBO.HR.Department;
using UFSoft.UBF.PL;
using U9.VOB.Cus.HBHJianLiYuan.GetPriceFromPurListBP;
using U9.VOB.Cus.HBHJianLiYuan.GetPriceFromPurListBP.Proxy;
using UFIDA.U9.Cust.HBH.Common.CommonLibary;
using UFIDA.U9.SM.Enums;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    public class Ship_BeforeDefaultValue : UFSoft.UBF.Eventing.IEventSubscriber
    {
       public void Notify(params object[] args)
       {
           if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
               return;
           BusinessEntity.EntityKey key = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;

           if (key == null)
               return;
           Ship ship = key.GetEntity() as Ship;//出货单实体
           if (ship == null)
               return;

           // 收货才写批号,出货同请购赋值价格即可。
           bool isUpdatePrice = false;
           if (ship.Status == ShipStateEnum.Creates
               || ship.Status == ShipStateEnum.Empty
               // 或单据日期有变化
               || (ship.OriginalData != null
                    && ship.OriginalData.BusinessDate != ship.BusinessDate
                    )
               )
           {
               // 弃审报错，不允许OBA更新
               if (ship.OriginalData == null
                   || ship.OriginalData.Status != ShipStateEnum.Approved
                   )
               {
                   isUpdatePrice = true;
               }
           }

           if (isUpdatePrice)
           {
               Department lineDept = ship.SaleDept;
               VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE.DeptItemSupplierLine deptLine = null;
               List<ItemPriceData> lstItemDTO = new List<ItemPriceData>(); 
               foreach (ShipLine line in ship.ShipLines)
               {
                    if (line.OrderPriceTC == 0)
                    {
                        if (lineDept != null && line.ItemInfo != null
                            //&& po.Supplier != null
                            //&& po.Supplier.SupplierKey != null
                            )
                        {
                            //deptLine = VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE.DeptItemSupplierLine.Finder.Find("ItemMaster=" + line.ItemInfo.ItemID.ID + " and DeptItemSupplier.Department.Name=" + lineDept.Name + "");
                            //if (deptLine != null)
                            {
                                DateTime dt = DateTime.Today;
                                DateTime docDate = ship.BusinessDate;
                                if (docDate != null
                                    && docDate.Year > 2000
                                    )
                                {
                                    dt = docDate;
                                }

                                //// 王忠伟,如果有日期重叠会怎么样,应该取创建日期更近的
                                //UFIDA.U9.PPR.PurPriceList.PurPriceLine purPriceLine = UFIDA.U9.PPR.PurPriceList.PurPriceLine.Finder.Find("ItemInfo.ItemID.Code=@ItemCode and Active=1 and FromDate<=@Date and ToDate >=@Date and PurPriceList.Supplier.Code=@SuptCode and PurPriceList.ID in (select PurchasePriceList from U9::VOB::Cus::HBHJianLiYuan::PPLDepartmentBE::PPLDepartment where Department.Name=@DeptName) order by CreatedOn desc "
                                //    ,new OqlParam("ItemCode",line.ItemInfo.ItemID.Code)
                                //    , new OqlParam("Date", dt)
                                //    , new OqlParam("SuptCode", po.Supplier.Supplier.Code)
                                //    , new OqlParam("DeptName", lineDept.Name)
                                //    );
                                //if (purPriceLine != null)
                                //{
                                //    decimal preDiscountPrice = HBHHelper.PPLineHelper.GetPreDiscountPrice(purPriceLine);
                                //    decimal discountedPrice = HBHHelper.PPLineHelper.GetFinallyPrice(purPriceLine);

                                //    line.OrderPriceTC = discountedPrice;
                                //    line.FinallyPriceTC = discountedPrice;
                                //    line.FinallyPriceFC = discountedPrice;
                                //    line.FinallyPriceAC = discountedPrice;

                                //    //line.DescFlexSegments.PubDescSeg1 = purPriceLine.DescFlexField.PubDescSeg1;
                                //    //line.DescFlexSegments.PubDescSeg2 = purPriceLine.DescFlexField.PubDescSeg2;


                                //    // 后面考虑是不是在 PRLineHelper里加个方法实现这个；
                                //    DescFlexFieldHelper.SetPreDiscountPrice(line.DescFlexSegments, preDiscountPrice);
                                //    DescFlexFieldHelper.SetDiscountRate(line.DescFlexSegments, DescFlexFieldHelper.GetDiscountRate(purPriceLine.DescFlexField));
                                //    DescFlexFieldHelper.SetDiscountLimit(line.DescFlexSegments, DescFlexFieldHelper.GetDiscountLimit(purPriceLine.DescFlexField));


                                //    // 赋值差额
                                //    decimal dif = preDiscountPrice - discountedPrice;
                                //    if (dif != HBHHelper.DescFlexFieldHelper.GetPriceDif(line.DescFlexSegments))
                                //    {
                                //        HBHHelper.DescFlexFieldHelper.SetPriceDif(line.DescFlexSegments, dif);
                                //    }
                                //}

                                ItemPriceData dto = new ItemPriceData();
                                dto.DocDate = docDate;
                                dto.DepartmentName = lineDept.Name;
                                dto.ItemCode = line.ItemInfo.ItemCode;

                                lstItemDTO.Add(dto);
                            }
                        }
                    }
               }

               if (lstItemDTO.Count > 0)
               { 
                    GetPriceFromPurListProxy proxy = new GetPriceFromPurListProxy();
                    proxy.ItemPrices = lstItemDTO;

                    List<ItemPriceData> lstPrices = proxy.Do();

                    if (lstPrices != null
                        && lstPrices.Count > 0
                        )
                    {
                        ship.ActivityType = SMActivityEnum.OBAUpdate;

                        //// 只更新扩展字段、不更新数量金额，所以做成服务更新即可。
                        //ship.ActivityType = SMActivityEnum.ServiceUpd;

                        foreach (ItemPriceData price in lstPrices)
                        {
                            if (price != null)
                            {
                                foreach (ShipLine line in ship.ShipLines)
                                {
                                    //bool isResetPrice = false;

                                    if (line != null
                                        && line.ItemInfo != null
                                        && line.ItemInfo.ItemIDKey != null
                                        && line.ItemInfo.ItemIDKey.ID > 0
                                        && !PubClass.IsNull(price.ItemCode)
                                        && price.FinallyPrice > 0
                                        && line.ItemInfo.ItemCode == price.ItemCode
                                        )
                                    {
                                        //decimal oldPrice = line.FinallyPriceTC.GetValueOrDefault(0);
                                        // 折前价
                                        DescFlexFieldHelper.SetPreDiscountPrice(line.DescFlexField, price.PreDiscountPrice);
                                        DescFlexFieldHelper.SetDiscountRate(line.DescFlexField,price.DiscountRate);
                                        DescFlexFieldHelper.SetDiscountLimit(line.DescFlexField,price.DiscountLimit);
                                        // 最终价
                                        line.OrderPrice = price.FinallyPrice;
                                        line.OrderPriceTC = price.FinallyPrice;
                                        line.FinallyPriceTC = price.FinallyPrice;
                                        line.FinallyPrice = price.FinallyPrice;


                                        // 清空金额
                                        line.TotalMoney = 0;
                                        line.TotalMoneyTC = 0;
                                        line.TotalMoneyFC = 0;
                                        line.TotalNetMoney = 0;
                                        line.TotalNetMoneyTC = 0;
                                        line.TotalNetMoneyFC = 0;
                                        line.TotalTax = 0;
                                        line.TotalTaxTC = 0;
                                        line.TotalTaxFC = 0;

                                        if (line.ShipTaxs != null
                                            && line.ShipTaxs.Count > 0
                                            )
                                        {
                                            for (int i = line.ShipTaxs.Count - 1; i >= 0; i--)
                                            {
                                                line.ShipTaxs.RemoveAt(i);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
               }

           }
       }
    }
}
