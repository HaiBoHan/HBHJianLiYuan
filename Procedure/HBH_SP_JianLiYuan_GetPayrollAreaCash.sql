
if exists(select * from sys.objects where Name='HBH_SP_JianLiYuan_GetPayrollAreaCash')
-- 如果存在则删掉
	drop proc HBH_SP_JianLiYuan_GetPayrollAreaCash
go
-- 创建存储过程
create proc HBH_SP_JianLiYuan_GetPayrollAreaCash  (
@PayrollDoc bigint = -1

,@PayrollCalculate bigint = -1
)
with encryption
as
	SET NOCOUNT ON;

	
declare @SysMlFlag varchar(11) = 'zh-CN'


if exists(select name from sys.objects where name = 'HBH_Debug_Param')
begin
	declare @Debugger bit = (select top 1 Debugger from HBH_Debug_Param where ProcName = 'HBH_SP_JianLiYuan_GetPayrollAreaCash' or ProcName is null or ProcName = '' order by ProcName desc)
	if(@Debugger=1)
	begin	
		if not exists(select name from sys.objects where name = 'HBH_SPParamRecord')
		begin
			create table HBH_SPParamRecord
			(ProcName varchar(501)
			,ParamName varchar(501)
			,ParamValue varchar(max)
			,CreatedOn DateTime
			,Memo varchar(max)	-- 备注
			)
		end

		insert into HBH_SPParamRecord
		(ProcName,ParamName,ParamValue,CreatedOn)
		select 'HBH_SP_JianLiYuan_GetPayrollAreaCash','@PayrollDoc',IsNull(cast(@PayrollDoc as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_GetPayrollAreaCash','@PayrollCalculate',IsNull(cast(@PayrollCalculate as varchar(max)),'null'),GETDATE()
		-- select 'HBH_SP_JianLiYuan_GetPayrollAreaCash','@StartDate',IsNull(Convert(varchar,@StartDate,120),'null'),GETDATE()
		-- union select 'HBH_SP_JianLiYuan_GetPayrollAreaCash','@EndDate',IsNull(Convert(varchar,@EndDate,120),'null'),GETDATE()
		--union select 'HBH_SP_JianLiYuan_GetPayrollAreaCash','@IsCalcAll',IsNull(cast(@IsCalcAll as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_GetPayrollAreaCash','ProcSql','exec HBH_SP_JianLiYuan_GetPayrollAreaCash '
				+ IsNull('''' + cast(@PayrollDoc as varchar(501)) + '''' ,'null')
				+ ',' + IsNull('''' + cast(@PayrollCalculate as varchar(501)) + '''' ,'null')
				-- + IsNull('''' + Convert(varchar,@StartDate,120) + '''' ,'null')
				-- + ',' + IsNull('''' + Convert(varchar,@EndDate,120) + '''' ,'null')
				--+ IsNull(cast(@IsCalcAll as varchar(501)),'null') 
			   ,GETDATE()
	end
end



	declare @SysLineNo int = 10
	declare @Now datetime = GetDate();
	declare @CurDate datetime = Convert(date,@Now)
	declare @SheetName varchar(125)
	declare @StartID bigint = -1
	declare @TotalIDCount int = 0
	declare @TotalLineCount int = 0
	declare @DetailLineCount int = 0
	declare @DefaultZero decimal(24,9) = 0
	declare @DefaultEmpty varchar(125) = ''
	declare @DefaultID bigint = -1
	
/*
区域经理，0005003
片区经理，0005004
餐厅经理，0007026
*/
	declare @AreaManager varchar(125) = '0005003'
	declare @ZoneManager varchar(125) = '0005004'
	declare @DiningManager varchar(125) = '0007026'

	
If OBJECT_ID('tempdb..#tmp_hbh_CashCalc') is not null
	Drop Table #tmp_hbh_CashCalc
	

select 
	line.Employee
	
	,StartDate = 
			case when monthPeriod.ID is not null 
				then monthPeriod.StartDate	-- and monthPeriod.EndDate
				when fourWeekPeriod.ID is not null 
				then fourWeekPeriod.StartDate	-- and fourWeekPeriod.EndDate)
				when twoWeekPeriod.ID is not null 
				then twoWeekPeriod.StartDate	-- and twoWeekPeriod.EndDate)
				when weekPeriod.ID is not null 
				then weekPeriod.StartDate	-- and weekPeriod.EndDate)
				when dayPeriod.ID is not null 
				then dayPeriod.StartDate	-- and dayPeriod.EndDate)
				else GetDate() end
	,EndDate = 
			case when monthPeriod.ID is not null 
				then monthPeriod.EndDate	-- and monthPeriod.EndDate
				when fourWeekPeriod.ID is not null 
				then fourWeekPeriod.EndDate	-- and fourWeekPeriod.EndDate)
				when twoWeekPeriod.ID is not null 
				then twoWeekPeriod.EndDate	-- and twoWeekPeriod.EndDate)
				when weekPeriod.ID is not null 
				then weekPeriod.EndDate	-- and weekPeriod.EndDate)
				when dayPeriod.ID is not null 
				then dayPeriod.EndDate	-- and dayPeriod.EndDate)
				else GetDate() end
				
	,Department = @DefaultID
	,DepartmentCode = @DefaultEmpty
	-- 区域 = 部门.全局段2
	,AreaCode = @DefaultEmpty
	-- 职务
	,JobCode = @DefaultEmpty

	--,DepartmentCode = IsNull(dept.Code,'')
	---- 区域 = 部门.全局段2
	--,AreaCode = IsNull(dept.DescFlexField_PrivateDescSeg2,'')
	---- 职务
	--,JobCode = @DefaultEmpty
		

	-- 绩效标准
	,PerformanceStandard = dbo.HBH_Fn_GetDecimal(line.ExtField168,0)

	-- 最终利润 = 部门总利润
	,DeptPerformance = dbo.HBH_Fn_GetDecimal(line.ExtField105,0)
	-- 部门应兑现 = 效益应兑现
	,DeptShouldBeCashed_NoUse = dbo.HBH_Fn_GetDecimal(line.ExtField153,0)

	-- 区域 总利润
	,AreaPerformance = @DefaultZero
	-- 区域应兑现
	,AreaShouldBeCashed = @DefaultZero
	
	
	-- 区域应兑现.区域部分 = 非兼职部门的算法，是 最终利润 * 2%
	,TotalAreaDeptShouldBeCashed = @DefaultZero
	-- 区域应兑现.兼职部门部分 = 最终利润<=0,最终利润*负激励系数 ； 最终利润>0，最终利润*正激励系
	,TotalPartDeptShouldBeCashed = @DefaultZero

	-- 区域应兑现.区域部分 = 非兼职部门的算法，是 最终利润 * 2%
	,AreaDeptShouldBeCashed = dbo.HBH_Fn_GetDecimal(line.ExtField105,0) * 0.02

	-- 区域应兑现.兼职部门部分 = 最终利润<=0,最终利润*负激励系数 ； 最终利润>0，最终利润*正激励系
	,PartDeptShouldBeCashed = @DefaultZero

	-- 正激励系数
	,PlusRatio = dbo.HBH_Fn_GetDecimal(line.ExtField184,0)
	-- 负激励系数
	,MinusRatio = dbo.HBH_Fn_GetDecimal(line.ExtField163,0)

