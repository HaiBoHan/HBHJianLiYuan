namespace U9.VOB.Cus.HBHJianLiYuan
{
	using System;
	using System.Collections.Generic;
	using System.Text; 
	using UFSoft.UBF.AopFrame;	
	using UFSoft.UBF.Util.Context;
    using UFSoft.UBF.Business;

	/// <summary>
	/// UpdateDayCheckInStatusBP partial 
	/// </summary>	
	public partial class UpdateDayCheckInStatusBP 
	{	
		internal BaseStrategy Select()
		{
			return new UpdateDayCheckInStatusBPImpementStrategy();	
		}		
	}
	
    //#region  implement strategy	
	/// <summary>
	/// Impement Implement
	/// 
	/// </summary>	
	internal partial class UpdateDayCheckInStatusBPImpementStrategy : BaseStrategy
	{
		public UpdateDayCheckInStatusBPImpementStrategy() { }

		public override object Do(object obj)
		{						
			UpdateDayCheckInStatusBP bpObj = (UpdateDayCheckInStatusBP)obj;
			
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
                    foreach (DayCheckIn.EntityKey key in bpObj.HeadIDs)
                    {
                        DayCheckIn entity = key.GetEntity();

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