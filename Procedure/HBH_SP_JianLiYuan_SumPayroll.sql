
if exists(select * from sys.objects where name='HBH_SP_JianLiYuan_SumPayroll')
-- ���������ɾ��
	drop proc HBH_SP_JianLiYuan_SumPayroll
go
-- �����洢����
create proc HBH_SP_JianLiYuan_SumPayroll  (
@ID bigint = -1

-- @PlanDate datetime = null
--,@ShipLineID bigint =-1
--,@LotCode varchar(125) = ''
--,@ItemSpec varchar(125) = ''
--,@SalesmanCode varchar(125) = ''
----,@IsAllSalesman smallint = 0
--,@IsFuzzySalesman smallint = 0
--,@IsContainBranchWh smallint = 0
--,@InvCategory bigint = -1
--,@ItemCode varchar(125) = ''
--,@ItemName varchar(125) = ''
----,@IsForceSalesman smallint = 0
--,@IsShowZeroQty smallint = 0
--,@Branch bigint = -1
)
with encryption
as
	--SET NOCOUNT ON;

	
declare @SysMlFlag varchar(11) = 'zh-CN'


if exists(select name from sys.objects where name = 'HBH_Debug_Param')
begin
	declare @Debugger bit = (select top 1 Debugger from HBH_Debug_Param where ProcName = 'HBH_SP_JianLiYuan_SumPayroll' or ProcName is null or ProcName = '' order by ProcName desc)
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
		select 'HBH_SP_JianLiYuan_SumPayroll','@ID',IsNull(cast(@ID as varchar(max)),'null'),GETDATE()
		-- select 'HBH_SP_JianLiYuan_SumPayroll','@PlanDate',IsNull(Convert(varchar,@PlanDate,120),'null'),GETDATE()
		--union select 'HBH_SP_JianLiYuan_SumPayroll','@IsCalcAll',IsNull(cast(@IsCalcAll as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_SumPayroll','ProcSql','exec HBH_SP_JianLiYuan_SumPayroll '
				+ IsNull('''' + cast(@ID as varchar(501)) + '''' ,'null')
				-- + IsNull('''' + Convert(varchar,@PlanDate,120) + '''' ,'null')
				--+ ',' + IsNull(cast(@IsCalcAll as varchar(501)),'null') 
			   ,GETDATE()
	end
end



	declare @PayrollTypeCode varchar(125) = 'JXLB'
	-- ����ǰ����
	declare @TransDeptBeforeCode varchar(125) = '092'
	-- ��������
	declare @TransDeptAfterCode varchar(125) = '093'
	-- ����ǰ���� = ����ǰӦ���ϼ�
	declare @TransDeptBeforeOrigPay varchar(125) = '111'
	-- ��������� = ������Ӧ���ϼ�
	declare @TransDeptAfterOrigPay varchar(125) = '112'
	-- ����ǰʵ���ϼ�
	declare @TransDeptBeforeActPay varchar(125) = '114'
	-- ������ʵ���ϼ�
	declare @TransDeptAfterActPay varchar(125) = '115'
	
	-- F����ǰ����
	declare @FTransDeptBeforeCode varchar(125) = 'F35'
	-- F��������
	declare @FTransDeptAfterCode varchar(125) = 'F36'
	-- F����ǰ�ϼ� = ����ǰӦ���ϼ�
	declare @FTransDeptBeforeOrigPay varchar(125) = 'F43'
	-- F������ϼ� = ������Ӧ���ϼ�
	declare @FTransDeptAfterOrigPay varchar(125) = 'F44'
	-- F����ǰʵ���ϼ�
	declare @FTransDeptBeforeActPay varchar(125) = 'F47'
	-- F������ʵ���ϼ�
	declare @FTransDeptAfterActPay varchar(125) = 'F48'

--	--declare @TransDeptBeforeCodeField varchar(125) = ''
--	--declare @TransDeptAfterCodeField varchar(125) = ''
--	--declare @TransDeptBeforeOrigPayField varchar(125) = ''
--	--declare @TransDeptAfterCodeField varchar(125) = ''


	-- �ֶΣ�
	-- ����ǰ����
	declare @TransDeptBeforeCodeField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @TransDeptBeforeCode)
	-- ��������
	declare @TransDeptAfterCodeField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @TransDeptAfterCode)
	-- ����ǰ���� = ����ǰӦ���ϼ�
	declare @TransDeptBeforeOrigPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @TransDeptBeforeOrigPay)
	-- ��������� = ������Ӧ���ϼ�
	declare @TransDeptAfterOrigPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @TransDeptAfterOrigPay)
	-- ����ǰʵ���ϼ�
	declare @TransDeptBeforeActPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @TransDeptBeforeActPay)
	-- ������ʵ���ϼ�
	declare @TransDeptAfterActPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @TransDeptAfterActPay)
	
	-- F����ǰ����
	declare @FTransDeptBeforeCodeField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @FTransDeptBeforeCode)
	-- F��������
	declare @FTransDeptAfterCodeField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @FTransDeptAfterCode)
	-- F����ǰ�ϼ� = ����ǰӦ���ϼ�
	declare @FTransDeptBeforeOrigPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @FTransDeptBeforeOrigPay)
	-- F������ϼ� = ������Ӧ���ϼ�
	declare @FTransDeptAfterOrigPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @FTransDeptAfterOrigPay)
	-- F����ǰʵ���ϼ�
	declare @FTransDeptBeforeActPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @FTransDeptBeforeActPay)
	-- F������ʵ���ϼ�
	declare @FTransDeptAfterActPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @FTransDeptAfterActPay)


	
	-- ���з��Ž�� = 109
	declare @BankPay varchar(125) = '109'
	declare @BankPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @BankPay)
	-- F���з��Ž�� = F31
	declare @FBankPay varchar(125) = 'F31'
	declare @FBankPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @FBankPay)
	-- �ֽ𷢷Ž�� = 110
	declare @CashPay varchar(125) = '110'
	declare @CashPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @CashPay)
	--  F�ֽ𷢷Ž�� = F32
	declare @FCashPay varchar(125) = 'F32'
	declare @FCashPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @FCashPay)
	-- �۷���� = 108
	declare @DeductPay varchar(125) = '108'
	declare @DeductPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @DeductPay)
	--  F�۷���� = F34
	declare @FDeductPay varchar(125) = 'F34'
	declare @FDeductPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @FDeductPay)
	-- Ӧ�����ʺϼ� = 121
	declare @GrossPay varchar(125) = '121'
	declare @GrossPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @GrossPay)
	--  FӦ�����ʺϼ� = F18
	declare @FGrossPay varchar(125) = 'F18'
	declare @FGrossPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @FGrossPay)
	-- ʵ���ϼ� = 005		��  087   115
	declare @ActualPay varchar(125) = '087' -- '005'
	declare @ActualPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @ActualPay)
	--  Fʵ���ϼ� = F22
	declare @FActualPay varchar(125) = 'F22'
	declare @FActualPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @FActualPay)



	declare @SysLineNo int = 10
	declare @Now datetime = GetDate();
	declare @CurDate datetime = Convert(date,@Now)
	declare @StartID bigint = -1
	declare @TotalIDCount int = 0
	declare @TotalLineCount int = 0
	declare @DetailLineCount int = 0
	declare @DefaultZero decimal(24,9) = 0
	
	select @SysLineNo=cast(isnull(b.Value,a.DefaultValue) as int)
	from Base_Profile a
	left join Base_ProfileValue b on b.Profile=a.ID 
	where Code='SysLineNo'

	select @CurDate = CheckInDate
	from [Cust_DayCheckIn]
	where ID = @ID

