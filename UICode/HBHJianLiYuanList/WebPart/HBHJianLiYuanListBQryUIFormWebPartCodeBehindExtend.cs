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



/***********************************************************************************************
 * Form ID: 
 * UIFactory Auto Generator 
 ***********************************************************************************************/
namespace U9.VOB.Cus.HBHJianLiYuan.HBHJianLiYuanListBQryUIModel
{
    public partial class HBHJianLiYuanListBQryUIFormWebPart
    {
        #region Custome eventBind
	
		 
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
			
		
			DataGrid1_GridRowDbClicked_DefaultImpl(sender,e);
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
						
			AfterCreateChildControls_Qry_DefaultImpl();//BE查询自动产生的代码
					

		
        }
        
        public void AfterEventBind()
        {
        }
        
		public void BeforeUIModelBinding()
		{
						
			BeforeUIModelBinding_Qry_DefaultImpl();//BE查询自动产生的代码


            // SCM.PM.PM3020_10&PR_Type=PM3020
            LinkParameter paramType = new LinkParameter("PR_Type", "PM3020", enuBindingType.value);
            LinkParameter paramOrg = new LinkParameter("__curOId", "Org", enuBindingType.column, enuBindingPropertyType.Key);
            
            U9.VOB.HBHCommon.HBHCommonUI.UICommonHelper.SetDocNoTitleClick(this.DataGrid1
                , "ID"
                , "DocNo", "4E0A5FF2-3FCD-4403-8D36-068F37667C4F", "请购单"
                , paramType, paramOrg
                );
		}

		public void AfterUIModelBinding()
		{
						
			AfterUIModelBinding_Qry_DefaultImpl();//BE查询自动产生的代码

            U9.VOB.HBHCommon.HBHCommonUI.HBHUIHelper.UIList_SetDocNoTitleClick(this, this.DataGrid1
                , "MainID"
                , "DocNo"
                , "4E0A5FF2-3FCD-4403-8D36-068F37667C4F"
                , "请购单"
                // , param
                );
		}


        #endregion
		
    }
}