using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.WebControls.Association.Adapter;
using UFSoft.UBF.UI.WebControls.Association;
using UFSoft.UBF.UI.WebControls.ClientCallBack;
using System.Collections;
using UFIDA.U9.SCM.SD.ShipUIModel;
using UFSoft.UBF.UI.Controls;
using UFSoft.UBF.UI.WebControls;
using UFIDA.U9.Cust.HBH.Common.CommonLibary;
using System.Collections.Specialized;

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

            _strongPart = Part as ShipMainUIFormWebPart;

            Ship_ShipLinesRecord record = _strongPart.Model.Ship_ShipLines.FocusedRecord;

            DataGrid10 = (IUFDataGrid)part.GetUFControlByName(part.TopLevelContainer, "DataGrid10");
            //Register_DataGrid10_Item_CallBack();//料品改变事件，自动带出单价
            RegisterGridCellDataChangedCallBack();
        }

        public override void AfterRender(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.AfterRender(Part, args);


            {
                //string orgID = "1001411156753998";
                string targetOrgID = PubClass.GetString(this._strongPart.NameValues["TargetOrg"]);
                string curOId = PubClass.GetString(this._strongPart.NameValues["__curOId"]);
                string urlID = PubClass.GetString(this._strongPart.NameValues["lnk"]);
                if (
                    !PubClass.IsNull(targetOrgID)
                    && targetOrgID != curOId
                    )
                {
                    NameValueCollection nvs = new NameValueCollection();
                    nvs.Add("__curOId", targetOrgID);
                    nvs.Add("SHIP_Type", PubClass.GetString(this._strongPart.NameValues["SHIP_Type"]));
                    //// 有上面菜单栏，但是组织无法切换
                    //this._strongPart.NavigatePage("Cust_Rcv", nvs);
                    // 无上面菜单栏，但组织可以切换；
                    this._strongPart.NavigateForm(urlID, nvs);
                }
            }
        }

        /// <summary>
        /// 注册表格单元格内容改变的回调事件
        /// </summary>
        private void RegisterGridCellDataChangedCallBack()
        {
            AssociationControl assocControl = new AssociationControl();
            assocControl.SourceServerControl = this.DataGrid10;
            assocControl.SourceControl.EventName = "OnCellDataValueChanged";
            ((IUFClientAssoGrid)assocControl.SourceControl).FireEventCols.Add("ItemID");
            CodeBlock cb = new CodeBlock();
            UFWebClientGridAdapter gridAdapter = new UFWebClientGridAdapter(this.DataGrid10);
            gridAdapter.IsPostBack = true;
            gridAdapter.PostBackTag = "OnCellDataValueChanged";
            cb.TargetControls.addControl(gridAdapter);
            assocControl.addBlock(cb);
            UFGrid itemGrid = this.DataGrid10 as UFGrid;
            itemGrid.GridCustomerPostBackEvent += new GridCustomerPostBackDelegate(GridCellOnChanged_DoCustomerAction_Grid);

        }

        /// <summary>
        /// 表格的CallBack处理方式
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private void GridCellOnChanged_DoCustomerAction_Grid(object sender, GridCustomerPostBackEventArgs e)
        {
            //获取最后的行号
            Ship_ShipLinesRecord record = _strongPart.Model.Ship_ShipLines.FocusedRecord;
            if (record != null)
            {
                //定价
                decimal price = 0;
                //取部门+料品，定了供应商，取供应商价目表
                if (_strongPart.Model.Ship_ShipLines.FocusedRecord.SaleDept > 0 && _strongPart.Model.Ship_ShipLines.FocusedRecord.ItemID != 0)
                {
                    VOB.Cus.HBHJianLiYuan.GetPriceFromPurListBP.Proxy.GetPriceFromPurListProxy proxy = new GetPriceFromPurListBP.Proxy.GetPriceFromPurListProxy();
                    proxy.Dept = _strongPart.Model.Ship_ShipLines.FocusedRecord.SaleDept.ToString();
                    proxy.ItemMaster = (_strongPart.Model.Ship_ShipLines.FocusedRecord.ItemID ?? 0).ToString();
                    price = proxy.Do();
                    record.OrderPriceTC = price; 
                    _strongPart.Cus_PriceList23_TextChanged(sender, e);
                    //_strongPart.OrderPriceTC54_TextChanged(sender, e);
                    //record.OrderPriceTC = 5;
                }
                _strongPart.DataBind();
                _strongPart.DataCollect();
                DataGrid10.BindData();
                DataGrid10.CollectData();
            }
        }
        public override void BeforeEventProcess(UFSoft.UBF.UI.IView.IPart Part, string eventName, object sender, EventArgs args, out bool executeDefault)
        {
            base.BeforeEventProcess(Part, eventName, sender, args, out executeDefault);
             UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter webButton = sender as UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter;
            #region//提交按钮
             if (webButton != null && webButton.Action == "SubmitClick")
             {
                 //拣货
                 if (_strongPart.Model.Ship.FocusedRecord != null && _strongPart.Model.Ship.FocusedRecord.DocNo != "")
                 {
                     VOB.Cus.HBHJianLiYuan.ShipPickByDocBP.Proxy.ShipPickByDocProxy proxy = new ShipPickByDocBP.Proxy.ShipPickByDocProxy();
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
            //UFWebClientGridAdapter _clientGrid = new UFWebClientGridAdapter(DataGrid10);
            //3）注册：事件源、事件名称、事件关联的列
            AssociationControl AssCtrl = new AssociationControl();
            AssCtrl.SourceServerControl = DataGrid10;
            AssCtrl.SourceControl.EventName = "OnCellDataChanged";
            ((UFWebClientGridAdapter)AssCtrl.SourceControl).FireEventCols.Add("ItemID");

            //4）创建：CallBack窗体、事件方法、CallBack对象、事件相关
            ClientCallBackFrm frm = new ClientCallBackFrm();
            //添加参数控件	
            frm.ParameterControls.Add(DataGrid10);
            frm.DoCustomerAction += new ClientCallBackFrm.ActionCustomer(DataGrid10_Price_OnCellDataChanged);
            frm.Add(AssCtrl);
        }
        object DataGrid10_Price_OnCellDataChanged(CustomerActionEventArgs args)
        {
            if (DataGrid10 == null)
                DataGrid10 = (IUFDataGrid)part.GetUFControlByName(part.TopLevelContainer, "DataGrid10");
            this.part.DataCollect();
            this.part.DataBinding();
            DataGrid10.BindData();
            DataGrid10.CollectData();
            ArrayList list = (ArrayList)args.ArgsHash[UFWebClientGridAdapter.ALL_GRIDDATA_SelectedRows];
            ArrayList lstAllData = (ArrayList)args.ArgsHash[DataGrid10.ClientID];
            int colIndex = Convert.ToInt32(args.ArgsHash["ALL_GRIDDATA_FocusColumn"]); //取列号
            int rowIndex = Convert.ToInt32(args.ArgsHash["ALL_GRIDDATA_FocusRow"]);    //取行号
            Hashtable hs = lstAllData[rowIndex] as Hashtable;

            UFWebClientGridAdapter grid = new UFWebClientGridAdapter(DataGrid10);
            long itemID = long.Parse(hs["ItemID"].ToString());
            if (itemID > 0)
            {
                if (String.IsNullOrEmpty(hs["OrderPriceTC"].ToString()) || decimal.Parse(hs["OrderPriceTC"].ToString()) == 0)
                {
                    //定价
                    decimal price = 0;
                    //取部门+料品，定了供应商，取供应商价目表
                    if (_strongPart.Model.Ship_ShipLines.FocusedRecord != null)
                    {
                        if (_strongPart.Model.Ship_ShipLines.FocusedRecord.SaleDept > 0 && _strongPart.Model.Ship_ShipLines.FocusedRecord.ItemID != 0)
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
                        }
                        args.ArgsResult.Add(grid.ClientInstanceWithValue);
                    }

                }
            }
            //this.part.DataCollect();
            //this.part.DataBinding();
            //DataGrid10.BindData();
            //DataGrid10.CollectData();
            return args;
        }
        #endregion

    }
}
