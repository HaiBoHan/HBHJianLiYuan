



declare @YearStart int = 2015
declare @YearEnd int = 2050
declare @MonthStart int = 1
declare @MonthEnd int = 12

declare @CurrentYear int = @YearStart
declare @CurrentMonth int = 0


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

		insert into Dim_U9_MonthFilter
		(MonthName)
		select cast(@CurrentYear as varchar(125)) + 'Äê' + right('00' + cast(@CurrentMonth as varchar(125)),2) + 'ÔÂ'

	end
end


/*
truncate table Dim_U9_MonthFilter


select *
from Dim_U9_MonthFilter
*/




