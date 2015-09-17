namespace U9.VOB.Cus.HBHJianLiYuan
{
	using System;
	using System.Collections.Generic;
	using System.Text; 
	using UFSoft.UBF.AopFrame;	
	using UFSoft.UBF.Util.Context;
    using UFIDA.U9.Cust.HBH.Common.CommonLibary;
    using UFIDA.U9.PM.Rcv;

	/// <summary>
	/// RcvToShipSV partial 
	/// </summary>	
	public partial class RcvToShipSV 
	{	
		internal BaseStrategy Select()
		{
			return new RcvToShipSVImpementStrategy();	
		}		
	}
	
    //#region  implement strategy	
	/// <summary>
	/// Impement Implement
	/// 
	/// </summary>	
	internal partial class RcvToShipSVImpementStrategy : BaseStrategy
	{
		public RcvToShipSVImpementStrategy() { }

		public override object Do(object obj)
		{						
			RcvToShipSV bpObj = (RcvToShipSV)obj;
			
			//get business operation context is as follows
			//IContext context = ContextManager.Context	
			
			//auto generating code end,underside is user custom code
			//and if you Implement replace this Exception Code...
            //throw new NotImplementedException();

            // 按收货ID转单
            if (bpObj.RcvIDs != null
                && bpObj.RcvIDs.Count > 0
                )
            { 
                string opath = string.Format("ID in ({0})"
                    , PubClass.GetOpathFromList(bpObj.RcvIDs)
                    );

                Receivement.EntityList lstRcv = Receivement.Finder.FindAll(opath);
            }
            // 按收货日期转单
            else
            {
                DateTime date = bpObj.Date;
                // 没传入日期，当天
                if (PubClass.IsNullDate2010(date))
                {
                    date = DateTime.Today;
                }
            }


            return null;
		}		
	}

    //#endregion
	
	
}