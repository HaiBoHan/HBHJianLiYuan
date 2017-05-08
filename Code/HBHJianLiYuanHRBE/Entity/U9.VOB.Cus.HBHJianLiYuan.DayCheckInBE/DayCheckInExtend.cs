

#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UFSoft.UBF.PL;
using UFSoft.UBF.Business;
using UFIDA.U9.Base;
using UFSoft.UBF.PL.Engine;
using U9.VOB.HBHCommon.U9CommonBE;
using UFIDA.U9.Approval.Util;
using UFIDA.U9.GeneralEvents;
using UFIDA.U9.CBO.HR.Department;
using HBH.DoNet.DevPlatform.EntityMapping;

#endregion

namespace U9.VOB.Cus.HBHJianLiYuan {

	public partial class DayCheckIn{

        // 管理中心组织编码 = 05
        public const string Const_ManageOrgCode = "05";

        public const string Const_ApproveFlowName = "ApproveFlow";

		#region Custom Constructor

		#endregion

		#region before & after CUD V 
		/*	实体提交的事件顺序示例(QHELP) 主实体A 组合 非主实体B .
		/ (新增A和B两个实例)A.OnSetDefaultValue->B.OnSetDefaultValue-> B.OnValidate ->A.OnValidate ->A.OnInserting ->B.OnInserting ->产生提交SQL ->B.OnInserted ->A.OnInserted
		/ (仅修改B,A也会被修改))A.OnSetDefaultValue->B.OnSetDefaultValue-> B.OnValidate ->A.OnValidate ->A.OnUpdating ->B.OnUpdating ->产生提交SQL ->B.OnUpdated ->A.OnUpdated
		/ (删除A,B会被级联删除))A.OnDeleting ->B.OnDeleting ->产生提交SQL ->B.OnDeleted ->A.OnDeleted
		/	产生提交SQL顺序则看其外键，增修一对多先A后B，一对一先B后A。　删除一对多先B后A，一对一先A后B .
		*/	
		/// <summary>
		/// 设置默认值
		/// </summary>
        protected override void OnSetDefaultValue()
        {
            if (this.SysState == ObjectState.Inserted)
            {
                this.StateMachineInstance.Initialize();
            }

            base.OnSetDefaultValue();

            if (this.Org == null)
            {
                // 默认组织不可以是其他组织
                if (Context.LoginOrg.Code != Const_ManageOrgCode)
                {
                    throw new BusinessException(string.Format("日考勤，默认组织[{0}]不满足管理中心组织约束!"
                        , Context.LoginOrg.Name
                        ));
                }

                this.Org = Context.LoginOrg;
            }

            // 审批方式 默认= 审批流
            if (this.ApproveType == null)
            {
                this.ApproveType = UFIDA.UBF.MD.Business.Attribute.Finder.Find("Name=@Name"
                    , new OqlParam("ApproveFlow")
                    );
            }


            if (this.OriginalData != null
                && this.OriginalData.Status != this.Status
                )
            {
                if (this.Status == DocStatus.Opened)
                {
                    // 弃审
                    if (this.OriginalData.Status == DocStatus.Approved)
                    {
                        DoUnApprove();
                    }
                    // 回收
                    else if (this.OriginalData.Status == DocStatus.Approving)
                    {
                        DoTerminate();
                    }
                }
                else if (this.Status == DocStatus.Approving)
                {
                    // 提交
                    if (this.OriginalData.Status == DocStatus.Opened)
                    {
                        DoSubmit();
                    }
                }
                else if (this.Status == DocStatus.Approved)
                {
                    // 审核
                    if (this.OriginalData.Status == DocStatus.Approving)
                    {
                        DoApprove();
                    }
                }
            }

            // 汇总行数量
            // 开立、或 第一次提交
            if (this.Status == DocStatus.Opened
                || (this.Status == DocStatus.Approving
                    && this.OriginalData != null
                    && this.OriginalData.Status == DocStatus.Opened
                    )
                )
            {
                decimal sumFull = 0;
                decimal sumPart = 0;
                decimal sumHourly = 0;
                foreach (DayCheckInLine line in this.DayCheckInLine)
                {
                    // 超出的 认为是加班的
                    if (line.FullTimeDay - 1 > 0)
                    {
                        sumFull += line.FullTimeDay - 1;
                    }
                    // 超出的 认为是加班的
                    if (line.PartTimeDay - 4 > 0)
                    {
                        sumPart += line.PartTimeDay - 4;
                    }
                    sumHourly += line.HourlyDay;
                }

                if (this.TotalFullTimeDay != sumFull)
                {
                    this.TotalFullTimeDay = sumFull;
                }
                if (this.TotalPartTimeDay != sumPart)
                {
                    this.TotalPartTimeDay = sumPart;
                }
                if (this.TotalHourlyDay != sumHourly)
                {
                    this.TotalHourlyDay = sumHourly;
                }
            }
        }
		/// <summary>
		/// before Insert
		/// </summary>
		protected override void OnInserting() {
			base.OnInserting();
			// TO DO: write your business code here...
		}

