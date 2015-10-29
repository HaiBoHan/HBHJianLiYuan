namespace U9.VOB.Cus.HBHJianLiYuan
{
	using System;
	using System.Collections.Generic;
	using System.Text; 
	using UFSoft.UBF.AopFrame;	
	using UFSoft.UBF.Util.Context;
    using UFIDA.U9.CBO.HR.Department;
    using UFIDA.U9.Cust.HBH.Common.CommonLibary;
    using UFIDA.U9.Base.Organization;
    using UFSoft.UBF.PL;

	/// <summary>
	/// GetDepartOrgBP partial 
	/// </summary>	
	public partial class GetDepartOrgBP 
	{	
		internal BaseStrategy Select()
		{
			return new GetDepartOrgBPImpementStrategy();	
		}		
	}
	
    //#region  implement strategy	
	/// <summary>
	/// Impement Implement
	/// 
	/// </summary>	
	internal partial class GetDepartOrgBPImpementStrategy : BaseStrategy
	{
		public GetDepartOrgBPImpementStrategy() { }

		public override object Do(object obj)
		{						
			GetDepartOrgBP bpObj = (GetDepartOrgBP)obj;
			
			//get business operation context is as follows
			//IContext context = ContextManager.Context	
			
			//auto generating code end,underside is user custom code
			//and if you Implement replace this Exception Code...
            //throw new NotImplementedException();

            List<DeptOrgResultDTO> listResult = new List<DeptOrgResultDTO>();
            if (bpObj != null
                && bpObj.Departments != null
                && bpObj.Departments.Count > 0
                )
            {
                List<long> lstDeptID = new List<long>();
                foreach (Department.EntityKey deptKey in bpObj.Departments)
                {
                    if (deptKey != null)
                    {
                        Department dept = deptKey.GetEntity();

                        if (dept != null)
                        {
                            long deptID = dept.ID;

                            if (!lstDeptID.Contains(deptID))
                            {
                                lstDeptID.Add(deptID);

                                DeptOrgResultDTO deptOrgDTO = GetDeptOrg(dept);

                                listResult.Add(deptOrgDTO);
                            }
                        }
                    }
                }
            }

            return listResult;
		}

        private DeptOrgResultDTO GetDeptOrg(Department dept)
        {
            DeptOrgResultDTO dto = new DeptOrgResultDTO();
            dto.DeptID = dept.ID;
            dto.DeptCode = dept.Code;
            dto.DeptName = dept.Name;

            string orgCode = HBHHelper.DepartmentHelper.GetRequireOrgCode(dept.DescFlexField);

            if (!PubClass.IsNull(orgCode))
            {
                Organization org = Organization.Finder.Find("Code=@Code", new OqlParam(orgCode));

                if (org != null)
                {
                    dto.OrgID = org.ID;
                    dto.OrgCode = org.Code;
                    dto.OrgName = org.Name;

                }
            }

            return dto;
        }
	}

    //#endregion
	
	
}