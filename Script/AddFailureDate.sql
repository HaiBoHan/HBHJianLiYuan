

	drop table HBH_Debug_Param;


	create table HBH_Debug_Param
	(ProcName varchar(501)
	,Debugger bit
	,CreatedOn datetime
	,Memo varchar(max)	-- ±¸×¢
	,FailureDate datetime
	)
	
	insert into HBH_Debug_Param
	(ProcName,Debugger,CreatedOn,Memo,FailureDate)
	values('',1,GETDATE(),'Initial ParamTable','2016-01-01')


