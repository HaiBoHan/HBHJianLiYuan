

/*
select *
from HBH_SPParamRecord
order by CreatedOn desc
*/

if exists(select * from sys.objects where name='HBH_BASP_JianLiYuan_DayCheckIn')
-- 如果存在则删掉
	drop proc HBH_BASP_JianLiYuan_DayCheckIn
go
-- 劳产率报表
-- 创建存储过程
create proc HBH_BASP_JianLiYuan_DayCheckIn  (
-- @StartDate datetime = null
-- ,@EndDate datetime = null

@请选择过滤年月 varchar(125) = ''
,@请选择大区 varchar(125) = ''
,@请选择区域 varchar(125) = ''
,@请选择部门 varchar(125) = ''
,@请选择开始日期 varchar(125) = ''
,@请选择结束日期 varchar(125) = ''
,@领导用表 varchar(125) = ''
)
with encryption
as
	SET NOCOUNT ON;
	

if exists(select name from sys.objects where name = 'HBH_Debug_Param')
begin
	declare @Debugger bit = (select top 1 Debugger from HBH_Debug_Param where ProcName = 'HBH_BASP_JianLiYuan_DayCheckIn' or ProcName is null or ProcName = '' order by ProcName desc)
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
		select 'HBH_BASP_JianLiYuan_DayCheckIn','@请选择过滤年月',IsNull(@请选择过滤年月,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_DayCheckIn','@请选择大区',IsNull(@请选择大区,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_DayCheckIn','@请选择区域',IsNull(@请选择区域,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_DayCheckIn','@请选择部门',IsNull(@请选择部门,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_DayCheckIn','@请选择开始日期',IsNull(@请选择开始日期,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_DayCheckIn','@请选择结束日期',IsNull(@请选择结束日期,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_DayCheckIn','@领导用表',IsNull(@领导用表,'null'),GETDATE()

		union select 'HBH_BASP_JianLiYuan_DayCheckIn','ProcSql','exec HBH_BASP_JianLiYuan_DayCheckIn '
				+ IsNull('''' + @请选择过滤年月 + '''' ,'null')
				+ ',' + IsNull('''' + @请选择大区 + '''' ,'null')
				+ ',' + IsNull('''' + @请选择区域 + '''' ,'null')
				+ ',' + IsNull('''' + @请选择部门 + '''' ,'null')
				+ ',' + IsNull('''' + @请选择开始日期 + '''' ,'null')
				+ ',' + IsNull('''' + @请选择结束日期 + '''' ,'null')
				+ ',' + IsNull('''' + @领导用表 + '''' ,'null')

			   ,GETDATE()
	end
end

	declare @SysMlFlag varchar(11) = 'zh-CN'
	declare @DefaultZero decimal(24,9) = 0
	declare @Now datetime = GetDate();
	declare @CurDate datetime = GetDate()
	declare @Today datetime = convert(varchar(10), GetDate(), 120)
	declare @StartDate datetime
	declare @EndDate datetime
	/*
	01	北京三源
	02	青岛健力源
	03	烟台分公司
	04	济南健力源
	05	管理中心
	06	采购中心
	07	北京健力源
	08	沈阳健力源
	*/
	declare @ManageOrgCode varchar(125) = '05'
	-- 多选分隔标记
	declare @Separator varchar(2) = ';'
	
	-- 设置每周第一天是哪天(周一)
	set datefirst 1

	

if(@请选择过滤年月 is null or @请选择过滤年月 = '')
begin
	select @StartDate=max(dateStart.DayDate)
	from Dim_U9_Date_Filter dateStart 
	where dateStart.DayName = @请选择开始日期

	select @EndDate = max(dateStart.DayDate)
	from Dim_U9_Date_Filter dateStart 
	where dateStart.DayName = @请选择结束日期

end
else
begin

	set @StartDate = cast((replace(replace(@请选择过滤年月,'年','-'),'月','-') + '01') as datetime)
	-- 下个月1号减一天
	set @EndDate = DateAdd(day,-1,DateAdd(Month,1,@StartDate))
	
end


-- 部门二表,删除数据，重新抽取
/*
truncate table Dim_U9_Department2

insert into Dim_U9_Department2
select dept.ID,dept.Code,deptTrl.Name,dept.Level+1 as Level,dept.Org
from [10.28.76.125].U9.dbo.CBO_Department dept 
	inner join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl 
	on dept.ID= deptTrl.ID and deptTrl.SysMLFlag='zh-CN'
where
	dept.Code != '00001'
	and Len(dept.Code) = 5
	

truncate table Dim_U9_Department3

insert into Dim_U9_Department3
select dept.ID,dept.Code,deptTrl.Name,dept.Level+1 as Level,dept.Org
from [10.28.76.125].U9.dbo.CBO_Department dept 
	inner join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl 
	on dept.ID= deptTrl.ID and deptTrl.SysMLFlag='zh-CN'
where
	Len(dept.Code) = 7
*/

-- 大区
insert into Dim_U9_Department2
select dept.ID,dept.Code,deptTrl.Name,dept.Level+1 as Level,dept.Org
from [10.28.76.125].U9.dbo.CBO_Department dept 
	inner join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl 
	on dept.ID= deptTrl.ID and deptTrl.SysMLFlag='zh-CN'
	inner join [10.28.76.125].U9.dbo.Base_Organization org
	on dept.Org = org.ID
where
	dept.Code != '00001'
	and Len(dept.Code) = 5
	-- 不存在的新增
	and dept.ID not in (select region2.ID from Dim_U9_Department2 region2)
	-- 管理组织
	and org.Code = @ManageOrgCode

-- 区域
insert into Dim_U9_Department3
select dept.ID,dept.Code,deptTrl.Name,dept.Level+1 as Level,dept.Org
from [10.28.76.125].U9.dbo.CBO_Department dept 
	inner join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl 
	on dept.ID= deptTrl.ID and deptTrl.SysMLFlag='zh-CN'
	inner join [10.28.76.125].U9.dbo.Base_Organization org
	on dept.Org = org.ID
where
	Len(dept.Code) = 7
	-- 不存在的新增
	and dept.ID not in (select region3.ID from Dim_U9_Department3 region3)
	-- 管理组织
	and org.Code = @ManageOrgCode


---- 部门
--insert into Dim_Department
--select dept.ID,dept.Code,deptTrl.Name,dept.Level+1 as Level,dept.Org
--from [10.28.76.125].U9.dbo.CBO_Department dept 
--	inner join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl 
--	on dept.ID= deptTrl.ID and deptTrl.SysMLFlag='zh-CN'
--	inner join [10.28.76.125].U9.dbo.Base_Organization org
--	on dept.Org = org.ID
--where
--	Len(dept.Code) = 10
--	-- 不存在的新增
--	and dept.ID not in (select dept.ID from Dim_Department dept)
--	-- 管理组织
--	and org.Code = @ManageOrgCode



-- 多选部门、区域

-- 部门
If OBJECT_ID('tempdb..#hbh_tmp_Department') is not null
	Drop Table #hbh_tmp_Department

select Item
into #hbh_tmp_Department
from dbo.HBH_Fn_StrSplitToTable(@请选择部门,@Separator,0)

-- 区域
If OBJECT_ID('tempdb..#hbh_tmp_Region') is not null
	Drop Table #hbh_tmp_Region

select Item
into #hbh_tmp_Region
from dbo.HBH_Fn_StrSplitToTable(@请选择区域,@Separator,0)

-- 大区
If OBJECT_ID('tempdb..#hbh_tmp_BigRegion') is not null
	Drop Table #hbh_tmp_BigRegion

select Item
into #hbh_tmp_BigRegion
from dbo.HBH_Fn_StrSplitToTable(@请选择大区,@Separator,0)




-- HRTest
-- 劳产率 = 收入 / 出勤日次
-- If((IsNull([查询1].[日出勤人次],0) = 0),0,[查询1].[收入] / [查询1].[日出勤人次])
-- 人工成本比例 = 工资 / 收入
-- If((IsNull([查询1].[收入],0) = 0),0,[查询1].[工资] / [查询1].[收入])

-- 抽取脚本

-- 事实表，Fact_U9_DayCheckIn
-- 删除
truncate table Fact_U9_DayCheckIn
-- 新增
insert into Fact_U9_DayCheckIn
--(
--	Department
--	,DepartmentCode
--	,DepartmentName
--	,DepartmentDisplayName varchar(200)
--	,CheckInDate
--	,StatisticsPeriod
--	-- 全日制
--	,FullTimeDay
--	-- 非全日制
--	,PartTimeDay
--	-- 钟点工
--	,HourlyDay
--	-- 日出勤人数 = 非全日制员工出勤合计/4 + 钟点工员工出勤合计/8 + 全日制员工出勤合计 
--	,PersonTime
--	-- 收入
--	,Income
--  -- 劳产率目标
--  ,LaborYieldTarget
--  -- 人工成本目标
--  ,LaborCostTarget
--	-- 区域
--	,Region
--	,RegionCode
--	,RegionName
--	,Salary
--)

select 
	-- 大区
	Region
	,RegionCode
	,RegionName -- = '(' + RegionCode + ')' + RegionName
	-- 区域
	,Region2 
	,Region2Code
	,Region2Name -- = '(' + Region2Code + ')' + Region2Name
	-- 部门
	,Department 
	,DepartmentCode
	,DepartmentName -- = '(' + DepartmentCode + ')' + DepartmentName

	-- 带上劳产率、人工成本目标的 部门显示名
	/*改为  部门名称 | 劳产率目标 | 人工成本目标
部门名称,预留11个字空间
劳产率目标，预留5个字空间
人工成本目标，预留5个字空间
	*/
	-- 如果字符串类型是varchar,则中文占2，数字占1; 如果字符串类型是nvarchar,则中文和数字都占2
	-- 默认查出来的是  nvarchar
	-- ,DepartmentDisplayName = Left(IsNull(DepartmentName,'') + '                      ',22 - (datalength(cast(DepartmentName as varchar(125))) - len(DepartmentName))) + '|'
	,DepartmentDisplayName = Left(IsNull(DepartmentName,'') + replicate('	',11),11 + Ceiling((cast(len(DepartmentName) * 2 as decimal(3,1)) - cast(datalength(cast(DepartmentName as varchar(125))) as decimal(3,1))) / 2)  ) + '|'
		+ Left(LaborYieldTarget + replicate('	',5),5 + Ceiling((cast(len(LaborYieldTarget) * 2 as decimal(3,1)) - cast(datalength(cast(LaborYieldTarget as varchar(125))) as decimal(3,1))) / 2)  ) + '|'
		+ Left(LaborCostTarget + replicate('	',5),5 + Ceiling((cast(len(LaborCostTarget) * 2 as decimal(3,1)) - cast(datalength(cast(LaborCostTarget as varchar(125))) as decimal(3,1))) / 2))
		
	--Department
	--,DepartmentCode
	--,DepartmentName
	---- 区域
	--,Region = Region
	--,RegionCode
	--,RegionName
	
	,CheckInDate
	,DisplayDate
	,StatisticsPeriod
	--,checkin.Status as Status
	--,checkin.CurrentOperator as CurrentOperator

	--,checkinLine.StaffMember as Staff
	--,person.PersonID as StaffCode
	--,person.Name as StaffName
	
	-- 全日制
	,FullTimeDay = Sum(IsNull(FullTimeDay,@DefaultZero))
	-- 非全日制
	,PartTimeDay = Sum(IsNull(PartTimeDay,@DefaultZero))
	-- 钟点工
	,HourlyDay = Sum(IsNull(HourlyDay,@DefaultZero))
	-- 日出勤人数 = 非全日制员工出勤合计/4 + 钟点工员工出勤合计/8 + 全日制员工出勤合计 
	--,PersonTime = Sum(IsNull(PartTimeDay,@DefaultZero)) / 4 + Sum(IsNull(HourlyDay,@DefaultZero)) / 8 + Sum(IsNull(FullTimeDay,@DefaultZero))
	,PersonTime = Sum(PersonTime)
	
	-- 收入(如果是按周，那么汇总每天)
	--,Income = Max(Income)
	,Income = Sum(Income)
	-- 劳产率目标
	--,LaborYieldTarget = dbo.HBH_Fn_GetString(Round(Max(LaborYieldTarget),0,0))
	,LaborYieldTarget = LaborYieldTarget
	-- 人工成本目标
	--,LaborCostTarget = dbo.HBH_Fn_GetString(Round(Max(LaborCostTarget),4,0) * 100) + '%'
	,LaborCostTarget = LaborCostTarget
	---- 标准工资
	--,StardardSalary = Sum(StardardSalary)
	
	-- 员工日工资
	-- 日工资=标准工资/27 *全日制员工出勤+钟点工工资标准(038)*钟点工出勤+非全日制员工出勤*钟标准工资		;钟点工工资标准取U9维护薪资项目数值点工工资标准
	-- 27 = 当月天数 - 4
	--,Salary = Sum(StardardSalary / MonthDays * IsNull(FullTimeDay,@DefaultZero) + IsNull(PartTimeDay,@DefaultZero) * IsNull(PartSalary,@DefaultZero) + IsNull(HourlyDay,@DefaultZero) * IsNull(OvertimeSalary,@DefaultZero))
	--,Salary = Sum(
	--	-- 全日制员工工资
	--	(StardardSalary / MonthDays * IsNull(FullTimeDay,@DefaultZero) 
	--	-- 非全日制员工工资
	--	+ IsNull(FPartSalary,@DefaultZero) * IsNull(PartTimeDay,@DefaultZero) 
	--	-- 全日制加班工资
	--	+ IsNull(OvertimeSalary,@DefaultZero) * IsNull(HourlyDay,@DefaultZero) 
	--	-- 非全日制加班工资
	--	+ IsNull(FOvertimeSalary,@DefaultZero) * IsNull(HourlyDay,@DefaultZero))
	--	)
	,Salary = Sum(Salary)

	-- 应出勤天数 = 当月天数 - 4
	,MonthDays  -- = (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end)
	-- 日保险
	--,DayInsurance = Sum
	--		(
	--		-- 全日制员工保险
	--		(IsNull(InsuranceSalary,@DefaultZero) / MonthDays * IsNull(FullTimeDay,@DefaultZero) 
	--		-- 非全日制员工保险
	--		+ (IsNull(FInsuranceSalary,@DefaultZero) / MonthDays) * IsNull(PartTimeDay,@DefaultZero) )
	--		)
	,DayInsurance = Sum(DayInsurance)
	
	-- 日成本合计 = 日工资 + 日保险
	,DayCost = Sum(Salary) + Sum(DayInsurance)

	-- 月份第一天
	,FirstDay
	-- 月份第一天  是第几周
	,FirstWeek
	-- 月份第一天  有周几
	,FirstWeekDay
		
	-- 月份最后一天
	,LastDay
	-- 月份最后一天  是第几周
	,LastWeek
	-- 月份最后一天  有周几
	,LastWeekDay

from (	
		select 
			-- 大区
			Region
			,RegionCode
			,RegionName
			-- 区域
			,Region2
			,Region2Code
			,Region2Name
			-- 部门
			,Department
			,DepartmentCode
			,DepartmentName

			---- 员工
			--,EmployeeArchive
			---- 部门
			--,Department = IsNull(dept.ID,-1)
			--,DepartmentCode = IsNull(dept.Code,'')
			--,DepartmentName = IsNull(deptTrl.Name,'')

			--,case when IsNull(@领导用表,'') = '是' then DATEPART(WEEK,checkin.CheckInDate) - DATEPART(WEEK, DateAdd(M,-1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)) + 1,DateAdd(M,1,checkin.CheckInDate)))) + 1
			--	else convert(varchar(10),checkin.CheckInDate,23)
			--	end as CheckInDate
			--,CheckInDate = convert(varchar(10),checkin.CheckInDate,23)
			,CheckInDate
			,StatisticsPeriod
			--,DisplayDate
			-- 普通，按天；领导，按周；
			,DisplayDate = case when IsNull(@领导用表,'') = '是' 
					--then Right(DateName(year,CheckInDate),2) + '年' + Right('00' + DateName(month,CheckInDate),2) + '月' + '第' + cast(IsNull(CheckMonthNumber,0) as char(1)) + '周(' 
					--	-- 周的当月第一天(周一是当月几号)
					--	+ case when CheckMonthNumber = FirstWeek then '1' else cast(IsNull(Day(DateAdd(d,-CheckWeekDay + 1,CheckInDate)),0) as varchar(2)) end
					--	+ '-'
					--	-- 周的当月最后一天
					--	+ case when CheckMonthNumber = LastWeek then cast(IsNull(Day(LastDay),0) as varchar(2)) 
					--			else cast(IsNull(Day(DateAdd(d,7 - CheckWeekDay,CheckInDate)),0) as varchar(2)) end
					--	+ ')'
					then Right(CONVERT(varchar(100),case when WeekFirstDay > @StartDate then WeekFirstDay else @StartDate end, 2),8)
						+ '-'
						+ Right(CONVERT(varchar(100),case when @EndDate > WeekLastDay then WeekLastDay else @EndDate end, 2),8)
					else DisplayDate
					end
	
			-- 全日制
			,FullTimeDay = Sum(IsNull(FullTimeDay,@DefaultZero))
			-- 非全日制
			,PartTimeDay = Sum(IsNull(PartTimeDay,@DefaultZero))
			-- 钟点工
			,HourlyDay = Sum(IsNull(HourlyDay,@DefaultZero))
			-- 日出勤人数 = 非全日制员工出勤合计/4 + 钟点工员工出勤合计/8 + 全日制员工出勤合计 
			,PersonTime = Sum(IsNull(PartTimeDay,@DefaultZero)) / 4 + Sum(IsNull(HourlyDay,@DefaultZero)) / 8 + Sum(IsNull(FullTimeDay,@DefaultZero))
	
			-- 收入
			,Income = Max(Income)
			-- 劳产率目标
			--,LaborYieldTarget = Max(LaborYieldTarget)
			,LaborYieldTarget = dbo.HBH_Fn_GetString(Round(Max(LaborYieldTarget),0,0))
			-- 人工成本目标
			--,LaborCostTarget = Max(LaborCostTarget)
			,LaborCostTarget = dbo.HBH_Fn_GetString(Round(Max(LaborCostTarget),4,0) * 100) + '%'
			---- 标准工资
			--,StardardSalary = Sum(StardardSalary)
		
			-- 员工日工资
			-- 日工资=标准工资/27 *全日制员工出勤+钟点工工资标准(038)*钟点工出勤+非全日制员工出勤*钟标准工资		;钟点工工资标准取U9维护薪资项目数值点工工资标准
			-- 27 = 当月天数 - 4
			--,Salary = Sum(StardardSalary / MonthDays * IsNull(FullTimeDay,@DefaultZero) + IsNull(PartTimeDay,@DefaultZero) * IsNull(PartSalary,@DefaultZero) + IsNull(HourlyDay,@DefaultZero) * IsNull(OvertimeSalary,@DefaultZero))
			,Salary = Sum(
				-- 全日制员工工资
				(StardardSalary / (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end) * IsNull(FullTimeDay,@DefaultZero) 
				-- 非全日制员工工资
				+ IsNull(FPartSalary,@DefaultZero) * IsNull(PartTimeDay,@DefaultZero) 
				-- 全日制加班工资
				+ IsNull(OvertimeSalary,@DefaultZero) * IsNull(HourlyDay,@DefaultZero) 
				-- 非全日制加班工资
				+ IsNull(FOvertimeSalary,@DefaultZero) * IsNull(HourlyDay,@DefaultZero))
				)

			---- 本月天数	
			-- 应出勤天数 = 当月天数 - 4
			,MonthDays = (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end)
		
	
			---- 全日制标准工资=基本工资（01）+周末加班工资（02）+电话补贴（03）+交通补贴(04)+午餐补贴（05）+职务补贴（07）
			--,StardardSalary
					
			---- 加班工资
			---- 钟点工工资标准 = 钟点工工资标准(06)			-- (038)
			--,OvertimeSalary
		
			---- F钟点工工资标准（F01） = 钟点工工资标准(F01)			-- (F13)
			--,FPartSalary
			---- F加班工资
			---- FJ钟点工工资标准（F06）=薪资项目中F工资标准项目(F06)					-- （F56）
			--,FOvertimeSalary

			---- 单位保险
			---- 单位保险（12）=薪资项目中 单位保险(12)					-- （113）
			--,InsuranceSalary
			---- F单位保险
			---- F单位保险（F04）=薪资项目中 F单位保险(F04)					-- （F52）
			--,FInsuranceSalary

			
			-- 日保险
			,DayInsurance = Sum
					(
					---- 全日制员工保险
					--(IsNull(InsuranceSalary,@DefaultZero) / (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end) * IsNull(FullTimeDay,@DefaultZero) 
					---- 非全日制员工保险
					--+ (IsNull(FInsuranceSalary,@DefaultZero) / (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end)) * IsNull(PartTimeDay,@DefaultZero) )

					-- 2017-04-11 李震林  就用部门在职的人员的单位保险的和除以应出勤天数就行了
					(IsNull(InsuranceSalary,@DefaultZero) / (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end))
					)
			
			-- 月份第一天
			,FirstDay
			-- 月份第一天  是第几周
			,FirstWeek
			-- 月份第一天  有周几
			,FirstWeekDay
		
			-- 月份最后一天
			,LastDay
			-- 月份最后一天  是第几周
			,LastWeek
			-- 月份最后一天  有周几
			,LastWeekDay
	from
		(
		select 
			-- 大区
			Region = IsNull(region.ID,-1)
			,RegionCode = IsNull(region.Code,'')
			,RegionName = IsNull(regionTrl.Name,'')
			-- 区域
			,Region2 = IsNull(region2.ID,-1)
			,Region2Code = IsNull(region2.Code,'')
			,Region2Name = IsNull(region2Trl.Name,'')
			-- 部门
			,Department = IsNull(dept.ID,-1)
			,DepartmentCode = IsNull(dept.Code,'')
			,DepartmentName = IsNull(deptTrl.Name,'')

			-- 员工
			,employee.ID as EmployeeArchive
			---- 部门
			--,Department = IsNull(dept.ID,-1)
			--,DepartmentCode = IsNull(dept.Code,'')
			--,DepartmentName = IsNull(deptTrl.Name,'')

			--,case when IsNull(@领导用表,'') = '是' then DATEPART(WEEK,checkin.CheckInDate) - DATEPART(WEEK, DateAdd(M,-1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)) + 1,DateAdd(M,1,checkin.CheckInDate)))) + 1
			--	else convert(varchar(10),checkin.CheckInDate,23)
			--	end as CheckInDate
			--,CheckInDate = convert(varchar(10),checkin.CheckInDate,23)
			,CheckInDate = checkin.CheckInDate
			,DisplayDate = Right('00' + DateName(Month,checkin.CheckInDate),2) + '.' + Right('00' + DateName(day,checkin.CheckInDate),2)
			,StatisticsPeriod = Right('0000' + DateName(year,checkin.CheckInDate),4) + '年' + Right('00' + DateName(month,checkin.CheckInDate),2) + '月'
		
			-- 本月第几周
			,CheckMonthNumber = DATEPART(WEEK,checkin.CheckInDate) - DATEPART(WEEK, DateAdd(M,-1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)) + 1,DateAdd(M,1,checkin.CheckInDate)))) + 1
			-- 本月第几天
			,CheckDayNumber = DATEPART(day,checkin.CheckInDate)
			-- 是周几
			,CheckWeekDay = DatePart(weekday,checkin.CheckInDate)

			-- 本周第一天
			,WeekFirstDay = DateAdd(day,1 -  DatePart(weekday,checkin.CheckInDate),checkin.CheckInDate)
			-- 本周最后一天
			,WeekLastDay = DateAdd(day,7 -  DatePart(weekday,checkin.CheckInDate),checkin.CheckInDate )

			-- 月份第一天
			,FirstDay = DateAdd(M,-1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)) + 1,DateAdd(M,1,checkin.CheckInDate))) 
			--,FirstDay = case when @请选择开始日期 is null or @请选择开始日期 = '' then DateAdd(M,-1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)) + 1,DateAdd(M,1,checkin.CheckInDate))) 
			--				else StartDate.DayDate end
			-- 月份第一天  是第几周
			,FirstWeek = 1
			-- 月份第一天  是周几
			,FirstWeekDay = DatePart(weekday,DateAdd(M,-1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)) + 1,DateAdd(M,1,checkin.CheckInDate)))) 
		
			-- 月份最后一天
			,LastDay = DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)),DateAdd(M,1,checkin.CheckInDate)) 
			--,LastDay =  case when @请选择结束日期 is null or @请选择结束日期 = ''
			--				then DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)),DateAdd(M,1,checkin.CheckInDate)) 
			--				else EndDate.DayDate end
			-- 月份最后一天  是第几周
			,LastWeek = DATEPART(WEEK,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)),DateAdd(M,1,checkin.CheckInDate))) - DATEPART(WEEK, DateAdd(M,-1,DateAdd(d,- day(DateAdd(M,1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)),DateAdd(M,1,checkin.CheckInDate)))) + 1,DateAdd(M,1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)),DateAdd(M,1,checkin.CheckInDate)))))) + 1 
			-- 月份最后一天  是周几
			,LastWeekDay = DatePart(weekday,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)),DateAdd(M,1,checkin.CheckInDate))) 

			--,checkin.Status as Status
			--,checkin.CurrentOperator as CurrentOperator

			--,checkinLine.StaffMember as Staff
			--,person.PersonID as StaffCode
			--,person.Name as StaffName

	
			-- 全日制
			,FullTimeDay = IsNull(checkinLine.FullTimeDay,@DefaultZero)
			-- 非全日制
			,PartTimeDay = IsNull(checkinLine.PartTimeDay,@DefaultZero)
			-- 钟点工
			,HourlyDay = IsNull(checkinLine.HourlyDay,@DefaultZero)
	
			-- 收入
			,Income = max(IsNull(checkin.Income,@DefaultZero))
			-- 劳产率目标 , 可能有没有行的，所以 做子查询吧
			--,LaborYieldTarget = max(IsNull(checkin.LaborYieldTarget,@DefaultZero))
			,LaborYieldTarget = (select max(IsNull(checkin2.LaborYieldTarget,@DefaultZero)) from [10.28.76.125].U9.dbo.Cust_DayCheckIn checkin2 
								where checkin.Department = checkin2.Department
									-- 年月相同
									and DatePart(year,checkin.CheckInDate)*100 + DatePart(month,checkin.CheckInDate)
										= DatePart(year,checkin2.CheckInDate)*100 + DatePart(month,checkin2.CheckInDate)
								)
			-- 人工成本目标 , 可能有没有行的，所以 做子查询吧
			--,LaborCostTarget = max(IsNull(checkin.LaborCostTarget,@DefaultZero))
			,LaborCostTarget = (select max(IsNull(checkin2.LaborCostTarget,@DefaultZero)) from [10.28.76.125].U9.dbo.Cust_DayCheckIn checkin2 
								where  checkin.Department = checkin2.Department
									-- 年月相同
									and DatePart(year,checkin.CheckInDate)*100 + DatePart(month,checkin.CheckInDate)
										= DatePart(year,checkin2.CheckInDate)*100 + DatePart(month,checkin2.CheckInDate)
								)
			---- 区域
			--,Region = IsNull(region.ID,-1)
			--,RegionCode = IsNull(region.Code,'')
			--,RegionName = IsNull(regionTrl.Name,'')
		
			---- 本月天数	
			-- 应出勤天数 = 当月天数 - 4
			,MonthDays = IsNull(Day(DateAdd(Day,-1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)) + 1,DateAdd(M,1,checkin.CheckInDate)))),27)  - 4
		-- 改为在日考勤中录入
			,IsNull((select max(IsNull(checkin2.MonthWorkDays,0)) 
						from [10.28.76.125].U9.dbo.Cust_DayCheckIn checkin2
						where checkin2.Department = checkIn.Department
						--group by checkin2.Department
						)
					,0) as MonthWorkDays
	
			-- 全日制标准工资=基本工资（01）+周末加班工资（02）+电话补贴（03）+交通补贴(04)+午餐补贴（05）+职务补贴（07）
			-- 2017-01-10 wf  现场让修改成：(基本工资（01） + 周末加班工资（02）)，改成了 标准工资.(14)
			,StardardSalary = Sum(dbo.HBH_Fn_GetDecimal(
					--case when salaryItem.Code in ('01','02','03','04','05','07') 
					case when salaryItem.Code in ('14','03','04','05','07')
						then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
					else @DefaultZero end
						,@DefaultZero))
		
			-- 加班工资
			-- 钟点工工资标准 = 钟点工工资标准(06)			-- (038)
			,OvertimeSalary = Sum(dbo.HBH_Fn_GetDecimal(
					case when salaryItem.Code in ('06') 
						then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
					else @DefaultZero end
						,@DefaultZero))
		
			-- F钟点工工资标准（F01） = 钟点工工资标准(F01)			-- (F13)
			,FPartSalary = Sum(dbo.HBH_Fn_GetDecimal(
					case when salaryItem.Code in ('F01') 
						then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
					else @DefaultZero end
						,@DefaultZero))
			-- F加班工资
			-- FJ钟点工工资标准（F06）=薪资项目中F工资标准项目(F06)					-- （F56）
			,FOvertimeSalary = Sum(dbo.HBH_Fn_GetDecimal(
					case when salaryItem.Code in ('F06') 
						then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
					else @DefaultZero end
						,@DefaultZero))

			-- 单位保险
			-- 单位保险（12）=薪资项目中 单位保险(12)					-- （113）
			,InsuranceSalary = Sum(dbo.HBH_Fn_GetDecimal(
					case when salaryItem.Code in ('12') 
						then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
					else @DefaultZero end
						,@DefaultZero))
			-- F单位保险
			-- F单位保险（F04）=薪资项目中 F单位保险(F04)					-- （F52）
			,FInsuranceSalary = Sum(dbo.HBH_Fn_GetDecimal(
					case when salaryItem.Code in ('F04') 
						then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
					else @DefaultZero end
						,@DefaultZero))
		from 		
			[10.28.76.125].U9.dbo.Cust_DayCheckIn checkin
			inner join [10.28.76.125].U9.dbo.Cust_DayCheckInLine checkinLine
			on checkin.ID = checkinLine.DayCheckIn

			inner join [10.28.76.125].U9.dbo.CBO_EmployeeArchive employee
			on checkinLine.EmployeeArchive = employee.ID	
			inner join [10.28.76.125].U9.dbo.CBO_Department dept
			-- 要按考勤头 的部门，来确认考勤部门
			on checkin.Department = dept.ID
			-- on employee.Dept = dept.ID
			inner join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl
			on deptTrl.ID = dept.ID
				and deptTrl.SysMLFlag = 'zh-CN'
	 
			--left join [10.28.76.125].U9.dbo.CBO_Department region
			--on SubString(dept.Code,1,5) = region.Code
			--left join [10.28.76.125].U9.dbo.CBO_Department_Trl regionTrl
			--on regionTrl.ID = region.ID
			--	and regionTrl.SysMLFlag = 'zh-CN'

			--left join CBO_Person person
			--on checkinLine.StaffMember = person.ID
			left join [10.28.76.125].U9.dbo.CBO_EmployeeSalaryFile salary
			on salary.Employee = employee.ID
			left join [10.28.76.125].U9.dbo.CBO_PublicSalaryItem salaryItem
			on salary.SalaryItem = salaryItem.ID
			left join [10.28.76.125].U9.dbo.CBO_PublicSalaryItem_Trl salaryItemTrl
			on salaryItemTrl.ID = salaryItem.ID 
				and salaryItemTrl.SysMLFlag = 'zh-CN'
			
	
			--left join [10.28.76.125].U9.dbo.CBO_Department dept
			---- on checkin.Department = dept.ID
			--on dept.ID = checkinSummary.Department

			--left join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl
			--on deptTrl.ID = dept.ID
			--	and deptTrl.SysMLFlag = 'zh-CN'
	 
			left join [10.28.76.125].U9.dbo.CBO_Department region
			on SubString(dept.Code,1,5) = region.Code
			left join [10.28.76.125].U9.dbo.CBO_Department_Trl regionTrl
			on regionTrl.ID = region.ID
				and regionTrl.SysMLFlag = 'zh-CN'
	 
			left join [10.28.76.125].U9.dbo.CBO_Department region2
			on SubString(dept.Code,1,7) = region2.Code
			left join [10.28.76.125].U9.dbo.CBO_Department_Trl region2Trl
			on region2Trl.ID = region2.ID
				and region2Trl.SysMLFlag = 'zh-CN'

			--left join (select max(dateStart.DayDate) as DayDate
			--		from Dim_U9_Date_Filter dateStart 
			--		where dateStart.DayName = @请选择开始日期) StartDate
			--on 1=1
			--left join (select max(dateStart.DayDate) as DayDate
			--		from Dim_U9_Date_Filter dateStart 
			--		where dateStart.DayName = @请选择结束日期) EndDate
			--on 1=1
		where 
				-- 全日制标准工资 = 标准工资=基本工资（01）+周末加班工资（02）+电话补贴（03）+交通补贴(04)+午餐补贴（05）+职务补贴（07）
			-- 非全日制标准工资 = 钟点工工资标准(06)			-- (038)
			-- 加班工资标准=薪资项目中F工资标准项目(F01)					-- （F13）
				--(salaryItem.Code is null 
				--	or salaryItem.Code in ('01','02','03','04','05','07' ,'06','F01'))

			dept.ID is not null

			
			/* -- 状态
			Approved	已审核	2
			Approving	审核中	1
			Closed	已关闭	3
			Opened	开立	0
			*/
			and checkin.Status = 2
			
			--and (@请选择大区 is null or @请选择大区 = ''
			--	or @请选择大区 = regionTrl.Name
			--	)
			--and (@请选择区域 is null or @请选择区域 = ''
			--	or @请选择区域 = region2Trl.Name
			--	)
			--and (@请选择部门 is null or @请选择部门 = ''
			--	or @请选择部门 = deptTrl.Name
			--	)

			and (@请选择大区 is null or @请选择大区 = ''
				-- or @请选择大区 = 
				or regionTrl.Name in (select Item from #hbh_tmp_BigRegion )
				)
			and (@请选择区域 is null or @请选择区域 = ''
				--or @请选择区域 = region2Trl.Name
				or region2Trl.Name in (select Item from #hbh_tmp_Region )
				)
			and (@请选择部门 is null or @请选择部门 = ''
				--or @请选择部门 = deptTrl.Name
				or deptTrl.Name in (select Item from #hbh_tmp_Department )
				)

			--and (@请选择过滤年月 is null or @请选择过滤年月 = ''
			--	or @请选择过滤年月 = Right('0000' + DateName(year,checkin.CheckInDate),4) + '年' + Right('00' + DateName(month,checkin.CheckInDate),2) + '月'
			--	)
			--and (@请选择开始日期 is null or @请选择开始日期 = ''
			--	or checkin.CheckInDate >= StartDate.DayDate --(select max(dateStart.DayDate) from Dim_U9_Date_Filter dateStart where dateStart.DayName = @请选择开始日期)
			--	)
			--and (@请选择结束日期 is null or @请选择结束日期 = ''
			--	or checkin.CheckInDate <= EndDate.DayDate --(select max(dateEnd.DayDate) from Dim_U9_Date_Filter dateEnd where dateEnd.DayName = @请选择结束日期)
			--	)

			and checkin.CheckInDate >= @StartDate
			and checkin.CheckInDate <= @EndDate

		group by 
			-- 大区
			IsNull(region.ID,-1)
			,IsNull(region.Code,'')
			,IsNull(regionTrl.Name,'')
			-- 区域
			,IsNull(region2.ID,-1)
			,IsNull(region2.Code,'')
			,IsNull(region2Trl.Name,'')
			-- 部门
			,checkIn.Department
			,IsNull(dept.ID,-1)
			,IsNull(dept.Code,'')
			,IsNull(deptTrl.Name,'')

			-- 员工
			,employee.ID
			,IsNull(dept.ID,-1)
			,IsNull(dept.Code,'')
			,IsNull(deptTrl.Name,'')
			,checkin.CheckInDate
			-- ,checkinLine.EmployeeArchive
	
			-- 全日制
			,IsNull(checkinLine.FullTimeDay,@DefaultZero)
			-- 非全日制
			,IsNull(checkinLine.PartTimeDay,@DefaultZero)
			-- 钟点工
			,IsNull(checkinLine.HourlyDay,@DefaultZero)	
			-- 收入
			,IsNull(checkin.Income,@DefaultZero)
			---- 区域
			--,IsNull(region.ID,-1)
			--,IsNull(region.Code,'')
			--,IsNull(regionTrl.Name,'')
			--,StartDate.DayDate
			--,EndDate.DayDate

		) as checkinSummary
	group by 
		-- 大区
		Region
		,RegionCode
		,RegionName
		-- 区域
		,Region2
		,Region2Code
		,Region2Name
		-- 部门
		,Department
		,DepartmentCode
		,DepartmentName

		---- 员工
		--,employee.ID
		,CheckInDate
		-- ,checkinLine.EmployeeArchive
		,DisplayDate
		,StatisticsPeriod
	
		-- 应出勤天数 = 当月天数 - 4
		,MonthDays
		-- 考勤表中的应出勤天数
		,MonthWorkDays
			
		-- 月份第一天
		,FirstDay
		-- 月份第一天  是第几周
		,FirstWeek
		-- 月份第一天  有周几
		,FirstWeekDay

		-- 本周第一天
		,WeekFirstDay
		-- 本周最后一天
		,WeekLastDay 
		
		-- 月份最后一天
		,LastDay
		-- 月份最后一天  是第几周
		,LastWeek
		-- 月份最后一天  有周几
		,LastWeekDay
			
		-- 本月第几周
		,CheckMonthNumber
		-- 本月第几天
		,CheckDayNumber
		-- 是周几
		,CheckWeekDay

		) as checkinSummary
--where 1=1
group by 
	Department
	,DepartmentCode
	,DepartmentName
	,Region
	,RegionCode
	,RegionName
	,Region2
	,Region2Code
	,Region2Name

	,CheckInDate
	,DisplayDate
	,StatisticsPeriod
	
	-- 应出勤天数 = 当月天数 - 4
	,MonthDays
	
	-- 月份第一天
	,FirstDay
	-- 月份第一天  是第几周
	,FirstWeek
	-- 月份第一天  有周几
	,FirstWeekDay
		
	-- 月份最后一天
	,LastDay
	-- 月份最后一天  是第几周
	,LastWeek
	-- 月份最后一天  有周几
	,LastWeekDay
	
	,LaborYieldTarget
	,LaborCostTarget


select *
from Fact_U9_DayCheckIn
where (@请选择过滤年月 is null or @请选择过滤年月 = ''
		or @请选择过滤年月 = StatisticsPeriod
		)
	and (@请选择大区 is null or @请选择大区 = ''
		or @请选择大区 = RegionName
		)
	and (@请选择区域 is null or @请选择区域 = ''
		or @请选择区域 = Region2Name
		)
	and (@请选择部门 is null or @请选择部门 = ''
		or @请选择部门 = DepartmentName
		)
	and (@请选择开始日期 is null or @请选择开始日期 = ''
		or CheckInDate >= (select max(dateStart.DayDate) from Dim_U9_Date_Filter dateStart where dateStart.DayName = @请选择开始日期)
		)
	and (@请选择结束日期 is null or @请选择结束日期 = ''
		or CheckInDate <= (select max(dateEnd.DayDate) from Dim_U9_Date_Filter dateEnd where dateEnd.DayName = @请选择结束日期)
		)
order by
	RegionCode
	,Region2Code
	,DepartmentCode 
	,DisplayDate