-- ������� ��ϸ��
delete from Cust_PayrollLineDetail
where PayrollLine in (select line.ID 
					from [Cust_TotalPayrollDocLine] as line
					where line.[TotalPayrollDoc] = @ID 
					)
;						
delete from [Cust_TotalPayrollDocLine]
where [TotalPayrollDoc] = @ID
;


--  ����Դ����н���뵥��
-- �ֹ���  ��н���뵥��Ȼ����ύ��
-- ��������ҳ�棬��� ��н���� ��Ȼ�����ɿ�����н��������ܵ����ύ���쵼��ˣ����ܵ���˺��Զ����ԭ�з�н���뵥��
If OBJECT_ID('tempdb..#hbh_tmp_TotalPayrollDocLine') is not null
	Drop Table #hbh_tmp_TotalPayrollDocLine

select
	payDetail.ID as PayDetailID
	,payHead.ID as PayHeadID
	,totalPay.ID as TotalPayrollDoc
	
	-- н������.����
	-- ,payDetail.Department as Department
	-- ��Ա.������
	,IsNull(dept.ID,-1) as Department
	-- ��н������н�ڼ�
	--,IsNull(payDetail.PayrollCaculate,-1) as PayrollCalculate
	-- 2017-10-14 wf ��ͬ��н�ڼ� �ϲ���������
	,-1 as PayrollCalculate
	-- ��н����
	-- ,IsNull(payCalc.SalarySolution,-1) as SalarySolution
	,-1  as SalarySolution

into #hbh_tmp_TotalPayrollDocLine
from [Cust_TotalPayrollDoc] totalPay

	--left join Base_ValueSetDef valueSet
	--on valueSet.Code = @PayrollTypeCode
	--left join Base_DefineValue defValue
	--on defValue.ValueSetDef = valueSet.ID
	--	and totalPay.PayrollType = defValue.ID


	inner join PAY_PayrollDoc payHead
	on totalPay.PayDate = payHead.PayDate

	inner join PAY_EmpPayroll payDetail
	on payHead.ID = payDetail.PayrollDoc
	left join Pay_PayrollCalculate payCalc
	on payDetail.PayrollCaculate = payCalc.ID
	
	inner join CBO_EmployeeArchive employee
	on payDetail.Employee = employee.ID
	-- inner join CBO_EmployCategory employeeCategory
	-- on employee.PersonCategory = employeeCategory.ID
	-- 	-- ������ܵ� ��н���� �ǿգ���ƥ��ȫ�� �� ����ƥ���Ӧ��
	-- 	and (totalPay.PayrollType is null 
	-- 		or defValue.Code = employeeCategory.DescFlexField_PrivateDescSeg1
	-- 		)
	
	left join CBO_Department srcDept
	on employee.Dept = srcDept.ID
	left join CBO_Department dept
	-- on employee.DescFlexField_PubDescSeg18 = dept.Code
    on srcDept.DescFlexField_PubDescSeg18 = dept.Code
		and dept.Org = employee.BusinessOrg
	
where totalPay.ID = @ID
	---- and defValue.Code = employee.DescFlexField_PrivateDescSeg1
	--and (
	--	-- employee.DescFlexField_PrivateDescSeg1 == '' 
	--	totalPay.PayrollType is null
	--	or defValue.ID is not null
	--	)
/*
Approved	�Ѻ�׼	2
Approving	��׼��	1
Cancelled	����	4
Closed	�ر�	5
Opened	����	0
Rejected	�ܾ�	3
*/
	--and payHead.Status in (2,5)
	and payHead.Status in (1)

	-- �� ��н��� ��Ϊ ��н��������
	and ',' + totalPay.PayrollCalculateIDs + ',' like '%,' + cast(payHead.PayrollCaculate as varchar(20)) + ',%'


