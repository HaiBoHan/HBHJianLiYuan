


select 
	-- ����
	Region = IsNull(region.ID,-1)
	,RegionCode = IsNull(region.Code,'')
	,RegionName = IsNull(regionTrl.Name,'')
	-- ����
	,Region2 = IsNull(region2.ID,-1)
	,Region2Code = IsNull(region2.Code,'')
	,Region2Name = IsNull(region2Trl.Name,'')
	-- ����
	,Department = IsNull(dept.ID,-1)
	,DepartmentCode = IsNull(dept.Code,'')
	,DepartmentName = IsNull(deptTrl.Name,'')

	,checkinSummary.CheckInDate
	,checkinSummary.DisplayDate
	,checkinSummary.StatisticsPeriod
	--,checkin.Status as Status
	--,checkin.CurrentOperator as CurrentOperator

	--,checkinLine.StaffMember as Staff
	--,person.PersonID as StaffCode
	--,person.Name as StaffName
		
	-- Ԥ��
	-- ���� = �ͱ� * �Ͳ�����
	,ForecastHolidayDayIncome = Warning.Income
	-- �ճ���Сʱ��
	,ForecastAttendance = Warning.AttendanceTime
	-- �����˹�����
	,ForecastWage = Warning.Wage
	-- Ԥ��.���˾�Ч�� = ���� / ��������
	,ForecastEfficiency = case when Warning.AttendanceTime is not null and Warning.AttendanceTime != 0
							then IsNull(Warning.Income,0) / Warning.AttendanceTime
							else 0 end
	
	-- ����
	,FactHolidayDayIncome = (checkinSummary.Income)
	-- �ճ������� = ��ȫ����Ա�����ںϼ�/4 + �ӵ㹤Ա�����ںϼ�/8 + ȫ����Ա�����ںϼ� 
	,FactAttendance = checkinSummary.PersonTime
	-- ʵ��.���˾�Ч�� =  ���� / ��������
	,FactEfficiency = case when checkinSummary.PersonTime is not null and checkinSummary.PersonTime != 0
							then IsNull(checkinSummary.Income,0) / checkinSummary.PersonTime
							else 0 end

	-- ȫ����
	,FullTimeDay = (IsNull(checkinSummary.FullTimeDay,@DefaultZero))
	-- ��ȫ����
	,PartTimeDay = (IsNull(checkinSummary.PartTimeDay,@DefaultZero))
	-- �ӵ㹤
	,HourlyDay = (IsNull(checkinSummary.HourlyDay,@DefaultZero))
	
from (
	select 
		---- ����
		--Department = IsNull(dept.ID,-1)
		--,DepartmentCode = IsNull(dept.Code,'')
		--,DepartmentName = IsNull(deptTrl.Name,'')
		Department = employee.Dept

		,checkin.CheckInDate
		,DisplayDate = Right('00' + DateName(Month,checkin.CheckInDate),2) + '.' + Right('00' + DateName(day,checkin.CheckInDate),2)
		,StatisticsPeriod = Right('0000' + DateName(year,checkin.CheckInDate),4) + '��' + Right('00' + DateName(month,checkin.CheckInDate),2) + '��'
			
		-- ȫ����
		,FullTimeDay = Sum(IsNull(checkinLine.FullTimeDay,@DefaultZero))
		-- ��ȫ����
		,PartTimeDay = Sum(IsNull(checkinLine.PartTimeDay,@DefaultZero))
		-- �ӵ㹤
		,HourlyDay = Sum(IsNull(checkinLine.HourlyDay,@DefaultZero))
	
		-- ����
		,Income = max(IsNull(checkin.Income,@DefaultZero))
		---- ����
		--,Region = IsNull(region.ID,-1)
		--,RegionCode = IsNull(region.Code,'')
		--,RegionName = IsNull(regionTrl.Name,'')
		---- ����
		--,Region2 = IsNull(region2.ID,-1)
		--,Region2Code = IsNull(region2.Code,'')
		--,Region2Name = IsNull(region2Trl.Name,'')
		
		-- �ճ������� = ��ȫ����Ա�����ںϼ�/4 + �ӵ㹤Ա�����ںϼ�/8 + ȫ����Ա�����ںϼ� 
		,PersonTime = Sum(IsNull(checkinLine.PartTimeDay,@DefaultZero)) / 4 + Sum(IsNull(checkinLine.HourlyDay,@DefaultZero)) / 8 + Sum(IsNull(checkinLine.FullTimeDay,@DefaultZero))
	
	from 		
		HR20161108.dbo.CBO_EmployeeArchive employee	
		
		left join HR20161108.dbo.Cust_DayCheckInLine checkinLine
		on checkinLine.EmployeeArchive = employee.ID
		left join HR20161108.dbo.Cust_DayCheckIn checkin
		on checkin.ID = checkinLine.DayCheckIn
	where 
		employee.Dept is not null
	group by 
		--IsNull(dept.ID,-1)
		--,IsNull(dept.Code,'')
		--,IsNull(deptTrl.Name,'')
		employee.Dept

		,checkin.CheckInDate
		-- ,checkinLine.EmployeeArchive
		---- ����
		--,IsNull(region.ID,-1)
		--,IsNull(region.Code,'')
		--,IsNull(regionTrl.Name,'')
		--,IsNull(region2.ID,-1)
		--,IsNull(region2.Code,'')
		--,IsNull(region2Trl.Name,'')
	) as checkinSummary

	full join (
			select 
				warning.Department
				,warningLine.Date
				-- ���� = �ͱ� * �Ͳ�����
				,Income = Sum(MealStandard * EstimatedQty)
				-- �ճ���Сʱ��
				,AttendanceTime = Max(AttendanceTime)
				-- �����˹�����
				,Wage = Max(Wage)
			from HR20161108.dbo.Cust_CostWarning warning
				inner join HR20161108.dbo.Cust_CostWarningLine warningLine
				on warning.ID = warningLine.CostWarning
			group by
				warning.Department
				,warningLine.Date
			) as Warning
	on checkinSummary.Department = Warning.Department
		and checkinSummary.CheckInDate = Warning.Date
		

	
	left join HR20161108.dbo.CBO_Department dept
	-- on checkin.Department = dept.ID
	on dept.ID = IsNull(checkinSummary.Department,Warning.Department)

	left join HR20161108.dbo.CBO_Department_Trl deptTrl
	on deptTrl.ID = dept.ID
		and deptTrl.SysMLFlag = 'zh-CN'
	 
	left join HR20161108.dbo.CBO_Department region
	on SubString(dept.Code,1,5) = region.Code
	left join HR20161108.dbo.CBO_Department_Trl regionTrl
	on regionTrl.ID = region.ID
		and regionTrl.SysMLFlag = 'zh-CN'
	 
	left join HR20161108.dbo.CBO_Department region2
	on SubString(dept.Code,1,7) = region2.Code
	left join HR20161108.dbo.CBO_Department_Trl region2Trl
	on region2Trl.ID = region2.ID
		and region2Trl.SysMLFlag = 'zh-CN'

