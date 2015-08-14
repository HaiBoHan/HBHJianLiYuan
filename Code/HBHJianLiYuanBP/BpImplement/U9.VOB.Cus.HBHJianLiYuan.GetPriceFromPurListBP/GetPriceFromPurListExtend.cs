namespace U9.VOB.Cus.HBHJianLiYuan.GetPriceFromPurListBP
{
	using System;
	using System.Collections.Generic;
	using System.Text; 
	using UFSoft.UBF.AopFrame;	
	using UFSoft.UBF.Util.Context;
    using UFIDA.U9.Cust.HBH.Common.CommonLibary;
    using UFSoft.UBF.PL;

	/// <summary>
	/// GetPriceFromPurList partial 
	/// </summary>	
	public partial class GetPriceFromPurList 
	{	
		internal BaseStrategy Select()
		{
			return new GetPriceFromPurListImpementStrategy();	
		}		
	}
	
    //#region  implement strategy	
	/// <summary>
	/// Impement Implement
	/// 
	/// </summary>	
	internal partial class GetPriceFromPurListImpementStrategy : BaseStrategy
	{
		public GetPriceFromPurListImpementStrategy() { }

		public override object Do(object obj)
		{						
			GetPriceFromPurList bpObj = (GetPriceFromPurList)obj;
			
			//get business operation context is as follows
			//IContext context = ContextManager.Context	
			
			//auto generating code end,underside is user custom code
			//and if you Implement replace this Exception Code...
            if (bpObj == null)
                return null;


            //decimal price = 0;
            //if (bpObj.Dept == "" || bpObj.ItemMaster == "")
            //    return price;
            //U9.VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE.DeptItemSupplierLine line = U9.VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE.DeptItemSupplierLine.Finder.Find("ItemMaster='" + bpObj.ItemMaster + "' and DeptItemSupplier.Department='"+bpObj.Dept+"'");
            //if (line != null && line.Supplier != null)
            //{
            //    UFIDA.U9.PPR.PurPriceList.PurPriceLine purPriceLine = UFIDA.U9.PPR.PurPriceList.PurPriceLine.Finder.Find("PurPriceList.Supplier.ID=" + line.Supplier.ID + " and ItemInfo.ItemID.ID=" + bpObj.ItemMaster + " and Active = 1 and FromDate <=getdate() and ToDate >=getdate() ");
            //    if (purPriceLine != null)
            //        price =  purPriceLine.Price;
            //}
            //return price;

            if (bpObj != null
                && bpObj.ItemPrices != null
                && bpObj.ItemPrices.Count > 0
                )
            {
                //StringBuilder sbItemCodes = new StringBuilder();

                //foreach(ItemPrice dto in bpObj.ItemPrices)
                //{
                //    sbItemCodes.Append(dto.ItemCode).Append(",");
                //}

                // string itemCodes = PubClass.GetOpathFromList(

                foreach (ItemPrice dto in bpObj.ItemPrices)
                {
                    string opath = "ItemInfo.ItemID.Code =@ItemCode and Active=1 and @DocDate between FromDate and ToDate and PurPriceList.Supplier in (select deptItemSuptLine.Supplier from U9::VOB::Cus::HBHJianLiYuan::DeptItemSupplierBE::DeptItemSupplierLine deptItemSuptLine where deptItemSuptLine.ItemMaster.Code = @ItemCode and deptItemSuptLine.DeptItemSupplier.Department.Name=@DeptName ) and PurPriceList.ID in (select pplDept.PurchasePriceList from U9::VOB::Cus::HBHJianLiYuan::PPLDepartmentBE::PPLDepartment pplDept where pplDept.Department.Name=@DeptName  )";

                    UFIDA.U9.PPR.PurPriceList.PurPriceLine priceLine = UFIDA.U9.PPR.PurPriceList.PurPriceLine.Finder.Find(opath
                        , new OqlParam("ItemCode", dto.ItemCode)
                        , new OqlParam("DocDate", dto.DocDate)
                        , new OqlParam("DeptName", dto.DepartmentName)
                        );

                    if (priceLine != null)
                    {
                        dto.PreDiscountPrice = HBHHelper.PPLineHelper.GetPreDiscountPrice(priceLine);
                        dto.FinallyPrice = HBHHelper.PPLineHelper.GetFinallyPrice(priceLine);
                        dto.DiscountRate = HBHHelper.DescFlexFieldHelper.GetDiscountRate(priceLine.DescFlexField);
                        dto.DiscountLimit = HBHHelper.DescFlexFieldHelper.GetDiscountLimit(priceLine.DescFlexField);

                        if (priceLine.PurPriceList != null)
                        {
                            dto.SrcPriceListCode = priceLine.PurPriceList.Code;
                        }
                        if (priceLine.DocLineNo != null)
                        {
                            dto.SrcPriceLineNo = priceLine.DocLineNo.ToString();
                        }
                    }
                }
            }

            return bpObj.ItemPrices;
		}		
	}

    //#endregion
	
	
}