namespace U9.VOB.Cus.HBHJianLiYuan.ShipPickByDocBP
{
	using System;
	using System.Collections.Generic;
	using System.Text; 
	using UFSoft.UBF.AopFrame;	
	using UFSoft.UBF.Util.Context;

	/// <summary>
	/// ShipPickByDoc partial 
	/// </summary>	
	public partial class ShipPickByDoc 
	{	
		internal BaseStrategy Select()
		{
			return new ShipPickByDocImpementStrategy();	
		}		
	}
	
	#region  implement strategy	
	/// <summary>
	/// Impement Implement
	/// 
	/// </summary>	
	internal partial class ShipPickByDocImpementStrategy : BaseStrategy
	{
		public ShipPickByDocImpementStrategy() { }

		public override object Do(object obj)
		{						
			ShipPickByDoc bpObj = (ShipPickByDoc)obj;
			
			//get business operation context is as follows
			//IContext context = ContextManager.Context	
			
			//auto generating code end,underside is user custom code
			//and if you Implement replace this Exception Code...
            UFIDA.U9.ISV.SM.ShipPickByDoc proxy = new UFIDA.U9.ISV.SM.ShipPickByDoc();
            proxy.ShipNos = bpObj.ShipNos;
            proxy.Do();
            return null;
		}		
	}

	#endregion
	
	
}