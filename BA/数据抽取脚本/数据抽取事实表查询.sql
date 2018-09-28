

/*
exec sp_executesql N'select a.Code,a.Name,c.Code ExtractTableCode,c.Name ExtractTableName,License from [dbo].[ExtractSubject] a inner join [dbo].[ExtractSubjectDetail] b
                                    on a.IID=b.SubjectId
                                    inner join [dbo].[ExtractTables] c on b.ExtractTableCode=c.Code
                                    where a.Code=@SubjectCode  and  c.SaveRule=1
                                     order by c.Type,b.ExtractTableCode',N'@SubjectCode nvarchar(3)',@SubjectCode=N'001'
*/

select *
from ExtractSubject


select *
from ExtractSubjectDetail

