


select a.Code,a.Name,c.Code ExtractTableCode,c.Name ExtractTableName,License from [dbo].[ExtractSubject] a inner join [dbo].[ExtractSubjectDetail] b
on a.IID=b.SubjectId
inner join [dbo].[ExtractTables] c on b.ExtractTableCode=c.Code
where 1=1
	-- and a.Code=@SubjectCode
	and  c.SaveRule=1
order by c.Type,b.ExtractTableCode

