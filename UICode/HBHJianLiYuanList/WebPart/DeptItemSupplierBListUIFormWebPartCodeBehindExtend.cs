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
using System.Collections.Specialized;
using HBH.DoNet.DevPlatform.EntityMapping;



/***********************************************************************************************
 * Form ID: 
 * UIFactory Auto Generator 
 ***********************************************************************************************/
namespace DeptItemSupplierBListUIModel
{
    public partial class DeptItemSupplierBListUIFormWebPart
    {
        #region Custome eventBind
	
		 
				//BtnNew_Click...
		private void BtnNew_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.


            //BtnNew_Click_DefaultImpl(sender, e);

            this.NavigatePage("U9.VOB.Cus.HBHJianLiYuan.DeptItemSupplierURI", null);
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

            DeptItemSupplierRecord head = this.Model.DeptItemSupplier.FocusedRecord;
            if (head != null
                )
            {
                long id = PubClass.GetLong(head["MainID"]);

                if (id > 0)
                {
                    //this.NavigatePage("d85d3974-ac43-4311-853d-bb47f4196b03", null);
                    this.OnNavigatCard("Browse", id.ToString(), "d85d3974-ac43-4311-853d-bb47f4196b03");
                }
            }
		}

        private void OnNavigatCard(string cardType, string recordID, string pageID)
        {
            if (!string.IsNullOrEmpty(recordID) || (string.Compare(cardType, "Browse", true) != 0))
            {
                NameValueCollection querystrings = new NameValueCollection();
                querystrings.Add("PDPageStatus", cardType);
                querystrings.Add("ID", recordID);
                NavigatePage(pageID, querystrings);
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

            
            U9.VOB.HBHCommon.HBHCommonUI.HBHUIHelper.UIList_SetDocNoTitleClick(this, this.DataGrid1
                , "ID"
                , "Department_Name"
                , "a6b16b6b-290b-4a08-8e7e-0705df746128"
                , "部门料品供应商交叉档"
                // , param
                );
		}


        #endregion
    }
}