/*   2015-12-06  ���Ÿ�Ϊ �ֲ�  ����ǰ���š��������ţ�

If OBJECT_ID('tempdb..#hbh_tmp_TotalLine') is not null
	Drop Table #hbh_tmp_TotalLine

select distinct tmpLine.TotalPayrollDoc
	,payDetail.Department as Department
	,IsNull(payDetail.PayrollCaculate,-1) as PayrollCaculate
	,IsNull(payCalc.SalarySolution,-1) as SalarySolution

	,IsNull(sum(payDetail.TotalOrigPay),0) as TotalOrigPay
	,IsNull(sum(payDetail.TotalActPay),0) as TotalActPay
	,count(payDetail.Employee) as PeopleNumber
into #hbh_tmp_TotalLine
from
	#hbh_tmp_TotalPayrollDocLine tmpLine
	inner join PAY_EmpPayroll payDetail
	on tmpLine.PayDetailID = payDetail.ID
	left join Pay_PayrollCalculate payCalc
	on payDetail.PayrollCaculate = payCalc.ID
group by tmpLine.TotalPayrollDoc
	,payDetail.Department
	,IsNull(payDetail.PayrollCaculate,-1)
	,IsNull(payCalc.SalarySolution,-1)



select @TotalLineCount = count(*) from #hbh_tmp_TotalLine
select @DetailLineCount = count(*)  from #hbh_tmp_TotalPayrollDocLine

	-- ����ID
	set @TotalIDCount = @TotalLineCount + @DetailLineCount
	

if(@TotalIDCount > 0)
begin

	-- select @StartID=0,@Count=@Count+1
	execute AllocSerials @TotalIDCount,@StartID output		
	

	insert into Cust_TotalPayrollDocLine
	(
		ID
		,SysVersion
		,CreatedBy
		,ModifiedBy
		,CreatedOn
		,ModifiedOn
		
		,TotalPayrollDoc
		,DocLineNo
		,Department
		,PayrollCalculate
		,SalarySolution
		

		,PeopleNumber	-- ��н����
		,TotalOrigPay	-- Ӧ���ϼ�
		,TotalActPay	-- ʵ���ϼ�
	)select 
		(@StartID + row_number() over (order by line.PayrollCaculate,line.Department) - 1)
		,1
		,head.CreatedBy
		,head.ModifiedBy
		,@Now
		,@Now

		,@ID
		,(row_number() over (order by Department) * 10) --  as DocLineNo
		-- ����
		,line.Department
		-- ��н�ڼ�
		,line.PayrollCaculate
		-- ��н����
		,line.SalarySolution

		-- ��н����
		,line.PeopleNumber
		-- Ӧ���ϼ�
		,line.TotalOrigPay
		-- ʵ����
		,line.TotalActPay
	from #hbh_tmp_TotalLine line
		inner join [Cust_TotalPayrollDoc] head
		on line.TotalPayrollDoc = head.ID

	

	insert into Cust_PayrollLineDetail
	(
		ID
		,SysVersion
		,CreatedBy
		,ModifiedBy
		,CreatedOn
		,ModifiedOn

		,PayrollLine
		,TotalPayrollDocLine
	)select 
		(@StartID + @TotalLineCount + row_number() over (order by line.PayHeadID,line.PayDetailID) - 1)
		,1
		,head.CreatedBy
		,head.ModifiedBy
		,@Now
		,@Now

		,line.PayDetailID
		,TotalPayrollDocLine = (select top 1 payLine.ID from Cust_TotalPayrollDocLine payLine
								where payLine.TotalPayrollDoc = @ID
									and exists(select 1 from PAY_EmpPayroll payDetail
											where line.PayDetailID = payDetail.ID
												and payDetail.Department = payLine.Department
												and payDetail.PayrollCaculate = payLine.PayrollCalculate
											)
								)
	from #hbh_tmp_TotalPayrollDocLine line
		inner join [Cust_TotalPayrollDoc] head
		on line.TotalPayrollDoc = head.ID


	-- ��������



end

*/



--select @TransDeptBeforeCodeField= PayrollField
--from Pay_SalaryItem
--where Code = @TransDeptBeforeCode

--select @TransDeptAfterCodeField= PayrollField
--from Pay_SalaryItem
--where Code = @TransDeptAfterCode

--if(@TransDeptBeforeCodeFieldis null or @TransDeptBeforeCodeField= '')
--	throw 

--if(@TransDeptAfterCodeFieldis null or @TransDeptAfterCodeField= '')
--	throw 


If OBJECT_ID('tempdb..#hbh_tmp_TotalPayrollDocLine_Dept') is not null
	Drop Table #hbh_tmp_TotalPayrollDocLine_Dept

create table #hbh_tmp_TotalPayrollDocLine_Dept
(
	 PayDetailID bigint
	,PayHeadID bigint
	,TotalPayrollDoc bigint
	
	,Department bigint
	,PayrollCalculate bigint
	,SalarySolution bigint

	,BeforeDeptName varchar(125)
	,AfterDeptName varchar(125)
	,BeforeDeptOrigPay decimal(24,9)
	,AfterDeptOrigPay decimal(24,9)
	,BeforeDeptActPay decimal(24,9)
	,AfterDeptActPay decimal(24,9)
	
	,FBeforeDeptName varchar(125)
	,FAfterDeptName varchar(125)
	,FBeforeDeptOrigPay decimal(24,9)
	,FAfterDeptOrigPay decimal(24,9)
	,FBeforeDeptActPay decimal(24,9)
	,FAfterDeptActPay decimal(24,9)
	
	-- ���з��Ž�� -- = 109
	,BankPay decimal(24,9) -- = '109'
	-- F���з��Ž�� -- = F31
	,FBankPay decimal(24,9) -- = 'F31'
	-- �ֽ𷢷Ž�� -- = 110
	,CashPay decimal(24,9) -- = '110'
	--  F�ֽ𷢷Ž�� -- = F32
	,FCashPay decimal(24,9) -- = 'F32'
	-- �۷���� -- = 108
	,DeductPay decimal(24,9) -- = '108'
	--  F�۷���� -- = F34
	,FDeductPay decimal(24,9) -- = 'F34'
	-- Ӧ�����ʺϼ� -- = 121
	,GrossPay decimal(24,9) -- = '121'
	--  FӦ�����ʺϼ� -- = F18
	,FGrossPay decimal(24,9) -- = 'F18'
	-- ʵ���ϼ� -- = 005
	,ActualPay decimal(24,9) -- = '005'
	--  Fʵ���ϼ� -- = F22
	,FActualPay decimal(24,9) -- = 'F22'

	-- ���뵥Ӧ��
	,TotalOrigPay decimal(24,9)
	-- ���뵥ʵ��
	,TotalActPay decimal(24,9)
)

