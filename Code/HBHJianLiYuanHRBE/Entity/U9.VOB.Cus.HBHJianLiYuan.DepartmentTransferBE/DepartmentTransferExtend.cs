

#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using U9.VOB.HBHCommon.U9CommonBE;
using UFIDA.U9.CBO.HR.Operator;
using UFSoft.UBF.PL;
using UFSoft.UBF.Business;
using UFSoft.UBF.Transactions;
using UFIDA.U9.HI.Transfer;
using U9.VOB.Cus.HBHJianLiYuan.HBHHelper;
using UFIDA.U9.CBO.HR.Person;
using UFIDA.U9.HI.Enums;
using UFIDA.U9.Base;
using UFIDA.U9.HI.InsideTransBP.Proxy;
using UFIDA.U9.HI.InsideTransBP;

#endregion

namespace U9.VOB.Cus.HBHJianLiYuan {

	public partial class DepartmentTransfer{

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
            base.OnSetDefaultValue();

            if (this.Org == null)
            {
                this.Org = Context.LoginOrg;
            }

            if (this.Status != DocStatus.Approved)
            {
                if (this.ChangedBeforeDept == null
                    && this.ChangedBeforeDeptKey != null
                    )
                {
                    this.ChangedBeforeDeptKey = null;
                }
                if (this.ChangedAfterDept == null
                    && this.ChangedAfterDeptKey != null
                    )
                {
                    this.ChangedAfterDeptKey = null;
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

            ApprovedDo();
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
        protected override void OnValidate()
        {
            base.OnValidate();
            this.SelfEntityValidator();
            // TO DO: write your business code here...

            //if (this.OriginalData != null
            //    && this.OriginalData.Status == DocStatus.Approved
            //    && this.Status == DocStatus.Opened
            //    )
            //{
            //    throw new BusinessException("部门调动单不允许弃审");
            //}

            if (this.SysState != UFSoft.UBF.PL.Engine.ObjectState.Deleted)
            {
                if ((this.Status == DocStatus.Opened
                    || this.Status == DocStatus.Empty
                    )
                    && this.ApplyMan == null
                    )
                {
                    throw new BusinessException("申请人不允许为空!");
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
		private static DepartmentTransfer CreateDefault_Extend() {
			//TODO delete next code and add your custome code here
			throw new NotImplementedException () ;
		}
		/// <summary>
		/// Create DefaultComponent
		/// </summary>
		/// <returns>DefaultComponent Instance</returns>
		private  static DepartmentTransfer CreateDefaultComponent_Extend(){
			//TODO delete next code and add your custome code here
			throw new NotImplementedException () ;
		}
		
		#endregion 






		#region Model Methods


        private void ApprovedDo()
        {
            // 审核时，查找出所有  转换前 部门的人员，转移到  转换后部门；
            if (this.OriginalData != null
                && this.OriginalData.Status == DocStatus.Approving
                && this.Status == DocStatus.Approved
                )
            {
                if (this.ChangedBeforeDeptKey != null
                    && this.ChangedBeforeDeptKey.ID > 0
                    && this.ChangedAfterDeptKey != null
                    && this.ChangedAfterDeptKey.ID > 0
                    )
                {
                    using (UBFTransactionScope trans = new UBFTransactionScope(TransactionOption.Required))
                    {
                        using (ISession session = Session.Open())
                        {
                            //Operators.EntityList lstOperator = Operators.Finder.FindAll("Dept=@Dept"
                            //        , new OqlParam(this.ChangedBeforeDeptKey.ID)
                            //        );

                            //if (lstOperator != null
                            //    && lstOperator.Count > 0
                            //    )
                            //{
                            //    // 产生部门调动单行
                            //    foreach (Operators opr in lstOperator)
                            //    {
                            //        if (opr != null)
                            //        {
                            //            opr.DeptKey = this.ChangedAfterDeptKey;
                            //        }
                            //    }
                            //}


                            // 查出所有的职员  DimissionDate 离职日期
                            EmployeeArchive.EntityList lstEmployee = EmployeeArchive.Finder.FindAll("Dept=@Dept and (DimissionDate is null or DimissionDate <= '1980-1-1' or DimissionDate > @Today)"
                                        , new OqlParam(this.ChangedBeforeDeptKey.ID)
                                        , new OqlParam(DateTime.Today)
                                        );
                            
                            if (lstEmployee != null
                                && lstEmployee.Count > 0
                                )
                            {
                                DateTime now = DateTime.Today;

                                // 产生部门调动单头
                                InsideTransferDoc transferHead = InsideTransferDoc.Create();

                                transferHead.DocStatus = DocStatusEnum.Opening;
                                transferHead.DocumentType = InsideTranferDocType.Finder.Find("Code=@Code"
                                        , new OqlParam(DepartmentTransferHelper.Const_InsideTranferDocTypeCode)
                                        );
                                //transferHead.DocNo = "自动编号";
                                transferHead.Name = string.Empty;
                                transferHead.TransferDate = this.ChangedDate;
                                transferHead.ApplyDate = now;
                                transferHead.BusinessDate = now;
                                //transferHead.SalaryAdjustDate = now;

                                transferHead.TransferReason = this.Memo;

                                transferHead.ApplyBusinessOrgKey = this.OrgKey;
                                if (this.ApplyMan != null)
                                {
                                    transferHead.ApplicantKey = this.ApplyManKey;
                                    transferHead.ApplyDeptKey = this.ApplyMan.DeptKey;
                                }

                                //transferHead.InsideTransDocPersons = new InsideTransDocPerson.EntityList();
                                // 产生部门调动单行
                                foreach (EmployeeArchive employee in lstEmployee)
                                {
                                    // 2017-05-03 wf  职务和岗位同时为空异常
                                    if (employee != null
                                        && ((employee.JobKey != null && employee.JobKey.ID > 0)
                                            || (employee.JobLevelKey != null && employee.JobLevelKey.ID > 0)
                                            )
                                        )
                                    {
                                        InsideTransDocPerson person = InsideTransDocPerson.Create(transferHead);

                                        person.TransferReason = string.Format("部门调动单自动产生");

                                        // 调动前
                                        person.Employee = employee;
                                        person.PersonKey = employee.PersonKey;
                                        person.WorkOrgKey = employee.WorkingOrgKey;
                                        person.TransferDate = this.ChangedDate;
                                        person.BeforeBusinessOrgKey = employee.BusinessOrgKey;
                                        person.BeforeDeptKey = this.ChangedBeforeDeptKey;
                                        person.BeforeJobKey = employee.JobKey;
                                        person.BeforeJobGradeKey = employee.JobLevelKey;
                                        person.BeforePositionKey = employee.PositionKey;

                                        // 调动后
                                        person.AcceptDepartKey = this.ChangedAfterDeptKey;
                                        person.AcceptBusinessOrgKey = this.ChangedAfterDept.OrgKey;
                                        person.AfterJobKey = employee.JobKey;
                                        person.AfterJobGradeKey = employee.JobLevelKey;
                                        // 王希,变化后岗位不处理行了，岗位还得手工添加
                                        //person.AfterPositionKey = employee.PositionKey;

                                        person.Status = transferHead.DocStatus;
                                    }
                                }

                                this.InsideTransferDocNo = transferHead.DocNo;

                                session.Commit();

                                // 提交
                                if (transferHead != null
                                    && transferHead.ID > 0
                                    )
                                {
                                    InsideTransDocApproveProxy insideTransDocApproveProxy = new InsideTransDocApproveProxy();
                                    insideTransDocApproveProxy.DocKey = transferHead.ID;
                                    insideTransDocApproveProxy.UISysVersion = transferHead.SysVersion.ToString();
                                    //insideTransDocApproveProxy.OperationType = 0;
                                    insideTransDocApproveProxy.OperationType = 0;
                                    //List<ErrorMessageDTOData> list = insideTransDocApproveProxy.Do();
                                    List<ErrorMessageDTOData> list = insideTransDocApproveProxy.Do();

                                    if (list != null
                                        && list.Count > 0
                                        && list[0] != null
                                        && !string.IsNullOrEmpty(list[0].Message)
                                        )
                                    {
                                        throw new BusinessException(string.Format("内部调动申请单[{0}]审核失败,错误消息:{1}!"
                                            , transferHead.DocNo
                                            , list[0].Message
                                            ));
                                    }

                                    transferHead = InsideTransferDoc.Finder.FindByID(transferHead.ID);
                                    if (transferHead.DocStatus == DocStatusEnum.Approving)
                                    {
                                        // 审核
                                        //                    InsideTransDocApproveProxy insideTransDocApproveProxy = new InsideTransDocApproveProxy();
                                        //insideTransDocApproveProxy.set_DocKey(this.CurrentModel.InsideTransferDoc.FocusedRecord.ID);
                                        //insideTransDocApproveProxy.set_UISysVersion(this.CurrentModel.InsideTransferDoc.FocusedRecord.SysVersion.ToString());
                                        //insideTransDocApproveProxy.set_OperationType(1);
                                        //List<ErrorMessageDTOData> list = insideTransDocApproveProxy.Do();
                                        insideTransDocApproveProxy.UISysVersion = transferHead.SysVersion.ToString();
                                        //insideTransDocApproveProxy.OperationType = 1;
                                        insideTransDocApproveProxy.OperationType = 1;
                                        //List<ErrorMessageDTOData> list = insideTransDocApproveProxy.Do();
                                        insideTransDocApproveProxy.Do();
                                    }
                                    else
                                    {
                                        throw new BusinessException(string.Format("内部调动申请单[{0}]审核失败!"
                                            , transferHead.DocNo
                                            ));
                                    }
                                }
                            }

                            using (ISession session2 = Session.Open())
                            {
                                // 旧部门失效
                                if (this.ChangedBeforeDept != null
                                    && this.ChangedBeforeDept.Effective != null
                                    )
                                {
                                    // 前一天失效，内部调动单就是调动日期  前一天失效、调动日期当天就转到新部门了；
                                    this.ChangedBeforeDept.Effective.DisableDate = this.ChangedDate.AddDays(-1);
                                }

                                session2.Commit();
                            }
                        }
                        trans.Commit();
                    }
                }
            }
        }


		#endregion		
	}
}
