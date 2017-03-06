
/*
select *
from HBH_SPParamRecord
order by CreatedOn desc
*/

if exists(select * from sys.objects where name='HBH_BASP_JianLiYuan_EfficiencyCostWarning')
-- 如果存在则删掉
	drop proc HBH_BASP_JianLiYuan_EfficiencyCostWarning
go
-- 假期人均效率人工成本预警表
-- 创建存储过程
create proc HBH_BASP_JianLiYuan_EfficiencyCostWarning  (
-- @StartDate datetime = null
-- ,@EndDate datetime = null

@请选择过滤年月 varchar(125) = ''
,@请选择大区 varchar(125) = ''
,@请选择区域 varchar(125) = ''
,@请选择部门 varchar(125) = ''
,@请选择开始日期 varchar(125) = ''
,@请选择结束日期 varchar(125) = ''
)
with encryption
as
	SET NOCOUNT ON;
	

if exists(select name from sys.objects where name = 'HBH_Debug_Param')
begin
	declare @Debugger bit = (select top 1 Debugger from HBH_Debug_Param where ProcName = 'HBH_BASP_JianLiYuan_EfficiencyCostWarning' or ProcName is null or ProcName = '' order by ProcName desc)
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
		select 'HBH_BASP_JianLiYuan_EfficiencyCostWarning','@请选择过滤年月',IsNull(@请选择过滤年月,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_EfficiencyCostWarning','@请选择大区',IsNull(@请选择大区,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_EfficiencyCostWarning','@请选择区域',IsNull(@请选择区域,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_EfficiencyCostWarning','@请选择部门',IsNull(@请选择部门,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_EfficiencyCostWarning','@请选择开始日期',IsNull(@请选择开始日期,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_EfficiencyCostWarning','@请选择结束日期',IsNull(@请选择结束日期,'null'),GETDATE()

		union select 'HBH_BASP_JianLiYuan_EfficiencyCostWarning','ProcSql','exec HBH_BASP_JianLiYuan_EfficiencyCostWarning '
				+ IsNull('''' + @请选择过滤年月 + '''' ,'null')
				+ ',' + IsNull('''' + @请选择大区 + '''' ,'null')
				+ ',' + IsNull('''' + @请选择区域 + '''' ,'null')
				+ ',' + IsNull('''' + @请选择部门 + '''' ,'null')
				+ ',' + IsNull('''' + @请选择开始日期 + '''' ,'null')
				+ ',' + IsNull('''' + @请选择结束日期 + '''' ,'null')

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
	-- 多选分隔标记
	declare @Separator varchar(2) = ';'


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
where
	dept.Code != '00001'
	and Len(dept.Code) = 5
	-- 不存在的新增
	and dept.ID not in (select region2.ID from Dim_U9_Department2 region2)

-- 区域
insert into Dim_U9_Department3
select dept.ID,dept.Code,deptTrl.Name,dept.Level+1 as Level,dept.Org
from [10.28.76.125].U9.dbo.CBO_Department dept 
	inner join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl 
	on dept.ID= deptTrl.ID and deptTrl.SysMLFlag='zh-CN'
where
	Len(dept.Code) = 7
	-- 不存在的新增
	and dept.ID not in (select region3.ID from Dim_U9_Department3 region3)



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

-- 事实表，Fact_U9_EfficiencyCostWarning
-- 删除
truncate table Fact_U9_EfficiencyCostWarning
-- 新增
insert into Fact_U9_EfficiencyCostWarning
--(
--	Region bigint
--	,RegionCode varchar(200)
--	,RegionName varchar(200)
--	,Region2 bigint
--	,Region2Code varchar(200)
--	,Region2Name varchar(200)

--	,Department bigint
--	,DepartmentCode varchar(200)
--	,DepartmentName varchar(200)
	
--	-- 年月期间
--	,StatisticsPeriod varchar(125)
--	-- 日期
--	,WarningDate varchar(125)
--	-- 餐次
--	,MealTime varchar(200) 
--	-- 预计就餐人数
--	,EstimatedQty decimal(24,9)
--	-- 餐标
--	,MealStandard decimal(24,9)
--	-- 就餐收入,这个做合计公式比较合适，不过不知道BA支持不;   =  早餐预计就餐人数*早餐餐标+中餐预计就餐人数*中餐餐标+晚餐预计就餐人数*晚餐餐标+夜餐预计就餐人数*夜餐餐标
--	,DiningIncome decimal(24,9)

--	-- 日出勤小时数
--	,AttendanceTime decimal(24,9)
--	-- 折算人数 ????  = 日出勤小时数/8
--	,TranslatedNumber decimal(24,9)

--	-- 当日人工工资
--	,Wage decimal(24,9)
--	-- 日综合毛利
--	,GrossProfit decimal(24,9)
	
--	-- 人均效率预警
--	-- 人均效率 = if（折算人数=0,0，就餐收入/折算人数）
--	,PerEfficiency decimal(24,9)
--	-- 公司节日控制标准(效率) = ：固定值500
--	,EfficiencyHolidayStandards decimal(24,9) default 500
--	-- 差异(效率) = 公司节日控制标准-人均效率
--	,EfficiencyDiffer decimal(24,9)
--	-- 达标情况(效率) = if(差异>=0,"达标","人均效率低于公司要求，请调整")
--	,EfficiencyStandardConditions varchar(125)

--	-- 人工成本预警
--	-- 人工成本 = IF(当日人工工资=0,"0",当日人工工资/就餐收入)
--	,PerCost decimal(24,9)
--	-- 公司节日控制标准(成本) = ：固定值20%
--	,CostHolidayStandards decimal(24,9) default 0.2
--	-- 差异(成本) = 公司节日控制标准-人均效率
--	,CostDiffer decimal(24,9)
--	-- 达标情况(成本) = IF(差异>=0,"人工成本超标，请调整","达标")
--	,CostStandardConditions varchar(125)

--	-- 备注
--	,Memo varchar(125)
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
	
	-- 年月期间
	,StatisticsPeriod
	-- 日期
	,WarningDate
	-- 日期
	,CheckInDate
	-- 餐次	(先做成合并的吧)
	,MealTime = -1
	-- 预计就餐人数
	,EstimatedQty = EstimatedQty
	-- 餐标
	,MealStandard = MealStandard
	-- 就餐收入,这个做合计公式比较合适，不过不知道BA支持不;   =  早餐预计就餐人数*早餐餐标+中餐预计就餐人数*中餐餐标+晚餐预计就餐人数*晚餐餐标+夜餐预计就餐人数*夜餐餐标
	,DiningIncome = DiningIncome

	-- 日出勤小时数
	,AttendanceTime = AttendanceTime
	-- 折算人数 ????  = 日出勤小时数/8
	,TranslatedNumber = TranslatedNumber

	-- 当日人工工资
	,Wage = Wage
	-- 日综合毛利
	,GrossProfit = GrossProfit
	
	-- 人均效率预警
	-- 人均效率 = if（折算人数=0,0，就餐收入/折算人数）
	,PerEfficiency = case when IsNull(TranslatedNumber,0) = 0 then 0
						else IsNull(DiningIncome,0) / IsNull(TranslatedNumber,0) end

	-- 公司节日控制标准(效率) = ：固定值500
	,EfficiencyHolidayStandards = 500
	-- 差异(效率) = 公司节日控制标准-人均效率
	,EfficiencyDiffer = case when IsNull(TranslatedNumber,0) = 0 then 0
							else IsNull(DiningIncome,0) / IsNull(TranslatedNumber,0) end
						- 500
	-- 达标情况(效率) = if(差异>=0,"达标","人均效率低于公司要求，请调整")
	,EfficiencyStandardConditions =  case when (case when IsNull(TranslatedNumber,0) = 0 then 0
											else IsNull(DiningIncome,0) / IsNull(TranslatedNumber,0) end
										- 500) > 0
									then '达标'
									else '人均效率低于公司要求，请调整' end

	-- 人工成本预警
	-- 人工成本 = IF(当日人工工资=0,"0",当日人工工资/就餐收入)
	,PerCost = case when IsNull(DiningIncome,0) = 0 then 0
						else IsNull(Wage,0) / IsNull(DiningIncome,0) end
	-- 公司节日控制标准(成本) = ：固定值20%
	,CostHolidayStandards = 0.2
	-- 差异(成本) = 公司节日控制标准-人均效率
	,CostDiffer = case when IsNull(DiningIncome,0) = 0 then 0
					else IsNull(Wage,0) / IsNull(DiningIncome,0) end
				- 0.2
	-- 达标情况(成本) = IF(差异>=0,"人工成本超标，请调整","达标")
	,CostStandardConditions = case when (case when IsNull(DiningIncome,0) = 0 then 0
										else IsNull(Wage,0) / IsNull(DiningIncome,0) end
									- 0.2) <= 0
									then '达标'
									else '人工成本超标，请调整' end

	-- 备注
	,Memo = ''
	
	-- 预计就餐人数
	,MorningEstimatedQty
	,NoonEstimatedQty
	,AfternoonEstimatedQty
	,NightEstimatedQty
	
	-- 餐标
	,MorningMealStandard
	,NoonMealStandard
	,AfternoonMealStandard
	,NightMealStandard
from (
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
	
		-- 年月期间
		,StatisticsPeriod = Right('0000' + DateName(year,warningLine.Date),4) + '年' + Right('00' + DateName(month,warningLine.Date),2) + '月'
		-- 日期
		,WarningDate = Right('00' + DateName(Month,warningLine.Date),2) + '.' + Right('00' + DateName(day,warningLine.Date),2)
		-- 日期
		,CheckInDate = warningLine.Date
		---- 餐次
		--,MealTime = MealTime
		-- 预计就餐人数
		,EstimatedQty = sum(IsNull(EstimatedQty,0))
		-- 餐标
		--,MealStandard = max(IsNull(MealStandard,0))
		,MealStandard = @DefaultZero
		-- 就餐收入,这个做合计公式比较合适，不过不知道BA支持不;   =  早餐预计就餐人数*早餐餐标+中餐预计就餐人数*中餐餐标+晚餐预计就餐人数*晚餐餐标+夜餐预计就餐人数*夜餐餐标
		,DiningIncome = sum(IsNull(EstimatedQty,0) * IsNull(MealStandard,0))

		-- 日出勤小时数
		,AttendanceTime = sum(IsNull(AttendanceTime,0))
		-- 折算人数 ????  = 日出勤小时数/8
		,TranslatedNumber = sum(IsNull(AttendanceTime,0) / 8)

		-- 当日人工工资
		,Wage = sum(IsNull(Wage,0))
		-- 日综合毛利
		,GrossProfit = sum(IsNull(GrossProfit,0))
	
		---- 人均效率预警
		---- 人均效率 = if（折算人数=0,0，就餐收入/折算人数）
		--,PerEfficiency = case when sum(IsNull(AttendanceTime,0)) = 0 then 0
		--					else sum(IsNull(DiningIncome,0)) / (sum(IsNull(AttendanceTime,0))/8) end

		---- 公司节日控制标准(效率) = ：固定值500
		--,EfficiencyHolidayStandards = 500
		---- 差异(效率) = 公司节日控制标准-人均效率
		--,EfficiencyDiffer = 500 - case when sum(IsNull(AttendanceTime,0)) = 0 then 0
		--							else sum(IsNull(DiningIncome,0)) / (sum(IsNull(AttendanceTime,0))/8) end
		---- 达标情况(效率) = if(差异>=0,"达标","人均效率低于公司要求，请调整")
		---- ,EfficiencyStandardConditions = sum(IsNull(,0))

		---- 人工成本预警
		---- 人工成本 = IF(当日人工工资=0,"0",当日人工工资/就餐收入)
		--,PerCost = case when sum(IsNull(DiningIncome,0)) = 0 then 0
		--					else sum(IsNull(Wage,0)) / sum(IsNull(DiningIncome,0)) end
		---- 公司节日控制标准(成本) = ：固定值20%
		--,CostHolidayStandards = 0.2
		---- 差异(成本) = 公司节日控制标准-人均效率
		--,CostDiffer = 0.2 - case when sum(IsNull(DiningIncome,0)) = 0 then 0
		--						else sum(IsNull(Wage,0)) / sum(IsNull(DiningIncome,0)) end
		---- 达标情况(成本) = IF(差异>=0,"人工成本超标，请调整","达标")
		----,CostStandardConditions = 

		---- 备注
		--,Memo = ''
	
		/*	MealTime
		Morning	早	0
		Noon	中	1
		Afternoon	晚	2
		Night	夜	3
		*/
		-- 预计就餐人数
		,MorningEstimatedQty = Sum(case when MealTime = 0 then IsNull(EstimatedQty,0)
									else @DefaultZero end
									)
		,NoonEstimatedQty = Sum(case when MealTime = 1 then IsNull(EstimatedQty,0)
									else @DefaultZero end
									)
		,AfternoonEstimatedQty = Sum(case when MealTime = 2 then IsNull(EstimatedQty,0)
									else @DefaultZero end
									)
		,NightEstimatedQty = Sum(case when MealTime = 3 then IsNull(EstimatedQty,0)
									else @DefaultZero end
									)
	
		-- 餐标
		,MorningMealStandard = Max(case when MealTime = 0 then IsNull(MealStandard,0)
									else @DefaultZero end
									)
		,NoonMealStandard = Max(case when MealTime = 1 then IsNull(MealStandard,0)
									else @DefaultZero end
									)
		,AfternoonMealStandard = Max(case when MealTime = 2 then IsNull(MealStandard,0)
									else @DefaultZero end
									)
		,NightMealStandard = Max(case when MealTime = 3 then IsNull(MealStandard,0)
									else @DefaultZero end
									)
	from 
		[10.28.76.125].U9.dbo.Cust_CostWarning warning
		inner join [10.28.76.125].U9.dbo.Cust_CostWarningLine warningLine
		on warning.ID = warningLine.CostWarning		

	
		left join [10.28.76.125].U9.dbo.CBO_Department dept
		-- on checkin.Department = dept.ID
		on dept.ID = warning.Department

		left join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl
		on deptTrl.ID = dept.ID
			and deptTrl.SysMLFlag = 'zh-CN'
	 
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
	where 1=1

		and (@请选择开始日期 is null or @请选择开始日期 = ''
			or warningLine.Date >= (select max(dateStart.DayDate) from Dim_U9_Date_Filter dateStart where dateStart.DayName = @请选择开始日期)
			)
		and (@请选择结束日期 is null or @请选择结束日期 = ''
			or warningLine.Date <= (select max(dateEnd.DayDate) from Dim_U9_Date_Filter dateEnd where dateEnd.DayName = @请选择结束日期)
			)

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
		,IsNull(dept.ID,-1)
		,IsNull(dept.Code,'')
		,IsNull(deptTrl.Name,'')
	
		-- 年月期间
		,Right('0000' + DateName(year,warningLine.Date),4) + '年' + Right('00' + DateName(month,warningLine.Date),2) + '月'
		-- 日期
		,Right('00' + DateName(Month,warningLine.Date),2) + '.' + Right('00' + DateName(day,warningLine.Date),2)
		,warningLine.Date
		---- 餐次
		--,MealTime = MealTime
	) warningDetail




select *
from Fact_U9_EfficiencyCostWarning
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




