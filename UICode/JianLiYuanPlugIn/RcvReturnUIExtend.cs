using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using UFSoft.UBF.UI.WebControls.Association;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.WebControls.Association.Adapter;
using UFSoft.UBF.UI.Controls;
using UFSoft.UBF.UI.WebControls;
using UFSoft.UBF.UI.ActionProcess;
using UFIDA.U9.SCM.PM.ReturnUI;
using HBH.DoNet.DevPlatform.EntityMapping;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInUI
{
    public class RcvReturnUIExtend : UFSoft.UBF.UI.Custom.ExtendedPartBase
    {
        public UFSoft.UBF.UI.IView.IPart part;
        private UFIDA.U9.SCM.PM.ReturnUI.ReturnMainUIFormWebPart _strongPart;
        IUFDataGrid LineDataGrid_MainBody;
        // 供应商参照
        IUFFldReference refSupt;
        // 物料参照
        IUFFldReference refItemMaster;
        
        public override void AfterInit(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.AfterInit(Part, args);

            part = Part;

            _strongPart = Part as ReturnMainUIFormWebPart;

            LineDataGrid_MainBody = (IUFDataGrid)part.GetUFControlByName(part.TopLevelContainer, "LineDataGrid_MainBody");
            //Register_DataGrid10_Item_CallBack();//料品改变事件，自动带出单价
            //RegisterGridCellDataChangedCallBack();

            // 部门参照PostBack
            // Card3    TabControl0     TabPage0    ReturnDept145
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
                        IUFFldReference refDept = (IUFFldReference)part.GetUFControlByName(tp1, "ReturnDept145");
                        refSupt = (IUFFldReference)part.GetUFControlByName(tp1, "Supplier_Supplier224");

                        if (refDept != null)
                        {
                            refDept.ContentChanged += new EventHandler(refDept_ContentChanged);
                            refDept.AutoPostBack = true;
                        }
                    }
                }


                //// 物料参照
                //// Card3    TabControl1     TabPage2    ItemInfo_ItemID57
                //IUFTabControl tc1 = (IUFTabControl)part.GetUFControlByName(card3, "TabControl1");
                //if (tc1 != null
                //    && tc1.TabPages != null
                //    && tc1.TabPages.Count > 0
                //    )
                //{
                //    IUFTabPage tpItem = null;

                //    foreach (IUFTabPage page in tc1.TabPages)
                //    {
                //        if (page != null
                //            && page.ID == "TabPage2"
                //            )
                //        {
                //            tpItem = page;
                //            break;
                //        }
                //    }

                //    if (tpItem != null)
                //    {
                //        refItemMaster = (IUFFldReference)part.GetUFControlByName(tpItem, "ItemInfo_ItemID57");
                //    }
                //}
            }

        }

        public override void BeforeRender(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.BeforeRender(Part, args);
        }

        public override void AfterRender(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.AfterRender(Part, args);

            // Supplier_Supplier224


            if (_strongPart.Model.Receivement.FocusedRecord != null
                //&& _strongPart.Model.Ship.FocusedRecord.SaleDept.GetValueOrDefault(-1) > 0
                )
            {
                // 供应商过滤条件
                string suptOpath = "Code in (select disLine.Supplier.Code from U9::VOB::Cus::HBHJianLiYuan::DeptItemSupplierBE::DeptItemSupplierLine disLine where disLine.DeptItemSupplier.Department.Name='" + _strongPart.Model.Receivement.FocusedRecord.ReturnDept_Name + "')";

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


                //// 物料过滤条件
                //IUFFldReferenceColumn itemRef = (IUFFldReferenceColumn)LineDataGrid_MainBody.Columns["ItemInfo_ItemID"];
                //string opath = "Code in (select disLine.ItemMaster.Code from U9::VOB::Cus::HBHJianLiYuan::DeptItemSupplierBE::DeptItemSupplierLine disLine where disLine.DeptItemSupplier.Department.Name='" + _strongPart.Model.Receivement.FocusedRecord.ReturnDept_Name + "')";
                ////string opath = "Code = '000001'";

                //string custFilter = BaseAction.Symbol_AddCustomFilter + "=";
                //// 特殊物料参照，用的是这个条件
                ////string custFilter = "ItemRefCondition=";
                //// DataGrid参照列
                //if (itemRef.CustomInParams != null
                //    && itemRef.CustomInParams.Contains(custFilter)
                //    )
                //{
                //    itemRef.CustomInParams = itemRef.CustomInParams.Replace(custFilter, custFilter + opath + " and ");
                //}
                //else
                //{
                //    itemRef.CustomInParams = custFilter + opath;
                //}
                //// 参照
                //if (refItemMaster.CustomInParams != null
                //    && refItemMaster.CustomInParams.Contains(custFilter)
                //    )
                //{
                //    refItemMaster.CustomInParams = refItemMaster.CustomInParams.Replace(custFilter, custFilter + opath + " and ");
                //}
                //else
                //{
                //    refItemMaster.CustomInParams = custFilter + opath;
                //}
            }
        }


        #region 事件

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
