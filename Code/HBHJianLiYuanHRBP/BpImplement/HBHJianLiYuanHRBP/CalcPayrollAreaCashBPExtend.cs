namespace U9.VOB.Cus.HBHJianLiYuan
{
	using System;
	using System.Collections.Generic;
	using System.Text; 
	using UFSoft.UBF.AopFrame;	
	using UFSoft.UBF.Util.Context;
    using HBH.DoNet.DevPlatform.EntityMapping;
    using System.Data;
    using UFIDA.U9.PAY.PayrollResult;
    using UFSoft.UBF.PL;
    using UFSoft.UBF.Business;

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
	
    //#region  implement strategy	
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
            
            if (bpObj != null
                && bpObj.IDs != null
                && bpObj.IDs.Count > 0
                )
            {
                foreach (long id in bpObj.IDs)
                {

                    if (id > 0
                        )
                    {
                        Dictionary<long, List<AreaCash>> dicEmployee2AreaCash = new Dictionary<long, List<AreaCash>>();

                        HBH.DoNet.DevPlatform.U9Mapping.ProcedureMapping procMapping = new HBH.DoNet.DevPlatform.U9Mapping.ProcedureMapping();
                        procMapping.ProcedureName = "HBH_SP_JianLiYuan_GetPayrollAreaCash";
                        procMapping.Params = new List<HBH.DoNet.DevPlatform.U9Mapping.ParamDTO>();

                        {
                            HBH.DoNet.DevPlatform.U9Mapping.ParamDTO suptParam = new HBH.DoNet.DevPlatform.U9Mapping.ParamDTO();
                            suptParam.ParamName = "PayrollDoc";
                            suptParam.ParamType = System.Data.DbType.Int64;
                            suptParam.ParamValue = -1;
                            procMapping.Params.Add(suptParam);
                        }

                        {
                            HBH.DoNet.DevPlatform.U9Mapping.ParamDTO suptParam = new HBH.DoNet.DevPlatform.U9Mapping.ParamDTO();
                            suptParam.ParamName = "PayrollCalculate";
                            suptParam.ParamType = System.Data.DbType.Int64;
                            suptParam.ParamValue = id;
                            procMapping.Params.Add(suptParam);
                        }


                        U9.VOB.HBHCommon.Proxy.U9CommonSVProxy proxy = new U9.VOB.HBHCommon.Proxy.U9CommonSVProxy();

                        proxy.EntityFullName = typeof(HBH.DoNet.DevPlatform.U9Mapping.ProcedureMapping).FullName;
                        proxy.EntityContent = HBH.DoNet.DevPlatform.EntityMapping.EntitySerialization.EntitySerial(procMapping);

                        string strResult = proxy.Do();

                        if (!PubClass.IsNull(strResult))
                        {
                            HBH.DoNet.DevPlatform.EntityMapping.EntityResult result = HBH.DoNet.DevPlatform.EntityMapping.EntitySerialization.EntityDeserial<HBH.DoNet.DevPlatform.EntityMapping.EntityResult>(strResult);

                            if (result != null)
                            {
                                if (result.Sucessfull)
                                {
                                    if (!PubClass.IsNull(result.StringValue))
                                    {
                                        DataSet ds = EntitySerialization.EntityDeserial<DataSet>(result.StringValue);


                                        if (ds != null
                                            && ds.Tables != null
                                            && ds.Tables.Count > 0
                                            )
                                        {
                                            DataTable tb = ds.Tables[0];

                                            if (tb != null
                                                && tb.Rows != null
                                                && tb.Rows.Count > 0
                                                )
                                            {
                                                foreach (DataRow row in tb.Rows)
                                                {
                                                    AreaCash dto = AreaCash.GetFromDataRow(row);

                                                    if (dto != null
                                                        && dto.EmployeeArchive > 0
                                                        )
                                                    {
                                                        if (!dicEmployee2AreaCash.ContainsKey(dto.EmployeeArchive))
                                                        {
                                                            dicEmployee2AreaCash.Add(dto.EmployeeArchive, new List<AreaCash>());
                                                        }

                                                        dicEmployee2AreaCash[dto.EmployeeArchive].Add(dto);
                                                    }
                                                }
                                            }
                                        }

                                    }


                                    StringBuilder sbOpath = new StringBuilder();
                                    sbOpath.Append("PayrollCaculate=@CalcID and (1=1 ");

                                    sbOpath.Append(" ) ");
                                    PayrollResult.EntityList payResultList = PayrollResult.Finder.FindAll(sbOpath.ToString()
                                        , new OqlParam(id)
                                        );

                                    if (payResultList != null
                                        && payResultList.Count > 0
                                        )
                                    {
                                        SetSalaryItem(dicEmployee2AreaCash, payResultList);
                                    }
                                }
                                else
                                {
                                    throw new BusinessException(result.Message);
                                }
                            }
                            else
                            {
                                throw new BusinessException("客开程序执行异常,无返回结果!");
                            }
                        }
                    }
                }
            }


            return null;
		}

        private void SetSalaryItem(Dictionary<long, List<AreaCash>> dicEmployee2AreaCash, PayrollResult.EntityList payResultList)
        {
            throw new NotImplementedException();
        }		
	}

    //#endregion

    /// <summary>
    /// 区域应兑现DTO
    /// </summary>
    public class AreaCash
    {
        // 部门
        private long department;
        /// <summary>
        /// 部门
        /// </summary>
        public long Department
        {
            get { return department; }
            set { department = value; }
        }

        // 员工
        private long employeeArchive;
        /// <summary>
        /// 员工
        /// </summary>
        public long EmployeeArchive
        {
            get { return employeeArchive; }
            set { employeeArchive = value; }
        }

        // 应兑现金额
        private decimal shouldBeCash;
        /// <summary>
        /// 应兑现金额
        /// </summary>
        public decimal ShouldBeCash
        {
            get { return shouldBeCash; }
            set { shouldBeCash = value; }
        }


        public static AreaCash GetFromDataRow(DataRow row)
        {
            AreaCash dto = new AreaCash();

            dto.EmployeeArchive = PubClass.GetLong(row.GetValue("EmployeeArchive"));
            dto.Department = PubClass.GetLong(row.GetValue("Department"));
            dto.ShouldBeCash = PubClass.GetInt(row.GetValue("ShouldBeCash"));
            
            return dto;
        }

    }
	
}