--declare @DeptSql varchar(max) = 'insert into #hbh_tmp_TotalPayrollDocLine_Dept
--select tmpLine.*
--	,payResult.' + @TransDeptBeforeCodeField+ ' as BeforeDeptName,payResult.' + @TransDeptAfterCodeField+ ' as AfterDeptName	
--	,dbo.HBH_Fn_GetDecimal(payDetail.' + @TransDeptBeforeOrigPayField + ',0) as BeforeDeptOrigPay,dbo.HBH_Fn_GetDecimal(payDetail.' + @TransDeptAfterOrigPayField + ',0) as AfterDeptOrigPay
--	,dbo.HBH_Fn_GetDecimal(payDetail.' + @TransDeptBeforeActPayField + ',0) as BeforeDeptActPay,dbo.HBH_Fn_GetDecimal(payDetail.' + @TransDeptAfterActPayField + ',0) as AfterDeptActPay
--	,payResult.' + @FTransDeptBeforeCodeField + ' as FBeforeDeptName,payResult.' + @FTransDeptAfterCodeField + ' as FAfterDeptName
--	,dbo.HBH_Fn_GetDecimal(payDetail.' + @FTransDeptBeforeOrigPayField + ',0) as FBeforeDeptOrigPay,dbo.HBH_Fn_GetDecimal(payDetail.' + @FTransDeptAfterOrigPayField + ',0) as FAfterDeptOrigPay
--	,dbo.HBH_Fn_GetDecimal(payDetail.' + @FTransDeptBeforeActPayField + ',0) as FBeforeDeptActPay,dbo.HBH_Fn_GetDecimal(payDetail.' + @FTransDeptAfterActPayField + ',0) as FAfterDeptActPay
--from #hbh_tmp_TotalPayrollDocLine tmpLine
--	inner join PAY_EmpPayroll payDetail
--	on tmpLine.PayDetailID = payDetail.ID
--	left join Pay_PayrollResult payResult
--	on payDetail.PayrollResult = payResult.ID
--	'
	
declare @DeptSql varchar(max) = 'insert into #hbh_tmp_TotalPayrollDocLine_Dept
select tmpLine.*
	,payResult.' + @TransDeptBeforeCodeField+ ' as BeforeDeptName,payResult.' + @TransDeptAfterCodeField+ ' as AfterDeptName	
	,dbo.HBH_Fn_GetDecimal(payDetail.' + @TransDeptBeforeOrigPayField + ',0) as BeforeDeptOrigPay,dbo.HBH_Fn_GetDecimal(payDetail.' + @TransDeptAfterOrigPayField + ',0) as AfterDeptOrigPay
	,dbo.HBH_Fn_GetDecimal(payDetail.' + @TransDeptBeforeActPayField + ',0) as BeforeDeptActPay,dbo.HBH_Fn_GetDecimal(payDetail.' + @TransDeptAfterActPayField + ',0) as AfterDeptActPay
	,payResult.' + @FTransDeptBeforeCodeField + ' as FBeforeDeptName,payResult.' + @FTransDeptAfterCodeField + ' as FAfterDeptName
	,dbo.HBH_Fn_GetDecimal(payDetail.' + @FTransDeptBeforeOrigPayField + ',0) as FBeforeDeptOrigPay,dbo.HBH_Fn_GetDecimal(payDetail.' + @FTransDeptAfterOrigPayField + ',0) as FAfterDeptOrigPay
	,dbo.HBH_Fn_GetDecimal(payDetail.' + @FTransDeptBeforeActPayField + ',0) as FBeforeDeptActPay,dbo.HBH_Fn_GetDecimal(payDetail.' + @FTransDeptAfterActPayField + ',0) as FAfterDeptActPay

	,dbo.HBH_Fn_GetDecimal(payResult.' + @BankPayField  + ',0) as BankPay ,dbo.HBH_Fn_GetDecimal(payResult.' + @FBankPayField  + ',0) as FBankPay 
	,dbo.HBH_Fn_GetDecimal(payResult.' + @CashPayField  + ',0) as CashPay ,dbo.HBH_Fn_GetDecimal(payResult.' + @FCashPayField  + ',0) as FCashPay 
	,dbo.HBH_Fn_GetDecimal(payResult.' + @DeductPayField  + ',0) as DeductPay ,dbo.HBH_Fn_GetDecimal(payResult.' + @FDeductPayField  + ',0) as FDeductPay 
	,dbo.HBH_Fn_GetDecimal(payResult.' + @GrossPayField  + ',0) as GrossPay ,dbo.HBH_Fn_GetDecimal(payResult.' + @FGrossPayField  + ',0) as FGrossPay 
	,dbo.HBH_Fn_GetDecimal(payResult.' + @ActualPayField  + ',0) as ActualPay  ,dbo.HBH_Fn_GetDecimal(payResult.' + @FActualPayField  + ',0) as FActualPay  
	,payDetail.TotalOrigPay as TotalOrigPay
	-- ,payDetail.TotalActPay as TotalActPay
	-- 2017-10-14 wf �����֣������ȡϵͳ��ʵ����ȡ ʵ��+Fʵ����ԭ���������м�����û˰������ϵͳ�������˰
	-- ������(�ൺ����Դ)  11:50:10      �ϼƱ��е�ʵ�����ʣ��͵���087��F22�ĺϼƾͿ�����
	,TotalActPay = dbo.HBH_Fn_GetDecimal(payResult.' + @ActualPayField  + ',0) + dbo.HBH_Fn_GetDecimal(payResult.' + @FActualPayField  + ',0)
from #hbh_tmp_TotalPayrollDocLine tmpLine
	inner join PAY_EmpPayroll payDetail
	on tmpLine.PayDetailID = payDetail.ID
	left join Pay_PayrollResult payResult
	on payDetail.PayrollResult = payResult.ID
	'
	
--print(@DeptSql)
exec (@DeptSql)
--declare @StrZero varchar(125) = N'0'
---- N'cast(0 as decimal(24,9))'
--exec sp_executesql @DeptSql,N'@DefaultZero varchar(125)',@StrZero


If OBJECT_ID('tempdb..#hbh_tmp_TotalLine') is not null
	Drop Table #hbh_tmp_TotalLine

