

if not exists(select * from Base_Application where Code='99010')
begin

	 exec HBH_SP_Common_AddModule '99010','JLYTC','��Ӫ����',null
	 exec HBH_SP_Common_AddModule '9901001','JLYTC01','��Ӫ����','99010'

end


if not exists(select * from Base_Application where Code='301101')
begin

	 exec HBH_SP_Common_AddModule '301101','JLYCheck','����Դ����','3011'

end
