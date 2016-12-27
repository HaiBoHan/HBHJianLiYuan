


exec sp_executesql N'declare @IsPublic bit
    if exists(select 1 from Portal_FolderOfUser where PKID=@folderID)
        set @IsPublic=0
    else
        set @IsPublic=1;
    
    select d.*, case when @IsPublic=0 then 3 when a.Auth is null and d.Creater=@user then 3 else isnull(a.Auth,0) end Auth
    from Portal_Document d
    left join ResourceAuthFinal a
        on a.ResourceType=''portal.document''
        and a.UserCode=@user
        and a.ResourceId=d.PKID
    where d.FolderID=@folderID
        and case when @IsPublic=0 then 3 when a.Auth is null and d.Creater=@user then 3 else isnull(a.Auth,0) end > 0
    order by d.Name',N'@folderID nvarchar(36),@user nvarchar(3)',@folderID=N'aeee96ee-60e0-41ab-a726-1413f0ccb6c7',@user=N'lzl'




select *
from Portal_Document report
	left join ResourceAuthFinal res
    on res.ResourceType='portal.document'
		-- and a.UserCode=@user
		and res.ResourceId=report.PKID
order by 
	report.CreateDate desc




select *, 3 Auth from Portal_Document where PKID in ('02f12d47-f32a-486b-ad5b-5c5f12155d87')


select *
from Report_DataProvider
where ReportID in (
	'905E5195-AADA-4C91-8AE6-273CDA3F3721'
	,'3B3B5A55-60F9-400E-8AAA-BB6D7BE5BD1A'
	,'E0EF8D8E-5D18-43BC-93A7-3F655033CDC7'
	,'B5696C01-2B0B-4745-9844-5F3EFE3FEA62'
	
	-- 备份
	,'02f12d47-f32a-486b-ad5b-5c5f12155d87'
	)


select *
from Report_Advance


-- 报表存储位置，存储列为  ReportDefinition
/*
select *
into tmp_ReportList_20161227001
from ReportList
*/

select *
from ReportList
where IID in (
	'905E5195-AADA-4C91-8AE6-273CDA3F3721'
	,'3B3B5A55-60F9-400E-8AAA-BB6D7BE5BD1A'
	,'E0EF8D8E-5D18-43BC-93A7-3F655033CDC7'
	,'B5696C01-2B0B-4745-9844-5F3EFE3FEA62'
)