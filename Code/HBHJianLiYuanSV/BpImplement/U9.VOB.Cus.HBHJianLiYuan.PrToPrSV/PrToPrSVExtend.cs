namespace U9.VOB.Cus.HBHJianLiYuan.PrToPrSV
{
	using System;
	using System.Collections.Generic;
	using System.Text; 
	using UFSoft.UBF.AopFrame;	
	using UFSoft.UBF.Util.Context;
    using UFIDA.U9.ISV.PRSV.Proxy;
    using UFIDA.U9.PR.PurchaseRequest;
    using UFSoft.UBF.PL;
    using UFSoft.UBF.Business;
    using UFIDA.U9.Base;
    using UFIDA.U9.CBO.SCM.Enums;
    using UFIDA.U9.CBO.Pub.Controller;

	/// <summary>
	/// PrToPrSV partial 
	/// </summary>	
	public partial class PrToPrSV 
	{	
		internal BaseStrategy Select()
		{
			return new PrToPrSVImpementStrategy();	
		}		
	}
	
	#region  implement strategy	
	/// <summary>
	/// Impement Implement
	/// 
	/// </summary>	
	internal partial class PrToPrSVImpementStrategy : BaseStrategy
	{
		public PrToPrSVImpementStrategy() { }

		public override object Do(object obj)
        {
            PrToPrSV bpObj = (PrToPrSV)obj;

            //get business operation context is as follows
            //IContext context = ContextManager.Context	

            //auto generating code end,underside is user custom code
            //and if you Implement replace this Exception Code...
            //Dictionary<long, List<PRInfoDTO>> dicOrgToPoInfo = new Dictionary<long, List<PRInfoDTO>>();
            Dictionary<long, long> dicSrcPrlineToTarget = new Dictionary<long, long>();
            //返回结果
            UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTO resultData = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTO();
            List<UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTO> resultDataList  = new List<UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTO>();
            foreach (PR.EntityKey prDto in bpObj.PR)
            {
                UFIDA.U9.PR.PurchaseRequest.PR pr = UFIDA.U9.PR.PurchaseRequest.PR.Finder.FindByID(prDto.ID);
                UFIDA.U9.PR.PurchaseRequest.PRLine findPR = UFIDA.U9.PR.PurchaseRequest.PRLine.Finder.Find("DescFlexSegments.PrivateDescSeg3='" + prDto.ID.ToString() + "'");
                if (findPR != null)
                {
                    resultData.Name = "请购单"+pr.DocNo+"已经下发";
                    resultDataList.Add(resultData);
                    continue;
                    //return resultDataList;
                }
                if (pr == null)
                {
                    //throw new Exception("请购单不能为空");
                    resultData.Name = "请购单不能为空";
                    resultDataList.Add(resultData);
                    return resultDataList;
                }
                if (pr.Status == PRStatusEnum.OpenOpen || pr.Status == PRStatusEnum.Approving)
                {
                    resultData.Name = "开立/核准中状态不能下发";
                    resultDataList.Add(resultData);
                    return resultDataList;
                }
                if (pr.ReqDepartment == null)
                {
                    //throw new Exception("请购单部门不能为空");
                    resultData.Name = "请购单"+pr.DocNo+"需求部门不能为空";
                    resultData.ID = pr.ID;
                    resultData.Code = pr.DocNo;
                    resultDataList.Add(resultData);
                    continue;
                    //return resultDataList;
                }
                if (pr.ReqDepartment.DescFlexField.PrivateDescSeg1 == "")
                {
                    //throw new Exception("请购单部门对应需求组织不能为空");
                    resultData.Name = "请购单部门对应需求组织不能为空";
                    resultData.ID = pr.ID;
                    resultData.Code = pr.DocNo;
                    resultDataList.Add(resultData);
                    return resultDataList;
                }
                if (pr.ReqDepartment.DescFlexField.PrivateDescSeg1.Equals(pr.Org.Code))
                {
                    resultData.ID = pr.ID;
                    resultData.Code = pr.DocNo;
                    resultDataList.Add(resultData);
                    return resultDataList;
                }
                // 跨组织，生成供应商对应组织的请购单;
                //需求组织
                UFIDA.U9.Base.Organization.Organization org = UFIDA.U9.Base.Organization.Organization.FindByCode(pr.ReqDepartment.DescFlexField.PrivateDescSeg1.ToString());
                //ContextDTO contextDTO = new ContextDTO();
                //contextDTO.UserCode = UFSoft.UBF.Util.Context.PlatformContext.Current.UserCode;
                //contextDTO.OrgCode = org.Code;
                //contextDTO.EntCode = UFSoft.UBF.Util.Context.PlatformContext.Current.EnterpriseID;
                //contextDTO.WriteToContext();

                //查询请购单服务
                UFIDA.U9.ISV.PRSV.Proxy.QueryPRSVForOtherSysProxy queryProxy = new QueryPRSVForOtherSysProxy();
                queryProxy.PRIDDocNoList = new List<UFIDA.U9.ISV.PRSV.PRConditionDTOData>();
                {
                    UFIDA.U9.ISV.PRSV.PRConditionDTOData queryprdto = new UFIDA.U9.ISV.PRSV.PRConditionDTOData();
                    queryprdto.PRID = pr.ID;
                    queryprdto.DocNO = pr.DocNo;
                    queryProxy.PRIDDocNoList.Add(queryprdto);
                }
                List<UFIDA.U9.ISV.PRSV.OtherSystemPRDTOData> listPRDTO = queryProxy.Do();
                //创建请购单服务
                UFIDA.U9.ISV.PRSV.Proxy.CreatePRSVForOtherSysProxy proxy = new UFIDA.U9.ISV.PRSV.Proxy.CreatePRSVForOtherSysProxy();
                //foreach (UFIDA.U9.ISV.PRSV.OtherSystemPRDTOData prDTO in listPRDTO)
                //{
                //    prDTO.Org = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                //    prDTO.Org.Code = org.Code;
                //    prDTO.BusinessDate = DateTime.Now;
                //    //prDTO.PRDocType = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                //    //prDTO.PRDocType.Code = prDTO.PRDocType.Code;
                //    foreach (UFIDA.U9.ISV.PRSV.OtherSystemPRLineDTOData prLineDTO in prDTO.PRLineList)
                //    {
                //        UFIDA.U9.ISV.PRSV.OtherSystemPRLineDTOData prlinedata = prLineDTO;
                //        prLineDTO.CurrentOrg = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                //        prLineDTO.CurrentOrg.Code = org.Code;
                //        prLineDTO.RegOrg = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                //        prLineDTO.RegOrg.Code = org.Code;
                //        //prLineDTO.ReqDept = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                //        //prLineDTO.ReqDept.Code = prlinedata.ReqDept.Code;
                //        ////prLineDTO.ItemInfo = new UFIDA.U9.CBO.SCM.Item.ItemInfoData();
                //        ////prLineDTO.ItemInfo.ItemCode = prlinedata.ItemInfo.ItemCode;
                //        ////prLineDTO.ItemInfo.ItemName = prlinedata.ItemInfo.ItemName;
                //        ////if (prline.SeiBanMaster != null)
                //        //if (prLineDTO.SeiBan != null)
                //        //{
                //        //    prLineDTO.SeiBan = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                //        //    prLineDTO.SeiBan.ID = 0;
                //        //    prLineDTO.SeiBan.Code = prlinedata.SeiBan.Code;
                //        //}

                //        //prLineDTO.SuggestedSupplier = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                //        //prLineDTO.SuggestedSupplier.Code = prlinedata.SuggestedSupplier.Code;

                //        //prLineDTO.ReqDept = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                //        //prLineDTO.ReqDept.Code = prlinedata.ReqDept.Code;
                //    }
                //}
                //proxy.PRDTOList = listPRDTO;
                //proxy.TargetOrgCode = org.Code;
                proxy.PRDTOList = new List<UFIDA.U9.ISV.PRSV.OtherSystemPRDTOData>();
                {
                    UFIDA.U9.ISV.PRSV.OtherSystemPRDTOData prDTO = new UFIDA.U9.ISV.PRSV.OtherSystemPRDTOData();

                    prDTO.BusinessDate = DateTime.Today;
                    prDTO.PRDocType = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                    prDTO.PRDocType.Code = pr.PRDocType.Code;
                    prDTO.Org = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                    prDTO.Org.Code = org.Code;
                    // 币种放到了行上赋值

                    prDTO.PRLineList = new List<UFIDA.U9.ISV.PRSV.OtherSystemPRLineDTOData>();

                    foreach (PRLine line in pr.PRLineList)
                    {
                        UFIDA.U9.ISV.PRSV.OtherSystemPRLineDTOData lineData = GetPRLineDTO(line, prDTO);

                        if (lineData != null)
                        {
                            prDTO.PRLineList.Add(lineData);
                        }
                    }

                    proxy.PRDTOList.Add(prDTO);
                }
                long id = org.ID;
                List<UFIDA.U9.ISV.PRSV.PRBizKeyDTOData> lstPR = proxy.Do(id);


                if (lstPR != null && lstPR.Count > 0)
                {
                    foreach (UFIDA.U9.ISV.PRSV.PRBizKeyDTOData prheadDTO in lstPR)
                    {
                        if (prheadDTO != null)
                        {
                            PR prhead = PR.Finder.Find("DocNo=@DocNo and Org=@Org", new OqlParam(prheadDTO.DocNo), new OqlParam(prheadDTO.Org.ID));
                            if (prhead != null)
                            {
                                resultData.ID = prhead.ID;
                                resultData.Code = prhead.DocNo;
                                // 提交请购单
                                using (ISession session = Session.Open())
                                {
                                    prhead.ActivityType = ActivityTypeEnum.SrvUpdate;
                                    prhead.Status = PRStatusEnum.Approving;

                                    foreach (PRLine line in prhead.PRLineList)
                                    {
                                        if (line != null)
                                        {
                                            line.Status = prhead.Status;
                                        }
                                    }

                                    session.Commit();
                                }

                                prhead = PR.Finder.FindByID(prhead.ID);
                                // 审核请购单
                                using (ISession session = Session.Open())
                                {
                                    prhead.ActivityType = ActivityTypeEnum.SrvUpdate;
                                    prhead.Status = PRStatusEnum.Approved;
                                    prhead.ApprovedOn = DateTime.Now;
                                    prhead.ApprovedBy = Context.LoginUser;
                                    //prhead.CancelApprovedOn = null;
                                    prhead.CancelApprovedBy = string.Empty;

                                    foreach (PRLine line in prhead.PRLineList)
                                    {
                                        if (line != null)
                                        {
                                            line.Status = prhead.Status;
                                        }
                                    }

                                    session.Commit();
                                }
                            }
                        }
                    }
                    resultDataList.Add(resultData);
                }
                else
                {
                    throw new BusinessException("请购单下发失败!");
                }
            }
            return resultDataList;
        }

        private UFIDA.U9.ISV.PRSV.OtherSystemPRLineDTOData GetPRLineDTO(PRLine prline, UFIDA.U9.ISV.PRSV.OtherSystemPRDTOData prHeadDTO)
        {

            //// 4、pr转pr核算币种没带过去
            //if (prHeadDTO != null
            //    && prHeadDTO.AC == null
            //    )
            //{
            //    prHeadDTO.AC = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
            //    prHeadDTO.AC.Code = prline.PR.AC.Code;
            //}

            if (prline != null && prline.ItemInfo != null)
            {
                UFIDA.U9.ISV.PRSV.OtherSystemPRLineDTOData lineData = new UFIDA.U9.ISV.PRSV.OtherSystemPRLineDTOData();

                lineData.ItemInfo = new UFIDA.U9.CBO.SCM.Item.ItemInfoData();
                lineData.ItemInfo.ItemCode = prline.ItemInfo.ItemCode;
                lineData.ItemInfo.ItemName = prline.ItemInfo.ItemName;
                lineData.CurrentOrg = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                lineData.CurrentOrg.Code = prline.ReqDept.DescFlexField.PrivateDescSeg1;
                //if (prline.SeiBanMaster != null)
                if (prline.SeiBanMasterKey != null)
                {
                    lineData.SeiBan = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                    lineData.SeiBan.ID = prline.SeiBanMasterKey.ID;
                    lineData.SeiBan.Code = prline.SeiBanMaster.SeibanNO;
                }

                lineData.ReqUOM = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                lineData.ReqUOM.Code = prline.ReqUOM.Code;

                lineData.ReqQtyTU = prline.ReqQtyTU;

                lineData.RequiredDeliveryDate = prline.RequiredDeliveryDate;

                lineData.SuggestedSupplier = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                lineData.SuggestedSupplier.Code = prline.SuggestedSupplier.Code;
                lineData.SuggestedPrice = prline.SuggestedPrice;

                lineData.ReqDept = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                lineData.ReqDept.Code = prline.ReqDept.Code;

                lineData.RcvOrg = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                lineData.RcvOrg.Code = prline.RcvOrg.Code;

                lineData.AccountOrg = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                lineData.AccountOrg.Code = prline.AccountOrg.Code;

                if (prline.ReqEmployee != null)
                {
                    lineData.ReqEmployee = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                    lineData.ReqEmployee.Code = prline.ReqEmployee.Code;
                }

                string strOrgCode = string.Empty;
                string strRcvOrgCode = string.Empty;
                if (prline.ReqDept != null)
                {
                    strOrgCode = prline.ReqDept.DescFlexField.PrivateDescSeg1;//需求组织

                   if(String.IsNullOrEmpty(strOrgCode))
                   {
                       lineData.RegOrg = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                       lineData.RegOrg.Code = strOrgCode;
                   }
                }
                //{
                //    lineData.RcvOrg = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                //    //// 开票单位(私有段4)
                //    //lineData.RcvOrg.Code = cust.DescFlexField.PrivateDescSeg4;
                //    // 改为供应商的扩展字段6 = 收货组织
                //    lineData.RcvOrg.Code = prline.SuggestedSupplier.Supplier.DescFlexField.PrivateDescSeg6;

                //    // 货主组织 = 收货组织
                //    lineData.ShipperOrg = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                //    lineData.ShipperOrg.Code = lineData.RcvOrg.Code;

                //    //lineData.SuggestedSupplier.Code = prline.SuggestedSupplier.Code;
                //}

                // 来源请购行ID = 私有段10
                lineData.DescFlexSegments = new UFIDA.U9.Base.FlexField.DescFlexField.DescFlexSegmentsData();

                lineData.DescFlexSegments.PrivateDescSeg1 = prline.DescFlexSegments.PrivateDescSeg1;
                lineData.DescFlexSegments.PrivateDescSeg2 = prline.DescFlexSegments.PrivateDescSeg2;
                lineData.DescFlexSegments.PrivateDescSeg3 = prline.PR.ID.ToString();
                lineData.DescFlexSegments.PrivateDescSeg4 = prline.PR.DocNo.ToString();
                lineData.DescFlexSegments.PrivateDescSeg5 = prline.DescFlexSegments.PrivateDescSeg5;
                lineData.DescFlexSegments.PrivateDescSeg6 = prline.DescFlexSegments.PrivateDescSeg6;
                lineData.DescFlexSegments.PrivateDescSeg7 = prline.DescFlexSegments.PrivateDescSeg7;
                lineData.DescFlexSegments.PrivateDescSeg8 = prline.DescFlexSegments.PrivateDescSeg8;
                lineData.DescFlexSegments.PrivateDescSeg9 = prline.DescFlexSegments.PrivateDescSeg9;
                lineData.DescFlexSegments.PrivateDescSeg10 = prline.DescFlexSegments.PrivateDescSeg10;
                lineData.DescFlexSegments.PrivateDescSeg11 = prline.DescFlexSegments.PrivateDescSeg11;
                lineData.DescFlexSegments.PrivateDescSeg12 = prline.DescFlexSegments.PrivateDescSeg12;
                lineData.DescFlexSegments.PrivateDescSeg13 = prline.DescFlexSegments.PrivateDescSeg13;
                lineData.DescFlexSegments.PrivateDescSeg14 = prline.DescFlexSegments.PrivateDescSeg14;
                lineData.DescFlexSegments.PrivateDescSeg15 = prline.DescFlexSegments.PrivateDescSeg15;
                lineData.DescFlexSegments.PrivateDescSeg16 = prline.DescFlexSegments.PrivateDescSeg16;
                lineData.DescFlexSegments.PrivateDescSeg17 = prline.DescFlexSegments.PrivateDescSeg17;
                lineData.DescFlexSegments.PrivateDescSeg18 = prline.DescFlexSegments.PrivateDescSeg18;
                lineData.DescFlexSegments.PrivateDescSeg19 = prline.DescFlexSegments.PrivateDescSeg19;
                lineData.DescFlexSegments.PrivateDescSeg20 = prline.DescFlexSegments.PrivateDescSeg20;
                lineData.DescFlexSegments.PrivateDescSeg21 = prline.DescFlexSegments.PrivateDescSeg21;
                lineData.DescFlexSegments.PrivateDescSeg22 = prline.DescFlexSegments.PrivateDescSeg22;
                lineData.DescFlexSegments.PrivateDescSeg23 = prline.DescFlexSegments.PrivateDescSeg23;
                lineData.DescFlexSegments.PrivateDescSeg24 = prline.DescFlexSegments.PrivateDescSeg24;
                lineData.DescFlexSegments.PrivateDescSeg25 = prline.DescFlexSegments.PrivateDescSeg25;
                lineData.DescFlexSegments.PrivateDescSeg26 = prline.DescFlexSegments.PrivateDescSeg26;
                lineData.DescFlexSegments.PrivateDescSeg27 = prline.DescFlexSegments.PrivateDescSeg27;
                lineData.DescFlexSegments.PrivateDescSeg28 = prline.DescFlexSegments.PrivateDescSeg28;
                lineData.DescFlexSegments.PrivateDescSeg29 = prline.DescFlexSegments.PrivateDescSeg29;
                lineData.DescFlexSegments.PrivateDescSeg30 = prline.DescFlexSegments.PrivateDescSeg30;


                lineData.DescFlexSegments.PubDescSeg1 = prline.DescFlexSegments.PubDescSeg1;
                lineData.DescFlexSegments.PubDescSeg2 = prline.DescFlexSegments.PubDescSeg2;
                lineData.DescFlexSegments.PubDescSeg3 = prline.DescFlexSegments.PubDescSeg3;
                lineData.DescFlexSegments.PubDescSeg4 = prline.DescFlexSegments.PubDescSeg4;
                lineData.DescFlexSegments.PubDescSeg5 = prline.DescFlexSegments.PubDescSeg5;
                lineData.DescFlexSegments.PubDescSeg6 = prline.DescFlexSegments.PubDescSeg6;
                lineData.DescFlexSegments.PubDescSeg7 = prline.DescFlexSegments.PubDescSeg7;
                lineData.DescFlexSegments.PubDescSeg8 = prline.DescFlexSegments.PubDescSeg8;
                lineData.DescFlexSegments.PubDescSeg9 = prline.DescFlexSegments.PubDescSeg9;
                lineData.DescFlexSegments.PubDescSeg10 = prline.DescFlexSegments.PubDescSeg10;
                lineData.DescFlexSegments.PubDescSeg11 = prline.DescFlexSegments.PubDescSeg11;
                lineData.DescFlexSegments.PubDescSeg12 = prline.DescFlexSegments.PubDescSeg12;
                lineData.DescFlexSegments.PubDescSeg13 = prline.DescFlexSegments.PubDescSeg13;
                lineData.DescFlexSegments.PubDescSeg14 = prline.DescFlexSegments.PubDescSeg14;
                lineData.DescFlexSegments.PubDescSeg15 = prline.DescFlexSegments.PubDescSeg15;
                lineData.DescFlexSegments.PubDescSeg16 = prline.DescFlexSegments.PubDescSeg16;
                lineData.DescFlexSegments.PubDescSeg17 = prline.DescFlexSegments.PubDescSeg17;
                lineData.DescFlexSegments.PubDescSeg18 = prline.DescFlexSegments.PubDescSeg18;
                lineData.DescFlexSegments.PubDescSeg19 = prline.DescFlexSegments.PubDescSeg19;
                lineData.DescFlexSegments.PubDescSeg20 = prline.DescFlexSegments.PubDescSeg20;
                lineData.DescFlexSegments.PubDescSeg21 = prline.DescFlexSegments.PubDescSeg21;
                lineData.DescFlexSegments.PubDescSeg22 = prline.DescFlexSegments.PubDescSeg22;
                lineData.DescFlexSegments.PubDescSeg23 = prline.DescFlexSegments.PubDescSeg23;
                lineData.DescFlexSegments.PubDescSeg24 = prline.DescFlexSegments.PubDescSeg24;
                lineData.DescFlexSegments.PubDescSeg25 = prline.DescFlexSegments.PubDescSeg25;
                lineData.DescFlexSegments.PubDescSeg26 = prline.DescFlexSegments.PubDescSeg26;
                lineData.DescFlexSegments.PubDescSeg27 = prline.DescFlexSegments.PubDescSeg27;
                lineData.DescFlexSegments.PubDescSeg28 = prline.DescFlexSegments.PubDescSeg28;
                lineData.DescFlexSegments.PubDescSeg29 = prline.DescFlexSegments.PubDescSeg29;
                lineData.DescFlexSegments.PubDescSeg30 = prline.DescFlexSegments.PubDescSeg30;
                lineData.DescFlexSegments.PubDescSeg31 = prline.DescFlexSegments.PubDescSeg31;
                lineData.DescFlexSegments.PubDescSeg32 = prline.DescFlexSegments.PubDescSeg32;
                lineData.DescFlexSegments.PubDescSeg33 = prline.DescFlexSegments.PubDescSeg33;
                lineData.DescFlexSegments.PubDescSeg34 = prline.DescFlexSegments.PubDescSeg34;
                lineData.DescFlexSegments.PubDescSeg35 = prline.DescFlexSegments.PubDescSeg35;
                lineData.DescFlexSegments.PubDescSeg36 = prline.DescFlexSegments.PubDescSeg36;
                lineData.DescFlexSegments.PubDescSeg37 = prline.DescFlexSegments.PubDescSeg37;
                lineData.DescFlexSegments.PubDescSeg38 = prline.DescFlexSegments.PubDescSeg38;
                lineData.DescFlexSegments.PubDescSeg39 = prline.DescFlexSegments.PubDescSeg39;
                lineData.DescFlexSegments.PubDescSeg40 = prline.DescFlexSegments.PubDescSeg40;
                lineData.DescFlexSegments.PubDescSeg41 = prline.DescFlexSegments.PubDescSeg41;
                lineData.DescFlexSegments.PubDescSeg42 = prline.DescFlexSegments.PubDescSeg42;
                lineData.DescFlexSegments.PubDescSeg43 = prline.DescFlexSegments.PubDescSeg43;
                lineData.DescFlexSegments.PubDescSeg44 = prline.DescFlexSegments.PubDescSeg44;
                lineData.DescFlexSegments.PubDescSeg45 = prline.DescFlexSegments.PubDescSeg45;
                lineData.DescFlexSegments.PubDescSeg46 = prline.DescFlexSegments.PubDescSeg46;
                lineData.DescFlexSegments.PubDescSeg47 = prline.DescFlexSegments.PubDescSeg47;
                lineData.DescFlexSegments.PubDescSeg48 = prline.DescFlexSegments.PubDescSeg48;
                lineData.DescFlexSegments.PubDescSeg49 = prline.DescFlexSegments.PubDescSeg49;
                lineData.DescFlexSegments.PubDescSeg50 = prline.DescFlexSegments.PubDescSeg50;

                lineData.DescFlexSegments.ContextValue = prline.DescFlexSegments.ContextValue;
                lineData.DescFlexSegments.CombineName = prline.DescFlexSegments.CombineName;



                return lineData;
            }

            return null;
        }
	}

	#endregion
	
	
}