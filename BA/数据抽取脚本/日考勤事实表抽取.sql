

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
	
	-- ȫ����
	,FullTimeDay = Sum(IsNull(FullTimeDay,0))
	-- ��ȫ����
	,PartTimeDay = Sum(IsNull(PartTimeDay,0))
	-- �ӵ㹤
	,HourlyDay = Sum(IsNull(HourlyDay,0))
	-- �ճ������� = ��ȫ����Ա�����ںϼ�/4 + �ӵ㹤Ա�����ںϼ�/8 + ȫ����Ա�����ںϼ� 
	,PersonTime = Sum(IsNull(PartTimeDay,0)) / 4 + Sum(IsNull(HourlyDay,0)) / 8 + Sum(IsNull(FullTimeDay,0))
	
	-- ����
	,Income = Sum(Income)
	-- ����
	,Region = Region
	,RegionCode
	,RegionName
	---- ��׼����
	--,StardardSalary = Sum(StardardSalary)
	
	-- Ա���չ���
	-- �չ���=��׼����/27 *ȫ����Ա������+�ӵ㹤���ʱ�׼(038)*�ӵ㹤����+��ȫ����Ա������*�ӱ�׼����		;�ӵ㹤���ʱ�׼ȡU9ά��н����Ŀ��ֵ�㹤���ʱ�׼
	,Salary = Sum(StardardSalary / 27 * IsNull(FullTimeDay,0) + IsNull(PartTimeDay,0) * 0 + IsNull(HourlyDay,0) * 0)

from (
	select 
		-- Ա��
		employee.ID as EmployeeArchive
		-- ����
		,Department = IsNull(dept.ID,-1)
		,DepartmentCode = IsNull(dept.Code,'')
		,DepartmentName = IsNull(deptTrl.Name,'')

		,checkin.CheckInDate
		--,checkin.Status as Status
		--,checkin.CurrentOperator as CurrentOperator

		--,checkinLine.StaffMember as Staff
		--,person.PersonID as StaffCode
		--,person.Name as StaffName

	
		-- ȫ����
		,FullTimeDay = IsNull(checkinLine.FullTimeDay,0)
		-- ��ȫ����
		,PartTimeDay = IsNull(checkinLine.PartTimeDay,0)
		-- �ӵ㹤
		,HourlyDay = IsNull(checkinLine.HourlyDay,0)
	
		-- ����
		,Income = IsNull(checkin.Income,0)
		-- ����
		,Region = IsNull(region.ID,-1)
		,RegionCode = IsNull(region.Code,'')
		,RegionName = IsNull(regionTrl.Name,'')
		-- ��׼���� = ��׼����=�������ʣ�01��+��ĩ�Ӱ๤�ʣ�02��+�绰������03��+��ͨ����(04)+��Ͳ�����05��+ְ������07���ӵ㹤���ʱ�׼=н����Ŀ��F���ʱ�׼��Ŀ��F13��
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
			-- ��׼���� = ��׼����=�������ʣ�01��+��ĩ�Ӱ๤�ʣ�02��+�绰������03��+��ͨ����(04)+��Ͳ�����05��+ְ������07���ӵ㹤���ʱ�׼=н����Ŀ��F���ʱ�׼��Ŀ��F13��
			(salaryItem.Code is null 
				or salaryItem.Code in ('01','02','03','04','05','07'))
	group by 
		-- Ա��
		employee.ID
		,IsNull(dept.ID,-1)
		,IsNull(dept.Code,'')
		,IsNull(deptTrl.Name,'')
		,checkin.CheckInDate
		-- ,checkinLine.EmployeeArchive
	
		-- ȫ����
		,IsNull(checkinLine.FullTimeDay,0)
		-- ��ȫ����
		,IsNull(checkinLine.PartTimeDay,0)
		-- �ӵ㹤
		,IsNull(checkinLine.HourlyDay,0)	
		-- ����
		,IsNull(checkin.Income,0)
		-- ����
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



