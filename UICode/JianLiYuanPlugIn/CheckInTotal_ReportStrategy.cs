using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFIDA.UBF.Report.App.UI.ProcessStrategy;
using UFSoft.UBF.Report.UI.ReportView;
using UFIDA.UBF.Query.CaseModel;
using UFIDA.U9.CBO.Report.UI.Strategy;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.Report.Filter.FilterModel;
using UFSoft.UBF.UI.ActionProcess;
using HBH.DoNet.DevPlatform.EntityMapping;
using System.Data;
using UFIDA.UBF.Report.App.UI;
using HBH.DoNet.DevPlatform.U9Mapping;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInUI
{
    public class CheckInTotal_ReportStrategy: ReportProcessStrategy
    {
        public CheckInTotal_ReportStrategy()
            : base()
        {
        }

        public CheckInTotal_ReportStrategy(LoadReportTemplateArgs loadReportTemplateArgs, Case usercase)
            : base(loadReportTemplateArgs, usercase)
        {

        }

        public CheckInTotal_ReportStrategy(LoadReportTemplateArgs loadReportTemplateArgs, Case usercase, CaseDefine usercaseDefine)
            : base(loadReportTemplateArgs, usercase, usercaseDefine)
        {

        }
	
        /// <summary>
        /// 处理格式
        /// </summary>
        protected override void ProcessFormat()
        {
            //this.ProcessFormat_Extend();
        }
	
		

        
		
		
        /// <summary>
        /// 校验参数合法性       
        /// </summary>
        /// <param name="usercase">查询方案</param>
        /// <returns>提示信息</returns>
        public override string VerifyParameters(Case usercase, CaseDefine caseDefine)
        {
            //return this.VerifyParameters_Extend(usercase, caseDefine);        
            return null;		   
        }
		

        
		
		
		
        /// <summary>
        /// 处理方案定义信息
        /// </summary>
        /// <param name="usercase"></param>
        /// <param name="caseDefine"></param>
        /// <returns></returns>
        public override CaseDefine ProcessCaseDefine(Case usercase, CaseDefine caseDefine)
        {			            
            //return this.ProcessCaseDefine_Extend(usercase, caseDefine);     


            caseDefine = base.ProcessCaseDefine(usercase, caseDefine);
            caseDefine.IsHeadItemVisible = true;
            if (usercase == null || usercase.BasicInfo.Title == "")
            {
                //// 默认组织处理
                //CaseHelper.SetDefaultValue(
                //                            caseDefine.FilterDefines.GetObjectByName("Org"),
                //                            PDContext.Current.OrgRef.NameColumn,
                //                            PDContext.Current.OrgRef.CodeColumn,
                //                            PDContext.Current.OrgRef.ID.ToString(),
                //                            UFSoft.UBF.Report.Filter.enuOperatorListType.Equal);

                string strUserID = PDContext.Current.UserID;
                long orgID = PDContext.Current.OrgRef.ID;
                {
                    string strFilter = "Department";
                    FilterDefine filterDefine = caseDefine.FilterDefines.GetObjectByName(strFilter);
                    if (filterDefine != null)
                    {
                        string sql = string.Format(@"
select 
	dept.ID
	,dept.Code
	,deptTrl.Name
    ,usr.Code
	,opr.Code
from Base_User usr
	left join CBO_Operators opr
	on usr.Contact = opr.Contact
	left join CBO_Department dept
	on opr.Dept = dept.ID
	left join CBO_Department_Trl deptTrl
	on deptTrl.ID = dept.ID
        and deptTrl.SysMLFlag = 'zh-CN'
where usr.ID = @User
"
                            );
                        ParamDTO paramDTO = new ParamDTO();
                        paramDTO.ParamDirection = ParameterDirection.Input;
                        paramDTO.ParamType = DbType.String;
                        paramDTO.ParamName = "User";
                        paramDTO.ParamValue = strUserID;

                        DataTable dt;
                        U9Helper.GetResultBySql(sql, out dt, paramDTO);

                        string strDeptID = string.Empty;
                        string strDeptCode = string.Empty;
                        string strDeptName = string.Empty;
                        if (dt != null
                            && dt.Rows != null
                            && dt.Rows.Count > 0
                            )
                        {
                            strDeptID = dt.Rows[0][0].GetString();
                            strDeptCode = dt.Rows[0][1].GetString();
                            strDeptName = dt.Rows[0][2].GetString();
                        }

                        if (strDeptCode.IsNotNullOrWhiteSpace())
                        {
                            // 默认值
                            CaseHelper.SetDefaultValue(
                                                        filterDefine
                                                        , strDeptCode
                                                        , strDeptName
                                                        , strDeptID
                                                        ,UFSoft.UBF.Report.Filter.enuOperatorListType.Equal
                                                        );

                            // 过滤条件
                            // "__curOId={0}&{1}={2}"
                            filterDefine.Reference.ReferenceObject.RefCondCollection[0].CustomInParams = string.Format("{0}={1}"
                                                            , BaseAction.Symbol_AddCustomFilter
                                                            , string.Format("Code like '{0}%' and Org='{1}'"
                                                                    , strDeptCode
                                                                    , orgID
                                                                    )
                                                            );
                        }
                    }
                }
            }

            ////处理弹性域和自由项
            //this.ProcessItemOptAndFlexField(usercase, caseDefine);

            return caseDefine;
        }
	
        /// <summary>
        /// 钻取到报表
        /// </summary>
        /// <param name="sourceDefine"></param>
        /// <param name="casemodel"></param>
        public override void DrillThroughToReport(Case sourceCase, LoadReportTemplateArgs drillArgs)
        {
            //this.DrillThroughToReport_Extend(sourceCase,drillArgs);
        }
	
		
		

        
		
        /// <summary>
        /// 构造钻取到表单的参数（单据ID，组织ID，PartID）
        /// </summary>
        /// <param name="args">钻取参数</param>
        /// <returns></returns>
        public override ThroughToDocParameter GetThroughToDocParameter(ReportDrillthroughArgs args)
        {
            //return this.GetThroughToDocParameter_Extend(args);
            return null;
        }
		
		

        
        /// <summary>
        /// 参数值变化事件处理入口
        /// </summary>
        /// <param name="sourceDefine"></param>
        /// <param name="casemodel"></param>
        public override void FilterItemChangedEventHandler(FilterDefine sourceDefine, CaseModel casemodel)
        {
            //this.FilterItemChangedEventHandler_Extend(sourceDefine,casemodel);

            //CaseDefine caseDefine = casemodel.CaseDefine;
            //Case usercase = casemodel.Case;

            //// 组织改变，清空存储地点、物料；  供应商是按编码匹配的
            //if (sourceDefine.Name == "Org")
            //{
            //    string orgID = string.Empty;
            //    ValueContext orgValue = ReportAppService.GetFilterValue("Org", usercase);
            //    if (orgValue.Values.Count > 0)
            //    {
            //        orgID = orgValue.Values[0];
            //    }
            //    List<string> emptyList = new List<string> { string.Empty, string.Empty, string.Empty };

            //    {
            //        string strFilter = "WhID";
            //        //FilterDefine whFilter = caseDefine.FilterDefines.GetObjectByName("WhID");
            //        //if (whFilter != null)
            //        {
            //            //ValueContext vc = new ValueContext();
            //            //vc.Values = new List<string>();
            //            //vc.Values.Add("");
            //            //ReportAppService.SetFilterValue(m_loadReportTemplateArgs.DefaultTemplate, strFilter, vc);

            //            FilterValue filterValue = usercase.FilterValues.GetObjectByName(strFilter);
            //            if (filterValue != null
            //                && filterValue.Values != null
            //                )
            //            {
            //                if (filterValue.Values.Labels != null
            //                    && filterValue.Values.Labels.Count > 0
            //                    )
            //                {
            //                    filterValue.Values.Labels[0] = string.Empty;
            //                }
            //                if (filterValue.Values.Values != null
            //                    && filterValue.Values.Values.Count > 0
            //                    )
            //                {
            //                    filterValue.Values.Values[0] = string.Empty;
            //                }
            //                if (filterValue.Values.ReferenceValues != null
            //                    && filterValue.Values.ReferenceValues.Count > 0
            //                    && filterValue.Values.ReferenceValues[0] != null
            //                    )
            //                {
            //                    filterValue.Values.ReferenceValues[0].CodeValue = string.Empty;
            //                    filterValue.Values.ReferenceValues[0].IDValue = string.Empty;
            //                }
            //            }

            //            FilterDefine filterDefine = usercase.FilterDefines.GetObjectByName(strFilter);
            //            if (filterDefine != null)
            //            {
            //                filterDefine.Reference.ReferenceObject.RefCondCollection[0].CustomInParams = string.Format("__curOId={0}&{1}={2}"
            //                                                , orgID
            //                                                , BaseAction.Symbol_AddCustomFilter
            //                                                , "(LocationType = 3 )"
            //                                                );
            //            }
            //            //if (!string.IsNullOrWhiteSpace(orgID))
            //            //{
            //            //    ValueContext vcParam = new ValueContext();
            //            //    vcParam.Values = new List<string>();
            //            //    vcParam.Values.Add(string.Format("OrgID={0}"
            //            //        , orgID));
            //            //    ReportAppService.AddReportParameter(m_loadReportTemplateArgs, strFilter, vcParam);
            //            //}
            //        }
            //    }

            //    {
            //        string strFilter = "ItemID";
            //        //FilterDefine itemFilter = caseDefine.FilterDefines.GetObjectByName("ItemID");
            //        //if (whFilter != null)
            //        {
            //            //ValueContext vc = new ValueContext();
            //            //vc.Values = new List<string>();
            //            //vc.Values.Add("");
            //            //ReportAppService.SetFilterValue(m_loadReportTemplateArgs.DefaultTemplate, strFilter, vc);

            //            FilterValue filterValue = usercase.FilterValues.GetObjectByName(strFilter);
            //            if (filterValue != null
            //                && filterValue.Values != null
            //                )
            //            {
            //                if (filterValue.Values.Labels != null
            //                    && filterValue.Values.Labels.Count > 0
            //                    )
            //                {
            //                    filterValue.Values.Labels[0] = string.Empty;
            //                }
            //                if (filterValue.Values.Values != null
            //                    && filterValue.Values.Values.Count > 0
            //                    )
            //                {
            //                    filterValue.Values.Values[0] = string.Empty;
            //                }
            //                if (filterValue.Values.ReferenceValues != null
            //                    && filterValue.Values.ReferenceValues.Count > 0
            //                    && filterValue.Values.ReferenceValues[0] != null
            //                    )
            //                {
            //                    filterValue.Values.ReferenceValues[0].CodeValue = string.Empty;
            //                    filterValue.Values.ReferenceValues[0].IDValue = string.Empty;
            //                }
            //            }

            //            FilterDefine filterDefine = usercase.FilterDefines.GetObjectByName(strFilter);
            //            if (filterDefine != null)
            //            {
            //                filterDefine.Reference.ReferenceObject.RefCondCollection[0].CustomInParams = string.Format("__curOId={0}"
            //                                                , orgID);
            //            }
            //            //if (!string.IsNullOrWhiteSpace(orgID))
            //            //{
            //            //    ValueContext vcParam = new ValueContext();
            //            //    vcParam.Values = new List<string>();
            //            //    vcParam.Values.Add(string.Format("Org={0}"
            //            //        , orgID));
            //            //    ReportAppService.AddReportParameter(m_loadReportTemplateArgs, strFilter, vcParam);
            //            //}
            //        }
            //    }

            //    {
            //        string strFilter = "Supplier";
            //        FilterDefine filterDefine = usercase.FilterDefines.GetObjectByName(strFilter);
            //        if (filterDefine != null)
            //        {
            //            filterDefine.Reference.ReferenceObject.RefCondCollection[0].CustomInParams = string.Format("__curOId={0}"
            //                                            , orgID);
            //        }
            //    }

            //}
        }
		

    }
}
