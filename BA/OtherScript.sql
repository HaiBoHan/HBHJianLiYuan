

	declare @DefaultZero decimal(24,9) = 0

-- HRTest
-- 劳产率 = 收入 / 出勤日次
-- If((IsNull([查询1].[日出勤人次],0) = 0),0,[查询1].[收入] / [查询1].[日出勤人次])
-- 人工成本比例 = 工资 / 收入
-- If((IsNull([查询1].[收入],0) = 0),0,[查询1].[工资] / [查询1].[收入])

-- 抽取脚本

-- 事实表，Fact_U9_DayCheckIn

select 
	Department
	,DepartmentCode
	,DepartmentName
	,CheckInDate
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
	,PersonTime = Sum(IsNull(PartTimeDay,@DefaultZero)) / 4 + Sum(IsNull(HourlyDay,@DefaultZero)) / 8 + Sum(IsNull(FullTimeDay,@DefaultZero))
	
	-- 收入
	,Income = Sum(Income)
	-- 区域
	,Region = Region
	,RegionCode
	,RegionName
	---- 标准工资
	--,StardardSalary = Sum(StardardSalary)
	
	-- 员工日工资
	-- 日工资=标准工资/27 *全日制员工出勤+钟点工工资标准(038)*钟点工出勤+非全日制员工出勤*钟标准工资		;钟点工工资标准取U9维护薪资项目数值点工工资标准
	-- 27 = 当月天数 - 4
	,Salary = Sum(StardardSalary / 27 * IsNull(FullTimeDay,@DefaultZero) + IsNull(PartTimeDay,@DefaultZero) * IsNull(PartSalary,@DefaultZero) + IsNull(HourlyDay,@DefaultZero) * IsNull(OvertimeSalary,@DefaultZero))

from (
	select 
		-- 员工
		employee.ID as EmployeeArchive
		-- 部门
		,Department = IsNull(dept.ID,-1)
		,DepartmentCode = IsNull(dept.Code,'')
		,DepartmentName = IsNull(deptTrl.Name,'')

		,checkin.CheckInDate
		,StatisticsPeriod = Datename(year,checkin.CheckInDate) + '年' + Datename(month,checkin.CheckInDate) + '月'
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
		,Income = IsNull(checkin.Income,@DefaultZero)
		-- 区域
		,Region = IsNull(region.ID,-1)
		,RegionCode = IsNull(region.Code,'')
		,RegionName = IsNull(regionTrl.Name,'')
		-- 全日制标准工资=基本工资（01）+周末加班工资（02）+电话补贴（03）+交通补贴(04)+午餐补贴（05）+职务补贴（07）
		,StardardSalary = Sum(dbo.HBH_Fn_GetDecimal(
				case when salaryItem.Code in ('01','02','03','04','05','07') 
					then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
				else @DefaultZero end
					,@DefaultZero))
		
		-- 非全日制标准工资 = 钟点工工资标准(06)			-- (038)
		,PartSalary = Sum(dbo.HBH_Fn_GetDecimal(
				case when salaryItem.Code in ('06') 
					then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
				else @DefaultZero end
					,@DefaultZero))
		-- 加班工资
		-- 加班工资标准=薪资项目中F工资标准项目(F01)					-- （F13）
		,OvertimeSalary = Sum(dbo.HBH_Fn_GetDecimal(
				case when salaryItem.Code in ('F01') 
					then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
				else @DefaultZero end
					,@DefaultZero))

	from 		
		HR20161108.dbo.CBO_EmployeeArchive employee	
		left join HR20161108.dbo.CBO_Department dept
		-- on checkin.Department = dept.ID
		on employee.Dept = dept.ID
		left join HR20161108.dbo.CBO_Department_Trl deptTrl
		on deptTrl.ID = dept.ID
			and deptTrl.SysMLFlag = 'zh-CN'
		
		left join HR20161108.dbo.Cust_DayCheckInLine checkinLine
		on checkinLine.EmployeeArchive = employee.ID
		left join HR20161108.dbo.Cust_DayCheckIn checkin
		on checkin.ID = checkinLine.DayCheckIn
	 
		left join HR20161108.dbo.CBO_Department region
		on SubString(dept.Code,1,5) = region.Code
		left join HR20161108.dbo.CBO_Department_Trl regionTrl
		on regionTrl.ID = region.ID
			and regionTrl.SysMLFlag = 'zh-CN'

		--left join CBO_Person person
		--on checkinLine.StaffMember = person.ID
		left join HR20161108.dbo.CBO_EmployeeSalaryFile salary
		on salary.Employee = employee.ID
		left join HR20161108.dbo.CBO_PublicSalaryItem salaryItem
		on salary.SalaryItem = salaryItem.ID
		left join HR20161108.dbo.CBO_PublicSalaryItem_Trl salaryItemTrl
		on salaryItemTrl.ID = salaryItem.ID 
			and salaryItemTrl.SysMLFlag = 'zh-CN'
	where 
			-- 全日制标准工资 = 标准工资=基本工资（01）+周末加班工资（02）+电话补贴（03）+交通补贴(04)+午餐补贴（05）+职务补贴（07）
		-- 非全日制标准工资 = 钟点工工资标准(06)			-- (038)
		-- 加班工资标准=薪资项目中F工资标准项目(F01)					-- （F13）
			(salaryItem.Code is null 
				or salaryItem.Code in ('01','02','03','04','05','07' ,'06','F01'))
	group by 
		-- 员工
		employee.ID
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
		-- 区域
		,IsNull(region.ID,-1)
		,IsNull(region.Code,'')
		,IsNull(regionTrl.Name,'')
	) as checkinSummary
--where 1=1
group by 
	Department
	,DepartmentCode
	,DepartmentName
	,CheckInDate
	,StatisticsPeriod
	,Region
	,RegionCode
	,RegionName





