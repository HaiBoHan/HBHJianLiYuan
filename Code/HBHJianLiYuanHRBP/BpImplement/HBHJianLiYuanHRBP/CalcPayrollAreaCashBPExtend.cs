namespace U9.VOB.Cus.HBHJianLiYuan
{
	using System;
	using System.Collections.Generic;
	using System.Text; 
	using UFSoft.UBF.AopFrame;	
	using UFSoft.UBF.Util.Context;

	/// <summary>
	/// CalcPayrollAreaCashBP partial 
	/// </summary>	
	public partial class CalcPayrollAreaCashBP 
	{	
		internal BaseStrategy Select()
		{
			return new CalcPayrollAreaCashBPImpementStrategy();	
		}		
	}
	
	#region  implement strategy	
	/// <summary>
	/// Impement Implement
	/// 
	/// </summary>	
	internal partial class CalcPayrollAreaCashBPImpementStrategy : BaseStrategy
	{
		public CalcPayrollAreaCashBPImpementStrategy() { }

		public override object Do(object obj)
		{						
			CalcPayrollAreaCashBP bpObj = (CalcPayrollAreaCashBP)obj;
			
			//get business operation context is as follows
			//IContext context = ContextManager.Context	
			
			//auto generating code end,underside is user custom code
			//and if you Implement replace this Exception Code...
			throw new NotImplementedException();
		}		
	}

	#endregion
	
	
}