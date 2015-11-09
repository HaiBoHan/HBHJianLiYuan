namespace U9.VOB.Cus.HBHJianLiYuan
{
	using System;
	using System.Collections.Generic;
	using System.Text; 
	using UFSoft.UBF.AopFrame;	
	using UFSoft.UBF.Util.Context;
    using UFSoft.UBF.Business;

	/// <summary>
	/// DepartImportCheckInBP partial 
	/// </summary>	
	public partial class DepartImportCheckInBP 
	{	
		internal BaseStrategy Select()
		{
			return new DepartImportCheckInBPImpementStrategy();	
		}		
	}
	
    //#region  implement strategy	
	/// <summary>
	/// Impement Implement
	/// 
	/// </summary>	
	internal partial class DepartImportCheckInBPImpementStrategy : BaseStrategy
	{
        public const string Const_DepartImportProcedure = "HBH_SP_JianLiYuan_DepartImport";

		public DepartImportCheckInBPImpementStrategy() { }

		public override object Do(object obj)
		{						
			DepartImportCheckInBP bpObj = (DepartImportCheckInBP)obj;
			
			//get business operation context is as follows
			//IContext context = ContextManager.Context	
			
			//auto generating code end,underside is user custom code
			//and if you Implement replace this Exception Code...
            //throw new NotImplementedException();


            if (bpObj != null
                && bpObj.CheckInIDs != null
                && bpObj.CheckInIDs.Count > 0
                )
            {
                foreach (long id in bpObj.CheckInIDs)
                {

                    UFSoft.UBF.Util.DataAccess.DataParamList sqlParams = new UFSoft.UBF.Util.DataAccess.DataParamList();

                    //sqlParams.Add(UFSoft.UBF.Util.DataAccess.DataParamFactory.CreateInput("@ItemList"
                    //    , bpObj.InParam.ItemList, System.Data.DbType.AnsiString));

                    //sqlParams.Add(UFSoft.UBF.Util.DataAccess.DataParamFactory.CreateInput("@ShipLineID"
                    //    , bpObj.InParam.ShipLineID, System.Data.DbType.Int64));
                    
                    //sqlParams.Add(UFSoft.UBF.Util.DataAccess.DataParamFactory.CreateInput("@IsFuzzySalesman"
                    //    , bpObj.InParam.IsFuzzySalesman ? 1 : 0, System.Data.DbType.Int16));

                    //sqlParams.Add(UFSoft.UBF.Util.DataAccess.DataParamFactory.CreateInput("@IsContainBranchWh"
                    //    , bpObj.InParam.IsContainBranchWh ? 1 : 0, System.Data.DbType.Int16));


                    //sqlParams.Add(UFSoft.UBF.Util.DataAccess.DataParamFactory.CreateInput("@ShipLineList"
                    //    , bpObj.InParam.ShipLineList, System.Data.DbType.AnsiString));
                    
                    
                    sqlParams.Add(UFSoft.UBF.Util.DataAccess.DataParamFactory.CreateInput("@ID"
                        , id, System.Data.DbType.Int64));

                    try
                    {
                        System.Data.DataSet ds = new System.Data.DataSet();
                        UFSoft.UBF.Util.DataAccess.DataAccessor.RunSP(Const_DepartImportProcedure, sqlParams, out ds);

                        //if (ds != null
                        //    && ds.Tables != null
                        //    && ds.Tables.Count > 0
                        //    && ds.Tables[0] != null
                        //    && ds.Tables[0].Rows != null
                        //    && ds.Tables[0].Rows.Count > 0
                        //    )
                        //{
                        //    List<LotWhqohQtyDTO> list = new List<LotWhqohQtyDTO>();
                        //    foreach (DataRow row in ds.Tables[0].Rows)
                        //    {
                        //        LotWhqohQtyDTO dto = new LotWhqohQtyDTO();

                        //        dto.ID = PubClass.GetLong(row["ID"]);

                        //        // 先设置批号扩展字段，因为后面会修改扩展字段的值(实时查询的物试批号、炉号、炉号供应商)
                        //        SetLotDescFlexFields(row, dto);


                        //        dto.ItemID = PubClass.GetLong(row["ItemID"]);
                        //        dto.ItemCode = PubClass.GetString(row["ItemCode"]);
                        //        dto.ItemName = PubClass.GetString(row["ItemName"]);

                        //        list.Add(dto);
                        //    }
                        //    return list;
                        //}
                    }
                    catch (Exception ex)
                    {
                        throw new BusinessException(ex.Message);
                    }

                }
            }

            return null;
		}		
	}

    //#endregion
	
	
}