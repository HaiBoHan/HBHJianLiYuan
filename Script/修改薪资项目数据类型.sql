

declare @SalaryItemCode varchar(125) = '00104055'



/*	PAY_SalaryItem		DataType
Strings	字符	0	DataType
Decimals	数值	1	DataType
Currency	货币	2	DataType
Date	日期	3	DataType
Boolean	布尔	4	DataType
EnumTypeVal	枚举	5	DataType
Entity	档案	6	DataType
*/
-- Entity	档案	6	DataType
declare @DataType int = 6
-- 值集编码
declare @DefCode varchar(125) = 'S008'
-- 薪资项目名称
declare @SalaryItemName varchar(125) = '临时部门'
-- 语种
declare @SysMLFlag varchar(125) = 'zh-CN'
/*	PAY_SalaryItem	ItemStyle
TypeAttribute	AttributeName	Value	AttributeValue	AttributeValueName
Salary	工资	0	ItemStyle	形态
RetirePay	离退休金	1	ItemStyle	形态
CheckAttendee	考勤	2	ItemStyle	形态
Achievement	绩效	3	ItemStyle	形态
*/
-- 形态
declare @ItemStyle int = 0
/*	PAY_SalaryItem	PlanPeriodType
TypeAttribute	AttributeName	Value	AttributeValue	AttributeValueName
Year	年	0	PlanPeriodType	计薪期间类型
HalfYear	半年	1	PlanPeriodType	计薪期间类型
Quarter	季	2	PlanPeriodType	计薪期间类型
Month	月	3	PlanPeriodType	计薪期间类型
FourWeek	四周	4	PlanPeriodType	计薪期间类型
TwoWeek	两周	5	PlanPeriodType	计薪期间类型
Week	周	6	PlanPeriodType	计薪期间类型
Day	日	7	PlanPeriodType	计薪期间类型
*/
-- 计薪期间类型
declare @PlanPeriodType int = 3
/*	PAY_SalaryItem	PayType
TypeAttribute	AttributeName	Value	AttributeValue	AttributeValueName
YearPaySalary	年薪	0	PayType	基准期间类型
HalfPaySalary	半年薪	1	PayType	基准期间类型
QuarterPaySalary	季薪	2	PayType	基准期间类型
MonthPaySalary	月薪	3	PayType	基准期间类型
WeekPaySalary	周薪	4	PayType	基准期间类型
DayPaySalary	日薪	5	PayType	基准期间类型
HourPaySalary	小时薪	6	PayType	基准期间类型
DoubleWeekPaySalary	双周薪	7	PayType	基准期间类型
*/
-- 基准期间类型
declare @PayType int = 3
/*	PAY_SalaryItem	ExecutePeriod
TypeAttribute	AttributeName	Value	AttributeValue	AttributeValueName
CurrentPeriod	当前期间	0	ExecutePeriod	执行计算期间
DelayPeriod	期间延后	1	ExecutePeriod	执行计算期间
AdvancedPeriod	期间提前	2	ExecutePeriod	执行计算期间
*/
-- 执行计算期间
declare @ExecutePeriod int = 0



select 
	item.DataType
	, item.ValueSet		-- Base_ValueSetDef		-- S008	部门
	, item.Code
	, itemTrl.Name
	
	-- 形态
	, item.ItemStyle
	-- 计薪期间类型
	, item.PlanPeriodType
	-- 计薪期间类型
	, item.PayType
	-- 执行计算期间
	, item.ExecutePeriod

	, item.*
from PAY_SalaryItem_Trl itemTrl
	inner join PAY_SalaryItem item
	on itemTrl.ID = item.ID
		and itemTrl.SysMLFlag = @SysMLFlag
where
	item.Code = @SalaryItemCode
	

	
/*
select 
	*
into hbh_tmp_PAY_SalaryItem_180917001
from PAY_SalaryItem
*/


update PAY_SalaryItem
set
	-- 数据类型
	DataType = @DataType
	-- 值集
	, ValueSet = (select def.ID from Base_ValueSetDef as def where def.Code = @DefCode)
	
	-- 形态
	, ItemStyle = @ItemStyle
	-- 计薪期间类型
	, PlanPeriodType = @PlanPeriodType
	-- 计薪期间类型
	, PayType = @PayType
	-- 执行计算期间
	, ExecutePeriod = @ExecutePeriod

	, ModifiedOn = GetDate()
	, ModifiedBy = 'hbh'
	, SysVersion = SysVersion + 1
where
	Code = @SalaryItemCode

	

update PAY_SalaryItem_Trl
set
	Name = @SalaryItemName

from PAY_SalaryItem_Trl itemTrl
	inner join PAY_SalaryItem item
	on itemTrl.ID = item.ID
		and itemTrl.SysMLFlag = @SysMLFlag
where
	item.Code = @SalaryItemCode



	

select 
	item.DataType
	, item.ValueSet		-- Base_ValueSetDef		-- S008	部门
	, item.Code
	, itemTrl.Name
	
	-- 形态
	, item.ItemStyle
	-- 计薪期间类型
	, item.PlanPeriodType
	-- 计薪期间类型
	, item.PayType
	-- 执行计算期间
	, item.ExecutePeriod

	, item.*
from PAY_SalaryItem_Trl itemTrl
	inner join PAY_SalaryItem item
	on itemTrl.ID = item.ID
		and itemTrl.SysMLFlag = @SysMLFlag
where
	item.Code = @SalaryItemCode
	
