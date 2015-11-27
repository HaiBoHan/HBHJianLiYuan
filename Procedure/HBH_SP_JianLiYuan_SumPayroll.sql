
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
	SET NOCOUNT ON;

	
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
		select 'HBH_SP_JianLiYuan_DepartImport','@ID',IsNull(cast(@ID as varchar(max)),'null'),GETDATE()
		-- select 'HBH_SP_JianLiYuan_SumPayroll','@PlanDate',IsNull(Convert(varchar,@PlanDate,120),'null'),GETDATE()
		--union select 'HBH_SP_JianLiYuan_SumPayroll','@IsCalcAll',IsNull(cast(@IsCalcAll as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_SumPayroll','ProcSql','exec HBH_SP_JianLiYuan_SumPayroll '
				+ IsNull('''' + cast(@ID as varchar(501)) + '''' ,'null')
				-- + IsNull('''' + Convert(varchar,@PlanDate,120) + '''' ,'null')
				--+ ',' + IsNull(cast(@IsCalcAll as varchar(501)),'null') 
			   ,GETDATE()
	end
end



	declare @SalePriceListCode varchar(125) = '001'
	declare @SysLineNo int = 10
	declare @Now datetime = GetDate();
	declare @CurDate datetime = GetDate()
	declare @SheetName varchar(125)
	declare @StartID bigint = -1
	declare @TotalIDCount int = 0
	declare @TotalLineCount int = 0
	declare @DetailLineCount int = 0
	
	select @SysLineNo=cast(isnull(b.Value,a.DefaultValue) as int)
	from Base_Profile a
	left join Base_ProfileValue b on b.Profile=a.ID 
	where Code='SysLineNo'

	select @CurDate = CheckInDate
	from [Cust_DayCheckIn]
	where ID = @ID

-- ������� ��ϸ��
delete from [Cust_TotalPayrollDocLine]
where [TotalPayrollDoc] = @ID


--  ����Դ����н���뵥��
-- �ֹ���  ��н���뵥��Ȼ����ύ��
-- ��������ҳ�棬��� ��н���� ��Ȼ�����ɿ�����н��������ܵ����ύ���쵼��ˣ����ܵ���˺��Զ����ԭ�з�н���뵥��
If OBJECT_ID('tempdb..#hbh_tmp_TotalPayrollDocLine') is not null
	Drop Table #hbh_tmp_TotalPayrollDocLine

select
	payDetail.ID as PayDetailID
	,payHead.ID as PayHeadID
	
into #hbh_tmp_TotalPayrollDocLine
from [Cust_TotalPayrollDoc] totalPay
	inner join PAY_PayrollDoc payHead
	on totalPay.PayDate = payHead.PayDate

	inner join PAY_EmpPayroll payDetail
	on payHead.ID = payDetail.PayrollDoc

where totalPay.ID = @ID
/*
Approved	�Ѻ�׼	2
Approving	��׼��	1
Cancelled	����	4
Closed	�ر�	5
Opened	����	0
Rejected	�ܾ�	3
*/
	and payHead.Status in (2,5)



If OBJECT_ID('tempdb..#hbh_tmp_TotalLine') is not null
	Drop Table #hbh_tmp_TotalLine

select distinct
	payDetail.Department as Department
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
group by
	payDetail.Department
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
		,(row_number() over (order by Department) * 10)  as DocLineNo
		-- ����
		,line.Department as Department
		-- ��н�ڼ�
		,line.PayrollCaculate as PayrollCaculate
		-- ��н����
		,line.SalarySolution as SalarySolution

		-- ��н����
		,PeopleNumber
		-- Ӧ���ϼ�
		,TotalOrigPay
		-- ʵ����
		,TotalActPay
	from #hbh_tmp_TotalLine line

	

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
		(@StartID + @TotalLineCount + row_number() over (order by line.PayrollCaculate,line.Department) - 1)
		,1
		,head.CreatedBy
		,head.ModifiedBy
		,@Now
		,@Now

		,line.ID
		,TotalPayrollDocLine = (select top 1 payLine.ID from Cust_TotalPayrollDocLine payLine
								where payLine.TotalPayrollDoc = @ID
									and exists(select 1 from PAY_EmpPayroll payDetail
											where line.PayDetailID = payDetail.ID
												and payDetail.Department = payLine.Department
												and payDetail.PayrollCaculate = payLine.PayrollCaculate
											)
								)
	from #hbh_tmp_TotalPayrollDocLine line



	-- ��������



end






