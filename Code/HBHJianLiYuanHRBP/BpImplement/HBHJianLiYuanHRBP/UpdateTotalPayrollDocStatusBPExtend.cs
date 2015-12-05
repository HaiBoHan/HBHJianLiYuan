namespace U9.VOB.Cus.HBHJianLiYuan
{
	using System;
	using System.Collections.Generic;
	using System.Text; 
	using UFSoft.UBF.AopFrame;	
	using UFSoft.UBF.Util.Context;
    using UFSoft.UBF.Business;

	/// <summary>
	/// UpdateTotalPayrollDocStatusBP partial 
	/// </summary>	
	public partial class UpdateTotalPayrollDocStatusBP 
	{	
		internal BaseStrategy Select()
		{
			return new UpdateTotalPayrollDocStatusBPImpementStrategy();	
		}		
	}
	
    //#region  implement strategy	
	/// <summary>
	/// Impement Implement
	/// 
	/// </summary>	
	internal partial class UpdateTotalPayrollDocStatusBPImpementStrategy : BaseStrategy
	{
		public UpdateTotalPayrollDocStatusBPImpementStrategy() { }

		public override object Do(object obj)
		{						
			UpdateTotalPayrollDocStatusBP bpObj = (UpdateTotalPayrollDocStatusBP)obj;
			
			//get business operation context is as follows
			//IContext context = ContextManager.Context	
			
			//auto generating code end,underside is user custom code
			//and if you Implement replace this Exception Code...
            //throw new NotImplementedException();

            if (bpObj.HeadIDs != null
                && bpObj.HeadIDs.Count > 0
                )
            {
                using (ISession session = Session.Open())
                {
                    foreach (TotalPayrollDoc.EntityKey key in bpObj.HeadIDs)
                    {
                        TotalPayrollDoc entity = key.GetEntity();

                        if (entity != null)
                        {
                            entity.Status = bpObj.TargetStatus;
                        }
                    }

                    session.Commit();
                }
            }

            return null;
		}		
	}

    //#endregion
	
	
}