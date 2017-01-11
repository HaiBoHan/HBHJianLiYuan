using System;
using System.Text;
using System.Collections;
using System.Xml;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Threading;

using Telerik.WebControls;
using UFSoft.UBF.UI.WebControls;
using UFSoft.UBF.UI.Controls;
using UFSoft.UBF.Util.Log;
using UFSoft.UBF.Util.Globalization;

using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.Engine;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.UI.ActionProcess;
using UFSoft.UBF.UI.WebControls.ClientCallBack;
using U9.VOB.Cus.HBHJianLiYuan.Proxy;
using U9.VOB.HBHCommon.U9CommonBE;
using U9.VOB.HBHCommon.HBHCommonUI;
using System.Collections.Generic;
using HBH.DoNet.DevPlatform.EntityMapping;
using U9.VOB.Cus.HBHJianLiYuan.HBHHelper;
using UFSoft.UBF.UI.WebControlAdapter;



/***********************************************************************************************
 * Form ID: 
 * UIFactory Auto Generator 
 ***********************************************************************************************/
namespace TotalPayrollDocUIModel
{
    public partial class TotalPayrollDocUIFormWebPart
    {
        #region Custome eventBind
	
		 
				//BtnSave_Click...
		private void BtnSave_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnSave_Click_DefaultImpl(sender,e);
		}	
		 
