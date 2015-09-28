namespace U9.VOB.Cus.HBHJianLiYuan
{
	using System;
	using System.Collections.Generic;
	using System.Text; 
	using UFSoft.UBF.AopFrame;	
	using UFSoft.UBF.Util.Context;
    using UFIDA.U9.Cust.HBH.Common.CommonLibary;
    using UFIDA.U9.PM.Rcv;
    using UFSoft.UBF.PL;
    using UFSoft.UBF.Business;
    using UFIDA.U9.SM.Ship;
    using UFIDA.U9.ISV.SM;
    using UFIDA.U9.Base;
    using UFIDA.U9.ISV.SM.Proxy;
    using UFIDA.U9.Base.FlexField.DescFlexField;
    using UFIDA.U9.CBO.Pub.Controller;
    using UFIDA.U9.Base.FlexField.ValueSet;
    using UFIDA.U9.CBO.SCM.Enums;
    using UFIDA.U9.CBO.SCM.Item;
    using UFIDA.U9.CBO.SCM.Customer;
    using UFIDA.U9.SM.Common;

	/// <summary>
	/// RcvToShipSV partial 
	/// </summary>	
	public partial class RcvToShipSV 
	{	
		internal BaseStrategy Select()
		{
			return new RcvToShipSVImpementStrategy();	
		}		
	}
	
    //#region  implement strategy	
	/// <summary>
	/// Impement Implement
	/// 
	/// </summary>	
	internal partial class RcvToShipSVImpementStrategy : BaseStrategy
	{
		public RcvToShipSVImpementStrategy() { }

		public override object Do(object obj)
		{						
			RcvToShipSV bpObj = (RcvToShipSV)obj;
			
			//get business operation context is as follows
			//IContext context = ContextManager.Context	
			
			//auto generating code end,underside is user custom code
			//and if you Implement replace this Exception Code...
            //throw new NotImplementedException();

            Receivement.EntityList lstRcv = null;

            // 按收货ID转单
            if (bpObj.RcvIDs != null
                && bpObj.RcvIDs.Count > 0
                )
            { 
                string opath = string.Format("ID in ({0}) and Org = {1}"
                    , PubClass.GetOpathFromList(bpObj.RcvIDs)
                    , Context.LoginOrg.ID
                    );

                lstRcv = Receivement.Finder.FindAll(opath);
            }
            // 按收货日期转单
            else
            {
                DateTime date = bpObj.Date;
                // 没传入日期，当天
                if (PubClass.IsNullDate2010(date))
                {
                    date = DateTime.Today;
                }

                string opath = string.Format("BusinessDate = @Date and Org = {1}"
                    , Context.LoginOrg.ID
                    );

                lstRcv = Receivement.Finder.FindAll(opath,new OqlParam(date));
            }

            if (lstRcv != null
                && lstRcv.Count > 0
                )
            {
                foreach (Receivement rcv in lstRcv)
                {
                    //using (ISession session = Session.Open())
                    //{



                    //    session.Commit();
                    //}

                    // 新增
                    if (!bpObj.IsRemove)
                    {
                        List<RcvLine> listRcvline = GetNeedToShipLine(rcv);

                        if (listRcvline != null
                            && listRcvline.Count > 0
                            )
                        {
                            RcvLine firstLine = listRcvline[0];

                            CreateShipSVProxy createSV = new CreateShipSVProxy();
                            createSV.ShipDTOs = new List<UFIDA.U9.ISV.SM.ShipDTOForIndustryChainData>();

                            UFIDA.U9.ISV.SM.ShipDTOForIndustryChainData shipDTO = new UFIDA.U9.ISV.SM.ShipDTOForIndustryChainData();

                            shipDTO.DescFlexField = new DescFlexSegmentsData();
                            // 领料部门 = 01 大灶
                            shipDTO.DescFlexField.PrivateDescSeg1 = "01";
                            shipDTO.DescFlexField.PrivateDescSeg2 = rcv.ID.ToString();
                            shipDTO.DescFlexField.PrivateDescSeg3 = rcv.DocNo;

                            shipDTO.DocumentType = new CommonArchiveDataDTOData();
                            shipDTO.DocumentType.Code = PubHelperExtend.Const_ShipDocTypeCode;

                            string deptCode = string.Empty;
                            string sellerCode = string.Empty;
                            if (firstLine.RcvDept != null)
                            {
                                deptCode = firstLine.RcvDept.Code;
                            }
                            if (firstLine.WhMan != null)
                            {
                                sellerCode = firstLine.WhMan.Code;
                            }

                            shipDTO.OrderBy = new CommonArchiveDataDTOData();
                            // 客户取部门对应的 第一个客户（王希说，部门-客户，是一对一的）
                            //shipDTO.OrderBy.Code = PubHelperExtend.Const_ShipCustomerCode;
                            string custOpath = string.Format("Department.Code=@DeptCode and Effective.IsEffective = 1 order by ID");
                            Customer customer = Customer.Finder.Find(custOpath, new OqlParam(deptCode));
                            if (customer != null)
                            {
                                shipDTO.OrderBy.Code = customer.Code;
                            }

                            if (firstLine.RcvDept != null)
                            {
                                shipDTO.SaleDept = new CommonArchiveDataDTOData();
                                //shipDTO.SaleDept.Code = firstLine.RcvDept.Code;
                                shipDTO.SaleDept.Code = deptCode;
                            }

                            if (firstLine.WhMan != null)
                            {
                                shipDTO.Seller = new CommonArchiveDataDTOData();
                                //shipDTO.Seller.Code = firstLine.WhMan.Code;
                                shipDTO.Seller.Code = sellerCode;
                            }

                            // 单据日期
                            shipDTO.BusinessDate = DateTime.Today;

                            // 出库确认日期
                            shipDTO.ShipConfirmDate = DateTime.Today;

                            //DefineValue defValue = null;
                            //// 订单类型 值集编码    Z03
                            //// 1,零售业务   ;   2,批发业务  ;   3,现款销售  ;   4,售后退换  ;
                            //defValue = DefineValue.Finder.Find("ValueSetDef.Code=@DefCode and Name=@Name", new OqlParam("Z03"), new OqlParam(entity.OrderType));
                            //if (defValue != null)
                            //{
                            //    // 交易类型/订单类型	OrderType		公共段2(自定义字段，手工同步值集)
                            //    shipDTO.DescFlexField.PubDescSeg2 = defValue.Code;
                            //}
                            //// 订单来源 值集编码    Z04
                            //// 1,API抓单    ;   2,手工新建  ;   3,现款销售  ;   4,Excel导入 ;   5,售后建单  ;
                            //defValue = DefineValue.Finder.Find("ValueSetDef.Code=@DefCode and Name=@Name", new OqlParam("Z04"), new OqlParam(entity.OrderSource));
                            //if (defValue != null)
                            //{
                            //    // 订单来源	OrderSource		公共段3(自定义字段，手工同步值集)
                            //    shipDTO.DescFlexField.PubDescSeg3 = defValue.Code;
                            //}

                            //shipDTO.IsPriceIncludeTax = true;

                            //// 业务员  当前组织扩展字段 是否默认业务员 = true
                            //shipDTO.Seller = new CommonArchiveDataDTOData();
                            ////shipDTO.Seller.ID = seller.ID;
                            //shipDTO.Seller.Code = seller.Code;
                            //if (seller.Dept != null)
                            //{
                            //    shipDTO.SaleDept = new CommonArchiveDataDTOData();
                            //    //shipDTO.SaleDept.ID = seller.DeptKey.ID;
                            //    shipDTO.SaleDept.Code = seller.Dept.Code;
                            //}

                            //// 币种、收款条件、 (客户)
                            //if (customer.TradeCurrency == null)
                            //{
                            //    shipDTO.TC = new CommonArchiveDataDTOData();
                            //    shipDTO.TC.Code = Const_TcCode;
                            //}
                            //// 收款条件
                            //if (customer.RecervalTerm == null)
                            //{
                            //    shipDTO.ReceivableTerm = new CommonArchiveDataDTOData();
                            //    shipDTO.ReceivableTerm.Code = Const_RecTermCode;
                            //    shipDTO.ReceivableTerm_Code = Const_RecTermCode;
                            //}
                            //// 税组合 
                            //if (customer.TaxSchedule == null)
                            //{
                            //    shipDTO.TaxSchedule = new CommonArchiveDataDTOData();
                            //    shipDTO.TaxSchedule.Code = Const_TaxSchedule;
                            //}
                            //// 出货原则编码
                            //if (customer.ShippmentRule == null)
                            //{
                            //    shipDTO.ShipmentRule = new CommonArchiveDataDTOData();
                            //    shipDTO.ShipmentRule.Code = Const_ShipRuleCode;
                            //}
                            //// 应收立账条码编码
                            //if (customer.ARConfirmTerm == null)
                            //{
                            //    shipDTO.ConfirmTerm = new CommonArchiveDataDTOData();
                            //    shipDTO.ConfirmTerm.Code = Const_ARConfirmTerm;
                            //}
                            //// 价格含税
                            //shipDTO.IsPriceIncludeTax = Const_IsPriceIncludeTax;

                            //// 出货方式     (Deliver配送 1、SelfPickUp自提 0)       (系统单据默认 = -1)
                            //shipDTO.ShipMode = (int)ShipModeEnumData.Empty;

                            //// 交易方式 
                            //shipDTO.TradeMode = (int)TradeTypeEnumData.Empty;

                            //// 核算组织是否可改
                            //shipDTO.IsAccountOrgChangeable = true;

                            //// 立账方式  (凭单制 0、非凭单制 1)
                            //if (shipDocType != null
                            //    )
                            //{
                            //    if (shipDocType.ConfirmMode != null)
                            //    {
                            //        shipDTO.ConfirmMode = shipDocType.ConfirmMode.Value;
                            //    }
                            //    if (shipDocType.ConfirmAccording != null)
                            //    {
                            //        shipDTO.ConfirmAccording = new CommonArchiveDataDTOData();
                            //        shipDTO.ConfirmAccording.Code = shipDocType.ConfirmAccording.Code;
                            //    }
                            //}

                            // (审核)  单据日期不能早于系统启用日期。

                            shipDTO.ShipLines = new List<UFIDA.U9.ISV.SM.ShipLineDTOForIndustryChainData>();


                            //foreach (RcvLine line in rcv.RcvLines)
                            foreach (RcvLine line in listRcvline)
                            {
                                if (line != null)
                                {
                                    UFIDA.U9.ISV.SM.ShipLineDTOForIndustryChainData shiplineDTO = new UFIDA.U9.ISV.SM.ShipLineDTOForIndustryChainData();

                                    shiplineDTO.DescFlexField = new DescFlexSegmentsData();
                                    shiplineDTO.DescFlexField.PrivateDescSeg3 = line.ID.ToString();
                                    shiplineDTO.DescFlexField.PrivateDescSeg4 = line.DocLineNo.ToString();
                                    shiplineDTO.DescFlexField.PrivateDescSeg5 = rcv.ID.ToString();
                                    shiplineDTO.DescFlexField.PrivateDescSeg6 = rcv.DocNo;

                                    //ItemMaster item = ItemMaster.Finder.Find(string.Format("Code='{0}'", line.MerchantCode));

                                    //if (item == null)
                                    //{
                                    //    throw new BusinessException(string.Format("物料编码[{0}]无法找到!", line.MerchantCode));
                                    //}

                                    shiplineDTO.ItemInfo = new ItemInfoData();
                                    //shiplineDTO.ItemInfo.ItemCode = line.MerchantCode;
                                    shiplineDTO.ItemInfo.ItemCode = line.ItemInfo.ItemID.Code;

                                    shiplineDTO.ShipQtyTUAmount = line.ArriveQtyTU;
                                    shiplineDTO.ShipQtyTBUAmount = line.ArriveQtyTBU;

                                    shiplineDTO.TotalMoneyTC = line.TotalMnyTC;
                                    shiplineDTO.TotalMoney = line.TotalMnyAC;
                                    shiplineDTO.TotalMoneyFC = line.TotalMnyFC;

                                    //shiplineDTO.FinallyPriceTC = Math.Round(shiplineDTO.TotalMoneyTC / shiplineDTO.ShipQtyTUAmount, 2);
                                    //shiplineDTO.FinallyPrice = shiplineDTO.FinallyPriceTC;

                                    shiplineDTO.ShipLineMemo = line.Memo;

                                    //// 赠品
                                    //if (shiplineDTO.TotalMoneyTC == 0)
                                    //{
                                    //    shiplineDTO.DonationType = (int)UFIDA.U9.CBO.SCM.Enums.FreeTypeEnumData.Present; // FreeTypeEnumData.Largess;
                                    //}

                                    // 免费类型        赠品
                                    shiplineDTO.DonationType = (int)UFIDA.U9.CBO.SCM.Enums.FreeTypeEnumData.Present; // FreeTypeEnumData.Largess;

                                    // 批号            取当天入库批号
                                    if (line.RcvLot != null)
                                    {
                                        shiplineDTO.LotInfo = new UFIDA.U9.CBO.SCM.PropertyTypes.LotInfoData();
                                        shiplineDTO.LotInfo.LotCode = line.RcvLot.LotCode;
                                        shiplineDTO.LotInfo.LotMaster = new UFIDA.U9.Base.PropertyTypes.BizEntityKeyData();
                                        shiplineDTO.LotInfo.LotMaster.EntityID = line.RcvLot.Key.ID;
                                        //shiplineDTO.LotInfo.LotMaster.EntityType = line.RcvLot.GetType();

                                    }

                                    // 仓库	WarehouseNo		存储地点(先手工，后自动)
                                    if (line.WhKey != null)
                                    {
                                        shiplineDTO.WH = new CommonArchiveDataDTOData();
                                        shiplineDTO.WH.ID = line.WhKey.ID;
                                    }


                                    //shiplineDTO.ConfirmMode = shipDTO.ConfirmMode;

                                    //shiplineDTO.ConfirmAccording = shipDTO.ConfirmAccording;

                                    shipDTO.ShipLines.Add(shiplineDTO);
                                }
                            }

                            createSV.ShipDTOs.Add(shipDTO);

                            List<DocKeyDTOData> list = createSV.Do();

                            // 自动审核
                            if (list != null
                                && list.Count > 0
                                && list[0] != null
                                )
                            {
                                DocKeyDTOData shipKey = list[0];

                                if (shipKey != null)
                                {
                                    Ship ship = Ship.Finder.FindByID(shipKey.DocID);

                                    if (ship != null)
                                    {
                                        // 提交
                                        UFIDA.U9.SM.Ship.Proxy.ShipmentSubmitProxy proxySumit = new UFIDA.U9.SM.Ship.Proxy.ShipmentSubmitProxy();
                                        proxySumit.ShipmentKey = ship.ID;
                                        proxySumit.SysVersion = ship.SysVersion;
                                        ErrorMessageDTOData dataSumit = proxySumit.Do();

                                        //审核
                                        ship = Ship.Finder.FindByID(shipKey.DocID);
                                        if (ship != null)
                                        {
                                            UFIDA.U9.SM.Ship.Proxy.ShipmentApproveProxy proxyApp = new UFIDA.U9.SM.Ship.Proxy.ShipmentApproveProxy();
                                            proxyApp.ShipmentKey = ship.ID;
                                            proxyApp.SysVersion = ship.SysVersion;
                                            proxyApp.IsUnApprove = false;

                                            ErrorMessageDTOData dataApp = proxyApp.Do();
                                        }
                                    }

                                }
                            }

                        }
                    }
                    // 删除
                    else
                    {
                        string opath = string.Format("DescFlexField.PrivateDescSeg2=@RcvID");

                        Ship.EntityList lstShip = Ship.Finder.FindAll(opath, new OqlParam(rcv.ID.ToString()));

                        if (lstShip != null
                            && lstShip.Count > 0
                            )
                        {
                            using (ISession session = Session.Open())
                            {
                                for (int i = lstShip.Count - 1; i >= 0; i--)
                                {
                                    Ship ship = lstShip[i];

                                    if (ship != null)
                                    {
                                        // 弃审
                                        if (ship.Status == ShipStateEnum.Approved)
                                        {
                                            UFIDA.U9.SM.Ship.Proxy.ShipmentApproveProxy proxyApp = new UFIDA.U9.SM.Ship.Proxy.ShipmentApproveProxy();
                                            proxyApp.ShipmentKey = ship.ID;
                                            proxyApp.SysVersion = ship.SysVersion;
                                            proxyApp.IsUnApprove = true;

                                            ErrorMessageDTOData dataApp = proxyApp.Do();
                                        }

                                        ship = Ship.Finder.FindByID(ship.ID);
                                        ship.Remove();
                                    }
                                }

                                session.Commit();
                            }
                        }
                    }
                }
            }

            return null;
		}

        private List<RcvLine> GetNeedToShipLine(Receivement rcv)
        {
            List<RcvLine> list = new List<RcvLine>();

            foreach (RcvLine line in rcv.RcvLines)
            {
                if (IsNeedToShip(line))
                {
                    list.Add(line);
                }
            }

            return list;
        }

        private bool IsNeedToShip(RcvLine line)
        {
            if (line != null
                && line.ItemInfo != null
                && line.ItemInfo.ItemID != null
                && line.ItemInfo.ItemID.PurchaseCategory != null
                )
            {
                // 蔬菜类物料在系统中采购分类为：0201 蔬菜类 0202 豆制品类
                if (line.ItemInfo.ItemID.PurchaseCategory.Code == "0201"
                    || line.ItemInfo.ItemID.PurchaseCategory.Code == "0202"
                    )
                {
                    return true;
                }
            }
            return false;
        }		
	}

    //#endregion
	
	
}