using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Service;
using UFIDA.U9.PAY.PayrollDoc;
using HBH.DoNet.DevPlatform.EntityMapping;
using System.Data;
using UFSoft.UBF.Business;
using UFIDA.U9.PAY.SalaryItem;
using UFSoft.UBF.PL;
using U9.VOB.Cus.HBHJianLiYuan.HBHHelper;
using UFIDA.U9.CBO.HR.Department;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBP
{
    public class CalculateActualPayBP_Plugin : BPSVExtendBase
    {
        public override void BeforeDo(object bp)
        {
            //throw new Exception("The method or operation is not implemented.");
            
            if (!HBHHelper.PubConfig.Const_ThirdHRStage)
            {
                return;
            }

            CalculateActualPayBP bpObj = bp as CalculateActualPayBP;

            if (bpObj != null
                && bpObj.PayrollDoc != null && bpObj.PayrollDoc.ID > 0)
            {
                long payrollHead = bpObj.PayrollDoc.ID;

                Dictionary<long, List<CheckInDTO>> dicEmployee2Checkin = new Dictionary<long, List<CheckInDTO>>();

                HBH.DoNet.DevPlatform.U9Mapping.ProcedureMapping procMapping = new HBH.DoNet.DevPlatform.U9Mapping.ProcedureMapping();
                procMapping.ProcedureName = "HBH_SP_JianLiYuan_GetAllCheckIn";
                procMapping.Params = new List<HBH.DoNet.DevPlatform.U9Mapping.ParamDTO>();

                {
                    HBH.DoNet.DevPlatform.U9Mapping.ParamDTO suptParam = new HBH.DoNet.DevPlatform.U9Mapping.ParamDTO();
                    suptParam.ParamName = "PayrollDoc";
                    suptParam.ParamType = System.Data.DbType.Int64;
                    //suptParam.ParamDirection = ParameterDirection.Input;
                    suptParam.ParamValue = bpObj.PayrollDoc.ID;
                    procMapping.Params.Add(suptParam);
                }

                {
                    HBH.DoNet.DevPlatform.U9Mapping.ParamDTO suptParam = new HBH.DoNet.DevPlatform.U9Mapping.ParamDTO();
                    suptParam.ParamName = "PayrollCalculate";
                    suptParam.ParamType = System.Data.DbType.AnsiString;
                    //suptParam.ParamDirection = ParameterDirection.Input;
                    //suptParam.ParamValue = bpObj.PayrollCalculate.ID;
                    suptParam.ParamValue = -1;
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



                            PayrollDoc payHead = PayrollDoc.Finder.FindByID(payrollHead);

                            if (payHead != null
                                && payHead.EmpPayrolls != null
                                && payHead.EmpPayrolls.Count > 0
                                )
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

                                foreach (EmpPayroll line in payHead.EmpPayrolls)
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
                                                    // 调动后部门   =   最后一个部门
                                                    CheckInDTO lastCheckIn = lstDTO[lstDTO.Count - 1];

                                                    if (lastCheckIn != null)
                                                    {
                                                        PayrollCalculateBP_Plugin.GetLastCheckinItem(checkinItem, beforeDeptItem, afterDeptItem, fbeforeDeptItem, fafterDeptItem, transferDayItem, workHoursItem, ftransferDayItem, fworkHoursItem, lastCheckIn, out _checkinItem, out _afterDeptItem, out _beforeDeptItem, out _transferDayItem, out _workHoursItem, out checkInDays, out fullCheckInDays, out workHours, out fPartCheckInDays, out transferDays);

                                                        PayrollCalculateBP_Plugin.SetSalaryValue(line, _checkinItem, _afterDeptItem, _beforeDeptItem, null, null, _transferDayItem, _workHoursItem, checkInDays, lastCheckIn.Department, -1, workHours, transferDays);
                                                    }
                                                }

                                                // 如果有多个，那么，调动前部门   =   第一个部门
                                                if (lstDTO.Count > 1)
                                                {
                                                    CheckInDTO firstCheckIn = lstDTO[0];

                                                    if (firstCheckIn != null)
                                                    {
                                                        PayrollCalculateBP_Plugin.GetFirstCheckinItem(checkinItem, beforeDeptItem, afterDeptItem, fbeforeDeptItem, fafterDeptItem, transferDayItem, workHoursItem, ftransferDayItem, fworkHoursItem, firstCheckIn, out _checkinItem, out _afterDeptItem, out _beforeDeptItem, out _transferDayItem, out _workHoursItem, out checkInDays, out fullCheckInDays, out workHours, out fPartCheckInDays, out transferDays);

                                                        PayrollCalculateBP_Plugin.SetSalaryValue(line, _checkinItem, _afterDeptItem, _beforeDeptItem, null, null, _transferDayItem, _workHoursItem, checkInDays, firstCheckIn.Department, -1, workHours, transferDays);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            PayrollCalculateBP_Plugin.SetSalaryValue(line, checkinItem, afterDeptItem, beforeDeptItem, fafterDeptItem, fbeforeDeptItem, transferDayItem, workHoursItem, 0, -1, -1, 0, 0);
                                        }
                                    }
                                }
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

        //private static void SetSalaryValue(EmpPayroll line, SalaryItem _checkinItem, SalaryItem _afterDeptItem, SalaryItem _beforeDeptItem, SalaryItem _afterDeptItem2, SalaryItem _beforeDeptItem2, SalaryItem _transferDayItem, SalaryItem _workHoursItem, int checkinDays, long deptID, long deptID2, int workHours, int transferDays)
        //{
        //    if (_checkinItem != null)
        //    {
        //        line.SetSalaryItem(_checkinItem, checkinDays);
        //    }
        //    if (_afterDeptItem != null)
        //    {
        //        if (deptID > 0)
        //        {
        //            Department afterDept = Department.Finder.FindByID(deptID);
        //            line.SetSalaryItem(_afterDeptItem, afterDept);
        //        }
        //        else
        //        {
        //            line.SetSalaryItem(_afterDeptItem, null);
        //        }
        //    }
        //    if (_beforeDeptItem != null)
        //    {
        //        if (deptID > 0)
        //        {
        //            Department beforeDept = Department.Finder.FindByID(deptID);
        //            line.SetSalaryItem(_beforeDeptItem, beforeDept);
        //        }
        //        else
        //        {
        //            line.SetSalaryItem(_beforeDeptItem, null);
        //        }
        //    }
        //    if (_afterDeptItem2 != null)
        //    {
        //        if (deptID2 > 0)
        //        {
        //            Department afterDept = Department.Finder.FindByID(deptID2);
        //            line.SetSalaryItem(_afterDeptItem2, afterDept);
        //        }
        //        else
        //        {
        //            line.SetSalaryItem(_afterDeptItem2, null);
        //        }
        //    }
        //    if (_beforeDeptItem2 != null)
        //    {
        //        if (deptID2 > 0)
        //        {
        //            Department beforeDept = Department.Finder.FindByID(deptID2);
        //            line.SetSalaryItem(_beforeDeptItem2, beforeDept);
        //        }
        //        else
        //        {
        //            line.SetSalaryItem(_beforeDeptItem2, null);
        //        }
        //    }
        //    if (_workHoursItem != null)
        //    {
        //        line.SetSalaryItem(_workHoursItem, workHours);
        //    }
        //    if (_transferDayItem != null)
        //    {
        //        line.SetSalaryItem(_transferDayItem, transferDays);
        //    }
        //}

        //private static void GetLastCheckinItem(SalaryItem checkinItem, SalaryItem beforeDeptItem, SalaryItem afterDeptItem, SalaryItem fbeforeDeptItem, SalaryItem fafterDeptItem, SalaryItem transferDayItem, SalaryItem workHoursItem, SalaryItem ftransferDayItem, SalaryItem fworkHoursItem, CheckInDTO lastCheckIn, out SalaryItem _checkinItem, out SalaryItem _afterDeptItem, out SalaryItem _beforeDeptItem, out SalaryItem _transferDayItem, out SalaryItem _workHoursItem,out int checkinDays, out int fullCheckInDays, out int workHours, out int fPartCheckInDays, out int transferDays)
        //{
        //    _checkinItem = checkinItem;

        //    // 最后一条赋值调动后部门，所以调动前部门为空
        //    _beforeDeptItem = null;
        //    //_beforeDeptItem = lastCheckIn.CheckType == CheckTypeEnum.FullTimeStaff.Value ? beforeDeptItem : fbeforeDeptItem;
        //    _afterDeptItem = lastCheckIn.CheckType == CheckTypeEnum.FullTimeStaff.Value ? afterDeptItem : fafterDeptItem;
        //    _transferDayItem = lastCheckIn.CheckType == CheckTypeEnum.FullTimeStaff.Value ? transferDayItem : ftransferDayItem;
        //    _workHoursItem = lastCheckIn.CheckType == CheckTypeEnum.FullTimeStaff.Value ? workHoursItem : fworkHoursItem;

        //    if (lastCheckIn.CheckType == CheckTypeEnum.FullTimeStaff.Value)
        //    {
        //        fullCheckInDays = lastCheckIn.FullTimeDay;
        //        workHours = lastCheckIn.HourlyDay;
        //        fPartCheckInDays = 0;

        //        checkinDays = lastCheckIn.FullTimeDay;
        //        transferDays = fullCheckInDays + workHours;
        //    }
        //    else
        //    {
        //        fullCheckInDays = 0;
        //        workHours = 0;
        //        //workHours = lastCheckIn.HourlyDay;
        //        fPartCheckInDays = lastCheckIn.PartTimeDay + lastCheckIn.HourlyDay;

        //        checkinDays = lastCheckIn.PartTimeDay;
        //        transferDays = fPartCheckInDays;
        //    }
        //}

        //private static void GetFirstCheckinItem(SalaryItem checkinItem, SalaryItem beforeDeptItem, SalaryItem afterDeptItem, SalaryItem fbeforeDeptItem, SalaryItem fafterDeptItem, SalaryItem transferDayItem, SalaryItem workHoursItem, SalaryItem ftransferDayItem, SalaryItem fworkHoursItem, CheckInDTO lastCheckIn, out SalaryItem _checkinItem, out SalaryItem _afterDeptItem, out SalaryItem _beforeDeptItem, out SalaryItem _transferDayItem, out SalaryItem _workHoursItem, out int checkinDays, out int fullCheckInDays, out int workHours, out int fPartCheckInDays, out int transferDays)
        //{
        //    _checkinItem = checkinItem;

        //    _beforeDeptItem = lastCheckIn.CheckType == CheckTypeEnum.FullTimeStaff.Value ? beforeDeptItem : fbeforeDeptItem;
        //    // 最后一条赋值调动前部门，所以调动后部门为空
        //    //_afterDeptItem = lastCheckIn.CheckType == CheckTypeEnum.FullTimeStaff.Value ? afterDeptItem : fafterDeptItem;
        //    _afterDeptItem = null;
        //    _transferDayItem = lastCheckIn.CheckType == CheckTypeEnum.FullTimeStaff.Value ? transferDayItem : ftransferDayItem;
        //    _workHoursItem = lastCheckIn.CheckType == CheckTypeEnum.FullTimeStaff.Value ? workHoursItem : fworkHoursItem;

        //    if (lastCheckIn.CheckType == CheckTypeEnum.FullTimeStaff.Value)
        //    {
        //        fullCheckInDays = lastCheckIn.FullTimeDay;
        //        workHours = lastCheckIn.HourlyDay;
        //        fPartCheckInDays = 0;

        //        checkinDays = lastCheckIn.FullTimeDay;
        //        transferDays = fullCheckInDays + workHours;
        //    }
        //    else
        //    {
        //        fullCheckInDays = 0;
        //        workHours = 0;
        //        //workHours = lastCheckIn.HourlyDay;
        //        fPartCheckInDays = lastCheckIn.PartTimeDay + lastCheckIn.HourlyDay;

        //        checkinDays = lastCheckIn.PartTimeDay;
        //        transferDays = fPartCheckInDays;

        //    }
        //}

        public override void AfterDo(object bp, ref object result)
        {
            //throw new NotImplementedException();
        }
    }


    /*      计算薪资按钮
    // UFIDA.U9.HR.PAY.PayrollDoc.PayrollDocUIModel.PayrollDocUIModelAction
private void ComputeRealPay_Extend(object sender, UFSoft.UBF.UI.ActionProcess.UIActionEventArgs e)
{
if (this.CurrentModel.ErrorMessage.hasErrorMessage)
{
    this.CurrentModel.ClearErrorMessage();
}
base.CommonAction.Save();
long iD = this.CurrentModel.PayrollDoc.FocusedRecord.PayrollDoc;
CalculateActualPayBPProxy calculateActualPayBPProxy = new CalculateActualPayBPProxy();
calculateActualPayBPProxy.set_PayrollDoc(iD);
calculateActualPayBPProxy.Do();
if (!this.CurrentModel.ErrorMessage.hasErrorMessage)
{
    this.Refresh();
}
}
     * 
     * 
    namespace UFIDA.U9.PAY.PayrollDoc
{
/// <summary>
/// Impement Implement
///
/// </summary>	
internal class CalculateActualPayBPImpementStrategy : BaseStrategy
{
    public override object Do(object obj)
    {
        CalculateActualPayBP calculateActualPayBP = (CalculateActualPayBP)obj;
        object result;
        if (calculateActualPayBP.PayrollDoc == null)
        {
            result = null;
        }
        else
        {
            PayrollDoc entity = calculateActualPayBP.PayrollDoc.GetEntity();
            if (entity.EmpPayrolls == null || entity.EmpPayrolls.get_Count() == 0)
            {
                throw new PaymasterNotExistException();
            }
            if (entity.PayrollDocItems == null || entity.PayrollDocItems.get_Count() == 0)
            {
                throw new PayItemNotExistCannotCalculateException();
            }
            if (entity.EmpPayrolls != null && entity.EmpPayrolls.get_Count() > 0 && entity.PayrollDocItems != null && entity.PayrollDocItems.get_Count() > 0)
            {
                using (Session session = Session.Open())
                {
                    entity.UpdateSalaryValue();
                    entity.UpdateSystemItemValue();
                    session.Commit();
                }
            }
            result = null;
        }
        return result;
    }
}
     */


}