into #tmp_hbh_CashCalc
--from PAY_PayrollDoc head
--	inner join PAY_EmpPayroll line
--	on head.ID = line.PayrollDoc
	
from Pay_PayrollCalculate head

	--left join PAY_PayrollDoc payHead
	--on payCalc.ID = payHead.PayrollCaculate
	--left join PAY_EmpPayroll as payLine	--发薪明细
	--on payHead.ID = payLine.PayrollDoc

	left join PAY_PayrollResult line
	on head.ID = line.PayrollCaculate
	
	left join PAY_PlanPeriod monthPeriod
	on head.PlanPeriodByMonth = monthPeriod.ID
	left join PAY_PlanPeriod fourWeekPeriod
	on head.PlanPeriodByFourWeek = fourWeekPeriod.ID
	left join PAY_PlanPeriod twoWeekPeriod
	on head.PlanPeriodByTwoWeek = twoWeekPeriod.ID
	left join PAY_PlanPeriod weekPeriod
	on head.PlanPeriodByWeek = weekPeriod.ID
	left join PAY_PlanPeriod dayPeriod
	on head.PlanPeriodByDay = dayPeriod.ID
	
	--left join CBO_EmployeeArchive arch
	--on line.Employee = arch.ID

	--left join CBO_Department dept
	--on arch.Dept = dept.ID
	
	left join CBO_Department dept
	on line.Department = dept.ID

where 
	--head.ID = @PayrollDoc
	head.ID = @PayrollCalculate


