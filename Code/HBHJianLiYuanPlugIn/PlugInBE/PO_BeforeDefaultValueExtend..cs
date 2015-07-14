using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Business;
using UFIDA.U9.PM.PO;
using UFIDA.U9.PR.PurchaseRequest;
using UFIDA.U9.CBO.HR.Department;
using U9.VOB.Cus.HBHJianLiYuan.HBHHelper;
using UFSoft.UBF.PL;

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

            #region 公共段不需要赋值,最终价理论上也应该会从请购单上带过来，有问题了再说

            if (po.SysState == UFSoft.UBF.PL.Engine.ObjectState.Inserted)
            {
                //// 取部门
                //if (po.SysState == UFSoft.UBF.PL.Engine.ObjectState.Inserted)
                //{
                //    //if (lineDept == null)
                //    //{
                //    //    if (prline.ReqDept != null)
                //    //    {
                //    //        string deptName = prline.ReqDept.Name;
                //    //        Department targetDept = Department.Finder.Find("Name=@Name and Org=@Org"
                //    //            , new OqlParam(deptName)
                //    //            , new OqlParam(org.ID)
                //    //            );

                //    //        if (targetDept != null)
                //    //        {
                //    //            lineData.ReqDept = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                //    //            //lineData.ReqDept.Code = prline.ReqDept.Code;
                //    //            //lineData.ReqDept.Name = prline.ReqDept.Name;
                //    //            lineData.ReqDept.Code = targetDept.Code;
                //    //        }
                //    //        else
                //    //        {
                //    //            string msg = string.Format("组织[{0}]中没有名称为[{1}]的部门!"
                //    //                , org.Name, deptName);
                //    //            throw new BusinessException(msg);
                //    //        }
                //    //    }
                //    //}

                //    foreach (POLine line in po.POLines)
                //    {
                //        if (line != null
                //            && line.SrcDocInfo != null
                //            && line.SrcDocInfo.SrcDocLine != null
                //            && line.SrcDocInfo.SrcDocLine.EntityID > 0
                //            )
                //        {
                //            PRLine prline = PRLine.Finder.FindByID(line.SrcDocInfo.SrcDocLine.EntityID);

                //            if (prline != null)
                //            {

                //                foreach (POShipLine subline in line.POShiplines)
                //                {
                //                    if (subline.RequireDeptKey == null)
                //                    {
                //                        subline.RequireDeptKey = prline.ReqDeptKey;
                //                    }
                //                }

                //                //// 取建议价格
                //                //line.FinallyPriceTC = prline.SuggestedPrice;
                //                //line.FinallyPriceFC = prline.SuggestPriceFC;
                //                //line.FinallyPriceAC = line.FinallyPriceTC;
                //            }
                //        }
                //    }
                //}


                VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE.DeptItemSupplierLine deptLine = null;
                foreach (POLine line in po.POLines)
                {
                    if (line.OrderPriceTC == 0)
                    {
                        POShipLine subline = line.POShiplines[0];
                        Department lineDept = null;
                        if (subline != null)
                        {
                            lineDept = subline.RequireDept;
                        }

                        if (lineDept != null && line.ItemInfo != null
                            && po.Supplier != null
                            && po.Supplier.SupplierKey != null
                            )
                        {
                            //deptLine = VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE.DeptItemSupplierLine.Finder.Find("ItemMaster=" + line.ItemInfo.ItemID.ID + " and DeptItemSupplier.Department.Name=" + lineDept.Name + "");
                            //if (deptLine != null)
                            {
                                DateTime dt = DateTime.Today;
                                DateTime docDate = GetDocDate(line);
                                if (docDate != null
                                    && docDate.Year > 2000
                                    )
                                {
                                    dt = docDate;
                                }

                                UFIDA.U9.PPR.PurPriceList.PurPriceLine purPriceLine = UFIDA.U9.PPR.PurPriceList.PurPriceLine.Finder.Find("ItemInfo.ItemID.Code=@ItemCode and Active=1 and FromDate<=@Date and ToDate >=@Date and PurPriceList.Supplier.Code=@SuptCode and PurPriceList.ID in (select PurchasePriceList from U9::VOB::Cus::HBHJianLiYuan::PPLDepartmentBE::PPLDepartment where Department.Name=@DeptName)"
                                    ,new OqlParam("ItemCode",line.ItemInfo.ItemID.Code)
                                    , new OqlParam("Date", dt)
                                    , new OqlParam("SuptCode", po.Supplier.SupplierKey.ID)
                                    , new OqlParam("DeptName", lineDept.Name)
                                    );
                                if (purPriceLine != null)
                                {
                                    decimal preDiscountPrice = HBHHelper.PPLineHelper.GetPreDiscountPrice(purPriceLine);
                                    decimal discountedPrice = HBHHelper.PPLineHelper.GetFinallyPrice(purPriceLine);

                                    line.OrderPriceTC = discountedPrice;
                                    line.FinallyPriceTC = discountedPrice;
                                    line.FinallyPriceFC = discountedPrice;
                                    line.FinallyPriceAC = discountedPrice;

                                    //line.DescFlexSegments.PubDescSeg1 = purPriceLine.DescFlexField.PubDescSeg1;
                                    //line.DescFlexSegments.PubDescSeg2 = purPriceLine.DescFlexField.PubDescSeg2;


                                    // 后面考虑是不是在 PRLineHelper里加个方法实现这个；
                                    DescFlexFieldHelper.SetPreDiscountPrice(line.DescFlexSegments, preDiscountPrice);
                                    DescFlexFieldHelper.SetDiscountRate(line.DescFlexSegments, DescFlexFieldHelper.GetDiscountRate(purPriceLine.DescFlexField));
                                    DescFlexFieldHelper.SetDiscountLimit(line.DescFlexSegments, DescFlexFieldHelper.GetDiscountLimit(purPriceLine.DescFlexField));


                                    // 赋值差额
                                    decimal dif = preDiscountPrice - discountedPrice;
                                    if (dif != HBHHelper.DescFlexFieldHelper.GetPriceDif(line.DescFlexSegments))
                                    {
                                        HBHHelper.DescFlexFieldHelper.SetPriceDif(line.DescFlexSegments, dif);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //if (line.PRLineID > 0)
                        //{
                        //    UFIDA.U9.PR.PurchaseRequest.PRLine prLine = UFIDA.U9.PR.PurchaseRequest.PRLine.Finder.FindByID(line.PRLineID);
                        //    if (prLine != null)
                        //    {
                        //        line.DescFlexSegments.PubDescSeg1 = prLine.DescFlexSegments.PubDescSeg1;
                        //        line.DescFlexSegments.PubDescSeg2 = prLine.DescFlexSegments.PubDescSeg2;
                        //    }
                        //}
                    }
                }
            }

            #endregion
        }

        private DateTime GetDocDate(POLine line)
        {
            if (line != null
                && line.POShiplines != null
                && line.POShiplines.Count > 0
                )
            {
                POShipLine subline = line.POShiplines[0];

                if (subline != null)
                {
                    return subline.DeliveryDate;
                }
            }

            return DateTime.Today;
        }
    }
}
