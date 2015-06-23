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
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.WebControls.Association;
using UFSoft.UBF.UI.WebControls.Association.Adapter;



/***********************************************************************************************
 * Form ID: 
 * UIFactory Auto Generator 
 ***********************************************************************************************/
namespace U9.VOB.Cus.HBHJianLiYuan
{
    public partial class PPLDepartmentUIFormWebPart
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
		}	
		 
				//BtnSubmit_Click...
		private void BtnSubmit_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnSubmit_Click_DefaultImpl(sender,e);
		}	
		 
				//BtnApprove_Click...
		private void BtnApprove_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnApprove_Click_DefaultImpl(sender,e);
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
			
		
			BtnList_Click_DefaultImpl(sender,e);
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

        public void BtnOk_Click_Extend(object sender, EventArgs e)
        {
            if (this.NameValues != null && this.NameValues["SrcPurpriceID"] != null)
            {
                foreach (PPLDepartmentRecord record in this.Model.PPLDepartment.Records)
                {
                    //record.PurchasePriceList = new long();
                    record.PurchasePriceList = Convert.ToInt64(this.NameValues["SrcPurpriceID"].ToString());
                    record.PurchasePriceList_Code = this.NameValues["SrcPurpriceCode"].ToString();
                    record.PurchasePriceList_Name = this.NameValues["SrcPurpriceName"].ToString();
                }
            }
            BtnOk_Click_DefaultImpl(sender, e);
        }

        public void BtnClose_Click_Extend(object sender, EventArgs e)
        {
            this.CloseDialog(false);
            BtnClose_Click_DefaultImpl(sender, e);
        }    
            
            
            

		#region 自定义数据初始化加载和数据收集
		private void OnLoadData_Extend(object sender)
		{
            //if (this.NameValues != null && this.NameValues["SrcPurpriceID"] != null)
            //{
            //    this.Model.PPLDepartment.CurrentFilter.OPath = this.Model.PPLDepartment.CurrentFilter.OPath + "PurchasePriceList.ID=" + Convert.ToInt64(this.NameValues["SrcPurpriceID"].ToString());
            //}
            if (this.NameValues != null && this.NameValues["SrcPurpriceID"] != null)
            {
                this.Model.PPLDepartment.CurrentFilter.OPath = "PurchasePriceList.ID=" + Convert.ToInt64(this.NameValues["SrcPurpriceID"].ToString());
                this.Model.PPLDepartment.Clear();
                this.Action.CommonAction.Load(this.Model.PPLDepartment);
            }
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

		}

        public void AfterCreateChildControls()
        {

            DeptCellDataChangedPostBack();
		
        }
        
        public void AfterEventBind()
        {
        }
        
		public void BeforeUIModelBinding()
		{

            SetControlCustomInParams();
		}

         /// <summary>
        /// 设置控件过滤条件、属性
        /// </summary>
        private void SetControlCustomInParams()
        {
            //部门过滤
            if (this.Model.PPLDepartment.Records.Count > 0)
            {
                string deptStr = "";
                //foreach (PPLDepartmentRecord record in this.Model.PPLDepartment.Records)
                //{
                //    if (record.Department > 0)
                //        deptStr += record.Department + ",";
                //}
                IUFFldReferenceColumn deptRef = (IUFFldReferenceColumn)this.DataGrid1.Columns["Department"];
                if (deptStr != "")
                {
                    deptStr = deptStr.Substring(0, deptStr.Length - 1);
                    if (deptStr != "")
                    {
                        deptRef.CustomInParams = BaseAction.Symbol_AddCustomFilter + "= ID not in (" + deptStr + ") and Org = " + UFIDA.U9.UI.PDHelper.PDContext.Current.OrgID + "";
                    }
                }
                else
                {
                    deptRef.CustomInParams = BaseAction.Symbol_AddCustomFilter + "= Org = " + UFIDA.U9.UI.PDHelper.PDContext.Current.OrgID + "";
                }
            }
        }

		public void AfterUIModelBinding()
		{
            //参数过滤条件
            this.SetControlCustomInParams();
		}

        #region 部门改变事件
        private void DeptCellDataChangedPostBack()
        {
            AssociationControl assocControl = new AssociationControl();
            assocControl.SourceServerControl = this.DataGrid1;
            assocControl.SourceControl.EventName = "OnCellDataChanged";
            ((IUFClientAssoGrid)assocControl.SourceControl).FireEventCols.Add("Department");
            CodeBlock cb = new CodeBlock();
            UFWebClientGridAdapter gridAdapter = new UFWebClientGridAdapter(this.DataGrid1);
            gridAdapter.IsPostBack = true;
            gridAdapter.PostBackTag = "OnCellDataChanged";//OnCellDataValueChanged
            cb.TargetControls.addControl(gridAdapter);
            assocControl.addBlock(cb);
            UFGrid deptGrid = this.DataGrid1 as UFGrid;
            deptGrid.GridCustomerPostBackEvent += new GridCustomerPostBackDelegate(deptGrid_GridCustomerPostBackEvent);

        }
        public void deptGrid_GridCustomerPostBackEvent(object sender, GridCustomerPostBackEventArgs e)
        {
            if (e.ColIndex == -1) return;
            if (e.RowIndex == -1) return;

            this.DataCollect();
            DataTable dt = this.CurrentState["ReturnPurPirceDeptList"] as DataTable;//参照中选择的数据
            if (dt != null && dt.Rows.Count > 0)
            {
                PPLDepartmentRecord recourd = null;
                recourd = this.Model.PPLDepartment.FocusedRecord;
                if (recourd == null)
                {

                    recourd = this.Model.PPLDepartment.AddNewUIRecord();
                }
                recourd.Department = long.Parse(dt.Rows[0]["ID"] + "");
                recourd.Department_Code = dt.Rows[0]["Code"] + "";
                recourd.Department_Name = dt.Rows[0]["Name"] + "";
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    recourd = this.Model.PPLDepartment.AddNewUIRecord();
                    recourd.Department = long.Parse(dt.Rows[i]["ID"] + "");
                    recourd.Department_Code = dt.Rows[i]["Code"] + "";
                    recourd.Department_Name = dt.Rows[i]["Name"] + "";
                }
            }
            this.CurrentState["ReturnPurPirceDeptList"] = null;
        }
        #endregion

        #endregion
		
        #endregion
		
    }
}