-- 按照结束日期取有效部门
update #tmp_hbh_CashCalc
set
	Department = IsNull(dept.ID,tmp.Department)
	,DepartmentCode = IsNull(dept.Code,tmp.DepartmentCode)
	-- 区域 = 部门.全局段2
	,AreaCode = IsNull(dept.DescFlexField_PrivateDescSeg2,tmp.AreaCode)
	-- 职务
	,JobCode = job.Code
	
	-- 区域应兑现-兼职部门部门计算 = （最终利润<=0,最终利润*负激励系数+现任部门编码前七位相同的部门的“最终利润”相加（排除兼职部门）*2%，如果（最终利润>0，最终利润*正激励系数）+现任部门编码前七位相同的部门的“最终利润”相加（排除兼职部门）*2%
	,PartDeptShouldBeCashed = case when DeptPerformance <= 0
							then DeptPerformance * MinusRatio
							else DeptPerformance * PlusRatio
							end

from #tmp_hbh_CashCalc tmp
/*
EntityName	DisplayName	DefaultTableName	AssemblyName	UIClassName	UIAssemblyName
UFIDA.U9.CBO.HR.Person.EmployeeAssignment	员工任职记录	CBO_EmployeeAssignment	UFIDA.U9.CBO.HRBE	UFIDA.U9.CBO.HR.PersonInfoUI.PersonInfoMainUIFormWebPart	UFIDA.U9.CBO.HR.PersonInfoUI.WebPart
*/
	inner join CBO_EmployeeAssignment ass
	on tmp.Employee = ass.Employee
		and tmp.EndDate between ass.AssgnBeginDate and ass.AssgnEndDate
		
	inner join CBO_Department dept
	on tmp.Department = dept.ID
	
	/*
Job	任职职务	UFIDA.U9.CBO.HR.Job.Job	CBO_Job
	*/
	inner join CBO_Job job
	on ass.Job = job.ID

where	
/*
区域经理，0005003
片区经理，0005004
*/
	-- job.Code in ('0005003','0005004')
	job.Code in (@AreaManager,@ZoneManager)




-- 建立部门区域绩效表
If OBJECT_ID('tempdb..#tmp_hbh_DeptPerformance') is not null
	Drop Table #tmp_hbh_DeptPerformance
	

select 
	DepartmentCode
	-- 区域 = 部门.全局段2
	,AreaCode
		
	,DeptPerformance = max(IsNull(DeptPerformance,0))
	
	-- 区域应兑现.区域部分 = 非兼职部门的算法，是 最终利润 * 2%
	,AreaDeptShouldBeCashed = max(IsNull(AreaDeptShouldBeCashed,0))
	
	-- 区域应兑现-兼职部门部门计算 = （最终利润<=0,最终利润*负激励系数+现任部门编码前七位相同的部门的“最终利润”相加（排除兼职部门）*2%，如果（最终利润>0，最终利润*正激励系数）+现任部门编码前七位相同的部门的“最终利润”相加（排除兼职部门）*2%
	,PartDeptShouldBeCashed = max(IsNull(PartDeptShouldBeCashed,0))

into #tmp_hbh_DeptPerformance

from #tmp_hbh_CashCalc
group by
	DepartmentCode
	,AreaCode




-- 建立人员兼职部门表
If OBJECT_ID('tempdb..#tmp_hbh_EmployeePartDept') is not null
	Drop Table #tmp_hbh_EmployeePartDept
	
	
select 

	tmp.Employee
	
	,DepartmentCode = IsNull(dept.Code,tmp.DepartmentCode)
	-- 区域 = 部门.全局段2
	,AreaCode = IsNull(dept.DescFlexField_PrivateDescSeg2,tmp.AreaCode)
	-- 职务
	,JobCode = job.Code
	
	-- 部门利润
	,DeptPerformance = (select IsNull(deptPreformance.DeptPerformance,@DefaultZero)
						from #tmp_hbh_DeptPerformance deptPreformance
						where
							deptPreformance.DepartmentCode = tmp.DepartmentCode
						)

	-- 区域应兑现-兼职部门部门计算 = （最终利润<=0,最终利润*负激励系数+现任部门编码前七位相同的部门的“最终利润”相加（排除兼职部门）*2%，如果（最终利润>0，最终利润*正激励系数）+现任部门编码前七位相同的部门的“最终利润”相加（排除兼职部门）*2%
	, PartDeptShouldBeCashed = tmp.PartDeptShouldBeCashed

into #tmp_hbh_EmployeePartDept

