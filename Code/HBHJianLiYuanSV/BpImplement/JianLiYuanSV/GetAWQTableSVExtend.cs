namespace U9.VOB.Cus.HBHJianLiYuan
{
	using System;
	using System.Collections.Generic;
	using System.Text; 
	using UFSoft.UBF.AopFrame;	
	using UFSoft.UBF.Util.Context;
    using HBH.DoNet.DevPlatform.U9Mapping;
    using HBH.DoNet.DevPlatform.EntityMapping;
    using UFSoft.UBF.Business;

	/// <summary>
	/// GetAWQTableSV partial 
	/// </summary>	
	public partial class GetAWQTableSV 
	{	
		internal BaseStrategy Select()
		{
			return new GetAWQTableSVImpementStrategy();	
		}		
	}
	
    //#region  implement strategy	
	/// <summary>
	/// Impement Implement
	/// 
	/// </summary>	
	internal partial class GetAWQTableSVImpementStrategy : BaseStrategy
	{
		public GetAWQTableSVImpementStrategy() { }

		public override object Do(object obj)
		{						
			GetAWQTableSV bpObj = (GetAWQTableSV)obj;
			
			//get business operation context is as follows
			//IContext context = ContextManager.Context	
			
			//auto generating code end,underside is user custom code
			//and if you Implement replace this Exception Code...
            //throw new NotImplementedException();


            string procName = "HBH_SP_JianLiYuan_GetAQWTableFromAWQ";
            List<ParamDTO> lstParam = new List<ParamDTO>();
            //{
            //    HBH.DoNet.DevPlatform.U9Mapping.ParamDTO param = new HBH.DoNet.DevPlatform.U9Mapping.ParamDTO();
            //    param.ParamName = "Org";
            //    param.ParamType = System.Data.DbType.Int64;
            //    param.ParamDirection = ParameterDirection.Input;
            //    param.ParamValue = Context.LoginOrg.ID;
            //    lstParam.Add(param);
            //}
            //{
            //    HBH.DoNet.DevPlatform.U9Mapping.ParamDTO param = new HBH.DoNet.DevPlatform.U9Mapping.ParamDTO();
            //    param.ParamName = "ItemIDs";
            //    param.ParamType = System.Data.DbType.AnsiString;
            //    param.ParamDirection = ParameterDirection.Input;
            //    param.ParamValue = strItemIDs;
            //    lstParam.Add(param);
            //}

            //DataTable dt = null;

            EntityResult result = U9Helper.GetResultByProcess(procName, lstParam.ToArray());

            if (result != null
                )
            {
                if (result.Sucessfull)
                {
                    return null;
                }
                else
                {
                    throw new BusinessException(string.Format("同步奥琦玮数据失败:{0}"
                        , result.Message
                        ));
                }
            }
            else
            {
                throw new BusinessException(string.Format("同步奥琦玮数据失败:{0}"
                    , "执行过程无返回结果!"
                    ));
            }

		}		
	}

    //#endregion
	
	
}