﻿namespace U9.VOB.Cus.HBHJianLiYuan
{
	using System;
	using System.Collections.Generic;
	using System.Text; 
	using UFSoft.UBF.AopFrame;	
	using UFSoft.UBF.Util.Context;
    using UFSoft.UBF.Business;
    using System.Data;
    using HBH.DoNet.DevPlatform.EntityMapping;
    using HBH.DoNet.DevPlatform.U9Mapping;
    using UFIDA.U9.ISV.RCV.DTO;
    using UFIDA.U9.CBO.SCM.Supplier;
    using UFSoft.UBF.PL;
    using UFIDA.U9.Base;
    using UFIDA.U9.PM.Pub;

	/// <summary>
	/// AQWTransToRcvSV partial 
	/// </summary>	
	public partial class AQWTransToRcvSV 
	{	
		internal BaseStrategy Select()
		{
			return new AQWTransToRcvSVImpementStrategy();	
		}		
	}
	
    //#region  implement strategy	
	/// <summary>
	/// Impement Implement
	/// 
	/// </summary>	
	internal partial class AQWTransToRcvSVImpementStrategy : BaseStrategy
	{
        private const string Const_RcvDocTypeCode = "RCV01";
        // 北京配送部
        private const string Const_SupplierCode = "00222";

		public AQWTransToRcvSVImpementStrategy() { }

		public override object Do(object obj)
		{						
			AQWTransToRcvSV bpObj = (AQWTransToRcvSV)obj;
			
			//get business operation context is as follows
			//IContext context = ContextManager.Context	
			
			//auto generating code end,underside is user custom code
			//and if you Implement replace this Exception Code...
            //throw new NotImplementedException();


            if (bpObj == null
                || bpObj.HeadIDs == null
                || bpObj.HeadIDs.Count == 0
                )
            {
                throw new BusinessException(string.Format("转单服务，参数不可为空!"));
            }

            DataTable dtID = new DataTable("HeadIDs");
            dtID.Columns.Add("ID", typeof(string));

            foreach (string id in bpObj.HeadIDs)
            {
                DataRow row = dtID.NewRow();

                row[0] = id;

                dtID.Rows.Add(row);
            }


            string strProcName = "HBH_SP_JianLiYuan_GetAQWRcvLineInfo";
            List<ParamDTO> lstParam = new List<ParamDTO>();
            {
                ParamDTO paramDTO = new ParamDTO();
                paramDTO.ParamName = "HeadIDs";
                paramDTO.ParamDirection = ParameterDirection.Input;
                paramDTO.ParamType = DbType.Xml;
                paramDTO.ParamValue = EntitySerialization.EntitySerial(dtID);

                lstParam.Add(paramDTO);
            }

            DataSet ds;
            EntityResult result = U9Helper.GetResultByProcess(strProcName, out ds, lstParam.ToArray());

            if (result != null)
            {
                if (result.Sucessfull)
                {
                    if (ds != null
                        )
                    {
                        List<AQWRcvDTO> lstRcvHead = AQWRcvDTO.GetAQWRcvByDataset(ds);

                        AQWRcvToErpRcv(lstRcvHead);
                    }
                }
            }

            return null;
		}

        private void AQWRcvToErpRcv(List<AQWRcvDTO> lstRcvHead)
        {
            UFIDA.U9.ISV.RCV.CreateRCVSRV creatRcv = new UFIDA.U9.ISV.RCV.CreateRCVSRV();

            creatRcv.RCVList = new List<UFIDA.U9.ISV.RCV.DTO.OBAReceivementDTO>();
            foreach (AQWRcvDTO aqwRcvDTO in lstRcvHead)
            {
                OBAReceivementDTO erpRcv = GetErpRcv(aqwRcvDTO);

                if (erpRcv != null)
                {
                    creatRcv.RCVList.Add(erpRcv);
                }
            }

            if (creatRcv.RCVList.Count > 0)
            {
                creatRcv.Do();
            }
        }

        private Supplier _rcvSupplier = null;
        private Supplier RcvSupplier
        {
            get
            {
                if (_rcvSupplier == null)
                {
                    _rcvSupplier = Supplier.Finder.Find("Org=@Org and Code=@Code"
                        , new OqlParam(Context.LoginOrg.ID)
                        , new OqlParam(Const_SupplierCode)
                        );

                    if (_rcvSupplier == null)
                    {
                        throw new BusinessException(string.Format("组织[{0}]下没有找到编码为[{1}]的供应商!"
                            , Context.LoginOrg.Name
                            , Const_SupplierCode
                            ));
                    }
                }
                return _rcvSupplier;
            }
        }

        private RcvDocType _rcvDocType = null;
        private RcvDocType RcvDocType
        {
            get
            {
                if (_rcvDocType == null)
                {
                    _rcvDocType = RcvDocType.Finder.Find("Org=@Org and Code=@Code"
                        , new OqlParam(Context.LoginOrg.ID)
                        , new OqlParam(Const_RcvDocTypeCode)
                        );

                    if (_rcvDocType == null)
                    {
                        throw new BusinessException(string.Format("组织[{0}]下没有找到编码为[{1}]的收货单单据类型!"
                            , Context.LoginOrg.Name
                            , Const_RcvDocTypeCode
                            ));
                    }
                }
                return _rcvDocType;
            }
        }

        private OBAReceivementDTO GetErpRcv(AQWRcvDTO aqwRcvDTO)
        {
            if (aqwRcvDTO != null
                && aqwRcvDTO.AQWRcvLineDTOs != null
                && aqwRcvDTO.AQWRcvLineDTOs.Count > 0
                )
            {
                StringBuilder sbError = new StringBuilder();

                OBAReceivementDTO erpRcvHead = new OBAReceivementDTO();

                RcvDocType rcvDocType = RcvDocType;
                if (rcvDocType != null)
                {
                    erpRcvHead.RcvDocType = new UFIDA.U9.PM.DTOs.BESimp4UIDTO();
                    erpRcvHead.RcvDocType.ID = rcvDocType.ID;
                    erpRcvHead.RcvDocType.Code = rcvDocType.Code;
                }

                erpRcvHead.BusinessDate = DateTime.Today;

                Supplier supt = RcvSupplier;

                if (supt != null)
                {
                    erpRcvHead.Supplier = new UFIDA.U9.CBO.SCM.Supplier.SupplierMISCInfo();
                    erpRcvHead.Supplier.Supplier = supt;
                }


                erpRcvHead.RcvFees = new List<OBARcvFeeDTO>();
                erpRcvHead.RcvDiscount = new List<OBARcvDiscountDTO>();
                erpRcvHead.RcvTax = new List<OBARcvTaxDTO>();
                erpRcvHead.RcvAddress = new List<OBARcvAddressDTO>();
                erpRcvHead.RcvContacts = new List<OBARcvContactDTO>();

                if (erpRcvHead.DescFlexField == null)
                {
                    erpRcvHead.DescFlexField = new UFIDA.U9.Base.FlexField.DescFlexField.DescFlexSegments();
                }
                erpRcvHead.DescFlexField.PrivateDescSeg1 = aqwRcvDTO.ldiid;
                erpRcvHead.DescFlexField.PrivateDescSeg2 = aqwRcvDTO.code;

                erpRcvHead.RcvLines = new List<OBARcvLineDTO>();
                foreach (AQWRcvLineDTO aqwRcvLineDTO in aqwRcvDTO.AQWRcvLineDTOs)
                {
                    if (aqwRcvLineDTO != null)
                    {
                        OBARcvLineDTO erpRcvLine = new OBARcvLineDTO();

                        erpRcvLine.ConfirmDate = aqwRcvDTO.arrivetime.GetDateTime(erpRcvHead.BusinessDate);

                        if (aqwRcvLineDTO.lgcode.IsNotNullOrWhiteSpace())
                        {
                            erpRcvLine.ItemInfo = new UFIDA.U9.CBO.SCM.Item.ItemInfo();
                            erpRcvLine.ItemInfo.ItemCode = aqwRcvLineDTO.lgcode;
                        }
                        else
                        {
                            sbError.AppendLine(string.Format("单据[{0}]货品[{1}]无法找到货品编码!"
                                , aqwRcvDTO.code
                                , aqwRcvLineDTO.lgid
                                ));
                            break;
                        }

                        erpRcvLine.ArriveQtyTU = aqwRcvLineDTO.amount.GetDecimal() + aqwRcvLineDTO.damount.GetDecimal();
                        erpRcvLine.FinallyPriceTC = aqwRcvLineDTO.uprice.GetDecimal();

                        //erpRcvLine.TotalMnyTC = aqwRcvLineDTO.total.GetDecimal();

                        erpRcvLine.RcvFees = new List<OBARcvFeeDTO>();
                        erpRcvLine.RcvDiscount = new List<OBARcvDiscountDTO>();
                        erpRcvLine.RcvTaxs = new List<OBARcvTaxDTO>();
                        erpRcvLine.RcvLineDispenses = new List<OBARcvLineDispenseDTO>();
                        erpRcvLine.RcvLineAllotMOs = new List<OBARcvLineAllotMODTO>();
                        erpRcvLine.RcvLineLocations = new List<OBARcvLineLocationDTO>();
                        erpRcvLine.RcvAddress = new List<OBARcvAddressDTO>();
                        erpRcvLine.RcvContacts = new List<OBARcvContactDTO>();
                        erpRcvLine.RcvSubLines = new List<OBARcvSubLineDTO>();


                        erpRcvHead.RcvLines.Add(erpRcvLine);
                    }
                }

                if (sbError.Length > 0)
                {
                    throw new BusinessException(sbError.ToString());
                }

                // 有行，才返回；没行没意义；
                if (erpRcvHead.RcvLines.Count > 0)
                {
                    return erpRcvHead;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }
	}

    //#endregion
	
	
}