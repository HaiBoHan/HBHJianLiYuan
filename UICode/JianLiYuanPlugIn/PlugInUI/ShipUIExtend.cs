using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.WebControls.Association.Adapter;
using UFSoft.UBF.UI.WebControls.Association;
using UFSoft.UBF.UI.WebControls.ClientCallBack;
using System.Collections;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInUI
{
   public class ShipUIExtend : UFSoft.UBF.UI.Custom.ExtendedPartBase
    {
        public UFSoft.UBF.UI.IView.IPart part;
        private UFIDA.U9.SCM.SD.ShipUIModel.ShipMainUIFormWebPart _strongPart;

        IUFDataGrid DataGrid10;
        public override void AfterInit(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.AfterInit(Part, args);

            if (Part == null || Part.Model == null)
                return;
            part = Part;

            _strongPart = Part as UFIDA.U9.SCM.SD.ShipUIModel.ShipMainUIFormWebPart;
            DataGrid10 = (IUFDataGrid)part.GetUFControlByName(part.TopLevelContainer, "DataGrid10");
            Register_DataGrid10_Item_CallBack();//料品改变事件，自动带出单价
        }

        public override void BeforeEventProcess(UFSoft.UBF.UI.IView.IPart Part, string eventName, object sender, EventArgs args, out bool executeDefault)
        {
            base.BeforeEventProcess(Part, eventName, sender, args, out executeDefault);
             UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter webButton = sender as UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter;
            #region//复制按钮
             if (webButton != null && webButton.Action == "SubmitClick")
             {
                 //拣货
                 if (_strongPart.Model.Ship.FocusedRecord != null && _strongPart.Model.Ship.FocusedRecord.DocNo != "")
                 {
                     UFIDA.U9.ISV.SM.ShipPickByDoc proxy = new UFIDA.U9.ISV.SM.ShipPickByDoc();
                     List<String> docList = new List<string>();
                     docList.Add(_strongPart.Model.Ship.FocusedRecord.DocNo);
                     proxy.ShipNos = docList;
                     proxy.Do();
                 }
             }
            #endregion
        }

        #region CallBack定价同步外销价
        private void Register_DataGrid10_Item_CallBack()
        {
            if (DataGrid10 == null)
            {
                return;
            }
            //2）创建表格适配器对象
            UFWebClientGridAdapter _clientGrid = new UFWebClientGridAdapter(DataGrid10);
            //3）注册：事件源、事件名称、事件关联的列
            AssociationControl AssCtrl = new AssociationControl();
            AssCtrl.SourceServerControl = DataGrid10;
            AssCtrl.SourceControl.EventName = "OnCellDataChanged";
            ((UFWebClientGridAdapter)AssCtrl.SourceControl).FireEventCols.Add("OrderPriceTC");

            //4）创建：CallBack窗体、事件方法、CallBack对象、事件相关
            ClientCallBackFrm frm = new ClientCallBackFrm();
            frm.DoCustomerAction += new ClientCallBackFrm.ActionCustomer(DataGrid10_Price_OnCellDataChanged);
            //添加参数控件	
            frm.ParameterControls.Add(DataGrid10);
            frm.Add(AssCtrl);
        }
        object DataGrid10_Price_OnCellDataChanged(CustomerActionEventArgs args)
        {
            if (DataGrid10 == null)
                DataGrid10 = (IUFDataGrid)part.GetUFControlByName(part.TopLevelContainer, "DataGrid10");
            this.part.DataCollect();
            this.part.DataBinding();
            DataGrid10.BindData();
            ArrayList list = (ArrayList)args.ArgsHash[UFWebClientGridAdapter.ALL_GRIDDATA_SelectedRows];
            ArrayList lstAllData = (ArrayList)args.ArgsHash[DataGrid10.ClientID];
            int colIndex = Convert.ToInt32(args.ArgsHash["ALL_GRIDDATA_FocusColumn"]); //取列号
            int rowIndex = Convert.ToInt32(args.ArgsHash["ALL_GRIDDATA_FocusRow"]);    //取行号
            Hashtable hs = lstAllData[rowIndex] as Hashtable;

            UFWebClientGridAdapter grid = new UFWebClientGridAdapter(DataGrid10);

            if (String.IsNullOrEmpty(hs["OrderPriceTC"].ToString()) || decimal.Parse(hs["OrderPriceTC"].ToString()) == 0)
            {
                //定价
                decimal price = 0;
                //取部门+料品，定了供应商，取供应商价目表
                if (_strongPart.Model.Ship_ShipLines.FocusedRecord != null)
                {
                    VOB.Cus.HBHJianLiYuan.GetPriceFromPurListBP.Proxy.GetPriceFromPurListProxy proxy = new GetPriceFromPurListBP.Proxy.GetPriceFromPurListProxy();
                    proxy.Dept = _strongPart.Model.Ship_ShipLines.FocusedRecord.SaleDept.ToString();
                    proxy.ItemMaster = (_strongPart.Model.Ship_ShipLines.FocusedRecord.ItemID ?? 0).ToString();
                    price = proxy.Do();
                    if (price > 0)
                    {

                        //行记录
                        grid.CellValue.Add(new Object[] { rowIndex, "OrderPriceTC", new string[] { price.ToString(), price.ToString(), price.ToString() } });
                    }

                    args.ArgsResult.Add(grid.ClientInstanceWithValue);
                }
               
            }
            return args;
        }
        #endregion

    }
}
