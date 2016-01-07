

#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UFIDA.U9.Base;
using U9.VOB.HBHCommon.U9CommonBE;
using UFIDA.U9.PAY.PayrollDoc;
using UFIDA.U9.PAY.PayrollDoc.Proxy;
using UFIDA.U9.PAY.Enums;
using UFSoft.UBF.Business;
using UFSoft.UBF.Transactions;
using UFIDA.U9.Approval.Util;
using UFSoft.UBF.PL.Engine;

#endregion

namespace U9.VOB.Cus.HBHJianLiYuan {

	public partial class TotalPayrollDoc{

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
                this.Org = Context.LoginOrg;
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

            ApproveDo();
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
                if (this.PayrollType == null)
                {
                    throw new BusinessException("计薪类别 不可为空!");
                }

                if (ApproveType == null)
                {
                    string strMsg = string.Format("计薪方案[{0}]发薪日期[{1}] 的审核方式不可为空!"
                            , this.PayrollType.Name
                            , this.PayDate.ToString("yyyy-MM-dd")
                            );
                    throw new BusinessException(strMsg);
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
		private static TotalPayrollDoc CreateDefault_Extend() {
			//TODO delete next code and add your custome code here
			throw new NotImplementedException () ;
		}
		/// <summary>
		/// Create DefaultComponent
		/// </summary>
		/// <returns>DefaultComponent Instance</returns>
		private  static TotalPayrollDoc CreateDefaultComponent_Extend(){
			//TODO delete next code and add your custome code here
			throw new NotImplementedException () ;
		}
		
		#endregion 






		#region Model Methods

        public void ApproveDo()
        { 
            // 审核时，查找出所有  转换前 部门的人员，转移到  转换后部门；
            if (this.OriginalData != null
                && this.OriginalData.Status == DocStatus.Approving
                && this.Status == DocStatus.Approved
                )
            {
                List<long> lstPayrollHead = new List<long>();

                if (this.TotalPayrollDocLine != null
                    && this.TotalPayrollDocLine.Count > 0
                    )
                {
                    foreach (TotalPayrollDocLine line in this.TotalPayrollDocLine)
                    {
                        if (line != null
                            && line.PayrollLineDetail != null
                            && line.PayrollLineDetail.Count > 0
                            )
                        {
                            foreach (PayrollLineDetail subline in line.PayrollLineDetail)
                            {
                                if (subline != null
                                    && subline.PayrollLine != null
                                    && subline.PayrollLine.PayrollDocKey != null
                                    )
                                {
                                    long headID = subline.PayrollLine.PayrollDocKey.ID;

                                    if (headID > 0
                                        && !lstPayrollHead.Contains(headID)
                                        )
                                    {
                                        lstPayrollHead.Add(headID);
                                    }
                                }
                            }
                        }
                    }
                }

                if (lstPayrollHead != null
                    && lstPayrollHead.Count > 0
                    )
                {
                    using (UBFTransactionScope trans = new UBFTransactionScope(TransactionOption.Required))
                    {
                        foreach (long headID in lstPayrollHead)
                        {
                            PayrollDoc payHead = PayrollDoc.Finder.FindByID(headID);

                            /*
                            Approved	已核准	2
                            Approving	核准中	1
                            Cancelled	作废	4
                            Closed	关闭	5
                            Opened	开立	0
                            Rejected	拒绝	3
                             */
                            if (payHead != null)
                            {
                                // 提交
                                //SubmitDocProxy submitDocProxy = new SubmitDocProxy();
                                //submitDocProxy.set_Doc(this.CurrentModel.PayrollDoc.FocusedRecord.ID);
                                //submitDocProxy.set_SysVersion(this.CurrentModel.PayrollDoc.FocusedRecord.SysVersion.Value);
                                //submitDocProxy.Do();


                                // 审核
                                if (payHead.Status == PayrollDocStatusEnum.Approving)
                                {
                                    ApproveDocProxy approveDocProxy = new ApproveDocProxy();
                                    approveDocProxy.Doc = headID;
                                    approveDocProxy.SysVersion = payHead.SysVersion;
                                    approveDocProxy.Do();
                                }
                                else
                                {
                                    string msg = string.Format("付薪申请单[{0}]状态[{1}]不是审核中，不允许审核!"
                                        , payHead.DocNo
                                        , payHead.Status.Name
                                        );
                                    throw new BusinessException(msg);
                                }
                            }
                        }
                        trans.Commit();
                    }
                }
            }
        }

        #region  工作流

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


                ServiceOrderSubscriber_TotalPayrollDoc serviceOrderSubscriber = new ServiceOrderSubscriber_TotalPayrollDoc();
                serviceOrderSubscriber.EntityKey = this.Key;
                ApprovalService.Instance.SubmitApproval(this);
                EventHelper.SubscribeApprovalResultEvent(this.Key, serviceOrderSubscriber);
            }
        }

        private bool IsApprovalFlow()
        {
            //if (this.DocType.ConfirmType == Base.Doc.ConfirmTypeEnum.ApproveFlow)
            return this.ApproveType != null
                            && this.ApproveType.Name == DayCheckIn.Const_ApproveFlowName;
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


        #endregion	
	
    
		#endregion		
	}
}
