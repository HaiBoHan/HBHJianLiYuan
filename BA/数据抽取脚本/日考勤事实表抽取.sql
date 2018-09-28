

select 
	Department
	,DepartmentCode
	,DepartmentName
	,CheckInDate
	--,checkin.Status as Status
	--,checkin.CurrentOperator as CurrentOperator

	--,checkinLine.StaffMember as Staff
	--,person.PersonID as StaffCode
	--,person.Name as StaffName
	
	-- 全日制
	,FullTimeDay = Sum(IsNull(FullTimeDay,0))
	-- 非全日制
	,PartTimeDay = Sum(IsNull(PartTimeDay,0))
	-- 钟点工
	,HourlyDay = Sum(IsNull(HourlyDay,0))
	-- 日出勤人数 = 非全日制员工出勤合计/4 + 钟点工员工出勤合计/8 + 全日制员工出勤合计 
	,PersonTime = Sum(IsNull(PartTimeDay,0)) / 4 + Sum(IsNull(HourlyDay,0)) / 8 + Sum(IsNull(FullTimeDay,0))
	
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
	,Salary = Sum(StardardSalary / 27 * IsNull(FullTimeDay,0) + IsNull(PartTimeDay,0) * 0 + IsNull(HourlyDay,0) * 0)

from (
	select 
		-- 员工
		employee.ID as EmployeeArchive
		-- 部门
		,Department = IsNull(dept.ID,-1)
		,DepartmentCode = IsNull(dept.Code,'')
		,DepartmentName = IsNull(deptTrl.Name,'')

		,checkin.CheckInDate
		--,checkin.Status as Status
		--,checkin.CurrentOperator as CurrentOperator

		--,checkinLine.StaffMember as Staff
		--,person.PersonID as StaffCode
		--,person.Name as StaffName

	
		-- 全日制
		,FullTimeDay = IsNull(checkinLine.FullTimeDay,0)
		-- 非全日制
		,PartTimeDay = IsNull(checkinLine.PartTimeDay,0)
		-- 钟点工
		,HourlyDay = IsNull(checkinLine.HourlyDay,0)
	
		-- 收入
		,Income = IsNull(checkin.Income,0)
		-- 区域
		,Region = IsNull(region.ID,-1)
		,RegionCode = IsNull(region.Code,'')
		,RegionName = IsNull(regionTrl.Name,'')
		-- 标准工资 = 标准工资=基本工资（01）+周末加班工资（02）+电话补贴（03）+交通补贴(04)+午餐补贴（05）+职务补贴（07）钟点工工资标准=薪资项目中F工资标准项目（F13）
		,StardardSalary = Sum(dbo.HBH_Fn_GetDecimal(IsNull(salary.SalaryItemVlaue,0),0))


	from 		
		CBO_EmployeeArchive employee	
		left join CBO_Department dept
		-- on checkin.Department = dept.ID
		on employee.Dept = dept.ID
		left join CBO_Department_Trl deptTrl
		on deptTrl.ID = dept.ID
			and deptTrl.SysMLFlag = 'zh-CN'
		
		left join Cust_DayCheckInLine checkinLine
		on checkinLine.EmployeeArchive = employee.ID
		left join Cust_DayCheckIn checkin
		on checkin.ID = checkinLine.DayCheckIn
	 
		left join CBO_Department region
		on SubString(dept.Code,1,5) = region.Code
		left join CBO_Department_Trl regionTrl
		on regionTrl.ID = region.ID
			and regionTrl.SysMLFlag = 'zh-CN'

		--left join CBO_Person person
		--on checkinLine.StaffMember = person.ID
		left join CBO_EmployeeSalaryFile salary
		on salary.Employee = employee.ID
		left join CBO_PublicSalaryItem salaryItem
		on salary.SalaryItem = salaryItem.ID
		left join CBO_PublicSalaryItem_Trl salaryItemTrl
		on salaryItemTrl.ID = salaryItem.ID 
			and salaryItemTrl.SysMLFlag = 'zh-CN'
	where 
			-- 标准工资 = 标准工资=基本工资（01）+周末加班工资（02）+电话补贴（03）+交通补贴(04)+午餐补贴（05）+职务补贴（07）钟点工工资标准=薪资项目中F工资标准项目（F13）
			(salaryItem.Code is null 
				or salaryItem.Code in ('01','02','03','04','05','07'))
	group by 
		-- 员工
		employee.ID
		,IsNull(dept.ID,-1)
		,IsNull(dept.Code,'')
		,IsNull(deptTrl.Name,'')
		,checkin.CheckInDate
		-- ,checkinLine.EmployeeArchive
	
		-- 全日制
		,IsNull(checkinLine.FullTimeDay,0)
		-- 非全日制
		,IsNull(checkinLine.PartTimeDay,0)
		-- 钟点工
		,IsNull(checkinLine.HourlyDay,0)	
		-- 收入
		,IsNull(checkin.Income,0)
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
	,Region
	,RegionCode
	,RegionName



