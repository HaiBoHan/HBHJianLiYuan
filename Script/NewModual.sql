

if not exists(select * from Base_Application where Code='99010')
begin

	 exec HBH_SP_Common_AddModule '99010','JLYTC','��Ӫ����',null
	 exec HBH_SP_Common_AddModule '9901001','JLYTC01','��Ӫ����','99010'

end