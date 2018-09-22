
if exists(select * from sys.objects where Name='HBH_SP_JianLiYuan_GetPayrollAreaCash')
-- 如果存在则删掉
	drop proc HBH_SP_JianLiYuan_GetPayrollAreaCash
go

/*
exec HBH_SP_JianLiYuan_GetPayrollAreaCash '-1','1001806010000032'
*/
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
	declare @DefaultBool bit = 0
	declare @DefaultInt int = 0

	
/*
区域经理，0005003
片区经理，0005004
餐厅经理，0007026
*/
	declare @AreaManager varchar(125) = '0005003'
	declare @ZoneManager varchar(125) = '0005004'

	-- 兼职职务			餐厅主管、餐厅副经理、餐厅经理
	-- 餐厅经理
	declare @DiningManager1 varchar(125) = '0007026'
	-- 餐厅副经理
	declare @DiningManager2 varchar(125) = '0008001'
	-- 餐厅主管
	declare @DiningManager3 varchar(125) = '0009024'

	
If OBJECT_ID('tempdb..#tmp_hbh_CashCalc') is not null
	Drop Table #tmp_hbh_CashCalc
	

select 
	Employee = line.Employee
	,EmployeeCode = employee.EmployeeCode
	,EmployeeName = employee.Name
		
	-- 区域管理类型(区域经理1,片区经理2)
	,AreaManagerType = @DefaultInt
	-- 区域应兑现
	,AreaShouldBeCashed = @DefaultZero
	
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
				
	-- 没有职务下面部门会更新为空，这个错误，所以，先把部门取出来
	--,Department = @DefaultID
	--,DepartmentCode = @DefaultEmpty

	-- 2018-09-17 wf 如果录入临时部门、按临时部门取；否则取 行部门
	,Department = IsNull(tmpDept.ID , dept.ID )
	,DepartmentCode = IsNull(tmpDept.Code , dept.Code )
	,DepartmentName = IsNull(tmpDeptTrl.Name , deptTrl.Name )

	-- 区域 = 部门.全局段2
	--,AreaCode = @DefaultEmpty
	-- 1、好多区域是空的; 2、问了下，区域不在这里用，这里用部门编码前7位;
	--,AreaCode =  cast(IsNull(case when len(dept.Code) > 7
	--					then SubString(dept.Code,1,7)
	--					else dept.Code
	--					end
	--					,'') as varchar(125))
	-- 2018-06-07 wf 区域经理，区域=7位；片区经理，区域=10位；
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

	-- 不需要，这里取部门就好了，算出来部门兼职应兑现
	-- 兼职需要找到兼职部门，取兼职部门的扩展字段
	-- 兼职 正激励系数
	-- 2018-09-17 wf 如果录入临时部门、按临时部门取；否则取 行部门
	,PartPlusRatio = dbo.HBH_Fn_GetDecimal(IsNull(dept.DescFlexField_PrivateDescSeg4 
											,tmpDept.DescFlexField_PrivateDescSeg4)
							,0)
	-- 兼职 负激励系数
	,PartMinusRatio = dbo.HBH_Fn_GetDecimal(IsNull(dept.DescFlexField_PrivateDescSeg5
											,tmpDept.DescFlexField_PrivateDescSeg5)
							,0)

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
	on dept.ID = line.Department

	left join CBO_Department_Trl deptTrl
	on deptTrl.ID = dept.ID
		and deptTrl.SysMlFlag = @SysMlFlag
		
	-- 临时部门
	left join CBO_Department tmpDept
	on tmpDept.Code = line.ExtField186
		and tmpDept.Org = head.HROrg

	left join CBO_Department_Trl tmpDeptTrl
	on tmpDeptTrl.ID = tmpDept.ID
		and tmpDeptTrl.SysMlFlag = @SysMlFlag

	left join CBO_EmployeeArchive employee
	on line.Employee = employee.ID

where 
	--head.ID = @PayrollDoc
	head.ID = @PayrollCalculate


