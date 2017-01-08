



declare @YearStart int = 2016
declare @YearEnd int = 2050
declare @MonthStart int = 1
declare @MonthEnd int = 12
declare @YearMonthStart int = 201606

declare @CurrentYear int = @YearStart
declare @CurrentMonth int = 0
-- declare @CurrentDate datetime = cast((cast(@YearStart as varchar(125)) + '-' + cast(@MonthStart as varchar(125)) + '-' + '1') as datetime)
declare @CurrentDate datetime = cast(cast((@YearStart * 10000 + @MonthStart * 100 + 1) as varchar(125)) as datetime)


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

		-- ���ڿ�ʼ���£��Ų�������
		if((@CurrentYear * 100 + @CurrentMonth) >= @YearMonthStart)
		begin
			insert into Dim_U9_MonthFilter
			(MonthName)
			select cast(@CurrentYear as varchar(125)) + '��' + right('00' + cast(@CurrentMonth as varchar(125)),2) + '��'
		end
	end
end


/*
truncate table Dim_U9_MonthFilter


select *
from Dim_U9_MonthFilter
*/



/*
truncate table Dim_U9_Date_Filter
*/
if not exists(select 1 from Dim_U9_Date_Filter)
begin
	while year(@CurrentDate) <= @YearEnd
	begin

		-- ���ڿ�ʼ���£��Ų�������
		if(year(@CurrentDate) * 100 + month(@CurrentDate) >= @YearMonthStart)
		begin
			insert into Dim_U9_Date_Filter
			(DayName,DayDate)
			select cast(year(@CurrentDate) as varchar(125)) + '��' + right('00' + cast(month(@CurrentDate) as varchar(125)),2) + '��' + right('00' + cast(day(@CurrentDate) as varchar(125)),2) + '��' + '[' + convert(varchar(8),@CurrentDate,112) + ']'
				,@CurrentDate
		end

		set @CurrentDate = DateAdd(Day,1,@CurrentDate)
	end
end