select 
	TotalPayrollDoc
	,Department
	,DepartmentName
	,PayrollCalculate
	,SalarySolution
	
	----,sum(IsNull(TotalOrigPay,0)) as TotalOrigPay
	----,sum(IsNull(TotalActPay,0)) as TotalActPay
	----,sum(IsNull(FTotalOrigPay,0)) as FTotalOrigPay
	----,sum(IsNull(FTotalActPay,0)) as FTotalActPay
	
	--,sum(IsNull(BeforeDeptTotalOrigPay,0)) as BeforeDeptTotalOrigPay
	--,sum(IsNull(AfterDeptTotalOrigPay,0)) as AfterDeptTotalOrigPay
	--,sum(IsNull(SystemTotalOrigPay,0)) as SystemTotalOrigPay
	--,sum(IsNull(BeforeDeptTotalActPay,0)) as BeforeDeptTotalActPay
	--,sum(IsNull(AfterDeptTotalActPay,0)) as AfterDeptTotalActPay
	--,sum(IsNull(SystemTotalActPay,0)) as SystemTotalActPay
	--,sum(IsNull(FBeforeDeptTotalOrigPay,0)) as FBeforeDeptTotalOrigPay
	--,sum(IsNull(FAfterDeptTotalOrigPay,0)) as FAfterDeptTotalOrigPay
	--,sum(IsNull(FBeforeDeptTotalActPay,0)) as FBeforeDeptTotalActPay
	--,sum(IsNull(FAfterDeptTotalActPay,0)) as FAfterDeptTotalActPay
	
	----,sum(IsNull(TotalOrigPay,0)) as TotalOrigPay
	----,sum(IsNull(TotalActPay,0)) as TotalActPay
	
	,sum(IsNull(BankPay ,0)) as BankPay
	,sum(IsNull(FBankPay ,0)) as FBankPay
	,sum(IsNull(CashPay ,0)) as CashPay 
	,sum(IsNull(FCashPay ,0)) as FCashPay 
	,sum(IsNull(DeductPay ,0)) as DeductPay 
	,sum(IsNull(FDeductPay ,0)) as FDeductPay 
	,sum(IsNull(GrossPay ,0)) as GrossPay 
	,sum(IsNull(FGrossPay ,0)) as FGrossPay 
	,sum(IsNull(ActualPay ,0)) as ActualPay 
	,sum(IsNull(FActualPay ,0)) as FActualPay 

	,sum(IsNull(TotalOrigPay ,0)) as TotalOrigPay 
	,sum(IsNull(TotalActPay ,0)) as TotalActPay 