from #tmp_hbh_CashCalc tmp
/*
EntityName	DisplayName	DefaultTableName	AssemblyName	UIClassName	UIAssemblyName
UFIDA.U9.CBO.HR.Person.EmployeeAssignment	员工任职记录	CBO_EmployeeAssignment	UFIDA.U9.CBO.HRBE	UFIDA.U9.CBO.HR.PersonInfoUI.PersonInfoMainUIFormWebPart	UFIDA.U9.CBO.HR.PersonInfoUI.WebPart
*/
	inner join CBO_EmployeeAssignment ass
	on tmp.Employee = ass.Employee
		and tmp.EndDate between ass.AssgnBeginDate and ass.AssgnEndDate
		
	inner join CBO_Department dept
	on tmp.Department = dept.ID
	
	/*
Job	任职职务	UFIDA.U9.CBO.HR.Job.Job	CBO_Job
	*/
	inner join CBO_Job job
	on ass.Job = job.ID

where	
	job.Code in (@DiningManager)



---- 计算部门应兑现

--update #tmp_hbh_EmployeePartDept
--set
--	-- 区域应兑现-兼职部门部门计算 = （最终利润<=0,最终利润*负激励系数+现任部门编码前七位相同的部门的“最终利润”相加（排除兼职部门）*2%，如果（最终利润>0，最终利润*正激励系数）+现任部门编码前七位相同的部门的“最终利润”相加（排除兼职部门）*2%
--	DeptShouldBeCashed = case when DeptPerformance <= 0
--							then DeptPerformance * MinusRatio
--							else DeptPerformance * PlusRatio
--							end

--from #tmp_hbh_CashCalc tmp




-- 更新区域绩效 = (区域相同的部门)
update #tmp_hbh_CashCalc
set
	--AreaPerformance = (select 
	--						IsNull(sum(IsNull(DeptPerformance,0)),0)
	--					from #tmp_hbh_DeptPerformance tmp2
	--					where 
	--						-- 区域应兑现=现任部门编码前七位相同的部门的“最终利润”相加*2%
	--						-- 本区域下部门扩展字段区域取值为空时相加，非空取首个员工的值
	--						((tmp.AreaCode is not null
	--							and tmp.AreaCode != ''
	--							)
	--							and tmp.AreaCode = tmp2.AreaCode
	--						)
	--						or ((tmp.AreaCode is null
	--							or tmp.AreaCode = ''
	--							)
	--							and case when len(tmp.DepartmentCode) > 7
	--								then SubString(tmp.DepartmentCode,0,7)
	--								else tmp.DepartmentCode end
	--																= case when len(tmp2.DepartmentCode) > 7
	--																	then SubString(tmp2.DepartmentCode,0,7)
	--																	else tmp2.DepartmentCode end
	--						)
	--					)
	
	
	-- 区域应兑现.区域部分 = 非兼职部门的算法，是 最终利润 * 2%
	TotalAreaDeptShouldBeCashed = IsNull(AreaDeptShouldBeCashed,0)

	-- 区域应兑现.兼职部门部分 = 最终利润<=0,最终利润*负激励系数 ； 最终利润>0，最终利润*正激励系
	,TotalPartDeptShouldBeCashed = (select 
							IsNull(sum(IsNull(tmp2.PartDeptShouldBeCashed,0)),0)
						from #tmp_hbh_EmployeePartDept tmp2
						where 
							-- 区域应兑现=现任部门编码前七位相同的部门的“最终利润”相加*2%
							-- 本区域下部门扩展字段区域取值为空时相加，非空取首个员工的值
							((tmp.AreaCode is not null
								and tmp.AreaCode != ''
								)
								and tmp.AreaCode = tmp2.AreaCode
							)
							or ((tmp.AreaCode is null
								or tmp.AreaCode = ''
								)
								and case when len(tmp.DepartmentCode) > 7
									then SubString(tmp.DepartmentCode,0,7)
									else tmp.DepartmentCode end
																	= case when len(tmp2.DepartmentCode) > 7
																		then SubString(tmp2.DepartmentCode,0,7)
																		else tmp2.DepartmentCode end
							)
						)
	--,TotalPartDeptShouldBeCashed = @DefaultZero
from #tmp_hbh_CashCalc tmp

--where
--	tmp.AreaCode is not null
--	and tmp.AreaCode != ''



-- 区域应兑现
update #tmp_hbh_CashCalc
set
	-- 区域应兑现
	AreaShouldBeCashed = IsNull(TotalAreaDeptShouldBeCashed,0) + IsNull(TotalPartDeptShouldBeCashed,0)
	
from #tmp_hbh_CashCalc tmp



select *
from #tmp_hbh_CashCalc


	