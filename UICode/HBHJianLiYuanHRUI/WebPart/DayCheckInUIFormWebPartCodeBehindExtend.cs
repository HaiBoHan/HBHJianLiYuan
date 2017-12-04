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
using System.Collections.Generic;
using U9.VOB.Cus.HBHJianLiYuan.Proxy;
using U9.VOB.HBHCommon.U9CommonBE;
using U9.VOB.HBHCommon.HBHCommonUI;
using UFSoft.UBF.UI.WebControls.Association;
using UFSoft.UBF.UI.WebControls.Association.Adapter;
using U9.VOB.Cus.HBHJianLiYuan;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.WebControlAdapter;
using UFSoft.UBF.UI.Engine.Runtime.GridEdit;
using HBH.DoNet.DevPlatform.EntityMapping;



/***********************************************************************************************
 * Form ID: 
 * UIFactory Auto Generator 
 ***********************************************************************************************/
namespace DayCheckInUIModel
{
    public partial class DayCheckInUIFormWebPart
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

            DayCheckInRecord head = this.Model.DayCheckIn.FocusedRecord;

            if (head != null)
            {
                if (this.Model.DayCheckIn.FieldStatus.DefaultValue != null)
                {
                    head.Status = (int)this.Model.DayCheckIn.FieldStatus.DefaultValue;
                }

                if (this.Model.DayCheckIn.FieldBusinessDate.DefaultValue != null)
                {
                    head.BusinessDate = (DateTime)this.Model.DayCheckIn.FieldBusinessDate.DefaultValue;
                }

                if (this.Model.DayCheckIn.FieldCheckInDate.DefaultValue != null)
                {
                    head.CheckInDate = (DateTime)this.Model.DayCheckIn.FieldCheckInDate.DefaultValue;
                }
            }
		}	
		 
				//BtnSubmit_Click...
		private void BtnSubmit_Click_Extend(object sender, EventArgs  e)
		{
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            //BtnSubmit_Click_DefaultImpl(sender,e);

            this.Model.ClearErrorMessage();

            UpdateStatus((int)DocStatusData.Approving);
		}	
		 
				//BtnApprove_Click...
		private void BtnApprove_Click_Extend(object sender, EventArgs  e)
		{
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            //BtnApprove_Click_DefaultImpl(sender,e);

            this.Model.ClearErrorMessage();

            DayCheckInRecord head = this.Model.DayCheckIn.FocusedRecord;

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

            DayCheckInRecord head = this.Model.DayCheckIn.FocusedRecord;

            if (head != null
                && head.ID > 0
                )
            {
                //if (head.IsApproveFlow.GetValueOrDefault(false))
                //{
                //    UFIDA.U9.UI.PDHelper.PDPopWebPart.ApproveFlow_ApproveBatchUIWebPart(this);
                //}
                //else
                {
                    //ActionHelper.DoApprove(entityKeys, entityFullName);//这里是不走工作流时审核处理
                    UpdateStatus((int)DocStatusData.Opened);
                }
            }
        }
		 
				//BtnUndoApprove_Click...
		private void BtnUndoApprove_Click_Extend(object sender, EventArgs  e)
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
            //BtnList_Click_DefaultImpl(sender,e);
            //this.NavigatePage("Cust_DayCheckIn_UIList", null);

            //U9.VOB.HBHCommon.HBHCommonUI.HBHUIHelper.UIForm_BtnList_Click(this, "DayCheckIn");
            U9.VOB.HBHCommon.HBHCommonUI.HBHUIHelper.UIForm_BtnList_Click(this );
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
            //BtnOk_Click_DefaultImpl(sender,e);

            this.CloseDialog();
		}	
		 
				//BtnClose_Click...
		private void BtnClose_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            //BtnClose_Click_DefaultImpl(sender,e);

            this.CloseDialog();
        }


        //BtnDepartImport_Click...
        private void BtnDepartImport_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            //BtnDepartImport_Click_DefaultImpl(sender,e);

            this.Model.ClearErrorMessage();

            BtnSave_Click_Extend(sender, e);

            if (!this.Model.ErrorMessage.hasErrorMessage)
            {
                if (this.Model.DayCheckIn.FocusedRecord != null)
                {
                    DepartImportCheckInBPProxy proxy = new DepartImportCheckInBPProxy();
                    proxy.CheckInIDs = new List<long>();

                    proxy.CheckInIDs.Add(this.Model.DayCheckIn.FocusedRecord.ID);

                    proxy.Do();
                }

                this.Action.NavigateAction.Refresh(null);
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
            UFIDA.U9.UI.PDHelper.PDFormMessage.ShowConfirmDialog(this.Page, "c1be31b7-e785-4b0f-a6f5-7154b7dbf796", "580", "408", Title, wpFindID.ClientID, this.BtnFind, null);

            // 取得提示信息资源：是否删除当前记录
            string message = UFIDA.U9.UI.PDHelper.PDResource.GetDeleteConfirmInfo();
            // 绑定注册弹出对话框到删除按钮 
            UFIDA.U9.UI.PDHelper.PDFormMessage.ShowDelConfirmDialog(this.Page, message, "", this.BtnDelete);

            // 启用页面个性化 
            UFIDA.U9.UI.PDHelper.PersonalizationHelper.SetPersonalizationEnable(this, true);
            // 启用弹性域
            UFIDA.U9.UI.PDHelper.FlexFieldHelper.SetDescFlexField(new UFIDA.U9.UI.PDHelper.DescFlexFieldParameter(this.FlexFieldPicker0, this.Model.DayCheckIn),
                new UFIDA.U9.UI.PDHelper.DescFlexFieldParameter(this.DataGrid5, UISceneHelper.GetSegColumnIndex(this.DataGrid5)));

            // 确认对话框
            UFIDA.U9.UI.PDHelper.PDFormMessage.ShowDelConfirmDialog(this.Page, "导入将清空原有行，确认导入？", "确认导入", this.BtnDepartImport);


            // DataGrid内Cell变更事件
            Regist_OnChangePostBack_DataGrid5_OnCellDataChanged();

            // 行变更事件
            Regist_OnChangePostBack_DataGrid5_RowChanged();

            // 批量修改
            this.DataGridBatchModify();
        }
        
        public void AfterEventBind()
        {
        }
        
		public void BeforeUIModelBinding()
		{

		}

		public void AfterUIModelBinding()
        {
            DayCheckInRecord focusedHead = this.Model.DayCheckIn.FocusedRecord;

            if (focusedHead == null)
                return;

            //// 如果保存过，不能修改维度，只能删除重新汇总
            //if (focusedHead.ID > 0)
            //{
            //    this.Department116.Enabled = false;
            //}
            //else
            //{
            //    this.Department116.Enabled = true;
            //}

            U9.VOB.HBHCommon.HBHCommonUI.UISceneHelper.SetToolBarStatus(this.Toolbar2
                , focusedHead.Status ?? (int)DocStatusData.Empty, focusedHead.DataRecordState, false, (int)DocStatusData.Opened, (int)DocStatusData.Approving, (int)DocStatusData.Approved, 1);

            DayCheckIn_DayCheckInLineView lineView = this.Model.DayCheckIn_DayCheckInLine;
            DayCheckIn_DayCheckInLineRecord focusedLine = lineView.FocusedRecord;
            if (focusedLine != null)
            {
                IUFDataGridColumn fullDayColumn = this.DataGrid5.Columns[lineView.FieldFullTimeDay.Name];
                IUFDataGridColumn partDayColumn = this.DataGrid5.Columns[lineView.FieldPartTimeDay.Name];

                if (focusedLine.CheckType.GetValueOrDefault(-1) == (int)CheckTypeEnumData.FullTimeStaff)
                {
                    if (fullDayColumn != null)
                    {
                        fullDayColumn.Enabled = true;
                    }
                    if (partDayColumn != null)
                    {
                        partDayColumn.Enabled = false;
                    }
                }
                else if (focusedLine.CheckType.GetValueOrDefault(-1) == (int)CheckTypeEnumData.PartTimeStaff)
                {
                    if (fullDayColumn != null)
                    {
                        fullDayColumn.Enabled = false;
                    }
                    if (partDayColumn != null)
                    {
                        partDayColumn.Enabled = true;
                    }
                }
            }

            this.DataGrid5.AutoEditModel = false;

            //// 审核后修改数据校验，测试
            //this.BtnSave.Enabled = true;

            // 考勤类别不可修改；这个字段放开，万一有问题了，可以有人修改；通过权限先把所有人的权限都做成只读
            //this.DataGrid5.Columns[this.Model.DayCheckIn_DayCheckInLine.FieldCheckType.Name].Enabled = false;

            this.Org38.ReadOnly = false;
            this.Org38.Enabled = true;
		}


        #endregion
		

        #region Customer Method


        private void UpdateStatus(int targetStatus)
        {
            DayCheckInRecord focusedRecord = this.Model.DayCheckIn.FocusedRecord;

            if (focusedRecord != null
                && focusedRecord.ID > 0
                )
            {
                UpdateDayCheckInStatusBPProxy proxy = new UpdateDayCheckInStatusBPProxy();
                proxy.TargetStatus = targetStatus;

                proxy.HeadIDs = new System.Collections.Generic.List<long>();
                proxy.HeadIDs.Add(focusedRecord.ID);

                proxy.Do();

                this.Action.NavigateAction.Refresh(null);
            }
        }


        #region PostBack

        private void Regist_OnChangePostBack_DataGrid5_OnCellDataChanged()
        {
            AssociationControl control = new AssociationControl();
            control.SourceServerControl = DataGrid5;
            control.SourceControl.EventName = "OnCellDataChanged";
            ((IUFClientAssoGrid)control.SourceControl).FireEventCols.Add(this.Model.DayCheckIn_DayCheckInLine.FieldCheckType.Name);
            ((IUFClientAssoGrid)control.SourceControl).FireEventCols.Add(this.Model.DayCheckIn_DayCheckInLine.FieldEmployeeArchive.Name);
            //((IUFClientAssoGrid)control.SourceControl).FireEventCols.Add(FieldName_FinallyPriceTC);
            CodeBlock block = new CodeBlock();
            UFWebClientGridAdapter adapter = new UFWebClientGridAdapter(DataGrid5);
            adapter.IsPostBack = true;
            adapter.PostBackTag = DataGrid5.ID + "_" + control.SourceControl.EventName;
            block.TargetControls.addControl(adapter);
            control.addBlock(block);
            UFGrid dataGrid = DataGrid5 as UFGrid;
            dataGrid.GridCustomerPostBackEvent += new GridCustomerPostBackDelegate(OnChangePostBack_DataGrid5_OnCellDataChanged);
        }

        private void OnChangePostBack_DataGrid5_OnCellDataChanged(object sender, GridCustomerPostBackEventArgs e)
        {
            if (e.PostTag.ToString().EndsWith("OnCellDataChanged")
                )
            {
                //清除错误信息
                this.Model.ClearErrorMessage();

                this.DataCollect();
                this.IsDataBinding = true; //当前事件执行后会进行数据绑定
                this.IsConsuming = false;

                //int qtyIndex = GetColIndex(datagrid, FieldName_ItemID);
                //&& e.ColIndex == qtyIndex

                //string uIFieldID = this.DataGrid5.Columns[e.ColIndex].UIFieldID;
                //if (uIFieldID == view.FieldCust_CustomerItemID.Name)

                string uIFieldID = DataGrid5.Columns[e.ColIndex].UIFieldID;
                if (uIFieldID == this.Model.DayCheckIn_DayCheckInLine.FieldEmployeeArchive.Name)
                {
                    SetCheckTypeByEmployee();
                }
                if (uIFieldID == this.Model.DayCheckIn_DayCheckInLine.FieldCheckType.Name)
                {

                    CheckTypeChanged();
                }
                //// 定价不可改，可能为扩展字段
                //else if (uIFieldID == this.Model.DayCheckIn_DayCheckInLine.FieldOrderPriceTC.Name)
                //{
                //    //    DayCheckIn_DayCheckInLineRecord line = curPart.Model.DayCheckIn_DayCheckInLine.FocusedRecord;

                //    //    // 设置售价状态 (正常售价,高于售价,低于售价)
                //    //    SetPriceStatus(line);

                //    // 手工录入最终价时，如果物料没有价格，物料变更时候没带出来，那么
                //}
            }
        }

        private void SetCheckTypeByEmployee()
        {
            DayCheckIn_DayCheckInLineView lineView = this.Model.DayCheckIn_DayCheckInLine;
            DayCheckIn_DayCheckInLineRecord focusedLine = lineView.FocusedRecord;
            if (focusedLine != null)
            {
                long employeeID = focusedLine.EmployeeArchive.GetValueOrDefault(-1);
                if (employeeID > 0)
                {
                    HBH.DoNet.DevPlatform.U9Mapping.ProcedureMapping procMapping = new HBH.DoNet.DevPlatform.U9Mapping.ProcedureMapping();
                    procMapping.ProcedureName = "HBH_SP_JianLiYuan_GetEmployeeCheckType";
                    procMapping.Params = new List<HBH.DoNet.DevPlatform.U9Mapping.ParamDTO>();

                    {
                        HBH.DoNet.DevPlatform.U9Mapping.ParamDTO suptParam = new HBH.DoNet.DevPlatform.U9Mapping.ParamDTO();
                        suptParam.ParamName = "ID";
                        suptParam.ParamType = System.Data.DbType.Int64;
                        //suptParam.ParamDirection = ParameterDirection.Input;
                        suptParam.ParamValue = employeeID;
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
                                if (!PubClass.IsNull(result.StringValue))
                                {
                                    DataSet ds = EntitySerialization.EntityDeserial<DataSet>(result.StringValue);
                                    DataTable dt = ds.GetTableOfFirst();

                                    if (dt != null
                                        && dt.Rows != null
                                        && dt.Rows.Count > 0
                                        )
                                    {
                                        int checkType = dt.Rows[0][0].GetInt();

                                        if (focusedLine.CheckType != checkType)
                                        {
                                            focusedLine.CheckType = checkType;

                                            CheckTypeChanged();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //U9.VOB.Cus.HBHShangLuo.HBHShangLuoUIsll.WebPart.HBHUIHelper.ShowErrorInfo(this, result.Message);
                                this.Model.ErrorMessage.Message = result.Message;
                            }
                        }
                        else
                        {

                        }
                    }
                }
            }
        }

        private void CheckTypeChanged()
        {
            DayCheckIn_DayCheckInLineView lineView = this.Model.DayCheckIn_DayCheckInLine;
            DayCheckIn_DayCheckInLineRecord focusedLine = lineView.FocusedRecord;
            if (focusedLine != null)
            {

                if (focusedLine.CheckType.GetValueOrDefault(-1) == (int)CheckTypeEnumData.FullTimeStaff)
                {
                    focusedLine.FullTimeDay = 1;
                    focusedLine.PartTimeDay = 0;
                }
                else if (focusedLine.CheckType.GetValueOrDefault(-1) == (int)CheckTypeEnumData.PartTimeStaff)
                {
                    focusedLine.FullTimeDay = 0;
                    focusedLine.PartTimeDay = 4;
                }
            }
        }


        #endregion


        #region RowChanged

        // UBF设置点击事件，刷新太大，无法删除行
        private void Regist_OnChangePostBack_DataGrid5_RowChanged()
        {
            //GridPostBack(this.DataGrid5);
            AssociationControl control = new AssociationControl();
            control.SourceServerControl = (DataGrid5);
            control.SourceControl.EventName = "OnRowChanged";
            CodeBlock block = new CodeBlock();
            UFWebClientGridAdapter adapter = new UFWebClientGridAdapter(DataGrid5);
            adapter.IsPostBack = (true);
            adapter.PostBackTag = (DataGrid5.ID);
            block.TargetControls.addControl(adapter);
            control.addBlock(block);
            UFGrid grid = DataGrid5 as UFGrid;
            grid.GridCustomerPostBackEvent += new GridCustomerPostBackDelegate(this.OnChangePostBack_DataGrid5_RowChanged);
        }
        
        private void OnChangePostBack_DataGrid5_RowChanged(object sender, GridCustomerPostBackEventArgs e)
        {
            this.OnDataCollect(this);
            base.IsDataBinding = true;
            base.IsConsuming = false;

            //// UBF设置点击事件，刷新太大，无法删除行
            //DataGrid5_GridRowClicked_Extend(null, null);
        }
                
        #endregion


        #region DataGrid批量修改

        /*
        http://121.42.141.121/U9/ufsoft/GridEdit.aspx?__sk=__SK92038&__curOId=1001510020000918&IsCondtionBatch=true
         */


        private void DataGridBatchModify()
        {
            List<string> list = new List<string>();
            list.Add(this.Model.DayCheckIn_DayCheckInLine.FieldDocLineNo.Name);
            list.Add(this.Model.DayCheckIn_DayCheckInLine.FieldEmployeeArchive.Name);
            list.Add(this.Model.DayCheckIn_DayCheckInLine.FieldCheckType.Name);
            list.Add(this.Model.DayCheckIn_DayCheckInLine.FieldFullTimeDay.Name);
            list.Add(this.Model.DayCheckIn_DayCheckInLine.FieldPartTimeDay.Name);
            list.Add(this.Model.DayCheckIn_DayCheckInLine.FieldHourlyDay.Name);
            list.Add(this.Model.DayCheckIn_DayCheckInLine.FieldDuty.Name);
            list.Add(this.Model.DayCheckIn_DayCheckInLine.FieldMemo.Name);
            for (int i = 1; i < 50; i++)
            {
                list.Add("DescFlexField_PubDescSeg" + i);
            }
            for (int i = 1; i < 30; i++)
            {
                list.Add("DescFlexField_PrivateDescSeg" + i);
            }
            for (int i = 1; i < 50; i++)
            {
                list.Add("DescFlexField_PubDescSeg" + i + "_ID");
            }
            for (int i = 1; i < 30; i++)
            {
                list.Add("DescFlexField_PrivateDescSeg" + i + "_ID");
            }
            ((UFWebDataGridAdapter)this.DataGrid5).RegisterGridBatchModifyEvent(new GridBatchModifyDelegate(this.GridBatchModify_ProcessEvent), list);
        }

        private void GridBatchModify_ProcessEvent(object sender, GridBatchModifyArgs e)
        {
            this.OnDataCollect(this);
            base.IsDataBinding = true;
            base.IsConsuming = false;

            UFSoft.UBF.UI.ControlModel.IUFDataGrid iUFDataGrid = (UFSoft.UBF.UI.ControlModel.IUFDataGrid)sender;
            UFSoft.UBF.UI.MD.Runtime.IUIRecordCollection iUIRecordCollection = e.HasCondition ? e.Condition.GetEffectedData() : iUFDataGrid.UIView.Records;
            if (iUIRecordCollection != null && iUIRecordCollection.Count > 0 && e.Values.Count > 0)
            {
                string fieldID = e.Values[0].FieldID;
                //Dictionary<long, UFIDA.U9.CBO.SCM.DTOs.POTaskRefUIDTOData> dictionary = new Dictionary<long, UFIDA.U9.CBO.SCM.DTOs.POTaskRefUIDTOData>();
                foreach (UFSoft.UBF.UI.MD.Runtime.IUIRecord current in iUIRecordCollection)
                {
                    DayCheckIn_DayCheckInLineRecord lineRecord = current as DayCheckIn_DayCheckInLineRecord;
                    foreach (EditItem current2 in e.Values)
                    {
                        if (!(current2.FieldID != fieldID))
                        {
                            current[current2.FieldID] = current2.Value;
                            if (fieldID.Contains("DescFlexField_PubDescSeg"))
                            {
                                if (current2.RefValues != null && current2.RefValues.Count > 0)
                                {
                                    string text = fieldID.Substring(0, fieldID.IndexOf("_ID"));
                                    if (current2.RefValues.ContainsKey("Code"))
                                    {
                                        lineRecord.SetValue(text, current2.RefValues["Code"]);
                                    }
                                    if (current2.RefValues.ContainsKey("Name"))
                                    {
                                        lineRecord.SetValue(text + "_Name", current2.RefValues["Name"]);
                                    }
                                }
                            }
                            else if (fieldID.Contains("DescFlexField_PrivateDescSeg"))
                            {
                                if (current2.RefValues != null && current2.RefValues.Count > 0)
                                {
                                    string text = fieldID.Substring(0, fieldID.IndexOf("_ID"));
                                    if (current2.RefValues.ContainsKey("Code"))
                                    {
                                        lineRecord.SetValue(text, current2.RefValues["Code"]);
                                    }
                                    if (current2.RefValues.ContainsKey("Name"))
                                    {
                                        lineRecord.SetValue(text + "_Name", current2.RefValues["Name"]);
                                    }
                                }
                            }
                            else // if (fieldID.Contains("DescFlexField_PrivateDescSeg"))
                            {
                                switch (fieldID)
                                {
                                    //case "DocLineNo":
                                    //    if (current2.Value is string)
                                    //    {
                                    //        string text3 = current2.Value as string;
                                    //        lineRecord.RequiredDeliveryDate = (string.IsNullOrEmpty(text3) ? null : new DateTime?(Convert.ToDateTime(text3)));
                                    //    }
                                    //    else
                                    //    {
                                    //        lineRecord.RequiredDeliveryDate = new DateTime?(Convert.ToDateTime(current2.Value));
                                    //    }
                                    //    this.Action.DeliveryDateDeal(lineRecord);
                                    //    break;
                                    case "EmployeeArchive":
                                        if (current2.Value is string)
                                        {
                                            string text3 = current2.Value as string;
                                            lineRecord.EmployeeArchive = new long?(string.IsNullOrEmpty(text3) ? -1L : Convert.ToInt64(text3));
                                        }
                                        else
                                        {
                                            lineRecord.EmployeeArchive = new long?(Convert.ToInt64(current2.Value));
                                        }
                                        if (current2.RefValues != null && current2.RefValues.Count > 0)
                                        {
                                            if (current2.RefValues.ContainsKey("EmployeeCode"))
                                            {
                                                //lineRecord.EmployeeArchive_Code = current2.RefValues["Code"];
                                                lineRecord.EmployeeArchive_EmployeeCode = current2.RefValues["EmployeeCode"];
                                            }
                                            if (current2.RefValues.ContainsKey("Name"))
                                            {
                                                lineRecord.EmployeeArchive_Name = current2.RefValues["Name"];
                                            }
                                        }
                                        break;
                                    case "CheckType":
                                        //break;
                                    case "DocLineNo":
                                    case "FullTimeDay":
                                    case "PartTimeDay":
                                    case "HourlyDay":
                                    case "Duty":
                                    case "Memo":
                                        //if (current2.Value is string)
                                        //{
                                        //    string text3 = current2.Value as string;
                                        //    lineRecord.ReqQtyTU = new decimal?(string.IsNullOrEmpty(text3) ? 0m : decimal.Parse(text3));
                                        //}
                                        //else
                                        //{
                                        //    lineRecord.ReqQtyTU = new decimal?(Convert.ToDecimal(current2.Value));
                                        //}
                                        //this.Action.RequestQtyAssoApproveQty(lineRecord, PRUIModelAction.SourceUomType.TU);
                                        //this.Action.LineQtyChanged(lineRecord, PRUIModelAction.SourceUomType.TU, PRUIModelAction.QtyType.Request);
                                        lineRecord.SetValue(fieldID, current2.Value);
                                        break;
                                    //case "Duty":
                                    //case "Memo":
                                    //    if (current2.Value is string)
                                    //    {
                                    //        string text3 = current2.Value as string;
                                    //        lineRecord.DemandCode = new int?(string.IsNullOrEmpty(text3) ? -1 : Convert.ToInt32(text3));
                                    //    }
                                    //    else
                                    //    {
                                    //        lineRecord.DemandCode = new int?(Convert.ToInt32(current2.Value));
                                    //    }
                                    //    break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        #region Disabled  引用的

        // UFIDA.U9.SCM.PM.PRUIModel.PRMainUIFormWebPart
        // DataGridBatchModify()

        //private void GridBatchModify_ProcessEvent(object sender, GridBatchModifyArgs e)
        //{
        //    this.OnDataCollect(this);
        //    base.IsDataBinding = true;
        //    base.IsConsuming = false;

        //    UFSoft.UBF.UI.ControlModel.IUFDataGrid iUFDataGrid = (UFSoft.UBF.UI.ControlModel.IUFDataGrid)sender;
        //    UFSoft.UBF.UI.MD.Runtime.IUIRecordCollection iUIRecordCollection = e.HasCondition ? e.Condition.GetEffectedData() : iUFDataGrid.UIView.Records;
        //    if (iUIRecordCollection != null && iUIRecordCollection.Count > 0 && e.Values.Count > 0)
        //    {
        //        string fieldID = e.Values[0].FieldID;
        //        Dictionary<long, UFIDA.U9.CBO.SCM.DTOs.POTaskRefUIDTOData> dictionary = new Dictionary<long, UFIDA.U9.CBO.SCM.DTOs.POTaskRefUIDTOData>();
        //        foreach (UFSoft.UBF.UI.MD.Runtime.IUIRecord current in iUIRecordCollection)
        //        {
        //            PR_PRLineListRecord pR_PRLineListRecord = current as PR_PRLineListRecord;
        //            foreach (EditItem current2 in e.Values)
        //            {
        //                if (!(current2.FieldID != fieldID))
        //                {
        //                    current[current2.FieldID] = current2.Value;
        //                    if (fieldID.Contains("DescFlexField_PubDescSeg"))
        //                    {
        //                        if (current2.RefValues != null && current2.RefValues.Count > 0)
        //                        {
        //                            string text = fieldID.Substring(0, fieldID.IndexOf("_ID"));
        //                            if (current2.RefValues.ContainsKey("Code"))
        //                            {
        //                                pR_PRLineListRecord.SetValue(text, current2.RefValues["Code"]);
        //                            }
        //                            if (current2.RefValues.ContainsKey("Name"))
        //                            {
        //                                pR_PRLineListRecord.SetValue(text + "_Name", current2.RefValues["Name"]);
        //                            }
        //                        }
        //                    }
        //                    if (fieldID.Contains("DescFlexField_PrivateDescSeg"))
        //                    {
        //                        if (current2.RefValues != null && current2.RefValues.Count > 0)
        //                        {
        //                            string text = fieldID.Substring(0, fieldID.IndexOf("_ID"));
        //                            if (current2.RefValues.ContainsKey("Code"))
        //                            {
        //                                pR_PRLineListRecord.SetValue(text, current2.RefValues["Code"]);
        //                            }
        //                            if (current2.RefValues.ContainsKey("Name"))
        //                            {
        //                                pR_PRLineListRecord.SetValue(text + "_Name", current2.RefValues["Name"]);
        //                            }
        //                        }
        //                    }
        //                    string text2 = fieldID;
        //                    switch (text2)
        //                    {
        //                        case "RequiredDeliveryDate":
        //                            if (current2.Value is string)
        //                            {
        //                                string text3 = current2.Value as string;
        //                                pR_PRLineListRecord.RequiredDeliveryDate = (string.IsNullOrEmpty(text3) ? null : new DateTime?(Convert.ToDateTime(text3)));
        //                            }
        //                            else
        //                            {
        //                                pR_PRLineListRecord.RequiredDeliveryDate = new DateTime?(Convert.ToDateTime(current2.Value));
        //                            }
        //                            this.Action.DeliveryDateDeal(pR_PRLineListRecord);
        //                            break;
        //                        case "ReqQtyTU":
        //                            if (current2.Value is string)
        //                            {
        //                                string text3 = current2.Value as string;
        //                                pR_PRLineListRecord.ReqQtyTU = new decimal?(string.IsNullOrEmpty(text3) ? 0m : decimal.Parse(text3));
        //                            }
        //                            else
        //                            {
        //                                pR_PRLineListRecord.ReqQtyTU = new decimal?(Convert.ToDecimal(current2.Value));
        //                            }
        //                            this.Action.RequestQtyAssoApproveQty(pR_PRLineListRecord, PRUIModelAction.SourceUomType.TU);
        //                            this.Action.LineQtyChanged(pR_PRLineListRecord, PRUIModelAction.SourceUomType.TU, PRUIModelAction.QtyType.Request);
        //                            break;
        //                        case "DemandCode":
        //                            if (current2.Value is string)
        //                            {
        //                                string text3 = current2.Value as string;
        //                                pR_PRLineListRecord.DemandCode = new int?(string.IsNullOrEmpty(text3) ? -1 : Convert.ToInt32(text3));
        //                            }
        //                            else
        //                            {
        //                                pR_PRLineListRecord.DemandCode = new int?(Convert.ToInt32(current2.Value));
        //                            }
        //                            break;
        //                        case "Warehouse":
        //                            if (current2.Value is string)
        //                            {
        //                                string text3 = current2.Value as string;
        //                                pR_PRLineListRecord.Warehouse = new long?(string.IsNullOrEmpty(text3) ? -1L : Convert.ToInt64(text3));
        //                            }
        //                            else
        //                            {
        //                                pR_PRLineListRecord.Warehouse = new long?(Convert.ToInt64(current2.Value));
        //                            }
        //                            if (current2.RefValues != null && current2.RefValues.Count > 0)
        //                            {
        //                                if (current2.RefValues.ContainsKey("Code"))
        //                                {
        //                                    pR_PRLineListRecord.Warehouse_Code = current2.RefValues["Code"];
        //                                }
        //                                if (current2.RefValues.ContainsKey("Name"))
        //                                {
        //                                    pR_PRLineListRecord.Warehouse_Name = current2.RefValues["Name"];
        //                                }
        //                            }
        //                            break;
        //                        case "SuggestedSupplier_Supplier":
        //                            if (current2.Value is string)
        //                            {
        //                                string text3 = current2.Value as string;
        //                                pR_PRLineListRecord.SuggestedSupplier_Supplier = new long?(string.IsNullOrEmpty(text3) ? -1L : Convert.ToInt64(text3));
        //                            }
        //                            else
        //                            {
        //                                pR_PRLineListRecord.SuggestedSupplier_Supplier = new long?(Convert.ToInt64(current2.Value));
        //                            }
        //                            if (current2.RefValues != null && current2.RefValues.Count > 0)
        //                            {
        //                                if (current2.RefValues.ContainsKey("Code"))
        //                                {
        //                                    pR_PRLineListRecord.SuggestedSupplier_Code = current2.RefValues["Code"];
        //                                }
        //                                if (current2.RefValues.ContainsKey("Name"))
        //                                {
        //                                    pR_PRLineListRecord.SuggestedSupplier_Name = current2.RefValues["Name"];
        //                                }
        //                                if (current2.RefValues.ContainsKey("1"))
        //                                {
        //                                    pR_PRLineListRecord.SuggestedSupplier_Supplier_IsMISC = false;
        //                                }
        //                            }
        //                            break;
        //                        case "ReqEmployee":
        //                            if (!this.IsLineReqInfoCanNotChangeForBatch(pR_PRLineListRecord))
        //                            {
        //                                if (current2.Value is string)
        //                                {
        //                                    string text3 = current2.Value as string;
        //                                    pR_PRLineListRecord.ReqEmployee = new long?(string.IsNullOrEmpty(text3) ? -1L : Convert.ToInt64(text3));
        //                                }
        //                                else
        //                                {
        //                                    pR_PRLineListRecord.ReqEmployee = new long?(Convert.ToInt64(current2.Value));
        //                                }
        //                                if (current2.RefValues != null && current2.RefValues.Count > 0)
        //                                {
        //                                    if (current2.RefValues.ContainsKey("Code"))
        //                                    {
        //                                        pR_PRLineListRecord.ReqEmployee_Code = current2.RefValues["Code"];
        //                                    }
        //                                    if (current2.RefValues.ContainsKey("Name"))
        //                                    {
        //                                        pR_PRLineListRecord.ReqEmployee_Name = current2.RefValues["Name"];
        //                                    }
        //                                    if (current2.RefValues.ContainsKey("Dept_ID"))
        //                                    {
        //                                        pR_PRLineListRecord.ReqDept = new long?(string.IsNullOrEmpty(current2.RefValues["Dept_ID"]) ? -1L : Convert.ToInt64(current2.RefValues["Dept_ID"]));
        //                                    }
        //                                    if (current2.RefValues.ContainsKey("Dept_Code"))
        //                                    {
        //                                        pR_PRLineListRecord.ReqDept_Code = current2.RefValues["Dept_Code"];
        //                                    }
        //                                    if (current2.RefValues.ContainsKey("Dept_Name"))
        //                                    {
        //                                        pR_PRLineListRecord.ReqDept_Name = current2.RefValues["Dept_Name"];
        //                                    }
        //                                }
        //                            }
        //                            break;
        //                        case "ReqDept":
        //                            if (!this.IsLineReqInfoCanNotChangeForBatch(pR_PRLineListRecord))
        //                            {
        //                                bool flag = true;
        //                                if (current2.Value is string)
        //                                {
        //                                    string text3 = current2.Value as string;
        //                                    long num2 = string.IsNullOrEmpty(text3) ? -1L : Convert.ToInt64(text3);
        //                                    if (num2 == pR_PRLineListRecord.ReqDept.GetValueOrDefault(-1L))
        //                                    {
        //                                        flag = false;
        //                                    }
        //                                    pR_PRLineListRecord.ReqDept = new long?(num2);
        //                                }
        //                                else
        //                                {
        //                                    long num2 = Convert.ToInt64(current2.Value);
        //                                    if (num2 == pR_PRLineListRecord.ReqDept.GetValueOrDefault(-1L))
        //                                    {
        //                                        flag = false;
        //                                    }
        //                                    pR_PRLineListRecord.ReqDept = new long?(num2);
        //                                }
        //                                if (current2.RefValues != null && current2.RefValues.Count > 0)
        //                                {
        //                                    if (current2.RefValues.ContainsKey("Code"))
        //                                    {
        //                                        pR_PRLineListRecord.ReqDept_Code = current2.RefValues["Code"];
        //                                    }
        //                                    if (current2.RefValues.ContainsKey("Name"))
        //                                    {
        //                                        pR_PRLineListRecord.ReqDept_Name = current2.RefValues["Name"];
        //                                    }
        //                                    if (flag)
        //                                    {
        //                                        pR_PRLineListRecord.ReqEmployee = new long?(-1L);
        //                                        pR_PRLineListRecord.ReqEmployee_Code = "";
        //                                        pR_PRLineListRecord.ReqEmployee_Name = "";
        //                                    }
        //                                }
        //                            }
        //                            break;
        //                        case "PurPerson":
        //                            if (!this.IsLinePurInfoCanNotChange(pR_PRLineListRecord))
        //                            {
        //                                if (current2.Value is string)
        //                                {
        //                                    string text3 = current2.Value as string;
        //                                    pR_PRLineListRecord.PurPerson = new long?(string.IsNullOrEmpty(text3) ? -1L : Convert.ToInt64(text3));
        //                                }
        //                                else
        //                                {
        //                                    pR_PRLineListRecord.PurPerson = new long?(Convert.ToInt64(current2.Value));
        //                                }
        //                                if (current2.RefValues != null && current2.RefValues.Count > 0)
        //                                {
        //                                    if (current2.RefValues.ContainsKey("Code"))
        //                                    {
        //                                        pR_PRLineListRecord.PurPerson_Code = current2.RefValues["Code"];
        //                                    }
        //                                    if (current2.RefValues.ContainsKey("Name"))
        //                                    {
        //                                        pR_PRLineListRecord.PurPerson_Name = current2.RefValues["Name"];
        //                                    }
        //                                    if (current2.RefValues.ContainsKey("Dept_ID"))
        //                                    {
        //                                        pR_PRLineListRecord.PurDepartment = new long?(string.IsNullOrEmpty(current2.RefValues["Dept_ID"]) ? -1L : Convert.ToInt64(current2.RefValues["Dept_ID"]));
        //                                    }
        //                                    if (current2.RefValues.ContainsKey("Dept_Code"))
        //                                    {
        //                                        pR_PRLineListRecord.PurDepartment_Code = current2.RefValues["Dept_Code"];
        //                                    }
        //                                    if (current2.RefValues.ContainsKey("Dept_Name"))
        //                                    {
        //                                        pR_PRLineListRecord.PurDepartment_Name = current2.RefValues["Dept_Name"];
        //                                    }
        //                                }
        //                            }
        //                            break;
        //                        case "PurDepartment":
        //                            if (!this.IsLinePurInfoCanNotChange(pR_PRLineListRecord))
        //                            {
        //                                bool flag2 = true;
        //                                if (current2.Value is string)
        //                                {
        //                                    string text3 = current2.Value as string;
        //                                    long num2 = string.IsNullOrEmpty(text3) ? -1L : Convert.ToInt64(text3);
        //                                    if (num2 == pR_PRLineListRecord.ReqDept.GetValueOrDefault(-1L))
        //                                    {
        //                                        flag2 = false;
        //                                    }
        //                                    pR_PRLineListRecord.PurDepartment = new long?(num2);
        //                                }
        //                                else
        //                                {
        //                                    long num2 = Convert.ToInt64(current2.Value);
        //                                    if (num2 == pR_PRLineListRecord.ReqDept.GetValueOrDefault(-1L))
        //                                    {
        //                                        flag2 = false;
        //                                    }
        //                                    pR_PRLineListRecord.PurDepartment = new long?(num2);
        //                                }
        //                                if (current2.RefValues != null && current2.RefValues.Count > 0)
        //                                {
        //                                    if (current2.RefValues.ContainsKey("Code"))
        //                                    {
        //                                        pR_PRLineListRecord.PurDepartment_Code = current2.RefValues["Code"];
        //                                    }
        //                                    if (current2.RefValues.ContainsKey("Name"))
        //                                    {
        //                                        pR_PRLineListRecord.PurDepartment_Name = current2.RefValues["Name"];
        //                                    }
        //                                    if (flag2)
        //                                    {
        //                                        pR_PRLineListRecord.PurPerson = new long?(-1L);
        //                                        pR_PRLineListRecord.PurPerson_Code = "";
        //                                        pR_PRLineListRecord.PurPerson_Name = "";
        //                                    }
        //                                }
        //                            }
        //                            break;
        //                        case "AccountOrg":
        //                            if (current2.Value is string)
        //                            {
        //                                string text3 = current2.Value as string;
        //                                pR_PRLineListRecord.AccountOrg = new long?(string.IsNullOrEmpty(text3) ? -1L : Convert.ToInt64(text3));
        //                            }
        //                            else
        //                            {
        //                                pR_PRLineListRecord.AccountOrg = new long?(Convert.ToInt64(current2.Value));
        //                            }
        //                            if (current2.RefValues != null && current2.RefValues.Count > 0)
        //                            {
        //                                if (current2.RefValues.ContainsKey("Code"))
        //                                {
        //                                    pR_PRLineListRecord.AccountOrg_Code = current2.RefValues["Code"];
        //                                }
        //                                if (current2.RefValues.ContainsKey("Name"))
        //                                {
        //                                    pR_PRLineListRecord.AccountOrg_Name = current2.RefValues["Name"];
        //                                }
        //                            }
        //                            break;
        //                        case "ShipperOrg":
        //                            if (current2.Value is string)
        //                            {
        //                                string text3 = current2.Value as string;
        //                                pR_PRLineListRecord.ShipperOrg = new long?(string.IsNullOrEmpty(text3) ? -1L : Convert.ToInt64(text3));
        //                            }
        //                            else
        //                            {
        //                                pR_PRLineListRecord.ShipperOrg = new long?(Convert.ToInt64(current2.Value));
        //                            }
        //                            if (current2.RefValues != null && current2.RefValues.Count > 0)
        //                            {
        //                                if (current2.RefValues.ContainsKey("Code"))
        //                                {
        //                                    pR_PRLineListRecord.ShipperOrg_Code = current2.RefValues["Code"];
        //                                }
        //                                if (current2.RefValues.ContainsKey("Name"))
        //                                {
        //                                    pR_PRLineListRecord.ShipperOrg_Name = current2.RefValues["Name"];
        //                                }
        //                            }
        //                            break;
        //                        case "RcvOrg":
        //                            if (current2.Value is string)
        //                            {
        //                                string text3 = current2.Value as string;
        //                                pR_PRLineListRecord.RcvOrg = new long?(string.IsNullOrEmpty(text3) ? -1L : Convert.ToInt64(text3));
        //                            }
        //                            else
        //                            {
        //                                pR_PRLineListRecord.RcvOrg = new long?(Convert.ToInt64(current2.Value));
        //                            }
        //                            if (current2.RefValues != null && current2.RefValues.Count > 0)
        //                            {
        //                                if (current2.RefValues.ContainsKey("Code"))
        //                                {
        //                                    pR_PRLineListRecord.RcvOrg_Code = current2.RefValues["Code"];
        //                                }
        //                                if (current2.RefValues.ContainsKey("Name"))
        //                                {
        //                                    pR_PRLineListRecord.RcvOrg_Name = current2.RefValues["Name"];
        //                                }
        //                            }
        //                            this.LineRcvOrgChange(pR_PRLineListRecord);
        //                            break;
        //                        case "SeiBanMaster":
        //                            {
        //                                long num3;
        //                                if (current2.Value is string)
        //                                {
        //                                    string text3 = current2.Value as string;
        //                                    num3 = (string.IsNullOrEmpty(text3) ? -1L : Convert.ToInt64(text3));
        //                                }
        //                                else
        //                                {
        //                                    num3 = Convert.ToInt64(current2.Value);
        //                                }
        //                                if (!this.LineSeibanCanNotChange(pR_PRLineListRecord, num3))
        //                                {
        //                                    pR_PRLineListRecord.SeiBanMaster = new long?(num3);
        //                                    if (current2.RefValues != null && current2.RefValues.Count > 0)
        //                                    {
        //                                        if (current2.RefValues.ContainsKey("SeibanNO"))
        //                                        {
        //                                            pR_PRLineListRecord.SeiBanMaster_SeibanNO = current2.RefValues["SeibanNO"];
        //                                            pR_PRLineListRecord.SeiBanCode = current2.RefValues["SeibanNO"];
        //                                        }
        //                                    }
        //                                }
        //                                break;
        //                            }
        //                        case "Project":
        //                            {
        //                                bool flag3 = true;
        //                                if (current2.Value is string)
        //                                {
        //                                    string text3 = current2.Value as string;
        //                                    long num4 = string.IsNullOrEmpty(text3) ? -1L : Convert.ToInt64(text3);
        //                                    if (num4 == pR_PRLineListRecord.Project.GetValueOrDefault(-1L))
        //                                    {
        //                                        flag3 = false;
        //                                    }
        //                                    pR_PRLineListRecord.Project = new long?(num4);
        //                                }
        //                                else
        //                                {
        //                                    long num4 = Convert.ToInt64(current2.Value);
        //                                    if (num4 == pR_PRLineListRecord.Project.GetValueOrDefault(-1L))
        //                                    {
        //                                        flag3 = false;
        //                                    }
        //                                    pR_PRLineListRecord.Project = new long?(num4);
        //                                }
        //                                if (current2.RefValues != null && current2.RefValues.Count > 0)
        //                                {
        //                                    if (current2.RefValues.ContainsKey("Code"))
        //                                    {
        //                                        pR_PRLineListRecord.Project_Code = current2.RefValues["Code"];
        //                                    }
        //                                    if (current2.RefValues.ContainsKey("Name"))
        //                                    {
        //                                        pR_PRLineListRecord.Project_Name = current2.RefValues["Name"];
        //                                    }
        //                                    if (flag3)
        //                                    {
        //                                        pR_PRLineListRecord.Task = new long?(-1L);
        //                                        pR_PRLineListRecord.Task_Code = "";
        //                                        pR_PRLineListRecord.Task_Name = "";
        //                                    }
        //                                }
        //                                break;
        //                            }
        //                        case "Task":
        //                            {
        //                                long num5;
        //                                if (current2.Value is string)
        //                                {
        //                                    string text3 = current2.Value as string;
        //                                    num5 = (string.IsNullOrEmpty(text3) ? -1L : Convert.ToInt64(text3));
        //                                }
        //                                else
        //                                {
        //                                    num5 = Convert.ToInt64(current2.Value);
        //                                }
        //                                UFIDA.U9.CBO.SCM.DTOs.POTaskRefUIDTOData pOTaskRefUIDTOData;
        //                                if (dictionary.ContainsKey(num5))
        //                                {
        //                                    pOTaskRefUIDTOData = dictionary[num5];
        //                                }
        //                                else
        //                                {
        //                                    pOTaskRefUIDTOData = this.Action.GetTaskRefDTO(num5);
        //                                    dictionary.Add(num5, pOTaskRefUIDTOData);
        //                                }
        //                                bool flag4 = true;
        //                                if (pOTaskRefUIDTOData != null && pOTaskRefUIDTOData.TaskOutput > 0L && pR_PRLineListRecord.ItemInfo_ItemID.GetValueOrDefault(-1L) != pOTaskRefUIDTOData.ItemInfo.ItemID)
        //                                {
        //                                    flag4 = false;
        //                                }
        //                                if (flag4)
        //                                {
        //                                    pR_PRLineListRecord.Task = new long?(num5);
        //                                    if (current2.RefValues != null && current2.RefValues.Count > 0)
        //                                    {
        //                                        if (current2.RefValues.ContainsKey("Code"))
        //                                        {
        //                                            pR_PRLineListRecord.Task_Code = current2.RefValues["Code"];
        //                                        }
        //                                        if (current2.RefValues.ContainsKey("Name"))
        //                                        {
        //                                            pR_PRLineListRecord.Task_Name = current2.RefValues["Name"];
        //                                        }
        //                                    }
        //                                    this.Action.LineTaskChanged(pR_PRLineListRecord, pOTaskRefUIDTOData);
        //                                }
        //                                break;
        //                            }
        //                        case "RegOrg":
        //                            if (current2.Value is string)
        //                            {
        //                                string text3 = current2.Value as string;
        //                                pR_PRLineListRecord.RegOrg = (string.IsNullOrEmpty(text3) ? -1L : Convert.ToInt64(text3));
        //                            }
        //                            else
        //                            {
        //                                pR_PRLineListRecord.RegOrg = Convert.ToInt64(current2.Value);
        //                            }
        //                            if (current2.RefValues != null && current2.RefValues.Count > 0)
        //                            {
        //                                if (current2.RefValues.ContainsKey("Code"))
        //                                {
        //                                    pR_PRLineListRecord.RegOrg_Code = current2.RefValues["Code"];
        //                                }
        //                                if (current2.RefValues.ContainsKey("Name"))
        //                                {
        //                                    pR_PRLineListRecord.RegOrg_Name = current2.RefValues["Name"];
        //                                }
        //                            }
        //                            break;
        //                    }
        //                }
        //            }
        //        }
        //        if (dictionary.Count >= 0)
        //        {
        //            base.CurrentState["____PO_Task_RefDTO____UIStateTag"] = null;
        //        }
        //    }
        //}

        #endregion

        #endregion



        #region 工作流方式

        /// <summary>
        /// 确认是否工作流方式
        /// </summary>
        /// <param name="model">当前Model</param>
        /// <returns></returns>
        internal static bool SetIsApprovalDoc(IUIModel model)
        {
            bool isAF = false;
            DayCheckInUIModelModel curModel = model as DayCheckInUIModelModel;
            if (curModel != null && curModel.DayCheckIn.FocusedRecord != null)
            {
                DayCheckInRecord rec = curModel.DayCheckIn.FocusedRecord;
                //isAF = rec.BillType_ConfirmType == (int)ConfirmTypeEnumData.ApproveFlow;
                //isAF = rec.IsApproveFlow.GetValueOrDefault(false) ;
                isAF = IsApproveFlow(rec);
            }
            return isAF;
        }

        public static bool IsApproveFlow(DayCheckInRecord record)
        {
            //if (record.IsApproveFlow.GetValueOrDefault(false))
            if (record.ApproveType_Name == Const_ApproveFlowName)
            {
                return true;
            }

            return false;
        }

        /*
        ApproveFlow     ComfirmWork     InTimeComfirm
         */
        public const string Const_ApproveFlowName = "ApproveFlow";

        #endregion


        #endregion
    }
}