				//BtnCancel_Click...
		private void BtnCancel_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnCancel_Click_DefaultImpl(sender,e);
		}	
		 
				//BtnAdd_Click...
		private void BtnAdd_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnAdd_Click_DefaultImpl(sender,e);
		}	
		 
				//BtnDelete_Click...
		private void BtnDelete_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnDelete_Click_DefaultImpl(sender,e);
		}	
		 
				//BtnCopy_Click...
		private void BtnCopy_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			BtnCopy_Click_DefaultImpl(sender,e);


            TotalPayrollDocRecord head = this.Model.TotalPayrollDoc.FocusedRecord;

            if (head != null)
            {
                if (this.Model.TotalPayrollDoc.FieldStatus.DefaultValue != null)
                {
                    head.Status = (int)this.Model.TotalPayrollDoc.FieldStatus.DefaultValue;
                }

                if (this.Model.TotalPayrollDoc.FieldPayDate.DefaultValue != null)
                {
                    head.PayDate = (DateTime)this.Model.TotalPayrollDoc.FieldPayDate.DefaultValue;
                }
            }
        }

        //BtnSubmit_Click...
        private void BtnSubmit_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            //BtnSubmit_Click_DefaultImpl(sender,e);

            this.Model.ClearErrorMessage();

            UpdateStatus((int)DocStatusData.Approving);
        }

        //BtnApprove_Click...
        private void BtnApprove_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            //BtnApprove_Click_DefaultImpl(sender,e);

            this.Model.ClearErrorMessage();

            //UpdateStatus((int)DocStatusData.Approved);
            TotalPayrollDocRecord head = this.Model.TotalPayrollDoc.FocusedRecord;

            if (head != null
                && head.ID > 0
                )
            {
                //if (head.IsApproveFlow.GetValueOrDefault(false))
                if (IsApproveFlow(head))
                {
                    UFIDA.U9.UI.PDHelper.PDPopWebPart.ApproveFlow_ApproveBatchUIWebPart(this);
                }
                else
                {
                    //ActionHelper.DoApprove(entityKeys, entityFullName);//这里是不走工作流时审核处理
                    UpdateStatus((int)DocStatusData.Approved);
                }
            }
        }

        //BtnRecovery_Click...
        private void BtnRecovery_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            //BtnRecovery_Click_DefaultImpl(sender,e);

            this.Model.ClearErrorMessage();

            UpdateStatus((int)DocStatusData.Opened);
        }

        //BtnUndoApprove_Click...
        private void BtnUndoApprove_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            //BtnUndoApprove_Click_DefaultImpl(sender,e);

            this.Model.ClearErrorMessage();

            UpdateStatus((int)DocStatusData.Opened);
        }	
		 
				//BtnFind_Click...
		private void BtnFind_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnFind_Click_DefaultImpl(sender,e);
		}	
		 
				//BtnList_Click...
		private void BtnList_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            //BtnList_Click_DefaultImpl(sender, e);
            //U9.VOB.HBHCommon.HBHCommonUI.HBHUIHelper.UIForm_BtnList_Click(this, "TotalPayrollDoc");

            U9.VOB.HBHCommon.HBHCommonUI.HBHUIHelper.UIForm_BtnList_Click(this);
		}	
		 
				//BtnFirstPage_Click...
		private void BtnFirstPage_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnFirstPage_Click_DefaultImpl(sender,e);
		}	
		 
				//BtnPrevPage_Click...
		private void BtnPrevPage_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnPrevPage_Click_DefaultImpl(sender,e);
		}	
		 
				//BtnNextPage_Click...
		private void BtnNextPage_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnNextPage_Click_DefaultImpl(sender,e);
		}	
		 
				//BtnLastPage_Click...
		private void BtnLastPage_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnLastPage_Click_DefaultImpl(sender,e);
		}	
		 
				//BtnAttachment_Click...
		private void BtnAttachment_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnAttachment_Click_DefaultImpl(sender,e);
		}	
		 
				//BtnFlow_Click...
		private void BtnFlow_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnFlow_Click_DefaultImpl(sender,e);
		}	
		 
				//BtnOutput_Click...
		private void BtnOutput_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnOutput_Click_DefaultImpl(sender,e);
		}	
		 
				//BtnPrint_Click...
		private void BtnPrint_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnPrint_Click_DefaultImpl(sender,e);
		}	
		 
				//BtnOk_Click...
		private void BtnOk_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnOk_Click_DefaultImpl(sender,e);
		}	
		 
				//BtnClose_Click...
		private void BtnClose_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnClose_Click_DefaultImpl(sender,e);
        }


        //TabControl0_SelectedIndexChanged...
        private void TabControl0_SelectedIndexChanged_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            TabControl0_SelectedIndexChanged_DefaultImpl(sender, e);
        }	

        // BtnSumPayroll_Click...
        private void BtnSumPayroll_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            //BtnSumPayroll_Click_DefaultImpl(sender, e);

            this.Model.ClearErrorMessage();

            TotalPayrollDocRecord head = this.Model.TotalPayrollDoc.FocusedRecord;

            if (head == null
                || (head.PayDate == null
                    || head.PayDate.GetValueOrDefault(new DateTime(2000,1,1)).Year <= 2000
                    )
                )
            {
               //HBHUIHelper.ShowErrorInfo(this, "汇总计算,必须选择一个汇总维度(客户、业务员、部门、产品线).");
                this.Model.ErrorMessage.Message = "发薪日期不可为空!";
                //this.Model.ErrorMessage.hasErrorMessage = true;
                return;
            }


            this.BtnSave_Click(null, null);

            head = this.Model.TotalPayrollDoc.FocusedRecord;

            if (head != null
                && head.ID > 0
                )
            {
                HBH.DoNet.DevPlatform.U9Mapping.ProcedureMapping procMapping = new HBH.DoNet.DevPlatform.U9Mapping.ProcedureMapping();
                procMapping.ProcedureName = "HBH_SP_JianLiYuan_SumPayroll";
                procMapping.Params = new List<HBH.DoNet.DevPlatform.U9Mapping.ParamDTO>();

                {
                    HBH.DoNet.DevPlatform.U9Mapping.ParamDTO suptParam = new HBH.DoNet.DevPlatform.U9Mapping.ParamDTO();
                    suptParam.ParamName = "ID";
                    suptParam.ParamType = System.Data.DbType.Int64;
                    //suptParam.ParamDirection = ParameterDirection.Input;
                    suptParam.ParamValue = head.ID;
                    procMapping.Params.Add(suptParam);
                }


                U9.VOB.HBHCommon.Proxy.U9CommonSVProxy proxy = new U9.VOB.HBHCommon.Proxy.U9CommonSVProxy();

                // "HBH.DoNet.DevPlatform.U9Mapping.ProcedureMapping";
                proxy.EntityFullName = typeof(HBH.DoNet.DevPlatform.U9Mapping.ProcedureMapping).FullName;
                proxy.EntityContent = HBH.DoNet.DevPlatform.EntityMapping.EntitySerialization.EntitySerial(procMapping);

                string strResult = proxy.Do();

                if (!PubClass.IsNull(strResult))
                {
                    HBH.DoNet.DevPlatform.EntityMapping.EntityResult result = HBH.DoNet.DevPlatform.EntityMapping.EntitySerialization.EntityDeserial<HBH.DoNet.DevPlatform.EntityMapping.EntityResult>(strResult);

                    if (result != null)
                    {
                        if (result.Sucessfull)
                        {
                            //if (!PubClass.IsNull(result.StringValue))
                            //{
                            //    DataSet ds = EntitySerialization.EntityDeserial<DataSet>(result.StringValue);
                            //}

                            this.Action.NavigateAction.Refresh(null);
                        }
                        else
                        {
                            //U9.VOB.Cus.HBHShangLuo.HBHShangLuoUIsll.WebPart.HBHUIHelper.ShowErrorInfo(this, result.Message);
                            this.Model.ErrorMessage.Message = result.Message;
                        }
                    }
                    else
                    {
                        //U9.VOB.Cus.HBHShangLuo.HBHShangLuoUIsll.WebPart.HBHUIHelper.ShowErrorInfo(this, "执行异常,无返回结果!");
                        this.Model.ErrorMessage.Message = "执行异常,无返回结果!";
                    }
                }
            }
        }

        #endregion
		

		
            
            
            

		#region 自定义数据初始化加载和数据收集
		private void OnLoadData_Extend(object sender)
		{	
			OnLoadData_DefaultImpl(sender);
		}
		private void OnDataCollect_Extend(object sender)
		{	
			OnDataCollect_DefaultImpl(sender);
		}
		#endregion  

        #region 自己扩展 Extended Event handler 
		public void AfterOnLoad()
        {
            string strIsAFKey = UFIDA.U9.UI.PDHelper.FormAuthorityHelper.GetIsApproveDocKey;
            this.CurrentState[strIsAFKey] = new UFIDA.U9.UI.PDHelper.SetIsApprovalDoc(SetIsApprovalDoc);//给出工作流标记

		}

        public void AfterCreateChildControls()
        {
            //查找对话框。
            UFIDA.U9.UI.PDHelper.PDFormMessage.ShowConfirmDialog(this.Page, "f3740355-ee66-46c3-9efe-c573fd2bacf3", "580", "408", Title, wpFindID.ClientID, this.BtnFind, null);

            // 取得提示信息资源：是否删除当前记录
            string message = UFIDA.U9.UI.PDHelper.PDResource.GetDeleteConfirmInfo();
            // 绑定注册弹出对话框到删除按钮 
            UFIDA.U9.UI.PDHelper.PDFormMessage.ShowDelConfirmDialog(this.Page, message, "", this.BtnDelete);

            // 启用页面个性化 
            UFIDA.U9.UI.PDHelper.PersonalizationHelper.SetPersonalizationEnable(this, true);
            // 启用弹性域
            UFIDA.U9.UI.PDHelper.FlexFieldHelper.SetDescFlexField(new UFIDA.U9.UI.PDHelper.DescFlexFieldParameter(this.FlexFieldPicker0, this.Model.TotalPayrollDoc),
                new UFIDA.U9.UI.PDHelper.DescFlexFieldParameter(this.DataGrid5, UISceneHelper.GetSegColumnIndex(this.DataGrid5)));


            // 绑定注册弹出对话框到删除按钮 
            UFIDA.U9.UI.PDHelper.PDFormMessage.ShowDelConfirmDialog(this.Page, "汇总会清空已有明细数据,确认汇总薪资？", "确认汇总薪资？", this.BtnSumPayroll);


            PayrollType65.CustomInParams = string.Format("ValueSetCode={0}", TotalPayrollDocHelper.Const_PayrollTypeValueSetCode);
            
            // 单行返回
            this.SalarySolution90.AddTypeParams(clsMultiSelect.Const_RefType, clsMultiSelect.Const_IsSingleReturn);
            // 单行返回
            this.PayrollCalculate138.AddTypeParams(clsMultiSelect.Const_RefType, clsMultiSelect.Const_IsSingleReturn);


            // 每次都重算汇总数据，要不单据切换时，汇总数不改变
            ((UFWebDataGridAdapter)this.DataGrid5).ResetSumData = true;
            ((UFWebDataGridAdapter)this.DataGrid6).ResetSumData = true;
        }
        
        public void AfterEventBind()
        {
        }
        
		public void BeforeUIModelBinding()
        {

		}

		public void AfterUIModelBinding()
        {
            TotalPayrollDocRecord focusedHead = this.Model.TotalPayrollDoc.FocusedRecord;

            if (focusedHead == null)
                return;

            //U9.VOB.HBHCommon.HBHCommonUI.UISceneHelper.SetToolBarStatus(this.Toolbar2
            //    , status, focusedHead.DataRecordState, false, 0, 1, 2, 2);

            U9.VOB.HBHCommon.HBHCommonUI.UISceneHelper.SetToolBarStatus(this.Toolbar2
                , focusedHead.Status ?? (int)DocStatusData.Empty, focusedHead.DataRecordState, false, (int)DocStatusData.Opened, (int)DocStatusData.Approving, (int)DocStatusData.Approved, 1);


            //this.BtnSave.Enabled = true;

            this.BtnOk.Visible = false;
            this.BtnClose.Visible = false;
            
		}


        #endregion

        #region Customer Method


        private void UpdateStatus(int targetStatus)
        {
            TotalPayrollDocRecord focusedRecord = this.Model.TotalPayrollDoc.FocusedRecord;

            if (focusedRecord != null
                && focusedRecord.ID > 0
                )
            {
                UpdateTotalPayrollDocStatusBPProxy proxy = new UpdateTotalPayrollDocStatusBPProxy();
                proxy.TargetStatus = targetStatus;

                proxy.HeadIDs = new System.Collections.Generic.List<long>();
                proxy.HeadIDs.Add(focusedRecord.ID);

                proxy.Do();

                this.Action.NavigateAction.Refresh(null);
            }
        }




        #region 工作流方式

        /// <summary>
        /// 确认是否工作流方式
        /// </summary>
        /// <param name="model">当前Model</param>
        /// <returns></returns>
        internal static bool SetIsApprovalDoc(IUIModel model)
        {
            bool isAF = false;
            TotalPayrollDocUIModelModel curModel = model as TotalPayrollDocUIModelModel;
            if (curModel != null && curModel.TotalPayrollDoc.FocusedRecord != null)
            {
                TotalPayrollDocRecord rec = curModel.TotalPayrollDoc.FocusedRecord;
                //isAF = rec.BillType_ConfirmType == (int)ConfirmTypeEnumData.ApproveFlow;
                //isAF = rec.IsApproveFlow.GetValueOrDefault(false) ;
                isAF = IsApproveFlow(rec);
            }
            return isAF;
        }

        public static bool IsApproveFlow(TotalPayrollDocRecord record)
        {
            //if (record.IsApproveFlow.GetValueOrDefault(false))
            if (record.ApproveType_Name == DayCheckInUIModel.DayCheckInUIFormWebPart.Const_ApproveFlowName)
            {
                return true;
            }

            return false;
        }

        #endregion

        #endregion
    }
}