		/// <summary>
		/// after Insert
		/// </summary>
		protected override void OnInserted() {
			base.OnInserted();
			// TO DO: write your business code here...
		}

		/// <summary>
		/// before Update
		/// </summary>
		protected override void OnUpdating() {
			base.OnUpdating();
			// TO DO: write your business code here...
		}

		/// <summary>
		/// after Update
		/// </summary>
		protected override void OnUpdated() {
			base.OnUpdated();
			// TO DO: write your business code here...
		}


		/// <summary>
		/// before Delete
		/// </summary>
		protected override void OnDeleting() {
			base.OnDeleting();
            // TO DO: write your business code here...

            if (this.Status != DocStatus.Opened
                )
            {
                throw new BusinessException(string.Format("部门[{0}]考勤日[{1}]非开立状态不允许删除!"
                    , this.Department != null ? this.Department.Name : string.Empty
                    , this.CheckInDate.ToString("yyyy-MM-dd")
                    ));
            }
		}

		/// <summary>
		/// after Delete
		/// </summary>
		protected override void OnDeleted() {
			base.OnDeleted();
			// TO DO: write your business code here...
		}

		/// <summary>
		/// on Validate
		/// </summary>
		protected override void OnValidate() {
			base.OnValidate();
			this.SelfEntityValidator();
			// TO DO: write your business code here...

            if (this.SysState != UFSoft.UBF.PL.Engine.ObjectState.Deleted)
            {
                if (this.DepartmentKey == null)
                    throw new BusinessException("部门不可为空!");


                DayCheckIn old = DayCheckIn.Finder.Find("Department=@Dept and ID!=@ID and Convert(char(8),CheckInDate,112)=Convert(char(8),@CheckInDate,112)"
                    , new OqlParam(this.DepartmentKey.ID)
                    , new OqlParam(this.ID)
                    , new OqlParam(this.CheckInDate)
                    );
                if (old != null)
                {
                    string strMsg = string.Format("存在相同部门[{0}],相同日期[{1}]的考勤记录!"
                            , this.Department.Name
                            , this.CheckInDate.ToString("yyyy-MM-dd")
                            );
                    throw new BusinessException(strMsg);
                }

                if (ApproveType == null)
                {
                    string strMsg = string.Format("部门[{0}]日期[{1}] 的审批方式不可为空!"
                            , this.Department.Name
                            , this.CheckInDate.ToString("yyyy-MM-dd")
                            );
                    throw new BusinessException(strMsg);
                }
            }

            if (this.Status == DocStatus.Approved
                || this.Status == DocStatus.Closed
                )
            {
                if (this.OriginalData != null)
                {
                    UnchangedValidate<long>("Department");
                    UnchangedValidate<long>("CurrentOperator");

                    UnchangedValidate<DateTime>("CheckInDate");
                    UnchangedValidate<decimal>("Income");
                    UnchangedValidate<decimal>("LaborCostTarget");
                    UnchangedValidate<decimal>("LaborYieldTarget");
                    UnchangedValidate<DateTime>("BusinessDate");
                }
            }

            // 提交校验
            if (this.Status == DocStatus.Approving)
            {
                StringBuilder sbIDs = new StringBuilder();
                foreach (DayCheckInLine line in this.DayCheckInLine)
                {
                    //if(line.StaffMemberKey != null
                    //    && line.StaffMemberKey.ID > 0
                    //    )
                    //{
                    //    sbIDs.Append(line.StaffMemberKey.ID).Append(",");
                    //}

                    if (line.EmployeeArchiveKey != null
                        && line.EmployeeArchiveKey.ID > 0
                        )
                    {
                        sbIDs.Append(line.EmployeeArchiveKey.ID).Append(",");
                    }
                }

                if (sbIDs.Length > 0)
                {
                    //DayCheckInLine.EntityList list = U9.VOB.Cus.HBHJianLiYuan.DayCheckInLine.Finder.FindAll(string.Format("DayCheckIn != @DayCheckIn and DayCheckIn.CheckInDate=@Date and StaffMember in ({0}) "
                    DayCheckInLine.EntityList list = U9.VOB.Cus.HBHJianLiYuan.DayCheckInLine.Finder.FindAll(string.Format("DayCheckIn != @DayCheckIn and DayCheckIn.CheckInDate=@Date and EmployeeArchive in ({0}) "
                    , sbIDs.GetStringRemoveLastSplit()
                    )
                        , new OqlParam(this.ID)
                        , new OqlParam(this.CheckInDate)
                        );

                    if (list != null)
                    {
                        StringBuilder sbError = new StringBuilder();
                        foreach (var line in list)
                        {
                            if (line != null)
                            {
                                sbError.Append(string.Format("部门:[{0}],日期:[{1}],人员:[{2}];"
                                    , line.DayCheckIn.Department.Name
                                    , line.DayCheckIn.CheckInDate.ToString("yyyy-MM-dd")
                                    , line.EmployeeArchive.Name
                                    ));
                            }
                        }

                        if (sbError.Length > 0)
                        {
                            throw new BusinessException(string.Format("本单考勤记录与以下考勤信息冲突: {0}"
                                , sbError.ToString()
                                ));
                        }
                    }
                }
            }
		}

