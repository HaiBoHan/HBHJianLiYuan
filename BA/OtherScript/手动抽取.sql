

use U9ETLMeta


select a.Code,a.Name,c.Code ExtractTableCode,c.Name ExtractTableName,License 
	,c.SaveRule
from [dbo].[ExtractSubject] a inner join [dbo].[ExtractSubjectDetail] b
on a.IID=b.SubjectId
inner join [dbo].[ExtractTables] c on b.ExtractTableCode=c.Code
where 1=1
	-- and a.Code=@SubjectCode
	and  c.SaveRule=1
order by c.Type,b.ExtractTableCode


select * from [ExtractTables]

/*
-- �ĳ�ÿ���洢�����Զ���ȡ�����ֶ���ȡ
update [ExtractTables]
set SaveRule = 0
where Code in ('901','902')
*/