into #hbh_tmp_TotalLine
from (
	--select tmpLine.TotalPayrollDoc
	--	-- ,payDetail.Department as Department
	--	,deptTrl.ID as Department
	--	,deptTrl.Name as DepartmentName
	--	,IsNull(payDetail.PayrollCaculate,-1) as PayrollCalculate
	--	,IsNull(payCalc.SalarySolution,-1) as SalarySolution

	--	--,IsNull(sum(payDetail.TotalOrigPay),0) as TotalOrigPay
	--	--,IsNull(sum(payDetail.TotalActPay),0) as TotalActPay

	--	,case when tmpLine.BeforeDeptName = deptTrl.Name then tmpLine.BeforeDeptOrigPay
	--		else 0 end
	--		 as BeforeDeptTotalOrigPay
	--	,case when tmpLine.AfterDeptName = deptTrl.Name then tmpLine.AfterDeptOrigPay
	--		else 0 end
	--		 as AfterDeptTotalOrigPay
	--	-- ʣ��ȡϵͳ���
	--	,case when tmpLine.BeforeDeptName != deptTrl.Name and tmpLine.AfterDeptName != deptTrl.Name 
	--			then payDetail.TotalOrigPay
	--		else 0 end
	--		 as SystemTotalOrigPay
			 
	--	,case when tmpLine.BeforeDeptName = deptTrl.Name then tmpLine.BeforeDeptActPay
	--		else 0 end
	--		 as BeforeDeptTotalActPay
	--	,case when  tmpLine.AfterDeptName = deptTrl.Name then tmpLine.AfterDeptActPay
	--		else 0 end
	--		 as AfterDeptTotalActPay
	--	-- ʣ��ȡϵͳ���
	--	,case when tmpLine.BeforeDeptName != deptTrl.Name and tmpLine.AfterDeptName != deptTrl.Name 
	--			then payDetail.TotalActPay
	--		else 0 end
	--		 as SystemTotalActPay

	--	,0 as FBeforeDeptTotalOrigPay
	--	,0 as FAfterDeptTotalOrigPay
	--	,0 as FBeforeDeptTotalActPay
	--	,0 as FAfterDeptTotalActPay

	--	-- ,count(payDetail.Employee) as PeopleNumber
	--from
	--	#hbh_tmp_TotalPayrollDocLine_Dept tmpLine
	--	inner join PAY_EmpPayroll payDetail
	--	on tmpLine.PayDetailID = payDetail.ID
	--	left join Pay_PayrollCalculate payCalc
	--	on payDetail.PayrollCaculate = payCalc.ID

	--	inner join CBO_Department_Trl deptTrl
	--	on (tmpLine.BeforeDeptName = deptTrl.Name
	--		or tmpLine.AfterDeptName = deptTrl.Name
	--		-- û�е���ǰ��F����ǰ���ţ���ȡϵͳ����
	--		or (IsNull(tmpLine.BeforeDeptName,'') = '' and IsNull(tmpLine.AfterDeptName,'') = '' 
	--			and (IsNull(tmpLine.FBeforeDeptName,'') = '' or IsNull(tmpLine.FBeforeDeptName,'') = '0')
	--			and (IsNull(tmpLine.FAfterDeptName,'') = '' or IsNull(tmpLine.FAfterDeptName,'') = '0')
	--			and payDetail.Department = deptTrl.ID
	--			 )
	--		)
	--union all	
	--select tmpLine.TotalPayrollDoc
	--	-- ,payDetail.Department as Department
	--	,deptTrl.ID as Department
	--	,deptTrl.Name as DepartmentName
	--	,IsNull(payDetail.PayrollCaculate,-1) as PayrollCalculate
	--	,IsNull(payCalc.SalarySolution,-1) as SalarySolution

	--	--,IsNull(sum(payDetail.TotalOrigPay),0) as TotalOrigPay
	--	--,IsNull(sum(payDetail.TotalActPay),0) as TotalActPay
		
	--	,0 as BeforeDeptTotalOrigPay
	--	,0 as AfterDeptTotalOrigPay
	--	,0 as SystemTotalOrigPay
	--	,0 as BeforeDeptTotalActPay
	--	,0 as AfterDeptTotalActPay
	--	,0 as SystemTotalActPay

	--	,case when tmpLine.FBeforeDeptName = deptTrl.Name then tmpLine.FBeforeDeptOrigPay
	--		-- else TotalOrigPay end
	--		else 0 end
	--		 as FBeforeDeptTotalOrigPay
	--	,case when tmpLine.FAfterDeptName = deptTrl.Name then tmpLine.FAfterDeptOrigPay
	--		-- else TotalOrigPay end
	--		else 0 end
	--		 as FAfterDeptTotalOrigPay
	--	,case when tmpLine.FBeforeDeptName = deptTrl.Name then tmpLine.FBeforeDeptActPay
	--		-- else TotalActPay end
	--		else 0 end
	--		 as FBeforeDeptTotalActPay
	--	,case when tmpLine.FAfterDeptName = deptTrl.Name then tmpLine.FAfterDeptActPay
	--		-- else TotalActPay end
	--		else 0 end
	--		 as FAfterDeptTotalActPay

	--	--,count(payDetail.Employee) as PeopleNumber
	--from
	--	#hbh_tmp_TotalPayrollDocLine_Dept tmpLine
	--	inner join PAY_EmpPayroll payDetail
	--	on tmpLine.PayDetailID = payDetail.ID
	--	left join Pay_PayrollCalculate payCalc
	--	on payDetail.PayrollCaculate = payCalc.ID

	--	inner join CBO_Department_Trl deptTrl
	--	on (tmpLine.FBeforeDeptName = deptTrl.Name
	--		or tmpLine.FAfterDeptName = deptTrl.Name
	--		--or (IsNull(tmpLine.BeforeDeptName,'') = '' and IsNull(tmpLine.AfterDeptName,'') = ''
	--		--	and payDetail.Department = deptTrl.ID
	--		--	 )
	--		)
	--	inner join CBO_Department dept
	--	on deptTrl.ID = dept.ID
	--		and dept.Org = payDetail.BusinessOrg

	
	select tmpLine.TotalPayrollDoc
		-- ,payDetail.Department as Department
		-- ,deptTrl.ID as Department
		,tmpLine.Department as Department
		,deptTrl.Name as DepartmentName
		--,IsNull(payDetail.PayrollCaculate,-1) as PayrollCalculate
		,IsNull(tmpLine.PayrollCalculate,-1) as PayrollCalculate
		--,IsNull(payCalc.SalarySolution,-1) as SalarySolution
		,IsNull(tmpLine.SalarySolution,-1) as SalarySolution
				
		,(IsNull(tmpLine.BankPay ,0)) as BankPay
		,(IsNull(tmpLine.FBankPay ,0)) as FBankPay
		,(IsNull(tmpLine.CashPay ,0)) as CashPay 
		,(IsNull(tmpLine.FCashPay ,0)) as FCashPay 
		,(IsNull(tmpLine.DeductPay ,0)) as DeductPay 
		,(IsNull(tmpLine.FDeductPay ,0)) as FDeductPay 
		,(IsNull(tmpLine.GrossPay ,0)) as GrossPay 
		,(IsNull(tmpLine.FGrossPay ,0)) as FGrossPay 
		,(IsNull(tmpLine.ActualPay ,0)) as ActualPay 
		,(IsNull(tmpLine.FActualPay ,0)) as FActualPay 
		
		,(IsNull(tmpLine.TotalOrigPay ,0)) as TotalOrigPay 
		,(IsNull(tmpLine.TotalActPay ,0)) as TotalActPay 

	from
		#hbh_tmp_TotalPayrollDocLine_Dept tmpLine
		inner join PAY_EmpPayroll payDetail
		on tmpLine.PayDetailID = payDetail.ID
		left join Pay_PayrollCalculate payCalc
		on payDetail.PayrollCaculate = payCalc.ID
		
		left join CBO_Department_Trl deptTrl
		-- on payDetail.Department = deptTrl.ID
		-- ��Ϊ������
		on tmpLine.Department = deptTrl.ID

	) as totalPay
group by TotalPayrollDoc
	,Department
	,DepartmentName
	,PayrollCalculate
	,SalarySolution




select @TotalLineCount = count(*) from #hbh_tmp_TotalLine

select @DetailLineCount = count(*)  
from #hbh_tmp_TotalLine totalLine 
	inner join #hbh_tmp_TotalPayrollDocLine_Dept detailLine
	on detailLine.SalarySolution = totalLine.SalarySolution
		and detailLine.PayrollCalculate = totalLine.PayrollCalculate
		-- and detailLine.Department = totalLine.Department
	
		---- ����
		--and (detailLine.BeforeDeptName = totalLine.DepartmentName
		--	or detailLine.AfterDeptName = totalLine.DepartmentName
		--	or detailLine.FBeforeDeptName = totalLine.DepartmentName
		--	or detailLine.FAfterDeptName = totalLine.DepartmentName
		--	-- û�е���ǰ��F����ǰ���ţ���ȡϵͳ����
		--	or (IsNull(detailLine.BeforeDeptName,'') = '' and IsNull(detailLine.AfterDeptName,'') = '' 
		--		-- and IsNull(detailLine.FBeforeDeptName,'') = '' and IsNull(detailLine.FAfterDeptName,'') = ''
		--		and (IsNull(detailLine.FBeforeDeptName,'') = '' or IsNull(detailLine.FBeforeDeptName,'') = '0')
		--		and (IsNull(detailLine.FAfterDeptName,'') = '' or IsNull(detailLine.FAfterDeptName,'') = '0')
		--		and detailLine.Department = totalLine.Department
		--		 )
		--	)

	-- ����ID
	set @TotalIDCount = @TotalLineCount + @DetailLineCount
	