		#endregion
		
		#region 异常处理，开发人员可以重新封装异常
		public override void  DealException(Exception e)
        	{
          		base.DealException(e);
          		throw e;
        	}
		#endregion

		#region  扩展属性代码区
		
		#endregion

		#region CreateDefault
		private static DayCheckIn CreateDefault_Extend() {
			//TODO delete next code and add your custome code here
			throw new NotImplementedException () ;
		}
		/// <summary>
		/// Create DefaultComponent
		/// </summary>
		/// <returns>DefaultComponent Instance</returns>
		private  static DayCheckIn CreateDefaultComponent_Extend(){
			//TODO delete next code and add your custome code here
			throw new NotImplementedException () ;
		}
		
		#endregion 






		#region Model Methods

        #region 提交、审核、弃审

        /// <summary>
        /// 提交
        /// </summary>
        public void DoSubmit()
        {
            this.Status = DocStatus.Approving;
            if (IsApprovalFlow()
                )
            {
                this.StateMachineInstance.Initialize();
                //this.StateMachineInstance.Opend_SumitEvent(new SubmitEvent());
                this.StateMachineInstance.Opened_SumitEvent(new SumitEvent());


                ServiceOrderSubscriber_DayCheckIn serviceOrderSubscriber = new ServiceOrderSubscriber_DayCheckIn();
                serviceOrderSubscriber.EntityKey = this.Key;
                ApprovalService.Instance.SubmitApproval(this);
                EventHelper.SubscribeApprovalResultEvent(this.Key, serviceOrderSubscriber);
            }
        }

        private bool IsApprovalFlow()
        {
            //if (this.DocType.ConfirmType == Base.Doc.ConfirmTypeEnum.ApproveFlow)
            return this.ApproveType != null
                            && this.ApproveType.Name == Const_ApproveFlowName;
        }

        /// <summary>
        /// 审核
        /// </summary>
        public void DoApprove()
        {
            this.Status = DocStatus.Approved;
            if (IsApprovalFlow())
            {
                //this.StateMachineInstance.ApprovingState_ApproveEvent(new GeneralEvents.ApprovalResultEvent());
                this.StateMachineInstance.Approving_ApproveEvent(new ApproveEvent());
            }
            this.ApprovedOn = DateTime.Now;
            this.ApprovedBy = Context.LoginUser;

        }

        /// <summary>
        /// 弃审
        /// </summary>
        public void DoUnApprove()
        {
            this.Status = DocStatus.Opened;
            if (IsApprovalFlow())
            {
                //this.StateMachineInstance.ApprovedState_UnApproveEvent(new UnApprovedEvent());
                this.StateMachineInstance.Approved_UnApproveEvent(new UnApproveEvent());
            }
            //this.ApprovedOn = DateTime.MinValue;
            this.ApprovedOn = new DateTime(2000, 1, 1);
            this.ApprovedBy = null;
        }

        #endregion

        #region 工作流 审核相关

        /// <summary>
        /// 审核
        /// </summary>
        //public void DoApprove(ApprovalResultEvent ev)
        public void DoApprove(ApproveEvent ev)
        {
            this.Status = DocStatus.Approved;
            this.ApprovedOn = DateTime.Now;
            this.ApprovedBy = Context.LoginUser;

            if (IsApprovalFlow())
            {
                this.StateMachineInstance.Approving_ApproveEvent(ev);
                //this.StateMachineInstance.ApprovingState_ApproveEvent(new ApproveEvent());
            }
        }

        /// <summary>
        /// 终止、拒绝
        /// </summary>
        public void DoTerminate()
        {
            this.Status = DocStatus.Opened;
            this.ApprovedOn = new DateTime(2000, 1, 1);
            this.ApprovedBy = null;

            if (IsApprovalFlow())
            {
                ApprovalService.Instance.KillApproval(this);
            }
        }

        #endregion

        
        private void UnchangedValidate<T>(string field)
        {
            if (IsChanged<T>(this, field))
            {
                throw new BusinessException(string.Format("考勤单据已审核(或关闭)，字段[{0}]不允许修改!"
                    , DayCheckIn.EntityRes.GetResource(field)
                    ));
            }
        }


        public static bool IsChanged<T>(BusinessEntity entity, string field)
        {
            if (entity != null
                && entity.OriginalData != null
                )
            {
                T oldValue = (T)entity.OriginalData.GetValue(field);
                T newValue = (T)entity.GetValue(field);

                if (oldValue == null)
                {
                    if (newValue == null)
                    {
                        return true;
                    }
                    return false;
                }
                if (oldValue.Equals(newValue))
                {
                    return false;
                }
            }
            return true;
        }

		#endregion		
	}
}
