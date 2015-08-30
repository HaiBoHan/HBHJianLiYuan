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
using UFSoft.UBF.UI.WebControls.Association;
using UFSoft.UBF.UI.WebControls.Association.Adapter;
using UFIDA.U9.UI.PDHelper;



/***********************************************************************************************
 * Form ID: 
 * UIFactory Auto Generator 
 ***********************************************************************************************/
namespace U9.VOB.Cus.HBHJianLiYuan
{
    public partial class DeptItemSupplierUIFormWebPart
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

            //BtnList_Click_DefaultImpl(sender,e);

            this.NavigatePage("Cust_DeptItemSupplierUIList", null);
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

            BtnOk_Click_DefaultImpl(sender, e);
        }

        public void BtnClose_Click_Extend(object sender, EventArgs e)
        {
            this.CloseDialog(false);
            BtnClose_Click_DefaultImpl(sender, e);
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

		}

        public void AfterCreateChildControls()
        {
            //查找
            //取得当前卡片参照的属性变量：FormID、Width、Height、Title；
            //传递隐藏域wpFindID的客户端ID；注意：隐藏域wpFindID会记录参照选择的记录ID；
            PDFormMessage.ShowConfirmDialog(this.Page, "51b1ffc6-10c8-4c1a-b2fb-0c5d606c3138 ", Title, wpFindID.ClientID, this.BtnFind, null);

            // 实现个性化
            UFIDA.U9.UI.PDHelper.PersonalizationHelper.SetPersonalizationEnable(this, true);

            // 实现删除提示是否需要删除
            UFIDA.U9.UI.PDHelper.PDFormMessage.ShowDelConfirmDialog(this.Page, UFIDA.U9.UI.PDHelper.PDResource.GetDeleteConfirmInfo(), "是否删除该记录信息", this.BtnDelete);

            //实现弹性域控件绑定
            FlexFieldHelper.SetDescFlexField(this.FlexFieldPicker0, this.Model.DeptItemSupplier);

            //行扩展字段
            FlexFieldHelper.SetDescFlexField(this.DataGrid5, this.DataGrid5.Columns.Count - 1);

            ItemCellDataChangedPostBack();//料品
        }
        
        public void AfterEventBind()
        {
        }
        
		public void BeforeUIModelBinding()
		{
           
		}

       
		public void AfterUIModelBinding()
		{


		}
        #region 料品改变事件
        private void ItemCellDataChangedPostBack()
        {
            AssociationControl assocControl = new AssociationControl();
            assocControl.SourceServerControl = this.DataGrid5;
            assocControl.SourceControl.EventName = "OnCellDataChanged";
            ((IUFClientAssoGrid)assocControl.SourceControl).FireEventCols.Add("ItemMaster");
            CodeBlock cb = new CodeBlock();
            UFWebClientGridAdapter gridAdapter = new UFWebClientGridAdapter(this.DataGrid5);
            gridAdapter.IsPostBack = true;
            gridAdapter.PostBackTag = "OnCellDataChanged";//OnCellDataValueChanged
            cb.TargetControls.addControl(gridAdapter);
            assocControl.addBlock(cb);
            UFGrid itemGrid = this.DataGrid5 as UFGrid;
            itemGrid.GridCustomerPostBackEvent += new GridCustomerPostBackDelegate(itemGrid_GridCustomerPostBackEvent);

        }
        public void itemGrid_GridCustomerPostBackEvent(object sender, GridCustomerPostBackEventArgs e)
        {
            if (e.ColIndex == -1) return;
            if (e.RowIndex == -1) return;

            this.DataCollect();
            DataTable dt = this.CurrentState["ReturnDeptItemList"] as DataTable;//参照中选择的数据
            if (dt != null && dt.Rows.Count > 0)
            {
                DeptItemSupplier_DeptItemSupplierLineRecord recourd = null;
                recourd = this.Model.DeptItemSupplier_DeptItemSupplierLine.FocusedRecord;
                if (recourd == null)
                {

                    recourd = this.Model.DeptItemSupplier_DeptItemSupplierLine.AddNewUIRecord();
                }
                recourd.ItemMaster = long.Parse(dt.Rows[0]["ID"] + "");
                recourd.ItemMaster_Code = dt.Rows[0]["Code"] + "";
                recourd.ItemMaster_Name = dt.Rows[0]["Name"] + "";
                recourd.Supplier = long.Parse(dt.Rows[0]["Supplier_ID"] + "");
                recourd.Supplier_Code = dt.Rows[0]["Supplier_Code"] + "";
                recourd.Supplier_Name = dt.Rows[0]["Supplier_Name"] + "";
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    recourd = this.Model.DeptItemSupplier_DeptItemSupplierLine.AddNewUIRecord();
                    recourd.ItemMaster = long.Parse(dt.Rows[i]["ID"] + "");
                    recourd.ItemMaster_Code = dt.Rows[i]["Code"] + "";
                    recourd.ItemMaster_Name = dt.Rows[i]["Name"] + "";
                    recourd.Supplier = long.Parse(dt.Rows[i]["Supplier_ID"] + "");
                    recourd.Supplier_Code = dt.Rows[i]["Supplier_Code"] + "";
                    recourd.Supplier_Name = dt.Rows[i]["Supplier_Name"] + "";
                    recourd.SetParentRecord(this.Model.DeptItemSupplier.FocusedRecord);
                }
            }
            this.CurrentState["ReturnDeptItemList"] = null;
        }
        #endregion

        #endregion
		
    }
}