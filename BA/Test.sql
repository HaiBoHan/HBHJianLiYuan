



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
	-- 标准工资=基本工资（01）+周末加班工资（02）+电话补贴（03）+交通补贴(04)+午餐补贴（05）+职务补贴（07）钟点工工资标准=薪资项目中F工资标准项目（F13）
	salaryItem.Code in ('01','02','03','04','05','07')
	--and employee.EmployeeCode = '00000019'



