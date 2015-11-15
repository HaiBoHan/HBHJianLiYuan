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
using UFSoft.UBF.UI.MD.Runtime;
using UFIDA.U9.CBO.SCM.Enums;
using U9.VOB.Cus.HBHJianLiYuan.GetPriceFromPurListBP;
using U9.VOB.Cus.HBHJianLiYuan.GetPriceFromPurListBP.Proxy;
using UFSoft.UBF.UI.JMF.ActionProcess;

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
            //RegisterGridCellDataChangedCallBack();

            // 料品改变Post，自动带出单价(部门、料品-->供应商-->价表行-->价格)
            Regist_OnChangePostBack_DataGrid10_ItemID();

            // 部门参照PostBack
            // Card3    TabControl0     TabPage1    SaleDept259
            IUFCard card3 = (IUFCard)part.GetUFControlByName(part.TopLevelContainer, "Card3");
            if (card3 != null)
            {
                IUFTabControl tc0 = (IUFTabControl)part.GetUFControlByName(card3, "TabControl0");
                if (tc0 != null
                    && tc0.TabPages != null
                    && tc0.TabPages.Count > 0
                    )
                {
                    IUFTabPage tp1 = null;

                    foreach (IUFTabPage page in tc0.TabPages)
                    {
                        if (page != null
                            && page.ID == "TabPage1"
                            )
                        {
                            tp1 = page;
                            break;
                        }
                    }

                    if (tp1 != null)
                    {
                        IUFFldReference refDept = (IUFFldReference)part.GetUFControlByName(tp1, "SaleDept259");

                        if (refDept != null)
                        {
                            refDept.ContentChanged += new EventHandler(refDept_ContentChanged);
                            refDept.AutoPostBack = true;
                        }
                    }
                }
            }

            //  不用了，不知道跟产品的什么东西冲突，导致，有时候物料放大镜 点击无反应；改为在弹出参照页面做过滤条件吧；
            //RegisterGridItemIDFilterAssociation();
        }

        void refDept_ContentChanged(object sender, EventArgs e)
        {
            //清除错误信息
            _strongPart.Model.ClearErrorMessage();

            _strongPart.DataCollect();
            _strongPart.IsDataBinding = true; //当前事件执行后会进行数据绑定
            _strongPart.IsConsuming = false;
        }

        public override void AfterRender(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.AfterRender(Part, args);

            AddItemRefCondition();

            //{
            //    //string orgID = "1001411156753998";
            //    string targetOrgID = PubClass.GetString(this._strongPart.NameValues["TargetOrg"]);
            //    string curOId = PubClass.GetString(this._strongPart.NameValues["__curOId"]);
            //    string urlID = PubClass.GetString(this._strongPart.NameValues["lnk"]);
            //    if (
            //        !PubClass.IsNull(targetOrgID)
            //        && targetOrgID != curOId
            //        )
            //    {
            //        NameValueCollection nvs = new NameValueCollection();
            //        nvs.Add("__curOId", targetOrgID);
            //        nvs.Add("SHIP_Type", PubClass.GetString(this._strongPart.NameValues["SHIP_Type"]));
            //        //// 有上面菜单栏，但是组织无法切换
            //        //this._strongPart.NavigatePage("Cust_Rcv", nvs);
            //        // 无上面菜单栏，但组织可以切换；
            //        this._strongPart.NavigateForm(urlID, nvs);
            //    }
            //}
        }

        public override void BeforeEventProcess(UFSoft.UBF.UI.IView.IPart Part, string eventName, object sender, EventArgs args, out bool executeDefault)
        {
            base.BeforeEventProcess(Part, eventName, sender, args, out executeDefault);
             UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter webButton = sender as UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter;
            #region//提交按钮
             if (webButton != null && webButton.Action == "SubmitClick")
             {
                 ////拣货
                 //if (_strongPart.Model.Ship.FocusedRecord != null && _strongPart.Model.Ship.FocusedRecord.DocNo != "")
                 //{
                 //    VOB.Cus.HBHJianLiYuan.ShipPickByDocBP.Proxy.ShipPickByDocProxy proxy = new ShipPickByDocBP.Proxy.ShipPickByDocProxy();
                 //    List<String> docList = new List<string>();
                 //    docList.Add(_strongPart.Model.Ship.FocusedRecord.DocNo);
                 //    proxy.ShipNos = docList;
                 //    proxy.Do();
                 //}
             }
            #endregion
        }

        #region CallBack定价同步外销价

        #region Old，事件不是GridCustomerPostBackDelegate，不太好用，已注释

        ///// <summary>
        ///// 注册表格单元格内容改变的回调事件
        ///// </summary>
        //private void RegisterGridCellDataChangedCallBack()
        //{
        //    AssociationControl assocControl = new AssociationControl();
        //    assocControl.SourceServerControl = this.DataGrid10;
        //    assocControl.SourceControl.EventName = "OnCellDataValueChanged";
        //    ((IUFClientAssoGrid)assocControl.SourceControl).FireEventCols.Add("ItemID");
        //    CodeBlock cb = new CodeBlock();
        //    UFWebClientGridAdapter gridAdapter = new UFWebClientGridAdapter(this.DataGrid10);
        //    gridAdapter.IsPostBack = true;
        //    gridAdapter.PostBackTag = "OnCellDataValueChanged";
        //    cb.TargetControls.addControl(gridAdapter);
        //    assocControl.addBlock(cb);
        //    UFGrid itemGrid = this.DataGrid10 as UFGrid;
        //    itemGrid.GridCustomerPostBackEvent += new GridCustomerPostBackDelegate(GridCellOnChanged_DoCustomerAction_Grid);

        //}

        ///// <summary>
        ///// 表格的CallBack处理方式
        ///// </summary>
        ///// <param name="args"></param>
        ///// <returns></returns>
        //private void GridCellOnChanged_DoCustomerAction_Grid(object sender, GridCustomerPostBackEventArgs e)
        //{
        //    //获取最后的行号
        //    Ship_ShipLinesRecord record = _strongPart.Model.Ship_ShipLines.FocusedRecord;
        //    if (record != null)
        //    {
        //        //定价
        //        decimal price = 0;
        //        //取部门+料品，定了供应商，取供应商价目表
        //        if (_strongPart.Model.Ship_ShipLines.FocusedRecord.SaleDept > 0 && _strongPart.Model.Ship_ShipLines.FocusedRecord.ItemID != 0)
        //        {
        //            VOB.Cus.HBHJianLiYuan.GetPriceFromPurListBP.Proxy.GetPriceFromPurListProxy proxy = new GetPriceFromPurListBP.Proxy.GetPriceFromPurListProxy();
        //            proxy.Dept = _strongPart.Model.Ship_ShipLines.FocusedRecord.SaleDept.ToString();
        //            proxy.ItemMaster = (_strongPart.Model.Ship_ShipLines.FocusedRecord.ItemID ?? 0).ToString();
        //            price = proxy.Do();
        //            record.OrderPriceTC = price;
        //            _strongPart.Cus_PriceList23_TextChanged(sender, e);
        //            //_strongPart.OrderPriceTC54_TextChanged(sender, e);
        //            //record.OrderPriceTC = 5;
        //        }
        //        _strongPart.DataBind();
        //        _strongPart.DataCollect();
        //        DataGrid10.BindData();
        //        DataGrid10.CollectData();
        //    }
        //}


        //private void Register_DataGrid10_Item_CallBack()
        //{
        //    if (DataGrid10 == null)
        //    {
        //        return;
        //    }
        //    //2）创建表格适配器对象
        //    //UFWebClientGridAdapter _clientGrid = new UFWebClientGridAdapter(DataGrid10);
        //    //3）注册：事件源、事件名称、事件关联的列
        //    AssociationControl AssCtrl = new AssociationControl();
        //    AssCtrl.SourceServerControl = DataGrid10;
        //    AssCtrl.SourceControl.EventName = "OnCellDataChanged";
        //    ((UFWebClientGridAdapter)AssCtrl.SourceControl).FireEventCols.Add("ItemID");

        //    //4）创建：CallBack窗体、事件方法、CallBack对象、事件相关
        //    ClientCallBackFrm frm = new ClientCallBackFrm();
        //    //添加参数控件	
        //    frm.ParameterControls.Add(DataGrid10);
        //    frm.DoCustomerAction += new ClientCallBackFrm.ActionCustomer(DataGrid10_Price_OnCellDataChanged);
        //    frm.Add(AssCtrl);
        //}
        //object DataGrid10_Price_OnCellDataChanged(CustomerActionEventArgs args)
        //{
        //    if (DataGrid10 == null)
        //        DataGrid10 = (IUFDataGrid)part.GetUFControlByName(part.TopLevelContainer, "DataGrid10");
        //    this.part.DataCollect();
        //    this.part.DataBinding();
        //    DataGrid10.BindData();
        //    DataGrid10.CollectData();
        //    ArrayList list = (ArrayList)args.ArgsHash[UFWebClientGridAdapter.ALL_GRIDDATA_SelectedRows];
        //    ArrayList lstAllData = (ArrayList)args.ArgsHash[DataGrid10.ClientID];
        //    int colIndex = Convert.ToInt32(args.ArgsHash["ALL_GRIDDATA_FocusColumn"]); //取列号
        //    int rowIndex = Convert.ToInt32(args.ArgsHash["ALL_GRIDDATA_FocusRow"]);    //取行号
        //    Hashtable hs = lstAllData[rowIndex] as Hashtable;

        //    UFWebClientGridAdapter grid = new UFWebClientGridAdapter(DataGrid10);
        //    long itemID = long.Parse(hs["ItemID"].ToString());
        //    if (itemID > 0)
        //    {
        //        if (String.IsNullOrEmpty(hs["OrderPriceTC"].ToString()) || decimal.Parse(hs["OrderPriceTC"].ToString()) == 0)
        //        {
        //            //定价
        //            decimal price = 0;
        //            //取部门+料品，定了供应商，取供应商价目表
        //            if (_strongPart.Model.Ship_ShipLines.FocusedRecord != null)
        //            {
        //                if (_strongPart.Model.Ship_ShipLines.FocusedRecord.SaleDept > 0 && _strongPart.Model.Ship_ShipLines.FocusedRecord.ItemID != 0)
        //                {
        //                    VOB.Cus.HBHJianLiYuan.GetPriceFromPurListBP.Proxy.GetPriceFromPurListProxy proxy = new GetPriceFromPurListBP.Proxy.GetPriceFromPurListProxy();
        //                    proxy.Dept = _strongPart.Model.Ship_ShipLines.FocusedRecord.SaleDept.ToString();
        //                    proxy.ItemMaster = (_strongPart.Model.Ship_ShipLines.FocusedRecord.ItemID ?? 0).ToString();
        //                    price = proxy.Do();
        //                    if (price > 0)
        //                    {
        //                        //行记录
        //                        grid.CellValue.Add(new Object[] { rowIndex, "OrderPriceTC", new string[] { price.ToString(), price.ToString(), price.ToString() } });
        //                    }
        //                }
        //                args.ArgsResult.Add(grid.ClientInstanceWithValue);
        //            }

        //        }
        //    }

        //    //this.part.DataCollect();
        //    //this.part.DataBinding();
        //    //DataGrid10.BindData();
        //    //DataGrid10.CollectData();
        //    return args;
        //}

        #endregion



        private void Regist_OnChangePostBack_DataGrid10_ItemID()
        {
            AssociationControl control = new AssociationControl();
            control.SourceServerControl = DataGrid10;
            control.SourceControl.EventName = "OnCellDataChanged";
            ((IUFClientAssoGrid)control.SourceControl).FireEventCols.Add(_strongPart.Model.Ship_ShipLines.FieldItemID.Name);
            //((IUFClientAssoGrid)control.SourceControl).FireEventCols.Add(FieldName_FinallyPriceTC);
            CodeBlock block = new CodeBlock();
            UFWebClientGridAdapter adapter = new UFWebClientGridAdapter(DataGrid10);
            adapter.IsPostBack = true;
            adapter.PostBackTag = DataGrid10.ID + "_" + control.SourceControl.EventName;
            block.TargetControls.addControl(adapter);
            control.addBlock(block);
            UFGrid dataGrid = DataGrid10 as UFGrid;
            dataGrid.GridCustomerPostBackEvent += new GridCustomerPostBackDelegate(OnChangePostBack_DataGrid10_ItemID);
        }

        private void OnChangePostBack_DataGrid10_ItemID(object sender, GridCustomerPostBackEventArgs e)
        {
            if (e.PostTag.ToString().EndsWith("OnCellDataChanged")
                )
            {
                //int qtyIndex = GetColIndex(datagrid, FieldName_ItemID);
                //&& e.ColIndex == qtyIndex

                //string uIFieldID = this.DataGrid10.Columns[e.ColIndex].UIFieldID;
                //if (uIFieldID == view.FieldCust_CustomerItemID.Name)

                string uIFieldID = DataGrid10.Columns[e.ColIndex].UIFieldID;
                if (uIFieldID == _strongPart.Model.Ship_ShipLines.FieldItemID.Name)
                {
                    //清除错误信息
                    _strongPart.Model.ClearErrorMessage();

                    _strongPart.DataCollect();
                    _strongPart.IsDataBinding = true; //当前事件执行后会进行数据绑定
                    _strongPart.IsConsuming = false;

                    // 物料可以多选,还可以在物料参照里录入数量
                    // 只计算最终价为0的行
                    GetAllFinallyPrice(sender, e, false);
                }
                // 定价不可改，可能为扩展字段
                else if (uIFieldID == _strongPart.Model.Ship_ShipLines.FieldOrderPriceTC.Name)
                {
                    //    Ship_ShipLinesRecord line = curPart.Model.Ship_ShipLines.FocusedRecord;

                    //    // 设置售价状态 (正常售价,高于售价,低于售价)
                    //    SetPriceStatus(line);

                    // 手工录入最终价时，如果物料没有价格，物料变更时候没带出来，那么
                }
            }
        }

        private void GetAllFinallyPrice(object sender, GridCustomerPostBackEventArgs e, bool isAllLines)
        {
            ShipRecord shipHead = _strongPart.Model.Ship.FocusedRecord;

            if(shipHead == null)
                return;

            IUIRecordCollection lstSOLines = _strongPart.Model.Ship_ShipLines.Records;
            if (lstSOLines != null
                && lstSOLines.Count > 0
                )
            {
                List<long> listItem = new List<long>();

                IUIRecord changedSource = null;
                if (e != null)
                {
                    changedSource = GetRecordByDataGridIndex(lstSOLines, e.RowIndex);
                }

                List<ItemPriceData> lstItemDTO = new List<ItemPriceData>(); 
                foreach (Ship_ShipLinesRecord line in _strongPart.Model.Ship_ShipLines.Records)
                {
                    if (line != null
                        && line.ItemInfo_ItemID.GetValueOrDefault(-1) > 0
                        )
                    {
                        //// 赋值价格来源 = 手工,因为不赋值价格来源，无法价格联动
                        //if (line.PriceSource == (int)PriceSourceEnumData.Empty)
                        //{
                        //    line.PriceSource = (int)PriceSourceEnumData.Custom;
                        //}

                        if (
                            // 是否此行物料变更
                            IsChangedSOline(changedSource, line)
                            // 或其他行需要取价格(物料多选返回)
                            || IsGetPrice(isAllLines, line)
                            )
                        {
                            //listItem.Add(line.ItemInfo_ItemID.GetValueOrDefault(-1));

                            ItemPriceData dto = new ItemPriceData();
                            dto.DepartmentName = shipHead.SaleDept_Name;
                            dto.ItemCode = line.ItemCode;
                            dto.DocDate = shipHead.BusinessDate;

                            lstItemDTO.Add(dto);
                        }
                    }
                }


                if (lstItemDTO.Count > 0)
                {
                    GetPriceFromPurListProxy proxy = new GetPriceFromPurListProxy();
                    proxy.ItemPrices = lstItemDTO;

                    List<ItemPriceData> lstPrices = proxy.Do();

                    if (lstPrices != null
                        && lstPrices.Count > 0
                        )
                    {
                        foreach (ItemPriceData price in lstPrices)
                        {
                            if (price != null)
                            {
                                foreach (Ship_ShipLinesRecord line in _strongPart.Model.Ship_ShipLines.Records)
                                {
                                    //bool isResetPrice = false;

                                    if (line != null
                                        && line.ItemInfo_ItemID.GetValueOrDefault(-1) > 0
                                        && !PubClass.IsNull(price.ItemCode)
                                        && price.FinallyPrice > 0
                                        && IsGetPrice(isAllLines, line)
                                        && line.ItemCode == price.ItemCode
                                        )
                                    {
                                        //isResetPrice = true;

                                        decimal oldPrice = line.FinallyPriceTC.GetValueOrDefault(0);
                                        // 折前价
                                        line[HBHHelper.DescFlexFieldHelper.DescFlexField_PreDiscountPriceUIField] = price.PreDiscountPrice.ToString("G0");
                                        line[HBHHelper.DescFlexFieldHelper.DescFlexField_DiscountRateUIField] = price.DiscountRate.ToString("G0");
                                        line[HBHHelper.DescFlexFieldHelper.DescFlexField_DiscountLimitUIField] = price.DiscountLimit.ToString("G0");
                                        // 最终价
                                        line.FinallyPriceTC = price.FinallyPrice;
                                        line.FinallyPrice = price.FinallyPrice;

                                        /*
                                        UFIDA.U9.SCM.SD.ShipUIModel.ShipMainUIFormWebPart
		private object cF1_DoCustomerAction_ShipLine_Price(UFSoft.UBF.UI.WebControls.ClientCallBack.CustomerActionEventArgs args)
		{}
                                         */
                                        // 调用价格变更
                                        _strongPart.FinallyPriceTC101_TextChanged(oldPrice, null);

                                        //// 最终价不等
                                        //// if (itemInfo.SalePrice != line.FinallyPriceTC)
                                        //{
                                        //    decimal oldPrice = line.OrderPriceTC;

                                        //    bool isChangedLine = IsChangedSOline(changedSource, line);
                                        //    // 如果最终价不等,不是用户修改(定价小于0,定价与最新价不等)
                                        //    if (
                                        //        // 是否此行物料变更
                                        //        isChangedLine
                                        //        // 如果非变更行，没有最终价
                                        //        || line.OrderPriceTC <= 0
                                        //        // 如果非变更行，如果定价不等于最新价，那认为取错了，重取最新价
                                        //        || (oldPrice != itemInfo.SalePrice
                                        //        // 新增行
                                        //            && line.DataRecordState == System.Data.DataRowState.Added
                                        //            )
                                        //        )
                                        //    {
                                        //        // 最终价
                                        //        line.FinallyPriceTC = itemInfo.SalePrice;

                                        //        // 定价,复制时定价不变; 所以这里更改定价
                                        //        if (line.OrderPriceTC != itemInfo.SalePrice)
                                        //            line.OrderPriceTC = itemInfo.SalePrice;

                                        //        // 有数量,则 联动金额     代码来源:
                                        //        /*
                                        //         * UFIDA.U9.SCM.SD.SOUI.WebPart             UFIDA.U9.SCM.SM.SOUIModel.StandardSOMainUIFormWebPart        搜索  FinallyPriceTC
                                        //         * grid_SOLineGridPostBack(
                                        //         * FinallyPriceTC140_TextChanged_Extend(
                                        //         */
                                        //        //if (line.OrderByQtyTU > 0)
                                        //        {
                                        //            Hashtable hashtable = new Hashtable(2);
                                        //            hashtable["RECORDID"] = line.ID;
                                        //            hashtable["OLDVALUE"] = oldPrice;
                                        //            UIActionEventArgs args2 = new UIActionEventArgs
                                        //            {
                                        //                Tag = hashtable
                                        //            };
                                        //            // 标准产品,如果不录入客户;那么没有联动订单  定价（OrderPriceTC）
                                        //            curPart.Action.OnFinallyPriceTCChange(sender, args2);

                                        //        }
                                        //    }
                                        //}
                                    }

                                    // 因为还要根据运费计算 售价状态，所以在BE里做，不在UI里写了；
                                    //// 复制时,会重按定价重算;所以原来按最终价设置的 价格状态 ,要重置;
                                    //if (isResetPrice
                                    //    || isAllLines
                                    //    )
                                    //{
                                    //    // 设置售价状态 (正常售价,高于售价,低于售价)
                                    //    SetPriceStatus(line);
                                    //}
                                }
                            }
                        }
                    }
                }
            }
        }

        // 是否此行物料变更
        /// <summary>
        /// 是否此行物料变更
        /// </summary>
        /// <param name="changedSource"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private static bool IsChangedSOline(IUIRecord changedSource, IUIRecord line)
        {
            return // 复制时，没有变更来源
                    changedSource != null
                    // 如果是当前行物料改变
                    && changedSource.PrimaryKey == line.PrimaryKey;
        }

        private static bool IsGetPrice(bool isAllLines, Ship_ShipLinesRecord line)
        {
            return isAllLines
                    //|| line.FinallyPriceTC <= 0
                    || (line.DataRecordState == System.Data.DataRowState.Added
                // && salePrice > 0
                        //&& line.OrderPriceTC != salePrice
                        && line.FinallyPriceTC <= 0
                        )
                    ;
        }

        #endregion



        private int GetColIndex(IUFDataGrid grid, string ColName)
        {
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                if (grid.Columns[i].UIFieldID.Equals(ColName) || grid.Columns[i].ID.Equals(ColName))
                {
                    return i;
                }
            }
            return -1;
        }

        private IUIRecord GetRecordByDataGridIndex(IUIRecordCollection records, int index)
        {
            if (records != null
                && records.Count > 0
                )
            {
                int gridIndex = 0;
                for (int i = 0; i < records.Count; i++)
                {
                    IUIRecord record = records[i];
                    if (record != null
                        && !record.Hidden
                        )
                    {
                        if (gridIndex == index)
                            return record;

                        gridIndex++;
                    }
                }
            }
            return null;
        }

        #region ItemID  OnBeforeCellFocusEnter  不用了，不知道跟产品的什么东西冲突，导致，有时候物料放大镜 点击无反应；改为在弹出参照页面做过滤条件吧；

        private void RegisterGridItemIDFilterAssociation()
        {
            AssociationControl associationControl = new AssociationControl();
            associationControl.SourceServerControl = (this.DataGrid10);
            associationControl.SourceControl.EventName = "OnBeforeCellFocusEnter";
            ((UFWebClientGridAdapter)associationControl.SourceControl).FireEventCols.Add("ItemID");
            ClientCallBackFrm clientCallBackFrm = new ClientCallBackFrm();
            clientCallBackFrm.DoCustomerAction += (new ClientCallBackFrm.ActionCustomer(this.AppendItemFilterPath));
            clientCallBackFrm.ParameterControls.Add(this.DataGrid10);
            clientCallBackFrm.Add(associationControl);
            _strongPart.Controls.Add(clientCallBackFrm);
        }
        private object AppendItemFilterPath(CustomerActionEventArgs args)
        {
            //ArrayList arrayList = (ArrayList)args.get_ArgsHash()[UFWebClientGridAdapter.ALL_GRIDDATA_SelectedRows];
            //ArrayList arrayList2 = args.get_ArgsHash()[this.DataGrid10.get_ClientID()] as ArrayList;
            //int num = Convert.ToInt32(args.get_ArgsHash()[UFWebClientGridAdapter.FocusRow]);
            //object result;
            //if (num < 0 || num >= arrayList2.Count)
            //{
            //    result = args;
            //}
            //else
            //{
            //    Hashtable hashtable = (Hashtable)arrayList2[num];
            //    int num2 = Convert.ToInt32(args.get_ArgsHash()[UFWebClientGridAdapter.FocusColumn]);
            //    long num3 = Convert.ToInt64(hashtable["ID"]);
            //    IUIRecord iUIRecord = this.Model.Ship_ShipLines.FindRecordByFieldValue("ID", num3);
            //    Ship_ShipLinesRecord ship_ShipLinesRecord = iUIRecord as Ship_ShipLinesRecord;
            //    if (ship_ShipLinesRecord == null)
            //    {
            //        result = args;
            //    }
            //    else
            //    {
            //        UFWebClientGridAdapter uFWebClientGridAdapter = new UFWebClientGridAdapter(this.DataGrid10);
            //        string bOMFilterOPath = this.GetBOMFilterOPath(ship_ShipLinesRecord);
            //        uFWebClientGridAdapter.ResetColumnEditorAttribute("BomIDRef", UFWebClientRefControlAdapter.Attributes_CustomInParams, bOMFilterOPath);
            //        args.get_ArgsResult().Add(uFWebClientGridAdapter.get_ClientInstanceWithRefCustomInParams());
            //        result = args;
            //    }
            //}
            //return result;
            AddItemRefCondition();

            return args;
        }

        #endregion

        private void AddItemRefCondition()
        {

            if (_strongPart.Model.Ship.FocusedRecord != null
                //&& _strongPart.Model.Ship.FocusedRecord.SaleDept.GetValueOrDefault(-1) > 0
                )
            {
                IUFFldReferenceColumn itemRef = (IUFFldReferenceColumn)DataGrid10.Columns["ItemID"];
                //if (_strongPart.Model.Views["PR"].FocusedRecord["ReqDepartment"] == null || (long)_strongPart.Model.Views["PR"].FocusedRecord["ReqDepartment"] ==0)
                //_strongPart.Model.ErrorMessage.Message = "请先选择需求部门";
                // itemRef.CustomInParams = BaseAction.Symbol_AddCustomFilter + "= ID in (select ItemMaster from U9::VOB::Cus::HBHJianLiYuan::DeptItemSupplierBE::DeptItemSupplierLine where DeptItemSupplier.Department.ID=" + _strongPart.Model.Views["PR"].FocusedRecord["ReqDepartment"] + ")";

                string opath = "Code in (select disLine.ItemMaster.Code from U9::VOB::Cus::HBHJianLiYuan::DeptItemSupplierBE::DeptItemSupplierLine disLine where disLine.DeptItemSupplier.Department.Name='" + _strongPart.Model.Ship.FocusedRecord.SaleDept_Name + "')";
                //string opath = "Code = '000001'";

                // 特殊参照，用的是这个条件
                //string custFilter = BaseAction.Symbol_AddCustomFilter + "=";
                string custFilter = "ItemRefCondition=";
                if (itemRef.CustomInParams != null
                    && itemRef.CustomInParams.Contains(custFilter)
                    )
                {
                    itemRef.CustomInParams = itemRef.CustomInParams.Replace(custFilter, custFilter + opath + " and ");
                }
                else
                {
                    itemRef.CustomInParams = custFilter + opath;
                }

                string oldItemRef = PubClass.GetString(_strongPart.CurrentState["ItemRefCondition"]);
                //CurrentState["ItemRefCondition"] = value;
                if (!PubClass.IsNull(oldItemRef)
                    )
                {
                    _strongPart.CurrentState["ItemRefCondition"] = string.Format("({0}) and ({1})", opath, oldItemRef);
                }
                else
                {
                    _strongPart.CurrentState["ItemRefCondition"] = opath;
                }
            }
        }
    }
}
