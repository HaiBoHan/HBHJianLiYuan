

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


select 
	item.DataType
	, item.ValueSet		-- Base_ValueSetDef		-- S008	部门
	, item.Code
	, itemTrl.Name
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
	DataType = @DataType
	, ValueSet = (select def.ID from Base_ValueSetDef as def where def.Code = @DefCode)

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
	, item.*
from PAY_SalaryItem_Trl itemTrl
	inner join PAY_SalaryItem item
	on itemTrl.ID = item.ID
		and itemTrl.SysMLFlag = @SysMLFlag
where
	item.Code = @SalaryItemCode
	
