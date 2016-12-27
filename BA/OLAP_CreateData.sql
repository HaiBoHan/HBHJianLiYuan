



declare @YearStart int = 2016
declare @YearEnd int = 2050
declare @MonthStart int = 1
declare @MonthEnd int = 12
declare @YearMonthStart int = 201606

declare @CurrentYear int = @YearStart
declare @CurrentMonth int = 0

/*
truncate table Dim_U9_MonthFilter
*/
if not exists(select 1 from Dim_U9_MonthFilter)
begin
	while @CurrentYear <= @YearEnd
	begin
		if(@CurrentMonth < @MonthEnd)
		begin
			set @CurrentMonth = @CurrentMonth + 1
		end
		else
		begin
			set @CurrentYear = @CurrentYear + 1
			set @CurrentMonth = @MonthStart
		end

		-- 大于开始年月，才插入数据
		if((@CurrentYear * 100 + @CurrentMonth) >= @YearMonthStart)
		begin
			insert into Dim_U9_MonthFilter
			(MonthName)
			select cast(@CurrentYear as varchar(125)) + '年' + right('00' + cast(@CurrentMonth as varchar(125)),2) + '月'
		end
	end
end


/*
truncate table Dim_U9_MonthFilter


select *
from Dim_U9_MonthFilter
*/




