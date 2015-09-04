

if not exists(select * from Base_Application where Code='99010')
begin

	 exec HBH_SP_Common_AddModule '99010','JLYTC','贸易中心',null
	 exec HBH_SP_Common_AddModule '9901001','JLYTC01','贸易中心','99010'

end