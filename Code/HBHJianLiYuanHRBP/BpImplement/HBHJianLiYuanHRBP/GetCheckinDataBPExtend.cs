namespace U9.VOB.Cus.HBHJianLiYuan
{
	using System;
	using System.Collections.Generic;
	using System.Text; 
	using UFSoft.UBF.AopFrame;	
	using UFSoft.UBF.Util.Context;
    using UFSoft.UBF.Business;
    using HBH.DoNet.DevPlatform.EntityMapping;
    using System.Data;
    using UFSoft.UBF.PL;
    using UFIDA.U9.PAY.PayrollResult;
    using UFIDA.U9.PAY.SalaryItem;
    using U9.VOB.Cus.HBHJianLiYuan.HBHHelper;
    using UFIDA.U9.PAY.PayrollDoc;
    using UFIDA.U9.CBO.HR.Department;

	/// <summary>
	/// GetCheckinDataBP partial 
	/// </summary>	
	public partial class GetCheckinDataBP 
	{	
		internal BaseStrategy Select()
		{
			return new GetCheckinDataBPImpementStrategy();	
		}		
	}
	
    //#region  implement strategy	
	/// <summary>
	/// Impement Implement
	/// 
	/// </summary>	
	internal partial class GetCheckinDataBPImpementStrategy : BaseStrategy
	{
		public GetCheckinDataBPImpementStrategy() { }

		public override object Do(object obj)
		{						
			GetCheckinDataBP bpObj = (GetCheckinDataBP)obj;
			
			//get business operation context is as follows
			//IContext context = ContextManager.Context	
			
			//auto generating code end,underside is user custom code
			//and if you Implement replace this Exception Code...
            //throw new NotImplementedException();

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
                        Dictionary<long, List<CheckInDTO>> dicEmployee2Checkin = new Dictionary<long, List<CheckInDTO>>();

                        HBH.DoNet.DevPlatform.U9Mapping.ProcedureMapping procMapping = new HBH.DoNet.DevPlatform.U9Mapping.ProcedureMapping();
                        procMapping.ProcedureName = "HBH_SP_JianLiYuan_GetAllCheckIn";
                        procMapping.Params = new List<HBH.DoNet.DevPlatform.U9Mapping.ParamDTO>();

                        {
                            HBH.DoNet.DevPlatform.U9Mapping.ParamDTO suptParam = new HBH.DoNet.DevPlatform.U9Mapping.ParamDTO();
                            suptParam.ParamName = "PayrollDoc";
                            suptParam.ParamType = System.Data.DbType.Int64;
                            //suptParam.ParamDirection = ParameterDirection.Input;
                            //suptParam.ParamValue = bpObj.PayrollCalculate.ID;
                            //suptParam.ParamValue = payrollCalculate.ID;
                            suptParam.ParamValue = -1;
                            procMapping.Params.Add(suptParam);
                        }

                        {
                            HBH.DoNet.DevPlatform.U9Mapping.ParamDTO suptParam = new HBH.DoNet.DevPlatform.U9Mapping.ParamDTO();
                            suptParam.ParamName = "PayrollCalculate";
                            suptParam.ParamType = System.Data.DbType.AnsiString;
                            //suptParam.ParamDirection = ParameterDirection.Input;
                            //suptParam.ParamValue = bpObj.PayrollCalculate.ID;
                            suptParam.ParamValue = id;
                            procMapping.Params.Add(suptParam);
                        }


                        U9.VOB.HBHCommon.Proxy.U9CommonSVProxy proxy = new U9.VOB.HBHCommon.Proxy.U9CommonSVProxy();

                        // "HBH.DoNet.DevPlatform.U9Mapping.ProcedureMapping";
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
                                                    CheckInDTO dto = CheckInDTO.GetFromDataRow(row);

                                                    if (dto != null
                                                        && dto.EmployeeArchive > 0
                                                        )
                                                    {
                                                        if (!dicEmployee2Checkin.ContainsKey(dto.EmployeeArchive))
                                                        {
                                                            dicEmployee2Checkin.Add(dto.EmployeeArchive, new List<CheckInDTO>());
                                                        }

                                                        dicEmployee2Checkin[dto.EmployeeArchive].Add(dto);
                                                    }
                                                }
                                            }
                                        }

                                    }



                                    //PayrollDoc payHead = PayrollDoc.Finder.FindByID(payrollHead);

                                    //if (payHead != null
                                    //    && payHead.EmpPayrolls != null
                                    //    && payHead.EmpPayrolls.Count > 0
                                    //    )

                                    StringBuilder sbOpath = new StringBuilder();
                                    sbOpath.Append("PayrollCaculate=@CalcID and (1=1 ");

                                    //if (bpObj.Depts != null
                                    //    && bpObj.Depts.Count > 0
                                    //    )
                                    //{
                                    //    //StringBuilder sbDepts = new StringBuilder();
                                    //    //foreach (Department.EntityKey deptKey in bpObj.Depts)
                                    //    //{
                                    //    //    if (deptKey != null
                                    //    //        && deptKey.ID > 0
                                    //    //        )
                                    //    //    {
                                    //    //        sbDepts.Append(deptKey.ID).Append(",");
                                    //    //    }
                                    //    //}

                                    //    //if (sbDepts.Length > 0)
                                    //    //{
                                    //    //    sbOpath.Append(string.Format(" and Department in ({0})"
                                    //    //        , sbDepts.GetStringRemoveLastSplit()
                                    //    //        ));
                                    //    //}

                                    //    //string strOpath = bpObj.Depts.GetListOpath<Department.EntityKey>();
                                    //    string strOpath = bpObj.Depts.GetListOpath();

                                    //    if (!string.IsNullOrWhiteSpace(strOpath))
                                    //    {
                                    //        sbOpath.Append(string.Format(" and Department in ({0})"
                                    //            , strOpath
                                    //            ));
                                    //    }
                                    //}

                                    //if (bpObj.Employee != null
                                    //    && bpObj.Employee.Count > 0
                                    //    )
                                    //{
                                    //    //ExtendMethod.GetListOpath(bpObj.Employee);
                                    //    //string strOpath = bpObj.Employee.GetListOpath<EmployeeArchive.EntityKey>();
                                    //    string strOpath = bpObj.Employee.GetListOpath();

                                    //    if (!string.IsNullOrWhiteSpace(strOpath))
                                    //    {
                                    //        sbOpath.Append(string.Format(" and Employee in ({0})"
                                    //            , strOpath
                                    //            ));
                                    //    }
                                    //}

                                    //if (bpObj.PayrollResults != null
                                    //    && bpObj.PayrollResults.Count > 0
                                    //    )
                                    //{
                                    //    //ExtendMethod.GetListOpath(bpObj.Employee);
                                    //    //string strOpath = bpObj.PayrollResults.GetListOpath<PayrollResult.EntityKey>();
                                    //    string strOpath = bpObj.PayrollResults.GetListOpath();

                                    //    if (!string.IsNullOrWhiteSpace(strOpath))
                                    //    {
                                    //        sbOpath.Append(string.Format(" and ID in ({0})"
                                    //            , strOpath
                                    //            ));
                                    //    }
                                    //}

                                    sbOpath.Append(" ) ");
                                    PayrollResult.EntityList payResultList = PayrollResult.Finder.FindAll(sbOpath.ToString()
                                        , new OqlParam(id)
                                        );

                                    if (payResultList != null
                                        && payResultList.Count > 0
                                        )
                                    {
                                        //SetSalaryItem_Old(dicEmployee2Checkin, payResultList);  

                                        SetSalaryItem_New(dicEmployee2Checkin, payResultList);  
                                    }

                                    // this.Action.NavigateAction.Refresh(null);
                                }
                                else
                                {
                                    //U9.VOB.Cus.HBHShangLuo.HBHShangLuoUIsll.WebPart.HBHUIHelper.ShowErrorInfo(this, result.Message);
                                    //this.Model.ErrorMessage.Message = result.Message;
                                    throw new BusinessException(result.Message);
                                }
                            }
                            else
                            {
                                //U9.VOB.Cus.HBHShangLuo.HBHShangLuoUIsll.WebPart.HBHUIHelper.ShowErrorInfo(this, "执行异常,无返回结果!");
                                //this.Model.ErrorMessage.Message = "执行异常,无返回结果!";
                                throw new BusinessException("客开程序执行异常,无返回结果!");
                            }
                        }
                    }
                }
            }


            return null;
		}


        private static void SetSalaryItem_New(Dictionary<long, List<CheckInDTO>> dicEmployee2Checkin, PayrollResult.EntityList payResultList)
        {
            // UFIDA.U9.PAY.PayrollDoc.PayrollDocItem
            // 薪资项目  SalaryItem       

            /*
            全日制员工出勤薪资取数规则：出勤天数=∑全日制员工出勤
            工时=∑钟点工出勤
            非全日制员工出勤薪资取数规则：F考勤工时=∑非全日制员工出勤+∑钟点工出勤
            非全日制出勤、全日制出勤、出勤天数均保留一位小数，其中钟点工出勤、非全日制出勤不能大于4.
             */

            using (ISession session = Session.Open())
            {
                bool isSeted = false;

                //foreach (EmpPayroll line in payHead.EmpPayrolls)
                foreach (PayrollResult line in payResultList)
                {
                    if (line != null
                        && line.EmployeeKey != null
                        )
                    {
                        long employeeID = line.EmployeeKey.ID;

                        if (employeeID > 0
                            && dicEmployee2Checkin.ContainsKey(employeeID)
                            )
                        {
                            //SalaryItem _checkinItem = SalaryItemHelper.SalaryItem_FirstDept;
                            //SalaryItem _afterDeptItem = null;
                            //SalaryItem _beforeDeptItem = null;
                            //SalaryItem _transferDayItem = null;
                            //SalaryItem _workHoursItem = null;
                            //decimal checkInDays = 0;
                            //decimal fullCheckInDays = 0;
                            //decimal workHours = 0;
                            //decimal fPartCheckInDays = 0;
                            //decimal transferDays = 0;
                            ////long deptID = -1;

                            List<CheckInDTO> lstDTO = dicEmployee2Checkin[employeeID];

                            if (lstDTO != null
                                && lstDTO.Count > 0
                                )
                            {

                                /*
                                全日制考勤取数规则：
                                调动前部门：取计薪期间内第一天所在部门  编码：092  名称：调动前部门
                                调动后部门：取计薪期间内调动后部门    编码：093  名称：调动后部门
                                调动天数：取计薪期间内调动后部门出勤天数   编码：094 名称：调动天数
                                非全日制考勤取数规则：
                                F调动前部门：取计薪期间内第一天所在部门 编码：F35  名称：F调动前部门
                                F调动后部门：取计薪期间内调动后部门    编码：F36  名称：F调动后部门
                                F调动天数：取计薪期间内调动后部门出勤天数   编码：F37 名称：F调动天数

                                                
                                全日制员工出勤薪资取数规则：出勤天数=∑全日制员工出勤
            工时=∑钟点工出勤
非全日制员工出勤薪资取数规则：F考勤工时=∑非全日制员工出勤+∑钟点工出勤
非全日制出勤、全日制出勤、出勤天数均保留一位小数，其中钟点工出勤、非全日制出勤不能大于4.
 
                                 */

                                decimal allCheckInDay = GetAllCheckInDay(lstDTO);
                                SalaryItem _allCheckinItem = SalaryItemHelper.SalaryItem_CheckInDays;
                                if (_allCheckinItem != null)
                                {
                                    line.SetSalaryItem(_allCheckinItem, allCheckInDay);
                                    isSeted = true;
                                }

                                {
                                    CheckInDTO firstCheckIn = lstDTO[0];

                                    if (firstCheckIn != null)
                                    {
                                        //GetFirstCheckinItem(checkinItem, beforeDeptItem, afterDeptItem, fbeforeDeptItem, fafterDeptItem, transferDayItem, workHoursItem, ftransferDayItem, fworkHoursItem, firstCheckIn, out _checkinItem, out _afterDeptItem, out _beforeDeptItem, out _transferDayItem, out _workHoursItem, ref checkInDays, ref fullCheckInDays, ref workHours, ref fPartCheckInDays, ref transferDays);

                                        //SetSalaryValue(line, _checkinItem, _afterDeptItem, _beforeDeptItem, null, null, _transferDayItem, _workHoursItem, checkInDays, firstCheckIn.Department, -1, workHours, transferDays);

                                        // checkInDTO.CheckType == CheckTypeEnum.FullTimeStaff.Value ? beforeDeptItem : fbeforeDeptItem
                                        if (firstCheckIn.CheckType == CheckTypeEnum.FullTimeStaff.Value)
                                        {
                                            SetSalaryValue(line, firstCheckIn, SalaryItemHelper.SalaryItem_FirstDept, null, SalaryItemHelper.SalaryItem_FirstDeptWorkHours);
                                        }
                                        else
                                        {
                                            SetSalaryValue(line, firstCheckIn, SalaryItemHelper.SalaryItem_FFirstDept, null,null);
                                        }
                                    }
                                }

                                // 如果有多个，那么，调动前部门   =   第一个部门
                                if (lstDTO.Count > 1)
                                {
                                    // 调动后部门   =   最后一个部门
                                    CheckInDTO secondCheckIn = lstDTO[1];

                                    if (secondCheckIn != null)
                                    {
                                        // checkInDTO.CheckType == CheckTypeEnum.FullTimeStaff.Value ? beforeDeptItem : fbeforeDeptItem
                                        if (secondCheckIn.CheckType == CheckTypeEnum.FullTimeStaff.Value)
                                        {
                                            SetSalaryValue(line, secondCheckIn, SalaryItemHelper.SalaryItem_SecondDept, SalaryItemHelper.SalaryItem_SecondDeptDays, SalaryItemHelper.SalaryItem_SecondDeptWorkHours);
                                        }
                                        else
                                        {
                                            SetSalaryValue(line, secondCheckIn, SalaryItemHelper.SalaryItem_FSecondDept, SalaryItemHelper.SalaryItem_FSecondDeptDays,null);
                                        }
                                    }
                                }

                                // 如果有多个，那么，调动前部门   =   第一个部门
                                if (lstDTO.Count > 2)
                                {
                                    // 调动后部门   =   最后一个部门
                                    CheckInDTO thirdCheckIn = lstDTO[2];

                                    if (thirdCheckIn != null)
                                    {

                                        // checkInDTO.CheckType == CheckTypeEnum.FullTimeStaff.Value ? beforeDeptItem : fbeforeDeptItem
                                        if (thirdCheckIn.CheckType == CheckTypeEnum.FullTimeStaff.Value)
                                        {
                                            SetSalaryValue(line, thirdCheckIn, SalaryItemHelper.SalaryItem_ThirdDept, SalaryItemHelper.SalaryItem_ThirdDeptDays,SalaryItemHelper.SalaryItem_ThirdDeptWorkHours);
                                        }
                                        else
                                        {
                                            SetSalaryValue(line, thirdCheckIn, SalaryItemHelper.SalaryItem_FThirdDept, SalaryItemHelper.SalaryItem_FThirdDeptDays,null);
                                        }
                                    }
                                }
                            }
                        }
                        // 如果不存在，那么不覆盖已有的手工录入的记录
                        else
                        {
                            //SetSalaryValue(line, checkinItem, afterDeptItem, beforeDeptItem, fafterDeptItem, fbeforeDeptItem, transferDayItem, workHoursItem, 0, -1, -1, 0, 0);
                        }
                    }
                }

                session.Commit();
            }
        }

        private static void SetSalaryValue(PayrollResult line, CheckInDTO checkInDTO, SalaryItem deptSalary, SalaryItem daysSalary, SalaryItem workHoursSalary)
        {
            if (line != null
                && checkInDTO != null
                )
            {
                bool isSeted = false;

                if (deptSalary != null)
                {
                    Department dept = null;
                    long deptID = checkInDTO.Department;
                    if (deptID > 0)
                    {
                        dept = Department.Finder.FindByID(deptID);
                    }
                    if (dept != null)
                    {
                        line.SetSalaryItem(deptSalary, dept.Name);
                        isSeted = true;
                    }
                    else
                    {
                        line.SetSalaryItem(deptSalary, string.Empty);
                        isSeted = true;
                    }
                }
                if (daysSalary != null)
                {
                    line.SetSalaryItem(daysSalary, checkInDTO.Days);
                    isSeted = true;
                }
                if (workHoursSalary != null)
                {
                    line.SetSalaryItem(workHoursSalary, checkInDTO.HourlyDay);
                    isSeted = true;
                }
            }
        }


        private static void SetSalaryItem_Old(Dictionary<long, List<CheckInDTO>> dicEmployee2Checkin, PayrollResult.EntityList payResultList)
        {
            // UFIDA.U9.PAY.PayrollDoc.PayrollDocItem
            // 薪资项目  SalaryItem       

            /*
            全日制员工出勤薪资取数规则：出勤天数=∑全日制员工出勤
            工时=∑钟点工出勤
            非全日制员工出勤薪资取数规则：F考勤工时=∑非全日制员工出勤+∑钟点工出勤
            非全日制出勤、全日制出勤、出勤天数均保留一位小数，其中钟点工出勤、非全日制出勤不能大于4.
             */
            SalaryItem checkinItem = SalaryItem.Finder.Find("Code=@Code"
                , new OqlParam(SalaryItemHelper.SalaryItemCode_CheckInDays)
                );

            SalaryItem beforeDeptItem = SalaryItem.Finder.Find("Code=@Code"
                , new OqlParam(SalaryItemHelper.SalaryItemCode_BeforeDept)
                );

            SalaryItem afterDeptItem = SalaryItem.Finder.Find("Code=@Code"
                , new OqlParam(SalaryItemHelper.SalaryItemCode_AfterDept)
                );

            SalaryItem transferDayItem = SalaryItem.Finder.Find("Code=@Code"
                , new OqlParam(SalaryItemHelper.SalaryItemCode_TransferDays)
                );

            SalaryItem workHoursItem = SalaryItem.Finder.Find("Code=@Code"
                , new OqlParam(SalaryItemHelper.SalaryItemCode_WorkHours)
                );

            SalaryItem fbeforeDeptItem = SalaryItem.Finder.Find("Code=@Code"
                , new OqlParam(SalaryItemHelper.SalaryItemCode_FBeforeDept)
                );

            SalaryItem fafterDeptItem = SalaryItem.Finder.Find("Code=@Code"
                , new OqlParam(SalaryItemHelper.SalaryItemCode_FAfterDept)
                );

            SalaryItem ftransferDayItem = SalaryItem.Finder.Find("Code=@Code"
                , new OqlParam(SalaryItemHelper.SalaryItemCode_FTransferDays)
                );

            SalaryItem fworkHoursItem = SalaryItem.Finder.Find("Code=@Code"
                , new OqlParam(SalaryItemHelper.SalaryItemCode_FWorkHours)
                );


            //salaryItem.Code

            using (ISession session = Session.Open())
            {
                //foreach (EmpPayroll line in payHead.EmpPayrolls)
                foreach (PayrollResult line in payResultList)
                {
                    if (line != null
                        && line.EmployeeKey != null
                        )
                    {
                        long employeeID = line.EmployeeKey.ID;

                        if (employeeID > 0
                            && dicEmployee2Checkin.ContainsKey(employeeID)
                            )
                        {
                            SalaryItem _checkinItem = checkinItem;
                            SalaryItem _afterDeptItem = null;
                            SalaryItem _beforeDeptItem = null;
                            SalaryItem _transferDayItem = null;
                            SalaryItem _workHoursItem = null;
                            decimal checkInDays = 0;
                            decimal fullCheckInDays = 0;
                            decimal workHours = 0;
                            decimal fPartCheckInDays = 0;
                            decimal transferDays = 0;
                            //long deptID = -1;

                            List<CheckInDTO> lstDTO = dicEmployee2Checkin[employeeID];

                            if (lstDTO != null
                                && lstDTO.Count > 0
                                )
                            {

                                /*
                                全日制考勤取数规则：
                                调动前部门：取计薪期间内第一天所在部门  编码：092  名称：调动前部门
                                调动后部门：取计薪期间内调动后部门    编码：093  名称：调动后部门
                                调动天数：取计薪期间内调动后部门出勤天数   编码：094 名称：调动天数
                                非全日制考勤取数规则：
                                F调动前部门：取计薪期间内第一天所在部门 编码：F35  名称：F调动前部门
                                F调动后部门：取计薪期间内调动后部门    编码：F36  名称：F调动后部门
                                F调动天数：取计薪期间内调动后部门出勤天数   编码：F37 名称：F调动天数

                                                
                                全日制员工出勤薪资取数规则：出勤天数=∑全日制员工出勤
            工时=∑钟点工出勤
非全日制员工出勤薪资取数规则：F考勤工时=∑非全日制员工出勤+∑钟点工出勤
非全日制出勤、全日制出勤、出勤天数均保留一位小数，其中钟点工出勤、非全日制出勤不能大于4.
 
                                 */

                                {
                                    CheckInDTO firstCheckIn = lstDTO[0];

                                    if (firstCheckIn != null)
                                    {
                                        GetFirstCheckinItem(checkinItem, beforeDeptItem, afterDeptItem, fbeforeDeptItem, fafterDeptItem, transferDayItem, workHoursItem, ftransferDayItem, fworkHoursItem, firstCheckIn, out _checkinItem, out _afterDeptItem, out _beforeDeptItem, out _transferDayItem, out _workHoursItem, ref checkInDays, ref fullCheckInDays, ref workHours, ref fPartCheckInDays, ref transferDays);

                                        SetSalaryValue(line, _checkinItem, _afterDeptItem, _beforeDeptItem, null, null, _transferDayItem, _workHoursItem, checkInDays, firstCheckIn.Department, -1, workHours, transferDays);
                                    }
                                }

                                // 如果有多个，那么，调动前部门   =   第一个部门
                                if (lstDTO.Count > 1)
                                {
                                    // 调动后部门   =   最后一个部门
                                    CheckInDTO lastCheckIn = lstDTO[lstDTO.Count - 1];

                                    if (lastCheckIn != null)
                                    {
                                        GetLastCheckinItem(checkinItem, beforeDeptItem, afterDeptItem, fbeforeDeptItem, fafterDeptItem, transferDayItem, workHoursItem, ftransferDayItem, fworkHoursItem, lastCheckIn, out _checkinItem, out _afterDeptItem, out _beforeDeptItem, out _transferDayItem, out _workHoursItem, ref checkInDays, ref fullCheckInDays, ref workHours, ref fPartCheckInDays, ref transferDays);

                                        SetSalaryValue(line, _checkinItem, _afterDeptItem, _beforeDeptItem, null, null, _transferDayItem, _workHoursItem, checkInDays, lastCheckIn.Department, -1, workHours, transferDays);
                                    }
                                }
                            }
                        }
                        // 如果不存在，那么不覆盖已有的手工录入的记录
                        else
                        {
                            //SetSalaryValue(line, checkinItem, afterDeptItem, beforeDeptItem, fafterDeptItem, fbeforeDeptItem, transferDayItem, workHoursItem, 0, -1, -1, 0, 0);
                        }
                    }
                }

                session.Commit();
            }
        }

        // 获取所有部门出勤天数
        /// <summary>
        /// 获取所有部门出勤天数
        /// </summary>
        /// <param name="lstDTO"></param>
        /// <returns></returns>
        private static decimal GetAllCheckInDay(List<CheckInDTO> lstDTO)
        {
            decimal all = 0;
            if (lstDTO != null
                && lstDTO.Count > 0
                )
            {
                foreach (CheckInDTO dto in lstDTO)
                {
                    all += dto.FullTimeDay;
                }
            }

            return all;
        }



        public static void SetSalaryValue(EmpPayroll line, SalaryItem _checkinItem, SalaryItem _afterDeptItem, SalaryItem _beforeDeptItem, SalaryItem _afterDeptItem2, SalaryItem _beforeDeptItem2, SalaryItem _transferDayItem, SalaryItem _workHoursItem, decimal checkinDays, long deptID, long deptID2, decimal workHours, decimal transferDays)
        {
            if (_checkinItem != null)
            {
                line.SetSalaryItem(_checkinItem, checkinDays);
            }
            if (_afterDeptItem != null)
            {
                bool isSeted = false;
                if (deptID > 0)
                {
                    Department afterDept = Department.Finder.FindByID(deptID);
                    if (afterDept != null)
                    {
                        line.SetSalaryItem(_afterDeptItem, afterDept.Name);
                        isSeted = true;
                    }
                }

                if (!isSeted)
                {
                    line.SetSalaryItem(_afterDeptItem, string.Empty);
                }
            }
            if (_beforeDeptItem != null)
            {
                bool isSeted = false;
                if (deptID > 0)
                {
                    Department beforeDept = Department.Finder.FindByID(deptID);
                    if (beforeDept != null)
                    {
                        line.SetSalaryItem(_beforeDeptItem, beforeDept.Name);
                        isSeted = true;
                    }
                }

                if (!isSeted)
                {
                    line.SetSalaryItem(_beforeDeptItem, string.Empty);
                }
            }
            if (_afterDeptItem2 != null)
            {
                bool isSeted = false;
                if (deptID2 > 0)
                {
                    Department afterDept = Department.Finder.FindByID(deptID2);
                    if (afterDept != null)
                    {
                        line.SetSalaryItem(_afterDeptItem2, afterDept.Name);
                        isSeted = true;
                    }
                }

                if (!isSeted)
                {
                    line.SetSalaryItem(_afterDeptItem2, string.Empty);
                }
            }
            if (_beforeDeptItem2 != null)
            {
                bool isSeted = false;
                if (deptID2 > 0)
                {
                    Department beforeDept = Department.Finder.FindByID(deptID2);
                    if (beforeDept != null)
                    {
                        line.SetSalaryItem(_beforeDeptItem2, beforeDept.Name);
                        isSeted = true;
                    }
                }

                if (!isSeted)
                {
                    line.SetSalaryItem(_beforeDeptItem2, string.Empty);
                }
            }
            if (_workHoursItem != null)
            {
                line.SetSalaryItem(_workHoursItem, workHours);
            }
            if (_transferDayItem != null)
            {
                line.SetSalaryItem(_transferDayItem, transferDays);
            }
        }

        public static void SetSalaryValue(PayrollResult line, SalaryItem _checkinItem, SalaryItem _afterDeptItem, SalaryItem _beforeDeptItem, SalaryItem _afterDeptItem2, SalaryItem _beforeDeptItem2, SalaryItem _transferDayItem, SalaryItem _workHoursItem, decimal checkinDays, long deptID, long deptID2, decimal workHours, decimal transferDays)
        {
            if (_checkinItem != null)
            {
                line.SetSalaryItem(_checkinItem, checkinDays);
            }
            if (_afterDeptItem != null)
            {
                bool isSeted = false;
                if (deptID > 0)
                {
                    Department afterDept = Department.Finder.FindByID(deptID);
                    if (afterDept != null)
                    {
                        line.SetSalaryItem(_afterDeptItem, afterDept.Name);
                        isSeted = true;
                    }
                }

                if (!isSeted)
                {
                    line.SetSalaryItem(_afterDeptItem, string.Empty);
                }
            }
            if (_beforeDeptItem != null)
            {
                bool isSeted = false;
                if (deptID > 0)
                {
                    Department beforeDept = Department.Finder.FindByID(deptID);
                    if (beforeDept != null)
                    {
                        line.SetSalaryItem(_beforeDeptItem, beforeDept.Name);
                        isSeted = true;
                    }
                }

                if (!isSeted)
                {
                    line.SetSalaryItem(_beforeDeptItem, string.Empty);
                }
            }
            if (_afterDeptItem2 != null)
            {
                bool isSeted = false;
                if (deptID2 > 0)
                {
                    Department afterDept = Department.Finder.FindByID(deptID2);
                    if (afterDept != null)
                    {
                        line.SetSalaryItem(_afterDeptItem2, afterDept.Name);
                        isSeted = true;
                    }
                }

                if (!isSeted)
                {
                    line.SetSalaryItem(_afterDeptItem2, string.Empty);
                }
            }
            if (_beforeDeptItem2 != null)
            {
                bool isSeted = false;
                if (deptID2 > 0)
                {
                    Department beforeDept = Department.Finder.FindByID(deptID2);
                    if (beforeDept != null)
                    {
                        line.SetSalaryItem(_beforeDeptItem2, beforeDept.Name);
                        isSeted = true;
                    }
                }

                if (!isSeted)
                {
                    line.SetSalaryItem(_beforeDeptItem2, string.Empty);
                }
            }
            if (_workHoursItem != null)
            {
                line.SetSalaryItem(_workHoursItem, workHours);
            }
            if (_transferDayItem != null)
            {
                line.SetSalaryItem(_transferDayItem, transferDays);
            }
        }

        public static void GetLastCheckinItem(SalaryItem checkinItem, SalaryItem beforeDeptItem, SalaryItem afterDeptItem, SalaryItem fbeforeDeptItem, SalaryItem fafterDeptItem, SalaryItem transferDayItem, SalaryItem workHoursItem, SalaryItem ftransferDayItem, SalaryItem fworkHoursItem, CheckInDTO checkInDTO, out SalaryItem _checkinItem, out SalaryItem _afterDeptItem, out SalaryItem _beforeDeptItem, out SalaryItem _transferDayItem, out SalaryItem _workHoursItem, ref decimal checkinDays, ref decimal fullCheckInDays, ref decimal workHours, ref decimal fPartCheckInDays, ref decimal transferDays)
        {
            _checkinItem = checkinItem;

            // 最后一条赋值调动后部门，所以调动前部门为空
            _beforeDeptItem = null;
            //_beforeDeptItem = lastCheckIn.CheckType == CheckTypeEnum.FullTimeStaff.Value ? beforeDeptItem : fbeforeDeptItem;
            _afterDeptItem = checkInDTO.CheckType == CheckTypeEnum.FullTimeStaff.Value ? afterDeptItem : fafterDeptItem;
            _transferDayItem = checkInDTO.CheckType == CheckTypeEnum.FullTimeStaff.Value ? transferDayItem : ftransferDayItem;
            _workHoursItem = checkInDTO.CheckType == CheckTypeEnum.FullTimeStaff.Value ? workHoursItem : fworkHoursItem;

            /*
            全日制员工出勤薪资取数规则：出勤天数=∑全日制员工出勤
                        工时=∑钟点工出勤
            非全日制员工出勤薪资取数规则：F考勤工时=∑非全日制员工出勤+∑钟点工出勤
             */
            if (checkInDTO.CheckType == CheckTypeEnum.FullTimeStaff.Value)
            {
                /*
                全日制考勤取数规则：
                调动前部门：取计薪期间内第一天所在部门  编码：092  名称：调动前部门
                调动后部门：取计薪期间内调动后部门    编码：093  名称：调动后部门
                调动天数：取计薪期间内调动后部门出勤天数   编码：094 名称：调动天数
                 */
                fullCheckInDays = checkInDTO.FullTimeDay;
                workHours = checkInDTO.HourlyDay;
                fPartCheckInDays = 0;

                checkinDays += checkInDTO.FullTimeDay;
                // 调动后才有调动天数
                // 调动天数：取计薪期间内调动后部门出勤天数   编码：094 名称：调动天数
                //transferDays = fullCheckInDays + workHours;
                // 调动后才有调动天数 = 全日制出勤 【因为全日制，有加班工时，考勤天数 天和加班工时 小时 单位不同，所以分开单独计算】
                transferDays = fullCheckInDays;
            }
            else
            {
                /*
                非全日制考勤取数规则：
                F调动前部门：取计薪期间内第一天所在部门 编码：F35  名称：F调动前部门
                F调动后部门：取计薪期间内调动后部门    编码：F36  名称：F调动后部门
                F调动天数：取计薪期间内调动后部门出勤天数   编码：F37 名称：F调动天数
                 */
                fullCheckInDays = 0;
                //workHours = 0;
                //workHours = lastCheckIn.HourlyDay;
                fPartCheckInDays = checkInDTO.PartTimeDay + checkInDTO.HourlyDay;

                //checkinDays = lastCheckIn.PartTimeDay;
                //checkinDays += fPartCheckInDays;
                checkinDays = 0;
                // 调动后才有调动天数 = 非全日制 + 钟点工出勤(加班公时)  【因为非全日制F，没有加班工时，全部合并到一起了】
                transferDays = checkInDTO.PartTimeDay + checkInDTO.HourlyDay;

                workHours += fPartCheckInDays;
            }
        }

        public static void GetFirstCheckinItem(SalaryItem checkinItem, SalaryItem beforeDeptItem, SalaryItem afterDeptItem, SalaryItem fbeforeDeptItem, SalaryItem fafterDeptItem, SalaryItem transferDayItem, SalaryItem workHoursItem, SalaryItem ftransferDayItem, SalaryItem fworkHoursItem, CheckInDTO checkInDTO, out SalaryItem _checkinItem, out SalaryItem _afterDeptItem, out SalaryItem _beforeDeptItem, out SalaryItem _transferDayItem, out SalaryItem _workHoursItem, ref decimal checkinDays, ref decimal fullCheckInDays, ref decimal workHours, ref decimal fPartCheckInDays, ref decimal transferDays)
        {
            _checkinItem = checkinItem;

            _beforeDeptItem = checkInDTO.CheckType == CheckTypeEnum.FullTimeStaff.Value ? beforeDeptItem : fbeforeDeptItem;
            // 最后一条赋值调动前部门，所以调动后部门为空
            //_afterDeptItem = lastCheckIn.CheckType == CheckTypeEnum.FullTimeStaff.Value ? afterDeptItem : fafterDeptItem;
            _afterDeptItem = null;
            _transferDayItem = checkInDTO.CheckType == CheckTypeEnum.FullTimeStaff.Value ? transferDayItem : ftransferDayItem;
            _workHoursItem = checkInDTO.CheckType == CheckTypeEnum.FullTimeStaff.Value ? workHoursItem : fworkHoursItem;

            /*
            全日制员工出勤薪资取数规则：出勤天数=∑全日制员工出勤
                        工时=∑钟点工出勤
            非全日制员工出勤薪资取数规则：F考勤工时=∑非全日制员工出勤+∑钟点工出勤
             */
            if (checkInDTO.CheckType == CheckTypeEnum.FullTimeStaff.Value)
            {
                /*
                全日制考勤取数规则：
                调动前部门：取计薪期间内第一天所在部门  编码：092  名称：调动前部门
                调动后部门：取计薪期间内调动后部门    编码：093  名称：调动后部门
                调动天数：取计薪期间内调动后部门出勤天数   编码：094 名称：调动天数
                 */
                fullCheckInDays = checkInDTO.FullTimeDay;
                workHours = checkInDTO.HourlyDay;
                fPartCheckInDays = 0;

                checkinDays += checkInDTO.FullTimeDay;
                // 调动后才有调动天数
                //transferDays = fullCheckInDays + workHours;
                transferDays = 0;
            }
            else
            {
                /*
                非全日制考勤取数规则：
                F调动前部门：取计薪期间内第一天所在部门 编码：F35  名称：F调动前部门
                F调动后部门：取计薪期间内调动后部门    编码：F36  名称：F调动后部门
                F调动天数：取计薪期间内调动后部门出勤天数   编码：F37 名称：F调动天数
                 */
                fullCheckInDays = 0;
                //workHours = 0;
                //workHours = lastCheckIn.HourlyDay;
                fPartCheckInDays = checkInDTO.PartTimeDay + checkInDTO.HourlyDay;

                //checkinDays += lastCheckIn.PartTimeDay;
                //checkinDays += fPartCheckInDays;
                checkinDays = 0;
                //transferDays = fPartCheckInDays;
                // 调动后才有调动天数
                transferDays = 0;

                workHours += fPartCheckInDays;
            }
        }

	}

    //#endregion


    public static class ExtendMethod
    {
        public static object GetValue(this DataRow row, string field)
        {
            if (row != null
                && row.Table != null
                && row.Table.Columns != null
                && row.Table.Columns.Contains(field)
                )
            {
                object objValue = row[field];

                return objValue;
            }

            return null;
        }

        public static void SetSalaryItem(this EmpPayroll rollLine, SalaryItem item, object value)
        {
            if (rollLine != null
                && item != null
                // && value != null
                )
            {
                rollLine.SetValue(item.PayrollField, value);
            }
        }

        public static void SetSalaryItem(this PayrollResult line, SalaryItem item, object value)
        {
            if (line != null
                && item != null
                // && value != null
                )
            {
                line.SetValue(item.PayrollField, value);
            }
        }

        public static string GetListOpath<T>(this List<T> list) where T : BusinessEntity.EntityKey
        {
            StringBuilder sbOpath = new StringBuilder();
            if (list != null
                && list.Count > 0
                )
            {
                foreach (BusinessEntity.EntityKey entityKey in list)
                {
                    if (entityKey != null
                        && entityKey.ID > 0
                        )
                    {
                        sbOpath.Append(entityKey.ID).Append(",");
                    }
                }
            }

            if (sbOpath.Length > 0)
            {
                sbOpath.GetStringRemoveLastSplit();
            }

            return sbOpath.ToString();
        }
    }

    public class CheckInDTO
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

        // 考勤类别
        private int checkType;
        /// <summary>
        /// 考勤类别
        /// </summary>
        public int CheckType
        {
            get { return checkType; }
            set { checkType = value; }
        }

        // 全日制员工出勤
        private decimal fullTimeDay;
        /// <summary>
        /// 全日制员工出勤
        /// </summary>
        public decimal FullTimeDay
        {
            get { return fullTimeDay; }
            set { fullTimeDay = value; }
        }

        // 非全日制员工出勤
        private decimal partTimeDay;
        /// <summary>
        /// 非全日制员工出勤
        /// </summary>
        public decimal PartTimeDay
        {
            get { return partTimeDay; }
            set { partTimeDay = value; }
        }

        // 员工出勤
        private decimal days;
        /// <summary>
        /// 员工出勤
        /// </summary>
        public decimal Days
        {
            get { return days; }
            set { days = value; }
        }


        // 钟点工出勤
        private decimal hourlyDay;
        /// <summary>
        /// 钟点工出勤
        /// </summary>
        public decimal HourlyDay
        {
            get { return hourlyDay; }
            set { hourlyDay = value; }
        }


        public static CheckInDTO GetFromDataRow(DataRow row)
        {
            CheckInDTO dto = new CheckInDTO();

            dto.EmployeeArchive = PubClass.GetLong(row.GetValue("EmployeeArchive"));
            dto.Department = PubClass.GetLong(row.GetValue("Department"));
            dto.CheckType = PubClass.GetInt(row.GetValue("CheckType"));

            dto.FullTimeDay = PubClass.GetDecimal(row.GetValue("FullTimeDay"));
            dto.HourlyDay = PubClass.GetDecimal(row.GetValue("HourlyDay"));
            dto.PartTimeDay = PubClass.GetDecimal(row.GetValue("PartTimeDay"));
            dto.Days = PubClass.GetDecimal(row.GetValue("Days"));

            return dto;
        }

    }
	
}