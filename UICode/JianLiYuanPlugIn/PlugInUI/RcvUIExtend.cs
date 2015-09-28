using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using UFIDA.U9.SCM.PM.ReceivementUIModel;
using UFIDA.U9.Cust.HBH.Common.CommonLibary;
using UFSoft.UBF.UI.WebControls.Association;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.WebControls.Association.Adapter;
using UFSoft.UBF.UI.Controls;
using UFSoft.UBF.UI.WebControls;
using UFSoft.UBF.UI.ActionProcess;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInUI
{
    public class RcvUIExtend : UFSoft.UBF.UI.Custom.ExtendedPartBase
    {
        public UFSoft.UBF.UI.IView.IPart part;
        private UFIDA.U9.SCM.PM.ReceivementUIModel.ReceivementMainUIFormWebPart _strongPart;
        IUFDataGrid lineDataGrid;
        // 供应商参照
        IUFFldReference refSupt;
        // 物料参照
        IUFFldReference refItemMaster;
        
        public override void AfterInit(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.AfterInit(Part, args);

            part = Part;

            _strongPart = Part as ReceivementMainUIFormWebPart;

            lineDataGrid = (IUFDataGrid)part.GetUFControlByName(part.TopLevelContainer, "LineDataGrid");
            //Register_DataGrid10_Item_CallBack();//料品改变事件，自动带出单价
            //RegisterGridCellDataChangedCallBack();

            // 料品改变Post，自动带出单价(部门、料品-->供应商-->价表行-->价格)
            Regist_OnChangePostBack_DataGrid_PreDiscountPrice();

            // 部门参照PostBack
            // Card3    TabControl0     TabPage0    RcvDepartment242
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
                            && page.ID == "TabPage0"
                            )
                        {
                            tp1 = page;
                            break;
                        }
                    }

                    if (tp1 != null)
                    {
                        IUFFldReference refDept = (IUFFldReference)part.GetUFControlByName(tp1, "RcvDepartment242");
                        refSupt = (IUFFldReference)part.GetUFControlByName(tp1, "Supplier_Supplier105");

                        if (refDept != null)
                        {
                            refDept.ContentChanged += new EventHandler(refDept_ContentChanged);
                            refDept.AutoPostBack = true;
                        }
                    }
                }


                // 物料参照
                // Card3    TabControl1     TabPageItem    ItemInfo_ItemID103
                IUFTabControl tc1 = (IUFTabControl)part.GetUFControlByName(card3, "TabControl1");
                if (tc1 != null
                    && tc1.TabPages != null
                    && tc1.TabPages.Count > 0
                    )
                {
                    IUFTabPage tpItem = null;

                    foreach (IUFTabPage page in tc1.TabPages)
                    {
                        if (page != null
                            && page.ID == "TabPageItem"
                            )
                        {
                            tpItem = page;
                            break;
                        }
                    }

                    if (tpItem != null)
                    {
                        refItemMaster = (IUFFldReference)part.GetUFControlByName(tpItem, "ItemInfo_ItemID103");
                    }
                }
            }
            //refSupt.CustomInParams = "'Test'='Test'";

        }

        public override void BeforeRender(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.BeforeRender(Part, args);
        }

        public override void AfterRender(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.AfterRender(Part, args);

            // Supplier_Supplier105


            if (_strongPart.Model.Receivement.FocusedRecord != null
                //&& _strongPart.Model.Ship.FocusedRecord.SaleDept.GetValueOrDefault(-1) > 0
                )
            {
                // 供应商过滤条件
                string suptOpath = "Code in (select disLine.Supplier.Code from U9::VOB::Cus::HBHJianLiYuan::DeptItemSupplierBE::DeptItemSupplierLine disLine where disLine.DeptItemSupplier.Department.Name='" + _strongPart.Model.Receivement.FocusedRecord.RcvDepartment_Name + "')";

                string refCustFilter = BaseAction.Symbol_AddCustomFilter + "=";
                if (refSupt.CustomInParams != null
                    && refSupt.CustomInParams.Contains(refCustFilter)
                    )
                {
                    refSupt.CustomInParams = refSupt.CustomInParams.Replace(refCustFilter, refCustFilter + suptOpath + " and ");
                }
                else
                {
                    refSupt.CustomInParams = refCustFilter + suptOpath;
                }

                string oldSuptRef = PubClass.GetString(_strongPart.CurrentState["SupplierCondition"]);
                //CurrentState["ItemRefCondition"] = value;
                if (!PubClass.IsNull(oldSuptRef)
                    )
                {
                    _strongPart.CurrentState["SupplierCondition"] = string.Format("({0}) and ({1})", suptOpath, oldSuptRef);
                }
                else
                {
                    _strongPart.CurrentState["SupplierCondition"] = suptOpath;
                }


                // 物料过滤条件
                IUFFldReferenceColumn itemRef = (IUFFldReferenceColumn)lineDataGrid.Columns["ItemInfo_ItemID"];
                string opath = "Code in (select disLine.ItemMaster.Code from U9::VOB::Cus::HBHJianLiYuan::DeptItemSupplierBE::DeptItemSupplierLine disLine where disLine.DeptItemSupplier.Department.Name='" + _strongPart.Model.Receivement.FocusedRecord.RcvDepartment_Name + "')";
                //string opath = "Code = '000001'";

                string custFilter = BaseAction.Symbol_AddCustomFilter + "=";
                // 特殊物料参照，用的是这个条件
                //string custFilter = "ItemRefCondition=";
                // DataGrid参照列
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
                // 参照
                if (refItemMaster.CustomInParams != null
                    && refItemMaster.CustomInParams.Contains(custFilter)
                    )
                {
                    refItemMaster.CustomInParams = refItemMaster.CustomInParams.Replace(custFilter, custFilter + opath + " and ");
                }
                else
                {
                    refItemMaster.CustomInParams = custFilter + opath;
                }
            }




            ////object lnk = this._strongPart.NameValues["lnk"];
            ////string url = "Cust_Rcv";
            ////if (lnk != null
            ////    && lnk.ToString() == url
            ////    )
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
            //        nvs.Add("RCV_Type", PubClass.GetString(this._strongPart.NameValues["RCV_Type"]));
            //        //// 有上面菜单栏，但是组织无法切换
            //        //this._strongPart.NavigatePage("Cust_Rcv", nvs);
            //        // 无上面菜单栏，但组织可以切换；
            //        this._strongPart.NavigateForm(urlID, nvs);
            //    }
            //}
        }


        #region 事件


        private void Regist_OnChangePostBack_DataGrid_PreDiscountPrice()
        {
            AssociationControl control = new AssociationControl();
            control.SourceServerControl = lineDataGrid;
            control.SourceControl.EventName = "OnCellDataChanged";
            //((IUFClientAssoGrid)control.SourceControl).FireEventCols.Add(this._strongPart.Model.Receivement_RcvLines.FieldDescFlexSegments_PubDescSeg3);
            ((IUFClientAssoGrid)control.SourceControl).FireEventCols.Add(HBHHelper.RcvLineHelper.DescFlexSegments_PreDiscountPriceUIField);
            //((IUFClientAssoGrid)control.SourceControl).FireEventCols.Add(FieldName_FinallyPriceTC);
            CodeBlock block = new CodeBlock();
            UFWebClientGridAdapter adapter = new UFWebClientGridAdapter(lineDataGrid);
            adapter.IsPostBack = true;
            adapter.PostBackTag = lineDataGrid.ID + "_" + control.SourceControl.EventName;
            block.TargetControls.addControl(adapter);
            control.addBlock(block);
            UFGrid dataGrid = lineDataGrid as UFGrid;
            dataGrid.GridCustomerPostBackEvent += new GridCustomerPostBackDelegate(OnChangePostBack_DataGrid_PreDiscountPrice);
        }

        private void OnChangePostBack_DataGrid_PreDiscountPrice(object sender, GridCustomerPostBackEventArgs e)
        {
            if (e.PostTag.ToString().EndsWith("OnCellDataChanged")
                )
            {
                //int qtyIndex = GetColIndex(datagrid, FieldName_ItemID);
                //&& e.ColIndex == qtyIndex

                //string uIFieldID = this.DataGrid10.Columns[e.ColIndex].UIFieldID;
                //if (uIFieldID == view.FieldCust_CustomerItemID.Name)

                string uIFieldID = lineDataGrid.Columns[e.ColIndex].UIFieldID;
                if (uIFieldID == HBHHelper.RcvLineHelper.DescFlexSegments_PreDiscountPriceUIField)
                {
                    //清除错误信息
                    _strongPart.Model.ClearErrorMessage();

                    _strongPart.DataCollect();
                    _strongPart.IsDataBinding = true; //当前事件执行后会进行数据绑定
                    _strongPart.IsConsuming = false;

                    //// 物料可以多选,还可以在物料参照里录入数量
                    //// 只计算最终价为0的行
                    //GetAllFinallyPrice(sender, e, false);

                    Receivement_RcvLinesRecord line = _strongPart.Model.Receivement_RcvLines.FocusedRecord;
                    decimal preDiscountPrice = PubClass.GetDecimal(line[HBHHelper.RcvLineHelper.DescFlexSegments_PreDiscountPriceUIField]);
                    decimal discountRate = PubClass.GetDecimal(line[HBHHelper.RcvLineHelper.DescFlexSegments_DiscountRateUIField]);
                    decimal discountLimit = PubClass.GetDecimal(line[HBHHelper.RcvLineHelper.DescFlexSegments_DiscountLimitUIField]);

                    decimal finallyPrice = HBHHelper.PPLineHelper.GetFinallyPrice(preDiscountPrice, discountRate, discountLimit);

                    // 差额
                    line[HBHHelper.RcvLineHelper.DescFlexSegments_PriceDifUIField] = preDiscountPrice - finallyPrice;

                    if (line.FinallyPriceTC != finallyPrice)
                    {
                        /*
		                private object GridCellValueChange_CallBack_Action(UFSoft.UBF.UI.WebControls.ClientCallBack.CustomerActionEventArgs args)
		                {
                         */


                        //decimal decimal14 = this.GetDecimal(hashtable[receivement_RcvLines.FieldFinallyPriceTC.Name]);
                        decimal oldPrice = line.FinallyPriceTC;
                        UIActionEventArgs uIActionEventArgs = new UIActionEventArgs();
                        uIActionEventArgs.Tag = oldPrice;
                        line.FinallyPriceTC = finallyPrice;
                        //line.FinallyPriceTC = finallyPrice;

                        _strongPart.Action.OnFinalPriceChange(new object(), uIActionEventArgs);
                    }
                }
            }
        }

        void refDept_ContentChanged(object sender, EventArgs e)
        {
            //清除错误信息
            _strongPart.Model.ClearErrorMessage();

            _strongPart.DataCollect();
            _strongPart.IsDataBinding = true; //当前事件执行后会进行数据绑定
            _strongPart.IsConsuming = false;

            // 这里需要清空下，否则会导致一直追加(收集完又追加新的)，导致录入部分编码、部分名称，无法过滤到任何供应商(ValueByClick=000)
            refSupt.CustomInParams = "";
            //_strongPart.CurrentState["SupplierCondition"] = "";

        }

        #endregion
    }
}
