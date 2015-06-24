namespace U9.VOB.Cus.HBHJianLiYuan.GetPriceFromPurListBP
{
	using System;
	using System.Collections.Generic;
	using System.Text; 
	using UFSoft.UBF.AopFrame;	
	using UFSoft.UBF.Util.Context;

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
	
	#region  implement strategy	
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
            decimal price = 0;
            if (bpObj.Dept == "" || bpObj.ItemMaster == "")
                return price;
            U9.VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE.DeptItemSupplierLine line = U9.VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE.DeptItemSupplierLine.Finder.Find("ItemMaster='" + bpObj.ItemMaster + "' and DeptItemSupplier.Department='"+bpObj.Dept+"'");
            if (line != null && line.Supplier != null)
            {
                UFIDA.U9.PPR.PurPriceList.PurPriceLine purPriceLine = UFIDA.U9.PPR.PurPriceList.PurPriceLine.Finder.Find("PurPriceList.Supplier.ID=" + line.Supplier.ID + " and ItemInfo.ItemID.ID=" + bpObj.ItemMaster + " and Active = 1 and FromDate <=getdate() and ToDate >=getdate() ");
                if (purPriceLine != null)
                    price =  purPriceLine.Price;
            }
            return price;
		}		
	}

	#endregion
	
	
}