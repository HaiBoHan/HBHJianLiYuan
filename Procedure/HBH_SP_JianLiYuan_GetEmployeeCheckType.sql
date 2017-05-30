


if exists(select * from sys.objects where name='HBH_SP_JianLiYuan_GetEmployeeCheckType')
-- ���������ɾ��
	drop proc HBH_SP_JianLiYuan_GetEmployeeCheckType
go
-- �����洢����
create proc HBH_SP_JianLiYuan_GetEmployeeCheckType  (
@ID bigint = -1
)
with encryption
as
	SET NOCOUNT ON;

	
declare @SysMlFlag varchar(11) = 'zh-CN'
declare @ContractTypeDefCode varchar(125) = 'ht'


if exists(select name from sys.objects where name = 'HBH_Debug_Param')
begin
	declare @Debugger bit = (select top 1 Debugger from HBH_Debug_Param where ProcName = 'HBH_SP_JianLiYuan_GetEmployeeCheckType' or ProcName is null or ProcName = '' order by ProcName desc)
	if(@Debugger=1)
	begin	
		if not exists(select name from sys.objects where name = 'HBH_SPParamRecord')
		begin
			create table HBH_SPParamRecord
			(ProcName varchar(501)
			,ParamName varchar(501)
			,ParamValue varchar(max)
			,CreatedOn DateTime
			,Memo varchar(max)	-- ��ע
			)
		end

		insert into HBH_SPParamRecord
		(ProcName,ParamName,ParamValue,CreatedOn)
		select 'HBH_SP_JianLiYuan_GetEmployeeCheckType','@ID',IsNull(cast(@ID as varchar(max)),'null'),GETDATE()
		--union select 'HBH_SP_JianLiYuan_GetEmployeeCheckType','@IsCalcAll',IsNull(cast(@IsCalcAll as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_GetEmployeeCheckType','ProcSql','exec HBH_SP_JianLiYuan_GetEmployeeCheckType '
				+ IsNull('''' + cast(@ID as varchar(501)) + '''' ,'null')
				--+ ',' + IsNull(cast(@IsCalcAll as varchar(501)),'null') 
			   ,GETDATE()
	end
end



	declare @SysLineNo int = 10
	declare @Now datetime = GetDate();
	declare @CurDate datetime = Convert(date,@Now)
	declare @SheetName varchar(125)
	declare @StartID bigint = -1
	declare @TotalIDCount int = 0
	declare @LineCount int = 0



select	
	case when defValueTrl.Name is not null and defValueTrl.Name like '%��ȫ����%'
		then 1
		else 0
		end as CheckType
	,employee.ContractType
	,defValueTrl.Name as ContractTypeName

from
	(select 
	
		-- �Ͷ���ͬ����
		-- �����Ͷ���ͬ����ȡֵ��ʽ��ȡ�Ͷ���ͬ����3��ֵ������Ͷ���ͬ����3Ϊ�գ�ȡ�Ͷ���ͬ����2��ֵ������Ͷ���ͬ����2��ֵΪ�գ�ȡ�Ͷ���ͬ����1��ֵ��
		case when person.DescFlexField_PrivateDescSeg19 is not null and person.DescFlexField_PrivateDescSeg19 != ''
				then person.DescFlexField_PrivateDescSeg19
			when person.DescFlexField_PrivateDescSeg16 is not null and person.DescFlexField_PrivateDescSeg16 != ''
				then person.DescFlexField_PrivateDescSeg16
			when person.DescFlexField_PrivateDescSeg13 is not null and person.DescFlexField_PrivateDescSeg13 != ''
				then person.DescFlexField_PrivateDescSeg13
			end as ContractType
	from CBO_EmployeeArchive arch
		inner join CBO_Person person
		on person.ID = arch.Person

	where 
		arch.ID = @ID
	) as employee
	
	left join Base_ValueSetDef def
	on def.Code in (@ContractTypeDefCode)
	left join Base_DefineValue defValue
	on defValue.Code = employee.ContractType
	left join Base_DefineValue_Trl defValueTrl
	on defValue.ID = defValueTrl.ID