-- 按照结束日期取有效部门
update #tmp_hbh_CashCalc
set
	--Department = IsNull(dept.ID,tmp.Department)
	--,DepartmentCode = IsNull(dept.Code,tmp.DepartmentCode)
	-- 区域 = 部门.全局段2
	--,AreaCode = IsNull(dept.DescFlexField_PrivateDescSeg2,tmp.AreaCode)
	-- 职务
	JobCode = job.Code

	-- 区域管理类型(区域经理1,片区经理2)
	,AreaManagerType = 
						-- 区域经理
						case when job.Code = @AreaManager
							then 1
						-- 片区经理
							when job.Code = @ZoneManager
							then 2
							else -1
						end
	
	-- 区域应兑现-兼职部门部门计算 = （最终利润<=0,最终利润*负激励系数+现任部门编码前七位相同的部门的“最终利润”相加（排除兼职部门）*2%，如果（最终利润>0，最终利润*正激励系数）+现任部门编码前七位相同的部门的“最终利润”相加（排除兼职部门）*2%
	,PartDeptShouldBeCashed = case when DeptPerformance <= 0
							then DeptPerformance * MinusRatio
							else DeptPerformance * PlusRatio
							end

from #tmp_hbh_CashCalc tmp
	inner join CBO_Department dept
	on tmp.Department = dept.ID
	
/*
EntityName	DisplayName	DefaultTableName	AssemblyName	UIClassName	UIAssemblyName
UFIDA.U9.CBO.HR.Person.EmployeeAssignment	员工任职记录	CBO_EmployeeAssignment	UFIDA.U9.CBO.HRBE	UFIDA.U9.CBO.HR.PersonInfoUI.PersonInfoMainUIFormWebPart	UFIDA.U9.CBO.HR.PersonInfoUI.WebPart
*/
	inner join CBO_EmployeeAssignment ass
	on tmp.Employee = ass.Employee
		and tmp.Department = ass.Dept
		--and tmp.EndDate between ass.AssgnBeginDate and ass.AssgnEndDate
		and tmp.EndDate between IsNull(ass.AssgnBeginDate,'2000-01-01') and IsNull(ass.AssgnEndDate,'9999-12-31')
		
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



-- 更新区域：  区域经理，区域=7位；片区经理，区域=10位；
update #tmp_hbh_CashCalc
set
	--,AreaCode =  cast(IsNull(case when len(dept.Code) > 7
	--					then SubString(dept.Code,1,7)
	--					else dept.Code
	--					end
	--					,'') as varchar(125))
	-- 2018-06-07 wf 区域经理，区域=7位；片区经理，区域=10位； 区域管理类型(区域经理1,片区经理2)
	AreaCode = cast(IsNull(
					case when AreaManagerType = 1 and len(tmp.DepartmentCode) > 7
						then SubString(tmp.DepartmentCode,1,7)
						when AreaManagerType = 2 and len(tmp.DepartmentCode) > 10
						then SubString(tmp.DepartmentCode,1,10)
						else tmp.DepartmentCode
					end
					,'') as varchar(125))

from #tmp_hbh_CashCalc tmp
where
	AreaManagerType >= 1



-- 建立部门区域绩效表
If OBJECT_ID('tempdb..#tmp_hbh_DeptPerformance') is not null
	Drop Table #tmp_hbh_DeptPerformance
	

