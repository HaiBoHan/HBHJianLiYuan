
if exists(select * from sys.objects where name='HBH_SP_JianLiYuan_SumPayroll')
-- 如果存在则删掉
	drop proc HBH_SP_JianLiYuan_SumPayroll
go
-- 创建存储过程
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
			,Memo varchar(max)	-- 备注
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

-- 清空所有 明细行
delete from [Cust_TotalPayrollDocLine]
where [TotalPayrollDoc] = @ID


--  数据源：发薪申请单；
-- 手工做  发薪申请单，然后点提交；
-- 汇总审批页面，点击 发薪汇总 ，然后生成开发的薪资申请汇总单，提交到领导审核，汇总单审核后，自动审核原有发薪申请单；
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
Approved	已核准	2
Approving	核准中	1
Cancelled	作废	4
Closed	关闭	5
Opened	开立	0
Rejected	拒绝	3
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

	-- 计算ID
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
		

		,PeopleNumber	-- 计薪人数
		,TotalOrigPay	-- 应发合计
		,TotalActPay	-- 实发合计
	)select 
		(@StartID + row_number() over (order by line.PayrollCaculate,line.Department) - 1)
		,1
		,head.CreatedBy
		,head.ModifiedBy
		,@Now
		,@Now

		,@ID
		,(row_number() over (order by Department) * 10)  as DocLineNo
		-- 部门
		,line.Department as Department
		-- 计薪期间
		,line.PayrollCaculate as PayrollCaculate
		-- 计薪方案
		,line.SalarySolution as SalarySolution

		-- 计薪人数
		,PeopleNumber
		-- 应发合计
		,TotalOrigPay
		-- 实发合
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



	-- 汇总数量



end






