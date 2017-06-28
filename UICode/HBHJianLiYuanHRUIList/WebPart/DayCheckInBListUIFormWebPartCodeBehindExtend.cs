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
using U9.VOB.HBHCommon.U9CommonBE;
using U9.VOB.Cus.HBHJianLiYuan.Proxy;
using System.Collections.Generic;
using HBH.DoNet.DevPlatform.EntityMapping;



/***********************************************************************************************
 * Form ID: 
 * UIFactory Auto Generator 
 ***********************************************************************************************/
namespace DayCheckInBListUIModel
{
    public partial class DayCheckInBListUIFormWebPart
    {
        #region Custome eventBind
	
		 
				//BtnNew_Click...
		private void BtnNew_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            //BtnNew_Click_DefaultImpl(sender, e);
            U9.VOB.HBHCommon.HBHCommonUI.HBHUIHelper.UIList_BtnNew_Click(this);
		}	
		 
				//BtnDelete_Click...
		private void BtnDelete_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnDelete_Click_DefaultImpl(sender,e);
		}	
		 
				//BtnOutPut_Click...
		private void BtnOutPut_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnOutPut_Click_DefaultImpl(sender,e);
		}	
		 
				//BtnPrint_Click...
		private void BtnPrint_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnPrint_Click_DefaultImpl(sender,e);
		}	
		 

			

		//DDLCase_TextChanged...
		private void DDLCase_TextChanged_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			DDLCase_TextChanged_DefaultImpl(sender,e);
		}	
		 
				//OnLookCase_Click...
		private void OnLookCase_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			OnLookCase_Click_DefaultImpl(sender,e);
		}	
		 
			
				

		//DataGrid1_GridRowDbClicked...
		private void DataGrid1_GridRowDbClicked_Extend(object sender, GridDBClickEventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            //DataGrid1_GridRowDbClicked_DefaultImpl(sender,e);

            U9.VOB.HBHCommon.HBHCommonUI.HBHUIHelper.UIList_GridRowDbClicked(this);
            //U9.VOB.HBHCommon.HBHCommonUI.UICommonHelper.OnNavigatCard(this, "DayCheckIn");
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

            DayCheckInRecord head = this.Model.DayCheckIn.FocusedRecord;

            if (head != null
                && head.ID > 0
                )
            {
                //if (head.IsApproveFlow.GetValueOrDefault(false))
                //if (IsApproveFlow(head))
                //{
                //    UFIDA.U9.UI.PDHelper.PDPopWebPart.ApproveFlow_ApproveBatchUIWebPart(this);
                //}
                //else
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
        private void BtnUndoApprove_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            //BtnUndoApprove_Click_DefaultImpl(sender,e);

            this.Model.ClearErrorMessage();

            UpdateStatus((int)DocStatusData.Opened);
        }


        private void UpdateStatus(int targetStatus)
        {
            IList<IUIRecord> selected = this.Model.DayCheckIn.Cache.GetSelectRecord();

            if (selected != null
                && selected.Count > 0
                )
            {
                UpdateDayCheckInStatusBPProxy proxy = new UpdateDayCheckInStatusBPProxy();
                proxy.TargetStatus = targetStatus;

                proxy.HeadIDs = new System.Collections.Generic.List<long>();
                //proxy.HeadIDs.Add(focusedRecord.ID);
                foreach (IUIRecord record in selected)
                {
                    long id = record["MainID"].GetLong();

                    if (id <= 0)
                    {
                        id = record.PrimaryKey;
                    }

                    proxy.HeadIDs.Add(id);
                }

                proxy.Do();

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
			
			AfterOnLoad_Qry_DefaultImpl();//BE列表自动产生的代码
	    
		}

        public void AfterCreateChildControls()
        {
									
			AfterCreateChildControls_Qry_DefaultImpl();//BE列表自动产生的代码


            // 取得提示信息资源：是否删除当前记录
            string message = UFIDA.U9.UI.PDHelper.PDResource.GetDeleteConfirmInfo();
            // 绑定注册弹出对话框到删除按钮 
            UFIDA.U9.UI.PDHelper.PDFormMessage.ShowDelConfirmDialog(this.Page, message, "", this.BtnDelete);

            // 启用页面个性化 
            UFIDA.U9.UI.PDHelper.PersonalizationHelper.SetPersonalizationEnable(this, true);
				
        }
        
        public void AfterEventBind()
        {
        }
        
		public void BeforeUIModelBinding()
		{
								
		}

		public void AfterUIModelBinding()
		{
									
			AfterUIModelBinding_Qry_DefaultImpl();//BE列表自动产生的代码


            U9.VOB.HBHCommon.HBHCommonUI.HBHUIHelper.UIList_SetDocNoTitleClick(this,this.DataGrid1
                , "MainID"
                , "CheckInDate"
                , "eff032a7-ce78-48c1-bb3f-f992965caa0d"
                , "日考勤"
                // , param
                );

            BtnRecovery.Visible = false;
            BtnApprove.Enabled = false;

		}


        #endregion
		
    }
}