select 
	DepartmentCode
	,DepartmentName
	--,AreaCode
		
	--,DeptPerformance = max(IsNull(DeptPerformance,@DefaultZero))
	,DeptPerformance = (select top 1 tmp1.DeptPerformance from #tmp_hbh_CashCalc tmp1 
				where tmp1.DepartmentCode = #tmp_hbh_CashCalc.DepartmentCode
				order by abs(tmp1.DeptPerformance)
				)
	
	-- 区域应兑现.区域部分 = 非兼职部门的算法，是 最终利润 * 2%
	-- ,AreaDeptShouldBeCashed = max(IsNull(AreaDeptShouldBeCashed,0))
	,AreaDeptShouldBeCashed = (select top 1 tmp1.AreaDeptShouldBeCashed from #tmp_hbh_CashCalc tmp1 
				where tmp1.DepartmentCode = #tmp_hbh_CashCalc.DepartmentCode
				order by abs(tmp1.AreaDeptShouldBeCashed)
				)
	
	-- 区域应兑现-兼职部门部门计算 = （最终利润<=0,最终利润*负激励系数+现任部门编码前七位相同的部门的“最终利润”相加（排除兼职部门）*2%，如果（最终利润>0，最终利润*正激励系数）+现任部门编码前七位相同的部门的“最终利润”相加（排除兼职部门）*2%
	-- ,PartDeptShouldBeCashed = max(IsNull(PartDeptShouldBeCashed,0))
	,PartDeptShouldBeCashed = @DefaultZero
	
	-- 正激励系数
	,PlusRatio = (select top 1 tmp1.PlusRatio from #tmp_hbh_CashCalc tmp1 
				where tmp1.DepartmentCode = #tmp_hbh_CashCalc.DepartmentCode
				order by abs(tmp1.PlusRatio)
				)
	-- 负激励系数
	,MinusRatio = (select top 1 tmp1.MinusRatio from #tmp_hbh_CashCalc tmp1 
				where tmp1.DepartmentCode = #tmp_hbh_CashCalc.DepartmentCode
				order by abs(tmp1.MinusRatio)
				)
				
				
	-- 兼职需要找到兼职部门，取兼职部门的扩展字段
	-- 2018-08-31 wf 李震林，部门上增加了 兼职 正、负 激励系数
	-- 兼职 正激励系数
	,PartPlusRatio = PartPlusRatio
	-- 兼职 负激励系数
	,PartMinusRatio = PartMinusRatio


into #tmp_hbh_DeptPerformance

from #tmp_hbh_CashCalc 
group by
	DepartmentCode
	,DepartmentName
	--,AreaCode

	-- 兼职 正激励系数
	,PartPlusRatio
	-- 兼职 负激励系数
	,PartMinusRatio



update #tmp_hbh_DeptPerformance
set	
	-- 区域应兑现-兼职部门部门计算 = （最终利润<=0,最终利润*负激励系数+现任部门编码前七位相同的部门的“最终利润”相加（排除兼职部门）*2%，如果（最终利润>0，最终利润*正激励系数）+现任部门编码前七位相同的部门的“最终利润”相加（排除兼职部门）*2%
	PartDeptShouldBeCashed = case when DeptPerformance < 0
							--then DeptPerformance * MinusRatio
							--else DeptPerformance * PlusRatio
							then DeptPerformance * PartMinusRatio
							else DeptPerformance * PartPlusRatio
							end



-- 建立人员兼职部门表
If OBJECT_ID('tempdb..#tmp_hbh_EmployeePartDept') is not null
	Drop Table #tmp_hbh_EmployeePartDept
	
	
select 

	tmp.Employee
	,tmp.EmployeeCode
	,tmp.EmployeeName
	
	,DepartmentCode = dept.Code
	,DepartmentName = deptTrl.Name
	-- 区域 = 部门.全局段2
	--,AreaCode = dept.DescFlexField_PrivateDescSeg2
	-- 1、好多区域是空的; 2、问了下，区域不在这里用，这里用部门编码前7位;
	--,AreaCode =  cast(IsNull(case when len(dept.Code) > 7
	--					then SubString(dept.Code,1,7)
	--					else dept.Code
	--					end
	--					,'') as varchar(125))
	-- 部门不能有区域，要按照人员做区域匹配
	-- ,AreaCode = @DefaultString

	-- 职务
	,JobCode = job.Code
	
	---- 部门利润
	--,DeptPerformance = (select IsNull(deptPreformance.DeptPerformance,@DefaultZero)
	--					from #tmp_hbh_DeptPerformance deptPreformance
	--					where
	--						deptPreformance.DepartmentCode = tmp.DepartmentCode
	--					)

	---- 区域应兑现-兼职部门部门计算 = （最终利润<=0,最终利润*负激励系数+现任部门编码前七位相同的部门的“最终利润”相加（排除兼职部门）*2%，如果（最终利润>0，最终利润*正激励系数）+现任部门编码前七位相同的部门的“最终利润”相加（排除兼职部门）*2%
	--, PartDeptShouldBeCashed = tmp.PartDeptShouldBeCashed
	
	
	-- 部门利润
	,DeptPerformance = IsNull(deptPreformance.DeptPerformance,@DefaultZero)

	-- 区域应兑现-兼职部门部门计算 = （最终利润<=0,最终利润*负激励系数+现任部门编码前七位相同的部门的“最终利润”相加（排除兼职部门）*2%，如果（最终利润>0，最终利润*正激励系数）+现任部门编码前七位相同的部门的“最终利润”相加（排除兼职部门）*2%
	--, PartDeptShouldBeCashed = IsNull(deptPreformance.PartDeptShouldBeCashed,@DefaultZero)
	, PartDeptShouldBeCashed = IsNull(deptPreformance.PartDeptShouldBeCashed,@DefaultZero)
	
	
	-- 兼职 正激励系数
	,PartPlusRatio = IsNull(deptPreformance.PartPlusRatio,@DefaultZero)
	-- 兼职 负激励系数
	,PartMinusRatio = IsNull(deptPreformance.PartMinusRatio,@DefaultZero)