if(@TotalIDCount > 0)
begin

	-- select @StartID=0,@Count=@Count+1
	execute AllocSerials @TotalIDCount,@StartID output		
	

	insert into Cust_TotalPayrollDocLine
	(
		ID
		,SysVersion
		,CreatedBy
		,ModifiedBy
		,CreatedOn
		,ModifiedOn
		
		,TotalPayrollDoc
		,DocLineNo
		,Department
		,DepartmentName
		,PayrollCalculate
		,SalarySolution
		
		
		--,BeforeDeptTotalOrigPay
		--,AfterDeptTotalOrigPay
		--,SystemTotalOrigPay
		--,BeforeDeptTotalActPay
		--,AfterDeptTotalActPay
		--,SystemTotalActPay
		--,FBeforeDeptTotalOrigPay
		--,FAfterDeptTotalOrigPay
		--,FBeforeDeptTotalActPay
		--,FAfterDeptTotalActPay

		,TotalOrigPay	-- Ӧ���ϼ�
		,TotalActPay	-- ʵ���ϼ�
		,PeopleNumber	-- ��н����

		
		-- ���з��Ž�� -- = 109
		,BankPay
		-- F���з��Ž�� -- = F31
		,FBankPay
		-- �ֽ𷢷Ž�� -- = 110
		,CashPay
		--  F�ֽ𷢷Ž�� -- = F32
		,FCashPay
		-- �۷���� -- = 108
		,DeductPay
		--  F�۷���� -- = F34
		,FDeductPay
		-- Ӧ�����ʺϼ� -- = 121
		,GrossPay
		--  FӦ�����ʺϼ� -- = F18
		,FGrossPay
		-- ʵ���ϼ� -- = 005
		,ActualPay
		--  Fʵ���ϼ� -- = F22
		,FActualPay

		-- ����ʵ��
		,BankTotalActPay
		-- �ֽ�ʵ��
		,CashTotalActPay
		-- �۷��ϼ�
		,WithholdingTotalActPay

	)select 
		(@StartID + row_number() over (order by totalLine.PayrollCalculate,totalLine.Department) - 1)
		,1
		,head.CreatedBy
		,head.ModifiedBy
		,@Now
		,@Now

		,@ID
		-- ����ƽ�������:�кſɷ�ĳ�1��2��3��4����
		--,(row_number() over (order by totalLine.PayrollCalculate,totalLine.Department) * 10) --  as DocLineNo
		,(row_number() over (order by totalLine.PayrollCalculate,totalLine.Department)) --  as DocLineNo
		-- ����
		,totalLine.Department
		,totalLine.DepartmentName
		-- ��н�ڼ�
		,totalLine.PayrollCalculate
		-- ��н����
		,totalLine.SalarySolution

		
		--,totalLine.BeforeDeptTotalOrigPay
		--,totalLine.AfterDeptTotalOrigPay
		--,totalLine.SystemTotalOrigPay
		--,totalLine.BeforeDeptTotalActPay
		--,totalLine.AfterDeptTotalActPay
		--,totalLine.SystemTotalActPay
		--,totalLine.FBeforeDeptTotalOrigPay
		--,totalLine.FAfterDeptTotalOrigPay
		--,totalLine.FBeforeDeptTotalActPay
		--,totalLine.FAfterDeptTotalActPay

		-- Ӧ���ϼ�
		--,totalLine.BeforeDeptTotalOrigPay + totalLine.AfterDeptTotalOrigPay + totalLine.SystemTotalOrigPay + totalLine.FBeforeDeptTotalOrigPay +totalLine.FAfterDeptTotalOrigPay
		--	 as TotalOrigPay
		-- ��β���Ϊȡ ���뵥
		--,TotalOrigPay = GrossPay + FGrossPay
		,TotalOrigPay = TotalOrigPay
		
		-- ʵ���ϼ�
		--,totalLine.BeforeDeptTotalActPay + totalLine.AfterDeptTotalActPay + totalLine.SystemTotalActPay + totalLine.FBeforeDeptTotalActPay + totalLine.FAfterDeptTotalActPay
		--	as TotalActPay
		-- ��β���Ϊȡ ���뵥
		--,TotalActPay = ActualPay + FActualPay
		,TotalActPay = TotalActPay 
		-- ��н����
		,(select count(*) from #hbh_tmp_TotalPayrollDocLine_Dept detailLine
					where detailLine.SalarySolution = totalLine.SalarySolution
						and detailLine.PayrollCalculate = totalLine.PayrollCalculate
						-- ����
						and detailLine.Department = totalLine.Department
	
						---- ����
						--and (detailLine.BeforeDeptName = totalLine.DepartmentName
						--	or detailLine.AfterDeptName = totalLine.DepartmentName
						--	or detailLine.FBeforeDeptName = totalLine.DepartmentName
						--	or detailLine.FAfterDeptName = totalLine.DepartmentName
						--	-- û�е���ǰ��F����ǰ���ţ���ȡϵͳ����
						--	or (
						--		IsNull(detailLine.BeforeDeptName,'') = '' and IsNull(detailLine.AfterDeptName,'') = '' 
						--		and (IsNull(detailLine.FBeforeDeptName,'') = '' or IsNull(detailLine.FBeforeDeptName,'') = '0')
						--		and (IsNull(detailLine.FAfterDeptName,'') = '' or IsNull(detailLine.FAfterDeptName,'') = '0')
						--		and detailLine.Department = totalLine.Department
						--			)
						--	)
				) as PeopleNumber
		
		
		-- ���з��Ž�� -- = 109
		,BankPay
		-- F���з��Ž�� -- = F31
		,FBankPay
		-- �ֽ𷢷Ž�� -- = 110
		,CashPay
		--  F�ֽ𷢷Ž�� -- = F32
		,FCashPay
		-- �۷���� -- = 108
		,DeductPay
		--  F�۷���� -- = F34
		,FDeductPay
		-- Ӧ�����ʺϼ� -- = 121
		,GrossPay
		--  FӦ�����ʺϼ� -- = F18
		,FGrossPay
		-- ʵ���ϼ� -- = 005
		,ActualPay
		--  Fʵ���ϼ� -- = F22
		,FActualPay

		-- ����ʵ��
		,BankTotalActPay = BankPay + FBankPay
		-- �ֽ�ʵ��
		,CashTotalActPay = CashPay + FCashPay
		-- �۷��ϼ�
		,WithholdingTotalActPay = DeductPay + FDeductPay

	from #hbh_tmp_TotalLine totalLine
		inner join [Cust_TotalPayrollDoc] head
		on totalLine.TotalPayrollDoc = head.ID

	

	insert into Cust_PayrollLineDetail
	(
		ID
		,SysVersion
		,CreatedBy
		,ModifiedBy
		,CreatedOn
		,ModifiedOn

		,PayrollLine
		,TotalPayrollDocLine
	)select 
		(@StartID + @TotalLineCount + row_number() over (order by detailLine.PayHeadID,detailLine.PayDetailID) - 1)
		,1
		,head.CreatedBy
		,head.ModifiedBy
		,@Now
		,@Now

		,detailLine.PayDetailID
		--,TotalPayrollDocLine = (select top 1 payLine.ID from Cust_TotalPayrollDocLine payLine
		--						where payLine.TotalPayrollDoc = @ID
		--							and exists(select 1 from PAY_EmpPayroll payDetail
		--									where line.PayDetailID = payDetail.ID
		--										and payDetail.PayrollCaculate = payLine.PayrollCalculate
		--										-- and payDetail.Department = payLine.Department

		--										-- ����
		--										and (detailLine.BeforeDeptName = totalLine,DepartmentName
		--											or detailLine.AfterDeptName = totalLine,DepartmentName
		--											or detailLine.FBeforeDeptName = totalLine,DepartmentName
		--											or detailLine.FAfterDeptName = totalLine,DepartmentName
		--											-- û�е���ǰ��F����ǰ���ţ���ȡϵͳ����
		--											or (IsNull(detailLine.BeforeDeptName,'') = '' and IsNull(detailLine.AfterDeptName,'') = '' and IsNull(detailLine.FBeforeDeptName,'') = '' and IsNull(detailLine.FAfterDeptName,'') = ''
		--												and payDetail.Department = totalLine.Department
		--												 )
		--											)
		--																			)
		--						)
		,TotalPayrollDocLine = totalLine.ID
	from 
		-- #hbh_tmp_TotalPayrollDocLine line
		#hbh_tmp_TotalPayrollDocLine_Dept detailLine
		inner join [Cust_TotalPayrollDoc] head
		on detailLine.TotalPayrollDoc = head.ID
		
		inner join Cust_TotalPayrollDocLine totalLine
		on detailLine.SalarySolution = totalLine.SalarySolution
			and detailLine.PayrollCalculate = totalLine.PayrollCalculate
			and detailLine.Department = totalLine.Department
			and head.ID = totalLine.TotalPayrollDoc
	
			---- ����
			--and (detailLine.BeforeDeptName = totalLine.DepartmentName
			--	or detailLine.AfterDeptName = totalLine.DepartmentName
			--	or detailLine.FBeforeDeptName = totalLine.DepartmentName
			--	or detailLine.FAfterDeptName = totalLine.DepartmentName
			--	-- û�е���ǰ��F����ǰ���ţ���ȡϵͳ����
			--	or (IsNull(detailLine.BeforeDeptName,'') = '' and IsNull(detailLine.AfterDeptName,'') = '' 
			--		-- and IsNull(detailLine.FBeforeDeptName,'') = '' and IsNull(detailLine.FAfterDeptName,'') = ''
			--		and (IsNull(detailLine.FBeforeDeptName,'') = '' or IsNull(detailLine.FBeforeDeptName,'') = '0')
			--		and (IsNull(detailLine.FAfterDeptName,'') = '' or IsNull(detailLine.FAfterDeptName,'') = '0')
			--		and detailLine.Department = totalLine.Department
			--		 )
			--	)

	-- ��������



