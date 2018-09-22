

declare @SalaryItemCode varchar(125) = '00104055'



/*	PAY_SalaryItem		DataType
Strings	�ַ�	0	DataType
Decimals	��ֵ	1	DataType
Currency	����	2	DataType
Date	����	3	DataType
Boolean	����	4	DataType
EnumTypeVal	ö��	5	DataType
Entity	����	6	DataType
*/
-- Entity	����	6	DataType
declare @DataType int = 6
-- ֵ������
declare @DefCode varchar(125) = 'S008'
-- н����Ŀ����
declare @SalaryItemName varchar(125) = '��ʱ����'
-- ����
declare @SysMLFlag varchar(125) = 'zh-CN'
/*	PAY_SalaryItem	ItemStyle
TypeAttribute	AttributeName	Value	AttributeValue	AttributeValueName
Salary	����	0	ItemStyle	��̬
RetirePay	�����ݽ�	1	ItemStyle	��̬
CheckAttendee	����	2	ItemStyle	��̬
Achievement	��Ч	3	ItemStyle	��̬
*/
-- ��̬
declare @ItemStyle int = 0
/*	PAY_SalaryItem	PlanPeriodType
TypeAttribute	AttributeName	Value	AttributeValue	AttributeValueName
Year	��	0	PlanPeriodType	��н�ڼ�����
HalfYear	����	1	PlanPeriodType	��н�ڼ�����
Quarter	��	2	PlanPeriodType	��н�ڼ�����
Month	��	3	PlanPeriodType	��н�ڼ�����
FourWeek	����	4	PlanPeriodType	��н�ڼ�����
TwoWeek	����	5	PlanPeriodType	��н�ڼ�����
Week	��	6	PlanPeriodType	��н�ڼ�����
Day	��	7	PlanPeriodType	��н�ڼ�����
*/
-- ��н�ڼ�����
declare @PlanPeriodType int = 3
/*	PAY_SalaryItem	PayType
TypeAttribute	AttributeName	Value	AttributeValue	AttributeValueName
YearPaySalary	��н	0	PayType	��׼�ڼ�����
HalfPaySalary	����н	1	PayType	��׼�ڼ�����
QuarterPaySalary	��н	2	PayType	��׼�ڼ�����
MonthPaySalary	��н	3	PayType	��׼�ڼ�����
WeekPaySalary	��н	4	PayType	��׼�ڼ�����
DayPaySalary	��н	5	PayType	��׼�ڼ�����
HourPaySalary	Сʱн	6	PayType	��׼�ڼ�����
DoubleWeekPaySalary	˫��н	7	PayType	��׼�ڼ�����
*/
-- ��׼�ڼ�����
declare @PayType int = 3
/*	PAY_SalaryItem	ExecutePeriod
TypeAttribute	AttributeName	Value	AttributeValue	AttributeValueName
CurrentPeriod	��ǰ�ڼ�	0	ExecutePeriod	ִ�м����ڼ�
DelayPeriod	�ڼ��Ӻ�	1	ExecutePeriod	ִ�м����ڼ�
AdvancedPeriod	�ڼ���ǰ	2	ExecutePeriod	ִ�м����ڼ�
*/
-- ִ�м����ڼ�
declare @ExecutePeriod int = 0



select 
	item.DataType
	, item.ValueSet		-- Base_ValueSetDef		-- S008	����
	, item.Code
	, itemTrl.Name
	
	-- ��̬
	, item.ItemStyle
	-- ��н�ڼ�����
	, item.PlanPeriodType
	-- ��н�ڼ�����
	, item.PayType
	-- ִ�м����ڼ�
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
	-- ��������
	DataType = @DataType
	-- ֵ��
	, ValueSet = (select def.ID from Base_ValueSetDef as def where def.Code = @DefCode)
	
	-- ��̬
	, ItemStyle = @ItemStyle
	-- ��н�ڼ�����
	, PlanPeriodType = @PlanPeriodType
	-- ��н�ڼ�����
	, PayType = @PayType
	-- ִ�м����ڼ�
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
	, item.ValueSet		-- Base_ValueSetDef		-- S008	����
	, item.Code
	, itemTrl.Name
	
	-- ��̬
	, item.ItemStyle
	-- ��н�ڼ�����
	, item.PlanPeriodType
	-- ��н�ڼ�����
	, item.PayType
	-- ִ�м����ڼ�
	, item.ExecutePeriod

	, item.*
from PAY_SalaryItem_Trl itemTrl
	inner join PAY_SalaryItem item
	on itemTrl.ID = item.ID
		and itemTrl.SysMLFlag = @SysMLFlag
where
	item.Code = @SalaryItemCode
	
