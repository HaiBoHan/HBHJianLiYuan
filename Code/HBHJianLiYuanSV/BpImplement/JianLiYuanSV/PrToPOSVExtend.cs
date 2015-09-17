namespace U9.VOB.Cus.HBHJianLiYuan
{
	using System;
	using System.Collections.Generic;
	using System.Text; 
	using UFSoft.UBF.AopFrame;	
	using UFSoft.UBF.Util.Context;

	/// <summary>
	/// PrToPOSV partial 
	/// </summary>	
	public partial class PrToPOSV 
	{	
		internal BaseStrategy Select()
		{
			return new PrToPOSVImpementStrategy();	
		}		
	}
	
	#region  implement strategy	
	/// <summary>
	/// Impement Implement
	/// 
	/// </summary>	
	internal partial class PrToPOSVImpementStrategy : BaseStrategy
	{
		public PrToPOSVImpementStrategy() { }

		public override object Do(object obj)
		{						
			PrToPOSV bpObj = (PrToPOSV)obj;
			
			//get business operation context is as follows
			//IContext context = ContextManager.Context	
			
			//auto generating code end,underside is user custom code
			//and if you Implement replace this Exception Code...
			throw new NotImplementedException();
		}		
	}

	#endregion
	
	
}