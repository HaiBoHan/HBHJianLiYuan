



select 
	person.PersonID
	,employee.EmployeeCode
	,salaryItem.Code
	,salaryItemTrl.Name
	,salary.SalaryItemVlaue
	,dbo.HBH_Fn_GetDecimal(salary.SalaryItemVlaue,0)
	,salaryItemTrl.*
	-- ,person.*
	,salary.*
from CBO_EmployeeSalaryFile salary
	inner join CBO_Person person
	on salary.Person = person.ID

	inner join CBO_EmployeeArchive employee
	on salary.Employee = employee.ID
	inner join CBO_PublicSalaryItem salaryItem
	on salary.SalaryItem = salaryItem.ID
	inner join CBO_PublicSalaryItem_Trl salaryItemTrl
	on salaryItemTrl.ID = salaryItem.ID 
		and SysMLFlag = 'zh-CN'

where
	-- ��׼����=�������ʣ�01��+��ĩ�Ӱ๤�ʣ�02��+�绰������03��+��ͨ����(04)+��Ͳ�����05��+ְ������07���ӵ㹤���ʱ�׼=н����Ŀ��F���ʱ�׼��Ŀ��F13��
	salaryItem.Code in ('01','02','03','04','05','07')
	--and employee.EmployeeCode = '00000019'



