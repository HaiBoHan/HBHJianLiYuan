/*

select distinct
	payHead.DocNo

from Cust_TotalPayrollDoc head
	inner join Cust_TotalPayrollDocLine line 
	on head.ID = line.TotalPayrollDoc
	inner join Cust_PayrollLineDetail subline
	on line.ID = subline.TotalPayrollDocLine


	inner join PAY_EmpPayroll payDetail
	on payDetail.ID = subline.PayrollLine
	inner join PAY_PayrollDoc payHead
	on payHead.ID = payDetail.PayrollDoc
	--on totalPay.PayDate = payHead.PayDate
	left join Pay_PayrollCalculate payCalc
	on payDetail.PayrollCaculate = payCalc.ID
where
	head.ID = 1001710130000800

*/

	-- 银行发放金额 = 109
	declare @BankPay varchar(125) = '109'
	declare @BankPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @BankPay)
	-- F银行发放金额 = F31
	declare @FBankPay varchar(125) = 'F31'
	declare @FBankPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @FBankPay)
	-- 现金发放金额 = 110
	declare @CashPay varchar(125) = '110'
	declare @CashPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @CashPay)
	--  F现金发放金额 = F32
	declare @FCashPay varchar(125) = 'F32'
	declare @FCashPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @FCashPay)
	-- 扣发金额 = 108
	declare @DeductPay varchar(125) = '108'
	declare @DeductPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @DeductPay)
	--  F扣发金额 = F34
	declare @FDeductPay varchar(125) = 'F34'
	declare @FDeductPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @FDeductPay)
	-- 应发工资合计 = 121
	declare @GrossPay varchar(125) = '121'
	declare @GrossPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @GrossPay)
	--  F应发工资合计 = F18
	declare @FGrossPay varchar(125) = 'F18'
	declare @FGrossPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @FGrossPay)
	-- 实发合计 = 005		，  087   115
	declare @ActualPay varchar(125) = '087' -- '005'
	declare @ActualPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @ActualPay)
	--  F实发合计 = F22
	declare @FActualPay varchar(125) = 'F22'
	declare @FActualPayField varchar(125) = (select PayrollField
from Pay_SalaryItem where Code = @FActualPay)


select @BankPay
	,@BankPayField
	,',IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.' + @BankPayField + ',0)),0) as [' + 'BankPay' + ']'
union select @FBankPay
	,@FBankPayField
	,',IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.' + @FBankPayField + ',0)),0) as [' + 'FBankPay' + ']'
union select @CashPay
	,@CashPayField
	,',IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.' + @CashPayField + ',0)),0) as [' + 'CashPay' + ']'
union select @FCashPay
	,@FCashPayField
	,',IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.' + @FCashPayField + ',0)),0) as [' + 'FCashPay' + ']'
union select @DeductPay
	,@DeductPayField
	,',IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.' + @DeductPayField + ',0)),0) as [' + 'DeductPay' + ']'
union select @FDeductPay
	,@FDeductPayField
	,',IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.' + @FDeductPayField + ',0)),0) as [' + 'FDeductPay' + ']'
union select @GrossPay
	,@GrossPayField
	,',IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.' + @GrossPayField + ',0)),0) as [' + 'GrossPay' + ']'
union select @FGrossPay
	,@FGrossPayField
	,',IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.' + @FGrossPayField + ',0)),0) as [' + 'FGrossPay' + ']'
union select @ActualPay
	,@ActualPayField
	,',IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.' + @ActualPayField + ',0)),0) as [' + 'ActualPay' + ']'
union select @FActualPay
	,@FActualPayField
	,',IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.' + @FActualPayField + ',0)),0) as [' + 'FActualPay' + ']'



select -- distinct
	--payHead.DocNo
	--,employee.EmployeeCode
	--,employee.Name
	--,payDetail.*
	IsNull(sum(payDetail.TotalOrigPay),0) as TotalOrigPay
	,IsNull(sum(payDetail.TotalActPay),0) as TotalActPay

	,IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.ExtField115,0)),0) as [ActualPay]
,IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.ExtField135,0)),0) as [DeductPay]
,IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.ExtField134,0)),0) as [BankPay]
,IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.ExtField133,0)),0) as [CashPay]
,IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.ExtField28,0)),0) as [GrossPay]
,IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.ExtField56,0)),0) as [FGrossPay]
,IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.ExtField61,0)),0) as [FActualPay]
,IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.ExtField80,0)),0) as [FBankPay]
,IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.ExtField81,0)),0) as [FCashPay]
,IsNull(sum(dbo.HBH_Fn_GetDecimal(payDetail.ExtField85,0)),0) as [FDeductPay]
from 
	--inner join PAY_PayrollDoc payHead
	--on totalPay.PayDate = payHead.PayDate
	PAY_PayrollDoc payHead
	inner join PAY_EmpPayroll payDetail
	on payHead.ID = payDetail.PayrollDoc
	--left join Pay_PayrollCalculate payCalc
	--on payDetail.PayrollCaculate = payCalc.ID
	
	--inner join CBO_EmployeeArchive employee
	--on payDetail.Employee = employee.ID

	--left join Cust_PayrollLineDetail subline
	--on payDetail.ID = subline.PayrollLine
	--left join Cust_TotalPayrollDocLine line 
	--on line.ID = subline.TotalPayrollDocLine
	----on head.ID = line.TotalPayrollDoc
	--	and line.TotalPayrollDoc = 1001710130000800

	
where payHead.DocNo in ('1'
,'PAY-00118'
,'PAY-00120'
,'PAY-00119'
)
	--and line.ID is not null




-- 查出来源行 实发汇总 != 实发 + F实发 的  （应发汇总相等）
select -- distinct
	payHead.DocNo
	,employee.EmployeeCode
	,employee.Name
	--,payDetail.*
	
	,payDetail.TotalActPay
	,CalcTotalActPay = (IsNull((dbo.HBH_Fn_GetDecimal(payDetail.ExtField115,0)),0)
									+ IsNull((dbo.HBH_Fn_GetDecimal(payDetail.ExtField61,0)),0)
									)
	,IsNull((dbo.HBH_Fn_GetDecimal(payDetail.ExtField115,0)),0) as [ActualPay]
	,IsNull((dbo.HBH_Fn_GetDecimal(payDetail.ExtField61,0)),0) as [FActualPay]
									
from 
	--inner join PAY_PayrollDoc payHead
	--on totalPay.PayDate = payHead.PayDate
	PAY_PayrollDoc payHead
	inner join PAY_EmpPayroll payDetail
	on payHead.ID = payDetail.PayrollDoc
	left join Pay_PayrollCalculate payCalc
	on payDetail.PayrollCaculate = payCalc.ID
	
	inner join CBO_EmployeeArchive employee
	on payDetail.Employee = employee.ID

	--left join Cust_PayrollLineDetail subline
	--on payDetail.ID = subline.PayrollLine
	--left join Cust_TotalPayrollDocLine line 
	--on line.ID = subline.TotalPayrollDocLine
	----on head.ID = line.TotalPayrollDoc
	--	and line.TotalPayrollDoc = 1001710130000800

	
where payHead.DocNo in ('1'
,'PAY-00118'
,'PAY-00120'
,'PAY-00119'
)
	--and line.ID is not null
	and payDetail.TotalActPay != (IsNull((dbo.HBH_Fn_GetDecimal(payDetail.ExtField115,0)),0)
									+ IsNull((dbo.HBH_Fn_GetDecimal(payDetail.ExtField61,0)),0)
									)