into #tmp_hbh_EmployeePartDept

from #tmp_hbh_CashCalc tmp
/*
EntityName	DisplayName	DefaultTableName	AssemblyName	UIClassName	UIAssemblyName
UFIDA.U9.CBO.HR.Person.EmployeeAssignment	员工任职记录	CBO_EmployeeAssignment	UFIDA.U9.CBO.HRBE	UFIDA.U9.CBO.HR.PersonInfoUI.PersonInfoMainUIFormWebPart	UFIDA.U9.CBO.HR.PersonInfoUI.WebPart
*/
	inner join CBO_EmployeeAssignment ass
	on tmp.Employee = ass.Employee
		and tmp.Department != ass.Dept
		and tmp.EndDate between IsNull(ass.AssgnBeginDate,'2000-01-01') and IsNull(ass.AssgnEndDate,'9999-12-31')
		
	inner join CBO_Department dept
	on ass.Dept = dept.ID

	left join CBO_Department_Trl deptTrl
	on deptTrl.ID = dept.ID
		and deptTrl.SysMlFlag = @SysMlFlag
	
	/*
Job	任职职务	UFIDA.U9.CBO.HR.Job.Job	CBO_Job
	*/
	inner join CBO_Job job
	on ass.Job = job.ID

	left join #tmp_hbh_DeptPerformance deptPreformance
	on dept.Code = deptPreformance.DepartmentCode

