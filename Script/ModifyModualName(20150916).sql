
declare @ModuleName nvarchar(10) = '��Ӫ����';

update Base_Application_TRL
set [Name] = @ModuleName
where ID in (select app.ID from Base_Application app where app.Code in ( '99010','9901001'))
	and SysMLFlag = 'zh-CN'
	and [Name] = 'ó������'
	;

update Base_Role_trl
set [Name] = @ModuleName+'Ӧ�ù���Ա��ɫ'
where ID in (select top 1 id from Base_Role where Code  in ('JLYTC'+'CustAppAdmin','JLYTC01'+'CustAppAdmin'))
	and SysMLFlag = 'zh-CN'
	and [Name] = 'ó������'+'Ӧ�ù���Ա��ɫ'
	;
	
/*

select *
from Base_Application_TRL

select *
from Base_Role_trl

*/

