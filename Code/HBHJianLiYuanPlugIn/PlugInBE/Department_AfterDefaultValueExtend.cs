using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Business;
using UFIDA.U9.SM.SO;
using UFSoft.UBF.PL;
using UFIDA.U9.SM.Enums;
using UFIDA.U9.PR.PurchaseRequest;
using U9.VOB.Cus.HBHJianLiYuan.HBHHelper;
using HBH.DoNet.DevPlatform.EntityMapping;
using UFIDA.U9.CBO.SCM.Supplier;
using UFIDA.U9.Base;
using UFIDA.U9.CBO.HR.Department;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    public class Department_AfterDefaultValueExtend : UFSoft.UBF.Eventing.IEventSubscriber
    {
        public void Notify(params object[] args)
        {
            if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
                return;
            BusinessEntity.EntityKey key = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;

            if (key == null)
                return;
            Department entity = key.GetEntity() as Department;//请购单实体
            if (entity == null)
                return;

            if (HBHHelper.PubConfig.Const_ThirdHRStage)
            {
                // 如果 编码为空、父部门编码不为空，则流水赋值编码
                string parentDeptCode = entity.DescFlexField.PrivateDescSeg1;
                if (string.IsNullOrEmpty(entity.Code)
                    && !string.IsNullOrEmpty(parentDeptCode)
                    )
                {
                    string strPre = parentDeptCode;
                    int parentDeptLen = parentDeptCode.Length;

                    // 3-2-2-3-2-2
                    int subLength = 0;
                    // 父是7，则子3；否则为2
                    if (parentDeptLen == 7)
                    {
                        subLength = 3;
                    }
                    else
                    {
                        subLength = 2;
                    }

                    //string maxOpath = string.Format("Code like @Code + '%' and Code != @Code order by Convert(int,Substring(Code,len(@Code)+1,len(Code)),8) desc"
                                //);
                    string maxOpath = string.Format("Code like @Code + '%' and Code != @Code2"
                                );
                    Department.EntityList lstDept = Department.Finder.FindAll(maxOpath
                        , new OqlParam(strPre)
                        , new OqlParam(strPre)
                        );

                    int maxFlow = 0;

                    if (lstDept != null
                        && lstDept.Count > 0
                        )
                    {
                        foreach (Department dept in lstDept)
                        {
                            int curFlow = GetFlow(dept.Code, parentDeptLen, subLength);

                            if (curFlow > maxFlow)
                                maxFlow = curFlow;
                        }
                        //if (maxDept != null)
                        //{
                        //    //string strDeptCode = maxDept.Code;
                        //    //string strFl = strDeptCode.Remove(0, strPre.Length - 1);

                        //    //int.TryParse(strFl, out maxFlow);
                        //    maxFlow = GetFlow(maxDept.Code, strPre.Length);
                        //}
                    }

                    maxFlow++;
                    //entity.Code = strPre + maxFlow.ToString();

                    string strNewFlow = maxFlow.ToString().PadLeft(subLength,'0');

                    entity.Code = strPre + strNewFlow;
                }
            }
        }

        private int GetFlow(string deptCode, int parentLength, int subLength)
        {
            int flow = 0;

            //string strFl = deptCode.Remove(0, parentLength);
            string strFl = deptCode.Substring(parentLength, subLength);

            int.TryParse(strFl, out flow);

            return flow;
        }
    }
}