where	
	job.Code in (@DiningManager1,@DiningManager2,@DiningManager3)



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
	
	-- 区域管理人员，才算区域绩效、及区域应兑现的区域部分
	-- 区域管理类型(区域经理1,片区经理2)
	AreaPerformance = case when AreaManagerType >= 1
						then (select 
								IsNull(sum(IsNull(DeptPerformance,0)),0)
							from #tmp_hbh_DeptPerformance tmp2
							where tmp2.DepartmentCode like tmp.AreaCode + '%'
								-- 没有兼职的部门（有兼职按兼职算法）
								and tmp2.DepartmentCode not in (
											select tmp3.DepartmentCode
											from #tmp_hbh_EmployeePartDept tmp3
											where tmp.EmployeeCode = tmp3.EmployeeCode
											)
							)
						else @DefaultZero
						end
	
	-- 区域应兑现.区域部分 = 非兼职部门的算法，是 最终利润 * 2%
	--TotalAreaDeptShouldBeCashed = IsNull(AreaDeptShouldBeCashed,0)

	-- 区域应兑现.兼职部门部分 = 最终利润<=0,最终利润*负激励系数 ； 最终利润>0，最终利润*正激励系
	--,TotalPartDeptShouldBeCashed = (select 
	--						IsNull(sum(IsNull(tmp2.PartDeptShouldBeCashed,0)),0)
	--					from #tmp_hbh_EmployeePartDept tmp2
	--					where 
	--						-- 区域应兑现=现任部门编码前七位相同的部门的“最终利润”相加*2%
	--						-- 本区域下部门扩展字段区域取值为空时相加，非空取首个员工的值
	--						-- 1、好多区域是空的; 2、问了下，区域不在这里用，这里用部门编码前7位;
	--						--((tmp.AreaCode is not null
	--						--	and tmp.AreaCode != ''
	--						--	)
	--						--	and tmp.AreaCode = tmp2.AreaCode
	--						--)
	--						--or ((tmp.AreaCode is null
	--						--	or tmp.AreaCode = ''
	--						--	)
	--						--	and case when len(tmp.DepartmentCode) > 7
	--						--		then SubString(tmp.DepartmentCode,0,7)
	--						--		else tmp.DepartmentCode end
	--						--										= case when len(tmp2.DepartmentCode) > 7
	--						--											then SubString(tmp2.DepartmentCode,0,7)
	--						--											else tmp2.DepartmentCode end
	--						--)
	--						1=1
	--						and tmp.EmployeeCode = tmp2.EmployeeCode
	--					)
	,TotalPartDeptShouldBeCashed = (select 
							IsNull(sum(
										--IsNull(tmp2.DeptPerformance,0)
										case when IsNull(tmp2.DeptPerformance,0) <= 0
											--then tmp2.DeptPerformance * IsNull(tmpEmployee.PartMinusRatio,0)
											-- 这里取部门的激励系数
											then tmp2.DeptPerformance * IsNull(tmp2.PartMinusRatio,0)
											--else tmp2.DeptPerformance * IsNull(tmpEmployee.PartPlusRatio,0)
											-- 这里取部门的激励系数
											else tmp2.DeptPerformance * IsNull(tmp2.PartPlusRatio,0)
										end
										),0)
						from #tmp_hbh_EmployeePartDept tmp2
							inner join #tmp_hbh_CashCalc tmpEmployee
							on tmpEmployee.EmployeeCode = tmp2.EmployeeCode
						where 
							-- 区域应兑现=现任部门编码前七位相同的部门的“最终利润”相加*2%
							-- 本区域下部门扩展字段区域取值为空时相加，非空取首个员工的值
							-- 1、好多区域是空的; 2、问了下，区域不在这里用，这里用部门编码前7位;
							--((tmp.AreaCode is not null
							--	and tmp.AreaCode != ''
							--	)
							--	and tmp.AreaCode = tmp2.AreaCode
							--)
							--or ((tmp.AreaCode is null
							--	or tmp.AreaCode = ''
							--	)
							--	and case when len(tmp.DepartmentCode) > 7
							--		then SubString(tmp.DepartmentCode,0,7)
							--		else tmp.DepartmentCode end
							--										= case when len(tmp2.DepartmentCode) > 7
							--											then SubString(tmp2.DepartmentCode,0,7)
							--											else tmp2.DepartmentCode end
							--)
							1=1
							and tmp.EmployeeCode = tmp2.EmployeeCode
						--group by
						--	--IsNull(tmpEmployee.PartMinusRatio,0)
						--	--,IsNull(tmpEmployee.PartPlusRatio,0)
						--	IsNull(tmp2.PartMinusRatio,0)
						--	,IsNull(tmp2.PartPlusRatio,0)
						)
	--,TotalPartDeptShouldBeCashed = @DefaultZero
from #tmp_hbh_CashCalc tmp
--where
--	-- 区域管理者，才计算区域应兑现
--	AreaManagerType >= 1


--where
--	tmp.AreaCode is not null
--	and tmp.AreaCode != ''


-- 区域应兑现
update #tmp_hbh_CashCalc
set
	-- 区域应兑现.区域部分 = 非兼职部门的算法，是 最终利润 * 2%
	TotalAreaDeptShouldBeCashed = IsNull(AreaPerformance,0) * 0.02
	
from #tmp_hbh_CashCalc tmp
--where
--	-- 区域管理者，才计算区域应兑现
--	AreaManagerType >= 1


-- 区域应兑现
update #tmp_hbh_CashCalc
set
	-- 区域应兑现
	AreaShouldBeCashed = IsNull(TotalAreaDeptShouldBeCashed,0) + IsNull(TotalPartDeptShouldBeCashed,0)
	
from #tmp_hbh_CashCalc tmp
--where
--	-- 区域管理者，才计算区域应兑现
--	AreaManagerType >= 1



select *
from #tmp_hbh_CashCalc
where 1=1
	-- and EmployeeName = '黄娣'




--select *
--from #tmp_hbh_EmployeePartDept



select *
from #tmp_hbh_CashCalc
where
	EmployeeCode = '00000693'



select *
from #tmp_hbh_EmployeePartDept
where
	EmployeeCode = '00000693'

	
select *
from #tmp_hbh_DeptPerformance
where DepartmentCode like '0000314003%'
		