end







--select 
--	@TransDeptBeforeCode as TransDeptBeforeCode
--	,@TransDeptBeforeCodeField as TransDeptBeforeCodeField
--	,@TransDeptAfterCode as TransDeptAfterCode
--	,@TransDeptAfterCodeField as TransDeptAfterCodeField
--	,@TransDeptBeforeOrigPay as TransDeptBeforeOrigPay
--	,@TransDeptBeforeOrigPayField as TransDeptBeforeOrigPayField
--	,@TransDeptAfterOrigPay as TransDeptAfterOrigPay
--	,@TransDeptAfterOrigPayField as TransDeptAfterOrigPayField
--	,@TransDeptBeforeActPay as TransDeptBeforeActPay
--	,@TransDeptBeforeActPayField as TransDeptBeforeActPayField
--	,@TransDeptAfterActPay as TransDeptAfterActPay
--	,@TransDeptAfterActPayField as TransDeptAfterActPayField
--	,@FTransDeptBeforeCode as FTransDeptBeforeCode
--	,@FTransDeptBeforeCodeField as FTransDeptBeforeCodeField
--	,@FTransDeptAfterCode as FTransDeptAfterCode
--	,@FTransDeptAfterCodeField as FTransDeptAfterCodeField
--	,@FTransDeptBeforeOrigPay as FTransDeptBeforeOrigPay
--	,@FTransDeptBeforeOrigPayField as FTransDeptBeforeOrigPayField
--	,@FTransDeptAfterOrigPay as FTransDeptAfterOrigPay
--	,@FTransDeptAfterOrigPayField as FTransDeptAfterOrigPayField
--	,@FTransDeptBeforeActPay as FTransDeptBeforeActPay
--	,@FTransDeptBeforeActPayField as FTransDeptBeforeActPayField
--	,@FTransDeptAfterActPay as FTransDeptAfterActPay
--	,@FTransDeptAfterActPayField as FTransDeptAfterActPayField
